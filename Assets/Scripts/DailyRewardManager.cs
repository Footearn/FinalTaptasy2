using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardManager : Singleton<DailyRewardManager>
{
	public struct DailyRewardData
	{
		public DailyRewardType currentDailyRewardType;

		public double dailyRewardValue;

		public DailyRewardData(DailyRewardType currentDailyRewardType, double dailyRewardValue)
		{
			this.currentDailyRewardType = currentDailyRewardType;
			this.dailyRewardValue = dailyRewardValue;
		}
	}

	public enum DailyRewardType
	{
		Gold,
		Ruby,
		TreasureKey
	}

	public static int DailyRewardMaxDay = 7;

	public List<Sprite> dailyRewardSpriteList;

	public Sprite canObtainRewardBackground;

	public Sprite obtainedBackground;

	public Sprite willObtainBackground;

	public Dictionary<int, DailyRewardData> currentDailyRewardDataList = new Dictionary<int, DailyRewardData>();

	public DateTime timeCheckTreasureDailyReward;

	public string currentEnoughTimeString = string.Empty;

	public bool isCanGetDailyReward = true;

	public bool timerAvailable;

	private void Awake()
	{
		currentDailyRewardDataList.Add(1, new DailyRewardData(DailyRewardType.Gold, 100.0));
		currentDailyRewardDataList.Add(2, new DailyRewardData(DailyRewardType.Ruby, 10.0));
		currentDailyRewardDataList.Add(3, new DailyRewardData(DailyRewardType.Gold, 200.0));
		currentDailyRewardDataList.Add(4, new DailyRewardData(DailyRewardType.Ruby, 15.0));
		currentDailyRewardDataList.Add(5, new DailyRewardData(DailyRewardType.Gold, 300.0));
		currentDailyRewardDataList.Add(6, new DailyRewardData(DailyRewardType.Ruby, 20.0));
		currentDailyRewardDataList.Add(7, new DailyRewardData(DailyRewardType.TreasureKey, 30.0));
	}

	private void Start()
	{
		timeCheckTreasureDailyReward = new DateTime(Singleton<DataManager>.instance.currentGameData.treasureDailyRewardTime);
		timerAvailable = false;
		currentEnoughTimeString = string.Empty;
		isCanGetDailyReward = UnbiasedTime.Instance.Now().Ticks > timeCheckTreasureDailyReward.Ticks;
	}

	public Sprite getDailyRewardSprite(DailyRewardType rewardType)
	{
		return dailyRewardSpriteList[(int)rewardType];
	}

	public void claimEvent(int targetDay)
	{
		if (Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedDayNumber != targetDay || Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedTime >= UnbiasedTime.Instance.Now().Ticks)
		{
			return;
		}
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		if (targetDay >= DailyRewardMaxDay)
		{
			Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedDayNumber = 1;
		}
		else
		{
			Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedDayNumber++;
		}
		DailyRewardData dailyRewardData = currentDailyRewardDataList[targetDay];
		switch (dailyRewardData.currentDailyRewardType)
		{
		case DailyRewardType.Gold:
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.Gold, 30L, 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			Singleton<GoldManager>.instance.increaseGold(CalculateManager.getCurrentStandardGold() * dailyRewardData.dailyRewardValue);
			break;
		case DailyRewardType.Ruby:
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.Ruby, (long)Math.Min(dailyRewardData.dailyRewardValue, 30.0), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			Singleton<RubyManager>.instance.increaseRuby((long)dailyRewardData.dailyRewardValue, true);
			break;
		case DailyRewardType.TreasureKey:
			Singleton<TreasureManager>.instance.increaseTreasurePiece((long)dailyRewardData.dailyRewardValue);
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.TreasurePiece, (long)Math.Min(dailyRewardData.dailyRewardValue, 30.0), 0.04f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			break;
		}
		DateTime dateTime = UnbiasedTime.Instance.Now().AddDays(1.0);
		Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0).Ticks;
		Singleton<DataManager>.instance.saveData();
		UIWindowDailyReward.instance.close();
	}

	private void OnApplicationPause(bool status)
	{
		UnbiasedTime.Instance.OnApplicationPauseForUnbiasedTime(status);
		if (!status)
		{
			if (!TutorialManager.isTutorial && !TutorialManager.isRebirthTutorial && GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				Singleton<DataManager>.instance.checkRewardDateTime();
			}
		}
		else
		{
			Singleton<DataManager>.instance.updateRewardDateTime();
			Singleton<DataManager>.instance.seenComebackRewardWindow = false;
		}
	}

	private void SaveTime(bool resetTimer = false)
	{
		if (resetTimer)
		{
			timeCheckTreasureDailyReward = UnbiasedTime.Instance.UtcNow().AddHours(24.0);
		}
		Singleton<DataManager>.instance.currentGameData.treasureDailyRewardTime = timeCheckTreasureDailyReward.Ticks;
		Singleton<DataManager>.instance.saveData();
	}

	private IEnumerator updateTimeForDailyReward()
	{
		while (true)
		{
			if (timeCheckTreasureDailyReward.Ticks > UnbiasedTime.Instance.UtcNow().Ticks)
			{
				timerAvailable = true;
				isCanGetDailyReward = false;
				TimeSpan timeleft = timeCheckTreasureDailyReward.Subtract(UnbiasedTime.Instance.UtcNow());
				currentEnoughTimeString = string.Format(((timeleft.Hours == 0) ? string.Empty : ("{0:D1}" + I18NManager.Get("HOURS"))) + ((timeleft.Minutes == 0) ? string.Empty : ("{1:D1}" + I18NManager.Get("MINUTES"))) + "{2:D1}" + I18NManager.Get("SECONDS"), timeleft.Hours, timeleft.Minutes, timeleft.Seconds);
			}
			else
			{
				timerAvailable = false;
				isCanGetDailyReward = true;
				currentEnoughTimeString = I18NManager.Get("CAN_OBTAIN_DAILY_REWARD");
			}
			yield return null;
		}
	}

	public void checkTreasureDailyRewards()
	{
		if (Singleton<TreasureManager>.instance.containTreasureFromInventory(TreasureManager.TreasureType.MonocleOfWiseGoddess) && isCanGetDailyReward)
		{
			if (UnbiasedTime.Instance.UtcNow().Ticks > timeCheckTreasureDailyReward.Ticks)
			{
				SaveTime(true);
				UIWindowTreasureDailyReward.instance.openDailyRewardByTreasureEffect(Singleton<TreasureManager>.instance.getTreasureDataFromInventory(TreasureManager.TreasureType.MonocleOfWiseGoddess));
			}
			StopCoroutine("updateTimeForDailyReward");
			StartCoroutine("updateTimeForDailyReward");
		}
	}
}
