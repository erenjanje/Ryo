using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Ryo.MotoRyo.Rendering;

public record struct Texture(int Tex) {
    public Vector2i Size { get; private set; }

    public Texture() : this(GL.GenTexture()) { }

    public void LoadFromStream(Stream stream) {
        GL.BindTexture(TextureTarget.Texture2D, Tex);
        GL.ActiveTexture(TextureUnit.Texture0);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, 0.0f);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.NearestMipmapNearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        this.Size = this.LoadImage(stream);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Bind(int textureUnit, int uniform) {
        this.Bind(textureUnit);
        GL.Uniform1(uniform, textureUnit);
    }

    public void Bind(int textureUnit) {
        GL.BindTexture(TextureTarget.Texture2D, Tex);
        GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
    }

    private unsafe Vector2i LoadImage(Stream file) {
        var pointer = (byte*)null;
        int width;
        int height;

        try {
            int componentCount;
            pointer = StbImage.stbi__load_and_postprocess_8bit(new StbImage.stbi__context(file), &width, &height,
                &componentCount, 4);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                width,
                height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                (IntPtr)pointer
            );
        } finally {
            if ((IntPtr)pointer != IntPtr.Zero) {
                Marshal.FreeHGlobal((IntPtr)pointer);
            }
        }

        return (width, height);
    }
}