using System;
using UnityEngine;

public class LayeredObject : NewMovingObject
{
	protected virtual void Update()
	{
		Vector3 position = base.cachedTransform.position;
		position.z = position.y * (Math.Abs(position.y) * 1.5f);
		if (base.cachedTransform.position.z != position.z)
		{
			base.cachedTransform.position = position;
		}
	}
}
