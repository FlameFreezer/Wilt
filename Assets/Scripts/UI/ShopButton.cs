using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public PlantTypes.Type plantType;
    public TextMeshProUGUI plantName;
    public TextMeshProUGUI plantDescription;
    public TextMeshProUGUI plantCost;
    public Image plantIcon;
    public Color32 colorWhenSelected;
    private Color32 _colorUnselected;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plantName.text = $"{plantType.GetNameCapitalized()}";
        plantIcon.sprite = PlantTypes.TypeToSprite(plantType);
        _colorUnselected = GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        uint cost = plantType.GetCost();
        if (Game.Instance().Player().onPlantEffect == PlantTypes.Type.HUNGERING_VOIDBEET)
        {
            cost = (uint)(cost * 0.85);
        }
        plantCost.text = $"{plantType.GetNameCapitalized()} - ${cost}";
    }

    public void SelectPlant()
    {
        Game.Instance()._player.GetComponent<Player>().SelectPlant(plantType);
        GetComponent<Image>().color = colorWhenSelected;
    }
}
