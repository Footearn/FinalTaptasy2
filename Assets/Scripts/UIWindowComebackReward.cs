using UnityEngine;
using UnityEngine.UI;

public class UIWindowComebackReward : UIWindow
{
	public static UIWindowComebackReward instance;

	public RectTransform goldboxTransform;

	public GameObject[] effectObjects;

	public Text rewardText;

	private double rewardAmount;

	public bool RewardAvailable
	{
		get;
		private set;
	}

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		GameObject[] array = effectObjects;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(true);
		}
		return base.OnBeforeOpen();
	}

	public void SetComebackReward(double h)
	{
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		RewardAvailable = true;
		rewardAmount = Singleton<DataManager>.instance.GetCalculatedReward(h);
		if (rewardAmount > 0.0)
		{
			rewardText.text = GameManager.changeUnit(rewardAmount);
			if (RewardAvailable)
			{
				Singleton<GoldManager>.instance.increaseGold(rewardAmount, true);
				Singleton<DataManager>.instance.saveData();
			}
			open();
		}
	}

	public void OnClickButton()
	{
		GameObject[] array = effectObjects;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		close();
	}
}
