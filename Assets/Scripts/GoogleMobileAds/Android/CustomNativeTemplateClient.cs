using System.Collections.Generic;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	internal class CustomNativeTemplateClient// : ICustomNativeTemplateClient
	{
		private AndroidJavaObject customNativeAd;

		public CustomNativeTemplateClient(AndroidJavaObject customNativeAd)
		{
			this.customNativeAd = customNativeAd;
		}
	}
}
