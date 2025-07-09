using UnityEngine;

public class TouchEffectForTutorial : MonoBehaviour
{
	public Transform targetTouchTransform;

	public void touchEffect()
	{
		Singleton<AudioManager>.instance.playEffectSound("touch", AudioManager.EffectType.Touch);
		ObjectPool.Spawn("fx_tap", targetTouchTransform.position);
	}
}
