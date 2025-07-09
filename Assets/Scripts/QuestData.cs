using System;

[Serializable]
public class QuestData
{
	public QuestManager.QuestType questType;

	public int questLevel;

	public double questValue;

	public double questGoal;

	public QuestManager.RewardType rewardType;

	public double rewardValue;

	public bool isComplete;

	public QuestData(QuestManager.QuestType questType, int questLevel, double questValue, double questGoal, QuestManager.RewardType rewardType, double rewardValue, bool isComplete)
	{
		this.questType = questType;
		this.questLevel = questLevel;
		this.questValue = questValue;
		this.questGoal = questGoal;
		this.rewardType = rewardType;
		this.rewardValue = rewardValue;
		this.isComplete = isComplete;
	}
}
