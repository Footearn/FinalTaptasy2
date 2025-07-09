using System;
using Google.Developers;

namespace Com.Google.Android.Gms.Common.Api
{
	public class PendingResult<R> : JavaObjWrapper where R : Result
	{
		private const string CLASS_NAME = "com/google/android/gms/common/api/PendingResult";

		public PendingResult(IntPtr ptr)
			: base(ptr)
		{
		}

		public void cancel()
		{
			InvokeCallVoid("cancel", "()V");
		}

		public void setResultCallback(ResultCallback<R> arg_ResultCallback_1)
		{
			InvokeCallVoid("setResultCallback", "(Lcom/google/android/gms/common/api/ResultCallback;)V", arg_ResultCallback_1);
		}
	}
}
