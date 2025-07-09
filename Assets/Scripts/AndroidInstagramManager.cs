using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AndroidInstagramManager : SA_Singleton<AndroidInstagramManager>
{
	[method: MethodImpl(32)]
	public static event Action<InstagramPostResult> OnPostingCompleteAction;

	static AndroidInstagramManager()
	{
		AndroidInstagramManager.OnPostingCompleteAction = delegate
		{
		};
	}

	public void Share(Texture2D texture)
	{
		Share(texture, string.Empty);
	}

	public void Share(Texture2D texture, string message)
	{
		byte[] inArray = texture.EncodeToPNG();
		string data = Convert.ToBase64String(inArray);
		AN_SocialSharingProxy.InstagramPostImage(data, message);
	}

	private void OnPostSuccess()
	{
		AndroidInstagramManager.OnPostingCompleteAction(InstagramPostResult.RESULT_OK);
	}

	private void OnPostFailed(string data)
	{
		int num = Convert.ToInt32(data);
		InstagramPostResult obj = InstagramPostResult.NO_APPLICATION_INSTALLED;
		switch (num)
		{
		case 1:
			obj = InstagramPostResult.NO_APPLICATION_INSTALLED;
			break;
		case 2:
			obj = InstagramPostResult.INTERNAL_EXCEPTION;
			break;
		}
		AndroidInstagramManager.OnPostingCompleteAction(obj);
	}
}
