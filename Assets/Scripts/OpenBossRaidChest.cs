using UnityEngine;

public class OpenBossRaidChest : MonoBehaviour
{
	public RectTransform chestOpenEffectTargetTransform;

	public void openChest()
	{
		UIWindowLotteryBossRaidChest.instance.openChest();
	}

	public void startEffect()
	{
		ObjectPool.Spawn("@BossChestOpenEffect", chestOpenEffectTargetTransform.position);
	}
}
