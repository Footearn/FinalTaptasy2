using System.Collections;
using UnityEngine;

public class ColleagueObjectGoblin : ColleagueObject
{
	public override void initColleague()
	{
		StopCoroutine("goblinCreatTreasureChestUpdate");
		base.initColleague();
		StartCoroutine("goblinCreatTreasureChestUpdate");
	}

	protected override void attackInit()
	{
		StopCoroutine("attackEndCheckUpdate");
		StartCoroutine("attackEndCheckUpdate");
		m_attackLimitedMaxTimer = 0f;
		playBoneAnimation(currentBoneAnimationName.attackName[0]);
	}

	public override void attackEnemy()
	{
		CharacterWarrior warriorCharacter = Singleton<CharacterManager>.instance.warriorCharacter;
		Vector2 zero = Vector2.zero;
		if (!warriorCharacter.currentGround.isFirstGround)
		{
			Direction direction = MovingObject.calculateDirection(warriorCharacter.currentGround.inPoint.position, warriorCharacter.currentGround.outPoint.position);
			if (direction == Direction.Right)
			{
				zero.x = Random.Range(zero.x, warriorCharacter.currentGround.outPoint.position.x);
			}
			else
			{
				zero.x = Random.Range(warriorCharacter.currentGround.outPoint.position.x, zero.x);
			}
			zero.y = warriorCharacter.currentGround.cachedTransform.position.y + 0.177f;
		}
		else
		{
			zero.x = Random.Range(warriorCharacter.cachedTransform.position.x, warriorCharacter.currentGround.downPoint.position.x);
			zero.y = warriorCharacter.currentGround.cachedTransform.position.y + 0.5024f;
		}
		Singleton<TreasureChestManager>.instance.spawnTreasure(zero, 0.0, 0.0, 0.0, 0.0, 100.0, false);
	}

	private IEnumerator goblinCreatTreasureChestUpdate()
	{
		float timer = 2.4f;
		CharacterWarrior warrior = Singleton<CharacterManager>.instance.warriorCharacter;
		while (true)
		{
			if (!GameManager.isPause && GameManager.currentGameState == GameManager.GameState.Playing && warrior.getState() != PublicDataManager.State.Wait && warrior.getState() != PublicDataManager.State.Die)
			{
				timer += Time.deltaTime * GameManager.timeScale;
				while (timer >= 2.9f)
				{
					timer -= 2.9f;
					if (currentGround.isFirstGround)
					{
						attackEnemy();
					}
					else
					{
						setState(PublicDataManager.State.Attack);
					}
				}
			}
			yield return null;
		}
	}
}
