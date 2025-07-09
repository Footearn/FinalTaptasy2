using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowColleagueInformation : UIWindow
{
	public static UIWindowColleagueInformation instance;

	public Text totalDamageText;

	public Text damageDetailText;

	public Text damageDetailValueText;

	public Text nameText;

	public Text currentLevelText;

	public Text skinExtraDamageText;

	public ColleagueManager.ColleagueType currentColleagueType;

	public CanvasGroup cachedCanvasGroup;

	public RectTransform colleagueUIObjectSpawnTargetParentTransform;

	public GameObject[] passiveSkillBlockedObjects;

	public Image[] passiveSkillIconImages;

	public Text[] passiveSkillRequireLevelTexts;

	public OptimizedScrollRect colleagueSkinParentScrollRect;

	public ColleagueUIObject currentColleagueUIObject;

	public Text bonusStatDescriptionText;

	public SpriteMask spriteMask;

	public List<ColleagueManager.ColleagueType> newSkinColleagueList = new List<ColleagueManager.ColleagueType>();

	private ColleagueInventoryData m_currentColleagueInventoryData;

	private Dictionary<ColleagueManager.PassiveSkillTierType, ColleaguePassiveSkillData> m_currentColleaguePassiveSkillData;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeClose()
	{
		for (int i = 0; i < colleagueSkinParentScrollRect.slotObjects.Count; i++)
		{
			(colleagueSkinParentScrollRect.slotObjects[i] as ColleagueSkinSlot).currentColleagueUIObject.followAlphaWithCanvas(cachedCanvasGroup);
		}
		return base.OnBeforeClose();
	}

	public void openWithColleagueInformation(ColleagueManager.ColleagueType colleagueType)
	{
		colleagueSkinParentScrollRect.horizontal = true;
		if (!Singleton<DataManager>.instance.currentGameData.isSeenColleagueSkinDescription)
		{
			Singleton<DataManager>.instance.currentGameData.isSeenColleagueSkinDescription = true;
			UIWindowColleague.instance.refreshSkinDescriptionObject();
			Singleton<DataManager>.instance.saveData();
		}
		if (currentColleagueUIObject != null)
		{
			currentColleagueUIObject.followAlphaWithCanvas(cachedCanvasGroup);
		}
		bonusStatDescriptionText.text = string.Format(I18NManager.Get("CHARACTER_SKIN_BONUS_DESCRIPTION"), I18NManager.Get("ALL_COLLEAGUE"));
		currentColleagueType = colleagueType;
		m_currentColleaguePassiveSkillData = Singleton<ColleagueManager>.instance.getColleaguePassiveSkillData(currentColleagueType);
		m_currentColleagueInventoryData = Singleton<ColleagueManager>.instance.getColleagueInventoryData(currentColleagueType);
		for (int i = 0; i < colleagueSkinParentScrollRect.slotObjects.Count; i++)
		{
			ObjectPool.Recycle(colleagueSkinParentScrollRect.slotObjects[i].name, colleagueSkinParentScrollRect.slotObjects[i].cachedGameObject);
		}
		colleagueSkinParentScrollRect.slotObjects.Clear();
		float num = 40.19994f;
		Dictionary<int, CharacterManager.CharacterBoneAnimationSpriteData> dictionary = Singleton<ColleagueManager>.instance.colleagueBoneSpriteDictionary[colleagueType];
		foreach (KeyValuePair<int, CharacterManager.CharacterBoneAnimationSpriteData> item in dictionary)
		{
			ColleagueSkinSlot component = ObjectPool.Spawn("@ColleagueSkinSlot", Vector2.zero, colleagueSkinParentScrollRect.content).GetComponent<ColleagueSkinSlot>();
			component.cachedRectTransform.anchoredPosition = new Vector2(num, -2f);
			component.parentScrollRect = colleagueSkinParentScrollRect;
			colleagueSkinParentScrollRect.slotObjects.Add(component);
			component.initSlot(m_currentColleagueInventoryData.colleagueType, item.Key);
			num += 199.200058f;
		}
		colleagueSkinParentScrollRect.content.sizeDelta = new Vector2(num, 376f);
		refreshInformation();
		spriteMask.updateSprites();
		open();
	}

	public void refreshInformation()
	{
		for (int i = 0; i < 6; i++)
		{
			ColleaguePassiveSkillData colleaguePassiveSkillData = m_currentColleaguePassiveSkillData[(ColleagueManager.PassiveSkillTierType)i];
			passiveSkillIconImages[i].sprite = Singleton<ColleagueManager>.instance.getColleaguePassiveSkillIconSprite(colleaguePassiveSkillData.passiveType, colleaguePassiveSkillData.passiveTargetType);
			passiveSkillRequireLevelTexts[i].text = "Lv.\n" + Singleton<ColleagueManager>.instance.getColleaguePassiveSkillUnlockLevel((ColleagueManager.PassiveSkillTierType)i);
		}
		for (int j = 0; j < 6; j++)
		{
			if (m_currentColleagueInventoryData.level >= Singleton<ColleagueManager>.instance.getColleaguePassiveSkillUnlockLevel((ColleagueManager.PassiveSkillTierType)j))
			{
				passiveSkillBlockedObjects[j].SetActive(false);
			}
			else
			{
				passiveSkillBlockedObjects[j].SetActive(true);
			}
		}
		if (currentColleagueUIObject != null)
		{
			ObjectPool.Recycle(currentColleagueUIObject.name, currentColleagueUIObject.cachedGameObject);
		}
		currentColleagueUIObject = null;
		currentColleagueUIObject = ObjectPool.Spawn("@ColleagueUIObject" + (int)(currentColleagueType + 1), Vector2.zero, colleagueUIObjectSpawnTargetParentTransform).GetComponent<ColleagueUIObject>();
		currentColleagueUIObject.cachedTransform.localPosition = new Vector2(-183.4f, 237.5f);
		currentColleagueUIObject.cachedTransform.localScale = new Vector3(150f, 150f, 1f);
		currentColleagueUIObject.initColleagueUI(currentColleagueType, m_currentColleagueInventoryData.currentEquippedSkinIndex);
		currentColleagueUIObject.followAlphaWithCanvas(cachedCanvasGroup);
		currentColleagueUIObject.changeLayer("PopUpLayer");
		Singleton<StatManager>.instance.refreshAllStats();
		m_currentColleagueInventoryData = Singleton<ColleagueManager>.instance.getColleagueInventoryData(currentColleagueType);
		m_currentColleaguePassiveSkillData = Singleton<ColleagueManager>.instance.getColleaguePassiveSkillData(currentColleagueType);
		nameText.text = Singleton<ColleagueManager>.instance.getColleagueI18NName(currentColleagueType, m_currentColleagueInventoryData.currentEquippedSkinIndex);
		currentLevelText.text = "Lv." + m_currentColleagueInventoryData.level;
		totalDamageText.text = GameManager.changeUnit(Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, 1f)) + " <size=23><color=#FAD725>" + I18NManager.Get("COLLEAGUE_DAMAGE_PER_SECOND") + "</color></size>";
		double colleagueDamage = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, false, 1f);
		double num = Singleton<StatManager>.instance.colleagueSelfPercentDamage[(int)currentColleagueType];
		double colleagueAllPercentDamageFromColleagueSkin = Singleton<StatManager>.instance.colleagueAllPercentDamageFromColleagueSkin;
		double allPercentDamageFromColleague = Singleton<StatManager>.instance.allPercentDamageFromColleague;
		double num2 = Singleton<StatManager>.instance.allPercentDamageFromTreasure + Singleton<StatManager>.instance.percentDamagePM + Singleton<StatManager>.instance.percentDamageAM + Singleton<StatManager>.instance.extraPercentDamageFromNobleBlade;
		damageDetailText.text = I18NManager.Get("COLLEAGUE_INFORMATION_BASE_DAMAGE") + "\n" + I18NManager.Get("COLLEAGUE_INFORMATION_SELF_DAMAGE") + "\n" + I18NManager.Get("SKIN_BONUS") + "\n" + I18NManager.Get("COLLEAGUE_INFORMATION_ALL_DAMAGE") + "\n" + I18NManager.Get("TREASURE_BONUS");
		damageDetailValueText.text = GameManager.changeUnit(colleagueDamage) + "\n" + "+" + num + "%\n" + "+" + colleagueAllPercentDamageFromColleagueSkin + "%\n" + "+" + allPercentDamageFromColleague + "%\n" + "+" + num2 + "%";
		double num3 = 0.0;
		foreach (KeyValuePair<int, bool> colleagueSkinDatum in m_currentColleagueInventoryData.colleagueSkinData)
		{
			if (colleagueSkinDatum.Value)
			{
				num3 += Singleton<ColleagueManager>.instance.getColleagueSkinStat(currentColleagueType, colleagueSkinDatum.Key);
			}
		}
		skinExtraDamageText.text = string.Format(I18NManager.Get("ALL_COLLEAGUE_DAMAGE_DESCRIPTION"), num3);
		UIWindowColleague.instance.refreshTotalDamage();
	}

	public void OnClickOpenDetailStat()
	{
		UIWindowColleagueStatDetailInformation.instance.openWithStatDetailDescription(currentColleagueType);
	}
}
