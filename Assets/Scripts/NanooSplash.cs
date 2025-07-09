using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NanooSplash : MonoBehaviour
{
	public Image logoImage;

	public Image backgroundImage;

	private void Awake()
	{
		DebugManager.Log("Resolution Before : " + Screen.width + " x " + Screen.height);
		StartCoroutine(changeResolution());
	}

	private IEnumerator changeResolution()
	{
		int originHeight = Screen.height;
		int originWidth = Screen.width;
		PlayerPrefs.SetInt("OriginWidth", originWidth);
		PlayerPrefs.SetInt("OriginHeight", originHeight);
		int height = Mathf.Min(960, Screen.height);
		int width = (int)((float)height * (float)originWidth / (float)originHeight);
		if (NSPlayerPrefs.GetInt("HDMode", 0) == 0)
		{
			Screen.SetResolution(width, height, Screen.fullScreen);
		}
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(0.2f);
		backgroundImage.color = Util.getCalculatedColor(36f, 36f, 36f);
	}

	public void loadScene()
	{
		logoImage.sprite = null;
		logoImage = null;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		SceneManager.LoadScene("Intro");
	}
}
