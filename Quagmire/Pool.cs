namespace Quagmire;

public interface IGenericPool
{
    public void Delete(object obj);
}

public class Pool<T> : IGenericPool where T : class, new()
{
    public int MaxSize;
    private Stack<T> _storage = [];

    public Pool(int maxSize = 64)
    {
        MaxSize = maxSize;
    }

    public T New => _storage.Count > 0 ? _storage.Pop() : new T();

    public void Delete(T item)
    {
        if (_storage.Count >= MaxSize)
            return;
        _storage.Push(item);
    }

    public void Delete(object item)
    {
        if (_storage.Count >= MaxSize)
            return;
        if (item is not T t)
            return;
        _storage.Push(t);
    }
}