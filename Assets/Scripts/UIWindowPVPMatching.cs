using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowPVPMatching : UIWindow
{
	public static UIWindowPVPMatching instance;

	public CharacterUIObject myCharacterUIObject;

	public CharacterUIObject enemyCharacterUIObject;

	public Image loadingCharacterHeadImage;

	public Text researchPriceText;

	public Text myNickNameText;

	public Text myTierText;

	public Image myTierImage;

	public Text myRecordText;

	public Text myDamageText;

	public Text myTotalSkinCountText;

	public Text enemyNickNameText;

	public Text enemyTierText;

	public Image enemyTierImage;

	public Text enemyRecordText;

	public Text enemyDamageText;

	public Text enemyTotalSkinCountText;

	public GameObject matchingObject;

	public GameObject enemyObject;

	private bool m_isMatchingSuccess;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openPVPMatching()
	{
		Singleton<PVPManager>.instance.enemyUnitData = null;
		researchPriceText.text = PVPManager.reserchEnemyPrice.ToString("N0");
		myNickNameText.text = Singleton<PVPManager>.instance.myPVPData.player.nickname;
		myTierText.text = Singleton<PVPManager>.instance.getTierName(Singleton<PVPManager>.instance.myPVPData.player.grade);
		myTierImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(Singleton<PVPManager>.instance.myPVPData.player.grade);
		myTierImage.SetNativeSize();
		myRecordText.text = string.Format(I18NManager.Get("PVP_MMR"), Singleton<PVPManager>.instance.myPVPData.player.point) + " / <color=#FDFCB7FF>" + string.Format(I18NManager.Get("RANK"), (Singleton<PVPManager>.instance.myPVPData.player.rank >= 0) ? Singleton<PVPManager>.instance.myPVPData.player.rank.ToString("N0") : "--") + "</color>";
		PVPManager.PVPGameData gameData = Singleton<PVPManager>.instance.convertStringToPVPGameData(Singleton<PVPManager>.instance.myPVPData.player.game_data);
		myDamageText.text = GameManager.changeUnit(Singleton<PVPManager>.instance.getTotalDamage(gameData));
		myTotalSkinCountText.text = Singleton<PVPManager>.instance.getTotalSkinCount(gameData).ToString("N0");
		myCharacterUIObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin, null, "PopUpLayer", 120);
		myCharacterUIObject.cachedSpriteGroup.setAlpha(1f);
		StopAllCoroutines();
		m_isMatchingSuccess = false;
		matchingObject.SetActive(true);
		enemyObject.SetActive(false);
		open();
		startMatching();
		StartCoroutine("matchingFakeUpdate");
	}

	private void startMatching()
	{
		Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.START_MATCH, delegate(bool isSuccess)
		{
			if (isSuccess)
			{
				if (!string.IsNullOrEmpty(Singleton<PVPManager>.instance.enemyUnitData.game_data))
				{
					PVPManager.PVPGameData pVPGameData = Singleton<PVPManager>.instance.convertStringToPVPGameData(Singleton<PVPManager>.instance.enemyUnitData.game_data);
					if (pVPGameData != null)
					{
						string[] array = ((pVPGameData.equippedCharacterData == null) ? null : pVPGameData.equippedCharacterData.Split(','));
						if (array != null && array.Length == 3)
						{
							CharacterSkinManager.WarriorSkinType warriorSkinType = (CharacterSkinManager.WarriorSkinType)int.Parse(array[0]);
							if (warriorSkinType < CharacterSkinManager.WarriorSkinType.Length)
							{
								enemyCharacterUIObject.initCharacterUIObject(warriorSkinType, null, "PopUpLayer", 120);
							}
							else
							{
								enemyCharacterUIObject.initCharacterUIObject(CharacterSkinManager.WarriorSkinType.William, null, "PopUpLayer", 120);
							}
						}
						else
						{
							enemyCharacterUIObject.initCharacterUIObject(CharacterSkinManager.WarriorSkinType.William, null, "PopUpLayer", 120);
						}
						enemyCharacterUIObject.cachedSpriteGroup.setAlpha(1f);
						enemyNickNameText.text = Singleton<PVPManager>.instance.enemyUnitData.nickname;
						enemyTierText.text = Singleton<PVPManager>.instance.getTierName(Singleton<PVPManager>.instance.enemyUnitData.grade);
						enemyTierImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(Singleton<PVPManager>.instance.enemyUnitData.grade);
						enemyTierImage.SetNativeSize();
						enemyRecordText.text = string.Format(I18NManager.Get("PVP_MMR"), Singleton<PVPManager>.instance.enemyUnitData.point) + " / <color=#FDFCB7FF>" + string.Format(I18NManager.Get("RANK"), (Singleton<PVPManager>.instance.enemyUnitData.rank >= 0) ? Singleton<PVPManager>.instance.enemyUnitData.rank.ToString("N0") : "--") + "</color>";
						enemyDamageText.text = GameManager.changeUnit(Singleton<PVPManager>.instance.getTotalDamage(pVPGameData));
						enemyTotalSkinCountText.text = Singleton<PVPManager>.instance.getTotalSkinCount(pVPGameData).ToString("N0");
						m_isMatchingSuccess = true;
					}
					else
					{
						startMatching();
					}
				}
				else
				{
					startMatching();
				}
			}
			else
			{
				UIWindowDialog.openDescription("ERROR_FOR_TOWER_MODE", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
				Singleton<PVPManager>.instance.increaseTicket(1, false);
				UIWindowPVPMainUI.instance.refreshAddTicketButtonObject();
				close();
			}
		}, false);
	}

	private IEnumerator matchingFakeUpdate()
	{
		float randomWaitTime = Random.Range(0.4f, 2f);
		float timerForEndCoroutine = 0f;
		float timerForRandomHead = 0f;
		while (timerForEndCoroutine < randomWaitTime)
		{
			timerForRandomHead += Time.deltaTime * GameManager.timeScale;
			if (timerForRandomHead >= 0.05f)
			{
				timerForRandomHead = 0f;
				Sprite headSprite = null;
				switch (Random.Range(0, 3))
				{
				case 0:
					headSprite = Singleton<CharacterManager>.instance.getWarriorHeadSprite((CharacterSkinManager.WarriorSkinType)Random.Range(0, 30));
					break;
				case 1:
					headSprite = Singleton<CharacterManager>.instance.getPriestHeadSprite((CharacterSkinManager.PriestSkinType)Random.Range(0, 30));
					break;
				case 2:
					headSprite = Singleton<CharacterManager>.instance.getArcherHeadSprite((CharacterSkinManager.ArcherSkinType)Random.Range(0, 30));
					break;
				}
				loadingCharacterHeadImage.sprite = headSprite;
				loadingCharacterHeadImage.SetNativeSize();
			}
			if (m_isMatchingSuccess)
			{
				timerForEndCoroutine += Time.deltaTime * GameManager.timeScale;
			}
			yield return null;
		}
		matchingSuccess();
	}

	private void matchingSuccess()
	{
		matchingObject.SetActive(false);
		enemyObject.SetActive(true);
	}

	public void OnClickStartPVP()
	{
		if (m_isMatchingSuccess)
		{
			Singleton<PVPManager>.instance.startPVP(Singleton<PVPManager>.instance.convertStringToPVPGameData(Singleton<PVPManager>.instance.enemyUnitData.game_data));
		}
	}

	public void OnClickResearchEnemy()
	{
		if (Singleton<DataManager>.instance.currentGameData._ruby >= PVPManager.reserchEnemyPrice)
		{
			Singleton<RubyManager>.instance.decreaseRuby(PVPManager.reserchEnemyPrice);
			openPVPMatching();
		}
		else
		{
			UIWindowDialog.openDescription("NOT_ENOUGH_RUBY", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowPopupShop.instance.openShopPopupWithType(ShopManager.ShopSelectedType.Ruby);
			}, string.Empty);
		}
	}
}
