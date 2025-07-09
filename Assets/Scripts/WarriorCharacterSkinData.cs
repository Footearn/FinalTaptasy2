using System;
using CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public class WarriorCharacterSkinData
{
	public bool isNotice;

	public bool isHaving;

	public CharacterSkinManager.WarriorSkinType skinType;

	public long skinLevel;

	public ObscuredLong _skinLevel;

	public WarriorCharacterSkinData(bool isHaving, CharacterSkinManager.WarriorSkinType skinType)
	{
		this.isHaving = isHaving;
		this.skinType = skinType;
		_skinLevel = 0L;
	}
}
