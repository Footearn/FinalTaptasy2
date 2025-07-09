using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Fabric.Crashlytics.Internal
{
	internal class AndroidImpl : Impl
	{
		public class JavaInteropException : Exception
		{
			public JavaInteropException(string message)
				: base(message)
			{
			}
		}

		private readonly List<IntPtr> references = new List<IntPtr>();

		private AndroidJavaObject native;

		private AndroidJavaClass crashWrapper;

		private AndroidJavaObject instance;


		public override void Crash()
		{
		}

		public override void Log(string message)
		{
			
		}

		public override void SetKeyValue(string key, string value)
		{
			
		}

		public override void SetUserIdentifier(string identifier)
		{
			
		}

		public override void SetUserEmail(string email)
		{
			
		}

		public override void SetUserName(string name)
		{
			
		}

		public override void RecordCustomException(string name, string reason, StackTrace stackTrace)
		{
			RecordCustomException(name, reason, stackTrace.ToString());
		}

		public override void RecordCustomException(string name, string reason, string stackTraceString)
		{
	
		}
	}
}
