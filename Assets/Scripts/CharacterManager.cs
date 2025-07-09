using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
	public enum CharacterType
	{
		Warrior,
		Priest,
		Archer,
		Length
	}

	public enum CharacterShaderType
	{
		DefaultShader,
		ReplaceShader,
		RebirthShader
	}

	[Serializable]
	public class CharacterBoneAnimationSpriteRendererData
	{
		public SpriteRenderer shieldSpriteRenderer;

		public SpriteRenderer headSpriteRenderer;

		public SpriteRenderer hairSpriteRenderer;

		public SpriteRenderer spineSpriteRenderer;

		public SpriteRenderer[] legSpriteRenderer;

		public SpriteRenderer leftWingSpriteRenderer;

		public SpriteRenderer rightWingSpriteRenderer;

		public SpriteRenderer capeSpriteRenderer;

		public SpriteRenderer weaponSpriteRenderer;

		public SpriteRenderer tailSpriteRenderer;

		public SpriteRenderer leftHandSpriteRenderer;

		public SpriteRenderer rightHandSpriteRenderer;

		public SpriteRenderer leftLegSpriteRenderer;

		public SpriteRenderer rightLegSpriteRenderer;

		public SpriteRenderer leftFingerSpriteRenderer;

		public SpriteRenderer rightFingerSpriteRenderer;
	}

	public class CharacterBoneAnimationSpriteData
	{
		public Sprite shieldSprite;

		public Sprite headSprite;

		public Sprite headDieSprite;

		public Sprite hairSprite;

		public Sprite spineSprite;

		public Sprite legSprite;

		public Sprite leftWingSprite;

		public Sprite rightWingSprite;

		public Sprite capeSprite;

		public Sprite weaponSprite;

		public Sprite tailSprite;

		public Sprite leftHandSprite;

		public Sprite rightHandSprite;

		public Sprite leftLegSprite;

		public Sprite rightLegSprite;

		public Sprite leftFingerSprite;

		public Sprite rightFingerSprite;
	}

	[Serializable]
	public struct CharacterAttackEffectSpriteData
	{
		public List<Sprite> spriteDataList;
	}

	public const float startPosX = -1.7f;

	public Dictionary<CharacterSkinManager.WarriorSkinType, CharacterBoneAnimationSpriteData> warriorBoneSpriteDictionary;

	public Dictionary<CharacterSkinManager.PriestSkinType, CharacterBoneAnimationSpriteData> priestBoneSpriteDictionary;

	public Dictionary<CharacterSkinManager.ArcherSkinType, CharacterBoneAnimationSpriteData> archerBoneSpriteDictionary;

	public static float intervalBetweenCharacter = 1.2f;

	public static float minXPos = -3.44f;

	public static float maxXPos = 3.44f;

	public int characterNumber;

	public List<CharacterObject> constCharacterList;

	public List<CharacterObject> characterList;

	public CharacterWarrior warriorCharacter;

	public CharacterArcher archerCharacter;

	public CharacterPriest priestCharacter;

	public List<CharacterAttackEffectSpriteData> attackEffectSpriteList;

	public CharacterAttackEffectSpriteData swordSoulAttackEffectSpriteList;

	public List<CharacterAttackEffectSpriteData> transcendAttackEffectSpriteList;

	public CharacterAttackEffectSpriteData transcendSwordSoulAttackEffectSpriteList;

	public Sprite priestNormalBulletSprite;

	public Sprite priestTranscendBulletSprite;

	public Sprite archerNormalBulletSprite;

	public Sprite archerTranscendBulletSprite;

	public double totalDamage
	{
		get
		{
			double num = ((!(warriorCharacter == null)) ? warriorCharacter.curDamage : 0.0);
			double num2 = ((!(priestCharacter == null)) ? priestCharacter.curDamage : 0.0);
			double num3 = ((!(archerCharacter == null)) ? archerCharacter.curDamage : 0.0);
			return num + num2 + num3;
		}
	}

	private void Awake()
	{
		loadCharacterSpriteData();
		characterList = new List<CharacterObject>();
	}

	public Sprite getWarriorHeadSprite(CharacterSkinManager.WarriorSkinType skinType)
	{
		return warriorBoneSpriteDictionary[skinType].headSprite;
	}

	public Sprite getPriestHeadSprite(CharacterSkinManager.PriestSkinType skinType)
	{
		return priestBoneSpriteDictionary[skinType].headSprite;
	}

	public Sprite getArcherHeadSprite(CharacterSkinManager.ArcherSkinType skinType)
	{
		return archerBoneSpriteDictionary[skinType].headSprite;
	}

	public Sprite getWarriorHeadDieSprite(CharacterSkinManager.WarriorSkinType skinType)
	{
		return warriorBoneSpriteDictionary[skinType].headDieSprite;
	}

	private void loadCharacterSpriteData()
	{
		warriorBoneSpriteDictionary = new Dictionary<CharacterSkinManager.WarriorSkinType, CharacterBoneAnimationSpriteData>();
		priestBoneSpriteDictionary = new Dictionary<CharacterSkinManager.PriestSkinType, CharacterBoneAnimationSpriteData>();
		archerBoneSpriteDictionary = new Dictionary<CharacterSkinManager.ArcherSkinType, CharacterBoneAnimationSpriteData>();
		Sprite[] ingameAtlas = Singleton<ResourcesManager>.instance.ingameAtlas;
		Dictionary<string, List<Sprite>> dictionary = new Dictionary<string, List<Sprite>>();
		for (int i = 0; i < ingameAtlas.Length; i++)
		{
			if (ingameAtlas[i].name.Contains("WarriorCharacter") || ingameAtlas[i].name.Contains("PriestCharacter") || ingameAtlas[i].name.Contains("ArcherCharacter"))
			{
				if (!dictionary.ContainsKey(ingameAtlas[i].name))
				{
					dictionary.Add(ingameAtlas[i].name, new List<Sprite>());
				}
				List<Sprite> list = dictionary[ingameAtlas[i].name];
				list.Add(ingameAtlas[i]);
			}
		}
		foreach (KeyValuePair<string, List<Sprite>> item in dictionary)
		{
			if (item.Key.Contains("WarriorCharacter"))
			{
				int num = int.Parse(item.Key.Replace("WarriorCharacter", string.Empty).Split('-')[0]);
				CharacterSkinManager.WarriorSkinType key = (CharacterSkinManager.WarriorSkinType)(num - 1);
				if (!warriorBoneSpriteDictionary.ContainsKey(key))
				{
					warriorBoneSpriteDictionary.Add(key, new CharacterBoneAnimationSpriteData());
				}
				CharacterBoneAnimationSpriteData characterBoneAnimationSpriteData = warriorBoneSpriteDictionary[key];
				List<Sprite> value = item.Value;
				string value2 = "WarriorCharacter" + num + "-Body";
				string value3 = "WarriorCharacter" + num + "-Head";
				string value4 = "WarriorCharacter" + num + "-HeadDie";
				string value5 = "WarriorCharacter" + num + "-Hair";
				string value6 = "WarriorCharacter" + num + "-Leg";
				string value7 = "WarriorCharacter" + num + "-Shield";
				string value8 = "WarriorCharacter" + num + "-Cape";
				string value9 = "WarriorCharacter" + num + "-L_Wing";
				string value10 = "WarriorCharacter" + num + "-R_Wing";
				for (int j = 0; j < value.Count; j++)
				{
					if (value[j].name.Equals(value2))
					{
						characterBoneAnimationSpriteData.spineSprite = value[j];
					}
					else if (value[j].name.Equals(value3))
					{
						characterBoneAnimationSpriteData.headSprite = value[j];
					}
					else if (value[j].name.Equals(value4))
					{
						characterBoneAnimationSpriteData.headDieSprite = value[j];
					}
					else if (value[j].name.Equals(value5))
					{
						characterBoneAnimationSpriteData.hairSprite = value[j];
					}
					else if (value[j].name.Equals(value6))
					{
						characterBoneAnimationSpriteData.legSprite = value[j];
					}
					else if (value[j].name.Equals(value7))
					{
						characterBoneAnimationSpriteData.shieldSprite = value[j];
					}
					else if (value[j].name.Equals(value8))
					{
						characterBoneAnimationSpriteData.capeSprite = value[j];
					}
					else if (value[j].name.Equals(value9))
					{
						characterBoneAnimationSpriteData.leftWingSprite = value[j];
					}
					else if (value[j].name.Equals(value10))
					{
						characterBoneAnimationSpriteData.rightWingSprite = value[j];
					}
				}
			}
			else if (item.Key.Contains("PriestCharacter"))
			{
				int num2 = int.Parse(item.Key.Replace("PriestCharacter", string.Empty).Split('-')[0]);
				CharacterSkinManager.PriestSkinType key2 = (CharacterSkinManager.PriestSkinType)(num2 - 1);
				if (!priestBoneSpriteDictionary.ContainsKey(key2))
				{
					priestBoneSpriteDictionary.Add(key2, new CharacterBoneAnimationSpriteData());
				}
				CharacterBoneAnimationSpriteData characterBoneAnimationSpriteData2 = priestBoneSpriteDictionary[key2];
				List<Sprite> value11 = item.Value;
				string value12 = "PriestCharacter" + num2 + "-Body";
				string value13 = "PriestCharacter" + num2 + "-Head";
				string value14 = "PriestCharacter" + num2 + "-Hair";
				string value15 = "PriestCharacter" + num2 + "-Leg";
				string value16 = "PriestCharacter" + num2 + "-Shield";
				string value17 = "PriestCharacter" + num2 + "-Cape";
				string value18 = "PriestCharacter" + num2 + "-L_Wing";
				string value19 = "PriestCharacter" + num2 + "-R_Wing";
				for (int k = 0; k < value11.Count; k++)
				{
					if (value11[k].name.Equals(value12))
					{
						characterBoneAnimationSpriteData2.spineSprite = value11[k];
					}
					else if (value11[k].name.Equals(value13))
					{
						characterBoneAnimationSpriteData2.headSprite = value11[k];
					}
					else if (value11[k].name.Equals(value14))
					{
						characterBoneAnimationSpriteData2.hairSprite = value11[k];
					}
					else if (value11[k].name.Equals(value15))
					{
						characterBoneAnimationSpriteData2.legSprite = value11[k];
					}
					else if (value11[k].name.Equals(value16))
					{
						characterBoneAnimationSpriteData2.shieldSprite = value11[k];
					}
					else if (value11[k].name.Equals(value17))
					{
						characterBoneAnimationSpriteData2.capeSprite = value11[k];
					}
					else if (value11[k].name.Equals(value18))
					{
						characterBoneAnimationSpriteData2.leftWingSprite = value11[k];
					}
					else if (value11[k].name.Equals(value19))
					{
						characterBoneAnimationSpriteData2.rightWingSprite = value11[k];
					}
				}
			}
			else
			{
				if (!item.Key.Contains("ArcherCharacter"))
				{
					continue;
				}
				int num3 = int.Parse(item.Key.Replace("ArcherCharacter", string.Empty).Split('-')[0]);
				CharacterSkinManager.ArcherSkinType key3 = (CharacterSkinManager.ArcherSkinType)(num3 - 1);
				if (!archerBoneSpriteDictionary.ContainsKey(key3))
				{
					archerBoneSpriteDictionary.Add(key3, new CharacterBoneAnimationSpriteData());
				}
				CharacterBoneAnimationSpriteData characterBoneAnimationSpriteData3 = archerBoneSpriteDictionary[key3];
				List<Sprite> value20 = item.Value;
				string value21 = "ArcherCharacter" + num3 + "-Body";
				string value22 = "ArcherCharacter" + num3 + "-Head";
				string value23 = "ArcherCharacter" + num3 + "-Hair";
				string value24 = "ArcherCharacter" + num3 + "-Leg";
				string value25 = "ArcherCharacter" + num3 + "-Shield";
				string value26 = "ArcherCharacter" + num3 + "-Cape";
				string value27 = "ArcherCharacter" + num3 + "-L_Wing";
				string value28 = "ArcherCharacter" + num3 + "-R_Wing";
				for (int l = 0; l < value20.Count; l++)
				{
					if (value20[l].name.Equals(value21))
					{
						characterBoneAnimationSpriteData3.spineSprite = value20[l];
					}
					else if (value20[l].name.Equals(value22))
					{
						characterBoneAnimationSpriteData3.headSprite = value20[l];
					}
					else if (value20[l].name.Equals(value23))
					{
						characterBoneAnimationSpriteData3.hairSprite = value20[l];
					}
					else if (value20[l].name.Equals(value24))
					{
						characterBoneAnimationSpriteData3.legSprite = value20[l];
					}
					else if (value20[l].name.Equals(value25))
					{
						characterBoneAnimationSpriteData3.shieldSprite = value20[l];
					}
					else if (value20[l].name.Equals(value26))
					{
						characterBoneAnimationSpriteData3.capeSprite = value20[l];
					}
					else if (value20[l].name.Equals(value27))
					{
						characterBoneAnimationSpriteData3.leftWingSprite = value20[l];
					}
					else if (value20[l].name.Equals(value28))
					{
						characterBoneAnimationSpriteData3.rightWingSprite = value20[l];
					}
				}
			}
		}
	}

	public void startGame()
	{
		characterList.Clear();
		constCharacterList[0].startGame();
	}

	public void endGame()
	{
		for (int i = 0; i < constCharacterList.Count; i++)
		{
			if (constCharacterList[i] != null)
			{
				constCharacterList[i].setStateLock(false);
			}
		}
		characterList.Clear();
	}

	public void changeCharacterSkin(CharacterSkinManager.WarriorSkinType skinType, CharacterBoneAnimationSpriteRendererData spriteRendererData)
	{
		CharacterBoneAnimationSpriteData characterBoneAnimationSpriteData = warriorBoneSpriteDictionary[skinType];
		GameObject gameObject = spriteRendererData.shieldSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.shieldSprite != null)
		{
			gameObject.SetActive(true);
			spriteRendererData.shieldSpriteRenderer.sprite = characterBoneAnimationSpriteData.shieldSprite;
		}
		else
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = spriteRendererData.headSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.headSprite != null)
		{
			gameObject2.SetActive(true);
			spriteRendererData.headSpriteRenderer.sprite = characterBoneAnimationSpriteData.headSprite;
		}
		else
		{
			gameObject2.SetActive(false);
		}
		GameObject gameObject3 = spriteRendererData.hairSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.hairSprite != null)
		{
			gameObject3.SetActive(true);
			spriteRendererData.hairSpriteRenderer.sprite = characterBoneAnimationSpriteData.hairSprite;
		}
		else
		{
			gameObject3.SetActive(false);
		}
		GameObject gameObject4 = spriteRendererData.spineSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.spineSprite != null)
		{
			gameObject4.SetActive(true);
			spriteRendererData.spineSpriteRenderer.sprite = characterBoneAnimationSpriteData.spineSprite;
		}
		else
		{
			gameObject4.SetActive(false);
		}
		for (int i = 0; i < spriteRendererData.legSpriteRenderer.Length; i++)
		{
			GameObject gameObject5 = spriteRendererData.legSpriteRenderer[i].gameObject;
			if (characterBoneAnimationSpriteData.legSprite != null)
			{
				gameObject5.SetActive(true);
				spriteRendererData.legSpriteRenderer[i].sprite = characterBoneAnimationSpriteData.legSprite;
			}
			else
			{
				gameObject5.SetActive(false);
			}
		}
		GameObject gameObject6 = spriteRendererData.leftWingSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.leftWingSprite != null)
		{
			gameObject6.SetActive(true);
			spriteRendererData.leftWingSpriteRenderer.sprite = characterBoneAnimationSpriteData.leftWingSprite;
		}
		else
		{
			gameObject6.SetActive(false);
		}
		GameObject gameObject7 = spriteRendererData.rightWingSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.rightWingSprite != null)
		{
			gameObject7.SetActive(true);
			spriteRendererData.rightWingSpriteRenderer.sprite = characterBoneAnimationSpriteData.rightWingSprite;
		}
		else
		{
			gameObject7.SetActive(false);
		}
		GameObject gameObject8 = spriteRendererData.capeSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.capeSprite != null)
		{
			gameObject8.SetActive(true);
			spriteRendererData.capeSpriteRenderer.sprite = characterBoneAnimationSpriteData.capeSprite;
		}
		else
		{
			gameObject8.SetActive(false);
		}
	}

	public void changeCharacterSkin(CharacterSkinManager.PriestSkinType skinType, CharacterBoneAnimationSpriteRendererData spriteRendererData)
	{
		CharacterBoneAnimationSpriteData characterBoneAnimationSpriteData = priestBoneSpriteDictionary[skinType];
		GameObject gameObject = spriteRendererData.shieldSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.shieldSprite != null)
		{
			gameObject.SetActive(true);
			spriteRendererData.shieldSpriteRenderer.sprite = characterBoneAnimationSpriteData.shieldSprite;
		}
		else
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = spriteRendererData.headSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.headSprite != null)
		{
			gameObject2.SetActive(true);
			spriteRendererData.headSpriteRenderer.sprite = characterBoneAnimationSpriteData.headSprite;
		}
		else
		{
			gameObject2.SetActive(false);
		}
		GameObject gameObject3 = spriteRendererData.hairSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.hairSprite != null)
		{
			gameObject3.SetActive(true);
			spriteRendererData.hairSpriteRenderer.sprite = characterBoneAnimationSpriteData.hairSprite;
		}
		else
		{
			gameObject3.SetActive(false);
		}
		GameObject gameObject4 = spriteRendererData.spineSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.spineSprite != null)
		{
			gameObject4.SetActive(true);
			spriteRendererData.spineSpriteRenderer.sprite = characterBoneAnimationSpriteData.spineSprite;
		}
		else
		{
			gameObject4.SetActive(false);
		}
		for (int i = 0; i < spriteRendererData.legSpriteRenderer.Length; i++)
		{
			GameObject gameObject5 = spriteRendererData.legSpriteRenderer[i].gameObject;
			if (characterBoneAnimationSpriteData.legSprite != null)
			{
				gameObject5.SetActive(true);
				spriteRendererData.legSpriteRenderer[i].sprite = characterBoneAnimationSpriteData.legSprite;
			}
			else
			{
				gameObject5.SetActive(false);
			}
		}
		GameObject gameObject6 = spriteRendererData.leftWingSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.leftWingSprite != null)
		{
			gameObject6.SetActive(true);
			spriteRendererData.leftWingSpriteRenderer.sprite = characterBoneAnimationSpriteData.leftWingSprite;
		}
		else
		{
			gameObject6.SetActive(false);
		}
		GameObject gameObject7 = spriteRendererData.rightWingSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.rightWingSprite != null)
		{
			gameObject7.SetActive(true);
			spriteRendererData.rightWingSpriteRenderer.sprite = characterBoneAnimationSpriteData.rightWingSprite;
		}
		else
		{
			gameObject7.SetActive(false);
		}
		GameObject gameObject8 = spriteRendererData.capeSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.capeSprite != null)
		{
			gameObject8.SetActive(true);
			spriteRendererData.capeSpriteRenderer.sprite = characterBoneAnimationSpriteData.capeSprite;
		}
		else
		{
			gameObject8.SetActive(false);
		}
	}

	public void changeCharacterSkin(CharacterSkinManager.ArcherSkinType skinType, CharacterBoneAnimationSpriteRendererData spriteRendererData)
	{
		CharacterBoneAnimationSpriteData characterBoneAnimationSpriteData = archerBoneSpriteDictionary[skinType];
		GameObject gameObject = spriteRendererData.shieldSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.shieldSprite != null)
		{
			gameObject.SetActive(true);
			spriteRendererData.shieldSpriteRenderer.sprite = characterBoneAnimationSpriteData.shieldSprite;
		}
		else
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = spriteRendererData.headSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.headSprite != null)
		{
			gameObject2.SetActive(true);
			spriteRendererData.headSpriteRenderer.sprite = characterBoneAnimationSpriteData.headSprite;
		}
		else
		{
			gameObject2.SetActive(false);
		}
		GameObject gameObject3 = spriteRendererData.hairSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.hairSprite != null)
		{
			gameObject3.SetActive(true);
			spriteRendererData.hairSpriteRenderer.sprite = characterBoneAnimationSpriteData.hairSprite;
		}
		else
		{
			gameObject3.SetActive(false);
		}
		GameObject gameObject4 = spriteRendererData.spineSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.spineSprite != null)
		{
			gameObject4.SetActive(true);
			spriteRendererData.spineSpriteRenderer.sprite = characterBoneAnimationSpriteData.spineSprite;
		}
		else
		{
			gameObject4.SetActive(false);
		}
		for (int i = 0; i < spriteRendererData.legSpriteRenderer.Length; i++)
		{
			GameObject gameObject5 = spriteRendererData.legSpriteRenderer[i].gameObject;
			if (characterBoneAnimationSpriteData.legSprite != null)
			{
				gameObject5.SetActive(true);
				spriteRendererData.legSpriteRenderer[i].sprite = characterBoneAnimationSpriteData.legSprite;
			}
			else
			{
				gameObject5.SetActive(false);
			}
		}
		GameObject gameObject6 = spriteRendererData.leftWingSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.leftWingSprite != null)
		{
			gameObject6.SetActive(true);
			spriteRendererData.leftWingSpriteRenderer.sprite = characterBoneAnimationSpriteData.leftWingSprite;
		}
		else
		{
			gameObject6.SetActive(false);
		}
		GameObject gameObject7 = spriteRendererData.rightWingSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.rightWingSprite != null)
		{
			gameObject7.SetActive(true);
			spriteRendererData.rightWingSpriteRenderer.sprite = characterBoneAnimationSpriteData.rightWingSprite;
		}
		else
		{
			gameObject7.SetActive(false);
		}
		GameObject gameObject8 = spriteRendererData.capeSpriteRenderer.gameObject;
		if (characterBoneAnimationSpriteData.capeSprite != null)
		{
			gameObject8.SetActive(true);
			spriteRendererData.capeSpriteRenderer.sprite = characterBoneAnimationSpriteData.capeSprite;
		}
		else
		{
			gameObject8.SetActive(false);
		}
	}

	public void refreshCharacterUnlockedState()
	{
		Debug.LogError("~~~~~~~~~~~~~equipCharacter 0%");
		Singleton<CachedManager>.instance.warriorTextureRendererCamera.setAcviveCamera(false);
		Debug.LogError("~~~~~~~~~~~~~equipCharacter 30%");
		Singleton<CachedManager>.instance.archerTextureRendererCamera.setAcviveCamera(false);
		Debug.LogError("~~~~~~~~~~~~~equipCharacter 60%");
		Singleton<CachedManager>.instance.priestTextureRendererCamera.setAcviveCamera(false);
		Debug.LogError("~~~~~~~~~~~~~equipCharacter 90%");
		characterNumber = 0;
		Debug.LogError("~~~~~~~~~~~~~equipCharacter开始");
		equipCharacter(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin);
		Singleton<WeaponManager>.instance.equipWeapon(Singleton<DataManager>.instance.currentGameData.warriorEquippedWeapon);
		characterNumber++;
		equipCharacter(Singleton<DataManager>.instance.currentGameData.equippedPriestSkin);
		Singleton<WeaponManager>.instance.equipWeapon(Singleton<DataManager>.instance.currentGameData.priestEquippedWeapon);
		characterNumber++;
		equipCharacter(Singleton<DataManager>.instance.currentGameData.equippedArcherSkin);
		Singleton<WeaponManager>.instance.equipWeapon(Singleton<DataManager>.instance.currentGameData.archerEquippedWeapon);
		characterNumber++;
	}

	public void equipCharacter(CharacterSkinManager.WarriorSkinType skinType)
	{
		Weapon weapon = null;
		if (warriorCharacter != null)
		{
			weapon = warriorCharacter.equippedWeapon;
			if (weapon != null)
			{
				weapon.cachedTransform.SetParent(null);
			}
			ObjectPool.Recycle("@" + warriorCharacter.name, warriorCharacter.cachedGameObject);
			warriorCharacter = null;
			constCharacterList[0] = null;
		}
		//TODO 角色
		Debug.LogError("~~~~~~~~~~~~~~~~~~~~~~equipCharacter");
		CharacterWarrior component = ObjectPool.Spawn("@CharacterWarrior", new Vector3(-1.7f, 0f, 0f), Singleton<CachedManager>.instance.characterTransform).GetComponent<CharacterWarrior>();
		Debug.LogError("~~~~~~~~~~~~~~~~~~~~~~equipCharacter: "+component);
		component.skinType = skinType;
		Singleton<CharacterSkinManager>.instance.warriorEquippedSkinData = Singleton<DataManager>.instance.currentGameData.warriorSkinData[(int)skinType];
		Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin = skinType;
		warriorCharacter = component;
		Singleton<CachedManager>.instance.warriorTextureRendererCamera.initCamera(component.cachedTransform);
		constCharacterList[0] = component;
		if (weapon != null)
		{
			component.equippedWeapon = weapon;
			weapon.cachedTransform.SetParent(component.weaponParentTransform);
			weapon.cachedTransform.localScale = Vector2.one;
		}
		if (constCharacterList[1] != null)
		{
			component.currentFollower = constCharacterList[1];
			constCharacterList[1].myLeaderCharacter = component;
		}
		changeCharacterSkin(skinType, component.characterBoneSpriteRendererData);
		component.hpGauge = Singleton<CachedManager>.instance.mainHPProgressBar;
		component.resetProperties();
		component.setState(PublicDataManager.State.Wait);
	}

	public void equipCharacter(CharacterSkinManager.PriestSkinType skinType)
	{
		Weapon weapon = null;
		if (priestCharacter != null)
		{
			weapon = priestCharacter.equippedWeapon;
			if (weapon != null)
			{
				weapon.cachedTransform.SetParent(null);
			}
			ObjectPool.Recycle("@" + priestCharacter.name, priestCharacter.cachedGameObject);
			priestCharacter = null;
			constCharacterList[3] = null;
		}
		CharacterPriest component = ObjectPool.Spawn("@CharacterPriest", new Vector3(-1.7f - intervalBetweenCharacter * 1f, 0f, -0.5f), Singleton<CachedManager>.instance.characterTransform).GetComponent<CharacterPriest>();
		component.skinType = skinType;
		Singleton<CharacterSkinManager>.instance.priestEquippedSkinData = Singleton<DataManager>.instance.currentGameData.priestSkinData[(int)skinType];
		Singleton<DataManager>.instance.currentGameData.equippedPriestSkin = skinType;
		priestCharacter = component;
		Singleton<CachedManager>.instance.priestTextureRendererCamera.initCamera(component.cachedTransform);
		constCharacterList[1] = component;
		if (weapon != null)
		{
			component.equippedWeapon = weapon;
			weapon.cachedTransform.SetParent(component.weaponParentTransform);
			weapon.cachedTransform.localScale = Vector2.one;
		}
		if (constCharacterList[0] != null)
		{
			constCharacterList[0].currentFollower = component;
			component.myLeaderCharacter = constCharacterList[0];
		}
		if (constCharacterList[2] != null)
		{
			constCharacterList[2].myLeaderCharacter = component;
			component.currentFollower = constCharacterList[2];
		}
		changeCharacterSkin(skinType, component.characterBoneSpriteRendererData);
		component.resetProperties();
		component.setState(PublicDataManager.State.Wait);
	}

	public void equipCharacter(CharacterSkinManager.ArcherSkinType skinType)
	{
		Weapon weapon = null;
		if (archerCharacter != null)
		{
			weapon = archerCharacter.equippedWeapon;
			if (weapon != null)
			{
				weapon.cachedTransform.SetParent(null);
			}
			ObjectPool.Recycle("@" + archerCharacter.name, archerCharacter.cachedGameObject);
			archerCharacter = null;
			constCharacterList[2] = null;
		}
		CharacterArcher component = ObjectPool.Spawn("@CharacterArcher", new Vector3(-1.7f - intervalBetweenCharacter * 2f, 0f, -1f), Singleton<CachedManager>.instance.characterTransform).GetComponent<CharacterArcher>();
		component.skinType = skinType;
		Singleton<CharacterSkinManager>.instance.archerEquippedSkinData = Singleton<DataManager>.instance.currentGameData.archerSkinData[(int)skinType];
		Singleton<DataManager>.instance.currentGameData.equippedArcherSkin = skinType;
		archerCharacter = component;
		Singleton<CachedManager>.instance.archerTextureRendererCamera.initCamera(component.cachedTransform);
		constCharacterList[2] = component;
		if (weapon != null)
		{
			component.equippedWeapon = weapon;
			weapon.cachedTransform.SetParent(component.weaponParentTransform);
			weapon.cachedTransform.localScale = Vector2.one;
		}
		if (constCharacterList[1] != null)
		{
			constCharacterList[1].currentFollower = component;
			component.myLeaderCharacter = constCharacterList[1];
		}
		if (constCharacterList[3] != null)
		{
			constCharacterList[3].myLeaderCharacter = component;
			component.currentFollower = constCharacterList[3];
		}
		changeCharacterSkin(skinType, component.characterBoneSpriteRendererData);
		component.resetProperties();
		component.setState(PublicDataManager.State.Wait);
	}

	public void feverCharacters(float percentage)
	{
		for (int i = 0; i < characterList.Count; i++)
		{
			characterList[i].fever(percentage);
		}
	}

	public void resetProperties()
	{
		for (int i = 0; i < characterList.Count; i++)
		{
			characterList[i].resetProperties();
		}
	}

	public float getDistanceWithWarriorForColleague(ColleagueObject targetColleague)
	{
		if (targetColleague.currentGround.isFirstGround || targetColleague.currentGround.isBossGround)
		{
			return 0f;
		}
		float num = 0f;
		CharacterWarrior characterWarrior = warriorCharacter;
		if (characterWarrior.currentStayingGroundIndex == targetColleague.currentStayingGroundIndex)
		{
			num = Vector2.Distance(characterWarrior.cachedTransform.position, targetColleague.cachedTransform.position);
		}
		else if (characterWarrior.currentStayingGroundIndex > targetColleague.currentStayingGroundIndex)
		{
			Ground ground = targetColleague.currentGround;
			num = Vector2.Distance(targetColleague.cachedTransform.position, ground.outPoint.position);
			while (ground.currnetFloor < characterWarrior.currentGround.currnetFloor)
			{
				Ground nextGround = Singleton<GroundManager>.instance.getNextGround(ground);
				if (nextGround == null)
				{
					break;
				}
				if (ground.stairpoint == null || ground.stairpoint.Length <= 0)
				{
					num = ((!(nextGround == characterWarrior.currentGround)) ? (num + Vector2.Distance(ground.outPoint.position, nextGround.inPoint.position)) : (num + Vector2.Distance(ground.outPoint.position, characterWarrior.cachedTransform.position)));
				}
				num = ((!(nextGround == characterWarrior.currentGround)) ? (num + Vector2.Distance(nextGround.inPoint.position, nextGround.outPoint.position)) : (num + Vector2.Distance(characterWarrior.currentGround.inPoint.position, characterWarrior.cachedTransform.position)));
				ground = nextGround;
			}
		}
		return num;
	}

	public Vector2 getCenterPosition()
	{
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		for (int i = 0; i < characterList.Count; i++)
		{
			zero2 += (Vector2)characterList[i].cachedTransform.position;
		}
		zero = zero2 / characterList.Count;
		if (characterList.Count == 0)
		{
			zero = Vector2.one * 1000f;
		}
		return zero;
	}

	public CharacterObject getLeaderCharacter()
	{
		CharacterObject characterObject = null;
		for (int i = 0; i < characterList.Count; i++)
		{
			characterObject = getCharacter((CharacterType)i);
			if (characterObject != null)
			{
				break;
			}
		}
		return characterObject;
	}

	public CharacterObject getCharacter(CharacterType type)
	{
		CharacterObject result = null;
		for (int i = 0; i < characterList.Count; i++)
		{
			if (characterList[i].currentCharacterType == type)
			{
				result = characterList[i];
				break;
			}
		}
		return result;
	}

	public CharacterObject getNearestCharacter(Vector2 startPosition, float range)
	{
		CharacterObject result = null;
		if (Mathf.Abs(warriorCharacter.cachedTransform.position.y - startPosition.y) <= 0.5f)
		{
			float num = Vector2.Distance(startPosition, warriorCharacter.cachedTransform.position);
			if (num <= range)
			{
				result = ((!warriorCharacter.isDead) ? warriorCharacter : null);
			}
		}
		return result;
	}

	public AttributeBaseStat getCharacterBaseStat(CharacterType type)
	{
		if (!ParsingManager.isLoaded)
		{
			return default(AttributeBaseStat);
		}
		CharacterStatData characterStatData = Singleton<ParsingManager>.instance.currentParsedStatData.characterStatData[type];
		AttributeBaseStat result = default(AttributeBaseStat);
		result.baseDelay = characterStatData.baseDelay;
		result.criticalChance = characterStatData.baseCriticalChance;
		result.criticalDamage = characterStatData.baseCriticalDamage;
		result.baseSpeed = characterStatData.baseSpeed;
		result.attackRange = characterStatData.baseRange;
		return result;
	}
}
