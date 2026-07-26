using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public struct PlantTypeMetadataType {
	public string name;
	public string nameCapitalized;
	public uint cost;
	public uint payout;
	public uint ticksUntilHarvest;
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

	public static uint GetPayout(this PlantTypes.Type element)
	{
		var attribute = element.GetType().GetField(element.ToString()).GetCustomAttribute<PlantTypes.PlantTypeMetadataAttribute>();
		if (attribute == null) return uint.MaxValue;

		return attribute.Metadata.payout;
	}
	public static uint GetTicksUntilHarvest(this PlantTypes.Type element)
	{
		var attribute = element.GetType().GetField(element.ToString()).GetCustomAttribute<PlantTypes.PlantTypeMetadataAttribute>();
		if (attribute == null) return uint.MaxValue;

		return attribute.Metadata.ticksUntilHarvest;
	}

}

public class PlantTypes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class PlantTypeMetadataAttribute : Attribute {
		public PlantTypeMetadataType Metadata { get; }

		public PlantTypeMetadataAttribute(string name, string nameCapitalized, uint cost, uint payout, uint ticksUntilHarvest) {
			Metadata = new() {
				name = name,
				nameCapitalized = nameCapitalized,
				cost = cost,
				payout = payout,
				ticksUntilHarvest = ticksUntilHarvest,
			};
		}
	}

    public enum Type : int {
		[PlantTypeMetadata("eyeweed", "Eyeweed", 1, 2, 5)]
		EYE_WEED,
		[PlantTypeMetadata("lambflower", "Lambflower", 7, 9, 8)]
        LAMBFLOWER,
		[PlantTypeMetadata("fusspot", "Fusspot", 20, 25, 18)]
        FUSSPOT,
		[PlantTypeMetadata("toadstool", "Traveling Toadstool", 16, 18, 10)]
        TOADSTOOL,
		[PlantTypeMetadata("cthulily", "Cthulily", 45, 25, 20)]
        CTHULILY,
		[PlantTypeMetadata("shyweed", "Shyweed", 14, 17, 20)]
		SHYWEED,
		[PlantTypeMetadata("fingerling", "Fingerling", 28, 40, 19)]
        FINGERLING,
		[PlantTypeMetadata("hungeringvoidbeet", "Hungering Voidbeet", 70, 20, 30)]
        HUNGERING_VOIDBEET,
        //KEEP AT BOTTOM
		[PlantTypeMetadata("nullplant", "Nullplant", uint.MaxValue, 0, uint.MaxValue)]
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

	public static Image TypeToImage(Type type)
	{
		return Resources.Load<Image>($"PlantPortraits/{type.GetName()}");
	}
}

