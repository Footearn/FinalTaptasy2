using UnityEngine;
using UnityEngine.UI;

public class NoticeSlot : ObjectBase
{
	public Text titleNameText;

	public Text descriptionText;

	public GameObject arrowObject;

	private string targetUrl;

	private NanooAPIManager.NoticeItem m_currentNoticeItem;

	public void resetSlot()
	{
		base.cachedGameObject.SetActive(false);
		titleNameText.text = string.Empty;
		descriptionText.text = string.Empty;
		targetUrl = string.Empty;
	}

	public void initNoticeSlot(NanooAPIManager.NoticeItem noticeItem)
	{
		m_currentNoticeItem = noticeItem;
		base.cachedGameObject.SetActive(true);
		titleNameText.text = m_currentNoticeItem.header;
		descriptionText.text = m_currentNoticeItem.content;
		targetUrl = m_currentNoticeItem.url;
	}

	public void OnClickOpenURL()
	{
		Singleton<NanooAPIManager>.instance.OpenForumView(targetUrl);
	}
}
