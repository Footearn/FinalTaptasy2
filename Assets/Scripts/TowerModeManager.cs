using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.UI;

public class TowerModeManager : Singleton<TowerModeManager>
{
	public enum TowerModeDifficultyType
	{
		None = -1,
		Easy,
		Normal,
		Hard,
		VeryHard,
		Hell,
		TimeAttack,
		Endless
	}

	public enum ObstacleType
	{
		None = -1,
		FireBall,
		IronMace,
		Rock,
		Length
	}

	[Serializable]
	public class UserInformationData
	{
		public string season_id;

		public string rank;

		public string nickname;

		public string clear_time;

		public string clear_floor;

		public string game_data;
	}

	[Serializable]
	public class HonorRankingResponseData
	{
		public string status;

		public List<UserInformationData> honor;
	}

	public enum TowerModeAPIType
	{
		NONE = -1,
		GET_SEASON_INFORMATION,
		GET_MY_RANKING,
		GET_ALL_RANKING,
		GET_HONOR_RANKING,
		SET_MY_RECORD
	}

	public enum FuntionParameterType
	{
		None = -1,
		ClearTime,
		ClearFloor,
		RankingStart,
		RankingEnd,
		Length
	}

	[Serializable]
	public class MyRankResponseData
	{
		public string status;

		public UserInformationData player;
	}

	[Serializable]
	public class SeasonResponseData
	{
		public string id;

		public string start_date;

		public string finish_date;
	}

	public enum TowerModeRankingRewardType
	{
		None = -1,
		Treasure_ConquerToken,
		Treasure_PatienceToken,
		WarriorSkin,
		PriestSkin,
		ArcherSkin,
		Length
	}

	[Serializable]
	public struct TowerModeRecordData
	{
		public ObscuredInt currentLifeCount;

		public ObscuredInt currentFloor;

		public ObscuredFloat currentPlayTime;

		public TowerModeRecordData(ObscuredInt currnetLifeCount, ObscuredInt currentFloor, ObscuredFloat currentPlayTime)
		{
			currentLifeCount = currnetLifeCount;
			this.currentFloor = currentFloor;
			this.currentPlayTime = currentPlayTime;
		}
	}

	public class APIData
	{
		public TowerModeAPIType currentAPIType;

		public APIMethodType methodType;

		public string uri;

		public ParameterType[] parameterTypeList;

		public APIData(TowerModeAPIType currentAPIType, APIMethodType methodType, string uri)
		{
			this.currentAPIType = currentAPIType;
			this.methodType = methodType;
			this.uri = uri;
			parameterTypeList = Singleton<TowerModeManager>.instance.getParameterTypeArray(currentAPIType);
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
		mode,
		uuid,
		postbox_id,
		nickname,
		clear_time,
		clear_floor,
		game_data,
		season_id,
		version,
		block,
		start,
		finish,
		ts,
		hash
	}

	[Serializable]
	public class SeasonRankingResponseData
	{
		public string status;

		public List<UserInformationData> player;

		public UserInformationData me;
	}

	public const double INTERVAL_BETWEEN_CHARGE_TICKET_TIME = 60.0;

	public List<TowerModeRecordData> currentPlayRecordDataList = new List<TowerModeRecordData>();

	public static bool isBossCheatOn = false;

	public static bool isPauseObstacleAndMonster;

	public static int currentSeason = 1;

	public long seasonDataTime = new DateTime(2017, 3, 1, 23, 59, 59).Ticks;

	public static DateTime currentSeasonEndDateTime = new DateTime(1970, 1, 1, 0, 0, 0);

	public static int adsTicketGainCount = 5;

	public static long freeTicketHour = 1L;

	public List<TowerModeMapObject> listTowerModeMapObjs = new List<TowerModeMapObject>();

	public TowerModeCharacterObject curPlayingCharacter;

	public bool isDead;

	public ObscuredBool isAbuser;

	public TowerModeDifficultyType currentDifficultyType = TowerModeDifficultyType.None;

	public bool isFightingWithBoss;

	public ObscuredInt currentFloor;

	public List<EnemyManager.MonsterType> rangedMonsterList = new List<EnemyManager.MonsterType>();

	public TowerModeSkyBackgroundObject currentSkyBackgroundObject;

	public bool isDeadBoss;

	public bool isPlayWithFreeTicket;

	private float m_intervalBetweenNormalMapObject = 2f;

	private float m_intervalBetweenMiniBossMapObject = 3.58f;

	private float m_intervalBetweenBossMapObject = 5.27f;

	private float m_curOffsetY;

	private int lastCreatedFloor = -1;

	private ObscuredFloat m_currentTimer;

	private List<TowerModeMonsterObject> m_enemyList;

	private List<TowerModeEnemyProjectileObject> m_projectileList;

	private Dictionary<ObstacleType, double> m_obstacleChance;

	public List<TowerModeMonsterObject> listTowerModeMonsterObjects = new List<TowerModeMonsterObject>();

	public List<TowerModeEnemyProjectileObject> listTowerModeProjectileObjects = new List<TowerModeEnemyProjectileObject>();

	public float monsterObjectSpawnTimer;

	public List<TowerModeObstacle> listTowerModeObstacles = new List<TowerModeObstacle>();

	public List<TowerModeFlameObject> listTowerModeFlames = new List<TowerModeFlameObject>();

	public Sprite[] obstacleIndicatorSpriteLists;

	public float obstacleSpawnTimer;

	public List<Sprite> obstacleSpriteList;

	private float m_obstacleRange = 0.65f;

	private float m_obstacleSpawnMinXPos = -2.7f;

	private float m_obstacleSpawnMaxXPos = 2.7f;

	public static readonly float maxTimeOutTime = 10f;

	public static readonly string uriPrefix = "/finaltaptasy/v1/demongtower/";

	private SeasonResponseData m_seasonData;

	private MyRankResponseData m_myTimeAttackRankingData;

	private MyRankResponseData m_myEndlessRankingData;

	private List<UserInformationData> m_totalTimeAttackSeasonRankingDataList = new List<UserInformationData>();

	private List<UserInformationData> m_totalEndlessSeasonRankingDataList = new List<UserInformationData>();

	private HonorRankingResponseData m_totalTimeAttackHonorRankingData;

	private HonorRankingResponseData m_totalEndlessHonorRankingData;

	private Dictionary<TowerModeAPIType, APIData> m_apiDataDictionary = new Dictionary<TowerModeAPIType, APIData>();

	private List<APIData> m_reservedAPIDataList = new List<APIData>();

	public Text[] ticketCountTexts;

	public float intervalBetweenNormalMap
	{
		get
		{
			return m_intervalBetweenNormalMapObject;
		}
	}

	public float intervalBetweenMiniBossMap
	{
		get
		{
			return m_intervalBetweenMiniBossMapObject;
		}
	}

	public float intervalBetweenBossMap
	{
		get
		{
			return m_intervalBetweenBossMapObject;
		}
	}

	public ObscuredFloat currentTimer
	{
		get
		{
			return m_currentTimer;
		}
	}

	public bool isLoadedSeasonInformation
	{
		get
		{
			return (m_seasonData != null) ? true : false;
		}
	}

	public SeasonResponseData seasonData
	{
		get
		{
			return m_seasonData;
		}
	}

	public MyRankResponseData myTimeAttackRankingData
	{
		get
		{
			return m_myTimeAttackRankingData;
		}
	}

	public MyRankResponseData myEndlessRankingData
	{
		get
		{
			return m_myEndlessRankingData;
		}
	}

	public List<UserInformationData> totalTimeAttackSeasonRankingDataList
	{
		get
		{
			return m_totalTimeAttackSeasonRankingDataList;
		}
	}

	public List<UserInformationData> totalEndlessSeasonRankingDataList
	{
		get
		{
			return m_totalEndlessSeasonRankingDataList;
		}
	}

	public HonorRankingResponseData totalTimeAttackHonorRankingData
	{
		get
		{
			return m_totalTimeAttackHonorRankingData;
		}
	}

