using System.Collections;
using UnityEngine;

public class TowerModeEnemyProjectileObject : MovingObject
{
	public SpriteRenderer cachedSpriteRenderer;

	private EnemyManager.MonsterType m_casterType;

	private float m_currentSpeed;

	public void initProjectile(Vector2 targetPosition, EnemyManager.MonsterType casterType)
	{
		m_casterType = casterType;
		m_currentSpeed = 3.5f;
		setSpeed(m_currentSpeed);
		setRotatable(true);
		setDirection(MovingObject.calculateDirection(base.cachedTransform.position, targetPosition));
		moveTo(targetPosition, m_currentSpeed, delegate
		{
			recycleProjectile();
		});
		StopCoroutine("collisionCheckUpdate");
		StartCoroutine("collisionCheckUpdate");
	}

	private IEnumerator collisionCheckUpdate()
	{
		TowerModeCharacterObject attackTargetCharacter;
		while (true)
		{
			if (!GameManager.isPause && !TowerModeManager.isPauseObstacleAndMonster)
			{
				setSpeed(m_currentSpeed);
				attackTargetCharacter = Singleton<TowerModeManager>.instance.getNearestCharacter(base.cachedTransform.position, 0.6f);
				if (attackTargetCharacter != null)
				{
					break;
				}
			}
			else
			{
				setSpeed(0f);
			}
			yield return null;
		}
		attackTargetCharacter.decreaseLifeCount();
		ObjectPool.Spawn("fx_boss_blowup", base.cachedTransform.position);
		recycleProjectile();
	}

	public void recycleProjectile(bool isRemoveFromList = true)
	{
		if (isRemoveFromList)
		{
			Singleton<TowerModeManager>.instance.listTowerModeProjectileObjects.Remove(this);
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
