using System;

[Serializable]
public struct ObtainData
{
	public int pieceCount;

	public CharacterManager.CharacterType weaponCharacterType;

	public WeaponManager.WarriorWeaponType warriorWeaponType;

	public WeaponManager.PriestWeaponType priestWeaponType;

	public WeaponManager.ArcherWeaponType archerWeaponType;
}
