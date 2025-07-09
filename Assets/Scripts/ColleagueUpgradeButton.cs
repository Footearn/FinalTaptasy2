using System;
using UnityEngine.EventSystems;

public class ColleagueUpgradeButton : Button
{
	private long downTimeCheck;

	public bool isTouchPress;

	public ColleagueSlotObject colleagueSlot;

	private DateTime touchWaitUntil;

	private bool m_isUpgraded;

	private new void Start()
	{
		isTouchPress = false;
		touchWaitUntil = DateTime.Now;
		base.Start();
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (base.interactable && !m_isUpgraded)
		{
			colleagueSlot.OnClickLevelUp();
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (base.interactable)
		{
			m_isUpgraded = false;
			isTouchPress = true;
			base.OnPointerDown(eventData);
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (base.interactable)
		{
			isTouchPress = false;
			base.OnPointerUp(eventData);
		}
	}
}
