using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowCollectEvent : UIWindow
{
	public static UIWindowCollectEvent instance;

	public CanvasGroup cachedCanvasGroup;

	public Text collectEventResourceCountText;

	public Text eventDateText;

	public List<CollectEventItemSlot> totalSlotObject;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
		eventDateText.text = string.Format(I18NManager.Get("COLLECT_EVENT_DATE"), Singleton<CollectEventManager>.instance.collectEventEndTime.Year, Singleton<CollectEventManager>.instance.collectEventEndTime.Month, Singleton<CollectEventManager>.instance.collectEventEndTime.Day);
		collectEventResourceCountText.text = Singleton<DataManager>.instance.currentGameData.collectEventResource.ToString("N0");
		refreshAllSlot();
		return base.OnBeforeOpen();
	}

	public override bool OnBeforeClose()
	{
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
		return base.OnBeforeClose();
	}

	public void refreshAllSlot()
	{
		for (int i = 0; i < totalSlotObject.Count; i++)
		{
			totalSlotObject[i].initCollectEventSlot();
		}
	}
}
