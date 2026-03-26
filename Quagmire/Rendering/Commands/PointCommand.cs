using System.Numerics;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering.Commands;

public class PointCommand : IDrawCommand
{
    public float ZIndex { get; set; }
    public Point Point;
    public Color Color;
    public void Draw()
    {
        SDL.SetRenderDrawColorFloat(Internals.Renderer, Color.R, Color.G, Color.B, Color.A);
        SDL.RenderPoint(Internals.Renderer, Point.X, Point.Y);
    }
}