using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using UnityEngine;

public class UIWindowJackPot : UIWindow
{
	public static UIWindowJackPot instance;

	public GameObject sunBurstEffectObject;

	public GameObject closeButtonObject;

	public float intervalBetweenNumber = 205f;

	public JackpotObjectData[] jackpotNumberObjects;

	public List<JackpotObjectData> currentNumberList;

	public List<int> jackpotOrder;

	public RectTransform content;

	public GameObject beforeStartObject;

	public GameObject afterJackpotObject;

	public ChangeNumberAnimate goldRewardText;

	public int targetNumberIndex;

	public RectTransform rewardButtonTransform;

	public RectTransform justRewardButtonTransform;

	public Transform ingameGoldIconTransform;

	public SpriteAnimation lightAnimation;

	public Transform goldIconTransform;

	public int currentDisplayedNumber;

	public ParticleSystem jackpotEndEffect;

	public GameObject bigJackpotEffectObject;

	public bool isJackpotOn;

	private JackpotObjectData m_currentTargetNumber;

	private double m_targetRewardGoldValue;

	private int m_randomJackpotValue;

	private MersenneTwister m_randomForJackpot = new MersenneTwister();

	private int m_uniqueIndex;

	private int index = 4;

	public override void Awake()
	{
		instance = this;
		currentNumberList = new List<JackpotObjectData>();
		jackpotOrder = new List<int>();
		base.Awake();
	}

