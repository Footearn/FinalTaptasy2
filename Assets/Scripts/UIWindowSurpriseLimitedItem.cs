using UnityEngine;
using UnityEngine.UI;

public class UIWindowSurpriseLimitedItem : UIWindow
{
	public static UIWindowSurpriseLimitedItem instance;

	public Transform silverFingerButtonTransform;

	public Transform premiumPackageButtonTransform;

	public Text silverFingerNameText;

	public Text silverFingerDescriptionText;

	public Text silverFingerPriceText;

	public Image premiumItemImage;

	public Text premiumItemNameText;

	public Text premiumItemDescriptionText;

	public Text premiumItemPriceText;

	public SpriteAnimation princessSpriteAnimation;

	private ShopManager.LimitedItemData m_premiumItemData;

	private ShopManager.LimitedItemData m_goldFinger500ItemData;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	private void initItemData()
	{
		if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(CharacterSkinManager.WarriorSkinType.Dragoon).isHaving)
		{
			m_premiumItemData = Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.LimitedSkinPremiumPack];
		}
		else
		{
			m_premiumItemData = Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.BadPriemiumPack];
		}
		m_goldFinger500ItemData = Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.GoldFinger500];
	}

	public void openLimitedPopupUI()
	{
		if (Singleton<DataManager>.instance.currentGameData.isBoughtAutoTouch || Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime > UnbiasedTime.Instance.Now().Ticks || Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime > UnbiasedTime.Instance.Now().Ticks || Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount > 0 || Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount > 0)
		{
			return;
		}
		initItemData();
		silverFingerNameText.text = Singleton<ShopManager>.instance.getLimitedItemTitleName(m_goldFinger500ItemData.limitedType);
		silverFingerDescriptionText.text = Singleton<ShopManager>.instance.getLimitedItemDescription(m_goldFinger500ItemData.limitedType, m_goldFinger500ItemData.limitedAttribudeList[0].value);
		string marketPrice = Singleton<PaymentManager>.instance.GetMarketPrice(m_goldFinger500ItemData.limitedType);
		if (marketPrice.Length > 0)
		{
			silverFingerPriceText.text = marketPrice;
		}
		else
		{
			silverFingerPriceText.text = m_goldFinger500ItemData.itemPriceString;
		}
		long count = 0L;
		for (int i = 0; i < m_premiumItemData.limitedAttribudeList.Count; i++)
		{
			if (m_premiumItemData.limitedAttribudeList[i].limitedItemType == ShopManager.LimitedAttributeType.Ruby)
			{
				count = m_premiumItemData.limitedAttribudeList[i].value;
			}
		}
		premiumItemImage.sprite = Singleton<ShopManager>.instance.getShopItemIcon(m_premiumItemData.limitedType);
		premiumItemNameText.text = Singleton<ShopManager>.instance.getLimitedItemTitleName(m_premiumItemData.limitedType);
		premiumItemDescriptionText.text = Singleton<ShopManager>.instance.getLimitedItemDescription(m_premiumItemData.limitedType, count);
		marketPrice = Singleton<PaymentManager>.instance.GetMarketPrice(m_premiumItemData.limitedType);
		if (marketPrice.Length > 0)
		{
			premiumItemPriceText.text = marketPrice;
		}
		else
		{
			premiumItemPriceText.text = m_premiumItemData.itemPriceString;
		}
		premiumItemImage.SetNativeSize();
		AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.SurpriseShopOpen);
		open();
	}

	public override void OnAfterActiveGameObject()
	{
		princessSpriteAnimation.animationType = "Princess" + GameManager.getCurrentPrincessNumber();
		princessSpriteAnimation.init();
		princessSpriteAnimation.playAnimation("Move", 0.18f, true);
		base.OnAfterActiveGameObject();
	}

	public void OnClickClose()
	{
		UIWindowDialog.openDescription("QUESTION_CLOSE_LIMITED_POPUP", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.SurpriseShopClose);
			close();
		}, string.Empty);
	}

	public void OnClickBuySilverFinger()
	{
		Singleton<ShopManager>.instance.purchaseLimitedItemEvent(m_goldFinger500ItemData, silverFingerButtonTransform);
	}

	public void OnClickBuyPremiumPackage()
	{
		Singleton<ShopManager>.instance.purchaseLimitedItemEvent(m_premiumItemData, premiumPackageButtonTransform);
	}
}
