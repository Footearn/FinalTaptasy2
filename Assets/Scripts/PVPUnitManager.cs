using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class PVPUnitManager : Singleton<PVPUnitManager>
{
	[Serializable]
	public struct PVPUnitStatData
	{
		public double damage;

		public float criticalChance;

		public double criticalDamage;

		public float attackSpeed;

		public float moveSpeed;

		public double hp;

		public float attackRange;

		public float detectRange
		{
			get
			{
				return attackRange + 2.5f;
			}
		}
	}

	public enum AttackType
	{
		None = -1,
		MeleeSingleAttack,
		MiddleRange,
		LongRange,
		MeleeSplashAttack,
		Length
	}

	public static int maxLineCount = 15;

	public static float lineMinPosition = -4.24f;

	public static float lineMaxPosition = -0.42f;

	public Dictionary<AttackType, List<PVPUnitObject>> totalAllyGroupByAttackType = new Dictionary<AttackType, List<PVPUnitObject>>();

	public List<PVPUnitObject> totalAllyList = new List<PVPUnitObject>();

	public Dictionary<AttackType, List<PVPUnitObject>> totalEnemyGroupByAttackType = new Dictionary<AttackType, List<PVPUnitObject>>();

	public List<PVPUnitObject> totalEnemyList = new List<PVPUnitObject>();

	public List<PVPTankObject> totalTankList = new List<PVPTankObject>();

	public PVPUnitObject allyLeaderWarrior;

	public PVPUnitObject enemyLeaderWarrior;

	public PVPManager.PVPGameData currentMyGameData;

	public PVPManager.PVPGameData currentEnemyGameData;

	public DateTime meleeAttackSoundLastPlayTime = Util.DateTimeNotUse();

	public DateTime middleAttackSoundLastPlayTime = Util.DateTimeNotUse();

	public DateTime rangedAttackSoundLastPlayTime = Util.DateTimeNotUse();

	public DateTime splashAttackSoundLastPlayTime = Util.DateTimeNotUse();

	private Coroutine m_allyDelayedSpawnUpdateCoroutine;

	private Coroutine m_enemyDelayedSpawnUpdateCoroutine;

	private Coroutine m_waitForEndPVPCoroutine;

	private Coroutine m_startEffectUpdateCoroutine;

	private List<PVPPrincessObject> m_totalPrincessObjectList = new List<PVPPrincessObject>();

	private List<PVPPrincessObject> m_totalAllyPrincessObjectList = new List<PVPPrincessObject>();

	private List<PVPPrincessObject> m_totalEnemyPrincessObjectList = new List<PVPPrincessObject>();

	private List<int> m_allyPrincessSpawnIndexList = new List<int>();

	private List<int> m_enemyPrincessSpawnIndexList = new List<int>();

	private List<PVPUnitObject> m_unitListForGetNearestUnits = new List<PVPUnitObject>();

	public bool isSpawning
	{
		get
		{
			return m_allyDelayedSpawnUpdateCoroutine != null || m_enemyDelayedSpawnUpdateCoroutine != null;
		}
	}

	public void startGame(PVPManager.PVPGameData allyData, PVPManager.PVPGameData enemyData)
	{
		currentMyGameData = allyData;
		currentEnemyGameData = enemyData;
		for (int i = 0; i < 4; i++)
		{
			if (!totalAllyGroupByAttackType.ContainsKey((AttackType)i))
			{
				totalAllyGroupByAttackType.Add((AttackType)i, new List<PVPUnitObject>());
			}
			if (!totalEnemyGroupByAttackType.ContainsKey((AttackType)i))
			{
				totalEnemyGroupByAttackType.Add((AttackType)i, new List<PVPUnitObject>());
			}
		}
		clearAllEnemy();
		m_allyPrincessSpawnIndexList.Clear();
		m_enemyPrincessSpawnIndexList.Clear();
		for (int j = 0; j < 4; j++)
		{
			spawnPrincess(true);
			spawnPrincess(false);
		}
		if (m_allyDelayedSpawnUpdateCoroutine == null)
		{
			m_allyDelayedSpawnUpdateCoroutine = StartCoroutine(spawnDelayUpdate(allyData, true));
		}
		if (m_enemyDelayedSpawnUpdateCoroutine == null)
		{
			m_enemyDelayedSpawnUpdateCoroutine = StartCoroutine(spawnDelayUpdate(enemyData, false));
		}
		if (m_waitForEndPVPCoroutine != null)
		{
			m_startEffectUpdateCoroutine = StartCoroutine(startEffectUpdate());
		}
		StartCoroutine(startEffectUpdate());
		m_waitForEndPVPCoroutine = StartCoroutine(waitForEndPVP());
	}

	public void endGame()
	{
		clearAllEnemy();
		if (m_allyDelayedSpawnUpdateCoroutine != null)
		{
			StopCoroutine(m_allyDelayedSpawnUpdateCoroutine);
		}
		if (m_enemyDelayedSpawnUpdateCoroutine != null)
		{
			StopCoroutine(m_enemyDelayedSpawnUpdateCoroutine);
		}
		m_allyDelayedSpawnUpdateCoroutine = null;
		m_enemyDelayedSpawnUpdateCoroutine = null;
	}

	public void addUnit(PVPUnitObject unit)
	{
		if (unit.isAlly)
		{
			totalAllyList.Add(unit);
			totalAllyGroupByAttackType[unit.currentAttackType].Add(unit);
		}
		else
		{
			totalEnemyList.Add(unit);
			totalEnemyGroupByAttackType[unit.currentAttackType].Add(unit);
		}
	}

	public void removeUnit(PVPUnitObject unit)
	{
		if (unit.isAlly)
		{
			totalAllyList.Remove(unit);
			totalAllyGroupByAttackType[unit.currentAttackType].Remove(unit);
		}
		else
		{
			totalEnemyList.Remove(unit);
			totalEnemyGroupByAttackType[unit.currentAttackType].Remove(unit);
		}
	}

	private IEnumerator waitForEndPVP()
	{
		bool isWin2 = false;
		while (true)
		{
			if (!isSpawning)
			{
				if (totalAllyList.Count <= 0)
				{
					isWin2 = false;
					break;
				}
				if (totalEnemyList.Count <= 0)
				{
					isWin2 = true;
					break;
				}
			}
			yield return null;
		}
		if (isWin2)
		{
			for (int l = 0; l < m_totalAllyPrincessObjectList.Count; l++)
			{
				m_totalAllyPrincessObjectList[l].stopRandomAnimationPlay();
				m_totalAllyPrincessObjectList[l].currentPrincessSpriteAnimation.playAnimation("Cheer", UnityEngine.Random.Range(0.05f, 0.07f));
			}
			for (int k = 0; k < m_totalEnemyPrincessObjectList.Count; k++)
			{
				m_totalEnemyPrincessObjectList[k].stopRandomAnimationPlay();
				m_totalEnemyPrincessObjectList[k].currentPrincessSpriteAnimation.playFixAnimation("Taking", 0);
			}
		}
		else
		{
			for (int j = 0; j < m_totalAllyPrincessObjectList.Count; j++)
			{
				m_totalAllyPrincessObjectList[j].stopRandomAnimationPlay();
				m_totalAllyPrincessObjectList[j].currentPrincessSpriteAnimation.playFixAnimation("Taking", 0);
			}
			for (int i = 0; i < m_totalEnemyPrincessObjectList.Count; i++)
			{
				m_totalEnemyPrincessObjectList[i].stopRandomAnimationPlay();
				m_totalEnemyPrincessObjectList[i].currentPrincessSpriteAnimation.playAnimation("Cheer", UnityEngine.Random.Range(0.05f, 0.07f));
			}
		}
		yield return new WaitForSeconds(1f);
		PVPManager.isPlayingPVP = false;
		UIWindowPVPResult.instance.openPVPResult(isWin2);
	}

	private IEnumerator startEffectUpdate()
	{
		float timer3 = 0f;
		while (true)
		{
			timer3 += Time.deltaTime * GameManager.timeScale;
			if (timer3 >= 1.6f)
			{
				break;
			}
			yield return null;
		}
		timer3 = 0f;
		GameManager.isPause = true;
		allyLeaderWarrior.setState(PublicDataManager.State.Wait);
		enemyLeaderWarrior.setState(PublicDataManager.State.Wait);
		yield return new WaitForSeconds(0.7f);
		allyLeaderWarrior.playBoneAnimation("Skill");
		enemyLeaderWarrior.playBoneAnimation("Skill");
		UIWindowPVP.instance.startEffectObject.SetActive(true);
		while (true)
		{
			timer3 += Time.deltaTime * GameManager.timeScale;
			if (timer3 >= 1.4f)
			{
				break;
			}
			yield return null;
		}
		Singleton<AudioManager>.instance.playEffectSound("pvp_battle_cry");
		timer3 = 0f;
		GameManager.isPause = false;
		m_startEffectUpdateCoroutine = null;
	}

	private void sortUnitData(PVPManager.PVPGameData unitData)
	{
		Dictionary<AttackType, List<PVPManager.PVPSkinData>> dictionary = new Dictionary<AttackType, List<PVPManager.PVPSkinData>>();
		dictionary.Add(AttackType.MeleeSingleAttack, new List<PVPManager.PVPSkinData>());
		dictionary.Add(AttackType.MiddleRange, new List<PVPManager.PVPSkinData>());
		dictionary.Add(AttackType.LongRange, new List<PVPManager.PVPSkinData>());
		dictionary.Add(AttackType.MeleeSplashAttack, new List<PVPManager.PVPSkinData>());
		for (int i = 0; i < unitData.totalSkinData.Count; i++)
		{
			dictionary[getAttackType(unitData.totalSkinData[i])].Add(unitData.totalSkinData[i]);
		}
		List<PVPManager.PVPSkinData> list = new List<PVPManager.PVPSkinData>();
		string[] array = ((unitData.equippedCharacterData == null) ? null : unitData.equippedCharacterData.Split(','));
		CharacterSkinManager.WarriorSkinType warriorSkinType = CharacterSkinManager.WarriorSkinType.William;
		if (array != null && array.Length == 3)
		{
			warriorSkinType = (CharacterSkinManager.WarriorSkinType)int.Parse(array[0]);
		}
		for (int j = 0; j < 4; j++)
		{
			if (j == 3)
			{
				continue;
			}
			List<PVPManager.PVPSkinData> list2 = dictionary[(AttackType)j];
			for (int k = 0; k < list2.Count; k++)
			{
				if (list2[k].currentColleagueType != ColleagueManager.ColleagueType.Pinky && list2[k].currentColleagueType != ColleagueManager.ColleagueType.BabyDragon && list2[k].currentWarriorSkinType != warriorSkinType)
				{
					list.Add(list2[k]);
				}
			}
		}
		list.Shuffle();
		List<PVPManager.PVPSkinData> list3 = dictionary[AttackType.MeleeSplashAttack];
		for (int l = 0; l < list3.Count; l++)
		{
			if (list3[l].currentColleagueType != ColleagueManager.ColleagueType.Pinky && list3[l].currentColleagueType != ColleagueManager.ColleagueType.BabyDragon && list3[l].currentWarriorSkinType != warriorSkinType)
			{
				list.Add(list3[l]);
			}
		}
		unitData.totalSkinData = list;
	}

	private IEnumerator spawnETCUnitUpdate(PVPManager.PVPGameData unitData, bool isAlly)
	{
		string[] characterTypeSplit = ((unitData.equippedCharacterData == null) ? null : unitData.equippedCharacterData.Split(','));
		CharacterSkinManager.WarriorSkinType warriorSkinType = CharacterSkinManager.WarriorSkinType.William;
		if (characterTypeSplit != null && characterTypeSplit.Length == 3)
		{
			warriorSkinType = (CharacterSkinManager.WarriorSkinType)int.Parse(characterTypeSplit[0]);
		}
		PVPManager.PVPSkinData equippedSkinData = null;
		for (int i = 0; i < unitData.totalSkinData.Count; i++)
		{
			if (unitData.totalSkinData[i].currentWarriorSkinType == warriorSkinType)
			{
				equippedSkinData = unitData.totalSkinData[i];
				break;
			}
		}
		if (equippedSkinData != null)
		{
			if (isAlly)
			{
				allyLeaderWarrior = spawnUnit(unitData, equippedSkinData, isAlly, new Vector2((!isAlly) ? 7.5f : (-7.5f), lineMinPosition + (lineMaxPosition - lineMinPosition) / 2f));
			}
			else
			{
				enemyLeaderWarrior = spawnUnit(unitData, equippedSkinData, isAlly, new Vector2((!isAlly) ? 7.5f : (-7.5f), lineMinPosition + (lineMaxPosition - lineMinPosition) / 2f));
			}
		}
		List<PVPManager.PVPSkinData> flyUnitList = new List<PVPManager.PVPSkinData>();
		for (int j = 0; j < unitData.totalSkinData.Count; j++)
		{
			if (unitData.totalSkinData[j].currentColleagueType == ColleagueManager.ColleagueType.Pinky || unitData.totalSkinData[j].currentColleagueType == ColleagueManager.ColleagueType.BabyDragon)
			{
				flyUnitList.Add(unitData.totalSkinData[j]);
			}
		}
		float flyUnitSpawnIntervalTime = 6f / (float)flyUnitList.Count;
		float timerForFlyUnit = 0f;
		int currentflyUnitIndex = 0;
		float timerForTank = 0f;
		int tankCurrentCount = 0;
		int tankSpawnMaxCount = unitData.tankData.Count;
		List<int> tankSpawnList = new List<int>();
		foreach (KeyValuePair<ObscuredInt, PVPTankData> item in unitData.tankData)
		{
			if ((bool)item.Value.isUnlocked)
			{
				tankSpawnList.Add(item.Value.tankIndex);
			}
		}
		while (true)
		{
			if (!GameManager.isPause)
			{
				timerForTank += Time.deltaTime * GameManager.timeScale;
				timerForFlyUnit += Time.deltaTime * GameManager.timeScale;
				if (timerForTank >= 0.5f && tankCurrentCount < tankSpawnMaxCount && tankSpawnList.Count > tankCurrentCount)
				{
					timerForTank -= 0.5f;
					spawnTank(unitData, isAlly, unitData.tankData[tankSpawnList[tankCurrentCount]]);
					tankCurrentCount++;
				}
				if (flyUnitList.Count > 0 && flyUnitList.Count > currentflyUnitIndex && timerForFlyUnit >= flyUnitSpawnIntervalTime)
				{
					timerForFlyUnit -= flyUnitSpawnIntervalTime;
					PVPManager.PVPSkinData skinData = flyUnitList[currentflyUnitIndex];
					spawnUnit(unitData, skinData, isAlly, 10, UnityEngine.Random.Range(4, 7));
					currentflyUnitIndex++;
				}
			}
			yield return null;
		}
	}

	private IEnumerator spawnDelayUpdate(PVPManager.PVPGameData unitData, bool isAlly)
	{
		StartCoroutine(spawnETCUnitUpdate(unitData, isAlly));
		sortUnitData(unitData);
		Dictionary<int, List<PVPManager.PVPSkinData>> totalSkinDataDictionary = new Dictionary<int, List<PVPManager.PVPSkinData>>();
		int columnCount = (int)Math.Ceiling((double)unitData.totalSkinData.Count / (double)maxLineCount);
		for (int i = 0; i < columnCount; i++)
		{
			if (!totalSkinDataDictionary.ContainsKey(i))
			{
				totalSkinDataDictionary.Add(i, new List<PVPManager.PVPSkinData>());
			}
			for (int j = maxLineCount * i; j < maxLineCount * i + maxLineCount; j++)
			{
				if (j < unitData.totalSkinData.Count)
				{
					totalSkinDataDictionary[i].Add(unitData.totalSkinData[j]);
				}
			}
		}
		float targetSpawnTime = 0.03f;
		float timer = targetSpawnTime;
		float timerForTank = 0f;
		int currentDictionaryIndex = 0;
		int currentListIndex = 0;
		int maxRowCount = 2;
		int currentRowCount = 0;
		while (totalSkinDataDictionary.ContainsKey(currentDictionaryIndex) && totalSkinDataDictionary[currentDictionaryIndex].Count > currentListIndex)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				timerForTank += Time.deltaTime * GameManager.timeScale;
				while (timer >= targetSpawnTime)
				{
					timer -= targetSpawnTime;
					PVPManager.PVPSkinData skinData = null;
					if (totalSkinDataDictionary.ContainsKey(currentDictionaryIndex) && totalSkinDataDictionary[currentDictionaryIndex].Count > currentListIndex)
					{
						skinData = totalSkinDataDictionary[currentDictionaryIndex][currentListIndex];
					}
					if (skinData == null)
					{
						break;
					}
					spawnUnit(unitData, skinData, isAlly, totalSkinDataDictionary[currentDictionaryIndex].Count, currentListIndex);
					currentListIndex++;
					if (currentListIndex >= maxLineCount)
					{
						currentDictionaryIndex++;
						currentListIndex = 0;
						if (currentRowCount >= maxRowCount - 1)
						{
							yield return new WaitForSeconds(0.27f);
							currentRowCount = 0;
						}
						else
						{
							yield return new WaitForSeconds(0.27f);
							currentRowCount++;
						}
					}
				}
			}
			yield return null;
		}
		if (isAlly)
		{
			m_allyDelayedSpawnUpdateCoroutine = null;
		}
		else
		{
			m_enemyDelayedSpawnUpdateCoroutine = null;
		}
	}

	private PVPUnitObject spawnUnit(PVPManager.PVPGameData baseGameData, PVPManager.PVPSkinData unitData, bool isAlly, int listMaxCount, int index)
	{
		string poolName = ((unitData.currentColleagueType != ColleagueManager.ColleagueType.None) ? ("@PVPColleague" + (int)(unitData.currentColleagueType + 1)) : "@PVPCharacter");
		PVPUnitObject pVPUnitObject = NewObjectPool.Spawn<PVPUnitObject>(poolName, new Vector2((!isAlly) ? 9 : (-9), getSpawnYPos(listMaxCount, index)));
		pVPUnitObject.isAlly = isAlly;
		pVPUnitObject.initUnit(baseGameData, unitData);
		addUnit(pVPUnitObject);
		return pVPUnitObject;
	}

	private PVPUnitObject spawnUnit(PVPManager.PVPGameData baseGameData, PVPManager.PVPSkinData unitData, bool isAlly, Vector2 position)
	{
		string poolName = ((unitData.currentColleagueType != ColleagueManager.ColleagueType.None) ? ("@PVPColleague" + (int)(unitData.currentColleagueType + 1)) : "@PVPCharacter");
		PVPUnitObject pVPUnitObject = NewObjectPool.Spawn<PVPUnitObject>(poolName, position);
		pVPUnitObject.isAlly = isAlly;
		pVPUnitObject.initUnit(baseGameData, unitData);
		addUnit(pVPUnitObject);
		return pVPUnitObject;
	}

	private void spawnTank(PVPManager.PVPGameData gameData, bool isAlly, PVPTankData tankData)
	{
		Vector2 tankInitPosition = getTankInitPosition(isAlly, tankData.tankIndex);
		tankInitPosition.x += ((!isAlly) ? 3f : (-3f));
		PVPTankObject pVPTankObject = NewObjectPool.Spawn<PVPTankObject>("@PVPTank", tankInitPosition);
		pVPTankObject.isAlly = isAlly;
		pVPTankObject.initTank(gameData, isAlly, tankData);
		totalTankList.Add(pVPTankObject);
	}

	private void spawnPrincess(bool isAlly)
	{
		int currentPrincessNumberForElopeMode = GameManager.getCurrentPrincessNumberForElopeMode();
		if (isAlly)
		{
			if (m_allyPrincessSpawnIndexList.Count < currentPrincessNumberForElopeMode)
			{
				int num = UnityEngine.Random.Range(1, currentPrincessNumberForElopeMode + 1);
				while (m_allyPrincessSpawnIndexList.Contains(num))
				{
					num = UnityEngine.Random.Range(1, currentPrincessNumberForElopeMode + 1);
				}
				PVPPrincessObject pVPPrincessObject = NewObjectPool.Spawn<PVPPrincessObject>("@PVPPrincess", getPrincessPosition(m_allyPrincessSpawnIndexList.Count, isAlly), Vector3.zero, new Vector3(-1f, 1f, 1f));
				pVPPrincessObject.initPrincess(num);
				m_totalPrincessObjectList.Add(pVPPrincessObject);
				m_totalAllyPrincessObjectList.Add(pVPPrincessObject);
				m_allyPrincessSpawnIndexList.Add(num);
			}
		}
		else if (m_enemyPrincessSpawnIndexList.Count < currentPrincessNumberForElopeMode)
		{
			int num2 = UnityEngine.Random.Range(1, currentPrincessNumberForElopeMode + 1);
			while (m_enemyPrincessSpawnIndexList.Contains(num2))
			{
				num2 = UnityEngine.Random.Range(1, currentPrincessNumberForElopeMode + 1);
			}
			PVPPrincessObject pVPPrincessObject2 = NewObjectPool.Spawn<PVPPrincessObject>("@PVPPrincess", getPrincessPosition(m_enemyPrincessSpawnIndexList.Count, isAlly), Vector3.zero, Vector3.one);
			pVPPrincessObject2.initPrincess(num2);
			m_totalPrincessObjectList.Add(pVPPrincessObject2);
			m_totalEnemyPrincessObjectList.Add(pVPPrincessObject2);
			m_enemyPrincessSpawnIndexList.Add(num2);
		}
	}

	public void clearAllEnemy()
	{
		for (int i = 0; i < totalTankList.Count; i++)
		{
			NewObjectPool.Recycle(totalTankList[i]);
		}
		for (int j = 0; j < totalAllyList.Count; j++)
		{
			NewObjectPool.Recycle(totalAllyList[j]);
		}
		for (int k = 0; k < totalEnemyList.Count; k++)
		{
			NewObjectPool.Recycle(totalEnemyList[k]);
		}
		for (int l = 0; l < m_totalPrincessObjectList.Count; l++)
		{
			NewObjectPool.Recycle(m_totalPrincessObjectList[l]);
		}
		totalAllyList.Clear();
		totalEnemyList.Clear();
		totalTankList.Clear();
		m_totalPrincessObjectList.Clear();
		m_totalAllyPrincessObjectList.Clear();
		m_totalEnemyPrincessObjectList.Clear();
		for (int m = 0; m < 4; m++)
		{
			totalAllyGroupByAttackType[(AttackType)m].Clear();
			totalEnemyGroupByAttackType[(AttackType)m].Clear();
		}
	}

	public PVPUnitObject getNearestUnit(Vector2 position, float range, bool isAlly)
	{
		PVPUnitObject result = null;
		List<PVPUnitObject> list = null;
		list = ((!isAlly) ? totalAllyList : totalEnemyList);
		float num = float.MaxValue;
		for (int i = 0; i < list.Count; i++)
		{
			float num2 = Vector2.Distance(position, list[i].cachedTransform.position);
			if (!list[i].isDead && num2 <= range && num2 < num)
			{
				num = num2;
				result = list[i];
			}
		}
		return result;
	}

	public PVPUnitObject getNearestUnit(Vector2 position, float range, bool isAlly, AttackType currentAttackType)
	{
		PVPUnitObject result = null;
		List<PVPUnitObject> list = null;
		list = ((!isAlly) ? totalAllyList : totalEnemyList);
		List<PVPUnitObject> list2 = null;
		switch (currentAttackType)
		{
		case AttackType.MeleeSingleAttack:
		case AttackType.MeleeSplashAttack:
			list2 = list;
			break;
		case AttackType.MiddleRange:
			list2 = ((!isAlly) ? ((totalAllyGroupByAttackType[AttackType.MeleeSingleAttack].Count <= 0) ? ((totalAllyGroupByAttackType[AttackType.MeleeSplashAttack].Count <= 0) ? list : totalAllyGroupByAttackType[AttackType.MeleeSplashAttack]) : totalAllyGroupByAttackType[AttackType.MeleeSingleAttack]) : ((totalEnemyGroupByAttackType[AttackType.MeleeSingleAttack].Count <= 0) ? ((totalEnemyGroupByAttackType[AttackType.MeleeSplashAttack].Count <= 0) ? list : totalEnemyGroupByAttackType[AttackType.MeleeSplashAttack]) : totalEnemyGroupByAttackType[AttackType.MeleeSingleAttack]));
			break;
		case AttackType.LongRange:
			list2 = ((!isAlly) ? ((totalAllyGroupByAttackType[AttackType.MiddleRange].Count <= 0) ? list : totalAllyGroupByAttackType[AttackType.MiddleRange]) : ((totalEnemyGroupByAttackType[AttackType.MiddleRange].Count <= 0) ? list : totalEnemyGroupByAttackType[AttackType.MiddleRange]));
			break;
		}
		float num = float.MaxValue;
		for (int i = 0; i < list2.Count; i++)
		{
			float num2 = Vector2.Distance(position, list[i].cachedTransform.position);
			if (!list[i].isDead && num2 <= range && num2 < num)
			{
				num = num2;
				result = list[i];
			}
		}
		return result;
	}

	public List<PVPUnitObject> getNearestUnits(Vector2 position, float range, bool isAlly)
	{
		m_unitListForGetNearestUnits.Clear();
		List<PVPUnitObject> list = null;
		list = ((!isAlly) ? totalAllyList : totalEnemyList);
		for (int i = 0; i < list.Count; i++)
		{
			float num = Vector2.Distance(position, list[i].cachedTransform.position);
			if (!list[i].isDead && num <= range)
			{
				m_unitListForGetNearestUnits.Add(list[i]);
			}
		}
		return m_unitListForGetNearestUnits;
	}

	public PVPUnitObject getLeaderUnit(bool isAlly)
	{
		PVPUnitObject result = null;
		List<PVPUnitObject> list = null;
		list = ((!isAlly) ? totalAllyList : totalEnemyList);
		float num = ((!isAlly) ? float.MinValue : float.MaxValue);
		for (int i = 0; i < list.Count; i++)
		{
			if (isAlly)
			{
				float x = list[i].cachedTransform.position.x;
				if (!list[i].isDead && x < num)
				{
					num = x;
					result = list[i];
				}
			}
			else
			{
				float x2 = list[i].cachedTransform.position.x;
				if (!list[i].isDead && x2 > num)
				{
					num = x2;
					result = list[i];
				}
			}
		}
		return result;
	}

	public bool isNoEnemy(bool isAlly)
	{
		return (!isAlly) ? (totalAllyList.Count == 0) : (totalEnemyList.Count == 0);
	}

	private float getRandomAllySpawnDelay(int spawnCount)
	{
		float num = 10f / (float)spawnCount;
		float min = num * UnityEngine.Random.Range(0.8f, 1.2f);
		float max = num * UnityEngine.Random.Range(0.8f, 1.2f);
		return UnityEngine.Random.Range(min, max);
	}

	public float getSpawnYPos(int maxCount, int index)
	{
		float num = 0f;
		maxCount++;
		index++;
		float num2 = (lineMaxPosition - lineMinPosition) / (float)maxCount;
		return lineMinPosition + num2 * (float)index;
	}

	public PVPUnitStatData getCalculatedUnitStat(PVPManager.PVPGameData pvpGameData, PVPManager.PVPSkinData unitData, AttackType attackType, bool isRandomize = true)
	{
		PVPUnitStatData result = default(PVPUnitStatData);
		float num = UnityEngine.Random.Range(0.9f, 1.1f);
		result.attackSpeed = 0.6f * num;
		result.criticalChance = 0f;
		result.criticalDamage = 0.0;
		result.moveSpeed = 1.5f;
		switch (attackType)
		{
		case AttackType.MeleeSingleAttack:
			result.damage = 30.0;
			result.hp = 300.0;
			result.moveSpeed = 1.875f;
			break;
		case AttackType.MeleeSplashAttack:
			result.damage = 25.0;
			result.hp = 900.0;
			result.moveSpeed = 1.7f;
			result.attackSpeed = 0.9f;
			break;
		case AttackType.MiddleRange:
			result.damage = 20.0;
			result.hp = 200.0;
			break;
		case AttackType.LongRange:
			result.damage = 50.0;
			result.hp = 150.0;
			break;
		}
		double num2 = 0.0;
		if (unitData.currentCharacterType != CharacterManager.CharacterType.Length)
		{
			CharacterSkinStatData characterSkinStatData = default(CharacterSkinStatData);
			switch (unitData.currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.warriorCharacterSkinData[unitData.currentWarriorSkinType];
				break;
			case CharacterManager.CharacterType.Priest:
				characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.priestCharacterSkinData[unitData.currentPriestSkinType];
				break;
			case CharacterManager.CharacterType.Archer:
				characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.archerCharacterSkinData[unitData.currentArcherSkinType];
				break;
			}
			num2 = Math.Max((double)(characterSkinStatData.percentDamage + characterSkinStatData.secondStat) / 2.0, 100.0);
		}
		else
		{
			num2 = Math.Max(Singleton<ColleagueManager>.instance.getColleagueSkinStat(unitData.currentColleagueType, (int)(long)unitData.currentColleagueSkinIndex), 100.0);
		}
		result.damage = result.damage / 100.0 * num2;
		result.hp = result.hp / 100.0 * num2;
		double calculatedDamageBonus = getCalculatedDamageBonus(pvpGameData.statData, attackType, unitData.currentColleagueType != ColleagueManager.ColleagueType.None);
		double calculatedHPBonus = getCalculatedHPBonus(pvpGameData.statData, unitData.skinLevel);
		result.damage += result.damage / 100.0 * calculatedDamageBonus;
		result.hp += result.hp / 100.0 * calculatedHPBonus;
		result.damage *= 1.0 + (double)((long)unitData.skinLevel - 1);
		result.hp *= 1.0 + (double)((long)unitData.skinLevel - 1);
		if (isRandomize)
		{
			if (this != Singleton<PVPUnitManager>.instance.allyLeaderWarrior && this != Singleton<PVPUnitManager>.instance.enemyLeaderWarrior)
			{
				result.moveSpeed *= UnityEngine.Random.Range(0.8f, 1.2f);
			}
			result.attackRange = getAttackRange(attackType) * UnityEngine.Random.Range(0.9f, 1.1f);
			result.damage *= num;
		}
		return result;
	}

	public PVPUnitStatData getCalculatedTankStat(PVPManager.PVPGameStatData statData, ObscuredLong tankLevel)
	{
		PVPUnitStatData result = default(PVPUnitStatData);
		float num = UnityEngine.Random.Range(0.9f, 1.1f);
		result.attackSpeed = 2f * num;
		result.criticalChance = 0f;
		result.criticalDamage = 0.0;
		result.hp = 0.0;
		result.damage = 40.0;
		result.moveSpeed = 1.1f;
		result.attackRange = 150f;
		result.damage *= num;
		double calculatedDamageBonus = getCalculatedDamageBonus(statData, AttackType.None, true);
		result.damage += result.damage / 100.0 * calculatedDamageBonus;
		result.damage *= 1.0 + (double)((long)tankLevel - 1);
		return result;
	}

	public AttackType getAttackType(PVPManager.PVPSkinData unitData)
	{
		AttackType result = AttackType.None;
		if (unitData.currentColleagueType == ColleagueManager.ColleagueType.None)
		{
			switch (unitData.currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				result = AttackType.MeleeSingleAttack;
				break;
			case CharacterManager.CharacterType.Priest:
				result = AttackType.MiddleRange;
				break;
			case CharacterManager.CharacterType.Archer:
				result = AttackType.LongRange;
				break;
			}
		}
		else
		{
			switch (unitData.currentColleagueType)
			{
			case ColleagueManager.ColleagueType.Isabelle:
			case ColleagueManager.ColleagueType.Stephanie:
			case ColleagueManager.ColleagueType.Scarlett:
			case ColleagueManager.ColleagueType.Sera:
			case ColleagueManager.ColleagueType.Pinky:
			case ColleagueManager.ColleagueType.Dinnerless:
			case ColleagueManager.ColleagueType.FatherKing:
			case ColleagueManager.ColleagueType.Seaghoul:
			case ColleagueManager.ColleagueType.Barbie:
			case ColleagueManager.ColleagueType.BabyDragon:
			case ColleagueManager.ColleagueType.Poty:
				result = AttackType.MiddleRange;
				break;
			case ColleagueManager.ColleagueType.Samantha:
			case ColleagueManager.ColleagueType.Puppy:
			case ColleagueManager.ColleagueType.Balbaria:
			case ColleagueManager.ColleagueType.Thyrael:
			case ColleagueManager.ColleagueType.GoldenFork:
			case ColleagueManager.ColleagueType.Kitty:
			case ColleagueManager.ColleagueType.HoneyQueen:
			case ColleagueManager.ColleagueType.Prince:
			case ColleagueManager.ColleagueType.InfernalDragon:
				result = AttackType.MeleeSingleAttack;
				break;
			case ColleagueManager.ColleagueType.Lawrence:
			case ColleagueManager.ColleagueType.Sushiro:
				result = AttackType.LongRange;
				break;
			case ColleagueManager.ColleagueType.Golem:
			case ColleagueManager.ColleagueType.Trall:
				result = AttackType.MeleeSplashAttack;
				break;
			}
		}
		return result;
	}

	public float getAttackRange(AttackType attackType)
	{
		float result = 0f;
		switch (attackType)
		{
		case AttackType.MeleeSingleAttack:
			result = 1.2f;
			break;
		case AttackType.MeleeSplashAttack:
			result = 1.4f;
			break;
		case AttackType.MiddleRange:
			result = 3.5f;
			break;
		case AttackType.LongRange:
			result = 7f;
			break;
		}
		return result;
	}

	public float getSplashRange(AttackType attackType)
	{
		float result = 0f;
		switch (attackType)
		{
		case AttackType.MeleeSplashAttack:
			result = 3f;
			break;
		case AttackType.MiddleRange:
			result = 1.4f;
			break;
		}
		return result;
	}

	public Vector2 getTankInitPosition(bool isAlly, int index)
	{
		Vector2 result = Vector2.zero;
		switch (index)
		{
		case 0:
			result = new Vector2((float)((!isAlly) ? 1 : (-1)) * 6.921f, -2.35f);
			break;
		case 1:
			result = new Vector2((float)((!isAlly) ? 1 : (-1)) * 6.7f, -1.42f);
			break;
		case 2:
			result = new Vector2((float)((!isAlly) ? 1 : (-1)) * 6.7f, -3.32f);
			break;
		case 3:
			result = new Vector2((float)((!isAlly) ? 1 : (-1)) * 6.45f, -0.62f);
			break;
		case 4:
			result = new Vector2((float)((!isAlly) ? 1 : (-1)) * 6.45f, -4.25f);
			break;
		}
		return result;
	}

	public double getCalculatedDamageBonus(PVPManager.PVPGameStatData statData, AttackType attackType, bool isColleague)
	{
		double num = 0.0;
		double num2 = ((!isColleague) ? statData.conquerorRingValue : statData.nobleBladeValue);
		double num3 = 0.0;
		switch (attackType)
		{
		case AttackType.MeleeSingleAttack:
		case AttackType.MeleeSplashAttack:
			num3 = statData.warriorCapeValue;
			break;
		case AttackType.MiddleRange:
			num3 = statData.angelHairPinValue;
			break;
		case AttackType.LongRange:
			num3 = statData.archerArrowValue;
			break;
		default:
			num3 = 0.0;
			break;
		}
		double num4 = 0.0;
		if (UnbiasedTime.Instance.Now().Hour >= 9 && UnbiasedTime.Instance.Now().Hour <= 20)
		{
			num4 = statData.heliosHarpValue;
		}
		else if (UnbiasedTime.Instance.Now().Hour <= 5 || UnbiasedTime.Instance.Now().Hour >= 21)
		{
			num4 = statData.charmOfLunarGoddessValue;
		}
		double num5 = statData.treasureBonus * 10.0;
		return (1.0 + num5 + (num2 + num4)) * (1.0 + statData.patienceTokenValue + num3 + statData.seraphHopeValue + (statData.princessBonus * 0.15 + statData.rebirthCount)) * 0.001;
	}

	public double getCalculatedHPBonus(PVPManager.PVPGameStatData statData, long skinLevel)
	{
		double num = 0.0;
		double num2 = statData.treasureBonus * 10.0;
		return (1.0 + num2 + statData.heavenShieldValue) * (1.0 + statData.conquerTokenValue + statData.seraphBlessValue + statData.princessBonus * 0.15 + statData.rebirthCount) * 0.006;
	}

	private Vector3 getPrincessPosition(int index, bool isAlly)
	{
		Vector3 result = Vector3.zero;
		switch (index)
		{
		case 0:
			result = new Vector3((float)((!isAlly) ? 1 : (-1)) * 4.801f, 1.882f, -0.5f);
			break;
		case 1:
			result = new Vector3((float)((!isAlly) ? 1 : (-1)) * 5.39f, 1.678f, -1f);
			break;
		case 2:
			result = new Vector3((float)((!isAlly) ? 1 : (-1)) * 5.946f, 1.529f, -1.5f);
			break;
		case 3:
			result = new Vector3((float)((!isAlly) ? 1 : (-1)) * 6.393f, 1.285f, -2f);
			break;
		}
		return result;
	}
}
