using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour {

    [SerializeField] private UInt32 width = 1;
    [SerializeField] private UInt32 height = 1;
    [SerializeField] private Vector2 spacing = new(1, 1.4142f);
    [SerializeField] private GameObject plotPrefab;

    private Queue<UInt32>[] _harvestQueues = new Queue<UInt32>[Enum.GetNames(typeof(PlantTypes.Type)).Length];
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start() {
        for(int i = 0; i < Enum.GetNames(typeof(PlantTypes.Type)).Length; i++) {
            _harvestQueues[i] = new Queue<UInt32>();
        }
        
        Game.Instance().EventBus().onTick += OnTick;

        Initialize();
    }

    private void OnDestroy() {
        Game.Instance().EventBus().onTick -= OnTick;
    }

    private void GetFlatIndexFromCoord(uint x, uint y, out int index) {
        index = (int)(y * width + x);
    }
    
    private bool Get2DCoord(uint index, out uint x, out uint y) {
        if (index >= width * height) {
            x = 0;
            y = 0;
            return false;
        }
		
        x = index % width;
        y = index / width;
        return true;
    }

    private List<Plot> GetAdjacentPlots(Plot[] grid, int xIndex, int yIndex) {
        List<Plot> adjacentPlots = new List<Plot>();
        
        for (int yOffset = -1; yOffset <= 1; yOffset++) {
            int adjacentY = yIndex + yOffset;
            if (adjacentY < 0 || adjacentY >= height) continue;

            for (int xOffset = -1; xOffset <= 1; xOffset++) {
                int adjacentX = xIndex + xOffset;
                if (adjacentX < 0 || adjacentX >= width) continue;

                if (adjacentX == xIndex && adjacentY == yIndex) continue;
                
                GetFlatIndexFromCoord((uint)adjacentX, (uint)adjacentY, out int index);

                adjacentPlots.Add(grid[index]);
            }
        }
        
        return adjacentPlots;
    }

    private void Initialize() {
        if (width <= 0 || height <= 0) {
            Debug.LogWarning("GridController: width and height must be greater than 0 (Given: )");
            return;
        }
        
        if (plotPrefab == null) {
            Debug.LogError("GridController: Plot prefab not set");
            return;
        }
        
        float xOrigin = (width - 1) / 2.0f * spacing.x;
        float yOrigin = (height - 1) / 2.0f * spacing.y;
        
        Plot[] plots = new Plot[width * height];

        for (uint yIdx = 0; yIdx < height; yIdx++) {
            float yOffset = yIdx * spacing.y - yOrigin;
            for (uint xIdx = 0; xIdx < width; xIdx++) {
                float xOffset = xIdx * spacing.x - xOrigin;
                
                GameObject newPlot = Instantiate(plotPrefab, transform.position + new Vector3(xOffset, 0, yOffset), Quaternion.identity, transform);
                var plotComponent = newPlot.GetComponent<Plot>();
                plotComponent.SetPosition(xIdx, yIdx);
                plotComponent.SetParentGrid(this);
                plotComponent.Remove(); // Remove sprite
                
                GetFlatIndexFromCoord(xIdx, yIdx, out int index);
                plots[index] = plotComponent;
            }
        }

        // Pre-compute adjacency lookups into the grid plots, for skipping coordinate logic in lookup
        foreach (Plot plot in plots) {
            plot.GetPosition(out uint x, out uint y);
            foreach (Plot aP in GetAdjacentPlots(plots, (int) x, (int) y)) {
                plot.AddAdjacentPlot(aP);
            }
        }

        if (transform.parent) {
            Transform modelTransform = transform.parent.Find("Model");
            if (modelTransform) {
                var modelScale = modelTransform.localScale;
                modelScale.x = width * spacing.x;
                modelScale.z = height * spacing.y;
                modelTransform.localScale = modelScale;
            }
        }
    }

    public bool GetPlot1D(uint index, out Plot plot) {
        if (!Get2DCoord(index, out uint x, out uint y)) {
            plot = null;
            return false;
        }
        
        return GetPlot2D(x, y, out plot);
    }

    public bool GetPlot2D(uint xIndex, uint yIndex, out Plot plot) {
        var filtered = GetPlots().Where(p => {
            p.GetPosition(out uint plotX, out uint plotY);
            return xIndex == plotX && yIndex == plotY;
        });

        if (filtered.Count() == 0) {
            plot = null;
            return false;
        }
        
        plot = filtered.First();
        return true;
    }

    private Plot[] GetPlots() {
        return transform.GetComponentsInChildren<Plot>();
    }
    
    public UInt32 QueryAdjacentTiles(UInt32 sourceId, GridQueryConfig config, Func<Plant, bool> criteria) {
        GetPlot1D(sourceId, out Plot plot);

        UInt32 matches = 0;

        foreach (Plot adjacentPlot in plot.GetAdjacentPlots()) {
            if(criteria.Invoke(adjacentPlot.plant)) {
                if(config.matchesRequired - 1 < matches++) { return matches; }
            }
        }
        

        return matches;
    }

    public void Harvest() {
        foreach(Queue<UInt32> queue in _harvestQueues) {
            if (queue == null) continue;

            foreach(UInt32 toHarvest in queue) {
                if (GetPlot1D(toHarvest, out Plot plot)) {
                    plot.Harvest(QueryAdjacentTiles);
                }
            }
        }

        foreach(Queue<UInt32> queue in _harvestQueues) {
            if (queue == null) continue;

            while(queue.TryDequeue(out UInt32 toHarvest)) {
                if (GetPlot1D(toHarvest, out Plot plot)) {
                    plot.Remove();
                }
            }
        }
    }

    // assumes there's not already a plant at this position.
    // TODO -- should we handle checking validity at the
    // input level?
    public void SpawnPlantAtGridPosition(UInt32 x, UInt32 y, PlantTypes.Type type) {
        UInt32 plantId = y * width + x;

        Plant newPlant = null;

        switch(type) {
            case PlantTypes.Type.EYE_WEED:
                newPlant = new EyeWeed();
                break;
            default: return; // TODO - send an error
        }

        newPlant.AssignId(plantId);
        GetPlot1D(plantId, out Plot plot);
        plot.plant = newPlant;
		
        newPlant.OnHarvestRequested += index => AddPlantToHarvestQueue(type, index);
    }
    
    private void AddPlantToHarvestQueue(PlantTypes.Type type, UInt32 index) {
        _harvestQueues[(int)type].Enqueue(index);
    }

    // Update is called once per frame
    private void Update() {
        
    }

    private void OnTick() {
        // The plant resolution order is defined here for now
        InvokeTick(PlantTypes.Type.EYE_WEED);
        Harvest();
    }

    private void InvokeTick(PlantTypes.Type type) {
        foreach(Plant plant in GetPlots().Select(p => p.plant)) {
            if (plant == null) continue;
            if(plant.type == type) {
                plant.Tick();
            }
        }
    }
}

public struct GridQueryConfig {
    public UInt32 matchesRequired;
}
