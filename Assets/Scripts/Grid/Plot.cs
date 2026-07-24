using UnityEngine;

public class Plot : MonoBehaviour, IClickable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        //Grid ownerGrid = GetComponentInParent<Grid>();
        Debug.Log("Plot clicked!");
    }
}
