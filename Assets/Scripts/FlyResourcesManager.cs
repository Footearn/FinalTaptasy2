using System;
using System.Collections;
using UnityEngine;

public class FlyResourcesManager : Singleton<FlyResourcesManager>
{
	public enum ResourceType
	{
		Gold,
		TreasurePiece,
		Ruby,
		TreasureEnchantStone,
		TranscendStone,
		HeartCoin,
		WeaponSkinPiece,
		WeaponSkinReinforcementMasterPiece
	}

	public Transform centerTransform;

	public Transform goldUITransform;

	public Transform rubyUITransform;

	public Transform treasureUITransform;

	public Transform transcendUITransform;

	public Transform heartCoinUITransform;

	public Transform heroTabUITransform;

	public Transform weaponSkinPieceUITransform;

	public Transform weaponSkinReinforcementMasterPieceUITransform;

	public Sprite goldSprite;

	public Sprite treasurePieceSprite;

	public Sprite treasureEnchantStoneSprite;

	public Sprite rubySprite;

	public Sprite transcendStoneSprite;

	public Sprite heartCoinSprite;

	public Sprite weaponSkinPieceSprite;

	public Sprite weaponSKinReinforcementMasterPieceSprite;

	public void playEffectResources(Vector2 startPosition, ResourceType resourceType, long spawnCount, float duration, Action arriveAction, bool isRotation = true)
	{
		Transform transform = null;
		float scale = 1f;
		switch (resourceType)
		{
		case ResourceType.Gold:
			transform = ((GameManager.currentDungeonType != GameManager.SpecialDungeonType.ElopeMode) ? goldUITransform : Singleton<ElopeModeManager>.instance.goldFlyTargetTransform);
			scale = 2f;
			break;
		case ResourceType.Ruby:
			transform = rubyUITransform;
			scale = 1.5f;
			break;
		case ResourceType.TreasurePiece:
			transform = ((GameManager.currentDungeonType != GameManager.SpecialDungeonType.ElopeMode) ? treasureUITransform : Singleton<ElopeModeManager>.instance.goldFlyTargetTransform);
			break;
		case ResourceType.TreasureEnchantStone:
			transform = treasureUITransform;
			break;
		case ResourceType.TranscendStone:
			transform = transcendUITransform;
			break;
		case ResourceType.HeartCoin:
			transform = heartCoinUITransform;
			break;
		case ResourceType.WeaponSkinPiece:
			scale = 2f;
			transform = ((!UIWindowWeaponSkin.instance.isOpen) ? heroTabUITransform : weaponSkinPieceUITransform);
			break;
		case ResourceType.WeaponSkinReinforcementMasterPiece:
			scale = 2f;
			transform = ((!UIWindowWeaponSkinInformation.instance.isOpen) ? heroTabUITransform : weaponSkinReinforcementMasterPieceUITransform);
			break;
		}
		StartCoroutine(resourceFlyUpdate(startPosition, transform.position, resourceType, spawnCount, duration, arriveAction, scale, isRotation));
	}

	public void playEffectResources(Transform startTransform, ResourceType resourceType, long spawnCount, float duration, Action arriveAction, bool isRotation = true)
	{
		Transform transform = null;
		float scale = 1f;
		switch (resourceType)
		{
		case ResourceType.Gold:
			transform = goldUITransform;
			scale = 2f;
			break;
		case ResourceType.Ruby:
			transform = rubyUITransform;
			scale = 1.5f;
			break;
		case ResourceType.TreasurePiece:
			transform = treasureUITransform;
			break;
		case ResourceType.TreasureEnchantStone:
			transform = treasureUITransform;
			break;
		case ResourceType.TranscendStone:
			transform = transcendUITransform;
			break;
		case ResourceType.HeartCoin:
			transform = heartCoinUITransform;
			break;
		case ResourceType.WeaponSkinPiece:
			scale = 2f;
			transform = ((!UIWindowWeaponSkin.instance.isOpen) ? heroTabUITransform : weaponSkinPieceUITransform);
			break;
		case ResourceType.WeaponSkinReinforcementMasterPiece:
			scale = 2f;
			transform = ((!UIWindowWeaponSkinInformation.instance.isOpen) ? heroTabUITransform : weaponSkinReinforcementMasterPieceUITransform);
			break;
		}
		StartCoroutine(resourceFlyUpdate(startTransform.position, transform.position, resourceType, spawnCount, duration, arriveAction, scale, isRotation));
	}

	public void playEffectResources(Transform startTransform, Transform targetTransform, ResourceType resourceType, long spawnCount, float duration, Action arriveAction, float scale, bool isRotation = true)
	{
		StartCoroutine(resourceFlyUpdate(startTransform.position, targetTransform.position, resourceType, spawnCount, duration, arriveAction, scale, isRotation));
	}

	private IEnumerator resourceFlyUpdate(Vector2 startPosition, Vector2 targetPosition, ResourceType resourceType, long spawnCount, float duration, Action arriveAction, float scale, bool isRotation)
	{
		if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
		{
			for (int i = 0; i < spawnCount; i++)
			{
				FlyingResourceObject flyResourceObject = ObjectPool.Spawn("@FlyingResource", Vector2.zero, centerTransform).GetComponent<FlyingResourceObject>();
				flyResourceObject.cachedTransform.position = startPosition;
				flyResourceObject.initResource(resourceType, startPosition, targetPosition, arriveAction, isRotation, scale);
				yield return new WaitForSeconds(duration);
			}
		}
	}
}
