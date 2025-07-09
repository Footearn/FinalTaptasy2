using UnityEngine;
using UnityEngine.UI;

public class UIWindowTowerModeRankingReward : UIWindow
{
	public static UIWindowTowerModeRankingReward instance;

	public GameObject sunburstEffectObject;

	public Text nameText;

	public CanvasGroup cachedCanvasGroup;

	public CharacterUIObject characterUIObject;

	public GameObject treasureObject;

	public GameObject characterSkinObject;

	public Image treasureIconImage;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openRankingReward(TowerModeManager.TowerModeRankingRewardType rewardType)
	{
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		sunburstEffectObject.SetActive(true);
		treasureObject.SetActive(false);
		characterSkinObject.SetActive(false);
		switch (rewardType)
		{
		case TowerModeManager.TowerModeRankingRewardType.Treasure_ConquerToken:
			nameText.text = Singleton<TreasureManager>.instance.getTreasureI18NName(TreasureManager.TreasureType.ConquerToken);
			treasureIconImage.sprite = Singleton<TreasureManager>.instance.getTreasureSprite(TreasureManager.TreasureType.ConquerToken);
			treasureObject.SetActive(true);
			break;
		case TowerModeManager.TowerModeRankingRewardType.Treasure_PatienceToken:
			nameText.text = Singleton<TreasureManager>.instance.getTreasureI18NName(TreasureManager.TreasureType.PatienceToken);
			treasureIconImage.sprite = Singleton<TreasureManager>.instance.getTreasureSprite(TreasureManager.TreasureType.PatienceToken);
			treasureObject.SetActive(true);
			break;
		case TowerModeManager.TowerModeRankingRewardType.WarriorSkin:
			nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(CharacterSkinManager.WarriorSkinType.MasterWilliam);
			characterSkinObject.SetActive(true);
			characterUIObject.initCharacterUIObject(CharacterSkinManager.WarriorSkinType.MasterWilliam, cachedCanvasGroup);
			break;
		case TowerModeManager.TowerModeRankingRewardType.PriestSkin:
			nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(CharacterSkinManager.PriestSkinType.MasterOlivia);
			characterSkinObject.SetActive(true);
			characterUIObject.initCharacterUIObject(CharacterSkinManager.PriestSkinType.MasterOlivia, cachedCanvasGroup);
			break;
		case TowerModeManager.TowerModeRankingRewardType.ArcherSkin:
			nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(CharacterSkinManager.ArcherSkinType.MasterWindstoker);
			characterSkinObject.SetActive(true);
			characterUIObject.initCharacterUIObject(CharacterSkinManager.ArcherSkinType.MasterWindstoker, cachedCanvasGroup);
			break;
		}
		treasureIconImage.SetNativeSize();
		open();
	}

	public override bool OnBeforeClose()
	{
		sunburstEffectObject.SetActive(false);
		return base.OnBeforeClose();
	}
}
