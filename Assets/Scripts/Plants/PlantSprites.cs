using UnityEngine;
using System.Collections.Generic;

public class PlantSprites : MonoBehaviour
{
    public List<Sprite> sprites = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance().plantSprites = this; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetSprite(PlantTypes.Type type)
    {
        return sprites[(int)type];
    }
}
