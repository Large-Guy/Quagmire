using Quagmire.Geometry;

namespace Quagmire.Rendering;

public static class World
{
    public static Point Scale = new Point(1f);
    public static Color Background = Color.Black;

    public static Point ToWorld(Point screen)
    {
        var cameraPosition = (Draw.Camera?.Position ?? new Point()) * Scale;
        var pivotOffset = Draw.Camera?.Pivot * Draw.Size ?? new Point();

        var world = (screen - cameraPosition - pivotOffset) / Scale;
        return world;
    }
}