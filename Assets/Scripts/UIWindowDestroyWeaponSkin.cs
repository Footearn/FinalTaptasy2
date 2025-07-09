using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowDestroyWeaponSkin : UIWindow
{
	public static UIWindowDestroyWeaponSkin instance;

	public Image[] weaponIconImages;

	public RectTransform weaponSkinIconRectTransform;

	public Text weaponSkinPieceText;

	public GameObject destroyDialogUIObject;

	public GameObject destroyEffectParentObject;

	public CanvasGroup destroyEffectCanvasGroup;

	public GameObject effectObject;

	public RectTransform weaponSkinIconRectTransformForEffect;

	public GameObject weaponSkinIconGameObjectForEffect;

	public GameObject destroyEffectObject;

	public GameObject resultObject;

	public Text weaponSkinPieceValueText;

	public Text weaponSkinReinforcementMasterPieceValueText;

	public CanvasGroup flashBlockCanvasGroup;

	public GameObject sunburstObject;

	private WeaponSkinData m_targetSkinData;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeClose()
	{
		sunburstObject.SetActive(false);
		return base.OnBeforeClose();
	}

	public void openDestroyWeaponSkin(WeaponSkinData skinData)
	{
		sunburstObject.SetActive(true);
		destroyDialogUIObject.SetActive(true);
		destroyEffectParentObject.SetActive(false);
		isCanCloseESC = true;
		m_targetSkinData = skinData;
		if (m_targetSkinData == null)
		{
			return;
		}
		weaponSkinPieceText.text = "x " + Singleton<WeaponSkinManager>.instance.getMinOrMaxWeaponSkinPieceForDestory(m_targetSkinData.currentGrade, true).ToString("N0") + "-" + Singleton<WeaponSkinManager>.instance.getMinOrMaxWeaponSkinPieceForDestory(m_targetSkinData.currentGrade, false).ToString("N0");
		bool flag = m_targetSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || m_targetSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || m_targetSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
		switch (m_targetSkinData.currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			for (int j = 0; j < weaponIconImages.Length; j++)
			{
				weaponIconImages[j].sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetSkinData.currentWarriorWeaponType));
			}
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			for (int k = 0; k < weaponIconImages.Length; k++)
			{
				weaponIconImages[k].sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetSkinData.currentPriestWeaponType));
			}
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			for (int i = 0; i < weaponIconImages.Length; i++)
			{
				weaponIconImages[i].sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(m_targetSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(m_targetSkinData.currentArcherWeaponType));
			}
			break;
		}
		}
		weaponSkinIconRectTransform.localEulerAngles = Singleton<WeaponSkinManager>.instance.getWeaponIconRotation(m_targetSkinData.currentCharacterType);
		weaponSkinIconRectTransformForEffect.localEulerAngles = Singleton<WeaponSkinManager>.instance.getWeaponIconRotation(m_targetSkinData.currentCharacterType);
		for (int l = 0; l < weaponIconImages.Length; l++)
		{
			weaponIconImages[l].SetNativeSize();
		}
		open();
	}

	private void startDestroyEffect()
	{
		UIWindowDialog.openDescription("DESTROY_WEAPON_SKIN_CHECK_DESCRIPTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			isCanCloseESC = false;
			destroyEffectCanvasGroup.alpha = 0f;
			destroyEffectParentObject.SetActive(true);
			destroyEffectObject.SetActive(true);
			resultObject.SetActive(false);
			weaponSkinIconRectTransformForEffect.position = weaponSkinIconRectTransform.position;
			flashBlockCanvasGroup.alpha = 0f;
			weaponSkinIconGameObjectForEffect.SetActive(true);
			effectObject.SetActive(false);
			StopAllCoroutines();
			StartCoroutine("destroyUpdate");
		}, string.Empty);
	}

	private IEnumerator destroyUpdate()
	{
		float distance = Vector2.Distance(weaponSkinIconRectTransformForEffect.anchoredPosition, Vector2.zero);
		while (destroyEffectCanvasGroup.alpha < 1f || distance > 1f)
		{
			destroyEffectCanvasGroup.alpha += Time.deltaTime * 1.4f;
			weaponSkinIconRectTransformForEffect.anchoredPosition = Vector2.Lerp(weaponSkinIconRectTransformForEffect.anchoredPosition, Vector2.zero, Time.deltaTime * 4f);
			distance = Vector2.Distance(weaponSkinIconRectTransformForEffect.anchoredPosition, Vector2.zero);
			yield return null;
		}
		destroyEffectCanvasGroup.alpha = 1f;
		weaponSkinIconRectTransformForEffect.anchoredPosition = Vector2.zero;
		weaponSkinIconGameObjectForEffect.SetActive(false);
		effectObject.SetActive(true);
		Singleton<AudioManager>.instance.playEffectSound("weapon_skin_destroy");
		yield return new WaitForSeconds(0.19f);
		while (flashBlockCanvasGroup.alpha < 1f)
		{
			flashBlockCanvasGroup.alpha += Time.deltaTime * 4f;
			yield return null;
		}
		flashBlockCanvasGroup.alpha = 1f;
		yield return new WaitForSeconds(0.25f);
		destroyDialogUIObject.SetActive(false);
		resultObject.SetActive(true);
		long weaponSkinPiece = 0L;
		long weaponSkinReinforcementMasterPiece = 0L;
		destroyEvent(out weaponSkinPiece, out weaponSkinReinforcementMasterPiece);
		weaponSkinPieceValueText.text = "x " + weaponSkinPiece.ToString("N0");
		weaponSkinReinforcementMasterPieceValueText.text = "x " + weaponSkinReinforcementMasterPiece.ToString("N0");
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		while (flashBlockCanvasGroup.alpha > 0f)
		{
			flashBlockCanvasGroup.alpha -= Time.deltaTime * 4f;
			yield return null;
		}
		flashBlockCanvasGroup.alpha = 0f;
		isCanCloseESC = true;
	}

	private void destroyEvent(out long weaponSkinPiece, out long weaponSkinReinforcementMasterPiece)
	{
		long num = (weaponSkinPiece = Singleton<WeaponSkinManager>.instance.getRandomWeaponSkinPieceForDestory(m_targetSkinData.currentGrade));
		double num2 = (double)Random.Range(0, 10000) / 100.0;
		if (num2 < (double)Singleton<WeaponSkinManager>.instance.getRandomWeaponSkinMasterPieceAppearChance(m_targetSkinData.currentGrade))
		{
			long num3 = (weaponSkinReinforcementMasterPiece = Singleton<WeaponSkinManager>.instance.getRandomWeaponSkinMasterPieceFromDestory(m_targetSkinData.currentGrade));
			Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.WeaponSkinReinforcementMasterPiece, (int)Mathf.Min(num3, 30f), 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			Singleton<WeaponSkinManager>.instance.increaseWeaponSkinReinforcementMasterPiece(num3);
		}
		else
		{
			weaponSkinReinforcementMasterPiece = 0L;
		}
		Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.WeaponSkinPiece, (int)Mathf.Min(num, 30f), 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		Singleton<WeaponSkinManager>.instance.increaseWeaponSkinPiece(num);
		Singleton<DataManager>.instance.currentGameData.weaponSkinData[m_targetSkinData.currentCharacterType].Remove(m_targetSkinData);
		Singleton<DataManager>.instance.saveData();
		UIWindowWeaponSkin.instance.openWeaponSkin(UIWindowWeaponSkin.instance.currentTabCharacterType);
		UIWindowWeaponSkinInformation.instance.close();
	}

	public void OnClickDestroyWeaponSkin()
	{
		if (m_targetSkinData != null)
		{
			startDestroyEffect();
		}
	}
}
