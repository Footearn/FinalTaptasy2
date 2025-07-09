using System.Collections.Generic;
using UnityEngine;

public class PVPMeteorObject : NewMovingObject
{
	public PVPShadowObject shadowObject;

	public void initMeteor(Vector2 arrivePosition, double damage, bool isAlly)
	{
		shadowObject.initShadow(arrivePosition.y);
		moveTo(arrivePosition, 21f, delegate
		{
			List<PVPUnitObject> nearestUnits = Singleton<PVPUnitManager>.instance.getNearestUnits(arrivePosition, 3f, isAlly);
			for (int i = 0; i < nearestUnits.Count; i++)
			{
				nearestUnits[i].decreaseHP(damage);
			}
			Singleton<AudioManager>.instance.playEffectSound("skill_smash", AudioManager.EffectType.Skill);
			NewObjectPool.Spawn<GameObject>("@PVPExplosionEffect", arrivePosition, new Vector3(90f, 0f, 0f));
			NewObjectPool.Recycle(this);
		});
	}
}
