using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowSetting : UIWindow
{
	[Serializable]
	public struct Result
	{
		public string rewardType;

		public long rewardCount;

		public string code;
	}

	[Serializable]
	public class CouponResult
	{
		public List<Result> result;

		public string error = string.Empty;
	}

	public static UIWindowSetting instance;

	public bool isOpenNextPage;

	public GameObject socialButtonObject;

	public GameObject couponButtonObject;

	public GameObject selectLanguageObjects;

	public GameObject enterCouponObjects;

	public GameObject voulmeObjects;

	public Slider voulmeSlider;

	public Text voulmeText;

	public CanvasGroup settingObjectCanvasGroup;

	public RectTransform settingObjectTransform;

	public CanvasGroup selectLanguageObjectCanvasGroup;

	public RectTransform selectLanguageTransform;

	public GameObject idRefreshButtonObject;

	public GameObject bgmDisabledObject;

	public Image bgmButtonBackgroundImage;

	public Image bgmIconImage;

	public GameObject sfxDisabledObject;

	public Image sfxButtonBackgroundImage;

	public Image sfxIconImage;

	public Text voulmeTitleText;

	public Text versionText;

	public Text userIDText;

	public Text currentLanguageText;

	public Text socialServiceText;

	public GameObject pushDisabledObject;

	public Image pushButtonBackgroundImage;

	public Image pushIconImage;

	public Image googleLoginImage;

	public Image googleLoginIconImage;

	public InputFieldCustom couponInputTextField;

	public RectTransform okButtonTransform;

	public RectTransform backgroundTransform;

	public GameObject facebookButtonObject;

	public GameObject twitterButtonObject;

	public GameObject facebookRewardButtonObject;

	public GameObject twitterRewardButtonObject;

	public Sprite achievementAndroidSprite;

	public Sprite achievementIOSSprite;

	public Image achievementIconImage;

	public Sprite leaderboardAndroidSprite;

	public Sprite leaderboardIOSSprite;

	public Image leaderboardIconImage;

	public GameObject lowDeviceDisabledObject;

	public Image lowDeviceButtonBackgroundImage;

	public Image lowDeviceIconImage;

	public GameObject hdModeDisabledObject;

	public Image hdModeButtonBackgroundImage;

	public Image hdModeIconImage;

	public GameObject screenProtectDisabledObject;

	public Image screenProtectButtonBackgroundImage;

	public Image screenProtectIconImage;

	private bool m_isBGMVoulme;

	public static DateTime lastTimeRestore = new DateTime(2010, 1, 1);

	public override void Awake()
	{
		achievementIconImage.sprite = achievementAndroidSprite;
		achievementIconImage.SetNativeSize();
		leaderboardIconImage.sprite = leaderboardAndroidSprite;
		leaderboardIconImage.SetNativeSize();
		instance = this;
		TouchScreenKeyboard.hideInput = true;
		base.Awake();
	}

	private void OnEnable()
	{
		refreshSetting();
	}

	public void refreshSetting(bool isResetUITransform = true)
	{
		switch (I18NManager.currentLanguage)
		{
		case I18NManager.Language.Korean:
			currentLanguageText.text = I18NManager.Get("SETTING_LANGUAGE_KOREAN");
			break;
		case I18NManager.Language.English:
			currentLanguageText.text = I18NManager.Get("SETTING_LANGUAGE_ENGLISH");
			break;
		case I18NManager.Language.ChineseSimplified:
			currentLanguageText.text = I18NManager.Get("SETTING_LANGUAGE_CHINESE_SIMPLIFIED");
			break;
		case I18NManager.Language.ChineseTraditional:
			currentLanguageText.text = I18NManager.Get("SETTING_LANGUAGE_CHINESE_TRADITIONAL");
			break;
		case I18NManager.Language.Japanese:
			currentLanguageText.text = I18NManager.Get("SETTING_LANGUAGE_JAPANESE");
			break;
		default:
			currentLanguageText.text = I18NManager.Get("SETTING_LANGUAGE_ENGLISH");
			break;
		}
		refreshSNSButtonState();
		if (NSPlayerPrefs.GetFloat("BGM", 1f) > 0f)
		{
			bgmDisabledObject.SetActive(false);
			bgmButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingOrangeButton;
			bgmIconImage.color = Color.white;
		}
		else
		{
			bgmDisabledObject.SetActive(true);
			bgmButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingGrayButton;
			bgmIconImage.color = Util.getCalculatedColor(182f, 182f, 182f);
		}
		if (NSPlayerPrefs.GetFloat("SFX", 1f) > 0f)
		{
			sfxDisabledObject.SetActive(false);
			sfxButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingOrangeButton;
			sfxIconImage.color = Color.white;
		}
		else
		{
			sfxDisabledObject.SetActive(true);
			sfxButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingGrayButton;
			sfxIconImage.color = Util.getCalculatedColor(182f, 182f, 182f);
		}
		if (Singleton<DataManager>.instance.currentGameData.isPushNotificationOn)
		{
			pushDisabledObject.SetActive(false);
			pushButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingOrangeButton;
			pushIconImage.color = Color.white;
		}
		else
		{
			pushDisabledObject.SetActive(true);
			pushButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingGrayButton;
			pushIconImage.color = Util.getCalculatedColor(182f, 182f, 182f);
		}
		if (Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			lowDeviceDisabledObject.SetActive(false);
			lowDeviceButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingOrangeButton;
			lowDeviceIconImage.color = Color.white;
		}
		else
		{
			lowDeviceDisabledObject.SetActive(true);
			lowDeviceButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingGrayButton;
			lowDeviceIconImage.color = Util.getCalculatedColor(182f, 182f, 182f);
		}
		int @int = NSPlayerPrefs.GetInt("ScreenProtect", 0);
		if (@int == 1)
		{
			screenProtectDisabledObject.SetActive(false);
			screenProtectButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingOrangeButton;
			screenProtectIconImage.color = Color.white;
		}
		else
		{
			screenProtectDisabledObject.SetActive(true);
			screenProtectButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingGrayButton;
			screenProtectIconImage.color = Util.getCalculatedColor(182f, 182f, 182f);
		}
		int int2 = NSPlayerPrefs.GetInt("HDMode", 0);
		int int3 = NSPlayerPrefs.GetInt("OriginWidth");
		int int4 = NSPlayerPrefs.GetInt("OriginHeight");
		if (int2 == 1)
		{
			hdModeDisabledObject.SetActive(false);
			hdModeButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingOrangeButton;
			hdModeIconImage.color = Color.white;
		}
		else
		{
			hdModeDisabledObject.SetActive(true);
			hdModeButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.settingGrayButton;
			hdModeIconImage.color = Util.getCalculatedColor(182f, 182f, 182f);
		}
		versionText.text = "ver." + GlobalSetting.s_bundleVersion + "  (" + GlobalSetting.s_bundleVersionCode + ")";
		if (Singleton<NanooAPIManager>.instance.UserID.Length > 0)
		{
			idRefreshButtonObject.SetActive(false);
			userIDText.text = "ID : " + Singleton<NanooAPIManager>.instance.UserID;
		}
		else
		{
			idRefreshButtonObject.SetActive(true);
			userIDText.text = "ID : N/A";
		}
		Singleton<AudioManager>.instance.refreshAudioVolume();
		if (Social.localUser.authenticated)
		{
			googleLoginImage.sprite = Singleton<CachedManager>.instance.settingOrangeButton;
			googleLoginIconImage.color = Color.white;
		}
		else
		{
			googleLoginImage.sprite = Singleton<CachedManager>.instance.settingGrayButton;
			googleLoginIconImage.color = Util.getCalculatedColor(182f, 182f, 182f);
		}
		if (isResetUITransform)
		{
			isOpenNextPage = false;
			settingObjectTransform.anchoredPosition = Vector2.zero;
			settingObjectCanvasGroup.alpha = 1f;
			selectLanguageTransform.anchoredPosition = new Vector2(492f, 0f);
			selectLanguageObjectCanvasGroup.alpha = 0f;
			StopAllCoroutines();
			StartCoroutine("uiMoveUpdate");
		}
	}

	public void openSelectLanguageUI()
	{
		isOpenNextPage = true;
		enterCouponObjects.SetActive(false);
		selectLanguageObjects.SetActive(true);
		voulmeObjects.SetActive(false);
	}

	public void closeNextPageUI()
	{
		isOpenNextPage = false;
		refreshSetting(false);
	}

	private IEnumerator uiMoveUpdate()
	{
		while (true)
		{
			if (!isOpenNextPage)
			{
				settingObjectTransform.anchoredPosition = Vector2.Lerp(settingObjectTransform.anchoredPosition, Vector2.zero, Time.deltaTime * 20f);
				settingObjectCanvasGroup.alpha = Mathf.Lerp(settingObjectCanvasGroup.alpha, 1f, Time.deltaTime * 5f);
				selectLanguageTransform.anchoredPosition = Vector2.Lerp(selectLanguageTransform.anchoredPosition, new Vector2(492f, 0f), Time.deltaTime * 20f);
				selectLanguageObjectCanvasGroup.alpha = Mathf.Lerp(selectLanguageObjectCanvasGroup.alpha, 0f, Time.deltaTime * 5f);
			}
			else
			{
				selectLanguageTransform.anchoredPosition = Vector2.Lerp(selectLanguageTransform.anchoredPosition, Vector2.zero, Time.deltaTime * 20f);
				selectLanguageObjectCanvasGroup.alpha = Mathf.Lerp(selectLanguageObjectCanvasGroup.alpha, 1f, Time.deltaTime * 5f);
				settingObjectTransform.anchoredPosition = Vector2.Lerp(settingObjectTransform.anchoredPosition, new Vector2(-492f, 0f), Time.deltaTime * 20f);
				settingObjectCanvasGroup.alpha = Mathf.Lerp(settingObjectCanvasGroup.alpha, 0f, Time.deltaTime * 5f);
			}
			yield return null;
		}
	}

	public void onClickBgmOnOrOff()
	{
		voulmeTitleText.text = I18NManager.Get("BGM");
		m_isBGMVoulme = true;
		isOpenNextPage = true;
		enterCouponObjects.SetActive(false);
		selectLanguageObjects.SetActive(false);
		voulmeObjects.SetActive(true);
		voulmeSlider.value = NSPlayerPrefs.GetFloat("BGM", 1f);
		voulmeText.text = (voulmeSlider.value * 100f).ToString("N0");
	}

	public void onClickSfxOnOrOff()
	{
		voulmeTitleText.text = I18NManager.Get("SFX");
		m_isBGMVoulme = false;
		isOpenNextPage = true;
		enterCouponObjects.SetActive(false);
		selectLanguageObjects.SetActive(false);
		voulmeObjects.SetActive(true);
		voulmeSlider.value = NSPlayerPrefs.GetFloat("SFX", 1f);
		voulmeText.text = (voulmeSlider.value * 100f).ToString("N0");
	}

	public void OnChangeVoulme()
	{
		if (m_isBGMVoulme)
		{
			NSPlayerPrefs.SetFloat("BGM", voulmeSlider.value);
		}
		else
		{
			NSPlayerPrefs.SetFloat("SFX", voulmeSlider.value);
		}
		voulmeText.text = (voulmeSlider.value * 100f).ToString("N0");
		Singleton<AudioManager>.instance.refreshAudioVolume();
	}

	public void onClickChangePushNotification()
	{
		if (Singleton<DataManager>.instance.currentGameData.isPushNotificationOn)
		{
			Singleton<DataManager>.instance.currentGameData.isPushNotificationOn = false;
			NotificationManager.CancelAllLocalNotification();
		}
		else
		{
			Singleton<DataManager>.instance.currentGameData.isPushNotificationOn = true;
			NotificationManager.CancelAllLocalNotification();
			Singleton<StatManager>.instance.refreshAllStats();
		}
		Singleton<DataManager>.instance.saveData();
		refreshSetting();
	}

	public void OnClickRefreshUserID()
	{
		if (string.IsNullOrEmpty(Singleton<NanooAPIManager>.instance.UserID))
		{
			Singleton<NanooAPIManager>.instance.refreshUserID();
		}
	}

	public void OnClickSocailLogin()
	{
		if (Social.localUser.authenticated)
		{
			PlayGamesPlatform.Instance.SignOut();
			Singleton<NanooAPIManager>.instance.ClearPostAPI();
			refreshSetting();
			return;
		}
		PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		PlayGamesPlatform.InitializeInstance(configuration);
		PlayGamesPlatform.DebugLogEnabled = false;
		PlayGamesPlatform.Activate();
		Social.localUser.Authenticate(delegate(bool success)
		{
			if (success)
			{
				Singleton<NanooAPIManager>.instance.InitPostBox();
				refreshSetting();
			}
		});
	}

	public void OnClickSaveToCloud()
	{
		if (Singleton<NanooAPIManager>.instance.UserID.Length > 0)
		{
			UIWindowSaveCloud.instance.open();
		}
		else
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("REQUIRE_LOGIN"), I18NManager.Get("GOOGLE_PLAY")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void OnClickLoadFromCloud()
	{
		if (Singleton<NanooAPIManager>.instance.UserID.Length > 0)
		{
			Singleton<DataManager>.instance.loadFromCloud();
		}
		else
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("REQUIRE_LOGIN"), I18NManager.Get("GOOGLE_PLAY")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void OnClickEnterCoupon()
	{
		if (Util.isInternetConnection())
		{
			if (Singleton<NanooAPIManager>.instance.UserID.Length > 0)
			{
				isOpenNextPage = true;
				enterCouponObjects.SetActive(true);
				voulmeObjects.SetActive(false);
				selectLanguageObjects.SetActive(false);
			}
			else
			{
				UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("REQUIRE_LOGIN"), I18NManager.Get("GOOGLE_PLAY")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		}
		else
		{
			UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	private void ActionConnectionResultReceived(GooglePlayConnectionResult result)
	{
		if (result.IsSuccess)
		{
			DebugManager.Log("Connected!");
			SA_Singleton<GooglePlayManager>.Instance.LoadAchievements();
			googleLoginImage.sprite = Singleton<CachedManager>.instance.settingRedButton;
			googleLoginIconImage.color = Color.white;
		}
		else
		{
			DebugManager.Log("Cnnection failed with code: " + result.code);
		}
		GooglePlayConnection.ActionConnectionResultReceived -= ActionConnectionResultReceived;
	}

	public void OnClickCopyID()
	{
		if (Singleton<NanooAPIManager>.instance.UserID.Length > 0)
		{
			//NativePluginManager.Instance.SetClipboardString("UserID", Singleton<NanooAPIManager>.instance.UserID);
			AndroidToast.ShowToastNotification(I18NManager.Get("USERID_COPIED"));
		}
		Singleton<LogglyManager>.instance.SendLoggly("Device Time:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", UnbiasedTime:" + UnbiasedTime.Instance.Now().ToString("yyyy-MM-dd HH:mm:ss") + ", lastSavedOffsetBetweenInternetAndUnbiased:" + Singleton<DataManager>.instance.currentGameData.lastSavedOffsetBetweenInternetAndUnbiased, "UnbiasedTime Info", LogType.Log);
	}

	public void OnClickOpenNotice()
	{
		if (Util.isInternetConnection())
		{
			if (UIWindowNotice.instance.prevLanguage != I18NManager.currentLanguage)
			{
				Singleton<NanooAPIManager>.instance.CallAPI(NanooAPIManager.APIType.EVENTBANNER);
				Singleton<NanooAPIManager>.instance.ForumThread();
				UIWindowLoading.instance.openLoadingUI();
				StopCoroutine("waitForLoadingNotice");
				StartCoroutine("waitForLoadingNotice");
			}
			else if ((bool)UIWindowNotice.instance)
			{
				UIWindowNotice.instance.openNoticeUI(Singleton<NanooAPIManager>.instance._noticeList, true);
				close();
			}
		}
		else
		{
			UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	private IEnumerator waitForLoadingNotice()
	{
		float timeOutChecking = 0f;
		while (!Singleton<NanooAPIManager>.instance.isNoticeComplete || !Singleton<NanooAPIManager>.instance.isEventBannerComplete)
		{
			timeOutChecking += Time.deltaTime;
			if (timeOutChecking >= 10f)
			{
				UIWindowLoading.instance.closeLoadingUI();
				UIWindowDialog.openDescription("NETWORK_TIME_OUT", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
				yield break;
			}
			yield return null;
		}
		UIWindowLoading.instance.closeLoadingUI();
		close();
		if ((bool)UIWindowNotice.instance)
		{
			UIWindowNotice.instance.openNoticeUI(Singleton<NanooAPIManager>.instance._noticeList, true);
		}
	}

	public void refreshSNSButtonState()
	{
		facebookRewardButtonObject.SetActive(!Singleton<DataManager>.instance.currentGameData.isObtainedFacebookReward);
		facebookButtonObject.SetActive(Singleton<DataManager>.instance.currentGameData.isObtainedFacebookReward);
		twitterRewardButtonObject.SetActive(!Singleton<DataManager>.instance.currentGameData.isObtainedTwitterReward);
		twitterButtonObject.SetActive(Singleton<DataManager>.instance.currentGameData.isObtainedTwitterReward);
	}

	public void OnClickFacebookLike()
	{
		if (Util.isInternetConnection())
		{
			if (!Singleton<DataManager>.instance.currentGameData.isObtainedFacebookReward)
			{
				Singleton<DataManager>.instance.currentGameData.isObtainedFacebookReward = true;
				Application.OpenURL("https://www.facebook.com/PlayNANOO/");
				StartCoroutine(waitForFakeLoadingForSNSLike(delegate
				{
					UIWindowDialog.openDescription("SUCCESS_OBTAIN_SNS_EVENT", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
					Singleton<RubyManager>.instance.increaseRuby(3.0);
					Singleton<FlyResourcesManager>.instance.playEffectResources(facebookRewardButtonObject.transform.position, FlyResourcesManager.ResourceType.Ruby, 3L, 0.02f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
					Singleton<DataManager>.instance.saveData();
					refreshSNSButtonState();
					UIWindowOutgame.instance.refreshSettingIndicator();
				}));
			}
			else
			{
				Application.OpenURL("https://www.facebook.com/PlayNANOO/");
			}
		}
		else
		{
			UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void OnClickTwitterFollow()
	{
		if (Util.isInternetConnection())
		{
			if (!Singleton<DataManager>.instance.currentGameData.isObtainedTwitterReward)
			{
				Singleton<DataManager>.instance.currentGameData.isObtainedTwitterReward = true;
				Application.OpenURL("https://twitter.com/playnanoo");
				StartCoroutine(waitForFakeLoadingForSNSLike(delegate
				{
					UIWindowDialog.openDescription("SUCCESS_OBTAIN_SNS_EVENT", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
					Singleton<RubyManager>.instance.increaseRuby(3.0);
					Singleton<FlyResourcesManager>.instance.playEffectResources(twitterRewardButtonObject.transform.position, FlyResourcesManager.ResourceType.Ruby, 3L, 0.02f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
					Singleton<DataManager>.instance.saveData();
					refreshSNSButtonState();
					UIWindowOutgame.instance.refreshSettingIndicator();
				}));
			}
			else
			{
				Application.OpenURL("https://twitter.com/playnanoo");
			}
		}
		else
		{
			UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void OnClickLowEndDevice()
	{
		Singleton<DataManager>.instance.currentGameData.isLowEndDevice = !Singleton<DataManager>.instance.currentGameData.isLowEndDevice;
		Singleton<DataManager>.instance.saveData();
		refreshSetting(false);
	}

	public void OnClickHDMode()
	{
		int hdMode = NSPlayerPrefs.GetInt("HDMode", 0);
		string i18NTitleID = ((hdMode != 0) ? "HD_MODE_LOW_DESCRIPTION" : "HD_MODE_HIGH_DESCRIPTION");
		UIWindowDialog.openDescription(i18NTitleID, UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			NSPlayerPrefs.SetInt("HDMode", (hdMode != 1) ? 1 : 0);
			UIWindowDialog.openDescription("HD_MODE_SUCCESS", UIWindowDialog.DialogState.DelegateOKUI, delegate
			{
				Application.Quit();
			}, string.Empty);
		}, string.Empty);
	}

	public void OnClickScreenProtectMode()
	{
		int @int = NSPlayerPrefs.GetInt("ScreenProtect", 0);
		NSPlayerPrefs.SetInt("ScreenProtect", (@int != 1) ? 1 : 0);
		refreshSetting(false);
	}

	private IEnumerator waitForFakeLoadingForSNSLike(Action endAction)
	{
		UIWindowLoading.instance.openLoadingUI();
		yield return new WaitForSeconds(1f);
		UIWindowLoading.instance.closeLoadingUI();
		endAction();
	}

	public void onClickRestore()
	{
		lastTimeRestore = UnbiasedTime.Instance.Now();
		Singleton<PaymentManager>.instance.Restore();
	}

	public void OnClickQA()
	{
		Singleton<NanooAPIManager>.instance.OpenHelp();
	}

	private string MyEscapeURL(string url)
	{
		return WWW.EscapeURL(url).Replace("+", "%20");
	}

	public void onClickCredit()
	{
		UIWindowCredit.instance.openWithCredit();
	}

	public void OnClickReplayIntro()
	{
		GameObject original = Resources.Load<GameObject>("Prefabs/UI/@Canvas (UI Opening)");
		UIWindowOpening component = UnityEngine.Object.Instantiate(original).GetComponent<UIWindowOpening>();
		component.nextScene(true);
	}

	public override bool OnBeforeOpen()
	{
		socialServiceText.text = I18NManager.Get("GOOGLE_PLAY");
		socialButtonObject.SetActive(false);
		couponButtonObject.SetActive(Singleton<NanooAPIManager>.instance.isServiceOn);
		couponInputTextField.text = string.Empty;
		return base.OnBeforeOpen();
	}

	public void OnClickOK()
	{
		string text = couponInputTextField.text.Trim();
		if (text.Length <= 4)
		{
			UIWindowDialog.openDescription("COUPON_CODE_ERROR", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			return;
		}
		Singleton<NanooAPIManager>.instance.CallAPI(NanooAPIManager.APIType.COUPON_CHECK, text);
		UIWindowLoading.instance.openLoadingUI();
	}

	public void OnClickLeaderBoard()
	{
		if (!Social.localUser.authenticated)
		{
			Social.localUser.Authenticate(delegate(bool success)
			{
				if (success)
				{
					Singleton<NanooAPIManager>.instance.InitPostBox();
					Social.ShowLeaderboardUI();
				}
			});
		}
		else
		{
			Social.ShowLeaderboardUI();
		}
	}

	public void OnClickAchievement()
	{
		if (!Social.localUser.authenticated)
		{
			Social.localUser.Authenticate(delegate(bool success)
			{
				if (success)
				{
					Singleton<NanooAPIManager>.instance.InitPostBox();
					Social.ShowAchievementsUI();
				}
			});
		}
		else
		{
			Social.ShowAchievementsUI();
		}
	}

	public void OnClickMoreGame()
	{
		Application.OpenURL("https://play.google.com/store/apps/developer?id=NANOO+COMPANY+Inc.");
	}

	public void OnClickPaste()
	{
		if (couponInputTextField.text == null)
		{
			couponInputTextField.text = string.Empty;
		}
		//couponInputTextField.text = ((NativePluginManager.Instance.GetClipboardString() != null) ? NativePluginManager.Instance.GetClipboardString() : string.Empty);
	}

	public void OnClickClose()
	{
		close();
	}

	public void OnClickGoldCheatForBIC()
	{
	}

	public void OnClickRubyCheatForBIC()
	{
	}

	public void OnClickKeyCheatForBIC()
	{
	}

	public void OnClickResetDataForBIC()
	{
	}
}
