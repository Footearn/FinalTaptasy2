using System.Collections.Generic;
using UnityEngine;

public class GroundManager : Singleton<GroundManager>
{
	private int m_currentFloor;

	public List<Ground> currentGroundList;

	public void addGround(Ground targetGround, int floor)
	{
		Color color = Color.white;
		switch (GameManager.getRealThemeNumber(GameManager.currentTheme))
		{
		case 1:
			color = Util.getCalculatedColor(132f, 115f, 83f);
			break;
		case 2:
			color = Util.getCalculatedColor(132f, 115f, 83f);
			break;
		case 3:
			color = Util.getCalculatedColor(122f, 132f, 56f);
			break;
		case 4:
			color = Util.getCalculatedColor(158f, 98f, 45f);
			break;
		case 5:
			color = Util.getCalculatedColor(52f, 148f, 152f);
			break;
		case 6:
			color = Util.getCalculatedColor(53f, 137f, 185f);
			break;
		case 7:
			color = Util.getCalculatedColor(129f, 117f, 61f);
			break;
		case 8:
			color = Util.getCalculatedColor(102f, 92f, 74f);
			break;
		case 9:
			color = Util.getCalculatedColor(16f, 83f, 89f);
			break;
		case 10:
			color = Util.getCalculatedColor(95f, 72f, 110f);
			break;
		}
		targetGround.floorText.color = color;
		targetGround.currnetFloor = floor;
		TextMesh floorText = targetGround.floorText;
		string text = floor + "f";
		targetGround.textShadow.text = text;
		floorText.text = text;
		currentGroundList.Add(targetGround);
	}

	public void clearGround()
	{
		List<Ground> list = Singleton<GroundManager>.instance.currentGroundList;
		int num = 1;
		while (num < list.Count)
		{
			list.Remove(list[num]);
		}
	}

	public void increaseFloor()
	{
		m_currentFloor++;
		string empty = string.Empty;
		float num = ((float)m_currentFloor - 1f) / (float)EnemyManager.bossStageFloor * 100f;
		if (num == 0f)
		{
			empty = "0";
		}
		else
		{
			empty = num.ToString("n0");
		}
	}

	public void startGame()
	{
		m_currentFloor = 0;
		Singleton<CachedManager>.instance.themebackground.sprite = Singleton<ResourcesManager>.instance.getAnimation(MapManager.prefixStage + "Castle" + ((GameManager.currentTheme <= 200) ? string.Empty : "2"))[0];
		Color color = Color.white;
		if (GameManager.currentStage > 10)
		{
			switch (GameManager.currentTheme)
			{
			case 7:
				color = Util.getCalculatedColor(211f, 88f, 255f);
				break;
			case 8:
				color = Util.getCalculatedColor(92f, 212f, 255f);
				break;
			case 9:
				color = Util.getCalculatedColor(243f, 165f, 255f);
				break;
			case 10:
				color = Util.getCalculatedColor(255f, 157f, 98f);
				break;
			}
			Singleton<CachedManager>.instance.themebackground.color = color;
		}
		else
		{
			Singleton<CachedManager>.instance.themebackground.color = Color.white;
		}
	}

	public Vector2 getHitGroundPosition(Vector2 startPosition)
	{
		if (BossRaidManager.isBossRaid)
		{
			return new Vector2(startPosition.x, -3.48f);
		}
		return getHitGroundPosition(startPosition, 0f);
	}

