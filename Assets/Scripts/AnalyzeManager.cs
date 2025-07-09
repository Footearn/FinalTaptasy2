using System.Collections.Generic;
using Prime31;
using TapjoyUnity;
using UnityEngine;

public class AnalyzeManager : MonoBehaviour
{
	public enum CategoryType
	{
		Dungeon,
		LevelUp,
		Unlock,
		Achievement,
		Rebirth,
		Gold,
		Diamond,
		WatchAd,
		Angel,
		Review,
		Purchase,
		Coupon,
		Treasure,
		BossRaid,
		EventReward,
		Postbox,
		SpecialAngel
	}

	public enum ActionType
	{
		Clear,
		Fail,
		Timeover,
		LevelUp,
		Achievement,
		Quest,
		ThemeUnlock,
		Rebirth,
		LotteryTreasure,
		DestroyTreasure,
		EnchantTreasure,
		BuyWeaponByGold,
		BuySkinByGold,
		GoldDungeon,
		JackPot,
		BuySkinByDiamond,
		BuyGold,
		ResetGoldDungeon,
		BuyAutoTouch24H,
		FreeDiamond,
		AdGoldDungeon,
		JackPotAd,
		TapjoyFreeDiamond,
		AngelTouch,
		AngelLater,
		AngelAd,
		ReviewRate,
		ReviewLater,
		ReviewClose,
		BuyDiamondTier3,
		BuyDiamondTier5,
		BuyDiamondTier10,
		BuyDiamondTier20,
		BuyDiamondTier60,
		AutoTouch,
		StarterPack,
		CouponDiamond,
		CouponGold,
		CouponAutoTouch,
		CouponTreasureKey,
		CouponTreasureEnchantStone,
		CouponEvent,
		EnterBossRaid,
		ClearBossRaid,
		GetBossRaidChest,
		GetCharacterFromBossRaid,
		GetWeaponSkinFromBossRaid,
		GetTreasureEnchantStoneFromBossRaid,
		GetTreasureKeyFromBossRaid,
		GetDiamondFromBossRaid,
		GetSkillPointFromBossRaid,
		GetFreeHeartByDiamond,
		GetGoldFromEventCode,
		GetDiamondFromEventCode,
		GetTreasureKeyFromEventCode,
		GetAutoTouchFromEventCode,
		GetTreasureEnchantStoneFromEventCode,
		PostboxDiamond,
		PostboxGold,
		PostboxAutoTouch,
		PostboxTreasure,
		PostboxTreasureKey,
		PostboxPowerStone,
		PostboxEvent,
		PurchaseOneplusOnePackage,
		PurchaseSkin1,
		PurchaseMonthlyDiamond,
		PurchasePremiumPackage,
		PurchasePremiumSkinPackage,
		PurchaseFirstPurchasePackage,
		PurchaseVIPPackage,
		SkillGoldFinger,
		SkillSilverFinger,
		LotteryTreasureByRuby,
		ShopGoldBuyTier1,
		ShopGoldBuyTier2,
		ShopGoldBuyTier3,
		FreeGold,
		SurpriseShopOpen,
		SurpriseShopClose,
		SkillGoldFingerCount,
		SkillSilverFingerCount,
		GoldFinger500,
		PurchaseMiniVIPPackage,
		NewEventPackage,
		ValkyriePackage,
		DoubleSpeed,
		BuyDiamondTier1,
		AutoOpenTreasureChest,
		AngelicaPackage,
		IceMeteorSkillPackage,
		AngelinaPackage,
		ReinforcementSkillPackage,
		ElopeSuperHandsomeGuy,
		ElopeSuperSpeedGuy,
		ElopeResourcesGuy,
		BrotherHoodPackage,
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
		HotSummerPackage,
		DemonCosplayPackage,
		HalloweenPackage,
		XmasPackage,
		March2018_FlowerPackage,
		YummyWeaponSkinPackage,
		WeddingSkinPackage,
		ColleagueSkinPackageC
	}

	public const string FLURRY_KEY = "J3594CH8WXJWSC2Z36FK";

	private static bool m_isInit;

	public static void retention(CategoryType category, ActionType action)
	{
		init();
		//FlurryAnalytics.logEvent(action.ToString(), false);
		if (Tapjoy.IsConnected)
		{
			Tapjoy.TrackEvent(action.ToString(), 0L);
		}
	}

	public static void retention(CategoryType category, ActionType action, Dictionary<string, string> param)
	{
		init();
		//FlurryAnalytics.logEvent(action.ToString(), param, false);
		if (Tapjoy.IsConnected)
		{
			Tapjoy.TrackEvent(action.ToString(), 0L);
		}
	}

	public static void init()
	{
		if (!m_isInit)
		{
			m_isInit = true;
			//FlurryAnalytics.startSession("J3594CH8WXJWSC2Z36FK");
		}
	}
}
