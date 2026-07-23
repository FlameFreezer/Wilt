using System.Collections.Generic;

public class Grid {
    private List<Plant> _plants;

	public int width;
	public int height;

	public void InvokeTick(PlantTypes.Type type) {
		foreach(Plant plant in _plants) {
			if(plant.type == type) {
				plant.Tick();
			}
		}
	}

	public bool GetPlantAtGridPosition(int x, int y, out Plant result) {
		result = null;

		if(_plants.Count < y * width + x) { return false; }

		result = _plants[y * width + x];
		return true;
	}
}