	public Vector2 getHitGroundPosition(Vector2 startPosition, float xOffset)
	{
		Vector2 zero = Vector2.zero;
		startPosition.x += xOffset;
		return Physics2D.Raycast(startPosition, Vector2.down, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground")).point;
	}

	public Ground getStayingGround(Vector2 startPosition)
	{
		return getStayingGround(startPosition, 0f);
	}

	public Ground getStayingGround(Vector2 startPosition, float xOffset)
	{
		Ground result = null;
		startPosition.x += xOffset;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(startPosition, Vector2.down, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
		if (raycastHit2D.transform != null)
		{
			result = raycastHit2D.transform.GetComponent<Ground>();
		}
		return result;
	}

	public Ground getNextGround(Ground curGround)
	{
		Ground result = null;
		for (int i = 0; i < currentGroundList.Count; i++)
		{
			if (currentGroundList[i] == curGround && i < currentGroundList.Count - 1)
			{
				result = currentGroundList[i + 1];
			}
		}
		return result;
	}

	public bool isGroundBiggerThanTargetGround(Ground currentGround, Ground compareTargetGround)
	{
		return (currentGround.currnetFloor > compareTargetGround.currnetFloor) ? true : false;
	}

	public Ground getPrevBossGround()
	{
		Ground ground = null;
		return currentGroundList[currentGroundList.Count - 2];
	}

	public int getFloor()
	{
		return m_currentFloor;
	}

	public int getCreateFloor()
	{
		return m_currentFloor + 4;
	}

	public List<GameObject> getTorchPos(Transform t, string torchType, string torchName, int torchCount, float[] torchX, float[] torchY)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < torchCount; i++)
		{
			SpriteAnimation component = ObjectPool.Spawn("Torch", new Vector2(torchX[i], torchY[i]), t).GetComponent<SpriteAnimation>();
			component.animationType = torchType;
			component.init();
			component.playAnimation(torchName, 0.1f, true);
			list.Add(component.gameObject);
		}
		return list;
	}

