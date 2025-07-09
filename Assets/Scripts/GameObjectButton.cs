using UnityEngine;
using UnityEngine.EventSystems;

public class GameObjectButton : Button, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerClickHandler
{
	public GameObject nonPressObject;

	public GameObject pressedObject;

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		nonPressObject.SetActive(false);
		pressedObject.SetActive(true);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		nonPressObject.SetActive(true);
		pressedObject.SetActive(false);
		base.OnPointerUp(eventData);
	}
}
