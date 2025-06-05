namespace Ryo.Event;

public class GameEvents : IGameEvents {
    public Event<IGameEvents.Load> OnLoad { get; } = new();
    public Event<IGameEvents.Close> OnClose { get; } = new();
    public Event<IGameEvents.Unload> OnUnload { get; } = new();

    public Event<IGameEvents.Resize> OnResize { get; } = new();

    public Event<IGameEvents.Update> OnUpdate { get; } = new();
    public Event<IGameEvents.Render> OnRender { get; } = new();

    public Event<IGameEvents.KeyDown> OnKeyDown { get; } = new();
    public Event<IGameEvents.KeyUp> OnKeyUp { get; } = new();

    public Event<IGameEvents.MouseUp> OnMouseUp { get; } = new();
    public Event<IGameEvents.MouseDown> OnMouseDown { get; } = new();
    public Event<IGameEvents.MouseMove> OnMouseMove { get; } = new();
}