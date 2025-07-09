using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PostItemSlot : ScrollSlotItem
{
	private ItemType itemType;

	private double itemValue;

	private string itemId;

	private string itemCode;

	public Image mainBackgroundImage;

	public Image itemIconImage;

	public Text descriptionText;

	public Text remainTimeText;

	private DateTime receiveCheckTime;

	private Action callbackForItemReceive;

	public override void UpdateItem(int index)
	{
		slotIndex = index;
		if ((slotIndex + 1) % 2 == 0)
		{
			mainBackgroundImage.color = new Color(0f, 0f, 0f, 0f);
		}
		else
		{
			mainBackgroundImage.color = Util.getCalculatedColor(0f, 11f, 28f, 51f);
		}
		PostBoxManager.PostboxItem item = Singleton<PostBoxManager>.instance.GetItem(index);
		if (item != null)
		{
			base.cachedGameObject.SetActive(true);
			itemId = item.itemId;
			itemCode = item.itemCode;
			itemValue = item.itemQuantity;
			itemType = item.itemType;
			descriptionText.text = GetItemDesc(item.itemType);
			remainTimeText.text = GetRemainTimeDesc(item.itemRemianSec);
			if (itemType == ItemType.SilverFinger || itemType == ItemType.GoldFinger || itemType == ItemType.CountSilverFinger || itemType == ItemType.CountGoldFinger)
			{
				itemIconImage.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			}
			else if (itemType == ItemType.TimerAutoOpenTreasureChest || itemType == ItemType.CountAutoOpenTreasureChest)
			{
				itemIconImage.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
			}
			else if (itemType == ItemType.TranscendStone)
			{
				itemIconImage.transform.localScale = Vector3.one;
			}
			else if (itemType == ItemType.HeartForElopeMode || itemType == ItemType.HeartCoinForElopeMode)
			{
				itemIconImage.transform.localScale = Vector3.one * 2f;
			}
			else if (itemType == ItemType.TowerModeRankingTimeAttackCharacterSkin || itemType == ItemType.TowerModeRankingEndlessCharacterSkin || itemType == ItemType.TowerModeRankingBothCharacterSkin)
			{
				itemIconImage.transform.localScale = Vector3.one * 0.6f;
			}
			else if (itemType == ItemType.PVPHonorToken)
			{
				itemIconImage.transform.localScale = Vector3.one * 2.5f;
			}
			else
			{
				itemIconImage.transform.localScale = Vector3.one * 0.8f;
			}
			if (itemType != ItemType.Treasure)
			{
				itemIconImage.sprite = Singleton<PostBoxManager>.instance.getItemIconSprite(item.itemType);
			}
			else
			{
				itemIconImage.sprite = Singleton<TreasureManager>.instance.getTreasureSprite((TreasureManager.TreasureType)itemValue);
			}
			itemIconImage.SetNativeSize();
		}
		else
		{
			base.cachedGameObject.SetActive(false);
		}
	}

	private string GetItemDesc(ItemType type)
	{
		string empty = string.Empty;
		string text = ((I18NManager.currentLanguage != I18NManager.Language.Korean && I18NManager.currentLanguage != I18NManager.Language.ChineseSimplified && I18NManager.currentLanguage != I18NManager.Language.ChineseTraditional) ? string.Empty : I18NManager.Get("KOREAN_COUNT"));
		switch (type)
		{
		case ItemType.Gold:
			empty = string.Format(I18NManager.Get("POST_BOX_ITEM_DESCRIPTION"), I18NManager.Get("COUPON_REWARD_GOLD"), GameManager.changeUnit(itemValue));
			break;
		case ItemType.Ruby:
			empty = string.Format(I18NManager.Get("POST_BOX_ITEM_DESCRIPTION"), I18NManager.Get("COUPON_REWARD_RUBY"), itemValue + text);
			break;
		case ItemType.Stone:
			empty = string.Format(I18NManager.Get("POST_BOX_ITEM_DESCRIPTION"), I18NManager.Get("COUPON_REWARD_TREASURE_ENCHANT_STONE"), itemValue + text);
			break;
		case ItemType.TreasureKey:
			empty = string.Format(I18NManager.Get("POST_BOX_ITEM_DESCRIPTION"), I18NManager.Get("COUPON_REWARD_KEYS"), itemValue + text);
			break;
		case ItemType.Preorder:
			empty = string.Format(I18NManager.Get("SHOP_RUBY_TITLE_TEXT_2"), (long)itemValue) + ", " + I18NManager.Get("WARRIOR_SKIN_NAME_8");
			break;
		case ItemType.HeroSquadNormal:
			empty = I18NManager.Get("PRIEST_SKIN_NAME_8");
			break;
		case ItemType.HeroSquadPaid1:
			empty = string.Format(I18NManager.Get("SHOP_RUBY_TITLE_TEXT_2"), (long)itemValue) + ", " + I18NManager.Get("PRIEST_SKIN_NAME_8") + ", " + I18NManager.Get("ARCHER_SKIN_NAME_8");
			break;
		case ItemType.HeroSquadPaid2:
		case ItemType.HeroSquadPaid3:
			empty = string.Format(I18NManager.Get("SHOP_RUBY_TITLE_TEXT_2"), (long)itemValue) + ", " + I18NManager.Get("COUPON_REWARD_KEYS") + 60 + text + I18NManager.Get("KOREAN_COUNT") + ", " + I18NManager.Get("PRIEST_SKIN_NAME_8") + "," + I18NManager.Get("ARCHER_SKIN_NAME_8");
			break;
		case ItemType.LimitedPremiumPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.PremiumPack);
			break;
		case ItemType.LimitedAllHeroPack:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.AllCharacterPack);
			break;
		case ItemType.LimitedOnePlusOneDiamond:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.OnePlusOneDiamonds);
			break;
		case ItemType.LimitedStarterPack:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.StarterPack);
			break;
		case ItemType.SurpriseSilverFinger:
			empty = "<color=white>" + I18NManager.Get("SURPRISE_SHOP_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.SilverFinger);
			break;
		case ItemType.SurpriseAutoTapPack:
			empty = "<color=white>" + I18NManager.Get("SURPRISE_SHOP_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.BadPriemiumPack);
			break;
		case ItemType.EventPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.EventPackage);
			break;
		case ItemType.WarriorSkin:
			empty = "<color=white>" + I18NManager.Get("WARRIOR") + " " + I18NManager.Get("SKIN") + "</color> : " + Singleton<CharacterSkinManager>.instance.getCharacterSkinName((CharacterSkinManager.WarriorSkinType)(itemValue - 1.0));
			break;
		case ItemType.PriestSkin:
			empty = "<color=white>" + I18NManager.Get("PRIEST") + " " + I18NManager.Get("SKIN") + "</color> : " + Singleton<CharacterSkinManager>.instance.getCharacterSkinName((CharacterSkinManager.PriestSkinType)(itemValue - 1.0));
			break;
		case ItemType.ArcherSkin:
			empty = "<color=white>" + I18NManager.Get("ARCHER") + " " + I18NManager.Get("SKIN") + "</color> : " + Singleton<CharacterSkinManager>.instance.getCharacterSkinName((CharacterSkinManager.ArcherSkinType)(itemValue - 1.0));
			break;
		case ItemType.SilverFinger:
			empty = I18NManager.Get("AUTOTOUCH_SKILL_NAME_1") + " <color=white>" + itemValue + I18NManager.Get("MINUTES") + "</color>";
			break;
		case ItemType.GoldFinger:
			empty = I18NManager.Get("AUTOTOUCH_SKILL_NAME_2") + " <color=white>" + itemValue + I18NManager.Get("MINUTES") + "</color>";
			break;
		case ItemType.Treasure:
			empty = "<color=white>" + I18NManager.Get("TREASURE") + "</color> : " + Singleton<TreasureManager>.instance.getTreasureI18NName((TreasureManager.TreasureType)itemValue);
			break;
		case ItemType.LimitedPremiumSkinPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.LimitedSkinPremiumPack);
			break;
		case ItemType.LimitedFirstPurchasePackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.FirstPurchasePackage);
			break;
		case ItemType.LimitedMonthlyVIPPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.VIPPackage);
			break;
		case ItemType.VIPReward:
			empty = string.Format(I18NManager.Get("POST_BOX_ITEM_DESCRIPTION"), I18NManager.Get("SHOP_LIMITED_TITLE_TEXT_10"), I18NManager.Get("REWARD"));
			break;
		case ItemType.CountSilverFinger:
			empty = I18NManager.Get("AUTOTOUCH_SKILL_NAME_1") + " <color=white>" + string.Format(I18NManager.Get("COUNT_AUTO_TAP_COUNT"), itemValue) + "</color>";
			break;
		case ItemType.CountGoldFinger:
			empty = I18NManager.Get("AUTOTOUCH_SKILL_NAME_3") + " <color=white>" + string.Format(I18NManager.Get("COUNT_AUTO_TAP_COUNT"), itemValue) + "</color>";
			break;
		case ItemType.TranscendStone:
			empty = string.Format(I18NManager.Get("POST_BOX_ITEM_DESCRIPTION"), I18NManager.Get("TRANSCEND_STONE"), itemValue + text);
			break;
		case ItemType.ValkyriePackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.ValkyrieSkinPackage);
			break;
		case ItemType.NewEventPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.NewTranscendEventPackage);
			break;
		case ItemType.MiniLimitedMonthlyVIPPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.MiniVIPPackage);
			break;
		case ItemType.DoubleSpeed:
			empty = I18NManager.Get("AUTOTOUCH_SKILL_NAME_5") + " <color=white>" + itemValue + I18NManager.Get("MINUTES") + "</color>";
			break;
		case ItemType.CountAutoOpenTreasureChest:
			empty = I18NManager.Get("AUTOTOUCH_SKILL_NAME_4") + " <color=white>" + string.Format(I18NManager.Get("COUNT_AUTO_TAP_COUNT"), itemValue) + "</color>";
			break;
		case ItemType.AngelicaPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.AngelicaPackage);
			break;
		case ItemType.Colleague:
			empty = "<color=white>" + I18NManager.Get("COLLEAGUE") + "</color> : " + Singleton<ColleagueManager>.instance.getColleagueI18NName((ColleagueManager.ColleagueType)(itemValue - 1.0), 1);
			break;
		case ItemType.TimerAutoOpenTreasureChest:
			empty = I18NManager.Get("AUTOTOUCH_SKILL_NAME_4") + " <color=white>" + itemValue + I18NManager.Get("MINUTES") + "</color>";
			break;
		case ItemType.CollectEventResource:
			empty = string.Format(I18NManager.Get("POST_BOX_ITEM_DESCRIPTION"), I18NManager.Get("COLLECT_EVENT_RESOURCE"), itemValue + text);
			break;
		case ItemType.IceMeteorPassiveSkillPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.IceMeteorSkillPackage);
			break;
		case ItemType.AngelinaPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.AngelinaPackage);
			break;
		case ItemType.ReinforcementSkillPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.ReinforcementSkillPackage);
			break;
		case ItemType.ReinforcementSkill:
			empty = "<color=white>" + I18NManager.Get("SUPER_SKILL") + "</color>";
			break;
		case ItemType.BrotherhoodPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.BrotherhoodPackage);
			break;
		case ItemType.HeartForElopeMode:
			empty = "<color=white>" + I18NManager.Get("ELOPE_MODE") + "</color> : " + string.Format(I18NManager.Get("ELOPE_HEART_POST_ITEM"), GameManager.changeUnit(itemValue));
			break;
		case ItemType.HeartCoinForElopeMode:
			empty = "<color=white>" + I18NManager.Get("ELOPE_MODE") + "</color> : " + string.Format(I18NManager.Get("ELOPE_HEART_COIM_POST_ITEM"), (long)itemValue);
			break;
		case ItemType.ElopeResources:
			empty = "<color=white>" + I18NManager.Get("ELOPE_MODE") + "</color> : " + string.Format(I18NManager.Get("ELOPE_RESOURCES_POST_ITEM"), (long)itemValue);
			break;
		case ItemType.HandsomeGuy:
			empty = "<color=white>" + I18NManager.Get("ELOPE_MODE") + "</color> : " + I18NManager.Get("ELOPE_DAEMON_KING_SLOT_TITLE_4");
			break;
		case ItemType.SuperHandsomeGuy:
			empty = "<color=white>" + I18NManager.Get("ELOPE_MODE") + "</color> : " + I18NManager.Get("ELOPE_DAEMON_KING_SLOT_TITLE_5");
			break;
		case ItemType.SpeedGuy:
			empty = "<color=white>" + I18NManager.Get("ELOPE_MODE") + "</color> : " + I18NManager.Get("ELOPE_DAEMON_KING_SLOT_TITLE_6");
			break;
		case ItemType.SuperSpeedGuy:
			empty = "<color=white>" + I18NManager.Get("ELOPE_MODE") + "</color> : " + I18NManager.Get("ELOPE_DAEMON_KING_SLOT_TITLE_7");
			break;
		case ItemType.DemonKingSkinPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.DemonKingSkinPackage);
			break;
		case ItemType.AngelaPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.AngelaPackage);
			break;
		case ItemType.GoblinPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.GoblinPackage);
			break;
		case ItemType.ZeusPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.ZeusPackage);
			break;
		case ItemType.ColleagueSkinPackageA:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.ColleagueSkinPackageA);
			break;
		case ItemType.EssentialPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.EssentialPackage);
			break;
		case ItemType.GoldRoulettePackage:
			empty = Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.GoldRoulettePackage);
			break;
		case ItemType.ColleagueSkinPackageB:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.ColleagueSkinPackageB);
			break;
		case ItemType.TowerModeFreeTicketPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.TowerModeFreeTicketPackage);
			break;
		case ItemType.ConquerorTokenTreasure:
			empty = string.Format(I18NManager.Get("TOWER_MODE_REWARD_POST_BOX_DESCRIPTION"), "<color=white>" + I18NManager.Get("TREASURE") + " : " + Singleton<TreasureManager>.instance.getTreasureI18NName(TreasureManager.TreasureType.ConquerToken) + "</color>");
			break;
		case ItemType.PatienceTokenTreasure:
			empty = string.Format(I18NManager.Get("TOWER_MODE_REWARD_POST_BOX_DESCRIPTION"), "<color=white>" + I18NManager.Get("TREASURE") + " : " + Singleton<TreasureManager>.instance.getTreasureI18NName(TreasureManager.TreasureType.PatienceToken) + "</color>");
			break;
		case ItemType.TowerModeRankingTimeAttackCharacterSkin:
			empty = string.Format(I18NManager.Get("TOWER_MODE_REWARD_POST_BOX_DESCRIPTION"), "<color=white>" + I18NManager.Get("TOWER_MODE_CHARACTER_SKIN") + " : " + Singleton<CharacterSkinManager>.instance.getCharacterSkinName(CharacterSkinManager.ArcherSkinType.MasterWindstoker) + "</color>");
			break;
		case ItemType.TowerModeRankingEndlessCharacterSkin:
			empty = string.Format(I18NManager.Get("TOWER_MODE_REWARD_POST_BOX_DESCRIPTION"), "<color=white>" + I18NManager.Get("TOWER_MODE_CHARACTER_SKIN") + " : " + Singleton<CharacterSkinManager>.instance.getCharacterSkinName(CharacterSkinManager.PriestSkinType.MasterOlivia) + "</color>");
			break;
		case ItemType.TowerModeRankingBothCharacterSkin:
			empty = string.Format(I18NManager.Get("TOWER_MODE_REWARD_POST_BOX_DESCRIPTION"), "<color=white>" + I18NManager.Get("TOWER_MODE_CHARACTER_SKIN") + " : " + Singleton<CharacterSkinManager>.instance.getCharacterSkinName(CharacterSkinManager.WarriorSkinType.MasterWilliam) + "</color>");
			break;
		case ItemType.MasterSkinPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.MasterSkinPackage);
			break;
		case ItemType.WeaponSkinPiece:
			empty = "<color=white>" + string.Format(I18NManager.Get("WEAPON_SKIN_PIECE"), itemValue.ToString("N0")) + "</color>";
			break;
		case ItemType.WeaponSkinReinforcementMasterPiece:
			empty = "<color=white>" + string.Format(I18NManager.Get("WEAPON_SKIN_MASTER_PIECE"), itemValue.ToString("N0")) + "</color>";
			break;
		case ItemType.WeaponSkinPremiumLottery:
			empty = "<color=white>" + I18NManager.Get("WEAPON_SKIN") + " : " + I18NManager.Get("WEAPON_SKIN_PREMIUM_LOTTERY") + "</color>";
			break;
		case ItemType.WeaponLegendarySkinPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.WeaponLegendarySkinPackage);
			break;
		case ItemType.RandomCharacterSkinPackageA:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.RandomCharacterSkinPackageA);
			break;
		case ItemType.GoldenPackageA:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.GoldenPackageA);
			break;
		case ItemType.GoldenPackageB:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.GoldenPackageB);
			break;
		case ItemType.GoldenPackageC:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.GoldenPackageC);
			break;
		case ItemType.FlowerPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.FlowerPackage);
			break;
		case ItemType.PVPHonorToken:
			empty = "PvP : " + I18NManager.Get("PVP_HONOR_TOKEN") + " " + itemValue + I18NManager.Get("KOREAN_COUNT");
			break;
		case ItemType.TranscendPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.TranscendPackage);
			break;
		case ItemType.HonorPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.HonorPackage);
			break;
		case ItemType.DemonPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.DemonPackage);
			break;
		case ItemType.RandomHeroPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.RandomHeroPackage);
			break;
		case ItemType.RandomLimitedHeroPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.RandomLimitedHeroPackage);
			break;
		case ItemType.AFKPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.AFKPackage);
			break;
		case ItemType.PVPCharacterPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.PVPCharacterPackage);
			break;
		case ItemType.SuperAFKPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.SuperAFKPackage);
			break;
		case ItemType.UltraAFKPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.UltraAFKPackage);
			break;
		case ItemType.BronzeFinger:
			empty = I18NManager.Get("BRONZE_FINGER");
			break;
		case ItemType.HotSummerPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.HotSummerPackage);
			break;
		case ItemType.DemonCosplayPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.DemonCosplayPackage);
			break;
		case ItemType.HalloweenPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.HalloweenPackage);
			break;
		case ItemType.ForcePaidUser:
			empty = "<color=white>" + I18NManager.Get("FORCE_PAIDUSER") + "</color>";
			break;
		case ItemType.XmasPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.XmasPackage);
			break;
		case ItemType.March2018_FlowerPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.March2018_FlowerPackage);
			break;
		case ItemType.YummyWeaponSkinPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.YummyWeaponSkinPackage);
			break;
		case ItemType.WeddingSkinPackage:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.WeddingSkinPackage);
			break;
		case ItemType.ColleagueSkinPackageC:
			empty = "<color=white>" + I18NManager.Get("SHOP_LIMITED_MAIN_TITLE_TEXT") + "</color> : " + Singleton<ShopManager>.instance.getLimitedItemTitleName(ShopManager.LimitedShopItemType.ColleagueSkinPackageB);
			break;
		default:
			empty = "Unknown Item";
			break;
		}
		string empty2 = string.Empty;
		if (type == ItemType.Gold || type == ItemType.Ruby || type == ItemType.Stone || type == ItemType.TreasureKey || type == ItemType.VIPReward || type == ItemType.TranscendStone || type == ItemType.CollectEventResource || type == ItemType.ConquerorTokenTreasure || type == ItemType.PatienceTokenTreasure || type == ItemType.TowerModeRankingTimeAttackCharacterSkin || type == ItemType.TowerModeRankingEndlessCharacterSkin || type == ItemType.TowerModeRankingBothCharacterSkin)
		{
			return empty;
		}
		return string.Format(I18NManager.Get("POST_BOX_ITEM_DESCRIPTION_2"), empty);
	}

	private string GetRemainTimeDesc(double s)
	{
		string result = string.Empty;
		if (s > 0.0)
		{
			int num = (int)s / 86400;
			int num2 = (int)s % 86400 / 3600;
			int num3 = (int)s % 86400 % 3600 / 60;
			result = string.Format(((num == 0) ? string.Empty : ("{0:D1}" + I18NManager.Get("SHOP_LIMITED_DAY_SIMPLE"))) + ((num2 == 0) ? string.Empty : ("{1:D1}" + I18NManager.Get("HOURS_INTERVAL"))) + ((num3 == 0) ? string.Empty : ("{2:D1}" + I18NManager.Get("MINUTES"))) + " " + I18NManager.Get("TIME_LEFT").Trim(), num, num2, num3);
		}
		return result;
	}

	public void OnClickRecieve()
	{
		UIWindowLoading.instance.openLoadingUI();
		StartCoroutine(ItemReceive());
	}

	public IEnumerator ItemReceive()
	{
		receiveCheckTime = UnbiasedTime.Instance.Now();
		bool isSuccess = true;
		callbackForItemReceive = Singleton<NanooAPIManager>.instance.GetActionByItemType(itemType, itemValue, base.cachedTransform, out isSuccess);
		Singleton<NanooAPIManager>.instance.CallAPI(NanooAPIManager.APIType.POSTBOX_RECEIVE, itemId);
		while (!Singleton<NanooAPIManager>.instance.isPostItemReceiveComplete && UnbiasedTime.Instance.Now().Subtract(receiveCheckTime).TotalSeconds < 10.0)
		{
			yield return null;
		}
		UIWindowLoading.instance.closeLoadingUI();
		if (Singleton<NanooAPIManager>.instance.isPostItemReceiveComplete)
		{
			if (Singleton<NanooAPIManager>.instance.isPostItemReceiveSuccess)
			{
				if (callbackForItemReceive != null)
				{
					callbackForItemReceive();
					Singleton<DataManager>.instance.saveData();
					Singleton<PostBoxManager>.instance.RemoveItem(slotIndex);
				}
			}
			else
			{
				UIWindowDialog.openDescriptionNotUsingI18N("Error: " + Singleton<NanooAPIManager>.instance.getErrorMessage, UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		}
		callbackForItemReceive = null;
	}

	public override void refreshSlot()
	{
		int postboxItemCount = Singleton<PostBoxManager>.instance.GetPostboxItemCount();
		if (slotIndex < 0 || slotIndex >= postboxItemCount)
		{
			base.cachedGameObject.SetActive(false);
			return;
		}
		if (!base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(false);
		}
		UpdateItem(slotIndex);
	}
}
