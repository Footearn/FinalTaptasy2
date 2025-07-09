using System.Collections.Generic;
using UnityEngine;

public class NewObjectPool : Singleton<NewObjectPool>
{
	public class PoolObject : ObjectBase
	{
		public Object cachedObject;

		public bool isDynamicPoolObject;
	}

	private Transform m_cachedObjectRecycleTargetTransform;

	private Transform m_cachedObjectRecycleTargetTransformForDynamicCreatedObject;

	private Dictionary<string, GameObject> m_basePrefabDictionary = new Dictionary<string, GameObject>();

	private Dictionary<string, List<PoolObject>> m_totalObjectPoolDictionary = new Dictionary<string, List<PoolObject>>();

	private Dictionary<string, List<PoolObject>> m_currentNotUsingPoolDictionary = new Dictionary<string, List<PoolObject>>();

	private Dictionary<string, List<PoolObject>> m_currentUsingPoolDictionary = new Dictionary<string, List<PoolObject>>();

	private List<string> m_totalDynamicCreatedPoolNameList = new List<string>();

	public Transform cachedObjectRecycleTargetTransform
	{
		get
		{
			if (m_cachedObjectRecycleTargetTransform == null)
			{
				Transform transform = (m_cachedObjectRecycleTargetTransform = new GameObject("PoolParent").GetComponent<Transform>());
				transform.SetParent(base.transform);
				transform.gameObject.SetActive(false);
			}
			return m_cachedObjectRecycleTargetTransform;
		}
	}

	public Transform cachedObjectRecycleTargetTransformForDynamicCreatedObject
	{
		get
		{
			if (m_cachedObjectRecycleTargetTransformForDynamicCreatedObject == null)
			{
				Transform transform = (m_cachedObjectRecycleTargetTransformForDynamicCreatedObject = new GameObject("PoolParentForDynamicCreatedObject").GetComponent<Transform>());
				transform.SetParent(base.transform);
				transform.gameObject.SetActive(false);
			}
			return m_cachedObjectRecycleTargetTransformForDynamicCreatedObject;
		}
	}

	public static void CreatePool<T>(GameObject baseObject, int poolCount, bool isDynamicPool = false) where T : Object
	{
		Singleton<NewObjectPool>.instance.createPool<T>(baseObject, poolCount, isDynamicPool);
	}

	public static T Spawn<T>(string poolName) where T : Object
	{
		return Singleton<NewObjectPool>.instance.spawn<T>(poolName, Vector3.zero, Vector3.zero, Vector3.one, null);
	}

	public static T Spawn<T>(string poolName, Vector3 localPosition) where T : Object
	{
		return Singleton<NewObjectPool>.instance.spawn<T>(poolName, localPosition, Vector3.zero, Vector3.one, null);
	}

	public static T Spawn<T>(string poolName, Vector3 localPosition, Transform parent) where T : Object
	{
		return Singleton<NewObjectPool>.instance.spawn<T>(poolName, localPosition, Vector3.zero, Vector3.one, parent);
	}

	public static T Spawn<T>(string poolName, Vector3 localPosition, Vector3 localRotation) where T : Object
	{
		return Singleton<NewObjectPool>.instance.spawn<T>(poolName, localPosition, localRotation, Vector3.one, null);
	}

	public static T Spawn<T>(string poolName, Vector3 localPosition, Vector3 localRotation, Vector3 localScale) where T : Object
	{
		return Singleton<NewObjectPool>.instance.spawn<T>(poolName, localPosition, localRotation, localScale, null);
	}

	public static T Spawn<T>(string poolName, Vector3 localPosition, Vector3 localRotation, Vector3 localScale, Transform parent) where T : Object
	{
		return Singleton<NewObjectPool>.instance.spawn<T>(poolName, localPosition, localRotation, localScale, parent);
	}

	public static void Recycle(Object targetObject)
	{
		Singleton<NewObjectPool>.instance.recycle(targetObject);
	}

	public static void Clear(params string[] ignoreObjectPoolNames)
	{
		Singleton<NewObjectPool>.instance.clear(ignoreObjectPoolNames);
	}

	public static void ClearTargetPools(params string[] targetObjectPoolNames)
	{
		Singleton<NewObjectPool>.instance.clearTargetPools(targetObjectPoolNames);
	}

	public static void DestroyDynamicPools()
	{
		Singleton<NewObjectPool>.instance.destroyDynamicPools();
	}

	private void createPool<T>(GameObject baseObject, int poolCount, bool isDynamicPool = false) where T : Object
	{
		string name = baseObject.name;
		Transform transform = null;
		if (!m_basePrefabDictionary.ContainsKey(name))
		{
			m_basePrefabDictionary.Add(name, baseObject);
		}
		if (isDynamicPool)
		{
			transform = cachedObjectRecycleTargetTransformForDynamicCreatedObject;
			if (!m_totalDynamicCreatedPoolNameList.Contains(name))
			{
				m_totalDynamicCreatedPoolNameList.Add(name);
			}
		}
		else
		{
			transform = cachedObjectRecycleTargetTransform;
		}
		if (!m_totalObjectPoolDictionary.ContainsKey(name))
		{
			m_totalObjectPoolDictionary.Add(name, new List<PoolObject>());
			m_currentNotUsingPoolDictionary.Add(name, new List<PoolObject>());
			m_currentUsingPoolDictionary.Add(name, new List<PoolObject>());
		}
		List<PoolObject> list = m_totalObjectPoolDictionary[name];
		List<PoolObject> list2 = m_currentNotUsingPoolDictionary[name];
		for (int i = 0; i < poolCount; i++)
		{
			PoolObject poolObject = Object.Instantiate(baseObject).AddComponent<PoolObject>();
			if (typeof(T) == poolObject.cachedGameObject.GetType())
			{
				poolObject.cachedObject = poolObject.cachedGameObject;
			}
			else
			{
				poolObject.cachedObject = poolObject.GetComponent<T>();
			}
			poolObject.name = name;
			poolObject.isDynamicPoolObject = isDynamicPool;
			poolObject.cachedTransform.SetParent(transform);
			list.Add(poolObject);
			list2.Add(poolObject);
		}
	}

