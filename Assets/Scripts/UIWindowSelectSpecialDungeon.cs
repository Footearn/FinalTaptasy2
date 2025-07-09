using UnityEngine;

public class UIWindowSelectSpecialDungeon : UIWindow
{
	public static UIWindowSelectSpecialDungeon instance;

	public GameObject elopeModeStartButtonObject;

	public GameObject elopeModeLockedObject;

	public RectTransform content;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		refreshUI();
		return base.OnBeforeOpen();
	}

	private void refreshUI()
	{
		elopeModeStartButtonObject.SetActive(Singleton<ElopeModeManager>.instance.isCanStartElopeMode());
		elopeModeLockedObject.SetActive(!Singleton<ElopeModeManager>.instance.isCanStartElopeMode());
	}

	public void OnClickElopeMode()
	{
		UIWindowOutgame.instance.OnClickEnterElopeMode();
	}

	public void OnClickTowerMode()
	{
		UIWindowSelectTowerModeDifficulty.instance.openSelectTowerModeDifficulty(false);
	}
}
