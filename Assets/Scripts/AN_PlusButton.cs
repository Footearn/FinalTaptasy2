using System;
using UnityEngine;

public class AN_PlusButton
{
	private int _ButtonId;

	private TextAnchor _anchor = TextAnchor.MiddleCenter;

	private int _x;

	private int _y;

	private bool _IsShowed = true;

	public Action ButtonClicked = delegate
	{
	};

	private static int _nextId;

	public int ButtonId
	{
		get
		{
			return _ButtonId;
		}
	}

	public int x
	{
		get
		{
			return _x;
		}
	}

	public int y
	{
		get
		{
			return _y;
		}
	}

	public bool IsShowed
	{
		get
		{
			return _IsShowed;
		}
	}

	public TextAnchor anchor
	{
		get
		{
			return _anchor;
		}
	}

	public int gravity
	{
		get
		{
			switch (_anchor)
			{
			case TextAnchor.LowerCenter:
				return 81;
			case TextAnchor.LowerLeft:
				return 83;
			case TextAnchor.LowerRight:
				return 85;
			case TextAnchor.MiddleCenter:
				return 17;
			case TextAnchor.MiddleLeft:
				return 19;
			case TextAnchor.MiddleRight:
				return 21;
			case TextAnchor.UpperCenter:
				return 49;
			case TextAnchor.UpperLeft:
				return 51;
			case TextAnchor.UpperRight:
				return 53;
			default:
				return 48;
			}
		}
	}

	private static int nextId
	{
		get
		{
			_nextId++;
			return _nextId;
		}
	}

	public AN_PlusButton(string url, AN_PlusBtnSize btnSize, AN_PlusBtnAnnotation annotation)
	{
		_ButtonId = nextId;
		AN_PlusButtonProxy.createPlusButton(_ButtonId, url, (int)btnSize, (int)annotation);
		SA_Singleton<AN_PlusButtonsManager>.instance.RegisterButton(this);
	}

	public void SetGravity(TextAnchor btnAnchor)
	{
		_anchor = btnAnchor;
		AN_PlusButtonProxy.setGravity(gravity, _ButtonId);
	}

	public void SetPosition(int btnX, int btnY)
	{
		_x = btnX;
		_y = btnY;
		AN_PlusButtonProxy.setPosition(_x, _y, _ButtonId);
	}

	public void Show()
	{
		_IsShowed = true;
		AN_PlusButtonProxy.show(_ButtonId);
	}

	public void Hide()
	{
		_IsShowed = false;
		AN_PlusButtonProxy.hide(_ButtonId);
	}

	public void Refresh()
	{
		AN_PlusButtonProxy.refresh(_ButtonId);
	}

	public void FireClickAction()
	{
		ButtonClicked();
	}
}
