using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColleagueSkinSlot : SlotObject
{
	public ColleagueManager.ColleagueType currentColleagueType;

	public int currentSkinIndex;

	public Image mainBackgroundImage;

	public Text nameText;

	public Text damageText;

	public Text damageDetailDescriptionText;

	public RectTransform backgroundImageTrasnform;

	public GameObject damageObject;

	public GameObject noBonusObject;

	public Text priceText;

	public Text premiumSkinPriceText;

	public RectTransform lockedObjectTransform;

	public GameObject lockedObject;

	public GameObject unlockObject;

	public GameObject equipButtonObject;

	public GameObject equippedObject;

	public ColleagueUIObject currentColleagueUIObject;

	public GameObject unlockButtonObject;

	public GameObject elopeShopDescriptionTextObject;

	public GameObject premiumSkinUnlockButtonObject;

	public GameObject premiumSkinRibbonObject;

	public PVPSkillManager.PVPSkillTypeData currentPVPSkillTypeData;

	public Image pvpSkillIconImage;

	private ColleagueInventoryData m_currentColleagueInventoryData;

	private bool m_isPremiumSkin;

	private List<GameObject> m_currentEquipEffectList = new List<GameObject>();

	public void initSlot(ColleagueManager.ColleagueType colleagueType, int skinIndex)
	{
		currentColleagueType = colleagueType;
		currentSkinIndex = skinIndex;
		if (currentColleagueUIObject != null)
		{
			ObjectPool.Recycle(currentColleagueUIObject.name, currentColleagueUIObject.cachedGameObject);
		}
		currentColleagueUIObject = null;
		currentColleagueUIObject = ObjectPool.Spawn("@ColleagueUIObject" + (int)(currentColleagueType + 1), Vector2.zero, backgroundImageTrasnform).GetComponent<ColleagueUIObject>();
		currentColleagueUIObject.cachedTransform.localPosition = new Vector2(0f, 57f);
		currentColleagueUIObject.cachedTransform.localScale = new Vector3(95f, 95f, 1f);
		currentColleagueUIObject.currentColleagueType = currentColleagueType;
		currentColleagueUIObject.initColleagueUI(currentColleagueType, currentSkinIndex);
		currentColleagueUIObject.changeLayer("PopUpLayer");
		currentColleagueUIObject.followAlphaWithCanvas(UIWindowColleagueInformation.instance.cachedCanvasGroup);
		lockedObjectTransform.SetAsLastSibling();
		m_currentColleagueInventoryData = Singleton<ColleagueManager>.instance.getColleagueInventoryData(currentColleagueType);
		PVPManager.PVPSkinData pVPSkinData = new PVPManager.PVPSkinData();
		pVPSkinData.currentColleagueType = currentColleagueType;
		pVPSkinData.currentColleagueSkinIndex = currentSkinIndex;
		currentPVPSkillTypeData = Singleton<PVPSkillManager>.instance.getSkillType(pVPSkinData);
		pvpSkillIconImage.sprite = Singleton<PVPSkillManager>.instance.getSkillIconSprite(currentPVPSkillTypeData);
		refreshSlot();
	}

	public override void refreshSlot()
	{
		double colleagueSkinStat = Singleton<ColleagueManager>.instance.getColleagueSkinStat(currentColleagueType, currentSkinIndex);
		bool flag = Singleton<ColleagueManager>.instance.containColleagueSkin(currentColleagueType, currentSkinIndex);
		nameText.text = Singleton<ColleagueManager>.instance.getColleagueI18NName(currentColleagueType, currentSkinIndex);
		damageObject.SetActive(false);
		noBonusObject.SetActive(false);
		if (colleagueSkinStat > 0.0)
		{
			damageObject.SetActive(true);
			damageText.text = "+" + colleagueSkinStat + "%";
		}
		else
		{
			noBonusObject.SetActive(true);
		}
		unlockObject.SetActive(false);
		lockedObject.SetActive(false);
		if (currentSkinIndex >= 4)
		{
			premiumSkinRibbonObject.SetActive(true);
			m_isPremiumSkin = true;
		}
		else
		{
			premiumSkinRibbonObject.SetActive(false);
			m_isPremiumSkin = false;
		}
		if (flag)
		{
			unlockObject.SetActive(true);
			nameText.color = Util.getCalculatedColor(30f, 199f, 255f);
			damageText.color = Color.white;
			damageDetailDescriptionText.color = Color.white;
			if (m_currentColleagueInventoryData.currentEquippedSkinIndex == currentSkinIndex)
			{
				mainBackgroundImage.sprite = Singleton<CachedManager>.instance.enableThumbnailCharacterSkinEquippedSprite;
				equipButtonObject.SetActive(false);
				equippedObject.SetActive(true);
			}
			else
			{
				mainBackgroundImage.sprite = Singleton<CachedManager>.instance.enableThumbnailWeaponSprite;
				equipButtonObject.SetActive(true);
				equippedObject.SetActive(false);
			}
			return;
		}
		lockedObject.SetActive(true);
		if (currentSkinIndex >= 4)
		{
			unlockButtonObject.SetActive(false);
			if (m_currentColleagueInventoryData.level >= 1000)
			{
				premiumSkinUnlockButtonObject.SetActive(true);
				elopeShopDescriptionTextObject.SetActive(false);
				premiumSkinPriceText.text = Singleton<ColleagueManager>.instance.getColleaguePremiumSkinUnlockPrice(currentColleagueType, currentSkinIndex).ToString("N0");
			}
			else
			{
				premiumSkinUnlockButtonObject.SetActive(false);
				elopeShopDescriptionTextObject.SetActive(true);
			}
		}
		else
		{
			unlockButtonObject.SetActive(true);
			premiumSkinUnlockButtonObject.SetActive(false);
			elopeShopDescriptionTextObject.SetActive(false);
		}
		mainBackgroundImage.sprite = Singleton<CachedManager>.instance.disableThumbnailSprite;
		nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
		damageText.color = Util.getCalculatedColor(153f, 153f, 153f);
		damageDetailDescriptionText.color = Util.getCalculatedColor(153f, 153f, 153f);
		priceText.text = Singleton<ColleagueManager>.instance.getColleagueSkinUnlockPrice().ToString("N0");
	}

	public void OnClickBuySkin()
	{
		long price = ((!m_isPremiumSkin) ? Singleton<ColleagueManager>.instance.getColleagueSkinUnlockPrice() : Singleton<ColleagueManager>.instance.getColleaguePremiumSkinUnlockPrice(currentColleagueType, currentSkinIndex));
		long num = ((!m_isPremiumSkin) ? Singleton<DataManager>.instance.currentGameData._ruby : Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode);
		if (num >= price)
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("CHARACTER_BUY_ASK_TEXT"), I18NManager.Get(Singleton<ColleagueManager>.instance.getColleagueI18NTitleID(currentColleagueType, currentSkinIndex))), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				if (m_isPremiumSkin)
				{
					Singleton<ElopeModeManager>.instance.decreaseHeartCoin(price);
				}
				else
				{
					Singleton<RubyManager>.instance.decreaseRuby(price);
				}
				Singleton<ColleagueManager>.instance.buyColleagueSkin(currentColleagueType, currentSkinIndex);
				Singleton<ColleagueManager>.instance.equipColleagueSkin(currentColleagueType, currentSkinIndex);
				OnClickEquip();
			}, string.Empty);
		}
		else if (m_isPremiumSkin)
		{
			string arg = I18NManager.Get("ELOPE_HEART_COIN");
			UIWindowDialog.openMiniDialogWithoutI18N(string.Format(I18NManager.Get("NOT_ENOUGH"), arg) + " \n<size=22><color=#FDFCB7>" + I18NManager.Get("ELOPE_SHOP_DESCIPRIOTN_2") + "</color></size>");
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}

	public void OnClickEquip()
	{
		if (!m_currentColleagueInventoryData.colleagueSkinData[currentSkinIndex])
		{
			return;
		}
		Singleton<ColleagueManager>.instance.equipColleagueSkin(currentColleagueType, currentSkinIndex);
		UIWindowColleagueInformation.instance.refreshInformation();
		UIWindowColleagueInformation.instance.colleagueSkinParentScrollRect.refreshSlots();
		UIWindowColleague.instance.colleagueScrollRectParent.refreshAll();
		for (int i = 0; i < UIWindowColleague.instance.colleagueScrollRectParent.itemList.Count; i++)
		{
			if ((UIWindowColleague.instance.colleagueScrollRectParent.itemList[i] as ColleagueSlotObject).currentColleagueType == currentColleagueType)
			{
				(UIWindowColleague.instance.colleagueScrollRectParent.itemList[i] as ColleagueSlotObject).refreshColleagueUIObject();
				break;
			}
		}
		for (int j = 0; j < m_currentEquipEffectList.Count; j++)
		{
			ObjectPool.Recycle(m_currentEquipEffectList[j].name, m_currentEquipEffectList[j]);
		}
		m_currentEquipEffectList.Clear();
		m_currentEquipEffectList.Add(ObjectPool.Spawn("fx_character_upgrade", new Vector2(0f, -0.277f), new Vector3(0f, 180f, 180f), currentColleagueUIObject.cachedTransform));
		m_currentEquipEffectList.Add(ObjectPool.Spawn("fx_character_upgrade", new Vector2(0f, -0.277f), new Vector3(0f, 180f, 180f), UIWindowColleagueInformation.instance.currentColleagueUIObject.cachedTransform));
		Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
	}

	public void OnClickOpenPVPSimpleInformation()
	{
		if (currentPVPSkillTypeData != null)
		{
			parentScrollRect.horizontal = false;
			UIWindowPVPSimpleSkillInformation.instance.openSkillInformation(currentPVPSkillTypeData, delegate
			{
				parentScrollRect.horizontal = true;
			});
		}
	}
}
