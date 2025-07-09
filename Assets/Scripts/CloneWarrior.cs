using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneWarrior : MovingObject
{
	public enum CloneWarriorState
	{
		FollowWarrior,
		Attack,
		Null
	}

	public CloneWarriorState currentCloneWarriorState;

	public Animation cachedBoneAnimation;

	public Transform weaponParentTransform;

	public Weapon weapon;

	public CharacterManager.CharacterBoneAnimationSpriteRendererData characterBoneSpriteRendererData;

	private CharacterWarrior mainBody;

	private bool m_isInit;

	private WarriorAttackEffectObject m_warriorAttackEffect1;

	private WarriorAttackEffectObject m_warriorAttackEffect2;

	private string m_currentAnimationName;

	private bool m_isPlayedConfusionEffect;

	private bool m_isReinforcementSkill;

	private IEnumerator animationSpeedUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				for (int j = 0; j < mainBody.currentBoneAnimationName.attackName.Length; j++)
				{
					cachedBoneAnimation[mainBody.currentBoneAnimationName.attackName[j]].speed = 1f / Mathf.Max(mainBody.curDelay, 0.2f) / 4f * GameManager.timeScale;
				}
			}
			else
			{
				for (int i = 0; i < mainBody.currentBoneAnimationName.attackName.Length; i++)
				{
					cachedBoneAnimation[mainBody.currentBoneAnimationName.attackName[i]].speed = 0f;
				}
			}
			yield return null;
		}
	}

	public void init(bool isReinforcementSkill = false)
	{
		m_isPlayedConfusionEffect = false;
		m_isReinforcementSkill = isReinforcementSkill;
		StopAllCoroutines();
		mainBody = Singleton<CharacterManager>.instance.warriorCharacter;
		removeAllAnimations();
		List<AnimationClip> list = null;
		list = ((mainBody.skinType != CharacterSkinManager.WarriorSkinType.Valkyrie1) ? Singleton<AnimationManager>.instance.warriorNormalAnimationClipList : Singleton<AnimationManager>.instance.warriorValkyrieAnimationClipList);
		for (int i = 0; i < list.Count; i++)
		{
			cachedBoneAnimation.AddClip(list[i], list[i].name);
		}
		m_currentAnimationName = string.Empty;
		if (m_warriorAttackEffect1 != null)
		{
			ObjectPool.Recycle(m_warriorAttackEffect1.name, m_warriorAttackEffect1.cachedGameObject);
		}
		if (m_warriorAttackEffect2 != null)
		{
			ObjectPool.Recycle(m_warriorAttackEffect2.name, m_warriorAttackEffect2.cachedGameObject);
		}
		if (weapon != null)
		{
			ObjectPool.Recycle(weapon.name, weapon.cachedGameObject);
		}
		weapon = null;
		m_warriorAttackEffect1 = null;
		m_warriorAttackEffect2 = null;
		currentCloneWarriorState = CloneWarriorState.Null;
		base.cachedTransform.position = mainBody.cachedTransform.position;
		Singleton<CharacterManager>.instance.changeCharacterSkin(mainBody.skinType, characterBoneSpriteRendererData);
		weapon = ObjectPool.Spawn("@Weapon", Vector3.zero, weaponParentTransform).GetComponent<Weapon>();
		weapon.weaponCharacterType = CharacterManager.CharacterType.Warrior;
		weapon.warriorWeaponType = mainBody.equippedWeapon.warriorWeaponType;
		weapon.initWeapon(true);
		weapon.refreshWeaponSkin();
		base.cachedTransform.localScale = new Vector2(mainBody.getDirection() * 4f, 4f);
		weapon.cachedTransform.localScale = Vector3.one;
		weapon.cachedTransform.localEulerAngles = Vector3.zero;
		StartCoroutine("animationSpeedUpdate");
		AnimationClip clip = cachedBoneAnimation.GetClip(mainBody.currentBoneAnimationName.attackName[0]);
		cachedBoneAnimation.RemoveClip(mainBody.currentBoneAnimationName.attackName[0]);
		AnimationClip clip2 = Object.Instantiate(clip);
		cachedBoneAnimation.AddClip(clip2, mainBody.currentBoneAnimationName.attackName[0]);
		cachedBoneAnimation[mainBody.currentBoneAnimationName.attackName[0]].wrapMode = WrapMode.Loop;
		moveTo(new Vector2(-0.4f, 0f) + (Vector2)mainBody.cachedTransform.position, 10f, delegate
		{
			m_isInit = true;
			setCloneWarriorState(CloneWarriorState.FollowWarrior);
		});
	}

	private void removeAllAnimations()
	{
		List<string> list = new List<string>();
		foreach (AnimationState item in cachedBoneAnimation)
		{
			list.Add(item.name);
		}
		for (int i = 0; i < list.Count; i++)
		{
			cachedBoneAnimation.RemoveClip(list[i]);
		}
	}

	public void setCloneWarriorState(CloneWarriorState state, bool force = false)
	{
		if (currentCloneWarriorState == state && !force)
		{
			return;
		}
		currentCloneWarriorState = state;
		switch (currentCloneWarriorState)
		{
		case CloneWarriorState.FollowWarrior:
			base.cachedTransform.SetParent(mainBody.cachedTransform);
			moveTo(new Vector2(0f, 0f) + (Vector2)mainBody.cachedTransform.position, 25f, delegate
			{
				base.cachedTransform.localScale = new Vector2(0f, 0f);
			});
			if (m_currentAnimationName != mainBody.currentBoneAnimationName.moveName[0])
			{
				m_currentAnimationName = mainBody.currentBoneAnimationName.moveName[0];
				cachedBoneAnimation.Stop();
				cachedBoneAnimation.Play(mainBody.currentBoneAnimationName.moveName[0]);
			}
			break;
		case CloneWarriorState.Attack:
			if (mainBody.targetEnemy == null)
			{
				setCloneWarriorState(CloneWarriorState.FollowWarrior);
				break;
			}
			base.cachedTransform.localScale = new Vector2(mainBody.getDirection() * 4f, 4f);
			base.cachedTransform.SetParent(null);
			moveTo(new Vector2((0f - mainBody.getDirection()) * -1.3f + mainBody.targetEnemy.cachedTransform.position.x, mainBody.targetEnemy.cachedTransform.position.y), 2000f, delegate
			{
				if (m_isReinforcementSkill && !m_isPlayedConfusionEffect)
				{
					List<Ground> list = new List<Ground>
					{
						mainBody.currentGround
					};
					Ground nextGround = Singleton<GroundManager>.instance.getNextGround(mainBody.currentGround);
					while (nextGround != null && list.Count < 1 + (int)Singleton<StatManager>.instance.reinforcementConfusionExtraFloor)
					{
						list.Add(nextGround);
						nextGround = Singleton<GroundManager>.instance.getNextGround(nextGround);
					}
					for (int i = 0; i < list.Count; i++)
					{
						ObjectPool.Spawn("@ReinforcementCloneWarriorConfusionBombEffect", list[i].cachedTransform.position, new Vector3(-90f, 0f, 0f));
						for (int j = 0; j < Singleton<EnemyManager>.instance.enemyList.Count; j++)
						{
							EnemyObject enemyObject = Singleton<EnemyManager>.instance.enemyList[j];
							if (enemyObject.currentGround == list[i])
							{
								m_isPlayedConfusionEffect = true;
								enemyObject.setConfusion(8f);
							}
						}
					}
				}
				base.cachedTransform.localScale = new Vector2(mainBody.getDirection() * 4f, 4f);
				if (m_currentAnimationName != mainBody.currentBoneAnimationName.attackName[0])
				{
					m_currentAnimationName = mainBody.currentBoneAnimationName.attackName[0];
					cachedBoneAnimation.Stop();
					cachedBoneAnimation.Play(mainBody.currentBoneAnimationName.attackName[0]);
				}
			});
			break;
		}
	}

	public void attackEnemy()
	{
		if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			m_warriorAttackEffect1 = ObjectPool.Spawn("@WarriorAttackEffect1", Vector3.zero, base.cachedTransform).GetComponent<WarriorAttackEffectObject>();
			m_warriorAttackEffect1.cachedTransform.localScale = new Vector3(0.25f, 0.25f, 1f);
			m_warriorAttackEffect1.init();
			m_warriorAttackEffect2 = ObjectPool.Spawn("@WarriorAttackEffect2", Vector2.zero, base.cachedTransform).GetComponent<WarriorAttackEffectObject>();
			m_warriorAttackEffect2.cachedTransform.localScale = new Vector3(0.25f, 0.25f, 1f);
			m_warriorAttackEffect2.init();
		}
		Singleton<AudioManager>.instance.playEffectSound("skill_clone", AudioManager.EffectType.Skill);
		if (!Singleton<SkillManager>.instance.isCastedSkillFromPreview && mainBody.targetEnemy != null && !mainBody.targetEnemy.isInvincible)
		{
			double skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.ClonedWarrior, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.ClonedWarrior).skillLevel);
			if (mainBody.targetEnemy.isBoss || (mainBody.targetEnemy.myMonsterObject != null && mainBody.targetEnemy.myMonsterObject.isMiniboss))
			{
				skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.ClonedWarrior, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.ClonedWarrior).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses);
			}
			if (!mainBody.targetEnemy.isBoss && !mainBody.targetEnemy.isMiniboss)
			{
				skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.ClonedWarrior, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.ClonedWarrior).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters);
			}
			mainBody.targetEnemy.decreasesHealth(skillValue);
			mainBody.targetEnemy.setDamageText(skillValue, CharacterManager.CharacterType.Warrior);
		}
	}

	public void attackEndEvent()
	{
		m_currentAnimationName = string.Empty;
	}

	private void Update()
	{
		if (GameManager.isPause)
		{
			return;
		}
		if (!Singleton<SkillManager>.instance.isClonedWarrior)
		{
			ObjectPool.Recycle(weapon.name, weapon.cachedGameObject);
			ObjectPool.Recycle(base.cachedGameObject.name, base.cachedGameObject);
		}
		else if (mainBody != null && !mainBody.isDead && m_isInit)
		{
			if (Singleton<EnemyManager>.instance.bossObject != null && Singleton<EnemyManager>.instance.bossObject.isDead)
			{
				if (!BossRaidManager.isBossRaid)
				{
					Singleton<SkillManager>.instance.StopCoroutine("ClonedWarrior");
					Singleton<SkillManager>.instance.isClonedWarrior = false;
					UIWindowIngame.instance.closeSkillCooltimeSlot(SkillManager.SkillType.ClonedWarrior);
					Singleton<SkillManager>.instance.CheckSkillAll();
				}
				else
				{
					setCloneWarriorState(CloneWarriorState.FollowWarrior, true);
				}
			}
			if (mainBody.targetEnemy != null && !mainBody.targetEnemy.isDead)
			{
				setCloneWarriorState(CloneWarriorState.Attack);
			}
			else
			{
				setCloneWarriorState(CloneWarriorState.FollowWarrior);
			}
		}
		else if (mainBody != null && mainBody.isDead)
		{
			ObjectPool.Recycle(weapon.name, weapon.cachedGameObject);
			ObjectPool.Recycle("@ClonedWarrior", base.cachedGameObject);
			Singleton<SkillManager>.instance.isClonedWarrior = false;
		}
	}
}
