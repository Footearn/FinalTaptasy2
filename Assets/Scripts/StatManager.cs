using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class StatManager : Singleton<StatManager>
{
	public enum WeaponStatType
	{
		FixedDamage,
		FixedHealth,
		CriticalChance,
		TapHeal,
		Length
	}

	public double percentGoldGain;

	public double allPercentDamage;

	public double allPercentHealthFromTreasure;

	public double allCriticalDamage;

	public double allPercentDamageFromTreasureChest;

	public double allCharacterAndColleaguePercentDamageFromPrincess;

	public float allCriticalChanceFromTreasure;

	public double allPercentDamageFromTreasure;

	public double[] colleagueSelfPercentDamage;

	public double allPercentDamageFromColleague;

	public double warriorPercentDamageFromColleague;

	public double warriorPercentHealthFromColleague;

	public float warriorCriticalChanceFromColleague;

	public double warriorCriticalDamageFromColleague;

	public double priestPercentDamageFromColleague;

	public double priestPercentTapHealFromColleague;

	public float priestCriticalChanceFromColleague;

	public double priestCriticalDamageFromColleague;

	public double archerPercentDamageFromColleague;

	public float archerCriticalChanceFromColleague;

	public double archerCriticalDamageFromColleague;

	public double colleagueAllPercentDamageFromColleagueSkin;

	public double allPercentDamageByConcentraion;

	public double colleagueLevelUpDiscountPercent;

	public double weaponUpgradeDiscountPercent;

	public float treasureChestDropExtraPercentChance;

	public double questRewardDoubleChance;

	public double revivePercentHealth;

	public double skillExtraPercentDamage;

	public double colleaguePassiveSkillExtraValue;

	public float manaGatherExtraValue;

	public double percentDamageAM;

	public double percentDamagePM;

	public float rubyDropExtraPercentChance;

	public int startManaCount;

	public float allPercentAttackDamageWhenDestroyTreasureChest;

	public double evasionPercentExtraChance;

	public double extraPercentDamageWhenHitBosses;

	public double healValueEveryTap;

	public double decreaseDamageFromHitBoss;

	public int maxExtraMana;

	public double percentJackPotRewardGain;

	public double characterSkinEffectBonus;

	public double extraPercentDamageWhenHitNormalMonsters;

	public double extraPercentTapHealValueFromTreasure;

	public double bossGoldGain;

	public float percentMovementSpeed;

	public double extraPercentDamageFromConquerorRing;

	public double extraPercentDamageFromNobleBlade;

	public double transcendDecreaseHitDamage;

	public double transcendIncreaseAllDamage;

	public double warriorSkinPercentDamage;

	public double warriorSkinHealthPercent;

	public double priestSkinPercentDamage;

	public double archerSkinPercentDamage;

	public float archerSkinCriticalChance;

	public double confusionValueFromReinforcementCloneWarrior;

	public float decreaseDamageFromReinforcementConcentration;

	public double allColleaguePercentDamageFromAngelinaColleague;

	public double specialAdsAngelDamage;

	public double specialAdsAngelHealth;

	public double reinforcementDivneSmashExtraDamageFromPremiumTreasure;

	public double reinforcementWhirlWindExtraHitChanceFromPremiumTreasure;

	public double reinforcementConcentrationExtraDamageFromPremiumTreasure;

	public double reinforcementCloneWarriorExtraDamageFromPremiumTreasure;

	public double reinforcementDragonBreathExtraDamageFromPremiumTreasure;

	public double meteorRainExtraSpawnCountFromPremiumTreasure;

	public double frostSkillExtraDurationFromPremiumTreasure;

	public double percentDamageFromPremiumTreasure;

	public double percentHealthFromPremiumTreasure;

	public double meteorRainExtraCastChanceFromPremiumTreasure;

	public double frostWallExtraCastChanceFromPremiumTreasure;

	public double reinforcementSkillExtraCastChanceFromPremiumTreasure;

	public double reinforcementConfusionExtraFloor;

	public double dragonBreathExtraAttackCount;

	public double frostWallExtraFloor;

	public float concentrationExtraDurationTimeFromPremiumTreasure;

	public double chanceForMultipliedTapHealFromPremiumTreasure;

	public double warriorPercentDamageFromPremiumTreasure;

	public double priestPercentDamageFromPremiumTreasure;

	public double archerPercentDamageFromPremiumTreasure;

	public double allHPAndTapHealFromTowerModeTreasure;

	public double allCharacterPercentDamageFromTowerModeTreasure;

	public double weaponSkinPercentDamageForWarrior;

	public double weaponSkinPercentDamageForPriest;

	public double weaponSkinPercentDamageForArcher;

	public double weaponSkinTotalPercentHealth;

	public double weaponSkinCriticalDamageForWarrior;

	public double weaponSkinCriticalDamageForPriest;

	public double weaponSkinCriticalDamageForArcher;

	public double weaponSkinMovementSpeed;

	public double weaponSkinSkillDamage;

	public double weaponSkinTapHeal;

	public double weaponSkinArmor;

	public double weaponSkinEvadeChance;

	public double weaponSkinCriticalChanceForWarrior;

	public double weaponSkinCriticalChanceForPriest;

	public double weaponSkinCriticalChanceForArcher;

	public double weaponSkinGoldGain;

	public ObscuredDouble weaponSkinTowerModeEvadeChance = 0.0;

	public double weaponSkinCollectEventItemDropChance;

	public double weaponSkinExtraCriticalDamageForWarrior;

	public double weaponSkinExtraCriticalDamageForPriest;

	public double weaponSkinExtraCriticalDamageForArcher;

	public double weaponSkinNewAttackDamageForWarrior;

	public double weaponSkinNewAttackDamageForPriest;

	public double weaponSkinNewAttackDamageForArcher;

	public double weaponSkinStackAttackDamageForWarrior;

	public double weaponSkinStackAttackDamageForPriest;

	public double weaponSkinStackAttackDamageForArcher;

	public double weaponSkinInvinciblePerson;

	public double weaponSkinReinforcementMana;

	public double weaponSkinTapHealthFromReinforcementMana;

	public double weaponSkinArmorFromReinforcementMana;

	public double weaponSkinDamageFromReinforcementMana;

	public double weaponSkinConcentrationAutoCastChance;

	public double weaponSkinCloneWarriorAutoCastChance;

	public double weaponSkinDragonBreathAutoCastChance;

	public double weaponSkinLegendaryChestDropChance;

	public void startGame()
	{
		refreshAllStats();
	}

	public void refreshAllStats()
	{
		allPercentDamageFromTreasure = 0.0;
		colleagueLevelUpDiscountPercent = 0.0;
		percentGoldGain = 0.0;
		confusionValueFromReinforcementCloneWarrior = 0.0;
		allPercentDamageFromTreasureChest = 0.0;
		allPercentDamage = 0.0;
		allPercentHealthFromTreasure = 0.0;
		allCriticalDamage = 0.0;
		allCriticalChanceFromTreasure = 0f;
		weaponUpgradeDiscountPercent = 0.0;
		maxExtraMana = 0;
		colleaguePassiveSkillExtraValue = 0.0;
		allPercentDamageByConcentraion = 0.0;
		treasureChestDropExtraPercentChance = 0f;
		manaGatherExtraValue = 0f;
		questRewardDoubleChance = 0.0;
		revivePercentHealth = 0.0;
		colleagueAllPercentDamageFromColleagueSkin = 0.0;
		skillExtraPercentDamage = 0.0;
		rubyDropExtraPercentChance = 0f;
		startManaCount = 0;
		allPercentAttackDamageWhenDestroyTreasureChest = 0f;
		evasionPercentExtraChance = 0.0;
		extraPercentDamageWhenHitBosses = 0.0;
		healValueEveryTap = 0.0;
		percentJackPotRewardGain = 0.0;
		decreaseDamageFromHitBoss = 0.0;
		extraPercentDamageFromConquerorRing = 0.0;
		extraPercentDamageFromNobleBlade = 0.0;
		characterSkinEffectBonus = 0.0;
		extraPercentDamageWhenHitNormalMonsters = 0.0;
		percentDamageAM = 0.0;
		percentDamagePM = 0.0;
		extraPercentTapHealValueFromTreasure = 0.0;
		bossGoldGain = 0.0;
		percentMovementSpeed = 0f;
		allColleaguePercentDamageFromAngelinaColleague = 0.0;
		warriorSkinPercentDamage = 0.0;
		warriorSkinHealthPercent = 0.0;
		priestSkinPercentDamage = 0.0;
		archerSkinPercentDamage = 0.0;
		archerSkinCriticalChance = 0f;
		allPercentDamageFromColleague = 0.0;
		warriorPercentDamageFromColleague = 0.0;
		warriorPercentHealthFromColleague = 0.0;
		warriorCriticalChanceFromColleague = 0f;
		warriorCriticalDamageFromColleague = 0.0;
		priestPercentDamageFromColleague = 0.0;
		priestPercentTapHealFromColleague = 0.0;
		priestCriticalChanceFromColleague = 0f;
		priestCriticalDamageFromColleague = 0.0;
		archerPercentDamageFromColleague = 0.0;
		archerCriticalChanceFromColleague = 0f;
		archerCriticalDamageFromColleague = 0.0;
		reinforcementDivneSmashExtraDamageFromPremiumTreasure = 0.0;
		reinforcementWhirlWindExtraHitChanceFromPremiumTreasure = 0.0;
		reinforcementConcentrationExtraDamageFromPremiumTreasure = 0.0;
		reinforcementCloneWarriorExtraDamageFromPremiumTreasure = 0.0;
		reinforcementDragonBreathExtraDamageFromPremiumTreasure = 0.0;
		reinforcementSkillExtraCastChanceFromPremiumTreasure = 0.0;
		meteorRainExtraSpawnCountFromPremiumTreasure = 0.0;
		frostSkillExtraDurationFromPremiumTreasure = 0.0;
		reinforcementConfusionExtraFloor = 0.0;
		dragonBreathExtraAttackCount = 0.0;
		frostWallExtraFloor = 0.0;
		percentDamageFromPremiumTreasure = 0.0;
		percentHealthFromPremiumTreasure = 0.0;
		meteorRainExtraCastChanceFromPremiumTreasure = 0.0;
		frostWallExtraCastChanceFromPremiumTreasure = 0.0;
		concentrationExtraDurationTimeFromPremiumTreasure = 0f;
		chanceForMultipliedTapHealFromPremiumTreasure = 0.0;
		warriorPercentDamageFromPremiumTreasure = 0.0;
		priestPercentDamageFromPremiumTreasure = 0.0;
		archerPercentDamageFromPremiumTreasure = 0.0;
		decreaseDamageFromReinforcementConcentration = 0f;
		allHPAndTapHealFromTowerModeTreasure = 0.0;
		allCharacterPercentDamageFromTowerModeTreasure = 0.0;
		weaponSkinPercentDamageForWarrior = 0.0;
		weaponSkinPercentDamageForPriest = 0.0;
		weaponSkinPercentDamageForArcher = 0.0;
		weaponSkinTotalPercentHealth = 0.0;
		weaponSkinCriticalDamageForWarrior = 0.0;
		weaponSkinCriticalDamageForPriest = 0.0;
		weaponSkinCriticalDamageForArcher = 0.0;
		weaponSkinMovementSpeed = 0.0;
		weaponSkinSkillDamage = 0.0;
		weaponSkinTapHeal = 0.0;
		weaponSkinArmor = 0.0;
		weaponSkinEvadeChance = 0.0;
		weaponSkinCriticalChanceForWarrior = 0.0;
		weaponSkinCriticalChanceForPriest = 0.0;
		weaponSkinCriticalChanceForArcher = 0.0;
		weaponSkinGoldGain = 0.0;
		weaponSkinTowerModeEvadeChance = 0.0;
		weaponSkinCollectEventItemDropChance = 0.0;
		weaponSkinExtraCriticalDamageForWarrior = 0.0;
		weaponSkinExtraCriticalDamageForPriest = 0.0;
		weaponSkinExtraCriticalDamageForArcher = 0.0;
		weaponSkinNewAttackDamageForWarrior = 0.0;
		weaponSkinNewAttackDamageForPriest = 0.0;
		weaponSkinNewAttackDamageForArcher = 0.0;
		weaponSkinStackAttackDamageForWarrior = 0.0;
		weaponSkinStackAttackDamageForPriest = 0.0;
		weaponSkinStackAttackDamageForArcher = 0.0;
		weaponSkinInvinciblePerson = 0.0;
		weaponSkinReinforcementMana = 0.0;
		weaponSkinArmorFromReinforcementMana = 0.0;
		weaponSkinTapHealthFromReinforcementMana = 0.0;
		weaponSkinDamageFromReinforcementMana = 0.0;
		weaponSkinConcentrationAutoCastChance = 0.0;
		weaponSkinCloneWarriorAutoCastChance = 0.0;
		weaponSkinDragonBreathAutoCastChance = 0.0;
		weaponSkinLegendaryChestDropChance = 0.0;
		allCharacterAndColleaguePercentDamageFromPrincess = Mathf.Max(GameManager.getCurrentPrincessNumber() - 1, 0) * 100;
		if (Singleton<TreasureManager>.instance != null)
		{
			Singleton<TreasureManager>.instance.calculateAllStatOfTreasure();
		}
		Singleton<WeaponSkinManager>.instance.calculateAllWeaponSkinStat();
		if (Singleton<ColleagueManager>.instance != null)
		{
			Singleton<ColleagueManager>.instance.calculateAllColleagueSkinStat();
			Singleton<ColleagueManager>.instance.refreshAllColleaguePassiveSkillStats();
		}
		Singleton<AdsAngelManager>.instance.refreshSpecialAdsAngelStat();
		if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(ColleagueManager.ColleagueType.Angelina).isUnlocked)
		{
			allColleaguePercentDamageFromAngelinaColleague += Singleton<ColleagueManager>.instance.getTotalColleagueSkinCount() * 10;
		}
		if (Singleton<CharacterSkinManager>.instance != null)
		{
			warriorSkinPercentDamage += Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType.Warrior);
			warriorSkinHealthPercent += Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinSecondStatValue(CharacterManager.CharacterType.Warrior);
			priestSkinPercentDamage += Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType.Priest);
			archerSkinPercentDamage += Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinPercentDamageValue(CharacterManager.CharacterType.Archer);
			archerSkinCriticalChance += Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinSecondStatValue(CharacterManager.CharacterType.Archer);
		}
		Singleton<SkillManager>.instance.refreshSkillBaseData();
		if (GameManager.currentDungeonType != GameManager.SpecialDungeonType.ElopeMode && Singleton<CharacterManager>.instance.constCharacterList != null)
		{
			for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
			{
				if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
				{
					Singleton<CharacterManager>.instance.constCharacterList[i].resetProperties(false);
					if (Singleton<CharacterManager>.instance.constCharacterList[i].equippedWeapon.weaponRealStats.secondStatType == WeaponStatType.TapHeal)
					{
						healValueEveryTap += Singleton<CharacterManager>.instance.constCharacterList[i].equippedWeapon.weaponRealStats.secondStatValue;
					}
				}
			}
		}
		healValueEveryTap += healValueEveryTap / 100.0 * (extraPercentTapHealValueFromTreasure + (double)Singleton<CharacterSkinManager>.instance.getTotalCharacterSkinSecondStatValue(CharacterManager.CharacterType.Priest) + priestPercentTapHealFromColleague);
		healValueEveryTap += healValueEveryTap / 100.0 * allHPAndTapHealFromTowerModeTreasure;
		healValueEveryTap += healValueEveryTap / 100.0 * weaponSkinTapHeal;
	}
}
