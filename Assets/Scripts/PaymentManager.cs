using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Fabric.Answers;
using Prime31;
using TapjoyUnity;
using UnityEngine;

public class PaymentManager : Singleton<PaymentManager>
{
	private class DefaultProductData
	{
		public double value;

		public string skuID;

		public DefaultProductData(double value, string skuID)
		{
			this.value = value;
			this.skuID = skuID;
		}
	}

	[Serializable]
	public class GoogleSkuInfoForJson
	{
		public string productId;

		public string type;

		public string price;

		public long price_amount_micros;

		public string price_currency_code;

		public string title;

		public string description;
	}

	private bool _isInited;

	private bool _listnersAdded;

	private string m_purchaseData;

	private string m_signature;

	private Action m_purchasePurchaseSuccessEvent;

	private ShopManager.DefaultRubyShopItemType m_currentPurchasingNonLimitedItemType;

	private ShopManager.LimitedShopItemType m_currentPurchasingLimitedType;

	private ShopManager.ElopeModeItemType m_currentPurchasingElopeModeProductType;

	private List<GoogleSkuInfo> m_skuDetailList;

	private Dictionary<ShopManager.DefaultRubyShopItemType, string> m_nonLimitedProductSKUDataDictionary;

	private Dictionary<ShopManager.LimitedShopItemType, string> m_limitedProductSKUDataDictionary;

	private Dictionary<ShopManager.ElopeModeItemType, string> m_elopeModeProductSKUDataDictionary;

	public bool isInited
	{
		get
		{
			return _isInited;
		}
	}

	public Dictionary<ShopManager.LimitedShopItemType, string> LimitedProductSKUDataDictionary
	{
		get
		{
			return m_limitedProductSKUDataDictionary;
		}
	}

	public Dictionary<ShopManager.ElopeModeItemType, string> ElopeModeProductSKUDataDictionary
	{
		get
		{
			return m_elopeModeProductSKUDataDictionary;
		}
	}

