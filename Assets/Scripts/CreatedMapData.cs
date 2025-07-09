using System.Collections.Generic;
using UnityEngine;

public struct CreatedMapData
{
	public GameObject wall;

	public GameObject line;

	public List<GameObject> torches;

	public CreatedMapData(GameObject wall, GameObject line, List<GameObject> torches)
	{
		this.wall = wall;
		this.line = line;
		this.torches = torches;
	}
}
