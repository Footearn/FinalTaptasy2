using System;
using System.Collections.Generic;
using UnityEngine;

public class I18NManager : Singleton<I18NManager>
{
	public enum Language
	{
		None,
		Korean,
		English,
		ChineseSimplified,
		ChineseTraditional,
		Japanese,
		Thai,
		Russian,
		Length
	}

	public static Language currentLanguage;

	private Action m_changeLanguageNotification;

	public Dictionary<string, Dictionary<Language, string>> localLanguageData = new Dictionary<string, Dictionary<Language, string>>();

	public static Action changeLanguageNotification
	{
		get
		{
			return Singleton<I18NManager>.instance.m_changeLanguageNotification;
		}
		set
		{
			Singleton<I18NManager>.instance.m_changeLanguageNotification = value;
		}
	}

	public static bool isInternetConnection()
	{
		bool result = false;
		if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			result = true;
		}
		return result;
	}

	private void Awake()
	{
		LoadData();
		SetDefaultLanguage();
		setLocalDefaultI18NData();
	}

	private Dictionary<Language, string> makeLocalI18NData(string koreaText, string englishText, string simplefiedChineseText, string traditionalChineseText, string japaneseText, string thai, string russian)
	{
		Dictionary<Language, string> dictionary = new Dictionary<Language, string>();
		dictionary.Add(Language.Korean, koreaText);
		dictionary.Add(Language.English, englishText);
		dictionary.Add(Language.ChineseSimplified, simplefiedChineseText);
		dictionary.Add(Language.ChineseTraditional, traditionalChineseText);
		dictionary.Add(Language.Japanese, japaneseText);
		dictionary.Add(Language.Thai, thai);
		dictionary.Add(Language.Russian, russian);
		return dictionary;
	}

	public void setLocalDefaultI18NData()
	{
		localLanguageData.Add("LOADING_DATA_PACK_STATE_TEXT_1", makeLocalI18NData("공주님 화장하는 중...", "Princess is doing make up...", "公主殿下正在化妆…", "公主殿下化妝中…", "化粧中…", "เจ\u0e49าหญ\u0e34งกำล\u0e31งแต\u0e48งหน\u0e49า...", "Принцесса прихорашивается..."));
		localLanguageData.Add("LOADING_DATA_PACK_STATE_TEXT_2", makeLocalI18NData("용사 라면 먹는 중...", "Hero is eating noodles...", "勇士正在吃泡面…", "勇士正在吃拉麵…", "お食事中…", "ฮ\u0e35โร\u0e48กำล\u0e31งทานราเมงอย\u0e48างเอร\u0e47ดอร\u0e48อย", "Герой поглощает обед…"));
		localLanguageData.Add("LOADING_DATA_PACK_STATE_TEXT_3", makeLocalI18NData("마왕 수트 입는 중...", "Daemon King is dressing up...", "魔王正在穿衣服…", "魔王正在換衣服…", "スーツ着用中…", "ราชาป\u0e35ศาจกำล\u0e31งแต\u0e48งต\u0e31ว...", "Король демонов красуется перед зеркалом..."));
		localLanguageData.Add("LOADING_DATA_PACK_STATE_TEXT_4", makeLocalI18NData("몬스터들 긴장하는 중...", "Monsters are getting nervous...", "怪物们很紧张…", "怪獸們覺得很緊張…", "モンスター緊張中…", "มอนสเตอร\u0e4cกำล\u0e31งกระวนกระวาย...", "Чудовища нервничают..."));
		localLanguageData.Add("LOADING_DATA_PACK_STATE_TEXT_5", makeLocalI18NData("로딩 완료!", "Loading Complete!", "成功载入！", "載入完成！", "ローディング完了！", "การโหลดเสร\u0e47จสมบ\u0e39รณ\u0e4c!", "Загрузка завершена!"));
		localLanguageData.Add("CLOSE", makeLocalI18NData("닫기", "Close", "关闭", "關閉", "閉じる", "ป\u0e34ด", "Закрыть"));
		localLanguageData.Add("OK", makeLocalI18NData("확인", "Ok", "确认", "確認", "確認", "ตกลง", "ОК"));
		localLanguageData.Add("YES", makeLocalI18NData("예", "Yes", "是", "是", "はい", "ใช\u0e48", "да"));
		localLanguageData.Add("NO", makeLocalI18NData("아니오", "No", "否", "否", "いいえ", "ไม\u0e48", "Нет"));
	}

	public void LoadData()
	{
		SetDefaultLanguage();
	}

	public void setI18NData(string text, ParsedStatData targetStatData)
	{
		targetStatData.languageData = new Dictionary<string, Dictionary<Language, string>>();
		string[] array = text.Split('\n');
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			Dictionary<Language, string> dictionary = null;
			string key = array2[0];
			if (!targetStatData.languageData.ContainsKey(key))
			{
				dictionary = new Dictionary<Language, string>();
				targetStatData.languageData.Add(key, dictionary);
			}
			else
			{
				dictionary = targetStatData.languageData[key];
			}
			for (int j = 1; j < array2.Length; j++)
			{
				dictionary.Add((Language)j, array2[j]);
			}
		}
	}

	public static string GetLocalI18N(string title)
	{
		return Singleton<I18NManager>.instance.getLocalI18N(title);
	}

	public static bool ContainsKey(string title)
	{
		return Singleton<ParsingManager>.instance.currentParsedStatData.languageData.ContainsKey(title);
	}

	private string getLocalI18N(string title)
	{
		string result = string.Empty;
		if (localLanguageData.ContainsKey(title))
		{
			result = localLanguageData[title][currentLanguage];
			result = result.Replace('^', '\n');
			result = result.Replace("\"", string.Empty);
		}
		return result;
	}

	public static bool Contains(string title)
	{
		return Singleton<ParsingManager>.instance.currentParsedStatData.languageData.ContainsKey(title);
	}

	public static string Get(string title)
	{
		string result = string.Empty;
		if (Singleton<ParsingManager>.instance.currentParsedStatData.languageData.ContainsKey(title))
		{
			result = Singleton<ParsingManager>.instance.currentParsedStatData.languageData[title][currentLanguage];
			result = result.Replace('^', '\n');
			result = result.Replace("\"", string.Empty);
		}
		return result;
	}

	public static string Get(string title, Language targetLanguage)
	{
		string result = string.Empty;
		if (Singleton<ParsingManager>.instance.currentParsedStatData.languageData.ContainsKey(title))
		{
			result = Singleton<ParsingManager>.instance.currentParsedStatData.languageData[title][targetLanguage];
			result = result.Replace('^', '\n');
			result = result.Replace("\"", string.Empty);
		}
		return result;
	}

	public static void ChangeLanguage(Language targetLanguage)
	{
		if (currentLanguage != targetLanguage)
		{
			NSPlayerPrefs.SetInt("CurrentLanguage", (int)targetLanguage);
			currentLanguage = targetLanguage;
			changeLanguageNotification();
		}
	}

	public void ChangeLanguage(int targetLanguage)
	{
		if (currentLanguage != (Language)targetLanguage)
		{
			NSPlayerPrefs.SetInt("CurrentLanguage", targetLanguage);
			currentLanguage = (Language)targetLanguage;
			changeLanguageNotification();
		}
	}

	private void SetDefaultLanguage()
	{
		currentLanguage = (Language)PlayerPrefs.GetInt("CurrentLanguage", 0);
		if (currentLanguage == Language.None)
		{
			switch (Application.systemLanguage)
			{
			case SystemLanguage.Korean:
				currentLanguage = Language.Korean;
				break;
			case SystemLanguage.English:
				currentLanguage = Language.English;
				break;
			case SystemLanguage.ChineseSimplified:
				currentLanguage = Language.ChineseSimplified;
				break;
			case SystemLanguage.ChineseTraditional:
				currentLanguage = Language.ChineseTraditional;
				break;
			case SystemLanguage.Japanese:
				currentLanguage = Language.Japanese;
				break;
			case SystemLanguage.Thai:
				currentLanguage = Language.Thai;
				break;
			case SystemLanguage.Russian:
				currentLanguage = Language.Russian;
				break;
			default:
				currentLanguage = Language.English;
				break;
			}
			PlayerPrefs.SetInt("CurrentLanguage", (int)currentLanguage);
		}
	}
}
