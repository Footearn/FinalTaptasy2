using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowAdsAngelBuffDialog : UIWindow
{
	public static UIWindowAdsAngelBuffDialog instance;

	public Text goldText;

	public GameObject goldBuffObject;

	public GameObject doubleGoldBuffObject;

	public GameObject doubleAttackDamageBuffObject;

	public GameObject specialAttackDamageBuffObject;

	public Text specialAttackDamageDescriptionText;

	public GameObject specialArmorDamageBuffObject;

	public Text specialArmorDescriptionText;

	public Text autoTouchDescriptionText;

	public Text autoOpenTreasureChestDescriptionText;

	private Action m_targetClickOKAction;

	private AdsAngelManager.AngelRewardType m_targetAngelRewardType;

	private AdsAngelManager.SpecialAngelRewardType m_targetSpecialAngelRewardType;

	private bool m_isSpecialAngel;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openWithAdsAngelBuffInformation(AdsAngelManager.AngelRewardType targetAngelRewardType, Action clickOKAction)
	{
		m_isSpecialAngel = false;
		UIWindowResult.instance.pauseNextTimer();
		goldBuffObject.SetActive(false);
		doubleGoldBuffObject.SetActive(false);
		doubleAttackDamageBuffObject.SetActive(false);
		specialAttackDamageBuffObject.SetActive(false);
		specialArmorDamageBuffObject.SetActive(false);
		m_targetClickOKAction = clickOKAction;
		m_targetAngelRewardType = targetAngelRewardType;
		m_targetSpecialAngelRewardType = AdsAngelManager.SpecialAngelRewardType.None;
		switch (targetAngelRewardType)
		{
		case AdsAngelManager.AngelRewardType.Gold:
			goldText.text = GameManager.changeUnit(Singleton<AdsAngelManager>.instance.targetRewardGoldValue);
			goldBuffObject.SetActive(true);
			break;
		case AdsAngelManager.AngelRewardType.AutoTouch:
			doubleGoldBuffObject.SetActive(true);
			autoTouchDescriptionText.text = string.Format(I18NManager.Get("ADS_ANGEL_FAST_TOUCH_DESCRIPTION_TEXT_2"), ParsingManager.adsAngelFastTouchDuringTime);
			break;
		case AdsAngelManager.AngelRewardType.AutoOpenTreasureChest:
			doubleAttackDamageBuffObject.SetActive(true);
			autoOpenTreasureChestDescriptionText.text = string.Format(I18NManager.Get("ADS_ANGEL_DOUBLE_DAMAGE_DESCRIPTION_TEXT_2"), ParsingManager.adsAngelAutoOpenTreasureChestDuringTime);
			break;
		}
		open();
	}

	public void openWithAdsAngelBuffInformation(AdsAngelManager.SpecialAngelRewardType targetAngelRewardType, Action clickOKAction)
	{
		m_isSpecialAngel = true;
		UIWindowResult.instance.pauseNextTimer();
		goldBuffObject.SetActive(false);
		doubleGoldBuffObject.SetActive(false);
		doubleAttackDamageBuffObject.SetActive(false);
		specialAttackDamageBuffObject.SetActive(false);
		specialArmorDamageBuffObject.SetActive(false);
		m_targetClickOKAction = clickOKAction;
		m_targetAngelRewardType = AdsAngelManager.AngelRewardType.NULL;
		m_targetSpecialAngelRewardType = targetAngelRewardType;
		string empty = string.Empty;
		switch (m_targetSpecialAngelRewardType)
		{
		case AdsAngelManager.SpecialAngelRewardType.Damage:
			specialAttackDamageDescriptionText.text = string.Format(I18NManager.Get("SPECIAL_ADS_ANGEL_DESCRIPTION_TEXT_1"), "15");
			specialAttackDamageBuffObject.SetActive(true);
			break;
		case AdsAngelManager.SpecialAngelRewardType.Health:
			specialArmorDescriptionText.text = string.Format(I18NManager.Get("SPECIAL_ADS_ANGEL_DESCRIPTION_TEXT_2"), "15");
			specialArmorDamageBuffObject.SetActive(true);
			break;
		}
		open();
	}

	public void onClickOK()
	{
		m_targetClickOKAction();
		if (!m_isSpecialAngel)
		{
			AnalyzeManager.retention(AnalyzeManager.CategoryType.Angel, AnalyzeManager.ActionType.AngelAd, new Dictionary<string, string>
			{
				{
					"AngelRewardType",
					m_targetAngelRewardType.ToString()
				}
			});
		}
		else
		{
			AnalyzeManager.retention(AnalyzeManager.CategoryType.SpecialAngel, AnalyzeManager.ActionType.AngelAd, new Dictionary<string, string>
			{
				{
					"SpecialAngelRewardType",
					m_targetSpecialAngelRewardType.ToString()
				}
			});
		}
		close();
	}

	public void onClickCancel()
	{
		m_targetClickOKAction = null;
		close();
	}

	public override void OnAfterClose()
	{
		if (UIWindowResult.instance.isOpen)
		{
			if (!m_isSpecialAngel && Singleton<AdsAngelManager>.instance.isProgressingBuff)
			{
				Singleton<AdsAngelManager>.instance.refreshSpawnTime();
			}
			UIWindowResult.instance.startNextTimer();
		}
		base.OnAfterClose();
	}
}
