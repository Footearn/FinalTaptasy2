using System;
using System.Collections;
using UnityEngine;

public class AutoRecycleEffect : ObjectBase
{
	public float duration;

	public Action recycleEvent;

	public bool isNewObjectPool;

	private IEnumerator m_recycleUpdateCoroutine;

	private void OnEnable()
	{
		resetTimer();
	}

	public void resetTimer()
	{
		m_recycleUpdateCoroutine = recycleUpdate();
		StartCoroutine(m_recycleUpdateCoroutine);
	}

	private IEnumerator recycleUpdate()
	{
		yield return null;
		float timer = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= duration)
				{
					break;
				}
			}
			yield return null;
		}
		if (recycleEvent != null)
		{
			recycleEvent();
		}
		if (isNewObjectPool)
		{
			NewObjectPool.Recycle(base.cachedGameObject);
		}
		else
		{
			ObjectPool.Recycle(base.name, base.cachedGameObject);
		}
	}
}
