using System;

[Serializable]
public struct CharacterStatData
{
	public CharacterManager.CharacterType currentCharacterType;

	public float baseDelay;

	public float baseCriticalChance;

	public float baseCriticalDamage;

	public float baseSpeed;

	public float baseRange;
}
