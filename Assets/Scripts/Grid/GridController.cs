using System;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public List<GameObject> soilPlots = new();
    public UInt32 width;
    public UInt32 height;
    private Grid _grid = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _grid.SetWidth(width);
        _grid.SetHeight(height);
        Game.Instance().EventBus()._onTick += OnTick;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTick()
    {
        // The plant resolution order is defined here for now
        _grid.InvokeTick(PlantTypes.Type.EYE_WEED);
    }
}
