using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingLanguageSlot : MonoBehaviour
{
	public static List<SettingLanguageSlot> currentSettingLanguageSlotList = new List<SettingLanguageSlot>();

	public I18NManager.Language currentLanguage;

	public Image buttonBackgroundImage;

	public Text languageText;

	private void Awake()
	{
		currentSettingLanguageSlotList.Add(this);
	}

	private void OnEnable()
	{
		refreshLanguageSlot();
	}

	private void refreshLanguageSlot()
	{
		if (currentLanguage == I18NManager.currentLanguage)
		{
			languageText.text = I18NManager.Get("SETTING_LANGUAGE_SELECTED");
			return;
		}
		switch (currentLanguage)
		{
		case I18NManager.Language.Korean:
			languageText.text = I18NManager.Get("SETTING_LANGUAGE_KOREAN");
			break;
		case I18NManager.Language.English:
			languageText.text = I18NManager.Get("SETTING_LANGUAGE_ENGLISH");
			break;
		case I18NManager.Language.ChineseSimplified:
			languageText.text = I18NManager.Get("SETTING_LANGUAGE_CHINESE_SIMPLIFIED");
			break;
		case I18NManager.Language.ChineseTraditional:
			languageText.text = I18NManager.Get("SETTING_LANGUAGE_CHINESE_TRADITIONAL");
			break;
		case I18NManager.Language.Japanese:
			languageText.text = I18NManager.Get("SETTING_LANGUAGE_JAPANESE");
			break;
		case I18NManager.Language.Thai:
			languageText.text = I18NManager.Get("SETTING_LANGUAGE_THAI_LANGUAGE");
			break;
		case I18NManager.Language.Russian:
			languageText.text = I18NManager.Get("SETTING_LANGUAGE_RUSSIAN");
			break;
		}
	}

	public void OnClickChangeLanguage()
	{
		I18NManager.ChangeLanguage(currentLanguage);
		UIWindowSetting.instance.refreshSetting(false);
		for (int i = 0; i < currentSettingLanguageSlotList.Count; i++)
		{
			currentSettingLanguageSlotList[i].refreshLanguageSlot();
		}
	}
}
