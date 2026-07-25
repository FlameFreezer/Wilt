using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public struct PlantTypeMetadataType {
	public string name;
	public string nameCapitalized;
	public uint cost;
}

public static class PlantTypeMetadataExtensions {
	public static string GetName(this PlantTypes.Type element) {
		var attribute = element.GetType().GetField(element.ToString()).GetCustomAttribute<PlantTypes.PlantTypeMetadataAttribute>();
		if(attribute == null) { return "nullPlantName"; }

		return attribute.Metadata.name;
	}

	public static string GetNameCapitalized(this PlantTypes.Type element) {
		var attribute = element.GetType().GetField(element.ToString()).GetCustomAttribute<PlantTypes.PlantTypeMetadataAttribute>();
		if(attribute == null) { return "NullPlantName"; }

		return attribute.Metadata.nameCapitalized;
	}

	public static uint GetCost(this PlantTypes.Type element) {
		var attribute = element.GetType().GetField(element.ToString()).GetCustomAttribute<PlantTypes.PlantTypeMetadataAttribute>();
		if(attribute == null) { return uint.MaxValue; }

		return attribute.Metadata.cost;
	}
}

public class PlantTypes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class PlantTypeMetadataAttribute : Attribute {
		public PlantTypeMetadataType Metadata { get; }

		public PlantTypeMetadataAttribute(string name, string nameCapitalized, uint cost) {
			Metadata = new() {
				name = name,
				nameCapitalized = nameCapitalized,
				cost = cost,
			};
		}
	}

    public enum Type : int {
		[PlantTypeMetadata("eyeweed", "Eyeweed", 1)]
		EYE_WEED,
		[PlantTypeMetadata("lambflower", "Lambflower", 7)]
        LAMBFLOWER,
		[PlantTypeMetadata("fusspot", "Fusspot", 20)]
        FUSSPOT,
		[PlantTypeMetadata("toadstool", "Toadstool", 16)]
        TOADSTOOL,
		[PlantTypeMetadata("cthulily", "Cthulily", 45)]
        CTHULILY,
        FINGERLING,
        //KEEP AT BOTTOM
		[PlantTypeMetadata("nullplant", "Nullplant", uint.MaxValue)]
        NULL_PLANT,
    }

	public static Type TypeFromString(string str) {
		string lowerString = str.ToLower();

		foreach(Type type in Enum.GetValues(typeof(Type))) {
			if(type.GetName() == lowerString) { return type; }
		}

		return Type.NULL_PLANT;
	}

	public static Sprite TypeToPortrait(Type type) {
		return Resources.Load<Sprite>($"PlantPortraits/{type.GetName()}");
	}

	public static Sprite TypeToSprite(Type type) {
		// TODO - separate from portrait-specific method for animations
		return TypeToPortrait(type);
	}
}

