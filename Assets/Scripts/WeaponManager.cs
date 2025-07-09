using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : Singleton<WeaponManager>
{
	public enum WarriorWeaponType
	{
		Null,
		WarriorWeapon1,
		WarriorWeapon2,
		WarriorWeapon3,
		WarriorWeapon4,
		WarriorWeapon5,
		WarriorWeapon6,
		WarriorWeapon7,
		WarriorWeapon8,
		WarriorWeapon9,
		WarriorWeapon10,
		WarriorWeapon11,
		WarriorWeapon12,
		WarriorWeapon13,
		WarriorWeapon14,
		WarriorWeapon15,
		WarriorWeapon16,
		WarriorWeapon17,
		WarriorWeapon18,
		WarriorWeapon19,
		WarriorWeapon20,
		WarriorWeapon21,
		WarriorWeapon22,
		WarriorWeapon23,
		WarriorWeapon24,
		WarriorWeapon25,
		WarriorWeapon26,
		WarriorWeapon27,
		WarriorWeapon28,
		WarriorWeapon29,
		WarriorWeapon30,
		WarriorWeapon31,
		WarriorWeapon32,
		WarriorWeapon33,
		WarriorWeapon34,
		WarriorWeapon35,
		WarriorWeapon36,
		WarriorWeapon37,
		WarriorWeapon38,
		WarriorWeapon39,
		WarriorWeapon40,
		WarriorWeapon41,
		WarriorWeapon42,
		WarriorWeapon43,
		WarriorWeapon44,
		WarriorWeapon45,
		WarriorWeapon46,
		WarriorWeapon47,
		Length
	}

	public enum PriestWeaponType
	{
		Null,
		PriestWeapon1,
		PriestWeapon2,
		PriestWeapon3,
		PriestWeapon4,
		PriestWeapon5,
		PriestWeapon6,
		PriestWeapon7,
		PriestWeapon8,
		PriestWeapon9,
		PriestWeapon10,
		PriestWeapon11,
		PriestWeapon12,
		PriestWeapon13,
		PriestWeapon14,
		PriestWeapon15,
		PriestWeapon16,
		PriestWeapon17,
		PriestWeapon18,
		PriestWeapon19,
		PriestWeapon20,
		PriestWeapon21,
		PriestWeapon22,
		PriestWeapon23,
		PriestWeapon24,
		PriestWeapon25,
		PriestWeapon26,
		PriestWeapon27,
		PriestWeapon28,
		PriestWeapon29,
		PriestWeapon30,
		PriestWeapon31,
		PriestWeapon32,
		PriestWeapon33,
		PriestWeapon34,
		PriestWeapon35,
		PriestWeapon36,
		PriestWeapon37,
		PriestWeapon38,
		PriestWeapon39,
		PriestWeapon40,
		PriestWeapon41,
		PriestWeapon42,
		PriestWeapon43,
		PriestWeapon44,
		PriestWeapon45,
		PriestWeapon46,
		PriestWeapon47,
		Length
	}

	public enum ArcherWeaponType
	{
		Null,
		ArcherWeapon1,
		ArcherWeapon2,
		ArcherWeapon3,
		ArcherWeapon4,
		ArcherWeapon5,
		ArcherWeapon6,
		ArcherWeapon7,
		ArcherWeapon8,
		ArcherWeapon9,
		ArcherWeapon10,
		ArcherWeapon11,
		ArcherWeapon12,
		ArcherWeapon13,
		ArcherWeapon14,
		ArcherWeapon15,
		ArcherWeapon16,
		ArcherWeapon17,
		ArcherWeapon18,
		ArcherWeapon19,
		ArcherWeapon20,
		ArcherWeapon21,
		ArcherWeapon22,
		ArcherWeapon23,
		ArcherWeapon24,
		ArcherWeapon25,
		ArcherWeapon26,
		ArcherWeapon27,
		ArcherWeapon28,
		ArcherWeapon29,
		ArcherWeapon30,
		ArcherWeapon31,
		ArcherWeapon32,
		ArcherWeapon33,
		ArcherWeapon34,
		ArcherWeapon35,
		ArcherWeapon36,
		ArcherWeapon37,
		ArcherWeapon38,
		ArcherWeapon39,
		ArcherWeapon40,
		ArcherWeapon41,
		ArcherWeapon42,
		ArcherWeapon43,
		ArcherWeapon44,
		ArcherWeapon45,
		ArcherWeapon46,
		ArcherWeapon47,
		Length
	}

	public static long quickUpgradeTargetEnchantCount = 25L;

	public Weapon warriorEquippedWeapon;

	public Weapon archerEquippedWeapon;

	public Weapon priestEquippedWeapon;

	public Sprite warriorSecondStatIconSprite;

	public Sprite priestSecondStatIconSprite;

	public Sprite archerSecondStatIconSprite;

	public List<WarriorWeaponType> warriorSlotWeaponTypeList = new List<WarriorWeaponType>();

	public List<PriestWeaponType> priestSlotWeaponTypeList = new List<PriestWeaponType>();

	public List<ArcherWeaponType> archerSlotWeaponTypeList = new List<ArcherWeaponType>();

	public Sprite[] warriorWeaponIngameSprites;

	public Sprite[] priestWeaponIngameSprites;

	public Sprite[] archerWeaponIngameSprites;

	public Text[] enchantValueTexts;

	public float enchantStoneCatchRange = 5f;

	public Transform moveTargetTransform;

	private void Start()
	{
		refreshSlotUnlockStatus();
	}

	private void refreshSlotUnlockStatus()
	{
		for (int i = 1; i <= 44; i++)
		{
			WarriorWeaponType type = (WarriorWeaponType)i;
			getWeaponFromInventory(type).isUnlockedFromSlot = true;
		}
		for (int j = 1; j <= 44; j++)
		{
			PriestWeaponType type2 = (PriestWeaponType)j;
			getWeaponFromInventory(type2).isUnlockedFromSlot = true;
		}
		for (int k = 1; k <= 44; k++)
		{
			ArcherWeaponType type3 = (ArcherWeaponType)k;
			getWeaponFromInventory(type3).isUnlockedFromSlot = true;
		}
	}

	public void refreshWeaponSlotTypeList()
	{
		warriorSlotWeaponTypeList.Clear();
		priestSlotWeaponTypeList.Clear();
		archerSlotWeaponTypeList.Clear();
		for (int i = 1; i < 48; i++)
		{
			WarriorWeaponType item = (WarriorWeaponType)i;
			warriorSlotWeaponTypeList.Add(item);
		}
		for (int j = 1; j < 48; j++)
		{
			PriestWeaponType item2 = (PriestWeaponType)j;
			priestSlotWeaponTypeList.Add(item2);
		}
		for (int k = 1; k < 48; k++)
		{
			ArcherWeaponType item3 = (ArcherWeaponType)k;
			archerSlotWeaponTypeList.Add(item3);
		}
	}

	public bool buyWeapon(WarriorWeaponType type)
	{
		if (!getWeaponFromInventory(type).isHaving)
		{
			double weaponGoldPriceForUnlock = getWeaponGoldPriceForUnlock(getWeaponTier(type));
			if (Singleton<DataManager>.instance.currentGameData.gold >= weaponGoldPriceForUnlock)
			{
				Singleton<GoldManager>.instance.decreaseGold(weaponGoldPriceForUnlock);
				obtainWeapon(type);
				Singleton<DataManager>.instance.saveData();
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Gold, AnalyzeManager.ActionType.BuyWeaponByGold, new Dictionary<string, string>
				{
					{
						"WeaponType",
						type.ToString()
					}
				});
				return true;
			}
			UIWindowDialog.openDescription("NOT_ENOUGH_GOLD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
				UIWindowPopupShop.instance.focusToGold();
			}, string.Empty);
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
		}
		return false;
	}

	public bool buyWeapon(PriestWeaponType type)
	{
		if (!getWeaponFromInventory(type).isHaving)
		{
			double weaponGoldPriceForUnlock = getWeaponGoldPriceForUnlock(getWeaponTier(type));
			if (Singleton<DataManager>.instance.currentGameData.gold >= weaponGoldPriceForUnlock)
			{
				Singleton<GoldManager>.instance.decreaseGold(weaponGoldPriceForUnlock);
				obtainWeapon(type);
				Singleton<DataManager>.instance.saveData();
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Gold, AnalyzeManager.ActionType.BuyWeaponByGold, new Dictionary<string, string>
				{
					{
						"WeaponType",
						type.ToString()
					}
				});
				return true;
			}
			UIWindowDialog.openDescription("NOT_ENOUGH_GOLD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
				UIWindowPopupShop.instance.focusToGold();
			}, string.Empty);
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
		}
		return false;
	}

	public bool buyWeapon(ArcherWeaponType type)
	{
		if (!getWeaponFromInventory(type).isHaving)
		{
			double weaponGoldPriceForUnlock = getWeaponGoldPriceForUnlock(getWeaponTier(type));
			if (Singleton<DataManager>.instance.currentGameData.gold >= weaponGoldPriceForUnlock)
			{
				obtainWeapon(type);
				Singleton<GoldManager>.instance.decreaseGold(weaponGoldPriceForUnlock);
				Singleton<DataManager>.instance.saveData();
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Gold, AnalyzeManager.ActionType.BuyWeaponByGold, new Dictionary<string, string>
				{
					{
						"WeaponType",
						type.ToString()
					}
				});
				return true;
			}
			UIWindowDialog.openDescription("NOT_ENOUGH_GOLD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
				UIWindowPopupShop.instance.focusToGold();
			}, string.Empty);
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
		}
		return false;
	}

	public int getMaxWeaponSpawnIndex()
	{
		return 48;
	}

	public void obtainWeapon(WarriorWeaponType type, bool save = true)
	{
		WarriorWeaponInventoryData weaponFromInventory = Singleton<WeaponManager>.instance.getWeaponFromInventory(type);
		if (!weaponFromInventory.isHaving)
		{
			weaponFromInventory.weapontype = type;
			weaponFromInventory.isHaving = true;
			weaponFromInventory.enchantCount = 0L;
			if (save)
			{
				Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.WeaponCollector, 1.0);
				Singleton<DataManager>.instance.saveData();
			}
		}
	}

	public void obtainWeapon(PriestWeaponType type, bool save = true)
	{
		PriestWeaponInventoryData weaponFromInventory = Singleton<WeaponManager>.instance.getWeaponFromInventory(type);
		if (!weaponFromInventory.isHaving)
		{
			weaponFromInventory.weapontype = type;
			weaponFromInventory.isHaving = true;
			weaponFromInventory.enchantCount = 0L;
			if (save)
			{
				Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.WeaponCollector, 1.0);
				Singleton<DataManager>.instance.saveData();
			}
		}
	}

	public void obtainWeapon(ArcherWeaponType type, bool save = true)
	{
		ArcherWeaponInventoryData weaponFromInventory = Singleton<WeaponManager>.instance.getWeaponFromInventory(type);
		if (!weaponFromInventory.isHaving)
		{
			weaponFromInventory.weapontype = type;
			weaponFromInventory.isHaving = true;
			weaponFromInventory.enchantCount = 0L;
			if (save)
			{
				Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.WeaponCollector, 1.0);
				Singleton<DataManager>.instance.saveData();
			}
		}
	}

	public void equipWeapon(WarriorWeaponType weaponType)
	{
		if (!Singleton<DataManager>.instance.currentGameData.warriorWeaponInventoryData[(int)(weaponType - 1)].isHaving)
		{
			equipWeapon(WarriorWeaponType.WarriorWeapon1);
			return;
		}
		if (warriorEquippedWeapon != null)
		{
			if (checkCmpareEquipedWeapon(weaponType))
			{
				return;
			}
			ObjectPool.Recycle(warriorEquippedWeapon.name, warriorEquippedWeapon.cachedGameObject);
			ObjectPool.Spawn("fx_character_upgrade", (Vector2)UIWindowManageHeroAndWeapon.instance.warriorImage.position - new Vector2(0f, 0.15f), new Vector3(0f, 180f, 180f));
			ObjectPool.Spawn("fx_character_upgrade", (Vector2)UIWindowOutgame.instance.warriorTabImage.transform.position - new Vector2(0f, 0.15f), new Vector3(0f, 180f, 180f));
		}
		Singleton<CharacterManager>.instance.warriorCharacter.equippedWeapon = null;
		warriorEquippedWeapon = null;
		Weapon component = ObjectPool.Spawn("@Weapon", Vector3.zero, Singleton<CharacterManager>.instance.warriorCharacter.weaponParentTransform).GetComponent<Weapon>();
		component.weaponCharacterType = CharacterManager.CharacterType.Warrior;
		component.warriorWeaponType = weaponType;
		component.initWeapon(true);
		component.cachedTransform.localEulerAngles = Vector3.zero;
		component.cachedTransform.localScale = Vector3.one;
		component.weaponRealStats = getWeaponStat(component.warriorWeaponType, getWeaponFromInventory(component.warriorWeaponType).enchantCount);
		warriorEquippedWeapon = component;
		Singleton<CharacterManager>.instance.warriorCharacter.equippedWeapon = component;
		Singleton<DataManager>.instance.currentGameData.warriorEquippedWeapon = weaponType;
		Singleton<CharacterManager>.instance.warriorCharacter.resetProperties();
		Singleton<CharacterManager>.instance.warriorCharacter.setState(PublicDataManager.State.Wait);
		Singleton<DataManager>.instance.saveData();
	}

	public void equipWeapon(PriestWeaponType weaponType)
	{
		if (!Singleton<WeaponManager>.instance.getWeaponFromInventory(weaponType).isHaving)
		{
			equipWeapon(PriestWeaponType.PriestWeapon1);
			return;
		}
		if (priestEquippedWeapon != null)
		{
			if (checkCmpareEquipedWeapon(weaponType))
			{
				return;
			}
			ObjectPool.Recycle(priestEquippedWeapon.name, priestEquippedWeapon.cachedGameObject);
			ObjectPool.Spawn("fx_character_upgrade", (Vector2)UIWindowManageHeroAndWeapon.instance.priestImage.position - new Vector2(0f, 0.15f), new Vector3(0f, 180f, 180f));
			ObjectPool.Spawn("fx_character_upgrade", (Vector2)UIWindowOutgame.instance.priestTabImage.transform.position - new Vector2(0f, 0.15f), new Vector3(0f, 180f, 180f));
		}
		Singleton<CharacterManager>.instance.priestCharacter.equippedWeapon = null;
		priestEquippedWeapon = null;
		Weapon component = ObjectPool.Spawn("@Weapon", Vector3.zero, Singleton<CharacterManager>.instance.priestCharacter.weaponParentTransform).GetComponent<Weapon>();
		component.weaponCharacterType = CharacterManager.CharacterType.Priest;
		component.priestWeaponType = weaponType;
		component.initWeapon(true);
		component.cachedTransform.localEulerAngles = Vector3.zero;
		component.cachedTransform.localScale = Vector3.one;
		component.weaponRealStats = getWeaponStat(component.priestWeaponType, getWeaponFromInventory(component.priestWeaponType).enchantCount);
		priestEquippedWeapon = component;
		Singleton<CharacterManager>.instance.priestCharacter.equippedWeapon = component;
		Singleton<DataManager>.instance.currentGameData.priestEquippedWeapon = weaponType;
		Singleton<CharacterManager>.instance.priestCharacter.resetProperties();
		Singleton<CharacterManager>.instance.priestCharacter.setState(PublicDataManager.State.Wait);
		Singleton<DataManager>.instance.saveData();
	}

	public void equipWeapon(ArcherWeaponType weaponType)
	{
		if (!Singleton<WeaponManager>.instance.getWeaponFromInventory(weaponType).isHaving)
		{
			equipWeapon(ArcherWeaponType.ArcherWeapon1);
			return;
		}
		if (archerEquippedWeapon != null)
		{
			if (checkCmpareEquipedWeapon(weaponType))
			{
				return;
			}
			ObjectPool.Recycle(archerEquippedWeapon.name, archerEquippedWeapon.cachedGameObject);
			ObjectPool.Spawn("fx_character_upgrade", (Vector2)UIWindowManageHeroAndWeapon.instance.archerImage.position - new Vector2(0f, 0.15f), new Vector3(0f, 180f, 180f));
			ObjectPool.Spawn("fx_character_upgrade", (Vector2)UIWindowOutgame.instance.archerTabImage.transform.position - new Vector2(0f, 0.15f), new Vector3(0f, 180f, 180f));
		}
		Singleton<CharacterManager>.instance.archerCharacter.equippedWeapon = null;
		archerEquippedWeapon = null;
		Weapon component = ObjectPool.Spawn("@Weapon", Vector3.zero, Singleton<CharacterManager>.instance.archerCharacter.weaponParentTransform).GetComponent<Weapon>();
		component.weaponCharacterType = CharacterManager.CharacterType.Archer;
		component.archerWeaponType = weaponType;
		component.initWeapon(true);
		component.cachedTransform.localEulerAngles = Vector3.zero;
		component.cachedTransform.localScale = Vector3.one;
		component.weaponRealStats = getWeaponStat(component.archerWeaponType, getWeaponFromInventory(component.archerWeaponType).enchantCount);
		archerEquippedWeapon = component;
		Singleton<CharacterManager>.instance.archerCharacter.equippedWeapon = component;
		Singleton<DataManager>.instance.currentGameData.archerEquippedWeapon = weaponType;
		Singleton<CharacterManager>.instance.archerCharacter.resetProperties();
		Singleton<CharacterManager>.instance.archerCharacter.setState(PublicDataManager.State.Wait);
		Singleton<DataManager>.instance.saveData();
	}

	public long getWeaponUpgradeMaxLevel()
	{
		long num = 0L;
		return long.MaxValue;
	}

	[ContextMenu("Get Ingame Weapon Sprites")]
	public void getIngameWeaponSprites()
	{
		Sprite[] array = Resources.LoadAll<Sprite>("Sprites/IngameAtlas/IngameAtlas");
		warriorWeaponIngameSprites = new Sprite[47];
		priestWeaponIngameSprites = new Sprite[47];
		archerWeaponIngameSprites = new Sprite[47];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].name.Contains("WarriorWeapon"))
			{
				warriorWeaponIngameSprites[int.Parse(Regex.Replace(array[i].name, "\\D", string.Empty)) - 1] = array[i];
			}
			if (array[i].name.Contains("PriestWeapon"))
			{
				priestWeaponIngameSprites[int.Parse(Regex.Replace(array[i].name, "\\D", string.Empty)) - 1] = array[i];
			}
			if (array[i].name.Contains("ArcherWeapon"))
			{
				archerWeaponIngameSprites[int.Parse(Regex.Replace(array[i].name, "\\D", string.Empty)) - 1] = array[i];
			}
		}
	}

	public int getWeaponTier(WarriorWeaponType weaponType)
	{
		int num = 0;
		return (int)weaponType;
	}

	public int getWeaponTier(PriestWeaponType weaponType)
	{
		int num = 0;
		return (int)weaponType;
	}

	public int getWeaponTier(ArcherWeaponType weaponType)
	{
		int num = 0;
		return (int)weaponType;
	}

	public long getUnlockNextRequiredLevel()
	{
		return 24L;
	}

	public string getWeaponStatI18NName(StatManager.WeaponStatType weaponStatType)
	{
		string result = string.Empty;
		switch (weaponStatType)
		{
		case StatManager.WeaponStatType.FixedDamage:
			result = "ATTRIBUTE_DAMAGE";
			break;
		case StatManager.WeaponStatType.FixedHealth:
			result = "ATTRIBUTE_HEALTH";
			break;
		case StatManager.WeaponStatType.CriticalChance:
			result = "ATTRIBUTE_CRITICAL_CHANCE";
			break;
		case StatManager.WeaponStatType.TapHeal:
			result = "ATTRIBUTE_TAP_HEAL";
			break;
		}
		return result;
	}

	public Sprite getWeaponSprite(WarriorWeaponType weaponType)
	{
		return warriorWeaponIngameSprites[(int)(weaponType - 1)];
	}

	public Sprite getWeaponSprite(PriestWeaponType weaponType)
	{
		return priestWeaponIngameSprites[(int)(weaponType - 1)];
	}

	public Sprite getWeaponSprite(ArcherWeaponType weaponType)
	{
		return archerWeaponIngameSprites[(int)(weaponType - 1)];
	}

	public WarriorWeaponInventoryData getWeaponFromInventory(WarriorWeaponType type)
	{
		WarriorWeaponInventoryData result = null;
		List<WarriorWeaponInventoryData> warriorWeaponInventoryData = Singleton<DataManager>.instance.currentGameData.warriorWeaponInventoryData;
		for (int i = 0; i < warriorWeaponInventoryData.Count; i++)
		{
			if (warriorWeaponInventoryData[i].weapontype == type)
			{
				result = warriorWeaponInventoryData[i];
				break;
			}
		}
		return result;
	}

	public ArcherWeaponInventoryData getWeaponFromInventory(ArcherWeaponType type)
	{
		ArcherWeaponInventoryData result = null;
		List<ArcherWeaponInventoryData> archerWeaponInventoryData = Singleton<DataManager>.instance.currentGameData.archerWeaponInventoryData;
		for (int i = 0; i < archerWeaponInventoryData.Count; i++)
		{
			if (archerWeaponInventoryData[i].weapontype == type)
			{
				result = archerWeaponInventoryData[i];
				break;
			}
		}
		return result;
	}

	public PriestWeaponInventoryData getWeaponFromInventory(PriestWeaponType type)
	{
		PriestWeaponInventoryData result = null;
		List<PriestWeaponInventoryData> priestWeaponInventoryData = Singleton<DataManager>.instance.currentGameData.priestWeaponInventoryData;
		for (int i = 0; i < priestWeaponInventoryData.Count; i++)
		{
			if (priestWeaponInventoryData[i].weapontype == type)
			{
				result = priestWeaponInventoryData[i];
				break;
			}
		}
		return result;
	}

	public double getWeaponGoldPriceForUnlock(int weaponTier)
	{
		double num = 0.0;
		return 100.0 * Math.Pow(3.0, weaponTier - 1);
	}

	public WeaponStat getWeaponStat(WarriorWeaponType weaponType, long enchantCount)
	{
		WeaponStat result = default(WeaponStat);
		result.secondStatType = StatManager.WeaponStatType.FixedHealth;
		result.weaponDamage = Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponDamageVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponDamageVariable2), getWeaponTier(weaponType) - 1) * (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponDamageVariable3) + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponDamageVariable4) * (double)enchantCount) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponDamageAllMultyply);
		result.secondStatValue = Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponHPVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponHPVariable2), getWeaponTier(weaponType) - 1) * (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponHPVariable3) + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponHPVariable4) * (double)enchantCount) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.WarriorWeaponHPAllMultyply);
		return result;
	}

	public WeaponStat getWeaponStat(PriestWeaponType weaponType, long enchantCount)
	{
		WeaponStat result = default(WeaponStat);
		result.secondStatType = StatManager.WeaponStatType.TapHeal;
		result.weaponDamage = Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestWeaponDamageVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestWeaponDamageVariable2), getWeaponTier(weaponType) - 1) * (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestWeaponDamageVariable3) + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestWeaponDamageVariable4) * (double)enchantCount) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestWeaponDamageAllMultyply);
		result.secondStatValue = Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestTapHealVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestTapHealVariable2), getWeaponTier(weaponType) - 1) * (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestTapHealVariable3) + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestTapHealVariable4) * (double)enchantCount) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.PriestTapHealAllMultyply);
		return result;
	}

	public WeaponStat getWeaponStat(ArcherWeaponType weaponType, long enchantCount)
	{
		WeaponStat result = default(WeaponStat);
		result.secondStatType = StatManager.WeaponStatType.CriticalChance;
		result.weaponDamage = Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ArcherWeaponDamageVariable1) * Math.Pow(Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ArcherWeaponDamageVariable2), getWeaponTier(weaponType) - 1) * (Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ArcherWeaponDamageVariable3) + Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ArcherWeaponDamageVariable4) * (double)enchantCount) * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ArcherWeaponDamageAllMultyply);
		result.secondStatValue = Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.ArcherCriticalChanceVariable1) * (double)enchantCount;
		result.secondStatValue = Util.Clamp(result.secondStatValue, 0.0, 25.0);
		return result;
	}

	public StatManager.WeaponStatType getSecondStatType(CharacterManager.CharacterType characterType)
	{
		StatManager.WeaponStatType result = StatManager.WeaponStatType.FixedDamage;
		switch (characterType)
		{
		case CharacterManager.CharacterType.Warrior:
			result = getWeaponStat(WarriorWeaponType.WarriorWeapon1, 1L).secondStatType;
			break;
		case CharacterManager.CharacterType.Priest:
			result = getWeaponStat(PriestWeaponType.PriestWeapon1, 1L).secondStatType;
			break;
		case CharacterManager.CharacterType.Archer:
			result = getWeaponStat(ArcherWeaponType.ArcherWeapon1, 1L).secondStatType;
			break;
		}
		return result;
	}

	public string getWeaponDamageDescription(WeaponStat currentStatDataOfWeapon)
	{
		string empty = string.Empty;
		return string.Format(I18NManager.Get(getWeaponStatI18NName(StatManager.WeaponStatType.FixedDamage)), string.Format("{0:0.##}", currentStatDataOfWeapon.weaponDamage));
	}

	public string getSecondStatDescription(WeaponStat currentStatDataOfWeapon)
	{
		string result = string.Empty;
		for (int i = 0; i < 4; i++)
		{
			if (i == (int)currentStatDataOfWeapon.secondStatType)
			{
				result = ((i != 2) ? string.Format(I18NManager.Get(getWeaponStatI18NName(currentStatDataOfWeapon.secondStatType)), GameManager.changeUnit((int)currentStatDataOfWeapon.secondStatValue)) : string.Format(I18NManager.Get(getWeaponStatI18NName(currentStatDataOfWeapon.secondStatType)), string.Format("{0:0.##}", currentStatDataOfWeapon.secondStatValue)));
			}
		}
		return result;
	}

	public bool isWeaponInventoryAvailable()
	{
		return Singleton<DataManager>.instance.currentGameData.warriorWeaponInventoryData.Count > 0 && Singleton<DataManager>.instance.currentGameData.archerWeaponInventoryData.Count > 0 && Singleton<DataManager>.instance.currentGameData.priestWeaponInventoryData.Count > 0;
	}

	public bool containInventory(WarriorWeaponType weaponType)
	{
		bool flag = false;
		return Singleton<DataManager>.instance.currentGameData.warriorWeaponInventoryData[(int)(weaponType - 1)].isHaving;
	}

	public bool containInventory(PriestWeaponType weaponType)
	{
		bool flag = false;
		return Singleton<DataManager>.instance.currentGameData.priestWeaponInventoryData[(int)(weaponType - 1)].isHaving;
	}

	public bool containInventory(ArcherWeaponType weaponType)
	{
		bool flag = false;
		return Singleton<DataManager>.instance.currentGameData.archerWeaponInventoryData[(int)(weaponType - 1)].isHaving;
	}

	public bool checkCmpareEquipedWeapon(WarriorWeaponType weaponType)
	{
		bool result = false;
		if (warriorEquippedWeapon != null && warriorEquippedWeapon.warriorWeaponType == weaponType)
		{
			result = true;
		}
		return result;
	}

	public bool checkCmpareEquipedWeapon(PriestWeaponType weaponType)
	{
		bool result = false;
		if (priestEquippedWeapon != null && priestEquippedWeapon.priestWeaponType == weaponType)
		{
			result = true;
		}
		return result;
	}

	public bool checkCmpareEquipedWeapon(ArcherWeaponType weaponType)
	{
		bool result = false;
		if (archerEquippedWeapon != null && archerEquippedWeapon.archerWeaponType == weaponType)
		{
			result = true;
		}
		return result;
	}

	public string getWeaponI18NName(WarriorWeaponType weaponType)
	{
		string empty = string.Empty;
		return I18NManager.Get(weaponType.ToString().ToUpper() + "_NAME");
	}

	public string getWeaponI18NName(PriestWeaponType weaponType)
	{
		string empty = string.Empty;
		return I18NManager.Get(weaponType.ToString().ToUpper() + "_NAME");
	}

	public string getWeaponI18NName(ArcherWeaponType weaponType)
	{
		string empty = string.Empty;
		return I18NManager.Get(weaponType.ToString().ToUpper() + "_NAME");
	}

	public void upgradeWeapon(WeaponSlot targetWeaponSlot, CharacterManager.CharacterType characterType, long enchantIncreaseCount = 1)
	{
		switch (characterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			WarriorWeaponInventoryData weaponFromInventory2 = getWeaponFromInventory(targetWeaponSlot.currentWarriorWeaponType);
			weaponFromInventory2.enchantCount += enchantIncreaseCount;
			if (TutorialManager.isTutorial && weaponFromInventory2.weapontype == WarriorWeaponType.WarriorWeapon1 && weaponFromInventory2.enchantCount >= 9)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType16);
			}
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			PriestWeaponInventoryData weaponFromInventory3 = getWeaponFromInventory(targetWeaponSlot.currentPriestWeaponType);
			weaponFromInventory3.enchantCount += enchantIncreaseCount;
			if (TutorialManager.isTutorial && weaponFromInventory3.weapontype == PriestWeaponType.PriestWeapon1 && weaponFromInventory3.enchantCount >= 9)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType22);
			}
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			ArcherWeaponInventoryData weaponFromInventory = getWeaponFromInventory(targetWeaponSlot.currentArcherWeaponType);
			weaponFromInventory.enchantCount += enchantIncreaseCount;
			if (TutorialManager.isTutorial && weaponFromInventory.weapontype == ArcherWeaponType.ArcherWeapon1 && weaponFromInventory.enchantCount >= 9)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType28);
			}
			break;
		}
		}
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.EnchantingHowTo, enchantIncreaseCount);
		Singleton<DataManager>.instance.saveData();
	}

	public double getWeaponUpgradeGoldPrice(int weaponTier, long weaponEnchantCount)
	{
		double num = 0.0;
		double weaponGoldPriceForUnlock = getWeaponGoldPriceForUnlock(weaponTier);
		num = weaponGoldPriceForUnlock / 5.0 * Math.Pow(1.0845, (int)weaponEnchantCount + 1);
		return num - num / 100.0 * Singleton<StatManager>.instance.weaponUpgradeDiscountPercent;
	}
}
