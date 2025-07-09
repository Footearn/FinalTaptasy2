using System.Collections;
using UnityEngine;

public class AdsAngelObject : ObjectBase
{
	public AdsAngelManager.AngelRewardType currentRewardType;

	public AdsAngelManager.SpecialAngelRewardType currentSpecialRewardType;

	public Animator cachedAnimator;

	public SpriteAnimation cachedSpriteAnimation;

	public ParticleSystem trailEffect;

	private bool m_isDead;

	private bool m_isSpecialAngel;

	public void initAdsAngel(AdsAngelManager.AngelRewardType targetRewardType)
	{
		m_isSpecialAngel = false;
		m_isDead = false;
		base.cachedRectTransform.localScale = Vector3.one;
		currentRewardType = targetRewardType;
		currentSpecialRewardType = AdsAngelManager.SpecialAngelRewardType.None;
		switch (targetRewardType)
		{
		case AdsAngelManager.AngelRewardType.Gold:
			cachedSpriteAnimation.playAnimation("AdsGoldAngel", 0.1f, true);
			break;
		case AdsAngelManager.AngelRewardType.AutoTouch:
			cachedSpriteAnimation.playAnimation("AdsTouchAngel", 0.1f, true);
			break;
		case AdsAngelManager.AngelRewardType.AutoOpenTreasureChest:
			cachedSpriteAnimation.playAnimation("AdsChestAngel", 0.1f, true);
			break;
		}
		base.cachedRectTransform.anchoredPosition = new Vector2(450f, 593f);
		trailEffect.Play();
		base.cachedAnimation.Play("AppearAnimation");
	}

	public void initAdsAngel(AdsAngelManager.SpecialAngelRewardType targetRewardType)
	{
		m_isSpecialAngel = true;
		m_isDead = false;
		base.cachedRectTransform.localScale = Vector3.one;
		currentRewardType = AdsAngelManager.AngelRewardType.NULL;
		currentSpecialRewardType = targetRewardType;
		switch (currentSpecialRewardType)
		{
		case AdsAngelManager.SpecialAngelRewardType.Damage:
			cachedSpriteAnimation.playAnimation("AdsColonyAngel", 0.1f, true);
			break;
		case AdsAngelManager.SpecialAngelRewardType.Health:
			cachedSpriteAnimation.playAnimation("AdsColonyAngel", 0.1f, true);
			break;
		}
		base.cachedRectTransform.anchoredPosition = new Vector2(-452f, 574f);
		trailEffect.Play();
		base.cachedAnimation.Play("SpecialAdsAngelAppearAnimation");
	}

	private IEnumerator waitForDie()
	{
		yield return new WaitForSeconds(3f);
		endAngel();
	}

	public void endAnimation()
	{
		m_isDead = true;
		trailEffect.Stop();
		if (base.cachedGameObject.activeSelf)
		{
			StartCoroutine("waitForDie");
		}
		base.cachedAnimation.CrossFade("DisappearAnimation");
		UIWindowAdsAngelBuffDialog.instance.close();
	}

	public void endAngel()
	{
		if (!m_isSpecialAngel)
		{
			Singleton<AdsAngelManager>.instance.recycleAngel();
		}
		else
		{
			Singleton<AdsAngelManager>.instance.recycleSpecialAngel();
		}
	}

	public void playStayAnimation()
	{
		if (!m_isSpecialAngel)
		{
			base.cachedAnimation.CrossFade("StayAnimation");
		}
		else
		{
			base.cachedAnimation.CrossFade("SpecialAdsAngelStayAnimation");
		}
	}

	public void onClickAngel()
	{
		if (m_isDead)
		{
			return;
		}
		if (!m_isSpecialAngel)
		{
			UIWindowAdsAngelBuffDialog.instance.openWithAdsAngelBuffInformation(currentRewardType, delegate
			{
				endAnimation();
				Singleton<AdsManager>.instance.showAds("adsAngel", delegate
				{
					Singleton<AdsAngelManager>.instance.rewardEvent(currentRewardType);
				});
			});
			return;
		}
		UIWindowAdsAngelBuffDialog.instance.openWithAdsAngelBuffInformation(currentSpecialRewardType, delegate
		{
			endAnimation();
			Singleton<AdsManager>.instance.showAds("adsAngel", delegate
			{
				Singleton<AdsAngelManager>.instance.rewardEvent(currentSpecialRewardType);
			});
		});
	}
}
