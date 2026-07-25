using FMODUnity;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalTimer : MonoBehaviour {
	[SerializeField] StudioEventEmitter ticSound;

	[SerializeField]
	private int _baseDuration = 75;

	private int _duration;

	private UInt32 _tickCount = 0;

	[SerializeField]
	private int _ticksPerClick = 50;

	[SerializeField]
	private double _costMutliplierPerClick = 1.25;

	[SerializeField]
	private uint _addTimeCost = 200;

	[SerializeField]
	private TextMeshProUGUI _display;

	private Dictionary<UInt32, Queue<Action>> _scheduledEvents = new();

	public double addTimeCostModifier = 1.0;

	void OnEnable() {
		_duration = _baseDuration;

		Game.Instance().globalTimer = this;

		Game.Instance().EventBus().onTick += AdvanceTimer;
		Game.Instance().EventBus().onEventScheduled += ScheduleEvent;
	}

	public void AddTime() {
		Player player = Game.Instance()._player.GetComponent<Player>();
		uint addTimeCost = (uint)(_addTimeCost * addTimeCostModifier);
		if(player.money < addTimeCost)
		{
			Debug.Log($"{player.money} is not enough to afford cost of {addTimeCost} to add time");
			return;
		}
		player.money -= addTimeCost;

		_duration += _ticksPerClick;
		_addTimeCost = (uint)(_addTimeCost * _costMutliplierPerClick);
		addTimeCostModifier = 1.0;

		Game.Instance().EventBus().OnGlobalTimeAdded();
		UpdateDisplay();
	}

	public uint GetAddTimeCost()
	{
		return (uint)_addTimeCost;
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
        ticSound.Play();
        _display.text = $"{_duration - _tickCount}";
	}
}
