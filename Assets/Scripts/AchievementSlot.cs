using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSlot : ScrollSlotItem
{
	public AchievementManager.AchievementType currentAchievementType;

	public Text nameText;

	public Text descriptionText;

	public Text progressValue;

	public Image progressBar;

	public Image backgroundImage;

	public Image achievementLevelImage;

	public RectTransform achievementLevelImageTrasnform;

	public Text[] rewardValueText;

	public GameObject completeObject;

	public GameObject notCompleteObject;

	public Image rewardButtonBackgroundImage;

	public Image rewardButtonRubyIconImage;

	public Text rewardButtonTitleText;

	public Transform rewardButtonCenterTransform;

	private AchievementData m_currentAchievementData;

	private AchievementManager.RewardData m_currentRewardData;

	private bool m_isOneLevelAchievement;

	private GameObject m_currentRewardButtonEffect;

	private GameObject m_currentRewardTrophyEffect;

	public override void UpdateItem(int count)
	{
		slotIndex = count;
		if (slotIndex % 2 == 0)
		{
			backgroundImage.color = new Color(0f, 0f, 0f, 0f);
		}
		else
		{
			backgroundImage.color = Util.getCalculatedColor(0f, 11f, 28f, 51f);
		}
		refreshSlot();
	}

	public override void refreshSlot()
	{
		int count = UIWindowManageAchievemant.instance.completedAchievementType.Count;
		if (count <= slotIndex)
		{
			currentAchievementType = UIWindowManageAchievemant.instance.notCompletedAchievementType[slotIndex - count];
		}
		else
		{
			currentAchievementType = UIWindowManageAchievemant.instance.completedAchievementType[slotIndex];
		}
		m_currentAchievementData = Singleton<AchievementManager>.instance.getAchievementDataFromInventory(currentAchievementType);
		m_isOneLevelAchievement = Singleton<AchievementManager>.instance.isOnlyLevelOneAchievement(currentAchievementType);
		m_currentRewardData = Singleton<AchievementManager>.instance.getRewardData(m_currentAchievementData.currentAchievementType, m_currentAchievementData.currentAchievementLevel);
		bool flag = Singleton<AchievementManager>.instance.isCompleteAchievement(currentAchievementType);
		if (flag)
		{
			if (!completeObject.activeSelf)
			{
				completeObject.SetActive(true);
			}
			if (notCompleteObject.activeSelf)
			{
				notCompleteObject.SetActive(false);
			}
		}
		else
		{
			if (completeObject.activeSelf)
			{
				completeObject.SetActive(false);
			}
			if (!notCompleteObject.activeSelf)
			{
				notCompleteObject.SetActive(true);
			}
			if (!Singleton<AchievementManager>.instance.isCanObtainReward(currentAchievementType))
			{
				rewardButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.disableButtonSprite;
				rewardButtonRubyIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				rewardButtonTitleText.color = Util.getCalculatedColor(153f, 153f, 153f);
				for (int i = 0; i < rewardValueText.Length; i++)
				{
					rewardValueText[i].color = Util.getCalculatedColor(153f, 153f, 153f);
				}
			}
			else
			{
				rewardButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.enableButtonOrangeSprite;
				rewardButtonRubyIconImage.color = Color.white;
				rewardButtonTitleText.color = Color.white;
				for (int j = 0; j < rewardValueText.Length; j++)
				{
					rewardValueText[j].color = Color.white;
				}
			}
		}
		double currentProgressValue = m_currentAchievementData.currentProgressValue;
		double achievementMaxProgressValue = Singleton<AchievementManager>.instance.getAchievementMaxProgressValue(m_currentAchievementData.currentAchievementType, m_currentAchievementData.currentAchievementLevel);
		nameText.text = Singleton<AchievementManager>.instance.getAchievementI18NName(m_currentAchievementData.currentAchievementType);
		if (currentAchievementType == AchievementManager.AchievementType.Millionaire)
		{
			descriptionText.text = string.Format(I18NManager.Get(Singleton<AchievementManager>.instance.getAchievementI18NDescriptionTitleID(m_currentAchievementData.currentAchievementType)), GameManager.changeUnit(achievementMaxProgressValue));
		}
		else
		{
			descriptionText.text = string.Format(I18NManager.Get(Singleton<AchievementManager>.instance.getAchievementI18NDescriptionTitleID(m_currentAchievementData.currentAchievementType)), achievementMaxProgressValue);
		}
		if (currentAchievementType == AchievementManager.AchievementType.Reborn && I18NManager.currentLanguage == I18NManager.Language.English && achievementMaxProgressValue == 1.0)
		{
			descriptionText.text = descriptionText.text.Trim();
			descriptionText.text = descriptionText.text.Substring(0, descriptionText.text.Length - 1);
		}
		if (currentAchievementType == AchievementManager.AchievementType.Millionaire)
		{
			progressValue.text = GameManager.changeUnit(Math.Min(currentProgressValue, achievementMaxProgressValue)) + "/" + GameManager.changeUnit(achievementMaxProgressValue);
		}
		else
		{
			progressValue.text = Math.Min(currentProgressValue, achievementMaxProgressValue) + "/" + achievementMaxProgressValue;
		}
		progressBar.fillAmount = (float)Math.Min(currentProgressValue, achievementMaxProgressValue) / (float)achievementMaxProgressValue;
		if (Singleton<AchievementManager>.instance.isCanObtainReward(currentAchievementType))
		{
			achievementLevelImage.sprite = Singleton<CachedManager>.instance.achievementCompleteSprites[m_currentAchievementData.currentAchievementLevel - 1];
		}
		else if (!flag)
		{
			achievementLevelImage.sprite = Singleton<CachedManager>.instance.achievementProgressingSprites[m_currentAchievementData.currentAchievementLevel - 1];
		}
		else
		{
			achievementLevelImage.sprite = Singleton<CachedManager>.instance.achievementCompleteSprites[m_currentAchievementData.currentAchievementLevel - 1];
		}
		achievementLevelImage.SetNativeSize();
		for (int k = 0; k < rewardValueText.Length; k++)
		{
			rewardValueText[k].text = GameManager.changeUnit(m_currentRewardData.rewardValue);
		}
	}

	public void reward()
	{
		if (!Singleton<AchievementManager>.instance.rewardEvent(currentAchievementType))
		{
			return;
		}
		AchievementData achievementDataFromInventory = Singleton<AchievementManager>.instance.getAchievementDataFromInventory(currentAchievementType);
		AchievementManager.RewardData rewardData = ((!m_isOneLevelAchievement) ? Singleton<AchievementManager>.instance.getRewardData(currentAchievementType, achievementDataFromInventory.currentAchievementLevel - 1) : Singleton<AchievementManager>.instance.getRewardData(currentAchievementType, achievementDataFromInventory.currentAchievementLevel));
		if (rewardData.currentRewardType == AchievementManager.RewardType.Ruby)
		{
			Singleton<FlyResourcesManager>.instance.playEffectResources(rewardButtonCenterTransform, FlyResourcesManager.ResourceType.Ruby, (int)Mathf.Min(rewardData.rewardValue, 10f), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
		}
		m_currentRewardTrophyEffect = ObjectPool.Spawn("fx_result_get_star", Vector2.zero, new Vector3(90f, 0f, 0f), achievementLevelImageTrasnform);
		m_currentRewardButtonEffect = ObjectPool.Spawn("fx_get_reward", Vector2.zero, rewardButtonCenterTransform);
		AnalyzeManager.retention(AnalyzeManager.CategoryType.Achievement, AnalyzeManager.ActionType.Achievement, new Dictionary<string, string>
		{
			{
				"AchievementType",
				achievementDataFromInventory.currentAchievementType.ToString()
			},
			{
				"AchievementLevel",
				achievementDataFromInventory.currentAchievementLevel.ToString()
			}
		});
		UIWindowManageAchievemant.instance.refreshCompleteNotification();
		if (Singleton<AchievementManager>.instance.isCompleteAchievement(currentAchievementType))
		{
			UIWindowManageAchievemant.instance.refreshAchievement();
		}
		else
		{
			refreshSlot();
		}
		if (Singleton<AchievementManager>.instance.isCanObtainReward(currentAchievementType) && !achievementDataFromInventory.isPushNotification)
		{
			achievementDataFromInventory.isPushNotification = true;
			Singleton<AchievementManager>.instance.openCompleteAchievementUI(currentAchievementType);
		}
	}

	private void OnDisable()
	{
		if (m_currentRewardTrophyEffect != null)
		{
			ObjectPool.Recycle(m_currentRewardTrophyEffect.name, m_currentRewardTrophyEffect);
		}
		if (m_currentRewardButtonEffect != null)
		{
			ObjectPool.Recycle(m_currentRewardButtonEffect.name, m_currentRewardButtonEffect);
		}
	}
}
