using Quagmire.Rendering;

namespace Quagmire.Nodes.UI;

public class ScreenElement : UIElement
{
    protected override void LayoutChildren()
    {
        X = new Position(Position.Label.Pixel, 0);
        Y = new Position(Position.Label.Pixel, 0);
        
        W = new Scale(Scale.Label.Pixel, Draw.Size.X);
        H = new Scale(Scale.Label.Pixel, Draw.Size.Y);
        base.LayoutChildren();
    }
}