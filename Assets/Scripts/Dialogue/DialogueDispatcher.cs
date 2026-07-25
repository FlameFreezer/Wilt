using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct Dialogue {
	public string text;
	public UInt32 minTickCount;
	public UInt32 maxTickCount;
}

public class DialogueDispatcher : MonoBehaviour {
	private bool _isTalking = false;

    void Start() {
		TextAsset[] dialogueAssets = Resources.LoadAll<TextAsset>("PlantDialogue");

		foreach(TextAsset dialogueAsset in dialogueAssets) {
			PlantTypes.Type correspondingPlantType = PlantTypes.TypeFromString(dialogueAsset.name);
			if(correspondingPlantType == PlantTypes.Type.NULL_PLANT) {
				Debug.LogWarning($"Tried to load plant dialogue for plant type \"{dialogueAsset.name}\", which does not exist.");
				continue;
			}

			Dictionary<UInt32, Dialogue> deserializedDialogues = JsonConvert.DeserializeObject<Dictionary<UInt32, Dialogue>>(dialogueAsset.text);

			foreach((UInt32 tickTime, Dialogue dialogue) in deserializedDialogues) {
				Game.Instance().EventBus().OnEventScheduled(tickTime, () => { TryPerformingDialogue(correspondingPlantType, dialogue); });
			}
		}
	}

	void TryPerformingDialogue(PlantTypes.Type speakerType, Dialogue dialogue) {
		// TODO - check if there's a plant of type speakerType on the board,
		// if not, fail quietly

		if(_isTalking) { return; }

		_isTalking = true;

		// TODO - figure out min/max tick count logic
		Game.Instance().EventBus().OnDialogueDisplayRequested(dialogue.text, PlantTypes.TypeToPortrait(speakerType));

		// TODO - type type type
		_isTalking = false;
	}
}
