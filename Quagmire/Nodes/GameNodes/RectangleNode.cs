using Quagmire.Geometry;
using Quagmire.Rendering;

namespace Quagmire.Nodes.GameNodes;

public class RectangleNode : GameNode
{
    public Color Color = Color.White;
    public Rectangle Rectangle;

    public RectangleNode(Rectangle rectangle)
    {
        Rectangle = rectangle;
    }
    
    public override void OnDraw()
    {
        Rendering.Draw.Rectangle(Color, new Rectangle(GlobalPosition + Rectangle.Position, Rectangle.Size), Pivot, GlobalRotation);
        base.OnDraw();
    }
}