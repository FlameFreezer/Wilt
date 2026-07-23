using System;
namespace PlantTypes
{
    public enum Type : UInt32
    {
		EYE_WEED,
        // KEEP THESE AT THE BOTTOM - this ensures that NUM_PLANT_TYPES==the number of types above it
        //  and therefore the number of valid plant types
        NUM_PLANT_TYPES,
        NULL_PLANT
    }
}

