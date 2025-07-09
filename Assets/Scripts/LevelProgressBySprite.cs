using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBySprite : MonoBehaviour
{
	public Sprite levelCellSprite;

	public Sprite emptySprite;

	public Image[] levelImagePool;

	public void setProgress(int value)
	{
		for (int i = 0; i < levelImagePool.Length; i++)
		{
			levelImagePool[i].sprite = emptySprite;
		}
		for (int j = 0; j < value; j++)
		{
			levelImagePool[j].sprite = levelCellSprite;
		}
	}
}
