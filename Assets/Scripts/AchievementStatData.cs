using System;

[Serializable]
public struct AchievementStatData
{
	public AchievementManager.AchievementType currentAchievementType;

	public int achievementMaxLevel;

	public double achievementLevelOneGoalValue;

	public double achievementLevelTwoGoalValue;

	public double achievementLevelThreeGoalValue;

	public long rewardValueLevelOne;

	public long rewardValueLevelTwo;

	public long rewardValueLevelThree;

	public long rewardType;
}
