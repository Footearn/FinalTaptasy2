using UnityEngine;
using UnityEngine.UI;

public class DynamicFontObject : MonoBehaviour
{
	public Text cachedText;

	private void Reset()
	{
		cachedText = GetComponent<Text>();
	}

	private void Awake()
	{
		if (cachedText == null)
		{
			cachedText = GetComponent<Text>();
		}
		Singleton<FontManager>.instance.dynamicFontObjectList.Add(this);
	}

	private void OnEnable()
	{
		Singleton<FontManager>.instance.changeFontOfText(this);
	}
}
