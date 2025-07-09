using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class SpriteMask : SpriteMaskingComponent
{
	public enum Type
	{
		Rectangle = 0,
		Sprite = 1,
		Texture = 2,
		CustomMesh = 3,
		None = 100
	}

	public const string SHADER_SPRITE_DEFAULT = "SpriteMask/Default";

	public const string SHADER_SPRITE_DIFFUSE = "SpriteMask/Diffuse";

	public const string SHADER_MASK_ROOT = "SpriteMask/Mask";

	private const string STR_STENCIL = "_Stencil";

	private const string STR_STENCIL_COMPARISON = "_StencilComp";

	private const string STR_STENCIL_READ_MASK = "_StencilReadMask";

	private const string STR_COLOR_MASK = "_ColorMask";

	private const int MAX_LEVELS = 3;

	private const int BASE_RENDER_QUEUE = 3000;

	[SerializeField]
	[FormerlySerializedAs("maskingRoots")]
	public List<Transform> maskedObjects = new List<Transform>();

	public bool forceDefaultMaterialOnChilds;

	public bool forceIndividualMaterialOnChilds;

	[SerializeField]
	private Type _type;

	[SerializeField]
	private Vector2 _size = new Vector2(100f, 100f);

	[SerializeField]
	private Vector2 _pivot = new Vector2(0.5f, 0.5f);

	[SerializeField]
	private Texture2D _texture;

	[SerializeField]
	private Sprite _sprite;

	[SerializeField]
	private bool _inverted;

	[SerializeField]
	private bool _showMaskGraphics;

	private Dictionary<Material, Material> individualMaterials;

	private static bool[] stencilIds = new bool[256];

	private int propertyStencilReadMask = -1;

	private int propertyStencilComp = -1;

	private int propertyColorMask = -1;

	private int propertyStencil = -1;

	private static List<Renderer> rendererComponents = new List<Renderer>();

	private static List<SpriteMask> maskComponents = new List<SpriteMask>();

	private static List<SpriteMaskingComponent> maskingComponents = new List<SpriteMaskingComponent>();

	private static List<SpriteMaskingPart> maskingPartComponents = new List<SpriteMaskingPart>();

	private static List<SkipMasking> skipMaskingComponents = new List<SkipMasking>();

	private Vector3[] vertices = new Vector3[4];

	private Vector2[] uv = new Vector2[4]
	{
		new Vector2(0f, 1f),
		new Vector2(1f, 1f),
		new Vector2(1f, 0f),
		new Vector2(0f, 0f)
	};

	private int[] triangles = new int[6]
	{
		0,
		1,
		2,
		2,
		3,
		0
	};

	private Material _defaultSpriteMaterial;

	[SerializeField]
	private string _defaultSpriteShaderName = "SpriteMask/Default";

	private SpriteRenderer spriteRenderer;

	private ClearStencilBufferComponent clearStencilComponent;

	private MeshRenderer meshRenderer;

	private bool isTypeApplyed;

	private Material _maskMaterial;

	private MeshFilter meshFilter;

	private int? parentStencilId;

	private string instanceId;

	private int _stencilId;

	private int _level;

	public int stencilId
	{
		get
		{
			return _stencilId;
		}
	}

	public int maskIdPerLevel
	{
		get
		{
			return _stencilId & 0x1F;
		}
	}

	public int level
	{
		get
		{
			return _level;
		}
	}

	public int maskRenderQueue
	{
		get
		{
			return 3000 + 100 * _level;
		}
	}

	public int spriteRenderQueue
	{
		get
		{
			return maskRenderQueue;
		}
	}

	public bool isRoot
	{
		get
		{
			return _level == 0;
		}
	}

	[Obsolete("Use maskedObjects")]
	public List<Transform> maskingRoots
	{
		get
		{
			return maskedObjects;
		}
	}

	public bool isChild
	{
		get
		{
			return _level > 0;
		}
	}

	public override bool isEnabled
	{
		get
		{
			return base.enabled;
		}
	}

	public Type type
	{
		get
		{
			return _type;
		}
		set
		{
			if (_type != value || !isTypeApplyed)
			{
				_type = value;
				applyType();
			}
		}
	}

	public Vector2 size
	{
		get
		{
			switch (_type)
			{
			case Type.Rectangle:
			case Type.Texture:
				return _size;
			case Type.Sprite:
				if (spriteRenderer != null && spriteRenderer.sprite != null)
				{
					return spriteRenderer.sprite.bounds.size;
				}
				break;
			case Type.CustomMesh:
				if (meshFilter != null && meshFilter.sharedMesh != null)
				{
					return meshFilter.sharedMesh.bounds.size;
				}
				break;
			}
			return Vector2.zero;
		}
		set
		{
			if (!isTypeApplyed)
			{
				applyType();
			}
			_size = value;
			if (_type == Type.Rectangle || _type == Type.Texture)
			{
				updateMeshSize();
			}
			else
			{
				Debug.LogWarning("Size change not supported on mask type: " + _type);
			}
		}
	}

	public Vector2 pivot
	{
		get
		{
			switch (_type)
			{
			case Type.Rectangle:
			case Type.Texture:
				return _pivot;
			case Type.Sprite:
				if (spriteRenderer != null && spriteRenderer.sprite != null)
				{
					Bounds bounds = spriteRenderer.sprite.bounds;
					Vector2 vector = bounds.min;
					Vector2 vector2 = bounds.size;
					return new Vector2((0f - vector.x) / vector2.x, (0f - vector.y) / vector2.y);
				}
				break;
			case Type.CustomMesh:
				if (meshFilter != null && meshFilter.sharedMesh != null)
				{
					return getPivot(meshFilter.sharedMesh.bounds);
				}
				break;
			}
			return Vector2.zero;
		}
		set
		{
			if (!isTypeApplyed)
			{
				applyType();
			}
			_pivot = value;
			if (_type == Type.Rectangle || _type == Type.Texture)
			{
				updateMeshSize();
			}
			else
			{
				Debug.LogWarning("Pivot change not supported on mask type: " + _type);
			}
		}
	}

	public Sprite sprite
	{
		get
		{
			return _sprite;
		}
		set
		{
			if (!isTypeApplyed)
			{
				applyType();
			}
			if (_type == Type.Sprite)
			{
				spriteRenderer.sprite = (_sprite = value);
			}
			else
			{
				Debug.LogWarning("Sprite change not supported on mask type: " + _type);
			}
		}
	}

	public Texture2D texture
	{
		get
		{
			return _texture;
		}
		set
		{
			if (!isTypeApplyed)
			{
				applyType();
			}
			if (_type == Type.Texture || _type == Type.CustomMesh)
			{
				maskMaterial.mainTexture = (_texture = value);
			}
			else
			{
				Debug.LogWarning("Texture change not supported on mask type: " + _type);
			}
		}
	}

	public bool inverted
	{
		get
		{
			return _inverted;
		}
		set
		{
			if (_inverted != value)
			{
				_inverted = value;
				updateSprites();
			}
		}
	}

	public bool showMaskGraphics
	{
		get
		{
			return _showMaskGraphics;
		}
		set
		{
			if (_showMaskGraphics != value)
			{
				_showMaskGraphics = value;
				maskMaterial.SetFloat(propertyColorMask, _showMaskGraphics ? 15 : 0);
			}
		}
	}

	public Shader spritesShader
	{
		get
		{
			return defaultSpriteMaterial.shader;
		}
		set
		{
			if (value == null)
			{
				Debug.LogWarning("Shader is null");
				return;
			}
			if (!value.isSupported)
			{
				Debug.LogWarning("Shader '" + value.name + "' not supported!");
				return;
			}
			Material defaultSpriteMaterial = this.defaultSpriteMaterial;
			if (defaultSpriteMaterial.shader != value)
			{
				Shader shader = defaultSpriteMaterial.shader;
				defaultSpriteMaterial.shader = value;
				if (!hasStencilSupport(defaultSpriteMaterial))
				{
					Debug.LogWarning("Shader '" + value.name + "' doesn't support Stencil buffer");
					defaultSpriteMaterial.shader = shader;
				}
				_defaultSpriteShaderName = defaultSpriteMaterial.shader.name;
			}
		}
	}

	private Material defaultSpriteMaterial
	{
		get
		{
			if (_defaultSpriteMaterial == null)
			{
				Shader shader = Shader.Find(_defaultSpriteShaderName);
				_defaultSpriteMaterial = new Material(shader);
				_defaultSpriteMaterial.name = shader.name + " OWNER_ID:" + instanceId;
			}
			return _defaultSpriteMaterial;
		}
	}

	private Material maskMaterial
	{
		get
		{
			if (_maskMaterial == null)
			{
				Shader shader = Shader.Find("SpriteMask/Mask");
				_maskMaterial = new Material(shader);
				_maskMaterial.hideFlags = HideFlags.HideAndDontSave;
				_maskMaterial.name = shader.name + " OWNER_ID:" + instanceId;
				_maskMaterial.SetFloat(propertyColorMask, _showMaskGraphics ? 15 : 0);
			}
			return _maskMaterial;
		}
	}

	private void Awake()
	{
		instanceId = GetInstanceID().ToString();
		owner = this;
		propertyStencil = Shader.PropertyToID("_Stencil");
		propertyStencilComp = Shader.PropertyToID("_StencilComp");
		propertyStencilReadMask = Shader.PropertyToID("_StencilReadMask");
		propertyColorMask = Shader.PropertyToID("_ColorMask");
		clearChildsMaterial(base.transform);
		spriteRenderer = GetComponent<SpriteRenderer>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
	}

	private void OnEnable()
	{
		parentStencilId = null;
		Renderer renderer = getRenderer(this);
		if (renderer != null)
		{
			renderer.enabled = true;
		}
		if (isTypeApplyed)
		{
			update();
		}
	}

	private void Start()
	{
		if (!isTypeApplyed)
		{
			applyType();
		}
		update();
	}

	private void OnDisable()
	{
		if (_stencilId > 0)
		{
			releaseId(_stencilId);
			_stencilId = 0;
		}
		if (base.gameObject.activeInHierarchy)
		{
			Renderer renderer = getRenderer(this);
			if (renderer != null)
			{
				renderer.enabled = false;
			}
			SpriteMask parentMask = getParentMask(base.transform);
			if (parentMask != null)
			{
				parentStencilId = parentMask.stencilId;
			}
			else
			{
				parentStencilId = null;
			}
			updateSprites();
			updateMaskingComponents();
		}
	}

	[ContextMenu("Update Mask")]
	public void updateMask()
	{
		if (!isTypeApplyed)
		{
			return;
		}
		if (_stencilId > 0)
		{
			releaseId(_stencilId);
			_stencilId = 0;
		}
		_level = 0;
		Transform parent = base.transform.parent;
		while (parent != null)
		{
			SpriteMaskingComponent maskingComponent = getMaskingComponent(parent);
			if (maskingComponent != null && maskingComponent.isEnabled)
			{
				_level++;
			}
			parent = parent.parent;
		}
		int num = 2;
		if (_level > num)
		{
			Debug.LogError("Maximum number of mask levels has been exceeded: max=" + num + " current=" + _level);
			_level = num;
		}
		_stencilId = getId(_level);
		if (_stencilId == -1)
		{
			Debug.LogError("Maximum number of mask per levels has been exceeded: " + 31);
			_stencilId = 0;
		}
		Material maskMaterial = this.maskMaterial;
		int value;
		CompareFunction value2;
		if (isRoot)
		{
			value = 255;
			value2 = CompareFunction.Always;
		}
		else
		{
			value = 255 >> _level - 1;
			value2 = CompareFunction.Less;
		}
		maskMaterial.renderQueue = maskRenderQueue;
		updateMaterial(maskMaterial, _stencilId, value2, value);
	}

	public void updateMaskingComponents()
	{
		if (maskedObjects == null || maskedObjects.Count == 0)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < maskedObjects.Count; i++)
		{
			if (maskedObjects[i] == null)
			{
				flag = true;
				continue;
			}
			SpriteMaskingComponent maskingComponent = getMaskingComponent(maskedObjects[i]);
			if (maskingComponent != this)
			{
				if (maskingComponent == null)
				{
					maskingComponent = maskedObjects[i].gameObject.AddComponent<SpriteMaskingComponent>();
					maskingComponent.owner = this;
				}
				else if (maskingComponent.owner == null)
				{
					maskingComponent.owner = this;
				}
				else if (maskingComponent.owner != this)
				{
					continue;
				}
				updateSprites(maskedObjects[i]);
			}
			else
			{
				maskedObjects[i] = null;
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		for (int j = 0; j < maskedObjects.Count; j++)
		{
			if (maskedObjects[j] == null)
			{
				maskedObjects.RemoveAt(j);
				j = 0;
			}
		}
	}

	[ContextMenu("Update Sprites")]
	public void updateSprites()
	{
		updateSprites(base.transform);
	}

	public void updateSprites(Transform trans)
	{
		if (isTypeApplyed)
		{
			int stencil;
			CompareFunction comp;
			if (parentStencilId.HasValue)
			{
				stencil = parentStencilId.Value;
				comp = ((!base.enabled || !_inverted) ? CompareFunction.Equal : CompareFunction.Less);
			}
			else
			{
				stencil = _stencilId;
				comp = ((!base.enabled) ? CompareFunction.Always : ((!_inverted) ? CompareFunction.Equal : ((!isRoot) ? CompareFunction.Less : CompareFunction.NotEqual)));
			}
			doUpdateSprite(trans, comp, stencil);
		}
	}

	public void update()
	{
		updateMask();
		updateSprites();
		updateMaskingComponents();
	}

	public static SpriteMask updateFor(Transform t)
	{
		SpriteMask parentMask = getParentMask(t);
		if (parentMask != null)
		{
			parentMask.updateSprites(t);
		}
		return parentMask;
	}

	public static SpriteMask getParentMask(Transform t)
	{
		t = t.parent;
		while (t != null)
		{
			SpriteMaskingComponent maskingComponent = getMaskingComponent(t);
			if (maskingComponent != null && maskingComponent.isEnabled)
			{
				return maskingComponent.owner;
			}
			t = t.parent;
		}
		return null;
	}

	private void applyType()
	{
		switch (_type)
		{
		case Type.Rectangle:
			_texture = null;
			_sprite = null;
			maskMaterial.mainTexture = null;
			destroySpriteComponents();
			ensureMeshComponents();
			updateMeshSize();
			break;
		case Type.Sprite:
			_texture = null;
			maskMaterial.mainTexture = null;
			destroyMeshComponents();
			ensureSpriteRenderer();
			spriteRenderer.sprite = _sprite;
			break;
		case Type.Texture:
			_sprite = null;
			destroySpriteComponents();
			ensureMeshComponents();
			updateMeshSize();
			maskMaterial.mainTexture = _texture;
			break;
		case Type.CustomMesh:
			_sprite = null;
			destroySpriteComponents();
			maybeDestroyRuntimeMesh();
			ensureMeshComponents();
			maskMaterial.mainTexture = _texture;
			break;
		case Type.None:
			_texture = null;
			_sprite = null;
			maskMaterial.mainTexture = null;
			destroySpriteComponents();
			destroyMeshComponents();
			break;
		}
		isTypeApplyed = true;
	}

	protected override void doUpdateSprite(Transform t, CompareFunction comp, int stencil)
	{
		if (hasSkipMasking(t))
		{
			return;
		}
		if (hasMaskingPart(t))
		{
			Renderer renderer = getRenderer(t);
			if (renderer != null)
			{
				renderer.sharedMaterial = maskMaterial;
			}
			return;
		}
		SpriteMaskingComponent maskingComponent = getMaskingComponent(t);
		bool flag = true;
		if (maskingComponent != null)
		{
			if (maskingComponent == this)
			{
				flag = false;
			}
			else if (maskingComponent.owner == this)
			{
				flag = true;
			}
			else if (maskingComponent is SpriteMask)
			{
				flag = false;
				if (maskingComponent.isEnabled)
				{
					((SpriteMask)maskingComponent).update();
					return;
				}
			}
			else if (maskingComponent.isEnabled)
			{
				return;
			}
		}
		if (flag)
		{
			Renderer renderer2 = getRenderer(t);
			if (renderer2 != null)
			{
				Material[] sharedMaterials = renderer2.sharedMaterials;
				if (sharedMaterials.Length > 1)
				{
					for (int i = 0; i < sharedMaterials.Length; i++)
					{
						if (sharedMaterials[i] != null && hasStencilSupport(sharedMaterials[i]))
						{
							if (forceIndividualMaterialOnChilds && sharedMaterials[i].GetInstanceID() > 0)
							{
								sharedMaterials[i] = getIndividualMaterial(sharedMaterials[i]);
							}
							sharedMaterials[i].renderQueue = spriteRenderQueue;
							updateMaterial(sharedMaterials[i], stencil, comp, null);
						}
					}
				}
				else
				{
					Material material;
					if (sharedMaterials.Length == 0)
					{
						material = (renderer2.sharedMaterial = this.defaultSpriteMaterial);
					}
					else
					{
						material = sharedMaterials[0];
						if (material == null || !hasStencilSupport(material) || forceDefaultMaterialOnChilds)
						{
							material = (renderer2.sharedMaterial = this.defaultSpriteMaterial);
						}
						else if (forceIndividualMaterialOnChilds && material.GetInstanceID() > 0)
						{
							material = (renderer2.sharedMaterial = getIndividualMaterial(material));
						}
					}
					material.renderQueue = spriteRenderQueue;
					updateMaterial(material, stencil, comp, null);
				}
			}
		}
		int childCount = t.childCount;
		if (childCount > 0)
		{
			for (int j = 0; j < childCount; j++)
			{
				doUpdateSprite(t.GetChild(j), comp, stencil);
			}
		}
	}

	private Material getIndividualMaterial(Material origin)
	{
		if (individualMaterials == null)
		{
			individualMaterials = new Dictionary<Material, Material>();
		}
		if (individualMaterials.ContainsKey(origin))
		{
			return individualMaterials[origin];
		}
		Material material = new Material(origin);
		material.name += " (Clone)";
		clearMaterial(origin);
		individualMaterials[origin] = material;
		return material;
	}

	private void clearChildsMaterial(Transform t)
	{
		int childCount = t.childCount;
		if (childCount == 0)
		{
			return;
		}
		for (int i = 0; i < childCount; i++)
		{
			Transform child = t.GetChild(i);
			SpriteMaskingComponent maskingComponent = getMaskingComponent(child);
			if (maskingComponent != null)
			{
				continue;
			}
			Renderer renderer = getRenderer(child);
			if (renderer != null)
			{
				Material sharedMaterial = renderer.sharedMaterial;
				if (sharedMaterial != null)
				{
					string text = readOwnerId(sharedMaterial.name);
					if (text != null && !text.Equals(instanceId))
					{
						renderer.sharedMaterial = null;
					}
				}
			}
			clearChildsMaterial(child);
		}
	}

	private void clearMaterial(Material m)
	{
		updateMaterial(m, 0, CompareFunction.Always, null);
	}

	private void updateMaterial(Material m, int? stencil, CompareFunction? comp, int? readMask)
	{
		if (stencil.HasValue)
		{
			m.SetInt(propertyStencil, stencil.Value);
		}
		if (comp.HasValue)
		{
			m.SetInt(propertyStencilComp, (int)comp.Value);
		}
		if (readMask.HasValue)
		{
			m.SetInt(propertyStencilReadMask, readMask.Value);
		}
	}

	private int getId(int level)
	{
		int num = 128 >> level;
		int num2 = 31;
		for (int i = 0; i < num2; i++)
		{
			int num3 = num + i;
			if (!stencilIds[num3])
			{
				stencilIds[num3] = true;
				return num3;
			}
		}
		return -1;
	}

	private void releaseId(int id)
	{
		stencilIds[id] = false;
	}

	private bool hasStencilSupport(Material m)
	{
		return m.HasProperty(propertyStencil) && m.HasProperty(propertyStencilComp);
	}

	private string readOwnerId(string str)
	{
		int num = str.IndexOf("OWNER_ID");
		if (num != -1)
		{
			return str.Substring(num + 9);
		}
		return null;
	}

	private void destroySpriteComponents()
	{
		if (spriteRenderer != null)
		{
			UnityEngine.Object.Destroy(spriteRenderer);
			spriteRenderer = null;
		}
	}

	private void destroyMeshComponents()
	{
		if (meshFilter != null)
		{
			UnityEngine.Object.Destroy(meshFilter);
			meshFilter = null;
		}
		if (meshRenderer != null)
		{
			UnityEngine.Object.Destroy(meshRenderer);
			meshRenderer = null;
		}
	}

	private void maybeDestroyRuntimeMesh()
	{
		if (meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.GetInstanceID() < 0)
		{
			UnityEngine.Object.Destroy(meshFilter.sharedMesh);
			meshFilter.sharedMesh = null;
			meshFilter.mesh = null;
		}
	}

	private void ensureSpriteRenderer()
	{
		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer == null)
			{
				spriteRenderer = base.gameObject.AddComponent<SpriteRenderer>();
			}
		}
		spriteRenderer.sharedMaterial = maskMaterial;
	}

	private void ensureMeshComponents()
	{
		if (meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
		}
		if (meshFilter.sharedMesh == null)
		{
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.name = "RectMesh OWNER_ID:" + instanceId;
			mesh.MarkDynamic();
			meshFilter.sharedMesh = mesh;
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
		}
		if (meshRenderer == null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
		}
		meshRenderer.sharedMaterial = maskMaterial;
	}

	private void updateMeshSize()
	{
		if (meshFilter != null)
		{
			float num = (0f - _pivot.x) * _size.x;
			float num2 = (0f - _pivot.y) * _size.y;
			float x = num + _size.x;
			float y = num2 + _size.y;
			vertices[0] = new Vector3(num, y, 0f);
			vertices[1] = new Vector3(x, y, 0f);
			vertices[2] = new Vector3(x, num2, 0f);
			vertices[3] = new Vector3(num, num2, 0f);
			meshFilter.sharedMesh.vertices = vertices;
			meshFilter.sharedMesh.RecalculateBounds();
		}
	}

	public Renderer getRenderer()
	{
		return getRenderer(this);
	}

	private static Renderer getRenderer(Component c)
	{
		c.GetComponents(rendererComponents);
		Renderer result = ((rendererComponents.Count <= 0) ? null : rendererComponents[0]);
		rendererComponents.Clear();
		return result;
	}

	private static bool hasMaskingPart(Component t)
	{
		t.GetComponents(maskingPartComponents);
		bool result = maskingPartComponents.Count > 0;
		maskingPartComponents.Clear();
		return result;
	}

	private static bool hasSkipMasking(Component t)
	{
		t.GetComponents(skipMaskingComponents);
		bool result = skipMaskingComponents.Count > 0;
		skipMaskingComponents.Clear();
		return result;
	}

	private static SpriteMaskingComponent getMaskingComponent(Component t)
	{
		t.GetComponents(maskingComponents);
		SpriteMaskingComponent result = ((maskingComponents.Count != 1) ? null : maskingComponents[0]);
		maskingComponents.Clear();
		return result;
	}

	private static SpriteMask getSpriteMask(Component t)
	{
		t.GetComponents(maskComponents);
		SpriteMask result = ((maskComponents.Count != 1) ? null : maskComponents[0]);
		maskComponents.Clear();
		return result;
	}

	public Vector2 getPivot(Bounds bounds)
	{
		Vector2 result = default(Vector2);
		result.x = (0f - bounds.center.x) / bounds.extents.x / 2f + 0.5f;
		result.y = (0f - bounds.center.y) / bounds.extents.y / 2f + 0.5f;
		return result;
	}
}
