using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour {

    [SerializeField] public UInt32 width = 1;
    [SerializeField] public UInt32 height = 1;
    [SerializeField] public UInt32 spacing = 1;
    [SerializeField] private GameObject plotPrefab;

    private Grid _grid = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start() {
        _grid.SetSize(width, height);
        Game.Instance().EventBus().onTick += OnTick;
    }

    private void OnDestroy() {
        Game.Instance().EventBus().onTick -= OnTick;
    }

    private void GetFlatIndexFromCoord(uint x, uint y, out int index) {
        index = (int)(y * width + x);
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

    private void OnEnable() {
        if (plotPrefab == null) {
            Debug.LogError("Plot prefab not set");
            return;
        }
        
        float xOrigin = (width - 1) / 2.0f * spacing;
        float yOrigin = (height - 1) / 2.0f * spacing;
        
        Plot[] plots = new Plot[width * height];

        for (uint yIdx = 0; yIdx < height; yIdx++) {
            float yOffset = yIdx * spacing - yOrigin;
            for (uint xIdx = 0; xIdx < width; xIdx++) {
                float xOffset = xIdx * spacing - xOrigin;
                
                GameObject newPlot = Instantiate(plotPrefab, transform.position + new Vector3(xOffset, 0, yOffset), Quaternion.identity, transform);
                newPlot.GetComponent<Plot>().SetPosition(xIdx, yIdx);
                newPlot.GetComponent<Plot>().SetParentGrid(_grid);
                
                GetFlatIndexFromCoord(xIdx, yIdx, out int index);
                plots[index] = newPlot.GetComponent<Plot>();
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
                modelScale.x = width * spacing;
                modelScale.z = height * spacing;
                modelTransform.localScale = modelScale;
            }
        }
    }

    public bool GetPlot(uint xIndex, uint yIndex, out Plot plot) {
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

    public void Harvest() {
        _grid.ResolveHarvesting(HarvestOne);
    }

    public void HarvestOne(uint x, uint y) {
        // TODO move more logic into this method

        if (GetPlot(x, y, out Plot plot)) {
            plot.Harvest();
        }
    }

    // Update is called once per frame
    private void Update() {
        
    }

    private void OnTick() {
        // The plant resolution order is defined here for now
        _grid.InvokeTick(PlantTypes.Type.EYE_WEED);
        Harvest();
    }
}
