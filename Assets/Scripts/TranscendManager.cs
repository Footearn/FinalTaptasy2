using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using UnityEngine;

public class TranscendManager : Singleton<TranscendManager>
{
	public enum TranscendPassiveSkillType
	{
		DecreaseHitDamage,
		IncreaseAllDamage,
		PenetrationArrow,
		Length
	}

	public long ingameTotalTranscendStone;

	public Sprite[] transcendSkillOnIcon;

	public Sprite[] transcendSkillOffIcon;

	private List<TranscendPassiveSkillType> m_currentCastingSkillList = new List<TranscendPassiveSkillType>();

	private Dictionary<TranscendPassiveSkillType, float> m_currentSkillTimer;

	private AutoRecycleEffect m_decreaseHitDamageAutoRecycleEffectObject;

	private AutoRecycleEffect m_increaseAllDamageAutoRecycleEffectObject;

	private void Awake()
	{
		m_currentSkillTimer = new Dictionary<TranscendPassiveSkillType, float>();
		for (int i = 0; i < 3; i++)
		{
			m_currentSkillTimer.Add((TranscendPassiveSkillType)i, 0f);
		}
	}

	public void startGame()
	{
		refreshManager();
	}

	public void endGame()
	{
		refreshManager();
	}

	private void refreshManager()
	{
		ingameTotalTranscendStone = 0L;
		UIWindowIngame.instance.closeTranscendSkillCooltimeSlot(TranscendPassiveSkillType.DecreaseHitDamage);
		UIWindowIngame.instance.closeTranscendSkillCooltimeSlot(TranscendPassiveSkillType.IncreaseAllDamage);
		m_currentCastingSkillList.Clear();
		resetAllSkillTimer();
	}

	public void spawnTranscendStone(Vector3 spawnPosition, long value)
	{
		Singleton<DropItemManager>.instance.spawnDropItem(DropItemManager.DropItemType.TranscendStone, spawnPosition, value);
	}

	public void increaseTranscendStone(long value)
	{
		Singleton<DataManager>.instance.currentGameData.transcendStone += value;
	}

	public void decreaseTranscendStone(long value)
	{
		Singleton<DataManager>.instance.currentGameData.transcendStone -= value;
	}

	public void transcendTierUp(CharacterManager.CharacterType targetCharacterType)
	{
		long num = Singleton<DataManager>.instance.currentGameData.currentTranscendTier[targetCharacterType];
		if (num < getTranscendMaxTier())
		{
			long transcendCost = getTranscendCost(num + 1);
			if (Singleton<DataManager>.instance.currentGameData.transcendStone >= transcendCost)
			{
				Dictionary<CharacterManager.CharacterType, long> currentTranscendTier;
				Dictionary<CharacterManager.CharacterType, long> dictionary = (currentTranscendTier = Singleton<DataManager>.instance.currentGameData.currentTranscendTier);
				CharacterManager.CharacterType key;
				CharacterManager.CharacterType key2 = (key = targetCharacterType);
				long num2 = currentTranscendTier[key];
				dictionary[key2] = num2 + 1;
				decreaseTranscendStone(transcendCost);
				trancendTierUpEvent(targetCharacterType);
				Singleton<DataManager>.instance.saveData();
			}
			else
			{
				UIWindowDialog.openDescription("NOT_ENOUGH_TRANSCEND_STONE_FOR_TIER_UP", UIWindowDialog.DialogState.CloseUI, null, string.Empty);
			}
		}
	}

