using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using UnityEngine;
using UnityEngine.UI;

public class BossRaidManager : Singleton<BossRaidManager>
{
	public enum BossRaidChestType
	{
		Null,
		Bronze,
		Gold,
		Dia
	}

	[Serializable]
	public struct BossRaidChestData
	{
		public BossRaidChestType chestType;

		public long monterLevelWhenDrop;

		public long stageWhenDrop;
	}

	[Serializable]
	public struct BossRaidBestRecordData
	{
		public long stage;

		public long monsterLevel;

		public bool isInitialized;

		public bool isBossMonster;

		public bool isMiniBossMonster;

		public EnemyManager.MonsterType monsterType;

		public EnemyManager.BossType bossType;
	}

	[Serializable]
	public class ChestRewardData
	{
		public BossRaidChestType chestType;

		public BossRaidChestRewardType targetRewardType;

		public double value;

		public CharacterManager.CharacterType characterType = CharacterManager.CharacterType.Length;

		public CharacterSkinManager.WarriorSkinType warriorCharacterSkinType = CharacterSkinManager.WarriorSkinType.Length;

		public CharacterSkinManager.PriestSkinType priestCharacterSkinType = CharacterSkinManager.PriestSkinType.Length;

		public CharacterSkinManager.ArcherSkinType archerCharacterSkinType = CharacterSkinManager.ArcherSkinType.Length;
	}

	public enum BossRaidChestRewardType
	{
		Null,
		Gold,
		Ruby,
		CharacterSkin,
		TreasureEnchantStone,
		TreasureKey,
		Length
	}

	[Serializable]
	private struct SpawnMonsterDataForBossRaid
	{
		public int monsterTargetTheme;

		public bool isLastMonster;

		public bool isBossMonster;

		public bool isMiniBossMonster;

		public EnemyManager.MonsterType monsterType;

		public EnemyManager.BossType bossType;
	}

	private enum MonsterType
	{
		NormalMonster,
		MiniBossMonster,
		RealBossMonster,
		Length
	}

	public const double INTERVAL_BETWEEN_CHARGE_HEART_TIME = 1200.0;

	public static int maxHeart = 3;

	public Text stageText;

	public Animation stageTextAnimation;

	public long currentStageForBossRaid;

	public CanvasGroup clearTitleCanvasGroup;

	public Animation clearTitleAnimation;

	public Animation stageInformationAnimation;

	public Sprite fillHeartSprite;

	public Sprite emptyHeartSprite;

	public static bool isBossRaid;

	public float intervalBetweenBossRaidBackground;

	public float intervalBetweenBossRaidMonsters;

	public float intervalBetweenMiniBossAndMonster;

	public float intervalBetweenBossAndMonster;

	public BossRaidBestRecordData lastKillBossData;

	public float maxCameraClampXPos;

	private double m_goldChestExtraChanceForMiniBoss;

	private double m_diaChestExtraChanceForMiniBoss;

	private double m_goldChestExtraChanceForBoss;

	private double m_diaChestExtraChanceForBoss;

	private List<BossRaidBackgroundObject> m_currentActivatingGroundList = new List<BossRaidBackgroundObject>();

	private List<SpawnMonsterDataForBossRaid> m_sortedMonsterList = new List<SpawnMonsterDataForBossRaid>();

	private float m_currentTargetSpawnGroundXPos;

	private int m_currentTheme;

	private int m_currentMonsterIndex;

	private Vector2 m_nextMonsterSpawnPosition;

	private int m_currentMonsterLevel;

	private int m_currentTotalSpawnedMonsterCountForMonsterLevelUp;

	private int m_currentTotalSpawnedMonsterCountForStatLevelUp;

	private SpawnMonsterDataForBossRaid m_prevMonsterData;

	private float m_monsterSpawnStartXPos;

	public Sprite bronzeChestSprite;

	public Sprite goldChestSprite;

	public Sprite diaChestSprite;

	public Text bronzeChestCountText;

	public Text goldChestCountText;

	public Text diaChestCountText;

	public float chestDissappearTime;

	public float chestCatchRange;

	public Animation[] bronzeChestCollectAnimation;

	public Animation[] goldChestCollectAnimation;

	public Animation[] diaChestCollectAnimation;

	public RectTransform bronzeChestIcon;

	public RectTransform goldChestIcon;

	public RectTransform diaChestIcon;

	public List<BossRaidChestObject> currentBronzeChestList = new List<BossRaidChestObject>();

	public List<BossRaidChestObject> currentGoldChestList = new List<BossRaidChestObject>();

	public List<BossRaidChestObject> currentDiaChestList = new List<BossRaidChestObject>();

	public List<BossRaidChestData> collectedBronzeChestList = new List<BossRaidChestData>();

	public List<BossRaidChestData> collectedGoldChestList = new List<BossRaidChestData>();

	public List<BossRaidChestData> collectedDiaChestList = new List<BossRaidChestData>();

	private MersenneTwister m_randomForAppearChestType = new MersenneTwister();

	private MersenneTwister m_randomForAppearItem = new MersenneTwister();

	private List<BossRaidChestObject> m_cachedDynamicList = new List<BossRaidChestObject>();

	public Text heartTimerText;

	private bool m_isCalculatedHeart;

	public float intervalDoubleSpeedAppear = 60f;

	public float doubleSpeedDuration = 300f;

