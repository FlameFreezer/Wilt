using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public UInt32 _money;

    void Start() {
        Game.Instance()._player = gameObject;
        Game.Instance().EventBus()._onTick += OnTick;
    }

    void Update() { }

	public void OnClick(InputAction.CallbackContext context) {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.0f));
		Vector3 cameraPosWorld = Camera.main.transform.position;
		Vector3 direction = mousePosWorld - cameraPosWorld;
		direction.Normalize();

		RaycastHit hit;
		Physics.Raycast(cameraPosWorld, direction, out hit, Mathf.Infinity);

		if(hit.transform == null) { return; }

		if(hit.transform.gameObject.TryGetComponent<IClickable>(out IClickable result)) {
			result.OnClick();
		}
    }

    private void OnTick() {
        Debug.Log($"Player: {_money} Time Shekels");
        ++_money;
    }    
}
