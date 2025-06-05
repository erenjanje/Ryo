using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Ryo.Event;

public class GameEvents {
    public GameEvents() { }

    #region Lifetime Events

    public readonly record struct Load;
    public Event<Load> OnLoad { get; } = new();

    public readonly record struct Close(Utils.ThreadSafeWrapper<bool> ShouldClose);
    public Event<Close> OnClose { get; } = new();

    public readonly record struct Unload;
    public Event<Unload> OnUnload { get; } = new();

    #endregion

    #region Window Events

    public record struct Resize(Vector2i NewSize);
    public Event<Resize> OnResize { get; } = new();

    #endregion

    #region Frame Events

    public record struct Update(double DeltaTime);
    public Event<Update> OnUpdate { get; } = new();

    public record struct Render(IRenderer Renderer);
    public Event<Render> OnRender { get; } = new();

    #endregion

    #region Keyboard Events

    public record struct KeyDown(Keys Keys, KeyModifiers Modifiers);
    public Event<KeyDown> OnKeyDown { get; } = new();


    public record struct KeyUp(Keys Keys, KeyModifiers Modifiers);
    public Event<KeyUp> OnKeyUp { get; } = new();

    #endregion

    #region Mouse Events

    public record struct MouseUp(Vector2 MousePosition, MouseButton Button, KeyModifiers Modifiers);
    public Event<MouseUp> OnMouseUp { get; } = new();


    public record struct MouseDown(Vector2 MousePosition, MouseButton Button, KeyModifiers Modifiers);
    public Event<MouseDown> OnMouseDown { get; } = new();


    public record struct MouseMove(Vector2 NewPosition, Vector2 Delta);
    public Event<MouseMove> OnMouseMove { get; } = new();

    #endregion
}