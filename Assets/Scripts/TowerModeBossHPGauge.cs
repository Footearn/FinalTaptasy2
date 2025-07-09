using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TowerModeBossHPGauge : ObjectBase
{
	public CanvasGroup cachedCanvasGroup;

	public Image hpProgress;

	public Image hpLerpProgress;

	public Image totalDecreasedHpProgress;

	public TowerModeBossObject currentTargetBoss;

	public TowerModeMapObject currentTargetMap;

	[HideInInspector]
	public float fillAmount = 1f;

	public Vector2 offset;

	private float m_totalDecreasedProgressTimer;

	public void setBossHPGauge(TowerModeBossObject target, TowerModeMapObject targetMap)
	{
		currentTargetBoss = target;
		currentTargetMap = targetMap;
		cachedCanvasGroup.alpha = 1f;
		m_totalDecreasedProgressTimer = 0f;
		refreshLerpHP();
		StopCoroutine("lerpProgressUpdate");
		StartCoroutine("lerpProgressUpdate");
	}

	public void refreshLerpHP()
	{
		m_totalDecreasedProgressTimer = 0f;
		hpLerpProgress.fillAmount = hpProgress.fillAmount;
		totalDecreasedHpProgress.fillAmount = hpProgress.fillAmount;
	}

	private IEnumerator lerpProgressUpdate()
	{
		while (!currentTargetBoss.isDead)
		{
			if (!GameManager.isPause)
			{
				m_totalDecreasedProgressTimer += Time.deltaTime * GameManager.timeScale;
				float hpFillAmount = hpProgress.fillAmount;
				float lerpHpFillAmount = hpLerpProgress.fillAmount;
				float totalDecreasedHpFillAmount = totalDecreasedHpProgress.fillAmount;
				if (m_totalDecreasedProgressTimer >= 0.6f)
				{
					if (totalDecreasedHpFillAmount != hpFillAmount)
					{
						totalDecreasedHpFillAmount = ((!(hpFillAmount > totalDecreasedHpFillAmount)) ? (totalDecreasedHpFillAmount - Time.deltaTime * GameManager.timeScale * 1.3f) : (totalDecreasedHpFillAmount + Time.deltaTime * GameManager.timeScale * 1.3f));
					}
					if (Mathf.Abs(totalDecreasedHpFillAmount - hpFillAmount) <= Time.deltaTime * GameManager.timeScale * 1.3f)
					{
						totalDecreasedHpFillAmount = hpFillAmount;
					}
					totalDecreasedHpProgress.fillAmount = totalDecreasedHpFillAmount;
				}
				if (lerpHpFillAmount != hpFillAmount)
				{
					lerpHpFillAmount = ((!(hpFillAmount > lerpHpFillAmount)) ? (lerpHpFillAmount - Time.deltaTime * GameManager.timeScale * 0.25f) : (lerpHpFillAmount + Time.deltaTime * GameManager.timeScale * 0.25f));
				}
				if (Mathf.Abs(lerpHpFillAmount - hpFillAmount) <= Time.deltaTime * GameManager.timeScale * 0.25f)
				{
					lerpHpFillAmount = hpFillAmount;
				}
				hpLerpProgress.fillAmount = lerpHpFillAmount;
			}
			yield return null;
		}
		while (cachedCanvasGroup.alpha > 0f)
		{
			if (!GameManager.isPause)
			{
				cachedCanvasGroup.alpha -= Time.deltaTime * GameManager.timeScale * 3f;
			}
			yield return null;
		}
		cachedCanvasGroup.alpha = 0f;
	}

	public void setProgress(double curHealth, double maxHealth)
	{
		m_totalDecreasedProgressTimer = 0f;
		fillAmount = (float)Util.Clamp(curHealth / maxHealth, 0.0, 1.0);
		if (fillAmount >= 1f)
		{
			refreshLerpHP();
		}
		hpProgress.fillAmount = fillAmount;
	}

	private void LateUpdate()
	{
		if (currentTargetMap != null)
		{
			base.cachedTransform.position = (Vector2)currentTargetMap.cachedTransform.position + offset;
		}
	}
}
