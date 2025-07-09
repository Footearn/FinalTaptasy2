public class WeaponInfiniteScroll : InfiniteScroll
{
	public void refreshAll(CharacterManager.CharacterType weaponCharacterType)
	{
		foreach (ScrollSlotItem item in itemList)
		{
			item.GetComponent<WeaponSlot>().UpdateItem(weaponCharacterType);
		}
	}
}
