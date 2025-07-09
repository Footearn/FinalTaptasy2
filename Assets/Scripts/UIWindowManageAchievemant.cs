using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowManageAchievemant : UIWindow
{
	public static UIWindowManageAchievemant instance;

	public InfiniteScroll scroll;

	public List<AchievementManager.AchievementType> completedAchievementType;

	public List<AchievementManager.AchievementType> notCompletedAchievementType;

	public Image achievementIconImage;

	public Sprite googlePlayAchievementIconSprite;

	public Sprite gameCenterAchievementIconSprite;

	public override void Awake()
	{
		if (completedAchievementType == null)
		{
			completedAchievementType = new List<AchievementManager.AchievementType>();
		}
		if (notCompletedAchievementType == null)
		{
			notCompletedAchievementType = new List<AchievementManager.AchievementType>();
		}
		instance = this;
		achievementIconImage.sprite = googlePlayAchievementIconSprite;
		achievementIconImage.SetNativeSize();
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		refreshCompleteNotification();
		return base.OnBeforeOpen();
	}

	public void refreshCompleteNotification()
	{
		UIWindowOutgame.instance.refreshAchievementCompleteIndicator();
	}

	private void OnEnable()
	{
		refreshAchievement();
	}

	public void refreshAchievement()
	{
		completedAchievementType.Clear();
		notCompletedAchievementType.Clear();
		for (int i = 0; i < 18; i++)
		{
			if (Singleton<AchievementManager>.instance.isCanObtainReward((AchievementManager.AchievementType)i))
			{
				completedAchievementType.Add((AchievementManager.AchievementType)i);
			}
			else
			{
				notCompletedAchievementType.Add((AchievementManager.AchievementType)i);
			}
		}
		scroll.refreshAll();
	}

	public bool isCompleted(AchievementManager.AchievementType type)
	{
		return completedAchievementType.Contains(type);
	}

	public bool isNotCompleted(AchievementManager.AchievementType type)
	{
		return notCompletedAchievementType.Contains(type);
	}

	public void OnClickAchievementUI()
	{
		if (!Social.localUser.authenticated)
		{
			Social.localUser.Authenticate(delegate(bool success)
			{
				if (success)
				{
					Social.ShowAchievementsUI();
				}
			});
		}
		else
		{
			Social.ShowAchievementsUI();
		}
	}
}
