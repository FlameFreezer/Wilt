using UnityEngine;

public class Plot : MonoBehaviour, IClickable
{
    private uint _xIndex;
    private uint _yIndex;
    private Grid _parentGrid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetPosition(uint xIndex, uint yIndex)
    {
        _xIndex = xIndex;
        _yIndex = yIndex;
    }

    public void OnClick()
    {
        _parentGrid.SpawnPlantAtGridPosition(_xIndex, _yIndex, PlantTypes.Type.EYE_WEED);
        Debug.Log("Planted Eyeweed");
    }

    public void SetParentGrid(Grid parentGrid)
    {
        _parentGrid = parentGrid;
    }
}
