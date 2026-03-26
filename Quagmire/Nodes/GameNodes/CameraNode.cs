using Quagmire.Rendering;

namespace Quagmire.Nodes.GameNodes;

public class CameraNode : GameNode
{
    public static CameraNode? Current;
    
    private Rendering.Camera _camera = new();

    public CameraNode()
    {
        MakeCurrent();
    }

    public void MakeCurrent()
    {
        Current = this;
        Use();
    }

    public void Disable()
    {
        if (Current == this)
        {
            Current = null;
            Draw.Camera = null;
        }
    }

    public override void OnUpdate()
    {
        Use();
    }

    public override void OnDraw()
    {
        _camera.Position = Position;
    }

    public void Use()
    {
        Draw.Camera = _camera;
    }
}