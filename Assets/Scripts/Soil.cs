using UnityEngine;

public class Soil : MonoBehaviour
{
    private int _plantTimer = -1;
    private PlantTypes.Type _plantType = PlantTypes.Type.NULL_PLANT;
    public int PlantTimer()
    {
        return _plantTimer;
    }
    public PlantTypes.Type PlantType()
    {
        return _plantType;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _plantTimer = -1;
        _plantType = PlantTypes.Type.NULL_PLANT;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTick()
    {
        if (_plantType != PlantTypes.Type.NULL_PLANT)
        {
            _plantTimer--;
        }
    }
}
