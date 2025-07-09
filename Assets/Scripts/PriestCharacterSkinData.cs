using System;
using CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public class PriestCharacterSkinData
{
	public bool isNotice;

	public bool isHaving;

	public CharacterSkinManager.PriestSkinType skinType;

	public long skinLevel;

	public ObscuredLong _skinLevel;

	public PriestCharacterSkinData(bool isHaving, CharacterSkinManager.PriestSkinType skinType)
	{
		this.isHaving = isHaving;
		this.skinType = skinType;
		_skinLevel = 0L;
	}
}
