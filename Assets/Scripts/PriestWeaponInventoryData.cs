using System;

[Serializable]
public class PriestWeaponInventoryData
{
	public WeaponManager.PriestWeaponType weapontype;

	public long enchantCount;

	public bool isHaving;

	public bool isUnlockedFromSlot;

	public PriestWeaponInventoryData(WeaponManager.PriestWeaponType weapontype, long enchantCount, bool isHaving, bool isUnlockedFromSlot)
	{
		this.weapontype = weapontype;
		this.enchantCount = enchantCount;
		this.isHaving = isHaving;
		this.isUnlockedFromSlot = isUnlockedFromSlot;
	}
}
