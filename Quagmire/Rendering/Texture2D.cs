using Quagmire.Assets;
using SDL3;
using Rectangle = Quagmire.Geometry.Rectangle;

namespace Quagmire.Rendering;

public class Texture2D : Texture
{
    public Texture2D(string path) : base(path)
    {
        var surface = SDL3.Image.Load(path);
        InternalTexture = SDL.CreateTextureFromSurface(Internals.Renderer, surface);
        SDL.SetTextureScaleMode(InternalTexture, SDL.ScaleMode.Nearest);
        SDL.DestroySurface(surface);

        SDL.GetTextureSize(InternalTexture, out var w, out var h);

        Source = new Rectangle(0, 0, w, h);
    }

    public Texture2D(int width, int height)
    {
        InternalTexture = SDL.CreateTexture(Internals.Renderer, SDL.PixelFormat.ABGR8888, SDL.TextureAccess.Static,
            width, height);
        Source = new Rectangle(0, 0, width, height);
    }
    
    public Texture2D(int width, int height, byte[] data)
    {
        InternalTexture = SDL.CreateTexture(Internals.Renderer, SDL.PixelFormat.ABGR8888, SDL.TextureAccess.Static,
            width, height);
        SDL.UpdateTexture(InternalTexture, IntPtr.Zero, data, 4 * width);
        
        Source = new Rectangle(0, 0, width, height);
    }
}