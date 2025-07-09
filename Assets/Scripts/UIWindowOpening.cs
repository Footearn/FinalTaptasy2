using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowOpening : MonoBehaviour
{
	public static UIWindowOpening instance;

	public Image cover;

	private Action m_fadeCallback;

	private int sceneCount;

	private bool isSkip;

	private bool isFading;

	public GameObject[] sceneObject;

	public Animation introShake;

	public GameObject skipButtonObject;

	public MovingObject cloud1;

	public MovingObject cloud2;

	public Animation butterfly1Animation;

	public Image butterfly1Image;

	public Sprite[] butterfly1Sprites;

	public Animation butterfly2Animation;

	public Image butterfly2Image;

	public Sprite[] butterfly2Sprites;

	public Animation butterfly3Animation;

	public Image butterfly3Image;

	public Animation butterfly4Animation;

	public Image butterfly4Image;

	public Sprite[] butterfly4Sprites;

	public Animation[] scene3bubbleAnimations;

	public GameObject[] scene3cutObjects;

	public Image fingerImage;

	public Sprite[] fingerSprites;

	public Image smokeImage;

	public Sprite[] smokeSprites;

	public Image scene3Image;

	public Animation[] scene4Animations;

	public Animation[] scene5Animations;

	public Animation[] scene6Animations;

	public Image demonImage;

	public Sprite[] demonSprites;

	public Image handImage;

	public Sprite[] handSprites;

	public Animation[] scene7Animations;

	public MovingObject cloud;

	public Animation[] scene8Animations;

	public Image heroesImage;

	public Sprite[] heroesSprites;

	private bool m_isReplay;

	public void Awake()
	{
		instance = this;
	}

	private IEnumerator scene1()
	{
		Singleton<AudioManager>.instance.playBackgroundSound("lobby02");
		bool eventProcess2 = false;
		cloud1.cachedTransform.localPosition = new Vector2(149f, 245f);
		cloud2.cachedTransform.localPosition = new Vector2(-71f, 118f);
		cloud1.moveTo((Vector2)cloud1.cachedTransform.position + new Vector2(300f, 0f), 10f);
		cloud2.moveTo((Vector2)cloud2.cachedTransform.position + new Vector2(-300f, 0f), 10f);
		coverFadeIn(2f, delegate
		{
			eventProcess2 = true;
		});
		yield return new WaitUntil(() => eventProcess2);
		eventProcess2 = false;
		butterfly1Animation.Play();
		bool imageSwitch = false;
		while (butterfly1Animation.isPlaying)
		{
			butterfly1Image.sprite = butterfly1Sprites[(!imageSwitch) ? 1 : 0];
			butterfly1Image.SetNativeSize();
			imageSwitch = !imageSwitch;
			yield return new WaitForSeconds(0.3f);
		}
		butterfly2Animation.Play();
		while (butterfly2Animation.isPlaying)
		{
			butterfly2Image.sprite = butterfly2Sprites[(!imageSwitch) ? 1 : 0];
			butterfly2Image.SetNativeSize();
			imageSwitch = !imageSwitch;
			yield return new WaitForSeconds(0.2f);
		}
		nextScene();
	}

	private IEnumerator scene2()
	{
		bool imageSwitch = false;
		butterfly3Animation.Play();
		while (butterfly3Animation.isPlaying)
		{
			butterfly3Image.sprite = butterfly1Sprites[(!imageSwitch) ? 1 : 0];
			butterfly3Image.SetNativeSize();
			imageSwitch = !imageSwitch;
			yield return new WaitForSeconds(0.3f);
		}
		nextScene();
	}

	private IEnumerator scene3()
	{
		bool imageSwitch = false;
		bool eventProcess = false;
		TypingText princessText = null;
		butterfly4Animation.Play();
		while (butterfly4Animation.isPlaying)
		{
			butterfly4Image.sprite = butterfly4Sprites[(!imageSwitch) ? 1 : 0];
			butterfly4Image.SetNativeSize();
			imageSwitch = !imageSwitch;
			yield return new WaitForSeconds(0.2f);
		}
		scene3bubbleAnimations[0].Play();
		yield return new WaitWhile(() => scene3bubbleAnimations[0].isPlaying);
		princessText = ObjectPool.Spawn("TypingText", new Vector2(-114f, 74f), scene3bubbleAnimations[0].transform.GetChild(0)).GetComponent<TypingText>();
		princessText.textMessage.color = Color.black;
		princessText.textMessage.fontSize = 22;
		princessText.textMessage.horizontalOverflow = HorizontalWrapMode.Wrap;
		princessText.textMessage.verticalOverflow = VerticalWrapMode.Truncate;
		princessText.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		princessText.textMessage.rectTransform.sizeDelta = new Vector2(199f, 54.85714f);
		princessText.textMessage.rectTransform.anchoredPosition = new Vector2(2.75f, 14.25f);
		princessText.textMessage.alignment = TextAnchor.MiddleCenter;
		princessText.SetText(I18NManager.Get("INTRO_1"), 10f, delegate
		{
			eventProcess = true;
		});
		yield return new WaitUntil(() => eventProcess);
		eventProcess = false;
		scene3cutObjects[0].SetActive(true);
		yield return new WaitForSeconds(1f);
		fingerImage.sprite = fingerSprites[1];
		scene3bubbleAnimations[1].Play();
		princessText = ObjectPool.Spawn("TypingText", new Vector2(0f, 4.6f), scene3bubbleAnimations[1].transform.GetChild(0)).GetComponent<TypingText>();
		princessText.textMessage.color = Color.black;
		princessText.textMessage.fontSize = 22;
		princessText.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		princessText.textMessage.alignment = TextAnchor.MiddleCenter;
		princessText.transform.localPosition = new Vector2(0f, 4.6f);
		princessText.SetText(I18NManager.Get("INTRO_2"), 10f, null);
		yield return new WaitWhile(() => scene3bubbleAnimations[1].isPlaying);
		yield return new WaitForSeconds(1f);
		float timer2 = 0f;
		while (true)
		{
			timer2 += Time.deltaTime * 0.5f;
			if (timer2 >= 1f)
			{
				break;
			}
			scene3Image.color = new Color(1f, 1f, 1f, timer2);
			yield return null;
		}
		timer2 = 1f;
		scene3Image.color = new Color(1f, 1f, 1f, 1f);
		smokeImage.gameObject.SetActive(true);
		scene3bubbleAnimations[2].Play("Smoke");
		for (int i = 0; i < smokeSprites.Length; i++)
		{
			if (i == smokeSprites.Length - 1)
			{
				yield return new WaitWhile(() => scene3bubbleAnimations[2].isPlaying);
				smokeImage.sprite = smokeSprites[i];
				smokeImage.SetNativeSize();
				scene3bubbleAnimations[2].Play("Shake");
				introShake.Play();
				yield return new WaitWhile(() => scene3bubbleAnimations[2].isPlaying);
			}
			else
			{
				smokeImage.sprite = smokeSprites[i];
				smokeImage.SetNativeSize();
				yield return new WaitForSeconds(0.45f);
			}
		}
		coverFadeOut(1f, delegate
		{
			nextScene();
		});
	}

	private IEnumerator scene4()
	{
		TypingText princessText2 = null;
		bool eventProcess = false;
		coverFadeIn(3f);
		scene4Animations[0].Play();
		yield return new WaitWhile(() => scene4Animations[0].isPlaying);
		scene4Animations[1].gameObject.SetActive(true);
		scene4Animations[2].Play();
		princessText2 = ObjectPool.Spawn("TypingText", new Vector2(5f, 0f), scene4Animations[2].transform).GetComponent<TypingText>();
		princessText2.textMessage.color = Color.black;
		princessText2.textMessage.fontSize = 45;
		princessText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		princessText2.textMessage.alignment = TextAnchor.MiddleCenter;
		princessText2.SetTextImmediate(I18NManager.Get("INTRO_3"), 10f, 1f, null);
		yield return new WaitWhile(() => scene4Animations[2].isPlaying);
		yield return new WaitForSeconds(1f);
		scene4Animations[3].Play();
		yield return new WaitWhile(() => scene4Animations[3].isPlaying);
		scene4Animations[4].Play();
		introShake.Play();
		yield return new WaitWhile(() => scene4Animations[4].isPlaying);
		coverFadeOut(1.5f, delegate
		{
			nextScene();
		});
	}

	private IEnumerator scene5()
	{
		TypingText princessText = null;
		TypingText demonText2 = null;
		bool eventProcess2 = false;
		coverFadeIn(2f);
		scene6Animations[0].Play();
		yield return new WaitWhile(() => scene6Animations[0].isPlaying);
		scene6Animations[1].Play();
		yield return new WaitWhile(() => scene6Animations[1].isPlaying);
		demonText2 = ObjectPool.Spawn("TypingText", new Vector2(-190f, 120f), scene6Animations[1].transform).GetComponent<TypingText>();
		demonText2.textMessage.color = Color.black;
		demonText2.textMessage.fontSize = 45;
		demonText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		demonText2.textMessage.alignment = TextAnchor.MiddleCenter;
		demonText2.SetTextDelay(I18NManager.Get("INTRO_4"), 10f, 0f, delegate(int idx)
		{
			if (idx == 1)
			{
				eventProcess2 = true;
			}
		}, null);
		yield return new WaitUntil(() => eventProcess2);
		eventProcess2 = false;
		scene6Animations[2].Play();
		yield return new WaitWhile(() => scene6Animations[2].isPlaying);
		princessText = ObjectPool.Spawn("TypingText", new Vector2(110f, 160f), scene6Animations[2].transform).GetComponent<TypingText>();
		princessText.textMessage.color = Color.black;
		princessText.textMessage.fontSize = 40;
		princessText.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		princessText.textMessage.alignment = TextAnchor.MiddleCenter;
		handImage.sprite = handSprites[1];
		princessText.SetText(I18NManager.Get("INTRO_5"), 20f, delegate
		{
			handImage.sprite = handSprites[0];
			eventProcess2 = true;
		});
		yield return new WaitUntil(() => eventProcess2);
		eventProcess2 = false;
		scene6Animations[2].transform.GetChild(0).localScale = Vector2.zero;
		ObjectPool.Recycle("TypingText", demonText2.gameObject);
		ObjectPool.Recycle("TypingText", princessText.gameObject);
		demonText2 = ObjectPool.Spawn("TypingText", new Vector2(-190f, 120f), scene6Animations[1].transform).GetComponent<TypingText>();
		demonText2.textMessage.color = Color.black;
		demonText2.textMessage.fontSize = 45;
		demonText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		demonText2.textMessage.alignment = TextAnchor.MiddleCenter;
		demonText2.SetText(I18NManager.Get("INTRO_6"), 5f, delegate
		{
			eventProcess2 = true;
		});
		yield return new WaitUntil(() => eventProcess2);
		eventProcess2 = false;
		ObjectPool.Recycle("TypingText", demonText2.gameObject);
		demonText2 = ObjectPool.Spawn("TypingText", new Vector2(-190f, 120f), scene6Animations[1].transform).GetComponent<TypingText>();
		demonText2.textMessage.color = Color.black;
		demonText2.textMessage.fontSize = 45;
		demonText2.rectTransform.sizeDelta = new Vector2(407.6f, 113.6842f);
		demonText2.textMessage.verticalOverflow = VerticalWrapMode.Truncate;
		demonText2.textMessage.horizontalOverflow = HorizontalWrapMode.Wrap;
		demonText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		demonText2.textMessage.alignment = TextAnchor.MiddleCenter;
		demonText2.SetTextDelay(I18NManager.Get("INTRO_7"), 10f, 0f, delegate
		{
		}, delegate
		{
			eventProcess2 = true;
		});
		yield return new WaitUntil(() => eventProcess2);
		eventProcess2 = false;
		scene6Animations[2].Play();
		yield return new WaitWhile(() => scene6Animations[2].isPlaying);
		princessText = ObjectPool.Spawn("TypingText", new Vector2(110f, 160f), scene6Animations[2].transform).GetComponent<TypingText>();
		princessText.textMessage.color = Color.black;
		princessText.textMessage.fontSize = 40;
		princessText.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		princessText.textMessage.alignment = TextAnchor.MiddleCenter;
		handImage.sprite = handSprites[1];
		princessText.SetText(I18NManager.Get("INTRO_5"), 20f, delegate
		{
			handImage.sprite = handSprites[0];
			eventProcess2 = true;
		});
		yield return new WaitUntil(() => eventProcess2);
		eventProcess2 = false;
		scene6Animations[2].transform.GetChild(0).localScale = Vector2.zero;
		ObjectPool.Recycle("TypingText", demonText2.gameObject);
		ObjectPool.Recycle("TypingText", princessText.gameObject);
		demonText2 = ObjectPool.Spawn("TypingText", new Vector2(-190f, 120f), scene6Animations[1].transform).GetComponent<TypingText>();
		demonText2.textMessage.color = Color.black;
		demonText2.textMessage.fontSize = 45;
		demonText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		demonText2.textMessage.alignment = TextAnchor.MiddleCenter;
		demonText2.SetText(I18NManager.Get("INTRO_6"), 5f, delegate
		{
			eventProcess2 = true;
		});
		yield return new WaitUntil(() => eventProcess2);
		eventProcess2 = false;
		ObjectPool.Recycle("TypingText", demonText2.gameObject);
		scene6Animations[2].Play();
		yield return new WaitWhile(() => scene6Animations[2].isPlaying);
		princessText = ObjectPool.Spawn("TypingText", new Vector2(110f, 160f), scene6Animations[2].transform).GetComponent<TypingText>();
		princessText.textMessage.color = Color.black;
		princessText.textMessage.fontSize = 40;
		princessText.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		princessText.textMessage.alignment = TextAnchor.MiddleCenter;
		handImage.sprite = handSprites[1];
		princessText.SetText(I18NManager.Get("INTRO_5"), 20f, delegate
		{
			handImage.sprite = handSprites[0];
			eventProcess2 = true;
		});
		yield return new WaitUntil(() => eventProcess2);
		eventProcess2 = false;
		scene6Animations[2].transform.GetChild(0).localScale = Vector2.zero;
		ObjectPool.Recycle("TypingText", princessText.gameObject);
		demonImage.sprite = demonSprites[1];
		demonText2 = ObjectPool.Spawn("TypingText", new Vector2(-190f, 120f), scene6Animations[1].transform).GetComponent<TypingText>();
		demonText2.textMessage.color = Color.black;
		demonText2.textMessage.fontSize = 45;
		demonText2.textMessage.horizontalOverflow = HorizontalWrapMode.Wrap;
		demonText2.textMessage.verticalOverflow = VerticalWrapMode.Truncate;
		demonText2.textMessage.rectTransform.sizeDelta = new Vector2(417.1f, 113.6327f);
		demonText2.textMessage.rectTransform.anchoredPosition = new Vector2(-190f, 120f);
		demonText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		demonText2.textMessage.alignment = TextAnchor.MiddleCenter;
		demonText2.SetTextImmediate(I18NManager.Get("INTRO_8"), 15f, 1f, delegate
		{
		});
		introShake.Play();
		yield return new WaitWhile(() => introShake.isPlaying);
		introShake.Play();
		yield return new WaitWhile(() => introShake.isPlaying);
		yield return new WaitForSeconds(1f);
		nextScene();
	}

	private IEnumerator scene6()
	{
		TypingText warriorText2 = null;
		TypingText priestText2 = null;
		TypingText archerText2 = null;
		bool eventProcess5 = false;
		cloud.moveTo(new Vector2(285f, 374f), 10f);
		coverFadeIn(1f);
		yield return new WaitForSeconds(1f);
		eventProcess5 = false;
		scene7Animations[2].Play();
		archerText2 = ObjectPool.Spawn("TypingText", new Vector2(0f, 83f), scene7Animations[2].transform).GetComponent<TypingText>();
		archerText2.textMessage.color = Color.black;
		archerText2.textMessage.fontSize = 26;
		archerText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		archerText2.textMessage.alignment = TextAnchor.MiddleCenter;
		handImage.sprite = handSprites[1];
		archerText2.SetText(I18NManager.Get("INTRO_9"), 15f, delegate
		{
			eventProcess5 = true;
		});
		yield return new WaitUntil(() => eventProcess5);
		eventProcess5 = false;
		scene7Animations[0].Play();
		warriorText2 = ObjectPool.Spawn("TypingText", new Vector2(-4.5f, 110f), scene7Animations[0].transform).GetComponent<TypingText>();
		warriorText2.textMessage.color = Color.black;
		warriorText2.textMessage.fontSize = 22;
		warriorText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		warriorText2.textMessage.alignment = TextAnchor.MiddleCenter;
		handImage.sprite = handSprites[1];
		warriorText2.SetText(I18NManager.Get("INTRO_10"), 15f, delegate
		{
			eventProcess5 = true;
		});
		yield return new WaitUntil(() => eventProcess5);
		eventProcess5 = false;
		scene7Animations[1].Play();
		priestText2 = ObjectPool.Spawn("TypingText", new Vector2(108f, 103f), scene7Animations[1].transform).GetComponent<TypingText>();
		priestText2.textMessage.color = Color.black;
		priestText2.textMessage.fontSize = 26;
		priestText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		priestText2.textMessage.alignment = TextAnchor.MiddleCenter;
		handImage.sprite = handSprites[1];
		priestText2.SetText(I18NManager.Get("INTRO_11"), 15f, delegate
		{
			eventProcess5 = true;
		});
		yield return new WaitUntil(() => eventProcess5);
		eventProcess5 = false;
		yield return new WaitForSeconds(1f);
		nextScene();
	}

	private IEnumerator scene7()
	{
		TypingText warriorText2 = null;
		TypingText heroesText2 = null;
		bool eventProcess3 = false;
		scene8Animations[0].Play();
		scene8Animations[1].Play();
		warriorText2 = ObjectPool.Spawn("TypingText", new Vector2(8f, 120f), scene8Animations[1].transform).GetComponent<TypingText>();
		warriorText2.textMessage.color = Color.black;
		warriorText2.textMessage.fontSize = 40;
		warriorText2.textMessage.horizontalOverflow = HorizontalWrapMode.Wrap;
		warriorText2.textMessage.verticalOverflow = VerticalWrapMode.Truncate;
		warriorText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		warriorText2.textMessage.rectTransform.anchoredPosition = new Vector2(0f, 120f);
		warriorText2.textMessage.rectTransform.sizeDelta = new Vector2(299.7f, 86.20408f);
		warriorText2.textMessage.alignment = TextAnchor.MiddleCenter;
		handImage.sprite = handSprites[1];
		warriorText2.SetText(I18NManager.Get("INTRO_12"), 15f, delegate
		{
			eventProcess3 = true;
		});
		yield return new WaitUntil(() => eventProcess3);
		eventProcess3 = false;
		scene8Animations[2].Play();
		heroesImage.sprite = heroesSprites[1];
		heroesText2 = ObjectPool.Spawn("TypingText", new Vector2(0f, 0f), scene8Animations[2].transform).GetComponent<TypingText>();
		heroesText2.textMessage.color = Color.black;
		heroesText2.textMessage.fontSize = 26;
		heroesText2.textMessage.horizontalOverflow = HorizontalWrapMode.Wrap;
		heroesText2.textMessage.verticalOverflow = VerticalWrapMode.Truncate;
		heroesText2.textMessage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		heroesText2.textMessage.rectTransform.anchoredPosition = Vector2.zero;
		heroesText2.textMessage.rectTransform.sizeDelta = new Vector2(123.2f, 36.57143f);
		heroesText2.textMessage.alignment = TextAnchor.MiddleCenter;
		heroesText2.SetText(I18NManager.Get("INTRO_13"), 15f, delegate
		{
			eventProcess3 = true;
		});
		yield return new WaitUntil(() => eventProcess3);
		eventProcess3 = false;
		handImage.sprite = handSprites[1];
		coverFadeOut(2f, delegate
		{
			endScene();
		});
	}

	public void nextScene(bool isReplay = false)
	{
		m_isReplay = isReplay;
		ObjectPool.Clear("TypingText");
		for (int i = 0; i < sceneObject.Length; i++)
		{
			sceneObject[i].SetActive(false);
		}
		sceneObject[sceneCount].SetActive(true);
		sceneCount++;
		StartCoroutine("scene" + sceneCount);
	}

	private void endScene()
	{
		ObjectPool.Clear("TypingText");
		NSPlayerPrefs.SetInt("Intro", 0);
		Singleton<ParsingManager>.instance.introObjects.SetActive(false);
		Singleton<CachedManager>.instance.coverUI.fadeInGame(0.5f, delegate
		{
			Singleton<AudioManager>.instance.playBackgroundSound("lobby02");
		}, m_isReplay);
		Destroy();
	}

	public void Destroy()
	{
		Singleton<TutorialManager>.instance.introSkipButton = null;
		UnityEngine.Object.DestroyImmediate(base.gameObject);
		instance = null;
		Resources.UnloadUnusedAssets();
	}

	private void coverFadeIn(float time = 0.5f, Action callback = null, bool force = false)
	{
		isFading = true;
		StartCoroutine("coverFadeInRoutine", time);
		m_fadeCallback = callback;
	}

	private void coverFadeOut(float time = 0.5f, Action callback = null, bool force = false)
	{
		isFading = true;
		StartCoroutine("coverFadeOutRoutine", time);
		m_fadeCallback = callback;
	}

	private IEnumerator coverFadeInRoutine(float timeToTake)
	{
		float alphaPercentage = 1f;
		float alphaIteration = 1f / timeToTake;
		while (!(alphaPercentage < 0f))
		{
			alphaPercentage -= Time.deltaTime * alphaIteration;
			cover.color = new Color(0f, 0f, 0f, alphaPercentage);
			yield return null;
		}
		if (m_fadeCallback != null)
		{
			m_fadeCallback();
			m_fadeCallback = null;
		}
		isFading = false;
	}

	private IEnumerator coverFadeOutRoutine(float timeToTake)
	{
		float alphaPercentage = 0f;
		float alphaIteration = 1f / timeToTake;
		while (!(alphaPercentage > 1f))
		{
			alphaPercentage += Time.deltaTime * alphaIteration;
			cover.color = new Color(0f, 0f, 0f, alphaPercentage);
			yield return null;
		}
		if (m_fadeCallback != null)
		{
			m_fadeCallback();
			m_fadeCallback = null;
		}
		isFading = false;
	}

	public void OnClickSkipIntro()
	{
		if (!isSkip)
		{
			isSkip = true;
			StopAllCoroutines();
			coverFadeOut(0.5f, delegate
			{
				endScene();
			}, true);
		}
	}
}