	public HonorRankingResponseData totalEndlessHonorRankingData
	{
		get
		{
			return m_totalEndlessHonorRankingData;
		}
	}

	public static int maxTicketCount
	{
		get
		{
			return 10;
		}
	}

	private void Start()
	{
		m_obstacleChance = new Dictionary<ObstacleType, double>();
		for (int i = 0; i < 3; i++)
		{
			m_obstacleChance.Add((ObstacleType)i, 0.0);
		}
		listTowerModeMapObjs.Clear();
		m_enemyList = new List<TowerModeMonsterObject>();
		m_projectileList = new List<TowerModeEnemyProjectileObject>();
		calculateTicketCount();
	}

	private void OnDetectedCheat()
	{
		isAbuser = true;
		Singleton<LogglyManager>.instance.SendLoggly("Detected Tower Mode Abuser UserID : " + Singleton<NanooAPIManager>.instance.UserID, "TowerModeManager.OnDetectedCheat()", LogType.Warning);
	}

	public void startTowerMode(TowerModeDifficultyType type, bool isFreeTicket)
	{
		Singleton<StatManager>.instance.refreshAllStats();
		ObscuredCheatingDetector.StartDetection(OnDetectedCheat);
		Singleton<ElopeModeManager>.instance.displayHeartCoin();
		GameManager.currentGameState = GameManager.GameState.Playing;
		GameManager.currentDungeonType = GameManager.SpecialDungeonType.TowerMode;
		Singleton<CachedManager>.instance.themeEventGround.SetActive(false);
		Singleton<DropItemManager>.instance.startGame();
		isPlayWithFreeTicket = isFreeTicket;
		if (currentSkyBackgroundObject == null)
		{
			currentSkyBackgroundObject = ObjectPool.Spawn("@TowerModeSky", Vector2.zero, CameraFollow.instance.cachedTransform).GetComponent<TowerModeSkyBackgroundObject>();
		}
		CameraFit.Instance.targetSize = -1f;
		CameraFit.Instance.ComputeResolution();
		Singleton<AudioManager>.instance.playBackgroundSound("tower_bgm1");
		currentDifficultyType = type;
		pauseAllObstaclesAndNormalMonsters(false);
		isFightingWithBoss = false;
		isAbuser = false;
		m_curOffsetY = 0f;
		lastCreatedFloor = 1;
		currentFloor = 1;
		obstacleSpawnTimer = 0f;
		isDead = false;
		isDeadBoss = false;
		createPool();
		GameManager.isPause = false;
		ShakeCamera.Instance.targetYPos = 0f;
		ShakeCamera.Instance.cachedTransform.localPosition = new Vector3(0f, 0f, -10f);
		CameraFollow.instance.startGame();
		UIWindow.CloseAll();
		UIWindowTowerMode.instance.open();
		UIWindowTowerMode.instance.setFloorText(1);
		UIWindowTowerMode.instance.setTimerText(m_currentTimer);
		Singleton<MiniPopupManager>.instance.refreshForcePositions();
		Singleton<MiniPopupManager>.instance.refreshTargetPositions();
		for (int i = 0; i < Singleton<CachedManager>.instance.alwaysTopCanvasUICanvasGroup.Length; i++)
		{
			Singleton<CachedManager>.instance.alwaysTopCanvasUICanvasGroup[i].alpha = 0f;
		}
		for (int j = 0; j < Singleton<CharacterManager>.instance.constCharacterList.Count; j++)
		{
			if (Singleton<CharacterManager>.instance.constCharacterList[j] != null)
			{
				ObjectPool.Recycle("@" + Singleton<CharacterManager>.instance.constCharacterList[j].name, Singleton<CharacterManager>.instance.constCharacterList[j].cachedGameObject);
			}
		}
		setObstacleChance(currentDifficultyType);
		setRangedMonsterLists();
		if (Singleton<AdsAngelManager>.instance.currentAngelObject != null)
		{
			Singleton<AdsAngelManager>.instance.currentAngelObject.endAnimation();
		}
		if (Singleton<AdsAngelManager>.instance.currentSpecialAngelObject != null)
		{
			Singleton<AdsAngelManager>.instance.currentSpecialAngelObject.endAnimation();
		}
		recycleAllBackground();
		recycleAllObstacles();
		recycleAllMonsterObjects();
		for (int k = 0; k < 5; k++)
		{
			spawnBackground(false);
		}
		spawnCharacter();
		Singleton<GameManager>.instance.Pause(false);
		StopCoroutine("obstacleSpawnUpdate");
		StopCoroutine("MonsterObjectSpawnUpdate");
		StartCoroutine("obstacleSpawnUpdate");
		StartCoroutine("MonsterObjectSpawnUpdate");
		m_currentTimer = 0f;
		currentPlayRecordDataList.Clear();
		currentPlayRecordDataList.Add(new TowerModeRecordData(3, 1, 0f));
		StopCoroutine("towerModeTimerUpdate");
		StartCoroutine("towerModeTimerUpdate");
		GC.Collect();
	}

	public void endTowerMode(bool withCollectGC = true, bool withOpenSelectUI = true)
	{
		Singleton<DropItemManager>.instance.endGame();
		currentPlayRecordDataList.Clear();
		GameManager.isPause = true;
		Singleton<AudioManager>.instance.playBackgroundSound("lobby02");
		for (int i = 0; i < Singleton<CachedManager>.instance.alwaysTopCanvasUICanvasGroup.Length; i++)
		{
			Singleton<CachedManager>.instance.alwaysTopCanvasUICanvasGroup[i].alpha = 1f;
		}
		StopAllCoroutines();
		recycleAllBackground();
		recycleAllObstacles();
		recycleCharacter();
		recycleAllMonsterObjects();
		if (currentSkyBackgroundObject != null)
		{
			ObjectPool.Recycle(currentSkyBackgroundObject.name, currentSkyBackgroundObject.cachedGameObject);
			currentSkyBackgroundObject = null;
		}
		Singleton<DataManager>.instance.saveData();
		Singleton<GameManager>.instance.setGameState(GameManager.GameState.OutGame);
		Singleton<MiniPopupManager>.instance.refreshForcePositions();
		Singleton<MiniPopupManager>.instance.refreshTargetPositions();
		GameManager.isWaitForStartGame = false;
		ObjectPool.DestroyDynamicCreatedPools();
		Resources.UnloadUnusedAssets();
		if (withCollectGC)
		{
			GC.Collect();
		}
		if (withOpenSelectUI)
		{
			UIWindowSelectTowerModeDifficulty.instance.openSelectTowerModeDifficulty(true);
		}
		ObscuredCheatingDetector.StopDetection();
	}

	public void setObstacleChance(TowerModeDifficultyType difficulty)
	{
		m_obstacleChance.Clear();
		switch (difficulty)
		{
		case TowerModeDifficultyType.Easy:
			m_obstacleChance[ObstacleType.FireBall] = 15.0;
			m_obstacleChance[ObstacleType.IronMace] = 30.0;
			m_obstacleChance[ObstacleType.Rock] = 55.0;
			break;
		case TowerModeDifficultyType.Normal:
			m_obstacleChance[ObstacleType.FireBall] = 30.0;
			m_obstacleChance[ObstacleType.IronMace] = 25.0;
			m_obstacleChance[ObstacleType.Rock] = 45.0;
			break;
		case TowerModeDifficultyType.Hard:
			m_obstacleChance[ObstacleType.FireBall] = 40.0;
			m_obstacleChance[ObstacleType.IronMace] = 30.0;
			m_obstacleChance[ObstacleType.Rock] = 30.0;
			break;
		case TowerModeDifficultyType.VeryHard:
			m_obstacleChance[ObstacleType.FireBall] = 50.0;
			m_obstacleChance[ObstacleType.IronMace] = 40.0;
			m_obstacleChance[ObstacleType.Rock] = 10.0;
			break;
		case TowerModeDifficultyType.Hell:
			m_obstacleChance[ObstacleType.FireBall] = 50.0;
			m_obstacleChance[ObstacleType.IronMace] = 45.0;
			m_obstacleChance[ObstacleType.Rock] = 5.0;
			break;
		case TowerModeDifficultyType.TimeAttack:
			setObstacleChance(getDifficultyForTimeAttackMode(currentFloor));
			break;
		case TowerModeDifficultyType.Endless:
			setObstacleChance(getDifficultyForEndlessMode(currentFloor));
			break;
		}
	}

