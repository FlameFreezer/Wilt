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

		Game.Instance().dialogueActive = true;
		Game.Instance().EventBus().OnPauseRequested();

		ContinueTyping();
	}

	void ContinueTyping() {
		if(!Game.Instance().dialogueActive) {
			return;
		}

		if(_textQueue.Count < 1) {
			Game.Instance().dialogueActive = false;
			_textDisplay.text = "";
			_portraitDisplay.sprite = null; // TODO - default portrait

			return;
		}

		_textDisplay.text = _textQueue.Dequeue();
	}
}
