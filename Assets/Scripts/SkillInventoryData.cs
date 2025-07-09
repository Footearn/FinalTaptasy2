using System;

[Serializable]
public class SkillInventoryData
{
	public int skillLevel;

	public SkillManager.SkillType skillType;

	public bool isNewSkill;

	public bool isHasSkill;

	public SkillInventoryData()
	{
		skillLevel = 1;
		skillType = SkillManager.SkillType.None;
		isNewSkill = false;
		isHasSkill = false;
	}
}
