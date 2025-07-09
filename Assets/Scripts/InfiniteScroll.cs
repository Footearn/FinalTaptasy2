using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : UIObjectBase
{
	public enum Direction
	{
		Vertical,
		Horizontal
	}

	public string slotPrefabName;

	[SerializeField]
	[Range(0f, 30f)]
	private int m_instantateItemCount = 9;

	public bool isRefreshContentPositionOnEnable = true;

	public Direction direction;

	[NonSerialized]
	public List<ScrollSlotItem> itemList = new List<ScrollSlotItem>();

	protected float m_diffPreFramePosition;

	protected int m_currentItemNo;

	private ScrollSlotItem m_itemBase;

	private bool m_isInitScrollType;

	private ScrollRect m_parentScrollRect;

	private ItemControllerLimited m_itemControllerLimited;

	private RectTransform m_rectTransform;

	private float m_itemScale = -1f;

	public int InstantateItemCount
	{
		get
		{
			return m_instantateItemCount;
		}
		set
		{
			m_instantateItemCount = value;
		}
	}

	public ScrollRect parentScrollRect
	{
		get
		{
			if (m_parentScrollRect == null)
			{
				m_parentScrollRect = _RectTransform.parent.GetComponent<ScrollRect>();
			}
			return m_parentScrollRect;
		}
	}

	protected ItemControllerLimited _ItemControllerLimited
	{
		get
		{
			if (m_itemControllerLimited == null)
			{
				m_itemControllerLimited = GetComponent<ItemControllerLimited>();
			}
			return m_itemControllerLimited;
		}
	}

	protected RectTransform _RectTransform
	{
		get
		{
			if (m_rectTransform == null)
			{
				m_rectTransform = GetComponent<RectTransform>();
			}
			return m_rectTransform;
		}
	}

	private float AnchoredPosition
	{
		get
		{
			return (direction != 0) ? _RectTransform.anchoredPosition.x : (0f - _RectTransform.anchoredPosition.y);
		}
	}

	protected float _anchoredPosition
	{
		get
		{
			return AnchoredPosition;
		}
	}

	public float ItemScale
	{
		get
		{
			if (m_itemBase != null && m_itemScale == -1f)
			{
				m_itemScale = ((direction != 0) ? m_itemBase.cachedRectTransform.sizeDelta.x : m_itemBase.cachedRectTransform.sizeDelta.y);
			}
			return m_itemScale;
		}
	}

	public void setScrollType()
	{
	}

	protected override void Start()
	{
		I18NManager.changeLanguageNotification = (Action)Delegate.Combine(I18NManager.changeLanguageNotification, new Action(refreshAll));
		List<IInfiniteScrollSetup> list = (from item in GetComponents<MonoBehaviour>()
			where item is IInfiniteScrollSetup
			select item as IInfiniteScrollSetup).ToList();
		parentScrollRect.content = _RectTransform;
		m_parentScrollRect.horizontal = direction == Direction.Horizontal;
		m_parentScrollRect.vertical = direction == Direction.Vertical;
		for (int i = 0; i < m_instantateItemCount; i++)
		{
			ScrollSlotItem component = ObjectPool.Spawn(slotPrefabName, Vector2.zero).GetComponent<ScrollSlotItem>();
			if (m_itemBase == null)
			{
				m_itemBase = component;
			}
			component.cachedRectTransform.SetParent(base.transform, false);
			component.name = slotPrefabName + i;
			component.cachedRectTransform.anchoredPosition = ((direction != 0) ? new Vector2(ItemScale * (float)i, 0f) : new Vector2(0f, (0f - ItemScale) * (float)i));
			itemList.Add(component);
			foreach (IInfiniteScrollSetup item in list)
			{
				item.OnUpdateItem(i, component);
			}
		}
		foreach (IInfiniteScrollSetup item2 in list)
		{
			item2.OnPostSetupItems();
		}
	}

	private new void OnEnable()
	{
		if (isRefreshContentPositionOnEnable)
		{
			resetContentPosition(Vector2.zero);
		}
	}

	public virtual void resetContentPosition(Vector2 position)
	{
		parentScrollRect.StopMovement();
		_RectTransform.anchoredPosition = position;
	}

	protected virtual void Update()
	{
		while (AnchoredPosition - m_diffPreFramePosition < (0f - ItemScale) * 2f)
		{
			m_diffPreFramePosition -= ItemScale;
			ScrollSlotItem scrollSlotItem = itemList[0];
			itemList.RemoveAt(0);
			itemList.Add(scrollSlotItem);
			float num = ItemScale * (float)m_instantateItemCount + ItemScale * (float)m_currentItemNo;
			scrollSlotItem.cachedRectTransform.anchoredPosition = ((direction != 0) ? new Vector2(num, 0f) : new Vector2(0f, 0f - num));
			_ItemControllerLimited.OnUpdateItem(m_currentItemNo + m_instantateItemCount, scrollSlotItem);
			m_currentItemNo++;
		}
		while (AnchoredPosition - m_diffPreFramePosition > 0f)
		{
			m_diffPreFramePosition += ItemScale;
			int index = m_instantateItemCount - 1;
			ScrollSlotItem scrollSlotItem2 = itemList[index];
			itemList.RemoveAt(index);
			itemList.Insert(0, scrollSlotItem2);
			m_currentItemNo--;
			float num2 = ItemScale * (float)m_currentItemNo;
			scrollSlotItem2.cachedRectTransform.anchoredPosition = ((direction != 0) ? new Vector2(num2, 0f) : new Vector2(0f, 0f - num2));
			_ItemControllerLimited.OnUpdateItem(m_currentItemNo, scrollSlotItem2);
		}
	}

	public virtual void rearrangeSlots()
	{
		List<IInfiniteScrollSetup> list = (from item in GetComponents<MonoBehaviour>()
			where item is IInfiniteScrollSetup
			select item as IInfiniteScrollSetup).ToList();
		foreach (IInfiniteScrollSetup item in list)
		{
			item.OnPostSetupItems();
		}
	}

	public virtual void syncAllSlotIndexFromPosition()
	{
		for (int i = 0; i < itemList.Count; i++)
		{
			int num = (int)((0f - itemList[i].cachedRectTransform.anchoredPosition.y) / ItemScale + 0.5f);
			if (itemList[i].cachedRectTransform.anchoredPosition.y > 0f)
			{
				num = -1;
			}
			if (num < 0)
			{
				if (itemList[i].cachedGameObject.activeSelf)
				{
					itemList[i].cachedGameObject.SetActive(false);
				}
			}
			else if (num < m_itemControllerLimited.MaxItemCount)
			{
				if (!itemList[i].cachedGameObject.activeSelf)
				{
					itemList[i].cachedGameObject.SetActive(true);
				}
				itemList[i].UpdateItem(num);
			}
			else if (itemList[i].cachedGameObject.activeSelf)
			{
				itemList[i].cachedGameObject.SetActive(false);
			}
		}
	}

	public void refreshAll()
	{
		for (int i = 0; i < itemList.Count; i++)
		{
			itemList[i].refreshSlot();
		}
	}

	public ScrollSlotItem getSlotItem(int index)
	{
		ScrollSlotItem result = null;
		for (int i = 0; i < itemList.Count; i++)
		{
			if (itemList[i].slotIndex == index)
			{
				result = itemList[i];
				break;
			}
		}
		return result;
	}

	public ScrollSlotItem getNextSlotItem(int index)
	{
		ScrollSlotItem result = null;
		for (int i = 0; i < itemList.Count; i++)
		{
			if (itemList[i].slotIndex == index + 1)
			{
				result = itemList[i];
			}
		}
		return result;
	}

	public virtual void refreshMaxCount(int slotMaxCount)
	{
		_ItemControllerLimited.MaxItemCount = slotMaxCount;
		List<IInfiniteScrollSetup> list = (from item in GetComponents<MonoBehaviour>()
			where item is IInfiniteScrollSetup
			select item as IInfiniteScrollSetup).ToList();
		foreach (IInfiniteScrollSetup item in list)
		{
			item.OnPostSetupItems();
		}
	}
}
