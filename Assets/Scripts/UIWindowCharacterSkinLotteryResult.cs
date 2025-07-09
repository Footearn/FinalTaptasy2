using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowCharacterSkinLotteryResult : UIWindow
{
	private struct NextSkinData
	{
		public CharacterManager.CharacterType characterType;

		public int skinIndex;

		public bool isNewSkin;

		public bool isLottery;

		public NextSkinData(CharacterManager.CharacterType characterType, int skinIndex, bool isNewSkin, bool isLottery)
		{
			this.characterType = characterType;
			this.skinIndex = skinIndex;
			this.isNewSkin = isNewSkin;
			this.isLottery = isLottery;
		}
	}

	public static UIWindowCharacterSkinLotteryResult instance;

	public Sprite normalLotteryIconSprite;

	public Sprite limitedLotteryIconSprite;

	public Sprite obtainIconSprite;

	public Image lotteryIconImage;

	public GameObject lotteryObject;

	public GameObject resultObject;

	public GameObject sunburstObject;

	public GameObject shyEffectObject;

	public CanvasGroup cachedCanvasGroup;

	public Image titleImage;

	public CharacterUIObject cachedCharacterUIObject;

	public Text nameText;

	public Text characterTypeText;

	public Text firstStatText;

	public RectTransform firstStatTextRectTransform;

	public GameObject secondStatObject;

	public Image secondStatIconImage;

	public Text secondStatText;

	public Image flashImage;

	private bool m_isProgressingLotteryUI;

	private List<NextSkinData> m_nextSkinDataList = new List<NextSkinData>();

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		sunburstObject.SetActive(true);
		shyEffectObject.SetActive(true);
		return base.OnBeforeOpen();
	}

	public override bool OnBeforeClose()
	{
		sunburstObject.SetActive(false);
		shyEffectObject.SetActive(false);
		return base.OnBeforeClose();
	}

	public void openRandomCharacterSkinUI(CharacterSkinManager.WarriorSkinType skinType, bool isNewSkin, bool isLottery = true)
	{
		if (m_isProgressingLotteryUI)
		{
			m_nextSkinDataList.Add(new NextSkinData(CharacterManager.CharacterType.Warrior, (int)skinType, isNewSkin, isLottery));
			return;
		}
		if (isLottery)
		{
			if (Singleton<CharacterSkinManager>.instance.isNormalCharacterSkin(skinType))
			{
				lotteryIconImage.sprite = normalLotteryIconSprite;
			}
			else
			{
				lotteryIconImage.sprite = limitedLotteryIconSprite;
			}
		}
		else
		{
			lotteryIconImage.sprite = obtainIconSprite;
		}
		m_isProgressingLotteryUI = true;
		cachedCharacterUIObject.initCharacterUIObject(skinType, cachedCanvasGroup, "PopUpLayer2", 1200);
		nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(skinType) + " <size=27>Lv." + Mathf.Max((long)Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType)._skinLevel, 1f) + "</size>";
		characterTypeText.text = I18NManager.Get("WARRIOR") + " " + I18NManager.Get("SKIN");
		CharacterSkinStatData characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.warriorCharacterSkinData[skinType];
		firstStatTextRectTransform.anchoredPosition = new Vector2(12.2f, 18f);
		double num = characterSkinStatData.percentDamage * Mathf.Max((long)Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType)._skinLevel, 1f);
		firstStatText.text = "+" + num.ToString("N0") + "%";
		secondStatIconImage.sprite = Singleton<CachedManager>.instance.warriorSecondStatIconSprite;
		double num2 = characterSkinStatData.secondStat * Mathf.Max((long)Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType)._skinLevel, 1f);
		secondStatText.text = "+" + num2.ToString("N0") + "%";
		secondStatIconImage.SetNativeSize();
		secondStatObject.SetActive(true);
		changeTitleImage(isNewSkin);
		open();
		startLottery();
	}

	public void openRandomCharacterSkinUI(CharacterSkinManager.PriestSkinType skinType, bool isNewSkin, bool isLottery = true)
	{
		if (m_isProgressingLotteryUI)
		{
			m_nextSkinDataList.Add(new NextSkinData(CharacterManager.CharacterType.Priest, (int)skinType, isNewSkin, isLottery));
			return;
		}
		if (isLottery)
		{
			if (Singleton<CharacterSkinManager>.instance.isNormalCharacterSkin(skinType))
			{
				lotteryIconImage.sprite = normalLotteryIconSprite;
			}
			else
			{
				lotteryIconImage.sprite = limitedLotteryIconSprite;
			}
		}
		else
		{
			lotteryIconImage.sprite = obtainIconSprite;
		}
		m_isProgressingLotteryUI = true;
		cachedCharacterUIObject.initCharacterUIObject(skinType, cachedCanvasGroup, "PopUpLayer2", 1200);
		nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(skinType) + " <size=27>Lv." + Mathf.Max((long)Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType)._skinLevel, 1f) + "</size>";
		characterTypeText.text = I18NManager.Get("PRIEST") + " " + I18NManager.Get("SKIN");
		CharacterSkinStatData characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.priestCharacterSkinData[skinType];
		firstStatTextRectTransform.anchoredPosition = new Vector2(12.2f, 18f);
		double num = characterSkinStatData.percentDamage * Mathf.Max((long)Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType)._skinLevel, 1f);
		firstStatText.text = "+" + num.ToString("N0") + "%";
		secondStatIconImage.sprite = Singleton<CachedManager>.instance.priestSecondStatIconSprite;
		double num2 = characterSkinStatData.secondStat * Mathf.Max((long)Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType)._skinLevel, 1f);
		secondStatText.text = "+" + num2.ToString("N0") + "%";
		secondStatIconImage.SetNativeSize();
		secondStatObject.SetActive(true);
		changeTitleImage(isNewSkin);
		open();
		startLottery();
	}

	public void openRandomCharacterSkinUI(CharacterSkinManager.ArcherSkinType skinType, bool isNewSkin, bool isLottery = true)
	{
		if (m_isProgressingLotteryUI)
		{
			m_nextSkinDataList.Add(new NextSkinData(CharacterManager.CharacterType.Archer, (int)skinType, isNewSkin, isLottery));
			return;
		}
		if (isLottery)
		{
			if (Singleton<CharacterSkinManager>.instance.isNormalCharacterSkin(skinType))
			{
				lotteryIconImage.sprite = normalLotteryIconSprite;
			}
			else
			{
				lotteryIconImage.sprite = limitedLotteryIconSprite;
			}
		}
		else
		{
			lotteryIconImage.sprite = obtainIconSprite;
		}
		m_isProgressingLotteryUI = true;
		cachedCharacterUIObject.initCharacterUIObject(skinType, cachedCanvasGroup, "PopUpLayer2", 1200);
		characterTypeText.text = I18NManager.Get("ARCHER") + " " + I18NManager.Get("SKIN");
		nameText.text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(skinType) + " <size=27>Lv." + Mathf.Max((long)Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType)._skinLevel, 1f) + "</size>";
		CharacterSkinStatData characterSkinStatData = Singleton<ParsingManager>.instance.currentParsedStatData.archerCharacterSkinData[skinType];
		firstStatTextRectTransform.anchoredPosition = new Vector2(12.2f, 0f);
		double num = characterSkinStatData.percentDamage * Mathf.Max((long)Singleton<CharacterSkinManager>.instance.getSkinDataFromInventory(skinType)._skinLevel, 1f);
		firstStatText.text = "+" + num.ToString("N0") + "%";
		secondStatObject.SetActive(false);
		changeTitleImage(isNewSkin);
		open();
		startLottery();
	}

	private void changeTitleImage(bool isNewSkin)
	{
		titleImage.sprite = ((!isNewSkin) ? Singleton<CachedManager>.instance.levelUpSkinTitleImage : Singleton<CachedManager>.instance.newSkinTitleImage);
		titleImage.SetNativeSize();
	}

	private void startLottery()
	{
		Singleton<AudioManager>.instance.playEffectSound("skin_draw");
		flashImage.color = new Color(1f, 1f, 1f, 0f);
		lotteryObject.SetActive(true);
		resultObject.SetActive(false);
		StopAllCoroutines();
		StartCoroutine("waitForEndEffect");
	}

	private IEnumerator waitForEndEffect()
	{
		yield return new WaitForSeconds(2.2f);
		float alpha2 = 0f;
		while (alpha2 < 1f)
		{
			Color color = flashImage.color;
			color.a = alpha2;
			flashImage.color = color;
			alpha2 += Time.deltaTime * GameManager.timeScale * 5f;
			yield return null;
		}
		alpha2 = 1f;
		flashImage.color = Color.white;
		lotteryObject.SetActive(false);
		resultObject.SetActive(true);
		while (alpha2 > 0f)
		{
			Color color2 = flashImage.color;
			color2.a = alpha2;
			flashImage.color = color2;
			alpha2 -= Time.deltaTime * GameManager.timeScale * 5f;
			yield return null;
		}
		m_isProgressingLotteryUI = false;
		flashImage.color = new Color(1f, 1f, 1f, 0f);
	}

	public void OnClickClose()
	{
		if (m_nextSkinDataList.Count > 0)
		{
			switch (m_nextSkinDataList[0].characterType)
			{
			case CharacterManager.CharacterType.Warrior:
				openRandomCharacterSkinUI((CharacterSkinManager.WarriorSkinType)m_nextSkinDataList[0].skinIndex, m_nextSkinDataList[0].isNewSkin, m_nextSkinDataList[0].isLottery);
				break;
			case CharacterManager.CharacterType.Priest:
				openRandomCharacterSkinUI((CharacterSkinManager.PriestSkinType)m_nextSkinDataList[0].skinIndex, m_nextSkinDataList[0].isNewSkin, m_nextSkinDataList[0].isLottery);
				break;
			case CharacterManager.CharacterType.Archer:
				openRandomCharacterSkinUI((CharacterSkinManager.ArcherSkinType)m_nextSkinDataList[0].skinIndex, m_nextSkinDataList[0].isNewSkin, m_nextSkinDataList[0].isLottery);
				break;
			}
			m_nextSkinDataList.RemoveAt(0);
		}
		else
		{
			close();
		}
	}
}
