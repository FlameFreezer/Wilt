using UnityEngine;

public class TestObject : MonoBehaviour, IClickable
{
	void Start() { }
	
	public void OnClick() {
		Debug.Log("click");
	}
}
