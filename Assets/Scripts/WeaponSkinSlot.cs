using UnityEngine;
using UnityEngine.UI;

public class WeaponSkinSlot : ObjectBase
{
	public GameObject noEmptyObject;

	public GameObject emptyObject;

	public Image weaponSkinIcon;

	public RectTransform weaponSkinIconRectTransform;

	public GameObject newIconObject;

	public Image gradeIcon;

	public Image backgroundImage;

	public GameObject equippedIconObject;

	private WeaponSkinData m_currentWeaponSkinData;

	public void initWeaponSkinSlot(WeaponSkinData weaponSkinData)
	{
		m_currentWeaponSkinData = weaponSkinData;
		if (m_currentWeaponSkinData != null)
		{
			if (!noEmptyObject.activeSelf)
			{
				noEmptyObject.SetActive(true);
			}
			if (emptyObject.activeSelf)
			{
				emptyObject.SetActive(false);
			}
			bool flag = m_currentWeaponSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || m_currentWeaponSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || m_currentWeaponSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
			switch (m_currentWeaponSkinData.currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				weaponSkinIcon.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_currentWeaponSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_currentWeaponSkinData.currentWarriorWeaponType));
				break;
			case CharacterManager.CharacterType.Priest:
				weaponSkinIcon.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_currentWeaponSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_currentWeaponSkinData.currentPriestWeaponType));
				break;
			case CharacterManager.CharacterType.Archer:
				weaponSkinIcon.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_currentWeaponSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_currentWeaponSkinData.currentArcherWeaponType));
				break;
			}
			if (UIWindowWeaponSkinChangeSkin.instance.isOpen)
			{
				equippedIconObject.SetActive(m_currentWeaponSkinData == UIWindowWeaponSkinChangeSkin.instance.originSkinData);
			}
			else if (equippedIconObject.activeSelf == !m_currentWeaponSkinData.isEquipped)
			{
				equippedIconObject.SetActive(m_currentWeaponSkinData.isEquipped);
			}
			weaponSkinIconRectTransform.localEulerAngles = Singleton<WeaponSkinManager>.instance.getWeaponIconRotation(m_currentWeaponSkinData.currentCharacterType);
			gradeIcon.sprite = Singleton<WeaponSkinManager>.instance.getGradeIconSprite(m_currentWeaponSkinData.currentGrade);
			backgroundImage.sprite = Singleton<WeaponSkinManager>.instance.getGradeBackgroundSprite(m_currentWeaponSkinData.currentGrade);
			gradeIcon.SetNativeSize();
			weaponSkinIcon.SetNativeSize();
		}
		else
		{
			if (noEmptyObject.activeSelf)
			{
				noEmptyObject.SetActive(false);
			}
			if (!emptyObject.activeSelf)
			{
				emptyObject.SetActive(true);
			}
		}
	}

	public void refreshNewIcon()
	{
		bool flag = UIWindowWeaponSkin.instance.newSkinData == m_currentWeaponSkinData;
		if (newIconObject.activeSelf != flag)
		{
			newIconObject.SetActive(flag);
		}
	}

	public void OnClickWeaponSkinSlot()
	{
		if (m_currentWeaponSkinData == null)
		{
			return;
		}
		if (UIWindowWeaponSkinChangeSkin.instance.isOpen)
		{
			UIWindowWeaponSkinChangeSkin.instance.setTargetWeaponSkin(m_currentWeaponSkinData);
			return;
		}
		if (UIWindowWeaponSkin.instance.newSkinData == m_currentWeaponSkinData)
		{
			UIWindowWeaponSkin.instance.newSkinData = null;
			refreshNewIcon();
		}
		UIWindowWeaponSkinInformation.instance.openWeaponSkinInformation(m_currentWeaponSkinData);
	}
}
