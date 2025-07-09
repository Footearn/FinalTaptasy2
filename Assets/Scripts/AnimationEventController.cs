using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class AnimationEventController : ObjectBase
{
	[Serializable]
	public struct AnimationActionData
	{
		public string eventName;

		public UnityEvent action;
	}

	public List<AnimationActionData> animationActionDataList;

	private Dictionary<string, UnityEvent> m_animationActionDictionary;

	private void Awake()
	{
		if (animationActionDataList != null)
		{
			m_animationActionDictionary = new Dictionary<string, UnityEvent>();
			for (int i = 0; i < animationActionDataList.Count; i++)
			{
				m_animationActionDictionary.Add(animationActionDataList[i].eventName, animationActionDataList[i].action);
			}
		}
	}

	public void playAnimationEvent(string eventName)
	{
		if (m_animationActionDictionary == null || !m_animationActionDictionary.ContainsKey(eventName))
		{
			DebugManager.LogError("NullReferenceException from AnimationEventController.playAnimationEvent()");
		}
		else
		{
			m_animationActionDictionary[eventName].Invoke();
		}
	}
}
