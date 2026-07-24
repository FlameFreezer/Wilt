using System;
using System.Diagnostics;
using Unity.VisualScripting;

public abstract class Plant {
	public PlantTypes.Type type;

	public int ticksUntilHarvest = 0;
	protected UInt32 _id = UInt32.MaxValue;

    public event Action<UInt32> OnHarvestRequested;

    private bool _complete = false;
    public bool Complete { get { return _complete; } set { _complete = value; } }

	public void AssignId(UInt32 id) {
		_id = id;
	}

	public void Tick()
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
	private UInt32 _payout = 2;

	public EyeWeed()
	{
		ticksUntilHarvest = 3;
		type = PlantTypes.Type.EYE_WEED;
	}

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += _payout;
	}

	public override void Harvest(Plot plot) {
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if(adjacentPlot.plant != null && adjacentPlot.plant.type == PlantTypes.Type.EYE_WEED && adjacentPlot.plant.ticksUntilHarvest < 1)
			{
				_payout = (UInt32)(_payout * 1.5);
				break;
			}
		}
		Complete = true;
	}
}

public class Lambflower : Plant
{
	private UInt32 _payout = 9;

	public Lambflower()
	{
		ticksUntilHarvest = 8;
		type = PlantTypes.Type.LAMBFLOWER;
	}

	public override bool CheckHarvest()
	{
		return !Complete && ticksUntilHarvest <= 3;
	}

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += _payout;
	}

    public override void Harvest(Plot plot)
    {
		Complete = true;
		ticksUntilHarvest = 0;
        return;
    }
}

public class Fusspot : Plant
{
	private UInt32 _payout = 25;
	private UInt32 _payoutPerSynergy = 5;

	public Fusspot()
	{
		ticksUntilHarvest = 18;
		type = PlantTypes.Type.FUSSPOT;
	}

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += _payout;
	}

	public override void Harvest(Plot plot)
	{
		// Apply time reduction bonus
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if (adjacentPlot.plant != null && adjacentPlot.plant.type != PlantTypes.Type.FUSSPOT)
			{
				adjacentPlot.plant.ticksUntilHarvest -= 2;
			}
		}

		// Apply synergy
		foreach(Plot adjacentPlot in plot.GetAdjacentPlots())
		{
			if (adjacentPlot.plant == null) continue;
			if (adjacentPlot.plant.type == PlantTypes.Type.FUSSPOT && adjacentPlot.plant.ticksUntilHarvest < 1)
			{
				_payout += _payoutPerSynergy;
			}
		}

		Complete = true;
	}
}
