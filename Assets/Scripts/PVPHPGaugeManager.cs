using System.Collections.Generic;
using UnityEngine;

public class PVPHPGaugeManager : Singleton<PVPHPGaugeManager>
{
	public List<PVPHPGaugeObject> totalHpGaugeList = new List<PVPHPGaugeObject>();

	public RectTransform centerForGaugeRectTransform;

	public void startGame()
	{
		clearAll();
	}

	public void endGame()
	{
		clearAll();
	}

	private void clearAll()
	{
		for (int i = 0; i < totalHpGaugeList.Count; i++)
		{
			totalHpGaugeList[i].recycleGaugeBar(false);
		}
		totalHpGaugeList.Clear();
	}

	public PVPHPGaugeObject spawnHPGauge(PVPUnitObject targetUnit, bool isAllyGauge)
	{
		string poolName = ((!isAllyGauge) ? "@HPGauge_Enemy" : "@HPGauge_Ally");
		PVPHPGaugeObject pVPHPGaugeObject = NewObjectPool.Spawn<PVPHPGaugeObject>(poolName, Vector2.zero, centerForGaugeRectTransform);
		pVPHPGaugeObject.initGaugeBar(targetUnit);
		totalHpGaugeList.Add(pVPHPGaugeObject);
		return pVPHPGaugeObject;
	}
}
