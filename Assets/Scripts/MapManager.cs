using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
	private const float m_heightOfTunnel = 1.934f;

	private const float m_minibossGenOffset = 1.5f;

	private const float m_minibossStageOffset = -3.46f;

	private const float m_minibossOffset = -5.03f;

	private const float m_bossStageOffset = -4.206f;

	private const float m_bossOffset = -6.576f;

	private const float m_princessOffset = -6.31f;

	private const float m_jailOffset = -6.576f;

	public const int firstCraetedFloor = 4;

	public static string prefixStage;

	public int maxMonsterCount = 7;

	public int maxStage = 10;

	private bool m_nextIsRight;

	private Vector2 m_firstGenLinePosition = new Vector2(0f, -3.62f);

	private Vector2 m_firstGenWallPosition = new Vector2(0f, -2.676f);

	private float m_thisGenY;

	private float m_thisGenWallY;

	private Vector2 m_firstGenEnemyPosition = new Vector2(3.1f, 0.472f);

	private float m_betweenEnemiesX = 0.95f;

	private float m_thisEnemyX;

	private Vector2 m_firstGenFogPosition = new Vector2(0f, -11.483f);

	private float m_currentFogSpawnYPos;

	private MovingObject.Direction m_thisEnemyDirection = MovingObject.Direction.Right;

	private Queue<CreatedMapData> m_createdMapQueue;

	private int m_recentFloor;

	private int m_minibossCount;

	private float m_warningTime = 10f;

	private void Awake()
	{
		m_createdMapQueue = new Queue<CreatedMapData>();
	}

	public void reset()
	{
		m_thisGenY = m_firstGenLinePosition.y - 1.934f;
		m_thisGenWallY = m_firstGenWallPosition.y - 1.934f;
		m_thisEnemyX = m_firstGenEnemyPosition.x - 1.5f;
		m_minibossCount = 0;
		m_currentFogSpawnYPos = m_firstGenFogPosition.y - 1.934f;
		m_thisEnemyDirection = MovingObject.Direction.Right;
		m_nextIsRight = false;
		m_recentFloor = 1;
	}

	public void loadStageData()
	{
		prefixStage = "Stage" + GameManager.getRealThemeNumber(GameManager.currentTheme);
		m_warningTime = 10f;
		reset();
	}

	public void startGame()
	{
		if (Singleton<DataManager>.instance.currentGameData.currentTheme <= 100)
		{
			maxMonsterCount = 5;
		}
		else
		{
			maxMonsterCount = 7;
		}
		EnemyManager.bossStageFloor = getMaxFloor(GameManager.currentTheme);
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon)
		{
			for (int i = 0; i <= 4; i++)
			{
				if (i != 0)
				{
					createGroundBelow(i);
				}
			}
		}
		else if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			Singleton<BossRaidManager>.instance.startBossRaid();
		}
	}

	public void resetQueue()
	{
		m_createdMapQueue.Clear();
	}

	public void createGroundBelow(int floor)
	{
		if (floor > EnemyManager.bossStageFloor)
		{
			return;
		}
		if (floor == EnemyManager.bossStageFloor)
		{
			if (GameManager.currentStage % maxStage == 0)
			{
				createBossStage(floor, true);
				return;
			}
			if (GameManager.currentTheme % 10 == 0)
			{
				createBossStage(floor, false);
				return;
			}
			createMiniBossStage(floor);
			fillMiniBossStage(floor);
		}
		else if (GameManager.currentTheme > 200)
		{
			if (floor == EnemyManager.miniBossFloorForOverStages201)
			{
				createMiniBossStage(floor);
				fillMiniBossStage(floor, true);
			}
			else
			{
				createGround(floor);
				fillNextEnemies();
			}
		}
		else
		{
			createGround(floor);
			fillNextEnemies();
		}
	}

	public void createGround(int floor)
	{
		LineBackground component = ObjectPool.Spawn("@Line", new Vector2(m_firstGenWallPosition.x, m_thisGenWallY), Singleton<CachedManager>.instance.backgroundTransform).GetComponent<LineBackground>();
		component.Set(floor - 1);
		m_thisGenWallY -= 1.934f;
		Ground component2;
		if (m_nextIsRight)
		{
			component2 = ObjectPool.Spawn("@EmptyRightLine", new Vector2(m_firstGenLinePosition.x, m_thisGenY), Singleton<CachedManager>.instance.emptyRightLinesTransform).GetComponent<Ground>();
			m_thisGenY -= 1.934f;
			m_nextIsRight = false;
		}
		else
		{
			component2 = ObjectPool.Spawn("@EmptyLeftLine", new Vector2(m_firstGenLinePosition.x, m_thisGenY), Singleton<CachedManager>.instance.emptyLeftLinesTransform).GetComponent<Ground>();
			m_thisGenY -= 1.934f;
			m_nextIsRight = true;
		}
		if (floor == 1)
		{
			component2.stairRenderer.gameObject.SetActive(false);
		}
		else
		{
			component2.stairRenderer.gameObject.SetActive(true);
			component2.stairRenderer.sprite = Singleton<ResourcesManager>.instance.getAnimation(prefixStage + "Stair")[0];
		}
		if (component2.shadowObject != null)
		{
			component2.shadowObject.SetActive(true);
		}
		component2.floorRender.sprite = Singleton<ResourcesManager>.instance.getAnimation(prefixStage + "Block_ground")[0];
		if (GameManager.currentTheme > 200)
		{
			switch (GameManager.getRealThemeNumber(GameManager.currentTheme))
			{
			case 1:
			case 2:
				component2.stairRenderer.color = Util.getCalculatedColor(169f, 155f, 255f);
				component2.floorRender.color = Util.getCalculatedColor(169f, 155f, 255f);
				break;
			case 5:
				component2.stairRenderer.color = Util.getCalculatedColor(197f, 210f, 246f);
				component2.floorRender.color = Util.getCalculatedColor(197f, 210f, 246f);
				break;
			case 8:
				component2.stairRenderer.color = Util.getCalculatedColor(255f, 180f, 180f);
				component2.floorRender.color = Util.getCalculatedColor(255f, 180f, 180f);
				break;
			case 10:
				component2.stairRenderer.color = Util.getCalculatedColor(236f, 226f, 255f);
				component2.floorRender.color = Util.getCalculatedColor(236f, 226f, 255f);
				break;
			}
		}
		else
		{
			component2.stairRenderer.color = Color.white;
			component2.floorRender.color = Color.white;
		}
		Singleton<GroundManager>.instance.addGround(component2, floor);
		m_createdMapQueue.Enqueue(new CreatedMapData(component.cachedGameObject, component2.cachedGameObject, Singleton<GroundManager>.instance.getTorchPos(component.transform, component.bgType)));
	}

	public void createMiniBossStage(int floor)
	{
		m_minibossCount++;
		float y = -3.46f + (0f - (float)(m_minibossCount - 1) * 1.5f) + (float)(-floor) * 1.934f;
		m_thisGenWallY -= 3.434f;
		m_thisGenY -= 3.434f;
		m_currentFogSpawnYPos -= 3.434f;
		BossGround component = ObjectPool.Spawn("@MiniBossStage", new Vector3(0f, y, 0f), Singleton<CachedManager>.instance.backgroundTransform).GetComponent<BossGround>();
		component.init(m_nextIsRight, floor == EnemyManager.bossStageFloor, floor);
		m_nextIsRight = !m_nextIsRight;
		for (int i = 0; i < component.emptyBossLine.Length; i++)
		{
			if (component.emptyBossLine[i].shadowObject != null)
			{
				component.emptyBossLine[i].shadowObject.SetActive(true);
			}
			if (component.emptyBossLine[i].floorTextObject != null)
			{
				component.emptyBossLine[i].floorTextObject.SetActive(true);
			}
		}
		m_createdMapQueue.Enqueue(new CreatedMapData(null, component.cachedGameObject, Singleton<GroundManager>.instance.getTorchPos(component.cachedTransform, component.isBoss)));
	}

	public void createBossStage(int floor, bool isLastBoss)
	{
		m_minibossCount++;
		float num = 0f - (float)(m_minibossCount - 1) * 1.5f + (float)(-floor) * 1.934f;
		float y = -4.206f + num;
		BossGround component = ObjectPool.Spawn("@BossStage", new Vector3(0f, y, 0f), Singleton<CachedManager>.instance.backgroundTransform).GetComponent<BossGround>();
		component.init(m_nextIsRight, floor == EnemyManager.bossStageFloor, floor);
		float y2 = -6.576f + num;
		float y3 = -6.31f + num;
		float y4 = -6.576f + num;
		int num2 = ((m_thisEnemyDirection != 0) ? 1 : (-1));
		float num3 = ((floor != EnemyManager.bossStageFloor) ? (-2.5f) : (-6f));
		if (Singleton<DataManager>.instance.currentGameData.bestTheme == GameManager.currentTheme)
		{
			if (isLastBoss)
			{
				GameObject gameObject = ObjectPool.Spawn("@LastPrincess", new Vector3(-2.8f * (float)num2, y3, 0f), Singleton<CachedManager>.instance.ingameBackgroundTransform);
				GameObject gameObject2 = ObjectPool.Spawn("@Jail", new Vector3(-2.8f * (float)num2, y4, 0f), Singleton<CachedManager>.instance.ingameBackgroundTransform);
				Singleton<CachedManager>.instance.princess = gameObject.GetComponent<LastPrincess>();
				Singleton<CachedManager>.instance.princess.cachedTransform.localScale = Vector3.one;
				Singleton<CachedManager>.instance.princess.spriteAnimation.playFixAnimation("Idle", 0);
				Singleton<CachedManager>.instance.princess.spriteAnimation.targetRenderer.sortingOrder = -2;
				Singleton<CachedManager>.instance.princess.setDirection(m_thisEnemyDirection);
				Singleton<CachedManager>.instance.jail = gameObject2.GetComponent<Jail>();
				Singleton<CachedManager>.instance.jail.DoorControl(false);
			}
			else if (GameManager.getRealThemeNumber(GameManager.currentTheme) != 10)
			{
				num3 = -2.5f;
			}
		}
		Singleton<EnemyManager>.instance.spawnBoss(Singleton<EnemyManager>.instance.getBossTypeByTheme(GameManager.currentTheme, GameManager.currentStage), new Vector3(num3 * (float)num2, y2, 0f), m_thisEnemyDirection, Singleton<EnemyManager>.instance.getCurrentMonsterLevel(), true);
		m_recentFloor++;
		m_createdMapQueue.Enqueue(new CreatedMapData(null, component.gameObject, Singleton<GroundManager>.instance.getTorchPos(component.cachedTransform, component.isBoss)));
	}

	public void fillMiniBossStage(int floor, bool isOverStages201 = false)
	{
		float y = Singleton<GroundManager>.instance.currentGroundList[floor].cachedTransform.position.y + 0.2f;
		m_thisEnemyX = ((m_thisEnemyDirection != 0) ? (m_firstGenEnemyPosition.x - 1.5f) : m_firstGenEnemyPosition.x);
		int num = ((m_thisEnemyDirection != 0) ? 1 : (-1));
		int num2 = ((m_thisEnemyDirection != 0) ? 4 : 0);
		for (int i = 0; i < maxMonsterCount; i++)
		{
			if (i == num2)
			{
				if (floor == EnemyManager.bossStageFloor || (GameManager.currentTheme > 200 && floor == EnemyManager.miniBossFloorForOverStages201))
				{
					Singleton<EnemyManager>.instance.spawnMiniBoss(Singleton<EnemyManager>.instance.getMiniBossTypeByTheme(GameManager.currentTheme, GameManager.currentStage), new Vector3(-2.8f * (float)num, y, 0f), m_thisEnemyDirection, Singleton<EnemyManager>.instance.getCurrentMonsterLevel(), !isOverStages201);
				}
			}
			else
			{
				Singleton<EnemyManager>.instance.spawnMonster(Singleton<EnemyManager>.instance.getMonsterTypeByTheme(GameManager.currentTheme), new Vector2(m_thisEnemyX, y), m_thisEnemyDirection, Singleton<EnemyManager>.instance.getCurrentMonsterLevel());
			}
			m_betweenEnemiesX = 5.7f / (float)maxMonsterCount;
			m_thisEnemyX -= m_betweenEnemiesX - 0.15f;
		}
		m_thisEnemyX = m_firstGenEnemyPosition.x;
		m_recentFloor++;
		m_thisEnemyDirection = ((m_thisEnemyDirection == MovingObject.Direction.Left) ? MovingObject.Direction.Right : MovingObject.Direction.Left);
	}

	public void deleteGroundAbove()
	{
		CreatedMapData createdMapData = m_createdMapQueue.Dequeue();
		for (int i = 0; i < createdMapData.torches.Count; i++)
		{
			ObjectPool.Recycle(createdMapData.torches[i].name, createdMapData.torches[i]);
		}
		if (createdMapData.wall != null)
		{
			ObjectPool.Recycle(createdMapData.wall.name, createdMapData.wall);
		}
		ObjectPool.Recycle(createdMapData.line.name, createdMapData.line);
	}

	public void fillNextEnemies()
	{
		float y = Singleton<GroundManager>.instance.currentGroundList[m_recentFloor].cachedTransform.position.y + 0.2f;
		m_thisEnemyX = ((m_thisEnemyDirection != 0) ? (m_firstGenEnemyPosition.x - 1.5f) : m_firstGenEnemyPosition.x);
		m_betweenEnemiesX = 5.7f / (float)maxMonsterCount;
		for (int i = 0; i < maxMonsterCount; i++)
		{
			Singleton<EnemyManager>.instance.spawnMonster(Singleton<EnemyManager>.instance.getMonsterTypeByTheme(GameManager.currentTheme), new Vector2(m_thisEnemyX, y), m_thisEnemyDirection, Singleton<EnemyManager>.instance.getCurrentMonsterLevel());
			m_thisEnemyX -= m_betweenEnemiesX;
		}
		m_recentFloor++;
		m_thisEnemyDirection = ((m_thisEnemyDirection == MovingObject.Direction.Left) ? MovingObject.Direction.Right : MovingObject.Direction.Left);
	}

	public int getMaxFloor(long theme)
	{
		int num = 0;
		return Mathf.Min((int)(5f + Mathf.Max(theme - 1, 0f) / 3f), 25);
	}
}
