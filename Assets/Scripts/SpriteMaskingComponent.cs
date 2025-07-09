using UnityEngine;
using UnityEngine.Rendering;

public class SpriteMaskingComponent : MonoBehaviour
{
	public SpriteMask owner;

	public virtual bool isEnabled
	{
		get
		{
			return !(owner == null) && owner.enabled;
		}
	}

	protected virtual void doUpdateSprite(Transform t, CompareFunction comp, int stencil)
	{
		if (owner != null)
		{
			owner.doUpdateSprite(t, comp, stencil);
		}
	}
}
