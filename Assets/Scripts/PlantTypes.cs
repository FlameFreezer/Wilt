using System;
using System.Collections.Generic;
public class PlantTypes
{
    public enum Type : UInt32
    {
        NULL_PLANT,
		EYE_WEED,
    }

    public static Dictionary<Type, uint> costs = new()
    {
        { Type.EYE_WEED, 2 }
    };
}

