using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : ObjectBase
{
	public enum FrameList
	{
		FirstFrame,
		CenterFrame,
		LastFrame
	}

	public bool loop = true;

	public float duration = 0.1f;

	public bool onEnable;

	public string animationType = string.Empty;

	public string animationName = "Idle";

	public SpriteRenderer targetRenderer;

	public Image targetImage;

	[HideInInspector]
	public bool isFixed;

	public bool isPlaying;

	[HideInInspector]
	public string nextAnimationName;

	public bool isForceSetPivotOnUGUI = true;

	private int m_eventIndex;

	private int m_eventIndex2;

	private bool m_isSpriteRenderer = true;

	private Dictionary<string, List<Sprite>> m_animationSprites;

	private Action m_eventAction;

	private Action m_eventAction2;

	private Action m_endAction;

	private Sprite[] m_animations;

	private IEnumerator m_fixAnimationUpdateCoroutine;

	private IEnumerator m_animationUpdateCoroutine;

	private bool m_init;

	private void OnEnable()
	{
		if (!m_init)
		{
			init();
		}
		stopAnimation();
		if (onEnable && m_animations != null)
		{
			playAnimation(animationName, duration, loop);
		}
	}

	public void clearData()
	{
		stopAnimation();
		if (targetRenderer != null)
		{
			targetRenderer.sprite = null;
		}
		if (targetImage != null)
		{
			targetImage.sprite = null;
		}
		m_animationSprites.Clear();
		m_eventAction = null;
		m_eventAction2 = null;
		m_endAction = null;
	}

	public void init()
	{
		nextAnimationName = animationName;
		targetRenderer = GetComponent<SpriteRenderer>();
		targetImage = GetComponent<Image>();
		if (targetRenderer != null)
		{
			m_isSpriteRenderer = true;
		}
		else
		{
			m_isSpriteRenderer = false;
		}
		m_animationSprites = new Dictionary<string, List<Sprite>>();
		m_animations = null;
		m_animations = Singleton<ResourcesManager>.instance.getAnimation(animationType);
		if (m_animations == null)
		{
			return;
		}
		string text = null;
		List<Sprite> list = new List<Sprite>();
		string text2 = string.Empty;
		string empty = string.Empty;
		for (int i = 0; i < m_animations.Length; i++)
		{
			list.Add(m_animations[i]);
			empty = m_animations[i].name;
			text = empty.Substring(empty.IndexOf('-') + 1);
			int num = text.LastIndexOf('_');
			if (num > 0)
			{
				text = text.Remove(num);
			}
			if (i < m_animations.Length - 1)
			{
				text2 = m_animations[i + 1].name.Substring(m_animations[i + 1].name.IndexOf('-') + 1);
				int num2 = text2.LastIndexOf('_');
				if (num2 > 0)
				{
					text2 = text2.Remove(num2);
				}
			}
			if (text2 == text && i < m_animations.Length - 1)
			{
				continue;
			}
			List<Sprite> list2 = new List<Sprite>();
			for (int j = 0; j < list.Count; j++)
			{
				list2.Add(list[j]);
			}
			if (int.Parse(list2[0].name.Split('_')[1]) > 1)
			{
				int num3 = int.Parse(list2[0].name.Split('_')[1]) - 1;
				for (int k = 0; k < num3; k++)
				{
					list2.Insert(0, null);
				}
			}
			m_animationSprites.Add(text, list2);
			list.Clear();
		}
		m_init = true;
	}

	public void playFixAnimation(string targetAnimationName, int targetIndex)
	{
		playFixAnimation(targetAnimationName, targetIndex, 0f, null);
	}

	public void playFixAnimation(string targetAniamtionName, int targetIndex, float targetDuration)
	{
		playFixAnimation(targetAniamtionName, targetIndex, targetDuration, null);
	}

	public void playFixAnimation(string targetAnimationName, int targetIndex, float targetDuration, Action endAction)
	{
		isFixed = true;
		stopAnimation();
		animationName = targetAnimationName;
		List<Sprite> value = null;
		if (!m_animationSprites.TryGetValue(animationName, out value))
		{
			return;
		}
		if (targetRenderer != null)
		{
			targetRenderer.sprite = value[targetIndex];
		}
		else
		{
			targetImage.sprite = value[targetIndex];
			if (isForceSetPivotOnUGUI)
			{
				targetImage.rectTransform.pivot = new Vector2(value[targetIndex].pivot.x / value[targetIndex].rect.width, value[targetIndex].pivot.y / value[targetIndex].rect.height);
			}
			targetImage.SetNativeSize();
		}
		m_endAction = endAction;
		if (targetDuration > 0f)
		{
			duration = targetDuration;
			m_fixAnimationUpdateCoroutine = fixAnimationUpdate();
			StartCoroutine(m_fixAnimationUpdateCoroutine);
		}
	}

	public void playAnimation(string targetAnimationName)
	{
		playAnimation(targetAnimationName, 0.1f, true, m_eventIndex, null, m_eventIndex2, null, null);
	}

	public void playAnimation(string targetAnimationName, float targetDuration)
	{
		playAnimation(targetAnimationName, targetDuration, true, m_eventIndex, null, m_eventIndex2, null, null);
	}

	public void playAnimation(string targetAnimationName, float targetDuration, bool targetLoop)
	{
		playAnimation(targetAnimationName, targetDuration, targetLoop, m_eventIndex, null, m_eventIndex2, null, null);
	}

	public void playAnimation(string targetAnimationName, float targetDuration, bool targetLoop, Action endAction)
	{
		playAnimation(targetAnimationName, targetDuration, targetLoop, 0, null, endAction);
	}

	public void playAnimation(string targetAnimationName, float targetDuration, bool targetLoop, FrameList targetEventIndex, Action targetEventAction)
	{
		List<Sprite> value = null;
		if (m_animationSprites.TryGetValue(targetAnimationName, out value))
		{
			int targetEventIndex2 = 0;
			switch (targetEventIndex)
			{
			case FrameList.FirstFrame:
				targetEventIndex2 = 0;
				break;
			case FrameList.CenterFrame:
				targetEventIndex2 = value.Count / 2;
				break;
			case FrameList.LastFrame:
				targetEventIndex2 = value.Count - 1;
				break;
			}
			playAnimation(targetAnimationName, targetDuration, targetLoop, targetEventIndex2, targetEventAction, m_eventIndex2, null, null);
		}
	}

	public void playAnimation(string targetAnimationName, float targetDuration, bool targetLoop, FrameList targetEventIndex, Action targetEventAction, Action endAction)
	{
		List<Sprite> value = null;
		if (m_animationSprites.TryGetValue(targetAnimationName, out value))
		{
			int targetEventIndex2 = 0;
			switch (targetEventIndex)
			{
			case FrameList.FirstFrame:
				targetEventIndex2 = 0;
				break;
			case FrameList.CenterFrame:
				targetEventIndex2 = value.Count / 2;
				break;
			case FrameList.LastFrame:
				targetEventIndex2 = value.Count - 1;
				break;
			}
			playAnimation(targetAnimationName, targetDuration, targetLoop, targetEventIndex2, targetEventAction, m_eventIndex2, null, endAction);
		}
	}

	public void playAnimation(string targetAnimationName, float targetDuration, bool targetLoop, FrameList targetEventIndex, Action targetEventAction, FrameList targetEventIndex2, Action targetEventAction2, Action endAction)
	{
		List<Sprite> value = null;
		if (m_animationSprites.TryGetValue(targetAnimationName, out value))
		{
			int targetEventIndex3 = 0;
			int targetEventIndex4 = 0;
			switch (targetEventIndex)
			{
			case FrameList.FirstFrame:
				targetEventIndex3 = 0;
				break;
			case FrameList.CenterFrame:
				targetEventIndex3 = value.Count / 2;
				break;
			case FrameList.LastFrame:
				targetEventIndex3 = value.Count - 1;
				break;
			}
			switch (targetEventIndex2)
			{
			case FrameList.FirstFrame:
				targetEventIndex4 = 0;
				break;
			case FrameList.CenterFrame:
				targetEventIndex4 = value.Count / 2;
				break;
			case FrameList.LastFrame:
				targetEventIndex4 = value.Count - 1;
				break;
			}
			playAnimation(targetAnimationName, targetDuration, targetLoop, targetEventIndex3, targetEventAction, targetEventIndex4, targetEventAction2, endAction);
		}
	}

	public void playAnimation(string targetAnimationName, float targetDuration, bool targetLoop, int targetEventIndex, Action targetEventAction, Action endAction)
	{
		playAnimation(targetAnimationName, targetDuration, targetLoop, targetEventIndex, targetEventAction, m_eventIndex2, null, endAction);
	}

	public void playAnimation(string targetAnimationName, float targetDuration, bool targetLoop, int targetEventIndex, Action targetEventAction, int targetEventIndex2, Action targetEventAction2, Action endAction)
	{
		isFixed = false;
		stopAnimation();
		m_endAction = endAction;
		animationName = targetAnimationName;
		duration = targetDuration;
		loop = targetLoop;
		m_eventIndex = targetEventIndex;
		m_eventIndex2 = targetEventIndex2;
		m_eventAction = targetEventAction;
		m_eventAction2 = targetEventAction2;
		if (animationName != targetAnimationName || !isPlaying)
		{
			m_animationUpdateCoroutine = animationUpdate();
			StartCoroutine(m_animationUpdateCoroutine);
		}
	}

	public void stopAnimation()
	{
		if (m_fixAnimationUpdateCoroutine != null)
		{
			StopCoroutine(m_fixAnimationUpdateCoroutine);
			m_fixAnimationUpdateCoroutine = null;
		}
		if (m_animationUpdateCoroutine != null)
		{
			StopCoroutine(m_animationUpdateCoroutine);
			m_animationUpdateCoroutine = null;
		}
		m_endAction = null;
		isPlaying = false;
	}

	private IEnumerator fixAnimationUpdate()
	{
		float timer = 0f;
		isPlaying = true;
		while (isPlaying)
		{
			if (GameManager.isPause)
			{
				yield return null;
				continue;
			}
			timer += Time.deltaTime * GameManager.timeScale;
			if (timer >= duration)
			{
				timer = 0f;
				isPlaying = false;
				if (m_endAction != null)
				{
					m_endAction();
				}
			}
			yield return null;
		}
	}

	private IEnumerator animationUpdate()
	{
		float timer = 0f;
		int curIndex = 0;
		List<Sprite> currentSpriteList = null;
		if (m_animationSprites.TryGetValue(animationName, out currentSpriteList))
		{
			isPlaying = true;
			bool isLastFrame = false;
			if (m_isSpriteRenderer)
			{
				targetRenderer.sprite = currentSpriteList[curIndex];
			}
			else
			{
				targetImage.sprite = currentSpriteList[curIndex];
				targetImage.SetNativeSize();
			}
			while (isPlaying)
			{
				if (!GameManager.isPause || GameManager.currentDungeonType == GameManager.SpecialDungeonType.Lobby)
				{
					timer += Time.fixedDeltaTime * GameManager.timeScale;
					duration = Mathf.Max(duration, 0.002f);
					while (timer >= duration)
					{
						timer -= duration;
						if (!isLastFrame)
						{
							if (m_isSpriteRenderer)
							{
								targetRenderer.sprite = currentSpriteList[curIndex];
							}
							else
							{
								targetImage.sprite = currentSpriteList[curIndex];
								if (isForceSetPivotOnUGUI)
								{
									targetImage.rectTransform.pivot = new Vector2(currentSpriteList[curIndex].pivot.x / currentSpriteList[curIndex].rect.width, currentSpriteList[curIndex].pivot.y / currentSpriteList[curIndex].rect.height);
								}
								targetImage.SetNativeSize();
							}
							if (curIndex == m_eventIndex && m_eventAction != null)
							{
								m_eventAction();
							}
							if (curIndex == m_eventIndex2 && m_eventAction2 != null)
							{
								m_eventAction2();
							}
							if (curIndex < currentSpriteList.Count - 1)
							{
								curIndex++;
							}
							else if (loop && m_endAction == null)
							{
								curIndex = 0;
							}
							else
							{
								isLastFrame = true;
							}
						}
						else
						{
							if (loop)
							{
								curIndex = 0;
								isLastFrame = false;
							}
							else
							{
								isPlaying = false;
							}
							if (m_endAction != null)
							{
								m_endAction();
							}
						}
					}
				}
				yield return new WaitForFixedUpdate();
			}
		}
		else
		{
			yield return null;
		}
	}
}
