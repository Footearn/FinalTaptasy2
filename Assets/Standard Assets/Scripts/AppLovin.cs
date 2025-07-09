using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AppLovin
{
	public AppLovin()
	{
	}


	public static void ShowInterstitial()
	{
		//getDefaultPlugin().showInterstitial();
	}

	public static void ShowInterstitial(string placement)
	{
		//getDefaultPlugin().showInterstitial(placement);
	}

	public static void ShowInterstitialForZoneId(string zoneId)
	{
		//getDefaultPlugin().showInterstitialForZoneId(zoneId);
	}

	public static void LoadRewardedInterstitial(string zoneId = null)
	{
		//getDefaultPlugin().loadIncentInterstitial(zoneId);
	}

	public static void ShowRewardedInterstitial()
	{
		//getDefaultPlugin().showIncentInterstitial();
	}

	public static void ShowRewardedInterstitial(string placement)
	{
		//getDefaultPlugin().showIncentInterstitial(placement);
	}

	public static void ShowRewardedInterstitialForZoneId(string zoneId = null)
	{
		//getDefaultPlugin().showIncentInterstitialForZoneId(zoneId);
	}

	public static void HideAd()
	{
		//getDefaultPlugin().hideAd();
	}

	public static void SetAdPosition(float x, float y)
	{
		//getDefaultPlugin().setAdPosition(x, y);
	}

	public static void SetAdWidth(int width)
	{
		//getDefaultPlugin().setAdWidth(width);
	}

	public static void SetSdkKey(string sdkKey)
	{
		//getDefaultPlugin().setSdkKey(sdkKey);
	}

	public static void SetVerboseLoggingOn(string verboseLogging)
	{
		//getDefaultPlugin().setVerboseLoggingOn(verboseLogging);
	}

	public static void SetMuted(string muted)
	{
		//getDefaultPlugin().setMuted(muted);
	}

	// public static bool IsMuted()
	// {
	// 	return getDefaultPlugin().isMuted();
	// }

	// public static void SetTestAdsEnabled(string enabled)
	// {
	// 	getDefaultPlugin().setTestAdsEnabled(enabled);
	// }

	// public static bool IsTestAdsEnabled()
	// {
	// 	return getDefaultPlugin().isTestAdsEnabled();
	// }

	// public static void PreloadInterstitial(string zoneId = null)
	// {
	// 	getDefaultPlugin().preloadInterstitial(zoneId);
	// }

	// public static bool HasPreloadedInterstitial(string zoneId = null)
	// {
	// 	return getDefaultPlugin().hasPreloadedInterstitial(zoneId);
	// }

	// public static bool IsInterstitialShowing()
	// {
	// 	return getDefaultPlugin().isInterstitialShowing();
	// }

	// public static bool IsIncentInterstitialReady(string zoneId = null)
	// {
	// 	return getDefaultPlugin().isIncentInterstitialReady(zoneId);
	// }

	// public static bool IsPreloadedInterstitialVideo()
	// {
	// 	return getDefaultPlugin().isPreloadedInterstitialVideo();
	// }

	public static void InitializeSdk()
	{
		//getDefaultPlugin().initializeSdk();
	}

	public static void SetUnityAdListener(string gameObjectToNotify)
	{
		//getDefaultPlugin().setAdListener(gameObjectToNotify);
	}

	public static void SetRewardedVideoUsername(string username)
	{
		//getDefaultPlugin().setRewardedVideoUsername(username);
	}

	public static void TrackEvent(string eventType, IDictionary<string, string> parameters)
	{
		//getDefaultPlugin().trackEvent(eventType, parameters);
	}

	public static void EnableImmersiveMode()
	{
		//getDefaultPlugin().enableImmersiveMode();
	}

	public static void SetHasUserConsent(string hasUserConsent)
	{
		//getDefaultPlugin().setHasUserConsent(hasUserConsent);
	}
}
