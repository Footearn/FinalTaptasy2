using System;
using UnityEngine;

public class Invoker : MonoBehaviour
{
	private Action _callback;

	public static Invoker Create()
	{
		return new GameObject("Invoker").AddComponent<Invoker>();
	}

	public void StartInvoke(Action callback, float time)
	{
		_callback = callback;
		Invoke("TimeOut", time);
	}

	private void TimeOut()
	{
		if (_callback != null)
		{
			_callback();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
