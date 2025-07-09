using System;
using System.Collections.Generic;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;
using MathNet.Numerics.Random;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
	public static int dataVersion = 29;

	public static bool finishProcessGameData;

	public static bool isDynamicAllocationGameData;

	[NonSerialized]
	public GameData currentGameData;

	public bool isLoadedData;

	public bool seenComebackRewardWindow;

	private DateTime m_notificationCooldownTime;

	private void Awake()
	{
		finishProcessGameData = loadData();
	}

	public void saveData()
	{
		if (!TutorialManager.isTutorial)
		{
			currentGameData.lastSaveTime = UnbiasedTime.Instance.Now().Ticks;
			if (Singleton<DataManager>.instance.currentGameData.isPushNotificationOn && m_notificationCooldownTime.Ticks < UnbiasedTime.Instance.Now().Ticks)
			{
				m_notificationCooldownTime = UnbiasedTime.Instance.Now().AddMinutes(5.0);
				NotificationManager.CancelAllLocalNotification();
				NotificationManager.SetComeBackRewardtNotification();
			}
			byte[] inArray = Util.SerializeToStream(currentGameData).ToArray();
			string value = Convert.ToBase64String(inArray);
			NSPlayerPrefs.SetString("GameData", value);
		}
	}

	public void saveData(long saveTime)
	{
		if (!TutorialManager.isTutorial)
		{
			currentGameData.lastSaveTime = saveTime;
			byte[] inArray = Util.SerializeToStream(currentGameData).ToArray();
			string value = Convert.ToBase64String(inArray);
			NSPlayerPrefs.SetString("GameData", value);
		}
	}

	public void initGameData()
	{
		if (currentGameData.dataVersion < 9)
		{
			currentGameData.obscuredRuby = currentGameData.ruby;
		}
		if (currentGameData.dataVersion < 10)
		{
			if (currentGameData.warriorSkinData != null)
			{
				for (int i = 0; i < currentGameData.warriorSkinData.Count; i++)
				{
					if (currentGameData.warriorSkinData[i].skinType == CharacterSkinManager.WarriorSkinType.William)
					{
						currentGameData.warriorSkinData[i].skinLevel = 1L;
					}
					currentGameData.warriorSkinData[i]._skinLevel = currentGameData.warriorSkinData[i].skinLevel;
				}
			}
			if (currentGameData.priestSkinData != null)
			{
				for (int j = 0; j < currentGameData.priestSkinData.Count; j++)
				{
					if (currentGameData.priestSkinData[j].skinType == CharacterSkinManager.PriestSkinType.Olivia)
					{
						currentGameData.priestSkinData[j].skinLevel = 1L;
					}
					currentGameData.priestSkinData[j]._skinLevel = currentGameData.priestSkinData[j].skinLevel;
				}
			}
			if (currentGameData.archerSkinData != null)
			{
				for (int k = 0; k < currentGameData.archerSkinData.Count; k++)
				{
					if (currentGameData.archerSkinData[k].skinType == CharacterSkinManager.ArcherSkinType.Windstoker)
					{
						currentGameData.archerSkinData[k].skinLevel = 1L;
					}
					currentGameData.archerSkinData[k]._skinLevel = currentGameData.archerSkinData[k].skinLevel;
				}
			}
			currentGameData.collectEventResource = 0L;
			currentGameData.collectEventTargetNextRewardTier = 0;
		}
		if (currentGameData.dataVersion < 11 && currentGameData.weaponSkinData != null && currentGameData.weaponSkinData.Count >= 3)
		{
			for (int l = 0; l < 3; l++)
			{
				List<WeaponSkinData> list = currentGameData.weaponSkinData[(CharacterManager.CharacterType)l];
				if (list != null)
				{
					for (int m = 0; m < list.Count; m++)
					{
						list[m].currentWarriorSpecialWeaponSkinType = WeaponSkinManager.WarriorSpecialWeaponSkinType.None;
						list[m].currentPriestSpecialWeaponSkinType = WeaponSkinManager.PriestSpecialWeaponSkinType.None;
						list[m].currentArcherSpecialWeaponSkinType = WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
					}
				}
			}
		}
		if (currentGameData.dataVersion < 14)
		{
			if (currentGameData.colleagueInventoryList != null)
			{
				for (int n = 0; n < currentGameData.colleagueInventoryList.Count; n++)
				{
					if (currentGameData.colleagueInventoryList[n].colleaugeSkinLevelData == null)
					{
						currentGameData.colleagueInventoryList[n].colleaugeSkinLevelData = new Dictionary<int, ObscuredLong>();
					}
					foreach (KeyValuePair<int, bool> colleagueSkinDatum in currentGameData.colleagueInventoryList[n].colleagueSkinData)
					{
						if (!currentGameData.colleagueInventoryList[n].colleaugeSkinLevelData.ContainsKey(colleagueSkinDatum.Key))
						{
							currentGameData.colleagueInventoryList[n].colleaugeSkinLevelData.Add(colleagueSkinDatum.Key, 1L);
						}
					}
				}
			}
			currentGameData.pvpHonorTokenRecord = Base36.Encode(currentGameData.pvpHonorToken);
			long num = 0L;
			if (currentGameData.warriorSkinData != null)
			{
				for (int num2 = 0; num2 < currentGameData.warriorSkinData.Count; num2++)
				{
					if ((long)currentGameData.warriorSkinData[num2]._skinLevel > 1)
					{
						num += (long)currentGameData.warriorSkinData[num2]._skinLevel - 1;
					}
				}
			}
			if (currentGameData.priestSkinData != null)
			{
				for (int num3 = 0; num3 < currentGameData.priestSkinData.Count; num3++)
				{
					if ((long)currentGameData.priestSkinData[num3]._skinLevel > 1)
					{
						num += (long)currentGameData.priestSkinData[num3]._skinLevel - 1;
					}
				}
			}
			if (currentGameData.archerSkinData != null)
			{
				for (int num4 = 0; num4 < currentGameData.archerSkinData.Count; num4++)
				{
					if ((long)currentGameData.archerSkinData[num4]._skinLevel > 1)
					{
						num += (long)currentGameData.archerSkinData[num4]._skinLevel - 1;
					}
				}
			}
			currentGameData.characterSkinTotalLevelUpRecord = Base36.Encode(num);
			currentGameData.colleagueSkinTotalLevelUpRecord = Base36.Encode(0L);
		}
		if (currentGameData.dataVersion < 15)
		{
			currentGameData.rubyRecord = Base36.Encode(currentGameData.obscuredRuby);
		}
		if (currentGameData.dataVersion < 16)
		{
			currentGameData.isBoughtBronzeFinger = false;
		}
		if (currentGameData.dataVersion < 17)
		{
			currentGameData.treasurePieceRecord = Base36.Encode(currentGameData.treasurePiece);
			Debug.LogError(currentGameData.treasurePiece);
			currentGameData.heartCoinForElopeModeRecord = Base36.Encode(currentGameData.heartCoinForElopeMode);
			long num5 = 0L;
			if (currentGameData.treasureInventoryData != null)
			{
				for (int num6 = 0; num6 < currentGameData.treasureInventoryData.Count; num6++)
				{
					num5 += currentGameData.treasureInventoryData[num6].treasureLevel - 1;
				}
			}
			currentGameData.treasureTotalLevelUpCountRecord = Base36.Encode(num5);
		}
		if (currentGameData.dataVersion < 18)
		{
			Debug.LogError(currentGameData.treasurePiece);
			currentGameData.treasurePieceRecord = Base36.Encode(currentGameData.treasurePiece);
		}
		if (currentGameData.dataVersion < 19)
		{
			long num7 = 0L;
			if (currentGameData.treasureInventoryData != null)
			{
				for (int num8 = 0; num8 < currentGameData.treasureInventoryData.Count; num8++)
				{
					num7 += currentGameData.treasureInventoryData[num8].treasureLevel - 1;
				}
			}
			currentGameData.treasureTotalLevelUpCountRecord = Base36.Encode(num7);
		}
		if (currentGameData.dataVersion < 20 && Singleton<ShopManager>.instance.isContainLimitedItemFromUsedInventory(ShopManager.LimitedShopItemType.StarterPack.ToString()))
		{
			currentGameData.isBoughtBronzeFinger = true;
		}
		if (currentGameData.dataVersion < 23)
		{
			ShopManager.LimitedShopItemType targetItemType = ShopManager.LimitedShopItemType.DemonCosplayPackage;
			if (currentGameData.newCurrentTimeCheckingLimitedItemList != null)
			{
				currentGameData.newCurrentTimeCheckingLimitedItemList.RemoveAll((ShopManager.LimitedItemData item) => item.limitedType == targetItemType);
			}
			if (currentGameData.newUsedLimitedItemList != null)
			{
				currentGameData.newUsedLimitedItemList.RemoveAll((ShopManager.LimitedItemData item) => item.limitedType == targetItemType);
			}
		}
		currentGameData.dataVersion = dataVersion;
		if (CollectEventManager.collectEventIndex != (int)currentGameData.currentCollectEventIndex)
		{
			currentGameData.currentCollectEventIndex = CollectEventManager.collectEventIndex;
			currentGameData.collectEventResource = 0L;
			currentGameData.collectEventTargetNextRewardTier = 0;
		}
		if (currentGameData.pvpTankData == null || currentGameData.pvpTankData.Count < (long)PVPManager.getTankMaxCount())
		{
			if (currentGameData.pvpTankData == null)
			{
				currentGameData.pvpTankData = new Dictionary<ObscuredInt, PVPTankData>();
			}
			int count = currentGameData.pvpTankData.Count;
			for (int num9 = count; num9 < (long)PVPManager.getTankMaxCount(); num9++)
			{
				PVPTankData pVPTankData = new PVPTankData();
				pVPTankData.tankIndex = num9;
				pVPTankData.isUnlocked = false;
				pVPTankData.tankLevel = 1L;
				currentGameData.pvpTankData.Add(num9, pVPTankData);
			}
		}
		if (currentGameData.currentElopeShopItemList == null)
		{
			currentGameData.currentElopeShopItemList = new List<ElopeShopItemData>();
		}
		if (currentGameData.elopeModeResourceData == null)
		{
			currentGameData.elopeModeResourceData = new Dictionary<DropItemManager.DropItemType, long>();
			currentGameData.elopeModeResourceData.Add(DropItemManager.DropItemType.ElopeResource1, 0L);
			currentGameData.elopeModeResourceData.Add(DropItemManager.DropItemType.ElopeResource2, 0L);
			currentGameData.elopeModeResourceData.Add(DropItemManager.DropItemType.ElopeResource3, 0L);
			currentGameData.elopeModeResourceData.Add(DropItemManager.DropItemType.ElopeResource4, 0L);
		}
		if (currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode == null)
		{
			currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode = new Dictionary<ElopeModeManager.DaemonKingSkillType, double>();
		}
		if (currentGameData.princessInventoryDataForElopeMode == null)
		{
			currentGameData.princessInventoryDataForElopeMode = new List<PrincessInventoryDataForElopeMode>();
		}
		if (currentGameData.currentDaemonKingLevelForElopeMode == null)
		{
			currentGameData.currentDaemonKingLevelForElopeMode = new Dictionary<ElopeModeManager.DaemonKingSkillType, long>();
		}
		if (currentGameData.reinforcementSkillInventoryData == null || currentGameData.reinforcementSkillInventoryData.Count == 0)
		{
			if (currentGameData.reinforcementSkillInventoryData == null)
			{
				currentGameData.reinforcementSkillInventoryData = new Dictionary<SkillManager.SkillType, ReinforcementSkillInventoryData>();
			}
			int count2 = currentGameData.reinforcementSkillInventoryData.Count;
			for (int num10 = count2; num10 < 5; num10++)
			{
				ReinforcementSkillInventoryData reinforcementSkillInventoryData = new ReinforcementSkillInventoryData();
				reinforcementSkillInventoryData.currentSkillType = (SkillManager.SkillType)num10;
				reinforcementSkillInventoryData.isUnlocked = false;
				reinforcementSkillInventoryData.skillLevel = 1L;
				currentGameData.reinforcementSkillInventoryData.Add(reinforcementSkillInventoryData.currentSkillType, reinforcementSkillInventoryData);
			}
		}
		if (currentGameData.transcendPassiveSkillInventoryData == null || currentGameData.transcendPassiveSkillInventoryData.Count == 0)
		{
			currentGameData.transcendPassiveSkillInventoryData = new Dictionary<TranscendManager.TranscendPassiveSkillType, TranscendPassiveSkillInventoryData>();
			for (int num11 = 0; num11 < 3; num11++)
			{
				TranscendPassiveSkillInventoryData transcendPassiveSkillInventoryData = new TranscendPassiveSkillInventoryData();
				transcendPassiveSkillInventoryData.currentTranscendType = (TranscendManager.TranscendPassiveSkillType)num11;
				transcendPassiveSkillInventoryData.skillLevel = 1L;
				transcendPassiveSkillInventoryData.isUnlocked = false;
				currentGameData.transcendPassiveSkillInventoryData.Add(transcendPassiveSkillInventoryData.currentTranscendType, transcendPassiveSkillInventoryData);
			}
		}
		if (currentGameData.currentTranscendTier == null || currentGameData.currentTranscendTier.Count == 0)
		{
			currentGameData.currentTranscendTier = new Dictionary<CharacterManager.CharacterType, long>();
			for (int num12 = 0; num12 < 3; num12++)
			{
				CharacterManager.CharacterType key = (CharacterManager.CharacterType)num12;
				if (!currentGameData.currentTranscendTier.ContainsKey(key))
				{
					currentGameData.currentTranscendTier.Add(key, 0L);
				}
			}
		}
		if (currentGameData.colleagueInventoryList == null || currentGameData.colleagueInventoryList.Count == 0)
		{
			currentGameData.colleagueInventoryList = new List<ColleagueInventoryData>();
			for (int num13 = 0; num13 < 28; num13++)
			{
				ColleagueInventoryData colleagueInventoryData = new ColleagueInventoryData();
				colleagueInventoryData.level = 1L;
				colleagueInventoryData.lastPassiveUnlockLevel = 1L;
				colleagueInventoryData.isUnlocked = false;
				colleagueInventoryData.isUnlockedFromSlot = false;
				colleagueInventoryData.colleagueType = (ColleagueManager.ColleagueType)num13;
				colleagueInventoryData.colleagueSkinData = new Dictionary<int, bool>();
				colleagueInventoryData.colleaugeSkinLevelData = new Dictionary<int, ObscuredLong>();
				colleagueInventoryData.currentEquippedSkinIndex = 1;
				colleagueInventoryData.colleagueSkinData.Add(colleagueInventoryData.currentEquippedSkinIndex, true);
				colleagueInventoryData.colleaugeSkinLevelData.Add(colleagueInventoryData.currentEquippedSkinIndex, 1L);
				currentGameData.colleagueInventoryList.Add(colleagueInventoryData);
			}
		}
		if (currentGameData.newCurrentTimeCheckingLimitedItemList == null)
		{
			currentGameData.newCurrentTimeCheckingLimitedItemList = new List<ShopManager.LimitedItemData>();
		}
		if (currentGameData.newUsedLimitedItemList == null)
		{
			currentGameData.newUsedLimitedItemList = new List<ShopManager.LimitedItemData>();
		}
		if (currentGameData.usedEventCouponList == null)
		{
			currentGameData.usedEventCouponList = new List<string>();
		}
		if (currentGameData.warriorSkinData == null || currentGameData.warriorSkinData.Count == 0)
		{
			currentGameData.warriorSkinData = new List<WarriorCharacterSkinData>();
			int num14 = 30;
			currentGameData.warriorSkinData.Add(new WarriorCharacterSkinData(true, CharacterSkinManager.WarriorSkinType.William));
			for (int num15 = 1; num15 < num14; num15++)
			{
				currentGameData.warriorSkinData.Add(new WarriorCharacterSkinData(false, (CharacterSkinManager.WarriorSkinType)num15));
			}
		}
		for (int num16 = 0; num16 < currentGameData.warriorSkinData.Count; num16++)
		{
			if (currentGameData.warriorSkinData[num16].isHaving && (long)currentGameData.warriorSkinData[num16]._skinLevel < 1)
			{
				currentGameData.warriorSkinData[num16]._skinLevel = 1L;
			}
		}
		if (currentGameData.priestSkinData == null || currentGameData.priestSkinData.Count == 0)
		{
			currentGameData.priestSkinData = new List<PriestCharacterSkinData>();
			int num17 = 30;
			currentGameData.priestSkinData.Add(new PriestCharacterSkinData(true, CharacterSkinManager.PriestSkinType.Olivia));
			for (int num18 = 1; num18 < num17; num18++)
			{
				currentGameData.priestSkinData.Add(new PriestCharacterSkinData(false, (CharacterSkinManager.PriestSkinType)num18));
			}
		}
		for (int num19 = 0; num19 < currentGameData.priestSkinData.Count; num19++)
		{
			if (currentGameData.priestSkinData[num19].isHaving && (long)currentGameData.priestSkinData[num19]._skinLevel < 1)
			{
				currentGameData.priestSkinData[num19]._skinLevel = 1L;
			}
		}
		if (currentGameData.archerSkinData == null || currentGameData.archerSkinData.Count == 0)
		{
			currentGameData.archerSkinData = new List<ArcherCharacterSkinData>();
			int num20 = 30;
			currentGameData.archerSkinData.Add(new ArcherCharacterSkinData(true, CharacterSkinManager.ArcherSkinType.Windstoker));
			for (int num21 = 1; num21 < num20; num21++)
			{
				currentGameData.archerSkinData.Add(new ArcherCharacterSkinData(false, (CharacterSkinManager.ArcherSkinType)num21));
			}
		}
		for (int num22 = 0; num22 < currentGameData.archerSkinData.Count; num22++)
		{
			if (currentGameData.archerSkinData[num22].isHaving && (long)currentGameData.archerSkinData[num22]._skinLevel < 1)
			{
				currentGameData.archerSkinData[num22]._skinLevel = 1L;
			}
		}
		if (currentGameData.skillInventoryData == null || currentGameData.skillInventoryData.Count == 0)
		{
			currentGameData.skillInventoryData = new List<SkillInventoryData>();
			SkillInventoryData skillInventoryData = new SkillInventoryData();
			skillInventoryData.skillLevel = 1;
			skillInventoryData.skillType = SkillManager.SkillType.DivineSmash;
			skillInventoryData.isHasSkill = true;
			currentGameData.skillInventoryData.Add(skillInventoryData);
			for (int num23 = 1; num23 < 5; num23++)
			{
				skillInventoryData = new SkillInventoryData();
				skillInventoryData.skillLevel = 1;
				skillInventoryData.skillType = (SkillManager.SkillType)num23;
				skillInventoryData.isHasSkill = false;
				currentGameData.skillInventoryData.Add(skillInventoryData);
			}
		}
		if (currentGameData.passiveSkillInventoryData == null || currentGameData.passiveSkillInventoryData.Count == 0)
		{
			currentGameData.passiveSkillInventoryData = new List<PassiveSkillInventoryData>();
			for (int num24 = 0; num24 < 3; num24++)
			{
				PassiveSkillInventoryData passiveSkillInventoryData = new PassiveSkillInventoryData();
				passiveSkillInventoryData.skillLevel = 1L;
				passiveSkillInventoryData.currentPassiveSkillType = (SkillManager.PassiveSkillType)num24;
				passiveSkillInventoryData.isUnlocked = false;
				currentGameData.passiveSkillInventoryData.Add(passiveSkillInventoryData);
			}
		}
		if (currentGameData.treasureInventoryData == null || currentGameData.treasureInventoryData.Count == 0)
		{
			currentGameData.treasureInventoryData = new List<TreasureInventoryData>();
		}
		if (currentGameData.warriorWeaponInventoryData == null || currentGameData.warriorWeaponInventoryData.Count == 0)
		{
			currentGameData.warriorWeaponInventoryData = new List<WarriorWeaponInventoryData>();
			for (int num25 = 1; num25 < 48; num25++)
			{
				WarriorWeaponInventoryData item2 = new WarriorWeaponInventoryData((WeaponManager.WarriorWeaponType)num25, 0L, false, false);
				currentGameData.warriorWeaponInventoryData.Add(item2);
			}
			Singleton<WeaponManager>.instance.obtainWeapon(WeaponManager.WarriorWeaponType.WarriorWeapon1, false);
		}
		if (currentGameData.priestWeaponInventoryData == null || currentGameData.priestWeaponInventoryData.Count == 0)
		{
			currentGameData.priestWeaponInventoryData = new List<PriestWeaponInventoryData>();
			for (int num26 = 1; num26 < 48; num26++)
			{
				PriestWeaponInventoryData item3 = new PriestWeaponInventoryData((WeaponManager.PriestWeaponType)num26, 0L, false, false);
				currentGameData.priestWeaponInventoryData.Add(item3);
			}
			Singleton<WeaponManager>.instance.obtainWeapon(WeaponManager.PriestWeaponType.PriestWeapon1, false);
		}
		if (currentGameData.archerWeaponInventoryData == null || currentGameData.archerWeaponInventoryData.Count == 0)
		{
			currentGameData.archerWeaponInventoryData = new List<ArcherWeaponInventoryData>();
			for (int num27 = 1; num27 < 48; num27++)
			{
				ArcherWeaponInventoryData item4 = new ArcherWeaponInventoryData((WeaponManager.ArcherWeaponType)num27, 0L, false, false);
				currentGameData.archerWeaponInventoryData.Add(item4);
			}
			Singleton<WeaponManager>.instance.obtainWeapon(WeaponManager.ArcherWeaponType.ArcherWeapon1, false);
		}
		if (currentGameData.achievementData == null || currentGameData.achievementData.Count == 0)
		{
			currentGameData.achievementData = new List<AchievementData>();
			for (int num28 = 0; num28 < 18; num28++)
			{
				AchievementData item5 = new AchievementData((AchievementManager.AchievementType)num28, 1, 0.0, 0.0, false, false);
				currentGameData.achievementData.Add(item5);
			}
		}
		if (currentGameData.questData == null || currentGameData.questData.Count == 0)
		{
			currentGameData.questData = new List<QuestData>();
		}
		saveData();
	}

	public bool loadData()
	{
		//Discarded unreachable code: IL_007a
		string @string = NSPlayerPrefs.GetString("GameData", string.Empty);
		if (@string != string.Empty)
		{
			byte[] buffer = Convert.FromBase64String(@string);
			try
			{
				object obj = Util.DeserializeFromStream(new MemoryStream(buffer));
				currentGameData = (GameData)obj;
			}
			catch (Exception)
			{
				UIWindowDialog.openDescription("FAIL_LOAD_DATA_VERSION", UIWindowDialog.DialogState.DelegateOKUI, delegate
				{
					Application.Quit();
				}, string.Empty);
				return false;
			}
			if (currentGameData == null)
			{
				UIWindowDialog.openDescription("FAIL_LOAD_DATA_VERSION", UIWindowDialog.DialogState.DelegateOKUI, delegate
				{
					Application.Quit();
				}, string.Empty);
				return false;
			}
			if (currentGameData.lastSaveTime > currentGameData.lastRewardTimeTick)
			{
				currentGameData.lastRewardTimeTick = currentGameData.lastSaveTime;
			}
			if (currentGameData.dataVersion < dataVersion)
			{
				initGameData();
			}
			else if (currentGameData.dataVersion > dataVersion)
			{
				return false;
			}
			UnbiasedTime.Instance.UpdateOffsetBetweenInternetTimeAndUnbiasedTime();
			if (currentGameData.bestTheme == 0)
			{
				currentGameData.bestTheme = currentGameData.unlockTheme;
			}
			Singleton<GameManager>.instance.setMaxLimitTheme();
			NSPlayerPrefs.SetInt("IsCanEnterElopeMode", Singleton<ElopeModeManager>.instance.isCanStartElopeMode() ? 1 : 0);
			if (currentGameData.lastDailyRewardReceivedDayNumber == 0)
			{
				currentGameData.lastDailyRewardReceivedDayNumber = 1;
			}
			checkNewWeapons();
			checkNewAchievement();
			checkNewSkill();
			checkNewColleagues();
			checkNewWarriorSkins();
			checkNewPriestSkins();
			checkNewArcherSkins();
			checkNewForTranscned();
			checkNewForElopeMode();
			checkNewWeaponSkin();
			Singleton<GoldManager>.instance.displayGold(false, false);
			Singleton<RubyManager>.instance.displayRuby(false, false);
			syncWithDataManager();
			if (currentGameData.treasureRandomSeed <= 0)
			{
				MersenneTwister mersenneTwister = new MersenneTwister();
				currentGameData.treasureRandomSeed = mersenneTwister.Next(2, 1073741823);
			}
			if (currentGameData.treasureItemRandomSeed <= 0)
			{
				MersenneTwister mersenneTwister2 = new MersenneTwister();
				currentGameData.treasureItemRandomSeed = mersenneTwister2.Next(2, 1073741823);
			}
			Debug.LogError("读取第一步");
			Singleton<StatManager>.instance.refreshAllStats();
			Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!refreshCharacterUnlockedState");
			Singleton<CharacterManager>.instance.refreshCharacterUnlockedState();
			Singleton<QuestManager>.instance.questData = currentGameData.questData;
			Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!refreshTreasureInventoryData");
			Singleton<TreasureManager>.instance.refreshTreasureInventoryData();
			if (currentGameData.adsAngelSpawnTime > 60 && UnbiasedTime.Instance.Now().Ticks > new DateTime(currentGameData.adsAngelSpawnTime).AddMinutes(-1.0).Ticks && !TutorialManager.isTutorial && GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				currentGameData.adsAngelSpawnTime = UnbiasedTime.Instance.Now().AddMinutes(1.0).Ticks;
			}
			Singleton<DataManager>.instance.saveData();
			Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!isLoadedData = true;");
			isLoadedData = true;
		}
		else
		{
			isDynamicAllocationGameData = true;
			currentGameData = new GameData();
			initGameData();
			loadData();
		}
		return true;
	}

	public void syncUserIDWithCurrentGameData()
	{
		if (string.IsNullOrEmpty(currentGameData.currentUserID) && Singleton<NanooAPIManager>.instance.UserID.Length > 0)
		{
			currentGameData.currentUserID = Singleton<NanooAPIManager>.instance.UserID;
			saveData();
		}
	}

	public void loadFromCloud()
	{
		if (Singleton<NanooAPIManager>.instance.UserID.Length > 0)
		{
			UIWindowDialog.openDescription("QUESTION_SURE_LOAD_CLOUD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				Singleton<NanooAPIManager>.instance.CallAPI(NanooAPIManager.APIType.LOAD_VALIDATION);
			}, string.Empty);
		}
		else
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("REQUIRE_LOGIN"), I18NManager.Get("GOOGLE_PLAY")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void saveToCloud()
	{
		if (Singleton<NanooAPIManager>.instance.UserID.Length > 0)
		{
			UIWindowLoading.instance.openTimeCheckingLoadingUI(10f, delegate
			{
				UIWindowDialog.openDescription("NETWORK_TIME_OUT", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			});
			GameData gameData = currentGameData;
			object o = gameData;
			byte[] inArray = Util.SerializeToStream(o).ToArray();
			string text = Convert.ToBase64String(inArray);
			Singleton<NanooAPIManager>.instance.CallAPI(NanooAPIManager.APIType.SAVE_VALIDATION, text);
		}
		else
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("REQUIRE_LOGIN"), I18NManager.Get("GOOGLE_PLAY")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void checkRewardDateTime()
	{
		if (TutorialManager.isTutorial || TutorialManager.isRebirthTutorial)
		{
			return;
		}
		DateTime value = new DateTime(currentGameData.lastRewardTimeTick);
		if (value.CompareTo(Util.DateTimeNotUse()) > 0 && UnbiasedTime.Instance.UtcNow().CompareTo(value) > 0 && !seenComebackRewardWindow)
		{
			double totalHours = UnbiasedTime.Instance.UtcNow().Subtract(value).TotalHours;
			if (totalHours >= 1.0)
			{
				UIWindowComebackReward.instance.SetComebackReward(totalHours);
				currentGameData.lastRewardTimeTick = UnbiasedTime.Instance.UtcNow().Ticks;
				saveData();
				seenComebackRewardWindow = true;
			}
		}
		updateRewardDateTime();
	}

	public double GetCalculatedReward()
	{
		DateTime value = new DateTime(currentGameData.lastRewardTimeTick);
		if (value.CompareTo(Util.DateTimeNotUse()) > 0 && UnbiasedTime.Instance.Now().CompareTo(value) > 0)
		{
			double totalHours = UnbiasedTime.Instance.Now().Subtract(value).TotalHours;
			if (totalHours >= 1.0)
			{
				return GetCalculatedReward(totalHours);
			}
		}
		return 0.0;
	}

	public double GetCalculatedReward(double h)
	{
		double num = Math.Min(h * 60.0, 3360.0);
		return num * CalculateManager.getCurrentStandardGold();
	}

	public void updateRewardDateTime()
	{
		if (!TutorialManager.isTutorial && !TutorialManager.isRebirthTutorial)
		{
			currentGameData.lastRewardTimeTick = UnbiasedTime.Instance.UtcNow().Ticks;
		}
	}

	private void checkNewSkill()
	{
		SkillInventoryData[] array = currentGameData.skillInventoryData.ToArray();
		if (array.Length < 5)
		{
			SkillInventoryData[] array2 = new SkillInventoryData[5];
			for (int i = 0; i < array2.Length; i++)
			{
				if (i < array.Length)
				{
					array2[i] = array[i];
					continue;
				}
				array2[i] = new SkillInventoryData();
				array2[i].skillLevel = 1;
				array2[i].skillType = (SkillManager.SkillType)i;
				currentGameData.skillInventoryData.Add(array2[i]);
			}
		}
		int count = currentGameData.passiveSkillInventoryData.Count;
		for (int j = count; j < 3; j++)
		{
			PassiveSkillInventoryData passiveSkillInventoryData = new PassiveSkillInventoryData();
			passiveSkillInventoryData.skillLevel = 1L;
			passiveSkillInventoryData.currentPassiveSkillType = (SkillManager.PassiveSkillType)j;
			passiveSkillInventoryData.isUnlocked = false;
			currentGameData.passiveSkillInventoryData.Add(passiveSkillInventoryData);
		}
		if (currentGameData.reinforcementSkillInventoryData == null)
		{
			currentGameData.reinforcementSkillInventoryData = new Dictionary<SkillManager.SkillType, ReinforcementSkillInventoryData>();
		}
		int count2 = currentGameData.reinforcementSkillInventoryData.Count;
		for (int k = count2; k < 5; k++)
		{
			ReinforcementSkillInventoryData reinforcementSkillInventoryData = new ReinforcementSkillInventoryData();
			reinforcementSkillInventoryData.currentSkillType = (SkillManager.SkillType)k;
			reinforcementSkillInventoryData.isUnlocked = false;
			reinforcementSkillInventoryData.skillLevel = 1L;
		}
	}

	private void checkNewForTranscned()
	{
		for (int i = 0; i < 3; i++)
		{
			CharacterManager.CharacterType key = (CharacterManager.CharacterType)i;
			if (!currentGameData.currentTranscendTier.ContainsKey(key))
			{
				currentGameData.currentTranscendTier.Add(key, 0L);
			}
		}
		for (int j = 0; j < 3; j++)
		{
			TranscendManager.TranscendPassiveSkillType transcendPassiveSkillType = (TranscendManager.TranscendPassiveSkillType)j;
			if (!currentGameData.transcendPassiveSkillInventoryData.ContainsKey(transcendPassiveSkillType))
			{
				TranscendPassiveSkillInventoryData transcendPassiveSkillInventoryData = new TranscendPassiveSkillInventoryData();
				transcendPassiveSkillInventoryData.currentTranscendType = transcendPassiveSkillType;
				transcendPassiveSkillInventoryData.skillLevel = 1L;
				transcendPassiveSkillInventoryData.isUnlocked = false;
				currentGameData.transcendPassiveSkillInventoryData.Add(transcendPassiveSkillType, transcendPassiveSkillInventoryData);
			}
		}
	}

	private void checkNewWeapons()
	{
		if (currentGameData.warriorWeaponInventoryData.Count < 47)
		{
			for (int i = currentGameData.warriorWeaponInventoryData.Count; i < 47; i++)
			{
				WarriorWeaponInventoryData item = new WarriorWeaponInventoryData((WeaponManager.WarriorWeaponType)(i + 1), 0L, false, false);
				currentGameData.warriorWeaponInventoryData.Add(item);
			}
		}
		if (currentGameData.priestWeaponInventoryData.Count < 47)
		{
			for (int j = currentGameData.priestWeaponInventoryData.Count; j < 47; j++)
			{
				PriestWeaponInventoryData item2 = new PriestWeaponInventoryData((WeaponManager.PriestWeaponType)(j + 1), 0L, false, false);
				currentGameData.priestWeaponInventoryData.Add(item2);
			}
		}
		if (currentGameData.archerWeaponInventoryData.Count < 47)
		{
			for (int k = currentGameData.archerWeaponInventoryData.Count; k < 47; k++)
			{
				ArcherWeaponInventoryData item3 = new ArcherWeaponInventoryData((WeaponManager.ArcherWeaponType)(k + 1), 0L, false, false);
				currentGameData.archerWeaponInventoryData.Add(item3);
			}
		}
	}

	private void checkNewColleagues()
	{
		int count = currentGameData.colleagueInventoryList.Count;
		if (currentGameData.colleagueInventoryList.Count < 28)
		{
			for (int i = count; i < 28; i++)
			{
				ColleagueInventoryData colleagueInventoryData = new ColleagueInventoryData();
				colleagueInventoryData.level = 1L;
				colleagueInventoryData.lastPassiveUnlockLevel = 1L;
				colleagueInventoryData.isUnlocked = false;
				colleagueInventoryData.isUnlockedFromSlot = false;
				colleagueInventoryData.colleagueType = (ColleagueManager.ColleagueType)i;
				colleagueInventoryData.colleagueSkinData = new Dictionary<int, bool>();
				colleagueInventoryData.colleaugeSkinLevelData = new Dictionary<int, ObscuredLong>();
				colleagueInventoryData.currentEquippedSkinIndex = 1;
				colleagueInventoryData.colleagueSkinData.Add(colleagueInventoryData.currentEquippedSkinIndex, true);
				colleagueInventoryData.colleaugeSkinLevelData.Add(colleagueInventoryData.currentEquippedSkinIndex, 1L);
				currentGameData.colleagueInventoryList.Add(colleagueInventoryData);
			}
		}
	}

	public void checkNewColleagueSkins()
	{
		for (int i = 0; i < currentGameData.colleagueInventoryList.Count; i++)
		{
			int count = currentGameData.colleagueInventoryList[i].colleagueSkinData.Count;
			int colleagueSkinMaxCount = Singleton<ColleagueManager>.instance.getColleagueSkinMaxCount(currentGameData.colleagueInventoryList[i].colleagueType);
			if (count >= colleagueSkinMaxCount)
			{
				continue;
			}
			for (int j = count; j < colleagueSkinMaxCount; j++)
			{
				if (!currentGameData.colleagueInventoryList[i].colleagueSkinData.ContainsKey(j + 1))
				{
					if (!UIWindowColleagueInformation.instance.newSkinColleagueList.Contains(currentGameData.colleagueInventoryList[i].colleagueType))
					{
						UIWindowColleagueInformation.instance.newSkinColleagueList.Add(currentGameData.colleagueInventoryList[i].colleagueType);
					}
					currentGameData.colleagueInventoryList[i].colleagueSkinData.Add(j + 1, false);
				}
				if (!currentGameData.colleagueInventoryList[i].colleaugeSkinLevelData.ContainsKey(j + 1))
				{
					currentGameData.colleagueInventoryList[i].colleaugeSkinLevelData.Add(j + 1, 1L);
				}
			}
		}
	}

	private void checkNewWarriorSkins()
	{
		if (currentGameData.warriorSkinData.Count >= 30)
		{
			return;
		}
		List<WarriorCharacterSkinData> warriorSkinData = currentGameData.warriorSkinData;
		if (warriorSkinData.Count < 30)
		{
			for (int i = warriorSkinData.Count; i < 30; i++)
			{
				warriorSkinData.Add(new WarriorCharacterSkinData(false, (CharacterSkinManager.WarriorSkinType)i));
			}
		}
	}

	private void checkNewPriestSkins()
	{
		if (currentGameData.priestSkinData.Count >= 30)
		{
			return;
		}
		List<PriestCharacterSkinData> priestSkinData = currentGameData.priestSkinData;
		if (priestSkinData.Count < 30)
		{
			for (int i = priestSkinData.Count; i < 30; i++)
			{
				priestSkinData.Add(new PriestCharacterSkinData(false, (CharacterSkinManager.PriestSkinType)i));
			}
		}
	}

	private void checkNewArcherSkins()
	{
		if (currentGameData.archerSkinData.Count >= 30)
		{
			return;
		}
		List<ArcherCharacterSkinData> archerSkinData = currentGameData.archerSkinData;
		if (archerSkinData.Count < 30)
		{
			for (int i = archerSkinData.Count; i < 30; i++)
			{
				archerSkinData.Add(new ArcherCharacterSkinData(false, (CharacterSkinManager.ArcherSkinType)i));
			}
		}
	}

	public void syncWithDataManager()
	{
		currentGameData.currentTheme = Mathf.Max(currentGameData.unlockTheme, 1);
		currentGameData.currentStage = Mathf.Clamp(currentGameData.unlockStage, 1, Singleton<MapManager>.instance.maxStage);
		currentGameData.unlockTheme = Mathf.Max(currentGameData.unlockTheme, 1);
		currentGameData.unlockStage = Mathf.Clamp(currentGameData.unlockStage, 1, Singleton<MapManager>.instance.maxStage);
		GameManager.currentStage = currentGameData.currentStage;
		GameManager.unlockTheme = currentGameData.unlockTheme;
		GameManager.currentTheme = currentGameData.currentTheme;
	}

	private void checkNewAchievement()
	{
		if (currentGameData.achievementData.Count < 18)
		{
			for (int i = currentGameData.achievementData.Count; i < 18; i++)
			{
				AchievementData item = new AchievementData((AchievementManager.AchievementType)i, 1, 0.0, 0.0, false, false);
				currentGameData.achievementData.Add(item);
			}
		}
		else if (currentGameData.achievementData.Count > 18)
		{
			List<AchievementData> list = new List<AchievementData>();
			for (int j = 18; j < currentGameData.achievementData.Count; j++)
			{
				list.Add(currentGameData.achievementData[j]);
			}
			for (int k = 0; k < list.Count; k++)
			{
				currentGameData.achievementData.Remove(list[k]);
			}
		}
	}

	private void checkNewForElopeMode()
	{
		int count = currentGameData.princessInventoryDataForElopeMode.Count;
		for (int i = count; i < GameManager.maxPrincessCount; i++)
		{
			PrincessInventoryDataForElopeMode princessInventoryDataForElopeMode = new PrincessInventoryDataForElopeMode();
			princessInventoryDataForElopeMode.currentPrincessIndex = i + 1;
			princessInventoryDataForElopeMode.princessLevel = 0L;
			princessInventoryDataForElopeMode.princessTranscendLevel = 0L;
			princessInventoryDataForElopeMode.isUnlocked = false;
			currentGameData.princessInventoryDataForElopeMode.Add(princessInventoryDataForElopeMode);
		}
		count = currentGameData.currentDaemonKingLevelForElopeMode.Count;
		for (int j = count; j < 8; j++)
		{
			ElopeModeManager.DaemonKingSkillType key = (ElopeModeManager.DaemonKingSkillType)j;
			if (!currentGameData.currentDaemonKingLevelForElopeMode.ContainsKey(key))
			{
				currentGameData.currentDaemonKingLevelForElopeMode.Add(key, 0L);
			}
		}
		for (int k = 0; k < 8; k++)
		{
			ElopeModeManager.DaemonKingSkillType key2 = (ElopeModeManager.DaemonKingSkillType)k;
			if (!currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode.ContainsKey(key2))
			{
				currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode.Add(key2, 0.0);
			}
		}
	}

	private void checkNewWeaponSkin()
	{
		if (currentGameData.weaponSkinData == null)
		{
			currentGameData.weaponSkinData = new Dictionary<CharacterManager.CharacterType, List<WeaponSkinData>>();
		}
		for (int i = 0; i < 3; i++)
		{
			CharacterManager.CharacterType key = (CharacterManager.CharacterType)i;
			if (!currentGameData.weaponSkinData.ContainsKey(key))
			{
				currentGameData.weaponSkinData.Add(key, new List<WeaponSkinData>());
			}
		}
	}

	public bool isPossessionCharacterSkin(CharacterSkinManager.WarriorSkinType skinType)
	{
		bool result = false;
		for (int i = 0; i < currentGameData.warriorSkinData.Count; i++)
		{
			if (currentGameData.warriorSkinData[i].skinType == skinType)
			{
				result = currentGameData.warriorSkinData[i].isHaving;
				break;
			}
		}
		return result;
	}

	public bool isPossessionCharacterSkin(CharacterSkinManager.PriestSkinType skinType)
	{
		bool result = false;
		for (int i = 0; i < currentGameData.priestSkinData.Count; i++)
		{
			if (currentGameData.priestSkinData[i].skinType == skinType)
			{
				result = currentGameData.priestSkinData[i].isHaving;
				break;
			}
		}
		return result;
	}

	public bool isPossessionCharacterSkin(CharacterSkinManager.ArcherSkinType skinType)
	{
		bool result = false;
		for (int i = 0; i < currentGameData.archerSkinData.Count; i++)
		{
			if (currentGameData.archerSkinData[i].skinType == skinType)
			{
				result = currentGameData.archerSkinData[i].isHaving;
				break;
			}
		}
		return result;
	}
}
