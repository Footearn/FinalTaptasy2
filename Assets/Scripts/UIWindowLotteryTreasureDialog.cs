using System;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowLotteryTreasureDialog : UIWindow
{
	public static UIWindowLotteryTreasureDialog instance;

	public Text descriptionText;

	public GameObject okAndCloseObject;

	public GameObject closeObject;

	private Action m_okCallback;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openCloseDescription(string dialogText)
	{
		descriptionText.text = I18NManager.Get(dialogText);
		okAndCloseObject.SetActive(false);
		closeObject.SetActive(true);
		m_okCallback = null;
		open();
	}

	public void openWithDescription(string dialogText, bool isLotteryByRuby, Action okCallBack)
	{
		descriptionText.text = I18NManager.Get(dialogText);
		if (isLotteryByRuby)
		{
			Text text = descriptionText;
			text.text = text.text + "\n(" + string.Format(I18NManager.Get("TREASURE_LOTTERY_ENOUGH_COUNT_DESCRIPTION"), Singleton<DataManager>.instance.currentGameData.treasureRubyLotteryEnoughCount) + ")";
		}
		okAndCloseObject.SetActive(true);
		closeObject.SetActive(false);
		m_okCallback = okCallBack;
		open();
	}

	public void clickOkButton()
	{
		close();
		if (m_okCallback != null)
		{
			m_okCallback();
		}
	}
}
