using System;

[Serializable]
public struct ActiveSkillStatData
{
	public SkillManager.SkillType currentSkillType;

	public float basePercentValue;

	public float increasePercentValue;

	public int manaCost;
}
