using System;
using UnityEngine;

public class GridController : MonoBehaviour {

    [SerializeField] public UInt32 width = 1;
    [SerializeField] public UInt32 height = 1;
    [SerializeField] public UInt32 spacing = 1;
    [SerializeField] private GameObject plotPrefab;

    private Grid _grid = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start() {
        _grid.SetWidth(width);
        _grid.SetHeight(height);
        Game.Instance().EventBus()._onTick += OnTick;
    }

    private void OnDestroy() {
        Game.Instance().EventBus()._onTick -= OnTick;
    }

    private void OnEnable() {
        if (plotPrefab == null) {
            Debug.LogError("Plot prefab not set");
            return;
        }
        
        float xOrigin = (width - 1) / 2.0f * spacing;
        float yOrigin = (height - 1) / 2.0f * spacing;

        for (int yIdx = 0; yIdx < height; yIdx++) {
            float yOffset = yIdx * spacing - yOrigin;
            for (int xIdx = 0; xIdx < width; xIdx++) {
                float xOffset = xIdx * spacing - xOrigin;
                
                GameObject newPlot = Instantiate(plotPrefab, transform.position + new Vector3(xOffset, 0, yOffset), Quaternion.identity, transform);
            }
        }

        if (transform.parent) {
            Transform modelTransform = transform.parent.Find("Model");
            if (modelTransform) {
                var modelScale = modelTransform.localScale;
                modelScale.x = width * spacing;
                modelScale.z = height * spacing;
                modelTransform.localScale = modelScale;
            }
        }
    }

    // Update is called once per frame
    private void Update() {
        
    }

    private void OnTick() {
        // The plant resolution order is defined here for now
        _grid.InvokeTick(PlantTypes.Type.EYE_WEED);
    }
}
