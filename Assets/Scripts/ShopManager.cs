using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
	public enum ShopSelectedType
	{
		None = -1,
		PremiumItem,
		SkinItem,
		NormalItem,
		Ruby
	}

	public enum LimitedShopItemType
	{
		None = -1,
		RecievedItemFromServer,
		PremiumPack,
		StarterPack,
		OnePlusOneDiamonds,
		AllCharacterPack,
		SilverFinger,
		EventPackage,
		BadPriemiumPack,
		FirstPurchasePackage,
		LimitedSkinPremiumPack,
		VIPPackage,
		GoldFinger500,
		NewTranscendEventPackage,
		ValkyrieSkinPackage,
		MiniVIPPackage,
		AngelicaPackage,
		IceMeteorSkillPackage,
		AngelinaPackage,
		ReinforcementSkillPackage,
		BrotherhoodPackage,
		DemonKingSkinPackage,
		AngelaPackage,
		GoblinPackage,
		ZeusPackage,
		ColleagueSkinPackageA,
		EssentialPackage,
		GoldRoulettePackage,
		ColleagueSkinPackageB,
		TowerModeFreeTicketPackage,
		MasterSkinPackage,
		WeaponSkinPremiumLottery,
		WeaponLegendarySkinPackage,
		RandomCharacterSkinPackageA,
		GoldenPackageA,
		GoldenPackageB,
		GoldenPackageC,
		FlowerPackage,
		TranscendPackage,
		HonorPackage,
		DemonPackage,
		RandomHeroPackage,
		RandomLimitedHeroPackage,
		AFKPackage,
		PVPCharacterPackage,
		SuperAFKPackage,
		UltraAFKPackage,
		HotSummerPackage,
		DemonCosplayPackage,
		HalloweenPackage,
		XmasPackage,
		March2018_FlowerPackage,
		YummyWeaponSkinPackage,
		WeddingSkinPackage,
		ColleagueSkinPackageC,
		Length
	}

	[Serializable]
	public class LimitedItemData
	{
		public LimitedShopItemType limitedType;

		public string limitedItemID;

		private string priceString;

		public List<LimitedItemSetData> limitedAttribudeList;

		public long targetLimitedItemEndTime;

		public int currentItemCount;

		public bool isDisabledItem;

		public string itemPriceString
		{
			get
			{
				return priceString;
			}
		}

		public LimitedItemData(LimitedShopItemType limitedType, string priceString, List<LimitedItemSetData> limitedAttribudeList, long targetLimitedItemEndTime, int currentItemCount, bool isDisabledItem)
		{
			this.limitedType = limitedType;
			limitedItemID = limitedType.ToString();
			this.priceString = priceString;
			this.limitedAttribudeList = limitedAttribudeList;
			this.targetLimitedItemEndTime = targetLimitedItemEndTime;
			this.currentItemCount = currentItemCount;
			this.isDisabledItem = isDisabledItem;
		}

		public LimitedItemData(LimitedShopItemType limitedType, string priceString, List<LimitedItemSetData> limitedAttribudeList, int currentItemCount)
		{
			this.limitedType = limitedType;
			limitedItemID = limitedType.ToString();
			this.priceString = priceString;
			this.limitedAttribudeList = limitedAttribudeList;
			targetLimitedItemEndTime = UnbiasedTime.Instance.Now().AddYears(100).Ticks;
			this.currentItemCount = currentItemCount;
			isDisabledItem = false;
		}

		public void setItemString(string price)
		{
			priceString = price;
		}
	}

	public enum ElopeModeItemType
	{
		None = -1,
		SuperHandsomeGuy,
		SuperSpeedGuy,
		ElopeResources
	}

	public enum DefaultRubyShopItemType
	{
		None = -1,
		AdsRuby,
		RubyTier1,
		RubyTier2,
		RubyTier3,
		RubyTier4,
		RubyTier5,
		RubyTier6,
		Tapjoy,
		Length
	}

	public enum DefaultGoldShopItemType
	{
		None = -1,
		AdsGold,
		GoldTier1,
		GoldTier2,
		GoldTier3,
		Length
	}

	public enum PurchaseType
	{
		None = -1,
		TapJoy,
		Ads,
		BuyRuby,
		BuyCash
	}

	[Serializable]
	public struct LimitedItemSetData
	{
		public LimitedAttributeType limitedItemType;

		public long value;

		public long secondValue;

		public LimitedItemSetData(LimitedAttributeType limitedItemType, long value)
		{
			this.limitedItemType = limitedItemType;
			this.value = value;
			secondValue = 0L;
		}

		public LimitedItemSetData(LimitedAttributeType limitedItemType, long value, long secondValue)
		{
			this.limitedItemType = limitedItemType;
			this.value = value;
			this.secondValue = secondValue;
		}
	}

	public enum LimitedAttributeType
	{
		None = -1,
		TimerSilverFinger,
		PermanentAutoTap,
		Ruby,
		Gold,
		AllCharacter,
		CountableSilverFinger,
		CountableGoldFinger,
		WarriorSkin,
		PriestSkin,
		ArcherSkin,
		TranscendStone,
		IcePassiveSkill,
		MeteorPassiveSkill,
		Colleague,
		ReinforcementSkill,
		HeartCoin,
		SuperSpeedGuy,
		ElopeResources,
		ColleagueSkin,
		GoldRouletteTicekt,
		TowerModeFreeTicket,
		TimerGoldFinger,
		WeaponSkinMasterPiece,
		RandomLegendaryWeaponSkin,
		RandomCharacterSkin,
		SwordSoulSkill,
		WarriorSpecialWeaponSkin,
		PriestSpecialWeaponSkin,
		ArcherSpecialWeaponSkin,
		BronzeFinger,
		HonorToken,
		LotteryRandomAllCharacterSkin,
		LotteryRandomLimitedCharacterSkin,
		CountDoubleSpeed,
		CountAutoOpenChest,
		TimerDoubleSpeed,
		TimerAutoOpenChest
	}

	[Serializable]
	public class DefaultRubyItemIconSpriteData
	{
		public DefaultRubyShopItemType itemType;

		public Sprite iconSprite;
	}

	[Serializable]
	public class DefaultGoldItemIconSpriteData
	{
		public DefaultGoldShopItemType itemType;

		public Sprite iconSprite;
	}

	[Serializable]
	public class LimitedItemIconSpriteData
	{
		public LimitedShopItemType itemType;

		public Sprite iconSprite;
	}

	public List<DefaultRubyItemIconSpriteData> defaultRubyItemIconSpriteDataList;

	public List<DefaultGoldItemIconSpriteData> defaultGoldItemIconSpriteDataList;

	[Header("Deprecated")]
	public List<LimitedItemIconSpriteData> limitedItemIconSpriteDataList;

	public List<LimitedItemIconSpriteData> newLimitedItemIconSpriteDataList;

	private Dictionary<DefaultRubyShopItemType, long> m_defaultProductRubyValueDictionary = new Dictionary<DefaultRubyShopItemType, long>();

	private Dictionary<DefaultGoldShopItemType, long> m_defaultProductGoldValueDictionary = new Dictionary<DefaultGoldShopItemType, long>();

	public List<LimitedItemData> premiumItemList = new List<LimitedItemData>();

	public List<LimitedItemData> skinItemList = new List<LimitedItemData>();

	public List<LimitedItemData> normalItemList = new List<LimitedItemData>();

	public List<LimitedItemData> rubyItemList = new List<LimitedItemData>();

	[NonSerialized]
	public Dictionary<LimitedShopItemType, LimitedItemData> totalLimitedItemDataDictionary;

	public static bool isPaidUser
	{
		get
		{
			if (Singleton<DataManager>.instance.currentGameData != null)
			{
				return (long)Singleton<DataManager>.instance.currentGameData.totalPurchasedMoney > 0;
			}
			return false;
		}
	}

	public void Initialize()
	{
		initDefaultItems();
		initLimitedItems();
	}

	private void initDefaultItems()
	{
		m_defaultProductRubyValueDictionary.Add(DefaultRubyShopItemType.Tapjoy, 0L);
		m_defaultProductRubyValueDictionary.Add(DefaultRubyShopItemType.AdsRuby, 10L);
		m_defaultProductRubyValueDictionary.Add(DefaultRubyShopItemType.RubyTier1, 300L);
		m_defaultProductRubyValueDictionary.Add(DefaultRubyShopItemType.RubyTier2, 550L);
		m_defaultProductRubyValueDictionary.Add(DefaultRubyShopItemType.RubyTier3, 1200L);
		m_defaultProductRubyValueDictionary.Add(DefaultRubyShopItemType.RubyTier4, 2600L);
		m_defaultProductRubyValueDictionary.Add(DefaultRubyShopItemType.RubyTier5, 7000L);
		m_defaultProductRubyValueDictionary.Add(DefaultRubyShopItemType.RubyTier6, 16000L);
		m_defaultProductGoldValueDictionary.Add(DefaultGoldShopItemType.AdsGold, 1L);
		m_defaultProductGoldValueDictionary.Add(DefaultGoldShopItemType.GoldTier1, 20L);
		m_defaultProductGoldValueDictionary.Add(DefaultGoldShopItemType.GoldTier2, 200L);
		m_defaultProductGoldValueDictionary.Add(DefaultGoldShopItemType.GoldTier3, 2000L);
	}

	private void initLimitedItems()
	{
		totalLimitedItemDataDictionary = new Dictionary<LimitedShopItemType, LimitedItemData>();
		if (Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList == null)
		{
			Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList = new List<LimitedItemData>();
		}
		if (Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList == null)
		{
			Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList = new List<LimitedItemData>();
		}
		int count = Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Count;
		List<LimitedItemSetData> list = new List<LimitedItemSetData>();
		list.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 100L));
		list.Add(new LimitedItemSetData(LimitedAttributeType.CountableGoldFinger, 300L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.FirstPurchasePackage, new LimitedItemData(LimitedShopItemType.FirstPurchasePackage, "$0.50", list, 1));
		List<LimitedItemSetData> list2 = new List<LimitedItemSetData>();
		list2.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 500L));
		list2.Add(new LimitedItemSetData(LimitedAttributeType.BronzeFinger, 1L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.StarterPack, new LimitedItemData(LimitedShopItemType.StarterPack, "$1.00", list2, 1));
		List<LimitedItemSetData> list3 = new List<LimitedItemSetData>();
		list3.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 1000L));
		list3.Add(new LimitedItemSetData(LimitedAttributeType.TranscendStone, 300L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.TranscendPackage, new LimitedItemData(LimitedShopItemType.TranscendPackage, "$3.00", list3, 10000));
		List<LimitedItemSetData> list4 = new List<LimitedItemSetData>();
		list4.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 1000L));
		list4.Add(new LimitedItemSetData(LimitedAttributeType.HonorToken, 1000L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.HonorPackage, new LimitedItemData(LimitedShopItemType.HonorPackage, "$5.00", list4, 10000));
		List<LimitedItemSetData> list5 = new List<LimitedItemSetData>();
		list5.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 1000L));
		list5.Add(new LimitedItemSetData(LimitedAttributeType.HeartCoin, 1000L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.DemonPackage, new LimitedItemData(LimitedShopItemType.DemonPackage, "$10.00", list5, 10000));
		List<LimitedItemSetData> list6 = new List<LimitedItemSetData>();
		list6.Add(new LimitedItemSetData(LimitedAttributeType.ColleagueSkin, 9L, 4L));
		list6.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 1000L));
		list6.Add(new LimitedItemSetData(LimitedAttributeType.HeartCoin, 500L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.ColleagueSkinPackageC, new LimitedItemData(LimitedShopItemType.ColleagueSkinPackageC, "$9.99", list6, new DateTime(2018, 7, 15, 23, 59, 59, 0).Ticks, 1, false));
		List<LimitedItemSetData> list7 = new List<LimitedItemSetData>();
		list7.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSkin, 30L));
		list7.Add(new LimitedItemSetData(LimitedAttributeType.PriestSkin, 30L));
		list7.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSkin, 30L));
		list7.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 500L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.WeddingSkinPackage, new LimitedItemData(LimitedShopItemType.WeddingSkinPackage, "$9.99", list7, new DateTime(2018, 6, 15, 23, 59, 59, 0).Ticks, 1, false));
		List<LimitedItemSetData> list8 = new List<LimitedItemSetData>();
		list8.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSpecialWeaponSkin, 1L));
		list8.Add(new LimitedItemSetData(LimitedAttributeType.PriestSpecialWeaponSkin, 1L));
		list8.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSpecialWeaponSkin, 1L));
		list8.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 500L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.YummyWeaponSkinPackage, new LimitedItemData(LimitedShopItemType.YummyWeaponSkinPackage, "$9.99", list8, new DateTime(2018, 4, 30, 23, 59, 59, 0).Ticks, 1, false));
		List<LimitedItemSetData> list9 = new List<LimitedItemSetData>();
		list9.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSkin, 29L));
		list9.Add(new LimitedItemSetData(LimitedAttributeType.PriestSkin, 29L));
		list9.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSkin, 29L));
		list9.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 500L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.March2018_FlowerPackage, new LimitedItemData(LimitedShopItemType.March2018_FlowerPackage, "$9.99", list9, new DateTime(2018, 3, 31, 23, 59, 59, 0).Ticks, 1, false));
		List<LimitedItemSetData> list10 = new List<LimitedItemSetData>();
		list10.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSkin, 28L));
		list10.Add(new LimitedItemSetData(LimitedAttributeType.PriestSkin, 28L));
		list10.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSkin, 28L));
		list10.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 500L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.XmasPackage, new LimitedItemData(LimitedShopItemType.XmasPackage, "$9.99", list10, new DateTime(2018, 1, 15, 23, 59, 59, 0).Ticks, 1, false));
		List<LimitedItemSetData> list11 = new List<LimitedItemSetData>();
		list11.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSkin, 27L));
		list11.Add(new LimitedItemSetData(LimitedAttributeType.PriestSkin, 27L));
		list11.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSkin, 27L));
		list11.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 500L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.HalloweenPackage, new LimitedItemData(LimitedShopItemType.HalloweenPackage, "$9.99", list11, new DateTime(2017, 11, 30, 23, 59, 59, 0).Ticks, 1, false));
		List<LimitedItemSetData> list12 = new List<LimitedItemSetData>();
		list12.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSkin, 26L));
		list12.Add(new LimitedItemSetData(LimitedAttributeType.PriestSkin, 26L));
		list12.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSkin, 26L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.DemonCosplayPackage, new LimitedItemData(LimitedShopItemType.DemonCosplayPackage, "$4.99", list12, new DateTime(2017, 10, 31, 23, 59, 59, 0).Ticks, 1, false));
		List<LimitedItemSetData> list13 = new List<LimitedItemSetData>();
		list13.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSkin, 25L));
		list13.Add(new LimitedItemSetData(LimitedAttributeType.PriestSkin, 25L));
		list13.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSkin, 25L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.HotSummerPackage, new LimitedItemData(LimitedShopItemType.HotSummerPackage, "$4.99", list13, new DateTime(2017, 8, 30, 23, 59, 59, 0).Ticks, 1, false));
		List<LimitedItemSetData> list14 = new List<LimitedItemSetData>();
		list14.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSkin, 24L));
		list14.Add(new LimitedItemSetData(LimitedAttributeType.PriestSkin, 24L));
		list14.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSkin, 24L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.PVPCharacterPackage, new LimitedItemData(LimitedShopItemType.PVPCharacterPackage, "$4.99", list14, new DateTime(2017, 7, 30, 23, 59, 59, 0).Ticks, 1, false));
		List<LimitedItemSetData> list15 = new List<LimitedItemSetData>();
		list15.Add(new LimitedItemSetData(LimitedAttributeType.LotteryRandomAllCharacterSkin, 1L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.RandomHeroPackage, new LimitedItemData(LimitedShopItemType.RandomHeroPackage, "$1.00", list15, 10000));
		List<LimitedItemSetData> list16 = new List<LimitedItemSetData>();
		list16.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 1000L));
		list16.Add(new LimitedItemSetData(LimitedAttributeType.CountableGoldFinger, 1000L));
		list16.Add(new LimitedItemSetData(LimitedAttributeType.CountDoubleSpeed, 1000L));
		list16.Add(new LimitedItemSetData(LimitedAttributeType.CountAutoOpenChest, 1000L));
		list16.Add(new LimitedItemSetData(LimitedAttributeType.LotteryRandomLimitedCharacterSkin, 3L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.RandomLimitedHeroPackage, new LimitedItemData(LimitedShopItemType.RandomLimitedHeroPackage, "$10.00", list16, 10000));
		List<LimitedItemSetData> list17 = new List<LimitedItemSetData>();
		list17.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 1000L));
		list17.Add(new LimitedItemSetData(LimitedAttributeType.WeaponSkinMasterPiece, 500L));
		list17.Add(new LimitedItemSetData(LimitedAttributeType.RandomLegendaryWeaponSkin, 1L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.WeaponLegendarySkinPackage, new LimitedItemData(LimitedShopItemType.WeaponLegendarySkinPackage, "$15.00", list17, 10000));
		List<LimitedItemSetData> list18 = new List<LimitedItemSetData>();
		list18.Add(new LimitedItemSetData(LimitedAttributeType.TimerSilverFinger, 720L));
		list18.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 10000L));
		list18.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSkin, 9L));
		list18.Add(new LimitedItemSetData(LimitedAttributeType.PriestSkin, 9L));
		list18.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSkin, 9L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.LimitedSkinPremiumPack, new LimitedItemData(LimitedShopItemType.LimitedSkinPremiumPack, "$15.00", list18, 1));
		List<LimitedItemSetData> list19 = new List<LimitedItemSetData>();
		list19.Add(new LimitedItemSetData(LimitedAttributeType.WarriorSkin, 21L));
		list19.Add(new LimitedItemSetData(LimitedAttributeType.PriestSkin, 21L));
		list19.Add(new LimitedItemSetData(LimitedAttributeType.ArcherSkin, 21L));
		list19.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 20000L));
		list19.Add(new LimitedItemSetData(LimitedAttributeType.TimerGoldFinger, 720L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.MasterSkinPackage, new LimitedItemData(LimitedShopItemType.MasterSkinPackage, "$15.00", list19, 1));
		List<LimitedItemSetData> list20 = new List<LimitedItemSetData>();
		list20.Add(new LimitedItemSetData(LimitedAttributeType.CountableGoldFinger, 100L));
		list20.Add(new LimitedItemSetData(LimitedAttributeType.CountDoubleSpeed, 100L));
		list20.Add(new LimitedItemSetData(LimitedAttributeType.CountAutoOpenChest, 100L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.AFKPackage, new LimitedItemData(LimitedShopItemType.AFKPackage, "$0.99", list20, 10000));
		List<LimitedItemSetData> list21 = new List<LimitedItemSetData>();
		list21.Add(new LimitedItemSetData(LimitedAttributeType.TimerGoldFinger, 72L));
		list21.Add(new LimitedItemSetData(LimitedAttributeType.TimerDoubleSpeed, 72L));
		list21.Add(new LimitedItemSetData(LimitedAttributeType.TimerAutoOpenChest, 72L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.SuperAFKPackage, new LimitedItemData(LimitedShopItemType.SuperAFKPackage, "$3.00", list21, 10000));
		List<LimitedItemSetData> list22 = new List<LimitedItemSetData>();
		list22.Add(new LimitedItemSetData(LimitedAttributeType.TimerGoldFinger, 672L));
		list22.Add(new LimitedItemSetData(LimitedAttributeType.TimerDoubleSpeed, 672L));
		list22.Add(new LimitedItemSetData(LimitedAttributeType.TimerAutoOpenChest, 672L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.UltraAFKPackage, new LimitedItemData(LimitedShopItemType.UltraAFKPackage, "$10.00", list22, 10000));
		List<LimitedItemSetData> list23 = new List<LimitedItemSetData>();
		list23.Add(new LimitedItemSetData(LimitedAttributeType.CountableGoldFinger, 500L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.GoldFinger500, new LimitedItemData(LimitedShopItemType.GoldFinger500, "$3.00", list23, 10000));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.VIPPackage, new LimitedItemData(LimitedShopItemType.VIPPackage, "$10.00", new List<LimitedItemSetData>(), 10000));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.MiniVIPPackage, new LimitedItemData(LimitedShopItemType.MiniVIPPackage, "$3.00", new List<LimitedItemSetData>(), 10000));
		List<LimitedItemSetData> list24 = new List<LimitedItemSetData>();
		list24.Add(new LimitedItemSetData(LimitedAttributeType.Ruby, 2000L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.OnePlusOneDiamonds, new LimitedItemData(LimitedShopItemType.OnePlusOneDiamonds, "$5.00", list24, 1));
		List<LimitedItemSetData> list25 = new List<LimitedItemSetData>();
		list25.Add(new LimitedItemSetData(LimitedAttributeType.TimerSilverFinger, 24L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.SilverFinger, new LimitedItemData(LimitedShopItemType.SilverFinger, "$3.99", list25, 1));
		List<LimitedItemSetData> list26 = new List<LimitedItemSetData>();
		list26.Add(new LimitedItemSetData(LimitedAttributeType.GoldRouletteTicekt, 1L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.GoldRoulettePackage, new LimitedItemData(LimitedShopItemType.GoldRoulettePackage, "$1.99", list26, 100000));
		List<LimitedItemSetData> list27 = new List<LimitedItemSetData>();
		list27.Add(new LimitedItemSetData(LimitedAttributeType.TowerModeFreeTicket, 1L));
		totalLimitedItemDataDictionary.Add(LimitedShopItemType.TowerModeFreeTicketPackage, new LimitedItemData(LimitedShopItemType.TowerModeFreeTicketPackage, "$1.99", list27, 10000));
		foreach (KeyValuePair<LimitedShopItemType, LimitedItemData> item in totalLimitedItemDataDictionary)
		{
			if (isHideProductFromShop(item.Key))
			{
				continue;
			}
			if (isCanPurchaseRepeat(item.Key))
			{
				if (isContainLimitedItemFromUsedInventory(item.Value.limitedItemID))
				{
					Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList.Remove(getLimitedItemDataFromUsedInventory(item.Value.limitedItemID));
				}
				if (isContainLimitedItemFromCurrentInventory(item.Value.limitedItemID))
				{
					Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Remove(getLimitedItemDataFromCurrentInventory(item.Value.limitedItemID));
				}
				Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Insert(0, item.Value);
			}
			else if (!isContainLimitedItemFromInventory(item.Value))
			{
				Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Add(item.Value);
			}
			else if (isContainLimitedItemFromCurrentInventory(item.Key.ToString()))
			{
				LimitedItemData limitedItemDataFromCurrentInventory = getLimitedItemDataFromCurrentInventory(item.Key.ToString());
				string text = Convert.ToBase64String(Util.SerializeToStream(item.Value.limitedAttribudeList).ToArray());
				string value = Convert.ToBase64String(Util.SerializeToStream(limitedItemDataFromCurrentInventory.limitedAttribudeList).ToArray());
				Match match = Regex.Match(limitedItemDataFromCurrentInventory.itemPriceString, "^\\$\\d+\\.99");
				if (!match.Success)
				{
					limitedItemDataFromCurrentInventory.setItemString(item.Value.itemPriceString);
				}
				if (!text.Equals(value))
				{
					limitedItemDataFromCurrentInventory.limitedAttribudeList = item.Value.limitedAttribudeList;
				}
			}
		}
		sortToFirst(LimitedShopItemType.ColleagueSkinPackageC);
		sortToFirst(LimitedShopItemType.GoldFinger500);
		sortToFirst(LimitedShopItemType.UltraAFKPackage);
		sortToFirst(LimitedShopItemType.SuperAFKPackage);
		sortToFirst(LimitedShopItemType.AFKPackage);
		refreshLimitedItemList();
		UIWindowOutgame.instance.refreshShopIndicator(Math.Max(Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Count - count, 0));
	}

	private void sortToFirst(LimitedShopItemType itemType)
	{
		if (isContainLimitedItemFromCurrentInventory(itemType.ToString()))
		{
			LimitedItemData limitedItemDataFromCurrentInventory = getLimitedItemDataFromCurrentInventory(itemType.ToString());
			Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Remove(limitedItemDataFromCurrentInventory);
			Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Insert(0, limitedItemDataFromCurrentInventory);
		}
	}

	public void refreshLimitedItemList()
	{
		refreshAllLimitedItemAvailable();
		premiumItemList.Clear();
		skinItemList.Clear();
		normalItemList.Clear();
		rubyItemList.Clear();
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Count; i++)
		{
			LimitedItemData limitedItemData = Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i];
			if (isPremiumItem(limitedItemData.limitedType))
			{
				premiumItemList.Add(limitedItemData);
			}
			else if (isSkinPackage(limitedItemData.limitedType))
			{
				skinItemList.Add(limitedItemData);
			}
			else if (isItemPackage(limitedItemData.limitedType))
			{
				normalItemList.Add(limitedItemData);
			}
			else if (isDiamondPackage(limitedItemData.limitedType))
			{
				rubyItemList.Add(limitedItemData);
			}
		}
	}

	public bool isContainLimitedItemFromInventory(LimitedItemData limitedData)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Count; i++)
		{
			list.Add(Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i].limitedItemID);
		}
		for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList.Count; j++)
		{
			list.Add(Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList[j].limitedItemID);
		}
		return list.Contains(limitedData.limitedItemID);
	}

	public bool isContainLimitedItemFromUsedInventory(string limitedItemID)
	{
		if (Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList != null)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList.Count; i++)
			{
				list.Add(Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList[i].limitedItemID);
			}
			return list.Contains(limitedItemID);
		}
		return false;
	}

	public bool isContainLimitedItemFromCurrentInventory(string limitedItemID)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Count; i++)
		{
			list.Add(Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i].limitedItemID);
		}
		return list.Contains(limitedItemID);
	}

	public void refreshAllLimitedItemAvailable()
	{
		List<LimitedItemData> list = new List<LimitedItemData>();
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Count; i++)
		{
			long targetLimitedItemEndTime = Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i].targetLimitedItemEndTime;
			if (UnbiasedTime.Instance.Now().Ticks >= targetLimitedItemEndTime)
			{
				list.Add(Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i]);
			}
			for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i].limitedAttribudeList.Count; j++)
			{
				if (Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i].limitedAttribudeList[j].limitedItemType == LimitedAttributeType.PermanentAutoTap)
				{
					if (Singleton<DataManager>.instance.currentGameData.isBoughtAutoTouch && !list.Contains(Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i]))
					{
						list.Add(Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i]);
					}
				}
				else if (Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i].limitedAttribudeList[j].limitedItemType == LimitedAttributeType.AllCharacter && Singleton<CharacterSkinManager>.instance.isAllCharacterUnlocked() && !list.Contains(Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i]))
				{
					list.Add(Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i]);
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList.Add(list[k]);
			Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Remove(list[k]);
		}
		Singleton<DataManager>.instance.saveData();
	}

	public void consumeLimitedItem(LimitedItemData itemData)
	{
		LimitedItemData limitedItemDataFromCurrentInventory = getLimitedItemDataFromCurrentInventory(itemData.limitedItemID);
		if (limitedItemDataFromCurrentInventory != null)
		{
			itemData = limitedItemDataFromCurrentInventory;
		}
		if (isContainLimitedItemFromCurrentInventory(itemData.limitedItemID))
		{
			if (!isInfinityCountProduct(itemData.limitedType))
			{
				getLimitedItemDataFromCurrentInventory(itemData.limitedItemID).currentItemCount--;
			}
			if (getLimitedItemDataFromCurrentInventory(itemData.limitedItemID).currentItemCount <= 0 && !isCanPurchaseRepeat(itemData.limitedType))
			{
				Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Remove(itemData);
				if (!isContainLimitedItemFromUsedInventory(itemData.limitedItemID))
				{
					Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList.Add(itemData);
				}
			}
		}
		else if (!isContainLimitedItemFromUsedInventory(itemData.limitedItemID) && !isCanPurchaseRepeat(itemData.limitedType))
		{
			Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList.Add(itemData);
		}
		Singleton<DataManager>.instance.saveData();
		refreshLimitedItemList();
	}

	public LimitedItemData getLimitedItemDataFromCurrentInventory(string itemId)
	{
		LimitedItemData result = null;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList.Count; i++)
		{
			if (Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i].limitedItemID.Equals(itemId))
			{
				result = Singleton<DataManager>.instance.currentGameData.newCurrentTimeCheckingLimitedItemList[i];
			}
		}
		return result;
	}

	public LimitedItemData getLimitedItemDataFromUsedInventory(string itemId)
	{
		LimitedItemData result = null;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList.Count; i++)
		{
			if (Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList[i].limitedItemID.Equals(itemId))
			{
				result = Singleton<DataManager>.instance.currentGameData.newUsedLimitedItemList[i];
			}
		}
		return result;
	}

	public void purchaseDefaultItemEvent(DefaultGoldShopItemType itemType, ShopSlot targetSlot)
	{
		switch (itemType)
		{
		case DefaultGoldShopItemType.AdsGold:
			watchAdsToGetFreeGold(targetSlot.resourcesEffectTargetTransform);
			break;
		case DefaultGoldShopItemType.GoldTier1:
		{
			long price = getProductValue(itemType);
			if (Singleton<DataManager>.instance.currentGameData._ruby >= price)
			{
				UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("SHOP_PURCHASE_GOLD_RAIN_QUESTION"), GameManager.changeUnit(CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BuyGoldTier1Multiply))), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.ShopGoldBuyTier1);
					Singleton<RubyManager>.instance.decreaseRuby(price);
					Singleton<GoldManager>.instance.increaseGold(CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BuyGoldTier1Multiply));
					Singleton<FlyResourcesManager>.instance.playEffectResources(targetSlot.resourcesEffectTargetTransform, FlyResourcesManager.ResourceType.Gold, 40L, 0.02f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
				}, string.Empty);
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopSelectedType.Ruby);
				}, string.Empty);
			}
			break;
		}
		case DefaultGoldShopItemType.GoldTier2:
		{
			long price2 = getProductValue(itemType);
			if (Singleton<DataManager>.instance.currentGameData._ruby >= price2)
			{
				UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("SHOP_PURCHASE_GOLD_RAIN_QUESTION"), GameManager.changeUnit(CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BuyGoldTier2Multiply))), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.ShopGoldBuyTier2);
					Singleton<RubyManager>.instance.decreaseRuby(price2);
					Singleton<GoldManager>.instance.increaseGold(CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BuyGoldTier2Multiply));
					Singleton<FlyResourcesManager>.instance.playEffectResources(targetSlot.resourcesEffectTargetTransform, FlyResourcesManager.ResourceType.Gold, 40L, 0.02f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
				}, string.Empty);
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopSelectedType.Ruby);
				}, string.Empty);
			}
			break;
		}
		case DefaultGoldShopItemType.GoldTier3:
		{
			long price3 = getProductValue(itemType);
			if (Singleton<DataManager>.instance.currentGameData._ruby >= price3)
			{
				UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("SHOP_PURCHASE_GOLD_RAIN_QUESTION"), GameManager.changeUnit(CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BuyGoldTier3Multiply))), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.ShopGoldBuyTier3);
					Singleton<RubyManager>.instance.decreaseRuby(price3);
					Singleton<GoldManager>.instance.increaseGold(CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BuyGoldTier3Multiply));
					Singleton<FlyResourcesManager>.instance.playEffectResources(targetSlot.resourcesEffectTargetTransform, FlyResourcesManager.ResourceType.Gold, 40L, 0.02f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
				}, string.Empty);
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopSelectedType.Ruby);
				}, string.Empty);
			}
			break;
		}
		}
		UIWindowManageShop.instance.refreshSlots();
		Singleton<DataManager>.instance.saveData();
	}

	public void purchaseDefaultItemEvent(DefaultRubyShopItemType itemType, ShopSlot targetSlot)
	{
		switch (itemType)
		{
		case DefaultRubyShopItemType.Tapjoy:
			Singleton<AdsManager>.instance.ShowOfferwall();
			break;
		case DefaultRubyShopItemType.AdsRuby:
			watchAdsToGetFreeRuby(targetSlot.resourcesEffectTargetTransform);
			break;
		case DefaultRubyShopItemType.RubyTier1:
		case DefaultRubyShopItemType.RubyTier2:
		case DefaultRubyShopItemType.RubyTier3:
		case DefaultRubyShopItemType.RubyTier4:
		case DefaultRubyShopItemType.RubyTier5:
		case DefaultRubyShopItemType.RubyTier6:
                Singleton<PaymentManager>.instance.Purchase(itemType, delegate
                {
                    Singleton<RubyManager>.instance.increaseRuby(getProductValue(itemType));
                    Singleton<FlyResourcesManager>.instance.playEffectResources(targetSlot.resourcesEffectTargetTransform, FlyResourcesManager.ResourceType.Ruby, 15L, 0.02f, delegate
                    {
                        Singleton<AudioManager>.instance.playEffectSound("getgold");
                    });
                });
                break;
		}
		UIWindowManageShop.instance.refreshSlots();
		Singleton<DataManager>.instance.saveData();
	}

	public Action getLimitedBuyAction(LimitedItemData itemData, Transform effectTargetTransform, bool isActionOnlyReward = false)
	{
		return delegate
		{
			if (itemData.limitedType == LimitedShopItemType.VIPPackage || itemData.limitedType == LimitedShopItemType.MiniVIPPackage)
			{
				int num = 0;
				num = ((itemData.limitedType != LimitedShopItemType.VIPPackage) ? 7 : 30);
				UIWindowLoading.instance.openLoadingUI();
				Singleton<NanooAPIManager>.instance.CallAPI(NanooAPIManager.APIType.SUBSCRIPTION_SAVE, "VIP_REWARD", num.ToString(), Singleton<NanooAPIManager>.instance.IsExistSubscriptionItem().ToString());
			}
			else
			{
				DateTime dateTime2 = default(DateTime);
				DateTime dateTime = default(DateTime);
				for (int i = 0; i < itemData.limitedAttribudeList.Count; i++)
				{
					switch (itemData.limitedAttribudeList[i].limitedItemType)
					{
					case LimitedAttributeType.TimerSilverFinger:
						if (Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime >= UnbiasedTime.Instance.Now().Ticks)
						{
							DateTime dateTime4 = new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime).AddHours(itemData.limitedAttribudeList[i].value);
							Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime = dateTime4.Ticks;
						}
						else
						{
							Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime = UnbiasedTime.Instance.Now().AddHours(itemData.limitedAttribudeList[i].value).Ticks;
						}
						Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
						break;
					case LimitedAttributeType.PermanentAutoTap:
						Singleton<DataManager>.instance.currentGameData.isBoughtAutoTouch = true;
						Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
						break;
					case LimitedAttributeType.Ruby:
						Singleton<RubyManager>.instance.increaseRuby(itemData.limitedAttribudeList[i].value);
						Singleton<FlyResourcesManager>.instance.playEffectResources(effectTargetTransform, FlyResourcesManager.ResourceType.Ruby, (long)Mathf.Min(itemData.limitedAttribudeList[i].value, 30f), 0.02f, delegate
						{
							Singleton<AudioManager>.instance.playEffectSound("getgold");
						});
						break;
					case LimitedAttributeType.AllCharacter:
						Singleton<CharacterSkinManager>.instance.unlockAllCharacters();
						break;
					case LimitedAttributeType.Gold:
						Singleton<GoldManager>.instance.increaseGold(itemData.limitedAttribudeList[i].value);
						Singleton<FlyResourcesManager>.instance.playEffectResources(effectTargetTransform, FlyResourcesManager.ResourceType.Gold, (long)Mathf.Min(itemData.limitedAttribudeList[i].value, 30f), 0.02f, delegate
						{
							Singleton<AudioManager>.instance.playEffectSound("getgold");
						});
						break;
					case LimitedAttributeType.CountableSilverFinger:
						Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount += (int)itemData.limitedAttribudeList[i].value;
						Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
						break;
					case LimitedAttributeType.CountableGoldFinger:
						Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount += (int)itemData.limitedAttribudeList[i].value;
						Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
						break;
					case LimitedAttributeType.WarriorSkin:
					{
						CharacterSkinManager.WarriorSkinType skinType2 = (CharacterSkinManager.WarriorSkinType)((int)itemData.limitedAttribudeList[i].value - 1);
						UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType2, !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType2).isHaving, false);
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType2);
						Singleton<CharacterManager>.instance.equipCharacter(skinType2);
						break;
					}
					case LimitedAttributeType.PriestSkin:
					{
						CharacterSkinManager.PriestSkinType skinType = (CharacterSkinManager.PriestSkinType)((int)itemData.limitedAttribudeList[i].value - 1);
						UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType, !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType).isHaving, false);
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType);
						Singleton<CharacterManager>.instance.equipCharacter(skinType);
						break;
					}
					case LimitedAttributeType.ArcherSkin:
					{
						CharacterSkinManager.ArcherSkinType skinType9 = (CharacterSkinManager.ArcherSkinType)((int)itemData.limitedAttribudeList[i].value - 1);
						UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType9, !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType9).isHaving, false);
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType9);
						Singleton<CharacterManager>.instance.equipCharacter(skinType9);
						break;
					}
					case LimitedAttributeType.TranscendStone:
						Singleton<TranscendManager>.instance.increaseTranscendStone(itemData.limitedAttribudeList[i].value);
						Singleton<FlyResourcesManager>.instance.playEffectResources(effectTargetTransform, FlyResourcesManager.ResourceType.TranscendStone, (long)Mathf.Min(itemData.limitedAttribudeList[i].value, 30f), 0.02f, delegate
						{
							Singleton<AudioManager>.instance.playEffectSound("getgold");
						});
						break;
					case LimitedAttributeType.Colleague:
						Singleton<ColleagueManager>.instance.getColleagueInventoryData((ColleagueManager.ColleagueType)(itemData.limitedAttribudeList[i].value - 1)).isUnlocked = true;
						if (UIWindowColleague.instance.isOpen)
						{
							UIWindowColleague.instance.colleagueScrollRectParent.refreshAll();
						}
						break;
					case LimitedAttributeType.IcePassiveSkill:
						Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.FrostSkill).isUnlocked = true;
						if (UIWindowSkill.instance.isOpen)
						{
							UIWindowSkill.instance.skillScroll.refreshAll();
						}
						break;
					case LimitedAttributeType.MeteorPassiveSkill:
						Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.MeteorRain).isUnlocked = true;
						if (UIWindowSkill.instance.isOpen)
						{
							UIWindowSkill.instance.skillScroll.refreshAll();
						}
						break;
					case LimitedAttributeType.ReinforcementSkill:
						foreach (KeyValuePair<SkillManager.SkillType, ReinforcementSkillInventoryData> reinforcementSkillInventoryDatum in Singleton<DataManager>.instance.currentGameData.reinforcementSkillInventoryData)
						{
							if (!reinforcementSkillInventoryDatum.Value.isUnlocked)
							{
								reinforcementSkillInventoryDatum.Value.isUnlocked = true;
							}
						}
						if (UIWindowSkill.instance.isOpen)
						{
							UIWindowSkill.instance.skillScroll.refreshAll();
						}
						break;
					case LimitedAttributeType.ElopeResources:
						Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource1, itemData.limitedAttribudeList[i].value);
						Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource2, itemData.limitedAttribudeList[i].value);
						Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource3, itemData.limitedAttribudeList[i].value);
						Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource4, itemData.limitedAttribudeList[i].value);
						break;
					case LimitedAttributeType.HeartCoin:
						Singleton<ElopeModeManager>.instance.increaseHeartCoin(itemData.limitedAttribudeList[i].value);
						break;
					case LimitedAttributeType.SuperSpeedGuy:
					{
						Dictionary<ElopeModeManager.DaemonKingSkillType, double> currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode;
						Dictionary<ElopeModeManager.DaemonKingSkillType, double> dictionary = (currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode = Singleton<DataManager>.instance.currentGameData.currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode);
						ElopeModeManager.DaemonKingSkillType key;
						ElopeModeManager.DaemonKingSkillType key2 = (key = ElopeModeManager.DaemonKingSkillType.SuperSpeedGuyDaemonKing);
						double num4 = currentDaemonKingSkillRemainSecondsInventoryDataForElopeMode[key];
						dictionary[key2] = num4 + (double)itemData.limitedAttribudeList[i].value;
						break;
					}
					case LimitedAttributeType.ColleagueSkin:
					{
						ColleagueManager.ColleagueType colleagueType = (ColleagueManager.ColleagueType)(itemData.limitedAttribudeList[i].value - 1);
						Singleton<ColleagueManager>.instance.buyColleagueSkin(colleagueType, (int)itemData.limitedAttribudeList[i].secondValue);
						Singleton<ColleagueManager>.instance.equipColleagueSkin(colleagueType, (int)itemData.limitedAttribudeList[i].secondValue);
						if (UIWindowColleague.instance.isOpen)
						{
							for (int j = 0; j < UIWindowColleague.instance.colleagueScrollRectParent.itemList.Count; j++)
							{
								if ((UIWindowColleague.instance.colleagueScrollRectParent.itemList[j] as ColleagueSlotObject).currentColleagueType == colleagueType)
								{
									(UIWindowColleague.instance.colleagueScrollRectParent.itemList[j] as ColleagueSlotObject).refreshColleagueUIObject();
									break;
								}
							}
						}
						break;
					}
					case LimitedAttributeType.GoldRouletteTicekt:
						Singleton<DataManager>.instance.currentGameData.goldRouletteTicket++;
						break;
					case LimitedAttributeType.TowerModeFreeTicket:
						if (Singleton<TowerModeManager>.instance.isFreeTicketOn())
						{
							Singleton<DataManager>.instance.currentGameData.towerModeFreeTicketEndTime = new DateTime(Singleton<DataManager>.instance.currentGameData.towerModeFreeTicketEndTime).AddHours(itemData.limitedAttribudeList[i].value).Ticks;
						}
						else
						{
							Singleton<DataManager>.instance.currentGameData.towerModeFreeTicketEndTime = UnbiasedTime.Instance.Now().AddHours(itemData.limitedAttribudeList[i].value).Ticks;
						}
						if (UIWindowSelectTowerModeDifficulty.instance.isOpen)
						{
							UIWindowSelectTowerModeDifficulty.instance.refreshUI();
						}
						break;
					case LimitedAttributeType.TimerGoldFinger:
						if (Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime >= UnbiasedTime.Instance.Now().Ticks)
						{
							DateTime dateTime3 = new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime).AddHours(itemData.limitedAttribudeList[i].value);
							Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime = dateTime3.Ticks;
						}
						else
						{
							Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime = UnbiasedTime.Instance.Now().AddHours(itemData.limitedAttribudeList[i].value).Ticks;
						}
						Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
						break;
					case LimitedAttributeType.WeaponSkinMasterPiece:
						Singleton<WeaponSkinManager>.instance.increaseWeaponSkinReinforcementMasterPiece(itemData.limitedAttribudeList[i].value);
						Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.WeaponSkinReinforcementMasterPiece, (int)Mathf.Min(itemData.limitedAttribudeList[i].value, 30f), 0.02f, delegate
						{
							Singleton<AudioManager>.instance.playEffectSound("getgold");
						});
						break;
					case LimitedAttributeType.RandomLegendaryWeaponSkin:
					{
						WeaponSkinData weaponSkinData = Singleton<WeaponSkinManager>.instance.startLottery(true, true);
						if (UIWindowWeaponSkin.instance.isOpen)
						{
							UIWindowWeaponSkin.instance.openWeaponSkin(weaponSkinData.currentCharacterType);
						}
						break;
					}
					case LimitedAttributeType.RandomCharacterSkin:
					{
						int min = (int)itemData.limitedAttribudeList[i].value - 1;
						int num7 = (int)itemData.limitedAttribudeList[i].secondValue - 1;
						int num8 = UnityEngine.Random.Range(min, num7 + 1);
						int num9 = UnityEngine.Random.Range(0, 3);
						bool flag4 = false;
						switch (num9)
						{
						case 0:
						{
							CharacterSkinManager.WarriorSkinType skinType8 = (CharacterSkinManager.WarriorSkinType)num8;
							flag4 = !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType8).isHaving;
							Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType8);
							Singleton<CharacterManager>.instance.equipCharacter(skinType8);
							UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType8, flag4);
							break;
						}
						case 1:
						{
							CharacterSkinManager.PriestSkinType skinType7 = (CharacterSkinManager.PriestSkinType)num8;
							flag4 = !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType7).isHaving;
							Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType7);
							Singleton<CharacterManager>.instance.equipCharacter(skinType7);
							UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType7, flag4);
							break;
						}
						default:
						{
							CharacterSkinManager.ArcherSkinType skinType6 = (CharacterSkinManager.ArcherSkinType)num8;
							flag4 = !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType6).isHaving;
							Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType6);
							Singleton<CharacterManager>.instance.equipCharacter(skinType6);
							UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType6, flag4);
							break;
						}
						}
						break;
					}
					case LimitedAttributeType.SwordSoulSkill:
						if (!Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.SwordSoul).isUnlocked)
						{
							Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.SwordSoul).isUnlocked = true;
						}
						else
						{
							Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.SwordSoul).skillLevel = Math.Min(Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.SwordSoul).skillLevel + 1, Singleton<SkillManager>.instance.getPassiveSkillMaxLevel(SkillManager.PassiveSkillType.SwordSoul));
							if (UIWindowSkill.instance.isOpen)
							{
								UIWindowSkill.instance.skillScroll.refreshAll();
							}
						}
						break;
					case LimitedAttributeType.WarriorSpecialWeaponSkin:
					{
						WeaponSkinData specialWeaponSkinData3 = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinData(CharacterManager.CharacterType.Warrior, (int)itemData.limitedAttribudeList[i].value, WeaponSkinManager.WeaponSkinGradeType.Rare);
						Singleton<WeaponSkinManager>.instance.obtainWeaponSkin(specialWeaponSkinData3);
						UIWindowWeaponSkinLottery.instance.openLotteryUI(specialWeaponSkinData3, true);
						break;
					}
					case LimitedAttributeType.PriestSpecialWeaponSkin:
					{
						WeaponSkinData specialWeaponSkinData2 = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinData(CharacterManager.CharacterType.Priest, (int)itemData.limitedAttribudeList[i].value, WeaponSkinManager.WeaponSkinGradeType.Rare);
						Singleton<WeaponSkinManager>.instance.obtainWeaponSkin(specialWeaponSkinData2);
						UIWindowWeaponSkinLottery.instance.openLotteryUI(specialWeaponSkinData2, true);
						break;
					}
					case LimitedAttributeType.ArcherSpecialWeaponSkin:
					{
						WeaponSkinData specialWeaponSkinData = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinData(CharacterManager.CharacterType.Archer, (int)itemData.limitedAttribudeList[i].value, WeaponSkinManager.WeaponSkinGradeType.Rare);
						Singleton<WeaponSkinManager>.instance.obtainWeaponSkin(specialWeaponSkinData);
						UIWindowWeaponSkinLottery.instance.openLotteryUI(specialWeaponSkinData, true);
						break;
					}
					case LimitedAttributeType.BronzeFinger:
						Singleton<DataManager>.instance.currentGameData.isBoughtBronzeFinger = true;
						Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
						break;
					case LimitedAttributeType.HonorToken:
						Singleton<PVPManager>.instance.increasePVPHonorToken(itemData.limitedAttribudeList[i].value);
						break;
					case LimitedAttributeType.LotteryRandomAllCharacterSkin:
					case LimitedAttributeType.LotteryRandomLimitedCharacterSkin:
					{
						bool flag = itemData.limitedAttribudeList[i].limitedItemType == LimitedAttributeType.LotteryRandomLimitedCharacterSkin;
						int num5 = (int)itemData.limitedAttribudeList[i].value;
						for (int k = 0; k < num5; k++)
						{
							int num6 = UnityEngine.Random.Range(0, 3);
							bool flag2 = false;
							bool flag3 = (double)UnityEngine.Random.Range(0, 10000) / 100.0 < 10.0;
							switch (num6)
							{
							case 0:
							{
								List<CharacterSkinManager.WarriorSkinType> list5 = new List<CharacterSkinManager.WarriorSkinType>();
								for (int n = 0; n < 30; n++)
								{
									CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)n;
									List<CharacterSkinManager.WarriorSkinType> list6 = new List<CharacterSkinManager.WarriorSkinType>
									{
										CharacterSkinManager.WarriorSkinType.William,
										CharacterSkinManager.WarriorSkinType.Dragoon,
										CharacterSkinManager.WarriorSkinType.MasterWilliam
									};
									if (!Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(warriorSkinType) && !list6.Contains(warriorSkinType))
									{
										if (!flag)
										{
											if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(warriorSkinType) || Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(warriorSkinType))
											{
												if (flag3)
												{
													list5.Add(warriorSkinType);
												}
											}
											else
											{
												list5.Add(warriorSkinType);
											}
										}
										else if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(warriorSkinType) || Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(warriorSkinType))
										{
											list5.Add(warriorSkinType);
										}
									}
								}
								CharacterSkinManager.WarriorSkinType skinType5 = list5[UnityEngine.Random.Range(0, list5.Count)];
								flag2 = !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType5).isHaving;
								Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType5);
								Singleton<CharacterManager>.instance.equipCharacter(skinType5);
								UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType5, flag2);
								break;
							}
							case 1:
							{
								List<CharacterSkinManager.PriestSkinType> list3 = new List<CharacterSkinManager.PriestSkinType>();
								List<CharacterSkinManager.PriestSkinType> list4 = new List<CharacterSkinManager.PriestSkinType>
								{
									CharacterSkinManager.PriestSkinType.Olivia,
									CharacterSkinManager.PriestSkinType.Dragoness,
									CharacterSkinManager.PriestSkinType.MasterOlivia
								};
								for (int m = 0; m < 30; m++)
								{
									CharacterSkinManager.PriestSkinType priestSkinType = (CharacterSkinManager.PriestSkinType)m;
									if (!Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(priestSkinType) && !list4.Contains(priestSkinType))
									{
										if (!flag)
										{
											if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(priestSkinType) || Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(priestSkinType))
											{
												if (flag3)
												{
													list3.Add(priestSkinType);
												}
											}
											else
											{
												list3.Add(priestSkinType);
											}
										}
										else if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(priestSkinType) || Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(priestSkinType))
										{
											list3.Add(priestSkinType);
										}
									}
								}
								CharacterSkinManager.PriestSkinType skinType4 = list3[UnityEngine.Random.Range(0, list3.Count)];
								flag2 = !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType4).isHaving;
								Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType4);
								Singleton<CharacterManager>.instance.equipCharacter(skinType4);
								UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType4, flag2);
								break;
							}
							default:
							{
								List<CharacterSkinManager.ArcherSkinType> list = new List<CharacterSkinManager.ArcherSkinType>();
								List<CharacterSkinManager.ArcherSkinType> list2 = new List<CharacterSkinManager.ArcherSkinType>
								{
									CharacterSkinManager.ArcherSkinType.Windstoker,
									CharacterSkinManager.ArcherSkinType.Dragona,
									CharacterSkinManager.ArcherSkinType.MasterWindstoker
								};
								for (int l = 0; l < 30; l++)
								{
									CharacterSkinManager.ArcherSkinType archerSkinType = (CharacterSkinManager.ArcherSkinType)l;
									if (!Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(archerSkinType) && !list2.Contains(archerSkinType))
									{
										if (!flag)
										{
											if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(archerSkinType) || Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(archerSkinType))
											{
												if (flag3)
												{
													list.Add(archerSkinType);
												}
											}
											else
											{
												list.Add(archerSkinType);
											}
										}
										else if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(archerSkinType) || Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(archerSkinType))
										{
											list.Add(archerSkinType);
										}
									}
								}
								CharacterSkinManager.ArcherSkinType skinType3 = list[UnityEngine.Random.Range(0, list.Count)];
								flag2 = !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType3).isHaving;
								Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType3);
								Singleton<CharacterManager>.instance.equipCharacter(skinType3);
								UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType3, flag2);
								break;
							}
							}
						}
						break;
					}
					case LimitedAttributeType.CountDoubleSpeed:
					{
						GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
						currentGameData.countDoubleSpeed = (long)currentGameData.countDoubleSpeed + itemData.limitedAttribudeList[i].value;
						Singleton<GameManager>.instance.refreshTimeScaleMiniPopup();
						break;
					}
					case LimitedAttributeType.CountAutoOpenChest:
						Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount += (int)itemData.limitedAttribudeList[i].value;
						Singleton<TreasureChestManager>.instance.refreshAutoOpenTreasureChest();
						break;
					case LimitedAttributeType.TimerDoubleSpeed:
					{
						long num3 = 0L;
						if (Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime > UnbiasedTime.Instance.Now().Ticks)
						{
							num3 = new DateTime(Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime).AddHours((int)itemData.limitedAttribudeList[i].value).Ticks;
						}
						else
						{
							dateTime2 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
							dateTime2 = dateTime2.AddHours((int)itemData.limitedAttribudeList[i].value);
							num3 = dateTime2.Ticks;
						}
						Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime = num3;
						Singleton<GameManager>.instance.refreshTimeScaleMiniPopup();
						break;
					}
					case LimitedAttributeType.TimerAutoOpenChest:
					{
						long num2 = 0L;
						if (Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime > UnbiasedTime.Instance.Now().Ticks)
						{
							num2 = new DateTime(Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime).AddHours((int)itemData.limitedAttribudeList[i].value).Ticks;
						}
						else
						{
							dateTime = new DateTime(UnbiasedTime.Instance.Now().Ticks);
							dateTime = dateTime.AddHours((int)itemData.limitedAttribudeList[i].value);
							num2 = dateTime.Ticks;
						}
						Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime = num2;
						Singleton<TreasureChestManager>.instance.refreshAutoOpenTreasureChest();
						break;
					}
					}
				}
			}
			if (!isActionOnlyReward)
			{
				consumeLimitedItem(itemData);
				Singleton<DataManager>.instance.saveData();
				refreshAllLimitedItemAvailable();
			}
			else
			{
				Singleton<DataManager>.instance.saveData();
			}
			if ((itemData.limitedType == LimitedShopItemType.WeaponLegendarySkinPackage || itemData.limitedType == LimitedShopItemType.FlowerPackage || itemData.limitedType == LimitedShopItemType.YummyWeaponSkinPackage || itemData.limitedType == LimitedShopItemType.WeddingSkinPackage) && UIWindowPopupShop.instance.isOpen)
			{
				UIWindowPopupShop.instance.close();
			}
			if (itemData.limitedType != LimitedShopItemType.VIPPackage && itemData.limitedType != LimitedShopItemType.MiniVIPPackage)
			{
				if (UIWindowManageShop.instance.isOpen)
				{
					UIWindowManageShop.instance.openShopUI(UIWindowManageShop.instance.currentSelectedType);
				}
				if (UIWindowPopupShop.instance.isOpen)
				{
					UIWindowPopupShop.instance.openShopPopupWithType(UIWindowPopupShop.instance.currentSelectedType);
				}
			}
			if (UIWindowSurpriseLimitedItem.instance.isOpen)
			{
				UIWindowSurpriseLimitedItem.instance.close();
			}
			if (UIWindowAdvertisementLimitedProduct.instance.isOpen)
			{
				UIWindowAdvertisementLimitedProduct.instance.OnClickClose();
			}
		};
	}

	public void purchaseLimitedItemEvent(LimitedItemData itemData, Transform effectTargetTransform, Action purchaseSuccessExtraEvent = null)
	{
		Action action = getLimitedBuyAction(itemData, effectTargetTransform);
		if (itemData.limitedType != LimitedShopItemType.VIPPackage && itemData.limitedType != LimitedShopItemType.MiniVIPPackage && itemData.limitedType != LimitedShopItemType.RandomCharacterSkinPackageA && itemData.limitedType != LimitedShopItemType.FlowerPackage && itemData.limitedType != LimitedShopItemType.YummyWeaponSkinPackage && itemData.limitedType != LimitedShopItemType.WeddingSkinPackage && !isSkinPackage(itemData.limitedType))
		{
			action = (Action)Delegate.Combine(action, (Action)delegate
			{
				UIWindowDialog.openDescription("SHOP_SUCESS_PURCHASE_LIMITED_ITEM", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			});
		}
		if (purchaseSuccessExtraEvent != null)
		{
			action = (Action)Delegate.Combine(action, purchaseSuccessExtraEvent);
		}
		Singleton<PaymentManager>.instance.Purchase(itemData.limitedType, action);
	}

	public void watchAdsToGetFreeRuby(Transform effectTargetTrnasform)
	{
		if (Singleton<DataManager>.instance.currentGameData.adsRubyWatchEndTime < UnbiasedTime.Instance.Now().Ticks)
		{
			Singleton<AdsManager>.instance.showAds("freeDiamond", delegate
			{
				Singleton<RubyManager>.instance.increaseRuby(m_defaultProductRubyValueDictionary[DefaultRubyShopItemType.AdsRuby], true);
				Singleton<FlyResourcesManager>.instance.playEffectResources(effectTargetTrnasform, FlyResourcesManager.ResourceType.Ruby, (int)Mathf.Min(m_defaultProductRubyValueDictionary[DefaultRubyShopItemType.AdsRuby], 30f), 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<DataManager>.instance.currentGameData.adsRubyWatchEndTime = UnbiasedTime.Instance.Now().AddHours(1.0).Ticks;
				Singleton<DataManager>.instance.saveData();
				if (UIWindowManageShop.instance.isOpen)
				{
					UIWindowManageShop.instance.refreshSlots();
				}
				if (UIWindowPopupShop.instance.isOpen)
				{
					UIWindowPopupShop.instance.refreshSlots();
				}
				AnalyzeManager.retention(AnalyzeManager.CategoryType.WatchAd, AnalyzeManager.ActionType.FreeDiamond);
			});
		}
		else
		{
			TimeSpan timeSpan = new TimeSpan(Singleton<DataManager>.instance.currentGameData.adsRubyWatchEndTime - UnbiasedTime.Instance.Now().Ticks);
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("CANNOT_WATCH_ADS_TO_GET_FREE_RUBY"), string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) + " " + I18NManager.Get("TIME_LEFT")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void watchAdsToGetFreeGold(Transform effectTargetTrnasform)
	{
		if (Singleton<DataManager>.instance.currentGameData.adsGoldWatchEndTime < UnbiasedTime.Instance.Now().Ticks)
		{
			Singleton<AdsManager>.instance.showAds("freeGold", delegate
			{
				Singleton<GoldManager>.instance.increaseGold(CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.AdsGoldMultiply));
				Singleton<FlyResourcesManager>.instance.playEffectResources(effectTargetTrnasform, FlyResourcesManager.ResourceType.Gold, 25L, 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<DataManager>.instance.currentGameData.adsGoldWatchEndTime = UnbiasedTime.Instance.Now().AddMinutes(30.0).Ticks;
				Singleton<DataManager>.instance.saveData();
				if (UIWindowManageShop.instance.isOpen)
				{
					UIWindowManageShop.instance.refreshSlots();
				}
				if (UIWindowPopupShop.instance.isOpen)
				{
					UIWindowPopupShop.instance.refreshSlots();
				}
				AnalyzeManager.retention(AnalyzeManager.CategoryType.WatchAd, AnalyzeManager.ActionType.FreeGold);
			});
		}
		else
		{
			TimeSpan timeSpan = new TimeSpan(Singleton<DataManager>.instance.currentGameData.adsGoldWatchEndTime - UnbiasedTime.Instance.Now().Ticks);
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("CANNOT_WATCH_ADS_TO_GET_FREE_GOLD"), string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) + " " + I18NManager.Get("TIME_LEFT")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void vipRewardEvent()
	{
		Singleton<TranscendManager>.instance.increaseTranscendStone(30L);
		Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.TranscendStone, 30L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		Singleton<RubyManager>.instance.increaseRuby(50.0);
		Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.Ruby, 30L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount += 100;
		Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
		Singleton<DataManager>.instance.saveData();
	}

	public bool isInfinityCountProduct(LimitedShopItemType itemType)
	{
		bool result = false;
		switch (itemType)
		{
		case LimitedShopItemType.VIPPackage:
		case LimitedShopItemType.GoldFinger500:
		case LimitedShopItemType.MiniVIPPackage:
		case LimitedShopItemType.WeaponLegendarySkinPackage:
		case LimitedShopItemType.TranscendPackage:
		case LimitedShopItemType.HonorPackage:
		case LimitedShopItemType.DemonPackage:
		case LimitedShopItemType.RandomHeroPackage:
		case LimitedShopItemType.RandomLimitedHeroPackage:
		case LimitedShopItemType.AFKPackage:
		case LimitedShopItemType.SuperAFKPackage:
		case LimitedShopItemType.UltraAFKPackage:
			result = true;
			break;
		}
		return result;
	}

	public bool isPremiumItem(LimitedShopItemType itemType)
	{
		bool result = false;
		switch (itemType)
		{
		case LimitedShopItemType.StarterPack:
		case LimitedShopItemType.FirstPurchasePackage:
		case LimitedShopItemType.FlowerPackage:
		case LimitedShopItemType.TranscendPackage:
		case LimitedShopItemType.HonorPackage:
		case LimitedShopItemType.DemonPackage:
		case LimitedShopItemType.YummyWeaponSkinPackage:
		case LimitedShopItemType.WeddingSkinPackage:
		case LimitedShopItemType.ColleagueSkinPackageC:
			result = true;
			break;
		}
		return result;
	}

	public bool isSkinPackage(LimitedShopItemType itemType)
	{
		bool result = false;
		switch (itemType)
		{
		case LimitedShopItemType.LimitedSkinPremiumPack:
		case LimitedShopItemType.MasterSkinPackage:
		case LimitedShopItemType.WeaponLegendarySkinPackage:
		case LimitedShopItemType.RandomHeroPackage:
		case LimitedShopItemType.RandomLimitedHeroPackage:
		case LimitedShopItemType.PVPCharacterPackage:
		case LimitedShopItemType.HotSummerPackage:
		case LimitedShopItemType.DemonCosplayPackage:
		case LimitedShopItemType.HalloweenPackage:
		case LimitedShopItemType.XmasPackage:
		case LimitedShopItemType.March2018_FlowerPackage:
			result = true;
			break;
		}
		return result;
	}

	public bool isItemPackage(LimitedShopItemType itemType)
	{
		bool result = false;
		switch (itemType)
		{
		case LimitedShopItemType.GoldFinger500:
		case LimitedShopItemType.AFKPackage:
		case LimitedShopItemType.SuperAFKPackage:
		case LimitedShopItemType.UltraAFKPackage:
			result = true;
			break;
		}
		return result;
	}

	public bool isDiamondPackage(LimitedShopItemType itemType)
	{
		bool result = false;
		if (itemType == LimitedShopItemType.OnePlusOneDiamonds || itemType == LimitedShopItemType.VIPPackage || itemType == LimitedShopItemType.MiniVIPPackage)
		{
			result = true;
		}
		return result;
	}

	public bool isConsumableProduct(DefaultRubyShopItemType itemType)
	{
		return true;
	}

	public bool isConsumableProduct(LimitedShopItemType itemType)
	{
		return true;
	}

	public long getProductValue(DefaultRubyShopItemType itemType)
	{
		return m_defaultProductRubyValueDictionary[itemType];
	}

	public long getProductValue(DefaultGoldShopItemType itemType)
	{
		return m_defaultProductGoldValueDictionary[itemType];
	}

	public string getDefaultItemTitleName(DefaultRubyShopItemType itemType)
	{
		return I18NManager.Get("SHOP_RUBY_TITLE_TEXT_" + (int)(itemType + 1));
	}

	public string getDefaultItemTitleName(DefaultRubyShopItemType itemType, long value)
	{
		return string.Format(I18NManager.Get("SHOP_RUBY_TITLE_TEXT_" + (int)(itemType + 1)), value);
	}

	public string getDefaultItemTitleName(DefaultGoldShopItemType itemType)
	{
		return I18NManager.Get("SHOP_SHOP_TITLE_TEXT_" + (int)(itemType + 1));
	}

	public string getDefaultItemDescription(DefaultRubyShopItemType itemType, long count)
	{
		if (count == 0L)
		{
			return I18NManager.Get("SHOP_RUBY_DESCRIPTION_TEXT_" + (int)(itemType + 1));
		}
		return string.Format(I18NManager.Get("SHOP_RUBY_DESCRIPTION_TEXT_" + (int)(itemType + 1)), count);
	}

	public string getDefaultItemDescription(DefaultGoldShopItemType itemType, double value)
	{
		if (value == 0.0)
		{
			return I18NManager.Get("SHOP_GOLD_DESCRIPTION_TEXT_" + (int)(itemType + 1));
		}
		return string.Format(I18NManager.Get("SHOP_GOLD_DESCRIPTION_TEXT_" + (int)(itemType + 1)), GameManager.changeUnit(value));
	}

	public string getRubyProductPriceString(DefaultRubyShopItemType itemType)
	{
		string text = string.Empty;
		if (itemType != DefaultRubyShopItemType.Tapjoy)
		{
			text = Singleton<PaymentManager>.instance.GetMarketPrice(itemType);
		}
		switch (itemType)
		{
		case DefaultRubyShopItemType.Tapjoy:
			text = string.Empty;
			break;
		case DefaultRubyShopItemType.RubyTier1:
			if (text.Length <= 0)
			{
				text = "$1.00";
			}
			break;
		case DefaultRubyShopItemType.RubyTier2:
			if (text.Length <= 0)
			{
				text = "$3.00";
			}
			break;
		case DefaultRubyShopItemType.RubyTier3:
			if (text.Length <= 0)
			{
				text = "$5.00";
			}
			break;
		case DefaultRubyShopItemType.RubyTier4:
			if (text.Length <= 0)
			{
				text = "$10.00";
			}
			break;
		case DefaultRubyShopItemType.RubyTier5:
			if (text.Length <= 0)
			{
				text = "$15.00";
			}
			break;
		case DefaultRubyShopItemType.RubyTier6:
			if (text.Length <= 0)
			{
				text = "$15.00";
			}
			break;
		}
		return text;
	}

	public PurchaseType getPurchaseType(DefaultRubyShopItemType itemType)
	{
		PurchaseType result = PurchaseType.None;
		switch (itemType)
		{
		case DefaultRubyShopItemType.Tapjoy:
			result = PurchaseType.TapJoy;
			break;
		case DefaultRubyShopItemType.AdsRuby:
			result = PurchaseType.Ads;
			break;
		case DefaultRubyShopItemType.RubyTier1:
		case DefaultRubyShopItemType.RubyTier2:
		case DefaultRubyShopItemType.RubyTier3:
		case DefaultRubyShopItemType.RubyTier4:
		case DefaultRubyShopItemType.RubyTier5:
		case DefaultRubyShopItemType.RubyTier6:
			result = PurchaseType.BuyCash;
			break;
		}
		return result;
	}

	public PurchaseType getPurchaseType(DefaultGoldShopItemType itemType)
	{
		PurchaseType result = PurchaseType.None;
		switch (itemType)
		{
		case DefaultGoldShopItemType.AdsGold:
			result = PurchaseType.Ads;
			break;
		case DefaultGoldShopItemType.GoldTier1:
		case DefaultGoldShopItemType.GoldTier2:
		case DefaultGoldShopItemType.GoldTier3:
			result = PurchaseType.BuyRuby;
			break;
		}
		return result;
	}

	public Sprite getShopItemIcon(DefaultRubyShopItemType itemType)
	{
		Sprite result = null;
		for (int i = 0; i < defaultRubyItemIconSpriteDataList.Count; i++)
		{
			if (defaultRubyItemIconSpriteDataList[i].itemType == itemType)
			{
				result = defaultRubyItemIconSpriteDataList[i].iconSprite;
				break;
			}
		}
		return result;
	}

	public Sprite getShopItemIcon(DefaultGoldShopItemType itemType)
	{
		Sprite result = null;
		for (int i = 0; i < defaultGoldItemIconSpriteDataList.Count; i++)
		{
			if (defaultGoldItemIconSpriteDataList[i].itemType == itemType)
			{
				result = defaultGoldItemIconSpriteDataList[i].iconSprite;
				break;
			}
		}
		return result;
	}

	public bool isCanPurchaseRepeat(LimitedShopItemType itemType)
	{
		bool result = false;
		if (itemType == LimitedShopItemType.BadPriemiumPack || itemType == LimitedShopItemType.SilverFinger || itemType == LimitedShopItemType.VIPPackage || itemType == LimitedShopItemType.MiniVIPPackage || itemType == LimitedShopItemType.GoldFinger500)
		{
			result = true;
		}
		return result;
	}

	public bool isHideProductFromShop(LimitedShopItemType itemType)
	{
		bool result = false;
		if (itemType == LimitedShopItemType.BadPriemiumPack || itemType == LimitedShopItemType.SilverFinger)
		{
			result = true;
		}
		return result;
	}

	public string getLimitedItemTitleName(LimitedShopItemType itemType)
	{
		return I18NManager.Get("SHOP_LIMITED_TITLE_TEXT_" + (int)itemType);
	}

	public string getLimitedItemDescription(LimitedShopItemType itemType, long count)
	{
		string text = I18NManager.Get("SHOP_LIMITED_DESCRIPTION_TEXT_" + (int)itemType);
		if (count == 0L || !text.Contains("{"))
		{
			return text;
		}
		return string.Format(text, count);
	}

	public string getLimitedItemDescription(LimitedShopItemType itemType, long count, double count2, bool isGold)
	{
		string text = I18NManager.Get("SHOP_LIMITED_DESCRIPTION_TEXT_" + (int)itemType);
		if (count == 0L || !text.Contains("{"))
		{
			return I18NManager.Get("SHOP_LIMITED_DESCRIPTION_TEXT_" + (int)itemType);
		}
		if (!isGold)
		{
			return string.Format(I18NManager.Get("SHOP_LIMITED_DESCRIPTION_TEXT_" + (int)itemType), count, count2);
		}
		return string.Format(I18NManager.Get("SHOP_LIMITED_DESCRIPTION_TEXT_" + (int)itemType), count, GameManager.changeUnit(count2));
	}

	public Sprite getShopItemIcon(LimitedShopItemType itemType)
	{
		Sprite result = null;
		for (int i = 0; i < newLimitedItemIconSpriteDataList.Count; i++)
		{
			if (newLimitedItemIconSpriteDataList[i].itemType == itemType)
			{
				result = newLimitedItemIconSpriteDataList[i].iconSprite;
				break;
			}
		}
		return result;
	}

	public string getLimitedItemRibbonTitleText(LimitedShopItemType itemType)
	{
		string result = null;
		switch (itemType)
		{
		case LimitedShopItemType.RandomLimitedHeroPackage:
		case LimitedShopItemType.AFKPackage:
			result = "Hot!";
			break;
		case LimitedShopItemType.VIPPackage:
		case LimitedShopItemType.MiniVIPPackage:
			result = "VIP";
			break;
		}
		return result;
	}
}
