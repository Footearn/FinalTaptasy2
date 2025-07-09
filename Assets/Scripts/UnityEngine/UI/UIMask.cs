using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	public class UIMask : MaskableGraphic
	{
		private static UIMask instance;

		[FormerlySerializedAs("m_Tex")]
		[SerializeField]
		private Texture m_Texture;

		[SerializeField]
		private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		private Action closeCallback;

		public RectTransform targetRectTransform;

		private RectTransform m_targetParentRectTransform;

		private int m_targetRectTransformSiblingIndex;

		private float m_alpha;

		private bool m_isOpen;

		public override Texture mainTexture
		{
			get
			{
				if (m_Texture == null && targetRectTransform != null)
				{
					Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, mipChain: false, linear: true);
					Color color = new Color(0f, 0f, 0f, 0.8f);
					Color color2 = new Color(0f, 0f, 0f, 0f);
					Color[] pixels = texture2D.GetPixels();
					Vector3 position = targetRectTransform.position;
					int num = (int)position.x;
					Vector3 position2 = targetRectTransform.position;
					int num2 = (int)position2.y;
					Vector2 vector = targetRectTransform.sizeDelta * 0.5f * 1.2f;
					int num3 = (int)(vector.x * 0.5f);
					int num4 = (int)(vector.y * 0.5f);
					for (int i = 0; i < pixels.Length; i++)
					{
						pixels[i] = color;
					}
					texture2D.SetPixels(pixels);
					for (int j = 0; (float)j < vector.x; j++)
					{
						for (int k = 0; (float)k < vector.y; k++)
						{
							texture2D.SetPixel(num - num3 + j, num2 - num4 + k, color2);
						}
					}
					texture2D.Apply();
					m_Texture = texture2D;
				}
				return m_Texture;
			}
		}

		public Rect uvRect
		{
			get
			{
				return m_UVRect;
			}
			set
			{
				if (!(m_UVRect == value))
				{
					m_UVRect = value;
					SetVerticesDirty();
				}
			}
		}

		protected UIMask()
		{
		}

		public static UIMask SetMask(RectTransform targetRectTransform)
		{
			instance.m_Texture = null;
			instance.SetEnable(_enable: true);
			instance.targetRectTransform = targetRectTransform;
			instance.m_targetRectTransformSiblingIndex = targetRectTransform.GetSiblingIndex();
			instance.m_targetParentRectTransform = targetRectTransform.parent.GetComponent<RectTransform>();
			instance.targetRectTransform.SetParent(instance.transform);
			instance.m_isOpen = true;
			instance.m_alpha = 0f;
			instance.color = new Color(0f, 0f, 0f, instance.m_alpha);
			return instance;
		}

		public static void CloseMask(Action callback)
		{
			if (instance.targetRectTransform != null)
			{
				instance.closeCallback = callback;
				instance.m_isOpen = false;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			instance = this;
			SetEnable(_enable: false);
		}

		private void SetEnable(bool _enable)
		{
			if (Application.isPlaying)
			{
				base.enabled = _enable;
			}
		}

		public override void SetNativeSize()
		{
			Texture mainTexture = this.mainTexture;
			if (mainTexture != null)
			{
				int num = Mathf.RoundToInt((float)mainTexture.width * uvRect.width);
				int num2 = Mathf.RoundToInt((float)mainTexture.height * uvRect.height);
				base.rectTransform.anchorMax = base.rectTransform.anchorMin;
				base.rectTransform.sizeDelta = new Vector2(num, num2);
			}
		}

		private void Update()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (m_isOpen)
			{
				if (m_alpha != 1f)
				{
					m_alpha += Time.deltaTime * GameManager.timeScale * 3f;
					m_alpha = Mathf.Clamp01(m_alpha);
					base.color = new Color(1f, 1f, 1f, m_alpha);
				}
				return;
			}
			m_alpha -= Time.deltaTime * GameManager.timeScale * 3f;
			m_alpha = Mathf.Clamp01(m_alpha);
			base.color = new Color(1f, 1f, 1f, m_alpha);
			if (instance.targetRectTransform != null)
			{
				instance.targetRectTransform.SetParent(instance.m_targetParentRectTransform);
				instance.targetRectTransform.SetSiblingIndex(instance.m_targetRectTransformSiblingIndex);
				instance.targetRectTransform = null;
			}
			if (m_alpha == 0f)
			{
				closeCallback();
				instance.SetEnable(_enable: false);
			}
		}

		protected override void OnFillVBO(List<UIVertex> vbo)
		{
			m_Texture = null;
			Texture mainTexture = this.mainTexture;
			if (mainTexture != null)
			{
				Vector4 zero = Vector4.zero;
				int num = Mathf.RoundToInt((float)mainTexture.width * uvRect.width);
				int num2 = Mathf.RoundToInt((float)mainTexture.height * uvRect.height);
				float num3 = ((num & 1) != 0) ? (num + 1) : num;
				float num4 = ((num2 & 1) != 0) ? (num2 + 1) : num2;
				zero.x = 0f;
				zero.y = 0f;
				zero.z = (float)num / num3;
				zero.w = (float)num2 / num4;
				float x = zero.x;
				Vector2 pivot = base.rectTransform.pivot;
				zero.x = x - pivot.x;
				float y = zero.y;
				Vector2 pivot2 = base.rectTransform.pivot;
				zero.y = y - pivot2.y;
				float z = zero.z;
				Vector2 pivot3 = base.rectTransform.pivot;
				zero.z = z - pivot3.x;
				float w = zero.w;
				Vector2 pivot4 = base.rectTransform.pivot;
				zero.w = w - pivot4.y;
				zero.x *= base.rectTransform.rect.width;
				zero.y *= base.rectTransform.rect.height;
				zero.z *= base.rectTransform.rect.width;
				zero.w *= base.rectTransform.rect.height;
				vbo.Clear();
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.position = new Vector2(zero.x, zero.y);
				simpleVert.uv0 = new Vector2(m_UVRect.xMin, m_UVRect.yMin);
				simpleVert.color = base.color;
				vbo.Add(simpleVert);
				simpleVert.position = new Vector2(zero.x, zero.w);
				simpleVert.uv0 = new Vector2(m_UVRect.xMin, m_UVRect.yMax);
				simpleVert.color = base.color;
				vbo.Add(simpleVert);
				simpleVert.position = new Vector2(zero.z, zero.w);
				simpleVert.uv0 = new Vector2(m_UVRect.xMax, m_UVRect.yMax);
				simpleVert.color = base.color;
				vbo.Add(simpleVert);
				simpleVert.position = new Vector2(zero.z, zero.y);
				simpleVert.uv0 = new Vector2(m_UVRect.xMax, m_UVRect.yMin);
				simpleVert.color = base.color;
				vbo.Add(simpleVert);
			}
		}
	}
}
