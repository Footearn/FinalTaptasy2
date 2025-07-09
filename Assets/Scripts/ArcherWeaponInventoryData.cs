using System;

[Serializable]
public class ArcherWeaponInventoryData
{
	public WeaponManager.ArcherWeaponType weapontype;

	public long enchantCount;

	public bool isHaving;

	public bool isUnlockedFromSlot;

	public ArcherWeaponInventoryData(WeaponManager.ArcherWeaponType weapontype, long enchantCount, bool isHaving, bool isUnlockedFromSlot)
	{
		this.weapontype = weapontype;
		this.enchantCount = enchantCount;
		this.isHaving = isHaving;
		this.isUnlockedFromSlot = isUnlockedFromSlot;
	}
}
