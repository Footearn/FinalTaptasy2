using System.Collections.Generic;
using UnityEngine.UI;

public class UIWindowNotice : UIWindow
{
	public static UIWindowNotice instance;

	public List<NoticeSlot> currentSlotObjects;

	public Image imageEventBanner;

	public bool firstTimeOpen;

	public I18NManager.Language prevLanguage;

	private List<NanooAPIManager.NoticeItem> m_currentNoticeList;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openNoticeUI(List<NanooAPIManager.NoticeItem> noticeList, bool forceOpen = false)
	{
		prevLanguage = I18NManager.currentLanguage;
		if (!firstTimeOpen || forceOpen)
		{
			firstTimeOpen = true;
			m_currentNoticeList = noticeList;
			for (int i = 0; i < currentSlotObjects.Count; i++)
			{
				currentSlotObjects[i].resetSlot();
			}
			for (int j = 0; j < m_currentNoticeList.Count; j++)
			{
				currentSlotObjects[j].initNoticeSlot(m_currentNoticeList[j]);
			}
			if (Singleton<NanooAPIManager>.instance.isEventBannerComplete && Singleton<NanooAPIManager>.instance.eventBannerSprite != null)
			{
				imageEventBanner.sprite = Singleton<NanooAPIManager>.instance.eventBannerSprite;
			}
			open();
		}
	}

	public void OnClickBanner()
	{
		if (!string.IsNullOrEmpty(Singleton<NanooAPIManager>.instance.eventBannerUrl))
		{
			Singleton<NanooAPIManager>.instance.OpenForumView(Singleton<NanooAPIManager>.instance.eventBannerUrl);
		}
	}
}
