using System;

[Serializable]
public struct TreasureStatData
{
	public TreasureManager.TreasureType currentTreasureType;

	public float damagePercentValue;

	public double treasureEffectValue;

	public int treasureTier;

	public double baseEnchantStonePrice;

	public double baseEnchantRubyPrice;

	public long maxLevel;

	public double increasingValueEveryEnchant;

	public double increasingEnchantStonePriceEveryEnchant;

	public double increasingRubyPriceEveryEnchant;
}
