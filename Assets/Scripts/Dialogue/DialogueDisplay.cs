using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueDisplay : MonoBehaviour {
	private TextMeshProUGUI _textDisplay;

	[SerializeField]
	private Image _portraitDisplay;

	void Start() {
		if(_portraitDisplay == null) {
			Debug.LogWarning("No portrait display set for DialogueDisplay!");
		}

		_textDisplay = GetComponent<TextMeshProUGUI>();
		
		Game.Instance().EventBus().onDialogueDisplayRequested += HandleDialogueDisplayRequested;
	}

	void HandleDialogueDisplayRequested(string text, Sprite portrait) {
		_textDisplay.text = text;
		_portraitDisplay.sprite = portrait;
	}
}
