using System;
using System.Collections;
using UnityEngine;

public class EnemyObject : MovingObject
{
	[HideInInspector]
	public double baseDamage;

	[HideInInspector]
	public double baseHealth;

	[HideInInspector]
	public float baseDelay;

	[HideInInspector]
	public float baseSpeed;

	[HideInInspector]
	public float baseAttackRange;

	public int currentLevel;

	public MonsterObject myMonsterObject;

	public BossObject myBossObjcect;

	public double curDamage;

	public float curDelay;

	public float curSpeed;

	public float curAttackRange;

	public bool isDead;

	public SpriteAnimation cachedSpriteAnimation;

	public AnimationNameData currentAnimationName;

	public Transform shootPoint;

	public double maxHealth;

	public double curHealth;

	public bool isMiniboss;

	public bool isBoss;

	public bool isInvincible;

	public Ground currentGround;

	protected float m_waitTimer;

	protected float m_attackTimer;

	protected bool m_isAttacking;

	protected CharacterObject m_targetCharacter;

	protected PublicDataManager.State m_currentState;

	protected SpriteGroup m_cachedSpriteGroup;

	protected bool m_hit;

	protected float m_gaugeTimer;

	protected MonsterObject monster;

	protected BossObject boss;

	protected SpecialObject special;

	private AttributeBaseStat m_enemyStat;

	protected Material m_spriteMaterials;

	protected GameObject m_currentStunEffectObject;

	protected bool isElite;

	protected FadeChangableObject m_cachedFrostEffect;

	protected bool m_isConfusion;

	private IEnumerator m_prevPlayingCoroutine;

	protected GameObject m_currentConfusionObject;

	protected double? m_percentHealthForRegen;

	protected int m_damageTextHeightCount;

	private float m_poisonTime = 0.5f;

	private int m_poisonCount = 3;

	protected virtual void Awake()
	{
		m_spriteMaterials = cachedSpriteAnimation.targetRenderer.material;
		m_cachedSpriteGroup = GetComponent<SpriteGroup>();
		setShader(CharacterManager.CharacterShaderType.DefaultShader);
	}

	public virtual void init(int level)
	{
		if (m_currentConfusionObject != null)
		{
			ObjectPool.Recycle(m_currentConfusionObject.name, m_currentConfusionObject);
			m_currentConfusionObject = null;
		}
		if (m_currentStunEffectObject != null)
		{
			ObjectPool.Recycle(m_currentStunEffectObject.name, m_currentStunEffectObject);
			m_currentStunEffectObject = null;
		}
		m_isConfusion = false;
		currentGround = Singleton<GroundManager>.instance.getStayingGround(base.cachedTransform.position);
		m_attackTimer = 0f;
		setShader(CharacterManager.CharacterShaderType.DefaultShader);
		m_cachedSpriteGroup.setAlpha(1f);
		m_hit = false;
		isDead = false;
		if (!BossRaidManager.isBossRaid)
		{
			if (this is MonsterObject)
			{
				monster = this as MonsterObject;
				if (GameManager.currentTheme > 200 && isMiniboss)
				{
					m_enemyStat = Singleton<EnemyManager>.instance.getMonsterBaseStat(monster.currentMonsterType, GameManager.currentTheme, GameManager.currentStage, true);
				}
				else
				{
					m_enemyStat = Singleton<EnemyManager>.instance.getMonsterBaseStat(monster.currentMonsterType, GameManager.currentTheme, GameManager.currentStage, isBoss);
				}
			}
			else if (this is SpecialObject)
			{
				special = this as SpecialObject;
				m_enemyStat = Singleton<EnemyManager>.instance.getSpecialMonsterBaseStat();
			}
			else if (this is BossObject)
			{
				boss = this as BossObject;
				m_enemyStat = Singleton<EnemyManager>.instance.getBossBaseStat(boss.currentBossType, GameManager.currentTheme, GameManager.currentStage);
			}
			else
			{
				DebugManager.LogError("Not exist type");
			}
		}
		else if (this is MonsterObject)
		{
			monster = this as MonsterObject;
			m_enemyStat = Singleton<EnemyManager>.instance.getMonsterBaseStatForBossRaid(monster.currentMonsterType, Singleton<BossRaidManager>.instance.currentStageForBossRaid);
			if (!isMiniboss)
			{
				m_enemyStat.baseDamage /= 5.0;
				m_enemyStat.baseHealth /= 30.0;
			}
		}
		else if (this is SpecialObject)
		{
			special = this as SpecialObject;
		}
		else if (this is BossObject)
		{
			boss = this as BossObject;
			m_enemyStat = Singleton<EnemyManager>.instance.getMonsterBaseStatForBossRaid(boss.currentBossType, Singleton<BossRaidManager>.instance.currentStageForBossRaid);
			if (!isMiniboss)
			{
				m_enemyStat.baseDamage *= ((boss.currentBossType == EnemyManager.BossType.Daemon1) ? 15 : 3);
				m_enemyStat.baseHealth *= ((boss.currentBossType == EnemyManager.BossType.Daemon1) ? 15 : 3);
			}
		}
		setProperties();
		currentLevel = level;
		setUpdatedProperties();
		setState(PublicDataManager.State.Idle);
		m_damageTextHeightCount = 0;
	}

