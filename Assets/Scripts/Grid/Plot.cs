using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour, IClickable
{
    private uint _xIndex;
    private uint _yIndex;
    private Grid _parentGrid;
    private readonly HashSet<Plot> _adjacentPlots = new();
    public GameObject plantSprite;
    public PlantTypes.Type plantType = PlantTypes.Type.NULL_PLANT;
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
        if (plantType != PlantTypes.Type.NULL_PLANT) return;
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
        _parentGrid.SpawnPlantAtGridPosition(_xIndex, _yIndex, player.selectedPlant);
        plantType = player.selectedPlant;
        player.money -= plantCost;
        plantSprite.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Harvest() {
        plantSprite.GetComponent<SpriteRenderer>().enabled = false;
        plantType = PlantTypes.Type.NULL_PLANT;
    }

    public void SetParentGrid(Grid parentGrid)
    {
        _parentGrid = parentGrid;
    }
}
