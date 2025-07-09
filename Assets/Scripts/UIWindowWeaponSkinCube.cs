using UnityEngine;
using UnityEngine.UI;

public class UIWindowWeaponSkinCube : UIWindow
{
	public static UIWindowWeaponSkinCube instance;

	public Image weaponSkinIconImage;

	public Text masterPiecePriceText;

	public Text[] priceTexts;

	public RectTransform weaponSkinIconRectTransform;

	private WeaponSkinData m_targetSkinData;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openWeaponSkinCube(WeaponSkinData skinData)
	{
		Singleton<WeaponSkinManager>.instance.displayWeaponReinforcementMasterPiece();
		m_targetSkinData = skinData;
		if (m_targetSkinData != null)
		{
			for (int i = 0; i < priceTexts.Length; i++)
			{
				priceTexts[i].text = WeaponSkinManager.weaponSkinCubePrice.ToString("N0");
			}
			bool flag = m_targetSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || m_targetSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || m_targetSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
			switch (m_targetSkinData.currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				weaponSkinIconImage.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetSkinData.currentWarriorWeaponType));
				break;
			case CharacterManager.CharacterType.Priest:
				weaponSkinIconImage.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetSkinData.currentPriestWeaponType));
				break;
			case CharacterManager.CharacterType.Archer:
				weaponSkinIconImage.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetSkinData.currentArcherWeaponType));
				break;
			}
			masterPiecePriceText.text = Singleton<WeaponSkinManager>.instance.getMasterPiecePrice(m_targetSkinData.currentGrade).ToString("N0");
			weaponSkinIconRectTransform.localEulerAngles = Singleton<WeaponSkinManager>.instance.getWeaponIconRotation(m_targetSkinData.currentCharacterType);
			weaponSkinIconImage.SetNativeSize();
			open();
		}
	}

	public void OnClickUseNormalCube()
	{
		if (Singleton<DataManager>.instance.currentGameData._ruby >= (long)WeaponSkinManager.weaponSkinCubePrice)
		{
			Singleton<RubyManager>.instance.decreaseRuby((long)WeaponSkinManager.weaponSkinCubePrice);
			Singleton<WeaponSkinManager>.instance.getRandomWeaponSkinData(false, false, m_targetSkinData);
			UIWindowWeaponSkin.instance.openWeaponSkin(m_targetSkinData.currentCharacterType);
			UIWindowWeaponSkinInformation.instance.openWeaponSkinInformation(m_targetSkinData);
			Singleton<DataManager>.instance.saveData();
			close();
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}

	public void OnClickUsePremiumCube()
	{
		if (Singleton<DataManager>.instance.currentGameData._ruby >= (long)WeaponSkinManager.weaponSkinCubePrice && (long)Singleton<DataManager>.instance.currentGameData.weaponSkinReinforcementMasterPiece >= Singleton<WeaponSkinManager>.instance.getMasterPiecePrice(m_targetSkinData.currentGrade))
		{
			Singleton<RubyManager>.instance.decreaseRuby((long)WeaponSkinManager.weaponSkinCubePrice);
			Singleton<WeaponSkinManager>.instance.decreaseWeaponSkinReinforcementMasterPiece(Singleton<WeaponSkinManager>.instance.getMasterPiecePrice(m_targetSkinData.currentGrade));
			Singleton<WeaponSkinManager>.instance.getRandomWeaponSkinData(false, true, m_targetSkinData);
			UIWindowWeaponSkin.instance.openWeaponSkin(m_targetSkinData.currentCharacterType);
			UIWindowWeaponSkinInformation.instance.openWeaponSkinInformation(m_targetSkinData);
			Singleton<DataManager>.instance.saveData();
			close();
		}
		else if (Singleton<DataManager>.instance.currentGameData._ruby < (long)WeaponSkinManager.weaponSkinCubePrice)
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_MASTER_PIECE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}

	public void OnClickOpenAbilityInformationUI()
	{
		UIWindowWeaponSkinAbilityInformation.instance.openWeaponSkinInformation();
	}
}
