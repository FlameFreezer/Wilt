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

    private Plot[] _plots;

    
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

    private void GetFlatIndexFromCoord(uint x, uint y, out uint index) {
        index = y * width + x;
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

    private void Initialize() {
        if (width <= 0 || height <= 0) {
            Debug.LogWarning($"GridController: width and height must be greater than 0 (Given: w{width}, h{height})");
            return;
        }
        
        if (plotPrefab == null) {
            Debug.LogError("GridController: Plot prefab not set");
            return;
        }
        
        float xOrigin = (width - 1) / 2.0f * spacing.x;
        float yOrigin = (height - 1) / 2.0f * spacing.y;
        
        _plots = new Plot[width * height];

        for (uint yIdx = 0; yIdx < height; yIdx++) {
            float yOffset = yIdx * spacing.y - yOrigin;
            for (uint xIdx = 0; xIdx < width; xIdx++) {
                float xOffset = xIdx * spacing.x - xOrigin;
                
                GameObject newPlot = Instantiate(plotPrefab, transform.position + new Vector3(xOffset, 0, yOffset), Quaternion.identity, transform);
                var plotComponent = newPlot.GetComponent<Plot>();
                plotComponent.SetPosition(xIdx, yIdx);
                plotComponent.SetParentGrid(this);
                plotComponent.RemovePlant(); // Remove sprite
                
                GetFlatIndexFromCoord(xIdx, yIdx, out uint index);
                _plots[index] = plotComponent;
            }
        }

        // Pre-compute adjacency lookups into the grid plots, for skipping coordinate logic in lookup
        foreach (Plot plot in _plots) {
            plot.GetPosition(out uint xIndex, out uint yIndex);
        
            for (int yOffset = -1; yOffset <= 1; yOffset++) {
                int adjacentY = (int) yIndex + yOffset;
                if (adjacentY < 0 || adjacentY >= height) continue;

                for (int xOffset = -1; xOffset <= 1; xOffset++) {
                    int adjacentX = (int) xIndex + xOffset;
                    if (adjacentX < 0 || adjacentX >= width) continue;

                    if (adjacentX == xIndex && adjacentY == yIndex) continue;
                
                    GetFlatIndexFromCoord((uint) adjacentX, (uint) adjacentY, out uint index);

                    plot.AddAdjacentPlot(_plots[index]);
                }
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
            case PlantTypes.Type.LAMBFLOWER:
                newPlant = new Lambflower();
                break;
            default: return; // TODO - send an error
        }

        newPlant.AssignId(plantId);
        GetPlot1D(plantId, out Plot plot);
        plot.plant = newPlant;
		
        newPlant.OnHarvestRequested += _harvestQueues[(int)type].Enqueue;
    }

    // Update is called once per frame
    private void Update() {
        
    }

    private void OnTick() {
        foreach(Plot plot in _plots)
        {
            if(plot.plant != null)
            {
                plot.plant.Tick();
            }
        }
        HarvestPlots();
    }

    private void HarvestPlots()
    {
        Queue<Plot> harvestQueue = new();
        // Scan over every plot in the grid
        foreach(Plot plot in _plots)
        {
            //Don't do harvest checks on plot if it doesn't have a plant or the plant shouldn't be checked
            bool DontCheckPlot(Plot plot)
            {
                return plot.plant == null || !plot.plant.CheckHarvest();
            }
            // If the plot is not to be harvested, continue
            if (plot.plant == null) continue;
            if (plot.plant.ticksUntilHarvest >= 1) continue;
            // Otherwise, add it to the harvest queue
            harvestQueue.Enqueue(plot);
            // While the harvest queue is not empty
            while (harvestQueue.Count > 0)
            {
                Plot currentPlot = harvestQueue.Dequeue();
                if (DontCheckPlot(currentPlot)) continue;
                // Enqueue all adjacent plots to the current one
                foreach(Plot adjacentPlot in currentPlot.GetAdjacentPlots())
                {
                    harvestQueue.Enqueue(adjacentPlot);
                }
                // Apply current plot's harvest bonus
                currentPlot.plant.Harvest(QueryAdjacentTiles);
            }
        }
        // Get the payouts and clear out the harvested plants
        foreach(Plot plot in _plots)
        {
            if (plot.plant != null && plot.plant.Complete)
            {
                plot.RemovePlant();
            }
        }
    }

}

public struct GridQueryConfig {
    public UInt32 matchesRequired;
}
