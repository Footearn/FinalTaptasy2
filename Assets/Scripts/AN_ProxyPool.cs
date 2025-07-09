using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AN_ProxyPool
{
	private static Dictionary<string, AndroidJavaObject> pool = new Dictionary<string, AndroidJavaObject>();

	public static void CallStatic(string className, string methodName, params object[] args)
	{

	}

}
