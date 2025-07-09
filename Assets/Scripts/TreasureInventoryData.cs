using System;

[Serializable]
public class TreasureInventoryData
{
	public TreasureManager.TreasureType treasureType;

	public double damagePercentValue;

	public double treasureEffectValue;

	public double extraTreasureEffectValue;

	public long baseTreasurePrice;

	public long baseTreasureIndex;

	public long treasureLevel = 1L;
}
