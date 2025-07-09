using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowDialog : UIWindow
{
	public enum DialogState
	{
		Empty,
		CloseUI,
		SelectBetweenYesOrNoUI,
		DelegateOKUI
	}

	public const string NOT_ENOUGH_GOLD = "NOT_ENOUGH_GOLD";

	public const string NOT_ENOUGH_RUBY = "NOT_ENOUGH_RUBY";

	public const string NOT_ENOUGH_TREASURE_PIECE = "NOT_ENOUGH_RARE_TREASURE_PIECE";

	public const string NOT_ENOUGH_TRANSCEND_STONE = "NOT_ENOUGH_TRANSCEND_STONE_FOR_TIER_UP";

	public const string NOT_ENOUGH_HEART_FOR_ELOPE_MODE = "NOT_ENOUGH_HEART_FOR_ELOPE_MODE";

	public const string NOT_ENOUGH_TICKET_FOR_TOWER_MODE = "NOT_ENOUGH_TOWER_MODE_TICKET_DESCRIPTION";

	public const string NOT_ENOUGH_WEAPON_SKIN_PIECE = "NOT_ENOUGH_WEAPON_SKIN_PIECE";

	public static UIWindowDialog[] instances;

	public GameObject dialogObject;

	public GameObject miniDialogObject;

	public CanvasGroup miniDialogCanvasGroup;

	public Animation miniDialogAnimation;

	public Text miniDialogText;

	public GameObject elopeDialogObject;

	public GameObject normalDialogObject;

	public GameObject closeButton;

	public GameObject[] okObjects;

	public GameObject[] delegateOkObjects;

	public GameObject[] okAndCancleObjects;

	public new GameObject cachedGameObject;

	public Text[] titleTexts;

	public Text[] descriptionTexts;

	public int instanceIndex;

	public DialogState currentDialogType;

	private Action m_okCallback;

	private string m_currentI18NTitleID;

	private int m_currentTargetValue;

	private bool? m_isMiniPopup;

	public override void Awake()
	{
		cachedGameObject = base.gameObject;
		if (instances == null)
		{
			instances = new UIWindowDialog[5];
		}
		instances[instanceIndex] = this;
		base.Awake();
	}

	public override void OnAfterClose()
	{
		m_okCallback = null;
		currentDialogType = DialogState.Empty;
		base.OnAfterClose();
	}

	public static void openDescription(string i18NTitleID, DialogState dialogType, Action okCallback = null, string titleText = "")
	{
		for (int i = 0; i < instances.Length; i++)
		{
			if (instances[i] != null && !instances[i].cachedGameObject.activeSelf)
			{
				instances[i].openWithDescription(I18NManager.Get(i18NTitleID), dialogType, okCallback, titleText);
				break;
			}
		}
	}

	public static void openDescriptionNotUsingI18N(string dialogText, DialogState dialogType, Action okCallback = null, string titleText = "")
	{
		for (int i = 0; i < instances.Length; i++)
		{
			if (instances[i] != null && !instances[i].cachedGameObject.activeSelf)
			{
				instances[i].openWithDescription(dialogText, dialogType, okCallback, titleText);
				break;
			}
		}
	}

	public static void openMiniDialog(string i18nText)
	{
		for (int i = 0; i < instances.Length; i++)
		{
			if (instances[i] != null && !instances[i].cachedGameObject.activeSelf)
			{
				instances[i].openMiniDialogUI(I18NManager.Get(i18nText));
				break;
			}
		}
	}

	public static void openMiniDialogWithoutI18N(string text)
	{
		for (int i = 0; i < instances.Length; i++)
		{
			if (instances[i] != null && !instances[i].cachedGameObject.activeSelf)
			{
				instances[i].openMiniDialogUI(text);
				break;
			}
		}
	}

	public void openWithDescription(string dialogText, DialogState dialogType, Action okCallback, string titleText)
	{
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode)
		{
			if (normalDialogObject.activeSelf)
			{
				normalDialogObject.SetActive(false);
			}
			if (!elopeDialogObject.activeSelf)
			{
				elopeDialogObject.SetActive(true);
			}
			for (int i = 0; i < titleTexts.Length; i++)
			{
				if (string.IsNullOrEmpty(titleText))
				{
					titleTexts[i].text = I18NManager.Get("ELOPE_MODE");
				}
				else
				{
					titleTexts[i].text = titleText;
				}
			}
		}
		else
		{
			if (!normalDialogObject.activeSelf)
			{
				normalDialogObject.SetActive(true);
			}
			if (elopeDialogObject.activeSelf)
			{
				elopeDialogObject.SetActive(false);
			}
		}
		isCanCloseESC = true;
		m_isMiniPopup = false;
		if (!dialogObject.activeSelf)
		{
			dialogObject.SetActive(true);
		}
		if (miniDialogObject.activeSelf)
		{
			miniDialogObject.SetActive(false);
		}
		currentDialogType = dialogType;
		if (dialogType == DialogState.DelegateOKUI)
		{
			isCanCloseESC = false;
		}
		else
		{
			isCanCloseESC = true;
		}
		switch (dialogType)
		{
		case DialogState.CloseUI:
		{
			for (int m = 0; m < delegateOkObjects.Length; m++)
			{
				delegateOkObjects[m].SetActive(false);
			}
			closeButton.SetActive(true);
			for (int n = 0; n < okObjects.Length; n++)
			{
				okObjects[n].SetActive(true);
			}
			for (int num = 0; num < okAndCancleObjects.Length; num++)
			{
				okAndCancleObjects[num].SetActive(false);
			}
			break;
		}
		case DialogState.SelectBetweenYesOrNoUI:
		{
			for (int num2 = 0; num2 < delegateOkObjects.Length; num2++)
			{
				delegateOkObjects[num2].SetActive(false);
			}
			closeButton.SetActive(true);
			for (int num3 = 0; num3 < okObjects.Length; num3++)
			{
				okObjects[num3].SetActive(false);
			}
			for (int num4 = 0; num4 < okAndCancleObjects.Length; num4++)
			{
				okAndCancleObjects[num4].SetActive(true);
			}
			break;
		}
		case DialogState.DelegateOKUI:
		{
			for (int j = 0; j < delegateOkObjects.Length; j++)
			{
				delegateOkObjects[j].SetActive(true);
			}
			closeButton.SetActive(false);
			for (int k = 0; k < okObjects.Length; k++)
			{
				okObjects[k].SetActive(false);
			}
			for (int l = 0; l < okAndCancleObjects.Length; l++)
			{
				okAndCancleObjects[l].SetActive(false);
			}
			break;
		}
		}
		m_okCallback = okCallback;
		m_currentI18NTitleID = string.Empty;
		m_currentTargetValue = 0;
		for (int num5 = 0; num5 < descriptionTexts.Length; num5++)
		{
			descriptionTexts[num5].text = dialogText;
		}
		open(true);
	}

	public void openMiniDialogUI(string text)
	{
		isCanCloseESC = false;
		open(true);
		m_isMiniPopup = true;
		miniDialogCanvasGroup.alpha = 0f;
		if (dialogObject.activeSelf)
		{
			dialogObject.SetActive(false);
		}
		miniDialogObject.SetActive(false);
		miniDialogObject.SetActive(true);
		miniDialogText.text = text;
		miniDialogAnimation.Stop();
		miniDialogAnimation.Play();
		StopAllCoroutines();
		StartCoroutine(waitForMiniDialog());
	}

	private IEnumerator waitForMiniDialog()
	{
		yield return new WaitWhile(() => miniDialogAnimation.isPlaying);
		close();
	}

	public void clickOkButton()
	{
		close();
		if (m_okCallback != null)
		{
			m_okCallback();
		}
	}

	public void OnClickCloseByBlock()
	{
		if (currentDialogType != DialogState.DelegateOKUI)
		{
			close();
		}
	}
}
