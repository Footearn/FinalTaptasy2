using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowOutgame : UIWindow
{
	public enum UIType
	{
		NONE,
		ManageHero,
		ManageColleague,
		ManageSkill,
		ManageTreasure,
		ManageShop
	}

	public static UIWindowOutgame instance;

	public UIType currentUIType;

	public GameObject limitedItemObject;

	public Text[] tabTexts;

	public RawImage warriorTabImage;

	public RawImage priestTabImage;

	public RawImage archerTabImage;

	public GameObject[] warriorWeaponNewIconObjects;

	public GameObject[] priestWeaponNewIconObjects;

	public GameObject[] archerWeaponNewIconObjects;

	public UIWindow colleagueUIWindow;

	public GameObject colleagueNonSelectObject;

	public GameObject colleagueSelectedObject;

	public GameObject colleagueIndigator;

	public CanvasGroup colleagueGroup;

	public CanvasGroup heroAndWeaponPreviewCanvasGroup;

	public CanvasGroup skillPreviewCanvasGroup;

	public CanvasGroup treasurePreviewCanvasGroup;

	public CanvasGroup shopPreviewCanvasGroup;

	public CanvasGroup previewGroup;

	public GameObject previewObject;

	public CanvasGroup topLeftGroup;

	public CanvasGroup toprightGroup;

	public UIWindowManageHeroAndWeapon manageHeroUIWindow;

	public GameObject manageHeroNonSelectObject;

	public GameObject manageHeroSelectedObject;

	public GameObject manageHeroIndigator;

	public UIWindowSkill manageSkillUIWindow;

	public GameObject manageSkillNonSelectObject;

	public GameObject manageSkillSelectedObject;

	public GameObject manageSkillIndigator;

	public UIWindowManageTreasure manageTreasureUIWindow;

	public GameObject manageTreasureNonSelectObject;

	public GameObject manageTreasureSelectedObject;

	public GameObject manageTreasureIndigator;

	public Text manageTreasureIndicatorCountText;

	public GameObject treasureLotteryByKeyIndicatorObject;

	public Text treasureLotteryByKeyIndicatorText;

	public GameObject rebirthIndicatorObject;

	public Text rebirthIndicatorText;

	public UIWindow manageShopUIWindow;

	public GameObject manageShopNonSelectObject;

	public GameObject manageShopSelectedObject;

	public GameObject manageShopIndigator;

	public GameObject limitedShopIndicator;

	public Text shopIndicatorText;

	public Text limitedShopIndicatorText;

	public GameObject questIndigator;

	public Text questCompleteCountText;

	public GameObject achievementIndigator;

	public Text achievementCompleteCountText;

	public GameObject settingIndicator;

	public Text settingIndicatorCountText;

	public Text menuTitleText;

	public RectTransform normalDengeonQuickStartButtonTransform;

	public RectTransform bossRaidSwordTargetTransform;

	public RectTransform elopeModeQuickStartButtonTransform;

	public RectTransform towerModeQuickStartButtonTransform;

	public GameObject bossRaidQuickStartButton;

	public GameObject closedBossRaidQuickStartButton;

	public int quickStartTargetTheme;

	public int quickStartTargetStage;

	public Text quickStartTargetThemeText;

	public Text quickStartTimerText;

	public GameObject princessCollectionIndicator;

	public Image battleBlock;

	public GameObject battleEffectObject;

	public GameObject normalDungeonStartIconObject;

	public GameObject elopeModeStartIconObject;

	public GameObject towerModeStartIconObject;

	public Transform battleEffect;

	public Transform battleIcon;

	public Transform battleAura;

	public AnimationCurve iconAnim;

	public Image notificationImage;

	private DateTime postboxCheckTime;

	public Text postboxCountText;

	public GameObject postboxCountObject;

	public GameObject vipOnObject;

	public GameObject vipOffObject;

	public Text vipRemainText;

	public Sprite[] heroBackgroundSprites;

	public Sprite[] colleagueBackgroundSprites;

	public Sprite[] skillBackgroundSprites;

	public Sprite[] treasureBackgroundSprites;

	public Sprite[] shopBackgroundSprites;

	public Image heroBackgroundImage;

	public Image colleagueBackgroundImage;

	public Image skillBackgroundImage;

	public Image treasureBackgroundImage;

	public Image shopBackgroundImage;

	public override void Awake()
	{
		instance = this;
		I18NManager.changeLanguageNotification = (Action)Delegate.Combine(I18NManager.changeLanguageNotification, new Action(onChangeLanguageNotification));
		base.Awake();
	}

	public void refreshVIPStatus()
	{
		if (Singleton<NanooAPIManager>.instance.IsExistSubscriptionItem())
		{
			vipRemainText.text = Singleton<NanooAPIManager>.instance.GetVipItemCount() + " " + I18NManager.Get("SHOP_LIMITED_DAY_SIMPLE");
			vipOnObject.SetActive(true);
			vipOffObject.SetActive(false);
		}
		else
		{
			vipOnObject.SetActive(false);
			vipOffObject.SetActive(true);
		}
	}

	private void onChangeLanguageNotification()
	{
		refreshVIPStatus();
		openUI(currentUIType, true);
	}

	public void refreshShopIndicator(int newProductCount)
	{
		if (newProductCount > 0)
		{
			manageShopIndigator.SetActive(true);
			limitedShopIndicator.SetActive(true);
			shopIndicatorText.text = newProductCount.ToString();
			Debug.LogError(newProductCount);
			limitedShopIndicatorText.text = newProductCount.ToString();
		}
		else
		{
			manageShopIndigator.SetActive(false);
			limitedShopIndicator.SetActive(false);
		}
	}

	public void refreshSettingIndicator()
	{
		int num = ((!Singleton<DataManager>.instance.currentGameData.isObtainedFacebookReward) ? 1 : 0) + ((!Singleton<DataManager>.instance.currentGameData.isObtainedTwitterReward) ? 1 : 0);
		if (num > 0)
		{
			settingIndicatorCountText.text = num.ToString();
			settingIndicator.SetActive(true);
		}
		else
		{
			settingIndicator.SetActive(false);
		}
	}

	public void refreshTreasureIndicator()
	{
		int num = (int)(Singleton<DataManager>.instance.currentGameData.treasurePiece / Singleton<TreasureManager>.instance.currentLotteryPriceForTreasurePiece);
		if (num > 0)
		{
			if (!UIWindowManageTreasure.instance.lotteryByKeyEffect.activeSelf)
			{
				UIWindowManageTreasure.instance.lotteryByKeyEffect.SetActive(true);
			}
			if (!treasureLotteryByKeyIndicatorObject.activeSelf)
			{
				treasureLotteryByKeyIndicatorObject.SetActive(true);
			}
			treasureLotteryByKeyIndicatorText.text = num.ToString();
		}
		else
		{
			if (UIWindowManageTreasure.instance.lotteryByKeyEffect.activeSelf)
			{
				UIWindowManageTreasure.instance.lotteryByKeyEffect.SetActive(false);
			}
			if (treasureLotteryByKeyIndicatorObject.activeSelf)
			{
				treasureLotteryByKeyIndicatorObject.SetActive(false);
			}
		}
		int num2 = ((Singleton<DataManager>.instance.currentGameData.unlockTheme > Singleton<RebirthManager>.instance.currentRebirthRequireTheme) ? 1 : 0);
		if (num2 > 0)
		{
			if (!rebirthIndicatorObject.activeSelf)
			{
				rebirthIndicatorObject.SetActive(true);
			}
			rebirthIndicatorText.text = num2.ToString();
		}
		else if (rebirthIndicatorObject.activeSelf)
		{
			rebirthIndicatorObject.SetActive(false);
		}
		int num3 = 0;
		if (Singleton<ElopeModeManager>.instance.isCanStartElopeMode())
		{
			TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.Now().Ticks - Singleton<DataManager>.instance.currentGameData.elopeShopTargetRefreshTime);
			long num4 = (long)(timeSpan.TotalHours / (double)ElopeModeManager.intervalRefreshItemHour) + 1;
			if (Singleton<DataManager>.instance.currentGameData.lastRefreshElopeShopHour < num4)
			{
				num3 = 3;
			}
		}
		if (num3 > 0)
		{
			if (!UIWindowManageTreasure.instance.elopeShopIndicator.activeSelf)
			{
				UIWindowManageTreasure.instance.elopeShopIndicator.SetActive(true);
			}
		}
		else if (UIWindowManageTreasure.instance.elopeShopIndicator.activeSelf)
		{
			UIWindowManageTreasure.instance.elopeShopIndicator.SetActive(false);
		}
		if (num + num2 + num3 > 0)
		{
			manageTreasureIndicatorCountText.text = (num + num2 + num3).ToString();
			if (!manageTreasureIndigator.activeSelf)
			{
				manageTreasureIndigator.SetActive(true);
			}
		}
		else if (manageTreasureIndigator.activeSelf)
		{
			manageTreasureIndigator.SetActive(false);
		}
	}

	public void refreshQuestCompleteIndicator()
	{
		int num = 0;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.questData.Count; i++)
		{
			if (Singleton<DataManager>.instance.currentGameData.questData[i].isComplete)
			{
				num++;
			}
		}
		if (num > 0)
		{
			questIndigator.SetActive(true);
			questCompleteCountText.text = num.ToString();
		}
		else
		{
			questIndigator.SetActive(false);
		}
	}

	public void refreshAchievementCompleteIndicator()
	{
		int num = 0;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.achievementData.Count; i++)
		{
			AchievementData achievementData = Singleton<DataManager>.instance.currentGameData.achievementData[i];
			if (!achievementData.isComplete && Singleton<AchievementManager>.instance.isCanObtainReward(achievementData.currentAchievementType))
			{
				num++;
			}
		}
		if (num > 0)
		{
			achievementIndigator.SetActive(true);
			achievementCompleteCountText.text = num.ToString();
		}
		else
		{
			achievementIndigator.SetActive(false);
		}
	}

	private void Start()
	{
		if (!Singleton<DataManager>.instance.currentGameData.isSeenPrincessCollectionForPrevVersion)
		{
			princessCollectionIndicator.SetActive(true);
			Singleton<DataManager>.instance.currentGameData.isSeenPrincessCollectionForPrevVersion = true;
			Singleton<DataManager>.instance.saveData();
		}
		Singleton<CachedManager>.instance.townSpriteGroup.setAlpha(0f);
		openUI(UIType.ManageHero, true);
		colleagueIndigator.SetActive(false);
		manageHeroIndigator.SetActive(false);
		manageSkillIndigator.SetActive(false);
		refreshTreasureIndicator();
		refreshSettingIndicator();
		manageShopIndigator.SetActive(false);
		refreshBossDungeonDoor();
		refreshVIPStatus();
		refreshCurrentStageText();
		StopCoroutine("waitForPostboxCount");
		StartCoroutine("waitForPostboxCount");
	}

	public void refreshBackgrounds()
	{
		int currentTheme = Singleton<DataManager>.instance.currentGameData.currentTheme;
		int num = ((currentTheme > 100) ? 1 : 0);
		heroBackgroundImage.sprite = heroBackgroundSprites[num];
		heroBackgroundImage.SetNativeSize();
		colleagueBackgroundImage.sprite = colleagueBackgroundSprites[num];
		colleagueBackgroundImage.SetNativeSize();
		skillBackgroundImage.sprite = skillBackgroundSprites[num];
		skillBackgroundImage.SetNativeSize();
		treasureBackgroundImage.sprite = treasureBackgroundSprites[num];
		treasureBackgroundImage.SetNativeSize();
		shopBackgroundImage.sprite = shopBackgroundSprites[num];
		shopBackgroundImage.SetNativeSize();
	}

	public void refreshCurrentStageText()
	{
		quickStartTargetTheme = Singleton<DataManager>.instance.currentGameData.unlockTheme;
		quickStartTargetStage = Singleton<DataManager>.instance.currentGameData.unlockStage;
		quickStartTargetThemeText.text = quickStartTargetTheme + "-" + quickStartTargetStage;
	}

	public void OnClickSelectMode()
	{
		UIWindowSelectSpecialDungeon.instance.open();
	}

	public void onClickQuickStart()
	{
		if (!GameManager.isWaitForStartGame)
		{
			Singleton<StatManager>.instance.refreshAllStats();
			GameManager.isWaitForStartGame = true;
			GameManager.currentTheme = (Singleton<DataManager>.instance.currentGameData.currentTheme = Singleton<DataManager>.instance.currentGameData.unlockTheme);
			GameManager.currentStage = (Singleton<DataManager>.instance.currentGameData.currentStage = Singleton<DataManager>.instance.currentGameData.unlockStage);
			normalDungeonStartIconObject.SetActive(true);
			elopeModeStartIconObject.SetActive(false);
			towerModeStartIconObject.SetActive(false);
			GameManager.currentDungeonType = GameManager.SpecialDungeonType.NormalDungeon;
			Singleton<GameManager>.instance.setMaxLimitTheme();
			Singleton<DataManager>.instance.saveData();
			Singleton<GameManager>.instance.gameReady(normalDengeonQuickStartButtonTransform);
		}
	}

	public void onClickEnterTowerMode(TowerModeManager.TowerModeDifficultyType difficulty, bool freeMode)
	{
		if (!GameManager.isWaitForStartGame)
		{
			GameManager.isWaitForStartGame = true;
			normalDungeonStartIconObject.SetActive(false);
			elopeModeStartIconObject.SetActive(false);
			towerModeStartIconObject.SetActive(true);
			switch (difficulty)
			{
			case TowerModeManager.TowerModeDifficultyType.TimeAttack:
				Singleton<GameManager>.instance.currentTargetStartSwordEffectTransform = UIWindowSelectTowerModeDifficulty.instance.timeAttackDifficultyButtonTransform;
				break;
			case TowerModeManager.TowerModeDifficultyType.Endless:
				Singleton<GameManager>.instance.currentTargetStartSwordEffectTransform = UIWindowSelectTowerModeDifficulty.instance.endlessDifficultyButtonTransform;
				break;
			}
			GameManager.currentGameState = GameManager.GameState.Playing;
			GameManager.currentDungeonType = GameManager.SpecialDungeonType.TowerMode;
			Singleton<GameManager>.instance.StartCoroutine(Singleton<GameManager>.instance.playSwordEffectUpdate(false, delegate
			{
				Singleton<TowerModeManager>.instance.startTowerMode(difficulty, freeMode);
				Singleton<CachedManager>.instance.coverUI.fadeIn(1f);
			}));
		}
	}

	public void OnClickEnterElopeMode()
	{
		if (!Singleton<ElopeModeManager>.instance.isCanStartElopeMode())
		{
			UIWindowDialog.openMiniDialog("CAN_NOT_START_ELOPE_MODE_DESCRIPTION");
		}
		else
		{
			if (GameManager.isWaitForStartGame)
			{
				return;
			}
			GameManager.isWaitForStartGame = true;
			Singleton<GameManager>.instance.currentTargetStartSwordEffectTransform = elopeModeQuickStartButtonTransform;
			normalDungeonStartIconObject.SetActive(false);
			elopeModeStartIconObject.SetActive(true);
			towerModeStartIconObject.SetActive(false);
			if (PlayerPrefs.GetInt("IsFirstStartElopeMode", 0) == 1)
			{
				GameManager.currentDungeonType = GameManager.SpecialDungeonType.ElopeMode;
				Singleton<GameManager>.instance.StartCoroutine(Singleton<GameManager>.instance.playSwordEffectUpdate(false, delegate
				{
					Singleton<ElopeModeManager>.instance.startElopeMode();
					Singleton<CachedManager>.instance.coverUI.fadeIn(1f);
				}));
			}
			else
			{
				PlayerPrefs.SetInt("IsFirstStartElopeMode", 1);
				UIWindowElopeFirstPlay.instance.open();
			}
		}
	}

	private void Update()
	{
		if (GameManager.currentGameState != GameManager.GameState.OutGame)
		{
			return;
		}
		if (Singleton<DataManager>.instance != null && Singleton<DataManager>.instance.currentGameData != null && Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList != null)
		{
			if (Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Count > 0)
			{
				if (!limitedItemObject.activeSelf)
				{
					limitedItemObject.SetActive(true);
				}
			}
			else if (limitedItemObject.activeSelf)
			{
				limitedItemObject.SetActive(false);
			}
		}
		else if (limitedItemObject.activeSelf)
		{
			limitedItemObject.SetActive(false);
		}
		switch (currentUIType)
		{
		case UIType.ManageHero:
			colleagueGroup.alpha = Mathf.Lerp(colleagueGroup.alpha, 0f, Time.deltaTime * 8f);
			heroAndWeaponPreviewCanvasGroup.alpha = Mathf.Lerp(heroAndWeaponPreviewCanvasGroup.alpha, 1f, Time.deltaTime * 8f);
			skillPreviewCanvasGroup.alpha = Mathf.Lerp(skillPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			treasurePreviewCanvasGroup.alpha = Mathf.Lerp(treasurePreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			shopPreviewCanvasGroup.alpha = Mathf.Lerp(shopPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			break;
		case UIType.ManageSkill:
			colleagueGroup.alpha = Mathf.Lerp(colleagueGroup.alpha, 0f, Time.deltaTime * 8f);
			heroAndWeaponPreviewCanvasGroup.alpha = Mathf.Lerp(heroAndWeaponPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			skillPreviewCanvasGroup.alpha = Mathf.Lerp(skillPreviewCanvasGroup.alpha, 1f, Time.deltaTime * 8f);
			treasurePreviewCanvasGroup.alpha = Mathf.Lerp(treasurePreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			shopPreviewCanvasGroup.alpha = Mathf.Lerp(shopPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			break;
		case UIType.ManageColleague:
			colleagueGroup.alpha = Mathf.Lerp(colleagueGroup.alpha, 1f, Time.deltaTime * 8f);
			heroAndWeaponPreviewCanvasGroup.alpha = Mathf.Lerp(heroAndWeaponPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			skillPreviewCanvasGroup.alpha = Mathf.Lerp(skillPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			treasurePreviewCanvasGroup.alpha = Mathf.Lerp(treasurePreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			shopPreviewCanvasGroup.alpha = Mathf.Lerp(shopPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			break;
		case UIType.ManageTreasure:
			colleagueGroup.alpha = Mathf.Lerp(colleagueGroup.alpha, 0f, Time.deltaTime * 8f);
			heroAndWeaponPreviewCanvasGroup.alpha = Mathf.Lerp(heroAndWeaponPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			skillPreviewCanvasGroup.alpha = Mathf.Lerp(skillPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			treasurePreviewCanvasGroup.alpha = Mathf.Lerp(treasurePreviewCanvasGroup.alpha, 1f, Time.deltaTime * 8f);
			shopPreviewCanvasGroup.alpha = Mathf.Lerp(shopPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			break;
		case UIType.ManageShop:
			colleagueGroup.alpha = Mathf.Lerp(colleagueGroup.alpha, 0f, Time.deltaTime * 8f);
			heroAndWeaponPreviewCanvasGroup.alpha = Mathf.Lerp(heroAndWeaponPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			skillPreviewCanvasGroup.alpha = Mathf.Lerp(skillPreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			treasurePreviewCanvasGroup.alpha = Mathf.Lerp(treasurePreviewCanvasGroup.alpha, 0f, Time.deltaTime * 8f);
			shopPreviewCanvasGroup.alpha = Mathf.Lerp(shopPreviewCanvasGroup.alpha, 1f, Time.deltaTime * 8f);
			break;
		}
	}

	public override bool OnBeforeOpen()
	{
		refreshBackgrounds();
		refreshSettingIndicator();
		refreshTreasureIndicator();
		refreshQuestCompleteIndicator();
		refreshAchievementCompleteIndicator();
		refreshCurrentStageText();
		Singleton<CachedManager>.instance.townSpriteGroup.setAlpha(0f);
		openUI(UIType.ManageHero, true);
		refreshBossDungeonDoor();
		return base.OnBeforeOpen();
	}

	public override void OnAfterActiveGameObject()
	{
		base.OnAfterActiveGameObject();
	}

	public void refreshBossDungeonDoor()
	{
	}

	public override void OnAfterOpen()
	{
		base.OnAfterOpen();
		StopAllCoroutines();
	}

	private void refreshAllButtonSprites()
	{
		colleagueSelectedObject.SetActive(false);
		manageHeroSelectedObject.SetActive(false);
		manageSkillSelectedObject.SetActive(false);
		manageTreasureSelectedObject.SetActive(false);
		manageShopSelectedObject.SetActive(false);
		colleagueNonSelectObject.SetActive(true);
		manageHeroNonSelectObject.SetActive(true);
		manageSkillNonSelectObject.SetActive(true);
		manageTreasureNonSelectObject.SetActive(true);
		manageShopNonSelectObject.SetActive(true);
		previewGroup.alpha = 0f;
		topLeftGroup.alpha = 1f;
		toprightGroup.alpha = 1f;
		previewObject.SetActive(false);
		for (int i = 0; i < 5; i++)
		{
			tabTexts[i].color = Util.getCalculatedColor(30f, 83f, 110f);
		}
	}

	public void openUI(int uiType)
	{
		openUI((UIType)uiType);
	}

	public void openUI(UIType uiType, bool force = false)
	{
		if (uiType == currentUIType && !force)
		{
			return;
		}
		UIWindowManager.isOpeningStageList = false;
		currentUIType = uiType;
		refreshAllButtonSprites();
		tabTexts[(int)(uiType - 1)].color = Util.getCalculatedColor(120f, 166f, 219f);
		switch (uiType)
		{
		case UIType.ManageHero:
			manageHeroSelectedObject.SetActive(true);
			manageHeroNonSelectObject.SetActive(false);
			colleagueUIWindow.close();
			manageHeroUIWindow.open();
			manageSkillUIWindow.close();
			manageTreasureUIWindow.close();
			manageShopUIWindow.close();
			manageHeroIndigator.SetActive(false);
			break;
		case UIType.ManageSkill:
			manageSkillSelectedObject.SetActive(true);
			manageSkillNonSelectObject.SetActive(false);
			menuTitleText.text = I18NManager.Get("SKILL");
			colleagueUIWindow.close();
			manageHeroUIWindow.close();
			manageSkillUIWindow.open();
			manageTreasureUIWindow.close();
			manageShopUIWindow.close();
			manageSkillIndigator.SetActive(false);
			break;
		case UIType.ManageColleague:
			if (TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType33);
			}
			colleagueSelectedObject.SetActive(true);
			colleagueNonSelectObject.SetActive(false);
			menuTitleText.text = I18NManager.Get("COLLEAGUE");
			colleagueUIWindow.open();
			manageHeroUIWindow.close();
			manageSkillUIWindow.close();
			manageTreasureUIWindow.close();
			manageShopUIWindow.close();
			colleagueIndigator.SetActive(false);
			break;
		case UIType.ManageTreasure:
			if (TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.RebirthTutorialType5);
			}
			manageTreasureSelectedObject.SetActive(true);
			manageTreasureNonSelectObject.SetActive(false);
			menuTitleText.text = I18NManager.Get("TREASURE");
			colleagueUIWindow.close();
			manageHeroUIWindow.close();
			manageSkillUIWindow.close();
			manageTreasureUIWindow.open();
			manageShopUIWindow.close();
			refreshTreasureIndicator();
			break;
		case UIType.ManageShop:
			manageShopSelectedObject.SetActive(true);
			manageShopNonSelectObject.SetActive(false);
			menuTitleText.text = I18NManager.Get("SHOP");
			colleagueUIWindow.close();
			manageHeroUIWindow.close();
			manageSkillUIWindow.close();
			manageTreasureUIWindow.close();
			manageShopUIWindow.open();
			manageShopIndigator.SetActive(false);
			break;
		}
	}

	public void OnClickGoToCommunity()
	{
		Singleton<NanooAPIManager>.instance.OpenForum();
	}

	public void OnClickOpenDailyRewardUI()
	{
		UIWindowRoulette.instance.openRouletteUI(true);
	}

	public void OnClickAddGold()
	{
		UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
		UIWindowPopupShop.instance.focusToGold();
	}

	public void OnClickAddRuby()
	{
		UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
	}

	public void OnClickLimitedItem()
	{
		ShopManager.ShopSelectedType targetSelectType = ShopManager.ShopSelectedType.PremiumItem;
		if (Singleton<ShopManager>.instance.premiumItemList.Count <= 0)
		{
			targetSelectType = ShopManager.ShopSelectedType.SkinItem;
			if (Singleton<ShopManager>.instance.skinItemList.Count <= 0)
			{
				targetSelectType = ShopManager.ShopSelectedType.Ruby;
			}
		}
		UIWindowPopupShop.instance.openShopPopupWithType(targetSelectType);
	}

	public void OnClickOpenPostBox()
	{
		UIWindowPostBox.instance.openPostBoxUI();
	}

	public IEnumerator waitForPostboxCount()
	{
		postboxCheckTime = UnbiasedTime.Instance.Now();
		while (!Singleton<NanooAPIManager>.instance.isPostInitComplete && UnbiasedTime.Instance.Now().Subtract(postboxCheckTime).TotalSeconds < 10.0)
		{
			yield return null;
		}
		if (!Singleton<NanooAPIManager>.instance.isPostInitComplete || Singleton<NanooAPIManager>.instance.postboxItemCount <= 0)
		{
			postboxCountObject.SetActive(false);
		}
		else
		{
			refreshPostboxCount(Singleton<NanooAPIManager>.instance.postboxItemCount);
		}
	}

	public void refreshPostboxCount(int count)
	{
		if (count > 0)
		{
			postboxCountText.text = count.ToString();
			postboxCountObject.SetActive(true);
		}
		else
		{
			postboxCountObject.SetActive(false);
		}
	}
}
