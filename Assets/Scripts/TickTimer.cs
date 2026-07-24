using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TickTimer : MonoBehaviour
{
    public double _ticksPerSecond;
    private double _timeSinceLastTick = 0.0;
    private bool _isPaused = true;
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
            double secondsPerTick = 1.0 / _ticksPerSecond;
            if (_timeSinceLastTick >= secondsPerTick)
            {
                Debug.Log("Tick");
                Game.Instance().EventBus().OnTick();
                _timeSinceLastTick -= secondsPerTick;
            }
        }
    }

    void PauseTimer(InputAction.CallbackContext context)
    {
        _isPaused = !_isPaused;
        _timeSinceLastTick = 0.0;
        Game.Instance().EventBus().OnPause(_isPaused);
    }

    public bool IsPaused()
    {
        return _isPaused;
    }
}
