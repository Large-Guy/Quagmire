using System.Collections;

namespace Quagmire.Coroutines;

public interface ICoroutineInstance
{
    
    bool MoveNext();
}

public class CoroutineInstance : ICoroutineInstance
{
    private readonly IEnumerator _enumerator;
    private ICoroutineInstance? _current = null;
    private ICoroutineInstance? _root = null;
    private IPromise? _awaiting = null;
    private Signal? _signal = null;
    
    public bool Finished => _finished;
    private bool _finished = false;
    
    public CoroutineInstance(IEnumerator enumerator, ICoroutineInstance? root = null)
    {
        _enumerator = enumerator;
        _root = root;
    }

    private void EndSignal()
    {
        _signal = null;
        Coroutine.Move(_root ?? this);
    }

    public void Stop()
    {
        _finished = true;
    }
    public bool MoveNext()
    {
        if (_finished)
            return false;
        
        if (_awaiting != null && !_awaiting.Finished)
        {
            return true;
        }

        if (_signal != null)
        {
            return true;
        }

        _awaiting = null;
        
        if (_current != null)
        {
            if (_current.MoveNext())
                return true;

            _current = null;
        }

        if (!_enumerator.MoveNext())
        {
            Stop();
            return false;
        }

        var yielded = _enumerator.Current;

        switch (yielded)
        {
            case ICoroutineInstance coroutine:
            {
                _current = coroutine;
                return true;
            }
            case IEnumerator enumerator:
            {
                _current = new CoroutineInstance(enumerator, _root ?? this);
                return true;
            }
            case IPromise promise:
            {
                _awaiting = promise;
                return true;
            }
            case Signal signal:
            {
                _signal = signal;
                _signal?.OneShot(EndSignal);
                return true;
            }
            case null:
            {
                return true;
            }
            default:
            {
                return true;
            }
        }
    }
}

public class CoroutineInstance<T> : ICoroutineInstance
{
    private readonly IEnumerator _enumerator;
    private ICoroutineInstance? _current = null;
    private ICoroutineInstance? _root = null;
    private Promise<T> _result = new();
    private Signal? _signal = null;
    
    private IPromise? _awaiting;

    public bool Finished => _finished;
    private bool _finished = false;

    public CoroutineInstance(IEnumerator enumerator, ICoroutineInstance? root = null)
    {
        _enumerator = enumerator;
        _root = root;
    }

    private void EndSignal()
    {
        _signal = null;
        Coroutine.Move(_root ?? this);
    }

    public void Stop()
    {
        _finished = true;
    }
    public Promise<T> Result => _result;

    public bool MoveNext()
    {
        if (_finished)
            return false;
        
        if (_awaiting != null && !_awaiting.Finished)
        {
            return true;
        }
        
        if (_signal != null)
        {
            return true;
        }

        _awaiting = null;
        
        if (_current != null)
        {
            if (_current.MoveNext())
                return true;

            _current = null;
        }

        if (!_enumerator.MoveNext())
        {
            _result.Cancel();
            Stop();
            return false;
        }

        var yielded = _enumerator.Current;

        switch (yielded)
        {
            case ICoroutineInstance coroutine:
            {
                _current = coroutine;
                return true;
            }
            case IEnumerator enumerator:
            {
                _current = new CoroutineInstance(enumerator, _root ?? this);
                return true;
            }
            case IPromise promise:
            {
                _awaiting = promise;
                return true;
            }
            case Signal signal:
            {
                _signal = signal;
                _signal?.OneShot(EndSignal);
                return true;
            }
            case T:
            {
                _result.Fulfil((T)yielded);
                Stop();
                return false;
            }
            case null:
            {
                return true;
            }
            default:
            {
                return true;
            }
        }
    }
}