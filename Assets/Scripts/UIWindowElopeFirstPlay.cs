public class UIWindowElopeFirstPlay : UIWindow
{
	public static UIWindowElopeFirstPlay instance;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override void OnAfterOpen()
	{
		Singleton<AudioManager>.instance.stopBackgroundSound();
		Singleton<AudioManager>.instance.playEffectSound("result_fail");
		base.OnAfterOpen();
	}

	public void OnClickStartElope()
	{
		Singleton<ElopeModeManager>.instance.startElopeMode();
		Singleton<CachedManager>.instance.coverUI.fadeIn(1f);
		close();
	}
}
