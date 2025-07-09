using System;
using UnityEngine;

public class CalculateManager : Singleton<CalculateManager>
{
	public static double getGoldValueForMonsters()
	{
		double currentStandardGold = getCurrentStandardGold();
		currentStandardGold = ((GameManager.currentTheme <= 100) ? (currentStandardGold * 25.0 / (double)(Singleton<MapManager>.instance.getMaxFloor(Singleton<DataManager>.instance.currentGameData.currentTheme) * 5)) : (currentStandardGold * 25.0 / (double)(Singleton<MapManager>.instance.getMaxFloor(Singleton<DataManager>.instance.currentGameData.currentTheme) * 5 + 50)));
		return Math.Max(currentStandardGold, 0.0);
	}

	public static double getCurrentStandardGold()
	{
		double num = 0.0;
		num = Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.GoldVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.GoldVariable2), Singleton<DataManager>.instance.currentGameData.unlockTheme - 1) * (1.0 + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.GoldVariable3) * (double)(Singleton<DataManager>.instance.currentGameData.unlockStage - 1));
		num += num / 100.0 * Singleton<StatManager>.instance.percentGoldGain;
		num += num / 100.0 * Singleton<StatManager>.instance.weaponSkinGoldGain;
		return Math.Max(num, 0.0);
	}

	public static float getCurrentDelay(float baseDelay)
	{
		float num = 0f;
		return baseDelay;
	}

	public static float getCurrentMoveSpeed(float baseSpeed)
	{
		float num = 0f;
		return baseSpeed;
	}

	public static float getCurrentCriticalProbability(float baseCriticalProbability)
	{
		float num = 0f;
		num = baseCriticalProbability;
		return Mathf.Min(num, 100f);
	}

	public static float getCurrentCriticalDamage(float baseCriticalDamage)
	{
		float num = 1f;
		return baseCriticalDamage;
	}
}
