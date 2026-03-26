using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering;

public class RenderTexture2D : Texture2D
{
    public RenderTexture2D(string path) : base(path)
    {
        throw new Exception("Don't initialize render target with a Path");
    }

    public RenderTexture2D(int width, int height) : base(width, height)
    {
        InternalTexture = SDL.CreateTexture(Internals.Renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Target,
            width, height);
        Source = new Rectangle(0, 0, width, height);
    }
}