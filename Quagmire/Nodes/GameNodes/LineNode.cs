using Quagmire.Geometry;
using Quagmire.Rendering;

namespace Quagmire.Nodes.GameNodes;

public class LineNode : GameNode
{
    public Color Color = Color.White;
    public Point Start;
    public Point End;

    public override void OnDraw()
    {
        Draw.Line(Color, Start, End);
    }
}