using UnityEngine;

public class UISorter : MonoBehaviour
{
	public string sortingLayer;

	public Renderer cachedRenderer;

	private string m_sortingLayerApplied;

	public int orderInLayer;

	public int prevOder;

	private void Update()
	{
		if (m_sortingLayerApplied != sortingLayer)
		{
			m_sortingLayerApplied = sortingLayer;
			cachedRenderer.sortingLayerName = sortingLayer;
		}
		if (prevOder != orderInLayer)
		{
			cachedRenderer.sortingOrder = orderInLayer;
			prevOder = orderInLayer;
		}
		base.enabled = false;
	}
}
