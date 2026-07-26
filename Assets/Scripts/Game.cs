using System;
using System.Collections.Generic;
using UnityEngine;

namespace HaltSeverity {
	public enum Type {
		UNBLOCKED,
		PAUSE_TICKS,
		PAUSE_GAME,
	}
}

public class Game
{
    private static Game _instance;
    private Game() { }

    public GameObject _player;
    private EventBus _eventBus = new();
    public TickTimer _tickTimer;
    public GlobalTimer globalTimer;

	public bool dialogueActive = false;

	private Dictionary<Guid, HaltSeverity.Type> _gameplayBlocks = new();

    public static Game Instance()
    {
        if (_instance == null)
        {
            _instance = new Game();
        }
        return _instance;
    }

    public EventBus EventBus()
    {
        return _eventBus;
    }

    public Player Player()
    {
        return _player.GetComponent<Player>();
    }

	public bool IsBlocked(HaltSeverity.Type tolerance) {
		foreach((var _, HaltSeverity.Type type) in _gameplayBlocks) {
			if(type > tolerance) { return true; }
		}

		return false;
	}

	public void AddBlock(Guid id, HaltSeverity.Type severity) {
		if(severity == HaltSeverity.Type.UNBLOCKED) { return; }

		_gameplayBlocks.Add(id, severity);
	}

	public void RemoveBlock(Guid id) {
		_gameplayBlocks.Remove(id);
	}
}
