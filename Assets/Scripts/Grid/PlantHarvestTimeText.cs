using TMPro;
using UnityEngine;

public class PlantHarvestTimeText : MonoBehaviour
{

    public Plot plot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance().EventBus().onTick += UpdateText; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText()
    {
        if(plot.plant != null)
        {
            TextMeshPro text = GetComponent<TextMeshPro>();
            text.text = $"{plot.plant.ticksUntilHarvest}";
        }
    }
}
