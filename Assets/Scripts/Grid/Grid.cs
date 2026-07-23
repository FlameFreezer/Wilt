using System.Collections.Generic;

public class Grid {
    private List<Plant> _plants;

	public void InvokeTick(PlantTypes.Type type) {
		foreach(Plant plant in _plants) {
			if(plant.type == type) {
				plant.Tick();
			}
		}
	}
}
