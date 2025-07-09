using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowComplete : MonoBehaviour
{
	[Serializable]
	public struct CompleteData
	{
		public CompleteType currentCompleteType;

		public AchievementManager.AchievementType currentAchieveType;

		public QuestData currentQuestData;

		public CompleteData(AchievementManager.AchievementType currentAchieveType, CompleteType currentCompleteType, QuestData currentQuestData)
		{
			this.currentAchieveType = currentAchieveType;
			this.currentCompleteType = currentCompleteType;
			this.currentQuestData = currentQuestData;
		}
	}

	public enum CompleteType
	{
		Achievement,
		Quest
	}

	public static UIWindowComplete instance;

	public Animation openPopupAnimation;

	public RectTransform openPopupTransform;

	public GameObject completeObjects;

	public Text nameText;

	public GameObject questCompleteObjects;

	public Image questIconImage;

	public Text questDescriptionText;

	private List<CompleteData> m_stackedCompletedData;

	private void Awake()
	{
		m_stackedCompletedData = new List<CompleteData>();
		instance = this;
		StartCoroutine("achievementUpdate");
	}

	private IEnumerator achievementUpdate()
	{
		while (true)
		{
			if (m_stackedCompletedData.Count > 0)
			{
				if (!openPopupAnimation.isPlaying)
				{
					refreshAchievementInformation(m_stackedCompletedData[0]);
					m_stackedCompletedData.Remove(m_stackedCompletedData[0]);
					openPopupAnimation.Play();
					Singleton<AudioManager>.instance.playEffectSound("questcomplete");
					yield return null;
				}
			}
			else if (!openPopupAnimation.isPlaying && openPopupTransform.localScale.y != 0f)
			{
				openPopupTransform.localScale = new Vector3(1f, 0f, 1f);
			}
			yield return null;
		}
	}

	private void refreshAchievementInformation(CompleteData completeData)
	{
		if (completeData.currentCompleteType == CompleteType.Achievement)
		{
			completeObjects.SetActive(true);
			questCompleteObjects.SetActive(false);
			AchievementData achievementDataFromInventory = Singleton<AchievementManager>.instance.getAchievementDataFromInventory(completeData.currentAchieveType);
			double achievementMaxProgressValue = Singleton<AchievementManager>.instance.getAchievementMaxProgressValue(achievementDataFromInventory.currentAchievementType, achievementDataFromInventory.currentAchievementLevel);
			nameText.text = Singleton<AchievementManager>.instance.getAchievementI18NName(achievementDataFromInventory.currentAchievementType);
			AchievementData achievementDataFromInventory2 = Singleton<AchievementManager>.instance.getAchievementDataFromInventory(completeData.currentAchieveType);
			return;
		}
		completeObjects.SetActive(false);
		questCompleteObjects.SetActive(true);
		questIconImage.sprite = Singleton<QuestManager>.instance.questIconSprites[(int)completeData.currentQuestData.questType];
		QuestManager.QuestType questType = completeData.currentQuestData.questType;
		if (questType == QuestManager.QuestType.NextStage)
		{
			int num = Mathf.FloorToInt((float)completeData.currentQuestData.questLevel / 100f);
			int num2 = (int)((float)completeData.currentQuestData.questLevel % 100f);
			string arg = num + "-" + num2;
			questDescriptionText.text = string.Format(I18NManager.Get("QUEST_DESCRIPTION_" + (int)completeData.currentQuestData.questType), arg);
		}
		else
		{
			questDescriptionText.text = string.Format(I18NManager.Get("QUEST_DESCRIPTION_" + (int)completeData.currentQuestData.questType), (completeData.currentQuestData.questType != QuestManager.QuestType.Gold) ? completeData.currentQuestData.questGoal.ToString("N0") : GameManager.changeUnit(completeData.currentQuestData.questGoal));
		}
		questIconImage.SetNativeSize();
	}

	public void registryComplete(AchievementManager.AchievementType achievementType)
	{
		m_stackedCompletedData.Add(new CompleteData(achievementType, CompleteType.Achievement, null));
	}

	public void registryComplete(QuestData questData)
	{
		m_stackedCompletedData.Add(new CompleteData(AchievementManager.AchievementType.Length, CompleteType.Quest, questData));
	}
}
