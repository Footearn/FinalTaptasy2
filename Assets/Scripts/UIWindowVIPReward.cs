using UnityEngine;
using UnityEngine.UI;

public class UIWindowVIPReward : UIWindow
{
	public static UIWindowVIPReward instance;

	public Text descriptionText;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openVIPReward(int leftDay)
	{
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		leftDay = Mathf.Max(leftDay, 0);
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
		descriptionText.text = string.Format(I18NManager.Get("VIP_REWARD_DESCRIPTION"), leftDay);
		open();
		Singleton<ShopManager>.instance.vipRewardEvent();
	}

	public override void OnAfterClose()
	{
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
		base.OnAfterClose();
	}
}
