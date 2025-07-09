using System;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using UnityEngine;

public class RouletteManager : Singleton<RouletteManager>
{
	[Serializable]
	public class RouletteRewardData
	{
		public readonly RouletteRewardType rewardType;

		public readonly double value;

		public readonly double appearChance;

		public RouletteRewardData(RouletteRewardType rewardType, double value, double appearChance)
		{
			this.rewardType = rewardType;
			this.value = value;
			this.appearChance = appearChance;
		}
	}

	public enum RouletteRewardType
	{
		None = -1,
		Gold,
		Ruby,
		HeartCoin,
		TreasureKey,
		TranscendStone
	}

	public static long intervalBronzeRouletteMinutes = 1440L;

	public List<Sprite> rouletteSpriteList;

	public List<RouletteRewardData> bronzeRouletteRewardList;

	public List<RouletteRewardData> goldRouletteRewardList;

	private void Awake()
	{
		bronzeRouletteRewardList = new List<RouletteRewardData>();
		goldRouletteRewardList = new List<RouletteRewardData>();
		setRouletteRewardData();
	}

	private void setRouletteRewardData()
	{
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Gold, 300.0, 10.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Gold, 500.0, 20.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Gold, 700.0, 15.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Gold, 1000.0, 10.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.HeartCoin, 10.0, 9.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.HeartCoin, 30.0, 5.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.HeartCoin, 50.0, 1.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.TreasureKey, 10.0, 9.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.TreasureKey, 30.0, 5.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.TreasureKey, 50.0, 1.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Ruby, 10.0, 9.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Ruby, 30.0, 5.0));
		bronzeRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Ruby, 50.0, 1.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Gold, 15000.0, 20.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Gold, 30000.0, 15.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Gold, 50000.0, 10.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.HeartCoin, 100.0, 9.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.HeartCoin, 300.0, 5.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.HeartCoin, 500.0, 1.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.TreasureKey, 100.0, 9.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.TreasureKey, 300.0, 5.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.TreasureKey, 500.0, 1.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Ruby, 200.0, 9.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Ruby, 300.0, 5.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.Ruby, 500.0, 1.0));
		goldRouletteRewardList.Add(new RouletteRewardData(RouletteRewardType.TranscendStone, 30.0, 10.0));
	}

	public RouletteRewardData getRandomRouletteData(bool isBronzeRoulette)
	{
		RouletteRewardData result = null;
		MersenneTwister mersenneTwister = new MersenneTwister();
		double num = mersenneTwister.Next(0, 10000);
		num /= 100.0;
		double num2 = 0.0;
		if (isBronzeRoulette)
		{
			for (int i = 0; i < bronzeRouletteRewardList.Count; i++)
			{
				num2 += bronzeRouletteRewardList[i].appearChance;
				if (num < num2)
				{
					result = bronzeRouletteRewardList[i];
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < goldRouletteRewardList.Count; j++)
			{
				num2 += goldRouletteRewardList[j].appearChance;
				if (num < num2)
				{
					result = goldRouletteRewardList[j];
					break;
				}
			}
		}
		return result;
	}

	public void doRewardGetEvent(RouletteRewardData rewardData)
	{
		switch (rewardData.rewardType)
		{
		case RouletteRewardType.Gold:
			Singleton<GoldManager>.instance.increaseGold(rewardData.value * CalculateManager.getCurrentStandardGold(), true);
			break;
		case RouletteRewardType.Ruby:
			Singleton<RubyManager>.instance.increaseRuby(rewardData.value, true);
			break;
		case RouletteRewardType.HeartCoin:
			Singleton<ElopeModeManager>.instance.increaseHeartCoin((long)rewardData.value);
			break;
		case RouletteRewardType.TreasureKey:
			Singleton<TreasureManager>.instance.increaseTreasurePiece((long)rewardData.value);
			break;
		case RouletteRewardType.TranscendStone:
			Singleton<TranscendManager>.instance.increaseTranscendStone((long)rewardData.value);
			break;
		}
	}

	public void doRewardResultEvent(RouletteRewardData rewardData, Action endAction)
	{
		switch (rewardData.rewardType)
		{
		case RouletteRewardType.Gold:
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.Gold, 30L, 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			break;
		case RouletteRewardType.Ruby:
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.Ruby, Mathf.Min((int)rewardData.value, 30), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			break;
		case RouletteRewardType.HeartCoin:
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.HeartCoin, Mathf.Min((int)rewardData.value, 30), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			break;
		case RouletteRewardType.TreasureKey:
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.TreasurePiece, Mathf.Min((int)rewardData.value, 30), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			Singleton<TreasureManager>.instance.increaseTreasurePiece((long)rewardData.value);
			break;
		case RouletteRewardType.TranscendStone:
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.TranscendStone, Mathf.Min((int)rewardData.value, 30), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			break;
		}
		if (endAction != null)
		{
			endAction();
		}
	}

	public bool isCanStartBronzeRoulette()
	{
		bool flag = false;
		if (Singleton<DataManager>.instance.currentGameData.bronzeRouletteTargetEndTime < UnbiasedTime.Instance.UtcNow().Ticks)
		{
			flag = true;
		}
		else
		{
			flag = false;
			TimeSpan timeSpan = new TimeSpan(Singleton<DataManager>.instance.currentGameData.bronzeRouletteTargetEndTime - UnbiasedTime.Instance.UtcNow().Ticks);
			if (timeSpan.TotalMinutes > (double)intervalBronzeRouletteMinutes)
			{
				Singleton<DataManager>.instance.currentGameData.bronzeRouletteTargetEndTime = UnbiasedTime.Instance.UtcNow().AddMinutes(intervalBronzeRouletteMinutes).Ticks;
				Singleton<DataManager>.instance.saveData();
			}
		}
		return flag;
	}
}
