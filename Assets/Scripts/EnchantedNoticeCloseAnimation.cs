using UnityEngine;

public class EnchantedNoticeCloseAnimation : MonoBehaviour
{
	public void openNextNotice()
	{
		UIWindowLotteryBossRaidChest.instance.closedEnchantedNoticeEvent();
	}
}
