using System;

public abstract class Plant {
	protected int _ticksUntilHarvest = 0;

    public event Action OnHarvestRequested;

	public abstract void Tick();
}
