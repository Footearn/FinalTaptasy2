using System.Collections.Generic;
using UnityEngine;

public class ColleagueUIObject : ObjectBase
{
	public ColleagueManager.ColleagueType currentColleagueType;

	public int currnetSkinIndex;

	public BoneAnimationNameData currentBoneAnimationName;

	public CharacterManager.CharacterBoneAnimationSpriteRendererData colleagueBoneSpriteRendererData;

	public Animation characterBoneAnimation;

	public SpriteGroup cachedSpriteGroup;

	private ColleagueInventoryData m_currentColleagueInventoryData;

	public List<SpriteRenderer> cachedSpriteRendererList;

	private CanvasGroup m_targetCanvasGroup;

	private void Awake()
	{
		cachedSpriteRendererList = new List<SpriteRenderer>();
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.capeSpriteRenderer);
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.hairSpriteRenderer);
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.headSpriteRenderer);
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.leftWingSpriteRenderer);
		for (int i = 0; i < colleagueBoneSpriteRendererData.legSpriteRenderer.Length; i++)
		{
			cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.legSpriteRenderer[i]);
		}
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.rightWingSpriteRenderer);
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.shieldSpriteRenderer);
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.spineSpriteRenderer);
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.tailSpriteRenderer);
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.weaponSpriteRenderer);
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.leftHandSpriteRenderer);
		cachedSpriteRendererList.Add(colleagueBoneSpriteRendererData.rightHandSpriteRenderer);
	}

	public void initColleagueUI(ColleagueManager.ColleagueType colleagueType, int skinIndex)
	{
		refreshMaterial();
		currentColleagueType = colleagueType;
		currnetSkinIndex = skinIndex;
		cachedSpriteGroup.setAlpha(1f);
		characterBoneAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
		m_currentColleagueInventoryData = Singleton<ColleagueManager>.instance.getColleagueInventoryData(currentColleagueType);
		Singleton<ColleagueManager>.instance.changeColleagueSkin(currentColleagueType, currnetSkinIndex, colleagueBoneSpriteRendererData);
	}

	public void refreshMaterial()
	{
		if (colleagueBoneSpriteRendererData.shieldSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.shieldSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.headSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.headSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.hairSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.hairSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.spineSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.spineSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.legSpriteRenderer != null && colleagueBoneSpriteRendererData.legSpriteRenderer.Length > 0)
		{
			for (int i = 0; i < colleagueBoneSpriteRendererData.legSpriteRenderer.Length; i++)
			{
				colleagueBoneSpriteRendererData.legSpriteRenderer[i].material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
			}
		}
		if (colleagueBoneSpriteRendererData.leftWingSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.leftWingSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.rightWingSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.rightWingSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.capeSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.capeSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.weaponSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.weaponSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.tailSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.tailSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.leftHandSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.leftHandSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.rightHandSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.rightHandSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.leftLegSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.leftLegSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.rightLegSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.rightLegSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.leftFingerSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.leftFingerSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
		if (colleagueBoneSpriteRendererData.rightFingerSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.rightFingerSpriteRenderer.material = Singleton<ResourcesManager>.instance.spriteDefaultMaterial;
		}
	}

	public void changeLayer(string layerName)
	{
		if (colleagueBoneSpriteRendererData.shieldSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.shieldSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.headSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.headSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.hairSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.hairSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.spineSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.spineSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.legSpriteRenderer != null && colleagueBoneSpriteRendererData.legSpriteRenderer.Length > 0)
		{
			for (int i = 0; i < colleagueBoneSpriteRendererData.legSpriteRenderer.Length; i++)
			{
				colleagueBoneSpriteRendererData.legSpriteRenderer[i].sortingLayerName = layerName;
			}
		}
		if (colleagueBoneSpriteRendererData.leftWingSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.leftWingSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.rightWingSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.rightWingSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.capeSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.capeSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.weaponSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.weaponSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.tailSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.tailSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.leftHandSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.leftHandSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.rightHandSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.rightHandSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.leftLegSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.leftLegSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.rightLegSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.rightLegSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.leftFingerSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.leftFingerSpriteRenderer.sortingLayerName = layerName;
		}
		if (colleagueBoneSpriteRendererData.rightFingerSpriteRenderer != null)
		{
			colleagueBoneSpriteRendererData.rightFingerSpriteRenderer.sortingLayerName = layerName;
		}
	}

	public void followAlphaWithCanvas(CanvasGroup targetCanvas)
	{
		m_targetCanvasGroup = targetCanvas;
	}

	private void Update()
	{
		if (m_targetCanvasGroup != null && cachedSpriteGroup.alpha != m_targetCanvasGroup.alpha)
		{
			cachedSpriteGroup.setAlpha(m_targetCanvasGroup.alpha);
		}
	}
}
