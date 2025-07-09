using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowServerNotice : UIWindow
{
	public static UIWindowServerNotice instance;

	public Image noticeImage;

	public GameObject checkObject;

	public bool isOpend;

	public GameObject dontShowCheckObject;

	private bool m_isCheckingDontShowToday;

	public override void Awake()
	{
		instance = this;
		ignoreOnCloseAll = true;
		base.Awake();
	}

	public override bool OnBeforeClose()
	{
		Singleton<NoticeManager>.instance.waitNextNotice();
		return base.OnBeforeClose();
	}

	private void openNextNotice()
	{
		dontShowCheckObject.SetActive(true);
		Singleton<NoticeManager>.instance.currentNotice = Singleton<NoticeManager>.instance.noticeList[0];
		noticeImage.sprite = Singleton<NoticeManager>.instance.currentNotice.noticeSprite;
		noticeImage.SetNativeSize();
		setDontShowToday(false);
		Singleton<NoticeManager>.instance.noticeList.Remove(Singleton<NoticeManager>.instance.noticeList[0]);
		open();
	}

	public void openPromotionNotice()
	{
		if (Singleton<NoticeManager>.instance.promotionNotice != null)
		{
			dontShowCheckObject.SetActive(false);
			Singleton<NoticeManager>.instance.currentNotice = Singleton<NoticeManager>.instance.promotionNotice;
			noticeImage.sprite = Singleton<NoticeManager>.instance.currentNotice.noticeSprite;
			noticeImage.SetNativeSize();
			setDontShowToday(false);
			open();
		}
	}

	public bool openAllServerNotice()
	{
		if (!Singleton<NoticeManager>.instance.isLoadingNotices)
		{
			if (Singleton<NoticeManager>.instance.noticeList.Count > 0)
			{
				openNextNotice();
				return true;
			}
		}
		else
		{
			StopCoroutine("waitOpenNotice");
			StartCoroutine("waitOpenNotice");
		}
		return false;
	}

	private IEnumerator waitOpenNotice()
	{
		while (true)
		{
			if (GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				UIWindowLoading.instance.openLoadingUI();
			}
			else
			{
				UIWindowLoading.instance.closeLoadingUI();
			}
			if (!Singleton<NoticeManager>.instance.isLoadingNotices)
			{
				break;
			}
			yield return null;
		}
		openNextNotice();
	}

	private void setDontShowToday(bool state)
	{
		m_isCheckingDontShowToday = state;
		checkObject.SetActive(m_isCheckingDontShowToday);
		if (m_isCheckingDontShowToday)
		{
			NSPlayerPrefs.SetInt(Singleton<NoticeManager>.instance.currentNotice.noticeID, UnbiasedTime.Instance.Now().DayOfYear);
		}
		else if (NSPlayerPrefs.HasKey(Singleton<NoticeManager>.instance.currentNotice.noticeID))
		{
			NSPlayerPrefs.DeleteKey(Singleton<NoticeManager>.instance.currentNotice.noticeID);
		}
	}

	public void OnClickDontShowToday()
	{
		setDontShowToday(!m_isCheckingDontShowToday);
	}
}
