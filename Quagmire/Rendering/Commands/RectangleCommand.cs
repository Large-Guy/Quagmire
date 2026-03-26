using System.Numerics;
using Quagmire.Geometry;
using SDL3;
using Rectangle = Quagmire.Geometry.Rectangle;

namespace Quagmire.Rendering.Commands;

public class RectangleCommand : IDrawCommand
{
    public float ZIndex { get; set; }
    
    public Color Color;
    public Rectangle Rectangle;
    public Point Origin;
    public float Rotation;

    public void Draw()
    {
        //TODO: Add support for rotating rects via a texture
        var origin = Origin * Rectangle.Size;
        SDL.SetRenderDrawColorFloat(Internals.Renderer, Color.R, Color.G, Color.B, Color.A);
        SDL.RenderFillRect(Internals.Renderer,
            new SDL.FRect
                { X = Rectangle.Position.X - origin.X, Y = Rectangle.Position.Y - origin.Y, W = Rectangle.Size.X, H = Rectangle.Size.Y });
    }
}