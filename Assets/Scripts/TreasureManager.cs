using System;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using UnityEngine;
using UnityEngine.UI;

public class TreasureManager : Singleton<TreasureManager>
{
	public enum TreasureType
	{
		Null,
		PrincessRing,
		PrincessNecklace,
		PrincessCheckCard,
		ScaleOfGoldenDragon,
		ToothOfBlueDragon,
		CatFoot,
		HeavenShield,
		HeliosHarp,
		ChaliceOfMiracle,
		CharmOfLunarGoddess,
		FeatherOfPhoenix,
		AncientGoldenCrown,
		AncientGoldenMask,
		HeaddressOfIceQueen,
		ProghetCrystalBall,
		OcarinaOfForestSpirit,
		MonocleOfWiseGoddess,
		RestedKey,
		MythrilArmorSet,
		DevilResortVIPInvitaion,
		ConquerorRing,
		NobleBlade,
		RustedShield,
		NecklaceOfSorcerers,
		DustedCrown,
		BrokenMask,
		SharpIcicle,
		WitchCrystalBall,
		RoyalGoldenKey,
		MythrilUnderwear,
		DragonNail,
		YoYo,
		MysteriousJewel,
		ConfusionalJewel,
		DragonGem,
		MeteorPiece,
		IceCrystal,
		SeraphHope,
		SeraphBless,
		FireHand,
		IceHand,
		LegendSkillBook,
		ChaosBlade,
		FireDragonHeart,
		IceHeart,
		AntiquityWarriorHelmet,
		BlessHood,
		WarriorCape,
		AngelHairPin,
		ArcherArrow,
		ConquerToken,
		PatienceToken,
		Length
	}

	public enum CooltimeTreasureEffectType
	{
		DestroyTreasureChest
	}

	public int maxTreasureCount = 1;

	public Text[] treasureChantStoneTexts;

	public Sprite[] tierBackgroundSprites;

	private TreasureType m_targetLotteryTreasure;

	public Sprite[] treasureIconSprites;

	public float treasureCatchRange = 5f;

	public Sprite rarePieceSprite;

	public Transform moveTargetTransform;

	private List<TreasurePieceObject> m_cachedDynamicList;

	public List<TreasurePieceObject> treasurePieceObjects;

	public long ingameTotalTreasurePiece;

	public long currentLotteryPriceForTreasurePiece
	{
		get
		{
			return 30L;
		}
	}

	public long currentLotteryPriceForRuby
	{
		get
		{
			return 300L;
		}
	}

	private void Awake()
	{
		m_cachedDynamicList = new List<TreasurePieceObject>();
		treasurePieceObjects = new List<TreasurePieceObject>();
	}

