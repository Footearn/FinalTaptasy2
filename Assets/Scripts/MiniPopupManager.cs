using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniPopupManager : Singleton<MiniPopupManager>
{
	public enum MiniPopupType
	{
		None = -1,
		PermanentAutoTouch,
		TimerSilverFinger,
		AdsAngelAutoTouch,
		AdsAngelDamageDouble,
		TreasureDamageForAM,
		TreasureDamageForPM,
		BossRaidAutoTouch,
		TimerGoldFinger,
		CountSilverFinger,
		CountGoldFiner,
		DoubleSpeed,
		AdsAngelAutoOpenTreasureChest,
		CountAutoOpenTreasureChest,
		TimerAutoOpenTreasureChest,
		SpecialAdsAngelDamage,
		SpecialAdsAngelArmor,
		BronzeFinger
	}

	[Serializable]
	public struct MiniPopupIconData
	{
		public MiniPopupType currentPopupType;

		public Sprite icon;
	}

	public List<MiniPopupIconData> currentMiniPopupIconDataList;

	public List<MiniPopupObject> currentMiniPopupObjectList;

	public Transform topLeftMiniPopupParentTransform;

	public Transform topCenterMiniPopupParentTransform;

	private Vector2 m_outgameBaseStartposition = new Vector2(0f, 150f);

	private Vector2 m_outgameBaseTargetposition = new Vector2(0f, -135f);

	private Vector2 m_ingameBaseStartposition = new Vector2(-150f, -170f);

	private Vector2 m_ingameBaseTargetposition = new Vector2(54f, -120f);

	private float m_interval = 95f;

	private int maxCellCount = 3;

	private void Awake()
	{
		currentMiniPopupObjectList = new List<MiniPopupObject>();
	}

	public Sprite getIconSprite(MiniPopupType miniPopupType)
	{
		Sprite result = null;
		for (int i = 0; i < currentMiniPopupIconDataList.Count; i++)
		{
			if (currentMiniPopupIconDataList[i].currentPopupType == miniPopupType)
			{
				result = currentMiniPopupIconDataList[i].icon;
				break;
			}
		}
		return result;
	}

	public void registryMiniPopupData(MiniPopupType miniPopupType, bool isEndlessTime)
	{
		registryMiniPopupData(miniPopupType, isEndlessTime, default(DateTime), null);
	}

	public void registryMiniPopupData(MiniPopupType miniPopupType, bool isEndlessTime, DateTime endTime, Action timeEndAction)
	{
		if (containFromCurrentMiniPopupList(miniPopupType))
		{
			getMiniPopupDataFromCurrentPopupList(miniPopupType).currentTargetEndTime = endTime;
			return;
		}
		bool flag = ((GameManager.currentGameState == GameManager.GameState.Playing) ? true : false);
		Transform parent = ((!flag) ? topCenterMiniPopupParentTransform : topLeftMiniPopupParentTransform);
		MiniPopupObject component = ObjectPool.Spawn("@MiniPopupObject", Vector2.zero, parent).GetComponent<MiniPopupObject>();
		component.initMiniPopupObject(miniPopupType, isEndlessTime, endTime, timeEndAction);
		currentMiniPopupObjectList.Add(component);
		if (flag)
		{
			component.cachedTransform.localPosition = new Vector2(m_ingameBaseStartposition.x - 150f, m_ingameBaseStartposition.y - (m_interval * (float)currentMiniPopupObjectList.Count - 1f));
		}
		else
		{
			float num = 0f - m_interval * (float)(currentMiniPopupObjectList.Count - 1) / 2f;
			component.cachedTransform.localPosition = new Vector2(num + (m_interval * (float)currentMiniPopupObjectList.Count - 1f * m_interval), m_outgameBaseStartposition.y + 200f);
		}
		refreshTargetPositions();
	}

	public void registryMiniPopupData(MiniPopupType miniPopupType, long leftCount, Action timeEndAction)
	{
		if (containFromCurrentMiniPopupList(miniPopupType))
		{
			getMiniPopupDataFromCurrentPopupList(miniPopupType).currentLeftCount = leftCount;
			getMiniPopupDataFromCurrentPopupList(miniPopupType).timerText.text = leftCount.ToString();
			return;
		}
		bool flag = ((GameManager.currentGameState == GameManager.GameState.Playing) ? true : false);
		Transform parent = ((!flag) ? topCenterMiniPopupParentTransform : topLeftMiniPopupParentTransform);
		MiniPopupObject component = ObjectPool.Spawn("@MiniPopupObject", Vector2.zero, parent).GetComponent<MiniPopupObject>();
		component.initMiniPopupObject(miniPopupType, leftCount, timeEndAction);
		currentMiniPopupObjectList.Add(component);
		if (flag)
		{
			component.cachedTransform.localPosition = new Vector2(m_ingameBaseStartposition.x - 150f, m_ingameBaseStartposition.y - (m_interval * (float)currentMiniPopupObjectList.Count - 1f));
		}
		else
		{
			float num = 0f - m_interval * (float)(currentMiniPopupObjectList.Count - 1) / 2f;
			component.cachedTransform.localPosition = new Vector2(num + (m_interval * (float)currentMiniPopupObjectList.Count - 1f * m_interval), m_outgameBaseStartposition.y + 200f);
		}
		refreshTargetPositions();
	}

	public MiniPopupObject getMiniPopupDataFromCurrentPopupList(MiniPopupType miniPopupType)
	{
		MiniPopupObject result = null;
		for (int i = 0; i < currentMiniPopupObjectList.Count; i++)
		{
			if (currentMiniPopupObjectList[i].currentMiniPopupType == miniPopupType)
			{
				result = currentMiniPopupObjectList[i];
				break;
			}
		}
		return result;
	}

	public bool containFromCurrentMiniPopupList(MiniPopupType popupType)
	{
		bool result = false;
		for (int i = 0; i < currentMiniPopupObjectList.Count; i++)
		{
			if (currentMiniPopupObjectList[i].currentMiniPopupType == popupType)
			{
				result = true;
			}
		}
		return result;
	}

	public void refreshForcePositions()
	{
		bool flag = ((GameManager.currentGameState == GameManager.GameState.Playing) ? true : false);
		int num = currentMiniPopupObjectList.Count / maxCellCount;
		float num2 = 0f - m_interval * (float)(Mathf.Min(currentMiniPopupObjectList.Count, maxCellCount) - 1) / 2f;
		Transform parent = ((!flag) ? topCenterMiniPopupParentTransform : topLeftMiniPopupParentTransform);
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < currentMiniPopupObjectList.Count; i++)
		{
			if (num3 >= maxCellCount)
			{
				num3 -= maxCellCount;
				num4++;
				int num5 = ((num <= num4) ? (currentMiniPopupObjectList.Count - maxCellCount * num4) : maxCellCount);
				num2 = 0f - m_interval * (float)(num5 - 1) / 2f;
			}
			Vector2 zero = Vector2.zero;
			if (flag)
			{
				zero = new Vector2(m_ingameBaseStartposition.x, m_ingameBaseStartposition.y - m_interval * (float)i);
			}
			else
			{
				float y = m_outgameBaseStartposition.y;
				y -= (float)(i / maxCellCount) * m_interval;
				zero = new Vector2(num2 + (float)(i % maxCellCount) * m_interval, y);
			}
			currentMiniPopupObjectList[i].cachedTransform.SetParent(parent);
			currentMiniPopupObjectList[i].cachedTransform.localPosition = zero;
		}
	}

	public void refreshTargetPositions()
	{
		bool flag = ((GameManager.currentGameState == GameManager.GameState.Playing) ? true : false);
		int num = currentMiniPopupObjectList.Count / maxCellCount;
		float num2 = 0f - m_interval * (float)(Mathf.Min(currentMiniPopupObjectList.Count, maxCellCount) - 1) / 2f;
		Transform parent = ((!flag) ? topCenterMiniPopupParentTransform : topLeftMiniPopupParentTransform);
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < currentMiniPopupObjectList.Count; i++)
		{
			if (num3 >= maxCellCount)
			{
				num3 -= maxCellCount;
				num4++;
				int num5 = ((num <= num4) ? (currentMiniPopupObjectList.Count - maxCellCount * num4) : maxCellCount);
				num2 = 0f - m_interval * (float)(num5 - 1) / 2f;
			}
			Vector2 zero = Vector2.zero;
			if (flag)
			{
				zero = new Vector2(m_ingameBaseTargetposition.x, m_ingameBaseTargetposition.y - m_interval * (float)i);
			}
			else
			{
				float y = m_outgameBaseTargetposition.y;
				y -= (float)(i / maxCellCount) * m_interval;
				zero = new Vector2(num2 + (float)(i % maxCellCount) * m_interval, y);
			}
			currentMiniPopupObjectList[i].cachedTransform.SetParent(parent);
			currentMiniPopupObjectList[i].setTargetPosition(zero, flag);
			num3++;
		}
	}

	public void startGame()
	{
		refreshForcePositions();
		refreshTargetPositions();
	}

	public void endGame()
	{
		refreshForcePositions();
		refreshTargetPositions();
	}
}
