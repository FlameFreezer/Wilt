using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tooltip : MonoBehaviour {
    
    [SerializeField] private TextMeshProUGUI textObject;
    
    private Camera _camera = null;

    private Camera Camera {
        get {
            if (_camera == null) {
                _camera = FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
            }
            return _camera;
        }
    }

    public void SetPlant(PlantTypes.Type plant) {
        SetText($"Cost: ${plant.GetCost()}\nYield: ${plant.GetPayout()}\nGrow: {plant.GetTicksUntilHarvest()} cycles");
    }

    public void SetText(string text) {
        transform.gameObject.SetActive(true);
        textObject.text = text;
    }

    public void Hide() {
        transform.gameObject.SetActive(false);
    }

    private void Update() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Mouse.current.position.ReadValue(), Camera, out Vector2 localPoint);
        transform.position = localPoint;
    }
    
}
