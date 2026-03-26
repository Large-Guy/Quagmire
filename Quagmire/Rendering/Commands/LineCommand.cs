using System.Numerics;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering.Commands;

public class LineCommand : IDrawCommand
{
    public float ZIndex { get; set; }
    public Color Color;
    public Point A, B;
    public float Width;
    public void Draw()
    {
        SDL.SetRenderDrawColorFloat(Internals.Renderer, Color.R, Color.G, Color.B, Color.A);
        SDL.RenderLine(Internals.Renderer, A.X, A.Y, B.X, B.Y);
    }
}