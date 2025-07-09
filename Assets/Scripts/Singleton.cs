using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private bool m_isCreatedSingleton;

	private static T m_instance;

	public static T instance
	{
		get
		{
			if ((Object)m_instance == (Object)null)
			{
				m_instance = (T)Object.FindObjectOfType(typeof(T));
				if ((Object)m_instance == (Object)null && Application.isPlaying)
				{
					m_instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
					m_instance.m_isCreatedSingleton = true;
				}
				if ((Object)m_instance != (Object)null)
				{
					m_instance.init();
				}
			}
			return m_instance;
		}
	}

	protected virtual void init()
	{
	}

	public void getInstance()
	{
	}

	private void OnApplicationQuit()
	{
		if (m_isCreatedSingleton && this != null && base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
