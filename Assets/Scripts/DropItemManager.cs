using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : Singleton<DropItemManager>
{
	public enum DropItemType
	{
		None = -1,
		Gold,
		Ruby,
		TreasureKey,
		TranscendStone,
		CollectEventResource,
		ElopeResource1,
		ElopeResource2,
		ElopeResource3,
		ElopeResource4,
		HeartCoin,
		WeaponSkinPiece,
		Length
	}

	public static float dropItemDissapearTime = 1f;

	public float dropItemCatchRange = 1.5f;

	public List<DropObject> currentDropItemList;

	public Dictionary<DropItemType, double> totalIngameGainDropItem = new Dictionary<DropItemType, double>();

	private List<DropObject> m_cachedDynamicList;

	private void Start()
	{
		for (int i = 0; i < 11; i++)
		{
			totalIngameGainDropItem.Add((DropItemType)i, 0.0);
		}
		m_cachedDynamicList = new List<DropObject>();
		currentDropItemList = new List<DropObject>();
	}

	public void startGame()
	{
		for (int i = 0; i < 11; i++)
		{
			totalIngameGainDropItem[(DropItemType)i] = 0.0;
		}
		StopCoroutine("dropItemCatchUpdate");
		clearList();
		StartCoroutine("dropItemCatchUpdate");
	}

	public void endGame()
	{
		StopCoroutine("dropItemCatchUpdate");
		collectAllItems();
		clearList();
	}

	private void clearList()
	{
		currentDropItemList.Clear();
	}

	public void collectAllItems()
	{
		for (int i = 0; i < currentDropItemList.Count; i++)
		{
			currentDropItemList[i].obtainEvent();
		}
	}

	public void spawnDropItem(DropItemType itemType, Vector3 spawnPosition, double value)
	{
		DropObject dropObject = null;
		switch (itemType)
		{
		case DropItemType.Gold:
			dropObject = ((GameManager.currentDungeonType != GameManager.SpecialDungeonType.ElopeMode) ? ObjectPool.Spawn("@Gold", spawnPosition).GetComponent<DropObject>() : ObjectPool.Spawn("@ElopeModeGold", spawnPosition).GetComponent<DropObject>());
			break;
		case DropItemType.Ruby:
		{
			dropObject = ObjectPool.Spawn("@Ruby", spawnPosition).GetComponent<DropObject>();
			Dictionary<DropItemType, double> dictionary;
			Dictionary<DropItemType, double> dictionary2 = (dictionary = Singleton<DropItemManager>.instance.totalIngameGainDropItem);
			DropItemType key;
			DropItemType key2 = (key = DropItemType.Ruby);
			double num = dictionary[key];
			dictionary2[key2] = num + value;
			break;
		}
		case DropItemType.TreasureKey:
			dropObject = ObjectPool.Spawn("@TreasurePiece", spawnPosition).GetComponent<DropObject>();
			break;
		case DropItemType.TranscendStone:
			dropObject = ObjectPool.Spawn("@TranscendStone", spawnPosition).GetComponent<DropObject>();
			break;
		case DropItemType.CollectEventResource:
			dropObject = ObjectPool.Spawn("@CollectEventDropObject", spawnPosition).GetComponent<DropObject>();
			break;
		case DropItemType.ElopeResource1:
		case DropItemType.ElopeResource2:
		case DropItemType.ElopeResource3:
		case DropItemType.ElopeResource4:
		{
			ElopeResourceDropObject component = ObjectPool.Spawn("@ElopeResourceDropObject", spawnPosition).GetComponent<ElopeResourceDropObject>();
			component.initElopeResource(itemType);
			dropObject = component;
			break;
		}
		case DropItemType.HeartCoin:
			dropObject = ObjectPool.Spawn("@ElopeHeartCoin", spawnPosition).GetComponent<DropObject>();
			break;
		case DropItemType.WeaponSkinPiece:
			dropObject = ObjectPool.Spawn("@WeaponSkinPiece", spawnPosition).GetComponent<DropObject>();
			break;
		}
		if (dropObject != null)
		{
			dropObject.initDropObject(value, itemType);
		}
	}

	private IEnumerator dropItemCatchUpdate()
	{
		List<DropObject> dropItemList3 = null;
		while (true)
		{
			if (!GameManager.isPause && GameManager.currentGameState == GameManager.GameState.Playing)
			{
				if (GameManager.currentDungeonType != GameManager.SpecialDungeonType.TowerMode)
				{
					for (int j = 0; j < Input.touches.Length; j++)
					{
						if (Input.touches[j].phase != 0 && Input.touches[j].phase != TouchPhase.Moved)
						{
							continue;
						}
						Vector2 mousePosition2 = Util.getCurrentScreenToWorldPosition(Input.touches[j].position);
						dropItemList3 = getNearestDropObjects(mousePosition2, dropItemCatchRange);
						if (dropItemList3.Count > 0)
						{
							int count2 = dropItemList3.Count;
							for (int k = 0; k < count2; k++)
							{
								dropItemList3[k].catchDropObject();
							}
							dropItemList3.Clear();
						}
					}
				}
				else if (Singleton<TowerModeManager>.instance.curPlayingCharacter != null)
				{
					Vector2 mousePosition2 = Singleton<TowerModeManager>.instance.curPlayingCharacter.cachedTransform.position;
					dropItemList3 = getNearestDropObjects(mousePosition2, dropItemCatchRange);
					if (dropItemList3.Count > 0)
					{
						int count = dropItemList3.Count;
						for (int i = 0; i < count; i++)
						{
							dropItemList3[i].catchDropObject();
						}
						dropItemList3.Clear();
					}
				}
			}
			yield return null;
		}
	}

	public void startAutoCatchAllItems()
	{
		StartCoroutine("autoCatchUpdate");
	}

	public void stopAutoCatchAllItems()
	{
		StopCoroutine("autoCatchUpdate");
	}

	private IEnumerator autoCatchUpdate()
	{
		while (true)
		{
			if (currentDropItemList.Count >= 1 && currentDropItemList != null)
			{
				DropObject targetDropObject = currentDropItemList[Random.Range(0, currentDropItemList.Count)];
				targetDropObject.catchDropObject();
			}
			yield return null;
		}
	}

	public List<DropObject> getNearestDropObjects(Vector2 startPosition, float range)
	{
		m_cachedDynamicList.Clear();
		int count = currentDropItemList.Count;
		for (int i = 0; i < count; i++)
		{
			if (Vector2.Distance(currentDropItemList[i].cachedTransform.position, startPosition) <= range && !currentDropItemList[i].isFlying)
			{
				m_cachedDynamicList.Add(currentDropItemList[i]);
			}
		}
		return m_cachedDynamicList;
	}
}
