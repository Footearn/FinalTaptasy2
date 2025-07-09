using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowTowerMode : UIWindow
{
	public static UIWindowTowerMode instance;

	public Sprite filledLifeImage;

	public Sprite emptyLifeImage;

	public Image[] lifeCountImages;

	public bool isPressedSkillButton;

	public GameObject stunGaugeObject;

	public Image stunGaugeImage;

	public FollowObject stunGaugeFollowObject;

	public Text heartCoinText;

	public Text bestFloorText;

	public Transform heartCoinFlyTargetTransform;

	public GameObject pauseObject;

	public Text timerText;

	public TextMeshProUGUI floorText;

	public CanvasGroup hitEffectCanvasGroup;

	public TowerModeBossHPGauge bossHPGauge;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		refreshHeartCoinText();
		int num = 0;
		num = ((Singleton<TowerModeManager>.instance.currentDifficultyType != TowerModeManager.TowerModeDifficultyType.TimeAttack) ? Singleton<DataManager>.instance.currentGameData.towerModeEndlessModeSeasonBestFloor : Singleton<DataManager>.instance.currentGameData.towerModeTimeAttackSeasonBestFloor);
		bestFloorText.text = "Best : <color=#FDFCB7FF>" + ((num <= 0) ? "--" : (num + "F")) + "</color>";
		bossHPGauge.currentTargetBoss = null;
		bossHPGauge.currentTargetMap = null;
		bossHPGauge.setProgress(1.0, 1.0);
		StopAllCoroutines();
		bossHPGauge.cachedCanvasGroup.alpha = 0f;
		hitEffectCanvasGroup.alpha = 0f;
		isPressedSkillButton = false;
		closeStunGauge();
		return base.OnBeforeOpen();
	}

	public void OnClickCastSkill()
	{
		Singleton<TowerModeManager>.instance.curPlayingCharacter.castAttack();
	}

	public void setLifeCountImage(int n)
	{
		for (int i = 0; i < lifeCountImages.Length; i++)
		{
			if (i < n)
			{
				lifeCountImages[i].sprite = filledLifeImage;
			}
			else
			{
				lifeCountImages[i].sprite = emptyLifeImage;
			}
			lifeCountImages[i].SetNativeSize();
		}
	}

	public void refreshHeartCoinText()
	{
		heartCoinText.text = Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.HeartCoin].ToString("N0");
	}

	public void OnClickPause()
	{
		Singleton<GameManager>.instance.Pause(true);
	}

	public void OnClickResume()
	{
		Singleton<GameManager>.instance.Pause(false);
	}

	public void OnClickGiveUp()
	{
		UIWindowDialog.openDescription("TOWER_MODE_EXIT_DESCRIPTION", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
		{
			Singleton<CachedManager>.instance.coverUI.fadeOut(0.5f, delegate
			{
				Singleton<TowerModeManager>.instance.endTowerMode();
				Singleton<CachedManager>.instance.coverUI.fadeIn();
			});
		}, string.Empty);
	}

	public void openStunGauge()
	{
		stunGaugeImage.fillAmount = 1f;
		stunGaugeFollowObject.followTarget = Singleton<TowerModeManager>.instance.curPlayingCharacter.cachedTransform;
		stunGaugeObject.SetActive(true);
	}

	public void closeStunGauge()
	{
		stunGaugeFollowObject.followTarget = null;
		stunGaugeImage.fillAmount = 0f;
		stunGaugeObject.SetActive(false);
	}

	public void setStunGauge(float current, float max)
	{
		stunGaugeImage.fillAmount = current / max;
	}

	public void setTimerText(float timer)
	{
		timer = Math.Max(timer, 0f);
		timerText.text = string.Format("{0:0.#}s", timer);
	}

	public void setFloorText(int targetFloor)
	{
		floorText.text = "Floor " + targetFloor;
	}

	public void startHitEffect()
	{
		hitEffectCanvasGroup.alpha = 0f;
		StopCoroutine("hitEffectUpdate");
		StartCoroutine("hitEffectUpdate");
	}

	private IEnumerator hitEffectUpdate()
	{
		float alpha = 0f;
		bool switchForHitEffect = true;
		int switchCount = 0;
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (switchForHitEffect)
				{
					alpha = Mathf.Min(alpha + Time.deltaTime * GameManager.timeScale * 15f, 1f);
					if (alpha >= 1f)
					{
						switchForHitEffect = !switchForHitEffect;
					}
				}
				else
				{
					alpha = Mathf.Max(alpha - Time.deltaTime * GameManager.timeScale, 0f);
					if (alpha <= 0f)
					{
						switchCount++;
						if (switchCount >= 1)
						{
							break;
						}
						switchForHitEffect = !switchForHitEffect;
					}
				}
				hitEffectCanvasGroup.alpha = alpha;
			}
			yield return null;
		}
		hitEffectCanvasGroup.alpha = 0f;
	}
}
