using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPUnitObject : LayeredObject
{
	public enum UnitMoveType
	{
		None = -1,
		GoAway,
		GoToEnemyWithDetectRange,
		GoToEnemyWithAttackRange,
		GoToTargetWithinDetectRange,
		Length
	}

	public bool isColleagueObject;

	public CharacterManager.CharacterType currentCharacterType;

	public CharacterSkinManager.WarriorSkinType currentWarriorSkinType = CharacterSkinManager.WarriorSkinType.Length;

	public CharacterSkinManager.PriestSkinType currentPriestSkinType = CharacterSkinManager.PriestSkinType.Length;

	public CharacterSkinManager.ArcherSkinType currentArcherSkinType = CharacterSkinManager.ArcherSkinType.Length;

	public ColleagueManager.ColleagueType currentColleagueType = ColleagueManager.ColleagueType.None;

	public int currentColleagueSkinIndex;

	public CharacterManager.CharacterBoneAnimationSpriteRendererData characterBoneSpriteRendererData;

	public SpriteRenderer weaponSpriteRenderer;

	public PVPUnitManager.PVPUnitStatData currentStatData;

	public PVPSkillManager.PVPSkillTypeData currentSkillTypeData;

	public double currentHP;

	public double maxHP;

	public PVPUnitObject targetUnitObject;

	public bool isAlly;

	public Transform centerPoint;

	public Transform shootPoint;

	public Transform bodyTransform;

	public SpriteGroup cachedSpriteGroup;

	public List<SpriteRenderer> totalSpriteRendererList = new List<SpriteRenderer>();

	public PVPUnitManager.AttackType currentAttackType;

	protected Vector2 m_originPosition;

	private PVPManager.PVPSkinData m_currentUnitData;

	private PVPUnitObject m_targetEnemyObject;

	protected float m_timerForAttack;

	protected bool m_isCanAttack;

	protected Coroutine m_attackTimerCheckUpdateCoroutine;

	private UnitMoveType m_currentUnitMoveType;

	private PVPHPGaugeObject m_currentHPGaugeObject;

	private GameObject m_currentRageEffectObject;

	private double m_rageValue;

	private GameObject m_currentHeavenArmorEffectObject;

	private double m_heavenArmorValue;

	private Coroutine m_heavenArmorUpdateCoroutine;

	private Coroutine m_rageUpdateCoroutine;

	private Coroutine m_changeProjectileAttributeUpdateCoroutine;

	protected PVPManager.PVPGameData m_currentGameData;

	protected PVPProjectileManager.ProjectileAttributeType m_currentProjectileAttributeType;

	public BoneAnimationNameData currentAnimationNameData;

	public Animation targetAnimation;

	private string m_currentAnimationName = string.Empty;

	protected bool m_isStateLock;

	public PublicDataManager.State currentState = PublicDataManager.State.None;

	private Coroutine m_prevStateCoroutine;

	public bool isDead
	{
		get
		{
			return currentHP <= 0.0 || currentState == PublicDataManager.State.Die;
		}
	}

	public void initUnit(PVPManager.PVPGameData gameData, PVPManager.PVPSkinData unitData)
	{
		setSpriteLayer("Player", 0);
		changeProjectileAttribute(PVPProjectileManager.ProjectileAttributeType.NormalProjectile);
		recycleRageEffect();
		recycleHeaveArmorEffect();
		m_currentGameData = gameData;
		m_currentUnitData = unitData;
		currentAttackType = Singleton<PVPUnitManager>.instance.getAttackType(m_currentUnitData);
		currentCharacterType = m_currentUnitData.currentCharacterType;
		currentWarriorSkinType = m_currentUnitData.currentWarriorSkinType;
		currentPriestSkinType = m_currentUnitData.currentPriestSkinType;
		currentArcherSkinType = m_currentUnitData.currentArcherSkinType;
		currentColleagueType = m_currentUnitData.currentColleagueType;
		currentColleagueSkinIndex = (int)(long)unitData.currentColleagueSkinIndex;
		isColleagueObject = currentColleagueType != ColleagueManager.ColleagueType.None;
		currentSkillTypeData = Singleton<PVPSkillManager>.instance.getSkillType(m_currentUnitData);
		if (!isColleagueObject)
		{
			removeAllAnimations();
			List<AnimationClip> list = null;
			switch (currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
			{
				list = ((currentWarriorSkinType != CharacterSkinManager.WarriorSkinType.Valkyrie1) ? Singleton<AnimationManager>.instance.warriorNormalAnimationClipList : Singleton<AnimationManager>.instance.warriorValkyrieAnimationClipList);
				for (int k = 0; k < list.Count; k++)
				{
					targetAnimation.AddClip(list[k], list[k].name);
				}
				Singleton<CharacterManager>.instance.changeCharacterSkin(currentWarriorSkinType, characterBoneSpriteRendererData);
				weaponSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite((WeaponManager.WarriorWeaponType)Math.Min(47L, unitData.skinLevel));
				break;
			}
			case CharacterManager.CharacterType.Priest:
			{
				list = ((currentPriestSkinType != CharacterSkinManager.PriestSkinType.Valkyrie2) ? Singleton<AnimationManager>.instance.priestNormalAnimationClipList : Singleton<AnimationManager>.instance.priestValkyrieAnimationClipList);
				for (int j = 0; j < list.Count; j++)
				{
					targetAnimation.AddClip(list[j], list[j].name);
				}
				Singleton<CharacterManager>.instance.changeCharacterSkin(currentPriestSkinType, characterBoneSpriteRendererData);
				weaponSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite((WeaponManager.PriestWeaponType)Math.Min(47L, unitData.skinLevel));
				break;
			}
			case CharacterManager.CharacterType.Archer:
			{
				list = ((currentArcherSkinType != CharacterSkinManager.ArcherSkinType.Valkyrie3) ? Singleton<AnimationManager>.instance.archerNormalAnimationClipList : Singleton<AnimationManager>.instance.archerValkyrieAnimationClipList);
				for (int i = 0; i < list.Count; i++)
				{
					targetAnimation.AddClip(list[i], list[i].name);
				}
				Singleton<CharacterManager>.instance.changeCharacterSkin(currentArcherSkinType, characterBoneSpriteRendererData);
				weaponSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite((WeaponManager.ArcherWeaponType)Math.Min(47L, unitData.skinLevel));
				break;
			}
			}
		}
		else
		{
			Singleton<ColleagueManager>.instance.changeColleagueSkin(currentColleagueType, currentColleagueSkinIndex, characterBoneSpriteRendererData);
		}
		initUnit(isAlly);
	}

	protected virtual void initUnit(bool isAlly)
	{
		targetAnimation[currentAnimationNameData.attackName[0]].speed = 0.5f;
		cachedSpriteGroup.setAlpha(1f);
		if (m_currentHPGaugeObject != null)
		{
			m_currentHPGaugeObject.recycleGaugeBar();
		}
		m_currentHPGaugeObject = null;
		m_timerForAttack = 0f;
		m_isCanAttack = true;
		targetUnitObject = null;
		m_originPosition = base.cachedTransform.position;
		m_targetEnemyObject = null;
		refreshStat();
		setStateLock(false);
		m_currentAnimationName = string.Empty;
		if (m_attackTimerCheckUpdateCoroutine != null)
		{
			StopCoroutine(m_attackTimerCheckUpdateCoroutine);
		}
		m_attackTimerCheckUpdateCoroutine = StartCoroutine(attackTimerCheckUpdate());
		setState(PublicDataManager.State.Move);
	}

	public void setSpriteLayer(string layerName, int sortingOrder)
	{
		for (int i = 0; i < totalSpriteRendererList.Count; i++)
		{
			if (totalSpriteRendererList[i] != null)
			{
				totalSpriteRendererList[i].sortingLayerName = layerName;
				totalSpriteRendererList[i].sortingOrder = sortingOrder;
			}
		}
	}

	[ContextMenu("Add to sprite in list")]
	public void initCurrentSpriteRendererList()
	{
		if (characterBoneSpriteRendererData.shieldSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.shieldSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.headSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.headSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.spineSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.spineSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.legSpriteRenderer != null && characterBoneSpriteRendererData.legSpriteRenderer.Length > 0)
		{
			for (int i = 0; i < characterBoneSpriteRendererData.legSpriteRenderer.Length; i++)
			{
				totalSpriteRendererList.Add(characterBoneSpriteRendererData.legSpriteRenderer[i]);
			}
		}
		if (characterBoneSpriteRendererData.leftWingSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.leftWingSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.rightWingSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.rightWingSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.capeSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.capeSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.weaponSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.weaponSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.tailSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.tailSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.leftHandSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.leftHandSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.rightHandSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.rightHandSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.leftLegSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.leftLegSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.rightLegSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.rightLegSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.leftFingerSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.leftFingerSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.rightFingerSpriteRenderer != null)
		{
			totalSpriteRendererList.Add(characterBoneSpriteRendererData.rightFingerSpriteRenderer);
		}
	}

	protected virtual void refreshStat()
	{
		currentStatData = Singleton<PVPUnitManager>.instance.getCalculatedUnitStat(m_currentGameData, m_currentUnitData, currentAttackType);
		maxHP = currentStatData.hp;
		currentHP = maxHP;
	}

	protected IEnumerator attackTimerCheckUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				m_timerForAttack += Time.deltaTime * GameManager.timeScale;
				if (m_timerForAttack > currentStatData.attackSpeed)
				{
					m_isCanAttack = true;
				}
				else
				{
					m_isCanAttack = false;
				}
			}
			yield return null;
		}
	}

	public void resetAttackTimer()
	{
		m_timerForAttack = currentStatData.attackSpeed;
	}

	public void playBoneAnimation(string animationName)
	{
		if (m_currentAnimationName != animationName)
		{
			m_currentAnimationName = animationName;
			targetAnimation.Stop();
			targetAnimation.Play(animationName);
		}
	}

	private void removeAllAnimations()
	{
		List<string> list = new List<string>();
		foreach (AnimationState item in targetAnimation)
		{
			list.Add(item.name);
		}
		for (int i = 0; i < list.Count; i++)
		{
			targetAnimation.RemoveClip(list[i]);
		}
	}

	public virtual void setState(PublicDataManager.State targetState)
	{
		if (!m_isStateLock)
		{
			stopMove();
			m_currentUnitMoveType = UnitMoveType.None;
			if (m_prevStateCoroutine != null)
			{
				StopCoroutine(m_prevStateCoroutine);
			}
			m_prevStateCoroutine = null;
			currentState = targetState;
			IEnumerator stateUpdateIEnumerator = getStateUpdateIEnumerator(currentState);
			if (stateUpdateIEnumerator != null)
			{
				m_prevStateCoroutine = StartCoroutine(stateUpdateIEnumerator);
			}
			else
			{
				DebugManager.LogError(currentState.ToString() + " IEnumrator is null!");
			}
		}
	}

	public virtual void setStateLock(bool value)
	{
		m_isStateLock = value;
	}

	private IEnumerator getStateUpdateIEnumerator(PublicDataManager.State targetState)
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
		case PublicDataManager.State.CastSkill:
			result = CastSkill();
			break;
		case PublicDataManager.State.Die:
			result = Die();
			break;
		case PublicDataManager.State.Wait:
			result = Wait();
			break;
		}
		return result;
	}

	protected virtual void idleInit()
	{
		playBoneAnimation(currentAnimationNameData.idleName[0]);
	}

	protected virtual IEnumerator Idle()
	{
		idleInit();
		while (currentState == PublicDataManager.State.Idle)
		{
			if (!GameManager.isPause)
			{
				if (targetUnitObject == null)
				{
					setState(PublicDataManager.State.Move);
					break;
				}
				if (m_isCanAttack)
				{
					float distance = Vector2.Distance(base.cachedTransform.position, targetUnitObject.cachedTransform.position);
					if (distance <= currentStatData.attackRange && !targetUnitObject.isDead)
					{
						m_timerForAttack = 0f;
						setState(PublicDataManager.State.Attack);
					}
					else
					{
						setState(PublicDataManager.State.Move);
					}
					break;
				}
			}
			yield return null;
		}
	}

	protected virtual void moveInit()
	{
		playBoneAnimation(currentAnimationNameData.moveName[0]);
		searchEnemyUnit();
		Action action = delegate
		{
			m_currentUnitMoveType = UnitMoveType.GoAway;
			Vector2 targetPosition = new Vector2(m_originPosition.x + (float)((!isAlly) ? (-1000) : 1000), m_originPosition.y);
			moveTo(targetPosition, currentStatData.moveSpeed);
		};
		if (targetUnitObject != null)
		{
			float num = Vector2.Distance(base.cachedTransform.position, targetUnitObject.cachedTransform.position);
			if (num <= currentStatData.attackRange)
			{
				setState(PublicDataManager.State.Idle);
				return;
			}
			if (num <= currentStatData.detectRange)
			{
				m_currentUnitMoveType = UnitMoveType.GoToTargetWithinDetectRange;
				moveTo(targetUnitObject.cachedTransform, currentStatData.moveSpeed, delegate
				{
					setState(PublicDataManager.State.Idle);
				});
				return;
			}
			PVPUnitObject leaderUnit = Singleton<PVPUnitManager>.instance.getLeaderUnit(isAlly);
			if (isAlly)
			{
				if (base.cachedTransform.position.x >= leaderUnit.cachedTransform.position.x)
				{
					m_currentUnitMoveType = UnitMoveType.GoToTargetWithinDetectRange;
					moveTo(targetUnitObject.cachedTransform, currentStatData.moveSpeed, delegate
					{
						setState(PublicDataManager.State.Idle);
					});
				}
				else
				{
					action();
				}
			}
			else if (base.cachedTransform.position.x <= leaderUnit.cachedTransform.position.x)
			{
				m_currentUnitMoveType = UnitMoveType.GoToTargetWithinDetectRange;
				moveTo(targetUnitObject.cachedTransform, currentStatData.moveSpeed, delegate
				{
					setState(PublicDataManager.State.Idle);
				});
			}
			else
			{
				action();
			}
		}
		else
		{
			action();
		}
	}

	protected virtual IEnumerator Move()
	{
		moveInit();
		while (currentState == PublicDataManager.State.Move)
		{
			if (!GameManager.isPause)
			{
				playBoneAnimation(currentAnimationNameData.moveName[0]);
				if (searchEnemyUnit())
				{
					float distance = Vector2.Distance(base.cachedTransform.position, targetUnitObject.cachedTransform.position);
					if (m_currentUnitMoveType == UnitMoveType.GoAway && (distance <= currentStatData.attackRange || distance <= currentStatData.detectRange))
					{
						setState(PublicDataManager.State.Move);
						break;
					}
					if (m_currentUnitMoveType == UnitMoveType.GoToTargetWithinDetectRange && distance <= currentStatData.attackRange)
					{
						setState(PublicDataManager.State.Idle);
						break;
					}
					if (m_currentUnitMoveType == UnitMoveType.GoAway)
					{
						PVPUnitObject leader = Singleton<PVPUnitManager>.instance.getLeaderUnit(isAlly);
						if (isAlly)
						{
							if (base.cachedTransform.position.x >= leader.cachedTransform.position.x)
							{
								setState(PublicDataManager.State.Move);
								break;
							}
						}
						else if (base.cachedTransform.position.x <= leader.cachedTransform.position.x)
						{
							setState(PublicDataManager.State.Move);
							break;
						}
					}
				}
				else if (Singleton<PVPUnitManager>.instance.isNoEnemy(isAlly))
				{
					setState(PublicDataManager.State.Wait);
					break;
				}
			}
			else if (this != Singleton<PVPUnitManager>.instance.allyLeaderWarrior && this != Singleton<PVPUnitManager>.instance.enemyLeaderWarrior)
			{
				playBoneAnimation(currentAnimationNameData.idleName[0]);
			}
			yield return null;
		}
	}

	protected virtual void attackInit()
	{
		setStateLock(true);
		if (targetUnitObject != null)
		{
			setDirection(NewMovingObject.calculateDirection(base.cachedTransform.position, targetUnitObject.cachedTransform.position));
		}
		if (currentAnimationNameData.attackName.Length > 0)
		{
			playBoneAnimation(currentAnimationNameData.attackName[UnityEngine.Random.Range(0, currentAnimationNameData.attackName.Length)]);
		}
		else
		{
			DebugManager.LogError("Attack animation of " + base.name + " is empty!");
		}
	}

	public virtual void attackEvent()
	{
		double realDamage = currentStatData.damage;
		double num = UnityEngine.Random.Range(0, 10000);
		bool isCritical = false;
		num /= 100.0;
		if (num < (double)currentStatData.criticalChance)
		{
			realDamage += realDamage / 100.0 * (currentStatData.criticalDamage - 100.0);
			isCritical = true;
		}
		PVPProjectileManager.ProjectileData projectileData = Singleton<PVPProjectileManager>.instance.getProjectileData(m_currentUnitData);
		PVPUnitObject attackTargetUnit = targetUnitObject;
		float volume = Singleton<AudioManager>.instance.effectVolume * UnityEngine.Random.Range(0.1f, 0.3f);
		switch (currentAttackType)
		{
		case PVPUnitManager.AttackType.MeleeSingleAttack:
			if (UnbiasedTime.Instance.UtcNow().Subtract(Singleton<PVPUnitManager>.instance.meleeAttackSoundLastPlayTime) > new TimeSpan(0, 0, 0, 0, UnityEngine.Random.Range(80, 150)))
			{
				Singleton<AudioManager>.instance.playForceEffectSound("pvp_hit_sword" + ((UnityEngine.Random.Range(0, 100) % 2 != 0) ? "2" : string.Empty), volume);
				Singleton<PVPUnitManager>.instance.meleeAttackSoundLastPlayTime = UnbiasedTime.Instance.UtcNow();
			}
			break;
		case PVPUnitManager.AttackType.MiddleRange:
			if (UnbiasedTime.Instance.UtcNow().Subtract(Singleton<PVPUnitManager>.instance.middleAttackSoundLastPlayTime) > new TimeSpan(0, 0, 0, 0, UnityEngine.Random.Range(80, 150)))
			{
				Singleton<AudioManager>.instance.playForceEffectSound("elope_hit", volume);
				Singleton<PVPUnitManager>.instance.middleAttackSoundLastPlayTime = UnbiasedTime.Instance.UtcNow();
			}
			break;
		case PVPUnitManager.AttackType.LongRange:
			if (UnbiasedTime.Instance.UtcNow().Subtract(Singleton<PVPUnitManager>.instance.rangedAttackSoundLastPlayTime) > new TimeSpan(0, 0, 0, 0, UnityEngine.Random.Range(80, 150)))
			{
				Singleton<AudioManager>.instance.playForceEffectSound("skill_arrow", volume);
				Singleton<PVPUnitManager>.instance.rangedAttackSoundLastPlayTime = UnbiasedTime.Instance.UtcNow();
			}
			break;
		case PVPUnitManager.AttackType.MeleeSplashAttack:
			if (UnbiasedTime.Instance.UtcNow().Subtract(Singleton<PVPUnitManager>.instance.splashAttackSoundLastPlayTime) > new TimeSpan(0, 0, 0, 0, UnityEngine.Random.Range(80, 150)))
			{
				Singleton<AudioManager>.instance.playForceEffectSound("pvp_tank_explosion", volume);
				Singleton<PVPUnitManager>.instance.splashAttackSoundLastPlayTime = UnbiasedTime.Instance.UtcNow();
			}
			break;
		}
		Action<PVPProjectileManager.ProjectileArriveData> arriveAction = delegate(PVPProjectileManager.ProjectileArriveData arriveData)
		{
			switch (arriveData.attribute)
			{
			case PVPProjectileManager.ProjectileAttributeType.NormalProjectile:
				if (currentAttackType == PVPUnitManager.AttackType.MeleeSplashAttack)
				{
					NewObjectPool.Spawn<GameObject>("@PVPExplosionEffect", shootPoint.position, new Vector3(90f, 0f, 0f));
				}
				if (currentAttackType == PVPUnitManager.AttackType.MeleeSplashAttack || currentAttackType == PVPUnitManager.AttackType.MiddleRange)
				{
					List<PVPUnitObject> nearestUnits2 = Singleton<PVPUnitManager>.instance.getNearestUnits(arriveData.arrivePosition, Singleton<PVPUnitManager>.instance.getSplashRange(currentAttackType), isAlly);
					for (int j = 0; j < nearestUnits2.Count; j++)
					{
						PVPUnitObject pVPUnitObject2 = nearestUnits2[j];
						if (pVPUnitObject2 != null && !pVPUnitObject2.isDead)
						{
							double num3 = realDamage;
							if (currentAttackType == PVPUnitManager.AttackType.MeleeSingleAttack)
							{
								if (pVPUnitObject2.currentAttackType == PVPUnitManager.AttackType.MiddleRange || pVPUnitObject2.currentAttackType == PVPUnitManager.AttackType.LongRange)
								{
									num3 *= 1.5;
								}
							}
							else if (currentAttackType == PVPUnitManager.AttackType.LongRange && pVPUnitObject2.currentAttackType == PVPUnitManager.AttackType.MiddleRange)
							{
								num3 *= 1.5;
							}
							if (m_rageValue > 0.0)
							{
								num3 += num3 / 100.0 * m_rageValue;
							}
							pVPUnitObject2.decreaseHP(num3, isCritical);
						}
					}
				}
				else if (attackTargetUnit != null && !attackTargetUnit.isDead)
				{
					attackTargetUnit.decreaseHP(realDamage, isCritical);
				}
				break;
			case PVPProjectileManager.ProjectileAttributeType.FireBall:
			{
				double skillValue2 = Singleton<PVPSkillManager>.instance.getSkillValue(new PVPSkillManager.PVPSkillTypeData(PVPSkillManager.MiddleSkillType.FireBall));
				List<PVPUnitObject> nearestUnits = Singleton<PVPUnitManager>.instance.getNearestUnits(arriveData.arrivePosition, 1.3f, isAlly);
				for (int i = 0; i < nearestUnits.Count; i++)
				{
					PVPUnitObject pVPUnitObject = nearestUnits[i];
					if (pVPUnitObject != null && !pVPUnitObject.isDead)
					{
						double num2 = realDamage / 100.0 * skillValue2;
						if (currentAttackType == PVPUnitManager.AttackType.MeleeSingleAttack)
						{
							if (pVPUnitObject.currentAttackType == PVPUnitManager.AttackType.MiddleRange || pVPUnitObject.currentAttackType == PVPUnitManager.AttackType.LongRange)
							{
								num2 *= 1.5;
							}
						}
						else if (currentAttackType == PVPUnitManager.AttackType.LongRange && pVPUnitObject.currentAttackType == PVPUnitManager.AttackType.MiddleRange)
						{
							num2 *= 1.5;
						}
						if (m_rageValue > 0.0)
						{
							num2 += num2 / 100.0 * m_rageValue;
						}
						pVPUnitObject.decreaseHP(num2, isCritical);
					}
				}
				Singleton<AudioManager>.instance.playEffectSound("skill_smash", AudioManager.EffectType.Skill);
				NewObjectPool.Spawn<GameObject>("@PVPExplosionEffect", arriveData.arrivePosition, new Vector3(90f, 0f, 0f));
				break;
			}
			case PVPProjectileManager.ProjectileAttributeType.Blow:
			{
				double skillValue = Singleton<PVPSkillManager>.instance.getSkillValue(new PVPSkillManager.PVPSkillTypeData(PVPSkillManager.RangedSkillType.Blow));
				if (targetUnitObject != null && !targetUnitObject.isDead)
				{
					targetUnitObject.decreaseHP(skillValue, isCritical);
				}
				break;
			}
			}
		};
		if (!projectileData.isImmediate)
		{
			Singleton<PVPProjectileManager>.instance.spawnProjectile(m_currentProjectileAttributeType, m_currentUnitData, shootPoint.position, targetUnitObject.centerPoint, delegate(PVPProjectileManager.ProjectileArriveData arriveData)
			{
				arriveAction(arriveData);
			});
			changeProjectileAttribute(PVPProjectileManager.ProjectileAttributeType.NormalProjectile);
		}
		else if (targetUnitObject != null && !targetUnitObject.isDead)
		{
			arriveAction(new PVPProjectileManager.ProjectileArriveData(targetUnitObject.cachedTransform.position, PVPProjectileManager.ProjectileAttributeType.NormalProjectile));
		}
	}

	public virtual void attackEnd()
	{
		if (!isDead && currentState != PublicDataManager.State.Die)
		{
			setStateLock(false);
			setState(PublicDataManager.State.Move);
		}
	}

	protected virtual IEnumerator Attack()
	{
		attackInit();
		yield return null;
	}

	protected virtual void castSkillInit()
	{
	}

	protected virtual IEnumerator CastSkill()
	{
		castSkillInit();
		yield return null;
	}

	protected virtual void dieInit()
	{
		setStateLock(true);
		Singleton<PVPUnitManager>.instance.removeUnit(this);
		playBoneAnimation(currentAnimationNameData.dieName[0]);
	}

	protected virtual IEnumerator Die()
	{
		dieInit();
		float alpha = 1f;
		while (alpha > 0f)
		{
			alpha -= Time.deltaTime * GameManager.timeScale * 5f;
			cachedSpriteGroup.setAlpha(alpha);
			yield return null;
		}
		NewObjectPool.Recycle(this);
	}

	protected virtual void waitInit()
	{
		playBoneAnimation(currentAnimationNameData.idleName[0]);
	}

	protected virtual IEnumerator Wait()
	{
		waitInit();
		while (currentState == PublicDataManager.State.Wait)
		{
			if (!GameManager.isPause)
			{
				if (!Singleton<PVPUnitManager>.instance.isNoEnemy(isAlly))
				{
					setState(PublicDataManager.State.Move);
					break;
				}
			}
			else
			{
				yield return new WaitForSeconds(2.9f);
				setState(PublicDataManager.State.Move);
			}
			yield return null;
		}
	}

	protected virtual bool searchEnemyUnit()
	{
		targetUnitObject = Singleton<PVPUnitManager>.instance.getNearestUnit(base.cachedTransform.position, currentStatData.detectRange, isAlly, currentAttackType);
		if (targetUnitObject == null && !Singleton<PVPUnitManager>.instance.isNoEnemy(isAlly))
		{
			targetUnitObject = Singleton<PVPUnitManager>.instance.getLeaderUnit(isAlly);
		}
		return targetUnitObject != null;
	}

	public virtual void heal(double value)
	{
		if (!isDead && PVPManager.isPlayingPVP)
		{
			double num = currentHP;
			currentHP = Math.Min(currentHP + value, maxHP);
			UIWindowPVP.instance.decreaseHP(num - currentHP, isAlly);
			string poolName = "@PVPHealEffect";
			Vector2 v = new Vector2(0f, -0.122f);
			if (currentColleagueType == ColleagueManager.ColleagueType.Golem || currentColleagueType == ColleagueManager.ColleagueType.Trall)
			{
				poolName = "@PVPHealBigEffect";
				v = new Vector2(-0.17f, -0.21f);
			}
			NewObjectPool.Spawn<GameObject>(poolName, v, base.cachedTransform);
		}
	}

	public virtual void decreaseHP(double value, bool isCritical = false)
	{
		if (isDead || !PVPManager.isPlayingPVP)
		{
			return;
		}
		if (m_heavenArmorValue > 0.0)
		{
			value -= value / 100.0 * m_heavenArmorValue;
		}
		double num = currentHP;
		currentHP = Math.Max(currentHP - value, 0.0);
		UIWindowPVP.instance.decreaseHP(num - currentHP, isAlly);
		if (isDead)
		{
			targetAnimation.Stop();
			setStateLock(false);
			setState(PublicDataManager.State.Die);
			if (m_currentHPGaugeObject != null)
			{
				m_currentHPGaugeObject.closeGauge();
			}
			m_currentHPGaugeObject = null;
		}
		else if (m_currentHPGaugeObject == null)
		{
			m_currentHPGaugeObject = Singleton<PVPHPGaugeManager>.instance.spawnHPGauge(this, isAlly);
		}
	}

	public void startRage(float duration, double value)
	{
		m_rageValue = value;
		spawnRageEffect();
		if (m_rageUpdateCoroutine != null)
		{
			StopCoroutine(m_rageUpdateCoroutine);
		}
		m_rageUpdateCoroutine = StartCoroutine(rageUpdate(duration));
	}

	protected virtual void spawnRageEffect()
	{
		if (!(m_currentRageEffectObject != null))
		{
			string text = "@PVPRangedDamageIncreaseEffect";
			Vector2 v = new Vector2(0f, 0.314f);
			if (currentColleagueType == ColleagueManager.ColleagueType.Golem || currentColleagueType == ColleagueManager.ColleagueType.Trall)
			{
				text = "@PVPMeleeDamageIncreaseBigEffect";
				v = new Vector2(-0.34f, 0.81f);
			}
			else
			{
				text = ((currentAttackType != 0 && currentAttackType != PVPUnitManager.AttackType.MeleeSplashAttack) ? "@PVPRangedDamageIncreaseEffect" : "@PVPMeleeDamageIncreaseEffect");
			}
			m_currentRageEffectObject = NewObjectPool.Spawn<GameObject>(text, v, base.cachedTransform);
		}
	}

	private void recycleRageEffect()
	{
		if (m_currentRageEffectObject != null)
		{
			NewObjectPool.Recycle(m_currentRageEffectObject);
		}
		m_currentRageEffectObject = null;
		m_rageValue = 0.0;
	}

	private IEnumerator rageUpdate(float duration)
	{
		float timer = 0f;
		while (timer < duration)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
			}
			yield return null;
		}
		recycleRageEffect();
	}

	public void changeProjectileAttribute(PVPProjectileManager.ProjectileAttributeType projectileType)
	{
		m_currentProjectileAttributeType = projectileType;
	}

	public void changeProjectileAttribute(PVPProjectileManager.ProjectileAttributeType projectileType, float duration)
	{
		if (m_changeProjectileAttributeUpdateCoroutine != null)
		{
			StopCoroutine(m_changeProjectileAttributeUpdateCoroutine);
		}
		m_changeProjectileAttributeUpdateCoroutine = StartCoroutine(changeProjectileAttributeUpdate(m_currentProjectileAttributeType, duration));
		m_currentProjectileAttributeType = projectileType;
	}

	private IEnumerator changeProjectileAttributeUpdate(PVPProjectileManager.ProjectileAttributeType originProjectileType, float duration)
	{
		float timer = 0f;
		while (timer < duration)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
			}
			yield return null;
		}
		changeProjectileAttribute(originProjectileType);
	}

	public void startHeavenArmor(float duration, double value)
	{
		m_heavenArmorValue = Math.Min(value, 90.0);
		spawnHeavenArmorEffect();
		if (m_heavenArmorUpdateCoroutine != null)
		{
			StopCoroutine(m_heavenArmorUpdateCoroutine);
		}
		m_heavenArmorUpdateCoroutine = StartCoroutine(heaveUpdate(duration));
	}

	private void spawnHeavenArmorEffect()
	{
		if (!(m_currentHeavenArmorEffectObject != null))
		{
			string poolName = "@PVPHeavenArmorEffect";
			Vector2 v = new Vector2(0f, 0.302f);
			if (currentColleagueType == ColleagueManager.ColleagueType.Golem || currentColleagueType == ColleagueManager.ColleagueType.Trall)
			{
				poolName = "@PVPHeavenArmorBigEffect";
				v = new Vector2(-0.23f, 0.83f);
			}
			m_currentHeavenArmorEffectObject = NewObjectPool.Spawn<GameObject>(poolName, v, new Vector3(-90f, 0f, 0f), Vector3.one, base.cachedTransform);
		}
	}

	private void recycleHeaveArmorEffect()
	{
		m_heavenArmorValue = 0.0;
		if (m_currentHeavenArmorEffectObject != null)
		{
			NewObjectPool.Recycle(m_currentHeavenArmorEffectObject);
		}
		m_currentHeavenArmorEffectObject = null;
		if (m_heavenArmorUpdateCoroutine != null)
		{
			StopCoroutine(m_heavenArmorUpdateCoroutine);
			m_heavenArmorUpdateCoroutine = null;
		}
	}

	private IEnumerator heaveUpdate(float duration)
	{
		float timer = 0f;
		while (timer < duration)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
			}
			yield return null;
		}
		recycleHeaveArmorEffect();
	}
}
