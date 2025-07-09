using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowPVPInformation : UIWindow
{
	public enum InformationType
	{
		None = -1,
		RankInformation,
		RankReward,
		SkillInformation,
		SeasonReward,
		EntryReward,
		TankInformation,
		DamageInformation,
		Length
	}

	public InformationType currentInformationType = InformationType.None;

	public List<GameObject> informationTypeObjects;

	public List<Text> skillDescriptionTexts;

	public List<Text> tankTotalCountTexts;

	public void openPVPInformationUI(int type)
	{
		for (int i = 0; i < informationTypeObjects.Count; i++)
		{
			if (i != type)
			{
				if (informationTypeObjects[i].activeSelf)
				{
					informationTypeObjects[i].SetActive(false);
				}
			}
			else if (!informationTypeObjects[i].activeSelf)
			{
				informationTypeObjects[i].SetActive(true);
			}
		}
		currentInformationType = (InformationType)type;
		switch (currentInformationType)
		{
		case InformationType.SkillInformation:
		{
			for (int k = 0; k < skillDescriptionTexts.Count; k++)
			{
				string skillDescription = Singleton<PVPSkillManager>.instance.getSkillDescription(Singleton<PVPSkillManager>.instance.convertIndexToSkillType(k));
				skillDescriptionTexts[k].text = skillDescription;
			}
			break;
		}
		case InformationType.TankInformation:
		{
			for (int j = 0; j < tankTotalCountTexts.Count; j++)
			{
				PVPTankData tankData = Singleton<PVPManager>.instance.getTankData(j);
				if ((bool)tankData.isUnlocked)
				{
					tankTotalCountTexts[j].text = "Lv." + tankData.tankLevel.ToString("N0");
				}
				else
				{
					tankTotalCountTexts[j].text = I18NManager.Get("NO_HAVE");
				}
			}
			break;
		}
		}
		open();
	}
}
