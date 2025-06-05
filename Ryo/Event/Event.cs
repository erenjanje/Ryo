using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace Ryo.Event;

public class Event<TArg> where TArg : struct {
    public delegate void Handler(object sender, TArg arg);

    private readonly List<Handler> _handlers = [];
    private readonly ConcurrentBag<Handler> _newHandlers = [];
    private readonly ConcurrentBag<Handler> _deletedHandlers = [];
    private readonly CountdownEvent _countdown = new(0);

    public Event() { }

    public void Subscribe(Handler handler) {
        _newHandlers.Add(handler);
    }

    public void Unsubscribe(Handler handler) {
        _deletedHandlers.Add(handler);
    }

    public void Invoke(object sender, TArg arg) {
        this.ApplyPendingOperations();
        for (var i = 0; i < _handlers.Count; ++i) {
            var handler = _handlers[i];
            handler(sender, arg);
        }
    }

    public void InvokeParallel(object sender, TArg arg) {
        this.ApplyPendingOperations();
        _countdown.Reset(_handlers.Count);

        for (var i = 0; i < _handlers.Count; i++) {
            ThreadPool.QueueUserWorkItem(
                static state => {
                    var (handler, countdown, sender, arg) = state;
                    handler.Invoke(sender, arg);
                    countdown.Signal();
                },
                (_handlers[i], _countdown, sender, arg),
                preferLocal: false);
        }

        _countdown.Wait();
    }

    private void ApplyPendingOperations() {
        if (!_newHandlers.IsEmpty) {
            _handlers.AddRange(_newHandlers);
            _newHandlers.Clear();
        }

        if (!_deletedHandlers.IsEmpty) {
            var deletedHandlers = _deletedHandlers.ToFrozenSet();
            _handlers.RemoveAll(h => deletedHandlers.Contains(h));
            _deletedHandlers.Clear();
        }
    }
}