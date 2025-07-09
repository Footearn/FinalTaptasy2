using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptimizedScrollRect : ScrollRect
{
	public enum Direction
	{
		Vertical,
		Horizontal
	}

	public bool isRefreshOnOnEnable = true;

	public bool isScrolling;

	public List<SlotObject> slotObjects;

	public bool isUsingFixedScroll = true;

	public float maxCellScale = 2f;

	public float minCellScale = 1f;

	public float sensingRange = 220f;

	public float fixScrollforce = 5.5f;

	public float intervalBetweenCell = 220f;

	public float offset;

	public SlotObject selectedSlot;

	public Transform centerTransform;

	public Vector2 centerPosition;

	public Direction direction;

	private RectTransform m_scrollRectRectTransform;

	private bool isStartScroll;

	private bool m_isEndInit;

	private bool m_isInitScrollType;

	private float timeForce = 1f;

	protected override void Awake()
	{
		base.Awake();
		m_isEndInit = true;
		isScrolling = false;
		m_scrollRectRectTransform = GetComponent<RectTransform>();
		I18NManager.changeLanguageNotification = (Action)Delegate.Combine(I18NManager.changeLanguageNotification, new Action(changeLanguageNotification));
		base.horizontal = direction == Direction.Horizontal;
		base.vertical = direction == Direction.Vertical;
	}

	public void setScrollType()
	{
	}

	public void disableSlot(SlotObject targetSlot)
	{
		if (targetSlot.cachedChildrenRectTransform == null)
		{
			targetSlot.cachedChildrenRectTransform = targetSlot.cachedTransform.GetChild(0).GetComponent<RectTransform>();
		}
		targetSlot.cachedGameObject.SetActive(false);
		targetSlot.isDisabled = true;
		targetSlot.cachedChildrenRectTransform.SetParent(Singleton<CachedManager>.instance.dummyTransform);
	}

	public void enableSlot(SlotObject targetSlot)
	{
		if (targetSlot.cachedChildrenRectTransform == null)
		{
			targetSlot.cachedChildrenRectTransform = targetSlot.cachedTransform.GetChild(0).GetComponent<RectTransform>();
		}
		targetSlot.cachedGameObject.SetActive(true);
		targetSlot.isDisabled = false;
		targetSlot.cachedChildrenRectTransform.SetParent(targetSlot.cachedTransform);
		if (targetSlot.cachedChildrenRectTransform.localScale != Vector3.one)
		{
			targetSlot.cachedChildrenRectTransform.localScale = Vector3.one;
		}
		targetSlot.cachedChildrenRectTransform.anchoredPosition = Vector3.zero;
		Vector3 localPosition = targetSlot.cachedChildrenRectTransform.localPosition;
		localPosition.z = 0f;
		targetSlot.cachedChildrenRectTransform.localPosition = localPosition;
	}

	private void changeLanguageNotification()
	{
		refreshSlots();
	}

	protected override void OnEnable()
	{
		if (!m_isEndInit || !Singleton<DataManager>.instance.isLoadedData)
		{
			return;
		}
		base.content.anchoredPosition = Vector2.zero;
		if (Application.isPlaying)
		{
			if (isRefreshOnOnEnable)
			{
				refreshSlots();
			}
			isScrolling = false;
			StopAllCoroutines();
			if (isUsingFixedScroll)
			{
				StartCoroutine("fixedScrollUpdata");
			}
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (base.content != null)
		{
			base.content.anchoredPosition = Vector2.zero;
		}
	}

	public virtual void refreshSlots()
	{
		for (int i = 0; i < slotObjects.Count; i++)
		{
			slotObjects[i].refreshSlot();
		}
	}

	public virtual void selectSlot(SlotObject currentSlot)
	{
		selectedSlot = currentSlot;
		for (int i = 0; i < slotObjects.Count; i++)
		{
			slotObjects[i].refreshSlot();
		}
	}

	protected virtual void scrollTo(int cellIndex)
	{
		Vector2 anchoredPosition = base.content.anchoredPosition;
		anchoredPosition.x = 0f - (offset + (float)cellIndex * intervalBetweenCell);
		base.content.anchoredPosition = anchoredPosition;
	}

	private IEnumerator fixedScrollUpdata()
	{
		while (true)
		{
			if (!isScrolling)
			{
				if (selectedSlot != null)
				{
					timeForce += Time.deltaTime * 0.5f;
					Vector2 contentAnchoredPosition2 = base.content.anchoredPosition;
					contentAnchoredPosition2 = Vector2.Lerp(contentAnchoredPosition2, new Vector2(0f - intervalBetweenCell * (float)selectedSlot.slotIndex, 0f), Time.deltaTime * GameManager.timeScale * fixScrollforce * timeForce);
					base.content.anchoredPosition = contentAnchoredPosition2;
				}
			}
			else
			{
				timeForce = 1f;
			}
			yield return null;
		}
	}

	private IEnumerator fixStartScrollUpdate()
	{
		while (!isScrolling && !isStartScroll)
		{
			base.content.anchoredPosition = Vector2.zero;
			yield return null;
		}
		isStartScroll = true;
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		isScrolling = true;
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		isScrolling = false;
	}
}
