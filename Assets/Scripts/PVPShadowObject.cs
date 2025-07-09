using UnityEngine;

public class PVPShadowObject : ObjectBase
{
	public SpriteGroup cachedSpriteGruop;

	public float minScale = 1f;

	public Transform targetTransform;

	private float m_originYPosition;

	public void initShadow(float fixedYPosition)
	{
		m_originYPosition = fixedYPosition;
	}

	private void Update()
	{
		if (targetTransform != null)
		{
			base.cachedTransform.position = new Vector2(targetTransform.position.x, m_originYPosition);
			float num = Vector2.Distance(base.cachedTransform.position, targetTransform.position);
			cachedSpriteGruop.setAlpha((9f - Mathf.Min(num, 9f)) / 9f);
			base.cachedTransform.localScale = Vector3.one * Mathf.Clamp(num, minScale, minScale * 3f);
		}
	}
}
