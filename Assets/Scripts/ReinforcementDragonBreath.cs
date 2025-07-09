using System.Collections.Generic;
using UnityEngine;

public class ReinforcementDragonBreath : MovingObject
{
	public Transform fireTransform;

	private List<EnemyObject> m_attackTargetEnemies = new List<EnemyObject>();

	public void initDragon(Ground targetGround)
	{
		fixedDirection = true;
		m_attackTargetEnemies.Clear();
		for (int i = 0; i < Singleton<EnemyManager>.instance.enemyList.Count; i++)
		{
			EnemyObject enemyObject = Singleton<EnemyManager>.instance.enemyList[i];
			if (enemyObject.currentGround == targetGround)
			{
				m_attackTargetEnemies.Add(enemyObject);
			}
		}
		Direction direction2 = MovingObject.calculateDirection(targetGround.inPoint.position, targetGround.outPoint.position);
		setDirection(direction2);
		switch (direction2)
		{
		case Direction.Left:
			fireTransform.localEulerAngles = new Vector3(90f, 180f, 0f);
			break;
		case Direction.Right:
			fireTransform.localEulerAngles = new Vector3(90f, 0f, 0f);
			break;
		}
		int direction = ((direction2 == Direction.Right) ? 1 : (-1));
		moveTo(targetGround.outPoint.position, 9f, delegate
		{
			Vector2 targetPosition = targetGround.outPoint.position;
			targetPosition.x += (float)(-direction) * ((!targetGround.isBossGround) ? 6.5f : 4f);
			for (int j = 0; j < m_attackTargetEnemies.Count; j++)
			{
				double skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DragonsBreath, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DragonsBreath).skillLevel);
				if (m_attackTargetEnemies[j].isBoss || (m_attackTargetEnemies[j].myMonsterObject != null && m_attackTargetEnemies[j].myMonsterObject.isMiniboss))
				{
					skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DragonsBreath, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DragonsBreath).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses);
				}
				if (!m_attackTargetEnemies[j].isBoss && !m_attackTargetEnemies[j].isMiniboss)
				{
					skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DragonsBreath, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DragonsBreath).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters);
				}
				skillValue += skillValue / 100.0 * Singleton<SkillManager>.instance.getReinforcementSkillValue(SkillManager.SkillType.DragonsBreath);
				skillValue *= 0.5;
				m_attackTargetEnemies[j].setPoisonDamage(skillValue, 0.25f, 6 + (int)(Singleton<StatManager>.instance.dragonBreathExtraAttackCount * 3.0));
				Singleton<CachedManager>.instance.ingameCameraShake.shake(1f, 1f);
			}
			moveTo(targetPosition, 9f, delegate
			{
				Vector2 targetPosition2 = targetGround.outPoint.position;
				targetPosition2.x += direction * 7;
				moveTo(targetPosition2, 9f, delegate
				{
					ObjectPool.Recycle(base.name, base.cachedGameObject);
				});
			});
		});
	}
}
