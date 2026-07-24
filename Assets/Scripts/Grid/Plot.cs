using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour, IClickable
{
    private uint _xIndex;
    private uint _yIndex;
    private Grid _parentGrid;
    private readonly HashSet<Plot> _adjacentPlots = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetPosition(out uint xIndex, out uint yIndex) {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    public void SetPosition(uint xIndex, uint yIndex) {
        _xIndex = xIndex;
        _yIndex = yIndex;
    }

    public void AddAdjacentPlot(Plot plot) {
        _adjacentPlots.Add(plot);
    }

    public IEnumerable<Plot> GetAdjacentPlots() {
        return _adjacentPlots;
    }

    public void OnClick()
    {
        _parentGrid.SpawnPlantAtGridPosition(_xIndex, _yIndex, PlantTypes.Type.EYE_WEED);
        Debug.Log("Planted Eyeweed");
    }

    public void SetParentGrid(Grid parentGrid)
    {
        _parentGrid = parentGrid;
    }
}
