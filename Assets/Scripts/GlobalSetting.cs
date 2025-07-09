using UnityEngine;

public class GlobalSetting : Singleton<GlobalSetting>
{
	public string bundleVersion = "1.0.0";

	public static string s_bundleVersion = string.Empty;

	public int bundleVersionCode = 1;

	public static int s_bundleVersionCode;

	public bool usingParsingFromGoogleDocs;

	public static bool s_usingParsingFromGoogleDocs;

	public bool ignoreTutorial;

	public static bool s_ignoreTutorial;

	public int targetFrameRate = 60;

	private void Awake()
	{
		Application.targetFrameRate = 60;
		s_bundleVersionCode = 1;
		Screen.sleepTimeout = -1;
		s_bundleVersion = "1.0.0";
		usingParsingFromGoogleDocs = false;
	}
}
