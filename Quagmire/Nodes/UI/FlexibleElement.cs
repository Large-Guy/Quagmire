using Quagmire.Geometry;

namespace Quagmire.Nodes.UI;

public abstract class FlexibleElement : UIElement
{
    public enum Alignment
    {
        Leading,
        Center,
        Trailing
    }

    public Scale XPadding;
    public Scale YPadding;
    public Scale XGap;
    public Scale YGap;
    public Alignment XAlignment;
    public Alignment YAlignment;
    public bool XOverflow;
    public bool YOverflow;
    public bool XFill;
    public bool YFill;
}