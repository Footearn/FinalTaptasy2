using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColleagueSlotObject : ScrollSlotItem
{
	public ColleagueManager.ColleagueType currentColleagueType;

	public InfiniteScroll scrollScript;

	public Image mainBackgroundImage;

	public Text nameText;

	public Text damageText;

	public Text levelText;

	public Text premiumColleagueLevelText;

	public Image levelUpButtonBackgroundImage;

	public Image levelUpGoldIconImage;

	public Text levelUpTitleText;

	public Text levelUpPriceText;

	public Image unlockButtonBackgroundImage;

	public Image unlockGoldIconImage;

	public Text unlockTitleText;

	public Text unlockPriceText;

	public Text afterLevelUpincreaseingValueText;

	public Image[] passiveSkillIconImages;

	public GameObject[] passiveSkillBlockedObjects;

	public Text[] passiveSkillRequireLevelTexts;

	public GameObject unlockedObject;

	public GameObject lockedObject;

	public GameObject unlockButtonObject;

	public GameObject lockedTextObject;

	public GameObject skinPlusObject;

	public Image iconBackgroundImage;

	public RectTransform iconBackgroundImageTrasnform;

	public ColleagueUIObject currentColleagueUIObject;

	public GameObject levelProgressObject;

	public Image levelProgressImage;

	public GameObject colleagueThumbnailObject;

	public GameObject skillUnlockObject;

	public ParticleSystem upgradeEffect;

	public ParticleSystem unlockEffect;

	public GameObject unlockableEffectObject;

	public Button unlockButton;

	public Image skinChangeButtonImage;

	public GameObject normalColleagueObject;

	public GameObject premiumColleagueObject;

	public GameObject expProgressObject;

	public GameObject newSkinIconObject;

	public GameObject premiumColleagueUnlockButtonObject;

	public GameObject premiumColleagueMaxButtonObject;

	public GameObject premiumColleagueUpgradeButtonObject;

	public Text premiumUpgradeIncreaseValueText;

	public Text premiumUpgradePriceText;

	public Text premiumStatText;

	public Text premiumDescriptionText;

	public Text premiumUnlockPrice;

	public Canvas premiumRibbonCanvas;

	private ColleagueInventoryData m_currentColleagueInventoryData;

	private Dictionary<ColleagueManager.PassiveSkillTierType, ColleaguePassiveSkillData> m_currentColleaguePassiveSkillData;

	private float m_quickUpgradeDisappearTimer;

	public Text quickUpgradeTotalPriceText;

	public Text quickUpgradeTargetLevelText;

	public GameObject quickUpgradeButtonObject;

	public Animation quickUpgradeButtonAnimation;

	public bool isQuickUpgrading = true;

	public GameObject slotLockedObject;

	private void Start()
	{
		for (int i = 0; i < 6; i++)
		{
			passiveSkillRequireLevelTexts[i].text = "Lv.\n" + Singleton<ColleagueManager>.instance.getColleaguePassiveSkillUnlockLevel((ColleagueManager.PassiveSkillTierType)i);
		}
	}

	public override void UpdateItem(int count)
	{
		base.UpdateItem(count);
		if (scrollScript == null)
		{
			scrollScript = base.cachedTransform.parent.GetComponent<InfiniteScroll>();
		}
		currentColleagueType = Singleton<ColleagueManager>.instance.colleagueSlotTypeList[count];
		initSlot();
	}

	public void initSlot()
	{
		if ((slotIndex + 1) % 2 == 0)
		{
			mainBackgroundImage.color = new Color(0f, 0f, 0f, 0f);
		}
		else
		{
			mainBackgroundImage.color = Util.getCalculatedColor(0f, 11f, 28f, 51f);
		}
		m_currentColleagueInventoryData = Singleton<ColleagueManager>.instance.getColleagueInventoryData(currentColleagueType);
		refreshColleagueUIObject();
		closeQuickUpgradeButton(true);
		refreshSlot();
	}

	public void refreshColleagueUIObject()
	{
		if (currentColleagueUIObject != null)
		{
			ObjectPool.Recycle(currentColleagueUIObject.name, currentColleagueUIObject.cachedGameObject);
		}
		currentColleagueUIObject = null;
		currentColleagueUIObject = ObjectPool.Spawn("@ColleagueUIObject" + (int)(currentColleagueType + 1), Vector2.zero, iconBackgroundImageTrasnform).GetComponent<ColleagueUIObject>();
		currentColleagueUIObject.cachedTransform.localPosition = new Vector2(0f, -25f);
		currentColleagueUIObject.cachedTransform.localScale = new Vector3(88f, 88f, 1f);
		currentColleagueUIObject.initColleagueUI(currentColleagueType, m_currentColleagueInventoryData.currentEquippedSkinIndex);
		currentColleagueUIObject.changeLayer("ScrollUI");
	}

	public override void refreshSlot()
	{
		newSkinIconObject.SetActive(false);
		m_currentColleagueInventoryData = Singleton<ColleagueManager>.instance.getColleagueInventoryData(currentColleagueType);
		m_currentColleaguePassiveSkillData = Singleton<ColleagueManager>.instance.getColleaguePassiveSkillData(currentColleagueType);
		nameText.text = Singleton<ColleagueManager>.instance.getColleagueI18NName(currentColleagueType, m_currentColleagueInventoryData.currentEquippedSkinIndex);
		slotLockedObject.SetActive(false);
		if (!Singleton<ColleagueManager>.instance.isPremiumColleague(currentColleagueType))
		{
			if (!skinPlusObject.activeSelf)
			{
				skinPlusObject.SetActive(true);
			}
			if (!expProgressObject.activeSelf)
			{
				expProgressObject.SetActive(true);
			}
			if (!normalColleagueObject.activeSelf)
			{
				normalColleagueObject.SetActive(true);
			}
			if (premiumColleagueObject.activeSelf)
			{
				premiumColleagueObject.SetActive(false);
			}
			levelText.text = "Lv." + m_currentColleagueInventoryData.level;
			if (Singleton<ColleagueManager>.instance.isCanUnlockSkill(currentColleagueType))
			{
				if (!skillUnlockObject.activeSelf)
				{
					skillUnlockObject.SetActive(true);
				}
				if (colleagueThumbnailObject.activeSelf)
				{
					colleagueThumbnailObject.SetActive(false);
				}
			}
			else
			{
				if (skillUnlockObject.activeSelf)
				{
					skillUnlockObject.SetActive(false);
				}
				if (!colleagueThumbnailObject.activeSelf)
				{
					colleagueThumbnailObject.SetActive(true);
				}
			}
			if (m_currentColleagueInventoryData.isUnlocked)
			{
				if (UIWindowColleagueInformation.instance.newSkinColleagueList.Contains(currentColleagueType))
				{
					newSkinIconObject.SetActive(true);
				}
				if (!levelProgressObject.activeSelf)
				{
					levelProgressObject.SetActive(true);
				}
				long nextSkillUnlockLevel = Singleton<ColleagueManager>.instance.getNextSkillUnlockLevel(currentColleagueType);
				long nextSkillUnlockLevel2 = Singleton<ColleagueManager>.instance.getNextSkillUnlockLevel(currentColleagueType);
				nextSkillUnlockLevel2 = ((m_currentColleagueInventoryData.lastPassiveUnlockLevel < Singleton<ColleagueManager>.instance.getColleaguePassiveSkillUnlockLevel(ColleagueManager.PassiveSkillTierType.PassiveSkillTier1)) ? (nextSkillUnlockLevel2 - 10) : ((m_currentColleagueInventoryData.lastPassiveUnlockLevel >= Singleton<ColleagueManager>.instance.getColleaguePassiveSkillUnlockLevel(ColleagueManager.PassiveSkillTierType.PassiveSkillTier2)) ? (nextSkillUnlockLevel2 - Singleton<ColleagueManager>.instance.multipleForInfinityLevelup) : 10));
				long num = m_currentColleagueInventoryData.level - nextSkillUnlockLevel2;
				levelProgressImage.fillAmount = (float)num / (float)(nextSkillUnlockLevel - nextSkillUnlockLevel2);
			}
			else
			{
				if (currentColleagueType > ColleagueManager.ColleagueType.Isabelle)
				{
					if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(Singleton<ColleagueManager>.instance.getPrevColleagueTypeForNormalColleague(currentColleagueType)).isUnlocked)
					{
						if (!unlockButtonObject.activeSelf)
						{
							unlockButtonObject.SetActive(true);
						}
						if (lockedTextObject.activeSelf)
						{
							lockedTextObject.SetActive(false);
						}
					}
					else
					{
						if (unlockButtonObject.activeSelf)
						{
							unlockButtonObject.SetActive(false);
						}
						if (!lockedTextObject.activeSelf)
						{
							lockedTextObject.SetActive(true);
						}
					}
				}
				else
				{
					if (!unlockButtonObject.activeSelf)
					{
						unlockButtonObject.SetActive(true);
					}
					if (lockedTextObject.activeSelf)
					{
						lockedTextObject.SetActive(false);
					}
				}
				if (levelProgressObject.activeSelf)
				{
					levelProgressObject.SetActive(false);
				}
			}
			refreshButtonState();
			for (int i = 0; i < 6; i++)
			{
				ColleaguePassiveSkillData colleaguePassiveSkillData = m_currentColleaguePassiveSkillData[(ColleagueManager.PassiveSkillTierType)i];
				passiveSkillIconImages[i].sprite = Singleton<ColleagueManager>.instance.getColleaguePassiveSkillIconSprite(colleaguePassiveSkillData.passiveType, colleaguePassiveSkillData.passiveTargetType);
			}
			for (int j = 0; j < 6; j++)
			{
				if (m_currentColleagueInventoryData.lastPassiveUnlockLevel >= Singleton<ColleagueManager>.instance.getColleaguePassiveSkillUnlockLevel((ColleagueManager.PassiveSkillTierType)j))
				{
					if (passiveSkillBlockedObjects[j].activeSelf)
					{
						passiveSkillBlockedObjects[j].SetActive(false);
					}
				}
				else if (!passiveSkillBlockedObjects[j].activeSelf)
				{
					passiveSkillBlockedObjects[j].SetActive(true);
				}
			}
			damageText.text = GameManager.changeUnit(Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, 1f));
			afterLevelUpincreaseingValueText.text = "+" + GameManager.changeUnit(Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level + 1, true, 1f) - Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, 1f), false);
			if (m_currentColleagueInventoryData.isUnlocked)
			{
				if (!unlockedObject.activeSelf)
				{
					unlockedObject.SetActive(true);
				}
				if (lockedObject.activeSelf)
				{
					lockedObject.SetActive(false);
				}
				levelUpPriceText.text = GameManager.changeUnit(Singleton<ColleagueManager>.instance.getColleagueLevelUpPrice(currentColleagueType, m_currentColleagueInventoryData.level));
			}
			else
			{
				if (unlockedObject.activeSelf)
				{
					unlockedObject.SetActive(false);
				}
				if (!lockedObject.activeSelf)
				{
					lockedObject.SetActive(true);
				}
				unlockPriceText.text = GameManager.changeUnit(Singleton<ColleagueManager>.instance.getColleagueUnlockPrice(currentColleagueType));
			}
			if (currentColleagueType == ColleagueManager.ColleagueType.Isabelle && TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.colleagueFirstUnlockButton = unlockButton;
			}
			if (!m_currentColleagueInventoryData.isUnlockedFromSlot)
			{
				if (lockedTextObject.activeSelf)
				{
					lockedTextObject.SetActive(false);
				}
				if (lockedObject.activeSelf)
				{
					lockedObject.SetActive(false);
				}
				slotLockedObject.SetActive(true);
				nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageText.color = Util.getCalculatedColor(153f, 153f, 153f);
				skinChangeButtonImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				Util.changeSpritesColor(currentColleagueUIObject.cachedSpriteRendererList.ToArray(), Util.getCalculatedColor(153f, 153f, 153f));
				iconBackgroundImage.sprite = Singleton<CachedManager>.instance.disableThumbnailSprite;
			}
			return;
		}
		if (skinPlusObject.activeSelf)
		{
			skinPlusObject.SetActive(false);
		}
		if (expProgressObject.activeSelf)
		{
			expProgressObject.SetActive(false);
		}
		if (normalColleagueObject.activeSelf)
		{
			normalColleagueObject.SetActive(false);
		}
		if (!premiumColleagueObject.activeSelf)
		{
			premiumColleagueObject.SetActive(true);
		}
		if (unlockableEffectObject.activeSelf)
		{
			unlockableEffectObject.SetActive(false);
		}
		if (!colleagueThumbnailObject.activeSelf)
		{
			colleagueThumbnailObject.SetActive(true);
		}
		skinChangeButtonImage.color = Color.white;
		Util.changeSpritesColor(currentColleagueUIObject.cachedSpriteRendererList.ToArray(), Color.white);
		iconBackgroundImage.sprite = Singleton<CachedManager>.instance.enableThumbnailFellowSprite;
		nameText.color = Util.getCalculatedColor(30f, 199f, 255f);
		premiumStatText.text = Singleton<ColleagueManager>.instance.getPremiumColleagueStatString(currentColleagueType);
		premiumDescriptionText.text = Singleton<ColleagueManager>.instance.getPremiumColleagueDescription(currentColleagueType);
		premiumUnlockPrice.text = Singleton<ColleagueManager>.instance.getPremiumColleagueUnlockPrice(currentColleagueType).ToString("N0");
		if (m_currentColleagueInventoryData.isUnlocked)
		{
			if (currentColleagueType != ColleagueManager.ColleagueType.Angela)
			{
				premiumColleagueLevelText.text = "Lv.1";
			}
			else
			{
				premiumColleagueLevelText.text = "Lv." + m_currentColleagueInventoryData.level;
			}
			if (premiumColleagueUnlockButtonObject.activeSelf)
			{
				premiumColleagueUnlockButtonObject.SetActive(false);
			}
			if (currentColleagueType == ColleagueManager.ColleagueType.Angela)
			{
				if (!premiumColleagueUpgradeButtonObject.activeSelf)
				{
					premiumColleagueUpgradeButtonObject.SetActive(true);
				}
				if (premiumColleagueMaxButtonObject.activeSelf)
				{
					premiumColleagueMaxButtonObject.SetActive(false);
				}
				premiumUpgradePriceText.text = Singleton<ColleagueManager>.instance.getCurrentPremiumColleagueUpgradePrice(currentColleagueType).ToString("N0");
				premiumUpgradeIncreaseValueText.text = "+" + string.Format("{0:0.##}", Singleton<ColleagueManager>.instance.getPremiumColleagueValue(currentColleagueType, m_currentColleagueInventoryData.level + 1) - Singleton<ColleagueManager>.instance.getPremiumColleagueValue(currentColleagueType)) + "%";
			}
			else
			{
				if (!premiumColleagueMaxButtonObject.activeSelf)
				{
					premiumColleagueMaxButtonObject.SetActive(true);
				}
				if (premiumColleagueUpgradeButtonObject.activeSelf)
				{
					premiumColleagueUpgradeButtonObject.SetActive(false);
				}
			}
		}
		else
		{
			if (currentColleagueType != ColleagueManager.ColleagueType.Angela)
			{
				premiumColleagueLevelText.text = string.Empty;
			}
			if (!premiumColleagueUnlockButtonObject.activeSelf)
			{
				premiumColleagueUnlockButtonObject.SetActive(true);
			}
			if (premiumColleagueMaxButtonObject.activeSelf)
			{
				premiumColleagueMaxButtonObject.SetActive(false);
			}
			if (premiumColleagueUpgradeButtonObject.activeSelf)
			{
				premiumColleagueUpgradeButtonObject.SetActive(false);
			}
		}
		premiumRibbonCanvas.enabled = false;
		if (base.cachedGameObject.activeInHierarchy && base.cachedGameObject.activeSelf)
		{
			StartCoroutine("waitForRibbonRender");
		}
		Canvas.ForceUpdateCanvases();
	}

	private IEnumerator waitForRibbonRender()
	{
		yield return null;
		premiumRibbonCanvas.enabled = true;
	}

	public void refreshButtonState()
	{
		if (unlockableEffectObject.activeSelf)
		{
			unlockableEffectObject.SetActive(false);
		}
		if (m_currentColleagueInventoryData.isUnlocked)
		{
			skinChangeButtonImage.color = Color.white;
			Util.changeSpritesColor(currentColleagueUIObject.cachedSpriteRendererList.ToArray(), Color.white);
			iconBackgroundImage.sprite = Singleton<CachedManager>.instance.enableThumbnailFellowSprite;
			nameText.color = Util.getCalculatedColor(30f, 199f, 255f);
			damageText.color = Color.white;
			for (int i = 0; i < passiveSkillRequireLevelTexts.Length; i++)
			{
				passiveSkillRequireLevelTexts[i].color = Color.white;
			}
			double colleagueLevelUpPrice = Singleton<ColleagueManager>.instance.getColleagueLevelUpPrice(currentColleagueType, m_currentColleagueInventoryData.level);
			if (Singleton<DataManager>.instance.currentGameData.gold >= colleagueLevelUpPrice)
			{
				levelUpButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.enableButtonOrangeSprite;
				levelUpTitleText.color = Util.getCalculatedColor(253f, 254f, 156f);
				levelUpPriceText.color = Color.white;
				levelUpGoldIconImage.color = Color.white;
				afterLevelUpincreaseingValueText.color = Color.white;
			}
			else
			{
				levelUpButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.disableButtonSprite;
				levelUpTitleText.color = Util.getCalculatedColor(153f, 153f, 153f);
				levelUpPriceText.color = Util.getCalculatedColor(153f, 153f, 153f);
				levelUpGoldIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				afterLevelUpincreaseingValueText.color = Util.getCalculatedColor(153f, 153f, 153f);
			}
		}
		else
		{
			skinChangeButtonImage.color = Util.getCalculatedColor(153f, 153f, 153f);
			Util.changeSpritesColor(currentColleagueUIObject.cachedSpriteRendererList.ToArray(), Util.getCalculatedColor(153f, 153f, 153f));
			iconBackgroundImage.sprite = Singleton<CachedManager>.instance.disableThumbnailSprite;
			nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
			damageText.color = Util.getCalculatedColor(153f, 153f, 153f);
			for (int j = 0; j < passiveSkillRequireLevelTexts.Length; j++)
			{
				passiveSkillRequireLevelTexts[j].color = Util.getCalculatedColor(153f, 153f, 153f);
			}
			if (unlockButtonObject.activeSelf)
			{
				double colleagueUnlockPrice = Singleton<ColleagueManager>.instance.getColleagueUnlockPrice(currentColleagueType);
				if (Singleton<DataManager>.instance.currentGameData.gold >= colleagueUnlockPrice)
				{
					if (!unlockableEffectObject.activeSelf)
					{
						unlockableEffectObject.SetActive(true);
					}
					unlockButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.enableButtonGreenSprite;
					unlockTitleText.color = Util.getCalculatedColor(253f, 254f, 156f);
					unlockPriceText.color = Color.white;
					unlockGoldIconImage.color = Color.white;
				}
				else
				{
					unlockButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.disableButtonSprite;
					unlockTitleText.color = Util.getCalculatedColor(153f, 153f, 153f);
					unlockPriceText.color = Util.getCalculatedColor(153f, 153f, 153f);
					unlockGoldIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				}
			}
		}
		Canvas.ForceUpdateCanvases();
	}

	public void OnClickLevelUp()
	{
		if (Singleton<ColleagueManager>.instance.colleagueLevelUp(m_currentColleagueInventoryData, Singleton<ColleagueManager>.instance.getColleagueLevelUpPrice(currentColleagueType, m_currentColleagueInventoryData.level)))
		{
			upgradeEffect.Stop();
			upgradeEffect.Play();
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			checkQuickUpgrade();
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
			UIWindowDialog.openDescription("NOT_ENOUGH_GOLD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
				UIWindowPopupShop.instance.focusToGold();
			}, string.Empty);
		}
	}

	public void OnClickLevelUpForPremiumColleague()
	{
		if (currentColleagueType != ColleagueManager.ColleagueType.Angela)
		{
			return;
		}
		long currentPremiumColleagueUpgradePrice = Singleton<ColleagueManager>.instance.getCurrentPremiumColleagueUpgradePrice(currentColleagueType);
		if (Singleton<DataManager>.instance.currentGameData._ruby >= currentPremiumColleagueUpgradePrice)
		{
			Singleton<RubyManager>.instance.decreaseRuby(currentPremiumColleagueUpgradePrice);
			m_currentColleagueInventoryData.level++;
			upgradeEffect.Stop();
			upgradeEffect.Play();
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			Singleton<DataManager>.instance.saveData();
			UIWindowColleague.instance.colleagueScrollRectParent.refreshAll();
			UIWindowColleague.instance.refreshTotalDamage();
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}

	public void OnClickQuickUpgrade()
	{
		long level = m_currentColleagueInventoryData.level;
		if (level < Singleton<ColleagueManager>.instance.getNextSkillUnlockLevel(currentColleagueType, false))
		{
			StopCoroutine("waitForCloseAnimation");
			if (Singleton<ColleagueManager>.instance.colleagueLevelUp(m_currentColleagueInventoryData, getCalculatedQuickUpgradePrice(), Singleton<ColleagueManager>.instance.getNextSkillUnlockLevel(currentColleagueType, false) - level))
			{
				upgradeEffect.Stop();
				upgradeEffect.Play();
				Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			}
			else
			{
				Singleton<AudioManager>.instance.playEffectSound("btn_fail");
				UIWindowDialog.openDescription("NOT_ENOUGH_GOLD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
					UIWindowPopupShop.instance.focusToGold();
				}, string.Empty);
			}
			checkQuickUpgrade();
		}
		else
		{
			closeQuickUpgradeButton();
		}
	}

	public void OnClickUnlock()
	{
		bool isPremiumColleague = Singleton<ColleagueManager>.instance.isPremiumColleague(currentColleagueType);
		if (isPremiumColleague && currentColleagueType == ColleagueManager.ColleagueType.Angelina && Singleton<ColleagueManager>.instance.getTotalColleagueSkinCount() < 5)
		{
			UIWindowDialog.openDescription("ANGEILNA_BUY_DESCRIPTION", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			return;
		}
		Action action = delegate
		{
			double num = ((!isPremiumColleague) ? Singleton<ColleagueManager>.instance.getColleagueUnlockPrice(currentColleagueType) : ((double)Singleton<ColleagueManager>.instance.getPremiumColleagueUnlockPrice(currentColleagueType)));
			if (((!isPremiumColleague) ? Singleton<DataManager>.instance.currentGameData.gold : ((double)Singleton<DataManager>.instance.currentGameData._ruby)) >= num)
			{
				m_currentColleagueInventoryData.isUnlocked = true;
				Singleton<AudioManager>.instance.playEffectSound("unlock_weapon");
				if (isPremiumColleague)
				{
					Singleton<RubyManager>.instance.decreaseRuby(num);
					switch (currentColleagueType)
					{
					case ColleagueManager.ColleagueType.Angelica:
						if (Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.AngelicaPackage.ToString()))
						{
							Singleton<ShopManager>.instance.consumeLimitedItem(Singleton<ShopManager>.instance.getLimitedItemDataFromCurrentInventory(ShopManager.LimitedShopItemType.AngelicaPackage.ToString()));
						}
						break;
					case ColleagueManager.ColleagueType.Angelina:
						if (Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.AngelinaPackage.ToString()))
						{
							Singleton<ShopManager>.instance.consumeLimitedItem(Singleton<ShopManager>.instance.getLimitedItemDataFromCurrentInventory(ShopManager.LimitedShopItemType.AngelinaPackage.ToString()));
						}
						break;
					case ColleagueManager.ColleagueType.Goblin:
						if (Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.GoblinPackage.ToString()))
						{
							Singleton<ShopManager>.instance.consumeLimitedItem(Singleton<ShopManager>.instance.getLimitedItemDataFromCurrentInventory(ShopManager.LimitedShopItemType.GoblinPackage.ToString()));
						}
						break;
					}
				}
				else
				{
					Singleton<GoldManager>.instance.decreaseGold(num);
				}
				Singleton<DataManager>.instance.saveData();
				scrollScript.refreshAll();
				unlockEffect.Stop();
				unlockEffect.Play();
				UIWindowColleague.instance.refreshTotalDamage();
				if (currentColleagueType == ColleagueManager.ColleagueType.Isabelle && TutorialManager.isTutorial)
				{
					Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType34);
				}
			}
			else
			{
				Singleton<AudioManager>.instance.playEffectSound("btn_fail");
				if (isPremiumColleague)
				{
					UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
					{
						UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
					}, string.Empty);
				}
				else
				{
					UIWindowDialog.openDescription("NOT_ENOUGH_GOLD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
					{
						UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
						UIWindowPopupShop.instance.focusToGold();
					}, string.Empty);
				}
			}
		};
		if (isPremiumColleague)
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("ANGEILCA_BUY_QUESTION"), Singleton<ColleagueManager>.instance.getColleagueI18NName(currentColleagueType, 1)), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, action, string.Empty);
		}
		else
		{
			action();
		}
	}

	public void OnClickUnlockSkill()
	{
		if (Singleton<ColleagueManager>.instance.unlockPassiveSkill(currentColleagueType))
		{
			Singleton<AudioManager>.instance.playEffectSound("unlock_weapon");
			Singleton<StatManager>.instance.refreshAllStats();
			UIWindowColleague.instance.refreshTotalDamage();
			refreshSlot();
			ColleagueManager.PassiveSkillTierType passiveTierByLevel = Singleton<ColleagueManager>.instance.getPassiveTierByLevel(m_currentColleagueInventoryData.lastPassiveUnlockLevel);
			UIWindowColleaguePassiveSkillUnlock.instance.openPassiveSkillUnlockUI(currentColleagueType, passiveTierByLevel, Singleton<ColleagueManager>.instance.getColleaguePassiveSkillData(currentColleagueType)[passiveTierByLevel]);
		}
		else
		{
			UIWindowDialog.openDescription("REQUIRE_MORE_LEVEL_FOR_UNLOCK_COLLEAGUE_PASSIVE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
		}
	}

	public void OnClickOpenColleagueInformation()
	{
		if (m_currentColleagueInventoryData.isUnlocked && !Singleton<ColleagueManager>.instance.isPremiumColleague(currentColleagueType))
		{
			if (UIWindowColleagueInformation.instance.newSkinColleagueList.Contains(currentColleagueType))
			{
				newSkinIconObject.SetActive(false);
				UIWindowColleagueInformation.instance.newSkinColleagueList.Remove(currentColleagueType);
			}
			UIWindowColleagueInformation.instance.openWithColleagueInformation(currentColleagueType);
		}
	}

	public void OnClickOpenDetailStat()
	{
		UIWindowColleagueStatDetailInformation.instance.openWithStatDetailDescription(currentColleagueType);
	}

	private void OnEnable()
	{
		closeQuickUpgradeButton(true);
	}

	private double getCalculatedQuickUpgradePrice()
	{
		double num = 0.0;
		long level = m_currentColleagueInventoryData.level;
		long nextSkillUnlockLevel = Singleton<ColleagueManager>.instance.getNextSkillUnlockLevel(currentColleagueType, false);
		for (long num2 = level; num2 < nextSkillUnlockLevel; num2++)
		{
			num += Singleton<ColleagueManager>.instance.getColleagueLevelUpPrice(currentColleagueType, num2);
		}
		return num;
	}

	private void checkQuickUpgrade()
	{
		long level = m_currentColleagueInventoryData.level;
		if (TutorialManager.isTutorial || TutorialManager.isRebirthTutorial || !base.cachedGameObject.activeSelf || !base.cachedGameObject.activeInHierarchy)
		{
			closeQuickUpgradeButton();
		}
		else if (Singleton<DataManager>.instance.currentGameData.gold >= getCalculatedQuickUpgradePrice())
		{
			m_quickUpgradeDisappearTimer = 0f;
			openQuickUpgradeButton();
			StopCoroutine("waitForCloseAnimation");
			StopCoroutine("quickUpgradeButtonDisappearUpdate");
			StartCoroutine("quickUpgradeButtonDisappearUpdate");
		}
		else
		{
			closeQuickUpgradeButton();
		}
	}

	private void openQuickUpgradeButton()
	{
		quickUpgradeTotalPriceText.text = GameManager.changeUnit(getCalculatedQuickUpgradePrice());
		quickUpgradeTargetLevelText.text = "<size=20>Lv.</size>" + Singleton<ColleagueManager>.instance.getNextSkillUnlockLevel(currentColleagueType, false);
		if (!isQuickUpgrading)
		{
			quickUpgradeButtonObject.SetActive(false);
			quickUpgradeButtonObject.SetActive(true);
			isQuickUpgrading = true;
		}
	}

	private void closeQuickUpgradeButton(bool forceClose = false)
	{
		StopCoroutine("quickUpgradeButtonDisappearUpdate");
		m_quickUpgradeDisappearTimer = 0f;
		if (quickUpgradeButtonObject.activeSelf)
		{
			if (!forceClose)
			{
				if (!quickUpgradeButtonAnimation.IsPlaying("WeaponQuickUpgradeButtonCloseAnimation"))
				{
					quickUpgradeButtonAnimation.Stop();
					quickUpgradeButtonAnimation.Play("WeaponQuickUpgradeButtonCloseAnimation");
					StopCoroutine("waitForCloseAnimation");
					StartCoroutine("waitForCloseAnimation");
				}
			}
			else
			{
				quickUpgradeButtonObject.SetActive(false);
			}
		}
		isQuickUpgrading = false;
	}

	private IEnumerator waitForCloseAnimation()
	{
		yield return new WaitWhile(() => quickUpgradeButtonAnimation.isPlaying);
		quickUpgradeButtonObject.SetActive(false);
	}

	private IEnumerator quickUpgradeButtonDisappearUpdate()
	{
		while (m_quickUpgradeDisappearTimer < 1.5f)
		{
			m_quickUpgradeDisappearTimer += Time.deltaTime;
			yield return null;
		}
		closeQuickUpgradeButton();
	}
}
