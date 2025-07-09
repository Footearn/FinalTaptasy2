using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteScroll))]
public class ItemControllerLimited : UIBehaviour, IInfiniteScrollSetup
{
	[SerializeField]
	[Range(1f, 99999f)]
	private int max = 30;

	private InfiniteScroll _infiniteScroll;

	private RectTransform _cachedRectTransform;

	public int MaxItemCount
	{
		get
		{
			return max;
		}
		set
		{
			max = value;
		}
	}

	private InfiniteScroll m_infiniteScroll
	{
		get
		{
			if (_infiniteScroll == null)
			{
				_infiniteScroll = GetComponent<InfiniteScroll>();
			}
			return _infiniteScroll;
		}
	}

	private RectTransform m_cachedRectTransform
	{
		get
		{
			if (_cachedRectTransform == null)
			{
				_cachedRectTransform = GetComponent<RectTransform>();
			}
			return _cachedRectTransform;
		}
	}

	public void OnPostSetupItems()
	{
		m_infiniteScroll.parentScrollRect.movementType = ScrollRect.MovementType.Elastic;
		Vector2 sizeDelta = m_cachedRectTransform.sizeDelta;
		if (m_infiniteScroll.direction == InfiniteScroll.Direction.Vertical)
		{
			sizeDelta.y = m_infiniteScroll.ItemScale * (float)max;
		}
		else
		{
			sizeDelta.x = m_infiniteScroll.ItemScale * (float)max;
		}
		m_cachedRectTransform.sizeDelta = sizeDelta;
	}

	public void OnUpdateItem(int itemCount, ScrollSlotItem obj)
	{
		if (itemCount < 0 || itemCount >= max)
		{
			if (obj.cachedGameObject.activeSelf)
			{
				obj.cachedGameObject.SetActive(false);
			}
			return;
		}
		if (!obj.cachedGameObject.activeSelf)
		{
			obj.cachedGameObject.SetActive(true);
		}
		obj.UpdateItem(itemCount);
	}
}
