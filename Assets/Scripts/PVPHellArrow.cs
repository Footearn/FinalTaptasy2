using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPHellArrow : NewMovingObject
{
	public Transform hitPoint;

	public Transform arrowEffectTransform;

	private bool m_isAlly;

	private float m_duration = 2.5f;

	private Coroutine m_arrowUpdateCoroutine;

	private double m_totaldamage;

	public void initHellArrow(bool isAlly, double totaldamage)
	{
		arrowEffectTransform.localScale = base.cachedTransform.localScale;
		m_isAlly = isAlly;
		m_totaldamage = totaldamage / (double)m_duration;
		if (m_arrowUpdateCoroutine != null)
		{
			StopCoroutine(m_arrowUpdateCoroutine);
		}
		m_arrowUpdateCoroutine = StartCoroutine(arrowUpdate());
	}

	private IEnumerator arrowUpdate()
	{
		float timer = 0f;
		while (timer < m_duration)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				List<PVPUnitObject> targetEnemyList = Singleton<PVPUnitManager>.instance.getNearestUnits(hitPoint.position, 9.5f, m_isAlly);
				for (int i = 0; i < targetEnemyList.Count; i++)
				{
					targetEnemyList[i].decreaseHP(m_totaldamage * (double)Time.deltaTime * (double)GameManager.timeScale);
				}
			}
			yield return null;
		}
		NewObjectPool.Recycle(this);
	}
}
