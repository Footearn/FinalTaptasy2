using System.Collections;
using UnityEngine;

public class EvasionTextObject : ObjectBase
{
	private CanvasGroup m_cachedCanvasGroup;

	private void Awake()
	{
		m_cachedCanvasGroup = GetComponent<CanvasGroup>();
	}

	private void OnEnable()
	{
		m_cachedCanvasGroup.alpha = 1f;
		StopCoroutine("moveUpdate");
		StartCoroutine("moveUpdate");
	}

	private IEnumerator moveUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				m_cachedCanvasGroup.alpha -= Time.deltaTime * GameManager.timeScale;
				Vector2 position = base.cachedTransform.position;
				position.y += Time.deltaTime * GameManager.timeScale * 0.4f;
				base.cachedTransform.position = position;
				if (m_cachedCanvasGroup.alpha <= 0f)
				{
					break;
				}
			}
			yield return null;
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
