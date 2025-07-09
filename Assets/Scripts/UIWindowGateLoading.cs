using System;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowGateLoading : UIWindow
{
	public static UIWindowGateLoading instance;

	public RectTransform rightGateTransform;

	public RectTransform leftGateTransform;

	public Animation gateAnimation;

	public RectTransform titleRectTransform;

	public GameObject loadingObject;

	public Image loadingProgressBarImage;

	public Text loadingProgressText;

	private Action m_openAction;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openGateLoading(bool isOutgameToPVP = false, Action openAction = null)
	{
		closeLoadingObject();
		m_openAction = openAction;
		rightGateTransform.anchoredPosition = new Vector2(1000f, 0f);
		leftGateTransform.anchoredPosition = new Vector2(-1000f, 0f);
		if (isOutgameToPVP)
		{
			titleRectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
			m_openAction = (Action)Delegate.Combine(m_openAction, new Action(openLoadingObject));
		}
		else
		{
			titleRectTransform.localEulerAngles = new Vector3(0f, 0f, 90f);
		}
		open();
		gateAnimation.Stop();
		gateAnimation.Play("GateOpenAnimation");
	}

	public void closeGateLoading(bool isOutgameToPVP = false)
	{
		rightGateTransform.anchoredPosition = new Vector2(233f, 0f);
		leftGateTransform.anchoredPosition = new Vector2(-233f, 0f);
		gateAnimation.Stop();
		if (isOutgameToPVP)
		{
			gateAnimation.Play("GateCloseOutgameToPVPAnimation");
		}
		else
		{
			gateAnimation.Play("GateClosePVPToOutgameAnimation");
		}
		closeLoadingObject();
	}

	public void openLoadingObject()
	{
		setLoadingText(1f, 0f);
		loadingObject.SetActive(true);
	}

	public void closeLoadingObject()
	{
		loadingObject.SetActive(false);
	}

	public void setLoadingText(float maxProgress, float currentProgress)
	{
		loadingProgressBarImage.fillAmount = currentProgress / maxProgress;
		loadingProgressText.text = (loadingProgressBarImage.fillAmount * 100f).ToString("f0") + "%";
	}

	public void gateOpenEvent()
	{
		if (m_openAction != null)
		{
			m_openAction();
		}
	}

	public void gateCloseEvent()
	{
		close();
	}
}
