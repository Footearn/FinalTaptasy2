using UnityEngine;
using UnityEngine.UI;

public class UIWindowRebirthResult : UIWindow
{
	public Text keyResult;

	public GameObject sunburst;

	public void SetResult(long keyCount)
	{
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		open();
		keyResult.text = string.Format(I18NManager.Get("REBIRTH_RESULT_KEY"), keyCount);
		sunburst.SetActive(true);
		UIWindowManageTreasure.instance.createTreasureSlots();
	}

	public override bool OnBeforeClose()
	{
		sunburst.SetActive(false);
		return base.OnBeforeClose();
	}
}
