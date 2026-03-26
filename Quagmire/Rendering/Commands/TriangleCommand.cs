using System.Numerics;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering.Commands;

public class TriangleCommand : IDrawCommand
{
    public float ZIndex { get; set; }
    public Color Color;
    public Point A, B, C;

    public void Draw()
    {
        SDL.Vertex[] verts = [
            new()
            {
                Color = new SDL.FColor(Color.R, Color.G, Color.B, Color.A),
                Position = new SDL.FPoint{X = A.X, Y = A.Y},
            },
            new()
            {
                Color = new SDL.FColor(Color.R, Color.G, Color.B, Color.A),
                Position = new SDL.FPoint{X = B.X, Y = B.Y},
            },
            new()
            {
                Color = new SDL.FColor(Color.R, Color.G, Color.B, Color.A),
                Position = new SDL.FPoint{X = C.X, Y = C.Y},
            }
        ];
        SDL.RenderGeometry(Internals.Renderer, IntPtr.Zero, verts, 3, IntPtr.Zero, 0);
    }
}