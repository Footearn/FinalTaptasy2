using System;
using System.Collections;
using UnityEngine;

public class MonsterObject : EnemyObject
{
	public EnemyManager.MonsterType currentMonsterType;

	public bool isEliteMonster;

	public bool isRangedMonster;

	private ElasticObject elasticObject;

	private Vector2 m_basePosition;

	private GameObject m_cachedEliteEffect;

	protected override void Awake()
	{
		cachedSpriteAnimation.animationType = currentMonsterType.ToString();
		base.Awake();
		myMonsterObject = this;
	}

	public void setMonsterDetailAttribute(EnemyManager.MonsterType targetType)
	{
		EnemyManager.MonsterDetailAttributeData monsterDetailAttributeData = Singleton<EnemyManager>.instance.currentMonsterDetailAttributeDictionary[currentMonsterType];
		isRangedMonster = monsterDetailAttributeData.isRangedMonster;
		cachedSpriteAnimation.cachedTransform.localPosition = monsterDetailAttributeData.localPosition;
		cachedSpriteAnimation.cachedTransform.localScale = monsterDetailAttributeData.localScale;
		currentAnimationName = monsterDetailAttributeData.animationNameData;
		cachedSpriteAnimation.animationType = targetType.ToString();
		cachedSpriteAnimation.init();
	}

	public override void init(int level)
	{
		setMonsterDetailAttribute(currentMonsterType);
		m_cachedEliteEffect = null;
		if (!isBoss && !isMiniboss)
		{
			if (isEliteMonster)
			{
				base.cachedTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
				cachedSpriteAnimation.targetRenderer.sortingOrder = -1;
				m_cachedEliteEffect = ObjectPool.Spawn("@EliteMonsterEffect", new Vector3(0.015f, 0.304f, 0f), base.cachedTransform);
			}
			else
			{
				base.cachedTransform.localScale = Vector3.one;
				cachedSpriteAnimation.targetRenderer.sortingOrder = -2;
			}
		}
		else
		{
			cachedSpriteAnimation.targetRenderer.sortingOrder = 0;
		}
		base.init(level);
		Singleton<EnemyManager>.instance.enemyList.Add(this);
		m_basePosition = base.cachedTransform.position;
	}

	public override void setState(PublicDataManager.State targetState)
	{
		base.setState(targetState);
	}

	protected override void setUpdatedProperties()
	{
		if (isEliteMonster)
		{
			baseDamage *= 2.0;
			baseHealth *= 4.0;
		}
		curDamage = baseDamage;
		curHealth = baseHealth;
		curDelay = CalculateManager.getCurrentDelay(baseDelay);
		curSpeed = CalculateManager.getCurrentMoveSpeed(baseSpeed);
		m_speed = curSpeed;
		curAttackRange = baseAttackRange;
		maxHealth = curHealth;
		m_percentHealthForRegen = maxHealth / 100.0 * 15.0;
	}

	protected override void idleInit()
	{
		cachedSpriteAnimation.playAnimation(currentAnimationName.idleName, 0.1f, true);
		if (m_cachedFrostEffect != null)
		{
			ObjectPool.Recycle(m_cachedFrostEffect.name, m_cachedFrostEffect.cachedGameObject);
			m_cachedFrostEffect = null;
		}
	}

