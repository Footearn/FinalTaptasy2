using System.Collections.Generic;
using UnityEngine;

public class EventRewardManager : Singleton<EventRewardManager>
{
	private bool m_isRecievedReward;

	public void getTestReward()
	{
	}

	public void checkEventReward(List<EventReward> evnetRewardList)
	{
		if (evnetRewardList != null && evnetRewardList.Count > 0)
		{
			for (int i = 0; i < evnetRewardList.Count; i++)
			{
				recieveReward(evnetRewardList[i]);
			}
		}
		m_isRecievedReward = true;
	}

	public void recieveReward(EventReward rewardData)
	{
		if (Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Contains(rewardData.id))
		{
			return;
		}
		Singleton<DataManager>.instance.currentGameData.usedEventCouponList.Add(rewardData.id);
		long num = rewardData.rewardCount;
		string rewardType = rewardData.rewardType;
		string text = ((I18NManager.currentLanguage != I18NManager.Language.Korean) ? rewardData.message.en : rewardData.message.kr);
		switch (rewardType)
		{
		default:
			return;
		case "gold":
			AnalyzeManager.retention(AnalyzeManager.CategoryType.EventReward, AnalyzeManager.ActionType.GetGoldFromEventCode, new Dictionary<string, string>
			{
				{
					"ObjectID",
					Singleton<NanooAPIManager>.instance.UserID
				}
			});
			Singleton<FlyResourcesManager>.instance.playEffectResources(Vector2.zero, FlyResourcesManager.ResourceType.Gold, 30L, 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			Singleton<GoldManager>.instance.increaseGold(num, true);
			break;
		case "ruby":
			AnalyzeManager.retention(AnalyzeManager.CategoryType.EventReward, AnalyzeManager.ActionType.GetDiamondFromEventCode, new Dictionary<string, string>
			{
				{
					"ObjectID",
					Singleton<NanooAPIManager>.instance.UserID
				}
			});
			Singleton<FlyResourcesManager>.instance.playEffectResources(Vector2.zero, FlyResourcesManager.ResourceType.Ruby, (long)Mathf.Min(num, 30f), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			Singleton<RubyManager>.instance.increaseRuby(num, true);
			break;
		case "key":
			AnalyzeManager.retention(AnalyzeManager.CategoryType.EventReward, AnalyzeManager.ActionType.GetTreasureKeyFromEventCode, new Dictionary<string, string>
			{
				{
					"ObjectID",
					Singleton<NanooAPIManager>.instance.UserID
				}
			});
			Singleton<TreasureManager>.instance.increaseTreasurePiece(num);
			Singleton<FlyResourcesManager>.instance.playEffectResources(Vector2.zero, FlyResourcesManager.ResourceType.TreasurePiece, (long)Mathf.Min(num, 30f), 0.04f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			break;
		case "touch":
			AnalyzeManager.retention(AnalyzeManager.CategoryType.EventReward, AnalyzeManager.ActionType.GetAutoTouchFromEventCode, new Dictionary<string, string>
			{
				{
					"ObjectID",
					Singleton<NanooAPIManager>.instance.UserID
				}
			});
			Singleton<DataManager>.instance.currentGameData.isBoughtAutoTouch = true;
			UIWindowManageShop.instance.refreshSlots();
			break;
		case "stone":
			AnalyzeManager.retention(AnalyzeManager.CategoryType.EventReward, AnalyzeManager.ActionType.GetTreasureEnchantStoneFromEventCode, new Dictionary<string, string>
			{
				{
					"ObjectID",
					Singleton<NanooAPIManager>.instance.UserID
				}
			});
			Singleton<TreasureManager>.instance.increaseTreasureEnchantStone(num);
			Singleton<FlyResourcesManager>.instance.playEffectResources(Vector2.zero, FlyResourcesManager.ResourceType.TreasureEnchantStone, (long)Mathf.Min(num, 30f), 0.04f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			break;
		}
		Singleton<DataManager>.instance.saveData();
		if (rewardType == "gold")
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(text, GameManager.changeUnit(num)), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
		else if (rewardType != "touch")
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(text, num), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
		else
		{
			UIWindowDialog.openDescriptionNotUsingI18N(text, UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}
}
