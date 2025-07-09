using System;

public class NotificationManager : Singleton<NotificationManager>
{
	private static string BossRaidNotificationID = "BossDungeonNotificationID";

	private static string ComeBackRewardNotificationID = "ComeBackRewardNotificationID";

	private static string ComeBackRewardNotificationDetail_ID = "ComeBackRewardNotificationDetail_ID";

	public static void SetBossRaidtNotification(int reservationTime)
	{
	}

	public static void SetComeBackRewardtNotification()
	{
		TimeSpan timeSpan = new TimeSpan(24, 0, 0);
		TimeSpan timeSpan2 = new TimeSpan(56, 0, 0);
		int reservationTime = (int)timeSpan.TotalSeconds;
		int reservationTime2 = (int)timeSpan2.TotalSeconds;
		CancelAllLocalNotification();
		if (Singleton<DataManager>.instance.currentGameData.isPushNotificationOn)
		{
			double calculatedReward = Singleton<DataManager>.instance.GetCalculatedReward(timeSpan.TotalHours);
			if (calculatedReward > 0.0)
			{
				string messages = string.Format(I18NManager.Get("LOCAL_PUSH_GOLD_BOX_DESCRIPTION_DETAIL"), GameManager.changeUnit(calculatedReward));
				Singleton<NotificationManager>.instance.setNotification(ComeBackRewardNotificationDetail_ID, I18NManager.Get("NOTIFICATION_TITLE"), messages, reservationTime);
			}
			Singleton<NotificationManager>.instance.setNotification(ComeBackRewardNotificationID, I18NManager.Get("NOTIFICATION_TITLE"), I18NManager.Get("LOCAL_PUSH_GOLD_BOX_DESCRIPTION"), reservationTime2);
		}
	}

	public static void SetNotification(string notificationName, string title, string messages, int reservationTime)
	{
		if (Singleton<DataManager>.instance.currentGameData.isPushNotificationOn)
		{
			Singleton<NotificationManager>.instance.setNotification(notificationName, title, messages, reservationTime);
		}
	}

	public static void CancelAllLocalNotification()
	{
		SA_Singleton<AndroidNotificationManager>.Instance.CancelAllLocalNotifications();
	}

	public void setNotification(string notificationName, string title, string messages, int reservationTime)
	{
		int @int = NSPlayerPrefs.GetInt(notificationName, -1);
		if (@int < 0)
		{
			NSPlayerPrefs.SetInt(notificationName, SA_Singleton<AndroidNotificationManager>.Instance.ScheduleLocalNotification(title, messages, reservationTime));
			return;
		}
		AndroidNotificationBuilder builder = new AndroidNotificationBuilder(@int, title, messages, reservationTime);
		SA_Singleton<AndroidNotificationManager>.Instance.ScheduleLocalNotification(builder);
	}
}
