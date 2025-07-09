using UnityEngine;

public class ElopeBackgroundObject : ObjectBase
{
	public SpriteRenderer[] backgroundSpriteRenderers;

	private Material[] m_cachedMaterialOfBackgrounds;

	private Vector2[] m_currentBackgroundOffsetArray = new Vector2[3]
	{
		Vector2.zero,
		Vector2.zero,
		Vector2.zero
	};

	public float[] m_currentBackgroundOffsetMultiplyValue;

	private Vector3 m_prevFrameCameraPosition;

	private void Awake()
	{
		m_cachedMaterialOfBackgrounds = new Material[backgroundSpriteRenderers.Length];
		for (int i = 0; i < backgroundSpriteRenderers.Length; i++)
		{
			m_cachedMaterialOfBackgrounds[i] = backgroundSpriteRenderers[i].material;
		}
	}

	public void initBackground()
	{
		m_prevFrameCameraPosition = CameraFollow.instance.cachedTransform.position;
	}

	public void changeBackground(int backgroundIndex)
	{
		backgroundSpriteRenderers[0].sprite = Singleton<ElopeModeManager>.instance.backgroundSpriteData[backgroundIndex].mainBackgroundSprite;
		backgroundSpriteRenderers[1].sprite = Singleton<ElopeModeManager>.instance.backgroundSpriteData[backgroundIndex].middleBackgroundSprite;
		backgroundSpriteRenderers[2].sprite = Singleton<ElopeModeManager>.instance.backgroundSpriteData[backgroundIndex].frontBackgroundSprite;
	}

	private void LateUpdate()
	{
		if (Singleton<ElopeModeManager>.instance.currentElopeModeDaemonKing != null)
		{
			for (int i = 0; i < m_cachedMaterialOfBackgrounds.Length; i++)
			{
				m_currentBackgroundOffsetArray[i].x = CameraFollow.instance.cachedTransform.position.x / (12.28f * m_currentBackgroundOffsetMultiplyValue[i]);
				m_cachedMaterialOfBackgrounds[i].mainTextureOffset = m_currentBackgroundOffsetArray[i];
			}
		}
	}
}
