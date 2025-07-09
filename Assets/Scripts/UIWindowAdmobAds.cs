using UnityEngine.UI;

public class UIWindowAdmobAds : UIWindow
{
	public static UIWindowAdmobAds instance;

	public Text firstPurchasePriceText;

	private ShopManager.LimitedItemData m_firstPurchaseItemData;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openAdmobAdsUI()
	{
		m_firstPurchaseItemData = Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.FirstPurchasePackage];
		string marketPrice = Singleton<PaymentManager>.instance.GetMarketPrice(m_firstPurchaseItemData.limitedType);
		if (marketPrice.Length > 0)
		{
			firstPurchasePriceText.text = marketPrice;
		}
		else
		{
			firstPurchasePriceText.text = m_firstPurchaseItemData.itemPriceString;
		}
		open();
	}

	public void OnClickWatchAds()
	{
		Singleton<AdsManager>.instance.showAds("interSkip", delegate
		{
			Singleton<DataManager>.instance.currentGameData.admobAdsIngnoreTargetTime = UnbiasedTime.Instance.UtcNow().AddMinutes(20.0).Ticks;
		});
		close();
	}

	public void OnClickBuyFirstPurchase()
	{
		Singleton<ShopManager>.instance.purchaseLimitedItemEvent(m_firstPurchaseItemData, base.cachedTransform, delegate
		{
			Singleton<DataManager>.instance.saveData();
			close();
		});
	}

	public void OnClickClose()
	{
		Singleton<AdsManager>.instance.showAdmobAds(delegate
		{
			UIWindowResult.instance.startNextTimer();
		});
		close();
	}
}
