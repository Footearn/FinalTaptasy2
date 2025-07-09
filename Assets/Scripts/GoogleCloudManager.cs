using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GoogleCloudManager : SA_Singleton<GoogleCloudManager>
{
	private int _maxStateSize = -1;

	private int _maxNumKeys = -1;

	private Dictionary<int, byte[]> _states = new Dictionary<int, byte[]>();

	public int maxStateSize
	{
		get
		{
			return _maxStateSize;
		}
	}

	public int maxNumKeys
	{
		get
		{
			return _maxNumKeys;
		}
	}

	public Dictionary<int, byte[]> states
	{
		get
		{
			return _states;
		}
	}

	[method: MethodImpl(32)]
	public static event Action<GoogleCloudResult> ActionStateDeleted;

	[method: MethodImpl(32)]
	public static event Action<GoogleCloudResult> ActionStateUpdated;

	[method: MethodImpl(32)]
	public static event Action<GoogleCloudResult> ActionStateLoaded;

	[method: MethodImpl(32)]
	public static event Action<GoogleCloudResult> ActionStateResolved;

	[method: MethodImpl(32)]
	public static event Action<GoogleCloudResult> ActionStateConflict;

	[method: MethodImpl(32)]
	public static event Action<GoogleCloudResult> ActionAllStatesLoaded;

	static GoogleCloudManager()
	{
		GoogleCloudManager.ActionStateDeleted = delegate
		{
		};
		GoogleCloudManager.ActionStateUpdated = delegate
		{
		};
		GoogleCloudManager.ActionStateLoaded = delegate
		{
		};
		GoogleCloudManager.ActionStateResolved = delegate
		{
		};
		GoogleCloudManager.ActionStateConflict = delegate
		{
		};
		GoogleCloudManager.ActionAllStatesLoaded = delegate
		{
		};
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Create()
	{
		Debug.Log("GoogleCloudManager was created");
	}

	public void loadAllStates()
	{
	}

	public void updateState(int stateKey, byte[] val)
	{
		string str = string.Empty;
		int num = val.Length;
		for (int i = 0; i < num; i++)
		{
			if (i != 0)
			{
				str += ",";
			}
			str += val[i];
		}
	}

	public void resolveState(int stateKey, byte[] resolvedData, string resolvedVersion)
	{
		string str = string.Empty;
		int num = resolvedData.Length;
		for (int i = 0; i < num; i++)
		{
			if (i != 0)
			{
				str += ",";
			}
			str += resolvedData[i];
		}
	}

	public void deleteState(int stateKey)
	{
	}

	public void loadState(int stateKey)
	{
	}

	public byte[] GetStateData(int stateKey)
	{
		if (_states.ContainsKey(stateKey))
		{
			return _states[stateKey];
		}
		return null;
	}

	private void OnAllStatesLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		GoogleCloudResult obj = new GoogleCloudResult(array[0]);
		if (array.Length > 1)
		{
			_states.Clear();
			for (int i = 1; i < array.Length && !(array[i] == "endofline"); i += 2)
			{
				PushStateData(array[i], ConvertStringToCloudData(array[i + 1]));
			}
			Debug.Log("Loaded: " + _states.Count + " States");
		}
		GoogleCloudManager.ActionAllStatesLoaded(obj);
	}

	private void OnStateConflict(string data)
	{
		string[] array = data.Split("|"[0]);
		GoogleCloudResult googleCloudResult = new GoogleCloudResult("0", array[0]);
		if (googleCloudResult.isSuccess)
		{
			googleCloudResult.stateData = ConvertStringToCloudData(array[1]);
			googleCloudResult.serverConflictData = ConvertStringToCloudData(array[2]);
			googleCloudResult.resolvedVersion = array[3];
			PushStateData(array[0], googleCloudResult.stateData);
		}
		GoogleCloudManager.ActionStateConflict(googleCloudResult);
	}

	private void OnStateLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		GoogleCloudResult googleCloudResult = new GoogleCloudResult(array[0], array[1]);
		googleCloudResult.stateData = ConvertStringToCloudData(array[2]);
		PushStateData(array[1], googleCloudResult.stateData);
		GoogleCloudManager.ActionStateLoaded(googleCloudResult);
	}

	private void OnStateResolved(string data)
	{
		string[] array = data.Split("|"[0]);
		GoogleCloudResult googleCloudResult = new GoogleCloudResult(array[0], array[1]);
		googleCloudResult.stateData = ConvertStringToCloudData(array[2]);
		PushStateData(array[1], googleCloudResult.stateData);
		GoogleCloudManager.ActionStateResolved(googleCloudResult);
	}

	private void OnStateUpdated(string data)
	{
		string[] array = data.Split("|"[0]);
		GoogleCloudResult googleCloudResult = new GoogleCloudResult(array[0], array[1]);
		googleCloudResult.stateData = ConvertStringToCloudData(array[2]);
		PushStateData(array[1], googleCloudResult.stateData);
		GoogleCloudManager.ActionStateUpdated(googleCloudResult);
	}

	private void OnKeyDeleted(string data)
	{
		string[] array = data.Split("|"[0]);
		GoogleCloudResult obj = new GoogleCloudResult(array[0], array[1]);
		GoogleCloudManager.ActionStateDeleted(obj);
	}

	private void OnCloudConnected(string data)
	{
		string[] array = data.Split("|"[0]);
		Debug.Log("Google Cloud is connected max state size: " + array[0] + " max state num " + array[1]);
		_maxNumKeys = Convert.ToInt32(array[1]);
		_maxStateSize = Convert.ToInt32(array[0]);
	}

	private void PushStateData(string stateKey, byte[] data)
	{
		PushStateData(Convert.ToInt32(stateKey), data);
	}

	private void PushStateData(int stateKey, byte[] data)
	{
		if (_states.ContainsKey(stateKey))
		{
			_states[stateKey] = data;
		}
		else
		{
			_states.Add(stateKey, data);
		}
	}

	private static byte[] ConvertStringToCloudData(string data)
	{
		if (data == null)
		{
			return null;
		}
		data = data.Replace("endofline", string.Empty);
		if (data.Equals(string.Empty))
		{
			return null;
		}
		string[] array = data.Split(","[0]);
		List<byte> list = new List<byte>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			Debug.Log(text);
			list.Add(Convert.ToByte(text));
		}
		return list.ToArray();
	}
}
