using System;
using System.Collections.Generic;
using Com.Google.Android.Gms.Common.Api;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace GooglePlayGames.Android
{
	internal class AndroidTokenClient //: TokenClient
	{
		private const string TokenFragmentClass = "com.google.games.bridge.TokenFragment";

		private const string FetchTokenSignature = "(Landroid/app/Activity;ZZZLjava/lang/String;Z[Ljava/lang/String;ZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;";

		private const string FetchTokenMethod = "fetchToken";

		private const string GetAnotherAuthCodeMethod = "getAnotherAuthCode";

		private const string GetAnotherAuthCodeSignature = "(Landroid/app/Activity;ZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;";

		private bool requestEmail;

		private bool requestAuthCode;

		private bool requestIdToken;

		private List<string> oauthScopes;

		private string webClientId;

		private bool forceRefresh;

		private bool hidePopups;

		private string accountName;

		private string email;

		private string authCode;

		private string idToken;
		public void SetRequestAuthCode(bool flag, bool forceRefresh)
		{
			requestAuthCode = flag;
			this.forceRefresh = forceRefresh;
		}

		public void SetRequestEmail(bool flag)
		{
			requestEmail = flag;
		}

		public void SetRequestIdToken(bool flag)
		{
			requestIdToken = flag;
		}

		public void SetWebClientId(string webClientId)
		{
			this.webClientId = webClientId;
		}

		public void SetHidePopups(bool flag)
		{
			hidePopups = flag;
		}

		public void SetAccountName(string accountName)
		{
			this.accountName = accountName;
		}

		public void AddOauthScopes(string[] scopes)
		{
			if (scopes != null)
			{
				if (oauthScopes == null)
				{
					oauthScopes = new List<string>();
				}
				oauthScopes.AddRange(scopes);
			}
		}

		public bool NeedsToRun()
		{
			return requestAuthCode || requestEmail || requestIdToken;
		}

		public void FetchTokens(Action<int> callback)
		{
			PlayGamesHelperObject.RunOnGameThread(delegate
			{
				DoFetchToken(callback);
			});
		}

		internal void DoFetchToken(Action<int> callback)
		{
			object[] args = new object[9];
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{

			}
			catch (Exception ex)
			{
				
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public string GetEmail()
		{
			return email;
		}

		public string GetAuthCode()
		{
			return authCode;
		}

		public void GetAnotherServerAuthCode(bool reAuthenticateIfNeeded, Action<string> callback)
		{
			object[] args = new object[3];
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{

			}
			catch (Exception ex)
			{
		;
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public string GetIdToken()
		{
			return idToken;
		}
	}
}
