using UnityEngine;
using UnityEngine.UI;

public class UIWindowCharacterSkin : UIWindow
{
	public static UIWindowCharacterSkin instance;

	public CanvasGroup cachedCanvasGroup;

	public RectTransform characterInformationTransform;

	public Text mainTitleText;

	public Text damageText;

	public Text damageDetailText;

	public Text damageDetailValueText;

	public Image secondStatImage;

	public Text secondStatText;

	public Text secondStatDetailText;

	public Text secondStatDetailVauleText;

	public GameObject bonusStatSetForWarriorAndPriest;

	public GameObject bonusStatSetForArcher;

	public Text bonusDamageText;

	public Text bonusDamageTextForArcher;

	public Image bonusSecondStatImage;

	public Text bonusSecondStatText;

	public Text bonusStatDescriptionText;

	public GameObject criticalDamageObject;

	public Text criticalDamageText;

	public CharacterUIObject currentCharacterUIObject;

	public CharacterSkinInfiniteScroll skinScroll;

	public Image transcendProgressBarImage;

	private CharacterManager.CharacterType m_currentTargetCharacterType;

	public int currentType;

	public Text transcendDescriptionText;

	public Text transcendCostText;

	public GameObject transcendButtonEffectObject;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openCharacterSkinUI(int type)
	{
		if (!m_isClosing)
		{
			instance.skinScroll.parentScrollRect.horizontal = true;
			transcendButtonEffectObject.SetActive(true);
			if (!Singleton<DataManager>.instance.currentGameData.isSeenCharacterSkinDescription)
			{
				Singleton<DataManager>.instance.currentGameData.isSeenCharacterSkinDescription = true;
				UIWindowManageHeroAndWeapon.instance.refreshSkinDescriptionObject();
				Singleton<DataManager>.instance.saveData();
			}
			criticalDamageObject.SetActive(false);
			currentType = type;
			Singleton<StatManager>.instance.refreshAllStats();
			long num = Singleton<DataManager>.instance.currentGameData.currentTranscendTier[(CharacterManager.CharacterType)type];
			transcendProgressBarImage.fillAmount = (float)num / (float)Singleton<TranscendManager>.instance.getTranscendMaxTier();
			if (num < Singleton<TranscendManager>.instance.getTranscendMaxTier())
			{
				transcendDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_TIER"), num + 1);
				transcendCostText.text = Singleton<DataManager>.instance.currentGameData.transcendStone.ToString("N0") + " / " + Singleton<TranscendManager>.instance.getTranscendCost(num + 1).ToString("N0");
			}
			else
			{
				transcendDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_TIER"), num);
				transcendCostText.text = Singleton<DataManager>.instance.currentGameData.transcendStone.ToString("N0") + " / Max";
			}
			Singleton<CharacterSkinManager>.instance.refreshCharacterTypeList();
			skinScroll.refreshMaxCount(Singleton<CharacterSkinManager>.instance.sortedSkinList[(CharacterManager.CharacterType)type].Count);
			bonusStatDescriptionText.text = string.Format(I18NManager.Get("CHARACTER_SKIN_BONUS_DESCRIPTION"), I18NManager.Get(((CharacterManager.CharacterType)type).ToString().ToUpper()));
			mainTitleText.text = I18NManager.Get(((CharacterManager.CharacterType)type).ToString().ToUpper());
			if (currentCharacterUIObject != null)
			{
				ObjectPool.Recycle(currentCharacterUIObject.name, currentCharacterUIObject.cachedGameObject);
			}
			currentCharacterUIObject = null;
			string poolName = "@CharacterUIObject";
			currentCharacterUIObject = ObjectPool.Spawn(poolName, new Vector2(-8.2f, 17.5f), characterInformationTransform).GetComponent<CharacterUIObject>();
			currentCharacterUIObject.RefreshMaterial();
			bonusStatSetForWarriorAndPriest.SetActive(false);
			bonusStatSetForArcher.SetActive(false);
			switch (type)
			{
			case 0:
			{
				secondStatImage.sprite = Singleton<CachedManager>.instance.warriorSecondStatIconSprite;
				secondStatImage.SetNativeSize();
				CharacterWarrior warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
				warriorCharacter.resetProperties();
				damageText.text = GameManager.changeUnit(warriorCharacter.curDamage) + " <size=23><color=#FAD725>" + I18NManager.Get("DAMAGE_DESCRIPTION_FOR_DAMAGE") + "</color></size>";
				secondStatText.text = GameManager.changeUnit(warriorCharacter.baseHealth) + " <size=23><color=#FAD725>" + I18NManager.Get("HEALTH_DESCRIPTION_FOR_CHARACTER_SKIN") + "</color></size>";
				double weaponDamage3 = warriorCharacter.equippedWeapon.weaponRealStats.weaponDamage;
				double num11 = Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType.Warrior);
				double num12 = Singleton<StatManager>.instance.warriorPercentDamageFromColleague + Singleton<StatManager>.instance.allPercentDamageFromColleague;
				double num13 = Singleton<StatManager>.instance.allPercentDamageFromTreasure + Singleton<StatManager>.instance.percentDamagePM + Singleton<StatManager>.instance.percentDamageAM + Singleton<StatManager>.instance.extraPercentDamageFromConquerorRing;
				damageDetailText.text = I18NManager.Get("WEAPON_STAT") + "\n" + I18NManager.Get("SKIN_BONUS") + "\n" + I18NManager.Get("COLLEAGUE_BONUS") + "\n" + I18NManager.Get("TREASURE_BONUS") + "\n";
				damageDetailValueText.text = GameManager.changeUnit(weaponDamage3) + "\n" + "+" + num11.ToString("N0") + "%\n" + "+" + num12.ToString("N0") + "%\n" + "+" + num13.ToString("N0") + "%";
				double secondStatValue3 = warriorCharacter.equippedWeapon.weaponRealStats.secondStatValue;
				double num14 = Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinSecondStatValue(CharacterManager.CharacterType.Warrior);
				double allPercentHealthFromTreasure = Singleton<StatManager>.instance.allPercentHealthFromTreasure;
				double warriorPercentHealthFromColleague = Singleton<StatManager>.instance.warriorPercentHealthFromColleague;
				secondStatDetailText.text = I18NManager.Get("WEAPON_STAT") + "\n" + I18NManager.Get("SKIN_BONUS") + "\n" + I18NManager.Get("COLLEAGUE_BONUS") + "\n" + I18NManager.Get("TREASURE_BONUS");
				secondStatDetailVauleText.text = GameManager.changeUnit(secondStatValue3) + "\n" + "+" + num14.ToString("N0") + "%\n" + "+" + warriorPercentHealthFromColleague.ToString("N0") + "%\n" + "+" + allPercentHealthFromTreasure.ToString("N0") + "%";
				CharacterSkinManager.WarriorSkinType equippedWarriorSkin = Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin;
				currentCharacterUIObject.initCharacterUIObject(equippedWarriorSkin, cachedCanvasGroup);
				bonusStatSetForWarriorAndPriest.SetActive(true);
				bonusDamageText.text = "+" + Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType.Warrior).ToString("N0") + "%";
				bonusSecondStatText.text = "+" + Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinSecondStatValue(CharacterManager.CharacterType.Warrior).ToString("N0") + "%";
				bonusSecondStatImage.sprite = Singleton<CachedManager>.instance.warriorSecondStatIconSprite;
				break;
			}
			case 1:
			{
				secondStatImage.sprite = Singleton<CachedManager>.instance.priestSecondStatIconSprite;
				secondStatImage.SetNativeSize();
				CharacterPriest priestCharacter = Singleton<CharacterManager>.instance.priestCharacter;
				priestCharacter.resetProperties();
				damageText.text = GameManager.changeUnit(priestCharacter.curDamage) + " <size=23><color=#FAD725>" + I18NManager.Get("DAMAGE_DESCRIPTION_FOR_DAMAGE") + "</color></size>";
				secondStatText.text = GameManager.changeUnit(Singleton<StatManager>.instance.healValueEveryTap, false) + " <size=23><color=#FAD725>" + I18NManager.Get("TAPHEAL_DESCRIPTION_FOR_CHARACTER_SKIN") + "</color></size>";
				double weaponDamage2 = priestCharacter.equippedWeapon.weaponRealStats.weaponDamage;
				double num7 = Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType.Priest);
				double num8 = Singleton<StatManager>.instance.priestPercentDamageFromColleague + Singleton<StatManager>.instance.allPercentDamageFromColleague;
				double num9 = Singleton<StatManager>.instance.allPercentDamageFromTreasure + Singleton<StatManager>.instance.percentDamagePM + Singleton<StatManager>.instance.percentDamageAM + Singleton<StatManager>.instance.extraPercentDamageFromConquerorRing;
				damageDetailText.text = I18NManager.Get("WEAPON_STAT") + "\n" + I18NManager.Get("SKIN_BONUS") + "\n" + I18NManager.Get("COLLEAGUE_BONUS") + "\n" + I18NManager.Get("TREASURE_BONUS");
				damageDetailValueText.text = GameManager.changeUnit(weaponDamage2) + "\n" + "+" + num7.ToString("N0") + "%\n" + "+" + num8.ToString("N0") + "%\n" + "+" + num9.ToString("N0") + "%";
				double secondStatValue2 = priestCharacter.equippedWeapon.weaponRealStats.secondStatValue;
				double num10 = Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinSecondStatValue(CharacterManager.CharacterType.Priest);
				double extraPercentTapHealValueFromTreasure = Singleton<StatManager>.instance.extraPercentTapHealValueFromTreasure;
				double priestPercentTapHealFromColleague = Singleton<StatManager>.instance.priestPercentTapHealFromColleague;
				secondStatDetailText.text = I18NManager.Get("WEAPON_STAT") + "\n" + I18NManager.Get("SKIN_BONUS") + "\n" + I18NManager.Get("COLLEAGUE_BONUS") + "\n" + I18NManager.Get("TREASURE_BONUS");
				secondStatDetailVauleText.text = GameManager.changeUnit(secondStatValue2, false) + "\n" + "+" + num10.ToString("N0") + "%\n" + "+" + priestPercentTapHealFromColleague.ToString("N0") + "%\n" + "+" + extraPercentTapHealValueFromTreasure.ToString("N0") + "%";
				CharacterSkinManager.PriestSkinType equippedPriestSkin = Singleton<DataManager>.instance.currentGameData.equippedPriestSkin;
				currentCharacterUIObject.initCharacterUIObject(equippedPriestSkin, cachedCanvasGroup);
				bonusStatSetForWarriorAndPriest.SetActive(true);
				bonusDamageText.text = "+" + Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType.Priest).ToString("N0") + "%";
				bonusSecondStatText.text = "+" + Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinSecondStatValue(CharacterManager.CharacterType.Priest).ToString("N0") + "%";
				bonusSecondStatImage.sprite = Singleton<CachedManager>.instance.priestSecondStatIconSprite;
				break;
			}
			case 2:
			{
				criticalDamageObject.SetActive(true);
				secondStatImage.sprite = Singleton<CachedManager>.instance.archerSecondStatIconSprite;
				secondStatImage.SetNativeSize();
				CharacterArcher archerCharacter = Singleton<CharacterManager>.instance.archerCharacter;
				archerCharacter.resetProperties();
				damageText.text = GameManager.changeUnit(archerCharacter.curDamage) + " <size=23><color=#FAD725>" + I18NManager.Get("DAMAGE_DESCRIPTION_FOR_DAMAGE") + "</color></size>";
				secondStatText.text = Mathf.Min(archerCharacter.curCriticalChance, 100f) + "% <size=23><color=#FAD725>" + I18NManager.Get("CRITICAL_DESCRIPTION_FOR_CHARACTER_SKIN") + "</color></size>";
				criticalDamageText.text = archerCharacter.curCriticalDamage + "% <size=23><color=#FAD725>" + I18NManager.Get("CRITICAL_DAMAGE_DESCRIPTION_FOR_CHARACTER_SKIN") + "</color></size>";
				double weaponDamage = archerCharacter.equippedWeapon.weaponRealStats.weaponDamage;
				double num2 = Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType.Archer);
				double num3 = Singleton<StatManager>.instance.archerPercentDamageFromColleague + Singleton<StatManager>.instance.allPercentDamageFromColleague;
				double num4 = Singleton<StatManager>.instance.allPercentDamageFromTreasure + Singleton<StatManager>.instance.percentDamagePM + Singleton<StatManager>.instance.percentDamageAM + Singleton<StatManager>.instance.extraPercentDamageFromConquerorRing;
				damageDetailText.text = I18NManager.Get("WEAPON_STAT") + "\n" + I18NManager.Get("SKIN_BONUS") + "\n" + I18NManager.Get("COLLEAGUE_BONUS") + "\n" + I18NManager.Get("TREASURE_BONUS");
				damageDetailValueText.text = GameManager.changeUnit(weaponDamage) + "\n" + "+" + num2.ToString("N0") + "%\n" + "+" + num3.ToString("N0") + "%\n" + "+" + num4.ToString("N0") + "%";
				double num5 = archerCharacter.attributeBaseStat.criticalChance;
				double secondStatValue = archerCharacter.equippedWeapon.weaponRealStats.secondStatValue;
				double num6 = Singleton<StatManager>.instance.allCriticalChanceFromTreasure;
				secondStatDetailText.text = I18NManager.Get("BASE_CRITICAL_CHANCE") + "\n" + I18NManager.Get("WEAPON_STAT") + "\n" + I18NManager.Get("TREASURE_BONUS");
				secondStatDetailVauleText.text = num5.ToString("N0") + "%\n" + "+" + secondStatValue.ToString("N0") + "%\n" + "+" + num6.ToString("N0") + "%";
				CharacterSkinManager.ArcherSkinType equippedArcherSkin = Singleton<DataManager>.instance.currentGameData.equippedArcherSkin;
				currentCharacterUIObject.initCharacterUIObject(equippedArcherSkin, cachedCanvasGroup);
				bonusStatSetForArcher.SetActive(true);
				bonusDamageTextForArcher.text = "+" + Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType.Archer).ToString("N0") + "%";
				break;
			}
			}
			currentCharacterUIObject.cachedTransform.localScale = new Vector3(120f, 120f, 1f);
			bonusSecondStatImage.SetNativeSize();
			m_currentTargetCharacterType = (CharacterManager.CharacterType)type;
			open();
		}
	}

	public void OnClickStartTranscend()
	{
		long currentTranscendTier = Singleton<DataManager>.instance.currentGameData.currentTranscendTier[m_currentTargetCharacterType];
		if (currentTranscendTier < Singleton<TranscendManager>.instance.getTranscendMaxTier())
		{
			long transcendCost = Singleton<TranscendManager>.instance.getTranscendCost(currentTranscendTier + 1);
			if (Singleton<DataManager>.instance.currentGameData.transcendStone >= transcendCost)
			{
				UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("TRANSCEND_QUESTION"), currentTranscendTier + 1), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowProgressTranscend.instance.startTranscend(m_currentTargetCharacterType, currentTranscendTier);
				}, string.Empty);
				Singleton<DataManager>.instance.saveData();
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_TRANSCEND_STONE_FOR_TIER_UP", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		}
		else
		{
			UIWindowDialog.openDescription("TRANSCEND_MAX", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public override bool OnBeforeClose()
	{
		transcendButtonEffectObject.SetActive(false);
		return base.OnBeforeClose();
	}

	public override void OnAfterActiveGameObject()
	{
		skinScroll.resetContentPosition(Vector2.zero);
		skinScroll.refreshAll(m_currentTargetCharacterType);
	}
}
