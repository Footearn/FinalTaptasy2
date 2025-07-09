using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public class ColleagueInventoryData
{
	public ColleagueManager.ColleagueType colleagueType;

	public bool isUnlocked;

	public bool isUnlockedFromSlot;

	public long level;

	public long lastPassiveUnlockLevel;

	public int currentEquippedSkinIndex;

	public Dictionary<int, bool> colleagueSkinData;

	public Dictionary<int, ObscuredLong> colleaugeSkinLevelData;
}
