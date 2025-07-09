using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TreasureCollectionSlotSet : SlotObject
{
	public TreasureCollectionCell[] cellObjects;

	public override void refreshSlot()
	{
		sortTier();
		for (int i = 0; i < cellObjects.Length; i++)
		{
			cellObjects[i].refreshCell();
		}
	}

	public void sortTier()
	{
		int num = int.Parse(Regex.Replace(base.name, "\\D", string.Empty));
		num--;
		List<TreasureManager.TreasureType> list = new List<TreasureManager.TreasureType>();
		List<TreasureManager.TreasureType> list2 = new List<TreasureManager.TreasureType>();
		for (int num2 = 52; num2 > 0; num2--)
		{
			if (Singleton<TreasureManager>.instance.getTreasureTier((TreasureManager.TreasureType)num2) == 0)
			{
				list.Add((TreasureManager.TreasureType)num2);
			}
		}
		for (int num3 = 52; num3 > 0; num3--)
		{
			if (Singleton<TreasureManager>.instance.getTreasureTier((TreasureManager.TreasureType)num3) == 1)
			{
				list.Add((TreasureManager.TreasureType)num3);
			}
		}
		for (int num4 = 52; num4 > 0; num4--)
		{
			if (Singleton<TreasureManager>.instance.getTreasureTier((TreasureManager.TreasureType)num4) == 2)
			{
				list.Add((TreasureManager.TreasureType)num4);
			}
		}
		for (int num5 = 52; num5 > 0; num5--)
		{
			if (Singleton<TreasureManager>.instance.getTreasureTier((TreasureManager.TreasureType)num5) == 3)
			{
				list.Add((TreasureManager.TreasureType)num5);
			}
		}
		for (int i = 1; i <= list.Count; i++)
		{
			if (i >= num * 5 + 1 && list2.Count < 5)
			{
				list2.Add(list[i - 1]);
			}
		}
		for (int j = 0; j < cellObjects.Length; j++)
		{
			if (list2.Count > j)
			{
				if (!cellObjects[j].cachedGameObject.activeSelf)
				{
					cellObjects[j].cachedGameObject.SetActive(true);
				}
				cellObjects[j].treasureType = list2[j];
			}
			else if (cellObjects[j].cachedGameObject.activeSelf)
			{
				cellObjects[j].cachedGameObject.SetActive(false);
			}
		}
	}
}
