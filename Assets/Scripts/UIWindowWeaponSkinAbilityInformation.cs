using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.UI;

public class UIWindowWeaponSkinAbilityInformation : UIWindow
{
	public static UIWindowWeaponSkinAbilityInformation instance;

	public Text abilityText;

	private Dictionary<WeaponSkinManager.WeaponSkinGradeType, List<WeaponSkinManager.WeaponSkinAbilityType>> m_abilityDictionary;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	private void resetAbilityDictionary()
	{
		m_abilityDictionary = new Dictionary<WeaponSkinManager.WeaponSkinGradeType, List<WeaponSkinManager.WeaponSkinAbilityType>>();
		for (int i = 0; i < 21; i++)
		{
			WeaponSkinManager.WeaponSkinAbilityType weaponSkinAbilityType = (WeaponSkinManager.WeaponSkinAbilityType)i;
			WeaponSkinManager.WeaponSkinGradeType abilityAppearGradeType = Singleton<ParsingManager>.instance.currentParsedStatData.weaponSkinAbilityStatData[weaponSkinAbilityType].abilityAppearGradeType;
			if (!m_abilityDictionary.ContainsKey(abilityAppearGradeType))
			{
				m_abilityDictionary.Add(abilityAppearGradeType, new List<WeaponSkinManager.WeaponSkinAbilityType>());
			}
			m_abilityDictionary[abilityAppearGradeType].Add(weaponSkinAbilityType);
		}
	}

	public void openWeaponSkinInformation()
	{
		if (m_abilityDictionary == null)
		{
			resetAbilityDictionary();
		}
		string text = string.Empty;
		for (int i = 0; i < 4; i++)
		{
			WeaponSkinManager.WeaponSkinGradeType weaponSkinGradeType = (WeaponSkinManager.WeaponSkinGradeType)i;
			switch (weaponSkinGradeType)
			{
			case WeaponSkinManager.WeaponSkinGradeType.Rare:
				text = text + "<size=27><color=#B3F2FF>" + I18NManager.Get("RARE") + "</color></size>\n";
				break;
			case WeaponSkinManager.WeaponSkinGradeType.Epic:
				text = text + "<size=27><color=#118DBF>" + I18NManager.Get("EPIC") + "</color></size>\n";
				break;
			case WeaponSkinManager.WeaponSkinGradeType.Unique:
				text = text + "<size=27><color=#FAD725>" + I18NManager.Get("UNIQUE") + "</color></size>\n";
				break;
			case WeaponSkinManager.WeaponSkinGradeType.Legendary:
				text = text + "<size=27><color=red>" + I18NManager.Get("LEGENDARY") + "</color></size>\n";
				break;
			}
			List<WeaponSkinManager.WeaponSkinAbilityType> list = m_abilityDictionary[weaponSkinGradeType];
			for (int j = 0; j < list.Count; j++)
			{
				Dictionary<WeaponSkinManager.WeaponSkinGradeType, ObscuredDouble> statDictionary = Singleton<ParsingManager>.instance.currentParsedStatData.weaponSkinAbilityStatData[list[j]].statDictionary;
				text = text + string.Format(Singleton<WeaponSkinManager>.instance.getAbilityDescription(list[j]), string.Concat(statDictionary[WeaponSkinManager.WeaponSkinGradeType.Rare], "~", statDictionary[WeaponSkinManager.WeaponSkinGradeType.Legendary])) + "\n";
			}
			if (weaponSkinGradeType != WeaponSkinManager.WeaponSkinGradeType.Legendary)
			{
				text += "\n";
			}
		}
		abilityText.text = text;
		open();
	}
}
