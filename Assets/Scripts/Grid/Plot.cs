using UnityEngine;

public class Plot : MonoBehaviour, IClickable
{
    private int _xIndex;
    private int _yIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(int xIndex, int yIndex)
    {
        _xIndex = xIndex;
        _yIndex = yIndex;
    }

    public void OnClick()
    {
        Debug.Log("Plot clicked!");
    }
}
