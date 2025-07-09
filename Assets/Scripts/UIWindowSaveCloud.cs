using UnityEngine;

public class UIWindowSaveCloud : UIWindow
{
	public static UIWindowSaveCloud instance;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void OnClickSave()
	{
		if (Singleton<NanooAPIManager>.instance.UserID.Length > 0)
		{
			Singleton<DataManager>.instance.saveToCloud();
		}
		else if (!Social.localUser.authenticated)
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("REQUIRE_LOGIN"), I18NManager.Get("GOOGLE_PLAY")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}
}
