using TMPro;
using UnityEngine;

public class GlobalTimer : MonoBehaviour {
	[SerializeField]
	private int _ticksRemaining = 500;

	[SerializeField]
	private int _ticksPerClick = 50;

	[SerializeField]
	private double _costMutliplierPerClick = 1.25;

	[SerializeField]
	private uint _addTimeCost = 200;

	[SerializeField]
	private TextMeshProUGUI _display;

	void Start() {
		Game.Instance().globalTimer = this;
		Game.Instance().EventBus().onTick += DecrementTimer;

		UpdateDisplay();
	}

	public void AddTime() {
		Player player = Game.Instance()._player.GetComponent<Player>();
		if(player.money < _addTimeCost)
		{
			Debug.Log($"{player.money} is not enough to afford cost of {_addTimeCost} to add time");
			return;
		}
		player.money -= _addTimeCost;

		_ticksRemaining += _ticksPerClick;
		_addTimeCost = (uint)(_addTimeCost * _costMutliplierPerClick);

		Game.Instance().EventBus().OnGlobalTimeAdded();
		UpdateDisplay();
	}

	public uint GetAddTimeCost()
	{
		return (uint)_addTimeCost;
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
