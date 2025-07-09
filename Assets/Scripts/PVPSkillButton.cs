using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PVPSkillButton : FollowObject
{
	public CanvasGroup cachedCanvasGroup;

	public Image skillIconImage;

	public Image skillAvailableTimeImage;

	public bool isOpen;

	private PVPSkillManager.PVPSkillTypeData m_currentSkillType;

	private PVPUnitObject m_currentUnitObject;

	private Coroutine m_fadeUpdateCoroutine;

	private float m_timer;

	public void initSkillButton(PVPUnitObject caster)
	{
		isOpen = false;
		cachedCanvasGroup.alpha = 0f;
		base.cachedGameObject.SetActive(false);
		skillIconImage.sprite = Singleton<PVPSkillManager>.instance.getSkillIconSprite(caster.currentSkillTypeData);
		skillIconImage.SetNativeSize();
		m_currentSkillType = caster.currentSkillTypeData;
		m_currentUnitObject = caster;
		followTarget = m_currentUnitObject.cachedTransform;
		if (m_currentUnitObject.currentColleagueType == ColleagueManager.ColleagueType.Golem || m_currentUnitObject.currentColleagueType == ColleagueManager.ColleagueType.Trall)
		{
			offset = new Vector2(-0.34887f, 2.595478f);
		}
		else
		{
			offset = new Vector2(-0.0180608f, 2.658f);
		}
		base.cachedGameObject.SetActive(true);
		openSkillButton();
	}

	private void openSkillButton()
	{
		if (!isOpen)
		{
			isOpen = true;
			m_timer = 0f;
			base.cachedGameObject.SetActive(true);
			if (m_fadeUpdateCoroutine != null)
			{
				StopCoroutine(m_fadeUpdateCoroutine);
			}
			m_fadeUpdateCoroutine = StartCoroutine(fadeUpdate(true));
			m_currentUnitObject.setSpriteLayer("PopUpLayer2", 100);
		}
	}

	public void closeSkillButton(bool force = false)
	{
		if (isOpen)
		{
			isOpen = false;
			if (m_fadeUpdateCoroutine != null)
			{
				StopCoroutine(m_fadeUpdateCoroutine);
			}
			if (!force)
			{
				m_fadeUpdateCoroutine = StartCoroutine(fadeUpdate(false));
			}
			else
			{
				base.cachedGameObject.SetActive(false);
				base.cachedRectTransform.anchoredPosition = Vector2.one * 1000f;
			}
			m_currentUnitObject.setSpriteLayer("Player", 0);
		}
	}

	private IEnumerator fadeUpdate(bool isOpen)
	{
		float alpha = ((!isOpen) ? 1 : 0);
		while ((!isOpen) ? (alpha > 0f) : (alpha < 1f))
		{
			if (!GameManager.isPause)
			{
				alpha += (float)(isOpen ? 1 : (-1)) * Time.deltaTime * GameManager.timeScale * 7f;
				cachedCanvasGroup.alpha = alpha;
			}
			yield return null;
		}
		if (!isOpen)
		{
			base.cachedGameObject.SetActive(false);
			base.cachedRectTransform.anchoredPosition = Vector2.one * 1000f;
		}
	}

	public void OnClickCastSkill()
	{
		if (PVPManager.isPlayingPVP && !(m_currentUnitObject == null) && !m_currentUnitObject.isDead && isOpen)
		{
			Singleton<PVPSkillManager>.instance.castSkill(m_currentUnitObject);
			closeSkillButton();
		}
	}

	private void Update()
	{
		if (isOpen)
		{
			if (!GameManager.isPause)
			{
				m_timer += Time.deltaTime * GameManager.timeScale;
				skillAvailableTimeImage.fillAmount = (PVPSkillManager.pvpSkillClickMaxTime - m_timer) / PVPSkillManager.pvpSkillClickMaxTime;
			}
			if (m_timer >= PVPSkillManager.pvpSkillClickMaxTime || m_currentUnitObject == null || m_currentUnitObject.isDead)
			{
				closeSkillButton();
			}
		}
	}
}
