using UnityEngine;

public class ElopeResourceDropObject : DropObject
{
	public SpriteRenderer targetRenderer;

	public void initElopeResource(DropItemManager.DropItemType resourceType)
	{
		targetRenderer.sprite = Singleton<ElopeModeManager>.instance.getPrincessUpgradeResourceIcon(resourceType);
	}
}
