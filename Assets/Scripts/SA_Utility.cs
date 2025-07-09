using System;

public static class SA_Utility
{
	public static void Invoke(float time, Action callback)
	{
		Invoker invoker = Invoker.Create();
		invoker.StartInvoke(callback, time);
	}
}
