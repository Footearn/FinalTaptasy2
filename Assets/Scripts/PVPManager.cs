using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;
using MathNet.Numerics.Random;
using UnityEngine;
using UnityEngine.UI;

public class PVPManager : Singleton<PVPManager>
{
	[Serializable]
	public class PVPSkinData
	{
		public CharacterManager.CharacterType currentCharacterType = CharacterManager.CharacterType.Length;

		public CharacterSkinManager.WarriorSkinType currentWarriorSkinType = CharacterSkinManager.WarriorSkinType.Length;

		public CharacterSkinManager.PriestSkinType currentPriestSkinType = CharacterSkinManager.PriestSkinType.Length;

		public CharacterSkinManager.ArcherSkinType currentArcherSkinType = CharacterSkinManager.ArcherSkinType.Length;

		public ColleagueManager.ColleagueType currentColleagueType = ColleagueManager.ColleagueType.None;

		public ObscuredLong currentColleagueSkinIndex;

		public ObscuredLong skinLevel;
	}

	[Serializable]
	public class PvPData
	{
		public PVPSeasonInformation season;

		public PVPUserData player;

		public SeasonEntryRewardData reward_season;

		public SeasonRankingRewardData reward_season_rank;

		public HonorRewardData reward_honor;
	}

	[Serializable]
	public class PVPUserData
	{
		public int rank;

		public string uuid;

		public string nickname;

		public string game_data;

		public string win;

		public string lose;

		public string point;

		public int grade;
	}

	[Serializable]
	public class SeasonRankingResponseData
	{
		public PVPUserData me;

		public PVPUserData[] ranker;
	}

	[Serializable]
	public class HonorRankingResponse
	{
		public HonorRankingData[] honor;
	}

	[Serializable]
	public class HonorRankingData
	{
		public string nickname;

		public string season;

		public string win;

		public string lose;

		public string game_data;

		public string point;

		public int grade;
	}

	[Serializable]
	public class HistoryResponseData
	{
		public HistoryData[] history;
	}

	[Serializable]
	public class HistoryData
	{
		public string season;

		public string state;

		public string point;

		public string add_point;

		public HistoryEnemyData target;

		public int ts;
	}

	public enum PVPAPIType
	{
		NONE = -1,
		GET_MY_DATA,
		START_MATCH,
		SET_RECORD,
		GET_SEASON_RANKING,
		GET_HONOR_RANKING,
		REWARD_HONOR,
		REWARD_SEASON_RANKING,
		REWARD_SEASON_ENTRY,
		GET_HISTORY_RECORD,
		Length
	}

	[Serializable]
	public class PVPGameData
	{
		public PVPGameStatData statData;

		public Dictionary<ObscuredInt, PVPTankData> tankData;

		public string equippedCharacterData;

		public List<PVPSkinData> totalSkinData;
	}

	[Serializable]
	public class PVPSeasonInformation
	{
		public int id;

		public string start_date;

		public string finish_date;
	}

	[Serializable]
	public class HonorRewardData
	{
		public int grade;

		public int point;

		public string item;

		public string entry_yn;

		public string active_yn;
	}

	[Serializable]
	public class SeasonRankingRewardData
	{
		public int rank;

		public string item;

		public string entry_yn;

		public string active_yn;
	}

	[Serializable]
	public class SeasonEntryRewardData
	{
		public string play_count;

		public string s1_entry_yn;

		public string s2_entry_yn;

		public string s3_entry_yn;

		public string s4_entry_yn;

		public string s1_active_yn;

		public string s2_active_yn;

		public string s3_active_yn;

		public string s4_active_yn;
	}

	public enum FuntionParameterType
	{
		None = -1,
		IsWin,
		RankingStart,
		RankingEnd,
		EnemyUUID,
		RewardIndex,
		HistoryLoadInterval,
		Length
	}

	[Serializable]
	public class PVPShopItemData
	{
		public ShopItemType shopItemType;

		public List<double> values = new List<double>();

		public bool isBought;
	}

	[Serializable]
	public class RecordResponseData
	{
		public int rank;

		public string win;

		public string lose;

		public string point;

		public int grade;

		public int play_count;
	}

	[Serializable]
	public class HistoryEnemyData
	{
		public string mode;

		public string nickname;

		public string point;

		public string grade;

		public string game_data;
	}

	[Serializable]
	public class PVPGameStatData
	{
		public double princessBonus;

		public double treasureBonus;

		public double rebirthCount;

		public double patienceTokenValue;

		public double archerArrowValue;

		public double angelHairPinValue;

		public double warriorCapeValue;

		public double seraphHopeValue;

		public double nobleBladeValue;

		public double conquerorRingValue;

		public double heliosHarpValue;

		public double charmOfLunarGoddessValue;

		public double conquerTokenValue;

		public double seraphBlessValue;

		public double heavenShieldValue;
	}

	public class APIData
	{
		public PVPAPIType currentAPIType;

		public APIMethodType methodType;

		public string uri;

		public ParameterType[] parameterTypeList;

		public APIData(PVPAPIType currentAPIType, APIMethodType methodType, string uri)
		{
			this.currentAPIType = currentAPIType;
			this.methodType = methodType;
			this.uri = uri;
			parameterTypeList = Singleton<PVPManager>.instance.getParameterTypeArray(currentAPIType);
		}
	}

	public enum APIMethodType
	{
		None = -1,
		Get,
		Post
	}

	public enum ParameterType
	{
		None = -1,
		client_id,
		platform,
		version,
		uuid,
		nickname,
		game_data,
		ymd,
		postbox_id,
		target_uuid,
		state,
		start,
		finish,
		step,
		limit,
		ts,
		hash
	}

	public enum ShopItemType
	{
		None = -1,
		CharacterSkin,
		ColleagueSkin,
		SpecialWeaponSkin,
		BuyTank,
		UpgradeTank,
		Length
	}

	public class PVPPurchaseData
	{
		public PurchaseType purchaseType;

		public double price;
	}

	public enum PurchaseType
	{
		None = -1,
		Ruby,
		HonorToken,
		ElopeHeartCoin,
		Length
	}

	public const double INTERVAL_BETWEEN_CHARGE_TICKET_TIME = 30.0;

	public const long TICKET_BUY_RUBY_PRICE = 100L;

	public static bool isPlayingPVP;

	public static long reserchEnemyPrice = 10L;

	public List<Sprite> tierSpriteList;

	public GameObject pvpObject;

	public Text[] pvpHonorTokenTexts;

	private PVPGameData m_allyData;

	private PVPGameData m_currentEnemyData;

	public List<PVPUserData> seasonTotalRankingList = new List<PVPUserData>();

	public HistoryResponseData currentHistoryData;

	public PVPUserData practiceTargetUserData;

	public bool isPracticePVP;

	private int m_totalTaskCount;

	private int m_totalCurrentTaskCount;

	public static readonly float maxTimeOutTime = 10f;

	public static readonly string uriPrefix = "/finaltaptasy/v1/arena/";

	public PvPData myPVPData;

	public PVPUserData enemyUnitData;

	public SeasonRankingResponseData currentSeasonRankingResponse;

	public RecordResponseData currentRecordResponse;

	public HonorRankingResponse currentHonorRankingResponse;

	private Dictionary<PVPAPIType, APIData> m_apiDataDictionary = new Dictionary<PVPAPIType, APIData>();

	private List<APIData> m_reservedAPIDataList = new List<APIData>();

	public static long pvpShopRefreshTimeHour = 3L;

	public static long pvpShopItemMaxCount = 6L;

	public static long pvpShopRefreshPrice = 30L;

	private Dictionary<ShopItemType, double> m_pvpShopItemRandomValueDictionary = new Dictionary<ShopItemType, double>
	{
		{
			ShopItemType.CharacterSkin,
			42.5
		},
		{
			ShopItemType.ColleagueSkin,
			42.5
		},
		{
			ShopItemType.SpecialWeaponSkin,
			5.0
		},
		{
			ShopItemType.BuyTank,
			3.0
		},
		{
			ShopItemType.UpgradeTank,
			7.0
		}
	};

	private MersenneTwister m_randomForRandomItemType = new MersenneTwister();

	public Text[] ticketCountTexts;

	public static int maxTicketCount
	{
		get
		{
			return 5;
		}
	}

