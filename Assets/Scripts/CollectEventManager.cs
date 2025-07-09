using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.UI;

public class CollectEventManager : Singleton<CollectEventManager>
{
	public enum CollectEventRewardType
	{
		Ruby,
		WarriorSkin,
		TreasureKey,
		ArcherSkin,
		TranscendStone,
		PriestSkin,
		HeartCoin,
		WeaponSkinPiece,
		WeaponSkinReinfecementMaterPiece,
		WarriorSpecialWeaponSkin,
		PriestSpecialWeaponSkin,
		ArcherSpecialWeaponSkin,
		HonorToken,
		Length
	}

	public struct CollectEventRewardData
	{
		public ObscuredLong rewardNeedValue;

		public CollectEventRewardType targetRewardType;

		public ObscuredLong rewardValue;

		public bool isCharacterSkin;

		public CollectEventRewardData(ObscuredLong rewardNeedValue, CollectEventRewardType targetRewardType, ObscuredLong rewardValue, bool isCharacterSkin)
		{
			this.rewardNeedValue = rewardNeedValue;
			this.targetRewardType = targetRewardType;
			this.rewardValue = rewardValue;
			this.isCharacterSkin = isCharacterSkin;
		}
	}

	[Serializable]
	public struct CollectEventRewardIconData
	{
		public CollectEventRewardType targetRewardType;

		public Sprite rewardIcon;
	}

	public static int collectEventIndex = 8;

	public static bool isOnCollectEvent = true;

	public DateTime collectEventEndTime = new DateTime(2018, 6, 15, 23, 59, 59);

	public GameObject ingameCollectEventObject;

	public Transform collectEventFlyTargetTransform;

	public Text collectEventIngameCountText;

	public List<CollectEventRewardData> collectEventRewardList;

	public List<CollectEventRewardIconData> collectEventRewardIconDataList;

	public GameObject collectEventButtonObject;

	private void Awake()
	{
		collectEventRewardList = new List<CollectEventRewardData>();
		collectEventRewardList.Add(new CollectEventRewardData(500L, CollectEventRewardType.HonorToken, 300L, false));
		collectEventRewardList.Add(new CollectEventRewardData(2500L, CollectEventRewardType.WeaponSkinReinfecementMaterPiece, 500L, false));
		collectEventRewardList.Add(new CollectEventRewardData(5000L, CollectEventRewardType.HeartCoin, 500L, false));
		collectEventRewardList.Add(new CollectEventRewardData(10000L, CollectEventRewardType.ArcherSkin, 29L, true));
		collectEventRewardList.Add(new CollectEventRewardData(15000L, CollectEventRewardType.WarriorSkin, 29L, true));
		collectEventRewardList.Add(new CollectEventRewardData(30000L, CollectEventRewardType.PriestSkin, 29L, true));
		DateTime dateTime = collectEventEndTime.AddDays(7.0);
		if (UnbiasedTime.Instance.Now().Ticks > dateTime.Ticks)
		{
			collectEventButtonObject.SetActive(false);
		}
		else
		{
			collectEventButtonObject.SetActive(true);
		}
	}

	public Sprite getRewardIcon(CollectEventRewardType rewardType)
	{
		Sprite result = null;
		for (int i = 0; i < collectEventRewardIconDataList.Count; i++)
		{
			if (collectEventRewardIconDataList[i].targetRewardType == rewardType)
			{
				result = collectEventRewardIconDataList[i].rewardIcon;
				break;
			}
		}
		return result;
	}

	public void checkIsOnCollectEvent()
	{
		if (UnbiasedTime.Instance.Now().Ticks > collectEventEndTime.Ticks)
		{
			ingameCollectEventObject.SetActive(false);
			isOnCollectEvent = false;
		}
		else
		{
			isOnCollectEvent = true;
		}
	}

	public CollectEventRewardData getCollectEventRewardData(int tier)
	{
		return collectEventRewardList[tier];
	}

	public void startGame()
	{
		displayCollectEventResourceCountText();
	}

	public void spawnCollectEventResource(Vector2 spawnPosition, long dropCount)
	{
		Singleton<DropItemManager>.instance.spawnDropItem(DropItemManager.DropItemType.CollectEventResource, spawnPosition, dropCount);
	}

	public void increaseCollectEventResource(long value)
	{
		GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
		currentGameData.collectEventResource = (long)currentGameData.collectEventResource + value;
		displayCollectEventResourceCountText();
	}

	public void displayCollectEventResourceCountText()
	{
		collectEventIngameCountText.text = Singleton<DataManager>.instance.currentGameData.collectEventResource.ToString("N0");
	}
}
