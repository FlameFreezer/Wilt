using System;
using System.Collections.Generic;
using System.Xml.Serialization;
public class PlantTypes
{
    public enum Type : int
    {
		EYE_WEED,
        LAMBFLOWER,

        //KEEP AT BOTTOM
        NULL_PLANT,
    }

    public static string TypeToString(PlantTypes.Type type)
    {
        switch(type)
        {
            case Type.EYE_WEED: return "Eyeweed";
            case Type.LAMBFLOWER: return "Lambflower";
            case Type.NULL_PLANT: return "NULL_PLANT";
        }
        // UNREACHBALE
        throw new InvalidProgramException();
    }

    public static Dictionary<Type, uint> costs = new()
    {
        { Type.EYE_WEED, 2 },
        { Type.LAMBFLOWER, 7 },
    };
}

