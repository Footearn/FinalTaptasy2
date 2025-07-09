using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteSlot : ObjectBase
{
	public TextMeshProUGUI titleText;

	public RectTransform iconImageTransform;

	public Image iconImage;

	private RouletteManager.RouletteRewardData m_currentRouletteData;

	public float zRotation
	{
		get
		{
			return base.cachedTransform.localEulerAngles.z;
		}
	}

	public void initRouletteSlot(RouletteManager.RouletteRewardData rouletteData)
	{
		m_currentRouletteData = rouletteData;
		string text = string.Empty;
		float d = 1f;
		switch (m_currentRouletteData.rewardType)
		{
		case RouletteManager.RouletteRewardType.Gold:
			text = GameManager.changeUnit(rouletteData.value * CalculateManager.getCurrentStandardGold());
			break;
		case RouletteManager.RouletteRewardType.Ruby:
			d = 1.5f;
			text = ((long)rouletteData.value).ToString("N0");
			break;
		case RouletteManager.RouletteRewardType.TreasureKey:
			d = 0.6f;
			text = ((long)rouletteData.value).ToString("N0");
			break;
		case RouletteManager.RouletteRewardType.HeartCoin:
			d = 1f;
			text = ((long)rouletteData.value).ToString("N0");
			break;
		case RouletteManager.RouletteRewardType.TranscendStone:
			d = 0.8f;
			text = ((long)rouletteData.value).ToString("N0");
			break;
		}
		iconImageTransform.localScale = Vector3.one * d;
		titleText.text = text;
		iconImage.sprite = Singleton<RouletteManager>.instance.rouletteSpriteList[(int)m_currentRouletteData.rewardType];
		iconImage.SetNativeSize();
	}
}
