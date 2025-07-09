using UnityEngine;

public class Jail : MonoBehaviour
{
	public SpriteRenderer jaildoor;

	public Sprite[] jaildoorSprite;

	public void DoorControl(bool isOpen)
	{
		if (isOpen)
		{
			jaildoor.sortingOrder = -2;
			jaildoor.sprite = jaildoorSprite[0];
		}
		else
		{
			jaildoor.sortingOrder = 1;
			jaildoor.sprite = jaildoorSprite[1];
		}
	}
}
