using UnityEngine.EventSystems;

public class TowerModeSkillButton : Button
{
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		UIWindowTowerMode.instance.isPressedSkillButton = true;
		UIWindowTowerMode.instance.OnClickCastSkill();
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		UIWindowTowerMode.instance.isPressedSkillButton = false;
	}
}
