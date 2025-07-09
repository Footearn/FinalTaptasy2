using System;
using System.Runtime.CompilerServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class InterstitialClient //: AndroidJavaProxy, IInterstitialClient
	{
		private AndroidJavaObject interstitial;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdLoaded;

		[method: MethodImpl(32)]
		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdOpening;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdClosed;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdLeavingApplication;

		public void CreateInterstitialAd(string adUnitId)
		{
			
		}

		public void LoadAd(AdRequest request)
		{
			
		}

		public void ShowInterstitial()
		{
			
		}

		public void DestroyInterstitial()
		{
			
		}

		public void onAdLoaded()
		{
			if (this.OnAdLoaded != null)
			{
				this.OnAdLoaded(this, EventArgs.Empty);
			}
		}

		public void onAdFailedToLoad(string errorReason)
		{
			if (this.OnAdFailedToLoad != null)
			{
				AdFailedToLoadEventArgs adFailedToLoadEventArgs = new AdFailedToLoadEventArgs();
				adFailedToLoadEventArgs.Message = errorReason;
				AdFailedToLoadEventArgs e = adFailedToLoadEventArgs;
				this.OnAdFailedToLoad(this, e);
			}
		}

		public void onAdOpened()
		{
			if (this.OnAdOpening != null)
			{
				this.OnAdOpening(this, EventArgs.Empty);
			}
		}

		public void onAdClosed()
		{
			if (this.OnAdClosed != null)
			{
				this.OnAdClosed(this, EventArgs.Empty);
			}
		}

		public void onAdLeftApplication()
		{
			if (this.OnAdLeavingApplication != null)
			{
				this.OnAdLeavingApplication(this, EventArgs.Empty);
			}
		}
	}
}
