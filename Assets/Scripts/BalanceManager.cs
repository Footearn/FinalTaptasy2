using System;
using System.Collections.Generic;

public class BalanceManager : Singleton<BalanceManager>
{
	public enum VariableType
	{
		GoldVariable1,
		GoldVariable2,
		GoldVariable3,
		NormalMonsterDamageVariable1,
		NormalMonsterDamageVariable2,
		NormalMonsterDamageVariable3,
		NormalMonsterDamageVariable4,
		NormalMonsterHPVariable1,
		NormalMonsterHPVariable2,
		NormalMonsterHPVariable3,
		NormalMonsterHPVariable4,
		MiniBossDamageVariable1,
		MiniBossHPVariable1,
		BossDamageVariable1,
		BossDamageVariable2,
		BossDamageVariable3,
		BossDamageVariable4,
		BossDamageVariable5,
		BossHPVariable1,
		BossHPVariable2,
		BossHPVariable3,
		BossHPVariable4,
		BossHPVariable5,
		AdsGoldMultiply,
		BuyGoldTier1Multiply,
		BuyGoldTier2Multiply,
		BuyGoldTier3Multiply,
		AdsAngelGoldMultiply,
		NormalMonsterHPAllMultiply,
		MiniBossHPAllMultiply,
		BossHPAllMultiply,
		WarriorWeaponDamageVariable1,
		WarriorWeaponDamageVariable2,
		WarriorWeaponDamageVariable3,
		WarriorWeaponDamageVariable4,
		WarriorWeaponDamageAllMultyply,
		PriestWeaponDamageVariable1,
		PriestWeaponDamageVariable2,
		PriestWeaponDamageVariable3,
		PriestWeaponDamageVariable4,
		PriestWeaponDamageAllMultyply,
		ArcherWeaponDamageVariable1,
		ArcherWeaponDamageVariable2,
		ArcherWeaponDamageVariable3,
		ArcherWeaponDamageVariable4,
		ArcherWeaponDamageAllMultyply,
		ColleagueDamageVariable1,
		ColleagueDamageVariable2,
		ColleagueDamageVariable3,
		ColleagueDamageVariable4,
		ColleagueDamageAllMultyply,
		NormalMonsterDamageAllMultyply,
		MiniBossDamageAllMultyply,
		BossDamageAllMultyply,
		WarriorWeaponHPVariable1,
		WarriorWeaponHPVariable2,
		WarriorWeaponHPVariable3,
		WarriorWeaponHPVariable4,
		WarriorWeaponHPAllMultyply,
		PriestTapHealVariable1,
		PriestTapHealVariable2,
		PriestTapHealVariable3,
		PriestTapHealVariable4,
		PriestTapHealAllMultyply,
		ColleagueUnlockPriceVariable1,
		ColleagueUnlockPriceVariable2,
		DaemonKingDamageVariable1,
		DaemonKingHPVariable,
		ArcherCriticalChanceVariable1,
		Length
	}

	[Serializable]
	public class VariableData
	{
		public double currentValue;

		public VariableData(double currentValue)
		{
			this.currentValue = currentValue;
		}
	}

	public Action resetAction = delegate
	{
	};

	public VariableSaveData currentVariableSaveData;

	public VariableData currentTypingVariableData;

	private bool m_isInit;

	private void Awake()
	{
		currentVariableSaveData = Singleton<ParsingManager>.instance.currentParsedStatData.balanceData;
	}

