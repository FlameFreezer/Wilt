using System;
using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour {
	private TextMeshProUGUI _displayText = null;

    void Start() {
		if(TryGetComponent<TextMeshProUGUI>(out _displayText)) {
			Game.Instance().EventBus().onPlayerMoneyChanged += _UpdateText;
		} else {
			Debug.LogError("No TMP_Text component on MoneyDisplay!");
		}
	}

	private void _UpdateText(UInt32 amount) {
		_displayText.text = $"${amount}";
	}
}
