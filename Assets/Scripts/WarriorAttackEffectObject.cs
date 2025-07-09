using System.Collections;
using UnityEngine;

public class WarriorAttackEffectObject : ObjectBase
{
	private SpriteGroup m_cachedSpriteGroup;

	public static int transcendWarriorAttackEffectIndex;

	public SpriteRenderer targetRenderer;

	public int attackIndex;

	public float alpha = 1f;

	public float speed = 1f;

	private IEnumerator m_alphaUpdateCoroutine;

	public AutoRecycleEffect cachedAutoRecycleEffect;

	private void Awake()
	{
		m_cachedSpriteGroup = GetComponent<SpriteGroup>();
	}

	public void init()
	{
		initWarriorAttackEffect(Singleton<FeverManager>.instance.getCurrentAttackEffectIndex(), Singleton<DataManager>.instance.currentGameData.currentTranscendTier[CharacterManager.CharacterType.Warrior] > 0);
	}

	public void initWarriorAttackEffect(int effectIndex, bool isTranscendEffect)
	{
		bool flag = Singleton<FeverManager>.instance.getCurrentAttackEffectIndex() >= 4 && Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.SwordSoul).isUnlocked;
		if (isTranscendEffect)
		{
			if (transcendWarriorAttackEffectIndex >= 4)
			{
				transcendWarriorAttackEffectIndex = 0;
			}
			int count = Singleton<CharacterManager>.instance.transcendAttackEffectSpriteList[effectIndex].spriteDataList.Count;
			if (!flag)
			{
				targetRenderer.sprite = Singleton<CharacterManager>.instance.transcendAttackEffectSpriteList[effectIndex].spriteDataList[transcendWarriorAttackEffectIndex];
			}
			else
			{
				targetRenderer.sprite = Singleton<CharacterManager>.instance.transcendSwordSoulAttackEffectSpriteList.spriteDataList[transcendWarriorAttackEffectIndex];
			}
			cachedAutoRecycleEffect.duration = 0.25f;
			speed = 11f;
			transcendWarriorAttackEffectIndex++;
		}
		else
		{
			if (!flag)
			{
				targetRenderer.sprite = Singleton<CharacterManager>.instance.attackEffectSpriteList[effectIndex].spriteDataList[attackIndex];
			}
			else
			{
				targetRenderer.sprite = Singleton<CharacterManager>.instance.swordSoulAttackEffectSpriteList.spriteDataList[attackIndex];
			}
			cachedAutoRecycleEffect.duration = 0.1f;
			speed = 25f;
		}
		m_cachedSpriteGroup.setAlpha(1f);
		if (m_alphaUpdateCoroutine != null)
		{
			StopCoroutine(m_alphaUpdateCoroutine);
		}
		m_alphaUpdateCoroutine = alphaUpdate();
		StartCoroutine(alphaUpdate());
		alpha = 1f;
	}

	private IEnumerator alphaUpdate()
	{
		float timer = 0f;
		while (true)
		{
			if (!GameManager.isPause || GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= 0.05f)
				{
					alpha -= Time.deltaTime * GameManager.timeScale * speed;
					m_cachedSpriteGroup.setAlpha(alpha);
					if (alpha <= 0f)
					{
						break;
					}
				}
			}
			yield return null;
		}
		m_alphaUpdateCoroutine = null;
	}
}
