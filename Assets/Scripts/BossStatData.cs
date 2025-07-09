using System;

[Serializable]
public struct BossStatData
{
	public EnemyManager.BossType currentBossType;

	public float baseDelay;

	public float baseSpeed;

	public float baseRange;
}
