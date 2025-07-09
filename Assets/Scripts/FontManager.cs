using System;
using System.Collections.Generic;
using UnityEngine;

public class FontManager : Singleton<FontManager>
{
	public List<DynamicFontObject> dynamicFontObjectList = new List<DynamicFontObject>();

	public Font pixelFont;

	public Font thaiFont;

	public Font englishFont;

	private void Awake()
	{
		thaiFont.material.mainTexture.filterMode = FilterMode.Bilinear;
		englishFont.material.mainTexture.filterMode = FilterMode.Bilinear;
		pixelFont.material.mainTexture.filterMode = FilterMode.Point;
		I18NManager.changeLanguageNotification = (Action)Delegate.Combine(I18NManager.changeLanguageNotification, new Action(changeAllFont));
	}

	public void changeAllFont()
	{
		for (int i = 0; i < dynamicFontObjectList.Count; i++)
		{
			changeFontOfText(dynamicFontObjectList[i]);
		}
	}

	public void changeFontOfText(DynamicFontObject dynamicFontObject)
	{
		if (dynamicFontObject != null)
		{
			Texture mainTexture = dynamicFontObject.cachedText.font.material.mainTexture;
			I18NManager.Language currentLanguage = I18NManager.currentLanguage;
			if (currentLanguage == I18NManager.Language.Thai)
			{
				dynamicFontObject.cachedText.font = thaiFont;
				dynamicFontObject.cachedText.fontStyle = FontStyle.Bold;
			}
			else
			{
				dynamicFontObject.cachedText.font = englishFont;
				dynamicFontObject.cachedText.fontStyle = FontStyle.Normal;
			}
		}
	}
}
