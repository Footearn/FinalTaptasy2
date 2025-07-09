using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowManageHeroAndWeapon : UIWindow
{
	public enum SelectTab
	{
		SelectTabWarrior = 1,
		SelectTabPriest,
		SelectTabArcher
	}

	public static UIWindowManageHeroAndWeapon instance;

	public WeaponInfiniteScroll weaponInfiniteScroll;

	public Action changeTabEvent;

	public GameObject[] selectedObjects;

	public Sprite nonSelecteBackgroundSprite;

	public Sprite selecteedBackgroundSprite;

	public Sprite noHaveBackgroundSprite;

	public SelectTab currentTab = SelectTab.SelectTabWarrior;

	public RectTransform warriorImage;

	public RectTransform priestImage;

	public RectTransform archerImage;

	public Text warriorDamageText;

	public Text priestDamageText;

	public Text archerDamageText;

	public Text warriorSecondStatText;

	public Text priestSecondStatText;

	public Text archerSecondStatText;

	public TextMeshProUGUI totalDamageText;

	public GameObject skinDescriptionObject;

	private UIWindowOutgame outgame;

	public override void Awake()
	{
		instance = this;
		base.Awake();
		I18NManager.changeLanguageNotification = (Action)Delegate.Combine(I18NManager.changeLanguageNotification, new Action(changeLanguageNotification));
	}

	public void refreshSkinDescriptionObject()
	{
		if (!TutorialManager.isTutorial)
		{
			skinDescriptionObject.SetActive(!Singleton<DataManager>.instance.currentGameData.isSeenCharacterSkinDescription);
		}
		else
		{
			skinDescriptionObject.SetActive(false);
		}
	}

	private void changeLanguageNotification()
	{
		outgame = UIWindowOutgame.instance;
		refreshCharacterInformation(currentTab, true);
	}

	public override bool OnBeforeOpen()
	{
		refreshSkinDescriptionObject();
		outgame = UIWindowOutgame.instance;
		refreshCharacterInformation(currentTab);
		refreshAllBuyState();
		StopAllCoroutines();
		selectTab(1);
		refreshTotalDamage();
		return base.OnBeforeOpen();
	}

	public void refreshTotalDamage()
	{
		totalDamageText.text = GameManager.changeUnit(Singleton<CharacterManager>.instance.totalDamage);
	}

	public void selectTab(int type)
	{
		for (int i = 0; i < selectedObjects.Length; i++)
		{
			selectedObjects[i].SetActive(false);
		}
		selectedObjects[type - 1].SetActive(true);
		if (UIWindowOutgame.instance.currentUIType != UIWindowOutgame.UIType.ManageHero)
		{
			UIWindowOutgame.instance.openUI(UIWindowOutgame.UIType.ManageHero);
		}
		SelectTab selectTab = currentTab;
		currentTab = (SelectTab)type;
		refreshCharacterInformation(currentTab);
		float num = ((!(weaponInfiniteScroll.ItemScale < 0f)) ? weaponInfiniteScroll.ItemScale : 131f);
		Singleton<WeaponManager>.instance.refreshWeaponSlotTypeList();
		switch (currentTab)
		{
		case SelectTab.SelectTabWarrior:
			weaponInfiniteScroll.refreshAll(CharacterManager.CharacterType.Warrior);
			weaponInfiniteScroll.refreshMaxCount(Singleton<WeaponManager>.instance.warriorSlotWeaponTypeList.Count);
			weaponInfiniteScroll.resetContentPosition(new Vector2(0f, (float)(Singleton<DataManager>.instance.currentGameData.warriorEquippedWeapon - 1) * num));
			break;
		case SelectTab.SelectTabPriest:
			if (TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType20);
			}
			weaponInfiniteScroll.refreshAll(CharacterManager.CharacterType.Priest);
			weaponInfiniteScroll.refreshMaxCount(Singleton<WeaponManager>.instance.priestSlotWeaponTypeList.Count);
			weaponInfiniteScroll.resetContentPosition(new Vector2(0f, (float)(Singleton<DataManager>.instance.currentGameData.priestEquippedWeapon - 1) * num));
			break;
		case SelectTab.SelectTabArcher:
			if (TutorialManager.isTutorial)
			{
				Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.TutorialType26);
			}
			weaponInfiniteScroll.refreshAll(CharacterManager.CharacterType.Archer);
			weaponInfiniteScroll.refreshMaxCount(Singleton<WeaponManager>.instance.archerSlotWeaponTypeList.Count);
			weaponInfiniteScroll.resetContentPosition(new Vector2(0f, (float)(Singleton<DataManager>.instance.currentGameData.archerEquippedWeapon - 1) * num));
			break;
		}
		StopAllCoroutines();
		if (selectTab != currentTab && changeTabEvent != null)
		{
			changeTabEvent();
		}
	}

	public void refreshCharacterInformation()
	{
		refreshCharacterInformation(currentTab);
	}

	public void refreshCharacterInformation(SelectTab type, bool isRefreshFromChangeLanguage = false)
	{
		Debug.LogError("refreshCharacterInformation 读取第一步");
		Singleton<StatManager>.instance.refreshAllStats();
		for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
		{
			if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
			{
				Singleton<CharacterManager>.instance.constCharacterList[i].resetProperties(false);
			}
		}

		warriorDamageText.text = GameManager.changeUnit(Singleton<CharacterManager>.instance.warriorCharacter.curDamage);
		warriorSecondStatText.text = GameManager.changeUnit(Singleton<CharacterManager>.instance.warriorCharacter.baseHealth);
		priestDamageText.text = GameManager.changeUnit(Singleton<CharacterManager>.instance.priestCharacter.curDamage);
		priestSecondStatText.text = GameManager.changeUnit(Singleton<StatManager>.instance.healValueEveryTap, false);
		archerDamageText.text = GameManager.changeUnit(Singleton<CharacterManager>.instance.archerCharacter.curDamage);
		archerSecondStatText.text = GameManager.changeUnit(Mathf.Min(Singleton<CharacterManager>.instance.archerCharacter.curCriticalChance, 100f), false) + "%";
		
		switch (type)
		{
		case SelectTab.SelectTabWarrior:
		{
			if (!isRefreshFromChangeLanguage)
			{
				outgame.menuTitleText.text = I18NManager.Get("WARRIOR");
			}
			CharacterSkinManager.WarriorSkinType equippedWarriorSkin = Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin;
			Singleton<CharacterSkinManager>.instance.warriorEquippedSkinData = Singleton<DataManager>.instance.currentGameData.warriorSkinData[(int)equippedWarriorSkin];
			break;
		}
		case SelectTab.SelectTabPriest:
		{
			if (!isRefreshFromChangeLanguage)
			{
				outgame.menuTitleText.text = I18NManager.Get("PRIEST");
			}
			CharacterSkinManager.PriestSkinType equippedPriestSkin = Singleton<DataManager>.instance.currentGameData.equippedPriestSkin;
			Singleton<CharacterSkinManager>.instance.priestEquippedSkinData = Singleton<DataManager>.instance.currentGameData.priestSkinData[(int)equippedPriestSkin];
			break;
		}
		case SelectTab.SelectTabArcher:
		{
			if (!isRefreshFromChangeLanguage)
			{
				outgame.menuTitleText.text = I18NManager.Get("ARCHER");
			}
			CharacterSkinManager.ArcherSkinType equippedArcherSkin = Singleton<DataManager>.instance.currentGameData.equippedArcherSkin;
			Singleton<CharacterSkinManager>.instance.archerEquippedSkinData = Singleton<DataManager>.instance.currentGameData.archerSkinData[(int)equippedArcherSkin];
			break;
		}
		}
	}

	public void refreshAllBuyState()
	{
		for (int i = 0; i < weaponInfiniteScroll.itemList.Count; i++)
		{
			WeaponSlot weaponSlot = weaponInfiniteScroll.itemList[i] as WeaponSlot;
			if (weaponSlot != null)
			{
				weaponSlot.refreshBuyState();
			}
		}
	}

	public void OnClickOpenWeaponSkin()
	{
		CharacterManager.CharacterType targetCharacterType = (CharacterManager.CharacterType)(currentTab - 1);
		UIWindowWeaponSkin.instance.openWeaponSkin(targetCharacterType);
	}
}
