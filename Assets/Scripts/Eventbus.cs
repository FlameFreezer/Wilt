using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{
    public event Action onTick;
    public event Action<bool> onPause;

	public event Action onPauseRequested;

	public event Action<UInt32> onPlayerMoneyChanged;

	public event Action<PlantTypes.Type> onPlantTypeFirstPlanted;

	public event Action<Queue<string>, Sprite> onDialogueDisplayRequested;
	public event Action onDialogueAdvanceRequested;

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

	public void OnPauseRequested() {
		onPauseRequested?.Invoke();
	}

	public void OnPlayerMoneyChanged(UInt32 money) {
		onPlayerMoneyChanged?.Invoke(money);
	}

	public void OnPlantTypeFirstPlanted(PlantTypes.Type plantType) {
		onPlantTypeFirstPlanted?.Invoke(plantType);
	}

	public void OnDialogueDisplayRequested(Queue<string> text, Sprite portrait) {
		onDialogueDisplayRequested?.Invoke(text, portrait);
	}

	public void OnDialogueAdvanceRequested() {
		onDialogueAdvanceRequested?.Invoke();
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
