using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public class GameData
{
	public int dataVersion = int.MinValue;

	public string applicationVersion;

	public string currentUserID;

	public double lastSavedOffsetBetweenInternetAndUnbiased = double.MinValue;

	public double lastSavedOffsetBetweenInternetAndUnbiasedForUTC = double.MinValue;

	public long lastSaveTime;

	public long lastRewardTimeTick;

	public double gold;

	public long ruby;

	public ObscuredLong obscuredRuby;

	public string rubyRecord;

	public int bestTheme = 1;

	public int unlockTheme = 1;

	public int unlockStage = 1;

	public int currentTheme = 1;

	public int currentStage = 1;

	public double treasureEnchantStone;

	public long adsAngelSpawnTime;

	public long specialAdsAngelSpawnTime;

	public long lastHeartUsedTime;

	public int heartForBossRaid = 3;

	public bool isBoughtAutoTouch;

	public long autoTouch13TapEndTime;

	public long autoTouch26TapEndTime;

	public float jackpotValue;

	public long treasurePiece;

	public string treasurePieceRecord = string.Empty;

	public long treasureRubyLotteryEnoughCount;

	public string treasureTotalLevelUpCountRecord = string.Empty;

	public long treasureDailyRewardTime;

	public long rebirthCount;

	public bool isPushNotificationOn = true;

	public bool isClearTutorial;

	public bool isClearRebirthTutorial;

	public int treasureRandomSeed;

	public int treasureItemRandomSeed;

	public int treasureLotteryCount;

	public bool isObtainedFacebookReward;

	public bool isObtainedTwitterReward;

	public bool isSeenCharacterSkinDescription;

	public bool isSeenColleagueSkinDescription;

	public long adsGoldWatchEndTime;

	public long adsRubyWatchEndTime;

	public int lastDailyRewardReceivedDayNumber;

	public long lastDailyRewardReceivedTime;

	public int countableSilverFingerLeftCount;

	public int countableGoldFingerLeftCount;

	public int countAutoOpenTreasureChestRemainCount;

	public long timerAutoOpenTreasureChestRemainTime;

	public ObscuredLong countDoubleSpeed = 0L;

	public long doubleSpeedEndTime;

	public int lastRecievedNewEventPackageMonth;

	public int lastRecievedWeaponLegendarySkinPackageMonth;

	public bool isSeenSurpriseShop = true;

	public bool isSeenPrincessCollectionForPrevVersion;

	public bool isLowEndDevice;

	public int goldRouletteLastDayOfYear;

	public int goldRouletteRemainCount;

	public int goldRouletteTicket;

	public long bronzeRouletteTargetEndTime;

	public long lastTowerModeStartTime;

	public int towerModeTicketCount;

	public long halloweenPumpkin;

	public int halloweenTargetNextRewardTier;

	public long peperoDayChocolate;

	public int peperoDayTargetNextRewardTier;

	public long christmasEventTree;

	public int christmasEventTargetNextRewardTier;

	public long happyNewYearEventLuckyBag;

	public int happyNewYearEventTargetNextRewardTier;

	public long valentineDayEventChocolet;

	public int valentineDayEventTargetNextRewardTier;

	public ObscuredLong weaponSkinUpdateEventToySword = 0L;

	public ObscuredInt weaponSkinUpdateEventTargetNextRewardTier = 0;

	public ObscuredLong flowerEventResource = 0L;

	public ObscuredInt flowerEventTargetNextRewardTier = 0;

	public ObscuredLong collectEventResource = 0L;

	public ObscuredInt collectEventTargetNextRewardTier = 0;

	public ObscuredInt currentCollectEventIndex = 0;

	public int towerModeLastSeason = -1;

	public int towerModeTimeAttackSeasonBestFloor;

	public float towerModeTimeAttackSeasonBestClearTime;

	public int towerModeTimeAttackBestFloor;

	public float towerModeTimeAttackBestClearTime;

	public int towerModeEndlessModeSeasonBestFloor;

	public float towerModeEndlessModeSeasonBestClearTime;

	public int towerModeEndlessModeBestFloor;

	public float towerModeEndlessModeBestClearTime;

	public long towerModeFreeTicketEndTime;

	public ObscuredLong totalPurchasedMoney = 0L;

	public ObscuredLong weaponSkinPiece = 0L;

	public ObscuredLong weaponSkinReinforcementMasterPiece = 0L;

	public ObscuredBool isUsedFreeWeaponSkinLottery = false;

	public long advertisementShopAppearEndTime;

	public Dictionary<CharacterManager.CharacterType, long> currentTranscendTier;

	public long transcendStone;

	public Dictionary<TranscendManager.TranscendPassiveSkillType, TranscendPassiveSkillInventoryData> transcendPassiveSkillInventoryData;

	public List<ColleagueInventoryData> colleagueInventoryList;

	public BossRaidManager.BossRaidBestRecordData bossRaidBestRecord = default(BossRaidManager.BossRaidBestRecordData);

	public List<AchievementData> achievementData;

	public List<WarriorWeaponInventoryData> warriorWeaponInventoryData;

	public List<PriestWeaponInventoryData> priestWeaponInventoryData;

	public List<ArcherWeaponInventoryData> archerWeaponInventoryData;

	public List<SkillInventoryData> skillInventoryData;

	public List<PassiveSkillInventoryData> passiveSkillInventoryData;

	public WeaponManager.WarriorWeaponType warriorEquippedWeapon = WeaponManager.WarriorWeaponType.WarriorWeapon1;

	public WeaponManager.ArcherWeaponType archerEquippedWeapon = WeaponManager.ArcherWeaponType.ArcherWeapon1;

	public WeaponManager.PriestWeaponType priestEquippedWeapon = WeaponManager.PriestWeaponType.PriestWeapon1;

	public List<TreasureInventoryData> treasureInventoryData;

	public List<WarriorCharacterSkinData> warriorSkinData;

	public List<ArcherCharacterSkinData> archerSkinData;

	public List<PriestCharacterSkinData> priestSkinData;

	public CharacterSkinManager.WarriorSkinType equippedWarriorSkin;

	public CharacterSkinManager.PriestSkinType equippedPriestSkin;

	public CharacterSkinManager.ArcherSkinType equippedArcherSkin;

	public List<QuestData> questData;

	public List<string> usedEventCouponList;

	public List<ShopManager.LimitedItemData> currentTimeCheckingLimitedItemList;

	public List<ShopManager.LimitedItemData> usedLimitedItemList;

	public List<ShopManager.LimitedItemData> newCurrentTimeCheckingLimitedItemList;

	public List<ShopManager.LimitedItemData> newUsedLimitedItemList;

	public Dictionary<SkillManager.SkillType, ReinforcementSkillInventoryData> reinforcementSkillInventoryData;

	private bool isPaidUser;

	public long adWatchCount;

	public long clearLastThemeCount;

	public double heartForElopeMode;

	public List<PrincessInventoryDataForElopeMode> princessInventoryDataForElopeMode;

	public long currentProgressDistanceForElopeMode;

	public double currentAttackingEnemyHeathForElopeMode;

	public Dictionary<ElopeModeManager.DaemonKingSkillType, long> currentDaemonKingLevelForElopeMode;

	public Dictionary<DropItemManager.DropItemType, long> elopeModeResourceData;

	public Dictionary<ElopeModeManager.DaemonKingSkillType, double> currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode;

	public long elopeShopTargetRefreshTime;

	public long lastRefreshElopeShopHour;

	public List<ElopeShopItemData> currentElopeShopItemList;

	public long heartCoinForElopeMode;

	public string heartCoinForElopeModeRecord;

	public long loveTimeMachineRemainCountForElopeMode;

	public bool isSeenElopeModeTutorial;

	public long speedGuyAdsEndTime;

	public Dictionary<CharacterManager.CharacterType, List<WeaponSkinData>> weaponSkinData;

	public ObscuredLong lastPVPStartTime = 0L;

	public ObscuredLong pvpTicketCount = 0L;

	public Dictionary<ObscuredInt, PVPTankData> pvpTankData;

	public ObscuredLong pvpHonorToken = 0L;

	public string pvpHonorTokenRecord = string.Empty;

	public ObscuredLong pvpShopTargetRefreshTime = 0L;

	public ObscuredLong pvpShopShopItemRefreshCount = 0L;

	public List<PVPManager.PVPShopItemData> pvpShopItemList;

	public string characterSkinTotalLevelUpRecord;

	public string colleagueSkinTotalLevelUpRecord;

	public long admobAdsIngnoreTargetTime;

	public bool isBoughtBronzeFinger;

	public long _ruby
	{
		get
		{
			return obscuredRuby;
		}
	}

	public bool IsPaidUser
	{
		get
		{
			DebugManager.LogError("Duplicated variable. Use ShopManager.isPaidUser instead");
			return ShopManager.isPaidUser;
		}
	}
}
