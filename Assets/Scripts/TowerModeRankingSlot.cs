using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerModeRankingSlot : ScrollSlotItem
{
	public UIWindowTowerModeRanking.TabType currentSlotType;

	public Image backgroundImage;

	public Text honorSeasonText;

	public Text[] rankingTexts;

	public Text[] nameTexts;

	public Text[] recordTexts;

	public GameObject seasonObject;

	public GameObject honorObject;

	public GameObject seasonRankingTextObject;

	public GameObject seasonRankingIconObject;

	public Image seasonRankingIconImage;

	public GameObject warriorEmptyObject;

	public GameObject priestEmptyObject;

	public GameObject archerEmptyObject;

	public CharacterUIObject warriorUIObject;

	public CharacterUIObject priestUIObject;

	public CharacterUIObject archerUIObject;

	private TowerModeManager.UserInformationData m_currentUserData;

	public override void UpdateItem(int count)
	{
		base.UpdateItem(count);
		if ((slotIndex + 1) % 2 == 0)
		{
			backgroundImage.color = new Color(0f, 0f, 0f, 0f);
		}
		else
		{
			backgroundImage.color = Util.getCalculatedColor(0f, 11f, 28f, 51f);
		}
		refreshSlot();
	}

	public override void refreshSlot()
	{
		currentSlotType = UIWindowTowerModeRanking.instance.currentTabType;
		List<TowerModeManager.UserInformationData> list = null;
		switch (UIWindowTowerModeRanking.instance.currentTabType)
		{
		case UIWindowTowerModeRanking.TabType.SeasonRankingTimeAttackMode:
			list = Singleton<TowerModeManager>.instance.totalTimeAttackSeasonRankingDataList;
			break;
		case UIWindowTowerModeRanking.TabType.SeasonRankingEndlessMode:
			list = Singleton<TowerModeManager>.instance.totalEndlessSeasonRankingDataList;
			break;
		case UIWindowTowerModeRanking.TabType.HonorRankingTimeAttackMode:
			list = Singleton<TowerModeManager>.instance.totalTimeAttackHonorRankingData.honor;
			break;
		case UIWindowTowerModeRanking.TabType.HonorRankingEndlessMode:
			list = Singleton<TowerModeManager>.instance.totalEndlessHonorRankingData.honor;
			break;
		}
		m_currentUserData = null;
		if (slotIndex >= 0 && list != null && list.Count > slotIndex)
		{
			m_currentUserData = list[slotIndex];
			if (!base.cachedGameObject.activeSelf)
			{
				base.cachedGameObject.SetActive(true);
			}
		}
		else if (base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(false);
		}
		if (m_currentUserData == null)
		{
			return;
		}
		for (int i = 0; i < nameTexts.Length; i++)
		{
			nameTexts[i].text = m_currentUserData.nickname;
		}
		int result = 0;
		if (!int.TryParse(m_currentUserData.rank, out result))
		{
			result = -1;
		}
		for (int j = 0; j < rankingTexts.Length; j++)
		{
			rankingTexts[j].text = ((result == -1) ? "--" : result.ToString("N0"));
		}
		float result2 = 0f;
		if (!float.TryParse(m_currentUserData.clear_time, out result2))
		{
			result2 = 0f;
		}
		else
		{
			result2 /= 1000f;
		}
		int result3 = -1;
		int.TryParse(m_currentUserData.clear_floor, out result3);
		if (result3 < 0)
		{
			result3 = 0;
		}
		float result4 = -1f;
		float.TryParse(m_currentUserData.clear_time, out result4);
		result4 = ((result4 == -1f) ? 0f : (result4 / 1000f));
		if (!string.IsNullOrEmpty(m_currentUserData.season_id))
		{
			int result5 = -1;
			int.TryParse(m_currentUserData.season_id, out result5);
			honorSeasonText.text = ((result5 == -1) ? "--" : result5.ToString("N0"));
		}
		string empty = string.Empty;
		empty = (((currentSlotType != 0 && currentSlotType != UIWindowTowerModeRanking.TabType.HonorRankingTimeAttackMode) || result3 < Singleton<TowerModeManager>.instance.getTotalFloor(TowerModeManager.TowerModeDifficultyType.TimeAttack)) ? string.Format(I18NManager.Get("FLOOR_TEXT"), result3) : I18NManager.Get("CLEAR"));
		TimeSpan timeSpan = TimeSpan.FromSeconds(result4);
		for (int k = 0; k < recordTexts.Length; k++)
		{
			recordTexts[k].text = empty + "<color=#FDFCB7> / " + string.Format("{0:00}:{1:00}.{2:000}", (int)timeSpan.TotalMinutes, timeSpan.Seconds, timeSpan.Milliseconds) + "</color>";
		}
		string[] array = ((m_currentUserData.game_data == null) ? null : m_currentUserData.game_data.Split(','));
		if (array != null && array.Length == 3)
		{
			CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)int.Parse(array[0]);
			CharacterSkinManager.PriestSkinType priestSkinType = (CharacterSkinManager.PriestSkinType)int.Parse(array[1]);
			CharacterSkinManager.ArcherSkinType archerSkinType = (CharacterSkinManager.ArcherSkinType)int.Parse(array[2]);
			if (warriorSkinType < CharacterSkinManager.WarriorSkinType.Length)
			{
				if (warriorEmptyObject.activeSelf)
				{
					warriorEmptyObject.SetActive(false);
				}
				if (!warriorUIObject.cachedGameObject.activeSelf)
				{
					warriorUIObject.cachedGameObject.SetActive(true);
				}
				warriorUIObject.initCharacterUIObject(warriorSkinType, UIWindowTowerModeRanking.instance.cachedCanvasGroup);
			}
			else
			{
				if (!warriorEmptyObject.activeSelf)
				{
					warriorEmptyObject.SetActive(true);
				}
				if (warriorUIObject.cachedGameObject.activeSelf)
				{
					warriorUIObject.cachedGameObject.SetActive(false);
				}
			}
			if (priestSkinType < CharacterSkinManager.PriestSkinType.Length)
			{
				if (priestEmptyObject.activeSelf)
				{
					priestEmptyObject.SetActive(false);
				}
				if (!priestUIObject.cachedGameObject.activeSelf)
				{
					priestUIObject.cachedGameObject.SetActive(true);
				}
				priestUIObject.initCharacterUIObject(priestSkinType, UIWindowTowerModeRanking.instance.cachedCanvasGroup);
			}
			else
			{
				if (!priestEmptyObject.activeSelf)
				{
					priestEmptyObject.SetActive(true);
				}
				if (priestUIObject.cachedGameObject.activeSelf)
				{
					priestUIObject.cachedGameObject.SetActive(false);
				}
			}
			if (archerSkinType < CharacterSkinManager.ArcherSkinType.Length)
			{
				if (archerEmptyObject.activeSelf)
				{
					archerEmptyObject.SetActive(false);
				}
				if (!archerUIObject.cachedGameObject.activeSelf)
				{
					archerUIObject.cachedGameObject.SetActive(true);
				}
				archerUIObject.initCharacterUIObject(archerSkinType, UIWindowTowerModeRanking.instance.cachedCanvasGroup);
			}
			else
			{
				if (!archerEmptyObject.activeSelf)
				{
					archerEmptyObject.SetActive(true);
				}
				if (archerUIObject.cachedGameObject.activeSelf)
				{
					archerUIObject.cachedGameObject.SetActive(false);
				}
			}
		}
		else
		{
			if (!warriorEmptyObject.activeSelf)
			{
				warriorEmptyObject.SetActive(true);
			}
			if (!priestEmptyObject.activeSelf)
			{
				priestEmptyObject.SetActive(true);
			}
			if (!archerEmptyObject.activeSelf)
			{
				archerEmptyObject.SetActive(true);
			}
			if (warriorUIObject.cachedGameObject.activeSelf)
			{
				warriorUIObject.cachedGameObject.SetActive(false);
			}
			if (priestUIObject.cachedGameObject.activeSelf)
			{
				priestUIObject.cachedGameObject.SetActive(false);
			}
			if (archerUIObject.cachedGameObject.activeSelf)
			{
				archerUIObject.cachedGameObject.SetActive(false);
			}
		}
		bool flag = currentSlotType == UIWindowTowerModeRanking.TabType.SeasonRankingEndlessMode || currentSlotType == UIWindowTowerModeRanking.TabType.SeasonRankingTimeAttackMode;
		result = 0;
		if (!int.TryParse(m_currentUserData.rank, out result))
		{
			result = int.MaxValue;
		}
		if (result == int.MaxValue)
		{
			for (int l = 0; l < rankingTexts.Length; l++)
			{
				rankingTexts[l].text = "--";
			}
		}
		if (flag)
		{
			if (!seasonObject.activeSelf)
			{
				seasonObject.SetActive(true);
			}
			if (honorObject.activeSelf)
			{
				honorObject.SetActive(false);
			}
			if (result > 3)
			{
				seasonRankingTextObject.SetActive(true);
				seasonRankingIconObject.SetActive(false);
			}
			else
			{
				seasonRankingTextObject.SetActive(false);
				seasonRankingIconObject.SetActive(true);
				seasonRankingIconImage.sprite = UIWindowTowerModeRanking.instance.rankingIconSprites[result - 1];
			}
		}
		else
		{
			if (seasonObject.activeSelf)
			{
				seasonObject.SetActive(false);
			}
			if (!honorObject.activeSelf)
			{
				honorObject.SetActive(true);
			}
		}
	}
}
