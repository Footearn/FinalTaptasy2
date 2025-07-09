using UnityEngine.UI;

public class UIWindowBuyTowerModeTicket : UIWindow
{
	public static UIWindowBuyTowerModeTicket instance;

	public Text adsTicketTitleText;

	public Text adsTicketDescriptionText;

	public Text freeTicketDescriptionText;

	public Text freeTicketPriceText;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		adsTicketTitleText.text = string.Format(I18NManager.Get("TOWER_MODE_ADS_TICKET_TITLE_TEXT"), TowerModeManager.adsTicketGainCount);
		adsTicketDescriptionText.text = string.Format(I18NManager.Get("TOWER_MODE_ADS_TICKET_DESCRIPTION_TEXT"), TowerModeManager.adsTicketGainCount);
		freeTicketDescriptionText.text = string.Format(I18NManager.Get("SHOP_LIMITED_DESCRIPTION_TEXT_28"), TowerModeManager.freeTicketHour);
		string marketPrice = Singleton<PaymentManager>.instance.GetMarketPrice(ShopManager.LimitedShopItemType.TowerModeFreeTicketPackage);
		if (marketPrice.Length > 0)
		{
			freeTicketPriceText.text = marketPrice;
		}
		else
		{
			freeTicketPriceText.text = Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.TowerModeFreeTicketPackage].itemPriceString;
		}
		return base.OnBeforeOpen();
	}

	public void OnClickAdsToGetButton()
	{
		if (!Singleton<AdsManager>.instance.isReady("towermodeads"))
		{
			return;
		}
		Singleton<AdsManager>.instance.showAds("towermodeads", delegate
		{
			Singleton<TowerModeManager>.instance.increaseTicket(TowerModeManager.adsTicketGainCount, false);
			if (UIWindowSelectTowerModeDifficulty.instance.isOpen)
			{
				UIWindowSelectTowerModeDifficulty.instance.refreshUI();
			}
			if (UIWindowTowerModeResult.instance.isOpen)
			{
				UIWindowTowerModeResult.instance.refreshRetryButton();
			}
		});
	}

	public void OnClickPurchaseFreeTicket()
	{
		Singleton<PaymentManager>.instance.Purchase(ShopManager.LimitedShopItemType.TowerModeFreeTicketPackage, delegate
		{
			Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.TowerModeFreeTicketPackage], base.cachedTransform)();
			if (UIWindowTowerModeResult.instance.isOpen)
			{
				UIWindowTowerModeResult.instance.refreshRetryButton();
			}
			close();
		});
	}
}
