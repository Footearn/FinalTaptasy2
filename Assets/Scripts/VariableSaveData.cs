using System;
using System.Collections.Generic;

[Serializable]
public class VariableSaveData
{
	public Dictionary<BalanceManager.VariableType, BalanceManager.VariableData> currentVariableDintionary = new Dictionary<BalanceManager.VariableType, BalanceManager.VariableData>();
}
