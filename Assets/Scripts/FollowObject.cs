using UnityEngine;

public class FollowObject : ObjectBase
{
	public Transform followTarget;

	public Vector2 offset;

	private void LateUpdate()
	{
		if (followTarget != null)
		{
			base.cachedTransform.position = followTarget.position + (Vector3)offset;
		}
	}
}
