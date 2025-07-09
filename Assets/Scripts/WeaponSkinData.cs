using System;
using System.Collections.Generic;

[Serializable]
public class WeaponSkinData
{
	public CharacterManager.CharacterType currentCharacterType = CharacterManager.CharacterType.Length;

	public WeaponManager.WarriorWeaponType currentWarriorWeaponType;

	public WeaponSkinManager.WarriorSpecialWeaponSkinType currentWarriorSpecialWeaponSkinType = WeaponSkinManager.WarriorSpecialWeaponSkinType.None;

	public WeaponManager.PriestWeaponType currentPriestWeaponType;

	public WeaponSkinManager.PriestSpecialWeaponSkinType currentPriestSpecialWeaponSkinType = WeaponSkinManager.PriestSpecialWeaponSkinType.None;

	public WeaponManager.ArcherWeaponType currentArcherWeaponType;

	public WeaponSkinManager.ArcherSpecialWeaponSkinType currentArcherSpecialWeaponSkinType = WeaponSkinManager.ArcherSpecialWeaponSkinType.None;

	public WeaponSkinManager.WeaponSkinGradeType currentGrade = WeaponSkinManager.WeaponSkinGradeType.None;

	public List<WeaponSkinAbilityData> abilityDataList;

	public bool isEquipped;
}
