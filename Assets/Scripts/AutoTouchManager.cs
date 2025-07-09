using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTouchManager : Singleton<AutoTouchManager>
{
	public enum AutoTouchType
	{
		None = -1,
		BaseTouch,
		VeryFast,
		AdsAngelAutoTouch,
		TimerSilverFinger,
		TimerGoldFinger,
		BossRaidAutoTouch,
		CountSilverFinger,
		CountGoldFinger,
		BronzeFinger
	}

	public float touchMaxTime;

	public bool isAutoClickOn = true;

	public bool isAutoTouchOnFromConsumableItem;

	public AutoTouchType targetAutoTouch = AutoTouchType.None;

	public AutoTouchType currentAutoTouchType = AutoTouchType.None;

	public void changeAutoClickState()
	{
		isAutoClickOn = !isAutoClickOn;
		UIWindowManageShop.instance.refreshSlots();
	}

	private void closeAutoTapMiniPopup(params MiniPopupManager.MiniPopupType[] exceptTypes)
	{
		List<MiniPopupManager.MiniPopupType> list = new List<MiniPopupManager.MiniPopupType>();
		list.Add(MiniPopupManager.MiniPopupType.AdsAngelAutoTouch);
		list.Add(MiniPopupManager.MiniPopupType.CountGoldFiner);
		list.Add(MiniPopupManager.MiniPopupType.CountSilverFinger);
		list.Add(MiniPopupManager.MiniPopupType.PermanentAutoTouch);
		list.Add(MiniPopupManager.MiniPopupType.TimerGoldFinger);
		list.Add(MiniPopupManager.MiniPopupType.TimerSilverFinger);
		list.Add(MiniPopupManager.MiniPopupType.BossRaidAutoTouch);
		list.Add(MiniPopupManager.MiniPopupType.BronzeFinger);
		if (exceptTypes != null)
		{
			for (int i = 0; i < exceptTypes.Length; i++)
			{
				list.Remove(exceptTypes[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(list[j]))
			{
				Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(list[j]).closeMiniPopupObject(false);
			}
		}
	}

	public void refreshAutoTouchType()
	{
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			closeAutoTapMiniPopup(MiniPopupManager.MiniPopupType.BossRaidAutoTouch);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.BossRaidAutoTouch, true);
		}
		else if (Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime > UnbiasedTime.Instance.Now().Ticks)
		{
			closeAutoTapMiniPopup(MiniPopupManager.MiniPopupType.TimerGoldFinger);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.TimerGoldFinger, false, new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime), delegate
			{
				refreshAutoTouchType();
			});
		}
		else if (Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount >= 1)
		{
			closeAutoTapMiniPopup(MiniPopupManager.MiniPopupType.CountGoldFiner);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.CountGoldFiner, Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount, delegate
			{
				refreshAutoTouchType();
			});
		}
		else if (Singleton<AdsAngelManager>.instance.isProgressingBuff && Singleton<AdsAngelManager>.instance.currentAngelRewardType == AdsAngelManager.AngelRewardType.AutoTouch)
		{
			closeAutoTapMiniPopup(MiniPopupManager.MiniPopupType.AdsAngelAutoTouch);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.AdsAngelAutoTouch, false, new DateTime(Singleton<AdsAngelManager>.instance.adsAngelBuffTime), delegate
			{
				Singleton<AdsAngelManager>.instance.endTimeEvent();
			});
		}
		else if (Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime > UnbiasedTime.Instance.Now().Ticks)
		{
			isAutoTouchOnFromConsumableItem = true;
			closeAutoTapMiniPopup(MiniPopupManager.MiniPopupType.TimerSilverFinger);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.TimerSilverFinger, false, new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime), delegate
			{
				isAutoTouchOnFromConsumableItem = false;
				UIWindowManageShop.instance.refreshSlots();
				refreshAutoTouchType();
			});
		}
		else if (Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount >= 1)
		{
			closeAutoTapMiniPopup(MiniPopupManager.MiniPopupType.CountSilverFinger);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.CountSilverFinger, Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount, delegate
			{
				refreshAutoTouchType();
			});
		}
		else if (Singleton<DataManager>.instance.currentGameData.isBoughtBronzeFinger)
		{
			closeAutoTapMiniPopup(MiniPopupManager.MiniPopupType.BronzeFinger);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.BronzeFinger, true);
		}
		refreshAutoTouchState();
	}

	public void startGame()
	{
		refreshAutoTouchType();
		if (Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount <= 0 && Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.CountGoldFiner))
		{
			Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.CountGoldFiner).closeMiniPopupObject();
		}
		if (Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount <= 0 && Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.CountSilverFinger))
		{
			Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.CountSilverFinger).closeMiniPopupObject();
		}
		if (currentAutoTouchType == AutoTouchType.CountGoldFinger && Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount >= 1)
		{
			Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount--;
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.CountGoldFiner, Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount, delegate
			{
				refreshAutoTouchType();
			});
		}
		if (currentAutoTouchType == AutoTouchType.CountSilverFinger && Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount >= 1)
		{
			Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount--;
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.CountSilverFinger, Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount, delegate
			{
				refreshAutoTouchType();
			});
		}
		Singleton<DataManager>.instance.saveData();
	}

	public void endGame()
	{
		refreshAutoTouchType();
		if (Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount <= 0 && Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.CountGoldFiner))
		{
			Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.CountGoldFiner).closeMiniPopupObject();
		}
		if (Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount <= 0 && Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.CountSilverFinger))
		{
			Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.CountSilverFinger).closeMiniPopupObject();
		}
	}

	public void refreshAutoTouchState()
	{
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			targetAutoTouch = AutoTouchType.BossRaidAutoTouch;
		}
		else if (Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime > UnbiasedTime.Instance.Now().Ticks)
		{
			targetAutoTouch = AutoTouchType.TimerGoldFinger;
		}
		else if (Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount >= 1)
		{
			targetAutoTouch = AutoTouchType.CountGoldFinger;
		}
		else if (Singleton<AdsAngelManager>.instance.isProgressingBuff && Singleton<AdsAngelManager>.instance.currentAngelRewardType == AdsAngelManager.AngelRewardType.AutoTouch)
		{
			targetAutoTouch = AutoTouchType.AdsAngelAutoTouch;
		}
		else if (Singleton<DataManager>.instance.currentGameData.isBoughtAutoTouch || isAutoTouchOnFromConsumableItem)
		{
			targetAutoTouch = AutoTouchType.TimerSilverFinger;
		}
		else if (Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount >= 1)
		{
			targetAutoTouch = AutoTouchType.CountSilverFinger;
		}
		else if (Singleton<DataManager>.instance.currentGameData.isBoughtBronzeFinger)
		{
			targetAutoTouch = AutoTouchType.BronzeFinger;
		}
		else
		{
			targetAutoTouch = AutoTouchType.BaseTouch;
		}
		startAutoTouch(targetAutoTouch);
	}

	public void stopAutoTouch()
	{
		StopCoroutine("autoTouchUpdate");
		currentAutoTouchType = AutoTouchType.None;
		touchMaxTime = 0f;
	}

	public void startAutoTouch(AutoTouchType targetTouchType)
	{
		if (currentAutoTouchType != targetTouchType)
		{
			stopAutoTouch();
			currentAutoTouchType = targetTouchType;
			StartCoroutine("autoTouchUpdate");
		}
	}

	private IEnumerator autoTouchUpdate()
	{
		float touchTimer = 0f;
		while (true)
		{
			if (!GameManager.isPause && GameManager.currentGameState == GameManager.GameState.Playing)
			{
				touchTimer += Time.deltaTime * GameManager.timeScale;
				touchMaxTime = getAutoTouchDelay(currentAutoTouchType);
				while (touchTimer >= touchMaxTime)
				{
					Singleton<FeverManager>.instance.touchEvent();
					touchTimer -= touchMaxTime;
				}
			}
			yield return null;
		}
	}

	public float getAutoTouchDelay(AutoTouchType autoTouchType)
	{
		float result = 0f;
		switch (autoTouchType)
		{
		case AutoTouchType.BaseTouch:
			result = 0.333f;
			break;
		case AutoTouchType.VeryFast:
			result = 0.02f;
			break;
		case AutoTouchType.AdsAngelAutoTouch:
			result = 0.053f;
			break;
		case AutoTouchType.TimerSilverFinger:
		case AutoTouchType.CountSilverFinger:
			result = 0.07692308f;
			break;
		case AutoTouchType.TimerGoldFinger:
		case AutoTouchType.CountGoldFinger:
			result = 0.03846154f;
			break;
		case AutoTouchType.BossRaidAutoTouch:
			result = 0.25f;
			break;
		case AutoTouchType.BronzeFinger:
			result = 0.166f;
			break;
		}
		return result;
	}
}
