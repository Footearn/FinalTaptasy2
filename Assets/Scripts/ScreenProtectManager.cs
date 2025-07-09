using System.Collections;
using UnityEngine;

public class ScreenProtectManager : Singleton<ScreenProtectManager>
{
	public CanvasGroup blockCanvasGroup;

	public Camera[] allCameras;

	private bool m_isOpenBlock;

	private float m_timer;

	private float m_fadeSpeed = 1.1f;

	private void Update()
	{
		if (NSPlayerPrefs.GetInt("ScreenProtect", 0) == 1)
		{
			if (!Singleton<GameManager>.instance.isThemeEvent && !PVPManager.isPlayingPVP)
			{
				if (Input.GetMouseButton(0) || Input.touchCount > 0)
				{
					setBlock(true);
					return;
				}
				m_timer += Time.deltaTime;
				if (m_timer >= 10f)
				{
					setBlock(false);
				}
			}
			else
			{
				setBlock(true);
			}
		}
		else
		{
			setBlock(true);
		}
	}

	public void setBlock(bool isOpen)
	{
		if (isOpen)
		{
			for (int i = 0; i < allCameras.Length; i++)
			{
				if (!allCameras[i].enabled)
				{
					allCameras[i].enabled = true;
				}
			}
			m_timer = 0f;
			if (m_isOpenBlock)
			{
				m_isOpenBlock = false;
				StopCoroutine("fadeUpdate");
				StartCoroutine("fadeUpdate", true);
			}
		}
		else if (!m_isOpenBlock)
		{
			m_isOpenBlock = true;
			StopCoroutine("fadeUpdate");
			StartCoroutine("fadeUpdate", false);
		}
	}

	private IEnumerator fadeUpdate(bool isFadeIn)
	{
		while ((!isFadeIn) ? (blockCanvasGroup.alpha < 1f) : (blockCanvasGroup.alpha > 0f))
		{
			blockCanvasGroup.alpha += ((!isFadeIn) ? m_fadeSpeed : (0f - m_fadeSpeed)) * Time.deltaTime;
			yield return null;
		}
		if (isFadeIn)
		{
			blockCanvasGroup.alpha = 0f;
			yield break;
		}
		blockCanvasGroup.alpha = 1f;
		for (int i = 0; i < allCameras.Length; i++)
		{
			if (allCameras[i].enabled)
			{
				allCameras[i].enabled = false;
			}
		}
	}
}
