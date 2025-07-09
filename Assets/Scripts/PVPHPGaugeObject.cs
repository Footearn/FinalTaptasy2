using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PVPHPGaugeObject : ObjectBase
{
	public PVPUnitObject targetUnitObject;

	public Vector2 offset;

	public Image hpBarImage;

	public Image hpLerpBarImage;

	public CanvasGroup cachedCanvasGroup;

	private bool m_isOpen;

	private Coroutine m_alphaUpdateCoroutine;

	public void initGaugeBar(PVPUnitObject targetUnit)
	{
		m_isOpen = false;
		targetUnitObject = targetUnit;
		openGauge();
	}

	public void recycleGaugeBar(bool withRemoveList = true)
	{
		if (withRemoveList)
		{
			Singleton<PVPHPGaugeManager>.instance.totalHpGaugeList.Remove(this);
		}
		targetUnitObject = null;
		NewObjectPool.Recycle(this);
	}

	private void openGauge()
	{
		if (!m_isOpen)
		{
			m_isOpen = true;
			if (m_alphaUpdateCoroutine != null)
			{
				StopCoroutine(m_alphaUpdateCoroutine);
			}
			if (Util.isActive(base.cachedGameObject))
			{
				m_alphaUpdateCoroutine = StartCoroutine(gaugeAlphaUpdate(true));
			}
		}
	}

	public void closeGauge()
	{
		if (m_isOpen)
		{
			m_isOpen = false;
			if (m_alphaUpdateCoroutine != null)
			{
				StopCoroutine(m_alphaUpdateCoroutine);
			}
			if (Util.isActive(base.cachedGameObject))
			{
				m_alphaUpdateCoroutine = StartCoroutine(gaugeAlphaUpdate(false));
			}
		}
	}

	private IEnumerator gaugeAlphaUpdate(bool isFadeIn)
	{
		cachedCanvasGroup.alpha = ((!isFadeIn) ? 1 : 0);
		while ((!isFadeIn) ? (cachedCanvasGroup.alpha > 0f) : (cachedCanvasGroup.alpha < 1f))
		{
			float deltaTime = Time.deltaTime * GameManager.timeScale;
			cachedCanvasGroup.alpha += (float)(isFadeIn ? 1 : (-1)) * Time.deltaTime * GameManager.timeScale * 5f;
			yield return null;
		}
		cachedCanvasGroup.alpha = (isFadeIn ? 1 : 0);
		if (!isFadeIn)
		{
			recycleGaugeBar();
		}
	}

	private void LateUpdate()
	{
		if (!(targetUnitObject == null))
		{
			base.cachedTransform.position = targetUnitObject.bodyTransform.position + (Vector3)offset;
			float num = (float)(targetUnitObject.currentHP / targetUnitObject.maxHP);
			hpBarImage.fillAmount = num;
			hpLerpBarImage.fillAmount = Mathf.Lerp(hpLerpBarImage.fillAmount, num, Time.deltaTime * GameManager.timeScale * 3f);
		}
	}
}
