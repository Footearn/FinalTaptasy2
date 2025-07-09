using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPProgress : ObjectBase
{
	public Image hpProgress;

	public Image hpLerpProgress;

	public Image totalDecreasedHpProgress;

	public Text hpText;

	public RectTransform[] hpMaxIamges;

	public Transform target;

	public Image flashImage;

	[HideInInspector]
	public float fillAmount = 1f;

	public Vector2 offset;

	public bool isShield;

	private float m_totalDecreasedProgressTimer;

	private void OnEnable()
	{
		if (!isShield)
		{
			m_totalDecreasedProgressTimer = 0f;
			refreshLerpHP();
			StopCoroutine("lerpProgressUpdate");
			StartCoroutine("lerpProgressUpdate");
		}
		Debug.LogWarning("Singleton<CharacterManager>.instance.warriorCharacter: "+Singleton<CharacterManager>.instance.warriorCharacter);
		target = Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform;
	}

	public void refreshLerpHP()
	{
		m_totalDecreasedProgressTimer = 0f;
		hpLerpProgress.fillAmount = hpProgress.fillAmount;
		totalDecreasedHpProgress.fillAmount = hpProgress.fillAmount;
	}

	private IEnumerator lerpProgressUpdate()
	{
		while (true)
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
	}

	public void setProgress(double curHealth, double maxHealth)
	{
		if (GameManager.currentGameState != 0)
		{
			return;
		}
		if (flashImage != null && (double)fillAmount > Util.Clamp(curHealth / maxHealth, 0.0, 1.0) && base.cachedGameObject.activeInHierarchy)
		{
			StartCoroutine("hitEffect");
		}
		m_totalDecreasedProgressTimer = 0f;
		fillAmount = (float)Util.Clamp(curHealth / maxHealth, 0.0, 1.0);
		if (fillAmount >= 1f && !isShield)
		{
			refreshLerpHP();
		}
		if (fillAmount <= 0.2f && fillAmount > 0f)
		{
			if (!UIWindowIngame.instance.hpWarningState)
			{
				UIWindowIngame.instance.hpWarningState = true;
				UIWindowIngame.instance.StartCoroutine("hpWarningUpdate");
			}
		}
		else if (UIWindowIngame.instance.hpWarningState)
		{
			UIWindowIngame.instance.hpWarningState = false;
		}
		hpProgress.fillAmount = fillAmount;
		if (hpText != null)
		{
			hpText.text = GameManager.changeUnit(curHealth);
		}
	}

	private void LateUpdate()
	{
		if (target != null)
		{
			base.cachedTransform.position = (Vector2)target.transform.position + offset;
		}
	}

	private IEnumerator hitEffect()
	{
		float alphaPercentage = 0f;
		float alphaIteration = 10f;
		while (!(alphaPercentage > 1f))
		{
			alphaPercentage += Time.deltaTime * alphaIteration;
			flashImage.color = new Color(1f, 0.66f, 0.168f, alphaPercentage);
			yield return null;
		}
		flashImage.color = new Color(1f, 0.66f, 0.168f, 0f);
	}
}
