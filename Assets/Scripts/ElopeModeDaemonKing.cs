using System.Collections;
using UnityEngine;

public class ElopeModeDaemonKing : MovingObject
{
	public enum DaemonKingFaceType
	{
		Normal,
		HandsomeGuy,
		SuperHandsomeGuy
	}

	public enum DaemonKingSwordType
	{
		Normal,
		SpeedGuy
	}

	public Animation daemonKingAnimation;

	public PublicDataManager.State currentState;

	public BoneAnimationNameData currentBoneAnimationName;

	public Transform headTransform;

	public SpriteRenderer headSpriteRenderer;

	public Transform r_HandTransform;

	public SpriteRenderer r_HandSpriteRenderer;

	public ElopeEnemyObject currentAttackTargetElopeEnemyObject;

	public bool isRewindEffect;

	private string m_currentAnimationName;

	private bool m_isCanAttack;

	private float m_attackTimer;

	private int m_attackCount;

	private DaemonKingFaceType m_currentFaceType;

	private GameObject m_currentShyEffect;

	private DaemonKingSwordType m_currentSwordType;

	private bool m_stateLock;

	public void init()
	{
		setStateLock(false);
		m_currentAnimationName = string.Empty;
		isRewindEffect = false;
		daemonKingAnimation[currentBoneAnimationName.moveName[0]].speed = 1f;
		setDirection(Direction.Right);
		recycleShyEffect();
		setState(PublicDataManager.State.Move);
		resetAttackTimer();
		StartCoroutine(attackTimerUpdate());
		changeFace(DaemonKingFaceType.Normal, false);
		changeSword(DaemonKingSwordType.Normal, false);
	}

	public void changeFace(DaemonKingFaceType faceType, bool withEffect)
	{
		if (m_currentFaceType != faceType)
		{
			m_currentFaceType = faceType;
			headSpriteRenderer.sprite = Singleton<ElopeModeManager>.instance.daemonKingFaceSpriteList[(int)faceType];
			recycleShyEffect();
			if (withEffect)
			{
				Singleton<AudioManager>.instance.playEffectSound("change_demon_face");
				ObjectPool.Spawn("@ElopeDaemonFaceChangeEffect", base.cachedTransform.position + new Vector3(0f, 1f, 0f), new Vector3(90f, 0f, 0f));
				Singleton<CachedManager>.instance.whiteCoverUI.fadeInGame(0.5f, null, true);
			}
			if (faceType == DaemonKingFaceType.HandsomeGuy || faceType == DaemonKingFaceType.SuperHandsomeGuy)
			{
				m_currentShyEffect = ObjectPool.Spawn("@ElopeDaemonFaceShyEffect", new Vector3(0f, 0.12f), new Vector3(0f, -540f, -540f), new Vector3(0.25f, 0.25f, 1f), headTransform);
			}
		}
	}

	public void changeSword(DaemonKingSwordType swordType, bool withEffect)
	{
		if (m_currentSwordType != swordType)
		{
			if (withEffect)
			{
				Singleton<AudioManager>.instance.playEffectSound("change_demon_face");
				ObjectPool.Spawn("@ElopeDaemonFaceChangeEffect", base.cachedTransform.position + new Vector3(0f, 1f, 0f), new Vector3(90f, 0f, 0f));
				Singleton<CachedManager>.instance.whiteCoverUI.fadeInGame(0.5f, null, true);
			}
			m_currentSwordType = swordType;
			r_HandSpriteRenderer.sprite = Singleton<ElopeModeManager>.instance.daemonKingSwordSprites[(int)swordType];
		}
	}

	private void recycleShyEffect()
	{
		if (m_currentShyEffect != null)
		{
			ObjectPool.Recycle(m_currentShyEffect.name, m_currentShyEffect);
			m_currentShyEffect = null;
		}
	}

	private IEnumerator attackTimerUpdate()
	{
		float attackSpeed2 = 0f;
		while (true)
		{
			attackSpeed2 = Singleton<ElopeModeManager>.instance.attackSpeedattackSpeed();
			for (int i = 0; i < currentBoneAnimationName.attackName.Length; i++)
			{
				daemonKingAnimation[currentBoneAnimationName.attackName[i]].speed = Mathf.Clamp(1f / attackSpeed2 * 0.5f, 1f, 2f);
			}
			m_attackTimer += Time.deltaTime * GameManager.timeScale;
			if (m_attackTimer >= attackSpeed2)
			{
				m_isCanAttack = true;
			}
			yield return null;
		}
	}

	private void resetAttackTimer()
	{
		m_isCanAttack = false;
		m_attackTimer = 0f;
	}

	public void setStateLock(bool value)
	{
		m_stateLock = value;
	}

