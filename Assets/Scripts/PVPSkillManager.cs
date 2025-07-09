using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPSkillManager : Singleton<PVPSkillManager>
{
	[Serializable]
	public class PVPSkillTypeData
	{
		public MeleeSkillType currentMeleeSkillType = MeleeSkillType.None;

		public MiddleSkillType currentMiddleSkillType = MiddleSkillType.None;

		public RangedSkillType currentRangedSkillType = RangedSkillType.None;

		public PVPSkillTypeData(MeleeSkillType meleeSkillType)
		{
			currentMeleeSkillType = meleeSkillType;
			currentMiddleSkillType = MiddleSkillType.None;
			currentRangedSkillType = RangedSkillType.None;
		}

		public PVPSkillTypeData(MiddleSkillType middleSkillType)
		{
			currentMeleeSkillType = MeleeSkillType.None;
			currentMiddleSkillType = middleSkillType;
			currentRangedSkillType = RangedSkillType.None;
		}

		public PVPSkillTypeData(RangedSkillType rangedSkillType)
		{
			currentMeleeSkillType = MeleeSkillType.None;
			currentMiddleSkillType = MiddleSkillType.None;
			currentRangedSkillType = rangedSkillType;
		}

		public static implicit operator PVPSkillTypeData(MeleeSkillType meleeSkillType)
		{
			return new PVPSkillTypeData(meleeSkillType);
		}

		public static implicit operator PVPSkillTypeData(MiddleSkillType middleSkillType)
		{
			return new PVPSkillTypeData(middleSkillType);
		}

		public static implicit operator PVPSkillTypeData(RangedSkillType rangedSkillType)
		{
			return new PVPSkillTypeData(rangedSkillType);
		}
	}

	public enum MeleeSkillType
	{
		None = -1,
		DivineSmash,
		DragonSmash,
		Concentration,
		CourageAnthem,
		DragonBreath,
		Length
	}

	public enum MiddleSkillType
	{
		None = -1,
		TargetHeal,
		SplashHeal,
		FireBall,
		Meteor,
		HeavenArmor,
		Length
	}

	public enum RangedSkillType
	{
		None = -1,
		Blow,
		HellArrow,
		PenetrationArrow,
		LastShot,
		RageYell,
		Length
	}

	public static float pvpSkillClickMaxTime = 3.5f;

	public static float baseSkillAppearIntervalTime = 6.5f;

	public static float allyMinSkillAppearXPos = -6.4f;

	public static float enemyMinSkillAppearXPos = 6.4f;

	public static double enemySkillCastSuccessChance = 60.0;

	private Coroutine m_skillAppearUpdateCoroutine;

	public Sprite[] skillGradeIconSprites;

	public Sprite[] meleeSkillIconSprites;

	public Sprite[] middleSkillIconSprites;

	public Sprite[] rangedSkillIconSprites;

	public void startGame()
	{
		startSkillAppear();
	}

	public void endGame()
	{
		stopSkillAppear();
	}

	private void startSkillAppear()
	{
		stopSkillAppear();
		m_skillAppearUpdateCoroutine = StartCoroutine(skillAppearUpdate());
	}

	private void stopSkillAppear()
	{
		if (m_skillAppearUpdateCoroutine != null)
		{
			StopCoroutine(m_skillAppearUpdateCoroutine);
		}
		m_skillAppearUpdateCoroutine = null;
	}

	private IEnumerator skillAppearUpdate()
	{
		PVPManager.PVPGameData myGameData = Singleton<PVPManager>.instance.convertStringToPVPGameData(Singleton<PVPManager>.instance.myPVPData.player.game_data);
		PVPManager.PVPGameData enemyGameData = Singleton<PVPManager>.instance.convertStringToPVPGameData((!PVPManager.isPlayingPVP) ? Singleton<PVPManager>.instance.enemyUnitData.game_data : Singleton<PVPManager>.instance.myPVPData.player.game_data);
		float allySkillAppearInterval = getCalculatedAppearInterval(myGameData.totalSkinData.Count);
		float timerForAllySkill = 0f;
		bool isCanAppearSkillForAlly = false;
		float enemySkillAppearInterval = getCalculatedAppearInterval(enemyGameData.totalSkinData.Count);
		float timerForEnemySkill = 0f;
		bool isCanAppearSkillForEnemy = false;
		while (PVPManager.isPlayingPVP)
		{
			if (!GameManager.isPause)
			{
				if (isCanAppearSkillForAlly)
				{
					timerForAllySkill += Time.deltaTime * GameManager.timeScale;
					if (timerForAllySkill >= allySkillAppearInterval && openRandomSkillButton())
					{
						timerForAllySkill = 0f;
					}
				}
				else
				{
					isCanAppearSkillForAlly = isCanAppearSkill(true);
				}
				if (isCanAppearSkillForEnemy)
				{
					timerForEnemySkill += Time.deltaTime * GameManager.timeScale;
					if (timerForEnemySkill >= enemySkillAppearInterval && castEnemySkill())
					{
						timerForEnemySkill = 0f;
					}
				}
				else
				{
					isCanAppearSkillForEnemy = isCanAppearSkill(false);
				}
			}
			yield return null;
		}
		for (int j = 0; j < UIWindowPVP.instance.allySkillBigButtons.Length; j++)
		{
			UIWindowPVP.instance.allySkillBigButtons[j].closeSkillButton(true);
		}
		for (int i = 0; i < UIWindowPVP.instance.allySkillButtons.Length; i++)
		{
			UIWindowPVP.instance.allySkillButtons[i].closeSkillButton(true);
		}
	}

	private bool openRandomSkillButton()
	{
		bool result = false;
		PVPUnitObject randomSkillCastingTargetUnit = getRandomSkillCastingTargetUnit(true);
		if (randomSkillCastingTargetUnit != null)
		{
			if (randomSkillCastingTargetUnit.currentColleagueType == ColleagueManager.ColleagueType.Golem || randomSkillCastingTargetUnit.currentColleagueType == ColleagueManager.ColleagueType.Trall)
			{
				for (int i = 0; i < UIWindowPVP.instance.allySkillBigButtons.Length; i++)
				{
					if (!UIWindowPVP.instance.allySkillBigButtons[i].isOpen)
					{
						UIWindowPVP.instance.allySkillBigButtons[i].initSkillButton(randomSkillCastingTargetUnit);
						result = true;
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < UIWindowPVP.instance.allySkillButtons.Length; j++)
				{
					if (!UIWindowPVP.instance.allySkillButtons[j].isOpen)
					{
						UIWindowPVP.instance.allySkillButtons[j].initSkillButton(randomSkillCastingTargetUnit);
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	private bool castEnemySkill()
	{
		bool result = false;
		PVPUnitObject randomSkillCastingTargetUnit = getRandomSkillCastingTargetUnit(false);
		if (randomSkillCastingTargetUnit != null)
		{
			double num = (double)UnityEngine.Random.Range(0, 10000) / 100.0;
			if (num < enemySkillCastSuccessChance)
			{
				castSkill(randomSkillCastingTargetUnit);
			}
			result = true;
		}
		return result;
	}

	private IEnumerator waitForPlaySmashSound()
	{
		yield return new WaitForSeconds(0.2f);
		Singleton<AudioManager>.instance.playEffectSound("skill_smash", AudioManager.EffectType.Skill);
	}

	public void castSkill(PVPUnitObject caster)
	{
		if (caster.isDead)
		{
			return;
		}
		PVPSkillTypeData currentSkillTypeData = caster.currentSkillTypeData;
		bool isAlly = caster.isAlly;
		double skillValue = getSkillValue(currentSkillTypeData);
		float duration = getDuration(currentSkillTypeData);
		double damage2 = caster.currentStatData.damage;
		Vector2 vector = caster.cachedTransform.position;
		if (currentSkillTypeData.currentMeleeSkillType != MeleeSkillType.None)
		{
			switch (currentSkillTypeData.currentMeleeSkillType)
			{
			case MeleeSkillType.DivineSmash:
			case MeleeSkillType.DragonSmash:
			{
				bool flag = currentSkillTypeData.currentMeleeSkillType == MeleeSkillType.DivineSmash;
				string empty = string.Empty;
				empty = ((!flag) ? "@PVPDragonSmashEffect" : "@PVPDivineSmashEffect");
				double value = damage2 / 100.0 * skillValue;
				NewObjectPool.Spawn<GameObject>(empty, vector + ((!flag) ? new Vector2(0.44f, 2.75f) : new Vector2(1.5f, 0f)), (!flag) ? new Vector3(90f, 0f, 0f) : new Vector3(146f, 0f, 0f), new Vector3(isAlly ? 1 : (-1), 1f, 1f));
				List<PVPUnitObject> nearestUnits = Singleton<PVPUnitManager>.instance.getNearestUnits(vector + new Vector2(((!flag) ? 3f : 1.5f) * (float)(isAlly ? 1 : (-1)), 0f), (!flag) ? 3f : 1.5f, isAlly);
				for (int j = 0; j < nearestUnits.Count; j++)
				{
					nearestUnits[j].decreaseHP(value);
				}
				Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
				StartCoroutine(waitForPlaySmashSound());
				break;
			}
			case MeleeSkillType.Concentration:
				if (caster.currentAttackType == PVPUnitManager.AttackType.MeleeSingleAttack || caster.currentAttackType == PVPUnitManager.AttackType.MeleeSplashAttack)
				{
					caster.startRage(duration, skillValue);
				}
				Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
				break;
			case MeleeSkillType.CourageAnthem:
			{
				List<PVPUnitObject> list = null;
				list = ((!isAlly) ? Singleton<PVPUnitManager>.instance.totalEnemyList : Singleton<PVPUnitManager>.instance.totalAllyList);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].currentAttackType == PVPUnitManager.AttackType.MeleeSingleAttack || list[i].currentAttackType == PVPUnitManager.AttackType.MeleeSplashAttack)
					{
						list[i].startRage(duration, skillValue);
					}
				}
				Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
				break;
			}
			case MeleeSkillType.DragonBreath:
			{
				double skillDamage = damage2 / 100.0 * skillValue;
				NewObjectPool.Spawn<PVPDragonBreath>("@PVPDragonBreath", vector).initDragon(isAlly, skillDamage);
				break;
			}
			}
		}
		else if (currentSkillTypeData.currentMiddleSkillType != MiddleSkillType.None)
		{
			switch (currentSkillTypeData.currentMiddleSkillType)
			{
			case MiddleSkillType.TargetHeal:
			{
				List<PVPUnitObject> list4 = new List<PVPUnitObject>();
				List<PVPUnitObject> list5 = null;
				list5 = ((!isAlly) ? Singleton<PVPUnitManager>.instance.totalEnemyList : Singleton<PVPUnitManager>.instance.totalAllyList);
				for (int n = 0; n < list5.Count; n++)
				{
					if (list5[n].currentAttackType == PVPUnitManager.AttackType.MeleeSingleAttack || list5[n].currentAttackType == PVPUnitManager.AttackType.MeleeSplashAttack)
					{
						list4.Add(list5[n]);
					}
				}
				list4.Sort((PVPUnitObject a, PVPUnitObject b) => a.currentHP.CompareTo(b.currentHP));
				if (list4.Count > 0)
				{
					for (int num = 0; num < list4.Count && !((float)num >= duration); num++)
					{
						list4[num].heal(list4[num].maxHP / 100.0 * skillValue);
					}
				}
				else
				{
					caster.heal(caster.maxHP / 100.0 * skillValue);
				}
				Singleton<AudioManager>.instance.playEffectSound("transcend_cast", AudioManager.EffectType.Skill);
				break;
			}
			case MiddleSkillType.SplashHeal:
			{
				List<PVPUnitObject> nearestUnits2 = Singleton<PVPUnitManager>.instance.getNearestUnits(vector, 3.5f, !isAlly);
				for (int l = 0; l < nearestUnits2.Count; l++)
				{
					nearestUnits2[l].heal(nearestUnits2[l].maxHP / 100.0 * skillValue);
				}
				Singleton<AudioManager>.instance.playEffectSound("transcend_cast", AudioManager.EffectType.Skill);
				break;
			}
			case MiddleSkillType.FireBall:
				caster.changeProjectileAttribute(PVPProjectileManager.ProjectileAttributeType.FireBall);
				caster.resetAttackTimer();
				break;
			case MiddleSkillType.Meteor:
			{
				List<PVPUnitObject> list3 = ((!isAlly) ? Singleton<PVPUnitManager>.instance.totalAllyList : Singleton<PVPUnitManager>.instance.totalEnemyList);
				Vector2 zero = Vector2.zero;
				for (int m = 0; m < list3.Count; m++)
				{
					PVPUnitObject pVPUnitObject2 = list3[m];
					zero += (Vector2)pVPUnitObject2.cachedTransform.position;
				}
				Vector2 vector2 = zero / Mathf.Max(list3.Count, 1);
				double damage3 = damage2 / 100.0 * skillValue;
				Vector2 v = vector2 + new Vector2(caster.getDirection() * -13.55f, 11.44f);
				PVPMeteorObject pVPMeteorObject = NewObjectPool.Spawn<PVPMeteorObject>("@PVPMeteor", v);
				pVPMeteorObject.initMeteor(vector2, damage3, isAlly);
				break;
			}
			case MiddleSkillType.HeavenArmor:
			{
				PVPUnitObject pVPUnitObject = null;
				List<PVPUnitObject> list2 = null;
				list2 = ((!isAlly) ? Singleton<PVPUnitManager>.instance.totalEnemyList : Singleton<PVPUnitManager>.instance.totalAllyList);
				for (int k = 0; k < list2.Count; k++)
				{
					if (list2[k].currentAttackType == PVPUnitManager.AttackType.MeleeSingleAttack || list2[k].currentAttackType == PVPUnitManager.AttackType.MeleeSplashAttack)
					{
						pVPUnitObject = list2[k];
						break;
					}
				}
				if (pVPUnitObject != null)
				{
					pVPUnitObject.startHeavenArmor(duration, skillValue);
				}
				else
				{
					caster.startHeavenArmor(duration, skillValue);
				}
				Singleton<AudioManager>.instance.playEffectSound("transcend_cast", AudioManager.EffectType.Skill);
				break;
			}
			}
		}
		else
		{
			if (currentSkillTypeData.currentRangedSkillType == RangedSkillType.None)
			{
				return;
			}
			switch (currentSkillTypeData.currentRangedSkillType)
			{
			case RangedSkillType.Blow:
				caster.changeProjectileAttribute(PVPProjectileManager.ProjectileAttributeType.Blow);
				caster.resetAttackTimer();
				Singleton<AudioManager>.instance.playEffectSound("transcend_cast", AudioManager.EffectType.Skill);
				break;
			case RangedSkillType.HellArrow:
			{
				List<PVPUnitObject> list8 = ((!isAlly) ? Singleton<PVPUnitManager>.instance.totalAllyList : Singleton<PVPUnitManager>.instance.totalEnemyList);
				Vector2 zero2 = Vector2.zero;
				for (int num5 = 0; num5 < list8.Count; num5++)
				{
					PVPUnitObject pVPUnitObject3 = list8[num5];
					zero2 += (Vector2)pVPUnitObject3.cachedTransform.position;
				}
				Vector2 vector3 = zero2 / Mathf.Max(list8.Count);
				double totaldamage = damage2 / 100.0 * skillValue;
				Vector2 v3 = vector3;
				PVPHellArrow pVPHellArrow = NewObjectPool.Spawn<PVPHellArrow>("@PVPHellArrow", v3);
				pVPHellArrow.setDirection(caster.currentDirection);
				pVPHellArrow.initHellArrow(isAlly, totaldamage);
				break;
			}
			case RangedSkillType.PenetrationArrow:
			{
				double damage4 = damage2 / 100.0 * skillValue;
				Vector2 v2 = caster.cachedTransform.position + new Vector3((!isAlly) ? (-15) : 15, 0f);
				PVPPenetrationArrow pVPPenetrationArrow = NewObjectPool.Spawn<PVPPenetrationArrow>("@PVPPenetrationArrow", caster.shootPoint.position);
				pVPPenetrationArrow.initPenetrationArrow(v2, isAlly, damage4);
				Singleton<AudioManager>.instance.playEffectSound("transcend_cast", AudioManager.EffectType.Skill);
				break;
			}
			case RangedSkillType.LastShot:
			{
				double damage = damage2 / 100.0 * skillValue;
				PVPUnitObject targetUnit = null;
				List<PVPUnitObject> list7 = null;
				list7 = ((!isAlly) ? Singleton<PVPUnitManager>.instance.totalAllyList : Singleton<PVPUnitManager>.instance.totalEnemyList);
				double num3 = double.MaxValue;
				for (int num4 = 0; num4 < list7.Count; num4++)
				{
					if (list7[num4].currentHP < num3)
					{
						num3 = list7[num4].currentHP;
						targetUnit = list7[num4];
					}
				}
				if (targetUnit != null)
				{
					NewMovingObject lastShot = NewObjectPool.Spawn<NewMovingObject>("@PVPLastShotArrow", caster.shootPoint.position);
					lastShot.setParabolic(true, 4.5f);
					lastShot.setLookAtTarget(true);
					lastShot.moveTo(targetUnit.cachedTransform, 5f, delegate
					{
						if (targetUnit != null && !targetUnit.isDead)
						{
							targetUnit.decreaseHP(damage);
						}
						NewObjectPool.Recycle(lastShot);
					});
				}
				Singleton<AudioManager>.instance.playEffectSound("transcend_cast", AudioManager.EffectType.Skill);
				break;
			}
			case RangedSkillType.RageYell:
			{
				List<PVPUnitObject> list6 = null;
				list6 = ((!isAlly) ? Singleton<PVPUnitManager>.instance.totalEnemyList : Singleton<PVPUnitManager>.instance.totalAllyList);
				for (int num2 = 0; num2 < list6.Count; num2++)
				{
					if (list6[num2].currentAttackType == PVPUnitManager.AttackType.LongRange)
					{
						list6[num2].startRage(duration, skillValue);
					}
				}
				Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
				break;
			}
			}
		}
	}

	public double getSkillValue(PVPSkillTypeData typeData)
	{
		double result = 0.0;
		if (typeData.currentMeleeSkillType != MeleeSkillType.None)
		{
			switch (typeData.currentMeleeSkillType)
			{
			case MeleeSkillType.DivineSmash:
				result = 300.0;
				break;
			case MeleeSkillType.DragonSmash:
				result = 400.0;
				break;
			case MeleeSkillType.Concentration:
				result = 200.0;
				break;
			case MeleeSkillType.CourageAnthem:
				result = 100.0;
				break;
			case MeleeSkillType.DragonBreath:
				result = 300.0;
				break;
			}
		}
		else if (typeData.currentMiddleSkillType != MiddleSkillType.None)
		{
			switch (typeData.currentMiddleSkillType)
			{
			case MiddleSkillType.TargetHeal:
				result = 100.0;
				break;
			case MiddleSkillType.SplashHeal:
				result = 50.0;
				break;
			case MiddleSkillType.FireBall:
				result = 300.0;
				break;
			case MiddleSkillType.Meteor:
				result = 500.0;
				break;
			case MiddleSkillType.HeavenArmor:
				result = 90.0;
				break;
			}
		}
		else if (typeData.currentRangedSkillType != RangedSkillType.None)
		{
			switch (typeData.currentRangedSkillType)
			{
			case RangedSkillType.Blow:
				result = 700.0;
				break;
			case RangedSkillType.HellArrow:
				result = 50.0;
				break;
			case RangedSkillType.PenetrationArrow:
				result = 250.0;
				break;
			case RangedSkillType.LastShot:
				result = 9999.0;
				break;
			case RangedSkillType.RageYell:
				result = 100.0;
				break;
			}
		}
		return result;
	}

	public float getDuration(PVPSkillTypeData typeData)
	{
		return 5f;
	}

	public Sprite getSkillIconSprite(PVPSkillTypeData skillTypeData)
	{
		Sprite result = null;
		if (skillTypeData.currentMeleeSkillType != MeleeSkillType.None)
		{
			result = meleeSkillIconSprites[(int)skillTypeData.currentMeleeSkillType];
		}
		else if (skillTypeData.currentMiddleSkillType != MiddleSkillType.None)
		{
			result = middleSkillIconSprites[(int)skillTypeData.currentMiddleSkillType];
		}
		else if (skillTypeData.currentRangedSkillType != RangedSkillType.None)
		{
			result = rangedSkillIconSprites[(int)skillTypeData.currentRangedSkillType];
		}
		return result;
	}

	public Sprite getSkillGradeIconSprite(PVPSkillTypeData skillTypeData)
	{
		Sprite sprite = null;
		return skillGradeIconSprites[getSkillGrade(skillTypeData) - 1];
	}

	public float getCalculatedAppearInterval(int skinCount)
	{
		float num = baseSkillAppearIntervalTime;
		num -= num / 150f * (float)skinCount;
		return Mathf.Clamp(num, 2.5f, baseSkillAppearIntervalTime);
	}

	private bool isCanAppearSkill(bool isAlly)
	{
		bool result = false;
		if (isAlly)
		{
			for (int i = 0; i < Singleton<PVPUnitManager>.instance.totalAllyList.Count; i++)
			{
				if (Singleton<PVPUnitManager>.instance.totalAllyList[i].cachedTransform.position.x >= allyMinSkillAppearXPos)
				{
					result = true;
				}
			}
		}
		else
		{
			for (int j = 0; j < Singleton<PVPUnitManager>.instance.totalEnemyList.Count; j++)
			{
				if (Singleton<PVPUnitManager>.instance.totalEnemyList[j].cachedTransform.position.x <= enemyMinSkillAppearXPos)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private PVPUnitObject getRandomSkillCastingTargetUnit(bool isAlly)
	{
		PVPUnitObject result = null;
		List<PVPUnitObject> list = ((!isAlly) ? Singleton<PVPUnitManager>.instance.totalEnemyList : Singleton<PVPUnitManager>.instance.totalAllyList);
		List<PVPUnitObject> list2 = new List<PVPUnitObject>();
		if (list != null && list.Count > 0)
		{
			List<PVPUnitObject> list3 = new List<PVPUnitObject>();
			for (int i = 0; i < list.Count; i++)
			{
				if (isAlly)
				{
					if (list[i].cachedTransform.position.x >= allyMinSkillAppearXPos)
					{
						list3.Add(list[i]);
					}
				}
				else if (list[i].cachedTransform.position.x <= enemyMinSkillAppearXPos)
				{
					list3.Add(list[i]);
				}
			}
			int num = UnityEngine.Random.Range(1, 4);
			if (list3 != null && list3.Count > 0)
			{
				while (list2.Count <= 0)
				{
					num = UnityEngine.Random.Range(1, 4);
					for (int j = 0; j < list3.Count; j++)
					{
						if (getSkillGrade(list3[j].currentSkillTypeData) == num)
						{
							list2.Add(list3[j]);
						}
					}
				}
			}
		}
		if (list2 != null && list2.Count > 0)
		{
			result = list2[UnityEngine.Random.Range(0, list2.Count)];
		}
		return result;
	}

	private int getSkillGrade(PVPSkillTypeData skillTypeData)
	{
		int result = 0;
		if (skillTypeData.currentMeleeSkillType != MeleeSkillType.None)
		{
			switch (skillTypeData.currentMeleeSkillType)
			{
			case MeleeSkillType.DivineSmash:
			case MeleeSkillType.Concentration:
				result = 3;
				break;
			case MeleeSkillType.DragonSmash:
			case MeleeSkillType.CourageAnthem:
				result = 2;
				break;
			case MeleeSkillType.DragonBreath:
				result = 1;
				break;
			}
		}
		else if (skillTypeData.currentMiddleSkillType != MiddleSkillType.None)
		{
			switch (skillTypeData.currentMiddleSkillType)
			{
			case MiddleSkillType.TargetHeal:
			case MiddleSkillType.FireBall:
				result = 3;
				break;
			case MiddleSkillType.SplashHeal:
			case MiddleSkillType.HeavenArmor:
				result = 2;
				break;
			case MiddleSkillType.Meteor:
				result = 1;
				break;
			}
		}
		else if (skillTypeData.currentRangedSkillType != RangedSkillType.None)
		{
			switch (skillTypeData.currentRangedSkillType)
			{
			case RangedSkillType.Blow:
			case RangedSkillType.HellArrow:
				result = 3;
				break;
			case RangedSkillType.PenetrationArrow:
			case RangedSkillType.LastShot:
				result = 2;
				break;
			case RangedSkillType.RageYell:
				result = 1;
				break;
			}
		}
		return result;
	}

	public PVPSkillTypeData convertIndexToSkillType(int index)
	{
		PVPSkillTypeData result = new PVPSkillTypeData(MeleeSkillType.None);
		switch (index)
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
			result = (MeleeSkillType)index;
			break;
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
			result = (MiddleSkillType)(index - 5);
			break;
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
			result = (RangedSkillType)(index - 10);
			break;
		}
		return result;
	}

	public string getSkillDescription(PVPSkillTypeData skillType)
	{
		int num = 0;
		if (skillType.currentMeleeSkillType != MeleeSkillType.None)
		{
			num = (int)(skillType.currentMeleeSkillType + 1);
		}
		else if (skillType.currentMiddleSkillType != MiddleSkillType.None)
		{
			num = (int)(skillType.currentMiddleSkillType + 6);
		}
		else if (skillType.currentRangedSkillType != RangedSkillType.None)
		{
			num = (int)(skillType.currentRangedSkillType + 11);
		}
		string text = I18NManager.Get("PVP_SKILL_DESCRIPTION_" + num);
		double skillValue = Singleton<PVPSkillManager>.instance.getSkillValue(skillType);
		double num2 = Singleton<PVPSkillManager>.instance.getDuration(skillType);
		if (!text.Contains("{1}"))
		{
			return string.Format(text, skillValue);
		}
		return string.Format(text, skillValue, num2);
	}

	public PVPSkillTypeData getSkillType(PVPManager.PVPSkinData pvpSkinData)
	{
		PVPSkillTypeData result = MeleeSkillType.DivineSmash;
		if (pvpSkinData.currentCharacterType != CharacterManager.CharacterType.Length)
		{
			switch (pvpSkinData.currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				switch (pvpSkinData.currentWarriorSkinType)
				{
				case CharacterSkinManager.WarriorSkinType.William:
				case CharacterSkinManager.WarriorSkinType.Desmond:
				case CharacterSkinManager.WarriorSkinType.Uriel:
				case CharacterSkinManager.WarriorSkinType.Odin:
					result = MeleeSkillType.DivineSmash;
					break;
				case CharacterSkinManager.WarriorSkinType.Samback:
				case CharacterSkinManager.WarriorSkinType.Broomy:
				case CharacterSkinManager.WarriorSkinType.Drake:
					result = MeleeSkillType.Concentration;
					break;
				case CharacterSkinManager.WarriorSkinType.Siegfried:
				case CharacterSkinManager.WarriorSkinType.Valkyrie1:
				case CharacterSkinManager.WarriorSkinType.Blueberry:
				case CharacterSkinManager.WarriorSkinType.Marbas:
				case CharacterSkinManager.WarriorSkinType.Zeus:
				case CharacterSkinManager.WarriorSkinType.Lovely:
				case CharacterSkinManager.WarriorSkinType.Achillia:
					result = MeleeSkillType.DragonSmash;
					break;
				case CharacterSkinManager.WarriorSkinType.Dragoon:
				case CharacterSkinManager.WarriorSkinType.Pirate:
				case CharacterSkinManager.WarriorSkinType.GuanYu:
				case CharacterSkinManager.WarriorSkinType.SnowMan:
				case CharacterSkinManager.WarriorSkinType.HwangJin:
				case CharacterSkinManager.WarriorSkinType.Charlotte:
				case CharacterSkinManager.WarriorSkinType.BaywatchWilliam:
				case CharacterSkinManager.WarriorSkinType.MiniDemon:
				case CharacterSkinManager.WarriorSkinType.Vampire:
				case CharacterSkinManager.WarriorSkinType.GingerMan:
				case CharacterSkinManager.WarriorSkinType.Cosmos:
					result = MeleeSkillType.CourageAnthem;
					break;
				case CharacterSkinManager.WarriorSkinType.Arthur:
				case CharacterSkinManager.WarriorSkinType.BlackKnight:
				case CharacterSkinManager.WarriorSkinType.Phoenix:
				case CharacterSkinManager.WarriorSkinType.MasterWilliam:
					result = MeleeSkillType.DragonBreath;
					break;
				default:
					result = MeleeSkillType.DivineSmash;
					break;
				}
				break;
			case CharacterManager.CharacterType.Priest:
				switch (pvpSkinData.currentPriestSkinType)
				{
				case CharacterSkinManager.PriestSkinType.Olivia:
				case CharacterSkinManager.PriestSkinType.Reinhard:
				case CharacterSkinManager.PriestSkinType.Wandy:
				case CharacterSkinManager.PriestSkinType.Undine:
					result = MiddleSkillType.TargetHeal;
					break;
				case CharacterSkinManager.PriestSkinType.Mona:
				case CharacterSkinManager.PriestSkinType.Mika:
				case CharacterSkinManager.PriestSkinType.Dorothy:
					result = MiddleSkillType.FireBall;
					break;
				case CharacterSkinManager.PriestSkinType.Candy:
				case CharacterSkinManager.PriestSkinType.Valkyrie2:
				case CharacterSkinManager.PriestSkinType.Orange:
				case CharacterSkinManager.PriestSkinType.Amy:
				case CharacterSkinManager.PriestSkinType.Aphrodite:
				case CharacterSkinManager.PriestSkinType.Sweetie:
				case CharacterSkinManager.PriestSkinType.Spartacus:
					result = MiddleSkillType.SplashHeal;
					break;
				case CharacterSkinManager.PriestSkinType.Dragoness:
				case CharacterSkinManager.PriestSkinType.Witch:
				case CharacterSkinManager.PriestSkinType.LiuBei:
				case CharacterSkinManager.PriestSkinType.SnowRabbit:
				case CharacterSkinManager.PriestSkinType.JangGeum:
				case CharacterSkinManager.PriestSkinType.Gabriel:
				case CharacterSkinManager.PriestSkinType.PicnicOlivia:
				case CharacterSkinManager.PriestSkinType.MiniMonkfish:
				case CharacterSkinManager.PriestSkinType.Succubus:
				case CharacterSkinManager.PriestSkinType.Penguin:
				case CharacterSkinManager.PriestSkinType.Daisy:
					result = MiddleSkillType.HeavenArmor;
					break;
				case CharacterSkinManager.PriestSkinType.Michael:
				case CharacterSkinManager.PriestSkinType.Criselda:
				case CharacterSkinManager.PriestSkinType.Elisha:
				case CharacterSkinManager.PriestSkinType.MasterOlivia:
				case CharacterSkinManager.PriestSkinType.Length:
					result = MiddleSkillType.Meteor;
					break;
				default:
					result = MiddleSkillType.TargetHeal;
					break;
				}
				break;
			case CharacterManager.CharacterType.Archer:
				switch (pvpSkinData.currentArcherSkinType)
				{
				case CharacterSkinManager.ArcherSkinType.Windstoker:
				case CharacterSkinManager.ArcherSkinType.Artemis:
				case CharacterSkinManager.ArcherSkinType.ElvenKing:
				case CharacterSkinManager.ArcherSkinType.Robin:
					result = RangedSkillType.Blow;
					break;
				case CharacterSkinManager.ArcherSkinType.Lambo:
				case CharacterSkinManager.ArcherSkinType.Siren:
				case CharacterSkinManager.ArcherSkinType.Bowee:
					result = RangedSkillType.HellArrow;
					break;
				case CharacterSkinManager.ArcherSkinType.Marauder:
				case CharacterSkinManager.ArcherSkinType.Valkyrie3:
				case CharacterSkinManager.ArcherSkinType.Cherry:
				case CharacterSkinManager.ArcherSkinType.Eligos:
				case CharacterSkinManager.ArcherSkinType.Apollo:
				case CharacterSkinManager.ArcherSkinType.Cupid:
				case CharacterSkinManager.ArcherSkinType.Spiculus:
					result = RangedSkillType.PenetrationArrow;
					break;
				case CharacterSkinManager.ArcherSkinType.Dragona:
				case CharacterSkinManager.ArcherSkinType.Alice:
				case CharacterSkinManager.ArcherSkinType.ZhangFei:
				case CharacterSkinManager.ArcherSkinType.Rudolph:
				case CharacterSkinManager.ArcherSkinType.PoliceChief:
				case CharacterSkinManager.ArcherSkinType.Rania:
				case CharacterSkinManager.ArcherSkinType.SurferWindstalker:
				case CharacterSkinManager.ArcherSkinType.MiniDragon:
				case CharacterSkinManager.ArcherSkinType.Owltears:
				case CharacterSkinManager.ArcherSkinType.SantaHelper:
				case CharacterSkinManager.ArcherSkinType.Anemone:
					result = RangedSkillType.LastShot;
					break;
				case CharacterSkinManager.ArcherSkinType.Patricia:
				case CharacterSkinManager.ArcherSkinType.Claude:
				case CharacterSkinManager.ArcherSkinType.Kyle:
				case CharacterSkinManager.ArcherSkinType.MasterWindstoker:
					result = RangedSkillType.RageYell;
					break;
				}
				break;
			}
		}
		else
		{
			switch (pvpSkinData.currentColleagueType)
			{
			case ColleagueManager.ColleagueType.Isabelle:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 4)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
						break;
					case 2L:
						goto IL_0375;
					case 3L:
						goto IL_0381;
					default:
						goto IL_038d;
					}
					result = MiddleSkillType.FireBall;
					break;
				}
				goto IL_038d;
			}
			case ColleagueManager.ColleagueType.Samantha:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 4)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					case 3L:
						goto IL_03e9;
					default:
						goto IL_03f5;
					}
					result = MeleeSkillType.DivineSmash;
					break;
				}
				goto IL_03f5;
			}
			case ColleagueManager.ColleagueType.Lawrence:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 4)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					case 3L:
						goto IL_0451;
					default:
						goto IL_045d;
					}
					result = RangedSkillType.HellArrow;
					break;
				}
				goto IL_045d;
			}
			case ColleagueManager.ColleagueType.Stephanie:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 4)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					case 3L:
						goto IL_04b9;
					default:
						goto IL_04c5;
					}
					result = RangedSkillType.HellArrow;
					break;
				}
				goto IL_04c5;
			}
			case ColleagueManager.ColleagueType.Puppy:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 4)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					case 3L:
						goto IL_0521;
					default:
						goto IL_052d;
					}
					result = MeleeSkillType.DivineSmash;
					break;
				}
				goto IL_052d;
			}
			case ColleagueManager.ColleagueType.Scarlett:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 4)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
						break;
					case 2L:
						goto IL_0589;
					case 3L:
						goto IL_0595;
					default:
						goto IL_05a1;
					}
					result = MiddleSkillType.FireBall;
					break;
				}
				goto IL_05a1;
			}
			case ColleagueManager.ColleagueType.Balbaria:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 4)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					case 3L:
						goto IL_05fd;
					default:
						goto IL_0609;
					}
					result = MeleeSkillType.DivineSmash;
					break;
				}
				goto IL_0609;
			}
			case ColleagueManager.ColleagueType.Sera:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 4)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					case 3L:
						goto IL_0665;
					default:
						goto IL_0671;
					}
					result = MiddleSkillType.TargetHeal;
					break;
				}
				goto IL_0671;
			}
			case ColleagueManager.ColleagueType.Sushiro:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 4)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					case 3L:
						goto IL_06cd;
					default:
						goto IL_06d9;
					}
					result = RangedSkillType.Blow;
					break;
				}
				goto IL_06d9;
			}
			case ColleagueManager.ColleagueType.Pinky:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0731;
					}
					result = MiddleSkillType.TargetHeal;
					break;
				}
				goto IL_0731;
			}
			case ColleagueManager.ColleagueType.Thyrael:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0789;
					}
					result = MeleeSkillType.Concentration;
					break;
				}
				goto IL_0789;
			}
			case ColleagueManager.ColleagueType.Dinnerless:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_07e1;
					}
					result = MiddleSkillType.TargetHeal;
					break;
				}
				goto IL_07e1;
			}
			case ColleagueManager.ColleagueType.FatherKing:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0839;
					}
					result = MiddleSkillType.FireBall;
					break;
				}
				goto IL_0839;
			}
			case ColleagueManager.ColleagueType.GoldenFork:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0891;
					}
					result = MeleeSkillType.DivineSmash;
					break;
				}
				goto IL_0891;
			}
			case ColleagueManager.ColleagueType.Kitty:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_08e9;
					}
					result = MeleeSkillType.Concentration;
					break;
				}
				goto IL_08e9;
			}
			case ColleagueManager.ColleagueType.Seaghoul:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0941;
					}
					result = MiddleSkillType.FireBall;
					break;
				}
				goto IL_0941;
			}
			case ColleagueManager.ColleagueType.HoneyQueen:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
						break;
					case 1L:
					case 2L:
						goto IL_0999;
					default:
						goto IL_09a5;
					}
					result = MeleeSkillType.Concentration;
					break;
				}
				goto IL_09a5;
			}
			case ColleagueManager.ColleagueType.Barbie:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_09fd;
					}
					result = RangedSkillType.Blow;
					break;
				}
				goto IL_09fd;
			}
			case ColleagueManager.ColleagueType.Prince:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0a55;
					}
					result = MeleeSkillType.DivineSmash;
					break;
				}
				goto IL_0a55;
			}
			case ColleagueManager.ColleagueType.BabyDragon:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0aad;
					}
					result = MiddleSkillType.FireBall;
					break;
				}
				goto IL_0aad;
			}
			case ColleagueManager.ColleagueType.Golem:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0b05;
					}
					result = MeleeSkillType.Concentration;
					break;
				}
				goto IL_0b05;
			}
			case ColleagueManager.ColleagueType.Poty:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0b5d;
					}
					result = MiddleSkillType.FireBall;
					break;
				}
				goto IL_0b5d;
			}
			case ColleagueManager.ColleagueType.Trall:
			{
				long num = pvpSkinData.currentColleagueSkinIndex;
				if (num >= 1 && num <= 3)
				{
					switch (num - 1)
					{
					case 0L:
					case 1L:
					case 2L:
						break;
					default:
						goto IL_0bb5;
					}
					result = MeleeSkillType.Concentration;
					break;
				}
				goto IL_0bb5;
			}
			default:
				{
					result = MeleeSkillType.DivineSmash;
					break;
				}
				IL_0595:
				result = MiddleSkillType.Meteor;
				break;
				IL_0589:
				result = MiddleSkillType.TargetHeal;
				break;
				IL_07e1:
				result = MiddleSkillType.TargetHeal;
				break;
				IL_0bb5:
				result = MeleeSkillType.Concentration;
				break;
				IL_0451:
				result = RangedSkillType.RageYell;
				break;
				IL_05a1:
				result = MiddleSkillType.TargetHeal;
				break;
				IL_0b5d:
				result = MiddleSkillType.FireBall;
				break;
				IL_0789:
				result = MeleeSkillType.Concentration;
				break;
				IL_0b05:
				result = MeleeSkillType.Concentration;
				break;
				IL_045d:
				result = RangedSkillType.HellArrow;
				break;
				IL_0521:
				result = MeleeSkillType.DragonBreath;
				break;
				IL_0aad:
				result = MiddleSkillType.FireBall;
				break;
				IL_0731:
				result = MiddleSkillType.TargetHeal;
				break;
				IL_0a55:
				result = MeleeSkillType.DivineSmash;
				break;
				IL_052d:
				result = MeleeSkillType.DivineSmash;
				break;
				IL_06cd:
				result = RangedSkillType.RageYell;
				break;
				IL_09fd:
				result = RangedSkillType.Blow;
				break;
				IL_06d9:
				result = RangedSkillType.Blow;
				break;
				IL_0999:
				result = MeleeSkillType.DivineSmash;
				break;
				IL_0375:
				result = MiddleSkillType.TargetHeal;
				break;
				IL_09a5:
				result = MeleeSkillType.DivineSmash;
				break;
				IL_038d:
				result = MiddleSkillType.TargetHeal;
				break;
				IL_0665:
				result = MiddleSkillType.Meteor;
				break;
				IL_04b9:
				result = MiddleSkillType.Meteor;
				break;
				IL_0941:
				result = MiddleSkillType.FireBall;
				break;
				IL_0671:
				result = MiddleSkillType.TargetHeal;
				break;
				IL_03e9:
				result = MeleeSkillType.DragonBreath;
				break;
				IL_08e9:
				result = MeleeSkillType.Concentration;
				break;
				IL_04c5:
				result = RangedSkillType.HellArrow;
				break;
				IL_05fd:
				result = MeleeSkillType.DragonBreath;
				break;
				IL_0891:
				result = MeleeSkillType.DivineSmash;
				break;
				IL_0609:
				result = MeleeSkillType.DivineSmash;
				break;
				IL_0381:
				result = MiddleSkillType.Meteor;
				break;
				IL_0839:
				result = MiddleSkillType.FireBall;
				break;
				IL_03f5:
				result = MeleeSkillType.DivineSmash;
				break;
			}
		}
		return result;
	}
}
