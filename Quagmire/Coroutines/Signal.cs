namespace Quagmire.Coroutines;

public class Signal
{
    private readonly List<Action> _on;
    private readonly List<Action> _oneShot;

    public Signal()
    {
        _on = [];
        _oneShot = [];
    }
    
    public void Connect(Action action)
    {
        _on.Add(action);
    }

    public void OneShot(Action action)
    {
        _oneShot.Add(action);
    }
    
    public void Emit()
    {
        var onSnapshot = _on.ToArray();
        var oneShots = _oneShot.ToArray();
        _oneShot.Clear();

        for (var i = 0; i < onSnapshot.Length; i++)
        {
            var action = onSnapshot[i];
            action();
        }

        for (var i = 0; i < oneShots.Length; i++)
        {
            var action = oneShots[i];
            action();
        }
    }
}