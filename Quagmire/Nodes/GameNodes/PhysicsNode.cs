using Quagmire.Geometry;
using Quagmire.Rendering;

namespace Quagmire.Nodes.GameNodes;

public enum PhysicsMode
{
    Static,
    Kinematic
}
public class PhysicsNode : GameNode
{
    protected PhysicsMode Mode;
    public Point Velocity = new Point();

    public PhysicsNode(PhysicsMode mode)
    {
        Mode = mode;
    }

    private static Point Mtv(List<ColliderNode> a, List<ColliderNode> b)
    {
        Point? smallest = null;
        var length = 0f;
        foreach (var collider in a)
        {
            if(collider.Polygon == null)
                continue;
            foreach (var against in b)
            {
                if(against.Polygon == null)
                    continue;
                
                var mtv = collider.GlobalPolygon.MinimumTranslationVector(against.GlobalPolygon);
                var mag = mtv.Length();
                if (mag > length)
                {
                    length = mag;
                    smallest = mtv;
                }
            }
        }

        return smallest != null ? smallest.Value : new Point();
    }
    public override void OnUpdate()
    {
        if (Mode == PhysicsMode.Static)
        {
            base.OnUpdate();
            return;
        }

        Position += Velocity * Time.DeltaTime;

        var myColliders = GetChildren<ColliderNode>(true);

        var worldPhysicsNodes = Root.GetChildren<PhysicsNode>(true);
        
        foreach (var node in worldPhysicsNodes)
        {
            if(node == this)
                continue;

            var mtv = Mtv(myColliders, node.GetChildren<ColliderNode>());
            Position += mtv;
        }
        
        base.OnUpdate();
    }
}