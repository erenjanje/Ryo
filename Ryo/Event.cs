namespace Ryo;

public readonly struct Event<TArg> where TArg : struct {
    public delegate void Handler(object sender, TArg arg);

    private readonly List<Handler> _handlers = [];

    public Event() { }

    public void Subscribe(Handler handler) {
        _handlers.Add(handler);
    }

    public void Unsubscribe(Handler handler) {
        _handlers.Remove(handler);
    }

    public void Invoke(object sender, TArg arg) {
        foreach (var handler in _handlers) {
            handler(sender, arg);
        }
    }

    public void InvokeParallel(object sender, TArg arg) {
        using var countdownEvent = new CountdownEvent(_handlers.Count);

        foreach (var handler in _handlers) {
            ThreadPool.QueueUserWorkItem(_ => {
                handler(sender, arg);
                countdownEvent.Signal();
            });
        }

        countdownEvent.Wait();
    }
}