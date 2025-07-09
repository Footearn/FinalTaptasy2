using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowLotteryTreasure : UIWindow
{
	public static UIWindowLotteryTreasure instance;

	public Animation lotteryAnimation;

	public Image flashBlockImage;

	public Sprite[] lotteryBoxSprites;

	public Sprite[] lotteryBoxOpenSprites;

	private Action m_endLotteryAction;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openWithStartTreasureLottery(Action endAction)
	{
		flashBlockImage.color = new Color(1f, 1f, 1f, 0f);
		StopAllCoroutines();
		m_endLotteryAction = endAction;
		Singleton<AudioManager>.instance.playEffectSound("treasure_open");
		open();
	}

	private void OnEnable()
	{
		if (m_endLotteryAction != null)
		{
			lotteryAnimation.Play();
		}
	}

	private IEnumerator lotteryUpdate()
	{
		Color color;
		while (true)
		{
			color = flashBlockImage.color;
			color.a += Time.deltaTime * GameManager.timeScale * 12f;
			flashBlockImage.color = color;
			if (color.a >= 1f)
			{
				break;
			}
			yield return null;
		}
		color.a = 1f;
		flashBlockImage.color = color;
		m_endLotteryAction();
		m_endLotteryAction = null;
		close();
	}

	public void endLottery()
	{
		StopAllCoroutines();
		StartCoroutine("lotteryUpdate");
	}
}
