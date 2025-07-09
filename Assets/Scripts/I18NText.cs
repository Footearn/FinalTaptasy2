using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DynamicFontObject))]
public class I18NText : MonoBehaviour
{
	public string id;

	public bool isUsingLocalI18NData;

	public string[] stringFormat;

	public Text cachedText;

	public TextMeshProUGUI cachedTextMeshPro;

	private I18NManager.Language m_currentLanguage;

	private void Reset()
	{
		cachedText = GetComponent<Text>();
		if (cachedText == null)
		{
			cachedTextMeshPro = GetComponent<TextMeshProUGUI>();
		}
		if (cachedTextMeshPro == null && !(cachedText == null))
		{
		}
	}

	private void Start()
	{
		getText();
	}

	private void OnEnable()
	{
		I18NManager.changeLanguageNotification = (Action)Delegate.Combine(I18NManager.changeLanguageNotification, new Action(changeLanguageNotification));
		getText();
	}

	private void OnDisable()
	{
		I18NManager.changeLanguageNotification = (Action)Delegate.Remove(I18NManager.changeLanguageNotification, new Action(changeLanguageNotification));
	}

	public void changeLanguageNotification()
	{
		getText();
	}

	private void getText()
	{
		if (m_currentLanguage == I18NManager.currentLanguage)
		{
			return;
		}
		m_currentLanguage = I18NManager.currentLanguage;
		if (!isUsingLocalI18NData)
		{
			if (cachedText != null)
			{
				if (stringFormat != null && stringFormat.Length > 0)
				{
					cachedText.text = string.Format(I18NManager.Get(id), stringFormat);
				}
				else
				{
					cachedText.text = I18NManager.Get(id);
				}
			}
			else if (stringFormat != null && stringFormat.Length > 0)
			{
				cachedTextMeshPro.text = string.Format(I18NManager.Get(id), stringFormat);
			}
			else
			{
				cachedTextMeshPro.text = I18NManager.Get(id);
			}
		}
		else if (cachedText != null)
		{
			if (stringFormat != null && stringFormat.Length > 0)
			{
				cachedText.text = string.Format(I18NManager.GetLocalI18N(id), stringFormat);
			}
			else
			{
				cachedText.text = I18NManager.GetLocalI18N(id);
			}
		}
		else if (stringFormat != null && stringFormat.Length > 0)
		{
			cachedTextMeshPro.text = string.Format(I18NManager.GetLocalI18N(id), stringFormat);
		}
		else
		{
			cachedTextMeshPro.text = I18NManager.GetLocalI18N(id);
		}
	}
}
