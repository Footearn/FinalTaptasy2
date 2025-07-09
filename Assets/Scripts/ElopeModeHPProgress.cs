using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElopeModeHPProgress : ObjectBase
{
	public CanvasGroup cachedCanvasGroup;

	public ElopeEnemyObject targetEnemyObject;

	public Vector2 offset;

	public Image hpProgressImage;

	public Image hpLerpProgressImage;

	public Text hpText;

	public Text nameText;

	private bool m_isOpen;

	public void openEnemyHPProgress(ElopeEnemyObject enemy)
	{
		m_isOpen = true;
		cachedCanvasGroup.alpha = 0f;
		targetEnemyObject = enemy;
		float fillAmount = (float)(targetEnemyObject.currentHelath / targetEnemyObject.maxHelath);
		hpProgressImage.fillAmount = fillAmount;
		hpLerpProgressImage.fillAmount = fillAmount;
		string text = string.Empty;
		if (enemy.currentCharacterType != CharacterManager.CharacterType.Length)
		{
			switch (enemy.currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(enemy.currentWarriorSkinType);
				break;
			case CharacterManager.CharacterType.Priest:
				text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(enemy.currentPriestSkinType);
				break;
			case CharacterManager.CharacterType.Archer:
				text = Singleton<CharacterSkinManager>.instance.getCharacterSkinName(enemy.currentArcherSkinType);
				break;
			}
		}
		else if (enemy.currentColleagueType != ColleagueManager.ColleagueType.None)
		{
			text = Singleton<ColleagueManager>.instance.getColleagueI18NName(enemy.currentColleagueType, enemy.currentColleaugeIndex);
		}
		string text2 = text;
		text = text2 + " <color=#FAD725FF>Lv." + enemy.currentLevel + "</color>";
		nameText.text = text;
		StopAllCoroutines();
		StartCoroutine(openHPProgressCoroutine());
	}

	public void closeEnemyHPProgress(bool forceClose = false)
	{
		m_isOpen = false;
		if (forceClose)
		{
			cachedCanvasGroup.alpha = 0f;
			targetEnemyObject = null;
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(closeHPProgressCoroutine());
		}
	}

	private IEnumerator openHPProgressCoroutine()
	{
		while (cachedCanvasGroup.alpha < 1f)
		{
			cachedCanvasGroup.alpha += Time.deltaTime * GameManager.timeScale * 6f;
			yield return null;
		}
		cachedCanvasGroup.alpha = 1f;
	}

	private IEnumerator closeHPProgressCoroutine()
	{
		while (cachedCanvasGroup.alpha > 0f)
		{
			cachedCanvasGroup.alpha -= Time.deltaTime * GameManager.timeScale * 6f;
			yield return null;
		}
		cachedCanvasGroup.alpha = 0f;
	}

	private void LateUpdate()
	{
		if (targetEnemyObject != null)
		{
			base.cachedTransform.position = targetEnemyObject.cachedTransform.position + (Vector3)offset;
			float num = (float)(targetEnemyObject.currentHelath / targetEnemyObject.maxHelath);
			hpProgressImage.fillAmount = num;
			hpLerpProgressImage.fillAmount = Mathf.Lerp(hpLerpProgressImage.fillAmount, num, Time.deltaTime * GameManager.timeScale * 7f);
			hpText.text = GameManager.changeUnit(targetEnemyObject.currentHelath);
			if (m_isOpen && targetEnemyObject.isDead)
			{
				closeEnemyHPProgress();
			}
		}
	}
}
