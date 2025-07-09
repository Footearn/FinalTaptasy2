using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
	public CloneWarrior parentCloneWarrior;

	public CharacterObject parentCharacterObject;

	public ColleagueObject parentColleagueObject;

	public ElopeModeDaemonKing parentElopeModeDaemonKing;

	public TowerModeCharacterObject parentTowerModeCharacterObject;

	public PVPUnitObject parentPVPUnitObject;

	private CharacterWarrior m_cachedCharacterWarrior;

	private void OnEnable()
	{
		if (parentCharacterObject != null && parentCharacterObject is CharacterWarrior)
		{
			m_cachedCharacterWarrior = parentCharacterObject as CharacterWarrior;
		}
	}

	public void attack()
	{
		if (parentCharacterObject != null)
		{
			parentCharacterObject.attackEnemy();
		}
		if (parentCloneWarrior != null)
		{
			parentCloneWarrior.attackEnemy();
		}
		if (parentColleagueObject != null)
		{
			parentColleagueObject.attackEnemy();
		}
		if (parentElopeModeDaemonKing != null)
		{
			parentElopeModeDaemonKing.attack();
		}
		else if (parentTowerModeCharacterObject != null)
		{
			parentTowerModeCharacterObject.attackEnemy();
		}
		if (parentPVPUnitObject != null)
		{
			parentPVPUnitObject.attackEvent();
		}
	}

	public void attackEnd()
	{
		if (parentCharacterObject != null && m_cachedCharacterWarrior != null && !m_cachedCharacterWarrior.isCastedDivineSmashFromPreview)
		{
			parentCharacterObject.attackEndEvent();
		}
		if (parentColleagueObject != null)
		{
			parentColleagueObject.attackEndEvent();
		}
		if (parentElopeModeDaemonKing != null)
		{
			parentElopeModeDaemonKing.attackEnd();
		}
		if (parentTowerModeCharacterObject != null)
		{
			parentTowerModeCharacterObject.attackEnd();
		}
		if (parentPVPUnitObject != null)
		{
			parentPVPUnitObject.attackEnd();
		}
	}

	public void divineSmash()
	{
		if (parentCharacterObject != null)
		{
			(parentCharacterObject as CharacterWarrior).divineSmash();
		}
		else if (parentTowerModeCharacterObject != null)
		{
			parentTowerModeCharacterObject.divineSmash();
		}
	}
}
