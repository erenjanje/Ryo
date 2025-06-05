namespace Ryo.Event;

public interface IEvent<T> where T : struct {
    Event<T> On { get; }
}