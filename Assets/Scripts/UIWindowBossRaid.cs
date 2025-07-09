using UnityEngine;
using UnityEngine.UI;

public class UIWindowBossRaid : UIWindow
{
	public static UIWindowBossRaid instance;

	public Image[] heartImages;

	public Text stageText;

	public Text levelText;

	public Text monsterNameText;

	public Text timerText;

	public Image monsterImage;

	public GameObject haveBestRecordObject;

	public GameObject noHaveBestRecordObject;

	public RectTransform enterButtonTransform;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void OnClickOpenClosedBossRaidUI()
	{
		UIWindow.Get("UI BossRaidDialog").open();
	}

	public void openBossRaidUI()
	{
		if (Singleton<DataManager>.instance.currentGameData.unlockTheme > 2)
		{
			Singleton<BossRaidManager>.instance.resetAll();
			Singleton<BossRaidManager>.instance.displayHeart();
			for (int i = 0; i < heartImages.Length; i++)
			{
				heartImages[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < BossRaidManager.maxHeart; j++)
			{
				heartImages[j].gameObject.SetActive(true);
			}
			BossRaidManager.BossRaidBestRecordData bossRaidBestRecord = Singleton<DataManager>.instance.currentGameData.bossRaidBestRecord;
			if (bossRaidBestRecord.isInitialized)
			{
				haveBestRecordObject.SetActive(true);
				noHaveBestRecordObject.SetActive(false);
				stageText.text = "stage " + bossRaidBestRecord.stage;
				levelText.text = "lv." + bossRaidBestRecord.monsterLevel;
				if (bossRaidBestRecord.isMiniBossMonster)
				{
					monsterNameText.text = I18NManager.Get(bossRaidBestRecord.monsterType.ToString().ToUpper() + "_NAME");
					monsterImage.sprite = Singleton<EnemyManager>.instance.getMonsterIconSprite(bossRaidBestRecord.monsterType);
					monsterImage.transform.localScale = new Vector3(4f, 4f, 1f);
				}
				else if (bossRaidBestRecord.isBossMonster)
				{
					monsterNameText.text = I18NManager.Get(bossRaidBestRecord.bossType.ToString().ToUpper() + "_NAME");
					monsterImage.sprite = Singleton<EnemyManager>.instance.getBossIconSprite(bossRaidBestRecord.bossType, true);
					monsterImage.transform.localScale = new Vector3(2f, 2f, 1f);
				}
				monsterImage.SetNativeSize();
			}
			else
			{
				haveBestRecordObject.SetActive(false);
				noHaveBestRecordObject.SetActive(true);
			}
			open();
		}
		else
		{
			UIWindow.Get("UI BossRaidDialog").open();
		}
	}

	public void OnClickChargeHeart()
	{
		if (Singleton<DataManager>.instance.currentGameData.heartForBossRaid < BossRaidManager.maxHeart)
		{
			if ((long)Singleton<DataManager>.instance.currentGameData.obscuredRuby >= 20)
			{
				Singleton<RubyManager>.instance.decreaseRuby(20.0);
				AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.GetFreeHeartByDiamond);
				Singleton<BossRaidManager>.instance.increaseHeart(1, false);
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, string.Empty);
			}
		}
		else
		{
			UIWindowDialog.openDescription("ALREADY_MAX_HEART", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}
}
