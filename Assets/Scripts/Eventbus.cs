using System;
using UnityEngine;

public class EventBus
{
    public event Action onTick;
    public event Action<bool> onPause;

	public event Action<UInt32> onPlayerMoneyChanged;

	public event Action<string, Sprite> onDialogueDisplayRequested;

	public event Action<UInt32, Action> onEventScheduled;

	public event Action onGlobalTimerExhausted;

    public event Action onGlobalTimeAdded;

	public event Action onPlantSelected;

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

	public void OnDialogueDisplayRequested(string text, Sprite portrait) {
		onDialogueDisplayRequested?.Invoke(text, portrait);
	}

	public void OnEventScheduled(UInt32 ticksUntilTrigger, Action callback) {
		onEventScheduled?.Invoke(ticksUntilTrigger, callback);
	}

	public void OnGlobalTimerExhausted() {
		onGlobalTimerExhausted?.Invoke();
	}

    public void OnGlobalTimeAdded()
    {
        onGlobalTimeAdded?.Invoke();
    }

	public void OnPlantSelected()
	{
		onPlantSelected?.Invoke();
	}
}
