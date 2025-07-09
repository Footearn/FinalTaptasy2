using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElopeSkillMiniPopupObject : ObjectBase
{
	public ElopeModeManager.DaemonKingSkillType currentSkillType;

	public Text remainTimeText;

	public CanvasGroup cachedCanvasGroup;

	public bool isOpen;

	public Vector2 targetPosition;

	public Vector2 currentTargetPosition;

	public void openMiniPopup(bool forceOpen = false)
	{
		currentTargetPosition = targetPosition;
		if (!isOpen || forceOpen)
		{
			isOpen = true;
			StopAllCoroutines();
			if (forceOpen)
			{
				base.cachedRectTransform.anchoredPosition = targetPosition;
				cachedCanvasGroup.alpha = 1f;
				return;
			}
			cachedCanvasGroup.alpha = 0f;
			Vector2 anchoredPosition = currentTargetPosition;
			anchoredPosition.x += UIWindowElopeMode.intervalBetweenMiniPopup;
			base.cachedRectTransform.anchoredPosition = anchoredPosition;
			base.cachedAnimation.Stop();
			base.cachedAnimation.Play("OpenElopeMiniPopup");
			StartCoroutine(positionUpdate());
		}
	}

	public void closeMiniPopup(bool forceClose = false)
	{
		currentTargetPosition.x += UIWindowElopeMode.intervalBetweenMiniPopup;
		if (isOpen || forceClose)
		{
			isOpen = false;
			StopAllCoroutines();
			if (forceClose)
			{
				base.cachedRectTransform.anchoredPosition = targetPosition;
				cachedCanvasGroup.alpha = 0f;
				return;
			}
			base.cachedAnimation.Stop();
			base.cachedAnimation.Play("CloseElopeMiniPopup");
			cachedCanvasGroup.alpha = 1f;
			StartCoroutine(positionUpdate());
		}
	}

	private IEnumerator positionUpdate()
	{
		double remainSeconds = Singleton<ElopeModeManager>.instance.getSkillRemainSecond(currentSkillType);
		while (remainSeconds > 0.0)
		{
			remainSeconds = Singleton<ElopeModeManager>.instance.getSkillRemainSecond(currentSkillType);
			TimeSpan remainTime = TimeSpan.FromSeconds(remainSeconds);
			string remainTimeString = string.Format((((long)remainTime.TotalHours <= 0) ? string.Empty : "{0:00}:") + "{1:00}:{2:00}", (int)remainTime.TotalHours, remainTime.Minutes, remainTime.Seconds);
			remainTimeText.text = remainTimeString;
			Vector2 position2 = base.cachedRectTransform.anchoredPosition;
			position2 = Vector2.Lerp(position2, currentTargetPosition, Time.deltaTime * 10f);
			base.cachedRectTransform.anchoredPosition = position2;
			yield return null;
		}
		UIWindowElopeMode.instance.closeSkillPopup(currentSkillType);
		Singleton<ElopeModeManager>.instance.refreshSkillStatus();
	}
}
