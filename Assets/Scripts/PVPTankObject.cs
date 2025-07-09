using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPTankObject : PVPUnitObject
{
	public SpriteAnimation cachedSpriteAnimation;

	private bool m_isInitedTank;

	private PVPTankData m_currentTankData;

	public void initTank(PVPManager.PVPGameData gameData, bool isAlly, PVPTankData tankData)
	{
		m_currentGameData = gameData;
		changeProjectileAttribute(PVPProjectileManager.ProjectileAttributeType.NormalProjectile);
		m_currentTankData = tankData;
		m_isInitedTank = false;
		refreshStat();
		if (m_attackTimerCheckUpdateCoroutine != null)
		{
			StopCoroutine(m_attackTimerCheckUpdateCoroutine);
		}
		m_attackTimerCheckUpdateCoroutine = StartCoroutine(attackTimerCheckUpdate());
		cachedSpriteAnimation.animationType = "Tank" + ((int)tankData.tankIndex + 1);
		cachedSpriteAnimation.init();
		setStateLock(false);
		setState(PublicDataManager.State.Move);
	}

	protected override void refreshStat()
	{
		currentStatData = Singleton<PVPUnitManager>.instance.getCalculatedTankStat(m_currentGameData.statData, m_currentTankData.tankLevel);
	}

	protected override void moveInit()
	{
		cachedSpriteAnimation.playAnimation("Move", 0.1f, true);
		if (!m_isInitedTank)
		{
			Vector2 tankInitPosition = Singleton<PVPUnitManager>.instance.getTankInitPosition(isAlly, m_currentTankData.tankIndex);
			moveTo(tankInitPosition, currentStatData.moveSpeed, delegate
			{
				setStateLock(false);
				setState(PublicDataManager.State.Idle);
			});
		}
		else
		{
			setState(PublicDataManager.State.Idle);
		}
		setStateLock(true);
	}

	protected override void idleInit()
	{
		cachedSpriteAnimation.playFixAnimation("Move", 0);
	}

	protected override IEnumerator Idle()
	{
		idleInit();
		while (currentState == PublicDataManager.State.Idle)
		{
			if (!GameManager.isPause && m_isCanAttack && PVPManager.isPlayingPVP)
			{
				m_timerForAttack = 0f;
				setState(PublicDataManager.State.Attack);
			}
			yield return null;
		}
	}

	protected override IEnumerator Move()
	{
		moveInit();
		yield return null;
	}

	protected override bool searchEnemyUnit()
	{
		return true;
	}

	protected override void attackInit()
	{
		setStateLock(true);
		cachedSpriteAnimation.playAnimation("Attack", 0.1f, false, SpriteAnimation.FrameList.CenterFrame, delegate
		{
			attackEvent();
		}, delegate
		{
			attackEnd();
		});
	}

	public override void attackEvent()
	{
		if (!PVPManager.isPlayingPVP)
		{
			return;
		}
		Vector2 vector = Vector2.one * 1000f;
		Vector2 vector2 = Vector2.one * -1000f;
		List<PVPUnitObject> list = ((!isAlly) ? Singleton<PVPUnitManager>.instance.totalAllyList : Singleton<PVPUnitManager>.instance.totalEnemyList);
		for (int i = 0; i < list.Count; i++)
		{
			PVPUnitObject pVPUnitObject = list[i];
			if (pVPUnitObject.cachedTransform.position.x < vector.x)
			{
				vector.x = pVPUnitObject.cachedTransform.position.x;
			}
			if (pVPUnitObject.cachedTransform.position.y < vector.y)
			{
				vector.y = pVPUnitObject.cachedTransform.position.y;
			}
			if (pVPUnitObject.cachedTransform.position.x > vector2.x)
			{
				vector2.x = pVPUnitObject.cachedTransform.position.x;
			}
			if (pVPUnitObject.cachedTransform.position.y > vector2.y)
			{
				vector2.y = pVPUnitObject.cachedTransform.position.y;
			}
		}
		Vector2 arrivaPosition = new Vector2(Random.Range(vector.x, vector2.x), Random.Range(vector.y, vector2.y));
		arrivaPosition.x += ((!isAlly) ? 0.6f : (-0.6f));
		PVPProjectileManager.ProjectileData data = default(PVPProjectileManager.ProjectileData);
		data.sprite = Singleton<PVPProjectileManager>.instance.tankProjectileSprite;
		data.isParabolic = true;
		data.isRotatable = true;
		data.projectileSpeed = 8.5f;
		Singleton<AudioManager>.instance.playForceEffectSound("pvp_tank_shoot", -1f);
		Singleton<PVPProjectileManager>.instance.spawnProjectile(m_currentProjectileAttributeType, data, shootPoint.position, arrivaPosition, 1.2f, delegate(PVPProjectileManager.ProjectileArriveData arriveData)
		{
			Singleton<AudioManager>.instance.playForceEffectSound("pvp_tank_explosion", -1f);
			NewObjectPool.Spawn<GameObject>("@PVPExplosionEffect", arriveData.arrivePosition, new Vector3(90f, 0f, 0f));
			List<PVPUnitObject> nearestUnits = Singleton<PVPUnitManager>.instance.getNearestUnits(arriveData.arrivePosition, 7.5f, isAlly);
			double damage = currentStatData.damage;
			for (int j = 0; j < nearestUnits.Count; j++)
			{
				PVPUnitObject pVPUnitObject2 = nearestUnits[j];
				if (pVPUnitObject2 != null && !pVPUnitObject2.isDead)
				{
					double value = damage;
					pVPUnitObject2.decreaseHP(value);
				}
			}
		});
	}

	public override void attackEnd()
	{
		setStateLock(false);
		setState(PublicDataManager.State.Idle);
	}
}
