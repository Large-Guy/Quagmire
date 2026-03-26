namespace Quagmire.Nodes;

public class Node<T> where T : Node<T>
{
    public string Name;

    private bool _alive = true;

    private T? _parent = null;

    public Node<T> Root => GetRoot();

    private Node<T> GetRoot()
    {
        return _parent?.GetRoot() ?? this;
    }
    
    public T? Parent
    {
        get => _parent;
        set
        {
            if (value != null)
            {
                value.AddChild((T)this);
            }
            else
            {
                if (_parent != null)
                {
                    _parent.RemoveChild((T)this);
                    _parent = null;
                }
            }
        }
    }
    protected readonly List<T> Children = [];
    
    public Node(string name = "")
    {
        Name = name;
    }
    
    public bool Alive => _alive;

    public void Destroy()
    {
        while (Children.Count > 0)
        {
            Children[0].Destroy();
        }
        _alive = false;
        if (_parent != null)
        {
            _parent.RemoveChild((T)this);
        }
    }

    public T AddChild(T child)
    {
        Children.Add(child);
        child._parent = (T?)this;
        return child;
    }
    
    public J AddChild<J>(J child) where J : T
    {
        Children.Add(child);
        child._parent = (T?)this;
        return child;
    }

    public void RemoveChild(T child)
    {
        child._parent = null;
        Children.Remove(child);
    }

    public T GetChild(int index)
    {
        return Children[index];
    }

    public TS? GetChild<TS>() where TS : T
    {
        return Children.Find(n => n is TS) as TS;
    }

    public List<TS> GetChildren<TS>(bool recursive = false) where TS : T
    {
        var children = Children.FindAll(n => n is TS).Cast<TS>().ToList();
        if (recursive)
        {
            foreach (var child in children.ToList())
            {
                children.AddRange(child.GetChildren<TS>(recursive));
            }
        }

        return children;
    }

    public int GetChildCount()
    {
        return Children.Count;
    }
}