	public List<GameObject> getTorchPos(Transform t, int bgType)
	{
		bgType++;
		int num = 0;
		float[] array = null;
		float[] array2 = null;
		string targetAnimationName = "Candlelight";
		switch (GameManager.currentTheme)
		{
		case 2:
			if (bgType == 7)
			{
				num = 3;
				array = new float[3]
				{
					-2.71f,
					0.18f,
					3.03f
				};
				array2 = new float[3]
				{
					0.39f,
					0.39f,
					0.39f
				};
			}
			break;
		case 3:
			if (bgType == 9)
			{
				num = 2;
				array = new float[2]
				{
					-2.82f,
					2.8f
				};
				array2 = new float[2]
				{
					0.43f,
					0.43f
				};
			}
			break;
		case 4:
			switch (bgType)
			{
			case 7:
				num = 2;
				array = new float[2]
				{
					-1.314f,
					1.613f
				};
				array2 = new float[2]
				{
					0.373f,
					0.373f
				};
				break;
			case 8:
				num = 2;
				array = new float[2]
				{
					-0.81f,
					2.77f
				};
				array2 = new float[2]
				{
					0.26f,
					0.26f
				};
				break;
			case 9:
				num = 2;
				array = new float[2]
				{
					-1.8f,
					1.77f
				};
				array2 = new float[2]
				{
					0.43f,
					0.43f
				};
				break;
			}
			break;
		case 9:
			switch (bgType)
			{
			case 8:
				num = 1;
				array = new float[1]
				{
					1.22f
				};
				array2 = new float[1]
				{
					0.49f
				};
				break;
			case 9:
				targetAnimationName = "Spiritfire";
				num = 4;
				array = new float[4]
				{
					-2.94f,
					-2.51f,
					0.8f,
					3.52f
				};
				array2 = new float[4]
				{
					0.26f,
					0.05f,
					0.34f,
					0.34f
				};
				break;
			}
			break;
		case 10:
			switch (bgType)
			{
			case 5:
				targetAnimationName = "Torchlight";
				num = 2;
				array = new float[2]
				{
					-1.84f,
					1.9f
				};
				array2 = new float[2]
				{
					-0.09f,
					-0.09f
				};
				break;
			case 7:
				targetAnimationName = "Candlelight1";
				num = 3;
				array = new float[3]
				{
					-2.45f,
					0.14f,
					2.62f
				};
				array2 = new float[3]
				{
					0.06f,
					0.06f,
					0.06f
				};
				break;
			case 8:
				targetAnimationName = "Torchlight";
				num = 2;
				array = new float[2]
				{
					-3.14f,
					1.51f
				};
				array2 = new float[2]
				{
					0.25f,
					0.25f
				};
				break;
			}
			break;
		}
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < num; i++)
		{
			SpriteAnimation component = ObjectPool.Spawn("Torch", new Vector2(array[i], array2[i]), t).GetComponent<SpriteAnimation>();
			component.animationType = "Candle" + GameManager.currentTheme;
			component.init();
			component.playAnimation(targetAnimationName, 0.1f, true);
			list.Add(component.gameObject);
		}
		return list;
	}

	public List<GameObject> getTorchPos(Transform t, bool isBoss)
	{
		int num = 0;
		float[] array = null;
		float y = 0f;
		string targetAnimationName = "Candlelight";
		switch (GameManager.currentTheme)
		{
		case 1:
			if (isBoss)
			{
				num = 4;
				array = new float[4]
				{
					-4.1f,
					-1.363f,
					1.376f,
					4.1f
				};
				y = 1.1f;
			}
			else
			{
				num = 4;
				array = new float[4]
				{
					-2.972f,
					-0.98f,
					1.04f,
					3.02f
				};
				y = 0.57f;
			}
			break;
		case 2:
			if (isBoss)
			{
				num = 4;
				array = new float[4]
				{
					-4.1f,
					-1.363f,
					1.376f,
					4.1f
				};
				y = 1.12f;
			}
			else
			{
				num = 3;
				array = new float[3]
				{
					-2.08f,
					0.06f,
					2.14f
				};
				y = 0.874f;
			}
			break;
		case 3:
			if (isBoss)
			{
				num = 4;
				array = new float[4]
				{
					-4.1f,
					-1.363f,
					1.376f,
					4.1f
				};
				y = 1.16f;
			}
			else
			{
				num = 3;
				array = new float[3]
				{
					-1.99f,
					0f,
					2.22f
				};
				y = 0.63f;
			}
			break;
		case 4:
			if (isBoss)
			{
				num = 4;
				array = new float[4]
				{
					-4.062f,
					-1.39f,
					1.36f,
					4.08f
				};
				y = 1.22f;
			}
			else
			{
				num = 5;
				array = new float[5]
				{
					-4.11f,
					-2.05f,
					0f,
					2.09f,
					4.18f
				};
				y = 1.14f;
			}
			break;
		case 5:
			if (isBoss)
			{
				num = 4;
				array = new float[4]
				{
					-4.1f,
					-1.363f,
					1.376f,
					4.1f
				};
				y = 0.962f;
			}
			else
			{
				num = 4;
				array = new float[4]
				{
					-2.98f,
					-0.95f,
					1.02f,
					3.03f
				};
				y = 0.68f;
			}
			break;
		case 7:
			if (!isBoss)
			{
				num = 3;
				array = new float[3]
				{
					-2.16f,
					-0.04f,
					2.09f
				};
				y = 0.73f;
			}
			break;
		case 8:
			if (isBoss)
			{
				num = 3;
				array = new float[3]
				{
					-2.706f,
					0.023f,
					2.765f
				};
				y = 1.04f;
			}
			else
			{
				num = 4;
				array = new float[4]
				{
					-2.99f,
					-0.99f,
					1.03f,
					3.03f
				};
				y = 0.66f;
			}
			break;
		case 9:
			if (isBoss)
			{
				num = 4;
				array = new float[4]
				{
					-4.1f,
					-1.363f,
					1.376f,
					4.1f
				};
				y = 1.14f;
			}
			else
			{
				num = 3;
				array = new float[3]
				{
					-2.09f,
					0f,
					2.099f
				};
				y = 0.898f;
			}
			break;
		case 10:
			if (isBoss)
			{
				targetAnimationName = "Candlelight2";
				num = 3;
				array = new float[3]
				{
					-2.743f,
					0f,
					2.734f
				};
				y = 1.149f;
			}
			break;
		}
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < num; i++)
		{
			SpriteAnimation component = ObjectPool.Spawn("Torch", new Vector2(array[i], y), t).GetComponent<SpriteAnimation>();
			component.animationType = "Candle" + GameManager.currentTheme;
			component.init();
			component.playAnimation(targetAnimationName, 0.1f, true);
			list.Add(component.gameObject);
		}
		return list;
	}
}
