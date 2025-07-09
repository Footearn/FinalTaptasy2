using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class TowerModeCharacterObject : MovingObject
{
	public CharacterSkinManager.WarriorSkinType currentWarriorSkinType;

	public BoneAnimationNameData currentBoneAnimationName;

	public CharacterManager.CharacterBoneAnimationSpriteRendererData characterBoneSpriteRendererData;

	public Animation characterBoneAnimation;

	public ObscuredInt currentLifeCount = -1;

	public Transform bodyTransform;

	public TowerModeMapObject currentMapObject;

	public SpriteGroup cachedSpriteGroup;

	protected bool m_stateLock;

	protected PublicDataManager.State m_currentState;

	private string m_currentAnimationName;

	private Transform m_cachedBodyTransform;

	private IEnumerator m_prevPlayingCoroutine;

	private float m_characterSpeedByTouch;

	private float m_stunDuration;

	private bool m_isDead;

	private float m_animationLimitedMaxTimer;

	private GameObject m_castSkillEffect;

	private float m_attackRange = 2f;

	private GameObject m_stunEffectObject;

	private bool m_isCastingSkill;

	private bool m_isAttacking;

	private bool m_isInvincible;

	private float m_characterSpeed = 7f;

	public void initCharacterObject(CharacterSkinManager.WarriorSkinType targetCharacterSkinType)
	{
		cachedSpriteGroup.setAlpha(1f);
		m_isInvincible = false;
		m_currentAnimationName = string.Empty;
		m_characterSpeedByTouch = 0.8f;
		currentLifeCount = 3;
		UIWindowTowerMode.instance.setLifeCountImage(currentLifeCount);
		UIWindowTowerMode.instance.closeStunGauge();
		m_stunDuration = 0f;
		m_animationLimitedMaxTimer = 0f;
		m_isDead = false;
		m_isCastingSkill = false;
		m_isAttacking = false;
		removeAllAnimations();
		StopAllCoroutines();
		Singleton<CharacterManager>.instance.changeCharacterSkin(targetCharacterSkinType, characterBoneSpriteRendererData);
		List<AnimationClip> list = null;
		list = ((targetCharacterSkinType != CharacterSkinManager.WarriorSkinType.Valkyrie1) ? Singleton<AnimationManager>.instance.warriorNormalAnimationClipList : Singleton<AnimationManager>.instance.warriorValkyrieAnimationClipList);
		for (int i = 0; i < list.Count; i++)
		{
			characterBoneAnimation.AddClip(list[i], list[i].name);
		}
		currentMapObject = Singleton<TowerModeManager>.instance.getStayingMapObject(base.cachedTransform.position);
		if (currentMapObject == null)
		{
			DebugManager.LogError("Can not find appropriate standing ground.");
		}
		else if (currentMapObject.direction == Direction.Right)
		{
			base.cachedTransform.position = currentMapObject.getPoint(true).position + new Vector3(2f, 0f);
		}
		else
		{
			base.cachedTransform.position = currentMapObject.getPoint(true).position - new Vector3(2f, 0f);
		}
		characterBoneAnimation[currentBoneAnimationName.skillName[0]].speed = 4f;
		characterBoneAnimation[currentBoneAnimationName.attackName[0]].speed = 0.5f;
		StopCoroutine("animationSpeedUpdate");
		StartCoroutine("animationSpeedUpdate");
		setStateLock(false);
		setState(PublicDataManager.State.Idle);
		if (m_castSkillEffect != null)
		{
			ObjectPool.Recycle(m_castSkillEffect.name, m_castSkillEffect);
		}
		if (m_stunEffectObject != null)
		{
			ObjectPool.Recycle(m_stunEffectObject.name, m_stunEffectObject);
		}
	}

	public PublicDataManager.State getState()
	{
		return m_currentState;
	}

	private void removeAllAnimations()
	{
		List<string> list = new List<string>();
		foreach (AnimationState item in characterBoneAnimation)
		{
			list.Add(item.name);
		}
		for (int i = 0; i < list.Count; i++)
		{
			characterBoneAnimation.RemoveClip(list[i]);
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

	public bool getStateLock()
	{
		return m_stateLock;
	}

	public void setStateLock(bool statelock)
	{
		m_stateLock = statelock;
	}

	public virtual void setState(PublicDataManager.State targetState)
	{
		bodyTransform.localPosition = Vector3.zero;
		bodyTransform.localEulerAngles = Vector3.zero;
		if ((!m_stateLock || targetState == PublicDataManager.State.Die) && !UIWindowIngame.instance.bossWarningState)
		{
			if (m_castSkillEffect != null)
			{
				ObjectPool.Recycle(m_castSkillEffect.name, m_castSkillEffect);
			}
			if (m_stunEffectObject != null)
			{
				ObjectPool.Recycle(m_stunEffectObject.name, m_stunEffectObject);
			}
			if (m_cachedBodyTransform == null)
			{
				m_cachedBodyTransform = characterBoneAnimation.transform;
			}
			if (m_cachedBodyTransform.localEulerAngles != Vector3.zero)
			{
				m_cachedBodyTransform.localEulerAngles = Vector3.zero;
			}
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
		case PublicDataManager.State.Wait:
			result = Wait();
			break;
		case PublicDataManager.State.Die:
			result = Die();
			break;
		case PublicDataManager.State.CastSkill:
			result = CastSkill();
			break;
		case PublicDataManager.State.Stun:
			result = Stun();
			break;
		case PublicDataManager.State.Attack:
			result = Attack();
			break;
		}
		return result;
	}

	public virtual void idleInit()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
	}

	protected virtual IEnumerator Idle()
	{
		idleInit();
		while (true)
		{
			if (!GameManager.isPause && !Singleton<TowerModeManager>.instance.isFightingWithBoss)
			{
				int touchCount = 0;
				for (int i = 0; i < Input.touchCount; i++)
				{
					if (Input.GetTouch(i).phase == TouchPhase.Began)
					{
						touchCount++;
					}
				}
				if (!UIWindowTowerMode.instance.isPressedSkillButton && (Input.GetMouseButtonDown(0) || touchCount > 0 || Input.GetKey(KeyCode.Space)))
				{
					break;
				}
			}
			yield return null;
		}
		setState(PublicDataManager.State.Move);
	}

	protected virtual void moveInit()
	{
		playBoneAnimation(currentBoneAnimationName.moveName[0]);
		currentMapObject = Singleton<TowerModeManager>.instance.getStayingMapObject(base.cachedTransform.position);
		if (currentMapObject == null)
		{
			DebugManager.LogError("Can not find appropriate standing ground.");
		}
		if (currentMapObject.isBossMap && Singleton<TowerModeManager>.instance.isDeadBoss)
		{
			moveTo(currentMapObject.getPoint(false).position, 7f, delegate
			{
				gotoNextFloor();
			});
		}
		else if (Singleton<TowerModeManager>.instance.isFightingWithBoss)
		{
			moveTo(currentMapObject.getPoint(true).position + new Vector3(2 * ((currentMapObject.direction != 0) ? 1 : (-1)), 0f, 0f), 7f, delegate
			{
				setState(PublicDataManager.State.Idle);
			});
		}
		else if (!currentMapObject.isApexMap)
		{
			if (UIWindowTowerMode.instance.isPressedSkillButton || Singleton<TowerModeManager>.instance.isDeadBoss || Singleton<TowerModeManager>.instance.isFightingWithBoss)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < Input.touchCount; i++)
			{
				if (Input.GetTouch(i).phase == TouchPhase.Began)
				{
					num++;
				}
			}
			if (num <= 0)
			{
				return;
			}
			Vector2 targetPosition = base.cachedTransform.position;
			Vector2 targetPosition2 = currentMapObject.getPoint(false).position;
			if (currentMapObject.direction == Direction.Left)
			{
				targetPosition.x -= m_characterSpeedByTouch;
				if (targetPosition.x < targetPosition2.x)
				{
					moveTo(targetPosition2, m_characterSpeed, delegate
					{
						gotoNextFloor();
					});
				}
				else
				{
					moveTo(targetPosition, m_characterSpeed, delegate
					{
						setState(PublicDataManager.State.Idle);
					});
				}
				return;
			}
			targetPosition.x += m_characterSpeedByTouch;
			if (targetPosition.x > targetPosition2.x)
			{
				moveTo(targetPosition2, m_characterSpeed, delegate
				{
					gotoNextFloor();
				});
			}
			else
			{
				moveTo(targetPosition, m_characterSpeed, delegate
				{
					setState(PublicDataManager.State.Idle);
				});
			}
		}
		else
		{
			if (!currentMapObject.isApexMap)
			{
				return;
			}
			if (!currentMapObject.isGateOpend)
			{
				Vector2 targetPosition3 = currentMapObject.centerPointTransform.position;
				targetPosition3.x += ((currentMapObject.direction != 0) ? (-2) : 2);
				moveTo(targetPosition3, m_characterSpeed, delegate
				{
					setStateLock(false);
					setState(PublicDataManager.State.Wait);
					currentMapObject.openGate();
					setStateLock(true);
					jump(new Vector2(0f, 3.5f), Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position).y);
				});
			}
			else
			{
				Vector2 targetPosition4 = currentMapObject.centerPointTransform.position;
				moveTo(targetPosition4, m_characterSpeed, delegate
				{
					setStateLock(false);
					setState(PublicDataManager.State.Wait);
					setStateLock(true);
					Singleton<TowerModeManager>.instance.resultTowerMode(true);
				});
			}
		}
	}

	protected virtual IEnumerator Move()
	{
		moveInit();
		yield return null;
		while (true)
		{
			int touchCount = 0;
			for (int i = 0; i < Input.touchCount; i++)
			{
				if (Input.GetTouch(i).phase == TouchPhase.Began)
				{
					touchCount++;
				}
			}
			if (touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space))
			{
				break;
			}
			yield return null;
		}
		setState(PublicDataManager.State.Move);
	}

	protected virtual void dieEvent()
	{
		setStateLock(true);
		playBoneAnimation(currentBoneAnimationName.dieName[0]);
		Singleton<TowerModeManager>.instance.resultTowerMode(false);
	}

	protected virtual IEnumerator Die()
	{
		dieEvent();
		yield return null;
	}

	protected virtual void waitEvent()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
		stopAll();
	}

	protected virtual IEnumerator Wait()
	{
		waitEvent();
		yield return null;
	}

	protected virtual void castSkillInit()
	{
		Singleton<AudioManager>.instance.playEffectSound("skill_jump", AudioManager.EffectType.Skill);
		Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
		if (m_castSkillEffect != null)
		{
			ObjectPool.Recycle(m_castSkillEffect.name, m_castSkillEffect);
		}
		m_castSkillEffect = ObjectPool.Spawn("@CastSkill", (Vector2)base.cachedTransform.position + Vector2.up * 1.5f);
		playBoneAnimation(currentBoneAnimationName.skillName[0]);
	}

	protected virtual IEnumerator CastSkill()
	{
		castSkillInit();
		yield return null;
	}

	protected virtual void stunInit()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
		if (m_stunEffectObject != null)
		{
			ObjectPool.Recycle(m_stunEffectObject.name, m_stunEffectObject);
		}
		m_stunEffectObject = ObjectPool.Spawn("@ConfusionEffect", (Vector2)base.cachedTransform.position + Vector2.up * 1.4f);
		UIWindowTowerMode.instance.openStunGauge();
	}

	protected virtual IEnumerator Stun()
	{
		stunInit();
		while (true)
		{
			if (!GameManager.isPause)
			{
				m_stunDuration -= Time.deltaTime * GameManager.timeScale;
				for (int i = 0; i < Input.touchCount; i++)
				{
					if (Input.GetTouch(i).phase == TouchPhase.Began)
					{
						m_stunDuration -= 0.15f;
					}
				}
				if (m_stunDuration <= 0f)
				{
					break;
				}
				UIWindowTowerMode.instance.setStunGauge(m_stunDuration, Singleton<TowerModeManager>.instance.getMaxStunDuration());
			}
			yield return null;
		}
		setStateLock(false);
		UIWindowTowerMode.instance.closeStunGauge();
		setState(PublicDataManager.State.Idle);
		m_stunDuration = 0f;
	}

	private void gotoNextFloor()
	{
		if (currentMapObject != null && currentMapObject.isBestFloor)
		{
			FollowObject component = ObjectPool.Spawn("@BestRecordEffect", base.cachedTransform.position + new Vector3(0f, 1.237665f, 0f)).GetComponent<FollowObject>();
			component.followTarget = base.cachedTransform;
			currentMapObject.closeBestFloorObject();
		}
		currentMapObject = Singleton<TowerModeManager>.instance.getNextMapObject(currentMapObject);
		if (currentMapObject != null)
		{
			if (Singleton<TowerModeManager>.instance.currentDifficultyType == TowerModeManager.TowerModeDifficultyType.Endless)
			{
				Singleton<TowerModeManager>.instance.setObstacleChance(Singleton<TowerModeManager>.instance.currentDifficultyType);
			}
			if (currentMapObject.isBossMap)
			{
				for (int i = 0; i < Singleton<TowerModeManager>.instance.listTowerModeMapObjs.Count; i++)
				{
					Singleton<TowerModeManager>.instance.recycleMonstersAndObstaclesBelongToTargetMap(Singleton<TowerModeManager>.instance.listTowerModeMapObjs[i]);
				}
				Singleton<TowerModeManager>.instance.StopCoroutine("obstacleSpawnUpdate");
				Singleton<TowerModeManager>.instance.StopCoroutine("MonsterObjectSpawnUpdate");
				currentMapObject.currentBossObject.doBossRandomAction();
				UIWindowTowerMode.instance.bossHPGauge.setBossHPGauge(currentMapObject.currentBossObject, currentMapObject);
				Singleton<TowerModeManager>.instance.isFightingWithBoss = true;
			}
			else if (currentMapObject.isMiniBossMap)
			{
				Singleton<TowerModeManager>.instance.recycleMonstersAndObstaclesBelongToTargetMap(currentMapObject);
				for (int j = 0; j < Singleton<TowerModeManager>.instance.listTowerModeObstacles.Count; j++)
				{
					ObjectPool.Spawn("fx_boss_blowup", Singleton<TowerModeManager>.instance.listTowerModeObstacles[j].cachedTransform.position);
					Singleton<TowerModeManager>.instance.listTowerModeObstacles[j].recycleObstacle(false);
				}
				Singleton<TowerModeManager>.instance.listTowerModeObstacles.Clear();
				Singleton<TowerModeManager>.instance.pauseAllObstaclesAndNormalMonsters(true);
				for (int k = 0; k < currentMapObject.currentMiniBosses.Count; k++)
				{
					currentMapObject.currentMiniBosses[k].setState(PublicDataManager.State.Move);
				}
				Singleton<TowerModeManager>.instance.isFightingWithBoss = true;
			}
			else
			{
				Singleton<TowerModeManager>.instance.pauseAllObstaclesAndNormalMonsters(false);
			}
			base.cachedTransform.position = currentMapObject.getPoint(true).position;
			if (!currentMapObject.isBossMap && !currentMapObject.isMiniBossMap)
			{
				if (!currentMapObject.isApexMap)
				{
					setState(PublicDataManager.State.Idle);
				}
				else
				{
					setState(PublicDataManager.State.Move);
				}
			}
			else
			{
				setState(PublicDataManager.State.Move);
			}
			setDirection(currentMapObject.direction);
			Singleton<TowerModeManager>.instance.spawnBackground(true);
			Singleton<TowerModeManager>.instance.currentFloor = currentMapObject.curFloor;
			UIWindowTowerMode.instance.setFloorText(currentMapObject.curFloor);
		}
		else
		{
			DebugManager.LogError("Can not find next map floor.");
		}
	}

	protected virtual void attackInit()
	{
		if (!m_isAttacking)
		{
			m_isAttacking = true;
			playBoneAnimation(currentBoneAnimationName.attackName[0]);
			StopCoroutine("animationEndCheckUpdate");
			StartCoroutine("animationEndCheckUpdate");
		}
	}

	protected virtual IEnumerator Attack()
	{
		attackInit();
		while (true)
		{
			yield return null;
		}
	}

	public virtual void attackEnemy()
	{
		WarriorAttackEffectObject component = ObjectPool.Spawn("@WarriorAttackEffect1", new Vector2(0f, 0.0108f), base.cachedTransform).GetComponent<WarriorAttackEffectObject>();
		component.cachedTransform.localScale = new Vector3(-1f, 1f, 1f);
		component.initWarriorAttackEffect(4, false);
		Singleton<CachedManager>.instance.ingameCameraShake.shake(1f, 1f);
		Singleton<AudioManager>.instance.playEffectSound("tower_attack", AudioManager.EffectType.Touch);
		List<TowerModeMonsterObject> nearestEnemies = Singleton<TowerModeManager>.instance.getNearestEnemies(base.cachedTransform.position, m_attackRange);
		if (nearestEnemies != null && nearestEnemies.Count > 0)
		{
			Singleton<AudioManager>.instance.playEffectSound("skill_concentration", AudioManager.EffectType.Touch);
			for (int i = 0; i < nearestEnemies.Count; i++)
			{
				nearestEnemies[i].decreaseHP(1);
			}
		}
		List<TowerModeEnemyProjectileObject> nearestProjectiles = Singleton<TowerModeManager>.instance.getNearestProjectiles(base.cachedTransform.position, m_attackRange);
		if (nearestProjectiles != null && nearestProjectiles.Count > 0)
		{
			for (int j = 0; j < nearestProjectiles.Count; j++)
			{
				ObjectPool.Spawn("fx_boss_blowup", nearestProjectiles[j].cachedTransform.position);
				nearestProjectiles[j].recycleProjectile();
			}
		}
	}

	public void decreaseLifeCount(int value = 1)
	{
		if (m_isDead || Singleton<TowerModeManager>.instance.isDeadBoss || m_isInvincible)
		{
			return;
		}
		Action action = delegate
		{
			currentLifeCount = Mathf.Max(0, (int)currentLifeCount - value);
			UIWindowTowerMode.instance.setLifeCountImage(currentLifeCount);
			UIWindowTowerMode.instance.startHitEffect();
			Singleton<AudioManager>.instance.playEffectSound("character_hit");
			if ((float)(int)currentLifeCount <= 0f)
			{
				setState(PublicDataManager.State.Die);
				m_isDead = true;
				Singleton<TowerModeManager>.instance.isDead = true;
			}
			else
			{
				setInvincible(1f);
			}
		};
		if ((double)Singleton<StatManager>.instance.weaponSkinTowerModeEvadeChance > 0.0)
		{
			double num = UnityEngine.Random.Range(0, 10000);
			num /= 100.0;
			if (num <= (double)Singleton<StatManager>.instance.weaponSkinTowerModeEvadeChance)
			{
				Singleton<AudioManager>.instance.playEffectSound("transcend_cast");
				ObjectPool.Spawn("@TowerModeEvadeEffect", new Vector2(0f, 1.228f), base.cachedTransform);
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

	public void setStun(float duration)
	{
		m_stunDuration = duration;
		setState(PublicDataManager.State.Stun);
		setStateLock(true);
		m_isCastingSkill = false;
		m_isAttacking = false;
	}

	public void castSkill()
	{
		if (!m_isCastingSkill && !m_stateLock && !m_isDead && !Singleton<TowerModeManager>.instance.isDeadBoss)
		{
			m_isCastingSkill = true;
			m_animationLimitedMaxTimer = 0f;
			setState(PublicDataManager.State.CastSkill);
			StopCoroutine("animationEndCheckUpdate");
			StartCoroutine("animationEndCheckUpdate");
		}
	}

	public void castAttack()
	{
		if (!m_isCastingSkill && !m_stateLock && !m_isDead && !Singleton<TowerModeManager>.instance.isDeadBoss)
		{
			m_animationLimitedMaxTimer = 0f;
			setState(PublicDataManager.State.Attack);
		}
	}

	public void divineSmash()
	{
		if (m_castSkillEffect != null)
		{
			ObjectPool.Recycle(m_castSkillEffect.name, m_castSkillEffect);
			m_castSkillEffect = null;
		}
		ObjectPool.Spawn("@ExplosionEffect", Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position), new Vector3(90f, 0f, 0f));
		Singleton<AudioManager>.instance.playEffectSound("skill_smash", AudioManager.EffectType.Skill);
		GameObject gameObject = ObjectPool.Spawn("@WarriorAttackEffect3", (Vector2)base.cachedTransform.position + new Vector2(getDirection() * 0.81f, 0.77f));
		gameObject.transform.localScale = new Vector2(getDirection(), 1f);
		Singleton<CachedManager>.instance.ingameCameraShake.shake(2f, 0.1f);
		List<TowerModeMonsterObject> nearestEnemies = Singleton<TowerModeManager>.instance.getNearestEnemies(base.cachedTransform.position, m_attackRange);
		if (nearestEnemies != null && nearestEnemies.Count > 0)
		{
			for (int i = 0; i < nearestEnemies.Count; i++)
			{
				nearestEnemies[i].decreaseHP(1);
			}
		}
	}

	public void attackEnd()
	{
		if (Singleton<TowerModeManager>.instance.isFightingWithBoss)
		{
			setState(PublicDataManager.State.Move);
		}
		else
		{
			setState(PublicDataManager.State.Idle);
		}
		m_isCastingSkill = false;
		m_isAttacking = false;
	}

	public void setInvincible(float duration)
	{
		if (!m_isInvincible)
		{
			m_isInvincible = true;
			StopCoroutine("invincibleUpdate");
			StartCoroutine("invincibleUpdate", duration);
		}
	}

	private IEnumerator invincibleUpdate(float duration)
	{
		float timer = 0f;
		float alpha = 1f;
		bool switchForAlpha = true;
		while (timer < duration)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (switchForAlpha)
				{
					alpha = Mathf.Max(alpha - Time.deltaTime * GameManager.timeScale * 20f, 0f);
					if (alpha <= 0f)
					{
						switchForAlpha = !switchForAlpha;
					}
				}
				else
				{
					alpha = Mathf.Min(alpha + Time.deltaTime * GameManager.timeScale * 20f, 1f);
					if (alpha >= 1f)
					{
						switchForAlpha = !switchForAlpha;
					}
				}
				cachedSpriteGroup.setAlpha(alpha);
			}
			yield return null;
		}
		cachedSpriteGroup.setAlpha(1f);
		m_isInvincible = false;
	}

	protected IEnumerator animationEndCheckUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (m_currentState == PublicDataManager.State.CastSkill || m_currentState == PublicDataManager.State.Attack)
				{
					m_animationLimitedMaxTimer += Time.deltaTime * GameManager.timeScale;
				}
				if ((m_currentState == PublicDataManager.State.CastSkill || m_currentState == PublicDataManager.State.Attack) && (!characterBoneAnimation.isPlaying || m_animationLimitedMaxTimer >= 0.5f))
				{
					break;
				}
			}
			yield return null;
		}
		attackEnd();
	}

	private IEnumerator animationSpeedUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				for (int j = 0; j < currentBoneAnimationName.moveName.Length; j++)
				{
					characterBoneAnimation[currentBoneAnimationName.moveName[j]].speed = m_characterSpeed / 3f * GameManager.timeScale;
				}
				characterBoneAnimation[currentBoneAnimationName.idleName[0]].speed = GameManager.timeScale;
			}
			else
			{
				for (int i = 0; i < currentBoneAnimationName.moveName.Length; i++)
				{
					characterBoneAnimation[currentBoneAnimationName.moveName[i]].speed = 0f;
				}
				characterBoneAnimation[currentBoneAnimationName.idleName[0]].speed = GameManager.timeScale;
			}
			yield return null;
		}
	}
}
