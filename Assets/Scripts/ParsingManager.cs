using System;
using System.Collections.Generic;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Newtonsoft.Json;

public class ParsingManager : Singleton<ParsingManager>
{
	public ParsedStatData currentParsedStatData;

	public GameObject introObjects;

	public GameObject ingameObjects;

	public static bool isLoaded;

	public static float jackpotMultiplyValue = 1f;

	public static float adsAngelFastTouchDuringTime = 4f;

	public static float adsAngelAutoOpenTreasureChestDuringTime = 4f;

	public static float adsAngelAppearMinTime = 4f;

	public static float adsAngelAppearMaxTime = 6f;

	private void Awake()
	{
		parseDataPack();
	}

	private void parseDataPack()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("DataPack.json");
		
		string jsonContent = textAsset.text;
		currentParsedStatData = JsonConvert.DeserializeObject<ParsedStatData>(jsonContent);
		isLoaded = true;
	}

	public Dictionary<CharacterManager.CharacterType, CharacterStatData> getCharacterStatData(string baseString)
	{
		Dictionary<CharacterManager.CharacterType, CharacterStatData> dictionary = new Dictionary<CharacterManager.CharacterType, CharacterStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			for (int j = 0; j < 3; j++)
			{
				if (array2[0] == ((CharacterManager.CharacterType)j).ToString())
				{
					CharacterStatData value = default(CharacterStatData);
					value.currentCharacterType = (CharacterManager.CharacterType)j;
					value.baseDelay = float.Parse(array2[1]);
					value.baseCriticalChance = float.Parse(array2[2]);
					value.baseCriticalDamage = float.Parse(array2[3]);
					value.baseSpeed = float.Parse(array2[4]);
					value.baseRange = float.Parse(array2[5]);
					dictionary.Add((CharacterManager.CharacterType)j, value);
				}
			}
		}
		return dictionary;
	}

	public Dictionary<CharacterSkinManager.WarriorSkinType, CharacterSkinStatData> getWarriorCharacterSkinStatData(string baseString)
	{
		Dictionary<CharacterSkinManager.WarriorSkinType, CharacterSkinStatData> dictionary = new Dictionary<CharacterSkinManager.WarriorSkinType, CharacterSkinStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (array2[0] == CharacterManager.CharacterType.Warrior.ToString())
			{
				CharacterSkinManager.WarriorSkinType key = (CharacterSkinManager.WarriorSkinType)(int)Enum.Parse(typeof(CharacterSkinManager.WarriorSkinType), array2[1]);
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, default(CharacterSkinStatData));
				}
				CharacterSkinStatData value = default(CharacterSkinStatData);
				value.currentCharacterType = CharacterManager.CharacterType.Warrior;
				value.percentDamage = float.Parse(array2[2]);
				value.secondStat = float.Parse(array2[3]);
				value.unlockRubyPrice = long.Parse(array2[4]);
				dictionary[key] = value;
			}
		}
		return dictionary;
	}

	public Dictionary<CharacterSkinManager.PriestSkinType, CharacterSkinStatData> getPriestCharacterSkinStatData(string baseString)
	{
		Dictionary<CharacterSkinManager.PriestSkinType, CharacterSkinStatData> dictionary = new Dictionary<CharacterSkinManager.PriestSkinType, CharacterSkinStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (array2[0] == CharacterManager.CharacterType.Priest.ToString())
			{
				CharacterSkinManager.PriestSkinType key = (CharacterSkinManager.PriestSkinType)(int)Enum.Parse(typeof(CharacterSkinManager.PriestSkinType), array2[1]);
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, default(CharacterSkinStatData));
				}
				CharacterSkinStatData value = default(CharacterSkinStatData);
				value.currentCharacterType = CharacterManager.CharacterType.Priest;
				value.percentDamage = float.Parse(array2[2]);
				value.secondStat = float.Parse(array2[3]);
				value.unlockRubyPrice = long.Parse(array2[4]);
				dictionary[key] = value;
			}
		}
		return dictionary;
	}

	public Dictionary<CharacterSkinManager.ArcherSkinType, CharacterSkinStatData> getArcherCharacterSkinStatData(string baseString)
	{
		Dictionary<CharacterSkinManager.ArcherSkinType, CharacterSkinStatData> dictionary = new Dictionary<CharacterSkinManager.ArcherSkinType, CharacterSkinStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (array2[0] == CharacterManager.CharacterType.Archer.ToString())
			{
				CharacterSkinManager.ArcherSkinType key = (CharacterSkinManager.ArcherSkinType)(int)Enum.Parse(typeof(CharacterSkinManager.ArcherSkinType), array2[1]);
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, default(CharacterSkinStatData));
				}
				CharacterSkinStatData value = default(CharacterSkinStatData);
				value.currentCharacterType = CharacterManager.CharacterType.Archer;
				value.percentDamage = float.Parse(array2[2]);
				value.secondStat = float.Parse(array2[3]);
				value.unlockRubyPrice = long.Parse(array2[4]);
				dictionary[key] = value;
			}
		}
		return dictionary;
	}

	public Dictionary<EnemyManager.MonsterType, MonsterStatData> getMonsterStatData(string baseString)
	{
		Dictionary<EnemyManager.MonsterType, MonsterStatData> dictionary = new Dictionary<EnemyManager.MonsterType, MonsterStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			for (int j = 0; j < 180; j++)
			{
				if (array2[0] == ((EnemyManager.MonsterType)j).ToString())
				{
					MonsterStatData value = default(MonsterStatData);
					value.baseDelay = float.Parse(array2[1]);
					value.baseSpeed = float.Parse(array2[2]);
					value.baseRange = float.Parse(array2[3]);
					dictionary.Add((EnemyManager.MonsterType)j, value);
				}
			}
		}
		return dictionary;
	}

	public Dictionary<EnemyManager.BossType, BossStatData> getBossStatData(string baseString)
	{
		Dictionary<EnemyManager.BossType, BossStatData> dictionary = new Dictionary<EnemyManager.BossType, BossStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			for (int j = 0; j < 10; j++)
			{
				if (array2[0] == ((EnemyManager.BossType)j).ToString())
				{
					BossStatData value = default(BossStatData);
					value.baseDelay = float.Parse(array2[1]);
					value.baseSpeed = float.Parse(array2[2]);
					value.baseRange = float.Parse(array2[3]);
					dictionary.Add((EnemyManager.BossType)j, value);
				}
			}
		}
		return dictionary;
	}

	public Dictionary<EnemyManager.SpecialType, SpecialMonsterStatData> getSpecialMonsterStatData(string baseString)
	{
		Dictionary<EnemyManager.SpecialType, SpecialMonsterStatData> dictionary = new Dictionary<EnemyManager.SpecialType, SpecialMonsterStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			for (int j = 0; j < 4; j++)
			{
				if (array2[0] == ((EnemyManager.SpecialType)j).ToString())
				{
					SpecialMonsterStatData value = default(SpecialMonsterStatData);
					value.baseDelay = float.Parse(array2[1]);
					value.baseSpeed = float.Parse(array2[2]);
					value.baseRange = float.Parse(array2[3]);
					dictionary.Add((EnemyManager.SpecialType)j, value);
				}
			}
		}
		return dictionary;
	}

	public Dictionary<TreasureManager.TreasureType, TreasureStatData> getTreasureStat(string baseString)
	{
		Dictionary<TreasureManager.TreasureType, TreasureStatData> dictionary = new Dictionary<TreasureManager.TreasureType, TreasureStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			for (int j = 1; j < 53; j++)
			{
				if (array2[0] == ((TreasureManager.TreasureType)j).ToString())
				{
					TreasureStatData value = default(TreasureStatData);
					value.currentTreasureType = (TreasureManager.TreasureType)j;
					value.damagePercentValue = float.Parse(array2[1]);
					value.treasureEffectValue = double.Parse(array2[2]);
					value.increasingValueEveryEnchant = double.Parse(array2[3]);
					value.maxLevel = long.Parse(array2[4]);
					value.increasingEnchantStonePriceEveryEnchant = double.Parse(array2[5]);
					value.baseEnchantStonePrice = double.Parse(array2[6]);
					value.increasingRubyPriceEveryEnchant = double.Parse(array2[7]);
					value.baseEnchantRubyPrice = double.Parse(array2[8]);
					value.treasureTier = int.Parse(array2[9]);
					dictionary.Add((TreasureManager.TreasureType)j, value);
				}
			}
		}
		return dictionary;
	}

	public Dictionary<SkillManager.SkillType, ActiveSkillStatData> getActiveSkillStat(string baseString)
	{
		Dictionary<SkillManager.SkillType, ActiveSkillStatData> dictionary = new Dictionary<SkillManager.SkillType, ActiveSkillStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			ActiveSkillStatData value = default(ActiveSkillStatData);
			for (int j = 0; j < 5; j++)
			{
				if (array2[0] == ((SkillManager.SkillType)j).ToString())
				{
					value.currentSkillType = (SkillManager.SkillType)j;
					value.basePercentValue = float.Parse(array2[1]);
					value.increasePercentValue = float.Parse(array2[2]);
					value.manaCost = int.Parse(array2[3]);
					dictionary.Add((SkillManager.SkillType)j, value);
				}
			}
		}
		return dictionary;
	}

	public AdsAngelStatData getAdsAngelStat(string baseString)
	{
		AdsAngelStatData result = default(AdsAngelStatData);
		string[] array = baseString.Split('\n');
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			result.appearMinTime = float.Parse(array2[1]);
			result.appearMaxTime = float.Parse(array2[2]);
			result.fastTouchDuringTime = float.Parse(array2[3]);
			result.damageDoubleDuringTime = float.Parse(array2[4]);
			result.goldPileAppearChance = float.Parse(array2[5]);
			result.fastTouchAppearChance = float.Parse(array2[6]);
			result.doubleDamageAppearChance = float.Parse(array2[7]);
		}
		return result;
	}

	public Dictionary<AchievementManager.AchievementType, AchievementStatData> getAchievementStatData(string baseString)
	{
		Dictionary<AchievementManager.AchievementType, AchievementStatData> dictionary = new Dictionary<AchievementManager.AchievementType, AchievementStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			for (int j = 0; j < 18; j++)
			{
				if (array2[0] == ((AchievementManager.AchievementType)j).ToString())
				{
					AchievementStatData value = default(AchievementStatData);
					value.currentAchievementType = (AchievementManager.AchievementType)j;
					value.achievementMaxLevel = int.Parse(array2[1]);
					value.achievementLevelOneGoalValue = double.Parse(array2[2]);
					value.achievementLevelTwoGoalValue = double.Parse(array2[3]);
					value.achievementLevelThreeGoalValue = double.Parse(array2[4]);
					value.rewardType = int.Parse(array2[5]);
					value.rewardValueLevelOne = long.Parse(array2[6]);
					value.rewardValueLevelTwo = long.Parse(array2[7]);
					value.rewardValueLevelThree = long.Parse(array2[8]);
					dictionary.Add((AchievementManager.AchievementType)j, value);
				}
			}
		}
		return dictionary;
	}

	public Dictionary<int, double> getJackpotChanceData(string baseString)
	{
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		string[] array = baseString.Split('\n');
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (!array2[0].Equals("SUM"))
			{
				dictionary.Add(int.Parse(array2[0]), double.Parse(array2[1]));
			}
		}
		return dictionary;
	}

	public Dictionary<ColleagueManager.ColleagueType, Dictionary<ColleagueManager.PassiveSkillTierType, ColleaguePassiveSkillData>> getColleagueEffect(string baseString)
	{
		Dictionary<ColleagueManager.ColleagueType, Dictionary<ColleagueManager.PassiveSkillTierType, ColleaguePassiveSkillData>> dictionary = new Dictionary<ColleagueManager.ColleagueType, Dictionary<ColleagueManager.PassiveSkillTierType, ColleaguePassiveSkillData>>();
		string[] array = baseString.Split('\n');
		for (int i = 3; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			ColleagueManager.ColleagueType key = (ColleagueManager.ColleagueType)(int)Enum.Parse(typeof(ColleagueManager.ColleagueType), array2[0]);
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, new Dictionary<ColleagueManager.PassiveSkillTierType, ColleaguePassiveSkillData>());
			}
			Dictionary<ColleagueManager.PassiveSkillTierType, ColleaguePassiveSkillData> dictionary2 = dictionary[key];
			for (int j = 0; j < 6; j++)
			{
				ColleagueManager.PassiveSkillTierType key2 = (ColleagueManager.PassiveSkillTierType)j;
				if (!dictionary2.ContainsKey(key2))
				{
					dictionary2.Add(key2, new ColleaguePassiveSkillData());
				}
				ColleaguePassiveSkillData colleaguePassiveSkillData = dictionary2[key2];
				int num = (j + 1) * 2 - 1;
				string[] array3 = array2[num].Split('_');
				colleaguePassiveSkillData.passiveType = (ColleagueManager.ColleaguePassiveSkillType)(int)Enum.Parse(typeof(ColleagueManager.ColleaguePassiveSkillType), array3[0]);
				colleaguePassiveSkillData.passiveTargetType = (ColleagueManager.ColleaguePassiveTargetType)(int)Enum.Parse(typeof(ColleagueManager.ColleaguePassiveTargetType), array3[1]);
				colleaguePassiveSkillData.passiveValue = long.Parse(array2[num + 1]);
			}
		}
		return dictionary;
	}

	public Dictionary<ColleagueManager.PassiveSkillTierType, long> getColleagueEffectUnlockLevelData(string baseString)
	{
		Dictionary<ColleagueManager.PassiveSkillTierType, long> dictionary = new Dictionary<ColleagueManager.PassiveSkillTierType, long>();
		string[] array = baseString.Split('\n')[1].Split('\t');
		for (int i = 0; i < 6; i++)
		{
			ColleagueManager.PassiveSkillTierType key = (ColleagueManager.PassiveSkillTierType)i;
			long num = 0L;
			int num2 = (i + 1) * 2 - 1;
			num = long.Parse(array[num2]);
			dictionary.Add(key, num);
		}
		return dictionary;
	}

	public Dictionary<ColleagueManager.ColleagueType, List<double>> getColleagueSkinStatData(string baseString)
	{
		Dictionary<ColleagueManager.ColleagueType, List<double>> dictionary = new Dictionary<ColleagueManager.ColleagueType, List<double>>();
		string[] array = baseString.Split('\n');
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			dictionary.Add((ColleagueManager.ColleagueType)(i - 1), new List<double>());
			List<double> list = dictionary[(ColleagueManager.ColleagueType)(i - 1)];
			for (int j = 1; j < array2.Length; j++)
			{
				list.Add(double.Parse(array2[j]));
			}
		}
		return dictionary;
	}

	public Dictionary<WeaponSkinManager.WeaponSkinAbilityType, WeaponSkinAbilityStatData> getWeaponSkinAbilityStatData(string baseString)
	{
		Dictionary<WeaponSkinManager.WeaponSkinAbilityType, WeaponSkinAbilityStatData> dictionary = new Dictionary<WeaponSkinManager.WeaponSkinAbilityType, WeaponSkinAbilityStatData>();
		string[] array = baseString.Split('\n');
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			WeaponSkinManager.WeaponSkinAbilityType weaponSkinAbilityType = (WeaponSkinManager.WeaponSkinAbilityType)(i - 1);
			if (!dictionary.ContainsKey(weaponSkinAbilityType))
			{
				dictionary.Add(weaponSkinAbilityType, new WeaponSkinAbilityStatData());
			}
			WeaponSkinAbilityStatData weaponSkinAbilityStatData = dictionary[weaponSkinAbilityType];
			WeaponSkinManager.WeaponSkinGradeType abilityAppearGradeType = WeaponSkinManager.WeaponSkinGradeType.None;
			switch (array2[1])
			{
			case "R":
				abilityAppearGradeType = WeaponSkinManager.WeaponSkinGradeType.Rare;
				break;
			case "E":
				abilityAppearGradeType = WeaponSkinManager.WeaponSkinGradeType.Epic;
				break;
			case "U":
				abilityAppearGradeType = WeaponSkinManager.WeaponSkinGradeType.Unique;
				break;
			case "L":
				abilityAppearGradeType = WeaponSkinManager.WeaponSkinGradeType.Legendary;
				break;
			}
			weaponSkinAbilityStatData.currentAbilityType = weaponSkinAbilityType;
			weaponSkinAbilityStatData.abilityAppearGradeType = abilityAppearGradeType;
			weaponSkinAbilityStatData.statDictionary = new Dictionary<WeaponSkinManager.WeaponSkinGradeType, ObscuredDouble>();
			for (int j = 2; j < array2.Length; j++)
			{
				weaponSkinAbilityStatData.statDictionary.Add((WeaponSkinManager.WeaponSkinGradeType)(j - 2), double.Parse(array2[j]));
			}
		}
		return dictionary;
	}
}
