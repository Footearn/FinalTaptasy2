using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class AdLoaderClient// : AndroidJavaProxy, IAdLoaderClient
	{
		private AndroidJavaObject adLoader;

		private Dictionary<string, Action<CustomNativeTemplateAd, string>> CustomNativeTemplateCallbacks
		{
			get;
			set;
		}

		[method: MethodImpl(32)]
		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		[method: MethodImpl(32)]
		public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;


		public void LoadAd(AdRequest request)
		{
			
		}

		private void onAdFailedToLoad(string errorReason)
		{
			AdFailedToLoadEventArgs adFailedToLoadEventArgs = new AdFailedToLoadEventArgs();
			adFailedToLoadEventArgs.Message = errorReason;
			AdFailedToLoadEventArgs e = adFailedToLoadEventArgs;
			this.OnAdFailedToLoad(this, e);
		}
	}
}
