using OpenTK.Mathematics;

namespace Ryo;

public interface IGameEvents {
    public Event<Load> OnLoad { get; }
    public Event<Update> OnUpdate { get; }
    public Event<Render> OnRender { get; }
    public Event<KeyDown> OnKeyDown { get; }
    public Event<KeyUp> OnKeyUp { get; }
    public Event<Resize> OnResize { get; }

    public record struct Load;

    public record struct Update(double DeltaTime);

    public record struct Render;

    public record struct KeyUp(Utils.KeyData Key);

    public record struct KeyDown(Utils.KeyData Key);

    public record struct Resize(Vector2i NewSize);
}