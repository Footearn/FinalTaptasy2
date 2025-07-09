using System;

public class NativePluginManager : INativePluginManager
{
	protected bool _isInit;

	private static NativePluginManager m_instance;

	// public static NativePluginManager Instance
	// {
	// 	get
	// 	{
	// 		if (m_instance == null)
	// 		{
	// 			m_instance = new NativePluginManager_Android();
	// 		}
	// 		return m_instance;
	// 	}
	// }

	public virtual void Init()
	{
		_isInit = true;
	}

	public virtual string GetClipboardString()
	{
		return string.Empty;
	}

	public virtual void SetClipboardString(string name, string content)
	{
	}

	public virtual string GetUDIDString()
	{
		return string.Empty;
	}

	public virtual void RegistRemoteNotification()
	{
	}

	public bool initCheck()
	{
		if (!_isInit)
		{
			throw new Exception("Call \"NativePluginManager.Init();\" before using NativePluginManager.");
		}
		return _isInit;
	}
}
