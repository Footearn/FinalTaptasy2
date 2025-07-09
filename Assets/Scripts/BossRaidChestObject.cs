using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRaidChestObject : MovingObject
{
	public bool isFlying;

	public SpriteRenderer chestRenderer;

	private float m_yOffset;

	private bool m_isDropping;

	private BossRaidManager.BossRaidChestData m_currentChestData;

	public ParticleSystem[] chestParticles;

	private Transform m_targetTransform;

	public void spawnBossRaidChest(BossRaidManager.BossRaidChestData chestData)
	{
		setDontStop(false);
		for (int i = 0; i < chestParticles.Length; i++)
		{
			chestParticles[i].loop = true;
		}
		base.cachedTransform.localScale = Vector3.one;
		switch (chestData.chestType)
		{
		case BossRaidManager.BossRaidChestType.Bronze:
			chestRenderer.sprite = Singleton<BossRaidManager>.instance.bronzeChestSprite;
			break;
		case BossRaidManager.BossRaidChestType.Gold:
			chestRenderer.sprite = Singleton<BossRaidManager>.instance.goldChestSprite;
			break;
		case BossRaidManager.BossRaidChestType.Dia:
			chestRenderer.sprite = Singleton<BossRaidManager>.instance.diaChestSprite;
			break;
		}
		m_currentChestData = chestData;
		base.cachedTransform.localScale = Vector2.one;
		stopAll();
		isFlying = false;
		m_isDropping = true;
		chestRenderer.color = new Color(1f, 1f, 1f, 1f);
		switch (m_currentChestData.chestType)
		{
		case BossRaidManager.BossRaidChestType.Bronze:
			m_targetTransform = Singleton<BossRaidManager>.instance.bronzeChestIcon;
			break;
		case BossRaidManager.BossRaidChestType.Gold:
			m_targetTransform = Singleton<BossRaidManager>.instance.goldChestIcon;
			break;
		case BossRaidManager.BossRaidChestType.Dia:
			m_targetTransform = Singleton<BossRaidManager>.instance.diaChestIcon;
			break;
		}
		Vector2 vector = new Vector2(0f, 14f);
		m_yOffset = 0f;
		StartCoroutine("neverMoveUpdate");
	}

	private IEnumerator neverMoveUpdate()
	{
		float timer = 0f;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (!m_isDropping)
				{
					yield break;
				}
				if (timer >= 3f)
				{
					break;
				}
			}
			yield return null;
		}
		m_isDropping = false;
		switch (m_currentChestData.chestType)
		{
		case BossRaidManager.BossRaidChestType.Bronze:
			Singleton<BossRaidManager>.instance.currentBronzeChestList.Add(this);
			break;
		case BossRaidManager.BossRaidChestType.Gold:
			Singleton<BossRaidManager>.instance.currentGoldChestList.Add(this);
			break;
		case BossRaidManager.BossRaidChestType.Dia:
			Singleton<BossRaidManager>.instance.currentDiaChestList.Add(this);
			break;
		}
		StopAllCoroutines();
		StartCoroutine("chestUpdate");
	}

	private IEnumerator chestUpdate()
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
				if (switchTimer >= 0.5f)
				{
					switchTimer = 0f;
					switchMove = !switchMove;
				}
				if (disappearTimer >= Singleton<BossRaidManager>.instance.chestDissappearTime)
				{
					break;
				}
			}
			yield return null;
		}
		catchChest();
	}

	public void catchChest()
	{
		if (!isFlying)
		{
			StopAllCoroutines();
			isFlying = true;
			RectTransform targetTransform = null;
			switch (m_currentChestData.chestType)
			{
			case BossRaidManager.BossRaidChestType.Bronze:
				targetTransform = Singleton<BossRaidManager>.instance.bronzeChestIcon;
				break;
			case BossRaidManager.BossRaidChestType.Gold:
				targetTransform = Singleton<BossRaidManager>.instance.goldChestIcon;
				break;
			case BossRaidManager.BossRaidChestType.Dia:
				targetTransform = Singleton<BossRaidManager>.instance.diaChestIcon;
				break;
			}
			moveTo(targetTransform, 10f, delegate
			{
				obtainEvent();
			});
		}
	}

	public void obtainEvent()
	{
		AnalyzeManager.retention(AnalyzeManager.CategoryType.BossRaid, AnalyzeManager.ActionType.GetBossRaidChest, new Dictionary<string, string>
		{
			{
				"ChestType",
				m_currentChestData.chestType.ToString()
			}
		});
		Singleton<BossRaidManager>.instance.increaseChest(m_currentChestData);
		for (int i = 0; i < chestParticles.Length; i++)
		{
			chestParticles[i].loop = false;
			chestParticles[i].Stop();
		}
		m_currentChestData = default(BossRaidManager.BossRaidChestData);
		Singleton<AudioManager>.instance.playEffectSound("monster_blowup");
		StopAllCoroutines();
		stopAll();
		base.cachedTransform.localScale = Vector3.zero;
		StartCoroutine("waitForRecycle");
	}

	private IEnumerator waitForRecycle()
	{
		setDontStop(true);
		float timer = 0f;
		while (true)
		{
			base.cachedTransform.position = m_targetTransform.position + new Vector3(0f, -0.2f, 0f);
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= 1.5f)
				{
					break;
				}
			}
			yield return null;
		}
		recycle();
	}

	private void recycle()
	{
		switch (m_currentChestData.chestType)
		{
		case BossRaidManager.BossRaidChestType.Bronze:
			Singleton<BossRaidManager>.instance.currentBronzeChestList.Remove(this);
			break;
		case BossRaidManager.BossRaidChestType.Gold:
			Singleton<BossRaidManager>.instance.currentGoldChestList.Remove(this);
			break;
		case BossRaidManager.BossRaidChestType.Dia:
			Singleton<BossRaidManager>.instance.currentDiaChestList.Remove(this);
			break;
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}
