using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace Ryo;

public readonly struct Event<TArg> where TArg : struct {
    public delegate void Handler(object sender, TArg arg);

    private readonly List<Handler> _handlers = [];
    private readonly ConcurrentBag<Handler> _newHandlers = [];
    private readonly ConcurrentBag<Handler> _deletedHandlers = [];

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
        using var countdownEvent = new CountdownEvent(_handlers.Count);

        for (var i = 0; i < _handlers.Count; i++) {
            var handler = _handlers[i];
            ThreadPool.QueueUserWorkItem(_ => {
                handler(sender, arg);
                countdownEvent.Signal();
            });
        }

        countdownEvent.Wait();
    }

    private void ApplyPendingOperations() {
        _handlers.AddRange(_newHandlers);
        _newHandlers.Clear();

        var deletedHandlers = _deletedHandlers.ToFrozenSet();
        _handlers.RemoveAll(h => deletedHandlers.Contains(h));
        _deletedHandlers.Clear();
    }
}