using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
	public enum SkillType
	{
		None = -1,
		DivineSmash,
		WhirlWind,
		Concentration,
		ClonedWarrior,
		DragonsBreath,
		Length
	}

	public enum PassiveSkillType
	{
		None = -1,
		FrostSkill,
		MeteorRain,
		SwordSoul,
		Length
	}

	public enum CustomSkillType
	{
		None = -1,
		TimerSilverFinger,
		TimerGoldFinger,
		CountGoldFinger,
		TimerAutoOpenTreasureChest,
		DoubleSpeed,
		Length
	}

	[Serializable]
	public class ActiveSkillData
	{
		public double baseDamage;

		public double upgradeDamage;

		public int cost;

		public ActiveSkillData(double baseDamage, double upgradePercent, int cost)
		{
			this.baseDamage = baseDamage;
			upgradeDamage = upgradePercent;
			this.cost = cost;
		}
	}

	public bool[] skillAvailable;

	public bool isSkillLock;

	public bool isClonedWarrior;

	public bool isConcentration;

	public float interval;

	public bool isCastedSkillFromPreview;

	public Sprite normalArrowSprite;

	public CloneWarrior currentCloneWarriorObject;

	private List<NumberText> m_numberTextList = new List<NumberText>();

	private UIWindowIngame ingameUI;

	private bool m_isCanCastSkill = true;

	public List<ActiveSkillData> activeSkillData = new List<ActiveSkillData>();

	private int manaCount;

	public GameObject[] previewObject;

	public int previewType = -1;

	public SpecialObject dummy;

	private bool dragonRecon;

	private bool m_isCastingSkill;

	private Dictionary<SkillType, float> m_currentActiveSkillTimer;

	private List<Bullet> m_currentArrowList = new List<Bullet>();

	private List<GameObject> auraObjects = new List<GameObject>();

	private List<PassiveSkillType> m_currentCastingPassiveSkillList = new List<PassiveSkillType>();

	private Dictionary<PassiveSkillType, float> m_currentPassiveSkillTimer;

	private void Awake()
	{
		m_currentPassiveSkillTimer = new Dictionary<PassiveSkillType, float>();
		for (int i = 0; i < 3; i++)
		{
			m_currentPassiveSkillTimer.Add((PassiveSkillType)i, 0f);
		}
		m_currentActiveSkillTimer = new Dictionary<SkillType, float>();
		for (int j = 0; j < 5; j++)
		{
			m_currentActiveSkillTimer.Add((SkillType)j, 0f);
		}
		refreshSkillBaseData();
	}

	public void startGame()
	{
		resetCooltimeTexts();
		StopAllCoroutines();
		auraObjects.Clear();
		isCastedSkillFromPreview = false;
		Singleton<SkillManager>.instance.currentCloneWarriorObject = null;
		m_isCastingSkill = false;
		m_isCanCastSkill = true;
		ingameUI = UIWindowIngame.instance;
		isClonedWarrior = false;
		isConcentration = false;
		LockSkillButton(false);
		manaCount = 0;
		Singleton<FeverManager>.instance.currentMana = Singleton<StatManager>.instance.startManaCount;
		CheckMana();
		CheckSkillAll();
		refreshSkillBaseData();
		for (int i = 0; i < ingameUI.skillCostText.Length; i++)
		{
			ingameUI.skillCostText[i].text = activeSkillData[i].cost.ToString();
		}
	}

	public void endGame()
	{
		resetCooltimeTexts();
		StopAllCoroutines();
		auraObjects.Clear();
	}

	private void resetCooltimeTexts()
	{
		UIWindowIngame.instance.closeSkillCooltimeSlot(SkillType.ClonedWarrior);
		UIWindowIngame.instance.closeSkillCooltimeSlot(SkillType.Concentration);
	}

	public void refreshSkillBaseData()
	{
		activeSkillData.Clear();
		foreach (KeyValuePair<SkillType, ActiveSkillStatData> activeSkillStatDatum in Singleton<ParsingManager>.instance.currentParsedStatData.activeSkillStatData)
		{
			if (activeSkillStatDatum.Key == SkillType.DragonsBreath)
			{
				activeSkillData.Add(new ActiveSkillData(activeSkillStatDatum.Value.basePercentValue, activeSkillStatDatum.Value.increasePercentValue, activeSkillStatDatum.Value.manaCost));
			}
			else
			{
				activeSkillData.Add(new ActiveSkillData(activeSkillStatDatum.Value.basePercentValue, activeSkillStatDatum.Value.increasePercentValue, activeSkillStatDatum.Value.manaCost));
			}
		}
	}

	public void CheckMana()
	{
		manaCount = Singleton<FeverManager>.instance.currentMana;
		if (manaCount >= 10)
		{
			int num = Mathf.FloorToInt((float)manaCount / 10f);
			int num2 = manaCount % 10;
			ingameUI.skillManaCountImage.sprite = ingameUI.skillStarCountSprites[num];
			ingameUI.skillManaCountImage.SetNativeSize();
			ingameUI.skillManaCountImage2.gameObject.SetActive(true);
			ingameUI.skillManaCountImage2.sprite = ingameUI.skillStarCountSprites[num2];
			ingameUI.skillManaCountImage2.SetNativeSize();
		}
		else
		{
			ingameUI.skillManaCountImage.sprite = ingameUI.skillStarCountSprites[manaCount];
			ingameUI.skillManaCountImage.SetNativeSize();
			ingameUI.skillManaCountImage2.gameObject.SetActive(false);
		}
		ingameUI.skillManaCountAnim.Play("SkillTextStarCount");
	}

	public void CheckSkillAll()
	{
		for (int i = 0; i < skillAvailable.Length; i++)
		{
			CheckSkill(i);
		}
	}

	public bool CheckSkill(int type)
	{
		CharacterWarrior warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		bool flag = false;
		manaCount = Singleton<FeverManager>.instance.currentMana;
		if (manaCount >= activeSkillData[type].cost)
		{
			flag = true;
		}
		if (warriorCharacter.getState() == PublicDataManager.State.CastSkill)
		{
			flag = false;
		}
		if (getSkillInventoryData((SkillType)type).isHasSkill)
		{
			ingameUI.skillLockedObjects[type].SetActive(false);
		}
		else
		{
			ingameUI.skillLockedObjects[type].SetActive(true);
			flag = false;
		}
		if (flag)
		{
			ingameUI.skillBlockedObjects[type].SetActive(false);
			ingameUI.skillGlowObjects[type].SetActive(true);
		}
		else
		{
			ingameUI.skillBlockedObjects[type].SetActive(true);
			ingameUI.skillGlowObjects[type].SetActive(false);
		}
		skillAvailable[type] = flag;
		if (type == 0 && warriorCharacter.isStair())
		{
			flag = false;
		}
		return flag;
	}

	public void LockSkillButton(bool lockSkill)
	{
		isSkillLock = lockSkill;
		for (int i = 0; i < 5; i++)
		{
			if (Singleton<FeverManager>.instance.currentMana < activeSkillData[i].cost)
			{
				continue;
			}
			if (getSkillInventoryData((SkillType)i).isHasSkill)
			{
				ingameUI.skillLockedObjects[i].SetActive(false);
				if (lockSkill)
				{
					ingameUI.skillBlockedObjects[i].SetActive(true);
					ingameUI.skillGlowObjects[i].SetActive(false);
				}
				else
				{
					ingameUI.skillBlockedObjects[i].SetActive(false);
					ingameUI.skillGlowObjects[i].SetActive(true);
				}
			}
			else
			{
				ingameUI.skillLockedObjects[i].SetActive(true);
				ingameUI.skillBlockedObjects[i].SetActive(true);
				ingameUI.skillGlowObjects[i].SetActive(false);
			}
		}
	}

	public void castSkillPreview(SkillType type)
	{
		if (type != SkillType.None)
		{
			StopCoroutine("waitClose");
			previewType = (int)type;
			previewObject[(int)type].SetActive(true);
			isCastedSkillFromPreview = true;
			Singleton<CachedManager>.instance.previewCamera.localPosition = new Vector2(-1.7f, 4.37f);
			dummy.cachedTransform.localPosition = new Vector2(1.34f, -3.203f);
			switch (type)
			{
			case SkillType.Concentration:
				Singleton<CharacterManager>.instance.warriorCharacter.targetEnemy = dummy;
				Util.changeSpritesColor(Singleton<CharacterManager>.instance.warriorCharacter.characterSpriteRenderers, new Color(1f, 0.4f, 0.4f));
				StartCoroutine("waitClose");
				Singleton<AudioManager>.instance.playEffectSound("skill_concentration_loop", true);
				break;
			case SkillType.DivineSmash:
				Singleton<CharacterManager>.instance.warriorCharacter.castSkill(type, true);
				break;
			case SkillType.WhirlWind:
				Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.localPosition = new Vector3(-1.79f, 1.65f, 0f);
				dummy.cachedTransform.localPosition = new Vector2(-0.24f, -3.2f);
				StopCoroutine("WhirlWind");
				StartCoroutine("WhirlWind", false);
				StartCoroutine("waitClose");
				break;
			case SkillType.ClonedWarrior:
				Singleton<CachedManager>.instance.previewCamera.localPosition = new Vector2(-0.35f, 4.37f);
				dummy.cachedTransform.localPosition = new Vector2(-0.12f, -3.203f);
				isClonedWarrior = true;
				Singleton<CharacterManager>.instance.warriorCharacter.targetEnemy = dummy;
				Singleton<CharacterManager>.instance.warriorCharacter.castSkill(type, true);
				StartCoroutine("waitClose");
				break;
			case SkillType.DragonsBreath:
				dragonMove(dragonRecon);
				Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.position = Vector2.one * 500f;
				Singleton<AudioManager>.instance.playEffectSound("skill_dragon", true);
				StartCoroutine("waitClose");
				break;
			}
		}
	}

	private IEnumerator waitClose()
	{
		yield return new WaitForSeconds(3f);
		UIWindowSkill.instance.OnClickClose();
	}

	public void stopSkillPreview(SkillType type)
	{
		if (type == SkillType.None)
		{
			return;
		}
		StopCoroutine("waitClose");
		previewType = -1;
		previewObject[(int)type].SetActive(false);
		Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.localPosition = new Vector3(-1.7f, 0f, 0f);
		switch (type)
		{
		case SkillType.Concentration:
			Singleton<AudioManager>.instance.stopLoopEffectSound("skill_concentration_loop");
			Util.changeSpritesColor(Singleton<CharacterManager>.instance.warriorCharacter.characterSpriteRenderers, Color.white);
			Singleton<CharacterManager>.instance.warriorCharacter.setState(PublicDataManager.State.Wait);
			break;
		case SkillType.DivineSmash:
			Singleton<CharacterManager>.instance.warriorCharacter.stopAll();
			Singleton<CharacterManager>.instance.warriorCharacter.setStateLock(false);
			Singleton<CharacterManager>.instance.warriorCharacter.setState(PublicDataManager.State.Wait);
			break;
		case SkillType.WhirlWind:
		{
			StopCoroutine("WhirlWind");
			for (int i = 0; i < m_currentArrowList.Count; i++)
			{
				if (m_currentArrowList[i] != null)
				{
					m_currentArrowList[i].recycleBullet();
				}
			}
			m_currentArrowList.Clear();
			break;
		}
		case SkillType.ClonedWarrior:
			isClonedWarrior = false;
			Singleton<CharacterManager>.instance.warriorCharacter.setState(PublicDataManager.State.Wait);
			break;
		case SkillType.DragonsBreath:
			Singleton<AudioManager>.instance.stopLoopEffectSound("skill_dragon");
			break;
		}
	}

	public void dragonMove(bool recon)
	{
		DragonBreath component = previewObject[4].transform.GetChild(0).GetComponent<DragonBreath>();
		component.fixedDirection = true;
		Vector2 targetPosition = new Vector2((!recon) ? (-2) : 0, previewObject[4].transform.position.y);
		component.moveTo(targetPosition, 1f, delegate
		{
			dragonRecon = !dragonRecon;
			dragonMove(dragonRecon);
		});
	}

	public SkillInventoryData getSkillInventoryData(SkillType skillType)
	{
		return Singleton<DataManager>.instance.currentGameData.skillInventoryData[(int)skillType];
	}

	public ReinforcementSkillInventoryData getReinforcementSkillInventoryData(SkillType skillType)
	{
		return Singleton<DataManager>.instance.currentGameData.reinforcementSkillInventoryData[skillType];
	}

	public float getSkillMaxDuration(SkillType skillType)
	{
		float result = 0f;
		if (skillType == SkillType.Concentration)
		{
			result = 10f + Singleton<StatManager>.instance.concentrationExtraDurationTimeFromPremiumTreasure;
		}
		return result;
	}

	public double getSkillValue(SkillType skillType, int level, params double[] arg)
	{
		double num = 0.0;
		if (skillType == SkillType.Concentration)
		{
			num = activeSkillData[(int)skillType].baseDamage + activeSkillData[(int)skillType].upgradeDamage * (double)(level - 1);
			return num + num / 100.0 * Singleton<StatManager>.instance.skillExtraPercentDamage;
		}
		num = (Singleton<CharacterManager>.instance.warriorCharacter.getCurrentDamage(arg) + Singleton<CharacterManager>.instance.priestCharacter.getCurrentDamage(arg) + Singleton<CharacterManager>.instance.archerCharacter.getCurrentDamage(arg)) * (activeSkillData[(int)skillType].baseDamage + activeSkillData[(int)skillType].upgradeDamage * (double)(level - 1)) / 100.0;
		return num + num / 100.0 * Singleton<StatManager>.instance.skillExtraPercentDamage;
	}

	public double getSkillUnlockGoldPrice(SkillType skillType)
	{
		return 100.0 * Math.Pow(5.0, 1.5 * (double)skillType);
	}

	public double getSkillUpgradePrice(SkillType skillType, int level)
	{
		return getSkillUnlockGoldPrice(skillType) / 7.0 * Math.Pow(1.20408, level);
	}

	public long getAutoTouchSkillBuyPrice(CustomSkillType skillType)
	{
		long result = 0L;
		switch (skillType)
		{
		case CustomSkillType.TimerSilverFinger:
			result = 30L;
			break;
		case CustomSkillType.TimerGoldFinger:
			result = 50L;
			break;
		case CustomSkillType.CountGoldFinger:
			result = 50L;
			break;
		case CustomSkillType.TimerAutoOpenTreasureChest:
			result = 70L;
			break;
		case CustomSkillType.DoubleSpeed:
			result = 100L;
			break;
		}
		return result;
	}

	private void resetActiveSkillTimer(SkillType skillType)
	{
		m_currentActiveSkillTimer[skillType] = 0f;
	}

	private void resetAllActiveSkillTimer()
	{
		for (int i = 0; i < 5; i++)
		{
			m_currentActiveSkillTimer[(SkillType)i] = 0f;
		}
	}

	public void OnClickCastSkill(int type)
	{
		if (isSkillLock || !m_isCanCastSkill || m_isCastingSkill || UIWindowIngame.instance.bossWarningState)
		{
			return;
		}
		CharacterWarrior warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		if (!warriorCharacter.isJumping() && !Singleton<CharacterManager>.instance.priestCharacter.isJumping() && !Singleton<CharacterManager>.instance.archerCharacter.isJumping() && !warriorCharacter.isFirstGround() && !Singleton<CharacterManager>.instance.priestCharacter.isFirstGround() && !Singleton<CharacterManager>.instance.archerCharacter.isFirstGround())
		{
			if (CheckSkill(type) && warriorCharacter.isCanCastSkill && warriorCharacter != null && warriorCharacter.getState() != PublicDataManager.State.Die)
			{
				Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.CorrectSkillUsage, 1.0);
				Singleton<QuestManager>.instance.questReport(QuestManager.QuestType.Skill, 1.0);
				warriorCharacter.setStateLock(false);
				warriorCharacter.setState(PublicDataManager.State.Wait);
				warriorCharacter.setStateLock(true);
				warriorCharacter.isAttacking = false;
				warriorCharacter.isCastingSkill = true;
				warriorCharacter.isCanPrintHitEffect = false;
				Singleton<FeverManager>.instance.UseMana(activeSkillData[type].cost);
				CheckSkillAll();
				m_isCanCastSkill = false;
				playActiveSkillEffect((SkillType)type);
			}
		}
	}

	private void playActiveSkillEffect(SkillType skillType)
	{
		CharacterWarrior warrior = Singleton<CharacterManager>.instance.warriorCharacter;
		switch (skillType)
		{
		case SkillType.DivineSmash:
			warrior.setStateLock(false);
			warrior.playBoneAnimation(warrior.currentBoneAnimationName.skillName[0]);
			castSkill(skillType);
			warrior.isCanPrintHitEffect = true;
			CheckSkill((int)skillType);
			m_isCanCastSkill = true;
			break;
		case SkillType.WhirlWind:
		{
			Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
			Singleton<GameManager>.instance.setTimeScale(true);
			warrior.setStateLock(false);
			for (int i = 0; i < Singleton<CharacterManager>.instance.characterList.Count; i++)
			{
				Singleton<CharacterManager>.instance.characterList[i].setState(PublicDataManager.State.Move);
			}
			castSkill(skillType);
			warrior.isCanPrintHitEffect = true;
			CheckSkill((int)skillType);
			warrior.isCastingSkill = false;
			m_isCanCastSkill = true;
			break;
		}
		case SkillType.Concentration:
		case SkillType.ClonedWarrior:
		case SkillType.DragonsBreath:
		{
			GameManager.timeScale = 0.2f;
			ParticleSystem castEffect = ObjectPool.Spawn("@CastSkill", (Vector2)warrior.cachedTransform.position + Vector2.up * 1.5f).GetComponent<ParticleSystem>();
			Singleton<AudioManager>.instance.playEffectSound("skill_casting", AudioManager.EffectType.Skill);
			Singleton<CachedManager>.instance.darkUI.fadeOut();
			CameraFit.Instance.setCameraSize(0.75f, warrior.cachedTransform);
			warrior.playBoneAnimation(warrior.currentBoneAnimationName.skillName[0]);
			StartCoroutine(waitForCastSkill(delegate
			{
				Singleton<GameManager>.instance.setTimeScale(true);
				warrior.setStateLock(false);
				for (int j = 0; j < Singleton<CharacterManager>.instance.characterList.Count; j++)
				{
					Singleton<CharacterManager>.instance.characterList[j].setState(PublicDataManager.State.Move);
				}
				Singleton<CachedManager>.instance.darkUI.fadeInGame();
				CameraFit.Instance.setCameraDefault();
				castSkill(skillType);
				ObjectPool.Recycle("@CastSkill", castEffect.gameObject);
				warrior.isCanPrintHitEffect = true;
				CheckSkill((int)skillType);
				warrior.isCastingSkill = false;
				m_isCanCastSkill = true;
			}));
			m_isCastingSkill = false;
			break;
		}
		}
	}

	public void castSkill(SkillType skillType)
	{
		bool flag = checkReinforcementSkill(skillType);
		string activeSkillCoroutine = getActiveSkillCoroutine(skillType, flag);
		if (activeSkillCoroutine != null)
		{
			resetActiveSkillTimer(skillType);
			if (skillType != SkillType.WhirlWind)
			{
				StopCoroutine(activeSkillCoroutine.ToString());
			}
			StartCoroutine(activeSkillCoroutine.ToString(), flag);
		}
	}

	private bool checkReinforcementSkill(SkillType skillType)
	{
		bool result = false;
		MersenneTwister mersenneTwister = new MersenneTwister();
		double num = mersenneTwister.Next(0, 100000);
		num /= 1000.0;
		if (getReinforcementSkillInventoryData(skillType).isUnlocked && getReinforcementSkillCastChance(skillType) > 0.0 && num <= getReinforcementSkillCastChance(skillType))
		{
			result = true;
		}
		return result;
	}

	public double getReinforcementSkillCastChance(SkillType skillType)
	{
		double num = 0.0;
		ReinforcementSkillInventoryData reinforcementSkillInventoryData = getReinforcementSkillInventoryData(skillType);
		return (double)(20 + (reinforcementSkillInventoryData.skillLevel - 1) * 5) + Singleton<StatManager>.instance.reinforcementSkillExtraCastChanceFromPremiumTreasure;
	}

	public double getReinforcementSkillValue(SkillType skillType)
	{
		double result = 0.0;
		switch (skillType)
		{
		case SkillType.DivineSmash:
			result = 200.0 + Singleton<StatManager>.instance.reinforcementDivneSmashExtraDamageFromPremiumTreasure;
			break;
		case SkillType.WhirlWind:
			result = 30.0;
			break;
		case SkillType.Concentration:
			result = 30.0;
			break;
		case SkillType.ClonedWarrior:
			result = 25.0 + Singleton<StatManager>.instance.reinforcementCloneWarriorExtraDamageFromPremiumTreasure;
			break;
		case SkillType.DragonsBreath:
			result = 100.0 + Singleton<StatManager>.instance.reinforcementDragonBreathExtraDamageFromPremiumTreasure;
			break;
		}
		return result;
	}

	public double getReinforcementCloneWarriorConfusionVaule()
	{
		return 20.0;
	}

	private string getActiveSkillCoroutine(SkillType skillType, bool isReinforcementSkill)
	{
		string result = null;
		switch (skillType)
		{
		case SkillType.DivineSmash:
			result = "DivineSmash";
			break;
		case SkillType.WhirlWind:
			result = "WhirlWind";
			break;
		case SkillType.Concentration:
			result = "Concentration";
			break;
		case SkillType.ClonedWarrior:
			result = "ClonedWarrior";
			break;
		case SkillType.DragonsBreath:
			result = "DragonsBreath";
			break;
		}
		return result;
	}

	private IEnumerator DivineSmash(bool isReinforcementSkill)
	{
		Singleton<CharacterManager>.instance.warriorCharacter.castSkill(SkillType.DivineSmash, false, isReinforcementSkill);
		yield return null;
	}

	private IEnumerator WhirlWind(bool isReinforcementSkill)
	{
		CharacterWarrior warrior = Singleton<CharacterManager>.instance.warriorCharacter;
		if (previewType == 1)
		{
			yield return new WaitForSeconds(0.7f);
			ObjectPool.Spawn("@WhirlWindEffectForPreview", new Vector2(0f, 0.24f), Vector3.zero, warrior.cachedTransform.localScale);
			Singleton<AudioManager>.instance.playEffectSound("skill_whirlwind");
		}
		else if (!isReinforcementSkill)
		{
			whirlWind(false);
		}
		else
		{
			float timer = 0.2f;
			int skillCastCount = 0;
			int attackCount = 3;
			if (Singleton<StatManager>.instance.reinforcementWhirlWindExtraHitChanceFromPremiumTreasure > 0.0)
			{
				double random = (double)UnityEngine.Random.Range(0, 10000) * 0.01;
				if (random <= Singleton<StatManager>.instance.reinforcementWhirlWindExtraHitChanceFromPremiumTreasure)
				{
					attackCount = 4;
				}
			}
			while (skillCastCount < attackCount)
			{
				if (!GameManager.isPause)
				{
					timer += Time.deltaTime * GameManager.timeScale;
					if (timer >= 0.2f)
					{
						timer = 0f;
						whirlWind(true, attackCount);
						skillCastCount++;
					}
				}
				yield return null;
			}
		}
		yield return null;
	}

	private void whirlWind(bool isReinforcementSkill, int hitCount = 1)
	{
		CharacterWarrior warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		if (!isReinforcementSkill)
		{
			ObjectPool.Spawn("@WhirlWindEffect", new Vector2(0f, 0.28f), Vector3.zero, warriorCharacter.cachedTransform.localScale, warriorCharacter.cachedTransform);
		}
		else
		{
			ObjectPool.Spawn("@ReinforcementWhirlWindEffect", new Vector2(0f, 0.28f), Vector3.zero, warriorCharacter.cachedTransform.localScale, warriorCharacter.cachedTransform);
		}
		Singleton<AudioManager>.instance.playEffectSound("skill_whirlwind");
		double num = Singleton<SkillManager>.instance.getSkillValue(SkillType.WhirlWind, Singleton<SkillManager>.instance.getSkillInventoryData(SkillType.WhirlWind).skillLevel);
		float range = ((!isReinforcementSkill) ? 3f : 4f);
		List<EnemyObject> nearestEnemiesIgnoreFloor = Singleton<EnemyManager>.instance.getNearestEnemiesIgnoreFloor(warriorCharacter.cachedTransform.position, range);
		if (nearestEnemiesIgnoreFloor.Count > 0)
		{
			ShakeCamera.Instance.shake(1f, 0.1f);
		}
		for (int i = 0; i < nearestEnemiesIgnoreFloor.Count; i++)
		{
			if (!nearestEnemiesIgnoreFloor[i].isDead)
			{
				if (nearestEnemiesIgnoreFloor[i].isBoss || (nearestEnemiesIgnoreFloor[i].myMonsterObject != null && nearestEnemiesIgnoreFloor[i].myMonsterObject.isMiniboss))
				{
					num = Singleton<SkillManager>.instance.getSkillValue(SkillType.WhirlWind, Singleton<SkillManager>.instance.getSkillInventoryData(SkillType.WhirlWind).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses);
				}
				if (!nearestEnemiesIgnoreFloor[i].isBoss && !nearestEnemiesIgnoreFloor[i].isMiniboss)
				{
					num = Singleton<SkillManager>.instance.getSkillValue(SkillType.WhirlWind, Singleton<SkillManager>.instance.getSkillInventoryData(SkillType.WhirlWind).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters);
				}
				if (isReinforcementSkill)
				{
					num += num / 100.0 * Singleton<SkillManager>.instance.getReinforcementSkillValue(SkillType.WhirlWind);
					num /= (double)hitCount;
				}
				nearestEnemiesIgnoreFloor[i].decreasesHealth(num);
				nearestEnemiesIgnoreFloor[i].setDamageText(num, CharacterManager.CharacterType.Warrior, true, true, true);
			}
		}
		CheckSkillAll();
	}

	private IEnumerator Concentration(bool isReinforcementSkill)
	{
		isConcentration = true;
		Singleton<AudioManager>.instance.stopLoopEffectSound("skill_concentration_loop");
		float maxCooltime = getSkillMaxDuration(SkillType.Concentration);
		double skillVal = getSkillValue(SkillType.Concentration, getSkillInventoryData(SkillType.Concentration).skillLevel);
		Singleton<StatManager>.instance.allPercentDamageByConcentraion = skillVal;
		UIWindowIngame.instance.setOpenSkillInformation(SkillType.Concentration, maxCooltime, isReinforcementSkill);
		for (int k = 0; k < auraObjects.Count; k++)
		{
			if (auraObjects[k] != null)
			{
				ObjectPool.Recycle(auraObjects[k].name, auraObjects[k]);
			}
		}
		auraObjects.Clear();
		for (int j = 0; j < Singleton<CharacterManager>.instance.characterList.Count; j++)
		{
			if (Singleton<CharacterManager>.instance.characterList[j] != null)
			{
				if (!isReinforcementSkill)
				{
					auraObjects.Add(ObjectPool.Spawn("fx_skill_concentration_aura", new Vector2(0f, 0.5f), Singleton<CharacterManager>.instance.characterList[j].cachedTransform));
				}
				else
				{
					auraObjects.Add(ObjectPool.Spawn("@ReinforcementConcentrationEffect", new Vector2(0f, 0.5f), Singleton<CharacterManager>.instance.characterList[j].cachedTransform));
				}
			}
		}
		if (isClonedWarrior && currentCloneWarriorObject != null)
		{
			if (!isReinforcementSkill)
			{
				auraObjects.Add(ObjectPool.Spawn("fx_skill_concentration_aura", new Vector2(0f, 0.127f), currentCloneWarriorObject.cachedTransform));
			}
			else
			{
				auraObjects.Add(ObjectPool.Spawn("@ReinforcementConcentrationEffect", new Vector2(0f, 0.127f), currentCloneWarriorObject.cachedTransform));
			}
		}
		Singleton<AudioManager>.instance.playEffectSound("skill_concentration_loop", true);
		if (isReinforcementSkill)
		{
			Singleton<StatManager>.instance.decreaseDamageFromReinforcementConcentration = (float)Singleton<SkillManager>.instance.getReinforcementSkillValue(SkillType.Concentration);
		}
		float timerForPremiumTreasure = 0f;
		while (m_currentActiveSkillTimer[SkillType.Concentration] < maxCooltime)
		{
			if (!GameManager.isPause)
			{
				if (isReinforcementSkill)
				{
					timerForPremiumTreasure += Time.deltaTime * GameManager.timeScale;
					if (timerForPremiumTreasure >= 1f)
					{
						timerForPremiumTreasure -= 1f;
						CharacterObject warrior = Singleton<CharacterManager>.instance.warriorCharacter;
						if (warrior != null)
						{
							double hpIncreaseValue = warrior.maxHealth / 100.0 * Singleton<StatManager>.instance.reinforcementConcentrationExtraDamageFromPremiumTreasure;
							warrior.increasesHealth(hpIncreaseValue, false);
							ObjectPool.Spawn("@AngelicaHealEffect", new Vector3(0f, -0.1187687f, 0f), warrior.cachedTransform);
						}
					}
				}
				Dictionary<SkillType, float> currentActiveSkillTimer;
				Dictionary<SkillType, float> dictionary = (currentActiveSkillTimer = m_currentActiveSkillTimer);
				SkillType key;
				SkillType key2 = (key = SkillType.Concentration);
				float num = currentActiveSkillTimer[key];
				dictionary[key2] = num + Time.deltaTime * GameManager.timeScale;
				UIWindowIngame.instance.changeSkillCooltimeData(SkillType.Concentration, maxCooltime, m_currentActiveSkillTimer[SkillType.Concentration]);
			}
			if (Singleton<CharacterManager>.instance.warriorCharacter.isDead || !isConcentration)
			{
				break;
			}
			yield return null;
		}
		Singleton<StatManager>.instance.decreaseDamageFromReinforcementConcentration = 0f;
		Singleton<AudioManager>.instance.stopLoopEffectSound("skill_concentration_loop");
		Singleton<StatManager>.instance.allPercentDamageByConcentraion = 0.0;
		isConcentration = false;
		UIWindowIngame.instance.closeSkillCooltimeSlot(SkillType.Concentration);
		for (int i = 0; i < auraObjects.Count; i++)
		{
			if (auraObjects[i] != null)
			{
				ObjectPool.Recycle(auraObjects[i].name, auraObjects[i]);
			}
		}
		auraObjects.Clear();
		CheckSkillAll();
	}

	private IEnumerator ClonedWarrior(bool isReinforcementSkill)
	{
		isClonedWarrior = true;
		Singleton<CharacterManager>.instance.warriorCharacter.castSkill(SkillType.ClonedWarrior, false, isReinforcementSkill);
		UIWindowIngame.instance.setOpenSkillInformation(SkillType.ClonedWarrior, 8f, isReinforcementSkill);
		if (isReinforcementSkill)
		{
			Singleton<StatManager>.instance.confusionValueFromReinforcementCloneWarrior = getReinforcementCloneWarriorConfusionVaule();
		}
		while (m_currentActiveSkillTimer[SkillType.ClonedWarrior] < 8f)
		{
			if (!GameManager.isPause)
			{
				Dictionary<SkillType, float> currentActiveSkillTimer;
				Dictionary<SkillType, float> dictionary = (currentActiveSkillTimer = m_currentActiveSkillTimer);
				SkillType key;
				SkillType key2 = (key = SkillType.ClonedWarrior);
				float num = currentActiveSkillTimer[key];
				dictionary[key2] = num + Time.deltaTime * GameManager.timeScale;
				UIWindowIngame.instance.changeSkillCooltimeData(SkillType.ClonedWarrior, 8f, m_currentActiveSkillTimer[SkillType.ClonedWarrior]);
			}
			if (Singleton<CharacterManager>.instance.warriorCharacter.isDead || !isClonedWarrior)
			{
				break;
			}
			yield return null;
		}
		Singleton<StatManager>.instance.confusionValueFromReinforcementCloneWarrior = 0.0;
		isClonedWarrior = false;
		UIWindowIngame.instance.closeSkillCooltimeSlot(SkillType.ClonedWarrior);
		CheckSkillAll();
	}

	private IEnumerator DragonsBreath(bool isReinforcementSkill)
	{
		if (!isReinforcementSkill)
		{
			DragonBreath wave = ObjectPool.Spawn("@DragonBreath", new Vector2(5f, Singleton<CharacterManager>.instance.warriorCharacter.shootPoint.position.y)).GetComponent<DragonBreath>();
			wave.lineIdx = Singleton<GroundManager>.instance.getFloor();
			wave.lineMax = 4;
			wave.shootWave(Singleton<CharacterManager>.instance.warriorCharacter);
		}
		else
		{
			CharacterWarrior warrior = Singleton<CharacterManager>.instance.warriorCharacter;
			Ground currentWarriorStayingGround = warrior.currentGround;
			List<Ground> attackTargetGroundList = new List<Ground>
			{
				currentWarriorStayingGround
			};
			Ground nextGround = Singleton<GroundManager>.instance.getNextGround(currentWarriorStayingGround);
			while (nextGround != null && attackTargetGroundList.Count < 4)
			{
				attackTargetGroundList.Add(nextGround);
				nextGround = Singleton<GroundManager>.instance.getNextGround(nextGround);
			}
			Singleton<AudioManager>.instance.playEffectSound("skill_dragon", AudioManager.EffectType.Skill);
			for (int i = 0; i < attackTargetGroundList.Count; i++)
			{
				MovingObject.Direction moveDirection = MovingObject.calculateDirection(attackTargetGroundList[i].inPoint.position, attackTargetGroundList[i].outPoint.position);
				int direction = ((moveDirection == MovingObject.Direction.Right) ? 1 : (-1));
				Vector3 spawnPosition = attackTargetGroundList[i].outPoint.position;
				spawnPosition.x += direction * 3;
				spawnPosition.z = -i;
				ObjectPool.Spawn("@ReinforcementDragonBreath", spawnPosition).GetComponent<ReinforcementDragonBreath>().initDragon(attackTargetGroundList[i]);
				yield return new WaitForSeconds(0.15f);
			}
		}
		yield return null;
	}

	private IEnumerator waitForCastSkill(Action castSkillAction)
	{
		if (m_isCastingSkill)
		{
			yield break;
		}
		float timer = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime;
				if (timer >= 0.45f)
				{
					break;
				}
			}
			yield return null;
		}
		castSkillAction();
		m_isCastingSkill = false;
	}

	public string getReinforcementSkillName(SkillType skillType)
	{
		return I18NManager.Get("REINFORCEMENT_SKILL_NAME_" + (int)(skillType + 1));
	}

	public string getReinforcementSkillDescription(SkillType skillType, double chance, double value)
	{
		if (skillType == SkillType.DivineSmash || skillType == SkillType.WhirlWind || skillType == SkillType.DragonsBreath)
		{
			value += 100.0;
		}
		return string.Format(I18NManager.Get("REINFORCEMENT_SKILL_DESCRIPTION_" + (int)(skillType + 1)), chance, value);
	}

	public long getReinforcementUnlockPrice(SkillType skillType)
	{
		return 1000L;
	}

	public long getReinforcementUpgradePrice(SkillType skillType, long skillLevel)
	{
		return 500L;
	}

	public long getReinforcementMaxLevel()
	{
		return 10L;
	}

	private void resetPassiveSkillTimer(PassiveSkillType skillType)
	{
		m_currentPassiveSkillTimer[skillType] = 0f;
	}

	private void resetAllPassiveSkillTimer()
	{
		for (int i = 0; i < 3; i++)
		{
			m_currentPassiveSkillTimer[(PassiveSkillType)i] = 0f;
		}
	}

	public void touchToCastPassiveSkill()
	{
		for (int i = 0; i < 3; i++)
		{
			PassiveSkillType skillType = (PassiveSkillType)i;
			if (getPassiveSkillInventoryData(skillType).isUnlocked)
			{
				checkChanceForPassiveSkill(skillType, 1f);
			}
		}
	}

	public void checkChanceForPassiveSkill(PassiveSkillType skillType, float multiply = 1f)
	{
		if (getPassiveSkillInventoryData(skillType).isUnlocked)
		{
			MersenneTwister mersenneTwister = new MersenneTwister();
			double num = mersenneTwister.Next(0, 100000);
			num /= 1000.0;
			if (num <= getPassiveSkillCastChance(skillType) * (double)multiply)
			{
				castPassiveSkill(skillType);
			}
		}
	}

	public void castPassiveSkill(PassiveSkillType skillType)
	{
		IEnumerator passiveSkillCoroutine = getPassiveSkillCoroutine(skillType);
		if (passiveSkillCoroutine != null)
		{
			playPassiveSkillEffect(skillType);
			resetPassiveSkillTimer(skillType);
			if (!m_currentCastingPassiveSkillList.Contains(skillType))
			{
				m_currentCastingPassiveSkillList.Add(skillType);
				StartCoroutine(passiveSkillCoroutine);
			}
		}
	}

	private void playPassiveSkillEffect(PassiveSkillType skillType)
	{
		if (skillType == PassiveSkillType.MeteorRain)
		{
			Singleton<AudioManager>.instance.playEffectSound("skill_meteor");
		}
	}

	private IEnumerator getPassiveSkillCoroutine(PassiveSkillType skillType)
	{
		IEnumerator result = null;
		switch (skillType)
		{
		case PassiveSkillType.FrostSkill:
			result = frostSkillUpdate();
			break;
		case PassiveSkillType.MeteorRain:
			result = meteorRainUpdate();
			break;
		}
		return result;
	}

	private IEnumerator frostSkillUpdate()
	{
		m_currentCastingPassiveSkillList.Remove(PassiveSkillType.FrostSkill);
		Ground currentWarriorStayingGround = Singleton<CharacterManager>.instance.warriorCharacter.currentGround;
		if (currentWarriorStayingGround == null || currentWarriorStayingGround.isFirstGround)
		{
			yield break;
		}
		List<Ground> frostGroundList = new List<Ground>
		{
			currentWarriorStayingGround
		};
		Ground nextGround = Singleton<GroundManager>.instance.getNextGround(currentWarriorStayingGround);
		while (nextGround != null && frostGroundList.Count < 1 + (int)Singleton<StatManager>.instance.frostWallExtraFloor)
		{
			frostGroundList.Add(nextGround);
			nextGround = Singleton<GroundManager>.instance.getNextGround(nextGround);
		}
		for (int i = 0; i < frostGroundList.Count; i++)
		{
			if (frostGroundList[i].isFirstGround)
			{
				frostGroundList.Remove(frostGroundList[i]);
				break;
			}
		}
		float frozenTime = getSkillDuration(PassiveSkillType.FrostSkill);
		for (int j = 0; j < frostGroundList.Count; j++)
		{
			FrozenSkillEffect frozenSkillEffect = ObjectPool.Spawn("@FrozenSkillEffect", frostGroundList[j].cachedTransform.position).GetComponent<FrozenSkillEffect>();
			FrozenSkillEffect.FrozenGroundType groundType = FrozenSkillEffect.FrozenGroundType.NormalGround;
			if (frostGroundList[j].isBossGround)
			{
				groundType = ((GameManager.currentStage % Singleton<MapManager>.instance.maxStage != 0 && GameManager.getRealThemeNumber(GameManager.currentTheme) != 10) ? FrozenSkillEffect.FrozenGroundType.MiniBossGround : FrozenSkillEffect.FrozenGroundType.BossGround);
			}
			MovingObject.Direction moveDirection = MovingObject.calculateDirection(frostGroundList[j].inPoint.position, frostGroundList[j].outPoint.position);
			frozenSkillEffect.init(groundType, frozenTime, moveDirection, frostGroundList[j].currnetFloor == 1);
			for (int k = 0; k < Singleton<EnemyManager>.instance.enemyList.Count; k++)
			{
				EnemyObject targetEnemy = Singleton<EnemyManager>.instance.enemyList[k];
				if (targetEnemy.currentGround == frostGroundList[j])
				{
					double targetDamage = getPassiveSkillValue(PassiveSkillType.FrostSkill, true);
					if (targetEnemy.isBoss || (targetEnemy.myMonsterObject != null && targetEnemy.myMonsterObject.isMiniboss))
					{
						targetDamage = getPassiveSkillValue(PassiveSkillType.FrostSkill, true, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses);
					}
					if (!targetEnemy.isBoss && !targetEnemy.isMiniboss)
					{
						targetDamage = getPassiveSkillValue(PassiveSkillType.FrostSkill, true, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters);
					}
					if (groundType != FrozenSkillEffect.FrozenGroundType.BossGround)
					{
						targetEnemy.setFrozenEnemy(frozenTime);
					}
					targetEnemy.decreasesHealth(targetDamage);
					targetEnemy.setDamageText(targetDamage, CharacterManager.CharacterType.Warrior, false, true);
				}
			}
			yield return new WaitForSeconds(0.15f);
		}
	}

	private IEnumerator meteorRainUpdate()
	{
		m_currentCastingPassiveSkillList.Remove(PassiveSkillType.MeteorRain);
		CharacterWarrior warrior = Singleton<CharacterManager>.instance.warriorCharacter;
		Ground currentWarriorStayingGround = warrior.currentGround;
		List<Ground> attackTargetGroundList = new List<Ground>
		{
			currentWarriorStayingGround
		};
		Ground nextGround = Singleton<GroundManager>.instance.getNextGround(currentWarriorStayingGround);
		while (nextGround != null && attackTargetGroundList.Count < 4)
		{
			attackTargetGroundList.Add(nextGround);
			nextGround = Singleton<GroundManager>.instance.getNextGround(nextGround);
		}
		for (int j = 0; j < attackTargetGroundList.Count; j++)
		{
			if (attackTargetGroundList[j].isFirstGround)
			{
				attackTargetGroundList.Remove(attackTargetGroundList[j]);
				break;
			}
		}
		int meteorSpawnCount = getMeteorSpawnCount();
		for (int i = 0; i < meteorSpawnCount; i++)
		{
			Ground attackTargetGround = attackTargetGroundList[UnityEngine.Random.Range(0, attackTargetGroundList.Count)];
			float minXPosition = ((!(attackTargetGround.outPoint.position.x < attackTargetGround.inPoint.position.x)) ? attackTargetGround.inPoint.position.x : attackTargetGround.outPoint.position.x);
			float maxXPosition = ((!(attackTargetGround.outPoint.position.x < attackTargetGround.inPoint.position.x)) ? attackTargetGround.outPoint.position.x : attackTargetGround.inPoint.position.x);
			Vector2 meteorTargetPosition = new Vector2(UnityEngine.Random.Range(minXPosition, maxXPosition), attackTargetGround.cachedTransform.position.y);
			if (attackTargetGround.isBossGround && Singleton<EnemyManager>.instance.bossObject != null)
			{
				meteorTargetPosition.x = UnityEngine.Random.Range(minXPosition - 3f, maxXPosition);
			}
			Vector2 meteorSpawnPosition = meteorTargetPosition + new Vector2(10f, 13f);
			Bullet meteor = ObjectPool.Spawn("@Meteor", meteorSpawnPosition).GetComponent<Bullet>();
			meteor.shootBullet(meteorTargetPosition, delegate(Bullet meteorObject)
			{
				List<EnemyObject> nearestEnemies = Singleton<EnemyManager>.instance.getNearestEnemies(meteorObject.cachedTransform.position, 2.2f);
				if (nearestEnemies != null && nearestEnemies.Count > 0)
				{
					for (int k = 0; k < nearestEnemies.Count; k++)
					{
						EnemyObject enemyObject = nearestEnemies[k];
						double passiveSkillValue = getPassiveSkillValue(PassiveSkillType.MeteorRain, true);
						if (enemyObject.isBoss || (enemyObject.myMonsterObject != null && enemyObject.myMonsterObject.isMiniboss))
						{
							passiveSkillValue = getPassiveSkillValue(PassiveSkillType.MeteorRain, true, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses);
						}
						if (!enemyObject.isBoss && !enemyObject.isMiniboss)
						{
							passiveSkillValue = getPassiveSkillValue(PassiveSkillType.MeteorRain, true, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters);
						}
						enemyObject.decreasesHealth(passiveSkillValue);
						enemyObject.setDamageText(passiveSkillValue, CharacterManager.CharacterType.Warrior, false, true);
					}
				}
				ObjectPool.Spawn("@ExplosionEffect", meteorObject.cachedTransform.position, new Vector3(90f, 0f, 0f));
				ShakeCamera.Instance.shake(1f, 0.1f);
				meteorObject.recycleBullet();
			}, Vector2.zero);
			yield return new WaitForSeconds(0.05f);
		}
	}

	public PassiveSkillInventoryData getPassiveSkillInventoryData(PassiveSkillType skillType)
	{
		return Singleton<DataManager>.instance.currentGameData.passiveSkillInventoryData[(int)skillType];
	}

	public double getPassiveSkillCastChance(PassiveSkillType passiveSkillType)
	{
		double result = 0.0;
		switch (passiveSkillType)
		{
		case PassiveSkillType.FrostSkill:
			result = 0.3 + Singleton<StatManager>.instance.frostWallExtraCastChanceFromPremiumTreasure;
			break;
		case PassiveSkillType.MeteorRain:
			result = 0.3 + Singleton<StatManager>.instance.meteorRainExtraCastChanceFromPremiumTreasure;
			break;
		}
		return result;
	}

	public double getPassiveSkillValue(PassiveSkillType passiveSkillType, bool isCalculatedWithCharacterCurrentDamage, params double[] arg)
	{
		double num = 0.0;
		PassiveSkillInventoryData passiveSkillInventoryData = getPassiveSkillInventoryData(passiveSkillType);
		double num2 = Singleton<CharacterManager>.instance.warriorCharacter.getCurrentDamage(arg) + Singleton<CharacterManager>.instance.priestCharacter.getCurrentDamage(arg) + Singleton<CharacterManager>.instance.archerCharacter.getCurrentDamage(arg);
		long num3 = (long)Mathf.Max(passiveSkillInventoryData.skillLevel, 1f);
		switch (passiveSkillType)
		{
		case PassiveSkillType.FrostSkill:
			num = 100 + (num3 - 1) * 10;
			break;
		case PassiveSkillType.MeteorRain:
			num = 100 + (num3 - 1) * 10;
			break;
		case PassiveSkillType.SwordSoul:
			num = 100 + (num3 - 1) * 100;
			break;
		}
		if (isCalculatedWithCharacterCurrentDamage)
		{
			num += num / 100.0 * Singleton<StatManager>.instance.skillExtraPercentDamage;
			num = num2 / 100.0 * num;
		}
		return num;
	}

	public float getSkillDuration(PassiveSkillType passiveSkillType)
	{
		float result = 0f;
		if (passiveSkillType == PassiveSkillType.FrostSkill)
		{
			result = 1.5f + (float)Singleton<StatManager>.instance.frostSkillExtraDurationFromPremiumTreasure;
		}
		return result;
	}

	public long getPassiveSkillUnlockRubyPrice(PassiveSkillType passiveskillType)
	{
		long result = 0L;
		switch (passiveskillType)
		{
		case PassiveSkillType.FrostSkill:
			result = 2000L;
			break;
		case PassiveSkillType.MeteorRain:
			result = 2000L;
			break;
		case PassiveSkillType.SwordSoul:
			result = 2000L;
			break;
		}
		return result;
	}

	public long getPassiveSkillUpgradeRubyPrice(PassiveSkillType passiveSkillType)
	{
		long result = 0L;
		PassiveSkillInventoryData passiveSkillInventoryData = getPassiveSkillInventoryData(passiveSkillType);
		long num = (long)Mathf.Max(passiveSkillInventoryData.skillLevel, 1f);
		switch (passiveSkillType)
		{
		case PassiveSkillType.FrostSkill:
			result = 10 + (num - 1) * 20;
			break;
		case PassiveSkillType.MeteorRain:
			result = 10 + (num - 1) * 20;
			break;
		case PassiveSkillType.SwordSoul:
			result = 2000L;
			break;
		}
		return result;
	}

	public long getPassiveSkillMaxLevel(PassiveSkillType passiveSkillType)
	{
		long result = long.MaxValue;
		if (passiveSkillType == PassiveSkillType.SwordSoul)
		{
			result = 10L;
		}
		return result;
	}

	public int getMeteorSpawnCount()
	{
		return 15 + (int)Singleton<StatManager>.instance.meteorRainExtraSpawnCountFromPremiumTreasure;
	}
}
