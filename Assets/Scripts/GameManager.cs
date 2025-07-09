using System;
using System.Collections;
using System.Collections.Generic;
using Fabric.Answers;
using TapjoyUnity;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public enum SpecialDungeonType
	{
		Lobby,
		NormalDungeon,
		BossRaid,
		ElopeMode,
		TowerMode,
		PVP,
		Length
	}

	public enum GameState
	{
		Playing,
		OutGame
	}

	public static int maxTheme = 250;

	public static GameState currentGameState = GameState.OutGame;

	public static int currentFloor;

	public static int unlockTheme = 1;

	public static int currentTheme = 1;

	public static SpecialDungeonType currentDungeonType = SpecialDungeonType.NormalDungeon;

	public static int currentStage = 1;

	public static float timeScale = 1f;

	public static bool isPause;

	public static bool isWaitForStartGame;

	public double originGold;

	public long originRuby;

	public bool isGameOver;

	public bool isBossDie;

	public GameObject normalDungeonObject;

	public bool isPlayedGameOnlyOnce;

	public int originWidth;

	public int originHeight;

	public Transform currentTargetStartSwordEffectTransform;

	public bool isThemeClearEvent;

	public static int maxPrincessCount = 25;

	private static int currentUnit = 0;

	private static string[] upperCaseUnit = new string[57]
	{
		string.Empty,
		"K",
		"M",
		"B",
		"T",
		"a",
		"aa",
		"b",
		"bb",
		"c",
		"cc",
		"d",
		"dd",
		"e",
		"ee",
		"f",
		"ff",
		"g",
		"gg",
		"h",
		"hh",
		"i",
		"ii",
		"j",
		"jj",
		"k",
		"kk",
		"l",
		"ll",
		"m",
		"mm",
		"n",
		"nn",
		"o",
		"oo",
		"p",
		"pp",
		"q",
		"qq",
		"r",
		"rr",
		"s",
		"ss",
		"t",
		"tt",
		"u",
		"uu",
		"v",
		"vv",
		"w",
		"ww",
		"x",
		"xx",
		"y",
		"yy",
		"z",
		"zz"
	};

	public GameObject themeEventBlock;

	private List<TypingText> textList = new List<TypingText>();

	public int currentThemeForThemeEvent;

	public bool reservationKidnappedEvent;

	public bool isThemeEvent;

	private bool m_isSkip;

	private void Awake()
	{
		originWidth = PlayerPrefs.GetInt("OriginWidth", Screen.width);
		originHeight = PlayerPrefs.GetInt("OriginHeight", Screen.height);
		PlayerPrefs.DeleteKey("OriginWidth");
		PlayerPrefs.DeleteKey("OriginHeight");
	}

	public void Pause(bool pause, bool withPauseUI = true)
	{
		if (TutorialManager.isTutorial)
		{
			return;
		}
		if (currentDungeonType == SpecialDungeonType.NormalDungeon)
		{
			if (Singleton<CharacterManager>.instance.warriorCharacter.getState() != PublicDataManager.State.Die)
			{
				isPause = pause;
				if (withPauseUI)
				{
					Singleton<CachedManager>.instance.uiWindowIngame.PauseUI.SetActive(pause);
				}
			}
		}
		else if (currentDungeonType == SpecialDungeonType.TowerMode && Singleton<TowerModeManager>.instance.curPlayingCharacter != null && Singleton<TowerModeManager>.instance.curPlayingCharacter.getState() != PublicDataManager.State.Die)
		{
			isPause = pause;
			if (withPauseUI)
			{
				UIWindowTowerMode.instance.pauseObject.SetActive(pause);
			}
		}
	}

	public void setMaxLimitTheme()
	{
		Singleton<DataManager>.instance.currentGameData.bestTheme = Math.Min(Singleton<DataManager>.instance.currentGameData.bestTheme, maxTheme + 1);
		Singleton<DataManager>.instance.currentGameData.unlockTheme = Math.Min(Singleton<DataManager>.instance.currentGameData.unlockTheme, maxTheme);
		Singleton<DataManager>.instance.currentGameData.unlockStage = Math.Min(Singleton<DataManager>.instance.currentGameData.unlockStage, Singleton<MapManager>.instance.maxStage);
		Singleton<DataManager>.instance.syncWithDataManager();
	}

	public void gameReady(Transform stageItemTransform)
	{
		isPlayedGameOnlyOnce = true;
		currentTargetStartSwordEffectTransform = stageItemTransform;
		if (!TutorialManager.isTutorial)
		{
			Singleton<TutorialManager>.instance.disableAllButtons();
		}
		else
		{
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType3);
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType18);
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType24);
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType30);
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType38);
		}
		StartCoroutine(gameLoading());
	}

	public IEnumerator playSwordEffectUpdate(bool withLoadingUI, Action effectEndAction)
	{
		if (currentTargetStartSwordEffectTransform == null)
		{
			DebugManager.LogError("m_stageStartSwordEffectTransform is null");
			yield break;
		}
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
		UIWindowOutgame.instance.battleEffect.position = currentTargetStartSwordEffectTransform.position;
		UIWindowOutgame.instance.battleIcon.position = currentTargetStartSwordEffectTransform.position;
		UIWindowOutgame.instance.battleAura.position = currentTargetStartSwordEffectTransform.position;
		UIWindowOutgame outgame = UIWindowOutgame.instance;
		float timer2 = 0f;
		float lerp2 = 0f;
		outgame.battleBlock.gameObject.SetActive(true);
		outgame.battleEffectObject.SetActive(true);
		if (currentDungeonType == SpecialDungeonType.ElopeMode)
		{
			Singleton<AudioManager>.instance.playEffectSound("enter_elope", AudioManager.EffectType.Skill);
		}
		while (true)
		{
			timer2 += Time.deltaTime * timeScale * 2f;
			lerp2 = outgame.iconAnim.Evaluate(timer2 * 2f);
			if (timer2 > 0.5f)
			{
				break;
			}
			outgame.battleBlock.color = new Color(0f, 0f, 0f, timer2);
			outgame.battleIcon.localScale = new Vector2(8f - lerp2 * 8f, 8f - lerp2 * 8f);
			outgame.battleAura.localScale = new Vector2(lerp2 * 8f, lerp2 * 8f);
			yield return null;
		}
		if (currentDungeonType != SpecialDungeonType.ElopeMode)
		{
			Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
		}
		timer2 = 0.5f;
		outgame.battleBlock.color = new Color(0f, 0f, 0f, timer2);
		outgame.battleIcon.localScale = new Vector2(8f - lerp2 * 8f, 8f - lerp2 * 8f);
		outgame.battleAura.localScale = new Vector2(lerp2 * 8f, lerp2 * 8f);
		bool isFinishFade = false;
		Action endAction = delegate
		{
			outgame.battleEffectObject.SetActive(false);
			outgame.battleBlock.gameObject.SetActive(false);
			outgame.battleIcon.localScale = Vector2.zero;
			outgame.battleAura.localScale = Vector2.zero;
			isFinishFade = true;
		};
		bool withLoadingUI2 = default(bool);
		Singleton<CachedManager>.instance.coverUI.fadeOut(1f, delegate
		{
			if (withLoadingUI2)
			{
				UIWindowLoading.instance.openLoadingUI(delegate
				{
					endAction();
				});
			}
			else
			{
				endAction();
			}
		});
		yield return new WaitUntil(() => isFinishFade);
		if (effectEndAction != null)
		{
			effectEndAction();
		}
	}

	private IEnumerator gameLoading()
	{
		yield return StartCoroutine(playSwordEffectUpdate(true, delegate
		{
			Singleton<TutorialManager>.instance.enableAllButtons();
			if (currentDungeonType == SpecialDungeonType.BossRaid)
			{
				currentTheme = 10;
				currentStage = 20;
			}
			Singleton<ResourcesManager>.instance.IngameObjectPools(true, delegate
			{
				normalDungeonObject.SetActive(true);
				gameStart();
				if (currentDungeonType == SpecialDungeonType.NormalDungeon)
				{
					Singleton<CachedManager>.instance.mainHPProgressBar.gameObject.SetActive(true);
					Singleton<AudioManager>.instance.playBackgroundSound("monster");
					CameraFollow.instance.maxClampPosition = new Vector2(0f, -4.8f);
					CameraFollow.instance.cachedTransform.localPosition = new Vector2(0f, -4.8f);
					CameraFollow.instance.offset = new Vector2(0f, -1.2f);
				}
				else if (currentDungeonType == SpecialDungeonType.BossRaid)
				{
					normalDungeonObject.SetActive(false);
					Singleton<CachedManager>.instance.mainHPProgressBar.gameObject.SetActive(true);
					Singleton<AudioManager>.instance.playBackgroundSound("boss");
					Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.localPosition = new Vector3(-5.5f, 0.1468978f, Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.localPosition.z);
					for (int i = 1; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
					{
						if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
						{
							Singleton<CharacterManager>.instance.constCharacterList[i].cachedTransform.localPosition = new Vector3(Singleton<CharacterManager>.instance.constCharacterList[i].myLeaderCharacter.cachedTransform.localPosition.x - CharacterManager.intervalBetweenCharacter, 0.1468978f, Singleton<CharacterManager>.instance.constCharacterList[i].cachedTransform.localPosition.z);
						}
					}
					Singleton<ColleagueManager>.instance.refreshStartPositions();
				}
				GC.Collect();
				Singleton<CachedManager>.instance.coverUI.fadeInGame(1f, delegate
				{
				});
			});
		}));
	}

	public void OnClickStartPVP()
	{
		UIWindowPVPMainUI.instance.openPVPMainUI();
	}

	public void gameStart()
	{
		if (currentGameState != 0)
		{
			isThemeEvent = false;
			UIWindowIngame.instance.bossWarningState = false;
			ShakeCamera.Instance.targetYPos = 1.34f;
			ShakeCamera.Instance.shake(0f, 0f);
			themeEventBlock.SetActive(false);
			for (int i = 0; i < Singleton<CachedManager>.instance.enemyInformations.Length; i++)
			{
				Singleton<CachedManager>.instance.enemyInformations[i].ResetEnemy();
				Singleton<CachedManager>.instance.enemyInformations[i].isOpen = false;
			}
			Singleton<GameManager>.instance.reservationKidnappedEvent = false;
			Singleton<CachedManager>.instance.bossInformation.ResetEnemy();
			Singleton<CachedManager>.instance.bossInformation.isOpen = false;
			EnemyInformation.resetEnemyList();
			UIWindowIngame.instance.warningImage.color = new Color(1f, 1f, 1f, 0f);
			UIWindowIngame.instance.hpWarning.color = new Color(1f, 1f, 1f, 0f);
			isGameOver = false;
			isBossDie = false;
			Singleton<AutoTouchManager>.instance.startGame();
			Singleton<AdsAngelManager>.instance.startGame();
			isWaitForStartGame = false;
			Singleton<CachedManager>.instance.townSpriteGroup.setAlpha(1f);
			clearAll();
			Singleton<CachedManager>.instance.warriorTextureRendererCamera.setAcviveCamera(false);
			Singleton<CachedManager>.instance.archerTextureRendererCamera.setAcviveCamera(false);
			Singleton<CachedManager>.instance.priestTextureRendererCamera.setAcviveCamera(false);
			originGold = Singleton<DataManager>.instance.currentGameData.gold;
			originRuby = Singleton<DataManager>.instance.currentGameData._ruby;
			Singleton<DataManager>.instance.syncWithDataManager();
			EnemyInformation.enemyInformationList.Clear();
			Pause(false);
			setGameState(GameState.Playing);
			Singleton<MiniPopupManager>.instance.refreshForcePositions();
			Singleton<MiniPopupManager>.instance.refreshTargetPositions();
			setTimeScale(true);
			if (Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime < UnbiasedTime.Instance.Now().Ticks && (long)Singleton<DataManager>.instance.currentGameData.countDoubleSpeed >= 1)
			{
				--Singleton<DataManager>.instance.currentGameData.countDoubleSpeed;
				refreshTimeScaleMiniPopup(true);
			}
			Singleton<MapManager>.instance.startGame();
			Singleton<CharacterManager>.instance.startGame();
			Singleton<StatManager>.instance.startGame();
			Singleton<GroundManager>.instance.startGame();
			Singleton<DropItemManager>.instance.startGame();
			Singleton<GoldManager>.instance.startGame();
			Singleton<RubyManager>.instance.startGame();
			Singleton<TreasureChestManager>.instance.startGame();
			Singleton<TreasureManager>.instance.startGame();
			Singleton<EnemyManager>.instance.startGame();
			Singleton<SkillManager>.instance.startGame();
			Singleton<FeverManager>.instance.startGame();
			Singleton<ColleagueManager>.instance.startGame();
			Singleton<CachedManager>.instance.darkUI.setAlpha(0f);
			CameraFollow.instance.startGame();
			Singleton<TranscendManager>.instance.startGame();
			Singleton<CollectEventManager>.instance.checkIsOnCollectEvent();
			if (CollectEventManager.isOnCollectEvent)
			{
				Singleton<CollectEventManager>.instance.startGame();
			}
			CustomText.textOrder = 10;
			Singleton<CachedManager>.instance.previewCamera.gameObject.SetActive(false);
			CameraFit.Instance.startGame();
			string level = "Boss Raid Dungeon";
			if (currentDungeonType == SpecialDungeonType.NormalDungeon)
			{
				level = string.Format("Theme {0}, Stage {1}", currentTheme, currentStage);
			}
			Answers.LogLevelStart(level);
			Singleton<CachedManager>.instance.ingameCamera.backgroundColor = Util.getCalculatedColor(0f, 0f, 0f);
			if (Tapjoy.IsConnected)
			{
				Tapjoy.SetUserID(NanooAPIManager.uuid);
				int userLevel = (int)Math.Round((double)((int)Singleton<DataManager>.instance.currentGameData.warriorEquippedWeapon + (int)Singleton<DataManager>.instance.currentGameData.priestEquippedWeapon + Singleton<DataManager>.instance.currentGameData.archerEquippedWeapon) / 3.0);
				Tapjoy.SetUserLevel(userLevel);
			}
		}
	}

	public void refreshTimeScaleMiniPopup(bool displayZero = false)
	{
		if (Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime > UnbiasedTime.Instance.Now().Ticks)
		{
			if (Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.DoubleSpeed))
			{
				Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.DoubleSpeed).closeMiniPopupObject(false);
			}
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.DoubleSpeed, false, new DateTime(Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime), delegate
			{
				refreshTimeScaleMiniPopup();
				setTimeScale(false);
			});
		}
		else if ((long)Singleton<DataManager>.instance.currentGameData.countDoubleSpeed > 0)
		{
			if (Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.DoubleSpeed))
			{
				Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.DoubleSpeed).closeMiniPopupObject(false);
			}
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.DoubleSpeed, Singleton<DataManager>.instance.currentGameData.countDoubleSpeed, delegate
			{
				refreshTimeScaleMiniPopup();
				setTimeScale(false);
			});
		}
		else if (!displayZero)
		{
			if (Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.DoubleSpeed))
			{
				Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.DoubleSpeed).closeMiniPopupObject(false);
			}
		}
		else
		{
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.DoubleSpeed, Singleton<DataManager>.instance.currentGameData.countDoubleSpeed, delegate
			{
				refreshTimeScaleMiniPopup();
				setTimeScale(false);
			});
		}
	}

	public void setTimeScale(bool isStartGame)
	{
		if ((Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime > UnbiasedTime.Instance.Now().Ticks || (long)Singleton<DataManager>.instance.currentGameData.countDoubleSpeed > 0) && isStartGame)
		{
			timeScale = 2f;
		}
		else
		{
			timeScale = 1f;
		}
	}

	public void giveup()
	{
		CharacterWarrior warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		warriorCharacter.setState(PublicDataManager.State.Die);
		Pause(false);
	}

	public void resetGameState(bool isRetry = false)
	{
		Pause(false);
		setTimeScale(false);
		themeEventBlock.SetActive(false);
		Singleton<AdsAngelManager>.instance.endGame();
		Singleton<AutoTouchManager>.instance.endGame();
		Singleton<DropItemManager>.instance.endGame();
		Singleton<CharacterManager>.instance.endGame();
		Singleton<ColleagueManager>.instance.endGame();
		Singleton<TranscendManager>.instance.endGame();
		Singleton<SkillManager>.instance.endGame();
		Singleton<CachedManager>.instance.uiWindowIngame.close();
		Singleton<CachedManager>.instance.uiWindowField.close();
		Singleton<CharacterManager>.instance.resetProperties();
		refreshTimeScaleMiniPopup();
		Singleton<TreasureChestManager>.instance.refreshAutoOpenTreasureChest();
		Singleton<CachedManager>.instance.previewCamera.gameObject.SetActive(true);
		CharacterWarrior warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		warriorCharacter.healthLvlUp();
		warriorCharacter.hpGauge.setProgress(warriorCharacter.curHealth, warriorCharacter.maxHealth);
		clearAll();
		Singleton<DataManager>.instance.saveData();
		moveCharactersToStart();
		Singleton<AudioManager>.instance.playBackgroundSound("lobby02");
		if (Singleton<CachedManager>.instance.princess != null)
		{
			ObjectPool.Recycle(Singleton<CachedManager>.instance.princess.name, Singleton<CachedManager>.instance.princess.cachedGameObject);
			Singleton<CachedManager>.instance.princess = null;
		}
	}

	public void gameEnd(bool isDestroyPool = true, bool isRetry = false)
	{
		setGameState(GameState.OutGame);
		currentDungeonType = SpecialDungeonType.Lobby;
		Singleton<MiniPopupManager>.instance.refreshForcePositions();
		Singleton<MiniPopupManager>.instance.refreshTargetPositions();
		resetGameState(isRetry);
		if (!isDestroyPool)
		{
			return;
		}
		Singleton<ResourcesManager>.instance.IngameObjectPools(false);
		EnemyInformation.resetEnemyList();
		for (int i = 0; i < Singleton<CachedManager>.instance.enemyInformations.Length; i++)
		{
			Singleton<CachedManager>.instance.enemyInformations[i].ResetEnemy();
			Singleton<CachedManager>.instance.enemyInformations[i].isOpen = false;
		}
		Singleton<CachedManager>.instance.bossInformation.ResetEnemy();
		Singleton<CachedManager>.instance.bossInformation.isOpen = false;
		Singleton<EnemyManager>.instance.currentBossObject = null;
		Singleton<EnemyManager>.instance.bossObject = null;
		for (int j = 0; j < Singleton<CharacterManager>.instance.constCharacterList.Count; j++)
		{
			if (Singleton<CharacterManager>.instance.constCharacterList[j] != null)
			{
				Singleton<CharacterManager>.instance.constCharacterList[j].targetEnemy = null;
			}
		}
		Singleton<CharacterManager>.instance.warriorCharacter.attackedEnemy = null;
	}

	public void setGameState(GameState state)
	{
		currentGameState = state;
		UIWindow.CloseAll();
		switch (state)
		{
		case GameState.Playing:
			Singleton<CachedManager>.instance.uiWindowIngame.open();
			Singleton<CachedManager>.instance.uiWindowField.open();
			break;
		case GameState.OutGame:
			currentDungeonType = SpecialDungeonType.Lobby;
			Singleton<CharacterManager>.instance.refreshCharacterUnlockedState();
			Singleton<CachedManager>.instance.uiWindowOutgame.open();
			break;
		}
	}

	public void clearAll()
	{
		ObjectPool.Clear(true, "Weapon", "Slot", "Character", "@MiniPopupObject", "@Colleague");
		ObjectPool.Clear("@ColleagueBullet");
		Singleton<EnemyManager>.instance.enemyList.Clear();
		Singleton<GroundManager>.instance.clearGround();
		Singleton<MapManager>.instance.loadStageData();
		Singleton<MapManager>.instance.resetQueue();
		Singleton<FeverManager>.instance.resetFever();
	}

	public void moveCharactersToStart()
	{
		Singleton<CharacterManager>.instance.warriorCharacter.setStateLock(false);
		Singleton<CharacterManager>.instance.warriorCharacter.stopJump();
		Singleton<CharacterManager>.instance.warriorCharacter.setState(PublicDataManager.State.Wait);
		Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.localPosition = new Vector3(-1.7f, 0f, 0f);
		Singleton<CharacterManager>.instance.warriorCharacter.setDirection(MovingObject.Direction.Right);
		CharacterObject currentFollower = Singleton<CharacterManager>.instance.warriorCharacter.currentFollower;
		int num = 0;
		while (currentFollower != null)
		{
			num++;
			currentFollower.setStateLock(false);
			currentFollower.stopJump();
			currentFollower.cachedTransform.localPosition = new Vector3(-1.7f - (float)num * CharacterManager.intervalBetweenCharacter, 0f, -0.2f * (float)num);
			currentFollower.setDirection(MovingObject.Direction.Right);
			currentFollower = currentFollower.currentFollower;
		}
	}

	public IEnumerator fadeOut()
	{
		isThemeClearEvent = true;
		Singleton<CachedManager>.instance.princess.spriteAnimation.targetRenderer.sortingOrder = 0;
		Singleton<CachedManager>.instance.princess.spriteAnimation.playFixAnimation("Idle", 0);
		LastPrincess princessObject = Singleton<CachedManager>.instance.princess;
		CharacterWarrior warriorObject = Singleton<CharacterManager>.instance.warriorCharacter;
		if (Singleton<CachedManager>.instance.emotionUI == null)
		{
			Transform emotion = ObjectPool.Spawn("@Emotion", Vector2.zero, warriorObject.cachedTransform).transform;
			emotion.position = (Vector2)warriorObject.cachedTransform.position + new Vector2(1f, 2f);
			emotion.localScale = new Vector2(-4f, 4f);
			Singleton<CachedManager>.instance.emotionUI = emotion.GetComponent<EmotionUI>();
		}
		float yLastFloor = warriorObject.cachedTransform.position.y;
		princessObject.setDirection((warriorObject.getDirection() != 1f) ? MovingObject.Direction.Right : MovingObject.Direction.Left);
		if (Singleton<CachedManager>.instance.jail != null)
		{
			Singleton<CachedManager>.instance.jail.DoorControl(true);
		}
		yield return new WaitForSeconds(1f);
		Vector2 endPointPrincess = new Vector2(princessObject.cachedTransform.position.x + 0.5f * princessObject.getDirection(), yLastFloor);
		princessObject.jump(new Vector2(princessObject.getDirection(), 3f), yLastFloor, delegate
		{
			princessObject.spriteAnimation.playAnimation("Move", 0.1f, true);
			princessObject.moveTo(endPointPrincess, 1f, delegate
			{
				princessObject.moveTo(new Vector2(princessObject.cachedTransform.position.x + 6f * princessObject.getDirection(), yLastFloor), 1f);
				Singleton<CachedManager>.instance.coverUI.fadeOutIn(1f, delegate
				{
					princessObject.spriteAnimation.playFixAnimation("Idle", 0);
					princessObject.stopMove();
					Singleton<CharacterManager>.instance.refreshCharacterUnlockedState();
					for (int i = 0; i < Singleton<CharacterManager>.instance.characterList.Count; i++)
					{
						Singleton<CharacterManager>.instance.characterList[i].setDirection(MovingObject.Direction.Left);
						Singleton<CharacterManager>.instance.characterList[i].cachedTransform.localPosition = new Vector3(CharacterManager.intervalBetweenCharacter * (float)i + 1f, 0f, (float)(Singleton<CharacterManager>.instance.characterList.Count - i) * 0.4f);
					}
					Vector2 a = Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.position;
					Vector2 vector = CameraFollow.instance.cachedTransform.position;
					if (currentGameState != 0)
					{
						vector = new Vector2(0f, -4.8f);
					}
					else
					{
						vector = Vector2.Lerp(vector, a + CameraFollow.instance.offset, Time.deltaTime * 4f);
						vector.x = Mathf.Clamp(vector.x, CameraFollow.instance.minClampPosition.x, CameraFollow.instance.maxClampPosition.x);
						vector.y = Mathf.Min(vector.y, CameraFollow.instance.maxClampPosition.y);
					}
					CameraFollow.instance.cachedTransform.position = vector;
					Singleton<CachedManager>.instance.uiWindowField.close();
					UIWindowIngame.instance.uiPanel.SetActive(false);
					UIWindowIngame.instance.eventPanel.SetActive(true);
					Singleton<CachedManager>.instance.themeEventGround.SetActive(true);
					Singleton<CachedManager>.instance.theme6EventBlank.SetActive(false);
					Singleton<CachedManager>.instance.theme6EventGround.SetActive(false);
					Singleton<CachedManager>.instance.theme6EventGround.transform.localPosition = new Vector2(-1.66f, 0.02f);
					StartCoroutine("themeEvent");
				});
			});
		});
	}

	public static string changeUnit(double value, bool integer = true)
	{
		string result = string.Empty;
		for (int num = upperCaseUnit.Length - 1; num > 0; num--)
		{
			if (value >= Math.Pow(1000.0, num))
			{
				value -= 0.005 * Math.Pow(1000.0, num);
				result = string.Format("{0:0.##}{1}", value / Math.Pow(1000.0, num), upperCaseUnit[num]);
				break;
			}
			result = ((!integer) ? string.Format("{0:0.##}", value) : value.ToString("f0"));
		}
		return result;
	}

	public static int getRealThemeNumber(int currentTheme)
	{
		string text = currentTheme.ToString();
		int num = int.Parse(text[text.Length - 1].ToString());
		if (num == 0)
		{
			num = 10;
		}
		return num;
	}

	public static int getCurrentPrincessNumberForElopeMode()
	{
		return Mathf.Min((Singleton<DataManager>.instance.currentGameData.bestTheme - 1) / 10 + 1, maxPrincessCount + 1);
	}

	public static int getCurrentPrincessNumber()
	{
		return Mathf.Min((Singleton<DataManager>.instance.currentGameData.bestTheme - 1) / 10 + 1, maxPrincessCount + 1);
	}

	public static int getCurrentPrincessNumber(int targetTheme)
	{
		return Mathf.Min((targetTheme - 1) / 10 + 1, maxPrincessCount + 1);
	}

	private TypingText createTypingText()
	{
		TypingText component = ObjectPool.Spawn("TypingText", new Vector2(0f, -68f), UIWindowIngame.instance.textParent).GetComponent<TypingText>();
		component.textMessage.horizontalOverflow = HorizontalWrapMode.Wrap;
		component.textMessage.resizeTextForBestFit = true;
		component.textMessage.resizeTextMaxSize = 35;
		component.textMessage.lineSpacing = 0.8f;
		component.textMessage.alignment = TextAnchor.UpperLeft;
		component.rectTransform.anchorMin = new Vector2(0.5f, 1f);
		component.rectTransform.anchorMax = new Vector2(0.5f, 1f);
		component.rectTransform.pivot = new Vector2(0.5f, 1f);
		component.rectTransform.offsetMin = new Vector2(80f, component.rectTransform.offsetMin.y);
		component.rectTransform.offsetMax = new Vector2(-80f, component.rectTransform.offsetMax.y);
		component.rectTransform.sizeDelta = new Vector2(560f, 40f);
		if (textList.Count > 0)
		{
			component.rectTransform.anchoredPosition = textList[textList.Count - 1].rectTransform.anchoredPosition + new Vector2(0f, -50f * textList[textList.Count - 1].lineCount);
		}
		else
		{
			component.rectTransform.anchoredPosition = new Vector2(0f, -68f);
		}
		textList.Add(component);
		return component;
	}

	private void textListClear()
	{
		for (int i = 0; i < textList.Count; i++)
		{
			textList[i].rectTransform.anchorMin = new Vector2(0f, 1f);
			textList[i].rectTransform.anchorMax = new Vector2(1f, 1f);
			ObjectPool.Recycle("TypingText", textList[i].gameObject);
		}
		textList.Clear();
	}

	public void startKidnappedEvent()
	{
		Singleton<TutorialManager>.instance.startPrincessKidnappedEvent();
	}

	public IEnumerator themeEvent()
	{
		isThemeEvent = true;
		ShakeCamera.Instance.targetYPos = 0.75f;
		ShakeCamera.Instance.shake(0f, 0f);
		m_isSkip = false;
		themeEventBlock.SetActive(false);
		LastPrincess princessObject = Singleton<CachedManager>.instance.princess;
		princessObject.setDirection(MovingObject.Direction.Right);
		BossObject boss2 = null;
		CharacterWarrior warrior = Singleton<CharacterManager>.instance.warriorCharacter;
		CharacterPriest priest = Singleton<CharacterManager>.instance.priestCharacter;
		CharacterArcher archer = Singleton<CharacterManager>.instance.archerCharacter;
		warrior.playBoneAnimation(warrior.currentBoneAnimationName.idleName[0]);
		priest.playBoneAnimation(warrior.currentBoneAnimationName.idleName[0]);
		archer.playBoneAnimation(warrior.currentBoneAnimationName.idleName[0]);
		bool eventProcess102 = false;
		float timer6 = 0f;
		if (Social.localUser.authenticated)
		{
			int nTheme = ((currentThemeForThemeEvent > 0) ? (currentThemeForThemeEvent - 1) : 0);
			Social.ReportScore(nTheme, "CgkIgP-i6oAVEAIQOQ", delegate
			{
			});
		}
		int targetTheme = getRealThemeNumber(currentThemeForThemeEvent - 1);
		Singleton<ElopeModeManager>.instance.refreshPrincessData();
		Vector2 bossSpawnPoint5;
		switch (targetTheme)
		{
		case 1:
		{
			princessObject.spriteAnimation.playFixAnimation("Laugh", 0);
			princessObject.cachedTransform.position = new Vector2(-1f, -3.635f);
			yield return new WaitForSeconds(3f);
			TypingText typingText1 = createTypingText();
			typingText1.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			typingText1.SetTextwithBalloon(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT1_SCRIPT_1"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), princessObject.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT1_SCRIPT_2"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), warrior.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			bossSpawnPoint5 = new Vector2(princessObject.cachedTransform.position.x, 10f);
			boss2 = Singleton<EnemyManager>.instance.spawnBoss((EnemyManager.BossType)targetTheme, bossSpawnPoint5, MovingObject.Direction.Right, 9999, true);
			boss2.nonviolence = true;
			princessObject.moveTo(new Vector2(-1.5f, -3.635f), 1f);
			boss2.setDirection(MovingObject.Direction.Right);
			boss2.fixedDirection = true;
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 1);
			boss2.moveTo(new Vector2(-2.5f, -3.635f), 30f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 1, 0.3f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			princessObject.spriteAnimation.playFixAnimation("Idle", 0);
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 2, 0.3f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			boss2.cachedSpriteAnimation.playAnimation("Idle", 0.3f, true);
			boss2.fixedDirection = false;
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			createTypingText().SetText(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT1_SCRIPT_3"), 2f, 1f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 1);
			princessObject.cachedTransform.SetParent(boss2.cachedTransform);
			princessObject.cachedTransform.localPosition = new Vector2(0.903f, 1.016f);
			princessObject.cachedTransform.localScale = new Vector3(1f, 1f, 1f);
			princessObject.bossObject[0].SetActive(true);
			textListClear();
			princessObject.spriteAnimation.playFixAnimation("Taking", 0);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT1_SCRIPT_4"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), princessObject.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			boss2.cachedSpriteAnimation.playFixAnimation("Attack2", 1);
			princessObject.cachedTransform.localPosition = new Vector2(-1.085f, 1.049f);
			princessObject.cachedTransform.localScale = new Vector3(-1f, 1f, 1f);
			boss2.moveTo(new Vector2(-10f, -3.635f), 8f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT1_SCRIPT_5"), 10f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
			{
				if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
				{
					Singleton<CharacterManager>.instance.constCharacterList[i].moveTo(new Vector2(-10f, -3.635f), 10f);
				}
			}
			ObjectPool.Recycle("@Emotion", Singleton<CachedManager>.instance.emotionUI.cachedGameObject);
			textListClear();
			break;
		}
		case 2:
		{
			princessObject.spriteAnimation.playFixAnimation("Laugh", 0);
			princessObject.cachedTransform.position = new Vector2(-1.3f, -3.635f);
			yield return new WaitForSeconds(3f);
			TypingText typingText2 = createTypingText();
			typingText2.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			typingText2.SetTextwithBalloon(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT2_SCRIPT_1"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), princessObject.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT2_SCRIPT_2"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), warrior.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Priest, "magenta", I18NManager.Get("THEMEEVENT2_SCRIPT_3"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), priest.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			bossSpawnPoint5 = new Vector2(-5f, -3.635f);
			boss2 = Singleton<EnemyManager>.instance.spawnBoss((EnemyManager.BossType)targetTheme, bossSpawnPoint5, MovingObject.Direction.Right, 9999, true);
			boss2.nonviolence = true;
			boss2.cachedSpriteAnimation.playAnimation("Move", 0.15f, true);
			boss2.moveTo(new Vector2(-2.5f, -3.635f), 8f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			boss2.fixedDirection = true;
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			boss2.cachedSpriteAnimation.playAnimation("Idle2", 0.15f, true);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT2_SCRIPT_4"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 2f), boss2.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			princessObject.setDirection(MovingObject.Direction.Left);
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 1, 0.15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			princessObject.bossObject[1].transform.localPosition = new Vector2(0.475f, 0.492f);
			princessObject.bossObject[1].transform.localScale = new Vector3(-4f, 4f, 4f);
			princessObject.bossObject[1].SetActive(true);
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 2, 0.15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			boss2.cachedSpriteAnimation.playFixAnimation("Idle2", 0);
			princessObject.cachedTransform.parent = boss2.cachedTransform;
			princessObject.cachedTransform.localPosition = new Vector2(0.239f, 0.751f);
			princessObject.cachedTransform.localScale = new Vector3(1f, 1f, 1f);
			princessObject.bossObject[1].transform.localPosition = new Vector2(-0.233f, 0.11f);
			princessObject.bossObject[1].transform.localScale = new Vector3(4f, 4f, 4f);
			princessObject.bossObject[1].SetActive(true);
			textListClear();
			princessObject.spriteAnimation.playFixAnimation("Taking", 0);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT2_SCRIPT_5"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), princessObject.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			boss2.fixedDirection = false;
			boss2.cachedSpriteAnimation.playAnimation("Move", 0.15f, true);
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			boss2.moveTo(new Vector2(-10f, -3.635f), 8f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT2_SCRIPT_6"), 10f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			for (int j = 0; j < Singleton<CharacterManager>.instance.constCharacterList.Count; j++)
			{
				if (Singleton<CharacterManager>.instance.constCharacterList[j] != null)
				{
					Singleton<CharacterManager>.instance.constCharacterList[j].moveTo(new Vector2(-10f, -3.635f), 10f);
				}
			}
			ObjectPool.Recycle("@Emotion", Singleton<CachedManager>.instance.emotionUI.cachedGameObject);
			break;
		}
		case 3:
		{
			princessObject.cachedTransform.position = new Vector2(-1.5f, -3.635f);
			yield return new WaitForSeconds(3f);
			TypingText typingText3 = createTypingText();
			typingText3.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			typingText3.SetTextwithBalloon(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT3_SCRIPT_1"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), princessObject.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT3_SCRIPT_2"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), warrior.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Priest, "magenta", I18NManager.Get("THEMEEVENT3_SCRIPT_3"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), priest.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Archer, "green", I18NManager.Get("THEMEEVENT3_SCRIPT_4"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), archer.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			bossSpawnPoint5 = new Vector2(-5f, -3.635f);
			boss2 = Singleton<EnemyManager>.instance.spawnBoss((EnemyManager.BossType)targetTheme, bossSpawnPoint5, MovingObject.Direction.Right, 9999, true);
			boss2.nonviolence = true;
			boss2.cachedSpriteAnimation.playAnimation("Move", 0.15f, true);
			boss2.moveTo(new Vector2(-2.5f, -3.635f), 4f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			boss2.fixedDirection = true;
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			boss2.cachedSpriteAnimation.playAnimation("Idle", 0.15f, true);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT3_SCRIPT_5"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 2f), boss2.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			princessObject.setDirection(MovingObject.Direction.Left);
			boss2.cachedSpriteAnimation.playFixAnimation("Attack2", 1, 0.15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			princessObject.bossObject[2].SetActive(true);
			boss2.cachedSpriteAnimation.playFixAnimation("Attack2", 3, 0.15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			boss2.cachedSpriteAnimation.playFixAnimation("Idle", 0);
			princessObject.cachedTransform.parent = boss2.cachedTransform;
			princessObject.cachedTransform.localPosition = new Vector2(0.115f, 0.702f);
			princessObject.cachedTransform.localScale = new Vector3(1f, 1f, 1f);
			princessObject.bossObject[2].SetActive(false);
			princessObject.bossObject[3].SetActive(true);
			textListClear();
			princessObject.spriteAnimation.playFixAnimation("Taking", 0);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT3_SCRIPT_6"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), princessObject.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			boss2.fixedDirection = false;
			boss2.cachedSpriteAnimation.playAnimation("Move", 0.15f, true);
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			boss2.moveTo(new Vector2(-10f, -3.635f), 4f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT3_SCRIPT_7"), 10f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			for (int k = 0; k < Singleton<CharacterManager>.instance.constCharacterList.Count; k++)
			{
				if (Singleton<CharacterManager>.instance.constCharacterList[k] != null)
				{
					Singleton<CharacterManager>.instance.constCharacterList[k].moveTo(new Vector2(-10f, -3.635f), 10f);
				}
			}
			ObjectPool.Recycle("@Emotion", Singleton<CachedManager>.instance.emotionUI.cachedGameObject);
			break;
		}
		case 4:
		{
			princessObject.cachedTransform.position = new Vector2(-1.5f, -3.635f);
			yield return new WaitForSeconds(3f);
			TypingText typingText4 = createTypingText();
			typingText4.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			typingText4.SetTextwithBalloon(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT4_SCRIPT_1"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), princessObject.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT4_SCRIPT_1"), 10f, 1f, null);
			createTypingText().SetText(TypingText.SpeakerType.Archer, "green", I18NManager.Get("THEMEEVENT4_SCRIPT_1"), 10f, 1f, null);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Priest, "magenta", I18NManager.Get("THEMEEVENT4_SCRIPT_1"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), priest.cachedTransform, TypingText.BalloonType.Heroes);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT4_SCRIPT_2"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), warrior.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			bossSpawnPoint5 = new Vector2(-5f, -3.635f);
			boss2 = Singleton<EnemyManager>.instance.spawnBoss((EnemyManager.BossType)targetTheme, bossSpawnPoint5, MovingObject.Direction.Right, 9999, true);
			boss2.nonviolence = true;
			boss2.cachedSpriteAnimation.playAnimation("Move", 0.15f, true);
			boss2.moveTo(new Vector2(-2.5f, -3.635f), 2f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			boss2.fixedDirection = true;
			princessObject.setDirection(MovingObject.Direction.Left);
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT4_SCRIPT_3"), 20f, 1f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetText(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT4_SCRIPT_4"), 1f, 1f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			princessObject.spriteAnimation.playFixAnimation("Laugh", 0);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT4_SCRIPT_5"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), princessObject.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT4_SCRIPT_6"), 20f, 1f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 0, 0.15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 1, 0.15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			princessObject.cachedGameObject.SetActive(false);
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 2, 0.15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			princessObject.spriteAnimation.playFixAnimation("Taking", 0);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Boss, "lightblue", I18NManager.Get("THEMEEVENT4_SCRIPT_7"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), princessObject.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			boss2.fixedDirection = false;
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			boss2.moveTo(new Vector2(-10f, -3.635f), 4f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT4_SCRIPT_8"), 10f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			for (int l = 0; l < Singleton<CharacterManager>.instance.constCharacterList.Count; l++)
			{
				if (Singleton<CharacterManager>.instance.constCharacterList[l] != null)
				{
					Singleton<CharacterManager>.instance.constCharacterList[l].moveTo(new Vector2(-10f, -3.635f), 10f);
				}
			}
			ObjectPool.Recycle("@Emotion", Singleton<CachedManager>.instance.emotionUI.cachedGameObject);
			break;
		}
		case 5:
		{
			princessObject.cachedTransform.position = new Vector2(-1f, -3.635f);
			GameObject blizzard1 = ObjectPool.Spawn("fx_snowstorm", new Vector2(3f, 3f));
			yield return new WaitForSeconds(3f);
			MovingObject snowball1 = ObjectPool.Spawn("Kingsnowball", new Vector2(6f, 6f)).GetComponent<MovingObject>();
			blizzard1.transform.localScale = new Vector3(1f, 1f, 1f);
			snowball1.moveTo(new Vector2(-2f, -2f), 8f, delegate
			{
				eventProcess102 = true;
				snowball1.moveTo(new Vector2(-6f, -6f), 8f, delegate
				{
					ObjectPool.Recycle("Kingsnowball", snowball1.gameObject);
				});
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			bossSpawnPoint5 = new Vector2(-2.5f, -3.635f);
			boss2 = Singleton<EnemyManager>.instance.spawnBoss((EnemyManager.BossType)targetTheme, bossSpawnPoint5, MovingObject.Direction.Right, 9999, true);
			boss2.nonviolence = true;
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 0, 2f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			ObjectPool.Recycle("fx_snowstorm", blizzard1);
			boss2.cachedSpriteAnimation.playAnimation("Idle");
			princessObject.setDirection(MovingObject.Direction.Left);
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			yield return new WaitForSeconds(1f);
			GameObject blizzard2 = ObjectPool.Spawn("fx_snowstorm", new Vector2(-3f, 3f));
			MovingObject snowball2 = ObjectPool.Spawn("Kingsnowball", new Vector2(-10f, 2f)).GetComponent<MovingObject>();
			blizzard2.transform.localScale = new Vector3(-1f, 1f, 1f);
			boss2.cachedSpriteAnimation.playFixAnimation("Attack2", 2);
			snowball2.moveTo(new Vector2(-2f, -2f), 8f, delegate
			{
				boss2.cachedTransform.position = new Vector2(-10f, -3.635f);
				princessObject.cachedTransform.position = new Vector2(-10f, -3.635f);
				snowball2.moveTo(new Vector2(2f, -4f), 8f, delegate
				{
					for (int num = 0; num < Singleton<CharacterManager>.instance.constCharacterList.Count; num++)
					{
						if (Singleton<CharacterManager>.instance.constCharacterList[num] != null)
						{
							ObjectPool.Spawn("SnowObject" + (num + 1), new Vector2(Singleton<CharacterManager>.instance.constCharacterList[num].cachedTransform.position.x, -3.152f));
						}
					}
					snowball2.moveTo(new Vector2(6f, -6f), 8f, delegate
					{
						eventProcess102 = true;
						ObjectPool.Recycle("Kingsnowball", snowball2.gameObject);
					});
				});
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(2f);
			ObjectPool.Recycle("fx_snowstorm", blizzard2);
			TypingText typingText5 = createTypingText();
			typingText5.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			typingText5.SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT4_SCRIPT_1"), 10f, 1f, null);
			createTypingText().SetText(TypingText.SpeakerType.Archer, "green", I18NManager.Get("THEMEEVENT4_SCRIPT_1"), 10f, 1f, null);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Priest, "magenta", I18NManager.Get("THEMEEVENT4_SCRIPT_1"), 10f, 2f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), priest.cachedTransform, TypingText.BalloonType.Heroes);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			break;
		}
		case 6:
		{
			princessObject.cachedTransform.position = new Vector3(1.6f, -3.635f, 1f);
			princessObject.setDirection(MovingObject.Direction.Left);
			MonsterObject slime = ObjectPool.Spawn("@MonsterObject", new Vector2(-2.5f, -3.68f)).GetComponent<MonsterObject>();
			slime.setMonsterDetailAttribute(EnemyManager.MonsterType.Slime1);
			slime.cachedSpriteAnimation.targetRenderer.color = new Color(1f, 1f, 1f, 1f);
			slime.cachedSpriteAnimation.targetRenderer.sortingOrder = 10;
			slime.cachedSpriteAnimation.playAnimation("Idle", 0.15f, true);
			bossSpawnPoint5 = new Vector2(-6f, -3.645f);
			boss2 = Singleton<EnemyManager>.instance.spawnBoss((EnemyManager.BossType)targetTheme, bossSpawnPoint5, MovingObject.Direction.Right, 9999, true);
			boss2.nonviolence = true;
			yield return new WaitForSeconds(3f);
			TypingText typingText6 = createTypingText();
			typingText6.SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT6_SCRIPT_1"), 10f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			typingText6.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			yield return new WaitUntil(() => eventProcess102);
			typingText6.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			eventProcess102 = false;
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			textListClear();
			slime.cachedSpriteAnimation.playAnimation("Move", 0.15f, true);
			slime.moveTo(new Vector2(-1.5f, -3.68f), 0.5f);
			boss2.cachedSpriteAnimation.playAnimation("Idle", 0.15f, true);
			boss2.moveTo(new Vector2(-2f, -3.645f), 1f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			slime.cachedSpriteAnimation.playAnimation("Idle", 0.15f, true);
			boss2.cachedSpriteAnimation.playAnimation("Idle", 0.15f, true);
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT6_SCRIPT_2"), 10f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			boss2.cachedSpriteAnimation.playAnimation("Attack2", 0.15f, false);
			timer6 = 0f;
			while (true)
			{
				timer6 += Time.deltaTime;
				slime.cachedTransform.localScale = new Vector2(1f + timer6 * 2f, 1f + timer6 * 2f);
				if (timer6 > 0.5f)
				{
					break;
				}
				yield return null;
			}
			slime.cachedTransform.localScale = new Vector2(2f, 2f);
			boss2.cachedSpriteAnimation.playFixAnimation("Laugh", 0);
			slime.cachedSpriteAnimation.playFixAnimation("Attack", 0, 1f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			slime.cachedSpriteAnimation.playFixAnimation("Attack", 1);
			slime.moveTo(new Vector2(0f, -3.68f), 10f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			SpriteAnimation slimeFace = ObjectPool.Spawn("SlimeFace", new Vector2(-0.0057568f, 0.55f), slime.cachedTransform).GetComponent<SpriteAnimation>();
			slimeFace.cachedTransform.localScale = new Vector2(-4f, 4f);
			slime.cachedSpriteAnimation.playFixAnimation("IdleBody", 0);
			slimeFace.playFixAnimation("IdleFace", 0);
			slime.cachedTransform.position = new Vector2(1.6f, -3.68f);
			slime.cachedSpriteAnimation.targetRenderer.color = new Color(1f, 1f, 1f, 0.6f);
			slime.cachedSpriteAnimation.targetRenderer.sortingOrder = 10;
			princessObject.cachedTransform.SetParent(slime.cachedTransform);
			warrior.cachedTransform.SetParent(slime.cachedTransform);
			priest.cachedTransform.SetParent(slime.cachedTransform);
			Color transparent = new Color(1f, 1f, 1f, 0.5f);
			princessObject.spriteAnimation.targetRenderer.color = transparent;
			princessObject.spriteAnimation.targetRenderer.sortingOrder = 6;
			for (int i3 = 0; i3 < warrior.characterSpriteRenderers.Length; i3++)
			{
				warrior.characterSpriteRenderers[i3].color = transparent;
			}
			for (int i2 = 0; i2 < priest.characterSpriteRenderers.Length; i2++)
			{
				priest.characterSpriteRenderers[i2].color = transparent;
			}
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT6_SCRIPT_3"), 10f, 2f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0.164f, 2.671f), boss2.cachedTransform, TypingText.BalloonType.Heart);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			createTypingText().SetText(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT6_SCRIPT_4"), 10f, 1.5f, null);
			princessObject.setDirection(MovingObject.Direction.Right);
			warrior.setDirection(MovingObject.Direction.Right);
			priest.setDirection(MovingObject.Direction.Right);
			slime.cachedSpriteAnimation.playAnimation("MoveBody", 0.15f, true);
			slimeFace.playAnimation("MoveFace", 0.15f, true);
			slime.moveTo(new Vector2(-0.5f, -3.68f), 2f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			slime.moveTo(bossSpawnPoint5, 2f);
			boss2.cachedSpriteAnimation.playAnimation("Move", 0.15f, true);
			boss2.moveTo(bossSpawnPoint5, 2f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Archer, "green", I18NManager.Get("THEMEEVENT6_SCRIPT_5"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), archer.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			princessObject.cachedTransform.SetParent(Singleton<CachedManager>.instance.ingameBackgroundTransform);
			warrior.cachedTransform.SetParent(Singleton<CachedManager>.instance.characterTransform);
			priest.cachedTransform.SetParent(Singleton<CachedManager>.instance.characterTransform);
			slimeFace.cachedTransform.SetParent(null);
			Color white = new Color(1f, 1f, 1f, 1f);
			princessObject.spriteAnimation.targetRenderer.color = white;
			princessObject.spriteAnimation.targetRenderer.sortingOrder = 0;
			warrior.cachedTransform.position = new Vector2(-5f, -2f);
			warrior.cachedTransform.localRotation = Quaternion.Euler(0f, 0f, -90f);
			for (int n = 0; n < warrior.characterSpriteRenderers.Length; n++)
			{
				warrior.characterSpriteRenderers[n].color = white;
			}
			warrior.jump(new Vector2(10f, 10f), -3.635f);
			priest.cachedTransform.position = new Vector2(-4.5f, -2f);
			priest.cachedTransform.localRotation = Quaternion.Euler(0f, 0f, -90f);
			for (int m = 0; m < priest.characterSpriteRenderers.Length; m++)
			{
				priest.characterSpriteRenderers[m].color = white;
			}
			priest.jump(new Vector2(10f, 6f), -3.635f);
			timer6 = 0f;
			int count = 0;
			slimeFace.playFixAnimation("slimebomb", count);
			slimeFace.cachedTransform.position = new Vector2(-4f, -2f);
			while (true)
			{
				timer6 += Time.deltaTime;
				if (timer6 > 0.1f)
				{
					timer6 = 0f;
					count++;
					if (count > 3)
					{
						break;
					}
					slimeFace.playFixAnimation("slimebomb", count);
					switch (count)
					{
					case 1:
						slimeFace.cachedTransform.position = new Vector2(-3.3f, -2f);
						break;
					case 2:
						slimeFace.cachedTransform.position = new Vector2(-2f, -2f);
						break;
					case 3:
						slimeFace.cachedTransform.position = new Vector2(-2f, -3.68f);
						break;
					}
				}
				yield return null;
			}
			timer6 = 0f;
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Archer, "green", I18NManager.Get("THEMEEVENT4_SCRIPT_1"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), archer.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			break;
		}
		case 7:
		{
			princessObject.cachedTransform.position = new Vector2(-5f, -3.635f);
			yield return new WaitForSeconds(3f);
			TypingText targetText7 = createTypingText();
			targetText7.SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT7_SCRIPT_1"), 15f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			targetText7.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			targetText7.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			textListClear();
			princessObject.spriteAnimation.playAnimation("Move", 0.1f, true);
			princessObject.moveTo(new Vector2(-1f, -3.635f), 2f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			princessObject.spriteAnimation.playFixAnimation("Idle", 0);
			createTypingText().SetText(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT7_SCRIPT_2"), 15f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			bossSpawnPoint5 = new Vector2(-8f, -2f);
			boss2 = Singleton<EnemyManager>.instance.spawnBoss((EnemyManager.BossType)targetTheme, bossSpawnPoint5, MovingObject.Direction.Right, 9999, true);
			boss2.nonviolence = true;
			boss2.fixedDirection = true;
			boss2.cachedSpriteAnimation.playFixAnimation("Flying", 0);
			boss2.moveTo(new Vector2(-2.5f, -3.635f), 5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			boss2.cachedSpriteAnimation.playFixAnimation("Attack", 1, 0.15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			boss2.cachedSpriteAnimation.playFixAnimation("Attack", 1, 0.15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			princessObject.setDirection(MovingObject.Direction.Left);
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			boss2.cachedSpriteAnimation.playAnimation("Attack", 0.15f, false, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<CachedManager>.instance.ingameCameraShake.shake(3f, 0.1f);
			yield return new WaitForSeconds(0.2f);
			boss2.cachedSpriteAnimation.playAnimation("Attack", 0.15f, false, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<CachedManager>.instance.ingameCameraShake.shake(3f, 0.1f);
			yield return new WaitForSeconds(0.2f);
			MonsterObject babyDragon = ObjectPool.Spawn("@MonsterObject", new Vector2(5f, -8f)).GetComponent<MonsterObject>();
			babyDragon.setMonsterDetailAttribute(EnemyManager.MonsterType.Babydragon1);
			babyDragon.cachedSpriteAnimation.targetRenderer.color = new Color(1f, 1f, 1f, 1f);
			babyDragon.cachedSpriteAnimation.playAnimation("Idle", 0.15f, true);
			babyDragon.cachedSpriteAnimation.targetRenderer.sortingLayerName = "UI";
			babyDragon.cachedSpriteAnimation.targetRenderer.sortingOrder = 5;
			babyDragon.moveTo(new Vector2(-1f, -8f), 5f);
			boss2.cachedSpriteAnimation.playAnimation("Attack", 0.15f, false, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<CachedManager>.instance.ingameCameraShake.shake(3f, 0.1f);
			boss2.cachedSpriteAnimation.playFixAnimation("Jump", 0, 0.15f, delegate
			{
				boss2.moveTo(bossSpawnPoint5, 5f);
			});
			boss2.moveTo(bossSpawnPoint5, 5f);
			SpriteAnimation fallEffect = ObjectPool.Spawn("FallEffect", new Vector2(-1.66f, -2.55f)).GetComponent<SpriteAnimation>();
			fallEffect.cachedTransform.localScale = Vector2.one * 4f;
			fallEffect.playAnimation("effect", 0.1f, false);
			princessObject.spriteAnimation.targetRenderer.sortingLayerName = "UI";
			princessObject.spriteAnimation.targetRenderer.sortingOrder = 6;
			princessObject.spriteAnimation.playFixAnimation("Taking", 0);
			princessObject.moveTo(new Vector2(-1f, -8.3f), 6f);
			Singleton<CachedManager>.instance.theme6EventBlank.SetActive(true);
			Singleton<CachedManager>.instance.theme6EventGround.SetActive(true);
			timer6 = 0f;
			while (true)
			{
				timer6 += Time.deltaTime;
				Singleton<CachedManager>.instance.theme6EventGround.transform.localPosition = new Vector2(-1.66f, Singleton<CachedManager>.instance.theme6EventGround.transform.localPosition.y + timer6 * (0f - timer6));
				if (timer6 > 0.8f)
				{
					break;
				}
				yield return null;
			}
			timer6 = 0f;
			ObjectPool.Recycle("FallEffect", fallEffect.gameObject);
			babyDragon.moveTo(new Vector2(-5f, -8f), 5f);
			princessObject.moveTo(new Vector2(-5f, -8.3f), 5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(2f);
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT7_SCRIPT_3"), 15f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			princessObject.spriteAnimation.targetRenderer.sortingLayerName = "Player";
			princessObject.spriteAnimation.targetRenderer.sortingOrder = 3;
			break;
		}
		case 8:
		{
			princessObject.cachedTransform.position = new Vector3(1.6f, -3.635f, 1f);
			princessObject.setDirection(MovingObject.Direction.Left);
			yield return new WaitForSeconds(3f);
			warrior.playBoneAnimation(warrior.currentBoneAnimationName.idleName[0]);
			TypingText typingText7 = createTypingText();
			typingText7.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			typingText7.SetTextwithBalloon(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT8_SCRIPT_1"), 15f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), warrior.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			priest.playBoneAnimation(priest.currentBoneAnimationName.idleName[0]);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Priest, "magenta", I18NManager.Get("THEMEEVENT8_SCRIPT_2"), 15f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), priest.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			archer.playBoneAnimation(archer.currentBoneAnimationName.idleName[0]);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Archer, "green", I18NManager.Get("THEMEEVENT8_SCRIPT_3"), 15f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), archer.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			warrior.playBoneAnimation(warrior.currentBoneAnimationName.idleName[0]);
			priest.playBoneAnimation(priest.currentBoneAnimationName.idleName[0]);
			archer.playBoneAnimation(archer.currentBoneAnimationName.idleName[0]);
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			MovingObject reaperBack = ObjectPool.Spawn("Reaper", new Vector2(6f, 0f)).GetComponent<MovingObject>();
			reaperBack.moveTo(new Vector2(-6f, 0f), 10f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			ObjectPool.Recycle("Reaper", reaperBack.gameObject);
			textListClear();
			bossSpawnPoint5 = new Vector2(-8f, -2f);
			boss2 = Singleton<EnemyManager>.instance.spawnBoss((EnemyManager.BossType)targetTheme, bossSpawnPoint5, MovingObject.Direction.Right, 9999, true);
			boss2.nonviolence = true;
			boss2.fixedDirection = true;
			boss2.cachedSpriteAnimation.playAnimation("Move", 0.15f, true);
			boss2.moveTo(new Vector2(1f, -4.4f), 15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetTextImmediate(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT8_SCRIPT_4"), 20f, 1f, null);
			boss2.cachedSpriteAnimation.playFixAnimation("Attack1", 1, 0.1f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			princessObject.spriteAnimation.targetRenderer.sortingOrder = 6;
			princessObject.cachedTransform.SetParent(boss2.cachedTransform);
			princessObject.cachedTransform.localPosition = new Vector2(-0.536f, 1f);
			princessObject.cachedTransform.localScale = new Vector3(-1f, 1f, 1f);
			princessObject.spriteAnimation.playFixAnimation("Taking", 0);
			princessObject.bossObject[4].SetActive(true);
			boss2.cachedSpriteAnimation.playFixAnimation("Move", 0, 0.1f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			boss2.moveTo(new Vector2(3f, -3.3f), 15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			boss2.moveTo(new Vector2(8f, 2f), 15f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(2f);
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT8_SCRIPT_5"), 15f, 2f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 1.3f), warrior.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			break;
		}
		case 9:
		{
			warrior.fixedDirection = true;
			priest.fixedDirection = true;
			archer.fixedDirection = true;
			princessObject.fixedDirection = true;
			princessObject.cachedTransform.position = new Vector3(1.6f, -3.635f, 1f);
			princessObject.setDirection(MovingObject.Direction.Left);
			boss2 = Singleton<EnemyManager>.instance.spawnBoss((EnemyManager.BossType)targetTheme, new Vector2(-2f, -3.635f), MovingObject.Direction.Right, 9999, true);
			boss2.cachedSpriteAnimation.playFixAnimation("Idle", 0);
			boss2.nonviolence = true;
			boss2.fixedDirection = true;
			yield return new WaitForSeconds(3f);
			TypingText typingText8 = createTypingText();
			typingText8.SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT9_SCRIPT_1"), 15f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			typingText8.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			yield return new WaitUntil(() => eventProcess102);
			typingText8.rectTransform.anchoredPosition = new Vector2(0f, -68f);
			eventProcess102 = false;
			textListClear();
			boss2.cachedSpriteAnimation.playFixAnimation("Move1", 1);
			boss2.cachedTransform.position = (Vector2)boss2.cachedTransform.position + new Vector2(0.2f, 0f);
			createTypingText().SetTextDelay(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT9_SCRIPT_2"), 10f, 1f, delegate(int delayIdx)
			{
				if (delayIdx == 0)
				{
					boss2.cachedSpriteAnimation.playFixAnimation("Move1", 0, 0.15f);
					boss2.cachedTransform.position = (Vector2)boss2.cachedTransform.position + new Vector2(0.2f, 0f);
				}
			}, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			warrior.moveTo((Vector2)warrior.cachedTransform.position + new Vector2(0.1f, 0f), 2f);
			priest.moveTo((Vector2)priest.cachedTransform.position + new Vector2(0.1f, 0f), 2f);
			archer.moveTo((Vector2)archer.cachedTransform.position + new Vector2(0.1f, 0f), 2f);
			princessObject.moveTo((Vector2)princessObject.cachedTransform.position + new Vector2(0.1f, 0f), 2f);
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT9_SCRIPT_3"), 15f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			boss2.cachedSpriteAnimation.playFixAnimation("Move1", 1);
			boss2.cachedTransform.position = (Vector2)boss2.cachedTransform.position + new Vector2(0.2f, 0f);
			createTypingText().SetTextDelay(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT9_SCRIPT_4"), 10f, 1f, delegate(int delayIdx)
			{
				if (delayIdx == 0)
				{
					boss2.cachedSpriteAnimation.playFixAnimation("Move1", 0, 0.15f);
					boss2.cachedTransform.position = (Vector2)boss2.cachedTransform.position + new Vector2(0.2f, 0f);
				}
			}, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			warrior.moveTo((Vector2)warrior.cachedTransform.position + new Vector2(0.1f, 0f), 2f);
			priest.moveTo((Vector2)priest.cachedTransform.position + new Vector2(0.1f, 0f), 2f);
			archer.moveTo((Vector2)archer.cachedTransform.position + new Vector2(0.1f, 0f), 2f);
			princessObject.moveTo((Vector2)princessObject.cachedTransform.position + new Vector2(0.1f, 0f), 2f);
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT9_SCRIPT_5"), 15f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			boss2.cachedSpriteAnimation.playFixAnimation("Move1", 1);
			boss2.cachedTransform.position = (Vector2)boss2.cachedTransform.position + new Vector2(0.2f, 0f);
			createTypingText().SetTextDelay(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT9_SCRIPT_6"), 10f, 1f, delegate(int delayIdx)
			{
				if (delayIdx == 0)
				{
					boss2.cachedSpriteAnimation.playFixAnimation("Move1", 0, 0.15f);
					boss2.cachedTransform.position = (Vector2)boss2.cachedTransform.position + new Vector2(0.2f, 0f);
				}
			}, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			createTypingText().SetText(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT9_SCRIPT_7"), 15f, 0.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT9_SCRIPT_8"), 15f, 1f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			princessObject.moveTo((Vector2)princessObject.cachedTransform.position + new Vector2(-0.3f, 0f), 2f);
			princessObject.spriteAnimation.playAnimation("Move", 0.15f, true);
			Singleton<CachedManager>.instance.coverUI.fadeOutIn(0.3f, delegate
			{
				Singleton<AudioManager>.instance.playBackgroundSound("boss");
				princessObject.cachedTransform.SetParent(boss2.cachedTransform);
				princessObject.cachedTransform.localPosition = new Vector2(-0.73f, 1.849f);
				princessObject.cachedTransform.localScale = new Vector3(-0.8333f, 0.8333f, 1f);
				princessObject.spriteAnimation.targetRenderer.sortingOrder = -2;
				princessObject.spriteAnimation.playFixAnimation("Laugh", 0);
				boss2.setDirection(MovingObject.Direction.Left);
				boss2.cachedSpriteAnimation.playFixAnimation("Move2", 0, 2f, delegate
				{
					eventProcess102 = true;
				});
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetTextwithBalloon(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT9_SCRIPT_9"), 10f, 1f, delegate
			{
				eventProcess102 = true;
			}, new Vector2(0f, 2f), boss2.cachedTransform);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			textListClear();
			warrior.moveTo((Vector2)warrior.cachedTransform.position + new Vector2(-0.3f, 0f), 1f);
			priest.moveTo((Vector2)priest.cachedTransform.position + new Vector2(-0.3f, 0f), 1f);
			archer.moveTo((Vector2)archer.cachedTransform.position + new Vector2(-0.3f, 0f), 1f);
			boss2.cachedSpriteAnimation.playAnimation("Move2", 0.15f, true);
			boss2.moveTo(new Vector2(-2.5f, -3.635f), 1f, delegate
			{
				boss2.cachedSpriteAnimation.playAnimation("Move2", 0.1f, true);
				boss2.moveTo(new Vector2(-6f, -3.635f), 8f, delegate
				{
					eventProcess102 = true;
				});
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT4_SCRIPT_1"), 15f, 1.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			break;
		}
		case 10:
		{
			reservationKidnappedEvent = true;
			UIWindowOutgame.instance.princessCollectionIndicator.SetActive(true);
			BossObjectForThemeEvent daemonKing = ObjectPool.Spawn("@DaemonKingForThemeEvent", new Vector2(-2.202f, -3.655f)).GetComponent<BossObjectForThemeEvent>();
			daemonKing.setDirection(MovingObject.Direction.Right);
			daemonKing.initBoss();
			warrior.cachedTransform.position = new Vector3(-0.14f, -3.626f, 0f);
			warrior.setDirection(MovingObject.Direction.Left);
			warrior.playBoneAnimation(warrior.currentBoneAnimationName.idleName[0]);
			priest.cachedTransform.position = new Vector3(2.267f, -3.626f, 0f);
			priest.setDirection(MovingObject.Direction.Left);
			priest.playBoneAnimation(priest.currentBoneAnimationName.idleName[0]);
			archer.cachedTransform.position = new Vector3(3.125f, -3.626f, 0f);
			archer.setDirection(MovingObject.Direction.Left);
			archer.playBoneAnimation(archer.currentBoneAnimationName.idleName[0]);
			princessObject.cachedTransform.position = new Vector3(1.737f, -3.627f);
			princessObject.setDirection(MovingObject.Direction.Left);
			yield return new WaitForSeconds(2f);
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT10_SCRIPT_1"), 11f, 0.5f, delegate
			{
				eventProcess102 = true;
			});
			warrior.playBoneAnimation(warrior.currentBoneAnimationName.skillName[0]);
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(0.7f);
			textListClear();
			createTypingText().SetText(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT10_SCRIPT_2"), 2f, 0.6f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			daemonKing.fallMask();
			yield return new WaitForSeconds(0.8f);
			textListClear();
			daemonKing.cachedSpriteAnimation.playFixAnimation("NormalState", 0);
			daemonKing.handObject.SetActive(false);
			daemonKing.maskObject.SetActive(false);
			yield return new WaitForSeconds(0.5f);
			daemonKing.shiningObject.SetActive(true);
			yield return new WaitForSeconds(1.1f);
			createTypingText().SetText(TypingText.SpeakerType.Boss, "red", I18NManager.Get("THEMEEVENT10_SCRIPT_3"), 8f, 0.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(1.3f);
			textListClear();
			ObjectPool.Spawn("@StartledEffect", new Vector2(1.228f, -2.79f), Vector3.zero, new Vector3(1f, 1f, 1f));
			princessObject.spriteAnimation.playFixAnimation("Shy", 0);
			createTypingText().SetText(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT10_SCRIPT_4"), 8f, 0.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(1f);
			daemonKing.shiningObject.SetActive(false);
			textListClear();
			warrior.playBoneAnimation(warrior.currentBoneAnimationName.idleName[0]);
			yield return new WaitForSeconds(0.5f);
			warrior.setDirection(MovingObject.Direction.Right);
			createTypingText().SetText(TypingText.SpeakerType.Warrior, "orange", I18NManager.Get("THEMEEVENT10_SCRIPT_5"), 15f, 0.3f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(1f);
			createTypingText().SetText(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT10_SCRIPT_6"), 8f, 0.3f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(0.5f);
			textListClear();
			princessObject.spriteAnimation.playAnimation("Move", 0.1f, true);
			princessObject.spriteAnimation.targetRenderer.sortingOrder = 2;
			princessObject.moveTo(new Vector2(-1.056f, princessObject.cachedTransform.position.y), 10f, delegate
			{
				princessObject.spriteAnimation.playFixAnimation("Idle", 0);
			});
			yield return new WaitUntil(() => princessObject.cachedTransform.position.x < warrior.cachedTransform.position.x + 0.2f);
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			warrior.playBoneAnimation(warrior.currentBoneAnimationName.dieName[0]);
			warrior.characterBoneSpriteRendererData.headSpriteRenderer.sprite = Singleton<CharacterManager>.instance.getWarriorHeadDieSprite(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin);
			ObjectPool.Spawn("@StartledEffect", new Vector2(2.79f, -2.642f), Vector3.zero, new Vector3(1f, 1f, 1f));
			ObjectPool.Spawn("@StartledEffect", new Vector2(1.888f, -2.79f), Vector3.zero, new Vector3(1f, 1f, 1f));
			ObjectPool.Spawn("@StartledEffect", new Vector2(-1.05f, -1.84f), Vector3.zero, new Vector3(-1f, 1f, 1f));
			daemonKing.cachedSpriteAnimation.playFixAnimation("Startled", 0);
			yield return new WaitForSeconds(0.5f);
			princessObject.setDirection(MovingObject.Direction.Right);
			yield return new WaitForSeconds(1f);
			daemonKing.cachedSpriteAnimation.playFixAnimation("NormalState", 0);
			createTypingText().SetText(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT10_SCRIPT_7"), 10f, 0.35f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(1.2f);
			textListClear();
			princessObject.setDirection(MovingObject.Direction.Left);
			createTypingText().SetText(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT10_SCRIPT_8"), 14f, 0.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			Singleton<CachedManager>.instance.coverUI.fadeOutIn(0.3f, delegate
			{
				textListClear();
				princessObject.setDirection(MovingObject.Direction.Left);
				princessObject.cachedTransform.SetParent(daemonKing.cachedTransform);
				princessObject.cachedTransform.localPosition = new Vector2(-0.714f, 1.902f);
				princessObject.spriteAnimation.playFixAnimation("Shy", 0);
				princessObject.spriteAnimation.targetRenderer.sortingOrder = -2;
				daemonKing.cachedSpriteAnimation.playFixAnimation("Move3", 0);
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(0.8f);
			daemonKing.setDirection(MovingObject.Direction.Left);
			createTypingText().SetText(TypingText.SpeakerType.Princess, "lightblue", I18NManager.Get("THEMEEVENT10_SCRIPT_9"), 14f, 0.5f, delegate
			{
				eventProcess102 = true;
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			yield return new WaitForSeconds(0.7f);
			textListClear();
			daemonKing.cachedSpriteAnimation.playAnimation("Move3", 0.1f, true);
			daemonKing.moveTo(new Vector2(-13f, daemonKing.cachedTransform.position.y), 2.5f);
			yield return new WaitForSeconds(1.5f);
			createTypingText().SetText(TypingText.SpeakerType.Priest, "magenta", I18NManager.Get("THEMEEVENT10_SCRIPT_10"), 14f, 0.2f, delegate
			{
				createTypingText().SetText(TypingText.SpeakerType.Archer, "green", I18NManager.Get("THEMEEVENT10_SCRIPT_6"), 14f, 1f, delegate
				{
					eventProcess102 = true;
				});
			});
			yield return new WaitUntil(() => eventProcess102);
			eventProcess102 = false;
			break;
		}
		}
		endThemeEvent();
	}

	public void OnClickEndThemeEvent()
	{
		if (!m_isSkip)
		{
			endThemeEvent();
		}
	}

	public void endThemeEvent()
	{
		isThemeEvent = false;
		m_isSkip = true;
		StopCoroutine("themeEvent");
		textListClear();
		Singleton<CachedManager>.instance.coverUI.fadeOutGame(true, false, delegate
		{
			Singleton<ScreenProtectManager>.instance.setBlock(true);
			Singleton<CharacterManager>.instance.warriorCharacter.characterBoneSpriteRendererData.headSpriteRenderer.sprite = Singleton<CharacterManager>.instance.getWarriorHeadSprite(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin);
			for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
			{
				if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
				{
					for (int j = 0; j < Singleton<CharacterManager>.instance.constCharacterList[i].characterSpriteRenderers.Length; j++)
					{
						Singleton<CharacterManager>.instance.constCharacterList[i].characterSpriteRenderers[j].color = new Color(1f, 1f, 1f, 1f);
					}
				}
			}
			if (Singleton<CachedManager>.instance.princess != null)
			{
				Singleton<CachedManager>.instance.princess.cachedTransform.localScale = Vector3.one;
				Singleton<CachedManager>.instance.princess.spriteAnimation.targetRenderer.color = new Color(1f, 1f, 1f, 1f);
			}
			themeEventBlock.SetActive(true);
		});
		isThemeClearEvent = false;
	}
}
