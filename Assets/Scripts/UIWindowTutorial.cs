using UnityEngine;

public class UIWindowTutorial : UIWindow
{
	public static UIWindowTutorial instance;

	public RectTransform topObject;

	public static float offset
	{
		get
		{
			return 640f - instance.topObject.localPosition.y;
		}
	}

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}
}
