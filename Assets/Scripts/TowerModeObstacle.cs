using System.Collections;
using UnityEngine;

public class TowerModeObstacle : MovingObject
{
	public TowerModeManager.ObstacleType currentObstacleType = TowerModeManager.ObstacleType.None;

	public SpriteRenderer obstacleSpriteRenderer;

	public TowerModeObstacleIndicator currentIndicatorObject;

	private float m_range;

	private float m_currentSpeed;

	public void initObstacle(TowerModeManager.ObstacleType type, float speed, float range)
	{
		currentObstacleType = type;
		recycleIndicator();
		m_currentSpeed = speed;
		setSpeed(m_currentSpeed);
		m_range = range;
		obstacleSpriteRenderer.sprite = Singleton<TowerModeManager>.instance.obstacleSpriteList[(int)type];
		Vector2 targetPosition = base.cachedTransform.position;
		targetPosition.y -= 30f;
		moveTo(targetPosition, (!TowerModeManager.isPauseObstacleAndMonster) ? m_currentSpeed : 0f, delegate
		{
			recycleObstacle();
		});
		StopCoroutine("obstacleUpdate");
		StartCoroutine("obstacleUpdate");
	}

	public void recycleObstacle(bool withRemoveFromList = true)
	{
		recycleIndicator();
		if (withRemoveFromList)
		{
			Singleton<TowerModeManager>.instance.listTowerModeObstacles.Remove(this);
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}

	private void spawnIndicator()
	{
		Singleton<AudioManager>.instance.playEffectSound("tower_warning");
		currentIndicatorObject = ObjectPool.Spawn("@TowerModeObstacleIndicator", Vector2.zero, Vector3.zero, new Vector3(1.2f, 1.2f, 1f), CameraFollow.instance.cachedTransform).GetComponent<TowerModeObstacleIndicator>();
		currentIndicatorObject.initObstacleIndicator(currentObstacleType);
		currentIndicatorObject.cachedTransform.position = new Vector2(base.cachedTransform.position.x, Singleton<CachedManager>.instance.ingameCamera.ViewportToWorldPoint(Vector2.one).y - 0.6f);
	}

	public void recycleIndicator()
	{
		if (currentIndicatorObject != null)
		{
			ObjectPool.Recycle(currentIndicatorObject.name, currentIndicatorObject.cachedGameObject);
		}
		currentIndicatorObject = null;
	}

	private IEnumerator obstacleUpdate()
	{
		TowerModeCharacterObject targetChr2 = null;
		bool isSpawnedIndicator = false;
		while (!Singleton<TowerModeManager>.instance.isDead)
		{
			if (!GameManager.isPause && !TowerModeManager.isPauseObstacleAndMonster)
			{
				if (currentIndicatorObject != null && base.cachedTransform.position.y < Singleton<CachedManager>.instance.ingameCamera.ViewportToWorldPoint(Vector2.one).y)
				{
					recycleIndicator();
				}
				if (!isSpawnedIndicator && base.cachedTransform.position.y < Singleton<CachedManager>.instance.ingameCamera.ViewportToWorldPoint(Vector2.one).y + m_speed)
				{
					spawnIndicator();
					isSpawnedIndicator = true;
				}
				setSpeed(m_currentSpeed);
				targetChr2 = Singleton<TowerModeManager>.instance.getNearestCharacter(base.cachedTransform.position, m_range);
				if (targetChr2 != null)
				{
					targetChr2.decreaseLifeCount();
					ObjectPool.Spawn("fx_boss_blowup", base.cachedTransform.position);
					ShakeCamera.Instance.shake(1f, 1f);
					recycleObstacle();
					break;
				}
			}
			else if (TowerModeManager.isPauseObstacleAndMonster)
			{
				setSpeed(0f);
			}
			yield return null;
		}
	}
}
