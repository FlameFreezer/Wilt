using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour, IClickable
{
    //AUDIO REFS
    [SerializeField ] StudioEventEmitter digNoise;
    [SerializeField] StudioEventEmitter cashNoise;

    //VARS
    private uint _xIndex;
    private uint _yIndex;
    private GridController _parentGrid;
    private readonly HashSet<Plot> _adjacentPlots = new();
    public GameObject plantSprite;
    public Plant plant = null;
    public GameObject harvestTimeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plantSprite.GetComponent<SpriteRenderer>().enabled = false;
        harvestTimeText.SetActive(false);
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
        uint plantCost = player.selectedPlant.GetCost();
        if (player.onPlantEffect == PlantTypes.Type.HUNGERING_VOIDBEET)
        {
            plantCost = (uint)(plantCost * 0.85);
        }
        if(plantCost > player.money)
        {
            Debug.Log($"Selected plant costs ${plantCost} but you only have {player.money}");
            return;
        }
        player.money -= plantCost;
        PlacePlant(player.selectedPlant);
    }

    public Plant PlacePlant(PlantTypes.Type type)
    {
        digNoise.Play();

        Plant placedPlant = _parentGrid.SpawnPlantAtGridPosition(_xIndex, _yIndex, type);
        Player player = Game.Instance()._player.GetComponent<Player>();
        if(player.onPlantEffect == PlantTypes.Type.LAMBFLOWER)
        {
            placedPlant.ticksUntilHarvest--;
        }
        else if (player.onPlantEffect == PlantTypes.Type.FINGERLING)
        {
            placedPlant.ticksUntilHarvest++;
        }
        plantSprite.GetComponent<SpriteRenderer>().enabled = true;
        harvestTimeText.SetActive(true);
        harvestTimeText.GetComponent<PlantHarvestTimeText>().UpdateText();
        SetModel(type);

        player.onPlantEffect = type;
        return placedPlant;
    }

    private void SetModel(PlantTypes.Type type) {
        if (type == PlantTypes.Type.CTHULILY) {
            // Right now only the Cthulily has animations
            plantSprite.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Animations/cthulily_idle{Random.Range(1, 4)}");
            // The animator itself set sprites, no need to set GetComponent<SpriteRenderer>().sprite
        } else {
            // Defer to non-animated sprite otherwise
            plantSprite.GetComponent<SpriteRenderer>().sprite = PlantTypes.TypeToSprite(type);
        }
    }

    private void ClearModel() {
        plantSprite.GetComponent<SpriteRenderer>().sprite = null;
        plantSprite.GetComponent<Animator>().runtimeAnimatorController = null;
    }

    public void Harvest() {
        plant?.Harvest(this);
        cashNoise.Play();
    }

    public void RemovePlant() {
        plant?.Payout();
        plant = null;
        ClearModel();
        plantSprite.GetComponent<SpriteRenderer>().enabled = false;
        harvestTimeText.SetActive(false);
    }

    public void SetParentGrid(GridController parentGrid)
    {
        _parentGrid = parentGrid;
    }

    public GridController GetParentGrid()
    {
        return _parentGrid;
    }

    public void Tick()
    {
        if (plant != null)
        {
            plant.Tick(this);
        }
    }
}
