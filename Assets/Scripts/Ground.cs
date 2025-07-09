using UnityEngine;

public class Ground : ObjectBase
{
	public Transform outPoint;

	public Transform inPoint;

	public GameObject shadowObject;

	public bool isFirstGround;

	public Transform downPoint;

	public bool isBossGround;

	public bool isBossRaidGround;

	public Transform[] stairpoint;

	public TextMesh floorText;

	public TextMesh textShadow;

	public SpriteRenderer stairRenderer;

	public SpriteRenderer floorRender;

	public GameObject floorTextObject;

	public int currnetFloor;
}
