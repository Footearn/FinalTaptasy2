using System.Collections.Generic;
using UnityEngine;

public class PVPDragonBreath : NewMovingObject
{
	public FadeChangableObject fadeChangableObject;

	public GameObject flameEffectObject;

	public Transform flameEffectTransform;

	public Transform hitTransform;

	public SpriteAnimation dragonAnimation;

	public NewMovingObject dragonMovingObject;

	public Transform shadowTransform;

	private bool m_isAttacking;

	private bool m_isAlly;

	private List<PVPUnitObject> m_attackIgnoreList = new List<PVPUnitObject>();

	private double m_skillDamage;

	public void initDragon(bool isAlly, double skillDamage)
	{
		m_skillDamage = skillDamage;
		m_isAlly = isAlly;
		m_attackIgnoreList.Clear();
		m_isAttacking = false;
		setFixedDirection(true);
		dragonMovingObject.setFixedDirection(true);
		setDirection(isAlly ? Direction.Right : Direction.Left);
		flameEffectTransform.localEulerAngles = new Vector3(90f, (currentDirection == Direction.Right) ? (-180) : 0, 0f);
		flameEffectObject.SetActive(false);
		dragonAnimation.playAnimation("move", 0.15f, true);
		dragonMovingObject.cachedTransform.position = base.cachedTransform.position + new Vector3((!m_isAlly) ? 6.64f : (-6.64f), 7.79f, 0f);
		dragonMovingObject.moveTo(base.cachedTransform.position + new Vector3((!isAlly) ? 1.96f : (-1.96f), 4.07f, 0f), 9f, delegate
		{
			dragonAnimation.playFixAnimation("attackready", 0, 0.3f, delegate
			{
				Singleton<AudioManager>.instance.playEffectSound("skill_dragon", AudioManager.EffectType.Skill);
				m_isAttacking = true;
				flameEffectObject.SetActive(true);
				dragonAnimation.playAnimation("attack", 0.15f, true);
				Vector2 targetPosition = new Vector2((!isAlly) ? (-9) : 9, base.cachedTransform.position.y);
				moveTo(targetPosition, 6.5f, delegate
				{
					m_isAttacking = false;
					flameEffectObject.SetActive(false);
					dragonMovingObject.moveTo(base.cachedTransform.position + new Vector3((!isAlly) ? (-1.96f) : 1.96f, 7.79f, 0f), 9f, delegate
					{
						fadeChangableObject.fadeIn(1.5f, delegate
						{
							NewObjectPool.Recycle(this);
						});
					});
				});
			});
		});
	}

	private void Update()
	{
		shadowTransform.position = new Vector2(dragonMovingObject.cachedTransform.position.x, base.cachedTransform.position.y);
		if (!m_isAttacking)
		{
			return;
		}
		List<PVPUnitObject> nearestUnits = Singleton<PVPUnitManager>.instance.getNearestUnits(hitTransform.position, 2f, m_isAlly);
		for (int i = 0; i < nearestUnits.Count; i++)
		{
			PVPUnitObject pVPUnitObject = nearestUnits[i];
			if (!m_attackIgnoreList.Contains(pVPUnitObject))
			{
				pVPUnitObject.decreaseHP(m_skillDamage);
				m_attackIgnoreList.Add(pVPUnitObject);
			}
		}
	}
}
