using UnityEngine;
using UnityEngine.UI;

public class UIWindowManageShop : UIWindow
{
	public static UIWindowManageShop instance;

	public OptimizedScrollRect[] itemScrollRects;

	public GameObject[] itemScrollRectObjects;

	public Text[] itemTitleTexts;

	public Image[] itemTabButtonBackgroundImages;

	public RectTransform[] itemTabButtonButtonTransforms;

	public GameObject[] itemTabButtonButtonGameObjects;

	public ShopManager.ShopSelectedType currentSelectedType;

	public GameObject emptyObject;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		UIWindowOutgame.instance.refreshShopIndicator(0);
		ShopManager.ShopSelectedType targetSelectType = ShopManager.ShopSelectedType.PremiumItem;
		if (Singleton<ShopManager>.instance.premiumItemList.Count <= 0)
		{
			targetSelectType = ShopManager.ShopSelectedType.SkinItem;
			if (Singleton<ShopManager>.instance.skinItemList.Count <= 0)
			{
				targetSelectType = ShopManager.ShopSelectedType.Ruby;
			}
		}
		openShopUI(targetSelectType);
		return base.OnBeforeOpen();
	}

	public void refreshSlots()
	{
		for (int i = 0; i < itemScrollRects.Length; i++)
		{
			for (int j = 0; j < itemScrollRects[i].slotObjects.Count; j++)
			{
				itemScrollRects[i].slotObjects[j].refreshSlot();
			}
		}
	}

	public void focus(ShopManager.LimitedShopItemType itemType)
	{
		OptimizedScrollRect optimizedScrollRect = itemScrollRects[(int)currentSelectedType];
		float num = optimizedScrollRect.GetComponent<RectTransform>().rect.width % optimizedScrollRect.intervalBetweenCell;
		for (int i = 0; i < optimizedScrollRect.slotObjects.Count; i++)
		{
			if ((optimizedScrollRect.slotObjects[i] as ShopSlot).currentLimitedItemType == itemType)
			{
				optimizedScrollRect.content.anchoredPosition = new Vector2((float)(-i + 1) * optimizedScrollRect.intervalBetweenCell - num + 174.79f, 0f);
				break;
			}
		}
	}

	public void focus(ShopManager.DefaultGoldShopItemType itemType)
	{
		OptimizedScrollRect optimizedScrollRect = itemScrollRects[(int)currentSelectedType];
		for (int i = 0; i < optimizedScrollRect.slotObjects.Count; i++)
		{
			if ((optimizedScrollRect.slotObjects[i] as ShopSlot).currentDefaultGoldItemType == itemType)
			{
				optimizedScrollRect.content.anchoredPosition = new Vector2(0f - optimizedScrollRect.slotObjects[i].cachedRectTransform.anchoredPosition.x, 0f);
				break;
			}
		}
	}

	public void focus(ShopManager.DefaultRubyShopItemType itemType)
	{
		OptimizedScrollRect optimizedScrollRect = itemScrollRects[(int)currentSelectedType];
		float num = optimizedScrollRect.GetComponent<RectTransform>().rect.width % optimizedScrollRect.intervalBetweenCell;
		for (int i = 0; i < optimizedScrollRect.slotObjects.Count; i++)
		{
			if ((optimizedScrollRect.slotObjects[i] as ShopSlot).currentDefaultRubyItemType == itemType)
			{
				optimizedScrollRect.content.anchoredPosition = new Vector2(0f - optimizedScrollRect.slotObjects[i].cachedRectTransform.anchoredPosition.x, 0f);
				break;
			}
		}
	}

	public void focusToGold()
	{
		focus(ShopManager.DefaultGoldShopItemType.GoldTier3);
	}

	public void focusToRuby()
	{
		focus(ShopManager.DefaultRubyShopItemType.Tapjoy);
	}

	public void focusingToVIPItem()
	{
		focus(ShopManager.LimitedShopItemType.MiniVIPPackage);
	}

	private void resetAllState()
	{
		for (int i = 0; i < itemTitleTexts.Length; i++)
		{
			itemTitleTexts[i].color = Util.getCalculatedColor(65f, 133f, 178f);
		}
		for (int j = 0; j < itemTabButtonBackgroundImages.Length; j++)
		{
			itemTabButtonBackgroundImages[j].sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
		}
		for (int k = 0; k < itemScrollRectObjects.Length; k++)
		{
			itemScrollRectObjects[k].SetActive(false);
		}
		for (int l = 0; l < itemTabButtonButtonTransforms.Length; l++)
		{
			itemTabButtonButtonTransforms[l].SetAsFirstSibling();
		}
		for (int m = 0; m < itemScrollRects.Length; m++)
		{
			for (int n = 0; n < itemScrollRects[m].slotObjects.Count; n++)
			{
				ObjectPool.Recycle(itemScrollRects[m].slotObjects[n].name, itemScrollRects[m].slotObjects[n].cachedGameObject);
			}
			itemScrollRects[m].slotObjects.Clear();
		}
	}

	public void openShopUI(ShopManager.ShopSelectedType targetSelectType)
	{
		Singleton<ShopManager>.instance.refreshAllLimitedItemAvailable();
		currentSelectedType = targetSelectType;
		resetAllState();
		createSlot(targetSelectType);
		emptyObject.SetActive(false);
		switch (currentSelectedType)
		{
		case ShopManager.ShopSelectedType.PremiumItem:
			if (Singleton<ShopManager>.instance.premiumItemList.Count <= 0)
			{
				emptyObject.SetActive(true);
			}
			itemTitleTexts[0].color = Color.white;
			itemTabButtonBackgroundImages[0].sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
			itemScrollRectObjects[0].SetActive(true);
			itemTabButtonButtonTransforms[0].SetAsLastSibling();
			break;
		case ShopManager.ShopSelectedType.SkinItem:
			if (Singleton<ShopManager>.instance.skinItemList.Count <= 0)
			{
				emptyObject.SetActive(true);
			}
			itemTitleTexts[1].color = Color.white;
			itemTabButtonBackgroundImages[1].sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
			itemScrollRectObjects[1].SetActive(true);
			itemTabButtonButtonTransforms[1].SetAsLastSibling();
			break;
		case ShopManager.ShopSelectedType.NormalItem:
			itemTitleTexts[2].color = Color.white;
			itemTabButtonBackgroundImages[2].sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
			itemScrollRectObjects[2].SetActive(true);
			itemTabButtonButtonTransforms[2].SetAsLastSibling();
			break;
		case ShopManager.ShopSelectedType.Ruby:
			itemTitleTexts[3].color = Color.white;
			itemTabButtonBackgroundImages[3].sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
			itemScrollRectObjects[3].SetActive(true);
			itemTabButtonButtonTransforms[3].SetAsLastSibling();
			break;
		}
		for (int i = 0; i < itemTabButtonButtonGameObjects.Length; i++)
		{
			itemTabButtonButtonGameObjects[i].SetActive(true);
		}
	}

	private void createSlot(ShopManager.ShopSelectedType targetType)
	{
		OptimizedScrollRect optimizedScrollRect = itemScrollRects[(int)targetType];
		for (int i = 0; i < optimizedScrollRect.slotObjects.Count; i++)
		{
			ObjectPool.Recycle(optimizedScrollRect.slotObjects[i].name, optimizedScrollRect.slotObjects[i].cachedGameObject);
		}
		optimizedScrollRect.slotObjects.Clear();
		switch (targetType)
		{
		case ShopManager.ShopSelectedType.PremiumItem:
		{
			float num3 = 19.98999f;
			if (Singleton<ShopManager>.instance.premiumItemList.Count <= 0)
			{
				break;
			}
			for (int k = 0; k < Singleton<ShopManager>.instance.premiumItemList.Count; k++)
			{
				ShopManager.LimitedItemData limitedItemData2 = Singleton<ShopManager>.instance.premiumItemList[k];
				if (!limitedItemData2.isDisabledItem)
				{
					ShopSlot component3 = ObjectPool.Spawn("@ShopSlot", new Vector2(num3, 0f), optimizedScrollRect.content).GetComponent<ShopSlot>();
					optimizedScrollRect.slotObjects.Add(component3);
					component3.cachedTransform.localScale = Vector3.one;
					component3.currentLimitedItemType = limitedItemData2.limitedType;
					component3.isDefailtItemTypeRuby = false;
					component3.isLimitedItem = true;
					component3.currentLimitedItemData = limitedItemData2;
					component3.parentScrollRect = optimizedScrollRect;
					component3.initSlot();
					num3 += optimizedScrollRect.intervalBetweenCell;
				}
			}
			optimizedScrollRect.content.sizeDelta = new Vector2(Mathf.Abs(num3), 493f);
			break;
		}
		case ShopManager.ShopSelectedType.SkinItem:
		{
			float num6 = 19.98999f;
			if (Singleton<ShopManager>.instance.skinItemList.Count <= 0)
			{
				break;
			}
			for (int m = 0; m < Singleton<ShopManager>.instance.skinItemList.Count; m++)
			{
				ShopManager.LimitedItemData limitedItemData4 = Singleton<ShopManager>.instance.skinItemList[m];
				if (!limitedItemData4.isDisabledItem)
				{
					ShopSlot component6 = ObjectPool.Spawn("@ShopSlot", new Vector2(num6, 0f), optimizedScrollRect.content).GetComponent<ShopSlot>();
					optimizedScrollRect.slotObjects.Add(component6);
					component6.cachedTransform.localScale = Vector3.one;
					component6.currentLimitedItemType = limitedItemData4.limitedType;
					component6.isDefailtItemTypeRuby = false;
					component6.isLimitedItem = true;
					component6.currentLimitedItemData = limitedItemData4;
					component6.parentScrollRect = optimizedScrollRect;
					component6.initSlot();
					num6 += optimizedScrollRect.intervalBetweenCell;
				}
			}
			optimizedScrollRect.content.sizeDelta = new Vector2(Mathf.Abs(num6), 493f);
			break;
		}
		case ShopManager.ShopSelectedType.NormalItem:
		{
			float num4 = 19.98999f;
			if (Singleton<ShopManager>.instance.normalItemList.Count > 0)
			{
				for (int l = 0; l < Singleton<ShopManager>.instance.normalItemList.Count; l++)
				{
					ShopManager.LimitedItemData limitedItemData3 = Singleton<ShopManager>.instance.normalItemList[l];
					if (!limitedItemData3.isDisabledItem)
					{
						ShopSlot component4 = ObjectPool.Spawn("@ShopSlot", new Vector2(num4, 0f), optimizedScrollRect.content).GetComponent<ShopSlot>();
						optimizedScrollRect.slotObjects.Add(component4);
						component4.cachedTransform.localScale = Vector3.one;
						component4.currentLimitedItemType = limitedItemData3.limitedType;
						component4.isDefailtItemTypeRuby = false;
						component4.isLimitedItem = true;
						component4.currentLimitedItemData = limitedItemData3;
						component4.parentScrollRect = optimizedScrollRect;
						component4.initSlot();
						num4 += optimizedScrollRect.intervalBetweenCell;
					}
				}
			}
			for (int num5 = 3; num5 >= 1; num5--)
			{
				ShopSlot component5 = ObjectPool.Spawn("@ShopSlot", new Vector2(num4, 0f), optimizedScrollRect.content).GetComponent<ShopSlot>();
				optimizedScrollRect.slotObjects.Add(component5);
				component5.cachedTransform.localScale = Vector3.one;
				component5.currentDefaultGoldItemType = (ShopManager.DefaultGoldShopItemType)num5;
				component5.isDefailtItemTypeRuby = false;
				component5.isLimitedItem = false;
				component5.parentScrollRect = optimizedScrollRect;
				component5.initSlot();
				num4 += optimizedScrollRect.intervalBetweenCell;
			}
			optimizedScrollRect.content.sizeDelta = new Vector2(Mathf.Abs(num4), 493f);
			break;
		}
		case ShopManager.ShopSelectedType.Ruby:
		{
			float num = 19.98999f;
			if (Singleton<ShopManager>.instance.rubyItemList.Count > 0)
			{
				for (int j = 0; j < Singleton<ShopManager>.instance.rubyItemList.Count; j++)
				{
					ShopManager.LimitedItemData limitedItemData = Singleton<ShopManager>.instance.rubyItemList[j];
					if (!limitedItemData.isDisabledItem)
					{
						ShopSlot component = ObjectPool.Spawn("@ShopSlot", new Vector2(num, 0f), optimizedScrollRect.content).GetComponent<ShopSlot>();
						optimizedScrollRect.slotObjects.Add(component);
						component.cachedTransform.localScale = Vector3.one;
						component.currentLimitedItemType = limitedItemData.limitedType;
						component.isDefailtItemTypeRuby = false;
						component.isLimitedItem = true;
						component.currentLimitedItemData = limitedItemData;
						component.parentScrollRect = optimizedScrollRect;
						component.initSlot();
						num += optimizedScrollRect.intervalBetweenCell;
					}
				}
			}
			for (int num2 = 7; num2 >= 1; num2--)
			{
				if (num2 == (int)ShopManager.DefaultRubyShopItemType.Tapjoy)
					continue;
					
				ShopSlot component2 = ObjectPool.Spawn("@ShopSlot", new Vector2(num, 0f), optimizedScrollRect.content).GetComponent<ShopSlot>();
				optimizedScrollRect.slotObjects.Add(component2);
				component2.cachedTransform.localScale = Vector3.one;
				component2.currentDefaultRubyItemType = (ShopManager.DefaultRubyShopItemType)num2;
				component2.isDefailtItemTypeRuby = true;
				component2.isLimitedItem = false;
				component2.parentScrollRect = optimizedScrollRect;
				component2.initSlot();
				num += optimizedScrollRect.intervalBetweenCell;
			}
			optimizedScrollRect.content.sizeDelta = new Vector2(Mathf.Abs(num), 493f);
			break;
		}
		}
	}

	public void OnClickOpenShopWithType(int type)
	{
		openShopUI((ShopManager.ShopSelectedType)type);
	}
}
