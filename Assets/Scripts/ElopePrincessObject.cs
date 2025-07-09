using System.Collections;
using UnityEngine;

public class ElopePrincessObject : MovingObject
{
	public SpriteAnimation currentPrincessSpriteAnimation;

	public string[] animationNameArray;

	private int m_currentPrincessIndex;

	public void initPrincess(int princessIndex)
	{
		m_currentPrincessIndex = princessIndex;
		currentPrincessSpriteAnimation.targetRenderer.sortingOrder = princessIndex;
		currentPrincessSpriteAnimation.animationType = "Princess" + m_currentPrincessIndex;
		currentPrincessSpriteAnimation.init();
		currentPrincessSpriteAnimation.playFixAnimation("Idle", 0);
		currentPrincessSpriteAnimation.stopAnimation();
		startRandomAnimaiotnPlay();
	}

	public void startRandomAnimaiotnPlay()
	{
		StopAllCoroutines();
		StartCoroutine(randomAnimationPlayUpdate());
	}

	public void stopRandomAnimationPlay()
	{
		StopAllCoroutines();
	}

	private IEnumerator randomAnimationPlayUpdate()
	{
		float timer = 0f;
		float targetAnimationChangeTime = 0f;
		while (true)
		{
			timer += Time.deltaTime;
			if (timer >= targetAnimationChangeTime)
			{
				timer -= targetAnimationChangeTime;
				targetAnimationChangeTime = Random.Range(2f, 4.5f);
				currentPrincessSpriteAnimation.playAnimation(animationNameArray[Random.Range(0, animationNameArray.Length)], Random.Range(0.1f, 0.19f), true);
			}
			yield return null;
		}
	}
}
