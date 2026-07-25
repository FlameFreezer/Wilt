using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalTimer : MonoBehaviour {
	[SerializeField]
	private int _baseDuration = 75;

	private int _duration;

	private UInt32 _tickCount = 0;

	[SerializeField]
	private TextMeshProUGUI _display;

	private Dictionary<UInt32, Queue<Action>> _scheduledEvents = new();

	void OnEnable() {
		_duration = _baseDuration;

		Game.Instance().EventBus().onTick += AdvanceTimer;
		Game.Instance().EventBus().onEventScheduled += ScheduleEvent;

		UpdateDisplay();
	}

	private void AddTime(UInt32 delta) {
		_duration += (int)delta;

		UpdateDisplay();
	}

	private void ScheduleEvent(UInt32 ticksUntilTrigger, Action callback) {
		if(!_scheduledEvents.TryGetValue(_tickCount + ticksUntilTrigger, out Queue<Action> eventQueue)) {
			eventQueue = new();
			_scheduledEvents[_tickCount + ticksUntilTrigger] = eventQueue;
		}

		eventQueue.Enqueue(callback);
	}

	private void AdvanceTimer() {
		if(_scheduledEvents.TryGetValue(++_tickCount, out Queue<Action> eventQueue)) {
			while(eventQueue.TryDequeue(out Action queuedAction)) {
				queuedAction.Invoke();
			}
		}

		UpdateDisplay();

		if(_duration - _tickCount < 1) {
			Game.Instance().EventBus().OnGlobalTimerExhausted();
		}
	}

	private void UpdateDisplay() {
		_display.text = $"{_duration - _tickCount}";
	}
}
