using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : Singleton<TutorialManager>
{
	public enum TutorialType
	{
		NotProgressing,
		TutorialType1,
		TutorialType2,
		TutorialType3,
		TutorialType4,
		TutorialType5,
		TutorialType6,
		TutorialType7,
		TutorialType8,
		TutorialType9,
		TutorialType10,
		TutorialType11,
		TutorialType12,
		TutorialType13,
		TutorialType14,
		TutorialType15,
		TutorialType16,
		TutorialType17,
		TutorialType18,
		TutorialType19,
		TutorialType20,
		TutorialType21,
		TutorialType22,
		TutorialType23,
		TutorialType24,
		TutorialType25,
		TutorialType26,
		TutorialType27,
		TutorialType28,
		TutorialType29,
		TutorialType30,
		TutorialType31,
		TutorialType32,
		TutorialType33,
		TutorialType34,
		TutorialType35,
		TutorialType36,
		TutorialType37,
		TutorialType38,
		TutorialType39,
		TutorialType40,
		TutorialType41,
		TutorialType42,
		TutorialType43,
		TutorialType44,
		TutorialType45,
		TutorialType46,
		TutorialType47,
		TutorialType48,
		TutorialType49,
		TutorialType50,
		TutorialType51,
		RebirthTutorialType1,
		RebirthTutorialType2,
		RebirthTutorialType3,
		RebirthTutorialType4,
		RebirthTutorialType5,
		RebirthTutorialType6,
		RebirthTutorialType7,
		RebirthTutorialType8,
		RebirthTutorialType9,
		RebirthTutorialType10,
		RebirthTutorialType11,
		RebirthTutorialType12,
		RebirthTutorialType13,
		KidnappedType1,
		DieWhenPlayingTutorial
	}

	public enum ArrowDirectionType
	{
		Top,
		Bottom,
		Left,
		Right
	}

	[Serializable]
	public struct ButtonEnableStateData
	{
		public Button targetButton;

		public ButtonEnableStateData(Button targetButton)
		{
			this.targetButton = targetButton;
		}
	}

	public enum TutorialPopupPosition
	{
		Top,
		Center,
		Bottom
	}

	public static bool isTutorial;

	public static bool isMainTutorial;

	public static bool isRebirthTutorial;

	public TutorialType currentTutorial;

	public GameObject tutorialSkipButtonObject;

	public GameObject blockObject;

	public Image focusImage;

	public Sprite rectSprite;

	public Sprite circleSprite;

	public GameObject allCoverBlockWithoutPopup;

	public RectTransform arrowParentRectTransform;

	public RectTransform focusObjectRectTransform;

	public List<ButtonEnableStateData> allButtonData;

	public GameObject slideToEatGoldObject;

	public Animation tutorialPopUpAnimation;

	public GameObject tutorialPopUpRectTransform;

	public TypingText tutorialText;

	public Button closePopupButton;

	public Button startGameButton;

	public Button cloudLoadButton;

	public Button introSkipButton;

	public Button tutorialSkipButton;

	public Button[] dialogOKButton;

	public Button[] dialogCancelButton;

	public Button[] dialogDelegateButton;

	public Button[] dialogCloseButton;

	public Button quickStartButton;

	public Button divineSmashButton;

	public Button gotoTownButton;

	public Button priestTapButton;

	public Button archerTapButton;

	public Button colleagueTapButton;

	public Button treasureTapButton;

	public Button rebirthButton;

	public Button colleagueFirstUnlockButton;

	public Button warriorFirstWeaponUpgradeButton;

	public Button priestFirstWeaponUpgradeButton;

	public Button archerFirstWeaponUpgradeButton;

	public RectTransform dialogRectTransform;

	public GameObject urgeTouchEffectObject;

	public GameObject closeBlock;

	public RectTransform questButtonRectTransform;

	public RectTransform achievementButtonRectTransform;

	public bool isInvincible;

	public Transform tutorialGoldEffectStartTransform;

	public GameObject tutorialRewardUIObject;

	private bool m_isClickCloseButton;

	private bool m_isClickCloseButtonForDie;

	private bool m_isCanGoNext;

	private int m_tutorialI18NIndex = 1;

	private ScrollRect[] m_allScrollRects;

	private bool m_onceClose;

	private bool m_isCanClose;

	private TutorialType m_prevTutorialType;

	public RectTransform tutorialPopupBackgroundRectTransform;

	public GameObject kidnappedUIObject;

	public GameObject kidnappedEffectObject;

	public Animation kidnappedUIAnimation;

	public bool isEndAngelAnimation;

	public GameObject kidnappedAngelObject;

	public GameObject kidnappedExclamtionObject;

	public SpriteAnimation[] princessAnimations;

	private string m_targetTutorialDescriptionContent = string.Empty;

	public void dieWhenPlayingTutorial()
	{
		disableFocus();
		if (currentTutorial != TutorialType.DieWhenPlayingTutorial)
		{
			m_prevTutorialType = currentTutorial;
		}
		currentTutorial = TutorialType.DieWhenPlayingTutorial;
		StopCoroutine("dieWaitUpdate");
		StartCoroutine("dieWaitUpdate");
	}

	private IEnumerator dieWaitUpdate()
	{
		openNextScriptForDie(1);
		yield return new WaitUntil(() => m_isClickCloseButtonForDie);
		m_isClickCloseButtonForDie = false;
		openNextScriptForDie(2);
		yield return new WaitUntil(() => m_isClickCloseButtonForDie);
		m_isClickCloseButtonForDie = false;
		openNextScriptForDie(3);
		yield return new WaitUntil(() => m_isClickCloseButtonForDie);
		m_isClickCloseButtonForDie = false;
		blockObject.SetActive(false);
		closeBlock.SetActive(false);
		Singleton<StatManager>.instance.revivePercentHealth = 100.0;
		Singleton<CharacterManager>.instance.warriorCharacter.revive();
		currentTutorial = m_prevTutorialType;
		Singleton<AudioManager>.instance.playBackgroundSound("monster");
	}

	public void startTutorial()
	{
		tutorialText.withoutPrefix = true;
		m_allScrollRects = Resources.FindObjectsOfTypeAll<ScrollRect>();
		Button[] array = Resources.FindObjectsOfTypeAll<Button>();
		allButtonData = new List<ButtonEnableStateData>();
		isInvincible = false;
		isTutorial = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != closePopupButton && array[i] != startGameButton && array[i] != introSkipButton && array[i] != dialogOKButton[0] && array[i] != dialogOKButton[1] && array[i] != dialogOKButton[2] && array[i] != dialogCancelButton[0] && array[i] != dialogCancelButton[1] && array[i] != dialogCancelButton[2] && array[i] != dialogCloseButton[0] && array[i] != dialogCloseButton[1] && array[i] != dialogDelegateButton[0] && array[i] != dialogDelegateButton[1] && array[i] != dialogDelegateButton[2] && array[i] != tutorialSkipButton && array[i].name != "SkipButton" && array[i] != cloudLoadButton)
			{
				allButtonData.Add(new ButtonEnableStateData(array[i]));
			}
		}
		allCoverBlockWithoutPopup.SetActive(false);
		enableAllScrollRects();
		enableAllButtons();
		kidnappedUIObject.SetActive(false);
		if (!Singleton<DataManager>.instance.currentGameData.isClearTutorial && !GlobalSetting.s_ignoreTutorial)
		{
			isTutorial = true;
			tutorialPopUpRectTransform.SetActive(true);
			UIWindowTutorial.instance.open();
			disableAllButtons();
			disableAllScrollRects();
			setTutorialType(TutorialType.TutorialType1);
			m_tutorialI18NIndex = 1;
			StopCoroutine("mainTutorialUpdate");
			StartCoroutine("mainTutorialUpdate");
		}
		else
		{
			isTutorial = false;
		}
	}

	public bool checkRebirthTutorial()
	{
		if (Singleton<DataManager>.instance.currentGameData.unlockTheme > Singleton<RebirthManager>.instance.currentRebirthRequireTheme && !Singleton<DataManager>.instance.currentGameData.isClearRebirthTutorial)
		{
			kidnappedUIObject.SetActive(false);
			isMainTutorial = false;
			isRebirthTutorial = true;
			m_tutorialI18NIndex = 1;
			isTutorial = true;
			tutorialPopUpRectTransform.SetActive(true);
			UIWindowTutorial.instance.open();
			disableAllButtons();
			disableAllScrollRects();
			setTutorialType(TutorialType.RebirthTutorialType1);
			StopCoroutine("rebirthTutorialUpdate");
			StartCoroutine("rebirthTutorialUpdate");
			return true;
		}
		return false;
	}

	private void enableAllScrollRects()
	{
		for (int i = 0; i < m_allScrollRects.Length; i++)
		{
			m_allScrollRects[i].enabled = true;
		}
	}

	private void disableAllScrollRects()
	{
		for (int i = 0; i < m_allScrollRects.Length; i++)
		{
			m_allScrollRects[i].enabled = false;
		}
	}

	public void checkTutorialState(TutorialType tutorialType)
	{
		if (isTutorial && currentTutorial == tutorialType)
		{
			m_isCanGoNext = true;
		}
	}

	public void onClickCloseButton()
	{
		if (m_isCanClose)
		{
			if (currentTutorial == TutorialType.DieWhenPlayingTutorial)
			{
				m_isClickCloseButtonForDie = true;
			}
			else
			{
				m_isClickCloseButton = true;
			}
			m_onceClose = true;
			closeTutorialPopup();
		}
	}

	private void openNextScript(TutorialType tutorlalType, float yOffset = 0f, string prefix = "TUTORIAL_DESCRIPTION_")
	{
		m_isCanGoNext = false;
		dialogRectTransform.anchoredPosition = new Vector2(0f, yOffset);
		closeBlock.SetActive(true);
		blockObject.SetActive(true);
		openTutorialPopup();
		m_targetTutorialDescriptionContent = I18NManager.Get(prefix + m_tutorialI18NIndex);
		tutorialText.SetText(TypingText.SpeakerType.Princess, m_targetTutorialDescriptionContent, 35f, null);
		setTutorialType(tutorlalType);
		m_tutorialI18NIndex++;
	}

	private void openNextScriptForDie(int scriptIndex)
	{
		m_isCanGoNext = false;
		dialogRectTransform.anchoredPosition = new Vector2(0f, 0f);
		closeBlock.SetActive(true);
		blockObject.SetActive(true);
		openTutorialPopup();
		m_targetTutorialDescriptionContent = I18NManager.Get("TUTORIAL_DEAD_DESCRIPTION_" + scriptIndex);
		tutorialText.SetText(TypingText.SpeakerType.Princess, m_targetTutorialDescriptionContent, 35f, null);
	}

	private void setTutorialType(TutorialType tutorlalType)
	{
		currentTutorial = tutorlalType;
	}

	private IEnumerator waitForCanCloseState()
	{
		float timer = 0f;
		while (true)
		{
			if (Input.GetMouseButtonDown(0))
			{
				tutorialText.ForceSetText(m_targetTutorialDescriptionContent);
			}
			timer += Time.deltaTime;
			if (timer >= 0.5f)
			{
				break;
			}
			yield return null;
		}
		m_isCanClose = true;
	}

	private void openTutorialPopup()
	{
		m_isCanClose = false;
		m_onceClose = false;
		tutorialPopUpAnimation.Play("OpenTutorialPopup");
		StartCoroutine("waitForCanCloseState");
	}

	private void closeTutorialPopup()
	{
		tutorialPopUpAnimation.Play("CloseTutorialPopup");
	}

	private void startTouchUrge()
	{
		StopCoroutine("tutorialUrgeTouchUpdate");
		StartCoroutine("tutorialUrgeTouchUpdate");
		urgeTouchEffectObject.SetActive(false);
	}

	private void stopTouchUrge()
	{
		StopCoroutine("tutorialUrgeTouchUpdate");
		urgeTouchEffectObject.SetActive(false);
	}

	private IEnumerator tutorialUrgeTouchUpdate()
	{
		float noTouchTimer = 0f;
		while (true)
		{
			if (!GameManager.isPause && isTutorial && GameManager.currentGameState == GameManager.GameState.Playing && !m_isClickCloseButton && !Singleton<GameManager>.instance.isGameOver && !Singleton<GameManager>.instance.isBossDie)
			{
				if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
				{
					noTouchTimer = 0f;
				}
				noTouchTimer += Time.deltaTime * GameManager.timeScale;
				if (noTouchTimer >= 2f && !Singleton<CharacterManager>.instance.warriorCharacter.isDead)
				{
					if (!urgeTouchEffectObject.activeSelf)
					{
						urgeTouchEffectObject.SetActive(true);
					}
				}
				else if (urgeTouchEffectObject.activeSelf)
				{
					urgeTouchEffectObject.SetActive(false);
				}
			}
			else
			{
				noTouchTimer = 0f;
				if (urgeTouchEffectObject.activeSelf)
				{
					urgeTouchEffectObject.SetActive(false);
				}
			}
			yield return null;
		}
	}

	public void setInvincible(bool isOn)
	{
		isInvincible = isOn;
	}

	public void startPrincessKidnappedEvent()
	{
		kidnappedUIObject.SetActive(true);
		kidnappedAngelObject.SetActive(true);
		kidnappedEffectObject.SetActive(false);
		isEndAngelAnimation = false;
		tutorialSkipButtonObject.SetActive(false);
		isMainTutorial = false;
		isRebirthTutorial = false;
		isEndAngelAnimation = false;
		StopAllCoroutines();
		StartCoroutine("princessKidnappedUpdate");
	}

	private IEnumerator princessKidnappedUpdate()
	{
		UIWindowTutorial.instance.open();
		disableFocus();
		m_tutorialI18NIndex = 1;
		kidnappedAngelObject.SetActive(true);
		closeBlock.SetActive(false);
		blockObject.SetActive(true);
		disableAllButtons();
		disableAllScrollRects();
		yield return new WaitUntil(() => isEndAngelAnimation);
		isEndAngelAnimation = false;
		kidnappedAngelObject.SetActive(false);
		kidnappedExclamtionObject.SetActive(true);
		Singleton<AudioManager>.instance.playBackgroundSound("boss");
		openNextScript(TutorialType.KidnappedType1, 0f, "NEW_PRINCESS_APPEAR_DESCRIPTION_TEXT_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		for (int k = 0; k < princessAnimations.Length; k++)
		{
			princessAnimations[k].animationType = "Princess" + GameManager.getCurrentPrincessNumber();
			princessAnimations[k].init();
		}
		kidnappedExclamtionObject.SetActive(false);
		closeBlock.SetActive(false);
		yield return new WaitForSeconds(0.4f);
		kidnappedEffectObject.SetActive(true);
		princessAnimations[0].playFixAnimation("Idle", 0);
		princessAnimations[1].playAnimation("Taking", 0.1f, true);
		yield return new WaitForSeconds(0.3f);
		for (int j = 0; j < Singleton<CharacterManager>.instance.constCharacterList.Count; j++)
		{
			if (Singleton<CharacterManager>.instance.constCharacterList[j] != null)
			{
				Singleton<CharacterManager>.instance.constCharacterList[j].playBoneAnimation(Singleton<CharacterManager>.instance.constCharacterList[j].currentBoneAnimationName.moveName[0]);
				Singleton<CharacterManager>.instance.constCharacterList[j].characterBoneAnimation[Singleton<CharacterManager>.instance.constCharacterList[j].currentBoneAnimationName.moveName[0]].speed = 1.8f;
			}
		}
		while (kidnappedUIAnimation.isPlaying)
		{
			yield return null;
		}
		for (int i = 0; i < Singleton<CharacterManager>.instance.constCharacterList.Count; i++)
		{
			if (Singleton<CharacterManager>.instance.constCharacterList[i] != null)
			{
				Singleton<CharacterManager>.instance.constCharacterList[i].playBoneAnimation(Singleton<CharacterManager>.instance.constCharacterList[i].currentBoneAnimationName.idleName[0]);
			}
		}
		Singleton<AudioManager>.instance.playBackgroundSound("lobby02");
		enableAllButtons();
		enableAllScrollRects();
		kidnappedEffectObject.SetActive(false);
		kidnappedUIObject.SetActive(false);
		UIWindowTutorial.instance.close();
		if (!Singleton<TutorialManager>.instance.checkRebirthTutorial() && Singleton<NanooAPIManager>.instance.UserID.Length > 0)
		{
			UIWindowDialog.openDescription("AUTO_SAVE_TO_CLOUD_QUESTION_DIALOG", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
			{
				UIWindowSaveCloud.instance.open();
			}, string.Empty);
		}
	}

	private IEnumerator rebirthTutorialUpdate()
	{
		tutorialSkipButtonObject.SetActive(false);
		isMainTutorial = false;
		isRebirthTutorial = true;
		disableFocus();
		openNextScript(TutorialType.RebirthTutorialType1, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.RebirthTutorialType2, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.RebirthTutorialType3, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.RebirthTutorialType4, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.RebirthTutorialType5, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		closeBlock.SetActive(false);
		setFocusRect(new Vector2(145.7f, -580f), new Vector2(139.5f, 120f));
		enableButton(treasureTapButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setFocusCircle(new Vector2(235.4f, -27.6f), 213.4f);
		enableButton(rebirthButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		blockObject.SetActive(false);
		disableAllButtons();
		disableFocus();
		yield return new WaitForSeconds(1.2f);
		openNextScript(TutorialType.RebirthTutorialType6, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.RebirthTutorialType7, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.RebirthTutorialType8, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.RebirthTutorialType9, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.RebirthTutorialType10, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.RebirthTutorialType11, 0f, "REBIRTH_TUTORIAL_DESCRIPTION_");
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		enableAllButtons();
		enableAllScrollRects();
		isRebirthTutorial = false;
		isTutorial = false;
		Singleton<DataManager>.instance.currentGameData.isClearRebirthTutorial = true;
		Singleton<DataManager>.instance.saveData();
		yield return new WaitForSeconds(0.4f);
		UIWindowTutorial.instance.close();
		if (Singleton<GameManager>.instance.isPlayedGameOnlyOnce && Singleton<DataManager>.instance.currentGameData.unlockTheme > 2)
		{
			int random = UnityEngine.Random.Range(0, 100);
			if (random < 3)
			{
				UIWindowSurpriseLimitedItem.instance.openLimitedPopupUI();
			}
		}
	}

	private IEnumerator mainTutorialUpdate()
	{
		tutorialSkipButtonObject.SetActive(true);
		isMainTutorial = true;
		isRebirthTutorial = false;
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		openNextScript(TutorialType.TutorialType1, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType2, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType3, -70f);
		enableButton(quickStartButton);
		closeBlock.SetActive(false);
		setFocusCircle(new Vector2(-273.3f, 247.9f), 261.4f, ArrowDirectionType.Right);
		setInvincible(true);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		blockObject.SetActive(false);
		disableFocus();
		closeTutorialPopup();
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setPauseState(true);
		openNextScript(TutorialType.TutorialType4, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType5, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType6, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		blockObject.SetActive(false);
		closeBlock.SetActive(false);
		setInvincible(false);
		setPauseState(false);
		startTouchUrge();
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setInvincible(true);
		setPauseState(true);
		openNextScript(TutorialType.TutorialType7, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		setFocusRect(new Vector2(0f, -566f), new Vector2(645.1f, 107.8f));
		openNextScript(TutorialType.TutorialType8, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		stopTouchUrge();
		openNextScript(TutorialType.TutorialType9, 0f);
		if (Singleton<FeverManager>.instance.currentMana < 1)
		{
			Singleton<FeverManager>.instance.currentMana = 1;
			Singleton<SkillManager>.instance.CheckMana();
			Singleton<SkillManager>.instance.CheckSkillAll();
		}
		setFocusRect(new Vector2(-267.9f, -438.6f), new Vector2(138.5f, 138.3f));
		enableButton(divineSmashButton);
		closeBlock.SetActive(false);
		blockObject.SetActive(true);
		setPauseState(false);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		blockObject.SetActive(false);
		closeTutorialPopup();
		disableAllButtons();
		disableFocus();
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		yield return new WaitForSeconds(1f);
		setPauseState(true);
		startTouchUrge();
		openNextScript(TutorialType.TutorialType10, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType11, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		enableButton(divineSmashButton);
		disableFocus();
		closeBlock.SetActive(false);
		blockObject.SetActive(false);
		setInvincible(false);
		setPauseState(false);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		openNextScript(TutorialType.TutorialType12, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType13, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType14, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		setFocusRect(new Vector2(-152.1f, -329.7f), new Vector2(240.2f, 116.9f));
		enableButton(gotoTownButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableAllButtons();
		disableFocus();
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setFocusRect(new Vector2(0f, -231.1f), new Vector2(696f, 131f));
		openNextScript(TutorialType.TutorialType15, 123f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType16, 123f);
		double totalUpgradeGold2 = 0.0;
		for (int j = 0; j < 9; j++)
		{
			totalUpgradeGold2 += Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(WeaponManager.WarriorWeaponType.WarriorWeapon1), j);
		}
		Singleton<GoldManager>.instance.increaseGold(totalUpgradeGold2 + 1.0);
		Singleton<FlyResourcesManager>.instance.playEffectResources(tutorialGoldEffectStartTransform.position, FlyResourcesManager.ResourceType.Gold, 15L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		setFocusRect(new Vector2(0f, -231.1f), new Vector2(696f, 131f), ArrowDirectionType.Right);
		enableButton(warriorFirstWeaponUpgradeButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableAllButtons();
		disableFocus();
		openNextScript(TutorialType.TutorialType17, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType18, -70f);
		enableButton(quickStartButton, divineSmashButton);
		closeBlock.SetActive(false);
		setFocusCircle(new Vector2(-273.3f, 247.9f), 261.4f, ArrowDirectionType.Right);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableFocus();
		closeTutorialPopup();
		blockObject.SetActive(false);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		openNextScript(TutorialType.TutorialType19, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType20, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		setFocusRect(new Vector2(-152.1f, -329.7f), new Vector2(240.2f, 116.9f));
		enableButton(gotoTownButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableAllButtons();
		disableFocus();
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setFocusRect(new Vector2(0f, -43.8f), new Vector2(231.2f, 231.2f));
		enableButton(priestTapButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setFocusRect(new Vector2(0f, -231.1f), new Vector2(696f, 131f));
		openNextScript(TutorialType.TutorialType21, 123f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType22, 123f);
		totalUpgradeGold2 = 0.0;
		for (int k = 0; k < 9; k++)
		{
			totalUpgradeGold2 += Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(WeaponManager.PriestWeaponType.PriestWeapon1), k);
		}
		Singleton<GoldManager>.instance.increaseGold(totalUpgradeGold2 + 1.0);
		Singleton<FlyResourcesManager>.instance.playEffectResources(tutorialGoldEffectStartTransform.position, FlyResourcesManager.ResourceType.Gold, 15L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		setFocusRect(new Vector2(0f, -231.1f), new Vector2(696f, 131f), ArrowDirectionType.Right);
		enableButton(priestFirstWeaponUpgradeButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableAllButtons();
		disableFocus();
		openNextScript(TutorialType.TutorialType23, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType24, -70f);
		enableButton(quickStartButton, divineSmashButton);
		closeBlock.SetActive(false);
		setFocusCircle(new Vector2(-273.3f, 247.9f), 261.4f, ArrowDirectionType.Right);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableFocus();
		closeTutorialPopup();
		blockObject.SetActive(false);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		openNextScript(TutorialType.TutorialType25, 23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType26, 23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		setFocusRect(new Vector2(-152.1f, -329.7f), new Vector2(240.2f, 116.9f));
		enableButton(gotoTownButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableAllButtons();
		disableFocus();
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setFocusRect(new Vector2(233.7f, -43.8f), new Vector2(231.2f, 231.2f));
		enableButton(archerTapButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setFocusRect(new Vector2(0f, -231.1f), new Vector2(696f, 131f));
		openNextScript(TutorialType.TutorialType27, 123f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType28, 123f);
		totalUpgradeGold2 = 0.0;
		for (int i = 0; i < 9; i++)
		{
			totalUpgradeGold2 += Singleton<WeaponManager>.instance.getWeaponUpgradeGoldPrice(Singleton<WeaponManager>.instance.getWeaponTier(WeaponManager.ArcherWeaponType.ArcherWeapon1), i);
		}
		Singleton<GoldManager>.instance.increaseGold(totalUpgradeGold2 + 1.0);
		Singleton<FlyResourcesManager>.instance.playEffectResources(tutorialGoldEffectStartTransform.position, FlyResourcesManager.ResourceType.Gold, 15L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		setFocusRect(new Vector2(0f, -231.1f), new Vector2(696f, 131f), ArrowDirectionType.Right);
		enableButton(archerFirstWeaponUpgradeButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableAllButtons();
		disableFocus();
		openNextScript(TutorialType.TutorialType29, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType30, -70f);
		enableButton(quickStartButton, divineSmashButton);
		closeBlock.SetActive(false);
		setFocusCircle(new Vector2(-273.3f, 247.9f), 261.4f, ArrowDirectionType.Right);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableFocus();
		closeTutorialPopup();
		blockObject.SetActive(false);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		openNextScript(TutorialType.TutorialType31, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType32, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		setFocusRect(new Vector2(-152.1f, -329.7f), new Vector2(240.2f, 116.9f));
		enableButton(gotoTownButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableAllButtons();
		disableFocus();
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setFocusRect(new Vector2(-142.2f, -580f), new Vector2(139.5f, 120f));
		enableButton(colleagueTapButton);
		openNextScript(TutorialType.TutorialType33, 0f);
		closeBlock.SetActive(false);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setFocusRect(new Vector2(0f, -3.8f), new Vector2(690.9f, 143.3f), ArrowDirectionType.Right);
		openNextScript(TutorialType.TutorialType34, -388f);
		closeBlock.SetActive(false);
		Singleton<FlyResourcesManager>.instance.playEffectResources(tutorialGoldEffectStartTransform.position, FlyResourcesManager.ResourceType.Gold, 15L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		Singleton<GoldManager>.instance.increaseGold(Singleton<ColleagueManager>.instance.getColleagueUnlockPrice(ColleagueManager.ColleagueType.Isabelle));
		enableButton(colleagueFirstUnlockButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		openNextScript(TutorialType.TutorialType35, -388f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		disableAllButtons();
		openNextScript(TutorialType.TutorialType36, -388f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType37, -388f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		disableFocus();
		openNextScript(TutorialType.TutorialType38, -70f);
		enableButton(quickStartButton, divineSmashButton);
		closeBlock.SetActive(false);
		setFocusCircle(new Vector2(-273.3f, 247.9f), 261.4f, ArrowDirectionType.Right);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableFocus();
		closeTutorialPopup();
		blockObject.SetActive(false);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		setInvincible(true);
		setPauseState(true);
		openNextScript(TutorialType.TutorialType39, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType40, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		closeBlock.SetActive(false);
		blockObject.SetActive(false);
		setInvincible(false);
		setPauseState(false);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		openNextScript(TutorialType.TutorialType41, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType42, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType43, -23f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		setFocusRect(new Vector2(-152.1f, -329.7f), new Vector2(240.2f, 116.9f));
		enableButton(gotoTownButton);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		disableAllButtons();
		disableFocus();
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		openNextScript(TutorialType.TutorialType44, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType45, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType46, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		openNextScript(TutorialType.TutorialType47, 0f);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		stopTouchUrge();
		disableFocus();
		closeBlock.SetActive(false);
		enableAllButtons();
		enableAllScrollRects();
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		tutorialRewardUIObject.SetActive(true);
		yield return new WaitUntil(() => m_isCanGoNext);
		m_isCanGoNext = false;
		yield return new WaitForSeconds(0.5f);
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.FirstStep, 1.0);
		isTutorial = false;
		isMainTutorial = false;
		Singleton<AdsAngelManager>.instance.setDefaultAdsAngelTimer();
		Singleton<DataManager>.instance.currentGameData.isClearTutorial = true;
		Singleton<DataManager>.instance.saveData();
		UIWindowManageHeroAndWeapon.instance.refreshSkinDescriptionObject();
		UIWindowTutorial.instance.close();
		if (Singleton<RouletteManager>.instance.isCanStartBronzeRoulette())
		{
			UIWindowRoulette.instance.openRouletteUI(true);
		}
		if (Util.isInternetConnection() && Singleton<NanooAPIManager>.instance.isNoticeComplete)
		{
			UIWindowNotice.instance.openNoticeUI(Singleton<NanooAPIManager>.instance._noticeList);
		}
	}

	public void skipTutorial()
	{
		if (isRebirthTutorial)
		{
			return;
		}
		UIWindowDialog.openDescription("TUTORIAL_SKIP_DESCRIPTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			StopAllCoroutines();
			stopTouchUrge();
			disableFocus();
			enableAllButtons();
			enableAllScrollRects();
			isTutorial = false;
			Singleton<AdsAngelManager>.instance.setDefaultAdsAngelTimer();
			Singleton<DataManager>.instance.currentGameData.isClearTutorial = true;
			Singleton<DataManager>.instance.saveData();
			UIWindowTutorial.instance.close();
			UIWindowManageHeroAndWeapon.instance.refreshSkinDescriptionObject();
			if (Singleton<RouletteManager>.instance.isCanStartBronzeRoulette())
			{
				UIWindowRoulette.instance.openRouletteUI(true);
			}
			if (GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				if (Util.isInternetConnection() && Singleton<NanooAPIManager>.instance.isNoticeComplete)
				{
					UIWindowNotice.instance.openNoticeUI(Singleton<NanooAPIManager>.instance._noticeList);
				}
			}
			else
			{
				Singleton<EnemyManager>.instance.endBossSafe();
				Singleton<CachedManager>.instance.coverUI.fadeOutGame();
			}
			ShowNotificationReminder();
		}, string.Empty);
	}

	private void setPauseState(bool isPause)
	{
		if (isTutorial)
		{
			if (isPause)
			{
				GameManager.timeScale = 0f;
				GameManager.isPause = true;
			}
			else
			{
				Singleton<GameManager>.instance.setTimeScale(true);
				GameManager.isPause = false;
			}
		}
	}

	private void disableFocus()
	{
		focusObjectRectTransform.anchoredPosition = Vector2.one * 10000f;
		blockObject.SetActive(false);
	}

	public void setFocusCircle(Vector2 position, float radius, ArrowDirectionType arrowDirection = ArrowDirectionType.Top, bool isUsingWorldPosition = false)
	{
		blockObject.SetActive(true);
		focusImage.sprite = circleSprite;
		switch (arrowDirection)
		{
		case ArrowDirectionType.Top:
			arrowParentRectTransform.anchorMin = new Vector2(0.5f, 1f);
			arrowParentRectTransform.anchorMax = new Vector2(0.5f, 1f);
			arrowParentRectTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
			arrowParentRectTransform.localScale = Vector3.one;
			break;
		case ArrowDirectionType.Bottom:
			arrowParentRectTransform.anchorMin = new Vector2(0.5f, 0f);
			arrowParentRectTransform.anchorMax = new Vector2(0.5f, 0f);
			arrowParentRectTransform.localEulerAngles = new Vector3(0f, 0f, 90f);
			arrowParentRectTransform.localScale = Vector3.one;
			break;
		case ArrowDirectionType.Left:
			arrowParentRectTransform.anchorMin = new Vector2(0f, 0.5f);
			arrowParentRectTransform.anchorMax = new Vector2(0f, 0.5f);
			arrowParentRectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
			arrowParentRectTransform.localScale = Vector3.one;
			break;
		case ArrowDirectionType.Right:
			arrowParentRectTransform.anchorMin = new Vector2(1f, 0.5f);
			arrowParentRectTransform.anchorMax = new Vector2(1f, 0.5f);
			arrowParentRectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
			arrowParentRectTransform.localScale = new Vector3(-1f, 1f, 1f);
			break;
		}
		if (position.y != -329.7f)
		{
			position.y += UIWindowTutorial.offset;
		}
		focusObjectRectTransform.sizeDelta = new Vector2(radius, radius);
		focusObjectRectTransform.anchoredPosition = position;
	}

	public void setFocusRect(Vector2 position, Vector2 sizeDelta, ArrowDirectionType arrowDirection = ArrowDirectionType.Top, bool isUsingWorldPosition = false)
	{
		blockObject.SetActive(true);
		focusImage.sprite = rectSprite;
		switch (arrowDirection)
		{
		case ArrowDirectionType.Top:
			arrowParentRectTransform.anchorMin = new Vector2(0.5f, 1f);
			arrowParentRectTransform.anchorMax = new Vector2(0.5f, 1f);
			arrowParentRectTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
			arrowParentRectTransform.localScale = Vector3.one;
			break;
		case ArrowDirectionType.Bottom:
			arrowParentRectTransform.anchorMin = new Vector2(0.5f, 0f);
			arrowParentRectTransform.anchorMax = new Vector2(0.5f, 0f);
			arrowParentRectTransform.localEulerAngles = new Vector3(0f, 0f, 90f);
			arrowParentRectTransform.localScale = Vector3.one;
			break;
		case ArrowDirectionType.Left:
			arrowParentRectTransform.anchorMin = new Vector2(0f, 0.5f);
			arrowParentRectTransform.anchorMax = new Vector2(0f, 0.5f);
			arrowParentRectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
			arrowParentRectTransform.localScale = Vector3.one;
			break;
		case ArrowDirectionType.Right:
			arrowParentRectTransform.anchorMin = new Vector2(1f, 0.5f);
			arrowParentRectTransform.anchorMax = new Vector2(1f, 0.5f);
			arrowParentRectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
			arrowParentRectTransform.localScale = new Vector3(-1f, 1f, 1f);
			break;
		}
		if (position.y != -329.7f)
		{
			position.y += UIWindowTutorial.offset;
		}
		focusObjectRectTransform.sizeDelta = sizeDelta;
		focusObjectRectTransform.anchoredPosition = position;
	}

	private IEnumerator waitForEnableCloseButton()
	{
		yield return new WaitForSeconds(1f);
		m_isCanClose = true;
		closeBlock.SetActive(true);
		yield return new WaitUntil(() => m_isClickCloseButton);
		m_isClickCloseButton = false;
		enableAllButtons();
		closeTutorialPopup();
		disableFocus();
		closeBlock.SetActive(false);
		blockObject.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		UIWindowTutorial.instance.close();
	}

	public void disableAllButtons()
	{
		for (int i = 0; i < allButtonData.Count; i++)
		{
			if (allButtonData[i].targetButton.interactable)
			{
				allButtonData[i].targetButton.interactable = false;
			}
		}
	}

	public void enableAllButtons()
	{
		for (int i = 0; i < allButtonData.Count; i++)
		{
			if (!allButtonData[i].targetButton.interactable)
			{
				allButtonData[i].targetButton.interactable = true;
			}
		}
	}

	public void enableButton(params Button[] targetButtons)
	{
		closeBlock.SetActive(false);
		for (int i = 0; i < allButtonData.Count; i++)
		{
			allButtonData[i].targetButton.interactable = false;
		}
		for (int j = 0; j < targetButtons.Length; j++)
		{
			targetButtons[j].interactable = true;
		}
	}

	private void OnApplicationQuit()
	{
		if (allButtonData != null || allButtonData.Count != 0)
		{
			enableAllButtons();
		}
	}

	public void OnClickGetReward()
	{
		Singleton<RubyManager>.instance.increaseRuby(10.0);
		Singleton<FlyResourcesManager>.instance.playEffectResources(Vector2.zero, FlyResourcesManager.ResourceType.Ruby, 10L, 0.02f, delegate
		{
			Singleton<AudioManager>.instance.playEffectSound("getgold");
		});
		DateTime dateTime = new DateTime(UnbiasedTime.Instance.Now().Ticks);
		dateTime = dateTime.AddMinutes(10.0);
		Singleton<DataManager>.instance.currentGameData.autoTouch13TapEndTime = dateTime.Ticks;
		Singleton<AutoTouchManager>.instance.refreshAutoTouchType();
		tutorialRewardUIObject.SetActive(false);
		checkTutorialState(TutorialType.TutorialType47);
		ShowNotificationReminder();
	}

	private void ShowNotificationReminder()
	{
	}
}
