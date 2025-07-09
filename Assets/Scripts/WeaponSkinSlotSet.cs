using System.Collections.Generic;

public class WeaponSkinSlotSet : ScrollSlotItem
{
	public List<WeaponSkinSlot> totalSlotList;

	private List<WeaponSkinData> m_currentWeaponSkinDataList;

	public override void UpdateItem(int count)
	{
		base.UpdateItem(count);
		m_currentWeaponSkinDataList = UIWindowWeaponSkin.instance.sortedSkinDictionary[UIWindowWeaponSkin.instance.currentTabCharacterType][slotIndex];
		refreshSlotSet();
	}

	public void refreshSlotSet()
	{
		for (int i = 0; i < totalSlotList.Count; i++)
		{
			if (i < m_currentWeaponSkinDataList.Count)
			{
				totalSlotList[i].initWeaponSkinSlot(m_currentWeaponSkinDataList[i]);
			}
			else
			{
				totalSlotList[i].initWeaponSkinSlot(null);
			}
		}
		refreshNewIcon();
	}

	public void refreshNewIcon()
	{
		for (int i = 0; i < totalSlotList.Count; i++)
		{
			totalSlotList[i].refreshNewIcon();
		}
	}
}
