using System;
using System.Collections;
using UnityEngine;

public class FadeChangableObject : SpriteGroup
{
	private Action m_endAction;

	private float m_speed;

	public float targetSpeed;

	public bool isFadeInOnEnable;

	public bool isFadeOutOnEnable;

	private void OnEnable()
	{
		if (isFadeInOnEnable)
		{
			fadeIn(targetSpeed, null);
		}
		else if (isFadeOutOnEnable)
		{
			fadeOut(targetSpeed, null);
		}
	}

	public void fadeIn(float speed, Action endAction)
	{
		m_speed = speed;
		m_endAction = endAction;
		StopCoroutine("fadeUpdate");
		StartCoroutine("fadeUpdate", true);
	}

	public void fadeOut(float speed, Action endAction)
	{
		m_speed = speed;
		m_endAction = endAction;
		StopCoroutine("fadeUpdate");
		StartCoroutine("fadeUpdate", false);
	}

	private IEnumerator fadeUpdate(bool isFadeIn)
	{
		setAlpha(isFadeIn ? 1 : 0);
		while (true)
		{
			if (isFadeIn)
			{
				alpha -= Time.deltaTime * m_speed;
				setAlpha(alpha);
				if (alpha <= 0f)
				{
					break;
				}
			}
			else
			{
				alpha += Time.deltaTime * m_speed;
				setAlpha(alpha);
				if (alpha >= 1f)
				{
					break;
				}
			}
			yield return null;
		}
		if (m_endAction != null)
		{
			m_endAction();
		}
	}
}
