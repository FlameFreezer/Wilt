using UnityEngine;

public class Game
{
    private static Game _instance;
    private Game() { }

    public GameObject _player;
    private EventBus _eventBus = new();
    public TickTimer _tickTimer;

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
}