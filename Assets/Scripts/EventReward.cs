using System;

[Serializable]
public struct EventReward
{
	public string id;

	public int rewardCount;

	public string rewardType;

	public EventRewardMessage message;
}
