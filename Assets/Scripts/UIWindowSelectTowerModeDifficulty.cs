using System;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowSelectTowerModeDifficulty : UIWindow
{
	public static UIWindowSelectTowerModeDifficulty instance;

	public Text timeAttackModeBestText;

	public Text endlessModeBestText;

	public Transform timeAttackDifficultyButtonTransform;

	public Transform endlessDifficultyButtonTransform;

	public GameObject[] noFreeStartButtonObjects;

	public GameObject[] freeStartButtonObjects;

	public Text ticketText;

	public GameObject ticketTimerObject;

	public Text ticketTimerText;

	public Text timeAttackModeDescriptionText;

	public GameObject freeObject;

	public Text timeAttackModeRankText;

	public Text endlessModeRankText;

	public Text currentSeasonText;

	public Text nextSeasonTimerText;

	private bool m_isFreeTicketOnOpenUI;

	private TowerModeManager.MyRankResponseData m_currentMyTimeAttackRanking;

	private TowerModeManager.MyRankResponseData m_currentMyEndlessRanking;

	private int m_successCount;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openSelectTowerModeDifficulty(bool isOpenOnEndGame)
	{
		Action action = delegate
		{
			if (TowerModeManager.isConnectedToServer())
			{
				if (!isOpenOnEndGame)
				{
					m_successCount = 0;
					Singleton<TowerModeManager>.instance.CallAPI(TowerModeManager.TowerModeAPIType.GET_MY_RANKING, true, null, delegate(bool isSuccess)
					{
						successEvent(isSuccess);
					});
					Singleton<TowerModeManager>.instance.CallAPI(TowerModeManager.TowerModeAPIType.GET_MY_RANKING, false, null, delegate(bool isSuccess)
					{
						successEvent(isSuccess);
					});
				}
				else
				{
					m_currentMyTimeAttackRanking = Singleton<TowerModeManager>.instance.myTimeAttackRankingData;
					m_currentMyEndlessRanking = Singleton<TowerModeManager>.instance.myEndlessRankingData;
					refreshUI();
					open();
				}
			}
			else
			{
				UIWindowDialog.openDescription("TOWER_MODE_DISCONNECTED_INTERNET_DESCRIPTION", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
				m_currentMyTimeAttackRanking = null;
				m_currentMyEndlessRanking = null;
				refreshUI();
				open();
			}
		};
		if (Singleton<TowerModeManager>.instance.seasonData != null)
		{
			string text = Singleton<TowerModeManager>.instance.seasonData.finish_date + "Z";
			text = text.Replace(" ", "T");
			TowerModeManager.currentSeasonEndDateTime = DateTime.Parse(text);
			TowerModeManager.currentSeasonEndDateTime = TowerModeManager.currentSeasonEndDateTime.ToLocalTime();
			TowerModeManager.currentSeason = 1;
			int.TryParse(Singleton<TowerModeManager>.instance.seasonData.id, out TowerModeManager.currentSeason);
			action();
		}
		else if (TowerModeManager.isConnectedToServer())
		{
			Singleton<TowerModeManager>.instance.CallAPI(TowerModeManager.TowerModeAPIType.GET_SEASON_INFORMATION, delegate(bool isSuccess)
			{
				if (isSuccess)
				{
					openSelectTowerModeDifficulty(false);
				}
				else
				{
					UIWindowDialog.openDescription("DEMON_TOWER_TRY_AGAIN", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
				}
			});
		}
		else
		{
			UIWindowDialog.openDescription("TOWER_MODE_DISCONNECTED_INTERNET_DESCRIPTION", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			m_currentMyTimeAttackRanking = null;
			m_currentMyEndlessRanking = null;
			refreshUI();
			open();
		}
	}

	private void successEvent(bool isSuccess)
	{
		if (isSuccess)
		{
			m_successCount++;
		}
		if (m_successCount >= 2)
		{
			m_currentMyTimeAttackRanking = Singleton<TowerModeManager>.instance.myTimeAttackRankingData;
			m_currentMyEndlessRanking = Singleton<TowerModeManager>.instance.myEndlessRankingData;
			refreshUI();
			open();
		}
	}

	public void refreshUI()
	{
		if (m_currentMyTimeAttackRanking == null || m_currentMyEndlessRanking == null)
		{
			m_currentMyTimeAttackRanking = new TowerModeManager.MyRankResponseData();
			m_currentMyTimeAttackRanking.player = new TowerModeManager.UserInformationData();
			m_currentMyTimeAttackRanking.player.rank = "--";
			m_currentMyTimeAttackRanking.player.clear_floor = "--";
			m_currentMyTimeAttackRanking.player.clear_time = "--";
			m_currentMyEndlessRanking = new TowerModeManager.MyRankResponseData();
			m_currentMyEndlessRanking.player = new TowerModeManager.UserInformationData();
			m_currentMyEndlessRanking.player.rank = "--";
			m_currentMyEndlessRanking.player.clear_floor = "--";
			m_currentMyEndlessRanking.player.clear_time = "--";
		}
		currentSeasonText.text = "Season " + TowerModeManager.currentSeason;
		TimeSpan timeSpan = new TimeSpan(new DateTime(TowerModeManager.currentSeasonEndDateTime.Ticks).Ticks - UnbiasedTime.Instance.Now().Ticks);
		string str = string.Format(((!(timeSpan.TotalDays > 0.0)) ? string.Empty : ("{0:00}" + I18NManager.Get("SHOP_LIMITED_DAY_SIMPLE") + " ")) + ((timeSpan.Hours <= 0) ? string.Empty : "{1:00}:") + "{2:00}:{3:00}", (int)timeSpan.TotalDays, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		nextSeasonTimerText.text = I18NManager.Get("TOWER_MODE_NEXT_SEASON_IN") + " <color=white>" + str + "</color>";
		string empty = string.Empty;
		int result = 0;
		float result2 = 0f;
		int result3 = 0;
		float result4 = 0f;
		int.TryParse(m_currentMyTimeAttackRanking.player.clear_floor, out result);
		if (float.TryParse(m_currentMyTimeAttackRanking.player.clear_time, out result2))
		{
			result2 /= 1000f;
		}
		int.TryParse(m_currentMyEndlessRanking.player.clear_floor, out result3);
		if (float.TryParse(m_currentMyEndlessRanking.player.clear_time, out result4))
		{
			result4 /= 1000f;
		}
		result = Math.Max(result, 0);
		result2 = Math.Max(result2, 0f);
		result3 = Math.Max(result3, 0);
		result4 = Math.Max(result4, 0f);
		empty = ((result < Singleton<TowerModeManager>.instance.getTotalFloor(TowerModeManager.TowerModeDifficultyType.TimeAttack)) ? string.Format(I18NManager.Get("FLOOR_TEXT"), result) : I18NManager.Get("CLEAR"));
		TimeSpan timeSpan2 = TimeSpan.FromSeconds(result2);
		TimeSpan timeSpan3 = TimeSpan.FromSeconds(result4);
		timeAttackModeBestText.text = I18NManager.Get("TOWER_MODE_SEASON_BEST_RECORD") + " : <color=#FAD725FF>" + empty + "</color><color=#FDFCB7> / " + string.Format("{0:00}:{1:00}.{2:000}", (int)timeSpan2.TotalMinutes, timeSpan2.Seconds, timeSpan2.Milliseconds) + "</color>";
		endlessModeBestText.text = I18NManager.Get("TOWER_MODE_SEASON_BEST_RECORD") + " : <color=#FAD725FF>" + string.Format(I18NManager.Get("FLOOR_TEXT"), result3) + "</color><color=#FDFCB7> / " + string.Format("{0:00}:{1:00}.{2:000}", (int)timeSpan3.TotalMinutes, timeSpan3.Seconds, timeSpan3.Milliseconds) + "</color>";
		timeAttackModeDescriptionText.text = string.Format(I18NManager.Get("TOWER_MODE_DIFFICULTY_DESCRIPTION_1"), Singleton<TowerModeManager>.instance.getTotalFloor(TowerModeManager.TowerModeDifficultyType.TimeAttack) - 1);
		int result5 = 0;
		if (!int.TryParse(m_currentMyTimeAttackRanking.player.rank, out result5))
		{
			result5 = -1;
		}
		int result6 = 0;
		if (!int.TryParse(m_currentMyEndlessRanking.player.rank, out result6))
		{
			result6 = -1;
		}
		timeAttackModeRankText.text = string.Format(I18NManager.Get("RANK"), (result5 == -1) ? "--" : result5.ToString("N0"));
		endlessModeRankText.text = string.Format(I18NManager.Get("RANK"), (result6 == -1) ? "--" : result6.ToString("N0"));
		refreshTicketText();
		refreshFreeTicketState();
	}

	public void OnClickStartTowerMode(bool isTimeAttackMode)
	{
		if (Singleton<DataManager>.instance.currentGameData.towerModeTicketCount > 0)
		{
			TowerModeManager.isBossCheatOn = false;
			UIWindowOutgame.instance.onClickEnterTowerMode((!isTimeAttackMode) ? TowerModeManager.TowerModeDifficultyType.Endless : TowerModeManager.TowerModeDifficultyType.TimeAttack, false);
			Singleton<TowerModeManager>.instance.decreaseTicket(1);
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_TOWER_MODE_TICKET_DESCRIPTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowBuyTowerModeTicket.instance.open();
			}, string.Empty);
		}
	}

	public void OnClickFreeStartTowerMode(bool isTimeAttackMode)
	{
		TowerModeManager.isBossCheatOn = false;
		UIWindowOutgame.instance.onClickEnterTowerMode((!isTimeAttackMode) ? TowerModeManager.TowerModeDifficultyType.Endless : TowerModeManager.TowerModeDifficultyType.TimeAttack, true);
	}

	public void OnClickOpenRanking(bool isTimeAttackMode)
	{
		if (TowerModeManager.isConnectedToServer())
		{
			UIWindowTowerModeRanking.instance.openTowerModeRanking(isTimeAttackMode);
		}
		else
		{
			UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void OnClickRakingReward()
	{
		UIWindow.Get("UI TowerModeRewardInformation").open();
	}

	public void OnClickChageTicket()
	{
		UIWindowBuyTowerModeTicket.instance.open();
	}

	public void refreshTicketText()
	{
		ticketText.text = Singleton<DataManager>.instance.currentGameData.towerModeTicketCount + "/" + TowerModeManager.maxTicketCount;
	}

	private void refreshFreeTicketState()
	{
		if (Singleton<TowerModeManager>.instance.isFreeTicketOn())
		{
			ticketTimerText.text = I18NManager.Get("TOWER_MODE_USING_FREE_TICKET");
			m_isFreeTicketOnOpenUI = true;
			freeObject.SetActive(true);
			for (int i = 0; i < noFreeStartButtonObjects.Length; i++)
			{
				noFreeStartButtonObjects[i].SetActive(false);
			}
			for (int j = 0; j < freeStartButtonObjects.Length; j++)
			{
				freeStartButtonObjects[j].SetActive(true);
			}
		}
		else
		{
			m_isFreeTicketOnOpenUI = false;
			freeObject.SetActive(false);
			for (int k = 0; k < noFreeStartButtonObjects.Length; k++)
			{
				noFreeStartButtonObjects[k].SetActive(true);
			}
			for (int l = 0; l < freeStartButtonObjects.Length; l++)
			{
				freeStartButtonObjects[l].SetActive(false);
			}
		}
	}

	private void Update()
	{
		if (!UIWindowIntro.isLoaded)
		{
			return;
		}
		if (TowerModeManager.currentSeasonEndDateTime.Ticks > UnbiasedTime.Instance.Now().Ticks)
		{
			TimeSpan timeSpan = new TimeSpan(new DateTime(TowerModeManager.currentSeasonEndDateTime.Ticks).Ticks - UnbiasedTime.Instance.Now().Ticks);
			string str = string.Format((((int)timeSpan.TotalDays <= 0) ? string.Empty : ("{0}" + I18NManager.Get("SHOP_LIMITED_DAY_SIMPLE") + " ")) + ((timeSpan.Hours <= 0) ? string.Empty : "{1:00}:") + "{2:00}:{3:00}", (int)timeSpan.TotalDays, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			nextSeasonTimerText.text = I18NManager.Get("TOWER_MODE_NEXT_SEASON_IN") + " <color=white>" + str + "</color>";
		}
		else
		{
			nextSeasonTimerText.text = string.Empty;
			if (Singleton<TowerModeManager>.instance.seasonData != null)
			{
				Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor = 0;
				Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestClearTime = 0f;
				Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor = 0;
				Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestClearTime = 0f;
				Singleton<DataManager>.instance.currentGameData.towerModeLastSeason++;
				Singleton<DataManager>.instance.saveData();
				DateTime dateTime = UnbiasedTime.Instance.Now().ToUniversalTime().AddDays(4.0);
				Singleton<TowerModeManager>.instance.seasonData.id = Singleton<DataManager>.instance.currentGameData.towerModeLastSeason.ToString();
				Singleton<TowerModeManager>.instance.seasonData.finish_date = dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day + " " + dateTime.Hour + ":" + dateTime.Minute + ":" + dateTime.Second;
				m_currentMyTimeAttackRanking = null;
				m_currentMyEndlessRanking = null;
				openSelectTowerModeDifficulty(false);
			}
		}
		if (Singleton<TowerModeManager>.instance.isFreeTicketOn())
		{
			TimeSpan timeSpan2 = new TimeSpan(new DateTime(Singleton<DataManager>.instance.currentGameData.towerModeFreeTicketEndTime).Ticks - UnbiasedTime.Instance.Now().Ticks);
			string text = string.Format(((!(timeSpan2.TotalHours > 0.0)) ? string.Empty : "{0:00}:") + "{1:00}:{2:00}", (int)timeSpan2.TotalHours, timeSpan2.Minutes, timeSpan2.Seconds);
			ticketText.text = text;
		}
		else if (m_isFreeTicketOnOpenUI)
		{
			refreshUI();
		}
		if (!m_isFreeTicketOnOpenUI)
		{
			if (Singleton<DataManager>.instance.currentGameData.towerModeTicketCount < TowerModeManager.maxTicketCount)
			{
				if (!ticketTimerObject.activeSelf)
				{
					ticketTimerObject.SetActive(true);
				}
				TimeSpan timeSpan3 = new TimeSpan(new DateTime(Singleton<DataManager>.instance.currentGameData.lastTowerModeStartTime).AddMinutes(60.0).Ticks - UnbiasedTime.Instance.Now().Ticks);
				string text2 = string.Format(I18NManager.Get("TOWER_MODE_TICKET_REMAIN_TIME_DESCRIPTION"), "<color=white>" + string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan3.TotalHours, timeSpan3.Minutes, timeSpan3.Seconds) + "</color>");
				ticketTimerText.text = text2;
			}
			else if (ticketTimerObject.activeSelf)
			{
				ticketTimerObject.SetActive(false);
			}
		}
		else if (!ticketTimerObject.activeSelf)
		{
			ticketTimerObject.SetActive(true);
		}
	}
}
