using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace OpenDan.Graphics;

public sealed class Texture2D : IDisposable
{
    public int Handle { get; }
    public int Width { get; }
    public int Height { get; }

    private Texture2D(int handle, int width, int height)
    {
        Handle = handle;
        Width = width;
        Height = height;
    }

    public static Texture2D LoadFromFile(string path, bool generateMipmaps)
    {
        using var stream = File.OpenRead(path);
        var img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        return FromPixels(img.Width, img.Height, img.Data, generateMipmaps);
    }

    public static Texture2D FromCheckerboard(int w, int h)
    {
        var data = new byte[w * h * 4];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                bool white = ((x ^ y) & 1) == 0;
                int i = (y * w + x) * 4;
                data[i + 0] = white ? (byte)240 : (byte)40;
                data[i + 1] = white ? (byte)240 : (byte)40;
                data[i + 2] = white ? (byte)240 : (byte)40;
                data[i + 3] = 255;
            }
        }
        return FromPixels(w, h, data, true);
    }

    public static Texture2D FromPixels(int width, int height, byte[] rgba, bool generateMipmaps)
    {
        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
            width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, rgba);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)(generateMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        if (generateMipmaps)
        {
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        return new Texture2D(handle, width, height);
    }

    public void Bind(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose()
    {
        GL.DeleteTexture(Handle);
    }
}
