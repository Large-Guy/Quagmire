namespace Quagmire.Nodes.GameNodes;

public class Component<T> : GameNode where T : GameNode 
{
    public T Base => Parent as T ?? throw new Exception($"Component node must be attached to {typeof(T)}");
    
    public Component(T attach)
    {
        attach.AddChild(this);
    }
}