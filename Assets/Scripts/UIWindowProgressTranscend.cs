using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowProgressTranscend : UIWindow
{
	public static UIWindowProgressTranscend instance;

	public CharacterUIObject currentCharacterUIObject;

	public RectTransform centerTransform;

	public CanvasGroup cachedCanvasGroup;

	public Text currentTranscendTierText;

	public Image flashImage;

	public GameObject resultObject;

	public GameObject effectObject;

	public Button[] closeButtons;

	public Text skillNameText;

	public Text skillDescriptionText;

	public GameObject skinUnlockedObject;

	public GameObject completeObject;

	private long m_targetTranscendTier;

	private CharacterManager.CharacterType m_targetCharacterType;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void startTranscend(CharacterManager.CharacterType targetType, long targetTranscendTier)
	{
		for (int i = 0; i < closeButtons.Length; i++)
		{
			closeButtons[i].interactable = false;
		}
		completeObject.SetActive(true);
		effectObject.SetActive(true);
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
		m_targetTranscendTier = targetTranscendTier;
		m_targetCharacterType = targetType;
		isCanCloseESC = false;
		StopAllCoroutines();
		resultObject.SetActive(false);
		flashImage.color = new Color(1f, 1f, 1f, 0f);
		createCharacterObject(true);
		currentTranscendTierText.text = string.Format(I18NManager.Get("TRANSCEND_TIER"), m_targetTranscendTier + 1);
		open();
		Singleton<AudioManager>.instance.playEffectSound("transcend");
		StartCoroutine("transcendUpdate");
	}

	public override bool OnBeforeClose()
	{
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
		completeObject.SetActive(false);
		return base.OnBeforeClose();
	}

	private void createCharacterObject(bool alwaysFollowCurrentCharacter)
	{
		if (currentCharacterUIObject != null)
		{
			ObjectPool.Recycle(currentCharacterUIObject.name, currentCharacterUIObject.cachedGameObject);
		}
		currentCharacterUIObject = null;
		string poolName = "@CharacterUIObject";
		currentCharacterUIObject = ObjectPool.Spawn(poolName, new Vector3(0f, -32.3f, 0f), Vector3.zero, new Vector3(150f, 150f, 150f), centerTransform).GetComponent<CharacterUIObject>();
		currentCharacterUIObject.RefreshMaterial();
		bool flag = true;
		if (!alwaysFollowCurrentCharacter && (m_targetTranscendTier == 1 || m_targetTranscendTier == 5 || m_targetTranscendTier == 10))
		{
			flag = false;
		}
		switch (m_targetCharacterType)
		{
		case CharacterManager.CharacterType.Warrior:
		{
			if (flag)
			{
				CharacterSkinManager.WarriorSkinType equippedWarriorSkin = Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin;
				currentCharacterUIObject.initCharacterUIObject(equippedWarriorSkin, cachedCanvasGroup, "PopUpLayer2");
				break;
			}
			CharacterSkinManager.WarriorSkinType targetCharacterSkinType3 = CharacterSkinManager.WarriorSkinType.Length;
			if (m_targetTranscendTier == 1)
			{
				targetCharacterSkinType3 = CharacterSkinManager.WarriorSkinType.Arthur;
			}
			else if (m_targetTranscendTier == 5)
			{
				targetCharacterSkinType3 = CharacterSkinManager.WarriorSkinType.BlackKnight;
			}
			else if (m_targetTranscendTier == 10)
			{
				targetCharacterSkinType3 = CharacterSkinManager.WarriorSkinType.Phoenix;
			}
			currentCharacterUIObject.initCharacterUIObject(targetCharacterSkinType3, cachedCanvasGroup, "PopUpLayer2");
			break;
		}
		case CharacterManager.CharacterType.Priest:
		{
			if (flag)
			{
				CharacterSkinManager.PriestSkinType equippedPriestSkin = Singleton<DataManager>.instance.currentGameData.equippedPriestSkin;
				currentCharacterUIObject.initCharacterUIObject(equippedPriestSkin, cachedCanvasGroup, "PopUpLayer2");
				break;
			}
			CharacterSkinManager.PriestSkinType targetCharacterSkinType2 = CharacterSkinManager.PriestSkinType.Length;
			if (m_targetTranscendTier == 1)
			{
				targetCharacterSkinType2 = CharacterSkinManager.PriestSkinType.Michael;
			}
			else if (m_targetTranscendTier == 5)
			{
				targetCharacterSkinType2 = CharacterSkinManager.PriestSkinType.Criselda;
			}
			else if (m_targetTranscendTier == 10)
			{
				targetCharacterSkinType2 = CharacterSkinManager.PriestSkinType.Elisha;
			}
			currentCharacterUIObject.initCharacterUIObject(targetCharacterSkinType2, cachedCanvasGroup, "PopUpLayer2");
			break;
		}
		case CharacterManager.CharacterType.Archer:
		{
			if (flag)
			{
				CharacterSkinManager.ArcherSkinType equippedArcherSkin = Singleton<DataManager>.instance.currentGameData.equippedArcherSkin;
				currentCharacterUIObject.initCharacterUIObject(equippedArcherSkin, cachedCanvasGroup, "PopUpLayer2");
				break;
			}
			CharacterSkinManager.ArcherSkinType targetCharacterSkinType = CharacterSkinManager.ArcherSkinType.Length;
			if (m_targetTranscendTier == 1)
			{
				targetCharacterSkinType = CharacterSkinManager.ArcherSkinType.Patricia;
			}
			else if (m_targetTranscendTier == 5)
			{
				targetCharacterSkinType = CharacterSkinManager.ArcherSkinType.Claude;
			}
			else if (m_targetTranscendTier == 10)
			{
				targetCharacterSkinType = CharacterSkinManager.ArcherSkinType.Kyle;
			}
			currentCharacterUIObject.initCharacterUIObject(targetCharacterSkinType, cachedCanvasGroup, "PopUpLayer2");
			break;
		}
		}
	}

	private IEnumerator transcendUpdate()
	{
		yield return new WaitForSeconds(1.5f);
		Color color = new Color(1f, 1f, 1f, 0f);
		while (color.a < 1f)
		{
			color = flashImage.color;
			color.a += Time.deltaTime * 2.5f;
			flashImage.color = color;
			yield return null;
		}
		flashImage.color = new Color(1f, 1f, 1f, 1f);
		yield return new WaitForSeconds(0.45f);
		resultObject.SetActive(true);
		Singleton<AudioManager>.instance.playEffectSound("result_clear");
		Singleton<TranscendManager>.instance.transcendTierUp(m_targetCharacterType);
		UIWindowCharacterSkin.instance.openCharacterSkinUI(UIWindowCharacterSkin.instance.currentType);
		m_targetTranscendTier++;
		TranscendPassiveSkillInventoryData skillData2 = null;
		if (m_targetTranscendTier == 1)
		{
			switch (m_targetCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				skillNameText.text = string.Format(I18NManager.Get("TRANSCEND_COMPLETE_SKILL_UNLOCK"), Singleton<TranscendManager>.instance.getTranscendPassiveSkillNameFromI18N(TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage));
				skillData2 = Singleton<TranscendManager>.instance.getTranscendPassiveSkillInventoryData(TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage);
				skillDescriptionText.text = Singleton<TranscendManager>.instance.getTranscendPassiveSkillDescriptionFromI18N(TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage, m_targetTranscendTier, skillData2.skillLevel, false, 0.0);
				break;
			case CharacterManager.CharacterType.Priest:
				skillNameText.text = string.Format(I18NManager.Get("TRANSCEND_COMPLETE_SKILL_UNLOCK"), Singleton<TranscendManager>.instance.getTranscendPassiveSkillNameFromI18N(TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage));
				skillData2 = Singleton<TranscendManager>.instance.getTranscendPassiveSkillInventoryData(TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage);
				skillDescriptionText.text = Singleton<TranscendManager>.instance.getTranscendPassiveSkillDescriptionFromI18N(TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage, m_targetTranscendTier, skillData2.skillLevel, false, 0.0);
				break;
			case CharacterManager.CharacterType.Archer:
				skillNameText.text = string.Format(I18NManager.Get("TRANSCEND_COMPLETE_SKILL_UNLOCK"), Singleton<TranscendManager>.instance.getTranscendPassiveSkillNameFromI18N(TranscendManager.TranscendPassiveSkillType.PenetrationArrow));
				skillData2 = Singleton<TranscendManager>.instance.getTranscendPassiveSkillInventoryData(TranscendManager.TranscendPassiveSkillType.PenetrationArrow);
				skillDescriptionText.text = Singleton<TranscendManager>.instance.getTranscendPassiveSkillDescriptionFromI18N(TranscendManager.TranscendPassiveSkillType.PenetrationArrow, m_targetTranscendTier, skillData2.skillLevel, false, 0.0);
				break;
			}
		}
		else
		{
			double increaseChance4 = 0.0;
			switch (m_targetCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				skillNameText.text = string.Format(I18NManager.Get("TRANSCEND_COMPLETE_SKILL_UPGRADED"), Singleton<TranscendManager>.instance.getTranscendPassiveSkillNameFromI18N(TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage));
				increaseChance4 = Singleton<TranscendManager>.instance.getTranscendPassiveSkillCastChance(TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage, m_targetTranscendTier) - Singleton<TranscendManager>.instance.getTranscendPassiveSkillCastChance(TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage, m_targetTranscendTier - 1);
				skillData2 = Singleton<TranscendManager>.instance.getTranscendPassiveSkillInventoryData(TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage);
				skillDescriptionText.text = Singleton<TranscendManager>.instance.getTranscendPassiveSkillDescriptionFromI18N(TranscendManager.TranscendPassiveSkillType.DecreaseHitDamage, m_targetTranscendTier, skillData2.skillLevel, false, increaseChance4);
				break;
			case CharacterManager.CharacterType.Priest:
				skillNameText.text = string.Format(I18NManager.Get("TRANSCEND_COMPLETE_SKILL_UPGRADED"), Singleton<TranscendManager>.instance.getTranscendPassiveSkillNameFromI18N(TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage));
				increaseChance4 = Singleton<TranscendManager>.instance.getTranscendPassiveSkillCastChance(TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage, m_targetTranscendTier) - Singleton<TranscendManager>.instance.getTranscendPassiveSkillCastChance(TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage, m_targetTranscendTier - 1);
				skillData2 = Singleton<TranscendManager>.instance.getTranscendPassiveSkillInventoryData(TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage);
				skillDescriptionText.text = Singleton<TranscendManager>.instance.getTranscendPassiveSkillDescriptionFromI18N(TranscendManager.TranscendPassiveSkillType.IncreaseAllDamage, m_targetTranscendTier, skillData2.skillLevel, false, increaseChance4);
				break;
			case CharacterManager.CharacterType.Archer:
				skillNameText.text = string.Format(I18NManager.Get("TRANSCEND_COMPLETE_SKILL_UPGRADED"), Singleton<TranscendManager>.instance.getTranscendPassiveSkillNameFromI18N(TranscendManager.TranscendPassiveSkillType.PenetrationArrow));
				increaseChance4 = Singleton<TranscendManager>.instance.getTranscendPassiveSkillCastChance(TranscendManager.TranscendPassiveSkillType.PenetrationArrow, m_targetTranscendTier) - Singleton<TranscendManager>.instance.getTranscendPassiveSkillCastChance(TranscendManager.TranscendPassiveSkillType.PenetrationArrow, m_targetTranscendTier - 1);
				skillData2 = Singleton<TranscendManager>.instance.getTranscendPassiveSkillInventoryData(TranscendManager.TranscendPassiveSkillType.PenetrationArrow);
				skillDescriptionText.text = Singleton<TranscendManager>.instance.getTranscendPassiveSkillDescriptionFromI18N(TranscendManager.TranscendPassiveSkillType.PenetrationArrow, m_targetTranscendTier, skillData2.skillLevel, false, increaseChance4);
				break;
			}
		}
		if (m_targetTranscendTier == 1 || m_targetTranscendTier == 5 || m_targetTranscendTier == 10)
		{
			skinUnlockedObject.SetActive(true);
		}
		else
		{
			skinUnlockedObject.SetActive(false);
		}
		effectObject.SetActive(false);
		createCharacterObject(false);
		while (color.a > 0f)
		{
			color = flashImage.color;
			color.a -= Time.deltaTime * 9f;
			flashImage.color = color;
			yield return null;
		}
		flashImage.color = new Color(1f, 1f, 1f, 0f);
		isCanCloseESC = true;
		for (int i = 0; i < closeButtons.Length; i++)
		{
			closeButtons[i].interactable = true;
		}
	}
}
