using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraFit : ObjectBase
{
	public enum Constraint
	{
		Landscape,
		Portrait
	}

	public Color wireColor = Color.white;

	public float UnitsSize = 1f;

	public Constraint constraint = Constraint.Portrait;

	public static CameraFit Instance;

	public Camera camera;

	private float m_unitsSize;

	private float cameraSize = 1f;

	public float targetSize = -1f;

	private float _width;

	private float _height;

	private Vector3 _bl;

	private Vector3 _bc;

	private Vector3 _br;

	private Vector3 _ml;

	private Vector3 _mc;

	private Vector3 _mr;

	private Vector3 _tl;

	private Vector3 _tc;

	private Vector3 _tr;

	public float Width
	{
		get
		{
			return _width;
		}
	}

	public float Height
	{
		get
		{
			return _height;
		}
	}

	public Vector3 BottomLeft
	{
		get
		{
			return _bl;
		}
	}

	public Vector3 BottomCenter
	{
		get
		{
			return _bc;
		}
	}

	public Vector3 BottomRight
	{
		get
		{
			return _br;
		}
	}

	public Vector3 MiddleLeft
	{
		get
		{
			return _ml;
		}
	}

	public Vector3 MiddleCenter
	{
		get
		{
			return _mc;
		}
	}

	public Vector3 MiddleRight
	{
		get
		{
			return _mr;
		}
	}

	public Vector3 TopLeft
	{
		get
		{
			return _tl;
		}
	}

	public Vector3 TopCenter
	{
		get
		{
			return _tc;
		}
	}

	public Vector3 TopRight
	{
		get
		{
			return _tr;
		}
	}

	private void Awake()
	{
		camera = GetComponent<Camera>();
		m_unitsSize = UnitsSize;
		ComputeResolution();
	}

	public void startGame()
	{
		setCameraDefault();
		ComputeResolution();
	}

	public void ComputeResolution()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		if (targetSize != -1f)
		{
			if (Mathf.Abs(UnitsSize - targetSize) >= 0.01f)
			{
				UnitsSize = Mathf.Lerp(UnitsSize, targetSize, Time.deltaTime * GameManager.timeScale * 12f);
			}
			else
			{
				UnitsSize = targetSize;
			}
		}
		else if (Mathf.Abs(m_unitsSize - UnitsSize) >= 0.01f)
		{
			UnitsSize = Mathf.Lerp(UnitsSize, m_unitsSize, Time.deltaTime * GameManager.timeScale * 12f);
		}
		else
		{
			UnitsSize = m_unitsSize;
		}
		if (constraint == Constraint.Landscape)
		{
			cameraSize = 1f / camera.aspect * UnitsSize / 2f;
		}
		else
		{
			cameraSize = UnitsSize / 2f;
		}
		camera.orthographicSize = cameraSize;
		_height = 2f * camera.orthographicSize;
		_width = _height * camera.aspect;
		float x = base.cachedTransform.position.x;
		float y = base.cachedTransform.position.y;
		float x2 = x - _width / 2f;
		float x3 = x + _width / 2f;
		float y2 = y + _height / 2f;
		float y3 = y - _height / 2f;
		_bl = new Vector3(x2, y3, 0f);
		_bc = new Vector3(x, y3, 0f);
		_br = new Vector3(x3, y3, 0f);
		_ml = new Vector3(x2, y, 0f);
		_mc = new Vector3(x, y, 0f);
		_mr = new Vector3(x3, y, 0f);
		_tl = new Vector3(x2, y2, 0f);
		_tc = new Vector3(x, y2, 0f);
		_tr = new Vector3(x3, y2, 0f);
		Instance = this;
		Vector2 v = Vector2.zero;
		Vector2 b = Vector2.zero;
		CameraFollow instance = CameraFollow.instance;
		if (instance != null && instance.targetTransform != null && !BossRaidManager.isBossRaid && GameManager.currentDungeonType != GameManager.SpecialDungeonType.TowerMode)
		{
			b = new Vector2(Mathf.Clamp(instance.targetTransform.position.x, -4.7f - MiddleLeft.x, 4.7f - MiddleRight.x), instance.targetTransform.position.y);
			v = Vector2.Lerp(instance.cachedTransform.position, b, Time.deltaTime * 12f);
			v.y = Mathf.Min(v.y, instance.maxClampPosition.y);
			instance.cachedTransform.position = v;
		}
	}

	public void setCameraSize(float sizePer, Transform target)
	{
		CameraFollow.instance.targetTransform = target;
		targetSize = m_unitsSize * sizePer;
	}

	public void setCameraDefault()
	{
		CameraFollow.instance.targetTransform = null;
		targetSize = -1f;
	}

	private void LateUpdate()
	{
		if (GameManager.currentGameState == GameManager.GameState.Playing && GameManager.currentDungeonType != GameManager.SpecialDungeonType.ElopeMode && (GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon || GameManager.currentDungeonType == GameManager.SpecialDungeonType.TowerMode))
		{
			if (UnitsSize != m_unitsSize || targetSize != -1f)
			{
				ComputeResolution();
			}
		}
		else if (camera.orthographicSize != 7.237906f)
		{
			camera.orthographicSize = 7.237906f;
			if (GameManager.currentDungeonType != GameManager.SpecialDungeonType.TowerMode)
			{
				base.cachedTransform.localPosition = new Vector3(0f, 0.75f, -10f);
			}
			else
			{
				base.cachedTransform.localPosition = new Vector3(0f, 0f, -10f);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = wireColor;
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		if (camera.orthographic)
		{
			float z = camera.farClipPlane - camera.nearClipPlane;
			float z2 = (camera.farClipPlane + camera.nearClipPlane) * 0.5f;
			Gizmos.DrawWireCube(new Vector3(0f, 0f, z2), new Vector3(camera.orthographicSize * 2f * camera.aspect, camera.orthographicSize * 2f, z));
		}
		else
		{
			Gizmos.DrawFrustum(Vector3.zero, camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
		}
		Gizmos.matrix = matrix;
	}
}
