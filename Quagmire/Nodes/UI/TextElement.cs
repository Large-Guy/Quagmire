using Quagmire.Rendering;

namespace Quagmire.Nodes.UI;

public class TextElement : UIElement
{
    public Color Color = Color.White;
    public string Text = "";
    public int Size = 24;
    public Font? Font = null;
    
    
    
    public TextElement()
    {
        
    }

    public override void OnDraw()
    {
        if (Font == null)
            return;
        Draw.Text(Text, Font, Size, ScreenRect.Position, Color, (int)ScreenRect.Size.X);
    }
}