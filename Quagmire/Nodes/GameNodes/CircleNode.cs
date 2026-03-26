using Quagmire.Rendering;

namespace Quagmire.Nodes.GameNodes;

public class CircleNode : GameNode
{
    public Color Color = Color.White;
    public float Radius = 1f;
    public bool Fill = true;
    public override void OnDraw()
    {
        if (Fill)
        {
            Draw.Circle(Color, GlobalPosition, Radius);
        }
        else
        {
            Draw.CircleOutline(Color, GlobalPosition, Radius);
        }
    }
}