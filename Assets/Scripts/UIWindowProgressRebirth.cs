using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowProgressRebirth : UIWindow
{
	public static UIWindowProgressRebirth instance;

	public RectTransform characterImageRectTransform;

	public GameObject effectCanvasObject;

	public GameObject characterCanvasObject;

	public Animation characterUIAnimation;

	public Image blockImage;

	public Image flashImage;

	public UIWindowRebirthResult resultDialog;

	private long m_targetRewardKeyValue;

	public override void Awake()
	{
		instance = this;
		ignoreOnCloseAll = true;
		base.Awake();
	}

	public void openWithStartRebirth(long targetRewardKeyValue)
	{
		m_targetRewardKeyValue = targetRewardKeyValue;
		effectCanvasObject.SetActive(false);
		characterCanvasObject.SetActive(true);
		characterUIAnimation.Stop();
		characterImageRectTransform.anchoredPosition = new Vector2(-1.300003f, 207f);
		characterImageRectTransform.localScale = Vector3.one;
		blockImage.color = new Color(0f, 0f, 0f, 0f);
		flashImage.color = new Color(1f, 1f, 1f, 0f);
		open();
		characterUIAnimation.Play();
		StopAllCoroutines();
		effectCanvasObject.SetActive(true);
		StartCoroutine("alphaUpdate");
		Singleton<AudioManager>.instance.playBackgroundSound("rebirth");
	}

	private IEnumerator alphaUpdate()
	{
		Singleton<AudioManager>.instance.playBackgroundSound("lobby02");
		Color color = blockImage.color;
		while (true)
		{
			color = blockImage.color;
			color.a += Time.deltaTime * 1.2f;
			blockImage.color = color;
			if (color.a >= 1f)
			{
				break;
			}
			yield return null;
		}
		UIWindowRebirth.instance.close();
		color.a = 1f;
		blockImage.color = color;
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
	}

	public void closeRebirth()
	{
		StopCoroutine("closeAlphaUpdate");
		StartCoroutine("closeAlphaUpdate");
	}

	private IEnumerator closeAlphaUpdate()
	{
		yield return new WaitForSeconds(2f);
		Color color;
		while (true)
		{
			color = flashImage.color;
			color.a += Time.deltaTime * 0.8f;
			flashImage.color = color;
			if (color.a >= 1f)
			{
				break;
			}
			yield return null;
		}
		color.a = 1f;
		blockImage.color = new Color(0f, 0f, 0f, 0f);
		effectCanvasObject.SetActive(false);
		characterCanvasObject.SetActive(false);
		flashImage.color = color;
		yield return new WaitForSeconds(2f);
		UIWindowOutgame.instance.manageHeroIndigator.SetActive(false);
		UIWindowOutgame.instance.manageShopIndigator.SetActive(false);
		UIWindowOutgame.instance.manageSkillIndigator.SetActive(false);
		UIWindowOutgame.instance.refreshTreasureIndicator();
		UIWindowOutgame.instance.questIndigator.SetActive(false);
		UIWindowOutgame.instance.colleagueIndigator.SetActive(false);
		UIWindowOutgame.instance.achievementIndigator.SetActive(false);
		Singleton<GameManager>.instance.resetGameState();
		Singleton<GameManager>.instance.setGameState(GameManager.GameState.OutGame);
		StopCoroutine("closeAlphaUpdateFadeOut");
		StartCoroutine("closeAlphaUpdateFadeOut");
	}

	private IEnumerator closeAlphaUpdateFadeOut()
	{
		StartCoroutine("heroRay");
		while (true)
		{
			Color color = flashImage.color;
			color.a -= Time.deltaTime * 0.8f;
			flashImage.color = color;
			if (color.a < 0f)
			{
				break;
			}
			yield return null;
		}
		flashImage.color = new Color(1f, 1f, 1f, 0f);
	}

	private IEnumerator heroRay()
	{
		UIWindowOutgame.instance.warriorTabImage.gameObject.SetActive(false);
		UIWindowOutgame.instance.priestTabImage.gameObject.SetActive(false);
		UIWindowOutgame.instance.archerTabImage.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.8f);
		Singleton<AudioManager>.instance.playEffectSound("rebirth_sfx");
		UIWindowOutgame.instance.warriorTabImage.gameObject.SetActive(true);
		ObjectPool.Spawn("fx_rebirth_hero", (Vector2)UIWindowOutgame.instance.warriorTabImage.transform.position);
		Singleton<CharacterManager>.instance.warriorCharacter.setShader(CharacterManager.CharacterShaderType.RebirthShader);
		yield return new WaitForSeconds(0.6f);
		Singleton<AudioManager>.instance.playEffectSound("rebirth_sfx");
		UIWindowOutgame.instance.priestTabImage.gameObject.SetActive(true);
		ObjectPool.Spawn("fx_rebirth_hero", (Vector2)UIWindowOutgame.instance.priestTabImage.transform.position);
		Singleton<CharacterManager>.instance.priestCharacter.setShader(CharacterManager.CharacterShaderType.RebirthShader);
		yield return new WaitForSeconds(0.6f);
		Singleton<AudioManager>.instance.playEffectSound("rebirth_sfx");
		UIWindowOutgame.instance.archerTabImage.gameObject.SetActive(true);
		ObjectPool.Spawn("fx_rebirth_hero", (Vector2)UIWindowOutgame.instance.archerTabImage.transform.position);
		Singleton<CharacterManager>.instance.archerCharacter.setShader(CharacterManager.CharacterShaderType.RebirthShader);
		yield return new WaitForSeconds(1.5f);
		close();
		resultDialog.SetResult(m_targetRewardKeyValue);
	}
}
