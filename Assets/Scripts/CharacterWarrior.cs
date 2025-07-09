using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWarrior : CharacterObject
{
	public CharacterSkinManager.WarriorSkinType skinType;

	public bool isUsedBossStageAction;

	public bool isCanAttackBoss;

	public bool isCastingSkill;

	public bool isCastedDivineSmashFromPreview;

	private bool m_isRevived;

	private ParticleSystem m_currentCastEffect;

	private bool m_isReinforcementDivineSmash;

	private GameObject strikeEffect;

	public EnemyObject attackedEnemy;

	public override void startGame()
	{
		m_isReinforcementDivineSmash = false;
		m_isFirstGround = true;
		isCastedDivineSmashFromPreview = false;
		isCastingSkill = false;
		m_isRevived = false;
		isCanAttackBoss = false;
		isUsedBossStageAction = false;
		base.startGame();
	}

	public override void resetProperties(bool changeAnimation = true)
	{
		m_currentAnimationName = string.Empty;
		if (changeAnimation)
		{
			removeAllAnimations();
			List<AnimationClip> list = null;
			list = ((skinType != CharacterSkinManager.WarriorSkinType.Valkyrie1) ? Singleton<AnimationManager>.instance.warriorNormalAnimationClipList : Singleton<AnimationManager>.instance.warriorValkyrieAnimationClipList);
			for (int i = 0; i < list.Count; i++)
			{
				characterBoneAnimation.AddClip(list[i], list[i].name);
			}
			playBoneAnimation(currentBoneAnimationName.idleName[0]);
		}
		base.resetProperties(changeAnimation);
		setDontStop(false);
		baseHealth += baseHealth / 100.0 * (Singleton<StatManager>.instance.allPercentHealthFromTreasure + Singleton<StatManager>.instance.warriorSkinHealthPercent + Singleton<StatManager>.instance.warriorPercentHealthFromColleague);
		baseHealth += baseHealth / 100.0 * Singleton<StatManager>.instance.specialAdsAngelHealth;
		baseHealth += baseHealth / 100.0 * Singleton<StatManager>.instance.percentHealthFromPremiumTreasure;
		baseHealth += baseHealth / 100.0 * Singleton<StatManager>.instance.weaponSkinTotalPercentHealth;
		if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(ColleagueManager.ColleagueType.Angela).isUnlocked)
		{
			baseHealth += baseHealth / 100.0 * Singleton<ColleagueManager>.instance.getPremiumColleagueValue(ColleagueManager.ColleagueType.Angela);
		}
		baseHealth += baseHealth / 100.0 * Singleton<StatManager>.instance.allHPAndTapHealFromTowerModeTreasure;
	}

	protected override void checkExtraCharacterStat()
	{
		m_extraDamageValue = Singleton<StatManager>.instance.warriorSkinPercentDamage;
	}

	protected override IEnumerator attackTimerUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause && !isDead && m_currentState != PublicDataManager.State.Wait)
			{
				m_attackTimer += Time.deltaTime * GameManager.timeScale;
				if (targetEnemy != null && targetEnemy.isDead)
				{
					targetEnemy = null;
					setState(PublicDataManager.State.Move);
				}
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

	public void castSkill(SkillManager.SkillType type, bool preview, bool isReinforcementSkill = false)
	{
		isCastedDivineSmashFromPreview = preview;
		switch (type)
		{
		case SkillManager.SkillType.DivineSmash:
			StopCoroutine("waitSkill");
			setState(PublicDataManager.State.CastSkill);
			if (TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType9);
			}
			if (isCastedDivineSmashFromPreview)
			{
				StopCoroutine("divineSmashUpdateForPreview");
				StartCoroutine("divineSmashUpdateForPreview");
				break;
			}
			m_isReinforcementDivineSmash = isReinforcementSkill;
			setStateLock(true);
			Singleton<AudioManager>.instance.playEffectSound("skill_jump", AudioManager.EffectType.Skill);
			if (m_currentCastEffect != null)
			{
				ObjectPool.Recycle(m_currentCastEffect.name, m_currentCastEffect.gameObject);
			}
			m_currentCastEffect = ObjectPool.Spawn("@CastSkill", (Vector2)base.cachedTransform.position + Vector2.up * 1.5f).GetComponent<ParticleSystem>();
			Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
			foreach (Transform item in m_currentCastEffect.transform)
			{
				item.gameObject.layer = LayerMask.NameToLayer("Player");
			}
			m_currentCastEffect.gameObject.layer = LayerMask.NameToLayer("Player");
			characterBoneAnimation[currentBoneAnimationName.skillName[1]].speed = 3f;
			playBoneAnimation(currentBoneAnimationName.skillName[1]);
			break;
		case SkillManager.SkillType.ClonedWarrior:
		{
			if (Singleton<SkillManager>.instance.currentCloneWarriorObject != null)
			{
				ObjectPool.Recycle(Singleton<SkillManager>.instance.currentCloneWarriorObject.name, Singleton<SkillManager>.instance.currentCloneWarriorObject.cachedGameObject);
			}
			CloneWarrior component = ObjectPool.Spawn("@ClonedWarrior", base.cachedTransform.localPosition, base.cachedTransform).GetComponent<CloneWarrior>();
			component.init(isReinforcementSkill);
			Singleton<SkillManager>.instance.currentCloneWarriorObject = component;
			break;
		}
		case SkillManager.SkillType.WhirlWind:
		case SkillManager.SkillType.Concentration:
			break;
		}
	}

	private IEnumerator divineSmashUpdateForPreview()
	{
		yield return new WaitForSeconds(0.5f);
		if (m_currentCastEffect != null)
		{
			ObjectPool.Recycle(m_currentCastEffect.name, m_currentCastEffect.gameObject);
		}
		m_currentCastEffect = ObjectPool.Spawn("@CastSkill", (Vector2)base.cachedTransform.position + Vector2.up * 1.5f).GetComponent<ParticleSystem>();
		Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
		foreach (Transform t in m_currentCastEffect.transform)
		{
			t.gameObject.layer = LayerMask.NameToLayer("PreviewLayer");
		}
		m_currentCastEffect.gameObject.layer = LayerMask.NameToLayer("PreviewLayer");
		setStateLock(true);
		Singleton<AudioManager>.instance.playEffectSound("skill_jump", AudioManager.EffectType.Skill);
		if (m_currentCastEffect != null)
		{
			ObjectPool.Recycle(m_currentCastEffect.name, m_currentCastEffect.gameObject);
		}
		m_currentCastEffect = ObjectPool.Spawn("@CastSkill", (Vector2)base.cachedTransform.position + Vector2.up * 1.5f).GetComponent<ParticleSystem>();
		Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
		playBoneAnimation(currentBoneAnimationName.skillName[1]);
	}

	public void divineSmash()
	{
		if (!m_isReinforcementDivineSmash)
		{
			strikeEffect = ObjectPool.Spawn("@ExplosionEffect", Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position), new Vector3(90f, 0f, 0f));
		}
		else
		{
			Vector2 v = base.cachedTransform.position + new Vector3(0.2f * getDirection(), 2.4f, 0f);
			strikeEffect = ObjectPool.Spawn("@ReinforcementDivineSmashEffect", v, new Vector3(90f, 0f, 0f), base.cachedTransform.localScale);
		}
		GameObject gameObject = null;
		if (!m_isReinforcementDivineSmash)
		{
			gameObject = ObjectPool.Spawn("@WarriorAttackEffect3", (Vector2)base.cachedTransform.position + new Vector2(getDirection() * 0.81f, 0.77f));
			gameObject.transform.localScale = new Vector2(getDirection(), 1f);
		}
		Singleton<AudioManager>.instance.playEffectSound("skill_smash", AudioManager.EffectType.Skill);
		if (isCastedDivineSmashFromPreview)
		{
			foreach (Transform item in strikeEffect.transform)
			{
				item.gameObject.layer = LayerMask.NameToLayer("PreviewLayer");
			}
			strikeEffect.layer = LayerMask.NameToLayer("PreviewLayer");
			if (gameObject != null)
			{
				foreach (Transform item2 in gameObject.transform)
				{
					item2.gameObject.layer = LayerMask.NameToLayer("PreviewLayer");
				}
				gameObject.layer = LayerMask.NameToLayer("PreviewLayer");
			}
		}
		else
		{
			foreach (Transform item3 in strikeEffect.transform)
			{
				item3.gameObject.layer = LayerMask.NameToLayer("Player");
			}
			strikeEffect.layer = LayerMask.NameToLayer("Player");
			if (gameObject != null)
			{
				foreach (Transform item4 in gameObject.transform)
				{
					item4.gameObject.layer = LayerMask.NameToLayer("Player");
				}
				gameObject.layer = LayerMask.NameToLayer("Player");
			}
		}
		Singleton<CachedManager>.instance.ingameCameraShake.shake(2f, 0.1f);
		if (isCastedDivineSmashFromPreview)
		{
			StartCoroutine("waitSkill");
		}
		else
		{
			for (int i = 0; i < Singleton<CharacterManager>.instance.characterList.Count; i++)
			{
				if (Singleton<CharacterManager>.instance.characterList[i].currentCharacterType != 0)
				{
					Singleton<CharacterManager>.instance.characterList[i].setState(PublicDataManager.State.Idle);
					Singleton<CharacterManager>.instance.characterList[i].isAttacking = false;
				}
			}
			isCastingSkill = false;
			float range = ((!m_isReinforcementDivineSmash) ? 3f : 4.5f);
			List<EnemyObject> nearestEnemies = Singleton<EnemyManager>.instance.getNearestEnemies(base.cachedTransform.position, range);
			double num = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DivineSmash, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DivineSmash).skillLevel);
			for (int j = 0; j < nearestEnemies.Count; j++)
			{
				if (nearestEnemies[j].isBoss || (nearestEnemies[j].myMonsterObject != null && nearestEnemies[j].myMonsterObject.isMiniboss))
				{
					num = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DivineSmash, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DivineSmash).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses);
				}
				if (!nearestEnemies[j].isBoss && !nearestEnemies[j].isMiniboss)
				{
					num = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DivineSmash, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DivineSmash).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters);
				}
				if (m_isReinforcementDivineSmash)
				{
					num += num / 100.0 * Singleton<SkillManager>.instance.getReinforcementSkillValue(SkillManager.SkillType.DivineSmash);
				}
				nearestEnemies[j].decreasesHealth(num);
				nearestEnemies[j].setDamageText(num, currentCharacterType, true, true, true);
			}
			Singleton<CachedManager>.instance.darkUI.fadeInGame();
			CameraFit.Instance.setCameraDefault();
		}
		ObjectPool.Recycle("@CastSkill", m_currentCastEffect.gameObject);
		Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType9);
	}

	private IEnumerator waitSkill()
	{
		yield return new WaitForSeconds(2f);
		if (Singleton<SkillManager>.instance.previewType == 0)
		{
			Singleton<SkillManager>.instance.stopSkillPreview(SkillManager.SkillType.DivineSmash);
			UIWindowSkill.instance.OnClickClose();
		}
	}

	protected override void moveInit()
	{
		stopAll();
		Vector2 position = base.cachedTransform.position;
		playBoneAnimation(currentBoneAnimationName.moveName[0]);
		if (!m_isStair)
		{
			currentGround = Singleton<GroundManager>.instance.getStayingGround(position);
			currentStayingGroundIndex = currentGround.currnetFloor;
		}
		if (currentGround == null)
		{
			setState(PublicDataManager.State.Idle);
			return;
		}
		m_isBossGround = currentGround.isBossGround;
		if (m_isStair)
		{
			fixedDirection = false;
			if (m_stairCount < currentGround.stairpoint.Length)
			{
				m_targetHolePosition = currentGround.stairpoint[m_stairCount].position;
			}
			else
			{
				m_targetHolePosition = currentGround.inPoint.position;
				Singleton<SkillManager>.instance.checkChanceForPassiveSkill(SkillManager.PassiveSkillType.FrostSkill, 15f);
				Singleton<SkillManager>.instance.checkChanceForPassiveSkill(SkillManager.PassiveSkillType.MeteorRain, 15f);
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
			if (currentGround.isBossGround && (GameManager.currentStage % Singleton<MapManager>.instance.maxStage == 0 || GameManager.getRealThemeNumber(GameManager.currentTheme) == 10))
			{
				Singleton<SkillManager>.instance.LockSkillButton(true);
			}
		}
		else if (m_isBossGround)
		{
			if (TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType6);
			}
			if (GameManager.currentStage % Singleton<MapManager>.instance.maxStage == 0 || GameManager.getRealThemeNumber(GameManager.currentTheme) == 10)
			{
				if (!isUsedBossStageAction)
				{
					setDontStop(false);
					setStateLock(true);
					Singleton<AudioManager>.instance.stopBackgroundSound();
					UIWindowIngame.instance.StartCoroutine("bossWarningUpdate");
					isUsedBossStageAction = true;
					Singleton<EnemyManager>.instance.startBoss();
					m_targetHolePosition = currentGround.outPoint.position;
					moveTo(m_targetHolePosition, curSpeed, delegate
					{
						setStateLock(false);
						if (GameManager.currentTheme == Singleton<DataManager>.instance.currentGameData.unlockTheme || GameManager.getRealThemeNumber(GameManager.currentTheme) == 10)
						{
							UIWindowIngame.instance.bossWarningState = false;
							setState(PublicDataManager.State.Wait);
							UIWindowIngame.instance.bossWarningState = true;
							if (Singleton<CachedManager>.instance.princess == null)
							{
								Singleton<EnemyManager>.instance.bossObject.setState(PublicDataManager.State.Move);
								Singleton<EnemyManager>.instance.bossObject.moveTo(new Vector2(base.cachedTransform.position.x + 2f * getDirection(), Singleton<EnemyManager>.instance.bossObject.cachedTransform.position.y), Singleton<EnemyManager>.instance.bossObject.curSpeed, delegate
								{
									setStateLock(false);
									UIWindowIngame.instance.bossWarningState = false;
									Singleton<AudioManager>.instance.playBackgroundSound("boss");
									Singleton<SkillManager>.instance.LockSkillButton(false);
									isCanAttackBoss = true;
									for (int j = 0; j < Singleton<CharacterManager>.instance.constCharacterList.Count; j++)
									{
										if (Singleton<CharacterManager>.instance.constCharacterList[j] != null)
										{
											Singleton<CharacterManager>.instance.constCharacterList[j].setState(PublicDataManager.State.Move);
										}
									}
									((BossObject)Singleton<EnemyManager>.instance.bossObject).isCanAttack = true;
									Singleton<EnemyManager>.instance.bossObject.setState(PublicDataManager.State.Idle);
								});
							}
							else
							{
								Singleton<CachedManager>.instance.princess.jump(Vector2.up * 2f, Singleton<CachedManager>.instance.princess.cachedTransform.position.y, delegate
								{
									jumpWithFollowingCharacter(Vector2.up * 2f, delegate
									{
										setStateLock(false);
										Transform transform = ObjectPool.Spawn("@Emotion", Vector2.zero, base.cachedTransform).transform;
										transform.position = (Vector2)base.cachedTransform.position + new Vector2(1f, 2f);
										transform.localScale = new Vector2(-4f, 4f);
										Singleton<CachedManager>.instance.emotionUI = transform.GetComponent<EmotionUI>();
										Singleton<EnemyManager>.instance.bossObject.setState(PublicDataManager.State.Move);
										Singleton<EnemyManager>.instance.bossObject.moveTo(new Vector2(base.cachedTransform.position.x + 2f * getDirection(), Singleton<EnemyManager>.instance.bossObject.cachedTransform.position.y), Singleton<EnemyManager>.instance.bossObject.curSpeed, delegate
										{
											UIWindowIngame.instance.bossWarningState = false;
											Singleton<AudioManager>.instance.playBackgroundSound("boss");
											Singleton<SkillManager>.instance.LockSkillButton(false);
											isCanAttackBoss = true;
											for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
											{
												if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
												{
													Singleton<CharacterManager>.instance.constCharacterList[i].setStateLock(false);
													Singleton<CharacterManager>.instance.constCharacterList[i].setState(PublicDataManager.State.Move);
												}
											}
											((BossObject)Singleton<EnemyManager>.instance.bossObject).isCanAttack = true;
											Singleton<EnemyManager>.instance.bossObject.setState(PublicDataManager.State.Idle);
										});
									});
								});
							}
						}
					});
				}
			}
			else if (Singleton<EnemyManager>.instance.bossObject != null && !Singleton<EnemyManager>.instance.bossObject.isDead)
			{
				moveTo(new Vector2(Singleton<EnemyManager>.instance.bossObject.cachedTransform.position.x, base.cachedTransform.position.y), curSpeed);
			}
			if (isCanAttackBoss)
			{
				if (Singleton<EnemyManager>.instance.bossObject != null && !Singleton<EnemyManager>.instance.bossObject.isDead)
				{
					setDontStop(true);
					moveTo(new Vector2(Singleton<EnemyManager>.instance.bossObject.cachedTransform.position.x, base.cachedTransform.position.y), curSpeed);
				}
				else
				{
					setState(PublicDataManager.State.Wait);
				}
			}
		}
		else if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon)
		{
			if (!m_isFirstGround)
			{
				Ground nextGround = Singleton<GroundManager>.instance.getNextGround(currentGround);
				int num = ((MovingObject.calculateDirection(currentGround.inPoint.position, currentGround.outPoint.position) != 0) ? 1 : (-1));
				if (nextGround.isBossGround && nextGround.stairpoint.Length == 1)
				{
					m_targetHolePosition = (Vector2)currentGround.outPoint.position + new Vector2(-num, 0f);
				}
				else
				{
					m_targetHolePosition = currentGround.outPoint.position;
				}
				moveTo(m_targetHolePosition, curSpeed, delegate
				{
					if (TutorialManager.isTutorial)
					{
						Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType38);
					}
					m_isStair = true;
					m_stairCount = 0;
					position = base.cachedTransform.position;
					Singleton<QuestManager>.instance.questReport(QuestManager.QuestType.Floor, 1.0);
					Singleton<GroundManager>.instance.increaseFloor();
					goToNextGround();
					Singleton<MapManager>.instance.createGroundBelow(Singleton<GroundManager>.instance.getCreateFloor());
					if (position.y < -25.4f)
					{
						Singleton<MapManager>.instance.deleteGroundAbove();
					}
				});
				return;
			}
			m_targetHolePosition = currentGround.downPoint.position;
			moveTo(m_targetHolePosition, curSpeed, delegate
			{
				position = base.cachedTransform.position;
				m_targetHolePosition = Singleton<GroundManager>.instance.getNextGround(currentGround).inPoint.position;
				currentGround = Singleton<GroundManager>.instance.getStayingGround(base.cachedTransform.position);
				Singleton<GroundManager>.instance.increaseFloor();
				Singleton<MapManager>.instance.createGroundBelow(Singleton<GroundManager>.instance.getCreateFloor());
				jump(new Vector2(1.5f, 4f), m_targetHolePosition.y, delegate
				{
					setStateLock(false);
					setDirection(Direction.Left);
					isCanCastSkill = true;
					position = base.cachedTransform.position;
					currentGround = Singleton<GroundManager>.instance.getStayingGround(position);
					currentStayingGroundIndex = currentGround.currnetFloor;
					m_targetHolePosition = currentGround.outPoint.position;
					isFirstJump = false;
					m_isFirstGround = false;
					setState(PublicDataManager.State.Move);
					Singleton<FeverManager>.instance.isStart = true;
					Singleton<SkillManager>.instance.checkChanceForPassiveSkill(SkillManager.PassiveSkillType.FrostSkill, 15f);
					Singleton<SkillManager>.instance.checkChanceForPassiveSkill(SkillManager.PassiveSkillType.MeteorRain, 15f);
				});
			});
		}
		else if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			setDontStop(true);
			moveTo(Singleton<CachedManager>.instance.bossRaidFollowTargetTransform, curSpeed);
		}
	}

	protected override IEnumerator Move()
	{
		moveInit();
		while (true)
		{
			if (!GameManager.isPause)
			{
				curSpeed = baseSpeed + baseSpeed / 100f * feverPercentage;
				curSpeed = Mathf.Clamp(curSpeed, baseSpeed + baseSpeed / 100f * feverPercentage, (baseSpeed + baseSpeed / 100f * feverPercentage) * 2f);
				curSpeed += curSpeed / 100f * (Singleton<StatManager>.instance.percentMovementSpeed * 0.01f * 60f);
				setSpeed(curSpeed);
				Vector3 position = base.cachedTransform.position;
				targetEnemy = Singleton<EnemyManager>.instance.getNearestEnemyforWarrior(position);
				if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid || (!m_isFirstGround && !m_isStair))
				{
					for (int k = 0; k < Singleton<CharacterManager>.instance.characterList.Count; k++)
					{
						Singleton<CharacterManager>.instance.characterList[k].targetEnemy = targetEnemy;
					}
					if (targetEnemy != null)
					{
						if (BossRaidManager.isBossRaid)
						{
							if (targetEnemy.isBoss || targetEnemy.isMiniboss || targetEnemy.myBossObjcect != null)
							{
								Singleton<CachedManager>.instance.bossInformation.setProperties(targetEnemy);
							}
							else
							{
								for (int j = 0; j < Singleton<CachedManager>.instance.enemyInformations.Length && !Singleton<CachedManager>.instance.enemyInformations[j].setProperties(targetEnemy); j++)
								{
								}
							}
						}
						else
						{
							for (int i = 0; i < Singleton<CachedManager>.instance.enemyInformations.Length && !Singleton<CachedManager>.instance.enemyInformations[i].setProperties(targetEnemy); i++)
							{
							}
						}
						setState(PublicDataManager.State.Idle);
					}
				}
				else
				{
					position = base.cachedTransform.position;
				}
				if (isStair() && !isMoving())
				{
					break;
				}
			}
			yield return null;
		}
		setState(PublicDataManager.State.Move);
	}

	protected override void waitEvent()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
		if (currentFollower != null)
		{
			currentFollower.setState(PublicDataManager.State.Wait);
		}
		stopAll();
	}

	public override void idleInit()
	{
		if (targetEnemy == null || (targetEnemy != null && targetEnemy.isDead))
		{
			targetEnemy = Singleton<EnemyManager>.instance.getNearestEnemyforWarrior(base.cachedTransform.position);
		}
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
	}

	protected override IEnumerator Idle()
	{
		idleInit();
		while (true)
		{
			targetEnemy = Singleton<EnemyManager>.instance.getNearestEnemyforWarrior(base.cachedTransform.position);
			if (targetEnemy == null || (targetEnemy != null && targetEnemy.isDead))
			{
				break;
			}
			yield return null;
		}
		targetEnemy = null;
		setState(PublicDataManager.State.Move);
	}

	protected override void dieAfterJump()
	{
		if (BossRaidManager.isBossRaid && Singleton<BossRaidManager>.instance.isDoubleSpeed)
		{
			Singleton<GameManager>.instance.setTimeScale(true);
		}
		if (Singleton<StatManager>.instance.revivePercentHealth != 0.0 && !m_isRevived)
		{
			m_isRevived = true;
			revive();
			return;
		}
		playBoneAnimation(currentBoneAnimationName.dieName[0]);
		Singleton<AudioManager>.instance.playEffectSound("character_die");
		characterBoneSpriteRendererData.headSpriteRenderer.sprite = Singleton<CharacterManager>.instance.getWarriorHeadDieSprite(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin);
		StartCoroutine("waitForResult");
	}

	private IEnumerator waitForResult()
	{
		yield return new WaitForSeconds(1f / GameManager.timeScale);
		Singleton<AudioManager>.instance.stopBackgroundSound();
		if (TutorialManager.isTutorial)
		{
			Singleton<TutorialManager>.instance.dieWhenPlayingTutorial();
		}
		else if (BossRaidManager.isBossRaid)
		{
			UIWindowReviveDialog.instance.openReviveDialog(delegate
			{
				UIWindowResult.instance.ResultGame(false, false);
			});
		}
		else
		{
			UIWindowResult.instance.ResultGame(false, false);
		}
	}

	public void revive()
	{
		StartCoroutine("waitForRevive");
	}

	private IEnumerator waitForRevive()
	{
		Singleton<CachedManager>.instance.darkUI.fadeOut();
		CameraFit.Instance.setCameraSize(0.75f, base.cachedTransform);
		yield return new WaitForSeconds(0.2f);
		ObjectPool.Spawn("fx_skill_hero_priest3", (Vector2)Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.position + new Vector2(0f, 1.855f), base.cachedTransform, true);
		while (GameManager.isPause)
		{
			yield return null;
		}
		ObjectPool.Spawn("fx_skill_hero_priest3_2", new Vector2(0f, 0.5f), base.cachedTransform);
		Singleton<CachedManager>.instance.darkUI.fadeInGame();
		CameraFit.Instance.setCameraDefault();
		double reviveHp2 = Singleton<CharacterManager>.instance.warriorCharacter.maxHealth / 100.0 * Singleton<StatManager>.instance.revivePercentHealth;
		reviveHp2 = Math.Min(reviveHp2, Singleton<CharacterManager>.instance.warriorCharacter.maxHealth);
		increasesHealth(reviveHp2);
		hpGauge.refreshLerpHP();
		if (TutorialManager.isTutorial)
		{
			Singleton<StatManager>.instance.revivePercentHealth = 0.0;
		}
		for (int i = 0; i < Singleton<CharacterManager>.instance.characterList.Count; i++)
		{
			Singleton<CharacterManager>.instance.characterList[i].isDead = false;
			if ((!BossRaidManager.isBossRaid || !(targetEnemy != null) || !targetEnemy.isBoss || !targetEnemy.isDead) && !Singleton<CharacterManager>.instance.characterList[i].isFirstGround())
			{
				Singleton<CharacterManager>.instance.characterList[i].setStateLock(false);
				Singleton<CharacterManager>.instance.characterList[i].setState(PublicDataManager.State.Move);
				Singleton<CharacterManager>.instance.characterList[i].isAttacking = false;
			}
		}
		characterBoneSpriteRendererData.headSpriteRenderer.sprite = Singleton<CharacterManager>.instance.getWarriorHeadSprite(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin);
		if (BossRaidManager.isBossRaid && Singleton<BossRaidManager>.instance.isDoubleSpeed)
		{
			Singleton<GameManager>.instance.setTimeScale(true);
		}
	}

	protected override void attack()
	{
		if (targetEnemy != null && m_isEquippedWeapon)
		{
			WeaponStat weaponRealStats = equippedWeapon.weaponRealStats;
		}
		if (!isAttacking)
		{
			isAttacking = true;
			setStateLock(true);
			m_attackLimitedMaxTimer = 0f;
			m_attackTimer = 0f;
			if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
			{
				WarriorAttackEffectObject component = ObjectPool.Spawn("@WarriorAttackEffect1", new Vector2(0f, 0.0108f), base.cachedTransform).GetComponent<WarriorAttackEffectObject>();
				component.cachedTransform.localScale = new Vector3(-1f, 1f, 1f);
				component.init();
			}
			playBoneAnimation(currentBoneAnimationName.attackName[0]);
			StopCoroutine("attackEndCheckUpdate");
			StartCoroutine("attackEndCheckUpdate");
			if (Singleton<SkillManager>.instance.isSkillLock)
			{
				Singleton<SkillManager>.instance.LockSkillButton(false);
			}
		}
		else
		{
			setState(PublicDataManager.State.Idle);
		}
	}

	public override void attackEnemy()
	{
		if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			WarriorAttackEffectObject component = ObjectPool.Spawn("@WarriorAttackEffect2", new Vector2(0f, 0.0108f), base.cachedTransform).GetComponent<WarriorAttackEffectObject>();
			component.cachedTransform.localScale = new Vector3(-1f, 1f, 1f);
			component.init();
		}
		double num = curDamage * (double)UnityEngine.Random.Range(0.9f, 1.1f);
		float num2 = UnityEngine.Random.Range(0f, 100f);
		bool flag = false;
		if (num2 < curCriticalChance)
		{
			flag = true;
			num *= curCriticalDamage / 100.0;
		}
		else
		{
			flag = false;
		}
		if (targetEnemy != null && !targetEnemy.isInvincible)
		{
			bool flag2 = false;
			if (targetEnemy.isBoss || (targetEnemy.myMonsterObject != null && targetEnemy.myMonsterObject.isMiniboss))
			{
				num = getCurrentDamage(Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
			}
			if (!targetEnemy.isBoss && !targetEnemy.isMiniboss)
			{
				num = getCurrentDamage(Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
			}
			double num3 = Util.Clamp(Singleton<FeverManager>.instance.getCurrentAttackEffectIndex() - 2, 0.0, 3.0) * 10.0;
			bool flag3 = Singleton<FeverManager>.instance.getCurrentAttackEffectIndex() >= 4 && Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.SwordSoul).isUnlocked;
			num += num / 100.0 * num3;
			if (flag3)
			{
				num += num / 100.0 * Singleton<SkillManager>.instance.getPassiveSkillValue(SkillManager.PassiveSkillType.SwordSoul, false);
			}
			if (Singleton<FeverManager>.instance.getCurrentAttackEffectIndex() >= 3)
			{
				float range = 2.3f;
				if (flag3)
				{
					range = 5f;
				}
				List<EnemyObject> nearestEnemies = Singleton<EnemyManager>.instance.getNearestEnemies(base.cachedTransform.position, range);
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
						attackedEnemy = nearestEnemies[j];
						break;
					}
				}
				for (int k = 0; k < nearestEnemies.Count; k++)
				{
					if (nearestEnemies[k] == attackedEnemy)
					{
						nearestEnemies[k].decreasesHealth(num);
						nearestEnemies[k].setDamageText(num, currentCharacterType, flag, flag2, flag2);
					}
					else
					{
						nearestEnemies[k].decreasesHealth(num * (double)((!flag3) ? 0.3f : 1f));
						nearestEnemies[k].setDamageText(num * (double)((!flag3) ? 0.3f : 1f), currentCharacterType, flag, flag2, flag2);
					}
				}
			}
			else
			{
				targetEnemy.decreasesHealth(num);
				targetEnemy.setDamageText(num, currentCharacterType, flag, flag2, flag2);
				attackedEnemy = targetEnemy;
			}
			if (flag && attackedEnemy != null && Singleton<StatManager>.instance.weaponSkinExtraCriticalDamageForWarrior > 0.0)
			{
				double num4 = num / 100.0 * Singleton<StatManager>.instance.weaponSkinExtraCriticalDamageForWarrior;
				attackedEnemy.decreasesHealth(num4);
				attackedEnemy.setPoisonDamageText(num4);
			}
			if (m_lastAttackTargetEnemy == null || m_lastAttackTargetEnemy != attackedEnemy)
			{
				m_attackStackCount = 0;
				m_lastAttackTargetEnemy = attackedEnemy;
				if (Singleton<StatManager>.instance.weaponSkinNewAttackDamageForWarrior > 0.0)
				{
					double num5 = num / 100.0 * Singleton<StatManager>.instance.weaponSkinNewAttackDamageForWarrior;
					attackedEnemy.decreasesHealth(num5);
					attackedEnemy.setPoisonDamageText(num5);
				}
			}
			else if (Singleton<StatManager>.instance.weaponSkinStackAttackDamageForWarrior > 0.0 && m_lastAttackTargetEnemy == attackedEnemy)
			{
				m_attackStackCount = Math.Min(m_attackStackCount + 1, 30);
			}
			else
			{
				m_attackStackCount = 0;
			}
			if (Singleton<SkillManager>.instance.isConcentration)
			{
				Singleton<AudioManager>.instance.playEffectSound("skill_concentration", AudioManager.EffectType.Skill);
				Singleton<CachedManager>.instance.ingameCameraShake.shake(2f, 0.1f);
			}
			else
			{
				Singleton<CachedManager>.instance.ingameCameraShake.shake(1f, 1f);
			}
		}
		if (targetEnemy != null && targetEnemy.isDead)
		{
			targetEnemy = null;
		}
	}

	public override void attackEndEvent()
	{
		isAttacking = false;
		setStateLock(false);
		if (Singleton<EnemyManager>.instance != null && !Singleton<EnemyManager>.instance.isBossDead)
		{
			setState(PublicDataManager.State.Idle);
		}
		else
		{
			setState(PublicDataManager.State.Wait);
		}
		if (Singleton<SkillManager>.instance != null)
		{
			Singleton<SkillManager>.instance.CheckSkillAll();
		}
	}

	public void clickAttack()
	{
		if (m_currentState == PublicDataManager.State.Wait || isDead || m_currentState != 0)
		{
			return;
		}
		if (targetEnemy != null)
		{
			for (int i = 0; i < Singleton<CharacterManager>.instance.characterList.Count; i++)
			{
				if (Singleton<CharacterManager>.instance.characterList[i].isFirstGround())
				{
					continue;
				}
				if (i != 0)
				{
					if (Singleton<CharacterManager>.instance.characterList[i].isCanAttack() && Singleton<CharacterManager>.instance.characterList[i].getState() == PublicDataManager.State.Idle)
					{
						Singleton<CharacterManager>.instance.characterList[i].targetEnemy = targetEnemy;
						Singleton<CharacterManager>.instance.characterList[i].setState(PublicDataManager.State.Attack);
					}
				}
				else if (m_isCanAttack)
				{
					Singleton<CharacterManager>.instance.characterList[i].targetEnemy = targetEnemy;
					Singleton<CharacterManager>.instance.characterList[i].setState(PublicDataManager.State.Attack);
				}
			}
			Singleton<AudioManager>.instance.playEffectSound("touch_attack", AudioManager.EffectType.Touch);
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("touch", AudioManager.EffectType.Touch);
		}
	}

	private bool getEnemyHittable(EnemyObject enemy, Vector2 startPosition)
	{
		float num = float.MaxValue;
		float num2 = 1f;
		if (!enemy.isDead)
		{
			if (Mathf.Abs(enemy.cachedTransform.position.y - startPosition.y) <= 0.5f)
			{
				float num3 = Vector2.Distance(startPosition, enemy.cachedTransform.position);
				if (num3 <= num)
				{
					num = num3;
					if (enemy is MonsterObject)
					{
						num2 = ((!((MonsterObject)enemy).isMiniboss) ? 1.2f : 1.8f);
						if (num <= num2)
						{
							return true;
						}
						return false;
					}
					if (enemy is SpecialObject)
					{
						num2 = 1.2f;
						if (num <= num2)
						{
							return true;
						}
						return false;
					}
					num2 = 2f;
					if (num <= num2)
					{
						return true;
					}
					return false;
				}
			}
			return false;
		}
		return false;
	}
}
