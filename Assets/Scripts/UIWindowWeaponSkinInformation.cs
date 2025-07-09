using UnityEngine;
using UnityEngine.UI;

public class UIWindowWeaponSkinInformation : UIWindow
{
	public static UIWindowWeaponSkinInformation instance;

	public Image weaponIconImage;

	public RectTransform weaponSkinIconRectTransform;

	public Image gradeIconImage;

	public Image backgroundImage;

	public Text weaponSkinNameText;

	public Text weaponSkinCharacterTypeText;

	public Text[] abilityTexts;

	public RectTransform[] abilityTextRectTransforms;

	public RectTransform abilityTextContents;

	public GameObject equipButtonObject;

	public GameObject unequipButtonObject;

	public Image characterTypeIconImage;

	private WeaponSkinData m_targetWeaponSkinData;

	public WeaponSkinData targetWeaponSkinData
	{
		get
		{
			return m_targetWeaponSkinData;
		}
	}

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openWeaponSkinInformation(WeaponSkinData targetSkinData)
	{
		m_targetWeaponSkinData = targetSkinData;
		if (m_targetWeaponSkinData == null)
		{
			return;
		}
		refreshEquipButton();
		weaponSkinIconRectTransform.localEulerAngles = Singleton<WeaponSkinManager>.instance.getWeaponIconRotation(m_targetWeaponSkinData.currentCharacterType);
		gradeIconImage.sprite = Singleton<WeaponSkinManager>.instance.getGradeIconSprite(m_targetWeaponSkinData.currentGrade);
		backgroundImage.sprite = Singleton<WeaponSkinManager>.instance.getGradeBackgroundSprite(m_targetWeaponSkinData.currentGrade);
		bool flag = m_targetWeaponSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || m_targetWeaponSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || m_targetWeaponSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
		switch (m_targetWeaponSkinData.currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			characterTypeIconImage.sprite = Singleton<CachedManager>.instance.warriorIconSprite;
			weaponIconImage.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetWeaponSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetWeaponSkinData.currentWarriorWeaponType));
			weaponSkinNameText.text = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinName(m_targetWeaponSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponI18NName(m_targetWeaponSkinData.currentWarriorWeaponType));
			weaponSkinCharacterTypeText.text = Singleton<WeaponSkinManager>.instance.getGradeString(m_targetWeaponSkinData.currentGrade) + " <color=#FDFCB7FF>(" + I18NManager.Get("WARRIOR") + ")</color>";
			break;
		case CharacterManager.CharacterType.Priest:
			characterTypeIconImage.sprite = Singleton<CachedManager>.instance.preistIconSprite;
			weaponIconImage.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetWeaponSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetWeaponSkinData.currentPriestWeaponType));
			weaponSkinNameText.text = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinName(m_targetWeaponSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponI18NName(m_targetWeaponSkinData.currentPriestWeaponType));
			weaponSkinCharacterTypeText.text = Singleton<WeaponSkinManager>.instance.getGradeString(m_targetWeaponSkinData.currentGrade) + " <color=#FDFCB7FF>(" + I18NManager.Get("PRIEST") + ")</color>";
			break;
		case CharacterManager.CharacterType.Archer:
			characterTypeIconImage.sprite = Singleton<CachedManager>.instance.archerIconSprite;
			weaponIconImage.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetWeaponSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetWeaponSkinData.currentArcherWeaponType));
			weaponSkinNameText.text = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinName(m_targetWeaponSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponI18NName(m_targetWeaponSkinData.currentArcherWeaponType));
			weaponSkinCharacterTypeText.text = Singleton<WeaponSkinManager>.instance.getGradeString(m_targetWeaponSkinData.currentGrade) + " <color=#FDFCB7FF>(" + I18NManager.Get("ARCHER") + ")</color>";
			break;
		}
		characterTypeIconImage.SetNativeSize();
		gradeIconImage.SetNativeSize();
		weaponIconImage.SetNativeSize();
		open();
		float num = -9.3f;
		float num2 = 0f - num;
		for (int i = 0; i < abilityTexts.Length; i++)
		{
			abilityTexts[i].text = string.Empty;
		}
		abilityTextRectTransforms[0].anchoredPosition = new Vector2(0f, num);
		for (int j = 0; j < m_targetWeaponSkinData.abilityDataList.Count; j++)
		{
			abilityTexts[j].text = "- " + Singleton<WeaponSkinManager>.instance.getAbilityString(m_targetWeaponSkinData.abilityDataList[j]);
			if (j >= 1)
			{
				float num3 = abilityTextRectTransforms[j - 1].anchoredPosition.y - abilityTexts[j - 1].preferredHeight;
				abilityTextRectTransforms[j].anchoredPosition = new Vector2(0f, num3);
				num2 = Mathf.Abs(num3) + abilityTexts[j].preferredHeight;
			}
		}
		abilityTextContents.sizeDelta = new Vector2(abilityTextContents.rect.width, num2 + 10f);
	}

	private void refreshEquipButton()
	{
		if (m_targetWeaponSkinData != null)
		{
			equipButtonObject.SetActive(!m_targetWeaponSkinData.isEquipped);
			unequipButtonObject.SetActive(m_targetWeaponSkinData.isEquipped);
		}
	}

	public void OnClickUseCube()
	{
		UIWindowWeaponSkinCube.instance.openWeaponSkinCube(m_targetWeaponSkinData);
	}

	public void OnClickEquip()
	{
		if (m_targetWeaponSkinData != null)
		{
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			Singleton<WeaponSkinManager>.instance.equipWeaponSkin(m_targetWeaponSkinData);
			refreshEquipButton();
		}
	}

	public void OnClickUnequip()
	{
		if (m_targetWeaponSkinData != null)
		{
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			Singleton<WeaponSkinManager>.instance.unequipWeaponSkin(m_targetWeaponSkinData);
			refreshEquipButton();
		}
	}

	public void OnClickOpenChangeWeaponSkinUI()
	{
		if (m_targetWeaponSkinData != null)
		{
			UIWindowWeaponSkinChangeSkin.instance.openWeaponSkinChangeUI(m_targetWeaponSkinData);
		}
	}

	public void OnClickDestroyWeaponSkin()
	{
		if (m_targetWeaponSkinData != null)
		{
			UIWindowDestroyWeaponSkin.instance.openDestroyWeaponSkin(m_targetWeaponSkinData);
		}
	}
}
