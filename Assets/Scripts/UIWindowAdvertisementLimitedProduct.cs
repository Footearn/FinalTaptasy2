using System.Collections.Generic;
using UnityEngine;

public class UIWindowAdvertisementLimitedProduct : UIWindow
{
	public static UIWindowAdvertisementLimitedProduct instance;

	public RectTransform slotParentRectTransform;

	private List<ShopSlot> m_shopSlots;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openAdverstisementLimitedProduct()
	{
		if (Singleton<DataManager>.instance.currentGameData.advertisementShopAppearEndTime > UnbiasedTime.Instance.Now().Ticks)
		{
			return;
		}
		Singleton<DataManager>.instance.currentGameData.advertisementShopAppearEndTime = UnbiasedTime.Instance.Now().AddMinutes(30.0).Ticks;
		if (m_shopSlots == null || m_shopSlots.Count < 1)
		{
			createSlot();
		}
		Singleton<ShopManager>.instance.refreshLimitedItemList();
		ShopManager.LimitedItemData limitedItemData = null;
		ShopManager.LimitedItemData limitedItemData2 = null;
		bool flag = false;
		if (ShopManager.isPaidUser)
		{
			if (Singleton<ShopManager>.instance.premiumItemList.Count > 0 && Singleton<ShopManager>.instance.skinItemList.Count > 0)
			{
				flag = true;
				limitedItemData = Singleton<ShopManager>.instance.premiumItemList[Random.Range(0, Singleton<ShopManager>.instance.premiumItemList.Count)];
				limitedItemData2 = Singleton<ShopManager>.instance.skinItemList[Random.Range(0, Singleton<ShopManager>.instance.premiumItemList.Count)];
			}
		}
		else if (Singleton<ShopManager>.instance.premiumItemList.Count > 0 && Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.Count > 0 && Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.StarterPack.ToString()) && Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.FirstPurchasePackage.ToString()) && Singleton<ShopManager>.instance.skinItemList.Count > 0)
		{
			flag = true;
			limitedItemData = ((Random.Range(0, 100) % 2 != 0) ? Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.FirstPurchasePackage] : Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.StarterPack]);
			limitedItemData2 = ((Random.Range(0, 100) % 2 != 0) ? Singleton<ShopManager>.instance.skinItemList[Random.Range(0, Singleton<ShopManager>.instance.premiumItemList.Count)] : Singleton<ShopManager>.instance.premiumItemList[Random.Range(0, Singleton<ShopManager>.instance.premiumItemList.Count)]);
		}
		if (flag)
		{
			m_shopSlots[0].currentLimitedItemType = limitedItemData.limitedType;
			m_shopSlots[0].currentLimitedItemData = limitedItemData;
			m_shopSlots[1].currentLimitedItemType = limitedItemData2.limitedType;
			m_shopSlots[1].currentLimitedItemData = limitedItemData2;
			for (int i = 0; i < m_shopSlots.Count; i++)
			{
				m_shopSlots[i].initSlot();
			}
			open();
		}
	}

	private void createSlot()
	{
		m_shopSlots = new List<ShopSlot>();
		float num = 19f;
		float num2 = 287f;
		for (int i = 0; i < 2; i++)
		{
			ShopSlot component = ObjectPool.Spawn("@ShopSlot", Vector2.zero, slotParentRectTransform).GetComponent<ShopSlot>();
			component.cachedRectTransform.anchoredPosition = new Vector2(num, 0f);
			component.cachedTransform.localScale = Vector3.one;
			component.isDefailtItemTypeRuby = false;
			component.isLimitedItem = true;
			num += num2;
			m_shopSlots.Add(component);
		}
	}

	public void OnClickClose()
	{
		Singleton<RubyManager>.instance.increaseRuby(5.0);
		Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.Ruby, 5L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		Singleton<DataManager>.instance.saveData();
		close();
	}
}
