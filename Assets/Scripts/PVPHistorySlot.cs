using System;
using UnityEngine;
using UnityEngine.UI;

public class PVPHistorySlot : ScrollSlotItem
{
	public Image backgroundImage;

	public GameObject winObject;

	public GameObject loseObject;

	public CharacterUIObject enemyWarriorUIObject;

	public CharacterUIObject enemyPriestUIObject;

	public CharacterUIObject enemyArcherUIObject;

	public Text nameText;

	public Text tierText;

	public Image historyModeIconImage;

	public Text changedMMRText;

	public Text historyTimeText;

	private PVPManager.HistoryData m_currentHistoryResponseData;

	private PVPManager.PVPGameData m_currentEnemyGameData;

	public override void UpdateItem(int count)
	{
		base.UpdateItem(count);
		if ((slotIndex + 1) % 2 == 0)
		{
			backgroundImage.color = new Color(0f, 0f, 0f, 0f);
		}
		else
		{
			backgroundImage.color = new Color(0f, 0f, 0f, 0.3f);
		}
		m_currentHistoryResponseData = null;
		m_currentEnemyGameData = null;
		refreshSlot();
	}

	public override void refreshSlot()
	{
		if (slotIndex >= 0 && Singleton<PVPManager>.instance.currentHistoryData.history.Length > slotIndex)
		{
			if (!base.cachedGameObject.activeSelf)
			{
				base.cachedGameObject.SetActive(true);
			}
			m_currentHistoryResponseData = Singleton<PVPManager>.instance.currentHistoryData.history[slotIndex];
		}
		else if (base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(false);
		}
		if (m_currentHistoryResponseData == null)
		{
			return;
		}
		bool flag = ((m_currentHistoryResponseData.state == "win") ? true : false);
		winObject.SetActive(flag);
		loseObject.SetActive(!flag);
		if (m_currentHistoryResponseData.target != null)
		{
			m_currentEnemyGameData = Singleton<PVPManager>.instance.convertStringToPVPGameData(m_currentHistoryResponseData.target.game_data);
		}
		if (m_currentEnemyGameData != null)
		{
			PVPManager.HistoryEnemyData target = m_currentHistoryResponseData.target;
			string[] array = ((m_currentEnemyGameData.equippedCharacterData == null) ? null : m_currentEnemyGameData.equippedCharacterData.Split(','));
			if (array != null && array.Length == 3)
			{
				CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)int.Parse(array[0]);
				CharacterSkinManager.PriestSkinType priestSkinType = (CharacterSkinManager.PriestSkinType)int.Parse(array[1]);
				CharacterSkinManager.ArcherSkinType archerSkinType = (CharacterSkinManager.ArcherSkinType)int.Parse(array[2]);
				if (warriorSkinType < CharacterSkinManager.WarriorSkinType.Length)
				{
					enemyWarriorUIObject.initCharacterUIObject(warriorSkinType, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				}
				else
				{
					enemyWarriorUIObject.initCharacterUIObject(CharacterSkinManager.WarriorSkinType.William, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				}
				if (priestSkinType < CharacterSkinManager.PriestSkinType.Length)
				{
					enemyPriestUIObject.initCharacterUIObject(priestSkinType, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				}
				else
				{
					enemyPriestUIObject.initCharacterUIObject(CharacterSkinManager.PriestSkinType.Olivia, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				}
				if (archerSkinType < CharacterSkinManager.ArcherSkinType.Length)
				{
					enemyArcherUIObject.initCharacterUIObject(archerSkinType, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				}
				else
				{
					enemyArcherUIObject.initCharacterUIObject(CharacterSkinManager.ArcherSkinType.Windstoker, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				}
			}
			else
			{
				enemyWarriorUIObject.initCharacterUIObject(CharacterSkinManager.WarriorSkinType.William, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				enemyPriestUIObject.initCharacterUIObject(CharacterSkinManager.PriestSkinType.Olivia, UIWindowPVPMainUI.instance.cachedCanvasGroup);
				enemyArcherUIObject.initCharacterUIObject(CharacterSkinManager.ArcherSkinType.Windstoker, UIWindowPVPMainUI.instance.cachedCanvasGroup);
			}
			nameText.text = target.nickname;
			tierText.text = Singleton<PVPManager>.instance.getTierName(int.Parse(target.grade)) + " / <color=#FDFCB7>" + string.Format(I18NManager.Get("PVP_MMR"), target.point) + "</color>";
			int num = int.Parse(m_currentHistoryResponseData.add_point);
			changedMMRText.text = m_currentHistoryResponseData.point + "(" + ((num < 0) ? string.Empty : "+") + num + ")";
			int ts = m_currentHistoryResponseData.ts;
			TimeSpan timeSpan = TimeSpan.FromSeconds(ts);
			if ((int)timeSpan.TotalDays >= 1)
			{
				historyTimeText.text = string.Format(I18NManager.Get("DAY_AGO"), (int)timeSpan.TotalDays);
			}
			else if ((int)timeSpan.TotalHours >= 1)
			{
				historyTimeText.text = string.Format(I18NManager.Get("HOUR_AGO"), (int)timeSpan.TotalHours);
			}
			else if ((int)timeSpan.TotalMinutes >= 1)
			{
				historyTimeText.text = string.Format(I18NManager.Get("MINUTE_AGO"), (int)timeSpan.TotalMinutes);
			}
			else if ((int)timeSpan.TotalSeconds >= 1)
			{
				historyTimeText.text = string.Format(I18NManager.Get("SECOND_AGO"), (int)timeSpan.TotalSeconds);
			}
			else
			{
				historyTimeText.text = "--";
			}
			historyModeIconImage.sprite = ((!(target.mode == "attack")) ? Singleton<CachedManager>.instance.pvpHistoryDefenceIconSprite : Singleton<CachedManager>.instance.pvpHistoryAttackIconSprite);
			historyModeIconImage.SetNativeSize();
		}
		else
		{
			base.cachedGameObject.SetActive(false);
		}
	}
}
