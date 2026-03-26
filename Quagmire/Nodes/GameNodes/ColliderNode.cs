using Quagmire.Geometry;

namespace Quagmire.Nodes.GameNodes;

public class ColliderNode : GameNode
{
    public Polygon? Polygon = null;

    public Polygon? GlobalPolygon => Polygon?.Offset(GlobalPosition);

    public ColliderNode(Polygon? polygon = null)
    {
        Polygon = polygon;
    }
}