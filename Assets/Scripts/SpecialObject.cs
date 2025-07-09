using System.Collections;
using UnityEngine;

public class SpecialObject : EnemyObject
{
	public EnemyManager.SpecialType currentSpecialType;

	private Vector2 m_basePosition;

	public override void init(int level)
	{
		isBoss = false;
		base.init(level);
		if (currentSpecialType != EnemyManager.SpecialType.Dummy)
		{
			Singleton<EnemyManager>.instance.enemyList.Add(this);
		}
		m_basePosition = base.cachedTransform.position;
	}

	protected override void setUpdatedProperties()
	{
		curDamage = baseDamage;
		curHealth = baseHealth;
		curDelay = CalculateManager.getCurrentDelay(baseDelay);
		curSpeed = CalculateManager.getCurrentMoveSpeed(baseSpeed);
		m_speed = curSpeed;
		curAttackRange = baseAttackRange;
		maxHealth = curHealth;
	}

	protected override void idleInit()
	{
		cachedSpriteAnimation.playAnimation(currentAnimationName.idleName, 0.2f);
	}

	protected override IEnumerator Idle()
	{
		idleInit();
		yield return null;
	}

	protected override void moveInit()
	{
	}

	protected override IEnumerator Move()
	{
		moveInit();
		yield return null;
	}

	protected override void dropEvent()
	{
	}

	protected override IEnumerator dieEffect()
	{
		yield return null;
	}

	protected override IEnumerator Wait()
	{
		waitEvent();
		m_waitTimer = 0f;
		bool isPlaying = false;
		while (true)
		{
			if (!m_isAttacking && !isPlaying)
			{
				isPlaying = true;
				cachedSpriteAnimation.playAnimation(currentAnimationName.idleName, 0.2f);
			}
			m_waitTimer += Time.deltaTime * GameManager.timeScale;
			if (m_waitTimer >= curDelay)
			{
				break;
			}
			yield return null;
		}
		m_waitTimer = 0f;
		setState(PublicDataManager.State.Idle);
	}
}