	public bool levelUpTreasure(TreasureType treasureType, bool isEnchantByRuby)
	{
		TreasureInventoryData treasureDataFromInventory = getTreasureDataFromInventory(treasureType);
		TreasureStatData treasureStatData = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType];
		if (treasureDataFromInventory.treasureLevel < treasureStatData.maxLevel)
		{
			if (!isEnchantByRuby)
			{
				if (Singleton<DataManager>.instance.currentGameData.treasureEnchantStone >= getCurrentTreasureLevelUpEnchantStonePrice(treasureType))
				{
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Treasure, AnalyzeManager.ActionType.EnchantTreasure, new Dictionary<string, string>
					{
						{
							"TreasureType",
							treasureType.ToString()
						}
					});
					decreaseTreasureEnchantStone(getCurrentTreasureLevelUpEnchantStonePrice(treasureType));
					treasureDataFromInventory.treasureLevel++;
					refreshTreasureInventoryData();
					UIWindowManageTreasure.instance.refreshSlots();
					Singleton<DataManager>.instance.saveData();
					return true;
				}
				UIWindowDialog.openDescription("NO_HAVE_TREASURE_ENCHANT_STONE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
				return false;
			}
			if (Singleton<DataManager>.instance.currentGameData._ruby >= getCurrentTreasureLevelUpRubyPrice(treasureType))
			{
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Treasure, AnalyzeManager.ActionType.EnchantTreasure, new Dictionary<string, string>
				{
					{
						"TreasureType",
						treasureType.ToString()
					}
				});
				Singleton<RubyManager>.instance.decreaseRuby(getCurrentTreasureLevelUpRubyPrice(treasureType));
				treasureDataFromInventory.treasureLevel++;
				refreshTreasureInventoryData();
				UIWindowManageTreasure.instance.refreshSlots();
				Singleton<DataManager>.instance.saveData();
				return true;
			}
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
			return false;
		}
		UIWindowDialog.openDescription("ALREADY_MAX_TREASURE_ENCHANT", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		return false;
	}

	public void increaseTreasureEnchantStone(double value)
	{
		Singleton<DataManager>.instance.currentGameData.treasureEnchantStone += value;
		displayTreasureEnchantStone();
	}

	public void decreaseTreasureEnchantStone(double value)
	{
		Singleton<DataManager>.instance.currentGameData.treasureEnchantStone -= value;
		displayTreasureEnchantStone();
	}

	public void displayTreasureEnchantStone()
	{
		for (int i = 0; i < treasureChantStoneTexts.Length; i++)
		{
			treasureChantStoneTexts[i].text = Singleton<DataManager>.instance.currentGameData.treasureEnchantStone.ToString("N0");
		}
	}

	public void refreshPMAndAMTreasureState(MiniPopupManager.MiniPopupType miniPopupType = MiniPopupManager.MiniPopupType.None)
	{
		if (UnbiasedTime.Instance.Now().Hour >= 9 && UnbiasedTime.Instance.Now().Hour <= 20)
		{
			if (containTreasureFromInventory(TreasureType.HeliosHarp))
			{
				DateTime endTime = new DateTime(UnbiasedTime.Instance.Now().Year, UnbiasedTime.Instance.Now().Month, UnbiasedTime.Instance.Now().Day, 21, 0, 0);
				MiniPopupManager.MiniPopupType popupTypeForRegist2 = MiniPopupManager.MiniPopupType.TreasureDamageForPM;
				if (miniPopupType != popupTypeForRegist2)
				{
					Singleton<MiniPopupManager>.instance.registryMiniPopupData(popupTypeForRegist2, false, endTime, delegate
					{
						refreshPMAndAMTreasureState(popupTypeForRegist2);
						if (GameManager.currentGameState != 0)
						{
							Singleton<StatManager>.instance.refreshAllStats();
						}
					});
				}
			}
		}
		else if (Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.TreasureDamageForPM))
		{
			Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.TreasureDamageForPM).closeMiniPopupObject(false);
		}
		if (UnbiasedTime.Instance.Now().Hour <= 5 || UnbiasedTime.Instance.Now().Hour >= 21)
		{
			if (!containTreasureFromInventory(TreasureType.CharmOfLunarGoddess))
			{
				return;
			}
			DateTime endTime2 = UnbiasedTime.Instance.Now();
			if (UnbiasedTime.Instance.Now().Hour >= 21)
			{
				endTime2 = new DateTime(UnbiasedTime.Instance.Now().Year, UnbiasedTime.Instance.Now().Month, UnbiasedTime.Instance.Now().Day, 6, 0, 0);
				endTime2 = endTime2.AddDays(1.0);
			}
			else if (UnbiasedTime.Instance.Now().Hour <= 5)
			{
				endTime2 = new DateTime(UnbiasedTime.Instance.Now().Year, UnbiasedTime.Instance.Now().Month, UnbiasedTime.Instance.Now().Day, 6, 0, 0);
			}
			MiniPopupManager.MiniPopupType popupTypeForRegist = MiniPopupManager.MiniPopupType.TreasureDamageForAM;
			if (miniPopupType == popupTypeForRegist)
			{
				return;
			}
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(popupTypeForRegist, false, endTime2, delegate
			{
				refreshPMAndAMTreasureState(popupTypeForRegist);
				if (GameManager.currentGameState != 0)
				{
					Singleton<StatManager>.instance.refreshAllStats();
				}
			});
		}
		else if (Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.TreasureDamageForAM))
		{
			Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.TreasureDamageForAM).closeMiniPopupObject(false);
		}
	}

	public void calculateAllStatOfTreasure()
	{
		refreshTreasureInventoryData();
		List<TreasureInventoryData> treasureInventoryData = Singleton<DataManager>.instance.currentGameData.treasureInventoryData;
		for (int i = 0; i < treasureInventoryData.Count; i++)
		{
			TreasureInventoryData treasureInventoryData2 = treasureInventoryData[i];
			Singleton<StatManager>.instance.allPercentDamageFromTreasure += treasureInventoryData2.damagePercentValue;
			double num = treasureInventoryData2.treasureEffectValue + treasureInventoryData2.extraTreasureEffectValue;
			switch (treasureInventoryData2.treasureType)
			{
			case TreasureType.PrincessRing:
				Singleton<StatManager>.instance.colleagueLevelUpDiscountPercent += num;
				break;
			case TreasureType.PrincessNecklace:
				Singleton<StatManager>.instance.weaponUpgradeDiscountPercent += num;
				break;
			case TreasureType.PrincessCheckCard:
				Singleton<StatManager>.instance.percentGoldGain += num;
				break;
			case TreasureType.ScaleOfGoldenDragon:
				Singleton<StatManager>.instance.treasureChestDropExtraPercentChance += (float)num;
				break;
			case TreasureType.ToothOfBlueDragon:
				Singleton<StatManager>.instance.rubyDropExtraPercentChance += (float)num;
				break;
			case TreasureType.CatFoot:
				Singleton<StatManager>.instance.skillExtraPercentDamage += num;
				break;
			case TreasureType.HeavenShield:
				Singleton<StatManager>.instance.allPercentHealthFromTreasure += num;
				break;
			case TreasureType.HeliosHarp:
				if (UnbiasedTime.Instance.Now().Hour >= 9 && UnbiasedTime.Instance.Now().Hour <= 20)
				{
					Singleton<StatManager>.instance.percentDamageAM += num;
				}
				else
				{
					Singleton<StatManager>.instance.percentDamageAM = 0.0;
				}
				break;
			case TreasureType.ChaliceOfMiracle:
				Singleton<StatManager>.instance.questRewardDoubleChance += num;
				break;
			case TreasureType.CharmOfLunarGoddess:
				if (UnbiasedTime.Instance.Now().Hour <= 5 || UnbiasedTime.Instance.Now().Hour >= 21)
				{
					Singleton<StatManager>.instance.percentDamagePM += num;
				}
				else
				{
					Singleton<StatManager>.instance.percentDamagePM = 0.0;
				}
				break;
			case TreasureType.FeatherOfPhoenix:
				Singleton<StatManager>.instance.revivePercentHealth += num;
				break;
			case TreasureType.AncientGoldenCrown:
				Singleton<StatManager>.instance.allCriticalChanceFromTreasure += (float)num;
				break;
			case TreasureType.AncientGoldenMask:
				Singleton<StatManager>.instance.allCriticalDamage += num;
				break;
			case TreasureType.HeaddressOfIceQueen:
				Singleton<StatManager>.instance.colleaguePassiveSkillExtraValue += num;
				break;
			case TreasureType.ProghetCrystalBall:
				Singleton<StatManager>.instance.manaGatherExtraValue += (float)num;
				break;
			case TreasureType.OcarinaOfForestSpirit:
				Singleton<StatManager>.instance.startManaCount += (int)num;
				break;
			case TreasureType.RestedKey:
				Singleton<StatManager>.instance.allPercentAttackDamageWhenDestroyTreasureChest += (float)num;
				break;
			case TreasureType.MythrilArmorSet:
				Singleton<StatManager>.instance.evasionPercentExtraChance += num;
				break;
			case TreasureType.DevilResortVIPInvitaion:
				Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses += num;
				break;
			case TreasureType.ConquerorRing:
				Singleton<StatManager>.instance.extraPercentDamageFromConquerorRing += num;
				break;
			case TreasureType.NobleBlade:
				Singleton<StatManager>.instance.extraPercentDamageFromNobleBlade += num;
				break;
			case TreasureType.RustedShield:
				Singleton<StatManager>.instance.decreaseDamageFromHitBoss += num;
				break;
			case TreasureType.NecklaceOfSorcerers:
				Singleton<StatManager>.instance.maxExtraMana += (int)num;
				break;
			case TreasureType.DustedCrown:
				Singleton<StatManager>.instance.percentJackPotRewardGain += num;
				break;
			case TreasureType.BrokenMask:
				Singleton<StatManager>.instance.characterSkinEffectBonus += num;
				break;
			case TreasureType.SharpIcicle:
				Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters += num;
				break;
			case TreasureType.WitchCrystalBall:
				Singleton<StatManager>.instance.extraPercentTapHealValueFromTreasure += num;
				break;
			case TreasureType.RoyalGoldenKey:
				Singleton<StatManager>.instance.bossGoldGain += num;
				break;
			case TreasureType.MythrilUnderwear:
				Singleton<StatManager>.instance.percentMovementSpeed += (float)num;
				break;
			case TreasureType.DragonNail:
				Singleton<StatManager>.instance.reinforcementDivneSmashExtraDamageFromPremiumTreasure += num;
				break;
			case TreasureType.YoYo:
				Singleton<StatManager>.instance.reinforcementWhirlWindExtraHitChanceFromPremiumTreasure += num;
				break;
			case TreasureType.MysteriousJewel:
				Singleton<StatManager>.instance.reinforcementConcentrationExtraDamageFromPremiumTreasure += num;
				break;
			case TreasureType.ConfusionalJewel:
				Singleton<StatManager>.instance.reinforcementCloneWarriorExtraDamageFromPremiumTreasure += num;
				break;
			case TreasureType.DragonGem:
				Singleton<StatManager>.instance.reinforcementDragonBreathExtraDamageFromPremiumTreasure += num;
				break;
			case TreasureType.MeteorPiece:
				Singleton<StatManager>.instance.meteorRainExtraSpawnCountFromPremiumTreasure += num;
				break;
			case TreasureType.IceCrystal:
				Singleton<StatManager>.instance.frostSkillExtraDurationFromPremiumTreasure += num;
				break;
			case TreasureType.SeraphHope:
				Singleton<StatManager>.instance.percentDamageFromPremiumTreasure += num;
				break;
			case TreasureType.SeraphBless:
				Singleton<StatManager>.instance.percentHealthFromPremiumTreasure += num;
				break;
			case TreasureType.FireHand:
				Singleton<StatManager>.instance.meteorRainExtraCastChanceFromPremiumTreasure += num;
				break;
			case TreasureType.IceHand:
				Singleton<StatManager>.instance.frostWallExtraCastChanceFromPremiumTreasure += num;
				break;
			case TreasureType.LegendSkillBook:
				Singleton<StatManager>.instance.reinforcementSkillExtraCastChanceFromPremiumTreasure += num;
				break;
			case TreasureType.ChaosBlade:
				Singleton<StatManager>.instance.reinforcementConfusionExtraFloor += num;
				break;
			case TreasureType.FireDragonHeart:
				Singleton<StatManager>.instance.dragonBreathExtraAttackCount += num;
				break;
			case TreasureType.IceHeart:
				Singleton<StatManager>.instance.frostWallExtraFloor += num;
				break;
			case TreasureType.AntiquityWarriorHelmet:
				Singleton<StatManager>.instance.concentrationExtraDurationTimeFromPremiumTreasure += (float)num;
				break;
			case TreasureType.BlessHood:
				Singleton<StatManager>.instance.chanceForMultipliedTapHealFromPremiumTreasure += num;
				break;
			case TreasureType.WarriorCape:
				Singleton<StatManager>.instance.warriorPercentDamageFromPremiumTreasure += num;
				break;
			case TreasureType.AngelHairPin:
				Singleton<StatManager>.instance.priestPercentDamageFromPremiumTreasure += num;
				break;
			case TreasureType.ArcherArrow:
				Singleton<StatManager>.instance.archerPercentDamageFromPremiumTreasure += num;
				break;
			case TreasureType.ConquerToken:
				Singleton<StatManager>.instance.allHPAndTapHealFromTowerModeTreasure += num;
				break;
			case TreasureType.PatienceToken:
				Singleton<StatManager>.instance.allCharacterPercentDamageFromTowerModeTreasure += num;
				break;
			}
		}
	}

	public void displayTreasurePiece(bool isIncrease)
	{
		UIWindowManageTreasure.instance.refreshSlots();
		if (isIncrease)
		{
			UIWindowManageTreasure.instance.rarePieceCountText.SetValue(Singleton<DataManager>.instance.currentGameData.treasurePiece, 0.6f, 2.9f);
		}
		else
		{
			UIWindowManageTreasure.instance.rarePieceCountText.SetText(Singleton<DataManager>.instance.currentGameData.treasurePiece);
		}
	}

	public TreasureType getRandomLotteryTreasureType()
	{
		TreasureType result = TreasureType.Null;
		List<TreasureType> list = new List<TreasureType>();
		List<TreasureType> list2 = new List<TreasureType>();
		List<TreasureType> list3 = new List<TreasureType>();
		for (int i = 1; i < 53; i++)
		{
			long maxLevel = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[(TreasureType)i].maxLevel;
			if (!containTreasureFromInventory((TreasureType)i))
			{
				switch (getTreasureTier((TreasureType)i))
				{
				case 1:
					list.Add((TreasureType)i);
					break;
				case 2:
					list2.Add((TreasureType)i);
					break;
				case 3:
					list3.Add((TreasureType)i);
					break;
				}
			}
			else if (getTreasureDataFromInventory((TreasureType)i).treasureLevel < maxLevel)
			{
				switch (getTreasureTier((TreasureType)i))
				{
				case 1:
					list.Add((TreasureType)i);
					break;
				case 2:
					list2.Add((TreasureType)i);
					break;
				case 3:
					list3.Add((TreasureType)i);
					break;
				}
			}
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count; j++)
		{
			switch (getTreasureTier(Singleton<DataManager>.instance.currentGameData.treasureInventoryData[j].treasureType))
			{
			case 1:
				num++;
				break;
			case 2:
				num2++;
				break;
			case 3:
				num3++;
				break;
			}
		}
		int num4 = 10;
		int num5 = 30;
		MersenneTwister mersenneTwister = new MersenneTwister(Singleton<DataManager>.instance.currentGameData.treasureRandomSeed + Singleton<DataManager>.instance.currentGameData.treasureLotteryCount);
		int num6 = mersenneTwister.Next(0, 10000);
		num6 /= 100;
		MersenneTwister mersenneTwister2 = new MersenneTwister(Singleton<DataManager>.instance.currentGameData.treasureItemRandomSeed + Singleton<DataManager>.instance.currentGameData.treasureLotteryCount);
		if (num6 < num4)
		{
			if (list.Count > 0)
			{
				result = list[mersenneTwister2.Next(0, list.Count)];
			}
			else if (list2.Count > 0)
			{
				result = list2[mersenneTwister2.Next(0, list2.Count)];
			}
			else if (list3.Count > 0)
			{
				result = list3[mersenneTwister2.Next(0, list3.Count)];
			}
		}
		else if (num6 < num4 + num5)
		{
			if (list2.Count > 0)
			{
				result = list2[mersenneTwister2.Next(0, list2.Count)];
			}
			else if (list3.Count > 0)
			{
				result = list3[mersenneTwister2.Next(0, list3.Count)];
			}
		}
		else if (list3.Count > 0)
		{
			result = list3[mersenneTwister2.Next(0, list3.Count)];
		}
		if (list.Count + list2.Count + list3.Count == 0)
		{
			result = TreasureType.Null;
		}
		return result;
	}

	public void lotteryTreasure(bool isRubyLottery)
	{
		m_targetLotteryTreasure = getRandomLotteryTreasureType();
		if (m_targetLotteryTreasure != 0)
		{
			if (!isRubyLottery)
			{
				if (Singleton<DataManager>.instance.currentGameData.treasurePiece >= currentLotteryPriceForTreasurePiece)
				{
					Singleton<TreasureManager>.instance.decreaseTreasurePiece(currentLotteryPriceForTreasurePiece);
					Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.TreasurePro, 1.0);
					Singleton<DataManager>.instance.currentGameData.treasureLotteryCount++;
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Treasure, AnalyzeManager.ActionType.LotteryTreasure, new Dictionary<string, string>
					{
						{
							"TreasureType",
							m_targetLotteryTreasure.ToString()
						}
					});
					startLottery();
				}
			}
			else if (Singleton<DataManager>.instance.currentGameData._ruby >= currentLotteryPriceForRuby)
			{
				Singleton<RubyManager>.instance.decreaseRuby(currentLotteryPriceForRuby);
				Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.TreasurePro, 1.0);
				Singleton<DataManager>.instance.currentGameData.treasureRubyLotteryEnoughCount--;
				Singleton<DataManager>.instance.currentGameData.treasureLotteryCount++;
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Treasure, AnalyzeManager.ActionType.LotteryTreasureByRuby, new Dictionary<string, string>
				{
					{
						"TreasureType",
						m_targetLotteryTreasure.ToString()
					}
				});
				startLottery();
			}
		}
		else
		{
			UIWindowDialog.openDescription("ARTIFACT_HAVE_ALL", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void OnClickLotteryTreasureByTreasurePiece()
	{
		if (TutorialManager.isTutorial)
		{
			return;
		}
		if (Singleton<DataManager>.instance.currentGameData.treasurePiece >= currentLotteryPriceForTreasurePiece)
		{
			UIWindowLotteryTreasureDialog.instance.openWithDescription("QUESTION_BUY_RARE_TREASURE", false, delegate
			{
				lotteryTreasure(false);
			});
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RARE_TREASURE_PIECE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void OnClickLotteryTreasureByRuby()
	{
		if (TutorialManager.isTutorial)
		{
			return;
		}
		if (Singleton<DataManager>.instance.currentGameData.treasureRubyLotteryEnoughCount > 0)
		{
			if (Singleton<DataManager>.instance.currentGameData._ruby >= currentLotteryPriceForRuby)
			{
				UIWindowLotteryTreasureDialog.instance.openWithDescription("QUESTION_BUY_RARE_TREASURE", true, delegate
				{
					lotteryTreasure(true);
				});
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, string.Empty);
			}
		}
		else
		{
			UIWindowLotteryTreasureDialog.instance.openCloseDescription("TREASURE_CAN_NOT_LOTTERY");
		}
	}

	public void obtainTreasure(TreasureType treasureType, TreasureValueData value)
	{
		if (!containTreasureFromInventory(treasureType))
		{
			TreasureInventoryData treasureInventoryData = new TreasureInventoryData();
			treasureInventoryData.treasureType = treasureType;
			treasureInventoryData.treasureEffectValue = value.treasureEffectValue;
			treasureInventoryData.damagePercentValue = value.damagePercentValue;
			treasureInventoryData.baseTreasurePrice = currentLotteryPriceForTreasurePiece;
			treasureInventoryData.baseTreasureIndex = Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count;
			Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Add(treasureInventoryData);
			UIWindowManageTreasure.instance.createTreasureSlots();
		}
		else
		{
			TreasureInventoryData treasureDataFromInventory = getTreasureDataFromInventory(treasureType);
			long maxLevel = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].maxLevel;
			if (treasureDataFromInventory.treasureLevel < maxLevel)
			{
				treasureDataFromInventory.treasureLevel++;
				Singleton<DataManager>.instance.currentGameData.treasureTotalLevelUpCountRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.treasureTotalLevelUpCountRecord) + 1);
			}
		}
		Singleton<DataManager>.instance.saveData();
		Singleton<StatManager>.instance.refreshAllStats();
		UIWindowManageHeroAndWeapon.instance.refreshAllBuyState();
	}

	private void startLottery()
	{
		UIWindowLotteryTreasure.instance.openWithStartTreasureLottery(delegate
		{
			treasureLotteryEvent(m_targetLotteryTreasure);
		});
	}

	public void treasureLotteryEvent(TreasureType treasureType, bool isRepeatLottery = false)
	{
		double damageValue = Singleton<TreasureManager>.instance.getDamageValue(treasureType);
		double treasureEffectValue = Singleton<TreasureManager>.instance.getTreasureEffectValue(treasureType);
		TreasureInventoryData treasureInventoryData = new TreasureInventoryData();
		treasureInventoryData.treasureType = treasureType;
		treasureInventoryData.treasureEffectValue = treasureEffectValue;
		treasureInventoryData.damagePercentValue = damageValue;
		treasureInventoryData.treasureLevel = 1L;
		TreasureValueData value = default(TreasureValueData);
		value.damagePercentValue = treasureInventoryData.damagePercentValue;
		value.treasureEffectValue = treasureInventoryData.treasureEffectValue;
		bool isDuplicatedTreasure = containTreasureFromInventory(treasureInventoryData.treasureType);
		Singleton<TreasureManager>.instance.obtainTreasure(treasureInventoryData.treasureType, value);
		UIWindowManageTreasure.instance.refreshSlots();
		refreshTreasureInventoryData();
		UIWindowResultTreasureLottery.instance.openWithTreasureInformation(getTreasureDataFromInventory(treasureInventoryData.treasureType), isDuplicatedTreasure, isRepeatLottery);
		UIWindowOutgame.instance.refreshTreasureIndicator();
	}

	public void refreshTreasureInventoryData()
	{
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count; i++)
		{
			Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureEffectValue = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureType].treasureEffectValue;
			Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].extraTreasureEffectValue = 0.0;
			int num = (int)Mathf.Min(Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureLevel, Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureType].maxLevel);
			Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureLevel = num;
			Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].extraTreasureEffectValue += Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureType].increasingValueEveryEnchant * (double)(num - 1);
			Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].damagePercentValue = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureType].damagePercentValue;
		}
	}

	public int getTreasureTier(TreasureType treasureType)
	{
		int num = 0;
		return Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].treasureTier;
	}

	public bool isElopeTreasure(TreasureType treasureType)
	{
		bool result = false;
		if (getTreasureTier(treasureType) == 0)
		{
			result = ((!isTowerModeRankingTreasure(treasureType)) ? true : false);
		}
		return result;
	}

	public bool isTowerModeRankingTreasure(TreasureType treasureType)
	{
		bool result = false;
		if (treasureType == TreasureType.ConquerToken || treasureType == TreasureType.PatienceToken)
		{
			result = true;
		}
		return result;
	}

	public double getCurrentTreasureLevelUpEnchantStonePrice(TreasureType treasureType)
	{
		if (containTreasureFromInventory(treasureType))
		{
			TreasureInventoryData treasureDataFromInventory = getTreasureDataFromInventory(treasureType);
			TreasureStatData treasureStatData = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType];
			return treasureStatData.baseEnchantStonePrice + treasureStatData.increasingEnchantStonePriceEveryEnchant * (double)treasureDataFromInventory.treasureLevel;
		}
		return 2147483647.0;
	}

	public long getCurrentTreasureLevelUpRubyPrice(TreasureType treasureType)
	{
		if (containTreasureFromInventory(treasureType))
		{
			TreasureInventoryData treasureDataFromInventory = getTreasureDataFromInventory(treasureType);
			TreasureStatData treasureStatData = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType];
			return (long)(treasureStatData.baseEnchantRubyPrice + treasureStatData.increasingRubyPriceEveryEnchant * (double)treasureDataFromInventory.treasureLevel);
		}
		return 2147483647L;
	}

	public Sprite getTreasureSprite(TreasureType treasureType)
	{
		return treasureIconSprites[(int)(treasureType - 1)];
	}

	public bool containTreasureFromInventory(TreasureType treasureType)
	{
		bool result = false;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count; i++)
		{
			if (Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureType == treasureType)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public TreasureInventoryData getTreasureDataFromInventory(TreasureType treasureType)
	{
		if (!containTreasureFromInventory(treasureType))
		{
			DebugManager.LogError("This treasure is empty");
			return null;
		}
		TreasureInventoryData result = null;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count; i++)
		{
			if (Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureType == treasureType)
			{
				result = Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i];
				break;
			}
		}
		return result;
	}

	public string getTreasureI18NName(TreasureType treasureType)
	{
		string empty = string.Empty;
		return I18NManager.Get("ARTIFACT_NAME_" + (int)treasureType);
	}

	public string getTreasureDescription(TreasureType treasureType, double value, double extraValue = 0.0, bool isColor = true)
	{
		string empty = string.Empty;
		if (treasureType == TreasureType.OcarinaOfForestSpirit || treasureType == TreasureType.MonocleOfWiseGoddess || treasureType == TreasureType.NecklaceOfSorcerers || treasureType == TreasureType.MeteorPiece || treasureType == TreasureType.IceCrystal || treasureType == TreasureType.ChaosBlade || treasureType == TreasureType.FireDragonHeart || treasureType == TreasureType.IceHeart || treasureType == TreasureType.AntiquityWarriorHelmet)
		{
			return string.Format(I18NManager.Get("ARTIFACT_DESCRIPTION_" + (int)treasureType), ((!isColor) ? string.Empty : "<color=white>") + value + ((!isColor) ? string.Empty : "</color>") + ((!(extraValue > 0.0)) ? string.Empty : (" <color=#FAD725>(+" + extraValue + ")</color>")));
		}
		return string.Format(I18NManager.Get("ARTIFACT_DESCRIPTION_" + (int)treasureType), ((!isColor) ? string.Empty : "<color=white>") + value + "%" + ((!isColor) ? string.Empty : "</color>") + ((!(extraValue > 0.0)) ? string.Empty : (" <color=#FAD725>(+" + extraValue + "%)</color>")));
	}

	public TreasureValueData getTreasureBaseData(TreasureType treasureType)
	{
		if (!ParsingManager.isLoaded)
		{
			return default(TreasureValueData);
		}
		TreasureValueData result = default(TreasureValueData);
		TreasureStatData treasureStatData = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType];
		result.damagePercentValue = treasureStatData.damagePercentValue;
		result.treasureEffectValue = treasureStatData.treasureEffectValue;
		return result;
	}

	public double getDamageValue(TreasureType treasureType)
	{
		int num = 0;
		num = (int)getTreasureBaseData(treasureType).damagePercentValue;
		return num;
	}

	public double getTreasureEffectValue(TreasureType treasureType)
	{
		double num = 0.0;
		return getTreasureBaseData(treasureType).treasureEffectValue;
	}

	public long getDestroyRubyPrice(TreasureInventoryData treasureInventoryData)
	{
		long num = 0L;
		return 20L;
	}

	public bool isPVPTreasure(TreasureType treasureType)
	{
		bool result = false;
		switch (treasureType)
		{
		case TreasureType.HeavenShield:
		case TreasureType.HeliosHarp:
		case TreasureType.CharmOfLunarGoddess:
		case TreasureType.ConquerorRing:
		case TreasureType.NobleBlade:
		case TreasureType.SeraphHope:
		case TreasureType.SeraphBless:
		case TreasureType.WarriorCape:
		case TreasureType.AngelHairPin:
		case TreasureType.ArcherArrow:
		case TreasureType.ConquerToken:
		case TreasureType.PatienceToken:
			result = true;
			break;
		}
		return result;
	}

	public void startGame()
	{
		ingameTotalTreasurePiece = 0L;
	}

	public void spawnTreasurePiece(Vector2 spawnPosition)
	{
		Singleton<DropItemManager>.instance.spawnDropItem(DropItemManager.DropItemType.TreasureKey, spawnPosition, 1.0);
	}

	public void increaseTreasurePiece(long value, bool refresh = true)
	{
		if (GameManager.currentGameState == GameManager.GameState.Playing)
		{
			ingameTotalTreasurePiece += value;
		}
		Singleton<DataManager>.instance.currentGameData.treasurePiece += value;
		Debug.LogError("赋值?");
		Singleton<DataManager>.instance.currentGameData.treasurePieceRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.treasurePieceRecord) + value);
		if (GameManager.currentGameState == GameManager.GameState.OutGame)
		{
			UIWindowOutgame.instance.refreshTreasureIndicator();
		}
		UIWindowOutgame.instance.refreshTreasureIndicator();
		if (refresh)
		{
			displayTreasurePiece(true);
		}
	}

	public void decreaseTreasurePiece(long value)
	{
		Singleton<DataManager>.instance.currentGameData.treasurePiece -= value;
		Singleton<DataManager>.instance.currentGameData.treasurePieceRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.treasurePieceRecord) - value);
		if (GameManager.currentGameState == GameManager.GameState.OutGame)
		{
			UIWindowOutgame.instance.refreshTreasureIndicator();
		}
		displayTreasurePiece(false);
	}
}
