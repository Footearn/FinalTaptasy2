using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowWeaponSkin : UIWindow
{
	public static UIWindowWeaponSkin instance;

	public CharacterManager.CharacterType currentTabCharacterType;

	public Dictionary<CharacterManager.CharacterType, Dictionary<int, List<WeaponSkinData>>> sortedSkinDictionary = new Dictionary<CharacterManager.CharacterType, Dictionary<int, List<WeaponSkinData>>>();

	public GameObject scrollObject;

	public GameObject emptyObject;

	public InfiniteScroll weaponSkinScroll;

	public Image warriorTabBackgroundImage;

	public Image priestTabBackgroundImage;

	public Image archerTabBackgroundImage;

	public Text warriorTabTitleText;

	public Text priestTabTitleText;

	public Text archerTabTitleText;

	public Text premiumLotteryPriceText;

	public WeaponSkinData newSkinData;

	public GameObject normalLotteryButtonObject;

	public GameObject freeLotteryButtonObject;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void OnClickOpenWeaponSkin(int type)
	{
		openWeaponSkin((CharacterManager.CharacterType)type);
	}

	public void openWeaponSkin(CharacterManager.CharacterType targetCharacterType)
	{
		string marketPrice = Singleton<PaymentManager>.instance.GetMarketPrice(ShopManager.LimitedShopItemType.WeaponSkinPremiumLottery);
		if (marketPrice.Length > 0)
		{
			premiumLotteryPriceText.text = marketPrice;
		}
		else
		{
			premiumLotteryPriceText.text = "$5.00";
		}
		normalLotteryButtonObject.SetActive(Singleton<DataManager>.instance.currentGameData.isUsedFreeWeaponSkinLottery);
		freeLotteryButtonObject.SetActive(!Singleton<DataManager>.instance.currentGameData.isUsedFreeWeaponSkinLottery);
		Singleton<WeaponSkinManager>.instance.displayWeaponSkinPiece();
		Singleton<WeaponSkinManager>.instance.displayWeaponReinforcementMasterPiece();
		currentTabCharacterType = targetCharacterType;
		switch (currentTabCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			warriorTabBackgroundImage.sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
			priestTabBackgroundImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
			archerTabBackgroundImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
			warriorTabTitleText.color = Color.white;
			priestTabTitleText.color = Util.getCalculatedColor(65f, 133f, 178f);
			archerTabTitleText.color = Util.getCalculatedColor(65f, 133f, 178f);
			break;
		case CharacterManager.CharacterType.Priest:
			warriorTabBackgroundImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
			priestTabBackgroundImage.sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
			archerTabBackgroundImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
			warriorTabTitleText.color = Util.getCalculatedColor(65f, 133f, 178f);
			priestTabTitleText.color = Color.white;
			archerTabTitleText.color = Util.getCalculatedColor(65f, 133f, 178f);
			break;
		case CharacterManager.CharacterType.Archer:
			warriorTabBackgroundImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
			priestTabBackgroundImage.sprite = Singleton<CachedManager>.instance.shopPopupTabNoneSelectSprite;
			archerTabBackgroundImage.sprite = Singleton<CachedManager>.instance.shopPopupTabSelectedSprite;
			warriorTabTitleText.color = Util.getCalculatedColor(65f, 133f, 178f);
			priestTabTitleText.color = Util.getCalculatedColor(65f, 133f, 178f);
			archerTabTitleText.color = Color.white;
			break;
		}
		sortSkinDictionary();
		if (sortedSkinDictionary[targetCharacterType].Count > 0)
		{
			if (!scrollObject.activeSelf)
			{
				scrollObject.SetActive(true);
			}
			if (emptyObject.activeSelf)
			{
				emptyObject.SetActive(false);
			}
			weaponSkinScroll.refreshMaxCount(sortedSkinDictionary[targetCharacterType].Count);
			if (!isOpen)
			{
				weaponSkinScroll.resetContentPosition(Vector2.zero);
			}
			weaponSkinScroll.syncAllSlotIndexFromPosition();
		}
		else
		{
			if (scrollObject.activeSelf)
			{
				scrollObject.SetActive(false);
			}
			if (!emptyObject.activeSelf)
			{
				emptyObject.SetActive(true);
			}
		}
		open();
	}

	public void sortSkinDictionary()
	{
		sortedSkinDictionary.Clear();
		for (int i = 0; i < 3; i++)
		{
			CharacterManager.CharacterType key = (CharacterManager.CharacterType)i;
			List<WeaponSkinData> list = Singleton<DataManager>.instance.currentGameData.weaponSkinData[key];
			List<WeaponSkinData> list2 = new List<WeaponSkinData>();
			List<WeaponSkinData> list3 = new List<WeaponSkinData>();
			List<WeaponSkinData> list4 = new List<WeaponSkinData>();
			List<WeaponSkinData> list5 = new List<WeaponSkinData>();
			for (int j = 0; j < list.Count; j++)
			{
				switch (list[j].currentGrade)
				{
				case WeaponSkinManager.WeaponSkinGradeType.Rare:
					list5.Add(list[j]);
					break;
				case WeaponSkinManager.WeaponSkinGradeType.Epic:
					list4.Add(list[j]);
					break;
				case WeaponSkinManager.WeaponSkinGradeType.Unique:
					list3.Add(list[j]);
					break;
				case WeaponSkinManager.WeaponSkinGradeType.Legendary:
					list2.Add(list[j]);
					break;
				}
			}
			list.Clear();
			for (int k = 0; k < list2.Count; k++)
			{
				list.Add(list2[k]);
			}
			for (int l = 0; l < list3.Count; l++)
			{
				list.Add(list3[l]);
			}
			for (int m = 0; m < list4.Count; m++)
			{
				list.Add(list4[m]);
			}
			for (int n = 0; n < list5.Count; n++)
			{
				list.Add(list5[n]);
			}
			sortedSkinDictionary.Add(key, new Dictionary<int, List<WeaponSkinData>>());
			int num = 0;
			int num2 = (int)Math.Ceiling((float)list.Count / 5f);
			for (int num3 = 0; num3 < num2; num3++)
			{
				sortedSkinDictionary[key][num3] = new List<WeaponSkinData>();
				for (int num4 = 0; num4 < 5; num4++)
				{
					if (num < list.Count)
					{
						sortedSkinDictionary[key][num3].Add(list[num]);
					}
					num++;
				}
			}
		}
	}

	public void OnClickNormalLottery()
	{
		//TODO 制作武器皮肤
		UIWindowDialog.openDescription("NORMAL_WEAPON_SKIN_LOTTERY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			if ((long)Singleton<DataManager>.instance.currentGameData.weaponSkinPiece >= (long)WeaponSkinManager.weaponSkinLotteryWeaponSkinPiecePrice)
			{
				Singleton<WeaponSkinManager>.instance.decreaseWeaponSkinPiece(WeaponSkinManager.weaponSkinLotteryWeaponSkinPiecePrice);
				Singleton<WeaponSkinManager>.instance.startLottery(false);
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_WEAPON_SKIN_PIECE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		}, string.Empty);
	}

	public void OnClickFreeLottery()
	{
		if ((bool)Singleton<DataManager>.instance.currentGameData.isUsedFreeWeaponSkinLottery)
		{
			OnClickNormalLottery();
			return;
		}
		UIWindowDialog.openDescription("NORMAL_WEAPON_SKIN_FREE_LOTTERY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			Singleton<DataManager>.instance.currentGameData.isUsedFreeWeaponSkinLottery = true;
			Singleton<WeaponSkinManager>.instance.startLottery(false);
		}, string.Empty);
	}

	public void OnClickPremiumLottery()
	{
		UIWindowDialog.openDescription("PREMIUM_WEAPON_SKIN_LOTTERY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			Singleton<PaymentManager>.instance.Purchase(ShopManager.LimitedShopItemType.WeaponSkinPremiumLottery, delegate
			{
				Singleton<WeaponSkinManager>.instance.startLottery(true);
			});
		}, string.Empty);
	}
}
