using System;
using System.Collections.Generic;

public abstract class Plant {
	public PlantTypes.Type type;

	public int ticksUntilHarvest = 0;
	public uint payout = 0;
	protected UInt32 _id = UInt32.MaxValue;

    public event Action<UInt32> OnHarvestRequested;

    private bool _complete = false;
    public bool Complete { get { return _complete; } set { _complete = value; } }

	public Plant(PlantTypes.Type plantType)
	{
		payout = plantType.GetPayout();
		ticksUntilHarvest = (int)plantType.GetTicksUntilHarvest();
		type = plantType;
	}

	public void AssignId(UInt32 id) {
		_id = id;
	}

	public virtual void Tick(Plot plot)
	{
		ticksUntilHarvest--;
	}

	public abstract void Harvest(Plot plot);

	public virtual bool CheckHarvest()
	{
		return !Complete && ticksUntilHarvest < 1;
	}

	protected void InvokeOnHarvestRequested() {
		OnHarvestRequested?.Invoke(_id);
	}

	public abstract void Payout();
}

public class EyeWeed : Plant {

	public EyeWeed() : base(PlantTypes.Type.EYE_WEED) { }

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += payout;
	}

	public override void Harvest(Plot plot) {
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if(adjacentPlot.plant != null && adjacentPlot.plant.type == PlantTypes.Type.EYE_WEED && adjacentPlot.plant.ticksUntilHarvest < 1)
			{
				payout = (UInt32)(payout * 1.5);
				break;
			}
		}
		Complete = true;
	}
}

public class Lambflower : Plant
{
	public Lambflower() : base(PlantTypes.Type.LAMBFLOWER) {}

	public override bool CheckHarvest()
	{
		return !Complete && ticksUntilHarvest <= 3;
	}

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += payout;
	}

    public override void Harvest(Plot plot)
    {
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if (adjacentPlot.plant != null && adjacentPlot.plant.ticksUntilHarvest < 1)
			{
				ticksUntilHarvest = 0;
			}
		}
		Complete = true;
    }
}

public class Fusspot : Plant
{
	private UInt32 _payoutPerSynergy = 5;

	public Fusspot() : base(PlantTypes.Type.FUSSPOT) {}

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += payout;
	}

	public override void Harvest(Plot plot)
	{
		// Apply time reduction bonus
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if (adjacentPlot.plant == null) continue;
			Plant adjacentPlant = adjacentPlot.plant;
			if (adjacentPlant.Complete) continue;
			if (adjacentPlant.type == PlantTypes.Type.FUSSPOT) continue;
			if (adjacentPlant.type == PlantTypes.Type.TOADSTOOL && (adjacentPlant as Toadstool).isTraveler) continue;
			adjacentPlant.ticksUntilHarvest -= 2;
		}

		// Apply synergy
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if (adjacentPlot.plant == null) continue;
			if (adjacentPlot.plant.type == PlantTypes.Type.FUSSPOT && adjacentPlot.plant.ticksUntilHarvest < 1)
			{
				payout += _payoutPerSynergy;
			}
		}

		Complete = true;
	}
}

public class Toadstool : Plant
{
	public bool isTraveler = false;

	public Toadstool() : base(PlantTypes.Type.TOADSTOOL) {}

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += payout;
	}

	public override void Harvest(Plot plot)
	{
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if (adjacentPlot.plant != null && adjacentPlot.plant.ticksUntilHarvest < 1)
			{
				List<Plot> openPlots = new();
				foreach(Plot adj in plot.GetAdjacentPlots())
				{
					if (adj.plant == null)
					{
						openPlots.Add(adj);						
					}
				}
				// If there are no open spots, don't try to place a new toadstool
				if (openPlots.Count < 1) break;

				int index = UnityEngine.Random.Range(0, openPlots.Count);
				Toadstool traveler = openPlots[index].PlacePlant(PlantTypes.Type.TOADSTOOL) as Toadstool;
				traveler.payout = 2;
				traveler.ticksUntilHarvest = 1;
				traveler.isTraveler = true;
				break;
			}
		}
        Complete = true;
	}
}

public class Cthulily : Plant
{
	private double _payoutMultiplier = 3.0;
	public Cthulily() : base(PlantTypes.Type.CTHULILY) {}

	public override void Payout()
	{
		Game.Instance().globalTimer.addTimeCostModifier *= 0.9;
	}

