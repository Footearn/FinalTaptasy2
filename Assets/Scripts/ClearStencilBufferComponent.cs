using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class ClearStencilBufferComponent : SkipMasking
{
	public const string SHADER_MASK_CLEAR = "SpriteMask/ClearStencil";

	protected static volatile ClearStencilBufferComponent _instance;

	[SerializeField]
	private Vector2 _size = new Vector2(100f, 100f);

	[SerializeField]
	private Vector2 _pivot = new Vector2(0.5f, 0.5f);

	private Vector3[] vertices = new Vector3[4];

	private MeshRenderer meshRenderer;

	private MeshFilter meshFilter;

	private int lastSortingOrder;

	private string lastSortingLayerName;

	public Vector2 size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
			updateMeshSize();
		}
	}

	public Vector2 pivot
	{
		get
		{
			return _pivot;
		}
		set
		{
			_pivot = value;
			updateMeshSize();
		}
	}

	public static ClearStencilBufferComponent Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = findInstance();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		base.name = "CLEAR_STENCIL_BUFFER";
		meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		if (meshFilter.sharedMesh == null)
		{
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.NotEditable;
			mesh.name = string.Concat("RectMesh");
			mesh.vertices = vertices;
			mesh.uv = new Vector2[4]
			{
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f)
			};
			mesh.triangles = new int[6]
			{
				0,
				1,
				2,
				2,
				3,
				0
			};
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			meshFilter.sharedMesh = mesh;
		}
		meshRenderer = GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}
		meshRenderer.sortingOrder = 10000;
		if (meshRenderer.sharedMaterial == null)
		{
			Shader shader = Shader.Find("SpriteMask/ClearStencil");
			Material material = new Material(shader);
			material.hideFlags = HideFlags.NotEditable;
			material.name = shader.name;
			meshRenderer.sharedMaterial = material;
		}
		updateMeshSize();
	}

	private void OnDrawGizmos()
	{
		if (base.enabled)
		{
			Renderer renderer = getRenderer();
			if (renderer != null)
			{
				Gizmos.color = Color.magenta;
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
				Bounds bounds = renderer.bounds;
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		_instance = null;
	}

	protected virtual void Reset()
	{
		Object[] array = Object.FindObjectsOfType(typeof(ClearStencilBufferComponent));
		if (array != null && array.Length > 1)
		{
			Debug.LogError("ClearStencilBufferComponent is already in the scene!");
		}
	}

	public MeshRenderer getRenderer()
	{
		return meshRenderer;
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

	private static ClearStencilBufferComponent findInstance()
	{
		return Object.FindObjectOfType(typeof(ClearStencilBufferComponent)) as ClearStencilBufferComponent;
	}
}
