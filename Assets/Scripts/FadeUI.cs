using System;
using System.Collections;
using UnityEngine;

public class FadeUI : SpriteGroup
{
	public bool endStage;

	public bool refreshGameState;

	public float MaxalphaPercentage = 1f;

	public Action fadeCallback;

	private bool m_isAutoFadeIn;

	private bool m_once;

	private bool m_justFade;

	public void fadeInGame(float time = 0.5f, Action callback = null, bool justFade = false)
	{
		m_justFade = justFade;
		setAlpha(MaxalphaPercentage);
		if (!base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(true);
		}
		StopCoroutine("fadeOutRoutineGame");
		StartCoroutine("fadeOutRoutineGame", time);
		fadeCallback = callback;
	}

	private IEnumerator fadeOutRoutineGame(float timeToTake)
	{
		float alphaPercentage = MaxalphaPercentage;
		float alphaIteration = MaxalphaPercentage / timeToTake;
		while (!(alphaPercentage < 0f))
		{
			alphaPercentage -= Time.deltaTime * alphaIteration;
			setAlpha(alphaPercentage);
			yield return null;
		}
		if (fadeCallback != null)
		{
			fadeCallback();
			fadeCallback = null;
		}
		if (m_justFade)
		{
			yield break;
		}
		if (GameManager.currentGameState != 0)
		{
			if (Util.isInternetConnection())
			{
				StartCoroutine(CheckForOpenNoticeUI());
			}
			if (TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType1);
			}
			else
			{
				Singleton<DataManager>.instance.checkRewardDateTime();
				Singleton<DailyRewardManager>.instance.checkTreasureDailyRewards();
				if (Singleton<RouletteManager>.instance.isCanStartBronzeRoulette() || Singleton<DataManager>.instance.currentGameData.goldRouletteTicket > 0)
				{
					UIWindowRoulette.instance.openRouletteUI(true);
				}
			}
			Singleton<TreasureManager>.instance.refreshPMAndAMTreasureState();
			Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			Singleton<GameManager>.instance.refreshTimeScaleMiniPopup();
			Singleton<TreasureChestManager>.instance.refreshAutoOpenTreasureChest();
			Singleton<AdsAngelManager>.instance.checkSpawnAngel();
			Singleton<StatManager>.instance.refreshAllStats();
		}
		else if (GameManager.currentGameState == GameManager.GameState.Playing)
		{
			UIWindowLoading.instance.closeLoadingUI();
		}
	}

	private IEnumerator CheckForOpenNoticeUI()
	{
		if (!Singleton<NanooAPIManager>.instance.isNoticeComplete)
		{
			UIWindowLoading.instance.openLoadingUI();
		}
		float timer = 0f;
		while ((!Singleton<NanooAPIManager>.instance.isNoticeComplete || !Singleton<NanooAPIManager>.instance.isEventBannerComplete || !Singleton<NanooAPIManager>.instance.isPromotionRequestComplete) && (Singleton<NanooAPIManager>.instance.isPromotionOn || !Singleton<NanooAPIManager>.instance.isNoticeComplete || !Singleton<NanooAPIManager>.instance.isEventBannerComplete))
		{
			timer += Time.deltaTime;
			if (timer >= 15f)
			{
				break;
			}
			yield return null;
		}
		UIWindowLoading.instance.closeLoadingUI();
		if (!TutorialManager.isTutorial && !m_once)
		{
			m_once = true;
			if ((bool)UIWindowNotice.instance)
			{
				UIWindowNotice.instance.openNoticeUI(Singleton<NanooAPIManager>.instance._noticeList);
			}
			if ((bool)UIWindowCrossPromotion.instance)
			{
				UIWindowCrossPromotion.instance.openPromotionUI(Singleton<NanooAPIManager>.instance.promotionSprite);
			}
		}
	}

	public void fadeOut(float outTime = 0.5f, Action callback = null)
	{
		fadeCallback = callback;
		setAlpha(0f);
		if (!base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(true);
		}
		StopCoroutine("fadeOutRoutine");
		StopCoroutine("fadeInRoutine");
		StartCoroutine("fadeInRoutine", outTime);
	}

	public void fadeIn(float outTime = 0.5f, Action callback = null)
	{
		fadeCallback = callback;
		setAlpha(MaxalphaPercentage);
		if (!base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(true);
		}
		StopCoroutine("fadeInRoutine");
		StopCoroutine("fadeOutRoutine");
		StartCoroutine("fadeOutRoutine", outTime);
	}

	private IEnumerator fadeInRoutine(float timeToTake)
	{
		float alphaPercentage = 0f;
		float alphaIteration = MaxalphaPercentage / timeToTake;
		while (!(alphaPercentage > MaxalphaPercentage))
		{
			alphaPercentage += Time.deltaTime * alphaIteration;
			setAlpha(alphaPercentage);
			yield return null;
		}
		if (fadeCallback != null)
		{
			fadeCallback();
		}
	}

	private IEnumerator fadeOutRoutine(float timeToTake)
	{
		float alphaPercentage = MaxalphaPercentage;
		float alphaIteration = MaxalphaPercentage / timeToTake;
		while (!(alphaPercentage < 0f))
		{
			alphaPercentage -= Time.deltaTime * alphaIteration;
			setAlpha(alphaPercentage);
			yield return null;
		}
		if (fadeCallback != null)
		{
			fadeCallback();
			fadeCallback = null;
		}
	}

	public void fadeOutIn(float outTime = 0.5f, Action callback = null)
	{
		setAlpha(0f);
		if (!base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(true);
		}
		StopCoroutine("fadeOutInRoutine");
		StartCoroutine("fadeOutInRoutine", outTime);
		fadeCallback = callback;
	}

	private IEnumerator fadeOutInRoutine(float timeToTake)
	{
		float alphaPercentage = 0f;
		float alphaIteration = MaxalphaPercentage / timeToTake;
		while (!(alphaPercentage > MaxalphaPercentage))
		{
			alphaPercentage += Time.deltaTime * alphaIteration;
			setAlpha(alphaPercentage);
			yield return null;
		}
		if (fadeCallback != null)
		{
			fadeCallback();
			fadeCallback = null;
		}
		yield return new WaitForSeconds(1f);
		fadeInGame(timeToTake);
	}

	public void fadeOutGame(bool stage = true, bool refreshGame = false, Action callback = null)
	{
		refreshGameState = refreshGame;
		endStage = stage;
		setAlpha(0f);
		StopCoroutine("fadeInModified");
		StartCoroutine("fadeInModified", 0.5f);
		fadeCallback = callback;
	}

	private IEnumerator fadeInModified(float timeToTake)
	{
		float alphaPercentage = 0f;
		float alphaIteration = 1f / timeToTake;
		while (!(alphaPercentage > MaxalphaPercentage))
		{
			alphaPercentage += Time.deltaTime * alphaIteration;
			setAlpha(alphaPercentage);
			yield return null;
		}
		if (fadeCallback != null)
		{
			fadeCallback();
			fadeCallback = null;
		}
		StartCoroutine("fadeOutRoutine", 0.5f);
		if (endStage)
		{
			Singleton<DataManager>.instance.checkRewardDateTime();
			Singleton<DailyRewardManager>.instance.checkTreasureDailyRewards();
			Singleton<TreasureManager>.instance.refreshPMAndAMTreasureState();
			Singleton<GameManager>.instance.gameEnd();
			Singleton<AdsAngelManager>.instance.checkSpawnAngel();
			Singleton<StatManager>.instance.refreshAllStats();
			if (!TutorialManager.isTutorial)
			{
				if (Singleton<GameManager>.instance.reservationKidnappedEvent && Singleton<DataManager>.instance.currentGameData.bestTheme <= GameManager.maxTheme)
				{
					Singleton<GameManager>.instance.startKidnappedEvent();
				}
				else if (Singleton<DataManager>.instance.currentGameData.unlockTheme > 2)
				{
					int random = UnityEngine.Random.Range(0, 100);
					if (random < 7 || !Singleton<DataManager>.instance.currentGameData.isSeenSurpriseShop)
					{
						Singleton<DataManager>.instance.currentGameData.isSeenSurpriseShop = true;
						UIWindowSurpriseLimitedItem.instance.openLimitedPopupUI();
						Singleton<DataManager>.instance.saveData();
					}
				}
				else if (GameManager.currentTheme >= 1 && GameManager.currentStage >= 5)
				{
					if (NSPlayerPrefs.GetInt("Review", 0) <= 1 && NSPlayerPrefs.GetInt("ReviewLaterDay", 0) != DateTime.Now.DayOfYear)
					{
						UIWindowReview.instance.open();
					}
				}
				else if (!Singleton<TutorialManager>.instance.checkRebirthTutorial())
				{
					UIWindowAdvertisementLimitedProduct.instance.openAdverstisementLimitedProduct();
				}
			}
			if (TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType14);
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType20);
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType26);
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType32);
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType43);
			}
		}
		else if (refreshGameState)
		{
			Singleton<GameManager>.instance.resetGameState();
		}
	}
}
