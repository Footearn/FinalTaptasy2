using UnityEngine;
using UnityEngine.UI;

public class UIWindowPVP : UIWindow
{
	public static UIWindowPVP instance;

	public RectTransform cachedCanvasRectTransform;

	public Image myTierIconImage;

	public Text myNickNameText;

	public Image enemyTierIconImage;

	public Text enemyNickNameText;

	public Image allyHPGaugeImage;

	public Image allyHPLerpGaugeImage;

	public Image enemyHPGaugeImage;

	public Image enemyHPLerpGaugeImage;

	public RectTransform myHPRectTransform;

	public RectTransform enemyHPRectTransform;

	public RectTransform leftRectTransform;

	public PVPSkillButton[] allySkillButtons;

	public PVPSkillButton[] allySkillBigButtons;

	public GameObject startEffectObject;

	private double m_allyTotalHP;

	private double m_allyCurrentHP;

	private double m_enemyTotalHP;

	private double m_enemyCurrentHP;

	private bool m_isOpenBlock;

	private Coroutine m_fadeUpdateCoroutine;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openPVPUI()
	{
		for (int i = 0; i < allySkillBigButtons.Length; i++)
		{
			allySkillBigButtons[i].closeSkillButton(true);
		}
		for (int j = 0; j < allySkillButtons.Length; j++)
		{
			allySkillButtons[j].closeSkillButton(true);
		}
		startEffectObject.SetActive(false);
		myNickNameText.text = Singleton<PVPManager>.instance.myPVPData.player.nickname;
		myTierIconImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(Singleton<PVPManager>.instance.myPVPData.player.grade);
		if (!Singleton<PVPManager>.instance.isPracticePVP)
		{
			enemyNickNameText.text = Singleton<PVPManager>.instance.enemyUnitData.nickname;
			enemyTierIconImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(Singleton<PVPManager>.instance.enemyUnitData.grade);
		}
		else
		{
			enemyNickNameText.text = Singleton<PVPManager>.instance.practiceTargetUserData.nickname;
			enemyTierIconImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(Singleton<PVPManager>.instance.practiceTargetUserData.grade);
		}
		myTierIconImage.SetNativeSize();
		enemyTierIconImage.SetNativeSize();
		open();
		float y = worldToCanvasPosition(leftRectTransform.position).y - 92.8f;
		myHPRectTransform.sizeDelta = new Vector2(98f, y);
		enemyHPRectTransform.sizeDelta = new Vector2(98f, y);
	}

	public void initTotalHPGauge(double totalHp, bool isAlly)
	{
		if (isAlly)
		{
			m_allyTotalHP = totalHp;
			m_allyCurrentHP = totalHp;
			allyHPGaugeImage.fillAmount = 1f;
			allyHPLerpGaugeImage.fillAmount = 1f;
		}
		else
		{
			m_enemyTotalHP = totalHp;
			m_enemyCurrentHP = totalHp;
			enemyHPGaugeImage.fillAmount = 1f;
			enemyHPLerpGaugeImage.fillAmount = 1f;
		}
	}

	public void increaseHP(double value, bool isAlly)
	{
		if (isAlly)
		{
			m_allyCurrentHP += value;
		}
		else
		{
			m_enemyCurrentHP += value;
		}
	}

	public void decreaseHP(double value, bool isAlly)
	{
		if (isAlly)
		{
			m_allyCurrentHP -= value;
		}
		else
		{
			m_enemyCurrentHP -= value;
		}
	}

	private void Update()
	{
		if (!GameManager.isPause)
		{
			float num = (float)(m_allyCurrentHP / m_allyTotalHP);
			allyHPGaugeImage.fillAmount = num;
			allyHPLerpGaugeImage.fillAmount = Mathf.Lerp(allyHPLerpGaugeImage.fillAmount, num, Time.deltaTime * GameManager.timeScale * 6f);
			float num2 = (float)(m_enemyCurrentHP / m_enemyTotalHP);
			enemyHPGaugeImage.fillAmount = num2;
			enemyHPLerpGaugeImage.fillAmount = Mathf.Lerp(enemyHPLerpGaugeImage.fillAmount, num2, Time.deltaTime * GameManager.timeScale * 6f);
		}
	}

	public Vector2 worldToCanvasPosition(Vector3 position)
	{
		Vector2 result = Singleton<CachedManager>.instance.pvpCamera.WorldToViewportPoint(position);
		result.x *= cachedCanvasRectTransform.sizeDelta.x;
		result.y *= cachedCanvasRectTransform.sizeDelta.y;
		result.x -= cachedCanvasRectTransform.sizeDelta.x * cachedCanvasRectTransform.pivot.x;
		result.y -= cachedCanvasRectTransform.sizeDelta.y * cachedCanvasRectTransform.pivot.y;
		return result;
	}

	private bool isOpenAnySkillButton()
	{
		bool flag = false;
		for (int i = 0; i < allySkillButtons.Length; i++)
		{
			if (allySkillButtons[i].isOpen)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			for (int j = 0; j < allySkillBigButtons.Length; j++)
			{
				if (allySkillBigButtons[j].isOpen)
				{
					flag = true;
					break;
				}
			}
		}
		return flag;
	}
}
