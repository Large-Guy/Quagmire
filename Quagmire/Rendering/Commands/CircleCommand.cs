using System.Numerics;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering.Commands;

public class CircleCommand : IDrawCommand
{
    public float ZIndex { get; set; }
    public bool Fill;
    public Point Center;
    public float Radius;
    public Color Color;
    public float Arc;
    public float Origin;
    public void Draw()
    {
        //TODO: Reimplement
        SDL.SetRenderDrawColorFloat(Internals.Renderer, Color.R, Color.G, Color.B, Color.A);
        
        var segments = (int)(Arc * Radius);
        if (Fill)
        {
            var vertices = new SDL.Vertex[segments * 3];
            var v = 0;
            for (var i = 1; i < segments + 1; i++)
            {
                var pp = (float)(i - 1) / segments;
                var pr = pp * Arc + Origin;
                var cp = (float)i / segments;
                var cr = cp * Arc + Origin;
                var px = Center.X + Radius * MathF.Cos(pr);
                var py = Center.Y + Radius * MathF.Sin(pr);
                var cx = Center.X + Radius * MathF.Cos(cr);
                var cy = Center.Y + Radius * MathF.Sin(cr);
                vertices[v++] = new SDL.Vertex
                {
                    Position = new SDL.FPoint { X = Center.X, Y = Center.Y },
                    Color = new SDL.FColor(Color.R, Color.G, Color.B, Color.A)
                };
                vertices[v++] = new SDL.Vertex
                {
                    Position = new SDL.FPoint { X = px, Y = py },
                    Color = new SDL.FColor(Color.R, Color.G, Color.B, Color.A)
                };
                vertices[v++] = new SDL.Vertex
                {
                    Position = new SDL.FPoint { X = cx, Y = cy },
                    Color = new SDL.FColor(Color.R, Color.G, Color.B, Color.A)
                };
            }

            SDL.RenderGeometry(Internals.Renderer, IntPtr.Zero, vertices, segments * 3, IntPtr.Zero, 0);
        }
        else
        {
            for (var i = 1; i < segments + 1; i++)
            {
                var pp = (float)(i - 1) / segments;
                var pr = pp * 2 * MathF.PI;
                var cp = (float)i / segments;
                var cr = cp * 2 * MathF.PI;
                var px = Center.X + Radius * MathF.Cos(pr);
                var py = Center.Y + Radius * MathF.Sin(pr);
                var cx = Center.X + Radius * MathF.Cos(cr);
                var cy = Center.Y + Radius * MathF.Sin(cr);
                SDL.RenderLine(Internals.Renderer, px, py, cx, cy);
            }
        }
    }
}