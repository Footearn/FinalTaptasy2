using System;
using System.Collections.Generic;
using UnityEngine;

public class AdsAngelManager : Singleton<AdsAngelManager>
{
	public enum AngelRewardType
	{
		Gold,
		AutoTouch,
		AutoOpenTreasureChest,
		NULL
	}

	public enum SpecialAngelRewardType
	{
		None = -1,
		Damage,
		Health,
		Length
	}

	public bool isCanSpawnAdsAngel;

	public Sprite fastTouchSprite;

	public Sprite doubleAttackDamageSprite;

	public AdsAngelObject currentAngelObject;

	public AdsAngelObject currentSpecialAngelObject;

	public double targetRewardGoldValue;

	public RectTransform rewardButtonTransform;

	public long adsAngelBuffTime;

	public AngelRewardType currentAngelRewardType;

	public bool isProgressingBuff;

	private bool m_isOpenBuffUIForIngame;

	public RectTransform targetSpawnCenterObjectTransform;

	public Canvas targetSpawnCenterObjectCanvas;

	public List<SpecialAngelRewardType> currentSpecialAngelRewardTypeList = new List<SpecialAngelRewardType>();

	private void Awake()
	{
		adsAngelBuffTime = 0L;
	}

	private void Start()
	{
		if (UnbiasedTime.Instance.Now().Ticks > Singleton<DataManager>.instance.currentGameData.adsAngelSpawnTime && !TutorialManager.isTutorial)
		{
			isCanSpawnAdsAngel = true;
		}
		else
		{
			isCanSpawnAdsAngel = true;
		}
	}

	public void startGame()
	{
		if (currentAngelObject != null)
		{
			refreshSpawnTime();
			Singleton<DataManager>.instance.saveData();
		}
		recycleAngel();
	}

	public void endGame()
	{
	}

	public void setDefaultAdsAngelTimer()
	{
		Singleton<DataManager>.instance.currentGameData.adsAngelSpawnTime = UnbiasedTime.Instance.Now().AddMinutes(1.0).Ticks;
		Singleton<DataManager>.instance.saveData();
	}

	public void spawnAdsAngel()
	{
		if (Singleton<AdsManager>.instance.isReady("adsAngel") && !(ParsingManager.adsAngelAppearMinTime <= 0f) && !(ParsingManager.adsAngelAppearMaxTime <= 0f) && (GameManager.currentGameState != 0 || UIWindowResult.instance.isOpen))
		{
			endTimeEvent(false);
			if (currentAngelObject != null)
			{
				recycleAngel();
			}
			isProgressingBuff = false;
			if (GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				targetSpawnCenterObjectCanvas.sortingOrder = 16;
			}
			else
			{
				targetSpawnCenterObjectCanvas.sortingOrder = 101;
			}
			AdsAngelObject adsAngelObject = (currentAngelObject = ObjectPool.Spawn("@AdsAngel", Vector2.zero, targetSpawnCenterObjectTransform).GetComponent<AdsAngelObject>());
			currentAngelRewardType = getRandomAngelRewardType();
			adsAngelObject.initAdsAngel(currentAngelRewardType);
			switch (currentAngelRewardType)
			{
			case AngelRewardType.Gold:
				targetRewardGoldValue = CalculateManager.getCurrentStandardGold() * Singleton<BalanceManager>.instance.getVariableValue(BalanceManager.VariableType.AdsAngelGoldMultiply);
				break;
			case AngelRewardType.AutoTouch:
			case AngelRewardType.AutoOpenTreasureChest:
				targetRewardGoldValue = 0.0;
				break;
			}
		}
	}

	public void recycleAngel()
	{
		if (currentAngelObject != null)
		{
			ObjectPool.Recycle(currentAngelObject.name, currentAngelObject.cachedGameObject);
			currentAngelObject = null;
		}
	}

	public void recycleSpecialAngel()
	{
		if (currentSpecialAngelObject != null)
		{
			ObjectPool.Recycle(currentSpecialAngelObject.name, currentSpecialAngelObject.cachedGameObject);
			currentSpecialAngelObject = null;
		}
	}

	public void refreshSpawnTime()
	{
		if (currentAngelRewardType == AngelRewardType.AutoTouch)
		{
			Singleton<DataManager>.instance.currentGameData.adsAngelSpawnTime = UnbiasedTime.Instance.Now().AddMinutes(ParsingManager.adsAngelAppearMinTime).Ticks;
		}
		else
		{
			Singleton<DataManager>.instance.currentGameData.adsAngelSpawnTime = UnbiasedTime.Instance.Now().AddMinutes(UnityEngine.Random.Range(ParsingManager.adsAngelAppearMinTime, ParsingManager.adsAngelAppearMaxTime)).Ticks;
		}
	}

