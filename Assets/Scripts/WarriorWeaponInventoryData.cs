using System;

[Serializable]
public class WarriorWeaponInventoryData
{
	public WeaponManager.WarriorWeaponType weapontype;

	public long enchantCount;

	public bool isHaving;

	public bool isUnlockedFromSlot;

	public WarriorWeaponInventoryData(WeaponManager.WarriorWeaponType weapontype, long enchantCount, bool isHaving, bool isUnlockedFromSlot)
	{
		this.weapontype = weapontype;
		this.enchantCount = enchantCount;
		this.isHaving = isHaving;
		this.isUnlockedFromSlot = isUnlockedFromSlot;
	}
}
