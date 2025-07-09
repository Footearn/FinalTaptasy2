using UnityEngine;
using UnityEngine.UI;

public class RouletteInformationSlot : ObjectBase
{
	public Image iconImage;

	public RectTransform iconTransform;

	public Text valueText;

	private RouletteManager.RouletteRewardData m_targetRewardData;

	public void initSlot(RouletteManager.RouletteRewardData targetRewardData)
	{
		if (!base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(true);
		}
		m_targetRewardData = targetRewardData;
		float d = 1f;
		switch (m_targetRewardData.rewardType)
		{
		case RouletteManager.RouletteRewardType.Gold:
			valueText.text = GameManager.changeUnit(CalculateManager.getCurrentStandardGold() * m_targetRewardData.value);
			d = 0.65f;
			break;
		case RouletteManager.RouletteRewardType.Ruby:
			valueText.text = m_targetRewardData.value.ToString("N0");
			d = 0.65f;
			break;
		case RouletteManager.RouletteRewardType.HeartCoin:
			valueText.text = m_targetRewardData.value.ToString("N0");
			d = 0.5f;
			break;
		case RouletteManager.RouletteRewardType.TreasureKey:
			valueText.text = m_targetRewardData.value.ToString("N0");
			d = 0.8f;
			break;
		case RouletteManager.RouletteRewardType.TranscendStone:
			valueText.text = m_targetRewardData.value.ToString("N0");
			d = 1f;
			break;
		}
		iconTransform.localScale = Vector3.one * d;
		iconImage.sprite = UIWindowRouletteReward.instance.rouletteRewardInformationSpriteList[(int)m_targetRewardData.rewardType];
		iconImage.SetNativeSize();
	}
}
