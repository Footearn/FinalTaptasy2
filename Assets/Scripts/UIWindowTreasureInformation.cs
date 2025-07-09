using UnityEngine;
using UnityEngine.UI;

public class UIWindowTreasureInformation : UIWindow
{
	public static UIWindowTreasureInformation instance;

	public GameObject donotOwnedObject;

	public GameObject ownObject;

	public GameObject pvpTreasureObject;

	public Text currentLevelText;

	public Text nextStatInformationText;

	public Text enchantStonePriceText;

	public Text rubyPriceText;

	public Image treasureIconImage;

	public Text nameText;

	public Text descriptionText;

	public Image treasureBackgroundImage;

	public GameObject premiumTreasureDescriptionObject;

	public Text premiumTreasureDescriptionText;

	public ParticleSystem upgradeEffect;

	private TreasureManager.TreasureType m_currentTreasureType;

	private TreasureInventoryData m_currentTreasureInventoryData;

	private TreasureStatData m_currentTreasureStatData;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openWithTreasureInformation(TreasureManager.TreasureType targetTreasureType)
	{
		m_currentTreasureType = targetTreasureType;
		m_currentTreasureStatData = Singleton<ParsingManager>.instance.currentParsedStatData.treasureStatData[m_currentTreasureType];
		if (Singleton<TreasureManager>.instance.containTreasureFromInventory(m_currentTreasureType))
		{
			m_currentTreasureInventoryData = Singleton<TreasureManager>.instance.getTreasureDataFromInventory(m_currentTreasureType);
		}
		else
		{
			m_currentTreasureInventoryData = null;
		}
		pvpTreasureObject.SetActive(Singleton<TreasureManager>.instance.isPVPTreasure(targetTreasureType));
		int treasureTier = Singleton<TreasureManager>.instance.getTreasureTier(m_currentTreasureType);
		if (treasureTier == 0)
		{
			if (!premiumTreasureDescriptionObject.activeSelf)
			{
				premiumTreasureDescriptionObject.SetActive(true);
			}
			if (Singleton<TreasureManager>.instance.isElopeTreasure(targetTreasureType))
			{
				premiumTreasureDescriptionText.text = I18NManager.Get("PREMIUM_TREASURE_DESCRIPTION");
			}
			else
			{
				string empty = string.Empty;
				empty = ((targetTreasureType != TreasureManager.TreasureType.ConquerToken) ? I18NManager.Get("TOWER_MODE_DIFFICULTY_2") : I18NManager.Get("TOWER_MODE_DIFFICULTY_1"));
				premiumTreasureDescriptionText.text = string.Format(I18NManager.Get("TOWER_MODE_TREASURE_DESCRIPTION"), empty);
			}
		}
		else if (premiumTreasureDescriptionObject.activeSelf)
		{
			premiumTreasureDescriptionObject.SetActive(false);
		}
		treasureBackgroundImage.sprite = Singleton<TreasureManager>.instance.tierBackgroundSprites[treasureTier];
		treasureIconImage.sprite = Singleton<TreasureManager>.instance.getTreasureSprite(m_currentTreasureType);
		treasureIconImage.SetNativeSize();
		nameText.text = Singleton<TreasureManager>.instance.getTreasureI18NName(m_currentTreasureType);
		if (m_currentTreasureInventoryData != null)
		{
			double num = m_currentTreasureInventoryData.treasureEffectValue + m_currentTreasureInventoryData.extraTreasureEffectValue;
			donotOwnedObject.SetActive(false);
			ownObject.SetActive(true);
			descriptionText.text = Singleton<TreasureManager>.instance.getTreasureDescription(m_currentTreasureType, m_currentTreasureInventoryData.treasureEffectValue + m_currentTreasureInventoryData.extraTreasureEffectValue, 0.0);
			if (m_currentTreasureInventoryData.treasureLevel < m_currentTreasureStatData.maxLevel)
			{
				currentLevelText.text = "Lv. " + m_currentTreasureInventoryData.treasureLevel;
				enchantStonePriceText.text = Singleton<TreasureManager>.instance.getCurrentTreasureLevelUpEnchantStonePrice(m_currentTreasureType).ToString("N0");
				rubyPriceText.text = Singleton<TreasureManager>.instance.getCurrentTreasureLevelUpRubyPrice(m_currentTreasureType).ToString("N0");
				if (m_currentTreasureType == TreasureManager.TreasureType.OcarinaOfForestSpirit || m_currentTreasureType == TreasureManager.TreasureType.MonocleOfWiseGoddess || m_currentTreasureType == TreasureManager.TreasureType.NecklaceOfSorcerers || m_currentTreasureType == TreasureManager.TreasureType.MeteorPiece || m_currentTreasureType == TreasureManager.TreasureType.IceCrystal || m_currentTreasureType == TreasureManager.TreasureType.ChaosBlade || m_currentTreasureType == TreasureManager.TreasureType.FireDragonHeart || m_currentTreasureType == TreasureManager.TreasureType.IceHeart)
				{
					nextStatInformationText.text = string.Format(I18NManager.Get("NEXT_LEVEL_FOR_TREASURE"), "<color=white>+" + (num + m_currentTreasureStatData.increasingValueEveryEnchant) + "</color>");
				}
				else
				{
					nextStatInformationText.text = string.Format(I18NManager.Get("NEXT_LEVEL_FOR_TREASURE"), "<color=white>+" + (num + m_currentTreasureStatData.increasingValueEveryEnchant) + "%</color>");
				}
			}
			else
			{
				currentLevelText.text = "Max";
				enchantStonePriceText.text = "Max";
				rubyPriceText.text = "Max";
				nextStatInformationText.text = string.Format(I18NManager.Get("NEXT_LEVEL_FOR_TREASURE"), "<color=white>Max Level</color>");
			}
		}
		else
		{
			descriptionText.text = Singleton<TreasureManager>.instance.getTreasureDescription(m_currentTreasureType, m_currentTreasureStatData.treasureEffectValue, 0.0);
			donotOwnedObject.SetActive(true);
			ownObject.SetActive(false);
		}
		open();
	}

	public void OnClickEnchantTreasure(bool isEnchantByRuby)
	{
		if (Singleton<TreasureManager>.instance.levelUpTreasure(m_currentTreasureType, isEnchantByRuby))
		{
			openWithTreasureInformation(m_currentTreasureType);
			upgradeEffect.Stop();
			upgradeEffect.Play();
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
		}
	}
}
