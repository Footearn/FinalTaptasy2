using System.Collections.Generic;

public class UIWindowDailyReward : UIWindow
{
	public static UIWindowDailyReward instance;

	public List<DailyRewardSlot> dailyRewardSlotList;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openDailyRewardUI()
	{
		for (int i = 0; i < dailyRewardSlotList.Count; i++)
		{
			dailyRewardSlotList[i].initDailyRewardSlot();
		}
		open();
	}
}
