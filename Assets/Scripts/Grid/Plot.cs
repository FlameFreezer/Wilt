using System;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour, IClickable
{
    private uint _xIndex;
    private uint _yIndex;
    private GridController _parentGrid;
    private readonly HashSet<Plot> _adjacentPlots = new();
    public GameObject plantSprite;
    public Plant plant = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plantSprite.GetComponent<SpriteRenderer>().enabled = false;
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
        if (plant != null) return;
        Player player = Game.Instance()._player.GetComponent<Player>();
        if (player.selectedPlant == PlantTypes.Type.NULL_PLANT)
        {
            return;
        }
        uint plantCost = PlantTypes.costs[player.selectedPlant];
        if(plantCost > player.money)
        {
            Debug.Log($"Selected plant costs ${plantCost} but you only have {player.money}");
            return;
        }
        player.money -= plantCost;
        PlacePlant(player.selectedPlant);
    }

    void PlacePlant(PlantTypes.Type type)
    {
        _parentGrid.SpawnPlantAtGridPosition(_xIndex, _yIndex, type);
        plantSprite.GetComponent<SpriteRenderer>().enabled = true;
        plantSprite.GetComponent<SpriteRenderer>().sprite = Game.Instance().plantSprites.GetSprite(type);
    }

    public void Harvest(Func<UInt32, GridQueryConfig, Func<Plant, bool>, UInt32> adjacentQueryCallback) {
        plant?.Harvest(adjacentQueryCallback);
    }

    public void Remove() {
        plant?.Payout();
        plant = null;
        plantSprite.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void SetParentGrid(GridController parentGrid)
    {
        _parentGrid = parentGrid;
    }
}
