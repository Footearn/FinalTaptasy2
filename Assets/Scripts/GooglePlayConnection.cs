using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GooglePlayConnection : SA_Singleton<GooglePlayConnection>
{
	private bool _IsInitialized;

	private static GPConnectionState _State = GPConnectionState.STATE_UNCONFIGURED;

	public bool IsConnected
	{
		get
		{
			return State == GPConnectionState.STATE_CONNECTED;
		}
	}

	[Obsolete("state is deprecated, please use State instead.")]
	public static GPConnectionState state
	{
		get
		{
			return State;
		}
	}

	public static GPConnectionState State
	{
		get
		{
			return _State;
		}
	}

	[Obsolete("isInitialized is deprecated, please use IsInitialized instead.")]
	public bool isInitialized
	{
		get
		{
			return IsInitialized;
		}
	}

	public bool IsInitialized
	{
		get
		{
			return _IsInitialized;
		}
	}

	[method: MethodImpl(32)]
	public static event Action<GooglePlayConnectionResult> ActionConnectionResultReceived;

	[method: MethodImpl(32)]
	public static event Action<GPConnectionState> ActionConnectionStateChanged;

	[method: MethodImpl(32)]
	public static event Action ActionPlayerConnected;

	[method: MethodImpl(32)]
	public static event Action ActionPlayerDisconnected;

	static GooglePlayConnection()
	{
		GooglePlayConnection.ActionConnectionResultReceived = delegate
		{
		};
		GooglePlayConnection.ActionConnectionStateChanged = delegate
		{
		};
		GooglePlayConnection.ActionPlayerConnected = delegate
		{
		};
		GooglePlayConnection.ActionPlayerDisconnected = delegate
		{
		};
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		SA_Singleton<GooglePlayManager>.instance.Create();
		Init();
	}

	private void Init()
	{
		string text = string.Empty;
		if (AndroidNativeSettings.Instance.EnableGamesAPI)
		{
			text += "GamesAPI";
		}
		if (AndroidNativeSettings.Instance.EnablePlusAPI)
		{
			text += "PlusAPI";
		}
		if (AndroidNativeSettings.Instance.EnableDriveAPI)
		{
			text += "DriveAPI";
		}
		if (AndroidNativeSettings.Instance.EnableAppInviteAPI)
		{
			text += "AppInvite";
		}
		AN_GMSGeneralProxy.setConnectionParams(AndroidNativeSettings.Instance.ShowConnectingPopup);
		AN_GMSGeneralProxy.playServiceInit(text);
	}

	[Obsolete("connect is deprecated, please use Connect instead.")]
	public void connect()
	{
		Connect();
	}

	public void Connect()
	{
		Connect(null);
	}

	[Obsolete("connect is deprecated, please use Connect instead.")]
	public void connect(string accountName)
	{
		Connect(accountName);
	}

	public void Connect(string accountName)
	{
		if (_State != GPConnectionState.STATE_CONNECTED && _State != GPConnectionState.STATE_CONNECTING)
		{
			OnStateChange(GPConnectionState.STATE_CONNECTING);
			if (accountName != null)
			{
				AN_GMSGeneralProxy.playServiceConnect(accountName);
			}
			else
			{
				AN_GMSGeneralProxy.playServiceConnect();
			}
		}
	}

	[Obsolete("disconnect is deprecated, please use Disconnect instead.")]
	public void disconnect()
	{
		Disconnect();
	}

	public void Disconnect()
	{
		if (_State != GPConnectionState.STATE_DISCONNECTED && _State != GPConnectionState.STATE_CONNECTING)
		{
			OnStateChange(GPConnectionState.STATE_DISCONNECTED);
			AN_GMSGeneralProxy.playServiceDisconnect();
		}
	}

	public static bool CheckState()
	{
		GPConnectionState state = _State;
		if (state == GPConnectionState.STATE_CONNECTED)
		{
			return true;
		}
		return false;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		AN_GMSGeneralProxy.OnApplicationPause(pauseStatus);
	}

	private void OnPlayServiceDisconnected(string data)
	{
		OnStateChange(GPConnectionState.STATE_DISCONNECTED);
	}

	private void OnConnectionResult(string resultCode)
	{
		Debug.Log("[OnConnectionResult] resultCode " + resultCode);
		GooglePlayConnectionResult googlePlayConnectionResult = new GooglePlayConnectionResult();
		googlePlayConnectionResult.code = (GP_ConnectionResultCode)Convert.ToInt32(resultCode);
		if (googlePlayConnectionResult.IsSuccess)
		{
			OnStateChange(GPConnectionState.STATE_CONNECTED);
		}
		else
		{
			OnStateChange(GPConnectionState.STATE_DISCONNECTED);
		}
		GooglePlayConnection.ActionConnectionResultReceived(googlePlayConnectionResult);
	}

	private void OnStateChange(GPConnectionState connectionState)
	{
		_State = connectionState;
		switch (_State)
		{
		case GPConnectionState.STATE_CONNECTED:
			GooglePlayConnection.ActionPlayerConnected();
			break;
		case GPConnectionState.STATE_DISCONNECTED:
			GooglePlayConnection.ActionPlayerDisconnected();
			break;
		}
		GooglePlayConnection.ActionConnectionStateChanged(_State);
		Debug.Log("Play Serice Connection State -> " + _State);
	}
}
