using System;
using System.Collections;
using UnityEngine;

public class UIWindowPostBox : UIWindow
{
	public static UIWindowPostBox instance;

	public GameObject emptyItemObject;

	public GameObject scrollGameObject;

	public InfiniteScroll infiniteScroll;

	public ItemControllerLimited itemControllerLimited;

	private DateTime postboxCheckTime;

	private new void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openPostBoxUI()
	{
		if (Util.isInternetConnection())
		{
			if (!Social.localUser.authenticated)
			{
				UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("REQUIRE_LOGIN"), I18NManager.Get("GOOGLE_PLAY")), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
				return;
			}
			if (!Singleton<NanooAPIManager>.instance.isPostInitComplete)
			{
				UIWindowDialog.openDescription("NOT_READY_POST_BOX", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
				return;
			}
			open();
			UIWindowLoading.instance.openLoadingUI();
			Singleton<NanooAPIManager>.instance.CallAPI(NanooAPIManager.APIType.POSTBOX_ITEM);
			StartCoroutine("WaitForCompletePostboxItem");
		}
		else
		{
			UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	private IEnumerator WaitForCompletePostboxItem()
	{
		postboxCheckTime = UnbiasedTime.Instance.Now();
		while (!Singleton<NanooAPIManager>.instance.isPostItemComplete && UnbiasedTime.Instance.Now().Subtract(postboxCheckTime).TotalSeconds < 10.0)
		{
			yield return null;
		}
		UIWindowLoading.instance.closeLoadingUI();
		if (Singleton<NanooAPIManager>.instance.isPostItemComplete)
		{
			initPostItemSlot();
		}
		else
		{
			UIWindowDialog.openDescriptionNotUsingI18N("Time out!", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void initPostItemSlot(bool refresh = false)
	{
		int postboxItemCount = Singleton<PostBoxManager>.instance.GetPostboxItemCount();
		if (postboxItemCount > 0)
		{
			infiniteScroll.refreshMaxCount(postboxItemCount);
			infiniteScroll.syncAllSlotIndexFromPosition();
			emptyItemObject.SetActive(false);
			scrollGameObject.SetActive(true);
		}
		else
		{
			emptyItemObject.SetActive(true);
			scrollGameObject.SetActive(false);
		}
		if (refresh)
		{
			infiniteScroll.rearrangeSlots();
			infiniteScroll.refreshAll();
			infiniteScroll.syncAllSlotIndexFromPosition();
		}
		UIWindowOutgame.instance.refreshPostboxCount(postboxItemCount);
	}

	public void OnClickCopyPostBoxID()
	{
		//NativePluginManager.Instance.SetClipboardString("UserID", Singleton<NanooAPIManager>.instance.PostBoxID);
		AndroidToast.ShowToastNotification(I18NManager.Get("INBOXID_COPIED"));
	}
}
