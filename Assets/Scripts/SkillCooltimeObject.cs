using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooltimeObject : ObjectBase
{
	public static float intervalBetweenSkillCooltimeSlot = 84f;

	public Image cooltimeBlockImage;

	public Text cooltimeText;

	public CanvasGroup cachedCanvasGroup;

	private Vector2 m_targetPosition;

	private void OnEnable()
	{
		Vector2 anchoredPosition = new Vector2(75f, base.cachedRectTransform.anchoredPosition.y);
		base.cachedRectTransform.anchoredPosition = anchoredPosition;
	}

	public void initCooltimeSlot(Vector2 targetPosition, float maxCooltime)
	{
		setTargetPosition(targetPosition);
		base.cachedRectTransform.anchoredPosition = new Vector2(75f, targetPosition.y);
		cachedCanvasGroup.alpha = 0f;
		cooltimeBlockImage.fillAmount = 1f;
		base.cachedGameObject.SetActive(true);
		StopCoroutine("closeCooltimeSlotUpdate");
		StopCoroutine("cooltimeUpdate");
		StartCoroutine("cooltimeUpdate");
	}

	public void forceClostCooltimeUI()
	{
		StopAllCoroutines();
		base.cachedGameObject.SetActive(false);
	}

	public void changeCooltimeValue(float maxCooltime, float currentCooltime)
	{
		cooltimeText.text = ((int)(maxCooltime - currentCooltime)).ToString();
		cooltimeBlockImage.fillAmount = 1f - currentCooltime / maxCooltime;
	}

	private IEnumerator cooltimeUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (cachedCanvasGroup.alpha < 1f)
				{
					cachedCanvasGroup.alpha = Mathf.Max(0f, cachedCanvasGroup.alpha + Time.deltaTime * GameManager.timeScale);
				}
				Vector2 anchoredPosition2 = base.cachedRectTransform.anchoredPosition;
				anchoredPosition2 = Vector2.Lerp(anchoredPosition2, m_targetPosition, Time.deltaTime * GameManager.timeScale * 4f);
				base.cachedRectTransform.anchoredPosition = anchoredPosition2;
			}
			yield return null;
		}
	}

	private IEnumerator closeCooltimeSlotUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (cachedCanvasGroup.alpha > 0f)
				{
					cachedCanvasGroup.alpha = Mathf.Min(1f, cachedCanvasGroup.alpha - Time.deltaTime * GameManager.timeScale * 4f);
				}
				if (cachedCanvasGroup.alpha <= 0f)
				{
					break;
				}
				Vector2 anchoredPosition2 = base.cachedRectTransform.anchoredPosition;
				anchoredPosition2 = Vector2.Lerp(anchoredPosition2, new Vector2(75f, anchoredPosition2.y), Time.deltaTime * GameManager.timeScale * 4f);
				base.cachedRectTransform.anchoredPosition = anchoredPosition2;
			}
			yield return null;
		}
		base.cachedGameObject.SetActive(false);
	}

	public void closeCooltimeSlot()
	{
		StopCoroutine("cooltimeUpdate");
		UIWindowIngame.instance.skillCooltimeObjectList.Remove(this);
		StopCoroutine("closeCooltimeSlotUpdate");
		if (base.cachedGameObject.activeSelf && base.cachedGameObject.activeInHierarchy)
		{
			StartCoroutine("closeCooltimeSlotUpdate");
			return;
		}
		Vector2 anchoredPosition = new Vector2(75f, base.cachedRectTransform.anchoredPosition.y);
		base.cachedRectTransform.anchoredPosition = anchoredPosition;
	}

	public void setTargetPosition(Vector2 targetPosition)
	{
		m_targetPosition = targetPosition;
	}
}
