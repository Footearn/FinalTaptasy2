using System.Collections;
using TMPro;
using UnityEngine;

public class CustomText : MovingObject
{
	public enum TextEffectType
	{
		Up,
		BreakAway
	}

	public static int textOrder = 10;

	public TextMeshPro damageTextMesh;

	public Transform damageTextTransform;

	private TextEffectType m_currentTextEffectType;

	private float m_targetYPosition;

	private float m_upSpeed = 1f;

	public void setText(double number, float size, TextEffectType effectType, float speed = 1f)
	{
		stopAll();
		damageTextMesh.text = GameManager.changeUnit(number);
		damageTextMesh.sortingOrder = textOrder;
		if (damageTextTransform.localScale.x != size)
		{
			damageTextTransform.localScale = new Vector3(size, size, 1f);
		}
		m_upSpeed = speed;
		damageTextMesh.color = new Color(1f, 1f, 1f, 1f);
		textOrder++;
		m_currentTextEffectType = effectType;
		switch (effectType)
		{
		case TextEffectType.Up:
			StartCoroutine(upEffectUpdate());
			break;
		case TextEffectType.BreakAway:
			m_targetYPosition = base.cachedTransform.position.y - 3.3f;
			StartCoroutine(breakAwayEffectUpdate());
			break;
		}
	}

	private IEnumerator breakAwayEffectUpdate()
	{
		jump(new Vector2(Random.Range(-1f, 1f), Random.Range(5f, 6.2f)), m_targetYPosition);
		float timer = 0f;
		float alpha = 1f;
		Color color = Color.white;
		while (true)
		{
			if (m_currentTextEffectType != TextEffectType.BreakAway)
			{
				yield break;
			}
			if (!GameManager.isPause)
			{
				color = damageTextMesh.color;
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= 0.2f)
				{
					alpha = (color.a = alpha - Time.deltaTime * GameManager.timeScale * 3f);
					if (alpha <= 0f)
					{
						break;
					}
				}
				damageTextMesh.color = color;
			}
			yield return null;
		}
		recycleText();
	}

	private IEnumerator upEffectUpdate()
	{
		float timer = 0f;
		Color color = Color.white;
		while (true)
		{
			if (m_currentTextEffectType != 0)
			{
				yield break;
			}
			if (!GameManager.isPause)
			{
				color = damageTextMesh.color;
				Vector2 position = base.cachedTransform.position;
				position.y += Time.deltaTime * GameManager.timeScale * m_upSpeed;
				base.cachedTransform.position = position;
				timer += Time.deltaTime * GameManager.timeScale * 3f;
				if (timer > 1f)
				{
					color.a = 2f - timer;
					if (timer >= 2f)
					{
						break;
					}
				}
				damageTextMesh.color = color;
			}
			yield return null;
		}
		recycleText();
	}

	public void recycleText()
	{
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
