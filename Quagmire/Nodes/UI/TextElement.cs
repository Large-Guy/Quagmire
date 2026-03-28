using Quagmire.Rendering;

namespace Quagmire.Nodes.UI;

public class TextElement : UIElement
{
    public enum Alignment
    {
        Leading,
        Center,
        Trailing
    }
    public Color Color = Color.White;
    public string Text = "";
    public int Size = 24;
    public Font? Font = null;
    public Alignment HorizontalAlignment = Alignment.Leading;
    public Alignment VerticalAlignment = Alignment.Leading;
    
    
    public TextElement()
    {
        
    }

    public override void OnDraw()
    {
        if (Font == null)
            return;
        var size = Font.Measure(Text, Size, (int)ScreenRect.Size.X);
        
        var position = ScreenRect.Position;

        switch (HorizontalAlignment)
        {
            case Alignment.Leading:
            {
                break;
            }
            case Alignment.Center:
            {
                position.X += (ScreenRect.Size.X - size.X) / 2;
                break;
            }
            case Alignment.Trailing:
            {
                position.X += ScreenRect.Size.X - size.X;
                break;
            }
        }

        switch (VerticalAlignment)
        {
            case Alignment.Leading:
            {
                break;
            }
            case Alignment.Center:
            {
                position.Y += (ScreenRect.Size.Y - size.Y) / 2;
                break;
            }
            case Alignment.Trailing:
            {
                position.Y += ScreenRect.Size.Y - size.Y;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Draw.Text(Text, Font, Size, position, Color, (int)ScreenRect.Size.X);
    }
}