using System;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardSlot : ObjectBase
{
	public int currentDayNumber;

	public Text dayNumberText;

	public Image rewardImage;

	public Text[] rewardValueText;

	public Image backgroundImage;

	public GameObject canObtainRewardObject;

	public GameObject rewardButtonObject;

	public GameObject timerObject;

	public Text timerText;

	public bool isTimerOn;

	public GameObject obtainedRewardObject;

	public GameObject willObtainRewardObject;

	private DailyRewardManager.DailyRewardData m_currentDailyRewardData;

	public void initDailyRewardSlot()
	{
		isTimerOn = false;
		m_currentDailyRewardData = Singleton<DailyRewardManager>.instance.currentDailyRewardDataList[currentDayNumber];
		canObtainRewardObject.SetActive(false);
		obtainedRewardObject.SetActive(false);
		willObtainRewardObject.SetActive(false);
		dayNumberText.text = string.Format(I18NManager.Get("DAILY_COUNTING"), currentDayNumber);
		rewardImage.sprite = Singleton<DailyRewardManager>.instance.getDailyRewardSprite(m_currentDailyRewardData.currentDailyRewardType);
		rewardImage.SetNativeSize();
		if (Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedDayNumber == currentDayNumber)
		{
			dayNumberText.color = Util.getCalculatedColor(253f, 252f, 183f);
			canObtainRewardObject.SetActive(true);
			backgroundImage.sprite = Singleton<DailyRewardManager>.instance.canObtainRewardBackground;
			if (Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedTime < UnbiasedTime.Instance.Now().Ticks)
			{
				rewardButtonObject.SetActive(true);
				timerObject.SetActive(false);
			}
			else
			{
				isTimerOn = true;
				rewardButtonObject.SetActive(false);
				timerObject.SetActive(true);
			}
		}
		else if (Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedDayNumber > currentDayNumber)
		{
			dayNumberText.color = Util.getCalculatedColor(30f, 199f, 255f);
			obtainedRewardObject.SetActive(true);
			backgroundImage.sprite = Singleton<DailyRewardManager>.instance.obtainedBackground;
		}
		else if (Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedDayNumber < currentDayNumber)
		{
			dayNumberText.color = Util.getCalculatedColor(30f, 199f, 255f);
			willObtainRewardObject.SetActive(true);
			backgroundImage.sprite = Singleton<DailyRewardManager>.instance.willObtainBackground;
		}
		string str = ((!canObtainRewardObject.activeSelf) ? "<color=#FDFCB7>" : "<color=#5A3E1F>");
		for (int i = 0; i < rewardValueText.Length; i++)
		{
			if (m_currentDailyRewardData.currentDailyRewardType == DailyRewardManager.DailyRewardType.Gold)
			{
				rewardValueText[i].text = str + GameManager.changeUnit(CalculateManager.getCurrentStandardGold() * m_currentDailyRewardData.dailyRewardValue) + "</color>";
				if (!canObtainRewardObject.activeSelf)
				{
					Text obj = rewardValueText[i];
					obj.text = obj.text + "\n<size=20>" + I18NManager.Get("COUPON_REWARD_GOLD") + "</size>";
				}
				continue;
			}
			rewardValueText[i].text = str + m_currentDailyRewardData.dailyRewardValue.ToString("N0") + "</color>";
			if (!canObtainRewardObject.activeSelf)
			{
				switch (m_currentDailyRewardData.currentDailyRewardType)
				{
				case DailyRewardManager.DailyRewardType.Ruby:
				{
					Text obj3 = rewardValueText[i];
					obj3.text = obj3.text + "\n<size=20>" + I18NManager.Get("COUPON_REWARD_RUBY") + "</size>";
					break;
				}
				case DailyRewardManager.DailyRewardType.TreasureKey:
				{
					Text obj2 = rewardValueText[i];
					obj2.text = obj2.text + "\n<size=20>" + I18NManager.Get("COUPON_REWARD_KEYS") + "</size>";
					break;
				}
				}
			}
		}
	}

	private void Update()
	{
		if (isTimerOn)
		{
			if (Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedTime < UnbiasedTime.Instance.Now().Ticks)
			{
				initDailyRewardSlot();
				return;
			}
			TimeSpan timeSpan = new DateTime(Singleton<DataManager>.instance.currentGameData.lastDailyRewardReceivedTime).Subtract(UnbiasedTime.Instance.Now());
			timerText.text = string.Format(((timeSpan.Days <= 0) ? string.Empty : ("{0:D1}" + I18NManager.Get("SHOP_LIMITED_DAY_SIMPLE") + " ")) + ((timeSpan.Hours <= 0) ? string.Empty : "{1:00}:") + ((timeSpan.Minutes <= 0) ? string.Empty : "{2:00}:") + "{3:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
	}

	public void OnClickClaim()
	{
		Singleton<DailyRewardManager>.instance.claimEvent(currentDayNumber);
	}
}
