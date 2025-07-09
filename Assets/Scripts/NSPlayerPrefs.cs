using System.Collections.Generic;
using UnityEngine;

public class NSPlayerPrefs : Singleton<NSPlayerPrefs>
{
	public static int totalReadCount = 0;

	public static int totalWriteCount = 0;

	public static int totalSameDataWriteCount = 0;

	public static Dictionary<string, int> readCount = new Dictionary<string, int>();

	public static Dictionary<string, int> writeCount = new Dictionary<string, int>();

	public static Dictionary<string, int> sameDataWriteCount = new Dictionary<string, int>();

	public static Dictionary<string, int> cachedInt = new Dictionary<string, int>();

	public static Dictionary<string, float> cachedFloat = new Dictionary<string, float>();

	public static Dictionary<string, string> cachedString = new Dictionary<string, string>();

	private void _logDict(Dictionary<string, int> dict)
	{
		int num = 5;
		int num2 = 0;
		foreach (KeyValuePair<string, int> item in dict)
		{
			if (num2 % num == 0)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
			}
			GUILayout.Label(item.Key, GUILayout.Width(80f));
			GUILayout.Label(item.Value + string.Empty, GUILayout.Width(20f));
			num2++;
			if (num2 % num == 0)
			{
				GUILayout.EndHorizontal();
			}
		}
		if (num2 % num != 0)
		{
			GUILayout.EndHorizontal();
		}
	}

	private static void logRead(string key)
	{
		totalReadCount++;
		if (readCount.ContainsKey(key))
		{
			Dictionary<string, int> dictionary;
			Dictionary<string, int> dictionary2 = (dictionary = readCount);
			string key2;
			string key3 = (key2 = key);
			int num = dictionary[key2];
			dictionary2[key3] = num + 1;
		}
		else
		{
			readCount.Add(key, 1);
		}
	}

	private static void logWrite(string key, bool isSameValue)
	{
		totalWriteCount++;
		if (isSameValue)
		{
			totalSameDataWriteCount++;
		}
		Dictionary<string, int> dictionary = ((!isSameValue) ? writeCount : sameDataWriteCount);
		if (dictionary.ContainsKey(key))
		{
			Dictionary<string, int> dictionary2;
			Dictionary<string, int> dictionary3 = (dictionary2 = dictionary);
			string key2;
			string key3 = (key2 = key);
			int num = dictionary2[key2];
			dictionary3[key3] = num + 1;
		}
		else
		{
			dictionary.Add(key, 1);
		}
	}

	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
		cachedInt.Clear();
		cachedFloat.Clear();
		cachedString.Clear();
	}

	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
		if (cachedInt.ContainsKey(key))
		{
			cachedInt.Remove(key);
		}
		else if (cachedFloat.ContainsKey(key))
		{
			cachedFloat.Remove(key);
		}
		else if (cachedString.ContainsKey(key))
		{
			cachedString.Remove(key);
		}
	}

	public static float GetFloat(string key)
	{
		return _GetFloat(key);
	}

	public static float GetFloat(string key, float defaultValue)
	{
		return _GetFloat(key, defaultValue);
	}

	private static float _GetFloat(string key, float? defaultValue = null)
	{
		logRead(key);
		float num;
		if (!cachedFloat.ContainsKey(key))
		{
			if (!defaultValue.HasValue)
			{
				num = PlayerPrefs.GetFloat(key);
				if (PlayerPrefs.HasKey(key))
				{
					cachedFloat.Add(key, num);
				}
			}
			else
			{
				num = PlayerPrefs.GetFloat(key, defaultValue.Value);
			}
		}
		else
		{
			num = cachedFloat[key];
		}
		return num;
	}

	public static int GetInt(string key)
	{
		return _GetInt(key);
	}

	public static int GetInt(string key, int defaultValue)
	{
		return _GetInt(key, defaultValue);
	}

	private static int _GetInt(string key, int? defaultValue = null)
	{
		logRead(key);
		int num;
		if (!cachedInt.ContainsKey(key))
		{
			if (!defaultValue.HasValue)
			{
				num = PlayerPrefs.GetInt(key);
				if (PlayerPrefs.HasKey(key))
				{
					cachedInt.Add(key, num);
				}
			}
			else
			{
				num = PlayerPrefs.GetInt(key, defaultValue.Value);
			}
		}
		else
		{
			num = cachedInt[key];
		}
		return num;
	}

	public static string GetString(string key)
	{
		return _GetString(key);
	}

	public static string GetString(string key, string defaultValue)
	{
		return _GetString(key, defaultValue);
	}

	private static string _GetString(string key, string defaultValue = null)
	{
		logRead(key);
		string text;
		if (!cachedString.ContainsKey(key))
		{
			if (string.IsNullOrEmpty(defaultValue))
			{
				text = PlayerPrefs.GetString(key);
				if (PlayerPrefs.HasKey(key))
				{
					cachedString.Add(key, text);
				}
			}
			else
			{
				text = PlayerPrefs.GetString(key, defaultValue);
				
			}
		}
		else
		{
			text = cachedString[key];
		}
		return text;
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	public static void Save()
	{
		PlayerPrefs.Save();
	}

	public static void SetFloat(string key, float value)
	{
		if (!cachedFloat.ContainsKey(key))
		{
			cachedFloat.Add(key, value);
			PlayerPrefs.SetFloat(key, value);
			logWrite(key, false);
		}
		else if (cachedFloat[key] != value)
		{
			cachedFloat[key] = value;
			PlayerPrefs.SetFloat(key, value);
			logWrite(key, false);
		}
		else
		{
			logWrite(key, true);
		}
	}

	public static void SetInt(string key, int value)
	{
		if (!cachedInt.ContainsKey(key))
		{
			cachedInt.Add(key, value);
			PlayerPrefs.SetInt(key, value);
			logWrite(key, false);
		}
		else if (cachedInt[key] != value)
		{
			cachedInt[key] = value;
			PlayerPrefs.SetInt(key, value);
			logWrite(key, false);
		}
		else
		{
			logWrite(key, true);
		}
	}

	public static void SetString(string key, string value)
	{
		if (!cachedString.ContainsKey(key))
		{
			cachedString.Add(key, value);
			PlayerPrefs.SetString(key, value);
			logWrite(key, false);
		}
		else if (cachedString[key] != value)
		{
			cachedString[key] = value;
			PlayerPrefs.SetString(key, value);
			logWrite(key, false);
		}
		else
		{
			logWrite(key, true);
		}
	}
}
