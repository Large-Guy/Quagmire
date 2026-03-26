namespace Quagmire.Nodes.UI;

public class HorizontalLayoutElement : FlexibleElement
{
    protected override void LayoutChildren()
    {
        var xPadding = XPadding.Get(Rect.Size.X, 0);
        var yPadding = YPadding.Get(Rect.Size.Y, 0);

        var paddingWidth = Rect.Size.X - xPadding * 2f;
        var paddingHeight = Rect.Size.Y - yPadding * 2f;

        var totalPercent = 0f;
        var remaining = paddingWidth - XGap.Get(paddingWidth, 0) * (Children.Count - 1);
        var autoCount = 0;

        foreach (var child in Children)
        {
            switch (child.W.Mode)
            {
                case Scale.Label.Auto:
                {
                    autoCount++;
                    break;
                }
                case Scale.Label.Percent:
                {
                    totalPercent += child.W.Value;
                    break;
                }
                case Scale.Label.Pixel:
                {
                    remaining -= child.W.Value;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        var percentScale = 1.0f;
        if ((XFill || (!XOverflow && totalPercent > 1.0f)) && totalPercent > 0.0f) {
            percentScale = 1.0f / totalPercent;
        }

        remaining -= paddingWidth * (totalPercent * percentScale);

        var autoSize = remaining / autoCount;

        var alignmentOffset = XAlignment switch
        {
            Alignment.Leading => 0f,
            Alignment.Center => remaining / 2f,
            Alignment.Trailing => remaining,
            _ => throw new ArgumentOutOfRangeException()
        };

        var consumed = 0f;

        foreach (var child in Children)
        {
            child.Rect.Position.Y = child.Y.Get(Rect.Position.Y + yPadding, paddingHeight);
            child.Rect.Size.Y = child.H.Get(paddingHeight, paddingHeight);
            
            child.Rect.Position.X = Rect.Position.X + xPadding + consumed + alignmentOffset;
            child.Rect.Size.X = child.W.Get(paddingWidth, autoSize);

            consumed += child.Rect.Size.X;

            if (child != Children.Last()) {
                consumed += XGap.Get(paddingWidth, 0.0f);
            } 
        }
    }
}