using System;
using UnityEngine;
using UnityEngine.UI;

public class ElopePrincessSlot : ScrollSlotItem
{
	public Text nameText;

	public Text currentRemainTimerText;

	public Text currentHeartProductionText;

	public Image currentHeartProductionProgressImage;

	public Text currentLevelText;

	public Text lockedDescriptionText;

	public SpriteAnimation currentPrincessSpriteAnimation;

	public GameObject lockedObject;

	public GameObject unlockedObject;

	public GameObject upgradeButtonObject;

	public GameObject doubleUpgradeButtonObject;

	public GameObject unlockButtonObject;

	public Image doubleUpgradeProgressImage;

	public Text unlockPriceText;

	public Text[] upgradePriceTexts;

	public Text[] increaseTexts;

	public Image[] upgradeResourceIcon;

	public GameObject unlockableEffectObject;

	private int m_currentPrincessIndex;

	private PrincessInventoryDataForElopeMode m_currentPrincessInventoryData;

	private ElopeModeManager.PrincessUpgradeResourceData m_currentUpgradeResourceData;

	public override void UpdateItem(int count)
	{
		base.UpdateItem(count);
		refreshElopePrincessSlot();
	}

	public override void refreshSlot()
	{
		UpdateItem(slotIndex);
	}

	public void startHeartFullEffect()
	{
	}

	private void playPrincessAnimation()
	{
		if (!currentPrincessSpriteAnimation.isPlaying)
		{
			currentPrincessSpriteAnimation.playAnimation("Move", UnityEngine.Random.Range(0.11f, 0.17f), true);
		}
	}

	private void refreshElopePrincessSlot()
	{
		m_currentPrincessIndex = slotIndex + 1;
		m_currentPrincessInventoryData = Singleton<ElopeModeManager>.instance.getPrincessInventoryData(m_currentPrincessIndex);
		string format = I18NManager.Get("PRINCESS_NAME");
		nameText.text = string.Format(format, m_currentPrincessIndex);
		currentPrincessSpriteAnimation.animationType = "Princess" + m_currentPrincessIndex;
		currentPrincessSpriteAnimation.init();
		currentPrincessSpriteAnimation.playFixAnimation("Idle", 0);
		currentPrincessSpriteAnimation.stopAnimation();
		m_currentUpgradeResourceData = Singleton<ElopeModeManager>.instance.getPrincessUpgradeResourceData(m_currentPrincessIndex);
		if (m_currentPrincessIndex < GameManager.getCurrentPrincessNumberForElopeMode() && (m_currentPrincessIndex == 1 || (m_currentPrincessIndex > 1 && Singleton<ElopeModeManager>.instance.getPrincessInventoryData(m_currentPrincessIndex - 1).isUnlocked)))
		{
			if (lockedObject.activeSelf)
			{
				lockedObject.SetActive(false);
			}
			if (!unlockedObject.activeSelf)
			{
				unlockedObject.SetActive(true);
			}
			currentPrincessSpriteAnimation.targetImage.color = new Color(1f, 1f, 1f, 1f);
			currentLevelText.text = "Lv." + (m_currentPrincessInventoryData.princessLevel + 1);
			currentHeartProductionText.text = GameManager.changeUnit(Singleton<ElopeModeManager>.instance.getPrincessProductionIncreaseValue(m_currentPrincessIndex));
			long num;
			for (num = m_currentPrincessInventoryData.princessLevel + 1; num > ElopeModeManager.princessDoubleHeartLevelUnit; num -= ElopeModeManager.princessDoubleHeartLevelUnit)
			{
			}
			doubleUpgradeProgressImage.fillAmount = (float)(num + 1) / (float)ElopeModeManager.princessDoubleHeartLevelUnit;
			if (m_currentPrincessInventoryData.isUnlocked)
			{
				if (unlockButtonObject.activeSelf)
				{
					unlockButtonObject.SetActive(false);
				}
				if ((m_currentPrincessInventoryData.princessLevel + 1) % ElopeModeManager.princessDoubleHeartLevelUnit == 0L)
				{
					if (upgradeButtonObject.activeSelf)
					{
						upgradeButtonObject.SetActive(false);
					}
					if (!doubleUpgradeButtonObject.activeSelf)
					{
						doubleUpgradeButtonObject.SetActive(true);
					}
					if (!unlockableEffectObject.activeSelf)
					{
						unlockableEffectObject.SetActive(true);
					}
				}
				else
				{
					if (!upgradeButtonObject.activeSelf)
					{
						upgradeButtonObject.SetActive(true);
					}
					if (doubleUpgradeButtonObject.activeSelf)
					{
						doubleUpgradeButtonObject.SetActive(false);
					}
					if (unlockableEffectObject.activeSelf)
					{
						unlockableEffectObject.SetActive(false);
					}
				}
				for (int i = 0; i < increaseTexts.Length; i++)
				{
					increaseTexts[i].text = "+" + GameManager.changeUnit(Singleton<ElopeModeManager>.instance.getPrincessProductionIncreaseValue(m_currentPrincessIndex, m_currentPrincessInventoryData.princessLevel + 1) - Singleton<ElopeModeManager>.instance.getPrincessProductionIncreaseValue(m_currentPrincessIndex));
				}
				for (int j = 0; j < upgradePriceTexts.Length; j++)
				{
					upgradePriceTexts[j].text = m_currentUpgradeResourceData.value.ToString("N0");
				}
				for (int k = 0; k < upgradeResourceIcon.Length; k++)
				{
					upgradeResourceIcon[k].sprite = Singleton<ElopeModeManager>.instance.getPrincessUpgradeResourceIcon(m_currentUpgradeResourceData.targetResourceType);
					upgradeResourceIcon[k].SetNativeSize();
				}
				return;
			}
			if (!unlockButtonObject.activeSelf)
			{
				unlockButtonObject.SetActive(true);
			}
			if (upgradeButtonObject.activeSelf)
			{
				upgradeButtonObject.SetActive(false);
			}
			if (doubleUpgradeButtonObject.activeSelf)
			{
				doubleUpgradeButtonObject.SetActive(false);
			}
			long princessUnlockPrice = Singleton<ElopeModeManager>.instance.getPrincessUnlockPrice(m_currentPrincessIndex);
			if (Singleton<DataManager>.instance.currentGameData._ruby >= princessUnlockPrice)
			{
				if (!unlockableEffectObject.activeSelf)
				{
					unlockableEffectObject.SetActive(true);
				}
			}
			else if (unlockableEffectObject.activeSelf)
			{
				unlockableEffectObject.SetActive(false);
			}
			unlockPriceText.text = princessUnlockPrice.ToString("N0");
			currentRemainTimerText.text = "00:00:00";
			currentHeartProductionProgressImage.fillAmount = 0f;
		}
		else
		{
			if (m_currentPrincessIndex >= GameManager.getCurrentPrincessNumberForElopeMode())
			{
				lockedDescriptionText.text = I18NManager.Get("NEED_TO_CLEAR_FOR_UNLOCK_PRINCESS");
			}
			else
			{
				lockedDescriptionText.text = I18NManager.Get("NEED_TO_UNLOCK_PREV_PRINCESS");
			}
			if (!lockedObject.activeSelf)
			{
				lockedObject.SetActive(true);
			}
			if (unlockedObject.activeSelf)
			{
				unlockedObject.SetActive(false);
			}
			currentPrincessSpriteAnimation.targetImage.color = new Color(0f, 0f, 0f, 0.6f);
		}
	}