	public void setRangedMonsterLists()
	{
		rangedMonsterList.Clear();
		rangedMonsterList.Add(EnemyManager.MonsterType.Goblin1);
		rangedMonsterList.Add(EnemyManager.MonsterType.Goblin2);
		rangedMonsterList.Add(EnemyManager.MonsterType.Goblin3);
	}

	private IEnumerator towerModeTimerUpdate()
	{
		m_currentTimer = 0f;
		float timerForRecordData = 0f;
		while (!isDead)
		{
			if (!GameManager.isPause)
			{
				TowerModeManager towerModeManager = this;
				towerModeManager.m_currentTimer = (float)towerModeManager.m_currentTimer + Time.deltaTime * GameManager.timeScale;
				timerForRecordData += Time.deltaTime * GameManager.timeScale;
				if (timerForRecordData >= 1f)
				{
					timerForRecordData -= 1f;
					currentPlayRecordDataList.Add(new TowerModeRecordData(curPlayingCharacter.currentLifeCount, currentFloor, (long)((float)m_currentTimer + 0.5f)));
				}
				UIWindowTowerMode.instance.setTimerText(m_currentTimer);
			}
			yield return null;
		}
	}

	private void createPool()
	{
		ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@ConfusionEffect"), 1, "@ConfusionEffect", true);
	}

	public void spawnBackground(bool isRecycleOldestObj)
	{
		string poolName = "@TowerModeMap";
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (lastCreatedFloor == getTotalFloor(currentDifficultyType))
		{
			flag = true;
		}
		else if (lastCreatedFloor == getTotalFloor(currentDifficultyType) - 1)
		{
			flag3 = true;
		}
		else
		{
			if (lastCreatedFloor > getTotalFloor(currentDifficultyType))
			{
				return;
			}
			if (lastCreatedFloor > 0 && lastCreatedFloor % 5 == 0)
			{
				flag2 = true;
			}
		}
		if (flag)
		{
			flag2 = false;
			flag3 = false;
			poolName = "@TowerModeMapApex";
		}
		else if (flag2)
		{
			poolName = "@TowerModeMiniBossMap";
		}
		else if (flag3)
		{
			poolName = "@TowerModeBossMap";
		}
		TowerModeMapObject component = ObjectPool.Spawn(poolName, new Vector2(0f, m_curOffsetY)).GetComponent<TowerModeMapObject>();
		component.initTowerModeMap((lastCreatedFloor % 2 != 0) ? MovingObject.Direction.Right : MovingObject.Direction.Left, lastCreatedFloor, flag, flag2, flag3);
		listTowerModeMapObjs.Add(component);
		float num = 0f;
		num = (flag2 ? m_intervalBetweenMiniBossMapObject : ((!flag3) ? m_intervalBetweenNormalMapObject : m_intervalBetweenBossMapObject));
		m_curOffsetY += num;
		if (isRecycleOldestObj && listTowerModeMapObjs.Count > 8)
		{
			TowerModeMapObject towerModeMapObject = listTowerModeMapObjs[0];
			listTowerModeMapObjs.Remove(towerModeMapObject);
			towerModeMapObject.recycleMap();
		}
		component.currentMiniBosses.Clear();
		component.currentBossObject = null;
		bool flag4 = false;
		if (flag3)
		{
			component.currentBossObject = spawnBossObject(component.getPoint(false).position + new Vector3(-2.2f * (float)((component.direction != 0) ? 1 : (-1)), 0f, 0f));
		}
		else if (flag2)
		{
			component.currentMiniBosses.Add(spawnMonsterObject(component.getPoint(false).position + new Vector3(-2.2f * (float)((component.direction != 0) ? 1 : (-1)), 0f, 0f), true));
		}
		else if (!flag)
		{
			double num2 = UnityEngine.Random.Range(0, 10000);
			num2 /= 100.0;
			if (num2 < getRangedMonsterSpawnChance(currentDifficultyType, lastCreatedFloor))
			{
				flag4 = true;
				spawnMonsterObject(getRandomRangedMonsterType(), component.getPoint(false).position + new Vector3((component.direction != 0) ? (-2) : 2, 0f, 0f), false, true);
			}
		}
		bool flag5 = false;
		switch (currentDifficultyType)
		{
		case TowerModeDifficultyType.TimeAttack:
			flag5 = getDifficultyForTimeAttackMode(currentFloor) >= TowerModeDifficultyType.Hell || !flag4;
			break;
		case TowerModeDifficultyType.Endless:
			flag5 = getDifficultyForEndlessMode(currentFloor) >= TowerModeDifficultyType.Hell || !flag4;
			break;
		}
		if (lastCreatedFloor == 3 || lastCreatedFloor == 4 || lastCreatedFloor == 5)
		{
			Vector2 pos = Vector2.zero;
			pos = ((component.direction == MovingObject.Direction.Right) ? new Vector2(UnityEngine.Random.Range(component.getPoint(true).position.x + 1.7f, component.getPoint(false).position.x - 1.7f), 0f) : new Vector2(UnityEngine.Random.Range(component.getPoint(false).position.x + 1.7f, component.getPoint(true).position.x - 1.7f), 0f));
			pos.y = component.getPoint(true).position.y;
			spawnMonsterObject(pos, false);
		}
		if (flag5 && !flag2 && !flag3 && !flag)
		{
			double num3 = UnityEngine.Random.Range(0, 10000);
			num3 /= 100.0;
			if (num3 < Singleton<TowerModeManager>.instance.getFlameSpawnChance(Singleton<TowerModeManager>.instance.currentDifficultyType))
			{
				Vector2 position = Vector2.zero;
				position = ((component.direction == MovingObject.Direction.Right) ? new Vector2(UnityEngine.Random.Range(component.getPoint(true).position.x + 1.7f, component.getPoint(false).position.x - 1.7f), 0f) : new Vector2(UnityEngine.Random.Range(component.getPoint(false).position.x + 1.7f, component.getPoint(true).position.x - 1.7f), 0f));
				position.y = component.getPoint(true).position.y;
				component.currentFlameObjects.Add(Singleton<TowerModeManager>.instance.spawnFlameObstacle(position));
			}
		}
		lastCreatedFloor++;
	}

	public void recycleAllBackground()
	{
		foreach (TowerModeMapObject listTowerModeMapObj in listTowerModeMapObjs)
		{
			listTowerModeMapObj.recycleMap();
		}
		listTowerModeMapObjs.Clear();
	}

	public void spawnCharacter()
	{
		TowerModeMapObject towerModeMapObject = null;
		for (int i = 0; i < listTowerModeMapObjs.Count; i++)
		{
			if (listTowerModeMapObjs[i].curFloor == 1)
			{
				towerModeMapObject = listTowerModeMapObjs[i];
				break;
			}
		}
		recycleCharacter();
		curPlayingCharacter = ObjectPool.Spawn("@TowerModeCharacter", towerModeMapObject.getPoint(true).position + new Vector3(0f, 0.5f)).GetComponent<TowerModeCharacterObject>();
		if (curPlayingCharacter != null)
		{
			curPlayingCharacter.initCharacterObject(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin);
		}
	}

	public void recycleCharacter()
	{
		if (curPlayingCharacter != null)
		{
			ObjectPool.Recycle(curPlayingCharacter.name, curPlayingCharacter.cachedGameObject);
		}
		curPlayingCharacter = null;
	}

	public void resultTowerMode(bool clear)
	{
		Singleton<AudioManager>.instance.stopBackgroundSound();
		StopAllCoroutines();
		UIWindowTowerModeResult.instance.openTowerModeResult(currentDifficultyType, clear);
	}

	public void pauseAllObstaclesAndNormalMonsters(bool pause)
	{
		isPauseObstacleAndMonster = pause;
	}

