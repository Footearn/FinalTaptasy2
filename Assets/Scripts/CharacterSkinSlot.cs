using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkinSlot : ScrollSlotItem
{
	public CharacterManager.CharacterType currentCharacterType;

	public CharacterSkinManager.WarriorSkinType currentWarriorSkinType;

	public CharacterSkinManager.PriestSkinType currentPriestSkinType;

	public CharacterSkinManager.ArcherSkinType currentArcherSkinType;

	public GameObject limitedObject;

	public Text ribbonText;

	public Text nameText;

	public Text priceText;

	public Image backgroundImage;

	public GameObject buyObject;

	public GameObject buyButtonObject;

	public GameObject cannotBuyDescriptionObject;

	public Text cannotBuyDescriptionText;

	public GameObject equipObject;

	public GameObject equippedObject;

	public CharacterUIObject currentCharacterUIObject;

	public GameObject noBonusObject;

	public GameObject haveBonusObject;

	public GameObject damageObject;

	public Text damageText;

	public GameObject secondStatObject;

	public Text secondStatText;

	public Image secondStatImage;

	public WarriorCharacterSkinData currentWarriorCharacterSkinData;

	public PriestCharacterSkinData currentPriestCharacterSkinData;

	public ArcherCharacterSkinData currentArcherCharacterSkinData;

	public Text levelText;

	public long rubyPrice;

	public string skinNameI18NTitleText;

	public PVPSkillManager.PVPSkillTypeData currentPVPSkillTypeData;

	public Image pvpSkillIconImage;

	private int nSlotIndex;

	public override void UpdateItem(int n)
	{
		nSlotIndex = n;
		switch (currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			try
			{
				currentWarriorSkinType = Singleton<CharacterSkinManager>.instance.sortedSkinList[currentCharacterType][n].warriorSkinType;
			}
			catch
			{
			}
			break;
		case CharacterManager.CharacterType.Priest:
			try
			{
				currentPriestSkinType = Singleton<CharacterSkinManager>.instance.sortedSkinList[currentCharacterType][n].priestSkinType;
			}
			catch
			{
			}
			break;
		case CharacterManager.CharacterType.Archer:
			try
			{
				currentArcherSkinType = Singleton<CharacterSkinManager>.instance.sortedSkinList[currentCharacterType][n].archerSkinType;
			}
			catch
			{
			}
			break;
		}
		refreshSlot();
	}

	public void UpdateItem(CharacterManager.CharacterType characterType)
	{
		currentCharacterType = characterType;
		UpdateItem(nSlotIndex);
	}

	public override void refreshSlot()
	{
		currentWarriorCharacterSkinData = null;
		currentPriestCharacterSkinData = null;
		currentArcherCharacterSkinData = null;
		bool flag = false;
		bool flag2 = false;
		string poolName = "@CharacterUIObject";
		Vector2 v = new Vector2(0f, 32.4f);
		switch (currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			v.x = 97f;
			break;
		case CharacterManager.CharacterType.Priest:
			v.x = 97.2f;
			break;
		case CharacterManager.CharacterType.Archer:
			v.x = 98.7f;
			break;
		}
		if (currentCharacterUIObject == null)
		{
			currentCharacterUIObject = ObjectPool.Spawn(poolName, v, base.cachedTransform).GetComponent<CharacterUIObject>();
		}
		currentCharacterUIObject.cachedTransform.localScale = new Vector3(100f, 100f, 1f);
		UIWindowCharacterSkin.instance.skinScroll.spriteMask.updateSprites();
		bool flag3 = false;
		switch (currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			PVPManager.PVPSkinData pVPSkinData2 = new PVPManager.PVPSkinData();
			pVPSkinData2.currentCharacterType = CharacterManager.CharacterType.Warrior;
			pVPSkinData2.currentWarriorSkinType = currentWarriorSkinType;
			currentPVPSkillTypeData = Singleton<PVPSkillManager>.instance.getSkillType(pVPSkinData2);
			skinNameI18NTitleText = "WARRIOR_SKIN_NAME_" + (int)(currentWarriorSkinType + 1);
			rubyPrice = Singleton<CharacterSkinManager>.instance.getRubyBuyPrice(currentWarriorSkinType);
			nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(currentWarriorSkinType);
			currentWarriorCharacterSkinData = Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(currentWarriorSkinType);
			flag = currentWarriorCharacterSkinData.isHaving;
			flag2 = Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin == currentWarriorSkinType;
			currentCharacterUIObject.initCharacterUIObject(currentWarriorSkinType, UIWindowCharacterSkin.instance.cachedCanvasGroup);
			priceText.text = Singleton<CharacterSkinManager>.instance.getRubyBuyPrice(currentWarriorSkinType).ToString("N0");
			if (flag)
			{
				levelText.text = "Lv." + currentWarriorCharacterSkinData._skinLevel.ToString("N0");
			}
			else
			{
				levelText.text = string.Empty;
			}
			if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(currentWarriorCharacterSkinData.skinType))
			{
				if (!limitedObject.activeSelf)
				{
					limitedObject.SetActive(true);
				}
				if (currentWarriorCharacterSkinData.skinType == CharacterSkinManager.WarriorSkinType.MasterWilliam)
				{
					ribbonText.text = I18NManager.Get("MASTER");
				}
				else
				{
					ribbonText.text = I18NManager.Get("LIMITED_SKIN");
				}
			}
			else if (Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(currentWarriorCharacterSkinData.skinType))
			{
				if (!limitedObject.activeSelf)
				{
					limitedObject.SetActive(true);
				}
				ribbonText.text = I18NManager.Get("EVENT");
			}
			else if (Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(currentWarriorCharacterSkinData.skinType))
			{
				if (!limitedObject.activeSelf)
				{
					limitedObject.SetActive(true);
				}
				ribbonText.text = I18NManager.Get("TRANSCEND_SKIN");
			}
			else if (limitedObject.activeSelf)
			{
				limitedObject.SetActive(false);
			}
			CharacterSkinStatData characterSkinStatData2 = Singleton<ParsingManager>.instance.currentParsedStatData.warriorCharacterSkinData[currentWarriorSkinType];
			flag3 = characterSkinStatData2.percentDamage != 0f || characterSkinStatData2.secondStat != 0f;
			if (characterSkinStatData2.percentDamage > 0f)
			{
				if (!damageObject.activeSelf)
				{
					damageObject.SetActive(true);
				}
				float num3 = characterSkinStatData2.percentDamage;
				if (flag)
				{
					num3 = characterSkinStatData2.percentDamage * (float)(long)currentWarriorCharacterSkinData._skinLevel;
				}
				damageText.text = "+" + (num3 + num3 / 100f * (float)Singleton<StatManager>.instance.characterSkinEffectBonus).ToString("N0") + "%";
			}
			else if (damageObject.activeSelf)
			{
				damageObject.SetActive(false);
			}
			if (characterSkinStatData2.secondStat > 0f)
			{
				if (!secondStatObject.activeSelf)
				{
					secondStatObject.SetActive(true);
				}
				float num4 = characterSkinStatData2.secondStat;
				if (flag)
				{
					num4 = characterSkinStatData2.secondStat * (float)(long)currentWarriorCharacterSkinData._skinLevel;
				}
				secondStatText.text = "+" + (num4 + num4 / 100f * (float)Singleton<StatManager>.instance.characterSkinEffectBonus).ToString("N0") + "%";
				secondStatImage.sprite = Singleton<CachedManager>.instance.warriorSecondStatIconSprite;
			}
			else if (secondStatObject.activeSelf)
			{
				secondStatObject.SetActive(false);
			}
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			PVPManager.PVPSkinData pVPSkinData3 = new PVPManager.PVPSkinData();
			pVPSkinData3.currentCharacterType = CharacterManager.CharacterType.Priest;
			pVPSkinData3.currentPriestSkinType = currentPriestSkinType;
			currentPVPSkillTypeData = Singleton<PVPSkillManager>.instance.getSkillType(pVPSkinData3);
			skinNameI18NTitleText = "PRIEST_SKIN_NAME_" + (int)(currentPriestSkinType + 1);
			rubyPrice = Singleton<CharacterSkinManager>.instance.getRubyBuyPrice(currentPriestSkinType);
			nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(currentPriestSkinType);
			currentPriestCharacterSkinData = Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(currentPriestSkinType);
			flag = currentPriestCharacterSkinData.isHaving;
			flag2 = Singleton<DataManager>.instance.currentGameData.equippedPriestSkin == currentPriestSkinType;
			currentCharacterUIObject.initCharacterUIObject(currentPriestSkinType, UIWindowCharacterSkin.instance.cachedCanvasGroup);
			priceText.text = Singleton<CharacterSkinManager>.instance.getRubyBuyPrice(currentPriestSkinType).ToString("N0");
			if (flag)
			{
				levelText.text = "Lv." + currentPriestCharacterSkinData._skinLevel.ToString("N0");
			}
			else
			{
				levelText.text = string.Empty;
			}
			if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(currentPriestCharacterSkinData.skinType))
			{
				if (!limitedObject.activeSelf)
				{
					limitedObject.SetActive(true);
				}
				if (currentPriestCharacterSkinData.skinType == CharacterSkinManager.PriestSkinType.MasterOlivia)
				{
					ribbonText.text = I18NManager.Get("MASTER");
				}
				else
				{
					ribbonText.text = I18NManager.Get("LIMITED_SKIN");
				}
			}
			else if (Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(currentPriestCharacterSkinData.skinType))
			{
				if (!limitedObject.activeSelf)
				{
					limitedObject.SetActive(true);
				}
				ribbonText.text = I18NManager.Get("EVENT");
			}
			else if (Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(currentPriestCharacterSkinData.skinType))
			{
				if (!limitedObject.activeSelf)
				{
					limitedObject.SetActive(true);
				}
				ribbonText.text = I18NManager.Get("TRANSCEND_SKIN");
			}
			else if (limitedObject.activeSelf)
			{
				limitedObject.SetActive(false);
			}
			CharacterSkinStatData characterSkinStatData3 = Singleton<ParsingManager>.instance.currentParsedStatData.priestCharacterSkinData[currentPriestSkinType];
			flag3 = characterSkinStatData3.percentDamage != 0f || characterSkinStatData3.secondStat != 0f;
			if (characterSkinStatData3.percentDamage > 0f)
			{
				if (!damageObject.activeSelf)
				{
					damageObject.SetActive(true);
				}
				float num5 = characterSkinStatData3.percentDamage;
				if (flag)
				{
					num5 = characterSkinStatData3.percentDamage * (float)(long)currentPriestCharacterSkinData._skinLevel;
				}
				damageText.text = "+" + (num5 + num5 / 100f * (float)Singleton<StatManager>.instance.characterSkinEffectBonus).ToString("N0") + "%";
			}
			else if (damageObject.activeSelf)
			{
				damageObject.SetActive(false);
			}
			if (characterSkinStatData3.secondStat > 0f)
			{
				if (!secondStatObject.activeSelf)
				{
					secondStatObject.SetActive(true);
				}
				float num6 = characterSkinStatData3.secondStat;
				if (flag)
				{
					num6 = characterSkinStatData3.secondStat * (float)(long)currentPriestCharacterSkinData._skinLevel;
				}
				secondStatText.text = "+" + (num6 + num6 / 100f * (float)Singleton<StatManager>.instance.characterSkinEffectBonus).ToString("N0") + "%";
				secondStatImage.sprite = Singleton<CachedManager>.instance.priestSecondStatIconSprite;
			}
			else if (secondStatObject.activeSelf)
			{
				secondStatObject.SetActive(false);
			}
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			PVPManager.PVPSkinData pVPSkinData = new PVPManager.PVPSkinData();
			pVPSkinData.currentCharacterType = CharacterManager.CharacterType.Archer;
			pVPSkinData.currentArcherSkinType = currentArcherSkinType;
			currentPVPSkillTypeData = Singleton<PVPSkillManager>.instance.getSkillType(pVPSkinData);
			skinNameI18NTitleText = "ARCHER_SKIN_NAME_" + (int)(currentArcherSkinType + 1);
			rubyPrice = Singleton<CharacterSkinManager>.instance.getRubyBuyPrice(currentArcherSkinType);
			nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(currentArcherSkinType);
			currentArcherCharacterSkinData = Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(currentArcherSkinType);
			flag = currentArcherCharacterSkinData.isHaving;
			flag2 = Singleton<DataManager>.instance.currentGameData.equippedArcherSkin == currentArcherSkinType;
			currentCharacterUIObject.initCharacterUIObject(currentArcherSkinType, UIWindowCharacterSkin.instance.cachedCanvasGroup);
			priceText.text = Singleton<CharacterSkinManager>.instance.getRubyBuyPrice(currentArcherSkinType).ToString("N0");
			if (flag)
			{
				levelText.text = "Lv." + currentArcherCharacterSkinData._skinLevel.ToString("N0");
			}
			else
			{
				levelText.text = string.Empty;
			}
			if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(currentArcherCharacterSkinData.skinType))
			{
				if (!limitedObject.activeSelf)
				{
					limitedObject.SetActive(true);
				}
				if (currentArcherCharacterSkinData.skinType == CharacterSkinManager.ArcherSkinType.MasterWindstoker)
				{
					ribbonText.text = I18NManager.Get("MASTER");
				}
				else
				{
					ribbonText.text = I18NManager.Get("LIMITED_SKIN");
				}
			}
			else if (Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(currentArcherCharacterSkinData.skinType))
			{
				if (!limitedObject.activeSelf)
				{
					limitedObject.SetActive(true);
				}
				ribbonText.text = I18NManager.Get("EVENT");
			}
			else if (Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(currentArcherCharacterSkinData.skinType))
			{
				if (!limitedObject.activeSelf)
				{
					limitedObject.SetActive(true);
				}
				ribbonText.text = I18NManager.Get("TRANSCEND_SKIN");
			}
			else if (limitedObject.activeSelf)
			{
				limitedObject.SetActive(false);
			}
			CharacterSkinStatData characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.archerCharacterSkinData[currentArcherSkinType];
			flag3 = characterSkinStatData.percentDamage != 0f || characterSkinStatData.secondStat != 0f;
			if (characterSkinStatData.percentDamage > 0f)
			{
				if (!damageObject.activeSelf)
				{
					damageObject.SetActive(true);
				}
				float num = characterSkinStatData.percentDamage;
				if (flag)
				{
					num = characterSkinStatData.percentDamage * (float)(long)currentArcherCharacterSkinData._skinLevel;
				}
				damageText.text = "+" + (num + num / 100f * (float)Singleton<StatManager>.instance.characterSkinEffectBonus).ToString("N0") + "%";
			}
			else if (damageObject.activeSelf)
			{
				damageObject.SetActive(false);
			}
			if (characterSkinStatData.secondStat > 0f)
			{
				if (!secondStatObject.activeSelf)
				{
					secondStatObject.SetActive(true);
				}
				float num2 = characterSkinStatData.secondStat;
				if (flag)
				{
					num2 = characterSkinStatData.secondStat * (float)(long)currentArcherCharacterSkinData._skinLevel;
				}
				secondStatText.text = "+" + (num2 + num2 / 100f * (float)Singleton<StatManager>.instance.characterSkinEffectBonus).ToString("N0") + "%";
				secondStatImage.sprite = Singleton<CachedManager>.instance.archerSecondStatIconSprite;
			}
			else if (secondStatObject.activeSelf)
			{
				secondStatObject.SetActive(false);
			}
			break;
		}
		}
		if (flag3)
		{
			if (noBonusObject.activeSelf)
			{
				noBonusObject.SetActive(false);
			}
			if (!haveBonusObject.activeSelf)
			{
				haveBonusObject.SetActive(true);
			}
		}
		else
		{
			if (!noBonusObject.activeSelf)
			{
				noBonusObject.SetActive(true);
			}
			if (haveBonusObject.activeSelf)
			{
				haveBonusObject.SetActive(false);
			}
		}
		secondStatImage.SetNativeSize();
		if (flag)
		{
			if (buyObject.activeSelf)
			{
				buyObject.SetActive(false);
			}
			nameText.color = Util.getCalculatedColor(30f, 199f, 255f);
			if (flag2)
			{
				backgroundImage.sprite = Singleton<CachedManager>.instance.enableThumbnailCharacterSkinEquippedSprite;
				if (equipObject.activeSelf)
				{
					equipObject.SetActive(false);
				}
				if (!equippedObject.activeSelf)
				{
					equippedObject.SetActive(true);
				}
			}
			else
			{
				backgroundImage.sprite = Singleton<CachedManager>.instance.enableThumbnailWeaponSprite;
				if (!equipObject.activeSelf)
				{
					equipObject.SetActive(true);
				}
				if (equippedObject.activeSelf)
				{
					equippedObject.SetActive(false);
				}
			}
		}
		else
		{
			nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
			backgroundImage.sprite = Singleton<CachedManager>.instance.disableButtonSprite;
			if (!buyObject.activeSelf)
			{
				buyObject.SetActive(true);
			}
			switch (currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				if (Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(currentWarriorSkinType))
				{
					if (!cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(true);
					}
					if (buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(false);
					}
					cannotBuyDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_SKIN_DESCRIPTION"), Singleton<TranscendManager>.instance.getTranscendCharacterUnlockTier(currentWarriorSkinType));
				}
				else if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(currentWarriorSkinType))
				{
					if (!cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(true);
					}
					if (buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(false);
					}
					if (currentWarriorSkinType == CharacterSkinManager.WarriorSkinType.MasterWilliam)
					{
						cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_TOWER_MODE_RANKING_SKIN_DESCRIPTION");
					}
					else if (currentWarriorSkinType == CharacterSkinManager.WarriorSkinType.Siegfried)
					{
						cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_PREORDER_SKIN_DESCRIPTION");
					}
					else
					{
						cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_LIMITED_SKIN_DESCRIPTION");
					}
				}
				else if (Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(currentWarriorSkinType))
				{
					if (!cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(true);
					}
					if (buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(false);
					}
					cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_EVENT_SKIN_DESCRIPTION");
				}
				else
				{
					if (!buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(true);
					}
					if (cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(false);
					}
				}
				break;
			case CharacterManager.CharacterType.Priest:
				if (Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(currentPriestSkinType))
				{
					if (!cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(true);
					}
					if (buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(false);
					}
					cannotBuyDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_SKIN_DESCRIPTION"), Singleton<TranscendManager>.instance.getTranscendCharacterUnlockTier(currentPriestSkinType));
				}
				else if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(currentPriestSkinType))
				{
					if (!cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(true);
					}
					if (buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(false);
					}
					if (currentPriestSkinType == CharacterSkinManager.PriestSkinType.MasterOlivia)
					{
						cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_TOWER_MODE_RANKING_SKIN_DESCRIPTION");
					}
					else if (currentPriestSkinType == CharacterSkinManager.PriestSkinType.Candy)
					{
						cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_PREORDER_SKIN_DESCRIPTION");
					}
					else
					{
						cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_LIMITED_SKIN_DESCRIPTION");
					}
				}
				else if (Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(currentPriestSkinType))
				{
					if (!cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(true);
					}
					if (buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(false);
					}
					cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_EVENT_SKIN_DESCRIPTION");
				}
				else
				{
					if (!buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(true);
					}
					if (cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(false);
					}
				}
				break;
			case CharacterManager.CharacterType.Archer:
				if (Singleton<CharacterSkinManager>.instance.isTranscendPremiumSkin(currentArcherSkinType))
				{
					if (!cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(true);
					}
					if (buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(false);
					}
					cannotBuyDescriptionText.text = string.Format(I18NManager.Get("TRANSCEND_SKIN_DESCRIPTION"), Singleton<TranscendManager>.instance.getTranscendCharacterUnlockTier(currentArcherSkinType));
				}
				else if (Singleton<CharacterSkinManager>.instance.isLimitedCharacterSkin(currentArcherSkinType))
				{
					if (!cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(true);
					}
					if (buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(false);
					}
					if (currentArcherSkinType == CharacterSkinManager.ArcherSkinType.MasterWindstoker)
					{
						cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_TOWER_MODE_RANKING_SKIN_DESCRIPTION");
					}
					else if (currentArcherSkinType == CharacterSkinManager.ArcherSkinType.Marauder)
					{
						cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_PREORDER_SKIN_DESCRIPTION");
					}
					else
					{
						cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_LIMITED_SKIN_DESCRIPTION");
					}
				}
				else if (Singleton<CharacterSkinManager>.instance.isEventCharacterSkin(currentArcherSkinType))
				{
					if (!cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(true);
					}
					if (buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(false);
					}
					cannotBuyDescriptionText.text = I18NManager.Get("CHARACTER_EVENT_SKIN_DESCRIPTION");
				}
				else
				{
					if (!buyButtonObject.activeSelf)
					{
						buyButtonObject.SetActive(true);
					}
					if (cannotBuyDescriptionObject.activeSelf)
					{
						cannotBuyDescriptionObject.SetActive(false);
					}
				}
				break;
			}
			if (equipObject.activeSelf)
			{
				equipObject.SetActive(false);
			}
			if (equippedObject.activeSelf)
			{
				equippedObject.SetActive(false);
			}
		}
		pvpSkillIconImage.sprite = Singleton<PVPSkillManager>.instance.getSkillIconSprite(currentPVPSkillTypeData);
	}

	public void OnClickEquipButton()
	{
		equipCharacterSkin();
	}

	public void OnClickBuySkinButton()
	{
		if (Singleton<DataManager>.instance.currentGameData._ruby >= rubyPrice)
		{
			UIWindowDialog.openDescriptionNotUsingI18N(string.Format(I18NManager.Get("CHARACTER_BUY_ASK_TEXT"), I18NManager.Get(skinNameI18NTitleText)), UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				Singleton<RubyManager>.instance.decreaseRuby(rubyPrice);
				Singleton<AudioManager>.instance.playEffectSound("btn_upgrade");
				switch (currentCharacterType)
				{
				case CharacterManager.CharacterType.Warrior:
					Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(currentWarriorSkinType);
					equipCharacterSkin();
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.BuySkinByDiamond, new Dictionary<string, string>
					{
						{
							"SkinType",
							currentWarriorSkinType.ToString()
						}
					});
					break;
				case CharacterManager.CharacterType.Archer:
					Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(currentArcherSkinType);
					equipCharacterSkin();
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.BuySkinByDiamond, new Dictionary<string, string>
					{
						{
							"SkinType",
							currentArcherSkinType.ToString()
						}
					});
					break;
				case CharacterManager.CharacterType.Priest:
					Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(currentPriestSkinType);
					equipCharacterSkin();
					AnalyzeManager.retention(AnalyzeManager.CategoryType.Diamond, AnalyzeManager.ActionType.BuySkinByDiamond, new Dictionary<string, string>
					{
						{
							"SkinType",
							currentPriestSkinType.ToString()
						}
					});
					break;
				}
			}, string.Empty);
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
		}
	}

	public virtual void equipCharacterSkin()
	{
		switch (currentCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			Singleton<CharacterManager>.instance.equipCharacter(currentWarriorSkinType);
			ObjectPool.Spawn("fx_character_upgrade", new Vector2(0f, -0.162f), new Vector3(0f, 0f, 180f), UIWindowCharacterSkin.instance.currentCharacterUIObject.cachedTransform);
			break;
		case CharacterManager.CharacterType.Priest:
			Singleton<CharacterManager>.instance.equipCharacter(currentPriestSkinType);
			ObjectPool.Spawn("fx_character_upgrade", new Vector2(0f, -0.162f), new Vector3(0f, 0f, 180f), UIWindowCharacterSkin.instance.currentCharacterUIObject.cachedTransform);
			break;
		case CharacterManager.CharacterType.Archer:
			Singleton<CharacterManager>.instance.equipCharacter(currentArcherSkinType);
			ObjectPool.Spawn("fx_character_upgrade", new Vector2(0f, -0.162f), new Vector3(0f, 0f, 180f), UIWindowCharacterSkin.instance.currentCharacterUIObject.cachedTransform);
			break;
		}
		UIWindowCharacterSkin.instance.openCharacterSkinUI(UIWindowCharacterSkin.instance.currentType);
		Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
		UIWindowCharacterSkin.instance.skinScroll.refreshAll();
		UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
		Singleton<DataManager>.instance.saveData();
	}

	public void OnClickOpenPVPSimpleInformation()
	{
		if (currentPVPSkillTypeData != null)
		{
			UIWindowCharacterSkin.instance.skinScroll.parentScrollRect.horizontal = false;
			UIWindowPVPSimpleSkillInformation.instance.openSkillInformation(currentPVPSkillTypeData, delegate
			{
				UIWindowCharacterSkin.instance.skinScroll.parentScrollRect.horizontal = true;
			});
		}
	}
}