	public void openJackpotUI(double goldValue)
	{
		UIWindowResult.instance.pauseNextTimer();
		isJackpotOn = true;
		m_uniqueIndex = 0;
		isCanCloseESC = true;
		index = 4;
		closeButtonObject.SetActive(true);
		jackpotEndEffect.Stop();
		bigJackpotEffectObject.SetActive(false);
		StopAllCoroutines();
		currentNumberList.Clear();
		jackpotOrder.Clear();
		m_targetRewardGoldValue = goldValue;
		m_randomJackpotValue = getRandomJackpotValue();
		content.anchoredPosition = Vector2.zero;
		goldRewardText.CurrentPrintType = ChangeNumberAnimate.PrintType.ChangeUnit;
		goldRewardText.SetText(m_targetRewardGoldValue);
		for (int i = 0; i < jackpotNumberObjects.Length; i++)
		{
			jackpotNumberObjects[i].cachedRectTransform.anchoredPosition = Vector2.zero;
			jackpotNumberObjects[i].cachedRectTransform.localScale = Vector3.one;
		}
		afterJackpotObject.SetActive(false);
		beforeStartObject.SetActive(true);
		currentDisplayedNumber = 1;
		jackpotOrder.Add(2);
		jackpotOrder.Add(100);
		jackpotOrder.Add(99);
		jackpotOrder.Add(98);
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, double> jackpotChanceDatum in Singleton<ParsingManager>.instance.currentParsedStatData.jackpotChanceData)
		{
			if (jackpotChanceDatum.Value > 0.0)
			{
				list.Add(jackpotChanceDatum.Key);
			}
		}
		for (int j = 0; j < 40; j++)
		{
			int num;
			while (true)
			{
				IL_01ad:
				num = list[Random.Range(0, list.Count - 1)];
				if (j >= 36 && num == m_randomJackpotValue)
				{
					continue;
				}
				for (int k = jackpotOrder.Count - 4; k < jackpotOrder.Count; k++)
				{
					if (jackpotOrder[k] == num)
					{
						goto IL_01ad;
					}
				}
				break;
			}
			jackpotOrder.Add(num);
		}
		targetNumberIndex = jackpotOrder.Count - 1;
		jackpotOrder.Add(m_randomJackpotValue);
		for (int l = 0; l < 4; l++)
		{
			int num2;
			while (true)
			{
				IL_026b:
				num2 = list[Random.Range(0, list.Count - 1)];
				for (int m = jackpotOrder.Count - 4; m < jackpotOrder.Count; m++)
				{
					if (jackpotOrder[m] == num2)
					{
						goto IL_026b;
					}
				}
				break;
			}
			jackpotOrder.Add(num2);
		}
		for (int n = 0; n < 4; n++)
		{
			addNumberInJackpotForInitialize(jackpotOrder[n]);
		}
		open();
		lightAnimation.playAnimation("JackpotLight", 0.2f);
	}

	private void addNumberInJackpotForInitialize(int targetNumberIndex)
	{
		if (currentNumberList.Count != 0)
		{
			jackpotNumberObjects[m_uniqueIndex].cachedRectTransform.anchoredPosition = new Vector2(0f, currentNumberList[currentNumberList.Count - 1].cachedRectTransform.anchoredPosition.y + intervalBetweenNumber);
		}
		else
		{
			jackpotNumberObjects[m_uniqueIndex].cachedRectTransform.anchoredPosition = new Vector2(0f, 0f - intervalBetweenNumber);
		}
		jackpotNumberObjects[m_uniqueIndex].jackpotNumberText.text = targetNumberIndex.ToString("N0");
		currentNumberList.Add(jackpotNumberObjects[m_uniqueIndex]);
		m_uniqueIndex++;
	}

	private void addNumberInJackpotForProgressing(int targetNumber)
	{
		JackpotObjectData jackpotObjectData = currentNumberList[0];
		jackpotObjectData.cachedRectTransform.anchoredPosition = new Vector2(0f, currentNumberList[currentNumberList.Count - 1].cachedRectTransform.anchoredPosition.y + intervalBetweenNumber);
		jackpotObjectData.jackpotNumberText.text = targetNumber.ToString("N0");
		currentNumberList.Remove(jackpotObjectData);
		currentNumberList.Add(jackpotObjectData);
		if (targetNumber == m_randomJackpotValue)
		{
			m_currentTargetNumber = jackpotObjectData;
		}
	}

	public void onClickPlayJackpot()
	{
		isCanCloseESC = false;
		if (!Singleton<AdsManager>.instance.isReady("jackpotReward"))
		{
			return;
		}
		Singleton<AdsManager>.instance.showAds("jackpotReward", delegate
		{
			if (instance != null)
			{
				sunBurstEffectObject.SetActive(true);
				closeButtonObject.SetActive(false);
				lightAnimation.playAnimation("JackpotLight", 0.09f);
				beforeStartObject.SetActive(false);
				AnalyzeManager.retention(AnalyzeManager.CategoryType.WatchAd, AnalyzeManager.ActionType.JackPotAd);
				StopAllCoroutines();
				StartCoroutine("jackpotUpdate");
			}
		});
	}

	public void onClickGetRewardAfterJackpot()
	{
		double num = m_targetRewardGoldValue * (double)m_randomJackpotValue;
		num += num / 100.0 * Singleton<StatManager>.instance.percentJackPotRewardGain;
		sunBurstEffectObject.SetActive(false);
		jackpotEndEffect.Stop();
		bigJackpotEffectObject.SetActive(false);
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.ProGambler, 1.0);
		UIWindowResult.instance.disableJackpotUI();
		Singleton<GoldManager>.instance.increaseGold(num);
		Dictionary<DropItemManager.DropItemType, double> totalIngameGainDropItem;
		Dictionary<DropItemManager.DropItemType, double> dictionary = (totalIngameGainDropItem = Singleton<DropItemManager>.instance.totalIngameGainDropItem);
		DropItemManager.DropItemType key;
		DropItemManager.DropItemType key2 = (key = DropItemManager.DropItemType.Gold);
		double num2 = totalIngameGainDropItem[key];
		dictionary[key2] = num2 + num;
		UIWindowResult.instance.goldText.SetValue(Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.Gold], 2f, 0.6f);
		Singleton<FlyResourcesManager>.instance.playEffectResources(rewardButtonTransform, ingameGoldIconTransform, FlyResourcesManager.ResourceType.Gold, 35L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		}, 1f);
		AnalyzeManager.retention(AnalyzeManager.CategoryType.Gold, AnalyzeManager.ActionType.JackPot, new Dictionary<string, string>
		{
			{
				"Multiply",
				m_randomJackpotValue.ToString()
			},
			{
				"Gold",
				(m_targetRewardGoldValue * (double)m_randomJackpotValue).ToString()
			}
		});
		close();
		if (m_randomJackpotValue >= 10 && NSPlayerPrefs.GetInt("Review", 0) <= 1)
		{
			UIWindowReview.instance.open();
		}
	}

	public override bool OnBeforeClose()
	{
		sunBurstEffectObject.SetActive(false);
		jackpotEndEffect.Stop();
		bigJackpotEffectObject.SetActive(false);
		return base.OnBeforeClose();
	}

	public override void OnAfterClose()
	{
		isJackpotOn = false;
		if (UIWindowResult.instance.isClear)
		{
			UIWindowResult.instance.startNextTimer();
		}
		base.OnAfterClose();
	}

	private IEnumerator endJackpotUpdate()
	{
		lightAnimation.playAnimation("JackpotLight", 0.11f);
		m_currentTargetNumber.cachedRectTransform.GetComponent<Animation>().Play();
		Singleton<AudioManager>.instance.playEffectSound("monster_blowup");
		double targetJackpotRewardValue2 = m_targetRewardGoldValue * (double)m_randomJackpotValue;
		targetJackpotRewardValue2 += targetJackpotRewardValue2 / 100.0 * Singleton<StatManager>.instance.percentJackPotRewardGain;
		goldRewardText.SetValue(targetJackpotRewardValue2, 1f, delegate
		{
		});
		float timer = 0f;
		while (true)
		{
			Singleton<AudioManager>.instance.playEffectSound("getresource", AudioManager.EffectType.Resource);
			timer += Time.deltaTime;
			if (timer >= 1f)
			{
				break;
			}
			yield return null;
		}
		afterJackpotObject.SetActive(true);
	}

	private IEnumerator waitForSoundPlay()
	{
		yield return new WaitForSeconds(0.43f);
		Singleton<AudioManager>.instance.playEffectSound("jackpot");
	}

	private IEnumerator jackpotUpdate()
	{
		float tempYMovedPosition = 0f;
		Vector2 position;
		while (true)
		{
			if (index < jackpotOrder.Count)
			{
				position = content.anchoredPosition;
				float acceleration2 = 0f;
				acceleration2 = ((!(Mathf.Abs((float)targetNumberIndex * (0f - intervalBetweenNumber) - position.y) < 15f)) ? 1f : 40f);
				position.y = Mathf.Lerp(position.y, (float)targetNumberIndex * (0f - intervalBetweenNumber), Time.deltaTime * acceleration2);
				if (content.anchoredPosition.y - position.y > 70f)
				{
					position.y = content.anchoredPosition.y - 70f;
				}
				tempYMovedPosition += content.anchoredPosition.y - position.y;
				if (tempYMovedPosition >= intervalBetweenNumber)
				{
					addNumberInJackpotForProgressing(jackpotOrder[index]);
					tempYMovedPosition -= intervalBetweenNumber;
					currentDisplayedNumber = jackpotOrder[index - 1];
					index++;
					position.y = (float)(index - 3) * (0f - intervalBetweenNumber);
					if (index == targetNumberIndex + 2)
					{
						StartCoroutine("waitForSoundPlay");
					}
					if (index == targetNumberIndex + 3)
					{
						break;
					}
					Singleton<AudioManager>.instance.playEffectSound("jackpot");
				}
				content.anchoredPosition = position;
			}
			yield return null;
		}
		position.y = (float)targetNumberIndex * (0f - intervalBetweenNumber);
		content.anchoredPosition = position;
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		jackpotEndEffect.Stop();
		jackpotEndEffect.Play();
		if (m_randomJackpotValue >= 10)
		{
			bigJackpotEffectObject.SetActive(true);
		}
		StopAllCoroutines();
		StartCoroutine("endJackpotUpdate");
	}

	private int getRandomJackpotValue()
	{
		int result = 0;
		List<int> list = new List<int>();
		List<double> list2 = new List<double>();
		foreach (KeyValuePair<int, double> jackpotChanceDatum in Singleton<ParsingManager>.instance.currentParsedStatData.jackpotChanceData)
		{
			list.Add(jackpotChanceDatum.Key);
			list2.Add(jackpotChanceDatum.Value);
		}
		double num = m_randomForJackpot.Next(0, 10000);
		num /= 100.0;
		double num2 = 0.0;
		for (int i = 0; i < list2.Count; i++)
		{
			num2 += list2[i];
			if (num2 > num)
			{
				result = list[i];
				break;
			}
		}
		return result;
	}
}
