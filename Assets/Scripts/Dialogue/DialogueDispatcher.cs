using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct Dialogue {
	public Queue<string> text;
}

public struct Dialogue2 {
	public List<string> text;
}

public class DialogueDispatcher : MonoBehaviour {
	private Dictionary<PlantTypes.Type, Dictionary<string, Dialogue>> _dialogueDictionary = new();

    void Start() {
		TextAsset[] dialogueAssets = Resources.LoadAll<TextAsset>("PlantDialogue");

		Game.Instance().EventBus().onPlantTypeFirstPlanted += HandleFirstInstancePlanted;

		foreach(TextAsset dialogueAsset in dialogueAssets) {
			PlantTypes.Type correspondingPlantType = PlantTypes.TypeFromString(dialogueAsset.name);
			if(correspondingPlantType == PlantTypes.Type.NULL_PLANT) {
				Debug.LogWarning($"Tried to load plant dialogue for plant type \"{dialogueAsset.name}\", which does not exist.");
				continue;
			}

			Dictionary<string, Dialogue2> deserializedDialogues = JsonConvert.DeserializeObject<Dictionary<string, Dialogue2>>(dialogueAsset.text);
			Dictionary<string, Dialogue> d = new();

			foreach((string s, Dialogue2 d2) in deserializedDialogues) {
				d.Add(s, new(){ text = new(d2.text), });
			}

			_dialogueDictionary[correspondingPlantType] = d;
		}
	}

	void HandleFirstInstancePlanted(PlantTypes.Type plantType) {
		TryPerformingDialogue(plantType, "firstplanted");
	}

	void TryPerformingDialogue(PlantTypes.Type speakerType, string dialogueIdentifier) {
		if(!_dialogueDictionary.TryGetValue(speakerType, out Dictionary<string, Dialogue> plantDialogueDictionary)) {
			Debug.LogWarning($"failed to retrieve plant dialogue dictionary for plant of type \"{speakerType.GetName()}\".");
			return;
		}

		if(!plantDialogueDictionary.TryGetValue(dialogueIdentifier, out Dialogue dialogue)) {
			Debug.LogWarning($"failed to retrieve plant dialogue with identifier \"{dialogueIdentifier}\" on plant of type \"{speakerType.GetName()}\".");
			return;
		}

		Game.Instance().EventBus().OnDialogueDisplayRequested(dialogue.text, PlantTypes.TypeToPortrait(speakerType));
	}
}
