using System.Collections.Generic;
using UnityEngine;

public class CharacterUIObject : ObjectBase
{
	public CharacterManager.CharacterType currentCharacterType;

	public CharacterSkinManager.WarriorSkinType currentWarriorSkinType;

	public CharacterSkinManager.PriestSkinType currentPriestSkinType;

	public CharacterSkinManager.ArcherSkinType currentArcherSkinType;

	public Transform weaponParentTransform;

	public BoneAnimationNameData currentBoneAnimationName;

	public CharacterManager.CharacterBoneAnimationSpriteRendererData characterBoneSpriteRendererData;

	public SpriteRenderer weaponSpriteRenderer;

	public Animation characterBoneAnimation;

	public List<SpriteRenderer> cachedSpriteRendererList;

	public SpriteGroup cachedSpriteGroup;

	public Material spriteMaskMaterial;

	private CanvasGroup m_targetFollowCanvasGroup;

	private AnimationClip m_idleAnimationClip;

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

	[ContextMenu("Reset Sorting Order")]
	public void resetSortingOrder()
	{
		if (characterBoneSpriteRendererData.shieldSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.shieldSpriteRenderer.sortingLayerName = "PopUpLayer";
			characterBoneSpriteRendererData.shieldSpriteRenderer.sortingOrder = 18;
		}
		if (characterBoneSpriteRendererData.headSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.headSpriteRenderer.sortingLayerName = "PopUpLayer";
			characterBoneSpriteRendererData.headSpriteRenderer.sortingOrder = 17;
		}
		if (characterBoneSpriteRendererData.hairSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.hairSpriteRenderer.sortingLayerName = "PopUpLayer";
			characterBoneSpriteRendererData.hairSpriteRenderer.sortingOrder = 14;
		}
		if (characterBoneSpriteRendererData.spineSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.spineSpriteRenderer.sortingLayerName = "PopUpLayer";
			characterBoneSpriteRendererData.spineSpriteRenderer.sortingOrder = 16;
		}
		if (characterBoneSpriteRendererData.legSpriteRenderer != null && characterBoneSpriteRendererData.legSpriteRenderer.Length > 0)
		{
			for (int i = 0; i < characterBoneSpriteRendererData.legSpriteRenderer.Length; i++)
			{
				characterBoneSpriteRendererData.legSpriteRenderer[i].sortingLayerName = "PopUpLayer";
				characterBoneSpriteRendererData.legSpriteRenderer[i].sortingOrder = 15;
			}
		}
		if (characterBoneSpriteRendererData.leftWingSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.leftWingSpriteRenderer.sortingLayerName = "PopUpLayer";
			characterBoneSpriteRendererData.leftWingSpriteRenderer.sortingOrder = 12;
		}
		if (characterBoneSpriteRendererData.rightWingSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.rightWingSpriteRenderer.sortingLayerName = "PopUpLayer";
			characterBoneSpriteRendererData.rightWingSpriteRenderer.sortingOrder = 12;
		}
		if (characterBoneSpriteRendererData.capeSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.capeSpriteRenderer.sortingLayerName = "PopUpLayer";
			characterBoneSpriteRendererData.capeSpriteRenderer.sortingOrder = 13;
		}
		if (characterBoneSpriteRendererData.weaponSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.weaponSpriteRenderer.sortingLayerName = "PopUpLayer";
			characterBoneSpriteRendererData.weaponSpriteRenderer.sortingOrder = 15;
		}
		if (characterBoneSpriteRendererData.tailSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.tailSpriteRenderer.sortingLayerName = "PopUpLayer";
			characterBoneSpriteRendererData.tailSpriteRenderer.sortingOrder = 15;
		}
	}

