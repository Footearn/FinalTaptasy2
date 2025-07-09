public class UIWindowReview : UIWindow
{
	public static UIWindowReview instance;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void OnClickReview()
	{
		if (NSPlayerPrefs.GetInt("Review", 0) == 0)
		{
			AnalyzeManager.retention(AnalyzeManager.CategoryType.Review, AnalyzeManager.ActionType.ReviewRate);
		}
		NSPlayerPrefs.SetInt("Review", 2);
		AndroidNativeUtility.OpenAppRatingPage("market://details?id=com.nanoo.finaltaptasy");
		close();
	}

	public void OnClickLater()
	{
		if (NSPlayerPrefs.GetInt("Review", 0) == 0)
		{
			AnalyzeManager.retention(AnalyzeManager.CategoryType.Review, AnalyzeManager.ActionType.ReviewLater);
		}
		NSPlayerPrefs.SetInt("Review", 1);
		NSPlayerPrefs.SetInt("ReviewLaterDay", UnbiasedTime.Instance.Now().DayOfYear);
		close();
	}

	public void OnClickClose()
	{
		if (NSPlayerPrefs.GetInt("Review", 0) == 0)
		{
			AnalyzeManager.retention(AnalyzeManager.CategoryType.Review, AnalyzeManager.ActionType.ReviewClose);
		}
		NSPlayerPrefs.SetInt("Review", 2);
		close();
	}
}