	public void setState(PublicDataManager.State targetState)
	{
		if (!m_stateLock)
		{
			stopMove();
			currentState = targetState;
			switch (currentState)
			{
			case PublicDataManager.State.Idle:
				idleEvent();
				break;
			case PublicDataManager.State.Move:
				moveEvent();
				break;
			case PublicDataManager.State.Attack:
				attackEvent();
				break;
			}
		}
	}

	private void idleEvent()
	{
		playBoneAnimation(currentBoneAnimationName.idleName[0]);
		StopCoroutine(waitForIsCanAttack());
		StartCoroutine(waitForIsCanAttack());
	}

	private IEnumerator waitForIsCanAttack()
	{
		while (!m_isCanAttack)
		{
			yield return null;
		}
		setState(PublicDataManager.State.Attack);
	}

	private void moveEvent()
	{
		refreshAttackTargetEnemy();
		Vector2 targetPosition = currentAttackTargetElopeEnemyObject.cachedTransform.position;
		targetPosition.x -= 2f;
		moveTo(targetPosition, 2f * Singleton<ElopeModeManager>.instance.daemonKingAttackSpeedMuliply, delegate
		{
			Singleton<DataManager>.instance.currentGameData.currentAttackingEnemyHeathForElopeMode = Singleton<ElopeModeManager>.instance.getEnemyMaxHealth(currentAttackTargetElopeEnemyObject.currentLevel);
			UIWindowElopeMode.instance.enemyHPProgressObject.openEnemyHPProgress(currentAttackTargetElopeEnemyObject);
			setState(PublicDataManager.State.Idle);
		});
	}

	private void Update()
	{
		if (currentState == PublicDataManager.State.Move)
		{
			setSpeed(2f * Singleton<ElopeModeManager>.instance.daemonKingAttackSpeedMuliply);
			daemonKingAnimation[currentBoneAnimationName.moveName[0]].speed = Singleton<ElopeModeManager>.instance.daemonKingAttackSpeedMuliply;
			if (!daemonKingAnimation.IsPlaying(currentBoneAnimationName.moveName[0]))
			{
				playBoneAnimation(currentBoneAnimationName.moveName[0]);
			}
		}
	}

	private void attackEvent()
	{
		if (!isRewindEffect)
		{
			resetAttackTimer();
			playBoneAnimation(currentBoneAnimationName.attackName[Random.Range(0, currentBoneAnimationName.attackName.Length)]);
		}
	}

	public void attack()
	{
		if (!isRewindEffect)
		{
			double num = Singleton<ElopeModeManager>.instance.getSkillValue(ElopeModeManager.DaemonKingSkillType.DaemonKingPower);
			double num2 = (double)Random.Range(0, 10000) / 100.0;
			bool isCritical = false;
			Singleton<AudioManager>.instance.playEffectSound("attack_demon_" + Random.Range(1, 3));
			Singleton<AudioManager>.instance.playEffectSound("elope_hit");
			if (num2 <= Singleton<ElopeModeManager>.instance.getDaemonKingCriticalChance())
			{
				isCritical = true;
				Singleton<AudioManager>.instance.playEffectSound("love_smash");
				num += num * 0.01 * Singleton<ElopeModeManager>.instance.getSkillValue(ElopeModeManager.DaemonKingSkillType.LoveLovePower);
				ObjectPool.Spawn("@ElopeDaemonCriticalEffect", currentAttackTargetElopeEnemyObject.cachedTransform.position);
				Singleton<CachedManager>.instance.darkUI.fadeInGame();
				Singleton<CachedManager>.instance.ingameCameraShake.shake(2f, 0.1f);
			}
			currentAttackTargetElopeEnemyObject.decreaseHealth(num, isCritical);
			double num3 = (double)Random.Range(0, 10000) / 100.0;
			if (num3 <= 3.0)
			{
				Singleton<ElopeModeManager>.instance.castActiveSkill(ElopeModeManager.DaemonKingSkillType.HandsomeGuyDaemonKing);
			}
			if (++m_attackCount % 15 == 0)
			{
				m_attackCount = 0;
				Singleton<DataManager>.instance.saveData();
			}
		}
	}

	public void attackEnd()
	{
		if (!isRewindEffect)
		{
			if (currentAttackTargetElopeEnemyObject.isDead)
			{
				setState(PublicDataManager.State.Move);
			}
			else
			{
				setState(PublicDataManager.State.Idle);
			}
		}
	}

	private void refreshAttackTargetEnemy()
	{
		currentAttackTargetElopeEnemyObject = Singleton<ElopeModeManager>.instance.getNextElopeEnemyObject();
	}

	public void playBoneAnimation(string animationName, bool forcePlay = false)
	{
		if (m_currentAnimationName != animationName || forcePlay)
		{
			m_currentAnimationName = animationName;
			daemonKingAnimation.Stop();
			daemonKingAnimation.Play(animationName);
		}
	}
}
