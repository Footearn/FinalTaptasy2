namespace PlayNANOO
{
	public static class Configure
	{
		public static readonly string MSG_ERROR = "error";

		public static readonly string MSG_ACCESS_TOKEN = "Please check access token";

		public static readonly string MSG_HASH = "Please check hash";

		public static readonly string MSG_INTERNET = "Please check your internet connection";

		public static readonly string MSG_UNKNOWN = "Unknown error";

		public static readonly string ACCESS_TOKEN = "PlayNANOOAccessToken";

		public static readonly string API_VERSION = "v20180501";

		public static readonly string HOST_PLAYNANOO_API = "https://api.playnanoo.com";

		public static readonly string HOST_PLAYNANOO_STORAGE_API = "https://api.playnanoo.com";

		public static readonly float WWW_TIMEOUT = 10f;

		public static readonly string URL_PLAYNANOO_API_ACCESS_INIT = string.Format("{0}/{1}/access/init", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_ADID = string.Format("{0}/{1}/access/adid", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_IDFA = string.Format("{0}/{1}/access/idfa", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_FORUM_THREAD = string.Format("{0}/{1}/forum/thread", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_COUPON_USE = string.Format("{0}/{1}/coupon/code_use", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_POSTBOX_ITEM = string.Format("{0}/{1}/postbox/item", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_POSTBOX_ITEM_USE = string.Format("{0}/{1}/postbox/item_use", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_POSTBOX_ITEM_SEND = string.Format("{0}/{1}/postbox/item_send", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_POSTBOX_FRIEND_ITEM_SEND = string.Format("{0}/{1}/postbox/item_send_friend", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_POSTBOX_CLEAR = string.Format("{0}/{1}/postbox/clear", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_POSTBOX_SUBSCRIPTION_REGISTER = string.Format("{0}/{1}/postbox/subscription_register", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_RECEIPT_AOS = string.Format("{0}/{1}/purchase/receipt_aos", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_RECEIPT_AOS_NOSKU = string.Format("{0}/{1}/purchase/receipt_aos_nosku", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_RECEIPT_IOS = string.Format("{0}/{1}/purchase/receipt_ios", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_STORAGE_LOAD = string.Format("{0}/{1}/storage/load", HOST_PLAYNANOO_STORAGE_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_STORAGE_SAVE = string.Format("{0}/{1}/storage/save", HOST_PLAYNANOO_STORAGE_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_RANKING_RECORD = string.Format("{0}/{1}/ranking/record", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_RANKING_ORDER = string.Format("{0}/{1}/ranking/order", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_RANKING_MY = string.Format("{0}/{1}/ranking/my_ranking", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_RANKING_TOP_RECORD = string.Format("{0}/{1}/ranking/top_record", HOST_PLAYNANOO_API, API_VERSION);

		public static readonly string URL_PLAYNANOO_API_RANKING_SEASON_INFO = string.Format("{0}/{1}/ranking/season_info", HOST_PLAYNANOO_API, API_VERSION);
	}
}
