using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowRoulette : UIWindow
{
	public static UIWindowRoulette instance;

	public Sprite bronzeRouletteCircleSprite;

	public Sprite goldRouletteCircleSprite;

	public Sprite bronzeRouletteSelectedSprite;

	public Sprite goldRouletteSelectedSprite;

	public GameObject bronzeRouletteObject;

	public GameObject goldRouletteObject;

	public SpriteAnimation lightSpriteAnimation;

	public Text[] goldRoulettePriceTexts;

	public RectTransform rouletteCircleTransform;

	public List<RouletteSlot> totalSlotList;

	public GameObject rouletteEndObject;

	public Image rouletteCircleImage;

	public Image rouletteSelctedImage;

	public GameObject descipriotnTextObject;

	public GameObject rewardButtonObject;

	public CanvasGroup rouletteObjectCanvasGroup;

	public GameObject bronzeRouletteTimerObject;

	public Text bronzeRouletteRemainTimeText;

	public Animation rouletteOpenAnimation;

	public Animation rouletteCircleAnimation;

	public float maxAcceleration = 1500f;

	public float rouletteAcceleration = 360f;

	public float currentAcceleration;

	public float minAcceleration = 200f;

	public Sprite bronzeRouletteSpinOnSprite;

	public Sprite bronzeRouletteSpinOffSprite;

	public Sprite goldRouletteSpinOnSprite;

	public Sprite goldRouletteSpinOffSprite;

	public Image bronzeRouletteSpinButtonImage;

	public GameObject bronzeRouletteSpinButton2Object;

	public Button[] bronzeSpinButtons;

	public Image goldRouletteSpinButtonImage;

	public GameObject goldRouletteSpinButton2Object;

	public Button[] goldSpinButtons;

	private bool m_isCanOpenInformation;

	private bool m_isCanStartRoulette;

	private bool m_isCanClose;

	private bool m_isBronzeRoulette;

	private bool m_isCanGetReward;

	private int m_targetInitSlotIndex;

	private RouletteManager.RouletteRewardData m_targetRewardData;

	private List<RouletteManager.RouletteRewardData> m_prevAppearedRandomRewardDataList = new List<RouletteManager.RouletteRewardData>();

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openRouletteUI(bool isBronzeRoulette, bool isOpenRepeat = false)
	{
		if (Singleton<DataManager>.instance.currentGameData.goldRouletteLastDayOfYear != UnbiasedTime.Instance.Now().DayOfYear)
		{
			Singleton<DataManager>.instance.currentGameData.goldRouletteLastDayOfYear = UnbiasedTime.Instance.Now().DayOfYear;
			Singleton<DataManager>.instance.currentGameData.goldRouletteRemainCount = 3;
			Singleton<DataManager>.instance.saveData();
		}
		if (isBronzeRoulette && !Singleton<RouletteManager>.instance.isCanStartBronzeRoulette())
		{
			isBronzeRoulette = false;
		}
		bronzeRouletteTimerObject.SetActive(!Singleton<RouletteManager>.instance.isCanStartBronzeRoulette());
		StopAllCoroutines();
		m_prevAppearedRandomRewardDataList.Clear();
		m_isCanOpenInformation = true;
		m_isCanStartRoulette = true;
		descipriotnTextObject.SetActive(true);
		rewardButtonObject.SetActive(false);
		rouletteEndObject.SetActive(false);
		currentAcceleration = 0f;
		m_isBronzeRoulette = isBronzeRoulette;
		isCanCloseESC = true;
		m_isCanClose = true;
		m_isCanGetReward = false;
		m_targetInitSlotIndex = 4;
		initAllSlot();
		string empty = string.Empty;
		refreshRouletteState(true, m_isBronzeRoulette);
		if (m_isBronzeRoulette)
		{
			rouletteCircleImage.sprite = bronzeRouletteCircleSprite;
			rouletteSelctedImage.sprite = bronzeRouletteSelectedSprite;
			bronzeRouletteObject.SetActive(true);
			goldRouletteObject.SetActive(false);
			empty = "BronzeRouletteLight";
			lightSpriteAnimation.cachedRectTransform.anchoredPosition = new Vector2(0f, -300.9f);
		}
		else
		{
			rouletteCircleImage.sprite = goldRouletteCircleSprite;
			rouletteSelctedImage.sprite = goldRouletteSelectedSprite;
			bronzeRouletteObject.SetActive(false);
			goldRouletteObject.SetActive(true);
			empty = "GoldRouletteLight";
			lightSpriteAnimation.cachedRectTransform.anchoredPosition = new Vector2(0f, -310f);
			ShopManager.LimitedItemData limitedItemData = Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.GoldRoulettePackage];
			string text = Singleton<PaymentManager>.instance.GetMarketPrice(limitedItemData.limitedType);
			if (Singleton<DataManager>.instance.currentGameData.goldRouletteTicket > 0)
			{
				text = "Free";
			}
			else if (text.Length < 1)
			{
				text = limitedItemData.itemPriceString;
			}
			for (int i = 0; i < goldRoulettePriceTexts.Length; i++)
			{
				goldRoulettePriceTexts[i].text = text;
			}
		}
		m_targetRewardData = Singleton<RouletteManager>.instance.getRandomRouletteData(m_isBronzeRoulette);
		rouletteCircleTransform.localEulerAngles = Vector3.zero;
		if (!isOpenRepeat)
		{
			rouletteObjectCanvasGroup.alpha = 1f;
			open();
		}
		else
		{
			rouletteObjectCanvasGroup.alpha = 0f;
			rouletteOpenAnimation.Stop();
			rouletteOpenAnimation.Play();
		}
		lightSpriteAnimation.playAnimation(empty, 0.5f, true);
	}

	private void refreshRouletteState(bool isOn, bool isBronze)
	{
		if (isOn)
		{
			if (isBronze)
			{
				for (int i = 0; i < bronzeSpinButtons.Length; i++)
				{
					bronzeSpinButtons[i].interactable = true;
				}
				bronzeRouletteSpinButtonImage.sprite = bronzeRouletteSpinOnSprite;
				bronzeRouletteSpinButton2Object.SetActive(true);
				return;
			}
			for (int j = 0; j < goldSpinButtons.Length; j++)
			{
				goldSpinButtons[j].interactable = true;
			}
			goldRouletteSpinButtonImage.sprite = goldRouletteSpinOnSprite;
			goldRouletteSpinButton2Object.SetActive(true);
			for (int k = 0; k < goldRoulettePriceTexts.Length; k++)
			{
				goldRoulettePriceTexts[k].color = Color.white;
			}
		}
		else if (isBronze)
		{
			for (int l = 0; l < bronzeSpinButtons.Length; l++)
			{
				bronzeSpinButtons[l].interactable = false;
			}
			bronzeRouletteSpinButtonImage.sprite = bronzeRouletteSpinOffSprite;
			bronzeRouletteSpinButton2Object.SetActive(false);
		}
		else
		{
			for (int m = 0; m < goldSpinButtons.Length; m++)
			{
				goldSpinButtons[m].interactable = false;
			}
			goldRouletteSpinButtonImage.sprite = goldRouletteSpinOffSprite;
			goldRouletteSpinButton2Object.SetActive(false);
			for (int n = 0; n < goldRoulettePriceTexts.Length; n++)
			{
				goldRoulettePriceTexts[n].color = Util.getCalculatedColor(121f, 121f, 121f);
			}
		}
	}

	private void initAllSlot()
	{
		if (m_isBronzeRoulette)
		{
			totalSlotList[0].initRouletteSlot(Singleton<RouletteManager>.instance.bronzeRouletteRewardList[6]);
			totalSlotList[1].initRouletteSlot(Singleton<RouletteManager>.instance.bronzeRouletteRewardList[12]);
			totalSlotList[2].initRouletteSlot(Singleton<RouletteManager>.instance.bronzeRouletteRewardList[9]);
			totalSlotList[3].initRouletteSlot(Singleton<RouletteManager>.instance.bronzeRouletteRewardList[11]);
			totalSlotList[9].initRouletteSlot(Singleton<RouletteManager>.instance.bronzeRouletteRewardList[3]);
			totalSlotList[8].initRouletteSlot(Singleton<RouletteManager>.instance.bronzeRouletteRewardList[5]);
		}
		else
		{
			totalSlotList[0].initRouletteSlot(Singleton<RouletteManager>.instance.goldRouletteRewardList[12]);
			totalSlotList[1].initRouletteSlot(Singleton<RouletteManager>.instance.goldRouletteRewardList[5]);
			totalSlotList[2].initRouletteSlot(Singleton<RouletteManager>.instance.goldRouletteRewardList[8]);
			totalSlotList[3].initRouletteSlot(Singleton<RouletteManager>.instance.goldRouletteRewardList[4]);
			totalSlotList[9].initRouletteSlot(Singleton<RouletteManager>.instance.goldRouletteRewardList[2]);
			totalSlotList[8].initRouletteSlot(Singleton<RouletteManager>.instance.goldRouletteRewardList[11]);
		}
	}

	private void Update()
	{
		if (!Singleton<RouletteManager>.instance.isCanStartBronzeRoulette())
		{
			TimeSpan timeSpan = new TimeSpan(new DateTime(Singleton<DataManager>.instance.currentGameData.bronzeRouletteTargetEndTime).Ticks - UnbiasedTime.Instance.UtcNow().Ticks);
			string text = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
			bronzeRouletteRemainTimeText.text = text;
		}
		else if (bronzeRouletteTimerObject.activeSelf)
		{
			bronzeRouletteTimerObject.SetActive(false);
		}
	}

	public void OnClickRewardInformation()
	{
		if (m_isCanOpenInformation)
		{
			UIWindowRouletteReward.instance.openRouletteUI(m_isBronzeRoulette);
		}
	}

	public void OnClickStartRoulette()
	{
		//TODO 转盘
		if (!m_isCanStartRoulette || (m_isBronzeRoulette && !Singleton<RouletteManager>.instance.isCanStartBronzeRoulette()))
		{
			return;
		}
		if (!m_isBronzeRoulette && Singleton<DataManager>.instance.currentGameData.goldRouletteRemainCount < 1)
		{
			UIWindowDialog.openMiniDialogWithoutI18N(I18NManager.Get("GOLD_ROULETTE_DESCRIPTION"));
			return;
		}
		Action rouletteAction = delegate
		{
			refreshRouletteState(false, m_isBronzeRoulette);
			m_isCanStartRoulette = false;
			isCanCloseESC = false;
			m_isCanClose = false;
			m_isCanOpenInformation = false;
			StopAllCoroutines();
			StartCoroutine("rouletteUpdate");
			Singleton<RouletteManager>.instance.doRewardGetEvent(m_targetRewardData);
			if (!m_isBronzeRoulette)
			{
				Singleton<DataManager>.instance.currentGameData.goldRouletteRemainCount--;
				Singleton<DataManager>.instance.saveData();
			}
		};
		bool flag = false;
		if (!m_isBronzeRoulette)
		{
			if (Singleton<DataManager>.instance.currentGameData.goldRouletteTicket > 0)
			{
				Singleton<DataManager>.instance.currentGameData.goldRouletteTicket--;
				flag = true;
			}
			else
			{
				Singleton<PaymentManager>.instance.Purchase(ShopManager.LimitedShopItemType.GoldRoulettePackage, delegate
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.GoldRoulettePackage], null)();
					Singleton<DataManager>.instance.currentGameData.goldRouletteTicket--;
					rouletteAction();
				});
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			rouletteAction();
		}
	}

	private IEnumerator rouletteUpdate()
	{
		Singleton<AudioManager>.instance.playEffectSound("roulette", AudioManager.EffectType.Princess);
		float totalChangedZRotation = 0f;
		float currentZRotation2 = 0f;
		while (true)
		{
			currentAcceleration = Mathf.Min(currentAcceleration + rouletteAcceleration * Time.deltaTime, maxAcceleration);
			lightSpriteAnimation.duration = Mathf.Min((1f - currentAcceleration / maxAcceleration) / 2f, 0.08f);
			currentZRotation2 += currentAcceleration * Time.deltaTime;
			totalChangedZRotation += currentAcceleration * Time.deltaTime;
			while (totalChangedZRotation > 72f)
			{
				Singleton<AudioManager>.instance.playEffectSound("roulette", AudioManager.EffectType.Princess);
				totalChangedZRotation -= 72f;
				refreshSlot(m_targetInitSlotIndex, false);
				m_targetInitSlotIndex++;
				if (m_targetInitSlotIndex >= totalSlotList.Count)
				{
					m_targetInitSlotIndex = 0;
				}
			}
			while (currentZRotation2 > 360f)
			{
				currentZRotation2 -= 360f;
			}
			rouletteCircleTransform.localEulerAngles = new Vector3(0f, 0f, currentZRotation2);
			if (currentAcceleration >= maxAcceleration)
			{
				break;
			}
			yield return null;
		}
		lightSpriteAnimation.duration = 0.08f;
		while (true)
		{
			if (currentZRotation2 > 360f)
			{
				currentZRotation2 -= 360f;
				continue;
			}
			if (currentAcceleration > minAcceleration)
			{
				currentAcceleration = Mathf.Max(currentAcceleration - Time.deltaTime * rouletteAcceleration * 1.5f, minAcceleration);
				currentZRotation2 += currentAcceleration * Time.deltaTime;
				totalChangedZRotation += currentAcceleration * Time.deltaTime;
				while (totalChangedZRotation > 72f)
				{
					Singleton<AudioManager>.instance.playEffectSound("roulette", AudioManager.EffectType.Princess);
					totalChangedZRotation -= 72f;
					refreshSlot(m_targetInitSlotIndex, false);
					m_targetInitSlotIndex++;
					if (m_targetInitSlotIndex >= totalSlotList.Count)
					{
						m_targetInitSlotIndex = 0;
					}
				}
			}
			else
			{
				if (currentZRotation2 >= 342f)
				{
					break;
				}
				currentZRotation2 = Mathf.Min(currentZRotation2 + currentAcceleration * Time.deltaTime, 342f);
				totalChangedZRotation += currentAcceleration * Time.deltaTime;
				while (totalChangedZRotation > 72f)
				{
					Singleton<AudioManager>.instance.playEffectSound("roulette", AudioManager.EffectType.Princess);
					totalChangedZRotation -= 72f;
					if (currentZRotation2 >= 252f)
					{
						refreshSlot(0, true);
					}
					else
					{
						refreshSlot(m_targetInitSlotIndex, false);
					}
					m_targetInitSlotIndex++;
					if (m_targetInitSlotIndex >= totalSlotList.Count)
					{
						m_targetInitSlotIndex = 0;
					}
				}
			}
			rouletteCircleTransform.localEulerAngles = new Vector3(0f, 0f, currentZRotation2);
			yield return null;
		}
		currentZRotation2 = 342f;
		rouletteCircleTransform.localEulerAngles = new Vector3(0f, 0f, currentZRotation2);
		refreshSlot(0, true);
		while (true)
		{
			currentZRotation2 = Mathf.Min(currentZRotation2 + currentAcceleration * Time.deltaTime, 360f);
			for (totalChangedZRotation += currentAcceleration * Time.deltaTime; totalChangedZRotation > 72f; totalChangedZRotation -= 72f)
			{
				Singleton<AudioManager>.instance.playEffectSound("roulette", AudioManager.EffectType.Princess);
			}
			rouletteCircleTransform.localEulerAngles = new Vector3(0f, 0f, currentZRotation2);
			if (currentZRotation2 >= 360f)
			{
				break;
			}
			yield return null;
		}
		rouletteCircleAnimation.Stop();
		rouletteCircleAnimation.Play();
		Singleton<AudioManager>.instance.playEffectSound("roulette_pick");
		yield return new WaitForSeconds(0.78f);
		if (m_isBronzeRoulette)
		{
			Singleton<DataManager>.instance.currentGameData.bronzeRouletteTargetEndTime = UnbiasedTime.Instance.UtcNow().AddMinutes(RouletteManager.intervalBronzeRouletteMinutes).Ticks;
			Singleton<DataManager>.instance.saveData();
		}
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
		lightSpriteAnimation.duration = 0.2f;
		rouletteEndObject.SetActive(true);
		descipriotnTextObject.SetActive(false);
		rewardButtonObject.SetActive(true);
		m_isCanOpenInformation = true;
		m_isCanGetReward = true;
	}

	private void refreshSlot(int index, bool isSelectedSlot)
	{
		RouletteManager.RouletteRewardData rouletteRewardData = null;
		rouletteRewardData = ((!isSelectedSlot) ? getRandomRewardData(m_isBronzeRoulette) : m_targetRewardData);
		if (rouletteRewardData != null)
		{
			totalSlotList[index].initRouletteSlot(rouletteRewardData);
		}
	}

	private RouletteManager.RouletteRewardData getRandomRewardData(bool isBronzeRoulette)
	{
		RouletteManager.RouletteRewardData rouletteRewardData = null;
		int num = 0;
		if (isBronzeRoulette)
		{
			num = Singleton<RouletteManager>.instance.bronzeRouletteRewardList.Count;
			rouletteRewardData = Singleton<RouletteManager>.instance.bronzeRouletteRewardList[UnityEngine.Random.Range(0, num)];
		}
		else
		{
			num = Singleton<RouletteManager>.instance.goldRouletteRewardList.Count;
			rouletteRewardData = Singleton<RouletteManager>.instance.goldRouletteRewardList[UnityEngine.Random.Range(0, num)];
		}
		if (num >= 10 && (m_prevAppearedRandomRewardDataList.Contains(rouletteRewardData) || rouletteRewardData == m_targetRewardData))
		{
			rouletteRewardData = getRandomRewardData(isBronzeRoulette);
		}
		else
		{
			if (m_prevAppearedRandomRewardDataList.Count >= 10)
			{
				m_prevAppearedRandomRewardDataList.RemoveAt(0);
			}
			m_prevAppearedRandomRewardDataList.Add(rouletteRewardData);
		}
		return rouletteRewardData;
	}

	public override bool OnBeforeClose()
	{
		rouletteEndObject.SetActive(false);
		return base.OnBeforeClose();
	}

	public void OnClickClose()
	{
		if (m_isCanClose)
		{
			m_isCanClose = false;
			close();
		}
	}

	public void OnClickReward()
	{
		if (!m_isCanGetReward)
		{
			return;
		}
		m_isCanGetReward = false;
		Action endAction = null;
		if (m_isBronzeRoulette)
		{
			endAction = delegate
			{
				openRouletteUI(false, true);
			};
		}
		else if (Singleton<DataManager>.instance.currentGameData.goldRouletteTicket > 0 || Singleton<DataManager>.instance.currentGameData.goldRouletteRemainCount > 0)
		{
			endAction = delegate
			{
				openRouletteUI(false, true);
			};
		}
		else
		{
			close();
		}
		if (m_targetRewardData != null)
		{
			Singleton<RouletteManager>.instance.doRewardResultEvent(m_targetRewardData, endAction);
		}
	}
}
