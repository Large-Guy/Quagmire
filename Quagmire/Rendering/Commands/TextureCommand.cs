using System.Numerics;
using System.Runtime.InteropServices;
using Quagmire.Geometry;
using SDL3;
using Rectangle = Quagmire.Geometry.Rectangle;

namespace Quagmire.Rendering.Commands;

public class TextureCommand : IDrawCommand
{
    public float ZIndex { get; set; }
    public Texture? Texture;
    public Point Position;
    public Point Size;
    public Point Origin;
    public float Rotation;
    public Color Tint;
    public Geometry.Rectangle Src;
    public void Draw()
    {
        if (Texture == null)
            return;
        
        var tex = Texture.GetTexture();

        if (tex == IntPtr.Zero)
            return;

        var origin = Origin * Size;
        var pivot = new SDL.FPoint{X = origin.X, Y = origin.Y};
        var source = new SDL.FRect{X = Src.Position.X, Y = Src.Position.Y, W = Src.Size.X, H = Src.Size.Y};
        var dest = new SDL.FRect{X = Position.X - pivot.X, Y = Position.Y - pivot.Y, W = Size.X, H = Size.Y};
        var color = new SDL.FColor(Tint.R, Tint.G, Tint.B, Tint.A);
        SDL.SetTextureColorModFloat(Texture.GetTexture(), color.R, color.G, color.B);
        SDL.SetTextureAlphaModFloat(Texture.GetTexture(), color.A);
        SDL.RenderTextureRotated(Internals.Renderer, tex, source, dest, Trig.Degrees(Rotation), pivot, SDL.FlipMode.None);
        SDL.SetRenderDrawColorFloat(Internals.Renderer, 1f, 1f, 0f, 1f);
    }
}