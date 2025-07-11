using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	public class RewardBasedVideoAd
	{
		private IRewardBasedVideoAdClient client;

		private static readonly RewardBasedVideoAd instance = new RewardBasedVideoAd();

		public static RewardBasedVideoAd Instance
		{
			get
			{
				return instance;
			}
		}

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdLoaded;

		[method: MethodImpl(32)]
		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdOpening;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdStarted;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdClosed;

		[method: MethodImpl(32)]
		public event EventHandler<Reward> OnAdRewarded;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdLeavingApplication;

		[method: MethodImpl(32)]
		public event EventHandler<EventArgs> OnAdCompleted;

		private RewardBasedVideoAd()
		{
			Type type = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp");
			MethodInfo method = type.GetMethod("BuildRewardBasedVideoAdClient", BindingFlags.Static | BindingFlags.Public);
			client = (IRewardBasedVideoAdClient)method.Invoke(null, null);
			client.CreateRewardBasedVideoAd();
			client.OnAdLoaded += delegate(object sender, EventArgs args)
			{
				if (this.OnAdLoaded != null)
				{
					this.OnAdLoaded(this, args);
				}
			};
			client.OnAdFailedToLoad += delegate(object sender, AdFailedToLoadEventArgs args)
			{
				if (this.OnAdFailedToLoad != null)
				{
					this.OnAdFailedToLoad(this, args);
				}
			};
			client.OnAdOpening += delegate(object sender, EventArgs args)
			{
				if (this.OnAdOpening != null)
				{
					this.OnAdOpening(this, args);
				}
			};
			client.OnAdStarted += delegate(object sender, EventArgs args)
			{
				if (this.OnAdStarted != null)
				{
					this.OnAdStarted(this, args);
				}
			};
			client.OnAdClosed += delegate(object sender, EventArgs args)
			{
				if (this.OnAdClosed != null)
				{
					this.OnAdClosed(this, args);
				}
			};
			client.OnAdLeavingApplication += delegate(object sender, EventArgs args)
			{
				if (this.OnAdLeavingApplication != null)
				{
					this.OnAdLeavingApplication(this, args);
				}
			};
			client.OnAdRewarded += delegate(object sender, Reward args)
			{
				if (this.OnAdRewarded != null)
				{
					this.OnAdRewarded(this, args);
				}
			};
			client.OnAdCompleted += delegate(object sender, EventArgs args)
			{
				if (this.OnAdCompleted != null)
				{
					this.OnAdCompleted(this, args);
				}
			};
		}

		public void LoadAd(AdRequest request, string adUnitId)
		{
			client.LoadAd(request, adUnitId);
		}

		public bool IsLoaded()
		{
			return client.IsLoaded();
		}

		public void Show()
		{
			client.ShowRewardBasedVideoAd();
		}

		public void SetUserId(string userId)
		{
			client.SetUserId(userId);
		}

		public string MediationAdapterClassName()
		{
			return client.MediationAdapterClassName();
		}
	}
}
