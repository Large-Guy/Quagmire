using System.Numerics;

namespace Quagmire;

public class Shared<T>
{
    public T Value;

    public Shared(T value)
    {
        Value = value;
    }

    public static implicit operator T(Shared<T> r) => r.Value;
}