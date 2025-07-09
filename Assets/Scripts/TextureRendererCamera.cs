using UnityEngine;

public class TextureRendererCamera : ObjectBase
{
	public Transform targetTransform;

	public void setAcviveCamera(bool isActive)
	{
		base.cachedGameObject.SetActive(isActive);
	}

	public void initCamera(Transform targetTransform)
	{
		this.targetTransform = targetTransform;
		setAcviveCamera(true);
	}

	private void Update()
	{
		if (targetTransform != null)
		{
			base.cachedTransform.position = targetTransform.position - new Vector3(0f, 0f, 3f);
		}
	}
}
