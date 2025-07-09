using System;
using UnityEngine;

public class ColleagueBulletObject : MovingObject
{
	public SpriteRenderer bulletSpriteRenderer;

	public FadeChangableObject currentFadeObject;

	private ColleagueManager.ColleagueBulletAttributeData m_currentBulletAttributeData;

	private Action m_arriveAction;

	public void initBullet(ColleagueManager.ColleagueType colleagueType, int skinIndex, Vector2 targetPosition, Action<EnemyObject> arriveAction, EnemyObject targetEnemy)
	{
		currentFadeObject.setAlpha(1f);
		m_arriveAction = delegate
		{
			arriveAction(targetEnemy);
		};
		m_currentBulletAttributeData = Singleton<ColleagueManager>.instance.colleagueBulletDataList[(int)colleagueType].bulletAttributes[skinIndex - 1];
		targetPosition.y += 0.35f;
		bulletSpriteRenderer.sprite = m_currentBulletAttributeData.bulletSprite;
		ySpeed = UnityEngine.Random.Range(4f, 7f);
		if (m_currentBulletAttributeData.isImmediateBullet)
		{
			if (colleagueType == ColleagueManager.ColleagueType.Golem || colleagueType == ColleagueManager.ColleagueType.Trall)
			{
				m_arriveAction = (Action)Delegate.Combine(m_arriveAction, (Action)delegate
				{
					ObjectPool.Spawn("@ExplosionEffect", base.cachedTransform.position - ((colleagueType != ColleagueManager.ColleagueType.Golem) ? new Vector3(0f, 0f, 0f) : new Vector3(0f, 0.4758f, 0f)), new Vector3(90f, 0f, 0f));
				});
			}
			if (m_arriveAction != null)
			{
				m_arriveAction();
			}
			base.cachedTransform.position = targetPosition;
			recycleEvent();
			return;
		}
		setParabolicWithRotationLikeBomb(m_currentBulletAttributeData.isParabolic);
		setRotatable(m_currentBulletAttributeData.isRotatable);
		if (!m_currentBulletAttributeData.isRotatable)
		{
			setLookAtTarget(true);
		}
		else
		{
			setLookAtTarget(false);
		}
		moveTo(targetPosition, m_currentBulletAttributeData.speed, delegate
		{
			if (m_arriveAction != null)
			{
				m_arriveAction();
			}
			recycleEvent();
		});
	}

	private void recycleEvent()
	{
		if (m_currentBulletAttributeData.isImmediateBullet)
		{
			currentFadeObject.fadeIn(4f, delegate
			{
				ObjectPool.Recycle(base.name, base.cachedGameObject);
			});
		}
		else
		{
			ObjectPool.Recycle(base.name, base.cachedGameObject);
		}
	}
}
