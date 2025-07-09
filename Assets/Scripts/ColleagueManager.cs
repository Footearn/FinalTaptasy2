using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ColleagueManager : Singleton<ColleagueManager>
{
	public enum ColleagueType
	{
		None = -1,
		Isabelle,
		Samantha,
		Lawrence,
		Stephanie,
		Puppy,
		Scarlett,
		Balbaria,
		Sera,
		Sushiro,
		Pinky,
		Thyrael,
		Dinnerless,
		FatherKing,
		GoldenFork,
		Kitty,
		Seaghoul,
		HoneyQueen,
		Barbie,
		Prince,
		BabyDragon,
		Golem,
		Angelica,
		Poty,
		Angelina,
		Trall,
		Angela,
		Goblin,
		InfernalDragon,
		Length
	}

	[Serializable]
	public struct ColleagueBulletAttributeData
	{
		public Sprite bulletSprite;

		public bool isImmediateBullet;

		public bool isParabolic;

		public bool isRotatable;

		public float speed;
	}

	[Serializable]
	public struct ColleagueBulletData
	{
		public ColleagueType currentColleagueType;

		public List<ColleagueBulletAttributeData> bulletAttributes;
	}

	public enum PassiveSkillTierType
	{
		None = -1,
		PassiveSkillTier1,
		PassiveSkillTier2,
		PassiveSkillTier3,
		PassiveSkillTier4,
		PassiveSkillTier5,
		PassiveSkillTier6,
		Length
	}

	public enum ColleaguePassiveSkillType
	{
		None = -1,
		Damage,
		CriticalChance,
		CriticalDamage,
		Health,
		TapHeal,
		Length
	}

	public enum ColleaguePassiveTargetType
	{
		None = -1,
		MySelf,
		Warrior,
		Priest,
		Archer,
		All,
		Legnth
	}

	public List<ColleagueObject> currentColleagueObject;

	public Dictionary<ColleagueType, Dictionary<int, CharacterManager.CharacterBoneAnimationSpriteData>> colleagueBoneSpriteDictionary;

	public Dictionary<ColleagueType, List<int>> colleagueSkinMaxCountList;

	public List<ColleagueBulletData> colleagueBulletDataList;

	public List<ColleagueType> colleagueSlotTypeList;

	public List<ColleagueType> normalColleagueTypeList;

	public Sprite[] colleaguePassiveSkillIconSprites;

	private Dictionary<ColleaguePassiveSkillType, Dictionary<ColleaguePassiveTargetType, Sprite>> m_currentPassiveSkillIconDictionary;

	[NonSerialized]
	public long multipleForInfinityLevelup = 25L;

	[ContextMenu("Reset Depth")]
	public void ResetDepth()
	{
		List<ColleagueObject> list = new List<ColleagueObject>();
		list.Clear();
		ColleagueObject[] array = Resources.LoadAll<ColleagueObject>("Prefabs/Ingame/Colleagues");
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		list.Sort((ColleagueObject colleague1, ColleagueObject colleague2) => colleague2.targetXOffsetForNormalDungeon.CompareTo(colleague1.targetXOffsetForNormalDungeon));
		int num = -1;
		int num2 = 9;
		for (int j = 0; j < list.Count; j++)
		{
			CharacterManager.CharacterBoneAnimationSpriteRendererData colleagueBoneSpriteRendererData = list[j].colleagueBoneSpriteRendererData;
			if (list[j].currentColleagueType == ColleagueType.Puppy || list[j].currentColleagueType == ColleagueType.Kitty || list[j].currentColleagueType == ColleagueType.BabyDragon || list[j].currentColleagueType == ColleagueType.Pinky || list[j].currentColleagueType == ColleagueType.Angelica || list[j].currentColleagueType == ColleagueType.Angela || list[j].currentColleagueType == ColleagueType.Goblin)
			{
				if (colleagueBoneSpriteRendererData.capeSpriteRenderer != null)
				{
					colleagueBoneSpriteRendererData.capeSpriteRenderer.sortingLayerName = "Player";
					colleagueBoneSpriteRendererData.capeSpriteRenderer.sortingOrder = num2;
					num2++;
				}
				if (colleagueBoneSpriteRendererData.leftWingSpriteRenderer != null)
				{
					colleagueBoneSpriteRendererData.leftWingSpriteRenderer.sortingLayerName = "Player";
					colleagueBoneSpriteRendererData.leftWingSpriteRenderer.sortingOrder = num2;
					num2++;
				}
				if (colleagueBoneSpriteRendererData.hairSpriteRenderer != null)
				{
					colleagueBoneSpriteRendererData.hairSpriteRenderer.sortingLayerName = "Player";
					colleagueBoneSpriteRendererData.hairSpriteRenderer.sortingOrder = num2;
					num2++;
				}
				if (colleagueBoneSpriteRendererData.tailSpriteRenderer != null || colleagueBoneSpriteRendererData.legSpriteRenderer != null)
				{
					if (colleagueBoneSpriteRendererData.tailSpriteRenderer != null)
					{
						colleagueBoneSpriteRendererData.tailSpriteRenderer.sortingLayerName = "Player";
						colleagueBoneSpriteRendererData.tailSpriteRenderer.sortingOrder = num2;
					}
					if (colleagueBoneSpriteRendererData.legSpriteRenderer != null)
					{
						for (int k = 0; k < colleagueBoneSpriteRendererData.legSpriteRenderer.Length; k++)
						{
							colleagueBoneSpriteRendererData.legSpriteRenderer[k].sortingLayerName = "Player";
							colleagueBoneSpriteRendererData.legSpriteRenderer[k].sortingOrder = num2;
						}
					}
					num2++;
				}
				if (colleagueBoneSpriteRendererData.spineSpriteRenderer != null)
				{
					colleagueBoneSpriteRendererData.spineSpriteRenderer.sortingLayerName = "Player";
					colleagueBoneSpriteRendererData.spineSpriteRenderer.sortingOrder = num2;
					num2++;
				}
				if (colleagueBoneSpriteRendererData.rightWingSpriteRenderer != null || colleagueBoneSpriteRendererData.headSpriteRenderer != null)
				{
					if (colleagueBoneSpriteRendererData.rightWingSpriteRenderer != null)
					{
						colleagueBoneSpriteRendererData.rightWingSpriteRenderer.sortingLayerName = "Player";
						colleagueBoneSpriteRendererData.rightWingSpriteRenderer.sortingOrder = num2;
					}
					if (colleagueBoneSpriteRendererData.headSpriteRenderer != null)
					{
						colleagueBoneSpriteRendererData.headSpriteRenderer.sortingLayerName = "Player";
						colleagueBoneSpriteRendererData.headSpriteRenderer.sortingOrder = num2;
					}
					num2++;
				}
				continue;
			}
			if (colleagueBoneSpriteRendererData.shieldSpriteRenderer != null)
			{
				colleagueBoneSpriteRendererData.shieldSpriteRenderer.sortingLayerName = "Player";
				colleagueBoneSpriteRendererData.shieldSpriteRenderer.sortingOrder = num;
				num--;
			}
			if (colleagueBoneSpriteRendererData.headSpriteRenderer != null)
			{
				colleagueBoneSpriteRendererData.headSpriteRenderer.sortingLayerName = "Player";
				colleagueBoneSpriteRendererData.headSpriteRenderer.sortingOrder = num;
				num--;
			}
			if (colleagueBoneSpriteRendererData.rightHandSpriteRenderer != null)
			{
				colleagueBoneSpriteRendererData.rightHandSpriteRenderer.sortingLayerName = "Player";
				colleagueBoneSpriteRendererData.rightHandSpriteRenderer.sortingOrder = num;
				num--;
			}
			if (colleagueBoneSpriteRendererData.spineSpriteRenderer != null)
			{
				colleagueBoneSpriteRendererData.spineSpriteRenderer.sortingLayerName = "Player";
				colleagueBoneSpriteRendererData.spineSpriteRenderer.sortingOrder = num;
				num--;
			}
			if (colleagueBoneSpriteRendererData.leftFingerSpriteRenderer != null)
			{
				colleagueBoneSpriteRendererData.leftFingerSpriteRenderer.sortingLayerName = "Player";
				colleagueBoneSpriteRendererData.leftFingerSpriteRenderer.sortingOrder = num;
				num--;
			}
			if (colleagueBoneSpriteRendererData.leftHandSpriteRenderer != null)
			{
				colleagueBoneSpriteRendererData.leftHandSpriteRenderer.sortingLayerName = "Player";
				colleagueBoneSpriteRendererData.leftHandSpriteRenderer.sortingOrder = num;
				num--;
			}
			if (colleagueBoneSpriteRendererData.rightLegSpriteRenderer != null || colleagueBoneSpriteRendererData.leftLegSpriteRenderer != null || colleagueBoneSpriteRendererData.weaponSpriteRenderer != null || colleagueBoneSpriteRendererData.legSpriteRenderer != null)
			{
				if (colleagueBoneSpriteRendererData.legSpriteRenderer != null)
				{
					for (int l = 0; l < colleagueBoneSpriteRendererData.legSpriteRenderer.Length; l++)
					{
						colleagueBoneSpriteRendererData.legSpriteRenderer[l].sortingLayerName = "Player";
						colleagueBoneSpriteRendererData.legSpriteRenderer[l].sortingOrder = num;
					}
				}
				if (colleagueBoneSpriteRendererData.rightLegSpriteRenderer != null)
				{
					colleagueBoneSpriteRendererData.rightLegSpriteRenderer.sortingLayerName = "Player";
					colleagueBoneSpriteRendererData.rightLegSpriteRenderer.sortingOrder = num;
				}
				if (colleagueBoneSpriteRendererData.leftLegSpriteRenderer != null)
				{
					colleagueBoneSpriteRendererData.leftLegSpriteRenderer.sortingLayerName = "Player";
					colleagueBoneSpriteRendererData.leftLegSpriteRenderer.sortingOrder = num;
				}
				if (colleagueBoneSpriteRendererData.weaponSpriteRenderer != null)
				{
					colleagueBoneSpriteRendererData.weaponSpriteRenderer.sortingLayerName = "Player";
					colleagueBoneSpriteRendererData.weaponSpriteRenderer.sortingOrder = num;
				}
				num--;
			}
			if (colleagueBoneSpriteRendererData.hairSpriteRenderer != null)
			{
				colleagueBoneSpriteRendererData.hairSpriteRenderer.sortingLayerName = "Player";
				colleagueBoneSpriteRendererData.hairSpriteRenderer.sortingOrder = num;
				num--;
			}
			if (colleagueBoneSpriteRendererData.capeSpriteRenderer != null)
			{
				colleagueBoneSpriteRendererData.capeSpriteRenderer.sortingLayerName = "Player";
				colleagueBoneSpriteRendererData.capeSpriteRenderer.sortingOrder = num;
				num--;
			}
			if (colleagueBoneSpriteRendererData.leftWingSpriteRenderer != null || colleagueBoneSpriteRendererData.rightWingSpriteRenderer != null)
			{
				if (colleagueBoneSpriteRendererData.leftWingSpriteRenderer != null)
				{
					colleagueBoneSpriteRendererData.leftWingSpriteRenderer.sortingLayerName = "Player";
					colleagueBoneSpriteRendererData.leftWingSpriteRenderer.sortingOrder = num;
				}
				if (colleagueBoneSpriteRendererData.rightWingSpriteRenderer != null)
				{
					colleagueBoneSpriteRendererData.rightWingSpriteRenderer.sortingLayerName = "Player";
					colleagueBoneSpriteRendererData.rightWingSpriteRenderer.sortingOrder = num;
				}
				num--;
			}
		}
	}

	private void Awake()
	{
		loadColleagueSpriteData();
		registryColleaguePassiveSkillIcon();
	}

	private void Start()
	{
		Singleton<DataManager>.instance.checkNewColleagueSkins();
		refreshSlotUnlockStatus();
		refreshColleagueSlotList();
	}

	private void refreshSlotUnlockStatus()
	{
		for (int i = 0; i <= 23; i++)
		{
			ColleagueType colleagueType = (ColleagueType)i;
			getColleagueInventoryData(colleagueType).isUnlockedFromSlot = true;
		}
		for (int j = 0; j < 28; j++)
		{
			ColleagueType colleagueType2 = (ColleagueType)j;
			if (isPremiumColleague(colleagueType2))
			{
				getColleagueInventoryData(colleagueType2).isUnlockedFromSlot = true;
			}
		}
	}

	public void refreshColleagueSlotList()
	{
		UIWindowColleague.instance.colleagueScrollRectParent.refreshMaxCount(Singleton<ColleagueManager>.instance.getCanSpawnSlotTotalCount());
		if (colleagueSlotTypeList == null)
		{
			colleagueSlotTypeList = new List<ColleagueType>();
		}
		if (normalColleagueTypeList == null)
		{
			normalColleagueTypeList = new List<ColleagueType>();
		}
		colleagueSlotTypeList.Clear();
		normalColleagueTypeList.Clear();
		normalColleagueTypeList.Add(ColleagueType.Isabelle);
		colleagueSlotTypeList.Add(ColleagueType.Isabelle);
		for (int i = 0; i < 2; i++)
		{
			bool flag = ((i == 0) ? true : false);
			for (int j = 1; j < 28; j++)
			{
				ColleagueType colleagueType = (ColleagueType)j;
				if (flag)
				{
					if (isPremiumColleague(colleagueType))
					{
						colleagueSlotTypeList.Add(colleagueType);
					}
				}
				else if (!isPremiumColleague(colleagueType))
				{
					colleagueSlotTypeList.Add(colleagueType);
					normalColleagueTypeList.Add(colleagueType);
				}
			}
		}
	}

	private void loadColleagueSpriteData()
	{
		colleagueSkinMaxCountList = new Dictionary<ColleagueType, List<int>>();
		for (int i = 0; i < 28; i++)
		{
			colleagueSkinMaxCountList.Add((ColleagueType)i, new List<int>());
		}
		colleagueBoneSpriteDictionary = new Dictionary<ColleagueType, Dictionary<int, CharacterManager.CharacterBoneAnimationSpriteData>>();
		Sprite[] ingameAtlas = Singleton<ResourcesManager>.instance.ingameAtlas;
		Dictionary<string, List<Sprite>> dictionary = new Dictionary<string, List<Sprite>>();
		for (int j = 0; j < ingameAtlas.Length; j++)
		{
			if (ingameAtlas[j].name.Contains("Fellow"))
			{
				if (!dictionary.ContainsKey(ingameAtlas[j].name.Replace("kin", string.Empty).Split('S')[0]))
				{
					dictionary.Add(ingameAtlas[j].name.Replace("kin", string.Empty).Split('S')[0], new List<Sprite>());
				}
				dictionary[ingameAtlas[j].name.Replace("kin", string.Empty).Split('S')[0]].Add(ingameAtlas[j]);
			}
		}
		foreach (KeyValuePair<string, List<Sprite>> item in dictionary)
		{
			for (int k = 0; k < 28; k++)
			{
				ColleagueType colleagueType = (ColleagueType)k;
				string text = "Fellow" + (int)(colleagueType + 1);
				if (!item.Key.Equals(text))
				{
					continue;
				}
				if (!colleagueBoneSpriteDictionary.ContainsKey(colleagueType))
				{
					colleagueBoneSpriteDictionary.Add(colleagueType, new Dictionary<int, CharacterManager.CharacterBoneAnimationSpriteData>());
				}
				Dictionary<int, CharacterManager.CharacterBoneAnimationSpriteData> dictionary2 = colleagueBoneSpriteDictionary[colleagueType];
				List<Sprite> value = item.Value;
				for (int l = 0; l < value.Count; l++)
				{
					int num = int.Parse(value[l].name.Replace(text, string.Empty).Split('-')[0].Replace("Skin", string.Empty));
					if (!colleagueSkinMaxCountList[colleagueType].Contains(num))
					{
						colleagueSkinMaxCountList[colleagueType].Add(num);
					}
					if (!dictionary2.ContainsKey(num))
					{
						dictionary2.Add(num, new CharacterManager.CharacterBoneAnimationSpriteData());
					}
					CharacterManager.CharacterBoneAnimationSpriteData characterBoneAnimationSpriteData = dictionary2[num];
					string value2 = text + "Skin" + num + "-Body";
					string value3 = text + "Skin" + num + "-Head";
					string value4 = text + "Skin" + num + "-Hair";
					string value5 = text + "Skin" + num + "-Leg";
					string value6 = text + "Skin" + num + "-L_Leg";
					string value7 = text + "Skin" + num + "-R_Leg";
					string value8 = text + "Skin" + num + "-Shield";
					string value9 = text + "Skin" + num + "-Cape";
					string value10 = text + "Skin" + num + "-L_Wing";
					string value11 = text + "Skin" + num + "-R_Wing";
					string value12 = text + "Skin" + num + "-Wing";
					string value13 = text + "Skin" + num + "-Weapon";
					string value14 = text + "Skin" + num + "-Tail";
					string value15 = text + "Skin" + num + "-L_Hand";
					string value16 = text + "Skin" + num + "-R_Hand";
					string value17 = text + "Skin" + num + "-L_Finger";
					string value18 = text + "Skin" + num + "-R_Finger";
					if (value[l].name.Equals(value2))
					{
						characterBoneAnimationSpriteData.spineSprite = value[l];
					}
					else if (value[l].name.Equals(value3))
					{
						characterBoneAnimationSpriteData.headSprite = value[l];
					}
					else if (value[l].name.Equals(value4))
					{
						characterBoneAnimationSpriteData.hairSprite = value[l];
					}
					else if (value[l].name.Equals(value5))
					{
						characterBoneAnimationSpriteData.legSprite = value[l];
					}
					else if (value[l].name.Equals(value8))
					{
						characterBoneAnimationSpriteData.shieldSprite = value[l];
					}
					else if (value[l].name.Equals(value9))
					{
						characterBoneAnimationSpriteData.capeSprite = value[l];
					}
					else if (value[l].name.Equals(value10))
					{
						characterBoneAnimationSpriteData.leftWingSprite = value[l];
					}
					else if (value[l].name.Equals(value11))
					{
						characterBoneAnimationSpriteData.rightWingSprite = value[l];
					}
					else if (value[l].name.Equals(value13))
					{
						characterBoneAnimationSpriteData.weaponSprite = value[l];
					}
					else if (value[l].name.Equals(value14))
					{
						characterBoneAnimationSpriteData.tailSprite = value[l];
					}
					else if (value[l].name.Equals(value12))
					{
						characterBoneAnimationSpriteData.rightWingSprite = value[l];
						characterBoneAnimationSpriteData.leftWingSprite = value[l];
					}
					else if (value[l].name.Equals(value15))
					{
						characterBoneAnimationSpriteData.leftHandSprite = value[l];
					}
					else if (value[l].name.Equals(value16))
					{
						characterBoneAnimationSpriteData.rightHandSprite = value[l];
					}
					else if (value[l].name.Equals(value6))
					{
						characterBoneAnimationSpriteData.leftLegSprite = value[l];
					}
					else if (value[l].name.Equals(value7))
					{
						characterBoneAnimationSpriteData.rightLegSprite = value[l];
					}
					else if (value[l].name.Equals(value17))
					{
						characterBoneAnimationSpriteData.leftFingerSprite = value[l];
					}
					else if (value[l].name.Equals(value18))
					{
						characterBoneAnimationSpriteData.rightFingerSprite = value[l];
					}
				}
			}
		}
	}

	public void calculateAllColleagueSkinStat()
	{
		Singleton<StatManager>.instance.colleagueAllPercentDamageFromColleagueSkin = 0.0;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.colleagueInventoryList.Count; i++)
		{
			foreach (KeyValuePair<int, bool> colleagueSkinDatum in Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[i].colleagueSkinData)
			{
				if (colleagueSkinDatum.Value)
				{
					Singleton<StatManager>.instance.colleagueAllPercentDamageFromColleagueSkin += getColleagueSkinStat(Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[i].colleagueType, colleagueSkinDatum.Key);
				}
			}
		}
	}

	public void buyColleagueSkin(ColleagueType colleagueType, int skinIndex)
	{
		ColleagueInventoryData colleagueInventoryData = getColleagueInventoryData(colleagueType);
		bool flag = !colleagueInventoryData.colleagueSkinData[skinIndex];
		colleagueInventoryData.colleagueSkinData[skinIndex] = true;
		if (flag)
		{
			colleagueInventoryData.colleaugeSkinLevelData[skinIndex] = 1L;
		}
		else
		{
			Dictionary<int, ObscuredLong> colleaugeSkinLevelData;
			Dictionary<int, ObscuredLong> dictionary = (colleaugeSkinLevelData = colleagueInventoryData.colleaugeSkinLevelData);
			int key;
			int key2 = (key = skinIndex);
			ObscuredLong obscuredLong = colleaugeSkinLevelData[key];
			ObscuredLong obscuredLong2 = obscuredLong;
			dictionary[key2] = ++obscuredLong2;
			long num = Base36.Decode(Singleton<DataManager>.instance.currentGameData.colleagueSkinTotalLevelUpRecord);
			num++;
			Singleton<DataManager>.instance.currentGameData.colleagueSkinTotalLevelUpRecord = Base36.Encode(num);
		}
		checkToDeleteShopItem();
	}

	public void equipColleagueSkin(ColleagueType colleagueType, int skinIndex)
	{
		ColleagueInventoryData colleagueInventoryData = getColleagueInventoryData(colleagueType);
		if (colleagueInventoryData.colleagueSkinData[skinIndex])
		{
			colleagueInventoryData.currentEquippedSkinIndex = skinIndex;
		}
	}

	public void checkToDeleteShopItem()
	{
		int num = 0;
		if (containColleagueSkin(ColleagueType.Isabelle, 4))
		{
			num++;
		}
		if (containColleagueSkin(ColleagueType.Samantha, 4))
		{
			num++;
		}
		if (containColleagueSkin(ColleagueType.Lawrence, 4))
		{
			num++;
		}
		if (num >= 3 && Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.ColleagueSkinPackageA.ToString()))
		{
			Singleton<ShopManager>.instance.consumeLimitedItem(Singleton<ShopManager>.instance.getLimitedItemDataFromCurrentInventory(ShopManager.LimitedShopItemType.ColleagueSkinPackageA.ToString()));
		}
	}

	public void refreshStartPositions()
	{
		for (int i = 0; i < currentColleagueObject.Count; i++)
		{
			currentColleagueObject[i].resetColleagueStartPosition();
		}
	}

	private void recycleColleagues()
	{
		for (int i = 0; i < currentColleagueObject.Count; i++)
		{
			ObjectPool.Recycle(currentColleagueObject[i].name, currentColleagueObject[i].cachedGameObject);
		}
		currentColleagueObject.Clear();
	}

	private void createColleagues()
	{
		recycleColleagues();
		if (TutorialManager.isTutorial)
		{
			if (getColleagueInventoryData(ColleagueType.Isabelle).isUnlocked)
			{
				for (int i = 0; i < 28; i++)
				{
					ColleagueObject component = ObjectPool.Spawn("@Colleague" + (int)(Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[i].colleagueType + 1), Vector2.zero, Singleton<CachedManager>.instance.colleagueTransform).GetComponent<ColleagueObject>();
					currentColleagueObject.Add(component);
				}
			}
			return;
		}
		for (int j = 0; j < 28; j++)
		{
			if (Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[j].isUnlocked)
			{
				ColleagueObject component2 = ObjectPool.Spawn("@Colleague" + (int)(Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[j].colleagueType + 1), Vector2.zero, Singleton<CachedManager>.instance.colleagueTransform).GetComponent<ColleagueObject>();
				currentColleagueObject.Add(component2);
			}
		}
	}

	public void startGame()
	{
		createColleagues();
		for (int i = 0; i < currentColleagueObject.Count; i++)
		{
			currentColleagueObject[i].initColleague();
		}
		refreshStartPositions();
	}

	public void endGame()
	{
		for (int i = 0; i < currentColleagueObject.Count; i++)
		{
			currentColleagueObject[i].setStateLock(false);
			currentColleagueObject[i].setState(PublicDataManager.State.Wait);
		}
		recycleColleagues();
	}

	public bool colleagueLevelUp(ColleagueInventoryData inventoryData, double levelUpPrice, long targetLevelupCount = 1)
	{
		bool flag = false;
		if (Singleton<DataManager>.instance.currentGameData.gold >= levelUpPrice)
		{
			Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.Companionship, targetLevelupCount);
			inventoryData.level += targetLevelupCount;
			Singleton<GoldManager>.instance.decreaseGold(levelUpPrice);
			Singleton<DataManager>.instance.saveData();
			UIWindowColleague.instance.refreshTotalDamage();
			return true;
		}
		return false;
	}

	public void changeColleagueSkin(ColleagueType colleagueType, int skinIndex, CharacterManager.CharacterBoneAnimationSpriteRendererData spriteRendererData)
	{
		Dictionary<int, CharacterManager.CharacterBoneAnimationSpriteData> dictionary = colleagueBoneSpriteDictionary[colleagueType];
		GameObject gameObject = spriteRendererData.shieldSpriteRenderer.gameObject;
		if (!dictionary.ContainsKey(skinIndex))
		{
			Debug.Log("changeColleagueSkin received wrong skinIndex: " + skinIndex);
			return;
		}
		if (dictionary[skinIndex].shieldSprite != null)
		{
			gameObject.SetActive(true);
			spriteRendererData.shieldSpriteRenderer.sprite = dictionary[skinIndex].shieldSprite;
		}
		else
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = spriteRendererData.headSpriteRenderer.gameObject;
		if (dictionary[skinIndex].headSprite != null)
		{
			gameObject2.SetActive(true);
			spriteRendererData.headSpriteRenderer.sprite = dictionary[skinIndex].headSprite;
		}
		else
		{
			gameObject2.SetActive(false);
		}
		GameObject gameObject3 = spriteRendererData.hairSpriteRenderer.gameObject;
		if (dictionary[skinIndex].hairSprite != null)
		{
			gameObject3.SetActive(true);
			spriteRendererData.hairSpriteRenderer.sprite = dictionary[skinIndex].hairSprite;
		}
		else
		{
			gameObject3.SetActive(false);
		}
		GameObject gameObject4 = spriteRendererData.spineSpriteRenderer.gameObject;
		if (dictionary[skinIndex].spineSprite != null)
		{
			gameObject4.SetActive(true);
			spriteRendererData.spineSpriteRenderer.sprite = dictionary[skinIndex].spineSprite;
		}
		else
		{
			gameObject4.SetActive(false);
		}
		if (spriteRendererData.legSpriteRenderer != null)
		{
			for (int i = 0; i < spriteRendererData.legSpriteRenderer.Length; i++)
			{
				GameObject gameObject5 = spriteRendererData.legSpriteRenderer[i].gameObject;
				if (dictionary[skinIndex].legSprite != null)
				{
					gameObject5.SetActive(true);
					spriteRendererData.legSpriteRenderer[i].sprite = dictionary[skinIndex].legSprite;
				}
				else
				{
					gameObject5.SetActive(false);
				}
			}
		}
		if (spriteRendererData.leftWingSpriteRenderer != null)
		{
			GameObject gameObject6 = spriteRendererData.leftWingSpriteRenderer.gameObject;
			if (dictionary[skinIndex].leftWingSprite != null)
			{
				gameObject6.SetActive(true);
				spriteRendererData.leftWingSpriteRenderer.sprite = dictionary[skinIndex].leftWingSprite;
			}
			else
			{
				gameObject6.SetActive(false);
			}
		}
		if (spriteRendererData.rightWingSpriteRenderer != null)
		{
			GameObject gameObject7 = spriteRendererData.rightWingSpriteRenderer.gameObject;
			if (dictionary[skinIndex].rightWingSprite != null)
			{
				gameObject7.SetActive(true);
				spriteRendererData.rightWingSpriteRenderer.sprite = dictionary[skinIndex].rightWingSprite;
			}
			else
			{
				gameObject7.SetActive(false);
			}
		}
		if (spriteRendererData.capeSpriteRenderer != null)
		{
			GameObject gameObject8 = spriteRendererData.capeSpriteRenderer.gameObject;
			if (dictionary[skinIndex].capeSprite != null)
			{
				gameObject8.SetActive(true);
				spriteRendererData.capeSpriteRenderer.sprite = dictionary[skinIndex].capeSprite;
			}
			else
			{
				gameObject8.SetActive(false);
			}
		}
		if (spriteRendererData.weaponSpriteRenderer != null)
		{
			GameObject gameObject9 = spriteRendererData.weaponSpriteRenderer.gameObject;
			if (dictionary[skinIndex].weaponSprite != null)
			{
				gameObject9.SetActive(true);
				spriteRendererData.weaponSpriteRenderer.sprite = dictionary[skinIndex].weaponSprite;
			}
			else
			{
				gameObject9.SetActive(false);
			}
		}
		if (spriteRendererData.tailSpriteRenderer != null)
		{
			GameObject gameObject10 = spriteRendererData.tailSpriteRenderer.gameObject;
			if (dictionary[skinIndex].tailSprite != null)
			{
				gameObject10.SetActive(true);
				spriteRendererData.tailSpriteRenderer.sprite = dictionary[skinIndex].tailSprite;
			}
			else
			{
				gameObject10.SetActive(false);
			}
		}
		if (spriteRendererData.leftHandSpriteRenderer != null)
		{
			GameObject gameObject11 = spriteRendererData.leftHandSpriteRenderer.gameObject;
			if (dictionary[skinIndex].leftHandSprite != null)
			{
				gameObject11.SetActive(true);
				spriteRendererData.leftHandSpriteRenderer.sprite = dictionary[skinIndex].leftHandSprite;
			}
			else
			{
				gameObject11.SetActive(false);
			}
		}
		if (spriteRendererData.rightHandSpriteRenderer != null)
		{
			GameObject gameObject12 = spriteRendererData.rightHandSpriteRenderer.gameObject;
			if (dictionary[skinIndex].rightHandSprite != null)
			{
				gameObject12.SetActive(true);
				spriteRendererData.rightHandSpriteRenderer.sprite = dictionary[skinIndex].rightHandSprite;
			}
			else
			{
				gameObject12.SetActive(false);
			}
		}
		if (spriteRendererData.leftLegSpriteRenderer != null)
		{
			GameObject gameObject13 = spriteRendererData.leftLegSpriteRenderer.gameObject;
			if (dictionary[skinIndex].leftLegSprite != null)
			{
				gameObject13.SetActive(true);
				spriteRendererData.leftLegSpriteRenderer.sprite = dictionary[skinIndex].leftLegSprite;
			}
			else
			{
				gameObject13.SetActive(false);
			}
		}
		if (spriteRendererData.rightLegSpriteRenderer != null)
		{
			GameObject gameObject14 = spriteRendererData.rightLegSpriteRenderer.gameObject;
			if (dictionary[skinIndex].rightLegSprite != null)
			{
				gameObject14.SetActive(true);
				spriteRendererData.rightLegSpriteRenderer.sprite = dictionary[skinIndex].rightLegSprite;
			}
			else
			{
				gameObject14.SetActive(false);
			}
		}
		if (spriteRendererData.leftFingerSpriteRenderer != null)
		{
			GameObject gameObject15 = spriteRendererData.leftFingerSpriteRenderer.gameObject;
			if (dictionary[skinIndex].leftFingerSprite != null)
			{
				gameObject15.SetActive(true);
				spriteRendererData.leftFingerSpriteRenderer.sprite = dictionary[skinIndex].leftFingerSprite;
			}
			else
			{
				gameObject15.SetActive(false);
			}
		}
		if (spriteRendererData.rightFingerSpriteRenderer != null)
		{
			GameObject gameObject16 = spriteRendererData.rightFingerSpriteRenderer.gameObject;
			if (dictionary[skinIndex].rightFingerSprite != null)
			{
				gameObject16.SetActive(true);
				spriteRendererData.rightFingerSpriteRenderer.sprite = dictionary[skinIndex].rightFingerSprite;
			}
			else
			{
				gameObject16.SetActive(false);
			}
		}
	}

	public double getCurrentDPS()
	{
		double num = 0.0;
		for (int i = 0; i < 28; i++)
		{
			ColleagueInventoryData colleagueInventoryData = Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[i];
			if (!isPremiumColleague(colleagueInventoryData.colleagueType) && colleagueInventoryData.isUnlocked)
			{
				num += getColleagueDamage(colleagueInventoryData.colleagueType, colleagueInventoryData.level, true, 1f);
			}
		}
		return num;
	}

	public long getColleagueSkinUnlockPrice()
	{
		return 300L;
	}

	public long getColleaguePremiumSkinUnlockPrice(ColleagueType colleagueType, int skinIndex)
	{
		return 1500L;
	}

	public bool containColleagueSkin(ColleagueType colleagueType, int skinIndex)
	{
		return Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[(int)colleagueType].colleagueSkinData[skinIndex];
	}

	public double getColleagueSkinStat(ColleagueType colleagueType, int skinIndex)
	{
		int num = skinIndex - 1;
		if (Singleton<ParsingManager>.instance.currentParsedStatData.colleagueSkinStatData.ContainsKey(colleagueType) && Singleton<ParsingManager>.instance.currentParsedStatData.colleagueSkinStatData[colleagueType].Count > num)
		{
			return Singleton<ParsingManager>.instance.currentParsedStatData.colleagueSkinStatData[colleagueType][num];
		}
		return 0.0;
	}

	public ColleagueType getPrevColleagueTypeForNormalColleague(ColleagueType axisColleague)
	{
		ColleagueType result = ColleagueType.None;
		for (int i = 0; i < normalColleagueTypeList.Count; i++)
		{
			if (normalColleagueTypeList[i] == axisColleague)
			{
				result = ((i <= 0) ? ColleagueType.None : normalColleagueTypeList[i - 1]);
			}
		}
		return result;
	}

	public float getColleagueDelay(ColleagueType colleagueType)
	{
		float result = 1f;
		switch (colleagueType)
		{
		case ColleagueType.Isabelle:
			result = 0.6f;
			break;
		case ColleagueType.Samantha:
			result = 0.3f;
			break;
		case ColleagueType.Lawrence:
			result = 1.1f;
			break;
		case ColleagueType.Stephanie:
			result = 1.5f;
			break;
		case ColleagueType.Puppy:
			result = 0.6f;
			break;
		case ColleagueType.Scarlett:
			result = 0.6f;
			break;
		case ColleagueType.Balbaria:
			result = 0.7f;
			break;
		case ColleagueType.Sera:
			result = 0.6f;
			break;
		case ColleagueType.Sushiro:
			result = 0.5f;
			break;
		case ColleagueType.Pinky:
			result = 0.6f;
			break;
		case ColleagueType.Thyrael:
			result = 0.7f;
			break;
		case ColleagueType.Dinnerless:
			result = 0.6f;
			break;
		case ColleagueType.FatherKing:
			result = 0.6f;
			break;
		case ColleagueType.GoldenFork:
			result = 0.7f;
			break;
		case ColleagueType.Kitty:
			result = 0.5f;
			break;
		case ColleagueType.Seaghoul:
			result = 0.8f;
			break;
		case ColleagueType.HoneyQueen:
			result = 0.3f;
			break;
		case ColleagueType.Barbie:
			result = 1.5f;
			break;
		case ColleagueType.Prince:
			result = 0.6f;
			break;
		case ColleagueType.BabyDragon:
			result = 0.25f;
			break;
		case ColleagueType.Trall:
			result = 0.5f;
			break;
		}
		return result;
	}

	public int getColleagueTier(ColleagueType colleagueType)
	{
		int result = 0;
		if (colleagueType <= ColleagueType.Golem)
		{
			result = (int)colleagueType;
		}
		else
		{
			switch (colleagueType)
			{
			case ColleagueType.Poty:
				result = 21;
				break;
			case ColleagueType.Trall:
				result = 22;
				break;
			case ColleagueType.InfernalDragon:
				result = 23;
				break;
			}
		}
		return result;
	}

	public long getPremiumColleagueUnlockPrice(ColleagueType colleagueType)
	{
		long result = 0L;
		switch (colleagueType)
		{
		case ColleagueType.Angelica:
			result = 5000L;
			break;
		case ColleagueType.Angelina:
			result = 1000L;
			break;
		case ColleagueType.Angela:
			result = 5000L;
			break;
		case ColleagueType.Goblin:
			result = 7000L;
			break;
		}
		return result;
	}

	public long getCurrentPremiumColleagueUpgradePrice(ColleagueType colleagueType)
	{
		long result = 0L;
		long level = getColleagueInventoryData(colleagueType).level;
		if (colleagueType == ColleagueType.Angela)
		{
			result = 10 + (level - 1) * 20;
		}
		return result;
	}

	public double getPremiumColleagueValue(ColleagueType colleagueType)
	{
		return getPremiumColleagueValue(colleagueType, getColleagueInventoryData(ColleagueType.Angela).level);
	}

	public double getPremiumColleagueValue(ColleagueType colleagueType, long level)
	{
		double result = 0.0;
		if (colleagueType == ColleagueType.Angela)
		{
			result = 100 + (level - 1) * 5;
		}
		return result;
	}

	public string getPremiumColleagueStatString(ColleagueType colleagueType)
	{
		string result = string.Empty;
		switch (colleagueType)
		{
		case ColleagueType.Angelica:
			result = I18NManager.Get("PREMIUM_COLLEAGUE_ANGELICA_STAT_DESCRIPTION");
			break;
		case ColleagueType.Angelina:
			result = string.Format(I18NManager.Get("PREMIUM_COLLEAGUE_ANGELINA_STAT_DESCRIPTION"), getTotalColleagueSkinCount() * 10);
			break;
		case ColleagueType.Angela:
			result = string.Format(I18NManager.Get("PREMIUM_COLLEAGUE_ANGELA_STAT_DESCRIPTION"), getPremiumColleagueValue(ColleagueType.Angela));
			break;
		case ColleagueType.Goblin:
			result = I18NManager.Get("PREMIUM_COLLEAGUE_GOBLIN_STAT_DESCRIPTION");
			break;
		}
		return result;
	}

	public string getPremiumColleagueDescription(ColleagueType colleagueType)
	{
		string result = string.Empty;
		switch (colleagueType)
		{
		case ColleagueType.Angelica:
			result = I18NManager.Get("PREMIUM_COLLEAGUE_ANGELICA_DESCRIPTION");
			break;
		case ColleagueType.Angelina:
			result = I18NManager.Get("PREMIUM_COLLEAGUE_ANGELINA_DESCRIPTION");
			break;
		case ColleagueType.Angela:
			result = I18NManager.Get("PREMIUM_COLLEAGUE_ANGELA_DESCRIPTION");
			break;
		case ColleagueType.Goblin:
			result = I18NManager.Get("PREMIUM_COLLEAGUE_GOBLIN_DESCRIPTION");
			break;
		}
		return result;
	}

	public bool isPremiumColleague(ColleagueType colleagueType)
	{
		bool result = false;
		if (colleagueType == ColleagueType.Angelica || colleagueType == ColleagueType.Angelina || colleagueType == ColleagueType.Angela || colleagueType == ColleagueType.Goblin)
		{
			result = true;
		}
		return result;
	}

	public int getTotalColleagueSkinCount()
	{
		int num = 0;
		for (int i = 0; i < 28; i++)
		{
			foreach (KeyValuePair<int, bool> colleagueSkinDatum in getColleagueInventoryData((ColleagueType)i).colleagueSkinData)
			{
				if (colleagueSkinDatum.Key > 1 && colleagueSkinDatum.Value)
				{
					num++;
				}
			}
		}
		return num;
	}

	public double getColleagueDamage(ColleagueType colleagueType, long level, bool isReturnWithCalculatedDamage = true, float delay = 1f, params double[] extraDamages)
	{
		double num = 0.0;
		if (TutorialManager.isTutorial)
		{
			level = 1L;
		}
		double num2 = 0.0;
		if (extraDamages != null)
		{
			for (int i = 0; i < extraDamages.Length; i++)
			{
				num2 += extraDamages[i];
			}
		}
		num = Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ColleagueDamageVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ColleagueDamageVariable2), getColleagueTier(colleagueType)) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ColleagueDamageVariable3) + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ColleagueDamageVariable4), level - 1) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ColleagueDamageAllMultyply) * (double)delay;
		if (isReturnWithCalculatedDamage)
		{
			num += num / 100.0 * (Singleton<StatManager>.instance.colleagueSelfPercentDamage[(int)colleagueType] + Singleton<StatManager>.instance.colleagueAllPercentDamageFromColleagueSkin + Singleton<StatManager>.instance.allPercentDamageFromColleague + Singleton<StatManager>.instance.percentDamageAM + Singleton<StatManager>.instance.percentDamagePM + Singleton<StatManager>.instance.allPercentDamageFromTreasure + Singleton<StatManager>.instance.extraPercentDamageFromNobleBlade + num2);
			if (Singleton<StatManager>.instance.transcendIncreaseAllDamage > 0.0)
			{
				num += num / 100.0 * Singleton<StatManager>.instance.transcendIncreaseAllDamage;
			}
			num += num / 100.0 * Singleton<StatManager>.instance.allCharacterAndColleaguePercentDamageFromPrincess;
			num += num / 100.0 * Singleton<StatManager>.instance.allColleaguePercentDamageFromAngelinaColleague;
			num += num / 100.0 * Singleton<StatManager>.instance.specialAdsAngelDamage;
			num += num / 100.0 * Singleton<StatManager>.instance.percentDamageFromPremiumTreasure;
			if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(ColleagueType.Angela).isUnlocked)
			{
				num += num / 100.0 * Singleton<ColleagueManager>.instance.getPremiumColleagueValue(ColleagueType.Angela);
			}
		}
		if (isPremiumColleague(colleagueType))
		{
			num = 0.0;
		}
		return num;
	}

	public string getColleagueI18NTitleID(ColleagueType colleagueType, int skinIndex)
	{
		return "COLLEAGUE_NAME_" + (int)(colleagueType + 1) + "_" + skinIndex;
	}

	public string getColleagueI18NName(ColleagueType colleagueType, int skinIndex)
	{
		return I18NManager.Get("COLLEAGUE_NAME_" + (int)(colleagueType + 1) + "_" + skinIndex);
	}

	public int getColleagueSkinMaxCount(ColleagueType colleagueType)
	{
		if (colleagueSkinMaxCountList.ContainsKey(colleagueType))
		{
			return colleagueSkinMaxCountList[colleagueType].Count;
		}
		return 0;
	}

	public double getColleagueUnlockPrice(ColleagueType colleagueType)
	{
		double num = 0.0;
		return Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ColleagueUnlockPriceVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ColleagueUnlockPriceVariable2), getColleagueTier(colleagueType));
	}

	public double getColleagueLevelUpPrice(ColleagueType colleagueType, long level)
	{
		double num = 0.0;
		double num2 = 1.0;
		long colleaguePassiveSkillUnlockLevel = getColleaguePassiveSkillUnlockLevel(PassiveSkillTierType.PassiveSkillTier1);
		long colleaguePassiveSkillUnlockLevel2 = getColleaguePassiveSkillUnlockLevel(PassiveSkillTierType.PassiveSkillTier2);
		long colleaguePassiveSkillUnlockLevel3 = getColleaguePassiveSkillUnlockLevel(PassiveSkillTierType.PassiveSkillTier3);
		long colleaguePassiveSkillUnlockLevel4 = getColleaguePassiveSkillUnlockLevel(PassiveSkillTierType.PassiveSkillTier4);
		long colleaguePassiveSkillUnlockLevel5 = getColleaguePassiveSkillUnlockLevel(PassiveSkillTierType.PassiveSkillTier5);
		long colleaguePassiveSkillUnlockLevel6 = getColleaguePassiveSkillUnlockLevel(PassiveSkillTierType.PassiveSkillTier6);
		if (level == colleaguePassiveSkillUnlockLevel6 - 1)
		{
			num2 = 3.0;
		}
		else if (level == colleaguePassiveSkillUnlockLevel5 - 1)
		{
			num2 = 2.8;
		}
		else if (level == colleaguePassiveSkillUnlockLevel4 - 1)
		{
			num2 = 2.6;
		}
		else if (level == colleaguePassiveSkillUnlockLevel3 - 1)
		{
			num2 = 2.4;
		}
		else if (level == colleaguePassiveSkillUnlockLevel2 - 1)
		{
			num2 = 2.2;
		}
		else if (level == colleaguePassiveSkillUnlockLevel - 1)
		{
			num2 = 2.0;
		}
		num = getColleagueUnlockPrice(colleagueType) / 10.0 * Math.Pow(1.060762, level) * num2;
		return num - num / 100.0 * Singleton<StatManager>.instance.colleagueLevelUpDiscountPercent;
	}

	public ColleagueInventoryData getColleagueInventoryData(ColleagueType colleagueType)
	{
		return Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[(int)colleagueType];
	}

	public long getColleaguePassiveSkillUnlockLevel(PassiveSkillTierType unlockTargetType)
	{
		return Singleton<ParsingManager>.instance.currentParsedStatData.colleaguePassiveUnlockLevelData[unlockTargetType];
	}

	public int getCanSpawnSlotTotalCount()
	{
		int num = 0;
		return Singleton<DataManager>.instance.currentGameData.colleagueInventoryList.Count;
	}

	public bool isHumanColleauge(ColleagueType colleagueType)
	{
		bool result = false;
		switch (colleagueType)
		{
		case ColleagueType.Isabelle:
		case ColleagueType.Lawrence:
		case ColleagueType.Balbaria:
		case ColleagueType.Sera:
		case ColleagueType.Thyrael:
		case ColleagueType.Dinnerless:
		case ColleagueType.FatherKing:
		case ColleagueType.GoldenFork:
		case ColleagueType.Seaghoul:
		case ColleagueType.Prince:
		case ColleagueType.Poty:
			result = true;
			break;
		}
		return result;
	}

	public ObscuredLong getColleagueSkinMaxLevel()
	{
		return 10L;
	}

	public bool unlockPassiveSkill(ColleagueType colleagueType)
	{
		ColleagueInventoryData colleagueInventoryData = getColleagueInventoryData(colleagueType);
		long nextSkillUnlockLevel = getNextSkillUnlockLevel(colleagueType);
		if (colleagueInventoryData.level >= nextSkillUnlockLevel)
		{
			colleagueInventoryData.lastPassiveUnlockLevel = nextSkillUnlockLevel;
			Singleton<DataManager>.instance.saveData();
			return true;
		}
		return false;
	}

	public bool isCanUnlockSkill(ColleagueType colleagueType)
	{
		bool result = false;
		ColleagueInventoryData colleagueInventoryData = getColleagueInventoryData(colleagueType);
		long nextSkillUnlockLevel = getNextSkillUnlockLevel(colleagueType);
		if (colleagueInventoryData.level >= nextSkillUnlockLevel && colleagueInventoryData.lastPassiveUnlockLevel < nextSkillUnlockLevel)
		{
			result = true;
		}
		return result;
	}

	public long getNextSkillUnlockLevel(ColleagueType colleagueType, bool isAxisLastUnlockLevel = true)
	{
		long result = 0L;
		Dictionary<PassiveSkillTierType, long> colleaguePassiveUnlockLevelData = Singleton<ParsingManager>.instance.currentParsedStatData.colleaguePassiveUnlockLevelData;
		ColleagueInventoryData colleagueInventoryData = getColleagueInventoryData(colleagueType);
		long level = colleagueInventoryData.level;
		long num = ((!isAxisLastUnlockLevel) ? level : colleagueInventoryData.lastPassiveUnlockLevel);
		if (level < colleaguePassiveUnlockLevelData[PassiveSkillTierType.PassiveSkillTier6] || num < colleaguePassiveUnlockLevelData[PassiveSkillTierType.PassiveSkillTier6])
		{
			for (int i = 0; i <= 5; i++)
			{
				if ((level >= colleaguePassiveUnlockLevelData[(PassiveSkillTierType)i] && num < colleaguePassiveUnlockLevelData[(PassiveSkillTierType)i]) || num < colleaguePassiveUnlockLevelData[(PassiveSkillTierType)i])
				{
					result = colleaguePassiveUnlockLevelData[(PassiveSkillTierType)i];
					break;
				}
				result = -90L;
			}
		}
		else
		{
			long num2;
			for (num2 = 0L; num >= num2; num2 += multipleForInfinityLevelup)
			{
			}
			result = num2;
		}
		return result;
	}

	public PassiveSkillTierType getPassiveTierByLevel(long level)
	{
		PassiveSkillTierType result = PassiveSkillTierType.None;
		if (level > 125)
		{
			result = ((level % getColleaguePassiveSkillUnlockLevel(PassiveSkillTierType.PassiveSkillTier5) != 0L) ? PassiveSkillTierType.PassiveSkillTier6 : PassiveSkillTierType.PassiveSkillTier5);
		}
		else
		{
			for (int num = 5; num >= 0; num--)
			{
				long colleaguePassiveSkillUnlockLevel = getColleaguePassiveSkillUnlockLevel((PassiveSkillTierType)num);
				if (level >= colleaguePassiveSkillUnlockLevel)
				{
					result = (PassiveSkillTierType)num;
					break;
				}
			}
		}
		return result;
	}

	public string getPassiveSkillDescription(ColleaguePassiveSkillData passiveSkillData, double value)
	{
		string result = string.Empty;
		switch (passiveSkillData.passiveType)
		{
		case ColleaguePassiveSkillType.Damage:
			switch (passiveSkillData.passiveTargetType)
			{
			case ColleaguePassiveTargetType.MySelf:
				result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_SELF_DAMAGE"), "<color=white>+" + value.ToString("f0") + "%</color>");
				break;
			case ColleaguePassiveTargetType.Warrior:
			case ColleaguePassiveTargetType.Priest:
			case ColleaguePassiveTargetType.Archer:
				result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_DAMAGE"), "<color=white>+" + value.ToString("f0") + "%</color>", I18NManager.Get(passiveSkillData.passiveTargetType.ToString().ToUpper()));
				break;
			case ColleaguePassiveTargetType.All:
				result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_ALL_DAMAGE"), "<color=white>+" + value.ToString("f0") + "%</color>");
				break;
			}
			break;
		case ColleaguePassiveSkillType.CriticalChance:
			result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_CRITICAL_CHANCE"), "<color=white>+" + value.ToString("f0") + "%</color>", I18NManager.Get(passiveSkillData.passiveTargetType.ToString().ToUpper()));
			break;
		case ColleaguePassiveSkillType.CriticalDamage:
			result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_CRITICAL_DAMAGE"), "<color=white>+" + value.ToString("f0") + "%</color>", I18NManager.Get(passiveSkillData.passiveTargetType.ToString().ToUpper()));
			break;
		case ColleaguePassiveSkillType.Health:
			result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_HEALTH"), "<color=white>+" + value.ToString("f0") + "%</color>");
			break;
		case ColleaguePassiveSkillType.TapHeal:
			result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_TAP_HEAL"), "<color=white>+" + value.ToString("f0") + "%</color>");
			break;
		}
		return result;
	}

	public string getPassiveSkillDescription(ColleaguePassiveSkillData passiveSkillData)
	{
		string result = string.Empty;
		switch (passiveSkillData.passiveType)
		{
		case ColleaguePassiveSkillType.Damage:
			switch (passiveSkillData.passiveTargetType)
			{
			case ColleaguePassiveTargetType.MySelf:
				result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_SELF_DAMAGE"), "<color=white>+" + passiveSkillData.passiveValue.ToString("f0") + "%</color>");
				break;
			case ColleaguePassiveTargetType.Warrior:
			case ColleaguePassiveTargetType.Priest:
			case ColleaguePassiveTargetType.Archer:
				result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_DAMAGE"), "<color=white>+" + passiveSkillData.passiveValue.ToString("f0") + "%</color>", I18NManager.Get(passiveSkillData.passiveTargetType.ToString().ToUpper()));
				break;
			case ColleaguePassiveTargetType.All:
				result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_ALL_DAMAGE"), "<color=white>+" + passiveSkillData.passiveValue.ToString("f0") + "%</color>");
				break;
			}
			break;
		case ColleaguePassiveSkillType.CriticalChance:
			result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_CRITICAL_CHANCE"), "<color=white>+" + passiveSkillData.passiveValue.ToString("f0") + "%</color>", I18NManager.Get(passiveSkillData.passiveTargetType.ToString().ToUpper()));
			break;
		case ColleaguePassiveSkillType.CriticalDamage:
			result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_CRITICAL_DAMAGE"), "<color=white>+" + passiveSkillData.passiveValue.ToString("f0") + "%</color>", I18NManager.Get(passiveSkillData.passiveTargetType.ToString().ToUpper()));
			break;
		case ColleaguePassiveSkillType.Health:
			result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_HEALTH"), "<color=white>+" + passiveSkillData.passiveValue.ToString("f0") + "%</color>");
			break;
		case ColleaguePassiveSkillType.TapHeal:
			result = string.Format(I18NManager.Get("COLLEAGUE_STAT_INFORMATION_TAP_HEAL"), "<color=white>+" + passiveSkillData.passiveValue.ToString("f0") + "%</color>");
			break;
		}
		return result;
	}

	public void refreshAllColleaguePassiveSkillStats()
	{
		Singleton<StatManager>.instance.colleagueSelfPercentDamage = new double[28];
		for (int i = 0; i < 28; i++)
		{
			ColleagueInventoryData colleagueInventoryData = Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[i];
			Dictionary<PassiveSkillTierType, ColleaguePassiveSkillData> colleaguePassiveSkillData = getColleaguePassiveSkillData(colleagueInventoryData.colleagueType);
			for (int j = 0; j < 6; j++)
			{
				if (colleagueInventoryData.lastPassiveUnlockLevel < getColleaguePassiveSkillUnlockLevel((PassiveSkillTierType)j))
				{
					continue;
				}
				ColleaguePassiveSkillData colleaguePassiveSkillData2 = colleaguePassiveSkillData[(PassiveSkillTierType)j];
				double passiveValue = colleaguePassiveSkillData2.passiveValue;
				switch (colleaguePassiveSkillData2.passiveType)
				{
				case ColleaguePassiveSkillType.Damage:
					switch (colleaguePassiveSkillData2.passiveTargetType)
					{
					case ColleaguePassiveTargetType.MySelf:
						Singleton<StatManager>.instance.colleagueSelfPercentDamage[(int)colleagueInventoryData.colleagueType] += passiveValue;
						break;
					case ColleaguePassiveTargetType.Warrior:
						Singleton<StatManager>.instance.warriorPercentDamageFromColleague += passiveValue;
						break;
					case ColleaguePassiveTargetType.Priest:
						Singleton<StatManager>.instance.priestPercentDamageFromColleague += passiveValue;
						break;
					case ColleaguePassiveTargetType.Archer:
						Singleton<StatManager>.instance.archerPercentDamageFromColleague += passiveValue;
						break;
					case ColleaguePassiveTargetType.All:
						Singleton<StatManager>.instance.allPercentDamageFromColleague += passiveValue;
						break;
					}
					break;
				case ColleaguePassiveSkillType.CriticalChance:
					switch (colleaguePassiveSkillData2.passiveTargetType)
					{
					case ColleaguePassiveTargetType.Warrior:
						Singleton<StatManager>.instance.warriorCriticalChanceFromColleague += (float)passiveValue;
						break;
					case ColleaguePassiveTargetType.Priest:
						Singleton<StatManager>.instance.priestCriticalChanceFromColleague += (float)passiveValue;
						break;
					case ColleaguePassiveTargetType.Archer:
						Singleton<StatManager>.instance.archerCriticalChanceFromColleague += (float)passiveValue;
						break;
					}
					break;
				case ColleaguePassiveSkillType.CriticalDamage:
					switch (colleaguePassiveSkillData2.passiveTargetType)
					{
					case ColleaguePassiveTargetType.Warrior:
						Singleton<StatManager>.instance.warriorCriticalDamageFromColleague += passiveValue;
						break;
					case ColleaguePassiveTargetType.Priest:
						Singleton<StatManager>.instance.priestCriticalDamageFromColleague += passiveValue;
						break;
					case ColleaguePassiveTargetType.Archer:
						Singleton<StatManager>.instance.archerCriticalDamageFromColleague += passiveValue;
						break;
					}
					break;
				case ColleaguePassiveSkillType.Health:
					Singleton<StatManager>.instance.warriorPercentHealthFromColleague += passiveValue;
					break;
				case ColleaguePassiveSkillType.TapHeal:
					Singleton<StatManager>.instance.priestPercentTapHealFromColleague += passiveValue;
					break;
				}
			}
		}
	}

	public Sprite getColleaguePassiveSkillIconSprite(ColleaguePassiveSkillType skillType, ColleaguePassiveTargetType targetType)
	{
		return m_currentPassiveSkillIconDictionary[skillType][targetType];
	}

	private void registryColleaguePassiveSkillIcon()
	{
		m_currentPassiveSkillIconDictionary = new Dictionary<ColleaguePassiveSkillType, Dictionary<ColleaguePassiveTargetType, Sprite>>();
		for (int i = 0; i < colleaguePassiveSkillIconSprites.Length; i++)
		{
			string text = colleaguePassiveSkillIconSprites[i].name.Replace("ui_icon_fellow_skill_", string.Empty);
			string[] array = text.Split('_');
			ColleaguePassiveTargetType key = ColleaguePassiveTargetType.None;
			switch (array[0])
			{
			case "hp":
			{
				Dictionary<ColleaguePassiveTargetType, Sprite> dictionary3 = new Dictionary<ColleaguePassiveTargetType, Sprite>();
				dictionary3.Add(ColleaguePassiveTargetType.Warrior, colleaguePassiveSkillIconSprites[i]);
				m_currentPassiveSkillIconDictionary.Add(ColleaguePassiveSkillType.Health, dictionary3);
				break;
			}
			case "heal":
			{
				Dictionary<ColleaguePassiveTargetType, Sprite> dictionary6 = new Dictionary<ColleaguePassiveTargetType, Sprite>();
				dictionary6.Add(ColleaguePassiveTargetType.Priest, colleaguePassiveSkillIconSprites[i]);
				m_currentPassiveSkillIconDictionary.Add(ColleaguePassiveSkillType.TapHeal, dictionary6);
				break;
			}
			case "dmg":
				if (array.Length > 1)
				{
					switch (array[1])
					{
					case "warrior":
						key = ColleaguePassiveTargetType.Warrior;
						break;
					case "priest":
						key = ColleaguePassiveTargetType.Priest;
						break;
					case "archer":
						key = ColleaguePassiveTargetType.Archer;
						break;
					case "all":
						key = ColleaguePassiveTargetType.All;
						break;
					}
					if (!m_currentPassiveSkillIconDictionary.ContainsKey(ColleaguePassiveSkillType.Damage))
					{
						m_currentPassiveSkillIconDictionary.Add(ColleaguePassiveSkillType.Damage, new Dictionary<ColleaguePassiveTargetType, Sprite>());
					}
					Dictionary<ColleaguePassiveTargetType, Sprite> dictionary4 = m_currentPassiveSkillIconDictionary[ColleaguePassiveSkillType.Damage];
					dictionary4.Add(key, colleaguePassiveSkillIconSprites[i]);
				}
				else
				{
					if (!m_currentPassiveSkillIconDictionary.ContainsKey(ColleaguePassiveSkillType.Damage))
					{
						m_currentPassiveSkillIconDictionary.Add(ColleaguePassiveSkillType.Damage, new Dictionary<ColleaguePassiveTargetType, Sprite>());
					}
					Dictionary<ColleaguePassiveTargetType, Sprite> dictionary5 = m_currentPassiveSkillIconDictionary[ColleaguePassiveSkillType.Damage];
					dictionary5.Add(ColleaguePassiveTargetType.MySelf, colleaguePassiveSkillIconSprites[i]);
				}
				break;
			case "critical":
				if (array[1].Contains("damage"))
				{
					switch (array[2])
					{
					case "warrior":
						key = ColleaguePassiveTargetType.Warrior;
						break;
					case "priest":
						key = ColleaguePassiveTargetType.Priest;
						break;
					case "archer":
						key = ColleaguePassiveTargetType.Archer;
						break;
					}
					if (!m_currentPassiveSkillIconDictionary.ContainsKey(ColleaguePassiveSkillType.CriticalDamage))
					{
						m_currentPassiveSkillIconDictionary.Add(ColleaguePassiveSkillType.CriticalDamage, new Dictionary<ColleaguePassiveTargetType, Sprite>());
					}
					Dictionary<ColleaguePassiveTargetType, Sprite> dictionary = m_currentPassiveSkillIconDictionary[ColleaguePassiveSkillType.CriticalDamage];
					dictionary.Add(key, colleaguePassiveSkillIconSprites[i]);
				}
				else
				{
					switch (array[1])
					{
					case "warrior":
						key = ColleaguePassiveTargetType.Warrior;
						break;
					case "priest":
						key = ColleaguePassiveTargetType.Priest;
						break;
					case "archer":
						key = ColleaguePassiveTargetType.Archer;
						break;
					}
					if (!m_currentPassiveSkillIconDictionary.ContainsKey(ColleaguePassiveSkillType.CriticalChance))
					{
						m_currentPassiveSkillIconDictionary.Add(ColleaguePassiveSkillType.CriticalChance, new Dictionary<ColleaguePassiveTargetType, Sprite>());
					}
					Dictionary<ColleaguePassiveTargetType, Sprite> dictionary2 = m_currentPassiveSkillIconDictionary[ColleaguePassiveSkillType.CriticalChance];
					dictionary2.Add(key, colleaguePassiveSkillIconSprites[i]);
				}
				break;
			}
		}
	}

	public Dictionary<PassiveSkillTierType, ColleaguePassiveSkillData> getColleaguePassiveSkillData(ColleagueType colleagueType)
	{
		Dictionary<PassiveSkillTierType, ColleaguePassiveSkillData> dictionary = new Dictionary<PassiveSkillTierType, ColleaguePassiveSkillData>();
		Dictionary<PassiveSkillTierType, long> colleaguePassiveUnlockLevelData = Singleton<ParsingManager>.instance.currentParsedStatData.colleaguePassiveUnlockLevelData;
		ColleagueInventoryData colleagueInventoryData = getColleagueInventoryData(colleagueType);
		foreach (KeyValuePair<PassiveSkillTierType, ColleaguePassiveSkillData> item in Singleton<ParsingManager>.instance.currentParsedStatData.colleagueEffectData[colleagueType])
		{
			ColleaguePassiveSkillData colleaguePassiveSkillData = new ColleaguePassiveSkillData();
			colleaguePassiveSkillData.passiveTargetType = item.Value.passiveTargetType;
			colleaguePassiveSkillData.passiveType = item.Value.passiveType;
			colleaguePassiveSkillData.passiveValue = item.Value.passiveValue;
			long num = colleagueInventoryData.lastPassiveUnlockLevel / colleaguePassiveUnlockLevelData[PassiveSkillTierType.PassiveSkillTier5] - 1;
			double passiveValue = Singleton<ParsingManager>.instance.currentParsedStatData.colleagueEffectData[colleagueType][PassiveSkillTierType.PassiveSkillTier5].passiveValue;
			switch (item.Key)
			{
			case PassiveSkillTierType.PassiveSkillTier5:
				if (colleagueInventoryData.lastPassiveUnlockLevel >= colleaguePassiveUnlockLevelData[PassiveSkillTierType.PassiveSkillTier5] * 2)
				{
					colleaguePassiveSkillData.passiveValue += item.Value.passiveValue * (double)num;
				}
				break;
			case PassiveSkillTierType.PassiveSkillTier6:
				if (colleagueInventoryData.lastPassiveUnlockLevel >= colleaguePassiveUnlockLevelData[PassiveSkillTierType.PassiveSkillTier6] + multipleForInfinityLevelup)
				{
					colleaguePassiveSkillData.passiveValue += (colleagueInventoryData.lastPassiveUnlockLevel - colleaguePassiveUnlockLevelData[PassiveSkillTierType.PassiveSkillTier6]) / multipleForInfinityLevelup * 100;
					colleaguePassiveSkillData.passiveValue -= passiveValue * (double)num;
				}
				break;
			}
			dictionary.Add(item.Key, colleaguePassiveSkillData);
		}
		foreach (KeyValuePair<PassiveSkillTierType, ColleaguePassiveSkillData> item2 in dictionary)
		{
			item2.Value.passiveValue += item2.Value.passiveValue / 100.0 * Singleton<StatManager>.instance.colleaguePassiveSkillExtraValue;
		}
		return dictionary;
	}
}
