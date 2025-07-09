using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPPenetrationArrow : ObjectBase
{
	public float arrowSpeed;

	private float m_moveStackXPosition;

	private List<PVPUnitObject> m_attackEnemyObject = new List<PVPUnitObject>();

	private Vector3 m_normalizedTargetDirection;

	private bool m_isAlly;

	private double m_damage;

	public void initPenetrationArrow(Vector3 targetPosition, bool isAlly, double damage)
	{
		m_isAlly = isAlly;
		m_damage = damage;
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
		while (Mathf.Abs(m_moveStackXPosition) < 30f)
		{
			if (!GameManager.isPause)
			{
				Vector3 position = base.cachedTransform.position;
				Vector3 prevPosition = position;
				position += m_normalizedTargetDirection * arrowSpeed * Time.deltaTime * GameManager.timeScale;
				m_moveStackXPosition += Vector3.Distance(prevPosition, position);
				base.cachedTransform.position = position;
				List<PVPUnitObject> nearEnemyList = Singleton<PVPUnitManager>.instance.getNearestUnits(position, 1.5f, m_isAlly);
				for (int i = 0; i < nearEnemyList.Count; i++)
				{
					if (!m_attackEnemyObject.Contains(nearEnemyList[i]))
					{
						PVPUnitObject enemy = nearEnemyList[i];
						m_attackEnemyObject.Add(enemy);
						Singleton<AudioManager>.instance.playEffectSound("monster_blowup", AudioManager.EffectType.Colleague);
						enemy.decreaseHP(m_damage);
					}
				}
			}
			yield return null;
		}
		recycle();
	}

	private void recycle()
	{
		NewObjectPool.Recycle(this);
	}
}
