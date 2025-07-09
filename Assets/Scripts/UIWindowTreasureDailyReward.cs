using UnityEngine;
using UnityEngine.UI;

public class UIWindowTreasureDailyReward : UIWindow
{
	public static UIWindowTreasureDailyReward instance;

	public Text descriptionText;

	public RectTransform rewardButtonRectTrasnform;

	private long m_currentRewardValue;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openDailyRewardByTreasureEffect(TreasureInventoryData treasure)
	{
		Singleton<TreasureManager>.instance.refreshTreasureInventoryData();
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
		m_currentRewardValue = (long)(treasure.treasureEffectValue + treasure.extraTreasureEffectValue);
		descriptionText.text = string.Format(I18NManager.Get("DAILY_REWARD_BY_TREASURE_DESCRIPTION"), Singleton<TreasureManager>.instance.getTreasureI18NName(treasure.treasureType), m_currentRewardValue);
		open();
	}

	public void onClickReward()
	{
		Singleton<AudioManager>.instance.playEffectSound("btn_reward");
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
		Singleton<RubyManager>.instance.increaseRuby(m_currentRewardValue);
		Singleton<FlyResourcesManager>.instance.playEffectResources(rewardButtonRectTrasnform, FlyResourcesManager.ResourceType.Ruby, m_currentRewardValue, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		Singleton<DataManager>.instance.saveData();
		close();
	}
}
