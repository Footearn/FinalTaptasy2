using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElopeModeManager : Singleton<ElopeModeManager>
{
	[Serializable]
	public struct BackgroundSpriteData
	{
		public Sprite mainBackgroundSprite;

		public Sprite middleBackgroundSprite;

		public Sprite frontBackgroundSprite;
	}

	public enum DaemonKingSkillType
	{
		None = -1,
		DaemonKingPower,
		LoveLovePower,
		ReinforcementSword,
		HandsomeGuyDaemonKing,
		SuperHandsomeGuyDaemonKing,
		SpeedGuyDaemonKing,
		SuperSpeedGuyDaemonKing,
		LoveTimeMachine,
		Length
	}

	public enum DaemonKingUpgradeResourceType
	{
		None = -1,
		Heart,
		Ruby,
		Ads,
		Cash,
		TimeMachine
	}

	public struct PrincessUpgradeResourceData
	{
		public DropItemManager.DropItemType targetResourceType;

		public long value;
	}

	public enum ElopeShopItemType
	{
		None = -1,
		Gold,
		TimerSilverFinger,
		TimerGoldFinger,
		TimerAutoOpenTreasureChest,
		TimerDoubleSpeed,
		Colleague,
		ColleagueSkin,
		CharacterSkin,
		Weapon,
		TreasureKey,
		Treasure
	}

	public enum TimeCheckType
	{
		SpawnEnemy,
		Length
	}

	[Serializable]
	public struct ElopeItemIconSpriteData
	{
		public ElopeShopItemType itemType;

		public Sprite iconSprite;
	}

	public static bool isLoadedElopeMode;

	public List<ElopeBackgroundObject> currentElopeBackgroundObject = new List<ElopeBackgroundObject>();

	public List<Sprite> daemonKingFaceSpriteList;

	public Sprite[] daemonKingSwordSprites;

	public ElopeModeDaemonKing currentElopeModeDaemonKing;

	public static long daemonKingDoublePowerLevelUnit = 25L;

	public static long princessDoubleHeartLevelUnit = 25L;

	public static float intervalBetweenEnemy = 1.8f;

	public static float attackSpeed = 1f;

	public static float princessProductionMultiplyValue;

	public float princessProductionMultiplyValueFromHandsomeGuy;

	public float daemonKingAttackSpeedMuliply;

	public Transform heartCoinFlyTargetTransform;

	public List<ElopeEnemyObject> currentElopeEnemyList = new List<ElopeEnemyObject>();

	public TextMeshProUGUI[] heartTotalValueTexts;

	public Text[] heartCoinTotalValueTexts;

	public Text[] resource1ValueTexts;

	public Text[] resource2ValueTexts;

	public Text[] resource3ValueTexts;

	public Text[] resource4ValueTexts;

	public Transform goldFlyTargetTransform;

	public Transform rubyFlyTargetTransform;

	public Transform daemonCoinFlyTargetTransform;

	public Sprite closedElopeModeButtonSprite;

	public Sprite openedElopeModeButtonSprite;

	public ElopeCartObject currentElopeCartObject;

	public Sprite heartIconSprite;

	public Sprite rubyIconSprite;

	public BackgroundSpriteData[] backgroundSpriteData;

	private float m_currentEnemySpawnXPosition;

	private float[] m_princessMaxProductionTimeArray = new float[25]
	{
		1f,
		2f,
		5f,
		10f,
		30f,
		60f,
		120f,
		180f,
		240f,
		300f,
		600f,
		900f,
		1800f,
		3600f,
		5400f,
		7200f,
		9000f,
		10800f,
		12600f,
		14400f,
		16200f,
		18000f,
		20000f,
		22000f,
		24000f
	};

	private float m_timerForPrincessEffect;

	private float m_princessEffectSoundMaxTime;

	private bool m_isUsingAnySkill;

	public List<Sprite> daemonKingSkillSprites;

	public List<Sprite> princessResourceSprites;

	public static long refreshPrice = 30L;

	public static long intervalRefreshItemHour = 3L;

	public List<ElopeItemIconSpriteData> iconSpriteList;

	private Dictionary<ElopeShopItemType, double> m_elopeShopItemRandomValueDictionary = new Dictionary<ElopeShopItemType, double>
	{
		{
			ElopeShopItemType.Gold,
			10.0
		},
		{
			ElopeShopItemType.TimerSilverFinger,
			7.0
		},
		{
			ElopeShopItemType.TimerGoldFinger,
			7.0
		},
		{
			ElopeShopItemType.TimerAutoOpenTreasureChest,
			6.0
		},
		{
			ElopeShopItemType.TimerDoubleSpeed,
			6.0
		},
		{
			ElopeShopItemType.Colleague,
			4.0
		},
		{
			ElopeShopItemType.ColleagueSkin,
			0.0
		},
		{
			ElopeShopItemType.CharacterSkin,
			7.0
		},
		{
			ElopeShopItemType.Weapon,
			7.0
		},
		{
			ElopeShopItemType.TreasureKey,
			11.0
		},
		{
			ElopeShopItemType.Treasure,
			35.0
		}
	};

	private MersenneTwister m_randomForTreasureTier = new MersenneTwister();

	private MersenneTwister m_randomForPremiumTreasure = new MersenneTwister();

	private MersenneTwister m_randomForTreasureType = new MersenneTwister();

	private float m_currentTimerForSimulationHuntDaemonKing;

	private int m_attackCount;

	private void Start()
	{
		resetEnemyHealthData();
		refreshPrincessProductionMultiplyValue(false);
	}

	public void startElopeMode(bool withCollectGC = true)
	{
		if (!isCanStartElopeMode())
		{
			return;
		}
		ShakeCamera.Instance.targetYPos = 0.75f;
		ShakeCamera.Instance.cachedTransform.localPosition = new Vector3(0f, ShakeCamera.Instance.targetYPos, -10f);
		GameManager.currentDungeonType = GameManager.SpecialDungeonType.ElopeMode;
		CameraFollow.instance.cameraLerpSpeedForElopeMode = 4f;
		Singleton<DropItemManager>.instance.startGame();
		Singleton<MiniPopupManager>.instance.refreshForcePositions();
		Singleton<MiniPopupManager>.instance.refreshTargetPositions();
		displayHeart();
		displayHeartCoin();
		displayResources();
		Singleton<DataManager>.instance.saveData();
		GameManager.isWaitForStartGame = false;
		Singleton<GoldManager>.instance.displayGold(false, false);
		Singleton<RubyManager>.instance.displayRuby(false, false);
		GameManager.currentGameState = GameManager.GameState.Playing;
		GameManager.currentDungeonType = GameManager.SpecialDungeonType.ElopeMode;
		UIWindowElopeMode.instance.forceCloseAllMiniPopUp();
		for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
		{
			if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
			{
				ObjectPool.Recycle("@" + Singleton<CharacterManager>.instance.constCharacterList[i].name, Singleton<CharacterManager>.instance.constCharacterList[i].cachedGameObject);
			}
		}
		GameManager.isPause = false;
		displayResources();
		refreshPrincessData();
		refreshPrincessProductionMultiplyValue(true);
		if (Singleton<AdsAngelManager>.instance.currentAngelObject != null)
		{
			Singleton<AdsAngelManager>.instance.currentAngelObject.endAnimation();
		}
		if (Singleton<AdsAngelManager>.instance.currentSpecialAngelObject != null)
		{
			Singleton<AdsAngelManager>.instance.currentSpecialAngelObject.endAnimation();
		}
		for (int j = 0; j < Singleton<CachedManager>.instance.alwaysTopCanvasUICanvasGroup.Length; j++)
		{
			Singleton<CachedManager>.instance.alwaysTopCanvasUICanvasGroup[j].alpha = 0f;
		}
		CameraFollow.instance.startGame();
		UIWindow.CloseAll();
		UIWindowElopeMode.instance.open();
		StopAllCoroutines();
		recycleEnemyCharacter();
		m_currentEnemySpawnXPosition = 1f;
		for (int k = 0; k < 3; k++)
		{
			spawnEnemyCharacter();
		}
		recycleCart();
		recycleDaemonKing();
		currentElopeModeDaemonKing = ObjectPool.Spawn("@ElopeModeDaemonKing", new Vector2(-3f, -2.17f), Singleton<CachedManager>.instance.ingameSetTransform).GetComponent<ElopeModeDaemonKing>();
		currentElopeModeDaemonKing.init();
		spawnCart();
		CameraFollow.instance.targetTransform = currentElopeModeDaemonKing.cachedTransform;
		recycleBackgrounds();
		spawnBackground();
		refreshSkillStatus(false);
		StartCoroutine(moveCheckingUpdate());
		if (withCollectGC)
		{
			GC.Collect();
		}
		isLoadedElopeMode = true;
	}

	public void stopElopeMode(bool withCollectGC = true)
	{
		isLoadedElopeMode = false;
		ObjectPool.Clear(true, "Weapon", "Slot", "Character", "@MiniPopupObject", "@Colleague");
		Singleton<DropItemManager>.instance.endGame();
		GameManager.isPause = true;
		Singleton<AudioManager>.instance.playBackgroundSound("lobby02");
		UIWindowElopeMode.instance.enemyHPProgressObject.closeEnemyHPProgress(true);
		for (int i = 0; i < Singleton<CachedManager>.instance.alwaysTopCanvasUICanvasGroup.Length; i++)
		{
			Singleton<CachedManager>.instance.alwaysTopCanvasUICanvasGroup[i].alpha = 1f;
		}
		StopAllCoroutines();
		recycleCart();
		recycleDaemonKing();
		recycleBackgrounds();
		recycleEnemyCharacter();
		refreshPrincessProductionMultiplyValue(false);
		Singleton<DataManager>.instance.saveData();
		Singleton<GameManager>.instance.setGameState(GameManager.GameState.OutGame);
		Singleton<MiniPopupManager>.instance.refreshForcePositions();
		Singleton<MiniPopupManager>.instance.refreshTargetPositions();
		if (withCollectGC)
		{
			GC.Collect();
		}
	}

	public void stopElopeModeWithFade(bool withCollectGC = true)
	{
		Singleton<CachedManager>.instance.coverUI.fadeOut(0.5f, delegate
		{
			stopElopeMode(withCollectGC);
			Singleton<CachedManager>.instance.coverUI.fadeIn();
		});
	}

	public void exitElopeModeWithDialog()
	{
		UIWindowDialog.openDescription("ELOPE_QUIT_QUESTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			stopElopeModeWithFade();
		}, string.Empty);
	}

	public int getCurrentBackgroundIndex()
	{
		return (int)(Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode / 10000) % backgroundSpriteData.Length;
	}

	public void changeAllBackground(int targetBackgoundIndex)
	{
		for (int i = 0; i < currentElopeBackgroundObject.Count; i++)
		{
			currentElopeBackgroundObject[i].changeBackground(targetBackgoundIndex);
		}
	}

	public void refreshSkillStatus(bool withEffect = true)
	{
		princessProductionMultiplyValueFromHandsomeGuy = 0f;
		daemonKingAttackSpeedMuliply = 1f;
		m_isUsingAnySkill = false;
		DaemonKingSkillType daemonKingSkillType = DaemonKingSkillType.None;
		if (getSkillRemainSecond(DaemonKingSkillType.HandsomeGuyDaemonKing) > 0.0)
		{
			float num = (float)getSkillValue(DaemonKingSkillType.HandsomeGuyDaemonKing);
			if (num > princessProductionMultiplyValueFromHandsomeGuy)
			{
				princessProductionMultiplyValueFromHandsomeGuy = num;
				daemonKingSkillType = DaemonKingSkillType.HandsomeGuyDaemonKing;
			}
		}
		if (getSkillRemainSecond(DaemonKingSkillType.SuperHandsomeGuyDaemonKing) > 0.0)
		{
			float num2 = (float)getSkillValue(DaemonKingSkillType.SuperHandsomeGuyDaemonKing);
			if (num2 > princessProductionMultiplyValueFromHandsomeGuy)
			{
				princessProductionMultiplyValueFromHandsomeGuy = num2;
				daemonKingSkillType = DaemonKingSkillType.SuperHandsomeGuyDaemonKing;
			}
		}
		switch (daemonKingSkillType)
		{
		case DaemonKingSkillType.HandsomeGuyDaemonKing:
			if (UIWindowElopeMode.instance.getElopeSkillMiniPopupObject(DaemonKingSkillType.SuperHandsomeGuyDaemonKing).isOpen)
			{
				UIWindowElopeMode.instance.closeSkillPopup(DaemonKingSkillType.SuperHandsomeGuyDaemonKing);
			}
			UIWindowElopeMode.instance.openSkillPopup(DaemonKingSkillType.HandsomeGuyDaemonKing);
			Singleton<ElopeModeManager>.instance.currentElopeCartObject.playAllPrincessAnimation("Cheer");
			currentElopeModeDaemonKing.changeFace(ElopeModeDaemonKing.DaemonKingFaceType.HandsomeGuy, withEffect);
			currentElopeCartObject.setHeartEffect(DaemonKingSkillType.HandsomeGuyDaemonKing);
			UIWindowElopeMode.instance.fillHeartEffectObject.SetActive(true);
			m_isUsingAnySkill = true;
			break;
		case DaemonKingSkillType.SuperHandsomeGuyDaemonKing:
			if (UIWindowElopeMode.instance.getElopeSkillMiniPopupObject(DaemonKingSkillType.HandsomeGuyDaemonKing).isOpen)
			{
				UIWindowElopeMode.instance.closeSkillPopup(DaemonKingSkillType.HandsomeGuyDaemonKing);
			}
			UIWindowElopeMode.instance.openSkillPopup(DaemonKingSkillType.SuperHandsomeGuyDaemonKing);
			Singleton<ElopeModeManager>.instance.currentElopeCartObject.playAllPrincessAnimation("Cheer");
			currentElopeModeDaemonKing.changeFace(ElopeModeDaemonKing.DaemonKingFaceType.SuperHandsomeGuy, withEffect);
			currentElopeCartObject.setHeartEffect(DaemonKingSkillType.SuperHandsomeGuyDaemonKing);
			UIWindowElopeMode.instance.fillHeartEffectObject.SetActive(true);
			m_isUsingAnySkill = true;
			break;
		case DaemonKingSkillType.None:
			Singleton<ElopeModeManager>.instance.currentElopeCartObject.playAllPrincessRandomAnimation();
			currentElopeModeDaemonKing.changeFace(ElopeModeDaemonKing.DaemonKingFaceType.Normal, withEffect);
			currentElopeCartObject.setHeartEffect(DaemonKingSkillType.None);
			UIWindowElopeMode.instance.fillHeartEffectObject.SetActive(false);
			break;
		}
		DaemonKingSkillType daemonKingSkillType2 = DaemonKingSkillType.None;
		if (getSkillRemainSecond(DaemonKingSkillType.SpeedGuyDaemonKing) > 0.0)
		{
			float num3 = (float)getSkillValue(DaemonKingSkillType.SpeedGuyDaemonKing);
			if (num3 > daemonKingAttackSpeedMuliply)
			{
				daemonKingAttackSpeedMuliply = num3;
				daemonKingSkillType2 = DaemonKingSkillType.SpeedGuyDaemonKing;
			}
		}
		if (getSkillRemainSecond(DaemonKingSkillType.SuperSpeedGuyDaemonKing) > 0.0)
		{
			float num4 = (float)getSkillValue(DaemonKingSkillType.SuperSpeedGuyDaemonKing);
			if (num4 > daemonKingAttackSpeedMuliply)
			{
				daemonKingAttackSpeedMuliply = num4;
				daemonKingSkillType2 = DaemonKingSkillType.SuperSpeedGuyDaemonKing;
			}
		}
		switch (daemonKingSkillType2)
		{
		case DaemonKingSkillType.SpeedGuyDaemonKing:
			if (UIWindowElopeMode.instance.getElopeSkillMiniPopupObject(DaemonKingSkillType.SuperSpeedGuyDaemonKing).isOpen)
			{
				UIWindowElopeMode.instance.closeSkillPopup(DaemonKingSkillType.SuperSpeedGuyDaemonKing);
			}
			UIWindowElopeMode.instance.openSkillPopup(DaemonKingSkillType.SpeedGuyDaemonKing);
			currentElopeModeDaemonKing.changeSword(ElopeModeDaemonKing.DaemonKingSwordType.SpeedGuy, withEffect);
			UIWindowElopeMode.instance.speedGuyEffectObject.SetActive(true);
			UIWindowElopeMode.instance.superSpeedGuyEffectObject.SetActive(false);
			m_isUsingAnySkill = true;
			break;
		case DaemonKingSkillType.SuperSpeedGuyDaemonKing:
			if (UIWindowElopeMode.instance.getElopeSkillMiniPopupObject(DaemonKingSkillType.SpeedGuyDaemonKing).isOpen)
			{
				UIWindowElopeMode.instance.closeSkillPopup(DaemonKingSkillType.SpeedGuyDaemonKing);
			}
			UIWindowElopeMode.instance.openSkillPopup(DaemonKingSkillType.SuperSpeedGuyDaemonKing);
			currentElopeModeDaemonKing.changeSword(ElopeModeDaemonKing.DaemonKingSwordType.SpeedGuy, withEffect);
			UIWindowElopeMode.instance.speedGuyEffectObject.SetActive(false);
			UIWindowElopeMode.instance.superSpeedGuyEffectObject.SetActive(true);
			m_isUsingAnySkill = true;
			break;
		default:
			currentElopeModeDaemonKing.changeSword(ElopeModeDaemonKing.DaemonKingSwordType.Normal, withEffect);
			UIWindowElopeMode.instance.speedGuyEffectObject.SetActive(false);
			UIWindowElopeMode.instance.superSpeedGuyEffectObject.SetActive(false);
			break;
		}
		if (daemonKingSkillType == DaemonKingSkillType.None)
		{
			UIWindowElopeMode.instance.daemonKingSkillBackgroundObject.SetActive(false);
		}
		else
		{
			UIWindowElopeMode.instance.daemonKingSkillBackgroundObject.SetActive(true);
		}
		if (daemonKingSkillType == DaemonKingSkillType.None && daemonKingSkillType2 == DaemonKingSkillType.None)
		{
			Singleton<AudioManager>.instance.playBackgroundSound("elope_bgm");
		}
		else
		{
			Singleton<AudioManager>.instance.playBackgroundSound("elope_bgm_fast");
		}
		refreshPrincessProductionMultiplyValue(true);
	}

	public void spawnElopeRecource(Vector2 spawnPosition, long value)
	{
		DropItemManager.DropItemType dropItemType = DropItemManager.DropItemType.None;
		double num = (double)UnityEngine.Random.Range(0, 10000) * 0.01;
		dropItemType = ((num <= 25.0) ? DropItemManager.DropItemType.ElopeResource1 : ((num <= 50.0) ? DropItemManager.DropItemType.ElopeResource2 : ((!(num <= 75.0)) ? DropItemManager.DropItemType.ElopeResource4 : DropItemManager.DropItemType.ElopeResource3)));
		Singleton<DropItemManager>.instance.spawnDropItem(dropItemType, spawnPosition, value);
	}

	public void spawnCart()
	{
		if (currentElopeCartObject == null)
		{
			currentElopeCartObject = ObjectPool.Spawn("@ElopeCart", new Vector3(-3.414f, 0f, 0f), currentElopeModeDaemonKing.cachedTransform).GetComponent<ElopeCartObject>();
		}
		currentElopeCartObject.initCart(getUnlockedPrincessCount());
	}

	private void recycleCart()
	{
		if (currentElopeCartObject != null)
		{
			currentElopeCartObject.recycleCart();
		}
		currentElopeCartObject = null;
	}

	private void refreshPrincessProductionMultiplyValue(bool isInElopeMode)
	{
		princessProductionMultiplyValue = 1f;
		if (isInElopeMode)
		{
			princessProductionMultiplyValue += princessProductionMultiplyValueFromHandsomeGuy;
		}
	}

	private void princessHeartFullRewardEvent(int princessIndex)
	{
		PrincessInventoryDataForElopeMode princessInventoryData = getPrincessInventoryData(princessIndex);
		princessInventoryData.currentTimer -= getPrincessMaxProductionTime(princessIndex);
		double princessProductionIncreaseValue = getPrincessProductionIncreaseValue(princessIndex);
		increaseHeart(princessProductionIncreaseValue);
		if (UIWindowElopeMode.instance.isOpen)
		{
			UIWindowElopeMode.instance.playHeartEffect(princessIndex);
		}
	}

	private void resetEnemyHealthData()
	{
		if (Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode <= 0.0)
		{
			Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode = getEnemyMaxHealth(Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode + 1);
		}
	}

	public void refreshPrincessData()
	{
		if (GameManager.getCurrentPrincessNumber() > 1)
		{
			Singleton<DataManager>.instance.currentGameData.princessInventoryDataForElopeMode[0].isUnlocked = true;
		}
	}

	private void recycleDaemonKing()
	{
		if (currentElopeModeDaemonKing != null)
		{
			ObjectPool.Recycle(currentElopeModeDaemonKing.name, currentElopeModeDaemonKing.cachedGameObject);
		}
	}

	private void recycleEnemyCharacter()
	{
		for (int i = 0; i < currentElopeEnemyList.Count; i++)
		{
			ObjectPool.Recycle(currentElopeEnemyList[i].name, currentElopeEnemyList[i].cachedGameObject);
		}
		currentElopeEnemyList.Clear();
	}

	private void spawnEnemyCharacter()
	{
		ElopeEnemyObject elopeEnemyObject = null;
		if (currentElopeEnemyList.Count > 0)
		{
			elopeEnemyObject = currentElopeEnemyList[currentElopeEnemyList.Count - 1];
		}
		ElopeEnemyObject component = ObjectPool.Spawn("@ElopeModeEnemyCharacter", new Vector3(m_currentEnemySpawnXPosition, -2.17f, 0f)).GetComponent<ElopeEnemyObject>();
		component.initElopeCharacter((long)Mathf.Max((long)Math.Ceiling((double)(Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode + 1) / 100.0), 1f), (!(elopeEnemyObject != null)) ? CharacterManager.CharacterType.Length : elopeEnemyObject.currentCharacterType, (!(elopeEnemyObject != null)) ? ColleagueManager.ColleagueType.None : elopeEnemyObject.currentColleagueType);
		currentElopeEnemyList.Add(component);
		m_currentEnemySpawnXPosition += intervalBetweenEnemy;
	}

	private void recycleBackgrounds()
	{
		for (int i = 0; i < currentElopeBackgroundObject.Count; i++)
		{
			ObjectPool.Recycle(currentElopeBackgroundObject[i].name, currentElopeBackgroundObject[i].cachedGameObject);
		}
		currentElopeBackgroundObject.Clear();
	}

	private void spawnBackground()
	{
		ElopeBackgroundObject component = ObjectPool.Spawn("@ElopeModeBackground", Vector2.zero).GetComponent<ElopeBackgroundObject>();
		component.cachedTransform.SetParent(Singleton<CachedManager>.instance.cameraParentTransform);
		component.cachedTransform.localPosition = new Vector2(0f, 4.056f);
		component.initBackground();
		component.changeBackground(getCurrentBackgroundIndex());
		currentElopeBackgroundObject.Add(component);
	}

	public void playRewindEffect()
	{
		Singleton<DropItemManager>.instance.collectAllItems();
		recycleEnemyCharacter();
		currentElopeModeDaemonKing.setState(PublicDataManager.State.Wait);
		currentElopeModeDaemonKing.setStateLock(true);
		currentElopeModeDaemonKing.daemonKingAnimation.Stop();
		currentElopeModeDaemonKing.playBoneAnimation(currentElopeModeDaemonKing.currentBoneAnimationName.moveName[0], true);
		currentElopeModeDaemonKing.isRewindEffect = true;
		CameraFollow.instance.cameraLerpSpeedForElopeMode = 100f;
		currentElopeModeDaemonKing.moveTo(new Vector2(-1000f, 0f), 15f, delegate
		{
		});
		currentElopeCartObject.playAllPrincessAnimation("Cheer", 0.04f);
		StopCoroutine(rewindSpeedUpdate());
		StartCoroutine(rewindSpeedUpdate());
		StopCoroutine("rewindEffectUpdate");
		StartCoroutine("rewindEffectUpdate", true);
	}

	private IEnumerator rewindSpeedUpdate()
	{
		float currentSpeed = 1f;
		while (currentElopeModeDaemonKing != null)
		{
			currentSpeed += Time.deltaTime * 7f;
			long distance2 = Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode;
			distance2 = (long)Mathf.Lerp(distance2, 0f, Time.deltaTime * currentSpeed * 0.3f);
			Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode = distance2;
			UIWindowElopeMode.instance.refreshDistanceStatus();
			currentElopeModeDaemonKing.daemonKingAnimation[currentElopeModeDaemonKing.currentBoneAnimationName.moveName[0]].speed = 0f - currentSpeed * 0.5f;
			currentElopeModeDaemonKing.setSpeed(currentSpeed);
			yield return null;
		}
	}

	private IEnumerator rewindEffectUpdate(bool isFirstEffect)
	{
		if (isFirstEffect)
		{
			yield return new WaitForSeconds(2.3f);
			Singleton<CachedManager>.instance.coverUI.fadeOut(0.5f, delegate
			{
				StopCoroutine("rewindSpeedUpdate");
				Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode = 0L;
				Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode = Singleton<ElopeModeManager>.instance.getEnemyMaxHealth(1L);
				Singleton<DataManager>.instance.saveData();
				stopElopeMode(false);
				Singleton<AudioManager>.instance.playEffectSound("questcomplete");
				startElopeMode();
				StartCoroutine("rewindEffectUpdate", false);
			});
		}
		else
		{
			yield return new WaitForSeconds(1f);
			Singleton<CachedManager>.instance.coverUI.fadeIn(1.6f, delegate
			{
			});
		}
	}

	public void increaseResource(DropItemManager.DropItemType resourceType, long value)
	{
		if (resourceType != DropItemManager.DropItemType.Ruby)
		{
			Dictionary<DropItemManager.DropItemType, long> elopeModeResourceData;
			Dictionary<DropItemManager.DropItemType, long> dictionary = (elopeModeResourceData = Singleton<DataManager>.instance.currentGameData.elopeModeResourceData);
			DropItemManager.DropItemType key;
			DropItemManager.DropItemType key2 = (key = resourceType);
			long num = elopeModeResourceData[key];
			dictionary[key2] = num + value;
			displayResources(resourceType);
		}
	}

	public void decreaseResource(DropItemManager.DropItemType resourceType, long value)
	{
		if (resourceType != DropItemManager.DropItemType.Ruby)
		{
			Dictionary<DropItemManager.DropItemType, long> elopeModeResourceData;
			Dictionary<DropItemManager.DropItemType, long> dictionary = (elopeModeResourceData = Singleton<DataManager>.instance.currentGameData.elopeModeResourceData);
			DropItemManager.DropItemType key;
			DropItemManager.DropItemType key2 = (key = resourceType);
			long num = elopeModeResourceData[key];
			dictionary[key2] = num - value;
		}
		else
		{
			Singleton<RubyManager>.instance.decreaseRuby(value);
		}
		displayResources(resourceType);
	}

	public void displayResources()
	{
		displayResources(DropItemManager.DropItemType.ElopeResource1);
		displayResources(DropItemManager.DropItemType.ElopeResource2);
		displayResources(DropItemManager.DropItemType.ElopeResource3);
		displayResources(DropItemManager.DropItemType.ElopeResource4);
	}

	public void displayResources(DropItemManager.DropItemType resourceType)
	{
		switch (resourceType)
		{
		case DropItemManager.DropItemType.ElopeResource1:
		{
			for (int l = 0; l < resource1ValueTexts.Length; l++)
			{
				resource1ValueTexts[l].text = getCurrentResourceValueFromInventory(resourceType).ToString("N0");
			}
			break;
		}
		case DropItemManager.DropItemType.ElopeResource2:
		{
			for (int j = 0; j < resource1ValueTexts.Length; j++)
			{
				resource2ValueTexts[j].text = getCurrentResourceValueFromInventory(resourceType).ToString("N0");
			}
			break;
		}
		case DropItemManager.DropItemType.ElopeResource3:
		{
			for (int k = 0; k < resource1ValueTexts.Length; k++)
			{
				resource3ValueTexts[k].text = getCurrentResourceValueFromInventory(resourceType).ToString("N0");
			}
			break;
		}
		case DropItemManager.DropItemType.ElopeResource4:
		{
			for (int i = 0; i < resource1ValueTexts.Length; i++)
			{
				resource4ValueTexts[i].text = getCurrentResourceValueFromInventory(resourceType).ToString("N0");
			}
			break;
		}
		}
	}

	public void increaseHeartCoin(long value, bool withDisplay = true)
	{
		Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode += value;
		Singleton<DataManager>.instance.currentGameData.heartCoinForElopeModeRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.heartCoinForElopeModeRecord) + value);
		if (withDisplay)
		{
			displayHeartCoin();
		}
	}

	public void decreaseHeartCoin(long value)
	{
		Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode -= value;
		Singleton<DataManager>.instance.currentGameData.heartCoinForElopeModeRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.heartCoinForElopeModeRecord) - value);
		displayHeartCoin();
	}

	public void displayHeartCoin()
	{
		for (int i = 0; i < heartCoinTotalValueTexts.Length; i++)
		{
			heartCoinTotalValueTexts[i].text = Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode.ToString("N0");
		}
	}

	public void increaseHeart(double value)
	{
		Singleton<DataManager>.instance.currentGameData.heartForElopeMode += value;
		displayHeart();
	}

	public void decreaseHeart(double value)
	{
		Singleton<DataManager>.instance.currentGameData.heartForElopeMode = Math.Max(Singleton<DataManager>.instance.currentGameData.heartForElopeMode - value, 0.0);
		displayHeart();
	}

	public void displayHeart()
	{
		for (int i = 0; i < heartTotalValueTexts.Length; i++)
		{
			heartTotalValueTexts[i].text = GameManager.changeUnit(Singleton<DataManager>.instance.currentGameData.heartForElopeMode);
		}
	}

	public void checkDistanceEvent()
	{
		if (Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode % 100 == 0L)
		{
			if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode)
			{
				Singleton<DropItemManager>.instance.spawnDropItem(DropItemManager.DropItemType.HeartCoin, currentElopeModeDaemonKing.currentAttackTargetElopeEnemyObject.cachedTransform.position, 1.0);
				increaseHeartCoin(1L, false);
			}
			else
			{
				increaseHeartCoin(1L);
			}
			Singleton<DataManager>.instance.saveData();
		}
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode && Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode % 10000 == 0L)
		{
			Singleton<CachedManager>.instance.darkUI.fadeOut(1f, delegate
			{
				StartCoroutine(waitForChangeBackground());
			});
		}
	}

	private IEnumerator waitForChangeBackground()
	{
		yield return new WaitForSeconds(0.3f);
		Singleton<CachedManager>.instance.darkUI.fadeInGame(1f, null, true);
		changeAllBackground(getCurrentBackgroundIndex());
	}

	private IEnumerator moveCheckingUpdate()
	{
		Vector2 prevPosition = currentElopeModeDaemonKing.cachedTransform.position;
		Vector2 currentPosition2 = Vector2.zero;
		Dictionary<TimeCheckType, float> distanceCheckingDictionary = new Dictionary<TimeCheckType, float>();
		for (int j = 0; j < 1; j++)
		{
			distanceCheckingDictionary.Add((TimeCheckType)j, 0f);
		}
		while (true)
		{
			currentPosition2 = currentElopeModeDaemonKing.cachedTransform.position;
			if (prevPosition != currentPosition2)
			{
				for (int i = 0; i < 1; i++)
				{
					Dictionary<TimeCheckType, float> dictionary;
					Dictionary<TimeCheckType, float> dictionary2 = (dictionary = distanceCheckingDictionary);
					TimeCheckType key;
					TimeCheckType key2 = (key = (TimeCheckType)i);
					float num = dictionary[key];
					dictionary2[key2] = num + (currentPosition2.x - prevPosition.x);
				}
				prevPosition = currentPosition2;
			}
			while (distanceCheckingDictionary[TimeCheckType.SpawnEnemy] > intervalBetweenEnemy)
			{
				Dictionary<TimeCheckType, float> dictionary3;
				Dictionary<TimeCheckType, float> dictionary4 = (dictionary3 = distanceCheckingDictionary);
				TimeCheckType key;
				TimeCheckType key3 = (key = TimeCheckType.SpawnEnemy);
				float num = dictionary3[key];
				dictionary4[key3] = num - intervalBetweenEnemy;
				spawnEnemyCharacter();
			}
			yield return null;
		}
	}

	public void castActiveSkill(DaemonKingSkillType activeSkillType, bool isCoupon = false)
	{
		switch (activeSkillType)
		{
		case DaemonKingSkillType.HandsomeGuyDaemonKing:
			Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode[DaemonKingSkillType.HandsomeGuyDaemonKing] = 6.4;
			Singleton<ElopeModeManager>.instance.refreshSkillStatus();
			break;
		case DaemonKingSkillType.SuperHandsomeGuyDaemonKing:
		{
			Action superHandsomeGuyDaemonKingPurchaseAction = delegate
			{
				Dictionary<DaemonKingSkillType, double> currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode3;
				Dictionary<DaemonKingSkillType, double> dictionary3 = (currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode3 = Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode);
				DaemonKingSkillType key5;
				DaemonKingSkillType key6 = (key5 = DaemonKingSkillType.SuperHandsomeGuyDaemonKing);
				double num3 = currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode3[key5];
				dictionary3[key6] = num3 + 600.0;
				UIWindowElopeMode.instance.openDaemonKingSkillCastEffect(DaemonKingSkillType.SuperHandsomeGuyDaemonKing);
				Singleton<ElopeModeManager>.instance.refreshSkillStatus();
			};
			if (!isCoupon)
			{
				Singleton<PaymentManager>.instance.Purchase(ShopManager.ElopeModeItemType.SuperHandsomeGuy, delegate
				{
					superHandsomeGuyDaemonKingPurchaseAction();
				});
			}
			else
			{
				superHandsomeGuyDaemonKingPurchaseAction();
			}
			break;
		}
		case DaemonKingSkillType.SpeedGuyDaemonKing:
			Singleton<AdsManager>.instance.showAds("elopeads", delegate
			{
				Singleton<DataManager>.instance.currentGameData.speedGuyAdsEndTime = UnbiasedTime.Instance.Now().AddMinutes(15.0).Ticks;
				Dictionary<DaemonKingSkillType, double> currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode;
				Dictionary<DaemonKingSkillType, double> dictionary = (currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode = Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode);
				DaemonKingSkillType key;
				DaemonKingSkillType key2 = (key = DaemonKingSkillType.SpeedGuyDaemonKing);
				double num = currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode[key];
				dictionary[key2] = num + 240.0;
				UIWindowElopeMode.instance.openDaemonKingSkillCastEffect(DaemonKingSkillType.SpeedGuyDaemonKing);
				Singleton<ElopeModeManager>.instance.refreshSkillStatus();
				UIWindowElopeMode.instance.elopeDaemonKingScrollRect.refreshAll();
			});
			break;
		case DaemonKingSkillType.SuperSpeedGuyDaemonKing:
		{
			Action superSpeedGuyDaemonKingPurchaseEndAction = delegate
			{
				Dictionary<DaemonKingSkillType, double> currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode2;
				Dictionary<DaemonKingSkillType, double> dictionary2 = (currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode2 = Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode);
				DaemonKingSkillType key3;
				DaemonKingSkillType key4 = (key3 = DaemonKingSkillType.SuperSpeedGuyDaemonKing);
				double num2 = currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode2[key3];
				dictionary2[key4] = num2 + 1200.0;
				UIWindowElopeMode.instance.openDaemonKingSkillCastEffect(DaemonKingSkillType.SuperSpeedGuyDaemonKing);
				Singleton<ElopeModeManager>.instance.refreshSkillStatus();
			};
			if (!isCoupon)
			{
				Singleton<PaymentManager>.instance.Purchase(ShopManager.ElopeModeItemType.SuperSpeedGuy, delegate
				{
					superSpeedGuyDaemonKingPurchaseEndAction();
				});
			}
			else
			{
				superSpeedGuyDaemonKingPurchaseEndAction();
			}
			break;
		}
		case DaemonKingSkillType.LoveTimeMachine:
			UIWindowDialog.openDescription("ELOPE_TIME_MACHINE_PROGRESS_QUESTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				double skillBuyPrice = Singleton<ElopeModeManager>.instance.getSkillBuyPrice(DaemonKingSkillType.LoveTimeMachine);
				if ((double)Singleton<DataManager>.instance.currentGameData._ruby >= skillBuyPrice)
				{
					Singleton<RubyManager>.instance.decreaseRuby(skillBuyPrice);
					Singleton<AudioManager>.instance.playEffectSound("love_timemachine");
					Singleton<ElopeModeManager>.instance.playRewindEffect();
				}
				else
				{
					UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
					{
						UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
					}, string.Empty);
				}
			}, string.Empty);
			break;
		}
		Singleton<DataManager>.instance.saveData();
	}

	private void Update()
	{
		if (!isCanStartElopeMode())
		{
			return;
		}
		UpdateForDaemonKingSimulation();
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode && isLoadedElopeMode)
		{
			UpdateForPrincess();
			UpdateForDecreaseSkillRemainSeconds();
			if (m_timerForPrincessEffect >= m_princessEffectSoundMaxTime)
			{
				m_timerForPrincessEffect -= m_princessEffectSoundMaxTime;
				m_princessEffectSoundMaxTime = UnityEngine.Random.Range(1f, 2f) * ((!m_isUsingAnySkill) ? 1f : 0.5f);
				playRandomPrincessEffect();
			}
			m_timerForPrincessEffect += Time.deltaTime;
		}
	}

	public void playRandomPrincessEffect(bool isBackgroundSound = true)
	{
		int num = UnityEngine.Random.Range(1, 12);
		if (isBackgroundSound)
		{
			Singleton<AudioManager>.instance.playEffectSound("princess_effect_" + num, AudioManager.EffectType.Princess);
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("princess_effect_" + num);
		}
	}

	public ElopeEnemyObject getNextElopeEnemyObject()
	{
		ElopeEnemyObject result = null;
		if (currentElopeEnemyList != null && currentElopeEnemyList.Count > 0)
		{
			result = currentElopeEnemyList[0];
		}
		return result;
	}

	public double getEnemyMaxHealth(long level)
	{
		double num = 0.0;
		return Math.Pow(3.0, Math.Round((double)level / 25.0) / 3.0) + 9.0 * Math.Pow(level, 2.0);
	}

	public float attackSpeedattackSpeed(bool isCalculatedAttackSpeed = true)
	{
		float num = 1f;
		if (isCalculatedAttackSpeed)
		{
			num /= daemonKingAttackSpeedMuliply;
		}
		return num;
	}

	public int getUnlockedPrincessCount()
	{
		int num = 0;
		foreach (PrincessInventoryDataForElopeMode item in Singleton<DataManager>.instance.currentGameData.princessInventoryDataForElopeMode)
		{
			if (item.isUnlocked)
			{
				num++;
				continue;
			}
			return num;
		}
		return num;
	}

	public bool isCanStartElopeMode()
	{
		return GameManager.getCurrentPrincessNumber() >= 2;
	}

	public long getSkillLevel(DaemonKingSkillType skillType)
	{
		if (Singleton<DataManager>.instance.currentGameData.currentDaemonKingLevelForElopeMode == null || !Singleton<DataManager>.instance.currentGameData.currentDaemonKingLevelForElopeMode.ContainsKey(skillType))
		{
			return 0L;
		}
		return Singleton<DataManager>.instance.currentGameData.currentDaemonKingLevelForElopeMode[skillType];
	}

	public bool isActiveSkill(DaemonKingSkillType skillType)
	{
		bool result = false;
		if (skillType == DaemonKingSkillType.SpeedGuyDaemonKing || skillType == DaemonKingSkillType.SuperSpeedGuyDaemonKing || skillType == DaemonKingSkillType.SuperHandsomeGuyDaemonKing || skillType == DaemonKingSkillType.LoveTimeMachine)
		{
			result = true;
		}
		return result;
	}

	public DaemonKingUpgradeResourceType getDaemonKingSkillUpgradeResourceType(DaemonKingSkillType skillType)
	{
		DaemonKingUpgradeResourceType result = DaemonKingUpgradeResourceType.None;
		switch (skillType)
		{
		case DaemonKingSkillType.DaemonKingPower:
			result = DaemonKingUpgradeResourceType.Heart;
			break;
		case DaemonKingSkillType.ReinforcementSword:
			result = DaemonKingUpgradeResourceType.Heart;
			break;
		case DaemonKingSkillType.LoveLovePower:
			result = DaemonKingUpgradeResourceType.Heart;
			break;
		case DaemonKingSkillType.HandsomeGuyDaemonKing:
			result = DaemonKingUpgradeResourceType.Ruby;
			break;
		case DaemonKingSkillType.SpeedGuyDaemonKing:
			result = DaemonKingUpgradeResourceType.Ads;
			break;
		case DaemonKingSkillType.SuperHandsomeGuyDaemonKing:
			result = DaemonKingUpgradeResourceType.Cash;
			break;
		case DaemonKingSkillType.SuperSpeedGuyDaemonKing:
			result = DaemonKingUpgradeResourceType.Cash;
			break;
		case DaemonKingSkillType.LoveTimeMachine:
			result = DaemonKingUpgradeResourceType.TimeMachine;
			break;
		}
		return result;
	}

	public Sprite getElopeDaemonSkillIconSprite(DaemonKingSkillType skillType)
	{
		return daemonKingSkillSprites[(int)skillType];
	}

	public double getSkillBuyPrice(DaemonKingSkillType skillType)
	{
		double result = 0.0;
		long num = Singleton<DataManager>.instance.currentGameData.currentDaemonKingLevelForElopeMode[skillType] + 1;
		switch (skillType)
		{
		case DaemonKingSkillType.DaemonKingPower:
			result = Math.Pow(10.0, Math.Round((double)num / 25.0) / 3.0) + 30.0 * Math.Pow(num, 2.0);
			break;
		case DaemonKingSkillType.ReinforcementSword:
			result = Math.Pow(100.0, Math.Round((double)num / 5.0) / 3.0) + 1500.0 * Math.Pow(num, 2.0);
			break;
		case DaemonKingSkillType.LoveLovePower:
			result = Math.Pow(20.0, Math.Round((double)(3 * num) / 25.0) / 3.0) + 180.0 * Math.Pow(num, 2.0);
			break;
		case DaemonKingSkillType.HandsomeGuyDaemonKing:
			result = 10 + num * 20;
			break;
		case DaemonKingSkillType.SuperHandsomeGuyDaemonKing:
			result = 1.99;
			break;
		case DaemonKingSkillType.SpeedGuyDaemonKing:
			result = 0.0;
			break;
		case DaemonKingSkillType.SuperSpeedGuyDaemonKing:
			result = 2.99;
			break;
		case DaemonKingSkillType.LoveTimeMachine:
			result = 1000.0;
			break;
		}
		return result;
	}

	public double getSkillValue(DaemonKingSkillType skillType, bool withRandomDamage = true)
	{
		return getSkillValue(skillType, getSkillLevel(skillType), withRandomDamage);
	}

	public double getSkillValue(DaemonKingSkillType skillType, long level, bool withRandomDamage = true)
	{
		double num = 0.0;
		switch (skillType)
		{
		case DaemonKingSkillType.DaemonKingPower:
			num = (double)(level + 1) * Math.Pow(2.0, (int)(level / daemonKingDoublePowerLevelUnit));
			num += num / 100.0 * getSkillValue(DaemonKingSkillType.ReinforcementSword);
			if (withRandomDamage)
			{
				num *= (double)UnityEngine.Random.Range(0.9f, 1.1f);
			}
			break;
		case DaemonKingSkillType.ReinforcementSword:
			num = (double)(10 * level) * Math.Pow(2.0, (long)((double)level / 25.0));
			break;
		case DaemonKingSkillType.LoveLovePower:
			num = 50.0 + (double)level;
			break;
		case DaemonKingSkillType.HandsomeGuyDaemonKing:
			num = 2.0 + 0.1 * (double)level;
			break;
		case DaemonKingSkillType.SuperHandsomeGuyDaemonKing:
			num = 10.0;
			break;
		case DaemonKingSkillType.SpeedGuyDaemonKing:
			num = 2.0;
			break;
		case DaemonKingSkillType.SuperSpeedGuyDaemonKing:
			num = 3.0;
			break;
		}
		return num;
	}

	public double getDaemonKingCriticalChance()
	{
		long skillLevel = getSkillLevel(DaemonKingSkillType.LoveLovePower);
		return Mathf.Min(10 + (int)(skillLevel / daemonKingDoublePowerLevelUnit), 100);
	}

	public float getPrincessMaxProductionTime(int princessIndex)
	{
		float num = 0f;
		return m_princessMaxProductionTimeArray[princessIndex - 1];
	}

	public PrincessInventoryDataForElopeMode getPrincessInventoryData(int princessIndex)
	{
		return Singleton<DataManager>.instance.currentGameData.princessInventoryDataForElopeMode[princessIndex - 1];
	}

	public double getPrincessProductionIncreaseValue(int princessIndex)
	{
		return getPrincessProductionIncreaseValue(princessIndex, getPrincessInventoryData(princessIndex).princessLevel);
	}

	public double getPrincessProductionIncreaseValue(int princessIndex, long targetLevel)
	{
		double num = 0.0;
		return Math.Pow(13.9, princessIndex) * (1.0 + 0.5 * (double)targetLevel) * Math.Pow(2.0, (int)(targetLevel / princessDoubleHeartLevelUnit));
	}

	public PrincessUpgradeResourceData getPrincessUpgradeResourceData(int princessIndex)
	{
		PrincessUpgradeResourceData result = default(PrincessUpgradeResourceData);
		PrincessInventoryDataForElopeMode princessInventoryData = getPrincessInventoryData(princessIndex);
		long princessLevel = princessInventoryData.princessLevel;
		long num = princessLevel % 5;
		if (num < 0 || num > 4)
		{
			goto IL_008d;
		}
		switch (num)
		{
		case 0L:
			break;
		case 1L:
			goto IL_0059;
		case 2L:
			goto IL_0066;
		case 3L:
			goto IL_0073;
		case 4L:
			goto IL_0080;
		default:
			goto IL_008d;
		}
		result.targetResourceType = DropItemManager.DropItemType.ElopeResource1;
		goto IL_009a;
		IL_009a:
		result.value = (long)(Math.Ceiling((float)(princessLevel + 1) / 5f) * (double)princessIndex);
		if ((princessLevel + 1) % princessDoubleHeartLevelUnit == 0L)
		{
			result.value *= 4L;
		}
		return result;
		IL_008d:
		result.targetResourceType = DropItemManager.DropItemType.None;
		goto IL_009a;
		IL_0080:
		result.targetResourceType = DropItemManager.DropItemType.Ruby;
		goto IL_009a;
		IL_0073:
		result.targetResourceType = DropItemManager.DropItemType.ElopeResource4;
		goto IL_009a;
		IL_0066:
		result.targetResourceType = DropItemManager.DropItemType.ElopeResource3;
		goto IL_009a;
		IL_0059:
		result.targetResourceType = DropItemManager.DropItemType.ElopeResource2;
		goto IL_009a;
	}

	public long getPrincessUnlockPrice(int princessIndex)
	{
		return (princessIndex - 1) * 100;
	}

	public Sprite getPrincessUpgradeResourceIcon(DropItemManager.DropItemType resourceType)
	{
		int index = 0;
		switch (resourceType)
		{
		case DropItemManager.DropItemType.ElopeResource1:
			index = 0;
			break;
		case DropItemManager.DropItemType.ElopeResource2:
			index = 1;
			break;
		case DropItemManager.DropItemType.ElopeResource3:
			index = 2;
			break;
		case DropItemManager.DropItemType.ElopeResource4:
			index = 3;
			break;
		case DropItemManager.DropItemType.Ruby:
			index = 4;
			break;
		}
		return princessResourceSprites[index];
	}

	public long getCurrentResourceValueFromInventory(DropItemManager.DropItemType resourceType)
	{
		if (resourceType == DropItemManager.DropItemType.Ruby)
		{
			return Singleton<DataManager>.instance.currentGameData.obscuredRuby;
		}
		return Singleton<DataManager>.instance.currentGameData.elopeModeResourceData[resourceType];
	}

	public string getResourceI18NName(DropItemManager.DropItemType resourceType)
	{
		string result = string.Empty;
		switch (resourceType)
		{
		case DropItemManager.DropItemType.Ruby:
			result = I18NManager.Get("COUPON_REWARD_RUBY");
			break;
		case DropItemManager.DropItemType.ElopeResource1:
			result = I18NManager.Get("ELOPE_RESOURCE_TITLE_NAME_1");
			break;
		case DropItemManager.DropItemType.ElopeResource2:
			result = I18NManager.Get("ELOPE_RESOURCE_TITLE_NAME_2");
			break;
		case DropItemManager.DropItemType.ElopeResource3:
			result = I18NManager.Get("ELOPE_RESOURCE_TITLE_NAME_3");
			break;
		case DropItemManager.DropItemType.ElopeResource4:
			result = I18NManager.Get("ELOPE_RESOURCE_TITLE_NAME_4");
			break;
		}
		return result;
	}

	public double getSkillRemainSecond(DaemonKingSkillType skillType)
	{
		return Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode[skillType];
	}

	public void checkElopeShopData()
	{
		if (Singleton<DataManager>.instance.currentGameData.elopeShopTargetRefreshTime <= 0 || Singleton<DataManager>.instance.currentGameData.elopeShopTargetRefreshTime > UnbiasedTime.Instance.Now().Ticks)
		{
			Singleton<DataManager>.instance.currentGameData.elopeShopTargetRefreshTime = UnbiasedTime.Instance.Now().Ticks;
			Singleton<DataManager>.instance.currentGameData.lastRefreshElopeShopHour = 0L;
		}
		TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.Now().Ticks - Singleton<DataManager>.instance.currentGameData.elopeShopTargetRefreshTime);
		long num = (long)(timeSpan.TotalHours / (double)intervalRefreshItemHour) + 1;
		if (Singleton<DataManager>.instance.currentGameData.lastRefreshElopeShopHour < num)
		{
			Singleton<DataManager>.instance.currentGameData.lastRefreshElopeShopHour = num;
			refreshElopeShopItems();
		}
		int num2 = 0;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList.Count; i++)
		{
			if (Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].price <= 0)
			{
				Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].price = getItemPrice(Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].currentItemType, Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].value, Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].secondValue);
				num2++;
			}
		}
		if (num2 > 0)
		{
			Singleton<DataManager>.instance.saveData();
		}
	}

	public void refreshElopeShopItems()
	{
		List<ElopeShopItemData> currentElopeShopItemList = Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList;
		currentElopeShopItemList.Clear();
		for (int i = 0; i < 3; i++)
		{
			currentElopeShopItemList.Add(getShopItemData());
		}
		Singleton<DataManager>.instance.saveData();
	}

	private ElopeShopItemData getShopItemData()
	{
		ElopeShopItemData elopeShopItemData = new ElopeShopItemData();
		ElopeShopItemType elopeShopItemType;
		double randomElopeShopItemValue;
		while (true)
		{
			IL_0006:
			elopeShopItemType = getRandomElopeShopItemType();
			int num = 0;
			if (Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList.Count > 0)
			{
				int num2 = 0;
				for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList.Count; i++)
				{
					if (elopeShopItemType == ElopeShopItemType.Treasure)
					{
						num++;
						if (num >= 2)
						{
							goto IL_0006;
						}
					}
					else
					{
						if (Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].currentItemType == elopeShopItemType)
						{
							goto IL_0006;
						}
						if (Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].currentItemType == ElopeShopItemType.Gold || Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].currentItemType == ElopeShopItemType.TimerSilverFinger || Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].currentItemType == ElopeShopItemType.TimerGoldFinger || Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].currentItemType == ElopeShopItemType.TimerAutoOpenTreasureChest || Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].currentItemType == ElopeShopItemType.TimerDoubleSpeed || Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i].currentItemType == ElopeShopItemType.TreasureKey)
						{
							num2++;
						}
					}
					if (num2 >= 2 && (elopeShopItemType == ElopeShopItemType.Gold || elopeShopItemType == ElopeShopItemType.TimerSilverFinger || elopeShopItemType == ElopeShopItemType.TimerGoldFinger || elopeShopItemType == ElopeShopItemType.TimerAutoOpenTreasureChest || elopeShopItemType == ElopeShopItemType.TimerDoubleSpeed || elopeShopItemType == ElopeShopItemType.TreasureKey))
					{
						elopeShopItemType = ElopeShopItemType.Treasure;
					}
				}
			}
			randomElopeShopItemValue = getRandomElopeShopItemValue(elopeShopItemType);
			switch (elopeShopItemType)
			{
			case ElopeShopItemType.Colleague:
			{
				ColleagueManager.ColleagueType colleagueType2 = (ColleagueManager.ColleagueType)randomElopeShopItemValue;
				if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(colleagueType2).isUnlockedFromSlot)
				{
					continue;
				}
				break;
			}
			case ElopeShopItemType.ColleagueSkin:
			{
				ColleagueManager.ColleagueType colleagueType = (ColleagueManager.ColleagueType)randomElopeShopItemValue;
				if (Singleton<ColleagueManager>.instance.isPremiumColleague(colleagueType) || !Singleton<ColleagueManager>.instance.getColleagueInventoryData(colleagueType).isUnlockedFromSlot)
				{
					continue;
				}
				List<int> list4 = new List<int>();
				int num7 = 0;
				int colleagueSkinMaxCount = Singleton<ColleagueManager>.instance.getColleagueSkinMaxCount(colleagueType);
				for (int num8 = 0; num8 < colleagueSkinMaxCount; num8++)
				{
					if (Singleton<ColleagueManager>.instance.containColleagueSkin(colleagueType, num8 + 1))
					{
						num7++;
					}
					else
					{
						list4.Add(num8 + 1);
					}
				}
				if (num7 >= colleagueSkinMaxCount)
				{
					continue;
				}
				elopeShopItemData.secondValue = list4[UnityEngine.Random.Range(0, list4.Count)];
				break;
			}
			case ElopeShopItemType.CharacterSkin:
			{
				long? num3 = null;
				switch ((int)randomElopeShopItemValue)
				{
				case 0:
				{
					List<CharacterSkinManager.WarriorSkinType> list2 = new List<CharacterSkinManager.WarriorSkinType>();
					list2.Add(CharacterSkinManager.WarriorSkinType.Marbas);
					for (int l = 0; l < list2.Count; l++)
					{
						if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(list2[l]).isHaving)
						{
							num3 = (long)list2[l];
							break;
						}
					}
					if (!num3.HasValue)
					{
						continue;
					}
					elopeShopItemData.secondValue = num3.Value;
					break;
				}
				case 1:
				{
					List<CharacterSkinManager.PriestSkinType> list3 = new List<CharacterSkinManager.PriestSkinType>();
					list3.Add(CharacterSkinManager.PriestSkinType.Amy);
					for (int m = 0; m < list3.Count; m++)
					{
						if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(list3[m]).isHaving)
						{
							num3 = (long)list3[m];
							break;
						}
					}
					if (!num3.HasValue)
					{
						continue;
					}
					elopeShopItemData.secondValue = num3.Value;
					break;
				}
				case 2:
				{
					List<CharacterSkinManager.ArcherSkinType> list = new List<CharacterSkinManager.ArcherSkinType>();
					list.Add(CharacterSkinManager.ArcherSkinType.Eligos);
					for (int k = 0; k < list.Count; k++)
					{
						if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(list[k]).isHaving)
						{
							num3 = (long)list[k];
							break;
						}
					}
					if (!num3.HasValue)
					{
						continue;
					}
					elopeShopItemData.secondValue = num3.Value;
					break;
				}
				}
				if (!num3.HasValue)
				{
					continue;
				}
				elopeShopItemData.secondValue = num3.Value;
				break;
			}
			case ElopeShopItemType.Weapon:
			{
				long? num4 = null;
				switch ((int)randomElopeShopItemValue)
				{
				case 0:
				{
					for (int num5 = 45; num5 < 48; num5++)
					{
						if (!Singleton<WeaponManager>.instance.getWeaponFromInventory((WeaponManager.WarriorWeaponType)num5).isUnlockedFromSlot)
						{
							num4 = num5;
							break;
						}
					}
					break;
				}
				case 1:
				{
					for (int num6 = 45; num6 < 48; num6++)
					{
						if (!Singleton<WeaponManager>.instance.getWeaponFromInventory((WeaponManager.PriestWeaponType)num6).isUnlockedFromSlot)
						{
							num4 = num6;
							break;
						}
					}
					break;
				}
				case 2:
				{
					for (int n = 45; n < 48; n++)
					{
						if (!Singleton<WeaponManager>.instance.getWeaponFromInventory((WeaponManager.ArcherWeaponType)n).isUnlockedFromSlot)
						{
							num4 = n;
							break;
						}
					}
					break;
				}
				}
				if (!num4.HasValue)
				{
					continue;
				}
				elopeShopItemData.secondValue = num4.Value;
				break;
			}
			case ElopeShopItemType.Treasure:
			{
				for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList.Count; j++)
				{
					if ((int)Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[j].value == (int)randomElopeShopItemValue)
					{
						goto IL_0006;
					}
				}
				TreasureManager.TreasureType treasureType = (TreasureManager.TreasureType)randomElopeShopItemValue;
				long maxLevel = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].maxLevel;
				if (treasureType == TreasureManager.TreasureType.Null || treasureType == TreasureManager.TreasureType.Length || (Singleton<TreasureManager>.instance.containTreasureFromInventory(treasureType) && Singleton<TreasureManager>.instance.getTreasureDataFromInventory(treasureType).treasureLevel >= maxLevel))
				{
					continue;
				}
				break;
			}
			}
			break;
		}
		elopeShopItemData.currentItemType = elopeShopItemType;
		elopeShopItemData.isBought = false;
		elopeShopItemData.value = randomElopeShopItemValue;
		elopeShopItemData.price = getItemPrice(elopeShopItemType, elopeShopItemData.value, elopeShopItemData.secondValue);
		return elopeShopItemData;
	}

	private double getRandomElopeShopItemValue(ElopeShopItemType itemType)
	{
		double result = 0.0;
		switch (itemType)
		{
		case ElopeShopItemType.Gold:
			result = UnityEngine.Random.Range(250, 751);
			break;
		case ElopeShopItemType.TimerSilverFinger:
			result = UnityEngine.Random.Range(20, 51);
			break;
		case ElopeShopItemType.TimerGoldFinger:
			result = UnityEngine.Random.Range(20, 51);
			break;
		case ElopeShopItemType.TimerAutoOpenTreasureChest:
			result = UnityEngine.Random.Range(20, 51);
			break;
		case ElopeShopItemType.TimerDoubleSpeed:
			result = UnityEngine.Random.Range(10, 21);
			break;
		case ElopeShopItemType.Colleague:
			result = ((!Singleton<ColleagueManager>.instance.getColleagueInventoryData(ColleagueManager.ColleagueType.Trall).isUnlockedFromSlot) ? 24.0 : 27.0);
			break;
		case ElopeShopItemType.CharacterSkin:
		case ElopeShopItemType.Weapon:
		{
			double num5 = (double)(float)UnityEngine.Random.Range(0, 10000) / 100.0;
			result = ((!(num5 <= 33.3)) ? ((!(num5 <= 66.6)) ? 2.0 : 1.0) : 0.0);
			break;
		}
		case ElopeShopItemType.TreasureKey:
			result = UnityEngine.Random.Range(3, 10);
			break;
		case ElopeShopItemType.Treasure:
		{
			List<TreasureManager.TreasureType> list = new List<TreasureManager.TreasureType>();
			List<TreasureManager.TreasureType> list2 = new List<TreasureManager.TreasureType>();
			for (int i = 1; i < 53; i++)
			{
				TreasureManager.TreasureType treasureType = (TreasureManager.TreasureType)i;
				if (Singleton<TreasureManager>.instance.isElopeTreasure(treasureType))
				{
					list.Add(treasureType);
				}
				else if (!Singleton<TreasureManager>.instance.isTowerModeRankingTreasure(treasureType))
				{
					list2.Add(treasureType);
				}
			}
			if (m_randomForTreasureTier.Next(0, 100) < 70)
			{
				Dictionary<TreasureManager.TreasureType, int> dictionary = new Dictionary<TreasureManager.TreasureType, int>();
				int num = 0;
				for (int j = 0; j < list.Count; j++)
				{
					int num2 = (int)Mathf.Min(Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[list[j]].maxLevel, 50f);
					num += num2;
					dictionary.Add(list[j], num2);
				}
				int num3 = m_randomForPremiumTreasure.Next(0, num + 1);
				TreasureManager.TreasureType treasureType2 = TreasureManager.TreasureType.Null;
				int num4 = 0;
				foreach (KeyValuePair<TreasureManager.TreasureType, int> item in dictionary)
				{
					num4 += item.Value;
					if (num3 <= num4)
					{
						treasureType2 = item.Key;
						break;
					}
				}
				result = (double)treasureType2;
			}
			else
			{
				result = (double)list2[m_randomForTreasureType.Next(0, list2.Count)];
			}
			break;
		}
		}
		return result;
	}

	public long getItemPrice(ElopeShopItemType itemType, double value, double secondValue)
	{
		long result = 0L;
		switch (itemType)
		{
		case ElopeShopItemType.Gold:
			result = UnityEngine.Random.Range(6, 16);
			break;
		case ElopeShopItemType.TimerSilverFinger:
			result = UnityEngine.Random.Range(10, 21);
			break;
		case ElopeShopItemType.TimerGoldFinger:
			result = UnityEngine.Random.Range(20, 31);
			break;
		case ElopeShopItemType.TimerAutoOpenTreasureChest:
			result = UnityEngine.Random.Range(25, 36);
			break;
		case ElopeShopItemType.TimerDoubleSpeed:
			result = UnityEngine.Random.Range(15, 26);
			break;
		case ElopeShopItemType.Colleague:
			result = (long)Math.Pow(value, 2.7164) / 100 * 100;
			break;
		case ElopeShopItemType.ColleagueSkin:
			result = 2500L;
			break;
		case ElopeShopItemType.CharacterSkin:
			result = 3000L;
			break;
		case ElopeShopItemType.Weapon:
			result = (long)Math.Pow(secondValue, 1.9967) / 100 * 100;
			break;
		case ElopeShopItemType.TreasureKey:
			result = UnityEngine.Random.Range(5, 16);
			break;
		case ElopeShopItemType.Treasure:
		{
			TreasureManager.TreasureType treasureType = (TreasureManager.TreasureType)value;
			switch (Singleton<TreasureManager>.instance.getTreasureTier(treasureType))
			{
			case 0:
				result = 70L;
				switch (treasureType)
				{
				case TreasureManager.TreasureType.MeteorPiece:
				case TreasureManager.TreasureType.IceCrystal:
					result = 150L;
					break;
				case TreasureManager.TreasureType.SeraphHope:
				case TreasureManager.TreasureType.SeraphBless:
					result = ((!Singleton<TreasureManager>.instance.containTreasureFromInventory(treasureType)) ? 100 : ((Singleton<TreasureManager>.instance.getTreasureDataFromInventory(treasureType).treasureLevel + 1) * 100));
					break;
				case TreasureManager.TreasureType.FireHand:
				case TreasureManager.TreasureType.IceHand:
					result = 200L;
					break;
				case TreasureManager.TreasureType.LegendSkillBook:
				case TreasureManager.TreasureType.BlessHood:
				case TreasureManager.TreasureType.WarriorCape:
				case TreasureManager.TreasureType.AngelHairPin:
				case TreasureManager.TreasureType.ArcherArrow:
					result = 100L;
					break;
				case TreasureManager.TreasureType.ChaosBlade:
				case TreasureManager.TreasureType.FireDragonHeart:
				case TreasureManager.TreasureType.IceHeart:
					result = 500L;
					break;
				case TreasureManager.TreasureType.AntiquityWarriorHelmet:
					result = 300L;
					break;
				}
				break;
			case 1:
				result = 50L;
				break;
			case 2:
				result = 50L;
				break;
			case 3:
				result = 50L;
				break;
			}
			break;
		}
		}
		return result;
	}

	private ElopeShopItemType getRandomElopeShopItemType()
	{
		ElopeShopItemType result = ElopeShopItemType.None;
		double num = 0.0;
		foreach (KeyValuePair<ElopeShopItemType, double> item in m_elopeShopItemRandomValueDictionary)
		{
			num += item.Value;
		}
		MersenneTwister mersenneTwister = new MersenneTwister();
		double num2 = (double)mersenneTwister.Next(0, (int)num * 100) * 0.01;
		double num3 = 0.0;
		foreach (KeyValuePair<ElopeShopItemType, double> item2 in m_elopeShopItemRandomValueDictionary)
		{
			num3 += item2.Value;
			if (num2 <= num3)
			{
				return item2.Key;
			}
		}
		return result;
	}

	public string getItemTypeI18NName(ElopeShopItemType itemType)
	{
		string result = string.Empty;
		switch (itemType)
		{
		case ElopeShopItemType.Gold:
			result = I18NManager.Get("COUPON_REWARD_GOLD");
			break;
		case ElopeShopItemType.TimerSilverFinger:
		case ElopeShopItemType.TimerGoldFinger:
		case ElopeShopItemType.TimerAutoOpenTreasureChest:
		case ElopeShopItemType.TimerDoubleSpeed:
			result = I18NManager.Get("ELOPE_CONSUMABLE_TYPE_TEXT");
			break;
		case ElopeShopItemType.Colleague:
			result = I18NManager.Get("COLLEAGUE");
			break;
		case ElopeShopItemType.ColleagueSkin:
			result = I18NManager.Get("ELOPE_COLLEAGUE_SKIN_TYPE_TEXT");
			break;
		case ElopeShopItemType.CharacterSkin:
			result = I18NManager.Get("LIMITED_SKIN");
			break;
		case ElopeShopItemType.Weapon:
			result = I18NManager.Get("WEAPON_STAT");
			break;
		case ElopeShopItemType.TreasureKey:
			result = I18NManager.Get("KEY");
			break;
		case ElopeShopItemType.Treasure:
			result = I18NManager.Get("TREASURE");
			break;
		}
		return result;
	}

	public Sprite getElopeShopItemIconSprite(ElopeShopItemType itemType, double value, double secondValue)
	{
		Sprite result = null;
		switch (itemType)
		{
		case ElopeShopItemType.Treasure:
		{
			TreasureManager.TreasureType treasureType = (TreasureManager.TreasureType)(long)value;
			result = Singleton<TreasureManager>.instance.getTreasureSprite(treasureType);
			break;
		}
		case ElopeShopItemType.Weapon:
			if (value == 0.0)
			{
				result = Singleton<WeaponManager>.instance.getWeaponSprite((WeaponManager.WarriorWeaponType)secondValue);
			}
			else if (value == 1.0)
			{
				result = Singleton<WeaponManager>.instance.getWeaponSprite((WeaponManager.PriestWeaponType)secondValue);
			}
			else if (value == 2.0)
			{
				result = Singleton<WeaponManager>.instance.getWeaponSprite((WeaponManager.ArcherWeaponType)secondValue);
			}
			break;
		default:
		{
			for (int i = 0; i < iconSpriteList.Count; i++)
			{
				if (iconSpriteList[i].itemType == itemType)
				{
					result = iconSpriteList[i].iconSprite;
				}
			}
			break;
		}
		}
		return result;
	}

	public void buyItem(ElopeShopItemData itemData, out bool isSuccess)
	{
		isSuccess = true;
		switch (itemData.currentItemType)
		{
		case ElopeShopItemType.Gold:
			Singleton<GoldManager>.instance.increaseGold(CalculateManager.getCurrentStandardGold() * itemData.value);
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.Gold, 30L, 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			break;
		case ElopeShopItemType.TimerSilverFinger:
		{
			long num = 0L;
			if (Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime > UnbiasedTime.Instance.Now().Ticks)
			{
				num = new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime).AddMinutes((long)itemData.value).Ticks;
			}
			else
			{
				DateTime dateTime = new DateTime(UnbiasedTime.Instance.Now().Ticks);
				num = dateTime.AddMinutes((long)itemData.value).Ticks;
			}
			Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime = num;
			Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			break;
		}
		case ElopeShopItemType.TimerGoldFinger:
		{
			long num3 = 0L;
			if (Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime > UnbiasedTime.Instance.Now().Ticks)
			{
				num3 = new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime).AddMinutes((long)itemData.value).Ticks;
			}
			else
			{
				DateTime dateTime3 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
				num3 = dateTime3.AddMinutes((long)itemData.value).Ticks;
			}
			Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime = num3;
			Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			break;
		}
		case ElopeShopItemType.TimerAutoOpenTreasureChest:
		{
			long num2 = 0L;
			if (Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime > UnbiasedTime.Instance.Now().Ticks)
			{
				num2 = new DateTime(Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime).AddMinutes((long)itemData.value).Ticks;
			}
			else
			{
				DateTime dateTime2 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
				num2 = dateTime2.AddMinutes((long)itemData.value).Ticks;
			}
			Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime = num2;
			Singleton<TreasureChestManager>.instance.refreshAutoOpenTreasureChest();
			break;
		}
		case ElopeShopItemType.TimerDoubleSpeed:
		{
			long num4 = 0L;
			if (Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime > UnbiasedTime.Instance.Now().Ticks)
			{
				num4 = new DateTime(Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime).AddMinutes((long)itemData.value).Ticks;
			}
			else
			{
				DateTime dateTime4 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
				num4 = dateTime4.AddMinutes((long)itemData.value).Ticks;
			}
			Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime = num4;
			Singleton<GameManager>.instance.refreshTimeScaleMiniPopup();
			break;
		}
		case ElopeShopItemType.Colleague:
		{
			ColleagueManager.ColleagueType colleagueType = (ColleagueManager.ColleagueType)itemData.value;
			if (!Singleton<ColleagueManager>.instance.getColleagueInventoryData(colleagueType).isUnlockedFromSlot)
			{
				Singleton<ColleagueManager>.instance.getColleagueInventoryData(colleagueType).isUnlockedFromSlot = true;
				Singleton<ColleagueManager>.instance.refreshColleagueSlotList();
			}
			else
			{
				isSuccess = false;
				UIWindowDialog.openMiniDialog("BUY_COLLEAGUE_FROM_ELOPE_SHOP_FAILED");
			}
			break;
		}
		case ElopeShopItemType.ColleagueSkin:
		{
			ColleagueManager.ColleagueType colleagueType2 = (ColleagueManager.ColleagueType)itemData.value;
			Singleton<ColleagueManager>.instance.buyColleagueSkin(colleagueType2, (int)itemData.secondValue);
			Singleton<ColleagueManager>.instance.equipColleagueSkin(colleagueType2, (int)itemData.secondValue);
			break;
		}
		case ElopeShopItemType.CharacterSkin:
			if (itemData.value == 0.0)
			{
				CharacterSkinManager.WarriorSkinType skinType = (CharacterSkinManager.WarriorSkinType)itemData.secondValue;
				if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType).isHaving)
				{
					Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType);
					Singleton<CharacterManager>.instance.equipCharacter(skinType);
					Singleton<CharacterSkinManager>.instance.checkRemoveFromPackageForDaemonKingPackage();
				}
				else
				{
					UIWindowDialog.openMiniDialog("BUY_CHARACTER_SKIN_FROM_ELOPE_SHOP_FAILED");
					isSuccess = false;
				}
			}
			if (itemData.value == 1.0)
			{
				CharacterSkinManager.PriestSkinType skinType2 = (CharacterSkinManager.PriestSkinType)itemData.secondValue;
				if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType2).isHaving)
				{
					Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType2);
					Singleton<CharacterManager>.instance.equipCharacter(skinType2);
					Singleton<CharacterSkinManager>.instance.checkRemoveFromPackageForDaemonKingPackage();
				}
				else
				{
					UIWindowDialog.openMiniDialog("BUY_CHARACTER_SKIN_FROM_ELOPE_SHOP_FAILED");
					isSuccess = false;
				}
			}
			if (itemData.value == 2.0)
			{
				CharacterSkinManager.ArcherSkinType skinType3 = (CharacterSkinManager.ArcherSkinType)itemData.secondValue;
				if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType3).isHaving)
				{
					Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType3);
					Singleton<CharacterManager>.instance.equipCharacter(skinType3);
					Singleton<CharacterSkinManager>.instance.checkRemoveFromPackageForDaemonKingPackage();
				}
				else
				{
					UIWindowDialog.openMiniDialog("BUY_CHARACTER_SKIN_FROM_ELOPE_SHOP_FAILED");
					isSuccess = false;
				}
			}
			break;
		case ElopeShopItemType.Weapon:
			if (itemData.value == 0.0)
			{
				if (!Singleton<WeaponManager>.instance.getWeaponFromInventory((WeaponManager.WarriorWeaponType)itemData.secondValue).isUnlockedFromSlot)
				{
					Singleton<WeaponManager>.instance.getWeaponFromInventory((WeaponManager.WarriorWeaponType)itemData.secondValue).isUnlockedFromSlot = true;
				}
				else
				{
					UIWindowDialog.openMiniDialog("BUY_WEAPON_FROM_ELOPE_SHOP_FAILED");
					isSuccess = false;
				}
			}
			if (itemData.value == 1.0)
			{
				if (!Singleton<WeaponManager>.instance.getWeaponFromInventory((WeaponManager.PriestWeaponType)itemData.secondValue).isUnlockedFromSlot)
				{
					Singleton<WeaponManager>.instance.getWeaponFromInventory((WeaponManager.PriestWeaponType)itemData.secondValue).isUnlockedFromSlot = true;
				}
				else
				{
					UIWindowDialog.openMiniDialog("BUY_WEAPON_FROM_ELOPE_SHOP_FAILED");
					isSuccess = false;
				}
			}
			if (itemData.value == 2.0)
			{
				if (!Singleton<WeaponManager>.instance.getWeaponFromInventory((WeaponManager.ArcherWeaponType)itemData.secondValue).isUnlockedFromSlot)
				{
					Singleton<WeaponManager>.instance.getWeaponFromInventory((WeaponManager.ArcherWeaponType)itemData.secondValue).isUnlockedFromSlot = true;
					break;
				}
				UIWindowDialog.openMiniDialog("BUY_WEAPON_FROM_ELOPE_SHOP_FAILED");
				isSuccess = false;
			}
			break;
		case ElopeShopItemType.TreasureKey:
			Singleton<TreasureManager>.instance.increaseTreasurePiece((long)itemData.value);
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.TreasurePiece, 25L, 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			break;
		case ElopeShopItemType.Treasure:
		{
			TreasureManager.TreasureType treasureType = (TreasureManager.TreasureType)itemData.value;
			if (Singleton<TreasureManager>.instance.containTreasureFromInventory(treasureType) && Singleton<TreasureManager>.instance.getTreasureDataFromInventory(treasureType).treasureLevel >= Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].maxLevel)
			{
				isSuccess = false;
				UIWindowDialog.openMiniDialog("BUY_TREASURE_FROM_ELOPE_FAILED");
				break;
			}
			Singleton<TreasureManager>.instance.treasureLotteryEvent(treasureType);
			Singleton<AudioManager>.instance.playEffectSound("questcomplete");
			if (treasureType == TreasureManager.TreasureType.MonocleOfWiseGoddess)
			{
				Singleton<DailyRewardManager>.instance.checkTreasureDailyRewards();
			}
			if (treasureType == TreasureManager.TreasureType.HeliosHarp || treasureType == TreasureManager.TreasureType.CharmOfLunarGoddess)
			{
				Singleton<TreasureManager>.instance.refreshPMAndAMTreasureState();
			}
			break;
		}
		}
	}

	private void UpdateForDaemonKingSimulation()
	{
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode)
		{
			return;
		}
		m_currentTimerForSimulationHuntDaemonKing += Time.deltaTime;
		float num = attackSpeedattackSpeed(false);
		while (m_currentTimerForSimulationHuntDaemonKing >= num)
		{
			double num2 = getSkillValue(DaemonKingSkillType.DaemonKingPower);
			double num3 = (double)UnityEngine.Random.Range(0, 10000) / 100.0;
			if (num3 <= Singleton<ElopeModeManager>.instance.getDaemonKingCriticalChance())
			{
				num2 += num2 * 0.01 * Singleton<ElopeModeManager>.instance.getSkillValue(DaemonKingSkillType.LoveLovePower);
			}
			m_currentTimerForSimulationHuntDaemonKing -= num;
			Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode -= num2;
			if (Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode <= 0.0)
			{
				Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode++;
				checkDistanceEvent();
				Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode = getEnemyMaxHealth((long)Mathf.Max((long)Math.Ceiling((double)(Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode + 1) / 100.0), 1f));
				m_currentTimerForSimulationHuntDaemonKing -= 0.335f;
			}
			if (++m_attackCount % 15 == 0)
			{
				m_attackCount = 0;
				if (GameManager.currentDungeonType != GameManager.SpecialDungeonType.TowerMode)
				{
					Singleton<DataManager>.instance.saveData();
				}
			}
		}
	}

	private void UpdateForPrincess()
	{
		if (GameManager.currentDungeonType != GameManager.SpecialDungeonType.ElopeMode)
		{
			return;
		}
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.princessInventoryDataForElopeMode.Count; i++)
		{
			PrincessInventoryDataForElopeMode princessInventoryDataForElopeMode = Singleton<DataManager>.instance.currentGameData.princessInventoryDataForElopeMode[i];
			if (princessInventoryDataForElopeMode.isUnlocked)
			{
				princessInventoryDataForElopeMode.currentTimer += Time.deltaTime * princessProductionMultiplyValue;
				while (princessInventoryDataForElopeMode.currentTimer >= getPrincessMaxProductionTime(princessInventoryDataForElopeMode.currentPrincessIndex))
				{
					princessHeartFullRewardEvent(princessInventoryDataForElopeMode.currentPrincessIndex);
				}
			}
		}
	}

	private void UpdateForDecreaseSkillRemainSeconds()
	{
		for (int i = 0; i < 8; i++)
		{
			DaemonKingSkillType daemonKingSkillType = (DaemonKingSkillType)i;
			if (getSkillRemainSecond(daemonKingSkillType) > 0.0)
			{
				double num = Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode[daemonKingSkillType];
				num = Math.Max(num - (double)Time.deltaTime, 0.0);
				Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode[daemonKingSkillType] = num;
				if (num <= 0.0)
				{
					refreshSkillStatus();
				}
			}
		}
	}
}
