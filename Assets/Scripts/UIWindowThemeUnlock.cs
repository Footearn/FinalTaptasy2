using UnityEngine;
using UnityEngine.UI;

public class UIWindowThemeUnlock : UIWindow
{
	public enum PopupType
	{
		None,
		Theme,
		SpecialDungeon,
		Elite,
		Rebirth
	}

	public static UIWindowThemeUnlock instance;

	public GameObject[] unlockObjects;

	public Text unlockThemeText;

	public Image unlockThemeImage;

	public Image eliteBossImage;

	public Sprite bossRaidSprite;

	private int m_theme = 2;

	public PopupType currentType;

	public PopupType nextType;

	public override void Awake()
	{
		base.Awake();
		instance = this;
	}

	public void OpenSpecialDungeon(int theme)
	{
		m_theme = theme;
		currentType = PopupType.SpecialDungeon;
		open();
	}

	public void OpenDungeon()
	{
		currentType = PopupType.Theme;
		open();
	}

	public void OpenEliteDungeon()
	{
		currentType = PopupType.Elite;
		open();
	}

	public void OpenRebirth()
	{
		UIWindowOutgame.instance.refreshTreasureIndicator();
		UIWindowManageTreasure.instance.rebirthIndigator.SetActive(true);
		currentType = PopupType.Rebirth;
		open();
	}

	public override bool OnBeforeOpen()
	{
		Singleton<AudioManager>.instance.playEffectSound("questcomplete");
		for (int i = 0; i < unlockObjects.Length; i++)
		{
			unlockObjects[i].SetActive(false);
		}
		Singleton<CachedManager>.instance.darkUI.gameObject.SetActive(true);
		Singleton<CachedManager>.instance.darkUI.setAlpha(1f);
		switch (currentType)
		{
		case PopupType.Theme:
			unlockObjects[0].SetActive(true);
			unlockThemeText.transform.localPosition = new Vector2(0f, 140f);
			unlockThemeText.fontSize = 33;
			m_theme = GameManager.unlockTheme;
			unlockThemeText.text = I18NManager.Get("STAGE_NAME_" + m_theme);
			switch (m_theme)
			{
			case 2:
				unlockThemeImage.transform.localPosition = new Vector2(0f, 71f);
				break;
			case 3:
				unlockThemeImage.transform.localPosition = new Vector2(-53f, 90f);
				break;
			case 4:
				unlockThemeImage.transform.localPosition = new Vector2(71f, 157f);
				break;
			case 5:
				unlockThemeImage.transform.localPosition = new Vector2(42f, 92f);
				break;
			case 6:
				unlockThemeImage.transform.localPosition = new Vector2(-59f, 67f);
				break;
			case 7:
				unlockThemeImage.transform.localPosition = new Vector2(3f, -4f);
				break;
			case 8:
				unlockThemeImage.transform.localPosition = new Vector2(-62f, -19f);
				break;
			case 9:
				unlockThemeImage.transform.localPosition = new Vector2(-69f, 22f);
				break;
			case 10:
				unlockThemeImage.transform.localPosition = new Vector2(-58f, -49f);
				break;
			}
			unlockThemeImage.sprite = Singleton<ResourcesManager>.instance.getAnimation("Stage" + m_theme + "Castle")[0];
			unlockThemeImage.SetNativeSize();
			unlockThemeImage.transform.localPosition = Vector2.zero;
			unlockThemeImage.transform.localScale = Vector3.one;
			break;
		case PopupType.SpecialDungeon:
		{
			unlockObjects[0].SetActive(true);
			unlockThemeText.transform.localPosition = new Vector2(0f, 140f);
			unlockThemeText.fontSize = 33;
			unlockThemeText.text = I18NManager.Get("SPECIAL_DUNGEON_NAME_" + m_theme);
			int theme = m_theme;
			if (theme == 1)
			{
				unlockThemeImage.transform.localPosition = new Vector2(0f, 20f);
			}
			unlockThemeImage.sprite = bossRaidSprite;
			unlockThemeImage.SetNativeSize();
			unlockThemeImage.transform.localScale = new Vector3(1f, 1f, 1f);
			Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
			break;
		}
		case PopupType.Elite:
		{
			unlockObjects[1].SetActive(true);
			unlockThemeText.transform.localPosition = new Vector2(0f, -240f);
			unlockThemeText.fontSize = 33;
			m_theme = GameManager.unlockTheme;
			Sprite[] animation = Singleton<ResourcesManager>.instance.getAnimation(((EnemyManager.BossType)(m_theme - 1)).ToString() + "Elite");
			for (int j = 0; j < animation.Length; j++)
			{
				if (animation[j].name.Contains("Idle"))
				{
					eliteBossImage.sprite = animation[j];
					eliteBossImage.SetNativeSize();
					break;
				}
			}
			unlockThemeText.text = string.Format(I18NManager.Get("NOTICE_ELITE_DUNGEON"), I18NManager.Get("STAGE_NAME_" + m_theme));
			break;
		}
		case PopupType.Rebirth:
			unlockObjects[2].SetActive(true);
			unlockThemeText.transform.localPosition = new Vector2(0f, -280f);
			unlockThemeText.fontSize = 30;
			unlockThemeText.text = I18NManager.Get("NOTICE_REBIRTH");
			break;
		}
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
		return base.OnBeforeOpen();
	}

	public override void OnAfterClose()
	{
		base.OnAfterClose();
		switch (nextType)
		{
		case PopupType.SpecialDungeon:
			OpenSpecialDungeon(2);
			break;
		case PopupType.Rebirth:
			OpenRebirth();
			break;
		case PopupType.Elite:
			break;
		}
	}

	public void OnClickClose()
	{
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
		if (nextType == PopupType.None && currentType == PopupType.Theme)
		{
			switch (m_theme)
			{
			case 3:
				nextType = PopupType.SpecialDungeon;
				close();
				return;
			case 4:
				nextType = PopupType.Rebirth;
				close();
				return;
			}
			nextType = PopupType.None;
			Singleton<GameManager>.instance.gameEnd();
			Singleton<CachedManager>.instance.darkUI.gameObject.SetActive(false);
			Singleton<CachedManager>.instance.darkUI.setAlpha(0f);
			Singleton<CachedManager>.instance.coverUI.fadeInGame();
		}
		else
		{
			nextType = PopupType.None;
			Singleton<GameManager>.instance.gameEnd();
			Singleton<CachedManager>.instance.darkUI.gameObject.SetActive(false);
			Singleton<CachedManager>.instance.darkUI.setAlpha(0f);
			Singleton<CachedManager>.instance.coverUI.fadeInGame();
		}
	}
}
