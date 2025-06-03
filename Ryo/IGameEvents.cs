namespace Ryo;

public interface IGameEvents {
    public Event<EventArgs.Load> OnLoad { get; }
    public Event<EventArgs.Update> OnUpdate { get; }
    public Event<EventArgs.Render> OnRender { get; }
    public Event<EventArgs.KeyDown> OnKeyDown { get; }
    public Event<EventArgs.KeyUp> OnKeyUp { get; }
}