	private void trancendTierUpEvent(CharacterManager.CharacterType targetCharacterType)
	{
		TranscendPassiveSkillType characterTranscendPassiveSkillType = getCharacterTranscendPassiveSkillType(targetCharacterType);
		TranscendPassiveSkillInventoryData transcendPassiveSkillInventoryData = getTranscendPassiveSkillInventoryData(characterTranscendPassiveSkillType);
		if (!transcendPassiveSkillInventoryData.isUnlocked)
		{
			transcendPassiveSkillInventoryData.isUnlocked = true;
			transcendPassiveSkillInventoryData.skillLevel = 1L;
		}
		if (UIWindowSkill.instance.isOpen)
		{
			UIWindowSkill.instance.skillScroll.refreshAll();
		}
		switch (Singleton<DataManager>.instance.currentGameData.currentTranscendTier[targetCharacterType])
		{
		case 1L:
			switch (targetCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.WarriorSkinType.Arthur);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.WarriorSkinType.Arthur);
				break;
			case CharacterManager.CharacterType.Priest:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.PriestSkinType.Michael);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.PriestSkinType.Michael);
				break;
			case CharacterManager.CharacterType.Archer:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.ArcherSkinType.Patricia);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.ArcherSkinType.Patricia);
				break;
			}
			UIWindowCharacterSkin.instance.skinScroll.refreshAll();
			break;
		case 5L:
			switch (targetCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.WarriorSkinType.BlackKnight);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.WarriorSkinType.BlackKnight);
				break;
			case CharacterManager.CharacterType.Priest:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.PriestSkinType.Criselda);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.PriestSkinType.Criselda);
				break;
			case CharacterManager.CharacterType.Archer:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.ArcherSkinType.Claude);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.ArcherSkinType.Claude);
				break;
			}
			UIWindowCharacterSkin.instance.skinScroll.refreshAll();
			break;
		case 10L:
			switch (targetCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.WarriorSkinType.Phoenix);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.WarriorSkinType.Phoenix);
				break;
			case CharacterManager.CharacterType.Priest:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.PriestSkinType.Elisha);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.PriestSkinType.Elisha);
				break;
			case CharacterManager.CharacterType.Archer:
				Singleton<CharacterSkinManager>.instance.obtainCharacterSkin(CharacterSkinManager.ArcherSkinType.Kyle);
				Singleton<CharacterManager>.instance.equipCharacter(CharacterSkinManager.ArcherSkinType.Kyle);
				break;
			}
			UIWindowCharacterSkin.instance.skinScroll.refreshAll();
			break;
		}
	}

	public long getTranscendCost(long transcendTier)
	{
		return transcendTier * 100;
	}

	public long getTranscendMaxTier()
	{
		return 10L;
	}

	public long getTranscendSkillUpgradePrice(TranscendPassiveSkillType skillType, long transcendSkillLevel)
	{
		long result = 0L;
		switch (skillType)
		{
		case TranscendPassiveSkillType.DecreaseHitDamage:
			result = 10 + (transcendSkillLevel - 1) * 20;
			break;
		case TranscendPassiveSkillType.IncreaseAllDamage:
			result = 10 + (transcendSkillLevel - 1) * 20;
			break;
		case TranscendPassiveSkillType.PenetrationArrow:
			result = 10 + (transcendSkillLevel - 1) * 20;
			break;
		}
		return result;
	}

	public Sprite getTranscendPassiveSkillIcon(TranscendPassiveSkillType skillType, bool isOnSprite)
	{
		if (isOnSprite)
		{
			return transcendSkillOnIcon[(int)skillType];
		}
		return transcendSkillOffIcon[(int)skillType];
	}

	public string getTranscendPassiveSkillNameFromI18N(TranscendPassiveSkillType skillType)
	{
		return I18NManager.Get("TRANSCEND_SKILL_NAME_" + (int)(skillType + 1));
	}

	public string getTranscendPassiveSkillDescriptionFromI18N(TranscendPassiveSkillType skillType, long transcendTier, long skillLevel, bool replaceColorCode, double increaseChance = 0.0)
	{
		string text = string.Empty;
		switch (skillType)
		{
		case TranscendPassiveSkillType.DecreaseHitDamage:
			text = string.Format(I18NManager.Get("TRANSCEND_SKILL_DESCRIPTION_" + (int)(skillType + 1)), getTranscendPassiveSkillCastChance(skillType, transcendTier) + ((!(increaseChance > 0.0)) ? string.Empty : ("(+" + string.Format("{0:0.##}", increaseChance) + ")")), string.Format("{0:0.##}", getTranscendPassiveSkillDuration(skillType, skillLevel)), string.Format("{0:0.##}", getTranscendPassiveSkillValue(skillType, skillLevel, false)));
			break;
		case TranscendPassiveSkillType.IncreaseAllDamage:
			text = string.Format(I18NManager.Get("TRANSCEND_SKILL_DESCRIPTION_" + (int)(skillType + 1)), getTranscendPassiveSkillCastChance(skillType, transcendTier) + ((!(increaseChance > 0.0)) ? string.Empty : ("(+" + string.Format("{0:0.##}", increaseChance) + ")")), string.Format("{0:0.##}", getTranscendPassiveSkillDuration(skillType, skillLevel)), (getTranscendPassiveSkillValue(skillType, skillLevel, false) + 100f) / 100f);
			break;
		case TranscendPassiveSkillType.PenetrationArrow:
		{
			double num = (double)(getTranscendPassiveSkillValue(skillType, skillLevel, false) / 100f) * Singleton<StatManager>.instance.skillExtraPercentDamage;
			string arg = getTranscendPassiveSkillValue(skillType, skillLevel, false) + ((!(num > 0.0)) ? string.Empty : ("<color=#FFFC2E>(+" + num.ToString("0.#") + ")</color>"));
			text = string.Format(I18NManager.Get("TRANSCEND_SKILL_DESCRIPTION_" + (int)(skillType + 1)), getTranscendPassiveSkillCastChance(skillType, transcendTier) + ((!(increaseChance > 0.0)) ? string.Empty : ("(+" + string.Format("{0:0.##}", increaseChance) + ")")), arg);
			break;
		}
		}
		if (replaceColorCode)
		{
			text = text.Replace("<color=#FAD725>", string.Empty).Replace("</color>", string.Empty).Replace("<color=white>", string.Empty)
				.Replace("<color=#FFFC2E>", string.Empty);
		}
		return text;
	}

	public TranscendPassiveSkillInventoryData getTranscendPassiveSkillInventoryData(TranscendPassiveSkillType skillType)
	{
		return Singleton<DataManager>.instance.currentGameData.transcendPassiveSkillInventoryData[skillType];
	}

	public TranscendPassiveSkillType getCharacterTranscendPassiveSkillType(CharacterManager.CharacterType characterType, long transcendTier = 1)
	{
		TranscendPassiveSkillType result = TranscendPassiveSkillType.Length;
		switch (characterType)
		{
		case CharacterManager.CharacterType.Warrior:
			result = TranscendPassiveSkillType.DecreaseHitDamage;
			break;
		case CharacterManager.CharacterType.Priest:
			result = TranscendPassiveSkillType.IncreaseAllDamage;
			break;
		case CharacterManager.CharacterType.Archer:
			result = TranscendPassiveSkillType.PenetrationArrow;
			break;
		}
		return result;
	}

	public double getTranscendPassiveSkillCastChance(TranscendPassiveSkillType skillType, long transcendTier)
	{
		transcendTier = Math.Max(transcendTier, 1L);
		double result = 0.0;
		switch (skillType)
		{
		case TranscendPassiveSkillType.DecreaseHitDamage:
			result = 0.3 + 0.3 * (double)(transcendTier - 1);
			break;
		case TranscendPassiveSkillType.IncreaseAllDamage:
			result = 0.3 + 0.3 * (double)(transcendTier - 1);
			break;
		case TranscendPassiveSkillType.PenetrationArrow:
			result = 0.3 + 0.3 * (double)(transcendTier - 1);
			break;
		}
		return result;
	}

	public float getTranscendPassiveSkillDuration(TranscendPassiveSkillType skillType, long skillLevel)
	{
		float num = 0f;
		switch (skillType)
		{
		case TranscendPassiveSkillType.DecreaseHitDamage:
			return 5f;
		case TranscendPassiveSkillType.IncreaseAllDamage:
			return 5f;
		default:
			return 0f;
		}
	}

	public float getTranscendPassiveSkillValue(TranscendPassiveSkillType skillType, long skillLevel, bool isCalculatedSkillValue = true)
	{
		skillLevel = Math.Max(skillLevel, 1L);
		float num = 0f;
		switch (skillType)
		{
		case TranscendPassiveSkillType.DecreaseHitDamage:
			num = (float)((1.0 - 1.0 / (((double)skillLevel + 9.0) * 0.3 + 1.0)) * 100.0);
			break;
		case TranscendPassiveSkillType.IncreaseAllDamage:
			num = (float)((1.0 + (double)(skillLevel - 1) * 0.25) * 100.0);
			break;
		case TranscendPassiveSkillType.PenetrationArrow:
			num = (float)((3.0 + (double)(skillLevel - 1) * 0.75) * 100.0);
			if (isCalculatedSkillValue)
			{
				num += (float)((double)(num / 100f) * Singleton<StatManager>.instance.skillExtraPercentDamage);
			}
			break;
		}
		return num;
	}

	public int getTranscendCharacterUnlockTier(CharacterSkinManager.WarriorSkinType skinType)
	{
		int result = 0;
		switch (skinType)
		{
		case CharacterSkinManager.WarriorSkinType.Arthur:
			result = 1;
			break;
		case CharacterSkinManager.WarriorSkinType.BlackKnight:
			result = 5;
			break;
		case CharacterSkinManager.WarriorSkinType.Phoenix:
			result = 10;
			break;
		}
		return result;
	}

	public int getTranscendCharacterUnlockTier(CharacterSkinManager.PriestSkinType skinType)
	{
		int result = 0;
		switch (skinType)
		{
		case CharacterSkinManager.PriestSkinType.Michael:
			result = 1;
			break;
		case CharacterSkinManager.PriestSkinType.Criselda:
			result = 5;
			break;
		case CharacterSkinManager.PriestSkinType.Elisha:
			result = 10;
			break;
		}
		return result;
	}

	public int getTranscendCharacterUnlockTier(CharacterSkinManager.ArcherSkinType skinType)
	{
		int result = 0;
		switch (skinType)
		{
		case CharacterSkinManager.ArcherSkinType.Patricia:
			result = 1;
			break;
		case CharacterSkinManager.ArcherSkinType.Claude:
			result = 5;
			break;
		case CharacterSkinManager.ArcherSkinType.Kyle:
			result = 10;
			break;
		}
		return result;
	}

	private void resetSkillTimer(TranscendPassiveSkillType skillType)
	{
		m_currentSkillTimer[skillType] = 0f;
	}

	private void resetAllSkillTimer()
	{
		for (int i = 0; i < 3; i++)
		{
			m_currentSkillTimer[(TranscendPassiveSkillType)i] = 0f;
		}
	}

	public void touchToCastTranscendSkill()
	{
		for (int i = 0; i < 3; i++)
		{
			CharacterManager.CharacterType characterType = (CharacterManager.CharacterType)i;
			long num = Singleton<DataManager>.instance.currentGameData.currentTranscendTier[characterType];
			if (num <= 0)
			{
				continue;
			}
			TranscendPassiveSkillType characterTranscendPassiveSkillType = getCharacterTranscendPassiveSkillType(characterType);
			if (getTranscendPassiveSkillInventoryData(characterTranscendPassiveSkillType).isUnlocked)
			{
				MersenneTwister mersenneTwister = new MersenneTwister();
				double num2 = mersenneTwister.Next(0, 100000);
				num2 /= 1000.0;
				if (num2 <= getTranscendPassiveSkillCastChance(characterTranscendPassiveSkillType, num))
				{
					castTranscendPassiveSkill(characterTranscendPassiveSkillType);
				}
			}
		}
	}

	public void castTranscendPassiveSkill(TranscendPassiveSkillType skillType)
	{
		IEnumerator passiveSkillCoroutine = getPassiveSkillCoroutine(skillType);
		if (passiveSkillCoroutine == null)
		{
			return;
		}
		playTranscendSkillEffect(skillType);
		resetSkillTimer(skillType);
		if (!m_currentCastingSkillList.Contains(skillType))
		{
			m_currentCastingSkillList.Add(skillType);
			StartCoroutine(passiveSkillCoroutine);
			return;
		}
		switch (skillType)
		{
		case TranscendPassiveSkillType.DecreaseHitDamage:
			if (m_decreaseHitDamageAutoRecycleEffectObject != null)
			{
				m_decreaseHitDamageAutoRecycleEffectObject.resetTimer();
			}
			break;
		case TranscendPassiveSkillType.IncreaseAllDamage:
			if (m_increaseAllDamageAutoRecycleEffectObject != null)
			{
				m_increaseAllDamageAutoRecycleEffectObject.resetTimer();
			}
			break;
		}
	}

	private void playTranscendSkillEffect(TranscendPassiveSkillType skillType)
	{
		TranscendPassiveSkillInventoryData transcendPassiveSkillInventoryData = getTranscendPassiveSkillInventoryData(skillType);
		GameObject gameObject = null;
		switch (skillType)
		{
		case TranscendPassiveSkillType.DecreaseHitDamage:
			Singleton<AudioManager>.instance.playEffectSound("transcend_cast", AudioManager.EffectType.TranscendSkill);
			ObjectPool.Clear("@TranscendDecreaseHitDamageEffect");
			gameObject = ObjectPool.Spawn("@TranscendDecreaseHitDamageEffect", new Vector3(0f, 0.5f, 0f), new Vector3(-90f, 0f, 0f));
			gameObject.GetComponent<FollowObject>().followTarget = Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform;
			m_decreaseHitDamageAutoRecycleEffectObject = gameObject.GetComponent<AutoRecycleEffect>();
			m_decreaseHitDamageAutoRecycleEffectObject.duration = getTranscendPassiveSkillDuration(TranscendPassiveSkillType.DecreaseHitDamage, transcendPassiveSkillInventoryData.skillLevel);
			break;
		case TranscendPassiveSkillType.IncreaseAllDamage:
			Singleton<AudioManager>.instance.playEffectSound("transcend_cast", AudioManager.EffectType.TranscendSkill);
			ObjectPool.Clear("@TranscendIncreaseAllDamageEffect");
			gameObject = ObjectPool.Spawn("@TranscendIncreaseAllDamageEffect", Vector2.zero, new Vector3(-90f, 0f, 0f));
			gameObject.GetComponent<FollowObject>().followTarget = Singleton<CharacterManager>.instance.priestCharacter.cachedTransform;
			m_increaseAllDamageAutoRecycleEffectObject = gameObject.GetComponent<AutoRecycleEffect>();
			m_increaseAllDamageAutoRecycleEffectObject.duration = getTranscendPassiveSkillDuration(TranscendPassiveSkillType.IncreaseAllDamage, transcendPassiveSkillInventoryData.skillLevel);
			break;
		case TranscendPassiveSkillType.PenetrationArrow:
			break;
		}
	}

	private IEnumerator getPassiveSkillCoroutine(TranscendPassiveSkillType skillType)
	{
		IEnumerator result = null;
		switch (skillType)
		{
		case TranscendPassiveSkillType.DecreaseHitDamage:
			result = decreaseHitDamageUpdate();
			break;
		case TranscendPassiveSkillType.IncreaseAllDamage:
			result = increaseAllDamageUpdate();
			break;
		case TranscendPassiveSkillType.PenetrationArrow:
			result = penetrationArrowUpdate();
			break;
		}
		return result;
	}

	private IEnumerator decreaseHitDamageUpdate()
	{
		TranscendPassiveSkillInventoryData skillData = getTranscendPassiveSkillInventoryData(TranscendPassiveSkillType.DecreaseHitDamage);
		UIWindowIngame.instance.setOpenTranscendSkillInformation(TranscendPassiveSkillType.DecreaseHitDamage, getTranscendPassiveSkillDuration(TranscendPassiveSkillType.DecreaseHitDamage, skillData.skillLevel));
		Singleton<StatManager>.instance.transcendDecreaseHitDamage = getTranscendPassiveSkillValue(TranscendPassiveSkillType.DecreaseHitDamage, skillData.skillLevel);
		while (m_currentSkillTimer[TranscendPassiveSkillType.DecreaseHitDamage] < getTranscendPassiveSkillDuration(TranscendPassiveSkillType.DecreaseHitDamage, skillData.skillLevel) && GameManager.currentGameState == GameManager.GameState.Playing)
		{
			if (!GameManager.isPause && !UIWindowIngame.instance.bossWarningState)
			{
				Dictionary<TranscendPassiveSkillType, float> currentSkillTimer;
				Dictionary<TranscendPassiveSkillType, float> dictionary = (currentSkillTimer = m_currentSkillTimer);
				TranscendPassiveSkillType key;
				TranscendPassiveSkillType key2 = (key = TranscendPassiveSkillType.DecreaseHitDamage);
				float num = currentSkillTimer[key];
				dictionary[key2] = num + Time.deltaTime * GameManager.timeScale;
				UIWindowIngame.instance.changeTranscendSkillCooltimeData(TranscendPassiveSkillType.DecreaseHitDamage, getTranscendPassiveSkillDuration(TranscendPassiveSkillType.DecreaseHitDamage, skillData.skillLevel), m_currentSkillTimer[TranscendPassiveSkillType.DecreaseHitDamage]);
			}
			yield return null;
		}
		UIWindowIngame.instance.closeTranscendSkillCooltimeSlot(TranscendPassiveSkillType.DecreaseHitDamage);
		Singleton<StatManager>.instance.transcendDecreaseHitDamage = 0.0;
		m_currentCastingSkillList.Remove(TranscendPassiveSkillType.DecreaseHitDamage);
	}

	private IEnumerator increaseAllDamageUpdate()
	{
		TranscendPassiveSkillInventoryData skillData = getTranscendPassiveSkillInventoryData(TranscendPassiveSkillType.IncreaseAllDamage);
		UIWindowIngame.instance.setOpenTranscendSkillInformation(TranscendPassiveSkillType.IncreaseAllDamage, getTranscendPassiveSkillDuration(TranscendPassiveSkillType.IncreaseAllDamage, skillData.skillLevel));
		Singleton<StatManager>.instance.transcendIncreaseAllDamage = getTranscendPassiveSkillValue(TranscendPassiveSkillType.IncreaseAllDamage, skillData.skillLevel);
		while (m_currentSkillTimer[TranscendPassiveSkillType.IncreaseAllDamage] < getTranscendPassiveSkillDuration(TranscendPassiveSkillType.IncreaseAllDamage, skillData.skillLevel) && GameManager.currentGameState == GameManager.GameState.Playing)
		{
			if (!GameManager.isPause && !UIWindowIngame.instance.bossWarningState)
			{
				Dictionary<TranscendPassiveSkillType, float> currentSkillTimer;
				Dictionary<TranscendPassiveSkillType, float> dictionary = (currentSkillTimer = m_currentSkillTimer);
				TranscendPassiveSkillType key;
				TranscendPassiveSkillType key2 = (key = TranscendPassiveSkillType.IncreaseAllDamage);
				float num = currentSkillTimer[key];
				dictionary[key2] = num + Time.deltaTime * GameManager.timeScale;
				UIWindowIngame.instance.changeTranscendSkillCooltimeData(TranscendPassiveSkillType.IncreaseAllDamage, getTranscendPassiveSkillDuration(TranscendPassiveSkillType.DecreaseHitDamage, skillData.skillLevel), m_currentSkillTimer[TranscendPassiveSkillType.IncreaseAllDamage]);
			}
			yield return null;
		}
		UIWindowIngame.instance.closeTranscendSkillCooltimeSlot(TranscendPassiveSkillType.IncreaseAllDamage);
		Singleton<StatManager>.instance.transcendIncreaseAllDamage = 0.0;
		m_currentCastingSkillList.Remove(TranscendPassiveSkillType.IncreaseAllDamage);
	}

	private IEnumerator penetrationArrowUpdate()
	{
		m_currentCastingSkillList.Remove(TranscendPassiveSkillType.PenetrationArrow);
		Vector2 spawnPosition = Singleton<CharacterManager>.instance.archerCharacter.cachedTransform.position + new Vector3(0f, 0.62f, 0f);
		EnemyObject targetEnemy = Singleton<EnemyManager>.instance.getBestNearestEnemy(Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform.position, 2.4f);
		if (targetEnemy != null)
		{
			Singleton<AudioManager>.instance.playEffectSound("transcend_cast", AudioManager.EffectType.TranscendSkill);
			PenetrationObject arrow = ObjectPool.Spawn("@PenetrationArrow", spawnPosition).GetComponent<PenetrationObject>();
			Vector3 targetPosition = targetEnemy.cachedTransform.position;
			if (Mathf.Abs(targetEnemy.cachedTransform.position.y - spawnPosition.y) <= 0.8f)
			{
				targetPosition.y = spawnPosition.y;
			}
			arrow.initPenetration(targetPosition);
			ObjectPool.Spawn("@PenetrationEffect", arrow.cachedTransform.position);
		}
		yield return null;
	}
}
