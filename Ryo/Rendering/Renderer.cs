using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;
using Ryo;

namespace Ryo.Rendering;

public static class Renderer {
    public readonly record struct Data(
        Vector2 Position,
        Vector2 Size,
        Vector2 TexturePosition,
        Vector2 TextureSize
    ) {
        public Data(Vector2 position, Vector2 size) : this(position, size, Vector2.Zero, size) { }
    }

    public interface IRequiredEvents
        : IEvent<GameEvents.Load>, IEvent<GameEvents.Resize>;

    private static class Constants {
        internal const int MaxRectangles = 65536;

        internal const int PositionComponentCount = 4;
        internal const int TextureComponentCount = 4;
        internal const int RectangleComponentCount = PositionComponentCount + TextureComponentCount;
        internal const int InstanceDataSize = RectangleComponentCount * sizeof(float) * MaxRectangles;

        internal const int VertexComponentCount = 2;
        internal const int VertexDataSize = 6 * VertexComponentCount * sizeof(float);
    }

    private static readonly int Vao = GL.GenVertexArray();
    private static readonly int VertexBuffer = GL.GenBuffer();
    private static readonly int InstanceBuffer = GL.GenBuffer();
    private static readonly Shader Shader = new("Shaders/Rectangle.vert", "Shaders/Rectangle.frag");
    private static readonly Texture Texture = new("Assets/TileMap.png");
    private static readonly int ImageUniformLocation;

    private static readonly float[] Buffer =
        new float[Constants.MaxRectangles * Constants.RectangleComponentCount * sizeof(float)];

    private static int _bufferIndex = 0;

    private static Vector2i _screenSize;

    static Renderer() {
        Renderer.ImageUniformLocation = Renderer.Shader["image"];
    }

    public static void Register(IRequiredEvents events) {
        events.Event<GameEvents.Load>().Subscribe(OnLoad);
        events.Event<GameEvents.Resize>().Subscribe(OnResize);
    }

    public static void Draw(Data data) {
        Buffer[_bufferIndex++] = data.Position.X / _screenSize.X;
        Buffer[_bufferIndex++] = data.Position.Y / _screenSize.Y;
        Buffer[_bufferIndex++] = data.Size.X / _screenSize.X;
        Buffer[_bufferIndex++] = data.Size.Y / _screenSize.Y;
        Buffer[_bufferIndex++] = data.TexturePosition.X / Texture.Size.X;
        Buffer[_bufferIndex++] = data.TexturePosition.Y / Texture.Size.Y;
        Buffer[_bufferIndex++] = data.TextureSize.X / Texture.Size.X;
        Buffer[_bufferIndex++] = data.TextureSize.Y / Texture.Size.Y;
    }

    private static void OnLoad(object sender, GameEvents.Load args) {
        InitRectangle();

        GL.ClearColor(Color4.Magenta);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    public static void Render() {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var instanceCount = _bufferIndex / Constants.RectangleComponentCount;

        GL.BindBuffer(BufferTarget.ArrayBuffer, InstanceBuffer);
        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _bufferIndex * sizeof(float), Buffer);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        Shader.Use();
        Texture.Bind(0, ImageUniformLocation);

        GL.BindVertexArray(Vao);

        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, instanceCount);
        GL.BindVertexArray(0);

        _bufferIndex = 0;
    }

    private static void OnResize(object sender, GameEvents.Resize args) {
        _screenSize = args.NewSize;
        GL.Viewport(0, 0, _screenSize.X, _screenSize.Y);
    }

    #region Initialization Logic

    private static void InitRectangle() {
        GL.BindVertexArray(Vao);

        AllocateBuffers();
        BindBuffers();
        DeclareAttributes();

        GL.BindVertexArray(0);
    }

    private static void AllocateBuffers() {
        float[] vertexData = [
            0.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f,

            1.0f, 1.0f,
            0.0f, 1.0f,
            0.0f, 0.0f
        ];

        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Constants.VertexDataSize, vertexData, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ArrayBuffer, InstanceBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Constants.InstanceDataSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
    }

    private static void BindBuffers() {
        GL.BindVertexBuffer(0, VertexBuffer, 0, Constants.VertexComponentCount * sizeof(float));

        GL.BindVertexBuffer(1, InstanceBuffer, 0, Constants.RectangleComponentCount * sizeof(float));
        GL.VertexBindingDivisor(1, 1);
    }

    private static void DeclareAttributes() {
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