    public override void Harvest(Plot plot)
    {
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if(adjacentPlot.plant != null && adjacentPlot.plant.type == PlantTypes.Type.CTHULILY)
			{
				Complete = true;
				return;
			}
		}
		plot.GetPosition(out uint xIndex, out uint yIndex);
		GridController grid = plot.GetParentGrid();
		Plot diagonalPlot = null;
		void ApplyBonus(Plot diagonalPlot)
		{
			if (diagonalPlot.plant != null)
			{
				diagonalPlot.plant.payout += (uint)(diagonalPlot.plant.type.GetPayout() * _payoutMultiplier);
			}
		}
		// Up left
		if(grid.GetPlot2D(xIndex - 1, yIndex - 1, out diagonalPlot))
		{
			ApplyBonus(diagonalPlot);
		}
		//Up right
		if(grid.GetPlot2D(xIndex + 1, yIndex - 1, out diagonalPlot))
		{
			ApplyBonus(diagonalPlot);
		}
		// Down left
		if(grid.GetPlot2D(xIndex - 1, yIndex + 1, out diagonalPlot))
		{
			ApplyBonus(diagonalPlot);
		}
		// Down right
		if(grid.GetPlot2D(xIndex + 1, yIndex + 1, out diagonalPlot))
		{
			ApplyBonus(diagonalPlot);
		}

        Complete = true;
    }
}

public class Fingerling : Plant
{

	public Fingerling() : base(PlantTypes.Type.FINGERLING) {}

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += payout;
	}

    public override void Harvest(Plot plot)
    {
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if (adjacentPlot.plant == null) continue;
			if (adjacentPlot.plant.type == PlantTypes.Type.FUSSPOT && adjacentPlot.plant.ticksUntilHarvest < 1)
			{
				foreach(Plot adj in plot.GetAdjacentPlots())
				{
					if (adj.plant == null) continue;
					if (adj.plant.type == PlantTypes.Type.FUSSPOT && adj.plant.ticksUntilHarvest < 1) continue;

                    adj.plant.Complete = true;
                    adj.plant.ticksUntilHarvest = 1;
				}
			}
		}
		Complete = true;
    }
}

public class Voidbeet : Plant
{
	private double _multiplierPerSynergy = 1.5;
	private uint _numSynergies = 0;
	public Voidbeet() : base(PlantTypes.Type.HUNGERING_VOIDBEET) {}

    public override bool CheckHarvest()
    {
		return false;
    }

	public override void Payout()
	{
		if (_numSynergies == 0) Game.Instance().Player().money += payout;
		else Game.Instance().Player().money += (uint)(payout * _multiplierPerSynergy * _numSynergies);
	}

    public override void Harvest(Plot plot)
    {
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if (adjacentPlot.plant == null) continue;
			if (adjacentPlot.plant.ticksUntilHarvest >= 1) continue;
			adjacentPlot.plant.payout = 0;
			_numSynergies++;
		}
		Complete = true;
    }
}

public class Shyweed : Plant
{
	private bool _beginTicking = false;
	private double _synergyMultiplier = 1.4;
	public Shyweed() : base(PlantTypes.Type.SHYWEED) {}

    public override void Payout()
    {
		Game.Instance().Player().money += payout;
    }

    public override void Tick(Plot plot)
    {
		if (_beginTicking)
		{
            --ticksUntilHarvest;
		}
		// Don't start ticking unless an adjacent plant is next to it
		else
		{
			foreach (Plot adjacentPlot in plot.GetAdjacentPlots())
			{
				if (adjacentPlot.plant != null)
				{
					_beginTicking = true;
					--ticksUntilHarvest;
					return;
				}
			}
		}
    }

    public override void Harvest(Plot plot)
    {
		uint numAdjacentEyeweeds = 0;
		uint numAdjacentShyweeds = 0;
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if (adjacentPlot.plant == null) continue;
			if (adjacentPlot.plant.type == PlantTypes.Type.SHYWEED && adjacentPlot.plant.ticksUntilHarvest < 1)
			{
				numAdjacentShyweeds++;
			}
			else if (adjacentPlot.plant.type == PlantTypes.Type.EYE_WEED && adjacentPlot.plant.ticksUntilHarvest < 1)
			{
				numAdjacentEyeweeds++;
			}
		}
		if(numAdjacentShyweeds == 2 || numAdjacentEyeweeds == 2)
		{
			payout = (uint)(payout * _synergyMultiplier);
		}
        Complete = true;
    }
}