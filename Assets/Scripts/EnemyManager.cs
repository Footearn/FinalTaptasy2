using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
	public enum BossType
	{
		QueenHornet,
		GreatGolem,
		KingLizard,
		KingScorpion,
		KingMonkfish,
		Icequeen,
		FairyKing,
		Lavadragon,
		GrimReaper,
		Daemon1
	}

	[Serializable]
	public struct BossMonsterDetailAttributeData
	{
		public BossType bossType;

		public Vector2 localPosition;

		public Vector3 localScale;

		public AnimationNameData animationNameData;
	}

	public enum MonsterType
	{
		Snail1,
		Snail2,
		Snail3,
		SnailBoss,
		Mushroom1,
		Mushroom2,
		Mushroom3,
		MushroomBoss,
		Ginseng1,
		Ginseng2,
		Ginseng3,
		GinsengBoss,
		Frog1,
		Frog2,
		Frog3,
		FrogBoss,
		Hornet1,
		Hornet2,
		Hornet3,
		HornetBoss,
		Wolf1,
		Wolf2,
		Wolf3,
		WolfBoss,
		Orc1,
		Orc2,
		Orc3,
		OrcBoss,
		Golem1,
		Golem2,
		Golem3,
		GolemBoss,
		Goblin1,
		Goblin2,
		Goblin3,
		GoblinBoss,
		Bat1,
		Bat2,
		Bat3,
		BatBoss,
		Lizard1,
		Lizard2,
		Lizard3,
		LizardBoss,
		Anaconda1,
		Anaconda2,
		Anaconda3,
		AnacondaBoss,
		Tiger1,
		Tiger2,
		Tiger3,
		TigerBoss,
		Oldbird1,
		Oldbird2,
		Oldbird3,
		OldbirdBoss,
		MudGolem1,
		MudGolem2,
		MudGolem3,
		MudGolemBoss,
		Anubis1,
		Anubis2,
		Anubis3,
		AnubisBoss,
		Spider1,
		Spider2,
		Spider3,
		SpiderBoss,
		DesertFox1,
		DesertFox2,
		DesertFox3,
		DesertFoxBoss,
		Mummy1,
		Mummy2,
		Mummy3,
		MummyBoss,
		Scorpion1,
		Scorpion2,
		Scorpion3,
		ScorpionBoss,
		Crab1,
		Crab2,
		Crab3,
		CrabBoss,
		Clam1,
		Clam2,
		Clam3,
		ClamBoss,
		Monkfish1,
		Monkfish2,
		Monkfish3,
		MonkfishBoss,
		Squid1,
		Squid2,
		Squid3,
		SquidBoss,
		Uparupa1,
		Uparupa2,
		Uparupa3,
		UparupaBoss,
		PolarBear1,
		PolarBear2,
		PolarBear3,
		PolarBearBoss,
		Arcticfox1,
		Arcticfox2,
		Arcticfox3,
		ArcticfoxBoss,
		IceGolem1,
		IceGolem2,
		IceGolem3,
		IceGolemBoss,
		Icespirit1,
		Icespirit2,
		Icespirit3,
		IcespiritBoss,
		Yeti1,
		Yeti2,
		Yeti3,
		YetiBoss,
		Slime1,
		Slime2,
		Slime3,
		SlimeBoss,
		Boar1,
		Boar2,
		Boar3,
		BoarBoss,
		Woodspirit1,
		Woodspirit2,
		Woodspirit3,
		WoodspiritBoss,
		Drosera1,
		Drosera2,
		Drosera3,
		DroseraBoss,
		Fairy1,
		Fairy2,
		Fairy3,
		FairyBoss,
		Salamander1,
		Salamander2,
		Salamander3,
		SalamanderBoss,
		Lavagolem1,
		Lavagolem2,
		Lavagolem3,
		LavagolemBoss,
		WingedEye1,
		WingedEye2,
		WingedEye3,
		WingedEyeBoss,
		Flamespirit1,
		Flamespirit2,
		Flamespirit3,
		FlamespiritBoss,
		Babydragon1,
		Babydragon2,
		Babydragon3,
		BabydragonBoss,
		Skeleton1,
		Skeleton2,
		Skeleton3,
		SkeletonBoss,
		Undead1,
		Undead2,
		Undead3,
		UndeadBoss,
		HellHound1,
		HellHound2,
		HellHound3,
		HellHoundBoss,
		Harpy1,
		Harpy2,
		Harpy3,
		HarpyBoss,
		Reaper1,
		Reaper2,
		Reaper3,
		ReaperBoss
	}

	[Serializable]
	public struct MonsterDetailAttributeData
	{
		public MonsterType monsterType;

		public Vector2 localPosition;

		public Vector3 localScale;

		public bool isRangedMonster;

		public AnimationNameData animationNameData;
	}

	public enum SpecialType
	{
		Mimic1,
		Mimic2,
		Dummy,
		Mimic3,
		Length
	}

	public Dictionary<int, List<MonsterType>> normalMonsterList = new Dictionary<int, List<MonsterType>>();

	public Dictionary<int, List<MonsterType>> miniBossesList = new Dictionary<int, List<MonsterType>>();

	public Dictionary<int, List<BossType>> realBossesList = new Dictionary<int, List<BossType>>();

	public List<MonsterDetailAttributeData> cachedMonsterTransformData;

	public List<BossMonsterDetailAttributeData> cachedBossMonsterTransformData;

	public Dictionary<MonsterType, MonsterDetailAttributeData> currentMonsterDetailAttributeDictionary;

	public Dictionary<BossType, BossMonsterDetailAttributeData> currentBossMonsterDetailAttributeDictionary;

	public EnemyObject currentBossObject;

	public bool isBoss;

	public bool isBossDead;

	public int maxTime = 30;

	public static int bossStageFloor = 101;

	public static int miniBossFloorForOverStages201 = 15;

	public List<EnemyObject> enemyList;

	public EnemyObject bossObject;

	public Sprite[] frogAttackSprites;

	public Sprite[] mummy1AttackSprites;

	public Sprite[] mummy2AttackSprites;

	public Sprite[] mummy3AttackSprites;

	public Sprite[] mummyBossAttackSprites;

	[ContextMenu("Set Boss Monster Attribute")]
	private void SetBossMonsterAttributes()
	{
		cachedBossMonsterTransformData = new List<BossMonsterDetailAttributeData>();
		EnemyObject[] array = Resources.LoadAll<EnemyObject>("Prefabs/Ingame/Bosses");
		for (int i = 0; i < array.Length; i++)
		{
			BossObject bossObject = array[i] as BossObject;
			BossMonsterDetailAttributeData item = default(BossMonsterDetailAttributeData);
			item.bossType = bossObject.currentBossType;
			item.localPosition = bossObject.cachedSpriteAnimation.cachedTransform.localPosition;
			item.localScale = bossObject.cachedSpriteAnimation.cachedTransform.localScale;
			item.animationNameData = bossObject.currentAnimationName;
			cachedBossMonsterTransformData.Add(item);
		}
	}

	private void Awake()
	{
		currentMonsterDetailAttributeDictionary = new Dictionary<MonsterType, MonsterDetailAttributeData>();
		for (int i = 0; i < cachedMonsterTransformData.Count; i++)
		{
			currentMonsterDetailAttributeDictionary.Add(cachedMonsterTransformData[i].monsterType, cachedMonsterTransformData[i]);
		}
		currentBossMonsterDetailAttributeDictionary = new Dictionary<BossType, BossMonsterDetailAttributeData>();
		for (int j = 0; j < cachedBossMonsterTransformData.Count; j++)
		{
			currentBossMonsterDetailAttributeDictionary.Add(cachedBossMonsterTransformData[j].bossType, cachedBossMonsterTransformData[j]);
		}
		enemyList = new List<EnemyObject>();
		isBossDead = false;
		sortMonsterType();
	}

	public void startGame()
	{
		isBossDead = false;
		bossObject = null;
	}

	public void startBoss()
	{
		isBoss = true;
	}

	public void endBossSafe()
	{
		isBoss = false;
		isBossDead = true;
		bossObject = null;
	}

	public MonsterObject spawnMonster(MonsterType type, Vector2 spawnPosition, MovingObject.Direction lookDirection, int level)
	{
		MonsterObject monsterObject = null;
		monsterObject = ObjectPool.Spawn("@MonsterObject", spawnPosition).GetComponent<MonsterObject>();
		bool isEliteMonster = false;
		if (GameManager.currentTheme > 100)
		{
			double num = UnityEngine.Random.Range(0, 10000);
			num /= 100.0;
			if (num <= 5.0)
			{
				isEliteMonster = true;
			}
		}
		monsterObject.isEliteMonster = isEliteMonster;
		monsterObject.cachedSpriteAnimation.stopAnimation();
		monsterObject.currentMonsterType = type;
		monsterObject.setDirection(lookDirection);
		monsterObject.cachedTransform.SetParent(Singleton<CachedManager>.instance.monster);
		monsterObject.isBoss = false;
		monsterObject.isMiniboss = false;
		monsterObject.init(level);
		monsterObject.setState(PublicDataManager.State.Move);
		return monsterObject;
	}

	public MonsterObject spawnMiniBoss(MonsterType type, Vector2 spawnPosition, MovingObject.Direction lookDirection, int level, bool isBoss)
	{
		MonsterObject monsterObject = null;
		monsterObject = ObjectPool.Spawn("@MonsterObject", spawnPosition).GetComponent<MonsterObject>();
		monsterObject.currentMonsterType = type;
		if (GameManager.currentTheme == 10 && GameManager.currentStage % 10 == 0)
		{
			monsterObject.cachedTransform.localScale = Vector2.one * 1.5f;
		}
		else
		{
			monsterObject.cachedTransform.localScale = Vector2.one * 2f;
		}
		monsterObject.isEliteMonster = false;
		monsterObject.setDirection(lookDirection);
		monsterObject.cachedTransform.SetParent(Singleton<CachedManager>.instance.boss);
		monsterObject.isBoss = isBoss;
		currentBossObject = monsterObject;
		if (isBoss)
		{
			Singleton<EnemyManager>.instance.bossObject = monsterObject;
		}
		monsterObject.isMiniboss = true;
		monsterObject.init(level);
		return monsterObject;
	}

	public BossObject spawnBoss(BossType type, Vector2 spawnPosition, MovingObject.Direction lookDirection, int level, bool isBoss, Ground targetGround = null)
	{
		BossObject component = ObjectPool.Spawn("@BossObject", spawnPosition).GetComponent<BossObject>();
		component.isBoss = isBoss;
		component.cachedTransform.localScale = Vector2.one * 1.2f;
		component.setDirection(lookDirection);
		component.cachedTransform.SetParent(Singleton<CachedManager>.instance.boss);
		currentBossObject = component;
		bossObject = component;
		component.isCanAttack = false;
		component.currentBossType = type;
		component.init(level);
		return component;
	}

	public void recycleEnemy(EnemyObject enemy)
	{
		ObjectPool.Recycle(enemy.name, enemy.cachedGameObject);
		enemyList.Remove(enemy);
	}

	public int getCurrentMonsterLevel()
	{
		return Singleton<DataManager>.instance.currentGameData.unlockTheme * Singleton<MapManager>.instance.maxStage + Singleton<DataManager>.instance.currentGameData.unlockStage;
	}

	private void sortMonsterType()
	{
		List<MonsterType> list = new List<MonsterType>();
		List<MonsterType> list2 = new List<MonsterType>();
		List<BossType> list3 = new List<BossType>();
		List<MonsterType> list4 = new List<MonsterType>();
		List<MonsterType> list5 = new List<MonsterType>();
		List<BossType> list6 = new List<BossType>();
		int num = 1;
		int num2 = 1;
		for (int i = 0; i < 180; i++)
		{
			if (num % 4 == 0)
			{
				list5.Add((MonsterType)i);
				list2.Add((MonsterType)i);
				num = 1;
			}
			else
			{
				list4.Add((MonsterType)i);
				list.Add((MonsterType)i);
				num++;
			}
			if ((i + 1) % 20 == 0)
			{
				normalMonsterList.Add(num2, list4);
				miniBossesList.Add(num2, list5);
				list4 = new List<MonsterType>();
				list5 = new List<MonsterType>();
				num2++;
			}
		}
		for (int j = 0; j < 10; j++)
		{
			if (j != 9)
			{
				list6.Add((BossType)j);
				realBossesList.Add(j + 1, list6);
				list6 = new List<BossType>();
			}
			list3.Add((BossType)j);
		}
		normalMonsterList.Add(10, list);
		miniBossesList.Add(10, list2);
		realBossesList.Add(10, list3);
	}

	public Sprite getMonsterIconSprite(MonsterType monsterType)
	{
		Sprite result = null;
		Sprite[] array = null;
		string animation = monsterType.ToString();
		array = Singleton<ResourcesManager>.instance.getAnimation(animation);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].name.Equals("Idle_1"))
			{
				result = array[i];
				break;
			}
		}
		return result;
	}

	public Sprite getBossIconSprite(BossType bossType, bool isEliteBoss)
	{
		Sprite result = null;
		Sprite[] array = null;
		string text = bossType.ToString();
		if (bossType >= BossType.FairyKing && isEliteBoss)
		{
			text += "Elite";
		}
		array = Singleton<ResourcesManager>.instance.getAnimation(text);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].name.Equals("Idle_1"))
			{
				result = array[i];
				break;
			}
		}
		return result;
	}

	public EnemyObject getNearestEnemy(Vector2 startPosition)
	{
		return getNearestEnemy(startPosition, float.MaxValue);
	}

	public EnemyObject getNearestEnemy(Vector2 startPosition, float range)
	{
		EnemyObject result = null;
		float num = float.MaxValue;
		for (int i = 0; i < enemyList.Count; i++)
		{
			if (enemyList[i].isDead || !(Mathf.Abs(enemyList[i].cachedTransform.position.y - startPosition.y) <= 0.8f))
			{
				continue;
			}
			float num2 = Vector2.Distance(startPosition, enemyList[i].cachedTransform.position);
			if (num2 <= num)
			{
				num = num2;
				if (num <= range)
				{
					result = enemyList[i];
				}
			}
		}
		return result;
	}

	public EnemyObject getBestNearestEnemy(Vector2 startPosition, float range)
	{
		EnemyObject result = null;
		float num = float.MaxValue;
		for (int i = 0; i < enemyList.Count; i++)
		{
			if (enemyList[i].isDead)
			{
				continue;
			}
			float num2 = Vector2.Distance(startPosition, enemyList[i].cachedTransform.position);
			if (num2 <= num)
			{
				num = num2;
				if (num <= range)
				{
					result = enemyList[i];
				}
			}
		}
		return result;
	}

	public EnemyObject getNearestEnemyforWarrior(Vector2 startPosition)
	{
		EnemyObject result = null;
		float num = float.MaxValue;
		float num2 = 1f;
		for (int i = 0; i < enemyList.Count; i++)
		{
			if (enemyList[i].isDead || !(Mathf.Abs(enemyList[i].cachedTransform.position.y - startPosition.y) <= 0.8f))
			{
				continue;
			}
			float num3 = Vector2.Distance(startPosition, enemyList[i].cachedTransform.position);
			if (!(num3 <= num))
			{
				continue;
			}
			num = num3;
			if (enemyList[i] is MonsterObject)
			{
				num2 = ((!((MonsterObject)enemyList[i]).isMiniboss) ? 1.2f : 1.8f);
				if (num <= num2)
				{
					result = enemyList[i];
				}
			}
			else if (enemyList[i] is SpecialObject)
			{
				num2 = 1.2f;
				if (num <= num2)
				{
					result = enemyList[i];
				}
			}
			else
			{
				num2 = 2f;
				if (num <= num2)
				{
					result = enemyList[i];
				}
			}
		}
		return result;
	}

	public List<EnemyObject> getNearestEnemies(Vector2 startPosition, float range)
	{
		List<EnemyObject> list = new List<EnemyObject>();
		float num = float.MaxValue;
		for (int i = 0; i < enemyList.Count; i++)
		{
			if (!enemyList[i].isDead && Mathf.Abs(enemyList[i].cachedTransform.position.y - startPosition.y) <= 0.8f)
			{
				num = Vector2.Distance(startPosition, enemyList[i].cachedTransform.position);
				if (num <= range)
				{
					list.Add(enemyList[i]);
				}
			}
		}
		return list;
	}

	public List<EnemyObject> getNearestEnemiesIgnoreFloor(Vector2 startPosition, float range)
	{
		List<EnemyObject> list = new List<EnemyObject>();
		float num = float.MaxValue;
		for (int i = 0; i < enemyList.Count; i++)
		{
			if (!enemyList[i].isDead)
			{
				num = Vector2.Distance(startPosition, enemyList[i].cachedTransform.position);
				if (num <= range)
				{
					list.Add(enemyList[i]);
				}
			}
		}
		return list;
	}

	public MonsterType getMonsterTypeByTheme(int themeNumber)
	{
		return normalMonsterList[GameManager.getRealThemeNumber(themeNumber)][UnityEngine.Random.Range(0, normalMonsterList[GameManager.getRealThemeNumber(themeNumber)].Count)];
	}

	public MonsterType getMiniBossTypeByTheme(int themeNumber, int currentStage)
	{
		MonsterType monsterType = MonsterType.SnailBoss;
		if (GameManager.currentTheme > 200)
		{
			return miniBossesList[10][UnityEngine.Random.Range(0, miniBossesList[10].Count)];
		}
		return miniBossesList[GameManager.getRealThemeNumber(themeNumber)][(currentStage - 1) % miniBossesList[GameManager.getRealThemeNumber(themeNumber)].Count];
	}

	public BossType getBossTypeByTheme(int themeNumber, int stageNumber)
	{
		int realThemeNumber = GameManager.getRealThemeNumber(themeNumber);
		if (realThemeNumber == 10)
		{
			return realBossesList[realThemeNumber][stageNumber - 1];
		}
		return realBossesList[realThemeNumber][0];
	}

	public AttributeBaseStat getMonsterBaseStatForBossRaid(MonsterType monsterType, long currentStage)
	{
		if (!ParsingManager.isLoaded)
		{
			return default(AttributeBaseStat);
		}
		MonsterStatData monsterStatData = Singleton<ParsingManager>.instance.currentParsedStatData.monsterStatData[monsterType];
		AttributeBaseStat result = default(AttributeBaseStat);
		result.baseDamage = 10.0 + Math.Pow(1.05, currentStage) + Math.Pow(currentStage, 1.3);
		result.baseHealth = 500.0 + Math.Pow(1.1, currentStage) + 150.0 * Math.Pow(currentStage, 1.3);
		result.baseDamage *= monsterStatData.baseDelay;
		result.baseDelay = monsterStatData.baseDelay;
		result.baseSpeed = monsterStatData.baseSpeed;
		result.attackRange = monsterStatData.baseRange;
		return result;
	}

	public AttributeBaseStat getMonsterBaseStatForBossRaid(BossType bossType, long currentStage)
	{
		if (!ParsingManager.isLoaded)
		{
			return default(AttributeBaseStat);
		}
		BossStatData bossStatData = Singleton<ParsingManager>.instance.currentParsedStatData.bossStatData[bossType];
		AttributeBaseStat result = default(AttributeBaseStat);
		result.baseDamage = 10.0 + Math.Pow(1.05, currentStage) + Math.Pow(currentStage, 1.3);
		result.baseHealth = 500.0 + Math.Pow(1.1, currentStage) + 150.0 * Math.Pow(currentStage, 1.3);
		result.baseDamage *= bossStatData.baseDelay;
		result.baseDelay = bossStatData.baseDelay;
		result.baseSpeed = bossStatData.baseSpeed;
		result.attackRange = bossStatData.baseRange;
		return result;
	}

	public AttributeBaseStat getMonsterBaseStat(MonsterType type, long currentTheme, long currentStage, bool isMiniBoss)
	{
		if (!ParsingManager.isLoaded)
		{
			return default(AttributeBaseStat);
		}
		MonsterStatData monsterStatData = Singleton<ParsingManager>.instance.currentParsedStatData.monsterStatData[type];
		AttributeBaseStat result = default(AttributeBaseStat);
		if (!isMiniBoss)
		{
			result.baseDamage = (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterDamageVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterDamageVariable2), currentTheme - 1) * (1.0 + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterDamageVariable3) * ((double)currentStage + (double)(currentTheme - 1) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterDamageVariable4))) - 1.0) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterDamageAllMultyply);
			result.baseHealth = (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterHPVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterHPVariable2), currentTheme - 1) * (1.0 + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterHPVariable3) * ((double)currentStage + (double)(currentTheme - 1) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterHPVariable4))) - 1.0) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterHPAllMultiply) * Math.Ceiling((double)currentTheme / 10.0);
			result.baseDamage *= monsterStatData.baseDelay;
			result.baseDelay = monsterStatData.baseDelay;
			result.baseSpeed = monsterStatData.baseSpeed;
			result.attackRange = monsterStatData.baseRange;
		}
		else
		{
			result.baseDamage = (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterDamageVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterDamageVariable2), currentTheme - 1) * (1.0 + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterDamageVariable3) * ((double)currentStage + (double)(currentTheme - 1) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterDamageVariable4))) - 1.0) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.MiniBossDamageAllMultyply);
			result.baseHealth = (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterHPVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterHPVariable2), currentTheme - 1) * (1.0 + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterHPVariable3) * ((double)currentStage + (double)(currentTheme - 1) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.NormalMonsterHPVariable4))) - 1.0) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.MiniBossHPAllMultiply) * Math.Ceiling((double)currentTheme / 10.0);
			result.baseDamage *= Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.MiniBossDamageVariable1);
			result.baseHealth *= Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.MiniBossHPVariable1);
			result.baseDamage *= monsterStatData.baseDelay;
			result.baseDelay = monsterStatData.baseDelay;
			result.baseSpeed = monsterStatData.baseSpeed;
			result.attackRange = monsterStatData.baseRange;
			if (GameManager.currentTheme > 200)
			{
				result.baseHealth *= 3.0;
			}
		}
		if (TutorialManager.isTutorial)
		{
			result.baseHealth *= 0.30000001192092896;
		}
		if (GameManager.currentTheme > 200)
		{
			result.baseDamage /= 4.0;
			result.baseDelay /= 4f;
		}
		return result;
	}

	public AttributeBaseStat getBossBaseStat(BossType type, long currentTheme, long currentStage)
	{
		if (!ParsingManager.isLoaded)
		{
			return default(AttributeBaseStat);
		}
		BossStatData bossStatData = Singleton<ParsingManager>.instance.currentParsedStatData.bossStatData[type];
		AttributeBaseStat result = default(AttributeBaseStat);
		result.baseDamage = (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossDamageVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossDamageVariable2), currentTheme - 1) * (1.0 + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossDamageVariable3) * ((double)currentStage + (double)(currentTheme - 1) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossDamageVariable4))) - 1.0) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossDamageVariable5) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossDamageAllMultyply);
		result.baseHealth = (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossHPVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossHPVariable2), currentTheme - 1) * (1.0 + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossHPVariable3) * ((double)currentStage + (double)(currentTheme - 1) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossHPVariable4))) - 1.0) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossHPVariable5) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.BossHPAllMultiply) * Math.Ceiling((double)currentTheme / 10.0);
		result.baseDamage *= bossStatData.baseDelay;
		result.baseDelay = bossStatData.baseDelay;
		result.baseSpeed = bossStatData.baseSpeed;
		result.attackRange = bossStatData.baseRange;
		if (type == BossType.Daemon1)
		{
			result.baseDamage *= Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.DaemonKingDamageVariable1);
			result.baseHealth *= Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.DaemonKingHPVariable);
		}
		if (GameManager.currentTheme > 200)
		{
			result.baseHealth *= 3.0;
		}
		if (TutorialManager.isTutorial)
		{
			result.baseHealth *= 0.30000001192092896;
		}
		if (GameManager.currentTheme > 200)
		{
			result.baseDamage /= 4.0;
			result.baseDelay /= 4f;
		}
		return result;
	}

	public AttributeBaseStat getSpecialMonsterBaseStat()
	{
		return default(AttributeBaseStat);
	}

	public Vector2 getIconLocalPosforStageSlot(string bossType)
	{
		switch ((int)Enum.Parse(typeof(MonsterType), bossType))
		{
		case 3:
			return new Vector2(34f, -12.3f);
		case 7:
			return new Vector2(1f, 1f);
		case 11:
			return new Vector2(3.2f, 0.3f);
		case 15:
			return new Vector2(-0.5f, -37.3f);
		case 19:
			return new Vector2(9.8f, -27f);
		case 23:
			return new Vector2(20.1f, -21.8f);
		case 27:
			return new Vector2(16.1f, -13f);
		case 31:
			return new Vector2(-4.5f, -19f);
		case 35:
			return new Vector2(16.8f, -16.8f);
		case 39:
			return new Vector2(13.1f, 11.9f);
		case 43:
			return new Vector2(15.2f, -24.7f);
		case 47:
			return new Vector2(20.1f, -23.3f);
		case 51:
			return new Vector2(15.2f, -8.5f);
		case 55:
			return new Vector2(29.3f, -20.4f);
		case 59:
			return new Vector2(13.1f, -23.2f);
		case 63:
			return new Vector2(7.5f, -3.5f);
		case 67:
			return new Vector2(6.1f, -16.2f);
		case 71:
			return new Vector2(27.9f, -19.7f);
		case 75:
			return new Vector2(18.1f, -9.9f);
		case 79:
			return new Vector2(10.2f, 17.4f);
		case 83:
			return new Vector2(6.7f, -15.2f);
		case 87:
			return new Vector2(8.5f, -3.5f);
		case 91:
			return new Vector2(15.5f, -5.5f);
		case 95:
			return new Vector2(11.1f, -0.2f);
		case 99:
			return new Vector2(3.2f, -3.7f);
		case 103:
			return new Vector2(5f, -9f);
		case 107:
			return new Vector2(12.9f, -7.2f);
		case 111:
			return new Vector2(4.1f, -9.8f);
		case 115:
			return new Vector2(2.3f, -5.4f);
		case 119:
			return new Vector2(1.4f, -8f);
		case 123:
			return new Vector2(0.5f, -4.5f);
		case 127:
			return new Vector2(10.2f, 2.5f);
		case 135:
			return new Vector2(0.5f, -8.1f);
		case 131:
			return new Vector2(18.1f, 5.1f);
		case 139:
			return new Vector2(4.9f, -3.7f);
		case 143:
			return new Vector2(26f, -28.4f);
		case 147:
			return new Vector2(19f, -19.6f);
		case 151:
			return new Vector2(7.5f, 3.3f);
		case 155:
			return new Vector2(11f, -0.2f);
		case 159:
			return new Vector2(13.6f, -10.8f);
		case 163:
			return new Vector2(-5.8f, -10.8f);
		case 167:
			return new Vector2(-5.8f, -10.8f);
		case 171:
			return new Vector2(18.8f, -16.1f);
		case 175:
			return new Vector2(14.4f, -9.9f);
		case 179:
			return new Vector2(9.1f, -9.9f);
		default:
			DebugManager.LogError("Impossible");
			return Vector2.zero;
		}
	}

	public Vector2 getBossIconLocalPosforStageSlot(string bossType)
	{
		switch ((int)Enum.Parse(typeof(BossType), bossType))
		{
		case 0:
			return new Vector2(29.7f, -25.2f);
		case 1:
			return new Vector2(6.2f, -31.4f);
		case 2:
			return new Vector2(15.8f, -35.1f);
		case 3:
			return new Vector2(17.2f, 24.5f);
		case 4:
			return new Vector2(17.2f, -7.9f);
		case 5:
			return new Vector2(11.3f, -35.1f);
		case 6:
			return new Vector2(6.1f, -26.3f);
		case 7:
			return new Vector2(30.4f, 26f);
		case 8:
			return new Vector2(4.6f, -14.5f);
		case 9:
			return new Vector2(19.5f, -19.7f);
		default:
			DebugManager.LogError("Impossible");
			return Vector2.zero;
		}
	}
}
