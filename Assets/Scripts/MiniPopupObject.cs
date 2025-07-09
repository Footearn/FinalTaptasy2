using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniPopupObject : ObjectBase
{
	public MiniPopupManager.MiniPopupType currentMiniPopupType;

	public Image iconImage;

	public RectTransform iconRectTransform;

	public Text timerText;

	public DateTime currentTargetEndTime;

	public long currentLeftCount;

	public CanvasGroup cachedCanvasGroup;

	private Vector2 m_targetPosition;

	private Action m_endTimeAction;

	private bool m_isInit;

	private Vector2 m_ingameBasePosition;

	private Vector2 m_outGameBasePosition;

	private bool m_isIngame;

	private bool m_isEndless;

	private bool m_isCountPopup;

	public void setTargetPosition(Vector2 position, bool isIngame)
	{
		m_isIngame = isIngame;
		m_targetPosition = position;
	}

	public void initMiniPopupObject(MiniPopupManager.MiniPopupType miniPopupType, bool isEndlessTime, DateTime targetEndTime, Action endTimeAction)
	{
		m_isCountPopup = false;
		currentLeftCount = 0L;
		currentMiniPopupType = miniPopupType;
		m_isInit = true;
		m_isEndless = isEndlessTime;
		m_endTimeAction = endTimeAction;
		if (m_isEndless)
		{
			timerText.text = string.Empty;
			iconRectTransform.anchoredPosition = Vector2.zero;
			iconRectTransform.localScale = new Vector3(1.3f, 1.3f, 1f);
		}
		else
		{
			iconRectTransform.anchoredPosition = new Vector2(0f, 10.2f);
			iconRectTransform.localScale = Vector3.one;
		}
		iconImage.sprite = Singleton<MiniPopupManager>.instance.getIconSprite(currentMiniPopupType);
		iconImage.SetNativeSize();
		currentTargetEndTime = targetEndTime;
	}

	public void initMiniPopupObject(MiniPopupManager.MiniPopupType miniPopupType, long leftCount, Action endTimeAction)
	{
		m_isCountPopup = true;
		currentLeftCount = leftCount;
		currentMiniPopupType = miniPopupType;
		m_isInit = true;
		m_isEndless = false;
		m_endTimeAction = endTimeAction;
		if (m_isEndless)
		{
			timerText.text = string.Empty;
			iconRectTransform.anchoredPosition = Vector2.zero;
			iconRectTransform.localScale = new Vector3(1.3f, 1.3f, 1f);
		}
		else
		{
			iconRectTransform.anchoredPosition = new Vector2(0f, 10.2f);
			iconRectTransform.localScale = Vector3.one;
		}
		timerText.text = currentLeftCount.ToString();
		iconImage.sprite = Singleton<MiniPopupManager>.instance.getIconSprite(currentMiniPopupType);
		iconImage.SetNativeSize();
	}

	public void closeMiniPopupObject(bool isExcuteEndTimeAction = true)
	{
		m_isInit = false;
		Singleton<MiniPopupManager>.instance.currentMiniPopupObjectList.Remove(this);
		if (isExcuteEndTimeAction && m_endTimeAction != null)
		{
			m_endTimeAction();
			m_endTimeAction = null;
		}
		Singleton<MiniPopupManager>.instance.refreshTargetPositions();
		StartCoroutine("closeUpdate");
	}

	private IEnumerator closeUpdate()
	{
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			cachedCanvasGroup.alpha = Mathf.Lerp(cachedCanvasGroup.alpha, 0f, Time.deltaTime * GameManager.timeScale * 8f);
			if (m_isIngame)
			{
				base.cachedTransform.position = Vector2.Lerp(base.cachedTransform.position, new Vector2(m_targetPosition.x - 150f, base.cachedTransform.position.y), Time.deltaTime * 5f);
			}
			else
			{
				base.cachedTransform.position = Vector2.Lerp(base.cachedTransform.position, new Vector2(m_targetPosition.x, base.cachedTransform.position.y + 200f), Time.deltaTime * 5f);
			}
			yield return null;
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}

	private void Update()
	{
		if (!m_isEndless)
		{
			if (!m_isCountPopup)
			{
				if (currentTargetEndTime.Ticks >= UnbiasedTime.Instance.Now().Ticks)
				{
					cachedCanvasGroup.alpha = Mathf.Lerp(cachedCanvasGroup.alpha, 1f, Time.deltaTime * GameManager.timeScale * 4f);
					TimeSpan timeSpan = new TimeSpan(currentTargetEndTime.Ticks - UnbiasedTime.Instance.Now().Ticks);
					string text = string.Format(((!(timeSpan.TotalHours > 0.0)) ? string.Empty : "{0:00}:") + "{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
					timerText.text = text;
				}
				else if (m_isInit)
				{
					closeMiniPopupObject();
				}
			}
			else
			{
				cachedCanvasGroup.alpha = Mathf.Lerp(cachedCanvasGroup.alpha, 1f, Time.deltaTime * GameManager.timeScale * 4f);
			}
		}
		else
		{
			cachedCanvasGroup.alpha = Mathf.Lerp(cachedCanvasGroup.alpha, 1f, Time.deltaTime * GameManager.timeScale * 4f);
		}
		if ((Vector2)base.cachedTransform.localPosition != m_targetPosition)
		{
			base.cachedTransform.localPosition = Vector2.Lerp(base.cachedTransform.localPosition, m_targetPosition, Time.deltaTime * 10f);
		}
	}
}
