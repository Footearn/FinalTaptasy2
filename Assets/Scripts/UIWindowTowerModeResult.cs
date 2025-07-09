using UnityEngine;
using UnityEngine.UI;

public class UIWindowTowerModeResult : UIWindow
{
	public static UIWindowTowerModeResult instance;

	public GameObject clearObject;

	public Text totalFloorText;

	public Text currentTimeText;

	public Text seasonBestTimeText;

	public Text gatherHeartCoinText;

	public GameObject sunBurstEffectGameObject;

	public Transform sunBurstEffectTransform;

	public GameObject gameResultObject;

	public RectTransform gameResultRectTransform;

	public GameObject buttonSetObject;

	public RectTransform buttonSetRectTransform;

	public CanvasGroup buttonSetCanvasGroup;

	public CanvasGroup cachedCanvasGroup;

	public GameObject ticketRetryObject;

	public GameObject freeRetryObject;

	public GameObject newRecordObject;

	public GameObject noBestRecordObject;

	public Text retryPriceText;

	private TowerModeManager.TowerModeDifficultyType m_currentDifficulty;

	private bool m_isClear;

	private bool m_isCanClickButton;

	private bool m_isFreeTicketOnOpen;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openTowerModeResult(TowerModeManager.TowerModeDifficultyType modeType, bool isClear)
	{
		Singleton<TowerModeManager>.instance.checkBestRecord(modeType);
		m_currentDifficulty = modeType;
		if (m_currentDifficulty == TowerModeManager.TowerModeDifficultyType.Endless)
		{
			isClear = true;
		}
		m_isClear = isClear;
		if (m_currentDifficulty == TowerModeManager.TowerModeDifficultyType.TimeAttack)
		{
			if ((int)Singleton<TowerModeManager>.instance.currentFloor == Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor && (float)Singleton<TowerModeManager>.instance.currentTimer == Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestClearTime)
			{
				newRecordObject.SetActive(true);
				noBestRecordObject.SetActive(false);
			}
			else
			{
				newRecordObject.SetActive(false);
				noBestRecordObject.SetActive(true);
			}
			currentTimeText.text = string.Format("{0:0.##}s", Singleton<TowerModeManager>.instance.currentTimer);
			seasonBestTimeText.text = string.Format("{0}F <size=23>({1:0.##}s)</size>", Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor, Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestClearTime);
		}
		else
		{
			if ((int)Singleton<TowerModeManager>.instance.currentFloor == Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor && (float)Singleton<TowerModeManager>.instance.currentTimer == Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestClearTime)
			{
				newRecordObject.SetActive(true);
				noBestRecordObject.SetActive(false);
			}
			else
			{
				newRecordObject.SetActive(false);
				noBestRecordObject.SetActive(true);
			}
			currentTimeText.text = string.Format("{0:0.##}s", Singleton<TowerModeManager>.instance.currentTimer);
			seasonBestTimeText.text = string.Format("{0}F <size=23>({1:0.##}s)</size>", Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor, Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestClearTime);
		}
		gatherHeartCoinText.text = Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.HeartCoin].ToString("N0");
		refreshRetryButton();
		resetResultUI();
		open();
	}

	public void refreshRetryButton()
	{
		if (Singleton<TowerModeManager>.instance.isFreeTicketOn())
		{
			m_isFreeTicketOnOpen = true;
			freeRetryObject.SetActive(true);
			ticketRetryObject.SetActive(false);
		}
		else
		{
			retryPriceText.text = "1 (" + Singleton<DataManager>.instance.currentGameData.towerModeTicketCount.ToString("N0") + ")";
			m_isFreeTicketOnOpen = false;
			freeRetryObject.SetActive(false);
			ticketRetryObject.SetActive(true);
		}
	}

	private void resetResultUI()
	{
		m_isCanClickButton = false;
		Singleton<CachedManager>.instance.warriorTextureRendererCamera.initCamera(Singleton<TowerModeManager>.instance.curPlayingCharacter.cachedTransform);
		cachedCanvasGroup.alpha = 0f;
		sunBurstEffectGameObject.SetActive(true);
		buttonSetCanvasGroup.alpha = 0f;
		switch (m_currentDifficulty)
		{
		case TowerModeManager.TowerModeDifficultyType.TimeAttack:
			if ((int)Singleton<TowerModeManager>.instance.currentFloor >= Singleton<TowerModeManager>.instance.getTotalFloor(TowerModeManager.TowerModeDifficultyType.TimeAttack))
			{
				if (!clearObject.activeSelf)
				{
					clearObject.SetActive(true);
				}
				totalFloorText.text = string.Empty;
			}
			else
			{
				if (clearObject.activeSelf)
				{
					clearObject.SetActive(false);
				}
				totalFloorText.text = Singleton<TowerModeManager>.instance.currentFloor.ToString() + "F";
			}
			break;
		case TowerModeManager.TowerModeDifficultyType.Endless:
			if (clearObject.activeSelf)
			{
				clearObject.SetActive(false);
			}
			totalFloorText.text = Singleton<TowerModeManager>.instance.currentFloor.ToString() + "F";
			break;
		}
	}

	public void OnClickExit()
	{
		if (m_isCanClickButton)
		{
			m_isCanClickButton = true;
			Singleton<CachedManager>.instance.coverUI.fadeOut(0.5f, delegate
			{
				Singleton<TowerModeManager>.instance.endTowerMode();
				Singleton<CachedManager>.instance.coverUI.fadeIn();
			});
		}
	}

	public void OnClickRetry()
	{
		if (!m_isCanClickButton)
		{
			return;
		}
		m_isCanClickButton = true;
		if (Singleton<TowerModeManager>.instance.isFreeTicketOn())
		{
			Singleton<CachedManager>.instance.coverUI.fadeOut(0.5f, delegate
			{
				Singleton<TowerModeManager>.instance.endTowerMode(false, false);
				Singleton<TowerModeManager>.instance.startTowerMode(Singleton<TowerModeManager>.instance.currentDifficultyType, Singleton<TowerModeManager>.instance.isFreeTicketOn());
				Singleton<CachedManager>.instance.coverUI.fadeIn();
			});
		}
		else if (Singleton<DataManager>.instance.currentGameData.towerModeTicketCount > 0)
		{
			Singleton<CachedManager>.instance.coverUI.fadeOut(0.5f, delegate
			{
				Singleton<TowerModeManager>.instance.endTowerMode(false, false);
				Singleton<TowerModeManager>.instance.startTowerMode(Singleton<TowerModeManager>.instance.currentDifficultyType, Singleton<TowerModeManager>.instance.isFreeTicketOn());
				Singleton<TowerModeManager>.instance.decreaseTicket(1);
				Singleton<CachedManager>.instance.coverUI.fadeIn();
			});
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_TOWER_MODE_TICKET_DESCRIPTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowBuyTowerModeTicket.instance.open();
			}, string.Empty);
		}
	}

	public void playSFX(string name)
	{
		Singleton<AudioManager>.instance.playEffectSound(name);
	}

	public void endResult()
	{
		m_isCanClickButton = true;
	}

	private void Update()
	{
		if (m_isFreeTicketOnOpen && !Singleton<TowerModeManager>.instance.isFreeTicketOn())
		{
			refreshRetryButton();
		}
	}
}
