using UnityEngine;

public class ImmersiveMode : SA_Singleton<ImmersiveMode>
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
