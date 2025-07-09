public class CharacterSkinInfiniteScroll : InfiniteScroll
{
	public SpriteMask spriteMask;

	private CharacterManager.CharacterType m_targetCharacterType;

	private bool m_isInit;

	public void refreshAll(CharacterManager.CharacterType characterType)
	{
		refreshMaxCount(Singleton<CharacterSkinManager>.instance.sortedSkinList[characterType].Count);
		m_targetCharacterType = characterType;
		if (itemList != null && itemList.Count > 0)
		{
			foreach (ScrollSlotItem item in itemList)
			{
				item.GetComponent<CharacterSkinSlot>().UpdateItem(characterType);
			}
		}
		spriteMask.updateSprites();
	}

	private void LateUpdate()
	{
		if (itemList != null && itemList.Count > 0 && !m_isInit)
		{
			m_isInit = true;
			refreshAll(m_targetCharacterType);
		}
	}
}
