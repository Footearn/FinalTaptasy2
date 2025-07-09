using UnityEngine;

public class ObjectBase : MonoBehaviour
{
	private Transform m_cachedTransform;

	private GameObject m_cachedGameObject;

	private Animation m_cachedAnimation;

	private RectTransform m_cachedRectTransform;

	private Button m_cachedButton;

	public Transform cachedTransform
	{
		get
		{
			if (m_cachedTransform == null)
			{
				m_cachedTransform = base.transform;
			}
			return m_cachedTransform;
		}
	}

	public GameObject cachedGameObject
	{
		get
		{
			if (m_cachedGameObject == null)
			{
				m_cachedGameObject = base.gameObject;
			}
			return m_cachedGameObject;
		}
	}

	public Animation cachedAnimation
	{
		get
		{
			if (m_cachedAnimation == null)
			{
				m_cachedAnimation = GetComponent<Animation>();
			}
			return m_cachedAnimation;
		}
	}

	public RectTransform cachedRectTransform
	{
		get
		{
			if (m_cachedRectTransform == null)
			{
				m_cachedRectTransform = GetComponent<RectTransform>();
			}
			return m_cachedRectTransform;
		}
	}

	public Button cachedButton
	{
		get
		{
			if (m_cachedButton == null)
			{
				m_cachedButton = GetComponent<Button>();
			}
			return m_cachedButton;
		}
	}
}
