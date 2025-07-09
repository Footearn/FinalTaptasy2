using UnityEngine;

public class LastPrincess : MovingObject
{
	public SpriteAnimation spriteAnimation;

	public GameObject[] bossObject;

	private int m_currentPrincessIndex = -10;

	private void OnEnable()
	{
		if (m_currentPrincessIndex != GameManager.getCurrentPrincessNumber())
		{
			m_currentPrincessIndex = GameManager.getCurrentPrincessNumber();
			spriteAnimation.animationType = "Princess" + m_currentPrincessIndex;
			spriteAnimation.init();
		}
		for (int i = 0; i < bossObject.Length; i++)
		{
			bossObject[i].SetActive(false);
		}
	}
}
