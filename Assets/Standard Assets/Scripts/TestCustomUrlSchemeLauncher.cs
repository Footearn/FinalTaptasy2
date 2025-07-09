using UnityEngine;

public class TestCustomUrlSchemeLauncher : MonoBehaviour
{
	private string _Url = "URL is nothing...";

	private string _Scheme = "Scheme is nothing...";

	private string _Host = "Host is nothing...";

	private string _Path = "Path is nothing...";

	private string _Query = "Query is nothing...";

	private string _PackageInstalledCheckerMessage = string.Empty;

	private string _YourPackageName = "your.package.name";

	private void Update()
	{
		if (UnityEngine.Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
		GUILayout.FlexibleSpace();
		GUIStyle none = GUIStyle.none;
		none.fontSize = 30;
		none.alignment = TextAnchor.MiddleCenter;
		float width = Screen.width;
		float height = (float)Screen.height * 0.1f;
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(_Url, none, GUILayout.Width(width), GUILayout.Height(height));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(_Scheme, none, GUILayout.Width(width), GUILayout.Height(height));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(_Host, none, GUILayout.Width(width), GUILayout.Height(height));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(_Path, none, GUILayout.Width(width), GUILayout.Height(height));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(_Query, none, GUILayout.Width(width), GUILayout.Height(height));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Get the info of launch from URL.", GUILayout.Width(width), GUILayout.Height(height)))
		{
			string launchedUrl = CustomUrlSchemeAndroid.GetLaunchedUrl();
			string launchedUrlScheme = CustomUrlSchemeAndroid.GetLaunchedUrlScheme();
			string launchedUrlHost = CustomUrlSchemeAndroid.GetLaunchedUrlHost();
			string launchedUrlPath = CustomUrlSchemeAndroid.GetLaunchedUrlPath();
			string launchedUrlQuery = CustomUrlSchemeAndroid.GetLaunchedUrlQuery();
			_Url = ((!string.IsNullOrEmpty(launchedUrl)) ? ("URL is : " + launchedUrl) : "URL is nothing...");
			_Scheme = ((!string.IsNullOrEmpty(launchedUrlScheme)) ? ("Scheme is : " + launchedUrlScheme) : "Scheme is nothing...");
			_Host = ((!string.IsNullOrEmpty(launchedUrlHost)) ? ("Host is : " + launchedUrlHost) : "Host is nothing...");
			_Path = ((!string.IsNullOrEmpty(launchedUrlPath)) ? ("Path is : " + launchedUrlPath) : "Path is nothing...");
			_Query = ((!string.IsNullOrEmpty(launchedUrlQuery)) ? ("Query is : " + launchedUrlQuery) : "Query is nothing...");
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		_YourPackageName = GUILayout.TextField(_YourPackageName, GUILayout.Width(width), GUILayout.Height(height));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Check the package is installed.", GUILayout.Width(width), GUILayout.Height(height)))
		{
			//_PackageInstalledCheckerMessage = ((!CustomUrlSchemeAndroid.IsPackageInstalled(_YourPackageName)) ? (_YourPackageName + " is not installed.") : (_YourPackageName + " is installed!!"));
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(_PackageInstalledCheckerMessage, none, GUILayout.Width(width), GUILayout.Height(height));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
}
