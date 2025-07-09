using System.Collections;
using UnityEngine;

public class BossObjectForThemeEvent : MovingObject
{
	public SpriteAnimation cachedSpriteAnimation;

	public SpriteRenderer maskSpriteRenderer;

	public Transform maskTransform;

	public GameObject maskObject;

	public GameObject handObject;

	public GameObject shiningObject;

	public void initBoss()
	{
		StopAllCoroutines();
		shiningObject.SetActive(false);
		cachedSpriteAnimation.playFixAnimation("Cry", 0);
		maskObject.SetActive(true);
		handObject.SetActive(true);
		maskTransform.localPosition = new Vector3(-0.005f, 1.22f, 0f);
		maskSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
	}

	public void fallMask()
	{
		StopCoroutine("fallMaskUpdate");
		StartCoroutine("fallMaskUpdate");
	}

	private IEnumerator fallMaskUpdate()
	{
		Vector2 position = Vector2.zero;
		Color color = Color.white;
		while (true)
		{
			position = maskTransform.localPosition;
			position.y -= Time.deltaTime * 1.2f;
			maskTransform.localPosition = position;
			color = maskSpriteRenderer.color;
			color.a -= Time.deltaTime;
			maskSpriteRenderer.color = color;
			if (position.y < 0.25f && color.a <= 0f)
			{
				break;
			}
			yield return null;
		}
		maskObject.SetActive(false);
		maskTransform.localPosition = new Vector3(-0.005f, 1.22f, 0f);
	}
}