	public bool isDoubleSpeed;

	public RectTransform doubleSpeedButtonObject;

	public RectTransform doubleSpeedProgressingObject;

	public Animation doubleSpeedButtonAnimation;

	public Animation doubleSpeedProgressingAnimation;

	public Text doubleSpeedTimerText;

	public GameObject[] doubleSpeedParticleObjects;

	private bool m_isOpendDoubleSpeedButton;

	public bool isAlreadySeenUI;

	private void Start()
	{
		calculateHeart();
	}

	public void startBossRaid()
	{
		refreshParticleBug();
		isDoubleSpeed = false;
		Singleton<GameManager>.instance.setTimeScale(true);
		m_isCalculatedHeart = false;
		AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.EnterBossRaid);
		clearTitleAnimation.Stop();
		clearTitleCanvasGroup.alpha = 0f;
		resetAll();
		sortMonsterList();
		resetChest();
		CameraFollow.instance.maxClampPosition = new Vector2(getCameraClampMaxXPos(), -4.8f);
		CameraFollow.instance.cachedTransform.localPosition = new Vector2(0f, -4.8f);
		CameraFollow.instance.offset = new Vector2(1f, 0f);
		UIWindowIngame.instance.isCanPause = true;
		isBossRaid = true;
		for (int i = 0; i < 3; i++)
		{
			spawnNextBossRaidGround(false);
		}
		StopCoroutine("doubleSpeedAppearUpdate");
		StartCoroutine("doubleSpeedAppearUpdate", true);
		StopCoroutine("stageInformationUpdate");
		StartCoroutine("stageInformationUpdate");
		StartCoroutine("bossRaidGroundSpawnUpdate");
		StartCoroutine("monsterSpawnUpdate");
		GC.Collect();
	}

	public void endGame()
	{
		if (isBossRaid)
		{
			isDoubleSpeed = false;
		}
		Singleton<GameManager>.instance.setTimeScale(true);
		calculateHeart();
		resetAll();
		resetChest();
		isBossRaid = false;
		displayHeart();
	}

	private float getCameraClampMaxXPos()
	{
		return 340f;
	}

	private void sortMonsterList()
	{
		List<SpawnMonsterDataForBossRaid> list = new List<SpawnMonsterDataForBossRaid>();
		List<SpawnMonsterDataForBossRaid> list2 = new List<SpawnMonsterDataForBossRaid>();
		List<SpawnMonsterDataForBossRaid> list3 = new List<SpawnMonsterDataForBossRaid>();
		Dictionary<int, Dictionary<MonsterType, List<SpawnMonsterDataForBossRaid>>> dictionary = new Dictionary<int, Dictionary<MonsterType, List<SpawnMonsterDataForBossRaid>>>();
		int num = 1;
		int num2 = 1;
		for (int i = 0; i < 180; i++)
		{
			SpawnMonsterDataForBossRaid item = default(SpawnMonsterDataForBossRaid);
			if (num % 4 == 0)
			{
				item.monsterTargetTheme = num2;
				item.isLastMonster = false;
				item.isBossMonster = false;
				item.isMiniBossMonster = true;
				item.monsterType = (EnemyManager.MonsterType)i;
				list2.Add(item);
				num = 1;
			}
			else
			{
				item.monsterTargetTheme = num2;
				item.isLastMonster = false;
				item.isBossMonster = false;
				item.isMiniBossMonster = false;
				item.monsterType = (EnemyManager.MonsterType)i;
				list.Add(item);
				num++;
			}
			if ((i + 1) % 20 == 0)
			{
				num2++;
			}
		}
		for (int j = 0; j < 10; j++)
		{
			SpawnMonsterDataForBossRaid item2 = default(SpawnMonsterDataForBossRaid);
			item2.monsterTargetTheme = j + 1;
			item2.isLastMonster = true;
			item2.isBossMonster = true;
			item2.isMiniBossMonster = false;
			item2.bossType = (EnemyManager.BossType)j;
			list3.Add(item2);
		}
		for (int k = 1; k < 11; k++)
		{
			if (!dictionary.ContainsKey(k))
			{
				Dictionary<MonsterType, List<SpawnMonsterDataForBossRaid>> value = new Dictionary<MonsterType, List<SpawnMonsterDataForBossRaid>>();
				dictionary.Add(k, value);
			}
			Dictionary<MonsterType, List<SpawnMonsterDataForBossRaid>> dictionary2 = dictionary[k];
			for (int l = 0; l < 3; l++)
			{
				if (!dictionary2.ContainsKey((MonsterType)l))
				{
					List<SpawnMonsterDataForBossRaid> value2 = new List<SpawnMonsterDataForBossRaid>();
					dictionary2.Add((MonsterType)l, value2);
				}
			}
			for (int m = 0; m < list.Count; m++)
			{
				if (list[m].monsterTargetTheme == k)
				{
					dictionary2[MonsterType.NormalMonster].Add(list[m]);
				}
			}
			for (int n = 0; n < list2.Count; n++)
			{
				if (list2[n].monsterTargetTheme == k)
				{
					dictionary2[MonsterType.MiniBossMonster].Add(list2[n]);
				}
			}
			for (int num3 = 0; num3 < list3.Count; num3++)
			{
				if (list3[num3].monsterTargetTheme == k)
				{
					dictionary2[MonsterType.RealBossMonster].Add(list3[num3]);
				}
			}
		}
		for (int num4 = 1; num4 < 11; num4++)
		{
			Dictionary<MonsterType, List<SpawnMonsterDataForBossRaid>> dictionary3 = dictionary[num4];
			List<SpawnMonsterDataForBossRaid> list4 = dictionary3[MonsterType.NormalMonster];
			List<SpawnMonsterDataForBossRaid> list5 = dictionary3[MonsterType.MiniBossMonster];
			List<SpawnMonsterDataForBossRaid> list6 = dictionary3[MonsterType.RealBossMonster];
			int num5 = list5.Count * 3;
			int num6 = 0;
			for (int num7 = 0; num7 < num5; num7++)
			{
				for (int num8 = 0; num8 < 5; num8++)
				{
					m_sortedMonsterList.Add(list4[UnityEngine.Random.Range(0, list4.Count)]);
				}
				m_sortedMonsterList.Add(list5[num6]);
				num6 = ((num6 < 4) ? (num6 + 1) : 0);
			}
			m_sortedMonsterList.Add(list6[0]);
		}
	}

	private IEnumerator waitForAction(Action action)
	{
		yield return new WaitForSeconds(1f);
		action();
	}

	public void realBossClearEvent()
	{
		clearTitleAnimation.Play("OpenBossClearForBossRaid");
	}

	public void miniBossClearEvent()
	{
		clearTitleAnimation.Play("OpenMiniBossClearForBossRaid");
		Singleton<AudioManager>.instance.playEffectSound("levelup");
	}

	public IEnumerator stageInformationUpdate()
	{
		stageInformationAnimation.Stop();
		stageInformationAnimation.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -215.3f);
		stageInformationAnimation.Play("OpenStageInformationForBossRaid");
		float timer = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= 2f)
				{
					break;
				}
			}
			yield return null;
		}
		stageInformationAnimation.Play("CloseStageInformationForBossRaid");
	}

	public void goToNextTheme()
	{
		Action value = delegate
		{
			StopCoroutine("stageInformationUpdate");
			StopCoroutine("bossRaidGroundSpawnUpdate");
			StopCoroutine("monsterSpawnUpdate");
			UIWindowIngame.instance.isCanPause = false;
			for (int i = 0; i < Singleton<CharacterManager>.instance.characterList.Count; i++)
			{
				if (Singleton<CharacterManager>.instance.characterList[i] != null)
				{
					Singleton<CharacterManager>.instance.characterList[i].setState(PublicDataManager.State.Wait);
				}
			}
			for (int j = 0; j < Singleton<ColleagueManager>.instance.currentColleagueObject.Count; j++)
			{
				Singleton<ColleagueManager>.instance.currentColleagueObject[j].setState(PublicDataManager.State.Wait);
			}
			m_prevMonsterData = default(SpawnMonsterDataForBossRaid);
			Singleton<CachedManager>.instance.coverUI.fadeOut(1.5f, delegate
			{
				Singleton<DropItemManager>.instance.collectAllItems();
				Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.localPosition = new Vector3(-5.5f, 0.1468978f, Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.localPosition.z);
				Vector2 v = Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.position + (Vector3)CameraFollow.instance.offset;
				v.x = 0f;
				CameraFollow.instance.cachedTransform.position = v;
				for (int k = 1; k < Singleton<CharacterManager>.instance.constCharacterList.Count; k++)
				{
					if (Singleton<CharacterManager>.instance.constCharacterList[k] != null)
					{
						Singleton<CharacterManager>.instance.constCharacterList[k].cachedTransform.localPosition = new Vector3(Singleton<CharacterManager>.instance.constCharacterList[k].myLeaderCharacter.cachedTransform.localPosition.x - CharacterManager.intervalBetweenCharacter, 0.1468978f, Singleton<CharacterManager>.instance.constCharacterList[k].cachedTransform.localPosition.z);
					}
				}
				Singleton<ColleagueManager>.instance.refreshStartPositions();
				m_currentTargetSpawnGroundXPos = 0f - intervalBetweenBossRaidBackground;
				for (int l = 0; l < m_currentActivatingGroundList.Count; l++)
				{
					if (m_currentActivatingGroundList[l] != null)
					{
						ObjectPool.Recycle(m_currentActivatingGroundList[l].name, m_currentActivatingGroundList[l].cachedGameObject);
					}
				}
				m_currentActivatingGroundList.Clear();
				m_nextMonsterSpawnPosition = new Vector2(m_monsterSpawnStartXPos, -3.5678f);
				if (m_currentTheme >= 10)
				{
					m_currentTheme = 1;
				}
				else
				{
					m_currentTheme++;
				}
				if (m_currentTheme == 10)
				{
					CameraFollow.instance.maxClampPosition = new Vector2(intervalBetweenBossAndMonster + 2.4f, -4.8f);
				}
				else
				{
					CameraFollow.instance.maxClampPosition = new Vector2(getCameraClampMaxXPos(), -4.8f);
				}
				for (int m = 0; m < 3; m++)
				{
					spawnNextBossRaidGround(false);
				}
				StartCoroutine("waitForMove", 1);
				Canvas.ForceUpdateCanvases();
				refreshParticleBug();
				Singleton<CachedManager>.instance.coverUI.fadeIn(1f, delegate
				{
					refreshParticleBug();
					Singleton<SkillManager>.instance.LockSkillButton(false);
					UIWindowIngame.instance.isCanPause = true;
					clearTitleCanvasGroup.alpha = 0f;
					StartCoroutine("stageInformationUpdate");
					StartCoroutine("bossRaidGroundSpawnUpdate");
					StartCoroutine("monsterSpawnUpdate");
				});
				GC.Collect();
			});
		};
		StartCoroutine("waitForAction", value);
	}

	private IEnumerator waitForMove(float waitTime)
	{
		float timer = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= waitTime)
				{
					break;
				}
			}
			yield return null;
		}
		for (int j = 0; j < Singleton<CharacterManager>.instance.characterList.Count; j++)
		{
			if (Singleton<CharacterManager>.instance.characterList[j] != null)
			{
				Singleton<CharacterManager>.instance.characterList[j].setState(PublicDataManager.State.Move);
			}
		}
		for (int i = 0; i < Singleton<ColleagueManager>.instance.currentColleagueObject.Count; i++)
		{
			Singleton<ColleagueManager>.instance.currentColleagueObject[i].setState(PublicDataManager.State.Move);
		}
	}

	public void resetAll()
	{
		lastKillBossData = default(BossRaidBestRecordData);
		currentStageForBossRaid = 1L;
		stageText.text = "stage " + currentStageForBossRaid;
		maxHeart = 3;
		Singleton<DataManager>.instance.currentGameData.heartForBossRaid = Mathf.Min(Singleton<DataManager>.instance.currentGameData.heartForBossRaid, maxHeart);
		m_monsterSpawnStartXPos = 13f;
		m_prevMonsterData = default(SpawnMonsterDataForBossRaid);
		m_currentMonsterLevel = 1;
		m_currentTotalSpawnedMonsterCountForStatLevelUp = 0;
		m_currentTotalSpawnedMonsterCountForMonsterLevelUp = 0;
		m_goldChestExtraChanceForMiniBoss = 0.0;
		m_diaChestExtraChanceForMiniBoss = 0.0;
		m_goldChestExtraChanceForBoss = 0.0;
		m_diaChestExtraChanceForBoss = 0.0;
		m_nextMonsterSpawnPosition = new Vector2(m_monsterSpawnStartXPos, -3.5678f);
		StopAllCoroutines();
		m_currentMonsterIndex = 0;
		m_currentTheme = 1;
		m_currentTargetSpawnGroundXPos = 0f - intervalBetweenBossRaidBackground;
		for (int i = 0; i < m_currentActivatingGroundList.Count; i++)
		{
			if (m_currentActivatingGroundList[i] != null)
			{
				ObjectPool.Recycle(m_currentActivatingGroundList[i].name, m_currentActivatingGroundList[i].cachedGameObject);
			}
		}
		m_currentActivatingGroundList.Clear();
		m_sortedMonsterList.Clear();
	}

	private void spawnNextMonster()
	{
		if (!m_prevMonsterData.isLastMonster)
		{
			SpawnMonsterDataForBossRaid spawnMonsterDataForBossRaid = (m_prevMonsterData = getNextMonsterType());
			if (spawnMonsterDataForBossRaid.isMiniBossMonster)
			{
				m_nextMonsterSpawnPosition.x += intervalBetweenMiniBossAndMonster;
			}
			if (spawnMonsterDataForBossRaid.isBossMonster)
			{
				m_goldChestExtraChanceForMiniBoss = Math.Min(32.0, m_goldChestExtraChanceForMiniBoss + 0.221);
				m_diaChestExtraChanceForMiniBoss = Math.Min(13.0, m_diaChestExtraChanceForMiniBoss + 0.092);
				m_goldChestExtraChanceForBoss = Math.Min(10.0, m_goldChestExtraChanceForBoss + 0.069);
				m_diaChestExtraChanceForBoss = Math.Min(15.0, m_diaChestExtraChanceForBoss + 0.103);
				Singleton<EnemyManager>.instance.spawnBoss(spawnMonsterDataForBossRaid.bossType, m_nextMonsterSpawnPosition, MovingObject.Direction.Left, m_currentMonsterLevel * 6, true);
			}
			else if (spawnMonsterDataForBossRaid.isMiniBossMonster)
			{
				m_goldChestExtraChanceForMiniBoss = Math.Min(32.0, m_goldChestExtraChanceForMiniBoss + 0.221);
				m_diaChestExtraChanceForMiniBoss = Math.Min(13.0, m_diaChestExtraChanceForMiniBoss + 0.092);
				m_goldChestExtraChanceForBoss = Math.Min(10.0, m_goldChestExtraChanceForBoss + 0.069);
				m_diaChestExtraChanceForBoss = Math.Min(15.0, m_diaChestExtraChanceForBoss + 0.103);
				Singleton<EnemyManager>.instance.spawnMiniBoss(spawnMonsterDataForBossRaid.monsterType, m_nextMonsterSpawnPosition, MovingObject.Direction.Left, m_currentMonsterLevel * 6, true);
			}
			else
			{
				Singleton<EnemyManager>.instance.spawnMonster(spawnMonsterDataForBossRaid.monsterType, m_nextMonsterSpawnPosition, MovingObject.Direction.Left, m_currentMonsterLevel);
			}
			if (!spawnMonsterDataForBossRaid.isBossMonster && !spawnMonsterDataForBossRaid.isMiniBossMonster)
			{
				m_nextMonsterSpawnPosition.x += intervalBetweenBossRaidMonsters;
			}
			else
			{
				m_nextMonsterSpawnPosition.x += intervalBetweenBossAndMonster;
			}
			m_currentTotalSpawnedMonsterCountForMonsterLevelUp++;
			m_currentTotalSpawnedMonsterCountForStatLevelUp++;
			if (m_currentTotalSpawnedMonsterCountForMonsterLevelUp >= 3)
			{
				m_currentTotalSpawnedMonsterCountForMonsterLevelUp = 0;
				m_currentMonsterLevel++;
			}
			m_currentMonsterIndex = (m_currentMonsterIndex + 1) % m_sortedMonsterList.Count;
		}
		else
		{
			m_nextMonsterSpawnPosition = new Vector2(m_monsterSpawnStartXPos, -3.5678f);
		}
	}

	private SpawnMonsterDataForBossRaid getNextMonsterType(int targetExtraValue = 0)
	{
		m_currentMonsterIndex %= m_sortedMonsterList.Count;
		int index = (m_currentMonsterIndex + targetExtraValue) % m_sortedMonsterList.Count;
		return m_sortedMonsterList[index];
	}

	private void spawnNextBossRaidGround(bool isRecycleFirstGround = true)
	{
		if (isRecycleFirstGround && m_currentActivatingGroundList[0] != null)
		{
			ObjectPool.Recycle(m_currentActivatingGroundList[0].name, m_currentActivatingGroundList[0].cachedGameObject);
			m_currentActivatingGroundList.Remove(m_currentActivatingGroundList[0]);
		}
		BossRaidBackgroundObject component = ObjectPool.Spawn("@BossRaidMapObject", new Vector2(m_currentTargetSpawnGroundXPos, -2.13f), Singleton<CachedManager>.instance.bossRaidBackgroundParentTrnasform).GetComponent<BossRaidBackgroundObject>();
		component.init(m_currentTheme);
		m_currentActivatingGroundList.Add(component);
		m_currentTargetSpawnGroundXPos += intervalBetweenBossRaidBackground;
	}

	public void increaseStage(bool isMiniBoss = false)
	{
		if (isMiniBoss)
		{
			StopCoroutine("stageInformationUpdate");
			StartCoroutine("stageInformationUpdate");
		}
		UIWindowIngame.instance.isCanPause = true;
		currentStageForBossRaid++;
		stageText.text = "stage " + currentStageForBossRaid;
		stageTextAnimation.Stop();
		stageTextAnimation.Play();
	}

	private void resetChest()
	{
		for (int i = 0; i < bronzeChestCollectAnimation.Length; i++)
		{
			bronzeChestCollectAnimation[i].transform.localScale = Vector3.one;
		}
		for (int j = 0; j < goldChestCollectAnimation.Length; j++)
		{
			goldChestCollectAnimation[j].transform.localScale = Vector3.one;
		}
		for (int k = 0; k < diaChestCollectAnimation.Length; k++)
		{
			diaChestCollectAnimation[k].transform.localScale = Vector3.one;
		}
		bronzeChestCountText.text = "0";
		goldChestCountText.text = "0";
		diaChestCountText.text = "0";
		currentBronzeChestList.Clear();
		currentGoldChestList.Clear();
		currentDiaChestList.Clear();
		collectedBronzeChestList.Clear();
		collectedGoldChestList.Clear();
		collectedDiaChestList.Clear();
	}

	public void tryToGetRandomChestForFossRaid(long monterLevelWhenDrop, long stageWhenDrop, bool isLastBoss, Vector2 spawnPosition)
	{
		BossRaidChestData chestData = default(BossRaidChestData);
		chestData.monterLevelWhenDrop = monterLevelWhenDrop;
		chestData.stageWhenDrop = stageWhenDrop;
		float num = 30f;
		num *= (float)((!isLastBoss) ? 1 : 10);
		int num2 = UnityEngine.Random.Range(0, 10000);
		num2 /= 100;
		if (num >= (float)num2)
		{
			BossRaidChestType bossRaidChestType = BossRaidChestType.Null;
			double num3 = m_randomForAppearChestType.Next(0, 100000);
			num3 /= 1000.0;
			double num4 = 0.0;
			double num5 = 0.0;
			if (isLastBoss)
			{
				num4 = 50.0 + m_goldChestExtraChanceForBoss;
				num5 = 15.0 + m_diaChestExtraChanceForBoss;
			}
			else
			{
				num4 = 18.0 + m_goldChestExtraChanceForMiniBoss;
				num5 = 2.0 + m_diaChestExtraChanceForMiniBoss;
			}
			bossRaidChestType = (chestData.chestType = ((num4 > num3) ? BossRaidChestType.Gold : ((!(num4 + num5 > num3)) ? BossRaidChestType.Bronze : BossRaidChestType.Dia)));
			spawnChest(chestData, spawnPosition);
		}
	}

	public void spawnChest(BossRaidChestData chestData, Vector2 spawnPosition)
	{
		BossRaidChestObject component = ObjectPool.Spawn("@BossRaidChestObject", spawnPosition).GetComponent<BossRaidChestObject>();
		component.spawnBossRaidChest(chestData);
	}

	public void increaseChest(BossRaidChestData chestData)
	{
		switch (chestData.chestType)
		{
		case BossRaidChestType.Bronze:
		{
			collectedBronzeChestList.Add(chestData);
			bronzeChestCountText.text = collectedBronzeChestList.Count.ToString();
			for (int j = 0; j < bronzeChestCollectAnimation.Length; j++)
			{
				bronzeChestCollectAnimation[j].Stop();
				bronzeChestCollectAnimation[j].Play();
			}
			break;
		}
		case BossRaidChestType.Gold:
		{
			collectedGoldChestList.Add(chestData);
			goldChestCountText.text = collectedGoldChestList.Count.ToString();
			for (int k = 0; k < goldChestCollectAnimation.Length; k++)
			{
				goldChestCollectAnimation[k].Stop();
				goldChestCollectAnimation[k].Play();
			}
			break;
		}
		case BossRaidChestType.Dia:
		{
			collectedDiaChestList.Add(chestData);
			diaChestCountText.text = collectedDiaChestList.Count.ToString();
			for (int i = 0; i < diaChestCollectAnimation.Length; i++)
			{
				diaChestCollectAnimation[i].Stop();
				diaChestCollectAnimation[i].Play();
			}
			break;
		}
		}
	}

	public List<BossRaidChestObject> getChests(Vector2 startPosition, float range)
	{
		m_cachedDynamicList.Clear();
		int count = currentBronzeChestList.Count;
		for (int i = 0; i < count; i++)
		{
			if (Vector2.Distance(currentBronzeChestList[i].cachedTransform.position, startPosition) <= range)
			{
				m_cachedDynamicList.Add(currentBronzeChestList[i]);
			}
		}
		int count2 = currentGoldChestList.Count;
		for (int j = 0; j < count2; j++)
		{
			if (Vector2.Distance(currentGoldChestList[j].cachedTransform.position, startPosition) <= range)
			{
				m_cachedDynamicList.Add(currentGoldChestList[j]);
			}
		}
		int count3 = currentDiaChestList.Count;
		for (int k = 0; k < count3; k++)
		{
			if (Vector2.Distance(currentDiaChestList[k].cachedTransform.position, startPosition) <= range)
			{
				m_cachedDynamicList.Add(currentDiaChestList[k]);
			}
		}
		return m_cachedDynamicList;
	}

	public List<ChestRewardData> calculateChestDataToChestRewardData(List<BossRaidChestData> totalRaidChestDataList)
	{
		List<ChestRewardData> list = new List<ChestRewardData>();
		for (int i = 0; i < totalRaidChestDataList.Count; i++)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			ChestRewardData chestRewardData = new ChestRewardData();
			switch (totalRaidChestDataList[i].chestType)
			{
			case BossRaidChestType.Bronze:
				num2 = 15.0;
				num = 5.0;
				num3 = 5.0;
				num4 = 0.0001;
				break;
			case BossRaidChestType.Gold:
				num2 = 20.0;
				num = 5.0;
				num3 = 10.0;
				num4 = 0.1;
				break;
			case BossRaidChestType.Dia:
				num2 = 30.0;
				num = 20.0;
				num3 = 20.0;
				num4 = 1.0;
				break;
			}
			chestRewardData.chestType = totalRaidChestDataList[i].chestType;
			if (totalRaidChestDataList[i].chestType == BossRaidChestType.Dia)
			{
				num4 *= (double)((float)totalRaidChestDataList[i].stageWhenDrop / 500f + 1f);
			}
			num4 = Math.Min(num4, 2.0);
			double num5 = m_randomForAppearItem.Next(0, 1000000);
			num5 /= 10000.0;
			if (num >= num5)
			{
				chestRewardData.targetRewardType = BossRaidChestRewardType.Ruby;
				switch (totalRaidChestDataList[i].chestType)
				{
				case BossRaidChestType.Bronze:
					chestRewardData.value = 1.0;
					break;
				case BossRaidChestType.Gold:
					chestRewardData.value = UnityEngine.Random.Range(1, 3);
					break;
				case BossRaidChestType.Dia:
					chestRewardData.value = UnityEngine.Random.Range(2, 6);
					break;
				}
			}
			else if (num + num3 >= num5)
			{
				chestRewardData.targetRewardType = BossRaidChestRewardType.TreasureKey;
				switch (totalRaidChestDataList[i].chestType)
				{
				case BossRaidChestType.Bronze:
					chestRewardData.value = 1.0;
					break;
				case BossRaidChestType.Gold:
					chestRewardData.value = UnityEngine.Random.Range(1, 4);
					break;
				case BossRaidChestType.Dia:
					chestRewardData.value = UnityEngine.Random.Range(3, 8);
					break;
				}
			}
			else if (num + num3 + num4 >= num5)
			{
				chestRewardData.targetRewardType = BossRaidChestRewardType.CharacterSkin;
				chestRewardData.value = 0.0;
			}
			else if (num + num3 + num4 + num2 >= num5)
			{
				chestRewardData.targetRewardType = BossRaidChestRewardType.TreasureEnchantStone;
				switch (totalRaidChestDataList[i].chestType)
				{
				case BossRaidChestType.Bronze:
					chestRewardData.value = UnityEngine.Random.Range(3, 6);
					break;
				case BossRaidChestType.Gold:
					chestRewardData.value = UnityEngine.Random.Range(5, 10);
					break;
				case BossRaidChestType.Dia:
					chestRewardData.value = UnityEngine.Random.Range(10, 16);
					break;
				}
			}
			else
			{
				chestRewardData.targetRewardType = BossRaidChestRewardType.Gold;
				long num6 = (totalRaidChestDataList[i].stageWhenDrop * (totalRaidChestDataList[i].monterLevelWhenDrop / 2) + 1000) * 2;
				num6 = (long)UnityEngine.Random.Range((float)num6 * 0.8f, (float)num6 * 1.2f);
				switch (totalRaidChestDataList[i].chestType)
				{
				case BossRaidChestType.Gold:
					num6 = (long)((float)num6 * 3f);
					break;
				case BossRaidChestType.Dia:
					num6 = (long)((float)num6 * 6f);
					break;
				}
				chestRewardData.value = num6;
			}
			list.Add(chestRewardData);
		}
		return list;
	}

	private IEnumerator bossRaidGroundSpawnUpdate()
	{
		CharacterObject warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		float initialPosition = warriorCharacter.cachedTransform.position.x;
		int lastBuiltGroundIdx = 0;
		while (true)
		{
			if (!GameManager.isPause)
			{
				float currentPosition = warriorCharacter.cachedTransform.position.x;
				int idx = (int)((currentPosition - initialPosition) / intervalBetweenBossRaidBackground);
				if (idx > lastBuiltGroundIdx)
				{
					spawnNextBossRaidGround();
					lastBuiltGroundIdx = idx;
				}
			}
			yield return null;
		}
	}

	private IEnumerator monsterSpawnUpdate()
	{
		CharacterObject warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		Vector3 previousCharacterPosition = warriorCharacter.cachedTransform.position;
		float stackCharacterMovedXPos2 = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				stackCharacterMovedXPos2 += warriorCharacter.cachedTransform.position.x - previousCharacterPosition.x;
				previousCharacterPosition = warriorCharacter.cachedTransform.position;
				if (!getNextMonsterType().isBossMonster && !getNextMonsterType().isMiniBossMonster)
				{
					if (stackCharacterMovedXPos2 >= intervalBetweenBossRaidMonsters)
					{
						stackCharacterMovedXPos2 -= intervalBetweenBossRaidMonsters;
						spawnNextMonster();
					}
				}
				else if (getNextMonsterType().isMiniBossMonster)
				{
					if (stackCharacterMovedXPos2 >= intervalBetweenMiniBossAndMonster)
					{
						stackCharacterMovedXPos2 -= intervalBetweenMiniBossAndMonster;
						stackCharacterMovedXPos2 -= intervalBetweenBossAndMonster;
						spawnNextMonster();
					}
				}
				else if (getNextMonsterType().isBossMonster && stackCharacterMovedXPos2 >= intervalBetweenBossAndMonster)
				{
					stackCharacterMovedXPos2 -= intervalBetweenBossAndMonster;
					spawnNextMonster();
				}
			}
			yield return null;
		}
	}

	private void calculateHeart()
	{
		maxHeart = 3;
		TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.Now().Ticks - Singleton<DataManager>.instance.currentGameData.lastHeartUsedTime);
		double num = 1200.0;
		int num2 = (int)(timeSpan.TotalSeconds / num);
		if (num2 > 0)
		{
			increaseHeart(num2, true);
		}
		m_isCalculatedHeart = true;
	}

	private void OnApplicationPause(bool pause)
	{
		if (GameManager.currentGameState == GameManager.GameState.OutGame && !pause)
		{
			calculateHeart();
		}
	}

	private void Update()
	{
		if (!m_isCalculatedHeart)
		{
			return;
		}
		maxHeart = 3;
		if (Singleton<DataManager>.instance.currentGameData.heartForBossRaid < maxHeart)
		{
			TimeSpan timeSpan = new TimeSpan(UnbiasedTime.Instance.Now().Ticks - Singleton<DataManager>.instance.currentGameData.lastHeartUsedTime);
			double num = 1200.0;
			if (timeSpan.TotalSeconds > 0.0 && timeSpan.TotalSeconds >= num)
			{
				increaseHeart(1, true);
			}
			TimeSpan timeSpan2 = new TimeSpan(new DateTime(Singleton<DataManager>.instance.currentGameData.lastHeartUsedTime).AddSeconds(num).Ticks - UnbiasedTime.Instance.Now().Ticks);
			heartTimerText.text = string.Format("{0:00}:{1:00}", Mathf.Abs(timeSpan2.Minutes), Mathf.Abs(timeSpan2.Seconds));
		}
		if (Singleton<DataManager>.instance.currentGameData.heartForBossRaid >= maxHeart && !heartTimerText.text.Equals("max"))
		{
			heartTimerText.text = "max";
		}
	}

	public void increaseHeart(int value, bool isInitTime)
	{
		if (value > 0)
		{
			maxHeart = 3;
			if (isInitTime)
			{
				Singleton<DataManager>.instance.currentGameData.lastHeartUsedTime = UnbiasedTime.Instance.Now().Ticks;
			}
			Singleton<DataManager>.instance.currentGameData.heartForBossRaid += value;
			Singleton<DataManager>.instance.currentGameData.heartForBossRaid = Mathf.Min(Singleton<DataManager>.instance.currentGameData.heartForBossRaid, maxHeart);
			Singleton<DataManager>.instance.saveData();
			displayHeart();
		}
	}

	public void decreaseHeart(int value)
	{
		if (Singleton<DataManager>.instance.currentGameData.heartForBossRaid >= maxHeart)
		{
			Singleton<DataManager>.instance.currentGameData.lastHeartUsedTime = UnbiasedTime.Instance.Now().Ticks;
		}
		Singleton<DataManager>.instance.currentGameData.heartForBossRaid -= value;
		Singleton<DataManager>.instance.saveData();
		NotificationManager.CancelAllLocalNotification();
		int num = Mathf.Max(Singleton<DataManager>.instance.currentGameData.heartForBossRaid, 0);
		DateTime value2 = new DateTime(Singleton<DataManager>.instance.currentGameData.lastHeartUsedTime);
		double num2 = 1200.0;
		double num3 = num2 * (double)(maxHeart - num);
		if (num < maxHeart && UnbiasedTime.Instance.Now() < value2.AddSeconds(num3))
		{
			double val = num3 - UnbiasedTime.Instance.Now().Subtract(value2).TotalSeconds;
			NotificationManager.SetBossRaidtNotification((int)Math.Min(val, num2 * (double)maxHeart));
		}
		displayHeart();
	}

	public void displayHeart()
	{
		if (GameManager.currentGameState == GameManager.GameState.OutGame)
		{
			resetAll();
			for (int i = 0; i < maxHeart; i++)
			{
				UIWindowBossRaid.instance.heartImages[i].sprite = emptyHeartSprite;
			}
			for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.heartForBossRaid && j <= UIWindowBossRaid.instance.heartImages.Length; j++)
			{
				UIWindowBossRaid.instance.heartImages[j].sprite = fillHeartSprite;
			}
		}
	}

	private void refreshParticleBug()
	{
		for (int i = 0; i < doubleSpeedParticleObjects.Length; i++)
		{
			doubleSpeedParticleObjects[i].SetActive(false);
			doubleSpeedParticleObjects[i].SetActive(true);
		}
	}

	public void resetDoubleSpeed()
	{
		m_isOpendDoubleSpeedButton = false;
		doubleSpeedProgressingObject.anchoredPosition = new Vector2(120f, -187f);
		doubleSpeedButtonObject.anchoredPosition = new Vector2(120f, -187f);
	}

	public void appearDoubleSpeedButton()
	{
		if (!m_isOpendDoubleSpeedButton)
		{
			isAlreadySeenUI = false;
			m_isOpendDoubleSpeedButton = true;
			doubleSpeedButtonAnimation.Play("DoubleSpeedButtonAppearAnimation");
		}
	}

	public void disappearDoubleSpeedButton()
	{
		if (m_isOpendDoubleSpeedButton)
		{
			m_isOpendDoubleSpeedButton = false;
			doubleSpeedButtonAnimation.Play("DoubleSpeedButtonDisappearAnimation");
			if (!isDoubleSpeed)
			{
				StartCoroutine("doubleSpeedAppearUpdate", false);
			}
		}
	}

	public void OnClickDoubleSpeed()
	{
		if (UIWindowIngame.instance.isCanPause && !isAlreadySeenUI)
		{
			isAlreadySeenUI = true;
			UIWIndowDoubleSpeedDialog.instance.openDoubleSpeedDialogUI(delegate
			{
				startDoubleSpeed();
				disappearDoubleSpeedButton();
			});
		}
	}

	public void startDoubleSpeed()
	{
		if (!isDoubleSpeed)
		{
			doubleSpeedProgressingAnimation.Play("DoubleSpeedButtonAppearAnimation");
			isDoubleSpeed = true;
			Singleton<GameManager>.instance.setTimeScale(true);
			StopCoroutine("doubleSpeedUpdate");
			StartCoroutine("doubleSpeedUpdate");
		}
	}

	public void stopDoubleSpeed()
	{
		if (isDoubleSpeed)
		{
			doubleSpeedProgressingAnimation.Play("DoubleSpeedButtonDisappearAnimation");
			StartCoroutine("doubleSpeedAppearUpdate", false);
			isDoubleSpeed = false;
			Singleton<GameManager>.instance.setTimeScale(true);
		}
	}

	private IEnumerator doubleSpeedUpdate()
	{
		float timer = 0f;
		while (true)
		{
			timer += Time.deltaTime * GameManager.timeScale / Time.timeScale;
			TimeSpan span = new TimeSpan(0, 0, (int)doubleSpeedDuration - (int)timer);
			doubleSpeedTimerText.text = string.Format("{0}:{1:00}", Mathf.Abs(span.Minutes), Mathf.Abs(span.Seconds));
			if (timer >= doubleSpeedDuration)
			{
				break;
			}
			yield return null;
		}
		stopDoubleSpeed();
	}

	private IEnumerator doubleSpeedAppearUpdate(bool isFirstAppear)
	{
		float timer = 0f;
		while (true)
		{
			timer += Time.deltaTime * GameManager.timeScale;
			if (!isFirstAppear)
			{
				if (timer >= intervalDoubleSpeedAppear)
				{
					appearDoubleSpeedButton();
					yield break;
				}
			}
			else if (timer >= 5f)
			{
				break;
			}
			yield return null;
		}
		appearDoubleSpeedButton();
	}
}
