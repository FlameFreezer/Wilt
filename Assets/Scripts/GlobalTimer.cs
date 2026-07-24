using System;
using TMPro;
using UnityEngine;

public class GlobalTimer : MonoBehaviour {
	[SerializeField]
	private int _ticksRemaining = 500;

	[SerializeField]
	private TextMeshProUGUI _display;

	void Start() {
		Game.Instance().EventBus().onTick += DecrementTimer;

		UpdateDisplay();
	}

	private void AddTime(UInt32 delta) {
		_ticksRemaining += (int)delta;

		UpdateDisplay();
	}

	private void DecrementTimer() {
		_ticksRemaining--;

		UpdateDisplay();

		if(_ticksRemaining < 1) {
			Game.Instance().EventBus().OnGlobalTimerExhausted();
		}
	}

	private void UpdateDisplay() {
		_display.text = $"{_ticksRemaining}";
	}
}
