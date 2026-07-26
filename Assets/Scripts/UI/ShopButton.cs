using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlantTypes.Type plantType;
    public TextMeshProUGUI plantName;
    public TextMeshProUGUI plantCost;
    public Image plantIcon;
    public Color32 colorWhenSelected;
    private Color32 _colorUnselected;
    public bool startSelected = false;

    private Tooltip _tooltip = null;

    private Tooltip Tooltip {
        get {
            if (_tooltip == null) {
                _tooltip = FindFirstObjectByType<Tooltip>(FindObjectsInactive.Include);
            }
            return _tooltip;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plantName.text = $"{plantType.GetNameCapitalized()}";
        plantIcon.sprite = PlantTypes.TypeToSprite(plantType);
        _colorUnselected = GetComponent<Image>().color;

        Game.Instance().EventBus().onPlantSelected += ResetColor;

        if(startSelected)
        {
            SelectPlant();
        }
    }

    // Update is called once per frame
    void Update()
    {
        uint cost = plantType.GetCost();
        if (Game.Instance().Player().onPlantEffect == PlantTypes.Type.HUNGERING_VOIDBEET)
        {
            cost = (uint)(cost * 0.85);
        }
        plantCost.text = $"${cost}";
    }

    public void SelectPlant()
    {
        Game.Instance().EventBus().OnPlantSelected();
        Game.Instance()._player.GetComponent<Player>().SelectPlant(plantType);
        GetComponent<Image>().color = colorWhenSelected;
    }

    private void ResetColor()
    {
        GetComponent<Image>().color = _colorUnselected;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Tooltip?.SetPlant(plantType);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Tooltip?.Hide();
    }
}
