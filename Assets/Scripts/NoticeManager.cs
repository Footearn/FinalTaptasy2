using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeManager : Singleton<NoticeManager>
{
	public class NoticeData
	{
		public string noticeID;

		public Sprite noticeSprite;
	}

	public List<NoticeData> noticeList = new List<NoticeData>();

	public List<NoticeData> noticeOriginList = new List<NoticeData>();

	public NoticeData promotionNotice;

	public NoticeData currentNotice;

	public bool isLoadingNotices;

	private int m_noticeLoadingTotalCount;

	public void loadAllNotice(List<string> noticeIDList)
	{
		m_noticeLoadingTotalCount = 0;
		for (int i = 0; i < noticeIDList.Count; i++)
		{
			if (isCanShowNotice(noticeIDList[i]))
			{
				m_noticeLoadingTotalCount++;
				isLoadingNotices = true;
				StartCoroutine(loadNotice(noticeIDList[i]));
			}
		}
		if (m_noticeLoadingTotalCount == 0)
		{
			isLoadingNotices = false;
		}
	}

	private IEnumerator loadNotice(string noticeID)
	{
		string url = string.Format("http://under2.nsouls.com/{0}.png", noticeID);
		WWW www = new WWW(url);
		yield return www;
		Texture noticeTexture = www.texture;
		Sprite noticeSprite = Sprite.Create(www.texture, new Rect(0f, 0f, www.texture.width, www.texture.height), Vector2.zero);
		addNotice(noticeID, noticeSprite);
	}

	private void addNotice(string noticeID, Sprite sprite)
	{
		NoticeData noticeData = new NoticeData();
		noticeData.noticeID = noticeID;
		noticeData.noticeSprite = sprite;
		if (noticeID.Contains("promotion"))
		{
			promotionNotice = noticeData;
		}
		else
		{
			noticeList.Add(noticeData);
			noticeOriginList.Add(noticeData);
		}
		m_noticeLoadingTotalCount--;
		if (m_noticeLoadingTotalCount == 0)
		{
			isLoadingNotices = false;
		}
	}

	public bool isCanShowNotice(string noticeID)
	{
		return NSPlayerPrefs.GetInt(noticeID, 0) != UnbiasedTime.Instance.Now().DayOfYear;
	}

	public void waitNextNotice()
	{
		StartCoroutine("waitForClose");
	}

	private IEnumerator waitForClose()
	{
		yield return new WaitForSeconds(0.15f);
		UIWindowServerNotice.instance.openAllServerNotice();
	}
}
