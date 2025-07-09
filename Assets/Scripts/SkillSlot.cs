using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : ScrollSlotItem
{
	public Image skillIcon;

	public Text skillName;

	public Text skillLevel;

	public Text skillDescrtiption;

	public Text costText;

	public Image backgroundImage;

	public GameObject upgradeObject;

	public Image upgradeButtonBackgroundImage;

	public Image upgradeGoldIconImage;

	public Text upgradeTitleText;

	public Text skillUpgradeValue;

	public Text skillUpgradePrice;

	public GameObject lockObject;

	public Text unlockTitleText;

	public Image unlockButtonImage;

	public Text unlockPriceText;

	public Image unlockRubyIconImage;

	public ParticleSystem upgradeEffect;

	public ParticleSystem unlockEffect;

	public ParticleSystem unlockableEffect;

	public Animation cantUpgradeButtonAnimation;

	public GameObject costObject;

	public GameObject previewButtonObject;

	public GameObject buyAutoTouchObject;

	public GameObject autoTouchOnObject;

	public Text autoTouchTimerText;

	public GameObject autoTouchTimerTextObject;

	public Text autoTouchCountText;

	public GameObject autoTouchCountTextObject;

	public Image autoTouchTimerIconImage;

	public Text autoTouchPriceText;

	public GameObject transcendSkillObject;

	public GameObject transcendSkillLockedIconObject;

	public GameObject transcendSkillUppgradeObject;

	public Text transcendSkillUpgradePriceText;

	public Text transcendSkillLevelText;

	public Text transcendSkillChanceDescriptionText;

	public Text transcendSkillUnlockDescriptionText;

	public int realSkillSlotIndex;

	private SkillInventoryData skillData;

	private PassiveSkillInventoryData passiveSkillData;

	private bool m_isCustomSkill;

	private bool m_isNormalSkill;

	private bool m_isReinforcementSkill;

	private bool m_isPassiveSkill;

	private bool m_isTranscendSkill;

	private float m_quickUpgradeDisappearTimer;

	public Text quickUpgradeTotalPriceText;

	public GameObject quickUpgradeButtonObject;

	public Animation quickUpgradeButtonAnimation;

	public bool isQuickUpgrading = true;

	public GameObject passiveSkillObject;

	public GameObject passiveSkillLockedObject;

	public GameObject passiveSkillUnlockedObject;

	public Text passiveSkillUnlockPriceText;

	public Text passiveSkillUpgradePriceText;

	public Text passiveSkillLevel;

	public GameObject passiveSkillUpgradeButtonObject;

	public GameObject passiveSkillMaxLevelObject;

	public Image[] passiveSkillUnlockAndUpgradeResourceImages;

	public GameObject reinforcementSkillObject;

	public GameObject reinforcementSkillLockedObject;

	public GameObject reinforcementSkillUnlockedObject;

	public GameObject reinforcementSkillUpgradeButtonObject;

	public GameObject reinforcementSkillMaxLevelButtonObject;

	public Text reinforcementSkillUnlockPriceText;

	public Text reinforcementSkillUpgradePriceText;

	public Text reinforcementSkillLevel;

	public ReinforcementSkillInventoryData currentReinforcementSkillInventoryData;

	public void refreshBuyState()
	{
		if (!m_isCustomSkill)
		{
			double num = Singleton<SkillManager>.instance.getSkillUpgradePrice((SkillManager.SkillType)realSkillSlotIndex, skillData.skillLevel);
			if (Singleton<DataManager>.instance.currentGameData.gold >= num)
			{
				upgradeButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.enableButtonOrangeSprite;
				upgradeTitleText.color = Util.getCalculatedColor(253f, 254f, 156f);
				skillUpgradePrice.color = Color.white;
				skillUpgradeValue.color = Color.white;
				upgradeGoldIconImage.color = Color.white;
			}
			else
			{
				upgradeButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.disableButtonSprite;
				upgradeTitleText.color = Util.getCalculatedColor(153f, 153f, 153f);
				skillUpgradePrice.color = Util.getCalculatedColor(153f, 153f, 153f);
				skillUpgradeValue.color = Util.getCalculatedColor(153f, 153f, 153f);
				upgradeGoldIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
			}
		}
	}

	public override void UpdateItem(int count)
	{
		base.UpdateItem(count);
		closeQuickUpgradeButton(true);
		refreshSlot();
	}

	private void refreshAutoTouchSkillTime()
	{
		switch (slotIndex)
		{
		case 0:
			if (!autoTouchTimerTextObject.activeSelf)
			{
				autoTouchTimerTextObject.SetActive(true);
			}
			if (autoTouchCountTextObject.activeSelf)
			{
				autoTouchCountTextObject.SetActive(false);
			}
			if (Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime > UnbiasedTime.Instance.Now().Ticks)
			{
				if (!autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(true);
				}
				if (autoTouchTimerText.color != Util.getCalculatedColor(250f, 215f, 37f) || autoTouchTimerText.color != Util.getCalculatedColor(250f, 215f, 37f))
				{
					autoTouchTimerText.color = Util.getCalculatedColor(250f, 215f, 37f);
					autoTouchTimerIconImage.color = Util.getCalculatedColor(250f, 215f, 37f);
				}
				TimeSpan timeSpan4 = new TimeSpan(Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime - UnbiasedTime.Instance.Now().Ticks);
				autoTouchTimerText.text = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan4.TotalHours, timeSpan4.Minutes, timeSpan4.Seconds) + " " + I18NManager.Get("TIME_LEFT");
			}
			else
			{
				if (autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(false);
				}
				if (!autoTouchTimerText.text.Equals("00:00:00 " + I18NManager.Get("TIME_LEFT")))
				{
					autoTouchTimerText.color = Util.getCalculatedColor(82f, 103f, 114f);
					autoTouchTimerIconImage.color = Util.getCalculatedColor(82f, 103f, 114f);
					autoTouchTimerText.text = "00:00:00 " + I18NManager.Get("TIME_LEFT");
				}
			}
			break;
		case 1:
			if (!autoTouchTimerTextObject.activeSelf)
			{
				autoTouchTimerTextObject.SetActive(true);
			}
			if (autoTouchCountTextObject.activeSelf)
			{
				autoTouchCountTextObject.SetActive(false);
			}
			if (Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime > UnbiasedTime.Instance.Now().Ticks)
			{
				if (!autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(true);
				}
				if (autoTouchTimerText.color != Util.getCalculatedColor(250f, 215f, 37f) || autoTouchTimerText.color != Util.getCalculatedColor(250f, 215f, 37f))
				{
					autoTouchTimerText.color = Util.getCalculatedColor(250f, 215f, 37f);
					autoTouchTimerIconImage.color = Util.getCalculatedColor(250f, 215f, 37f);
				}
				TimeSpan timeSpan2 = new TimeSpan(Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime - UnbiasedTime.Instance.Now().Ticks);
				autoTouchTimerText.text = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan2.TotalHours, timeSpan2.Minutes, timeSpan2.Seconds) + " " + I18NManager.Get("TIME_LEFT");
			}
			else
			{
				if (autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(false);
				}
				if (!autoTouchTimerText.text.Equals("00:00:00 " + I18NManager.Get("TIME_LEFT")))
				{
					autoTouchTimerText.color = Util.getCalculatedColor(82f, 103f, 114f);
					autoTouchTimerIconImage.color = Util.getCalculatedColor(82f, 103f, 114f);
					autoTouchTimerText.text = "00:00:00 " + I18NManager.Get("TIME_LEFT");
				}
			}
			break;
		case 2:
			if (autoTouchTimerTextObject.activeSelf)
			{
				autoTouchTimerTextObject.SetActive(false);
			}
			if (!autoTouchCountTextObject.activeSelf)
			{
				autoTouchCountTextObject.SetActive(true);
			}
			if (Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount > 0)
			{
				if (!autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(true);
				}
				if (autoTouchCountText.color != Util.getCalculatedColor(250f, 215f, 37f) || autoTouchCountText.color != Util.getCalculatedColor(250f, 215f, 37f))
				{
					autoTouchCountText.color = Util.getCalculatedColor(250f, 215f, 37f);
				}
				if (autoTouchCountText.text != string.Format(I18NManager.Get("COUNT_AUTO_TAP_COUNT"), Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount) + " " + I18NManager.Get("TIME_LEFT"))
				{
					autoTouchCountText.text = string.Format(I18NManager.Get("COUNT_AUTO_TAP_COUNT"), Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount) + " " + I18NManager.Get("TIME_LEFT");
				}
			}
			else
			{
				if (autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(false);
				}
				if (!autoTouchCountText.text.Equals(string.Format(I18NManager.Get("COUNT_AUTO_TAP_COUNT"), 0) + " " + I18NManager.Get("TIME_LEFT")))
				{
					autoTouchCountText.color = Util.getCalculatedColor(82f, 103f, 114f);
					autoTouchCountText.text = string.Format(I18NManager.Get("COUNT_AUTO_TAP_COUNT"), 0) + " " + I18NManager.Get("TIME_LEFT");
				}
			}
			break;
		case 3:
			if (!autoTouchTimerTextObject.activeSelf)
			{
				autoTouchTimerTextObject.SetActive(true);
			}
			if (autoTouchCountTextObject.activeSelf)
			{
				autoTouchCountTextObject.SetActive(false);
			}
			if (Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime > UnbiasedTime.Instance.Now().Ticks)
			{
				if (!autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(true);
				}
				if (autoTouchTimerText.color != Util.getCalculatedColor(250f, 215f, 37f) || autoTouchTimerText.color != Util.getCalculatedColor(250f, 215f, 37f))
				{
					autoTouchTimerText.color = Util.getCalculatedColor(250f, 215f, 37f);
					autoTouchTimerIconImage.color = Util.getCalculatedColor(250f, 215f, 37f);
				}
				TimeSpan timeSpan3 = new TimeSpan(Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime - UnbiasedTime.Instance.Now().Ticks);
				autoTouchTimerText.text = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan3.TotalHours, timeSpan3.Minutes, timeSpan3.Seconds) + " " + I18NManager.Get("TIME_LEFT");
			}
			else
			{
				if (autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(false);
				}
				if (!autoTouchTimerText.text.Equals("00:00:00 " + I18NManager.Get("TIME_LEFT")))
				{
					autoTouchTimerText.color = Util.getCalculatedColor(82f, 103f, 114f);
					autoTouchTimerIconImage.color = Util.getCalculatedColor(82f, 103f, 114f);
					autoTouchTimerText.text = "00:00:00 " + I18NManager.Get("TIME_LEFT");
				}
			}
			break;
		case 4:
			if (!autoTouchTimerTextObject.activeSelf)
			{
				autoTouchTimerTextObject.SetActive(true);
			}
			if (autoTouchCountTextObject.activeSelf)
			{
				autoTouchCountTextObject.SetActive(false);
			}
			if (Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime > UnbiasedTime.Instance.Now().Ticks)
			{
				if (!autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(true);
				}
				if (autoTouchTimerText.color != Util.getCalculatedColor(250f, 215f, 37f) || autoTouchTimerText.color != Util.getCalculatedColor(250f, 215f, 37f))
				{
					autoTouchTimerText.color = Util.getCalculatedColor(250f, 215f, 37f);
					autoTouchTimerIconImage.color = Util.getCalculatedColor(250f, 215f, 37f);
				}
				TimeSpan timeSpan = new TimeSpan(Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime - UnbiasedTime.Instance.Now().Ticks);
				autoTouchTimerText.text = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds) + " " + I18NManager.Get("TIME_LEFT");
			}
			else
			{
				if (autoTouchOnObject.activeSelf)
				{
					autoTouchOnObject.SetActive(false);
				}
				if (!autoTouchTimerText.text.Equals("00:00:00 " + I18NManager.Get("TIME_LEFT")))
				{
					autoTouchTimerText.color = Util.getCalculatedColor(82f, 103f, 114f);
					autoTouchTimerIconImage.color = Util.getCalculatedColor(82f, 103f, 114f);
					autoTouchTimerText.text = "00:00:00 " + I18NManager.Get("TIME_LEFT");
				}
			}
			break;
		}
	}

	private void Update()
	{
		if (m_isCustomSkill)
		{
			refreshAutoTouchSkillTime();
		}
	}

	public override void refreshSlot()
	{
		int num = 5;
		int num2 = 5;
		int num3 = 5;
		int num4 = 3;
		int num5 = 3;
		m_isCustomSkill = ((base.slotIndex < num) ? true : false);
		m_isNormalSkill = ((base.slotIndex < num + num2) ? true : false);
		m_isReinforcementSkill = ((base.slotIndex < num + num2 + num3) ? true : false);
		m_isPassiveSkill = ((base.slotIndex < num + num2 + num3 + num4) ? true : false);
		m_isTranscendSkill = ((base.slotIndex < num + num2 + num3 + num4 + num5) ? true : false);
		unlockableEffect.Stop();
		if ((base.slotIndex + 1) % 2 == 0)
		{
			backgroundImage.color = new Color(0f, 0f, 0f, 0f);
		}
		else
		{
			backgroundImage.color = Util.getCalculatedColor(0f, 11f, 28f, 51f);
		}
		if (m_isCustomSkill)
		{
			skillData = null;
			skillIcon.sprite = UIWindowSkill.instance.autoTouchSkillIcons[base.slotIndex];
			skillName.text = I18NManager.Get("AUTOTOUCH_SKILL_NAME_" + (base.slotIndex + 1));
			SkillManager.CustomSkillType slotIndex = (SkillManager.CustomSkillType)base.slotIndex;
			AutoTouchManager.AutoTouchType autoTouchType = AutoTouchManager.AutoTouchType.TimerSilverFinger;
			switch (slotIndex)
			{
			case SkillManager.CustomSkillType.TimerSilverFinger:
				autoTouchType = AutoTouchManager.AutoTouchType.TimerSilverFinger;
				break;
			case SkillManager.CustomSkillType.TimerGoldFinger:
				autoTouchType = AutoTouchManager.AutoTouchType.TimerGoldFinger;
				break;
			case SkillManager.CustomSkillType.CountGoldFinger:
				autoTouchType = AutoTouchManager.AutoTouchType.CountGoldFinger;
				break;
			}
			switch (slotIndex)
			{
			case SkillManager.CustomSkillType.CountGoldFinger:
				skillDescrtiption.text = string.Format(I18NManager.Get("AUTOTOUCH_SKILL_COUNT_DESCRIPTION"), Math.Round(1f / Singleton<AutoTouchManager>.instance.getAutoTouchDelay(autoTouchType)).ToString());
				break;
			case SkillManager.CustomSkillType.DoubleSpeed:
				skillDescrtiption.text = I18NManager.Get("AUTOTOUCH_SKILL_DESCRIPTION_5");
				break;
			case SkillManager.CustomSkillType.TimerAutoOpenTreasureChest:
				skillDescrtiption.text = I18NManager.Get("AUTOTOUCH_SKILL_DESCRIPTION_4");
				break;
			default:
				skillDescrtiption.text = string.Format(I18NManager.Get("AUTOTOUCH_SKILL_DESCRIPTION"), Math.Round(1f / Singleton<AutoTouchManager>.instance.getAutoTouchDelay(autoTouchType)).ToString());
				break;
			}
			skillName.color = Util.getCalculatedColor(253f, 252f, 183f);
			skillDescrtiption.color = Util.getCalculatedColor(30f, 199f, 255f);
			if (passiveSkillObject.activeSelf)
			{
				passiveSkillObject.SetActive(false);
			}
			if (lockObject.activeSelf)
			{
				lockObject.SetActive(false);
			}
			if (upgradeObject.activeSelf)
			{
				upgradeObject.SetActive(false);
			}
			if (costObject.activeSelf)
			{
				costObject.SetActive(false);
			}
			if (previewButtonObject.activeSelf)
			{
				previewButtonObject.SetActive(false);
			}
			if (transcendSkillObject.activeSelf)
			{
				transcendSkillObject.SetActive(false);
			}
			if (!buyAutoTouchObject.activeSelf)
			{
				buyAutoTouchObject.SetActive(true);
			}
			if (reinforcementSkillObject.activeSelf)
			{
				reinforcementSkillObject.SetActive(false);
			}
			autoTouchPriceText.text = Singleton<SkillManager>.instance.getAutoTouchSkillBuyPrice(slotIndex).ToString("N0");
			refreshAutoTouchSkillTime();
		}
		else if (m_isNormalSkill)
		{
			if (passiveSkillObject.activeSelf)
			{
				passiveSkillObject.SetActive(false);
			}
			if (autoTouchOnObject.activeSelf)
			{
				autoTouchOnObject.SetActive(false);
			}
			if (!costObject.activeSelf)
			{
				costObject.SetActive(true);
			}
			if (!previewButtonObject.activeSelf)
			{
				previewButtonObject.SetActive(true);
			}
			if (buyAutoTouchObject.activeSelf)
			{
				buyAutoTouchObject.SetActive(false);
			}
			if (transcendSkillObject.activeSelf)
			{
				transcendSkillObject.SetActive(false);
			}
			if (reinforcementSkillObject.activeSelf)
			{
				reinforcementSkillObject.SetActive(false);
			}
			realSkillSlotIndex = base.slotIndex - 5;
			skillData = Singleton<SkillManager>.instance.getSkillInventoryData((SkillManager.SkillType)realSkillSlotIndex);
			costText.text = Singleton<SkillManager>.instance.activeSkillData[realSkillSlotIndex].cost.ToString();
			int num6 = realSkillSlotIndex + 1;
			skillName.text = I18NManager.Get("SKILL_NAME_" + num6);
			skillLevel.text = "Lv. " + skillData.skillLevel;
			double num7 = Singleton<SkillManager>.instance.activeSkillData[realSkillSlotIndex].baseDamage + Singleton<SkillManager>.instance.activeSkillData[realSkillSlotIndex].upgradeDamage * (double)(skillData.skillLevel - 1);
			double num8 = num7 / 100.0 * Singleton<StatManager>.instance.skillExtraPercentDamage;
			if (num8 != 0.0)
			{
				if (realSkillSlotIndex == 4)
				{
					skillDescrtiption.text = string.Format(I18NManager.Get("SKILL_DESCRIPTION_" + num6), "<color=white>" + num7.ToString("0.#") + "</color><color=#FFFC2E>(+" + num8.ToString("0.#") + ")</color>", 3 + (int)Singleton<StatManager>.instance.dragonBreathExtraAttackCount);
				}
				else
				{
					skillDescrtiption.text = string.Format(I18NManager.Get("SKILL_DESCRIPTION_" + num6), "<color=white>" + num7.ToString("0.#") + "</color><color=#FFFC2E>(+" + num8.ToString("0.#") + ")</color>");
				}
			}
			else if (realSkillSlotIndex == 4)
			{
				skillDescrtiption.text = string.Format(I18NManager.Get("SKILL_DESCRIPTION_" + num6), "<color=white>" + num7.ToString("0.#") + "</color>", 3 + (int)Singleton<StatManager>.instance.dragonBreathExtraAttackCount);
			}
			else
			{
				skillDescrtiption.text = string.Format(I18NManager.Get("SKILL_DESCRIPTION_" + num6), "<color=white>" + num7.ToString("0.#") + "</color>");
			}
			if (realSkillSlotIndex == 2)
			{
				skillDescrtiption.text = string.Format(skillDescrtiption.text.Replace("@", "<color=white>{0}</color>"), Singleton<SkillManager>.instance.getSkillMaxDuration(SkillManager.SkillType.Concentration));
			}
			if (skillData.isHasSkill)
			{
				skillIcon.sprite = UIWindowSkill.instance.skillIcon[realSkillSlotIndex];
				if (lockObject.activeSelf)
				{
					lockObject.SetActive(false);
				}
				if (!upgradeObject.activeSelf)
				{
					upgradeObject.SetActive(true);
				}
				double num9 = Singleton<SkillManager>.instance.activeSkillData[realSkillSlotIndex].baseDamage + Singleton<SkillManager>.instance.activeSkillData[realSkillSlotIndex].upgradeDamage * (double)skillData.skillLevel;
				skillUpgradeValue.text = "+ " + (num9 - num7).ToString("0.#") + "%";
				skillUpgradePrice.text = GameManager.changeUnit(Singleton<SkillManager>.instance.getSkillUpgradePrice((SkillManager.SkillType)realSkillSlotIndex, skillData.skillLevel));
				skillData.isNewSkill = false;
				skillName.color = Util.getCalculatedColor(253f, 252f, 183f);
				skillDescrtiption.color = Util.getCalculatedColor(30f, 199f, 255f);
			}
			else
			{
				skillIcon.sprite = UIWindowSkill.instance.skillLockedIcon[realSkillSlotIndex];
				skillName.color = Util.getCalculatedColor(153f, 153f, 153f);
				skillDescrtiption.color = Util.getCalculatedColor(153f, 153f, 153f);
				skillDescrtiption.text = skillDescrtiption.text.Replace("<color=white>", string.Empty).Replace("</color>", string.Empty).Replace("<color=#FFFC2E>", string.Empty);
				if (!lockObject.activeSelf)
				{
					lockObject.SetActive(true);
				}
				if (upgradeObject.activeSelf)
				{
					upgradeObject.SetActive(false);
				}
				double skillUnlockGoldPrice = Singleton<SkillManager>.instance.getSkillUnlockGoldPrice(skillData.skillType);
				unlockPriceText.text = GameManager.changeUnit(skillUnlockGoldPrice);
				if (Singleton<DataManager>.instance.currentGameData.gold >= skillUnlockGoldPrice)
				{
					unlockableEffect.Stop();
					unlockableEffect.Play();
					unlockPriceText.color = Color.white;
					unlockButtonImage.sprite = Singleton<CachedManager>.instance.enableButtonGreenSprite;
					unlockRubyIconImage.color = Color.white;
					unlockPriceText.color = Color.white;
					unlockTitleText.color = Util.getCalculatedColor(253f, 254f, 156f);
				}
				else
				{
					unlockableEffect.Stop();
					unlockPriceText.color = Util.getCalculatedColor(153f, 153f, 153f);
					unlockButtonImage.sprite = Singleton<CachedManager>.instance.disableButtonSprite;
					unlockRubyIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
					unlockPriceText.color = Util.getCalculatedColor(153f, 153f, 153f);
					unlockTitleText.color = Util.getCalculatedColor(153f, 153f, 153f);
				}
			}
			refreshBuyState();
		}
		else if (m_isReinforcementSkill)
		{
			if (passiveSkillObject.activeSelf)
			{
				passiveSkillObject.SetActive(false);
			}
			if (autoTouchOnObject.activeSelf)
			{
				autoTouchOnObject.SetActive(false);
			}
			if (costObject.activeSelf)
			{
				costObject.SetActive(false);
			}
			if (previewButtonObject.activeSelf)
			{
				previewButtonObject.SetActive(false);
			}
			if (buyAutoTouchObject.activeSelf)
			{
				buyAutoTouchObject.SetActive(false);
			}
			if (transcendSkillObject.activeSelf)
			{
				transcendSkillObject.SetActive(false);
			}
			if (lockObject.activeSelf)
			{
				lockObject.SetActive(false);
			}
			if (upgradeObject.activeSelf)
			{
				upgradeObject.SetActive(false);
			}
			if (!reinforcementSkillObject.activeSelf)
			{
				reinforcementSkillObject.SetActive(true);
			}
			realSkillSlotIndex = base.slotIndex - (num + num2);
			currentReinforcementSkillInventoryData = Singleton<SkillManager>.instance.getReinforcementSkillInventoryData((SkillManager.SkillType)realSkillSlotIndex);
			SkillManager.SkillType skillType = (SkillManager.SkillType)realSkillSlotIndex;
			skillName.text = Singleton<SkillManager>.instance.getReinforcementSkillName(skillType);
			reinforcementSkillLevel.text = "Lv. " + currentReinforcementSkillInventoryData.skillLevel;
			double reinforcementSkillCastChance = Singleton<SkillManager>.instance.getReinforcementSkillCastChance(skillType);
			double reinforcementSkillValue = Singleton<SkillManager>.instance.getReinforcementSkillValue(skillType);
			skillDescrtiption.text = Singleton<SkillManager>.instance.getReinforcementSkillDescription(skillType, reinforcementSkillCastChance, reinforcementSkillValue);
			if (currentReinforcementSkillInventoryData.isUnlocked)
			{
				if (reinforcementSkillLockedObject.activeSelf)
				{
					reinforcementSkillLockedObject.SetActive(false);
				}
				if (!reinforcementSkillUnlockedObject.activeSelf)
				{
					reinforcementSkillUnlockedObject.SetActive(true);
				}
				skillIcon.sprite = UIWindowSkill.instance.reinforcementSkillIcon[realSkillSlotIndex];
				skillName.color = Util.getCalculatedColor(253f, 252f, 183f);
				skillDescrtiption.color = Util.getCalculatedColor(30f, 199f, 255f);
				double num10 = Singleton<SkillManager>.instance.getReinforcementUpgradePrice(skillType, currentReinforcementSkillInventoryData.skillLevel);
				reinforcementSkillUpgradePriceText.text = num10.ToString("N0");
				if (currentReinforcementSkillInventoryData.skillLevel < Singleton<SkillManager>.instance.getReinforcementMaxLevel())
				{
					if (!reinforcementSkillUpgradeButtonObject.activeSelf)
					{
						reinforcementSkillUpgradeButtonObject.SetActive(true);
					}
					if (reinforcementSkillMaxLevelButtonObject.activeSelf)
					{
						reinforcementSkillMaxLevelButtonObject.SetActive(false);
					}
				}
				else
				{
					if (reinforcementSkillUpgradeButtonObject.activeSelf)
					{
						reinforcementSkillUpgradeButtonObject.SetActive(false);
					}
					if (!reinforcementSkillMaxLevelButtonObject.activeSelf)
					{
						reinforcementSkillMaxLevelButtonObject.SetActive(true);
					}
				}
			}
			else
			{
				if (!reinforcementSkillLockedObject.activeSelf)
				{
					reinforcementSkillLockedObject.SetActive(true);
				}
				if (reinforcementSkillUnlockedObject.activeSelf)
				{
					reinforcementSkillUnlockedObject.SetActive(false);
				}
				skillIcon.sprite = UIWindowSkill.instance.reinforcementSkillLockedIcon[realSkillSlotIndex];
				skillName.color = Util.getCalculatedColor(153f, 153f, 153f);
				skillDescrtiption.color = Util.getCalculatedColor(153f, 153f, 153f);
				skillDescrtiption.text = skillDescrtiption.text.Replace("<color=white>", string.Empty).Replace("<color=#FAD725>", string.Empty).Replace("<color=#FFFC2E>", string.Empty)
					.Replace("</color>", string.Empty);
				double num11 = Singleton<SkillManager>.instance.getReinforcementUnlockPrice(skillType);
				reinforcementSkillUnlockPriceText.text = num11.ToString("N0");
				if ((double)Singleton<DataManager>.instance.currentGameData._ruby >= num11 && Singleton<SkillManager>.instance.getSkillInventoryData(skillType).skillLevel >= 100)
				{
					unlockableEffect.Stop();
					unlockableEffect.Play();
				}
				else
				{
					unlockableEffect.Stop();
				}
			}
		}
		else if (m_isPassiveSkill)
		{
			if (!passiveSkillObject.activeSelf)
			{
				passiveSkillObject.SetActive(true);
			}
			if (autoTouchOnObject.activeSelf)
			{
				autoTouchOnObject.SetActive(false);
			}
			if (costObject.activeSelf)
			{
				costObject.SetActive(false);
			}
			if (previewButtonObject.activeSelf)
			{
				previewButtonObject.SetActive(false);
			}
			if (buyAutoTouchObject.activeSelf)
			{
				buyAutoTouchObject.SetActive(false);
			}
			if (transcendSkillObject.activeSelf)
			{
				transcendSkillObject.SetActive(false);
			}
			if (lockObject.activeSelf)
			{
				lockObject.SetActive(false);
			}
			if (upgradeObject.activeSelf)
			{
				upgradeObject.SetActive(false);
			}
			if (reinforcementSkillObject.activeSelf)
			{
				reinforcementSkillObject.SetActive(false);
			}
			realSkillSlotIndex = base.slotIndex - (num + num2 + num3);
			passiveSkillData = Singleton<SkillManager>.instance.getPassiveSkillInventoryData((SkillManager.PassiveSkillType)realSkillSlotIndex);
			SkillManager.PassiveSkillType passiveSkillType = (SkillManager.PassiveSkillType)realSkillSlotIndex;
			int num12 = realSkillSlotIndex + 1;
			skillName.text = I18NManager.Get("PASSIVE_SKILL_NAME_" + num12);
			passiveSkillLevel.text = "Lv. " + passiveSkillData.skillLevel;
			double passiveSkillCastChance = Singleton<SkillManager>.instance.getPassiveSkillCastChance(passiveSkillType);
			float skillDuration = Singleton<SkillManager>.instance.getSkillDuration(passiveSkillType);
			double passiveSkillValue = Singleton<SkillManager>.instance.getPassiveSkillValue(passiveSkillType, false);
			double num13 = passiveSkillValue / 100.0 * Singleton<StatManager>.instance.skillExtraPercentDamage;
			string text = passiveSkillValue.ToString("0.#") + ((!(num13 > 0.0)) ? string.Empty : ("<color=#FFFC2E>(+" + num13.ToString("0.#") + ")</color>"));
			if (passiveSkillType == SkillManager.PassiveSkillType.SwordSoul)
			{
				skillDescrtiption.text = string.Format(I18NManager.Get("PASSIVE_SKILL_DESCRIPTION_" + num12), text);
			}
			else if (skillDuration > 0f)
			{
				skillDescrtiption.text = string.Format(I18NManager.Get("PASSIVE_SKILL_DESCRIPTION_" + num12), passiveSkillCastChance, skillDuration, text);
			}
			else if (passiveSkillType == SkillManager.PassiveSkillType.MeteorRain)
			{
				skillDescrtiption.text = string.Format(I18NManager.Get("PASSIVE_SKILL_DESCRIPTION_" + num12), passiveSkillCastChance, text, Singleton<SkillManager>.instance.getMeteorSpawnCount());
			}
			else
			{
				skillDescrtiption.text = string.Format(I18NManager.Get("PASSIVE_SKILL_DESCRIPTION_" + num12), passiveSkillCastChance, text);
			}
			if (passiveSkillType == SkillManager.PassiveSkillType.SwordSoul)
			{
				for (int i = 0; i < passiveSkillUnlockAndUpgradeResourceImages.Length; i++)
				{
					passiveSkillUnlockAndUpgradeResourceImages[i].sprite = Singleton<CachedManager>.instance.heartCoinIconImage;
					passiveSkillUnlockAndUpgradeResourceImages[i].transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
					passiveSkillUnlockAndUpgradeResourceImages[i].SetNativeSize();
				}
			}
			else
			{
				for (int j = 0; j < passiveSkillUnlockAndUpgradeResourceImages.Length; j++)
				{
					passiveSkillUnlockAndUpgradeResourceImages[j].sprite = Singleton<CachedManager>.instance.rubyIconImage;
					passiveSkillUnlockAndUpgradeResourceImages[j].transform.localScale = Vector3.one;
					passiveSkillUnlockAndUpgradeResourceImages[j].SetNativeSize();
				}
			}
			if (passiveSkillData.isUnlocked)
			{
				if (passiveSkillLockedObject.activeSelf)
				{
					passiveSkillLockedObject.SetActive(false);
				}
				if (!passiveSkillUnlockedObject.activeSelf)
				{
					passiveSkillUnlockedObject.SetActive(true);
				}
				skillIcon.sprite = UIWindowSkill.instance.passiveSkillIcon[realSkillSlotIndex];
				skillName.color = Util.getCalculatedColor(253f, 252f, 183f);
				skillDescrtiption.color = Util.getCalculatedColor(30f, 199f, 255f);
				double num14 = Singleton<SkillManager>.instance.getPassiveSkillUpgradeRubyPrice(passiveSkillType);
				passiveSkillUpgradePriceText.text = num14.ToString("N0");
				if (passiveSkillData.skillLevel >= Singleton<SkillManager>.instance.getPassiveSkillMaxLevel(passiveSkillType))
				{
					passiveSkillUpgradeButtonObject.SetActive(false);
					passiveSkillMaxLevelObject.SetActive(true);
				}
				else
				{
					passiveSkillUpgradeButtonObject.SetActive(true);
					passiveSkillMaxLevelObject.SetActive(false);
				}
			}
			else
			{
				if (!passiveSkillLockedObject.activeSelf)
				{
					passiveSkillLockedObject.SetActive(true);
				}
				if (passiveSkillUnlockedObject.activeSelf)
				{
					passiveSkillUnlockedObject.SetActive(false);
				}
				skillIcon.sprite = UIWindowSkill.instance.passiveSkillLockedIcon[realSkillSlotIndex];
				skillName.color = Util.getCalculatedColor(153f, 153f, 153f);
				skillDescrtiption.color = Util.getCalculatedColor(153f, 153f, 153f);
				skillDescrtiption.text = skillDescrtiption.text.Replace("<color=white>", string.Empty).Replace("<color=#FAD725>", string.Empty).Replace("<color=#FFFC2E>", string.Empty)
					.Replace("</color>", string.Empty);
				double num15 = Singleton<SkillManager>.instance.getPassiveSkillUnlockRubyPrice(passiveSkillType);
				passiveSkillUnlockPriceText.text = num15.ToString("N0");
				if ((double)Singleton<DataManager>.instance.currentGameData._ruby >= num15)
				{
					unlockableEffect.Stop();
					unlockableEffect.Play();
				}
				else
				{
					unlockableEffect.Stop();
				}
			}
		}
		else if (m_isTranscendSkill)
		{
			if (passiveSkillObject.activeSelf)
			{
				passiveSkillObject.SetActive(false);
			}
			if (lockObject.activeSelf)
			{
				lockObject.SetActive(false);
			}
			if (upgradeObject.activeSelf)
			{
				upgradeObject.SetActive(false);
			}
			if (costObject.activeSelf)
			{
				costObject.SetActive(false);
			}
			if (previewButtonObject.activeSelf)
			{
				previewButtonObject.SetActive(false);
			}
			if (!transcendSkillObject.activeSelf)
			{
				transcendSkillObject.SetActive(true);
			}
			if (buyAutoTouchObject.activeSelf)
			{
				buyAutoTouchObject.SetActive(false);
			}
			if (reinforcementSkillObject.activeSelf)
			{
				reinforcementSkillObject.SetActive(false);
			}
			TranscendManager.TranscendPassiveSkillType transcendPassiveSkillType = (TranscendManager.TranscendPassiveSkillType)(base.slotIndex - (num + num2 + num3 + num4));
			CharacterManager.CharacterType key = CharacterManager.CharacterType.Length;
			TranscendPassiveSkillInventoryData transcendPassiveSkillInventoryData = Singleton<TranscendManager>.instance.getTranscendPassiveSkillInventoryData(transcendPassiveSkillType);
			switch (transcendPassiveSkillType)
			{
			case TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage:
				key = CharacterManager.CharacterType.Warrior;
				transcendSkillChanceDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_SKILL_CHANCE_DESCRIPTION"), I18NManager.Get("WARRIOR"));
				transcendSkillUnlockDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_SKILL_UNLOCK_DESCRIPTION"), I18NManager.Get("WARRIOR"));
				break;
			case TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage:
				key = CharacterManager.CharacterType.Priest;
				transcendSkillChanceDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_SKILL_CHANCE_DESCRIPTION"), I18NManager.Get("PRIEST"));
				transcendSkillUnlockDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_SKILL_UNLOCK_DESCRIPTION"), I18NManager.Get("PRIEST"));
				break;
			case TranscendManager.TranscendPassiveSkillType.PenetrationArrow:
				key = CharacterManager.CharacterType.Archer;
				transcendSkillChanceDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_SKILL_CHANCE_DESCRIPTION"), I18NManager.Get("ARCHER"));
				transcendSkillUnlockDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_SKILL_UNLOCK_DESCRIPTION"), I18NManager.Get("ARCHER"));
				break;
			}
			skillName.text = Singleton<TranscendManager>.instance.getTranscendPassiveSkillNameFromI18N(transcendPassiveSkillType);
			skillDescrtiption.text = Singleton<TranscendManager>.instance.getTranscendPassiveSkillDescriptionFromI18N(transcendPassiveSkillType, Math.Max(Singleton<DataManager>.instance.currentGameData.currentTranscendTier[key], 0L), Math.Max(transcendPassiveSkillInventoryData.skillLevel, 0L), !transcendPassiveSkillInventoryData.isUnlocked, 0.0);
			skillIcon.sprite = Singleton<TranscendManager>.instance.getTranscendPassiveSkillIcon(transcendPassiveSkillType, transcendPassiveSkillInventoryData.isUnlocked);
			transcendSkillLockedIconObject.SetActive(!transcendPassiveSkillInventoryData.isUnlocked);
			transcendSkillUppgradeObject.SetActive(transcendPassiveSkillInventoryData.isUnlocked);
			if (transcendPassiveSkillInventoryData.isUnlocked)
			{
				transcendSkillUpgradePriceText.text = Singleton<TranscendManager>.instance.getTranscendSkillUpgradePrice(transcendPassiveSkillType, transcendPassiveSkillInventoryData.skillLevel).ToString("N0");
				transcendSkillLevelText.text = "Lv. " + transcendPassiveSkillInventoryData.skillLevel;
				transcendSkillUnlockDescriptionText.text = string.Empty;
				skillName.color = Util.getCalculatedColor(253f, 252f, 183f);
				skillDescrtiption.color = Util.getCalculatedColor(30f, 199f, 255f);
				transcendSkillChanceDescriptionText.color = Util.getCalculatedColor(250f, 215f, 37f);
			}
			else
			{
				transcendSkillLevelText.text = string.Empty;
				skillName.color = Util.getCalculatedColor(153f, 153f, 153f);
				skillDescrtiption.color = Util.getCalculatedColor(153f, 153f, 153f);
				transcendSkillChanceDescriptionText.color = Util.getCalculatedColor(153f, 153f, 153f);
			}
		}
	}

	public void OnClickUnlockReinforcementSkill()
	{
		if (currentReinforcementSkillInventoryData == null || currentReinforcementSkillInventoryData.isUnlocked)
		{
			return;
		}
		SkillManager.SkillType currentSkillType = currentReinforcementSkillInventoryData.currentSkillType;
		if (Singleton<SkillManager>.instance.getSkillInventoryData(currentSkillType).skillLevel < 100)
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("REQUIRE_LEVEL_FOR_REINFORCEMENT_UNLOCK"), I18NManager.Get("SKILL_NAME_" + (int)(currentSkillType + 1)), 100), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			return;
		}
		double skillUnlockPrice = Singleton<SkillManager>.instance.getReinforcementUnlockPrice(currentReinforcementSkillInventoryData.currentSkillType);
		if ((double)Singleton<DataManager>.instance.currentGameData._ruby >= skillUnlockPrice)
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("ANGEILCA_BUY_QUESTION"), Singleton<SkillManager>.instance.getReinforcementSkillName(currentReinforcementSkillInventoryData.currentSkillType)), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				unlockEffect.Stop();
				unlockEffect.Play();
				Singleton<AudioManager>.instance.playEffectSound("unlock_weapon");
				Singleton<RubyManager>.instance.decreaseRuby(skillUnlockPrice);
				currentReinforcementSkillInventoryData.isUnlocked = true;
				Singleton<DataManager>.instance.saveData();
				refreshSlot();
				int num = 0;
				foreach (KeyValuePair<SkillManager.SkillType, ReinforcementSkillInventoryData> reinforcementSkillInventoryDatum in Singleton<DataManager>.instance.currentGameData.reinforcementSkillInventoryData)
				{
					if (reinforcementSkillInventoryDatum.Value.isUnlocked)
					{
						num++;
					}
				}
				if (num >= 5 && Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.ReinforcementSkillPackage.ToString()))
				{
					Singleton<ShopManager>.instance.consumeLimitedItem(Singleton<ShopManager>.instance.getLimitedItemDataFromCurrentInventory(ShopManager.LimitedShopItemType.ReinforcementSkillPackage.ToString()));
				}
			}, string.Empty);
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}

	public void OnClickUpgradeReinforcementSkill()
	{
		if (currentReinforcementSkillInventoryData == null || !currentReinforcementSkillInventoryData.isUnlocked)
		{
			return;
		}
		if (currentReinforcementSkillInventoryData.skillLevel >= Singleton<SkillManager>.instance.getReinforcementMaxLevel())
		{
			UIWindowDialog.openDescription("ALREADY_MAX_LEVEL", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			return;
		}
		SkillManager.SkillType currentSkillType = currentReinforcementSkillInventoryData.currentSkillType;
		if (Singleton<SkillManager>.instance.getSkillInventoryData(currentSkillType).skillLevel < 100 * currentReinforcementSkillInventoryData.skillLevel + 100)
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("REQUIRE_LEVEL_FOR_REINFORCEMENT_UPGRADE"), I18NManager.Get("SKILL_NAME_" + (int)(currentSkillType + 1)), 100 * currentReinforcementSkillInventoryData.skillLevel + 100), UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			return;
		}
		long reinforcementUpgradePrice = Singleton<SkillManager>.instance.getReinforcementUpgradePrice(currentReinforcementSkillInventoryData.currentSkillType, currentReinforcementSkillInventoryData.skillLevel);
		if (Singleton<DataManager>.instance.currentGameData._ruby >= reinforcementUpgradePrice)
		{
			upgradeEffect.Stop();
			upgradeEffect.Play();
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			Singleton<RubyManager>.instance.decreaseRuby(reinforcementUpgradePrice);
			currentReinforcementSkillInventoryData.skillLevel++;
			refreshSlot();
			Singleton<DataManager>.instance.saveData();
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
		}
	}

	public void OnClickUpgradeTranscendSkill()
	{
		int num = 5;
		int num2 = 5;
		int num3 = 5;
		int num4 = 3;
		TranscendManager.TranscendPassiveSkillType skillType = (TranscendManager.TranscendPassiveSkillType)(slotIndex - (num + num2 + num3 + num4));
		TranscendPassiveSkillInventoryData transcendPassiveSkillInventoryData = Singleton<TranscendManager>.instance.getTranscendPassiveSkillInventoryData(skillType);
		long transcendSkillUpgradePrice = Singleton<TranscendManager>.instance.getTranscendSkillUpgradePrice(skillType, transcendPassiveSkillInventoryData.skillLevel);
		if (Singleton<DataManager>.instance.currentGameData._ruby >= transcendSkillUpgradePrice)
		{
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			Singleton<RubyManager>.instance.decreaseRuby(transcendSkillUpgradePrice);
			upgradeEffect.Stop();
			upgradeEffect.Play();
			transcendPassiveSkillInventoryData.skillLevel++;
			Singleton<DataManager>.instance.saveData();
			refreshSlot();
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}

	public void OnClickUnlockSkill()
	{
		if (skillData == null || skillData.isHasSkill)
		{
			return;
		}
		double skillUnlockGoldPrice = Singleton<SkillManager>.instance.getSkillUnlockGoldPrice(skillData.skillType);
		if (Singleton<DataManager>.instance.currentGameData.gold >= skillUnlockGoldPrice)
		{
			unlockEffect.Stop();
			unlockEffect.Play();
			Singleton<AudioManager>.instance.playEffectSound("unlock_weapon");
			Singleton<GoldManager>.instance.decreaseGold(skillUnlockGoldPrice);
			skillData.isHasSkill = true;
			Singleton<DataManager>.instance.saveData();
			refreshSlot();
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
			UIWindowDialog.openDescription("NOT_ENOUGH_GOLD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
				UIWindowPopupShop.instance.focusToGold();
			}, string.Empty);
		}
	}

	private void OnEnable()
	{
		closeQuickUpgradeButton(true);
	}

	public void OnClickSkillPreview()
	{
		if (skillData != null)
		{
			UIWindowSkill.instance.OnClickPreview(skillData);
		}
	}

	public void OnClickSkillUpgrade()
	{
		if (skillData != null && skillData.isHasSkill)
		{
			double price = Singleton<SkillManager>.instance.getSkillUpgradePrice((SkillManager.SkillType)realSkillSlotIndex, skillData.skillLevel);
			upgradeSkill(price);
		}
	}

	private void upgradeSkill(double price, int increaseLevel = 1)
	{
		if (Singleton<DataManager>.instance.currentGameData.gold >= price)
		{
			upgradeEffect.Stop();
			upgradeEffect.Play();
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			Singleton<GoldManager>.instance.decreaseGold(price);
			skillData.skillLevel += increaseLevel;
			refreshSlot();
			Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.SkillfulStrike, increaseLevel);
			Singleton<DataManager>.instance.saveData();
			checkQuickUpgrade();
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_GOLD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
				UIWindowPopupShop.instance.focusToGold();
			}, string.Empty);
			cantUpgradeButtonAnimation.Play();
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
		}
		UIWindowSkill.instance.currentSkillData = skillData;
	}

	public void OnClickBuyAutoTouchSkill()
	{
		if (!m_isCustomSkill)
		{
			return;
		}
		SkillManager.CustomSkillType slotIndex = (SkillManager.CustomSkillType)base.slotIndex;
		long autoTouchSkillBuyPrice = Singleton<SkillManager>.instance.getAutoTouchSkillBuyPrice(slotIndex);
		if (Singleton<DataManager>.instance.currentGameData._ruby >= autoTouchSkillBuyPrice)
		{
			long num = 0L;
			switch (slotIndex)
			{
			case SkillManager.CustomSkillType.TimerSilverFinger:
				if (Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime > UnbiasedTime.Instance.Now().Ticks)
				{
					num = new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime).AddMinutes(60.0).Ticks;
				}
				else
				{
					DateTime dateTime2 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
					num = dateTime2.AddMinutes(60.0).Ticks;
				}
				Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime = num;
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.SkillSilverFinger);
				break;
			case SkillManager.CustomSkillType.TimerGoldFinger:
				if (Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime > UnbiasedTime.Instance.Now().Ticks)
				{
					num = new DateTime(Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime).AddMinutes(60.0).Ticks;
				}
				else
				{
					DateTime dateTime3 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
					num = dateTime3.AddMinutes(60.0).Ticks;
				}
				Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime = num;
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.SkillGoldFinger);
				break;
			case SkillManager.CustomSkillType.CountGoldFinger:
				Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount += 50;
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.SkillGoldFingerCount);
				break;
			case SkillManager.CustomSkillType.TimerAutoOpenTreasureChest:
				if (Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime > UnbiasedTime.Instance.Now().Ticks)
				{
					num = new DateTime(Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime).AddMinutes(60.0).Ticks;
				}
				else
				{
					DateTime dateTime4 = new DateTime(UnbiasedTime.Instance.Now().Ticks);
					num = dateTime4.AddMinutes(60.0).Ticks;
				}
				Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime = num;
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.AutoOpenTreasureChest);
				Singleton<TreasureChestManager>.instance.refreshAutoOpenTreasureChest();
				break;
			case SkillManager.CustomSkillType.DoubleSpeed:
				if (Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime > UnbiasedTime.Instance.Now().Ticks)
				{
					num = new DateTime(Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime).AddMinutes(30.0).Ticks;
				}
				else
				{
					DateTime dateTime = new DateTime(UnbiasedTime.Instance.Now().Ticks);
					num = dateTime.AddMinutes(30.0).Ticks;
				}
				Singleton<DataManager>.instance.currentGameData.doubleSpeedEndTime = num;
				Singleton<GameManager>.instance.refreshTimeScaleMiniPopup();
				AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.DoubleSpeed);
				break;
			}
			upgradeEffect.Stop();
			upgradeEffect.Play();
			Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			Singleton<RubyManager>.instance.decreaseRuby(autoTouchSkillBuyPrice);
			Singleton<DataManager>.instance.saveData();
			refreshSlot();
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}

	public void OnClickUnlockPassiveSkill()
	{
		int num = 5;
		int num2 = 5;
		int num3 = 5;
		SkillManager.PassiveSkillType currentPassiveSkillType = (SkillManager.PassiveSkillType)(slotIndex - (num + num2 + num3));
		PassiveSkillInventoryData passiveSkillData = Singleton<SkillManager>.instance.getPassiveSkillInventoryData(currentPassiveSkillType);
		double skillUnlockPrice = Singleton<SkillManager>.instance.getPassiveSkillUnlockRubyPrice(currentPassiveSkillType);
		if ((double)((currentPassiveSkillType != SkillManager.PassiveSkillType.SwordSoul) ? Singleton<DataManager>.instance.currentGameData._ruby : Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode) >= skillUnlockPrice)
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("ANGEILCA_BUY_QUESTION"), I18NManager.Get("PASSIVE_SKILL_NAME_" + (int)(currentPassiveSkillType + 1))), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				if ((currentPassiveSkillType == SkillManager.PassiveSkillType.FrostSkill || currentPassiveSkillType == SkillManager.PassiveSkillType.MeteorRain) && Singleton<ShopManager>.instance.isContainLimitedItemFromCurrentInventory(ShopManager.LimitedShopItemType.IceMeteorSkillPackage.ToString()))
				{
					Singleton<ShopManager>.instance.consumeLimitedItem(Singleton<ShopManager>.instance.getLimitedItemDataFromCurrentInventory(ShopManager.LimitedShopItemType.IceMeteorSkillPackage.ToString()));
				}
				unlockEffect.Stop();
				unlockEffect.Play();
				Singleton<AudioManager>.instance.playEffectSound("unlock_weapon");
				if (currentPassiveSkillType == SkillManager.PassiveSkillType.SwordSoul)
				{
					Singleton<ElopeModeManager>.instance.decreaseHeartCoin((long)skillUnlockPrice);
				}
				else
				{
					Singleton<RubyManager>.instance.decreaseRuby(skillUnlockPrice);
				}
				passiveSkillData.isUnlocked = true;
				Singleton<DataManager>.instance.saveData();
				refreshSlot();
			}, string.Empty);
			return;
		}
		Singleton<AudioManager>.instance.playEffectSound("btn_fail");
		if (currentPassiveSkillType == SkillManager.PassiveSkillType.SwordSoul)
		{
			string arg = I18NManager.Get("ELOPE_HEART_COIN");
			UIWindowDialog.openMiniDialogWithoutI18N(string.Format(I18NManager.Get("NOT_ENOUGH"), arg) + " \n<size=22><color=#FDFCB7>" + I18NManager.Get("ELOPE_SHOP_DESCIPRIOTN_2") + "</color></size>");
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}

	public void OnClickUpgradePassiveSkill()
	{
		int num = 5;
		int num2 = 5;
		int num3 = 5;
		SkillManager.PassiveSkillType passiveSkillType = (SkillManager.PassiveSkillType)(slotIndex - (num + num2 + num3));
		PassiveSkillInventoryData passiveSkillInventoryData = Singleton<SkillManager>.instance.getPassiveSkillInventoryData(passiveSkillType);
		long passiveSkillUpgradeRubyPrice = Singleton<SkillManager>.instance.getPassiveSkillUpgradeRubyPrice(passiveSkillType);
		if (((passiveSkillType != SkillManager.PassiveSkillType.SwordSoul) ? Singleton<DataManager>.instance.currentGameData._ruby : Singleton<DataManager>.instance.currentGameData.heartCoinForElopeMode) >= passiveSkillUpgradeRubyPrice)
		{
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			if (passiveSkillType == SkillManager.PassiveSkillType.SwordSoul)
			{
				Singleton<ElopeModeManager>.instance.decreaseHeartCoin(passiveSkillUpgradeRubyPrice);
			}
			else
			{
				Singleton<RubyManager>.instance.decreaseRuby(passiveSkillUpgradeRubyPrice);
			}
			upgradeEffect.Stop();
			upgradeEffect.Play();
			passiveSkillInventoryData.skillLevel++;
			Singleton<DataManager>.instance.saveData();
			refreshSlot();
			return;
		}
		Singleton<AudioManager>.instance.playEffectSound("btn_fail");
		if (passiveSkillType == SkillManager.PassiveSkillType.SwordSoul)
		{
			string arg = I18NManager.Get("ELOPE_HEART_COIN");
			UIWindowDialog.openMiniDialogWithoutI18N(string.Format(I18NManager.Get("NOT_ENOUGH"), arg) + " \n<size=22><color=#FDFCB7>" + I18NManager.Get("ELOPE_SHOP_DESCIPRIOTN_2") + "</color></size>");
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}

	public void OnClickQuickUpgrade()
	{
		long num = skillData.skillLevel;
		upgradeSkill(getCalculatedQuickUpgradePrice(), 10);
	}

	private double getCalculatedQuickUpgradePrice()
	{
		double num = 0.0;
		long num2 = skillData.skillLevel;
		for (long num3 = num2; num3 < num2 + 10; num3++)
		{
			num += Singleton<SkillManager>.instance.getSkillUpgradePrice(skillData.skillType, (int)num3);
		}
		return num;
	}

	private void checkQuickUpgrade()
	{
		long num = skillData.skillLevel;
		if (TutorialManager.isTutorial || TutorialManager.isRebirthTutorial || !base.cachedGameObject.activeSelf || !base.cachedGameObject.activeInHierarchy)
		{
			closeQuickUpgradeButton();
		}
		else if (Singleton<DataManager>.instance.currentGameData.gold >= getCalculatedQuickUpgradePrice())
		{
			m_quickUpgradeDisappearTimer = 0f;
			openQuickUpgradeButton();
			StopCoroutine("waitForCloseAnimation");
			StopCoroutine("quickUpgradeButtonDisappearUpdate");
			StartCoroutine("quickUpgradeButtonDisappearUpdate");
		}
		else
		{
			closeQuickUpgradeButton();
		}
	}

	private void openQuickUpgradeButton()
	{
		quickUpgradeTotalPriceText.text = GameManager.changeUnit(getCalculatedQuickUpgradePrice());
		if (!isQuickUpgrading)
		{
			quickUpgradeButtonObject.SetActive(false);
			quickUpgradeButtonObject.SetActive(true);
			isQuickUpgrading = true;
		}
	}

	private void closeQuickUpgradeButton(bool forceClose = false)
	{
		StopCoroutine("quickUpgradeButtonDisappearUpdate");
		m_quickUpgradeDisappearTimer = 0f;
		if (quickUpgradeButtonObject.activeSelf)
		{
			if (!forceClose)
			{
				if (!quickUpgradeButtonAnimation.IsPlaying("WeaponQuickUpgradeButtonCloseAnimation"))
				{
					quickUpgradeButtonAnimation.Stop();
					quickUpgradeButtonAnimation.Play("WeaponQuickUpgradeButtonCloseAnimation");
					StopCoroutine("waitForCloseAnimation");
					StartCoroutine("waitForCloseAnimation");
				}
			}
			else
			{
				quickUpgradeButtonObject.SetActive(false);
			}
		}
		isQuickUpgrading = false;
	}

	private IEnumerator waitForCloseAnimation()
	{
		yield return new WaitWhile(() => quickUpgradeButtonAnimation.isPlaying);
		quickUpgradeButtonObject.SetActive(false);
	}

	private IEnumerator quickUpgradeButtonDisappearUpdate()
	{
		while (m_quickUpgradeDisappearTimer < 1.5f)
		{
			m_quickUpgradeDisappearTimer += Time.deltaTime;
			yield return null;
		}
		closeQuickUpgradeButton();
	}
}