	protected virtual void setProperties()
	{
		baseDamage = 0.0;
		baseHealth = 0.0;
		m_isAttacking = false;
		baseSpeed = m_enemyStat.baseSpeed;
		baseDamage = m_enemyStat.baseDamage;
		baseHealth = m_enemyStat.baseHealth;
		baseDelay = m_enemyStat.baseDelay;
		baseAttackRange = m_enemyStat.attackRange;
		maxHealth = baseHealth;
		curHealth = maxHealth;
	}

	protected virtual void setUpdatedProperties()
	{
	}

	public virtual void setState(PublicDataManager.State targetState)
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
		case PublicDataManager.State.Die:
			result = Die();
			break;
		case PublicDataManager.State.Wait:
			result = Wait();
			break;
		case PublicDataManager.State.Stun:
			result = Stun();
			break;
		case PublicDataManager.State.Frozen:
			result = Frozen();
			break;
		}
		return result;
	}

	protected virtual void idleInit()
	{
	}

	protected virtual IEnumerator Idle()
	{
		idleInit();
		while (true)
		{
			yield return null;
		}
	}

	protected virtual void moveInit()
	{
	}

	protected virtual IEnumerator Move()
	{
		moveInit();
		while (true)
		{
			yield return null;
		}
	}

	protected virtual void attack()
	{
	}

	protected virtual IEnumerator Attack()
	{
		attack();
		while (true)
		{
			yield return null;
		}
	}

	protected virtual void dropEvent()
	{
	}

	protected virtual void recycleEvent()
	{
	}

	protected virtual IEnumerator dieEffect()
	{
		recycleEvent();
		yield break;
	}

	protected virtual void dieEvent()
	{
		if (m_currentConfusionObject != null)
		{
			ObjectPool.Recycle(m_currentConfusionObject.name, m_currentConfusionObject);
			m_currentConfusionObject = null;
		}
		StopCoroutine("waitStunUpdate");
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.ProHunter, 1.0);
		dropEvent();
	}

	protected virtual IEnumerator Die()
	{
		dieEvent();
		while (true)
		{
			yield return null;
		}
	}

	protected virtual void waitEvent()
	{
	}

	protected virtual IEnumerator Wait()
	{
		waitEvent();
		while (true)
		{
			yield return null;
		}
	}

	public virtual void stunEnemy(float stunTime)
	{
	}

	private IEnumerator waitStunUpdate(float stunTime)
	{
		float timer = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= stunTime)
				{
					break;
				}
				if (isDead && m_currentStunEffectObject != null)
				{
					ObjectPool.Recycle(m_currentStunEffectObject.name, m_currentStunEffectObject);
					m_currentStunEffectObject = null;
				}
			}
			yield return null;
		}
		if (!isDead)
		{
			ObjectPool.Recycle(m_currentStunEffectObject.name, m_currentStunEffectObject);
			m_currentStunEffectObject = null;
			setState(PublicDataManager.State.Idle);
		}
	}

	protected virtual void stunInit()
	{
		cachedSpriteAnimation.playAnimation(currentAnimationName.idleName, 0.1f, true);
	}

	protected virtual IEnumerator Stun()
	{
		stunInit();
		while (true)
		{
			yield return null;
		}
	}

	protected virtual void frozenInit()
	{
		cachedSpriteAnimation.playFixAnimation(currentAnimationName.dieName, 0);
		if (m_cachedFrostEffect == null)
		{
			m_cachedFrostEffect = ObjectPool.Spawn("@FrostEffect", Vector2.zero, base.cachedTransform).GetComponent<FadeChangableObject>();
			m_cachedFrostEffect.setAlpha(1f);
		}
	}

	protected virtual IEnumerator Frozen()
	{
		frozenInit();
		yield return null;
	}

	public virtual void setConfusion(float duration)
	{
		if (m_currentConfusionObject != null)
		{
			ObjectPool.Recycle(m_currentConfusionObject.name, m_currentConfusionObject);
			m_currentConfusionObject = null;
		}
		m_currentConfusionObject = ObjectPool.Spawn("@ConfusionEffect", base.cachedTransform.position + new Vector3(0f, (!isBoss) ? 1.35f : 2.9f));
		FollowObject component = m_currentConfusionObject.GetComponent<FollowObject>();
		component.followTarget = base.cachedTransform;
		component.offset = new Vector3(0f, (!isBoss) ? 1.35f : 2.9f);
		m_isConfusion = true;
		StopCoroutine("confusionUpdata");
		StartCoroutine("confusionUpdata", duration);
	}

	private IEnumerator confusionUpdata(float duration)
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
		if (m_currentConfusionObject != null)
		{
			ObjectPool.Recycle(m_currentConfusionObject.name, m_currentConfusionObject);
			m_currentConfusionObject = null;
		}
		m_isConfusion = false;
	}

	public virtual void increaseHealth(double value)
	{
		curHealth = Math.Min(curHealth + value, maxHealth);
	}

	public virtual void decreasesHealth(double damage)
	{
		if (BossRaidManager.isBossRaid)
		{
			if (isBoss || isMiniboss || myBossObjcect != null)
			{
				Singleton<CachedManager>.instance.bossInformation.setProperties(this);
				return;
			}
			for (int i = 0; i < Singleton<CachedManager>.instance.enemyInformations.Length && !Singleton<CachedManager>.instance.enemyInformations[i].setProperties(this); i++)
			{
			}
		}
		else
		{
			for (int j = 0; j < Singleton<CachedManager>.instance.enemyInformations.Length && !Singleton<CachedManager>.instance.enemyInformations[j].setProperties(this); j++)
			{
			}
		}
	}

	protected virtual void Update()
	{
		if (!GameManager.isPause && !isDead && GameManager.currentTheme > 200)
		{
			double? percentHealthForRegen = m_percentHealthForRegen;
			if (percentHealthForRegen.HasValue && ((myMonsterObject != null && myMonsterObject.isEliteMonster) || isBoss))
			{
				double? percentHealthForRegen2 = m_percentHealthForRegen;
				increaseHealth(percentHealthForRegen2.Value * (double)Time.deltaTime * (double)GameManager.timeScale);
			}
		}
	}

	public void setShader(CharacterManager.CharacterShaderType shaderType)
	{
		if (!(m_spriteMaterials == null))
		{
			switch (shaderType)
			{
			case CharacterManager.CharacterShaderType.DefaultShader:
				cachedSpriteAnimation.targetRenderer.color = Color.white;
				break;
			case CharacterManager.CharacterShaderType.ReplaceShader:
				cachedSpriteAnimation.targetRenderer.color = new Color(1f, 0.44f, 0f);
				break;
			}
		}
	}

	protected IEnumerator hitEffect()
	{
		float timer = 0f;
		setShader(CharacterManager.CharacterShaderType.ReplaceShader);
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= 0.06f)
				{
					break;
				}
			}
			yield return null;
		}
		setShader(CharacterManager.CharacterShaderType.DefaultShader);
	}

	public virtual void setPoisonDamageText(double damage)
	{
		if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			ObjectPool.Spawn("@DamageYellowText", base.cachedTransform.position + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(0.8f, 1.1f), 0f)).GetComponent<CustomText>().setText(damage, 0.6f, CustomText.TextEffectType.BreakAway, 1f);
		}
	}

	public virtual void setPercentDamageText(double damage)
	{
		if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			ObjectPool.Spawn("@DamageText", base.cachedTransform.position + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(0.8f, 1.1f), 0f)).GetComponent<CustomText>().setText(damage, 0.8f, CustomText.TextEffectType.BreakAway, 1f);
		}
	}

	public virtual void setDamageText(double damage, CharacterManager.CharacterType attackerType, bool isCritical = false, bool isSkillAttack = false, bool isHitting = false, bool isTranscendSkill = false)
	{
		if (Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			return;
		}
		if (m_isConfusion)
		{
			damage += damage / 100.0 * Singleton<StatManager>.instance.confusionValueFromReinforcementCloneWarrior;
		}
		m_damageTextHeightCount++;
		Vector3 zero = Vector3.zero;
		zero = new Vector3(getDirection() * 0.4f, 1f + UnityEngine.Random.Range(0.3f, 0.5f), (float)m_damageTextHeightCount * -0.01f);
		if (m_damageTextHeightCount > 7)
		{
			m_damageTextHeightCount = 0;
		}
		if (isTranscendSkill)
		{
			ObjectPool.Spawn("@DamageYellowText", cachedSpriteAnimation.cachedTransform.position + zero).GetComponent<CustomText>().setText(damage, 1.3f, CustomText.TextEffectType.BreakAway, 1f);
		}
		else if (isSkillAttack)
		{
			if (isHitting)
			{
				if (!isCritical)
				{
					ObjectPool.Spawn("@DamageYellowText", cachedSpriteAnimation.cachedTransform.position + zero).GetComponent<CustomText>().setText(damage, 0.8f, CustomText.TextEffectType.Up, 1f);
				}
				else
				{
					ObjectPool.Spawn("@DamageYellowText", cachedSpriteAnimation.cachedTransform.position + zero).GetComponent<CustomText>().setText(damage, 1.3f, CustomText.TextEffectType.Up, 3f);
				}
			}
			else
			{
				ObjectPool.Spawn("@DamageYellowText", cachedSpriteAnimation.cachedTransform.position + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(0.8f, 1.1f), 0f)).GetComponent<CustomText>().setText(damage, 0.6f, CustomText.TextEffectType.BreakAway, 1f);
			}
		}
		else if (!isCritical)
		{
			ObjectPool.Spawn("@DamageText", cachedSpriteAnimation.cachedTransform.position + zero).GetComponent<CustomText>().setText(damage, 0.8f, CustomText.TextEffectType.Up, 1f);
		}
		else
		{
			ObjectPool.Spawn("@DamageText", cachedSpriteAnimation.cachedTransform.position + zero).GetComponent<CustomText>().setText(damage, 1.5f, CustomText.TextEffectType.Up, 3f);
		}
	}

	public void setPoisonDamage(double damage, float time, int count)
	{
		if (!isDead)
		{
			m_poisonTime = time;
			m_poisonCount = count;
			StartCoroutine("poisonUpdate", damage);
		}
	}

	public void setPoisonDamage(double damagePerSecond)
	{
		if (!isDead)
		{
			m_poisonTime = 0.5f;
			m_poisonCount = 3;
			StartCoroutine("poisonUpdate", damagePerSecond);
		}
	}

	private IEnumerator poisonUpdate(double damagePerSecond)
	{
		float timer = m_poisonTime;
		int dotCount = 0;
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (isDead)
				{
					break;
				}
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= m_poisonTime)
				{
					timer = 0f;
					decreasesHealth(damagePerSecond);
					setPoisonDamageText(damagePerSecond);
					dotCount++;
				}
				if (dotCount >= m_poisonCount)
				{
					break;
				}
			}
			yield return null;
		}
	}

	public void setFrozenEnemy(float duration)
	{
		if (m_currentState != PublicDataManager.State.Die)
		{
			setState(PublicDataManager.State.Frozen);
			StopCoroutine("waitForFrozenDurationUpdate");
			StartCoroutine("waitForFrozenDurationUpdate", duration);
		}
	}

	private IEnumerator waitForFrozenDurationUpdate(float duration)
	{
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
		if (m_cachedFrostEffect != null)
		{
			if (!base.cachedGameObject.activeSelf || !m_cachedFrostEffect.cachedGameObject.activeSelf)
			{
				ObjectPool.Recycle(m_cachedFrostEffect.name, m_cachedFrostEffect.cachedGameObject);
				m_cachedFrostEffect = null;
			}
			else
			{
				m_cachedFrostEffect.fadeIn(3f, delegate
				{
					if (m_cachedFrostEffect != null)
					{
						ObjectPool.Recycle(m_cachedFrostEffect.name, m_cachedFrostEffect.cachedGameObject);
						m_cachedFrostEffect = null;
					}
				});
			}
		}
		if (!isDead && m_currentState != PublicDataManager.State.Die)
		{
			setState(PublicDataManager.State.Idle);
		}
	}
}
