using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowReviveDialog : UIWindow
{
	public static UIWindowReviveDialog instance;

	public Text reviveTimerText;

	public bool isReviveByWatchAds;

	public GameObject sunburstEffectObject;

	private float m_reviveContinueTimer;

	private Action m_endGameEvent;

	private bool m_isRevived;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void startGame()
	{
		m_isRevived = false;
		isReviveByWatchAds = false;
	}

	public void endGame()
	{
		m_isRevived = false;
	}

	public void openReviveDialog(Action endGameEvent)
	{
		if (!m_isRevived)
		{
			sunburstEffectObject.SetActive(true);
			m_isRevived = true;
			m_endGameEvent = endGameEvent;
			m_reviveContinueTimer = 10f;
			reviveTimerText.text = m_reviveContinueTimer.ToString("N0");
			open();
			StopAllCoroutines();
			StartCoroutine("reviveUpdate");
		}
		else
		{
			endGameEvent();
		}
	}

	private IEnumerator reviveUpdate()
	{
		while (true)
		{
			m_reviveContinueTimer -= Time.deltaTime * GameManager.timeScale;
			reviveTimerText.text = m_reviveContinueTimer.ToString("N0");
			if (m_reviveContinueTimer <= 0f)
			{
				break;
			}
			yield return null;
		}
		m_endGameEvent();
		m_endGameEvent = null;
		close();
	}

	public void OnClickRevive()
	{
		Singleton<AdsManager>.instance.showAds("reviveBossDungeon", delegate
		{
			isReviveByWatchAds = true;
			StopAllCoroutines();
			Singleton<StatManager>.instance.revivePercentHealth = 100.0;
			Singleton<CharacterManager>.instance.warriorCharacter.revive();
			Singleton<AudioManager>.instance.playBackgroundSound("boss");
			close();
		});
	}

	public void OnClickClose()
	{
		m_endGameEvent();
		m_endGameEvent = null;
		close();
	}

	public override bool OnBeforeClose()
	{
		sunburstEffectObject.SetActive(false);
		return base.OnBeforeClose();
	}
}
