namespace Quagmire.Coroutines;

public interface IPromise
{
    public void Cancel();
    public bool Finished { get; }
}

public class Promise<T> : IPromise
{
    public enum PromiseState
    {
        Pending,
        Completed,
        Canceled
    }
    private PromiseState _state = PromiseState.Pending;
    private T? _value;

    public T Result => _state == PromiseState.Completed && _value != null ? _value : throw new Exception("Promise not completed");
    public PromiseState State => _state;
    
    public void Fulfil(T value)
    {
        _state = PromiseState.Completed;
        _value = value;
    }

    public void Cancel()
    {
        _state = PromiseState.Canceled;
    }

    public void Reset()
    {
        _state = PromiseState.Pending;
    }

    public bool Finished => _state == PromiseState.Completed || _state == PromiseState.Canceled;

    public bool Completed => _state == PromiseState.Completed;
}