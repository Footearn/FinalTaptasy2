public class UIWindowTranscendInformation : UIWindow
{
	public static UIWindowTranscendInformation instance;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}
}
