using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObject : MovingObject
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
	public float baseCriticalChance;

	[HideInInspector]
	public double baseCriticalDamage;

	[HideInInspector]
	public float baseAttackRange;

	public float feverPercentage;

	public double curDamage;

	public float curDelay;

	public float curSpeed;

	public float curCriticalChance;

	public double curCriticalDamage;

	public float curAttackRange;

	public HPProgress hpGauge;

	public bool isDead;

	public bool isCanCastSkill;

	public Transform shootPoint;

	public CharacterObject currentFollower;

	public CharacterObject myLeaderCharacter;

	public Animation characterBoneAnimation;

	public CharacterManager.CharacterType currentCharacterType;

	public BoneAnimationNameData currentBoneAnimationName;

	public CharacterManager.CharacterBoneAnimationSpriteRendererData characterBoneSpriteRendererData;

	public Ground currentGround;

	public EnemyObject targetEnemy;

	public Weapon equippedWeapon;

	public bool isCanPrintHitEffect;

	public double maxHealth;

	public double curHealth;

	public bool isAttacking;

	public int currentStayingGroundIndex;

	[HideInInspector]
	public SpriteRenderer[] characterSpriteRenderers;

	public Transform weaponParentTransform;

	public AttributeBaseStat attributeBaseStat;

	protected Vector2 m_attackTargetPosition;

	protected Vector2 m_targetHolePosition;

	protected PublicDataManager.State m_currentState;

	protected bool m_stateLock;

	protected bool m_isFirstGround;

	protected bool m_isStair;

	protected int m_stairCount;

	protected bool m_isFever;

	protected bool m_isEquippedWeapon;

	protected bool m_isBossGround;

	protected float m_attackTimer;

	protected bool m_isCanAttack;

	public bool isFirstJump;

	protected double m_extraDamageValue;

	protected string m_currentAnimationName;

	private Transform m_cachedBodyTransform;

	private IEnumerator m_prevPlayingCoroutine;

	protected EnemyObject m_lastAttackTargetEnemy;

	protected int m_attackStackCount;

	protected float m_attackLimitedMaxTimer;

	protected virtual void Start()
	{
		initCurrentSpriteRendererList();
		setShader(CharacterManager.CharacterShaderType.DefaultShader);
		resetProperties();
		setState(PublicDataManager.State.Wait);
	}

	private void initCurrentSpriteRendererList()
	{
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		if (characterBoneSpriteRendererData.shieldSpriteRenderer != null)
		{
			list.Add(characterBoneSpriteRendererData.shieldSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.headSpriteRenderer != null)
		{
			list.Add(characterBoneSpriteRendererData.headSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.spineSpriteRenderer != null)
		{
			list.Add(characterBoneSpriteRendererData.spineSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.legSpriteRenderer != null && characterBoneSpriteRendererData.legSpriteRenderer.Length > 0)
		{
			for (int i = 0; i < characterBoneSpriteRendererData.legSpriteRenderer.Length; i++)
			{
				list.Add(characterBoneSpriteRendererData.legSpriteRenderer[i]);
			}
		}
		if (characterBoneSpriteRendererData.leftWingSpriteRenderer != null)
		{
			list.Add(characterBoneSpriteRendererData.leftWingSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.rightWingSpriteRenderer != null)
		{
			list.Add(characterBoneSpriteRendererData.rightWingSpriteRenderer);
		}
		if (characterBoneSpriteRendererData.capeSpriteRenderer != null)
		{
			list.Add(characterBoneSpriteRendererData.capeSpriteRenderer);
		}
		characterSpriteRenderers = list.ToArray();
	}

	public virtual void startGame()
	{
		stopAll();
		currentStayingGroundIndex = 0;
		Singleton<CharacterManager>.instance.characterList.Add(this);
		m_isFever = false;
		isAttacking = false;
		m_isStair = false;
		m_stairCount = 0;
		isDead = false;
		attributeBaseStat = Singleton<CharacterManager>.instance.getCharacterBaseStat(currentCharacterType);
		resetProperties();
		isFirstJump = true;
		healthLvlUp();
		maxHealth = baseHealth;
		curHealth = maxHealth;
		setDirection(Direction.Right);
		setState(PublicDataManager.State.Move);
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon)
		{
			isCanCastSkill = false;
			setStateLock(true);
		}
		else
		{
			isCanCastSkill = true;
			setStateLock(false);
		}
		if (currentCharacterType == CharacterManager.CharacterType.Warrior)
		{
			hpGauge.setProgress(curHealth, maxHealth);
		}
		if (currentFollower != null)
		{
			currentFollower.startGame();
		}
		m_attackTimer = 0f;
		m_isCanAttack = false;
		StopCoroutine("attackTimerUpdate");
		StartCoroutine("attackTimerUpdate");
		StopCoroutine("animationSpeedUpdate");
		StartCoroutine("animationSpeedUpdate");
		targetEnemy = null;
	}

	private IEnumerator animationSpeedUpdate()
	{
		while (true)
		{
			if (GameManager.currentGameState == GameManager.GameState.Playing)
			{
				if (!GameManager.isPause)
				{
					for (int l = 0; l < currentBoneAnimationName.attackName.Length; l++)
					{
						characterBoneAnimation[currentBoneAnimationName.attackName[l]].speed = 1f / Mathf.Max(curDelay, 0.2f) / 4f * GameManager.timeScale;
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
					characterBoneAnimation[currentBoneAnimationName.idleName[0]].speed = GameManager.timeScale;
				}
			}
			else
			{
				characterBoneAnimation[currentBoneAnimationName.idleName[0]].speed = 1f;
			}
			yield return null;
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

	public bool isEquipedWeapon()
	{
		return m_isEquippedWeapon;
	}

	public bool isCanAttack()
	{
		return m_isCanAttack;
	}

	public void setShader(CharacterManager.CharacterShaderType shaderType)
	{
		if (characterSpriteRenderers == null || characterSpriteRenderers.Length <= 0)
		{
			return;
		}
		switch (shaderType)
		{
		case CharacterManager.CharacterShaderType.DefaultShader:
		{
			for (int j = 0; j < characterSpriteRenderers.Length; j++)
			{
				characterSpriteRenderers[j].material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
			}
			break;
		}
		case CharacterManager.CharacterShaderType.ReplaceShader:
		{
			for (int i = 0; i < characterSpriteRenderers.Length; i++)
			{
				characterSpriteRenderers[i].material = Singleton<ResourcesManager>.instance.redColorSpriteDefaultMaterial;
			}
			break;
		}
		case CharacterManager.CharacterShaderType.RebirthShader:
			StartCoroutine("heroWhite");
			break;
		}
	}

	private IEnumerator heroWhite()
	{
		float color = 0f;
		while (true)
		{
			color += Time.deltaTime * 1.5f;
			for (int j = 0; j < characterSpriteRenderers.Length; j++)
			{
				characterSpriteRenderers[j].color = new Color(1f, 1f, 1f, color);
			}
			if (equippedWeapon.cachedSpriteRenderer != null)
			{
				equippedWeapon.cachedSpriteRenderer.color = new Color(1f, 1f, 1f, color);
			}
			if (color > 1f)
			{
				break;
			}
			yield return null;
		}
		for (int i = 0; i < characterSpriteRenderers.Length; i++)
		{
			characterSpriteRenderers[i].color = new Color(1f, 1f, 1f, 1f);
		}
		if (equippedWeapon.cachedSpriteRenderer != null)
		{
			equippedWeapon.cachedSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
		}
	}

	protected virtual IEnumerator attackTimerUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause && !isDead && m_currentState != PublicDataManager.State.Wait)
			{
				m_attackTimer += Time.deltaTime * GameManager.timeScale;
				targetEnemy = myLeaderCharacter.targetEnemy;
				if (m_attackTimer >= curDelay)
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

	public bool isFirstGround()
	{
		return m_isFirstGround;
	}

	public bool isStair()
	{
		return m_isStair;
	}

	public virtual void resetProperties(bool changeAnimation = true)
	{
		m_attackStackCount = 0;
		m_lastAttackTargetEnemy = null;
		attributeBaseStat = Singleton<CharacterManager>.instance.getCharacterBaseStat(currentCharacterType);
		m_currentAnimationName = string.Empty;
		StopCoroutine("attackEndCheckUpdate");
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			m_isFirstGround = false;
		}
		else
		{
			m_isFirstGround = true;
		}
		isCanPrintHitEffect = true;
		fixedDirection = true;
		setShader(CharacterManager.CharacterShaderType.DefaultShader);
		feverPercentage = 0f;
		baseDamage = 0.0;
		baseHealth = 0.0;
		baseCriticalDamage = 0.0;
		baseDelay = 0f;
		baseCriticalDamage = 0.0;
		baseAttackRange = 0f;
		isAttacking = false;
		m_extraDamageValue = 0.0;
		baseDelay = CalculateManager.getCurrentDelay(attributeBaseStat.baseDelay);
		baseSpeed = CalculateManager.getCurrentMoveSpeed(attributeBaseStat.baseSpeed);
		baseCriticalChance = CalculateManager.getCurrentCriticalProbability(attributeBaseStat.criticalChance);
		baseCriticalDamage = CalculateManager.getCurrentCriticalDamage(attributeBaseStat.criticalDamage);
		baseSpeed = CalculateManager.getCurrentMoveSpeed(baseSpeed);
		baseHealth = 0.0;
		baseAttackRange = attributeBaseStat.attackRange;
		m_isEquippedWeapon = true;
		if (equippedWeapon != null)
		{
			equippedWeapon.cachedTransform.localPosition = Vector3.zero;
			equippedWeapon.cachedTransform.localEulerAngles = Vector3.zero;
			equippedWeapon.refreshWeaponStats();
			equippedWeapon.refreshWeaponSkin();
			WeaponStat weaponRealStats = equippedWeapon.weaponRealStats;
			baseDamage += weaponRealStats.weaponDamage;
			if (weaponRealStats.secondStatType == StatManager.WeaponStatType.FixedHealth)
			{
				baseHealth += weaponRealStats.secondStatValue;
			}
			if (weaponRealStats.secondStatType == StatManager.WeaponStatType.CriticalChance)
			{
				baseCriticalChance += (float)weaponRealStats.secondStatValue;
			}
		}
		checkExtraCharacterStat();
		baseCriticalChance = Mathf.Min(100f, baseCriticalChance);
		curDelay = baseDelay;
		curSpeed = baseSpeed;
		m_speed = curSpeed;
		curDamage = getCurrentDamage();
		switch (currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			curCriticalChance = baseCriticalChance + Singleton<StatManager>.instance.allCriticalChanceFromTreasure + Singleton<StatManager>.instance.warriorCriticalChanceFromColleague;
			curCriticalDamage = baseCriticalDamage + Singleton<StatManager>.instance.allCriticalDamage + Singleton<StatManager>.instance.warriorCriticalDamageFromColleague;
			break;
		case CharacterManager.CharacterType.Priest:
			curCriticalChance = baseCriticalChance + Singleton<StatManager>.instance.allCriticalChanceFromTreasure + Singleton<StatManager>.instance.priestCriticalChanceFromColleague;
			curCriticalDamage = baseCriticalDamage + Singleton<StatManager>.instance.allCriticalDamage + Singleton<StatManager>.instance.priestCriticalDamageFromColleague;
			break;
		case CharacterManager.CharacterType.Archer:
			curCriticalChance = baseCriticalChance + Singleton<StatManager>.instance.allCriticalChanceFromTreasure + Singleton<StatManager>.instance.archerCriticalChanceFromColleague;
			curCriticalDamage = baseCriticalDamage + Singleton<StatManager>.instance.allCriticalDamage + Singleton<StatManager>.instance.archerCriticalDamageFromColleague;
			break;
		}
		curAttackRange = baseAttackRange;
		StopCoroutine("statUpdate");
		if (base.cachedGameObject.activeSelf && base.cachedGameObject.activeInHierarchy)
		{
			StartCoroutine("statUpdate");
		}
		m_isFever = false;
		if (GameManager.currentGameState == GameManager.GameState.OutGame)
		{
			UIWindowManageHeroAndWeapon.instance.refreshTotalDamage();
		}
	}

	protected virtual void checkExtraCharacterStat()
	{
	}

	public double getCurrentDamage(params double[] args)
	{
		double num = 0.0;
		double num2 = 0.0;
		switch (currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			num2 = Singleton<StatManager>.instance.allPercentDamage + Singleton<StatManager>.instance.allPercentDamageFromTreasure + Singleton<StatManager>.instance.warriorPercentDamageFromColleague + Singleton<StatManager>.instance.allPercentDamageFromColleague + Singleton<StatManager>.instance.percentDamageAM + Singleton<StatManager>.instance.percentDamagePM + Singleton<StatManager>.instance.extraPercentDamageFromConquerorRing + m_extraDamageValue;
			for (int j = 0; j < args.Length; j++)
			{
				num2 += args[j];
			}
			num = baseDamage + baseDamage / 100.0 * num2;
			num += num / 100.0 * (Singleton<StatManager>.instance.allPercentDamageFromTreasureChest + Singleton<StatManager>.instance.transcendIncreaseAllDamage);
			num += num / 100.0 * Singleton<StatManager>.instance.allPercentDamageByConcentraion;
			num += num / 100.0 * Singleton<StatManager>.instance.allCharacterAndColleaguePercentDamageFromPrincess;
			num += num / 100.0 * Singleton<StatManager>.instance.specialAdsAngelDamage;
			num += num / 100.0 * Singleton<StatManager>.instance.percentDamageFromPremiumTreasure;
			num += num / 100.0 * Singleton<StatManager>.instance.allCharacterPercentDamageFromTowerModeTreasure;
			num += num / 100.0 * Singleton<StatManager>.instance.warriorPercentDamageFromPremiumTreasure;
			num += num / 100.0 * Singleton<StatManager>.instance.weaponSkinPercentDamageForWarrior;
			num += num / 100.0 * Singleton<StatManager>.instance.weaponSkinDamageFromReinforcementMana;
			num += num / 100.0 * (Singleton<StatManager>.instance.weaponSkinStackAttackDamageForWarrior * (double)m_attackStackCount);
			if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(ColleagueManager.ColleagueType.Angela).isUnlocked)
			{
				num += num / 100.0 * Singleton<ColleagueManager>.instance.getPremiumColleagueValue(ColleagueManager.ColleagueType.Angela);
			}
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			num2 = Singleton<StatManager>.instance.allPercentDamage + Singleton<StatManager>.instance.allPercentDamageFromTreasure + Singleton<StatManager>.instance.priestPercentDamageFromColleague + Singleton<StatManager>.instance.allPercentDamageFromColleague + Singleton<StatManager>.instance.percentDamageAM + Singleton<StatManager>.instance.percentDamagePM + Singleton<StatManager>.instance.extraPercentDamageFromConquerorRing + m_extraDamageValue;
			for (int k = 0; k < args.Length; k++)
			{
				num2 += args[k];
			}
			num = baseDamage + baseDamage / 100.0 * num2;
			num += num / 100.0 * (Singleton<StatManager>.instance.allPercentDamageFromTreasureChest + Singleton<StatManager>.instance.transcendIncreaseAllDamage);
			num += num / 100.0 * Singleton<StatManager>.instance.allPercentDamageByConcentraion;
			num += num / 100.0 * Singleton<StatManager>.instance.allCharacterAndColleaguePercentDamageFromPrincess;
			num += num / 100.0 * Singleton<StatManager>.instance.specialAdsAngelDamage;
			num += num / 100.0 * Singleton<StatManager>.instance.percentDamageFromPremiumTreasure;
			num += num / 100.0 * Singleton<StatManager>.instance.allCharacterPercentDamageFromTowerModeTreasure;
			num += num / 100.0 * Singleton<StatManager>.instance.priestPercentDamageFromPremiumTreasure;
			num += num / 100.0 * Singleton<StatManager>.instance.weaponSkinPercentDamageForPriest;
			num += num / 100.0 * Singleton<StatManager>.instance.weaponSkinDamageFromReinforcementMana;
			num += num / 100.0 * (Singleton<StatManager>.instance.weaponSkinStackAttackDamageForPriest * (double)m_attackStackCount);
			if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(ColleagueManager.ColleagueType.Angela).isUnlocked)
			{
				num += num / 100.0 * Singleton<ColleagueManager>.instance.getPremiumColleagueValue(ColleagueManager.ColleagueType.Angela);
			}
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			num2 = Singleton<StatManager>.instance.allPercentDamage + Singleton<StatManager>.instance.allPercentDamageFromTreasure + Singleton<StatManager>.instance.archerPercentDamageFromColleague + Singleton<StatManager>.instance.allPercentDamageFromColleague + Singleton<StatManager>.instance.percentDamageAM + Singleton<StatManager>.instance.percentDamagePM + Singleton<StatManager>.instance.extraPercentDamageFromConquerorRing + m_extraDamageValue;
			for (int i = 0; i < args.Length; i++)
			{
				num2 += args[i];
			}
			num = baseDamage + baseDamage / 100.0 * num2;
			num += num / 100.0 * (Singleton<StatManager>.instance.allPercentDamageFromTreasureChest + Singleton<StatManager>.instance.transcendIncreaseAllDamage);
			num += num / 100.0 * Singleton<StatManager>.instance.allPercentDamageByConcentraion;
			num += num / 100.0 * Singleton<StatManager>.instance.allCharacterAndColleaguePercentDamageFromPrincess;
			num += num / 100.0 * Singleton<StatManager>.instance.specialAdsAngelDamage;
			num += num / 100.0 * Singleton<StatManager>.instance.percentDamageFromPremiumTreasure;
			num += num / 100.0 * Singleton<StatManager>.instance.allCharacterPercentDamageFromTowerModeTreasure;
			num += num / 100.0 * Singleton<StatManager>.instance.archerPercentDamageFromPremiumTreasure;
			num += num / 100.0 * Singleton<StatManager>.instance.weaponSkinPercentDamageForArcher;
			num += num / 100.0 * Singleton<StatManager>.instance.weaponSkinDamageFromReinforcementMana;
			num += num / 100.0 * (Singleton<StatManager>.instance.weaponSkinStackAttackDamageForArcher * (double)m_attackStackCount);
			if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(ColleagueManager.ColleagueType.Angela).isUnlocked)
			{
				num += num / 100.0 * Singleton<ColleagueManager>.instance.getPremiumColleagueValue(ColleagueManager.ColleagueType.Angela);
			}
			break;
		}
		}
		return Math.Max(num, 0.0);
	}

	protected virtual IEnumerator statUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				switch (currentCharacterType)
				{
				case CharacterManager.CharacterType.Warrior:
					curCriticalChance = baseCriticalChance + Singleton<StatManager>.instance.allCriticalChanceFromTreasure + Singleton<StatManager>.instance.warriorCriticalChanceFromColleague + (float)Singleton<StatManager>.instance.weaponSkinCriticalChanceForWarrior;
					curCriticalDamage = baseCriticalDamage + Singleton<StatManager>.instance.allCriticalDamage + Singleton<StatManager>.instance.warriorCriticalDamageFromColleague;
					curCriticalDamage += Singleton<StatManager>.instance.weaponSkinCriticalDamageForWarrior;
					curDamage = getCurrentDamage();
					break;
				case CharacterManager.CharacterType.Priest:
					curDamage = getCurrentDamage();
					curCriticalChance = baseCriticalChance + Singleton<StatManager>.instance.allCriticalChanceFromTreasure + Singleton<StatManager>.instance.priestCriticalChanceFromColleague + (float)Singleton<StatManager>.instance.weaponSkinCriticalChanceForPriest;
					curCriticalDamage = baseCriticalDamage + Singleton<StatManager>.instance.allCriticalDamage + Singleton<StatManager>.instance.priestCriticalDamageFromColleague;
					curCriticalDamage += Singleton<StatManager>.instance.weaponSkinCriticalDamageForPriest;
					break;
				case CharacterManager.CharacterType.Archer:
					curDamage = getCurrentDamage();
					curCriticalChance = baseCriticalChance + Singleton<StatManager>.instance.allCriticalChanceFromTreasure + Singleton<StatManager>.instance.archerCriticalChanceFromColleague + (float)Singleton<StatManager>.instance.weaponSkinCriticalChanceForArcher;
					curCriticalDamage = baseCriticalDamage + Singleton<StatManager>.instance.allCriticalDamage + Singleton<StatManager>.instance.archerCriticalDamageFromColleague;
					curCriticalDamage += Singleton<StatManager>.instance.weaponSkinCriticalDamageForArcher;
					break;
				}
				feverPercentage = Mathf.Max(0f, feverPercentage - Time.deltaTime * 50f);
			}
			yield return null;
		}
	}

	public virtual void healthLvlUp()
	{
		curHealth = baseHealth;
	}

	public virtual void checkLeaderMovedDistance()
	{
		StopCoroutine("checkUpdate");
		StartCoroutine("checkUpdate");
	}

	private IEnumerator checkUpdate()
	{
		float tempXPos = myLeaderCharacter.cachedTransform.position.x;
		float stackTotalXDistance = 0f;
		while (true)
		{
			float curXPos = myLeaderCharacter.cachedTransform.position.x;
			if (tempXPos != curXPos)
			{
				stackTotalXDistance += Mathf.Abs(curXPos - tempXPos);
				tempXPos = curXPos;
			}
			if (stackTotalXDistance >= CharacterManager.intervalBetweenCharacter)
			{
				break;
			}
			yield return null;
		}
		goToNextGround();
	}

	public virtual void fever(float percentage)
	{
		if (!m_stateLock)
		{
			feverPercentage = Mathf.Min(80f, feverPercentage + percentage);
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

	public virtual void setState(PublicDataManager.State targetState)
	{
		if ((!m_stateLock || targetState == PublicDataManager.State.Die) && !UIWindowIngame.instance.bossWarningState)
		{
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
		case PublicDataManager.State.Attack:
			result = Attack();
			break;
		case PublicDataManager.State.Wait:
			result = Wait();
			break;
		case PublicDataManager.State.Die:
			result = Die();
			break;
		case PublicDataManager.State.Stun:
			result = Stun();
			break;
		case PublicDataManager.State.CastSkill:
			result = CastSkill();
			break;
		}
		return result;
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
				if (m_currentState == PublicDataManager.State.Attack && (!characterBoneAnimation.isPlaying || m_attackLimitedMaxTimer >= 0.5f))
				{
					break;
				}
			}
			yield return null;
		}
		attackEndEvent();
	}

	public PublicDataManager.State getState()
	{
		return m_currentState;
	}

	public virtual void goToNextGround()
	{
		currentGround = Singleton<GroundManager>.instance.getNextGround(currentGround);
		currentStayingGroundIndex = currentGround.currnetFloor;
		if (currentGround != null)
		{
			setDirection(MovingObject.calculateDirection(currentGround.inPoint.position, currentGround.outPoint.position));
			if (currentFollower != null)
			{
				currentFollower.setState(PublicDataManager.State.Move);
			}
			setState(PublicDataManager.State.Move);
		}
		else
		{
			setState(PublicDataManager.State.Idle);
		}
	}

	public virtual void idleInit()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
	}

	protected virtual IEnumerator Idle()
	{
		idleInit();
		do
		{
			yield return null;
			if (currentGround == myLeaderCharacter.currentGround && !isStair() && Mathf.Abs(myLeaderCharacter.cachedTransform.position.x - base.cachedTransform.position.x) > CharacterManager.intervalBetweenCharacter + 0.25f)
			{
				setState(PublicDataManager.State.Move);
				yield break;
			}
		}
		while (!(targetEnemy == null));
		setState(PublicDataManager.State.Move);
	}

	protected virtual void stunInit()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
	}

	protected virtual IEnumerator Stun()
	{
		stunInit();
		while (true)
		{
			yield return null;
		}
	}

	protected virtual void moveInit()
	{
		playBoneAnimation(currentBoneAnimationName.moveName[0]);
		isAttacking = false;
		Vector2 startPosition = base.cachedTransform.position;
		if (!m_isStair)
		{
			currentGround = Singleton<GroundManager>.instance.getStayingGround(startPosition);
			currentStayingGroundIndex = currentGround.currnetFloor;
		}
		setDontStop(false);
		if (m_isStair)
		{
			float num = Vector2.Distance(base.cachedTransform.position, myLeaderCharacter.cachedTransform.position);
			if (!(num >= CharacterManager.intervalBetweenCharacter))
			{
				return;
			}
			fixedDirection = false;
			if (m_stairCount < currentGround.stairpoint.Length)
			{
				m_targetHolePosition = currentGround.stairpoint[m_stairCount].position;
			}
			else
			{
				m_targetHolePosition = currentGround.inPoint.position;
			}
			moveTo(m_targetHolePosition, curSpeed, delegate
			{
				if (m_stairCount < currentGround.stairpoint.Length)
				{
					m_stairCount++;
				}
				else
				{
					m_isStair = false;
					fixedDirection = true;
				}
				setState(PublicDataManager.State.Move);
			});
		}
		else if (!m_isFirstGround)
		{
			if (!BossRaidManager.isBossRaid)
			{
				if (currentGround != myLeaderCharacter.currentGround)
				{
					Ground nextGround = Singleton<GroundManager>.instance.getNextGround(currentGround);
					int num2 = ((MovingObject.calculateDirection(currentGround.inPoint.position, currentGround.outPoint.position) != 0) ? 1 : (-1));
					if (nextGround.isBossGround && nextGround.stairpoint.Length == 1)
					{
						m_targetHolePosition = (Vector2)currentGround.outPoint.position + new Vector2(-num2, 0f);
					}
					else
					{
						m_targetHolePosition = currentGround.outPoint.position;
					}
					if (Vector2.Distance(m_targetHolePosition, base.cachedTransform.position) + Vector2.Distance(myLeaderCharacter.cachedTransform.position, m_targetHolePosition) >= CharacterManager.intervalBetweenCharacter)
					{
						moveTo(m_targetHolePosition, curSpeed, delegate
						{
							m_isStair = true;
							m_stairCount = 0;
							goToNextGround();
						});
					}
				}
				else
				{
					setDontStop(true);
					float num3 = myLeaderCharacter.cachedTransform.position.x - base.cachedTransform.position.x;
					moveTo(myLeaderCharacter.cachedTransform, (0f - CharacterManager.intervalBetweenCharacter) * myLeaderCharacter.getDirection(), curSpeed, delegate
					{
						setDirection(myLeaderCharacter.getDirectionEnum());
					});
				}
			}
			else
			{
				setDontStop(true);
				moveTo(myLeaderCharacter.cachedTransform, (0f - CharacterManager.intervalBetweenCharacter) * myLeaderCharacter.getDirection(), curSpeed, delegate
				{
					setDirection(myLeaderCharacter.getDirectionEnum());
				});
			}
		}
		else if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon)
		{
			setStateLock(true);
			if (!isFirstJump)
			{
				return;
			}
			isFirstJump = false;
			if (currentGround.downPoint != null)
			{
				m_targetHolePosition = currentGround.downPoint.position;
			}
			moveTo(m_targetHolePosition, curSpeed, delegate
			{
				isFirstJump = false;
				m_targetHolePosition = Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position, 1f);
				jump(new Vector2(2.5f, 4f), m_targetHolePosition.y, delegate
				{
					setStateLock(false);
					isFirstJump = false;
					setDirection(Direction.Left);
					isCanCastSkill = true;
					setDontStop(true);
					currentGround = Singleton<GroundManager>.instance.getStayingGround(base.cachedTransform.position);
					moveTo(myLeaderCharacter.cachedTransform, (0f - CharacterManager.intervalBetweenCharacter) * myLeaderCharacter.getDirection(), curSpeed);
					m_isFirstGround = false;
					setState(PublicDataManager.State.Idle);
					if (currentCharacterType == CharacterManager.CharacterType.Archer && TutorialManager.isTutorial)
					{
						Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType3);
					}
				});
			});
		}
		else if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			setDontStop(true);
			moveTo(myLeaderCharacter.cachedTransform, (0f - CharacterManager.intervalBetweenCharacter) * myLeaderCharacter.getDirection(), curSpeed, delegate
			{
				setDirection(myLeaderCharacter.getDirectionEnum());
			});
		}
	}

	protected virtual IEnumerator Move()
	{
		moveInit();
		while (true)
		{
			if (myLeaderCharacter != null)
			{
				if (myLeaderCharacter.currentGround == currentGround)
				{
					fixedDirection = true;
					curSpeed = baseSpeed + baseSpeed / 100f * feverPercentage;
					curSpeed *= 1f + (Mathf.Abs(myLeaderCharacter.cachedTransform.position.x - base.cachedTransform.position.x) - CharacterManager.intervalBetweenCharacter);
					curSpeed = Mathf.Clamp(curSpeed, baseSpeed + baseSpeed / 100f * feverPercentage, (baseSpeed + baseSpeed / 100f * feverPercentage) * 2f);
					curSpeed += curSpeed / 100f * (Singleton<StatManager>.instance.percentMovementSpeed * 0.01f * 60f);
					setSpeed(curSpeed);
					if (!m_isFirstGround)
					{
						float dist = myLeaderCharacter.cachedTransform.position.x - base.cachedTransform.position.x;
						if (Mathf.Abs(dist) > CharacterManager.intervalBetweenCharacter && !isMoving())
						{
							moveTo(myLeaderCharacter.cachedTransform, (0f - CharacterManager.intervalBetweenCharacter) * myLeaderCharacter.getDirection(), curSpeed, delegate
							{
								setDirection(myLeaderCharacter.getDirectionEnum());
								setState(PublicDataManager.State.Idle);
							});
						}
					}
				}
				else
				{
					fixedDirection = false;
					if (currentGround != null && currentGround.outPoint != null && myLeaderCharacter != null && myLeaderCharacter.currentGround != null && myLeaderCharacter.currentGround.inPoint != null)
					{
						float realDistance = CharacterManager.intervalBetweenCharacter + Mathf.Abs(base.cachedTransform.position.x - currentGround.outPoint.position.x) + Mathf.Abs(myLeaderCharacter.currentGround.inPoint.position.x - myLeaderCharacter.cachedTransform.position.x);
						curSpeed = baseSpeed + baseSpeed / 100f * feverPercentage;
						curSpeed *= 1f + (realDistance - CharacterManager.intervalBetweenCharacter);
						curSpeed = Mathf.Clamp(curSpeed, baseSpeed + baseSpeed / 100f * feverPercentage, (baseSpeed + baseSpeed / 100f * feverPercentage) * 2f);
						curSpeed += curSpeed / 100f * (Singleton<StatManager>.instance.percentMovementSpeed * 0.01f * 60f);
						setSpeed(curSpeed);
					}
				}
				if (!isFirstGround())
				{
					float distanceBetweenLeaderCharacter = Vector2.Distance(base.cachedTransform.position, myLeaderCharacter.cachedTransform.position);
					if (myLeaderCharacter.currentGround != currentGround)
					{
						if (Vector2.Distance(m_targetHolePosition, base.cachedTransform.position) + Vector2.Distance(myLeaderCharacter.cachedTransform.position, m_targetHolePosition) < CharacterManager.intervalBetweenCharacter)
						{
							setState(PublicDataManager.State.Idle);
							yield break;
						}
					}
					else
					{
						if (isStair() && distanceBetweenLeaderCharacter < CharacterManager.intervalBetweenCharacter)
						{
							setState(PublicDataManager.State.Idle);
							yield break;
						}
						if (!isMoving() && distanceBetweenLeaderCharacter > CharacterManager.intervalBetweenCharacter + 1f)
						{
							yield return null;
							setState(PublicDataManager.State.Move);
							yield break;
						}
					}
				}
			}
			if (!BossRaidManager.isBossRaid)
			{
				if (targetEnemy != null && currentGround == Singleton<CharacterManager>.instance.warriorCharacter.currentGround && !isMoving() && !m_isFirstGround)
				{
					setState(PublicDataManager.State.Idle);
					yield break;
				}
				if (!m_isFirstGround && !isMoving())
				{
					yield return null;
					setState(PublicDataManager.State.Move);
					yield break;
				}
			}
			else if (targetEnemy != null && !isMoving())
			{
				break;
			}
			yield return null;
		}
		setState(PublicDataManager.State.Idle);
	}

	protected virtual void attack()
	{
	}

	public virtual void attackEnemy()
	{
	}

	public virtual void attackEndEvent()
	{
	}

	protected virtual IEnumerator Attack()
	{
		attack();
		yield return null;
	}

	protected virtual void dieEvent()
	{
		setStateLock(true);
		if (currentFollower != null && !currentFollower.isFirstGround())
		{
			currentFollower.setState(PublicDataManager.State.Die);
		}
		targetEnemy = null;
		isDead = true;
		Singleton<SkillManager>.instance.isClonedWarrior = false;
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
		dieAfterJump();
	}

	protected virtual void dieAfterJump()
	{
	}

	protected virtual IEnumerator Die()
	{
		dieEvent();
		yield return null;
	}

	protected virtual void waitEvent()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
		if (currentFollower != null)
		{
			currentFollower.setState(PublicDataManager.State.Wait);
		}
		stopAll();
	}

	protected virtual IEnumerator Wait()
	{
		waitEvent();
		yield return null;
	}

	protected IEnumerator hitEffect()
	{
		setShader(CharacterManager.CharacterShaderType.ReplaceShader);
		float timer = 0f;
		int hitCount = 0;
		bool switchShader = false;
		ObjectPool.Spawn("fx_hit_hero", (Vector2)base.cachedTransform.position + new Vector2(0f, 0.7f));
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (isCanPrintHitEffect)
				{
					timer += Time.deltaTime * GameManager.timeScale;
					if (timer >= 0.05f)
					{
						hitCount++;
						timer = 0f;
						if (!switchShader)
						{
							setShader(CharacterManager.CharacterShaderType.DefaultShader);
						}
						else
						{
							setShader(CharacterManager.CharacterShaderType.ReplaceShader);
						}
						switchShader = !switchShader;
						if (hitCount >= 3)
						{
							break;
						}
					}
				}
				else
				{
					setShader(CharacterManager.CharacterShaderType.DefaultShader);
				}
			}
			yield return null;
		}
	}

	public virtual void castSkill()
	{
	}

	protected virtual void playSkillAnimation()
	{
	}

	protected virtual IEnumerator CastSkill()
	{
		m_stateLock = true;
		playSkillAnimation();
		yield return null;
	}

	public virtual void decreasesHealth(double damage)
	{
		if (!BossRaidManager.isBossRaid && Singleton<EnemyManager>.instance.bossObject != null && Singleton<EnemyManager>.instance.bossObject.isDead)
		{
			return;
		}
		bool flag = ((UnityEngine.Random.Range(0f, 100f) < (float)Math.Min(Singleton<StatManager>.instance.evasionPercentExtraChance + Singleton<StatManager>.instance.weaponSkinEvadeChance, 90.0)) ? true : false);
		if (Singleton<StatManager>.instance.decreaseDamageFromReinforcementConcentration > 0f)
		{
			damage = Math.Max(damage - damage / 100.0 * (double)Singleton<StatManager>.instance.decreaseDamageFromReinforcementConcentration, 0.0);
		}
		if (Singleton<StatManager>.instance.transcendDecreaseHitDamage > 0.0)
		{
			damage = Math.Max(damage - damage / 100.0 * Singleton<StatManager>.instance.transcendDecreaseHitDamage, 0.0);
		}
		if (Singleton<StatManager>.instance.weaponSkinArmor > 0.0)
		{
			damage = Math.Max(damage - damage / 100.0 * Singleton<StatManager>.instance.weaponSkinArmor, 0.0);
		}
		if (Singleton<StatManager>.instance.weaponSkinArmorFromReinforcementMana > 0.0)
		{
			damage = Math.Max(damage - damage / 100.0 * Math.Min(Singleton<StatManager>.instance.weaponSkinArmorFromReinforcementMana, 95.0), 0.0);
		}
		if (!flag)
		{
			if (isDead)
			{
				return;
			}
			curHealth = Math.Max(curHealth - damage, 0.0);
			if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
			{
				ObjectPool.Spawn("@DamageBlueText", (Vector2)Singleton<CachedManager>.instance.mainHPProgressBar.cachedTransform.position + new Vector2(0.3f, 0.3f)).GetComponent<CustomText>().setText(damage, 0.6f, CustomText.TextEffectType.BreakAway, 1f);
			}
			StopCoroutine(hitEffect());
			StartCoroutine(hitEffect());
			if (curHealth <= 0.0)
			{
				if (Singleton<StatManager>.instance.weaponSkinInvinciblePerson > 0.0)
				{
					double num = UnityEngine.Random.Range(0, 10000);
					num /= 100.0;
					if (num < Singleton<StatManager>.instance.weaponSkinInvinciblePerson)
					{
						ObjectPool.Spawn("@WeaponSkinInvinciblePersonEffect", new Vector3(0f, 0.17f, 0f), base.cachedTransform);
						increasesHealth(maxHealth);
					}
					else
					{
						setState(PublicDataManager.State.Die);
					}
				}
				else
				{
					setState(PublicDataManager.State.Die);
				}
			}
			if (currentCharacterType == CharacterManager.CharacterType.Warrior)
			{
				hpGauge.setProgress(curHealth, maxHealth);
			}
			Singleton<AudioManager>.instance.playEffectSound("character_hit");
		}
		else
		{
			Transform transform = ObjectPool.Spawn("@EvasionText", base.cachedTransform.position + new Vector3(0.3f * getDirection(), 1f), Singleton<CachedManager>.instance.uiIngameFieldWrapperTransform, true).transform;
			transform.localScale = Vector3.one;
		}
	}

	public void increasesHealth(double value, bool isHeal = true)
	{
		curHealth = Math.Min(curHealth + value, maxHealth);
		if (currentCharacterType == CharacterManager.CharacterType.Warrior)
		{
			hpGauge.setProgress(curHealth, maxHealth);
		}
	}

	private IEnumerator waitForJump(Vector2 velocity, Action endAction)
	{
		yield return null;
		yield return null;
		currentFollower.jumpWithFollowingCharacter(velocity, endAction);
	}

	public void jumpWithFollowingCharacter(Vector2 velocity, Action endAction)
	{
		setStateLock(true);
		stopAll();
		if (currentFollower != null)
		{
			jump(velocity, Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position).y, delegate
			{
				setStateLock(false);
			});
			StartCoroutine(waitForJump(velocity, endAction));
		}
		else
		{
			endAction();
			jump(velocity, Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position).y, endAction);
		}
	}

	public virtual void setDamageText(double damage, bool isBoss)
	{
	}

	protected void removeAllAnimations()
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
}