	public void startPVP(PVPGameData enemyData, bool isPractice = false)
	{
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.PVP)
		{
			return;
		}
		isPracticePVP = isPractice;
		isPlayingPVP = true;
		if (Singleton<AdsAngelManager>.instance.currentAngelObject != null)
		{
			Singleton<AdsAngelManager>.instance.currentAngelObject.endAnimation();
		}
		if (Singleton<AdsAngelManager>.instance.currentSpecialAngelObject != null)
		{
			Singleton<AdsAngelManager>.instance.currentSpecialAngelObject.endAnimation();
		}
		GameManager.currentDungeonType = GameManager.SpecialDungeonType.PVP;
		m_allyData = null;
		UIWindowGateLoading.instance.openGateLoading(true, delegate
		{
			Singleton<AudioManager>.instance.playForceEffectSound("skill_casting", -1f);
			StartCoroutine(waitForSomething(delegate
			{
				m_allyData = getMyUnitData();
				m_currentEnemyData = enemyData;
				for (int i = 0; i < m_currentEnemyData.totalSkinData.Count; i++)
				{
					switch (m_currentEnemyData.totalSkinData[i].currentCharacterType)
					{
					case CharacterManager.CharacterType.Warrior:
					{
						CharacterSkinManager.WarriorSkinType currentWarriorSkinType = m_currentEnemyData.totalSkinData[i].currentWarriorSkinType;
						if (currentWarriorSkinType >= CharacterSkinManager.WarriorSkinType.Length)
						{
							m_currentEnemyData.totalSkinData[i].currentWarriorSkinType = CharacterSkinManager.WarriorSkinType.William;
						}
						break;
					}
					case CharacterManager.CharacterType.Priest:
					{
						CharacterSkinManager.PriestSkinType currentPriestSkinType = m_currentEnemyData.totalSkinData[i].currentPriestSkinType;
						if (currentPriestSkinType >= CharacterSkinManager.PriestSkinType.Length)
						{
							m_currentEnemyData.totalSkinData[i].currentPriestSkinType = CharacterSkinManager.PriestSkinType.Olivia;
						}
						break;
					}
					case CharacterManager.CharacterType.Archer:
					{
						CharacterSkinManager.ArcherSkinType currentArcherSkinType = m_currentEnemyData.totalSkinData[i].currentArcherSkinType;
						if (currentArcherSkinType >= CharacterSkinManager.ArcherSkinType.Length)
						{
							m_currentEnemyData.totalSkinData[i].currentArcherSkinType = CharacterSkinManager.ArcherSkinType.Windstoker;
						}
						break;
					}
					}
				}
				UIWindow.CloseAll();
				pvpObject.SetActive(true);
				Singleton<CachedManager>.instance.ingameSetObject.SetActive(false);
				UIWindowPVP.instance.openPVPUI();
				UIWindowPVP.instance.initTotalHPGauge(getTotalHP(m_allyData), true);
				UIWindowPVP.instance.initTotalHPGauge(getTotalHP(m_currentEnemyData), false);
				createPool(delegate
				{
					Singleton<AudioManager>.instance.playBackgroundSound("pvp_bgm");
					Singleton<PVPHPGaugeManager>.instance.startGame();
					Singleton<PVPProjectileManager>.instance.startGame();
					Singleton<PVPUnitManager>.instance.startGame(m_allyData, m_currentEnemyData);
					Singleton<PVPSkillManager>.instance.startGame();
					UIWindowGateLoading.instance.closeGateLoading(true);
				});
			}));
		});
	}

	public void endPVP()
	{
		UIWindowGateLoading.instance.openGateLoading(false, delegate
		{
			Singleton<AudioManager>.instance.playBackgroundSound("lobby02");
			Singleton<PVPHPGaugeManager>.instance.endGame();
			Singleton<PVPProjectileManager>.instance.endGame();
			Singleton<PVPUnitManager>.instance.endGame();
			Singleton<PVPSkillManager>.instance.endGame();
			destroyPool();
			UIWindow.CloseAll();
			pvpObject.SetActive(false);
			Singleton<CachedManager>.instance.ingameSetObject.SetActive(true);
			Singleton<GameManager>.instance.gameEnd();
			UIWindowGateLoading.instance.closeGateLoading();
			UIWindowPVPMainUI.instance.openPVPMainUI();
			if (!isPracticePVP && (bool)isAbuser())
			{
				UIWindowDialog.openDescriptionNotUsingI18N("[Error Code : 119]", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		});
	}

	private IEnumerator waitForSomething(Action action)
	{
		yield return null;
		action();
	}

	private void createPool(Action endAction)
	{
		m_totalTaskCount = 0;
		m_totalCurrentTaskCount = 0;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < m_currentEnemyData.totalSkinData.Count; i++)
		{
			if (m_currentEnemyData.totalSkinData[i].currentColleagueType == ColleagueManager.ColleagueType.None)
			{
				num2++;
			}
		}
		for (int j = 0; j < m_allyData.totalSkinData.Count; j++)
		{
			if (m_allyData.totalSkinData[j].currentColleagueType == ColleagueManager.ColleagueType.None)
			{
				num++;
			}
		}
		registryToMultithread(delegate
		{
			for (int num8 = 0; num8 < m_currentEnemyData.totalSkinData.Count; num8++)
			{
				if (m_currentEnemyData.totalSkinData[num8].currentColleagueType != ColleagueManager.ColleagueType.None)
				{
					string path = "Prefabs/Ingame/PVP/@PVPColleague" + (int)(m_currentEnemyData.totalSkinData[num8].currentColleagueType + 1);
					NewObjectPool.CreatePool<PVPUnitObject>(Resources.Load<GameObject>(path), 1, true);
				}
			}
			for (int num9 = 0; num9 < m_allyData.totalSkinData.Count; num9++)
			{
				if (m_allyData.totalSkinData[num9].currentColleagueType != ColleagueManager.ColleagueType.None)
				{
					string path2 = "Prefabs/Ingame/PVP/@PVPColleague" + (int)(m_allyData.totalSkinData[num9].currentColleagueType + 1);
					NewObjectPool.CreatePool<PVPUnitObject>(Resources.Load<GameObject>(path2), 1, true);
				}
			}
		});
		int num3 = 1;
		num3 = num + num2;
		num3 = (int)Math.Ceiling((double)num3 / 10.0);
		GameObject baseCharacterObject = Resources.Load<GameObject>("Prefabs/Ingame/PVP/@PVPCharacter");
		for (int k = 0; k < num3; k++)
		{
			registryToMultithread(delegate
			{
				NewObjectPool.CreatePool<PVPUnitObject>(baseCharacterObject, 10, true);
			});
		}
		int num4 = (int)Math.Ceiling((double)m_allyData.totalSkinData.Count / 10.0);
		GameObject baseAllyHP = Resources.Load<GameObject>("Prefabs/Ingame/PVP/@HPGauge_Ally");
		for (int l = 0; l < num4; l++)
		{
			registryToMultithread(delegate
			{
				NewObjectPool.CreatePool<PVPHPGaugeObject>(baseAllyHP, 10, true);
			});
		}
		int num5 = (int)Math.Ceiling((double)m_currentEnemyData.totalSkinData.Count / 10.0);
		GameObject baseEnemyHP = Resources.Load<GameObject>("Prefabs/Ingame/PVP/@HPGauge_Enemy");
		for (int m = 0; m < num5; m++)
		{
			registryToMultithread(delegate
			{
				NewObjectPool.CreatePool<PVPHPGaugeObject>(baseEnemyHP, 10, true);
			});
		}
		registryToMultithread(delegate
		{
			NewObjectPool.CreatePool<PVPTankObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/@PVPTank"), 10, true);
			NewObjectPool.CreatePool<GameObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/@PVPExplosionEffect"), 10, true);
		});
		num3 = m_currentEnemyData.totalSkinData.Count + m_allyData.totalSkinData.Count;
		int num6 = (int)Math.Ceiling((double)num3 / 10.0);
		GameObject baseProjectilObject = Resources.Load<GameObject>("Prefabs/Ingame/PVP/@Projectile");
		GameObject baseHealEffectObject = Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPHealEffect");
		for (int n = 0; n < num6; n++)
		{
			registryToMultithread(delegate
			{
				NewObjectPool.CreatePool<PVPProjectileObject>(baseProjectilObject, 10, true);
				NewObjectPool.CreatePool<GameObject>(baseHealEffectObject, 10, true);
			});
		}
		GameObject baseMeeleIncreaseDamageEffectObject = Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPMeleeDamageIncreaseEffect");
		GameObject baseRangedIncreaseDamageEffectObject = Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPRangedDamageIncreaseEffect");
		for (int num7 = 0; num7 < 5; num7++)
		{
			registryToMultithread(delegate
			{
				NewObjectPool.CreatePool<GameObject>(baseMeeleIncreaseDamageEffectObject, 10, true);
				NewObjectPool.CreatePool<GameObject>(baseRangedIncreaseDamageEffectObject, 10, true);
			});
		}
		registryToMultithread(delegate
		{
			NewObjectPool.CreatePool<GameObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPDivineSmashEffect"), 2, true);
			NewObjectPool.CreatePool<GameObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPDragonSmashEffect"), 2, true);
			NewObjectPool.CreatePool<GameObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPMeleeDamageIncreaseBigEffect"), 6, true);
			NewObjectPool.CreatePool<PVPDragonBreath>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPDragonBreath"), 2, true);
			NewObjectPool.CreatePool<GameObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPHealBigEffect"), 6, true);
			NewObjectPool.CreatePool<PVPProjectileObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPFireBallProjectile"), 4, true);
			NewObjectPool.CreatePool<PVPMeteorObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPMeteor"), 2, true);
			NewObjectPool.CreatePool<GameObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPHeavenArmorBigEffect"), 2, true);
			NewObjectPool.CreatePool<GameObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPHeavenArmorEffect"), 2, true);
			NewObjectPool.CreatePool<PVPHellArrow>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPHellArrow"), 2, true);
			NewObjectPool.CreatePool<PVPPenetrationArrow>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPPenetrationArrow"), 2, true);
			NewObjectPool.CreatePool<NewMovingObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/SkillPrefabs/@PVPLastShotArrow"), 2, true);
		});
		registryToMultithread(delegate
		{
			NewObjectPool.CreatePool<PVPPrincessObject>(Resources.Load<GameObject>("Prefabs/Ingame/PVP/@PVPPrincess"), 8, true);
		});
		registryToMultithread(delegate
		{
			UIWindowGateLoading.instance.setLoadingText(1f, 1f);
		});
		registryToMultithread(delegate
		{
			UIWindowGateLoading.instance.setLoadingText(1f, 1f);
		});
		registryToMultithread(delegate
		{
			endAction();
			Resources.UnloadUnusedAssets();
			GC.Collect();
		});
	}

	private void registryToMultithread(Action action)
	{
		m_totalTaskCount++;
		MultiThreadManager.RegistryTask(delegate
		{
			m_totalCurrentTaskCount++;
			UIWindowGateLoading.instance.setLoadingText(m_totalTaskCount, m_totalCurrentTaskCount);
			action();
		});
	}

	private void destroyPool()
	{
		NewObjectPool.DestroyDynamicPools();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public void increasePVPHonorToken(ObscuredLong value)
	{
		GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
		currentGameData.pvpHonorToken = (long)currentGameData.pvpHonorToken + (long)value;
		Singleton<DataManager>.instance.currentGameData.pvpHonorTokenRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.pvpHonorTokenRecord) + (long)value);
		displayPVPHonorToken();
	}

	public void decreasePVPHonorToken(ObscuredLong value)
	{
		GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
		currentGameData.pvpHonorToken = (long)currentGameData.pvpHonorToken - (long)value;
		Singleton<DataManager>.instance.currentGameData.pvpHonorTokenRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.pvpHonorTokenRecord) - (long)value);
		displayPVPHonorToken();
	}

	public void displayPVPHonorToken()
	{
		for (int i = 0; i < pvpHonorTokenTexts.Length; i++)
		{
			pvpHonorTokenTexts[i].text = Singleton<DataManager>.instance.currentGameData.pvpHonorToken.ToString("N0");
		}
	}

	public void buyTank(int index)
	{
		PVPTankData pVPTankData = Singleton<DataManager>.instance.currentGameData.pvpTankData[index];
		if ((bool)pVPTankData.isUnlocked)
		{
			pVPTankData.tankLevel = Math.Min((long)pVPTankData.tankLevel + 1, getTankMaxLevel());
		}
		else
		{
			pVPTankData.isUnlocked = true;
		}
	}

	private ParameterType[] getParameterTypeArray(PVPAPIType apiType)
	{
		List<ParameterType> list = new List<ParameterType>();
		switch (apiType)
		{
		case PVPAPIType.GET_MY_DATA:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.platform);
			list.Add(ParameterType.version);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.game_data);
			list.Add(ParameterType.ymd);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case PVPAPIType.START_MATCH:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.platform);
			list.Add(ParameterType.version);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.game_data);
			list.Add(ParameterType.ymd);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case PVPAPIType.SET_RECORD:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.platform);
			list.Add(ParameterType.version);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.game_data);
			list.Add(ParameterType.ymd);
			list.Add(ParameterType.target_uuid);
			list.Add(ParameterType.state);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case PVPAPIType.GET_SEASON_RANKING:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.platform);
			list.Add(ParameterType.version);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.game_data);
			list.Add(ParameterType.ymd);
			list.Add(ParameterType.start);
			list.Add(ParameterType.finish);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case PVPAPIType.GET_HONOR_RANKING:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.platform);
			list.Add(ParameterType.version);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.ymd);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case PVPAPIType.REWARD_HONOR:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.platform);
			list.Add(ParameterType.version);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.ymd);
			list.Add(ParameterType.postbox_id);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case PVPAPIType.REWARD_SEASON_RANKING:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.platform);
			list.Add(ParameterType.version);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.ymd);
			list.Add(ParameterType.postbox_id);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case PVPAPIType.REWARD_SEASON_ENTRY:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.platform);
			list.Add(ParameterType.version);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.ymd);
			list.Add(ParameterType.postbox_id);
			list.Add(ParameterType.step);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case PVPAPIType.GET_HISTORY_RECORD:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.platform);
			list.Add(ParameterType.version);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.ymd);
			list.Add(ParameterType.start);
			list.Add(ParameterType.limit);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		}
		return list.ToArray();
	}

	private void Awake()
	{
		m_apiDataDictionary.Add(PVPAPIType.GET_MY_DATA, new APIData(PVPAPIType.GET_MY_DATA, APIMethodType.Post, "info"));
		m_apiDataDictionary.Add(PVPAPIType.START_MATCH, new APIData(PVPAPIType.START_MATCH, APIMethodType.Post, "match"));
		m_apiDataDictionary.Add(PVPAPIType.SET_RECORD, new APIData(PVPAPIType.SET_RECORD, APIMethodType.Post, "record"));
		m_apiDataDictionary.Add(PVPAPIType.GET_SEASON_RANKING, new APIData(PVPAPIType.GET_SEASON_RANKING, APIMethodType.Post, "rank"));
		m_apiDataDictionary.Add(PVPAPIType.GET_HONOR_RANKING, new APIData(PVPAPIType.GET_HONOR_RANKING, APIMethodType.Post, "honor"));
		m_apiDataDictionary.Add(PVPAPIType.REWARD_HONOR, new APIData(PVPAPIType.REWARD_HONOR, APIMethodType.Post, "reward_honor"));
		m_apiDataDictionary.Add(PVPAPIType.REWARD_SEASON_RANKING, new APIData(PVPAPIType.REWARD_SEASON_RANKING, APIMethodType.Post, "reward_season_rank"));
		m_apiDataDictionary.Add(PVPAPIType.REWARD_SEASON_ENTRY, new APIData(PVPAPIType.REWARD_SEASON_ENTRY, APIMethodType.Post, "reward_season_entry"));
		m_apiDataDictionary.Add(PVPAPIType.GET_HISTORY_RECORD, new APIData(PVPAPIType.GET_HISTORY_RECORD, APIMethodType.Post, "history"));
	}

	public static bool isConnectedToPvPServer()
	{
		bool result = false;
		if (Util.isInternetConnection() && !string.IsNullOrEmpty(Singleton<NanooAPIManager>.instance.UserID) && Social.localUser.authenticated && !string.IsNullOrEmpty(Social.localUser.userName))
		{
			result = true;
		}
		return result;
	}

	public void CallAPI(PVPAPIType apiType, Action<bool> endAction = null)
	{
		CallAPI(apiType, null, endAction, true);
	}

	public void CallAPI(PVPAPIType apiType, Dictionary<FuntionParameterType, double> parameterDictionary, Action<bool> endAction = null)
	{
		CallAPI(apiType, parameterDictionary, endAction, true);
	}

	public void CallAPI(PVPAPIType apiType, Action<bool> endAction, bool withLoadingUI)
	{
		CallAPI(apiType, null, endAction, withLoadingUI);
	}

	public void CallAPI(PVPAPIType apiType, Dictionary<FuntionParameterType, double> parameterDictionary, Action<bool> doneAction, bool withLoadingUI)
	{
		if (!isConnectedToPvPServer())
		{
			if (UIWindowLoading.instance != null)
			{
				UIWindowLoading.instance.closeLoadingUI();
			}
			UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			return;
		}
		if (!m_apiDataDictionary.ContainsKey(apiType))
		{
			DebugManager.LogError("KeyNotFound Exception");
			return;
		}
		if (parameterDictionary == null)
		{
			parameterDictionary = new Dictionary<FuntionParameterType, double>();
		}
		for (int i = 0; i < 6; i++)
		{
			if (!parameterDictionary.ContainsKey((FuntionParameterType)i))
			{
				parameterDictionary.Add((FuntionParameterType)i, 0.0);
			}
		}
		APIData aPIData = m_apiDataDictionary[apiType];
		string str = uriPrefix + aPIData.uri;
		string text = NanooAPIManager.getNanooURL() + str;
		WWW wWW = null;
		WWWForm wWWForm = null;
		if (aPIData.methodType == APIMethodType.Get)
		{
			if (aPIData.parameterTypeList != null && aPIData.parameterTypeList.Length > 0)
			{
				string text2 = string.Empty;
				for (int j = 0; j < aPIData.parameterTypeList.Length; j++)
				{
					string text3 = text2;
					text2 = text3 + ((j != 0) ? "&" : "?") + aPIData.parameterTypeList[j].ToString() + "=" + getCalculatedParameter(aPIData.parameterTypeList[j], parameterDictionary);
				}
				text += text2;
			}
			wWW = new WWW(text);
		}
		else
		{
			wWWForm = new WWWForm();
			string text4 = string.Empty;
			if (aPIData.parameterTypeList != null && aPIData.parameterTypeList.Length > 0)
			{
				for (int k = 0; k < aPIData.parameterTypeList.Length; k++)
				{
					string calculatedParameter = getCalculatedParameter(aPIData.parameterTypeList[k], parameterDictionary);
					if (aPIData.parameterTypeList[k] != ParameterType.hash)
					{
						wWWForm.AddField(aPIData.parameterTypeList[k].ToString(), calculatedParameter);
						switch (aPIData.parameterTypeList[k])
						{
						case ParameterType.client_id:
						case ParameterType.uuid:
						case ParameterType.ymd:
						case ParameterType.ts:
							text4 += calculatedParameter;
							break;
						}
						continue;
					}
					text4 = Util.SHA256TokenUTF8(text4, NanooAPIManager.getNanooSecretKey());
					wWWForm.AddField(ParameterType.hash.ToString(), text4);
					break;
				}
			}
			wWW = new WWW(text, wWWForm);
		}
		m_reservedAPIDataList.Add(aPIData);
		StartCoroutine(waitForRequest(apiType, wWW, doneAction, withLoadingUI));
	}

	private IEnumerator waitForRequest(PVPAPIType apiType, WWW www, Action<bool> doneAction, bool withLoadingUI)
	{
		if (www == null)
		{
			yield break;
		}
		if (withLoadingUI)
		{
			UIWindowLoading.instance.openLoadingUI(null, false);
		}
		StopCoroutine("timeOutCheckUpdate");
		StartCoroutine("timeOutCheckUpdate", apiType);
		yield return www;
		m_reservedAPIDataList.Remove(m_apiDataDictionary[apiType]);
		if (m_reservedAPIDataList.Count <= 0)
		{
			StopCoroutine("timeOutCheckUpdate");
			if (withLoadingUI)
			{
				UIWindowLoading.instance.closeLoadingUI();
			}
		}
		bool isSuccess = false;
		string response = www.text.Trim();
		if (www.error != null || string.IsNullOrEmpty(response) || response.Contains("fail"))
		{
			isSuccess = false;
		}
		else
		{
			switch (apiType)
			{
			case PVPAPIType.GET_MY_DATA:
			{
				PvPData data = JsonUtility.FromJson<PvPData>(response);
				if (data != null)
				{
					myPVPData = data;
					isSuccess = true;
				}
				break;
			}
			case PVPAPIType.START_MATCH:
			{
				PVPUserData data2 = (enemyUnitData = JsonUtility.FromJson<PVPUserData>(response));
				if (data2 != null && !string.IsNullOrEmpty(data2.game_data))
				{
					isSuccess = true;
				}
				break;
			}
			case PVPAPIType.SET_RECORD:
				currentRecordResponse = JsonUtility.FromJson<RecordResponseData>(response);
				if (currentRecordResponse != null)
				{
					isSuccess = true;
				}
				break;
			case PVPAPIType.GET_SEASON_RANKING:
			{
				SeasonRankingResponseData seasonRankingResponse = JsonUtility.FromJson<SeasonRankingResponseData>(response);
				if (seasonRankingResponse == null)
				{
					break;
				}
				currentSeasonRankingResponse = seasonRankingResponse;
				if (currentSeasonRankingResponse.ranker != null)
				{
					for (int i = 0; i < currentSeasonRankingResponse.ranker.Length; i++)
					{
						seasonTotalRankingList.Add(currentSeasonRankingResponse.ranker[i]);
					}
					isSuccess = true;
				}
				break;
			}
			case PVPAPIType.GET_HONOR_RANKING:
			{
				HonorRankingResponse honorResponse = JsonUtility.FromJson<HonorRankingResponse>(response);
				if (honorResponse != null)
				{
					currentHonorRankingResponse = honorResponse;
					isSuccess = true;
				}
				break;
			}
			case PVPAPIType.REWARD_HONOR:
				if (response.Contains("success"))
				{
					isSuccess = true;
				}
				break;
			case PVPAPIType.REWARD_SEASON_RANKING:
				if (response.Contains("success"))
				{
					isSuccess = true;
				}
				break;
			case PVPAPIType.REWARD_SEASON_ENTRY:
				if (response.Contains("success"))
				{
					isSuccess = true;
				}
				break;
			case PVPAPIType.GET_HISTORY_RECORD:
			{
				HistoryResponseData historyData = JsonUtility.FromJson<HistoryResponseData>(response);
				if (historyData != null)
				{
					isSuccess = true;
					currentHistoryData = historyData;
				}
				break;
			}
			}
		}
		if (!isSuccess && apiType != PVPAPIType.START_MATCH)
		{
			UIWindowDialog.openDescription("ERROR_FOR_TOWER_MODE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
		if (doneAction != null)
		{
			doneAction(isSuccess);
		}
	}

	private IEnumerator timeOutCheckUpdate(PVPAPIType apiType)
	{
		float timer = 0f;
		while (timer < maxTimeOutTime)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		m_reservedAPIDataList.Remove(m_apiDataDictionary[apiType]);
		StopCoroutine("timeOutCheckUpdate");
		UIWindowLoading.instance.closeLoadingUI();
		if (apiType == PVPAPIType.GET_MY_DATA)
		{
			UIWindowDialog.openDescription("ERROR_FOR_TOWER_MODE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	private bool isStatusSuccess(string status)
	{
		bool result = false;
		switch (status)
		{
		case "success":
			result = true;
			break;
		case "fail":
		case "request_data":
		case "match_hash":
		case "rank":
		case "player":
		case "data_record":
		case "score_record":
		case "client":
			UIWindowDialog.openDescription("ERROR_FOR_TOWER_MODE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			break;
		}
		return result;
	}

	private string getCalculatedParameter(ParameterType parameterType, Dictionary<FuntionParameterType, double> parameterDictionary)
	{
		string result = string.Empty;
		switch (parameterType)
		{
		case ParameterType.platform:
			result = "aos";
			break;
		case ParameterType.version:
			result = GlobalSetting.s_bundleVersion;
			break;
		case ParameterType.client_id:
			result = NanooAPIManager.getNanooGameID();
			break;
		case ParameterType.uuid:
			result = Singleton<NanooAPIManager>.instance.UserID;
			break;
		case ParameterType.nickname:
			result = ((!Social.localUser.authenticated) ? "N/A" : Social.localUser.userName);
			break;
		case ParameterType.game_data:
			result = convertPVPGameDataToString(getMyUnitData());
			break;
		case ParameterType.ymd:
		{
			DateTime dateTime = UnbiasedTime.Instance.Now();
			string arg = dateTime.Year.ToString();
			arg = arg + ((dateTime.Month >= 10) ? string.Empty : "0") + dateTime.Month;
			arg = arg + ((dateTime.Day >= 10) ? string.Empty : "0") + dateTime.Day;
			result = arg;
			break;
		}
		case ParameterType.postbox_id:
			result = Singleton<NanooAPIManager>.instance.PostBoxID;
			break;
		case ParameterType.target_uuid:
			if (enemyUnitData != null)
			{
				result = enemyUnitData.uuid;
			}
			break;
		case ParameterType.state:
			result = ((parameterDictionary[FuntionParameterType.IsWin] != 0.0) ? "win" : "lose");
			break;
		case ParameterType.start:
			if (parameterDictionary.ContainsKey(FuntionParameterType.RankingStart))
			{
				result = parameterDictionary[FuntionParameterType.RankingStart].ToString();
			}
			break;
		case ParameterType.finish:
			if (parameterDictionary.ContainsKey(FuntionParameterType.RankingEnd))
			{
				result = parameterDictionary[FuntionParameterType.RankingEnd].ToString();
			}
			break;
		case ParameterType.ts:
			result = Util.UnixTimestampFromDateTime(UnbiasedTime.Instance.UtcNow()).ToString();
			break;
		case ParameterType.step:
			result = "s" + parameterDictionary[FuntionParameterType.RewardIndex];
			break;
		case ParameterType.limit:
			result = parameterDictionary[FuntionParameterType.HistoryLoadInterval].ToString();
			break;
		}
		return result;
	}

	public ObscuredBool isAbuser()
	{
		bool value = false;
		ObscuredLong value2 = 0L;
		if (Singleton<DataManager>.instance.currentGameData.warriorSkinData != null)
		{
			for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.warriorSkinData.Count; i++)
			{
				if ((long)Singleton<DataManager>.instance.currentGameData.warriorSkinData[i]._skinLevel > 1)
				{
					value2 = (long)value2 + ((long)Singleton<DataManager>.instance.currentGameData.warriorSkinData[i]._skinLevel - 1);
				}
			}
		}
		if (Singleton<DataManager>.instance.currentGameData.priestSkinData != null)
		{
			for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.priestSkinData.Count; j++)
			{
				if ((long)Singleton<DataManager>.instance.currentGameData.priestSkinData[j]._skinLevel > 1)
				{
					value2 = (long)value2 + ((long)Singleton<DataManager>.instance.currentGameData.priestSkinData[j]._skinLevel - 1);
				}
			}
		}
		if (Singleton<DataManager>.instance.currentGameData.archerSkinData != null)
		{
			for (int k = 0; k < Singleton<DataManager>.instance.currentGameData.archerSkinData.Count; k++)
			{
				if ((long)Singleton<DataManager>.instance.currentGameData.archerSkinData[k]._skinLevel > 1)
				{
					value2 = (long)value2 + ((long)Singleton<DataManager>.instance.currentGameData.archerSkinData[k]._skinLevel - 1);
				}
			}
		}
		ObscuredLong value3 = 0L;
		if (Singleton<DataManager>.instance.currentGameData.colleagueInventoryList != null)
		{
			foreach (ColleagueInventoryData colleagueInventory in Singleton<DataManager>.instance.currentGameData.colleagueInventoryList)
			{
				foreach (KeyValuePair<int, ObscuredLong> colleaugeSkinLevelDatum in colleagueInventory.colleaugeSkinLevelData)
				{
					if ((long)colleaugeSkinLevelDatum.Value > 1)
					{
						value3 = (long)value3 + ((long)colleaugeSkinLevelDatum.Value - 1);
					}
				}
			}
		}
		ObscuredLong value4 = 0L;
		if (Singleton<DataManager>.instance.currentGameData.treasureInventoryData != null)
		{
			for (int l = 0; l < Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count; l++)
			{
				value4 = (long)value4 + (Singleton<DataManager>.instance.currentGameData.treasureInventoryData[l].treasureLevel - 1);
			}
		}
		if (Base36.Encode(value4) != Singleton<DataManager>.instance.currentGameData.treasureTotalLevelUpCountRecord)
		{
			value = true;
		}
		if (Base36.Encode(value2) != Singleton<DataManager>.instance.currentGameData.characterSkinTotalLevelUpRecord)
		{
			value = true;
		}
		if (Base36.Encode(value3) != Singleton<DataManager>.instance.currentGameData.colleagueSkinTotalLevelUpRecord)
		{
			value = true;
		}
		if (Base36.Encode(Singleton<DataManager>.instance.currentGameData.pvpHonorToken) != Singleton<DataManager>.instance.currentGameData.pvpHonorTokenRecord)
		{
			value = true;
		}
		if (Base36.Encode(Singleton<DataManager>.instance.currentGameData.obscuredRuby) != Singleton<DataManager>.instance.currentGameData.rubyRecord)
		{
			value = true;
		}
		if (Base36.Encode(Singleton<DataManager>.instance.currentGameData.treasurePiece) != Singleton<DataManager>.instance.currentGameData.treasurePieceRecord)
		{
			value = true;
		}
		if (Base36.Encode(Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode) != Singleton<DataManager>.instance.currentGameData.heartCoinForElopeModeRecord)
		{
			value = true;
		}
		return value;
	}

	public void checkPVPShopData()
	{
		if ((long)Singleton<DataManager>.instance.currentGameData.pvpShopTargetRefreshTime <= 0 || (long)Singleton<DataManager>.instance.currentGameData.pvpShopTargetRefreshTime > UnbiasedTime.Instance.UtcNow().Ticks)
		{
			Singleton<DataManager>.instance.currentGameData.pvpShopTargetRefreshTime = UnbiasedTime.Instance.UtcNow().Ticks;
			Singleton<DataManager>.instance.currentGameData.pvpShopShopItemRefreshCount = 0L;
		}
		TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.UtcNow().Ticks - (long)Singleton<DataManager>.instance.currentGameData.pvpShopTargetRefreshTime);
		long num = (long)(timeSpan.TotalHours / (double)pvpShopRefreshTimeHour) + 1;
		if ((long)Singleton<DataManager>.instance.currentGameData.pvpShopShopItemRefreshCount < num)
		{
			Singleton<DataManager>.instance.currentGameData.pvpShopShopItemRefreshCount = num;
			refreshPVPShopItems();
		}
	}

	public void refreshPVPShopItems()
	{
		if (Singleton<DataManager>.instance.currentGameData.pvpShopItemList == null)
		{
			Singleton<DataManager>.instance.currentGameData.pvpShopItemList = new List<PVPShopItemData>();
		}
		List<PVPShopItemData> pvpShopItemList = Singleton<DataManager>.instance.currentGameData.pvpShopItemList;
		pvpShopItemList.Clear();
		List<ShopItemType> exclusiveTypeList = new List<ShopItemType>();
		for (int i = 0; i < pvpShopItemMaxCount; i++)
		{
			pvpShopItemList.Add(getShopItemData(pvpShopItemList, exclusiveTypeList));
		}
		Singleton<DataManager>.instance.saveData();
	}

	private PVPShopItemData getShopItemData(List<PVPShopItemData> currentShopItemList, List<ShopItemType> exclusiveTypeList)
	{
		int num = 0;
		PVPShopItemData pVPShopItemData;
		do
		{
			IL_0002:
			num++;
			if (num >= 1000)
			{
				return null;
			}
			pVPShopItemData = new PVPShopItemData();
			pVPShopItemData.isBought = false;
			ShopItemType randomPVPShopItemType = getRandomPVPShopItemType(exclusiveTypeList);
			switch (randomPVPShopItemType)
			{
			case ShopItemType.CharacterSkin:
			{
				CharacterManager.CharacterType characterType2 = (CharacterManager.CharacterType)((double)UnityEngine.Random.Range(0, 30000) / 10000.0);
				int num4 = 0;
				switch (characterType2)
				{
				case CharacterManager.CharacterType.Warrior:
				{
					num4 = UnityEngine.Random.Range(0, 30);
					CharacterSkinManager.WarriorSkinType skinType2 = (CharacterSkinManager.WarriorSkinType)num4;
					WarriorCharacterSkinData skinDataFromInventory2 = Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType2);
					if ((!Singleton<CharacterSkinManager>.instance.isNormalCharacterSkin(skinType2) && !skinDataFromInventory2.isHaving) || (long)skinDataFromInventory2._skinLevel >= Singleton<CharacterSkinManager>.instance.getSkinMaxLevel())
					{
						goto IL_0002;
					}
					break;
				}
				case CharacterManager.CharacterType.Priest:
				{
					num4 = UnityEngine.Random.Range(0, 30);
					CharacterSkinManager.PriestSkinType skinType3 = (CharacterSkinManager.PriestSkinType)num4;
					PriestCharacterSkinData skinDataFromInventory3 = Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType3);
					if ((!Singleton<CharacterSkinManager>.instance.isNormalCharacterSkin(skinType3) && !skinDataFromInventory3.isHaving) || (long)skinDataFromInventory3._skinLevel >= Singleton<CharacterSkinManager>.instance.getSkinMaxLevel())
					{
						goto IL_0002;
					}
					break;
				}
				case CharacterManager.CharacterType.Archer:
				{
					num4 = UnityEngine.Random.Range(0, 30);
					CharacterSkinManager.ArcherSkinType skinType = (CharacterSkinManager.ArcherSkinType)num4;
					ArcherCharacterSkinData skinDataFromInventory = Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType);
					if ((!Singleton<CharacterSkinManager>.instance.isNormalCharacterSkin(skinType) && !skinDataFromInventory.isHaving) || (long)skinDataFromInventory._skinLevel >= Singleton<CharacterSkinManager>.instance.getSkinMaxLevel())
					{
						goto IL_0002;
					}
					break;
				}
				}
				pVPShopItemData.values.Add((double)characterType2);
				pVPShopItemData.values.Add(num4);
				break;
			}
			case ShopItemType.ColleagueSkin:
			{
				int num5 = UnityEngine.Random.Range(0, 280000) / 10000;
				int num6 = 0;
				ColleagueManager.ColleagueType colleagueType = (ColleagueManager.ColleagueType)num5;
				if (Singleton<ColleagueManager>.instance.isPremiumColleague(colleagueType))
				{
					goto IL_0002;
				}
				ColleagueInventoryData colleagueInventoryData = Singleton<ColleagueManager>.instance.getColleagueInventoryData(colleagueType);
				if (colleagueInventoryData.isUnlockedFromSlot && colleagueInventoryData.isUnlocked)
				{
					num6 = UnityEngine.Random.Range(1, Singleton<ColleagueManager>.instance.getColleagueSkinMaxCount(colleagueType) + 1);
					if (!colleagueInventoryData.colleagueSkinData[num6] || (long)colleagueInventoryData.colleaugeSkinLevelData[num6] >= (long)Singleton<ColleagueManager>.instance.getColleagueSkinMaxLevel())
					{
						goto IL_0002;
					}
					pVPShopItemData.values.Add(num5);
					pVPShopItemData.values.Add(num6);
					break;
				}
				goto IL_0002;
			}
			case ShopItemType.SpecialWeaponSkin:
			{
				CharacterManager.CharacterType characterType = (CharacterManager.CharacterType)((double)UnityEngine.Random.Range(0, 30000) / 10000.0);
				int num3 = 0;
				switch (characterType)
				{
				case CharacterManager.CharacterType.Warrior:
					num3 = UnityEngine.Random.Range(0, 20000) / 10000;
					break;
				case CharacterManager.CharacterType.Priest:
					num3 = UnityEngine.Random.Range(0, 20000) / 10000;
					break;
				case CharacterManager.CharacterType.Archer:
					num3 = UnityEngine.Random.Range(0, 20000) / 10000;
					break;
				}
				pVPShopItemData.values.Add((double)characterType);
				pVPShopItemData.values.Add(num3);
				break;
			}
			case ShopItemType.BuyTank:
			{
				List<ObscuredInt> list2 = new List<ObscuredInt>();
				for (int j = 0; j < (long)getTankMaxCount(); j++)
				{
					PVPTankData tankData2 = getTankData(j);
					if (tankData2 != null && !tankData2.isUnlocked)
					{
						list2.Add(j);
					}
				}
				if (list2.Count == 0)
				{
					goto IL_0002;
				}
				int num7 = list2[UnityEngine.Random.Range(0, list2.Count)];
				pVPShopItemData.values.Add(num7);
				break;
			}
			case ShopItemType.UpgradeTank:
			{
				List<ObscuredInt> list = new List<ObscuredInt>();
				for (int i = 0; i < (long)getTankMaxCount(); i++)
				{
					PVPTankData tankData = getTankData(i);
					if (tankData != null && (bool)tankData.isUnlocked && (long)tankData.tankLevel < (long)getTankMaxLevel())
					{
						list.Add(i);
					}
				}
				if (list.Count == 0)
				{
					goto IL_0002;
				}
				int num2 = list[UnityEngine.Random.Range(0, list.Count)];
				pVPShopItemData.values.Add(num2);
				break;
			}
			}
			pVPShopItemData.shopItemType = randomPVPShopItemType;
		}
		while (isIncludingSameItem(currentShopItemList, pVPShopItemData));
		return pVPShopItemData;
	}

	private bool isIncludingSameItem(List<PVPShopItemData> itemDataList, PVPShopItemData targetItemData)
	{
		bool result = false;
		for (int i = 0; i < itemDataList.Count; i++)
		{
			if (itemDataList[i].shopItemType != targetItemData.shopItemType)
			{
				continue;
			}
			int count = itemDataList[i].values.Count;
			if (count != targetItemData.values.Count)
			{
				continue;
			}
			if (count == 0)
			{
				result = true;
				continue;
			}
			int num = 0;
			for (int j = 0; j < itemDataList[i].values.Count; j++)
			{
				if (itemDataList[i].values[j] == targetItemData.values[j])
				{
					num++;
				}
			}
			if (num == count)
			{
				result = true;
			}
		}
		return result;
	}

	private ShopItemType getRandomPVPShopItemType(List<ShopItemType> exclusiveTypeList)
	{
		ShopItemType shopItemType = ShopItemType.None;
		while (true)
		{
			IL_0002:
			double num = 0.0;
			foreach (KeyValuePair<ShopItemType, double> item in m_pvpShopItemRandomValueDictionary)
			{
				num += item.Value;
			}
			double num2 = (double)m_randomForRandomItemType.Next(0, (int)num * 100) * 0.01;
			double num3 = 0.0;
			foreach (KeyValuePair<ShopItemType, double> item2 in m_pvpShopItemRandomValueDictionary)
			{
				num3 += item2.Value;
				if (num2 <= num3)
				{
					if (exclusiveTypeList.Contains(item2.Key))
					{
						goto IL_0002;
					}
					shopItemType = item2.Key;
					break;
				}
			}
			break;
		}
		switch (shopItemType)
		{
		case ShopItemType.SpecialWeaponSkin:
		case ShopItemType.BuyTank:
		case ShopItemType.UpgradeTank:
			exclusiveTypeList.Add(shopItemType);
			break;
		}
		return shopItemType;
	}

	public PVPPurchaseData getPurchaseData(PVPShopItemData itemData)
	{
		PVPPurchaseData pVPPurchaseData = new PVPPurchaseData();
		switch (itemData.shopItemType)
		{
		case ShopItemType.CharacterSkin:
		{
			CharacterManager.CharacterType characterType = (CharacterManager.CharacterType)itemData.values[0];
			CharacterSkinStatData characterSkinStatData = default(CharacterSkinStatData);
			bool flag = false;
			bool flag2 = true;
			long num3 = 0L;
			switch (characterType)
			{
			case CharacterManager.CharacterType.Warrior:
			{
				CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)itemData.values[1];
				characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.warriorCharacterSkinData[warriorSkinType];
				flag = Singleton<CharacterSkinManager>.instance.isNormalCharacterSkin(warriorSkinType);
				flag2 = Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(warriorSkinType).isHaving;
				num3 = Singleton<CharacterSkinManager>.instance.getRubyBuyPrice(warriorSkinType);
				break;
			}
			case CharacterManager.CharacterType.Priest:
			{
				CharacterSkinManager.PriestSkinType priestSkinType = (CharacterSkinManager.PriestSkinType)itemData.values[1];
				characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.priestCharacterSkinData[priestSkinType];
				flag = Singleton<CharacterSkinManager>.instance.isNormalCharacterSkin(priestSkinType);
				flag2 = Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(priestSkinType).isHaving;
				num3 = Singleton<CharacterSkinManager>.instance.getRubyBuyPrice(priestSkinType);
				break;
			}
			case CharacterManager.CharacterType.Archer:
			{
				CharacterSkinManager.ArcherSkinType archerSkinType = (CharacterSkinManager.ArcherSkinType)itemData.values[1];
				characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.archerCharacterSkinData[archerSkinType];
				flag = Singleton<CharacterSkinManager>.instance.isNormalCharacterSkin(archerSkinType);
				flag2 = Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(archerSkinType).isHaving;
				num3 = Singleton<CharacterSkinManager>.instance.getRubyBuyPrice(archerSkinType);
				break;
			}
			}
			num3 = 500L;
			pVPPurchaseData.purchaseType = PurchaseType.HonorToken;
			pVPPurchaseData.price = 300.0;
			if (flag2)
			{
				int num4 = (int)(characterSkinStatData.percentDamage + characterSkinStatData.secondStat);
				switch (num4)
				{
				case 200:
					pVPPurchaseData.price = 500.0;
					break;
				case 400:
					pVPPurchaseData.price = 1000.0;
					break;
				default:
				{
					int num5 = num4 / 200;
					pVPPurchaseData.price = num5 * 500;
					break;
				}
				}
			}
			else if (flag)
			{
				pVPPurchaseData.price = num3;
			}
			pVPPurchaseData.price = Math.Max(pVPPurchaseData.price, 500.0);
			break;
		}
		case ShopItemType.ColleagueSkin:
		{
			pVPPurchaseData.purchaseType = PurchaseType.HonorToken;
			ColleagueManager.ColleagueType colleagueType = (ColleagueManager.ColleagueType)itemData.values[0];
			double colleagueSkinStat = Singleton<ColleagueManager>.instance.getColleagueSkinStat(colleagueType, (int)itemData.values[1]);
			if (colleagueSkinStat == 100.0)
			{
				pVPPurchaseData.price = 500.0;
			}
			else if (colleagueSkinStat == 200.0)
			{
				pVPPurchaseData.price = 1000.0;
			}
			else
			{
				int num2 = (int)(colleagueSkinStat / 100.0);
				pVPPurchaseData.price = num2 * 500;
			}
			pVPPurchaseData.price = Math.Max(pVPPurchaseData.price, 500.0);
			break;
		}
		case ShopItemType.SpecialWeaponSkin:
			pVPPurchaseData.purchaseType = PurchaseType.HonorToken;
			pVPPurchaseData.price = 1000.0;
			break;
		case ShopItemType.BuyTank:
			pVPPurchaseData.purchaseType = PurchaseType.HonorToken;
			pVPPurchaseData.price = 1000.0;
			break;
		case ShopItemType.UpgradeTank:
		{
			int num = (int)itemData.values[0];
			pVPPurchaseData.purchaseType = PurchaseType.HonorToken;
			pVPPurchaseData.price = 1000.0;
			break;
		}
		}
		return pVPPurchaseData;
	}

	public void purchaseEvent(PVPShopItemData itemData)
	{
		itemData.isBought = true;
		switch (itemData.shopItemType)
		{
		case ShopItemType.CharacterSkin:
			switch ((int)itemData.values[0])
			{
			case 0:
			{
				CharacterSkinManager.WarriorSkinType skinType3 = (CharacterSkinManager.WarriorSkinType)itemData.values[1];
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType3);
				UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType3, false);
				break;
			}
			case 1:
			{
				CharacterSkinManager.PriestSkinType skinType2 = (CharacterSkinManager.PriestSkinType)itemData.values[1];
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType2);
				UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType2, false);
				break;
			}
			case 2:
			{
				CharacterSkinManager.ArcherSkinType skinType = (CharacterSkinManager.ArcherSkinType)itemData.values[1];
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType);
				UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType, false);
				break;
			}
			}
			break;
		case ShopItemType.ColleagueSkin:
		{
			ColleagueManager.ColleagueType colleagueType = (ColleagueManager.ColleagueType)itemData.values[0];
			Singleton<ColleagueManager>.instance.buyColleagueSkin(colleagueType, (int)itemData.values[1]);
			break;
		}
		case ShopItemType.SpecialWeaponSkin:
		{
			CharacterManager.CharacterType characterType = (CharacterManager.CharacterType)itemData.values[0];
			WeaponSkinData specialWeaponSkinData = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinData(characterType, (int)itemData.values[1], WeaponSkinManager.WeaponSkinGradeType.Rare);
			Singleton<WeaponSkinManager>.instance.obtainWeaponSkin(specialWeaponSkinData);
			UIWindowWeaponSkinLottery.instance.openLotteryUI(specialWeaponSkinData, true);
			break;
		}
		case ShopItemType.BuyTank:
		case ShopItemType.UpgradeTank:
			buyTank((int)itemData.values[0]);
			break;
		}
		Singleton<DataManager>.instance.saveData();
	}

	private void calculateTicketCount()
	{
		TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.UtcNow().Ticks - (long)Singleton<DataManager>.instance.currentGameData.lastPVPStartTime);
		double num = 30.0;
		int num2 = (int)(timeSpan.TotalMinutes / num);
		if (num2 > 0)
		{
			increaseTicket(num2, true);
		}
	}

	public void increaseTicket(int value, bool isInitTime)
	{
		if (value > 0)
		{
			if (isInitTime)
			{
				Singleton<DataManager>.instance.currentGameData.lastPVPStartTime = UnbiasedTime.Instance.UtcNow().Ticks;
			}
			GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
			currentGameData.pvpTicketCount = (long)currentGameData.pvpTicketCount + value;
			Singleton<DataManager>.instance.currentGameData.pvpTicketCount = (long)Mathf.Min((long)Singleton<DataManager>.instance.currentGameData.pvpTicketCount, maxTicketCount);
			Singleton<DataManager>.instance.saveData();
			displayTicket();
		}
	}

	public void decreaseTicket(int value)
	{
		if ((long)Singleton<DataManager>.instance.currentGameData.pvpTicketCount >= maxTicketCount)
		{
			Singleton<DataManager>.instance.currentGameData.lastPVPStartTime = UnbiasedTime.Instance.UtcNow().Ticks;
		}
		Singleton<DataManager>.instance.currentGameData.pvpTicketCount = Math.Max((long)Singleton<DataManager>.instance.currentGameData.pvpTicketCount - value, 0L);
		Singleton<DataManager>.instance.saveData();
		int num = (int)Mathf.Max((long)Singleton<DataManager>.instance.currentGameData.pvpTicketCount, 0f);
		DateTime value2 = new DateTime(Singleton<DataManager>.instance.currentGameData.lastPVPStartTime);
		double num2 = 30.0;
		double num3 = num2 * (double)(maxTicketCount - num);
		if (num < maxTicketCount && UnbiasedTime.Instance.UtcNow() < value2.AddMinutes(num3))
		{
			double num4 = num3 - UnbiasedTime.Instance.UtcNow().Subtract(value2).TotalSeconds;
		}
		displayTicket();
	}

	public void displayTicket()
	{
		if (UIWindowPVPMainUI.instance.isOpen)
		{
			for (int i = 0; i < ticketCountTexts.Length; i++)
			{
				ticketCountTexts[i].text = Singleton<DataManager>.instance.currentGameData.pvpTicketCount.ToString("N0");
			}
			UIWindowSelectTowerModeDifficulty.instance.refreshTicketText();
		}
	}

	private void Update()
	{
		if ((long)Singleton<DataManager>.instance.currentGameData.pvpTicketCount < maxTicketCount)
		{
			TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.UtcNow().Ticks - (long)Singleton<DataManager>.instance.currentGameData.lastPVPStartTime);
			if (timeSpan.TotalMinutes > 0.0 && timeSpan.TotalMinutes >= 30.0)
			{
				calculateTicketCount();
			}
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (GameManager.currentGameState == GameManager.GameState.OutGame && !pause)
		{
			calculateTicketCount();
		}
	}

	public int getTotalSkinCount(PVPGameData gameData)
	{
		return gameData.totalSkinData.Count;
	}

	public double getTotalDamage(PVPGameData gameData)
	{
		double num = 0.0;
		for (int i = 0; i < gameData.totalSkinData.Count; i++)
		{
			num += Singleton<PVPUnitManager>.instance.getCalculatedUnitStat(gameData, gameData.totalSkinData[i], Singleton<PVPUnitManager>.instance.getAttackType(gameData.totalSkinData[i]), false).damage;
		}
		return num;
	}

	public string convertPVPGameDataToString(PVPGameData unitData)
	{
		string result = string.Empty;
		if (unitData != null)
		{
			byte[] inArray = Util.SerializeToStream(unitData).ToArray();
			result = Convert.ToBase64String(inArray);
		}
		return result;
	}

	public PVPGameData convertStringToPVPGameData(string dataString)
	{
		PVPGameData result = null;
		try
		{
			if (!string.IsNullOrEmpty(dataString))
			{
				byte[] buffer = Convert.FromBase64String(dataString);
				object obj = Util.DeserializeFromStream(new MemoryStream(buffer));
				return (PVPGameData)obj;
			}
			return result;
		}
		catch
		{
			return null;
		}
	}

	private PVPGameData getMyUnitData()
	{
		PVPGameData pVPGameData = new PVPGameData();
		pVPGameData.equippedCharacterData = (int)Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin + "," + (int)Singleton<DataManager>.instance.currentGameData.equippedPriestSkin + "," + (int)Singleton<DataManager>.instance.currentGameData.equippedArcherSkin;
		List<PVPSkinData> list = new List<PVPSkinData>();
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.warriorSkinData.Count; i++)
		{
			WarriorCharacterSkinData warriorCharacterSkinData = Singleton<DataManager>.instance.currentGameData.warriorSkinData[i];
			if (warriorCharacterSkinData.isHaving)
			{
				PVPSkinData pVPSkinData = new PVPSkinData();
				pVPSkinData.currentCharacterType = CharacterManager.CharacterType.Warrior;
				pVPSkinData.currentWarriorSkinType = warriorCharacterSkinData.skinType;
				pVPSkinData.skinLevel = (long)Mathf.Max((long)warriorCharacterSkinData._skinLevel, 1f);
				list.Add(pVPSkinData);
			}
		}
		for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.priestSkinData.Count; j++)
		{
			PriestCharacterSkinData priestCharacterSkinData = Singleton<DataManager>.instance.currentGameData.priestSkinData[j];
			if (priestCharacterSkinData.isHaving)
			{
				PVPSkinData pVPSkinData2 = new PVPSkinData();
				pVPSkinData2.currentCharacterType = CharacterManager.CharacterType.Priest;
				pVPSkinData2.currentPriestSkinType = priestCharacterSkinData.skinType;
				pVPSkinData2.skinLevel = (long)Mathf.Max((long)priestCharacterSkinData._skinLevel, 1f);
				list.Add(pVPSkinData2);
			}
		}
		for (int k = 0; k < Singleton<DataManager>.instance.currentGameData.archerSkinData.Count; k++)
		{
			ArcherCharacterSkinData archerCharacterSkinData = Singleton<DataManager>.instance.currentGameData.archerSkinData[k];
			if (archerCharacterSkinData.isHaving)
			{
				PVPSkinData pVPSkinData3 = new PVPSkinData();
				pVPSkinData3.currentCharacterType = CharacterManager.CharacterType.Archer;
				pVPSkinData3.currentArcherSkinType = archerCharacterSkinData.skinType;
				pVPSkinData3.skinLevel = (long)Mathf.Max((long)archerCharacterSkinData._skinLevel, 1f);
				list.Add(pVPSkinData3);
			}
		}
		for (int l = 0; l < Singleton<DataManager>.instance.currentGameData.colleagueInventoryList.Count; l++)
		{
			ColleagueInventoryData colleagueInventoryData = Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[l];
			if (Singleton<ColleagueManager>.instance.isPremiumColleague(colleagueInventoryData.colleagueType) || !colleagueInventoryData.isUnlocked || !colleagueInventoryData.isUnlockedFromSlot)
			{
				continue;
			}
			foreach (KeyValuePair<int, bool> colleagueSkinDatum in colleagueInventoryData.colleagueSkinData)
			{
				if (colleagueSkinDatum.Value)
				{
					PVPSkinData pVPSkinData4 = new PVPSkinData();
					pVPSkinData4.currentColleagueType = colleagueInventoryData.colleagueType;
					pVPSkinData4.currentColleagueSkinIndex = colleagueSkinDatum.Key;
					pVPSkinData4.skinLevel = colleagueInventoryData.colleaugeSkinLevelData[colleagueSkinDatum.Key];
					list.Add(pVPSkinData4);
				}
			}
		}
		pVPGameData.totalSkinData = list;
		pVPGameData.tankData = Singleton<DataManager>.instance.currentGameData.pvpTankData;
		PVPGameStatData pVPGameStatData = new PVPGameStatData();
		pVPGameStatData.princessBonus = (GameManager.getCurrentPrincessNumber() - 1) * 100;
		double num = (pVPGameStatData.treasureBonus = Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count * 50);
		pVPGameStatData.rebirthCount = Singleton<DataManager>.instance.currentGameData.rebirthCount;
		pVPGameStatData.patienceTokenValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.PatienceToken);
		pVPGameStatData.archerArrowValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.ArcherArrow);
		pVPGameStatData.angelHairPinValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.AngelHairPin);
		pVPGameStatData.warriorCapeValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.WarriorCape);
		pVPGameStatData.seraphHopeValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.SeraphHope);
		pVPGameStatData.nobleBladeValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.NobleBlade);
		pVPGameStatData.conquerorRingValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.ConquerorRing);
		pVPGameStatData.heliosHarpValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.HeliosHarp);
		pVPGameStatData.charmOfLunarGoddessValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.CharmOfLunarGoddess);
		pVPGameStatData.conquerTokenValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.ConquerToken);
		pVPGameStatData.seraphBlessValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.SeraphBless);
		pVPGameStatData.heavenShieldValue = getTreasureCalculatedValueForPvP(TreasureManager.TreasureType.HeavenShield);
		pVPGameData.statData = pVPGameStatData;
		return pVPGameData;
	}

	private double getTreasureCalculatedValueForPvP(TreasureManager.TreasureType treasureType)
	{
		double result = 0.0;
		if (Singleton<TreasureManager>.instance.containTreasureFromInventory(treasureType))
		{
			TreasureInventoryData treasureDataFromInventory = Singleton<TreasureManager>.instance.getTreasureDataFromInventory(treasureType);
			int num = (int)Mathf.Min(treasureDataFromInventory.treasureLevel, Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].maxLevel);
			result = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].treasureEffectValue;
			result += Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].increasingValueEveryEnchant * (double)(num - 1);
		}
		return result;
	}

	public double getTotalDamageAverage(PVPGameData unitData)
	{
		double num = 0.0;
		for (int i = 0; i < unitData.totalSkinData.Count; i++)
		{
			num += Singleton<PVPUnitManager>.instance.getCalculatedUnitStat(unitData, unitData.totalSkinData[i], Singleton<PVPUnitManager>.instance.getAttackType(unitData.totalSkinData[i]), false).damage;
		}
		return num / (double)unitData.totalSkinData.Count;
	}

	private double getTotalHP(PVPGameData unitData)
	{
		double num = 0.0;
		for (int i = 0; i < unitData.totalSkinData.Count; i++)
		{
			num += Singleton<PVPUnitManager>.instance.getCalculatedUnitStat(unitData, unitData.totalSkinData[i], Singleton<PVPUnitManager>.instance.getAttackType(unitData.totalSkinData[i]), false).hp;
		}
		return num;
	}

	public string getTierName(int tier)
	{
		string title = "PVP_TIER_" + (tier + 1);
		if (I18NManager.Contains(title))
		{
			return I18NManager.Get(title);
		}
		return "--";
	}

	public Sprite getTierIconSprite(int tier)
	{
		tier = Mathf.Clamp(tier, 0, 9);
		return tierSpriteList[tier];
	}

	public static ObscuredLong getTankMaxCount()
	{
		return 5L;
	}

	public static ObscuredLong getTankMaxLevel()
	{
		return 21L;
	}

	public PVPTankData getTankData(ObscuredInt tankIndex)
	{
		PVPTankData pVPTankData = null;
		if (Singleton<DataManager>.instance.currentGameData.pvpTankData.ContainsKey(tankIndex))
		{
			return Singleton<DataManager>.instance.currentGameData.pvpTankData[tankIndex];
		}
		return null;
	}

	public Sprite getPurchaseResourceIconSprite(PurchaseType purchaseType)
	{
		Sprite result = null;
		switch (purchaseType)
		{
		case PurchaseType.Ruby:
			result = Singleton<CachedManager>.instance.rubyIconImage;
			break;
		case PurchaseType.HonorToken:
			result = Singleton<CachedManager>.instance.pvpHonorTokenSprite;
			break;
		case PurchaseType.ElopeHeartCoin:
			result = Singleton<FlyResourcesManager>.instance.heartCoinSprite;
			break;
		}
		return result;
	}

	public string getTankName(int tankIndex)
	{
		return I18NManager.Get("PVP_TANK_NAME_" + (tankIndex + 1));
	}
}
