using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fabric.Answers;
using Fabric.Crashlytics;
using PlayNANOO;
using SimpleJSON;
using UnityEngine;

public class NanooAPIManager : Singleton<NanooAPIManager>
{
	[Serializable]
	public class NoticeItem
	{
		public string header;

		public string content;

		public string url;
	}

	public enum APIType
	{
		PROMOTION,
		ADID_RECORD,
		SAVE_VALIDATION,
		LOAD_VALIDATION,
		CONTROL_FINALTAPTASY,
		RECEIPT_VALIDATION,
		POSTBOX_INIT,
		POSTBOX_ITEM,
		POSTBOX_RECEIVE,
		POSTBOX_GIVE,
		COUPON_CHECK,
		NOTICE,
		EVENTBANNER,
		SUBSCRIPTION_SAVE,
		TotalAPICount
	}

	[Serializable]
	public class PromotionRequestResponse
	{
		public string code;

		public string package_name;

		public string url_scheme;

		public string image;

		public string image_width;

		public string image_height;

		public string url;
	}

	[Serializable]
	public class AdidRecordResponse
	{
		public string result;
	}

	[Serializable]
	public class PostboxInitResponse
	{
		[Serializable]
		public class SubscriptionData
		{
			public string item_code;

			public long expired_sec;
		}

		public string postbox_id;

		public string postbox_item_count;

		public List<SubscriptionData> subscription_item;
	}

	[Serializable]
	public class PostItem
	{
		public string item_id;

		public string message_code;

		public string item_code;

		public string item_count;

		public double expired_sec;

		public string post_date;
	}

	[Serializable]
	public class PostboxItemResponse
	{
		public List<PostItem> data;
	}

	[Serializable]
	public class PostboxReceiveResponse
	{
		public string result;
	}

	[Serializable]
	public class PostboxGiveResponse
	{
		public string result;
	}

	[Serializable]
	public class ReceiptValidatorResponse
	{
		public string result;

		public string product_id;

		public long timestamp;

		public string hash;
	}

	[Serializable]
	public class ControlResponse
	{
		[Serializable]
		public class AdsType
		{
			[Serializable]
			public class Ad
			{
				public bool use;

				public string percent;
			}

			public Ad applovin;

			public Ad tapjoy;

			public Ad unityads;
		}

		[Serializable]
		public class Promotion
		{
			public bool use;

			public string percent;
		}

		public bool service;

		public bool update;

		public AdsType ad;

		public Promotion crosspromotion;

		public TowerModeManager.SeasonResponseData demongtower_season;
	}

	[Serializable]
	public class NoticeResponse
	{
		[Serializable]
		public class Notice
		{
			[Serializable]
			public class NoticeImage
			{
				public string url;

				public string width;

				public string height;
			}

			public string title;

			public string description;

			public NoticeImage[] attach_image;

			public long post_timestamp;

			public string link;
		}

		public int count;

		public Notice[] data;
	}

	[Serializable]
	public class EventBanner
	{
		public string banner;

		public long start_timestamp;

		public long finish_timestamp;
	}

	[Serializable]
	public class EventBannerResponse
	{
		[Serializable]
		public class Event
		{
			public string title;

			public string description;

			public EventBanner @event;

			public string link;
		}

		public int count;

		public Event[] data;
	}

	[Serializable]
	public class CloudSaveResponse
	{
		public string result;
	}

	[Serializable]
	public class CloudLoadResponse
	{
		[Serializable]
		public class CloudLoadData
		{
			public string version;

			public string game_data;
		}

		public string result;

		public CloudLoadData data;
	}

	[Serializable]
	public class CouponCheckResponse
	{
		public string result;

		public string item;

		public long timestamp;

		public string hash;
	}

	[Serializable]
	public class ErrorResponse
	{
		[Serializable]
		public class Error
		{
			public int code;

			public string message;
		}

		public Error error;
	}

	[Serializable]
	public class SubscriptionSaveResponse
	{
		public string result;

		public string expired_sec;
	}

	public Plugin playNANOO;

	public PlayNANOO.EventHandler playNANOOHandler;

	private bool hasPlayNANOO;

	private bool initPlayNANOO;

	private static readonly string nanooURL = "https://api.playnanoo.com";

	private static readonly string nanooGameID = "gVUwx1KYB4UZiZ2";

	private static readonly string nanooSecretKey = "HYaCBOBdb3RrzF2oJO3HrhSDVdcKbFOL";

	private string platform;

	private string orientation;

	private string call;

	[NonSerialized]
	public string udid = string.Empty;

	[NonSerialized]
	public static string uuid = string.Empty;

	[NonSerialized]
	public string adid = string.Empty;

	private string postboxID = string.Empty;

	[NonSerialized]
	public int postboxItemCount = -1;

	[NonSerialized]
	public string[] suffixURL = new string[14]
	{
		nanooURL + "/v20160401/promotion/request",
		nanooURL + "/v20160401/adid/record",
		nanooURL + "/finaltaptasy/v1/cloud/save",
		nanooURL + "/finaltaptasy/v1/cloud/load",
		nanooURL + "/finaltaptasy/v1/init",
		nanooURL + "/v20160401/purchase/receipt_validator_android",
		nanooURL + "/v20160401/postbox/init",
		nanooURL + "/v20160401/postbox/item",
		nanooURL + "/v20160401/postbox/receive",
		nanooURL + "/v20160401/postbox/give",
		nanooURL + "/v20160401/coupon/check",
		"https://api.nanoo.so/v2/content",
		"https://api.nanoo.so/v2/content",
		nanooURL + "/v20160401/subscription/save"
	};

	private Queue _queueAPICall;

	[NonSerialized]
	public bool isReceiptValidatorComplete;

	private ReceiptValidatorResponse _receiptValidatorResponse = new ReceiptValidatorResponse();

	[NonSerialized]
	public bool isControlReceiveComplete;

	private ControlResponse _controlResponse = new ControlResponse();

	[NonSerialized]
	public bool isPromotionRequestComplete;

	[NonSerialized]
	public Sprite promotionSprite;

	private PromotionRequestResponse _promotionRequestResponse = new PromotionRequestResponse();

	[NonSerialized]
	public bool isNoticeComplete;

	public List<NoticeItem> _noticeList = new List<NoticeItem>();

	private NoticeResponse _noticeResponse = new NoticeResponse();

	[NonSerialized]
	public Sprite eventBannerSprite;

	[NonSerialized]
	public string eventBannerUrl = string.Empty;

	[NonSerialized]
	public bool isEventBannerComplete;

	public EventBanner eventBanner = new EventBanner();

	private EventBannerResponse _eventBannerResponse = new EventBannerResponse();

	[NonSerialized]
	public bool isPostInitComplete;

	private PostboxInitResponse _postInitResponse = new PostboxInitResponse();

	[NonSerialized]
	public bool isPostItemComplete;

	private List<PostItem> _postItemList = new List<PostItem>();

	private PostboxItemResponse _postItemResponse = new PostboxItemResponse();

	[NonSerialized]
	public bool isPostItemReceiveSuccess;

	[NonSerialized]
	public bool isPostItemReceiveComplete;

	[NonSerialized]
	public bool isCloudSaveComplete;

	private CloudSaveResponse _cloudSaveResponse = new CloudSaveResponse();

	[NonSerialized]
	public bool isCloudLoadComplete;

	private CloudLoadResponse _cloudLoadResponse = new CloudLoadResponse();

	[NonSerialized]
	public bool isCouponCheckComplete;

	private CouponCheckResponse _couponCheckResponse = new CouponCheckResponse();

	private ErrorResponse _errorResponse = new ErrorResponse();

	private SubscriptionSaveResponse _subscriptionSaveResponse;

	private long m_vipItemRemainSeconds;

	private bool m_isExistSubscriptionItemWhenPurchaseVIP;

	private AndroidJavaClass DeviceIdentifier;

	public string PostBoxID
	{
		get
		{
			return postboxID;
		}
	}

	public string userName
	{
		get;
		set;
	}

	public string ObjectID
	{
		get
		{
			return uuid;
		}
	}

	public string UserID
	{
		get
		{
			return uuid;
		}
	}

	public bool isServiceOn
	{
		get
		{
			if (!isControlReceiveComplete)
			{
				return false;
			}
			return _controlResponse.service;
		}
	}

	public bool isPromotionOn
	{
		get
		{
			if (!isControlReceiveComplete)
			{
				return false;
			}
			return _controlResponse.crosspromotion.use;
		}
	}

	public bool isForceUpdate
	{
		get
		{
			if (!isControlReceiveComplete)
			{
				return false;
			}
			return _controlResponse.update;
		}
	}

	public TowerModeManager.SeasonResponseData towerModeSeasonData
	{
		get
		{
			return _controlResponse.demongtower_season;
		}
		set
		{
			_controlResponse.demongtower_season = value;
		}
	}

	public PromotionRequestResponse GetPromotionInfo
	{
		get
		{
			if (!isPromotionRequestComplete)
			{
				return null;
			}
			return _promotionRequestResponse;
		}
	}

	public List<NoticeItem> getNoticeInfo
	{
		get
		{
			if (!isNoticeComplete)
			{
				return null;
			}
			return _noticeList;
		}
	}

	public int getPostboxCount
	{
		get
		{
			if (!isPostInitComplete)
			{
				return 0;
			}
			return int.Parse(_postInitResponse.postbox_item_count);
		}
	}

	public List<PostItem> getPostItemList
	{
		get
		{
			if (!isPostItemComplete)
			{
				return null;
			}
			return _postItemList;
		}
	}

	public string getErrorMessage
	{
		get
		{
			if (_errorResponse == null)
			{
				return "Unknown Error";
			}
			return _errorResponse.error.message;
		}
	}

	public static string getNanooURL()
	{
		return nanooURL;
	}

	public static string getNanooGameID()
	{
		return nanooGameID;
	}

	public static string getNanooSecretKey()
	{
		return nanooSecretKey;
	}

	private bool IsQueueAPICallEmpty()
	{
		if (_queueAPICall.Count <= 0)
		{
			return true;
		}
		return false;
	}

	private void AddAPICall(APIType api)
	{
		if (!_queueAPICall.Contains(api))
		{
			bool flag = false;
			if (IsQueueAPICallEmpty())
			{
				flag = true;
			}
			_queueAPICall.Enqueue(api);
			if (flag)
			{
				StopCoroutine("updateForAPICall");
				StartCoroutine("updateForAPICall");
			}
		}
	}

	private string PopAPICall()
	{
		if (IsQueueAPICallEmpty())
		{
			return _queueAPICall.Dequeue() as string;
		}
		return null;
	}

	private IEnumerator updateForAPICall()
	{
		while (!IsQueueAPICallEmpty())
		{
			yield return null;
		}
	}

	public bool IsExistSubscriptionItem()
	{
		if (m_vipItemRemainSeconds > 0)
		{
			return true;
		}
		return false;
	}

	public int GetVipItemCount()
	{
		int result = 0;
		if (m_vipItemRemainSeconds > 0)
		{
			result = (int)new TimeSpan(new DateTime(TimeSpan.FromSeconds(m_vipItemRemainSeconds).Ticks).Ticks).TotalDays;
		}
		return result;
	}

