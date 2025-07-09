using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColleagueObject : MovingObject
{
	public ColleagueManager.ColleagueType currentColleagueType;

	public Transform shootPoint;

	public double curDamage;

	public float curSpeed;

	public float curDelay;

	public string attackSFXSoundName;

	public float baseYPosition;

	public Ground currentGround;

	public int currentStayingGroundIndex;

	public int index;

	public bool isAttackable;

	public bool isSplashAttack;

	public string arriveBulletEffectName;

	public EnemyObject currentAttackingTargetEnemy;

	public float targetXOffsetForNormalDungeon;

	public float targetXOffsetForBossRaid;

	public float currentXOffset;

	public BoneAnimationNameData currentBoneAnimationName;

	public CharacterManager.CharacterBoneAnimationSpriteRendererData colleagueBoneSpriteRendererData;

	public Animation characterBoneAnimation;

	private PublicDataManager.State m_currentState;

	private bool m_stateLock;

	protected Vector2 m_attackTargetPosition;

	protected float m_attackLimitedMaxTimer;

	private bool m_isStair;

	private int m_stairCount;

	private CharacterWarrior m_cachedWarriorChracter;

	private float m_attackTimer;

	private bool m_isAttacking;

	private ColleagueInventoryData m_currentColleagueInventoryData;

	private string m_currentAnimationName;

	private IEnumerator m_prevPlayingCoroutine;

	public virtual void initColleague()
	{
		m_currentAnimationName = string.Empty;
		m_currentColleagueInventoryData = Singleton<ColleagueManager>.instance.getColleagueInventoryData(currentColleagueType);
		Singleton<ColleagueManager>.instance.changeColleagueSkin(currentColleagueType, m_currentColleagueInventoryData.currentEquippedSkinIndex, colleagueBoneSpriteRendererData);
		curDelay = Singleton<ColleagueManager>.instance.getColleagueDelay(currentColleagueType);
		curDamage = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay);
		m_attackLimitedMaxTimer = 0f;
		fixedDirection = true;
		StopAllCoroutines();
		m_isAttacking = false;
		m_attackTimer = curDelay;
		currentStayingGroundIndex = 0;
		m_stairCount = 0;
		m_isStair = false;
		setStateLock(false);
		StopCoroutine("attackEndCheckUpdate");
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			currentXOffset = targetXOffsetForBossRaid;
		}
		else
		{
			currentXOffset = targetXOffsetForNormalDungeon;
		}
		m_cachedWarriorChracter = Singleton<CharacterManager>.instance.warriorCharacter;
		setDirection(Direction.Right);
		curSpeed = 3f;
		setState(PublicDataManager.State.Move);
		if (isAttackable)
		{
			StopCoroutine("attackCheckUpdate");
			StartCoroutine("attackCheckUpdate");
		}
		StopCoroutine("animationSpeedUpdate");
		StartCoroutine("animationSpeedUpdate");
	}

	public void resetColleagueStartPosition()
	{
		int count = Singleton<ColleagueManager>.instance.currentColleagueObject.Count;
		Vector3 position = base.cachedTransform.position;
		position = m_cachedWarriorChracter.cachedTransform.position - new Vector3(currentXOffset, 0f, 0f);
		position.z = 0.5f + (float)count * 0.2f - currentXOffset;
		base.cachedTransform.position = position;
	}

	private IEnumerator animationSpeedUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				for (int l = 0; l < currentBoneAnimationName.attackName.Length; l++)
				{
					characterBoneAnimation[currentBoneAnimationName.attackName[l]].speed = Mathf.Clamp(1.5f / Mathf.Max(curDelay, 0.7f), 0.7f, 1f) * GameManager.timeScale;
				}
				for (int k = 0; k < currentBoneAnimationName.moveName.Length; k++)
				{
					characterBoneAnimation[currentBoneAnimationName.moveName[k]].speed = curSpeed / 3f * GameManager.timeScale;
				}
				characterBoneAnimation[currentBoneAnimationName.idleName[0]].speed = GameManager.timeScale;
			}
			else
			{
				for (int j = 0; j < currentBoneAnimationName.attackName.Length; j++)
				{
					characterBoneAnimation[currentBoneAnimationName.attackName[j]].speed = 0f;
				}
				for (int i = 0; i < currentBoneAnimationName.moveName.Length; i++)
				{
					characterBoneAnimation[currentBoneAnimationName.moveName[i]].speed = 0f;
				}
				characterBoneAnimation[currentBoneAnimationName.idleName[0]].speed = 0f;
			}
			yield return null;
		}
	}

	protected virtual IEnumerator attackCheckUpdate()
	{
		yield return null;
		while (true)
		{
			if (!GameManager.isPause)
			{
				m_attackTimer += Time.deltaTime * GameManager.timeScale;
				if (m_cachedWarriorChracter != null && !m_cachedWarriorChracter.isDead && currentGround != null && !currentGround.isFirstGround && m_cachedWarriorChracter.getState() != PublicDataManager.State.Wait && m_cachedWarriorChracter.targetEnemy != null)
				{
					currentAttackingTargetEnemy = m_cachedWarriorChracter.targetEnemy;
					if (currentStayingGroundIndex == m_cachedWarriorChracter.currentStayingGroundIndex && Mathf.Abs(base.cachedTransform.position.x - m_cachedWarriorChracter.cachedTransform.position.x) < currentXOffset + 1.5f && m_attackTimer >= curDelay)
					{
						m_attackTimer = 0f;
						setState(PublicDataManager.State.Attack);
					}
				}
				else
				{
					currentAttackingTargetEnemy = null;
				}
			}
			yield return null;
		}
	}

	public void playBoneAnimation(string animationName)
	{
		if (m_currentAnimationName != animationName)
		{
			m_currentAnimationName = animationName;
			characterBoneAnimation.Stop();
			characterBoneAnimation.Play(animationName);
		}
	}

	public void setState(PublicDataManager.State targetState)
	{
		if (!m_stateLock)
		{
			stopAll();
			if (m_prevPlayingCoroutine != null)
			{
				StopCoroutine(m_prevPlayingCoroutine);
			}
			m_currentState = targetState;
			m_prevPlayingCoroutine = getStateCoroutine(targetState);
			if (m_prevPlayingCoroutine != null)
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
		case PublicDataManager.State.Attack:
			result = Attack();
			break;
		case PublicDataManager.State.Wait:
			result = Wait();
			break;
		}
		return result;
	}

	public void setStateLock(bool targetState)
	{
		m_stateLock = targetState;
	}

	protected virtual void idleInit()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
	}

	protected virtual IEnumerator Idle()
	{
		yield return null;
		idleInit();
		int currentGroundIndex2 = 0;
		int warriorGroundIndex2 = 0;
		bool isCastingDivineSkillWhenSetStateToIdle = m_cachedWarriorChracter.isCastingSkill;
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (isCastingDivineSkillWhenSetStateToIdle)
				{
					if (!m_cachedWarriorChracter.isCastingSkill)
					{
						setState(PublicDataManager.State.Move);
						yield break;
					}
					yield return null;
					continue;
				}
				yield return null;
				if (m_cachedWarriorChracter.currentGround != null || m_cachedWarriorChracter.currentGround.inPoint == null || m_cachedWarriorChracter.currentGround.outPoint == null)
				{
					yield return null;
				}
				bool isCanSetStateToMove = false;
				Direction targetMoveDirection = MovingObject.calculateDirection(m_cachedWarriorChracter.currentGround.inPoint.position, m_cachedWarriorChracter.currentGround.outPoint.position);
				currentGroundIndex2 = currentStayingGroundIndex;
				warriorGroundIndex2 = m_cachedWarriorChracter.currentStayingGroundIndex;
				if (currentGroundIndex2 < warriorGroundIndex2)
				{
					if (GameManager.currentDungeonType != GameManager.SpecialDungeonType.BossRaid)
					{
						float distance4 = Vector2.Distance(base.cachedTransform.position, currentGround.outPoint.position) + Vector2.Distance(currentGround.outPoint.position, m_cachedWarriorChracter.cachedTransform.position);
						if (distance4 > currentXOffset)
						{
							setState(PublicDataManager.State.Move);
							yield break;
						}
					}
					else
					{
						setState(PublicDataManager.State.Move);
					}
				}
				else
				{
					if (currentGroundIndex2 != warriorGroundIndex2)
					{
						continue;
					}
					float distance3 = 0f;
					if (!BossRaidManager.isBossRaid && !(Vector2.Distance(m_cachedWarriorChracter.cachedTransform.position, m_cachedWarriorChracter.currentGround.outPoint.position) < Vector2.Distance(base.cachedTransform.position, m_cachedWarriorChracter.currentGround.outPoint.position)))
					{
						continue;
					}
					if (!m_isStair)
					{
						distance3 = Vector2.Distance(base.cachedTransform.position, m_cachedWarriorChracter.cachedTransform.position);
						if (targetMoveDirection == Direction.Left)
						{
							if (base.cachedTransform.position.x - currentXOffset > m_cachedWarriorChracter.cachedTransform.position.x)
							{
								isCanSetStateToMove = true;
							}
						}
						else if (base.cachedTransform.position.x + currentXOffset < m_cachedWarriorChracter.cachedTransform.position.x)
						{
							isCanSetStateToMove = true;
						}
						if (isCanSetStateToMove && distance3 > currentXOffset + 0.3f)
						{
							setState(PublicDataManager.State.Move);
							yield break;
						}
					}
					else
					{
						distance3 = Vector2.Distance(base.cachedTransform.position, currentGround.inPoint.position) + Vector2.Distance(m_cachedWarriorChracter.cachedTransform.position, m_cachedWarriorChracter.currentGround.inPoint.position);
						if (distance3 > currentXOffset + 0.3f)
						{
							break;
						}
					}
				}
			}
			else
			{
				yield return null;
			}
		}
		setState(PublicDataManager.State.Move);
	}

	private void stairMoveEvent()
	{
		Vector2 zero = Vector2.zero;
		zero = ((currentGround.stairpoint.Length <= 0 || m_stairCount >= currentGround.stairpoint.Length) ? ((Vector2)currentGround.inPoint.position) : ((Vector2)currentGround.stairpoint[m_stairCount].position));
		setDirection(MovingObject.calculateDirection(base.cachedTransform.position, zero));
		moveTo(zero, curSpeed, delegate
		{
			if (currentGround.stairpoint.Length > 0 && m_stairCount < currentGround.stairpoint.Length)
			{
				m_stairCount++;
				stairMoveEvent();
			}
			else
			{
				m_isStair = false;
				setState(PublicDataManager.State.Move);
			}
		});
	}

	protected virtual void moveInit()
	{
		playBoneAnimation(currentBoneAnimationName.moveName[0]);
		if (!m_isStair)
		{
			currentGround = Singleton<GroundManager>.instance.getStayingGround(base.cachedTransform.position);
			currentStayingGroundIndex = currentGround.currnetFloor;
		}
		if (currentGround == null)
		{
			DebugManager.LogError("currentGround is null");
			return;
		}
		if (currentGround.isFirstGround)
		{
			setStateLock(true);
			moveTo(currentGround.downPoint.position, curSpeed, delegate
			{
				jump(new Vector2(2.5f, 4f), Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position, 1f).y, delegate
				{
					setDirection(Direction.Left);
					setStateLock(false);
					setState(PublicDataManager.State.Move);
				});
			});
			return;
		}
		int currentGroundIndex = currentStayingGroundIndex;
		int num = m_cachedWarriorChracter.currentStayingGroundIndex;
		if (currentGroundIndex < num && currentGround == m_cachedWarriorChracter.currentGround)
		{
			currentStayingGroundIndex = (currentGroundIndex = num);
		}
		bool flag = Singleton<GroundManager>.instance.isGroundBiggerThanTargetGround(currentGround, m_cachedWarriorChracter.currentGround);
		if (currentGroundIndex < num)
		{
			if (!m_isStair)
			{
				setDontStop(false);
				Vector2 zero = Vector2.zero;
				Ground nextGround = Singleton<GroundManager>.instance.getNextGround(currentGround);
				if (nextGround != null)
				{
					int num2 = ((MovingObject.calculateDirection(currentGround.inPoint.position, currentGround.outPoint.position) != 0) ? 1 : (-1));
					zero = ((!nextGround.isBossGround || nextGround.stairpoint.Length != 1) ? ((Vector2)currentGround.outPoint.position) : ((Vector2)currentGround.outPoint.position + new Vector2(-num2, 0f)));
					setDirection(MovingObject.calculateDirection(base.cachedTransform.position, zero));
					moveTo(zero, curSpeed, delegate
					{
						m_isStair = true;
						currentGround = Singleton<GroundManager>.instance.getNextGround(currentGround);
						currentStayingGroundIndex = currentGround.currnetFloor;
						m_stairCount = 0;
						stairMoveEvent();
					});
				}
			}
			else
			{
				stairMoveEvent();
			}
		}
		else if (currentGroundIndex == num)
		{
			if (!m_isStair || flag)
			{
				Action arriveAction = null;
				if (flag)
				{
					arriveAction = delegate
					{
						currentGroundIndex = m_cachedWarriorChracter.currentStayingGroundIndex;
						currentGround = m_cachedWarriorChracter.currentGround;
					};
				}
				setDontStop(true);
				setDirection(MovingObject.calculateDirection(base.cachedTransform.position, m_cachedWarriorChracter.cachedTransform.position));
				moveTo(m_cachedWarriorChracter.cachedTransform, (0f - currentXOffset) * m_cachedWarriorChracter.getDirection(), curSpeed, arriveAction);
			}
			else
			{
				stairMoveEvent();
			}
		}
		else
		{
			setState(PublicDataManager.State.Idle);
		}
	}

	protected virtual IEnumerator Move()
	{
		yield return null;
		moveInit();
		float distanceWithWarrior = 0f;
		bool isSameGroundWithWarriorWhenInitMove2 = false;
		if (!currentGround.isFirstGround)
		{
			isSameGroundWithWarriorWhenInitMove2 = m_cachedWarriorChracter.currentStayingGroundIndex == currentStayingGroundIndex;
		}
		while (true)
		{
			if (!GameManager.isPause)
			{
				distanceWithWarrior = Mathf.Lerp(distanceWithWarrior, Singleton<CharacterManager>.instance.getDistanceWithWarriorForColleague(this) - currentXOffset, Time.deltaTime * GameManager.timeScale * 5f);
				curSpeed = m_cachedWarriorChracter.curSpeed * Mathf.Clamp(distanceWithWarrior, 1f, 2.7f);
				setSpeed(curSpeed);
				if (m_cachedWarriorChracter.isCastingSkill)
				{
					setState(PublicDataManager.State.Idle);
					yield break;
				}
				yield return null;
				if (!(currentGround != null) || currentGround.isFirstGround)
				{
					continue;
				}
				Direction targetMoveDirection = MovingObject.calculateDirection(m_cachedWarriorChracter.currentGround.inPoint.position, m_cachedWarriorChracter.currentGround.outPoint.position);
				int currentGroundIndex = currentStayingGroundIndex;
				int warriorGroundIndex = m_cachedWarriorChracter.currentStayingGroundIndex;
				if (currentGroundIndex > warriorGroundIndex)
				{
					setState(PublicDataManager.State.Idle);
					yield break;
				}
				if (currentGroundIndex == warriorGroundIndex)
				{
					float distance2 = 0f;
					if (!BossRaidManager.isBossRaid && Vector2.Distance(m_cachedWarriorChracter.cachedTransform.position, m_cachedWarriorChracter.currentGround.outPoint.position) > Vector2.Distance(base.cachedTransform.position, m_cachedWarriorChracter.currentGround.outPoint.position))
					{
						setState(PublicDataManager.State.Idle);
						yield break;
					}
					if (!m_isStair)
					{
						distance2 = Vector2.Distance(base.cachedTransform.position, m_cachedWarriorChracter.cachedTransform.position);
						if (targetMoveDirection == Direction.Left)
						{
							if (base.cachedTransform.position.x - currentXOffset < m_cachedWarriorChracter.cachedTransform.position.x)
							{
								setState(PublicDataManager.State.Idle);
							}
						}
						else if (base.cachedTransform.position.x + currentXOffset > m_cachedWarriorChracter.cachedTransform.position.x)
						{
							setState(PublicDataManager.State.Idle);
						}
					}
					else
					{
						distance2 = Vector2.Distance(base.cachedTransform.position, currentGround.inPoint.position) + Vector2.Distance(m_cachedWarriorChracter.cachedTransform.position, m_cachedWarriorChracter.currentGround.inPoint.position);
					}
					if (distance2 <= currentXOffset + 0.2f)
					{
						setState(PublicDataManager.State.Idle);
						yield break;
					}
				}
				if (isSameGroundWithWarriorWhenInitMove2 && m_cachedWarriorChracter.currentStayingGroundIndex != currentStayingGroundIndex)
				{
					break;
				}
			}
			else
			{
				yield return null;
			}
		}
		isSameGroundWithWarriorWhenInitMove2 = false;
		setState(PublicDataManager.State.Move);
	}

	protected IEnumerator attackEndCheckUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (m_currentState == PublicDataManager.State.Attack)
				{
					m_attackLimitedMaxTimer += Time.deltaTime * GameManager.timeScale;
				}
				if (m_currentState == PublicDataManager.State.Attack && (!characterBoneAnimation.isPlaying || m_attackLimitedMaxTimer >= curDelay + 0.2f))
				{
					break;
				}
			}
			yield return null;
		}
		attackEndEvent();
	}

	protected virtual void attackInit()
	{
		StopCoroutine("attackEndCheckUpdate");
		StartCoroutine("attackEndCheckUpdate");
		m_attackLimitedMaxTimer = 0f;
		m_attackTargetPosition = currentAttackingTargetEnemy.cachedTransform.position;
		m_isAttacking = true;
		setDirection(MovingObject.calculateDirection(base.cachedTransform.position, currentAttackingTargetEnemy.cachedTransform.position));
		playBoneAnimation(currentBoneAnimationName.attackName[0]);
	}

	public virtual void attackEndEvent()
	{
		m_isAttacking = false;
		setState(PublicDataManager.State.Move);
	}

	public virtual void attackEnemy()
	{
		if (!isAttackable)
		{
			return;
		}
		if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			ColleagueBulletObject component = ObjectPool.Spawn("@ColleagueBullet", shootPoint.position).GetComponent<ColleagueBulletObject>();
			component.initBullet(currentColleagueType, m_currentColleagueInventoryData.currentEquippedSkinIndex, m_attackTargetPosition, delegate(EnemyObject realTargetEnemy)
			{
				if (isSplashAttack)
				{
					List<EnemyObject> nearestEnemies2 = Singleton<EnemyManager>.instance.getNearestEnemies(m_attackTargetPosition, 2.5f);
					List<Transform> list2 = new List<Transform>();
					for (int l = 0; l < nearestEnemies2.Count; l++)
					{
						list2.Add(nearestEnemies2[l].cachedTransform);
					}
					Transform nearTransform2 = Util.getNearTransform(base.cachedTransform.position, list2.ToArray());
					for (int m = 0; m < nearestEnemies2.Count; m++)
					{
						if (nearestEnemies2[m].cachedTransform == nearTransform2)
						{
							realTargetEnemy = nearestEnemies2[m];
							break;
						}
					}
					for (int n = 0; n < nearestEnemies2.Count; n++)
					{
						double num3 = curDamage;
						num3 *= (double)UnityEngine.Random.Range(0.9f, 1.1f);
						if (nearestEnemies2[n] == realTargetEnemy)
						{
							if (realTargetEnemy.isBoss || (realTargetEnemy.myMonsterObject != null && realTargetEnemy.myMonsterObject.isMiniboss))
							{
								num3 = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
							}
							if (!realTargetEnemy.isBoss && !realTargetEnemy.isMiniboss)
							{
								num3 = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
							}
							nearestEnemies2[n].decreasesHealth(num3);
							nearestEnemies2[n].setDamageText(num3, CharacterManager.CharacterType.Warrior);
						}
						else
						{
							if (realTargetEnemy.isBoss || (realTargetEnemy.myMonsterObject != null && realTargetEnemy.myMonsterObject.isMiniboss))
							{
								num3 = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
							}
							if (!realTargetEnemy.isBoss && !realTargetEnemy.isMiniboss)
							{
								num3 = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
							}
							nearestEnemies2[n].decreasesHealth(num3 * 0.5);
							nearestEnemies2[n].setDamageText(num3 * 0.5, CharacterManager.CharacterType.Warrior);
						}
					}
				}
				else if (realTargetEnemy != null)
				{
					double num4 = curDamage;
					num4 *= (double)UnityEngine.Random.Range(0.9f, 1.1f);
					if (realTargetEnemy.isBoss || (realTargetEnemy.myMonsterObject != null && realTargetEnemy.myMonsterObject.isMiniboss))
					{
						num4 = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					}
					if (!realTargetEnemy.isBoss && !realTargetEnemy.isMiniboss)
					{
						num4 = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					}
					realTargetEnemy.decreasesHealth(num4);
					realTargetEnemy.setDamageText(num4, CharacterManager.CharacterType.Warrior);
				}
				if (!string.IsNullOrEmpty(arriveBulletEffectName))
				{
					Vector3 localRotation = Vector3.zero;
					if (isSplashAttack)
					{
						localRotation = new Vector3(90f, 0f, 0f);
					}
					ObjectPool.Spawn(arriveBulletEffectName, m_attackTargetPosition, localRotation);
				}
				if (!string.IsNullOrEmpty(attackSFXSoundName))
				{
					Singleton<AudioManager>.instance.playEffectSound(attackSFXSoundName, AudioManager.EffectType.Colleague);
				}
			}, currentAttackingTargetEnemy);
			return;
		}
		EnemyObject enemyObject = null;
		if (isSplashAttack)
		{
			List<EnemyObject> nearestEnemies = Singleton<EnemyManager>.instance.getNearestEnemies(m_attackTargetPosition, 2.5f);
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < nearestEnemies.Count; i++)
			{
				list.Add(nearestEnemies[i].cachedTransform);
			}
			Transform nearTransform = Util.getNearTransform(base.cachedTransform.position, list.ToArray());
			for (int j = 0; j < nearestEnemies.Count; j++)
			{
				if (nearestEnemies[j].cachedTransform == nearTransform)
				{
					enemyObject = nearestEnemies[j];
					break;
				}
			}
			for (int k = 0; k < nearestEnemies.Count; k++)
			{
				double num = curDamage;
				num *= (double)UnityEngine.Random.Range(0.9f, 1.1f);
				if (nearestEnemies[k] == enemyObject)
				{
					if (enemyObject.isBoss || (enemyObject.myMonsterObject != null && enemyObject.myMonsterObject.isMiniboss))
					{
						num = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					}
					if (!enemyObject.isBoss && !enemyObject.isMiniboss)
					{
						num = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					}
					nearestEnemies[k].decreasesHealth(num);
				}
				else
				{
					if (enemyObject.isBoss || (enemyObject.myMonsterObject != null && enemyObject.myMonsterObject.isMiniboss))
					{
						num = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					}
					if (!enemyObject.isBoss && !enemyObject.isMiniboss)
					{
						num = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					}
					nearestEnemies[k].decreasesHealth(num * 0.5);
				}
			}
			return;
		}
		enemyObject = currentAttackingTargetEnemy;
		if (enemyObject != null)
		{
			double num2 = curDamage;
			num2 *= (double)UnityEngine.Random.Range(0.9f, 1.1f);
			if (enemyObject.isBoss || (enemyObject.myMonsterObject != null && enemyObject.myMonsterObject.isMiniboss))
			{
				num2 = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
			}
			if (!enemyObject.isBoss && !enemyObject.isMiniboss)
			{
				num2 = Singleton<ColleagueManager>.instance.getColleagueDamage(currentColleagueType, m_currentColleagueInventoryData.level, true, curDelay, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
			}
			enemyObject.decreasesHealth(num2);
		}
	}

	protected virtual IEnumerator Attack()
	{
		attackInit();
		yield return null;
	}

	protected virtual void waitInit()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
	}

	protected virtual IEnumerator Wait()
	{
		waitInit();
		yield return null;
	}
}
