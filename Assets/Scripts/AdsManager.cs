using System;
using System.Collections;
using System.Collections.Generic;
using Fabric.Crashlytics;
using GoogleMobileAds.Api;
using TapjoyUnity;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : Singleton<AdsManager>
{
	private enum TargetAds
	{
		UnityAds,
		ApplovinAds,
		TapjoyAds,
		None
	}

	public const string GOLD_DUNGEON_ADS_NAME = "goldDungeon";

	public const string JACKPOT_ADS_NAME = "jackpotReward";

	public const string FREE_RUBY_NAME = "freeDiamond";

	public const string ADS_ANGEL_NAME = "adsAngel";

	public const string REVIVE_ADS_NAME = "reviveBossDungeon";

	public const string DOUBLE_SPEED_ADS_NAME = "doubleSpeedBossDungeon";

	public const string FREE_GOLD_NAME = "freeGold";

	public const string DEMON_KING_HANDSOME_GUY = "elopeads";

	public const string TOWER_MODE_FREE_TICKET = "towermodeads";

	public const string ADMOB_ADS = "interSkip";

	public const string SPECIAL_ADS_ANGEL_ZONE_ID = "vzf5626a989ad1464c85";

	private TargetAds targetAds = TargetAds.None;

	[NonSerialized]
	public int valueUnityAds = 10;

	[NonSerialized]
	public int valueApplovin = 50;

	[NonSerialized]
	public int valueTapjoy = 50;

	public static Action callbackFinishAd;

	private static TJPlacement offerwallPlacement;

	private static TJPlacement videoAdPlayPlacement;

	public static bool contentIsReadyForPlacement;

	private bool tapjoyIsConnected;

	private long tapjoyCurrency;

	private InterstitialAd m_interstitialForAdmob;

	private Action<bool> m_admobCloseAction;

	private bool m_isAdmobEnableCountry = true;

	private bool tapjoyVideoComplete;

	private bool setUserID;

	private bool isShowingAd;

	public bool isAdmobEnableCountry
	{
		get
		{
			return m_isAdmobEnableCountry;
		}
	}

	public bool IsShowingAd
	{
		get
		{
			return isShowingAd;
		}
	}

	private void Awake()
	{
		//TODO 注释
		// if (Advertisement.isSupported && !Advertisement.isInitialized)
		// {
		// 	bool flag = true;
		// 	flag = false;
		// 	Advertisement.Initialize("121321", flag);
		// }
		// Tapjoy.OnConnectSuccess += HandleConnectSuccess;
		// Tapjoy.OnConnectFailure += HandleConnectFailure;
		// Tapjoy.OnGetCurrencyBalanceResponse += HandleGetCurrencyBalanceResponse;
		// Tapjoy.OnEarnedCurrency += HandleEarnedCurrency;
		// Tapjoy.OnSpendCurrencyResponse += HandleSpendCurrencyResponse;
		// Tapjoy.OnSpendCurrencyResponseFailure += HandleSpendCurrencyResponseFailure;
		// TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
		// TJPlacement.OnContentReady += HandlePlacementContentReady;
		// TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
		// TJPlacement.OnVideoComplete += HandleVideoComplete;
		// TJPlacement.OnVideoStart += HandleVideoStart;
		// requestNewInterstitial();
	}

	public void showAdmobAds(Action<bool> closeAction)
	{
		m_admobCloseAction = null;
	}

	private void EventAdClose(object sender, EventArgs e)
	{
		if (m_admobCloseAction != null)
		{
			m_admobCloseAction(true);
			m_admobCloseAction = null;
		}
		requestNewInterstitial();
	}

	private void requestNewInterstitial()
	{
		if (m_interstitialForAdmob != null)
		{
			m_interstitialForAdmob.Destroy();
			m_interstitialForAdmob = null;
		}
		m_interstitialForAdmob = new InterstitialAd("ca-app-pub-5195866754572119/6519341986");
		m_interstitialForAdmob.OnAdClosed += EventAdClose;
		AdRequest request = new AdRequest.Builder().Build();
		m_interstitialForAdmob.LoadAd(request);
	}

	private void Start()
	{
		AppLovin.SetSdkKey("HMenQXixA_QPcuR3dKb__mzZ5vkqH3_ZPeKrA1nK1Bkd3AhhVIhU2e28TnyqqyOEai4mlr5m1qO5-5oTNTzQ60");
		AppLovin.InitializeSdk();
		AppLovin.SetUnityAdListener("AdsManager");
		AppLovin.LoadRewardedInterstitial();
		AndroidNativeUtility.LocaleInfoLoaded += LocaleInfoLoaded;
		SA_Singleton<AndroidNativeUtility>.Instance.LoadLocaleInfo();
	}

	private void OnDestroy()
	{
		TJPlacement.OnRequestSuccess -= HandlePlacementRequestSuccess;
		TJPlacement.OnContentReady -= HandlePlacementContentReady;
		TJPlacement.OnContentDismiss -= HandlePlacementContentDismiss;
		TJPlacement.OnVideoComplete -= HandleVideoComplete;
		TJPlacement.OnVideoStart -= HandleVideoStart;
		Tapjoy.OnSpendCurrencyResponse -= HandleSpendCurrencyResponse;
		Tapjoy.OnSpendCurrencyResponseFailure -= HandleSpendCurrencyResponseFailure;
		Tapjoy.OnGetCurrencyBalanceResponse -= HandleGetCurrencyBalanceResponse;
		Tapjoy.OnEarnedCurrency -= HandleEarnedCurrency;
		Tapjoy.OnConnectSuccess -= HandleConnectSuccess;
		Tapjoy.OnConnectFailure -= HandleConnectFailure;
	}

	private void LocaleInfoLoaded(AN_Locale locale)
	{
		switch (locale.CountryCode.ToLower().Trim())
		{
		case "us":
		case "kr":
		case "jp":
		case "cn":
			m_isAdmobEnableCountry = false;
			break;
		default:
			m_isAdmobEnableCountry = true;
			break;
		}
		AndroidNativeUtility.LocaleInfoLoaded -= LocaleInfoLoaded;
	}

	public void HandleConnectSuccess()
	{
		DebugManager.Log("C# HandleConnectSuccess");
		tapjoyIsConnected = true;
		if (videoAdPlayPlacement == null)
		{
			videoAdPlayPlacement = TJPlacement.CreatePlacement("VideoAd");
			DebugManager.Log("videoAdPlayPlacement:" + videoAdPlayPlacement != null);
			if (videoAdPlayPlacement != null)
			{
				DebugManager.Log("RequestContent");
				videoAdPlayPlacement.RequestContent();
			}
		}
		if (offerwallPlacement == null)
		{
			offerwallPlacement = TJPlacement.CreatePlacement("OfferWall");
		}
		DebugManager.Log("offerwallPlacement:" + offerwallPlacement != null);
	}

	public void HandleConnectFailure()
	{
		DebugManager.LogError("Tapjoy connect failed!");
	}

	public void HandlePlacementRequestSuccess(TJPlacement placement)
	{
		if (placement.IsContentAvailable())
		{
			if (placement.GetName() == "VideoAd")
			{
				DebugManager.Log("C#: Content available for " + placement.GetName());
				contentIsReadyForPlacement = true;
			}
			else if (!(placement.GetName() == "OfferWall"))
			{
			}
		}
		else
		{
			DebugManager.Log("C#: No content available for " + placement.GetName());
			placement.RequestContent();
		}
	}

	public void HandlePlacementContentReady(TJPlacement placement)
	{
		DebugManager.Log("C#: HandlePlacementContentReady");
		if (placement.IsContentAvailable())
		{
			if (placement.GetName() == "OfferWall")
			{
				placement.ShowContent();
				StopCoroutine("waitForOpenTapjoy");
				UIWindowLoading.instance.closeLoadingUI();
			}
		}
		else
		{
			DebugManager.Log("C#: no content");
			placement.RequestContent();
		}
	}

	public void HandlePlacementContentDismiss(TJPlacement placement)
	{
		if (placement.GetName() == "VideoAd")
		{
			if (tapjoyVideoComplete)
			{
				Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.AdsAngel, 1.0);
				if (callbackFinishAd != null)
				{
					callbackFinishAd();
				}
			}
			isShowingAd = false;
			Screen.orientation = ScreenOrientation.Portrait;
			contentIsReadyForPlacement = false;
			videoAdPlayPlacement.RequestContent();
		}
		else
		{
			Tapjoy.GetCurrencyBalance();
		}
		if (!UIWindowResult.instance.isOpen && NSPlayerPrefs.GetFloat("BGM", 1f) > 0f)
		{
			Singleton<AudioManager>.instance.bgmAudioSource.Play();
		}
		tapjoyVideoComplete = false;
	}

	public void HandleEarnedCurrency(string currencyName, int amount)
	{
		DebugManager.Log("C#: HandleEarnedCurrency: currencyName: " + currencyName + ", amount: " + amount);
		Tapjoy.ShowDefaultEarnedCurrencyAlert();
	}

	public void HandleVideoStart(TJPlacement placement)
	{
		if (!UIWindowResult.instance.isOpen && NSPlayerPrefs.GetFloat("BGM", 1f) > 0f)
		{
			Singleton<AudioManager>.instance.stopBackgroundSound();
		}
	}

	public void HandleVideoComplete(TJPlacement placement)
	{
		DebugManager.Log("C#: HandleVideoComplete for placement " + placement.GetName());
		tapjoyVideoComplete = true;
	}

	public void HandleGetCurrencyBalanceResponse(string currencyName, int balance)
	{
		if (balance > 0)
		{
			UIWindowLoading.instance.openLoadingUI();
			Tapjoy.SpendCurrency(balance);
			tapjoyCurrency = balance;
		}
		else
		{
			tapjoyCurrency = 0L;
		}
		DebugManager.Log("HandleGetCurrencyBalanceResponse:" + balance);
	}

	public void HandleSpendCurrencyResponse(string currencyName, int balance)
	{
		DebugManager.Log("C#: HandleSpendCurrencyResponse: currencyName: " + currencyName + ", balance: " + balance);
		UIWindowLoading.instance.closeLoadingUI();
		if (tapjoyCurrency > 0)
		{
			Singleton<FlyResourcesManager>.instance.playEffectResources(Singleton<FlyResourcesManager>.instance.centerTransform, FlyResourcesManager.ResourceType.Ruby, Math.Min(tapjoyCurrency, 30L), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			Singleton<RubyManager>.instance.increaseRuby(tapjoyCurrency);
			AnalyzeManager.retention(AnalyzeManager.CategoryType.WatchAd, AnalyzeManager.ActionType.TapjoyFreeDiamond, new Dictionary<string, string>
			{
				{
					"Diamond",
					tapjoyCurrency.ToString()
				}
			});
			tapjoyCurrency = 0L;
		}
		if (balance > 0)
		{
			Tapjoy.GetCurrencyBalance();
		}
	}

	public void HandleSpendCurrencyResponseFailure(string error)
	{
		UIWindowLoading.instance.closeLoadingUI();
		UIWindowDialog.openDescription("TAPJOY_OFFERWALL_FAIL", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		DebugManager.Log("C#: HandleSpendCurrencyResponseFailure: " + error);
	}

	public void ShowOfferwall()
	{
		if (Util.isInternetConnection())
		{
			if (offerwallPlacement != null)
			{
				offerwallPlacement.RequestContent();
				UIWindowLoading.instance.openLoadingUI();
				StopCoroutine("waitForOpenTapjoy");
				StartCoroutine("waitForOpenTapjoy");
			}
			else
			{
				UIWindowDialog.openDescription("COUPON_UNKNOWN_ERROR", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		}
		else
		{
			UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	private IEnumerator waitForOpenTapjoy()
	{
		yield return new WaitForSeconds(10f);
		UIWindowLoading.instance.closeLoadingUI();
	}

	public bool isReady(string adsName)
	{
		//TODO 注释
		// if (!Util.isInternetConnection())
		// {
		// 	return false;
		// }
		// if (!setUserID && NanooAPIManager.uuid != string.Empty)
		// {
		// 	AppLovin.SetRewardedVideoUsername(NanooAPIManager.uuid);
		// 	if (Tapjoy.IsConnected)
		// 	{
		// 		Tapjoy.SetUserID(NanooAPIManager.uuid);
		// 	}
		// 	setUserID = true;
		// }
		// if (videoAdPlayPlacement != null && !contentIsReadyForPlacement)
		// {
		// 	videoAdPlayPlacement.RequestContent();
		// }
		// if (!AppLovin.IsIncentInterstitialReady())
		// {
		// 	AppLovin.LoadRewardedInterstitial();
		// }
		// bool flag = Advertisement.IsReady(adsName);
		// bool flag2 = AppLovin.IsIncentInterstitialReady();
		// bool flag3 = videoAdPlayPlacement != null && videoAdPlayPlacement.IsContentAvailable();
		// bool flag4 = flag || flag2 || flag3;
		// if (!flag4)
		// {
		// 	UIWindowDialog.openMiniDialog("NOTAVAILABLE_ADS");
		// }
		// return flag4;
		return false;
	}

	public void showAds(string adsName, Action finishAction)
	{
		// if (!Util.isInternetConnection())
		// {
		// 	UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		// 	return;
		// }
		// Crashlytics.SetKeyValue("AdsManager.showAds()", "before finishAction");
		// isShowingAd = true;
		// finishAction = (Action)Delegate.Combine(finishAction, (Action)delegate
		// {
		// 	if (Social.localUser.authenticated)
		// 	{
		// 		Social.ReportScore(++Singleton<DataManager>.instance.currentGameData.adWatchCount, "CgkIgP-i6oAVEAIQOA", delegate
		// 		{
		// 		});
		// 		Singleton<DataManager>.instance.saveData();
		// 	}
		// 	if (UIWindowResult.instance.isOpen && !UIWindowResult.instance.isLastBossClear)
		// 	{
		// 		UIWindowResult.instance.startNextTimer();
		// 	}
		// 	isShowingAd = false;
		// 	Screen.orientation = ScreenOrientation.Portrait;
		// });
		// Crashlytics.SetKeyValue("AdsManager.showAds()", "init");
		// int num = UnityEngine.Random.Range(0, valueUnityAds + valueApplovin + valueTapjoy);
		// targetAds = ((num >= valueUnityAds) ? ((num < valueUnityAds + valueApplovin) ? TargetAds.ApplovinAds : TargetAds.TapjoyAds) : TargetAds.UnityAds);
		// Crashlytics.SetKeyValue("Advertisement.IsReady()", Advertisement.IsReady(adsName).ToString());
		// Crashlytics.SetKeyValue("AppLovin.IsIncentInterstitialReady()", AppLovin.IsIncentInterstitialReady().ToString());
		// Crashlytics.SetKeyValue("videoAdPlayPlacement", (videoAdPlayPlacement != null) ? "videoAdPlayPlacement" : "null");
		// bool flag = false;
		// if (targetAds == TargetAds.UnityAds)
		// {
		// 	if (Advertisement.IsReady(adsName))
		// 	{
		// 		flag = true;
		// 	}
		// 	else if (AppLovin.IsIncentInterstitialReady())
		// 	{
		// 		targetAds = TargetAds.ApplovinAds;
		// 		flag = true;
		// 	}
		// 	else if (videoAdPlayPlacement != null && videoAdPlayPlacement.IsContentAvailable() && videoAdPlayPlacement.IsContentReady())
		// 	{
		// 		targetAds = TargetAds.TapjoyAds;
		// 		flag = true;
		// 	}
		// }
		// else if (targetAds == TargetAds.ApplovinAds)
		// {
		// 	if (AppLovin.IsIncentInterstitialReady())
		// 	{
		// 		flag = true;
		// 	}
		// 	else if (Advertisement.IsReady(adsName))
		// 	{
		// 		targetAds = TargetAds.UnityAds;
		// 		flag = true;
		// 	}
		// 	else if (videoAdPlayPlacement != null && videoAdPlayPlacement.IsContentAvailable() && videoAdPlayPlacement.IsContentReady())
		// 	{
		// 		targetAds = TargetAds.TapjoyAds;
		// 		flag = true;
		// 	}
		// }
		// else if (videoAdPlayPlacement != null && videoAdPlayPlacement.IsContentAvailable() && videoAdPlayPlacement.IsContentReady())
		// {
		// 	flag = true;
		// }
		// else if (Advertisement.IsReady(adsName))
		// {
		// 	targetAds = TargetAds.UnityAds;
		// 	flag = true;
		// }
		// else if (AppLovin.IsIncentInterstitialReady())
		// {
		// 	targetAds = TargetAds.ApplovinAds;
		// 	flag = true;
		// }
		// Crashlytics.SetKeyValue("AdsManager.showAds()", "init done");
		// if (flag)
		// {
		// 	switch (targetAds)
		// 	{
		// 	case TargetAds.UnityAds:
		// 		Advertisement.Show(adsName, new ShowOptions
		// 		{
		// 			resultCallback = delegate(ShowResult result)
		// 			{
		// 				switch (result)
		// 				{
		// 				case ShowResult.Finished:
		// 					Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.AdsAngel, 1.0);
		// 					if (finishAction != null)
		// 					{
		// 						finishAction();
		// 					}
		// 					else
		// 					{
		// 						Crashlytics.SetKeyValue("finishAction after watch ads", "null");
		// 					}
		// 					finishAction = null;
		// 					break;
		// 				case ShowResult.Failed:
		// 					isShowingAd = false;
		// 					break;
		// 				default:
		// 					isShowingAd = false;
		// 					break;
		// 				}
		// 			}
		// 		});
		// 		break;
		// 	case TargetAds.ApplovinAds:
		// 		callbackFinishAd = finishAction;
		// 		AppLovin.ShowRewardedInterstitial();
		// 		break;
		// 	case TargetAds.TapjoyAds:
		// 		callbackFinishAd = finishAction;
		// 		if (videoAdPlayPlacement != null && videoAdPlayPlacement.IsContentReady())
		// 		{
		// 			videoAdPlayPlacement.ShowContent();
		// 		}
		// 		else
		// 		{
		// 			isShowingAd = false;
		// 		}
		// 		break;
		// 	}
		// }
		// else
		// {
		// 	isShowingAd = false;
		// 	UIWindowDialog.openDescription("NOTAVAILABLE_ADS", UIWindowDialog.DialogState.DelegateOKUI, null, string.Empty);
		// }
	}

	private void onAppLovinEventReceived(string ev)
	{
		DebugManager.Log("Applovin ev:" + ev);
		if (ev.Contains("REWARDAPPROVEDINFO"))
		{
			return;
		}
		if (ev.Contains("VIDEOBEGAN"))
		{
			if (!UIWindowResult.instance.isOpen && NSPlayerPrefs.GetFloat("BGM", 1f) > 0f)
			{
				Singleton<AudioManager>.instance.stopBackgroundSound();
			}
		}
		else if (ev.Contains("VIDEOSTOPPED"))
		{
			if (!UIWindowResult.instance.isOpen && NSPlayerPrefs.GetFloat("BGM", 1f) > 0f)
			{
				Singleton<AudioManager>.instance.bgmAudioSource.Play();
			}
		}
		else if (ev.Contains("HIDDENREWARDED"))
		{
			AppLovin.LoadRewardedInterstitial();
			if (callbackFinishAd != null)
			{
				Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.AdsAngel, 1.0);
				callbackFinishAd();
				callbackFinishAd = null;
			}
			isShowingAd = false;
			Screen.orientation = ScreenOrientation.Portrait;
		}
	}
}
