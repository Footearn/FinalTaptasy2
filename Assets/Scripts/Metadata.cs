using System;
using System.Collections.Generic;

[Serializable]
public class Metadata
{
	public VersionInfo versionInfo;

	public AdsAngelStatData adsAngelStatData;

	public TargetMedia targetMedia;

	public Jackpot jackpot;

	public BossRaidChestItemAppearChanceData bossRaidChestItemAppearChanceData;

	public RebirthRewardKeyCalculationData rebirthRewardKeyCalculationData;

	public List<string> noticeList;

	public List<EventReward> eventReward;

	public string warnUserList;

	public string banUserList;
}
