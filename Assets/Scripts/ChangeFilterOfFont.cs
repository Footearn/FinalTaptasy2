using UnityEngine;
using UnityEngine.UI;

public class ChangeFilterOfFont : MonoBehaviour
{
	public FilterMode filterMove;

	public Text cachedText;

	private void Reset()
	{
		cachedText = GetComponent<Text>();
		cachedText.mainTexture.filterMode = filterMove;
	}

	private void Awake()
	{
		cachedText.mainTexture.filterMode = filterMove;
	}
}