	public void recycleMonstersAndObstaclesBelongToTargetMap(TowerModeMapObject mapObject, bool isAlsoRecycleBoss = true)
	{
		List<TowerModeMonsterObject> list = new List<TowerModeMonsterObject>();
		List<TowerModeObstacle> list2 = new List<TowerModeObstacle>();
		for (int i = 0; i < Singleton<TowerModeManager>.instance.listTowerModeMonsterObjects.Count; i++)
		{
			TowerModeMonsterObject towerModeMonsterObject = Singleton<TowerModeManager>.instance.listTowerModeMonsterObjects[i];
			if ((!isAlsoRecycleBoss || (!towerModeMonsterObject.isMiniBoss && !towerModeMonsterObject.isBoss)) && towerModeMonsterObject.currentMapObject.curFloor == mapObject.curFloor)
			{
				ObjectPool.Spawn("fx_boss_blowup", towerModeMonsterObject.cachedSpriteAnimation.cachedTransform.position);
				towerModeMonsterObject.recycleMonster(false);
				list.Add(towerModeMonsterObject);
			}
		}
		for (int j = 0; j < Singleton<TowerModeManager>.instance.listTowerModeObstacles.Count; j++)
		{
			TowerModeObstacle towerModeObstacle = Singleton<TowerModeManager>.instance.listTowerModeObstacles[j];
			TowerModeMapObject stayingMapObject = Singleton<TowerModeManager>.instance.getStayingMapObject(towerModeObstacle.cachedTransform.position);
			if (stayingMapObject != null && stayingMapObject.curFloor == mapObject.curFloor)
			{
				ObjectPool.Spawn("fx_boss_blowup", towerModeObstacle.cachedTransform.position);
				towerModeObstacle.recycleObstacle(false);
				list2.Add(towerModeObstacle);
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			listTowerModeMonsterObjects.Remove(list[k]);
		}
		for (int l = 0; l < list2.Count; l++)
		{
			listTowerModeObstacles.Remove(list2[l]);
		}
	}

	public void checkBestRecord(TowerModeDifficultyType modeType)
	{
		if (!isConnectedToServer() || currentSeasonEndDateTime.Ticks <= UnbiasedTime.Instance.Now().Ticks || seasonData == null)
		{
			return;
		}
		currentFloor = Mathf.Min(currentFloor, 4000);
		switch (modeType)
		{
		case TowerModeDifficultyType.TimeAttack:
			if (getCalculatedRankingPoint(currentFloor, m_currentTimer) < getCalculatedRankingPoint(Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor, Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestClearTime))
			{
				Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor = currentFloor;
				Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestClearTime = m_currentTimer;
				Dictionary<FuntionParameterType, double> dictionary2 = new Dictionary<FuntionParameterType, double>();
				dictionary2.Add(FuntionParameterType.ClearFloor, (int)currentFloor);
				dictionary2.Add(FuntionParameterType.ClearTime, (float)m_currentTimer);
				CallAPI(TowerModeAPIType.SET_MY_RECORD, true, dictionary2, null, false);
			}
			break;
		case TowerModeDifficultyType.Endless:
			if (getCalculatedRankingPoint(currentFloor, m_currentTimer) < getCalculatedRankingPoint(Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor, Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestClearTime))
			{
				Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor = currentFloor;
				Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestClearTime = m_currentTimer;
				Dictionary<FuntionParameterType, double> dictionary = new Dictionary<FuntionParameterType, double>();
				dictionary.Add(FuntionParameterType.ClearFloor, (int)currentFloor);
				dictionary.Add(FuntionParameterType.ClearTime, (float)m_currentTimer);
				CallAPI(TowerModeAPIType.SET_MY_RECORD, false, dictionary, null, false);
			}
			break;
		}
		Singleton<DataManager>.instance.saveData();
	}

	public bool isFreeTicketOn()
	{
		bool result = false;
		if (Singleton<DataManager>.instance.currentGameData.towerModeFreeTicketEndTime > UnbiasedTime.Instance.Now().Ticks)
		{
			result = true;
		}
		return result;
	}

	public TowerModeMapObject getStayingMapObject(Vector2 position)
	{
		TowerModeMapObject result = null;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(position, Vector2.down, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
		if (raycastHit2D.transform != null)
		{
			result = raycastHit2D.transform.GetComponent<TowerModeMapObject>();
		}
		return result;
	}

	public TowerModeMapObject getNextMapObject(TowerModeMapObject mapObject)
	{
		TowerModeMapObject result = null;
		for (int i = 0; i < listTowerModeMapObjs.Count; i++)
		{
			if (listTowerModeMapObjs[i].curFloor == mapObject.curFloor + 1)
			{
				result = listTowerModeMapObjs[i];
				break;
			}
		}
		return result;
	}

	public TowerModeMapObject getPrevMapObject(TowerModeMapObject mapObject)
	{
		TowerModeMapObject result = null;
		for (int i = 0; i < listTowerModeMapObjs.Count; i++)
		{
			if (listTowerModeMapObjs[i].curFloor == mapObject.curFloor - 1)
			{
				result = listTowerModeMapObjs[i];
				break;
			}
		}
		return result;
	}

	public TowerModeCharacterObject getNearestCharacter(Vector2 pos, float range)
	{
		TowerModeCharacterObject result = null;
		if (curPlayingCharacter != null && (Vector2.Distance(pos, curPlayingCharacter.cachedTransform.position) <= range || Vector2.Distance(pos, curPlayingCharacter.cachedTransform.position + new Vector3(0f, 0.355f, 0f)) <= range))
		{
			result = curPlayingCharacter;
		}
		return result;
	}

	public List<TowerModeMonsterObject> getNearestEnemies(Vector2 startPosition, float range)
	{
		m_enemyList.Clear();
		float num = float.MaxValue;
		for (int i = 0; i < listTowerModeMonsterObjects.Count; i++)
		{
			if (!listTowerModeMonsterObjects[i].isDead && Mathf.Abs(listTowerModeMonsterObjects[i].cachedTransform.position.y - startPosition.y) <= 0.8f)
			{
				float num2 = range;
				if (listTowerModeMonsterObjects[i].isBoss)
				{
					num2 += 0.3f;
				}
				num = Vector2.Distance(startPosition, listTowerModeMonsterObjects[i].cachedTransform.position);
				if (num <= num2)
				{
					m_enemyList.Add(listTowerModeMonsterObjects[i]);
				}
			}
		}
		return m_enemyList;
	}

	public List<TowerModeEnemyProjectileObject> getNearestProjectiles(Vector2 startPosition, float range)
	{
		m_projectileList.Clear();
		float num = float.MaxValue;
		for (int i = 0; i < listTowerModeProjectileObjects.Count; i++)
		{
			if (Mathf.Abs(listTowerModeProjectileObjects[i].cachedTransform.position.y - startPosition.y) <= 0.8f)
			{
				num = Vector2.Distance(startPosition, listTowerModeProjectileObjects[i].cachedTransform.position);
				if (num <= range)
				{
					m_projectileList.Add(listTowerModeProjectileObjects[i]);
				}
			}
		}
		return m_projectileList;
	}

	public float getEnemySpawnTime(TowerModeDifficultyType difficulty)
	{
		float num = 0f;
		float min = 0f;
		float max = 0f;
		switch (difficulty)
		{
		case TowerModeDifficultyType.Easy:
			min = 1f;
			max = 3f;
			break;
		case TowerModeDifficultyType.Normal:
			min = 0.9f;
			max = 2.7f;
			break;
		case TowerModeDifficultyType.Hard:
			min = 0.8f;
			max = 2.3f;
			break;
		case TowerModeDifficultyType.VeryHard:
			min = 0.7f;
			max = 1.9f;
			break;
		case TowerModeDifficultyType.Hell:
			min = 0.6f;
			max = 1.7f;
			break;
		case TowerModeDifficultyType.TimeAttack:
			return getEnemySpawnTime(getDifficultyForTimeAttackMode(currentFloor));
		case TowerModeDifficultyType.Endless:
			return getEnemySpawnTime(getDifficultyForEndlessMode(currentFloor));
		}
		return UnityEngine.Random.Range(min, max);
	}

	public float getObstacleSpawnTime(TowerModeDifficultyType difficulty)
	{
		float num = 0f;
		float min = 1.8f;
		float max = 3f;
		return UnityEngine.Random.Range(min, max);
	}

	public float getMaxStunDuration()
	{
		return 1.5f;
	}

	public int getTotalFloor(TowerModeDifficultyType type)
	{
		int result = 0;
		switch (type)
		{
		case TowerModeDifficultyType.TimeAttack:
			result = ((!isBossCheatOn) ? 51 : 5);
			break;
		case TowerModeDifficultyType.Endless:
			result = int.MaxValue;
			break;
		}
		return result;
	}

	public long getTowerModeEnterHeartCoinPrice()
	{
		return 15L;
	}

	public ObstacleType getObstacleType()
	{
		ObstacleType result = ObstacleType.None;
		double num = UnityEngine.Random.Range(0, 10000);
		num /= 100.0;
		double num2 = 0.0;
		foreach (KeyValuePair<ObstacleType, double> item in m_obstacleChance)
		{
			num2 += item.Value;
			if (num < num2)
			{
				return item.Key;
			}
		}
		return result;
	}

	public float getObstacleSpeed(ObstacleType type)
	{
		float result = 0f;
		switch (type)
		{
		case ObstacleType.FireBall:
			result = 7f;
			break;
		case ObstacleType.IronMace:
			result = 2.3f;
			break;
		case ObstacleType.Rock:
			result = 4f;
			break;
		}
		return result;
	}

	public int getMiniBossHP(TowerModeDifficultyType difficulty)
	{
		int result = 0;
		switch (difficulty)
		{
		case TowerModeDifficultyType.Easy:
			result = UnityEngine.Random.Range(3, 7);
			break;
		case TowerModeDifficultyType.Normal:
			result = UnityEngine.Random.Range(4, 8);
			break;
		case TowerModeDifficultyType.Hard:
			result = UnityEngine.Random.Range(6, 10);
			break;
		case TowerModeDifficultyType.VeryHard:
			result = UnityEngine.Random.Range(9, 13);
			break;
		case TowerModeDifficultyType.Hell:
			result = UnityEngine.Random.Range(10, 16);
			break;
		case TowerModeDifficultyType.TimeAttack:
			result = getMiniBossHP(getDifficultyForTimeAttackMode(currentFloor));
			break;
		case TowerModeDifficultyType.Endless:
			result = getMiniBossHP(getDifficultyForEndlessMode(currentFloor));
			break;
		}
		return result;
	}

	public float getMiniBossSpeed(TowerModeDifficultyType difficulty)
	{
		float result = 0f;
		switch (difficulty)
		{
		case TowerModeDifficultyType.Easy:
			result = UnityEngine.Random.Range(1.5f, 4f);
			break;
		case TowerModeDifficultyType.Normal:
			result = UnityEngine.Random.Range(2.5f, 5f);
			break;
		case TowerModeDifficultyType.Hard:
			result = UnityEngine.Random.Range(4f, 7f);
			break;
		case TowerModeDifficultyType.VeryHard:
			result = UnityEngine.Random.Range(5f, 8f);
			break;
		case TowerModeDifficultyType.Hell:
			result = UnityEngine.Random.Range(5f, 8f);
			break;
		case TowerModeDifficultyType.TimeAttack:
			result = getMiniBossSpeed(getDifficultyForTimeAttackMode(currentFloor));
			break;
		case TowerModeDifficultyType.Endless:
			result = getMiniBossSpeed(getDifficultyForEndlessMode(currentFloor));
			break;
		}
		return result;
	}

	public double getSpeedMonsterSpawnChance(TowerModeDifficultyType difficulty)
	{
		double result = 0.0;
		switch (difficulty)
		{
		case TowerModeDifficultyType.Easy:
			result = 5.0;
			break;
		case TowerModeDifficultyType.Normal:
			result = 6.5;
			break;
		case TowerModeDifficultyType.Hard:
			result = 8.0;
			break;
		case TowerModeDifficultyType.VeryHard:
			result = 9.0;
			break;
		case TowerModeDifficultyType.Hell:
			result = 10.0;
			break;
		case TowerModeDifficultyType.TimeAttack:
			result = getSpeedMonsterSpawnChance(getDifficultyForTimeAttackMode(currentFloor));
			break;
		case TowerModeDifficultyType.Endless:
			result = getSpeedMonsterSpawnChance(getDifficultyForEndlessMode(currentFloor));
			break;
		}
		return result;
	}

	public double getRangedMonsterSpawnChance(TowerModeDifficultyType difficulty, int floor)
	{
		double result = 0.0;
		switch (difficulty)
		{
		case TowerModeDifficultyType.Easy:
			result = 0.0;
			break;
		case TowerModeDifficultyType.Normal:
			result = 0.0;
			break;
		case TowerModeDifficultyType.Hard:
			result = 0.0;
			break;
		case TowerModeDifficultyType.VeryHard:
			result = 35.0;
			break;
		case TowerModeDifficultyType.Hell:
			result = 60.0;
			break;
		case TowerModeDifficultyType.TimeAttack:
			result = getRangedMonsterSpawnChance(getDifficultyForTimeAttackMode(floor), floor);
			break;
		case TowerModeDifficultyType.Endless:
			result = getRangedMonsterSpawnChance(getDifficultyForEndlessMode(floor), floor);
			break;
		}
		return result;
	}

	public EnemyManager.MonsterType getRandomRangedMonsterType()
	{
		return rangedMonsterList[UnityEngine.Random.Range(0, rangedMonsterList.Count)];
	}

	public double getFlameSpawnChance(TowerModeDifficultyType difficulty)
	{
		double result = 0.0;
		switch (difficulty)
		{
		case TowerModeDifficultyType.Easy:
			result = 0.0;
			break;
		case TowerModeDifficultyType.Normal:
			result = 30.0;
			break;
		case TowerModeDifficultyType.Hard:
			result = 40.0;
			break;
		case TowerModeDifficultyType.VeryHard:
			result = 50.0;
			break;
		case TowerModeDifficultyType.Hell:
			result = 80.0;
			break;
		case TowerModeDifficultyType.TimeAttack:
			result = getFlameSpawnChance(getDifficultyForTimeAttackMode(currentFloor));
			break;
		case TowerModeDifficultyType.Endless:
			result = getFlameSpawnChance(getDifficultyForEndlessMode(currentFloor));
			break;
		}
		return result;
	}

	public int getBossMaxHP()
	{
		return 20;
	}

	public float getBossMoveSpeed()
	{
		return UnityEngine.Random.Range(5f, 8.5f);
	}

	public float getRangedMonsterAttackSpeed(TowerModeDifficultyType difficulty)
	{
		float result = 3f;
		switch (difficulty)
		{
		case TowerModeDifficultyType.Hard:
			result = 2.5f;
			break;
		case TowerModeDifficultyType.VeryHard:
			result = 2f;
			break;
		case TowerModeDifficultyType.Hell:
			result = 1.75f;
			break;
		case TowerModeDifficultyType.TimeAttack:
			result = getRangedMonsterAttackSpeed(getDifficultyForTimeAttackMode(currentFloor));
			break;
		case TowerModeDifficultyType.Endless:
			result = getRangedMonsterAttackSpeed(getDifficultyForEndlessMode(currentFloor));
			break;
		}
		return result;
	}

	public TowerModeDifficultyType getDifficultyForTimeAttackMode(int floor)
	{
		TowerModeDifficultyType towerModeDifficultyType = TowerModeDifficultyType.Easy;
		if (floor <= 10)
		{
			return TowerModeDifficultyType.Easy;
		}
		if (floor <= 20)
		{
			return TowerModeDifficultyType.Normal;
		}
		if (floor <= 30)
		{
			return TowerModeDifficultyType.Hard;
		}
		if (floor <= 40)
		{
			return TowerModeDifficultyType.VeryHard;
		}
		return TowerModeDifficultyType.Hell;
	}

	public TowerModeDifficultyType getDifficultyForEndlessMode(int floor)
	{
		TowerModeDifficultyType towerModeDifficultyType = TowerModeDifficultyType.Easy;
		if (floor <= 20)
		{
			return TowerModeDifficultyType.Easy;
		}
		if (floor <= 40)
		{
			return TowerModeDifficultyType.Normal;
		}
		if (floor <= 60)
		{
			return TowerModeDifficultyType.Hard;
		}
		if (floor <= 80)
		{
			return TowerModeDifficultyType.VeryHard;
		}
		return TowerModeDifficultyType.Hell;
	}

	public float getCalculatedRankingPoint(int floor, float clearTime)
	{
		float num = 0f;
		return (4000f - (float)floor) * Mathf.Pow(10f, 6f) + clearTime;
	}

	public ObscuredBool isTrulyRecord(ObscuredInt currentFloor, ObscuredFloat currentClearTime, List<TowerModeRecordData> recordData)
	{
		if ((int)(float)currentClearTime != 0 && recordData.Count - 1 <= 0)
		{
			return false;
		}
		ObscuredBool result = true;
		if ((int)(float)currentClearTime != recordData.Count - 1)
		{
			result = false;
		}
		else if (recordData.Count > 1)
		{
			ObscuredInt value = recordData[0].currentFloor;
			int num = 1;
			while (true)
			{
				if (num < recordData.Count)
				{
					if ((int)recordData[num].currentFloor - (int)value >= 3)
					{
						result = false;
						break;
					}
					value = recordData[num].currentFloor;
					num++;
					continue;
				}
				ObscuredInt currentLifeCount = recordData[0].currentLifeCount;
				int num2 = 1;
				while (true)
				{
					if (num2 < recordData.Count)
					{
						if ((int)currentLifeCount < (int)recordData[num2].currentLifeCount)
						{
							result = false;
							break;
						}
						currentLifeCount = recordData[num2].currentLifeCount;
						num2++;
						continue;
					}
					ObscuredFloat currentPlayTime = recordData[0].currentPlayTime;
					for (int i = 1; i < recordData.Count; i++)
					{
						if ((float)recordData[i].currentPlayTime - (float)currentPlayTime < 1f || (float)recordData[i].currentPlayTime - (float)currentPlayTime >= 3f)
						{
							result = false;
							break;
						}
						currentPlayTime = recordData[i].currentPlayTime;
					}
					break;
				}
				break;
			}
		}
		else
		{
			result = false;
		}
		if ((bool)isAbuser)
		{
			result = false;
		}
		return result;
	}

	public TowerModeMonsterObject spawnMonsterObject(Vector2 pos, bool isMiniBoss, bool isRangedMonster = false, bool spawnedFromBoss = false)
	{
		List<EnemyManager.MonsterType> list = Singleton<EnemyManager>.instance.normalMonsterList[10];
		EnemyManager.MonsterType monsterType = list[UnityEngine.Random.Range(0, list.Count)];
		while (monsterType == EnemyManager.MonsterType.Flamespirit1 || monsterType == getRandomRangedMonsterType())
		{
			monsterType = list[UnityEngine.Random.Range(0, list.Count)];
		}
		return spawnMonsterObject(monsterType, pos, isMiniBoss, isRangedMonster, spawnedFromBoss);
	}

	public TowerModeMonsterObject spawnMonsterObject(EnemyManager.MonsterType monsterType, Vector2 pos, bool isMiniBoss, bool isRangedMonster = false, bool spawnedFromBoss = false)
	{
		string poolName = "@TowerModeMonsterObject";
		TowerModeMonsterObject component = ObjectPool.Spawn(poolName, pos).GetComponent<TowerModeMonsterObject>();
		listTowerModeMonsterObjects.Add(component);
		component.initMonster(monsterType, isMiniBoss, isRangedMonster, spawnedFromBoss);
		return component;
	}

	public TowerModeBossObject spawnBossObject(Vector2 position)
	{
		string poolName = "@TowerModeBossObject";
		TowerModeBossObject component = ObjectPool.Spawn(poolName, position).GetComponent<TowerModeBossObject>();
		listTowerModeMonsterObjects.Add(component);
		component.initBossMonster();
		return component;
	}

	public TowerModeEnemyProjectileObject spawnProjectile(Vector2 spawnPosition, MovingObject.Direction direction, EnemyManager.MonsterType casterType)
	{
		TowerModeEnemyProjectileObject component = ObjectPool.Spawn("@TowerModeProjectile", spawnPosition).GetComponent<TowerModeEnemyProjectileObject>();
		component.initProjectile(spawnPosition + new Vector2((direction != 0) ? 13 : (-13), 0f), casterType);
		listTowerModeProjectileObjects.Add(component);
		return component;
	}

	public void recycleAllMonsterObjects()
	{
		for (int i = 0; i < listTowerModeMonsterObjects.Count; i++)
		{
			ObjectPool.Recycle(listTowerModeMonsterObjects[i].name, listTowerModeMonsterObjects[i].cachedGameObject);
		}
		listTowerModeMonsterObjects.Clear();
		recycleAllProjectiles();
	}

	public void recycleAllProjectiles()
	{
		if (listTowerModeProjectileObjects != null)
		{
			for (int i = 0; i < listTowerModeProjectileObjects.Count; i++)
			{
				listTowerModeProjectileObjects[i].recycleProjectile(false);
			}
			listTowerModeProjectileObjects.Clear();
		}
	}

	private IEnumerator MonsterObjectSpawnUpdate()
	{
		float targetSpawnMonsterObjectTime = getEnemySpawnTime(currentDifficultyType);
		while (!Singleton<TowerModeManager>.instance.isDead)
		{
			if (!GameManager.isPause && !isPauseObstacleAndMonster)
			{
				monsterObjectSpawnTimer += Time.deltaTime * GameManager.timeScale;
				if (monsterObjectSpawnTimer > targetSpawnMonsterObjectTime)
				{
					monsterObjectSpawnTimer = 0f;
					Vector2 spawnPosition = listTowerModeMapObjs[listTowerModeMapObjs.Count - 1].getPoint(false).position;
					spawnMonsterObject(spawnPosition, false);
					targetSpawnMonsterObjectTime = getEnemySpawnTime(currentDifficultyType);
				}
			}
			yield return null;
		}
	}

	public void spawnObstacle(Vector2 pos)
	{
		TowerModeObstacle component = ObjectPool.Spawn("@TowerModeObstacle", pos).GetComponent<TowerModeObstacle>();
		listTowerModeObstacles.Add(component);
		ObstacleType obstacleType = getObstacleType();
		component.initObstacle(obstacleType, getObstacleSpeed(obstacleType), m_obstacleRange);
	}

	public TowerModeFlameObject spawnFlameObstacle(Vector2 position)
	{
		TowerModeFlameObject component = ObjectPool.Spawn("@TowerModeFlame", position).GetComponent<TowerModeFlameObject>();
		component.initFlame();
		listTowerModeFlames.Add(component);
		return component;
	}

	public void recycleAllObstacles()
	{
		for (int i = 0; i < listTowerModeObstacles.Count; i++)
		{
			listTowerModeObstacles[i].recycleObstacle(false);
		}
		listTowerModeObstacles.Clear();
		recycleAllFlames();
	}

	public void recycleAllFlames()
	{
		for (int i = 0; i < listTowerModeFlames.Count; i++)
		{
			listTowerModeFlames[i].recycleFlame(false);
		}
		listTowerModeFlames.Clear();
	}

	private IEnumerator obstacleSpawnUpdate()
	{
		float targetSpawnObstacleTime = getObstacleSpawnTime(currentDifficultyType);
		float targetSpawnXPos = UnityEngine.Random.Range(m_obstacleSpawnMinXPos, m_obstacleSpawnMaxXPos);
		while (!Singleton<TowerModeManager>.instance.isDead)
		{
			if (!GameManager.isPause && !isPauseObstacleAndMonster)
			{
				obstacleSpawnTimer += Time.deltaTime * GameManager.timeScale;
				if (obstacleSpawnTimer > targetSpawnObstacleTime)
				{
					obstacleSpawnTimer = 0f;
					spawnObstacle(new Vector2(targetSpawnXPos, CameraFollow.instance.cachedTransform.position.y + 15f));
					targetSpawnXPos = UnityEngine.Random.Range(m_obstacleSpawnMinXPos, m_obstacleSpawnMaxXPos);
					targetSpawnObstacleTime = getObstacleSpawnTime(currentDifficultyType);
				}
			}
			yield return null;
		}
	}

	private ParameterType[] getParameterTypeArray(TowerModeAPIType apiType)
	{
		List<ParameterType> list = new List<ParameterType>();
		switch (apiType)
		{
		case TowerModeAPIType.GET_MY_RANKING:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.mode);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.season_id);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case TowerModeAPIType.GET_ALL_RANKING:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.mode);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.season_id);
			list.Add(ParameterType.start);
			list.Add(ParameterType.finish);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case TowerModeAPIType.GET_HONOR_RANKING:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.mode);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		case TowerModeAPIType.SET_MY_RECORD:
			list.Add(ParameterType.client_id);
			list.Add(ParameterType.mode);
			list.Add(ParameterType.uuid);
			list.Add(ParameterType.postbox_id);
			list.Add(ParameterType.nickname);
			list.Add(ParameterType.clear_time);
			list.Add(ParameterType.clear_floor);
			list.Add(ParameterType.game_data);
			list.Add(ParameterType.season_id);
			list.Add(ParameterType.version);
			list.Add(ParameterType.block);
			list.Add(ParameterType.ts);
			list.Add(ParameterType.hash);
			break;
		}
		return list.ToArray();
	}

	private void Awake()
	{
		m_apiDataDictionary.Add(TowerModeAPIType.GET_SEASON_INFORMATION, new APIData(TowerModeAPIType.GET_SEASON_INFORMATION, APIMethodType.Get, "season"));
		m_apiDataDictionary.Add(TowerModeAPIType.GET_MY_RANKING, new APIData(TowerModeAPIType.GET_MY_RANKING, APIMethodType.Post, "me"));
		m_apiDataDictionary.Add(TowerModeAPIType.GET_ALL_RANKING, new APIData(TowerModeAPIType.GET_ALL_RANKING, APIMethodType.Post, "rank"));
		m_apiDataDictionary.Add(TowerModeAPIType.GET_HONOR_RANKING, new APIData(TowerModeAPIType.GET_HONOR_RANKING, APIMethodType.Post, "honor"));
		m_apiDataDictionary.Add(TowerModeAPIType.SET_MY_RECORD, new APIData(TowerModeAPIType.SET_MY_RECORD, APIMethodType.Post, "record"));
		if (Singleton<NanooAPIManager>.instance.towerModeSeasonData != null)
		{
			m_seasonData = Singleton<NanooAPIManager>.instance.towerModeSeasonData;
		}
	}

	public static bool isConnectedToServer()
	{
		bool result = false;
		if (Util.isInternetConnection() && !string.IsNullOrEmpty(Singleton<NanooAPIManager>.instance.UserID) && Social.localUser.authenticated && !string.IsNullOrEmpty(Social.localUser.userName))
		{
			result = true;
		}
		return result;
	}

	public void CallAPI(TowerModeAPIType apiType, Action<bool> endAction = null)
	{
		CallAPI(apiType, true, null, endAction);
	}

	public void CallAPI(TowerModeAPIType apiType, bool isTimeAttackMode, Dictionary<FuntionParameterType, double> parameterDictionary = null, Action<bool> doneAction = null, bool withLoadingUI = true)
	{
		if (!isConnectedToServer())
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
		for (int i = 0; i < 4; i++)
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
					text2 = text3 + ((j != 0) ? "&" : "?") + aPIData.parameterTypeList[j].ToString() + "=" + getCalculatedParameter(aPIData.parameterTypeList[j], (long)parameterDictionary[FuntionParameterType.RankingStart], (long)parameterDictionary[FuntionParameterType.RankingEnd], (float)parameterDictionary[FuntionParameterType.ClearTime], (int)parameterDictionary[FuntionParameterType.ClearFloor], isTimeAttackMode);
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
					string calculatedParameter = getCalculatedParameter(aPIData.parameterTypeList[k], (long)parameterDictionary[FuntionParameterType.RankingStart], (long)parameterDictionary[FuntionParameterType.RankingEnd], (float)parameterDictionary[FuntionParameterType.ClearTime], (int)parameterDictionary[FuntionParameterType.ClearFloor], isTimeAttackMode);
					if (aPIData.parameterTypeList[k] != ParameterType.hash)
					{
						wWWForm.AddField(aPIData.parameterTypeList[k].ToString(), calculatedParameter);
						if (aPIData.parameterTypeList[k] != ParameterType.game_data && aPIData.parameterTypeList[k] != ParameterType.version && aPIData.parameterTypeList[k] != ParameterType.block)
						{
							text4 += calculatedParameter;
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
		StartCoroutine(waitForRequest(apiType, isTimeAttackMode, wWW, doneAction, withLoadingUI));
	}

	private void setMyRankingDate(UserInformationData data, bool isTimeAttackMode)
	{
		if (isTimeAttackMode)
		{
			int result = 0;
			int.TryParse(data.clear_floor, out result);
			Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor = result;
			float result2 = 0f;
			if (float.TryParse(data.clear_time, out result2))
			{
				result2 /= 1000f;
			}
			Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestClearTime = result2;
			m_myTimeAttackRankingData.player = data;
		}
		else
		{
			int result3 = 0;
			int.TryParse(data.clear_floor, out result3);
			Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor = result3;
			float result4 = 0f;
			if (float.TryParse(data.clear_time, out result4))
			{
				result4 /= 1000f;
			}
			Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestClearTime = result4;
			m_myEndlessRankingData.player = data;
		}
	}

	private IEnumerator waitForRequest(TowerModeAPIType apiType, bool isTimeAttackMode, WWW www, Action<bool> doneAction, bool withLoadingUI)
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
		string response = www.text;
		bool isSuccess = false;
		switch (apiType)
		{
		case TowerModeAPIType.GET_SEASON_INFORMATION:
			isSuccess = false;
			if (!string.IsNullOrEmpty(response))
			{
				SeasonResponseData seasonData = JsonUtility.FromJson<SeasonResponseData>(response);
				if (seasonData != null)
				{
					Singleton<NanooAPIManager>.instance.towerModeSeasonData = seasonData;
					m_seasonData = seasonData;
					isSuccess = true;
				}
			}
			break;
		case TowerModeAPIType.GET_MY_RANKING:
			if (isTimeAttackMode)
			{
				m_myTimeAttackRankingData = new MyRankResponseData();
				m_myTimeAttackRankingData = JsonUtility.FromJson<MyRankResponseData>(response);
				if (m_myTimeAttackRankingData != null)
				{
					if (!isStatusSuccess(m_myTimeAttackRankingData.status))
					{
						m_myTimeAttackRankingData = null;
						break;
					}
					isSuccess = true;
					setMyRankingDate(m_myTimeAttackRankingData.player, isTimeAttackMode);
				}
				break;
			}
			m_myEndlessRankingData = new MyRankResponseData();
			m_myEndlessRankingData = JsonUtility.FromJson<MyRankResponseData>(response);
			if (m_myEndlessRankingData != null)
			{
				if (!isStatusSuccess(m_myEndlessRankingData.status))
				{
					m_myEndlessRankingData = null;
					break;
				}
				isSuccess = true;
				setMyRankingDate(m_myEndlessRankingData.player, isTimeAttackMode);
			}
			break;
		case TowerModeAPIType.GET_ALL_RANKING:
		{
			SeasonRankingResponseData totalSeasonRankingResponseData2 = new SeasonRankingResponseData();
			totalSeasonRankingResponseData2 = JsonUtility.FromJson<SeasonRankingResponseData>(response);
			if (totalSeasonRankingResponseData2 == null || !isStatusSuccess(totalSeasonRankingResponseData2.status))
			{
				break;
			}
			isSuccess = true;
			if (isTimeAttackMode)
			{
				if (m_totalTimeAttackSeasonRankingDataList == null)
				{
					m_totalTimeAttackSeasonRankingDataList = new List<UserInformationData>();
				}
				for (int j = 0; j < totalSeasonRankingResponseData2.player.Count; j++)
				{
					m_totalTimeAttackSeasonRankingDataList.Add(totalSeasonRankingResponseData2.player[j]);
				}
				if (totalSeasonRankingResponseData2.me != null)
				{
					setMyRankingDate(totalSeasonRankingResponseData2.me, isTimeAttackMode);
				}
			}
			else
			{
				if (m_totalEndlessSeasonRankingDataList == null)
				{
					m_totalEndlessSeasonRankingDataList = new List<UserInformationData>();
				}
				for (int i = 0; i < totalSeasonRankingResponseData2.player.Count; i++)
				{
					m_totalEndlessSeasonRankingDataList.Add(totalSeasonRankingResponseData2.player[i]);
				}
				if (totalSeasonRankingResponseData2.me != null)
				{
					setMyRankingDate(totalSeasonRankingResponseData2.me, isTimeAttackMode);
				}
			}
			break;
		}
		case TowerModeAPIType.GET_HONOR_RANKING:
			if (isTimeAttackMode)
			{
				m_totalTimeAttackHonorRankingData = new HonorRankingResponseData();
				m_totalTimeAttackHonorRankingData = JsonUtility.FromJson<HonorRankingResponseData>(response);
				isSuccess = isStatusSuccess(m_totalTimeAttackHonorRankingData.status);
			}
			else
			{
				m_totalEndlessHonorRankingData = new HonorRankingResponseData();
				m_totalEndlessHonorRankingData = JsonUtility.FromJson<HonorRankingResponseData>(response);
				isSuccess = isStatusSuccess(m_totalEndlessHonorRankingData.status);
			}
			break;
		case TowerModeAPIType.SET_MY_RECORD:
			if (isTimeAttackMode)
			{
				m_myTimeAttackRankingData = new MyRankResponseData();
				m_myTimeAttackRankingData = JsonUtility.FromJson<MyRankResponseData>(response);
				if (m_myTimeAttackRankingData != null)
				{
					if (!isStatusSuccess(m_myTimeAttackRankingData.status))
					{
						m_myTimeAttackRankingData = null;
						break;
					}
					isSuccess = true;
					setMyRankingDate(m_myTimeAttackRankingData.player, isTimeAttackMode);
				}
				break;
			}
			m_myEndlessRankingData = new MyRankResponseData();
			m_myEndlessRankingData = JsonUtility.FromJson<MyRankResponseData>(response);
			if (m_myEndlessRankingData != null)
			{
				if (!isStatusSuccess(m_myEndlessRankingData.status))
				{
					m_myEndlessRankingData = null;
					break;
				}
				isSuccess = true;
				setMyRankingDate(m_myEndlessRankingData.player, isTimeAttackMode);
			}
			break;
		}
		if (doneAction != null)
		{
			doneAction(isSuccess);
		}
	}

	private IEnumerator timeOutCheckUpdate(TowerModeAPIType apiType)
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
		if (apiType == TowerModeAPIType.GET_SEASON_INFORMATION)
		{
			UIWindowDialog.openDescription("DEMON_TOWER_TRY_AGAIN", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
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

	private string getCalculatedParameter(ParameterType parameterType, long minRanking, long maxRanking, float clearTime, int clearFloor, bool isTimeAttackMode)
	{
		string text = string.Empty;
		switch (parameterType)
		{
		case ParameterType.client_id:
			text = NanooAPIManager.getNanooGameID();
			break;
		case ParameterType.mode:
			text = ((!isTimeAttackMode) ? "infinity" : "timeattack");
			break;
		case ParameterType.uuid:
			text = Singleton<NanooAPIManager>.instance.UserID;
			break;
		case ParameterType.postbox_id:
			text = Singleton<NanooAPIManager>.instance.PostBoxID;
			break;
		case ParameterType.nickname:
			text = ((!Social.localUser.authenticated) ? "N/A" : Social.localUser.userName);
			break;
		case ParameterType.clear_time:
			text = ((int)(clearTime * 1000f)).ToString();
			break;
		case ParameterType.clear_floor:
			text = clearFloor.ToString();
			break;
		case ParameterType.game_data:
			text = (int)Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin + "," + (int)Singleton<DataManager>.instance.currentGameData.equippedPriestSkin + "," + (int)Singleton<DataManager>.instance.currentGameData.equippedArcherSkin;
			break;
		case ParameterType.season_id:
			text = ((m_seasonData == null) ? "N/A" : m_seasonData.id);
			break;
		case ParameterType.version:
			text = GlobalSetting.s_bundleVersion;
			break;
		case ParameterType.block:
			text = ((!isTrulyRecord(currentFloor, m_currentTimer, currentPlayRecordDataList)) ? "1" : "0");
			if (text.Equals("0") && (bool)Singleton<PVPManager>.instance.isAbuser())
			{
				text = "1";
			}
			break;
		case ParameterType.start:
			text = minRanking.ToString();
			break;
		case ParameterType.finish:
			text = maxRanking.ToString();
			break;
		case ParameterType.ts:
			text = Util.UnixTimestampFromDateTime(UnbiasedTime.Instance.Now()).ToString();
			break;
		}
		return text;
	}

	private void calculateTicketCount()
	{
		TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.Now().Ticks - Singleton<DataManager>.instance.currentGameData.lastTowerModeStartTime);
		double num = 60.0;
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
				Singleton<DataManager>.instance.currentGameData.lastTowerModeStartTime = UnbiasedTime.Instance.Now().Ticks;
			}
			Singleton<DataManager>.instance.currentGameData.towerModeTicketCount += value;
			Singleton<DataManager>.instance.currentGameData.towerModeTicketCount = Mathf.Min(Singleton<DataManager>.instance.currentGameData.towerModeTicketCount, maxTicketCount);
			Singleton<DataManager>.instance.saveData();
			displayTicket();
		}
	}

	public void decreaseTicket(int value)
	{
		if (Singleton<DataManager>.instance.currentGameData.towerModeTicketCount >= maxTicketCount)
		{
			Singleton<DataManager>.instance.currentGameData.lastTowerModeStartTime = UnbiasedTime.Instance.Now().Ticks;
		}
		Singleton<DataManager>.instance.currentGameData.towerModeTicketCount = Math.Max(Singleton<DataManager>.instance.currentGameData.towerModeTicketCount - value, 0);
		Singleton<DataManager>.instance.saveData();
		int num = Mathf.Max(Singleton<DataManager>.instance.currentGameData.towerModeTicketCount, 0);
		DateTime value2 = new DateTime(Singleton<DataManager>.instance.currentGameData.lastTowerModeStartTime);
		double num2 = 60.0;
		double num3 = num2 * (double)(maxTicketCount - num);
		if (num < maxTicketCount && UnbiasedTime.Instance.Now() < value2.AddMinutes(num3))
		{
			double num4 = num3 - UnbiasedTime.Instance.Now().Subtract(value2).TotalSeconds;
		}
		displayTicket();
	}

	public void displayTicket()
	{
		if (UIWindowSelectTowerModeDifficulty.instance.isOpen)
		{
			for (int i = 0; i < ticketCountTexts.Length; i++)
			{
				ticketCountTexts[i].text = Singleton<DataManager>.instance.currentGameData.towerModeTicketCount.ToString("N0");
			}
			UIWindowSelectTowerModeDifficulty.instance.refreshTicketText();
		}
	}

	private void Update()
	{
		if (Singleton<DataManager>.instance.currentGameData.towerModeTicketCount < maxTicketCount)
		{
			TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.Now().Ticks - Singleton<DataManager>.instance.currentGameData.lastTowerModeStartTime);
			if (timeSpan.TotalMinutes > 0.0 && timeSpan.TotalMinutes >= 60.0)
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
}
