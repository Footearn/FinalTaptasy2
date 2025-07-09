using UnityEngine;
using UnityEngine.UI;

public class PVPHonorRankingSlot : ScrollSlotItem
{
	public Image backgroundImage;

	public Image tierIconImage;

	public Text seasonText;

	public CharacterUIObject warriorUIObject;

	public CharacterUIObject priestUIObject;

	public CharacterUIObject archerUIObject;

	public Text nickNameText;

	public Text recordText;

	private PVPManager.HonorRankingData m_currentHonorRankingData;

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
		if (slotIndex >= 0 && Singleton<PVPManager>.instance.currentHonorRankingResponse.honor.Length > slotIndex)
		{
			if (!base.cachedGameObject.activeSelf)
			{
				base.cachedGameObject.SetActive(true);
			}
			m_currentHonorRankingData = Singleton<PVPManager>.instance.currentHonorRankingResponse.honor[slotIndex];
		}
		else if (base.cachedGameObject.activeSelf)
		{
			base.cachedGameObject.SetActive(false);
		}
		if (m_currentHonorRankingData == null)
		{
			return;
		}
		seasonText.text = m_currentHonorRankingData.season;
		tierIconImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(m_currentHonorRankingData.grade);
		tierIconImage.SetNativeSize();
		nickNameText.text = m_currentHonorRankingData.nickname;
		m_currentGameDataData = Singleton<PVPManager>.instance.convertStringToPVPGameData(m_currentHonorRankingData.game_data);
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
		recordText.text = string.Format(I18NManager.Get("PVP_MMR"), m_currentHonorRankingData.point);
	}

	public void OnClickBattle()
	{
		if (m_currentGameDataData != null)
		{
			PVPManager.PVPUserData pVPUserData = new PVPManager.PVPUserData();
			pVPUserData.nickname = m_currentHonorRankingData.nickname;
			pVPUserData.win = m_currentHonorRankingData.win;
			pVPUserData.lose = m_currentHonorRankingData.lose;
			pVPUserData.game_data = m_currentHonorRankingData.game_data;
			pVPUserData.point = m_currentHonorRankingData.point;
			pVPUserData.grade = m_currentHonorRankingData.grade;
			Singleton<PVPManager>.instance.practiceTargetUserData = pVPUserData;
			Singleton<PVPManager>.instance.startPVP(m_currentGameDataData, true);
		}
		else
		{
			UIWindowDialog.openDescription("ERROR_FOR_TOWER_MODE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}
}
