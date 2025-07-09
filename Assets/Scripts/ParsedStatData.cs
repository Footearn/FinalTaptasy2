using System;
using System.Collections.Generic;

[Serializable]
public class ParsedStatData
{
	public Dictionary<string, Dictionary<I18NManager.Language, string>> languageData;

	public Dictionary<CharacterManager.CharacterType, CharacterStatData> characterStatData;

	public Dictionary<CharacterSkinManager.WarriorSkinType, CharacterSkinStatData> warriorCharacterSkinData;

	public Dictionary<CharacterSkinManager.PriestSkinType, CharacterSkinStatData> priestCharacterSkinData;

	public Dictionary<CharacterSkinManager.ArcherSkinType, CharacterSkinStatData> archerCharacterSkinData;

	public Dictionary<EnemyManager.MonsterType, MonsterStatData> monsterStatData;

	public Dictionary<EnemyManager.SpecialType, SpecialMonsterStatData> specialMonsterStatData;

	public Dictionary<EnemyManager.BossType, BossStatData> bossStatData;

	public Dictionary<TreasureManager.TreasureType, TreasureStatData> treasureStatData;

	public Dictionary<SkillManager.SkillType, ActiveSkillStatData> activeSkillStatData;

	public Dictionary<AchievementManager.AchievementType, AchievementStatData> achievementStatData;

	public Dictionary<int, double> jackpotChanceData;

	public Dictionary<ColleagueManager.ColleagueType, Dictionary<ColleagueManager.PassiveSkillTierType, ColleaguePassiveSkillData>> colleagueEffectData;

	public Dictionary<ColleagueManager.PassiveSkillTierType, long> colleaguePassiveUnlockLevelData;

	public Dictionary<ColleagueManager.ColleagueType, List<double>> colleagueSkinStatData;

	public VariableSaveData balanceData;

	public Dictionary<WeaponSkinManager.WeaponSkinAbilityType, WeaponSkinAbilityStatData> weaponSkinAbilityStatData;
}