	private void Update()
	{
		if (m_currentPrincessInventoryData != null && m_currentPrincessInventoryData.isUnlocked)
		{
			playPrincessAnimation();
			float princessMaxProductionTime = Singleton<ElopeModeManager>.instance.getPrincessMaxProductionTime(m_currentPrincessIndex);
			float num = Mathf.Max(princessMaxProductionTime - m_currentPrincessInventoryData.currentTimer, 0f);
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			string empty = string.Empty;
			empty = string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, Mathf.Min((int)((double)timeSpan.Seconds + (double)timeSpan.Milliseconds * 0.001 + 0.5), 59));
			currentRemainTimerText.text = empty;
			currentHeartProductionProgressImage.fillAmount = m_currentPrincessInventoryData.currentTimer / princessMaxProductionTime;
		}
	}

	public void OnClickUnlock()
	{
		UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("ELOPE_PRINCESS_UNLOCK_QUESTION"), m_currentPrincessIndex), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			long princessUnlockPrice = Singleton<ElopeModeManager>.instance.getPrincessUnlockPrice(m_currentPrincessIndex);
			if (Singleton<DataManager>.instance.currentGameData._ruby >= princessUnlockPrice)
			{
				Singleton<ElopeModeManager>.instance.playRandomPrincessEffect(false);
				Singleton<AudioManager>.instance.playEffectSound("unlock_princess");
				Singleton<RubyManager>.instance.decreaseRuby(princessUnlockPrice);
				m_currentPrincessInventoryData.isUnlocked = true;
				Singleton<DataManager>.instance.saveData();
				Singleton<ElopeModeManager>.instance.spawnCart();
				UIWindowElopeMode.instance.elopePrincessScrollRect.refreshAll();
				ObjectPool.Spawn("@ElopeUpgradeEffect", currentPrincessSpriteAnimation.cachedTransform.position, base.cachedTransform, true);
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
				}, string.Empty);
			}
		}, string.Empty);
	}

	public void OnClickUpgrade()
	{
		long value = m_currentUpgradeResourceData.value;
		if (Singleton<ElopeModeManager>.instance.getCurrentResourceValueFromInventory(m_currentUpgradeResourceData.targetResourceType) >= value)
		{
			ObjectPool.Spawn("@ElopeUpgradeEffect", currentPrincessSpriteAnimation.cachedTransform.position, base.cachedTransform, true);
			Singleton<ElopeModeManager>.instance.decreaseResource(m_currentUpgradeResourceData.targetResourceType, value);
			if ((m_currentPrincessInventoryData.princessLevel + 1) % ElopeModeManager.princessDoubleHeartLevelUnit == 0L)
			{
				Singleton<AudioManager>.instance.playEffectSound("unlock_princess");
			}
			else
			{
				Singleton<AudioManager>.instance.playEffectSound("upgrade_princess");
			}
			m_currentPrincessInventoryData.princessLevel++;
			refreshSlot();
		}
		else if (m_currentUpgradeResourceData.targetResourceType != DropItemManager.DropItemType.Ruby)
		{
			string resourceI18NName = Singleton<ElopeModeManager>.instance.getResourceI18NName(m_currentUpgradeResourceData.targetResourceType);
			UIWindowDialog.openMiniDialogWithoutI18N(string.Format(I18NManager.Get("NOT_ENOUGH_RESOURCE"), resourceI18NName, resourceI18NName));
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}
}
