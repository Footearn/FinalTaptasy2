using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrationObject : ObjectBase
{
	public float arrowSpeed;

	private float m_moveStackXPosition;

	private List<EnemyObject> m_attackEnemyObject = new List<EnemyObject>();

	private Vector3 m_normalizedTargetDirection;

	public void initPenetration(Vector3 targetPosition)
	{
		Vector3 vector = targetPosition - base.cachedTransform.position;
		base.cachedTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(vector.y, vector.x) * 57.29578f));
		m_normalizedTargetDirection = vector.normalized;
		StopAllCoroutines();
		m_attackEnemyObject.Clear();
		m_moveStackXPosition = 0f;
		StartCoroutine("arrowUpdate");
	}

	private IEnumerator arrowUpdate()
	{
		float skillValue = Singleton<TranscendManager>.instance.getTranscendPassiveSkillValue(TranscendManager.TranscendPassiveSkillType.PenetrationArrow, Singleton<TranscendManager>.instance.getTranscendPassiveSkillInventoryData(TranscendManager.TranscendPassiveSkillType.PenetrationArrow).skillLevel);
		while (Mathf.Abs(m_moveStackXPosition) < 12f)
		{
			if (!GameManager.isPause)
			{
				Vector3 position = base.cachedTransform.position;
				Vector3 prevPosition = position;
				position += m_normalizedTargetDirection * arrowSpeed * Time.deltaTime * GameManager.timeScale;
				m_moveStackXPosition += Vector3.Distance(prevPosition, position);
				base.cachedTransform.position = position;
				List<EnemyObject> nearEnemyList = Singleton<EnemyManager>.instance.getNearestEnemies(position, 1.3f);
				for (int i = 0; i < nearEnemyList.Count; i++)
				{
					if (m_attackEnemyObject.Contains(nearEnemyList[i]))
					{
						continue;
					}
					EnemyObject enemy = nearEnemyList[i];
					if (!(enemy is BossObject) || ((BossObject)enemy).isCanAttack)
					{
						m_attackEnemyObject.Add(enemy);
						double arrowDamage = Singleton<CharacterManager>.instance.archerCharacter.curDamage / 100.0 * (double)skillValue;
						if (enemy.isBoss || (enemy.myMonsterObject != null && enemy.myMonsterObject.isMiniboss))
						{
							arrowDamage = Singleton<CharacterManager>.instance.archerCharacter.getCurrentDamage(Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) / 100.0 * (double)skillValue;
						}
						if (!enemy.isBoss && !enemy.isMiniboss)
						{
							arrowDamage = Singleton<CharacterManager>.instance.archerCharacter.getCurrentDamage(Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) / 100.0 * (double)skillValue;
						}
						Singleton<AudioManager>.instance.playEffectSound("monster_blowup", AudioManager.EffectType.Colleague);
						enemy.decreasesHealth(arrowDamage);
						enemy.setDamageText(arrowDamage, CharacterManager.CharacterType.Warrior, false, false, false, true);
					}
				}
			}
			yield return null;
		}
		recycle();
	}

	private void recycle()
	{
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
