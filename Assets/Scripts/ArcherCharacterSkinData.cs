using System;
using CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public class ArcherCharacterSkinData
{
	public bool isNotice;

	public bool isHaving;

	public CharacterSkinManager.ArcherSkinType skinType;

	public long skinLevel;

	public ObscuredLong _skinLevel;

	public ArcherCharacterSkinData(bool isHaving, CharacterSkinManager.ArcherSkinType skinType)
	{
		this.isHaving = isHaving;
		this.skinType = skinType;
		_skinLevel = 0L;
	}
}
