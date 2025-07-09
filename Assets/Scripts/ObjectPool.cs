using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
	public Transform cachedDynamicTransformParent;

	private List<GameObject> m_currentUsingPools;

	private Dictionary<string, GameObject[]> m_poolDictionary;

	private Dictionary<string, GameObject> m_basePrefabs;

	private List<GameObject> m_dynamicCreatedPoolList = new List<GameObject>();

	private List<string> m_dynamicCreatedPoolNameList = new List<string>();

	private Transform m_cachedTransform;

	private void initVariables()
	{
		base.transform.position = new Vector3(0f, -10000f, 0f);
		m_currentUsingPools = new List<GameObject>();
		m_poolDictionary = new Dictionary<string, GameObject[]>();
		m_basePrefabs = new Dictionary<string, GameObject>();
		m_cachedTransform = base.transform;
	}

	public static GameObject GetBasePrefab(string poolName)
	{
		return Singleton<ObjectPool>.instance.getBasePrefab(poolName);
	}

	public static void Clear(bool isIgnore, params string[] ignoreStringArray)
	{
		Singleton<ObjectPool>.instance.clear(ignoreStringArray, isIgnore);
	}

	public static void Clear(string poolName)
	{
		Singleton<ObjectPool>.instance.clear(poolName);
	}

	public static void CreatePool(GameObject prefab, int count, string poolName, bool isDynamicPool)
	{
		Singleton<ObjectPool>.instance.createPool(prefab, count, poolName, isDynamicPool);
	}

	public static void CreatePool(GameObject prefab, int count, string poolName)
	{
		Singleton<ObjectPool>.instance.createPool(prefab, count, poolName);
	}

	public static void DestroyDynamicCreatedPools()
	{
		Singleton<ObjectPool>.instance.destroyDynamicCreatedPools();
	}

	public static GameObject Spawn(string poolName, Vector3 localPosition, Transform parent = null, bool reversePositionningFromParent = false)
	{
		return Singleton<ObjectPool>.instance.spawn(poolName, localPosition, Vector3.zero, Vector3.one, parent, reversePositionningFromParent);
	}

	public static GameObject Spawn(string poolName, Vector3 localPosition, Vector3 localRotation, Transform parent = null, bool reversePositionningFromParent = false)
	{
		return Singleton<ObjectPool>.instance.spawn(poolName, localPosition, localRotation, Vector3.one, parent, reversePositionningFromParent);
	}

	public static GameObject Spawn(string poolName, Vector3 localPosition, Vector3 localRotation, Vector3 localScale, Transform parent = null, bool reversePositionningFromParent = false)
	{
		return Singleton<ObjectPool>.instance.spawn(poolName, localPosition, localRotation, localScale, parent, reversePositionningFromParent);
	}

	public static void Recycle(string poolName, GameObject instance)
	{
		Singleton<ObjectPool>.instance.recycle(poolName, instance);
	}

	private GameObject getBasePrefab(string poolName)
	{
		if (m_basePrefabs.ContainsKey(poolName))
		{
			return m_basePrefabs[poolName];
		}
		return null;
	}

	private GameObject spawn(string poolName, Vector3 localPosition, Vector3 localRotation, Vector3 localScale, Transform parent, bool reversePositionningFromParent)
	{
		if (m_poolDictionary == null)
		{
			initVariables();
		}
		if (m_poolDictionary.ContainsKey(poolName))
		{
			GameObject[] array = m_poolDictionary[poolName];
			GameObject gameObject = null;
			Transform transform = null;
			for (int i = 0; i < array.Length; i++)
			{
				if (!m_currentUsingPools.Contains(array[i]))
				{
					gameObject = array[i];
					transform = gameObject.transform;
					if (reversePositionningFromParent)
					{
						transform.position = localPosition;
						transform.SetParent(parent);
					}
					else
					{
						transform.SetParent(parent);
						transform.localPosition = localPosition;
					}
					transform.rotation = Quaternion.Euler(localRotation);
					transform.localScale = localScale;
					gameObject.SetActive(true);
					m_currentUsingPools.Add(gameObject);
					return gameObject;
				}
			}
			GameObject[] array2 = new GameObject[array.Length + 1];
			for (int j = 0; j < array.Length; j++)
			{
				array2[j] = array[j];
			}
			array2[array2.Length - 1] = Object.Instantiate(m_basePrefabs[poolName]);
			Transform transform2 = array2[array2.Length - 1].transform;
			transform2.name = transform2.name.Replace("(Clone)", string.Empty);
			SpriteRenderer[] componentsInChildren = transform2.GetComponentsInChildren<SpriteRenderer>(true);
			if (isCanChangeShader(transform2.gameObject.name))
			{
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					if (componentsInChildren[k].material != Singleton<ResourcesManager>.instance.spriteDefaultMaterial)
					{
						componentsInChildren[k].material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
					}
				}
			}
			if (reversePositionningFromParent)
			{
				transform2.position = localPosition;
				transform2.SetParent(parent);
			}
			else
			{
				transform2.SetParent(parent);
				transform2.localPosition = localPosition;
			}
			transform2.rotation = Quaternion.Euler(localRotation);
			transform2.localScale = localScale;
			m_currentUsingPools.Add(array2[array2.Length - 1]);
			if (m_dynamicCreatedPoolNameList.Contains(poolName))
			{
			}
			m_poolDictionary[poolName] = array2;
			return array2[array2.Length - 1];
		}
		DebugManager.LogError("This gameObject is null\nPoolName : " + poolName);
		return null;
	}

	private void recycle(string poolName, GameObject instance)
	{
		if (m_poolDictionary == null)
		{
			initVariables();
		}
		if (!m_poolDictionary.ContainsKey(poolName))
		{
			DebugManager.LogError("Not found this pool\npoolName : " + poolName);
			return;
		}
		GameObject[] array = m_poolDictionary[poolName];
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == instance))
			{
				continue;
			}
			if (m_currentUsingPools.Contains(array[i]))
			{
				Transform transform = array[i].transform;
				if (m_dynamicCreatedPoolList.Contains(array[i]))
				{
					transform.SetParent(cachedDynamicTransformParent);
				}
				else
				{
					transform.SetParent(m_cachedTransform);
				}
				transform.localPosition = Vector3.zero;
				array[i].SetActive(false);
				m_currentUsingPools.Remove(array[i]);
			}
			break;
		}
	}

	private void createPool(GameObject basePrefab, int count, string poolName, bool isDynamicPool = false)
	{
		if (m_poolDictionary == null)
		{
			initVariables();
		}
		if (!m_poolDictionary.ContainsKey(poolName))
		{
			m_basePrefabs.Add(poolName, basePrefab);
			GameObject[] array = new GameObject[count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Object.Instantiate(basePrefab);
				SpriteRenderer[] componentsInChildren = array[i].GetComponentsInChildren<SpriteRenderer>(true);
				array[i].name = array[i].name.Replace("(Clone)", string.Empty);
				if (isCanChangeShader(array[i].name))
				{
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						if (componentsInChildren[j].material != Singleton<ResourcesManager>.instance.spriteDefaultMaterial)
						{
							componentsInChildren[j].material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
						}
					}
				}
				Transform transform = array[i].transform;
				if (isDynamicPool)
				{
					transform.SetParent(cachedDynamicTransformParent);
					m_dynamicCreatedPoolNameList.Add(poolName);
					m_dynamicCreatedPoolList.Add(array[i]);
				}
				else
				{
					transform.SetParent(m_cachedTransform);
				}
				transform.localPosition = Vector3.zero;
				array[i].SetActive(false);
			}
			m_poolDictionary.Add(poolName, array);
			return;
		}
		GameObject[] array2 = new GameObject[m_poolDictionary[poolName].Length + count];
		for (int k = 0; k < m_poolDictionary[poolName].Length; k++)
		{
			array2[k] = m_poolDictionary[poolName][k];
		}
		for (int l = m_poolDictionary[poolName].Length - 1; l < array2.Length; l++)
		{
			array2[l] = Object.Instantiate(basePrefab);
			array2[l].name = array2[l].name.Replace("(Clone)", string.Empty);
			SpriteRenderer[] componentsInChildren2 = array2[l].GetComponentsInChildren<SpriteRenderer>(true);
			if (isCanChangeShader(array2[l].name))
			{
				for (int m = 0; m < componentsInChildren2.Length; m++)
				{
					if (componentsInChildren2[m].material != Singleton<ResourcesManager>.instance.spriteDefaultMaterial)
					{
						componentsInChildren2[m].material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
					}
				}
			}
			Transform transform2 = array2[l].transform;
			if (isDynamicPool)
			{
				transform2.SetParent(cachedDynamicTransformParent);
				m_dynamicCreatedPoolList.Add(array2[l]);
			}
			else
			{
				transform2.SetParent(m_cachedTransform);
			}
			transform2.localPosition = Vector3.zero;
			array2[l].SetActive(false);
		}
		m_poolDictionary[poolName] = array2;
	}

	private void clear(string[] ignoreString, bool isIgnore)
	{
		foreach (KeyValuePair<string, GameObject[]> item in m_poolDictionary)
		{
			bool flag = true;
			for (int i = 0; i < ignoreString.Length; i++)
			{
				if (item.Key.Contains(ignoreString[i]))
				{
					flag = false;
					break;
				}
			}
			if ((flag ? 1 : 0) == (isIgnore ? 1 : 0))
			{
				GameObject[] value = item.Value;
				for (int j = 0; j < value.Length; j++)
				{
					recycle(item.Key, value[j]);
				}
			}
		}
	}

	private void clear(string poolName)
	{
		if (m_poolDictionary.ContainsKey(poolName))
		{
			GameObject[] array = m_poolDictionary[poolName];
			for (int i = 0; i < array.Length; i++)
			{
				recycle(poolName, array[i]);
			}
		}
	}

	private void destroyDynamicCreatedPools()
	{
		for (int i = 0; i < m_dynamicCreatedPoolList.Count; i++)
		{
			if (m_dynamicCreatedPoolList[i] != null)
			{
				if (m_currentUsingPools.Contains(m_dynamicCreatedPoolList[i]))
				{
					m_currentUsingPools.Remove(m_dynamicCreatedPoolList[i]);
				}
				if (m_poolDictionary.ContainsKey(m_dynamicCreatedPoolList[i].name))
				{
					m_poolDictionary.Remove(m_dynamicCreatedPoolList[i].name);
				}
				if (m_basePrefabs.ContainsKey(m_dynamicCreatedPoolList[i].name))
				{
					m_basePrefabs.Remove(m_dynamicCreatedPoolList[i].name);
				}
				Object.DestroyImmediate(m_dynamicCreatedPoolList[i]);
			}
		}
		m_dynamicCreatedPoolList.Clear();
		m_dynamicCreatedPoolNameList.Clear();
	}

	private bool isCanChangeShader(string name)
	{
		bool result = true;
		switch (name)
		{
		case "@ElopeModeBackground":
		case "@TowerModeSky":
		case "@TowerModeMap":
		case "@TowerModeBossMap":
		case "@TowerModeMiniBossMap":
			result = false;
			break;
		}
		return result;
	}
}
