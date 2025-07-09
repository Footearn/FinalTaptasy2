using System;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowPVPSimpleSkillInformation : UIWindow
{
	public static UIWindowPVPSimpleSkillInformation instance;

	public Text descriptionText;

	public Image skillIconImage;

	public Image skillGradeImage;

	private Action m_closeAction;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openSkillInformation(PVPSkillManager.PVPSkillTypeData skillType, Action closeAction = null)
	{
		m_closeAction = closeAction;
		descriptionText.text = Singleton<PVPSkillManager>.instance.getSkillDescription(skillType);
		skillIconImage.sprite = Singleton<PVPSkillManager>.instance.getSkillIconSprite(skillType);
		skillGradeImage.sprite = Singleton<PVPSkillManager>.instance.getSkillGradeIconSprite(skillType);
		open();
	}

	public override bool OnBeforeClose()
	{
		if (m_closeAction != null)
		{
			m_closeAction();
		}
		return base.OnBeforeClose();
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(0) || Input.touchCount <= 0)
		{
			close();
		}
	}
}
