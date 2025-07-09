using UnityEngine;
using UnityEngine.UI;

public class TreasureCollectionCell : ObjectBase
{
	public TreasureManager.TreasureType treasureType;

	public Text enchantCountText;

	public Transform slotIconTransform;

	public Image backgroundImage;

	public Image treasureIconImage;

	public GameObject disableObject;

	public GameObject pvpIconObject;

	public void refreshCell()
	{
		slotIconTransform.SetAsFirstSibling();
		backgroundImage.sprite = Singleton<TreasureManager>.instance.tierBackgroundSprites[Singleton<TreasureManager>.instance.getTreasureTier(treasureType)];
		treasureIconImage.sprite = Singleton<TreasureManager>.instance.getTreasureSprite(treasureType);
		pvpIconObject.SetActive(Singleton<TreasureManager>.instance.isPVPTreasure(treasureType));
		if (Singleton<TreasureManager>.instance.containTreasureFromInventory(treasureType))
		{
			if (Singleton<TreasureManager>.instance.getTreasureDataFromInventory(treasureType).treasureLevel < Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].maxLevel)
			{
				enchantCountText.text = "Lv." + Singleton<TreasureManager>.instance.getTreasureDataFromInventory(treasureType).treasureLevel;
			}
			else
			{
				enchantCountText.text = "Max";
			}
			disableObject.SetActive(false);
		}
		else
		{
			enchantCountText.text = string.Empty;
			disableObject.SetActive(true);
		}
		backgroundImage.SetNativeSize();
		treasureIconImage.SetNativeSize();
	}

	public void onClickSelectTreasureCell()
	{
		UIWindowTreasureInformation.instance.openWithTreasureInformation(treasureType);
	}
}
