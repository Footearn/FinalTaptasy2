using System;
using System.Runtime.CompilerServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class RewardBasedVideoAdClient //: AndroidJavaProxy, IRewardBasedVideoAdClient
	{
		private AndroidJavaObject androidRewardBasedVideo;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdLoaded = delegate
		{
		};

		[method: MethodImpl(32)]
		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate
		{
		};

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdOpening = delegate
		{
		};

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdStarted = delegate
		{
		};

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdClosed = delegate
		{
		};

		[method: MethodImpl(32)]
		public event EventHandler<Reward> OnAdRewarded = delegate
		{
		};

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdLeavingApplication = delegate
		{
		};

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdCompleted = delegate
		{
		};

		public void CreateRewardBasedVideoAd()
		{
			
		}

		public void LoadAd(AdRequest request, string adUnitId)
		{
			
		}

		public void ShowRewardBasedVideoAd()
		{
			
		}

		private void onAdLoaded()
		{
			if (this.OnAdLoaded != null)
			{
				this.OnAdLoaded(this, EventArgs.Empty);
			}
		}

		private void onAdFailedToLoad(string errorReason)
		{
			if (this.OnAdFailedToLoad != null)
			{
				AdFailedToLoadEventArgs adFailedToLoadEventArgs = new AdFailedToLoadEventArgs();
				adFailedToLoadEventArgs.Message = errorReason;
				AdFailedToLoadEventArgs e = adFailedToLoadEventArgs;
				this.OnAdFailedToLoad(this, e);
			}
		}

		private void onAdOpened()
		{
			if (this.OnAdOpening != null)
			{
				this.OnAdOpening(this, EventArgs.Empty);
			}
		}

		private void onAdStarted()
		{
			if (this.OnAdStarted != null)
			{
				this.OnAdStarted(this, EventArgs.Empty);
			}
		}

		private void onAdClosed()
		{
			if (this.OnAdClosed != null)
			{
				this.OnAdClosed(this, EventArgs.Empty);
			}
		}

		private void onAdRewarded(string type, float amount)
		{
			if (this.OnAdRewarded != null)
			{
				Reward reward = new Reward();
				reward.Type = type;
				reward.Amount = amount;
				Reward e = reward;
				this.OnAdRewarded(this, e);
			}
		}

		private void onAdLeftApplication()
		{
			if (this.OnAdLeavingApplication != null)
			{
				this.OnAdLeavingApplication(this, EventArgs.Empty);
			}
		}

		private void onAdCompleted()
		{
			if (this.OnAdCompleted != null)
			{
				this.OnAdCompleted(this, EventArgs.Empty);
			}
		}
	}
}
