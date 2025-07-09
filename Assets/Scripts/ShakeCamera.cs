using UnityEngine;

public class ShakeCamera : ObjectBase
{
	private static ShakeCamera m_instance;

	public bool Shaking;

	public float targetYPos;

	private float ShakeDecay;

	private float ShakeIntensity;

	private Vector3 OriginalPos;

	private Quaternion OriginalRot;

	public static ShakeCamera Instance
	{
		get
		{
			return m_instance;
		}
	}

	private void Start()
	{
		m_instance = this;
		Shaking = false;
	}

	private void Update()
	{
		if (ShakeIntensity > 0f)
		{
			base.cachedTransform.localPosition = new Vector3(OriginalPos.x + Random.Range(-0.05f, 0.05f), OriginalPos.y + Random.Range(-0.05f, 0.05f), base.transform.localPosition.z);
			base.cachedTransform.rotation = new Quaternion(OriginalRot.x, OriginalRot.y, OriginalRot.z + Random.Range(0f - ShakeIntensity, ShakeIntensity) * 0.2f, OriginalRot.w + Random.Range(0f - ShakeIntensity, ShakeIntensity) * 0.2f);
			ShakeIntensity -= ShakeDecay;
		}
		else if (Shaking)
		{
			base.cachedTransform.rotation = OriginalRot;
			base.cachedTransform.localPosition = OriginalPos;
			Shaking = false;
		}
	}

	public void shake(float intensity = 1f, float decay = 1f)
	{
		if (!Shaking)
		{
			base.cachedTransform.localPosition = new Vector3(0f, targetYPos, -10f);
			if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode)
			{
				OriginalPos = new Vector3(0f, 0.75f, -10f);
			}
			else
			{
				OriginalPos = base.cachedTransform.localPosition;
			}
			base.cachedTransform.rotation = Quaternion.identity;
			OriginalRot = base.cachedTransform.rotation;
			ShakeIntensity = intensity * 0.01f;
			ShakeDecay = decay * 0.01f;
			Shaking = true;
		}
	}
}
