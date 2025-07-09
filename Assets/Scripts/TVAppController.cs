using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TVAppController : SA_Singleton<TVAppController>
{
	private bool _IsRuningOnTVDevice;

	public bool IsRuningOnTVDevice
	{
		get
		{
			return _IsRuningOnTVDevice;
		}
	}

	[method: MethodImpl(32)]
	public static event Action DeviceTypeChecked;

	static TVAppController()
	{
		TVAppController.DeviceTypeChecked = delegate
		{
		};
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void CheckForATVDevice()
	{
		AN_TVControllerProxy.AN_CheckForATVDevice();
	}

	private void OnDeviceStateResponce(string data)
	{
		if (data.Equals("1"))
		{
			_IsRuningOnTVDevice = true;
		}
		TVAppController.DeviceTypeChecked();
	}
}
