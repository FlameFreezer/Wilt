using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UIElements;

public class TickTimer : MonoBehaviour
{
    [SerializeField] StudioEventEmitter pauseNoise;
    [SerializeField] StudioEventEmitter unpauseNoise;

    private double _timeSinceLastTick = 0.0;
    private bool _isPaused = true;
    public enum TickRate
    {
        SLOW, MEDIUM, FAST
    }
    public TickRate tickRate = TickRate.SLOW;
    [SerializeField]
    private double slowTickRate = 1.0;
    [SerializeField]
    private double mediumTickRate = 5.0;
    [SerializeField]
    private double fastTickRate = 10.0;
    private readonly static Dictionary<TickRate, double> ticksPerSecond = new()
    {
        {TickRate.SLOW , 0}, {TickRate.MEDIUM, 0.0 }, {TickRate.FAST, 0.0}
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeSinceLastTick = 0.0;

        Game.Instance()._tickTimer = this;
        ticksPerSecond[TickRate.SLOW] = slowTickRate;
        ticksPerSecond[TickRate.MEDIUM] = mediumTickRate;
        ticksPerSecond[TickRate.FAST] = fastTickRate;

		Game.Instance().EventBus().onPauseRequested += Pause;
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

    public void RequestPause(InputAction.CallbackContext context)
    {
		if(!context.started) { return; }

		Game.Instance().EventBus().OnPauseRequested();
    }

	public void Pause() {
        if(Game.Instance().dialogueActive && _isPaused) {
			return;
		}

        _isPaused = !_isPaused;
        Game.Instance().EventBus().OnPause(_isPaused);

        if (_isPaused)
        {
            pauseNoise.Play();
        }
        else
        {
            unpauseNoise.Play();
        }
    }

    public void IncreaseTickRate(InputAction.CallbackContext context)
    {
        if(!context.started)
        {
            return;
        }
        if (tickRate != TickRate.FAST)
        {
            tickRate++;
            _timeSinceLastTick = 0;
        }
    }

    public void DecreaseTickRate(InputAction.CallbackContext context)
    {
        if(!context.started)
        {
            return;
        }

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
