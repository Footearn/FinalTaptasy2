using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowTowerModeRanking : UIWindow
{
	public enum TabType
	{
		None = -1,
		SeasonRankingTimeAttackMode,
		SeasonRankingEndlessMode,
		HonorRankingTimeAttackMode,
		HonorRankingEndlessMode,
		Length
	}

	public static UIWindowTowerModeRanking instance;

	public TabType currentTabType;

	public Sprite[] rankingIconSprites;

	public CanvasGroup cachedCanvasGroup;

	public GameObject seasonScrollObject;

	public GameObject seasonNoEmptyRankingObject;

	public GameObject seasonEmptyRankingObject;

	public TowerModeRankingInfinityScroll seasonInfiniteScrollRect;

	public RectTransform seasonRankingScrollContent;

	public RectTransform seasonRankingScrollTransform;

	public GameObject honorScrollObject;

	public GameObject honorNoEmptyRankingObject;

	public GameObject honorEmptyRankingObject;

	public TowerModeRankingInfinityScroll honorInfiniteScrollRect;

	public Text myRankingText;

	public Text myNameText;

	public Text myRecordText;

	public CharacterUIObject myWarriorUIObject;

	public CharacterUIObject myPriestUIObject;

	public CharacterUIObject myArcherUIObject;

	public Image timeAttackModeTabButtonImage;

	public Text timeAttackModeTabButtonText;

	public Image endlessModeTabButtonImage;

	public Text endlessModeTabButtonText;

	public Image seasonButtonTabButtonImage;

	public Text seasonButtonTabButtonText;

	public Image honorButtonTabButtonImage;

	public Text honorButtonTabButtonText;

	public SpriteMask[] spriteMasks;

	private int m_intervalLoadRankingCount = 10;

	private int m_currentTimeAttackRankingStartValue;

	private int m_currentTimeAttackRankingEndValue;

	private int m_currentEndlessRankingStartValue;

	private int m_currentEndlessRankingEndValue;

	private int m_loadedAPICount;

	private bool m_timeAttackModeOnFirstOpen;

	private List<TabType> m_isInitializedSpriteMask = new List<TabType>();

	private bool m_isLoadingExtraUser;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		currentTabType = TabType.None;
		return base.OnBeforeOpen();
	}

	public override void OnAfterClose()
	{
		currentTabType = TabType.None;
		base.OnAfterClose();
	}

	public void openTowerModeRanking(bool isTimeAttackMode)
	{
		m_isLoadingExtraUser = false;
		if (Singleton<TowerModeManager>.instance.totalTimeAttackSeasonRankingDataList != null)
		{
			Singleton<TowerModeManager>.instance.totalTimeAttackSeasonRankingDataList.Clear();
		}
		if (Singleton<TowerModeManager>.instance.totalEndlessSeasonRankingDataList != null)
		{
			Singleton<TowerModeManager>.instance.totalEndlessSeasonRankingDataList.Clear();
		}
		m_timeAttackModeOnFirstOpen = isTimeAttackMode;
		myNameText.text = Social.localUser.userName;
		m_loadedAPICount = 0;
		m_currentTimeAttackRankingStartValue = 1;
		m_currentTimeAttackRankingEndValue = m_currentTimeAttackRankingStartValue + m_intervalLoadRankingCount;
		m_currentEndlessRankingStartValue = 1;
		m_currentEndlessRankingEndValue = m_currentEndlessRankingStartValue + m_intervalLoadRankingCount;
		Dictionary<TowerModeManager.FuntionParameterType, double> dictionary = new Dictionary<TowerModeManager.FuntionParameterType, double>();
		dictionary.Add(TowerModeManager.FuntionParameterType.RankingStart, m_currentTimeAttackRankingStartValue);
		dictionary.Add(TowerModeManager.FuntionParameterType.RankingEnd, m_currentTimeAttackRankingEndValue - 1);
		Singleton<TowerModeManager>.instance.CallAPI(TowerModeManager.TowerModeAPIType.GET_ALL_RANKING, true, dictionary, delegate(bool isSuccess)
		{
			loadFinishEvent(isSuccess);
		});
		dictionary.Clear();
		dictionary.Add(TowerModeManager.FuntionParameterType.RankingStart, m_currentEndlessRankingStartValue);
		dictionary.Add(TowerModeManager.FuntionParameterType.RankingEnd, m_currentEndlessRankingEndValue - 1);
		Singleton<TowerModeManager>.instance.CallAPI(TowerModeManager.TowerModeAPIType.GET_ALL_RANKING, false, dictionary, delegate(bool isSuccess)
		{
			loadFinishEvent(isSuccess);
		});
		Singleton<TowerModeManager>.instance.CallAPI(TowerModeManager.TowerModeAPIType.GET_HONOR_RANKING, true, null, delegate(bool isSuccess)
		{
			loadFinishEvent(isSuccess);
		});
		Singleton<TowerModeManager>.instance.CallAPI(TowerModeManager.TowerModeAPIType.GET_HONOR_RANKING, false, null, delegate(bool isSuccess)
		{
			loadFinishEvent(isSuccess);
		});
	}

	private void refreshMyData(TowerModeManager.UserInformationData myData, bool isTimeAttackMode)
	{
		if (myData == null)
		{
			return;
		}
		int result = -1;
		int.TryParse(myData.rank, out result);
		int result2 = -1;
		int.TryParse(myData.clear_floor, out result2);
		if (result2 < 0)
		{
			result2 = 0;
		}
		float result3 = -1f;
		float.TryParse(myData.clear_time, out result3);
		result3 = ((result3 == -1f) ? 0f : (result3 / 1000f));
		myRankingText.text = ((result == -1) ? "--" : result.ToString());
		string empty = string.Empty;
		empty = ((!isTimeAttackMode || result2 < Singleton<TowerModeManager>.instance.getTotalFloor(TowerModeManager.TowerModeDifficultyType.TimeAttack)) ? string.Format(I18NManager.Get("FLOOR_TEXT"), result2) : I18NManager.Get("CLEAR"));
		TimeSpan timeSpan = TimeSpan.FromSeconds(result3);
		myRecordText.text = empty + "<color=#FDFCB7> / " + string.Format("{0:00}:{1:00}.{2:000}", (int)timeSpan.TotalMinutes, timeSpan.Seconds, timeSpan.Milliseconds) + "</color>";
		string[] array = ((myData.game_data == null) ? null : myData.game_data.Split(','));
		if (array != null && array.Length == 3)
		{
			CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)int.Parse(array[0]);
			CharacterSkinManager.PriestSkinType priestSkinType = (CharacterSkinManager.PriestSkinType)int.Parse(array[1]);
			CharacterSkinManager.ArcherSkinType archerSkinType = (CharacterSkinManager.ArcherSkinType)int.Parse(array[2]);
			if (warriorSkinType < CharacterSkinManager.WarriorSkinType.Length)
			{
				myWarriorUIObject.initCharacterUIObject(warriorSkinType, cachedCanvasGroup);
			}
			else
			{
				myWarriorUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin, cachedCanvasGroup);
			}
			if (priestSkinType < CharacterSkinManager.PriestSkinType.Length)
			{
				myPriestUIObject.initCharacterUIObject(priestSkinType, cachedCanvasGroup);
			}
			else
			{
				myPriestUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedPriestSkin, cachedCanvasGroup);
			}
			if (archerSkinType < CharacterSkinManager.ArcherSkinType.Length)
			{
				myArcherUIObject.initCharacterUIObject(archerSkinType, cachedCanvasGroup);
			}
			else
			{
				myArcherUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedArcherSkin, cachedCanvasGroup);
			}
		}
		else
		{
			myWarriorUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin, cachedCanvasGroup);
			myPriestUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedPriestSkin, cachedCanvasGroup);
			myArcherUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedArcherSkin, cachedCanvasGroup);
		}
	}

	private void loadFinishEvent(bool isSuccess)
	{
		m_loadedAPICount++;
		if (m_loadedAPICount >= 4)
		{
			open();
			openTab((!m_timeAttackModeOnFirstOpen) ? TabType.SeasonRankingEndlessMode : TabType.SeasonRankingTimeAttackMode);
		}
	}

	public void loadExtraRanking(bool isTimeAttackMode)
	{
		if (isTimeAttackMode)
		{
			m_currentTimeAttackRankingStartValue = Singleton<TowerModeManager>.instance.totalTimeAttackSeasonRankingDataList.Count + 1;
			m_currentTimeAttackRankingEndValue = m_currentTimeAttackRankingStartValue + m_intervalLoadRankingCount;
			Dictionary<TowerModeManager.FuntionParameterType, double> dictionary = new Dictionary<TowerModeManager.FuntionParameterType, double>();
			dictionary.Add(TowerModeManager.FuntionParameterType.RankingStart, m_currentTimeAttackRankingStartValue);
			dictionary.Add(TowerModeManager.FuntionParameterType.RankingEnd, m_currentTimeAttackRankingEndValue - 1);
			Singleton<TowerModeManager>.instance.CallAPI(TowerModeManager.TowerModeAPIType.GET_ALL_RANKING, true, dictionary, delegate(bool isSuccess)
			{
				loadExtraRankingCallback(isSuccess);
			});
		}
		else
		{
			m_currentEndlessRankingStartValue = Singleton<TowerModeManager>.instance.totalEndlessSeasonRankingDataList.Count + 1;
			m_currentEndlessRankingEndValue = m_currentEndlessRankingStartValue + m_intervalLoadRankingCount;
			Dictionary<TowerModeManager.FuntionParameterType, double> dictionary2 = new Dictionary<TowerModeManager.FuntionParameterType, double>();
			dictionary2.Add(TowerModeManager.FuntionParameterType.RankingStart, m_currentEndlessRankingStartValue);
			dictionary2.Add(TowerModeManager.FuntionParameterType.RankingEnd, m_currentEndlessRankingEndValue - 1);
			Singleton<TowerModeManager>.instance.CallAPI(TowerModeManager.TowerModeAPIType.GET_ALL_RANKING, false, dictionary2, delegate(bool isSuccess)
			{
				loadExtraRankingCallback(isSuccess);
			});
		}
	}

	private void loadExtraRankingCallback(bool isSuccess)
	{
		if (isSuccess)
		{
			openTab(currentTabType, true);
		}
	}

	public void openTab(TabType tabType, bool forceOpen = false)
	{
		if (currentTabType == tabType && !forceOpen)
		{
			return;
		}
		m_isLoadingExtraUser = false;
		seasonInfiniteScrollRect.parentScrollRect.vertical = true;
		currentTabType = tabType;
		if (currentTabType == TabType.SeasonRankingTimeAttackMode || currentTabType == TabType.SeasonRankingEndlessMode)
		{
			seasonButtonTabButtonImage.sprite = Singleton<CachedManager>.instance.towerModeSeasonAndHonorTabSelectedSprite;
			seasonButtonTabButtonText.color = Color.white;
			honorButtonTabButtonImage.sprite = Singleton<CachedManager>.instance.towerModeSeasonAndHonorTabNonSelectedSprite;
			honorButtonTabButtonText.color = Util.getCalculatedColor(65f, 133f, 178f);
			switch (currentTabType)
			{
			case TabType.SeasonRankingTimeAttackMode:
				timeAttackModeTabButtonImage.sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
				timeAttackModeTabButtonText.color = Color.white;
				endlessModeTabButtonImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
				endlessModeTabButtonText.color = Util.getCalculatedColor(65f, 133f, 178f);
				refreshMyData(Singleton<TowerModeManager>.instance.myTimeAttackRankingData.player, true);
				break;
			case TabType.SeasonRankingEndlessMode:
				endlessModeTabButtonImage.sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
				endlessModeTabButtonText.color = Color.white;
				timeAttackModeTabButtonImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
				timeAttackModeTabButtonText.color = Util.getCalculatedColor(65f, 133f, 178f);
				refreshMyData(Singleton<TowerModeManager>.instance.myEndlessRankingData.player, false);
				break;
			}
			if (!seasonScrollObject.activeSelf)
			{
				seasonScrollObject.SetActive(true);
			}
			if (honorScrollObject.activeSelf)
			{
				honorScrollObject.SetActive(false);
			}
		}
		else
		{
			seasonButtonTabButtonImage.sprite = Singleton<CachedManager>.instance.towerModeSeasonAndHonorTabNonSelectedSprite;
			seasonButtonTabButtonText.color = Util.getCalculatedColor(65f, 133f, 178f);
			honorButtonTabButtonImage.sprite = Singleton<CachedManager>.instance.towerModeSeasonAndHonorTabSelectedSprite;
			honorButtonTabButtonText.color = Color.white;
			switch (currentTabType)
			{
			case TabType.HonorRankingTimeAttackMode:
				timeAttackModeTabButtonImage.sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
				timeAttackModeTabButtonText.color = Color.white;
				endlessModeTabButtonImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
				endlessModeTabButtonText.color = Util.getCalculatedColor(65f, 133f, 178f);
				refreshMyData(Singleton<TowerModeManager>.instance.myTimeAttackRankingData.player, true);
				break;
			case TabType.HonorRankingEndlessMode:
				endlessModeTabButtonImage.sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
				endlessModeTabButtonText.color = Color.white;
				timeAttackModeTabButtonImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
				timeAttackModeTabButtonText.color = Util.getCalculatedColor(65f, 133f, 178f);
				refreshMyData(Singleton<TowerModeManager>.instance.myEndlessRankingData.player, false);
				break;
			}
			if (seasonScrollObject.activeSelf)
			{
				seasonScrollObject.SetActive(false);
			}
			if (!honorScrollObject.activeSelf)
			{
				honorScrollObject.SetActive(true);
			}
		}
		int num = 0;
		switch (currentTabType)
		{
		case TabType.SeasonRankingTimeAttackMode:
			num = ((Singleton<TowerModeManager>.instance.totalTimeAttackSeasonRankingDataList != null) ? Singleton<TowerModeManager>.instance.totalTimeAttackSeasonRankingDataList.Count : 0);
			if (num > 0)
			{
				seasonInfiniteScrollRect.refreshMaxCount(num);
				if (!forceOpen)
				{
					seasonInfiniteScrollRect.resetContentPosition(Vector2.zero);
				}
				seasonInfiniteScrollRect.syncAllSlotIndexFromPosition();
			}
			break;
		case TabType.SeasonRankingEndlessMode:
			num = ((Singleton<TowerModeManager>.instance.totalEndlessSeasonRankingDataList != null) ? Singleton<TowerModeManager>.instance.totalEndlessSeasonRankingDataList.Count : 0);
			if (num > 0)
			{
				seasonInfiniteScrollRect.refreshMaxCount(num);
				if (!forceOpen)
				{
					seasonInfiniteScrollRect.resetContentPosition(Vector2.zero);
				}
				seasonInfiniteScrollRect.syncAllSlotIndexFromPosition();
			}
			break;
		case TabType.HonorRankingTimeAttackMode:
			num = ((Singleton<TowerModeManager>.instance.totalTimeAttackHonorRankingData != null && Singleton<TowerModeManager>.instance.totalTimeAttackHonorRankingData.honor != null) ? Singleton<TowerModeManager>.instance.totalTimeAttackHonorRankingData.honor.Count : 0);
			if (num > 0)
			{
				honorInfiniteScrollRect.refreshMaxCount(num);
				if (!forceOpen)
				{
					honorInfiniteScrollRect.resetContentPosition(Vector2.zero);
				}
				honorInfiniteScrollRect.syncAllSlotIndexFromPosition();
			}
			break;
		case TabType.HonorRankingEndlessMode:
			num = ((Singleton<TowerModeManager>.instance.totalEndlessHonorRankingData != null && Singleton<TowerModeManager>.instance.totalEndlessHonorRankingData.honor != null) ? Singleton<TowerModeManager>.instance.totalEndlessHonorRankingData.honor.Count : 0);
			if (num > 0)
			{
				honorInfiniteScrollRect.refreshMaxCount(num);
				if (!forceOpen)
				{
					honorInfiniteScrollRect.resetContentPosition(Vector2.zero);
				}
				honorInfiniteScrollRect.syncAllSlotIndexFromPosition();
			}
			break;
		}
		if (currentTabType == TabType.SeasonRankingTimeAttackMode || currentTabType == TabType.SeasonRankingEndlessMode)
		{
			if (num <= 0)
			{
				if (seasonNoEmptyRankingObject.activeSelf)
				{
					seasonNoEmptyRankingObject.SetActive(false);
				}
				if (!seasonEmptyRankingObject.activeSelf)
				{
					seasonEmptyRankingObject.SetActive(true);
				}
			}
			else
			{
				if (!seasonNoEmptyRankingObject.activeSelf)
				{
					seasonNoEmptyRankingObject.SetActive(true);
				}
				if (seasonEmptyRankingObject.activeSelf)
				{
					seasonEmptyRankingObject.SetActive(false);
				}
			}
		}
		else if (currentTabType == TabType.HonorRankingTimeAttackMode || currentTabType == TabType.HonorRankingEndlessMode)
		{
			if (num <= 0)
			{
				if (seasonNoEmptyRankingObject.activeSelf)
				{
					seasonNoEmptyRankingObject.SetActive(false);
				}
				if (!honorEmptyRankingObject.activeSelf)
				{
					honorEmptyRankingObject.SetActive(true);
				}
			}
			else
			{
				if (!seasonNoEmptyRankingObject.activeSelf)
				{
					seasonNoEmptyRankingObject.SetActive(true);
				}
				if (honorEmptyRankingObject.activeSelf)
				{
					honorEmptyRankingObject.SetActive(false);
				}
			}
		}
		if (!m_isInitializedSpriteMask.Contains(currentTabType))
		{
			m_isInitializedSpriteMask.Add(currentTabType);
			StartCoroutine("waitForSpriteMask");
		}
	}

	private IEnumerator waitForSpriteMask()
	{
		yield return null;
		for (int i = 0; i < spriteMasks.Length; i++)
		{
			spriteMasks[i].update();
		}
	}

	public void OnClickModeTab(int type)
	{
		if (type == 0)
		{
			openTab(TabType.SeasonRankingTimeAttackMode);
		}
		else
		{
			openTab(TabType.SeasonRankingEndlessMode);
		}
	}

	public void OnClickSeasonOrHonor(int type)
	{
		if (type == 0)
		{
			if (currentTabType == TabType.HonorRankingTimeAttackMode)
			{
				openTab(TabType.SeasonRankingTimeAttackMode);
			}
			else if (currentTabType == TabType.HonorRankingEndlessMode)
			{
				openTab(TabType.SeasonRankingEndlessMode);
			}
		}
		else if (currentTabType == TabType.SeasonRankingTimeAttackMode)
		{
			openTab(TabType.HonorRankingTimeAttackMode);
		}
		else if (currentTabType == TabType.SeasonRankingEndlessMode)
		{
			openTab(TabType.HonorRankingEndlessMode);
		}
	}

	private void Update()
	{
		if ((currentTabType != 0 && currentTabType != TabType.SeasonRankingEndlessMode) || m_isLoadingExtraUser || Input.touchCount > 0 || Input.GetMouseButton(0))
		{
			return;
		}
		if (seasonRankingScrollContent.rect.height < seasonRankingScrollTransform.rect.height)
		{
			if (seasonRankingScrollContent.anchoredPosition.y > 45f)
			{
				loadExtraRanking(currentTabType == TabType.SeasonRankingTimeAttackMode);
				m_isLoadingExtraUser = true;
			}
			return;
		}
		float num = seasonRankingScrollContent.rect.height - seasonRankingScrollTransform.rect.height;
		if (seasonRankingScrollContent.anchoredPosition.y > num + 45f)
		{
			loadExtraRanking(currentTabType == TabType.SeasonRankingTimeAttackMode);
			m_isLoadingExtraUser = true;
		}
	}
}
