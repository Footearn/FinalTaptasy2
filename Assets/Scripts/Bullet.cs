using System;
using System.Collections;
using UnityEngine;

public class Bullet : MovingObject
{
	public SpriteRenderer targetRenderer;

	public float bulletSpeed;

	public bool isRotatable;

	public bool isParabolic;

	public bool isBomb;

	public bool isLookAtTarget;

	public float recycleTime;

	private Sprite m_cachedArrowSprite;

	private void Awake()
	{
		setLookAtTarget(isLookAtTarget);
		setParabolic(isParabolic);
		setRotatable(isRotatable);
		setBomb(isBomb);
	}

	public void shootBullet(Transform targetTransform, Action<EnemyObject> arriveAction, Vector2 offset, EnemyObject targetEnemy)
	{
		Action arriveAction2 = delegate
		{
			arriveAction(targetEnemy);
			recycleBullet();
		};
		m_offset = offset;
		moveTo(targetTransform, bulletSpeed, arriveAction2);
	}

	public void shootBullet(Vector2 targetPosition, Action<EnemyObject> arriveAction, Vector2 offset, EnemyObject targetEnemy)
	{
		Action arriveAction2 = delegate
		{
			arriveAction(targetEnemy);
			recycleBullet();
		};
		moveTo(targetPosition, bulletSpeed, arriveAction2);
		m_offset = offset;
	}

	public void shootBullet(Vector2 targetPosition, Action<CharacterObject> arriveAction, Vector2 offset, CharacterObject targetCharacter)
	{
		Action arriveAction2 = delegate
		{
			arriveAction(targetCharacter);
			recycleBullet();
		};
		moveTo(targetPosition, bulletSpeed, arriveAction2);
		m_offset = offset;
	}

	public void shootBullet(Vector2 targetPosition, Action<Transform> arriveAction, Vector2 offset)
	{
		moveTo(targetPosition, bulletSpeed, delegate
		{
			arriveAction(base.cachedTransform);
		});
		m_offset = offset;
	}

	public void shootBullet(Vector2 targetPosition, Action<Bullet> arriveAction, Vector2 offset)
	{
		moveTo(targetPosition, bulletSpeed, delegate
		{
			arriveAction(this);
		});
		m_offset = offset;
	}

	public void changeArrowSprite(Sprite sprite = null)
	{
		m_cachedArrowSprite = sprite;
		if (m_cachedArrowSprite == null)
		{
			m_cachedArrowSprite = Singleton<SkillManager>.instance.normalArrowSprite;
		}
		targetRenderer.sprite = m_cachedArrowSprite;
	}

	public void recycleBullet()
	{
		if (recycleTime <= 0f)
		{
			m_cachedArrowSprite = null;
			ObjectPool.Recycle(base.name, base.cachedGameObject);
		}
		else
		{
			StopCoroutine("recycleUpdate");
			StartCoroutine("recycleUpdate");
		}
	}

	private IEnumerator recycleUpdate()
	{
		float timer = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= recycleTime)
				{
					break;
				}
			}
			yield return null;
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
