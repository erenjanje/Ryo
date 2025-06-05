using System.ComponentModel;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Ryo;

public sealed class Application : GameWindow {
    public Application(int width = 1280, int height = 960) :
        base(GameWindowSettings.Default, new NativeWindowSettings()) {
        GL.LoadBindings(new GLFWBindingsContext()); // In NativeAot, bindings are not loaded by default

        _events = new GameEvents();
        _renderer = new Renderer();
        _tileMap = new TileMap(80, 60);

        _renderer.Register(_events);
        _tileMap.Register(_events);

        this.Size = new Vector2i(width, height);
        this.CenterWindow();
    }

    private readonly IGameEvents _events;
    private readonly IRenderer _renderer;
    private readonly ITileMap _tileMap;

    #region Events

    #region Lifetime Events

    protected override void OnLoad() {
        base.OnLoad();
        _events.OnLoad.Invoke(this, new());
    }

    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        var shouldClose = new Utils.ThreadSafeWrapper<bool>(e.Cancel);
        _events.OnClose.Invoke(this, new(shouldClose));
        e.Cancel = shouldClose.Value;
    }

    protected override void OnUnload() {
        base.OnUnload();
        _events.OnUnload.Invoke(this, new());
    }

    #endregion

    #region Window Events

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        _events.OnResize.Invoke(this, new(e.Size));
    }

    #endregion

    #region Frame Events

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);
        _events.OnUpdate.InvokeParallel(this, new(args.Time));
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);
        _events.OnRender.Invoke(this, new(_renderer));
        _renderer.Render();
        this.SwapBuffers();
    }

    #endregion

    #region Keyboard Events

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        base.OnKeyDown(e);
        _events.OnKeyDown.Invoke(this, new(e.Key, e.Modifiers));
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e) {
        base.OnKeyUp(e);
        _events.OnKeyUp.Invoke(this, new(e.Key, e.Modifiers));
    }

    #endregion

    #region Mouse Events

    protected override void OnMouseDown(MouseButtonEventArgs e) {
        base.OnMouseDown(e);
        _events.OnMouseDown.Invoke(this, new(this.MousePosition, e.Button, e.Modifiers));
    }

    protected override void OnMouseUp(MouseButtonEventArgs e) {
        base.OnMouseUp(e);
        _events.OnMouseUp.Invoke(this, new(this.MousePosition, e.Button, e.Modifiers));
    }

    protected override void OnMouseMove(MouseMoveEventArgs e) {
        base.OnMouseMove(e);
        _events.OnMouseMove.Invoke(this, new(e.Position, e.Delta));
    }

    #endregion

    #endregion
}