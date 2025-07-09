using System;

[Serializable]
public class AchievementData
{
	public AchievementManager.AchievementType currentAchievementType;

	public int currentAchievementLevel;

	public double currentProgressValue;

	public double prevProgressValueBeforeRebirth;

	public bool isComplete;

	public bool isPushNotification;

	public AchievementData(AchievementManager.AchievementType currentAchievementType, int currentAchievementLevel, double currentProgressValue, double prevProgressValueBeforeRebirth, bool isComplete, bool isPushNotification)
	{
		this.currentAchievementType = currentAchievementType;
		this.currentAchievementLevel = currentAchievementLevel;
		this.currentProgressValue = currentProgressValue;
		this.prevProgressValueBeforeRebirth = prevProgressValueBeforeRebirth;
		this.isComplete = isComplete;
		this.isPushNotification = isPushNotification;
	}
}
