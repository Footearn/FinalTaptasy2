using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowManageTreasure : UIWindow
{
	public static UIWindowManageTreasure instance;

	public List<TreasureCollectionSlotSet> currentTreasureSlots;

	public ChangeNumberAnimate rarePieceCountText;

	public Text treasureLotteryPriceText;

	public RectTransform content;

	public RectTransform scrollRect;

	public ScrollRect scrollRectObject;

	public GameObject lotteryByKeyEffect;

	public GameObject elopeShopIndicator;

	public GameObject rebirthIndigator;

	public Text totalTreasureCountText;

	public Text treasureTotalDamageDescriptionText;

	public GameObject rebirthUnlockableObject;

	private List<TreasureCollectionSlotSet> m_allTreasureSlot;

	private float m_intervalBetweenCell = 140f;

	private bool m_isInitScrollType;

	public override void Awake()
	{
		m_allTreasureSlot = new List<TreasureCollectionSlotSet>();
		instance = this;
		scrollRectObject.vertical = true;
		base.Awake();
		instance.rarePieceCountText.CurrentPrintType = ChangeNumberAnimate.PrintType.Number;
		I18NManager.changeLanguageNotification = (Action)Delegate.Combine(I18NManager.changeLanguageNotification, new Action(changeLanguageNotification));
	}

	public void setScrollType()
	{
	}

	private void changeLanguageNotification()
	{
		refreshSlots();
	}

	private void Start()
	{
		createTreasureSlots();
	}

	private void OnEnable()
	{
		content.anchoredPosition = Vector2.zero;
		Singleton<TreasureManager>.instance.displayTreasurePiece(false);
		Singleton<TreasureManager>.instance.displayTreasureEnchantStone();
		StopCoroutine("disableUpdate");
		StartCoroutine("disableUpdate");
	}

	public override bool OnBeforeOpen()
	{
		if (Singleton<DataManager>.instance.currentGameData.currentTheme > Singleton<RebirthManager>.instance.currentRebirthRequireTheme)
		{
			rebirthUnlockableObject.SetActive(true);
		}
		else
		{
			rebirthUnlockableObject.SetActive(false);
		}
		Singleton<TreasureManager>.instance.refreshTreasureInventoryData();
		return base.OnBeforeOpen();
	}

	public void createTreasureSlots(bool isSortByTier = true)
	{
		Singleton<TreasureManager>.instance.refreshTreasureInventoryData();
		for (int i = 0; i < currentTreasureSlots.Count; i++)
		{
			if (currentTreasureSlots[i].isDisabled)
			{
				enableSlot(currentTreasureSlots[i]);
			}
			ObjectPool.Recycle("@TreasureSlotSet", currentTreasureSlots[i].cachedGameObject);
		}
		m_allTreasureSlot.Clear();
		currentTreasureSlots.Clear();
		float num = -52.3f;
		List<TreasureInventoryData> list = null;
		if (isSortByTier)
		{
			list = getSortedTreasureData(Singleton<DataManager>.instance.currentGameData.treasureInventoryData);
		}
		else
		{
			list = Singleton<DataManager>.instance.currentGameData.treasureInventoryData;
		}
		int num2 = (int)Math.Ceiling(10.399999618530273);
		for (int j = 0; j < num2; j++)
		{
			TreasureCollectionSlotSet component = ObjectPool.Spawn("@TreasureSlotSet", new Vector2(0f, num), content).GetComponent<TreasureCollectionSlotSet>();
			component.cachedGameObject.name = "@TreasureSlotSet" + (j + 1);
			component.cachedRectTransform.localScale = Vector2.one;
			component.refreshSlot();
			m_allTreasureSlot.Add(component);
			currentTreasureSlots.Add(component);
			num -= m_intervalBetweenCell;
		}
		content.sizeDelta = new Vector2(720f, Mathf.Abs(num) + 10f);
		refreshSlots();
	}

	private List<TreasureInventoryData> getSortedTreasureData(List<TreasureInventoryData> treasureInventoryData)
	{
		List<TreasureInventoryData> list = new List<TreasureInventoryData>();
		List<TreasureInventoryData> list2 = new List<TreasureInventoryData>();
		List<TreasureInventoryData> list3 = new List<TreasureInventoryData>();
		List<TreasureInventoryData> list4 = new List<TreasureInventoryData>();
		for (int i = 0; i < treasureInventoryData.Count; i++)
		{
			switch (Singleton<TreasureManager>.instance.getTreasureTier(treasureInventoryData[i].treasureType))
			{
			case 1:
				list2.Add(Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i]);
				break;
			case 2:
				list3.Add(Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i]);
				break;
			case 3:
				list4.Add(Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i]);
				break;
			}
		}
		for (int j = 0; j < list2.Count; j++)
		{
			list.Add(list2[j]);
		}
		for (int k = 0; k < list3.Count; k++)
		{
			list.Add(list3[k]);
		}
		for (int l = 0; l < list4.Count; l++)
		{
			list.Add(list4[l]);
		}
		return list;
	}

	public void refreshSlots()
	{
		Singleton<StatManager>.instance.refreshAllStats();
		double num = 0.0;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count; i++)
		{
			num += Singleton<DataManager>.instance.currentGameData.treasureInventoryData[i].damagePercentValue;
		}
		treasureTotalDamageDescriptionText.text = string.Format(I18NManager.Get("TREASURE_TOTAL_BONUS_DAMAGE_DESCRIPTION"), num);
		totalTreasureCountText.text = "<color=#FAD725>" + Singleton<DataManager>.instance.currentGameData.treasureInventoryData.Count + "</color> of " + 52;
		treasureLotteryPriceText.text = Singleton<TreasureManager>.instance.currentLotteryPriceForTreasurePiece.ToString();
		if (currentTreasureSlots != null && currentTreasureSlots.Count != 0)
		{
			for (int j = 0; j < currentTreasureSlots.Count; j++)
			{
				currentTreasureSlots[j].refreshSlot();
			}
		}
	}

	private IEnumerator disableUpdate()
	{
		while (true)
		{
			for (int i = 0; i < m_allTreasureSlot.Count; i++)
			{
				RectTransform slotRectTransform = m_allTreasureSlot[i].cachedRectTransform;
				GameObject slotGameObject = m_allTreasureSlot[i].cachedGameObject;
				Vector2 slotPosition = slotRectTransform.anchoredPosition;
				float slotHeight = slotRectTransform.rect.height;
				if (Mathf.Abs(slotPosition.y) + slotHeight <= content.anchoredPosition.y || 0f - (scrollRect.rect.height + slotPosition.y) >= content.anchoredPosition.y)
				{
					if (!m_allTreasureSlot[i].isDisabled)
					{
						disableSlot(m_allTreasureSlot[i]);
					}
				}
				else if (m_allTreasureSlot[i].isDisabled)
				{
					enableSlot(m_allTreasureSlot[i]);
				}
			}
			yield return null;
		}
	}

	private void disableSlot(TreasureCollectionSlotSet targetSlot)
	{
		if (targetSlot.cachedChildrenRectTransform == null)
		{
			targetSlot.cachedChildrenRectTransform = targetSlot.cachedTransform.GetChild(0).GetComponent<RectTransform>();
		}
		targetSlot.isDisabled = true;
		targetSlot.cachedChildrenRectTransform.SetParent(Singleton<CachedManager>.instance.dummyTransform);
	}

	private void enableSlot(TreasureCollectionSlotSet targetSlot)
	{
		if (targetSlot.cachedChildrenRectTransform == null)
		{
			targetSlot.cachedChildrenRectTransform = targetSlot.cachedTransform.GetChild(0).GetComponent<RectTransform>();
		}
		targetSlot.isDisabled = false;
		targetSlot.cachedChildrenRectTransform.SetParent(targetSlot.cachedTransform);
		if (targetSlot.cachedChildrenRectTransform.localScale != Vector3.one)
		{
			targetSlot.cachedChildrenRectTransform.localScale = Vector3.one;
		}
		targetSlot.cachedChildrenRectTransform.anchoredPosition = Vector3.zero;
	}
}
