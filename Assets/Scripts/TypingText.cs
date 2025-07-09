using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TypingText : ObjectBase
{
	public enum State
	{
		Type,
		Up
	}

	public enum SpeakerType
	{
		None,
		Princess,
		Warrior,
		Priest,
		Archer,
		Boss
	}

	public enum BalloonType
	{
		Default,
		Heroes,
		Heart
	}

	public SpeakerType speakerType;

	public RectTransform rectTransform;

	public Text textMessage;

	public float messageIndex;

	public float prevY;

	public float y;

	public float timer;

	public float lineCount = 1f;

	public float prevIndex;

	public bool withoutPrefix;

	public bool settingComplete;

	private float m_height;

	private string m_prefixSpeaker = string.Empty;

	private string m_message = string.Empty;

	private float m_speed = 20f;

	private int m_delayActionCount;

	private Action<int> m_messageDelayAction;

	private Action m_messageEndAction;

	private float m_delay = 1f;

	public bool m_immediate;

	private GameObject m_balloon;

	private BalloonType m_balloonType;

	public string text
	{
		set
		{
			m_message = value;
			messageIndex = 0f;
			prevY = 0f;
			y = 0f;
			timer = 0f;
			lineCount = 1f;
			prevIndex = 0f;
			textMessage.text = string.Empty;
			if (base.cachedGameObject.activeInHierarchy && base.cachedGameObject.activeSelf)
			{
				StartCoroutine("Typing");
			}
			else
			{
				ForceSetText(m_message);
			}
		}
	}

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	private void OnEnable()
	{
		if (rectTransform.localScale != Vector3.one)
		{
			rectTransform.localScale = Vector3.one;
		}
		if (!settingComplete)
		{
			RectTransform component = textMessage.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0.5f, 0.5f);
			component.anchorMax = new Vector2(0.5f, 0.5f);
			component.pivot = new Vector2(0f, 1f);
			component.sizeDelta = new Vector2(100f, 100f);
			textMessage.fontSize = 30;
			textMessage.alignment = TextAnchor.MiddleRight;
			textMessage.horizontalOverflow = HorizontalWrapMode.Overflow;
			textMessage.verticalOverflow = VerticalWrapMode.Overflow;
			textMessage.resizeTextForBestFit = false;
			textMessage.color = Color.white;
			lineCount = 0f;
		}
	}

	public void ForceSetText(string text)
	{
		StopCoroutine("Typing");
		textMessage.text = text;
	}

	public void SetText(string text, float speed, Action action)
	{
		SetText(SpeakerType.None, "white", text, speed, action);
	}

	public void SetText(SpeakerType speaker, string text, float speed, Action endAction)
	{
		SetText(speaker, "white", text, speed, endAction);
	}

	public void SetText(SpeakerType speaker, string color, string text, float speed, Action endAction)
	{
		speakerType = speaker;
		setPrefixSpeaker(speaker, color);
		this.text = text;
		m_speed = speed;
		m_messageEndAction = endAction;
		m_delay = 1f;
		m_immediate = false;
	}

	public void SetText(SpeakerType speaker, string color, string text, float speed, float delay, Action endAction)
	{
		speakerType = speaker;
		setPrefixSpeaker(speaker, color);
		this.text = text;
		m_speed = speed;
		m_messageEndAction = endAction;
		m_delay = delay;
		m_immediate = false;
	}

	public void SetTextImmediate(string text, float speed, float delay, Action endAction)
	{
		SetTextImmediate(SpeakerType.None, "white", text, speed, delay, endAction);
	}

	public void SetTextImmediate(SpeakerType speaker, string color, string text, float speed, float delay, Action endAction)
	{
		speakerType = speaker;
		setPrefixSpeaker(speaker, color);
		this.text = text;
		m_speed = speed;
		m_messageEndAction = endAction;
		m_delay = delay;
		m_immediate = true;
	}

	public void SetTextDelay(string text, float speed, float delay, Action<int> delayAction, Action endAction)
	{
		SetTextDelay(SpeakerType.None, "white", text, speed, delay, delayAction, endAction);
	}

	public void SetTextDelay(SpeakerType speaker, string color, string text, float speed, float delay, Action<int> delayAction, Action endAction)
	{
		speakerType = speaker;
		setPrefixSpeaker(speaker, color);
		this.text = text;
		m_speed = speed;
		m_delayActionCount = 0;
		m_messageDelayAction = delayAction;
		m_messageEndAction = endAction;
		m_delay = delay;
		m_immediate = false;
	}

	public void SetTextwithBalloon(SpeakerType speaker, string color, string text, float speed, float delay, Action endAction, Vector2 pos, Transform parent, BalloonType type = BalloonType.Default)
	{
		speakerType = speaker;
		setPrefixSpeaker(speaker, color);
		this.text = text;
		m_speed = speed;
		m_messageEndAction = endAction;
		m_delay = delay;
		m_balloonType = type;
		m_immediate = false;
		switch (type)
		{
		case BalloonType.Heroes:
			m_balloon = ObjectPool.Spawn("LongTextBalloon", pos, parent);
			break;
		case BalloonType.Heart:
			m_balloon = ObjectPool.Spawn("HeartBalloon", pos, parent);
			break;
		default:
			m_balloon = ObjectPool.Spawn("TextBalloon", pos, parent);
			break;
		}
	}

	private IEnumerator heightUpdate()
	{
		m_height = textMessage.preferredHeight;
		while (true)
		{
			lineCount = textMessage.preferredHeight / m_height;
			yield return null;
		}
	}

	private IEnumerator Typing()
	{
		StartCoroutine("heightUpdate");
		while (true)
		{
			if (!string.IsNullOrEmpty(m_message))
			{
				messageIndex += Time.deltaTime * m_speed;
				messageIndex = Mathf.Min(m_message.Length, messageIndex);
				if ((int)prevIndex != (int)messageIndex)
				{
					prevIndex = Mathf.Min(m_message.Length, (int)messageIndex);
					if (m_immediate)
					{
						messageIndex = m_message.Length;
						if (speakerType != 0 && !withoutPrefix)
						{
							textMessage.text = m_prefixSpeaker + " : " + m_message;
						}
						else
						{
							textMessage.text = m_message;
						}
					}
					else
					{
						int currentIndex = Mathf.Clamp((int)messageIndex - 1, 0, m_message.Length - 1);
						char currentChar = m_message[currentIndex];
						if (currentChar == '[')
						{
							int regexEndIdx = 0;
							for (int i = 0; i < m_message.Length; i++)
							{
								if (m_message[i] == ']')
								{
									regexEndIdx = i;
									break;
								}
							}
							float delay = float.Parse(m_message.Substring(currentIndex + 1, regexEndIdx - currentIndex - 1));
							m_message = m_message.Replace(m_message.Substring(currentIndex, regexEndIdx - currentIndex + 1), string.Empty);
							messageIndex += 1f;
							yield return new WaitForSeconds(delay);
							m_messageDelayAction(m_delayActionCount);
							m_delayActionCount++;
						}
						if (speakerType != 0 && currentChar != ' ')
						{
							Singleton<AudioManager>.instance.playEffectSound("talk_loop");
						}
						if (speakerType != 0 && !withoutPrefix)
						{
							textMessage.text = m_prefixSpeaker + " : " + m_message.Substring(0, (int)prevIndex);
						}
						else
						{
							textMessage.text = m_message.Substring(0, (int)prevIndex);
						}
					}
					if ((float)m_message.Length == messageIndex)
					{
						break;
					}
				}
			}
			yield return null;
		}
		yield return new WaitForSeconds(m_delay);
		if (m_messageEndAction != null)
		{
			m_messageEndAction();
		}
		if (m_balloon != null)
		{
			switch (m_balloonType)
			{
			case BalloonType.Heroes:
				ObjectPool.Recycle("LongTextBalloon", m_balloon);
				break;
			case BalloonType.Heart:
				ObjectPool.Recycle("HeartBalloon", m_balloon);
				break;
			default:
				ObjectPool.Recycle("TextBalloon", m_balloon);
				break;
			}
		}
		StopCoroutine("heightUpdate");
	}

	private void setPrefixSpeaker(SpeakerType speaker, string color)
	{
		switch (speaker)
		{
		case SpeakerType.Boss:
			if (GameManager.getRealThemeNumber(GameManager.currentTheme - 1) != 10)
			{
				m_prefixSpeaker = "<color=" + color + ">" + I18NManager.Get(((EnemyManager.BossType)(GameManager.getRealThemeNumber(GameManager.currentTheme) - 1)).ToString().ToUpper() + "_NAME") + "</color>";
			}
			else
			{
				m_prefixSpeaker = "<color=" + color + ">" + I18NManager.Get(EnemyManager.BossType.Daemon1.ToString().ToUpper() + "_NAME") + "</color>";
			}
			break;
		case SpeakerType.Warrior:
			m_prefixSpeaker = "<color=" + color + ">" + I18NManager.Get("WARRIOR_SKIN_NAME_" + (int)(Singleton<CharacterSkinManager>.instance.warriorEquippedSkinData.skinType + 1)) + "</color>";
			break;
		case SpeakerType.Priest:
			m_prefixSpeaker = "<color=" + color + ">" + I18NManager.Get("PRIEST_SKIN_NAME_" + (int)(Singleton<CharacterSkinManager>.instance.priestEquippedSkinData.skinType + 1)) + "</color>";
			break;
		case SpeakerType.Archer:
			m_prefixSpeaker = "<color=" + color + ">" + I18NManager.Get("ARCHER_SKIN_NAME_" + (int)(Singleton<CharacterSkinManager>.instance.archerEquippedSkinData.skinType + 1)) + "</color>";
			break;
		case SpeakerType.Princess:
			m_prefixSpeaker = "<color=" + color + ">" + I18NManager.Get("PRINCESS") + "</color>";
			break;
		}
	}
}
