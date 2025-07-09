using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BaseAndroidPopup : MonoBehaviour
{
	public string title;

	public string message;

	[method: MethodImpl(32)]
	public event Action<AndroidDialogResult> ActionComplete = delegate
	{
	};

	public void onDismissed(string data)
	{
		this.ActionComplete(AndroidDialogResult.CLOSED);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected void DispatchAction(AndroidDialogResult res)
	{
		this.ActionComplete(res);
	}
}
