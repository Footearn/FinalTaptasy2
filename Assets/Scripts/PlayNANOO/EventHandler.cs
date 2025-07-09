using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayNANOO
{
	public class EventHandler : MonoBehaviour
	{
		public PlayNANOOScreenshotDelegate OnScreenshotDelegate = OnScreenshotResult;

		public PlayNANOODelegate OnAccessDelegate = OnBasicResult;

		public PlayNANOODelegate OnCouponDelegate = OnBasicResult;

		public PlayNANOODelegate OnPostboxDelegate = OnBasicResult;

		public PlayNANOODelegate OnReceiptDelegate = OnBasicResult;

		public PlayNANOODelegate OnForumDelegate = OnBasicResult;

		public PlayNANOODelegate OnStorageDelegate = OnBasicResult;

		private static void OnScreenshotResult(byte[] result)
		{
		}

		private static void OnBasicResult(Dictionary<string, object> result)
		{
		}

		private bool ValidationHash(string message, string hash)
		{
			if (hash.Equals(Util.HashMessage(message, Setting.SECRET_KEY)))
			{
				return true;
			}
			return false;
		}

		private Dictionary<string, object> OnReceiveController(string result)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (result.Equals(Configure.MSG_ERROR))
			{
				dictionary.Add("status", ResultType.ERROR);
				dictionary.Add("message", Configure.MSG_UNKNOWN);
			}
			else
			{
				JSONNode jSONNode = JSON.Parse(result);
				if (jSONNode["error"] == null)
				{
					dictionary.Add("status", ResultType.SUCCESS);
					dictionary.Add("data", jSONNode);
				}
				else
				{
					switch ((int)jSONNode["error"]["http_code"])
					{
					case 400:
					case 401:
					case 403:
						dictionary.Add("status", ResultType.ERROR);
						dictionary.Add("message", (string)jSONNode["error"]["message_code"]);
						break;
					case 503:
						dictionary.Add("status", ResultType.UPDATE);
						dictionary.Add("message", (string)jSONNode["error"]["message_code"]);
						break;
					default:
						dictionary.Add("status", ResultType.FAILED);
						dictionary.Add("message", Configure.MSG_UNKNOWN);
						break;
					}
				}
			}
			return dictionary;
		}

		private Dictionary<string, object> OnInternetConnection()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("status", ResultType.INTERNET);
			dictionary.Add("message", Configure.MSG_INTERNET);
			return dictionary;
		}

		public void OnInitReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)dictionary["data"];
				dictionary.Add("token", jSONNode["token"].Value);
				dictionary.Add("time", jSONNode["time"].AsDouble);
				if (jSONNode["postbox_count"] != null)
				{
					dictionary.Add("postbox_count", jSONNode["postbox_count"].AsInt);
				}
				if (jSONNode["postbox_subscription"] != null)
				{
					ArrayList arrayList = new ArrayList();
					for (int i = 0; i < jSONNode["postbox_subscription"].AsArray.Count; i++)
					{
						Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
						dictionary2.Add("product", jSONNode["postbox_subscription"][i]["product"].Value);
						dictionary2.Add("ttl", jSONNode["postbox_subscription"][i]["ttl"].AsDouble);
						arrayList.Add(dictionary2);
					}
					dictionary.Add("postbox_subscription", arrayList);
				}
				dictionary.Remove("data");
				PlayerPrefs.SetString(Configure.ACCESS_TOKEN, jSONNode["token"]);
			}
			OnAccessDelegate(dictionary);
		}

		public void OnInitReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)dictionary["data"];
				dictionary.Add("token", jSONNode["token"].Value);
				dictionary.Add("time", jSONNode["time"].AsDouble);
				if (jSONNode["postbox_count"] != null)
				{
					dictionary.Add("postbox_count", jSONNode["postbox_count"].AsInt);
				}
				if (jSONNode["postbox_subscription"] != null)
				{
					ArrayList arrayList = new ArrayList();
					for (int i = 0; i < jSONNode["postbox_subscription"].AsArray.Count; i++)
					{
						Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
						dictionary2.Add("product", jSONNode["postbox_subscription"][i]["product"].Value);
						dictionary2.Add("ttl", jSONNode["postbox_subscription"][i]["ttl"].AsDouble);
						arrayList.Add(dictionary2);
					}
					dictionary.Add("postbox_subscription", arrayList);
				}
				dictionary.Remove("data");
				PlayerPrefs.SetString(Configure.ACCESS_TOKEN, jSONNode["token"]);
			}
			callback(dictionary);
		}

		public void OnCouponCheckReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			try
			{
				if (dictionary["status"].Equals(ResultType.SUCCESS))
				{
					JSONNode jSONNode = (JSONNode)dictionary["data"];
					dictionary.Add("item_code", jSONNode["item_code"].Value);
					dictionary.Add("item_count", (int)jSONNode["item_count"]);
					dictionary.Remove("data");
					if (jSONNode["hash"] != null)
					{
						string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, jSONNode["uuid"].Value, jSONNode["item_code"].Value);
						if (!ValidationHash(message, jSONNode["hash"]))
						{
							throw new Exception(Configure.MSG_HASH);
						}
					}
				}
			}
			catch (Exception ex)
			{
				dictionary.Clear();
				dictionary.Add("status", ResultType.HASH);
				dictionary.Add("message", ex.Message);
			}
			finally
			{
				OnCouponDelegate(dictionary);
			}
		}

		public void OnCouponCheckReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			try
			{
				if (dictionary["status"].Equals(ResultType.SUCCESS))
				{
					JSONNode jSONNode = (JSONNode)dictionary["data"];
					dictionary.Add("item_code", jSONNode["item_code"].Value);
					dictionary.Add("item_count", (int)jSONNode["item_count"]);
					dictionary.Remove("data");
					if (jSONNode["hash"] != null)
					{
						string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, jSONNode["uuid"].Value, jSONNode["item_code"].Value);
						if (!ValidationHash(message, jSONNode["hash"]))
						{
							throw new Exception(Configure.MSG_HASH);
						}
					}
				}
			}
			catch (Exception ex)
			{
				dictionary.Clear();
				dictionary.Add("status", ResultType.HASH);
				dictionary.Add("message", ex.Message);
			}
			finally
			{
				callback(dictionary);
			}
		}

		public void OnForumThreadReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)(dictionary["thread"] = (JSONNode)dictionary["data"]);
				dictionary.Remove("data");
			}
			OnForumDelegate(dictionary);
		}

		public void OnForumThreadReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)(dictionary["thread"] = (JSONNode)dictionary["data"]);
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnReceiptReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			try
			{
				if (dictionary["status"].Equals(ResultType.SUCCESS))
				{
					JSONNode jSONNode = (JSONNode)dictionary["data"];
					dictionary.Add("package", jSONNode["package"].Value);
					dictionary.Add("product_id", jSONNode["product_id"].Value);
					dictionary.Add("order_id", jSONNode["order_id"].Value);
					dictionary.Remove("data");
					if (jSONNode["hash"] != null)
					{
						string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, jSONNode["uuid"].Value, jSONNode["order_id"].Value);
						if (!ValidationHash(message, jSONNode["hash"]))
						{
							throw new Exception(Configure.MSG_HASH);
						}
					}
				}
			}
			catch (Exception ex)
			{
				dictionary.Clear();
				dictionary.Add("status", ResultType.HASH);
				dictionary.Add("message", ex.Message);
			}
			finally
			{
				OnReceiptDelegate(dictionary);
			}
		}

		public void OnReceiptReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			try
			{
				if (dictionary["status"].Equals(ResultType.SUCCESS))
				{
					JSONNode jSONNode = (JSONNode)dictionary["data"];
					dictionary.Add("package", jSONNode["package"].Value);
					dictionary.Add("product_id", jSONNode["product_id"].Value);
					dictionary.Add("order_id", jSONNode["order_id"].Value);
					dictionary.Remove("data");
					if (jSONNode["hash"] != null)
					{
						string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, jSONNode["uuid"].Value, jSONNode["order_id"].Value);
						if (!ValidationHash(message, jSONNode["hash"]))
						{
							throw new Exception(Configure.MSG_HASH);
						}
					}
				}
			}
			catch (Exception ex)
			{
				dictionary.Clear();
				dictionary.Add("status", ResultType.HASH);
				dictionary.Add("message", ex.Message);
			}
			finally
			{
				callback(dictionary);
			}
		}

		public void OnPostboxItemReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)dictionary["data"];
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < jSONNode.AsArray.Count; i++)
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("uid", jSONNode[i]["uid"].Value);
					dictionary2.Add("message", jSONNode[i]["message"].Value);
					dictionary2.Add("item_code", jSONNode[i]["item_code"].Value);
					dictionary2.Add("item_count", (int)jSONNode[i]["item_count"]);
					dictionary2.Add("expire_sec", (int)jSONNode[i]["expire_sec"]);
					arrayList.Add(dictionary2);
				}
				dictionary.Add("item", arrayList);
				dictionary.Remove("data");
			}
			OnPostboxDelegate(dictionary);
		}

		public void OnPostboxItemReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)dictionary["data"];
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < jSONNode.AsArray.Count; i++)
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("uid", jSONNode[i]["uid"].Value);
					dictionary2.Add("message", jSONNode[i]["message"].Value);
					dictionary2.Add("item_code", jSONNode[i]["item_code"].Value);
					dictionary2.Add("item_count", (int)jSONNode[i]["item_count"]);
					dictionary2.Add("expire_sec", (int)jSONNode[i]["expire_sec"]);
					arrayList.Add(dictionary2);
				}
				dictionary.Add("item", arrayList);
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnPostboxItemUseReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			try
			{
				if (dictionary["status"].Equals(ResultType.SUCCESS))
				{
					JSONNode jSONNode = (JSONNode)dictionary["data"];
					dictionary.Add("item_code", jSONNode["item_code"].Value);
					dictionary.Add("item_count", (int)jSONNode["item_count"]);
					dictionary.Remove("data");
					if (jSONNode["hash"] != null)
					{
						string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, jSONNode["uuid"].Value, jSONNode["item_code"].Value);
						if (!ValidationHash(message, jSONNode["hash"]))
						{
							throw new Exception(Configure.MSG_HASH);
						}
					}
				}
			}
			catch (Exception ex)
			{
				dictionary.Clear();
				dictionary.Add("status", ResultType.HASH);
				dictionary.Add("message", ex.Message);
			}
			finally
			{
				OnPostboxDelegate(dictionary);
			}
		}

		public void OnPostboxItemUseReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			try
			{
				if (dictionary["status"].Equals(ResultType.SUCCESS))
				{
					JSONNode jSONNode = (JSONNode)dictionary["data"];
					dictionary.Add("item_code", jSONNode["item_code"].Value);
					dictionary.Add("item_count", (int)jSONNode["item_count"]);
					dictionary.Remove("data");
					if (jSONNode["hash"] != null)
					{
						string message = string.Format("{0}{1}{2}{3}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, jSONNode["uuid"].Value, jSONNode["item_code"].Value);
						if (!ValidationHash(message, jSONNode["hash"]))
						{
							throw new Exception(Configure.MSG_HASH);
						}
					}
				}
			}
			catch (Exception ex)
			{
				dictionary.Clear();
				dictionary.Add("status", ResultType.HASH);
				dictionary.Add("message", ex.Message);
			}
			finally
			{
				callback(dictionary);
			}
		}

		public void OnPostboxItemSendReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			OnPostboxDelegate(dictionary);
		}

		public void OnPostboxItemSendReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnPostboxFriendItemSendReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			OnPostboxDelegate(dictionary);
		}

		public void OnPostboxFriendItemSendReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnPostboxClearReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			OnPostboxDelegate(dictionary);
		}

		public void OnPostboxClearReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnPostboxPostboxSubscriptionRegisterReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			OnPostboxDelegate(dictionary);
		}

		public void OnPostboxPostboxSubscriptionRegisterReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnStorageSaveReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			OnStorageDelegate(dictionary);
		}

		public void OnStorageSaveReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnStorageLoadReceive(string result)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			try
			{
				if (dictionary["status"].Equals(ResultType.SUCCESS))
				{
					JSONNode jSONNode = JSON.Parse(dictionary["data"].ToString());
					if (jSONNode.Count > 0)
					{
						if (jSONNode["hash"] != null)
						{
							string message = string.Format("{0}{1}{2}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, jSONNode["uuid"].Value);
							if (!ValidationHash(message, jSONNode["hash"]))
							{
								throw new Exception(Configure.MSG_HASH);
							}
							jSONNode.Remove("hash");
						}
						foreach (KeyValuePair<string, JSONNode> item in jSONNode.AsObject)
						{
							dictionary.Add(item.Key, jSONNode[item.Key].Value);
						}
					}
					dictionary.Remove("data");
				}
			}
			catch (Exception ex)
			{
				dictionary.Clear();
				dictionary.Add("status", ResultType.HASH);
				dictionary.Add("message", ex.Message);
			}
			finally
			{
				OnStorageDelegate(dictionary);
			}
		}

		public void OnStorageLoadReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			try
			{
				if (dictionary["status"].Equals(ResultType.SUCCESS))
				{
					JSONNode jSONNode = JSON.Parse(dictionary["data"].ToString());
					if (jSONNode.Count > 0)
					{
						if (jSONNode["hash"] != null)
						{
							string message = string.Format("{0}{1}{2}", Setting.SERVICE_KEY, Setting.CHANNEL_ID, jSONNode["uuid"].Value);
							if (!ValidationHash(message, jSONNode["hash"]))
							{
								throw new Exception(Configure.MSG_HASH);
							}
							jSONNode.Remove("hash");
						}
						foreach (KeyValuePair<string, JSONNode> item in jSONNode.AsObject)
						{
							dictionary.Add(item.Key, jSONNode[item.Key].Value);
						}
					}
					dictionary.Remove("data");
				}
			}
			catch (Exception ex)
			{
				dictionary.Clear();
				dictionary.Add("status", ResultType.HASH);
				dictionary.Add("message", ex.Message);
			}
			finally
			{
				callback(dictionary);
			}
		}

		public void OnRankingSeasonInfoReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)dictionary["data"];
				dictionary.Add("season", jSONNode["season"].AsInt);
				dictionary.Add("expire_sec", jSONNode["expire_sec"].AsInt);
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnRankingReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)dictionary["data"];
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < jSONNode.AsArray.Count; i++)
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("uuid", jSONNode[i]["uuid"].Value);
					dictionary2.Add("nickname", jSONNode[i]["nickname"].Value);
					dictionary2.Add("country", jSONNode[i]["country"].Value);
					dictionary2.Add("score", jSONNode[i]["score"].AsInt);
					dictionary2.Add("player_data", jSONNode[i]["data"].Value);
					arrayList.Add(dictionary2);
				}
				dictionary.Add("order", arrayList);
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnRankingRecordReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnRankingMyReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)dictionary["data"];
				dictionary.Add("ranking", jSONNode["ranking"].AsInt);
				if (jSONNode["nickname"] != null)
				{
					dictionary.Add("nickname", jSONNode["nickname"].Value);
				}
				if (jSONNode["country"] != null)
				{
					dictionary.Add("country", jSONNode["country"].Value);
				}
				if (jSONNode["score"] != null)
				{
					dictionary.Add("score", jSONNode["score"].AsInt);
				}
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void OnRankingTopRecordReceive(string result, PlayNANOODelegate callback)
		{
			Dictionary<string, object> dictionary = OnReceiveController(result);
			if (dictionary["status"].Equals(ResultType.SUCCESS))
			{
				JSONNode jSONNode = (JSONNode)dictionary["data"];
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < jSONNode.AsArray.Count; i++)
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("uuid", jSONNode[i]["uuid"].Value);
					dictionary2.Add("nickname", jSONNode[i]["nickname"].Value);
					dictionary2.Add("season", jSONNode[i]["season"].AsInt);
					dictionary2.Add("score", jSONNode[i]["score"].AsInt);
					dictionary2.Add("country", jSONNode[i]["country"].Value);
					dictionary2.Add("player_data", jSONNode[i]["data"].Value);
					arrayList.Add(dictionary2);
				}
				dictionary.Add("order", arrayList);
				dictionary.Remove("data");
			}
			callback(dictionary);
		}

		public void Screenshot()
		{
			StartCoroutine(ScreenshotRequest());
		}

		private IEnumerator ScreenshotRequest()
		{
			yield return new WaitForEndOfFrame();
			Texture2D textures = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false);
			textures.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			textures.Apply();
			byte[] imageBase64 = textures.EncodeToJPG();
			UnityEngine.Object.DestroyImmediate(textures);
			OnScreenshotDelegate(imageBase64);
		}
	}
}
