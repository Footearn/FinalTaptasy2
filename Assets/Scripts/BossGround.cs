using UnityEngine;

public class BossGround : ObjectBase
{
	public Ground[] emptyBossLine;

	public GameObject bossWallObject;

	public SpriteRenderer background;

	public SpriteRenderer underground;

	public bool isBoss;

	public void init(bool nextRight, bool realBoss, int floor)
	{
		bossWallObject.SetActive(realBoss);
		emptyBossLine[0].isBossGround = (emptyBossLine[1].isBossGround = realBoss);
		if (nextRight)
		{
			Singleton<GroundManager>.instance.addGround(emptyBossLine[0], floor);
			emptyBossLine[1].cachedGameObject.SetActive(false);
			emptyBossLine[0].cachedGameObject.SetActive(true);
			if (floor == 1)
			{
				emptyBossLine[0].stairRenderer.gameObject.SetActive(false);
			}
			else
			{
				emptyBossLine[0].stairRenderer.gameObject.SetActive(true);
				emptyBossLine[0].stairRenderer.sprite = Singleton<ResourcesManager>.instance.getAnimation(MapManager.prefixStage + "Stair")[(!isBoss) ? 1 : 2];
			}
			emptyBossLine[0].floorRender.sprite = Singleton<ResourcesManager>.instance.getAnimation(MapManager.prefixStage + "Block_ground")[0];
		}
		else
		{
			Singleton<GroundManager>.instance.addGround(emptyBossLine[1], floor);
			emptyBossLine[0].cachedGameObject.SetActive(false);
			emptyBossLine[1].cachedGameObject.SetActive(true);
			if (floor == 1)
			{
				emptyBossLine[1].stairRenderer.gameObject.SetActive(false);
			}
			else
			{
				emptyBossLine[1].stairRenderer.gameObject.SetActive(true);
				emptyBossLine[1].stairRenderer.sprite = Singleton<ResourcesManager>.instance.getAnimation(MapManager.prefixStage + "Stair")[(!isBoss) ? 1 : 2];
			}
			emptyBossLine[1].floorRender.sprite = Singleton<ResourcesManager>.instance.getAnimation(MapManager.prefixStage + "Block_ground")[0];
		}
		if (isBoss)
		{
			background.sprite = Singleton<ResourcesManager>.instance.getAnimation(MapManager.prefixStage + "Boss")[0];
		}
		else
		{
			background.sprite = Singleton<ResourcesManager>.instance.getAnimation(MapManager.prefixStage + "Boss")[1];
		}
		underground.sprite = Singleton<ResourcesManager>.instance.getAnimation(MapManager.prefixStage + "Boss")[2];
		if (GameManager.currentTheme > 200)
		{
			switch (GameManager.getRealThemeNumber(GameManager.currentTheme))
			{
			case 1:
			case 2:
				background.color = Util.getCalculatedColor(169f, 155f, 255f);
				underground.color = Util.getCalculatedColor(169f, 155f, 255f);
				emptyBossLine[0].stairRenderer.color = Util.getCalculatedColor(169f, 155f, 255f);
				emptyBossLine[1].stairRenderer.color = Util.getCalculatedColor(169f, 155f, 255f);
				emptyBossLine[0].floorRender.color = Util.getCalculatedColor(169f, 155f, 255f);
				emptyBossLine[1].floorRender.color = Util.getCalculatedColor(169f, 155f, 255f);
				break;
			case 5:
				background.color = Util.getCalculatedColor(197f, 210f, 246f);
				underground.color = Util.getCalculatedColor(197f, 210f, 246f);
				emptyBossLine[0].stairRenderer.color = Util.getCalculatedColor(197f, 210f, 246f);
				emptyBossLine[1].stairRenderer.color = Util.getCalculatedColor(197f, 210f, 246f);
				emptyBossLine[0].floorRender.color = Util.getCalculatedColor(197f, 210f, 246f);
				emptyBossLine[1].floorRender.color = Util.getCalculatedColor(197f, 210f, 246f);
				break;
			case 8:
				background.color = Util.getCalculatedColor(255f, 180f, 180f);
				underground.color = Util.getCalculatedColor(255f, 180f, 180f);
				emptyBossLine[0].stairRenderer.color = Util.getCalculatedColor(255f, 180f, 180f);
				emptyBossLine[1].stairRenderer.color = Util.getCalculatedColor(255f, 180f, 180f);
				emptyBossLine[0].floorRender.color = Util.getCalculatedColor(255f, 180f, 180f);
				emptyBossLine[1].floorRender.color = Util.getCalculatedColor(255f, 180f, 180f);
				break;
			case 10:
				background.color = Util.getCalculatedColor(236f, 226f, 255f);
				underground.color = Util.getCalculatedColor(236f, 226f, 255f);
				emptyBossLine[0].stairRenderer.color = Util.getCalculatedColor(236f, 226f, 255f);
				emptyBossLine[1].stairRenderer.color = Util.getCalculatedColor(236f, 226f, 255f);
				emptyBossLine[0].floorRender.color = Util.getCalculatedColor(236f, 226f, 255f);
				emptyBossLine[1].floorRender.color = Util.getCalculatedColor(236f, 226f, 255f);
				break;
			case 3:
			case 4:
			case 6:
			case 7:
			case 9:
				break;
			}
		}
		else
		{
			background.color = Color.white;
			underground.color = Color.white;
			emptyBossLine[0].stairRenderer.color = Color.white;
			emptyBossLine[1].stairRenderer.color = Color.white;
			emptyBossLine[0].floorRender.color = Color.white;
			emptyBossLine[1].floorRender.color = Color.white;
		}
	}
}
