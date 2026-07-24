using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;

public struct GridQueryConfig {
	public UInt32 matchesRequired;
}

public class Grid {
    private List<Plant> _plants = new();
	private Queue<UInt32>[] _harvestQueues = new Queue<UInt32>[Enum.GetNames(typeof(PlantTypes.Type)).Length];

	private UInt32 _width;
	private UInt32 _height;

	public void InvokeTick(PlantTypes.Type type) {
		foreach(Plant plant in _plants) {
			if (plant == null) continue;
			if(plant.type == type) {
				plant.Tick();
			}
		}
	}

	public void SetWidth(UInt32 width)
	{
		_width = width;
	}
	public void SetHeight(UInt32 height)
	{
		_height = height;
	}

	public void SetSize(UInt32 width, UInt32 height)
	{
		_width = width;
		_height = height;
		for(int i = 0; i < width * height; i++)
		{
			_plants.Add(null);
		}
		for(int i = 0; i < Enum.GetNames(typeof(PlantTypes.Type)).Length; i++)
		{
			_harvestQueues[i] = new Queue<UInt32>();
		}
	}

	public bool GetPlantAtGridPosition(UInt32 x, UInt32 y, out Plant result) {
		result = null;

		if(_plants.Count < y * _width + x) { return false; }

		result = _plants[(int)(y * _width + x)];
		return true;
	}

	// assumes there's not already a plant at this position.
	// TODO -- should we handle checking validity at the
	// input level?
	public void SpawnPlantAtGridPosition(UInt32 x, UInt32 y, PlantTypes.Type type) {
		UInt32 plantId = y * _width + x;

		Plant newPlant = null;

		switch(type) {
			case PlantTypes.Type.EYE_WEED:
				newPlant = new EyeWeed();
				break;
			default: return; // TODO - send an error
		}

		newPlant.AssignId(plantId);
		_plants[(int)plantId] = newPlant;
		
		newPlant.OnHarvestRequested += AddPlantToHarvestQueue;
	}

	private void AddPlantToHarvestQueue(UInt32 id) {
		Plant plant = _plants[(int)id];

		_harvestQueues[(int)plant.type].Enqueue(id);
	}

	public UInt32 QueryAdjacentTiles(UInt32 sourceId, GridQueryConfig config, Func<Plant, bool> criteria) {
		UInt32 matches = 0;

		int sourceX = (int)(sourceId % _width);
		int sourceY = (int)(sourceId / _width);

		for(int yOffset = -1; yOffset < 2; yOffset++) {
			for(int xOffset = -1; xOffset < 2; xOffset++) {
				if(yOffset == 0 && xOffset == 0) { continue; }

				int adjacentX = sourceX + xOffset;
				if(adjacentX < 0 || (int)_width - 1 < adjacentX) { continue; }

				int adjacentY = sourceY + yOffset;
				if(adjacentY < 0 || (int)_height - 1 < adjacentY) { continue; }
				
				int adjacentId = (int)_width * adjacentY + adjacentX;

				if (_plants[adjacentId] == null) continue;
				if(criteria.Invoke(_plants[adjacentId])) {
					if(config.matchesRequired - 1 < matches++) { return matches; }
				}
			}
		}

		return matches;
	}

	public void ResolveHarvesting() {
		foreach(Queue<UInt32> queue in _harvestQueues) {
			if (queue == null) continue;

			foreach(UInt32 toHarvest in queue) {
				Plant plant = _plants[(int)toHarvest];

				plant.Harvest(QueryAdjacentTiles);
			}
		}

		foreach(Queue<UInt32> queue in _harvestQueues) {
			if (queue == null) continue;

			while(queue.TryDequeue(out UInt32 toHarvest)) {
				Plant plant = _plants[(int)toHarvest];

				plant.OnHarvestRequested -= AddPlantToHarvestQueue;
				plant.Payout();
				_plants[(int)toHarvest] = null;
			}
		}
	}
}
