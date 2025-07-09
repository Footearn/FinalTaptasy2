using System;
using System.Collections.Generic;
using UnityEngine;

public class PVPProjectileManager : Singleton<PVPProjectileManager>
{
	public struct ProjectileData
	{
		public Sprite sprite;

		public float projectileSpeed;

		public bool isRotatable;

		public bool isParabolic;

		public float parabolicMultiply;

		public bool isImmediate;
	}

	public struct ProjectileArriveData
	{
		public Vector2 arrivePosition;

		public ProjectileAttributeType attribute;

		public ProjectileArriveData(Vector2 arrivePosition, ProjectileAttributeType attribute)
		{
			this.arrivePosition = arrivePosition;
			this.attribute = attribute;
		}
	}

	public enum ProjectileAttributeType
	{
		None = -1,
		NormalProjectile,
		FireBall,
		Blow,
		Length
	}

	public Sprite blowProjectileSprite;

	public Sprite tankProjectileSprite;

	public Sprite priestProjectileSprite;

	public Sprite archerProjectileSprite;

	public List<PVPProjectileObject> totalProjectileObject = new List<PVPProjectileObject>();

	public void startGame()
	{
		clearAllProjectiles();
	}

	public void endGame()
	{
		clearAllProjectiles();
	}

	public void clearAllProjectiles()
	{
		for (int i = 0; i < totalProjectileObject.Count; i++)
		{
			NewObjectPool.Recycle(totalProjectileObject[i]);
		}
		totalProjectileObject.Clear();
	}

	public void spawnProjectile(ProjectileAttributeType projectileAttributeType, PVPManager.PVPSkinData unitData, Vector3 spawnPosition, Transform arriveTransform, Action<ProjectileArriveData> arriveAction)
	{
		ProjectileData projectileData = getProjectileData(unitData);
		string poolName = string.Empty;
		switch (projectileAttributeType)
		{
		case ProjectileAttributeType.NormalProjectile:
			poolName = "@Projectile";
			break;
		case ProjectileAttributeType.FireBall:
			poolName = "@PVPFireBallProjectile";
			projectileData.projectileSpeed = 5f;
			projectileData.parabolicMultiply = 3.5f;
			break;
		case ProjectileAttributeType.Blow:
			poolName = "@Projectile";
			projectileData.sprite = blowProjectileSprite;
			projectileData.projectileSpeed = 5f;
			projectileData.parabolicMultiply = 4.5f;
			break;
		}
		PVPProjectileObject pVPProjectileObject = NewObjectPool.Spawn<PVPProjectileObject>(poolName, spawnPosition);
		pVPProjectileObject.initProjectile(arriveTransform, projectileData, arriveAction, projectileAttributeType);
		totalProjectileObject.Add(pVPProjectileObject);
	}

	public void spawnProjectile(ProjectileAttributeType projectileAttributeType, ProjectileData data, Vector3 spawnPosition, Vector2 arrivaPosition, float parabolicValue, Action<ProjectileArriveData> arriveAction)
	{
		string poolName = string.Empty;
		switch (projectileAttributeType)
		{
		case ProjectileAttributeType.NormalProjectile:
			poolName = "@Projectile";
			break;
		case ProjectileAttributeType.FireBall:
			poolName = "@PVPFireBallProjectile";
			break;
		case ProjectileAttributeType.Blow:
			poolName = "@Projectile";
			data.sprite = blowProjectileSprite;
			break;
		}
		PVPProjectileObject pVPProjectileObject = NewObjectPool.Spawn<PVPProjectileObject>(poolName, spawnPosition);
		pVPProjectileObject.initProjectile(arrivaPosition, data, parabolicValue, arriveAction, projectileAttributeType);
		totalProjectileObject.Add(pVPProjectileObject);
	}

	public ProjectileData getProjectileData(PVPManager.PVPSkinData unitData)
	{
		ProjectileData result = default(ProjectileData);
		if (unitData.currentColleagueType == ColleagueManager.ColleagueType.None)
		{
			result.isImmediate = false;
			switch (unitData.currentCharacterType)
			{
			case CharacterManager.CharacterType.Warrior:
				result.isImmediate = true;
				break;
			case CharacterManager.CharacterType.Priest:
				result.projectileSpeed = 10f;
				result.isParabolic = true;
				result.parabolicMultiply = 0.6f;
				result.sprite = priestProjectileSprite;
				break;
			case CharacterManager.CharacterType.Archer:
				result.projectileSpeed = 12.5f;
				result.isParabolic = true;
				result.parabolicMultiply = 1.2f;
				result.sprite = archerProjectileSprite;
				break;
			}
		}
		else
		{
			ColleagueManager.ColleagueBulletAttributeData colleagueBulletAttributeData = Singleton<ColleagueManager>.instance.colleagueBulletDataList[(int)unitData.currentColleagueType].bulletAttributes[(int)(long)unitData.currentColleagueSkinIndex - 1];
			result.isParabolic = true;
			switch (Singleton<PVPUnitManager>.instance.getAttackType(unitData))
			{
			case PVPUnitManager.AttackType.MiddleRange:
				result.parabolicMultiply = 0.6f;
				break;
			case PVPUnitManager.AttackType.LongRange:
				result.parabolicMultiply = 1.2f;
				break;
			}
			result.isRotatable = colleagueBulletAttributeData.isRotatable;
			result.isImmediate = colleagueBulletAttributeData.isImmediateBullet;
			result.sprite = colleagueBulletAttributeData.bulletSprite;
			result.projectileSpeed = colleagueBulletAttributeData.speed;
		}
		return result;
	}
}
