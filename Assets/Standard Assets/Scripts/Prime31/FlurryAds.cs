using UnityEngine;

namespace Prime31
{
	public class FlurryAds
	{
		private static AndroidJavaObject _plugin;

		static FlurryAds()
		{
			if (Application.platform == RuntimePlatform.Android)
			{

			}
		}

		public static void enableAds(bool enableTestAds = false)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				
			}
		}

		public static void fetchAdsForSpace(string adSpace, FlurryAdPlacement adSize)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				
			}
		}

		public static void displayAd(string adSpace, FlurryAdPlacement adSize, long timeout)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				
			}
		}

		public static void removeAd(string adSpace)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				
			}
		}

		public static void checkIfAdIsAvailable(string adSpace, FlurryAdPlacement adSize, long timeout)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				
			}
		}
	}
}