	protected override IEnumerator Idle()
	{
		idleInit();
		Vector2 position2 = Vector2.zero;
		float keepTime = UnityEngine.Random.Range(2, 4);
		float timer = 0f;
		CharacterObject targetCharacter;
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (!BossRaidManager.isBossRaid)
				{
					timer += Time.deltaTime * GameManager.timeScale;
					if (timer >= keepTime && !isBoss)
					{
						setState(PublicDataManager.State.Move);
						yield break;
					}
				}
				position2 = base.cachedTransform.position;
				targetCharacter = Singleton<CharacterManager>.instance.getNearestCharacter(position2, curAttackRange);
				if (targetCharacter != null)
				{
					break;
				}
			}
			yield return null;
		}
		m_targetCharacter = targetCharacter;
		setState(PublicDataManager.State.Attack);
	}

	protected override void moveInit()
	{
		if (BossRaidManager.isBossRaid)
		{
			setState(PublicDataManager.State.Idle);
			return;
		}
		cachedSpriteAnimation.playAnimation(currentAnimationName.walkName, 0.1f, true);
		Vector2 targetPosition = new Vector2(m_basePosition.x + UnityEngine.Random.Range(-0.1f, 0.1f), base.cachedTransform.position.y);
		Direction direction = ((UnityEngine.Random.Range(0, 2) != 0) ? Direction.Right : Direction.Left);
		float num = 0f;
		do
		{
			switch (direction)
			{
			case Direction.Left:
				num = UnityEngine.Random.Range(-0.4f, -0.15f);
				break;
			case Direction.Right:
				num = UnityEngine.Random.Range(0.15f, 0.4f);
				break;
			}
		}
		while (Mathf.Abs(num - base.cachedTransform.position.x) <= 0.1f);
		targetPosition.x = m_basePosition.x + num;
		if (GetInstanceID() == instanceID)
		{
			MonoBehaviour.print("Start Move");
		}
		moveTo(targetPosition, curSpeed, delegate
		{
			setState(PublicDataManager.State.Idle);
		});
	}

	protected override IEnumerator Move()
	{
		moveInit();
		Vector2 position2 = Vector2.zero;
		while (true)
		{
			position2 = base.cachedTransform.position;
			CharacterObject targetCharacter = Singleton<CharacterManager>.instance.getNearestCharacter(position2, curAttackRange);
			if (targetCharacter != null)
			{
				m_targetCharacter = targetCharacter;
				setState(PublicDataManager.State.Attack);
			}
			yield return null;
		}
	}

	protected override void attack()
	{
		if (isRangedMonster)
		{
			if (TutorialManager.isTutorial && Singleton<TutorialManager>.instance.isInvincible)
			{
				return;
			}
			m_isAttacking = true;
			setDirection(MovingObject.calculateDirection(base.cachedTransform.position, m_targetCharacter.cachedTransform.position));
			cachedSpriteAnimation.playAnimation(currentAnimationName.attackName, curDelay * 0.1f, true, SpriteAnimation.FrameList.CenterFrame, delegate
			{
				if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
				{
					Bullet bullet = ObjectPool.Spawn("@BoneBullet", shootPoint.position).GetComponent<Bullet>();
					if (isMiniboss)
					{
						bullet.cachedTransform.localScale = Vector2.one * 2f;
					}
					else
					{
						bullet.cachedTransform.localScale = Vector2.one;
					}
					SpriteAnimation component = bullet.cachedImageTransform.GetComponent<SpriteAnimation>();
					component.animationType = currentMonsterType.ToString();
					Vector2 vector = (Vector2)m_targetCharacter.cachedTransform.position + new Vector2(0f, 0.5f);
					if (currentMonsterType == EnemyManager.MonsterType.Flamespirit1 || currentMonsterType == EnemyManager.MonsterType.Flamespirit2 || currentMonsterType == EnemyManager.MonsterType.Flamespirit3 || currentMonsterType == EnemyManager.MonsterType.FlamespiritBoss)
					{
						if (isMiniboss)
						{
							bullet.cachedImageTransform.localRotation = Quaternion.Euler(Vector3.zero);
							vector = (Vector2)m_targetCharacter.cachedTransform.position + new Vector2(0f, 1.2f);
						}
						bullet.cachedTransform.position = vector;
						bullet.setParabolic(false);
						bullet.setRotatable(false);
						bullet.setBomb(false);
						bullet.setLookAtTarget(false);
						bullet.bulletSpeed = 10f;
						component.init();
						component.playAnimation("Effect", 0.05f, true, delegate
						{
							if (!Singleton<CharacterManager>.instance.warriorCharacter.isDead)
							{
								double num5 = curDamage * (double)UnityEngine.Random.Range(0.9f, 1.1f);
								if (isMiniboss || isBoss)
								{
									num5 -= num5 / 100.0 * Singleton<StatManager>.instance.decreaseDamageFromHitBoss;
								}
								m_targetCharacter.decreasesHealth(num5);
								m_targetCharacter.setDamageText(num5, false);
							}
							bullet.recycleBullet();
						});
					}
					else
					{
						switch (currentMonsterType)
						{
						case EnemyManager.MonsterType.Goblin1:
						case EnemyManager.MonsterType.Goblin2:
						case EnemyManager.MonsterType.Goblin3:
						case EnemyManager.MonsterType.GoblinBoss:
							bullet.setParabolic(false);
							bullet.setRotatable(true);
							bullet.setBomb(false);
							bullet.setLookAtTarget(false);
							bullet.bulletSpeed = 10f;
							break;
						case EnemyManager.MonsterType.Yeti1:
						case EnemyManager.MonsterType.Yeti2:
						case EnemyManager.MonsterType.Yeti3:
						case EnemyManager.MonsterType.YetiBoss:
							bullet.setParabolic(true);
							bullet.setRotatable(false);
							bullet.setBomb(true);
							bullet.setLookAtTarget(false);
							bullet.bulletSpeed = 5f;
							break;
						case EnemyManager.MonsterType.Bat1:
						case EnemyManager.MonsterType.Bat2:
						case EnemyManager.MonsterType.Bat3:
						case EnemyManager.MonsterType.BatBoss:
							bullet.setParabolic(false);
							bullet.setRotatable(false);
							bullet.setBomb(false);
							bullet.setLookAtTarget(false);
							bullet.bulletSpeed = 10f;
							break;
						default:
							bullet.setParabolic(false);
							bullet.setRotatable(false);
							bullet.setBomb(false);
							bullet.setLookAtTarget(true);
							bullet.bulletSpeed = 10f;
							break;
						}
						component.init();
						component.playAnimation("Bullet", 0.1f, true);
						bullet.shootBullet(vector, delegate(CharacterObject realTargetCharacter)
						{
							if (realTargetCharacter != null && !realTargetCharacter.isDead)
							{
								double num4 = curDamage * (double)UnityEngine.Random.Range(0.9f, 1.1f);
								if (isMiniboss || isBoss)
								{
									num4 -= num4 / 100.0 * Singleton<StatManager>.instance.decreaseDamageFromHitBoss;
								}
								realTargetCharacter.decreasesHealth(num4);
								realTargetCharacter.setDamageText(num4, false);
							}
						}, Vector2.zero, m_targetCharacter);
					}
				}
				else if (!Singleton<CharacterManager>.instance.warriorCharacter.isDead)
				{
					double num3 = curDamage * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					if (isMiniboss || isBoss)
					{
						num3 -= num3 / 100.0 * Singleton<StatManager>.instance.decreaseDamageFromHitBoss;
					}
					m_targetCharacter.decreasesHealth(num3);
					m_targetCharacter.setDamageText(num3, false);
				}
			}, delegate
			{
				m_isAttacking = false;
				setState(PublicDataManager.State.Wait);
			});
		}
		else
		{
			if (TutorialManager.isTutorial && Singleton<TutorialManager>.instance.isInvincible)
			{
				return;
			}
			if (elasticObject != null)
			{
				ObjectPool.Recycle(elasticObject.name, elasticObject.cachedGameObject);
			}
			elasticObject = null;
			m_isAttacking = true;
			setState(PublicDataManager.State.Wait);
			setDirection(MovingObject.calculateDirection(base.cachedTransform.position, m_targetCharacter.cachedTransform.position));
			switch (currentMonsterType)
			{
			case EnemyManager.MonsterType.Frog1:
			case EnemyManager.MonsterType.Frog2:
			case EnemyManager.MonsterType.Frog3:
			case EnemyManager.MonsterType.FrogBoss:
			case EnemyManager.MonsterType.Mummy1:
			case EnemyManager.MonsterType.Mummy2:
			case EnemyManager.MonsterType.Mummy3:
			case EnemyManager.MonsterType.MummyBoss:
				cachedSpriteAnimation.playAnimation(currentAnimationName.attackName, curDelay * 0.1f, false, 2, delegate
				{
					switch (currentMonsterType)
					{
					case EnemyManager.MonsterType.Frog1:
					case EnemyManager.MonsterType.Frog2:
					case EnemyManager.MonsterType.Frog3:
					case EnemyManager.MonsterType.FrogBoss:
						elasticObject = ObjectPool.Spawn("@ElasticBullet", new Vector2(0.3f, 0.7f), base.cachedTransform).GetComponent<ElasticObject>();
						elasticObject.scaler.sprite = Singleton<EnemyManager>.instance.frogAttackSprites[1];
						elasticObject.hitObject.sprite = Singleton<EnemyManager>.instance.frogAttackSprites[0];
						elasticObject.SetTarget(this, m_targetCharacter);
						break;
					case EnemyManager.MonsterType.Mummy1:
					{
						Sprite[] mummy2AttackSprites = Singleton<EnemyManager>.instance.mummy1AttackSprites;
						elasticObject = ObjectPool.Spawn("@ElasticBullet", new Vector2(1f, 0.6f), base.cachedTransform).GetComponent<ElasticObject>();
						elasticObject.scaler.sprite = mummy2AttackSprites[1];
						elasticObject.hitObject.sprite = mummy2AttackSprites[0];
						elasticObject.SetTarget(this, m_targetCharacter);
						break;
					}
					case EnemyManager.MonsterType.Mummy2:
					{
						Sprite[] mummy2AttackSprites = Singleton<EnemyManager>.instance.mummy2AttackSprites;
						elasticObject = ObjectPool.Spawn("@ElasticBullet", new Vector2(1f, 0.6f), base.cachedTransform).GetComponent<ElasticObject>();
						elasticObject.scaler.sprite = mummy2AttackSprites[1];
						elasticObject.hitObject.sprite = mummy2AttackSprites[0];
						elasticObject.SetTarget(this, m_targetCharacter);
						break;
					}
					case EnemyManager.MonsterType.Mummy3:
					{
						Sprite[] mummy2AttackSprites = Singleton<EnemyManager>.instance.mummy3AttackSprites;
						elasticObject = ObjectPool.Spawn("@ElasticBullet", new Vector2(1f, 0.6f), base.cachedTransform).GetComponent<ElasticObject>();
						elasticObject.scaler.sprite = mummy2AttackSprites[1];
						elasticObject.hitObject.sprite = mummy2AttackSprites[0];
						elasticObject.SetTarget(this, m_targetCharacter);
						break;
					}
					case EnemyManager.MonsterType.MummyBoss:
					{
						Sprite[] mummy2AttackSprites = Singleton<EnemyManager>.instance.mummyBossAttackSprites;
						elasticObject = ObjectPool.Spawn("@ElasticBullet", new Vector2(1f, 0.6f), base.cachedTransform).GetComponent<ElasticObject>();
						elasticObject.scaler.sprite = mummy2AttackSprites[1];
						elasticObject.hitObject.sprite = mummy2AttackSprites[0];
						elasticObject.SetTarget(this, m_targetCharacter);
						break;
					}
					default:
						if (!m_targetCharacter.isDead)
						{
							double num2 = curDamage * (double)UnityEngine.Random.Range(0.9f, 1.1f);
							if (isMiniboss)
							{
								Singleton<AudioManager>.instance.playEffectSound("monster_boss_attack");
							}
							if (isMiniboss || isBoss)
							{
								num2 -= num2 / 100.0 * Singleton<StatManager>.instance.decreaseDamageFromHitBoss;
							}
							m_targetCharacter.decreasesHealth(num2);
							m_targetCharacter.setDamageText(num2, false);
						}
						break;
					}
				}, 3, delegate
				{
					switch (currentMonsterType)
					{
					case EnemyManager.MonsterType.Frog1:
					case EnemyManager.MonsterType.Frog2:
					case EnemyManager.MonsterType.Frog3:
					case EnemyManager.MonsterType.FrogBoss:
						elasticObject.Resetform();
						break;
					case EnemyManager.MonsterType.Mummy1:
					case EnemyManager.MonsterType.Mummy2:
					case EnemyManager.MonsterType.Mummy3:
					case EnemyManager.MonsterType.MummyBoss:
						if (elasticObject != null)
						{
							ObjectPool.Recycle("@ElasticBullet", elasticObject.gameObject);
							elasticObject = null;
						}
						break;
					}
				}, delegate
				{
					if (elasticObject != null)
					{
						ObjectPool.Recycle("@ElasticBullet", elasticObject.gameObject);
						elasticObject = null;
					}
					m_targetCharacter = null;
					m_isAttacking = false;
				});
				return;
			}
			cachedSpriteAnimation.playAnimation(currentAnimationName.attackName, curDelay * 0.1f, false, SpriteAnimation.FrameList.CenterFrame, delegate
			{
				if (!m_targetCharacter.isDead)
				{
					double num = curDamage * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					if (isMiniboss)
					{
						Singleton<AudioManager>.instance.playEffectSound("monster_boss_attack");
					}
					if (isMiniboss || isBoss)
					{
						num -= num / 100.0 * Singleton<StatManager>.instance.decreaseDamageFromHitBoss;
					}
					m_targetCharacter.decreasesHealth(num);
					m_targetCharacter.setDamageText(num, false);
				}
			}, delegate
			{
				m_targetCharacter = null;
				m_isAttacking = false;
			});
		}
	}

	protected override IEnumerator Attack()
	{
		attack();
		while (true)
		{
			yield return null;
		}
	}

	protected override void dropEvent()
	{
		if (!BossRaidManager.isBossRaid)
		{
			float num = UnityEngine.Random.Range(0f, 100f);
			float num2 = 0f;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			if (isMiniboss && isBoss)
			{
				num2 = 100f;
				num3 = 0.0;
				num4 = 0.0;
				num5 = 100.0;
				num6 = 0.0;
			}
			else
			{
				num2 = (isEliteMonster ? 100f : 6.6f);
				num3 = 90.5;
				num4 = 9.5;
				num5 = 0.0;
				num6 = 0.0;
				if (Singleton<DataManager>.instance.currentGameData.currentTheme >= 50 && Singleton<DataManager>.instance.currentGameData.currentTheme <= 100)
				{
					num6 = 0.1;
				}
				else if (Singleton<DataManager>.instance.currentGameData.currentTheme > 100)
				{
					num6 = 1.0 + 0.01 * Math.Truncate((double)(Singleton<DataManager>.instance.currentGameData.currentTheme - 100) / 10.0) * 2.0;
				}
				num6 *= (double)((!isEliteMonster) ? 1 : 2);
				num6 += num6 / 100.0 * Singleton<StatManager>.instance.weaponSkinLegendaryChestDropChance;
				num3 -= num6;
				if (CollectEventManager.isOnCollectEvent)
				{
					double num7 = UnityEngine.Random.Range(0, 10000);
					num7 /= 100.0;
					double num8 = ((GameManager.currentTheme <= 100) ? 2.0 : 1.42);
					num8 += num8 / 100.0 * Singleton<StatManager>.instance.weaponSkinCollectEventItemDropChance;
					if (num7 < num8)
					{
						Singleton<CollectEventManager>.instance.spawnCollectEventResource(base.cachedTransform.position, 1L);
					}
				}
				double num9 = (double)UnityEngine.Random.Range(0, 10000) / 100.0;
				double num10 = ((GameManager.currentTheme <= 100) ? 0.4 : 0.284);
				if (num9 < num10)
				{
					Singleton<WeaponSkinManager>.instance.spawnWeaponSkinPiece(base.cachedTransform.position);
				}
			}
			num2 += num2 / 100f * (Singleton<StatManager>.instance.treasureChestDropExtraPercentChance * 0.82f);
			if (num < num2)
			{
				Singleton<TreasureChestManager>.instance.spawnTreasure(base.cachedTransform.position, num3, num4, num5, num6, 0.0, false);
			}
		}
		if (isBoss)
		{
			return;
		}
		int num11 = UnityEngine.Random.Range(2, 4);
		for (int i = 0; i < num11; i++)
		{
			double num12 = CalculateManager.getGoldValueForMonsters() / (double)num11;
			if (BossRaidManager.isBossRaid)
			{
				num12 = Math.Max(num12 / 20.0, 1.0);
			}
			if (!BossRaidManager.isBossRaid)
			{
				Singleton<GoldManager>.instance.spawnGold(base.cachedTransform.position, num12);
			}
			else
			{
				Singleton<GoldManager>.instance.spawnGold(base.cachedTransform.position, num12);
			}
		}
	}

	protected override IEnumerator dieEffect()
	{
		if (BossRaidManager.isBossRaid)
		{
			if (isBoss)
			{
				UIWindowIngame.instance.isCanPause = false;
				Singleton<BossRaidManager>.instance.miniBossClearEvent();
				float timer = 0f;
				float timerForEffect = 0f;
				float maxTimerForEffect = 0.2f;
				while (true)
				{
					if (GameManager.isPause)
					{
						continue;
					}
					for (int m = 0; m < Singleton<CharacterManager>.instance.constCharacterList.Count; m++)
					{
						if (Singleton<CharacterManager>.instance.constCharacterList[m] != null)
						{
							Singleton<CharacterManager>.instance.constCharacterList[m].setState(PublicDataManager.State.Wait);
						}
					}
					timerForEffect += Time.deltaTime * GameManager.timeScale;
					if (timerForEffect >= maxTimerForEffect)
					{
						maxTimerForEffect = Mathf.Max(maxTimerForEffect - 0.02f, 0.075f);
						timerForEffect = 0f;
						Vector2 targetBombPosition2 = (Vector2)cachedSpriteAnimation.cachedTransform.position + new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f));
						for (int l = 0; l < 2; l++)
						{
							double goldValue5 = CalculateManager.getGoldValueForMonsters() * 0.5;
							goldValue5 += goldValue5 / 100.0 * Singleton<StatManager>.instance.bossGoldGain;
							goldValue5 = Math.Max(goldValue5 / 20.0, 1.0);
							Singleton<GoldManager>.instance.spawnGold(base.cachedTransform.position, goldValue5);
						}
						if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
						{
							ObjectPool.Spawn("fx_boss_blowup", targetBombPosition2);
						}
						Singleton<AudioManager>.instance.playEffectSound("monster_blowup");
					}
					timer += Time.deltaTime * GameManager.timeScale;
					if (timer >= 1.5f)
					{
						break;
					}
					yield return null;
				}
				Singleton<BossRaidManager>.instance.tryToGetRandomChestForFossRaid(currentLevel, Singleton<BossRaidManager>.instance.currentStageForBossRaid, false, base.cachedTransform.position);
				if (Singleton<CharacterManager>.instance.warriorCharacter.getState() != PublicDataManager.State.Die)
				{
					for (int k = 0; k < Singleton<CharacterManager>.instance.constCharacterList.Count; k++)
					{
						if (Singleton<CharacterManager>.instance.constCharacterList[k] != null)
						{
							Singleton<CharacterManager>.instance.constCharacterList[k].setState(PublicDataManager.State.Move);
						}
					}
				}
				Singleton<SkillManager>.instance.LockSkillButton(false);
				Singleton<BossRaidManager>.instance.increaseStage(true);
				Singleton<EnemyManager>.instance.recycleEnemy(this);
				yield break;
			}
			float alpha2 = 1f;
			while (true)
			{
				alpha2 -= Time.deltaTime * GameManager.timeScale;
				m_cachedSpriteGroup.setAlpha(alpha2);
				if (alpha2 <= 0f)
				{
					break;
				}
				yield return null;
			}
			Singleton<EnemyManager>.instance.recycleEnemy(this);
			m_cachedSpriteGroup.setAlpha(0f);
			yield break;
		}
		if (isBoss)
		{
			Singleton<GameManager>.instance.isBossDie = true;
			Singleton<CharacterManager>.instance.warriorCharacter.setState(PublicDataManager.State.Wait);
			Singleton<AudioManager>.instance.stopBackgroundSound();
			float delay = 0.3f / GameManager.timeScale;
			while (true)
			{
				for (int j = 0; j < Singleton<CharacterManager>.instance.constCharacterList.Count; j++)
				{
					if (Singleton<CharacterManager>.instance.constCharacterList[j] != null && Singleton<CharacterManager>.instance.constCharacterList[j].getState() != PublicDataManager.State.Wait)
					{
						Singleton<CharacterManager>.instance.constCharacterList[j].setState(PublicDataManager.State.Wait);
					}
				}
				if (delay <= 0f)
				{
					break;
				}
				if (delay < 0.2f)
				{
					Vector2 targetBombPosition = (Vector2)cachedSpriteAnimation.cachedTransform.position + new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f));
					double goldValue2 = CalculateManager.getGoldValueForMonsters() * 15.0 / 10.0 / 3.0;
					goldValue2 += goldValue2 / 100.0 * Singleton<StatManager>.instance.bossGoldGain;
					for (int i = 0; i < 3; i++)
					{
						Singleton<GoldManager>.instance.spawnGold(base.cachedTransform.position, goldValue2);
					}
					if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
					{
						ObjectPool.Spawn("fx_boss_blowup", targetBombPosition);
					}
					Singleton<AudioManager>.instance.playEffectSound("monster_blowup");
				}
				if (Singleton<CachedManager>.instance.ingameCoverUI.alpha != 0f || delay < 0.3f)
				{
				}
				yield return new WaitForSeconds(delay);
				StartCoroutine("ColorUpdate");
				delay -= 0.02f;
			}
			StopCoroutine("ColorUpdate");
			dropEvent();
			Singleton<EnemyManager>.instance.recycleEnemy(this);
			if (!BossRaidManager.isBossRaid)
			{
				Singleton<EnemyManager>.instance.endBossSafe();
			}
			yield break;
		}
		float alpha = 1f;
		while (true)
		{
			alpha -= Time.deltaTime * GameManager.timeScale;
			m_cachedSpriteGroup.setAlpha(alpha);
			if (alpha <= 0f)
			{
				break;
			}
			yield return null;
		}
		Singleton<EnemyManager>.instance.recycleEnemy(this);
		m_cachedSpriteGroup.setAlpha(0f);
	}

	private IEnumerator ColorUpdate()
	{
		bool color = false;
		while (true)
		{
			color = !color;
			if (color)
			{
				cachedSpriteAnimation.targetRenderer.color = Color.white;
			}
			else
			{
				cachedSpriteAnimation.targetRenderer.color = new Color(41f / 85f, 79f / 255f, 1f);
			}
			yield return new WaitForSeconds(0.07f);
		}
	}

	public override void stunEnemy(float stunTime)
	{
		if (m_currentStunEffectObject != null)
		{
			ObjectPool.Recycle(m_currentStunEffectObject.name, m_currentStunEffectObject);
			m_currentStunEffectObject = null;
		}
		if (isBoss)
		{
			m_currentStunEffectObject = ObjectPool.Spawn("@BossStunEffect", new Vector2(0f, 1.3f), base.cachedTransform);
		}
		else
		{
			m_currentStunEffectObject = ObjectPool.Spawn("@StunEffect", new Vector2(0f, 1.3f), base.cachedTransform);
		}
		StopCoroutine("waitStunUpdate");
		StartCoroutine("waitStunUpdate", stunTime);
		setState(PublicDataManager.State.Stun);
	}

	protected override void dieEvent()
	{
		if (m_currentConfusionObject != null)
		{
			ObjectPool.Recycle(m_currentConfusionObject.name, m_currentConfusionObject);
			m_currentConfusionObject = null;
		}
		if (elasticObject != null)
		{
			ObjectPool.Recycle("@ElasticBullet", elasticObject.gameObject);
			elasticObject = null;
		}
		StopCoroutine("waitForFrozenDurationUpdate");
		if (m_cachedEliteEffect != null)
		{
			ObjectPool.Recycle(m_cachedEliteEffect.name, m_cachedEliteEffect);
		}
		if (m_cachedFrostEffect != null)
		{
			ObjectPool.Recycle(m_cachedFrostEffect.name, m_cachedFrostEffect.cachedGameObject);
		}
		StopCoroutine("waitStunUpdate");
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.ProHunter, 1.0);
		Singleton<QuestManager>.instance.questReport(QuestManager.QuestType.MonsterHunt, 1.0);
		if (BossRaidManager.isBossRaid && (isMiniboss || isBoss))
		{
			BossRaidManager.BossRaidBestRecordData bossRaidBestRecordData = default(BossRaidManager.BossRaidBestRecordData);
			bossRaidBestRecordData.stage = Singleton<BossRaidManager>.instance.currentStageForBossRaid;
			bossRaidBestRecordData.isBossMonster = false;
			bossRaidBestRecordData.isMiniBossMonster = true;
			bossRaidBestRecordData.monsterLevel = currentLevel;
			bossRaidBestRecordData.isInitialized = true;
			bossRaidBestRecordData.monsterType = currentMonsterType;
			Singleton<BossRaidManager>.instance.lastKillBossData = bossRaidBestRecordData;
			if (Singleton<BossRaidManager>.instance.currentStageForBossRaid >= Singleton<DataManager>.instance.currentGameData.bossRaidBestRecord.stage)
			{
				Singleton<DataManager>.instance.currentGameData.bossRaidBestRecord = bossRaidBestRecordData;
				Singleton<DataManager>.instance.saveData();
			}
		}
		if (isBoss)
		{
			isDead = true;
			cachedSpriteAnimation.playAnimation(currentAnimationName.dieName);
			Singleton<SkillManager>.instance.LockSkillButton(true);
			UIWindowIngame.instance.isCanPause = false;
			if (!BossRaidManager.isBossRaid)
			{
				for (int i = 0; i < Singleton<EnemyManager>.instance.enemyList.Count; i++)
				{
					if (Singleton<EnemyManager>.instance.enemyList[i] != this)
					{
						Singleton<EnemyManager>.instance.enemyList[i].decreasesHealth(Singleton<EnemyManager>.instance.enemyList[i].maxHealth);
					}
				}
			}
			StartCoroutine("dieEffect");
			return;
		}
		dropEvent();
		isDead = true;
		cachedSpriteAnimation.playAnimation(currentAnimationName.dieName);
		jump(targetYPos: Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position).y, velocity: new Vector2(-0.9f * getDirection(), 4f));
		StartCoroutine("dieEffect");
		if (Singleton<GroundManager>.instance.getFloor() != EnemyManager.bossStageFloor - 1)
		{
			return;
		}
		bool flag = false;
		for (int j = 0; j < Singleton<EnemyManager>.instance.enemyList.Count; j++)
		{
			if (!Singleton<EnemyManager>.instance.enemyList[j].isBoss)
			{
				if (!Singleton<EnemyManager>.instance.enemyList[j].isDead)
				{
					flag = false;
					break;
				}
				flag = true;
			}
		}
		if (flag)
		{
			Singleton<SkillManager>.instance.LockSkillButton(true);
		}
	}

	protected override IEnumerator Die()
	{
		dieEvent();
		while (true)
		{
			yield return null;
		}
	}

	protected override void waitEvent()
	{
	}

	protected override IEnumerator Wait()
	{
		waitEvent();
		m_waitTimer = 0f;
		bool isPlaying = false;
		yield return null;
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (!m_isAttacking && !isPlaying)
				{
					isPlaying = true;
					cachedSpriteAnimation.playAnimation(currentAnimationName.idleName, 0.1f, true);
				}
				m_waitTimer += Time.deltaTime * GameManager.timeScale;
				if (m_waitTimer >= curDelay)
				{
					break;
				}
			}
			yield return null;
		}
		m_waitTimer = 0f;
		setState(PublicDataManager.State.Idle);
	}

	public override void decreasesHealth(double damage)
	{
		if ((!(m_targetCharacter != null) || !m_targetCharacter.isDead) && !isDead)
		{
			if (m_isConfusion)
			{
				damage += damage / 100.0 * Singleton<StatManager>.instance.confusionValueFromReinforcementCloneWarrior;
			}
			base.decreasesHealth(damage);
			curHealth = Math.Max(curHealth - damage, 0.0);
			StopCoroutine(hitEffect());
			StartCoroutine(hitEffect());
			m_gaugeTimer = 0f;
			if (!m_hit)
			{
				m_hit = true;
			}
			if (curHealth <= 0.0)
			{
				setState(PublicDataManager.State.Die);
			}
		}
	}
}
