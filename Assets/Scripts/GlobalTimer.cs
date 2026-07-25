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

	public double addTimeCostModifier = 1.0;

	void Start() {
		Game.Instance().globalTimer = this;
		Game.Instance().EventBus().onTick += DecrementTimer;

		UpdateDisplay();
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

		_ticksRemaining += _ticksPerClick;
		_addTimeCost = (uint)(_addTimeCost * _costMutliplierPerClick);
		addTimeCostModifier = 1.0;

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
