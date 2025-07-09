using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PermissionsManager : SA_Singleton<PermissionsManager>
{
	private const string PM_CLASS_NAME = "com.androidnative.features.permissions.PermissionsManager";

	[method: MethodImpl(32)]
	public static event Action<AN_GrantPermissionsResult> ActionPermissionsRequestCompleted;

	static PermissionsManager()
	{
		PermissionsManager.ActionPermissionsRequestCompleted = delegate
		{
		};
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RequestPermissions(params AN_ManifestPermission[] permissions)
	{
		List<string> list = new List<string>();
		foreach (AN_ManifestPermission permission in permissions)
		{
			list.Add(permission.GetFullName());
		}
		RequestPermissions(list.ToArray());
	}

	public void RequestPermissions(params string[] permissions)
	{
		
	}

	public static AN_ManifestPermission GetPermissionByName(string fullName)
	{
		foreach (int value in Enum.GetValues(typeof(AN_ManifestPermission)))
		{
			if (((AN_ManifestPermission)value).GetFullName().Equals(fullName))
			{
				return (AN_ManifestPermission)value;
			}
		}
		return AN_ManifestPermission.UNDEFINED;
	}
}
