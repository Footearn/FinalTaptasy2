using UnityEngine;
using UnityEngine.UI;

public class PVPShopSlot : ObjectBase
{
	public CharacterUIObject currentCharacterUIObject;

	public ColleagueUIObject currentColleagueUIObject;

	public RectTransform centerRectTransform;

	public GameObject iconObject;

	public RectTransform iconRectTransform;

	public Image iconImage;

	public Text tagText;

	public GameObject levelTextObject;

	public Text levelText;

	public Text nameText;

	public RectTransform resourceIconImageRectTransform;

	public Image resourceIconImage;

	public Text priceText;

	public GameObject buyButtonObject;

	public GameObject purchaseCompleteObject;

	private PVPManager.PVPShopItemData m_currentItemData;

	private PVPManager.PVPPurchaseData m_currentPurchaseData;

	public void initPVPShopSlot(PVPManager.PVPShopItemData itemData)
	{
		if (itemData == null)
		{
			base.cachedGameObject.SetActive(false);
			m_currentItemData = null;
			m_currentPurchaseData = null;
			return;
		}
		base.cachedGameObject.SetActive(true);
		m_currentItemData = itemData;
		m_currentPurchaseData = Singleton<PVPManager>.instance.getPurchaseData(m_currentItemData);
		buyButtonObject.SetActive(!m_currentItemData.isBought);
		purchaseCompleteObject.SetActive(m_currentItemData.isBought);
		if (currentColleagueUIObject != null)
		{
			ObjectPool.Recycle(currentColleagueUIObject.name, currentColleagueUIObject.cachedGameObject);
			currentColleagueUIObject = null;
		}
		if (currentCharacterUIObject.cachedGameObject.activeSelf)
		{
			currentCharacterUIObject.cachedGameObject.SetActive(false);
		}
		if (!iconObject.activeSelf)
		{
			iconObject.SetActive(true);
		}
		iconRectTransform.localScale = Vector3.one;
		levelTextObject.SetActive(false);
		switch (m_currentItemData.shopItemType)
		{
		case PVPManager.ShopItemType.CharacterSkin:
			iconObject.SetActive(false);
			currentCharacterUIObject.cachedGameObject.SetActive(true);
			tagText.text = I18NManager.Get("SKIN");
			switch ((int)itemData.values[0])
			{
			case 0:
			{
				CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)itemData.values[1];
				currentCharacterUIObject.initCharacterUIObject(warriorSkinType, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(warriorSkinType);
				if (Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(warriorSkinType).isHaving)
				{
					levelTextObject.SetActive(true);
					levelText.text = "Lv." + Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(warriorSkinType)._skinLevel;
				}
				else
				{
					levelTextObject.SetActive(false);
				}
				break;
			}
			case 1:
			{
				CharacterSkinManager.PriestSkinType priestSkinType = (CharacterSkinManager.PriestSkinType)itemData.values[1];
				currentCharacterUIObject.initCharacterUIObject(priestSkinType, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(priestSkinType);
				if (Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(priestSkinType).isHaving)
				{
					levelTextObject.SetActive(true);
					levelText.text = "Lv." + Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(priestSkinType)._skinLevel;
				}
				else
				{
					levelTextObject.SetActive(false);
				}
				break;
			}
			case 2:
			{
				CharacterSkinManager.ArcherSkinType archerSkinType = (CharacterSkinManager.ArcherSkinType)itemData.values[1];
				currentCharacterUIObject.initCharacterUIObject(archerSkinType, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(archerSkinType);
				if (Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(archerSkinType).isHaving)
				{
					levelTextObject.SetActive(true);
					levelText.text = "Lv." + Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(archerSkinType)._skinLevel;
				}
				else
				{
					levelTextObject.SetActive(false);
				}
				break;
			}
			}
			break;
		case PVPManager.ShopItemType.ColleagueSkin:
		{
			iconObject.SetActive(false);
			levelTextObject.SetActive(true);
			tagText.text = I18NManager.Get("ELOPE_COLLEAGUE_SKIN_TYPE_TEXT");
			ColleagueManager.ColleagueType colleagueType = (ColleagueManager.ColleagueType)itemData.values[0];
			currentColleagueUIObject = ObjectPool.Spawn("@ColleagueUIObject" + (itemData.values[0] + 1.0), Vector2.zero, centerRectTransform).GetComponent<ColleagueUIObject>();
			currentColleagueUIObject.cachedTransform.localScale = new Vector3(110f, 110f, 1f);
			currentColleagueUIObject.currentColleagueType = colleagueType;
			currentColleagueUIObject.initColleagueUI(colleagueType, (int)itemData.values[1]);
			currentColleagueUIObject.changeLayer("PopUpLayer2");
			currentColleagueUIObject.followAlphaWithCanvas(UIWindowPVPMainUI.instance.cachedCanvasGroup);
			nameText.text = Singleton<ColleagueManager>.instance.getColleagueI18NName(colleagueType, (int)itemData.values[1]);
			levelText.text = "Lv." + Singleton<ColleagueManager>.instance.getColleagueInventoryData(colleagueType).colleaugeSkinLevelData[(int)itemData.values[1]];
			break;
		}
		case PVPManager.ShopItemType.SpecialWeaponSkin:
			levelTextObject.SetActive(true);
			tagText.text = I18NManager.Get("SPECIAL_WEAPON_SKIN");
			switch ((int)itemData.values[0])
			{
			case 0:
			{
				WeaponSkinManager.WarriorSpecialWeaponSkinType skinType3 = (WeaponSkinManager.WarriorSpecialWeaponSkinType)itemData.values[1];
				iconImage.sprite = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(skinType3);
				nameText.text = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinName(skinType3);
				levelText.text = I18NManager.Get("WARRIOR");
				break;
			}
			case 1:
			{
				WeaponSkinManager.PriestSpecialWeaponSkinType skinType2 = (WeaponSkinManager.PriestSpecialWeaponSkinType)itemData.values[1];
				iconImage.sprite = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(skinType2);
				nameText.text = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinName(skinType2);
				levelText.text = I18NManager.Get("PRIEST");
				break;
			}
			case 2:
			{
				WeaponSkinManager.ArcherSpecialWeaponSkinType skinType = (WeaponSkinManager.ArcherSpecialWeaponSkinType)itemData.values[1];
				iconImage.sprite = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(skinType);
				nameText.text = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinName(skinType);
				levelText.text = I18NManager.Get("ARCHER");
				break;
			}
			}
			iconRectTransform.localScale = Vector3.one * 4f;
			break;
		case PVPManager.ShopItemType.BuyTank:
			tagText.text = I18NManager.Get("PVP_TANK");
			iconImage.sprite = Singleton<CachedManager>.instance.tankUnlockIconSprites[(int)m_currentItemData.values[0]];
			nameText.text = string.Format(I18NManager.Get("UNLOCKED_2"), Singleton<PVPManager>.instance.getTankName((int)m_currentItemData.values[0]));
			break;
		case PVPManager.ShopItemType.UpgradeTank:
			levelTextObject.SetActive(true);
			tagText.text = I18NManager.Get("PVP_TANK");
			iconImage.sprite = Singleton<CachedManager>.instance.tankUpgradeIconSprites[(int)m_currentItemData.values[0]];
			nameText.text = Singleton<PVPManager>.instance.getTankName((int)m_currentItemData.values[0]) + " " + I18NManager.Get("UPGRADE");
			levelText.text = "Lv." + Singleton<DataManager>.instance.currentGameData.pvpTankData[(int)m_currentItemData.values[0]].tankLevel;
			break;
		}
		iconImage.SetNativeSize();
		resourceIconImage.sprite = Singleton<PVPManager>.instance.getPurchaseResourceIconSprite(m_currentPurchaseData.purchaseType);
		switch (m_currentPurchaseData.purchaseType)
		{
		case PVPManager.PurchaseType.Ruby:
			resourceIconImageRectTransform.localScale = Vector3.one;
			break;
		case PVPManager.PurchaseType.HonorToken:
			resourceIconImageRectTransform.localScale = Vector3.one * 1.2f;
			break;
		case PVPManager.PurchaseType.ElopeHeartCoin:
			resourceIconImageRectTransform.localScale = Vector3.one * 0.75f;
			break;
		}
		resourceIconImage.SetNativeSize();
		priceText.text = m_currentPurchaseData.price.ToString("N0");
	}

	public void OnClickPurchaseItem()
	{
		if (m_currentItemData.isBought)
		{
			return;
		}
		UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("ELOPE_SHOP_BUY_DESCRIPTION"), nameText.text), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			bool flag = false;
			switch (m_currentPurchaseData.purchaseType)
			{
			case PVPManager.PurchaseType.Ruby:
				flag = (double)Singleton<DataManager>.instance.currentGameData._ruby >= m_currentPurchaseData.price;
				break;
			case PVPManager.PurchaseType.HonorToken:
				flag = (double)(long)Singleton<DataManager>.instance.currentGameData.pvpHonorToken >= m_currentPurchaseData.price;
				break;
			case PVPManager.PurchaseType.ElopeHeartCoin:
				flag = (double)Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode >= m_currentPurchaseData.price;
				break;
			}
			if (flag)
			{
				switch (m_currentPurchaseData.purchaseType)
				{
				case PVPManager.PurchaseType.Ruby:
					Singleton<RubyManager>.instance.decreaseRuby(m_currentPurchaseData.price);
					break;
				case PVPManager.PurchaseType.HonorToken:
					Singleton<PVPManager>.instance.decreasePVPHonorToken((long)m_currentPurchaseData.price);
					break;
				case PVPManager.PurchaseType.ElopeHeartCoin:
					Singleton<ElopeModeManager>.instance.decreaseHeartCoin((long)m_currentPurchaseData.price);
					break;
				}
				Singleton<PVPManager>.instance.purchaseEvent(m_currentItemData);
				initPVPShopSlot(m_currentItemData);
				Singleton<DataManager>.instance.saveData();
				Singleton<AudioManager>.instance.playEffectSound("buy_success");
				UIWindowDialog.openMiniDialog("SUCCESS_BUY");
				UIWindowPVPMainUI.instance.openPVPMainUI(false);
			}
			else
			{
				switch (m_currentPurchaseData.purchaseType)
				{
				case PVPManager.PurchaseType.Ruby:
					UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
					{
						UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
					}, string.Empty);
					break;
				case PVPManager.PurchaseType.HonorToken:
					UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("NOT_ENOUGH"), I18NManager.Get("PVP_HONOR_TOKEN")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
					break;
				case PVPManager.PurchaseType.ElopeHeartCoin:
					UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("NOT_ENOUGH"), I18NManager.Get("ELOPE_HEART_COIN")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
					break;
				}
			}
		}, string.Empty);
	}
}
