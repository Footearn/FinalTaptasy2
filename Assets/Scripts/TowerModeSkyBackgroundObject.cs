using UnityEngine;

public class TowerModeSkyBackgroundObject : ObjectBase
{
	public SpriteRenderer cachedSpriteRenderer;

	public float speedMultiply = 1f;

	private Material m_cachedMaterialOfBackground;

	private Vector2 m_currentBackgroundOffset;

	private float m_distanceBetweenTopAndBottom;

	private void Awake()
	{
		m_cachedMaterialOfBackground = cachedSpriteRenderer.material;
	}

	private void Start()
	{
		m_distanceBetweenTopAndBottom = Singleton<CachedManager>.instance.ingameCamera.ViewportToWorldPoint(new Vector2(0.5f, 1f)).y - Singleton<CachedManager>.instance.ingameCamera.ViewportToWorldPoint(new Vector2(0.5f, 0f)).y;
	}

	private void LateUpdate()
	{
		m_currentBackgroundOffset.y = CameraFollow.instance.cachedTransform.position.y / (m_distanceBetweenTopAndBottom * speedMultiply);
		m_cachedMaterialOfBackground.mainTextureOffset = m_currentBackgroundOffset;
	}
}
