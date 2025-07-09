using System;
using System.Collections.Generic;
using UnityEngine;

public class PostBoxManager : Singleton<PostBoxManager>
{
	public class PostboxItem
	{
		public string itemCode;

		public ItemType itemType;

		public string itemId;

		public double itemQuantity;

		public double itemRemianSec;
	}

	[Serializable]
	public struct PostBoxItemSpriteData
	{
		public ItemType currentItemType;

		public Sprite iconSprite;
	}

	public Sprite defaultPostItemSpriteIcon;

	[NonSerialized]
	public List<PostboxItem> m_postboxItemList;

	public List<PostBoxItemSpriteData> iconSpriteList;

	private void Awake()
	{
		if (m_postboxItemList == null)
		{
			m_postboxItemList = new List<PostboxItem>();
		}
	}

	public void Clear()
	{
		m_postboxItemList.Clear();
	}

	public int GetPostboxItemCount()
	{
		return m_postboxItemList.Count;
	}

	public void Add(string id, string code, ItemType itemType, string count, double sec)
	{
		if (itemType != ItemType.None)
		{
			PostboxItem postboxItem = new PostboxItem();
			postboxItem.itemId = id;
			postboxItem.itemCode = code;
			postboxItem.itemRemianSec = sec;
			postboxItem.itemType = itemType;
			postboxItem.itemQuantity = Singleton<NanooAPIManager>.instance.GetItemQuantityByCode(code);
			m_postboxItemList.Add(postboxItem);
		}
	}

	public PostboxItem GetItem(int index)
	{
		if (index < 0 || index >= m_postboxItemList.Count)
		{
			return null;
		}
		return m_postboxItemList[index];
	}

	public Sprite getItemIconSprite(ItemType itemType)
	{
		Sprite sprite = null;
		for (int i = 0; i < iconSpriteList.Count; i++)
		{
			if (iconSpriteList[i].currentItemType == itemType)
			{
				sprite = iconSpriteList[i].iconSprite;
			}
		}
		if (sprite == null)
		{
			sprite = defaultPostItemSpriteIcon;
		}
		return sprite;
	}

	public void RemoveItem(int index)
	{
		if (index >= 0 && index < m_postboxItemList.Count)
		{
			m_postboxItemList.RemoveAt(index);
			UIWindowPostBox.instance.initPostItemSlot(true);
		}
	}
}
