using UnityEngine;
using UnityEngine.UI;

public class PrincessCollectionSlot : ScrollSlotItem
{
	public GameObject backgroundObject;

	public Text nameText;

	public SpriteAnimation princessSpriteAnimation;

	public Image backgroundImage;

	public Text passiveSkillDescriptionText;

	public Text friendshipLevelText;

	private int m_currentPrincessIndex;

	public override void refreshSlot()
	{
		UpdateItem(slotIndex);
	}

	public override void UpdateItem(int count)
	{
		base.UpdateItem(count);
		m_currentPrincessIndex = slotIndex;
		if (m_currentPrincessIndex > GameManager.maxPrincessCount || slotIndex < 1)
		{
			if (backgroundObject.activeSelf)
			{
				backgroundObject.SetActive(false);
			}
			return;
		}
		if (!backgroundObject.activeSelf)
		{
			backgroundObject.SetActive(true);
		}
		refreshPrincessSlot();
	}

	public void refreshPrincessSlot()
	{
		string format = I18NManager.Get("PRINCESS_NAME");
		nameText.text = string.Format(format, m_currentPrincessIndex);
		princessSpriteAnimation.animationType = "Princess" + m_currentPrincessIndex;
		princessSpriteAnimation.init();
		princessSpriteAnimation.playFixAnimation("Idle", 0);
		int currentPrincessNumber = GameManager.getCurrentPrincessNumber(Singleton<DataManager>.instance.currentGameData.bestTheme);
		string empty = string.Empty;
		int num = Mathf.Clamp(Singleton<DataManager>.instance.currentGameData.bestTheme - (m_currentPrincessIndex - 1) * 10 - 1, 0, 10);
		if (m_currentPrincessIndex == currentPrincessNumber)
		{
			backgroundImage.sprite = Singleton<CachedManager>.instance.princessRescueingBackgroundSprite;
			passiveSkillDescriptionText.color = Util.getCalculatedColor(116f, 72f, 61f);
			empty = "+100%";
		}
		else
		{
			backgroundImage.sprite = Singleton<CachedManager>.instance.princessSlotNormalBackgroundSprite;
			if (m_currentPrincessIndex > currentPrincessNumber)
			{
				passiveSkillDescriptionText.color = Util.getCalculatedColor(142f, 133f, 96f);
				empty = "+100%";
			}
			else
			{
				passiveSkillDescriptionText.color = Color.white;
				empty = "<color=#FAD725FF>+100%</color>";
			}
		}
		passiveSkillDescriptionText.text = I18NManager.Get("ALL_CHARACTER_AND_COLLEAGUE_DAMAGE") + "\n" + empty;
		if (m_currentPrincessIndex <= currentPrincessNumber)
		{
			princessSpriteAnimation.targetImage.color = new Color(1f, 1f, 1f, 1f);
			if (m_currentPrincessIndex == currentPrincessNumber)
			{
				friendshipLevelText.text = I18NManager.Get("PRINCESS_CURRENT_RESCUE");
			}
			else
			{
				friendshipLevelText.text = I18NManager.Get("RESCUE_SUCCESS");
			}
		}
		else
		{
			princessSpriteAnimation.targetImage.color = new Color(0f, 0f, 0f, 0.9f);
			friendshipLevelText.text = I18NManager.Get("PRINCESS_NEVER_RESCUE");
		}
	}
}