	public void endTimeEvent(bool isRefreshSpawnTime = true)
	{
		if (isProgressingBuff)
		{
			if (isRefreshSpawnTime)
			{
				refreshSpawnTime();
			}
			Singleton<DataManager>.instance.saveData();
			adsAngelBuffTime = 0L;
			currentAngelRewardType = AngelRewardType.NULL;
			isProgressingBuff = false;
			Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
		}
	}

	public void rewardEvent(AngelRewardType targetRewardType)
	{
		isProgressingBuff = true;
		currentAngelRewardType = targetRewardType;
		switch (targetRewardType)
		{
		case AngelRewardType.Gold:
			adsAngelBuffTime = 0L;
			Singleton<GoldManager>.instance.increaseGold(targetRewardGoldValue, true);
			Singleton<FlyResourcesManager>.instance.playEffectResources(rewardButtonTransform, FlyResourcesManager.ResourceType.Gold, 20L, 0.02f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("getgold");
			});
			UIWindowManageHeroAndWeapon.instance.refreshAllBuyState();
			targetRewardGoldValue = 0.0;
			endTimeEvent();
			break;
		case AngelRewardType.AutoTouch:
			adsAngelBuffTime = UnbiasedTime.Instance.Now().AddMinutes(ParsingManager.adsAngelFastTouchDuringTime).Ticks;
			if (new DateTime(adsAngelBuffTime).Subtract(UnbiasedTime.Instance.Now()).TotalMinutes > (double)ParsingManager.adsAngelFastTouchDuringTime)
			{
				adsAngelBuffTime = UnbiasedTime.Instance.Now().AddMinutes(ParsingManager.adsAngelFastTouchDuringTime).Ticks;
			}
			Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
			break;
		case AngelRewardType.AutoOpenTreasureChest:
			adsAngelBuffTime = UnbiasedTime.Instance.Now().AddMinutes(ParsingManager.adsAngelAutoOpenTreasureChestDuringTime).Ticks;
			if (new DateTime(adsAngelBuffTime).Subtract(UnbiasedTime.Instance.Now()).TotalMinutes > (double)ParsingManager.adsAngelAutoOpenTreasureChestDuringTime)
			{
				adsAngelBuffTime = UnbiasedTime.Instance.Now().AddMinutes(ParsingManager.adsAngelAutoOpenTreasureChestDuringTime).Ticks;
			}
			Singleton<TreasureChestManager>.instance.refreshAutoOpenTreasureChest();
			break;
		}
	}

	private AngelRewardType getRandomAngelRewardType(bool withoutAutoTouch = false)
	{
		float num = 35f;
		float num2 = (withoutAutoTouch ? 0f : 35f);
		float num3 = ((Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime <= UnbiasedTime.Instance.Now().Ticks) ? 30f : 0f);
		float num4 = UnityEngine.Random.Range(0f, num + num2 + num3);
		if (num4 < num)
		{
			return AngelRewardType.Gold;
		}
		if (num4 < num + num2)
		{
			if (Singleton<DataManager>.instance.currentGameData.autoTouch26TapEndTime > UnbiasedTime.Instance.Now().Ticks || Singleton<DataManager>.instance.currentGameData.countableGoldFingerLeftCount > 0)
			{
				return getRandomAngelRewardType(withoutAutoTouch);
			}
			return AngelRewardType.AutoTouch;
		}
		return AngelRewardType.AutoOpenTreasureChest;
	}

	public void checkSpawnAngel()
	{
		if (UnbiasedTime.Instance.Now().Ticks > Singleton<DataManager>.instance.currentGameData.adsAngelSpawnTime && !TutorialManager.isTutorial && !isProgressingBuff)
		{
			isCanSpawnAdsAngel = true;
		}
		else
		{
			isCanSpawnAdsAngel = false;
		}
		if (UnbiasedTime.Instance.Now().Ticks > Singleton<DataManager>.instance.currentGameData.adsAngelSpawnTime && !TutorialManager.isTutorial && !isProgressingBuff && isCanSpawnAdsAngel)
		{
			spawnAdsAngel();
		}
		if (UnbiasedTime.Instance.Now().Ticks > Singleton<DataManager>.instance.currentGameData.specialAdsAngelSpawnTime && !TutorialManager.isTutorial)
		{
			spawnSpcialAdsAngel();
		}
	}

	public void spawnSpcialAdsAngel()
	{
		if (Singleton<AdsManager>.instance.isReady("adsAngel") && (GameManager.currentGameState != 0 || UIWindowResult.instance.isOpen))
		{
			if (GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				targetSpawnCenterObjectCanvas.sortingOrder = 16;
			}
			else
			{
				targetSpawnCenterObjectCanvas.sortingOrder = 101;
			}
			Singleton<DataManager>.instance.currentGameData.specialAdsAngelSpawnTime = UnbiasedTime.Instance.Now().AddMinutes(16.0).Ticks;
			SpecialAngelRewardType specialAngelRewardType = SpecialAngelRewardType.None;
			specialAngelRewardType = ((currentSpecialAngelRewardTypeList.Contains(SpecialAngelRewardType.Damage) && !currentSpecialAngelRewardTypeList.Contains(SpecialAngelRewardType.Health)) ? SpecialAngelRewardType.Health : ((currentSpecialAngelRewardTypeList.Contains(SpecialAngelRewardType.Damage) || !currentSpecialAngelRewardTypeList.Contains(SpecialAngelRewardType.Health)) ? ((UnityEngine.Random.Range(0, 100) % 2 != 0) ? SpecialAngelRewardType.Health : SpecialAngelRewardType.Damage) : SpecialAngelRewardType.Damage));
			(currentSpecialAngelObject = ObjectPool.Spawn("@AdsAngel", Vector2.zero, targetSpawnCenterObjectTransform).GetComponent<AdsAngelObject>()).initAdsAngel(specialAngelRewardType);
		}
	}

	public void rewardEvent(SpecialAngelRewardType targetRewardType)
	{
		if (!currentSpecialAngelRewardTypeList.Contains(targetRewardType))
		{
			currentSpecialAngelRewardTypeList.Add(targetRewardType);
		}
		switch (targetRewardType)
		{
		case SpecialAngelRewardType.Damage:
			if (Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.SpecialAdsAngelDamage))
			{
				Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.SpecialAdsAngelDamage).closeMiniPopupObject(false);
			}
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.SpecialAdsAngelDamage, false, UnbiasedTime.Instance.Now().AddMinutes(30.0), delegate
			{
				if (currentSpecialAngelRewardTypeList.Contains(targetRewardType))
				{
					currentSpecialAngelRewardTypeList.Remove(targetRewardType);
				}
				refreshSpecialAdsAngelStat();
			});
			break;
		case SpecialAngelRewardType.Health:
			if (Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.SpecialAdsAngelArmor))
			{
				Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.SpecialAdsAngelArmor).closeMiniPopupObject(false);
			}
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.SpecialAdsAngelArmor, false, UnbiasedTime.Instance.Now().AddMinutes(30.0), delegate
			{
				if (currentSpecialAngelRewardTypeList.Contains(targetRewardType))
				{
					currentSpecialAngelRewardTypeList.Remove(targetRewardType);
				}
				refreshSpecialAdsAngelStat();
			});
			break;
		}
		refreshSpecialAdsAngelStat();
	}

	public void refreshSpecialAdsAngelStat()
	{
		Singleton<StatManager>.instance.specialAdsAngelDamage = 0.0;
		Singleton<StatManager>.instance.specialAdsAngelHealth = 0.0;
		for (int i = 0; i < currentSpecialAngelRewardTypeList.Count; i++)
		{
			switch (currentSpecialAngelRewardTypeList[i])
			{
			case SpecialAngelRewardType.Damage:
				Singleton<StatManager>.instance.specialAdsAngelDamage = 15.0;
				break;
			case SpecialAngelRewardType.Health:
				Singleton<StatManager>.instance.specialAdsAngelHealth = 15.0;
				break;
			}
		}
	}

	private void Update()
	{
		if (Singleton<DataManager>.instance.isLoadedData && !(ParsingManager.adsAngelAppearMinTime <= 0f) && !(ParsingManager.adsAngelAppearMaxTime <= 0f))
		{
			if (UnbiasedTime.Instance.Now().Ticks > Singleton<DataManager>.instance.currentGameData.adsAngelSpawnTime && !TutorialManager.isTutorial && GameManager.currentGameState == GameManager.GameState.OutGame && !isProgressingBuff)
			{
				isCanSpawnAdsAngel = true;
			}
			else
			{
				isCanSpawnAdsAngel = false;
			}
		}
	}
}
