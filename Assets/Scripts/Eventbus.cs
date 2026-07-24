using System;

public class EventBus
{
    public event Action onTick;
    public event Action<bool> onPause;

    public void OnTick()
    {
        onTick?.Invoke();
    }

    public void OnPause(bool isPaused)
    {
        onPause?.Invoke(isPaused);
    }
}