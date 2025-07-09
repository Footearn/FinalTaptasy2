using System;

public class UIWIndowDoubleSpeedDialog : UIWindow
{
	public static UIWIndowDoubleSpeedDialog instance;

	private Action m_okAction;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openDoubleSpeedDialogUI(Action okAction)
	{
		Singleton<GameManager>.instance.Pause(true, false);
		m_okAction = okAction;
		open();
	}

	public void OnClickOK()
	{
		Singleton<AdsManager>.instance.showAds("doubleSpeedBossDungeon", delegate
		{
			m_okAction();
			close();
		});
	}

	public override bool OnBeforeClose()
	{
		Singleton<GameManager>.instance.Pause(false);
		return base.OnBeforeClose();
	}

	public void OnClickClose()
	{
		Singleton<BossRaidManager>.instance.isAlreadySeenUI = false;
		m_okAction = null;
		close();
	}
}