	private void Awake()
	{
        platform = "ANDROID";
        orientation = "VERTICAL";
        udid = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Device UDID (from Unity): " + udid);

        // Kiểm tra các component cần thiết
        if (playNANOO == null)
        {
            Debug.LogError("Add nanoo plugin component in " + base.gameObject.name);
        }
        if (playNANOOHandler == null)
        {
            Debug.LogError("Add nanoo eventHandler component in " + base.gameObject.name);
        }
        hasPlayNANOO = playNANOO != null && playNANOOHandler != null;
        playNANOO.SetInfo(uuid, userName, GlobalSetting.s_bundleVersionCode);
		//playNANOO.Init(true, delegate(Dictionary<string, object> dic)
		//{
		//	if (dic["status"].Equals(ResultType.SUCCESS))
		//	{
		//		Debug.Log(string.Format("token=>{0}", dic["token"].ToString()));
		//		Debug.Log(string.Format("time=>{0}", dic["time"].ToString()));
		//		if (dic.ContainsKey("postbox_count"))
		//		{
		//			Debug.Log(string.Format("postbox_count=>{0}", dic["postbox_count"].ToString()));
		//		}
		//		if (dic.ContainsKey("postbox_subscription"))
		//		{
		//			ArrayList arrayList = (ArrayList)dic["postbox_subscription"];
		//			if (arrayList.Count > 0)
		//			{
		//				foreach (Dictionary<string, object> item in arrayList)
		//				{
		//					Debug.Log(string.Format("product=>{0}", item["product"].ToString()));
		//					Debug.Log(string.Format("ttl=>{0}", item["ttl"].ToString()));
		//				}
		//			}
		//		}
		//		initPlayNANOO = true;
		//	}
		//	else if (dic["status"].Equals(ResultType.UPDATE))
		//	{
		//		Debug.Log(string.Format("message=>{0}", dic["message"]));
		//	}
		//	else if (dic["status"].Equals(ResultType.ERROR))
		//	{
		//		Debug.Log(string.Format("message=>{0}", dic["message"]));
		//	}
		//	else if (dic["status"].Equals(ResultType.INTERNET))
		//	{
		//		Debug.Log(string.Format("message=>{0}", dic["message"]));
		//	}
		//	if (initPlayNANOO)
		//	{
		//		ForumThread();
		//	}
		//	else
		//	{
		//		isNoticeComplete = true;
		//	}
		//});
	}

	private void Start()
	{
		refreshUserID();
		//if (Util.isInternetConnection())
		//{
		//	CallAPI(APIType.CONTROL_FINALTAPTASY);
		//	CallAPI(APIType.EVENTBANNER);
		//}
	}

	public void refreshUserID()
	{
		if (Social.localUser.authenticated)
		{
			InitPostBox();
			return;
		}
		uuid = string.Empty;
		userName = string.Empty;
		Social.localUser.Authenticate(delegate(bool success)
		{
			if (success)
			{
				InitPostBox();
				if (UIWindowSetting.instance != null && UIWindowSetting.instance.isOpen)
				{
					UIWindowSetting.instance.refreshSetting();
				}
			}
		});
	}

	public void InitPostBox()
	{
		postboxID = string.Empty;
		string input = Social.localUser.id.Trim().ToLower();
		input = Regex.Replace(input, "[^0-9]+", string.Empty);
		if (Regex.IsMatch(input, "^\\d+$"))
		{
			SetUUID(input);

            // BÂY GIỜ, SAU KHI CÓ UUID, CHÚNG TA SẼ GỌI HÀM INIT CỦA SDK MỚI
            // THAY VÌ GỌI API CŨ
            userName = Social.localUser.userName; // Lấy userName ở đây
            playNANOO.SetInfo(uuid, userName, GlobalSetting.s_bundleVersionCode);
            playNANOO.Init(true, delegate (Dictionary<string, object> dic)
            {
                // Toàn bộ logic xử lý kết quả từ server sẽ nằm ở đây
                if (dic["status"].Equals(ResultType.SUCCESS))
                {
                    Debug.Log("NANOO INIT THÀNH CÔNG (từ InitPostBox)!");
                    initPlayNANOO = true;
                    // ... xử lý dữ liệu trả về nếu cần ...
                }
                else
                {
                    Debug.LogError("Nanoo Init thất bại: " + dic["message"]);
                }
            });
        }
		else
		{
            userName = Social.localUser.userName;
            Singleton<LogglyManager>.instance.SendLoggly("UUID value: " + input + ", UserName: " + userName, "Fail to validate regular expression", LogType.Error);
        }
  //      if (Util.isInternetConnection())
		//{
		//	CallAPI(APIType.POSTBOX_INIT);
		//}
	}

