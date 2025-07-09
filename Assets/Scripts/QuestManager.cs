using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
	public enum QuestType
	{
		None,
		MonsterHunt,
		Gold,
		PlayTime,
		Enchant,
		TotalTreasureBox,
		NextStage,
		Floor,
		Skill,
		Length
	}

	public enum RewardType
	{
		Gold,
		Ruby
	}

	public List<QuestData> questData = new List<QuestData>();

	public Sprite[] questIconSprites;

	public Sprite[] rewardSprites;

	public bool isAnimating;

	public void Start()
	{
		setDefaultQuest();
	}

	public void setDefaultQuest()
	{
		if (questData == null || questData.Count == 0 || Singleton<DataManager>.instance.currentGameData.questData == null || Singleton<DataManager>.instance.currentGameData.questData.Count == 0)
		{
			questData = new List<QuestData>();
			questData.Add(getQuest(0, QuestType.NextStage));
			questData.Add(getQuest(1, QuestType.MonsterHunt));
			questData.Add(getQuest(2, QuestType.PlayTime));
			if (questData.Count == 3)
			{
				saveQuest();
			}
		}
		else
		{
			questData = Singleton<DataManager>.instance.currentGameData.questData;
		}
	}

	public void saveQuest()
	{
		Singleton<DataManager>.instance.currentGameData.questData = questData;
		Singleton<DataManager>.instance.saveData();
	}

	public MonsterApearInformation getMonsterEmergeInformation(int level)
	{
		int num = level;
		int num2 = 1;
		int num3 = 1;
		while (num > 1)
		{
			num--;
			num3++;
			if (num3 > 5)
			{
				num3 = 1;
				num2++;
			}
		}
		bool flag = false;
		if (level % 5 == 4)
		{
			flag = true;
		}
		return new MonsterApearInformation(num2, (!flag) ? num3 : 4);
	}

	public QuestData getQuest(int idx, QuestType targetQuestType = QuestType.Length)
	{
		QuestType questType = QuestType.Length;
		questType = (QuestType)((targetQuestType != QuestType.Length) ? ((int)targetQuestType) : Random.Range(1, 9));
		int num = 0;
		int num2 = 0;
		bool flag = true;
		switch (questType)
		{
		case QuestType.MonsterHunt:
			num = 10;
			break;
		case QuestType.Gold:
			num = Random.Range(0, 26) + 1;
			break;
		case QuestType.PlayTime:
		case QuestType.Enchant:
		case QuestType.TotalTreasureBox:
		case QuestType.Skill:
			num = Random.Range(0, 2) + 1;
			break;
		case QuestType.NextStage:
		{
			int num3 = 1;
			num2 = Singleton<DataManager>.instance.currentGameData.unlockTheme;
			num3 = Singleton<DataManager>.instance.currentGameData.unlockStage;
			if (num3 >= 10)
			{
				num2++;
				num3 = 1;
			}
			else
			{
				num3++;
			}
			num = num2 * 100 + num3;
			break;
		}
		case QuestType.Floor:
			num = 1;
			break;
		}
		double questGoal = getQuestGoal(questType, num);
		QuestData result = new QuestData(questType, num, 0.0, questGoal, getRewardType(questType), getRewardValue(questType, num, questGoal), false);
		for (int i = 0; i < questData.Count; i++)
		{
			if (questType == questData[i].questType || (questType == questData[i].questType && num == questData[i].questLevel) || !flag)
			{
				result = getQuest(idx);
			}
		}
		return result;
	}

	public double getQuestGoal(QuestData quest)
	{
		return getQuestGoal(quest.questType, quest.questLevel);
	}

	public double getQuestGoal(QuestType quest, int level)
	{
		double result = 0.0;
		switch (quest)
		{
		case QuestType.MonsterHunt:
			result = (long)((float)(Singleton<MapManager>.instance.getMaxFloor(Singleton<DataManager>.instance.currentGameData.currentTheme) * 5) * 2.5f);
			break;
		case QuestType.Gold:
			result = CalculateManager.getGoldValueForMonsters() * 300.0;
			break;
		case QuestType.PlayTime:
			result = 5.0;
			break;
		case QuestType.Enchant:
			result = 3.0;
			break;
		case QuestType.TotalTreasureBox:
			result = 15.0;
			break;
		case QuestType.NextStage:
			result = 1.0;
			break;
		case QuestType.Floor:
			result = (long)((float)Singleton<MapManager>.instance.getMaxFloor(Singleton<DataManager>.instance.currentGameData.currentTheme) * 1.5f);
			break;
		case QuestType.Skill:
			result = 10.0;
			break;
		}
		return result;
	}

	public RewardType getRewardType(QuestType questType)
	{
		return RewardType.Gold;
	}

	public double getRewardValue(QuestData quest)
	{
		return getRewardValue(quest.questType, quest.questLevel, quest.questGoal);
	}

	public double getRewardValue(QuestType quest, int level, double goal)
	{
		double num = CalculateManager.getCurrentStandardGold() * 15.0;
		switch (quest)
		{
		case QuestType.MonsterHunt:
			num *= 1.7000000476837158;
			break;
		case QuestType.Gold:
			num *= 1.5;
			break;
		case QuestType.PlayTime:
			num *= 1.6000000238418579;
			break;
		case QuestType.Enchant:
		{
			int num2 = level;
			if (num2 == 2)
			{
				num *= 1.2000000476837158;
			}
			break;
		}
		case QuestType.TotalTreasureBox:
			switch (level)
			{
			case 1:
				num *= 1.3999999761581421;
				break;
			case 2:
				num *= 1.3999999761581421;
				break;
			}
			break;
		case QuestType.NextStage:
			num *= 1.1000000238418579;
			break;
		case QuestType.Floor:
			num *= 1.2999999523162842;
			break;
		case QuestType.Skill:
		{
			int num2 = level;
			if (num2 == 2)
			{
				num *= 1.0;
			}
			break;
		}
		}
		return num;
	}

	public int getLevelforMonsterhunt(int type)
	{
		return Mathf.FloorToInt(type / 4) + 1;
	}

	public void questReportForStage(QuestType questType, int theme, int stage)
	{
		if (BossRaidManager.isBossRaid)
		{
			return;
		}
		for (int i = 0; i < questData.Count; i++)
		{
			int num = Mathf.FloorToInt((float)questData[i].questLevel / 100f);
			int num2 = (int)((float)questData[i].questLevel % 100f);
			if (questType != questData[i].questType || num != theme || num2 != stage)
			{
				continue;
			}
			questData[i].questValue += 1.0;
			if (!questData[i].isComplete)
			{
				if (questData[i].questValue >= questData[i].questGoal)
				{
					questData[i].isComplete = true;
					questData[i].questValue = questData[i].questGoal;
					UIWindowComplete.instance.registryComplete(questData[i]);
					UIWindowOutgame.instance.refreshQuestCompleteIndicator();
				}
				if (getQuestIngame(questData[i].questType))
				{
					Singleton<CachedManager>.instance.uiWindowIngame.questIngame.questReport(questData[i]);
				}
			}
		}
	}

	public void questReport(QuestType questType, double value)
	{
		if (BossRaidManager.isBossRaid)
		{
			return;
		}
		for (int i = 0; i < questData.Count; i++)
		{
			if (questType != questData[i].questType)
			{
				continue;
			}
			questData[i].questValue += value;
			if (!questData[i].isComplete)
			{
				if (questData[i].questValue >= questData[i].questGoal)
				{
					questData[i].isComplete = true;
					questData[i].questValue = questData[i].questGoal;
					UIWindowComplete.instance.registryComplete(questData[i]);
					UIWindowOutgame.instance.refreshQuestCompleteIndicator();
				}
				if (getQuestIngame(questData[i].questType))
				{
					Singleton<CachedManager>.instance.uiWindowIngame.questIngame.questReport(questData[i]);
				}
			}
		}
	}

	public void questFail(QuestType questType, int level)
	{
		for (int i = 0; i < questData.Count; i++)
		{
			if (questType == questData[i].questType && level == questData[i].questLevel && questData[i].questValue < questData[i].questGoal)
			{
				questData[i].questValue = 0.0;
			}
		}
	}

	public void questFail(QuestType questType)
	{
		for (int i = 0; i < questData.Count; i++)
		{
			if (questType == questData[i].questType && questData[i].questValue < questData[i].questGoal)
			{
				questData[i].questValue = 0.0;
			}
		}
	}

	public bool getQuestIngame(QuestType questType)
	{
		bool flag = false;
		if (questType != QuestType.Enchant)
		{
		}
		return true;
	}

	public bool isCompleted(int questIdx)
	{
		if (questData[questIdx].questValue >= questData[questIdx].questGoal)
		{
			return true;
		}
		return false;
	}
}
