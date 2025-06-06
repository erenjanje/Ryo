using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace Ryo.MotoRyo.Rendering;

public readonly record struct Shader(int Program) {
    public Shader() : this(GL.CreateProgram()) { }

    public void LoadFromStreams(StreamReader vertexShaderStream, StreamReader fragmentShaderStream) {
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

        Compile(vertexShader, vertexShaderStream.ReadToEnd(), "Vertex Shader");
        Compile(fragmentShader, fragmentShaderStream.ReadToEnd(), "Fragment Shader");
        Link(Program, vertexShader, fragmentShader);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use() => GL.UseProgram(Program);

    public int this[string variable] => GL.GetUniformLocation(Program, variable);

    private static void Compile(int shader, string source, string context) {
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var compileStatus);

        if (compileStatus == (int)All.True) return;
        var infoLog = GL.GetShaderInfoLog(shader);
        throw new Exception($"Error while compiling shader ({context})\n{infoLog ?? ""}");
    }

    private static void Link(int program, int vertexShader, int fragmentShader) {
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);

        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var linkStatus);

        if (linkStatus != (int)All.True) {
            var infoLog = GL.GetProgramInfoLog(program);
            throw new Exception($"Error while linking program\n{infoLog ?? ""}");
        }

        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);
    }
}