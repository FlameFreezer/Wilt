using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueDisplay : MonoBehaviour {
	private TextMeshProUGUI _textDisplay;

	[SerializeField]
	private Image _portraitDisplay;

	private Queue<string> _textQueue;

	private bool _displaying = false;

	void Start() {
		if(_portraitDisplay == null) {
			Debug.LogWarning("No portrait display set for DialogueDisplay!");
		}

		_textDisplay = GetComponent<TextMeshProUGUI>();
		
		Game.Instance().EventBus().onDialogueDisplayRequested += HandleDialogueDisplayRequested;

		Game.Instance().EventBus().onDialogueAdvanceRequested += ContinueTyping;
	}

	void HandleDialogueDisplayRequested(Queue<string> text, Sprite portrait) {
		_textQueue = text;
		_portraitDisplay.sprite = portrait;

		_displaying = true;

		ContinueTyping();
	}

	void ContinueTyping() {
		if(!_displaying) {
			return;
		}

		if(_textQueue.Count < 1) {
			_displaying = false;
			_textDisplay.text = "";
			_portraitDisplay.sprite = null; // TODO - default portrait

			return;
		}

		_textDisplay.text = _textQueue.Dequeue();
	}
}
