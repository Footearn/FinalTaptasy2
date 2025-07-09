using System;
using Com.Google.Android.Gms.Common.Api;
using Google.Developers;

namespace GooglePlayGames.Android
{
	internal class TokenResult : JavaObjWrapper//, Result
	{
		public TokenResult(IntPtr ptr)
			: base(ptr)
		{
		}
	}
}
