using System;
using UnityEngine;

public class TickTimer : MonoBehaviour
{
    public double _ticksPerSecond;
    private double _timeSinceLastTick = 0.0;
    public Action _tickUpdate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeSinceLastTick = 0.0;
        _tickUpdate += OnTick;
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceLastTick += Time.deltaTime;
        double secondsPerTick = 1.0 / _ticksPerSecond;
        if (_timeSinceLastTick >= secondsPerTick)
        {
            _tickUpdate?.Invoke();
            _timeSinceLastTick -= secondsPerTick;
        }
    }
    private void OnTick()
    {
        Debug.Log("Tick Timer: Tick update!");
    }
}
