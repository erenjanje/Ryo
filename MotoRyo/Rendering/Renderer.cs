using System.Runtime.CompilerServices;
using Ryo.MotoRyo.Event;
using Ryo.MotoRyo.Tiles;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Ryo.MotoRyo.Rendering;

internal static class Constants {
    internal const int MaxRectangles = 65536;

    internal const int PositionComponentCount = 4;
    internal const int TextureComponentCount = 4;
    internal const int RectangleComponentCount = PositionComponentCount + TextureComponentCount;
    internal const int InstanceDataSize = RectangleComponentCount * sizeof(float) * MaxRectangles;

    internal const int VertexComponentCount = 2;
    internal const int VertexDataSize = 6 * VertexComponentCount * sizeof(float);

    internal const int CellUnit = 16;
    internal static readonly Vector2i CellSize = new(CellUnit, CellUnit);
}

public sealed class Renderer : IRenderer {
    public HashSet<string> SupportedExtensions { get; } = [];

    // Buffers
    private readonly int _vao = GL.GenVertexArray();
    private readonly int _vertexBuffer = GL.GenBuffer();
    private readonly int _instanceBuffer = GL.GenBuffer();

    // Shader data
    private readonly Shader _shader = new();
    private int _imageUniformLocation;

    // Buffer and bookkeeping
    private readonly float[] _buffer =
        new float[Constants.MaxRectangles * Constants.RectangleComponentCount * sizeof(float)];
    private int _bufferIndex = 0;

    private Texture _texture = new();
    private Vector2i _screenSize = Vector2i.Zero;

    public void Register(IGameEvents events) {
        events.OnLoad.Subscribe(this.OnLoad);
        events.OnResize.Subscribe(this.OnResize);
    }

    public void Draw(Vector2 position, Vector2 size, Vector2 texturePosition, Vector2 textureSize) {
        _buffer[_bufferIndex++] = position.X / _screenSize.X;
        _buffer[_bufferIndex++] = position.Y / _screenSize.Y;
        _buffer[_bufferIndex++] = size.X / _screenSize.X;
        _buffer[_bufferIndex++] = size.Y / _screenSize.Y;
        _buffer[_bufferIndex++] = texturePosition.X / _texture.Size.X;
        _buffer[_bufferIndex++] = texturePosition.Y / _texture.Size.Y;
        _buffer[_bufferIndex++] = textureSize.X / _texture.Size.X;
        _buffer[_bufferIndex++] = textureSize.Y / _texture.Size.Y;
    }

    public void DrawTile(Vector2 position, Vector2i atlasPosition) =>
        Draw(position, Constants.CellSize, atlasPosition * Constants.CellSize, Constants.CellSize);

    public void DrawTile(Vector2 position, Vector2 size, Vector2i atlasPosition) =>
        Draw(position, size, atlasPosition * Constants.CellSize, Constants.CellSize);

    public void Render() {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var instanceCount = _bufferIndex / Constants.RectangleComponentCount;

        GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceBuffer);
        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _bufferIndex * sizeof(float), _buffer);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        _shader.Use();
        _texture.Bind(0, _imageUniformLocation);

        GL.BindVertexArray(_vao);

        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, instanceCount);
        GL.BindVertexArray(0);

        _bufferIndex = 0;
    }

    #region Events

    private void OnLoad(object sender, IGameEvents.Load args) {
        this.LoadSupportedExtensions();
        this.InitRectangle();

        _shader.LoadFromStreams(
            args.assetManager.LoadAssetTextStream("Shaders/Rectangle.vert"),
            args.assetManager.LoadAssetTextStream("Shaders/Rectangle.frag")
        );
        _texture.LoadFromStream(args.assetManager.LoadAssetStream("Map/TileMap.png"));
        _imageUniformLocation = _shader["image"];

        GL.ClearColor(Color4.Magenta);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private void OnResize(object sender, IGameEvents.Resize args) {
        _screenSize = args.NewSize;
        GL.Viewport(0, 0, _screenSize.X, _screenSize.Y);
    }

    #endregion

    #region Initialization Logic

    private void LoadSupportedExtensions() {
        var count = GL.GetInteger(GetPName.NumExtensions);
        for (var i = 0; i < count; i++) {
            var extension = GL.GetString(StringNameIndexed.Extensions, i);
            SupportedExtensions.Add(extension);
        }
        // Console.WriteLine($"[{SupportedExtensions.Aggregate((a, b) => $"{a}, {b}")}]");
    }

    private void InitRectangle() {
        GL.BindVertexArray(_vao);

        this.AllocateBuffers();
        this.BindBuffers();
        this.DeclareAttributes();

        GL.BindVertexArray(0);
    }

    private void AllocateBuffers() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        unsafe {
            var vertexData = stackalloc float[] {
                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,

                1.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 0.0f
            };
            GL.BufferData(BufferTarget.ArrayBuffer, Constants.VertexDataSize, new IntPtr(vertexData),
                BufferUsageHint.StaticDraw);
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Constants.InstanceDataSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
    }

    private void BindBuffers() {
        GL.BindVertexBuffer(0, _vertexBuffer, 0, Constants.VertexComponentCount * sizeof(float));

        GL.BindVertexBuffer(1, _instanceBuffer, 0, Constants.RectangleComponentCount * sizeof(float));
        GL.VertexBindingDivisor(1, 1);
    }

    private void DeclareAttributes() {
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribBinding(0, 0);
        GL.VertexAttribFormat(0, Constants.VertexComponentCount, VertexAttribType.Float, false, 0);

        var offset = 0;

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribBinding(1, 1);
        GL.VertexAttribFormat(1, Constants.PositionComponentCount, VertexAttribType.Float, false, offset);
        offset += Constants.PositionComponentCount * sizeof(float);

        GL.EnableVertexAttribArray(2);
        GL.VertexAttribBinding(2, 1);
        GL.VertexAttribFormat(2, Constants.TextureComponentCount, VertexAttribType.Float, false, offset);
        offset += Constants.TextureComponentCount * sizeof(float);
    }

    #endregion
}