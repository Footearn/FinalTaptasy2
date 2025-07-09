using UnityEngine;

public class CameraFollow : ObjectBase
{
	public static CameraFollow instance;

	public Vector2 minClampPosition;

	public Vector2 maxClampPosition;

	public Transform targetTransform;

	public Vector2 offset;

	public Vector2 offsetForElopeMode;

	public float cameraLerpSpeedForElopeMode = 4f;

	public Vector3 offsetForTowerMode;

	public float cameraLerpSpeedForTowerMode = 4f;

	private CharacterObject m_targetCharacter;

	private void Awake()
	{
		instance = this;
	}

	public void startGame()
	{
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon)
		{
			maxClampPosition.y = -4.8f;
		}
		else if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.TowerMode)
		{
			float num = 0f;
			int num2 = Singleton<TowerModeManager>.instance.getTotalFloor(Singleton<TowerModeManager>.instance.currentDifficultyType) / 5;
			num = (float)(Singleton<TowerModeManager>.instance.getTotalFloor(Singleton<TowerModeManager>.instance.currentDifficultyType) - num2) * Singleton<TowerModeManager>.instance.intervalBetweenNormalMap + (float)num2 * Singleton<TowerModeManager>.instance.intervalBetweenMiniBossMap - 2.5f;
			minClampPosition.y = 0f;
			maxClampPosition.y = num;
		}
	}

	private void LateUpdate()
	{
		Vector2 vector;
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode && targetTransform != null)
		{
			Vector2 a = targetTransform.position;
			vector = base.cachedTransform.position;
			vector = Vector2.Lerp(base.cachedTransform.position, a + offsetForElopeMode, Time.deltaTime * cameraLerpSpeedForElopeMode);
			vector.y = -4.056f;
			base.cachedTransform.position = vector;
			return;
		}
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.TowerMode && Singleton<TowerModeManager>.instance.curPlayingCharacter != null)
		{
			vector = base.cachedTransform.position;
			vector = Vector2.Lerp(base.cachedTransform.position, Singleton<TowerModeManager>.instance.curPlayingCharacter.cachedTransform.position + offsetForTowerMode, Time.deltaTime * cameraLerpSpeedForTowerMode);
			vector.x = Mathf.Clamp(vector.x, minClampPosition.x, maxClampPosition.x);
			vector.y = Mathf.Clamp(vector.y, minClampPosition.y, maxClampPosition.y);
			base.cachedTransform.position = vector;
			return;
		}
		m_targetCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		if (m_targetCharacter != null)
		{
			Vector2 a = m_targetCharacter.cachedTransform.position;
			vector = base.cachedTransform.position;
			if (GameManager.currentGameState != 0)
			{
				vector = new Vector2(0f, -4.8f);
			}
			else
			{
				vector = ((!Singleton<GameManager>.instance.isThemeClearEvent) ? Vector2.Lerp(vector, a + offset, Time.deltaTime * 4f) : (a + offset));
				vector.x = Mathf.Clamp(vector.x, minClampPosition.x, maxClampPosition.x);
				vector.y = Mathf.Min(vector.y, maxClampPosition.y);
			}
			base.cachedTransform.position = vector;
		}
	}
}
