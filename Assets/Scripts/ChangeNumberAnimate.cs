using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNumberAnimate : ObjectBase
{
	public enum PrintType
	{
		Nomal,
		Percentage,
		Number,
		ChangeUnit
	}

	public enum PlayMode
	{
		Auto,
		Manual
	}

	private double prevValue;

	private double currentValue;

	private double nextValue;

	private bool m_isChangeValue;

	private float m_gameFinissIncreaseNumberAnimationTimer;

	private float m_gameFinissIncreaseNumberAnimationTime = 0.7f;

	public string FrontStr;

	public string BackStr;

	private float m_prefixPlaySpeed = 1f;

	public PrintType CurrentPrintType;

	public PlayMode m_playMode;

	private bool m_isPlaying;

	public Action doneAction;

	public TextMesh textmeshUI;

	public Text textUI;

	private double m_prevPrintValue = double.MinValue;

	public double CurrentValue
	{
		get
		{
			return currentValue;
		}
	}

	public double NextValue
	{
		get
		{
			return nextValue;
		}
	}

	public bool IsChangeValue
	{
		get
		{
			return m_isChangeValue;
		}
	}

	public float GameFinissIncreaseNumberAnimationTime
	{
		get
		{
			return m_gameFinissIncreaseNumberAnimationTime;
		}
		set
		{
			m_gameFinissIncreaseNumberAnimationTime = value;
		}
	}

	public float PrefixPlaySpeed
	{
		set
		{
			m_prefixPlaySpeed = value;
		}
	}

	public bool IsPlaying
	{
		get
		{
			return m_isPlaying;
		}
	}

	public void SetPrintType(PrintType type)
	{
		CurrentPrintType = type;
	}

	private void Update()
	{
		if (!m_isPlaying || !m_isChangeValue)
		{
			return;
		}
		if (m_gameFinissIncreaseNumberAnimationTimer == 0f)
		{
			m_gameFinissIncreaseNumberAnimationTimer += Time.deltaTime * m_prefixPlaySpeed;
		}
		else if (m_gameFinissIncreaseNumberAnimationTimer < m_gameFinissIncreaseNumberAnimationTime)
		{
			m_gameFinissIncreaseNumberAnimationTimer += Time.deltaTime * m_prefixPlaySpeed;
			float num = m_gameFinissIncreaseNumberAnimationTimer / m_gameFinissIncreaseNumberAnimationTime;
			if (num > 1f)
			{
				num = 1f;
			}
			currentValue = prevValue + (nextValue - prevValue) * (double)num;
			_SetLabelText();
		}
		else
		{
			currentValue = nextValue;
			_SetLabelText();
			m_isChangeValue = false;
			m_isPlaying = false;
			if (doneAction != null)
			{
				doneAction();
			}
		}
	}

	public void Play()
	{
		if (m_isChangeValue)
		{
			m_isPlaying = true;
			base.gameObject.SetActive(true);
		}
	}

	public void Stop()
	{
		m_isPlaying = false;
	}

	public void SetText(double value)
	{
		nextValue = value;
		prevValue = currentValue;
		currentValue = nextValue;
		_SetLabelText();
		m_isChangeValue = false;
		m_isPlaying = false;
	}

	public void SetValue(double value, float time, Action action)
	{
		doneAction = action;
		SetValue(value, time);
	}

	public void SetValue(double value, Action action)
	{
		doneAction = action;
		SetValue(value);
	}

	public void SetValue(double value)
	{
		if (m_isChangeValue ? (nextValue != value) : (currentValue != value))
		{
			nextValue = value;
			prevValue = currentValue;
			m_isChangeValue = true;
			m_gameFinissIncreaseNumberAnimationTimer = 0f;
			if (m_playMode == PlayMode.Auto)
			{
				m_isPlaying = true;
			}
		}
		else if (doneAction != null)
		{
			doneAction();
		}
	}

	private IEnumerator waitForStartTime(double value, float time, float startTime)
	{
		yield return new WaitForSeconds(startTime);
		if (m_isChangeValue ? (nextValue != value) : (currentValue != value))
		{
			nextValue = value;
			prevValue = currentValue;
			m_isChangeValue = true;
			m_gameFinissIncreaseNumberAnimationTimer = 0f;
			m_gameFinissIncreaseNumberAnimationTime = time;
			if (m_playMode == PlayMode.Auto)
			{
				m_isPlaying = true;
			}
		}
	}

	public void SetValue(double value, float time, float startTime)
	{
		if (base.cachedGameObject.activeInHierarchy && base.cachedGameObject.activeSelf)
		{
			StartCoroutine(waitForStartTime(value, time, startTime));
		}
		else if (m_isChangeValue ? (nextValue != value) : (currentValue != value))
		{
			nextValue = value;
			prevValue = currentValue;
			m_isChangeValue = true;
			m_gameFinissIncreaseNumberAnimationTimer = 0f;
			m_gameFinissIncreaseNumberAnimationTime = time;
			if (m_playMode == PlayMode.Auto)
			{
				m_isPlaying = true;
			}
		}
	}

	public void SetValue(double value, float time)
	{
		if (m_isChangeValue ? (nextValue != value) : (currentValue != value))
		{
			nextValue = value;
			prevValue = currentValue;
			m_isChangeValue = true;
			m_gameFinissIncreaseNumberAnimationTimer = 0f;
			m_gameFinissIncreaseNumberAnimationTime = time;
			if (m_playMode == PlayMode.Auto)
			{
				m_isPlaying = true;
			}
		}
	}

	public void SetAnimationTime(float time)
	{
		m_gameFinissIncreaseNumberAnimationTime = time;
	}

	public void Reset()
	{
		prevValue = 0.0;
		currentValue = 0.0;
		m_isChangeValue = false;
		textmeshUI = GetComponent<TextMesh>();
		textUI = GetComponent<Text>();
	}

	private void _SetLabelText()
	{
		if (m_prevPrintValue != currentValue)
		{
			string text = string.Empty;
			switch (CurrentPrintType)
			{
			case PrintType.Nomal:
				text = ((string.IsNullOrEmpty(FrontStr) && string.IsNullOrEmpty(BackStr)) ? currentValue.ToString() : string.Format("{0}{1}{2}", FrontStr, (int)currentValue, BackStr));
				break;
			case PrintType.Number:
				text = ((string.IsNullOrEmpty(FrontStr) && string.IsNullOrEmpty(BackStr)) ? currentValue.ToString("N0") : string.Format("{0}{1:N0}{2}", FrontStr, (int)currentValue, BackStr));
				break;
			case PrintType.Percentage:
				text = ((string.IsNullOrEmpty(FrontStr) && string.IsNullOrEmpty(BackStr)) ? currentValue.ToString("P1") : string.Format("{0}{1:P1}{2}", FrontStr, (int)currentValue, BackStr));
				break;
			case PrintType.ChangeUnit:
				text = ((string.IsNullOrEmpty(FrontStr) && string.IsNullOrEmpty(BackStr)) ? GameManager.changeUnit((float)currentValue) : (FrontStr + GameManager.changeUnit((float)currentValue) + BackStr));
				break;
			}
			m_prevPrintValue = currentValue;
			if (textmeshUI != null)
			{
				textmeshUI.text = text;
			}
			else if (text != null)
			{
				textUI.text = text;
			}
		}
	}
}
