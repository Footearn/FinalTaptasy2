using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : SlotObject
{
	public bool isLimitedItem;

	public GameObject limitedObject;

	public GameObject ribbonObject;

	public Text ribbonText;

	public Image mainBackgroundImage;

	public Image[] buttonsImages;

	public Text titleNameText;

	public Text descriptionText;

	public Image itemIconImage;

	public Transform resourcesEffectTargetTransform;

	public GameObject limitedVIPTimerObject;

	public GameObject limitedTimerObject;

	public Text limitedTimerText;

	public GameObject vipTimerObject;

	public Text vipTimerText;

	public GameObject limitedItemPurchaseButtonObject;

	public GameObject defaultButtonSetObject;

	public bool isDefailtItemTypeRuby;

	public ShopManager.DefaultRubyShopItemType currentDefaultRubyItemType;

	public ShopManager.DefaultGoldShopItemType currentDefaultGoldItemType;

	public ShopManager.PurchaseType currentPurchaseType;

	public GameObject purchaseByCashButtonObject;

	public Text purchaseByCashPriceText;

	public GameObject purchaseByRubyButtonObject;

	public Text purchaseByRubyPriceText;

	public Image purchaseByRubyButtonBackgroundImage;

	public Image purchaseByRubyButtonRubyIconImage;

	public GameObject nonePurchaseButtonObject;

	public GameObject noneButtonObject;

	public GameObject timerObject;

	public Text timerText;

	public GameObject adsIconObject;

	public GameObject nonPurchaseTextObject;

	public Text nonePurchaseText;

	private long m_currentRubyValue;

	public ShopManager.LimitedShopItemType currentLimitedItemType;

	public ShopManager.LimitedItemData currentLimitedItemData;

	public Text limitedItemPriceText;

	public Text remainText;

	protected override void Awake()
	{
		I18NManager.changeLanguageNotification = (Action)Delegate.Combine(I18NManager.changeLanguageNotification, new Action(refreshSlot));
		base.Awake();
	}

	public override void initSlot()
	{
		base.initSlot();
		refreshSlot();
	}

	public override void refreshSlot()
	{
		mainBackgroundImage.transform.SetAsFirstSibling();
		descriptionText.rectTransform.anchoredPosition = new Vector2(0f, -40.9f);
		descriptionText.rectTransform.sizeDelta = new Vector2(239.83f, 54f);
		if (isLimitedItem)
		{
			limitedObject.SetActive(true);
			refreshSlotForLimitedItem();
			if (Singleton<ShopManager>.instance.isPremiumItem(currentLimitedItemType))
			{
				titleNameText.color = Util.getCalculatedColor(252f, 250f, 60f);
				descriptionText.color = Util.getCalculatedColor(14f, 228f, 230f);
				mainBackgroundImage.sprite = Singleton<CachedManager>.instance.limitedItemBackground;
				for (int i = 0; i < buttonsImages.Length; i++)
				{
					buttonsImages[i].sprite = Singleton<CachedManager>.instance.enableButtonGreenSprite;
				}
			}
			else if (Singleton<ShopManager>.instance.isSkinPackage(currentLimitedItemType))
			{
				titleNameText.color = Util.getCalculatedColor(252f, 250f, 60f);
				descriptionText.color = Util.getCalculatedColor(255f, 148f, 213f);
				mainBackgroundImage.sprite = Singleton<CachedManager>.instance.skinItemBackground;
				for (int j = 0; j < buttonsImages.Length; j++)
				{
					buttonsImages[j].sprite = Singleton<CachedManager>.instance.enableButtonOrangeSprite;
				}
			}
			else if (Singleton<ShopManager>.instance.isItemPackage(currentLimitedItemType))
			{
				titleNameText.color = Util.getCalculatedColor(253f, 252f, 183f);
				descriptionText.color = Util.getCalculatedColor(165f, 137f, 97f);
				mainBackgroundImage.sprite = Singleton<CachedManager>.instance.goldItemBackground;
				for (int k = 0; k < buttonsImages.Length; k++)
				{
					buttonsImages[k].sprite = Singleton<CachedManager>.instance.enableButtonOrangeSprite;
				}
			}
			else if (Singleton<ShopManager>.instance.isDiamondPackage(currentLimitedItemType))
			{
				titleNameText.color = Color.white;
				descriptionText.color = Util.getCalculatedColor(167f, 121f, 219f);
				mainBackgroundImage.sprite = Singleton<CachedManager>.instance.rubyItemBackground;
				for (int l = 0; l < buttonsImages.Length; l++)
				{
					buttonsImages[l].sprite = Singleton<CachedManager>.instance.enableButtonPurpleSprite;
				}
			}
			return;
		}
		if (isDefailtItemTypeRuby)
		{
			for (int m = 0; m < buttonsImages.Length; m++)
			{
				buttonsImages[m].sprite = Singleton<CachedManager>.instance.enableButtonPurpleSprite;
			}
			mainBackgroundImage.sprite = Singleton<CachedManager>.instance.rubyItemBackground;
			titleNameText.color = Color.white;
			descriptionText.color = Util.getCalculatedColor(167f, 121f, 219f);
		}
		else
		{
			for (int n = 0; n < buttonsImages.Length; n++)
			{
				buttonsImages[n].sprite = Singleton<CachedManager>.instance.enableButtonGreenSprite;
			}
			mainBackgroundImage.sprite = Singleton<CachedManager>.instance.goldItemBackground;
			titleNameText.color = Util.getCalculatedColor(253f, 252f, 183f);
			descriptionText.color = Util.getCalculatedColor(165f, 137f, 97f);
		}
		limitedObject.SetActive(false);
		refreshSlotForDefaultItem();
	}

	private void refreshSlotForDefaultItem()
	{
		defaultButtonSetObject.SetActive(true);
		ribbonObject.SetActive(false);
		if (isDefailtItemTypeRuby)
		{
			currentPurchaseType = Singleton<ShopManager>.instance.getPurchaseType(currentDefaultRubyItemType);
			m_currentRubyValue = Singleton<ShopManager>.instance.getProductValue(currentDefaultRubyItemType);
			switch (currentDefaultRubyItemType)
			{
			case ShopManager.DefaultRubyShopItemType.AdsRuby:
				ribbonObject.SetActive(true);
				ribbonText.text = "Free";
				titleNameText.text = Singleton<ShopManager>.instance.getDefaultItemTitleName(currentDefaultRubyItemType);
				descriptionText.text = Singleton<ShopManager>.instance.getDefaultItemDescription(currentDefaultRubyItemType, m_currentRubyValue);
				break;
			case ShopManager.DefaultRubyShopItemType.Tapjoy:
				nonePurchaseText.text = I18NManager.Get("SHOP_CLICK_FREE_BUTTON");
				ribbonObject.SetActive(true);
				ribbonText.text = "Free";
				titleNameText.text = Singleton<ShopManager>.instance.getDefaultItemTitleName(currentDefaultRubyItemType);
				descriptionText.text = Singleton<ShopManager>.instance.getDefaultItemDescription(currentDefaultRubyItemType, m_currentRubyValue);
				break;
			case ShopManager.DefaultRubyShopItemType.RubyTier1:
			case ShopManager.DefaultRubyShopItemType.RubyTier2:
				purchaseByCashPriceText.text = Singleton<ShopManager>.instance.getRubyProductPriceString(currentDefaultRubyItemType);
				titleNameText.text = Singleton<ShopManager>.instance.getDefaultItemTitleName(currentDefaultRubyItemType, m_currentRubyValue);
				descriptionText.text = Singleton<ShopManager>.instance.getDefaultItemDescription(currentDefaultRubyItemType, 0L);
				break;
			case ShopManager.DefaultRubyShopItemType.RubyTier3:
			case ShopManager.DefaultRubyShopItemType.RubyTier4:
			case ShopManager.DefaultRubyShopItemType.RubyTier5:
			case ShopManager.DefaultRubyShopItemType.RubyTier6:
				purchaseByCashPriceText.text = Singleton<ShopManager>.instance.getRubyProductPriceString(currentDefaultRubyItemType);
				titleNameText.text = Singleton<ShopManager>.instance.getDefaultItemTitleName(currentDefaultRubyItemType, m_currentRubyValue);
				descriptionText.text = Singleton<ShopManager>.instance.getDefaultItemDescription(currentDefaultRubyItemType, 0L);
				break;
			}
			if (currentDefaultRubyItemType == ShopManager.DefaultRubyShopItemType.RubyTier4)
			{
				ribbonObject.SetActive(true);
				ribbonText.text = "Hot!";
			}
			else if (currentDefaultRubyItemType == ShopManager.DefaultRubyShopItemType.RubyTier6)
			{
				ribbonObject.SetActive(true);
				ribbonText.text = "Best!";
			}
			itemIconImage.sprite = Singleton<ShopManager>.instance.getShopItemIcon(currentDefaultRubyItemType);
		}
		else
		{
			double value = 0.0;
			currentPurchaseType = Singleton<ShopManager>.instance.getPurchaseType(currentDefaultGoldItemType);
			m_currentRubyValue = Singleton<ShopManager>.instance.getProductValue(currentDefaultGoldItemType);
			titleNameText.text = Singleton<ShopManager>.instance.getDefaultItemTitleName(currentDefaultGoldItemType);
			switch (currentDefaultGoldItemType)
			{
			case ShopManager.DefaultGoldShopItemType.AdsGold:
				value = CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.AdsGoldMultiply);
				nonePurchaseText.text = I18NManager.Get("SHOP_CLICK_FREE_BUTTON");
				ribbonObject.SetActive(true);
				ribbonText.text = "Free";
				break;
			case ShopManager.DefaultGoldShopItemType.GoldTier1:
				value = CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BuyGoldTier1Multiply);
				purchaseByRubyPriceText.text = m_currentRubyValue.ToString();
				break;
			case ShopManager.DefaultGoldShopItemType.GoldTier2:
				value = CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BuyGoldTier2Multiply);
				purchaseByRubyPriceText.text = m_currentRubyValue.ToString();
				break;
			case ShopManager.DefaultGoldShopItemType.GoldTier3:
				value = CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BuyGoldTier3Multiply);
				purchaseByRubyPriceText.text = m_currentRubyValue.ToString();
				break;
			}
			descriptionText.text = Singleton<ShopManager>.instance.getDefaultItemDescription(currentDefaultGoldItemType, value);
			itemIconImage.sprite = Singleton<ShopManager>.instance.getShopItemIcon(currentDefaultGoldItemType);
		}
		purchaseByCashButtonObject.SetActive(false);
		purchaseByRubyButtonObject.SetActive(false);
		nonePurchaseButtonObject.SetActive(false);
		switch (currentPurchaseType)
		{
		case ShopManager.PurchaseType.None:
			nonePurchaseButtonObject.SetActive(true);
			noneButtonObject.SetActive(true);
			timerObject.SetActive(false);
			nonPurchaseTextObject.SetActive(true);
			adsIconObject.SetActive(false);
			break;
		case ShopManager.PurchaseType.Ads:
			nonePurchaseButtonObject.SetActive(true);
			if (isDefailtItemTypeRuby)
			{
				if (Singleton<DataManager>.instance.currentGameData.adsRubyWatchEndTime > UnbiasedTime.Instance.Now().Ticks)
				{
					noneButtonObject.SetActive(false);
					timerObject.SetActive(true);
				}
				else
				{
					noneButtonObject.SetActive(true);
					timerObject.SetActive(false);
				}
			}
			else if (Singleton<DataManager>.instance.currentGameData.adsGoldWatchEndTime > UnbiasedTime.Instance.Now().Ticks)
			{
				noneButtonObject.SetActive(false);
				timerObject.SetActive(true);
			}
			else
			{
				noneButtonObject.SetActive(true);
				timerObject.SetActive(false);
			}
			nonPurchaseTextObject.SetActive(false);
			adsIconObject.SetActive(true);
			break;
		case ShopManager.PurchaseType.TapJoy:
			nonePurchaseButtonObject.SetActive(true);
			noneButtonObject.SetActive(true);
			timerObject.SetActive(false);
			nonPurchaseTextObject.SetActive(true);
			adsIconObject.SetActive(false);
			break;
		case ShopManager.PurchaseType.BuyRuby:
			purchaseByRubyButtonObject.SetActive(true);
			break;
		case ShopManager.PurchaseType.BuyCash:
			purchaseByCashButtonObject.SetActive(true);
			break;
		}
		itemIconImage.SetNativeSize();
	}

	private void refreshSlotForLimitedItem()
	{
		if (currentLimitedItemData.currentItemCount <= 1)
		{
			ribbonObject.SetActive(true);
			ribbonText.text = I18NManager.Get("SHOP_LIMITED_TITLE_TEXT");
		}
		else
		{
			string limitedItemRibbonTitleText = Singleton<ShopManager>.instance.getLimitedItemRibbonTitleText(currentLimitedItemType);
			if (string.IsNullOrEmpty(limitedItemRibbonTitleText))
			{
				ribbonObject.SetActive(false);
			}
			else
			{
				ribbonObject.SetActive(true);
				ribbonText.text = limitedItemRibbonTitleText;
			}
		}
		limitedTimerObject.SetActive(true);
		if (currentLimitedItemType == ShopManager.LimitedShopItemType.VIPPackage || currentLimitedItemType == ShopManager.LimitedShopItemType.MiniVIPPackage)
		{
			limitedVIPTimerObject.SetActive(true);
			limitedTimerObject.SetActive(false);
			if (Singleton<NanooAPIManager>.instance.GetVipItemCount() > 0)
			{
				vipTimerObject.SetActive(true);
				vipTimerText.text = " " + Singleton<NanooAPIManager>.instance.GetVipItemCount() + I18NManager.Get("SHOP_LIMITED_DAY_SIMPLE") + " " + I18NManager.Get("TIME_LEFT");
			}
			else
			{
				vipTimerObject.SetActive(false);
			}
		}
		else
		{
			limitedVIPTimerObject.SetActive(false);
			ribbonText.text = I18NManager.Get("SHOP_LIMITED_TITLE_TEXT");
		}
		defaultButtonSetObject.SetActive(false);
		for (int i = 0; i < currentLimitedItemData.limitedAttribudeList.Count; i++)
		{
			if (currentLimitedItemData.limitedAttribudeList[i].limitedItemType == ShopManager.LimitedAttributeType.Ruby)
			{
				m_currentRubyValue = currentLimitedItemData.limitedAttribudeList[i].value;
			}
		}
		string marketPrice = Singleton<PaymentManager>.instance.GetMarketPrice(currentLimitedItemType);
		if (marketPrice.Length > 0)
		{
			limitedItemPriceText.text = marketPrice;
		}
		else
		{
			limitedItemPriceText.text = currentLimitedItemData.itemPriceString;
		}
		titleNameText.text = Singleton<ShopManager>.instance.getLimitedItemTitleName(currentLimitedItemType);
		bool flag = false;
		double count = 0.0;
		for (int j = 0; j < currentLimitedItemData.limitedAttribudeList.Count; j++)
		{
			if (currentLimitedItemData.limitedAttribudeList[j].limitedItemType == ShopManager.LimitedAttributeType.Gold || currentLimitedItemData.limitedAttribudeList[j].limitedItemType == ShopManager.LimitedAttributeType.CountableGoldFinger || currentLimitedItemData.limitedAttribudeList[j].limitedItemType == ShopManager.LimitedAttributeType.CountableSilverFinger || currentLimitedItemData.limitedAttribudeList[j].limitedItemType == ShopManager.LimitedAttributeType.TranscendStone)
			{
				count = currentLimitedItemData.limitedAttribudeList[j].value;
				flag = true;
			}
		}
		if (!Singleton<ShopManager>.instance.isInfinityCountProduct(currentLimitedItemType))
		{
			remainText.text = string.Format(I18NManager.Get("REMAIN_TEXT"), currentLimitedItemData.currentItemCount);
		}
		else
		{
			remainText.text = string.Empty;
		}
		if (!flag)
		{
			descriptionText.text = Singleton<ShopManager>.instance.getLimitedItemDescription(currentLimitedItemType, m_currentRubyValue);
		}
		else if (currentLimitedItemType == ShopManager.LimitedShopItemType.GoldFinger500)
		{
			descriptionText.text = Singleton<ShopManager>.instance.getLimitedItemDescription(currentLimitedItemType, currentLimitedItemData.limitedAttribudeList[0].value);
		}
		else
		{
			descriptionText.text = Singleton<ShopManager>.instance.getLimitedItemDescription(currentLimitedItemType, m_currentRubyValue, count, true);
		}
		if (new DateTime(currentLimitedItemData.targetLimitedItemEndTime).Subtract(new DateTime(UnbiasedTime.Instance.Now().Ticks)).TotalDays >= 180.0 && currentLimitedItemType != ShopManager.LimitedShopItemType.VIPPackage && currentLimitedItemType != ShopManager.LimitedShopItemType.MiniVIPPackage)
		{
			limitedTimerObject.SetActive(false);
			descriptionText.rectTransform.anchoredPosition = new Vector2(0f, -62.1f);
			descriptionText.rectTransform.sizeDelta = new Vector2(239.83f, 96.4f);
		}
		else if ((currentLimitedItemType == ShopManager.LimitedShopItemType.VIPPackage || currentLimitedItemType == ShopManager.LimitedShopItemType.MiniVIPPackage) && !Singleton<NanooAPIManager>.instance.IsExistSubscriptionItem() && Singleton<NanooAPIManager>.instance.GetVipItemCount() <= 0)
		{
			descriptionText.rectTransform.anchoredPosition = new Vector2(0f, -62.1f);
			descriptionText.rectTransform.sizeDelta = new Vector2(239.83f, 96.4f);
		}
		itemIconImage.sprite = Singleton<ShopManager>.instance.getShopItemIcon(currentLimitedItemType);
		itemIconImage.SetNativeSize();
	}

	private void Update()
	{
		if (isLimitedItem)
		{
			if (currentLimitedItemData.targetLimitedItemEndTime >= UnbiasedTime.Instance.Now().Ticks)
			{
				TimeSpan timeSpan = new DateTime(currentLimitedItemData.targetLimitedItemEndTime).Subtract(new DateTime(UnbiasedTime.Instance.Now().Ticks));
				if (timeSpan.TotalDays < 180.0)
				{
					string text = string.Format(((timeSpan.Days == 0) ? string.Empty : ("{0:D1}" + I18NManager.Get("SHOP_LIMITED_DAY_SIMPLE") + " ")) + ((timeSpan.Hours == 0) ? string.Empty : "{1:00}:") + ((timeSpan.Minutes == 0) ? string.Empty : "{2:00}:") + "{3:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
					limitedTimerText.text = text;
				}
				return;
			}
			Singleton<ShopManager>.instance.refreshLimitedItemList();
			if (UIWindowPopupShop.instance.isOpen)
			{
				UIWindowPopupShop.instance.openShopPopupWithType(UIWindowPopupShop.instance.currentSelectedType);
			}
			if (UIWindowManageShop.instance.isOpen)
			{
				UIWindowManageShop.instance.openShopUI(UIWindowManageShop.instance.currentSelectedType);
			}
			if (UIWindowAdvertisementLimitedProduct.instance.isOpen)
			{
				UIWindowAdvertisementLimitedProduct.instance.OnClickClose();
			}
		}
		else
		{
			if (currentPurchaseType != ShopManager.PurchaseType.Ads)
			{
				return;
			}
			if (isDefailtItemTypeRuby)
			{
				if (Singleton<DataManager>.instance.currentGameData.adsRubyWatchEndTime > UnbiasedTime.Instance.Now().Ticks)
				{
					TimeSpan timeSpan2 = new DateTime(Singleton<DataManager>.instance.currentGameData.adsRubyWatchEndTime).Subtract(new DateTime(UnbiasedTime.Instance.Now().Ticks));
					timerText.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan2.Hours, timeSpan2.Minutes, timeSpan2.Seconds) + " " + I18NManager.Get("TIME_LEFT");
					return;
				}
				if (!noneButtonObject.activeSelf)
				{
					noneButtonObject.SetActive(true);
				}
				if (!timerObject.activeSelf)
				{
					timerObject.SetActive(false);
				}
			}
			else if (Singleton<DataManager>.instance.currentGameData.adsGoldWatchEndTime > UnbiasedTime.Instance.Now().Ticks)
			{
				TimeSpan timeSpan3 = new DateTime(Singleton<DataManager>.instance.currentGameData.adsGoldWatchEndTime).Subtract(new DateTime(UnbiasedTime.Instance.Now().Ticks));
				timerText.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan3.Hours, timeSpan3.Minutes, timeSpan3.Seconds) + " " + I18NManager.Get("TIME_LEFT");
			}
			else
			{
				if (!noneButtonObject.activeSelf)
				{
					noneButtonObject.SetActive(true);
				}
				if (!timerObject.activeSelf)
				{
					timerObject.SetActive(false);
				}
			}
		}
	}

	public void OnClickBuyShopItem()
	{
		if (isLimitedItem)
		{
			if (Util.isInternetConnection())
			{
				if (currentLimitedItemType == ShopManager.LimitedShopItemType.VIPPackage || currentLimitedItemType == ShopManager.LimitedShopItemType.MiniVIPPackage)
				{
					UIWindowDialog.openDescription("VIP_PURCHASE_DESCRIPTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
					{
						Singleton<ShopManager>.instance.purchaseLimitedItemEvent(currentLimitedItemData, resourcesEffectTargetTransform);
					}, string.Empty);
				}
				else
				{
					Action action = delegate
					{
						Singleton<ShopManager>.instance.purchaseLimitedItemEvent(currentLimitedItemData, resourcesEffectTargetTransform);
					};
					bool flag = false;
					List<ColleagueManager.ColleagueType> list = new List<ColleagueManager.ColleagueType>();
					if (currentLimitedItemData != null && currentLimitedItemData.limitedAttribudeList != null)
					{
						List<ShopManager.LimitedItemSetData> limitedAttribudeList = currentLimitedItemData.limitedAttribudeList;
						for (int i = 0; i < limitedAttribudeList.Count; i++)
						{
							if (limitedAttribudeList[i].limitedItemType == ShopManager.LimitedAttributeType.ColleagueSkin)
							{
								ColleagueManager.ColleagueType colleagueType = (ColleagueManager.ColleagueType)(limitedAttribudeList[i].value - 1);
								if (!Singleton<ColleagueManager>.instance.getColleagueInventoryData(colleagueType).isUnlocked)
								{
									list.Add(colleagueType);
								}
								flag = true;
							}
						}
					}
					if (flag)
					{
						if (list.Count <= 0)
						{
							action();
						}
						else
						{
							string text = string.Empty;
							for (int j = 0; j < list.Count; j++)
							{
								text = text + Singleton<ColleagueManager>.instance.getColleagueI18NName(list[j], 1) + ((j == list.Count - 1) ? string.Empty : ", ");
							}
							UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("FAIL_PURCHASE_COLLEAGUE_SKIN_PACKAGE"), text), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						}
					}
					else
					{
						action();
					}
				}
			}
			else
			{
				UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		}
		else if (isDefailtItemTypeRuby)
		{
			if (Util.isInternetConnection())
			{
				Singleton<ShopManager>.instance.purchaseDefaultItemEvent(currentDefaultRubyItemType, this);
			}
			else
			{
				UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		}
		else
		{
			Singleton<ShopManager>.instance.purchaseDefaultItemEvent(currentDefaultGoldItemType, this);
		}
		UIWindowManageShop.instance.refreshSlots();
	}
}