	private T spawn<T>(string poolName, Vector3 localPosition, Vector3 localRotation, Vector3 localScale, Transform parent) where T : Object
	{
		PoolObject poolObject = null;
		if (m_totalObjectPoolDictionary.ContainsKey(poolName))
		{
			List<PoolObject> list = m_currentNotUsingPoolDictionary[poolName];
			if (list.Count > 1)
			{
				poolObject = list[0];
				list.Remove(poolObject);
			}
			else
			{
				PoolObject poolObject2 = Object.Instantiate(m_basePrefabDictionary[poolName]).AddComponent<PoolObject>();
				if (typeof(T) == poolObject2.cachedGameObject.GetType())
				{
					poolObject2.cachedObject = poolObject2.cachedGameObject;
				}
				else
				{
					poolObject2.cachedObject = poolObject2.GetComponent<T>();
				}
				poolObject2.name = poolName;
				poolObject = poolObject2;
			}
			Transform cachedTransform = poolObject.cachedTransform;
			GameObject cachedGameObject = poolObject.cachedGameObject;
			cachedTransform.SetParent(parent);
			cachedTransform.localPosition = localPosition;
			cachedTransform.localEulerAngles = localRotation;
			cachedTransform.localScale = localScale;
			if (!cachedGameObject.activeSelf)
			{
				cachedGameObject.SetActive(true);
			}
			m_currentUsingPoolDictionary[poolName].Add(poolObject);
		}
		return (!(poolObject != null)) ? ((T)null) : (poolObject.cachedObject as T);
	}

	private void recycle(Object targetObject)
	{
		string name = targetObject.name;
		if (!m_currentUsingPoolDictionary.ContainsKey(name))
		{
			return;
		}
		List<PoolObject> list = m_currentUsingPoolDictionary[name];
		PoolObject poolObject = null;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].cachedObject == targetObject || list[i].cachedGameObject == targetObject)
			{
				poolObject = list[i];
			}
		}
		if (poolObject != null)
		{
			Transform transform = null;
			transform = ((!poolObject.isDynamicPoolObject) ? cachedObjectRecycleTargetTransform : cachedObjectRecycleTargetTransformForDynamicCreatedObject);
			poolObject.cachedTransform.SetParent(transform);
			list.Remove(poolObject);
			m_currentNotUsingPoolDictionary[name].Add(poolObject);
		}
	}

	private void clear(params string[] ignoreObjectPoolNames)
	{
		List<PoolObject> list = new List<PoolObject>();
		foreach (KeyValuePair<string, List<PoolObject>> item in m_currentUsingPoolDictionary)
		{
			bool flag = true;
			for (int i = 0; i < ignoreObjectPoolNames.Length; i++)
			{
				if (ignoreObjectPoolNames[i].Equals(item.Key))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				List<PoolObject> value = item.Value;
				for (int j = 0; j < value.Count; j++)
				{
					list.Add(value[j]);
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			string name = list[k].name;
			if (m_currentUsingPoolDictionary.ContainsKey(name))
			{
				recycle(list[k].cachedObject);
			}
		}
	}

	private void clearTargetPools(params string[] targetObjectPoolNames)
	{
		List<PoolObject> list = new List<PoolObject>();
		foreach (KeyValuePair<string, List<PoolObject>> item in m_currentUsingPoolDictionary)
		{
			bool flag = false;
			for (int i = 0; i < targetObjectPoolNames.Length; i++)
			{
				if (targetObjectPoolNames[i].Equals(item.Key))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				List<PoolObject> value = item.Value;
				for (int j = 0; j < value.Count; j++)
				{
					list.Add(value[j]);
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			string name = list[k].name;
			if (m_currentUsingPoolDictionary.ContainsKey(name))
			{
				recycle(list[k].cachedObject);
			}
		}
	}

	private void destroyDynamicPools()
	{
		for (int i = 0; i < m_totalDynamicCreatedPoolNameList.Count; i++)
		{
			List<PoolObject> list = m_totalObjectPoolDictionary[m_totalDynamicCreatedPoolNameList[i]];
			for (int j = 0; j < list.Count; j++)
			{
				Object.Destroy(list[j].cachedGameObject);
			}
			List<PoolObject> list2 = m_currentUsingPoolDictionary[m_totalDynamicCreatedPoolNameList[i]];
			for (int k = 0; k < list2.Count; k++)
			{
				Object.Destroy(list2[k].cachedGameObject);
			}
			List<PoolObject> list3 = m_currentNotUsingPoolDictionary[m_totalDynamicCreatedPoolNameList[i]];
			for (int l = 0; l < list3.Count; l++)
			{
				Object.Destroy(list3[l].cachedGameObject);
			}
			m_totalObjectPoolDictionary.Remove(m_totalDynamicCreatedPoolNameList[i]);
			m_currentUsingPoolDictionary.Remove(m_totalDynamicCreatedPoolNameList[i]);
			m_currentNotUsingPoolDictionary.Remove(m_totalDynamicCreatedPoolNameList[i]);
		}
		m_totalDynamicCreatedPoolNameList.Clear();
	}
}
