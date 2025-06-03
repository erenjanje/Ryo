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
    );

    public interface IRequiredEvents
        : IEvent<GameEvents.Load>, IEvent<GameEvents.Render>, IEvent<GameEvents.Resize>;

    private static class Constants {
        internal const int MaxRectangles = 256;

        internal const int PositionComponentCount = 4;
        internal const int TextureComponentCount = 4;
        internal const int RectangleComponentCount = PositionComponentCount + TextureComponentCount;
        internal const int InstanceDataSize = RectangleComponentCount * sizeof(float) * MaxRectangles;

        internal const int VertexComponentCount = 2;
        internal const int VertexDataSize = 6 * VertexComponentCount * sizeof(float);
    }

    private static readonly List<Data> Rectangles = [];

    private static readonly int Vao = GL.GenVertexArray();
    private static readonly int VertexBuffer = GL.GenBuffer();
    private static readonly int InstanceBuffer = GL.GenBuffer();
    private static readonly Shader Shader = new("Shaders/Rectangle.vert", "Shaders/Rectangle.frag");
    private static readonly Texture Texture = new("Assets/zortium.png");
    private static readonly int ImageUniformLocation;

    private static Vector2i _screenSize;

    static Renderer() {
        Renderer.ImageUniformLocation = Renderer.Shader["image"];
    }

    public static void Register(IRequiredEvents events) {
        events.Event<GameEvents.Load>().Subscribe(OnLoad);
        events.Event<GameEvents.Render>().Subscribe(OnRender);
        events.Event<GameEvents.Resize>().Subscribe(OnResize);
    }

    public static void Render(Data data) {
        Rectangles.Add(data);
    }

    private static void OnLoad(object sender, GameEvents.Load args) {
        InitRectangle();

        GL.ClearColor(Color4.Magenta);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private static void OnRender(object sender, GameEvents.Render args) {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var bufferData = new float[Rectangles.Count * Constants.RectangleComponentCount];
        var index = 0;
        for (var i = 0; i < Rectangles.Count; i++) {
            var data = Rectangles[i];
            bufferData[index++] = data.Position.X / _screenSize.X;
            bufferData[index++] = data.Position.Y / _screenSize.Y;
            bufferData[index++] = data.Size.X / _screenSize.X;
            bufferData[index++] = data.Size.Y / _screenSize.Y;
            bufferData[index++] = data.TexturePosition.X / Texture.Size.X;
            bufferData[index++] = data.TexturePosition.Y / Texture.Size.Y;
            bufferData[index++] = data.TextureSize.X / Texture.Size.X;
            bufferData[index++] = data.TextureSize.Y / Texture.Size.Y;
        }

        var instanceCount = Rectangles.Count;
        Rectangles.Clear();

        GL.BindBuffer(BufferTarget.ArrayBuffer, InstanceBuffer);
        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, bufferData.Length * sizeof(float), bufferData);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        Shader.Use();
        Texture.Bind(0, ImageUniformLocation);

        GL.BindVertexArray(Vao);

        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, instanceCount);
        GL.BindVertexArray(0);
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