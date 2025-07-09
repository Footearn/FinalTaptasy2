using UnityEngine;
using UnityEngine.UI;

public class PVPSeasonRankingSlot : ScrollSlotItem
{
	public Image backgroundImage;

	public Image rankingIconImage;

	public GameObject rankingIconGameObject;

	public Text rankingText;

	public GameObject rankingTextGameObject;

	public Image tierIconImage;

	public CharacterUIObject warriorUIObject;

	public CharacterUIObject priestUIObject;

	public CharacterUIObject archerUIObject;

	public Text nickNameText;

	public Text recordText;

	private PVPManager.PVPUserData m_currentUserData;

	private PVPManager.PVPGameData m_currentGameDataData;

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
		refreshSlot();
	}

	public override void refreshSlot()
	{
		if (slotIndex >= 0 && Singleton<PVPManager>.instance.seasonTotalRankingList.Count > slotIndex)
		{
			if (!base.cachedGameObject.activeSelf)
			{
				base.cachedGameObject.SetActive(true);
			}
			m_currentUserData = Singleton<PVPManager>.instance.seasonTotalRankingList[slotIndex];
		}
		else if (base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(false);
		}
		if (m_currentUserData == null)
		{
			return;
		}
		tierIconImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(m_currentUserData.grade);
		tierIconImage.SetNativeSize();
		nickNameText.text = m_currentUserData.nickname;
		int rank = m_currentUserData.rank;
		if (rank > 3)
		{
			rankingTextGameObject.SetActive(true);
			rankingIconGameObject.SetActive(false);
		}
		else
		{
			rankingTextGameObject.SetActive(false);
			rankingIconGameObject.SetActive(true);
			rankingIconImage.sprite = UIWindowTowerModeRanking.instance.rankingIconSprites[rank - 1];
			rankingIconImage.SetNativeSize();
		}
		rankingText.text = ((rank == -1) ? "--" : rank.ToString("N0"));
		m_currentGameDataData = Singleton<PVPManager>.instance.convertStringToPVPGameData(m_currentUserData.game_data);
		if (m_currentGameDataData != null)
		{
			string[] array = ((m_currentGameDataData.equippedCharacterData == null) ? null : m_currentGameDataData.equippedCharacterData.Split(','));
			if (array != null && array.Length == 3)
			{
				CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)int.Parse(array[0]);
				CharacterSkinManager.PriestSkinType priestSkinType = (CharacterSkinManager.PriestSkinType)int.Parse(array[1]);
				CharacterSkinManager.ArcherSkinType archerSkinType = (CharacterSkinManager.ArcherSkinType)int.Parse(array[2]);
				if (warriorSkinType < CharacterSkinManager.WarriorSkinType.Length)
				{
					warriorUIObject.initCharacterUIObject(warriorSkinType, UIWindowPVPMainUI.instance.cachedCanvasGroup, "PopUpLayer", 120);
				}
				else
				{
					warriorUIObject.initCharacterUIObject(CharacterSkinManager.WarriorSkinType.William, UIWindowPVPMainUI.instance.cachedCanvasGroup, "PopUpLayer", 120);
				}
				if (priestSkinType < CharacterSkinManager.PriestSkinType.Length)
				{
					priestUIObject.initCharacterUIObject(priestSkinType, UIWindowPVPMainUI.instance.cachedCanvasGroup, "PopUpLayer", 120);
				}
				else
				{
					priestUIObject.initCharacterUIObject(CharacterSkinManager.PriestSkinType.Olivia, UIWindowPVPMainUI.instance.cachedCanvasGroup, "PopUpLayer", 120);
				}
				if (archerSkinType < CharacterSkinManager.ArcherSkinType.Length)
				{
					archerUIObject.initCharacterUIObject(archerSkinType, UIWindowPVPMainUI.instance.cachedCanvasGroup, "PopUpLayer", 120);
				}
				else
				{
					archerUIObject.initCharacterUIObject(CharacterSkinManager.ArcherSkinType.Windstoker, UIWindowPVPMainUI.instance.cachedCanvasGroup, "PopUpLayer", 120);
				}
			}
		}
		else
		{
			warriorUIObject.initCharacterUIObject(CharacterSkinManager.WarriorSkinType.William, UIWindowPVPMainUI.instance.cachedCanvasGroup, "PopUpLayer", 120);
			priestUIObject.initCharacterUIObject(CharacterSkinManager.PriestSkinType.Olivia, UIWindowPVPMainUI.instance.cachedCanvasGroup, "PopUpLayer", 120);
			archerUIObject.initCharacterUIObject(CharacterSkinManager.ArcherSkinType.Windstoker, UIWindowPVPMainUI.instance.cachedCanvasGroup, "PopUpLayer", 120);
		}
		recordText.text = string.Format(I18NManager.Get("PVP_MMR"), m_currentUserData.point) + " <color=#FDFCB7>/ " + string.Format(I18NManager.Get("PVP_TOTAL_PLAY_TEXT"), int.Parse(m_currentUserData.win) + int.Parse(m_currentUserData.lose)) + " " + string.Format(I18NManager.Get("PVP_TOTAL_WIN"), m_currentUserData.win) + "</color>";
	}

	public void OnClickBattle()
	{
		if (m_currentGameDataData != null)
		{
			Singleton<PVPManager>.instance.practiceTargetUserData = m_currentUserData;
			Singleton<PVPManager>.instance.startPVP(m_currentGameDataData, true);
		}
		else
		{
			UIWindowDialog.openDescription("ERROR_FOR_TOWER_MODE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}
}
