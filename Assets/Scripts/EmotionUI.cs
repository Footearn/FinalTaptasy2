using UnityEngine;

public class EmotionUI : SpriteGroup
{
	public enum emotionType
	{
		Surprise,
		Angry
	}

	private Animation emotionAnim;

	private void Awake()
	{
		emotionAnim = GetComponent<Animation>();
	}

	public void SetEmotion(emotionType type)
	{
		if (!emotionAnim.isPlaying)
		{
			emotionAnim.Play("Emotion" + type);
		}
	}
}
