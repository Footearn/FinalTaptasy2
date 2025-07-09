using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChestManager : Singleton<TreasureChestManager>
{
	public enum TreasureType
	{
		BronzeTreasure,
		SilverTreasure,
		GoldTreasure,
		PlatinumTreasure,
		TranscendTreasure,
		GoblinTreasure
	}

	public enum DropItemType
	{
		Gold,
		Ruby,
		TreasurePiece,
		TranscendStone,
		ElopeResource
	}

	public enum AutoTreasureChestOpenType
	{
		CountAutoOpenTreasureChest,
		TimerAutoOpenTreasureChest,
		FromAdsAngel,
		None
	}

	public int maxTouchCount = 3;

	public int maxDropItemCount = 5;

	public List<TreasureChestObject> currentTreasureObjects;

	public Sprite bronzeTreasureSprite;

	public Sprite bronzeTreasureOpenSprite;

	public Sprite silverTreasureSprite;

	public Sprite silverTreasureOpenSprite;

	public Sprite goldTreasureSprite;

	public Sprite goldTreasureOpenSprite;

	public Sprite platinumTreasureSprite;

	public Sprite platinumTreasureOpenSprite;

	public Sprite transcendTreasureSprite;

	public Sprite transcendTreasureOpenSprite;

	public Sprite goblinTreasureSprite;

	public Sprite goblinTreasureOpenSprite;

	public TreasureChestObject currentBossTreasureChest;

	public AutoTreasureChestOpenType currentAutoTreasureChestOpenType;

	public bool isAutoOpenTreasureChest;

	private bool m_isAlreadyIncreassedAttackDamageFromDestroyChest;

	private float m_increaseAttackSpeedTimer;

	private void Awake()
	{
		currentTreasureObjects = new List<TreasureChestObject>();
	}

	public void recycleBossTreasureChest()
	{
		if (currentBossTreasureChest != null)
		{
			currentBossTreasureChest.recycleBossTreasureChest();
			currentBossTreasureChest = null;
		}
	}

	public void startGame()
	{
		StopCoroutine("increaseAllAttackSpeedFromDestroyChestTreasure");
		m_increaseAttackSpeedTimer = 0f;
		m_isAlreadyIncreassedAttackDamageFromDestroyChest = false;
		clearTreasureChestList();
		refreshAutoOpenTreasureChest();
		if (Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount <= 0 && Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(MiniPopupManager.MiniPopupType.CountAutoOpenTreasureChest))
		{
			Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(MiniPopupManager.MiniPopupType.CountAutoOpenTreasureChest).closeMiniPopupObject();
		}
		if (currentAutoTreasureChestOpenType == AutoTreasureChestOpenType.CountAutoOpenTreasureChest && Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount >= 1)
		{
			Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount--;
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.CountAutoOpenTreasureChest, Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount, delegate
			{
				refreshAutoOpenTreasureChest();
			});
			Singleton<DataManager>.instance.saveData();
		}
		StopCoroutine("treasureCatchUpdate");
		StartCoroutine("treasureCatchUpdate");
	}

	public void endGame()
	{
		StopCoroutine("treasureCatchUpdate");
		recycleBossTreasureChest();
		clearTreasureChestList();
	}

	private void clearTreasureChestList()
	{
		for (int i = 0; i < currentTreasureObjects.Count; i++)
		{
			if (currentTreasureObjects[i] != null)
			{
				ObjectPool.Recycle(currentTreasureObjects[i].name, currentTreasureObjects[i].cachedGameObject);
			}
		}
		currentTreasureObjects.Clear();
	}

	private void closeAutoOpenTreasureChestMiniPopup(params MiniPopupManager.MiniPopupType[] exceptTypes)
	{
		List<MiniPopupManager.MiniPopupType> list = new List<MiniPopupManager.MiniPopupType>();
		list.Add(MiniPopupManager.MiniPopupType.AdsAngelAutoOpenTreasureChest);
		list.Add(MiniPopupManager.MiniPopupType.CountAutoOpenTreasureChest);
		list.Add(MiniPopupManager.MiniPopupType.TimerAutoOpenTreasureChest);
		if (exceptTypes != null)
		{
			for (int i = 0; i < exceptTypes.Length; i++)
			{
				list.Remove(exceptTypes[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (Singleton<MiniPopupManager>.instance.containFromCurrentMiniPopupList(list[j]))
			{
				Singleton<MiniPopupManager>.instance.getMiniPopupDataFromCurrentPopupList(list[j]).closeMiniPopupObject(false);
			}
		}
	}

	public void refreshAutoOpenTreasureChest()
	{
		currentAutoTreasureChestOpenType = AutoTreasureChestOpenType.None;
		isAutoOpenTreasureChest = false;
		if (Singleton<AdsAngelManager>.instance.currentAngelRewardType == AdsAngelManager.AngelRewardType.AutoOpenTreasureChest && Singleton<AdsAngelManager>.instance.adsAngelBuffTime > UnbiasedTime.Instance.Now().Ticks)
		{
			currentAutoTreasureChestOpenType = AutoTreasureChestOpenType.FromAdsAngel;
			isAutoOpenTreasureChest = true;
			closeAutoOpenTreasureChestMiniPopup(MiniPopupManager.MiniPopupType.AdsAngelAutoOpenTreasureChest);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.AdsAngelAutoOpenTreasureChest, false, new DateTime(Singleton<AdsAngelManager>.instance.adsAngelBuffTime), delegate
			{
				refreshAutoOpenTreasureChest();
				Singleton<AdsAngelManager>.instance.endTimeEvent();
			});
		}
		else if (Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime > UnbiasedTime.Instance.Now().Ticks)
		{
			currentAutoTreasureChestOpenType = AutoTreasureChestOpenType.TimerAutoOpenTreasureChest;
			isAutoOpenTreasureChest = true;
			closeAutoOpenTreasureChestMiniPopup(MiniPopupManager.MiniPopupType.TimerAutoOpenTreasureChest);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.TimerAutoOpenTreasureChest, false, new DateTime(Singleton<DataManager>.instance.currentGameData.timerAutoOpenTreasureChestRemainTime), delegate
			{
				refreshAutoOpenTreasureChest();
			});
		}
		else if (Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount > 0)
		{
			currentAutoTreasureChestOpenType = AutoTreasureChestOpenType.CountAutoOpenTreasureChest;
			isAutoOpenTreasureChest = true;
			closeAutoOpenTreasureChestMiniPopup(MiniPopupManager.MiniPopupType.CountAutoOpenTreasureChest);
			Singleton<MiniPopupManager>.instance.registryMiniPopupData(MiniPopupManager.MiniPopupType.CountAutoOpenTreasureChest, Singleton<DataManager>.instance.currentGameData.countAutoOpenTreasureChestRemainCount, delegate
			{
				refreshAutoOpenTreasureChest();
			});
		}
		else
		{
			closeAutoOpenTreasureChestMiniPopup();
		}
	}

	public void spawnTreasure(Vector2 spawnPosition, double bronze, double silver, double gold, double transcend, double goblinTreasure, bool isRealboss)
	{
		TreasureChestObject component = ObjectPool.Spawn("@TreasureChest", spawnPosition).GetComponent<TreasureChestObject>();
		component.initTreasure(bronze, silver, gold, transcend, goblinTreasure, isRealboss);
	}

	public void treasureRecycleEvent()
	{
		if (Singleton<StatManager>.instance.allPercentAttackDamageWhenDestroyTreasureChest != 0f)
		{
			if (!m_isAlreadyIncreassedAttackDamageFromDestroyChest)
			{
				Singleton<StatManager>.instance.allPercentDamageFromTreasureChest += Singleton<StatManager>.instance.allPercentAttackDamageWhenDestroyTreasureChest;
			}
			m_increaseAttackSpeedTimer = 0f;
			StopCoroutine("increaseAllAttackSpeedFromDestroyChestTreasure");
			StartCoroutine("increaseAllAttackSpeedFromDestroyChestTreasure");
			if (!m_isAlreadyIncreassedAttackDamageFromDestroyChest)
			{
				m_isAlreadyIncreassedAttackDamageFromDestroyChest = true;
				UIWindowIngame.instance.setOpenTreasureCooltimeInformation(TreasureManager.CooltimeTreasureEffectType.DestroyTreasureChest, Singleton<StatManager>.instance.allPercentAttackDamageWhenDestroyTreasureChest);
			}
		}
	}

	private IEnumerator increaseAllAttackSpeedFromDestroyChestTreasure()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				m_increaseAttackSpeedTimer += Time.deltaTime * GameManager.timeScale;
				UIWindowIngame.instance.changeSkillCooltimeData(TreasureManager.CooltimeTreasureEffectType.DestroyTreasureChest, 3f, m_increaseAttackSpeedTimer);
				if (m_increaseAttackSpeedTimer >= 3f)
				{
					break;
				}
			}
			yield return null;
		}
		m_isAlreadyIncreassedAttackDamageFromDestroyChest = false;
		Singleton<StatManager>.instance.allPercentDamageFromTreasureChest -= Singleton<StatManager>.instance.allPercentAttackDamageWhenDestroyTreasureChest;
		UIWindowIngame.instance.closeSkillCooltimeSlot(TreasureManager.CooltimeTreasureEffectType.DestroyTreasureChest);
	}

	private IEnumerator treasureCatchUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause && GameManager.currentGameState == GameManager.GameState.Playing)
			{
				for (int i = 0; i < Input.touches.Length; i++)
				{
					if (Input.touches[i].phase == TouchPhase.Began)
					{
						Vector2 mousePosition = Singleton<CachedManager>.instance.ingameCamera.ScreenToWorldPoint(Input.touches[i].position);
						TreasureChestObject touchedTreasure2 = getNearestTreasure(mousePosition, 1.3f);
						if (touchedTreasure2 != null)
						{
							touchedTreasure2.touchTreasure();
							touchedTreasure2 = null;
						}
					}
				}
			}
			yield return null;
		}
	}

	public TreasureChestObject getNearestTreasure(Vector2 mousePosition, float range)
	{
		TreasureChestObject result = null;
		float num = float.MaxValue;
		for (int i = 0; i < currentTreasureObjects.Count; i++)
		{
			if (!(currentTreasureObjects[i] != null))
			{
				continue;
			}
			float num2 = Vector2.Distance(mousePosition, currentTreasureObjects[i].cachedTransform.position);
			if (num2 <= num)
			{
				num = num2;
				if (num <= range)
				{
					result = currentTreasureObjects[i];
				}
			}
		}
		return result;
	}

	public DropItemType getDropItemType(TreasureType treasureType)
	{
		DropItemType result = DropItemType.Gold;
		double num = UnityEngine.Random.Range(0, 10000);
		num /= 100.0;
		double num2 = 99.6;
		double num3 = 0.4;
		double num4 = 0.0;
		double num5 = 0.52;
		double num6 = 97.3;
		double num7 = 2.7;
		double num8 = 0.0;
		double num9 = 3.51;
		double num10 = 96.25;
		double num11 = 2.0;
		double num12 = 1.75;
		double num13 = 2.6;
		num3 *= (double)(1f + Singleton<StatManager>.instance.rubyDropExtraPercentChance * 0.82f / 100f);
		if (GameManager.currentTheme > 100)
		{
			num3 *= 0.89999997615814209;
			num5 *= 0.89999997615814209;
		}
		num2 = 100.0 - num3 - num4 - num5;
		num7 *= (double)(1f + Singleton<StatManager>.instance.rubyDropExtraPercentChance * 0.82f / 100f);
		if (GameManager.currentTheme > 100)
		{
			num7 *= 0.89999997615814209;
			num9 *= 0.89999997615814209;
		}
		num6 = 100.0 - num7 - num8 - num9;
		num11 *= (double)(1f + Singleton<StatManager>.instance.rubyDropExtraPercentChance * 0.82f / 100f);
		if (GameManager.currentTheme > 100)
		{
			num11 *= 0.89999997615814209;
			num13 *= 0.89999997615814209;
		}
		num10 = 100.0 - num11 - num12 - num13;
		switch (treasureType)
		{
		case TreasureType.BronzeTreasure:
			result = ((!(num < num2)) ? ((num < num3 + num2) ? DropItemType.Ruby : ((num < num3 + num2 + num5) ? (Singleton<ElopeModeManager>.instance.isCanStartElopeMode() ? DropItemType.ElopeResource : DropItemType.Gold) : DropItemType.Gold)) : DropItemType.Gold);
			break;
		case TreasureType.SilverTreasure:
			result = ((!(num < num6)) ? ((num < num7 + num6) ? DropItemType.Ruby : ((num < num7 + num6 + num9) ? (Singleton<ElopeModeManager>.instance.isCanStartElopeMode() ? DropItemType.ElopeResource : DropItemType.Gold) : DropItemType.Gold)) : DropItemType.Gold);
			break;
		case TreasureType.TranscendTreasure:
			result = ((!(num < num10)) ? ((num < num11 + num10) ? DropItemType.Ruby : ((!(num < num11 + num10 + num13)) ? DropItemType.TranscendStone : (Singleton<ElopeModeManager>.instance.isCanStartElopeMode() ? DropItemType.ElopeResource : DropItemType.Gold))) : DropItemType.Gold);
			break;
		}
		return result;
	}
}
