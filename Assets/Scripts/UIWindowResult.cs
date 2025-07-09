using System;
using System.Collections;
using System.Collections.Generic;
using Fabric.Answers;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowResult : UIWindow
{
	public static UIWindowResult instance;

	public Image background;

	public GameObject bossRaidUIObject;

	public GameObject goldDungeonUIObject;

	public GameObject normalUIObject;

	public Text decreasStageText;

	public RectTransform recordObject;

	public RectTransform resultTitleObject;

	public RectTransform resultRewardObject;

	public RectTransform buttonSetForBossRaid;

	public CanvasGroup buttonSetCanvasGroupForBossRaid;

	public RectTransform openAllChestButtonObject;

	public RectTransform openChestButtonObject;

	public RectTransform goToTownButtonObjectForBossRaid;

	public Text stageTextForBossRaid;

	public Text levelTextForBossRaid;

	public Text monsterNameTextForBossRaid;

	public Image monsterImageForBossRaid;

	public Text bronzeChestCountTextForBossRaid;

	public Text goldChestCountTextForBossRaid;

	public Text diaChestCountTextForBossRaid;

	public GameObject haveRecordObjectForBossRaid;

	public GameObject noHaveRecordObjectForBossRaid;

	public Animation bronzeChestAnimation;

	public Animation goldChestAnimation;

	public Animation diaChestAnimation;

	public GameObject defeatedTitleObject;

	public GameObject titleObject;

	public RectTransform titleTransform;

	public CanvasGroup titleAlpha;

	public AnimationCurve titleAnimCurve;

	public RectTransform resultTransform;

	public RawImage warriorImage;

	public RawImage priestImage;

	public RawImage archerImage;

	public RectTransform bubbleTrnasform;

	public Text bubbleText;

	public float messageIndex;

	private string message = string.Empty;

	private int prevIndex;

	private bool endTyping;

	public RectTransform rewardObject;

	public ChangeNumberAnimate goldText;

	public ChangeNumberAnimate rubyText;

	public Text transcendStoneText;

	public Text[] elopeResourcesTexts;

	public Image jackpotImage;

	public RectTransform jackpotObject;

	public GameObject jackpotpanel;

	public Image jackpotGauge;

	public Text jackpotText;

	public GameObject lotteryNotice;

	public GameObject jackpotFullIconImageObject;

	public GameObject jackpotNormalImageObject;

	public Sprite[] jackpotSprites;

	private float jackpotValue;

	public RectTransform buttonGroup;

	public CanvasGroup buttonAlpha;

	public AnimationCurve buttonAnimCurve;

	public GameObject normalStageButtonSet;

	public GameObject bossStageButtonSet;

	public GameObject failButtonSet;

	public GameObject lastBossClearButtonSet;

	public bool isLastBossClear;

	public Text defeatedDescriptionText;

	public Text[] nextStageTexts;

	public Text[] nextStageTimerTexts;

	public float nextStageMaxTimer;

	public Animation titleAnimation;

	public RectTransform titleImageTransform;

	public RectTransform sunBurstEffectTransform;

	public RectTransform sunBurstEffectTransformForBossRaid;

	public bool isClear;

	private int m_ItemCount;

	private bool m_isBoss;

	public bool isCanContinue;

	private bool m_isCanClickJackpot;

	private int m_targetNextTheme;

	private int m_targetNextStage;

	private int m_originTheme;

	private float fillAmount = 1f;

	private float m_totalDecreasedProgressTimer;

	private readonly float bonusDungeonCooltime = 60f;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		jackpotpanel.SetActive(false);
		return base.OnBeforeOpen();
	}

	public override bool OnBeforeClose()
	{
		StopAllCoroutines();
		Singleton<DataManager>.instance.saveData();
		return base.OnBeforeClose();
	}

	public void ResultGame(bool clear, bool isBoss)
	{
		if (Singleton<GameManager>.instance.isGameOver)
		{
			return;
		}
		Singleton<SkillManager>.instance.isClonedWarrior = false;
		isCanContinue = false;
		if (TutorialManager.isTutorial)
		{
			Singleton<TutorialManager>.instance.disableAllButtons();
		}
		UIWindowIngame.instance.StopCoroutine("bossWarningUpdate");
		UIWindowIngame.instance.StopCoroutine("bossWarningLabelUpdate");
		UIWindowIngame.instance.bossWarning.SetActive(false);
		UIWindowIngame.instance.hpWarning.gameObject.SetActive(false);
		m_originTheme = GameManager.getRealThemeNumber(GameManager.currentTheme);
		Singleton<DataManager>.instance.syncWithDataManager();
		isLastBossClear = false;
		lastBossClearButtonSet.SetActive(false);
		failButtonSet.SetActive(false);
		normalStageButtonSet.SetActive(false);
		bossStageButtonSet.SetActive(false);
		if (clear)
		{
			nextStageMaxTimer = 5f;
		}
		else
		{
			nextStageMaxTimer = 7f;
		}
		for (int i = 0; i < nextStageTimerTexts.Length; i++)
		{
			nextStageTimerTexts[i].text = nextStageMaxTimer.ToString();
		}
		if (clear)
		{
			if (GameManager.currentStage % Singleton<MapManager>.instance.maxStage == 0 && Singleton<DataManager>.instance.currentGameData.bestTheme == GameManager.currentTheme)
			{
				lastBossClearButtonSet.SetActive(true);
				isLastBossClear = true;
			}
			else
			{
				normalStageButtonSet.SetActive(true);
			}
		}
		else
		{
			failButtonSet.SetActive(true);
			if (!BossRaidManager.isBossRaid)
			{
				int num = Singleton<DataManager>.instance.currentGameData.unlockStage - 2;
				if (num <= 0)
				{
					if (Singleton<DataManager>.instance.currentGameData.unlockTheme > 1)
					{
						Singleton<DataManager>.instance.currentGameData.unlockTheme--;
						Singleton<DataManager>.instance.currentGameData.unlockStage = Singleton<MapManager>.instance.maxStage - Mathf.Abs(num);
					}
					else
					{
						Singleton<DataManager>.instance.currentGameData.unlockTheme = 1;
						Singleton<DataManager>.instance.currentGameData.unlockStage = 1;
					}
				}
				else
				{
					Singleton<DataManager>.instance.currentGameData.unlockStage = num;
				}
				Singleton<GameManager>.instance.setMaxLimitTheme();
				Singleton<DataManager>.instance.saveData();
				decreasStageText.text = string.Format(I18NManager.Get("DECREASE_STAGE_NUMBER"), " <size=24><color=#FAD725>" + GameManager.currentTheme + "-" + GameManager.currentStage + "</color></size>");
			}
		}
		Singleton<GameManager>.instance.isGameOver = true;
		m_isBoss = isBoss;
		isClear = clear;
		m_isCanClickJackpot = false;
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon)
		{
			Singleton<CachedManager>.instance.warriorTextureRendererCamera.initCamera(Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform);
			Singleton<CachedManager>.instance.priestTextureRendererCamera.initCamera(Singleton<CharacterManager>.instance.priestCharacter.cachedTransform);
			Singleton<CachedManager>.instance.archerTextureRendererCamera.initCamera(Singleton<CharacterManager>.instance.archerCharacter.cachedTransform);
			ResultOpen(isClear);
			bossRaidUIObject.SetActive(false);
			normalUIObject.SetActive(true);
			goldDungeonUIObject.SetActive(false);
			StartCoroutine("HeroMovingUpdate");
		}
		else if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.ClearBossRaid, new Dictionary<string, string>
			{
				{
					"ClearStage",
					Singleton<BossRaidManager>.instance.lastKillBossData.stage.ToString()
				},
				{
					"BronzeChest",
					Singleton<BossRaidManager>.instance.collectedBronzeChestList.Count.ToString()
				},
				{
					"GoldChest",
					Singleton<BossRaidManager>.instance.collectedGoldChestList.Count.ToString()
				},
				{
					"DiamondChest",
					Singleton<BossRaidManager>.instance.collectedDiaChestList.Count.ToString()
				}
			});
			if (Singleton<BossRaidManager>.instance.isDoubleSpeed)
			{
				Singleton<GameManager>.instance.setTimeScale(true);
			}
			ResultOpen(isClear);
			bossRaidUIObject.SetActive(true);
			StartCoroutine("bossRaidEndUpdate");
			goldDungeonUIObject.SetActive(false);
			normalUIObject.SetActive(false);
		}
		if (!BossRaidManager.isBossRaid)
		{
			if (clear)
			{
				if (Singleton<DataManager>.instance.currentGameData.currentStage >= Singleton<MapManager>.instance.maxStage)
				{
					m_targetNextTheme = Singleton<DataManager>.instance.currentGameData.currentTheme + 1;
					m_targetNextStage = 1;
				}
				else
				{
					m_targetNextTheme = Singleton<DataManager>.instance.currentGameData.currentTheme;
					m_targetNextStage = Singleton<DataManager>.instance.currentGameData.currentStage + 1;
				}
				if (m_targetNextTheme >= GameManager.maxTheme + 1)
				{
					m_targetNextTheme = Math.Min(m_targetNextTheme, GameManager.maxTheme);
					m_targetNextStage = 10;
				}
				for (int j = 0; j < nextStageTexts.Length; j++)
				{
					nextStageTexts[j].text = "Stage " + m_targetNextTheme + "-" + m_targetNextStage;
				}
			}
			else
			{
				for (int k = 0; k < nextStageTexts.Length; k++)
				{
					nextStageTexts[k].text = "Stage " + GameManager.currentTheme + "-" + GameManager.currentStage;
				}
				defeatedDescriptionText.text = string.Format(I18NManager.Get("DESCREASED_STAGE"), GameManager.currentTheme + "-" + GameManager.currentStage);
			}
		}
		string level = "Boss Dungeon";
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon)
		{
			level = string.Format("Theme {0}, Stage {1}", GameManager.currentTheme, GameManager.currentStage);
		}
		Answers.LogLevelEnd(level, Singleton<MapManager>.instance.maxMonsterCount, isClear);
	}

	private IEnumerator bossRaidEndUpdate()
	{
		isCanContinue = false;
		sunBurstEffectTransformForBossRaid.localScale = new Vector3(0f, 0f, 1f);
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
		resultTitleObject.localScale = Vector2.zero;
		resultTitleObject.anchoredPosition = new Vector2(0f, 92f / GameManager.timeScale);
		recordObject.gameObject.SetActive(false);
		resultRewardObject.gameObject.SetActive(false);
		bronzeChestCountTextForBossRaid.text = string.Empty;
		goldChestCountTextForBossRaid.text = string.Empty;
		diaChestCountTextForBossRaid.text = string.Empty;
		BossRaidManager.BossRaidBestRecordData lastKillData = Singleton<BossRaidManager>.instance.lastKillBossData;
		if (lastKillData.isInitialized)
		{
			haveRecordObjectForBossRaid.SetActive(true);
			noHaveRecordObjectForBossRaid.SetActive(false);
			stageTextForBossRaid.text = "stage " + lastKillData.stage;
			levelTextForBossRaid.text = "lv." + lastKillData.monsterLevel;
			if (lastKillData.isMiniBossMonster)
			{
				monsterNameTextForBossRaid.text = I18NManager.Get(lastKillData.monsterType.ToString().ToUpper() + "_NAME");
				monsterImageForBossRaid.sprite = Singleton<EnemyManager>.instance.getMonsterIconSprite(lastKillData.monsterType);
				monsterImageForBossRaid.transform.localScale = new Vector3(4f, 4f, 1f);
			}
			else if (lastKillData.isBossMonster)
			{
				monsterNameTextForBossRaid.text = I18NManager.Get(lastKillData.bossType.ToString().ToUpper() + "_NAME");
				monsterImageForBossRaid.sprite = Singleton<EnemyManager>.instance.getBossIconSprite(lastKillData.bossType, true);
				monsterImageForBossRaid.transform.localScale = new Vector3(2f, 2f, 1f);
			}
			monsterImageForBossRaid.SetNativeSize();
		}
		else
		{
			haveRecordObjectForBossRaid.SetActive(false);
			noHaveRecordObjectForBossRaid.SetActive(true);
		}
		openChestButtonObject.gameObject.SetActive(false);
		openAllChestButtonObject.gameObject.SetActive(false);
		goToTownButtonObjectForBossRaid.gameObject.SetActive(false);
		bronzeChestAnimation.transform.localScale = new Vector3(0f, 0f, 1f);
		goldChestAnimation.transform.localScale = new Vector3(0f, 0f, 1f);
		diaChestAnimation.transform.localScale = new Vector3(0f, 0f, 1f);
		if (Singleton<BossRaidManager>.instance.collectedBronzeChestList.Count + Singleton<BossRaidManager>.instance.collectedGoldChestList.Count + Singleton<BossRaidManager>.instance.collectedDiaChestList.Count > 0)
		{
			openChestButtonObject.gameObject.SetActive(true);
			openAllChestButtonObject.gameObject.SetActive(true);
			goToTownButtonObjectForBossRaid.gameObject.SetActive(false);
		}
		else
		{
			openChestButtonObject.gameObject.SetActive(false);
			openAllChestButtonObject.gameObject.SetActive(false);
			goToTownButtonObjectForBossRaid.gameObject.SetActive(true);
		}
		buttonSetCanvasGroupForBossRaid.alpha = 0f;
		buttonSetForBossRaid.localScale = new Vector3(1f, 0f, 1f);
		sunBurstEffectTransform.localScale = new Vector3(0f, 0f, 1f);
		background.color = new Color(0f, 0f, 0f, 0f);
		Color backgroundColor;
		while (true)
		{
			backgroundColor = background.color;
			backgroundColor.a += Time.deltaTime * 1.2f;
			background.color = backgroundColor;
			if (backgroundColor.a >= 0.9f)
			{
				break;
			}
			yield return null;
		}
		backgroundColor.a = 0.9f;
		background.color = backgroundColor;
		resultTitleObject.GetComponent<Animation>().Play("StageClearTitleAnimation");
		yield return new WaitWhile(() => resultTitleObject.GetComponent<Animation>().isPlaying);
		yield return new WaitForSeconds(0.2f / GameManager.timeScale);
		Vector3 sunBurstScale = Vector2.zero;
		while (true)
		{
			resultTitleObject.anchoredPosition = Vector2.Lerp(resultTitleObject.anchoredPosition, new Vector2(0f, 304f), Time.deltaTime * 23f);
			sunBurstScale = sunBurstEffectTransformForBossRaid.localScale;
			sunBurstScale.x += Time.deltaTime * 8.5f;
			sunBurstScale.y += Time.deltaTime * 8.5f;
			sunBurstEffectTransformForBossRaid.localScale = sunBurstScale;
			if (sunBurstScale.x >= 1f)
			{
				sunBurstEffectTransformForBossRaid.localScale = Vector3.one;
			}
			if (Vector2.Distance(resultTitleObject.anchoredPosition, new Vector2(0f, 304f)) <= 10f)
			{
				break;
			}
			yield return null;
		}
		resultTitleObject.anchoredPosition = new Vector2(0f, 304f / GameManager.timeScale);
		Singleton<AudioManager>.instance.playEffectSound("result_clear");
		yield return new WaitForSeconds(0.4f / GameManager.timeScale);
		recordObject.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.4f / GameManager.timeScale);
		resultRewardObject.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.3f / GameManager.timeScale);
		bronzeChestAnimation.Play();
		yield return new WaitForSeconds(0.25f / GameManager.timeScale);
		goldChestAnimation.Play();
		yield return new WaitForSeconds(0.25f / GameManager.timeScale);
		diaChestAnimation.Play();
		yield return new WaitForSeconds(0.7f / GameManager.timeScale);
		while (true)
		{
			if (buttonSetCanvasGroupForBossRaid.alpha < 1f)
			{
				buttonSetCanvasGroupForBossRaid.alpha += Time.deltaTime * 1.5f;
			}
			else
			{
				buttonSetCanvasGroupForBossRaid.alpha = 1f;
			}
			buttonSetForBossRaid.localScale = Vector3.Lerp(buttonSetForBossRaid.localScale, Vector3.one, Time.deltaTime * 8f);
			if (buttonSetCanvasGroupForBossRaid.alpha >= 1f && buttonSetForBossRaid.localScale.y >= 0.95f)
			{
				break;
			}
			yield return null;
		}
		buttonSetCanvasGroupForBossRaid.alpha = 1f;
		buttonSetForBossRaid.localScale = Vector3.one;
		isCanContinue = true;
	}

	private IEnumerator HeroMovingUpdate()
	{
		Singleton<CharacterManager>.instance.warriorCharacter.setStateLock(true);
		isCanContinue = false;
		titleTransform.localScale = Vector2.one;
		titleAlpha.alpha = (isClear ? 1 : 0);
		titleObject.SetActive(false);
		if (isClear)
		{
			warriorImage.transform.localScale = Vector2.zero;
			priestImage.transform.localScale = Vector2.zero;
			archerImage.transform.localScale = Vector2.zero;
			titleImageTransform.anchoredPosition = new Vector2(0f, -330f);
			titleImageTransform.localScale = Vector2.zero;
			warriorImage.transform.localPosition = new Vector3(-120f, 4f, 0f);
			priestImage.transform.localPosition = new Vector3(0f, 4f, 0f);
			archerImage.transform.localPosition = new Vector3(120f, 4f, 0f);
			priestImage.gameObject.SetActive(true);
			archerImage.gameObject.SetActive(true);
		}
		else
		{
			warriorImage.transform.localScale = Vector2.one * 7f;
			bubbleTrnasform.anchoredPosition = new Vector2(0f, 881.4f / GameManager.timeScale);
			warriorImage.transform.localPosition = new Vector3(0f, 80f);
			priestImage.gameObject.SetActive(false);
			archerImage.gameObject.SetActive(false);
		}
		bubbleTrnasform.localScale = Vector2.zero;
		rewardObject.localScale = Vector2.zero;
		jackpotObject.localScale = Vector2.zero;
		buttonAlpha.alpha = 0f;
		background.color = new Color(0f, 0f, 0f, 0f);
		sunBurstEffectTransform.localScale = Vector2.zero;
		float timer4 = 0f;
		if (isClear)
		{
			Color backgroundColor;
			while (true)
			{
				backgroundColor = background.color;
				backgroundColor.a += Time.deltaTime * 1.2f * GameManager.timeScale;
				background.color = backgroundColor;
				if (backgroundColor.a >= 0.9f)
				{
					break;
				}
				yield return null;
			}
			backgroundColor.a = 0.9f;
			background.color = backgroundColor;
			titleObject.SetActive(true);
			yield return new WaitForSeconds(0.2f / GameManager.timeScale);
		}
		if (isClear)
		{
			while (Vector2.Distance(warriorImage.transform.localScale, Vector2.one * 6f) > 0.25f || Vector2.Distance(titleImageTransform.anchoredPosition, new Vector2(0f, -97f)) > 0.25f)
			{
				warriorImage.transform.localScale = Vector2.Lerp(warriorImage.transform.localScale, Vector2.one * 6f, Time.deltaTime * 20f * GameManager.timeScale);
				priestImage.transform.localScale = Vector2.Lerp(priestImage.transform.localScale, Vector2.one * 6f, Time.deltaTime * 20f * GameManager.timeScale);
				archerImage.transform.localScale = Vector2.Lerp(archerImage.transform.localScale, Vector2.one * 6f, Time.deltaTime * 20f * GameManager.timeScale);
				titleImageTransform.anchoredPosition = Vector2.Lerp(titleImageTransform.anchoredPosition, new Vector2(0f, -97f), Time.deltaTime * 10f * GameManager.timeScale);
				yield return null;
			}
			warriorImage.transform.localScale = Vector2.one * 6f;
			priestImage.transform.localScale = Vector2.one * 6f;
			archerImage.transform.localScale = Vector2.one * 6f;
			titleTransform.localScale = Vector3.one;
		}
		else
		{
			float startDist = Vector2.Distance(warriorImage.transform.localPosition, new Vector2(0f, 80f));
			while (true)
			{
				float dist = Vector2.Distance(warriorImage.transform.localPosition, new Vector2(0f, 80f));
				if (!(Mathf.Abs(dist) > 1.5f))
				{
					break;
				}
				warriorImage.transform.localScale = Vector2.Lerp(warriorImage.transform.localScale, new Vector2(7f, 7f), Time.deltaTime * 2f * GameManager.timeScale);
				warriorImage.transform.localPosition = Vector2.Lerp(warriorImage.transform.localPosition, new Vector2(0f, 80f), Time.deltaTime * 3f * GameManager.timeScale);
				background.color = new Color(0f, 0f, 0f, 0.9f * ((startDist - dist) / startDist));
				yield return null;
			}
			titleTransform.localScale = Vector3.one;
			warriorImage.transform.localScale = Vector2.one * 7f;
			warriorImage.transform.localPosition = new Vector2(0f, 80f);
			background.color = new Color(0f, 0f, 0f, 0.9f / GameManager.timeScale);
		}
		Singleton<AdsAngelManager>.instance.checkSpawnAngel();
		if (isClear)
		{
			Singleton<AudioManager>.instance.playEffectSound("result_clear");
			Singleton<CharacterManager>.instance.warriorCharacter.playBoneAnimation(Singleton<CharacterManager>.instance.warriorCharacter.currentBoneAnimationName.skillName[0]);
			yield return new WaitForSeconds(0.2f / GameManager.timeScale);
			Singleton<CharacterManager>.instance.priestCharacter.playBoneAnimation(Singleton<CharacterManager>.instance.priestCharacter.currentBoneAnimationName.skillName[0]);
			yield return new WaitForSeconds(0.2f / GameManager.timeScale);
			Singleton<CharacterManager>.instance.archerCharacter.playBoneAnimation(Singleton<CharacterManager>.instance.archerCharacter.currentBoneAnimationName.skillName[0]);
			Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
			Vector3 sunBurstScale = Vector2.zero;
			while (true)
			{
				sunBurstScale = sunBurstEffectTransform.localScale;
				sunBurstScale.x += Time.deltaTime * 6f * GameManager.timeScale;
				sunBurstScale.y += Time.deltaTime * 6f * GameManager.timeScale;
				sunBurstEffectTransform.localScale = sunBurstScale;
				if (sunBurstScale.x >= 1f)
				{
					break;
				}
				yield return null;
			}
			sunBurstEffectTransform.localScale = Vector3.one;
		}
		else
		{
			while (true)
			{
				timer4 += Time.deltaTime * GameManager.timeScale * 5f;
				titleAlpha.alpha = timer4;
				if (timer4 > 1f)
				{
					break;
				}
				yield return null;
			}
			timer4 = 1f;
			titleAlpha.alpha = timer4;
			timer4 = 0f;
			Singleton<AudioManager>.instance.playEffectSound("result_fail");
			int maxTipCount;
			for (maxTipCount = 1; I18NManager.ContainsKey("RESULT_TIP_" + maxTipCount); maxTipCount++)
			{
			}
			message = I18NManager.Get("RESULT_TIP_" + UnityEngine.Random.Range(1, maxTipCount));
			messageIndex = 0f;
			prevIndex = 0;
			endTyping = false;
			while (true)
			{
				timer4 += Time.deltaTime * GameManager.timeScale * 5f;
				warriorImage.transform.localScale = Vector2.one * (1f + 5f * timer4);
				bubbleTrnasform.transform.localScale = Vector2.one * timer4;
				if (timer4 > 1f)
				{
					break;
				}
				yield return null;
			}
			timer4 = 1f;
			warriorImage.transform.localScale = Vector2.one * (1f + 5f * timer4);
			bubbleTrnasform.transform.localScale = Vector2.one * timer4;
			timer4 = 0f;
		}
		if (isClear)
		{
			timer4 = 0f;
			while (timer4 < 0.4f)
			{
				timer4 += Time.deltaTime * GameManager.timeScale;
				yield return null;
			}
		}
		else
		{
			titleAnimation.Play("StageFailTitleAnimation");
		}
		while (0.95f > rewardObject.localScale.x)
		{
			rewardObject.localScale = Vector2.Lerp(rewardObject.localScale, Vector2.one, Time.deltaTime * 15f * GameManager.timeScale);
			yield return null;
		}
		rewardObject.localScale = Vector2.one;
		while (0.95f > jackpotObject.localScale.x)
		{
			jackpotObject.localScale = Vector2.Lerp(jackpotObject.localScale, Vector2.one, Time.deltaTime * 15f * GameManager.timeScale);
			yield return null;
		}
		jackpotObject.localScale = Vector2.one;
		timer4 = 0f;
		while (timer4 < 0.5f)
		{
			if (jackpotValue < Singleton<DataManager>.instance.currentGameData.jackpotValue)
			{
				float lerpFillAmount = jackpotGauge.fillAmount;
				if (lerpFillAmount != fillAmount)
				{
					lerpFillAmount = ((!(fillAmount > lerpFillAmount)) ? (lerpFillAmount - Time.deltaTime * GameManager.timeScale * 0.25f) : (lerpFillAmount + Time.deltaTime * GameManager.timeScale * 0.25f));
				}
				if (Mathf.Abs(lerpFillAmount - fillAmount) <= Time.deltaTime * GameManager.timeScale * 0.25f)
				{
					lerpFillAmount = fillAmount;
				}
				jackpotValue = Mathf.CeilToInt(lerpFillAmount * 100f);
				jackpotGauge.fillAmount = lerpFillAmount;
				jackpotText.text = string.Format("{0:n0}%", jackpotValue);
				refreshJackpotButton();
			}
			Singleton<AudioManager>.instance.playEffectSound("getresource", AudioManager.EffectType.Resource);
			timer4 += Time.deltaTime * GameManager.timeScale;
			yield return null;
		}
		jackpotValue = Singleton<DataManager>.instance.currentGameData.jackpotValue;
		refreshJackpotButton();
		jackpotGauge.fillAmount = jackpotValue * 0.01f;
		jackpotText.text = string.Format("{0:n0}%", jackpotValue);
		timer4 = 0f;
		while (true)
		{
			timer4 += Time.deltaTime * GameManager.timeScale * 2f;
			if (timer4 > 1f)
			{
				break;
			}
			buttonAlpha.alpha = timer4;
			buttonGroup.anchoredPosition = new Vector2(0f, -390.8f * buttonAnimCurve.Evaluate(timer4));
			yield return null;
		}
		timer4 = 1f;
		buttonAlpha.alpha = timer4;
		buttonGroup.anchoredPosition = new Vector2(0f, -390.8f * buttonAnimCurve.Evaluate(timer4));
		buttonAlpha.alpha = 1f;
		timer4 = 0f;
		m_isCanClickJackpot = true;
		yield return new WaitForSeconds(0.5f / GameManager.timeScale);
		isCanContinue = true;
		if (!TutorialManager.isTutorial)
		{
			bool isHaveFingers = Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount > 0 || Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount > 0 || Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime > UnbiasedTime.Instance.Now().Ticks || Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime > UnbiasedTime.Instance.Now().Ticks || Singleton<DataManager>.instance.currentGameData.isBoughtBronzeFinger;
			if (Util.isInternetConnection() && Singleton<AdsManager>.instance.isAdmobEnableCountry && !ShopManager.isPaidUser && !isHaveFingers && Singleton<DataManager>.instance.currentGameData.admobAdsIngnoreTargetTime <= UnbiasedTime.Instance.UtcNow().Ticks)
			{
				double randomForAdmob = (double)UnityEngine.Random.Range(0, 10000) / 100.0;
				if (randomForAdmob < 100.0)
				{
					UIWindowAdmobAds.instance.openAdmobAdsUI();
				}
				else
				{
					startNextTimer();
				}
			}
			else
			{
				startNextTimer();
			}
		}
		else
		{
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType11);
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType18);
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType24);
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType30);
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType40);
		}
	}

	private IEnumerator escUpdate()
	{
		while (true)
		{
			if (isCanContinue && Input.GetKeyDown(KeyCode.Escape) && UIWindow.GetTopWindow() == instance && !UIWindowDialog.instances[0].cachedGameObject.activeSelf && !UIWindowDialog.instances[1].cachedGameObject.activeSelf && !UIWindowJackPot.instance.cachedGameObject.activeSelf && !UIWindowAdsAngelBuffDialog.instance.cachedGameObject.activeSelf)
			{
				UIWindowDialog.openDescription("INGAME_QUIT_QUESTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					OnClickContinue();
					instance.StopCoroutine("escUpdate");
				}, string.Empty);
			}
			yield return null;
		}
	}

	private void Update()
	{
		if (endTyping || string.IsNullOrEmpty(message))
		{
			return;
		}
		messageIndex += Time.deltaTime * 20f * GameManager.timeScale;
		messageIndex = Mathf.Min(message.Length, messageIndex);
		if (prevIndex != (int)messageIndex)
		{
			prevIndex = Mathf.Min(message.Length, (int)messageIndex);
			bubbleText.text = message.Substring(0, prevIndex);
			char c = message[Mathf.Clamp((int)messageIndex - 1, 0, message.Length - 1)];
			if (c == '\n')
			{
			}
			if ((float)message.Length == messageIndex)
			{
				endTyping = true;
			}
		}
	}

	public void ResultOpen(bool clear)
	{
		open();
		if (BossRaidManager.isBossRaid)
		{
			return;
		}
		StopCoroutine("escUpdate");
		goldText.SetText(0.0);
		rubyText.SetText(0.0);
		transcendStoneText.text = "0";
		Singleton<CachedManager>.instance.warriorTextureRendererCamera.setAcviveCamera(true);
		Singleton<CachedManager>.instance.priestTextureRendererCamera.setAcviveCamera(true);
		Singleton<CachedManager>.instance.archerTextureRendererCamera.setAcviveCamera(true);
		if (clear)
		{
			if (!isLastBossClear)
			{
				StartCoroutine("escUpdate");
			}
			for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
			{
				if (Singleton<CharacterManager>.instance.constCharacterList[i] != null && Singleton<CharacterManager>.instance.constCharacterList[i].getState() != PublicDataManager.State.Wait)
				{
					Singleton<CharacterManager>.instance.constCharacterList[i].setState(PublicDataManager.State.Wait);
				}
			}
			defeatedTitleObject.SetActive(false);
			resultTransform.anchoredPosition = new Vector2(0f, 0f);
			AnalyzeManager.retention(AnalyzeManager.CategoryType.Dungeon, AnalyzeManager.ActionType.Clear, new Dictionary<string, string>
			{
				{
					"Theme",
					GameManager.currentTheme.ToString()
				},
				{
					"Stage",
					GameManager.currentStage.ToString()
				}
			});
		}
		else
		{
			StartCoroutine("escUpdate");
			defeatedTitleObject.SetActive(true);
			titleObject.SetActive(false);
			resultTransform.anchoredPosition = Vector2.zero;
			AnalyzeManager.retention(AnalyzeManager.CategoryType.Dungeon, AnalyzeManager.ActionType.Fail, new Dictionary<string, string>
			{
				{
					"Theme",
					GameManager.currentTheme.ToString()
				},
				{
					"Stage",
					GameManager.currentStage.ToString()
				}
			});
		}
		for (int j = 0; j < elopeResourcesTexts.Length; j++)
		{
			long num = 0L;
			switch (j)
			{
			case 0:
				num = (long)Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.ElopeResource1];
				break;
			case 1:
				num = (long)Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.ElopeResource2];
				break;
			case 2:
				num = (long)Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.ElopeResource3];
				break;
			case 3:
				num = (long)Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.ElopeResource4];
				break;
			}
			elopeResourcesTexts[j].text = num.ToString("N0");
		}
		transcendStoneText.text = Singleton<TranscendManager>.instance.ingameTotalTranscendStone.ToString("N0");
		goldText.CurrentPrintType = ChangeNumberAnimate.PrintType.ChangeUnit;
		goldText.SetValue(Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.Gold], 1f);
		rubyText.CurrentPrintType = ChangeNumberAnimate.PrintType.Number;
		rubyText.SetValue(Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.Ruby], 1f);
		jackpotValue = Singleton<DataManager>.instance.currentGameData.jackpotValue;
		jackpotGauge.fillAmount = jackpotValue / 100f;
		jackpotText.text = string.Format("{0:n0}%", jackpotValue);
		refreshJackpotButton();
		jackpotpanel.SetActive(true);
		float num2 = 15f * ((!isClear) ? ((float)Singleton<GroundManager>.instance.getFloor() / (float)Singleton<MapManager>.instance.getMaxFloor(GameManager.currentTheme)) : 1f);
		if (ParsingManager.jackpotMultiplyValue > 0f)
		{
			num2 *= ParsingManager.jackpotMultiplyValue;
		}
		Singleton<DataManager>.instance.currentGameData.jackpotValue += Mathf.Clamp(num2, 0f, 100f);
		fillAmount = Singleton<DataManager>.instance.currentGameData.jackpotValue / 100f;
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.CanNotStop, 1.0);
		Singleton<QuestManager>.instance.questReport(QuestManager.QuestType.PlayTime, 1.0);
		if (isClear)
		{
			Singleton<QuestManager>.instance.questReportForStage(QuestManager.QuestType.NextStage, GameManager.currentTheme, GameManager.currentStage);
		}
	}

	public void OnClickContinue()
	{
		if (isCanContinue)
		{
			if (TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType14);
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType20);
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType26);
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType32);
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType43);
			}
			Singleton<DropItemManager>.instance.stopAutoCatchAllItems();
			isCanContinue = false;
			Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
			if (!BossRaidManager.isBossRaid && isClear)
			{
				close();
				calculateStageInformation(true);
				return;
			}
			Singleton<EnemyManager>.instance.endBossSafe();
			Singleton<CachedManager>.instance.coverUI.fadeOutGame();
			Singleton<DataManager>.instance.syncWithDataManager();
			Singleton<DataManager>.instance.saveData();
		}
	}

	public void startNextTimer()
	{
		if (!isLastBossClear)
		{
			if (isClear)
			{
				nextStageMaxTimer = 5f;
			}
			else
			{
				nextStageMaxTimer = 7f;
			}
			StopCoroutine("nextTimerUpdate");
			StartCoroutine("nextTimerUpdate");
		}
	}

	public void pauseNextTimer()
	{
		if (isClear)
		{
			nextStageMaxTimer = 5f;
		}
		else
		{
			nextStageMaxTimer = 7f;
		}
		StopCoroutine("nextTimerUpdate");
	}

	private IEnumerator nextTimerUpdate()
	{
		while (true)
		{
			if (!UIWindowJackPot.instance.isJackpotOn && !UIWindowAdsAngelBuffDialog.instance.isOpen && !isLastBossClear && !Singleton<AdsManager>.instance.IsShowingAd && !UIWindowDialog.instances[0].isOpen && !UIWindowDialog.instances[1].isOpen)
			{
				nextStageMaxTimer -= Time.deltaTime * GameManager.timeScale;
				for (int j = 0; j < nextStageTimerTexts.Length; j++)
				{
					nextStageTimerTexts[j].text = nextStageMaxTimer.ToString("N0");
				}
				if (nextStageMaxTimer <= 0f)
				{
					break;
				}
			}
			yield return null;
		}
		nextStageMaxTimer = 0f;
		for (int i = 0; i < nextStageTimerTexts.Length; i++)
		{
			nextStageTimerTexts[i].text = "0";
		}
		OnClickNext();
	}

	public void OnClickOpenChestsForBossRaid()
	{
		if (isCanContinue)
		{
			isCanContinue = false;
			UIWindowLotteryBossRaidChest.instance.openLotteryBossRaidchest(false);
		}
	}

	public void OnClickOpenAllChestsForBossRaid()
	{
		if (isCanContinue)
		{
			isCanContinue = false;
			UIWindowLotteryBossRaidChest.instance.openLotteryBossRaidchest(true);
		}
	}

	public void OnClickNext()
	{
		if (!isCanContinue)
		{
			return;
		}
		isCanContinue = false;
		bool isShouldReload = m_originTheme != GameManager.getRealThemeNumber(m_targetNextTheme);
		if (isClear)
		{
			Singleton<DropItemManager>.instance.stopAutoCatchAllItems();
			calculateStageInformation(false);
			Singleton<CachedManager>.instance.coverUI.fadeOut(0.5f, delegate
			{
				ignoreOnCloseAll = true;
				Singleton<GameManager>.instance.gameEnd(isShouldReload, true);
				GameManager.currentDungeonType = GameManager.SpecialDungeonType.NormalDungeon;
				GameManager.currentTheme = (Singleton<DataManager>.instance.currentGameData.currentTheme = m_targetNextTheme);
				GameManager.currentStage = (Singleton<DataManager>.instance.currentGameData.currentStage = m_targetNextStage);
				UIWindowLoading.instance.openLoadingUI(delegate
				{
					Action playAction2 = delegate
					{
						Singleton<EnemyManager>.instance.endBossSafe();
						Singleton<GameManager>.instance.gameStart();
						Singleton<AudioManager>.instance.playBackgroundSound("monster");
						ignoreOnCloseAll = false;
						StopCoroutine("waitForRetryLoading");
						StartCoroutine("waitForRetryLoading");
					};
					if (isShouldReload)
					{
						Singleton<ResourcesManager>.instance.IngameObjectPools(true, delegate
						{
							playAction2();
						});
					}
					else
					{
						playAction2();
					}
				});
			});
			return;
		}
		isShouldReload = m_originTheme != GameManager.getRealThemeNumber(GameManager.currentTheme);
		Singleton<CachedManager>.instance.coverUI.fadeOut(0.5f, delegate
		{
			ignoreOnCloseAll = true;
			Singleton<GameManager>.instance.gameEnd(isShouldReload, true);
			GameManager.currentDungeonType = GameManager.SpecialDungeonType.NormalDungeon;
			UIWindowLoading.instance.openLoadingUI(delegate
			{
				Action playAction = delegate
				{
					Singleton<EnemyManager>.instance.endBossSafe();
					Singleton<GameManager>.instance.gameStart();
					Singleton<AudioManager>.instance.playBackgroundSound("monster");
					ignoreOnCloseAll = false;
					instance.StopCoroutine("waitForRetryLoading");
					instance.StartCoroutine("waitForRetryLoading");
				};
				if (isShouldReload)
				{
					Singleton<ResourcesManager>.instance.IngameObjectPools(true, delegate
					{
						playAction();
					});
				}
				else
				{
					playAction();
				}
			});
		});
	}

	private void calculateStageInformation(bool withBossClearEvent)
	{
		bool flag = false;
		if (GameManager.currentStage % Singleton<MapManager>.instance.maxStage == 0 && GameManager.unlockTheme == GameManager.currentTheme)
		{
			Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.TraceOfTravels, 1.0);
			if (withBossClearEvent)
			{
				flag = true;
				if (Singleton<DataManager>.instance.currentGameData.bestTheme == Singleton<DataManager>.instance.currentGameData.unlockTheme)
				{
					pauseNextTimer();
					Singleton<GameManager>.instance.StartCoroutine("fadeOut");
				}
				else
				{
					Singleton<CachedManager>.instance.coverUI.fadeOutGame();
				}
			}
			else if (Singleton<DataManager>.instance.currentGameData.bestTheme == Singleton<DataManager>.instance.currentGameData.unlockTheme)
			{
				flag = true;
				Singleton<CachedManager>.instance.coverUI.fadeOutGame();
			}
			if (Singleton<DataManager>.instance.currentGameData.bestTheme == Singleton<DataManager>.instance.currentGameData.unlockTheme)
			{
				Singleton<DataManager>.instance.currentGameData.bestTheme = Singleton<DataManager>.instance.currentGameData.unlockTheme + 1;
			}
			Singleton<DataManager>.instance.currentGameData.unlockTheme++;
			Singleton<DataManager>.instance.currentGameData.unlockStage = 1;
			if (Singleton<DataManager>.instance.currentGameData.unlockTheme >= GameManager.maxTheme + 1)
			{
				Singleton<DataManager>.instance.currentGameData.unlockStage = Singleton<MapManager>.instance.maxStage;
			}
			AnalyzeManager.retention(AnalyzeManager.CategoryType.Unlock, AnalyzeManager.ActionType.ThemeUnlock, new Dictionary<string, string>
			{
				{
					"Theme",
					Singleton<DataManager>.instance.currentGameData.unlockTheme.ToString()
				}
			});
		}
		else if (GameManager.unlockTheme == GameManager.currentTheme)
		{
			Singleton<DataManager>.instance.currentGameData.unlockStage++;
		}
		if (withBossClearEvent && !flag)
		{
			flag = true;
			Singleton<CachedManager>.instance.coverUI.fadeOutGame();
		}
		if (Singleton<AdsAngelManager>.instance.currentAngelObject != null)
		{
			Singleton<AdsAngelManager>.instance.currentAngelObject.endAnimation();
		}
		if (Singleton<AdsAngelManager>.instance.currentSpecialAngelObject != null)
		{
			Singleton<AdsAngelManager>.instance.currentSpecialAngelObject.endAnimation();
		}
		Singleton<GameManager>.instance.currentThemeForThemeEvent = Singleton<DataManager>.instance.currentGameData.unlockTheme;
		Singleton<GameManager>.instance.setMaxLimitTheme();
		Singleton<DataManager>.instance.saveData();
	}

	public IEnumerator waitForRetryLoading()
	{
		yield return new WaitForSeconds(1f / GameManager.timeScale);
		UIWindowLoading.instance.closeLoadingUI();
		Singleton<CachedManager>.instance.coverUI.fadeInGame(1f);
		close();
	}

	public void OnClickOpenLottery()
	{
		if (!(jackpotValue < 100f) && m_isCanClickJackpot && !(Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.Gold] <= 0.0))
		{
			double goldValue = Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.Gold];
			UIWindowJackPot.instance.openJackpotUI(goldValue);
		}
	}

	private void refreshJackpotButton()
	{
		if (Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.Gold] <= 0.0)
		{
			lotteryNotice.SetActive(false);
			jackpotFullIconImageObject.SetActive(false);
			jackpotNormalImageObject.SetActive(false);
		}
		else if (jackpotValue < 100f)
		{
			lotteryNotice.SetActive(false);
			jackpotFullIconImageObject.SetActive(false);
			jackpotNormalImageObject.SetActive(true);
		}
		else
		{
			lotteryNotice.SetActive(true);
			jackpotFullIconImageObject.SetActive(true);
			jackpotNormalImageObject.SetActive(false);
		}
	}

	public void disableJackpotUI()
	{
		jackpotValue = 0f;
		jackpotGauge.fillAmount = 0f;
		jackpotText.text = "0%";
		Singleton<DataManager>.instance.currentGameData.jackpotValue = 0f;
		lotteryNotice.SetActive(false);
		jackpotFullIconImageObject.SetActive(false);
		jackpotNormalImageObject.SetActive(true);
	}
}