	public VariableSaveData getVariableData()
	{
		currentVariableSaveData = new VariableSaveData();
		currentVariableSaveData.currentVariableDintionary = new Dictionary<VariableType, VariableData>();
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.GoldVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.GoldVariable1, new VariableData(5.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.GoldVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.GoldVariable2, new VariableData(1.4));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.GoldVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.GoldVariable3, new VariableData(0.05));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterDamageVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterDamageVariable1, new VariableData(2.5));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterDamageVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterDamageVariable2, new VariableData(1.25));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterDamageVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterDamageVariable3, new VariableData(0.12));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterDamageVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterDamageVariable4, new VariableData(10.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterDamageAllMultyply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterDamageAllMultyply, new VariableData(3.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterHPVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterHPVariable1, new VariableData(50.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterHPVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterHPVariable2, new VariableData(1.3));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterHPVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterHPVariable3, new VariableData(0.12));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterHPVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterHPVariable4, new VariableData(10.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.NormalMonsterHPAllMultiply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.NormalMonsterHPAllMultiply, new VariableData(6.68));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.MiniBossDamageVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.MiniBossDamageVariable1, new VariableData(2.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.MiniBossDamageAllMultyply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.MiniBossDamageAllMultyply, new VariableData(3.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.MiniBossHPVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.MiniBossHPVariable1, new VariableData(33.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.MiniBossHPAllMultiply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.MiniBossHPAllMultiply, new VariableData(6.68));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossDamageVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossDamageVariable1, new VariableData(2.5));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossDamageVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossDamageVariable2, new VariableData(1.25));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossDamageVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossDamageVariable3, new VariableData(0.12));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossDamageVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossDamageVariable4, new VariableData(10.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossDamageVariable5))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossDamageVariable5, new VariableData(5.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossDamageAllMultyply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossDamageAllMultyply, new VariableData(3.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossHPVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossHPVariable1, new VariableData(50.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossHPVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossHPVariable2, new VariableData(1.3));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossHPVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossHPVariable3, new VariableData(0.12));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossHPVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossHPVariable4, new VariableData(10.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossHPVariable5))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossHPVariable5, new VariableData(77.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BossHPAllMultiply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BossHPAllMultiply, new VariableData(6.68));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.AdsGoldMultiply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.AdsGoldMultiply, new VariableData(450.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BuyGoldTier1Multiply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BuyGoldTier1Multiply, new VariableData(1365.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BuyGoldTier2Multiply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BuyGoldTier2Multiply, new VariableData(15000.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.BuyGoldTier3Multiply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.BuyGoldTier3Multiply, new VariableData(165000.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.AdsAngelGoldMultiply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.AdsAngelGoldMultiply, new VariableData(525.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponDamageVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponDamageVariable1, new VariableData(7.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponDamageVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponDamageVariable2, new VariableData(2.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponDamageVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponDamageVariable3, new VariableData(1.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponDamageVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponDamageVariable4, new VariableData(0.03));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponDamageAllMultyply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponDamageAllMultyply, new VariableData(6.68));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestWeaponDamageVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestWeaponDamageVariable1, new VariableData(6.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestWeaponDamageVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestWeaponDamageVariable2, new VariableData(2.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestWeaponDamageVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestWeaponDamageVariable3, new VariableData(1.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestWeaponDamageVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestWeaponDamageVariable4, new VariableData(0.03));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestWeaponDamageAllMultyply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestWeaponDamageAllMultyply, new VariableData(6.68));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ArcherWeaponDamageVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ArcherWeaponDamageVariable1, new VariableData(17.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ArcherWeaponDamageVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ArcherWeaponDamageVariable2, new VariableData(2.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ArcherWeaponDamageVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ArcherWeaponDamageVariable3, new VariableData(1.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ArcherWeaponDamageVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ArcherWeaponDamageVariable4, new VariableData(0.03));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ArcherWeaponDamageAllMultyply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ArcherWeaponDamageAllMultyply, new VariableData(6.68));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ColleagueDamageVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ColleagueDamageVariable1, new VariableData(15.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ColleagueDamageVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ColleagueDamageVariable2, new VariableData(3.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ColleagueDamageVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ColleagueDamageVariable3, new VariableData(1.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ColleagueDamageVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ColleagueDamageVariable4, new VariableData(0.03));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ColleagueDamageAllMultyply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ColleagueDamageAllMultyply, new VariableData(6.68));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponHPVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponHPVariable1, new VariableData(100.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponHPVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponHPVariable2, new VariableData(2.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponHPVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponHPVariable3, new VariableData(1.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponHPVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponHPVariable4, new VariableData(0.03));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.WarriorWeaponHPAllMultyply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.WarriorWeaponHPAllMultyply, new VariableData(3.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestTapHealVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestTapHealVariable1, new VariableData(0.2));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestTapHealVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestTapHealVariable2, new VariableData(2.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestTapHealVariable3))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestTapHealVariable3, new VariableData(1.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestTapHealVariable4))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestTapHealVariable4, new VariableData(0.03));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.PriestTapHealAllMultyply))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.PriestTapHealAllMultyply, new VariableData(3.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ColleagueUnlockPriceVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ColleagueUnlockPriceVariable1, new VariableData(200.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ColleagueUnlockPriceVariable2))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ColleagueUnlockPriceVariable2, new VariableData(5.0));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.DaemonKingDamageVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.DaemonKingDamageVariable1, new VariableData(1.15));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.DaemonKingHPVariable))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.DaemonKingHPVariable, new VariableData(1.3));
		}
		if (!currentVariableSaveData.currentVariableDintionary.ContainsKey(VariableType.ArcherCriticalChanceVariable1))
		{
			currentVariableSaveData.currentVariableDintionary.Add(VariableType.ArcherCriticalChanceVariable1, new VariableData(1.0));
		}
		return currentVariableSaveData;
	}

	public double getVariableValue(VariableType targetType)
	{
		return currentVariableSaveData.currentVariableDintionary[targetType].currentValue;
	}
}
