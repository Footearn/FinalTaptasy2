using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowRouletteReward : UIWindow
{
	public static UIWindowRouletteReward instance;

	public Text titleText;

	public List<Sprite> rouletteRewardInformationSpriteList;

	public List<RouletteInformationSlot> totalSlotObjects;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openRouletteUI(bool isBronzeRoulette)
	{
		titleText.text = I18NManager.Get((!isBronzeRoulette) ? "GOLD_ROULETTE_REWARD" : "BRONZE_ROULETTE_REWARD");
		for (int i = 0; i < totalSlotObjects.Count; i++)
		{
			if (totalSlotObjects[i].cachedGameObject.activeSelf)
			{
				totalSlotObjects[i].cachedGameObject.SetActive(false);
			}
		}
		List<RouletteManager.RouletteRewardData> list = ((!isBronzeRoulette) ? Singleton<RouletteManager>.instance.goldRouletteRewardList : Singleton<RouletteManager>.instance.bronzeRouletteRewardList);
		for (int j = 0; j < list.Count; j++)
		{
			totalSlotObjects[j].initSlot(list[j]);
		}
		open();
	}
}
