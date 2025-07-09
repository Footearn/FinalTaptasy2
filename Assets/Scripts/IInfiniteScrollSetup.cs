public interface IInfiniteScrollSetup
{
	void OnPostSetupItems();

	void OnUpdateItem(int itemCount, ScrollSlotItem obj);
}
