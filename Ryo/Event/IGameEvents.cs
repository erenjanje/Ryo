using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Ryo.Event;

public interface IGameEvents {
    #region Lifetime Events

    readonly record struct Load;
    Event<Load> OnLoad { get; }

    readonly record struct Close(Utils.ThreadSafeWrapper<bool> ShouldClose);
    Event<Close> OnClose { get; }

    readonly record struct Unload;
    Event<Unload> OnUnload { get; }

    #endregion

    #region Window Events

    record struct Resize(Vector2i NewSize);
    Event<Resize> OnResize { get; }

    #endregion

    #region Frame Events

    record struct Update(double DeltaTime);
    Event<Update> OnUpdate { get; }

    record struct Render(IRenderer Renderer);
    Event<Render> OnRender { get; }

    #endregion

    #region Keyboard Events

    record struct KeyDown(Keys Keys, KeyModifiers Modifiers);
    Event<KeyDown> OnKeyDown { get; }

    record struct KeyUp(Keys Keys, KeyModifiers Modifiers);
    Event<KeyUp> OnKeyUp { get; }

    #endregion

    #region Mouse Events

    record struct MouseUp(Vector2 MousePosition, MouseButton Button, KeyModifiers Modifiers);
    Event<MouseUp> OnMouseUp { get; }

    record struct MouseDown(Vector2 MousePosition, MouseButton Button, KeyModifiers Modifiers);
    Event<MouseDown> OnMouseDown { get; }

    record struct MouseMove(Vector2 NewPosition, Vector2 Delta);
    Event<MouseMove> OnMouseMove { get; }

    #endregion
}