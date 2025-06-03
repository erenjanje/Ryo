using System.ComponentModel;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Ryo;

public sealed class Application : GameWindow {
    public Application(int width = 800, int height = 600) :
        base(GameWindowSettings.Default, new NativeWindowSettings()) {
        GL.LoadBindings(new GLFWBindingsContext()); // In NativeAot, bindings are not loaded by default

        this.Size = new Vector2i(width, height);
        this.CenterWindow();

        Renderer.Register(GameEvents.Instance);
    }

    #region Lifetime Events

    protected override void OnLoad() {
        base.OnLoad();
        GameEvents.Instance.Event<GameEvents.Load>().Invoke(this, new());
    }

    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        var shouldClose = new Utils.ThreadSafeWrapper<bool>(e.Cancel);
        GameEvents.Instance.Event<GameEvents.Close>().Invoke(this, new(shouldClose));
        e.Cancel = shouldClose.Value;
    }

    protected override void OnUnload() {
        base.OnUnload();
        GameEvents.Instance.Event<GameEvents.Unload>().Invoke(this, new());
    }

    #endregion

    #region Window Events

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        GameEvents.Instance.Event<GameEvents.Resize>().Invoke(this, new(e.Size));
    }

    #endregion

    #region Frame Events

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);
        Renderer.Render(
            new Renderer.Data(new Vector2(0, 0), new Vector2(512f, 343.5f), new Vector2(0, 0), new Vector2(1024, 687))
        );
        GameEvents.Instance.Event<GameEvents.Update>().InvokeParallel(this, new(args.Time));
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);
        GameEvents.Instance.Event<GameEvents.Render>().Invoke(this, new());
        this.SwapBuffers();
    }

    #endregion

    #region Keyboard Events

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        base.OnKeyDown(e);
        GameEvents.Instance.Event<GameEvents.KeyDown>().Invoke(this, new(e.Key, e.Modifiers));
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e) {
        base.OnKeyUp(e);
        GameEvents.Instance.Event<GameEvents.KeyUp>().Invoke(this, new(e.Key, e.Modifiers));
    }

    #endregion

    #region Mouse Events

    protected override void OnMouseDown(MouseButtonEventArgs e) {
        base.OnMouseDown(e);
        GameEvents.Instance.Event<GameEvents.MouseDown>().Invoke(this, new(this.MousePosition, e.Button, e.Modifiers));
    }

    protected override void OnMouseUp(MouseButtonEventArgs e) {
        base.OnMouseUp(e);
        GameEvents.Instance.Event<GameEvents.MouseUp>().Invoke(this, new(this.MousePosition, e.Button, e.Modifiers));
    }

    protected override void OnMouseMove(MouseMoveEventArgs e) {
        base.OnMouseMove(e);
        GameEvents.Instance.Event<GameEvents.MouseMove>().Invoke(this, new(e.Position, e.Delta));
    }

    #endregion
}