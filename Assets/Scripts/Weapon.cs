using UnityEngine;

public class Weapon : ObjectBase
{
	public CharacterManager.CharacterType weaponCharacterType;

	public WeaponManager.WarriorWeaponType warriorWeaponType;

	public WeaponManager.PriestWeaponType priestWeaponType;

	public WeaponManager.ArcherWeaponType archerWeaponType;

	public WeaponStat weaponRealStats;

	public SpriteRenderer cachedSpriteRenderer;

	public string baseWeaponAnimationTypeName;

	private WeaponSkinAuraObject m_currentAuraObject;

	public void initWeapon(bool isIngameWeapon)
	{
		if (isIngameWeapon)
		{
			cachedSpriteRenderer.sortingLayerName = "Player";
			cachedSpriteRenderer.sortingOrder = 0;
		}
		else
		{
			cachedSpriteRenderer.sortingLayerName = "PopUpLayer";
			cachedSpriteRenderer.sortingOrder = 15;
		}
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			base.cachedGameObject.layer = LayerMask.NameToLayer("WarriorLayer");
			cachedSpriteRenderer.gameObject.layer = LayerMask.NameToLayer("WarriorLayer");
			break;
		case CharacterManager.CharacterType.Priest:
			base.cachedGameObject.layer = LayerMask.NameToLayer("PriestLayer");
			cachedSpriteRenderer.gameObject.layer = LayerMask.NameToLayer("PriestLayer");
			break;
		case CharacterManager.CharacterType.Archer:
			base.cachedGameObject.layer = LayerMask.NameToLayer("ArcherLayer");
			cachedSpriteRenderer.gameObject.layer = LayerMask.NameToLayer("ArcherLayer");
			break;
		}
		refreshWeaponSkin();
	}

	public void refreshWeaponStats()
	{
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			weaponRealStats = Singleton<WeaponManager>.instance.getWeaponStat(warriorWeaponType, Singleton<WeaponManager>.instance.getWeaponFromInventory(warriorWeaponType).enchantCount);
			break;
		case CharacterManager.CharacterType.Priest:
			weaponRealStats = Singleton<WeaponManager>.instance.getWeaponStat(priestWeaponType, Singleton<WeaponManager>.instance.getWeaponFromInventory(priestWeaponType).enchantCount);
			break;
		case CharacterManager.CharacterType.Archer:
			weaponRealStats = Singleton<WeaponManager>.instance.getWeaponStat(archerWeaponType, Singleton<WeaponManager>.instance.getWeaponFromInventory(archerWeaponType).enchantCount);
			break;
		}
	}

	public void refreshWeaponSkin()
	{
		WeaponSkinData equippedWeaponSkinData = Singleton<WeaponSkinManager>.instance.getEquippedWeaponSkinData(weaponCharacterType);
		if (equippedWeaponSkinData != null)
		{
			if (m_currentAuraObject == null)
			{
				m_currentAuraObject = ObjectPool.Spawn("@WeaponSkinAuraObject", Vector3.zero, base.cachedTransform).GetComponent<WeaponSkinAuraObject>();
			}
			m_currentAuraObject.initAuraObject(weaponCharacterType);
			bool flag = equippedWeaponSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || equippedWeaponSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || equippedWeaponSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
			switch (equippedWeaponSkinData.currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				cachedSpriteRenderer.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(equippedWeaponSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(equippedWeaponSkinData.currentWarriorWeaponType));
				break;
			case CharacterManager.CharacterType.Priest:
				cachedSpriteRenderer.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(equippedWeaponSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(equippedWeaponSkinData.currentPriestWeaponType));
				break;
			case CharacterManager.CharacterType.Archer:
				cachedSpriteRenderer.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(equippedWeaponSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(equippedWeaponSkinData.currentArcherWeaponType));
				break;
			}
		}
		else
		{
			if (m_currentAuraObject != null)
			{
				ObjectPool.Recycle(m_currentAuraObject.name, m_currentAuraObject.cachedGameObject);
				m_currentAuraObject = null;
			}
			switch (weaponCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				cachedSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(warriorWeaponType);
				break;
			case CharacterManager.CharacterType.Priest:
				cachedSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(priestWeaponType);
				break;
			case CharacterManager.CharacterType.Archer:
				cachedSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(archerWeaponType);
				break;
			}
		}
	}
}
