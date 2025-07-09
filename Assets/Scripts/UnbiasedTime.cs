using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UnbiasedTime : MonoBehaviour
{
	private static UnbiasedTime instance;

	[HideInInspector]
	public long timeOffset;

	[HideInInspector]
	public long? _ntpTimeOffset = 0L;

	private Thread socketThread;

	private UdpClient client;

	private float m_internetOffsetSaveTimer;

	public static UnbiasedTime Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject gameObject = new GameObject("UnbiasedTimeSingleton");
				instance = gameObject.AddComponent<UnbiasedTime>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return instance;
		}
	}

	[HideInInspector]
	public long ntpTimeOffset
	{
		get
		{
			if ((!_ntpTimeOffset.HasValue) ? true : false)
			{
				return 0L;
			}
			return _ntpTimeOffset.Value;
		}
	}

	private void Awake()
	{
		SessionStart();
	}

	public void OnApplicationPauseForUnbiasedTime(bool pause)
	{
		if (pause)
		{
			SessionEnd();
		}
		else
		{
			SessionStart();
		}
	}

	private void OnApplicationQuit()
	{
		SessionEnd();
		if (socketThread != null)
		{
			socketThread.Abort();
		}
	}

	public DateTime UtcNow()
	{
		long? ntpTimeOffset = _ntpTimeOffset;
		if (ntpTimeOffset.HasValue)
		{
			return DateTime.UtcNow.AddSeconds(-1.0 * (double)this.ntpTimeOffset);
		}
		long num = 317700000L;
		long num2 = -317700000L;
		double num3 = ((!(Singleton<DataManager>.instance != null) || Singleton<DataManager>.instance.currentGameData == null) ? 0.0 : Singleton<DataManager>.instance.currentGameData.lastSavedOffsetBetweenInternetAndUnbiasedForUTC);
		long num4 = 0L;
		num4 = ((num3 > (double)num) ? num : ((!(num3 < (double)num2)) ? Convert.ToInt64(num3) : num2));
		return DateTime.UtcNow.AddSeconds(-1f * (float)(timeOffset - num4));
	}

	public DateTime Now()
	{
		long? ntpTimeOffset = _ntpTimeOffset;
		if (ntpTimeOffset.HasValue)
		{
			return DateTime.Now.AddSeconds(-1.0 * (double)this.ntpTimeOffset);
		}
		long num = 317700000L;
		long num2 = -317700000L;
		double num3 = ((!(Singleton<DataManager>.instance != null) || Singleton<DataManager>.instance.currentGameData == null) ? 0.0 : Singleton<DataManager>.instance.currentGameData.lastSavedOffsetBetweenInternetAndUnbiased);
		long num4 = 0L;
		num4 = ((num3 > (double)num) ? num : ((!(num3 < (double)num2)) ? Convert.ToInt64(num3) : num2));
		return DateTime.Now.AddSeconds(-1f * (float)(timeOffset - num4));
	}

	private void Update()
	{
		m_internetOffsetSaveTimer += Time.deltaTime;
		long? ntpTimeOffset = _ntpTimeOffset;
		if (ntpTimeOffset.HasValue)
		{
			if (m_internetOffsetSaveTimer >= 1f)
			{
				m_internetOffsetSaveTimer = 0f;
				UpdateOffsetBetweenInternetTimeAndUnbiasedTime();
			}
		}
		else if (m_internetOffsetSaveTimer >= 1f)
		{
			m_internetOffsetSaveTimer = 0f;
			if (Util.isInternetConnection() && socketThread == null)
			{
				SessionEnd();
				SessionStart();
			}
		}
		if (!Util.isInternetConnection())
		{
			long? ntpTimeOffset2 = _ntpTimeOffset;
			if (ntpTimeOffset2.HasValue)
			{
				_ntpTimeOffset = null;
			}
			if (socketThread != null)
			{
				socketThread = null;
			}
		}
	}

	public void UpdateOffsetBetweenInternetTimeAndUnbiasedTime()
	{
		if (UIWindowIntro.isLoaded)
		{
			long? ntpTimeOffset = _ntpTimeOffset;
			if (ntpTimeOffset.HasValue && Singleton<DataManager>.instance != null && Singleton<DataManager>.instance.currentGameData != null)
			{
				Singleton<DataManager>.instance.currentGameData.lastSavedOffsetBetweenInternetAndUnbiased = DateTime.Now.AddSeconds(-1.0 * (double)this.ntpTimeOffset).Subtract(DateTime.Now.AddSeconds(-1f * (float)timeOffset)).TotalSeconds;
				Singleton<DataManager>.instance.currentGameData.lastSavedOffsetBetweenInternetAndUnbiasedForUTC = DateTime.UtcNow.AddSeconds(-1.0 * (double)this.ntpTimeOffset).Subtract(DateTime.UtcNow.AddSeconds(-1f * (float)timeOffset)).TotalSeconds;
			}
		}
	}

	public void UpdateNetworkTimeOffset()
	{
		socketThread = new Thread(NtpImpl);
		socketThread.IsBackground = true;
		socketThread.Start();
	}

	private void SessionStart()
	{
		//StartAndroid();
		UpdateNetworkTimeOffset();
		UpdateOffsetBetweenInternetTimeAndUnbiasedTime();
	}

	private void SessionEnd()
	{
		//EndAndroid();
		_ntpTimeOffset = null;
	}

	private void NtpImpl()
	{
		byte[] array = new byte[48];
		array[0] = 27;
		try
		{
			IPAddress[] addressList = Dns.GetHostEntry("pool.ntp.org").AddressList;
			IPAddress iPAddress = null;
			for (int i = 0; i < addressList.Length; i++)
			{
				if (addressList[i].AddressFamily == AddressFamily.InterNetwork)
				{
					iPAddress = addressList[i];
					break;
				}
			}
			if (iPAddress == null)
			{
				_ntpTimeOffset = null;
				return;
			}
			IPEndPoint remoteEP = new IPEndPoint(iPAddress, 123);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.ReceiveTimeout = 5000;
			socket.SendTimeout = 5000;
			socket.Connect(remoteEP);
			socket.Send(array);
			socket.Receive(array);
			socket.Close();
			ulong num = ((ulong)array[40] << 24) | ((ulong)array[41] << 16) | ((ulong)array[42] << 8) | array[43];
			ulong num2 = ((ulong)array[44] << 24) | ((ulong)array[45] << 16) | ((ulong)array[46] << 8) | array[47];
			ulong num3 = num * 1000 + num2 * 1000 / 4294967296uL;
			DateTime d = new DateTime(1900, 1, 1).AddMilliseconds((long)num3);
			_ntpTimeOffset = (long)(DateTime.UtcNow - d).TotalSeconds;
		}
		catch (Exception)
		{
			_ntpTimeOffset = null;
		}
	}
}
