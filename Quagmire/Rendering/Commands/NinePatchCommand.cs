using System.Numerics;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering.Commands;

public class NinePatchCommand : IDrawCommand
{
    public float ZIndex { get; set; }
    public NinePatch? Texture;
    public Point Position;
    public Point Size;
    public Color Tint;
    public void Draw()
    {
        var texture = Texture.Texture;
        var center = Texture.Center;
        var texture2D = texture.GetTexture();
        if (texture2D == IntPtr.Zero)
            return;
        
        var rect = texture.GetSource();
        var source = new SDL.FRect{X = rect.Position.X, Y = rect.Position.Y, W = rect.Size.X, H = rect.Size.Y};

        var dest = new SDL.FRect{X = Position.X, Y = Position.Y, W = Size.X, H = Size.Y};
        
        //SDL.RenderTexture(Internals.Renderer, texture2D, source, dest);

        SDL.SetTextureColorModFloat(texture.GetTexture(), Tint.R, Tint.G, Tint.B);
        SDL.SetTextureAlphaModFloat(texture.GetTexture(), Tint.A);

        var left = center.Position.X;
        var right = rect.Size.X - (center.Position.X + center.Size.X);
        var top = center.Position.Y;
        var bottom = rect.Size.Y - (center.Position.Y + center.Size.Y);
        
        SDL.RenderTexture9Grid(Internals.Renderer, texture2D,
            source,
            left,
            right,
            top,
            bottom,
            0f,
            dest
        );

        //SDL.RenderTexture(Internals.Renderer, texture.GetTexture(), source, dest);
    }
}