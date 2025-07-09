using UnityEngine;
using UnityEngine.UI;

public class CollectEventItemSlot : ObjectBase
{
	public int slotTier;

	public CollectEventManager.CollectEventRewardType currentCollectEventRewardType;

	public Image backgroundImage;

	public Image rewardIconImage;

	public Image rewardButtonImage;

	public Text rewardButtonText;

	public Text needValueText;

	public Text rewardTitleText;

	public Text rewardDetailText;

	public GameObject rewardIconObject;

	public GameObject rewardButtonObject;

	public GameObject checkObject;

	public GameObject receivedObject;

	public RectTransform rewardInformationTransform;

	public CharacterUIObject currentCharacterUIObject;

	private CollectEventManager.CollectEventRewardData m_currentCollectEventRewardData;

	public void initCollectEventSlot()
	{
		m_currentCollectEventRewardData = Singleton<CollectEventManager>.instance.getCollectEventRewardData(slotTier);
		currentCollectEventRewardType = m_currentCollectEventRewardData.targetRewardType;
		rewardInformationTransform.anchoredPosition = new Vector2(0f, 0f);
		needValueText.text = m_currentCollectEventRewardData.rewardNeedValue.ToString();
		if (m_currentCollectEventRewardData.isCharacterSkin)
		{
			rewardIconObject.SetActive(false);
			if (currentCharacterUIObject == null)
			{
				currentCharacterUIObject = ObjectPool.Spawn("@CharacterUIObject", new Vector2(2.7f, -22.2f), Vector3.zero, new Vector3(100f, 100f, 1f), rewardInformationTransform).GetComponent<CharacterUIObject>();
				currentCharacterUIObject.cachedSpriteGroup.setAlpha(1f);
			}
			switch (currentCollectEventRewardType)
			{
			case CollectEventManager.CollectEventRewardType.WarriorSkin:
			{
				CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)(long)m_currentCollectEventRewardData.rewardValue;
				currentCharacterUIObject.initCharacterUIObject(warriorSkinType, UIWindowCollectEvent.instance.cachedCanvasGroup);
				rewardTitleText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(warriorSkinType);
				rewardDetailText.text = I18NManager.Get("WARRIOR_LIMITED_SKIN");
				break;
			}
			case CollectEventManager.CollectEventRewardType.PriestSkin:
			{
				CharacterSkinManager.PriestSkinType priestSkinType = (CharacterSkinManager.PriestSkinType)(long)m_currentCollectEventRewardData.rewardValue;
				currentCharacterUIObject.initCharacterUIObject(priestSkinType, UIWindowCollectEvent.instance.cachedCanvasGroup);
				rewardTitleText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(priestSkinType);
				rewardDetailText.text = I18NManager.Get("PRIEST_LIMITED_SKIN");
				break;
			}
			case CollectEventManager.CollectEventRewardType.ArcherSkin:
			{
				CharacterSkinManager.ArcherSkinType archerSkinType = (CharacterSkinManager.ArcherSkinType)(long)m_currentCollectEventRewardData.rewardValue;
				currentCharacterUIObject.initCharacterUIObject(archerSkinType, UIWindowCollectEvent.instance.cachedCanvasGroup);
				rewardTitleText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(archerSkinType);
				rewardDetailText.text = I18NManager.Get("ARCHER_LIMITED_SKIN");
				break;
			}
			}
			currentCharacterUIObject.changeLayer("PopUpLayer2", 100);
		}
		else
		{
			bool flag = false;
			rewardIconImage.transform.localScale = Vector3.one;
			switch (currentCollectEventRewardType)
			{
			case CollectEventManager.CollectEventRewardType.Ruby:
				rewardIconImage.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
				rewardDetailText.text = I18NManager.Get("COUPON_REWARD_RUBY");
				break;
			case CollectEventManager.CollectEventRewardType.TreasureKey:
				rewardDetailText.text = I18NManager.Get("COUPON_REWARD_KEYS");
				break;
			case CollectEventManager.CollectEventRewardType.TranscendStone:
				rewardDetailText.text = I18NManager.Get("TRANSCEND_STONE");
				break;
			case CollectEventManager.CollectEventRewardType.HeartCoin:
				rewardIconImage.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
				rewardDetailText.text = I18NManager.Get("ELOPE_HEART_COIN");
				break;
			case CollectEventManager.CollectEventRewardType.WeaponSkinPiece:
				rewardIconImage.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
				rewardDetailText.text = I18NManager.Get("WEAPON_SKIN_PIECE_TITLE_TEXT");
				break;
			case CollectEventManager.CollectEventRewardType.WeaponSkinReinfecementMaterPiece:
				rewardDetailText.text = I18NManager.Get("WEAPON_SKIN_MASTER_PIECE_TITLE_TEXT");
				break;
			case CollectEventManager.CollectEventRewardType.WarriorSpecialWeaponSkin:
			case CollectEventManager.CollectEventRewardType.PriestSpecialWeaponSkin:
			case CollectEventManager.CollectEventRewardType.ArcherSpecialWeaponSkin:
				rewardIconImage.transform.localScale = new Vector3(4f, 4f, 1f);
				rewardDetailText.text = I18NManager.Get("SPECIAL_WEAPON_SKIN");
				flag = true;
				break;
			case CollectEventManager.CollectEventRewardType.HonorToken:
				rewardDetailText.text = I18NManager.Get("PVP_HONOR_TOKEN");
				break;
			}
			rewardTitleText.text = (((long)m_currentCollectEventRewardData.rewardValue > 1) ? m_currentCollectEventRewardData.rewardValue.ToString() : string.Empty);
			Sprite sprite = null;
			if (!flag)
			{
				sprite = Singleton<CollectEventManager>.instance.getRewardIcon(currentCollectEventRewardType);
			}
			else
			{
				switch (currentCollectEventRewardType)
				{
				case CollectEventManager.CollectEventRewardType.WarriorSpecialWeaponSkin:
					sprite = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite((WeaponSkinManager.WarriorSpecialWeaponSkinType)(long)m_currentCollectEventRewardData.rewardValue);
					rewardTitleText.text = I18NManager.Get("WARRIOR");
					break;
				case CollectEventManager.CollectEventRewardType.PriestSpecialWeaponSkin:
					sprite = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite((WeaponSkinManager.PriestSpecialWeaponSkinType)(long)m_currentCollectEventRewardData.rewardValue);
					rewardTitleText.text = I18NManager.Get("PRIEST");
					break;
				case CollectEventManager.CollectEventRewardType.ArcherSpecialWeaponSkin:
					sprite = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite((WeaponSkinManager.ArcherSpecialWeaponSkinType)(long)m_currentCollectEventRewardData.rewardValue);
					rewardTitleText.text = I18NManager.Get("ARCHER");
					break;
				}
			}
			rewardIconImage.sprite = sprite;
			rewardIconImage.SetNativeSize();
			rewardIconObject.SetActive(true);
		}
		if (slotTier < (int)Singleton<DataManager>.instance.currentGameData.collectEventTargetNextRewardTier)
		{
			backgroundImage.sprite = Singleton<CachedManager>.instance.collectEventRecievedBackgroundSprite;
			rewardTitleText.color = Util.getCalculatedColor(253f, 252f, 183f);
			rewardDetailText.color = Util.getCalculatedColor(253f, 252f, 183f);
			rewardButtonObject.SetActive(false);
			checkObject.SetActive(true);
			receivedObject.SetActive(true);
			return;
		}
		if (slotTier == (int)Singleton<DataManager>.instance.currentGameData.collectEventTargetNextRewardTier)
		{
			rewardButtonObject.SetActive(true);
			if ((long)Singleton<DataManager>.instance.currentGameData.collectEventResource >= (long)m_currentCollectEventRewardData.rewardNeedValue)
			{
				rewardButtonImage.sprite = Singleton<CachedManager>.instance.enableButtonOrangeSprite;
				rewardButtonText.color = Util.getCalculatedColor(253f, 254f, 156f);
			}
			else
			{
				rewardButtonImage.sprite = Singleton<CachedManager>.instance.enableButtonOrangeSprite;
				rewardButtonText.color = Util.getCalculatedColor(253f, 254f, 156f);
			}
		}
		else
		{
			rewardButtonObject.SetActive(false);
			rewardInformationTransform.anchoredPosition = new Vector2(0f, -15f);
			rewardButtonImage.sprite = Singleton<CachedManager>.instance.enableButtonOrangeSprite;
			rewardButtonText.color = Util.getCalculatedColor(253f, 254f, 156f);
		}
		backgroundImage.sprite = Singleton<CachedManager>.instance.collectEventBeforeRecieveBackgroundSprite;
		rewardTitleText.color = Util.getCalculatedColor(253f, 252f, 183f);
		rewardDetailText.color = Util.getCalculatedColor(253f, 252f, 183f);
		receivedObject.SetActive(false);
		checkObject.SetActive(false);
	}

	public void OnClickGetReward()
	{
		if (slotTier != (int)Singleton<DataManager>.instance.currentGameData.collectEventTargetNextRewardTier)
		{
			return;
		}
		if ((long)Singleton<DataManager>.instance.currentGameData.collectEventResource >= (long)m_currentCollectEventRewardData.rewardNeedValue)
		{
			switch (m_currentCollectEventRewardData.targetRewardType)
			{
			case CollectEventManager.CollectEventRewardType.Ruby:
				Singleton<FlyResourcesManager>.instance.playEffectResources(rewardButtonObject.transform, FlyResourcesManager.ResourceType.Ruby, 30L, 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<RubyManager>.instance.increaseRuby((long)m_currentCollectEventRewardData.rewardValue);
				break;
			case CollectEventManager.CollectEventRewardType.WarriorSkin:
			{
				CharacterSkinManager.WarriorSkinType skinType2 = (CharacterSkinManager.WarriorSkinType)(long)m_currentCollectEventRewardData.rewardValue;
				UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType2, !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType2).isHaving, false);
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType2);
				Singleton<CharacterManager>.instance.equipCharacter(skinType2);
				break;
			}
			case CollectEventManager.CollectEventRewardType.TreasureKey:
				Singleton<FlyResourcesManager>.instance.playEffectResources(rewardButtonObject.transform, FlyResourcesManager.ResourceType.TreasurePiece, 30L, 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<TreasureManager>.instance.increaseTreasurePiece(m_currentCollectEventRewardData.rewardValue);
				break;
			case CollectEventManager.CollectEventRewardType.ArcherSkin:
			{
				CharacterSkinManager.ArcherSkinType skinType3 = (CharacterSkinManager.ArcherSkinType)(long)m_currentCollectEventRewardData.rewardValue;
				UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType3, !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType3).isHaving, false);
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType3);
				Singleton<CharacterManager>.instance.equipCharacter(skinType3);
				break;
			}
			case CollectEventManager.CollectEventRewardType.TranscendStone:
				Singleton<FlyResourcesManager>.instance.playEffectResources(rewardButtonObject.transform, FlyResourcesManager.ResourceType.TranscendStone, 30L, 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<TranscendManager>.instance.increaseTranscendStone(m_currentCollectEventRewardData.rewardValue);
				break;
			case CollectEventManager.CollectEventRewardType.PriestSkin:
			{
				CharacterSkinManager.PriestSkinType skinType = (CharacterSkinManager.PriestSkinType)(long)m_currentCollectEventRewardData.rewardValue;
				UIWindowCharacterSkinLotteryResult.instance.openRandomCharacterSkinUI(skinType, !Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType).isHaving, false);
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(skinType);
				Singleton<CharacterManager>.instance.equipCharacter(skinType);
				break;
			}
			case CollectEventManager.CollectEventRewardType.HeartCoin:
				Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.HeartCoin, Mathf.Min((int)(long)m_currentCollectEventRewardData.rewardValue, 30), 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<ElopeModeManager>.instance.increaseHeartCoin(m_currentCollectEventRewardData.rewardValue);
				break;
			case CollectEventManager.CollectEventRewardType.WeaponSkinPiece:
				Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.WeaponSkinPiece, Mathf.Min((int)(long)m_currentCollectEventRewardData.rewardValue, 30), 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<WeaponSkinManager>.instance.increaseWeaponSkinPiece(m_currentCollectEventRewardData.rewardValue);
				break;
			case CollectEventManager.CollectEventRewardType.WeaponSkinReinfecementMaterPiece:
				Singleton<FlyResourcesManager>.instance.playEffectResources(Util.getCurrentScreenToWorldPosition(), FlyResourcesManager.ResourceType.WeaponSkinReinforcementMasterPiece, Mathf.Min((int)(long)m_currentCollectEventRewardData.rewardValue, 30), 0.02f, delegate
				{
					Singleton<AudioManager>.instance.playEffectSound("getgold");
				});
				Singleton<WeaponSkinManager>.instance.increaseWeaponSkinReinforcementMasterPiece(m_currentCollectEventRewardData.rewardValue);
				break;
			case CollectEventManager.CollectEventRewardType.WarriorSpecialWeaponSkin:
			{
				WeaponSkinData specialWeaponSkinData3 = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinData(CharacterManager.CharacterType.Warrior, (int)(long)m_currentCollectEventRewardData.rewardValue, WeaponSkinManager.WeaponSkinGradeType.Rare);
				Singleton<WeaponSkinManager>.instance.obtainWeaponSkin(specialWeaponSkinData3);
				UIWindowWeaponSkinLottery.instance.openLotteryUI(specialWeaponSkinData3, true);
				break;
			}
			case CollectEventManager.CollectEventRewardType.PriestSpecialWeaponSkin:
			{
				WeaponSkinData specialWeaponSkinData2 = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinData(CharacterManager.CharacterType.Priest, (int)(long)m_currentCollectEventRewardData.rewardValue, WeaponSkinManager.WeaponSkinGradeType.Rare);
				Singleton<WeaponSkinManager>.instance.obtainWeaponSkin(specialWeaponSkinData2);
				UIWindowWeaponSkinLottery.instance.openLotteryUI(specialWeaponSkinData2, true);
				break;
			}
			case CollectEventManager.CollectEventRewardType.ArcherSpecialWeaponSkin:
			{
				WeaponSkinData specialWeaponSkinData = Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinData(CharacterManager.CharacterType.Archer, (int)(long)m_currentCollectEventRewardData.rewardValue, WeaponSkinManager.WeaponSkinGradeType.Rare);
				Singleton<WeaponSkinManager>.instance.obtainWeaponSkin(specialWeaponSkinData);
				UIWindowWeaponSkinLottery.instance.openLotteryUI(specialWeaponSkinData, true);
				break;
			}
			case CollectEventManager.CollectEventRewardType.HonorToken:
				Singleton<PVPManager>.instance.increasePVPHonorToken(m_currentCollectEventRewardData.rewardValue);
				break;
			}
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			Singleton<AudioManager>.instance.playEffectSound("result_clear");
			ObjectPool.Spawn("@CollectEventRewardEffect", rewardIconObject.transform.position);
			++Singleton<DataManager>.instance.currentGameData.collectEventTargetNextRewardTier;
			Singleton<DataManager>.instance.saveData();
			UIWindowCollectEvent.instance.refreshAllSlot();
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_COLLECT_EVENT_RESOURCE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
		}
	}
}
