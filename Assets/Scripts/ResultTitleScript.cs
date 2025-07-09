using UnityEngine;

public class ResultTitleScript : MonoBehaviour
{
	public void playClashSound()
	{
		Singleton<AudioManager>.instance.playEffectSound("monster_blowup");
	}
}
