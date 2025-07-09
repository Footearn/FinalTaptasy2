using UnityEngine;

public class TowerModeObstacleIndicator : ObjectBase
{
	public TowerModeManager.ObstacleType currentObstacleType;

	public SpriteRenderer cachedSpriteRenderer;

	public void initObstacleIndicator(TowerModeManager.ObstacleType obstacleType)
	{
		currentObstacleType = obstacleType;
		cachedSpriteRenderer.sprite = Singleton<TowerModeManager>.instance.obstacleIndicatorSpriteLists[(int)obstacleType];
	}
}
