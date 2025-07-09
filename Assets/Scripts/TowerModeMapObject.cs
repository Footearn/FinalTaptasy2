using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerModeMapObject : ObjectBase
{
	public GameObject leftDoor;

	public GameObject rightDoor;

	public Transform leftPointTransform;

	public Transform rightPointTransform;

	public Transform centerPointTransform;

	public int curFloor = -1;

	public bool isApexMap;

	public bool isMiniBossMap;

	public bool isBossMap;

	public SpriteAnimation gateSpriteAnimation;

	public GameObject undergroundObject;

	public SpriteGroup cachedSpriteGroup;

	public List<TowerModeFlameObject> currentFlameObjects = new List<TowerModeFlameObject>();

	public List<TowerModeMonsterObject> currentMiniBosses = new List<TowerModeMonsterObject>();

	public TowerModeBossObject currentBossObject;

	public MovingObject.Direction direction;

	public GameObject bestRecordObject;

	public FadeChangableObject bestRecordFadeObject;

	public SpriteRenderer bestRecordTextSpriteRenderer;

	private Material m_bestRecordTextMaterial;

	public bool isGateOpend;

	public bool isBestFloor;

	public Transform getPoint(bool isInpoint)
	{
		Transform transform = null;
		if (direction == MovingObject.Direction.Left)
		{
			if (isInpoint)
			{
				return rightPointTransform;
			}
			return leftPointTransform;
		}
		if (isInpoint)
		{
			return leftPointTransform;
		}
		return rightPointTransform;
	}

	public void initTowerModeMap(MovingObject.Direction dir, int floor, bool isApex, bool isMiniBoss, bool isBoss)
	{
		isGateOpend = false;
		if (gateSpriteAnimation != null)
		{
			gateSpriteAnimation.playFixAnimation("TowerGate", 0);
		}
		if (bestRecordTextSpriteRenderer != null && m_bestRecordTextMaterial == null)
		{
			m_bestRecordTextMaterial = bestRecordTextSpriteRenderer.material;
		}
		int num = 0;
		num = ((Singleton<TowerModeManager>.instance.currentDifficultyType != TowerModeManager.TowerModeDifficultyType.TimeAttack) ? Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor : Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor);
		if (!isApex && floor == num)
		{
			isBestFloor = true;
			bestRecordFadeObject.setAlpha(1f);
			if (!bestRecordObject.activeSelf)
			{
				bestRecordObject.SetActive(true);
			}
		}
		else
		{
			isBestFloor = false;
			if (bestRecordObject != null && bestRecordObject.activeSelf)
			{
				bestRecordObject.SetActive(false);
			}
		}
		direction = dir;
		curFloor = floor;
		bool flag = direction == MovingObject.Direction.Left;
		leftDoor.SetActive(flag);
		rightDoor.SetActive(!flag);
		isApexMap = isApex;
		isMiniBossMap = isMiniBoss;
		isBossMap = isBoss;
		cachedSpriteGroup.setAlpha(1f);
		if (undergroundObject != null)
		{
			if (floor == 1)
			{
				undergroundObject.SetActive(true);
			}
			else
			{
				undergroundObject.SetActive(false);
			}
		}
	}

	public void recycleMap()
	{
		for (int i = 0; i < currentFlameObjects.Count; i++)
		{
			currentFlameObjects[i].recycleFlame();
		}
		currentFlameObjects.Clear();
		if (currentBossObject != null)
		{
			currentBossObject.recycleMonster(false);
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}

	public void openGate()
	{
		if (isApexMap)
		{
			gateSpriteAnimation.playAnimation("TowerGate", 0.1f, false, delegate
			{
				isGateOpend = true;
				StopCoroutine("waitForOpenGate");
				StartCoroutine("waitForOpenGate");
			});
		}
	}

	private IEnumerator waitForOpenGate()
	{
		yield return new WaitForSeconds(1f);
		Singleton<TowerModeManager>.instance.curPlayingCharacter.setStateLock(false);
		Singleton<TowerModeManager>.instance.curPlayingCharacter.setState(PublicDataManager.State.Move);
	}

	public void closeBestFloorObject()
	{
		if (isBestFloor && bestRecordFadeObject != null)
		{
			bestRecordFadeObject.fadeIn(3f, delegate
			{
				bestRecordObject.SetActive(false);
			});
		}
	}

	private void LateUpdate()
	{
		if (!(m_bestRecordTextMaterial == null) && isBestFloor)
		{
			Vector2 mainTextureOffset = m_bestRecordTextMaterial.mainTextureOffset;
			mainTextureOffset.x += Time.deltaTime * GameManager.timeScale * 0.3f;
			m_bestRecordTextMaterial.mainTextureOffset = mainTextureOffset;
		}
	}
}
