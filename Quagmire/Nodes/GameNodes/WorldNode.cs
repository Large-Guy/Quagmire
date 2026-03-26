using Quagmire.Geometry;
using Quagmire.Rendering;

namespace Quagmire.Nodes.GameNodes;

public class WorldNode : GameNode
{
    public Color Background;

    public WorldNode(Color background, float scale = 1f)
    {
        Background = background;
        Scale = new Point(scale);
    }

    public override void OnDraw()
    {
        World.Scale = Scale;
        World.Background = Background;
        CameraNode.Current?.Use();
    }
}