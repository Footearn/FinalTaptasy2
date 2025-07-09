using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkinManager : Singleton<CharacterSkinManager>
{
	public enum ArcherSkinType
	{
		Windstoker,
		Lambo,
		Artemis,
		Siren,
		ElvenKing,
		Bowee,
		Robin,
		Marauder,
		Dragona,
		Patricia,
		Claude,
		Kyle,
		Valkyrie3,
		Alice,
		Cherry,
		ZhangFei,
		Eligos,
		Rudolph,
		Apollo,
		PoliceChief,
		MasterWindstoker,
		Cupid,
		Rania,
		Spiculus,
		SurferWindstalker,
		MiniDragon,
		Owltears,
		SantaHelper,
		Anemone,
		FlowerGirl,
		Length
	}

	public enum WarriorSkinType
	{
		William,
		Samback,
		Desmond,
		Broomy,
		Uriel,
		Drake,
		Odin,
		Siegfried,
		Dragoon,
		Arthur,
		BlackKnight,
		Phoenix,
		Valkyrie1,
		Pirate,
		Blueberry,
		GuanYu,
		Marbas,
		SnowMan,
		Zeus,
		HwangJin,
		MasterWilliam,
		Lovely,
		Charlotte,
		Achillia,
		BaywatchWilliam,
		MiniDemon,
		Vampire,
		GingerMan,
		Cosmos,
		Groom,
		Length
	}

	public enum PriestSkinType
	{
		Olivia,
		Mona,
		Reinhard,
		Mika,
		Wandy,
		Dorothy,
		Undine,
		Candy,
		Dragoness,
		Michael,
		Criselda,
		Elisha,
		Valkyrie2,
		Witch,
		Orange,
		LiuBei,
		Amy,
		SnowRabbit,
		Aphrodite,
		JangGeum,
		MasterOlivia,
		Sweetie,
		Gabriel,
		Spartacus,
		PicnicOlivia,
		MiniMonkfish,
		Succubus,
		Penguin,
		Daisy,
		Bride,
		Length
	}

	public struct CharacterSkinTypeData
	{
		public WarriorSkinType warriorSkinType;

		public PriestSkinType priestSkinType;

		public ArcherSkinType archerSkinType;

		public CharacterSkinTypeData(WarriorSkinType warriorSkinType, PriestSkinType priestSkinType, ArcherSkinType archerSkinType)
		{
			this.warriorSkinType = warriorSkinType;
			this.priestSkinType = priestSkinType;
			this.archerSkinType = archerSkinType;
		}
	}

	public enum SkinStateType
	{
		Equipped,
		NotEquipped,
		NoHave
	}

	[Serializable]
	public class CharacterSkillData
	{
		public float percentDamage;

		public float secondStat;

		public long unlockRubyPrice;

		public CharacterSkillData(float percentDamage, float secondStat, long unlockRubyPrice)
		{
			this.percentDamage = percentDamage;
			this.secondStat = secondStat;
			this.unlockRubyPrice = unlockRubyPrice;
		}
	}

	public Sprite[] warriorSkinSprites;

	public Sprite[] priestSkinSprites;

	public Sprite[] archerSkinSprites;

	public List<CharacterSkillData> warriorCharacterSkillData;

	public List<CharacterSkillData> priestCharacterSkillData;

	public List<CharacterSkillData> archerCharacterSkillData;

	private Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>> m_limitedSkinDictionary = new Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>>();

	private Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>> m_eventSkinDictionary = new Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>>();

	private Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>> m_normalSkinDictionary = new Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>>();

	private Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>> m_transcendSkinDictionary = new Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>>();

	public Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>> sortedSkinList = new Dictionary<CharacterManager.CharacterType, List<CharacterSkinTypeData>>();

	[HideInInspector]
	public WarriorCharacterSkinData warriorEquippedSkinData;

	[HideInInspector]
	public PriestCharacterSkinData priestEquippedSkinData;

	[HideInInspector]
	public ArcherCharacterSkinData archerEquippedSkinData;

	private void Awake()
	{
		warriorCharacterSkillData = new List<CharacterSkillData>();
		priestCharacterSkillData = new List<CharacterSkillData>();
		archerCharacterSkillData = new List<CharacterSkillData>();
		foreach (KeyValuePair<WarriorSkinType, CharacterSkinStatData> warriorCharacterSkinDatum in Singleton<ParsingManager>.instance.currentParsedStatData.warriorCharacterSkinData)
		{
			warriorCharacterSkillData.Add(new CharacterSkillData(warriorCharacterSkinDatum.Value.percentDamage, warriorCharacterSkinDatum.Value.secondStat, warriorCharacterSkinDatum.Value.unlockRubyPrice));
		}
		foreach (KeyValuePair<PriestSkinType, CharacterSkinStatData> priestCharacterSkinDatum in Singleton<ParsingManager>.instance.currentParsedStatData.priestCharacterSkinData)
		{
			priestCharacterSkillData.Add(new CharacterSkillData(priestCharacterSkinDatum.Value.percentDamage, priestCharacterSkinDatum.Value.secondStat, priestCharacterSkinDatum.Value.unlockRubyPrice));
		}
		foreach (KeyValuePair<ArcherSkinType, CharacterSkinStatData> archerCharacterSkinDatum in Singleton<ParsingManager>.instance.currentParsedStatData.archerCharacterSkinData)
		{
			archerCharacterSkillData.Add(new CharacterSkillData(archerCharacterSkinDatum.Value.percentDamage, archerCharacterSkinDatum.Value.secondStat, archerCharacterSkinDatum.Value.unlockRubyPrice));
		}
	}

	public void obtainCharacterSkin(WarriorSkinType skinType)
	{
		if (!Singleton<DataManager>.instance.currentGameData.warriorSkinData[(int)skinType].isHaving)
		{
			Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.HeroCollector, 1.0);
			Singleton<DataManager>.instance.currentGameData.warriorSkinData[(int)skinType].isHaving = true;
			Singleton<DataManager>.instance.currentGameData.warriorSkinData[(int)skinType]._skinLevel = 1L;
		}
		else
		{
			++Singleton<DataManager>.instance.currentGameData.warriorSkinData[(int)skinType]._skinLevel;
			long num = Base36.Decode(Singleton<DataManager>.instance.currentGameData.characterSkinTotalLevelUpRecord);
			num++;
			Singleton<DataManager>.instance.currentGameData.characterSkinTotalLevelUpRecord = Base36.Encode(num);
		}
	}

	public void obtainCharacterSkin(PriestSkinType skinType)
	{
		if (!Singleton<DataManager>.instance.currentGameData.priestSkinData[(int)skinType].isHaving)
		{
			Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.HeroCollector, 1.0);
			Singleton<DataManager>.instance.currentGameData.priestSkinData[(int)skinType].isHaving = true;
			Singleton<DataManager>.instance.currentGameData.priestSkinData[(int)skinType]._skinLevel = 1L;
		}
		else
		{
			++Singleton<DataManager>.instance.currentGameData.priestSkinData[(int)skinType]._skinLevel;
			long num = Base36.Decode(Singleton<DataManager>.instance.currentGameData.characterSkinTotalLevelUpRecord);
			num++;
			Singleton<DataManager>.instance.currentGameData.characterSkinTotalLevelUpRecord = Base36.Encode(num);
		}
	}

	public void obtainCharacterSkin(ArcherSkinType skinType)
	{
		if (!Singleton<DataManager>.instance.currentGameData.archerSkinData[(int)skinType].isHaving)
		{
			Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.HeroCollector, 1.0);
			Singleton<DataManager>.instance.currentGameData.archerSkinData[(int)skinType].isHaving = true;
			Singleton<DataManager>.instance.currentGameData.archerSkinData[(int)skinType]._skinLevel = 1L;
		}
		else
		{
			++Singleton<DataManager>.instance.currentGameData.archerSkinData[(int)skinType]._skinLevel;
			long num = Base36.Decode(Singleton<DataManager>.instance.currentGameData.characterSkinTotalLevelUpRecord);
			num++;
			Singleton<DataManager>.instance.currentGameData.characterSkinTotalLevelUpRecord = Base36.Encode(num);
		}
	}

	public void unlockAllCharacters()
	{
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.warriorSkinData.Count; i++)
		{
			WarriorSkinType skinType = Singleton<DataManager>.instance.currentGameData.warriorSkinData[i].skinType;
			if (skinType != 0 && !isLimitedCharacterSkin(skinType) && !isTranscendPremiumSkin(skinType) && !isEventCharacterSkin(skinType))
			{
				obtainCharacterSkin(skinType);
			}
		}
		for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.priestSkinData.Count; j++)
		{
			PriestSkinType skinType2 = Singleton<DataManager>.instance.currentGameData.priestSkinData[j].skinType;
			if (skinType2 != 0 && !isLimitedCharacterSkin(skinType2) && !isTranscendPremiumSkin(skinType2) && !isEventCharacterSkin(skinType2))
			{
				obtainCharacterSkin(Singleton<DataManager>.instance.currentGameData.priestSkinData[j].skinType);
			}
		}
		for (int k = 0; k < Singleton<DataManager>.instance.currentGameData.archerSkinData.Count; k++)
		{
			ArcherSkinType skinType3 = Singleton<DataManager>.instance.currentGameData.archerSkinData[k].skinType;
			if (skinType3 != 0 && !isLimitedCharacterSkin(skinType3) && !isTranscendPremiumSkin(skinType3) && !isEventCharacterSkin(skinType3))
			{
				obtainCharacterSkin(Singleton<DataManager>.instance.currentGameData.archerSkinData[k].skinType);
			}
		}
		UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
	}

	public void refreshCharacterTypeList()
	{
		sortedSkinList.Clear();
		m_limitedSkinDictionary.Clear();
		m_normalSkinDictionary.Clear();
		m_transcendSkinDictionary.Clear();
		m_eventSkinDictionary.Clear();
		for (int i = 0; i < 3; i++)
		{
			CharacterManager.CharacterType characterType = (CharacterManager.CharacterType)i;
			switch (characterType)
			{
			case CharacterManager.CharacterType.Warrior:
			{
				CharacterSkinTypeData item3 = default(CharacterSkinTypeData);
				for (int k = 0; k < 30; k++)
				{
					WarriorSkinType warriorSkinType = (WarriorSkinType)k;
					bool flag5 = Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(warriorSkinType);
					bool flag6 = Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(warriorSkinType);
					bool flag7 = Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(warriorSkinType);
					bool flag8 = !flag5 && !flag7;
					if (flag5)
					{
						if (!m_limitedSkinDictionary.ContainsKey(characterType))
						{
							m_limitedSkinDictionary.Add(characterType, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list5 = m_limitedSkinDictionary[CharacterManager.CharacterType.Warrior];
						CharacterSkinTypeData characterSkinTypeData2 = new CharacterSkinTypeData(warriorSkinType, PriestSkinType.Length, ArcherSkinType.Length);
						list5.Add(characterSkinTypeData2);
						if (warriorSkinType == WarriorSkinType.MasterWilliam)
						{
							item3 = characterSkinTypeData2;
						}
					}
					else if (flag6)
					{
						if (!m_eventSkinDictionary.ContainsKey(characterType))
						{
							m_eventSkinDictionary.Add(characterType, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list6 = m_eventSkinDictionary[CharacterManager.CharacterType.Warrior];
						CharacterSkinTypeData item4 = new CharacterSkinTypeData(warriorSkinType, PriestSkinType.Length, ArcherSkinType.Length);
						list6.Add(item4);
					}
					else if (flag8)
					{
						if (!m_normalSkinDictionary.ContainsKey(characterType))
						{
							m_normalSkinDictionary.Add(characterType, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list7 = m_normalSkinDictionary[CharacterManager.CharacterType.Warrior];
						list7.Add(new CharacterSkinTypeData(warriorSkinType, PriestSkinType.Length, ArcherSkinType.Length));
					}
					else if (flag7)
					{
						if (!m_transcendSkinDictionary.ContainsKey(characterType))
						{
							m_transcendSkinDictionary.Add(characterType, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list8 = m_transcendSkinDictionary[CharacterManager.CharacterType.Warrior];
						list8.Add(new CharacterSkinTypeData(warriorSkinType, PriestSkinType.Length, ArcherSkinType.Length));
					}
				}
				if (m_limitedSkinDictionary[characterType].Contains(item3))
				{
					m_limitedSkinDictionary[characterType].Remove(item3);
				}
				m_normalSkinDictionary[characterType].Insert(0, item3);
				break;
			}
			case CharacterManager.CharacterType.Priest:
			{
				CharacterSkinTypeData item5 = default(CharacterSkinTypeData);
				for (int l = 0; l < 30; l++)
				{
					PriestSkinType priestSkinType = (PriestSkinType)l;
					bool flag9 = Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(priestSkinType);
					bool flag10 = Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(priestSkinType);
					bool flag11 = Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(priestSkinType);
					bool flag12 = !flag9 && !flag11;
					if (flag9)
					{
						if (!m_limitedSkinDictionary.ContainsKey(CharacterManager.CharacterType.Priest))
						{
							m_limitedSkinDictionary.Add(CharacterManager.CharacterType.Priest, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list9 = m_limitedSkinDictionary[CharacterManager.CharacterType.Priest];
						CharacterSkinTypeData characterSkinTypeData3 = new CharacterSkinTypeData(WarriorSkinType.Length, priestSkinType, ArcherSkinType.Length);
						list9.Add(characterSkinTypeData3);
						if (priestSkinType == PriestSkinType.MasterOlivia)
						{
							item5 = characterSkinTypeData3;
						}
					}
					else if (flag10)
					{
						if (!m_eventSkinDictionary.ContainsKey(characterType))
						{
							m_eventSkinDictionary.Add(characterType, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list10 = m_eventSkinDictionary[CharacterManager.CharacterType.Priest];
						CharacterSkinTypeData item6 = new CharacterSkinTypeData(WarriorSkinType.Length, priestSkinType, ArcherSkinType.Length);
						list10.Add(item6);
					}
					else if (flag12)
					{
						if (!m_normalSkinDictionary.ContainsKey(CharacterManager.CharacterType.Priest))
						{
							m_normalSkinDictionary.Add(CharacterManager.CharacterType.Priest, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list11 = m_normalSkinDictionary[CharacterManager.CharacterType.Priest];
						list11.Add(new CharacterSkinTypeData(WarriorSkinType.Length, priestSkinType, ArcherSkinType.Length));
					}
					else if (flag11)
					{
						if (!m_transcendSkinDictionary.ContainsKey(characterType))
						{
							m_transcendSkinDictionary.Add(characterType, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list12 = m_transcendSkinDictionary[CharacterManager.CharacterType.Priest];
						list12.Add(new CharacterSkinTypeData(WarriorSkinType.Length, priestSkinType, ArcherSkinType.Length));
					}
				}
				if (m_limitedSkinDictionary[characterType].Contains(item5))
				{
					m_limitedSkinDictionary[characterType].Remove(item5);
				}
				m_normalSkinDictionary[characterType].Insert(0, item5);
				break;
			}
			case CharacterManager.CharacterType.Archer:
			{
				CharacterSkinTypeData item = default(CharacterSkinTypeData);
				for (int j = 0; j < 30; j++)
				{
					ArcherSkinType archerSkinType = (ArcherSkinType)j;
					bool flag = Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(archerSkinType);
					bool flag2 = Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(archerSkinType);
					bool flag3 = Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(archerSkinType);
					bool flag4 = !flag && !flag3;
					if (flag)
					{
						if (!m_limitedSkinDictionary.ContainsKey(CharacterManager.CharacterType.Archer))
						{
							m_limitedSkinDictionary.Add(CharacterManager.CharacterType.Archer, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list = m_limitedSkinDictionary[CharacterManager.CharacterType.Archer];
						CharacterSkinTypeData characterSkinTypeData = new CharacterSkinTypeData(WarriorSkinType.Length, PriestSkinType.Length, archerSkinType);
						list.Add(characterSkinTypeData);
						if (archerSkinType == ArcherSkinType.MasterWindstoker)
						{
							item = characterSkinTypeData;
						}
					}
					else if (flag2)
					{
						if (!m_eventSkinDictionary.ContainsKey(characterType))
						{
							m_eventSkinDictionary.Add(characterType, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list2 = m_eventSkinDictionary[CharacterManager.CharacterType.Archer];
						CharacterSkinTypeData item2 = new CharacterSkinTypeData(WarriorSkinType.Length, PriestSkinType.Length, archerSkinType);
						list2.Add(item2);
					}
					else if (flag4)
					{
						if (!m_normalSkinDictionary.ContainsKey(CharacterManager.CharacterType.Archer))
						{
							m_normalSkinDictionary.Add(CharacterManager.CharacterType.Archer, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list3 = m_normalSkinDictionary[CharacterManager.CharacterType.Archer];
						list3.Add(new CharacterSkinTypeData(WarriorSkinType.Length, PriestSkinType.Length, archerSkinType));
					}
					else if (flag3)
					{
						if (!m_transcendSkinDictionary.ContainsKey(characterType))
						{
							m_transcendSkinDictionary.Add(characterType, new List<CharacterSkinTypeData>());
						}
						List<CharacterSkinTypeData> list4 = m_transcendSkinDictionary[CharacterManager.CharacterType.Archer];
						list4.Add(new CharacterSkinTypeData(WarriorSkinType.Length, PriestSkinType.Length, archerSkinType));
					}
				}
				if (m_limitedSkinDictionary[characterType].Contains(item))
				{
					m_limitedSkinDictionary[characterType].Remove(item);
				}
				m_normalSkinDictionary[characterType].Insert(0, item);
				break;
			}
			}
			if (!sortedSkinList.ContainsKey(characterType))
			{
				sortedSkinList.Add(characterType, new List<CharacterSkinTypeData>());
			}
			if (m_normalSkinDictionary.ContainsKey(characterType))
			{
				for (int m = 0; m < m_normalSkinDictionary[characterType].Count; m++)
				{
					sortedSkinList[characterType].Add(m_normalSkinDictionary[characterType][m]);
				}
			}
			if (m_transcendSkinDictionary.ContainsKey(characterType))
			{
				for (int n = 0; n < m_transcendSkinDictionary[characterType].Count; n++)
				{
					sortedSkinList[characterType].Add(m_transcendSkinDictionary[characterType][n]);
				}
			}
			if (m_limitedSkinDictionary.ContainsKey(characterType))
			{
				for (int num = 0; num < m_limitedSkinDictionary[characterType].Count; num++)
				{
					sortedSkinList[characterType].Add(m_limitedSkinDictionary[characterType][num]);
				}
			}
			if (m_eventSkinDictionary.ContainsKey(characterType))
			{
				for (int num2 = 0; num2 < m_eventSkinDictionary[characterType].Count; num2++)
				{
					sortedSkinList[characterType].Add(m_eventSkinDictionary[characterType][num2]);
				}
			}
		}
	}

	public void checkRemoveFromPackageForDaemonKingPackage()
	{
		int num = 0;
		if (Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(WarriorSkinType.Marbas).isHaving)
		{
			num++;
		}
		if (Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(PriestSkinType.Amy).isHaving)
		{
			num++;
		}
		if (Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(ArcherSkinType.Eligos).isHaving)
		{
			num++;
		}
		if (num >= 2 && Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.DemonKingSkinPackage.ToString()))
		{
			Singleton<ShopManager>.instance.consumeLimitedItem(Singleton<ShopManager>.instance.getLimitedItemDataFromCurrentInventory(ShopManager.LimitedShopItemType.DemonKingSkinPackage.ToString()));
		}
	}

	public bool isNormalCharacterSkin(WarriorSkinType skinType)
	{
		return !isLimitedCharacterSkin(skinType) && !isEventCharacterSkin(skinType) && !isTranscendPremiumSkin(skinType);
	}

	public bool isNormalCharacterSkin(PriestSkinType skinType)
	{
		return !isLimitedCharacterSkin(skinType) && !isEventCharacterSkin(skinType) && !isTranscendPremiumSkin(skinType);
	}

	public bool isNormalCharacterSkin(ArcherSkinType skinType)
	{
		return !isLimitedCharacterSkin(skinType) && !isEventCharacterSkin(skinType) && !isTranscendPremiumSkin(skinType);
	}

	public bool isLimitedCharacterSkin(WarriorSkinType skinType)
	{
		bool result = false;
		if (skinType >= WarriorSkinType.Siegfried && skinType < WarriorSkinType.Length && !isTranscendPremiumSkin(skinType))
		{
			result = true;
		}
		if (isEventCharacterSkin(skinType))
		{
			result = false;
		}
		return result;
	}

	public bool isLimitedCharacterSkin(PriestSkinType skinType)
	{
		bool result = false;
		if (skinType >= PriestSkinType.Candy && skinType < PriestSkinType.Length && !isTranscendPremiumSkin(skinType))
		{
			result = true;
		}
		if (isEventCharacterSkin(skinType))
		{
			result = false;
		}
		return result;
	}

	public bool isLimitedCharacterSkin(ArcherSkinType skinType)
	{
		bool result = false;
		if (skinType >= ArcherSkinType.Marauder && skinType < ArcherSkinType.Length && !isTranscendPremiumSkin(skinType))
		{
			result = true;
		}
		if (isEventCharacterSkin(skinType))
		{
			result = false;
		}
		return result;
	}

	public bool isEventCharacterSkin(WarriorSkinType skinType)
	{
		bool result = false;
		switch (skinType)
		{
		case WarriorSkinType.Pirate:
		case WarriorSkinType.Blueberry:
		case WarriorSkinType.SnowMan:
		case WarriorSkinType.HwangJin:
		case WarriorSkinType.Lovely:
			result = true;
			break;
		}
		return result;
	}

	public bool isEventCharacterSkin(PriestSkinType skinType)
	{
		bool result = false;
		switch (skinType)
		{
		case PriestSkinType.Witch:
		case PriestSkinType.Orange:
		case PriestSkinType.SnowRabbit:
		case PriestSkinType.JangGeum:
		case PriestSkinType.Sweetie:
			result = true;
			break;
		}
		return result;
	}

	public bool isEventCharacterSkin(ArcherSkinType skinType)
	{
		bool result = false;
		switch (skinType)
		{
		case ArcherSkinType.Alice:
		case ArcherSkinType.Cherry:
		case ArcherSkinType.Rudolph:
		case ArcherSkinType.PoliceChief:
		case ArcherSkinType.Cupid:
			result = true;
			break;
		}
		return result;
	}

	public bool isTranscendPremiumSkin(WarriorSkinType skinType)
	{
		bool result = false;
		if (skinType == WarriorSkinType.Arthur || skinType == WarriorSkinType.BlackKnight || skinType == WarriorSkinType.Phoenix)
		{
			result = true;
		}
		return result;
	}

	public bool isTranscendPremiumSkin(PriestSkinType skinType)
	{
		bool result = false;
		if (skinType == PriestSkinType.Michael || skinType == PriestSkinType.Criselda || skinType == PriestSkinType.Elisha)
		{
			result = true;
		}
		return result;
	}

	public bool isTranscendPremiumSkin(ArcherSkinType skinType)
	{
		bool result = false;
		if (skinType == ArcherSkinType.Patricia || skinType == ArcherSkinType.Claude || skinType == ArcherSkinType.Kyle)
		{
			result = true;
		}
		return result;
	}

	public bool isAllCharacterUnlocked()
	{
		bool result = true;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.warriorSkinData.Count; i++)
		{
			if (!Singleton<DataManager>.instance.currentGameData.warriorSkinData[i].isHaving)
			{
				result = false;
				break;
			}
		}
		for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.priestSkinData.Count; j++)
		{
			if (!Singleton<DataManager>.instance.currentGameData.priestSkinData[j].isHaving)
			{
				result = false;
				break;
			}
		}
		for (int k = 0; k < Singleton<DataManager>.instance.currentGameData.archerSkinData.Count; k++)
		{
			if (!Singleton<DataManager>.instance.currentGameData.archerSkinData[k].isHaving)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public string getCharacterSkinName(WarriorSkinType skinType)
	{
		return I18NManager.Get("WARRIOR_SKIN_NAME_" + (int)(skinType + 1));
	}

	public string getCharacterSkinName(PriestSkinType skinType)
	{
		return I18NManager.Get("PRIEST_SKIN_NAME_" + (int)(skinType + 1));
	}

	public string getCharacterSkinName(ArcherSkinType skinType)
	{
		return I18NManager.Get("ARCHER_SKIN_NAME_" + (int)(skinType + 1));
	}

	public long getRubyBuyPrice(WarriorSkinType skinType)
	{
		long num = 0L;
		return warriorCharacterSkillData[(int)skinType].unlockRubyPrice;
	}

	public long getRubyBuyPrice(PriestSkinType skinType)
	{
		long num = 0L;
		return priestCharacterSkillData[(int)skinType].unlockRubyPrice;
	}

	public long getRubyBuyPrice(ArcherSkinType skinType)
	{
		long num = 0L;
		return archerCharacterSkillData[(int)skinType].unlockRubyPrice;
	}

	public float getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType characterType)
	{
		float num = 0f;
		switch (characterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.warriorSkinData.Count; j++)
			{
				if (Singleton<DataManager>.instance.currentGameData.warriorSkinData[j].isHaving)
				{
					num += warriorCharacterSkillData[j].percentDamage * (float)(long)Singleton<DataManager>.instance.currentGameData.warriorSkinData[j]._skinLevel;
				}
			}
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			for (int k = 0; k < Singleton<DataManager>.instance.currentGameData.priestSkinData.Count; k++)
			{
				if (Singleton<DataManager>.instance.currentGameData.priestSkinData[k].isHaving)
				{
					num += priestCharacterSkillData[k].percentDamage * (float)(long)Singleton<DataManager>.instance.currentGameData.priestSkinData[k]._skinLevel;
				}
			}
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.archerSkinData.Count; i++)
			{
				if (Singleton<DataManager>.instance.currentGameData.archerSkinData[i].isHaving)
				{
					num += archerCharacterSkillData[i].percentDamage * (float)(long)Singleton<DataManager>.instance.currentGameData.archerSkinData[i]._skinLevel;
				}
			}
			break;
		}
		}
		return num + num / 100f * (float)Singleton<StatManager>.instance.characterSkinEffectBonus;
	}

	public float getTotalCharacterSkinSecondStatValue(CharacterManager.CharacterType characterType)
	{
		float num = 0f;
		switch (characterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			for (int k = 0; k < Singleton<DataManager>.instance.currentGameData.warriorSkinData.Count; k++)
			{
				if (Singleton<DataManager>.instance.currentGameData.warriorSkinData[k].isHaving)
				{
					num += warriorCharacterSkillData[k].secondStat * (float)(long)Singleton<DataManager>.instance.currentGameData.warriorSkinData[k]._skinLevel;
				}
			}
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			for (int j = 0; j < Singleton<DataManager>.instance.currentGameData.priestSkinData.Count; j++)
			{
				if (Singleton<DataManager>.instance.currentGameData.priestSkinData[j].isHaving)
				{
					num += priestCharacterSkillData[j].secondStat * (float)(long)Singleton<DataManager>.instance.currentGameData.priestSkinData[j]._skinLevel;
				}
			}
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.archerSkinData.Count; i++)
			{
				if (Singleton<DataManager>.instance.currentGameData.archerSkinData[i].isHaving)
				{
					num += archerCharacterSkillData[i].secondStat * (float)(long)Singleton<DataManager>.instance.currentGameData.archerSkinData[i]._skinLevel;
				}
			}
			num = Mathf.Min(num, 100f);
			break;
		}
		}
		return num + num / 100f * (float)Singleton<StatManager>.instance.characterSkinEffectBonus;
	}

	public WarriorCharacterSkinData getSkinDataFromInventory(WarriorSkinType skinType)
	{
		return Singleton<DataManager>.instance.currentGameData.warriorSkinData[(int)skinType];
	}

	public PriestCharacterSkinData getSkinDataFromInventory(PriestSkinType skinType)
	{
		return Singleton<DataManager>.instance.currentGameData.priestSkinData[(int)skinType];
	}

	public ArcherCharacterSkinData getSkinDataFromInventory(ArcherSkinType skinType)
	{
		return Singleton<DataManager>.instance.currentGameData.archerSkinData[(int)skinType];
	}

	public long getSkinMaxLevel()
	{
		return long.MaxValue;
	}
}
