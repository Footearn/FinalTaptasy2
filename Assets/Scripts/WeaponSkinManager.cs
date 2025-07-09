using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MathNet.Numerics.Random;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSkinManager : Singleton<WeaponSkinManager>
{
	public enum WarriorSpecialWeaponSkinType
	{
		None = -1,
		Rose,
		Chicken,
		Length
	}

	public enum PriestSpecialWeaponSkinType
	{
		None = -1,
		SunFlower,
		Egg,
		Length
	}

	public enum ArcherSpecialWeaponSkinType
	{
		None = -1,
		FlowerBow,
		Sushi,
		Length
	}

	public enum WeaponSkinGradeType
	{
		None = -1,
		Rare,
		Epic,
		Unique,
		Legendary,
		Length
	}

	public enum WeaponSkinAbilityType
	{
		None = -1,
		PercentDamage,
		PercentHealth,
		CriticalDamage,
		MoveSpeed,
		SkillDamage,
		TapHeal,
		Armor,
		Evade,
		CriticalChance,
		GoldGain,
		TowerModeEvade,
		CollectEventDropChance,
		CriticalExtraDamage,
		NewAttackExtraDamage,
		StackDamage,
		InvinciblePerson,
		ReinforcementMana,
		AutoCastConcecntration,
		AutoCastCloneWarrior,
		AutoCastDragonBreath,
		LegendaryChestDropChance,
		Length
	}

	public static ObscuredLong weaponSkinLotteryWeaponSkinPiecePrice = 50L;

	public static ObscuredLong weaponSkinCubePrice = 150L;

	public static ObscuredLong weaponSkinChangePrice = 500L;

	public Text[] weaponSkinPieceTexts;

	public Text[] weaponSkinReinforcementMasterPieceTexts;

	public Sprite normalLotteryAnvilSprite;

	public Sprite premiumLotteryAnvilSprite;

	public Sprite newSkinTitleSprite;

	public Sprite successTitleSprite;

	public Sprite greatTitleSprite;

	public Sprite[] warriorSpecialWeaponSkinSprite;

	public Sprite[] priestSpecialWeaponSkinSprite;

	public Sprite[] archerSpecialWeaponSkinSprite;

	private MersenneTwister m_randomForCharacterType = new MersenneTwister();

	private MersenneTwister m_randomForWeaponType = new MersenneTwister();

	private MersenneTwister m_randomForRandomAbilityType = new MersenneTwister();

	private MersenneTwister m_randomForRandomAbilityValue = new MersenneTwister();

	private MersenneTwister m_randomForRandomGrade = new MersenneTwister();

	private readonly int[] m_weaponSkinAbilityCount = new int[4]
	{
		1,
		2,
		4,
		5
	};

	private Dictionary<WeaponSkinGradeType, double> m_gradeAppearChanceDictionaryForFirstLottery = new Dictionary<WeaponSkinGradeType, double>
	{
		{
			WeaponSkinGradeType.Rare,
			82.5
		},
		{
			WeaponSkinGradeType.Epic,
			15.0
		},
		{
			WeaponSkinGradeType.Unique,
			1.5
		},
		{
			WeaponSkinGradeType.Legendary,
			0.5
		}
	};

	public Sprite[] gradeIconSprites;

	public Sprite[] gradeBackgroundSprites;

	public WeaponSkinData startLottery(bool isPremiumLottery, bool isOnlyLegendary = false)
	{
		WeaponSkinData randomWeaponSkinData = getRandomWeaponSkinData(isPremiumLottery, false, null, isOnlyLegendary);
		if (!Singleton<DataManager>.instance.currentGameData.weaponSkinData.ContainsKey(randomWeaponSkinData.currentCharacterType))
		{
			Singleton<DataManager>.instance.currentGameData.weaponSkinData.Add(randomWeaponSkinData.currentCharacterType, new List<WeaponSkinData>());
		}
		obtainWeaponSkin(randomWeaponSkinData);
		UIWindowWeaponSkinLottery.instance.openLotteryUI(randomWeaponSkinData, isPremiumLottery);
		return randomWeaponSkinData;
	}

	public void obtainWeaponSkin(WeaponSkinData skinData)
	{
		Singleton<DataManager>.instance.currentGameData.weaponSkinData[skinData.currentCharacterType].Add(skinData);
		Singleton<DataManager>.instance.saveData();
		UIWindowWeaponSkin.instance.newSkinData = skinData;
	}

	public void equipWeaponSkin(WeaponSkinData skinData)
	{
		if (getEquippedWeaponSkinData(skinData.currentCharacterType) != null)
		{
			getEquippedWeaponSkinData(skinData.currentCharacterType).isEquipped = false;
		}
		skinData.isEquipped = true;
		for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
		{
			if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
			{
				Singleton<CharacterManager>.instance.constCharacterList[i].equippedWeapon.refreshWeaponSkin();
			}
		}
		Singleton<DataManager>.instance.saveData();
		Singleton<StatManager>.instance.refreshAllStats();
		if (UIWindowWeaponSkin.instance.isOpen)
		{
			UIWindowWeaponSkin.instance.openWeaponSkin(UIWindowWeaponSkin.instance.currentTabCharacterType);
		}
		if (UIWindowManageHeroAndWeapon.instance.isOpen)
		{
			UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
		}
	}

	public void unequipWeaponSkin(WeaponSkinData skinData)
	{
		if (getEquippedWeaponSkinData(skinData.currentCharacterType) != null)
		{
			getEquippedWeaponSkinData(skinData.currentCharacterType).isEquipped = false;
		}
		for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
		{
			if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
			{
				Singleton<CharacterManager>.instance.constCharacterList[i].equippedWeapon.refreshWeaponSkin();
			}
		}
		Singleton<DataManager>.instance.saveData();
		Singleton<StatManager>.instance.refreshAllStats();
		if (UIWindowWeaponSkin.instance.isOpen)
		{
			UIWindowWeaponSkin.instance.openWeaponSkin(UIWindowWeaponSkin.instance.currentTabCharacterType);
		}
		if (UIWindowManageHeroAndWeapon.instance.isOpen)
		{
			UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
		}
	}

	public void spawnWeaponSkinPiece(Vector2 spawnPosition)
	{
		Singleton<DropItemManager>.instance.spawnDropItem(DropItemManager.DropItemType.WeaponSkinPiece, spawnPosition, 1.0);
	}

	public void displayWeaponSkinPiece()
	{
		for (int i = 0; i < weaponSkinPieceTexts.Length; i++)
		{
			weaponSkinPieceTexts[i].text = Singleton<DataManager>.instance.currentGameData.weaponSkinPiece.ToString("N0");
		}
	}

	public void increaseWeaponSkinPiece(long value)
	{
		GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
		currentGameData.weaponSkinPiece = (long)currentGameData.weaponSkinPiece + value;
		Singleton<DataManager>.instance.saveData();
		displayWeaponSkinPiece();
	}

	public void decreaseWeaponSkinPiece(long value)
	{
		GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
		currentGameData.weaponSkinPiece = (long)currentGameData.weaponSkinPiece - value;
		Singleton<DataManager>.instance.saveData();
		displayWeaponSkinPiece();
	}

	public void displayWeaponReinforcementMasterPiece()
	{
		for (int i = 0; i < weaponSkinReinforcementMasterPieceTexts.Length; i++)
		{
			weaponSkinReinforcementMasterPieceTexts[i].text = Singleton<DataManager>.instance.currentGameData.weaponSkinReinforcementMasterPiece.ToString("N0");
		}
	}

	public void increaseWeaponSkinReinforcementMasterPiece(long value)
	{
		GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
		currentGameData.weaponSkinReinforcementMasterPiece = (long)currentGameData.weaponSkinReinforcementMasterPiece + value;
		Singleton<DataManager>.instance.saveData();
		displayWeaponReinforcementMasterPiece();
	}

	public void decreaseWeaponSkinReinforcementMasterPiece(long value)
	{
		GameData currentGameData = Singleton<DataManager>.instance.currentGameData;
		currentGameData.weaponSkinReinforcementMasterPiece = (long)currentGameData.weaponSkinReinforcementMasterPiece - value;
		Singleton<DataManager>.instance.saveData();
		displayWeaponReinforcementMasterPiece();
	}

	public void calculateAllWeaponSkinStat()
	{
		for (int i = 0; i < 3; i++)
		{
			CharacterManager.CharacterType characterType = (CharacterManager.CharacterType)i;
			WeaponSkinData equippedWeaponSkinData = getEquippedWeaponSkinData(characterType);
			if (equippedWeaponSkinData == null)
			{
				continue;
			}
			for (int j = 0; j < equippedWeaponSkinData.abilityDataList.Count; j++)
			{
				double num = Singleton<ParsingManager>.instance.currentParsedStatData.weaponSkinAbilityStatData[equippedWeaponSkinData.abilityDataList[j].currentAbilityType].statDictionary[equippedWeaponSkinData.abilityDataList[j].valueGradeType];
				switch (equippedWeaponSkinData.abilityDataList[j].currentAbilityType)
				{
				case WeaponSkinAbilityType.PercentDamage:
					switch (equippedWeaponSkinData.currentCharacterType)
					{
					case CharacterManager.CharacterType.Warrior:
						Singleton<StatManager>.instance.weaponSkinPercentDamageForWarrior += num;
						break;
					case CharacterManager.CharacterType.Priest:
						Singleton<StatManager>.instance.weaponSkinPercentDamageForPriest += num;
						break;
					case CharacterManager.CharacterType.Archer:
						Singleton<StatManager>.instance.weaponSkinPercentDamageForArcher += num;
						break;
					}
					break;
				case WeaponSkinAbilityType.PercentHealth:
					Singleton<StatManager>.instance.weaponSkinTotalPercentHealth += num;
					break;
				case WeaponSkinAbilityType.CriticalDamage:
					switch (equippedWeaponSkinData.currentCharacterType)
					{
					case CharacterManager.CharacterType.Warrior:
						Singleton<StatManager>.instance.weaponSkinCriticalDamageForWarrior += num;
						break;
					case CharacterManager.CharacterType.Priest:
						Singleton<StatManager>.instance.weaponSkinCriticalDamageForPriest += num;
						break;
					case CharacterManager.CharacterType.Archer:
						Singleton<StatManager>.instance.weaponSkinCriticalDamageForArcher += num;
						break;
					}
					break;
				case WeaponSkinAbilityType.MoveSpeed:
					Singleton<StatManager>.instance.weaponSkinMovementSpeed += num;
					Singleton<StatManager>.instance.percentMovementSpeed += (float)num;
					break;
				case WeaponSkinAbilityType.SkillDamage:
					Singleton<StatManager>.instance.weaponSkinSkillDamage += num;
					Singleton<StatManager>.instance.skillExtraPercentDamage += num;
					break;
				case WeaponSkinAbilityType.TapHeal:
					Singleton<StatManager>.instance.weaponSkinTapHeal += num;
					break;
				case WeaponSkinAbilityType.Armor:
					Singleton<StatManager>.instance.weaponSkinArmor += num;
					break;
				case WeaponSkinAbilityType.Evade:
					Singleton<StatManager>.instance.weaponSkinEvadeChance += num;
					break;
				case WeaponSkinAbilityType.CriticalChance:
					switch (equippedWeaponSkinData.currentCharacterType)
					{
					case CharacterManager.CharacterType.Warrior:
						Singleton<StatManager>.instance.weaponSkinCriticalChanceForWarrior += num;
						break;
					case CharacterManager.CharacterType.Priest:
						Singleton<StatManager>.instance.weaponSkinCriticalChanceForPriest += num;
						break;
					case CharacterManager.CharacterType.Archer:
						Singleton<StatManager>.instance.weaponSkinCriticalChanceForArcher += num;
						break;
					}
					break;
				case WeaponSkinAbilityType.GoldGain:
					Singleton<StatManager>.instance.weaponSkinGoldGain += num;
					break;
				case WeaponSkinAbilityType.TowerModeEvade:
				{
					StatManager instance = Singleton<StatManager>.instance;
					instance.weaponSkinTowerModeEvadeChance = (double)instance.weaponSkinTowerModeEvadeChance + num;
					break;
				}
				case WeaponSkinAbilityType.CollectEventDropChance:
					Singleton<StatManager>.instance.weaponSkinCollectEventItemDropChance += num;
					break;
				case WeaponSkinAbilityType.CriticalExtraDamage:
					switch (equippedWeaponSkinData.currentCharacterType)
					{
					case CharacterManager.CharacterType.Warrior:
						Singleton<StatManager>.instance.weaponSkinExtraCriticalDamageForWarrior += num;
						break;
					case CharacterManager.CharacterType.Priest:
						Singleton<StatManager>.instance.weaponSkinExtraCriticalDamageForPriest += num;
						break;
					case CharacterManager.CharacterType.Archer:
						Singleton<StatManager>.instance.weaponSkinExtraCriticalDamageForArcher += num;
						break;
					}
					break;
				case WeaponSkinAbilityType.NewAttackExtraDamage:
					switch (equippedWeaponSkinData.currentCharacterType)
					{
					case CharacterManager.CharacterType.Warrior:
						Singleton<StatManager>.instance.weaponSkinNewAttackDamageForWarrior += num;
						break;
					case CharacterManager.CharacterType.Priest:
						Singleton<StatManager>.instance.weaponSkinNewAttackDamageForPriest += num;
						break;
					case CharacterManager.CharacterType.Archer:
						Singleton<StatManager>.instance.weaponSkinNewAttackDamageForArcher += num;
						break;
					}
					break;
				case WeaponSkinAbilityType.StackDamage:
					switch (equippedWeaponSkinData.currentCharacterType)
					{
					case CharacterManager.CharacterType.Warrior:
						Singleton<StatManager>.instance.weaponSkinStackAttackDamageForWarrior += num;
						break;
					case CharacterManager.CharacterType.Priest:
						Singleton<StatManager>.instance.weaponSkinStackAttackDamageForPriest += num;
						break;
					case CharacterManager.CharacterType.Archer:
						Singleton<StatManager>.instance.weaponSkinStackAttackDamageForArcher += num;
						break;
					}
					break;
				case WeaponSkinAbilityType.InvinciblePerson:
					Singleton<StatManager>.instance.weaponSkinInvinciblePerson += num;
					break;
				case WeaponSkinAbilityType.ReinforcementMana:
					Singleton<StatManager>.instance.weaponSkinReinforcementMana += num;
					break;
				case WeaponSkinAbilityType.AutoCastConcecntration:
					Singleton<StatManager>.instance.weaponSkinConcentrationAutoCastChance += num;
					break;
				case WeaponSkinAbilityType.AutoCastCloneWarrior:
					Singleton<StatManager>.instance.weaponSkinCloneWarriorAutoCastChance += num;
					break;
				case WeaponSkinAbilityType.AutoCastDragonBreath:
					Singleton<StatManager>.instance.weaponSkinDragonBreathAutoCastChance += num;
					break;
				case WeaponSkinAbilityType.LegendaryChestDropChance:
					Singleton<StatManager>.instance.weaponSkinLegendaryChestDropChance += num;
					break;
				}
			}
		}
	}

	public void afterUseCubeEvent()
	{
		increaseWeaponSkinPiece(1L);
		Singleton<FlyResourcesManager>.instance.playEffectResources(new Vector2(0f, -2.862801f), FlyResourcesManager.ResourceType.WeaponSkinPiece, 1L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		double num = (double)Random.Range(0, 10000) / 100.0;
		if (num < 20.0)
		{
			increaseWeaponSkinReinforcementMasterPiece(1L);
			Singleton<FlyResourcesManager>.instance.playEffectResources(new Vector2(0f, -2.862801f), FlyResourcesManager.ResourceType.WeaponSkinReinforcementMasterPiece, 1L, 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
		}
	}

	public void touchToCastAutoSkill()
	{
		if (Singleton<CharacterManager>.instance.warriorCharacter.isFirstGround())
		{
			return;
		}
		if (Singleton<StatManager>.instance.weaponSkinConcentrationAutoCastChance > 0.0)
		{
			double num = (double)Random.Range(0, 10000) / 100.0;
			if (num < Singleton<StatManager>.instance.weaponSkinConcentrationAutoCastChance)
			{
				Singleton<SkillManager>.instance.castSkill(SkillManager.SkillType.Concentration);
			}
		}
		if (Singleton<StatManager>.instance.weaponSkinCloneWarriorAutoCastChance > 0.0)
		{
			double num2 = (double)Random.Range(0, 10000) / 100.0;
			if (num2 < Singleton<StatManager>.instance.weaponSkinCloneWarriorAutoCastChance)
			{
				Singleton<SkillManager>.instance.castSkill(SkillManager.SkillType.ClonedWarrior);
			}
		}
		if (Singleton<StatManager>.instance.weaponSkinDragonBreathAutoCastChance > 0.0)
		{
			double num3 = (double)Random.Range(0, 10000) / 100.0;
			if (num3 < Singleton<StatManager>.instance.weaponSkinDragonBreathAutoCastChance)
			{
				Singleton<SkillManager>.instance.castSkill(SkillManager.SkillType.DragonsBreath);
			}
		}
	}

	public Sprite getSpecialWeaponSkinSprite(WarriorSpecialWeaponSkinType skinType)
	{
		Sprite result = null;
		if (warriorSpecialWeaponSkinSprite.Length > 0 && warriorSpecialWeaponSkinSprite.Length > (int)skinType)
		{
			result = warriorSpecialWeaponSkinSprite[(int)skinType];
		}
		return result;
	}

	public Sprite getSpecialWeaponSkinSprite(PriestSpecialWeaponSkinType skinType)
	{
		Sprite result = null;
		if (priestSpecialWeaponSkinSprite.Length > 0 && priestSpecialWeaponSkinSprite.Length > (int)skinType)
		{
			result = priestSpecialWeaponSkinSprite[(int)skinType];
		}
		return result;
	}

	public Sprite getSpecialWeaponSkinSprite(ArcherSpecialWeaponSkinType skinType)
	{
		Sprite result = null;
		if (archerSpecialWeaponSkinSprite.Length > 0 && archerSpecialWeaponSkinSprite.Length > (int)skinType)
		{
			result = archerSpecialWeaponSkinSprite[(int)skinType];
		}
		return result;
	}

	public string getSpecialWeaponSkinName(WarriorSpecialWeaponSkinType skinType)
	{
		return I18NManager.Get("WARRIOR_SPECIAL_WEAPON_SKIN_" + (int)(skinType + 1));
	}

	public string getSpecialWeaponSkinName(PriestSpecialWeaponSkinType skinType)
	{
		return I18NManager.Get("PRIEST_SPECIAL_WEAPON_SKIN_" + (int)(skinType + 1));
	}

	public string getSpecialWeaponSkinName(ArcherSpecialWeaponSkinType skinType)
	{
		return I18NManager.Get("ARCHER_SPECIAL_WEAPON_SKIN_" + (int)(skinType + 1));
	}

	public WeaponSkinData getSpecialWeaponSkinData(CharacterManager.CharacterType characterType, int weaponSkinIndex, WeaponSkinGradeType grade)
	{
		WeaponSkinData weaponSkinData = new WeaponSkinData();
		switch (characterType)
		{
		case CharacterManager.CharacterType.Warrior:
			weaponSkinData.currentWarriorSpecialWeaponSkinType = (WarriorSpecialWeaponSkinType)weaponSkinIndex;
			break;
		case CharacterManager.CharacterType.Priest:
			weaponSkinData.currentPriestSpecialWeaponSkinType = (PriestSpecialWeaponSkinType)weaponSkinIndex;
			break;
		case CharacterManager.CharacterType.Archer:
			weaponSkinData.currentArcherSpecialWeaponSkinType = (ArcherSpecialWeaponSkinType)weaponSkinIndex;
			break;
		}
		weaponSkinData.isEquipped = false;
		weaponSkinData.abilityDataList = new List<WeaponSkinAbilityData>();
		for (int i = 0; i < m_weaponSkinAbilityCount[(int)grade]; i++)
		{
			weaponSkinData.abilityDataList.Add(getRandomAbility(grade, false));
		}
		weaponSkinData.currentGrade = grade;
		weaponSkinData.currentCharacterType = characterType;
		return weaponSkinData;
	}

	public WeaponSkinData getRandomWeaponSkinData(bool isPremiumLottery, bool isReinforcementLottery = false, WeaponSkinData baseSkinData = null, bool isOnlyLegendary = false)
	{
		bool flag = false;
		bool isGradeUp = false;
		WeaponSkinData weaponSkinData = ((baseSkinData != null) ? baseSkinData : new WeaponSkinData());
		if (baseSkinData == null)
		{
			weaponSkinData.isEquipped = false;
		}
		CharacterManager.CharacterType characterType = CharacterManager.CharacterType.Length;
		WeaponSkinGradeType weaponSkinGradeType = WeaponSkinGradeType.Rare;
		if (baseSkinData != null)
		{
			weaponSkinGradeType = baseSkinData.currentGrade;
			characterType = baseSkinData.currentCharacterType;
			flag = true;
		}
		else
		{
			characterType = (CharacterManager.CharacterType)m_randomForCharacterType.Next(0, 3);
			switch (characterType)
			{
			case CharacterManager.CharacterType.Warrior:
				weaponSkinData.currentWarriorWeaponType = getRandomWarriorWeaponSkinType();
				break;
			case CharacterManager.CharacterType.Priest:
				weaponSkinData.currentPriestWeaponType = getRandomPriestWeaponSkinType();
				break;
			case CharacterManager.CharacterType.Archer:
				weaponSkinData.currentArcherWeaponType = getRandomArcherWeaponSkinType();
				break;
			}
		}
		WeaponSkinGradeType ranodmGrade = getRanodmGrade(weaponSkinGradeType, isReinforcementLottery, baseSkinData == null, isPremiumLottery, isOnlyLegendary);
		if (ranodmGrade > weaponSkinGradeType)
		{
			isGradeUp = true;
		}
		if (weaponSkinGradeType == WeaponSkinGradeType.Unique && ranodmGrade == WeaponSkinGradeType.Legendary)
		{
			isReinforcementLottery = false;
		}
		weaponSkinData.abilityDataList = new List<WeaponSkinAbilityData>();
		for (int i = 0; i < m_weaponSkinAbilityCount[(int)ranodmGrade]; i++)
		{
			weaponSkinData.abilityDataList.Add(getRandomAbility(ranodmGrade, isReinforcementLottery));
		}
		weaponSkinData.currentGrade = ranodmGrade;
		weaponSkinData.currentCharacterType = characterType;
		if (flag)
		{
			UIWindowWeaponSkinLottery.instance.openCubeUI(weaponSkinData, isReinforcementLottery, isGradeUp);
		}
		return weaponSkinData;
	}

	private WeaponManager.WarriorWeaponType getRandomWarriorWeaponSkinType()
	{
		WeaponManager.WarriorWeaponType result = WeaponManager.WarriorWeaponType.Null;
		int num = 0;
		double num2 = 0.0;
		int num3 = 48;
		for (int i = 0; i < num3; i++)
		{
			num += num3 - i;
		}
		double num4 = m_randomForWeaponType.Next(0, num);
		for (int j = 0; j < num3; j++)
		{
			num2 += (double)(num3 - j);
			if (num4 <= num2)
			{
				result = (WeaponManager.WarriorWeaponType)(j + 1);
				break;
			}
		}
		return result;
	}

	private WeaponManager.PriestWeaponType getRandomPriestWeaponSkinType()
	{
		WeaponManager.PriestWeaponType result = WeaponManager.PriestWeaponType.Null;
		int num = 0;
		double num2 = 0.0;
		int num3 = 48;
		for (int i = 0; i < num3; i++)
		{
			num += num3 - i;
		}
		double num4 = m_randomForWeaponType.Next(0, num);
		for (int j = 0; j < num3; j++)
		{
			num2 += (double)(num3 - j);
			if (num4 <= num2)
			{
				result = (WeaponManager.PriestWeaponType)(j + 1);
				break;
			}
		}
		return result;
	}

	private WeaponManager.ArcherWeaponType getRandomArcherWeaponSkinType()
	{
		WeaponManager.ArcherWeaponType result = WeaponManager.ArcherWeaponType.Null;
		int num = 0;
		double num2 = 0.0;
		int num3 = 48;
		for (int i = 0; i < num3; i++)
		{
			num += num3 - i;
		}
		double num4 = m_randomForWeaponType.Next(0, num);
		for (int j = 0; j < num3; j++)
		{
			num2 += (double)(num3 - j);
			if (num4 <= num2)
			{
				result = (WeaponManager.ArcherWeaponType)(j + 1);
				break;
			}
		}
		return result;
	}

	private WeaponSkinGradeType getRanodmGrade(WeaponSkinGradeType minGrade, bool isReinforcementLottery, bool isFirstLottery, bool isPremiumLottery, bool isOnlyLegendary)
	{
		WeaponSkinGradeType weaponSkinGradeType = minGrade;
		if (!isOnlyLegendary)
		{
			if (weaponSkinGradeType < WeaponSkinGradeType.Legendary)
			{
				if (!isFirstLottery)
				{
					double num = getGradeUpChance(weaponSkinGradeType);
					if (isReinforcementLottery)
					{
						num *= 2.0;
					}
					double num2 = (double)m_randomForRandomGrade.Next(0, 10000) / 100.0;
					if (num2 < num)
					{
						weaponSkinGradeType++;
					}
				}
				else
				{
					if (!isPremiumLottery)
					{
						double num3 = 0.0;
						double num4 = 0.0;
						foreach (KeyValuePair<WeaponSkinGradeType, double> item in m_gradeAppearChanceDictionaryForFirstLottery)
						{
							num3 += item.Value;
						}
						double num5 = (double)m_randomForRandomGrade.Next(0, (int)num3 * 100) * 0.01;
						{
							foreach (KeyValuePair<WeaponSkinGradeType, double> item2 in m_gradeAppearChanceDictionaryForFirstLottery)
							{
								num4 += item2.Value;
								if (num5 <= num4)
								{
									return item2.Key;
								}
							}
							return weaponSkinGradeType;
						}
					}
					double num6 = (double)m_randomForRandomGrade.Next(0, 10000) / 100.0;
					weaponSkinGradeType = ((!(num6 <= 90.0)) ? WeaponSkinGradeType.Legendary : WeaponSkinGradeType.Unique);
				}
			}
		}
		else
		{
			weaponSkinGradeType = WeaponSkinGradeType.Legendary;
		}
		return weaponSkinGradeType;
	}

	private double getGradeUpChance(WeaponSkinGradeType currentGrade)
	{
		double result = 0.0;
		switch (currentGrade)
		{
		case WeaponSkinGradeType.Rare:
			result = 20.0;
			break;
		case WeaponSkinGradeType.Epic:
			result = 6.0;
			break;
		case WeaponSkinGradeType.Unique:
			result = 0.55;
			break;
		}
		return result;
	}

	private WeaponSkinAbilityData getRandomAbility(WeaponSkinGradeType currentGrade, bool isReinforcementLottery)
	{
		WeaponSkinAbilityData weaponSkinAbilityData = new WeaponSkinAbilityData();
		weaponSkinAbilityData.currentAbilityType = WeaponSkinAbilityType.None;
		List<WeaponSkinAbilityType> list = new List<WeaponSkinAbilityType>();
		for (int i = 0; i < 21; i++)
		{
			WeaponSkinAbilityType weaponSkinAbilityType = (WeaponSkinAbilityType)i;
			if (Singleton<ParsingManager>.instance.currentParsedStatData.weaponSkinAbilityStatData[weaponSkinAbilityType].abilityAppearGradeType <= currentGrade)
			{
				list.Add(weaponSkinAbilityType);
			}
		}
		if (list.Count > 0)
		{
			weaponSkinAbilityData.currentAbilityType = list[m_randomForRandomAbilityType.Next(0, list.Count)];
			WeaponSkinGradeType weaponSkinGradeType = (WeaponSkinGradeType)m_randomForRandomAbilityValue.Next(0, (int)(currentGrade + 1));
			if (currentGrade == WeaponSkinGradeType.Legendary)
			{
				WeaponSkinGradeType weaponSkinGradeType2 = ((!isReinforcementLottery) ? WeaponSkinGradeType.Epic : WeaponSkinGradeType.Legendary);
				while (weaponSkinGradeType < weaponSkinGradeType2)
				{
					weaponSkinGradeType = (WeaponSkinGradeType)m_randomForRandomAbilityValue.Next(0, (int)(currentGrade + 1));
				}
			}
			weaponSkinAbilityData.valueGradeType = weaponSkinGradeType;
		}
		return weaponSkinAbilityData;
	}

	public Sprite getGradeIconSprite(WeaponSkinGradeType gradeType)
	{
		return gradeIconSprites[(int)gradeType];
	}

	public Sprite getGradeBackgroundSprite(WeaponSkinGradeType gradeType)
	{
		return gradeBackgroundSprites[(int)gradeType];
	}

	public string getGradeString(WeaponSkinGradeType gradeType)
	{
		string result = string.Empty;
		switch (gradeType)
		{
		case WeaponSkinGradeType.Rare:
			result = I18NManager.Get("RARE");
			break;
		case WeaponSkinGradeType.Epic:
			result = I18NManager.Get("EPIC");
			break;
		case WeaponSkinGradeType.Unique:
			result = I18NManager.Get("UNIQUE");
			break;
		case WeaponSkinGradeType.Legendary:
			result = I18NManager.Get("LEGENDARY");
			break;
		}
		return result;
	}

	public string getAbilityString(List<WeaponSkinAbilityData> abilityList)
	{
		string text = string.Empty;
		for (int i = 0; i < abilityList.Count; i++)
		{
			if (i != 0)
			{
				text += "\n";
			}
			text = text + "- " + getAbilityString(abilityList[i]);
		}
		return text;
	}

	public string getAbilityString(WeaponSkinAbilityData abilityData)
	{
		string empty = string.Empty;
		double num = Singleton<ParsingManager>.instance.currentParsedStatData.weaponSkinAbilityStatData[abilityData.currentAbilityType].statDictionary[abilityData.valueGradeType];
		return empty + string.Format(getAbilityDescription(abilityData.currentAbilityType), num);
	}

	public string getAbilityDescription(WeaponSkinAbilityType abilityType)
	{
		return I18NManager.Get("WEAPON_SKIN_ABILITY_DESCRIPTION_" + (int)(abilityType + 1));
	}

	public WeaponSkinData getEquippedWeaponSkinData(CharacterManager.CharacterType characterType)
	{
		WeaponSkinData result = null;
		for (int i = 0; i < Singleton<DataManager>.instance.currentGameData.weaponSkinData[characterType].Count; i++)
		{
			if (Singleton<DataManager>.instance.currentGameData.weaponSkinData[characterType][i].isEquipped)
			{
				result = Singleton<DataManager>.instance.currentGameData.weaponSkinData[characterType][i];
				break;
			}
		}
		return result;
	}

	public long getMasterPiecePrice(WeaponSkinGradeType gradeType)
	{
		long result = 0L;
		switch (gradeType)
		{
		case WeaponSkinGradeType.Rare:
			result = 5L;
			break;
		case WeaponSkinGradeType.Epic:
			result = 25L;
			break;
		case WeaponSkinGradeType.Unique:
			result = 50L;
			break;
		case WeaponSkinGradeType.Legendary:
			result = 100L;
			break;
		}
		return result;
	}

	public long getMinOrMaxWeaponSkinPieceForDestory(WeaponSkinGradeType gradeType, bool isMin)
	{
		long result = 0L;
		switch (gradeType)
		{
		case WeaponSkinGradeType.Rare:
			result = (isMin ? 1 : 10);
			break;
		case WeaponSkinGradeType.Epic:
			result = ((!isMin) ? 30 : 15);
			break;
		case WeaponSkinGradeType.Unique:
			result = ((!isMin) ? 120 : 80);
			break;
		case WeaponSkinGradeType.Legendary:
			result = ((!isMin) ? 400 : 200);
			break;
		}
		return result;
	}

	public long getRandomWeaponSkinPieceForDestory(WeaponSkinGradeType gradeType)
	{
		long num = 0L;
		long minOrMaxWeaponSkinPieceForDestory = getMinOrMaxWeaponSkinPieceForDestory(gradeType, true);
		long minOrMaxWeaponSkinPieceForDestory2 = getMinOrMaxWeaponSkinPieceForDestory(gradeType, false);
		return (int)Random.Range(minOrMaxWeaponSkinPieceForDestory, minOrMaxWeaponSkinPieceForDestory2 + 1);
	}

	public long getRandomWeaponSkinMasterPieceFromDestory(WeaponSkinGradeType gradeType)
	{
		long result = 0L;
		switch (gradeType)
		{
		case WeaponSkinGradeType.Rare:
			result = Random.Range(1, 3);
			break;
		case WeaponSkinGradeType.Epic:
			result = Random.Range(5, 11);
			break;
		case WeaponSkinGradeType.Unique:
			result = Random.Range(20, 36);
			break;
		case WeaponSkinGradeType.Legendary:
			result = Random.Range(250, 401);
			break;
		}
		return result;
	}

	public long getRandomWeaponSkinMasterPieceAppearChance(WeaponSkinGradeType gradeType)
	{
		long result = 0L;
		switch (gradeType)
		{
		case WeaponSkinGradeType.Rare:
			result = 40L;
			break;
		case WeaponSkinGradeType.Epic:
			result = 50L;
			break;
		case WeaponSkinGradeType.Unique:
			result = 70L;
			break;
		case WeaponSkinGradeType.Legendary:
			result = 100L;
			break;
		}
		return result;
	}

	public Vector3 getWeaponIconRotation(CharacterManager.CharacterType characterType)
	{
		Vector3 result = Vector3.zero;
		switch (characterType)
		{
		case CharacterManager.CharacterType.Warrior:
		case CharacterManager.CharacterType.Archer:
			result = new Vector3(0f, 0f, 135f);
			break;
		case CharacterManager.CharacterType.Priest:
			result = new Vector3(0f, 0f, 45f);
			break;
		}
		return result;
	}
}
