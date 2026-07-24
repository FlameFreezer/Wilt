using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlantTypes.Type selectedPlant = PlantTypes.Type.EYE_WEED;
    public UInt32 startingMoney;
    private UInt32 _money;
	public UInt32 money {
		get { return _money; }
		set {
			if(_money == value) { return; }

			_money = value;
			Game.Instance().EventBus().OnPlayerMoneyChanged(_money);
		}
	}

    void Start() {
        Game.Instance()._player = gameObject;
        money = startingMoney;
    }

    void Update() { }

	public void OnClick(InputAction.CallbackContext context) {
        if(!context.started)
        {
            return;
        }
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity);

		if(hit.transform == null) { return; }

		if(hit.transform.gameObject.TryGetComponent<IClickable>(out IClickable result)) {
			result.OnClick();
		}
    }

    public void SelectPlant(PlantTypes.Type type)
    {
        Debug.Log($"Selected {PlantTypes.TypeToString(type)}");
        selectedPlant = type;
    }
}
