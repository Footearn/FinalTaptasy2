using System;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class TowerModeMonsterObject : MovingObject
{
	public EnemyManager.MonsterType currentMonsterType;

	public SpriteAnimation cachedSpriteAnimation;

	public AnimationNameData currentAnimationNameData;

	public float monsterSpeed;

	public bool isDead;

	public ObscuredInt currentHP;

	public TowerModeMapObject currentMapObject;

	public bool isBoss;

	public bool isMiniBoss;

	public Transform shootPoint;

	protected float m_collisionRange;

	protected PublicDataManager.State m_currentState;

	protected bool m_isRangedMonster;

	protected bool m_stateLock;

	protected bool m_isFirstMoveForBoss;

	private IEnumerator m_prevPlayingCoroutine;

	private GameObject m_fastMonsterEffect;

	public virtual void initMonster(EnemyManager.MonsterType monsterType, bool miniBoss, bool isRangedMonster = false, bool spawnedFromBoss = false)
	{
		m_isFirstMoveForBoss = true;
		setStateLock(false);
		isBoss = false;
		m_isRangedMonster = isRangedMonster;
		isMiniBoss = miniBoss;
		m_collisionRange = 0.3f;
		if (isMiniBoss)
		{
			currentHP = Singleton<TowerModeManager>.instance.getMiniBossHP(Singleton<TowerModeManager>.instance.currentDifficultyType);
			m_collisionRange = m_collisionRange * 1.5f + 0.5f;
		}
		else
		{
			currentHP = 1;
		}
		currentMonsterType = monsterType;
		if (m_fastMonsterEffect != null)
		{
			ObjectPool.Recycle(m_fastMonsterEffect.name, m_fastMonsterEffect);
			m_fastMonsterEffect = null;
		}
		if (!spawnedFromBoss)
		{
			if (!isRangedMonster && !isMiniBoss)
			{
				double num = UnityEngine.Random.Range(0, 10000);
				num /= 100.0;
				if (num <= Singleton<TowerModeManager>.instance.getSpeedMonsterSpawnChance(Singleton<TowerModeManager>.instance.currentDifficultyType))
				{
					m_fastMonsterEffect = ObjectPool.Spawn("@TowerModeFastMonsterEffect", new Vector3(-0.084f, 0.239f, 0f), base.cachedTransform);
					currentMonsterType = EnemyManager.MonsterType.Flamespirit1;
					monsterSpeed = 9f;
				}
				else
				{
					monsterSpeed = 3f;
				}
			}
			else
			{
				monsterSpeed = 3f;
			}
		}
		else
		{
			monsterSpeed = UnityEngine.Random.Range(3f, 4.3f);
		}
		isDead = false;
		EnemyManager.MonsterDetailAttributeData monsterDetailAttributeData = Singleton<EnemyManager>.instance.currentMonsterDetailAttributeDictionary[currentMonsterType];
		currentAnimationNameData = monsterDetailAttributeData.animationNameData;
		cachedSpriteAnimation.cachedTransform.localPosition = monsterDetailAttributeData.localPosition;
		cachedSpriteAnimation.cachedTransform.localScale = monsterDetailAttributeData.localScale;
		cachedSpriteAnimation.animationType = currentMonsterType.ToString();
		cachedSpriteAnimation.init();
		currentMapObject = Singleton<TowerModeManager>.instance.getStayingMapObject(base.cachedTransform.position);
		if (isMiniBoss)
		{
			base.cachedTransform.localScale = Vector3.one * 1.5f;
			setState(PublicDataManager.State.Idle);
			isRangedMonster = false;
		}
		else
		{
			base.cachedTransform.localScale = Vector3.one;
			if (isRangedMonster)
			{
				setState(PublicDataManager.State.Idle);
			}
			else
			{
				setState(PublicDataManager.State.Move);
			}
		}
		StopCoroutine("collisionCheckUpdate");
		StartCoroutine("collisionCheckUpdate");
		if (currentMapObject != null)
		{
			setDirection((currentMapObject.direction == Direction.Left) ? Direction.Right : Direction.Left);
		}
		else
		{
			DebugManager.LogError("Cannot find appropriate standing ground.");
		}
	}

	public void setStateLock(bool statelock)
	{
		m_stateLock = statelock;
	}

	public virtual void setState(PublicDataManager.State targetState)
	{
		if (!isDead && !m_stateLock)
		{
			setDirection((currentMapObject.direction == Direction.Left) ? Direction.Right : Direction.Left);
			StopCoroutine("waitForIdle");
			stopAll();
			if (m_prevPlayingCoroutine != null)
			{
				StopCoroutine(m_prevPlayingCoroutine);
			}
			m_currentState = targetState;
			m_prevPlayingCoroutine = getStateCoroutine(targetState);
			if (m_prevPlayingCoroutine != null && base.cachedGameObject.activeSelf)
			{
				StartCoroutine(m_prevPlayingCoroutine);
			}
		}
	}

	private IEnumerator getStateCoroutine(PublicDataManager.State targetState)
	{
		IEnumerator result = null;
		switch (targetState)
		{
		case PublicDataManager.State.Idle:
			result = Idle();
			break;
		case PublicDataManager.State.Move:
			result = Move();
			break;
		case PublicDataManager.State.Die:
			result = Die();
			break;
		case PublicDataManager.State.Attack:
			result = Attack();
			break;
		}
		return result;
	}

	protected virtual void idleInit()
	{
		cachedSpriteAnimation.playAnimation(currentAnimationNameData.idleName, 0.1f, true);
	}

	protected virtual IEnumerator Idle()
	{
		idleInit();
		float attackTimer = 0f;
		while (true)
		{
			if (!isMiniBoss && !m_isRangedMonster && !TowerModeManager.isPauseObstacleAndMonster)
			{
				setState(PublicDataManager.State.Move);
				yield break;
			}
			if (m_isRangedMonster && !TowerModeManager.isPauseObstacleAndMonster)
			{
				attackTimer += Time.deltaTime * GameManager.timeScale;
				if (attackTimer >= Singleton<TowerModeManager>.instance.getRangedMonsterAttackSpeed(Singleton<TowerModeManager>.instance.currentDifficultyType))
				{
					break;
				}
			}
			yield return null;
		}
		setState(PublicDataManager.State.Attack);
	}

	protected virtual void moveInit()
	{
		cachedSpriteAnimation.playAnimation(currentAnimationNameData.walkName, 0.1f, true);
		currentMapObject = Singleton<TowerModeManager>.instance.getStayingMapObject(base.cachedTransform.position);
		if (currentMapObject == null)
		{
			DebugManager.LogError("Can not find appropriate standing ground.");
		}
		if (!isMiniBoss)
		{
			moveTo(currentMapObject.getPoint(true).position, (!TowerModeManager.isPauseObstacleAndMonster) ? monsterSpeed : 0f, delegate
			{
				gotoPrevFloor();
			});
			return;
		}
		TowerModeCharacterObject curPlayingCharacter = Singleton<TowerModeManager>.instance.curPlayingCharacter;
		if (currentMapObject.curFloor == curPlayingCharacter.currentMapObject.curFloor)
		{
			if (m_isFirstMoveForBoss)
			{
				m_isFirstMoveForBoss = false;
				monsterSpeed = 3f;
			}
			else
			{
				monsterSpeed = Singleton<TowerModeManager>.instance.getMiniBossSpeed(Singleton<TowerModeManager>.instance.currentDifficultyType);
			}
			moveTo(currentMapObject.getPoint(true).position, monsterSpeed);
		}
	}

	protected virtual IEnumerator Move()
	{
		moveInit();
		while (!TowerModeManager.isPauseObstacleAndMonster || isMiniBoss)
		{
			yield return null;
		}
		setState(PublicDataManager.State.Idle);
	}

	protected virtual void dieEvent()
	{
		dropEvent();
		if (isMiniBoss)
		{
			Singleton<TowerModeManager>.instance.pauseAllObstaclesAndNormalMonsters(false);
			Singleton<TowerModeManager>.instance.isFightingWithBoss = false;
		}
		cachedSpriteAnimation.playAnimation(currentAnimationNameData.dieName, 0.1f, false);
		ObjectPool.Spawn("fx_boss_blowup", cachedSpriteAnimation.cachedTransform.position);
		recycleMonster();
		isDead = true;
	}

	protected virtual void dropEvent()
	{
		if (Singleton<TowerModeManager>.instance.isPlayWithFreeTicket)
		{
			return;
		}
		if (isMiniBoss)
		{
			Singleton<ElopeModeManager>.instance.increaseHeartCoin(1L, false);
			Singleton<DropItemManager>.instance.spawnDropItem(DropItemManager.DropItemType.HeartCoin, base.cachedTransform.position, 1.0);
		}
		else if (isBoss)
		{
			Singleton<ElopeModeManager>.instance.increaseHeartCoin(5L, false);
			for (int i = 0; i < 5; i++)
			{
				Singleton<DropItemManager>.instance.spawnDropItem(DropItemManager.DropItemType.HeartCoin, base.cachedTransform.position, 1.0);
			}
		}
	}

	protected virtual IEnumerator Die()
	{
		dieEvent();
		yield return null;
	}

	protected virtual void attackInit()
	{
		cachedSpriteAnimation.playAnimation(currentAnimationNameData.attackName, 0.1f, true, SpriteAnimation.FrameList.CenterFrame, delegate
		{
			Singleton<TowerModeManager>.instance.spawnProjectile(shootPoint.position, getDirectionEnum(), currentMonsterType);
		}, delegate
		{
			setState(PublicDataManager.State.Idle);
		});
	}

	protected virtual IEnumerator Attack()
	{
		attackInit();
		yield return null;
	}

	private void gotoPrevFloor()
	{
		currentMapObject = Singleton<TowerModeManager>.instance.getPrevMapObject(currentMapObject);
		if (currentMapObject != null)
		{
			base.cachedTransform.position = currentMapObject.getPoint(false).position;
			setState(PublicDataManager.State.Move);
			setDirection((currentMapObject.direction == Direction.Left) ? Direction.Right : Direction.Left);
		}
		else
		{
			recycleMonster();
		}
	}

	public void recycleMonster(bool withRemoveFromList = true)
	{
		if (m_fastMonsterEffect != null)
		{
			ObjectPool.Recycle(m_fastMonsterEffect.name, m_fastMonsterEffect);
			m_fastMonsterEffect = null;
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
		if (withRemoveFromList)
		{
			Singleton<TowerModeManager>.instance.listTowerModeMonsterObjects.Remove(this);
		}
	}

	protected IEnumerator collisionCheckUpdate()
	{
		TowerModeCharacterObject targetChr2 = null;
		while (!Singleton<TowerModeManager>.instance.isDead)
		{
			if (!GameManager.isPause)
			{
				targetChr2 = Singleton<TowerModeManager>.instance.getNearestCharacter(base.cachedTransform.position, m_collisionRange);
				if (targetChr2 != null)
				{
					targetChr2.decreaseLifeCount();
					decreaseHP(1);
				}
			}
			yield return null;
		}
	}

	public virtual void decreaseHP(int value)
	{
		currentHP = Math.Max((int)currentHP - value, 0);
		if ((int)currentHP <= 0)
		{
			setState(PublicDataManager.State.Die);
			return;
		}
		setState(PublicDataManager.State.Idle);
		jump(new Vector2(UnityEngine.Random.Range(3.5f, 9f) * (float)((currentMapObject.direction != 0) ? 1 : (-1)), 2.5f), Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position).y, delegate
		{
			setState(PublicDataManager.State.Move);
		});
	}
}
