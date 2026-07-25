using TMPro;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public PlantTypes.Type plantType;
    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = $"{plantType.GetNameCapitalized()} - ${plantType.GetCost()}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectPlant()
    {
        Game.Instance()._player.GetComponent<Player>().SelectPlant(plantType);
    }
}
