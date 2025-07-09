using System;
using UnityEngine;

public static class CustomUrlSchemeAndroid
{
	private const string KEY_URL = "UrlStr";

	private const string KEY_SCHEME = "UrlScheme";

	private const string KEY_HOST = "UrlHost";

	private const string KEY_PATH = "UrlPath";

	private const string KEY_QUERY = "UrlQuery";

	private static string GetPlayerPrefString(string key, bool clearDataAfterGet)
	{
		string result = null;
		if (PlayerPrefs.HasKey(key))
		{
			result = PlayerPrefs.GetString(key);
			if (clearDataAfterGet)
			{
				PlayerPrefs.DeleteKey(key);
			}
		}
		return result;
	}

	public static string GetLaunchedUrl(bool clearDataAfterGet = true)
	{
		return GetPlayerPrefString("UrlStr", clearDataAfterGet);
	}

	public static string GetLaunchedUrlScheme(bool clearDataAfterGet = true)
	{
		return GetPlayerPrefString("UrlScheme", clearDataAfterGet);
	}

	public static string GetLaunchedUrlHost(bool clearDataAfterGet = true)
	{
		return GetPlayerPrefString("UrlHost", clearDataAfterGet);
	}

	public static string GetLaunchedUrlPath(bool clearDataAfterGet = true)
	{
		return GetPlayerPrefString("UrlPath", clearDataAfterGet);
	}

	public static string GetLaunchedUrlQuery(bool clearDataAfterGet = true)
	{
		return GetPlayerPrefString("UrlQuery", clearDataAfterGet);
	}

	public static void ClearSavedData()
	{
		PlayerPrefs.DeleteKey("UrlStr");
		PlayerPrefs.DeleteKey("UrlScheme");
		PlayerPrefs.DeleteKey("UrlHost");
		PlayerPrefs.DeleteKey("UrlPath");
		PlayerPrefs.DeleteKey("UrlQuery");
	}
}
