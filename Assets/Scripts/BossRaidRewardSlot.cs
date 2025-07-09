using UnityEngine;
using UnityEngine.UI;

public class BossRaidRewardSlot : ObjectBase
{
	public Text rewardCountText;

	public GameObject rewardChestTypeImageObject;

	public Image rewardChestTypeImage;

	public Image rewardTargetIconImage;

	public RectTransform rewardTargetIconImageRectTransform;

	private BossRaidManager.ChestRewardData m_currentRewardData;

	public void refreshSlot(BossRaidManager.ChestRewardData rewardType)
	{
		m_currentRewardData = rewardType;
		if (rewardType.targetRewardType == BossRaidManager.BossRaidChestRewardType.Null)
		{
			base.cachedGameObject.SetActive(false);
			return;
		}
		base.cachedGameObject.SetActive(true);
		rewardChestTypeImageObject.SetActive(false);
		rewardCountText.text = string.Empty;
		rewardTargetIconImageRectTransform.anchoredPosition = new Vector2(0f, 13.8f);
		rewardTargetIconImage.transform.localScale = Vector3.one;
		switch (rewardType.targetRewardType)
		{
		case BossRaidManager.BossRaidChestRewardType.Gold:
			rewardCountText.text = GameManager.changeUnit(rewardType.value);
			rewardTargetIconImage.sprite = UIWindowLotteryBossRaidChest.instance.goldIconSprite;
			rewardTargetIconImageRectTransform.localScale = new Vector3(1f, 1f, 1f);
			break;
		case BossRaidManager.BossRaidChestRewardType.Ruby:
			rewardCountText.text = rewardType.value.ToString("N0");
			rewardTargetIconImage.sprite = UIWindowLotteryBossRaidChest.instance.rubyIconSprite;
			rewardTargetIconImageRectTransform.localScale = new Vector3(1f, 1f, 1f);
			break;
		case BossRaidManager.BossRaidChestRewardType.CharacterSkin:
			rewardTargetIconImageRectTransform.localScale = new Vector3(1f, 1f, 1f);
			rewardTargetIconImageRectTransform.anchoredPosition = new Vector2(0f, 1.6f);
			rewardChestTypeImageObject.SetActive(true);
			switch (rewardType.chestType)
			{
			case BossRaidManager.BossRaidChestType.Bronze:
				rewardChestTypeImage.sprite = Singleton<BossRaidManager>.instance.bronzeChestSprite;
				break;
			case BossRaidManager.BossRaidChestType.Gold:
				rewardChestTypeImage.sprite = Singleton<BossRaidManager>.instance.goldChestSprite;
				break;
			case BossRaidManager.BossRaidChestType.Dia:
				rewardChestTypeImage.sprite = Singleton<BossRaidManager>.instance.diaChestSprite;
				break;
			}
			rewardChestTypeImage.SetNativeSize();
			switch (rewardType.characterType)
			{
			case CharacterManager.CharacterType.Warrior:
				if (rewardType.warriorCharacterSkinType == CharacterSkinManager.WarriorSkinType.Drake)
				{
					rewardTargetIconImage.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				}
				rewardTargetIconImage.sprite = Singleton<CharacterSkinManager>.instance.warriorSkinSprites[(int)rewardType.warriorCharacterSkinType];
				break;
			case CharacterManager.CharacterType.Archer:
				rewardTargetIconImage.sprite = Singleton<CharacterSkinManager>.instance.archerSkinSprites[(int)rewardType.archerCharacterSkinType];
				break;
			case CharacterManager.CharacterType.Priest:
				rewardTargetIconImage.sprite = Singleton<CharacterSkinManager>.instance.priestSkinSprites[(int)rewardType.priestCharacterSkinType];
				break;
			}
			break;
		case BossRaidManager.BossRaidChestRewardType.TreasureEnchantStone:
			rewardTargetIconImage.sprite = UIWindowLotteryBossRaidChest.instance.treasureEnchantStoneIconSprite;
			rewardCountText.text = rewardType.value.ToString("N0");
			rewardTargetIconImageRectTransform.localScale = new Vector3(1.5f, 1.5f, 1f);
			break;
		case BossRaidManager.BossRaidChestRewardType.TreasureKey:
			rewardTargetIconImage.sprite = UIWindowLotteryBossRaidChest.instance.treasureKeyIconSprite;
			rewardCountText.text = rewardType.value.ToString("N0");
			rewardTargetIconImageRectTransform.localScale = new Vector3(2f, 2f, 1f);
			break;
		}
		rewardTargetIconImage.SetNativeSize();
	}
}
