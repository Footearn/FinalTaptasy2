using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowWeaponSkinLottery : UIWindow
{
	public static UIWindowWeaponSkinLottery instance;

	public GameObject cubeObject;

	public GameObject normalCubeEffectObject;

	public GameObject reinforcementCubeEffectObject;

	public GameObject lotteryObject;

	public Image anvilIconImage;

	public GameObject normalLotteryEffectObject;

	public GameObject premiumLotteryEffectObject;

	public GameObject resultObject;

	public Image titleImage;

	public GameObject gradeUpTextObject;

	public Image weaponskinBackgrondImage;

	public Image gradeImage;

	public Image weaponSkinIconImage;

	public Text weaponSkinNameText;

	public Text gradeText;

	public RectTransform weaponSkinIconRectTransform;

	public Text abilityText;

	public CanvasGroup flashBlockCanvasGroup;

	public GameObject flashBlockObject;

	private WeaponSkinData m_targetWeaponSkinData;

	public GameObject sunburstObject;

	public GameObject closeButtonObject;

	public GameObject cubeButtonObject;

	public Text masterPiecePriceText;

	public Text[] priceTexts;

	private bool m_isGradeUp;

	private bool m_isProgressingLotteryUI;

	private List<WeaponSkinData> m_nextWeaponSkinDataList = new List<WeaponSkinData>();

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		isCanCloseESC = false;
		return base.OnBeforeOpen();
	}

	public void openCubeUI(WeaponSkinData skinData, bool isReinforcementCube, bool isGradeUp)
	{
		m_isProgressingLotteryUI = false;
		m_nextWeaponSkinDataList.Clear();
		m_targetWeaponSkinData = skinData;
		m_isGradeUp = isGradeUp;
		cubeObject.SetActive(true);
		flashBlockObject.SetActive(false);
		flashBlockCanvasGroup.alpha = 0f;
		normalCubeEffectObject.SetActive(!isReinforcementCube);
		reinforcementCubeEffectObject.SetActive(isReinforcementCube);
		lotteryObject.SetActive(false);
		resultObject.SetActive(false);
		open();
		StopAllCoroutines();
		StartCoroutine("flashUpdate", false);
	}

	public void openLotteryUI(WeaponSkinData skinData, bool isPremiumLottery)
	{
		if (m_isProgressingLotteryUI)
		{
			m_nextWeaponSkinDataList.Add(skinData);
			return;
		}
		m_isProgressingLotteryUI = true;
		m_targetWeaponSkinData = skinData;
		lotteryObject.SetActive(true);
		anvilIconImage.sprite = ((!isPremiumLottery) ? Singleton<WeaponSkinManager>.instance.normalLotteryAnvilSprite : Singleton<WeaponSkinManager>.instance.premiumLotteryAnvilSprite);
		anvilIconImage.SetNativeSize();
		flashBlockObject.SetActive(false);
		flashBlockCanvasGroup.alpha = 0f;
		normalLotteryEffectObject.SetActive(!isPremiumLottery);
		premiumLotteryEffectObject.SetActive(isPremiumLottery);
		cubeObject.SetActive(false);
		resultObject.SetActive(false);
		open();
		StopAllCoroutines();
		StartCoroutine("flashUpdate", true);
	}

	private IEnumerator flashUpdate(bool isLottery)
	{
		if (isLottery)
		{
			Singleton<AudioManager>.instance.playEffectSound("weapon_skin_forge");
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("weapon_skin_cube");
		}
		yield return new WaitForSeconds((!isLottery) ? 0.68f : 1.5f);
		flashBlockObject.SetActive(true);
		while (flashBlockCanvasGroup.alpha < 1f)
		{
			flashBlockCanvasGroup.alpha += Time.deltaTime * 4f;
			yield return null;
		}
		flashBlockCanvasGroup.alpha = 1f;
		m_isProgressingLotteryUI = false;
		if (isLottery)
		{
			UIWindowWeaponSkin.instance.openWeaponSkin(m_targetWeaponSkinData.currentCharacterType);
			openWeaponSkinInformation(m_targetWeaponSkinData, true, false);
			if (m_targetWeaponSkinData.currentGrade >= WeaponSkinManager.WeaponSkinGradeType.Unique)
			{
				Singleton<AudioManager>.instance.playEffectSound("grade_up");
			}
		}
		else
		{
			openWeaponSkinInformation(m_targetWeaponSkinData, false, m_isGradeUp);
			if (m_targetWeaponSkinData.isEquipped)
			{
				Singleton<StatManager>.instance.refreshAllStats();
				if (UIWindowManageHeroAndWeapon.instance.isOpen)
				{
					UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
				}
			}
		}
		while (flashBlockCanvasGroup.alpha > 0f)
		{
			flashBlockCanvasGroup.alpha -= Time.deltaTime * 4f;
			yield return null;
		}
		flashBlockCanvasGroup.alpha = 0f;
		flashBlockObject.SetActive(false);
	}

	public void openWeaponSkinInformation(WeaponSkinData skinData, bool isNewSkin, bool isGradeUp)
	{
		m_targetWeaponSkinData = skinData;
		sunburstObject.SetActive(true);
		gradeUpTextObject.SetActive(isGradeUp);
		if (isNewSkin)
		{
			titleImage.sprite = Singleton<WeaponSkinManager>.instance.newSkinTitleSprite;
			closeButtonObject.SetActive(true);
			cubeButtonObject.SetActive(false);
		}
		else
		{
			closeButtonObject.SetActive(false);
			cubeButtonObject.SetActive(true);
			for (int i = 0; i < priceTexts.Length; i++)
			{
				priceTexts[i].text = WeaponSkinManager.weaponSkinCubePrice.ToString("N0");
			}
			masterPiecePriceText.text = Singleton<WeaponSkinManager>.instance.getMasterPiecePrice(m_targetWeaponSkinData.currentGrade).ToString("N0");
			Singleton<WeaponSkinManager>.instance.afterUseCubeEvent();
			if (!isGradeUp)
			{
				titleImage.sprite = Singleton<WeaponSkinManager>.instance.successTitleSprite;
				Singleton<AudioManager>.instance.playEffectSound("result_cube");
			}
			else
			{
				titleImage.sprite = Singleton<WeaponSkinManager>.instance.greatTitleSprite;
				Singleton<AudioManager>.instance.playEffectSound("grade_up");
			}
		}
		titleImage.SetNativeSize();
		lotteryObject.SetActive(false);
		cubeObject.SetActive(false);
		resultObject.SetActive(true);
		weaponSkinIconRectTransform.localEulerAngles = Singleton<WeaponSkinManager>.instance.getWeaponIconRotation(m_targetWeaponSkinData.currentCharacterType);
		gradeImage.sprite = Singleton<WeaponSkinManager>.instance.getGradeIconSprite(m_targetWeaponSkinData.currentGrade);
		weaponskinBackgrondImage.sprite = Singleton<WeaponSkinManager>.instance.getGradeBackgroundSprite(m_targetWeaponSkinData.currentGrade);
		gradeText.text = Singleton<WeaponSkinManager>.instance.getGradeString(m_targetWeaponSkinData.currentGrade);
		bool flag = m_targetWeaponSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || m_targetWeaponSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || m_targetWeaponSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
		switch (m_targetWeaponSkinData.currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			weaponSkinIconImage.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetWeaponSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetWeaponSkinData.currentWarriorWeaponType));
			weaponSkinNameText.text = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinName(m_targetWeaponSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponI18NName(m_targetWeaponSkinData.currentWarriorWeaponType));
			break;
		case CharacterManager.CharacterType.Priest:
			weaponSkinIconImage.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetWeaponSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetWeaponSkinData.currentPriestWeaponType));
			weaponSkinNameText.text = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinName(m_targetWeaponSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponI18NName(m_targetWeaponSkinData.currentPriestWeaponType));
			break;
		case CharacterManager.CharacterType.Archer:
			weaponSkinIconImage.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetWeaponSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetWeaponSkinData.currentArcherWeaponType));
			weaponSkinNameText.text = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinName(m_targetWeaponSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponI18NName(m_targetWeaponSkinData.currentArcherWeaponType));
			break;
		}
		gradeImage.SetNativeSize();
		weaponSkinIconImage.SetNativeSize();
		abilityText.text = Singleton<WeaponSkinManager>.instance.getAbilityString(m_targetWeaponSkinData.abilityDataList);
		open();
		isCanCloseESC = true;
	}

	public override bool OnBeforeClose()
	{
		sunburstObject.SetActive(false);
		return base.OnBeforeClose();
	}

	public void OnClickClose()
	{
		if (m_nextWeaponSkinDataList.Count > 0)
		{
			openLotteryUI(m_nextWeaponSkinDataList[0], true);
			m_nextWeaponSkinDataList.RemoveAt(0);
		}
		else
		{
			close();
		}
	}

	public void OnClickNormalCube()
	{
		UIWindowWeaponSkinCube.instance.OnClickUseNormalCube();
	}

	public void OnClickReinforcementCube()
	{
		UIWindowWeaponSkinCube.instance.OnClickUsePremiumCube();
	}
}
