using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MovingObject
{
	public DropItemManager.DropItemType currentDropItemType;

	protected float m_yOffset = 0.2f;

	private double m_increaseValue;

	private bool m_isDropping;

	public bool isFlying;

	public virtual void initDropObject(double value, DropItemManager.DropItemType itemType)
	{
		switch (itemType)
		{
		case DropItemManager.DropItemType.TranscendStone:
			m_yOffset = 0.3f;
			break;
		case DropItemManager.DropItemType.CollectEventResource:
			m_yOffset = 0.3f;
			break;
		default:
			m_yOffset = 0.2f;
			break;
		}
		currentDropItemType = itemType;
		base.cachedTransform.localScale = Vector2.one;
		stopAll();
		isFlying = false;
		m_isDropping = true;
		m_increaseValue = value;
		Vector2 velocity = new Vector2(UnityEngine.Random.Range(-1.4f, 1.4f), UnityEngine.Random.Range(5.5f, 8.5f));
		Action action = delegate
		{
			m_isDropping = false;
			Singleton<DropItemManager>.instance.currentDropItemList.Add(this);
			StopAllCoroutines();
			StartCoroutine("dropUpdate");
		};
		if (GameManager.currentDungeonType != GameManager.SpecialDungeonType.TowerMode)
		{
			jump(velocity, Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position, velocity.x).y + m_yOffset, action);
			return;
		}
		base.cachedTransform.position = new Vector3(base.cachedTransform.position.x, Singleton<GroundManager>.instance.getHitGroundPosition(base.cachedTransform.position, velocity.x).y + m_yOffset);
		action();
	}

	public virtual void catchDropObject()
	{
		Transform transform = Singleton<GoldManager>.instance.goldUITransform;
		switch (currentDropItemType)
		{
		case DropItemManager.DropItemType.Gold:
			transform = ((GameManager.currentDungeonType != GameManager.SpecialDungeonType.ElopeMode) ? Singleton<GoldManager>.instance.goldUITransform : Singleton<ElopeModeManager>.instance.goldFlyTargetTransform);
			break;
		case DropItemManager.DropItemType.Ruby:
			transform = ((GameManager.currentDungeonType != GameManager.SpecialDungeonType.ElopeMode) ? Singleton<RubyManager>.instance.rubyUITransform : Singleton<ElopeModeManager>.instance.rubyFlyTargetTransform);
			break;
		case DropItemManager.DropItemType.TreasureKey:
			transform = Singleton<GoldManager>.instance.goldUITransform;
			break;
		case DropItemManager.DropItemType.TranscendStone:
			transform = Singleton<GoldManager>.instance.goldUITransform;
			break;
		case DropItemManager.DropItemType.CollectEventResource:
			transform = Singleton<CollectEventManager>.instance.collectEventFlyTargetTransform;
			break;
		case DropItemManager.DropItemType.HeartCoin:
			if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.TowerMode)
			{
				ObjectPool.Spawn("@HeartCoinObtainEffect", base.cachedTransform.position + new Vector3(0f, 0.4f, 0f), new Vector3(-90f, 0f, 0f));
				Singleton<AudioManager>.instance.playEffectSound("getgold", AudioManager.EffectType.Resource);
				transform = UIWindowTowerMode.instance.heartCoinFlyTargetTransform;
			}
			else
			{
				transform = Singleton<ElopeModeManager>.instance.heartCoinFlyTargetTransform;
			}
			break;
		case DropItemManager.DropItemType.WeaponSkinPiece:
			transform = Singleton<GoldManager>.instance.goldUITransform;
			break;
		}
		if (transform != null && !isFlying)
		{
			StopAllCoroutines();
			isFlying = true;
			moveTo(transform, 10.5f, delegate
			{
				obtainEvent();
			});
		}
	}

	public virtual void obtainEvent()
	{
		Singleton<AudioManager>.instance.playEffectSound("getgold", AudioManager.EffectType.Resource);
		StopAllCoroutines();
		if (currentDropItemType != DropItemManager.DropItemType.Ruby)
		{
			Dictionary<DropItemManager.DropItemType, double> totalIngameGainDropItem;
			Dictionary<DropItemManager.DropItemType, double> dictionary = (totalIngameGainDropItem = Singleton<DropItemManager>.instance.totalIngameGainDropItem);
			DropItemManager.DropItemType key;
			DropItemManager.DropItemType key2 = (key = currentDropItemType);
			double num = totalIngameGainDropItem[key];
			dictionary[key2] = num + m_increaseValue;
		}
		bool isOpen = UIWindowResult.instance.isOpen;
		switch (currentDropItemType)
		{
		case DropItemManager.DropItemType.Gold:
			Singleton<GoldManager>.instance.increaseGoldByMonster(m_increaseValue);
			break;
		case DropItemManager.DropItemType.Ruby:
			Singleton<RubyManager>.instance.increaseRuby((long)m_increaseValue);
			break;
		case DropItemManager.DropItemType.TreasureKey:
			Singleton<TreasureManager>.instance.increaseTreasurePiece((long)m_increaseValue);
			break;
		case DropItemManager.DropItemType.TranscendStone:
			Singleton<TranscendManager>.instance.increaseTranscendStone((long)m_increaseValue);
			Singleton<TranscendManager>.instance.ingameTotalTranscendStone += (long)m_increaseValue;
			break;
		case DropItemManager.DropItemType.CollectEventResource:
			Singleton<CollectEventManager>.instance.increaseCollectEventResource((long)m_increaseValue);
			break;
		case DropItemManager.DropItemType.ElopeResource1:
		case DropItemManager.DropItemType.ElopeResource2:
		case DropItemManager.DropItemType.ElopeResource3:
		case DropItemManager.DropItemType.ElopeResource4:
			if (isOpen)
			{
				for (int i = 0; i < UIWindowResult.instance.elopeResourcesTexts.Length; i++)
				{
					long num2 = 0L;
					switch (i)
					{
					case 0:
						num2 = (long)Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.ElopeResource1];
						break;
					case 1:
						num2 = (long)Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.ElopeResource2];
						break;
					case 2:
						num2 = (long)Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.ElopeResource3];
						break;
					case 3:
						num2 = (long)Singleton<DropItemManager>.instance.totalIngameGainDropItem[DropItemManager.DropItemType.ElopeResource4];
						break;
					}
					UIWindowResult.instance.elopeResourcesTexts[i].text = num2.ToString("N0");
				}
			}
			Singleton<ElopeModeManager>.instance.increaseResource(currentDropItemType, (long)m_increaseValue);
			break;
		case DropItemManager.DropItemType.HeartCoin:
			Singleton<AudioManager>.instance.playEffectSound("earn_heart_coin");
			Singleton<ElopeModeManager>.instance.displayHeartCoin();
			if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.TowerMode)
			{
				UIWindowTowerMode.instance.refreshHeartCoinText();
			}
			break;
		case DropItemManager.DropItemType.WeaponSkinPiece:
			Singleton<WeaponSkinManager>.instance.increaseWeaponSkinPiece((long)m_increaseValue);
			break;
		}
		m_increaseValue = 0.0;
		stopAll();
		recycle();
	}

	public virtual IEnumerator dropUpdate()
	{
		float baseYPos = base.cachedTransform.position.y;
		bool switchMove = false;
		float switchTimer = 0f;
		float disappearTimer = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				disappearTimer += Time.deltaTime * GameManager.timeScale;
				switchTimer += Time.deltaTime * GameManager.timeScale;
				Vector3 position = base.cachedTransform.position;
				position.y = Mathf.Lerp(position.y, baseYPos + (switchMove ? 0.05f : (-0.05f)), Time.deltaTime * GameManager.timeScale * 2f);
				if (switchTimer >= 0.5f)
				{
					switchTimer = 0f;
					switchMove = !switchMove;
				}
				if (disappearTimer >= DropItemManager.dropItemDissapearTime)
				{
					break;
				}
			}
			yield return null;
		}
		catchDropObject();
	}

	protected virtual void recycle()
	{
		Singleton<DropItemManager>.instance.currentDropItemList.Remove(this);
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
