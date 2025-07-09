using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class MobileAdsClient// : IMobileAdsClient
	{
		private static MobileAdsClient instance = new MobileAdsClient();

		public static MobileAdsClient Instance
		{
			get
			{
				return instance;
			}
		}

		private MobileAdsClient()
		{
		}

		public void SetiOSAppPauseOnBackground(bool pause)
		{
		}
	}
}
