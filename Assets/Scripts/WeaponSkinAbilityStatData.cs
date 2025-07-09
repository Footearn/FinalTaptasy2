using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public class WeaponSkinAbilityStatData
{
	public WeaponSkinManager.WeaponSkinAbilityType currentAbilityType;

	public WeaponSkinManager.WeaponSkinGradeType abilityAppearGradeType;

	public Dictionary<WeaponSkinManager.WeaponSkinGradeType, ObscuredDouble> statDictionary;
}
