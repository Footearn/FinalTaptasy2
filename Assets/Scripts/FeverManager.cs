using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FeverManager : Singleton<FeverManager>
{
	public Image feverProgressImage;

	public Image skillFlash;

	public float currentFeverValue;

	public float maxFeverValue;

	public bool isFever;

	public bool isStart;

	public int maxManaCount;

	public float feverTime = 5f;

	public int currentMana;

	public Image manaGaugeWhiteBlock;

	private bool m_isFeverButtonOn;

	public Image manaGaugeGlowEffectImage;

	private double m_previousDPS;

	private double m_currentDPS;

	private int m_tapCount;

	private int m_totalTapCount;

	private float m_totalTapCountForEffect;

	private float m_totalTpsTimer;

	private float m_tpsTimer;

	public float tps;

	public ParticleSystem[] tapEffects;

	private Transform[] m_tapEffectTransforms;

	private bool m_isUpdatingManaGauge;

	private void Awake()
	{
		m_tapEffectTransforms = new Transform[tapEffects.Length];
		for (int i = 0; i < tapEffects.Length; i++)
		{
			m_tapEffectTransforms[i] = tapEffects[i].transform;
		}
	}

	public void resetFever()
	{
		if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			isStart = true;
		}
		else
		{
			isStart = false;
		}
		isFever = false;
		m_isFeverButtonOn = false;
		currentFeverValue = 0f;
		feverProgressImage.fillAmount = 0f;
		StopCoroutine("startMaxManaGaugeUpdate");
	}

	public void startGame()
	{
		manaGaugeGlowEffectImage.color = new Color(1f, 1f, 1f, 0.3f);
		manaGaugeWhiteBlock.color = new Color(1f, 1f, 1f, 0f);
		maxManaCount = 5 + Singleton<StatManager>.instance.maxExtraMana;
		resetFever();
		m_totalTapCountForEffect = 0f;
		m_previousDPS = 0.0;
		m_totalTpsTimer = 0f;
		m_currentDPS = 0.0;
		m_tapCount = 0;
		m_totalTapCount = 0;
		m_tpsTimer = 0f;
		tps = 0f;
	}

	public void increaseFever(float value)
	{
		if (GameManager.isPause)
		{
			return;
		}
		if (currentMana < maxManaCount)
		{
			StartCoroutine(flashEffect());
			currentFeverValue += value;
			currentFeverValue = Mathf.Clamp(currentFeverValue, 0f, maxFeverValue);
			if (m_isFeverButtonOn)
			{
				m_isFeverButtonOn = false;
				isFever = true;
			}
			if (currentFeverValue >= maxFeverValue)
			{
				m_isFeverButtonOn = true;
			}
			if (currentFeverValue >= maxFeverValue)
			{
				currentFeverValue = 0f;
				currentMana++;
				Singleton<SkillManager>.instance.CheckMana();
				Singleton<SkillManager>.instance.CheckSkillAll();
			}
			float fillAmount = currentFeverValue / maxFeverValue;
			feverProgressImage.fillAmount = fillAmount;
		}
		else
		{
			feverProgressImage.fillAmount = 1f;
			if (!m_isUpdatingManaGauge)
			{
				StopCoroutine(glowEffect());
				manaGaugeGlowEffectImage.color = new Color(1f, 1f, 1f, 1f);
				StopCoroutine("startMaxManaGaugeUpdate");
				StartCoroutine("startMaxManaGaugeUpdate");
			}
		}
	}

	private IEnumerator startMaxManaGaugeUpdate()
	{
		m_isUpdatingManaGauge = true;
		bool switchAlpha = true;
		manaGaugeWhiteBlock.color = new Color(1f, 1f, 1f, 0f);
		while (true)
		{
			Color color = manaGaugeWhiteBlock.color;
			if (switchAlpha)
			{
				color.a += Time.deltaTime * GameManager.timeScale * 2f;
				if (color.a >= 0.6f)
				{
					switchAlpha = false;
					color.a = 0.6f;
				}
			}
			else
			{
				color.a -= Time.deltaTime * GameManager.timeScale * 2f;
				if (color.a < 0f)
				{
					switchAlpha = true;
					color.a = 0f;
				}
			}
			manaGaugeWhiteBlock.color = color;
			yield return null;
		}
	}

	public void UseMana(int mana)
	{
		m_isUpdatingManaGauge = false;
		manaGaugeWhiteBlock.color = new Color(1f, 1f, 1f, 0f);
		StopCoroutine("startMaxManaGaugeUpdate");
		currentMana -= mana;
		if (Singleton<StatManager>.instance.weaponSkinReinforcementMana > 0.0)
		{
			double num = Singleton<StatManager>.instance.weaponSkinReinforcementMana * (double)mana;
			Singleton<StatManager>.instance.weaponSkinArmorFromReinforcementMana = Util.Clamp(Singleton<StatManager>.instance.weaponSkinArmorFromReinforcementMana + num, 0.0, 95.0);
			Singleton<StatManager>.instance.weaponSkinTapHealthFromReinforcementMana += num;
			Singleton<StatManager>.instance.weaponSkinDamageFromReinforcementMana += num;
			UIWindowIngame.instance.reinforcementManaText.text = string.Format("{0:0.##}", Singleton<StatManager>.instance.weaponSkinTapHealthFromReinforcementMana) + "%";
		}
		Singleton<SkillManager>.instance.CheckMana();
	}

	private void Update()
	{
		if (GameManager.currentGameState != 0 || GameManager.isPause || GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode || GameManager.currentDungeonType == GameManager.SpecialDungeonType.BossRaid)
		{
			return;
		}
		CharacterWarrior warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		if (warriorCharacter != null)
		{
			for (int i = 0; i < Input.touches.Length; i++)
			{
				if (Input.touches[i].phase == TouchPhase.Began)
				{
					touchEvent();
				}
			}
		}
		if (!GameManager.isPause && Singleton<CharacterManager>.instance.warriorCharacter != null && !Singleton<CharacterManager>.instance.warriorCharacter.isDead && Singleton<CharacterManager>.instance.warriorCharacter.isCanAttackBoss == Singleton<CharacterManager>.instance.warriorCharacter.isUsedBossStageAction)
		{
			m_tpsTimer += Time.deltaTime * GameManager.timeScale;
			m_totalTpsTimer += Time.deltaTime * GameManager.timeScale;
			if (m_tpsTimer > 1f)
			{
				tps = m_tapCount;
				m_previousDPS = m_currentDPS;
				double num = Util.Clamp(Singleton<FeverManager>.instance.getCurrentAttackEffectIndex() - 2, 0.0, 3.0) * 10.0;
				double num2 = ((!(Singleton<CharacterManager>.instance != null) || !(Singleton<CharacterManager>.instance.warriorCharacter != null)) ? 0.0 : (Singleton<CharacterManager>.instance.warriorCharacter.curDamage * (double)Mathf.Min(m_tapCount, 8)));
				num2 += num2 / 100.0 * num;
				double num3 = ((!(Singleton<CharacterManager>.instance != null) || !(Singleton<CharacterManager>.instance.priestCharacter != null)) ? 0.0 : (Singleton<CharacterManager>.instance.priestCharacter.curDamage * (double)Mathf.Min(m_tapCount, 8)));
				double num4 = ((!(Singleton<CharacterManager>.instance != null) || !(Singleton<CharacterManager>.instance.archerCharacter != null)) ? 0.0 : (Singleton<CharacterManager>.instance.archerCharacter.curDamage * (double)Mathf.Min(m_tapCount, 8)));
				m_currentDPS = Singleton<ColleagueManager>.instance.getCurrentDPS() + num2 + num3 + num4;
				m_tapCount = 0;
				m_tpsTimer = 0f;
			}
			else
			{
				m_previousDPS = Math.Max(Util.Lerp(m_previousDPS, m_currentDPS, Time.deltaTime * GameManager.timeScale * 15f), 0.0);
				UIWindowIngame.instance.currentDPSText.text = GameManager.changeUnit(m_previousDPS) + " <size=23>DPS</size>";
			}
			m_totalTapCountForEffect = Mathf.Lerp(m_totalTapCountForEffect, 0f, Time.deltaTime * GameManager.timeScale * 0.8f);
			m_totalTpsTimer = Mathf.Lerp(m_totalTpsTimer, 0f, Time.deltaTime * GameManager.timeScale * 0.8f);
		}
	}

	public int getCurrentAttackEffectIndex()
	{
		int num = 0;
		num = (int)(m_totalTapCountForEffect / m_totalTpsTimer / 5f);
		return Mathf.Clamp(num, 0, 4);
	}

	private IEnumerator glowEffect()
	{
		float alpha2 = 1f;
		manaGaugeGlowEffectImage.color = new Color(1f, 1f, 1f, 1f);
		while (true)
		{
			Color color = manaGaugeGlowEffectImage.color;
			color.a = alpha2;
			manaGaugeGlowEffectImage.color = color;
			if (alpha2 <= 0.3f)
			{
				break;
			}
			alpha2 -= Time.deltaTime * GameManager.timeScale * 4f;
			yield return null;
		}
		alpha2 = 0.3f;
		manaGaugeGlowEffectImage.color = new Color(1f, 1f, 1f, 0.3f);
	}

	public void touchEvent()
	{
		if ((TutorialManager.isTutorial && Singleton<TutorialManager>.instance.isInvincible) || Singleton<CharacterManager>.instance.warriorCharacter.getState() == PublicDataManager.State.Wait || Singleton<GameManager>.instance.isGameOver || GameManager.isPause || Singleton<CharacterManager>.instance.warriorCharacter.isDead)
		{
			return;
		}
		float num = 2f;
		num += num / 100f * Singleton<StatManager>.instance.manaGatherExtraValue;
		Singleton<FeverManager>.instance.increaseFever(num);
		int num2 = UnityEngine.Random.Range(0, tapEffects.Length);
		if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			StartCoroutine(glowEffect());
			ParticleSystem particleSystem = tapEffects[num2];
			Transform transform = m_tapEffectTransforms[num2];
			particleSystem.Stop();
			particleSystem.Play();
			transform.position = Util.getCurrentScreenToWorldPosition() + new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
		}
		Singleton<SkillManager>.instance.touchToCastPassiveSkill();
		Singleton<TranscendManager>.instance.touchToCastTranscendSkill();
		Singleton<WeaponSkinManager>.instance.touchToCastAutoSkill();
		if (!isStart)
		{
			return;
		}
		if (Singleton<StatManager>.instance.healValueEveryTap > 0.0)
		{
			double num3 = Singleton<StatManager>.instance.healValueEveryTap;
			if (Singleton<StatManager>.instance.chanceForMultipliedTapHealFromPremiumTreasure > 0.0)
			{
				double num4 = UnityEngine.Random.Range(0, 10000);
				num4 /= 100.0;
				if (num4 < Singleton<StatManager>.instance.chanceForMultipliedTapHealFromPremiumTreasure)
				{
					num3 *= 10.0;
				}
			}
			Singleton<CharacterManager>.instance.warriorCharacter.increasesHealth(num3, false);
		}
		Singleton<CharacterManager>.instance.feverCharacters(10f);
		Singleton<CharacterManager>.instance.warriorCharacter.clickAttack();
		m_tapCount++;
		m_totalTapCountForEffect += 1f;
		m_totalTapCount++;
	}

	private IEnumerator flashEffect()
	{
		float alphaPercentage = 0f;
		float alphaIteration = 3f;
		while (!(alphaPercentage > 0.3f))
		{
			alphaPercentage += Time.deltaTime * alphaIteration;
			skillFlash.color = new Color(1f, 1f, 1f, alphaPercentage);
			yield return null;
		}
		skillFlash.color = new Color(1f, 1f, 1f, 0f);
	}
}
