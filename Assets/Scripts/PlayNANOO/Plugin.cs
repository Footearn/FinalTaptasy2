using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace PlayNANOO
{
	public class Plugin : MonoBehaviour
	{
		private delegate void PluginDelegate(string value);

		private delegate void EventHandlerDelegate(string value, PlayNANOODelegate callback);

		private static string _gameUUID;

		private static string _gameNickname;

		private static string _platform;

		private static int _versionCode;

		private static Dictionary<string, object> _optional = new Dictionary<string, object>();

		private Dictionary<string, string> _storageData = new Dictionary<string, string>();

		private EventHandler _playNANOOHandler;

		private AndroidJavaObject bridgeClass;

		private void Awake()
		{
			_playNANOOHandler = GameObject.Find("PlayNANOO").GetComponent<EventHandler>();
			try
			{
				bridgeClass = new AndroidJavaClass("com.playnanoo.plugin.Plugin");
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
		}

		public void SetInfo(string uuid, string nickname, int versionCode)
		{
			_gameUUID = uuid;
			_gameNickname = nickname;
			_versionCode = versionCode;
			_platform = "EDITOR";
			_platform = "AOS";
		}

		public void SetHelpOptional(string key, string value)
		{
			if (_optional.ContainsKey(key))
			{
				_optional[key] = value;
			}
			else
			{
				_optional.Add(key, value);
			}
		}

		public void OpenScreenshot()
		{
			_playNANOOHandler.OnScreenshotDelegate = OpenScreenshot;
			_playNANOOHandler.Screenshot();
		}

		private bool AccessTokenValidation()
		{
			if (!PlayerPrefs.HasKey(Configure.ACCESS_TOKEN))
			{
				Init(false);
			}
			return true;
		}

		public void Init(bool isSaveID)
		{
			try
			{
				int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
				string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
				string value = HashMessage(message, Setting.SECRET_KEY);
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("uuid", _gameUUID);
				wWWForm.AddField("nickname", _gameNickname);
				wWWForm.AddField("version", _versionCode.ToString());
				wWWForm.AddField("platform", _platform);
				wWWForm.AddField("ts", num.ToString());
				wWWForm.AddField("hash", value);
				StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_ACCESS_INIT, wWWForm, false, false, _playNANOOHandler.OnInitReceive));
				if (isSaveID)
				{

				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
		}

		public void Init(bool isSaveID, PlayNANOODelegate callback)
		{
			try
			{
				int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
				string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
				string value = HashMessage(message, Setting.SECRET_KEY);
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("uuid", _gameUUID);
				wWWForm.AddField("nickname", _gameNickname);
				wWWForm.AddField("version", _versionCode.ToString());
				wWWForm.AddField("platform", _platform);
				wWWForm.AddField("ts", num.ToString());
				wWWForm.AddField("hash", value);
				StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_ACCESS_INIT, wWWForm, false, false, _playNANOOHandler.OnInitReceive, callback));
				if (isSaveID)
				{

				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
		}

		public void CouponCheck(string code)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, code, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("code", code);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_COUPON_USE, wWWForm, true, true, _playNANOOHandler.OnCouponCheckReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void CouponCheck(string code, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, code, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("code", code);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_COUPON_USE, wWWForm, true, true, _playNANOOHandler.OnCouponCheckReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxItem()
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_ITEM, wWWForm, true, true, _playNANOOHandler.OnPostboxItemReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxItem(PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_ITEM, wWWForm, true, true, _playNANOOHandler.OnPostboxItemReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxItemUse(string itemID)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, itemID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", itemID);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_ITEM_USE, wWWForm, true, true, _playNANOOHandler.OnPostboxItemUseReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxItemUse(string itemID, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, itemID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", itemID);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_ITEM_USE, wWWForm, true, true, _playNANOOHandler.OnPostboxItemUseReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxItemSend(string itemCode, int itemCount, short itemExpireDay, string itemMessage)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}{5}{6}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, itemCode, itemCount.ToString(), itemExpireDay.ToString(), num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("item_code", itemCode);
					wWWForm.AddField("item_count", itemCount.ToString());
					if (itemMessage != null)
					{
						wWWForm.AddField("item_message", itemMessage);
					}
					wWWForm.AddField("item_expire_day", itemExpireDay.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_ITEM_SEND, wWWForm, true, true, _playNANOOHandler.OnPostboxItemSendReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxItemSend(string itemCode, int itemCount, short itemExpireDay, string itemMessage, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}{5}{6}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, itemCode, itemCount.ToString(), itemExpireDay.ToString(), num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("item_code", itemCode);
					wWWForm.AddField("item_count", itemCount.ToString());
					if (itemMessage != null)
					{
						wWWForm.AddField("item_message", itemMessage);
					}
					wWWForm.AddField("item_expire_day", itemExpireDay.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_ITEM_SEND, wWWForm, true, true, _playNANOOHandler.OnPostboxItemSendReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxFriendItemSend(string friendUUID, string itemCode, int itemCount, short itemExpireDay, string itemMessage)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, friendUUID, itemCode, itemCount.ToString(), itemExpireDay.ToString(), num);
					Debug.LogWarning(message);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("friend_uuid", friendUUID);
					wWWForm.AddField("item_code", itemCode);
					wWWForm.AddField("item_count", itemCount.ToString());
					if (itemMessage != null)
					{
						wWWForm.AddField("item_message", itemMessage);
					}
					wWWForm.AddField("item_expire_day", itemExpireDay.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_FRIEND_ITEM_SEND, wWWForm, true, true, _playNANOOHandler.OnPostboxFriendItemSendReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxFriendItemSend(string friendUUID, string itemCode, int itemCount, short itemExpireDay, string itemMessage, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, friendUUID, itemCode, itemCount.ToString(), itemExpireDay.ToString(), num);
					Debug.LogWarning(message);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("friend_uuid", friendUUID);
					wWWForm.AddField("item_code", itemCode);
					wWWForm.AddField("item_count", itemCount.ToString());
					if (itemMessage != null)
					{
						wWWForm.AddField("item_message", itemMessage);
					}
					wWWForm.AddField("item_expire_day", itemExpireDay.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_FRIEND_ITEM_SEND, wWWForm, true, true, _playNANOOHandler.OnPostboxFriendItemSendReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxClear()
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_CLEAR, wWWForm, true, true, _playNANOOHandler.OnPostboxClearReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxClear(PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_CLEAR, wWWForm, true, true, _playNANOOHandler.OnPostboxClearReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxSubscriptionRegister(string product)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, product, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("product_uid", product);
					wWWForm.AddField("offset", TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalSeconds.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_SUBSCRIPTION_REGISTER, wWWForm, true, true, _playNANOOHandler.OnPostboxPostboxSubscriptionRegisterReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void PostboxSubscriptionRegister(string product, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, product, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("product_uid", product);
					wWWForm.AddField("offset", TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalSeconds.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_POSTBOX_SUBSCRIPTION_REGISTER, wWWForm, true, true, _playNANOOHandler.OnPostboxPostboxSubscriptionRegisterReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void ForumThread(string language, string section, short limit)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("language", language);
					wWWForm.AddField("section", section);
					wWWForm.AddField("limit", limit.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_FORUM_THREAD, wWWForm, true, true, _playNANOOHandler.OnForumThreadReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void ForumThread(string language, string section, short limit, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("language", language);
					wWWForm.AddField("section", section);
					wWWForm.AddField("limit", limit.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_FORUM_THREAD, wWWForm, true, true, _playNANOOHandler.OnForumThreadReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		[Obsolete("Do not call this method.")]
		public void SetStorageData(string column, string value)
		{
			_storageData.Add(column, value);
		}

		[Obsolete("Do not call this method.")]
		public void StorageSave(string key, bool isPrivate)
		{
			try
			{
				if (AccessTokenValidation())
				{
					if (_storageData.Count <= 0)
					{
						throw new Exception("Please check data");
					}
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("skey", key);
					if (isPrivate)
					{
						wWWForm.AddField("private", "on");
					}
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					foreach (KeyValuePair<string, string> storageDatum in _storageData)
					{
						wWWForm.AddField(string.Format("_pns_{0}", storageDatum.Key), storageDatum.Value.ToString());
					}
					List<string> list = new List<string>(_storageData.Keys);
					foreach (string item in list)
					{
						wWWForm.AddField("column[]", item);
					}
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_STORAGE_SAVE, wWWForm, true, true, _playNANOOHandler.OnStorageSaveReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
				_storageData.Clear();
			}
		}

		public void StorageSave(string key, string value, bool isPrivate)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value2 = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("skey", key);
					wWWForm.AddField("svalue", value);
					if (isPrivate)
					{
						wWWForm.AddField("private", "on");
					}
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value2);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_STORAGE_SAVE, wWWForm, true, true, _playNANOOHandler.OnStorageSaveReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
				_storageData.Clear();
			}
		}

		public void StorageSave(string key, string value, bool isPrivate, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value2 = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("skey", key);
					wWWForm.AddField("svalue", value);
					if (isPrivate)
					{
						wWWForm.AddField("private", "on");
					}
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value2);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_STORAGE_SAVE, wWWForm, true, true, _playNANOOHandler.OnStorageSaveReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
				_storageData.Clear();
			}
		}

		public void StorageLoad(string key)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("skey", key);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_STORAGE_LOAD, wWWForm, true, true, _playNANOOHandler.OnStorageLoadReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void StorageLoad(string key, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("skey", key);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_STORAGE_LOAD, wWWForm, true, true, _playNANOOHandler.OnStorageLoadReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		[Obsolete("Do not call this method.")]
		public void StorageLoad(string key, string column)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("skey", key);
					wWWForm.AddField("column", column);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_STORAGE_LOAD, wWWForm, true, true, _playNANOOHandler.OnStorageLoadReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void RankingSeasonInfo(string uid, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, uid, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", uid);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RANKING_SEASON_INFO, wWWForm, true, true, _playNANOOHandler.OnRankingSeasonInfoReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void RankingRecord(string uid, string country, int score, string data, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}{5}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, uid, score.ToString(), num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", uid);
					wWWForm.AddField("country", country);
					wWWForm.AddField("score", score.ToString());
					wWWForm.AddField("player_data", data);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RANKING_RECORD, wWWForm, true, true, _playNANOOHandler.OnRankingRecordReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void Ranking(string uid, int limit, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, uid, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", uid);
					wWWForm.AddField("limit", limit.ToString());
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RANKING_ORDER, wWWForm, true, true, _playNANOOHandler.OnRankingReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void Ranking(string uid, int season, int limit, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, uid, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", uid);
					wWWForm.AddField("season", season.ToString());
					wWWForm.AddField("limit", limit.ToString());
					wWWForm.AddField("request", "season");
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RANKING_ORDER, wWWForm, true, true, _playNANOOHandler.OnRankingReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void Ranking(string uid, int season, string country, int limit, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, uid, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", uid);
					wWWForm.AddField("season", season.ToString());
					wWWForm.AddField("country", country);
					wWWForm.AddField("limit", limit.ToString());
					wWWForm.AddField("request", "season_country");
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RANKING_ORDER, wWWForm, true, true, _playNANOOHandler.OnRankingReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void RankingMy(string uid, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, uid, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", uid);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RANKING_MY, wWWForm, true, true, _playNANOOHandler.OnRankingMyReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void RankingMy(string uid, int season, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, uid, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", uid);
					wWWForm.AddField("season", season.ToString());
					wWWForm.AddField("request", "season");
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RANKING_MY, wWWForm, true, true, _playNANOOHandler.OnRankingMyReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void RankingMy(string uid, int season, string country, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, uid, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", uid);
					wWWForm.AddField("season", season.ToString());
					wWWForm.AddField("country", country);
					wWWForm.AddField("request", "season_country");
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RANKING_MY, wWWForm, true, true, _playNANOOHandler.OnRankingMyReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void RankingTopRecord(string uid, int limit, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, uid, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("uid", uid);
					wWWForm.AddField("limit", limit.ToString());
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RANKING_TOP_RECORD, wWWForm, true, true, _playNANOOHandler.OnRankingTopRecordReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void OnADIDReceive(string result)
		{
			try
			{
				if (AccessTokenValidation())
				{
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("adid", result);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_ADID, wWWForm, false, false, null));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
			finally
			{
			}
		}

		public void OpenForum()
		{
			try
			{
				if (AccessTokenValidation())
				{

				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
			finally
			{
			}
		}

		public void OpenForumView(string url)
		{
			try
			{
				if (AccessTokenValidation())
				{

				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message.ToString());
			}
			finally
			{
			}
		}

		public void OpenHelp()
		{
			try
			{
				if (AccessTokenValidation())
				{

				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
			finally
			{
			}
		}

		public void OpenScreenshot(byte[] result)
		{
			try
			{
				if (AccessTokenValidation())
				{

				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
			finally
			{
			}
		}

		public void ReceiptVerification(string productID, string purchase, string signature, string sku)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, productID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("product_id", productID);
					wWWForm.AddField("purchase", purchase);
					wWWForm.AddField("signature", signature);
					wWWForm.AddField("sku", sku);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RECEIPT_AOS, wWWForm, true, true, _playNANOOHandler.OnReceiptReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void ReceiptVerification(string productID, string purchase, string signature, string sku, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, productID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("product_id", productID);
					wWWForm.AddField("purchase", purchase);
					wWWForm.AddField("signature", signature);
					wWWForm.AddField("sku", sku);
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RECEIPT_AOS, wWWForm, true, true, _playNANOOHandler.OnReceiptReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void ReceiptVerification(string productID, string purchase, string signature, string currency, double price)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, productID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("product_id", productID);
					wWWForm.AddField("purchase", purchase);
					wWWForm.AddField("signature", signature);
					wWWForm.AddField("currency", currency);
					wWWForm.AddField("price", price.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RECEIPT_AOS_NOSKU, wWWForm, true, true, _playNANOOHandler.OnReceiptReceive));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		public void ReceiptVerification(string productID, string purchase, string signature, string currency, double price, PlayNANOODelegate callback)
		{
			try
			{
				if (AccessTokenValidation())
				{
					int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					string message = string.Format("{0}{1}{2}{3}{4}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, _gameUUID, productID, num);
					string value = HashMessage(message, Setting.SECRET_KEY);
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("uuid", _gameUUID);
					wWWForm.AddField("nickname", _gameNickname);
					wWWForm.AddField("product_id", productID);
					wWWForm.AddField("purchase", purchase);
					wWWForm.AddField("signature", signature);
					wWWForm.AddField("currency", currency);
					wWWForm.AddField("price", price.ToString());
					wWWForm.AddField("version", _versionCode.ToString());
					wWWForm.AddField("platform", _platform);
					wWWForm.AddField("ts", num.ToString());
					wWWForm.AddField("hash", value);
					StartCoroutine(HTTPPost(Configure.URL_PLAYNANOO_API_RECEIPT_AOS_NOSKU, wWWForm, true, true, _playNANOOHandler.OnReceiptReceive, callback));
				}
				else
				{
					Debug.LogWarning(Configure.MSG_ACCESS_TOKEN);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
			finally
			{
			}
		}

		private string HashMessage(string message, string secret)
		{
			//Discarded unreachable code: IL_0043
			secret = secret ?? string.Empty;
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			byte[] bytes = aSCIIEncoding.GetBytes(secret);
			byte[] bytes2 = aSCIIEncoding.GetBytes(message);
			using (HMACSHA256 hMACSHA = new HMACSHA256(bytes))
			{
				byte[] inArray = hMACSHA.ComputeHash(bytes2);
				return Convert.ToBase64String(inArray);
			}
		}

		private IEnumerator HTTPPost(string url, WWWForm form, bool isAccessToken, bool isWait, PluginDelegate callback)
		{
			if (isWait && !PlayerPrefs.HasKey(Configure.ACCESS_TOKEN))
			{
				yield return new WaitForSeconds(5f);
			}
			float request_timer = 0f;
			bool request_failed = false;
			Dictionary<string, string> headers = new Dictionary<string, string>();
			if (isAccessToken)
			{
				headers.Add("Authorization", string.Format("Bearer {0}", PlayerPrefs.GetString(Configure.ACCESS_TOKEN)));
			}
			headers.Add("x-playnanoo-key", Setting.SERVICE_KEY);
			if (!string.IsNullOrEmpty(Singleton<NanooAPIManager>.instance.udid))
			{
				headers.Add("x-playnanoo-device", Singleton<NanooAPIManager>.instance.udid);
			}
			else
			{
				headers.Add("x-playnanoo-device", string.Empty);
			}
			headers.Add("x-playnanoo-id", Setting.CHANNEL_ID);
			byte[] postData = form.data;
			WWW www = new WWW(url, postData, headers);
			while (!www.isDone)
			{
				if (request_timer > Configure.WWW_TIMEOUT)
				{
					request_failed = true;
					break;
				}
				request_timer += Time.deltaTime;
				yield return null;
			}
			if (request_failed)
			{
				www.Dispose();
				callback(Configure.MSG_ERROR);
				yield break;
			}
			Debug.Log(www.text);
			if (www.error == null)
			{
				callback(www.text);
			}
			else
			{
				callback(Configure.MSG_ERROR);
			}
		}

		private IEnumerator HTTPPost(string url, WWWForm form, bool isAccessToken, bool isWait, EventHandlerDelegate callback, PlayNANOODelegate eventCallback)
		{
			if (isWait && !PlayerPrefs.HasKey(Configure.ACCESS_TOKEN))
			{
				yield return new WaitForSeconds(5f);
			}
			float request_timer = 0f;
			bool request_failed = false;
			Dictionary<string, string> headers = new Dictionary<string, string>();
			if (isAccessToken)
			{
				headers.Add("Authorization", string.Format("Bearer {0}", PlayerPrefs.GetString(Configure.ACCESS_TOKEN)));
			}
			headers.Add("x-playnanoo-key", Setting.SERVICE_KEY);
			if (!string.IsNullOrEmpty(Singleton<NanooAPIManager>.instance.udid))
			{
				headers.Add("x-playnanoo-device", Singleton<NanooAPIManager>.instance.udid);
			}
			else
			{
				headers.Add("x-playnanoo-device", string.Empty);
			}
			headers.Add("x-playnanoo-id", Setting.CHANNEL_ID);
			byte[] postData = form.data;
			WWW www = new WWW(url, postData, headers);
			while (!www.isDone)
			{
				if (request_timer > Configure.WWW_TIMEOUT)
				{
					request_failed = true;
					break;
				}
				request_timer += Time.deltaTime;
				yield return null;
			}
			if (request_failed)
			{
				www.Dispose();
				if (callback != null)
				{
					callback(Configure.MSG_ERROR, eventCallback);
				}
				yield break;
			}
			Debug.Log(www.text);
			if (www.error == null)
			{
				if (callback != null)
				{
					callback(www.text, eventCallback);
				}
			}
			else if (callback != null)
			{
				callback(Configure.MSG_ERROR, eventCallback);
			}
		}
	}
}
