using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowElopeMode : UIWindow
{
	public static UIWindowElopeMode instance;

	public static Vector2 miniPopupStartPosition = new Vector2(-63.8f, -146.2f);

	public static float intervalBetweenMiniPopup = 116.5999f;

	public ElopeModeHPProgress enemyHPProgressObject;

	public List<ElopeSkillMiniPopupObject> totalElopeSkillMiniPopupObjectList;

	public List<ElopeSkillMiniPopupObject> currentUsingElopeSkillMiniPopupObjectList = new List<ElopeSkillMiniPopupObject>();

	public GameObject elopePrincessIndicator;

	public GameObject elopePrincessScrollObject;

	public Text elopePrincessTabText;

	public Image elopePrincessTabBackground;

	public GameObject elopeDaemonKingIndicator;

	public GameObject elopeDaemonKingScrollObject;

	public Text elopeDaemonKingTabText;

	public Image elopeDaemonKingTabBackground;

	public InfiniteScroll elopePrincessScrollRect;

	public InfiniteScroll elopeDaemonKingScrollRect;

	public Text distanceText;

	public Image distanceProgressBarImage;

	public RectTransform distanceProgressIconTransform;

	public GameObject fillHeartEffectObject;

	public GameObject daemonKingSkillCastObject;

	public GameObject daemonKingHandsomeCastObject;

	public GameObject daemonKingSpeedCastObject;

	public Animation daemonKingSkillCastAnimation;

	public GameObject speedGuyEffectObject;

	public GameObject superSpeedGuyEffectObject;

	public GameObject daemonKingSkillBackgroundObject;

	public Text castSkillEffectSkillNameText;

	private Dictionary<ElopeModeManager.DaemonKingSkillType, ElopeSkillMiniPopupObject> m_currentElopSkillMiniPopupObjectDictionary;

	public override void Awake()
	{
		instance = this;
		m_currentElopSkillMiniPopupObjectDictionary = new Dictionary<ElopeModeManager.DaemonKingSkillType, ElopeSkillMiniPopupObject>();
		for (int i = 0; i < totalElopeSkillMiniPopupObjectList.Count; i++)
		{
			m_currentElopSkillMiniPopupObjectDictionary.Add(totalElopeSkillMiniPopupObjectList[i].currentSkillType, totalElopeSkillMiniPopupObjectList[i]);
		}
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		daemonKingSkillCastObject.SetActive(false);
		enemyHPProgressObject.closeEnemyHPProgress(true);
		refreshDistanceStatus();
		if (elopePrincessIndicator.activeSelf)
		{
			elopePrincessIndicator.SetActive(false);
		}
		if (elopeDaemonKingIndicator.activeSelf)
		{
			elopeDaemonKingIndicator.SetActive(false);
		}
		elopeDaemonKingScrollRect.refreshMaxCount(8);
		elopeDaemonKingScrollRect.refreshAll();
		elopePrincessScrollRect.refreshMaxCount(GameManager.maxPrincessCount);
		elopePrincessScrollRect.refreshAll();
		OnClickTab(0);
		return base.OnBeforeOpen();
	}

	public override void OnAfterOpen()
	{
		if (!Singleton<DataManager>.instance.currentGameData.isSeenElopeModeTutorial)
		{
			UIWindow.Get("UI ElopeModeTutorialDialog").open();
			Singleton<DataManager>.instance.currentGameData.isSeenElopeModeTutorial = true;
		}
		base.OnAfterOpen();
	}

	public void openDaemonKingSkillCastEffect(ElopeModeManager.DaemonKingSkillType skillType)
	{
		daemonKingSkillCastObject.SetActive(false);
		daemonKingSkillCastObject.SetActive(true);
		castSkillEffectSkillNameText.text = I18NManager.Get("ELOPE_DAEMON_KING_SLOT_TITLE_" + (int)(skillType + 1));
		switch (skillType)
		{
		case ElopeModeManager.DaemonKingSkillType.HandsomeGuyDaemonKing:
		case ElopeModeManager.DaemonKingSkillType.SuperHandsomeGuyDaemonKing:
			Singleton<AudioManager>.instance.playEffectSound("handsome_guy_scene");
			daemonKingHandsomeCastObject.SetActive(true);
			daemonKingSpeedCastObject.SetActive(false);
			break;
		case ElopeModeManager.DaemonKingSkillType.SpeedGuyDaemonKing:
		case ElopeModeManager.DaemonKingSkillType.SuperSpeedGuyDaemonKing:
			Singleton<AudioManager>.instance.playEffectSound("speed_guy_scene");
			daemonKingHandsomeCastObject.SetActive(false);
			daemonKingSpeedCastObject.SetActive(true);
			break;
		}
		StopCoroutine(waitForCloseEffect());
		StartCoroutine(waitForCloseEffect());
	}

	private IEnumerator waitForCloseEffect()
	{
		while (daemonKingSkillCastAnimation.isPlaying)
		{
			yield return null;
		}
		daemonKingSkillCastObject.SetActive(false);
	}

	public void refreshDistanceStatus()
	{
		long currentProgressDistanceForElopeMode = Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode;
		distanceText.text = currentProgressDistanceForElopeMode.ToString("N0") + "m";
		long num = (currentProgressDistanceForElopeMode / 100 + 1) * 100;
		float num2 = (float)(100 - (num - currentProgressDistanceForElopeMode)) / 100f;
		distanceProgressBarImage.fillAmount = num2;
		distanceProgressIconTransform.anchoredPosition = new Vector2(202f * num2, 0f);
	}

	public void OnClickTab(int type)
	{
		switch (type)
		{
		case 0:
			elopePrincessScrollObject.SetActive(true);
			elopeDaemonKingScrollObject.SetActive(false);
			elopePrincessTabText.color = Color.white;
			elopeDaemonKingTabText.color = Util.getCalculatedColor(139f, 44f, 88f);
			elopePrincessTabBackground.sprite = Singleton<CachedManager>.instance.elopeModeSelectedSprite;
			elopeDaemonKingTabBackground.sprite = Singleton<CachedManager>.instance.elopeModeNonSelectedSprite;
			elopePrincessScrollRect.refreshAll();
			break;
		case 1:
			elopePrincessScrollObject.SetActive(false);
			elopeDaemonKingScrollObject.SetActive(true);
			elopePrincessTabText.color = Util.getCalculatedColor(139f, 44f, 88f);
			elopeDaemonKingTabText.color = Color.white;
			elopePrincessTabBackground.sprite = Singleton<CachedManager>.instance.elopeModeNonSelectedSprite;
			elopeDaemonKingTabBackground.sprite = Singleton<CachedManager>.instance.elopeModeSelectedSprite;
			elopeDaemonKingScrollRect.refreshAll();
			break;
		}
	}

	public void playHeartEffect(int princessIndex)
	{
		ScrollSlotItem slotItem = elopePrincessScrollRect.getSlotItem(princessIndex - 1);
		if (slotItem != null)
		{
			(slotItem as ElopePrincessSlot).startHeartFullEffect();
		}
	}

	public void openSkillPopup(ElopeModeManager.DaemonKingSkillType skillType)
	{
		ElopeSkillMiniPopupObject elopeSkillMiniPopupObject = m_currentElopSkillMiniPopupObjectDictionary[skillType];
		if (!currentUsingElopeSkillMiniPopupObjectList.Contains(elopeSkillMiniPopupObject))
		{
			currentUsingElopeSkillMiniPopupObjectList.Add(elopeSkillMiniPopupObject);
		}
		refreshMiniPopupIndex();
		elopeSkillMiniPopupObject.openMiniPopup();
	}

	public void closeSkillPopup(ElopeModeManager.DaemonKingSkillType skillType)
	{
		ElopeSkillMiniPopupObject elopeSkillMiniPopupObject = m_currentElopSkillMiniPopupObjectDictionary[skillType];
		if (currentUsingElopeSkillMiniPopupObjectList.Contains(elopeSkillMiniPopupObject))
		{
			currentUsingElopeSkillMiniPopupObjectList.Remove(elopeSkillMiniPopupObject);
		}
		refreshMiniPopupIndex();
		elopeSkillMiniPopupObject.closeMiniPopup();
	}

	public void forceCloseAllMiniPopUp()
	{
		for (int i = 0; i < totalElopeSkillMiniPopupObjectList.Count; i++)
		{
			totalElopeSkillMiniPopupObjectList[i].closeMiniPopup(true);
		}
		currentUsingElopeSkillMiniPopupObjectList.Clear();
	}

	public ElopeSkillMiniPopupObject getElopeSkillMiniPopupObject(ElopeModeManager.DaemonKingSkillType skillType)
	{
		return m_currentElopSkillMiniPopupObjectDictionary[skillType];
	}

	private void refreshMiniPopupIndex()
	{
		Vector2 targetPosition = miniPopupStartPosition;
		for (int i = 0; i < currentUsingElopeSkillMiniPopupObjectList.Count; i++)
		{
			currentUsingElopeSkillMiniPopupObjectList[i].targetPosition = targetPosition;
			targetPosition.y -= intervalBetweenMiniPopup;
		}
	}

	public void OnClickAddResources()
	{
		UIWindowBuyElopeResources.instance.openBuyElopeResources(delegate
		{
			Singleton<PaymentManager>.instance.Purchase(ShopManager.ElopeModeItemType.ElopeResources, delegate
			{
				Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource1, 100L);
				Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource2, 100L);
				Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource3, 100L);
				Singleton<ElopeModeManager>.instance.increaseResource(DropItemManager.DropItemType.ElopeResource4, 100L);
			});
		});
	}

	public void OnClickOpenElopeModeTutorialDialog()
	{
		UIWindow.Get("UI ElopeModeTutorialDialog").open();
	}
}
