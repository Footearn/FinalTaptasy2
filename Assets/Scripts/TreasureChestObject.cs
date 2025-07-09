using System.Collections;
using UnityEngine;

public class TreasureChestObject : MovingObject
{
	public float lifeTime;

	public SpriteGroup cachedSpriteGroup;

	public TreasureChestManager.TreasureType currentTreasureType;

	public SpriteRenderer treasureChestImage;

	public Animation treasureAnimation;

	private int m_currentTouchedCount;

	private float m_originYPos;

	private bool m_isDropedWeapon;

	private bool isRealBoss;

	private GameObject shinyEffect;

	private double goldDrop;

	public void initTreasure(double bronze, double silver, double gold, double transcend, double goblinTreasure, bool isRealBoss)
	{
		this.isRealBoss = isRealBoss;
		cachedImageTransform.localPosition = Vector3.zero;
		treasureChestImage.color = new Color(1f, 1f, 1f, 1f);
		m_isDropedWeapon = false;
		treasureAnimation.Stop();
		treasureAnimation.Play("TreasureChestIdle");
		m_currentTouchedCount = 0;
		double num = Random.Range(0, 100000);
		num /= 1000.0;
		cachedSpriteGroup.setAlpha(1f);
		double num2 = 0.0;
		double num3 = 0.0;
		double[] array = new double[6]
		{
			bronze,
			silver,
			gold,
			0.0,
			transcend,
			goblinTreasure
		};
		m_originYPos = base.cachedTransform.position.y;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == 0.0)
			{
				continue;
			}
			if (i == 0)
			{
				num3 = array[i];
				if (num2 < num && num <= num3)
				{
					currentTreasureType = (TreasureChestManager.TreasureType)i;
					break;
				}
				num2 = array[i];
			}
			else
			{
				num3 = num2 + array[i];
				if (num2 < num && num <= num3)
				{
					currentTreasureType = (TreasureChestManager.TreasureType)i;
					break;
				}
				num2 = num3;
			}
		}
		if (!isRealBoss && currentTreasureType == TreasureChestManager.TreasureType.GoldTreasure)
		{
			currentTreasureType = TreasureChestManager.TreasureType.PlatinumTreasure;
		}
		if (GameManager.currentTheme > 100)
		{
			goldDrop = CalculateManager.getCurrentStandardGold() * 25.0 / (double)(Singleton<MapManager>.instance.getMaxFloor(GameManager.currentTheme) * 5 + 50) * 3.0;
		}
		else
		{
			goldDrop = CalculateManager.getCurrentStandardGold() * 25.0 / (double)(Singleton<MapManager>.instance.getMaxFloor(GameManager.currentTheme) * 5) * 5.0;
		}
		switch (currentTreasureType)
		{
		case TreasureChestManager.TreasureType.BronzeTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.bronzeTreasureSprite;
			base.cachedTransform.localScale = Vector2.one;
			break;
		case TreasureChestManager.TreasureType.SilverTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.silverTreasureSprite;
			base.cachedTransform.localScale = Vector2.one;
			goldDrop *= 2.0;
			break;
		case TreasureChestManager.TreasureType.GoldTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.goldTreasureSprite;
			base.cachedTransform.localScale = Vector2.one * 1.5f;
			shinyEffect = ObjectPool.Spawn("fx_boss_chest_shiny", new Vector2(0f, 0.5f), base.cachedTransform);
			goldDrop *= 2.0;
			StopCoroutine("waitForOpenBossTreasureChest");
			StartCoroutine("waitForOpenBossTreasureChest");
			break;
		case TreasureChestManager.TreasureType.PlatinumTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.platinumTreasureSprite;
			base.cachedTransform.localScale = Vector2.one * 1.5f;
			shinyEffect = ObjectPool.Spawn("fx_boss_chest_shiny", new Vector2(0f, 0.5f), base.cachedTransform);
			goldDrop *= 2.0;
			Singleton<TreasureChestManager>.instance.currentBossTreasureChest = this;
			StopCoroutine("waitForOpenBossTreasureChest");
			StartCoroutine("waitForOpenBossTreasureChest");
			break;
		case TreasureChestManager.TreasureType.TranscendTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.transcendTreasureSprite;
			base.cachedTransform.localScale = Vector2.one;
			goldDrop *= 2.0;
			break;
		case TreasureChestManager.TreasureType.GoblinTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.goblinTreasureSprite;
			base.cachedTransform.localScale = Vector2.one;
			goldDrop *= 2.0;
			break;
		}
		jump(new Vector2(0f, 2.5f), m_originYPos, delegate
		{
			if (currentTreasureType == TreasureChestManager.TreasureType.GoblinTreasure)
			{
				ObjectPool.Spawn("@GoblinChestEffect", base.cachedTransform.position + new Vector3(0f, 0.3f, 0f), new Vector3(-90f, 0f, 0f));
			}
			Singleton<TreasureChestManager>.instance.currentTreasureObjects.Add(this);
			StopCoroutine("dissapearUpdate");
			if (currentTreasureType <= TreasureChestManager.TreasureType.SilverTreasure || currentTreasureType == TreasureChestManager.TreasureType.TranscendTreasure || currentTreasureType == TreasureChestManager.TreasureType.GoblinTreasure)
			{
				StartCoroutine("dissapearUpdate");
			}
		});
	}

	private IEnumerator waitForOpenBossTreasureChest()
	{
		yield return new WaitForSeconds(0.5f);
		openBossTreasureChest();
		for (int i = 0; i < Singleton<TreasureChestManager>.instance.currentTreasureObjects.Count; i++)
		{
			Singleton<TreasureChestManager>.instance.currentTreasureObjects[i].touchTreasure();
		}
		Singleton<DropItemManager>.instance.startAutoCatchAllItems();
	}

	public void openBossTreasureChest()
	{
		if (m_currentTouchedCount > 0)
		{
			return;
		}
		Singleton<AudioManager>.instance.playEffectSound("treasurechest_open");
		switch (currentTreasureType)
		{
		case TreasureChestManager.TreasureType.GoldTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.goldTreasureOpenSprite;
			break;
		case TreasureChestManager.TreasureType.PlatinumTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.platinumTreasureOpenSprite;
			break;
		}
		for (int i = 0; i < Singleton<TreasureChestManager>.instance.maxDropItemCount; i++)
		{
			switch (Singleton<TreasureChestManager>.instance.getDropItemType(TreasureChestManager.TreasureType.SilverTreasure))
			{
			case TreasureChestManager.DropItemType.Ruby:
				Singleton<RubyManager>.instance.spawnRuby(base.cachedTransform.position, 1L);
				break;
			case TreasureChestManager.DropItemType.ElopeResource:
				Singleton<ElopeModeManager>.instance.spawnElopeRecource(base.cachedTransform.position, 1L);
				break;
			}
		}
		if (GameManager.currentTheme > 100)
		{
			goldDrop = CalculateManager.getCurrentStandardGold() * 25.0 / (double)(Singleton<MapManager>.instance.getMaxFloor(GameManager.currentTheme) * 5 + 50) * 3.0;
		}
		else
		{
			goldDrop = CalculateManager.getCurrentStandardGold() * 25.0 / (double)(Singleton<MapManager>.instance.getMaxFloor(GameManager.currentTheme) * 5) * 5.0;
		}
		int num = ((!Singleton<DataManager>.instance.currentGameData.isLowEndDevice) ? 25 : 5);
		for (int j = 0; j < num; j++)
		{
			Singleton<GoldManager>.instance.spawnGold(base.cachedTransform.position, goldDrop / (double)num);
		}
		treasureChestImage.color = new Color(1f, 1f, 1f, 0f);
		Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.FindingTreasures, 1.0);
		Singleton<QuestManager>.instance.questReport(QuestManager.QuestType.TotalTreasureBox, 1.0);
		StartCoroutine("waitForResult");
		m_currentTouchedCount++;
	}

	private IEnumerator waitForResult()
	{
		yield return new WaitForSeconds(1.5f / GameManager.timeScale);
		Singleton<DropItemManager>.instance.startAutoCatchAllItems();
		Singleton<CachedManager>.instance.uiWindowResult.ResultGame(true, isRealBoss);
		yield return new WaitForSeconds(1f / GameManager.timeScale);
		if (currentTreasureType == TreasureChestManager.TreasureType.GoldTreasure)
		{
			recycleBossTreasureChest();
		}
	}

	public void touchTreasure()
	{
		if (m_currentTouchedCount <= 0 && currentTreasureType != TreasureChestManager.TreasureType.GoldTreasure && currentTreasureType != TreasureChestManager.TreasureType.PlatinumTreasure)
		{
			Singleton<AudioManager>.instance.playEffectSound("treasurechest_open");
			treasureAnimation.Stop();
			treasureAnimation.Play();
			StopCoroutine("openChest");
			StartCoroutine("openChest");
			m_currentTouchedCount++;
			if (currentTreasureType != TreasureChestManager.TreasureType.GoblinTreasure)
			{
				dropItems();
				Singleton<AchievementManager>.instance.increaseAchievementValue(AchievementManager.AchievementType.FindingTreasures, 1.0);
				Singleton<QuestManager>.instance.questReport(QuestManager.QuestType.TotalTreasureBox, 1.0);
			}
		}
	}

	private IEnumerator openChest()
	{
		switch (currentTreasureType)
		{
		case TreasureChestManager.TreasureType.BronzeTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.bronzeTreasureOpenSprite;
			break;
		case TreasureChestManager.TreasureType.SilverTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.silverTreasureOpenSprite;
			break;
		case TreasureChestManager.TreasureType.GoldTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.goldTreasureOpenSprite;
			break;
		case TreasureChestManager.TreasureType.PlatinumTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.platinumTreasureOpenSprite;
			break;
		case TreasureChestManager.TreasureType.GoblinTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.goblinTreasureOpenSprite;
			break;
		}
		yield return new WaitForSeconds(0.2f);
		switch (currentTreasureType)
		{
		case TreasureChestManager.TreasureType.BronzeTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.bronzeTreasureSprite;
			break;
		case TreasureChestManager.TreasureType.SilverTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.silverTreasureSprite;
			break;
		case TreasureChestManager.TreasureType.GoldTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.goldTreasureSprite;
			break;
		case TreasureChestManager.TreasureType.PlatinumTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.platinumTreasureSprite;
			break;
		case TreasureChestManager.TreasureType.GoblinTreasure:
			treasureChestImage.sprite = Singleton<TreasureChestManager>.instance.goblinTreasureSprite;
			break;
		}
		recycleTreasure();
	}

	private IEnumerator dissapearUpdate()
	{
		float timer = 0f;
		bool disappear = false;
		while (true)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				if (timer >= GoldManager.goldDissapearTimer * 0.7f)
				{
					disappear = true;
				}
				if (disappear)
				{
					if (Singleton<TreasureChestManager>.instance.isAutoOpenTreasureChest)
					{
						touchTreasure();
						yield break;
					}
					cachedSpriteGroup.setAlpha((lifeTime - timer) / (lifeTime * 0.3f));
				}
				if (timer >= lifeTime)
				{
					break;
				}
			}
			yield return null;
		}
		recycleTreasure(true);
	}

	public void recycleBossTreasureChest()
	{
		ObjectPool.Recycle("fx_boss_chest_shiny", shinyEffect);
		recycleTreasure();
	}

	private void recycleTreasure(bool isDisappearByLifeTime = false)
	{
		m_currentTouchedCount = 0;
		Singleton<TreasureChestManager>.instance.currentTreasureObjects.Remove(this);
		if (!isDisappearByLifeTime)
		{
			Singleton<TreasureChestManager>.instance.treasureRecycleEvent();
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}

	private void dropItems()
	{
		if (currentTreasureType == TreasureChestManager.TreasureType.GoblinTreasure)
		{
			return;
		}
		int maxDropItemCount = Singleton<TreasureChestManager>.instance.maxDropItemCount;
		maxDropItemCount *= ((currentTreasureType != TreasureChestManager.TreasureType.TranscendTreasure) ? 1 : 2);
		for (int i = 0; i < maxDropItemCount; i++)
		{
			switch (Singleton<TreasureChestManager>.instance.getDropItemType(currentTreasureType))
			{
			case TreasureChestManager.DropItemType.Gold:
				Singleton<GoldManager>.instance.spawnGold(base.cachedTransform.position, goldDrop / (double)Singleton<TreasureChestManager>.instance.maxDropItemCount);
				break;
			case TreasureChestManager.DropItemType.Ruby:
				Singleton<RubyManager>.instance.spawnRuby(base.cachedTransform.position, 1L);
				break;
			case TreasureChestManager.DropItemType.TreasurePiece:
				Singleton<TreasureManager>.instance.spawnTreasurePiece(base.cachedTransform.position);
				break;
			case TreasureChestManager.DropItemType.TranscendStone:
				Singleton<TranscendManager>.instance.spawnTranscendStone(base.cachedTransform.position, 1L);
				break;
			case TreasureChestManager.DropItemType.ElopeResource:
				Singleton<ElopeModeManager>.instance.spawnElopeRecource(base.cachedTransform.position, 1L);
				break;
			}
		}
	}
}
