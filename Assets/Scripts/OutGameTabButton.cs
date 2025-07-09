using UnityEngine;
using UnityEngine.EventSystems;

public class OutGameTabButton : Button, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerClickHandler
{
	public UIWindowOutgame.UIType uiType;

	public GameObject nonPressObject;

	public GameObject pressedObject;

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (base.interactable)
		{
			base.OnPointerClick(eventData);
			if (uiType != UIWindowOutgame.instance.currentUIType)
			{
				nonPressObject.SetActive(false);
				pressedObject.SetActive(true);
			}
		}
	}
}
