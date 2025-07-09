using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElopeDaemonKingSlot : ScrollSlotItem
{
	public ElopeModeManager.DaemonKingSkillType currentSlotType;

	public GameObject damageObject;

	public Text damageText;

	public Text nameText;

	public Text descriptionText;

	public Text levelText;

	public Image iconImage;

	public TextMeshProUGUI[] priceTexts;

	public Text[] priceTexts2;

	public Text[] increaseValueText;

	public GameObject progressObject;

	public GameObject upgradeButtonObject;

	public GameObject heartAndRubyUpgradeButton;

	public Image upgradeResourceIconImage;

	public GameObject adsUpgradeButton;

	public GameObject cashUpgradeButton;

	public GameObject timeMachineButtonObject;

	public GameObject daemonKingMultiplyUpgradeBackgroundObject;

	public Image daemonKingPowerProgressBarImage;

	public GameObject unlockableObject;

	public GameObject timerObject;

	public Text timerText;

	public override void UpdateItem(int count)
	{
		base.UpdateItem(count);
		refreshElopeDaemonKingSlot();
	}

	public override void refreshSlot()
	{
		UpdateItem(slotIndex);
	}

	private void refreshElopeDaemonKingSlot()
	{
		if (slotIndex < 0)
		{
			return;
		}
		StopAllCoroutines();
		currentSlotType = (ElopeModeManager.DaemonKingSkillType)slotIndex;
		nameText.text = I18NManager.Get("ELOPE_DAEMON_KING_SLOT_TITLE_" + (slotIndex + 1));
		iconImage.sprite = Singleton<ElopeModeManager>.instance.getElopeDaemonSkillIconSprite(currentSlotType);
		iconImage.SetNativeSize();
		ElopeModeManager.DaemonKingUpgradeResourceType daemonKingSkillUpgradeResourceType = Singleton<ElopeModeManager>.instance.getDaemonKingSkillUpgradeResourceType(currentSlotType);
		if (!upgradeButtonObject.activeSelf)
		{
			upgradeButtonObject.SetActive(true);
		}
		if (timerObject.activeSelf)
		{
			timerObject.SetActive(false);
		}
		switch (daemonKingSkillUpgradeResourceType)
		{
		case ElopeModeManager.DaemonKingUpgradeResourceType.Heart:
		case ElopeModeManager.DaemonKingUpgradeResourceType.Ruby:
			if (timeMachineButtonObject.activeSelf)
			{
				timeMachineButtonObject.SetActive(false);
			}
			if (!heartAndRubyUpgradeButton.activeSelf)
			{
				heartAndRubyUpgradeButton.SetActive(true);
			}
			if (adsUpgradeButton.activeSelf)
			{
				adsUpgradeButton.SetActive(false);
			}
			if (cashUpgradeButton.activeSelf)
			{
				cashUpgradeButton.SetActive(false);
			}
			upgradeResourceIconImage.sprite = ((daemonKingSkillUpgradeResourceType != 0) ? Singleton<ElopeModeManager>.instance.rubyIconSprite : Singleton<ElopeModeManager>.instance.heartIconSprite);
			upgradeResourceIconImage.SetNativeSize();
			break;
		case ElopeModeManager.DaemonKingUpgradeResourceType.Ads:
			if (timeMachineButtonObject.activeSelf)
			{
				timeMachineButtonObject.SetActive(false);
			}
			if (heartAndRubyUpgradeButton.activeSelf)
			{
				heartAndRubyUpgradeButton.SetActive(false);
			}
			if (!adsUpgradeButton.activeSelf)
			{
				adsUpgradeButton.SetActive(true);
			}
			if (cashUpgradeButton.activeSelf)
			{
				cashUpgradeButton.SetActive(false);
			}
			if (currentSlotType != ElopeModeManager.DaemonKingSkillType.SpeedGuyDaemonKing)
			{
				break;
			}
			if (UnbiasedTime.Instance.Now().Ticks > Singleton<DataManager>.instance.currentGameData.speedGuyAdsEndTime)
			{
				if (!upgradeButtonObject.activeSelf)
				{
					upgradeButtonObject.SetActive(true);
				}
				if (timerObject.activeSelf)
				{
					timerObject.SetActive(false);
				}
				break;
			}
			if (upgradeButtonObject.activeSelf)
			{
				upgradeButtonObject.SetActive(false);
			}
			if (!timerObject.activeSelf)
			{
				timerObject.SetActive(true);
			}
			if (base.cachedGameObject.activeSelf && base.cachedGameObject.activeInHierarchy)
			{
				StartCoroutine("adsTimerUpdate");
			}
			break;
		case ElopeModeManager.DaemonKingUpgradeResourceType.Cash:
			if (timeMachineButtonObject.activeSelf)
			{
				timeMachineButtonObject.SetActive(false);
			}
			if (heartAndRubyUpgradeButton.activeSelf)
			{
				heartAndRubyUpgradeButton.SetActive(false);
			}
			if (adsUpgradeButton.activeSelf)
			{
				adsUpgradeButton.SetActive(false);
			}
			if (!cashUpgradeButton.activeSelf)
			{
				cashUpgradeButton.SetActive(true);
			}
			break;
		case ElopeModeManager.DaemonKingUpgradeResourceType.TimeMachine:
			if (!timeMachineButtonObject.activeSelf)
			{
				timeMachineButtonObject.SetActive(true);
			}
			if (heartAndRubyUpgradeButton.activeSelf)
			{
				heartAndRubyUpgradeButton.SetActive(false);
			}
			if (adsUpgradeButton.activeSelf)
			{
				adsUpgradeButton.SetActive(false);
			}
			if (cashUpgradeButton.activeSelf)
			{
				cashUpgradeButton.SetActive(false);
			}
			break;
		}
		for (int i = 0; i < priceTexts.Length; i++)
		{
			switch (daemonKingSkillUpgradeResourceType)
			{
			case ElopeModeManager.DaemonKingUpgradeResourceType.Heart:
				priceTexts[i].text = GameManager.changeUnit(Singleton<ElopeModeManager>.instance.getSkillBuyPrice(currentSlotType));
				break;
			case ElopeModeManager.DaemonKingUpgradeResourceType.Ruby:
				priceTexts[i].text = Singleton<ElopeModeManager>.instance.getSkillBuyPrice(currentSlotType).ToString("N0");
				break;
			case ElopeModeManager.DaemonKingUpgradeResourceType.Cash:
			{
				string text = string.Empty;
				if (currentSlotType == ElopeModeManager.DaemonKingSkillType.SuperHandsomeGuyDaemonKing)
				{
					text = Singleton<PaymentManager>.instance.GetMarketPrice(ShopManager.ElopeModeItemType.SuperHandsomeGuy);
				}
				else if (currentSlotType == ElopeModeManager.DaemonKingSkillType.SuperSpeedGuyDaemonKing)
				{
					text = Singleton<PaymentManager>.instance.GetMarketPrice(ShopManager.ElopeModeItemType.SuperSpeedGuy);
				}
				if (!string.IsNullOrEmpty(text))
				{
					priceTexts[i].text = text;
				}
				else
				{
					priceTexts[i].text = "$" + Singleton<ElopeModeManager>.instance.getSkillBuyPrice(currentSlotType);
				}
				break;
			}
			case ElopeModeManager.DaemonKingUpgradeResourceType.TimeMachine:
				priceTexts[i].text = Singleton<ElopeModeManager>.instance.getSkillBuyPrice(currentSlotType).ToString("N0");
				break;
			}
		}
		for (int j = 0; j < priceTexts2.Length; j++)
		{
			switch (daemonKingSkillUpgradeResourceType)
			{
			case ElopeModeManager.DaemonKingUpgradeResourceType.Heart:
				priceTexts2[j].text = GameManager.changeUnit(Singleton<ElopeModeManager>.instance.getSkillBuyPrice(currentSlotType));
				break;
			case ElopeModeManager.DaemonKingUpgradeResourceType.Ruby:
				priceTexts2[j].text = Singleton<ElopeModeManager>.instance.getSkillBuyPrice(currentSlotType).ToString("N0");
				break;
			case ElopeModeManager.DaemonKingUpgradeResourceType.Cash:
			{
				string text2 = string.Empty;
				if (currentSlotType == ElopeModeManager.DaemonKingSkillType.SuperHandsomeGuyDaemonKing)
				{
					text2 = Singleton<PaymentManager>.instance.GetMarketPrice(ShopManager.ElopeModeItemType.SuperHandsomeGuy);
				}
				else if (currentSlotType == ElopeModeManager.DaemonKingSkillType.SuperSpeedGuyDaemonKing)
				{
					text2 = Singleton<PaymentManager>.instance.GetMarketPrice(ShopManager.ElopeModeItemType.SuperSpeedGuy);
				}
				if (!string.IsNullOrEmpty(text2))
				{
					priceTexts2[j].text = text2;
				}
				else
				{
					priceTexts2[j].text = "$" + Singleton<ElopeModeManager>.instance.getSkillBuyPrice(currentSlotType);
				}
				break;
			}
			case ElopeModeManager.DaemonKingUpgradeResourceType.TimeMachine:
				priceTexts2[j].text = Singleton<ElopeModeManager>.instance.getSkillBuyPrice(currentSlotType).ToString("N0");
				break;
			}
		}
		long skillLevel = Singleton<ElopeModeManager>.instance.getSkillLevel(currentSlotType);
		if (Singleton<ElopeModeManager>.instance.isActiveSkill(currentSlotType))
		{
			levelText.text = string.Empty;
			if (daemonKingMultiplyUpgradeBackgroundObject.activeSelf)
			{
				daemonKingMultiplyUpgradeBackgroundObject.SetActive(false);
			}
			if (unlockableObject.activeSelf)
			{
				unlockableObject.SetActive(false);
			}
		}
		else
		{
			levelText.text = "Lv." + (Singleton<DataManager>.instance.currentGameData.currentDaemonKingLevelForElopeMode[currentSlotType] + 1);
			double num = Singleton<ElopeModeManager>.instance.getSkillValue(currentSlotType, skillLevel + 1, false) - Singleton<ElopeModeManager>.instance.getSkillValue(currentSlotType, skillLevel, false);
			for (int k = 0; k < increaseValueText.Length; k++)
			{
				if (currentSlotType != ElopeModeManager.DaemonKingSkillType.ReinforcementSword)
				{
					increaseValueText[k].text = "+" + ((!(num < 1.0)) ? GameManager.changeUnit(num) : string.Format("{0:0.##}", num));
				}
				else
				{
					increaseValueText[k].text = "+" + num.ToString("N0");
				}
			}
			if (currentSlotType == ElopeModeManager.DaemonKingSkillType.DaemonKingPower || currentSlotType == ElopeModeManager.DaemonKingSkillType.ReinforcementSword || (currentSlotType == ElopeModeManager.DaemonKingSkillType.LoveLovePower && Singleton<ElopeModeManager>.instance.getDaemonKingCriticalChance() < 100.0))
			{
				if (!progressObject.activeSelf)
				{
					progressObject.SetActive(true);
				}
				if ((skillLevel + 1) % ElopeModeManager.daemonKingDoublePowerLevelUnit == 0L)
				{
					if (!daemonKingMultiplyUpgradeBackgroundObject.activeSelf)
					{
						daemonKingMultiplyUpgradeBackgroundObject.SetActive(true);
					}
					if (!unlockableObject.activeSelf)
					{
						unlockableObject.SetActive(true);
					}
					if (currentSlotType == ElopeModeManager.DaemonKingSkillType.LoveLovePower)
					{
						for (int l = 0; l < increaseValueText.Length; l++)
						{
							increaseValueText[l].text = I18NManager.Get("ELOPE_DAEMON_CHANCE_TITLE_TEXT");
						}
					}
				}
				else
				{
					if (daemonKingMultiplyUpgradeBackgroundObject.activeSelf)
					{
						daemonKingMultiplyUpgradeBackgroundObject.SetActive(false);
					}
					if (unlockableObject.activeSelf)
					{
						unlockableObject.SetActive(false);
					}
				}
				long num2;
				for (num2 = skillLevel + 1; num2 > ElopeModeManager.daemonKingDoublePowerLevelUnit; num2 -= ElopeModeManager.daemonKingDoublePowerLevelUnit)
				{
				}
				daemonKingPowerProgressBarImage.fillAmount = (float)num2 / (float)ElopeModeManager.daemonKingDoublePowerLevelUnit;
			}
			else
			{
				if (progressObject.activeSelf)
				{
					progressObject.SetActive(false);
				}
				if (daemonKingMultiplyUpgradeBackgroundObject.activeSelf)
				{
					daemonKingMultiplyUpgradeBackgroundObject.SetActive(false);
				}
				if (unlockableObject.activeSelf)
				{
					unlockableObject.SetActive(false);
				}
			}
		}
		if (currentSlotType == ElopeModeManager.DaemonKingSkillType.DaemonKingPower)
		{
			if (!damageObject.activeSelf)
			{
				damageObject.SetActive(true);
			}
			descriptionText.text = string.Empty;
			damageText.text = GameManager.changeUnit(Singleton<ElopeModeManager>.instance.getSkillValue(currentSlotType, false));
			return;
		}
		if (damageObject.activeSelf)
		{
			damageObject.SetActive(false);
		}
		string text3 = I18NManager.Get("ELOPE_DAEMON_KING_SLOT_DESCRIPTION_" + (slotIndex + 1));
		if (text3.Contains("{"))
		{
			if (currentSlotType == ElopeModeManager.DaemonKingSkillType.LoveLovePower)
			{
				descriptionText.text = string.Format(text3, Singleton<ElopeModeManager>.instance.getDaemonKingCriticalChance(), Singleton<ElopeModeManager>.instance.getSkillValue(currentSlotType, false) + 100.0);
			}
			else
			{
				descriptionText.text = string.Format(text3, Singleton<ElopeModeManager>.instance.getSkillValue(currentSlotType, false));
			}
		}
		else
		{
			descriptionText.text = text3;
		}
	}

	private IEnumerator adsTimerUpdate()
	{
		while (UnbiasedTime.Instance.Now().Ticks < Singleton<DataManager>.instance.currentGameData.speedGuyAdsEndTime)
		{
			TimeSpan endTimeToTimeSpan = new DateTime(Singleton<DataManager>.instance.currentGameData.speedGuyAdsEndTime).Subtract(new DateTime(UnbiasedTime.Instance.Now().Ticks));
			string limitedTimerString = string.Format("{0:00}:{1:00}", endTimeToTimeSpan.Minutes, endTimeToTimeSpan.Seconds);
			timerText.text = limitedTimerString;
			yield return null;
		}
		refreshSlot();
	}

	public void OnClickBuy()
	{
		double skillBuyPrice = Singleton<ElopeModeManager>.instance.getSkillBuyPrice(currentSlotType);
		double num = 0.0;
		ElopeModeManager.DaemonKingUpgradeResourceType daemonKingSkillUpgradeResourceType = Singleton<ElopeModeManager>.instance.getDaemonKingSkillUpgradeResourceType(currentSlotType);
		switch (daemonKingSkillUpgradeResourceType)
		{
		case ElopeModeManager.DaemonKingUpgradeResourceType.Heart:
			num = Singleton<DataManager>.instance.currentGameData.heartForElopeMode;
			break;
		case ElopeModeManager.DaemonKingUpgradeResourceType.Ruby:
			num = Singleton<DataManager>.instance.currentGameData._ruby;
			break;
		}
		if (!Singleton<ElopeModeManager>.instance.isActiveSkill(currentSlotType))
		{
			if (num >= skillBuyPrice)
			{
				switch (daemonKingSkillUpgradeResourceType)
				{
				case ElopeModeManager.DaemonKingUpgradeResourceType.Heart:
					Singleton<ElopeModeManager>.instance.decreaseHeart(skillBuyPrice);
					break;
				case ElopeModeManager.DaemonKingUpgradeResourceType.Ruby:
					Singleton<RubyManager>.instance.decreaseRuby(skillBuyPrice);
					break;
				}
				long skillLevel = Singleton<ElopeModeManager>.instance.getSkillLevel(currentSlotType);
				if (currentSlotType != ElopeModeManager.DaemonKingSkillType.HandsomeGuyDaemonKing)
				{
					if ((skillLevel + 1) % ElopeModeManager.daemonKingDoublePowerLevelUnit == 0L)
					{
						Singleton<AudioManager>.instance.playEffectSound("unlock_princess");
					}
					else
					{
						Singleton<AudioManager>.instance.playEffectSound("upgrade_princess");
					}
				}
				else
				{
					Singleton<AudioManager>.instance.playEffectSound("upgrade_princess");
				}
				Dictionary<ElopeModeManager.DaemonKingSkillType, long> currentDaemonKingLevelForElopeMode;
				Dictionary<ElopeModeManager.DaemonKingSkillType, long> dictionary = (currentDaemonKingLevelForElopeMode = Singleton<DataManager>.instance.currentGameData.currentDaemonKingLevelForElopeMode);
				ElopeModeManager.DaemonKingSkillType key;
				ElopeModeManager.DaemonKingSkillType key2 = (key = currentSlotType);
				long num2 = currentDaemonKingLevelForElopeMode[key];
				dictionary[key2] = num2 + 1;
				Singleton<DataManager>.instance.saveData();
				ObjectPool.Spawn("@ElopeDaemonUpgradeEffect", iconImage.transform.position + new Vector3(0.85f, 0f, 0f), Vector3.zero, new Vector3(88f, 88f, 88f), base.cachedTransform, true);
				refreshSlot();
				if (currentSlotType == ElopeModeManager.DaemonKingSkillType.ReinforcementSword)
				{
					ScrollSlotItem slotItem = UIWindowElopeMode.instance.elopeDaemonKingScrollRect.getSlotItem(0);
					if (slotItem != null)
					{
						slotItem.refreshSlot();
					}
				}
				return;
			}
			switch (daemonKingSkillUpgradeResourceType)
			{
			case ElopeModeManager.DaemonKingUpgradeResourceType.Heart:
				UIWindowDialog.openMiniDialog("NOT_ENOUGH_HEART_FOR_ELOPE_MODE");
				break;
			case ElopeModeManager.DaemonKingUpgradeResourceType.Ruby:
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, string.Empty);
				break;
			}
		}
		else
		{
			Singleton<ElopeModeManager>.instance.castActiveSkill(currentSlotType);
			Singleton<ElopeModeManager>.instance.refreshSkillStatus();
		}
	}
}
