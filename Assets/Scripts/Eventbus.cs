using System;

public class EventBus
{
    public event Action onTick;
    public event Action<bool> onPause;

	public event Action<UInt32> onPlayerMoneyChanged;

    public void OnTick()
    {
        onTick?.Invoke();
    }

    public void OnPause(bool isPaused)
    {
        onPause?.Invoke(isPaused);
    }

	public void OnPlayerMoneyChanged(UInt32 money) {
		onPlayerMoneyChanged?.Invoke(money);
	}
}
