using System;
using System.Diagnostics;

public abstract class Plant {
	public PlantTypes.Type type;

	public int ticksUntilHarvest = 0;
	protected UInt32 _id = UInt32.MaxValue;

    public event Action<UInt32> OnHarvestRequested;

	public void AssignId(UInt32 id) {
		_id = id;
	}

	public void Tick()
	{
		ticksUntilHarvest--;
		if(ticksUntilHarvest < 1)
		{
			InvokeOnHarvestRequested();
		}
	}

	public abstract void Harvest(Func<UInt32, GridQueryConfig, Func<Plant, bool>, UInt32> adjacentQueryCallback);

	protected void InvokeOnHarvestRequested() {
		OnHarvestRequested?.Invoke(_id);
	}

	public abstract void Payout();
}

public class EyeWeed : Plant {
	private UInt32 _payout = 5;

	public EyeWeed()
	{
		ticksUntilHarvest = 3;
		type = PlantTypes.Type.EYE_WEED;
	}

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += _payout;
	}

	public override void Harvest(Func<UInt32, GridQueryConfig, Func<Plant, bool>, UInt32> adjacentQueryCallback) {
		if(adjacentQueryCallback.Invoke(_id, new() { matchesRequired = 1 }, _Criteria) > 0) {
			_payout = (UInt32)(_payout * 1.5);
		}

		return;

		bool _Criteria(Plant subject) {
			return subject != null && subject.type == PlantTypes.Type.EYE_WEED && subject.ticksUntilHarvest < 1;
		}
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

	public override void Payout()
	{
		Game.Instance()._player.GetComponent<Player>().money += _payout;
	}

    public override void Harvest(Func<uint, GridQueryConfig, Func<Plant, bool>, uint> adjacentQueryCallback)
    {
        throw new NotImplementedException();
    }
}
