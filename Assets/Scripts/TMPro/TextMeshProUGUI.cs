using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasRenderer))]
	[AddComponentMenu("UI/TextMeshPro - Text (UI)", 11)]
	[SelectionBase]
	[RequireComponent(typeof(RectTransform))]
	public class TextMeshProUGUI : TMP_Text, ILayoutElement
	{
		[SerializeField]
		private Vector2 m_uvOffset = Vector2.zero;

		[SerializeField]
		private float m_uvLineOffset;

		[SerializeField]
		private bool m_hasFontAssetChanged;

		[SerializeField]
		protected TMP_SubMeshUI[] m_subTextObjects = new TMP_SubMeshUI[8];

		private float m_previousLossyScaleY = -1f;

		private Vector3[] m_RectTransformCorners = new Vector3[4];

		private CanvasRenderer m_canvasRenderer;

		private Canvas m_canvas;

		private bool m_isFirstAllocation;

		private int m_max_characters = 8;

		private WordWrapState m_SavedWordWrapState = default(WordWrapState);

		private WordWrapState m_SavedLineState = default(WordWrapState);

		private bool m_isMaskingEnabled;

		[SerializeField]
		private Material m_baseMaterial;

		private bool m_isScrollRegionSet;

		private int m_stencilID;

		[SerializeField]
		private Vector4 m_maskOffset;

		private Matrix4x4 m_EnvMapMatrix = default(Matrix4x4);

		[NonSerialized]
		private bool m_isRegisteredForEvents;

		private int m_recursiveCount;

		private int m_recursiveCountA;

		private int loopCountA;

		private bool m_isRebuildingLayout;

		public override Material materialForRendering
		{
			get
			{
				if (m_sharedMaterial == null)
				{
					return null;
				}
				return GetModifiedMaterial(m_sharedMaterial);
			}
		}

		public override Mesh mesh
		{
			get
			{
				return m_mesh;
			}
		}

		public new CanvasRenderer canvasRenderer
		{
			get
			{
				if (m_canvasRenderer == null)
				{
					m_canvasRenderer = GetComponent<CanvasRenderer>();
				}
				return m_canvasRenderer;
			}
		}

		public InlineGraphicManager inlineGraphicManager
		{
			get
			{
				return m_inlineGraphics;
			}
		}

		public override Bounds bounds
		{
			get
			{
				if (m_mesh != null)
				{
					return m_mesh.bounds;
				}
				return default(Bounds);
			}
		}

		public Vector4 maskOffset
		{
			get
			{
				return m_maskOffset;
			}
			set
			{
				m_maskOffset = value;
				UpdateMask();
				m_havePropertiesChanged = true;
			}
		}

		protected override void Awake()
		{
			m_canvas = base.canvas;
			m_isOrthographic = true;
			m_rectTransform = base.gameObject.GetComponent<RectTransform>();
			if (m_rectTransform == null)
			{
				m_rectTransform = base.gameObject.AddComponent<RectTransform>();
			}
			m_canvasRenderer = GetComponent<CanvasRenderer>();
			if (m_canvasRenderer == null)
			{
				m_canvasRenderer = base.gameObject.AddComponent<CanvasRenderer>();
			}
			if (m_mesh == null)
			{
				m_mesh = new Mesh();
				m_mesh.hideFlags = HideFlags.HideAndDontSave;
			}
			if (m_text == null)
			{
				m_enableWordWrapping = TMP_Settings.enableWordWrapping;
				m_enableKerning = TMP_Settings.enableKerning;
				m_enableExtraPadding = TMP_Settings.enableExtraPadding;
				m_tintAllSprites = TMP_Settings.enableTintAllSprites;
				m_parseCtrlCharacters = TMP_Settings.enableParseEscapeCharacters;
			}
			LoadFontAsset();
			TMP_StyleSheet.LoadDefaultStyleSheet();
			m_char_buffer = new int[m_max_characters];
			m_cached_TextElement = new TMP_Glyph();
			m_isFirstAllocation = true;
			if (m_textInfo == null)
			{
				m_textInfo = new TMP_TextInfo(this);
			}
			if (m_fontAsset == null)
			{
				Debug.LogWarning("Please assign a Font Asset to this " + base.transform.name + " gameobject.", this);
				return;
			}
			if (m_fontSizeMin == 0f)
			{
				m_fontSizeMin = m_fontSize / 2f;
			}
			if (m_fontSizeMax == 0f)
			{
				m_fontSizeMax = m_fontSize * 2f;
			}
			m_isInputParsingRequired = true;
			m_havePropertiesChanged = true;
			m_isCalculateSizeRequired = true;
			m_isAwake = true;
		}

		protected override void OnEnable()
		{
			if (!m_isRegisteredForEvents)
			{
				m_isRegisteredForEvents = true;
			}
			m_canvas = GetCanvas();
			SetActiveSubMeshes(true);
			GraphicRegistry.RegisterGraphicForCanvas(m_canvas, this);
			ComputeMarginSize();
			m_verticesAlreadyDirty = false;
			m_layoutAlreadyDirty = false;
			m_ShouldRecalculateStencil = true;
			m_isInputParsingRequired = true;
			SetAllDirty();
			RecalculateClipping();
		}

		protected override void OnDisable()
		{
			if (m_MaskMaterial != null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(m_MaskMaterial);
				m_MaskMaterial = null;
			}
			GraphicRegistry.UnregisterGraphicForCanvas(m_canvas, this);
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (m_canvasRenderer != null)
			{
				m_canvasRenderer.Clear();
			}
			SetActiveSubMeshes(false);
			LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
			RecalculateClipping();
		}

		protected override void OnDestroy()
		{
			GraphicRegistry.UnregisterGraphicForCanvas(m_canvas, this);
			if (m_mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(m_mesh);
			}
			if (m_MaskMaterial != null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(m_MaskMaterial);
				m_MaskMaterial = null;
			}
			m_isRegisteredForEvents = false;
		}

		protected override void LoadFontAsset()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (m_fontAsset == null)
			{
				if (TMP_Settings.defaultFontAsset != null)
				{
					m_fontAsset = TMP_Settings.defaultFontAsset;
				}
				else
				{
					m_fontAsset = Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
				}
				if (m_fontAsset == null)
				{
					Debug.LogWarning("The ARIAL SDF Font Asset was not found. There is no Font Asset assigned to " + base.gameObject.name + ".", this);
					return;
				}
				if (m_fontAsset.characterDictionary == null)
				{
					Debug.Log("Dictionary is Null!");
				}
				m_sharedMaterial = m_fontAsset.material;
			}
			else
			{
				if (m_fontAsset.characterDictionary == null)
				{
					m_fontAsset.ReadFontDefinition();
				}
				if (m_sharedMaterial == null && m_baseMaterial != null)
				{
					m_sharedMaterial = m_baseMaterial;
					m_baseMaterial = null;
				}
				if (m_sharedMaterial == null || m_sharedMaterial.mainTexture == null || m_fontAsset.atlas.GetInstanceID() != m_sharedMaterial.mainTexture.GetInstanceID())
				{
					if (m_fontAsset.material == null)
					{
						Debug.LogWarning("The Font Atlas Texture of the Font Asset " + m_fontAsset.name + " assigned to " + base.gameObject.name + " is missing.", this);
					}
					else
					{
						m_sharedMaterial = m_fontAsset.material;
					}
				}
			}
			GetSpecialCharacters(m_fontAsset);
			m_padding = GetPaddingForMaterial();
			SetMaterialDirty();
		}

		private Canvas GetCanvas()
		{
			Canvas result = null;
			List<Canvas> list = TMP_ListPool<Canvas>.Get();
			base.gameObject.GetComponentsInParent(false, list);
			if (list.Count > 0)
			{
				result = list[0];
			}
			TMP_ListPool<Canvas>.Release(list);
			return result;
		}

		private void UpdateEnvMapMatrix()
		{
			if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) && !(m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) == null))
			{
				Vector3 euler = m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
				m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(euler), Vector3.one);
				m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, m_EnvMapMatrix);
			}
		}

		private void EnableMasking()
		{
			if (m_fontMaterial == null)
			{
				m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
				m_canvasRenderer.SetMaterial(m_fontMaterial, m_sharedMaterial.mainTexture);
			}
			m_sharedMaterial = m_fontMaterial;
			if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_ClipRect))
			{
				m_sharedMaterial.EnableKeyword(ShaderUtilities.Keyword_MASK_SOFT);
				m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_HARD);
				m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_TEX);
				UpdateMask();
			}
			m_isMaskingEnabled = true;
		}

		private void DisableMasking()
		{
			if (m_fontMaterial != null)
			{
				if (m_stencilID > 0)
				{
					m_sharedMaterial = m_MaskMaterial;
				}
				m_canvasRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
				UnityEngine.Object.DestroyImmediate(m_fontMaterial);
			}
			m_isMaskingEnabled = false;
		}

		private void UpdateMask()
		{
			if (m_rectTransform != null)
			{
				if (!ShaderUtilities.isInitialized)
				{
					ShaderUtilities.GetShaderPropertyIDs();
				}
				m_isScrollRegionSet = true;
				float num = Mathf.Min(Mathf.Min(m_margin.x, m_margin.z), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
				float num2 = Mathf.Min(Mathf.Min(m_margin.y, m_margin.w), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
				num = ((!(num > 0f)) ? 0f : num);
				num2 = ((!(num2 > 0f)) ? 0f : num2);
				float z = (m_rectTransform.rect.width - Mathf.Max(m_margin.x, 0f) - Mathf.Max(m_margin.z, 0f)) / 2f + num;
				float w = (m_rectTransform.rect.height - Mathf.Max(m_margin.y, 0f) - Mathf.Max(m_margin.w, 0f)) / 2f + num2;
				Vector2 vector = m_rectTransform.localPosition + new Vector3((0.5f - m_rectTransform.pivot.x) * m_rectTransform.rect.width + (Mathf.Max(m_margin.x, 0f) - Mathf.Max(m_margin.z, 0f)) / 2f, (0.5f - m_rectTransform.pivot.y) * m_rectTransform.rect.height + (0f - Mathf.Max(m_margin.y, 0f) + Mathf.Max(m_margin.w, 0f)) / 2f);
				Vector4 vector2 = new Vector4(vector.x, vector.y, z, w);
				m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, vector2);
			}
		}

		protected override Material GetMaterial(Material mat)
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (m_fontMaterial == null || m_fontMaterial.GetInstanceID() != mat.GetInstanceID())
			{
				m_fontMaterial = CreateMaterialInstance(mat);
			}
			m_sharedMaterial = m_fontMaterial;
			m_padding = GetPaddingForMaterial();
			m_ShouldRecalculateStencil = true;
			SetVerticesDirty();
			SetMaterialDirty();
			return m_sharedMaterial;
		}

		protected override Material[] GetMaterials(Material[] mats)
		{
			int materialCount = m_textInfo.materialCount;
			if (m_fontMaterials == null)
			{
				m_fontMaterials = new Material[materialCount];
			}
			else if (m_fontMaterials.Length != materialCount)
			{
				TMP_TextInfo.Resize(ref m_fontMaterials, materialCount, false);
			}
			for (int i = 0; i < materialCount; i++)
			{
				if (i == 0)
				{
					m_fontMaterials[i] = base.fontMaterial;
				}
				else
				{
					m_fontMaterials[i] = m_subTextObjects[i].material;
				}
			}
			m_fontSharedMaterials = m_fontMaterials;
			return m_fontMaterials;
		}

		protected override void SetSharedMaterial(Material mat)
		{
			m_sharedMaterial = mat;
			m_padding = GetPaddingForMaterial();
			SetMaterialDirty();
		}

		protected override Material[] GetSharedMaterials()
		{
			int materialCount = m_textInfo.materialCount;
			if (m_fontSharedMaterials == null)
			{
				m_fontSharedMaterials = new Material[materialCount];
			}
			else if (m_fontSharedMaterials.Length != materialCount)
			{
				TMP_TextInfo.Resize(ref m_fontSharedMaterials, materialCount, false);
			}
			for (int i = 0; i < materialCount; i++)
			{
				if (i == 0)
				{
					m_fontSharedMaterials[i] = m_sharedMaterial;
				}
				else
				{
					m_fontSharedMaterials[i] = m_subTextObjects[i].sharedMaterial;
				}
			}
			return m_fontSharedMaterials;
		}

		protected override void SetSharedMaterials(Material[] materials)
		{
			int materialCount = m_textInfo.materialCount;
			if (m_fontSharedMaterials == null)
			{
				m_fontSharedMaterials = new Material[materialCount];
			}
			else if (m_fontSharedMaterials.Length != materialCount)
			{
				TMP_TextInfo.Resize(ref m_fontSharedMaterials, materialCount, false);
			}
			for (int i = 0; i < materialCount; i++)
			{
				if (i == 0)
				{
					if (!(materials[i].mainTexture == null) && materials[i].mainTexture.GetInstanceID() == m_sharedMaterial.mainTexture.GetInstanceID())
					{
						m_sharedMaterial = (m_fontSharedMaterials[i] = materials[i]);
						m_padding = GetPaddingForMaterial(m_sharedMaterial);
					}
				}
				else if (!(materials[i].mainTexture == null) && materials[i].mainTexture.GetInstanceID() == m_subTextObjects[i].sharedMaterial.mainTexture.GetInstanceID() && m_subTextObjects[i].isDefaultMaterial)
				{
					m_subTextObjects[i].sharedMaterial = (m_fontSharedMaterials[i] = materials[i]);
				}
			}
		}

		protected override void SetOutlineThickness(float thickness)
		{
			if (m_fontMaterial != null && m_sharedMaterial.GetInstanceID() != m_fontMaterial.GetInstanceID())
			{
				m_sharedMaterial = m_fontMaterial;
				m_canvasRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			}
			else if (m_fontMaterial == null)
			{
				m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
				m_sharedMaterial = m_fontMaterial;
				m_canvasRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			}
			thickness = Mathf.Clamp01(thickness);
			m_sharedMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
			m_padding = GetPaddingForMaterial();
		}

		protected override void SetFaceColor(Color32 color)
		{
			if (m_fontMaterial == null)
			{
				m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
			}
			m_sharedMaterial = m_fontMaterial;
			m_padding = GetPaddingForMaterial();
			m_sharedMaterial.SetColor(ShaderUtilities.ID_FaceColor, color);
		}

		protected override void SetOutlineColor(Color32 color)
		{
			if (m_fontMaterial == null)
			{
				m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
			}
			m_sharedMaterial = m_fontMaterial;
			m_padding = GetPaddingForMaterial();
			m_sharedMaterial.SetColor(ShaderUtilities.ID_OutlineColor, color);
		}

		protected override void SetShaderDepth()
		{
			if (!(m_canvas == null) && !(m_sharedMaterial == null))
			{
				if (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || m_isOverlay)
				{
					m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0f);
				}
				else
				{
					m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 4f);
				}
			}
		}

		protected override void SetCulling()
		{
			if (m_isCullingEnabled)
			{
				m_canvasRenderer.GetMaterial().SetFloat("_CullMode", 2f);
			}
			else
			{
				m_canvasRenderer.GetMaterial().SetFloat("_CullMode", 0f);
			}
		}

		private void SetPerspectiveCorrection()
		{
			if (m_isOrthographic)
			{
				m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0f);
			}
			else
			{
				m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
			}
		}

		protected override float GetPaddingForMaterial(Material mat)
		{
			m_padding = ShaderUtilities.GetPadding(mat, m_enableExtraPadding, m_isUsingBold);
			m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
			m_isSDFShader = mat.HasProperty(ShaderUtilities.ID_WeightNormal);
			return m_padding;
		}

		protected override float GetPaddingForMaterial()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
			m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
			m_isSDFShader = m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal);
			return m_padding;
		}

		private void SetMeshArrays(int size)
		{
			m_textInfo.meshInfo[0].ResizeMeshInfo(size);
			m_canvasRenderer.SetMesh(m_textInfo.meshInfo[0].mesh);
		}

		protected override int SetArraySizes(int[] chars)
		{
			int endIndex = 0;
			int num = 0;
			m_totalCharacterCount = 0;
			m_isUsingBold = false;
			m_isParsingText = false;
			tag_NoParsing = false;
			m_style = m_fontStyle;
			m_fontWeightInternal = (((m_style & FontStyles.Bold) != FontStyles.Bold) ? m_fontWeight : 700);
			m_fontWeightStack.SetDefault(m_fontWeightInternal);
			m_currentFontAsset = m_fontAsset;
			m_currentMaterial = m_sharedMaterial;
			m_currentMaterialIndex = 0;
			m_materialReferenceStack.SetDefault(new MaterialReference(0, m_currentFontAsset, null, m_currentMaterial, m_padding));
			m_materialReferenceIndexLookup.Clear();
			MaterialReference.AddMaterialReference(m_currentMaterial, m_currentFontAsset, m_materialReferences, m_materialReferenceIndexLookup);
			if (m_textInfo == null)
			{
				m_textInfo = new TMP_TextInfo();
			}
			m_textElementType = TMP_TextElementType.Character;
			for (int i = 0; chars[i] != 0; i++)
			{
				if (m_textInfo.characterInfo == null || m_totalCharacterCount >= m_textInfo.characterInfo.Length)
				{
					TMP_TextInfo.Resize(ref m_textInfo.characterInfo, m_totalCharacterCount + 1, true);
				}
				int num2 = chars[i];
				if (m_isRichText && num2 == 60)
				{
					int currentMaterialIndex = m_currentMaterialIndex;
					if (ValidateHtmlTag(chars, i + 1, out endIndex))
					{
						i = endIndex;
						if ((m_style & FontStyles.Bold) == FontStyles.Bold)
						{
							m_isUsingBold = true;
						}
						if (m_textElementType == TMP_TextElementType.Sprite)
						{
							m_materialReferences[m_currentMaterialIndex].referenceCount++;
							m_textInfo.characterInfo[m_totalCharacterCount].character = (char)(57344 + m_spriteIndex);
							m_textInfo.characterInfo[m_totalCharacterCount].fontAsset = m_currentFontAsset;
							m_textInfo.characterInfo[m_totalCharacterCount].materialReferenceIndex = m_currentMaterialIndex;
							m_textElementType = TMP_TextElementType.Character;
							m_currentMaterialIndex = currentMaterialIndex;
							num++;
							m_totalCharacterCount++;
						}
						continue;
					}
				}
				bool flag = false;
				bool isUsingAlternateTypeface = false;
				TMP_FontAsset currentFontAsset = m_currentFontAsset;
				Material currentMaterial = m_currentMaterial;
				int currentMaterialIndex2 = m_currentMaterialIndex;
				if (m_textElementType == TMP_TextElementType.Character)
				{
					if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
					{
						if (char.IsLower((char)num2))
						{
							num2 = char.ToUpper((char)num2);
						}
					}
					else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
					{
						if (char.IsUpper((char)num2))
						{
							num2 = char.ToLower((char)num2);
						}
					}
					else if (((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num2))
					{
						num2 = char.ToUpper((char)num2);
					}
				}
				TMP_FontAsset fontAssetForWeight = GetFontAssetForWeight(m_fontWeightInternal);
				if (fontAssetForWeight != null)
				{
					flag = true;
					isUsingAlternateTypeface = true;
					m_currentFontAsset = fontAssetForWeight;
					m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(m_currentMaterial, m_currentFontAsset.material.GetTexture(ShaderUtilities.ID_MainTex));
				}
				TMP_Glyph value;
				if (!m_currentFontAsset.characterDictionary.TryGetValue(num2, out value))
				{
					if (m_currentFontAsset.fallbackFontAssets != null && m_currentFontAsset.fallbackFontAssets.Count > 0)
					{
						for (int j = 0; j < m_currentFontAsset.fallbackFontAssets.Count; j++)
						{
							fontAssetForWeight = m_currentFontAsset.fallbackFontAssets[j];
							if (!(fontAssetForWeight == null) && fontAssetForWeight.characterDictionary.TryGetValue(num2, out value))
							{
								flag = true;
								m_currentFontAsset = fontAssetForWeight;
								m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(m_currentMaterial, m_currentFontAsset.material.GetTexture(ShaderUtilities.ID_MainTex));
								break;
							}
						}
					}
					if (value == null && TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
					{
						for (int k = 0; k < TMP_Settings.fallbackFontAssets.Count; k++)
						{
							fontAssetForWeight = TMP_Settings.fallbackFontAssets[k];
							if (!(fontAssetForWeight == null) && fontAssetForWeight.characterDictionary.TryGetValue(num2, out value))
							{
								flag = true;
								m_currentFontAsset = fontAssetForWeight;
								m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(m_currentMaterial, m_currentFontAsset.material.GetTexture(ShaderUtilities.ID_MainTex));
								break;
							}
						}
					}
					if (value == null)
					{
						if (char.IsLower((char)num2))
						{
							if (m_currentFontAsset.characterDictionary.TryGetValue(char.ToUpper((char)num2), out value))
							{
								num2 = (chars[i] = char.ToUpper((char)num2));
							}
						}
						else if (char.IsUpper((char)num2) && m_currentFontAsset.characterDictionary.TryGetValue(char.ToLower((char)num2), out value))
						{
							num2 = (chars[i] = char.ToLower((char)num2));
						}
					}
					if (value == null)
					{
						int num3 = ((TMP_Settings.missingGlyphCharacter != 0) ? TMP_Settings.missingGlyphCharacter : 9633);
						if (m_currentFontAsset.characterDictionary.TryGetValue(num3, out value))
						{
							if (!TMP_Settings.warningsDisabled)
							{
								Debug.LogWarning("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table.", this);
							}
							num2 = (chars[i] = num3);
						}
						else
						{
							if (TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
							{
								for (int l = 0; l < TMP_Settings.fallbackFontAssets.Count; l++)
								{
									fontAssetForWeight = TMP_Settings.fallbackFontAssets[l];
									if (!(fontAssetForWeight == null) && fontAssetForWeight.characterDictionary.TryGetValue(num3, out value))
									{
										if (!TMP_Settings.warningsDisabled)
										{
											Debug.LogWarning("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table.", this);
										}
										num2 = (chars[i] = num3);
										flag = true;
										m_currentFontAsset = fontAssetForWeight;
										m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(m_currentMaterial, m_currentFontAsset.material.GetTexture(ShaderUtilities.ID_MainTex));
										break;
									}
								}
							}
							if (value == null)
							{
								fontAssetForWeight = TMP_Settings.GetFontAsset();
								if (fontAssetForWeight != null && fontAssetForWeight.characterDictionary.TryGetValue(num3, out value))
								{
									if (!TMP_Settings.warningsDisabled)
									{
										Debug.LogWarning("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table.", this);
									}
									num2 = (chars[i] = num3);
									flag = true;
									m_currentFontAsset = fontAssetForWeight;
									m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(m_currentMaterial, m_currentFontAsset.material.GetTexture(ShaderUtilities.ID_MainTex));
								}
								else
								{
									fontAssetForWeight = TMP_FontAsset.defaultFontAsset;
									if (fontAssetForWeight != null && fontAssetForWeight.characterDictionary.TryGetValue(num3, out value))
									{
										if (!TMP_Settings.warningsDisabled)
										{
											Debug.LogWarning("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table.", this);
										}
										num2 = (chars[i] = num3);
										flag = true;
										m_currentFontAsset = fontAssetForWeight;
										m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(m_currentMaterial, m_currentFontAsset.material.GetTexture(ShaderUtilities.ID_MainTex));
									}
									else if (m_currentFontAsset.characterDictionary.TryGetValue(32, out value))
									{
										if (!TMP_Settings.warningsDisabled)
										{
											Debug.LogWarning("Character with ASCII value of " + num2 + " was not found in the Font Asset Glyph Table. It was replaced by a space.", this);
										}
										num2 = (chars[i] = 32);
									}
								}
							}
						}
					}
				}
				m_textInfo.characterInfo[m_totalCharacterCount].textElement = value;
				m_textInfo.characterInfo[m_totalCharacterCount].isUsingAlternateTypeface = isUsingAlternateTypeface;
				m_textInfo.characterInfo[m_totalCharacterCount].character = (char)num2;
				m_textInfo.characterInfo[m_totalCharacterCount].fontAsset = m_currentFontAsset;
				m_textInfo.characterInfo[m_totalCharacterCount].material = m_currentMaterial;
				if (flag)
				{
					m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, m_currentFontAsset, m_materialReferences, m_materialReferenceIndexLookup);
				}
				m_textInfo.characterInfo[m_totalCharacterCount].materialReferenceIndex = m_currentMaterialIndex;
				m_materialReferences[m_currentMaterialIndex].isFallbackFont = flag;
				if (!char.IsWhiteSpace((char)num2))
				{
					m_materialReferences[m_currentMaterialIndex].referenceCount++;
				}
				if (flag)
				{
					m_currentFontAsset = currentFontAsset;
					m_currentMaterial = currentMaterial;
					m_currentMaterialIndex = currentMaterialIndex2;
				}
				m_totalCharacterCount++;
			}
			m_textInfo.spriteCount = num;
			int num4 = (m_textInfo.materialCount = m_materialReferenceIndexLookup.Count);
			if (num4 > m_textInfo.meshInfo.Length)
			{
				TMP_TextInfo.Resize(ref m_textInfo.meshInfo, num4, false);
			}
			if (m_textInfo.characterInfo.Length - m_totalCharacterCount > 256)
			{
				TMP_TextInfo.Resize(ref m_textInfo.characterInfo, Mathf.Max(m_totalCharacterCount + 1, 256), true);
			}
			for (int m = 0; m < num4; m++)
			{
				if (m > 0)
				{
					if (m_subTextObjects[m] == null)
					{
						m_subTextObjects[m] = TMP_SubMeshUI.AddSubTextObject(this, m_materialReferences[m]);
						m_textInfo.meshInfo[m].vertices = null;
					}
					if (m_rectTransform.pivot != m_subTextObjects[m].rectTransform.pivot)
					{
						m_subTextObjects[m].rectTransform.pivot = m_rectTransform.pivot;
					}
					if (m_subTextObjects[m].sharedMaterial == null || m_subTextObjects[m].sharedMaterial.GetInstanceID() != m_materialReferences[m].material.GetInstanceID())
					{
						bool isDefaultMaterial = m_materialReferences[m].isDefaultMaterial;
						m_subTextObjects[m].isDefaultMaterial = isDefaultMaterial;
						if (!isDefaultMaterial || m_subTextObjects[m].sharedMaterial == null || m_subTextObjects[m].sharedMaterial.mainTexture.GetInstanceID() != m_materialReferences[m].material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
						{
							m_subTextObjects[m].sharedMaterial = m_materialReferences[m].material;
							m_subTextObjects[m].fontAsset = m_materialReferences[m].fontAsset;
							m_subTextObjects[m].spriteAsset = m_materialReferences[m].spriteAsset;
						}
					}
					if (m_materialReferences[m].isFallbackFont)
					{
						Material material = m_materialReferences[m].material;
						Material fallbackMaterial = m_subTextObjects[m].m_fallbackMaterial;
						if (material != fallbackMaterial)
						{
							TMP_MaterialManager.RemoveFallbackMaterialReference(fallbackMaterial);
							m_subTextObjects[m].m_fallbackMaterial = material;
							TMP_MaterialManager.AddFallbackMaterialReference(material);
						}
					}
				}
				int referenceCount = m_materialReferences[m].referenceCount;
				if (m_textInfo.meshInfo[m].vertices == null || m_textInfo.meshInfo[m].vertices.Length < referenceCount * 4)
				{
					if (m_textInfo.meshInfo[m].vertices == null)
					{
						if (m == 0)
						{
							m_textInfo.meshInfo[m] = new TMP_MeshInfo(m_mesh, referenceCount + 1);
						}
						else
						{
							m_textInfo.meshInfo[m] = new TMP_MeshInfo(m_subTextObjects[m].mesh, referenceCount + 1);
						}
					}
					else
					{
						m_textInfo.meshInfo[m].ResizeMeshInfo((referenceCount <= 1024) ? Mathf.NextPowerOfTwo(referenceCount) : (referenceCount + 256));
					}
				}
				else if (m_textInfo.meshInfo[m].vertices.Length - referenceCount * 4 > 1024)
				{
					m_textInfo.meshInfo[m].ResizeMeshInfo((referenceCount <= 1024) ? Mathf.Max(Mathf.NextPowerOfTwo(referenceCount), 256) : (referenceCount + 256));
				}
			}
			TMP_MaterialManager.CleanupFallbackMaterials();
			for (int n = num4; n < m_subTextObjects.Length + 1 && m_subTextObjects[n] != null; n++)
			{
				if (n < m_textInfo.meshInfo.Length)
				{
					m_subTextObjects[n].canvasRenderer.SetMesh(null);
				}
			}
			return m_totalCharacterCount;
		}

		protected override void ComputeMarginSize()
		{
			if (base.rectTransform != null)
			{
				m_marginWidth = m_rectTransform.rect.width - m_margin.x - m_margin.z;
				m_marginHeight = m_rectTransform.rect.height - m_margin.y - m_margin.w;
				m_RectTransformCorners = GetTextContainerLocalCorners();
			}
		}

		protected override void OnDidApplyAnimationProperties()
		{
			m_havePropertiesChanged = true;
			SetVerticesDirty();
			SetLayoutDirty();
		}

		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			m_canvas = base.canvas;
			ComputeMarginSize();
			m_havePropertiesChanged = true;
		}

		protected override void OnRectTransformDimensionsChange()
		{
			if (base.gameObject.activeInHierarchy)
			{
				ComputeMarginSize();
				UpdateSubObjectPivot();
				SetVerticesDirty();
				SetLayoutDirty();
			}
		}

		private void LateUpdate()
		{
			if (m_rectTransform.hasChanged)
			{
				float y = m_rectTransform.lossyScale.y;
				if (!m_havePropertiesChanged && y != m_previousLossyScaleY && m_text != string.Empty && m_text != null)
				{
					UpdateSDFScale(y);
					m_previousLossyScaleY = y;
				}
				m_rectTransform.hasChanged = false;
			}
		}

		private void OnPreRenderCanvas()
		{
			if (!m_isAwake || (!m_ignoreActiveState && !IsActive()))
			{
				return;
			}
			if (m_canvas == null)
			{
				m_canvas = base.canvas;
				if (m_canvas == null)
				{
					return;
				}
			}
			loopCountA = 0;
			if (m_havePropertiesChanged || m_isLayoutDirty)
			{
				if (checkPaddingRequired)
				{
					UpdateMeshPadding();
				}
				if (m_isInputParsingRequired || m_isTextTruncated)
				{
					ParseInputText();
				}
				if (m_enableAutoSizing)
				{
					m_fontSize = Mathf.Clamp(m_fontSize, m_fontSizeMin, m_fontSizeMax);
				}
				m_maxFontSize = m_fontSizeMax;
				m_minFontSize = m_fontSizeMin;
				m_lineSpacingDelta = 0f;
				m_charWidthAdjDelta = 0f;
				m_recursiveCount = 0;
				m_isCharacterWrappingEnabled = false;
				m_isTextTruncated = false;
				m_havePropertiesChanged = false;
				m_isLayoutDirty = false;
				m_ignoreActiveState = false;
				GenerateTextMesh();
			}
		}

		protected override void GenerateTextMesh()
		{
			if (m_fontAsset == null || m_fontAsset.characterDictionary == null)
			{
				Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + GetInstanceID());
				return;
			}
			if (m_textInfo != null)
			{
				m_textInfo.Clear();
			}
			if (m_char_buffer == null || m_char_buffer.Length == 0 || m_char_buffer[0] == 0)
			{
				ClearMesh();
				m_preferredWidth = 0f;
				m_preferredHeight = 0f;
				return;
			}
			m_currentFontAsset = m_fontAsset;
			m_currentMaterial = m_sharedMaterial;
			m_currentMaterialIndex = 0;
			m_materialReferenceStack.SetDefault(new MaterialReference(0, m_currentFontAsset, null, m_currentMaterial, m_padding));
			m_currentSpriteAsset = m_spriteAsset;
			int totalCharacterCount = m_totalCharacterCount;
			m_fontScale = m_fontSize / m_currentFontAsset.fontInfo.PointSize;
			float num = m_fontSize / m_fontAsset.fontInfo.PointSize * m_fontAsset.fontInfo.Scale;
			float num2 = m_fontScale;
			m_fontScaleMultiplier = 1f;
			m_currentFontSize = m_fontSize;
			m_sizeStack.SetDefault(m_currentFontSize);
			float num3 = 0f;
			int num4 = 0;
			m_style = m_fontStyle;
			m_fontWeightInternal = (((m_style & FontStyles.Bold) != FontStyles.Bold) ? m_fontWeight : 700);
			m_fontWeightStack.SetDefault(m_fontWeightInternal);
			m_lineJustification = m_textAlignment;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 1f;
			m_baselineOffset = 0f;
			bool flag = false;
			Vector3 start = Vector3.zero;
			Vector3 zero = Vector3.zero;
			bool flag2 = false;
			Vector3 start2 = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			m_fontColor32 = m_fontColor;
			m_htmlColor = m_fontColor32;
			m_colorStack.SetDefault(m_htmlColor);
			m_styleStack.Clear();
			m_actionStack.Clear();
			m_lineOffset = 0f;
			m_lineHeight = 0f;
			float num8 = m_currentFontAsset.fontInfo.LineHeight - (m_currentFontAsset.fontInfo.Ascender - m_currentFontAsset.fontInfo.Descender);
			m_cSpacing = 0f;
			m_monoSpacing = 0f;
			float num9 = 0f;
			m_xAdvance = 0f;
			tag_LineIndent = 0f;
			tag_Indent = 0f;
			m_indentStack.SetDefault(0f);
			tag_NoParsing = false;
			m_characterCount = 0;
			m_firstCharacterOfLine = 0;
			m_lastCharacterOfLine = 0;
			m_firstVisibleCharacterOfLine = 0;
			m_lastVisibleCharacterOfLine = 0;
			m_maxLineAscender = float.NegativeInfinity;
			m_maxLineDescender = float.PositiveInfinity;
			m_lineNumber = 0;
			m_lineVisibleCharacterCount = 0;
			bool flag3 = true;
			m_pageNumber = 0;
			int num10 = Mathf.Clamp(m_pageToDisplay - 1, 0, m_textInfo.pageInfo.Length - 1);
			int num11 = 0;
			Vector4 margin = m_margin;
			float marginWidth = m_marginWidth;
			float marginHeight = m_marginHeight;
			m_marginLeft = 0f;
			m_marginRight = 0f;
			m_width = -1f;
			float num12 = marginWidth + 0.0001f - m_marginLeft - m_marginRight;
			m_meshExtents.min = TMP_Text.k_InfinityVectorPositive;
			m_meshExtents.max = TMP_Text.k_InfinityVectorNegative;
			m_textInfo.ClearLineInfo();
			m_maxAscender = 0f;
			m_maxDescender = 0f;
			float num13 = 0f;
			float num14 = 0f;
			bool flag4 = false;
			m_isNewPage = false;
			bool flag5 = true;
			bool flag6 = false;
			int num15 = 0;
			loopCountA++;
			int endIndex = 0;
			Vector3 topLeft = default(Vector3);
			Vector3 bottomLeft = default(Vector3);
			Vector3 topRight = default(Vector3);
			Vector3 bottomRight = default(Vector3);
			for (int i = 0; m_char_buffer[i] != 0; i++)
			{
				num4 = m_char_buffer[i];
				m_textElementType = TMP_TextElementType.Character;
				m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;
				m_currentFontAsset = m_materialReferences[m_currentMaterialIndex].fontAsset;
				int currentMaterialIndex = m_currentMaterialIndex;
				if (m_isRichText && num4 == 60)
				{
					m_isParsingText = true;
					if (ValidateHtmlTag(m_char_buffer, i + 1, out endIndex))
					{
						i = endIndex;
						if (m_textElementType == TMP_TextElementType.Character)
						{
							continue;
						}
					}
				}
				m_isParsingText = false;
				bool isUsingAlternateTypeface = m_textInfo.characterInfo[m_characterCount].isUsingAlternateTypeface;
				float num16 = 1f;
				if (m_textElementType == TMP_TextElementType.Character)
				{
					if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
					{
						if (char.IsLower((char)num4))
						{
							num4 = char.ToUpper((char)num4);
						}
					}
					else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
					{
						if (char.IsUpper((char)num4))
						{
							num4 = char.ToLower((char)num4);
						}
					}
					else if (((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num4))
					{
						num16 = 0.8f;
						num4 = char.ToUpper((char)num4);
					}
				}
				if (m_textElementType == TMP_TextElementType.Sprite)
				{
					TMP_Sprite tMP_Sprite = m_currentSpriteAsset.spriteInfoList[m_spriteIndex];
					if (tMP_Sprite == null)
					{
						continue;
					}
					num4 = 57344 + m_spriteIndex;
					m_currentFontAsset = m_fontAsset;
					float num17 = m_currentFontSize / m_fontAsset.fontInfo.PointSize * m_fontAsset.fontInfo.Scale;
					num2 = m_fontAsset.fontInfo.Ascender / tMP_Sprite.height * tMP_Sprite.scale * num17;
					m_cached_TextElement = tMP_Sprite;
					m_textInfo.characterInfo[m_characterCount].elementType = TMP_TextElementType.Sprite;
					m_textInfo.characterInfo[m_characterCount].scale = num17;
					m_textInfo.characterInfo[m_characterCount].spriteAsset = m_currentSpriteAsset;
					m_textInfo.characterInfo[m_characterCount].fontAsset = m_currentFontAsset;
					m_textInfo.characterInfo[m_characterCount].materialReferenceIndex = m_currentMaterialIndex;
					m_currentMaterialIndex = currentMaterialIndex;
					num5 = 0f;
				}
				else if (m_textElementType == TMP_TextElementType.Character)
				{
					m_cached_TextElement = m_textInfo.characterInfo[m_characterCount].textElement;
					if (m_cached_TextElement == null)
					{
						continue;
					}
					m_currentFontAsset = m_textInfo.characterInfo[m_characterCount].fontAsset;
					m_currentMaterial = m_textInfo.characterInfo[m_characterCount].material;
					m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;
					m_fontScale = m_currentFontSize * num16 / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale;
					num2 = m_fontScale * m_fontScaleMultiplier;
					m_textInfo.characterInfo[m_characterCount].elementType = TMP_TextElementType.Character;
					m_textInfo.characterInfo[m_characterCount].scale = num2;
					num5 = ((m_currentMaterialIndex != 0) ? m_subTextObjects[m_currentMaterialIndex].padding : m_padding);
				}
				float num18 = num2;
				if (num4 == 173)
				{
					num2 = 0f;
				}
				if (m_isRightToLeft)
				{
					m_xAdvance -= ((m_cached_TextElement.xAdvance * num7 + m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing) * (1f - m_charWidthAdjDelta);
				}
				m_textInfo.characterInfo[m_characterCount].character = (char)num4;
				m_textInfo.characterInfo[m_characterCount].pointSize = m_currentFontSize;
				m_textInfo.characterInfo[m_characterCount].color = m_htmlColor;
				m_textInfo.characterInfo[m_characterCount].style = m_style;
				m_textInfo.characterInfo[m_characterCount].index = (short)i;
				if (m_enableKerning && m_characterCount >= 1)
				{
					int character = m_textInfo.characterInfo[m_characterCount - 1].character;
					KerningPairKey kerningPairKey = new KerningPairKey(character, num4);
					KerningPair value;
					m_currentFontAsset.kerningDictionary.TryGetValue(kerningPairKey.key, out value);
					if (value != null)
					{
						m_xAdvance += value.XadvanceOffset * num2;
					}
				}
				float num19 = 0f;
				if (m_monoSpacing != 0f)
				{
					num19 = (m_monoSpacing / 2f - (m_cached_TextElement.width / 2f + m_cached_TextElement.xOffset) * num2) * (1f - m_charWidthAdjDelta);
					m_xAdvance += num19;
				}
				if (m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && ((m_style & FontStyles.Bold) == FontStyles.Bold || (m_fontStyle & FontStyles.Bold) == FontStyles.Bold))
				{
					num6 = m_currentFontAsset.boldStyle * 2f;
					num7 = 1f + m_currentFontAsset.boldSpacing * 0.01f;
				}
				else
				{
					num6 = m_currentFontAsset.normalStyle * 2f;
					num7 = 1f;
				}
				float baseline = m_currentFontAsset.fontInfo.Baseline;
				topLeft.x = m_xAdvance + (m_cached_TextElement.xOffset - num5 - num6) * num2 * (1f - m_charWidthAdjDelta);
				topLeft.y = (baseline + m_cached_TextElement.yOffset + num5) * num2 - m_lineOffset + m_baselineOffset;
				topLeft.z = 0f;
				bottomLeft.x = topLeft.x;
				bottomLeft.y = topLeft.y - (m_cached_TextElement.height + num5 * 2f) * num2;
				bottomLeft.z = 0f;
				topRight.x = bottomLeft.x + (m_cached_TextElement.width + num5 * 2f + num6 * 2f) * num2 * (1f - m_charWidthAdjDelta);
				topRight.y = topLeft.y;
				topRight.z = 0f;
				bottomRight.x = topRight.x;
				bottomRight.y = bottomLeft.y;
				bottomRight.z = 0f;
				if (m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && ((m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic))
				{
					float num20 = (float)(int)m_currentFontAsset.italicStyle * 0.01f;
					Vector3 vector = new Vector3(num20 * ((m_cached_TextElement.yOffset + num5 + num6) * num2), 0f, 0f);
					Vector3 vector2 = new Vector3(num20 * ((m_cached_TextElement.yOffset - m_cached_TextElement.height - num5 - num6) * num2), 0f, 0f);
					topLeft += vector;
					bottomLeft += vector2;
					topRight += vector;
					bottomRight += vector2;
				}
				m_textInfo.characterInfo[m_characterCount].bottomLeft = bottomLeft;
				m_textInfo.characterInfo[m_characterCount].topLeft = topLeft;
				m_textInfo.characterInfo[m_characterCount].topRight = topRight;
				m_textInfo.characterInfo[m_characterCount].bottomRight = bottomRight;
				m_textInfo.characterInfo[m_characterCount].origin = m_xAdvance;
				m_textInfo.characterInfo[m_characterCount].baseLine = 0f - m_lineOffset + m_baselineOffset;
				m_textInfo.characterInfo[m_characterCount].aspectRatio = (topRight.x - bottomLeft.x) / (topLeft.y - bottomLeft.y);
				float num21 = m_currentFontAsset.fontInfo.Ascender * ((m_textElementType != 0) ? m_textInfo.characterInfo[m_characterCount].scale : num2) + m_baselineOffset;
				m_textInfo.characterInfo[m_characterCount].ascender = num21 - m_lineOffset;
				m_maxLineAscender = ((!(num21 > m_maxLineAscender)) ? m_maxLineAscender : num21);
				float num22 = m_currentFontAsset.fontInfo.Descender * ((m_textElementType != 0) ? m_textInfo.characterInfo[m_characterCount].scale : num2) + m_baselineOffset;
				float num23 = (m_textInfo.characterInfo[m_characterCount].descender = num22 - m_lineOffset);
				m_maxLineDescender = ((!(num22 < m_maxLineDescender)) ? m_maxLineDescender : num22);
				if ((m_style & FontStyles.Subscript) == FontStyles.Subscript || (m_style & FontStyles.Superscript) == FontStyles.Superscript)
				{
					float num24 = (num21 - m_baselineOffset) / m_currentFontAsset.fontInfo.SubSize;
					num21 = m_maxLineAscender;
					m_maxLineAscender = ((!(num24 > m_maxLineAscender)) ? m_maxLineAscender : num24);
					float num25 = (num22 - m_baselineOffset) / m_currentFontAsset.fontInfo.SubSize;
					num22 = m_maxLineDescender;
					m_maxLineDescender = ((!(num25 < m_maxLineDescender)) ? m_maxLineDescender : num25);
				}
				if (m_lineNumber == 0)
				{
					m_maxAscender = ((!(m_maxAscender > num21)) ? num21 : m_maxAscender);
				}
				if (m_lineOffset == 0f)
				{
					num13 = ((!(num13 > num21)) ? num21 : num13);
				}
				m_textInfo.characterInfo[m_characterCount].isVisible = false;
				if (num4 == 9 || !char.IsWhiteSpace((char)num4) || m_textElementType == TMP_TextElementType.Sprite)
				{
					m_textInfo.characterInfo[m_characterCount].isVisible = true;
					num12 = ((m_width == -1f) ? (marginWidth + 0.0001f - m_marginLeft - m_marginRight) : Mathf.Min(marginWidth + 0.0001f - m_marginLeft - m_marginRight, m_width));
					m_textInfo.lineInfo[m_lineNumber].marginLeft = m_marginLeft;
					if (Mathf.Abs(m_xAdvance) + (m_isRightToLeft ? 0f : m_cached_TextElement.xAdvance) * (1f - m_charWidthAdjDelta) * ((num4 == 173) ? num18 : num2) > num12)
					{
						num11 = m_characterCount - 1;
						if (base.enableWordWrapping && m_characterCount != m_firstCharacterOfLine)
						{
							if (num15 == m_SavedWordWrapState.previous_WordBreak || flag5)
							{
								if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
								{
									if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100f)
									{
										loopCountA = 0;
										m_charWidthAdjDelta += 0.01f;
										GenerateTextMesh();
										return;
									}
									m_maxFontSize = m_fontSize;
									m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.05f);
									m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20f + 0.5f) / 20f;
									if (loopCountA <= 20)
									{
										GenerateTextMesh();
									}
									return;
								}
								if (!m_isCharacterWrappingEnabled)
								{
									m_isCharacterWrappingEnabled = true;
								}
								else
								{
									flag6 = true;
								}
								m_recursiveCount++;
								if (m_recursiveCount > 20)
								{
									continue;
								}
							}
							i = RestoreWordWrappingState(ref m_SavedWordWrapState);
							num15 = i;
							if (m_char_buffer[i] == 173)
							{
								m_isTextTruncated = true;
								m_char_buffer[i] = 45;
								GenerateTextMesh();
								return;
							}
							if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f && !m_isNewPage)
							{
								float num26 = m_maxLineAscender - m_startOfLineAscender;
								AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num26);
								m_lineOffset += num26;
								m_SavedWordWrapState.lineOffset = m_lineOffset;
								m_SavedWordWrapState.previousLineAscender = m_maxLineAscender;
							}
							m_isNewPage = false;
							float num27 = m_maxLineAscender - m_lineOffset;
							float num28 = m_maxLineDescender - m_lineOffset;
							m_maxDescender = ((!(m_maxDescender < num28)) ? num28 : m_maxDescender);
							if (!flag4)
							{
								num14 = m_maxDescender;
							}
							if (m_useMaxVisibleDescender && (m_characterCount >= m_maxVisibleCharacters || m_lineNumber >= m_maxVisibleLines))
							{
								flag4 = true;
							}
							m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstCharacterOfLine;
							m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex = (m_firstVisibleCharacterOfLine = ((m_firstCharacterOfLine <= m_firstVisibleCharacterOfLine) ? m_firstVisibleCharacterOfLine : m_firstCharacterOfLine));
							m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = (m_lastCharacterOfLine = ((m_characterCount - 1 > 0) ? (m_characterCount - 1) : 0));
							m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = (m_lastVisibleCharacterOfLine = ((m_lastVisibleCharacterOfLine >= m_firstVisibleCharacterOfLine) ? m_lastVisibleCharacterOfLine : m_firstVisibleCharacterOfLine));
							m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;
							m_textInfo.lineInfo[m_lineNumber].visibleCharacterCount = m_lineVisibleCharacterCount;
							m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, num28);
							m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, num27);
							m_textInfo.lineInfo[m_lineNumber].length = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x;
							m_textInfo.lineInfo[m_lineNumber].width = num12;
							m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 - m_cSpacing;
							m_textInfo.lineInfo[m_lineNumber].baseline = 0f - m_lineOffset;
							m_textInfo.lineInfo[m_lineNumber].ascender = num27;
							m_textInfo.lineInfo[m_lineNumber].descender = num28;
							m_textInfo.lineInfo[m_lineNumber].lineHeight = num27 - num28 + num8 * num;
							m_firstCharacterOfLine = m_characterCount;
							m_lineVisibleCharacterCount = 0;
							SaveWordWrappingState(ref m_SavedLineState, i, m_characterCount - 1);
							m_lineNumber++;
							flag3 = true;
							if (m_lineNumber >= m_textInfo.lineInfo.Length)
							{
								ResizeLineExtents(m_lineNumber);
							}
							if (m_lineHeight == 0f)
							{
								float num29 = m_textInfo.characterInfo[m_characterCount].ascender - m_textInfo.characterInfo[m_characterCount].baseLine;
								num9 = 0f - m_maxLineDescender + num29 + (num8 + m_lineSpacing + m_lineSpacingDelta) * num;
								m_lineOffset += num9;
								m_startOfLineAscender = num29;
							}
							else
							{
								m_lineOffset += m_lineHeight + m_lineSpacing * num;
							}
							m_maxLineAscender = float.NegativeInfinity;
							m_maxLineDescender = float.PositiveInfinity;
							m_xAdvance = 0f + tag_Indent;
							continue;
						}
						if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
						{
							if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100f)
							{
								loopCountA = 0;
								m_charWidthAdjDelta += 0.01f;
								GenerateTextMesh();
								return;
							}
							m_maxFontSize = m_fontSize;
							m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.05f);
							m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20f + 0.5f) / 20f;
							m_recursiveCount = 0;
							if (loopCountA <= 20)
							{
								GenerateTextMesh();
							}
							return;
						}
						switch (m_overflowMode)
						{
						case TextOverflowModes.Overflow:
							if (m_isMaskingEnabled)
							{
								DisableMasking();
							}
							break;
						case TextOverflowModes.Ellipsis:
							if (m_isMaskingEnabled)
							{
								DisableMasking();
							}
							m_isTextTruncated = true;
							if (m_characterCount < 1)
							{
								m_textInfo.characterInfo[m_characterCount].isVisible = false;
								break;
							}
							m_char_buffer[i - 1] = 8230;
							m_char_buffer[i] = 0;
							if (m_cached_Ellipsis_GlyphInfo != null)
							{
								m_textInfo.characterInfo[num11].character = '…';
								m_textInfo.characterInfo[num11].textElement = m_cached_Ellipsis_GlyphInfo;
								m_textInfo.characterInfo[num11].fontAsset = m_materialReferences[0].fontAsset;
								m_textInfo.characterInfo[num11].material = m_materialReferences[0].material;
								m_textInfo.characterInfo[num11].materialReferenceIndex = 0;
							}
							else
							{
								Debug.LogWarning("Unable to use Ellipsis character since it wasn't found in the current Font Asset [" + m_fontAsset.name + "]. Consider regenerating this font asset to include the Ellipsis character (u+2026).\nNote: Warnings can be disabled in the TMP Settings file.", this);
							}
							m_totalCharacterCount = num11 + 1;
							GenerateTextMesh();
							return;
						case TextOverflowModes.Masking:
							if (!m_isMaskingEnabled)
							{
								EnableMasking();
							}
							break;
						case TextOverflowModes.ScrollRect:
							if (!m_isMaskingEnabled)
							{
								EnableMasking();
							}
							break;
						case TextOverflowModes.Truncate:
							if (m_isMaskingEnabled)
							{
								DisableMasking();
							}
							m_textInfo.characterInfo[m_characterCount].isVisible = false;
							break;
						}
					}
					if (num4 != 9)
					{
						Color32 vertexColor = ((!m_overrideHtmlColors) ? m_htmlColor : m_fontColor32);
						if (m_textElementType == TMP_TextElementType.Character)
						{
							SaveGlyphVertexInfo(num5, num6, vertexColor);
						}
						else if (m_textElementType == TMP_TextElementType.Sprite)
						{
							SaveSpriteVertexInfo(vertexColor);
						}
					}
					else
					{
						m_textInfo.characterInfo[m_characterCount].isVisible = false;
						m_lastVisibleCharacterOfLine = m_characterCount;
						m_textInfo.lineInfo[m_lineNumber].spaceCount++;
						m_textInfo.spaceCount++;
					}
					if (m_textInfo.characterInfo[m_characterCount].isVisible && num4 != 173)
					{
						if (flag3)
						{
							flag3 = false;
							m_firstVisibleCharacterOfLine = m_characterCount;
						}
						m_lineVisibleCharacterCount++;
						m_lastVisibleCharacterOfLine = m_characterCount;
					}
				}
				else if ((num4 == 10 || char.IsSeparator((char)num4)) && num4 != 173 && num4 != 8203 && num4 != 8288)
				{
					m_textInfo.lineInfo[m_lineNumber].spaceCount++;
					m_textInfo.spaceCount++;
				}
				if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f && !m_isNewPage)
				{
					float num30 = m_maxLineAscender - m_startOfLineAscender;
					AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num30);
					num23 -= num30;
					m_lineOffset += num30;
					m_startOfLineAscender += num30;
					m_SavedWordWrapState.lineOffset = m_lineOffset;
					m_SavedWordWrapState.previousLineAscender = m_startOfLineAscender;
				}
				m_textInfo.characterInfo[m_characterCount].lineNumber = (short)m_lineNumber;
				m_textInfo.characterInfo[m_characterCount].pageNumber = (short)m_pageNumber;
				if ((num4 != 10 && num4 != 13 && num4 != 8230) || m_textInfo.lineInfo[m_lineNumber].characterCount == 1)
				{
					m_textInfo.lineInfo[m_lineNumber].alignment = m_lineJustification;
				}
				if (m_maxAscender - num23 > marginHeight + 0.0001f)
				{
					if (m_enableAutoSizing && m_lineSpacingDelta > m_lineSpacingMax && m_lineNumber > 0)
					{
						m_lineSpacingDelta -= 1f;
						GenerateTextMesh();
						return;
					}
					if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
					{
						m_maxFontSize = m_fontSize;
						m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.05f);
						m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20f + 0.5f) / 20f;
						m_recursiveCount = 0;
						if (loopCountA <= 20)
						{
							GenerateTextMesh();
						}
						return;
					}
					switch (m_overflowMode)
					{
					case TextOverflowModes.Overflow:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						break;
					case TextOverflowModes.Ellipsis:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						if (m_lineNumber > 0)
						{
							m_char_buffer[m_textInfo.characterInfo[num11].index] = 8230;
							m_char_buffer[m_textInfo.characterInfo[num11].index + 1] = 0;
							if (m_cached_Ellipsis_GlyphInfo != null)
							{
								m_textInfo.characterInfo[num11].character = '…';
								m_textInfo.characterInfo[num11].textElement = m_cached_Ellipsis_GlyphInfo;
								m_textInfo.characterInfo[num11].fontAsset = m_materialReferences[0].fontAsset;
								m_textInfo.characterInfo[num11].material = m_materialReferences[0].material;
								m_textInfo.characterInfo[num11].materialReferenceIndex = 0;
							}
							else
							{
								Debug.LogWarning("Unable to use Ellipsis character since it wasn't found in the current Font Asset [" + m_fontAsset.name + "]. Consider regenerating this font asset to include the Ellipsis character (u+2026).\nNote: Warnings can be disabled in the TMP Settings file.", this);
							}
							m_totalCharacterCount = num11 + 1;
							GenerateTextMesh();
							m_isTextTruncated = true;
						}
						else
						{
							ClearMesh();
						}
						return;
					case TextOverflowModes.Masking:
						if (!m_isMaskingEnabled)
						{
							EnableMasking();
						}
						break;
					case TextOverflowModes.ScrollRect:
						if (!m_isMaskingEnabled)
						{
							EnableMasking();
						}
						break;
					case TextOverflowModes.Truncate:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						if (m_lineNumber > 0)
						{
							m_char_buffer[m_textInfo.characterInfo[num11].index + 1] = 0;
							m_totalCharacterCount = num11 + 1;
							GenerateTextMesh();
							m_isTextTruncated = true;
						}
						else
						{
							ClearMesh();
						}
						return;
					case TextOverflowModes.Page:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						if (num4 == 13 || num4 == 10)
						{
							break;
						}
						i = RestoreWordWrappingState(ref m_SavedLineState);
						if (i == 0)
						{
							ClearMesh();
							return;
						}
						m_isNewPage = true;
						m_xAdvance = 0f + tag_Indent;
						m_lineOffset = 0f;
						m_lineNumber++;
						m_pageNumber++;
						continue;
					}
				}
				if (num4 == 9)
				{
					m_xAdvance += m_currentFontAsset.fontInfo.TabWidth * num2;
				}
				else if (m_monoSpacing != 0f)
				{
					m_xAdvance += (m_monoSpacing - num19 + (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing) * (1f - m_charWidthAdjDelta);
				}
				else if (!m_isRightToLeft)
				{
					m_xAdvance += ((m_cached_TextElement.xAdvance * num7 + m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 + m_cSpacing) * (1f - m_charWidthAdjDelta);
				}
				m_textInfo.characterInfo[m_characterCount].xAdvance = m_xAdvance;
				if (num4 == 13)
				{
					m_xAdvance = 0f + tag_Indent;
				}
				if (num4 == 10 || m_characterCount == totalCharacterCount - 1)
				{
					if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == 0f && !m_isNewPage)
					{
						float num31 = m_maxLineAscender - m_startOfLineAscender;
						AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num31);
						num23 -= num31;
						m_lineOffset += num31;
					}
					m_isNewPage = false;
					float num32 = m_maxLineAscender - m_lineOffset;
					float num33 = m_maxLineDescender - m_lineOffset;
					m_maxDescender = ((!(m_maxDescender < num33)) ? num33 : m_maxDescender);
					if (!flag4)
					{
						num14 = m_maxDescender;
					}
					if (m_useMaxVisibleDescender && (m_characterCount >= m_maxVisibleCharacters || m_lineNumber >= m_maxVisibleLines))
					{
						flag4 = true;
					}
					m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstCharacterOfLine;
					m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex = (m_firstVisibleCharacterOfLine = ((m_firstCharacterOfLine <= m_firstVisibleCharacterOfLine) ? m_firstVisibleCharacterOfLine : m_firstCharacterOfLine));
					m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = (m_lastCharacterOfLine = m_characterCount);
					m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = (m_lastVisibleCharacterOfLine = ((m_lastVisibleCharacterOfLine >= m_firstVisibleCharacterOfLine) ? m_lastVisibleCharacterOfLine : m_firstVisibleCharacterOfLine));
					m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;
					m_textInfo.lineInfo[m_lineNumber].visibleCharacterCount = m_lineVisibleCharacterCount;
					m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, num33);
					m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, num32);
					m_textInfo.lineInfo[m_lineNumber].length = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - num5 * num2;
					m_textInfo.lineInfo[m_lineNumber].width = num12;
					if (m_textInfo.lineInfo[m_lineNumber].characterCount == 1)
					{
						m_textInfo.lineInfo[m_lineNumber].alignment = m_lineJustification;
					}
					if (m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].isVisible)
					{
						m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 - m_cSpacing;
					}
					else
					{
						m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastCharacterOfLine].xAdvance - (m_characterSpacing + m_currentFontAsset.normalSpacingOffset) * num2 - m_cSpacing;
					}
					m_textInfo.lineInfo[m_lineNumber].baseline = 0f - m_lineOffset;
					m_textInfo.lineInfo[m_lineNumber].ascender = num32;
					m_textInfo.lineInfo[m_lineNumber].descender = num33;
					m_textInfo.lineInfo[m_lineNumber].lineHeight = num32 - num33 + num8 * num;
					m_firstCharacterOfLine = m_characterCount + 1;
					m_lineVisibleCharacterCount = 0;
					if (num4 == 10)
					{
						SaveWordWrappingState(ref m_SavedLineState, i, m_characterCount);
						SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
						m_lineNumber++;
						flag3 = true;
						if (m_lineNumber >= m_textInfo.lineInfo.Length)
						{
							ResizeLineExtents(m_lineNumber);
						}
						if (m_lineHeight == 0f)
						{
							num9 = 0f - m_maxLineDescender + num21 + (num8 + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * num;
							m_lineOffset += num9;
						}
						else
						{
							m_lineOffset += m_lineHeight + (m_lineSpacing + m_paragraphSpacing) * num;
						}
						m_maxLineAscender = float.NegativeInfinity;
						m_maxLineDescender = float.PositiveInfinity;
						m_startOfLineAscender = num21;
						m_xAdvance = 0f + tag_LineIndent + tag_Indent;
						num11 = m_characterCount - 1;
						m_characterCount++;
						continue;
					}
				}
				if (m_textInfo.characterInfo[m_characterCount].isVisible)
				{
					m_meshExtents.min.x = Mathf.Min(m_meshExtents.min.x, m_textInfo.characterInfo[m_characterCount].bottomLeft.x);
					m_meshExtents.min.y = Mathf.Min(m_meshExtents.min.y, m_textInfo.characterInfo[m_characterCount].bottomLeft.y);
					m_meshExtents.max.x = Mathf.Max(m_meshExtents.max.x, m_textInfo.characterInfo[m_characterCount].topRight.x);
					m_meshExtents.max.y = Mathf.Max(m_meshExtents.max.y, m_textInfo.characterInfo[m_characterCount].topRight.y);
				}
				if (m_overflowMode == TextOverflowModes.Page && num4 != 13 && num4 != 10 && m_pageNumber < 16)
				{
					m_textInfo.pageInfo[m_pageNumber].ascender = num13;
					m_textInfo.pageInfo[m_pageNumber].descender = ((!(num22 < m_textInfo.pageInfo[m_pageNumber].descender)) ? m_textInfo.pageInfo[m_pageNumber].descender : num22);
					if (m_pageNumber == 0 && m_characterCount == 0)
					{
						m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
					}
					else if (m_characterCount > 0 && m_pageNumber != m_textInfo.characterInfo[m_characterCount - 1].pageNumber)
					{
						m_textInfo.pageInfo[m_pageNumber - 1].lastCharacterIndex = m_characterCount - 1;
						m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
					}
					else if (m_characterCount == totalCharacterCount - 1)
					{
						m_textInfo.pageInfo[m_pageNumber].lastCharacterIndex = m_characterCount;
					}
				}
				if (m_enableWordWrapping || m_overflowMode == TextOverflowModes.Truncate || m_overflowMode == TextOverflowModes.Ellipsis)
				{
					if ((char.IsWhiteSpace((char)num4) || num4 == 45 || num4 == 173) && !m_isNonBreakingSpace && num4 != 160 && num4 != 8209 && num4 != 8239 && num4 != 8288)
					{
						SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
						m_isCharacterWrappingEnabled = false;
						flag5 = false;
					}
					else if (((num4 > 4352 && num4 < 4607) || (num4 > 11904 && num4 < 40959) || (num4 > 43360 && num4 < 43391) || (num4 > 44032 && num4 < 55295) || (num4 > 63744 && num4 < 64255) || (num4 > 65072 && num4 < 65103) || (num4 > 65280 && num4 < 65519)) && !m_isNonBreakingSpace)
					{
						if (flag5 || flag6 || (!TMP_Settings.linebreakingRules.leadingCharacters.ContainsKey(num4) && m_characterCount < totalCharacterCount - 1 && !TMP_Settings.linebreakingRules.followingCharacters.ContainsKey(m_textInfo.characterInfo[m_characterCount + 1].character)))
						{
							SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
							m_isCharacterWrappingEnabled = false;
							flag5 = false;
						}
					}
					else if (flag5 || m_isCharacterWrappingEnabled || flag6)
					{
						SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
					}
				}
				m_characterCount++;
			}
			num3 = m_maxFontSize - m_minFontSize;
			if (!m_isCharacterWrappingEnabled && m_enableAutoSizing && num3 > 0.051f && m_fontSize < m_fontSizeMax)
			{
				m_minFontSize = m_fontSize;
				m_fontSize += Mathf.Max((m_maxFontSize - m_fontSize) / 2f, 0.05f);
				m_fontSize = (float)(int)(Mathf.Min(m_fontSize, m_fontSizeMax) * 20f + 0.5f) / 20f;
				if (loopCountA <= 20)
				{
					GenerateTextMesh();
				}
				return;
			}
			m_isCharacterWrappingEnabled = false;
			if (m_characterCount == 0)
			{
				ClearMesh();
				return;
			}
			int index = m_materialReferences[0].referenceCount * 4;
			m_textInfo.meshInfo[0].Clear(false);
			Vector3 a = Vector3.zero;
			Vector3[] rectTransformCorners = m_RectTransformCorners;
			switch (m_textAlignment)
			{
			case TextAlignmentOptions.TopLeft:
			case TextAlignmentOptions.Top:
			case TextAlignmentOptions.TopRight:
			case TextAlignmentOptions.TopJustified:
				a = ((m_overflowMode == TextOverflowModes.Page) ? (rectTransformCorners[1] + new Vector3(0f + margin.x, 0f - m_textInfo.pageInfo[num10].ascender - margin.y, 0f)) : (rectTransformCorners[1] + new Vector3(0f + margin.x, 0f - m_maxAscender - margin.y, 0f)));
				break;
			case TextAlignmentOptions.Left:
			case TextAlignmentOptions.Center:
			case TextAlignmentOptions.Right:
			case TextAlignmentOptions.Justified:
				a = ((m_overflowMode == TextOverflowModes.Page) ? ((rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (m_textInfo.pageInfo[num10].ascender + margin.y + m_textInfo.pageInfo[num10].descender - margin.w) / 2f, 0f)) : ((rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (m_maxAscender + margin.y + num14 - margin.w) / 2f, 0f)));
				break;
			case TextAlignmentOptions.BottomLeft:
			case TextAlignmentOptions.Bottom:
			case TextAlignmentOptions.BottomRight:
			case TextAlignmentOptions.BottomJustified:
				a = ((m_overflowMode == TextOverflowModes.Page) ? (rectTransformCorners[0] + new Vector3(0f + margin.x, 0f - m_textInfo.pageInfo[num10].descender + margin.w, 0f)) : (rectTransformCorners[0] + new Vector3(0f + margin.x, 0f - num14 + margin.w, 0f)));
				break;
			case TextAlignmentOptions.BaselineLeft:
			case TextAlignmentOptions.Baseline:
			case TextAlignmentOptions.BaselineRight:
			case TextAlignmentOptions.BaselineJustified:
				a = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f, 0f);
				break;
			case TextAlignmentOptions.MidlineLeft:
			case TextAlignmentOptions.Midline:
			case TextAlignmentOptions.MidlineRight:
			case TextAlignmentOptions.MidlineJustified:
				a = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (m_meshExtents.max.y + margin.y + m_meshExtents.min.y - margin.w) / 2f, 0f);
				break;
			}
			Vector3 b = Vector3.zero;
			Vector3 zero3 = Vector3.zero;
			int index_X = 0;
			int index_X2 = 0;
			int num34 = 0;
			int num35 = 0;
			int num36 = 0;
			bool flag7 = false;
			int num37 = 0;
			int num38 = 0;
			bool flag8 = ((!(m_canvas.worldCamera == null)) ? true : false);
			float num39 = (m_previousLossyScaleY = base.transform.lossyScale.y);
			RenderMode renderMode = m_canvas.renderMode;
			float scaleFactor = m_canvas.scaleFactor;
			Color32 underlineColor = Color.white;
			Color32 underlineColor2 = Color.white;
			float num40 = 0f;
			float num41 = 0f;
			float num42 = 0f;
			float num43 = float.PositiveInfinity;
			int num44 = 0;
			float num45 = 0f;
			float num46 = 0f;
			float b2 = 0f;
			TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
			for (int j = 0; j < m_characterCount; j++)
			{
				char character2 = characterInfo[j].character;
				int lineNumber = characterInfo[j].lineNumber;
				TMP_LineInfo tMP_LineInfo = m_textInfo.lineInfo[lineNumber];
				num35 = lineNumber + 1;
				switch (tMP_LineInfo.alignment)
				{
				case TextAlignmentOptions.TopLeft:
				case TextAlignmentOptions.Left:
				case TextAlignmentOptions.BottomLeft:
				case TextAlignmentOptions.BaselineLeft:
				case TextAlignmentOptions.MidlineLeft:
					b = (m_isRightToLeft ? new Vector3(0f - tMP_LineInfo.maxAdvance, 0f, 0f) : new Vector3(0f + tMP_LineInfo.marginLeft, 0f, 0f));
					break;
				case TextAlignmentOptions.Top:
				case TextAlignmentOptions.Center:
				case TextAlignmentOptions.Bottom:
				case TextAlignmentOptions.Baseline:
				case TextAlignmentOptions.Midline:
					b = new Vector3(tMP_LineInfo.marginLeft + tMP_LineInfo.width / 2f - tMP_LineInfo.maxAdvance / 2f, 0f, 0f);
					break;
				case TextAlignmentOptions.TopRight:
				case TextAlignmentOptions.Right:
				case TextAlignmentOptions.BottomRight:
				case TextAlignmentOptions.BaselineRight:
				case TextAlignmentOptions.MidlineRight:
					b = (m_isRightToLeft ? new Vector3(tMP_LineInfo.marginLeft + tMP_LineInfo.width, 0f, 0f) : new Vector3(tMP_LineInfo.marginLeft + tMP_LineInfo.width - tMP_LineInfo.maxAdvance, 0f, 0f));
					break;
				case TextAlignmentOptions.TopJustified:
				case TextAlignmentOptions.Justified:
				case TextAlignmentOptions.BottomJustified:
				case TextAlignmentOptions.BaselineJustified:
				case TextAlignmentOptions.MidlineJustified:
				{
					if (character2 == '­' || character2 == '\u200b' || character2 == '\u2060')
					{
						break;
					}
					char character3 = characterInfo[tMP_LineInfo.lastCharacterIndex].character;
					if (char.IsControl(character3) || lineNumber >= m_lineNumber)
					{
						b = new Vector3(tMP_LineInfo.marginLeft, 0f, 0f);
						break;
					}
					float num47 = tMP_LineInfo.width - tMP_LineInfo.maxAdvance;
					float num48 = ((tMP_LineInfo.spaceCount <= 2) ? 1f : m_wordWrappingRatios);
					if (lineNumber != num36 || j == 0)
					{
						b = new Vector3(tMP_LineInfo.marginLeft, 0f, 0f);
					}
					else if (character2 == '\t' || char.IsSeparator(character2))
					{
						int num49 = ((!characterInfo[tMP_LineInfo.lastCharacterIndex].isVisible) ? (tMP_LineInfo.spaceCount - 1) : tMP_LineInfo.spaceCount);
						if (num49 < 1)
						{
							num49 = 1;
						}
						b += new Vector3(num47 * (1f - num48) / (float)num49, 0f, 0f);
					}
					else
					{
						b += new Vector3(num47 * num48 / (float)(tMP_LineInfo.visibleCharacterCount - 1), 0f, 0f);
					}
					break;
				}
				}
				zero3 = a + b;
				bool isVisible = characterInfo[j].isVisible;
				if (isVisible)
				{
					TMP_TextElementType elementType = characterInfo[j].elementType;
					switch (elementType)
					{
					case TMP_TextElementType.Character:
					{
						Extents lineExtents = tMP_LineInfo.lineExtents;
						float num50 = m_uvLineOffset * (float)lineNumber % 1f + m_uvOffset.x;
						switch (m_horizontalMapping)
						{
						case TextureMappingOptions.Character:
							characterInfo[j].vertex_BL.uv2.x = 0f + m_uvOffset.x;
							characterInfo[j].vertex_TL.uv2.x = 0f + m_uvOffset.x;
							characterInfo[j].vertex_TR.uv2.x = 1f + m_uvOffset.x;
							characterInfo[j].vertex_BR.uv2.x = 1f + m_uvOffset.x;
							break;
						case TextureMappingOptions.Line:
							if (m_textAlignment != TextAlignmentOptions.Justified)
							{
								characterInfo[j].vertex_BL.uv2.x = (characterInfo[j].vertex_BL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num50;
								characterInfo[j].vertex_TL.uv2.x = (characterInfo[j].vertex_TL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num50;
								characterInfo[j].vertex_TR.uv2.x = (characterInfo[j].vertex_TR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num50;
								characterInfo[j].vertex_BR.uv2.x = (characterInfo[j].vertex_BR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num50;
							}
							else
							{
								characterInfo[j].vertex_BL.uv2.x = (characterInfo[j].vertex_BL.position.x + b.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num50;
								characterInfo[j].vertex_TL.uv2.x = (characterInfo[j].vertex_TL.position.x + b.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num50;
								characterInfo[j].vertex_TR.uv2.x = (characterInfo[j].vertex_TR.position.x + b.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num50;
								characterInfo[j].vertex_BR.uv2.x = (characterInfo[j].vertex_BR.position.x + b.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num50;
							}
							break;
						case TextureMappingOptions.Paragraph:
							characterInfo[j].vertex_BL.uv2.x = (characterInfo[j].vertex_BL.position.x + b.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num50;
							characterInfo[j].vertex_TL.uv2.x = (characterInfo[j].vertex_TL.position.x + b.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num50;
							characterInfo[j].vertex_TR.uv2.x = (characterInfo[j].vertex_TR.position.x + b.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num50;
							characterInfo[j].vertex_BR.uv2.x = (characterInfo[j].vertex_BR.position.x + b.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num50;
							break;
						case TextureMappingOptions.MatchAspect:
						{
							switch (m_verticalMapping)
							{
							case TextureMappingOptions.Character:
								characterInfo[j].vertex_BL.uv2.y = 0f + m_uvOffset.y;
								characterInfo[j].vertex_TL.uv2.y = 1f + m_uvOffset.y;
								characterInfo[j].vertex_TR.uv2.y = 0f + m_uvOffset.y;
								characterInfo[j].vertex_BR.uv2.y = 1f + m_uvOffset.y;
								break;
							case TextureMappingOptions.Line:
								characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num50;
								characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num50;
								characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
								characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
								break;
							case TextureMappingOptions.Paragraph:
								characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + num50;
								characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + num50;
								characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
								characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
								break;
							case TextureMappingOptions.MatchAspect:
								Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
								break;
							}
							float num51 = (1f - (characterInfo[j].vertex_BL.uv2.y + characterInfo[j].vertex_TL.uv2.y) * characterInfo[j].aspectRatio) / 2f;
							characterInfo[j].vertex_BL.uv2.x = characterInfo[j].vertex_BL.uv2.y * characterInfo[j].aspectRatio + num51 + num50;
							characterInfo[j].vertex_TL.uv2.x = characterInfo[j].vertex_BL.uv2.x;
							characterInfo[j].vertex_TR.uv2.x = characterInfo[j].vertex_TL.uv2.y * characterInfo[j].aspectRatio + num51 + num50;
							characterInfo[j].vertex_BR.uv2.x = characterInfo[j].vertex_TR.uv2.x;
							break;
						}
						}
						switch (m_verticalMapping)
						{
						case TextureMappingOptions.Character:
							characterInfo[j].vertex_BL.uv2.y = 0f + m_uvOffset.y;
							characterInfo[j].vertex_TL.uv2.y = 1f + m_uvOffset.y;
							characterInfo[j].vertex_TR.uv2.y = 1f + m_uvOffset.y;
							characterInfo[j].vertex_BR.uv2.y = 0f + m_uvOffset.y;
							break;
						case TextureMappingOptions.Line:
							characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - tMP_LineInfo.descender) / (tMP_LineInfo.ascender - tMP_LineInfo.descender) + m_uvOffset.y;
							characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - tMP_LineInfo.descender) / (tMP_LineInfo.ascender - tMP_LineInfo.descender) + m_uvOffset.y;
							characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
							characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
							break;
						case TextureMappingOptions.Paragraph:
							characterInfo[j].vertex_BL.uv2.y = (characterInfo[j].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
							characterInfo[j].vertex_TL.uv2.y = (characterInfo[j].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
							characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
							characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
							break;
						case TextureMappingOptions.MatchAspect:
						{
							float num52 = (1f - (characterInfo[j].vertex_BL.uv2.x + characterInfo[j].vertex_TR.uv2.x) / characterInfo[j].aspectRatio) / 2f;
							characterInfo[j].vertex_BL.uv2.y = num52 + characterInfo[j].vertex_BL.uv2.x / characterInfo[j].aspectRatio + m_uvOffset.y;
							characterInfo[j].vertex_TL.uv2.y = num52 + characterInfo[j].vertex_TR.uv2.x / characterInfo[j].aspectRatio + m_uvOffset.y;
							characterInfo[j].vertex_BR.uv2.y = characterInfo[j].vertex_BL.uv2.y;
							characterInfo[j].vertex_TR.uv2.y = characterInfo[j].vertex_TL.uv2.y;
							break;
						}
						}
						float num53 = characterInfo[j].scale * (1f - m_charWidthAdjDelta);
						if (!characterInfo[j].isUsingAlternateTypeface && (characterInfo[j].style & FontStyles.Bold) == FontStyles.Bold)
						{
							num53 *= -1f;
						}
						switch (renderMode)
						{
						case RenderMode.ScreenSpaceOverlay:
							num53 *= num39 / scaleFactor;
							break;
						case RenderMode.ScreenSpaceCamera:
							num53 *= ((!flag8) ? 1f : num39);
							break;
						case RenderMode.WorldSpace:
							num53 *= num39;
							break;
						}
						float x = characterInfo[j].vertex_BL.uv2.x;
						float y = characterInfo[j].vertex_BL.uv2.y;
						float x2 = characterInfo[j].vertex_TR.uv2.x;
						float y2 = characterInfo[j].vertex_TR.uv2.y;
						float num54 = Mathf.Floor(x);
						float num55 = Mathf.Floor(y);
						x -= num54;
						x2 -= num54;
						y -= num55;
						y2 -= num55;
						characterInfo[j].vertex_BL.uv2.x = PackUV(x, y);
						characterInfo[j].vertex_BL.uv2.y = num53;
						characterInfo[j].vertex_TL.uv2.x = PackUV(x, y2);
						characterInfo[j].vertex_TL.uv2.y = num53;
						characterInfo[j].vertex_TR.uv2.x = PackUV(x2, y2);
						characterInfo[j].vertex_TR.uv2.y = num53;
						characterInfo[j].vertex_BR.uv2.x = PackUV(x2, y);
						characterInfo[j].vertex_BR.uv2.y = num53;
						break;
					}
					}
					if (j < m_maxVisibleCharacters && lineNumber < m_maxVisibleLines && m_overflowMode != TextOverflowModes.Page)
					{
						characterInfo[j].vertex_BL.position += zero3;
						characterInfo[j].vertex_TL.position += zero3;
						characterInfo[j].vertex_TR.position += zero3;
						characterInfo[j].vertex_BR.position += zero3;
					}
					else if (j < m_maxVisibleCharacters && lineNumber < m_maxVisibleLines && m_overflowMode == TextOverflowModes.Page && characterInfo[j].pageNumber == num10)
					{
						characterInfo[j].vertex_BL.position += zero3;
						characterInfo[j].vertex_TL.position += zero3;
						characterInfo[j].vertex_TR.position += zero3;
						characterInfo[j].vertex_BR.position += zero3;
					}
					else
					{
						characterInfo[j].vertex_BL.position = Vector3.zero;
						characterInfo[j].vertex_TL.position = Vector3.zero;
						characterInfo[j].vertex_TR.position = Vector3.zero;
						characterInfo[j].vertex_BR.position = Vector3.zero;
					}
					switch (elementType)
					{
					case TMP_TextElementType.Character:
						FillCharacterVertexBuffers(j, index_X);
						break;
					case TMP_TextElementType.Sprite:
						FillSpriteVertexBuffers(j, index_X2);
						break;
					}
				}
				m_textInfo.characterInfo[j].bottomLeft += zero3;
				m_textInfo.characterInfo[j].topLeft += zero3;
				m_textInfo.characterInfo[j].topRight += zero3;
				m_textInfo.characterInfo[j].bottomRight += zero3;
				m_textInfo.characterInfo[j].origin += zero3.x;
				m_textInfo.characterInfo[j].xAdvance += zero3.x;
				m_textInfo.characterInfo[j].ascender += zero3.y;
				m_textInfo.characterInfo[j].descender += zero3.y;
				m_textInfo.characterInfo[j].baseLine += zero3.y;
				if (isVisible)
				{
				}
				if (lineNumber != num36 || j == m_characterCount - 1)
				{
					if (lineNumber != num36)
					{
						m_textInfo.lineInfo[num36].baseline += zero3.y;
						m_textInfo.lineInfo[num36].ascender += zero3.y;
						m_textInfo.lineInfo[num36].descender += zero3.y;
						m_textInfo.lineInfo[num36].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[num36].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[num36].descender);
						m_textInfo.lineInfo[num36].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[num36].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[num36].ascender);
					}
					if (j == m_characterCount - 1)
					{
						m_textInfo.lineInfo[lineNumber].baseline += zero3.y;
						m_textInfo.lineInfo[lineNumber].ascender += zero3.y;
						m_textInfo.lineInfo[lineNumber].descender += zero3.y;
						m_textInfo.lineInfo[lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lineNumber].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[lineNumber].descender);
						m_textInfo.lineInfo[lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lineNumber].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[lineNumber].ascender);
					}
				}
				if (char.IsLetterOrDigit(character2) || character2 == '-' || character2 == '­' || character2 == '‐' || character2 == '‑')
				{
					if (!flag7)
					{
						flag7 = true;
						num37 = j;
					}
					if (flag7 && j == m_characterCount - 1)
					{
						int num56 = m_textInfo.wordInfo.Length;
						int wordCount = m_textInfo.wordCount;
						if (m_textInfo.wordCount + 1 > num56)
						{
							TMP_TextInfo.Resize(ref m_textInfo.wordInfo, num56 + 1);
						}
						num38 = j;
						m_textInfo.wordInfo[wordCount].firstCharacterIndex = num37;
						m_textInfo.wordInfo[wordCount].lastCharacterIndex = num38;
						m_textInfo.wordInfo[wordCount].characterCount = num38 - num37 + 1;
						m_textInfo.wordInfo[wordCount].textComponent = this;
						num34++;
						m_textInfo.wordCount++;
						m_textInfo.lineInfo[lineNumber].wordCount++;
					}
				}
				else if ((flag7 || (j == 0 && (!char.IsPunctuation(character2) || char.IsWhiteSpace(character2) || j == m_characterCount - 1))) && (j <= 0 || j >= m_characterCount || (character2 != '\'' && character2 != '’') || !char.IsLetterOrDigit(characterInfo[j - 1].character) || !char.IsLetterOrDigit(characterInfo[j + 1].character)))
				{
					num38 = ((j != m_characterCount - 1 || !char.IsLetterOrDigit(character2)) ? (j - 1) : j);
					flag7 = false;
					int num57 = m_textInfo.wordInfo.Length;
					int wordCount2 = m_textInfo.wordCount;
					if (m_textInfo.wordCount + 1 > num57)
					{
						TMP_TextInfo.Resize(ref m_textInfo.wordInfo, num57 + 1);
					}
					m_textInfo.wordInfo[wordCount2].firstCharacterIndex = num37;
					m_textInfo.wordInfo[wordCount2].lastCharacterIndex = num38;
					m_textInfo.wordInfo[wordCount2].characterCount = num38 - num37 + 1;
					m_textInfo.wordInfo[wordCount2].textComponent = this;
					num34++;
					m_textInfo.wordCount++;
					m_textInfo.lineInfo[lineNumber].wordCount++;
				}
				if ((m_textInfo.characterInfo[j].style & FontStyles.Underline) == FontStyles.Underline)
				{
					bool flag9 = true;
					int pageNumber = m_textInfo.characterInfo[j].pageNumber;
					if (j > m_maxVisibleCharacters || lineNumber > m_maxVisibleLines || (m_overflowMode == TextOverflowModes.Page && pageNumber + 1 != m_pageToDisplay))
					{
						flag9 = false;
					}
					if (!char.IsWhiteSpace(character2))
					{
						num42 = Mathf.Max(num42, m_textInfo.characterInfo[j].scale);
						num43 = Mathf.Min((pageNumber != num44) ? float.PositiveInfinity : num43, m_textInfo.characterInfo[j].baseLine + base.font.fontInfo.Underline * num42);
						num44 = pageNumber;
					}
					if (!flag && flag9 && j <= tMP_LineInfo.lastVisibleCharacterIndex && character2 != '\n' && character2 != '\r' && (j != tMP_LineInfo.lastVisibleCharacterIndex || !char.IsSeparator(character2)))
					{
						flag = true;
						num40 = m_textInfo.characterInfo[j].scale;
						if (num42 == 0f)
						{
							num42 = num40;
						}
						start = new Vector3(m_textInfo.characterInfo[j].bottomLeft.x, num43, 0f);
						underlineColor = m_textInfo.characterInfo[j].color;
					}
					if (flag && m_characterCount == 1)
					{
						flag = false;
						zero = new Vector3(m_textInfo.characterInfo[j].topRight.x, num43, 0f);
						num41 = m_textInfo.characterInfo[j].scale;
						DrawUnderlineMesh(start, zero, ref index, num40, num41, num42, underlineColor);
						num42 = 0f;
						num43 = float.PositiveInfinity;
					}
					else if (flag && (j == tMP_LineInfo.lastCharacterIndex || j >= tMP_LineInfo.lastVisibleCharacterIndex))
					{
						if (char.IsWhiteSpace(character2))
						{
							int lastVisibleCharacterIndex = tMP_LineInfo.lastVisibleCharacterIndex;
							zero = new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, num43, 0f);
							num41 = m_textInfo.characterInfo[lastVisibleCharacterIndex].scale;
						}
						else
						{
							zero = new Vector3(m_textInfo.characterInfo[j].topRight.x, num43, 0f);
							num41 = m_textInfo.characterInfo[j].scale;
						}
						flag = false;
						DrawUnderlineMesh(start, zero, ref index, num40, num41, num42, underlineColor);
						num42 = 0f;
						num43 = float.PositiveInfinity;
					}
					else if (flag && !flag9)
					{
						flag = false;
						zero = new Vector3(m_textInfo.characterInfo[j - 1].topRight.x, num43, 0f);
						num41 = m_textInfo.characterInfo[j - 1].scale;
						DrawUnderlineMesh(start, zero, ref index, num40, num41, num42, underlineColor);
						num42 = 0f;
						num43 = float.PositiveInfinity;
					}
				}
				else if (flag)
				{
					flag = false;
					zero = new Vector3(m_textInfo.characterInfo[j - 1].topRight.x, num43, 0f);
					num41 = m_textInfo.characterInfo[j - 1].scale;
					DrawUnderlineMesh(start, zero, ref index, num40, num41, num42, underlineColor);
					num42 = 0f;
					num43 = float.PositiveInfinity;
				}
				if ((m_textInfo.characterInfo[j].style & FontStyles.Strikethrough) == FontStyles.Strikethrough)
				{
					bool flag10 = true;
					if (j > m_maxVisibleCharacters || lineNumber > m_maxVisibleLines || (m_overflowMode == TextOverflowModes.Page && m_textInfo.characterInfo[j].pageNumber + 1 != m_pageToDisplay))
					{
						flag10 = false;
					}
					if (!flag2 && flag10 && j <= tMP_LineInfo.lastVisibleCharacterIndex && character2 != '\n' && character2 != '\r' && (j != tMP_LineInfo.lastVisibleCharacterIndex || !char.IsSeparator(character2)))
					{
						flag2 = true;
						num45 = m_textInfo.characterInfo[j].pointSize;
						num46 = m_textInfo.characterInfo[j].scale;
						start2 = new Vector3(m_textInfo.characterInfo[j].bottomLeft.x, m_textInfo.characterInfo[j].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2.75f * num46, 0f);
						underlineColor2 = m_textInfo.characterInfo[j].color;
						b2 = m_textInfo.characterInfo[j].baseLine;
					}
					if (flag2 && m_characterCount == 1)
					{
						flag2 = false;
						zero2 = new Vector3(m_textInfo.characterInfo[j].topRight.x, m_textInfo.characterInfo[j].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num46, 0f);
						DrawUnderlineMesh(start2, zero2, ref index, num46, num46, num46, underlineColor2);
					}
					else if (flag2 && j == tMP_LineInfo.lastCharacterIndex)
					{
						if (!char.IsWhiteSpace(character2))
						{
							zero2 = new Vector3(m_textInfo.characterInfo[j].topRight.x, m_textInfo.characterInfo[j].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num46, 0f);
						}
						else
						{
							int lastVisibleCharacterIndex2 = tMP_LineInfo.lastVisibleCharacterIndex;
							zero2 = new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex2].topRight.x, m_textInfo.characterInfo[lastVisibleCharacterIndex2].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num46, 0f);
						}
						flag2 = false;
						DrawUnderlineMesh(start2, zero2, ref index, num46, num46, num46, underlineColor2);
					}
					else if (flag2 && j < m_characterCount && (m_textInfo.characterInfo[j + 1].pointSize != num45 || !TMP_Math.Approximately(m_textInfo.characterInfo[j + 1].baseLine + zero3.y, b2)))
					{
						flag2 = false;
						int lastVisibleCharacterIndex3 = tMP_LineInfo.lastVisibleCharacterIndex;
						zero2 = ((j > lastVisibleCharacterIndex3) ? new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex3].topRight.x, m_textInfo.characterInfo[lastVisibleCharacterIndex3].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num46, 0f) : new Vector3(m_textInfo.characterInfo[j].topRight.x, m_textInfo.characterInfo[j].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num46, 0f));
						DrawUnderlineMesh(start2, zero2, ref index, num46, num46, num46, underlineColor2);
					}
					else if (flag2 && !flag10)
					{
						flag2 = false;
						zero2 = new Vector3(m_textInfo.characterInfo[j - 1].topRight.x, m_textInfo.characterInfo[j - 1].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * num46, 0f);
						DrawUnderlineMesh(start2, zero2, ref index, num46, num46, num46, underlineColor2);
					}
				}
				else if (flag2)
				{
					flag2 = false;
					zero2 = new Vector3(m_textInfo.characterInfo[j - 1].topRight.x, m_textInfo.characterInfo[j - 1].baseLine + (base.font.fontInfo.Ascender + base.font.fontInfo.Descender) / 2f * m_fontScale, 0f);
					DrawUnderlineMesh(start2, zero2, ref index, num46, num46, num46, underlineColor2);
				}
				num36 = lineNumber;
			}
			m_textInfo.characterCount = (short)m_characterCount;
			m_textInfo.spriteCount = m_spriteCount;
			m_textInfo.lineCount = (short)num35;
			m_textInfo.wordCount = ((num34 == 0 || m_characterCount <= 0) ? 1 : ((short)num34));
			m_textInfo.pageCount = m_pageNumber + 1;
			if (m_renderMode == TextRenderFlags.Render)
			{
				m_mesh.MarkDynamic();
				m_mesh.vertices = m_textInfo.meshInfo[0].vertices;
				m_mesh.uv = m_textInfo.meshInfo[0].uvs0;
				m_mesh.uv2 = m_textInfo.meshInfo[0].uvs2;
				m_mesh.colors32 = m_textInfo.meshInfo[0].colors32;
				m_mesh.RecalculateBounds();
				m_canvasRenderer.SetMesh(m_mesh);
				for (int k = 1; k < m_textInfo.materialCount; k++)
				{
					m_textInfo.meshInfo[k].ClearUnusedVertices();
					if (!(m_subTextObjects[k] == null))
					{
						m_subTextObjects[k].mesh.MarkDynamic();
						m_subTextObjects[k].mesh.vertices = m_textInfo.meshInfo[k].vertices;
						m_subTextObjects[k].mesh.uv = m_textInfo.meshInfo[k].uvs0;
						m_subTextObjects[k].mesh.uv2 = m_textInfo.meshInfo[k].uvs2;
						m_subTextObjects[k].mesh.colors32 = m_textInfo.meshInfo[k].colors32;
						m_subTextObjects[k].mesh.RecalculateBounds();
						m_subTextObjects[k].canvasRenderer.SetMesh(m_subTextObjects[k].mesh);
					}
				}
			}
			TMPro_EventManager.ON_TEXT_CHANGED(this);
		}

		protected override Vector3[] GetTextContainerLocalCorners()
		{
			if (m_rectTransform == null)
			{
				m_rectTransform = base.rectTransform;
			}
			m_rectTransform.GetLocalCorners(m_RectTransformCorners);
			return m_RectTransformCorners;
		}

		private void ClearMesh()
		{
			m_canvasRenderer.SetMesh(null);
			for (int i = 1; i < m_subTextObjects.Length && m_subTextObjects[i] != null; i++)
			{
				m_subTextObjects[i].canvasRenderer.SetMesh(null);
			}
		}

		protected override void SetActiveSubMeshes(bool state)
		{
			for (int i = 1; i < m_subTextObjects.Length && m_subTextObjects[i] != null; i++)
			{
				if (m_subTextObjects[i].enabled != state)
				{
					m_subTextObjects[i].enabled = state;
				}
			}
		}

		private void UpdateSDFScale(float lossyScale)
		{
			lossyScale = ((lossyScale != 0f) ? lossyScale : 1f);
			float num = 0f;
			float scaleFactor = m_canvas.scaleFactor;
			num = ((m_canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? (lossyScale / scaleFactor) : ((m_canvas.renderMode != RenderMode.ScreenSpaceCamera) ? lossyScale : ((!(m_canvas.worldCamera != null)) ? 1f : lossyScale)));
			for (int i = 0; i < m_textInfo.characterCount; i++)
			{
				if (m_textInfo.characterInfo[i].isVisible && m_textInfo.characterInfo[i].elementType == TMP_TextElementType.Character)
				{
					float num2 = num * m_textInfo.characterInfo[i].scale * (1f - m_charWidthAdjDelta);
					if ((m_textInfo.characterInfo[i].style & FontStyles.Bold) == FontStyles.Bold)
					{
						num2 *= -1f;
					}
					int materialReferenceIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
					int vertexIndex = m_textInfo.characterInfo[i].vertexIndex;
					m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex].y = num2;
					m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex + 1].y = num2;
					m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex + 2].y = num2;
					m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexIndex + 3].y = num2;
				}
			}
			for (int j = 0; j < m_textInfo.materialCount; j++)
			{
				if (j == 0)
				{
					m_mesh.uv2 = m_textInfo.meshInfo[0].uvs2;
					m_canvasRenderer.SetMesh(m_mesh);
				}
				else
				{
					m_subTextObjects[j].mesh.uv2 = m_textInfo.meshInfo[j].uvs2;
					m_subTextObjects[j].canvasRenderer.SetMesh(m_subTextObjects[j].mesh);
				}
			}
		}

		protected override void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
			Vector3 vector = new Vector3(0f, offset, 0f);
			for (int i = startIndex; i <= endIndex; i++)
			{
				m_textInfo.characterInfo[i].bottomLeft -= vector;
				m_textInfo.characterInfo[i].topLeft -= vector;
				m_textInfo.characterInfo[i].topRight -= vector;
				m_textInfo.characterInfo[i].bottomRight -= vector;
				m_textInfo.characterInfo[i].ascender -= vector.y;
				m_textInfo.characterInfo[i].baseLine -= vector.y;
				m_textInfo.characterInfo[i].descender -= vector.y;
				if (m_textInfo.characterInfo[i].isVisible)
				{
					m_textInfo.characterInfo[i].vertex_BL.position -= vector;
					m_textInfo.characterInfo[i].vertex_TL.position -= vector;
					m_textInfo.characterInfo[i].vertex_TR.position -= vector;
					m_textInfo.characterInfo[i].vertex_BR.position -= vector;
				}
			}
		}

		public void CalculateLayoutInputHorizontal()
		{
			if (base.gameObject.activeInHierarchy && (m_isCalculateSizeRequired || m_rectTransform.hasChanged))
			{
				m_preferredWidth = GetPreferredWidth();
				ComputeMarginSize();
				m_isLayoutDirty = true;
			}
		}

		public void CalculateLayoutInputVertical()
		{
			if (base.gameObject.activeInHierarchy)
			{
				if (m_isCalculateSizeRequired || m_rectTransform.hasChanged)
				{
					m_preferredHeight = GetPreferredHeight();
					ComputeMarginSize();
					m_isLayoutDirty = true;
				}
				m_isCalculateSizeRequired = false;
			}
		}

		public override void SetVerticesDirty()
		{
			if (!m_verticesAlreadyDirty && !(this == null) && IsActive())
			{
				m_verticesAlreadyDirty = true;
				CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			}
		}

		public override void SetLayoutDirty()
		{
			if (!m_layoutAlreadyDirty && !(this == null) && IsActive())
			{
				m_layoutAlreadyDirty = true;
				LayoutRebuilder.MarkLayoutForRebuild(base.rectTransform);
				m_isLayoutDirty = true;
			}
		}

		public override void SetMaterialDirty()
		{
			if (IsActive())
			{
				m_isMaterialDirty = true;
				CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			}
		}

		public override void SetAllDirty()
		{
			SetLayoutDirty();
			SetVerticesDirty();
			SetMaterialDirty();
		}

		public override void Rebuild(CanvasUpdate update)
		{
			if (!(this == null) && update == CanvasUpdate.PreRender)
			{
				OnPreRenderCanvas();
				m_verticesAlreadyDirty = false;
				m_layoutAlreadyDirty = false;
				if (m_isMaterialDirty)
				{
					UpdateMaterial();
					m_isMaterialDirty = false;
				}
			}
		}

		private void UpdateSubObjectPivot()
		{
			if (m_textInfo != null)
			{
				for (int i = 1; i < m_subTextObjects.Length && m_subTextObjects[i] != null; i++)
				{
					m_subTextObjects[i].SetPivotDirty();
				}
			}
		}

		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			Material material = baseMaterial;
			if (m_ShouldRecalculateStencil)
			{
				m_stencilID = TMP_MaterialManager.GetStencilID(base.gameObject);
				m_ShouldRecalculateStencil = false;
			}
			if (m_stencilID > 0)
			{
				material = TMP_MaterialManager.GetStencilMaterial(baseMaterial, m_stencilID);
				if (m_MaskMaterial != null)
				{
					TMP_MaterialManager.ReleaseStencilMaterial(m_MaskMaterial);
				}
				m_MaskMaterial = material;
			}
			return material;
		}

		protected override void UpdateMaterial()
		{
			if (m_canvasRenderer == null)
			{
				m_canvasRenderer = canvasRenderer;
			}
			m_canvasRenderer.materialCount = 1;
			m_canvasRenderer.SetMaterial(materialForRendering, m_sharedMaterial.mainTexture);
		}

		public override void RecalculateClipping()
		{
			base.RecalculateClipping();
		}

		public override void RecalculateMasking()
		{
			m_ShouldRecalculateStencil = true;
			SetMaterialDirty();
		}

		public override void UpdateMeshPadding()
		{
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
			m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
			m_havePropertiesChanged = true;
			checkPaddingRequired = false;
			for (int i = 1; i < m_textInfo.materialCount; i++)
			{
				m_subTextObjects[i].UpdateMeshPadding(m_enableExtraPadding, m_isUsingBold);
			}
		}

		protected override void InternalCrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
			int materialCount = m_textInfo.materialCount;
			for (int i = 1; i < materialCount; i++)
			{
				m_subTextObjects[i].CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
			}
		}

		protected override void InternalCrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
			int materialCount = m_textInfo.materialCount;
			for (int i = 1; i < materialCount; i++)
			{
				m_subTextObjects[i].CrossFadeAlpha(alpha, duration, ignoreTimeScale);
			}
		}

		public override void ForceMeshUpdate()
		{
			m_havePropertiesChanged = true;
			OnPreRenderCanvas();
		}

		public override void ForceMeshUpdate(bool ignoreInactive)
		{
			m_havePropertiesChanged = true;
			m_ignoreActiveState = true;
			OnPreRenderCanvas();
		}

		public override TMP_TextInfo GetTextInfo(string text)
		{
			StringToCharArray(text, ref m_char_buffer);
			SetArraySizes(m_char_buffer);
			m_renderMode = TextRenderFlags.DontRender;
			ComputeMarginSize();
			if (m_canvas == null)
			{
				m_canvas = base.canvas;
			}
			GenerateTextMesh();
			m_renderMode = TextRenderFlags.Render;
			return base.textInfo;
		}

		public override void UpdateGeometry(Mesh mesh, int index)
		{
			mesh.RecalculateBounds();
			if (index == 0)
			{
				m_canvasRenderer.SetMesh(mesh);
			}
			else
			{
				m_subTextObjects[index].canvasRenderer.SetMesh(mesh);
			}
		}

		public override void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
		{
			int materialCount = m_textInfo.materialCount;
			for (int i = 0; i < materialCount; i++)
			{
				Mesh mesh = ((i != 0) ? m_subTextObjects[i].mesh : m_mesh);
				if ((flags & TMP_VertexDataUpdateFlags.Vertices) == TMP_VertexDataUpdateFlags.Vertices)
				{
					mesh.vertices = m_textInfo.meshInfo[i].vertices;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Uv0) == TMP_VertexDataUpdateFlags.Uv0)
				{
					mesh.uv = m_textInfo.meshInfo[i].uvs0;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Uv2) == TMP_VertexDataUpdateFlags.Uv2)
				{
					mesh.uv2 = m_textInfo.meshInfo[i].uvs2;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Colors32) == TMP_VertexDataUpdateFlags.Colors32)
				{
					mesh.colors32 = m_textInfo.meshInfo[i].colors32;
				}
				mesh.RecalculateBounds();
				if (i == 0)
				{
					m_canvasRenderer.SetMesh(mesh);
				}
				else
				{
					m_subTextObjects[i].canvasRenderer.SetMesh(mesh);
				}
			}
		}

		public override void UpdateVertexData()
		{
			int materialCount = m_textInfo.materialCount;
			for (int i = 0; i < materialCount; i++)
			{
				Mesh mesh = ((i != 0) ? m_subTextObjects[i].mesh : m_mesh);
				mesh.vertices = m_textInfo.meshInfo[i].vertices;
				mesh.uv = m_textInfo.meshInfo[i].uvs0;
				mesh.uv2 = m_textInfo.meshInfo[i].uvs2;
				mesh.colors32 = m_textInfo.meshInfo[i].colors32;
				mesh.RecalculateBounds();
				if (i == 0)
				{
					m_canvasRenderer.SetMesh(mesh);
				}
				else
				{
					m_subTextObjects[i].canvasRenderer.SetMesh(mesh);
				}
			}
		}

		public void UpdateFontAsset()
		{
			LoadFontAsset();
		}

		//TODO 注释
		// virtual float ILayoutElement.get_minWidth()
		// {
		// 	return base.minWidth;
		// }

		// virtual float ILayoutElement.get_flexibleWidth()
		// {
		// 	return base.flexibleWidth;
		// }

		// virtual float ILayoutElement.get_minHeight()
		// {
		// 	return base.minHeight;
		// }

		// virtual float ILayoutElement.get_flexibleHeight()
		// {
		// 	return base.flexibleHeight;
		// }

		// virtual int ILayoutElement.get_layoutPriority()
		// {
		// 	return base.layoutPriority;
		// }
	}
}
