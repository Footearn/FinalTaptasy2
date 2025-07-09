using UnityEngine;

public class UIWindowTerms : UIWindow
{
	public GameObject OKbutton_enable;

	public GameObject OKbutton_disable;

	public GameObject checkImage;

	private bool agree;

	private string targetUrl = "https://game-service.playnanoo.com/privacy";

	private void Start()
	{
		if (PlayerPrefs.GetInt("TermsAgree", 0) == 1)
		{
			close();
		}
	}

	public void clickCheckButton()
	{
		checkImage.SetActive(!agree);
		OKbutton_disable.SetActive(agree);
		OKbutton_enable.SetActive(!agree);
		agree = !agree;
	}

	public void clickOkButton()
	{
		PlayerPrefs.SetInt("TermsAgree", 1);
		close();
	}

	public void clickOnTermsButton()
	{
		if (!string.IsNullOrEmpty(targetUrl))
		{
			Singleton<NanooAPIManager>.instance.OpenForumView(targetUrl);
		}
	}
}
