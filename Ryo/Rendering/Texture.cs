using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Ryo.Rendering;

public readonly record struct Texture(int Tex) {
    public Vector2i Size { get; private init; }

    public Texture(string file) : this(GL.GenTexture()) {
        GL.BindTexture(TextureTarget.Texture2D, Tex);
        GL.ActiveTexture(TextureUnit.Texture0);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, 0.0f);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        var image = ImageResult.FromStream(File.OpenRead(file), ColorComponents.RedGreenBlueAlpha);

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

        this.Size = new Vector2i(image.Width, image.Height);
    }

    public void Bind(int textureUnit, int uniform) {
        this.Bind(textureUnit);
        GL.Uniform1(uniform, textureUnit);
    }

    public void Bind(int textureUnit) {
        GL.BindTexture(TextureTarget.Texture2D, Tex);
        GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
    }
}