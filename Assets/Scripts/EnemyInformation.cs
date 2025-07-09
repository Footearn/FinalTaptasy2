using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInformation : ObjectBase
{
	public static List<EnemyObject> enemyInformationList = new List<EnemyObject>();

	public bool isBossInformation;

	[HideInInspector]
	public EnemyObject targetEnemy;

	public CanvasGroup cachedCanvasGroup;

	public Image enemyIconImage;

	public Image hpProgressImage;

	public Image hpProgressLerpImage;

	public Image hpProgressLerpImage2;

	public Text levelText;

	public Text nameText;

	public Text hpText;

	public bool isOpen;

	private float m_disableTimer;

	private MonsterObject monster;

	private BossObject boss;

	private SpecialObject special;

	private Vector2 offset = new Vector2(0f, 1.5f);

	private float initPosY;

	private float m_totalDecreasedProgressTimer;

	private void Awake()
	{
		if (cachedCanvasGroup == null)
		{
			cachedCanvasGroup = GetComponent<CanvasGroup>();
		}
	}

	public static void resetEnemyList()
	{
		enemyInformationList.Clear();
	}

	public void ResetEnemy()
	{
		targetEnemy = null;
		monster = null;
		boss = null;
		special = null;
		if (cachedCanvasGroup == null)
		{
			cachedCanvasGroup = GetComponent<CanvasGroup>();
		}
		cachedCanvasGroup.alpha = 0f;
	}

	private void OnEnable()
	{
		if (!isBossInformation)
		{
			base.cachedRectTransform.anchoredPosition = new Vector2(1500f, 1500f);
		}
	}

	public bool setProperties(EnemyObject enemy)
	{
		bool result = false;
		if (enemy == targetEnemy)
		{
			cachedCanvasGroup.alpha = 1f;
			m_disableTimer = 0f;
		}
		if (!enemyInformationList.Contains(enemy))
		{
			if (!isOpen)
			{
				cachedCanvasGroup.alpha = 1f;
				StopCoroutine("lerpProgressUpdate");
				StopCoroutine("followTargetEnemyUpdate");
				m_disableTimer = 0f;
				targetEnemy = enemy;
				if (!isBossInformation)
				{
					base.cachedTransform.position = new Vector2(targetEnemy.cachedTransform.position.x, targetEnemy.cachedTransform.position.y - 0.2f);
				}
				if (enemy is MonsterObject)
				{
					monster = enemy as MonsterObject;
					if (monster.isMiniboss)
					{
						offset.y = 2f;
					}
					else
					{
						offset.y = 1f;
					}
					nameText.text = ((!monster.isEliteMonster) ? string.Empty : (I18NManager.Get("ELITE_MONSTER") + " ")) + I18NManager.Get(monster.currentMonsterType.ToString().ToUpper() + "_NAME");
				}
				else if (enemy is SpecialObject)
				{
					special = enemy as SpecialObject;
					nameText.text = I18NManager.Get(special.currentSpecialType.ToString().ToUpper() + "_NAME");
				}
				else
				{
					offset.y = 3f;
					boss = enemy as BossObject;
					nameText.text = I18NManager.Get(boss.currentBossType.ToString().ToUpper() + "_NAME");
				}
				levelText.text = "Lv." + targetEnemy.currentLevel;
				enemyInformationList.Add(targetEnemy);
				base.cachedAnimation.Stop();
				base.cachedAnimation.Play("OpenZombieInformation");
				isOpen = true;
				result = true;
				StartCoroutine("followTargetEnemyUpdate");
				if (isBossInformation)
				{
					m_totalDecreasedProgressTimer = 0f;
					float fillAmount = (float)(targetEnemy.curHealth / targetEnemy.maxHealth);
					hpProgressLerpImage.fillAmount = fillAmount;
					hpProgressLerpImage2.fillAmount = fillAmount;
					StartCoroutine("lerpProgressUpdate");
				}
			}
		}
		else
		{
			result = false;
		}
		return result;
	}

	private IEnumerator lerpProgressUpdate()
	{
		while (true)
		{
			if (!GameManager.isPause)
			{
				m_totalDecreasedProgressTimer += Time.deltaTime * GameManager.timeScale;
				float lerpHpFillAmount = hpProgressLerpImage.fillAmount;
				float totalDecreasedHpFillAmount = hpProgressLerpImage2.fillAmount;
				if (m_totalDecreasedProgressTimer >= 0.6f)
				{
					if (totalDecreasedHpFillAmount != hpProgressImage.fillAmount)
					{
						totalDecreasedHpFillAmount = ((!(hpProgressImage.fillAmount > totalDecreasedHpFillAmount)) ? (totalDecreasedHpFillAmount - Time.deltaTime * GameManager.timeScale * 1.3f) : (totalDecreasedHpFillAmount + Time.deltaTime * GameManager.timeScale * 1.3f));
					}
					if (Mathf.Abs(totalDecreasedHpFillAmount - hpProgressImage.fillAmount) <= Time.deltaTime * GameManager.timeScale * 1.3f)
					{
						totalDecreasedHpFillAmount = hpProgressImage.fillAmount;
					}
					hpProgressLerpImage2.fillAmount = totalDecreasedHpFillAmount;
				}
				if (lerpHpFillAmount != hpProgressImage.fillAmount)
				{
					lerpHpFillAmount = ((!(hpProgressImage.fillAmount > lerpHpFillAmount)) ? (lerpHpFillAmount - Time.deltaTime * GameManager.timeScale * 0.25f) : (lerpHpFillAmount + Time.deltaTime * GameManager.timeScale * 0.25f));
				}
				if (Mathf.Abs(lerpHpFillAmount - hpProgressImage.fillAmount) <= Time.deltaTime * GameManager.timeScale * 0.25f)
				{
					lerpHpFillAmount = hpProgressImage.fillAmount;
				}
				hpProgressLerpImage.fillAmount = lerpHpFillAmount;
			}
			yield return null;
		}
	}

	private IEnumerator followTargetEnemyUpdate()
	{
		while (true)
		{
			hpText.text = GameManager.changeUnit(targetEnemy.curHealth);
			float curFullAmount = (float)(targetEnemy.curHealth / targetEnemy.maxHealth);
			hpProgressImage.fillAmount = curFullAmount;
			if (targetEnemy.isDead)
			{
				break;
			}
			m_disableTimer += Time.deltaTime * GameManager.timeScale;
			if (!isBossInformation)
			{
				base.cachedTransform.position = new Vector2(targetEnemy.cachedTransform.position.x, targetEnemy.cachedTransform.position.y - 0.2f);
			}
			yield return null;
		}
		hpText.text = "0";
		base.cachedAnimation.Stop();
		base.cachedAnimation.Play("CloseZombieInformation");
	}

	public void closeInformation()
	{
		if (!isBossInformation)
		{
			base.cachedRectTransform.anchoredPosition = new Vector2(1500f, 1500f);
		}
		StopCoroutine("followTargetEnemyUpdate");
		enemyInformationList.Remove(targetEnemy);
		targetEnemy = null;
		isOpen = false;
	}
}
