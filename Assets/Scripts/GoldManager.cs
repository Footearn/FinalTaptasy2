using System.Collections.Generic;
using UnityEngine;

public class GoldManager : Singleton<GoldManager>
{
	public static float goldDissapearTimer = 1f;

	public float goldCatchRange = 5f;

	public Transform goldUITransform;

	public ChangeNumberAnimate[] goldValueAnimateTexts;

	private void Start()
	{
		for (int i = 0; i < goldValueAnimateTexts.Length; i++)
		{
			goldValueAnimateTexts[i].CurrentPrintType = ChangeNumberAnimate.PrintType.ChangeUnit;
		}
		displayGold(false, false);
	}

	public void startGame()
	{
		displayGold(false, false);
	}

	public void spawnGold(Vector3 spawnPosition, double value)
	{
		if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice || GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode)
		{
			Singleton<DropItemManager>.instance.spawnDropItem(DropItemManager.DropItemType.Gold, spawnPosition, value);
			return;
		}
		Dictionary<DropItemManager.DropItemType, double> totalIngameGainDropItem;
		Dictionary<DropItemManager.DropItemType, double> dictionary = (totalIngameGainDropItem = Singleton<DropItemManager>.instance.totalIngameGainDropItem);
		DropItemManager.DropItemType key;
		DropItemManager.DropItemType key2 = (key = DropItemManager.DropItemType.Gold);
		double num = totalIngameGainDropItem[key];
		dictionary[key2] = num + value;
		increaseGoldByMonster(value);
	}

	public void displayGold(bool isWithResorcesEffect, bool isIncrease)
	{
		for (int i = 0; i < goldValueAnimateTexts.Length; i++)
		{
			if (isIncrease)
			{
				if (isWithResorcesEffect)
				{
					goldValueAnimateTexts[i].SetValue(Singleton<DataManager>.instance.currentGameData.gold, 0.8f, 0.8f);
				}
				else
				{
					goldValueAnimateTexts[i].SetValue(Singleton<DataManager>.instance.currentGameData.gold, 1f);
				}
			}
			else
			{
				goldValueAnimateTexts[i].SetText(Singleton<DataManager>.instance.currentGameData.gold);
			}
		}
	}

	public void increaseGoldByMonster(double value)
	{
		if (GameManager.currentGameState != GameManager.GameState.OutGame)
		{
			if (UIWindowResult.instance.isOpen)
			{
				UIWindowResult.instance.goldText.SetValue(Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.Gold], 1f);
			}
			Singleton<QuestManager>.instance.questReport(QuestManager.QuestType.Gold, value);
		}
		increaseGold(value, false, false);
	}

	public void increaseGold(double value, bool isWithResourceEffect = false, bool isUsingLerpGoldText = true)
	{
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.Millionaire, value);
		Singleton<DataManager>.instance.currentGameData.gold = Util.Clamp(Singleton<DataManager>.instance.currentGameData.gold + value, 0.0, double.MaxValue);
		displayGold(isWithResourceEffect, isUsingLerpGoldText);
		if (GameManager.currentGameState == GameManager.GameState.OutGame)
		{
			if (UIWindowManageHeroAndWeapon.instance.isOpen)
			{
				UIWindowManageHeroAndWeapon.instance.refreshAllBuyState();
			}
			if (UIWindowColleague.instance.isOpen)
			{
				UIWindowColleague.instance.colleagueScrollRectParent.refreshAll();
			}
			if (UIWindowSkill.instance.isOpen)
			{
				UIWindowSkill.instance.skillScroll.refreshAll();
			}
		}
	}

	public void decreaseGold(double value)
	{
		Singleton<DataManager>.instance.currentGameData.gold = Util.Clamp(Singleton<DataManager>.instance.currentGameData.gold - value, 0.0, double.MaxValue);
		displayGold(false, false);
		if (GameManager.currentGameState == GameManager.GameState.OutGame)
		{
			if (UIWindowManageHeroAndWeapon.instance.isOpen)
			{
				UIWindowManageHeroAndWeapon.instance.refreshAllBuyState();
			}
			if (UIWindowColleague.instance.isOpen)
			{
				UIWindowColleague.instance.colleagueScrollRectParent.refreshAll();
			}
			if (UIWindowSkill.instance.isOpen)
			{
				UIWindowSkill.instance.skillScroll.refreshAll();
			}
		}
	}
}
