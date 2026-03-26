using Quagmire.Rendering;

namespace Quagmire.Nodes.UI;

public class ButtonElement : UIElement
{
    public NinePatch? Normal;
    public new NinePatch? Hovered;
    public new NinePatch? Pressed;
    public new NinePatch? Selected;

    public Action? OnClick = null;
    
    public ButtonElement(Action? onClick = null)
    {
        ConsumeInput = true;
        OnClick = onClick;
    }

    public override void OnMouseEnter()
    {
    }
    
    public override void OnMouseExit()
    {
    }

    public override void OnPressed()
    {
        OnClick?.Invoke();
    }

    public override void OnDraw()
    {
        var patch = Normal;

        if (IsHovered && Hovered != null)
            patch = Hovered;

        if (IsSelected && Selected != null)
            patch = Selected;

        if (IsPressed && Pressed != null)
            patch = Pressed;

        if (patch != null)
        {
            Draw.NinePatch(patch, Rect.Position, Rect.Size, new Color(1f, 1f, 1f, TotalOpacity));
        }
    }
}