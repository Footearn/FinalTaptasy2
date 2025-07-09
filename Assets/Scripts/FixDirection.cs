using UnityEngine;

public class FixDirection : ObjectBase
{
	private void LateUpdate()
	{
		base.cachedTransform.localScale = new Vector2(Singleton<CharacterManager>.instance.warriorCharacter.getDirection(), 1f);
	}
}
