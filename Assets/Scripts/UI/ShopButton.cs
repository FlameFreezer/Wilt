using TMPro;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public PlantTypes.Type plantType;
    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = $"{PlantTypes.TypeToString(plantType)} - ${PlantTypes.costs[plantType]}";
    }

    // Update is called once per frame
    void Update()
    {
        uint cost = PlantTypes.costs[plantType];
        if (Game.Instance()._player.GetComponent<Player>().onPlantEffect == PlantTypes.Type.HUNGERING_VOIDBEET)
        {
            cost = (uint)(cost * 0.85);
        }
        text.text = $"{PlantTypes.TypeToString(plantType)} - ${cost}";
    }

    public void SelectPlant()
    {
        Game.Instance()._player.GetComponent<Player>().SelectPlant(plantType);
    }
}
