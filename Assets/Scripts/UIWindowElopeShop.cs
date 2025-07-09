using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowElopeShop : UIWindow
{
	public static UIWindowElopeShop instance;

	public List<ElopeShopSlot> totalItemSlotList;

	public CanvasGroup cachedCanvasGroup;

	public Text refreshRemainText;

	private bool m_isFinishRefreshTime;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		if (UIWindowManageTreasure.instance.elopeShopIndicator.activeSelf)
		{
			UIWindowManageTreasure.instance.elopeShopIndicator.SetActive(false);
		}
		if (!Singleton<ElopeModeManager>.instance.isCanStartElopeMode())
		{
			UIWindowDialog.openMiniDialog("CAN_NOT_ENTER_ELOPE_SHOP_DESCRIPTION");
			return false;
		}
		Singleton<ElopeModeManager>.instance.displayHeartCoin();
		Singleton<ElopeModeManager>.instance.checkElopeShopData();
		if (UIWindowOutgame.instance.isOpen)
		{
			UIWindowOutgame.instance.refreshTreasureIndicator();
		}
		m_isFinishRefreshTime = false;
		refreshSlots();
		return base.OnBeforeOpen();
	}

	public void refreshSlots()
	{
		for (int i = 0; i < totalItemSlotList.Count; i++)
		{
			totalItemSlotList[i].initSlot(Singleton<DataManager>.instance.currentGameData.currentElopeShopItemList[i]);
		}
	}

	public void OnClickRefresh()
	{
		UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("ELOPE_REFRESH_DESCRIPTION"), ElopeModeManager.refreshPrice), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			if (Singleton<DataManager>.instance.currentGameData._ruby >= ElopeModeManager.refreshPrice)
			{
				Singleton<RubyManager>.instance.decreaseRuby(ElopeModeManager.refreshPrice);
				Singleton<ElopeModeManager>.instance.refreshElopeShopItems();
				refreshSlots();
				UIWindowDialog.openMiniDialog("ELOPE_REFRESH_SUCCESS");
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, I18NManager.Get("COUPON_REWARD_RUBY"));
			}
		}, I18NManager.Get("REFRESH_ELOPE_ITEM"));
	}

	private void Update()
	{
		TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.Now().Ticks - Singleton<DataManager>.instance.currentGameData.elopeShopTargetRefreshTime);
		long num = (long)(timeSpan.TotalHours / (double)ElopeModeManager.intervalRefreshItemHour) + 1;
		if (Singleton<DataManager>.instance.currentGameData.lastRefreshElopeShopHour == num)
		{
			TimeSpan timeSpan2 = new TimeSpan(new DateTime(Singleton<DataManager>.instance.currentGameData.elopeShopTargetRefreshTime).AddHours(ElopeModeManager.intervalRefreshItemHour * num).Ticks - UnbiasedTime.Instance.Now().Ticks);
			string text = I18NManager.Get("REFRESH_TARGET_TIME") + " : <color=white>" + string.Format((((long)timeSpan2.TotalHours <= 0) ? string.Empty : "{0:00}:") + "{1:00}:{2:00}", (int)timeSpan2.TotalHours, timeSpan2.Minutes, timeSpan2.Seconds) + "</color>";
			refreshRemainText.text = text;
			m_isFinishRefreshTime = false;
			return;
		}
		if (refreshRemainText.text != string.Empty)
		{
			refreshRemainText.text = string.Empty;
		}
		if (!m_isFinishRefreshTime)
		{
			m_isFinishRefreshTime = true;
			Singleton<ElopeModeManager>.instance.checkElopeShopData();
			refreshSlots();
			UIWindowDialog.openMiniDialog("ELOPE_REFRESH_SUCCESS");
		}
	}
}
