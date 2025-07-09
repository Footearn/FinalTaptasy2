using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowCredit : UIWindow
{
	public static UIWindowCredit instance;

	[HideInInspector]
	public byte[] bt;

	[HideInInspector]
	public Text byteText;

	public RectTransform contentsRectTransform;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openWithCredit()
	{
		StopAllCoroutines();
		contentsRectTransform.anchoredPosition = new Vector2(0f, -1700f);
		byteText.text = string.Empty;
		open();
		StartCoroutine("creditUpdate");
		open();
	}

	private IEnumerator creditUpdate()
	{
		float speed = 1f;
		int touchCount = 0;
		while (true)
		{
			if (Input.GetMouseButtonUp(0))
			{
				speed = 1f;
				touchCount++;
				if (touchCount >= 25)
				{
					byteText.text = Encoding.ASCII.GetString(bt);
				}
			}
			if (Input.GetMouseButton(0))
			{
				speed = 2f;
			}
			Vector2 position = contentsRectTransform.anchoredPosition;
			if (position.y <= 1682f)
			{
				position.y += Time.deltaTime * 120f * speed;
			}
			contentsRectTransform.anchoredPosition = position;
			yield return null;
		}
	}
}
