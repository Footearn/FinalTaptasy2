using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : Singleton<AchievementManager>
{
	public enum AchievementType
	{
		FirstStep,
		ProHunter,
		Millionaire,
		FindingTreasures,
		CorrectSkillUsage,
		WeaponCollector,
		EnchantingHowTo,
		HeroCollector,
		Companionship,
		SkillfulStrike,
		TreasurePro,
		AdsAngel,
		QuestMania,
		CanNotStop,
		ProGambler,
		TraceOfTravels,
		Reborn,
		PieceOfCakeKillDemonKing,
		Length
	}

	public struct RewardData
	{
		public AchievementType currentAchievementType;

		public RewardType currentRewardType;

		public long rewardValue;

		public RewardData(AchievementType currentAchievementType, RewardType currentRewardType, long rewardValue)
		{
			this.currentAchievementType = currentAchievementType;
			this.currentRewardType = currentRewardType;
			this.rewardValue = rewardValue;
		}
	}

	public enum RewardType
	{
		Ruby
	}

	private Dictionary<AchievementType, List<string>> m_achievementIDDictionary = new Dictionary<AchievementType, List<string>>();

	private void Awake()
	{
		initAchievementID();
	}

	private void initAchievementID()
	{
		m_achievementIDDictionary.Add(AchievementType.FirstStep, new List<string>
		{
			"CgkIgP-i6oAVEAIQAQ"
		});
		m_achievementIDDictionary.Add(AchievementType.ProHunter, new List<string>
		{
			"CgkIgP-i6oAVEAIQAg",
			"CgkIgP-i6oAVEAIQAw",
			"CgkIgP-i6oAVEAIQBA"
		});
		m_achievementIDDictionary.Add(AchievementType.Millionaire, new List<string>
		{
			"CgkIgP-i6oAVEAIQBQ",
			"CgkIgP-i6oAVEAIQBw",
			"CgkIgP-i6oAVEAIQCA"
		});
		m_achievementIDDictionary.Add(AchievementType.FindingTreasures, new List<string>
		{
			"CgkIgP-i6oAVEAIQCQ",
			"CgkIgP-i6oAVEAIQCg",
			"CgkIgP-i6oAVEAIQCw"
		});
		m_achievementIDDictionary.Add(AchievementType.CorrectSkillUsage, new List<string>
		{
			"CgkIgP-i6oAVEAIQDA",
			"CgkIgP-i6oAVEAIQDQ",
			"CgkIgP-i6oAVEAIQDg"
		});
		m_achievementIDDictionary.Add(AchievementType.WeaponCollector, new List<string>
		{
			"CgkIgP-i6oAVEAIQDw",
			"CgkIgP-i6oAVEAIQEA",
			"CgkIgP-i6oAVEAIQEQ"
		});
		m_achievementIDDictionary.Add(AchievementType.EnchantingHowTo, new List<string>
		{
			"CgkIgP-i6oAVEAIQEg",
			"CgkIgP-i6oAVEAIQEw",
			"CgkIgP-i6oAVEAIQFA"
		});
		m_achievementIDDictionary.Add(AchievementType.HeroCollector, new List<string>
		{
			"CgkIgP-i6oAVEAIQFQ",
			"CgkIgP-i6oAVEAIQFg",
			"CgkIgP-i6oAVEAIQFw"
		});
		m_achievementIDDictionary.Add(AchievementType.Companionship, new List<string>
		{
			"CgkIgP-i6oAVEAIQGA",
			"CgkIgP-i6oAVEAIQGQ",
			"CgkIgP-i6oAVEAIQGg"
		});
		m_achievementIDDictionary.Add(AchievementType.SkillfulStrike, new List<string>
		{
			"CgkIgP-i6oAVEAIQGw",
			"CgkIgP-i6oAVEAIQHA",
			"CgkIgP-i6oAVEAIQHQ"
		});
		m_achievementIDDictionary.Add(AchievementType.TreasurePro, new List<string>
		{
			"CgkIgP-i6oAVEAIQHg",
			"CgkIgP-i6oAVEAIQHw",
			"CgkIgP-i6oAVEAIQIA"
		});
		m_achievementIDDictionary.Add(AchievementType.AdsAngel, new List<string>
		{
			"CgkIgP-i6oAVEAIQIQ",
			"CgkIgP-i6oAVEAIQIg",
			"CgkIgP-i6oAVEAIQIw"
		});
		m_achievementIDDictionary.Add(AchievementType.QuestMania, new List<string>
		{
			"CgkIgP-i6oAVEAIQJA",
			"CgkIgP-i6oAVEAIQJQ",
			"CgkIgP-i6oAVEAIQJg"
		});
		m_achievementIDDictionary.Add(AchievementType.CanNotStop, new List<string>
		{
			"CgkIgP-i6oAVEAIQJw",
			"CgkIgP-i6oAVEAIQKA",
			"CgkIgP-i6oAVEAIQKQ"
		});
		m_achievementIDDictionary.Add(AchievementType.ProGambler, new List<string>
		{
			"CgkIgP-i6oAVEAIQKg",
			"CgkIgP-i6oAVEAIQKw",
			"CgkIgP-i6oAVEAIQLA"
		});
		m_achievementIDDictionary.Add(AchievementType.TraceOfTravels, new List<string>
		{
			"CgkIgP-i6oAVEAIQLQ",
			"CgkIgP-i6oAVEAIQLg",
			"CgkIgP-i6oAVEAIQLw"
		});
		m_achievementIDDictionary.Add(AchievementType.Reborn, new List<string>
		{
			"CgkIgP-i6oAVEAIQMA",
			"CgkIgP-i6oAVEAIQMQ",
			"CgkIgP-i6oAVEAIQMg"
		});
		m_achievementIDDictionary.Add(AchievementType.PieceOfCakeKillDemonKing, new List<string>
		{
			"CgkIgP-i6oAVEAIQMw",
			"CgkIgP-i6oAVEAIQNA",
			"CgkIgP-i6oAVEAIQNQ"
		});
	}

	public int getSumCurrentAllAchievementLevel()
	{
		int num = 0;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.achievementData.Count; i++)
		{
			num = ((!Singleton<DataManager>.instance.currentGameData.achievementData[i].isComplete) ? (num + (Singleton<DataManager>.instance.currentGameData.achievementData[i].currentAchievementLevel - 1)) : (num + Singleton<DataManager>.instance.currentGameData.achievementData[i].currentAchievementLevel));
		}
		return num;
	}

	public int getSumMaxAllAchievementLevel()
	{
		int num = 0;
		for (int i = 0; i < 18; i++)
		{
			num += getAchievementMaxLevel((AchievementType)i);
		}
		return num;
	}

	public bool isMaxLevelAchievement(AchievementType achievementType)
	{
		bool result = false;
		AchievementData achievementDataFromInventory = getAchievementDataFromInventory(achievementType);
		if (isOnlyLevelOneAchievement(achievementDataFromInventory.currentAchievementType))
		{
			result = true;
		}
		else if (achievementDataFromInventory.currentAchievementLevel >= 3)
		{
			result = true;
		}
		return result;
	}

	public bool isCompleteAchievement(AchievementType achievementType)
	{
		return getAchievementDataFromInventory(achievementType).isComplete;
	}

	public bool isCanObtainReward(AchievementType achievementType)
	{
		bool result = false;
		AchievementData achievementDataFromInventory = getAchievementDataFromInventory(achievementType);
		if (achievementDataFromInventory.currentProgressValue >= getAchievementMaxProgressValue(achievementDataFromInventory.currentAchievementType, achievementDataFromInventory.currentAchievementLevel) && !achievementDataFromInventory.isComplete)
		{
			result = true;
		}
		return result;
	}

	public bool isOnlyLevelOneAchievement(AchievementType achievementType)
	{
		bool result = false;
		if (getAchievementMaxLevel(achievementType) == 1)
		{
			result = true;
		}
		return result;
	}

	public RewardData getRewardData(AchievementType achievementType, int currentAchievementLevel)
	{
		long rewardValue = 0L;
		RewardType currentRewardType = RewardType.Ruby;
		switch (currentAchievementLevel)
		{
		case 1:
			rewardValue = Singleton<ParsingManager>.instance.currentParsedStatData.achievementStatData[achievementType].rewardValueLevelOne;
			break;
		case 2:
			rewardValue = Singleton<ParsingManager>.instance.currentParsedStatData.achievementStatData[achievementType].rewardValueLevelTwo;
			break;
		case 3:
			rewardValue = Singleton<ParsingManager>.instance.currentParsedStatData.achievementStatData[achievementType].rewardValueLevelThree;
			break;
		}
		long rewardType = Singleton<ParsingManager>.instance.currentParsedStatData.achievementStatData[achievementType].rewardType;
		if (rewardType == 1)
		{
			currentRewardType = RewardType.Ruby;
		}
		return new RewardData(achievementType, currentRewardType, rewardValue);
	}

	public double getAchievementMaxProgressValue(AchievementType achievementType, int currentAchievementLevel)
	{
		if (!Singleton<ParsingManager>.instance.currentParsedStatData.achievementStatData.ContainsKey(achievementType))
		{
			return 0.0;
		}
		double result = 0.0;
		switch (currentAchievementLevel)
		{
		case 1:
			result = Singleton<ParsingManager>.instance.currentParsedStatData.achievementStatData[achievementType].achievementLevelOneGoalValue;
			break;
		case 2:
			result = Singleton<ParsingManager>.instance.currentParsedStatData.achievementStatData[achievementType].achievementLevelTwoGoalValue;
			break;
		case 3:
			result = Singleton<ParsingManager>.instance.currentParsedStatData.achievementStatData[achievementType].achievementLevelThreeGoalValue;
			break;
		}
		return result;
	}

	public int getAchievementMaxLevel(AchievementType achievementType)
	{
		int num = 0;
		return Singleton<ParsingManager>.instance.currentParsedStatData.achievementStatData[achievementType].achievementMaxLevel;
	}

	public AchievementData getAchievementDataFromInventory(AchievementType achievementType)
	{
		return Singleton<DataManager>.instance.currentGameData.achievementData[(int)achievementType];
	}

	public string getAchievementI18NName(AchievementType achievementType)
	{
		return I18NManager.Get("ACHIEVEMENT_NAME_" + (int)(achievementType + 1));
	}

	public string getAchievementI18NDescriptionTitleID(AchievementType achievementType)
	{
		return "ACHIEVEMENT_DESCRIPTION_" + (int)(achievementType + 1);
	}

	public void increaseAchievementValue(AchievementType achievementType, double increaseValue)
	{
		AchievementData achievementDataFromInventory = getAchievementDataFromInventory(achievementType);
		if (achievementDataFromInventory.prevProgressValueBeforeRebirth >= achievementDataFromInventory.currentProgressValue)
		{
			if (!achievementDataFromInventory.isComplete)
			{
				double achievementMaxProgressValue = getAchievementMaxProgressValue(achievementType, achievementDataFromInventory.currentAchievementLevel);
				achievementDataFromInventory.currentProgressValue += increaseValue;
				achievementDataFromInventory.prevProgressValueBeforeRebirth += increaseValue;
				if (isCanObtainReward(achievementType) && !achievementDataFromInventory.isPushNotification)
				{
					achievementDataFromInventory.isPushNotification = true;
					openCompleteAchievementUI(achievementType);
				}
			}
		}
		else
		{
			achievementDataFromInventory.prevProgressValueBeforeRebirth += increaseValue;
		}
	}

	public void openCompleteAchievementUI(AchievementType achievementType)
	{
		UIWindowOutgame.instance.refreshAchievementCompleteIndicator();
		UIWindowComplete.instance.registryComplete(achievementType);
	}

	public bool rewardEvent(AchievementType achievementType)
	{
		bool result = false;
		AchievementData achievementDataFromInventory = getAchievementDataFromInventory(achievementType);
		if (!isCompleteAchievement(achievementType) && isCanObtainReward(achievementType))
		{
			RewardData rewardData = getRewardData(achievementType, achievementDataFromInventory.currentAchievementLevel);
			if (rewardData.currentRewardType == RewardType.Ruby)
			{
				Singleton<RubyManager>.instance.increaseRuby(rewardData.rewardValue, true);
				if (UIWindowManageShop.instance != null)
				{
					UIWindowManageShop.instance.refreshSlots();
				}
			}
			string achievementID = getAchievementID(achievementType, achievementDataFromInventory.currentAchievementLevel);
			if (!string.IsNullOrEmpty(achievementID))
			{
				Social.ReportProgress(achievementID, 100.0, delegate
				{
				});
			}
			if (achievementDataFromInventory.currentAchievementLevel < getAchievementMaxLevel(achievementType))
			{
				achievementDataFromInventory.isPushNotification = false;
				achievementDataFromInventory.currentAchievementLevel++;
			}
			else
			{
				achievementDataFromInventory.isComplete = true;
			}
			Singleton<AudioManager>.instance.playEffectSound("questcomplete");
			Singleton<DataManager>.instance.saveData();
			result = true;
		}
		UIWindowOutgame.instance.refreshAchievementCompleteIndicator();
		return result;
	}

	private string getAchievementID(AchievementType achievementType, int level)
	{
		return m_achievementIDDictionary[achievementType][level - 1];
	}
}
