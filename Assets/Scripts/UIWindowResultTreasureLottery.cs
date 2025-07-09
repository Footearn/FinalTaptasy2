using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowResultTreasureLottery : UIWindow
{
	public static UIWindowResultTreasureLottery instance;

	public GameObject newTreasureObject;

	public GameObject duplicatedTreasureObject;

	public GameObject resultEffect;

	public Text[] nameTexts;

	public Text[] descriptionTexts;

	public Image[] treasureBackgrounds;

	public Image[] treasureImages;

	public Image flashBlock;

	public GameObject flashBlockObject;

	public GameObject lotteryRepeatButtonObject;

	public Text remainLotteryCountText;

	public GameObject treasureRepeatLotteryObject;

	public GameObject pvpIconObject;

	private TreasureInventoryData m_treasureInventoryData;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeClose()
	{
		if (m_treasureInventoryData.treasureType == TreasureManager.TreasureType.MonocleOfWiseGoddess)
		{
			Singleton<DailyRewardManager>.instance.checkTreasureDailyRewards();
		}
		if (m_treasureInventoryData.treasureType == TreasureManager.TreasureType.HeliosHarp || m_treasureInventoryData.treasureType == TreasureManager.TreasureType.CharmOfLunarGoddess)
		{
			Singleton<TreasureManager>.instance.refreshPMAndAMTreasureState();
		}
		resultEffect.SetActive(false);
		return base.OnBeforeClose();
	}

	public void openWithTreasureInformation(TreasureInventoryData targetTreasure, bool isDuplicatedTreasure, bool isRepeatLottery)
	{
		pvpIconObject.SetActive(Singleton<TreasureManager>.instance.isPVPTreasure(targetTreasure.treasureType));
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode)
		{
			if (lotteryRepeatButtonObject.activeSelf)
			{
				lotteryRepeatButtonObject.SetActive(false);
			}
		}
		else if (Singleton<DataManager>.instance.currentGameData.treasurePiece >= Singleton<TreasureManager>.instance.currentLotteryPriceForTreasurePiece)
		{
			if (!lotteryRepeatButtonObject.activeSelf)
			{
				lotteryRepeatButtonObject.SetActive(true);
			}
			remainLotteryCountText.text = string.Format(I18NManager.Get("REMAIN_TEXT"), ((int)(Singleton<DataManager>.instance.currentGameData.treasurePiece / Singleton<TreasureManager>.instance.currentLotteryPriceForTreasurePiece)).ToString("N0"));
		}
		else if (lotteryRepeatButtonObject.activeSelf)
		{
			lotteryRepeatButtonObject.SetActive(false);
		}
		m_treasureInventoryData = targetTreasure;
		for (int i = 0; i < treasureImages.Length; i++)
		{
			treasureImages[i].sprite = Singleton<TreasureManager>.instance.getTreasureSprite(targetTreasure.treasureType);
			treasureImages[i].SetNativeSize();
		}
		for (int j = 0; j < treasureBackgrounds.Length; j++)
		{
			treasureBackgrounds[j].sprite = Singleton<TreasureManager>.instance.tierBackgroundSprites[Singleton<TreasureManager>.instance.getTreasureTier(targetTreasure.treasureType)];
			treasureBackgrounds[j].SetNativeSize();
		}
		for (int k = 0; k < nameTexts.Length; k++)
		{
			nameTexts[k].text = Singleton<TreasureManager>.instance.getTreasureI18NName(targetTreasure.treasureType);
		}
		descriptionTexts[0].text = Singleton<TreasureManager>.instance.getTreasureDescription(targetTreasure.treasureType, targetTreasure.treasureEffectValue + targetTreasure.extraTreasureEffectValue, 0.0);
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
		resultEffect.SetActive(true);
		if (!isDuplicatedTreasure)
		{
			newTreasureObject.SetActive(true);
			duplicatedTreasureObject.SetActive(false);
		}
		else
		{
			TreasureManager.TreasureType treasureType = targetTreasure.treasureType;
			newTreasureObject.SetActive(false);
			duplicatedTreasureObject.SetActive(true);
			Text obj = nameTexts[1];
			string text = obj.text;
			obj.text = text + " <color=#FAD725>Lv." + targetTreasure.treasureLevel + "</color>";
			descriptionTexts[1].text = Singleton<TreasureManager>.instance.getTreasureDescription(targetTreasure.treasureType, targetTreasure.treasureEffectValue + targetTreasure.extraTreasureEffectValue, targetTreasure.treasureEffectValue + targetTreasure.extraTreasureEffectValue - (targetTreasure.treasureEffectValue + targetTreasure.extraTreasureEffectValue - Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[treasureType].increasingValueEveryEnchant));
		}
		open();
		treasureRepeatLotteryObject.SetActive(false);
		if (!isRepeatLottery)
		{
			flashBlockObject.SetActive(true);
			flashBlock.color = new Color(1f, 1f, 1f, 1f);
			StopCoroutine("flashUpdate");
			StartCoroutine("flashUpdate");
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("treasure_open_short");
			treasureRepeatLotteryObject.SetActive(true);
			flashBlockObject.SetActive(false);
		}
	}

	public void OnClickLotteryRepeat()
	{
		if (Singleton<DataManager>.instance.currentGameData.treasurePiece >= Singleton<TreasureManager>.instance.currentLotteryPriceForTreasurePiece)
		{
			TreasureManager.TreasureType randomLotteryTreasureType = Singleton<TreasureManager>.instance.getRandomLotteryTreasureType();
			Singleton<TreasureManager>.instance.decreaseTreasurePiece(Singleton<TreasureManager>.instance.currentLotteryPriceForTreasurePiece);
			Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.TreasurePro, 1.0);
			Singleton<DataManager>.instance.currentGameData.treasureLotteryCount++;
			AnalyzeManager.retention(AnalyzeManager.CategoryType.Treasure, AnalyzeManager.ActionType.LotteryTreasure, new Dictionary<string, string>
			{
				{
					"TreasureType",
					randomLotteryTreasureType.ToString()
				}
			});
			Singleton<TreasureManager>.instance.treasureLotteryEvent(randomLotteryTreasureType, true);
		}
	}

	private IEnumerator flashUpdate()
	{
		yield return new WaitForSeconds(0.3f);
		Color color;
		while (true)
		{
			color = flashBlock.color;
			color.a -= Time.deltaTime * GameManager.timeScale * 0.7f;
			flashBlock.color = color;
			if (color.a <= 0f)
			{
				break;
			}
			yield return null;
		}
		color.a = 0f;
		flashBlock.color = color;
		flashBlockObject.SetActive(false);
	}
}
