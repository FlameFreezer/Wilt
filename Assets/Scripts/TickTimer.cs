using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TickTimer : MonoBehaviour
{
    private double _timeSinceLastTick = 0.0;
    private bool _isPaused = true;
    public enum TickRate
    {
        SLOW, MEDIUM, FAST
    }
    public TickRate tickRate = TickRate.SLOW;
    public static readonly Dictionary<TickRate, double> ticksPerSecond = new()
    {
        {TickRate.SLOW , 1.0}, {TickRate.MEDIUM, 3.0 }, {TickRate.FAST, 5.0}
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeSinceLastTick = 0.0;

        Game.Instance()._tickTimer = this;

        InputSystem.actions.FindAction("Pause").performed += PauseTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isPaused)
        {
            _timeSinceLastTick += Time.deltaTime;
            double secondsPerTick = 1.0 / ticksPerSecond[tickRate];
            if (_timeSinceLastTick >= secondsPerTick)
            {
                Game.Instance().EventBus().OnTick();
                _timeSinceLastTick -= secondsPerTick;
            }
        }
    }

    void PauseTimer(InputAction.CallbackContext context)
    {
        _isPaused = !_isPaused;
        Game.Instance().EventBus().OnPause(_isPaused);
    }

    public void IncreaseTickRate()
    {
        if (tickRate != TickRate.FAST)
        {
            tickRate++;
            _timeSinceLastTick = 0;
        }
    }

    public void DecreaseTickRate()
    {
        if (tickRate != TickRate.SLOW)
        {
            tickRate--;
            _timeSinceLastTick = 0;
        }
    }

    public bool IsPaused()
    {
        return _isPaused;
    }
}
