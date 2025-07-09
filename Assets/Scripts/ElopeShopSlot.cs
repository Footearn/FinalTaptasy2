using UnityEngine;
using UnityEngine.UI;

public class ElopeShopSlot : ObjectBase
{
	public Text typeText;

	public Text nameText;

	public Text descriptionText;

	public Text priceText;

	public GameObject buyButtonObject;

	public GameObject boughtObject;

	public RectTransform characterAndColleagueTargetSpawnTransform;

	public Image iconImage;

	public GameObject iconImageObject;

	public RectTransform iconTransform;

	public CharacterUIObject currentCharacterUIObject;

	public ColleagueUIObject currentColleagueUIObject;

	public GameObject treasureBackgroundObject;

	public GameObject pvpIconObject;

	public Image treasureBackgroundImage;

	public GameObject treasureNewIconObject;

	private ElopeShopItemData m_currentItemData;

	public void initSlot(ElopeShopItemData itemData)
	{
		m_currentItemData = itemData;
		if (currentCharacterUIObject != null)
		{
			ObjectPool.Recycle(currentCharacterUIObject.name, currentCharacterUIObject.cachedGameObject);
			currentCharacterUIObject = null;
		}
		if (currentColleagueUIObject != null)
		{
			ObjectPool.Recycle(currentColleagueUIObject.name, currentColleagueUIObject.cachedGameObject);
			currentColleagueUIObject = null;
		}
		typeText.text = Singleton<ElopeModeManager>.instance.getItemTypeI18NName(m_currentItemData.currentItemType);
		if (m_currentItemData.currentItemType == ElopeModeManager.ElopeShopItemType.CharacterSkin || m_currentItemData.currentItemType == ElopeModeManager.ElopeShopItemType.Colleague || m_currentItemData.currentItemType == ElopeModeManager.ElopeShopItemType.ColleagueSkin)
		{
			if (iconImageObject.activeSelf)
			{
				iconImageObject.SetActive(false);
			}
		}
		else
		{
			if (!iconImageObject.activeSelf)
			{
				iconImageObject.SetActive(true);
			}
			iconImage.sprite = Singleton<ElopeModeManager>.instance.getElopeShopItemIconSprite(m_currentItemData.currentItemType, m_currentItemData.value, m_currentItemData.secondValue);
			switch (m_currentItemData.currentItemType)
			{
			case ElopeModeManager.ElopeShopItemType.Gold:
			case ElopeModeManager.ElopeShopItemType.TimerSilverFinger:
			case ElopeModeManager.ElopeShopItemType.TimerGoldFinger:
			case ElopeModeManager.ElopeShopItemType.TimerAutoOpenTreasureChest:
			case ElopeModeManager.ElopeShopItemType.TimerDoubleSpeed:
			case ElopeModeManager.ElopeShopItemType.Treasure:
				iconTransform.localScale = new Vector3(0.8f, 0.8f, 1f);
				break;
			case ElopeModeManager.ElopeShopItemType.Weapon:
				iconTransform.localScale = new Vector3(4f, 4f, 1f);
				break;
			default:
				iconTransform.localScale = Vector3.one;
				break;
			}
			iconImage.SetNativeSize();
		}
		if (m_currentItemData.isBought)
		{
			if (buyButtonObject.activeSelf)
			{
				buyButtonObject.SetActive(false);
			}
			if (!boughtObject.activeSelf)
			{
				boughtObject.SetActive(true);
			}
		}
		else
		{
			if (!buyButtonObject.activeSelf)
			{
				buyButtonObject.SetActive(true);
			}
			if (boughtObject.activeSelf)
			{
				boughtObject.SetActive(false);
			}
			priceText.text = m_currentItemData.price.ToString("N0");
		}
		if (m_currentItemData.currentItemType == ElopeModeManager.ElopeShopItemType.Treasure)
		{
			if (!treasureBackgroundObject.activeSelf)
			{
				treasureBackgroundObject.SetActive(true);
			}
			treasureBackgroundImage.sprite = Singleton<TreasureManager>.instance.tierBackgroundSprites[Singleton<TreasureManager>.instance.getTreasureTier((TreasureManager.TreasureType)m_currentItemData.value)];
			pvpIconObject.SetActive(Singleton<TreasureManager>.instance.isPVPTreasure((TreasureManager.TreasureType)m_currentItemData.value));
		}
		else
		{
			pvpIconObject.SetActive(false);
			if (treasureBackgroundObject.activeSelf)
			{
				treasureBackgroundObject.SetActive(false);
			}
		}
		treasureNewIconObject.SetActive(false);
		switch (m_currentItemData.currentItemType)
		{
		case ElopeModeManager.ElopeShopItemType.Gold:
			nameText.text = I18NManager.Get("COUPON_REWARD_GOLD");
			descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_GOLD_DESCRIPTION"), GameManager.changeUnit(CalculateManager.getCurrentStandardGold() * m_currentItemData.value));
			break;
		case ElopeModeManager.ElopeShopItemType.TimerSilverFinger:
			nameText.text = I18NManager.Get("AUTOTOUCH_SKILL_NAME_1");
			descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_SILVER_FINGER_DESCRIPTION"), (long)m_currentItemData.value);
			break;
		case ElopeModeManager.ElopeShopItemType.TimerGoldFinger:
			nameText.text = I18NManager.Get("AUTOTOUCH_SKILL_NAME_2");
			descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_GOLD_FINGER_DESCRIPTION"), (long)m_currentItemData.value);
			break;
		case ElopeModeManager.ElopeShopItemType.TimerAutoOpenTreasureChest:
			nameText.text = I18NManager.Get("AUTOTOUCH_SKILL_NAME_4");
			descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_AUTO_TREASURE_CHEST_DESCRIPTION"), (long)m_currentItemData.value);
			break;
		case ElopeModeManager.ElopeShopItemType.TimerDoubleSpeed:
			nameText.text = I18NManager.Get("AUTOTOUCH_SKILL_NAME_5");
			descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_DOUBLE_SPEED_CHEST_DESCRIPTION"), (long)m_currentItemData.value);
			break;
		case ElopeModeManager.ElopeShopItemType.Colleague:
		{
			ColleagueManager.ColleagueType colleagueType = (ColleagueManager.ColleagueType)m_currentItemData.value;
			string colleagueI18NName = Singleton<ColleagueManager>.instance.getColleagueI18NName(colleagueType, 1);
			nameText.text = colleagueI18NName;
			descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_COLLEAGUE_DESCRIPTION"), colleagueI18NName);
			currentColleagueUIObject = ObjectPool.Spawn("@ColleagueUIObject" + (int)(colleagueType + 1), Vector2.zero, characterAndColleagueTargetSpawnTransform).GetComponent<ColleagueUIObject>();
			currentColleagueUIObject.cachedTransform.localScale = new Vector3(25f, 25f, 1f);
			currentColleagueUIObject.currentColleagueType = colleagueType;
			currentColleagueUIObject.initColleagueUI(colleagueType, 1);
			currentColleagueUIObject.changeLayer("PopUpLayer");
			currentColleagueUIObject.followAlphaWithCanvas(UIWindowElopeShop.instance.cachedCanvasGroup);
			break;
		}
		case ElopeModeManager.ElopeShopItemType.ColleagueSkin:
		{
			ColleagueManager.ColleagueType colleagueType2 = (ColleagueManager.ColleagueType)m_currentItemData.value;
			string colleagueI18NName2 = Singleton<ColleagueManager>.instance.getColleagueI18NName(colleagueType2, (int)m_currentItemData.secondValue);
			nameText.text = colleagueI18NName2;
			descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_COLLEAGUE_SKIN_DESCRIPTION"), colleagueI18NName2);
			currentColleagueUIObject = ObjectPool.Spawn("@ColleagueUIObject" + (int)(colleagueType2 + 1), Vector2.zero, characterAndColleagueTargetSpawnTransform).GetComponent<ColleagueUIObject>();
			currentColleagueUIObject.cachedTransform.localScale = new Vector3(25f, 25f, 1f);
			currentColleagueUIObject.currentColleagueType = colleagueType2;
			currentColleagueUIObject.initColleagueUI(colleagueType2, (int)m_currentItemData.secondValue);
			currentColleagueUIObject.changeLayer("PopUpLayer");
			currentColleagueUIObject.followAlphaWithCanvas(UIWindowElopeShop.instance.cachedCanvasGroup);
			break;
		}
		case ElopeModeManager.ElopeShopItemType.CharacterSkin:
			currentCharacterUIObject = ObjectPool.Spawn("@CharacterUIObject", new Vector2(0f, -1.2f), characterAndColleagueTargetSpawnTransform).GetComponent<CharacterUIObject>();
			currentCharacterUIObject.cachedTransform.localScale = new Vector3(25f, 25f, 1f);
			if (m_currentItemData.value == 0.0)
			{
				CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)m_currentItemData.secondValue;
				currentCharacterUIObject.initCharacterUIObject(warriorSkinType, UIWindowElopeShop.instance.cachedCanvasGroup);
				nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(warriorSkinType);
				descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_CHARACTER_SKIN_DESCRIPTION"), I18NManager.Get("WARRIOR"), nameText.text);
			}
			else if (m_currentItemData.value == 1.0)
			{
				CharacterSkinManager.PriestSkinType skinType = (CharacterSkinManager.PriestSkinType)m_currentItemData.secondValue;
				currentCharacterUIObject.initCharacterUIObject((CharacterSkinManager.PriestSkinType)m_currentItemData.secondValue, UIWindowElopeShop.instance.cachedCanvasGroup);
				nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(skinType);
				descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_CHARACTER_SKIN_DESCRIPTION"), I18NManager.Get("PRIEST"), nameText.text);
			}
			else if (m_currentItemData.value == 2.0)
			{
				CharacterSkinManager.ArcherSkinType skinType2 = (CharacterSkinManager.ArcherSkinType)m_currentItemData.secondValue;
				currentCharacterUIObject.initCharacterUIObject((CharacterSkinManager.ArcherSkinType)m_currentItemData.secondValue, UIWindowElopeShop.instance.cachedCanvasGroup);
				nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(skinType2);
				descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_CHARACTER_SKIN_DESCRIPTION"), I18NManager.Get("ARCHER"), nameText.text);
			}
			currentCharacterUIObject.changeLayer("PopUpLayer");
			break;
		case ElopeModeManager.ElopeShopItemType.Weapon:
			if (m_currentItemData.value == 0.0)
			{
				nameText.text = Singleton<WeaponManager>.instance.getWeaponI18NName((WeaponManager.WarriorWeaponType)m_currentItemData.secondValue);
				descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_WEAPON_DESCRIPTION"), nameText.text, I18NManager.Get("WARRIOR"));
			}
			else if (m_currentItemData.value == 1.0)
			{
				nameText.text = Singleton<WeaponManager>.instance.getWeaponI18NName((WeaponManager.PriestWeaponType)m_currentItemData.secondValue);
				descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_WEAPON_DESCRIPTION"), nameText.text, I18NManager.Get("PRIEST"));
			}
			else if (m_currentItemData.value == 2.0)
			{
				nameText.text = Singleton<WeaponManager>.instance.getWeaponI18NName((WeaponManager.ArcherWeaponType)m_currentItemData.secondValue);
				descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_WEAPON_DESCRIPTION"), nameText.text, I18NManager.Get("ARCHER"));
			}
			break;
		case ElopeModeManager.ElopeShopItemType.TreasureKey:
			nameText.text = I18NManager.Get("COUPON_REWARD_KEYS");
			descriptionText.text = string.Format(I18NManager.Get("ELOPE_SHOP_TREASURE_KEY_DESCRIPTION"), (long)m_currentItemData.value);
			break;
		case ElopeModeManager.ElopeShopItemType.Treasure:
		{
			TreasureManager.TreasureType treasureType = (TreasureManager.TreasureType)m_currentItemData.value;
			long num = ((!Singleton<TreasureManager>.instance.containTreasureFromInventory(treasureType)) ? 1 : (Singleton<TreasureManager>.instance.getTreasureDataFromInventory(treasureType).treasureLevel + 1));
			if (m_currentItemData.isBought)
			{
				num--;
			}
			nameText.text = Singleton<TreasureManager>.instance.getTreasureI18NName(treasureType) + " <size=18><color=#FAD725>Lv." + num + "</color></size>";
			if (Singleton<TreasureManager>.instance.containTreasureFromInventory(treasureType))
			{
				descriptionText.text = Singleton<TreasureManager>.instance.getTreasureDescription(treasureType, Singleton<TreasureManager>.instance.getTreasureDataFromInventory(treasureType).treasureEffectValue + Singleton<TreasureManager>.instance.getTreasureDataFromInventory(treasureType).extraTreasureEffectValue + (m_currentItemData.isBought ? 0.0 : Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].increasingValueEveryEnchant), 0.0);
				break;
			}
			treasureNewIconObject.SetActive(true);
			descriptionText.text = Singleton<TreasureManager>.instance.getTreasureDescription(treasureType, Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].treasureEffectValue, 0.0);
			break;
		}
		}
	}

	public void OnClickBuy()
	{
		if (m_currentItemData.isBought)
		{
			return;
		}
		UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("ELOPE_SHOP_BUY_DESCRIPTION"), nameText.text), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			long price = m_currentItemData.price;
			if (Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode >= price)
			{
				bool isSuccess = false;
				Singleton<ElopeModeManager>.instance.buyItem(m_currentItemData, out isSuccess);
				if (isSuccess)
				{
					Singleton<AudioManager>.instance.playEffectSound("buy_success");
					if (m_currentItemData.currentItemType != ElopeModeManager.ElopeShopItemType.Treasure)
					{
						UIWindowDialog.openMiniDialog("SUCCESS_BUY");
					}
					Transform transform = ObjectPool.Spawn("@CollectEventRewardEffect", iconTransform.position).transform;
					transform.SetParent(base.cachedTransform);
					Singleton<ElopeModeManager>.instance.decreaseHeartCoin(price);
					m_currentItemData.isBought = true;
					Singleton<DataManager>.instance.saveData();
					initSlot(m_currentItemData);
				}
			}
			else
			{
				string arg = I18NManager.Get("ELOPE_HEART_COIN");
				UIWindowDialog.openMiniDialogWithoutI18N(string.Format(I18NManager.Get("NOT_ENOUGH"), arg) + " \n<size=22><color=#FDFCB7>" + I18NManager.Get("ELOPE_SHOP_DESCIPRIOTN_2") + "</color></size>");
			}
		}, string.Empty);
	}
}
