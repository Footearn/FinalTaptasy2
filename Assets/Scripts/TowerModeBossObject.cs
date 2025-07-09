using System;
using System.Collections;
using UnityEngine;

public class TowerModeBossObject : TowerModeMonsterObject
{
	public enum BossSkillAttackType
	{
		None = -1,
		SpawnMonster,
		Stun,
		Length
	}

	public Animation cachedBoneAnimation;

	public BoneAnimationNameData currentBoneAnimationName;

	private BossSkillAttackType m_targetBossSkillAttackType = BossSkillAttackType.None;

	private string m_currentAnimationName;

	public void initBossMonster()
	{
		StopCoroutine("waitForIdle");
		m_targetBossSkillAttackType = BossSkillAttackType.None;
		setStateLock(false);
		isBoss = true;
		m_isRangedMonster = false;
		isMiniBoss = false;
		m_currentAnimationName = string.Empty;
		m_collisionRange = 1f;
		currentHP = Singleton<TowerModeManager>.instance.getBossMaxHP();
		isDead = false;
		currentMapObject = Singleton<TowerModeManager>.instance.getStayingMapObject(base.cachedTransform.position);
		setState(PublicDataManager.State.Idle);
		StopCoroutine("waitForDieEffect");
		StopCoroutine("collisionCheckUpdate");
		StartCoroutine("collisionCheckUpdate");
		StopCoroutine("animationSpeedUpdate");
		StartCoroutine("animationSpeedUpdate");
		if (currentMapObject != null)
		{
			setDirection((currentMapObject.direction == Direction.Left) ? Direction.Right : Direction.Left);
		}
		else
		{
			DebugManager.LogError("Cannot find appropriate standing ground.");
		}
	}

