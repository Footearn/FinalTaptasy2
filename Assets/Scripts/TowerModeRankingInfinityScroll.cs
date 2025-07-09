using UnityEngine;

public class TowerModeRankingInfinityScroll : InfiniteScroll
{
	public override void resetContentPosition(Vector2 position)
	{
		base.resetContentPosition(position);
		syncAllSlotIndexFromPosition();
	}

	public override void refreshMaxCount(int slotMaxCount)
	{
		base.refreshMaxCount(slotMaxCount);
		syncAllSlotIndexFromPosition();
	}

	protected override void Update()
	{
		while (base._anchoredPosition - m_diffPreFramePosition < (0f - base.ItemScale) * 2f)
		{
			m_diffPreFramePosition -= base.ItemScale;
			ScrollSlotItem scrollSlotItem = itemList[0];
			itemList.RemoveAt(0);
			itemList.Add(scrollSlotItem);
			float num = base.ItemScale * (float)base.InstantateItemCount + base.ItemScale * (float)m_currentItemNo;
			scrollSlotItem.cachedRectTransform.anchoredPosition = ((direction != 0) ? new Vector2(num, 0f) : new Vector2(0f, 0f - num));
			int itemCount = (int)((0f - scrollSlotItem.cachedRectTransform.anchoredPosition.y) / base.ItemScale + 0.5f);
			if (scrollSlotItem.cachedRectTransform.anchoredPosition.y > 0f)
			{
				itemCount = -1;
			}
			base._ItemControllerLimited.OnUpdateItem(itemCount, scrollSlotItem);
			m_currentItemNo++;
		}
		while (base._anchoredPosition - m_diffPreFramePosition > 0f)
		{
			m_diffPreFramePosition += base.ItemScale;
			int index = base.InstantateItemCount - 1;
			ScrollSlotItem scrollSlotItem2 = itemList[index];
			itemList.RemoveAt(index);
			itemList.Insert(0, scrollSlotItem2);
			m_currentItemNo--;
			float num2 = base.ItemScale * (float)m_currentItemNo;
			scrollSlotItem2.cachedRectTransform.anchoredPosition = ((direction != 0) ? new Vector2(num2, 0f) : new Vector2(0f, 0f - num2));
			int itemCount2 = (int)((0f - scrollSlotItem2.cachedRectTransform.anchoredPosition.y) / base.ItemScale + 0.5f);
			if (scrollSlotItem2.cachedRectTransform.anchoredPosition.y > 0f)
			{
				itemCount2 = -1;
			}
			base._ItemControllerLimited.OnUpdateItem(itemCount2, scrollSlotItem2);
		}
	}
}
