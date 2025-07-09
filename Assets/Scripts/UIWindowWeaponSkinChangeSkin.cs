using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowWeaponSkinChangeSkin : UIWindow
{
	public static UIWindowWeaponSkinChangeSkin instance;

	public GameObject changeUIObject;

	public GameObject changeEffectUIObject;

	public GameObject spiritIconObject;

	public CanvasGroup flashBlockCanvasGroup;

	public Image[] originWeaponSkinIconImages;

	public RectTransform[] originWeaponSkinIconTransforms;

	public Image[] targetWeaponSkinIconImages;

	public RectTransform[] targetWeaponSkinIconTransforms;

	public GameObject targetWeaponSkinIconObject;

	public GameObject selectTextObject;

	public GameObject weaponSkinScrollObject;

	public InfiniteScroll weaponSkinScrollRect;

	public GameObject scrollObject;

	public GameObject emptyObject;

	public CanvasGroup selectUICanvasGroup;

	public WeaponSkinData originSkinData;

	public WeaponSkinData targetSkinData;

	public bool isOpenWeaponSkinSelectUI
	{
		get
		{
			return weaponSkinScrollObject.activeSelf;
		}
	}

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		isCanCloseESC = true;
		flashBlockCanvasGroup.alpha = 0f;
		setTargetWeaponSkin(null);
		return base.OnBeforeOpen();
	}

	public void openWeaponSkinChangeUI(WeaponSkinData originData)
	{
		originSkinData = originData;
		changeUIObject.SetActive(true);
		changeEffectUIObject.SetActive(false);
		spiritIconObject.SetActive(true);
		for (int i = 0; i < originWeaponSkinIconTransforms.Length; i++)
		{
			originWeaponSkinIconTransforms[i].localEulerAngles = Singleton<WeaponSkinManager>.instance.getWeaponIconRotation(originSkinData.currentCharacterType);
		}
		bool flag = originSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || originSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || originSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
		switch (originSkinData.currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			for (int k = 0; k < originWeaponSkinIconImages.Length; k++)
			{
				originWeaponSkinIconImages[k].sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(originSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(originSkinData.currentWarriorWeaponType));
			}
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			for (int l = 0; l < originWeaponSkinIconImages.Length; l++)
			{
				originWeaponSkinIconImages[l].sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(originSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(originSkinData.currentPriestWeaponType));
			}
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			for (int j = 0; j < originWeaponSkinIconImages.Length; j++)
			{
				originWeaponSkinIconImages[j].sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(originSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(originSkinData.currentArcherWeaponType));
			}
			break;
		}
		}
		for (int m = 0; m < originWeaponSkinIconImages.Length; m++)
		{
			originWeaponSkinIconImages[m].SetNativeSize();
		}
		refreshSelectText();
		open();
	}

	public void OnClickSelectWeaponSkin()
	{
		selectUICanvasGroup.alpha = 0f;
		refreshWeaponSkinScroll();
		weaponSkinScrollObject.SetActive(true);
	}

	private void refreshWeaponSkinScroll()
	{
		CharacterManager.CharacterType currentCharacterType = originSkinData.currentCharacterType;
		if (UIWindowWeaponSkin.instance.sortedSkinDictionary[currentCharacterType].Count > 0)
		{
			if (!scrollObject.activeSelf)
			{
				scrollObject.SetActive(true);
			}
			if (emptyObject.activeSelf)
			{
				emptyObject.SetActive(false);
			}
			weaponSkinScrollRect.refreshMaxCount(UIWindowWeaponSkin.instance.sortedSkinDictionary[currentCharacterType].Count);
			weaponSkinScrollRect.resetContentPosition(Vector2.zero);
			weaponSkinScrollRect.syncAllSlotIndexFromPosition();
		}
		else
		{
			if (scrollObject.activeSelf)
			{
				scrollObject.SetActive(false);
			}
			if (!emptyObject.activeSelf)
			{
				emptyObject.SetActive(true);
			}
		}
	}

	public void closeWeaponSkinScroll()
	{
		weaponSkinScrollObject.SetActive(false);
	}

	public void setTargetWeaponSkin(WeaponSkinData skinData)
	{
		targetSkinData = skinData;
		if (targetSkinData != null)
		{
			for (int i = 0; i < targetWeaponSkinIconTransforms.Length; i++)
			{
				targetWeaponSkinIconTransforms[i].localEulerAngles = Singleton<WeaponSkinManager>.instance.getWeaponIconRotation(targetSkinData.currentCharacterType);
			}
			bool flag = targetSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || targetSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || targetSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
			switch (targetSkinData.currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
			{
				for (int k = 0; k < targetWeaponSkinIconImages.Length; k++)
				{
					targetWeaponSkinIconImages[k].sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(targetSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(targetSkinData.currentWarriorWeaponType));
				}
				break;
			}
			case CharacterManager.CharacterType.Priest:
			{
				for (int l = 0; l < targetWeaponSkinIconImages.Length; l++)
				{
					targetWeaponSkinIconImages[l].sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(targetSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(targetSkinData.currentPriestWeaponType));
				}
				break;
			}
			case CharacterManager.CharacterType.Archer:
			{
				for (int j = 0; j < targetWeaponSkinIconImages.Length; j++)
				{
					targetWeaponSkinIconImages[j].sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(targetSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(targetSkinData.currentArcherWeaponType));
				}
				break;
			}
			}
			for (int m = 0; m < targetWeaponSkinIconImages.Length; m++)
			{
				targetWeaponSkinIconImages[m].SetNativeSize();
			}
		}
		refreshSelectText();
		closeWeaponSkinScroll();
	}

	public void OnClickStartChange()
	{
		bool flag = false;
		if (targetSkinData != null)
		{
			if (Singleton<DataManager>.instance.currentGameData._ruby >= (long)WeaponSkinManager.weaponSkinChangePrice)
			{
				bool flag2 = originSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || originSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || originSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
				switch (originSkinData.currentCharacterType)
				{
				case CharacterManager.CharacterType.Warrior:
				{
					WeaponManager.WarriorWeaponType currentWarriorWeaponType = originSkinData.currentWarriorWeaponType;
					WeaponSkinManager.WarriorSpecialWeaponSkinType currentWarriorSpecialWeaponSkinType = originSkinData.currentWarriorSpecialWeaponSkinType;
					WeaponManager.WarriorWeaponType currentWarriorWeaponType2 = targetSkinData.currentWarriorWeaponType;
					WeaponSkinManager.WarriorSpecialWeaponSkinType currentWarriorSpecialWeaponSkinType2 = targetSkinData.currentWarriorSpecialWeaponSkinType;
					if (flag2 ? (currentWarriorSpecialWeaponSkinType == currentWarriorSpecialWeaponSkinType2) : (currentWarriorWeaponType == currentWarriorWeaponType2))
					{
						UIWindowDialog.openDescription("CANNOT_CHANGE_SAME_WEAPON_SKIN", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						break;
					}
					flag = true;
					originSkinData.currentWarriorWeaponType = currentWarriorWeaponType2;
					targetSkinData.currentWarriorWeaponType = currentWarriorWeaponType;
					originSkinData.currentWarriorSpecialWeaponSkinType = currentWarriorSpecialWeaponSkinType2;
					targetSkinData.currentWarriorSpecialWeaponSkinType = currentWarriorSpecialWeaponSkinType;
					break;
				}
				case CharacterManager.CharacterType.Priest:
				{
					WeaponManager.PriestWeaponType currentPriestWeaponType = originSkinData.currentPriestWeaponType;
					WeaponSkinManager.PriestSpecialWeaponSkinType currentPriestSpecialWeaponSkinType = originSkinData.currentPriestSpecialWeaponSkinType;
					WeaponManager.PriestWeaponType currentPriestWeaponType2 = targetSkinData.currentPriestWeaponType;
					WeaponSkinManager.PriestSpecialWeaponSkinType currentPriestSpecialWeaponSkinType2 = targetSkinData.currentPriestSpecialWeaponSkinType;
					if (flag2 ? (currentPriestSpecialWeaponSkinType == currentPriestSpecialWeaponSkinType2) : (currentPriestWeaponType == currentPriestWeaponType2))
					{
						UIWindowDialog.openDescription("CANNOT_CHANGE_SAME_WEAPON_SKIN", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						break;
					}
					flag = true;
					originSkinData.currentPriestWeaponType = currentPriestWeaponType2;
					targetSkinData.currentPriestWeaponType = currentPriestWeaponType;
					originSkinData.currentPriestSpecialWeaponSkinType = currentPriestSpecialWeaponSkinType2;
					targetSkinData.currentPriestSpecialWeaponSkinType = currentPriestSpecialWeaponSkinType;
					break;
				}
				case CharacterManager.CharacterType.Archer:
				{
					WeaponManager.ArcherWeaponType currentArcherWeaponType = originSkinData.currentArcherWeaponType;
					WeaponSkinManager.ArcherSpecialWeaponSkinType currentArcherSpecialWeaponSkinType = originSkinData.currentArcherSpecialWeaponSkinType;
					WeaponManager.ArcherWeaponType currentArcherWeaponType2 = targetSkinData.currentArcherWeaponType;
					WeaponSkinManager.ArcherSpecialWeaponSkinType currentArcherSpecialWeaponSkinType2 = targetSkinData.currentArcherSpecialWeaponSkinType;
					if (flag2 ? (currentArcherSpecialWeaponSkinType == currentArcherSpecialWeaponSkinType2) : (currentArcherWeaponType == currentArcherWeaponType2))
					{
						UIWindowDialog.openDescription("CANNOT_CHANGE_SAME_WEAPON_SKIN", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
						break;
					}
					flag = true;
					originSkinData.currentArcherWeaponType = currentArcherWeaponType2;
					targetSkinData.currentArcherWeaponType = currentArcherWeaponType;
					originSkinData.currentArcherSpecialWeaponSkinType = currentArcherSpecialWeaponSkinType2;
					targetSkinData.currentArcherSpecialWeaponSkinType = currentArcherSpecialWeaponSkinType;
					break;
				}
				}
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, string.Empty);
			}
		}
		else
		{
			UIWindowDialog.openDescription("HAVE_TO_SELECT_TARGET_WEAPON_SKIN", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
		if (flag)
		{
			UIWindowWeaponSkin.instance.openWeaponSkin(UIWindowWeaponSkin.instance.currentTabCharacterType);
			UIWindowWeaponSkinInformation.instance.openWeaponSkinInformation(UIWindowWeaponSkinInformation.instance.targetWeaponSkinData);
			Singleton<RubyManager>.instance.decreaseRuby((long)WeaponSkinManager.weaponSkinChangePrice);
			Singleton<DataManager>.instance.saveData();
			isCanCloseESC = false;
			spiritIconObject.SetActive(false);
			changeEffectUIObject.SetActive(true);
		}
	}

	public void OnClickClose()
	{
		if (isCanCloseESC)
		{
			close();
		}
	}

	public void flashEvent()
	{
		Singleton<AudioManager>.instance.playEffectSound("change_weapon_skin");
		StopCoroutine("flashUpdate");
		StartCoroutine("flashUpdate");
	}

	private IEnumerator flashUpdate()
	{
		while (flashBlockCanvasGroup.alpha < 1f)
		{
			flashBlockCanvasGroup.alpha += Time.deltaTime * 4f;
			yield return null;
		}
		flashBlockCanvasGroup.alpha = 1f;
		openWeaponSkinChangeUI(originSkinData);
		setTargetWeaponSkin(targetSkinData);
		while (flashBlockCanvasGroup.alpha > 0f)
		{
			flashBlockCanvasGroup.alpha -= Time.deltaTime * 4f;
			yield return null;
		}
		isCanCloseESC = true;
	}

	private void refreshSelectText()
	{
		targetWeaponSkinIconObject.SetActive(targetSkinData != null);
		selectTextObject.SetActive(targetSkinData == null);
	}
}
