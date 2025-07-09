using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class RebirthManager : Singleton<RebirthManager>
{
	public Transform doubleRebirthButtonTransform;

	public long currentRebirthKeyRewardValue
	{
		get
		{
			return Math.Max((Singleton<DataManager>.instance.currentGameData.currentTheme - (currentRebirthRequireTheme + 1)) * 5, 0L) + (50 + 5 * Math.Max(currentRebirthRequireTheme - 10, 0L));
		}
	}

	public long currentRebirthRequireTheme
	{
		get
		{
			return Math.Min(Singleton<DataManager>.instance.currentGameData.rebirthCount + 1, GameManager.maxTheme - 1);
		}
	}

	public void OnClickStartRebirth(bool withDoubleKey)
	{
		if (!withDoubleKey)
		{
			UIWindowDialog.openDescription("REBIRTH_QUESTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				if (Social.localUser.authenticated)
				{
					Social.ReportScore(Singleton<DataManager>.instance.currentGameData.rebirthCount + 1, "CgkIgP-i6oAVEAIQNw", delegate
					{
					});
				}
				initializeGameData(false);
			}, string.Empty);
			return;
		}
		UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("DOUBLE_KEY_REBIRTH_QUESTION"), currentRebirthKeyRewardValue * 2), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			if (Singleton<DataManager>.instance.currentGameData._ruby >= 200)
			{
				Singleton<RubyManager>.instance.decreaseRuby(200.0);
				if (Social.localUser.authenticated)
				{
					Social.ReportScore(Singleton<DataManager>.instance.currentGameData.rebirthCount + 1, "CgkIgP-i6oAVEAIQNw", delegate
					{
					});
				}
				initializeGameData(true);
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, string.Empty);
			}
		}, string.Empty);
	}

	private void initializeGameData(bool isDoubleKeyRebirth)
	{
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.Reborn, 1.0);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Theme", Singleton<DataManager>.instance.currentGameData.unlockTheme.ToString());
		dictionary.Add("Gold", Singleton<DataManager>.instance.currentGameData.gold.ToString());
		dictionary.Add("Ruby", Singleton<DataManager>.instance.currentGameData._ruby.ToString());
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count; i++)
		{
			dictionary.Add("InventoryTreasureType" + (i + 1), Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].treasureType.ToString());
		}
		AnalyzeManager.retention(AnalyzeManager.CategoryType.Rebirth, AnalyzeManager.ActionType.Rebirth, dictionary);
		int countableSilverFingerLeftCount = Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount;
		int countableGoldFingerLeftCount = Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount;
		int treasureItemRandomSeed = Singleton<DataManager>.instance.currentGameData.treasureItemRandomSeed;
		int treasureRandomSeed = Singleton<DataManager>.instance.currentGameData.treasureRandomSeed;
		int treasureLotteryCount = Singleton<DataManager>.instance.currentGameData.treasureLotteryCount;
		long num = currentRebirthKeyRewardValue * ((!isDoubleKeyRebirth) ? 1 : 2);
		Debug.LogError("赋值?");
		long treasurePiece = Singleton<DataManager>.instance.currentGameData.treasurePiece + num;
		string treasurePieceRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.treasurePieceRecord) + num);
		long ruby = Singleton<DataManager>.instance.currentGameData._ruby;
		ObscuredLong obscuredRuby = Singleton<DataManager>.instance.currentGameData.obscuredRuby;
		long adsAngelSpawnTime = Singleton<DataManager>.instance.currentGameData.adsAngelSpawnTime;
		long rebirthCount = Singleton<DataManager>.instance.currentGameData.rebirthCount + 1;
		bool isClearTutorial = Singleton<DataManager>.instance.currentGameData.isClearTutorial;
		bool isPushNotificationOn = Singleton<DataManager>.instance.currentGameData.isPushNotificationOn;
		bool isBoughtAutoTouch = Singleton<DataManager>.instance.currentGameData.isBoughtAutoTouch;
		bool isClearRebirthTutorial = Singleton<DataManager>.instance.currentGameData.isClearRebirthTutorial;
		long autoTouch13TapEndTime = Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime;
		int bestTheme = Singleton<DataManager>.instance.currentGameData.bestTheme;
		long treasureRubyLotteryEnoughCount = Singleton<DataManager>.instance.currentGameData.treasureRubyLotteryEnoughCount + 1;
		long autoTouch13TapEndTime2 = Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime;
		long autoTouch26TapEndTime = Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime;
		bool isObtainedFacebookReward = Singleton<DataManager>.instance.currentGameData.isObtainedFacebookReward;
		bool isObtainedTwitterReward = Singleton<DataManager>.instance.currentGameData.isObtainedTwitterReward;
		bool isSeenCharacterSkinDescription = Singleton<DataManager>.instance.currentGameData.isSeenCharacterSkinDescription;
		bool isSeenColleagueSkinDescription = Singleton<DataManager>.instance.currentGameData.isSeenColleagueSkinDescription;
		long adsRubyWatchEndTime = Singleton<DataManager>.instance.currentGameData.adsRubyWatchEndTime;
		long adsGoldWatchEndTime = Singleton<DataManager>.instance.currentGameData.adsGoldWatchEndTime;
		long treasureDailyRewardTime = Singleton<DataManager>.instance.currentGameData.treasureDailyRewardTime;
		int lastDailyRewardReceivedDayNumber = Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedDayNumber;
		long lastDailyRewardReceivedTime = Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedTime;
		long adWatchCount = Singleton<DataManager>.instance.currentGameData.adWatchCount;
		long clearLastThemeCount = Singleton<DataManager>.instance.currentGameData.clearLastThemeCount;
		long transcendStone = Singleton<DataManager>.instance.currentGameData.transcendStone;
		long doubleSpeedEndTime = Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime;
		int lastRecievedNewEventPackageMonth = Singleton<DataManager>.instance.currentGameData.lastRecievedNewEventPackageMonth;
		int countAutoOpenTreasureChestRemainCount = Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount;
		bool isLowEndDevice = Singleton<DataManager>.instance.currentGameData.isLowEndDevice;
		bool isSeenPrincessCollectionForPrevVersion = Singleton<DataManager>.instance.currentGameData.isSeenPrincessCollectionForPrevVersion;
		double lastSavedOffsetBetweenInternetAndUnbiased = Singleton<DataManager>.instance.currentGameData.lastSavedOffsetBetweenInternetAndUnbiased;
		long timerAutoOpenTreasureChestRemainTime = Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime;
		ObscuredLong collectEventResource = Singleton<DataManager>.instance.currentGameData.collectEventResource;
		ObscuredInt collectEventTargetNextRewardTier = Singleton<DataManager>.instance.currentGameData.collectEventTargetNextRewardTier;
		long specialAdsAngelSpawnTime = Singleton<DataManager>.instance.currentGameData.specialAdsAngelSpawnTime;
		long currentProgressDistanceForElopeMode = Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode;
		double currentAttackingEnemyHeathForElopeMode = Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode;
		double heartForElopeMode = Singleton<DataManager>.instance.currentGameData.heartForElopeMode;
		long heartCoinForElopeMode = Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode;
		long loveTimeMachineRemainCountForElopeMode = Singleton<DataManager>.instance.currentGameData.loveTimeMachineRemainCountForElopeMode;
		bool isSeenElopeModeTutorial = Singleton<DataManager>.instance.currentGameData.isSeenElopeModeTutorial;
		long lastRefreshElopeShopHour = Singleton<DataManager>.instance.currentGameData.lastRefreshElopeShopHour;
		long speedGuyAdsEndTime = Singleton<DataManager>.instance.currentGameData.speedGuyAdsEndTime;
		int goldRouletteTicket = Singleton<DataManager>.instance.currentGameData.goldRouletteTicket;
		int goldRouletteLastDayOfYear = Singleton<DataManager>.instance.currentGameData.goldRouletteLastDayOfYear;
		int goldRouletteRemainCount = Singleton<DataManager>.instance.currentGameData.goldRouletteRemainCount;
		long bronzeRouletteTargetEndTime = Singleton<DataManager>.instance.currentGameData.bronzeRouletteTargetEndTime;
		long lastTowerModeStartTime = Singleton<DataManager>.instance.currentGameData.lastTowerModeStartTime;
		int towerModeTicketCount = Singleton<DataManager>.instance.currentGameData.towerModeTicketCount;
		int towerModeTimeAttackBestFloor = Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackBestFloor;
		float towerModeTimeAttackBestClearTime = Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackBestClearTime;
		int towerModeEndlessModeBestFloor = Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeBestFloor;
		float towerModeEndlessModeBestClearTime = Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeBestClearTime;
		int towerModeTimeAttackSeasonBestFloor = Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor;
		float towerModeTimeAttackSeasonBestClearTime = Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestClearTime;
		int towerModeEndlessModeSeasonBestFloor = Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor;
		float towerModeEndlessModeSeasonBestClearTime = Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestClearTime;
		long towerModeFreeTicketEndTime = Singleton<DataManager>.instance.currentGameData.towerModeFreeTicketEndTime;
		int towerModeLastSeason = Singleton<DataManager>.instance.currentGameData.towerModeLastSeason;
		ObscuredLong weaponSkinPiece = Singleton<DataManager>.instance.currentGameData.weaponSkinPiece;
		ObscuredLong weaponSkinReinforcementMasterPiece = Singleton<DataManager>.instance.currentGameData.weaponSkinReinforcementMasterPiece;
		ObscuredLong totalPurchasedMoney = Singleton<DataManager>.instance.currentGameData.totalPurchasedMoney;
		long advertisementShopAppearEndTime = Singleton<DataManager>.instance.currentGameData.advertisementShopAppearEndTime;
		int lastRecievedWeaponLegendarySkinPackageMonth = Singleton<DataManager>.instance.currentGameData.lastRecievedWeaponLegendarySkinPackageMonth;
		ObscuredBool isUsedFreeWeaponSkinLottery = Singleton<DataManager>.instance.currentGameData.isUsedFreeWeaponSkinLottery;
		double lastSavedOffsetBetweenInternetAndUnbiasedForUTC = Singleton<DataManager>.instance.currentGameData.lastSavedOffsetBetweenInternetAndUnbiasedForUTC;
		ObscuredLong lastPVPStartTime = Singleton<DataManager>.instance.currentGameData.lastPVPStartTime;
		ObscuredLong pvpTicketCount = Singleton<DataManager>.instance.currentGameData.pvpTicketCount;
		ObscuredLong pvpHonorToken = Singleton<DataManager>.instance.currentGameData.pvpHonorToken;
		string pvpHonorTokenRecord = Singleton<DataManager>.instance.currentGameData.pvpHonorTokenRecord;
		ObscuredLong pvpShopTargetRefreshTime = Singleton<DataManager>.instance.currentGameData.pvpShopTargetRefreshTime;
		ObscuredLong pvpShopShopItemRefreshCount = Singleton<DataManager>.instance.currentGameData.pvpShopShopItemRefreshCount;
		long admobAdsIngnoreTargetTime = Singleton<DataManager>.instance.currentGameData.admobAdsIngnoreTargetTime;
		string characterSkinTotalLevelUpRecord = Singleton<DataManager>.instance.currentGameData.characterSkinTotalLevelUpRecord;
		string colleagueSkinTotalLevelUpRecord = Singleton<DataManager>.instance.currentGameData.colleagueSkinTotalLevelUpRecord;
		string rubyRecord = Singleton<DataManager>.instance.currentGameData.rubyRecord;
		long value = Singleton<DataManager>.instance.currentGameData.countDoubleSpeed;
		ObscuredInt currentCollectEventIndex = Singleton<DataManager>.instance.currentGameData.currentCollectEventIndex;
		string heartCoinForElopeModeRecord = Singleton<DataManager>.instance.currentGameData.heartCoinForElopeModeRecord;
		string treasureTotalLevelUpCountRecord = Singleton<DataManager>.instance.currentGameData.treasureTotalLevelUpCountRecord;
		bool isBoughtBronzeFinger = Singleton<DataManager>.instance.currentGameData.isBoughtBronzeFinger;
		List<WarriorWeaponInventoryData> list = new List<WarriorWeaponInventoryData>();
		List<PriestWeaponInventoryData> list2 = new List<PriestWeaponInventoryData>();
		List<ArcherWeaponInventoryData> list3 = new List<ArcherWeaponInventoryData>();
		Dictionary<ElopeModeManager.DaemonKingSkillType, long> dictionary2 = new Dictionary<ElopeModeManager.DaemonKingSkillType, long>();
		List<ColleagueInventoryData> list4 = new List<ColleagueInventoryData>();
		CharacterSkinManager.WarriorSkinType equippedWarriorSkin = Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin;
		CharacterSkinManager.PriestSkinType equippedPriestSkin = Singleton<DataManager>.instance.currentGameData.equippedPriestSkin;
		CharacterSkinManager.ArcherSkinType equippedArcherSkin = Singleton<DataManager>.instance.currentGameData.equippedArcherSkin;
		List<TreasureInventoryData> list5 = new List<TreasureInventoryData>();
		List<AchievementData> list6 = new List<AchievementData>();
		List<WarriorCharacterSkinData> list7 = new List<WarriorCharacterSkinData>();
		List<PriestCharacterSkinData> list8 = new List<PriestCharacterSkinData>();
		List<ArcherCharacterSkinData> list9 = new List<ArcherCharacterSkinData>();
		List<ShopManager.LimitedItemData> list10 = new List<ShopManager.LimitedItemData>();
		List<ShopManager.LimitedItemData> list11 = new List<ShopManager.LimitedItemData>();
		List<ShopManager.LimitedItemData> list12 = new List<ShopManager.LimitedItemData>();
		List<ShopManager.LimitedItemData> list13 = new List<ShopManager.LimitedItemData>();
		List<SkillInventoryData> list14 = new List<SkillInventoryData>();
		Dictionary<TranscendManager.TranscendPassiveSkillType, TranscendPassiveSkillInventoryData> dictionary3 = new Dictionary<TranscendManager.TranscendPassiveSkillType, TranscendPassiveSkillInventoryData>();
		Dictionary<SkillManager.SkillType, ReinforcementSkillInventoryData> dictionary4 = new Dictionary<SkillManager.SkillType, ReinforcementSkillInventoryData>();
		Dictionary<CharacterManager.CharacterType, long> dictionary5 = new Dictionary<CharacterManager.CharacterType, long>();
		List<PassiveSkillInventoryData> list15 = new List<PassiveSkillInventoryData>();
		List<PrincessInventoryDataForElopeMode> list16 = new List<PrincessInventoryDataForElopeMode>();
		Dictionary<DropItemManager.DropItemType, long> dictionary6 = new Dictionary<DropItemManager.DropItemType, long>();
		Dictionary<ElopeModeManager.DaemonKingSkillType, double> dictionary7 = new Dictionary<ElopeModeManager.DaemonKingSkillType, double>();
		Dictionary<CharacterManager.CharacterType, List<WeaponSkinData>> dictionary8 = new Dictionary<CharacterManager.CharacterType, List<WeaponSkinData>>();
		List<PVPManager.PVPShopItemData> list17 = new List<PVPManager.PVPShopItemData>();
		Dictionary<ObscuredInt, PVPTankData> dictionary9 = new Dictionary<ObscuredInt, PVPTankData>();
		BossRaidManager.BossRaidBestRecordData bossRaidBestRecord = Singleton<DataManager>.instance.currentGameData.bossRaidBestRecord;
		int heartForBossRaid = Singleton<DataManager>.instance.currentGameData.heartForBossRaid;
		long lastHeartUsedTime = Singleton<DataManager>.instance.currentGameData.lastHeartUsedTime;
		double treasureEnchantStone = Singleton<DataManager>.instance.currentGameData.treasureEnchantStone;
		long elopeShopTargetRefreshTime = Singleton<DataManager>.instance.currentGameData.elopeShopTargetRefreshTime;
		List<ElopeShopItemData> list18 = new List<ElopeShopItemData>();
		List<string> list19 = new List<string>();
		foreach (KeyValuePair<ObscuredInt, PVPTankData> pvpTankDatum in Singleton<DataManager>.instance.currentGameData.pvpTankData)
		{
			dictionary9.Add(pvpTankDatum.Key, pvpTankDatum.Value);
		}
		if (Singleton<DataManager>.instance.currentGameData.pvpShopItemList != null)
		{
			for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.pvpShopItemList.Count; j++)
			{
				list17.Add(Singleton<DataManager>.instance.currentGameData.pvpShopItemList[j]);
			}
		}
		foreach (KeyValuePair<CharacterManager.CharacterType, List<WeaponSkinData>> weaponSkinDatum in Singleton<DataManager>.instance.currentGameData.weaponSkinData)
		{
			dictionary8.Add(weaponSkinDatum.Key, weaponSkinDatum.Value);
		}
		for (int k = 0; k < Singleton<DataManager>.instance.currentGameData.warriorWeaponInventoryData.Count; k++)
		{
			WarriorWeaponInventoryData warriorWeaponInventoryData = Singleton<DataManager>.instance.currentGameData.warriorWeaponInventoryData[k];
			if (k != 0)
			{
				warriorWeaponInventoryData.isHaving = false;
			}
			else
			{
				warriorWeaponInventoryData.isHaving = true;
			}
			warriorWeaponInventoryData.enchantCount = 0L;
			list.Add(warriorWeaponInventoryData);
		}
		for (int l = 0; l < Singleton<DataManager>.instance.currentGameData.priestWeaponInventoryData.Count; l++)
		{
			PriestWeaponInventoryData priestWeaponInventoryData = Singleton<DataManager>.instance.currentGameData.priestWeaponInventoryData[l];
			if (l != 0)
			{
				priestWeaponInventoryData.isHaving = false;
			}
			else
			{
				priestWeaponInventoryData.isHaving = true;
			}
			priestWeaponInventoryData.enchantCount = 0L;
			list2.Add(priestWeaponInventoryData);
		}
		for (int m = 0; m < Singleton<DataManager>.instance.currentGameData.archerWeaponInventoryData.Count; m++)
		{
			ArcherWeaponInventoryData archerWeaponInventoryData = Singleton<DataManager>.instance.currentGameData.archerWeaponInventoryData[m];
			if (m != 0)
			{
				archerWeaponInventoryData.isHaving = false;
			}
			else
			{
				archerWeaponInventoryData.isHaving = true;
			}
			archerWeaponInventoryData.enchantCount = 0L;
			list3.Add(archerWeaponInventoryData);
		}
		for (int n = 0; n < Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList.Count; n++)
		{
			list18.Add(Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[n]);
		}
		foreach (KeyValuePair<ElopeModeManager.DaemonKingSkillType, double> item in Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode)
		{
			dictionary7.Add(item.Key, item.Value);
		}
		foreach (KeyValuePair<DropItemManager.DropItemType, long> elopeModeResourceDatum in Singleton<DataManager>.instance.currentGameData.elopeModeResourceData)
		{
			dictionary6.Add(elopeModeResourceDatum.Key, elopeModeResourceDatum.Value);
		}
		foreach (KeyValuePair<ElopeModeManager.DaemonKingSkillType, long> item2 in Singleton<DataManager>.instance.currentGameData.currentDaemonKingLevelForElopeMode)
		{
			dictionary2.Add(item2.Key, item2.Value);
		}
		for (int num2 = 0; num2 < Singleton<DataManager>.instance.currentGameData.princessInventoryDataForElopeMode.Count; num2++)
		{
			list16.Add(Singleton<DataManager>.instance.currentGameData.princessInventoryDataForElopeMode[num2]);
		}
		for (int num3 = 0; num3 < Singleton<DataManager>.instance.currentGameData.passiveSkillInventoryData.Count; num3++)
		{
			list15.Add(Singleton<DataManager>.instance.currentGameData.passiveSkillInventoryData[num3]);
		}
		foreach (KeyValuePair<TranscendManager.TranscendPassiveSkillType, TranscendPassiveSkillInventoryData> transcendPassiveSkillInventoryDatum in Singleton<DataManager>.instance.currentGameData.transcendPassiveSkillInventoryData)
		{
			dictionary3.Add(transcendPassiveSkillInventoryDatum.Key, transcendPassiveSkillInventoryDatum.Value);
		}
		foreach (KeyValuePair<SkillManager.SkillType, ReinforcementSkillInventoryData> reinforcementSkillInventoryDatum in Singleton<DataManager>.instance.currentGameData.reinforcementSkillInventoryData)
		{
			dictionary4.Add(reinforcementSkillInventoryDatum.Key, reinforcementSkillInventoryDatum.Value);
		}
		foreach (KeyValuePair<CharacterManager.CharacterType, long> item3 in Singleton<DataManager>.instance.currentGameData.currentTranscendTier)
		{
			dictionary5.Add(item3.Key, item3.Value);
		}
		for (int num4 = 0; num4 < Singleton<DataManager>.instance.currentGameData.skillInventoryData.Count; num4++)
		{
			SkillInventoryData skillInventoryData = Singleton<DataManager>.instance.currentGameData.skillInventoryData[num4];
			skillInventoryData.skillLevel = Singleton<DataManager>.instance.currentGameData.skillInventoryData[num4].skillLevel;
			list14.Add(skillInventoryData);
		}
		if (Singleton<DataManager>.instance.currentGameData.currentTimeCheckingLimitedItemList != null)
		{
			for (int num5 = 0; num5 < Singleton<DataManager>.instance.currentGameData.currentTimeCheckingLimitedItemList.Count; num5++)
			{
				list10.Add(Singleton<DataManager>.instance.currentGameData.currentTimeCheckingLimitedItemList[num5]);
			}
		}
		if (Singleton<DataManager>.instance.currentGameData.usedLimitedItemList != null)
		{
			for (int num6 = 0; num6 < Singleton<DataManager>.instance.currentGameData.usedLimitedItemList.Count; num6++)
			{
				list11.Add(Singleton<DataManager>.instance.currentGameData.usedLimitedItemList[num6]);
			}
		}
		for (int num7 = 0; num7 < Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Count; num7++)
		{
			list12.Add(Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[num7]);
		}
		for (int num8 = 0; num8 < Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList.Count; num8++)
		{
			list13.Add(Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList[num8]);
		}
		for (int num9 = 0; num9 < Singleton<DataManager>.instance.currentGameData.colleagueInventoryList.Count; num9++)
		{
			ColleagueInventoryData colleagueInventoryData = Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[num9];
			colleagueInventoryData.isUnlocked = Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[num9].isUnlocked;
			colleagueInventoryData.level = Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[num9].level;
			colleagueInventoryData.lastPassiveUnlockLevel = Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[num9].lastPassiveUnlockLevel;
			list4.Add(colleagueInventoryData);
		}
		for (int num10 = 0; num10 < Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Count; num10++)
		{
			list19.Add(Singleton<DataManager>.instance.currentGameData.usedEventCouponList[num10]);
		}
		for (int num11 = 0; num11 < Singleton<DataManager>.instance.currentGameData.warriorSkinData.Count; num11++)
		{
			list7.Add(Singleton<DataManager>.instance.currentGameData.warriorSkinData[num11]);
			if (!list7[num11].isHaving)
			{
				list7[num11].isNotice = false;
			}
		}
		for (int num12 = 0; num12 < Singleton<DataManager>.instance.currentGameData.priestSkinData.Count; num12++)
		{
			list8.Add(Singleton<DataManager>.instance.currentGameData.priestSkinData[num12]);
			if (!list8[num12].isHaving)
			{
				list8[num12].isNotice = false;
			}
		}
		for (int num13 = 0; num13 < Singleton<DataManager>.instance.currentGameData.archerSkinData.Count; num13++)
		{
			list9.Add(Singleton<DataManager>.instance.currentGameData.archerSkinData[num13]);
			if (!list9[num13].isHaving)
			{
				list9[num13].isNotice = false;
			}
		}
		for (int num14 = 0; num14 < Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count; num14++)
		{
			list5.Add(Singleton<DataManager>.instance.currentGameData.treasureInventoryData[num14]);
		}
		for (int num15 = 0; num15 < Singleton<DataManager>.instance.currentGameData.achievementData.Count; num15++)
		{
			list6.Add(Singleton<DataManager>.instance.currentGameData.achievementData[num15]);
		}
		Singleton<DataManager>.instance.currentGameData = new GameData();
		Singleton<DataManager>.instance.initGameData();
		Singleton<DataManager>.instance.currentGameData.isBoughtBronzeFinger = isBoughtBronzeFinger;
		Singleton<DataManager>.instance.currentGameData.treasureTotalLevelUpCountRecord = treasureTotalLevelUpCountRecord;
		Singleton<DataManager>.instance.currentGameData.heartCoinForElopeModeRecord = heartCoinForElopeModeRecord;
		Singleton<DataManager>.instance.currentGameData.treasurePieceRecord = treasurePieceRecord;
		Singleton<DataManager>.instance.currentGameData.currentCollectEventIndex = currentCollectEventIndex;
		Singleton<DataManager>.instance.currentGameData.countDoubleSpeed = value;
		Singleton<DataManager>.instance.currentGameData.rubyRecord = rubyRecord;
		Singleton<DataManager>.instance.currentGameData.characterSkinTotalLevelUpRecord = characterSkinTotalLevelUpRecord;
		Singleton<DataManager>.instance.currentGameData.colleagueSkinTotalLevelUpRecord = colleagueSkinTotalLevelUpRecord;
		Singleton<DataManager>.instance.currentGameData.pvpHonorTokenRecord = pvpHonorTokenRecord;
		Singleton<DataManager>.instance.currentGameData.admobAdsIngnoreTargetTime = admobAdsIngnoreTargetTime;
		Singleton<DataManager>.instance.currentGameData.pvpTankData = dictionary9;
		Singleton<DataManager>.instance.currentGameData.pvpShopShopItemRefreshCount = pvpShopShopItemRefreshCount;
		Singleton<DataManager>.instance.currentGameData.pvpShopTargetRefreshTime = pvpShopTargetRefreshTime;
		Singleton<DataManager>.instance.currentGameData.pvpShopItemList = list17;
		Singleton<DataManager>.instance.currentGameData.pvpHonorToken = pvpHonorToken;
		Singleton<DataManager>.instance.currentGameData.lastPVPStartTime = lastPVPStartTime;
		Singleton<DataManager>.instance.currentGameData.pvpTicketCount = pvpTicketCount;
		Singleton<DataManager>.instance.currentGameData.lastSavedOffsetBetweenInternetAndUnbiasedForUTC = lastSavedOffsetBetweenInternetAndUnbiasedForUTC;
		Singleton<DataManager>.instance.currentGameData.isUsedFreeWeaponSkinLottery = isUsedFreeWeaponSkinLottery;
		Singleton<DataManager>.instance.currentGameData.lastRecievedWeaponLegendarySkinPackageMonth = lastRecievedWeaponLegendarySkinPackageMonth;
		Singleton<DataManager>.instance.currentGameData.advertisementShopAppearEndTime = advertisementShopAppearEndTime;
		Singleton<DataManager>.instance.currentGameData.totalPurchasedMoney = totalPurchasedMoney;
		Singleton<DataManager>.instance.currentGameData.weaponSkinReinforcementMasterPiece = weaponSkinReinforcementMasterPiece;
		Singleton<DataManager>.instance.currentGameData.weaponSkinPiece = weaponSkinPiece;
		Singleton<DataManager>.instance.currentGameData.weaponSkinData = dictionary8;
		Singleton<DataManager>.instance.currentGameData.towerModeLastSeason = towerModeLastSeason;
		Singleton<DataManager>.instance.currentGameData.towerModeFreeTicketEndTime = towerModeFreeTicketEndTime;
		Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor = towerModeTimeAttackSeasonBestFloor;
		Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestClearTime = towerModeTimeAttackSeasonBestClearTime;
		Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor = towerModeEndlessModeSeasonBestFloor;
		Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestClearTime = towerModeEndlessModeSeasonBestClearTime;
		Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackBestFloor = towerModeTimeAttackBestFloor;
		Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackBestClearTime = towerModeTimeAttackBestClearTime;
		Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeBestFloor = towerModeEndlessModeBestFloor;
		Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeBestClearTime = towerModeEndlessModeBestClearTime;
		Singleton<DataManager>.instance.currentGameData.towerModeTicketCount = towerModeTicketCount;
		Singleton<DataManager>.instance.currentGameData.lastTowerModeStartTime = lastTowerModeStartTime;
		Singleton<DataManager>.instance.currentGameData.goldRouletteTicket = goldRouletteTicket;
		Singleton<DataManager>.instance.currentGameData.goldRouletteLastDayOfYear = goldRouletteLastDayOfYear;
		Singleton<DataManager>.instance.currentGameData.goldRouletteRemainCount = goldRouletteRemainCount;
		Singleton<DataManager>.instance.currentGameData.bronzeRouletteTargetEndTime = bronzeRouletteTargetEndTime;
		Singleton<DataManager>.instance.currentGameData.speedGuyAdsEndTime = speedGuyAdsEndTime;
		Singleton<DataManager>.instance.currentGameData.lastRefreshElopeShopHour = lastRefreshElopeShopHour;
		Singleton<DataManager>.instance.currentGameData.warriorWeaponInventoryData = list;
		Singleton<DataManager>.instance.currentGameData.priestWeaponInventoryData = list2;
		Singleton<DataManager>.instance.currentGameData.archerWeaponInventoryData = list3;
		Singleton<DataManager>.instance.currentGameData.isSeenElopeModeTutorial = isSeenElopeModeTutorial;
		Singleton<DataManager>.instance.currentGameData.loveTimeMachineRemainCountForElopeMode = loveTimeMachineRemainCountForElopeMode;
		Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode = heartCoinForElopeMode;
		Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList = list18;
		Singleton<DataManager>.instance.currentGameData.elopeShopTargetRefreshTime = elopeShopTargetRefreshTime;
		Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode = dictionary7;
		Singleton<DataManager>.instance.currentGameData.elopeModeResourceData = dictionary6;
		Singleton<DataManager>.instance.currentGameData.heartForElopeMode = heartForElopeMode;
		Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode = currentAttackingEnemyHeathForElopeMode;
		Singleton<DataManager>.instance.currentGameData.currentDaemonKingLevelForElopeMode = dictionary2;
		Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode = currentProgressDistanceForElopeMode;
		Singleton<DataManager>.instance.currentGameData.princessInventoryDataForElopeMode = list16;
		Singleton<DataManager>.instance.currentGameData.specialAdsAngelSpawnTime = specialAdsAngelSpawnTime;
		Singleton<DataManager>.instance.currentGameData.passiveSkillInventoryData = list15;
		Singleton<DataManager>.instance.currentGameData.collectEventTargetNextRewardTier = collectEventTargetNextRewardTier;
		Singleton<DataManager>.instance.currentGameData.collectEventResource = collectEventResource;
		Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime = timerAutoOpenTreasureChestRemainTime;
		Singleton<DataManager>.instance.currentGameData.lastSavedOffsetBetweenInternetAndUnbiased = lastSavedOffsetBetweenInternetAndUnbiased;
		Singleton<DataManager>.instance.currentGameData.isSeenPrincessCollectionForPrevVersion = isSeenPrincessCollectionForPrevVersion;
		Singleton<DataManager>.instance.currentGameData.isLowEndDevice = isLowEndDevice;
		Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount = countAutoOpenTreasureChestRemainCount;
		Singleton<DataManager>.instance.currentGameData.lastRecievedNewEventPackageMonth = lastRecievedNewEventPackageMonth;
		Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime = doubleSpeedEndTime;
		Singleton<DataManager>.instance.currentGameData.currentTranscendTier = dictionary5;
		Singleton<DataManager>.instance.currentGameData.reinforcementSkillInventoryData = dictionary4;
		Singleton<DataManager>.instance.currentGameData.transcendPassiveSkillInventoryData = dictionary3;
		Singleton<DataManager>.instance.currentGameData.transcendStone = transcendStone;
		Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount = countableSilverFingerLeftCount;
		Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount = countableGoldFingerLeftCount;
		Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedDayNumber = lastDailyRewardReceivedDayNumber;
		Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedTime = lastDailyRewardReceivedTime;
		Singleton<DataManager>.instance.currentGameData.treasureDailyRewardTime = treasureDailyRewardTime;
		Singleton<DataManager>.instance.currentGameData.adsRubyWatchEndTime = adsRubyWatchEndTime;
		Singleton<DataManager>.instance.currentGameData.adsGoldWatchEndTime = adsGoldWatchEndTime;
		Singleton<DataManager>.instance.currentGameData.isSeenCharacterSkinDescription = isSeenCharacterSkinDescription;
		Singleton<DataManager>.instance.currentGameData.isSeenColleagueSkinDescription = isSeenColleagueSkinDescription;
		Singleton<DataManager>.instance.currentGameData.isObtainedFacebookReward = isObtainedFacebookReward;
		Singleton<DataManager>.instance.currentGameData.isObtainedTwitterReward = isObtainedTwitterReward;
		Singleton<DataManager>.instance.currentGameData.isClearRebirthTutorial = isClearRebirthTutorial;
		Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime = autoTouch13TapEndTime2;
		Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime = autoTouch26TapEndTime;
		Singleton<DataManager>.instance.currentGameData.skillInventoryData = list14;
		Singleton<DataManager>.instance.currentGameData.treasureRubyLotteryEnoughCount = treasureRubyLotteryEnoughCount;
		Singleton<DataManager>.instance.currentGameData.bestTheme = bestTheme;
		Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime = autoTouch13TapEndTime;
		Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList = list12;
		Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList = list13;
		Singleton<DataManager>.instance.currentGameData.treasureItemRandomSeed = treasureItemRandomSeed;
		Singleton<DataManager>.instance.currentGameData.colleagueInventoryList = list4;
		Singleton<DataManager>.instance.currentGameData.treasureRandomSeed = treasureRandomSeed;
		Singleton<DataManager>.instance.currentGameData.treasureLotteryCount = treasureLotteryCount;
		Singleton<DataManager>.instance.currentGameData.usedEventCouponList = list19;
		Singleton<DataManager>.instance.currentGameData.treasureEnchantStone = treasureEnchantStone;
		Singleton<DataManager>.instance.currentGameData.bossRaidBestRecord = bossRaidBestRecord;
		Singleton<DataManager>.instance.currentGameData.heartForBossRaid = heartForBossRaid;
		Singleton<DataManager>.instance.currentGameData.lastHeartUsedTime = lastHeartUsedTime;
		Singleton<DataManager>.instance.currentGameData.adsAngelSpawnTime = adsAngelSpawnTime;
		Singleton<DataManager>.instance.currentGameData.isBoughtAutoTouch = isBoughtAutoTouch;
		Singleton<DataManager>.instance.currentGameData.rebirthCount = rebirthCount;
		Singleton<DataManager>.instance.currentGameData.isClearTutorial = isClearTutorial;
		Singleton<DataManager>.instance.currentGameData.isPushNotificationOn = isPushNotificationOn;
		Singleton<DataManager>.instance.currentGameData.treasurePiece = treasurePiece;
		Debug.LogError(Singleton<DataManager>.instance.currentGameData.treasurePiece);
		Singleton<DataManager>.instance.currentGameData.ruby = ruby;
		Singleton<DataManager>.instance.currentGameData.obscuredRuby = obscuredRuby;
		Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin = equippedWarriorSkin;
		Singleton<DataManager>.instance.currentGameData.equippedPriestSkin = equippedPriestSkin;
		Singleton<DataManager>.instance.currentGameData.equippedArcherSkin = equippedArcherSkin;
		Singleton<DataManager>.instance.currentGameData.warriorSkinData = list7;
		Singleton<DataManager>.instance.currentGameData.priestSkinData = list8;
		Singleton<DataManager>.instance.currentGameData.archerSkinData = list9;
		Singleton<DataManager>.instance.currentGameData.treasureInventoryData = list5;
		Singleton<DataManager>.instance.currentGameData.achievementData = list6;
		Singleton<DataManager>.instance.currentGameData.adWatchCount = adWatchCount;
		Singleton<DataManager>.instance.currentGameData.clearLastThemeCount = clearLastThemeCount;
		Singleton<DataManager>.instance.syncWithDataManager();
		Singleton<QuestManager>.instance.setDefaultQuest();
		Singleton<DataManager>.instance.currentGameData.isSeenSurpriseShop = false;
		Singleton<DataManager>.instance.saveData();
		Singleton<DataManager>.instance.loadData();
		UIWindowProgressRebirth.instance.openWithStartRebirth(num);
		UIWindowColleague.instance.colleagueScrollRectParent.refreshAll();
		UIWindowManageHeroAndWeapon.instance.weaponInfiniteScroll.refreshAll();
	}
}
