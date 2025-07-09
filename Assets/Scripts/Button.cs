using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : UnityEngine.UI.Button, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerClickHandler
{
	[Serializable]
	public class SimpleWindowControl
	{
		public string windowName;

		public bool open;
	}

	public bool isDisableSFX;

	protected Color defaultColor;

	public SimpleWindowControl[] windowControl;

	public Action<PointerEventData> OnClickTrigger;

	public UnityEvent onButtonDownEvent;

	public RectTransform rectTransform;

	protected override void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		if (base.targetGraphic != null)
		{
			defaultColor = base.targetGraphic.color;
		}
		base.Awake();
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		if (base.interactable && !isDisableSFX)
		{
			Singleton<AudioManager>.instance.playEffectSound("btn_default");
		}
		if (base.animator != null && base.interactable)
		{
			base.animator.SetBool("Pressed", true);
		}
		if (onButtonDownEvent != null)
		{
			onButtonDownEvent.Invoke();
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (base.animator != null && base.interactable)
		{
			base.animator.SetBool("Pressed", false);
		}
		base.OnPointerUp(eventData);
	}

	protected override void OnDisable()
	{
		if (Application.isPlaying)
		{
			base.OnDisable();
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (OnClickTrigger != null)
		{
			OnClickTrigger(eventData);
			return;
		}
		base.OnPointerClick(eventData);
		for (int i = 0; i < windowControl.Length; i++)
		{
			string windowName = windowControl[i].windowName;
			bool open = windowControl[i].open;
			if (string.IsNullOrEmpty(windowName))
			{
				DebugManager.LogError("Button.windowControl[" + i + "].windowName !!NotSet!!");
				DebugManager.LogError(this);
				Debug.Break();
				continue;
			}
			UIWindow uIWindow = UIWindow.Get(windowName);
			if (uIWindow != null)
			{
				if (open)
				{
					if (!uIWindow.isOpen)
					{
						uIWindow.open();
					}
				}
				else if (uIWindow.isOpen)
				{
					uIWindow.close();
				}
			}
			else
			{
				DebugManager.LogError("UIWindow.Get(\"" + windowName + "\") == null");
				DebugManager.LogError(this);
				Debug.Break();
			}
		}
	}
}