	public void changeLayer(string layerName, int sortingOrder = int.MinValue)
	{
		if (characterBoneSpriteRendererData.shieldSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.shieldSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.shieldSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.headSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.headSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.headSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.hairSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.hairSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.hairSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.spineSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.spineSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.spineSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.legSpriteRenderer != null && characterBoneSpriteRendererData.legSpriteRenderer.Length > 0)
		{
			for (int i = 0; i < characterBoneSpriteRendererData.legSpriteRenderer.Length; i++)
			{
				characterBoneSpriteRendererData.legSpriteRenderer[i].sortingLayerName = layerName;
				if (sortingOrder != int.MinValue)
				{
					characterBoneSpriteRendererData.legSpriteRenderer[i].sortingOrder = sortingOrder;
				}
			}
		}
		if (characterBoneSpriteRendererData.leftWingSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.leftWingSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.leftWingSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.rightWingSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.rightWingSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.rightWingSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.capeSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.capeSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.capeSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.weaponSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.weaponSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.weaponSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.tailSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.tailSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.tailSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.leftHandSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.leftHandSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.leftHandSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
		if (characterBoneSpriteRendererData.rightHandSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.rightHandSpriteRenderer.sortingLayerName = layerName;
			if (sortingOrder != int.MinValue)
			{
				characterBoneSpriteRendererData.rightHandSpriteRenderer.sortingOrder = sortingOrder;
			}
		}
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

	public void initCharacterUIObject(CharacterSkinManager.WarriorSkinType targetCharacterSkinType, CanvasGroup targetFollowCanvasGroup, string layerName = "PopUpLayer", int sortingOrder = int.MinValue)
	{
		m_targetFollowCanvasGroup = targetFollowCanvasGroup;
		changeLayer(layerName, sortingOrder);
		StopAllCoroutines();
		currentCharacterType = CharacterManager.CharacterType.Warrior;
		Singleton<CharacterManager>.instance.changeCharacterSkin(targetCharacterSkinType, characterBoneSpriteRendererData);
		removeAllAnimations();
		List<AnimationClip> list = null;
		list = ((targetCharacterSkinType != CharacterSkinManager.WarriorSkinType.Valkyrie1) ? Singleton<AnimationManager>.instance.warriorNormalAnimationClipList : Singleton<AnimationManager>.instance.warriorValkyrieAnimationClipList);
		m_idleAnimationClip = null;
		for (int i = 0; i < list.Count; i++)
		{
			characterBoneAnimation.AddClip(list[i], list[i].name);
			if (list[i].name.Equals("Idle"))
			{
				m_idleAnimationClip = list[i];
			}
		}
		characterBoneAnimation.clip = m_idleAnimationClip;
		characterBoneAnimation.Play();
		WeaponSkinData equippedWeaponSkinData = Singleton<WeaponSkinManager>.instance.getEquippedWeaponSkinData(CharacterManager.CharacterType.Warrior);
		if (equippedWeaponSkinData == null)
		{
			weaponSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(Singleton<DataManager>.instance.currentGameData.warriorEquippedWeapon);
		}
		else
		{
			bool flag = equippedWeaponSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || equippedWeaponSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || equippedWeaponSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
			weaponSpriteRenderer.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(equippedWeaponSkinData.currentWarriorSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(equippedWeaponSkinData.currentWarriorWeaponType));
		}
		cachedSpriteGroup.setAlpha(1f);
		cachedSpriteGroup.resetData();
		cachedSpriteGroup.setAlpha(0f);
	}

	public void initCharacterUIObject(CharacterSkinManager.PriestSkinType targetCharacterSkinType, CanvasGroup targetFollowCanvasGroup, string layerName = "PopUpLayer", int sortingOrder = int.MinValue)
	{
		m_targetFollowCanvasGroup = targetFollowCanvasGroup;
		changeLayer(layerName, sortingOrder);
		StopAllCoroutines();
		currentCharacterType = CharacterManager.CharacterType.Priest;
		Singleton<CharacterManager>.instance.changeCharacterSkin(targetCharacterSkinType, characterBoneSpriteRendererData);
		removeAllAnimations();
		List<AnimationClip> list = null;
		list = ((targetCharacterSkinType != CharacterSkinManager.PriestSkinType.Valkyrie2) ? Singleton<AnimationManager>.instance.priestNormalAnimationClipList : Singleton<AnimationManager>.instance.priestValkyrieAnimationClipList);
		m_idleAnimationClip = null;
		for (int i = 0; i < list.Count; i++)
		{
			characterBoneAnimation.AddClip(list[i], list[i].name);
			if (list[i].name.Equals("Idle"))
			{
				m_idleAnimationClip = list[i];
			}
		}
		characterBoneAnimation.clip = m_idleAnimationClip;
		characterBoneAnimation.Play();
		WeaponSkinData equippedWeaponSkinData = Singleton<WeaponSkinManager>.instance.getEquippedWeaponSkinData(CharacterManager.CharacterType.Priest);
		if (equippedWeaponSkinData == null)
		{
			weaponSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(Singleton<DataManager>.instance.currentGameData.priestEquippedWeapon);
		}
		else
		{
			bool flag = equippedWeaponSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || equippedWeaponSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || equippedWeaponSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
			weaponSpriteRenderer.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(equippedWeaponSkinData.currentPriestSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(equippedWeaponSkinData.currentPriestWeaponType));
		}
		cachedSpriteGroup.setAlpha(1f);
		cachedSpriteGroup.resetData();
		cachedSpriteGroup.setAlpha(0f);
	}

	public void initCharacterUIObject(CharacterSkinManager.ArcherSkinType targetCharacterSkinType, CanvasGroup targetFollowCanvasGroup, string layerName = "PopUpLayer", int sortingOrder = int.MinValue)
	{
		m_targetFollowCanvasGroup = targetFollowCanvasGroup;
		changeLayer(layerName, sortingOrder);
		StopAllCoroutines();
		currentCharacterType = CharacterManager.CharacterType.Archer;
		Singleton<CharacterManager>.instance.changeCharacterSkin(targetCharacterSkinType, characterBoneSpriteRendererData);
		removeAllAnimations();
		List<AnimationClip> list = null;
		list = ((targetCharacterSkinType != CharacterSkinManager.ArcherSkinType.Valkyrie3) ? Singleton<AnimationManager>.instance.archerNormalAnimationClipList : Singleton<AnimationManager>.instance.archerValkyrieAnimationClipList);
		m_idleAnimationClip = null;
		for (int i = 0; i < list.Count; i++)
		{
			characterBoneAnimation.AddClip(list[i], list[i].name);
			if (list[i].name.Equals("Idle"))
			{
				m_idleAnimationClip = list[i];
			}
		}
		characterBoneAnimation.clip = m_idleAnimationClip;
		characterBoneAnimation.Play();
		WeaponSkinData equippedWeaponSkinData = Singleton<WeaponSkinManager>.instance.getEquippedWeaponSkinData(CharacterManager.CharacterType.Archer);
		if (equippedWeaponSkinData == null)
		{
			weaponSpriteRenderer.sprite = Singleton<WeaponManager>.instance.getWeaponSprite(Singleton<DataManager>.instance.currentGameData.archerEquippedWeapon);
		}
		else
		{
			bool flag = equippedWeaponSkinData.currentWarriorSpecialWeaponSkinType != WeaponSkinManager.WarriorSpecialWeaponSkinType.None || equippedWeaponSkinData.currentPriestSpecialWeaponSkinType != WeaponSkinManager.PriestSpecialWeaponSkinType.None || equippedWeaponSkinData.currentArcherSpecialWeaponSkinType != WeaponSkinManager.ArcherSpecialWeaponSkinType.None;
			weaponSpriteRenderer.sprite = (flag ? Singleton<WeaponSkinManager>.instance.getSpecialWeaponSkinSprite(equippedWeaponSkinData.currentArcherSpecialWeaponSkinType) : Singleton<WeaponManager>.instance.getWeaponSprite(equippedWeaponSkinData.currentArcherWeaponType));
		}
		cachedSpriteGroup.setAlpha(1f);
		cachedSpriteGroup.resetData();
		cachedSpriteGroup.setAlpha(0f);
	}

	public void RefreshMaterial()
	{
		if (characterBoneSpriteRendererData.shieldSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.shieldSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (characterBoneSpriteRendererData.headSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.headSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (characterBoneSpriteRendererData.hairSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.hairSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (characterBoneSpriteRendererData.spineSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.spineSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (characterBoneSpriteRendererData.legSpriteRenderer != null && characterBoneSpriteRendererData.legSpriteRenderer.Length > 0)
		{
			for (int i = 0; i < characterBoneSpriteRendererData.legSpriteRenderer.Length; i++)
			{
				characterBoneSpriteRendererData.legSpriteRenderer[i].material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
			}
		}
		if (characterBoneSpriteRendererData.leftWingSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.leftWingSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (characterBoneSpriteRendererData.rightWingSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.rightWingSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (characterBoneSpriteRendererData.capeSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.capeSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (characterBoneSpriteRendererData.weaponSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.weaponSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (characterBoneSpriteRendererData.tailSpriteRenderer != null)
		{
			characterBoneSpriteRendererData.tailSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
	}

	private void Update()
	{
		if (UIWindowCharacterSkin.instance != null && m_targetFollowCanvasGroup != null && cachedSpriteGroup.alpha != m_targetFollowCanvasGroup.alpha)
		{
			cachedSpriteGroup.setAlpha(m_targetFollowCanvasGroup.alpha);
		}
	}
}
