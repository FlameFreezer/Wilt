using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
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
        Game.Instance().EventBus().onTick += OnTick;
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

    private void OnTick() {
        Debug.Log($"Player: {_money} Time Shekels");
    }    
}
