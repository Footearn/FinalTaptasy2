using UnityEngine;

public class WeaponSkinAuraObject : ObjectBase
{
	public GameObject[] characterAuraObjects;

	public GameObject[] warriorAuraObjects;

	public GameObject[] priestAuraObjects;

	public GameObject[] archerAuraObjects;

	public void initAuraObject(CharacterManager.CharacterType characterType)
	{
		base.cachedTransform.localEulerAngles = Vector3.zero;
		base.cachedTransform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
		for (int i = 0; i < characterAuraObjects.Length; i++)
		{
			characterAuraObjects[i].SetActive(false);
		}
		WeaponSkinData equippedWeaponSkinData = Singleton<WeaponSkinManager>.instance.getEquippedWeaponSkinData(characterType);
		if (equippedWeaponSkinData == null)
		{
			ObjectPool.Recycle(base.name, base.cachedGameObject);
			return;
		}
		switch (characterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			characterAuraObjects[0].SetActive(true);
			for (int l = 0; l < warriorAuraObjects.Length; l++)
			{
				warriorAuraObjects[l].SetActive(false);
			}
			warriorAuraObjects[(int)equippedWeaponSkinData.currentGrade].SetActive(true);
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			characterAuraObjects[1].SetActive(true);
			for (int k = 0; k < priestAuraObjects.Length; k++)
			{
				priestAuraObjects[k].SetActive(false);
			}
			priestAuraObjects[(int)equippedWeaponSkinData.currentGrade].SetActive(true);
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			characterAuraObjects[2].SetActive(true);
			for (int j = 0; j < archerAuraObjects.Length; j++)
			{
				archerAuraObjects[j].SetActive(false);
			}
			archerAuraObjects[(int)equippedWeaponSkinData.currentGrade].SetActive(true);
			break;
		}
		}
	}
}
