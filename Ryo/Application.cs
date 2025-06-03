using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Ryo;

public class Application : GameWindow, IGameEvents {
    private readonly Renderer _renderer;

    Event<IGameEvents.Load> IGameEvents.OnLoad { get; } = new();
    Event<IGameEvents.Update> IGameEvents.OnUpdate { get; } = new();
    Event<IGameEvents.Render> IGameEvents.OnRender { get; } = new();
    Event<IGameEvents.KeyDown> IGameEvents.OnKeyDown { get; } = new();
    Event<IGameEvents.KeyUp> IGameEvents.OnKeyUp { get; } = new();

    public Application(int width = 800, int height = 600) :
        base(GameWindowSettings.Default, new NativeWindowSettings()) {
        _renderer = new Renderer(this) {
            ScreenSize = new Vector2i(width, height)
        };
        this.Size = new Vector2i(width, height);
        this.CenterWindow();
        GL.Viewport(0, 0, width, height);
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        _renderer.ScreenSize = e.Size;
        GL.Viewport(0, 0, e.Size.X, e.Size.Y);
    }

    protected override void OnLoad() {
        base.OnLoad();
        GL.ClearColor(Color4.Magenta);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        (this as IGameEvents).OnLoad.Invoke(this, new IGameEvents.Load());
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);
        (this as IGameEvents).OnUpdate.InvokeParallel(this, new IGameEvents.Update(args.Time));
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _renderer.Rectangles.Add(
            new Renderer.Data(new Vector2(0, 0), new Vector2(512f, 343.5f), new Vector2(0, 0), new Vector2(1024, 687))
        );

        (this as IGameEvents).OnRender.Invoke(this, new IGameEvents.Render());

        this.SwapBuffers();
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        base.OnKeyDown(e);
        (this as IGameEvents).OnKeyDown.Invoke(this, new IGameEvents.KeyDown(new Utils.KeyData(e.Key, e.Modifiers())));
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e) {
        base.OnKeyUp(e);
        (this as IGameEvents).OnKeyUp.Invoke(this, new IGameEvents.KeyUp(new Utils.KeyData(e.Key, e.Modifiers())));
    }

    protected override void OnMouseDown(MouseButtonEventArgs e) {
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e) {
        base.OnMouseUp(e);
    }
}