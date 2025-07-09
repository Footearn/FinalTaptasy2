using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowLoading : UIWindow
{
	public static UIWindowLoading instance;

	private Action m_afterOpenAcrtion;

	public Text tipText;

	public SpriteAnimation princessSpriteAnimation;

	public RectTransform loadingObjectRectTransform;

	private int m_currentPrincessIndex = -10;

	private bool m_isWatchTip;

	private float m_maxTimeOutTime;

	private Action m_timeOutAction;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		int i = 1;
		if (m_currentPrincessIndex != GameManager.getCurrentPrincessNumber())
		{
			m_currentPrincessIndex = GameManager.getCurrentPrincessNumber();
			princessSpriteAnimation.animationType = "Princess" + Math.Min(m_currentPrincessIndex, GameManager.maxPrincessCount);
			princessSpriteAnimation.init();
		}
		for (; I18NManager.ContainsKey("RESULT_TIP_" + i); i++)
		{
		}
		if (m_isWatchTip)
		{
			tipText.text = "Tip : " + I18NManager.Get("RESULT_TIP_" + UnityEngine.Random.Range(1, i));
		}
		else
		{
			tipText.text = string.Empty;
		}
		if (m_afterOpenAcrtion != null)
		{
			m_afterOpenAcrtion();
		}
		return base.OnBeforeOpen();
	}

	public override void OnAfterActiveGameObject()
	{
		princessSpriteAnimation.playAnimation("Move", 0.18f, true);
		base.OnAfterActiveGameObject();
	}

	public void openTimeCheckingLoadingUI(float timeOutTime, Action timeOutAction)
	{
		m_maxTimeOutTime = timeOutTime;
		m_timeOutAction = timeOutAction;
		m_timeOutAction = (Action)Delegate.Combine(m_timeOutAction, new Action(closeLoadingUI));
		m_isWatchTip = true;
		m_afterOpenAcrtion = null;
		open();
		StopCoroutine("timeOutCheckUpdate");
		StartCoroutine("timeOutCheckUpdate");
	}

	public void stopTimeOutCheck()
	{
		StopCoroutine("timeOutCheckUpdate");
		m_timeOutAction = null;
	}

	private IEnumerator timeOutCheckUpdate()
	{
		float timer = 0f;
		while (timer < m_maxTimeOutTime)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		m_timeOutAction();
		stopTimeOutCheck();
	}

	public void openLoadingUI(Action afterOpenAcrion = null, bool isWatchTip = true)
	{
		m_isWatchTip = isWatchTip;
		if (m_isWatchTip)
		{
			loadingObjectRectTransform.anchoredPosition = new Vector2(0f, 44f);
		}
		else
		{
			loadingObjectRectTransform.anchoredPosition = new Vector2(0f, -10f);
		}
		m_afterOpenAcrtion = afterOpenAcrion;
		open();
	}

	public void closeLoadingUI()
	{
		stopTimeOutCheck();
		close();
	}
}
