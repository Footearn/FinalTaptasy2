using System.Collections;
using UnityEngine;

public class TowerModeFlameObject : MovingObject
{
	public GameObject cachedParticleSystemObject;

	private bool m_isHited;

	public void initFlame()
	{
		m_isHited = false;
		StopCoroutine("flameCollsionUpdate");
		StartCoroutine("flameCollsionUpdate");
	}

	private IEnumerator flameCollsionUpdate()
	{
		cachedParticleSystemObject.SetActive(false);
		cachedParticleSystemObject.SetActive(true);
		float timer2 = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer2 += Time.deltaTime * GameManager.timeScale;
				if (timer2 >= 1.6f)
				{
					break;
				}
			}
			yield return null;
		}
		timer2 = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer2 += Time.deltaTime * GameManager.timeScale;
				if (!m_isHited)
				{
					TowerModeCharacterObject character = Singleton<TowerModeManager>.instance.getNearestCharacter(base.cachedTransform.position + new Vector3(0f, 0.5f, 0f), 0.75f);
					if (character != null)
					{
						m_isHited = true;
						character.decreaseLifeCount();
						ShakeCamera.Instance.shake(2f, 1f);
					}
				}
				if (timer2 >= 1f)
				{
					break;
				}
			}
			yield return null;
		}
		initFlame();
	}

	public void recycleFlame(bool isRemoveFromList = true)
	{
		if (isRemoveFromList)
		{
			Singleton<TowerModeManager>.instance.listTowerModeFlames.Remove(this);
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