	private void Awake()
	{
		if (!_listnersAdded)
		{
			m_nonLimitedProductSKUDataDictionary = new Dictionary<ShopManager.DefaultRubyShopItemType, string>();
			m_limitedProductSKUDataDictionary = new Dictionary<ShopManager.LimitedShopItemType, string>();
			m_elopeModeProductSKUDataDictionary = new Dictionary<ShopManager.ElopeModeItemType, string>();
			m_nonLimitedProductSKUDataDictionary.Add(ShopManager.DefaultRubyShopItemType.RubyTier1, "diamond_tier3");
			m_nonLimitedProductSKUDataDictionary.Add(ShopManager.DefaultRubyShopItemType.RubyTier2, "diamond_tier5");
			m_nonLimitedProductSKUDataDictionary.Add(ShopManager.DefaultRubyShopItemType.RubyTier3, "diamond_tier10");
			m_nonLimitedProductSKUDataDictionary.Add(ShopManager.DefaultRubyShopItemType.RubyTier4, "diamond_tier20");
			m_nonLimitedProductSKUDataDictionary.Add(ShopManager.DefaultRubyShopItemType.RubyTier5, "diamond_tier50");
			m_nonLimitedProductSKUDataDictionary.Add(ShopManager.DefaultRubyShopItemType.RubyTier6, "diamond_tier60");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.BadPriemiumPack, "silverfinger_package_tier50");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.PremiumPack, "premium_package_tier50");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.OnePlusOneDiamonds, "oneplus_diamond_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.AllCharacterPack, "skin1_tier30");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.AngelicaPackage, "angelica_package_tier50");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.SilverFinger, "autotouch_tier3");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.EventPackage, "monthly_diamond_tier30");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.LimitedSkinPremiumPack, "limited_skin_package_tier50");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.FirstPurchasePackage, "first_purchase_package_tier1");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.VIPPackage, "monthly_vip_pack_tier25");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.MiniVIPPackage, "monthly_mini_vip_pack_tier8");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.GoldFinger500, "gold_finger_count500_tier4");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.NewTranscendEventPackage, "new_monthly_transcend_tier30");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.ValkyrieSkinPackage, "valkyrie_skin_package_tier20");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.IceMeteorSkillPackage, "ice_meteor_passive_skill_package_tier25");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.AngelinaPackage, "angelina_package_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.ReinforcementSkillPackage, "reinforcement_skill_package_tier25");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.BrotherhoodPackage, "brother_hood_package_tier50");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.DemonKingSkinPackage, "demon_king_skin_package_tier25");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.AngelaPackage, "angela_package_tier40");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.GoblinPackage, "goblin_package_tier50");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.ZeusPackage, "zeus_package_tier50");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.ColleagueSkinPackageA, "colleague_skin_package_a_tier30");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.EssentialPackage, "essential_package_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.GoldRoulettePackage, "gold_roulette_package_tier2");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.ColleagueSkinPackageB, "colleague_skin_package_b_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.TowerModeFreeTicketPackage, "tower_mode_free_ticket_1_hour_tier1");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.MasterSkinPackage, "master_skin_package_tier_100");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.WeaponSkinPremiumLottery, "weapon_skin_premium_lottery_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.WeaponLegendarySkinPackage, "weapon_legendary_skin_package_tier50");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.RandomCharacterSkinPackageA, "random_character_skin_package_a_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.GoldenPackageA, "golden_package_a_tier30");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.GoldenPackageB, "golden_package_b_tier30");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.GoldenPackageC, "golden_package_c_tier30");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.FlowerPackage, "flower_package_tier4");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.StarterPack, "new_starter_package_tier5");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.TranscendPackage, "transcend_package_tier20");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.HonorPackage, "honor_package_tier20");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.DemonPackage, "demon_package_tier20");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.RandomHeroPackage, "random_hero_package_tier3");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.RandomLimitedHeroPackage, "random_limited_hero_package_tier25");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.AFKPackage, "afk_package_tier1");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.PVPCharacterPackage, "pvp_character_package_tier5");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.SuperAFKPackage, "super_afk_package_tier5");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.UltraAFKPackage, "ultra_afk_package_tier25");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.HotSummerPackage, "hot_summer_package_tier5");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.DemonCosplayPackage, "demon_cosplay_package_tier5");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.HalloweenPackage, "halloween_package_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.XmasPackage, "xmas_2017_package_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.March2018_FlowerPackage, "flower_201803_package_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.YummyWeaponSkinPackage, "yummy_weapon_package_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.WeddingSkinPackage, "wedding_package_tier10");
			m_limitedProductSKUDataDictionary.Add(ShopManager.LimitedShopItemType.ColleagueSkinPackageC, "colleague_skin_package_c_tier10");
			m_elopeModeProductSKUDataDictionary.Add(ShopManager.ElopeModeItemType.SuperHandsomeGuy, "super_handsome_guy_tier2");
			m_elopeModeProductSKUDataDictionary.Add(ShopManager.ElopeModeItemType.SuperSpeedGuy, "super_speed_guy_tier3");
			m_elopeModeProductSKUDataDictionary.Add(ShopManager.ElopeModeItemType.ElopeResources, "elope_resources_tier4");
			GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
			GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
			GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
			GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
			GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
			GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
			GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;
			GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
			GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
			if (!_isInited)
			{
				string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsdmu4xvSgwaSOA1R9plkAEHPuQ1mQzJ48TE2mTLnp1GWebirXHP5qWPsG+l1gYH+cbQ2K5qHGk9NcL12usGaaGWkVEGc49qqv7Zq5HoXBMNRdom6484Eib3l1xyh0GKgVcmovkHfM1mY0qeVgoANFeixJS/QhZpQlNlud2p4wVd93HVV36FLc2HJ3v33xrDkddCEWXRPJgg0ZnMKo/Qn2H2cAdYXxVSclfqV8fsfkEz9XiVaWer6QqdcWAI5vnUNFiTePg7ZbjSkJKBiYHUT1qzfmMVefGYPNlIoN8viku8L6bU2nzcFIFGKhiFemWFAB6RvmJDvROLE7e5CECqIEQIDAQAB";
				GoogleIAB.init(publicKey);
				_isInited = true;
			}
			m_skuDetailList = new List<GoogleSkuInfo>();
			_listnersAdded = true;
		}
	}

	private ShopManager.DefaultRubyShopItemType getDefaultProductTypeBySKUID(string sku)
	{
		ShopManager.DefaultRubyShopItemType defaultRubyShopItemType = ShopManager.DefaultRubyShopItemType.None;
		defaultRubyShopItemType = m_nonLimitedProductSKUDataDictionary.FirstOrDefault((KeyValuePair<ShopManager.DefaultRubyShopItemType, string> x) => x.Value == sku).Key;
		if (defaultRubyShopItemType.Equals(ShopManager.DefaultRubyShopItemType.AdsRuby))
		{
			defaultRubyShopItemType = ShopManager.DefaultRubyShopItemType.None;
		}
		return defaultRubyShopItemType;
	}

	private ShopManager.LimitedShopItemType getLimitedProductTypeBySKUID(string sku)
	{
		ShopManager.LimitedShopItemType limitedShopItemType = ShopManager.LimitedShopItemType.None;
		limitedShopItemType = m_limitedProductSKUDataDictionary.FirstOrDefault((KeyValuePair<ShopManager.LimitedShopItemType, string> x) => x.Value == sku).Key;
		if (limitedShopItemType.Equals(ShopManager.LimitedShopItemType.RecievedItemFromServer))
		{
			limitedShopItemType = ShopManager.LimitedShopItemType.None;
		}
		return limitedShopItemType;
	}

	private ShopManager.ElopeModeItemType getElopeModeProductTypeBySKUID(string sku)
	{
		ShopManager.ElopeModeItemType elopeModeItemType = ShopManager.ElopeModeItemType.None;
		elopeModeItemType = m_elopeModeProductSKUDataDictionary.FirstOrDefault((KeyValuePair<ShopManager.ElopeModeItemType, string> x) => x.Value == sku).Key;
		if (elopeModeItemType.Equals(ShopManager.ElopeModeItemType.SuperHandsomeGuy))
		{
			elopeModeItemType = ShopManager.ElopeModeItemType.None;
		}
		return elopeModeItemType;
	}

	private void OnDestroy()
	{
		GoogleIABManager.billingSupportedEvent -= billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent -= purchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}

	public void Purchase(ShopManager.DefaultRubyShopItemType productType, Action successPurchaseEvent)
	{
		if (!isInited)
		{
			AndroidMessage.Create(I18NManager.Get("PURCHASE_INIT_ERROR"), I18NManager.Get("PURCHASE_INIT_ERROR_CONTENT"));
			return;
		}
		m_currentPurchasingNonLimitedItemType = productType;
		m_currentPurchasingLimitedType = ShopManager.LimitedShopItemType.None;
		m_currentPurchasingElopeModeProductType = ShopManager.ElopeModeItemType.None;
		m_purchasePurchaseSuccessEvent = successPurchaseEvent;
		GoogleIAB.purchaseProduct(m_nonLimitedProductSKUDataDictionary[productType]);
	}

	public void Purchase(ShopManager.LimitedShopItemType productType, Action successPurchaseEvent)
	{
		if (!isInited)
		{
			AndroidMessage.Create(I18NManager.Get("PURCHASE_INIT_ERROR"), I18NManager.Get("PURCHASE_INIT_ERROR_CONTENT"));
			return;
		}
		m_currentPurchasingNonLimitedItemType = ShopManager.DefaultRubyShopItemType.None;
		m_currentPurchasingLimitedType = productType;
		m_currentPurchasingElopeModeProductType = ShopManager.ElopeModeItemType.None;
		m_purchasePurchaseSuccessEvent = successPurchaseEvent;
		GoogleIAB.purchaseProduct(m_limitedProductSKUDataDictionary[productType]);
	}

	public void Purchase(ShopManager.ElopeModeItemType productType, Action successPurchaseEvent)
	{
		if (!isInited)
		{
			AndroidMessage.Create(I18NManager.Get("PURCHASE_INIT_ERROR"), I18NManager.Get("PURCHASE_INIT_ERROR_CONTENT"));
			return;
		}
		m_currentPurchasingNonLimitedItemType = ShopManager.DefaultRubyShopItemType.None;
		m_currentPurchasingLimitedType = ShopManager.LimitedShopItemType.None;
		m_currentPurchasingElopeModeProductType = productType;
		m_purchasePurchaseSuccessEvent = successPurchaseEvent;
		GoogleIAB.purchaseProduct(m_elopeModeProductSKUDataDictionary[productType]);
	}

	public void Consume(string SKU)
	{
		if (!isInited)
		{
			AndroidMessage.Create(I18NManager.Get("PURCHASE_INIT_ERROR"), I18NManager.Get("PURCHASE_INIT_ERROR_CONTENT"));
		}
		else
		{
			GoogleIAB.consumeProduct(SKU);
		}
	}

	public void Consume(string[] skuList)
	{
		if (!isInited)
		{
			AndroidMessage.Create(I18NManager.Get("PURCHASE_INIT_ERROR"), I18NManager.Get("PURCHASE_INIT_ERROR_CONTENT"));
		}
		else
		{
			GoogleIAB.consumeProducts(skuList);
		}
	}

	private void billingSupportedEvent()
	{
		DebugManager.Log("billingSupportedEvent");
		List<string> list = new List<string>();
		foreach (KeyValuePair<ShopManager.DefaultRubyShopItemType, string> item in m_nonLimitedProductSKUDataDictionary)
		{
			list.Add(item.Value);
		}
		foreach (KeyValuePair<ShopManager.LimitedShopItemType, string> item2 in m_limitedProductSKUDataDictionary)
		{
			list.Add(item2.Value);
		}
		foreach (KeyValuePair<ShopManager.ElopeModeItemType, string> item3 in m_elopeModeProductSKUDataDictionary)
		{
			list.Add(item3.Value);
		}
		if (Util.isInternetConnection() && list.Count > 0)
		{
			GoogleIAB.queryInventory(list.ToArray());
		}
	}

	private void billingNotSupportedEvent(string error)
	{
		DebugManager.Log("billingNotSupportedEvent: " + error);
	}

	private void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		DebugManager.Log(string.Format("queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count));
		Utils.logObject(purchases);
		Utils.logObject(skus);
		if (m_skuDetailList != null)
		{
			m_skuDetailList.Clear();
			foreach (GoogleSkuInfo sku in skus)
			{
				m_skuDetailList.Add(sku);
			}
		}
		bool flag = false;
		List<string> list = new List<string>();
		for (int i = 0; i < purchases.Count; i++)
		{
			list.Add(purchases[i].productId);
		}
		if (list.Count > 0)
		{
			Consume(list.ToArray());
		}
		if ((bool)UIWindowSetting.instance && UIWindowSetting.instance.isOpen && UnbiasedTime.Instance.Now().Subtract(UIWindowSetting.lastTimeRestore).TotalSeconds < 5.0)
		{
			if (flag)
			{
				UIWindowDialog.openDescription("DIALOG_SUCCESS_RESTORE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
			else
			{
				UIWindowDialog.openDescription("DIALOG_FAIL_RESTORE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		}
	}

	private void queryInventoryFailedEvent(string error)
	{
		DebugManager.Log("queryInventoryFailedEvent: " + error);
		UIWindowDialog.openDescription("PURCHASE_INIT_ERROR_CONTENT", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
	}

	private void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
	{
		DebugManager.Log("purchaseCompleteAwaitingVerificationEvent. purchaseData: " + purchaseData + ", signature: " + signature);
		m_purchaseData = purchaseData;
		m_signature = signature;
	}

	private void purchaseSucceededEvent(GooglePurchase purchase)
	{
		Singleton<LogglyManager>.instance.SendLoggly("Purchase complete and Call validation (Google) : " + purchase.orderId + ", SKU:" + purchase.productId, "OnProductPurchased()", LogType.Log);
		UIWindowLoading.instance.openLoadingUI(null, false);
		string currency = string.Empty;
		double result = 0.0;
		bool flag = false;
		foreach (GoogleSkuInfo skuDetail in m_skuDetailList)
		{
			if (skuDetail.productId == purchase.productId)
			{
				currency = skuDetail.priceCurrencyCode;
				string text = skuDetail.price;
				string text2 = Regex.Replace(text, "[\\d-]", ".");
				text2 = text2.Replace(".", string.Empty);
				for (int i = 0; i < text2.Length; i++)
				{
					text = text.Replace(text2.Substring(i, 1), string.Empty);
				}
				flag = double.TryParse(text, out result);
			}
		}
		Singleton<NanooAPIManager>.instance.ReceiptVerificationAndroid(purchase.productId, purchase.originalJson, purchase.signature, currency, result);
	}

	private void purchaseFailedEvent(string error, int response)
	{
		Singleton<LogglyManager>.instance.SendLoggly("OnPuchased Fail - " + error, "purchaseFailedEvent()", LogType.Error);
		switch (response)
		{
		case 7:
			UIWindowDialog.openDescription("DIALOG_ALREADY_OWNED_ITEM", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			break;
		case -1005:
		case 1:
			break;
		default:
		{
			string message = string.Format(I18NManager.Get("PURCHASE_TRANSACTION_FAIL_CONTENT"), response) + ", " + error;
			AndroidMessage.Create(I18NManager.Get("PURCHASE_TRANSACTION_FAIL"), message);
			break;
		}
		}
	}

	private void consumePurchaseSucceededEvent(GooglePurchase purchase)
	{
		DebugManager.Log("consumePurchaseSucceededEvent: " + purchase);
		ProcessingConsume(purchase.productId);
	}

	private void consumePurchaseFailedEvent(string error)
	{
		DebugManager.LogError("consumePurchaseFailedEvent: " + error);
		Singleton<LogglyManager>.instance.SendLoggly("OnProductConsume Fail - " + error, "consumePurchaseFailedEvent()", LogType.Error);
		AndroidMessage.Create("Product Cousume Failed", error);
	}

	public void ProcessingPurchase(string product_id)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (m_currentPurchasingNonLimitedItemType != ShopManager.DefaultRubyShopItemType.None)
		{
			flag2 = true;
			if (Singleton<ShopManager>.instance.isConsumableProduct(m_currentPurchasingNonLimitedItemType))
			{
				Consume(m_nonLimitedProductSKUDataDictionary[m_currentPurchasingNonLimitedItemType]);
				flag = true;
			}
		}
		else if (m_currentPurchasingLimitedType != ShopManager.LimitedShopItemType.None)
		{
			flag3 = true;
			if (Singleton<ShopManager>.instance.isConsumableProduct(m_currentPurchasingLimitedType))
			{
				Consume(m_limitedProductSKUDataDictionary[m_currentPurchasingLimitedType]);
				flag = true;
			}
		}
		else if (m_currentPurchasingElopeModeProductType != ShopManager.ElopeModeItemType.None)
		{
			flag4 = true;
			Consume(m_elopeModeProductSKUDataDictionary[m_currentPurchasingElopeModeProductType]);
			flag = true;
		}
		if (flag)
		{
			return;
		}
		if (flag2)
		{
			if (m_nonLimitedProductSKUDataDictionary[m_currentPurchasingNonLimitedItemType].Equals(product_id))
			{
				if (m_purchasePurchaseSuccessEvent != null)
				{
					m_purchasePurchaseSuccessEvent();
				}
				m_purchasePurchaseSuccessEvent = null;
			}
		}
		else if (flag3)
		{
			if (m_limitedProductSKUDataDictionary[m_currentPurchasingLimitedType].Equals(product_id))
			{
				if (m_purchasePurchaseSuccessEvent != null)
				{
					m_purchasePurchaseSuccessEvent();
				}
				m_purchasePurchaseSuccessEvent = null;
			}
		}
		else if (flag4 && m_elopeModeProductSKUDataDictionary[m_currentPurchasingElopeModeProductType].Equals(product_id))
		{
			if (m_purchasePurchaseSuccessEvent != null)
			{
				m_purchasePurchaseSuccessEvent();
			}
			m_purchasePurchaseSuccessEvent = null;
		}

		// Đảm bảo đóng loading screen sau khi xử lý xong
		if (UIWindowLoading.instance != null)
		{
			UIWindowLoading.instance.closeLoadingUI();
		}
	}

	public void ProcessingConsume(string productId)
	{
		if (m_currentPurchasingNonLimitedItemType != ShopManager.DefaultRubyShopItemType.None)
		{
			if (m_nonLimitedProductSKUDataDictionary[m_currentPurchasingNonLimitedItemType].Equals(productId))
			{
				if (m_purchasePurchaseSuccessEvent != null)
				{
					m_purchasePurchaseSuccessEvent();
				}
				m_purchasePurchaseSuccessEvent = null;
			}
		}
		else if (m_currentPurchasingLimitedType != ShopManager.LimitedShopItemType.None)
		{
			if (m_limitedProductSKUDataDictionary[m_currentPurchasingLimitedType].Equals(productId))
			{
				if (m_purchasePurchaseSuccessEvent != null)
				{
					m_purchasePurchaseSuccessEvent();
				}
				m_purchasePurchaseSuccessEvent = null;
			}
		}
		else if (m_currentPurchasingElopeModeProductType != ShopManager.ElopeModeItemType.None)
		{
			if (m_elopeModeProductSKUDataDictionary[m_currentPurchasingElopeModeProductType].Equals(productId))
			{
				if (m_purchasePurchaseSuccessEvent != null)
				{
					m_purchasePurchaseSuccessEvent();
				}
				m_purchasePurchaseSuccessEvent = null;
			}
		}
		else
		{
			Singleton<LogglyManager>.instance.SendLoggly("Can't find suitable product type:" + productId, "ProcessingConsume()", LogType.Error);
		}
		Singleton<DataManager>.instance.saveData();
		foreach (GoogleSkuInfo skuDetail in m_skuDetailList)
		{
			if (skuDetail.productId.CompareTo(productId) == 0)
			{
				GoogleSkuInfoForJson googleSkuInfoForJson = new GoogleSkuInfoForJson();
				googleSkuInfoForJson.productId = skuDetail.productId;
				googleSkuInfoForJson.type = skuDetail.type;
				googleSkuInfoForJson.price = skuDetail.price;
				googleSkuInfoForJson.price_amount_micros = skuDetail.priceAmountMicros;
				googleSkuInfoForJson.price_currency_code = skuDetail.priceCurrencyCode;
				googleSkuInfoForJson.title = skuDetail.title;
				googleSkuInfoForJson.description = skuDetail.description;
				GoogleSkuInfoForJson obj = googleSkuInfoForJson;
				string skuDetails = JsonUtility.ToJson(obj);
				Tapjoy.TrackPurchaseInGooglePlayStore(skuDetails, m_purchaseData, m_signature);
				break;
			}
		}
	}

	public void Restore()
	{
	}

	public void LogAnswer(bool success, string product_id)
	{
		ShopManager.DefaultRubyShopItemType defaultRubyShopItemType = ShopManager.DefaultRubyShopItemType.None;
		ShopManager.LimitedShopItemType limitedShopItemType = ShopManager.LimitedShopItemType.None;
		ShopManager.ElopeModeItemType elopeModeItemType = ShopManager.ElopeModeItemType.None;
		defaultRubyShopItemType = getDefaultProductTypeBySKUID(product_id);
		limitedShopItemType = getLimitedProductTypeBySKUID(product_id);
		elopeModeItemType = getElopeModeProductTypeBySKUID(product_id);
		int num = 0;
		string itemType = "Unknown";
		string text = string.Empty;
		if (defaultRubyShopItemType != ShopManager.DefaultRubyShopItemType.None)
		{
			num = (int)(double.Parse(Regex.Replace(Singleton<ShopManager>.instance.getRubyProductPriceString(defaultRubyShopItemType), "\\D", string.Empty)) + 1.0) * 10;
			itemType = "Currency";
			text = Singleton<ShopManager>.instance.getProductValue(defaultRubyShopItemType) + " Diamond";
			if (success)
			{
				switch (defaultRubyShopItemType)
				{
				case ShopManager.DefaultRubyShopItemType.RubyTier1:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.BuyDiamondTier1);
					break;
				case ShopManager.DefaultRubyShopItemType.RubyTier2:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.BuyDiamondTier3);
					break;
				case ShopManager.DefaultRubyShopItemType.RubyTier3:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.BuyDiamondTier5);
					break;
				case ShopManager.DefaultRubyShopItemType.RubyTier4:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.BuyDiamondTier10);
					break;
				case ShopManager.DefaultRubyShopItemType.RubyTier5:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.BuyDiamondTier20);
					break;
				}
			}
		}
		else if (limitedShopItemType != ShopManager.LimitedShopItemType.None)
		{
			if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(limitedShopItemType))
			{
				num = (int)(double.Parse(Regex.Replace(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[limitedShopItemType].itemPriceString, "\\D", string.Empty)) + 1.0) * 10;
			}
			else if (limitedShopItemType == ShopManager.LimitedShopItemType.NewTranscendEventPackage)
			{
				num = 30000;
			}
			itemType = "Limited Product";
			text = I18NManager.Get("SHOP_LIMITED_TITLE_TEXT_" + (int)limitedShopItemType, I18NManager.Language.English);
			if (success)
			{
				switch (limitedShopItemType)
				{
				case ShopManager.LimitedShopItemType.PremiumPack:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.PurchasePremiumPackage);
					break;
				case ShopManager.LimitedShopItemType.StarterPack:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.StarterPack);
					break;
				case ShopManager.LimitedShopItemType.OnePlusOneDiamonds:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.PurchaseOneplusOnePackage);
					break;
				case ShopManager.LimitedShopItemType.AllCharacterPack:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.PurchaseSkin1);
					break;
				case ShopManager.LimitedShopItemType.SilverFinger:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.AutoTouch);
					break;
				case ShopManager.LimitedShopItemType.EventPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.PurchaseMonthlyDiamond);
					break;
				case ShopManager.LimitedShopItemType.BadPriemiumPack:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.PurchasePremiumPackage);
					break;
				case ShopManager.LimitedShopItemType.FirstPurchasePackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.PurchaseFirstPurchasePackage);
					break;
				case ShopManager.LimitedShopItemType.LimitedSkinPremiumPack:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.PurchasePremiumSkinPackage);
					break;
				case ShopManager.LimitedShopItemType.VIPPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.PurchaseVIPPackage);
					break;
				case ShopManager.LimitedShopItemType.GoldFinger500:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.GoldFinger500);
					break;
				case ShopManager.LimitedShopItemType.NewTranscendEventPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.NewEventPackage);
					break;
				case ShopManager.LimitedShopItemType.ValkyrieSkinPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.ValkyriePackage);
					break;
				case ShopManager.LimitedShopItemType.MiniVIPPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.PurchaseMiniVIPPackage);
					break;
				case ShopManager.LimitedShopItemType.AngelicaPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.AngelicaPackage);
					break;
				case ShopManager.LimitedShopItemType.IceMeteorSkillPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.IceMeteorSkillPackage);
					break;
				case ShopManager.LimitedShopItemType.AngelinaPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.AngelinaPackage);
					break;
				case ShopManager.LimitedShopItemType.ReinforcementSkillPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.ReinforcementSkillPackage);
					break;
				case ShopManager.LimitedShopItemType.BrotherhoodPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.BrotherHoodPackage);
					break;
				case ShopManager.LimitedShopItemType.DemonKingSkinPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.DemonKingSkinPackage);
					break;
				case ShopManager.LimitedShopItemType.AngelaPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.AngelaPackage);
					break;
				case ShopManager.LimitedShopItemType.GoblinPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.GoblinPackage);
					break;
				case ShopManager.LimitedShopItemType.ZeusPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.ZeusPackage);
					break;
				case ShopManager.LimitedShopItemType.ColleagueSkinPackageA:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.ColleagueSkinPackageA);
					break;
				case ShopManager.LimitedShopItemType.EssentialPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.EssentialPackage);
					break;
				case ShopManager.LimitedShopItemType.GoldRoulettePackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.GoldRoulettePackage);
					break;
				case ShopManager.LimitedShopItemType.ColleagueSkinPackageB:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.ColleagueSkinPackageB);
					break;
				case ShopManager.LimitedShopItemType.TowerModeFreeTicketPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.TowerModeFreeTicketPackage);
					break;
				case ShopManager.LimitedShopItemType.MasterSkinPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.MasterSkinPackage);
					break;
				case ShopManager.LimitedShopItemType.WeaponSkinPremiumLottery:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.WeaponSkinPremiumLottery);
					break;
				case ShopManager.LimitedShopItemType.WeaponLegendarySkinPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.WeaponLegendarySkinPackage);
					break;
				case ShopManager.LimitedShopItemType.RandomCharacterSkinPackageA:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.RandomCharacterSkinPackageA);
					break;
				case ShopManager.LimitedShopItemType.GoldenPackageA:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.GoldenPackageA);
					break;
				case ShopManager.LimitedShopItemType.GoldenPackageB:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.GoldenPackageB);
					break;
				case ShopManager.LimitedShopItemType.GoldenPackageC:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.GoldenPackageC);
					break;
				case ShopManager.LimitedShopItemType.FlowerPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.FlowerPackage);
					break;
				case ShopManager.LimitedShopItemType.HotSummerPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.HotSummerPackage);
					break;
				case ShopManager.LimitedShopItemType.DemonCosplayPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.DemonCosplayPackage);
					break;
				case ShopManager.LimitedShopItemType.HalloweenPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.HalloweenPackage);
					break;
				case ShopManager.LimitedShopItemType.XmasPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.XmasPackage);
					break;
				case ShopManager.LimitedShopItemType.March2018_FlowerPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.March2018_FlowerPackage);
					break;
				case ShopManager.LimitedShopItemType.YummyWeaponSkinPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.YummyWeaponSkinPackage);
					break;
				case ShopManager.LimitedShopItemType.WeddingSkinPackage:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.WeddingSkinPackage);
					break;
				case ShopManager.LimitedShopItemType.ColleagueSkinPackageC:
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.ColleagueSkinPackageC);
					break;
				}
			}
		}
		else if (elopeModeItemType != ShopManager.ElopeModeItemType.None)
		{
			itemType = "Elope Product";
			if (success)
			{
				switch (elopeModeItemType)
				{
				case ShopManager.ElopeModeItemType.SuperHandsomeGuy:
					text = I18NManager.Get("ELOPE_DAEMON_KING_SLOT_TITLE_5");
					num = 2000;
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.ElopeSuperHandsomeGuy);
					break;
				case ShopManager.ElopeModeItemType.SuperSpeedGuy:
					text = I18NManager.Get("ELOPE_DAEMON_KING_SLOT_TITLE_7");
					num = 3000;
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.ElopeSuperSpeedGuy);
					break;
				case ShopManager.ElopeModeItemType.ElopeResources:
					text = I18NManager.Get("ELOPE_RESOURCES");
					num = 4000;
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Purchase, AnalyzeManager.ActionType.ElopeResourcesGuy);
					break;
				}
			}
		}
		if (!string.IsNullOrEmpty(text) && num > 0)
		{
			GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
			currentGameData.totalPurchasedMoney = (long)currentGameData.totalPurchasedMoney + num;
			Answers.LogPurchase(num, "KRW", success, text, itemType, product_id);
			Singleton<DataManager>.instance.saveData();
		}
	}

	public string GetMarketPrice(ShopManager.DefaultRubyShopItemType itemType)
	{
		string result = string.Empty;
		if (m_nonLimitedProductSKUDataDictionary != null && m_nonLimitedProductSKUDataDictionary.ContainsKey(itemType))
		{
			string productId = m_nonLimitedProductSKUDataDictionary[itemType];
			if (m_skuDetailList != null && m_skuDetailList.Count > 0 && productId.Length > 0 && m_skuDetailList.Exists((GoogleSkuInfo x) => x.productId == productId))
			{
				GoogleSkuInfo googleSkuInfo = m_skuDetailList.Find((GoogleSkuInfo x) => x.productId == productId);
				if (googleSkuInfo != null)
				{
					result = googleSkuInfo.price;
				}
			}
		}
		return result;
	}

	public string GetMarketPrice(ShopManager.LimitedShopItemType itemType)
	{
		if (!m_limitedProductSKUDataDictionary.ContainsKey(itemType))
		{
			DebugManager.LogError("DictionaryKeyNotFoundError");
			return string.Empty;
		}
		string result = string.Empty;
		string productId = m_limitedProductSKUDataDictionary[itemType];
		if (m_skuDetailList != null && m_skuDetailList.Count > 0 && productId.Length > 0)
		{
			result = ((!m_skuDetailList.Exists((GoogleSkuInfo x) => x.productId == productId)) ? string.Empty : m_skuDetailList.Find((GoogleSkuInfo x) => x.productId == productId).price);
		}
		return result;
	}

	public string GetMarketPrice(ShopManager.ElopeModeItemType itemType)
	{
		string result = string.Empty;
		string productId = m_elopeModeProductSKUDataDictionary[itemType];
		if (m_skuDetailList != null && m_skuDetailList.Count > 0 && productId.Length > 0)
		{
			result = ((!m_skuDetailList.Exists((GoogleSkuInfo x) => x.productId == productId)) ? string.Empty : m_skuDetailList.Find((GoogleSkuInfo x) => x.productId == productId).price);
		}
		return result;
	}
}
