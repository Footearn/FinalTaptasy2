using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterArcher : CharacterObject
{
	public CharacterSkinManager.ArcherSkinType skinType;

	public override void resetProperties(bool changeAnimation = true)
	{
		m_currentAnimationName = string.Empty;
		if (changeAnimation)
		{
			removeAllAnimations();
			List<AnimationClip> list = null;
			list = ((skinType != CharacterSkinManager.ArcherSkinType.Valkyrie3) ? Singleton<AnimationManager>.instance.archerNormalAnimationClipList : Singleton<AnimationManager>.instance.archerValkyrieAnimationClipList);
			for (int i = 0; i < list.Count; i++)
			{
				characterBoneAnimation.AddClip(list[i], list[i].name);
			}
			playBoneAnimation(currentBoneAnimationName.idleName[0]);
		}
		base.resetProperties(changeAnimation);
	}

	protected override void checkExtraCharacterStat()
	{
		m_extraDamageValue = Singleton<StatManager>.instance.archerSkinPercentDamage;
		baseCriticalChance += Singleton<StatManager>.instance.archerSkinCriticalChance;
	}

	protected override void attack()
	{
		if (currentGround != null && myLeaderCharacter != null && myLeaderCharacter.currentGround != null)
		{
			if (m_isCanAttack && targetEnemy != null && !isAttacking)
			{
				setStateLock(true);
				m_attackLimitedMaxTimer = 0f;
				m_attackTargetPosition = (Vector2)targetEnemy.cachedTransform.position + new Vector2(0f, -0.1f);
				m_attackTimer = 0f;
				m_isCanAttack = false;
				isAttacking = true;
				setDirection(MovingObject.calculateDirection(base.cachedTransform.position, targetEnemy.cachedTransform.position));
				playBoneAnimation(currentBoneAnimationName.attackName[0]);
				StopCoroutine("attackEndCheckUpdate");
				StartCoroutine("attackEndCheckUpdate");
			}
			else
			{
				setState(PublicDataManager.State.Idle);
			}
		}
	}

	public override void attackEndEvent()
	{
		setStateLock(false);
		setState(PublicDataManager.State.Idle);
		isAttacking = false;
	}

	public override void attackEnemy()
	{
		double realDamage = curDamage * (double)UnityEngine.Random.Range(0.9f, 1.1f);
		EnemyObject targetEnemy = base.targetEnemy;
		if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			Bullet component = ObjectPool.Spawn("@NormalArrow", shootPoint.position).GetComponent<Bullet>();
			if (Singleton<DataManager>.instance.currentGameData.currentTranscendTier[CharacterManager.CharacterType.Archer] > 0)
			{
				component.targetRenderer.sprite = Singleton<CharacterManager>.instance.archerTranscendBulletSprite;
			}
			else
			{
				component.targetRenderer.sprite = Singleton<CharacterManager>.instance.archerNormalBulletSprite;
			}
			component.setParabolic(true);
			component.setParabolic(false);
			component.shootBullet(m_attackTargetPosition, delegate(EnemyObject realTargetEnemy)
			{
				if (realTargetEnemy != null && !realTargetEnemy.isDead)
				{
					if (m_isEquippedWeapon)
					{
						WeaponStat weaponRealStats2 = equippedWeapon.weaponRealStats;
					}
					float num2 = UnityEngine.Random.Range(0f, 100f);
					if (realTargetEnemy.isBoss || (realTargetEnemy.myMonsterObject != null && realTargetEnemy.myMonsterObject.isMiniboss))
					{
						realDamage = getCurrentDamage(Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					}
					if (!realTargetEnemy.isBoss && !realTargetEnemy.isMiniboss)
					{
						realDamage = getCurrentDamage(Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
					}
					bool flag = false;
					if (num2 < curCriticalChance)
					{
						flag = true;
						realDamage *= curCriticalDamage / 100.0;
					}
					else
					{
						flag = false;
					}
					realTargetEnemy.decreasesHealth(realDamage);
					realTargetEnemy.setDamageText(realDamage, currentCharacterType, flag);
					if (flag && realTargetEnemy != null && Singleton<StatManager>.instance.weaponSkinExtraCriticalDamageForArcher > 0.0)
					{
						double num3 = realDamage / 100.0 * Singleton<StatManager>.instance.weaponSkinExtraCriticalDamageForArcher;
						realTargetEnemy.decreasesHealth(num3);
						realTargetEnemy.setPoisonDamageText(num3);
					}
					if (m_lastAttackTargetEnemy == null || m_lastAttackTargetEnemy != realTargetEnemy)
					{
						m_attackStackCount = 0;
						m_lastAttackTargetEnemy = realTargetEnemy;
						if (Singleton<StatManager>.instance.weaponSkinNewAttackDamageForWarrior > 0.0)
						{
							double num4 = realDamage / 100.0 * Singleton<StatManager>.instance.weaponSkinNewAttackDamageForArcher;
							realTargetEnemy.decreasesHealth(num4);
							realTargetEnemy.setPoisonDamageText(num4);
						}
					}
					else if (Singleton<StatManager>.instance.weaponSkinStackAttackDamageForArcher > 0.0 && m_lastAttackTargetEnemy == realTargetEnemy)
					{
						m_attackStackCount = Math.Min(m_attackStackCount + 1, 30);
					}
					else
					{
						m_attackStackCount = 0;
					}
				}
			}, new Vector2(0f, 0.45f), base.targetEnemy);
		}
		else if (targetEnemy != null && !targetEnemy.isDead)
		{
			if (m_isEquippedWeapon)
			{
				WeaponStat weaponRealStats = equippedWeapon.weaponRealStats;
			}
			float num = UnityEngine.Random.Range(0f, 100f);
			if (targetEnemy.isBoss || (targetEnemy.myMonsterObject != null && targetEnemy.myMonsterObject.isMiniboss))
			{
				realDamage = getCurrentDamage(Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
			}
			if (!targetEnemy.isBoss && !targetEnemy.isMiniboss)
			{
				realDamage = getCurrentDamage(Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters) * (double)UnityEngine.Random.Range(0.9f, 1.1f);
			}
			if (num < curCriticalChance)
			{
				realDamage *= curCriticalDamage / 100.0;
			}
			targetEnemy.decreasesHealth(realDamage);
		}
	}
}
