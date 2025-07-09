using System;
using System.Collections;
using System.Collections.Generic;
using PlayNANOO;
using SimpleJSON;
using UnityEngine;

public class PlayNANOOExample : MonoBehaviour
{
	private Plugin _playNANOO;

	private PlayNANOO.EventHandler _playNANOOHandler;

	private readonly string gameUUID = "Example";

	private readonly string gameNickname = "Example";

	private readonly int versionCode;

	private void Awake()
	{
		_playNANOO = GameObject.Find("PlayNANOO").GetComponent<Plugin>();
		_playNANOOHandler = GameObject.Find("PlayNANOO").GetComponent<PlayNANOO.EventHandler>();
	}

	public void OpenForum()
	{
		try
		{
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.OpenForum();
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void OpenForumView()
	{
		try
		{
			string url = "Please enter a webpage URL";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.OpenForumView(url);
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.LogError("Please check your device");
		}
	}

	public void OpenScreenshot()
	{
		try
		{
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.OpenScreenshot();
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void OpenScreenshotImage()
	{
		try
		{
			Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			texture2D.Apply();
			byte[] result = texture2D.EncodeToJPG();
			UnityEngine.Object.DestroyImmediate(texture2D);
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.OpenScreenshot(result);
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void OpenHelp()
	{
		try
		{
			string key = "Please enter item key value.";
			string value = "Please enter item value.";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.SetHelpOptional(key, value);
			_playNANOO.OpenHelp();
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void Init()
	{
		try
		{
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.Init(true, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log(string.Format("token=>{0}", dic["token"].ToString()));
					Debug.Log(string.Format("time=>{0}", dic["time"].ToString()));
					if (dic.ContainsKey("postbox_count"))
					{
						Debug.Log(string.Format("postbox_count=>{0}", dic["postbox_count"].ToString()));
					}
					if (dic.ContainsKey("postbox_subscription"))
					{
						ArrayList arrayList = (ArrayList)dic["postbox_subscription"];
						if (arrayList.Count > 0)
						{
							foreach (Dictionary<string, object> item in arrayList)
							{
								Debug.Log(string.Format("product=>{0}", item["product"].ToString()));
								Debug.Log(string.Format("ttl=>{0}", item["ttl"].ToString()));
							}
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.LogError(message);
			Debug.LogError("Please check your device");
		}
	}

	public void ForumThread()
	{
		try
		{
			string language = "ko_KR";
			string section = "sticky";
			short limit = 10;
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.ForumThread(language, section, limit, delegate(Dictionary<string, object> dic)
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
							Debug.Log(value);
							Debug.Log(value2);
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void ReceiptVerificationAOS()
	{
		try
		{
			string productID = "Please enter Product ID";
			string purchase = "Please enter receipt";
			string signature = "Please enter signature";
			string currency = "Please enter currency";
			double price = 0.0;
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.ReceiptVerification(productID, purchase, signature, currency, price, OnReceiptVerification);
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void ReceiptVerificationIOS()
	{
	}

	private void OnReceiptState(Dictionary<string, object> dic)
	{
		if (dic["status"].Equals(ResultType.SUCCESS))
		{
			Debug.Log("success");
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

	private void OnReceiptVerification(Dictionary<string, object> dic)
	{
		if (dic["status"].Equals(ResultType.SUCCESS))
		{
			Debug.Log(string.Format("package=>{0}", dic["package"].ToString()));
			Debug.Log(string.Format("product_id=>{0}", dic["product_id"].ToString()));
			Debug.Log(string.Format("order_id=>{0}", dic["order_id"].ToString()));
			string product = "Please enter Subscription Product Unique ID";
			if (!dic["product_id"].ToString().Equals("PRODUCT_ID"))
			{
				return;
			}
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOOHandler.OnReceiptDelegate = delegate(Dictionary<string, object> result)
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
			_playNANOO.PostboxSubscriptionRegister(product);
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

	public void CouponCheck()
	{
		try
		{
			string code = "Please enter coupon code";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.CouponCheck(code, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log(string.Format("item_code=>{0}", dic["item_code"].ToString()));
					Debug.Log(string.Format("item_count=>{0}", dic["item_count"].ToString()));
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void PostboxItem()
	{
		try
		{
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.PostboxItem(delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					if (dic.ContainsKey("item"))
					{
						ArrayList arrayList = (ArrayList)dic["item"];
						if (arrayList.Count > 0)
						{
							foreach (Dictionary<string, object> item in arrayList)
							{
								Debug.Log(string.Format("item_uid=>{0}", item["uid"].ToString()));
								Debug.Log(string.Format("item_code=>{0}", item["item_code"].ToString()));
								Debug.Log(string.Format("item_count=>{0}", item["item_count"].ToString()));
								Debug.Log(string.Format("expire_sec=>{0}", item["expire_sec"].ToString()));
								if (item["message"] != null)
								{
									Debug.Log(string.Format("message=>{0}", item["message"].ToString()));
								}
							}
						}
						else
						{
							Debug.Log("No item");
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void PostboxItemUse()
	{
		try
		{
			string itemID = "Please enter item uid";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.PostboxItemUse(itemID, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log(string.Format("item_code=>{0}", dic["item_code"].ToString()));
					Debug.Log(string.Format("item_count=>{0}", dic["item_count"].ToString()));
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void PostboxItemSend()
	{
		try
		{
			string itemCode = "Please enter item code";
			int itemCount = 1;
			short itemExpireDay = 1;
			string itemMessage = null;
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.PostboxItemSend(itemCode, itemCount, itemExpireDay, itemMessage, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log("success");
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void PostboxFriendItemSend()
	{
		try
		{
			string friendUUID = "Please enter friend unique game id";
			string itemCode = "Please enter item code";
			int itemCount = 0;
			short itemExpireDay = 0;
			string itemMessage = null;
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.PostboxFriendItemSend(friendUUID, itemCode, itemCount, itemExpireDay, itemMessage, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log("success");
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void PostboxClear()
	{
		try
		{
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.PostboxClear(delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log("success");
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void PostboxSubscriptionRegister()
	{
		try
		{
			string product = "Please enter subscription product unique id";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.PostboxSubscriptionRegister(product, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log("success");
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void StorageSave()
	{
		try
		{
			string key = "TEST:000001";
			string value = "{\"Sample\" : \"JsonText\"}";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.StorageSave(key, value, false, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log("success");
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
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void StorageLoad()
	{
		try
		{
			string key = "Please enter the storage key";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.StorageLoad(key, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log(string.Format("{0}", dic["value"]));
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
				else
				{
					Debug.Log(string.Format("message=>{0}", dic["message"]));
				}
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void RankingSeasonInfo()
	{
		try
		{
			string uid = "Please enter ranking uid";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.RankingSeasonInfo(uid, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log(string.Format("season=>{0}", dic["season"].ToString()));
					Debug.Log(string.Format("expire_sec=>{0}", dic["expire_sec"].ToString()));
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
				else
				{
					Debug.Log(string.Format("message=>{0}", dic["message"]));
				}
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void RankingRecord()
	{
		try
		{
			string uid = "Please enter ranking uid";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.RankingRecord(uid, "KR", 100, "TEST", delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log("success");
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
				else
				{
					Debug.Log(string.Format("message=>{0}", dic["message"]));
				}
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void RankingMy()
	{
		try
		{
			string uid = "Please enter ranking uid";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.RankingMy(uid, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					Debug.Log(string.Format("ranking=>{0}", dic["ranking"].ToString()));
					if (dic.ContainsKey("nickname"))
					{
						Debug.Log(string.Format("nickname=>{0}", dic["nickname"].ToString()));
					}
					if (dic.ContainsKey("country"))
					{
						Debug.Log(string.Format("country=>{0}", dic["country"].ToString()));
					}
					if (dic.ContainsKey("score"))
					{
						Debug.Log(string.Format("score=>{0}", dic["score"].ToString()));
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
				else
				{
					Debug.Log(string.Format("message=>{0}", dic["message"]));
				}
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void Ranking()
	{
		try
		{
			string uid = "Please enter ranking uid";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.Ranking(uid, 50, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					if (dic.ContainsKey("order"))
					{
						ArrayList arrayList = (ArrayList)dic["order"];
						if (arrayList.Count > 0)
						{
							foreach (Dictionary<string, object> item in arrayList)
							{
								Debug.Log(string.Format("uuid=>{0}", item["uuid"].ToString()));
								Debug.Log(string.Format("score=>{0}", item["score"].ToString()));
								Debug.Log(string.Format("nickname=>{0}", item["nickname"].ToString()));
								Debug.Log(string.Format("country=>{0}", item["country"].ToString()));
								Debug.Log(string.Format("player_data=>{0}", item["player_data"].ToString()));
							}
						}
						else
						{
							Debug.Log("No Ranking");
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
				else
				{
					Debug.Log(string.Format("message=>{0}", dic["message"]));
				}
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}

	public void RankingTopRecord()
	{
		try
		{
			string uid = "Please enter ranking uid";
			_playNANOO.SetInfo(gameUUID, gameNickname, versionCode);
			_playNANOO.RankingTopRecord(uid, 50, delegate(Dictionary<string, object> dic)
			{
				if (dic["status"].Equals(ResultType.SUCCESS))
				{
					if (dic.ContainsKey("order"))
					{
						ArrayList arrayList = (ArrayList)dic["order"];
						if (arrayList.Count > 0)
						{
							foreach (Dictionary<string, object> item in arrayList)
							{
								Debug.Log(string.Format("uuid=>{0}", item["uuid"].ToString()));
								Debug.Log(string.Format("nickname=>{0}", item["nickname"].ToString()));
								Debug.Log(string.Format("season=>{0}", item["season"].ToString()));
								Debug.Log(string.Format("country=>{0}", item["country"].ToString()));
								Debug.Log(string.Format("score=>{0}", item["score"].ToString()));
								Debug.Log(string.Format("player_data=>{0}", item["player_data"].ToString()));
							}
						}
						else
						{
							Debug.Log("No Record");
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
				else
				{
					Debug.Log(string.Format("message=>{0}", dic["message"]));
				}
			});
		}
		catch (NullReferenceException message)
		{
			Debug.Log(message);
			Debug.Log("Please check your device");
		}
	}
}
