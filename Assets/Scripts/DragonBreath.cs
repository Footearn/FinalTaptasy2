using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBreath : MovingObject
{
	public int lineIdx;

	public int lineMax;

	public SpriteAnimation dragon;

	public ParticleSystem breath;

	public GameObject firewall;

	private Ground ground;

	private bool isBossGround;

	private List<EnemyObject> m_ignoreEnemyList;

	private int idx;

	private CharacterObject caster;

	private Coroutine m_waveUpdateCoroutine;

	private void Awake()
	{
		m_ignoreEnemyList = new List<EnemyObject>();
	}

	public void shootWave(CharacterObject caster)
	{
		fixedDirection = false;
		this.caster = caster;
		m_ignoreEnemyList.Clear();
		StopAllCoroutines();
		lineIdx = Mathf.Min(lineIdx, EnemyManager.bossStageFloor);
		float num = ((caster.cachedTransform.position.x > 0f) ? 1 : (-1));
		ground = Singleton<GroundManager>.instance.currentGroundList[lineIdx];
		isBossGround = ground.isBossGround;
		setDirection(caster.getDirectionEnum());
		if (m_waveUpdateCoroutine != null)
		{
			StopCoroutine(m_waveUpdateCoroutine);
			m_waveUpdateCoroutine = null;
		}
		base.cachedTransform.position = new Vector2(num * 5f, caster.shootPoint.position.y);
		setLocalPosition(false);
		moveTo(new Vector2(0f, ground.outPoint.position.y), 15f, delegate
		{
			fixedDirection = true;
			dragon.playFixAnimation("attackready", 0, 0.05f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("skill_dragon", AudioManager.EffectType.Skill);
				breath.transform.localRotation = Quaternion.Euler(90f, (getDirection() == 1f) ? 180 : 0, 0f);
				breath.gameObject.SetActive(true);
				firewall.SetActive(true);
				m_waveUpdateCoroutine = StartCoroutine(waveUpdate());
			});
		});
		idx = 0;
		breath.gameObject.SetActive(false);
		firewall.SetActive(false);
	}

	private IEnumerator waveUpdate()
	{
		dragon.playAnimation("attack", 0.15f, true);
		if (isBossGround)
		{
			if (!GameManager.isPause)
			{
				float dealingTime = 0.25f;
				float dealingTimer = 0.25f;
				float lifeTime = 3f;
				float lifeTimer = 0f;
				LineFollow(Singleton<EnemyManager>.instance.bossObject.cachedTransform, 13f);
				while (true)
				{
					lifeTimer += Time.deltaTime * GameManager.timeScale;
					dealingTimer += Time.deltaTime * GameManager.timeScale;
					if (dealingTimer >= dealingTime)
					{
						dealingTimer = 0f;
						damageEnemiesForBossRaid(Singleton<EnemyManager>.instance.getNearestEnemies(firewall.transform.position, 4f));
					}
					if (lifeTimer >= lifeTime)
					{
						break;
					}
					yield return null;
				}
				dragon.playAnimation("move", 0.15f, true);
				breath.gameObject.SetActive(false);
				firewall.SetActive(false);
				moveTo(new Vector2(-5f, ground.inPoint.position.y), 6f, delegate
				{
					ObjectPool.Recycle(base.name, base.cachedGameObject);
				});
			}
		}
		else
		{
			while (true)
			{
				if (!GameManager.isPause)
				{
					int line = idx + lineIdx;
					if (idx >= lineMax || line >= Singleton<GroundManager>.instance.currentGroundList.Count)
					{
						break;
					}
					ground = Singleton<GroundManager>.instance.currentGroundList[line];
					damageEnemies(Singleton<EnemyManager>.instance.getNearestEnemies(firewall.transform.position, 2f));
					if (ground != null)
					{
						if (ground.outPoint.position.x > 0f)
						{
							if (base.cachedTransform.position.x < ground.outPoint.position.x)
							{
								base.cachedTransform.position = new Vector2(base.cachedTransform.position.x + (Singleton<CharacterManager>.instance.warriorCharacter.curSpeed + 1f) * Time.deltaTime * GameManager.timeScale, Mathf.Lerp(base.cachedTransform.position.y, ground.cachedTransform.position.y, Time.deltaTime * GameManager.timeScale * 12f));
							}
							else
							{
								idx++;
							}
						}
						else if (base.cachedTransform.position.x > ground.outPoint.position.x)
						{
							base.cachedTransform.position = new Vector2(base.cachedTransform.position.x - (Singleton<CharacterManager>.instance.warriorCharacter.curSpeed + 1f) * Time.deltaTime * GameManager.timeScale, Mathf.Lerp(base.cachedTransform.position.y, ground.cachedTransform.position.y, Time.deltaTime * GameManager.timeScale * 12f));
						}
						else
						{
							idx++;
						}
					}
					else
					{
						idx++;
					}
				}
				yield return null;
			}
			idx = 0;
			dragon.playAnimation("move", 0.15f, true);
			breath.gameObject.SetActive(false);
			firewall.SetActive(false);
			moveTo(new Vector2(-5f, Singleton<GroundManager>.instance.currentGroundList[idx + lineIdx].inPoint.position.y), 10f, delegate
			{
				ObjectPool.Recycle(base.name, base.cachedGameObject);
			});
		}
		m_waveUpdateCoroutine = null;
	}

	private void LineFollow(Transform t, float speed)
	{
		m_ignoreEnemyList.Clear();
		moveTo(t, speed, delegate
		{
			idx++;
			Singleton<CachedManager>.instance.ingameCameraShake.shake(1f, 1f);
			if (idx <= lineMax + 1)
			{
				if (idx % 2 == 0)
				{
					t = Singleton<CharacterManager>.instance.warriorCharacter.cachedTransform;
				}
				else
				{
					t = Singleton<EnemyManager>.instance.bossObject.cachedTransform;
				}
				LineFollow(t, 13f);
			}
		});
	}

	private void damageEnemies(List<EnemyObject> attackEnemies)
	{
		for (int i = 0; i < attackEnemies.Count; i++)
		{
			if (!m_ignoreEnemyList.Contains(attackEnemies[i]))
			{
				double skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DragonsBreath, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DragonsBreath).skillLevel);
				if (attackEnemies[i].isBoss || (attackEnemies[i].myMonsterObject != null && attackEnemies[i].myMonsterObject.isMiniboss))
				{
					skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DragonsBreath, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DragonsBreath).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses);
				}
				if (!attackEnemies[i].isBoss && !attackEnemies[i].isMiniboss)
				{
					skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DragonsBreath, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DragonsBreath).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters);
				}
				attackEnemies[i].setPoisonDamage(skillValue, 0.1f, 3 + (int)Singleton<StatManager>.instance.dragonBreathExtraAttackCount);
				Singleton<CachedManager>.instance.ingameCameraShake.shake(1f, 1f);
				m_ignoreEnemyList.Add(attackEnemies[i]);
			}
		}
	}

	private void damageEnemiesForBossRaid(List<EnemyObject> attackEnemies)
	{
		for (int i = 0; i < attackEnemies.Count; i++)
		{
			double skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DragonsBreath, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DragonsBreath).skillLevel);
			if (attackEnemies[i].isBoss || (attackEnemies[i].myMonsterObject != null && attackEnemies[i].myMonsterObject.isMiniboss))
			{
				skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DragonsBreath, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DragonsBreath).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitBosses);
			}
			if (!attackEnemies[i].isBoss && !attackEnemies[i].isMiniboss)
			{
				skillValue = Singleton<SkillManager>.instance.getSkillValue(SkillManager.SkillType.DragonsBreath, Singleton<SkillManager>.instance.getSkillInventoryData(SkillManager.SkillType.DragonsBreath).skillLevel, Singleton<StatManager>.instance.extraPercentDamageWhenHitNormalMonsters);
			}
			skillValue *= (double)(3 + (int)Singleton<StatManager>.instance.dragonBreathExtraAttackCount);
			skillValue /= 12.0;
			attackEnemies[i].setPoisonDamage(skillValue, 0f, 1);
			Singleton<CachedManager>.instance.ingameCameraShake.shake(1f, 1f);
		}
	}
}