	private IEnumerator animationSpeedUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				for (int l = 0; l < currentBoneAnimationName.moveName.Length; l++)
				{
					cachedBoneAnimation[currentBoneAnimationName.moveName[l]].speed = m_speed / 3f * GameManager.timeScale;
				}
				cachedBoneAnimation[currentBoneAnimationName.idleName[0]].speed = GameManager.timeScale;
				for (int k = 0; k < currentBoneAnimationName.attackName.Length; k++)
				{
					cachedBoneAnimation[currentBoneAnimationName.attackName[k]].speed = 1f;
				}
			}
			else
			{
				for (int j = 0; j < currentBoneAnimationName.moveName.Length; j++)
				{
					cachedBoneAnimation[currentBoneAnimationName.moveName[j]].speed = 0f;
				}
				cachedBoneAnimation[currentBoneAnimationName.idleName[0]].speed = GameManager.timeScale;
				for (int i = 0; i < currentBoneAnimationName.attackName.Length; i++)
				{
					cachedBoneAnimation[currentBoneAnimationName.attackName[i]].speed = 0f;
				}
			}
			yield return null;
		}
	}

	public override void initMonster(EnemyManager.MonsterType monsterType, bool miniBoss, bool isRangedMonster = false, bool spawnedFromBoss = false)
	{
	}

	public void doBossRandomAction()
	{
		setState(PublicDataManager.State.Idle);
		StartCoroutine("waitForIdle");
	}

	private IEnumerator waitForIdle()
	{
		bool isWillAttack = false;
		double randomForAction2 = UnityEngine.Random.Range(0, 10000);
		randomForAction2 /= 100.0;
		isWillAttack = ((!(randomForAction2 < 75.0)) ? true : false);
		float timer = 0f;
		float targetTime = ((!isWillAttack) ? 0f : UnityEngine.Random.Range(0.01f, 0.15f));
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= targetTime)
				{
					break;
				}
			}
			yield return null;
		}
		if (!isWillAttack)
		{
			setState(PublicDataManager.State.Move);
		}
		else
		{
			doSkillAttack();
		}
	}

	public void playBoneAnimation(string animationName)
	{
		if (m_currentAnimationName != animationName)
		{
			m_currentAnimationName = animationName;
			cachedBoneAnimation.Stop();
			cachedBoneAnimation.Play(animationName);
		}
	}

	protected override void idleInit()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
	}

	protected override IEnumerator Idle()
	{
		idleInit();
		yield return null;
	}

	protected override void moveInit()
	{
		playBoneAnimation(currentBoneAnimationName.moveName[0]);
		TowerModeCharacterObject curPlayingCharacter = Singleton<TowerModeManager>.instance.curPlayingCharacter;
		if (currentMapObject.curFloor != curPlayingCharacter.currentMapObject.curFloor)
		{
			return;
		}
		if (m_isFirstMoveForBoss)
		{
			m_isFirstMoveForBoss = false;
			monsterSpeed = 3f;
		}
		else
		{
			monsterSpeed = Singleton<TowerModeManager>.instance.getBossMoveSpeed();
		}
		double num = UnityEngine.Random.Range(0, 10000);
		num /= 100.0;
		if (num < 30.0)
		{
			Vector2 originPosition = base.cachedTransform.position;
			Vector2 targetPosition = new Vector2(0f, currentMapObject.getPoint(true).position.y);
			targetPosition.x = (base.cachedTransform.position.x + Singleton<TowerModeManager>.instance.curPlayingCharacter.cachedTransform.position.x) / 2f;
			setDirection((currentMapObject.direction == Direction.Left) ? Direction.Right : Direction.Left);
			moveTo(targetPosition, monsterSpeed, delegate
			{
				setDirection(currentMapObject.direction);
				moveTo(originPosition, monsterSpeed, delegate
				{
					setDirection((currentMapObject.direction == Direction.Left) ? Direction.Right : Direction.Left);
					doBossRandomAction();
				});
			});
		}
		else
		{
			moveTo(currentMapObject.getPoint(true).position, monsterSpeed);
		}
	}

	protected override IEnumerator Move()
	{
		moveInit();
		yield return null;
	}

	protected override void attackInit()
	{
		m_currentAnimationName = string.Empty;
		switch (m_targetBossSkillAttackType)
		{
		case BossSkillAttackType.SpawnMonster:
			playBoneAnimation(currentBoneAnimationName.attackName[0]);
			break;
		case BossSkillAttackType.Stun:
			playBoneAnimation(currentBoneAnimationName.attackName[1]);
			break;
		}
	}

	protected override IEnumerator Attack()
	{
		attackInit();
		yield return null;
	}

	public void doSkillAttack()
	{
		double num = UnityEngine.Random.Range(0, 10000);
		num /= 100.0;
		if (num < 50.0)
		{
			m_targetBossSkillAttackType = BossSkillAttackType.SpawnMonster;
		}
		else if (Singleton<TowerModeManager>.instance.listTowerModeMonsterObjects.Count > 1)
		{
			m_targetBossSkillAttackType = BossSkillAttackType.SpawnMonster;
		}
		else
		{
			m_targetBossSkillAttackType = BossSkillAttackType.Stun;
		}
		setState(PublicDataManager.State.Attack);
	}

	public void spawnMonsterAttackEvent()
	{
		Singleton<TowerModeManager>.instance.spawnMonsterObject(base.cachedTransform.position, false, false, true);
	}

	public void endSpawnMonsterAttackEvent()
	{
		double num = UnityEngine.Random.Range(0, 10000);
		num /= 100.0;
		if (num < 80.0)
		{
			m_targetBossSkillAttackType = BossSkillAttackType.SpawnMonster;
			setState(PublicDataManager.State.Attack);
		}
		else
		{
			doBossRandomAction();
		}
	}

	public void stunAttackEvent()
	{
		ShakeCamera.Instance.shake(3f, 1f);
		Singleton<TowerModeManager>.instance.curPlayingCharacter.setStun(Singleton<TowerModeManager>.instance.getMaxStunDuration());
	}

	public void endStunAttackEvent()
	{
		doBossRandomAction();
	}

	protected override void dieEvent()
	{
		dropEvent();
		StopCoroutine("collisionCheckUpdate");
		Singleton<TowerModeManager>.instance.listTowerModeMonsterObjects.Remove(this);
		isDead = true;
		Singleton<TowerModeManager>.instance.curPlayingCharacter.setState(PublicDataManager.State.Wait);
		Singleton<TowerModeManager>.instance.curPlayingCharacter.setStateLock(true);
		Singleton<TowerModeManager>.instance.isDeadBoss = true;
		playBoneAnimation(currentBoneAnimationName.dieName[0]);
		Singleton<AudioManager>.instance.stopBackgroundSound();
		Singleton<TowerModeManager>.instance.isFightingWithBoss = false;
		StopCoroutine("waitForDieEffect");
		StartCoroutine("waitForDieEffect");
	}

	protected override IEnumerator Die()
	{
		dieEvent();
		yield return null;
	}

	private IEnumerator waitForDieEffect()
	{
		yield return new WaitForSeconds(3f);
		if (Singleton<TowerModeManager>.instance.curPlayingCharacter != null)
		{
			Singleton<TowerModeManager>.instance.curPlayingCharacter.setStateLock(false);
			Singleton<TowerModeManager>.instance.curPlayingCharacter.setState(PublicDataManager.State.Move);
		}
	}

	public override void decreaseHP(int value)
	{
		currentHP = Math.Max((int)currentHP - value, 0);
		UIWindowTowerMode.instance.bossHPGauge.setProgress((int)currentHP, Singleton<TowerModeManager>.instance.getBossMaxHP());
		if ((int)currentHP <= 0)
		{
			setState(PublicDataManager.State.Die);
			return;
		}
		setState(PublicDataManager.State.Idle);
		jump(new Vector2(9.5f * (float)((currentMapObject.direction != 0) ? 1 : (-1)), 2.5f), Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position).y, delegate
		{
			doBossRandomAction();
		});
	}
}
