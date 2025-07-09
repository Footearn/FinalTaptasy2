using System;
using UnityEngine;

public class PVPProjectileObject : NewMovingObject
{
	public SpriteRenderer projectileRenderer;

	public GameObject projectileObject;

	public GameObject trailObject;

	public PVPProjectileManager.ProjectileAttributeType currentProjectileAttributeType = PVPProjectileManager.ProjectileAttributeType.None;

	private PVPProjectileManager.ProjectileData m_currentData;

	public void initProjectile(Transform arriveTransform, PVPProjectileManager.ProjectileData data, Action<PVPProjectileManager.ProjectileArriveData> arriveAction, PVPProjectileManager.ProjectileAttributeType projectileAttributeType)
	{
		currentProjectileAttributeType = projectileAttributeType;
		bool flag = currentProjectileAttributeType == PVPProjectileManager.ProjectileAttributeType.Blow;
		if (trailObject != null)
		{
			trailObject.SetActive(flag);
		}
		if (projectileRenderer != null)
		{
			if (flag)
			{
				projectileRenderer.sortingOrder = 3;
			}
			else
			{
				projectileRenderer.sortingOrder = 1;
			}
		}
		m_currentData = data;
		if (projectileObject != null)
		{
			projectileObject.SetActive(m_currentData.sprite != null);
		}
		if (projectileRenderer != null)
		{
			projectileRenderer.sprite = m_currentData.sprite;
		}
		setParabolic(m_currentData.isParabolic, data.parabolicMultiply);
		setLookAtTarget(!m_currentData.isRotatable);
		setRotatable(m_currentData.isRotatable);
		moveTo(arriveTransform, m_currentData.projectileSpeed, delegate
		{
			PVPProjectileManager.ProjectileArriveData obj = new PVPProjectileManager.ProjectileArriveData(base.cachedTransform.position, currentProjectileAttributeType);
			arriveAction(obj);
			recycle();
		});
	}

	public void initProjectile(Vector2 arrivePosition, PVPProjectileManager.ProjectileData data, float parabolicValue, Action<PVPProjectileManager.ProjectileArriveData> arriveAction, PVPProjectileManager.ProjectileAttributeType projectileAttributeType)
	{
		currentProjectileAttributeType = projectileAttributeType;
		bool flag = currentProjectileAttributeType == PVPProjectileManager.ProjectileAttributeType.Blow;
		if (trailObject != null)
		{
			trailObject.SetActive(flag);
		}
		if (projectileRenderer != null)
		{
			if (flag)
			{
				projectileRenderer.sortingOrder = 3;
			}
			else
			{
				projectileRenderer.sortingOrder = 1;
			}
		}
		m_currentData = data;
		setRotatable(data.isRotatable);
		setLookAtTarget(!m_currentData.isRotatable);
		setParabolic(m_currentData.isParabolic, parabolicValue);
		if (projectileObject != null)
		{
			projectileObject.SetActive(m_currentData.sprite != null);
		}
		if (projectileRenderer != null)
		{
			projectileRenderer.sprite = m_currentData.sprite;
		}
		moveTo(arrivePosition, m_currentData.projectileSpeed, delegate
		{
			PVPProjectileManager.ProjectileArriveData obj = new PVPProjectileManager.ProjectileArriveData(base.cachedTransform.position, currentProjectileAttributeType);
			arriveAction(obj);
			recycle();
		});
	}

	private void recycle()
	{
		stopMove();
		NewObjectPool.Recycle(this);
		Singleton<PVPProjectileManager>.instance.totalProjectileObject.Remove(this);
	}
}
