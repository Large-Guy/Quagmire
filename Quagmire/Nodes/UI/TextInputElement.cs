using Quagmire.Geometry;
using Quagmire.Input;
using Quagmire.Rendering;

namespace Quagmire.Nodes.UI;

public class TextInputElement : UIElement
{
    public Color Color = Color.White;
    public string Text = "";
    public int Size = 32;
    public Font? Font = null;
    
    public string PlaceHolder = "...";
    private int _cursor = 0;

    public TextInputElement()
    {
        ConsumeInput = true;
    }

    public override void OnUpdate()
    {
        if (IsSelected)
        {
            Keyboard.Text(ref Text, ref _cursor);
        }
    }

    public override void OnDraw()
    {
        if (Font == null)
            return;
        
        var size = Font.Measure(Text, Size, (int)ScreenRect.Size.X);
        
        var verticalCentering = (ScreenRect.Size.Y - size.Y) / 2;
        
        Draw.Text(Text, Font, Size, ScreenRect.Position + new Point(8f, verticalCentering), Color, (int)ScreenRect.Size.X);
    }
}