	public IEnumerable<string> SplitInParts(string s, int partLength)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (partLength <= 0)
		{
			throw new ArgumentException("Part length has to be positive.", "partLength");
		}
		for (int i = 0; i < s.Length; i += partLength)
		{
			yield return s.Substring(i, Math.Min(partLength, s.Length - i));
		}
	}

	public void ClearPostAPI()
	{
		postboxID = string.Empty;
		UIWindowOutgame.instance.refreshPostboxCount(0);
	}

	public void CallAPI(APIType apiType, params string[] args)
	{
		if (!Util.isInternetConnection())
		{
			if (UIWindowLoading.instance != null)
			{
				UIWindowLoading.instance.closeLoadingUI();
			}
			UIWindowDialog.openDescription("INTERNET_DISCONNECTED", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			return;
		}
		string iSO = GetISO639();
		WWW wWW = null;
		WWWForm wWWForm = new WWWForm();
		long num = 0L;
		char c = Convert.ToChar(Convert.ToInt32(UnityEngine.Random.Range(97, 123)));
		call = c + UnityEngine.Random.Range(0, 99999).ToString();
		string empty = string.Empty;
		string empty2 = string.Empty;
		string text = suffixURL[(int)apiType];
		switch (apiType)
		{
		default:
			return;
		case APIType.CONTROL_FINALTAPTASY:
			empty = "?client_id=" + nanooGameID + "&version=" + GlobalSetting.s_bundleVersion + "&platform=" + platform;
			wWW = new WWW(text + empty);
			isControlReceiveComplete = false;
			break;
		case APIType.PROMOTION:
			empty = "?client_id=" + nanooGameID + "&platform=" + platform + "&orientation=" + orientation + "&language=" + iSO;
			wWW = new WWW(text + empty);
			isPromotionRequestComplete = false;
			break;
		case APIType.POSTBOX_INIT:
			if (uuid.Length <= 0)
			{
				return;
			}
			empty2 = Util.SHA256Token(nanooGameID + uuid + call, nanooSecretKey);
			empty2 = Uri.EscapeDataString(empty2);
			empty = "?client_id=" + nanooGameID + "&uuid=" + uuid + "&platform=" + platform + "&subscription=Y&version=" + GlobalSetting.s_bundleVersion + "&call=" + call + "&hash=" + empty2;
			wWW = new WWW(text + empty);
			isPostInitComplete = false;
			break;
		case APIType.POSTBOX_ITEM:
			empty2 = Util.SHA256Token(nanooGameID + postboxID + call, nanooSecretKey);
			empty2 = Uri.EscapeDataString(empty2);
			empty = "?client_id=" + nanooGameID + "&postbox_id=" + postboxID + "&call=" + call + "&hash=" + empty2;
			wWW = new WWW(text + empty);
			isPostItemComplete = false;
			break;
		case APIType.NOTICE:
			empty = "?key=" + GetNanooGameChannelID() + "&code=" + GetNoticeCode() + "&order=LATEST&limit=3";
			wWW = new WWW(text + empty);
			isNoticeComplete = false;
			break;
		case APIType.EVENTBANNER:
			empty = "?key=" + GetNanooGameChannelID() + "&code=" + GetEventCode() + "&order=LATEST&limit=1";
			wWW = new WWW(text + empty);
			isEventBannerComplete = false;
			break;
		case APIType.ADID_RECORD:
			if (args.Length < 1)
			{
				return;
			}
			wWWForm.AddField("client_id", nanooGameID);
			wWWForm.AddField("id", adid);
			wWWForm.AddField("platform", platform);
			wWW = new WWW(text, wWWForm);
			break;
		case APIType.RECEIPT_VALIDATION:
			if (args.Length < 2)
			{
				return;
			}
			wWWForm.AddField("client_id", nanooGameID);
			wWWForm.AddField("purchase", args[0]);
			wWWForm.AddField("signature", args[1]);
			Singleton<LogglyManager>.instance.SendLoggly("UserID : " + Singleton<NanooAPIManager>.instance.UserID + ", Purchase: " + args[0], "Server Receipt Check", LogType.Log);
			wWW = new WWW(text, wWWForm);
			isReceiptValidatorComplete = false;
			break;
		case APIType.LOAD_VALIDATION:
			if (uuid.Length <= 0)
			{
				return;
			}
			wWWForm.AddField("client_id", nanooGameID);
			wWWForm.AddField("uuid", uuid);
			wWWForm.AddField("udid", udid);
			num = Util.UnixTimestampFromDateTime(UnbiasedTime.Instance.Now());
			wWWForm.AddField("ts", num.ToString());
			empty2 = Util.SHA256Token(nanooGameID + uuid + udid + num, nanooSecretKey);
			wWWForm.AddField("hash", empty2);
			UIWindowLoading.instance.openLoadingUI();
			wWW = new WWW(text, wWWForm);
			isCloudLoadComplete = false;
			break;
		case APIType.SAVE_VALIDATION:
			if (uuid.Length <= 0 || args.Length < 1)
			{
				return;
			}
			UnbiasedTime.Instance.UpdateOffsetBetweenInternetTimeAndUnbiasedTime();
			wWWForm.AddField("client_id", nanooGameID);
			wWWForm.AddField("uuid", uuid);
			wWWForm.AddField("udid", udid);
			wWWForm.AddField("version", GlobalSetting.s_bundleVersion);
			wWWForm.AddField("data", args[0]);
			num = Util.UnixTimestampFromDateTime(UnbiasedTime.Instance.Now());
			wWWForm.AddField("ts", num.ToString());
			empty2 = Util.SHA256Token(nanooGameID + uuid + udid + GlobalSetting.s_bundleVersion + num, nanooSecretKey);
			wWWForm.AddField("hash", empty2);
			wWW = new WWW(text, wWWForm);
			isCloudSaveComplete = false;
			break;
		case APIType.COUPON_CHECK:
			if (uuid.Length <= 0 || args.Length < 1)
			{
				return;
			}
			wWWForm.AddField("client_id", nanooGameID);
			wWWForm.AddField("coupon", args[0]);
			wWWForm.AddField("uuid", uuid);
			if (args[0].StartsWith("_"))
			{
				text = "http://ftcoupon.azurewebsites.net/coupon";
			}
			wWW = new WWW(text, wWWForm);
			isCouponCheckComplete = false;
			break;
		case APIType.POSTBOX_RECEIVE:
			if (uuid.Length <= 0 || args.Length < 1)
			{
				return;
			}
			wWWForm.AddField("client_id", nanooGameID);
			wWWForm.AddField("postbox_id", postboxID);
			wWWForm.AddField("item_id", args[0]);
			wWWForm.AddField("uuid", uuid);
			wWWForm.AddField("call", call);
			empty2 = Util.SHA256Token(nanooGameID + postboxID + args[0] + call, nanooSecretKey);
			wWWForm.AddField("hash", empty2);
			wWW = new WWW(text, wWWForm);
			isPostItemReceiveSuccess = false;
			isPostItemReceiveComplete = false;
			break;
		case APIType.POSTBOX_GIVE:
			if (args.Length < 5)
			{
				return;
			}
			wWWForm.AddField("client_id", nanooGameID);
			wWWForm.AddField("postbox_id", postboxID);
			wWWForm.AddField("message_code", args[0]);
			wWWForm.AddField("item_code", args[1]);
			wWWForm.AddField("item_count", args[2]);
			wWWForm.AddField("expired_yn", args[3]);
			wWWForm.AddField("expired_add_day", args[4]);
			wWWForm.AddField("operator", string.Empty);
			wWWForm.AddField("etc", string.Empty);
			wWWForm.AddField("call", call);
			empty2 = Util.SHA256Token(nanooGameID + postboxID + call, nanooSecretKey);
			wWWForm.AddField("hash", empty2);
			wWW = new WWW(text, wWWForm);
			break;
		case APIType.SUBSCRIPTION_SAVE:
		{
			if (args.Length < 2)
			{
				return;
			}
			wWWForm.AddField("client_id", nanooGameID);
			wWWForm.AddField("uuid", uuid);
			wWWForm.AddField("item_code", args[0]);
			wWWForm.AddField("platform", platform);
			string value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			wWWForm.AddField("datetime", value);
			wWWForm.AddField("period", int.Parse(args[1]));
			num = Util.UnixTimestampFromDateTime(DateTime.Now);
			wWWForm.AddField("ts", num.ToString());
			empty2 = Util.SHA256Token(nanooGameID + uuid + args[0] + num, nanooSecretKey);
			wWWForm.AddField("hash", empty2);
			m_isExistSubscriptionItemWhenPurchaseVIP = bool.Parse(args[2]);
			wWW = new WWW(text, wWWForm);
			break;
		}
		}
		if (wWW != null)
		{
			StartCoroutine(WaitForRequest(wWW, apiType));
		}
	}

	public IEnumerator WaitForRequest(WWW www, APIType apiType)
	{
		yield return www;
		if (www.error == null)
		{
			if (www.bytes.Length > 0)
			{
				if (string.IsNullOrEmpty(www.text))
				{
					yield break;
				}
				string response = www.text.Trim();
				switch (apiType)
				{
				case APIType.CONTROL_FINALTAPTASY:
					if (_controlResponse == null)
					{
						_controlResponse = new ControlResponse();
					}
					_controlResponse = JsonUtility.FromJson<ControlResponse>(response);
					if (_controlResponse.ad != null)
					{
						Singleton<AdsManager>.instance.valueUnityAds = (_controlResponse.ad.unityads.use ? int.Parse(_controlResponse.ad.unityads.percent) : 0);
						Singleton<AdsManager>.instance.valueApplovin = (_controlResponse.ad.applovin.use ? int.Parse(_controlResponse.ad.applovin.percent) : 0);
						Singleton<AdsManager>.instance.valueTapjoy = (_controlResponse.ad.tapjoy.use ? int.Parse(_controlResponse.ad.tapjoy.percent) : 0);
					}
					if (_controlResponse.crosspromotion != null && _controlResponse.crosspromotion.use)
					{
						int percentValue = 0;
						int.TryParse(_controlResponse.crosspromotion.percent, out percentValue);
						if (UnityEngine.Random.Range(0, 100) < percentValue)
						{
							_controlResponse.crosspromotion.use = true;
						}
						else
						{
							_controlResponse.crosspromotion.use = false;
						}
					}
					isControlReceiveComplete = true;
					StartCoroutine(waitForLoadIngameObject(delegate
					{
						if (_controlResponse.demongtower_season != null)
						{
							int result = -1;
							int.TryParse(_controlResponse.demongtower_season.id, out result);
							if (result > 0 && Singleton<DataManager>.instance.currentGameData.towerModeLastSeason != result)
							{
								Singleton<DataManager>.instance.currentGameData.towerModeLastSeason = result;
								Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor = 0;
								Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestClearTime = 0f;
								Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor = 0;
								Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestClearTime = 0f;
								Singleton<DataManager>.instance.saveData();
								if (UIWindowSelectTowerModeDifficulty.instance.isOpen)
								{
									UIWindowSelectTowerModeDifficulty.instance.openSelectTowerModeDifficulty(false);
								}
							}
						}
					}));
					if (_controlResponse.update)
					{
						UIWindowDialog.openDescription("FORCE_UPDATE", UIWindowDialog.DialogState.DelegateOKUI, delegate
						{
							Application.Quit();
						}, string.Empty);
					}
					if (_controlResponse.crosspromotion != null && _controlResponse.crosspromotion.use)
					{
						CallAPI(APIType.PROMOTION);
					}
					break;
				case APIType.PROMOTION:
					if (_promotionRequestResponse == null)
					{
						_promotionRequestResponse = new PromotionRequestResponse();
					}
					_promotionRequestResponse = JsonUtility.FromJson<PromotionRequestResponse>(response);
					GetPromotionTexture();
					break;
				case APIType.LOAD_VALIDATION:
					if (_cloudLoadResponse == null)
					{
						_cloudLoadResponse = new CloudLoadResponse();
					}
					_cloudLoadResponse = JsonUtility.FromJson<CloudLoadResponse>(response);
					if (UIWindowLoading.instance != null)
					{
						UIWindowLoading.instance.closeLoadingUI();
					}
					if (!string.IsNullOrEmpty(_cloudLoadResponse.result))
					{
						string resultMsg = string.Empty;
						switch (_cloudLoadResponse.result.ToLower())
						{
						case "success":
						{
							Version versionOfLoadedGameData = new Version(_cloudLoadResponse.data.version);
							Version currentVersion = new Version(GlobalSetting.s_bundleVersion);
							if (versionOfLoadedGameData != null && versionOfLoadedGameData.CompareTo(currentVersion) > 0)
							{
								UIWindowDialog.openDescription("FAIL_LOAD_DATA_VERSION", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
								break;
							}
							PlayerPrefs.SetString("GameData", _cloudLoadResponse.data.game_data);
							if (UIWindowSetting.instance != null)
							{
								UIWindowSetting.instance.close();
							}
							UIWindowDialog.openDescription("CLOUD_LOAD_SUCCESS", UIWindowDialog.DialogState.DelegateOKUI, delegate
							{
								Application.Quit();
							}, string.Empty);
							break;
						}
						case "none":
							resultMsg = I18NManager.Get("CLOUD_LOAD_NONE");
							break;
						case "fail":
							resultMsg = I18NManager.Get("CLOUD_LOAD_FAIL");
							break;
						case "limit":
							resultMsg = I18NManager.Get("CLOUD_LOAD_LIMIT");
							break;
						case "time":
							resultMsg = I18NManager.Get("CLOUD_LOAD_TIME");
							break;
						default:
							resultMsg = I18NManager.Get("CLOUD_LOAD_UNKNOWN_ERROR") + _cloudSaveResponse.result;
							break;
						}
						if (_cloudLoadResponse.result.ToLower() != "success")
						{
							UIWindowDialog.openDescriptionNotUsingI18N(resultMsg, UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						}
					}
					else
					{
						UIWindowDialog.openDescriptionNotUsingI18N(I18NManager.Get("CLOUD_LOAD_UNKNOWN_ERROR"), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
					}
					isCloudLoadComplete = true;
					break;
				case APIType.POSTBOX_INIT:
					if (_postInitResponse == null)
					{
						_postInitResponse = new PostboxInitResponse();
					}
					_postInitResponse = JsonUtility.FromJson<PostboxInitResponse>(response);
					if (!string.IsNullOrEmpty(_postInitResponse.postbox_id) && !string.IsNullOrEmpty(_postInitResponse.postbox_item_count))
					{
						postboxID = _postInitResponse.postbox_id;
						postboxItemCount = int.Parse(_postInitResponse.postbox_item_count);
						if (GameManager.currentGameState == GameManager.GameState.OutGame && UIWindowOutgame.instance != null && UIWindowOutgame.instance.isOpen)
						{
							UIWindowOutgame.instance.refreshPostboxCount(postboxItemCount);
						}
					}
					isPostInitComplete = true;
					if (_postInitResponse.subscription_item != null && _postInitResponse.subscription_item.Count > 0)
					{
						m_vipItemRemainSeconds = _postInitResponse.subscription_item[0].expired_sec;
					}
					else
					{
						m_vipItemRemainSeconds = 0L;
					}
					while (UIWindowOutgame.instance == null)
					{
						yield return null;
					}
					UIWindowOutgame.instance.refreshVIPStatus();
					break;
				case APIType.POSTBOX_ITEM:
					if (_postItemResponse == null)
					{
						_postItemResponse = new PostboxItemResponse();
					}
					_postItemResponse = JsonUtility.FromJson<PostboxItemResponse>("{\"data\":" + response + "}");
					if (Singleton<PostBoxManager>.instance != null && _postItemResponse.data != null)
					{
						Singleton<PostBoxManager>.instance.Clear();
						foreach (PostItem each in _postItemResponse.data)
						{
							Singleton<PostBoxManager>.instance.Add(each.item_id, each.item_code, GetItemTypeByCode(each.item_code), each.item_count, each.expired_sec);
						}
					}
					isPostItemComplete = true;
					break;
				case APIType.NOTICE:
					if (_noticeResponse == null)
					{
						_noticeResponse = new NoticeResponse();
					}
					_noticeResponse = JsonUtility.FromJson<NoticeResponse>(response);
					if (_noticeList == null)
					{
						_noticeList = new List<NoticeItem>();
					}
					_noticeList.Clear();
					if (_noticeResponse.data != null)
					{
						NoticeResponse.Notice[] data = _noticeResponse.data;
						foreach (NoticeResponse.Notice each2 in data)
						{
							string[] lines = each2.description.Split(new string[2]
							{
								"\r\n",
								"\n"
							}, StringSplitOptions.RemoveEmptyEntries);
							string value = string.Empty;
							string[] array = lines;
							foreach (string selected in array)
							{
								if (selected.Trim().Length > 0)
								{
									value = ((selected.Length <= 65) ? selected : (selected.Substring(0, 65) + "..."));
									break;
								}
							}
							NoticeItem item = new NoticeItem
							{
								header = each2.title.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"")
									.Replace("&apos;", "'")
									.Replace("&amp;", "&"),
								content = value,
								url = each2.link
							};
							_noticeList.Add(item);
						}
					}
					isNoticeComplete = true;
					break;
				case APIType.EVENTBANNER:
					if (_eventBannerResponse == null)
					{
						_eventBannerResponse = new EventBannerResponse();
					}
					_eventBannerResponse = JsonUtility.FromJson<EventBannerResponse>(response);
					eventBanner = null;
					if (_eventBannerResponse.data != null)
					{
						EventBannerResponse.Event[] data2 = _eventBannerResponse.data;
						foreach (EventBannerResponse.Event each3 in data2)
						{
							eventBanner = new EventBanner();
							eventBanner.banner = each3.@event.banner;
							eventBanner.start_timestamp = each3.@event.start_timestamp;
							eventBanner.finish_timestamp = each3.@event.finish_timestamp;
						}
						StartCoroutine(DownloadEventBannerTexture());
					}
					break;
				case APIType.RECEIPT_VALIDATION:
				{
					string resultStr = Encoding.UTF8.GetString(www.bytes, 0, www.bytes.Length);
					Singleton<LogglyManager>.instance.SendLoggly("Response ReceiptCheck : " + resultStr, "NanooAPIManager:RECEIPT_VALIDATION", LogType.Log);
					if (_receiptValidatorResponse == null)
					{
						_receiptValidatorResponse = new ReceiptValidatorResponse();
					}
					if (UIWindowLoading.instance != null)
					{
						UIWindowLoading.instance.closeLoadingUI();
					}
					_receiptValidatorResponse = JsonUtility.FromJson<ReceiptValidatorResponse>(response);
					if (!string.IsNullOrEmpty(_receiptValidatorResponse.result))
					{
						switch (_receiptValidatorResponse.result.ToLower())
						{
						case "success":
						{
							string hashStr = Util.SHA256Token(nanooGameID + _receiptValidatorResponse.result + _receiptValidatorResponse.product_id + _receiptValidatorResponse.timestamp, nanooSecretKey);
							if (hashStr.CompareTo(_receiptValidatorResponse.hash) == 0)
							{
								Singleton<PaymentManager>.instance.LogAnswer(true, _receiptValidatorResponse.product_id);
								Singleton<PaymentManager>.instance.ProcessingPurchase(_receiptValidatorResponse.product_id);
							}
							else
							{
								Singleton<PaymentManager>.instance.LogAnswer(false, _receiptValidatorResponse.product_id);
								Singleton<LogglyManager>.instance.SendLoggly("ReceiptCheck Hash Error", "NanooAPIManager:RECEIPT_VALIDATION", LogType.Warning);
							}
							break;
						}
						default:
							Singleton<PaymentManager>.instance.LogAnswer(false, _receiptValidatorResponse.product_id);
							break;
						}
					}
					isReceiptValidatorComplete = true;
					break;
				}
				case APIType.COUPON_CHECK:
				{
					if (_couponCheckResponse == null)
					{
						_couponCheckResponse = new CouponCheckResponse();
					}
					if (UIWindowLoading.instance != null)
					{
						UIWindowLoading.instance.closeLoadingUI();
					}
					_couponCheckResponse = JsonUtility.FromJson<CouponCheckResponse>(response);
					string resultMsg4 = string.Empty;
					if (!string.IsNullOrEmpty(_couponCheckResponse.result))
					{
						bool alreadyUsed = true;
						switch (_couponCheckResponse.result.ToLower())
						{
						case "success":
						{
							ItemType itemType = GetItemTypeByCode(_couponCheckResponse.item);
							double itemQuantity = GetItemQuantityByCode(_couponCheckResponse.item);
							resultMsg4 = GetItemDescByItemType(itemType, itemQuantity);
							Action actionCoupon = GetActionByItemType(itemType, itemQuantity, UIWindowSetting.instance.enterCouponObjects.transform, out alreadyUsed);
							if (actionCoupon != null)
							{
								actionCoupon();
								Singleton<DataManager>.instance.saveData();
							}
							break;
						}
						case "fail":
							resultMsg4 = I18NManager.Get("COUPON_FAIL");
							break;
						case "exist":
							resultMsg4 = I18NManager.Get("COUPON_EXIST");
							break;
						case "limit":
							resultMsg4 = I18NManager.Get("COUPON_LIMIT");
							break;
						case "expired":
							resultMsg4 = I18NManager.Get("COUPON_EXPIRED");
							break;
						default:
							resultMsg4 = I18NManager.Get("COUPON_UNKNOWN_ERROR") + _couponCheckResponse.result;
							break;
						}
						if (alreadyUsed)
						{
							UIWindowDialog.openDescriptionNotUsingI18N(resultMsg4, UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						}
						else
						{
							UIWindowDialog.openDescriptionNotUsingI18N(I18NManager.Get("COUPON_JUST_SUCESS") + " " + I18NManager.Get("ALREADY_USED_COUPON"), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						}
					}
					else
					{
						Singleton<LogglyManager>.instance.SendLoggly("Coupon Use Error", "NanooAPIManager:COUPON_CHECK", LogType.Error);
					}
					isCouponCheckComplete = true;
					break;
				}
				case APIType.POSTBOX_RECEIVE:
					isPostItemReceiveSuccess = response.ToLower().Contains("success");
					if (!isPostItemReceiveSuccess)
					{
						if (_errorResponse == null)
						{
							_errorResponse = new ErrorResponse();
						}
						_errorResponse = JsonUtility.FromJson<ErrorResponse>(response);
					}
					else
					{
						_errorResponse = null;
					}
					isPostItemReceiveComplete = true;
					break;
				case APIType.SAVE_VALIDATION:
					if (_cloudSaveResponse == null)
					{
						_cloudSaveResponse = new CloudSaveResponse();
					}
					_cloudSaveResponse = JsonUtility.FromJson<CloudSaveResponse>(response);
					if (UIWindowSaveCloud.instance != null)
					{
						UIWindowSaveCloud.instance.close();
					}
					if (UIWindowLoading.instance != null)
					{
						UIWindowLoading.instance.closeLoadingUI();
					}
					if (!string.IsNullOrEmpty(_cloudSaveResponse.result))
					{
						string resultMsg2 = string.Empty;
						switch (_cloudSaveResponse.result.ToLower())
						{
						case "success":
							resultMsg2 = I18NManager.Get("CLOUD_SAVE_SUCCESS");
							break;
						case "fail":
							resultMsg2 = I18NManager.Get("CLOUD_SAVE_FAIL");
							break;
						case "device":
							resultMsg2 = I18NManager.Get("CLOUD_SAVE_DEVICE");
							break;
						case "time":
							resultMsg2 = I18NManager.Get("CLOUD_SAVE_TIME");
							break;
						default:
							resultMsg2 = I18NManager.Get("CLOUD_SAVE_UNKNOWN_ERROR") + _cloudSaveResponse.result;
							break;
						}
						if (_cloudSaveResponse.result.ToLower() == "success")
						{
							UIWindowDialog.openDescription("SUCCESS_SAVE_TO_CLOUD", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						}
						else
						{
							UIWindowDialog.openDescriptionNotUsingI18N(resultMsg2, UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						}
					}
					else
					{
						UIWindowDialog.openDescription("CLOUD_SAVE_UNKNOWN_ERROR", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
					}
					isCloudSaveComplete = true;
					break;
				case APIType.ADID_RECORD:
				case APIType.POSTBOX_GIVE:
					break;
				case APIType.SUBSCRIPTION_SAVE:
					if (_subscriptionSaveResponse == null)
					{
						_subscriptionSaveResponse = new SubscriptionSaveResponse();
					}
					_subscriptionSaveResponse = JsonUtility.FromJson<SubscriptionSaveResponse>(response);
					if (string.IsNullOrEmpty(_subscriptionSaveResponse.result))
					{
						break;
					}
					if (!_subscriptionSaveResponse.result.ToLower().Contains("success"))
					{
						Singleton<LogglyManager>.instance.SendLoggly("Subscription Report Fail", "NanooAPIManager:SUBSCRIPTION_SAVE", LogType.Warning);
						if (UIWindowLoading.instance != null)
						{
							UIWindowLoading.instance.close();
						}
						break;
					}
					m_vipItemRemainSeconds = int.Parse(_subscriptionSaveResponse.expired_sec);
					if (!m_isExistSubscriptionItemWhenPurchaseVIP)
					{
						m_isExistSubscriptionItemWhenPurchaseVIP = true;
						bool success = true;
						GetActionByItemType(ItemType.VIPReward, 0.0, null, out success)();
					}
					else
					{
						UIWindowDialog.openDescription("VIP_TIME_STACK_DESCRIPTION", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						if (UIWindowPopupShop.instance.isOpen)
						{
							UIWindowPopupShop.instance.openShopPopupWithType(UIWindowPopupShop.instance.currentSelectedType);
							UIWindowPopupShop.instance.focusingToVIPItem();
						}
						if (UIWindowManageShop.instance.isOpen)
						{
							UIWindowManageShop.instance.openShopUI(UIWindowManageShop.instance.currentSelectedType);
							UIWindowManageShop.instance.focusingToVIPItem();
						}
					}
					UIWindowOutgame.instance.refreshVIPStatus();
					Singleton<DataManager>.instance.saveData();
					if (UIWindowLoading.instance != null)
					{
						UIWindowLoading.instance.close();
					}
					break;
				default:
					if (UIWindowLoading.instance != null)
					{
						UIWindowLoading.instance.closeLoadingUI();
					}
					break;
				}
			}
			else if (UIWindowLoading.instance != null)
			{
				UIWindowLoading.instance.closeLoadingUI();
			}
		}
		else if (UIWindowLoading.instance != null)
		{
			UIWindowLoading.instance.closeLoadingUI();
		}
	}

	private IEnumerator waitForLoadIngameObject(Action loadingEndAction)
	{
		yield return new WaitUntil(() => UIWindowIntro.isLoaded);
		yield return null;
		if (loadingEndAction != null)
		{
			loadingEndAction();
		}
	}

	public void SetUUID(string value)
	{
		int partLength = 6;
		string[] array = SplitInParts(value, partLength).ToArray();
		int num = array.Length;
		if (num > 0)
		{
			if (num >= 4)
			{
				int num2 = int.Parse(array[0]);
				int num3 = int.Parse(array[1]);
				int num4 = int.Parse(array[2]);
				int num5 = int.Parse(array[3]);
				uuid = string.Format("{0}-{1}-{2}{3}", Base36.Encode(num2), Base36.Encode(num3), Base36.Encode(num4), Base36.Encode(num5));
			}
			else if (num >= 3)
			{
				int num6 = int.Parse(array[0]);
				int num7 = int.Parse(array[1]);
				int num8 = int.Parse(array[2]);
				uuid = string.Format("{0}-{1}{2}", Base36.Encode(num6), Base36.Encode(num7), Base36.Encode(num8));
			}
			else if (num >= 2)
			{
				int num9 = int.Parse(array[0]);
				int num10 = int.Parse(array[1]);
				uuid = string.Format("{0}-{1}", Base36.Encode(num9), Base36.Encode(num10));
			}
			else
			{
				int num11 = int.Parse(array[0]);
				uuid = string.Format("{0}", Base36.Encode(num11));
			}
			PlayerPrefs.SetString("UserID", uuid);
			Crashlytics.SetUserIdentifier(uuid);
		}
		else
		{
			Singleton<LogglyManager>.instance.SendLoggly("UserID value: " + value, "Fail to split UUID string", LogType.Error);
		}
	}

	private void ActionAdvertisingIdLoaded(GP_AdvertisingIdLoadResult res)
	{
		GooglePlayUtils.ActionAdvertisingIdLoaded = (Action<GP_AdvertisingIdLoadResult>)Delegate.Remove(GooglePlayUtils.ActionAdvertisingIdLoaded, new Action<GP_AdvertisingIdLoadResult>(ActionAdvertisingIdLoaded));
		if (res.IsSucceeded)
		{
			adid = res.id;
			CallAPI(APIType.ADID_RECORD, adid);
		}
		else
		{
			adid = string.Empty;
		}
	}

	public string GetNanooGameChannelID()
	{
		string empty = string.Empty;
		switch (I18NManager.currentLanguage)
		{
		case I18NManager.Language.Korean:
			return "Soqf0nckf8VWLxDK2KSyxmIUBbI00pCes93jj0rPhavq2L7QpP";
		case I18NManager.Language.ChineseSimplified:
		case I18NManager.Language.ChineseTraditional:
			return "AO3v0GaHsqAkWAPzpvKTfbRs9LDcbz77NbZqrAZk6HVQV0gtcg";
		case I18NManager.Language.Japanese:
			return "YZ3CT7KUZusZuFmG5ojTppSfdRBEw4cPeHbjnIyX7F166RPEZT";
		default:
			return "PtwQ10M55GkstzgHsVYQ4YqV5Hr5dHI06cZQ8Nf7eYy6KKZjXo";
		}
	}

	public string GetNoticeCode()
	{
		string empty = string.Empty;
		switch (I18NManager.currentLanguage)
		{
		case I18NManager.Language.Korean:
			return "8b1deeb45b6a30cddc62778e09ce6437";
		case I18NManager.Language.ChineseSimplified:
		case I18NManager.Language.ChineseTraditional:
			return "c6fb9c497e8ee1ca1d0bba03270f448a";
		case I18NManager.Language.Japanese:
			return "6f5707836e5f75b076170640a1bf6806";
		default:
			return "5dea62093135800e59fe7d0a4bd4ca50";
		}
	}

	public string GetEventCode()
	{
		string empty = string.Empty;
		switch (I18NManager.currentLanguage)
		{
		case I18NManager.Language.Korean:
			return "c4a3b019e5c442ea3c5fa13a478c23dc";
		case I18NManager.Language.ChineseSimplified:
		case I18NManager.Language.ChineseTraditional:
			return "708e4e886a67f162ecee6f1134fdee69";
		case I18NManager.Language.Japanese:
			return "8f02c2b18214b7d4cdb63f61f34f9671";
		default:
			return "5a9c58404ca2837a78db01efcc73352a";
		}
	}

	public string GetEscapePrincessCode()
	{
		string empty = string.Empty;
		switch (I18NManager.currentLanguage)
		{
		case I18NManager.Language.Korean:
			return "092c151b3cd7bdf5bb32035dd2aba4b8";
		case I18NManager.Language.ChineseSimplified:
		case I18NManager.Language.ChineseTraditional:
			return "3cee5e19e27cb745c46b6164eac261de";
		case I18NManager.Language.Japanese:
			return "e850814af824eda43b8322dfafbc8bc2";
		default:
			return "58977fbd1668a77625f74593548465a0";
		}
	}

	public string GetISO639()
	{
		string empty = string.Empty;
		switch (I18NManager.currentLanguage)
		{
		case I18NManager.Language.Korean:
			return "ko";
		case I18NManager.Language.ChineseSimplified:
			return "zh";
		case I18NManager.Language.ChineseTraditional:
			return "zh";
		case I18NManager.Language.Japanese:
			return "ja";
		default:
			return "en";
		}
	}

	public string GetISOforForum()
	{
		string empty = string.Empty;
		switch (I18NManager.currentLanguage)
		{
		case I18NManager.Language.Korean:
			return "ko_KR";
		case I18NManager.Language.ChineseSimplified:
			return "zh_CN";
		case I18NManager.Language.ChineseTraditional:
			return "zh_CN";
		case I18NManager.Language.Japanese:
			return "ja_JP";
		default:
			return "en_US";
		}
	}

	private void GetPromotionTexture()
	{
		StartCoroutine(DownloadPromotionTexture());
	}

	private IEnumerator DownloadPromotionTexture()
	{
		if (!string.IsNullOrEmpty(_promotionRequestResponse.image))
		{
			WWW www = new WWW(_promotionRequestResponse.image);
			yield return www;
			if (www.error != null || www.texture.width == 8 || www.texture.height == 8)
			{
				promotionSprite = null;
			}
			else
			{
				Texture2D texture = www.texture;
				promotionSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
			}
		}
		isPromotionRequestComplete = true;
	}

	private IEnumerator DownloadEventBannerTexture()
	{
		if (eventBanner != null && !string.IsNullOrEmpty(eventBanner.banner))
		{
			WWW www = new WWW(eventBanner.banner);
			yield return www;
			if (www.error == null && www.texture.width != 8 && www.texture.height != 8)
			{
				Texture2D texture = www.texture;
				eventBannerSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
			}
		}
		isEventBannerComplete = true;
	}

	private ItemType AssertCodeCheck(ItemType curItem, ItemType newItem)
	{
		if (curItem != ItemType.None)
		{
			DebugManager.LogError("중복된 쿠폰코드 발견!");
			return ItemType.None;
		}
		return newItem;
	}

	public ItemType GetItemTypeByCode(string code)
	{
		Regex regex = new Regex("[_a-zA-Z]+_[\\d]+");
		if (regex.IsMatch(code))
		{
			int length = code.LastIndexOf('_');
			string text = code.Substring(0, length);
			ItemType itemType = ItemType.None;
			if (text.Equals("ARCHER_SKIN"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.ArcherSkin);
			}
			if (text.Equals("COLLECT_EVENT_RESOURCE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.CollectEventResource);
			}
			if (text.Equals("COUNT_SILVER_FINGER"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.CountSilverFinger);
			}
			if (text.Equals("COUNT_GOLD_FINGER"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.CountGoldFinger);
			}
			if (text.Equals("COUNT_AUTO_OPEN_CHEST"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.CountAutoOpenTreasureChest);
			}
			if (text.Equals("COLLEAGUE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.Colleague);
			}
			if (text.Equals("DOUBLE_SPEED"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.DoubleSpeed);
			}
			if (text.Equals("DIA"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.Ruby);
			}
			if (text.Equals("ELOPE_RESOURCE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.ElopeResources);
			}
			if (text.Equals("GOLD"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.Gold);
			}
			if (text.Equals("GOLD_FINGER"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.GoldFinger);
			}
			if (text.Equals("HEART_FOR_ELOPE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.HeartForElopeMode);
			}
			if (text.Equals("HEART_COIN_FOR_ELOPE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.HeartCoinForElopeMode);
			}
			if (text.Equals("HONOR"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.PVPHonorToken);
			}
			if (text.Equals("KEY"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.TreasureKey);
			}
			if (text.Equals("PRIEST_SKIN"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.PriestSkin);
			}
			if (text.Equals("PSTONE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.Stone);
			}
			if (text.Equals("SILVER_FINGER"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.SilverFinger);
			}
			if (text.Equals("TIMER_AUTO_OPEN_CHEST"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.TimerAutoOpenTreasureChest);
			}
			if (text.Equals("TRANSCEND_STONE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.TranscendStone);
			}
			if (text.Equals("TREASURE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.Treasure);
			}
			if (text.Equals("WEAPON_SKIN_MASTER_PIECE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.WeaponSkinReinforcementMasterPiece);
			}
			if (text.Equals("WEAPON_SKIN_PIECE"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.WeaponSkinPiece);
			}
			if (text.Equals("WARRIOR_SKIN"))
			{
				itemType = AssertCodeCheck(itemType, ItemType.WarriorSkin);
			}
			if (itemType != ItemType.None)
			{
				return itemType;
			}
		}
		if (code.Equals("PREORDER"))
		{
			return ItemType.Preorder;
		}
		if (code.Equals("HEROSQUAD_NORMAL"))
		{
			return ItemType.HeroSquadNormal;
		}
		if (code.Equals("HEROSQUAD_PAID1"))
		{
			return ItemType.HeroSquadPaid1;
		}
		if (code.Equals("HEROSQUAD_PAID2"))
		{
			return ItemType.HeroSquadPaid2;
		}
		if (code.Equals("HEROSQUAD_PAID3"))
		{
			return ItemType.HeroSquadPaid3;
		}
		if (code.Equals("LIMITED_PREMIUM"))
		{
			return ItemType.LimitedPremiumPackage;
		}
		if (code.Equals("LIMITED_STARTER_PACK"))
		{
			return ItemType.LimitedStarterPack;
		}
		if (code.Equals("LIMITED_ONEPLUSONE_DIAMOND"))
		{
			return ItemType.LimitedOnePlusOneDiamond;
		}
		if (code.Equals("LIMITED_ALL_HERO_PACK"))
		{
			return ItemType.LimitedAllHeroPack;
		}
		if (code.Equals("SURPRISE_SILVER_FINGER_PACK"))
		{
			return ItemType.SurpriseSilverFinger;
		}
		if (code.Equals("SURPRISE_AUTO_TAP_PACK"))
		{
			return ItemType.SurpriseAutoTapPack;
		}
		if (code.Equals("VALKYRIE_SKIN_PACKAGE"))
		{
			return ItemType.ValkyriePackage;
		}
		if (code.Equals("NEW_TRANSCEND_PACKAGE"))
		{
			return ItemType.NewEventPackage;
		}
		if (code.Equals("EVENT_PACKAGE"))
		{
			return ItemType.EventPackage;
		}
		if (code.Equals("LIMITED_SKIN_PACK"))
		{
			return ItemType.LimitedPremiumSkinPackage;
		}
		if (code.Equals("LIMITED_FIRST_PURCHASE"))
		{
			return ItemType.LimitedFirstPurchasePackage;
		}
		if (code.Equals("MONTHLY_VIP_PACK"))
		{
			return ItemType.LimitedMonthlyVIPPackage;
		}
		if (code.Equals("VIP_REWARD"))
		{
			return ItemType.VIPReward;
		}
		if (code.Equals("MONTHLY_MINI_VIP_PACK"))
		{
			return ItemType.MiniLimitedMonthlyVIPPackage;
		}
		if (code.Equals("ANGELICA_PACKAGE"))
		{
			return ItemType.AngelicaPackage;
		}
		if (code.Equals("ICE_METEOR_SKILL_PACKAGE"))
		{
			return ItemType.IceMeteorPassiveSkillPackage;
		}
		if (code.Equals("ANGELINA_PACKAGE"))
		{
			return ItemType.AngelinaPackage;
		}
		if (code.Equals("REINFORCEMENTSKILL"))
		{
			return ItemType.ReinforcementSkill;
		}
		if (code.Equals("REINFORCEMENT_SKILL_PACKAGE"))
		{
			return ItemType.ReinforcementSkillPackage;
		}
		if (code.Equals("BROTHER_HOOD_SKIN_PACKAGE"))
		{
			return ItemType.BrotherhoodPackage;
		}
		if (code.Equals("HAND_SOME_GUY"))
		{
			return ItemType.HandsomeGuy;
		}
		if (code.Equals("SUPER_HAND_SOME_GUY"))
		{
			return ItemType.SuperHandsomeGuy;
		}
		if (code.Equals("SPEED_GUY"))
		{
			return ItemType.SpeedGuy;
		}
		if (code.Equals("SUPER_SPEED_GUY"))
		{
			return ItemType.SuperSpeedGuy;
		}
		if (code.Equals("DEMON_KING_SKIN_PACKAGE"))
		{
			return ItemType.DemonKingSkinPackage;
		}
		if (code.Equals("ANGELA_PACKAGE"))
		{
			return ItemType.AngelaPackage;
		}
		if (code.Equals("GOBLIN_PACKAGE"))
		{
			return ItemType.GoblinPackage;
		}
		if (code.Equals("ZEUS_PACKAGE"))
		{
			return ItemType.ZeusPackage;
		}
		if (code.Equals("COLLEAGUESKIN_PACKAGE_A"))
		{
			return ItemType.ColleagueSkinPackageA;
		}
		if (code.Equals("ESSENTIAL_PACKAGE"))
		{
			return ItemType.EssentialPackage;
		}
		if (code.Equals("GOLDROULETTE_PACKAGE"))
		{
			return ItemType.GoldRoulettePackage;
		}
		if (code.Equals("COLLEAGUESKIN_PACKAGE_B"))
		{
			return ItemType.ColleagueSkinPackageB;
		}
		if (code.Equals("TOWER_MODE_FREE_TICKET_PACKAGE"))
		{
			return ItemType.TowerModeFreeTicketPackage;
		}
		if (code.Equals("TIME_ATTACK_MODE_RANKING_100TH_REWARD"))
		{
			return ItemType.ConquerorTokenTreasure;
		}
		if (code.Equals("ENDLESS_MODE_RANKING_100TH_REWARD"))
		{
			return ItemType.PatienceTokenTreasure;
		}
		if (code.Equals("TIME_ATTACK_MODE_RANKING_3TH_REWARD"))
		{
			return ItemType.TowerModeRankingTimeAttackCharacterSkin;
		}
		if (code.Equals("ENDLESS_MODE_RANKING_3TH_REWARD"))
		{
			return ItemType.TowerModeRankingEndlessCharacterSkin;
		}
		if (code.Equals("BOTH_TOWER_MODE_RANKING_3TH_REWARD"))
		{
			return ItemType.TowerModeRankingBothCharacterSkin;
		}
		if (code.Equals("MASTER_SKIN_PACKAGE"))
		{
			return ItemType.MasterSkinPackage;
		}
		if (code.Equals("WEAPONSKIN_PREMIUM_LOTTERY"))
		{
			return ItemType.WeaponSkinPremiumLottery;
		}
		if (code.Equals("WEAPON_LEGENDARY_SKIN_PACKAGE"))
		{
			return ItemType.WeaponLegendarySkinPackage;
		}
		if (code.Equals("RANDOM_CHARACTER_SKIN_PACKAGE_A"))
		{
			return ItemType.RandomCharacterSkinPackageA;
		}
		if (code.Equals("GOLDEN_PACKAGE_A"))
		{
			return ItemType.GoldenPackageA;
		}
		if (code.Equals("GOLDEN_PACKAGE_B"))
		{
			return ItemType.GoldenPackageB;
		}
		if (code.Equals("GOLDEN_PACKAGE_C"))
		{
			return ItemType.GoldenPackageC;
		}
		if (code.Equals("FLOWER_PACKAGE"))
		{
			return ItemType.FlowerPackage;
		}
		if (code.Equals("TRANSCEND_PACKAGE"))
		{
			return ItemType.TranscendPackage;
		}
		if (code.Equals("HONOR_PACKAGE"))
		{
			return ItemType.HonorPackage;
		}
		if (code.Equals("DEMON_PACKAGE"))
		{
			return ItemType.DemonPackage;
		}
		if (code.Equals("ALL_RANDOM_HERO_PACKAGE"))
		{
			return ItemType.RandomHeroPackage;
		}
		if (code.Equals("LIMITED_RANDOM_HERO_PACKAGE"))
		{
			return ItemType.RandomLimitedHeroPackage;
		}
		if (code.Equals("AFK_PACKAGE"))
		{
			return ItemType.AFKPackage;
		}
		if (code.Equals("PVP_CHARACTER_PACKAGE"))
		{
			return ItemType.PVPCharacterPackage;
		}
		if (code.Equals("SUPER_AFK_PACKAGE"))
		{
			return ItemType.SuperAFKPackage;
		}
		if (code.Equals("ULTRA_AFK_PACKAGE"))
		{
			return ItemType.UltraAFKPackage;
		}
		if (code.Equals("BRONZE_FINGER"))
		{
			return ItemType.BronzeFinger;
		}
		if (code.Equals("HOT_SUMMER_PACKAGE"))
		{
			return ItemType.HotSummerPackage;
		}
		if (code.Equals("DEMON_COSPLAY_PACKAGE"))
		{
			return ItemType.DemonCosplayPackage;
		}
		if (code.Equals("HALLOWEEN_PACKAGE"))
		{
			return ItemType.HalloweenPackage;
		}
		if (code.Equals("FORCE_PAIDUSER"))
		{
			return ItemType.ForcePaidUser;
		}
		if (code.Equals("XMAS_PACKAGE"))
		{
			return ItemType.XmasPackage;
		}
		if (code.Equals("MARCH2018_PACKAGE"))
		{
			return ItemType.March2018_FlowerPackage;
		}
		if (code.Equals("YUMMYWEAPON_PACKAGE"))
		{
			return ItemType.YummyWeaponSkinPackage;
		}
		if (code.Equals("WEDDING_PACKAGE"))
		{
			return ItemType.WeddingSkinPackage;
		}
		if (code.Equals("COLLEAGUESKIN_PACKAGE_C"))
		{
			return ItemType.ColleagueSkinPackageC;
		}
		return ItemType.None;
	}

	public double GetItemQuantityByCode(string code)
	{
		double result = 0.0;
		if (code.Contains("PREORDER"))
		{
			result = 300.0;
		}
		else if (code.Contains("HEROSQUAD_NORMAL"))
		{
			result = 0.0;
		}
		else if (code.Contains("HEROSQUAD_PAID1"))
		{
			result = 300.0;
		}
		else if (code.Contains("HEROSQUAD_PAID2"))
		{
			result = 1200.0;
		}
		else if (code.Contains("HEROSQUAD_PAID3"))
		{
			result = 2100.0;
		}
		else
		{
			string[] array = code.Split('_');
			if (!double.TryParse(code.Split('_')[array.Length - 1], out result))
			{
				result = 0.0;
			}
		}
		return result;
	}

	public string GetItemDescByItemType(ItemType itemType, double itemQuantity)
	{
		string empty = string.Empty;
		switch (itemType)
		{
		case ItemType.Gold:
			empty += I18NManager.Get("COUPON_REWARD_GOLD");
			empty += string.Format(" : {0}", GameManager.changeUnit(itemQuantity));
			return string.Format(I18NManager.Get("COUPON_SUCESS"), empty);
		case ItemType.Ruby:
			empty += I18NManager.Get("COUPON_REWARD_RUBY");
			empty += string.Format(" : {0} {1}", itemQuantity.ToString(), I18NManager.Get("KOREAN_COUNT"));
			return string.Format(I18NManager.Get("COUPON_SUCESS"), empty);
		case ItemType.Stone:
			empty += I18NManager.Get(string.Empty);
			empty += string.Format(" : {0} {1}", itemQuantity.ToString(), I18NManager.Get("KOREAN_COUNT"));
			return string.Format(I18NManager.Get("COUPON_SUCESS"), empty);
		case ItemType.TreasureKey:
			empty += I18NManager.Get("COUPON_REWARD_KEYS");
			empty += string.Format(" : {0} {1}", itemQuantity.ToString(), I18NManager.Get("KOREAN_COUNT"));
			return string.Format(I18NManager.Get("COUPON_SUCESS"), empty);
		case ItemType.Preorder:
			return empty + string.Format(I18NManager.Get("SHOP_RUBY_TITLE_TEXT_2"), (long)itemQuantity) + ", " + I18NManager.Get("WARRIOR_SKIN_NAME_8");
		case ItemType.HeroSquadNormal:
			return empty + I18NManager.Get("PRIEST_SKIN_NAME_8");
		case ItemType.HeroSquadPaid1:
		{
			string text = empty;
			return text + string.Format(I18NManager.Get("SHOP_RUBY_TITLE_TEXT_2"), (long)itemQuantity) + ", " + I18NManager.Get("PRIEST_SKIN_NAME_8") + ", " + I18NManager.Get("ARCHER_SKIN_NAME_8");
		}
		case ItemType.HeroSquadPaid2:
		case ItemType.HeroSquadPaid3:
		{
			string text = empty;
			return text + string.Format(I18NManager.Get("SHOP_RUBY_TITLE_TEXT_2"), (long)itemQuantity) + ", " + I18NManager.Get("COUPON_REWARD_KEYS") + 60 + I18NManager.Get("KOREAN_COUNT") + ", " + I18NManager.Get("PRIEST_SKIN_NAME_8") + "," + I18NManager.Get("ARCHER_SKIN_NAME_8");
		}
		default:
			return empty + I18NManager.Get("COUPON_JUST_SUCESS");
		}
	}

	public Action GetActionByItemType(ItemType itemType, double itemValue, Transform t, out bool isSuccess)
	{
		isSuccess = true;
		switch (itemType)
		{
		case ItemType.Gold:
			return delegate
			{
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Postbox, AnalyzeManager.ActionType.PostboxGold);
				Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.Gold, 30L, 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<GoldManager>.instance.increaseGold(itemValue, true);
			};
		case ItemType.Ruby:
			return delegate
			{
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Postbox, AnalyzeManager.ActionType.PostboxDiamond);
				Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.Ruby, (long)Math.Min(itemValue, 30.0), 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<RubyManager>.instance.increaseRuby(itemValue, true);
			};
		case ItemType.Stone:
			return delegate
			{
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Postbox, AnalyzeManager.ActionType.PostboxGold);
				Singleton<TreasureManager>.instance.increaseTreasureEnchantStone(itemValue);
				Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.TreasureEnchantStone, (long)Math.Min(itemValue, 30.0), 0.04f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
			};
		case ItemType.TreasureKey:
			return delegate
			{
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Postbox, AnalyzeManager.ActionType.PostboxTreasureKey);
				Singleton<TreasureManager>.instance.increaseTreasurePiece((long)itemValue);
				Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.TreasurePiece, (long)Math.Min(itemValue, 30.0), 0.04f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
			};
		case ItemType.Preorder:
			if (!Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Contains("PREORDER"))
			{
				return delegate
				{
					Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Add("PREORDER");
					Singleton<RubyManager>.instance.increaseRuby(300.0, true);
					Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.Ruby, 15L, 0.02f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
					if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(CharacterSkinManager.WarriorSkinType.Siegfried).isHaving)
					{
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.WarriorSkinType.Siegfried);
						Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.WarriorSkinType.Siegfried);
					}
					Singleton<CharacterSkinManager>.instance.refreshCharacterTypeList();
					UIWindowCharacterSkin.instance.skinScroll.refreshAll();
					UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
				};
			}
			isSuccess = false;
			return delegate
			{
			};
		case ItemType.HeroSquadNormal:
			if (!Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Contains("HEROSQUAD_NORMAL"))
			{
				return delegate
				{
					Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Add("HEROSQUAD_NORMAL");
					if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(CharacterSkinManager.PriestSkinType.Candy).isHaving)
					{
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.PriestSkinType.Candy);
						Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.PriestSkinType.Candy);
					}
					Singleton<CharacterSkinManager>.instance.refreshCharacterTypeList();
					UIWindowCharacterSkin.instance.skinScroll.refreshAll();
					UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
				};
			}
			isSuccess = false;
			return null;
		case ItemType.HeroSquadPaid1:
			if (!Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Contains("HEROSQUAD_PAID1"))
			{
				return delegate
				{
					Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Add("HEROSQUAD_PAID1");
					Singleton<RubyManager>.instance.increaseRuby(300.0, true);
					Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.Ruby, 20L, 0.02f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
					if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(CharacterSkinManager.PriestSkinType.Candy).isHaving)
					{
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.PriestSkinType.Candy);
						Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.PriestSkinType.Candy);
					}
					if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(CharacterSkinManager.ArcherSkinType.Marauder).isHaving)
					{
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.ArcherSkinType.Marauder);
						Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.ArcherSkinType.Marauder);
					}
					Singleton<CharacterSkinManager>.instance.refreshCharacterTypeList();
					UIWindowCharacterSkin.instance.skinScroll.refreshAll();
					UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
				};
			}
			isSuccess = false;
			return null;
		case ItemType.HeroSquadPaid2:
			if (!Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Contains("HEROSQUAD_PAID2"))
			{
				return delegate
				{
					Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Add("HEROSQUAD_PAID2");
					Singleton<RubyManager>.instance.increaseRuby(1200.0, true);
					Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.Ruby, 30L, 0.02f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
					Singleton<TreasureManager>.instance.increaseTreasurePiece(60L);
					Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.TreasurePiece, 30L, 0.04f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
					if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(CharacterSkinManager.PriestSkinType.Candy).isHaving)
					{
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.PriestSkinType.Candy);
						Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.PriestSkinType.Candy);
					}
					if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(CharacterSkinManager.ArcherSkinType.Marauder).isHaving)
					{
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.ArcherSkinType.Marauder);
						Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.ArcherSkinType.Marauder);
					}
					UIWindowCharacterSkin.instance.skinScroll.refreshAll();
					UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
				};
			}
			isSuccess = false;
			return null;
		case ItemType.HeroSquadPaid3:
			if (!Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Contains("HEROSQUAD_PAID3"))
			{
				return delegate
				{
					Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Add("HEROSQUAD_PAID3");
					Singleton<RubyManager>.instance.increaseRuby(2100.0, true);
					Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.Ruby, 40L, 0.02f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
					Singleton<TreasureManager>.instance.increaseTreasurePiece(60L);
					Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.TreasurePiece, 30L, 0.04f, delegate
					{
						Singleton<AudioManager>.instance.playEffectSound("getgold");
					});
					if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(CharacterSkinManager.PriestSkinType.Candy).isHaving)
					{
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.PriestSkinType.Candy);
						Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.PriestSkinType.Candy);
					}
					if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(CharacterSkinManager.ArcherSkinType.Marauder).isHaving)
					{
						Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.ArcherSkinType.Marauder);
						Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.ArcherSkinType.Marauder);
					}
					UIWindowCharacterSkin.instance.skinScroll.refreshAll();
					UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
				};
			}
			isSuccess = false;
			return null;
		case ItemType.LimitedPremiumPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.PremiumPack], t);
		case ItemType.LimitedStarterPack:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.StarterPack], t);
		case ItemType.LimitedOnePlusOneDiamond:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.OnePlusOneDiamonds], t);
		case ItemType.LimitedAllHeroPack:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.AllCharacterPack], t);
		case ItemType.SurpriseSilverFinger:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.SilverFinger], t);
		case ItemType.SurpriseAutoTapPack:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.BadPriemiumPack], t);
		case ItemType.EventPackage:
			if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.EventPackage))
			{
				return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.EventPackage], t);
			}
			return delegate
			{
				Singleton<FlyResourcesManager>.instance.playEffectResources(t, FlyResourcesManager.ResourceType.Ruby, 30L, 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<RubyManager>.instance.increaseRuby(5000.0, true);
			};
		case ItemType.WarriorSkin:
			return delegate
			{
				CharacterSkinManager.WarriorSkinType skinType2 = (CharacterSkinManager.WarriorSkinType)(itemValue - 1.0);
				if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType2).isHaving)
				{
					Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType2);
					Singleton<CharacterManager>.instance.equipCharacter(skinType2);
				}
			};
		case ItemType.PriestSkin:
			return delegate
			{
				CharacterSkinManager.PriestSkinType skinType = (CharacterSkinManager.PriestSkinType)(itemValue - 1.0);
				if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType).isHaving)
				{
					Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType);
					Singleton<CharacterManager>.instance.equipCharacter(skinType);
				}
			};
		case ItemType.ArcherSkin:
			return delegate
			{
				CharacterSkinManager.ArcherSkinType skinType3 = (CharacterSkinManager.ArcherSkinType)(itemValue - 1.0);
				if (!Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType3).isHaving)
				{
					Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType3);
					Singleton<CharacterManager>.instance.equipCharacter(skinType3);
				}
			};
		case ItemType.Treasure:
			return delegate
			{
				TreasureManager.TreasureType treasureType = (TreasureManager.TreasureType)itemValue;
				double damageValue3 = Singleton<TreasureManager>.instance.getDamageValue(treasureType);
				double treasureEffectValue3 = Singleton<TreasureManager>.instance.getTreasureEffectValue(treasureType);
				TreasureInventoryData treasureInventoryData3 = new TreasureInventoryData
				{
					treasureType = treasureType,
					treasureEffectValue = treasureEffectValue3,
					damagePercentValue = damageValue3,
					treasureLevel = 1L
				};
				TreasureValueData value3 = default(TreasureValueData);
				value3.damagePercentValue = treasureInventoryData3.damagePercentValue;
				value3.treasureEffectValue = treasureInventoryData3.treasureEffectValue;
				Singleton<TreasureManager>.instance.obtainTreasure(treasureType, value3);
			};
		case ItemType.SilverFinger:
			return delegate
			{
				long num4 = 0L;
				if (Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime > UnbiasedTime.Instance.Now().Ticks)
				{
					num4 = new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime).AddMinutes(itemValue).Ticks;
				}
				else
				{
					DateTime dateTime4 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
					num4 = dateTime4.AddMinutes(itemValue).Ticks;
				}
				Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime = num4;
				Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			};
		case ItemType.GoldFinger:
			return delegate
			{
				long num3 = 0L;
				if (Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime > UnbiasedTime.Instance.Now().Ticks)
				{
					num3 = new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime).AddMinutes(itemValue).Ticks;
				}
				else
				{
					DateTime dateTime3 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
					num3 = dateTime3.AddMinutes(itemValue).Ticks;
				}
				Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime = num3;
				Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			};
		case ItemType.LimitedPremiumSkinPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.LimitedSkinPremiumPack], t);
		case ItemType.LimitedFirstPurchasePackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.FirstPurchasePackage], t);
		case ItemType.LimitedMonthlyVIPPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.VIPPackage], t);
		case ItemType.MiniLimitedMonthlyVIPPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.MiniVIPPackage], t);
		case ItemType.VIPReward:
			return delegate
			{
				UIWindowVIPReward.instance.openVIPReward(Singleton<NanooAPIManager>.instance.GetVipItemCount());
				if (UIWindowPopupShop.instance.isOpen)
				{
					UIWindowPopupShop.instance.openShopPopupWithType(UIWindowPopupShop.instance.currentSelectedType);
					UIWindowPopupShop.instance.focusingToVIPItem();
				}
				if (UIWindowManageShop.instance.isOpen)
				{
					UIWindowManageShop.instance.openShopUI(UIWindowManageShop.instance.currentSelectedType);
					UIWindowManageShop.instance.focusingToVIPItem();
				}
				UIWindowOutgame.instance.refreshVIPStatus();
			};
		case ItemType.CountSilverFinger:
			return delegate
			{
				Singleton<DataManager>.instance.currentGameData.countableSilverFingerLeftCount += (int)itemValue;
				Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			};
		case ItemType.CountGoldFinger:
			return delegate
			{
				Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount += (int)itemValue;
				Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			};
		case ItemType.TranscendStone:
			return delegate
			{
				Singleton<TranscendManager>.instance.increaseTranscendStone((long)itemValue);
			};
		case ItemType.ValkyriePackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.ValkyrieSkinPackage], t);
		case ItemType.NewEventPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.NewTranscendEventPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.NewTranscendEventPackage], t)();
				}
				else
				{
					List<ShopManager.LimitedItemSetData> limitedAttribudeList = new List<ShopManager.LimitedItemSetData>
					{
						new ShopManager.LimitedItemSetData(ShopManager.LimitedAttributeType.Ruby, 5000L),
						new ShopManager.LimitedItemSetData(ShopManager.LimitedAttributeType.TranscendStone, 100L)
					};
					Singleton<ShopManager>.instance.getLimitedBuyAction(new ShopManager.LimitedItemData(ShopManager.LimitedShopItemType.NewTranscendEventPackage, "$29.99", limitedAttribudeList, UnbiasedTime.Instance.Now().AddYears(1000).Ticks, 3, false), t, true)();
				}
			};
		case ItemType.DoubleSpeed:
			return delegate
			{
				long num = 0L;
				if (Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime > UnbiasedTime.Instance.Now().Ticks)
				{
					num = new DateTime(Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime).AddMinutes(itemValue).Ticks;
				}
				else
				{
					DateTime dateTime = new DateTime(UnbiasedTime.Instance.Now().Ticks);
					num = dateTime.AddMinutes(itemValue).Ticks;
				}
				Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime = num;
				Singleton<GameManager>.instance.refreshTimeScaleMiniPopup();
			};
		case ItemType.CountAutoOpenTreasureChest:
			return delegate
			{
				Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount += (int)itemValue;
				Singleton<TreasureChestManager>.instance.refreshAutoOpenTreasureChest();
			};
		case ItemType.AngelicaPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.AngelicaPackage], t);
		case ItemType.Colleague:
			return delegate
			{
				Singleton<ColleagueManager>.instance.getColleagueInventoryData((ColleagueManager.ColleagueType)(itemValue - 1.0)).isUnlocked = true;
				if (UIWindowColleague.instance.isOpen)
				{
					UIWindowColleague.instance.colleagueScrollRectParent.refreshAll();
				}
			};
		case ItemType.TimerAutoOpenTreasureChest:
			return delegate
			{
				long num2 = 0L;
				if (Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime > UnbiasedTime.Instance.Now().Ticks)
				{
					num2 = new DateTime(Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime).AddMinutes(itemValue).Ticks;
				}
				else
				{
					DateTime dateTime2 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
					num2 = dateTime2.AddMinutes(itemValue).Ticks;
				}
				Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime = num2;
				Singleton<TreasureChestManager>.instance.refreshAutoOpenTreasureChest();
			};
		case ItemType.CollectEventResource:
			return delegate
			{
				Singleton<CollectEventManager>.instance.increaseCollectEventResource((long)itemValue);
				Singleton<DataManager>.instance.saveData();
			};
		case ItemType.IceMeteorPassiveSkillPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.IceMeteorSkillPackage], t);
		case ItemType.AngelinaPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.AngelinaPackage], t);
		case ItemType.ReinforcementSkillPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.ReinforcementSkillPackage], t);
		case ItemType.ReinforcementSkill:
			return delegate
			{
				foreach (KeyValuePair<SkillManager.SkillType, ReinforcementSkillInventoryData> reinforcementSkillInventoryDatum in Singleton<DataManager>.instance.currentGameData.reinforcementSkillInventoryData)
				{
					if (!reinforcementSkillInventoryDatum.Value.isUnlocked)
					{
						reinforcementSkillInventoryDatum.Value.isUnlocked = true;
					}
				}
				if (Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.ReinforcementSkillPackage.ToString()))
				{
					Singleton<ShopManager>.instance.consumeLimitedItem(Singleton<ShopManager>.instance.getLimitedItemDataFromCurrentInventory(ShopManager.LimitedShopItemType.ReinforcementSkillPackage.ToString()));
				}
				if (UIWindowSkill.instance.isOpen)
				{
					UIWindowSkill.instance.skillScroll.refreshAll();
				}
			};
		case ItemType.BrotherhoodPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.BrotherhoodPackage], t);
		case ItemType.HeartForElopeMode:
			return delegate
			{
				Singleton<ElopeModeManager>.instance.increaseHeart(itemValue);
			};
		case ItemType.HeartCoinForElopeMode:
			return delegate
			{
				Singleton<ElopeModeManager>.instance.increaseHeartCoin((long)itemValue);
			};
		case ItemType.ElopeResources:
			return delegate
			{
				Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource1, (long)itemValue);
				Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource2, (long)itemValue);
				Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource3, (long)itemValue);
				Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource4, (long)itemValue);
			};
		case ItemType.HandsomeGuy:
			return delegate
			{
				Singleton<ElopeModeManager>.instance.castActiveSkill(ElopeModeManager.DaemonKingSkillType.HandsomeGuyDaemonKing, true);
			};
		case ItemType.SuperHandsomeGuy:
			return delegate
			{
				Singleton<ElopeModeManager>.instance.castActiveSkill(ElopeModeManager.DaemonKingSkillType.SuperHandsomeGuyDaemonKing, true);
			};
		case ItemType.SpeedGuy:
			return delegate
			{
				Singleton<ElopeModeManager>.instance.castActiveSkill(ElopeModeManager.DaemonKingSkillType.SpeedGuyDaemonKing, true);
			};
		case ItemType.SuperSpeedGuy:
			return delegate
			{
				Singleton<ElopeModeManager>.instance.castActiveSkill(ElopeModeManager.DaemonKingSkillType.SuperSpeedGuyDaemonKing, true);
			};
		case ItemType.DemonKingSkinPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.DemonKingSkinPackage], t);
		case ItemType.AngelaPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.AngelaPackage], t);
		case ItemType.GoblinPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.GoblinPackage], t);
		case ItemType.ZeusPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.ZeusPackage], t);
		case ItemType.ColleagueSkinPackageA:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.ColleagueSkinPackageA], t);
		case ItemType.EssentialPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.EssentialPackage], t);
		case ItemType.GoldRoulettePackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.GoldRoulettePackage], t);
		case ItemType.ColleagueSkinPackageB:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.ColleagueSkinPackageB], t);
		case ItemType.TowerModeFreeTicketPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.TowerModeFreeTicketPackage], t);
		case ItemType.ConquerorTokenTreasure:
			return delegate
			{
				double damageValue = Singleton<TreasureManager>.instance.getDamageValue(TreasureManager.TreasureType.ConquerToken);
				double treasureEffectValue = Singleton<TreasureManager>.instance.getTreasureEffectValue(TreasureManager.TreasureType.ConquerToken);
				TreasureInventoryData treasureInventoryData = new TreasureInventoryData
				{
					treasureType = TreasureManager.TreasureType.ConquerToken,
					treasureEffectValue = treasureEffectValue,
					damagePercentValue = damageValue,
					treasureLevel = 1L
				};
				TreasureValueData value = default(TreasureValueData);
				value.damagePercentValue = treasureInventoryData.damagePercentValue;
				value.treasureEffectValue = treasureInventoryData.treasureEffectValue;
				Singleton<TreasureManager>.instance.obtainTreasure(TreasureManager.TreasureType.ConquerToken, value);
				UIWindowTowerModeRankingReward.instance.openRankingReward(TowerModeManager.TowerModeRankingRewardType.Treasure_ConquerToken);
			};
		case ItemType.PatienceTokenTreasure:
			return delegate
			{
				double damageValue2 = Singleton<TreasureManager>.instance.getDamageValue(TreasureManager.TreasureType.PatienceToken);
				double treasureEffectValue2 = Singleton<TreasureManager>.instance.getTreasureEffectValue(TreasureManager.TreasureType.PatienceToken);
				TreasureInventoryData treasureInventoryData2 = new TreasureInventoryData
				{
					treasureType = TreasureManager.TreasureType.PatienceToken,
					treasureEffectValue = treasureEffectValue2,
					damagePercentValue = damageValue2,
					treasureLevel = 1L
				};
				TreasureValueData value2 = default(TreasureValueData);
				value2.damagePercentValue = treasureInventoryData2.damagePercentValue;
				value2.treasureEffectValue = treasureInventoryData2.treasureEffectValue;
				Singleton<TreasureManager>.instance.obtainTreasure(TreasureManager.TreasureType.PatienceToken, value2);
				UIWindowTowerModeRankingReward.instance.openRankingReward(TowerModeManager.TowerModeRankingRewardType.Treasure_PatienceToken);
			};
		case ItemType.TowerModeRankingTimeAttackCharacterSkin:
			return delegate
			{
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.ArcherSkinType.MasterWindstoker);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.ArcherSkinType.MasterWindstoker);
				UIWindowTowerModeRankingReward.instance.openRankingReward(TowerModeManager.TowerModeRankingRewardType.ArcherSkin);
			};
		case ItemType.TowerModeRankingEndlessCharacterSkin:
			return delegate
			{
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.PriestSkinType.MasterOlivia);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.PriestSkinType.MasterOlivia);
				UIWindowTowerModeRankingReward.instance.openRankingReward(TowerModeManager.TowerModeRankingRewardType.PriestSkin);
			};
		case ItemType.TowerModeRankingBothCharacterSkin:
			return delegate
			{
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.WarriorSkinType.MasterWilliam);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.WarriorSkinType.MasterWilliam);
				UIWindowTowerModeRankingReward.instance.openRankingReward(TowerModeManager.TowerModeRankingRewardType.WarriorSkin);
			};
		case ItemType.MasterSkinPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.MasterSkinPackage], t);
		case ItemType.WeaponSkinPiece:
			return delegate
			{
				Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.WeaponSkinPiece, Math.Min((long)itemValue, 30L), 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<WeaponSkinManager>.instance.increaseWeaponSkinPiece((long)itemValue);
			};
		case ItemType.WeaponSkinReinforcementMasterPiece:
			return delegate
			{
				Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.WeaponSkinReinforcementMasterPiece, Math.Min((long)itemValue, 30L), 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<WeaponSkinManager>.instance.increaseWeaponSkinReinforcementMasterPiece((long)itemValue);
			};
		case ItemType.WeaponSkinPremiumLottery:
			return delegate
			{
				WeaponSkinData weaponSkinData = Singleton<WeaponSkinManager>.instance.startLottery(true);
				if (UIWindowWeaponSkin.instance.isOpen)
				{
					UIWindowWeaponSkin.instance.openWeaponSkin(weaponSkinData.currentCharacterType);
				}
			};
		case ItemType.WeaponLegendarySkinPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.WeaponLegendarySkinPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.WeaponLegendarySkinPackage], t)();
				}
				else
				{
					List<ShopManager.LimitedItemSetData> limitedAttribudeList2 = new List<ShopManager.LimitedItemSetData>
					{
						new ShopManager.LimitedItemSetData(ShopManager.LimitedAttributeType.Ruby, 1000L),
						new ShopManager.LimitedItemSetData(ShopManager.LimitedAttributeType.WeaponSkinMasterPiece, 1000L),
						new ShopManager.LimitedItemSetData(ShopManager.LimitedAttributeType.RandomLegendaryWeaponSkin, 1L)
					};
					Singleton<ShopManager>.instance.getLimitedBuyAction(new ShopManager.LimitedItemData(ShopManager.LimitedShopItemType.WeaponLegendarySkinPackage, "$49.99", limitedAttribudeList2, UnbiasedTime.Instance.Now().AddYears(1000).Ticks, 3, false), t, true)();
				}
			};
		case ItemType.RandomCharacterSkinPackageA:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.RandomCharacterSkinPackageA], t);
		case ItemType.GoldenPackageA:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.GoldenPackageA], t);
		case ItemType.GoldenPackageB:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.GoldenPackageB], t);
		case ItemType.GoldenPackageC:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.GoldenPackageC], t);
		case ItemType.FlowerPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.FlowerPackage], t);
		case ItemType.PVPHonorToken:
			return delegate
			{
				Singleton<PVPManager>.instance.increasePVPHonorToken((long)itemValue);
			};
		case ItemType.TranscendPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.TranscendPackage], t);
		case ItemType.HonorPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.HonorPackage], t);
		case ItemType.DemonPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.DemonPackage], t);
		case ItemType.RandomHeroPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.RandomHeroPackage], t);
		case ItemType.RandomLimitedHeroPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.RandomLimitedHeroPackage], t);
		case ItemType.AFKPackage:
			return Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.AFKPackage], t);
		case ItemType.PVPCharacterPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.PVPCharacterPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.PVPCharacterPackage], t)();
				}
			};
		case ItemType.SuperAFKPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.SuperAFKPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.SuperAFKPackage], t)();
				}
			};
		case ItemType.UltraAFKPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.UltraAFKPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.UltraAFKPackage], t)();
				}
			};
		case ItemType.BronzeFinger:
			return delegate
			{
				Singleton<DataManager>.instance.currentGameData.isBoughtBronzeFinger = true;
				Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			};
		case ItemType.HotSummerPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.HotSummerPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.HotSummerPackage], t)();
				}
			};
		case ItemType.DemonCosplayPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.DemonCosplayPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.DemonCosplayPackage], t)();
				}
			};
		case ItemType.HalloweenPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.HalloweenPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.HalloweenPackage], t)();
				}
			};
		case ItemType.ForcePaidUser:
			return delegate
			{
				GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
				currentGameData.totalPurchasedMoney = (long)currentGameData.totalPurchasedMoney + 1;
				Singleton<LogglyManager>.instance.SendLoggly("UserID : " + Singleton<NanooAPIManager>.instance.UserID + ", TotalPurchasedMoney: " + Singleton<DataManager>.instance.currentGameData.totalPurchasedMoney, "Force PaidUser", LogType.Log);
			};
		case ItemType.XmasPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.XmasPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.XmasPackage], t)();
				}
			};
		case ItemType.March2018_FlowerPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.March2018_FlowerPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.March2018_FlowerPackage], t)();
				}
			};
		case ItemType.YummyWeaponSkinPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.YummyWeaponSkinPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.YummyWeaponSkinPackage], t)();
				}
			};
		case ItemType.WeddingSkinPackage:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.WeddingSkinPackage))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.WeddingSkinPackage], t)();
				}
			};
		case ItemType.ColleagueSkinPackageC:
			return delegate
			{
				if (Singleton<ShopManager>.instance.totalLimitedItemDataDictionary.ContainsKey(ShopManager.LimitedShopItemType.ColleagueSkinPackageC))
				{
					Singleton<ShopManager>.instance.getLimitedBuyAction(Singleton<ShopManager>.instance.totalLimitedItemDataDictionary[ShopManager.LimitedShopItemType.ColleagueSkinPackageC], t)();
				}
			};
		default:
			return null;
		}
	}

	public void OpenForum()
	{
		if (hasPlayNANOO && initPlayNANOO)
		{
			try
			{
				playNANOO.OpenForum();
			}
			catch (NullReferenceException message)
			{
				Debug.Log(message);
				Debug.LogError("Please check your device");
			}
		}
	}

	public void OpenForumView(string url)
	{
		if (hasPlayNANOO && initPlayNANOO && !string.IsNullOrEmpty(url))
		{
			try
			{
				playNANOO.OpenForumView(url);
			}
			catch (NullReferenceException message)
			{
				Debug.Log(message);
				Debug.LogError("Please check your device");
			}
		}
	}

	public void OpenHelp()
	{
		if (hasPlayNANOO && initPlayNANOO)
		{
			try
			{
				playNANOO.OpenHelp();
			}
			catch (NullReferenceException message)
			{
				Debug.Log(message);
				Debug.LogError("Please check your device");
			}
		}
	}

	public void ForumThread()
	{
		if (!hasPlayNANOO || !initPlayNANOO)
		{
			return;
		}
		try
		{
			string iSOforForum = GetISOforForum();
			string section = "sticky";
			short limit = 3;
			playNANOO.ForumThread(iSOforForum, section, limit, delegate(Dictionary<string, object> dic)
			{
				if (_noticeList == null)
				{
					_noticeList = new List<NoticeItem>();
				}
				_noticeList.Clear();
				if (dic.Count > 0 || dic.ContainsKey("status"))
				{
					if (dic["status"].Equals(ResultType.SUCCESS))
					{
						JSONArray jSONArray = (JSONArray)dic["thread"];
						if (jSONArray.Count > 0)
						{
							for (int i = 0; i < jSONArray.Count; i++)
							{
								JSONNode jSONNode = jSONArray[i];
								string value = jSONNode["title"].Value;
								string value2 = jSONNode["url"].Value;
								string value3 = jSONNode["summary"].Value;
								string[] array = value3.Split('\u00a0');
								string text = string.Empty;
								string[] array2 = array;
								foreach (string text2 in array2)
								{
									if (text2.Trim().Length > 0)
									{
										text = text.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"")
											.Replace("&apos;", "'")
											.Replace("&amp;", "&");
										text = ((text2.Length <= 46) ? text2 : (text2.Substring(0, 45) + "..."));
										Debug.Log("No." + i + "  length:" + text2.Length);
										break;
									}
								}
								if (string.IsNullOrEmpty(eventBannerUrl))
								{
									eventBannerUrl = value2;
									Debug.Log("eventBannerUrl: " + eventBannerUrl);
								}
								NoticeItem item = new NoticeItem
								{
									header = value.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"")
										.Replace("&apos;", "'")
										.Replace("&amp;", "&"),
									content = text,
									url = value2
								};
								_noticeList.Add(item);
							}
						}
					}
					else if (dic["status"].Equals(ResultType.UPDATE))
					{
						Debug.Log(string.Format("message=>{0}", dic["message"]));
					}
					else if (dic["status"].Equals(ResultType.ERROR))
					{
						Debug.Log(string.Format("message=>{0}", dic["message"]));
					}
					else if (dic["status"].Equals(ResultType.INTERNET))
					{
						Debug.Log(string.Format("message=>{0}", dic["message"]));
					}
				}
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.LogError("Please check your device");
		}
		finally
		{
			Debug.LogError(isNoticeComplete);
			isNoticeComplete = true;
		}
	}

	public void ReceiptVerificationAndroid(string productID, string purchase, string signature, string currency, double price)
	{
		//if (hasPlayNANOO && initPlayNANOO)
		//{
		//	try
		//	{
		//		playNANOO.ReceiptVerification(productID, purchase, signature, currency, price, OnReceiptVerification);
		//		isReceiptValidatorComplete = false;
		//	}
		//	catch (NullReferenceException message)
		//	{
		//		Debug.Log(message);
		//		Debug.LogError("Please check your device");
		//	}
		//}
		Singleton<PaymentManager>.instance.ProcessingPurchase(productID);
		
		// Đóng loading screen sau khi xử lý thanh toán hoàn tất
		if (UIWindowLoading.instance != null)
		{
			UIWindowLoading.instance.closeLoadingUI();
		}
	}

	public void ReceiptVerificationIOS(string productID, string receipt, string currency, double price)
	{
		if (hasPlayNANOO && initPlayNANOO)
		{
		}
	}

	private void OnReceiptVerification(Dictionary<string, object> dic)
	{
		Singleton<LogglyManager>.instance.SendLoggly("Response ReceiptCheck : " + dic["status"].ToString(), "NanooAPIManager:RECEIPT_VALIDATION", LogType.Log);
		if (dic["status"].Equals(ResultType.SUCCESS))
		{
			Debug.Log(string.Format("package=>{0}", dic["package"].ToString()));
			Debug.Log(string.Format("product_id=>{0}", dic["product_id"].ToString()));
			Debug.Log(string.Format("order_id=>{0}", dic["order_id"].ToString()));
			playNANOOHandler.OnReceiptDelegate = delegate(Dictionary<string, object> result)
			{
				if (result["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log("success");
				}
				else if (result["status"].Equals(ResultType.UPDATE))
				{
					Debug.Log(string.Format("message=>{0}", result["message"]));
				}
				else if (result["status"].Equals(ResultType.ERROR))
				{
					Debug.Log(string.Format("message=>{0}", result["message"]));
				}
				else if (result["status"].Equals(ResultType.INTERNET))
				{
					Debug.Log(string.Format("message=>{0}", result["message"]));
				}
			};
			Singleton<PaymentManager>.instance.LogAnswer(true, dic["product_id"].ToString());
			Singleton<PaymentManager>.instance.ProcessingPurchase(dic["product_id"].ToString());
		}
		else
		{
			if (dic["status"].Equals(ResultType.UPDATE))
			{
				Debug.Log(string.Format("message=>{0}", dic["message"]));
			}
			else if (dic["status"].Equals(ResultType.ERROR))
			{
				Debug.Log(string.Format("message=>{0}", dic["message"]));
			}
			else if (dic["status"].Equals(ResultType.INTERNET))
			{
				Debug.Log(string.Format("message=>{0}", dic["message"]));
			}
			Singleton<PaymentManager>.instance.LogAnswer(false, _receiptValidatorResponse.product_id);
		}
		if (UIWindowLoading.instance != null)
		{
			UIWindowLoading.instance.closeLoadingUI();
		}
		isReceiptValidatorComplete = true;
	}
}
