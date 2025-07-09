using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : ScrollSlotItem
{
	public StatManager.WeaponStatType firstStatType;

	public StatManager.WeaponStatType secondStatType;

	public WeaponInfiniteScroll weaponInfiniteScroll;

	public CharacterManager.CharacterType weaponCharacterType;

	public WeaponManager.WarriorWeaponType currentWarriorWeaponType;

	public WeaponManager.ArcherWeaponType currentArcherWeaponType;

	public WeaponManager.PriestWeaponType currentPriestWeaponType;

	public bool equippedThisWeapon;

	public bool isHaving;

	public bool isNew;

	public long currentLevel;

	public long maxLevel;

	[HideInInspector]
	public WarriorWeaponInventoryData currentWeaponOfWarriorData;

	[HideInInspector]
	public ArcherWeaponInventoryData currentWeaponOfArcherData;

	[HideInInspector]
	public PriestWeaponInventoryData currentWeaponOfPriestData;

	public GameObject equippedIcon;

	public GameObject slotLockedObject;

	public GameObject unlockObject;

	public GameObject noHaveObject;

	public GameObject haveObject;

	public GameObject equipObject;

	public GameObject upgradeButtonObject;

	public Text nameText;

	public Text enchantCountText;

	public Text damageText;

	public Text secondStatText;

	public Text upgradePriceText;

	public Text unlockPriceText;

	public Image damageIconImage;

	public Image secondStatIconImage;

	public Image weaponImage;

	public RectTransform weaponIconTransform;

	public Image weaponBackGroundImage;

	public Image backGroundImage;

	public Image innerBackground;

	public Image[] buttonImages;

	public Animation unlockButtonCantBuyAnimation;

	public Animation upgradeButtonCantBuyAnimation;

	public Button upgradeButton;

	public RectTransform weaponBackgrooundTransform;

	public Image upgradeButtonGoldIconImage;

	public Image unlockButtonGoldIconImage;

	public Text upgradeButtonTitleText;

	public Text unlockButtonTitleText;

	public Image upgradeButtonBackgroundImage;

	public Image unlockButtonBackgroundImage;

	[HideInInspector]
	public WeaponStat currentStatDataOfWeapon;

	public GameObject requiredLevelObject;

	public GameObject unlockButtonObject;

	public Image requiredLevelProgressBarImage;

	public ParticleSystem upgradeEffect;

	public ParticleSystem unlockEffect;

	public GameObject unlockableEffectObject;

	public double upgradePrice;

	public GameObject quickUpgradeButtonObject;

	public Animation quickUpgradeButtonAnimation;

	public Text quickUpgradeTotalPriceText;

	public bool isQuickUpgrading;

	private float m_quickUpgradeDisappearTimer;

	public void resetAllTypeNull()
	{
		currentWarriorWeaponType = WeaponManager.WarriorWeaponType.Null;
		currentArcherWeaponType = WeaponManager.ArcherWeaponType.Null;
		currentPriestWeaponType = WeaponManager.PriestWeaponType.Null;
	}

	private void Awake()
	{
		weaponCharacterType = CharacterManager.CharacterType.Warrior;
	}

	private void Start()
	{
		UIWindowManageHeroAndWeapon instance = UIWindowManageHeroAndWeapon.instance;
		instance.changeTabEvent = (Action)Delegate.Combine(instance.changeTabEvent, (Action)delegate
		{
			closeQuickUpgradeButton(true);
		});
	}

	public override void UpdateItem(int n)
	{
		base.UpdateItem(n);
		if (weaponInfiniteScroll == null)
		{
			weaponInfiniteScroll = base.cachedTransform.parent.GetComponent<WeaponInfiniteScroll>();
		}
		resetAllTypeNull();
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			if (Singleton<WeaponManager>.instance.warriorSlotWeaponTypeList.Count > slotIndex)
			{
				currentWarriorWeaponType = Singleton<WeaponManager>.instance.warriorSlotWeaponTypeList[slotIndex];
				break;
			}
			return;
		case CharacterManager.CharacterType.Priest:
			if (Singleton<WeaponManager>.instance.priestSlotWeaponTypeList.Count > slotIndex)
			{
				currentPriestWeaponType = Singleton<WeaponManager>.instance.priestSlotWeaponTypeList[slotIndex];
				break;
			}
			return;
		case CharacterManager.CharacterType.Archer:
			if (Singleton<WeaponManager>.instance.archerSlotWeaponTypeList.Count > slotIndex)
			{
				currentArcherWeaponType = Singleton<WeaponManager>.instance.archerSlotWeaponTypeList[slotIndex];
				break;
			}
			return;
		}
		initSlot(n);
	}

	public void UpdateItem(CharacterManager.CharacterType w)
	{
		weaponCharacterType = w;
		UpdateItem(slotIndex);
	}

	public void initSlot(int slotIndex)
	{
		if (slotIndex % 2 == 0)
		{
			backGroundImage.color = Util.getCalculatedColor(0f, 11f, 28f, 51f);
		}
		else
		{
			backGroundImage.color = new Color(0f, 0f, 0f, 0f);
		}
		refreshBuyState();
		refreshEquippedState();
		refreshSlot();
	}

	public void refreshBuyState()
	{
		if (unlockableEffectObject.activeSelf)
		{
			unlockableEffectObject.SetActive(false);
		}
		double num = 0.0;
		double num2 = 0.0;
		bool flag = false;
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			num = Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(currentWarriorWeaponType), currentWeaponOfWarriorData.enchantCount);
			num2 = Singleton<WeaponManager>.instance.getWeaponGoldPriceForUnlock(Singleton<WeaponManager>.instance.getWeaponTier(currentWarriorWeaponType));
			flag = Singleton<WeaponManager>.instance.getWeaponFromInventory(currentWarriorWeaponType).isHaving;
			break;
		case CharacterManager.CharacterType.Priest:
			num = Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(currentPriestWeaponType), currentWeaponOfPriestData.enchantCount);
			num2 = Singleton<WeaponManager>.instance.getWeaponGoldPriceForUnlock(Singleton<WeaponManager>.instance.getWeaponTier(currentPriestWeaponType));
			flag = Singleton<WeaponManager>.instance.getWeaponFromInventory(currentPriestWeaponType).isHaving;
			break;
		case CharacterManager.CharacterType.Archer:
			num = Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(currentArcherWeaponType), currentWeaponOfArcherData.enchantCount);
			num2 = Singleton<WeaponManager>.instance.getWeaponGoldPriceForUnlock(Singleton<WeaponManager>.instance.getWeaponTier(currentArcherWeaponType));
			flag = Singleton<WeaponManager>.instance.getWeaponFromInventory(currentArcherWeaponType).isHaving;
			break;
		}
		if (Singleton<DataManager>.instance.currentGameData.gold >= num)
		{
			upgradePriceText.color = Color.white;
			upgradeButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.enableButtonOrangeSprite;
			upgradeButtonGoldIconImage.color = Color.white;
			upgradeButtonTitleText.color = Util.getCalculatedColor(253f, 254f, 156f);
		}
		else
		{
			upgradePriceText.color = Util.getCalculatedColor(153f, 153f, 153f);
			upgradeButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.disableButtonSprite;
			upgradeButtonGoldIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
			upgradeButtonTitleText.color = Util.getCalculatedColor(153f, 153f, 153f);
		}
		bool flag2 = false;
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			flag2 = currentWeaponOfWarriorData.isUnlockedFromSlot;
			break;
		case CharacterManager.CharacterType.Priest:
			flag2 = currentWeaponOfPriestData.isUnlockedFromSlot;
			break;
		case CharacterManager.CharacterType.Archer:
			flag2 = currentWeaponOfArcherData.isUnlockedFromSlot;
			break;
		}
		if (flag || !flag2 || !unlockButtonObject.activeSelf)
		{
			return;
		}
		if (Singleton<DataManager>.instance.currentGameData.gold >= num2)
		{
			if (!unlockableEffectObject.activeSelf)
			{
				unlockableEffectObject.SetActive(true);
			}
			unlockPriceText.color = Color.white;
			unlockButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.enableButtonGreenSprite;
			unlockButtonGoldIconImage.color = Color.white;
			unlockButtonTitleText.color = Util.getCalculatedColor(230f, 255f, 153f);
		}
		else
		{
			unlockPriceText.color = Util.getCalculatedColor(153f, 153f, 153f);
			unlockButtonBackgroundImage.sprite = Singleton<CachedManager>.instance.disableButtonSprite;
			unlockButtonGoldIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
			unlockButtonTitleText.color = Util.getCalculatedColor(153f, 153f, 153f);
		}
	}

	private void OnEnable()
	{
		closeQuickUpgradeButton(true);
	}

	public override void refreshSlot()
	{
		if (slotLockedObject.activeSelf)
		{
			slotLockedObject.SetActive(false);
		}
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			currentWeaponOfWarriorData = Singleton<WeaponManager>.instance.getWeaponFromInventory(currentWarriorWeaponType);
			currentStatDataOfWeapon = Singleton<WeaponManager>.instance.getWeaponStat(currentWarriorWeaponType, currentWeaponOfWarriorData.enchantCount);
			secondStatIconImage.sprite = Singleton<WeaponManager>.instance.warriorSecondStatIconSprite;
			equippedThisWeapon = Singleton<DataManager>.instance.currentGameData.warriorEquippedWeapon == currentWeaponOfWarriorData.weapontype;
			isHaving = Singleton<WeaponManager>.instance.containInventory(currentWeaponOfWarriorData.weapontype);
			if (currentWarriorWeaponType > WeaponManager.WarriorWeaponType.WarriorWeapon1)
			{
				WeaponManager.WarriorWeaponType type2 = currentWarriorWeaponType - 1;
				WarriorWeaponInventoryData weaponFromInventory2 = Singleton<WeaponManager>.instance.getWeaponFromInventory(type2);
				if (weaponFromInventory2.enchantCount >= Singleton<WeaponManager>.instance.getUnlockNextRequiredLevel())
				{
					if (requiredLevelObject.activeSelf)
					{
						requiredLevelObject.SetActive(false);
					}
					if (!unlockButtonObject.activeSelf)
					{
						unlockButtonObject.SetActive(true);
					}
				}
				else
				{
					requiredLevelProgressBarImage.fillAmount = (float)weaponFromInventory2.enchantCount / (float)Singleton<WeaponManager>.instance.getUnlockNextRequiredLevel();
					if (!requiredLevelObject.activeSelf)
					{
						requiredLevelObject.SetActive(true);
					}
					if (unlockButtonObject.activeSelf)
					{
						unlockButtonObject.SetActive(false);
					}
				}
			}
			else
			{
				if (requiredLevelObject.activeSelf)
				{
					requiredLevelObject.SetActive(false);
				}
				if (!unlockButtonObject.activeSelf)
				{
					unlockButtonObject.SetActive(true);
				}
			}
			if (!currentWeaponOfWarriorData.isUnlockedFromSlot)
			{
				if (unlockObject.activeSelf)
				{
					unlockObject.SetActive(false);
				}
				if (noHaveObject.activeSelf)
				{
					noHaveObject.SetActive(false);
				}
				if (haveObject.activeSelf)
				{
					haveObject.SetActive(false);
				}
				if (!slotLockedObject.activeSelf)
				{
					slotLockedObject.SetActive(true);
				}
				if (equipObject.activeSelf)
				{
					equipObject.SetActive(false);
				}
				nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
				enchantCountText.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageText.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatText.color = Util.getCalculatedColor(153f, 153f, 153f);
				weaponImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
			}
			else if (isHaving)
			{
				if (unlockObject.activeSelf)
				{
					unlockObject.SetActive(false);
				}
				if (noHaveObject.activeSelf)
				{
					noHaveObject.SetActive(false);
				}
				if (equippedThisWeapon)
				{
					if (!haveObject.activeSelf)
					{
						haveObject.SetActive(true);
					}
					nameText.color = Util.getCalculatedColor(253f, 252f, 183f);
					enchantCountText.color = Util.getCalculatedColor(250f, 215f, 37f);
					damageText.color = Color.white;
					secondStatText.color = Color.white;
					weaponImage.color = Color.white;
					damageIconImage.color = Color.white;
					secondStatIconImage.color = Color.white;
					if (equipObject.activeSelf)
					{
						equipObject.SetActive(false);
					}
				}
				else
				{
					if (haveObject.activeSelf)
					{
						haveObject.SetActive(false);
					}
					nameText.color = Util.getCalculatedColor(30f, 199f, 255f);
					enchantCountText.color = Util.getCalculatedColor(30f, 199f, 255f);
					damageText.color = Util.getCalculatedColor(30f, 199f, 255f);
					secondStatText.color = Util.getCalculatedColor(30f, 199f, 255f);
					weaponImage.color = Color.white;
					damageIconImage.color = Color.white;
					secondStatIconImage.color = Color.white;
					if (!equipObject.activeSelf)
					{
						equipObject.SetActive(true);
					}
				}
			}
			else
			{
				if (haveObject.activeSelf)
				{
					haveObject.SetActive(false);
				}
				if (equipObject.activeSelf)
				{
					equipObject.SetActive(false);
				}
				if (!noHaveObject.activeSelf)
				{
					noHaveObject.SetActive(true);
				}
				nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
				enchantCountText.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageText.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatText.color = Util.getCalculatedColor(153f, 153f, 153f);
				weaponImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				unlockPriceText.text = GameManager.changeUnit(Singleton<WeaponManager>.instance.getWeaponGoldPriceForUnlock(Singleton<WeaponManager>.instance.getWeaponTier(currentWarriorWeaponType)));
				if (!unlockObject.activeSelf)
				{
					unlockObject.SetActive(true);
				}
			}
			nameText.text = Singleton<WeaponManager>.instance.getWeaponI18NName(currentWeaponOfWarriorData.weapontype);
			maxLevel = Singleton<WeaponManager>.instance.getWeaponUpgradeMaxLevel();
			weaponImage.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(currentWarriorWeaponType);
			weaponIconTransform.localScale = new Vector3(4f, 4f, 4f);
			weaponIconTransform.localEulerAngles = Vector3.zero;
			if (currentWarriorWeaponType == WeaponManager.WarriorWeaponType.WarriorWeapon1)
			{
				Singleton<TutorialManager>.instance.warriorFirstWeaponUpgradeButton = upgradeButton;
			}
			break;
		case CharacterManager.CharacterType.Priest:
			secondStatIconImage.sprite = Singleton<WeaponManager>.instance.priestSecondStatIconSprite;
			currentWeaponOfPriestData = Singleton<WeaponManager>.instance.getWeaponFromInventory(currentPriestWeaponType);
			currentStatDataOfWeapon = Singleton<WeaponManager>.instance.getWeaponStat(currentPriestWeaponType, currentWeaponOfPriestData.enchantCount);
			equippedThisWeapon = Singleton<DataManager>.instance.currentGameData.priestEquippedWeapon == currentWeaponOfPriestData.weapontype;
			isHaving = Singleton<WeaponManager>.instance.containInventory(currentWeaponOfPriestData.weapontype);
			if (currentPriestWeaponType > WeaponManager.PriestWeaponType.PriestWeapon1)
			{
				WeaponManager.PriestWeaponType type3 = currentPriestWeaponType - 1;
				PriestWeaponInventoryData weaponFromInventory3 = Singleton<WeaponManager>.instance.getWeaponFromInventory(type3);
				if (weaponFromInventory3.enchantCount >= Singleton<WeaponManager>.instance.getUnlockNextRequiredLevel())
				{
					if (requiredLevelObject.activeSelf)
					{
						requiredLevelObject.SetActive(false);
					}
					if (!unlockButtonObject.activeSelf)
					{
						unlockButtonObject.SetActive(true);
					}
				}
				else
				{
					requiredLevelProgressBarImage.fillAmount = (float)weaponFromInventory3.enchantCount / (float)Singleton<WeaponManager>.instance.getUnlockNextRequiredLevel();
					if (!requiredLevelObject.activeSelf)
					{
						requiredLevelObject.SetActive(true);
					}
					if (unlockButtonObject.activeSelf)
					{
						unlockButtonObject.SetActive(false);
					}
				}
			}
			else
			{
				if (requiredLevelObject.activeSelf)
				{
					requiredLevelObject.SetActive(false);
				}
				if (!unlockButtonObject.activeSelf)
				{
					unlockButtonObject.SetActive(true);
				}
			}
			if (!currentWeaponOfPriestData.isUnlockedFromSlot)
			{
				if (unlockObject.activeSelf)
				{
					unlockObject.SetActive(false);
				}
				if (noHaveObject.activeSelf)
				{
					noHaveObject.SetActive(false);
				}
				if (haveObject.activeSelf)
				{
					haveObject.SetActive(false);
				}
				if (!slotLockedObject.activeSelf)
				{
					slotLockedObject.SetActive(true);
				}
				if (equipObject.activeSelf)
				{
					equipObject.SetActive(false);
				}
				nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
				enchantCountText.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageText.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatText.color = Util.getCalculatedColor(153f, 153f, 153f);
				weaponImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
			}
			else if (isHaving)
			{
				if (unlockObject.activeSelf)
				{
					unlockObject.SetActive(false);
				}
				if (noHaveObject.activeSelf)
				{
					noHaveObject.SetActive(false);
				}
				if (equippedThisWeapon)
				{
					if (!haveObject.activeSelf)
					{
						haveObject.SetActive(true);
					}
					nameText.color = Util.getCalculatedColor(253f, 252f, 183f);
					enchantCountText.color = Util.getCalculatedColor(250f, 215f, 37f);
					damageText.color = Color.white;
					secondStatText.color = Color.white;
					weaponImage.color = Color.white;
					damageIconImage.color = Color.white;
					secondStatIconImage.color = Color.white;
					if (equipObject.activeSelf)
					{
						equipObject.SetActive(false);
					}
				}
				else
				{
					if (haveObject.activeSelf)
					{
						haveObject.SetActive(false);
					}
					nameText.color = Util.getCalculatedColor(30f, 199f, 255f);
					enchantCountText.color = Util.getCalculatedColor(30f, 199f, 255f);
					damageText.color = Util.getCalculatedColor(30f, 199f, 255f);
					secondStatText.color = Util.getCalculatedColor(30f, 199f, 255f);
					weaponImage.color = Color.white;
					damageIconImage.color = Color.white;
					secondStatIconImage.color = Color.white;
					if (!equipObject.activeSelf)
					{
						equipObject.SetActive(true);
					}
				}
			}
			else
			{
				if (haveObject.activeSelf)
				{
					haveObject.SetActive(false);
				}
				if (!noHaveObject.activeSelf)
				{
					noHaveObject.SetActive(true);
				}
				if (equipObject.activeSelf)
				{
					equipObject.SetActive(false);
				}
				nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
				enchantCountText.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageText.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatText.color = Util.getCalculatedColor(153f, 153f, 153f);
				weaponImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				unlockPriceText.text = GameManager.changeUnit(Singleton<WeaponManager>.instance.getWeaponGoldPriceForUnlock(Singleton<WeaponManager>.instance.getWeaponTier(currentPriestWeaponType)));
				if (!unlockObject.activeSelf)
				{
					unlockObject.SetActive(true);
				}
			}
			nameText.text = Singleton<WeaponManager>.instance.getWeaponI18NName(currentWeaponOfPriestData.weapontype);
			maxLevel = Singleton<WeaponManager>.instance.getWeaponUpgradeMaxLevel();
			weaponImage.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(currentPriestWeaponType);
			weaponIconTransform.localScale = new Vector3(4f, -4f, 4f);
			weaponIconTransform.localEulerAngles = new Vector3(0f, 0f, 90f);
			if (currentPriestWeaponType == WeaponManager.PriestWeaponType.PriestWeapon1)
			{
				Singleton<TutorialManager>.instance.priestFirstWeaponUpgradeButton = upgradeButton;
			}
			break;
		case CharacterManager.CharacterType.Archer:
			secondStatIconImage.sprite = Singleton<WeaponManager>.instance.archerSecondStatIconSprite;
			currentWeaponOfArcherData = Singleton<WeaponManager>.instance.getWeaponFromInventory(currentArcherWeaponType);
			currentStatDataOfWeapon = Singleton<WeaponManager>.instance.getWeaponStat(currentArcherWeaponType, currentWeaponOfArcherData.enchantCount);
			equippedThisWeapon = Singleton<DataManager>.instance.currentGameData.archerEquippedWeapon == currentWeaponOfArcherData.weapontype;
			isHaving = Singleton<WeaponManager>.instance.containInventory(currentWeaponOfArcherData.weapontype);
			if (currentArcherWeaponType > WeaponManager.ArcherWeaponType.ArcherWeapon1)
			{
				WeaponManager.ArcherWeaponType type = currentArcherWeaponType - 1;
				ArcherWeaponInventoryData weaponFromInventory = Singleton<WeaponManager>.instance.getWeaponFromInventory(type);
				if (weaponFromInventory.enchantCount >= Singleton<WeaponManager>.instance.getUnlockNextRequiredLevel())
				{
					if (requiredLevelObject.activeSelf)
					{
						requiredLevelObject.SetActive(false);
					}
					if (!unlockButtonObject.activeSelf)
					{
						unlockButtonObject.SetActive(true);
					}
				}
				else
				{
					requiredLevelProgressBarImage.fillAmount = (float)weaponFromInventory.enchantCount / (float)Singleton<WeaponManager>.instance.getUnlockNextRequiredLevel();
					if (!requiredLevelObject.activeSelf)
					{
						requiredLevelObject.SetActive(true);
					}
					if (unlockButtonObject.activeSelf)
					{
						unlockButtonObject.SetActive(false);
					}
				}
			}
			else
			{
				if (requiredLevelObject.activeSelf)
				{
					requiredLevelObject.SetActive(false);
				}
				if (!unlockButtonObject.activeSelf)
				{
					unlockButtonObject.SetActive(true);
				}
			}
			if (!currentWeaponOfArcherData.isUnlockedFromSlot)
			{
				if (unlockObject.activeSelf)
				{
					unlockObject.SetActive(false);
				}
				if (noHaveObject.activeSelf)
				{
					noHaveObject.SetActive(false);
				}
				if (haveObject.activeSelf)
				{
					haveObject.SetActive(false);
				}
				if (!slotLockedObject.activeSelf)
				{
					slotLockedObject.SetActive(true);
				}
				if (equipObject.activeSelf)
				{
					equipObject.SetActive(false);
				}
				nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
				enchantCountText.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageText.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatText.color = Util.getCalculatedColor(153f, 153f, 153f);
				weaponImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
			}
			else if (isHaving)
			{
				if (unlockObject.activeSelf)
				{
					unlockObject.SetActive(false);
				}
				if (noHaveObject.activeSelf)
				{
					noHaveObject.SetActive(false);
				}
				if (equippedThisWeapon)
				{
					if (!haveObject.activeSelf)
					{
						haveObject.SetActive(true);
					}
					nameText.color = Util.getCalculatedColor(253f, 252f, 183f);
					enchantCountText.color = Util.getCalculatedColor(250f, 215f, 37f);
					damageText.color = Color.white;
					secondStatText.color = Color.white;
					weaponImage.color = Color.white;
					damageIconImage.color = Color.white;
					secondStatIconImage.color = Color.white;
					if (equipObject.activeSelf)
					{
						equipObject.SetActive(false);
					}
				}
				else
				{
					if (haveObject.activeSelf)
					{
						haveObject.SetActive(false);
					}
					nameText.color = Util.getCalculatedColor(30f, 199f, 255f);
					enchantCountText.color = Util.getCalculatedColor(30f, 199f, 255f);
					damageText.color = Util.getCalculatedColor(30f, 199f, 255f);
					secondStatText.color = Util.getCalculatedColor(30f, 199f, 255f);
					weaponImage.color = Color.white;
					damageIconImage.color = Color.white;
					secondStatIconImage.color = Color.white;
					if (!equipObject.activeSelf)
					{
						equipObject.SetActive(true);
					}
				}
			}
			else
			{
				if (haveObject.activeSelf)
				{
					haveObject.SetActive(false);
				}
				if (equipObject.activeSelf)
				{
					equipObject.SetActive(false);
				}
				nameText.color = Util.getCalculatedColor(153f, 153f, 153f);
				enchantCountText.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageText.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatText.color = Util.getCalculatedColor(153f, 153f, 153f);
				weaponImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				damageIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				secondStatIconImage.color = Util.getCalculatedColor(153f, 153f, 153f);
				unlockPriceText.text = GameManager.changeUnit(Singleton<WeaponManager>.instance.getWeaponGoldPriceForUnlock(Singleton<WeaponManager>.instance.getWeaponTier(currentArcherWeaponType)));
				if (!unlockObject.activeSelf)
				{
					unlockObject.SetActive(true);
				}
				if (!noHaveObject.activeSelf)
				{
					noHaveObject.SetActive(true);
				}
			}
			nameText.text = Singleton<WeaponManager>.instance.getWeaponI18NName(currentWeaponOfArcherData.weapontype);
			maxLevel = Singleton<WeaponManager>.instance.getWeaponUpgradeMaxLevel();
			weaponImage.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(currentArcherWeaponType);
			weaponIconTransform.localScale = new Vector3(4f, 4f, 4f);
			weaponIconTransform.localEulerAngles = Vector3.zero;
			if (currentArcherWeaponType == WeaponManager.ArcherWeaponType.ArcherWeapon1)
			{
				Singleton<TutorialManager>.instance.archerFirstWeaponUpgradeButton = upgradeButton;
			}
			break;
		}
		weaponImage.SetNativeSize();
		firstStatType = StatManager.WeaponStatType.FixedDamage;
		secondStatType = currentStatDataOfWeapon.secondStatType;
		if (currentLevel < maxLevel)
		{
			double num = 0.0;
			double num2 = 0.0;
			switch (weaponCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				currentLevel = currentWeaponOfWarriorData.enchantCount;
				num = currentStatDataOfWeapon.weaponDamage;
				num2 = currentStatDataOfWeapon.secondStatValue;
				upgradePrice = Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(currentWarriorWeaponType), currentLevel);
				break;
			case CharacterManager.CharacterType.Priest:
				currentLevel = currentWeaponOfPriestData.enchantCount;
				num = currentStatDataOfWeapon.weaponDamage;
				num2 = currentStatDataOfWeapon.secondStatValue;
				upgradePrice = Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(currentPriestWeaponType), currentLevel);
				break;
			case CharacterManager.CharacterType.Archer:
				currentLevel = currentWeaponOfArcherData.enchantCount;
				num = currentStatDataOfWeapon.weaponDamage;
				num2 = currentStatDataOfWeapon.secondStatValue;
				upgradePrice = Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(currentArcherWeaponType), currentLevel);
				break;
			}
			if (isHaving)
			{
				enchantCountText.text = "Lv." + (currentLevel + 1);
			}
			else
			{
				enchantCountText.text = string.Empty;
			}
			upgradePriceText.text = GameManager.changeUnit(upgradePrice);
			if (!upgradeButtonObject.activeSelf)
			{
				upgradeButtonObject.SetActive(true);
			}
		}
		else if (upgradeButtonObject.activeSelf)
		{
			upgradeButtonObject.SetActive(false);
		}
		damageText.text = GameManager.changeUnit(currentStatDataOfWeapon.weaponDamage);
		if (secondStatType == StatManager.WeaponStatType.FixedHealth)
		{
			secondStatText.text = string.Format(I18NManager.Get("WARRIOR_SECOND_STAT_SIMPLE_DESCRIPTION"), GameManager.changeUnit(currentStatDataOfWeapon.secondStatValue));
		}
		else if (secondStatType == StatManager.WeaponStatType.TapHeal)
		{
			secondStatText.text = string.Format(I18NManager.Get("PRIEST_SECOND_STAT_SIMPLE_DESCRIPTION"), GameManager.changeUnit(currentStatDataOfWeapon.secondStatValue, false));
		}
		else if (secondStatType == StatManager.WeaponStatType.CriticalChance)
		{
			secondStatText.text = string.Format(I18NManager.Get("ARCHER_SECOND_STAT_SIMPLE_DESCRIPTION"), GameManager.changeUnit(Math.Min(100.0, currentStatDataOfWeapon.secondStatValue), false));
		}
		if (!isHaving)
		{
			secondStatText.text = secondStatText.text.Replace("<color=#1EC7FF>", string.Empty).Replace("</color>", string.Empty);
		}
		refreshBuyState();
		secondStatIconImage.SetNativeSize();
		refreshEquippedState();
	}

	public void refreshEquippedState()
	{
		if (isHaving)
		{
			switch (weaponCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				if (Singleton<DataManager>.instance.currentGameData.warriorEquippedWeapon == currentWarriorWeaponType)
				{
					if (!equippedIcon.activeSelf)
					{
						equippedIcon.SetActive(true);
					}
					innerBackground.sprite = Singleton<CachedManager>.instance.enableThumbnailWeaponEquippedSprite;
				}
				else
				{
					if (equippedIcon.activeSelf)
					{
						equippedIcon.SetActive(false);
					}
					innerBackground.sprite = Singleton<CachedManager>.instance.enableThumbnailWeaponSprite;
				}
				break;
			case CharacterManager.CharacterType.Priest:
				if (Singleton<DataManager>.instance.currentGameData.priestEquippedWeapon == currentPriestWeaponType)
				{
					if (!equippedIcon.activeSelf)
					{
						equippedIcon.SetActive(true);
					}
					innerBackground.sprite = Singleton<CachedManager>.instance.enableThumbnailWeaponEquippedSprite;
				}
				else
				{
					if (equippedIcon.activeSelf)
					{
						equippedIcon.SetActive(false);
					}
					innerBackground.sprite = Singleton<CachedManager>.instance.enableThumbnailWeaponSprite;
				}
				break;
			case CharacterManager.CharacterType.Archer:
				if (Singleton<DataManager>.instance.currentGameData.archerEquippedWeapon == currentArcherWeaponType)
				{
					if (!equippedIcon.activeSelf)
					{
						equippedIcon.SetActive(true);
					}
					innerBackground.sprite = Singleton<CachedManager>.instance.enableThumbnailWeaponEquippedSprite;
				}
				else
				{
					if (equippedIcon.activeSelf)
					{
						equippedIcon.SetActive(false);
					}
					innerBackground.sprite = Singleton<CachedManager>.instance.enableThumbnailWeaponSprite;
				}
				break;
			}
		}
		else
		{
			innerBackground.sprite = Singleton<CachedManager>.instance.disableThumbnailSprite;
			if (equippedIcon.activeSelf)
			{
				equippedIcon.SetActive(false);
			}
		}
	}

	public void upgradeWeapon()
	{
		upgradeThisWeapon(upgradePrice, 1L);
	}

	private void upgradeThisWeapon(double targetUpgradePrice, long increaseCount)
	{
		if (Singleton<DataManager>.instance.currentGameData.gold >= targetUpgradePrice)
		{
			m_quickUpgradeDisappearTimer = 0f;
			Singleton<WeaponManager>.instance.upgradeWeapon(this, weaponCharacterType, increaseCount);
			UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
			Singleton<GoldManager>.instance.decreaseGold(targetUpgradePrice);
			refreshSlot();
			checkQuickUpgrade();
			WeaponSlot weaponSlot = UIWindowManageHeroAndWeapon.instance.weaponInfiniteScroll.getNextSlotItem(slotIndex) as WeaponSlot;
			if (weaponSlot != null)
			{
				weaponSlot.refreshSlot();
			}
			upgradeEffect.Stop();
			upgradeEffect.Play();
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
			Singleton<QuestManager>.instance.questReport(QuestManager.QuestType.Enchant, increaseCount);
			checkQuickUpgrade();
		}
		else
		{
			Singleton<AudioManager>.instance.playEffectSound("btn_fail");
			UIWindowDialog.openDescription("NOT_ENOUGH_GOLD", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.NormalItem);
				UIWindowPopupShop.instance.focusToGold();
			}, string.Empty);
		}
	}

	public void unlockWeapon()
	{
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			if (!Singleton<WeaponManager>.instance.buyWeapon(currentWarriorWeaponType))
			{
				unlockButtonCantBuyAnimation.Play();
				break;
			}
			Singleton<AudioManager>.instance.playEffectSound("unlock_weapon");
			unlockEffect.Stop();
			unlockEffect.Play();
			equipWeapon(false);
			break;
		case CharacterManager.CharacterType.Priest:
			if (!Singleton<WeaponManager>.instance.buyWeapon(currentPriestWeaponType))
			{
				unlockButtonCantBuyAnimation.Play();
				break;
			}
			Singleton<AudioManager>.instance.playEffectSound("unlock_weapon");
			unlockEffect.Stop();
			unlockEffect.Play();
			equipWeapon(false);
			break;
		case CharacterManager.CharacterType.Archer:
			if (!Singleton<WeaponManager>.instance.buyWeapon(currentArcherWeaponType))
			{
				unlockButtonCantBuyAnimation.Play();
				break;
			}
			Singleton<AudioManager>.instance.playEffectSound("unlock_weapon");
			unlockEffect.Stop();
			unlockEffect.Play();
			equipWeapon(false);
			break;
		}
	}

	public void equipWeapon(bool withEffectSound = true)
	{
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			Singleton<WeaponManager>.instance.equipWeapon(currentWarriorWeaponType);
			break;
		case CharacterManager.CharacterType.Priest:
			Singleton<WeaponManager>.instance.equipWeapon(currentPriestWeaponType);
			break;
		case CharacterManager.CharacterType.Archer:
			Singleton<WeaponManager>.instance.equipWeapon(currentArcherWeaponType);
			break;
		}
		weaponInfiniteScroll.refreshAll();
		if (withEffectSound)
		{
			Singleton<AudioManager>.instance.playEffectSound("weapon_equip");
		}
		UIWindowManageHeroAndWeapon.instance.refreshCharacterInformation();
	}

	public double getCalculatedQuickUpgradePrice()
	{
		double num = 0.0;
		long num2 = 0L;
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			num2 = currentWeaponOfWarriorData.enchantCount;
			for (long num4 = num2; num4 < WeaponManager.quickUpgradeTargetEnchantCount - 1; num4++)
			{
				num += Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(currentWarriorWeaponType), num4);
			}
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			num2 = currentWeaponOfPriestData.enchantCount;
			for (long num5 = num2; num5 < WeaponManager.quickUpgradeTargetEnchantCount - 1; num5++)
			{
				num += Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(currentPriestWeaponType), num5);
			}
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			num2 = currentWeaponOfArcherData.enchantCount;
			for (long num3 = num2; num3 < WeaponManager.quickUpgradeTargetEnchantCount - 1; num3++)
			{
				num += Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(currentArcherWeaponType), num3);
			}
			break;
		}
		}
		return num;
	}

	public void OnClickQuickUpgrade()
	{
		long num = 0L;
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			num = currentWeaponOfWarriorData.enchantCount;
			break;
		case CharacterManager.CharacterType.Priest:
			num = currentWeaponOfPriestData.enchantCount;
			break;
		case CharacterManager.CharacterType.Archer:
			num = currentWeaponOfArcherData.enchantCount;
			break;
		}
		if (num < WeaponManager.quickUpgradeTargetEnchantCount - 1)
		{
			StopCoroutine("waitForCloseAnimation");
			upgradeThisWeapon(getCalculatedQuickUpgradePrice(), WeaponManager.quickUpgradeTargetEnchantCount - 1 - num);
			closeQuickUpgradeButton();
		}
		else
		{
			closeQuickUpgradeButton();
		}
	}

	private void checkQuickUpgrade()
	{
		long num = 0L;
		switch (weaponCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
			num = currentWeaponOfWarriorData.enchantCount;
			break;
		case CharacterManager.CharacterType.Priest:
			num = currentWeaponOfPriestData.enchantCount;
			break;
		case CharacterManager.CharacterType.Archer:
			num = currentWeaponOfArcherData.enchantCount;
			break;
		}
		if (TutorialManager.isTutorial || TutorialManager.isRebirthTutorial || !base.cachedGameObject.activeSelf || !base.cachedGameObject.activeInHierarchy || num >= WeaponManager.quickUpgradeTargetEnchantCount - 1)
		{
			closeQuickUpgradeButton();
		}
		else if (Singleton<DataManager>.instance.currentGameData.gold >= getCalculatedQuickUpgradePrice())
		{
			m_quickUpgradeDisappearTimer = 0f;
			openQuickUpgradeButton();
			StopCoroutine("waitForCloseAnimation");
			StopCoroutine("quickUpgradeButtonDisappearUpdate");
			StartCoroutine("quickUpgradeButtonDisappearUpdate");
		}
		else
		{
			closeQuickUpgradeButton();
		}
	}

	private void openQuickUpgradeButton()
	{
		quickUpgradeTotalPriceText.text = GameManager.changeUnit(getCalculatedQuickUpgradePrice());
		if (!isQuickUpgrading)
		{
			quickUpgradeButtonObject.SetActive(false);
			quickUpgradeButtonObject.SetActive(true);
			isQuickUpgrading = true;
		}
	}

	private void closeQuickUpgradeButton(bool forceClose = false)
	{
		StopCoroutine("quickUpgradeButtonDisappearUpdate");
		m_quickUpgradeDisappearTimer = 0f;
		if (quickUpgradeButtonObject.activeSelf)
		{
			if (!forceClose)
			{
				if (!quickUpgradeButtonAnimation.IsPlaying("WeaponQuickUpgradeButtonCloseAnimation"))
				{
					quickUpgradeButtonAnimation.Stop();
					quickUpgradeButtonAnimation.Play("WeaponQuickUpgradeButtonCloseAnimation");
					StopCoroutine("waitForCloseAnimation");
					StartCoroutine("waitForCloseAnimation");
				}
			}
			else
			{
				quickUpgradeButtonObject.SetActive(false);
			}
		}
		isQuickUpgrading = false;
	}

	private IEnumerator waitForCloseAnimation()
	{
		yield return new WaitWhile(() => quickUpgradeButtonAnimation.isPlaying);
		quickUpgradeButtonObject.SetActive(false);
	}

	private IEnumerator quickUpgradeButtonDisappearUpdate()
	{
		while (m_quickUpgradeDisappearTimer < 1.5f)
		{
			m_quickUpgradeDisappearTimer += Time.deltaTime;
			yield return null;
		}
		closeQuickUpgradeButton();
	}
}
