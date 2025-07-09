using UnityEngine;

public class PlaySoundForAnimation : MonoBehaviour
{
	public void playCrashSound()
	{
		Singleton<AudioManager>.instance.playEffectSound("monster_blowup");
	}

	public void playLevelUpSound()
	{
		Singleton<AudioManager>.instance.playEffectSound("levelup");
	}
}
