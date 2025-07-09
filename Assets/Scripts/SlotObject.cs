using System.Collections;
using UnityEngine;

public class SlotObject : ObjectBase
{
	public OptimizedScrollRect parentScrollRect;

	public int slotIndex;

	public Transform targetImageTransform;

	public bool isDisabled;

	public RectTransform cachedChildrenRectTransform;

	protected virtual void Reset()
	{
		targetImageTransform = base.transform.GetChild(0);
	}

	protected virtual void Awake()
	{
		if (cachedChildrenRectTransform == null)
		{
			cachedChildrenRectTransform = base.cachedTransform.GetChild(0).GetComponent<RectTransform>();
		}
	}

	public virtual void initSlot()
	{
		if (parentScrollRect != null && parentScrollRect.isUsingFixedScroll)
		{
			StopAllCoroutines();
			StartCoroutine("checkDistanceUpdate");
		}
	}

	public virtual void refreshSlot()
	{
	}

	private IEnumerator checkDistanceUpdate()
	{
		while (true)
		{
			Vector2 position = base.cachedTransform.position;
			float widthRatio = (float)Screen.width / 555f;
			float dist2 = Mathf.Abs(parentScrollRect.centerPosition.x - position.x);
			dist2 /= widthRatio;
			float scale = Mathf.Max((1f - dist2 / parentScrollRect.sensingRange) * parentScrollRect.maxCellScale, parentScrollRect.minCellScale);
			if (parentScrollRect.selectedSlot == null && parentScrollRect.selectedSlot != this)
			{
				parentScrollRect.selectSlot(this);
			}
			if (scale > parentScrollRect.minCellScale && parentScrollRect.selectedSlot != this)
			{
				float distOfSelectedSlot2 = Mathf.Abs(parentScrollRect.centerPosition.x - parentScrollRect.selectedSlot.cachedTransform.position.x);
				distOfSelectedSlot2 /= widthRatio;
				if (distOfSelectedSlot2 > dist2)
				{
					parentScrollRect.selectSlot(this);
				}
			}
			targetImageTransform.localScale = new Vector2(scale, scale);
			yield return null;
		}
	}
}
