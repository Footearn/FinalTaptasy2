using System;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroProcess : MonoBehaviour
{
	private bool isAuthFinished;

	private AsyncOperation async;

	public Text textTopMessage;

	public Text textWarnHeader;

	public Text textWarnContent;

	public Image imageBackground;

	public Image imageBossMonster;

	public CanvasGroup backgroundCanvasGroup;

	public GameObject warningObject;

	public GameObject elopeBackgroundObject;

	public GameObject normalBackgroundObject;

	private string[] m_translatedValue;

	private DateTime timeoutCheckDate;

	private void Awake()
	{
		m_translatedValue = getText();
		timeoutCheckDate = DateTime.Now.AddSeconds(30.0);
		bool flag = ((PlayerPrefs.GetInt("IsCanEnterElopeMode", 0) == 1) ? true : false);
		elopeBackgroundObject.SetActive(flag);
		normalBackgroundObject.SetActive(!flag);
	}

	private void Start()
	{
		PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		PlayGamesPlatform.InitializeInstance(configuration);
		PlayGamesPlatform.DebugLogEnabled = false;
		PlayGamesPlatform.Activate();
		textTopMessage.text = m_translatedValue[0];
		textWarnHeader.text = m_translatedValue[1];
		textWarnContent.text = m_translatedValue[2];
		StartCoroutine("fadeInUpdate");
	}

	private IEnumerator fadeInUpdate()
	{
		while (backgroundCanvasGroup.alpha < 1f)
		{
			backgroundCanvasGroup.alpha += Time.deltaTime * 5f;
			yield return null;
		}
		backgroundCanvasGroup.alpha = 1f;
		timeoutCheckDate = DateTime.Now.AddSeconds(7.0);
		Social.localUser.Authenticate(delegate (bool success)
		{
			isAuthFinished = true;
		});
		yield return new WaitForSeconds(0.5f);
	}

	private void Update()
	{
		if (isAuthFinished || DateTime.Now.CompareTo(timeoutCheckDate) > 0)
		{
			isAuthFinished = false;
			imageBackground = null;
			imageBossMonster = null;
			Resources.UnloadUnusedAssets();
			GC.Collect();
			SceneManager.LoadScene("Ingame");
		}
	}

	private string[] getText()
	{
		string[] array = new string[3]
		{
			string.Empty,
			string.Empty,
			string.Empty
		};
		I18NManager.Language language = (I18NManager.Language)NSPlayerPrefs.GetInt("CurrentLanguage", 0);
		if (language == I18NManager.Language.None)
		{
			switch (Application.systemLanguage)
			{
			case SystemLanguage.Korean:
				language = I18NManager.Language.Korean;
				break;
			case SystemLanguage.ChineseSimplified:
				language = I18NManager.Language.ChineseSimplified;
				break;
			case SystemLanguage.ChineseTraditional:
				language = I18NManager.Language.ChineseTraditional;
				break;
			case SystemLanguage.Japanese:
				language = I18NManager.Language.Japanese;
				break;
			default:
				language = I18NManager.Language.English;
				break;
			}
		}
		switch (language)
		{
		case I18NManager.Language.Korean:
			return new string[3]
			{
				"로그인 중...",
				"주의",
				"저장하지 않은 데이터는 기기를 변경하거나 게임을 삭제시 복구되지 않습니다."
			};
		case I18NManager.Language.English:
			return new string[3]
			{
				"Signing in...",
				"Warning",
				"Game data cannot be\nrestored unless saved."
			};
		case I18NManager.Language.Japanese:
			return new string[3]
			{
				"ログイン中…",
				"注意",
				"セーブしていないデータは、機器の変更や\nゲームを削除すると復旧できません。"
			};
		case I18NManager.Language.ChineseSimplified:
			return new string[3]
			{
				"正在登陆",
				"注意",
				"未储存的数据在更换设备或卸载游戏时将无法恢复。"
			};
		case I18NManager.Language.ChineseTraditional:
			return new string[3]
			{
				"載入中",
				"注意",
				"更改設備或刪除遊戲時將無法還原尚未儲存的數據。"
			};
		case I18NManager.Language.Thai:
			return new string[3]
			{
				"ลงช\u0e37\u0e48อเข\u0e49าใช\u0e49...",
				"คำเต\u0e37อน",
				"ข\u0e49อม\u0e39ลเกมไม\u0e48สามารถเร\u0e35ยกค\u0e37นได\u0e49 จนกว\u0e48าจะม\u0e35การบ\u0e31นท\u0e36กไว\u0e49"
			};
		case I18NManager.Language.Russian:
			return new string[3]
			{
				"Вход...",
				"Внимание!",
				"Для восстановления данных игры нужно сначала сохранить их."
			};
		default:
			return new string[3]
			{
				"Signing in...",
				"Warning",
				"Game data cannot be\nrestored unless saved."
			};
		}
	}
}
