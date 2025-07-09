using System;
using System.Runtime.CompilerServices;

public class AndroidAppInfoLoader : SA_Singleton<AndroidAppInfoLoader>
{
	public PackageAppInfo PacakgeInfo = new PackageAppInfo();

	[method: MethodImpl(32)]
	public static event Action<PackageAppInfo> ActionPacakgeInfoLoaded;

	static AndroidAppInfoLoader()
	{
		AndroidAppInfoLoader.ActionPacakgeInfoLoaded = delegate
		{
		};
	}

	public void LoadPackageInfo()
	{
		AndroidNative.LoadPackageInfo();
	}

	private void OnPackageInfoLoaded(string data)
	{
		string[] array = data.Split("|"[0]);
		PacakgeInfo.versionName = array[0];
		PacakgeInfo.versionCode = array[1];
		PacakgeInfo.packageName = array[2];
		PacakgeInfo.lastUpdateTime = Convert.ToInt64(array[3]);
		PacakgeInfo.sharedUserId = array[3];
		PacakgeInfo.sharedUserLabel = array[4];
		AndroidAppInfoLoader.ActionPacakgeInfoLoaded(PacakgeInfo);
	}
}
