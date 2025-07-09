using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossObject : EnemyObject
{
	public bool isCanAttack;

	public bool nonviolence;

	public EnemyManager.BossType currentBossType;

	private bool isRangeAttack;

	private bool isAttack1;

	protected override void Awake()
	{
		myBossObjcect = this;
		base.Awake();
	}

	public void setBossMonsterDetailAttribute(EnemyManager.BossType targetType)
	{
		EnemyManager.BossMonsterDetailAttributeData bossMonsterDetailAttributeData = Singleton<EnemyManager>.instance.currentBossMonsterDetailAttributeDictionary[targetType];
		cachedSpriteAnimation.cachedTransform.localPosition = bossMonsterDetailAttributeData.localPosition;
		cachedSpriteAnimation.cachedTransform.localScale = bossMonsterDetailAttributeData.localScale;
		currentAnimationName = bossMonsterDetailAttributeData.animationNameData;
	}

	public override void init(int lv)
	{
		setBossMonsterDetailAttribute(currentBossType);
		if (currentBossType >= EnemyManager.BossType.FairyKing)
		{
			cachedSpriteAnimation.animationType = currentBossType.ToString() + "Elite";
			isElite = true;
		}
		else
		{
			cachedSpriteAnimation.animationType = currentBossType.ToString();
			isElite = false;
		}
		if (Singleton<GameManager>.instance.isThemeClearEvent)
		{
			cachedSpriteAnimation.targetRenderer.sortingOrder = -1;
		}
		else
		{
			cachedSpriteAnimation.targetRenderer.sortingOrder = 2;
		}
		cachedSpriteAnimation.init();
		nonviolence = false;
		base.init(lv);
		Singleton<EnemyManager>.instance.enemyList.Add(this);
	}

	public override void setState(PublicDataManager.State targetState)
	{
		base.setState(targetState);
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
		m_percentHealthForRegen = maxHealth / 100.0 * 15.0;
	}

	protected override void moveInit()
	{
		cachedSpriteAnimation.playAnimation(Singleton<EnemyManager>.instance.bossObject.currentAnimationName.walkName, 0.1f, true);
	}

	protected override void idleInit()
	{
		cachedSpriteAnimation.playAnimation(currentAnimationName.idleName, 0.1f);
	}

	protected override IEnumerator Idle()
	{
		idleInit();
		Vector2 position2 = Vector2.zero;
		CharacterObject targetCharacter;
		while (true)
		{
			yield return null;
			position2 = base.cachedTransform.position;
			if (nonviolence || !isCanAttack)
			{
				continue;
			}
			targetCharacter = Singleton<CharacterManager>.instance.getNearestCharacter(position2, curAttackRange);
			if (targetCharacter != null)
			{
				m_attackTimer += Time.deltaTime * GameManager.timeScale;
				if (m_attackTimer >= curDelay)
				{
					break;
				}
			}
		}
		m_attackTimer = 0f;
		m_targetCharacter = targetCharacter;
		setState(PublicDataManager.State.Attack);
	}

	private void attack(bool attack1)
	{
		m_isAttacking = true;
		setState(PublicDataManager.State.Wait);
		setDirection(MovingObject.calculateDirection(base.cachedTransform.position, m_targetCharacter.cachedTransform.position));
		SpriteAnimation attackEffect = null;
		Bullet[] bullets = null;
		SpriteAnimation animation = null;
		switch (currentBossType)
		{
		case EnemyManager.BossType.QueenHornet:
			bullets = new Bullet[8];
			attackEffect = ObjectPool.Spawn("BossAttack", Vector2.zero, base.cachedTransform).GetComponent<SpriteAnimation>();
			break;
		case EnemyManager.BossType.FairyKing:
		{
			GameObject fairyKingHit = ObjectPool.Spawn("@FairyKingHit", m_targetCharacter.cachedTransform.position);
			animation = fairyKingHit.transform.GetChild(0).GetComponent<SpriteAnimation>();
			animation.cachedTransform.localPosition = ((!attack1) ? new Vector2(0f, 1.034f) : new Vector2(0f, 0.71f));
			animation.animationType = currentBossType.ToString();
			animation.init();
			animation.playAnimation((!attack1) ? "Effect2" : "Effect1", curDelay * 0.1f, false, delegate
			{
				ObjectPool.Recycle("@FairyKingHit", fairyKingHit);
			});
			break;
		}
		case EnemyManager.BossType.Lavadragon:
		{
			if (!attack1)
			{
				attackEffect = ObjectPool.Spawn("BossAttack", Vector2.zero, base.cachedTransform).GetComponent<SpriteAnimation>();
			}
			GameObject fire = ObjectPool.Spawn("@LavadragonBullet", (!attack1) ? new Vector2(2.95f, 1.32f) : new Vector2(2.61f, 0.8f), base.cachedTransform);
			animation = fire.transform.GetChild(0).GetComponent<SpriteAnimation>();
			animation.animationType = currentBossType.ToString();
			animation.init();
			animation.playAnimation((!attack1) ? "Fire2" : "Fire1", curDelay * 0.1f, false, delegate
			{
				ObjectPool.Recycle("@LavadragonBullet", fire);
			});
			break;
		}
		case EnemyManager.BossType.GrimReaper:
		{
			GameObject grimReaperHit = ObjectPool.Spawn("@GrimReaperHit", (Vector2)m_targetCharacter.cachedTransform.position + ((!attack1) ? new Vector2(0f, 2f) : new Vector2(0f, 1f)), Vector3.zero, new Vector3(getDirection(), 1f, 1f));
			animation = grimReaperHit.transform.GetChild(0).GetComponent<SpriteAnimation>();
			animation.animationType = currentBossType.ToString();
			animation.init();
			animation.playAnimation((!attack1) ? "Effect2" : "Effect1", curDelay * 0.1f, false, delegate
			{
				ObjectPool.Recycle("@GrimReaperHit", grimReaperHit);
			});
			break;
		}
		default:
			attackEffect = ObjectPool.Spawn("BossAttack", Vector2.zero, base.cachedTransform).GetComponent<SpriteAnimation>();
			break;
		}
		if (attackEffect != null)
		{
			attackEffect.transform.localPosition = cachedSpriteAnimation.transform.localPosition;
			attackEffect.transform.localScale = cachedSpriteAnimation.transform.localScale;
			attackEffect.animationType = currentBossType.ToString();
			attackEffect.animationName = ((!attack1) ? "Effect2" : "Effect1");
			attackEffect.init();
			attackEffect.playAnimation((!attack1) ? "Effect2" : "Effect1", curDelay * 0.1f, false, delegate
			{
				ObjectPool.Recycle("BossAttack", attackEffect.gameObject);
			});
		}
		cachedSpriteAnimation.playAnimation((!attack1) ? "Attack2" : currentAnimationName.attackName, curDelay * 0.1f, false, SpriteAnimation.FrameList.CenterFrame, delegate
		{
			isRangeAttack = false;
			switch (currentBossType)
			{
			case EnemyManager.BossType.QueenHornet:
				if (!attack1)
				{
					isRangeAttack = true;
					for (int j = 0; j < 8; j++)
					{
						bullets[j] = ObjectPool.Spawn("@QueenHornetBullet", (Vector2)cachedSpriteAnimation.cachedTransform.position + new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f)), Vector3.zero, new Vector3(getDirection(), 1f, 1f)).GetComponent<Bullet>();
						bullets[j].GetComponent<FadeChangableObject>().setAlpha(1f);
						animation = bullets[j].cachedImageTransform.GetComponent<SpriteAnimation>();
						animation.animationType = currentBossType.ToString();
						animation.init();
						animation.playAnimation("Bullet", 0.1f, true);
					}
				}
				break;
			case EnemyManager.BossType.GreatGolem:
			case EnemyManager.BossType.KingScorpion:
				if (attack1)
				{
					Singleton<CachedManager>.instance.ingameCameraShake.shake(3f, 0.1f);
				}
				break;
			case EnemyManager.BossType.KingLizard:
			case EnemyManager.BossType.Lavadragon:
			case EnemyManager.BossType.GrimReaper:
			case EnemyManager.BossType.Daemon1:
				if (!attack1)
				{
					Singleton<CachedManager>.instance.ingameCameraShake.shake(3f, 0.1f);
				}
				break;
			}
			if (!m_targetCharacter.isDead && !isRangeAttack && isCanAttack)
			{
				double num2 = curDamage * (double)UnityEngine.Random.Range(0.9f, 1.1f);
				if (!m_targetCharacter.isDead)
				{
					num2 -= num2 / 100.0 * Singleton<StatManager>.instance.decreaseDamageFromHitBoss;
					m_targetCharacter.decreasesHealth(num2);
					m_targetCharacter.setDamageText(num2, true);
					Singleton<AudioManager>.instance.playEffectSound("monster_boss_attack");
				}
			}
		}, delegate
		{
			if (currentBossType == EnemyManager.BossType.QueenHornet)
			{
				if (!attack1)
				{
					for (int i = 0; i < 8; i++)
					{
						bullets[i].shootBullet(m_targetCharacter.cachedTransform.position, delegate(Transform t)
						{
							double num = curDamage * (double)UnityEngine.Random.Range(0.9f, 1.1f) / 8.0;
							if (!m_targetCharacter.isDead)
							{
								num -= num / 100.0 * Singleton<StatManager>.instance.decreaseDamageFromHitBoss;
								m_targetCharacter.decreasesHealth(num);
								m_targetCharacter.setDamageText(num, true);
								Singleton<AudioManager>.instance.playEffectSound("monster_boss_attack");
							}
							t.GetComponent<FadeChangableObject>().fadeIn(5f, delegate
							{
								t.GetComponent<Bullet>().recycleBullet();
							});
						}, Vector2.zero);
					}
				}
			}
			else
			{
				m_targetCharacter = null;
			}
			m_isAttacking = false;
		});
	}

	protected override IEnumerator Attack()
	{
		isAttack1 = !isAttack1;
		attack(isAttack1);
		while (true)
		{
			yield return null;
		}
	}

	public override void stunEnemy(float stunTime)
	{
		if (m_currentStunEffectObject != null)
		{
			ObjectPool.Recycle(m_currentStunEffectObject.name, m_currentStunEffectObject);
			m_currentStunEffectObject = null;
		}
		m_currentStunEffectObject = ObjectPool.Spawn("@BossStunEffect", new Vector2(0f, 1.3f), base.cachedTransform);
		StopCoroutine("waitStunUpdate");
		StartCoroutine("waitStunUpdate", stunTime);
		setState(PublicDataManager.State.Stun);
	}

	protected override void dropEvent()
	{
		Singleton<TreasureChestManager>.instance.spawnTreasure(base.cachedTransform.position, 0.0, 0.0, 100.0, 0.0, 0.0, true);
	}

	private IEnumerator allCharacterSetStateToWait()
	{
		while (true)
		{
			for (int i = 0; i < Singleton<CharacterManager>.instance.characterList.Count; i++)
			{
				Singleton<CharacterManager>.instance.characterList[i].setState(PublicDataManager.State.Wait);
			}
			yield return null;
		}
	}

	protected override IEnumerator dieEffect()
	{
		float delay = 0.5f / GameManager.timeScale;
		if (BossRaidManager.isBossRaid)
		{
			StopCoroutine("allCharacterSetStateToWait");
			StartCoroutine("allCharacterSetStateToWait");
			Singleton<BossRaidManager>.instance.realBossClearEvent();
			Singleton<SkillManager>.instance.isClonedWarrior = false;
			UIWindowIngame.instance.isCanPause = false;
			while (true)
			{
				if (delay < 0.4f)
				{
					Vector2 targetBombPosition2 = (Vector2)cachedSpriteAnimation.cachedTransform.position + new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f));
					for (int k = 0; k < 2; k++)
					{
						double goldValue5 = CalculateManager.getGoldValueForMonsters() * 0.5;
						goldValue5 += goldValue5 / 100.0 * Singleton<StatManager>.instance.bossGoldGain;
						goldValue5 = Math.Max(goldValue5 / 20.0, 1.0);
						Singleton<GoldManager>.instance.spawnGold(base.cachedTransform.position, goldValue5);
						Dictionary<DropItemManager.DropItemType, double> totalIngameGainDropItem;
						Dictionary<DropItemManager.DropItemType, double> dictionary = (totalIngameGainDropItem = Singleton<DropItemManager>.instance.totalIngameGainDropItem);
						DropItemManager.DropItemType key;
						DropItemManager.DropItemType key2 = (key = DropItemManager.DropItemType.Gold);
						double num = totalIngameGainDropItem[key];
						dictionary[key2] = num + goldValue5;
					}
					if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
					{
						ObjectPool.Spawn("fx_boss_blowup", targetBombPosition2);
					}
					Singleton<AudioManager>.instance.playEffectSound("monster_blowup");
				}
				if (Singleton<CachedManager>.instance.ingameCoverUI.alpha != 0f || delay < 0.3f)
				{
				}
				if (delay <= 0f)
				{
					break;
				}
				yield return new WaitForSeconds(delay);
				StartCoroutine("ColorUpdate", new Color(41f / 85f, 79f / 255f, 1f));
				delay -= 0.02f;
			}
			StopCoroutine("allCharacterSetStateToWait");
			Singleton<BossRaidManager>.instance.tryToGetRandomChestForFossRaid(currentLevel, Singleton<BossRaidManager>.instance.currentStageForBossRaid, true, base.cachedTransform.position);
			Singleton<EnemyManager>.instance.recycleEnemy(this);
			Singleton<BossRaidManager>.instance.goToNextTheme();
			yield break;
		}
		if (isBoss)
		{
			Singleton<GameManager>.instance.isBossDie = true;
			StopCoroutine("allCharacterSetStateToWait");
			StartCoroutine("allCharacterSetStateToWait");
			UIWindowIngame.instance.isCanPause = false;
			Singleton<CharacterManager>.instance.warriorCharacter.setState(PublicDataManager.State.Wait);
			Singleton<AudioManager>.instance.stopBackgroundSound();
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
				if (delay < 0.4f)
				{
					Vector2 targetBombPosition = (Vector2)cachedSpriteAnimation.cachedTransform.position + new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f));
					double goldValue2 = CalculateManager.getGoldValueForMonsters() * 15.0 / 20.0 / 3.0;
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
				StartCoroutine("ColorUpdate", new Color(41f / 85f, 79f / 255f, 1f));
				delay -= 0.02f;
			}
			StopCoroutine("ColorUpdate");
			dropEvent();
			if (currentBossType >= EnemyManager.BossType.FairyKing && !isElite)
			{
				switch (currentBossType)
				{
				case EnemyManager.BossType.FairyKing:
					base.cachedTransform.localPosition = new Vector2(-1.1f, base.cachedTransform.localPosition.y + 0.9f);
					base.cachedTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
					break;
				case EnemyManager.BossType.Lavadragon:
					base.cachedTransform.localPosition = new Vector2(-2.5f, base.cachedTransform.localPosition.y + 2f);
					base.cachedTransform.localScale = new Vector3(-1.2f, -1.2f, 1f);
					break;
				case EnemyManager.BossType.GrimReaper:
					base.cachedTransform.localPosition = new Vector2(-1.1f, base.cachedTransform.localPosition.y + 1f);
					base.cachedTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
					break;
				case EnemyManager.BossType.Daemon1:
					base.cachedTransform.localPosition = new Vector2(-1f, base.cachedTransform.localPosition.y + 0.8f);
					base.cachedTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
					break;
				}
			}
			else
			{
				Singleton<EnemyManager>.instance.recycleEnemy(this);
			}
			Singleton<EnemyManager>.instance.endBossSafe();
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
		Singleton<TreasureChestManager>.instance.spawnTreasure(base.cachedTransform.position, 90.0, 10.0, 0.0, 0.0, 0.0, true);
	}

	protected IEnumerator rebornEvent()
	{
		bool isEliteForm = false;
		Sprite[] eliteImage = Singleton<ResourcesManager>.instance.getAnimation(currentBossType.ToString() + "Elite");
		Sprite dieSprite = null;
		for (int i = 0; i < eliteImage.Length; i++)
		{
			if (eliteImage[i].name.Contains("Die"))
			{
				dieSprite = eliteImage[i];
				break;
			}
		}
		float delay = 0.2f;
		while (!(delay <= 0f))
		{
			if (isEliteForm)
			{
				cachedSpriteAnimation.targetRenderer.sprite = dieSprite;
			}
			else
			{
				cachedSpriteAnimation.playFixAnimation("Die", 0);
			}
			isEliteForm = !isEliteForm;
			yield return new WaitForSeconds(delay);
			delay -= 0.005f;
		}
		if (currentBossType >= EnemyManager.BossType.FairyKing && !isElite)
		{
			cachedSpriteAnimation.targetRenderer.sprite = dieSprite;
			cachedSpriteAnimation.animationType = currentBossType.ToString() + "Elite";
			cachedSpriteAnimation.init();
			isCanAttack = false;
			yield return new WaitForSeconds(1f);
			switch (currentBossType)
			{
			case EnemyManager.BossType.FairyKing:
				base.cachedTransform.localPosition = new Vector2(-2f, base.cachedTransform.localPosition.y - 0.9f);
				base.cachedTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				break;
			case EnemyManager.BossType.Lavadragon:
				base.cachedTransform.localPosition = new Vector2(-2f, base.cachedTransform.localPosition.y - 2f);
				base.cachedTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
				break;
			case EnemyManager.BossType.GrimReaper:
				base.cachedTransform.localPosition = new Vector2(-2f, base.cachedTransform.localPosition.y - 1f);
				base.cachedTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				break;
			case EnemyManager.BossType.Daemon1:
				base.cachedTransform.localPosition = new Vector2(-1.5f, base.cachedTransform.localPosition.y - 0.8f);
				base.cachedTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				break;
			}
			setState(PublicDataManager.State.Idle);
			Singleton<CharacterManager>.instance.warriorCharacter.jumpWithFollowingCharacter(new Vector2(2f, 3f), delegate
			{
			});
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			yield return new WaitForSeconds(0.5f);
			Singleton<CachedManager>.instance.emotionUI.SetEmotion(EmotionUI.emotionType.Angry);
			yield return new WaitForSeconds(1.5f);
			m_targetCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
			attack(false);
			yield return new WaitForSeconds(0.5f);
			Singleton<CharacterManager>.instance.warriorCharacter.jumpWithFollowingCharacter(new Vector2(2f, 3f), delegate
			{
			});
			yield return new WaitForSeconds(2f);
			moveTo(new Vector2(-4f, base.cachedTransform.position.y), curSpeed * 2f, delegate
			{
				Singleton<CachedManager>.instance.coverUI.fadeOutGame(false, false, delegate
				{
					UIWindowThemeUnlock.instance.OpenEliteDungeon();
				});
				moveTo(new Vector2(-6f, base.cachedTransform.position.y), curSpeed * 2f);
			});
		}
		else
		{
			Singleton<EnemyManager>.instance.recycleEnemy(this);
		}
		Singleton<EnemyManager>.instance.endBossSafe();
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

	protected override void dieEvent()
	{
		if (m_currentConfusionObject != null)
		{
			ObjectPool.Recycle(m_currentConfusionObject.name, m_currentConfusionObject);
			m_currentConfusionObject = null;
		}
		if (m_cachedFrostEffect != null)
		{
			ObjectPool.Recycle(m_cachedFrostEffect.name, m_cachedFrostEffect.cachedGameObject);
			m_cachedFrostEffect = null;
		}
		if (currentBossType == EnemyManager.BossType.QueenHornet)
		{
			ObjectPool.Clear("@QueenHornetBullet");
		}
		Singleton<QuestManager>.instance.questReport(QuestManager.QuestType.MonsterHunt, 1.0);
		if (!BossRaidManager.isBossRaid && currentBossType == EnemyManager.BossType.Daemon1 && isElite && GameManager.currentTheme == Singleton<DataManager>.instance.currentGameData.bestTheme)
		{
			Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.PieceOfCakeKillDemonKing, 1.0);
		}
		StopCoroutine("waitStunUpdate");
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.ProHunter, 1.0);
		if (BossRaidManager.isBossRaid)
		{
			BossRaidManager.BossRaidBestRecordData bossRaidBestRecordData = default(BossRaidManager.BossRaidBestRecordData);
			bossRaidBestRecordData.stage = Singleton<BossRaidManager>.instance.currentStageForBossRaid;
			bossRaidBestRecordData.isBossMonster = true;
			bossRaidBestRecordData.isMiniBossMonster = false;
			bossRaidBestRecordData.monsterLevel = currentLevel;
			bossRaidBestRecordData.isInitialized = true;
			bossRaidBestRecordData.bossType = currentBossType;
			Singleton<BossRaidManager>.instance.lastKillBossData = bossRaidBestRecordData;
			if (Singleton<BossRaidManager>.instance.currentStageForBossRaid >= Singleton<DataManager>.instance.currentGameData.bossRaidBestRecord.stage)
			{
				Singleton<DataManager>.instance.currentGameData.bossRaidBestRecord = bossRaidBestRecordData;
				Singleton<DataManager>.instance.saveData();
			}
			Singleton<BossRaidManager>.instance.increaseStage();
		}
		isDead = true;
		cachedSpriteAnimation.playAnimation(currentAnimationName.dieName);
		Vector2 hitGroundPosition = Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position);
		if (isBoss && !BossRaidManager.isBossRaid)
		{
			Singleton<SkillManager>.instance.LockSkillButton(true);
		}
		StartCoroutine("dieEffect");
		if (!BossRaidManager.isBossRaid)
		{
			for (int i = 0; i < Singleton<EnemyManager>.instance.enemyList.Count; i++)
			{
				Singleton<EnemyManager>.instance.enemyList[i].decreasesHealth(Singleton<EnemyManager>.instance.enemyList[i].maxHealth);
			}
		}
		UIWindowIngame.instance.StopCoroutine("bossWarningUpdate");
		UIWindowIngame.instance.StopCoroutine("bossWarningLabelUpdate");
		UIWindowIngame.instance.bossWarning.SetActive(false);
		UIWindowIngame.instance.hpWarning.gameObject.SetActive(false);
	}

	protected override IEnumerator Die()
	{
		dieEvent();
		while (true)
		{
			yield return null;
		}
	}

	protected override IEnumerator Wait()
	{
		waitEvent();
		m_waitTimer = 0f;
		bool isPlaying = false;
		do
		{
			yield return null;
			if (!m_isAttacking && !isPlaying)
			{
				isPlaying = true;
				cachedSpriteAnimation.playAnimation(currentAnimationName.idleName, curDelay * 0.1f);
			}
			m_waitTimer += Time.deltaTime * GameManager.timeScale;
		}
		while (!(m_waitTimer >= curDelay));
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

	public override void setDamageText(double damage, CharacterManager.CharacterType attackerType, bool isCritical = false, bool isSkillAttack = false, bool isHitting = false, bool isTranscendSkill = false)
	{
		if (Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			return;
		}
		if (m_isConfusion)
		{
			damage += damage / 100.0 * Singleton<StatManager>.instance.confusionValueFromReinforcementCloneWarrior;
		}
		Color white = Color.white;
		m_damageTextHeightCount++;
		Vector3 b = new Vector3(getDirection() * 0.4f, 2.5f + UnityEngine.Random.Range(0.3f, 0.5f), (float)m_damageTextHeightCount * -0.01f);
		if (m_damageTextHeightCount > 7)
		{
			m_damageTextHeightCount = 0;
		}
		if (isTranscendSkill)
		{
			ObjectPool.Spawn("@DamageYellowText", cachedSpriteAnimation.cachedTransform.position + b).GetComponent<CustomText>().setText(damage, 1.3f, CustomText.TextEffectType.BreakAway, 1f);
		}
		else if (isSkillAttack)
		{
			if (isHitting)
			{
				if (!isCritical)
				{
					ObjectPool.Spawn("@DamageYellowText", base.cachedTransform.position + b).GetComponent<CustomText>().setText(damage, 0.8f, CustomText.TextEffectType.Up, 1f);
				}
				else
				{
					ObjectPool.Spawn("@DamageYellowText", base.cachedTransform.position + b).GetComponent<CustomText>().setText(damage, 1.3f, CustomText.TextEffectType.Up, 3f);
				}
			}
			else
			{
				ObjectPool.Spawn("@DamageYellowText", base.cachedTransform.position + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(0.8f, 1.1f), 0f)).GetComponent<CustomText>().setText(damage, 0.6f, CustomText.TextEffectType.BreakAway, 1f);
			}
		}
		else if (!isCritical)
		{
			ObjectPool.Spawn("@DamageText", base.cachedTransform.position + b).GetComponent<CustomText>().setText(damage, 0.8f, CustomText.TextEffectType.Up, 1f);
		}
		else
		{
			ObjectPool.Spawn("@DamageText", base.cachedTransform.position + b).GetComponent<CustomText>().setText(damage, 1.3f, CustomText.TextEffectType.Up, 3f);
		}
	}
}
