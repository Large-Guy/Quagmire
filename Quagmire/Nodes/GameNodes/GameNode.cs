using Quagmire.Geometry;
using Quagmire.Rendering;

namespace Quagmire.Nodes.GameNodes;

public class GameNode : Node<GameNode>
{
    public bool Visible = true;
    public bool Active = true;
    
    public Point Position = new Point();
    public Point Scale = new Point(1f);
    public Point Pivot = new Point(0.5f);
    public float Rotation = 0f;
    
    public Point GlobalPosition => GetGlobalPosition();
    public Point ScreenPosition => GetScreenPosition();

    public float GlobalRotation => GetGlobalRotation();
    public Point GlobalScale => GetGlobalScale();

    private Point GetGlobalPosition()
    {
        //TODO: handle rotation and scaling stuff
        return Position + (Parent?.Position ?? new Point());
    }

    private Point GetScreenPosition()
    {
        return Draw.ToScreen(GlobalPosition, false);
    }

    private float GetGlobalRotation()
    {
        return Rotation + (Parent?.Rotation ?? 0f);
    }

    private Point GetGlobalScale()
    {
        return Scale * (Parent?.Scale ?? new Point(1f));
    }

    public GameNode(string name = "") : base(name)
    {
        
    }

    public virtual void OnUpdate()
    {
        
    }

    public void DoUpdate()
    {
        OnUpdate();
        foreach (var child in Children)
        {
            if(child.Active)
                child.DoUpdate();
        }
    }

    public virtual void OnDraw()
    {
        
    }

    public void DoDraw()
    {
        CameraNode.Current?.Use();
        OnDraw();
        foreach (var child in Children)
        {
            if(child.Visible)
                child.DoDraw();
        }
    }
}