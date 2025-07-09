using System;
using System.Collections.Generic;
using UnityEngine;

public class ElopeCartObject : ObjectBase
{
	public static float startXPosition = 1.169f;

	public static float maxPrincessCountForOneFloor = 7f;

	public static float intervalBetweenPrincess = 0.374f;

	public ElopePrincessObject firstPrincess;

	public List<ElopePrincessObject> currentPrincessList = new List<ElopePrincessObject>();

	public GameObject[] floorObjects;

	public Transform[] floorTransforms;

	public GameObject wheelObject;

	public Transform[] wheelTransforms;

	private GameObject m_currentHeartEffect;

	public void initCart(int totalPrincessCount)
	{
		totalPrincessCount--;
		recyclePrincess();
		int num = (int)Math.Ceiling((float)totalPrincessCount / maxPrincessCountForOneFloor);
		for (int i = 0; i < floorObjects.Length; i++)
		{
			if (i < num)
			{
				if (!floorObjects[i].activeSelf)
				{
					floorObjects[i].SetActive(true);
				}
			}
			else if (floorObjects[i].activeSelf)
			{
				floorObjects[i].SetActive(false);
			}
		}
		if (num <= 0)
		{
			CameraFollow.instance.offsetForElopeMode.x = 0.49f;
			if (wheelObject.activeSelf)
			{
				wheelObject.SetActive(false);
			}
		}
		else
		{
			CameraFollow.instance.offsetForElopeMode.x = -0.5f;
			if (!wheelObject.activeSelf)
			{
				wheelObject.SetActive(true);
			}
		}
		Vector2 v = new Vector2(startXPosition, 0.1276f);
		int num2 = 1;
		int num3 = 0;
		ElopePrincessObject component = ObjectPool.Spawn("@ElopePrincess", new Vector3(-0.201f, 0.199651f, 0f), Vector3.zero, new Vector3(0.25f, 0.25f, 1f), Singleton<ElopeModeManager>.instance.currentElopeModeDaemonKing.headTransform).GetComponent<ElopePrincessObject>();
		component.initPrincess(1);
		currentPrincessList.Add(component);
		firstPrincess = component;
		for (int j = 0; j < totalPrincessCount; j++)
		{
			Transform parent = floorTransforms[num2 - 1];
			component = ObjectPool.Spawn("@ElopePrincess", v, parent).GetComponent<ElopePrincessObject>();
			component.initPrincess(j + 2);
			currentPrincessList.Add(component);
			v.x -= intervalBetweenPrincess;
			if ((float)(++num3) >= maxPrincessCountForOneFloor)
			{
				num3 = 0;
				num2++;
				v.x = startXPosition;
			}
		}
		setHeartEffect(ElopeModeManager.DaemonKingSkillType.None);
	}

	public void setHeartEffect(ElopeModeManager.DaemonKingSkillType skillType)
	{
		if (m_currentHeartEffect != null)
		{
			ObjectPool.Recycle(m_currentHeartEffect.name, m_currentHeartEffect);
			m_currentHeartEffect = null;
		}
		switch (skillType)
		{
		case ElopeModeManager.DaemonKingSkillType.HandsomeGuyDaemonKing:
			m_currentHeartEffect = ObjectPool.Spawn("@ElopePrincessHeartEffect", new Vector3(0f, 0.809f, 0f), new Vector3(-90f, 0f, 0f), firstPrincess.cachedTransform);
			break;
		case ElopeModeManager.DaemonKingSkillType.SuperHandsomeGuyDaemonKing:
			m_currentHeartEffect = ObjectPool.Spawn("@ElopePrincessHeartEffect_Fast", new Vector3(0f, 0.809f, 0f), new Vector3(-90f, 0f, 0f), firstPrincess.cachedTransform);
			break;
		}
	}

	public void playAllPrincessAnimation(string animationName)
	{
		for (int i = 0; i < currentPrincessList.Count; i++)
		{
			currentPrincessList[i].stopRandomAnimationPlay();
			currentPrincessList[i].currentPrincessSpriteAnimation.playAnimation(animationName, UnityEngine.Random.Range(0.08f, 0.12f), true);
		}
	}

	public void playAllPrincessAnimation(string animationName, float duration)
	{
		for (int i = 0; i < currentPrincessList.Count; i++)
		{
			currentPrincessList[i].stopRandomAnimationPlay();
			currentPrincessList[i].currentPrincessSpriteAnimation.playAnimation(animationName, duration, true);
		}
	}

	public void playAllPrincessRandomAnimation()
	{
		for (int i = 0; i < currentPrincessList.Count; i++)
		{
			currentPrincessList[i].startRandomAnimaiotnPlay();
		}
	}

	public void recycleCart()
	{
		if (m_currentHeartEffect != null)
		{
			ObjectPool.Recycle(m_currentHeartEffect.name, m_currentHeartEffect);
			m_currentHeartEffect = null;
		}
		recyclePrincess();
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}

	private void recyclePrincess()
	{
		for (int i = 0; i < currentPrincessList.Count; i++)
		{
			ObjectPool.Recycle(currentPrincessList[i].name, currentPrincessList[i].cachedGameObject);
		}
		currentPrincessList.Clear();
	}

	private void Update()
	{
		if (Singleton<ElopeModeManager>.instance.currentElopeModeDaemonKing != null && Singleton<ElopeModeManager>.instance.currentElopeModeDaemonKing.isMoving())
		{
			for (int i = 0; i < wheelTransforms.Length; i++)
			{
				Vector3 localEulerAngles = wheelTransforms[i].localEulerAngles;
				localEulerAngles.z -= Time.deltaTime * 350f * Singleton<ElopeModeManager>.instance.currentElopeModeDaemonKing.getSpeed() * (float)((!Singleton<ElopeModeManager>.instance.currentElopeModeDaemonKing.isRewindEffect) ? 1 : (-1));
				wheelTransforms[i].localEulerAngles = localEulerAngles;
			}
		}
	}
}
