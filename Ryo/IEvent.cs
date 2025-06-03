namespace Ryo;

public interface IEvent<T> where T : struct {
    public Event<T> On { get; }
}