using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlyingResourceObject : MovingObject
{
	public Image resourceImage;

	public Transform imageTransform;

	private Vector3 m_targetPosition;

	private Action m_arriveAction;

	public void initResource(FlyResourcesManager.ResourceType resourceType, Vector2 startPosition, Vector2 targetPosition, Action arriveAction, bool isRotation, float scale)
	{
		imageTransform.localScale = Vector3.one * scale;
		base.cachedTransform.localScale = Vector3.one;
		imageTransform.localEulerAngles = Vector3.zero;
		switch (resourceType)
		{
		case FlyResourcesManager.ResourceType.Gold:
			resourceImage.sprite = Singleton<FlyResourcesManager>.instance.goldSprite;
			break;
		case FlyResourcesManager.ResourceType.Ruby:
			resourceImage.sprite = Singleton<FlyResourcesManager>.instance.rubySprite;
			break;
		case FlyResourcesManager.ResourceType.TreasurePiece:
			resourceImage.sprite = Singleton<FlyResourcesManager>.instance.treasurePieceSprite;
			break;
		case FlyResourcesManager.ResourceType.TreasureEnchantStone:
			resourceImage.sprite = Singleton<FlyResourcesManager>.instance.treasureEnchantStoneSprite;
			break;
		case FlyResourcesManager.ResourceType.TranscendStone:
			resourceImage.sprite = Singleton<FlyResourcesManager>.instance.transcendStoneSprite;
			break;
		case FlyResourcesManager.ResourceType.HeartCoin:
			resourceImage.sprite = Singleton<FlyResourcesManager>.instance.heartCoinSprite;
			break;
		case FlyResourcesManager.ResourceType.WeaponSkinPiece:
			resourceImage.sprite = Singleton<FlyResourcesManager>.instance.weaponSkinPieceSprite;
			break;
		case FlyResourcesManager.ResourceType.WeaponSkinReinforcementMasterPiece:
			resourceImage.sprite = Singleton<FlyResourcesManager>.instance.weaponSKinReinforcementMasterPieceSprite;
			break;
		}
		resourceImage.SetNativeSize();
		m_arriveAction = arriveAction;
		if (isRotation)
		{
			StartCoroutine("rotateUpdate");
		}
		m_targetPosition = targetPosition;
		StartCoroutine("initMoveUpdate", startPosition);
	}

	private IEnumerator initMoveUpdate(Vector2 originStartPosition)
	{
		Vector2 targetPosition = new Vector2(originStartPosition.x + UnityEngine.Random.Range(-1.5f, 1.5f), originStartPosition.y + UnityEngine.Random.Range(-0.9f, 0.9f));
		float speed = UnityEngine.Random.Range(1f, 3f);
		float timer = 0f;
		float maxTime = UnityEngine.Random.Range(0.5f, 0.8f);
		while (true)
		{
			Vector2 position2 = base.cachedTransform.position;
			position2 = Vector2.Lerp(position2, targetPosition, Time.deltaTime * speed);
			base.cachedTransform.position = position2;
			timer += Time.deltaTime;
			if (timer >= maxTime)
			{
				break;
			}
			yield return null;
		}
		StartCoroutine("flyUpdate");
	}

	private IEnumerator rotateUpdate()
	{
		float timer = 0f;
		while (true)
		{
			Vector3 rotation = imageTransform.localEulerAngles;
			rotation.y += Time.deltaTime * 720f;
			imageTransform.localEulerAngles = rotation;
			timer += Time.deltaTime;
			if (timer >= 0.5f)
			{
				break;
			}
			yield return null;
		}
		imageTransform.localEulerAngles = Vector3.zero;
	}

	private IEnumerator flyUpdate()
	{
		float acceleration = 0f;
		float multiflyAcceleraiotn = UnityEngine.Random.Range(20f, 35f);
		while (true)
		{
			acceleration += Time.deltaTime * multiflyAcceleraiotn;
			Vector2 position2 = base.cachedTransform.position;
			position2 = Vector2.Lerp(position2, m_targetPosition, Time.deltaTime * acceleration);
			base.cachedTransform.position = position2;
			if (Vector2.Distance(position2, m_targetPosition) <= 0.2f)
			{
				break;
			}
			yield return null;
		}
		if (m_arriveAction != null)
		{
			m_arriveAction();
		}
		m_arriveAction = null;
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
