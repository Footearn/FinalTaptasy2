using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElopeEnemyObject : MovingObject
{
	public long currentLevel;

	public CharacterManager.CharacterType currentCharacterType;

	public ColleagueManager.ColleagueType currentColleagueType;

	public int currentColleaugeIndex;

	public CharacterSkinManager.WarriorSkinType currentWarriorSkinType;

	public CharacterSkinManager.PriestSkinType currentPriestSkinType;

	public CharacterSkinManager.ArcherSkinType currentArcherSkinType;

	public Transform weaponParentTransform;

	public BoneAnimationNameData currentBoneAnimationName;

	public CharacterManager.CharacterBoneAnimationSpriteRendererData characterBoneSpriteRendererData;

	public SpriteRenderer weaponSpriteRenderer;

	public Animation characterBoneAnimation;

	private string m_currentAnimationName;

	public List<SpriteRenderer> cachedSpriteRendererList;

	public SpriteGroup cachedSpriteGroup;

	public bool isDead;

	public double maxHelath;

	public double currentHelath;

	public SpriteRenderer[] characterSpriteRenderers;

	private Transform m_cachedBodyTransform;

	private void Awake()
	{
		cachedSpriteRendererList = new List<SpriteRenderer>();
		cachedSpriteRendererList.Add(characterBoneSpriteRendererData.capeSpriteRenderer);
		cachedSpriteRendererList.Add(characterBoneSpriteRendererData.hairSpriteRenderer);
		cachedSpriteRendererList.Add(characterBoneSpriteRendererData.headSpriteRenderer);
		cachedSpriteRendererList.Add(characterBoneSpriteRendererData.leftWingSpriteRenderer);
		for (int i = 0; i < characterBoneSpriteRendererData.legSpriteRenderer.Length; i++)
		{
			cachedSpriteRendererList.Add(characterBoneSpriteRendererData.legSpriteRenderer[i]);
		}
		cachedSpriteRendererList.Add(characterBoneSpriteRendererData.rightWingSpriteRenderer);
		cachedSpriteRendererList.Add(characterBoneSpriteRendererData.shieldSpriteRenderer);
		cachedSpriteRendererList.Add(characterBoneSpriteRendererData.spineSpriteRenderer);
		cachedSpriteRendererList.Add(characterBoneSpriteRendererData.tailSpriteRenderer);
		cachedSpriteRendererList.Add(characterBoneSpriteRendererData.weaponSpriteRenderer);
	}

	private void removeAllAnimations()
	{
		List<string> list = new List<string>();
		foreach (AnimationState item in characterBoneAnimation)
		{
			list.Add(item.name);
		}
		for (int i = 0; i < list.Count; i++)
		{
			characterBoneAnimation.RemoveClip(list[i]);
		}
	}

	public void initElopeCharacter(long level, CharacterManager.CharacterType prevCharacaterType, ColleagueManager.ColleagueType prevColleagueType)
	{
		m_currentAnimationName = string.Empty;
		isDead = false;
		currentLevel = level;
		setShader(CharacterManager.CharacterShaderType.DefaultShader);
		if (level == Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode + 1)
		{
			maxHelath = Singleton<ElopeModeManager>.instance.getEnemyMaxHealth(currentLevel);
			currentHelath = Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode;
		}
		else
		{
			maxHelath = (currentHelath = Singleton<ElopeModeManager>.instance.getEnemyMaxHealth(currentLevel));
		}
		int num = UnityEngine.Random.Range(0, 10000);
		if (num % 2 == 0)
		{
			currentColleagueType = ColleagueManager.ColleagueType.None;
			currentColleaugeIndex = 0;
			float num2 = ((prevCharacaterType != 0) ? 33.33f : 0f);
			float num3 = ((prevCharacaterType != CharacterManager.CharacterType.Priest) ? 33.33f : 0f);
			float num4 = ((prevCharacaterType != CharacterManager.CharacterType.Archer) ? 33.33f : 0f);
			double num5 = (double)UnityEngine.Random.Range(0f, num2 + num3 + num4) / 100.0;
			if (num5 < (double)num2)
			{
				initElopeCharacter((CharacterSkinManager.WarriorSkinType)UnityEngine.Random.Range(0, 30));
			}
			else if (num5 < (double)(num2 + num3))
			{
				initElopeCharacter((CharacterSkinManager.PriestSkinType)UnityEngine.Random.Range(0, 30));
			}
			else if (num5 < (double)(num2 + num3 + num4))
			{
				initElopeCharacter((CharacterSkinManager.ArcherSkinType)UnityEngine.Random.Range(0, 30));
			}
		}
		else
		{
			currentCharacterType = CharacterManager.CharacterType.Length;
			currentColleagueType = (ColleagueManager.ColleagueType)UnityEngine.Random.Range(0, 28);
			while (!Singleton<ColleagueManager>.instance.isHumanColleauge(currentColleagueType) || prevColleagueType == currentColleagueType)
			{
				currentColleagueType = (ColleagueManager.ColleagueType)UnityEngine.Random.Range(0, 28);
			}
			currentColleaugeIndex = UnityEngine.Random.Range(1, Singleton<ColleagueManager>.instance.getColleagueSkinMaxCount(currentColleagueType) + 1);
			Singleton<ColleagueManager>.instance.changeColleagueSkin(currentColleagueType, currentColleaugeIndex, characterBoneSpriteRendererData);
			removeAllAnimations();
			List<AnimationClip> list = null;
			if (currentColleagueType == ColleagueManager.ColleagueType.Balbaria || currentColleagueType == ColleagueManager.ColleagueType.Thyrael || currentColleagueType == ColleagueManager.ColleagueType.GoldenFork || currentColleagueType == ColleagueManager.ColleagueType.Prince)
			{
				list = Singleton<AnimationManager>.instance.warriorNormalAnimationClipList;
			}
			else if (currentColleagueType == ColleagueManager.ColleagueType.Isabelle || currentColleagueType == ColleagueManager.ColleagueType.Sera || currentColleagueType == ColleagueManager.ColleagueType.Dinnerless || currentColleagueType == ColleagueManager.ColleagueType.FatherKing || currentColleagueType == ColleagueManager.ColleagueType.Seaghoul || currentColleagueType == ColleagueManager.ColleagueType.Poty)
			{
				list = Singleton<AnimationManager>.instance.priestNormalAnimationClipList;
			}
			else if (currentColleagueType == ColleagueManager.ColleagueType.Lawrence)
			{
				list = Singleton<AnimationManager>.instance.archerNormalAnimationClipList;
			}
			for (int i = 0; i < list.Count; i++)
			{
				characterBoneAnimation.AddClip(list[i], list[i].name);
			}
			playBoneAnimation(currentBoneAnimationName.idleName[0]);
		}
		if (m_cachedBodyTransform == null)
		{
			m_cachedBodyTransform = characterBoneAnimation.transform;
		}
		if (m_cachedBodyTransform.localEulerAngles != Vector3.zero)
		{
			m_cachedBodyTransform.localEulerAngles = Vector3.zero;
		}
		cachedSpriteGroup.setAlpha(1f);
		setDirection(Direction.Left);
	}

	public void initElopeCharacter(CharacterSkinManager.WarriorSkinType targetCharacterSkinType)
	{
		StopAllCoroutines();
		currentWarriorSkinType = targetCharacterSkinType;
		currentCharacterType = CharacterManager.CharacterType.Warrior;
		Singleton<CharacterManager>.instance.changeCharacterSkin(targetCharacterSkinType, characterBoneSpriteRendererData);
		removeAllAnimations();
		List<AnimationClip> list = null;
		list = ((targetCharacterSkinType != CharacterSkinManager.WarriorSkinType.Valkyrie1) ? Singleton<AnimationManager>.instance.warriorNormalAnimationClipList : Singleton<AnimationManager>.instance.warriorValkyrieAnimationClipList);
		for (int i = 0; i < list.Count; i++)
		{
			characterBoneAnimation.AddClip(list[i], list[i].name);
		}
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
		WeaponManager.WarriorWeaponType weaponType = (WeaponManager.WarriorWeaponType)UnityEngine.Random.Range(1, 48);
		weaponSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(weaponType);
	}

	private void initElopeCharacter(CharacterSkinManager.PriestSkinType targetCharacterSkinType)
	{
		StopAllCoroutines();
		currentPriestSkinType = targetCharacterSkinType;
		currentCharacterType = CharacterManager.CharacterType.Priest;
		Singleton<CharacterManager>.instance.changeCharacterSkin(targetCharacterSkinType, characterBoneSpriteRendererData);
		removeAllAnimations();
		List<AnimationClip> list = null;
		list = ((targetCharacterSkinType != CharacterSkinManager.PriestSkinType.Valkyrie2) ? Singleton<AnimationManager>.instance.priestNormalAnimationClipList : Singleton<AnimationManager>.instance.priestValkyrieAnimationClipList);
		for (int i = 0; i < list.Count; i++)
		{
			characterBoneAnimation.AddClip(list[i], list[i].name);
		}
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
		WeaponManager.PriestWeaponType weaponType = (WeaponManager.PriestWeaponType)UnityEngine.Random.Range(1, 48);
		weaponSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(weaponType);
	}

	private void initElopeCharacter(CharacterSkinManager.ArcherSkinType targetCharacterSkinType)
	{
		StopAllCoroutines();
		currentArcherSkinType = targetCharacterSkinType;
		currentCharacterType = CharacterManager.CharacterType.Archer;
		Singleton<CharacterManager>.instance.changeCharacterSkin(targetCharacterSkinType, characterBoneSpriteRendererData);
		removeAllAnimations();
		List<AnimationClip> list = null;
		list = ((targetCharacterSkinType != CharacterSkinManager.ArcherSkinType.Valkyrie3) ? Singleton<AnimationManager>.instance.archerNormalAnimationClipList : Singleton<AnimationManager>.instance.archerValkyrieAnimationClipList);
		for (int i = 0; i < list.Count; i++)
		{
			characterBoneAnimation.AddClip(list[i], list[i].name);
		}
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
		WeaponManager.ArcherWeaponType weaponType = (WeaponManager.ArcherWeaponType)UnityEngine.Random.Range(1, 48);
		weaponSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(weaponType);
	}

	public void playBoneAnimation(string animationName, bool forcePlay = false)
	{
		if (m_currentAnimationName != animationName || forcePlay)
		{
			m_currentAnimationName = animationName;
			characterBoneAnimation.Stop();
			characterBoneAnimation.Play(animationName);
		}
	}

	public void decreaseHealth(double damage, bool isCritical)
	{
		ObjectPool.Spawn("@ElopeDamageText", base.cachedTransform.position + new Vector3(0f, 2.1f)).GetComponent<CustomText>().setText(damage, (!isCritical) ? 1f : 1.5f, CustomText.TextEffectType.Up, (!isCritical) ? 1f : 1.5f);
		if (!isDead)
		{
			ObjectPool.Spawn("@ElopeHitEffect", base.cachedTransform.position + new Vector3(0f, 0.6f, 0f));
			StartCoroutine(hitEffect());
			currentHelath = Math.Max(currentHelath - damage, 0.0);
			Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode = currentHelath;
			if (currentHelath <= 0.0)
			{
				deadEvent();
			}
		}
	}

	private IEnumerator hitEffect()
	{
		setShader(CharacterManager.CharacterShaderType.ReplaceShader);
		float timer = 0f;
		int hitCount = 0;
		bool switchShader = false;
		while (true)
		{
			timer += Time.deltaTime;
			if (timer >= 0.05f)
			{
				hitCount++;
				timer = 0f;
				if (!switchShader)
				{
					setShader(CharacterManager.CharacterShaderType.DefaultShader);
				}
				else
				{
					setShader(CharacterManager.CharacterShaderType.ReplaceShader);
				}
				switchShader = !switchShader;
				if (hitCount >= 3)
				{
					break;
				}
			}
			yield return null;
		}
		setShader(CharacterManager.CharacterShaderType.DefaultShader);
	}

	private void setShader(CharacterManager.CharacterShaderType shaderType)
	{
		if (characterSpriteRenderers == null || characterSpriteRenderers.Length <= 0)
		{
			return;
		}
		switch (shaderType)
		{
		case CharacterManager.CharacterShaderType.DefaultShader:
		{
			for (int j = 0; j < characterSpriteRenderers.Length; j++)
			{
				characterSpriteRenderers[j].material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
			}
			break;
		}
		case CharacterManager.CharacterShaderType.ReplaceShader:
		{
			for (int i = 0; i < characterSpriteRenderers.Length; i++)
			{
				characterSpriteRenderers[i].material = Singleton<ResourcesManager>.instance.redColorSpriteDefaultMaterial;
			}
			break;
		}
		case CharacterManager.CharacterShaderType.RebirthShader:
			StartCoroutine("heroWhite");
			break;
		}
	}

	private void deadEvent()
	{
		isDead = true;
		Singleton<AudioManager>.instance.playEffectSound("elope_die");
		Singleton<DataManager>.instance.currentGameData.currentProgressDistanceForElopeMode++;
		Singleton<ElopeModeManager>.instance.checkDistanceEvent();
		UIWindowElopeMode.instance.refreshDistanceStatus();
		double value = CalculateManager.getGoldValueForMonsters() * 0.33000001311302185;
		for (int i = 0; i < 3; i++)
		{
			Singleton<GoldManager>.instance.spawnGold(base.cachedTransform.position, value);
		}
		Singleton<ElopeModeManager>.instance.currentElopeEnemyList.Remove(this);
		playBoneAnimation(currentBoneAnimationName.dieName[0]);
		StartCoroutine(dieAlphaUpdate());
	}

	private IEnumerator dieAlphaUpdate()
	{
		float alpha = 1f;
		while (alpha > 0f)
		{
			alpha -= Time.deltaTime * GameManager.timeScale * 1.5f;
			cachedSpriteGroup.setAlpha(alpha);
			yield return null;
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
