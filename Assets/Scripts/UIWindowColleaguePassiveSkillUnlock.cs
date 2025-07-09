using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowColleaguePassiveSkillUnlock : UIWindow
{
	public static UIWindowColleaguePassiveSkillUnlock instance;

	public Transform sunBurstTransform;

	public Image passiveSkillIconImage;

	public Text descriptionText;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openPassiveSkillUnlockUI(ColleagueManager.ColleagueType colleagueType, ColleagueManager.PassiveSkillTierType targetTier, ColleaguePassiveSkillData passiveSkillData)
	{
		StopAllCoroutines();
		sunBurstTransform.localScale = Vector3.one;
		passiveSkillIconImage.sprite = Singleton<ColleagueManager>.instance.getColleaguePassiveSkillIconSprite(passiveSkillData.passiveType, passiveSkillData.passiveTargetType);
		if (passiveSkillData.passiveTargetType == ColleagueManager.ColleaguePassiveTargetType.All)
		{
			UIWindowColleague.instance.colleagueScrollRectParent.refreshAll();
		}
		descriptionText.text = Singleton<ColleagueManager>.instance.getPassiveSkillDescription(passiveSkillData, Singleton<ParsingManager>.instance.currentParsedStatData.colleagueEffectData[colleagueType][targetTier].passiveValue);
		open();
	}

	public void OnClickClose()
	{
		StartCoroutine("sunBurstScaleUpdate");
		close();
	}

	private IEnumerator sunBurstScaleUpdate()
	{
		while (true)
		{
			sunBurstTransform.localScale = Vector3.Lerp(sunBurstTransform.localScale, new Vector3(0f, 0f, 0f), Time.deltaTime * 15f);
			yield return null;
		}
	}
}
