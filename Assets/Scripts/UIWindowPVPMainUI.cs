using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowPVPMainUI : UIWindow
{
	public enum PVPUITabType
	{
		None = -1,
		SeasonShopTab,
		RecordHistoryTab,
		BattleTab,
		SeasonRankingTab,
		HonorRankingTab,
		Length
	}

	public static UIWindowPVPMainUI instance;

	public static DateTime currentSeasonEndDateTime;

	public static int intervalRankingLoadCount = 10;

	public Text currentSeasonText;

	public Text nextSeasonTimerText;

	public CanvasGroup cachedCanvasGroup;

	public List<GameObject> tabObjectList;

	public List<RectTransform> tabButtonTransformList;

	public List<Image> tabImageList;

	public List<Text> tabTextList;

	public Text ticketText;

	public TextMeshProUGUI totalDamageText;

	public RectTransform startMatchingButtonRectTransform;

	public GameObject addTicketButtonObject;

	public Text myRecordText;

	public Text myTierText;

	public Text myRankingText;

	public Image myTierIconImage;

	public Image dailyHonorRewardTierIconImage;

	public Text dailyHonorRewardTierText;

	public Text dailyHonorRewardText;

	public GameObject dailyHonorRewardButtonObject;

	public GameObject dailyHonorRewardDisabledObject;

	public GameObject dailyHonorRewardCompleteObject;

	public Text seasonRankingText;

	public Text seasonRankingRewardText;

	public GameObject seasonRankingRewardButtonObject;

	public GameObject seasonRankingRewardDisabledObject;

	public GameObject seasonRankingRewardCompleteObject;

	public Image seasonPlayCountImage;

	public Text seasonTotalPlayCountText;

	public GameObject seasonPlayCountRewardRecieveButtonObject1;

	public GameObject seasonPlayCountRewardRecieveDisabledButtonObject1;

	public GameObject seasonPlayCountCompleteObject1;

	public Image seasonPlayCountRewardOnOffIconImage1;

	public Text seasonPlayCountRewardText1;

	public GameObject seasonPlayCountRewardRecieveButtonObject2;

	public GameObject seasonPlayCountRewardRecieveDisabledButtonObject2;

	public GameObject seasonPlayCountCompleteObject2;

	public Image seasonPlayCountRewardOnOffIconImage2;

	public Text seasonPlayCountRewardText2;

	public GameObject seasonPlayCountRewardRecieveButtonObject3;

	public GameObject seasonPlayCountRewardRecieveDisabledButtonObject3;

	public GameObject seasonPlayCountCompleteObject3;

	public Image seasonPlayCountRewardOnOffIconImage3;

	public Text seasonPlayCountRewardText3;

	public GameObject seasonPlayCountRewardRecieveButtonObject4;

	public GameObject seasonPlayCountRewardRecieveDisabledButtonObject4;

	public GameObject seasonPlayCountCompleteObject4;

	public Image seasonPlayCountRewardOnOffIconImage4;

	public Text seasonPlayCountRewardText4;

	public TextMeshProUGUI tankTotalCountText;

	public InfiniteScroll histroyScroll;

	public SpriteMask historyMask;

	public PVPManager.PVPUserData mySeasonRankingData;

	public Text seasonRankingMyRankText;

	public Text seasonRankingMyNameText;

	public Text seasonRankingMyRecordText;

	public CharacterUIObject seasonRankingMyRankingWarriorUIObject;

	public CharacterUIObject seasonRankingMyRankingPriestUIObject;

	public CharacterUIObject seasonRankingMyRankingArcherUIObject;

	public InfiniteScroll seasonRankingScroll;

	public SpriteMask seasonRankingMask;

	public GameObject seasonRankingEmptyObject;

	public RectTransform seasonRankingScrollTransform;

	public RectTransform seasonRankingScrollContent;

	private bool m_isLoadingExtraUser;

	public InfiniteScroll honorRankingScroll;

	public SpriteMask honorRankingMask;

	public PVPShopSlot[] pvpShopslots;

	public Text pvpShopRefreshRemainText;

	private bool m_isFinishRefreshTime;

	public PVPUITabType currentTabType = PVPUITabType.None;

	private int m_currentSeasonRankingStartValue;

	private int m_currentSeasonRankingEndValue;

	private bool m_isChangingNextSeason;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openPVPMainUI(bool withRefresh = true)
	{
		Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.GET_MY_DATA, delegate(bool isSuccess)
		{
			if (isSuccess)
			{
				m_isChangingNextSeason = false;
				refreshAddTicketButtonObject();
				if (refreshUI())
				{
					if (withRefresh)
					{
						openTab(PVPUITabType.BattleTab);
					}
					open();
				}
			}
			else if (isOpen)
			{
				close();
			}
		}, withRefresh);
	}

	private void refreshSeasonTimer()
	{
		TimeSpan timeSpan = new TimeSpan(currentSeasonEndDateTime.Ticks - UnbiasedTime.Instance.UtcNow().Ticks);
		string str = string.Format(((!(timeSpan.TotalDays > 0.0)) ? string.Empty : ("{0}" + I18NManager.Get("SHOP_LIMITED_DAY_SIMPLE") + " ")) + ((timeSpan.Hours <= 0) ? string.Empty : "{1:00}:") + "{2:00}:{3:00}", (int)timeSpan.TotalDays, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		nextSeasonTimerText.text = I18NManager.Get("TOWER_MODE_NEXT_SEASON_IN") + " <color=#FAD725>" + str + "</color>";
	}

	private bool refreshUI()
	{
		PVPManager.PvPData myPVPData = Singleton<PVPManager>.instance.myPVPData;
		PVPManager.PVPGameData pVPGameData = Singleton<PVPManager>.instance.convertStringToPVPGameData(myPVPData.player.game_data);
		int num = 0;
		foreach (KeyValuePair<ObscuredInt, PVPTankData> tankDatum in pVPGameData.tankData)
		{
			if ((bool)tankDatum.Value.isUnlocked)
			{
				num++;
			}
		}
		tankTotalCountText.text = "x" + num.ToString("N0");
		totalDamageText.text = GameManager.changeUnit(Singleton<PVPManager>.instance.getTotalDamage(pVPGameData));
		currentSeasonText.text = "Season " + myPVPData.season.id;
		currentSeasonEndDateTime = DateTime.Parse(myPVPData.season.finish_date);
		refreshSeasonTimer();
		myTierText.text = Singleton<PVPManager>.instance.getTierName(myPVPData.player.grade);
		myRankingText.text = string.Format(I18NManager.Get("PVP_MMR"), myPVPData.player.point) + " / " + string.Format(I18NManager.Get("RANK"), (myPVPData.player.rank >= 0) ? myPVPData.player.rank.ToString("N0") : "--");
		myTierIconImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(myPVPData.player.grade);
		myTierIconImage.SetNativeSize();
		dailyHonorRewardTierText.text = Singleton<PVPManager>.instance.getTierName(myPVPData.reward_honor.grade);
		dailyHonorRewardTierIconImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(myPVPData.reward_honor.grade);
		dailyHonorRewardTierIconImage.SetNativeSize();
		if (myPVPData.reward_honor.entry_yn.Contains("Y"))
		{
			dailyHonorRewardButtonObject.SetActive(false);
			dailyHonorRewardCompleteObject.SetActive(true);
			dailyHonorRewardDisabledObject.SetActive(false);
		}
		else
		{
			dailyHonorRewardCompleteObject.SetActive(false);
			if (myPVPData.reward_honor.active_yn.Contains("Y"))
			{
				dailyHonorRewardButtonObject.SetActive(true);
				dailyHonorRewardDisabledObject.SetActive(false);
			}
			else
			{
				dailyHonorRewardButtonObject.SetActive(false);
				dailyHonorRewardDisabledObject.SetActive(true);
			}
		}
		string[] array = myPVPData.reward_honor.item.Split('_');
		string empty = string.Empty;
		if (array.Length > 1)
		{
			empty = array[1];
			if (myPVPData.reward_honor.grade < 0)
			{
				empty = "0";
			}
			dailyHonorRewardText.text = I18NManager.Get("REWARD") + " : <color=#FAD725>" + int.Parse(empty).ToString("N0") + I18NManager.Get("KOREAN_COUNT") + "</color>";
			seasonRankingText.text = string.Format(I18NManager.Get("RANK"), (myPVPData.reward_season_rank.rank <= 0) ? "--" : myPVPData.reward_season_rank.rank.ToString("N0"));
			if (myPVPData.reward_season_rank.entry_yn.Contains("Y"))
			{
				seasonRankingRewardButtonObject.SetActive(false);
				seasonRankingRewardCompleteObject.SetActive(true);
				seasonRankingRewardDisabledObject.SetActive(false);
			}
			else
			{
				seasonRankingRewardCompleteObject.SetActive(false);
				if (myPVPData.reward_season_rank.active_yn.Contains("Y"))
				{
					seasonRankingRewardButtonObject.SetActive(true);
					seasonRankingRewardDisabledObject.SetActive(false);
				}
				else
				{
					seasonRankingRewardButtonObject.SetActive(false);
					seasonRankingRewardDisabledObject.SetActive(true);
				}
			}
			array = myPVPData.reward_season_rank.item.Split('_');
			string empty2 = string.Empty;
			if (array.Length > 1)
			{
				empty2 = array[1];
				if (myPVPData.reward_season_rank.rank <= 0)
				{
					empty2 = "0";
				}
				seasonRankingRewardText.text = I18NManager.Get("REWARD") + " : <color=#FAD725>" + int.Parse(empty2).ToString("N0") + I18NManager.Get("KOREAN_COUNT") + "</color>";
				int num2 = int.Parse(myPVPData.player.win);
				int num3 = int.Parse(myPVPData.player.lose);
				int num4 = num2 + num3;
				float num5 = (float)num2 / ((float)num2 + (float)num3) * 100f;
				if (float.IsNaN(num5))
				{
					num5 = 0f;
				}
				myRecordText.text = string.Format(I18NManager.Get("PVP_TOTAL_PLAY_TEXT"), num4) + " " + string.Format(I18NManager.Get("PVP_TOTAL_WIN"), num2) + " (" + string.Format("{0:0.#}", num5) + "%)";
				int num6 = int.Parse(myPVPData.reward_season.play_count);
				seasonTotalPlayCountText.text = string.Format(I18NManager.Get("PVP_TOTAL_PLAY_COUNT"), num6.ToString("N0"));
				seasonPlayCountImage.fillAmount = (float)num6 / 100f;
				if (num6 >= 10)
				{
					seasonPlayCountRewardOnOffIconImage1.sprite = Singleton<CachedManager>.instance.pvpSeasonRewardOnSprite;
				}
				else
				{
					seasonPlayCountRewardOnOffIconImage1.sprite = Singleton<CachedManager>.instance.pvpSeasonRewardOffSprite;
				}
				if (num6 >= 30)
				{
					seasonPlayCountRewardOnOffIconImage2.sprite = Singleton<CachedManager>.instance.pvpSeasonRewardOnSprite;
				}
				else
				{
					seasonPlayCountRewardOnOffIconImage2.sprite = Singleton<CachedManager>.instance.pvpSeasonRewardOffSprite;
				}
				if (num6 >= 60)
				{
					seasonPlayCountRewardOnOffIconImage3.sprite = Singleton<CachedManager>.instance.pvpSeasonRewardOnSprite;
				}
				else
				{
					seasonPlayCountRewardOnOffIconImage3.sprite = Singleton<CachedManager>.instance.pvpSeasonRewardOffSprite;
				}
				if (num6 >= 100)
				{
					seasonPlayCountRewardOnOffIconImage4.sprite = Singleton<CachedManager>.instance.pvpSeasonRewardOnSprite;
				}
				else
				{
					seasonPlayCountRewardOnOffIconImage4.sprite = Singleton<CachedManager>.instance.pvpSeasonRewardOffSprite;
				}
				seasonPlayCountRewardOnOffIconImage1.SetNativeSize();
				seasonPlayCountRewardOnOffIconImage2.SetNativeSize();
				seasonPlayCountRewardOnOffIconImage3.SetNativeSize();
				seasonPlayCountRewardOnOffIconImage4.SetNativeSize();
				bool flag = false;
				if (myPVPData.reward_season.s1_entry_yn.Contains("Y"))
				{
					seasonPlayCountRewardRecieveButtonObject1.SetActive(false);
					seasonPlayCountRewardRecieveDisabledButtonObject1.SetActive(false);
					seasonPlayCountCompleteObject1.SetActive(true);
				}
				else
				{
					seasonPlayCountCompleteObject1.SetActive(false);
					if (myPVPData.reward_season.s1_active_yn.Contains("Y"))
					{
						seasonPlayCountRewardRecieveButtonObject1.SetActive(true);
						seasonPlayCountRewardRecieveDisabledButtonObject1.SetActive(false);
						flag = true;
					}
					else
					{
						seasonPlayCountRewardRecieveButtonObject1.SetActive(false);
						seasonPlayCountRewardRecieveDisabledButtonObject1.SetActive(true);
					}
				}
				if (myPVPData.reward_season.s2_entry_yn.Contains("Y"))
				{
					seasonPlayCountRewardRecieveButtonObject2.SetActive(false);
					seasonPlayCountRewardRecieveDisabledButtonObject2.SetActive(false);
					seasonPlayCountCompleteObject2.SetActive(true);
				}
				else
				{
					seasonPlayCountCompleteObject2.SetActive(false);
					if (myPVPData.reward_season.s2_active_yn.Contains("Y") && !flag)
					{
						flag = true;
						seasonPlayCountRewardRecieveButtonObject2.SetActive(true);
						seasonPlayCountRewardRecieveDisabledButtonObject2.SetActive(false);
					}
					else
					{
						seasonPlayCountRewardRecieveButtonObject2.SetActive(false);
						seasonPlayCountRewardRecieveDisabledButtonObject2.SetActive(true);
					}
				}
				if (myPVPData.reward_season.s3_entry_yn.Contains("Y"))
				{
					seasonPlayCountRewardRecieveButtonObject3.SetActive(false);
					seasonPlayCountRewardRecieveDisabledButtonObject3.SetActive(false);
					seasonPlayCountCompleteObject3.SetActive(true);
				}
				else
				{
					seasonPlayCountCompleteObject3.SetActive(false);
					if (myPVPData.reward_season.s3_active_yn.Contains("Y") && !flag)
					{
						flag = true;
						seasonPlayCountRewardRecieveButtonObject3.SetActive(true);
						seasonPlayCountRewardRecieveDisabledButtonObject3.SetActive(false);
					}
					else
					{
						seasonPlayCountRewardRecieveButtonObject3.SetActive(false);
						seasonPlayCountRewardRecieveDisabledButtonObject3.SetActive(true);
					}
				}
				if (myPVPData.reward_season.s4_entry_yn.Contains("Y"))
				{
					seasonPlayCountRewardRecieveButtonObject4.SetActive(false);
					seasonPlayCountRewardRecieveDisabledButtonObject4.SetActive(false);
					seasonPlayCountCompleteObject4.SetActive(true);
				}
				else
				{
					seasonPlayCountCompleteObject4.SetActive(false);
					if (myPVPData.reward_season.s4_active_yn.Contains("Y") && !flag)
					{
						flag = true;
						seasonPlayCountRewardRecieveButtonObject4.SetActive(true);
						seasonPlayCountRewardRecieveDisabledButtonObject4.SetActive(false);
					}
					else
					{
						seasonPlayCountRewardRecieveButtonObject4.SetActive(false);
						seasonPlayCountRewardRecieveDisabledButtonObject4.SetActive(true);
					}
				}
				return true;
			}
			UIWindowDialog.openDescription("ERROR_FOR_TOWER_MODE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			return false;
		}
		UIWindowDialog.openDescription("ERROR_FOR_TOWER_MODE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		return false;
	}

	public void openTab(int targetTabType)
	{
		openTab((PVPUITabType)targetTabType);
	}

	private void openTabObject(PVPUITabType targetTabType)
	{
		for (int i = 0; i < tabObjectList.Count; i++)
		{
			if (i == (int)targetTabType)
			{
				if (!tabObjectList[i].activeSelf)
				{
					tabObjectList[i].SetActive(true);
				}
				tabImageList[i].sprite = Singleton<CachedManager>.instance.pvpTabSelectedSprite;
				tabTextList[i].color = Color.white;
				tabButtonTransformList[i].SetAsLastSibling();
			}
			else
			{
				if (tabObjectList[i].activeSelf)
				{
					tabObjectList[i].SetActive(false);
				}
				tabImageList[i].sprite = Singleton<CachedManager>.instance.pvpTabNonSelectedSprite;
				tabTextList[i].color = Util.getCalculatedColor(139f, 139f, 162f);
				tabButtonTransformList[i].SetAsFirstSibling();
			}
		}
	}

	public void openTab(PVPUITabType targetTabType)
	{
		if (currentTabType == targetTabType)
		{
			return;
		}
		currentTabType = targetTabType;
		switch (currentTabType)
		{
		case PVPUITabType.SeasonShopTab:
		{
			Singleton<PVPManager>.instance.displayPVPHonorToken();
			Singleton<PVPManager>.instance.checkPVPShopData();
			openTabObject(targetTabType);
			for (int i = 0; i < pvpShopslots.Length; i++)
			{
				pvpShopslots[i].initPVPShopSlot(Singleton<DataManager>.instance.currentGameData.pvpShopItemList[i]);
			}
			m_isFinishRefreshTime = false;
			break;
		}
		case PVPUITabType.BattleTab:
			openTabObject(targetTabType);
			break;
		case PVPUITabType.SeasonRankingTab:
		{
			m_isLoadingExtraUser = false;
			Singleton<PVPManager>.instance.seasonTotalRankingList.Clear();
			m_currentSeasonRankingStartValue = 1;
			m_currentSeasonRankingEndValue = m_currentSeasonRankingStartValue + intervalRankingLoadCount;
			Dictionary<PVPManager.FuntionParameterType, double> dictionary2 = new Dictionary<PVPManager.FuntionParameterType, double>();
			dictionary2.Add(PVPManager.FuntionParameterType.RankingStart, m_currentSeasonRankingStartValue);
			dictionary2.Add(PVPManager.FuntionParameterType.RankingEnd, m_currentSeasonRankingEndValue - 1);
			if (!seasonRankingEmptyObject.activeSelf)
			{
				seasonRankingEmptyObject.SetActive(true);
			}
			if (seasonRankingScroll.cachedGameObject.activeSelf)
			{
				seasonRankingScroll.cachedGameObject.SetActive(false);
			}
			mySeasonRankingData = null;
			Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.GET_SEASON_RANKING, dictionary2, delegate(bool isSuccess)
			{
				if (isSuccess)
				{
					mySeasonRankingData = Singleton<PVPManager>.instance.currentSeasonRankingResponse.me;
					if (mySeasonRankingData != null)
					{
						openTabObject(targetTabType);
						seasonRankingMyNameText.text = mySeasonRankingData.nickname;
						seasonRankingMyRankText.text = mySeasonRankingData.rank.ToString("N0");
						seasonRankingMyRecordText.text = string.Format(I18NManager.Get("PVP_MMR"), mySeasonRankingData.point) + " <color=#FDFCB7>/ " + string.Format(I18NManager.Get("PVP_TOTAL_PLAY_TEXT"), int.Parse(mySeasonRankingData.win) + int.Parse(mySeasonRankingData.lose)) + " " + string.Format(I18NManager.Get("PVP_TOTAL_WIN"), mySeasonRankingData.win) + "</color>";
						seasonRankingMyRankingWarriorUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin, cachedCanvasGroup, "PopUpLayer", 120);
						seasonRankingMyRankingPriestUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedPriestSkin, cachedCanvasGroup, "PopUpLayer", 120);
						seasonRankingMyRankingArcherUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedArcherSkin, cachedCanvasGroup, "PopUpLayer", 120);
					}
					if (seasonRankingEmptyObject.activeSelf)
					{
						seasonRankingEmptyObject.SetActive(false);
					}
					if (!seasonRankingScroll.cachedGameObject.activeSelf)
					{
						seasonRankingScroll.cachedGameObject.SetActive(true);
					}
					seasonRankingScroll.refreshMaxCount(Singleton<PVPManager>.instance.seasonTotalRankingList.Count);
					seasonRankingScroll.syncAllSlotIndexFromPosition();
					seasonRankingMask.enabled = false;
					seasonRankingMask.enabled = true;
				}
				else
				{
					if (!seasonRankingEmptyObject.activeSelf)
					{
						seasonRankingEmptyObject.SetActive(true);
					}
					if (seasonRankingScroll.cachedGameObject.activeSelf)
					{
						seasonRankingScroll.cachedGameObject.SetActive(false);
					}
				}
			});
			break;
		}
		case PVPUITabType.HonorRankingTab:
			Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.GET_HONOR_RANKING, delegate(bool isSuccess)
			{
				if (isSuccess)
				{
					openTabObject(targetTabType);
					honorRankingScroll.refreshMaxCount(Singleton<PVPManager>.instance.currentHonorRankingResponse.honor.Length);
					honorRankingScroll.syncAllSlotIndexFromPosition();
					honorRankingMask.enabled = false;
					honorRankingMask.enabled = true;
				}
			});
			break;
		case PVPUITabType.RecordHistoryTab:
		{
			Dictionary<PVPManager.FuntionParameterType, double> dictionary = new Dictionary<PVPManager.FuntionParameterType, double>();
			dictionary.Add(PVPManager.FuntionParameterType.RankingStart, 1.0);
			dictionary.Add(PVPManager.FuntionParameterType.HistoryLoadInterval, 20.0);
			Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.GET_HISTORY_RECORD, dictionary, delegate(bool isSuccess)
			{
				if (isSuccess)
				{
					openTabObject(targetTabType);
					histroyScroll.refreshMaxCount(Singleton<PVPManager>.instance.currentHistoryData.history.Length);
					histroyScroll.syncAllSlotIndexFromPosition();
					historyMask.enabled = false;
					historyMask.enabled = true;
				}
			});
			break;
		}
		}
	}

	public void refreshAddTicketButtonObject()
	{
		addTicketButtonObject.SetActive((long)Singleton<DataManager>.instance.currentGameData.pvpTicketCount <= 0);
	}

	private void refreshTicketRemainText()
	{
		if ((long)Singleton<DataManager>.instance.currentGameData.pvpTicketCount >= PVPManager.maxTicketCount)
		{
			if (startMatchingButtonRectTransform.anchoredPosition.x != 0f)
			{
				startMatchingButtonRectTransform.anchoredPosition = new Vector2(0f, -333.8f);
			}
			ticketText.text = string.Concat(Singleton<DataManager>.instance.currentGameData.pvpTicketCount, "/", PVPManager.maxTicketCount);
			return;
		}
		if ((long)Singleton<DataManager>.instance.currentGameData.pvpTicketCount <= 0)
		{
			if (startMatchingButtonRectTransform.anchoredPosition.x != -85f)
			{
				startMatchingButtonRectTransform.anchoredPosition = new Vector2(-85f, -333.8f);
				refreshAddTicketButtonObject();
			}
		}
		else if (startMatchingButtonRectTransform.anchoredPosition.x != 0f)
		{
			startMatchingButtonRectTransform.anchoredPosition = new Vector2(0f, -333.8f);
		}
		TimeSpan timeSpan = new TimeSpan(new DateTime(Singleton<DataManager>.instance.currentGameData.lastPVPStartTime).AddMinutes(30.0).Ticks - UnbiasedTime.Instance.UtcNow().Ticks);
		string text = string.Format("{0:00}:{1:00}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
		ticketText.text = string.Concat(Singleton<DataManager>.instance.currentGameData.pvpTicketCount, "/", PVPManager.maxTicketCount, " <size=20>(", text, ")</size>");
	}

	public void OnClickChargeTicket()
	{
		UIWindowDialog.openDescription("PVP_TICKET_BUT_DESCRIPTION_2", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			if (Singleton<DataManager>.instance.currentGameData._ruby >= 100)
			{
				Singleton<RubyManager>.instance.decreaseRuby(100.0);
				Singleton<PVPManager>.instance.increaseTicket(PVPManager.maxTicketCount, false);
				refreshTicketRemainText();
				refreshAddTicketButtonObject();
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, string.Empty);
			}
		}, string.Empty);
	}

	public void OnClickStartMatching()
	{
		if ((long)Singleton<DataManager>.instance.currentGameData.pvpTicketCount > 0)
		{
			Singleton<PVPManager>.instance.decreaseTicket(1);
			UIWindowPVPMatching.instance.openPVPMatching();
			return;
		}
		UIWindowDialog.openDescription("PVP_TICKET_BUT_DESCRIPTION_1", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			if (Singleton<DataManager>.instance.currentGameData._ruby >= 100)
			{
				Singleton<RubyManager>.instance.decreaseRuby(100.0);
				Singleton<PVPManager>.instance.increaseTicket(PVPManager.maxTicketCount, false);
				refreshTicketRemainText();
				refreshAddTicketButtonObject();
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, string.Empty);
			}
		}, string.Empty);
	}

	public void OnClickTierReward()
	{
		Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.REWARD_HONOR, delegate(bool isSuccess)
		{
			if (isSuccess)
			{
				UIWindowDialog.openDescription("PVP_REWARD_SUCCESS_SEND", UIWindowDialog.DialogState.DelegateOKUI, delegate
				{
					openPVPMainUI();
				}, string.Empty);
			}
		});
	}

	public void OnClickSeasonRankReward()
	{
		Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.REWARD_SEASON_RANKING, delegate(bool isSuccess)
		{
			if (isSuccess)
			{
				UIWindowDialog.openDescription("PVP_REWARD_SUCCESS_SEND", UIWindowDialog.DialogState.DelegateOKUI, delegate
				{
					openPVPMainUI();
				}, string.Empty);
			}
		});
	}

	public void OnClickSeasonPlayCountReward(int index)
	{
		Dictionary<PVPManager.FuntionParameterType, double> dictionary = new Dictionary<PVPManager.FuntionParameterType, double>();
		dictionary.Add(PVPManager.FuntionParameterType.RewardIndex, index);
		Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.REWARD_SEASON_ENTRY, dictionary, delegate(bool isSuccess)
		{
			if (isSuccess)
			{
				UIWindowDialog.openDescription("PVP_REWARD_SUCCESS_SEND", UIWindowDialog.DialogState.DelegateOKUI, delegate
				{
					openPVPMainUI();
				}, string.Empty);
			}
		});
	}

	public void loadExtraRanking()
	{
		m_currentSeasonRankingStartValue = Singleton<PVPManager>.instance.seasonTotalRankingList.Count + 1;
		m_currentSeasonRankingEndValue = m_currentSeasonRankingStartValue + intervalRankingLoadCount;
		Dictionary<PVPManager.FuntionParameterType, double> dictionary = new Dictionary<PVPManager.FuntionParameterType, double>();
		dictionary.Add(PVPManager.FuntionParameterType.RankingStart, m_currentSeasonRankingStartValue);
		dictionary.Add(PVPManager.FuntionParameterType.RankingEnd, m_currentSeasonRankingEndValue - 1);
		Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.GET_SEASON_RANKING, dictionary, delegate
		{
			if (!seasonRankingScroll.cachedGameObject.activeSelf)
			{
				seasonRankingScroll.cachedGameObject.SetActive(true);
			}
			seasonRankingScroll.refreshMaxCount(Singleton<PVPManager>.instance.seasonTotalRankingList.Count);
			seasonRankingScroll.syncAllSlotIndexFromPosition();
			seasonRankingMask.enabled = false;
			seasonRankingMask.enabled = true;
			m_isLoadingExtraUser = false;
		});
	}

	private void Update()
	{
		refreshTicketRemainText();
		if (currentSeasonEndDateTime.Ticks > UnbiasedTime.Instance.UtcNow().Ticks)
		{
			refreshSeasonTimer();
		}
		else if (!m_isChangingNextSeason)
		{
			m_isChangingNextSeason = true;
			nextSeasonTimerText.text = "Loading..";
			openPVPMainUI();
		}
		if (currentTabType == PVPUITabType.SeasonRankingTab)
		{
			if (m_isLoadingExtraUser || Input.touchCount > 0 || Input.GetMouseButton(0))
			{
				return;
			}
			if (seasonRankingScrollContent.rect.height < seasonRankingScrollTransform.rect.height)
			{
				if (seasonRankingScrollContent.anchoredPosition.y > 45f)
				{
					loadExtraRanking();
					m_isLoadingExtraUser = true;
				}
				return;
			}
			float num = seasonRankingScrollContent.rect.height - seasonRankingScrollTransform.rect.height;
			if (seasonRankingScrollContent.anchoredPosition.y > num + 45f)
			{
				loadExtraRanking();
				m_isLoadingExtraUser = true;
			}
		}
		else
		{
			if (currentTabType != 0)
			{
				return;
			}
			TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.UtcNow().Ticks - (long)Singleton<DataManager>.instance.currentGameData.pvpShopTargetRefreshTime);
			long num2 = (long)(timeSpan.TotalHours / (double)PVPManager.pvpShopRefreshTimeHour) + 1;
			if ((long)Singleton<DataManager>.instance.currentGameData.pvpShopShopItemRefreshCount == num2)
			{
				TimeSpan timeSpan2 = new TimeSpan(new DateTime(Singleton<DataManager>.instance.currentGameData.pvpShopTargetRefreshTime).AddHours(PVPManager.pvpShopRefreshTimeHour * num2).Ticks - UnbiasedTime.Instance.UtcNow().Ticks);
				string text = I18NManager.Get("REFRESH_TARGET_TIME") + " : <color=white>" + string.Format((((long)timeSpan2.TotalHours <= 0) ? string.Empty : "{0:00}:") + "{1:00}:{2:00}", (int)timeSpan2.TotalHours, timeSpan2.Minutes, timeSpan2.Seconds) + "</color>";
				pvpShopRefreshRemainText.text = text;
				m_isFinishRefreshTime = false;
				return;
			}
			if (pvpShopRefreshRemainText.text != string.Empty)
			{
				pvpShopRefreshRemainText.text = string.Empty;
			}
			if (!m_isFinishRefreshTime)
			{
				m_isFinishRefreshTime = true;
				Singleton<PVPManager>.instance.checkPVPShopData();
				for (int i = 0; i < pvpShopslots.Length; i++)
				{
					pvpShopslots[i].initPVPShopSlot(Singleton<DataManager>.instance.currentGameData.pvpShopItemList[i]);
				}
				UIWindowDialog.openMiniDialog("ELOPE_REFRESH_SUCCESS");
			}
		}
	}

	public void OnClickRefreshShopItemList()
	{
		UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("ELOPE_REFRESH_DESCRIPTION"), PVPManager.pvpShopRefreshPrice), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			if (Singleton<DataManager>.instance.currentGameData._ruby >= PVPManager.pvpShopRefreshPrice)
			{
				Singleton<RubyManager>.instance.decreaseRuby(PVPManager.pvpShopRefreshPrice);
				Singleton<PVPManager>.instance.refreshPVPShopItems();
				for (int i = 0; i < pvpShopslots.Length; i++)
				{
					pvpShopslots[i].initPVPShopSlot(Singleton<DataManager>.instance.currentGameData.pvpShopItemList[i]);
				}
				UIWindowDialog.openMiniDialog("ELOPE_REFRESH_SUCCESS");
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, string.Empty);
			}
		}, string.Empty);
	}
}
