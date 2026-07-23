using System;
using System.Collections.Generic;
using System.Linq;

public class Harvest {

    // The optimal order in harvesting plants. If it is not present in this dictionary, then it will not be
    // harvested (i.e. null plant) Values are sorted in ascending order. Order is preserved for plants with same values.
    private readonly Dictionary<PlantTypes.Type, Int16> _harvestOrder = new() {
        // Example entry: [PlantTypes.Type.NULL_PLANT] = 0

        // [PlantTypes.Type.EYEWEED] = 0
        // [PlantTypes.Type.LAMB_FLOWER] = 1
        // [PlantTypes.Type.SUN_FLOWER] = 2
        // [PlantTypes.Type.BED_EGGPLANT] = 3
    };

    private static Harvest _instance;
    
    public static Harvest Instance() {
        if (_instance == null) _instance = new Harvest();
        return _instance;
    }

    /** Removes plants from the farm. Code written speculatively, can flatten abstraction as needed by game architecture

    tileGetter: Array or list of objects to check for contained plant. Represents the entire grid.
    typeGetter: Extracts the plant type from objects in the list
    harvestAction: Callback to run on the object. These are run exactly in the order as from _harvestOrder

     */
    public void DoHarvest<T>(Func<IEnumerable<T>> tileGetter, Func<T, PlantTypes.Type> typeGetter, Predicate<T> canHarvest, Action<T> harvestAction) {
        // TODO Redesign loop so that a new grid is created all at once, that collects all side effects and applies them
        //  in parallel (in theory, doesn't have to actually be async) instead of relying on cellular automata.
        //  The dict will still help in reconciling crop interactions
        tileGetter()
            .Where(o => canHarvest(o))
            // TODO pre-process by adjacency for the SunFlower+LambFlower interaction
            .Where(o => _harvestOrder.ContainsKey(typeGetter(o)))
            .OrderBy(o => _harvestOrder[typeGetter(o)])
            .ToList()
            .ForEach(harvestAction)
            ;
    }
    
}
