using Quagmire.Geometry;

namespace Quagmire.Rendering;

public class Camera
{
    public Point Position;
    public Point Pivot;

    public Camera()
    {
        Position = new Point(0, 0);
        Pivot = new Point(0.5f);
    }
    
    public Camera(Point position, Point? pivot = null)
    {
        Position = position;
        Pivot = pivot != null ? pivot.Value : new Point(0.5f);
    }
}