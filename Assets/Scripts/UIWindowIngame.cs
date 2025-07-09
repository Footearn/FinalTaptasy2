using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowIngame : UIWindow
{
	public static UIWindowIngame instance;

	public Animation gameStartInformationAnimation;

	public CanvasGroup gameStartInformationCanvaGroup;

	public Text currentDPSText;

	public Text currentGameStartInformationText;

	public Text rescueCountText;

	public GameObject firstFloorShadow1;

	public GameObject firstFloorShadow2;

	public Text[] stageTexts;

	public GameObject PauseUI;

	public Image hpWarning;

	public bool hpWarningState;

	public GameObject bossWarning;

	public RawImage warningLabel;

	public Image warningImage;

	public bool bossWarningState;

	public SkillCooltimeObject concentrationCooltimeObject;

	public SkillCooltimeObject healingTimeCooltimeObject;

	public SkillCooltimeObject destroyTreausreChestCooltimeObject;

	public SkillCooltimeObject transcendDecreaseHitDamageCooltimeObject;

	public SkillCooltimeObject transcendIncreaseAllDamageCooltimeObject;

	public List<SkillCooltimeObject> skillCooltimeObjectList;

	public Animation skillManaCountAnim;

	public Image skillManaCountImage;

	public Image skillManaCountImage2;

	public Sprite[] skillStarCountSprites;

	public Text[] skillCostText;

	public GameObject[] skillGlowObjects;

	public GameObject[] skillBlockedObjects;

	public GameObject[] skillLockedObjects;

	private ObtainData m_prevObtainData;

	private bool m_markerSwitch;

	public IngameQuestObject questIngame;

	public GameObject uiPanel;

	public GameObject eventPanel;

	public Transform textParent;

	public bool isCanPause;

	public GameObject[] normalStageObjects;

	public GameObject specialStageObject;

	public Image startCountImage;

	public AnimationCurve startCurve;

	public GameObject skyBackground;

	public GameObject questInformationObject;

	public GameObject stageInformationObject;

	public Image currentProgressBarImage;

	public Text currentProgressPercentText;

	public Sprite[] concentrationIconSprites;

	public Sprite[] cloneWarriorIconSprites;

	public Image concentrationSkillImage;

	public TextMeshProUGUI concentrationCooltimeText;

	public Image concentrationCooltimeBackground;

	public Image cloneWarriorSkillImage;

	public TextMeshProUGUI cloneWarriorCooltimeText;

	public Image cloneWarriorCooltimeBackground;

	public Sprite normalConcentrationCoolTimeSprite;

	public Sprite normalCloneWarriorCoolTimeSprite;

	public Image concentrationCoolTimeImage;

	public Image cloneWarriorCoolTimeImage;

	public GameObject reinforcementManaObject;

	public Text reinforcementManaText;

	public override void Awake()
	{
		base.Awake();
		instance = this;
		skillCooltimeObjectList = new List<SkillCooltimeObject>();
	}

	public override bool OnBeforeOpen()
	{
		concentrationSkillImage.sprite = concentrationIconSprites[0];
		cloneWarriorSkillImage.sprite = cloneWarriorIconSprites[0];
		reinforcementManaObject.SetActive(Singleton<StatManager>.instance.weaponSkinReinforcementMana > 0.0);
		reinforcementManaText.text = "0%";
		currentDPSText.text = "0";
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			questInformationObject.SetActive(false);
			stageInformationObject.SetActive(false);
		}
		else
		{
			questInformationObject.SetActive(true);
			stageInformationObject.SetActive(true);
		}
		for (int i = 0; i < normalStageObjects.Length; i++)
		{
			normalStageObjects[i].SetActive(true);
		}
		specialStageObject.SetActive(false);
		skyBackground.SetActive(false);
		isCanPause = true;
		uiPanel.SetActive(true);
		eventPanel.SetActive(false);
		Singleton<CachedManager>.instance.themeEventGround.SetActive(false);
		bossWarning.SetActive(false);
		warningImage.color = new Color(1f, 1f, 1f, 0f);
		hpWarning.color = new Color(1f, 1f, 1f, 0f);
		return base.OnBeforeOpen();
	}

	public override void OnAfterActiveGameObject()
	{
		currentProgressBarImage.fillAmount = 0f;
		currentProgressPercentText.text = "0";
		StopCoroutine("currentProgressBarUpdate");
		StartCoroutine("currentProgressBarUpdate");
		base.OnAfterActiveGameObject();
	}

	private void OnEnable()
	{
		for (int i = 0; i < skillCooltimeObjectList.Count; i++)
		{
			skillCooltimeObjectList[i].forceClostCooltimeUI();
		}
		skillCooltimeObjectList.Clear();
		firstFloorShadow1.SetActive(true);
		firstFloorShadow2.SetActive(true);
		for (int j = 0; j < stageTexts.Length; j++)
		{
			stageTexts[j].text = "Stage " + GameManager.currentTheme + "-" + GameManager.currentStage;
		}
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			questIngame.cachedGameObject.SetActive(false);
		}
		else
		{
			questIngame.cachedGameObject.SetActive(true);
		}
		questIngame.initialize();
		gameStartInformationAnimation.Stop();
		gameStartInformationCanvaGroup.alpha = 0f;
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon)
		{
			currentGameStartInformationText.text = "Stage " + GameManager.currentStage;
			string text = GameManager.currentTheme.ToString();
			if (I18NManager.currentLanguage == I18NManager.Language.English)
			{
				text = ((GameManager.currentTheme == 1) ? (text + "st") : ((GameManager.currentTheme != 2) ? (text + "th") : (text + "nd")));
			}
			rescueCountText.text = string.Format(I18NManager.Get("RESCUE_COUNT_TEXT"), text);
			StartCoroutine("waitForOpenStageInformation");
		}
	}

	private IEnumerator currentProgressBarUpdate()
	{
		float fillAmount = 0f;
		bool isBossDead = false;
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (Singleton<EnemyManager>.instance.bossObject != null && Singleton<EnemyManager>.instance.bossObject.isDead)
				{
					isBossDead = true;
				}
				else if (!isBossDead)
				{
					fillAmount = Mathf.Lerp(fillAmount, Mathf.Max((float)Singleton<GroundManager>.instance.getFloor() - 1f, 0f) / (float)Singleton<MapManager>.instance.getMaxFloor(GameManager.currentTheme), Time.deltaTime * GameManager.timeScale * 3.5f);
				}
				if (isBossDead)
				{
					fillAmount = Mathf.Lerp(fillAmount, 1f, Time.deltaTime * GameManager.timeScale * 3.5f);
				}
				currentProgressBarImage.fillAmount = fillAmount;
				currentProgressPercentText.text = (fillAmount * 100f).ToString("N0");
			}
			yield return null;
		}
	}

	private IEnumerator waitForOpenStageInformation()
	{
		yield return new WaitForSeconds(1f);
		gameStartInformationAnimation.Play();
	}

	private IEnumerator hpWarningUpdate()
	{
		hpWarning.gameObject.SetActive(true);
		float timer = 0f;
		while (true)
		{
			timer += Time.deltaTime * GameManager.timeScale * 5f;
			hpWarning.color = new Color(1f, 1f, 1f, Mathf.PingPong(timer, 1f));
			if (!hpWarningState)
			{
				break;
			}
			yield return null;
		}
		hpWarning.gameObject.SetActive(false);
	}

	private IEnumerator bossWarningLabelUpdate()
	{
		float timer = 0f;
		while (true)
		{
			timer += Time.deltaTime * GameManager.timeScale * 0.1f;
			warningLabel.uvRect = new Rect(timer, 0f, 1f, 1f);
			yield return null;
		}
	}

	private IEnumerator bossWarningUpdate()
	{
		isCanPause = false;
		bossWarningState = true;
		bossWarning.SetActive(true);
		hpWarning.gameObject.SetActive(true);
		StartCoroutine("bossWarningLabelUpdate");
		if (hpWarningState)
		{
			StopCoroutine("hpWarningUpdate");
		}
		warningImage.color = new Color(1f, 1f, 1f, 0f);
		hpWarning.color = new Color(1f, 1f, 1f, 0f);
		float alpha = 0f;
		bool alphaTrigger = false;
		while (true)
		{
			if (!alphaTrigger)
			{
				alpha += Time.deltaTime * GameManager.timeScale * 1.85f;
				if (alpha >= 0.95f)
				{
					alphaTrigger = true;
					Singleton<AudioManager>.instance.playEffectSound("monster_boss_incounter");
				}
			}
			else
			{
				alpha -= Time.deltaTime * GameManager.timeScale * 1.85f;
				if (alpha <= 0.05f)
				{
					alphaTrigger = false;
				}
			}
			warningImage.color = new Color(1f, 1f, 1f, alpha);
			hpWarning.color = new Color(1f, 1f, 1f, alpha);
			if (!bossWarningState)
			{
				break;
			}
			yield return null;
		}
		bossWarning.SetActive(false);
		hpWarning.gameObject.SetActive(false);
		isCanPause = true;
		StopCoroutine("bossWarningLabelUpdate");
		if (hpWarningState)
		{
			StartCoroutine("hpWarningUpdate");
		}
	}

	public void setOpenSkillInformation(SkillManager.SkillType skillType, float maxCooltime, bool isReinforcementSkill)
	{
		switch (skillType)
		{
		case SkillManager.SkillType.Concentration:
			if (isReinforcementSkill)
			{
				concentrationCoolTimeImage.sprite = UIWindowSkill.instance.reinforcementSkillIcon[(int)skillType];
				concentrationSkillImage.sprite = concentrationIconSprites[1];
			}
			else
			{
				concentrationCoolTimeImage.sprite = normalConcentrationCoolTimeSprite;
				concentrationSkillImage.sprite = concentrationIconSprites[0];
			}
			if (!skillCooltimeObjectList.Contains(concentrationCooltimeObject))
			{
				concentrationCooltimeObject.initCooltimeSlot(new Vector2(-47f, -383.6f - SkillCooltimeObject.intervalBetweenSkillCooltimeSlot * (float)skillCooltimeObjectList.Count), maxCooltime);
				skillCooltimeObjectList.Add(concentrationCooltimeObject);
			}
			else
			{
				changeSkillCooltimeData(skillType, maxCooltime, maxCooltime);
			}
			break;
		case SkillManager.SkillType.ClonedWarrior:
			if (isReinforcementSkill)
			{
				cloneWarriorCoolTimeImage.sprite = UIWindowSkill.instance.reinforcementSkillIcon[(int)skillType];
				cloneWarriorSkillImage.sprite = cloneWarriorIconSprites[1];
			}
			else
			{
				cloneWarriorCoolTimeImage.sprite = normalCloneWarriorCoolTimeSprite;
				cloneWarriorSkillImage.sprite = cloneWarriorIconSprites[0];
			}
			if (!skillCooltimeObjectList.Contains(healingTimeCooltimeObject))
			{
				healingTimeCooltimeObject.initCooltimeSlot(new Vector2(-47f, -383.6f - SkillCooltimeObject.intervalBetweenSkillCooltimeSlot * (float)skillCooltimeObjectList.Count), maxCooltime);
				skillCooltimeObjectList.Add(healingTimeCooltimeObject);
			}
			else
			{
				changeSkillCooltimeData(skillType, maxCooltime, maxCooltime);
			}
			break;
		}
	}

	public void setOpenTranscendSkillInformation(TranscendManager.TranscendPassiveSkillType skillType, float maxCooltime)
	{
		switch (skillType)
		{
		case TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage:
			transcendDecreaseHitDamageCooltimeObject.initCooltimeSlot(new Vector2(-47f, -383.6f - SkillCooltimeObject.intervalBetweenSkillCooltimeSlot * (float)skillCooltimeObjectList.Count), maxCooltime);
			skillCooltimeObjectList.Add(transcendDecreaseHitDamageCooltimeObject);
			break;
		case TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage:
			transcendIncreaseAllDamageCooltimeObject.initCooltimeSlot(new Vector2(-47f, -383.6f - SkillCooltimeObject.intervalBetweenSkillCooltimeSlot * (float)skillCooltimeObjectList.Count), maxCooltime);
			skillCooltimeObjectList.Add(transcendIncreaseAllDamageCooltimeObject);
			break;
		}
	}

	public void setOpenTreasureCooltimeInformation(TreasureManager.CooltimeTreasureEffectType treasureType, float maxCooltime)
	{
		if (treasureType == TreasureManager.CooltimeTreasureEffectType.DestroyTreasureChest)
		{
			destroyTreausreChestCooltimeObject.initCooltimeSlot(new Vector2(-47f, -383.6f - SkillCooltimeObject.intervalBetweenSkillCooltimeSlot * (float)skillCooltimeObjectList.Count), maxCooltime);
			skillCooltimeObjectList.Add(destroyTreausreChestCooltimeObject);
		}
	}

	public void changeSkillCooltimeData(SkillManager.SkillType skillType, float maxCooltime, float currentCooltime)
	{
		switch (skillType)
		{
		case SkillManager.SkillType.Concentration:
			concentrationCooltimeObject.changeCooltimeValue(maxCooltime, currentCooltime);
			concentrationCooltimeText.text = ((int)(maxCooltime - currentCooltime)).ToString();
			concentrationCooltimeBackground.fillAmount = 1f - currentCooltime / maxCooltime;
			break;
		case SkillManager.SkillType.ClonedWarrior:
			healingTimeCooltimeObject.changeCooltimeValue(maxCooltime, currentCooltime);
			cloneWarriorCooltimeText.text = ((int)(maxCooltime - currentCooltime)).ToString();
			cloneWarriorCooltimeBackground.fillAmount = 1f - currentCooltime / maxCooltime;
			break;
		}
	}

	public void changeSkillCooltimeData(TreasureManager.CooltimeTreasureEffectType treasureType, float maxCooltime, float currentCooltime)
	{
		if (treasureType == TreasureManager.CooltimeTreasureEffectType.DestroyTreasureChest)
		{
			destroyTreausreChestCooltimeObject.changeCooltimeValue(maxCooltime, currentCooltime);
		}
	}

	public void changeTranscendSkillCooltimeData(TranscendManager.TranscendPassiveSkillType skillType, float maxCooltime, float currentCooltime)
	{
		switch (skillType)
		{
		case TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage:
			transcendDecreaseHitDamageCooltimeObject.changeCooltimeValue(maxCooltime, currentCooltime);
			break;
		case TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage:
			transcendIncreaseAllDamageCooltimeObject.changeCooltimeValue(maxCooltime, currentCooltime);
			break;
		}
	}

	public void closeSkillCooltimeSlot(SkillManager.SkillType skillType)
	{
		switch (skillType)
		{
		case SkillManager.SkillType.Concentration:
			concentrationSkillImage.sprite = concentrationIconSprites[0];
			concentrationCooltimeObject.closeCooltimeSlot();
			concentrationCooltimeText.text = string.Empty;
			concentrationCooltimeBackground.fillAmount = 0f;
			break;
		case SkillManager.SkillType.ClonedWarrior:
			cloneWarriorSkillImage.sprite = cloneWarriorIconSprites[0];
			healingTimeCooltimeObject.closeCooltimeSlot();
			cloneWarriorCooltimeText.text = string.Empty;
			cloneWarriorCooltimeBackground.fillAmount = 0f;
			break;
		}
		refreshSkillInformations();
	}

	public void closeSkillCooltimeSlot(TreasureManager.CooltimeTreasureEffectType treasureType)
	{
		if (treasureType == TreasureManager.CooltimeTreasureEffectType.DestroyTreasureChest)
		{
			destroyTreausreChestCooltimeObject.closeCooltimeSlot();
		}
		refreshSkillInformations();
	}

	public void closeTranscendSkillCooltimeSlot(TranscendManager.TranscendPassiveSkillType skillType)
	{
		switch (skillType)
		{
		case TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage:
			transcendDecreaseHitDamageCooltimeObject.closeCooltimeSlot();
			break;
		case TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage:
			transcendIncreaseAllDamageCooltimeObject.closeCooltimeSlot();
			break;
		}
		refreshSkillInformations();
	}

	public void refreshSkillInformations()
	{
		for (int i = 0; i < skillCooltimeObjectList.Count; i++)
		{
			skillCooltimeObjectList[i].setTargetPosition(new Vector2(-47f, -383.6f - SkillCooltimeObject.intervalBetweenSkillCooltimeSlot * (float)i));
		}
	}

	public void OnClickRevive()
	{
		Singleton<CharacterManager>.instance.warriorCharacter.increasesHealth(Singleton<CharacterManager>.instance.warriorCharacter.maxHealth);
		Singleton<CharacterManager>.instance.warriorCharacter.hpGauge.refreshLerpHP();
		for (int i = 0; i < Singleton<CharacterManager>.instance.characterList.Count; i++)
		{
			Singleton<CharacterManager>.instance.characterList[i].isDead = false;
			Singleton<CharacterManager>.instance.characterList[i].setState(PublicDataManager.State.Move);
		}
	}

	private void OnApplicationPause()
	{
		if (isCanPause)
		{
			Singleton<GameManager>.instance.Pause(true);
		}
	}

	public void OnClickDie()
	{
		Singleton<EnemyManager>.instance.endBossSafe();
		Singleton<CachedManager>.instance.coverUI.fadeOutGame();
	}

	public void OnClickPause(bool pause)
	{
		if (isCanPause)
		{
			Singleton<GameManager>.instance.Pause(pause);
		}
	}

	public void OnClickGiveUp()
	{
		UIWindowDialog.openDescription("INGAME_QUIT_QUESTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			Singleton<EnemyManager>.instance.endBossSafe();
			Singleton<CachedManager>.instance.coverUI.fadeOutGame();
			PauseUI.SetActive(false);
		}, string.Empty);
	}

	public void onClickVeryFast()
	{
		Singleton<AutoTouchManager>.instance.startAutoTouch(AutoTouchManager.AutoTouchType.VeryFast);
	}

	public void onClickFast()
	{
		Singleton<AutoTouchManager>.instance.startAutoTouch(AutoTouchManager.AutoTouchType.AdsAngelAutoTouch);
	}

	public void onClickSlow()
	{
		Singleton<AutoTouchManager>.instance.startAutoTouch(AutoTouchManager.AutoTouchType.TimerSilverFinger);
	}
}
