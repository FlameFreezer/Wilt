using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private double _secondsPerTick;
    private double _timeSinceLastTick = 0.0;
    public Action _tickUpdate;
    public void SetTicksPerSecond(double ticksPerSecond)
    {
        _secondsPerTick = 1.0 / ticksPerSecond;
    }
    public double TicksPerSecond()
    {
        return 1.0 / _secondsPerTick;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeSinceLastTick = 0.0; 
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceLastTick += Time.deltaTime;
        if (_timeSinceLastTick > _secondsPerTick)
        {
            _tickUpdate?.Invoke();
        }
    }
    private void OnTick()
    {
        Debug.Log("Tick update!");
    }
}
