using System;
using Com.Google.Android.Gms.Common.Api;
using Com.Google.Android.Gms.Games;
using Com.Google.Android.Gms.Games.Stats;
using GooglePlayGames.BasicApi;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace GooglePlayGames.Android
{
	internal class AndroidClient// : IClientImpl
	{
		private class StatsResultCallback //: ResultCallbackProxy<Stats_LoadPlayerStatsResultObject>
		{
		}

		private TokenClient tokenClient;

		private static AndroidJavaObject invisible;

		public TokenClient CreateTokenClient(bool reset)
		{
			if (tokenClient == null)
			{
				//tokenClient = new AndroidTokenClient();
			}
			else if (reset)
			{
				//tokenClient.Signout();
			}
			return tokenClient;
		}
	}
}
