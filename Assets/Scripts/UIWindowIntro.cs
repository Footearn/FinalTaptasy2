using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowIntro : MonoBehaviour
{
	public static bool isLoaded;

	public static UIWindowIntro instance;

	public GameObject splashObject;

	public AnimationCurve handShakeAnim;

	public RectTransform handImage;

	public CanvasGroup splashFrontImage;

	public GameObject startButton;

	public Image backgroundImage;

	public GameObject loadingObject;

	public Image dataParsingProgressBar;

	public Text dataParsingProgressingText;

	public Text loadingStateText;

	public int totalTaskCount;

	public int totalCurrentTaskCount;

	public GameObject cloudLoadButtonObject;

	private Action m_endAction;

	public UIWindowTerms termsUI;

	private void Awake()
	{
		instance = this;
		dataParsingProgressBar.fillAmount = 0f;
		dataParsingProgressingText.text = "0%";
		//NativePluginManager.Instance.Init();
		splashObject.SetActive(true);
	}

	private void Start()
	{
		loadingStateText.text = I18NManager.GetLocalI18N("LOADING_DATA_PACK_STATE_TEXT_1");
		Singleton<AudioManager>.instance.playBackgroundSound("lobby");
	}

	public void startLoading()
	{
		StopCoroutine("loadingUpdate");
		StartCoroutine("loadingUpdate");
	}

	private IEnumerator loadingUpdate()
	{
		while (!ParsingManager.isLoaded)
		{
			yield return null;
		}
		yield return null;
		float progress = 0f;
		while (progress < 100f)
		{
			progress = (1f - (float)totalCurrentTaskCount / (float)totalTaskCount) * 100f;
			setLoadingText(progress);
			dataParsingProgressBar.fillAmount = progress * 0.01f;
			dataParsingProgressingText.text = progress.ToString("f0") + "%";
			yield return null;
		}
		setLoadingText(100f);
		Singleton<AudioManager>.instance.playEffectSound("title_hand");
		Singleton<ParsingManager>.instance.ingameObjects.SetActive(true);
		yield return null;
		if (DataManager.isDynamicAllocationGameData && TowerModeManager.isConnectedToServer())
		{
			cloudLoadButtonObject.SetActive(true);
		}
		else
		{
			cloudLoadButtonObject.SetActive(false);
		}
		StartCoroutine("handShake");
		Singleton<CollectEventManager>.instance.checkIsOnCollectEvent();
		float alpha = 1f;
		while (true)
		{
			alpha -= Time.deltaTime;
			if (!(alpha > 0.1f))
			{
				break;
			}
			splashFrontImage.alpha = alpha;
			yield return null;
		}
		splashFrontImage.alpha = 0f;
		splashFrontImage.gameObject.SetActive(false);
		isLoaded = true;
		startButton.SetActive(true);
	}

	public void setLoadingText(float currentProgress)
	{
		if (currentProgress < 30f)
		{
			instance.loadingStateText.text = I18NManager.GetLocalI18N("LOADING_DATA_PACK_STATE_TEXT_2");
		}
		else if (currentProgress < 60f)
		{
			instance.loadingStateText.text = I18NManager.GetLocalI18N("LOADING_DATA_PACK_STATE_TEXT_3");
		}
		else if (currentProgress < 90f)
		{
			instance.loadingStateText.text = I18NManager.GetLocalI18N("LOADING_DATA_PACK_STATE_TEXT_4");
		}
		else if (currentProgress <= 100f)
		{
			instance.loadingStateText.text = I18NManager.GetLocalI18N("LOADING_DATA_PACK_STATE_TEXT_5");
			startButton.SetActive(true);
		}
	}

	private void changeIngameScene()
	{
		Singleton<ResourcesManager>.instance.removeFromDictionary("Intro");
		UnityEngine.Object.DestroyImmediate(base.gameObject);
		Singleton<TutorialManager>.instance.startGameButton = null;
		Resources.UnloadUnusedAssets();
	}

	private IEnumerator handShake()
	{
		float timer = 1f;
		while (true)
		{
			timer += Time.deltaTime * 0.2f;
			handImage.anchoredPosition = new Vector2(0f, 146f * handShakeAnim.Evaluate(timer));
			if (timer > 1f)
			{
				timer = 0f;
			}
			yield return null;
		}
	}

	public void OnClickStart()
	{
		if (!isLoaded)
		{
			return;
		}
		if (Social.localUser.authenticated && string.IsNullOrEmpty(Singleton<NanooAPIManager>.instance.UserID) && !Singleton<NanooAPIManager>.instance.isPostInitComplete)
		{
			Singleton<NanooAPIManager>.instance.InitPostBox();
		}
		if (PlayerPrefs.GetInt("TermsAgree", 0) == 0)
		{
			termsUI.open();
			return;
		}
		Singleton<TutorialManager>.instance.startTutorial();
		changeIngameScene();
		Singleton<ShopManager>.instance.Initialize();
		if (NSPlayerPrefs.GetInt("Intro", 1) == 1)
		{
			GameObject original = Resources.Load<GameObject>("Prefabs/UI/@Canvas (UI Opening)");
			UIWindowOpening component = UnityEngine.Object.Instantiate(original).GetComponent<UIWindowOpening>();
			component.nextScene();
		}
		else
		{
			Singleton<AudioManager>.instance.playBackgroundSound("lobby02");
			Singleton<ParsingManager>.instance.introObjects.SetActive(false);
			Singleton<CachedManager>.instance.coverUI.fadeInGame();
		}
	}
}
