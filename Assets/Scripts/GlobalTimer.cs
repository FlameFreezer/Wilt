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

	private void AddTime(int delta) {
		_ticksRemaining += delta;

		UpdateDisplay();
	}

	private void DecrementTimer() {
		_ticksRemaining--;

		UpdateDisplay();
	}

	private void UpdateDisplay() {
		_display.text = $"{_ticksRemaining}";
	}
}
