using System.Collections;

namespace Quagmire.Coroutines;

public static class Coroutine
{
    private static readonly List<ICoroutineInstance> _running = [];
    private static readonly List<ICoroutineInstance> _toAdd = [];

    public static CoroutineInstance Start(IEnumerator coroutine)
    {
        var instance = new CoroutineInstance(coroutine);
        _toAdd.Add(instance);
        return instance;
    }

    public static Promise<T> Start<T>(IEnumerator coroutine)
    {
        var instance = new CoroutineInstance<T>(coroutine);
        _toAdd.Add(instance);
        return instance.Result;
    }

    public static void Update()
    {
        _running.AddRange(_toAdd);
        _toAdd.Clear();

        for (var i = _running.Count - 1; i >= 0; i--)
        {
            if (!_running[i].MoveNext())
            {
                _running.RemoveAt(i);
            }
        }
    }

    public static void Move(ICoroutineInstance instance)
    {
        var index = _running.IndexOf(instance);
        if (index == -1) return;
        if(!_running[index].MoveNext()) 
            _running.RemoveAt(index);
    }

    public static IEnumerator Wait(float seconds)
    {
        var time = 0f;
        while (time < seconds)
        {
            yield return null;
            time += Time.DeltaTime;
        }
    }
}