using System.Runtime.CompilerServices;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Ryo;

public record GameEvents
    : IEvent<GameEvents.Load>,
      IEvent<GameEvents.Close>,
      IEvent<GameEvents.Unload>,
      IEvent<GameEvents.Resize>,
      IEvent<GameEvents.Update>,
      IEvent<GameEvents.Render>,
      IEvent<GameEvents.KeyUp>,
      IEvent<GameEvents.KeyDown>,
      IEvent<GameEvents.MouseUp>,
      IEvent<GameEvents.MouseDown>,
      IEvent<GameEvents.MouseMove>,
      Renderer.IRequiredEvents,
      TileMap.IRequiredEvents {
    public static GameEvents Instance { get; } = new();

    private GameEvents() { }

    #region Lifetime Events

    public record struct Load;

    Event<Load> IEvent<Load>.On { get; } = new();


    public record struct Close(Utils.ThreadSafeWrapper<bool> ShouldClose);

    Event<Close> IEvent<Close>.On { get; } = new();

    public record struct Unload;

    Event<Unload> IEvent<Unload>.On { get; } = new();

    #endregion

    #region Window Events

    public record struct Resize(Vector2i NewSize);

    Event<Resize> IEvent<Resize>.On { get; } = new();

    #endregion

    #region Frame Events

    public record struct Update(double DeltaTime);

    Event<Update> IEvent<Update>.On { get; } = new();

    public record struct Render;

    Event<Render> IEvent<Render>.On { get; } = new();

    #endregion

    #region Keyboard Events

    public record struct KeyDown(Keys Keys, KeyModifiers Modifiers);

    Event<KeyDown> IEvent<KeyDown>.On { get; } = new();


    public record struct KeyUp(Keys Keys, KeyModifiers Modifiers);

    Event<KeyUp> IEvent<KeyUp>.On { get; } = new();

    #endregion

    #region Mouse Events

    public record struct MouseUp(Vector2 MousePosition, MouseButton Button, KeyModifiers Modifiers);

    Event<MouseUp> IEvent<MouseUp>.On { get; } = new();


    public record struct MouseDown(Vector2 MousePosition, MouseButton Button, KeyModifiers Modifiers);

    Event<MouseDown> IEvent<MouseDown>.On { get; } = new();


    public record struct MouseMove(Vector2 NewPosition, Vector2 Delta);

    Event<MouseMove> IEvent<MouseMove>.On { get; } = new();

    #endregion
}