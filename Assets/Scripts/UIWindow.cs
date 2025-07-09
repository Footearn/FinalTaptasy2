using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindow : ObjectBase
{
	protected static Dictionary<string, UIWindow> windows = new Dictionary<string, UIWindow>();

	protected static List<UIWindow> openedWindows = new List<UIWindow>();

	private static int topSortingOrder = 0;

	public string windowName;

	public bool isCanCloseESC = true;

	public bool visibility;

	[HideInInspector]
	public Canvas cachedCanvas;

	[HideInInspector]
	public GraphicRaycaster cachedRaycaster;

	public Animator cachedAnimator;

	public bool isOpen;

	public bool ignoreOnCloseAll;

	protected Button[] m_buttonChildrens;

	protected float width;

	protected bool m_isClosing;

	public virtual void OnAfterActiveGameObject()
	{
	}

	public virtual bool OnBeforeOpen()
	{
		return true;
	}

	public virtual void OnAfterOpen()
	{
	}

	public virtual bool OnBeforeClose()
	{
		return true;
	}

	public virtual void OnAfterClose()
	{
	}

	public static UIWindow[] CloseAll()
	{
		UIWindow[] array = openedWindows.ToArray();
		for (int num = array.Length - 1; num >= 0; num--)
		{
			if (array[num] != null && !array[num].ignoreOnCloseAll)
			{
				array[num].close();
			}
		}
		return array;
	}

	public static UIWindow Get(string name)
	{
		UIWindow result = null;
		if (windows.ContainsKey(name))
		{
			result = windows[name];
		}
		return result;
	}

	public virtual void Awake()
	{
		if (cachedCanvas == null)
		{
			cachedCanvas = GetComponent<Canvas>();
		}
		width = 2000f;
		m_isClosing = false;
		m_buttonChildrens = base.transform.GetComponentsInChildren<Button>(true);
		cachedCanvas.pixelPerfect = false;
		if (string.IsNullOrEmpty(windowName))
		{
			DebugManager.LogError(base.gameObject);
			throw new Exception("WindowName cannot be null or empty.");
		}
		if (windows.ContainsKey(windowName))
		{
			throw new Exception("WindowName(" + base.name + ", " + windowName + ") already registered!");
		}
		if (!string.IsNullOrEmpty(windowName))
		{
			windows.Add(windowName, this);
		}
		if (!visibility)
		{
			isOpen = false;
			for (int i = 0; i < m_buttonChildrens.Length; i++)
			{
				m_buttonChildrens[i].enabled = false;
			}
			base.cachedGameObject.SetActive(false);
			cachedCanvas.enabled = false;
			if (cachedRaycaster != null)
			{
				cachedRaycaster.enabled = false;
			}
			if (cachedAnimator != null)
			{
				cachedAnimator.enabled = false;
			}
		}
		else
		{
			StartCoroutine("waitForAwake");
		}
	}

	private IEnumerator waitForAwake()
	{
		yield return null;
		if (visibility)
		{
			for (int i = 0; i < m_buttonChildrens.Length; i++)
			{
				m_buttonChildrens[i].enabled = true;
			}
			OnBeforeOpen();
			OnAfterActiveGameObject();
			OnAfterOpen();
			isOpen = true;
			openedWindows.Add(this);
		}
	}

	private void OnDestroy()
	{
		if (!string.IsNullOrEmpty(windowName))
		{
			windows.Remove(windowName);
		}
	}

	public static UIWindow GetTopWindow()
	{
		UIWindow result = null;
		if (openedWindows.Count > 0)
		{
			result = openedWindows[openedWindows.Count - 1];
		}
		return result;
	}

	public static int GetNextSortingOrder()
	{
		topSortingOrder++;
		return topSortingOrder;
	}

	public virtual void open(bool forceOpen = false)
	{
		if (isOpen && !forceOpen)
		{
			return;
		}
		CancelInvoke("afterClose");
		if (!OnBeforeOpen())
		{
			return;
		}
		if (m_buttonChildrens != null)
		{
			for (int i = 0; i < m_buttonChildrens.Length; i++)
			{
				m_buttonChildrens[i].enabled = true;
			}
		}
		isOpen = true;
		if (cachedAnimator != null)
		{
			cachedAnimator.enabled = true;
		}
		base.cachedGameObject.SetActive(true);
		OnAfterActiveGameObject();
		cachedCanvas.enabled = true;
		if (cachedRaycaster != null)
		{
			cachedRaycaster.enabled = true;
		}
		CancelInvoke("afterOpen");
		Invoke("afterOpen", 0.15f);
		if (cachedAnimator != null)
		{
			cachedAnimator.SetTrigger("Open");
		}
		if (openedWindows.Contains(this))
		{
			openedWindows.Remove(this);
		}
		openedWindows.Add(this);
		if (!(cachedCanvas != null) || cachedCanvas.sortingOrder > 0)
		{
		}
		visibility = true;
	}

	public void setTop()
	{
		cachedCanvas.sortingOrder = GetNextSortingOrder();
	}

	public void DelayClose(float delay)
	{
		StartCoroutine(_delayClose(delay));
	}

	private IEnumerator _delayClose(float delay)
	{
		yield return new WaitForSeconds(delay);
		close();
	}

	public virtual void close()
	{
		if (isOpen && OnBeforeClose())
		{
			m_isClosing = true;
			for (int i = 0; i < m_buttonChildrens.Length; i++)
			{
				m_buttonChildrens[i].enabled = false;
			}
			isOpen = false;
			CancelInvoke("afterClose");
			CancelInvoke("afterOpen");
			if (openedWindows.Contains(this))
			{
				openedWindows.Remove(this);
			}
			if (cachedAnimator != null)
			{
				cachedAnimator.SetTrigger("Close");
				Invoke("afterClose", 0.5f);
			}
			else
			{
				afterClose();
			}
			visibility = false;
		}
	}

	public void closeAll()
	{
		CloseAll();
	}

	private void afterOpen()
	{
		OnAfterOpen();
	}

	private void afterClose()
	{
		if (!isOpen)
		{
			OnAfterClose();
			m_isClosing = false;
			base.cachedGameObject.SetActive(false);
			cachedCanvas.enabled = false;
			if (cachedRaycaster != null)
			{
				cachedRaycaster.enabled = false;
			}
			if (cachedAnimator != null)
			{
				cachedAnimator.enabled = false;
			}
		}
	}
}
