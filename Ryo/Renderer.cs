using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Ryo;

public static class Renderer {
    public readonly record struct Data(
        Vector2 Position,
        Vector2 Size,
        Vector2 TexturePosition,
        Vector2 TextureSize
    );

    private static class Constants {
        internal const int MaxRectangles = 256;

        internal const int PositionComponentCount = 4;
        internal const int TextureComponentCount = 4;
        internal const int RectangleComponentCount = PositionComponentCount + TextureComponentCount;
        internal const int InstanceDataSize = RectangleComponentCount * sizeof(float) * MaxRectangles;

        internal const int VertexComponentCount = 2;
        internal const int VertexDataSize = 6 * VertexComponentCount * sizeof(float);
    }

    private static readonly List<Data> _rectangles = [];

    private static int _vao;
    private static int _vertexBuffer;
    private static int _instanceBuffer;
    private static int _shaderProgram;
    private static int _texture;
    private static int _imageUniformLocation;

    private static Vector2i _textureSize;
    private static Vector2i _screenSize;

    public static void Init() {
        _vao = GL.GenVertexArray();
        _vertexBuffer = GL.GenBuffer();
        _instanceBuffer = GL.GenBuffer();
        _shaderProgram = GL.CreateProgram();
        _texture = GL.GenTexture();
    }

    public static void Register(IGameEvents events) {
        events.OnLoad.Subscribe(OnLoad);
        events.OnRender.Subscribe(OnRender);
        events.OnResize.Subscribe(OnResize);
    }

    public static void Render(Data data) {
        _rectangles.Add(data);
    }

    private static void OnLoad(object sender, IGameEvents.Load args) {
        InitRectangle();
        _imageUniformLocation = InitShader();
        _textureSize = InitTexture();

        GL.ClearColor(Color4.Magenta);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private static void OnRender(object sender, IGameEvents.Render args) {
        var bufferData = new float[_rectangles.Count * Constants.RectangleComponentCount];
        var index = 0;
        for (var i = 0; i < _rectangles.Count; i++) {
            var data = _rectangles[i];
            bufferData[index++] = data.Position.X / _screenSize.X;
            bufferData[index++] = data.Position.Y / _screenSize.Y;
            bufferData[index++] = data.Size.X / _screenSize.X;
            bufferData[index++] = data.Size.Y / _screenSize.Y;
            bufferData[index++] = data.TexturePosition.X / _textureSize.X;
            bufferData[index++] = data.TexturePosition.Y / _textureSize.Y;
            bufferData[index++] = data.TextureSize.X / _textureSize.X;
            bufferData[index++] = data.TextureSize.Y / _textureSize.Y;
        }

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceBuffer);
        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, bufferData.Length * sizeof(float), bufferData);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        GL.BindTexture(TextureTarget.Texture2D, _texture);
        GL.ActiveTexture(TextureUnit.Texture0);

        GL.UseProgram(_shaderProgram);
        GL.Uniform1(_imageUniformLocation, 0);

        GL.BindVertexArray(_vao);

        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _rectangles.Count);
        GL.BindVertexArray(0);

        GL.UseProgram(0);

        _rectangles.Clear();
    }

    private static void OnResize(object sender, IGameEvents.Resize args) {
        _screenSize = args.NewSize;
        GL.Viewport(0, 0, _screenSize.X, _screenSize.Y);
    }

    #region Initialization Logic

    private static void InitRectangle() {
        GL.BindVertexArray(_vao);

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

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Constants.VertexDataSize, vertexData, BufferUsageHint.StaticDraw);


        GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, Constants.InstanceDataSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
    }

    private static void BindBuffers() {
        GL.BindVertexBuffer(0, _vertexBuffer, 0, Constants.VertexComponentCount * sizeof(float));

        GL.BindVertexBuffer(1, _instanceBuffer, 0, Constants.RectangleComponentCount * sizeof(float));
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

    private static int InitShader() {
        var vertexShaderData = File.ReadAllText("Shaders/Rectangle.vert");
        var fragmentShaderData = File.ReadAllText("Shaders/Rectangle.frag");

        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

        CompileShader(vertexShader, vertexShaderData, "Vertex Shader");
        CompileShader(fragmentShader, fragmentShaderData, "Fragment Shader");
        LinkProgram(_shaderProgram, vertexShader, fragmentShader);

        GL.DetachShader(_shaderProgram, vertexShader);
        GL.DetachShader(_shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return GL.GetUniformLocation(_shaderProgram, "image");
    }

    private static Vector2i InitTexture() {
        GL.BindTexture(TextureTarget.Texture2D, _texture);
        GL.ActiveTexture(TextureUnit.Texture0);

        float[] borderColor = [0.0f, 0.0f, 0.0f, 0.0f];

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        var image = ImageResult.FromStream(File.OpenRead("Assets/zortium.png"), ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            image.Width,
            image.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            image.Data
        );
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        return new Vector2i(image.Width, image.Height);
    }


    private static void CompileShader(int shader, string source, string context) {
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var compileStatus);

        if (compileStatus == (int)All.True) return;
        var infoLog = GL.GetShaderInfoLog(shader);
        throw new Exception($"Error while compiling shader ({context})\n{infoLog ?? ""}");
    }

    private static void LinkProgram(int program, int vertexShader, int fragmentShader) {
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var linkStatus);

        if (linkStatus == (int)All.True) return;
        var infoLog = GL.GetProgramInfoLog(program);
        throw new Exception($"Error while linking program\n{infoLog ?? ""}");
    }

    #endregion
}