using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
	private TickTimer _tickTimer;

	public void Initialize(TickTimer tickTimer) {
		_tickTimer = tickTimer;

		_tickTimer._tickUpdate += OnTickUpdate;
	}

	private void OnTickUpdate() {
		for(int i = 0; i < _plantTickCallbacks.Length; i++) {
			foreach(Action tickCallback in _plantTickCallbacks[i].Values) {
				tickCallback.Invoke();
			}
		}
	}

	public void SpawnPlant(PlantTypes.Type type) {
		
	}

	private void RemovePlant()

	public void Deinitialize() {
		_tickTimer._tickUpdate -= OnTickUpdate;
	}
}
