using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantTypes
{
    public enum Type : int
    {
		EYE_WEED,
        LAMBFLOWER,
        FUSSPOT,
        TOADSTOOL,
        CTHULILY,
        //KEEP AT BOTTOM
        NULL_PLANT,
    }

    public static string TypeToString(PlantTypes.Type type)
    {
        switch(type)
        {
            case Type.EYE_WEED: return "Eyeweed";
            case Type.LAMBFLOWER: return "Lambflower";
            case Type.FUSSPOT: return "Fusspot";
            case Type.TOADSTOOL: return "Traveling Toadstool";
            case Type.CTHULILY: return "Cthulily";
            case Type.NULL_PLANT: return "NULL_PLANT";
        }
        // UNREACHBALE
        throw new InvalidProgramException();
    }

	public static Type StringToType(string str) {
		return str.ToLower() switch {
			"eye_weed" => Type.EYE_WEED,
			"lambflower" => Type.LAMBFLOWER,
			_ => Type.NULL_PLANT
		};
	}

	public static Sprite TypeToPortrait(Type type) {
		return Resources.Load<Sprite>($"PlantPortraits/{TypeToString(type)}");
	}

	public static Sprite TypeToSprite(Type type) {
		// TODO - separate from portrait-specific method for animations
		return TypeToPortrait(type);
	}

    public static Dictionary<Type, uint> costs = new()
    {
        { Type.EYE_WEED, 1 },
        { Type.LAMBFLOWER, 7 },
        { Type.FUSSPOT, 20 },
        { Type.TOADSTOOL, 16 },
        { Type.CTHULILY, 45 },
    };
}

