using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class ParsePushesStub
{
	[method: MethodImpl(32)]
	public static event Action<string, Dictionary<string, object>> OnPushReceived;

	static ParsePushesStub()
	{
		ParsePushesStub.OnPushReceived = delegate
		{
		};
	}

	public static void InitParse()
	{
	}
}
