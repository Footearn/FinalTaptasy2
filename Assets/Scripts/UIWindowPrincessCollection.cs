using UnityEngine;
using UnityEngine.UI;

public class UIWindowPrincessCollection : UIWindow
{
	public static UIWindowPrincessCollection instance;

	public InfiniteScroll princessScroll;

	public Text totalRescueCountDescriptionText;

	public Text totalBonusDamageText;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		if (UIWindowOutgame.instance.princessCollectionIndicator.activeSelf)
		{
			UIWindowOutgame.instance.princessCollectionIndicator.SetActive(false);
		}
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
		return base.OnBeforeOpen();
	}

	public override void OnAfterClose()
	{
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
		base.OnAfterClose();
	}

	public void openPrincessCollection()
	{
		princessScroll.refreshAll();
		totalRescueCountDescriptionText.text = string.Format(I18NManager.Get("TOTAL_RESCUE_COUNT_DESCRIPTION"), Mathf.Max(GameManager.getCurrentPrincessNumber() - 1, 0));
		totalBonusDamageText.text = string.Format(I18NManager.Get("TOTAL_BONUS_PERCENT_DAMAGE"), (GameManager.getCurrentPrincessNumber() - 1) * 100);
		princessScroll.resetContentPosition(Vector2.zero);
		princessScroll.resetContentPosition(new Vector2(0f - (((princessScroll.ItemScale != -1f) ? princessScroll.ItemScale : 290f) * (float)GameManager.getCurrentPrincessNumber() - 335f), 0f));
		open();
	}
}
