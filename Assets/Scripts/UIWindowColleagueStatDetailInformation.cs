using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowColleagueStatDetailInformation : UIWindow
{
	public static UIWindowColleagueStatDetailInformation instance;

	public GameObject[] passiveSkillBlockedObjects;

	public Image[] passiveSkillIconImages;

	public Text[] passiveSkillRequireLevelTexts;

	public Text[] passiveSkillDescriptions;

	private ColleagueManager.ColleagueType m_currentColleagueType;

	private Dictionary<ColleagueManager.PassiveSkillTierType, ColleaguePassiveSkillData> m_currentColleaguePassiveSkillData;

	private ColleagueInventoryData m_currentColleagueInventoryData;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openWithStatDetailDescription(ColleagueManager.ColleagueType colleagueType)
	{
		m_currentColleagueType = colleagueType;
		m_currentColleaguePassiveSkillData = Singleton<ColleagueManager>.instance.getColleaguePassiveSkillData(m_currentColleagueType);
		m_currentColleagueInventoryData = Singleton<ColleagueManager>.instance.getColleagueInventoryData(m_currentColleagueType);
		for (int i = 0; i < 6; i++)
		{
			ColleagueManager.PassiveSkillTierType passiveSkillTierType = (ColleagueManager.PassiveSkillTierType)i;
			ColleaguePassiveSkillData colleaguePassiveSkillData = m_currentColleaguePassiveSkillData[passiveSkillTierType];
			passiveSkillIconImages[i].sprite = Singleton<ColleagueManager>.instance.getColleaguePassiveSkillIconSprite(colleaguePassiveSkillData.passiveType, colleaguePassiveSkillData.passiveTargetType);
			passiveSkillDescriptions[i].text = Singleton<ColleagueManager>.instance.getPassiveSkillDescription(colleaguePassiveSkillData);
			passiveSkillRequireLevelTexts[i].text = "Lv.\n" + Singleton<ColleagueManager>.instance.getColleaguePassiveSkillUnlockLevel((ColleagueManager.PassiveSkillTierType)i);
			switch (passiveSkillTierType)
			{
			case ColleagueManager.PassiveSkillTierType.PassiveSkillTier5:
			{
				Text obj2 = passiveSkillDescriptions[i];
				obj2.text = obj2.text + "\n" + I18NManager.Get("COLLEAGUE_DETAIL_INFORMATION_TIER_5");
				break;
			}
			case ColleagueManager.PassiveSkillTierType.PassiveSkillTier6:
			{
				Text obj = passiveSkillDescriptions[i];
				obj.text = obj.text + "\n" + I18NManager.Get("COLLEAGUE_DETAIL_INFORMATION_TIER_6");
				break;
			}
			}
		}
		for (int j = 0; j < 6; j++)
		{
			if (m_currentColleagueInventoryData.lastPassiveUnlockLevel >= Singleton<ColleagueManager>.instance.getColleaguePassiveSkillUnlockLevel((ColleagueManager.PassiveSkillTierType)j))
			{
				passiveSkillBlockedObjects[j].SetActive(false);
			}
			else
			{
				passiveSkillBlockedObjects[j].SetActive(true);
			}
		}
		open();
	}
}
