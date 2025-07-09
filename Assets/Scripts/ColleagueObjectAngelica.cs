using System.Collections;
using UnityEngine;

public class ColleagueObjectAngelica : ColleagueObject
{
	public override void initColleague()
	{
		StopCoroutine("angelicaPassiveUpdate");
		base.initColleague();
		StartCoroutine("angelicaPassiveUpdate");
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
	}

	private IEnumerator angelicaPassiveUpdate()
	{
		float timer = 0f;
		CharacterWarrior warrior = Singleton<CharacterManager>.instance.warriorCharacter;
		while (true)
		{
			if (!GameManager.isPause && GameManager.currentGameState == GameManager.GameState.Playing && warrior.getState() != PublicDataManager.State.Wait && warrior.getState() != PublicDataManager.State.Die && !warrior.isFirstGround())
			{
				timer += Time.deltaTime * GameManager.timeScale;
				while (timer >= 1f)
				{
					timer -= 1f;
					double health = warrior.maxHealth / 100.0 * 10.0;
					warrior.increasesHealth(health);
					ObjectPool.Spawn("@AngelicaHealEffect", new Vector3(0f, -0.1187687f, 0f), warrior.cachedTransform);
					setState(PublicDataManager.State.Attack);
				}
			}
			yield return null;
		}
	}
}
