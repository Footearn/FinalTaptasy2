using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResetSkillCooltime : ObjectBase
{
	private Image m_cachedImage;

	private float m_startAlpha;

	private void Awake()
	{
		m_cachedImage = GetComponent<Image>();
	}

	public void setCooltimeEffect(float startAlpha)
	{
		m_startAlpha = startAlpha;
		StopAllCoroutines();
		if (base.cachedGameObject.activeSelf)
		{
			StartCoroutine("alphaUpdate");
		}
	}

	private IEnumerator alphaUpdate()
	{
		Color color = m_cachedImage.color;
		color.a = m_startAlpha;
		m_cachedImage.color = color;
		while (true)
		{
			if (!GameManager.isPause)
			{
				color = m_cachedImage.color;
				color.a -= Time.deltaTime * GameManager.timeScale * 1.5f;
				m_cachedImage.color = color;
				if (color.a <= 0f)
				{
					break;
				}
			}
			yield return null;
		}
		color.a = 0f;
		m_cachedImage.color = color;
	}
}
