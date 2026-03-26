namespace Quagmire.Nodes.UI;

public class VerticalLayoutElement : FlexibleElement
{
    protected override void LayoutChildren()
    {
        var xPadding = XPadding.Get(Rect.Size.X, 0);
        var yPadding = YPadding.Get(Rect.Size.Y, 0);

        var paddingWidth = Rect.Size.X - xPadding * 2f;
        var paddingHeight = Rect.Size.Y - yPadding * 2f;

        var totalPercent = 0f;
        var remaining = paddingHeight - YGap.Get(paddingHeight, 0) * (Children.Count - 1);
        var autoCount = 0;

        foreach (var child in Children)
        {
            switch (child.H.Mode)
            {
                case Scale.Label.Auto:
                {
                    autoCount++;
                    break;
                }
                case Scale.Label.Percent:
                {
                    totalPercent += child.H.Value;
                    break;
                }
                case Scale.Label.Pixel:
                {
                    remaining -= child.H.Value;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        var percentScale = 1.0f;
        if ((YFill || (!YOverflow && totalPercent > 1.0f)) && totalPercent > 0.0f) {
            percentScale = 1.0f / totalPercent;
        }

        remaining -= paddingWidth * (totalPercent * percentScale);

        var autoSize = remaining / autoCount;

        var alignmentOffset = YAlignment switch
        {
            Alignment.Leading => 0f,
            Alignment.Center => remaining / 2f,
            Alignment.Trailing => remaining,
            _ => throw new ArgumentOutOfRangeException()
        };

        var consumed = 0f;

        foreach (var child in Children)
        {
            child.Rect.Position.X = child.X.Get(Rect.Position.X + xPadding, paddingWidth);
            child.Rect.Size.X = child.W.Get(paddingWidth, paddingWidth);
            
            child.Rect.Position.Y = Rect.Position.Y + yPadding + consumed + alignmentOffset;
            child.Rect.Size.Y = child.H.Get(paddingHeight, autoSize);

            consumed += child.Rect.Size.Y;

            if (child != Children.Last()) {
                consumed += YGap.Get(paddingHeight, 0.0f);
            } 
        }
    }
}