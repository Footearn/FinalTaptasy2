public class ScrollSlotItem : ObjectBase
{
	public int slotIndex = -1;

	public virtual void UpdateItem(int count)
	{
		slotIndex = count;
	}

	public virtual void refreshSlot()
	{
	}
}
