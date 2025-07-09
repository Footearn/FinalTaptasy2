using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowLotteryBossRaidChest : UIWindow
{
	[Serializable]
	public class EnchantData
	{
		public EnchantTargetType targetEnchantTargetType;

		public CharacterManager.CharacterType targetCharacterType;

		public CharacterSkinManager.WarriorSkinType warriorCharacterSkinType;

		public CharacterSkinManager.PriestSkinType priestCharacterSkinType;

		public CharacterSkinManager.ArcherSkinType archerCharacterSkinType;

		public int baseLevel;

		public int currentLevel;
	}

	public enum EnchantTargetType
	{
		Character,
		WeaponSkin,
		Length
	}

	public static UIWindowLotteryBossRaidChest instance;

	public Sprite treasureKeyIconSprite;

	public Sprite treasureEnchantStoneIconSprite;

	public Sprite skillPointIconSprite;

	public Sprite rubyIconSprite;

	public Sprite goldIconSprite;

	public GameObject openAllObjects;

	public GameObject justOpenObjects;

	public GameObject[] rewardObjects;

	public GameObject rewardObjectParent;

	public GameObject openChestObjectParent;

	public Text goldNameText;

	public Text rubyNameText;

	public Text characterSkinNameText;

	public Image characterSkinImage;

	public Text characterSkinCharacterTypeText;

	public Text treasureEnchantStoneNameText;

	public Text treasureKeyNameText;

	public Text skillPointNameText;

	public Image nextChestImage;

	public RectTransform nextChestImageTransform;

	public GameObject nextOpenButtonObject;

	public GameObject goToTownButtonObject;

	public Animation openChestAnimation;

	public RectTransform currentChestTransform;

	public CanvasGroup currentChestCanvasGroup;

	public Image currentChestImage;

	public Image flashBlock;

	public List<BossRaidManager.BossRaidChestData> totalRaidChestDataList;

	public List<BossRaidManager.ChestRewardData> totalRewardDataList;

	public OptimizedScrollRect bossRaidRewardScrollRect;

	public RectTransform content;

	public GameObject openAllResultObject;

	public GameObject openAllOpenEffectObject;

	public GameObject[] openAllEffectObjects;

	public RectTransform chestOpenEffectTargetTransform;

	public bool isOpenAll;

	public CanvasGroup enchantNoticeBlockCanvasGroup;

	public Image enchantNoticeBlockImage;

	public GameObject enchantNoticeObject;

	public RectTransform enchantNoticeBackgroundRectTransform;

	public Text enchantedNameText;

	public Image enchantedImage;

	public Text enchantedLevelText;

	public Animation enchantNoticeAnimation;

	public Animation enchantNoticeBlockAnimation;

	private bool m_isCanContinue;

	private bool m_isCanGoNext;

	private bool m_isClosingEnchantedNotice;

	private List<EnchantData> m_allEnchantedRewardData = new List<EnchantData>();

	private List<EnchantData> m_targetEnchantedRewardData = new List<EnchantData>();

	private MersenneTwister m_characterRandom = new MersenneTwister();

	public override void Awake()
	{
		instance = this;
		totalRaidChestDataList = new List<BossRaidManager.BossRaidChestData>();
		base.Awake();
	}

	public void openLotteryBossRaidchest(bool openAll)
	{
		m_allEnchantedRewardData.Clear();
		totalRaidChestDataList.Clear();
		m_targetEnchantedRewardData.Clear();
		addAllEnchantTargetRewardInList();
		enchantNoticeObject.SetActive(false);
		enchantNoticeBlockCanvasGroup.alpha = 0f;
		for (int i = 0; i < Singleton<BossRaidManager>.instance.collectedBronzeChestList.Count; i++)
		{
			totalRaidChestDataList.Add(Singleton<BossRaidManager>.instance.collectedBronzeChestList[i]);
		}
		for (int j = 0; j < Singleton<BossRaidManager>.instance.collectedGoldChestList.Count; j++)
		{
			totalRaidChestDataList.Add(Singleton<BossRaidManager>.instance.collectedGoldChestList[j]);
		}
		for (int k = 0; k < Singleton<BossRaidManager>.instance.collectedDiaChestList.Count; k++)
		{
			totalRaidChestDataList.Add(Singleton<BossRaidManager>.instance.collectedDiaChestList[k]);
		}
		totalRewardDataList = Singleton<BossRaidManager>.instance.calculateChestDataToChestRewardData(totalRaidChestDataList);
		calculateData();
		m_isCanContinue = true;
		isOpenAll = openAll;
		if (!openAll)
		{
			openAllObjects.SetActive(false);
			justOpenObjects.SetActive(true);
			openChestObjectParent.SetActive(true);
			rewardObjectParent.SetActive(false);
			m_isCanGoNext = false;
			flashBlock.color = new Color(1f, 1f, 1f, 0f);
			rewardObjectParent.SetActive(false);
			resetCurrentChestImage();
			openNextRewardData();
			resetTexts();
			resetNextImage();
			refreshButtonStatus();
			open();
			openChestAnimation.Stop();
			currentChestCanvasGroup.alpha = 0f;
			currentChestTransform.anchoredPosition = new Vector2(0f, -130f);
			openChestAnimation.Play();
			return;
		}
		open();
		openAllOpenEffectObject.SetActive(true);
		openAllResultObject.SetActive(true);
		for (int l = 0; l < bossRaidRewardScrollRect.slotObjects.Count; l++)
		{
			if (bossRaidRewardScrollRect.slotObjects[l] != null)
			{
				ObjectPool.Recycle(bossRaidRewardScrollRect.slotObjects[l].name, bossRaidRewardScrollRect.slotObjects[l].cachedGameObject);
			}
		}
		bossRaidRewardScrollRect.slotObjects.Clear();
		openAllResultObject.SetActive(false);
		openAllResultObject.transform.localScale = new Vector3(1f, 1f, 1f);
		openAllObjects.SetActive(true);
		justOpenObjects.SetActive(false);
		double num = 0.0;
		long num2 = 0L;
		long num3 = 0L;
		long num4 = 0L;
		List<BossRaidManager.ChestRewardData> list = new List<BossRaidManager.ChestRewardData>();
		List<BossRaidManager.ChestRewardData> list2 = new List<BossRaidManager.ChestRewardData>();
		for (int m = 0; m < totalRewardDataList.Count; m++)
		{
			switch (totalRewardDataList[m].targetRewardType)
			{
			case BossRaidManager.BossRaidChestRewardType.Gold:
				num += totalRewardDataList[m].value;
				break;
			case BossRaidManager.BossRaidChestRewardType.Ruby:
				num2 += (long)totalRewardDataList[m].value;
				break;
			case BossRaidManager.BossRaidChestRewardType.CharacterSkin:
				list.Add(totalRewardDataList[m]);
				break;
			case BossRaidManager.BossRaidChestRewardType.TreasureEnchantStone:
				num3 += (long)totalRewardDataList[m].value;
				break;
			case BossRaidManager.BossRaidChestRewardType.TreasureKey:
				num4 += (long)totalRewardDataList[m].value;
				break;
			}
		}
		Dictionary<BossRaidManager.BossRaidChestType, List<BossRaidManager.ChestRewardData>> dictionary = new Dictionary<BossRaidManager.BossRaidChestType, List<BossRaidManager.ChestRewardData>>();
		for (int n = 1; n < 4; n++)
		{
			dictionary.Add((BossRaidManager.BossRaidChestType)n, new List<BossRaidManager.ChestRewardData>());
		}
		for (int num5 = 0; num5 < list.Count; num5++)
		{
			dictionary[list[num5].chestType].Add(list[num5]);
		}
		List<BossRaidManager.ChestRewardData> list3 = new List<BossRaidManager.ChestRewardData>();
		for (int num6 = 3; num6 > 0; num6--)
		{
			for (int num7 = 0; num7 < dictionary[(BossRaidManager.BossRaidChestType)num6].Count; num7++)
			{
				list3.Add(dictionary[(BossRaidManager.BossRaidChestType)num6][num7]);
			}
		}
		if (num4 > 0)
		{
			BossRaidManager.ChestRewardData chestRewardData = new BossRaidManager.ChestRewardData();
			chestRewardData.value = num4;
			chestRewardData.targetRewardType = BossRaidManager.BossRaidChestRewardType.TreasureKey;
			list3.Add(chestRewardData);
		}
		if (num3 > 0)
		{
			BossRaidManager.ChestRewardData chestRewardData2 = new BossRaidManager.ChestRewardData();
			chestRewardData2.value = num3;
			chestRewardData2.targetRewardType = BossRaidManager.BossRaidChestRewardType.TreasureEnchantStone;
			list3.Add(chestRewardData2);
		}
		if (num2 > 0)
		{
			BossRaidManager.ChestRewardData chestRewardData3 = new BossRaidManager.ChestRewardData();
			chestRewardData3.value = num2;
			chestRewardData3.targetRewardType = BossRaidManager.BossRaidChestRewardType.Ruby;
			list3.Add(chestRewardData3);
		}
		if (num > 0.0)
		{
			BossRaidManager.ChestRewardData chestRewardData4 = new BossRaidManager.ChestRewardData();
			chestRewardData4.value = num;
			chestRewardData4.targetRewardType = BossRaidManager.BossRaidChestRewardType.Gold;
			list3.Add(chestRewardData4);
		}
		int num8 = (int)Math.Ceiling((float)list3.Count / 3f);
		List<BossRaidRewardSlot> list4 = new List<BossRaidRewardSlot>();
		for (int num9 = 0; num9 < num8; num9++)
		{
			BossRaidRewardSlotSet component = ObjectPool.Spawn("@BossRaidRewardSlotSet", Vector2.zero, content).GetComponent<BossRaidRewardSlotSet>();
			component.cachedRectTransform.anchoredPosition = new Vector2(0f, -36 - num9 * 164);
			component.cachedRectTransform.localScale = new Vector3(1f, 1f, 1f);
			component.parentScrollRect = bossRaidRewardScrollRect;
			for (int num10 = 0; num10 < component.rewardSlots.Length; num10++)
			{
				list4.Add(component.rewardSlots[num10]);
			}
		}
		content.sizeDelta = new Vector2(536f, 50 + num8 * 164);
		for (int num11 = 0; num11 < list4.Count; num11++)
		{
			if (num11 < list3.Count)
			{
				list4[num11].refreshSlot(list3[num11]);
			}
			else
			{
				list4[num11].refreshSlot(new BossRaidManager.ChestRewardData());
			}
		}
		startOpenAllChests();
	}

	private void addAllEnchantTargetRewardInList()
	{
		for (int i = 0; i < 30; i++)
		{
			EnchantData enchantData = new EnchantData();
			enchantData.targetEnchantTargetType = EnchantTargetType.Character;
			enchantData.targetCharacterType = CharacterManager.CharacterType.Warrior;
			enchantData.warriorCharacterSkinType = (CharacterSkinManager.WarriorSkinType)i;
			enchantData.baseLevel = 0;
			enchantData.currentLevel = enchantData.baseLevel;
			m_allEnchantedRewardData.Add(enchantData);
		}
		for (int j = 0; j < 30; j++)
		{
			EnchantData enchantData2 = new EnchantData();
			enchantData2.targetEnchantTargetType = EnchantTargetType.Character;
			enchantData2.targetCharacterType = CharacterManager.CharacterType.Priest;
			enchantData2.priestCharacterSkinType = (CharacterSkinManager.PriestSkinType)j;
			enchantData2.baseLevel = 0;
			enchantData2.currentLevel = enchantData2.baseLevel;
			m_allEnchantedRewardData.Add(enchantData2);
		}
		for (int k = 0; k < 30; k++)
		{
			EnchantData enchantData3 = new EnchantData();
			enchantData3.targetEnchantTargetType = EnchantTargetType.Character;
			enchantData3.targetCharacterType = CharacterManager.CharacterType.Archer;
			enchantData3.archerCharacterSkinType = (CharacterSkinManager.ArcherSkinType)k;
			enchantData3.baseLevel = 0;
			enchantData3.currentLevel = enchantData3.baseLevel;
			m_allEnchantedRewardData.Add(enchantData3);
		}
	}

	private void startOpenAllChests()
	{
		int count = Singleton<BossRaidManager>.instance.collectedGoldChestList.Count;
		int count2 = Singleton<BossRaidManager>.instance.collectedDiaChestList.Count;
		for (int i = 0; i < openAllEffectObjects.Length; i++)
		{
			openAllEffectObjects[i].SetActive(false);
		}
		int num = 0;
		if (count2 > 0)
		{
			num = 2;
		}
		else if (count > 0)
		{
			num = 1;
		}
		openAllEffectObjects[num].SetActive(true);
		StopCoroutine("waitOpenAllChest");
		StartCoroutine("waitOpenAllChest");
	}

	private IEnumerator waitOpenAllChest()
	{
		yield return new WaitForSeconds(1f);
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		ObjectPool.Spawn("@BossChestOpenEffect", chestOpenEffectTargetTransform.position);
		StartCoroutine("flashUpdate");
		openAllOpenEffectObject.SetActive(false);
		openAllResultObject.SetActive(true);
	}

	private void refreshButtonStatus()
	{
		if (totalRewardDataList.Count >= 1)
		{
			nextOpenButtonObject.SetActive(true);
			goToTownButtonObject.SetActive(false);
		}
		else
		{
			nextOpenButtonObject.SetActive(false);
			goToTownButtonObject.SetActive(true);
		}
	}

	public void openChest()
	{
		if (!isOpenAll)
		{
			refreshButtonStatus();
			rewardObjectParent.SetActive(true);
			openChestObjectParent.SetActive(false);
			Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		}
	}

	private IEnumerator flashUpdate()
	{
		flashBlock.color = new Color(1f, 1f, 1f, 0.8f);
		flashBlock.gameObject.SetActive(true);
		Color color;
		while (true)
		{
			color = flashBlock.color;
			color.a -= Time.deltaTime;
			if (color.a <= 0f)
			{
				break;
			}
			flashBlock.color = color;
			yield return null;
		}
		color.a = 0f;
		flashBlock.color = color;
		flashBlock.gameObject.SetActive(false);
	}

	private void openNextRewardData()
	{
		for (int i = 0; i < rewardObjects.Length; i++)
		{
			rewardObjects[i].SetActive(false);
		}
		rewardObjects[(int)(totalRewardDataList[0].targetRewardType - 1)].SetActive(true);
		switch (totalRewardDataList[0].targetRewardType)
		{
		case BossRaidManager.BossRaidChestRewardType.Gold:
			goldNameText.text = string.Format(I18NManager.Get("BOSS_RAID_CHEST_GOLD_NAME_TEXT"), GameManager.changeUnit(totalRewardDataList[0].value));
			break;
		case BossRaidManager.BossRaidChestRewardType.Ruby:
			rubyNameText.text = string.Format(I18NManager.Get("BOSS_RAID_CHEST_RUBY_NAME_TEXT"), totalRewardDataList[0].value);
			break;
		case BossRaidManager.BossRaidChestRewardType.CharacterSkin:
			switch (totalRewardDataList[0].characterType)
			{
			case CharacterManager.CharacterType.Warrior:
				characterSkinNameText.text = I18NManager.Get("WARRIOR_SKIN_NAME_" + (int)(totalRewardDataList[0].warriorCharacterSkinType + 1));
				characterSkinImage.sprite = Singleton<CharacterSkinManager>.instance.warriorSkinSprites[(int)totalRewardDataList[0].warriorCharacterSkinType];
				characterSkinCharacterTypeText.text = "(" + I18NManager.Get("WARRIOR_NAME") + ")";
				break;
			case CharacterManager.CharacterType.Priest:
				characterSkinNameText.text = I18NManager.Get("PRIEST_SKIN_NAME_" + (int)(totalRewardDataList[0].priestCharacterSkinType + 1));
				characterSkinImage.sprite = Singleton<CharacterSkinManager>.instance.priestSkinSprites[(int)totalRewardDataList[0].priestCharacterSkinType];
				characterSkinCharacterTypeText.text = "(" + I18NManager.Get("PRIEST_NAME") + ")";
				break;
			case CharacterManager.CharacterType.Archer:
				characterSkinNameText.text = I18NManager.Get("ARCHER_SKIN_NAME_" + (int)(totalRewardDataList[0].archerCharacterSkinType + 1));
				characterSkinImage.sprite = Singleton<CharacterSkinManager>.instance.archerSkinSprites[(int)totalRewardDataList[0].archerCharacterSkinType];
				characterSkinCharacterTypeText.text = "(" + I18NManager.Get("ARCHER_NAME") + ")";
				break;
			}
			characterSkinImage.SetNativeSize();
			break;
		case BossRaidManager.BossRaidChestRewardType.TreasureEnchantStone:
			treasureEnchantStoneNameText.text = string.Format(I18NManager.Get("BOSS_RAID_CHEST_TREASURE_ENCHANT_STONE_NAME_TEXT"), totalRewardDataList[0].value);
			break;
		case BossRaidManager.BossRaidChestRewardType.TreasureKey:
			treasureKeyNameText.text = string.Format(I18NManager.Get("BOSS_RAID_CHEST_TREASURE_KEY_NAME_TEXT"), totalRewardDataList[0].value);
			break;
		}
		switch (totalRaidChestDataList[0].chestType)
		{
		case BossRaidManager.BossRaidChestType.Bronze:
			Singleton<BossRaidManager>.instance.collectedBronzeChestList.Remove(Singleton<BossRaidManager>.instance.collectedBronzeChestList[0]);
			UIWindowResult.instance.bronzeChestCountTextForBossRaid.text = Singleton<BossRaidManager>.instance.collectedBronzeChestList.Count.ToString();
			break;
		case BossRaidManager.BossRaidChestType.Gold:
			Singleton<BossRaidManager>.instance.collectedGoldChestList.Remove(Singleton<BossRaidManager>.instance.collectedGoldChestList[0]);
			UIWindowResult.instance.goldChestCountTextForBossRaid.text = Singleton<BossRaidManager>.instance.collectedGoldChestList.Count.ToString();
			break;
		case BossRaidManager.BossRaidChestType.Dia:
			Singleton<BossRaidManager>.instance.collectedDiaChestList.Remove(Singleton<BossRaidManager>.instance.collectedDiaChestList[0]);
			UIWindowResult.instance.diaChestCountTextForBossRaid.text = Singleton<BossRaidManager>.instance.collectedDiaChestList.Count.ToString();
			break;
		}
		totalRaidChestDataList.Remove(totalRaidChestDataList[0]);
		totalRewardDataList.Remove(totalRewardDataList[0]);
	}

	private void calculateData()
	{
		for (int i = 0; i < totalRewardDataList.Count; i++)
		{
			BossRaidManager.BossRaidChestRewardType targetRewardType = totalRewardDataList[i].targetRewardType;
			if (targetRewardType == BossRaidManager.BossRaidChestRewardType.CharacterSkin)
			{
				int num = UnityEngine.Random.Range(0, 3);
				CharacterManager.CharacterType characterType = CharacterManager.CharacterType.Length;
				switch (num)
				{
				case 0:
					characterType = CharacterManager.CharacterType.Warrior;
					break;
				case 1:
					characterType = CharacterManager.CharacterType.Priest;
					break;
				case 2:
					characterType = CharacterManager.CharacterType.Archer;
					break;
				}
				totalRewardDataList[i].characterType = characterType;
				switch (characterType)
				{
				case CharacterManager.CharacterType.Warrior:
				{
					int warriorCharacterSkinType = m_characterRandom.Next(5, 30);
					totalRewardDataList[i].warriorCharacterSkinType = (CharacterSkinManager.WarriorSkinType)warriorCharacterSkinType;
					break;
				}
				case CharacterManager.CharacterType.Priest:
				{
					int priestCharacterSkinType = m_characterRandom.Next(5, 30);
					totalRewardDataList[i].priestCharacterSkinType = (CharacterSkinManager.PriestSkinType)priestCharacterSkinType;
					break;
				}
				case CharacterManager.CharacterType.Archer:
				{
					int archerCharacterSkinType = m_characterRandom.Next(5, 30);
					totalRewardDataList[i].archerCharacterSkinType = (CharacterSkinManager.ArcherSkinType)archerCharacterSkinType;
					break;
				}
				}
			}
			rewardObtainEvent(totalRewardDataList[i]);
		}
	}

	private void rewardObtainEvent(BossRaidManager.ChestRewardData rewardData)
	{
		switch (rewardData.targetRewardType)
		{
		case BossRaidManager.BossRaidChestRewardType.Gold:
			Singleton<GoldManager>.instance.increaseGold(rewardData.value);
			break;
		case BossRaidManager.BossRaidChestRewardType.Ruby:
			Singleton<RubyManager>.instance.increaseRuby((long)rewardData.value);
			AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.GetDiamondFromBossRaid, new Dictionary<string, string>
			{
				{
					"DiamondCount",
					rewardData.value.ToString()
				}
			});
			break;
		case BossRaidManager.BossRaidChestRewardType.CharacterSkin:
			switch (rewardData.characterType)
			{
			case CharacterManager.CharacterType.Warrior:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(rewardData.warriorCharacterSkinType);
				AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.GetCharacterFromBossRaid, new Dictionary<string, string>
				{
					{
						"Warrior Character Skin Type",
						rewardData.warriorCharacterSkinType.ToString()
					}
				});
				break;
			case CharacterManager.CharacterType.Priest:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(rewardData.priestCharacterSkinType);
				AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.GetCharacterFromBossRaid, new Dictionary<string, string>
				{
					{
						"Priest Character Skin Type",
						rewardData.priestCharacterSkinType.ToString()
					}
				});
				break;
			case CharacterManager.CharacterType.Archer:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(rewardData.archerCharacterSkinType);
				AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.GetCharacterFromBossRaid, new Dictionary<string, string>
				{
					{
						"Archer Character Skin Type",
						rewardData.archerCharacterSkinType.ToString()
					}
				});
				break;
			}
			break;
		case BossRaidManager.BossRaidChestRewardType.TreasureEnchantStone:
			Singleton<TreasureManager>.instance.increaseTreasureEnchantStone((long)rewardData.value);
			AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.GetTreasureEnchantStoneFromBossRaid, new Dictionary<string, string>
			{
				{
					"TreasurEnchantStone",
					rewardData.value.ToString()
				}
			});
			break;
		case BossRaidManager.BossRaidChestRewardType.TreasureKey:
			Singleton<TreasureManager>.instance.increaseTreasurePiece((long)rewardData.value);
			AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.GetTreasureKeyFromBossRaid, new Dictionary<string, string>
			{
				{
					"TreasureKey",
					rewardData.value.ToString()
				}
			});
			break;
		}
	}

	private void resetNextImage()
	{
		if (totalRaidChestDataList.Count > 0)
		{
			switch (totalRaidChestDataList[0].chestType)
			{
			case BossRaidManager.BossRaidChestType.Bronze:
				nextChestImage.sprite = Singleton<BossRaidManager>.instance.bronzeChestSprite;
				nextChestImageTransform.anchoredPosition = new Vector2(-89.6f, 10.9f);
				break;
			case BossRaidManager.BossRaidChestType.Gold:
				nextChestImage.sprite = Singleton<BossRaidManager>.instance.goldChestSprite;
				nextChestImageTransform.anchoredPosition = new Vector2(-89.6f, 5.5f);
				break;
			case BossRaidManager.BossRaidChestType.Dia:
				nextChestImage.sprite = Singleton<BossRaidManager>.instance.diaChestSprite;
				nextChestImageTransform.anchoredPosition = new Vector2(-89.6f, 0.8f);
				break;
			}
		}
	}

	private void resetCurrentChestImage()
	{
		if (totalRaidChestDataList.Count > 0)
		{
			switch (totalRaidChestDataList[0].chestType)
			{
			case BossRaidManager.BossRaidChestType.Bronze:
				currentChestImage.sprite = Singleton<BossRaidManager>.instance.bronzeChestSprite;
				break;
			case BossRaidManager.BossRaidChestType.Gold:
				currentChestImage.sprite = Singleton<BossRaidManager>.instance.goldChestSprite;
				break;
			case BossRaidManager.BossRaidChestType.Dia:
				currentChestImage.sprite = Singleton<BossRaidManager>.instance.diaChestSprite;
				break;
			}
		}
	}

	private void resetTexts()
	{
		UIWindowResult.instance.bronzeChestCountTextForBossRaid.text = Singleton<BossRaidManager>.instance.collectedBronzeChestList.Count.ToString();
		UIWindowResult.instance.goldChestCountTextForBossRaid.text = Singleton<BossRaidManager>.instance.collectedGoldChestList.Count.ToString();
		UIWindowResult.instance.diaChestCountTextForBossRaid.text = Singleton<BossRaidManager>.instance.collectedDiaChestList.Count.ToString();
	}

	public void OnClickOpenNext()
	{
		rewardObjectParent.SetActive(false);
		openChestObjectParent.SetActive(true);
		openChestAnimation.Stop();
		currentChestCanvasGroup.alpha = 0f;
		flashBlock.color = new Color(1f, 1f, 1f, 0f);
		currentChestTransform.anchoredPosition = new Vector2(0f, -130f);
		openChestAnimation.Play();
		resetCurrentChestImage();
		openNextRewardData();
		resetNextImage();
		resetTexts();
		currentChestImage.SetNativeSize();
	}

	public void OnClickGoToTown()
	{
		if (m_isCanContinue)
		{
			m_isCanContinue = false;
			if (m_targetEnchantedRewardData.Count > 0)
			{
				showEnchantedNotice();
				Canvas.ForceUpdateCanvases();
			}
			else
			{
				UIWindowResult.instance.isCanContinue = true;
				UIWindowResult.instance.OnClickContinue();
			}
		}
	}

	private void showEnchantedNotice()
	{
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		m_isClosingEnchantedNotice = false;
		if (!enchantNoticeObject.activeSelf)
		{
			enchantNoticeObject.SetActive(true);
		}
		EnchantData enchantData = m_targetEnchantedRewardData[0];
		int baseLevel = enchantData.baseLevel;
		int currentLevel = enchantData.currentLevel;
		if (enchantData.targetEnchantTargetType == EnchantTargetType.Character)
		{
			enchantedLevelText.text = "lv." + baseLevel + " > <color=#FAD725>" + currentLevel + "</color>";
			enchantedImage.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
			switch (enchantData.targetCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				enchantedNameText.text = string.Format(I18NManager.Get("ENRICHMENT_STAT_UP"), I18NManager.Get("WARRIOR_SKIN_NAME_" + (int)(enchantData.warriorCharacterSkinType + 1)));
				enchantedImage.sprite = Singleton<CharacterSkinManager>.instance.warriorSkinSprites[(int)enchantData.warriorCharacterSkinType];
				break;
			case CharacterManager.CharacterType.Priest:
				enchantedNameText.text = string.Format(I18NManager.Get("ENRICHMENT_STAT_UP"), I18NManager.Get("PRIEST_SKIN_NAME_" + (int)(enchantData.priestCharacterSkinType + 1)));
				enchantedImage.sprite = Singleton<CharacterSkinManager>.instance.priestSkinSprites[(int)enchantData.priestCharacterSkinType];
				break;
			case CharacterManager.CharacterType.Archer:
				enchantedNameText.text = string.Format(I18NManager.Get("ENRICHMENT_STAT_UP"), I18NManager.Get("ARCHER_SKIN_NAME_" + (int)(enchantData.archerCharacterSkinType + 1)));
				enchantedImage.sprite = Singleton<CharacterSkinManager>.instance.archerSkinSprites[(int)enchantData.archerCharacterSkinType];
				break;
			}
		}
		enchantedImage.SetNativeSize();
		m_targetEnchantedRewardData.Remove(m_targetEnchantedRewardData[0]);
		enchantNoticeAnimation.Stop();
		enchantNoticeAnimation.Play("OpenEnchantEventUIAtBossRaid");
	}

	public void OnClickCloseEnchantedNotice()
	{
		if (!m_isClosingEnchantedNotice)
		{
			m_isClosingEnchantedNotice = true;
			enchantNoticeAnimation.Stop();
			enchantNoticeAnimation.Play("CloseEnchantEventUIAtBossRaid");
			if (m_targetEnchantedRewardData.Count <= 0)
			{
				m_isCanContinue = false;
				enchantNoticeBlockAnimation.Play("CloseLotteryBossRaidEnchantNoticeBlockAnimation");
				UIWindowResult.instance.isCanContinue = true;
				UIWindowResult.instance.OnClickContinue();
			}
		}
	}

	public void closedEnchantedNoticeEvent()
	{
		if (m_targetEnchantedRewardData.Count > 0)
		{
			showEnchantedNotice();
		}
	}
}
