using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObject : MonoBehaviour
{
	public QuestData questData;

	public CanvasGroup alphaGroup;

	public Image iconImage;

	public Text questText;

	public Text questProgressText;

	public RectTransform rewardIconTransform;

	public Image rewardIconImage;

	public Text[] rewardTexts;

	public CanvasGroup rewardGroup;

	public GameObject notCompleteObject;

	public GameObject completeObject;

	public int targetThemeIndex;

	public int targetStageIndex;

	public int idx;

	private QuestManager m_questManager;

	public void LoadQuest(int idx, bool isNewQuest)
	{
		this.idx = idx;
		if (!isNewQuest)
		{
			base.transform.localPosition = new Vector2(0f, base.transform.localPosition.y);
			alphaGroup.alpha = 1f;
		}
		m_questManager = Singleton<QuestManager>.instance;
		questData = m_questManager.questData[idx];
		bool isComplete = questData.isComplete;
		if (isComplete)
		{
			notCompleteObject.SetActive(false);
			completeObject.SetActive(true);
		}
		else
		{
			notCompleteObject.SetActive(true);
			completeObject.SetActive(false);
		}
		targetThemeIndex = 0;
		targetStageIndex = 0;
		iconImage.sprite = m_questManager.questIconSprites[(int)questData.questType];
		iconImage.transform.localScale = Vector2.one;
		QuestManager.QuestType questType = questData.questType;
		if (questType == QuestManager.QuestType.NextStage)
		{
			int num = Mathf.FloorToInt((float)questData.questLevel / 100f);
			int num2 = (int)((float)questData.questLevel % 100f);
			string empty = string.Empty;
			empty = num + "-" + num2;
			questText.text = string.Format(I18NManager.Get("QUEST_DESCRIPTION_" + (int)questData.questType), empty);
			if (num <= Singleton<DataManager>.instance.currentGameData.unlockTheme && num2 <= Singleton<DataManager>.instance.currentGameData.unlockStage)
			{
				targetThemeIndex = num;
				targetStageIndex = num2;
			}
		}
		else
		{
			questText.text = string.Format(I18NManager.Get("QUEST_DESCRIPTION_" + (int)questData.questType), (questData.questType != QuestManager.QuestType.Gold) ? questData.questGoal.ToString() : GameManager.changeUnit(questData.questGoal, false));
		}
		iconImage.SetNativeSize();
		if (!isComplete)
		{
			questProgressText.text = "<size=35><color=#FAD725>" + GameManager.changeUnit(questData.questValue) + "</color></size> of " + GameManager.changeUnit(questData.questGoal);
		}
		rewardIconImage.sprite = m_questManager.rewardSprites[(int)questData.rewardType];
		if (isComplete)
		{
			rewardGroup.alpha = 1f;
		}
		else
		{
			rewardGroup.alpha = 0.5f;
		}
		rewardIconImage.SetNativeSize();
		for (int i = 0; i < rewardTexts.Length; i++)
		{
			double rewardValue = questData.rewardValue;
			rewardTexts[i].text = GameManager.changeUnit(rewardValue);
		}
	}

	public void OnClickReward()
	{
		if (!m_questManager.questData[idx].isComplete)
		{
			return;
		}
		AnalyzeManager.retention(AnalyzeManager.CategoryType.Achievement, AnalyzeManager.ActionType.Quest, new Dictionary<string, string>
		{
			{
				"QuestType",
				questData.questType.ToString()
			}
		});
		double num = questData.rewardValue;
		if (Singleton<StatManager>.instance.questRewardDoubleChance > 0.0)
		{
			int num2 = UnityEngine.Random.Range(0, 10000);
			num2 /= 100;
			if (Singleton<StatManager>.instance.questRewardDoubleChance > (double)num2)
			{
				num *= 2.0;
				ObjectPool.Spawn("@QuestDoubleBonus", Vector2.zero, Singleton<CachedManager>.instance.canvasAlwaysTopUICenterTransform).GetComponent<RectTransform>().position = Util.getCurrentScreenToWorldPosition();
				Singleton<AudioManager>.instance.playEffectSound("questcomplete");
			}
		}
		switch (questData.rewardType)
		{
		case QuestManager.RewardType.Gold:
			Singleton<FlyResourcesManager>.instance.playEffectResources(rewardIconTransform, FlyResourcesManager.ResourceType.Gold, (int)Math.Min(num, 10.0), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			Singleton<GoldManager>.instance.increaseGold(num, true);
			break;
		case QuestManager.RewardType.Ruby:
			Singleton<FlyResourcesManager>.instance.playEffectResources(rewardIconTransform, FlyResourcesManager.ResourceType.Ruby, (long)Math.Min(num, 10.0), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			Singleton<RubyManager>.instance.increaseRuby((long)num, true);
			break;
		}
		UIWindowManageHeroAndWeapon.instance.refreshAllBuyState();
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.QuestMania, 1.0);
		m_questManager.questData[idx] = m_questManager.getQuest(idx);
		m_questManager.saveQuest();
		UIWindowQuest.instance.refreshCompleteNotification();
		Singleton<AudioManager>.instance.playEffectSound("btn_reward");
		StartCoroutine("alphaAnimation");
		for (int i = 0; i < UIWindowQuest.instance.questObjects.Length; i++)
		{
			if (i != idx)
			{
				UIWindowQuest.instance.questObjects[i].LoadQuest(i, false);
			}
		}
	}

	private IEnumerator alphaAnimation()
	{
		float timer2 = 0f;
		while (true)
		{
			timer2 += Time.deltaTime * GameManager.timeScale;
			if (timer2 >= 0.5f)
			{
				break;
			}
			base.transform.localPosition = new Vector2(base.transform.localPosition.x - timer2 * 20f, base.transform.localPosition.y);
			alphaGroup.alpha = Mathf.Clamp01(1f - timer2 * 2f);
			yield return null;
		}
		LoadQuest(idx, true);
		timer2 = 0f;
		base.transform.localPosition = new Vector2(150f, base.transform.localPosition.y);
		while (true)
		{
			timer2 += Time.deltaTime * GameManager.timeScale;
			if (timer2 >= 0.5f)
			{
				break;
			}
			base.transform.localPosition = new Vector2(base.transform.localPosition.x - timer2 * 20f, base.transform.localPosition.y);
			alphaGroup.alpha = Mathf.Clamp01(timer2 * 2f);
			yield return null;
		}
		base.transform.localPosition = new Vector2(0f, base.transform.localPosition.y);
	}
}
