using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpriteButton : Button, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerClickHandler
{
	public Image buttonImage;

	public Sprite nonPressSprite;

	public Sprite pressedSprite;

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		buttonImage.sprite = pressedSprite;
		buttonImage.SetNativeSize();
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		buttonImage.sprite = nonPressSprite;
		buttonImage.SetNativeSize();
	}
}
