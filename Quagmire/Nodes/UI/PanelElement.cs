using Quagmire.Rendering;

namespace Quagmire.Nodes.UI;

public class PanelElement : UIElement
{
    public NinePatch? Texture;
    
    public PanelElement()
    {
        ConsumeInput = true;
    }

    public override void OnDraw()
    {
        if (Texture != null)
        {
            Draw.NinePatch(Texture, Rect.Position, Rect.Size, new Color(1f, 1f, 1f, TotalOpacity));
        }
    }
}