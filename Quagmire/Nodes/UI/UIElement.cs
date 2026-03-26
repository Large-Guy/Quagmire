using Quagmire.Geometry;
using Quagmire.Input;
using Quagmire.Rendering;

namespace Quagmire.Nodes.UI;

public class UIElement : Node<UIElement>
{
    public struct Position
    {
        public enum Label
        {
            Auto,
            Percent,
            Pixel
        }
        
        public Label Mode;
        public float Value;

        public Position(Position.Label mode = Position.Label.Auto, float value = 0)
        {
            Mode = mode;
            Value = value;
        }

        public float Get(float start, float relative)
        {
            return Mode switch
            {
                Label.Auto => start,
                Label.Percent => start + Value * relative,
                Label.Pixel => start + Value,
                _ => start
            };
        }
    }

    public struct Scale
    {
        public enum Label
        {
            Auto,
            Percent,
            Pixel
        }

        public Label Mode;
        public float Value;

        public Scale(Scale.Label mode = Scale.Label.Auto, float value = 1f)
        {
            Mode = mode;
            Value = value;
        }
        
        public float Get(float relative, float autoSize)
        {
            return Mode switch
            {
                Label.Auto => autoSize,
                Label.Percent => Value * relative,
                Label.Pixel => Value,
                _ => Value
            };
        }
    }

    public static UIElement? Hovered { get; set; } = null;
    public static UIElement? Selected { get; set; } = null;
    public static UIElement? Pressed { get; set; } = null;

    public float Opacity = 1f;
    
    public float TotalOpacity => Opacity * (Parent?.TotalOpacity ?? 1f);

    public Position X;
    public Position Y;
    public Scale W;
    public Scale H;

    public Position XPivot;
    public Position YPivot;

    public bool ConsumeInput;

    protected internal Rectangle Rect;

    public Rectangle ScreenRect => Rect;
    
    public bool IsHovered => Hovered == this;
    public bool IsSelected => Selected == this;
    public bool IsPressed => Pressed == this;

    protected virtual void LayoutChildren()
    {
        foreach (var child in Children)
        {
            child.Rect.Position.X = child.X.Get(Rect.Position.X, Rect.Size.X);
            child.Rect.Position.Y = child.Y.Get(Rect.Position.Y, Rect.Size.Y);
            
            child.Rect.Size.X = child.W.Get(Rect.Size.X, Rect.Size.X);
            child.Rect.Size.Y = child.H.Get(Rect.Size.Y, Rect.Size.Y);
        }
    }

    public void Layout()
    {
        if (Parent == null)
        {
            Rect = new Rectangle(X.Value, Y.Value, W.Value, H.Value);
        }

        Rect.Position.X -= XPivot.Get(0, Rect.Size.X);
        Rect.Position.Y -= YPivot.Get(0, Rect.Size.Y);
        
        LayoutChildren();

        foreach (var child in Children)
        {
            child.Layout();
        }
    }

    public virtual void OnUpdate()
    {
    }
    
    public virtual void OnMouseEnter()
    {
    }

    public virtual void OnMouseExit()
    {
    }

    public virtual void OnPressed()
    {
    }

    public virtual void OnReleased()
    {
    }

    public virtual void OnSelect()
    {
    }

    public virtual void OnDeselect()
    {
    }
    
    public void DoUpdate()
    {
        OnUpdate();

        foreach (var child in Children)
        {
            if(!child.Alive)
                continue;
            child.DoUpdate();
        }
    }

    public bool DoInput()
    {
        if (Mouse.IsButtonJustReleased(Button.Left))
        {
            Pressed?.Release();
        }
        
        foreach (var child in Children)
        {
            if(!child.Alive)
                continue;
            if (child.DoInput())
                return true;
        }
        
        if (ConsumeInput)
        {
            if (Rect.Contains(Mouse.Position))
            {
                if (Mouse.IsButtonJustPressed(Button.Left))
                {
                    Press();
                }

                if (Mouse.Delta.Length() > 0)
                {
                    Hover();

                    return true;
                }
            }
        }
        
        if (Mouse.Delta.Length() > 0)
            UnHover();
        
        return false;
    }
    
    public virtual void OnDraw()
    {
    }

    public void DoDraw(float startDepth = 0f)
    {
        Draw.Depth = startDepth;
        Draw.Camera = null;
        var prevScale = World.Scale;
        World.Scale = new Point(1f);
        OnDraw();
        foreach (var child in Children)
        {
            if(!child.Alive)
                continue;
            child.DoDraw(startDepth + 1f);
        }
        World.Scale = prevScale;
    }
    
    public void Hover()
    {
        if (Hovered != this)
            Hovered?.UnHover();
        else
            return;

        if (Hovered != null) 
            return;
        
        Hovered = this;
        OnMouseEnter();
    }

    public void UnHover()
    {
        if (Hovered != this) 
            return;
        
        Hovered.OnMouseExit();
        Hovered = null;
    }

    public void Select()
    {
        if(Selected != this)
            Selected?.Deselect();

        if (Selected != null) 
            return;
        
        Selected = this;
        OnSelect();
    }

    public void Deselect()
    {
        if (Selected != this) 
            return;
        
        Selected.OnDeselect();
        Selected = null;    
    }

    public void Press()
    {
        Pressed?.Release();

        Pressed = this;
        OnPressed();
        Select();
    }

    public void Release()
    {
        if (Pressed != this) 
            return;
        
        OnReleased();
        Pressed = null;
    }
    
    public void Click()
    {
        OnPressed();
        Select();
        OnReleased();
    }
}