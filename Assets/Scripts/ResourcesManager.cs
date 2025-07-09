using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
	[Serializable]
	public struct SpriteDictionary
	{
		public string name;

		public Sprite[] sprites;

		public SpriteDictionary(string name, Sprite[] sprites)
		{
			this.name = name;
			this.sprites = sprites;
		}
	}

	public Sprite[] ingameAtlas;

	public Sprite[] themeAtlas;

	public AudioClip[] effectAllAudioClips;

	public AudioClip[] backgroundBgmAllAudioClips;

	public Material spriteDefaultMaterial;

	public Material redColorSpriteDefaultMaterial;

	public List<SpriteDictionary> animationCache;

	public void Awake()
	{
		getSoundFiles();
		createObjectPools();
	}

	[ContextMenu("Reset All")]
	public void ResetAll()
	{
		ingameAtlas = Resources.LoadAll<Sprite>("Sprites/IngameAtlas/IngameAtlas");
		Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!ingameAtlas: "+ingameAtlas);
		themeAtlas = Resources.LoadAll<Sprite>("Sprites/ThemeBackgroundAtlas/ThemeBackgroundAtlas");
		effectAllAudioClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
		backgroundBgmAllAudioClips = Resources.LoadAll<AudioClip>("Sounds/BGM");
		setSprites();
		WeaponManager[] array = Resources.FindObjectsOfTypeAll<WeaponManager>();
		if (array != null && array.Length > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].getIngameWeaponSprites();
			}
		}
	}

	private void getSoundFiles()
	{
		for (int i = 0; i < effectAllAudioClips.Length; i++)
		{
			Singleton<AudioManager>.instance.addEffectAudioClip(effectAllAudioClips[i].name, effectAllAudioClips[i]);
		}
		for (int j = 0; j < backgroundBgmAllAudioClips.Length; j++)
		{
			Singleton<AudioManager>.instance.addBackgroundAudioClip(backgroundBgmAllAudioClips[j].name, backgroundBgmAllAudioClips[j]);
		}
	}

	public void IngameObjectPools(bool load, Action endLoadDelegateOnlyWorkingLoad = null)
	{
		if (load)
		{
			if (!Singleton<DataManager>.instance.currentGameData.isLowEndDevice)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@ColleagueBullet"), 20, "@ColleagueBullet", true);
				});
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@NormalArrow"), 5, "@NormalArrow", true);
				});
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@PriestBullet"), 5, "@PriestBullet", true);
				});
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@BoneBullet"), 10, "@BoneBullet", true);
				});
				GameObject goldPrefabObject = Resources.Load<GameObject>("Prefabs/Ingame/@Gold");
				for (int i = 0; i < 10; i++)
				{
					MultiThreadManager.RegistryTask(delegate
					{
						ObjectPool.CreatePool(goldPrefabObject, 15, "@Gold", true);
					});
				}
				GameObject damageBlueText = Resources.Load<GameObject>("Prefabs/Ingame/DamageFonts/@DamageBlueText");
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(damageBlueText, 5, "@DamageBlueText", true);
				});
				GameObject damageText = Resources.Load<GameObject>("Prefabs/Ingame/DamageFonts/@DamageText");
				for (int j = 0; j < 10; j++)
				{
					MultiThreadManager.RegistryTask(delegate
					{
						ObjectPool.CreatePool(damageText, 5, "@DamageText", true);
					});
				}
				GameObject damageYellowText = Resources.Load<GameObject>("Prefabs/Ingame/DamageFonts/@DamageYellowText");
				for (int k = 0; k < 12; k++)
				{
					MultiThreadManager.RegistryTask(delegate
					{
						ObjectPool.CreatePool(damageYellowText, 5, "@DamageYellowText", true);
					});
				}
			}
			if (Singleton<SkillManager>.instance.getReinforcementSkillInventoryData(SkillManager.SkillType.DivineSmash).isUnlocked)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@ReinforcementDivineSmashEffect"), 4, "@ReinforcementDivineSmashEffect", true);
				});
			}
			if (Singleton<SkillManager>.instance.getReinforcementSkillInventoryData(SkillManager.SkillType.DragonsBreath).isUnlocked)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@ReinforcementDragonBreath"), 12, "@ReinforcementDragonBreath", true);
				});
			}
			if (Singleton<SkillManager>.instance.getReinforcementSkillInventoryData(SkillManager.SkillType.WhirlWind).isUnlocked)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@ReinforcementWhirlWindEffect"), 6, "@ReinforcementWhirlWindEffect", true);
				});
			}
			if (Singleton<SkillManager>.instance.getReinforcementSkillInventoryData(SkillManager.SkillType.Concentration).isUnlocked)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@ReinforcementConcentrationEffect"), 2, "@ReinforcementConcentrationEffect", true);
				});
			}
			if (Singleton<SkillManager>.instance.getReinforcementSkillInventoryData(SkillManager.SkillType.ClonedWarrior).isUnlocked)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@ReinforcementCloneWarriorConfusionBombEffect"), 2, "@ReinforcementCloneWarriorConfusionBombEffect", true);
				});
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@ConfusionEffect"), 8, "@ConfusionEffect", true);
				});
			}
			if (Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.FrostSkill).isUnlocked)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@FrozenSkillEffect"), 2, "@FrozenSkillEffect", true);
				});
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@FrostEffect"), 7, "@FrostEffect", true);
				});
			}
			if (Singleton<SkillManager>.instance.getPassiveSkillInventoryData(SkillManager.PassiveSkillType.MeteorRain).isUnlocked)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@Meteor"), 20, "@Meteor", true);
				});
			}
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@EliteMonsterEffect"), 5, "@EliteMonsterEffect", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@PenetrationEffect"), 3, "@PenetrationEffect", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/@PenetrationArrow"), 3, "@PenetrationArrow", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@AngelicaHealEffect"), 3, "@AngelicaHealEffect", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@TranscendDecreaseHitDamageEffect"), 1, "@TranscendDecreaseHitDamageEffect", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@WhirlWindEffect"), 6, "@WhirlWindEffect", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@TranscendIncreaseAllDamageEffect"), 1, "@TranscendIncreaseAllDamageEffect", true);
			});
			if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(ColleagueManager.ColleagueType.Goblin).isUnlocked)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Effect/@GoblinChestEffect"), 1, "@GoblinChestEffect", true);
				});
			}
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@ElasticBullet"), 5, "@ElasticBullet", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/@Ruby"), 5, "@Ruby", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/@CollectEventDropObject"), 10, "@CollectEventDropObject", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/@TranscendStone"), 2, "@TranscendStone", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/@WeaponSkinPiece"), 2, "@WeaponSkinPiece", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/@TreasureChest"), 5, "@TreasureChest", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/SavedPeople/@LastPrincess"), 2, "@LastPrincess", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/SavedPeople/@Emotion"), 1, "@Emotion", true);
			});
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/SavedPeople/@Jail"), 2, "@Jail", true);
			});
			int realThemeNumber = GameManager.getRealThemeNumber(GameManager.currentTheme);
			List<EnemyManager.BossType> list = Singleton<EnemyManager>.instance.realBossesList[realThemeNumber];
			for (int l = 0; l < 6; l++)
			{
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Monsters/@MonsterObject"), (Singleton<DataManager>.instance.currentGameData.currentTheme <= 100) ? 5 : 7, "@MonsterObject", true);
				});
			}
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Monsters/@BossObject"), 1, "@BossObject", true);
			});
			if (realThemeNumber >= 10)
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/@DaemonKingForThemeEvent"), 1, "@DaemonKingForThemeEvent", true);
			}
			switch (realThemeNumber)
			{
			case 1:
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@QueenHornetBullet"), 8, "@QueenHornetBullet", true);
				});
				break;
			case 7:
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@FairyKingHit"), 1, "@FairyKingHit", true);
				});
				break;
			case 8:
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@LavadragonBullet"), 2, "@LavadragonBullet", true);
				});
				break;
			case 9:
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@GrimReaperHit"), 1, "@GrimReaperHit", true);
				});
				break;
			case 10:
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@QueenHornetBullet"), 8, "@QueenHornetBullet", true);
				});
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@FairyKingHit"), 1, "@FairyKingHit", true);
				});
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@LavadragonBullet"), 2, "@LavadragonBullet", true);
				});
				MultiThreadManager.RegistryTask(delegate
				{
					ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Bullets/@GrimReaperHit"), 1, "@GrimReaperHit", true);
				});
				break;
			}
			MultiThreadManager.RegistryTask(delegate
			{
				endLoadDelegateOnlyWorkingLoad();
			});
		}
		else
		{
			ObjectPool.DestroyDynamicCreatedPools();
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
	}

	private void registryToMultithread(string path, int count, string poolName)
	{
		int num = 1;
		if (count >= 30)
		{
			count /= 2;
			num = 2;
		}
		for (int i = 0; i < num; i++)
		{
			UIWindowIntro.instance.totalTaskCount++;
			UIWindowIntro.instance.totalCurrentTaskCount++;
			MultiThreadManager.RegistryTask(delegate
			{
				ObjectPool.CreatePool(Resources.Load<GameObject>(path), count, poolName);
				UIWindowIntro.instance.totalCurrentTaskCount--;
			});
		}
	}

	private void registryToMultithread(Action action)
	{
		UIWindowIntro.instance.totalTaskCount++;
		UIWindowIntro.instance.totalCurrentTaskCount++;
		action = (Action)Delegate.Combine(action, (Action)delegate
		{
			UIWindowIntro.instance.totalCurrentTaskCount--;
		});
		MultiThreadManager.RegistryTask(action.Invoke);
	}

	private void createObjectPools()
	{
		registryToMultithread("Prefabs/Ingame/Bullets/@NormalBullet", 20, "@NormalBullet");
		registryToMultithread("Prefabs/Ingame/@ExplosionEffect", 30, "@ExplosionEffect");
		registryToMultithread("Prefabs/Ingame/@AdsAngel", 1, "@AdsAngel");
		registryToMultithread("Prefabs/Ingame/Effect/@StartledEffect", 3, "@StartledEffect");
		registryToMultithread("Prefabs/Ingame/Effect/fx_result_get_star", 5, "fx_result_get_star");
		registryToMultithread("Prefabs/Ingame/Effect/@WeaponSkinInvinciblePersonEffect", 2, "@WeaponSkinInvinciblePersonEffect");
		registryToMultithread("Prefabs/Ingame/Effect/@TowerModeEvadeEffect", 2, "@TowerModeEvadeEffect");
		registryToMultithread("Prefabs/Ingame/UI/@PrincessCollectionSlot", 5, "@PrincessCollectionSlot");
		registryToMultithread("Prefabs/Ingame/UI/@FlyingResource", 20, "@FlyingResource");
		registryToMultithread("Prefabs/Ingame/UI/@PostItemSlot", 5, "@PostItemSlot");
		registryToMultithread("Prefabs/Ingame/UI/@WeaponSlot", 5, "@WeaponSlot");
		registryToMultithread("Prefabs/Ingame/UI/@ShopSlot", 15, "@ShopSlot");
		int count = (int)Math.Ceiling(10.399999618530273);
		registryToMultithread("Prefabs/Ingame/UI/@TreasureSlotSet", count, "@TreasureSlotSet");
		registryToMultithread("Prefabs/Ingame/UI/@AchievementSlot", 6, "@AchievementSlot");
		registryToMultithread("Prefabs/Ingame/UI/@CharacterSkinSlot", 5, "@CharacterSkinSlot");
		registryToMultithread("Prefabs/Ingame/UI/@ColleagueSkinSlot", 3, "@ColleagueSkinSlot");
		registryToMultithread("Prefabs/Ingame/UI/@MiniPopupObject", 3, "@MiniPopupObject");
		registryToMultithread("Prefabs/Ingame/UI/@SkillSlot", 6, "@SkillSlot");
		registryToMultithread("Prefabs/Ingame/UI/CharacterUIObjects/@CharacterUIObject", 31, "@CharacterUIObject");
		registryToMultithread("Prefabs/Ingame/UI/@EvasionText", 2, "@EvasionText");
		registryToMultithread("Prefabs/Ingame/UI/TypingText", 5, "TypingText");
		registryToMultithread("Prefabs/Ingame/UI/TextBalloon", 3, "TextBalloon");
		registryToMultithread("Prefabs/Ingame/UI/LongTextBalloon", 1, "LongTextBalloon");
		registryToMultithread("Prefabs/Ingame/UI/HeartBalloon", 1, "HeartBalloon");
		registryToMultithread("Prefabs/Ingame/UI/@WeaponSkinSlotSet", 10, "@WeaponSkinSlotSet");
		registryToMultithread("Prefabs/Ingame/Map/@EmptyLeftLine", 8, "@EmptyLeftLine");
		registryToMultithread("Prefabs/Ingame/Map/@EmptyRightLine", 8, "@EmptyRightLine");
		registryToMultithread("Prefabs/Ingame/Map/@Line", 24, "@Line");
		registryToMultithread("Prefabs/Ingame/Map/@MiniBossStage", 6, "@MiniBossStage");
		registryToMultithread("Prefabs/Ingame/Map/@BossStage", 1, "@BossStage");
		registryToMultithread("Prefabs/Ingame/Map/Torch", 20, "Torch");
		registryToMultithread("Prefabs/Ingame/Effect/@QuestDoubleBonus", 1, "@QuestDoubleBonus");
		registryToMultithread("Prefabs/Ingame/Effect/fx_skill_hero_priest3", 1, "fx_skill_hero_priest3");
		registryToMultithread("Prefabs/Ingame/Effect/fx_skill_hero_priest3_2", 1, "fx_skill_hero_priest3_2");
		registryToMultithread("Prefabs/Ingame/Effect/@DragonBreath", 8, "@DragonBreath");
		registryToMultithread("Prefabs/Ingame/Effect/@CastSkill", 5, "@CastSkill");
		registryToMultithread("Prefabs/Ingame/@ClonedWarrior", 1, "@ClonedWarrior");
		registryToMultithread("Prefabs/Ingame/Effect/BossAttack", 2, "BossAttack");
		registryToMultithread("Prefabs/Ingame/Effect/@WhirlWindEffectForPreview", 1, "@WhirlWindEffectForPreview");
		registryToMultithread("Prefabs/Ingame/Effect/@CollectEventRewardEffect", 1, "@CollectEventRewardEffect");
		registryToMultithread("Prefabs/Ingame/Effect/fx_character_upgrade", 5, "fx_character_upgrade");
		registryToMultithread("Prefabs/Ingame/Effect/fx_get_reward", 2, "fx_get_reward");
		registryToMultithread("Prefabs/Ingame/Effect/fx_skill_concentration_aura", 3, "fx_skill_concentration_aura");
		registryToMultithread("Prefabs/Ingame/Effect/fx_skill_divine_arrow_ground_hit", 3, "fx_skill_divine_arrow_ground_hit");
		registryToMultithread("Prefabs/Ingame/Effect/fx_rebirth_hero", 3, "fx_rebirth_hero");
		registryToMultithread("Prefabs/Ingame/@WarriorAttackEffect1", 10, "@WarriorAttackEffect1");
		registryToMultithread("Prefabs/Ingame/@WarriorAttackEffect2", 10, "@WarriorAttackEffect2");
		registryToMultithread("Prefabs/Ingame/@WarriorAttackEffect3", 3, "@WarriorAttackEffect3");
		registryToMultithread("Prefabs/Ingame/Effect/fx_hit_hero", 5, "fx_hit_hero");
		registryToMultithread("Prefabs/Ingame/Effect/fx_boss_blowup", 11, "fx_boss_blowup");
		registryToMultithread("Prefabs/Ingame/Effect/fx_boss_chest_shiny", 1, "fx_boss_chest_shiny");
		registryToMultithread("Prefabs/Ingame/Event/fx_snowstorm", 1, "fx_snowstorm");
		registryToMultithread("Prefabs/Ingame/Event/Kingsnowball", 2, "Kingsnowball");
		registryToMultithread("Prefabs/Ingame/Event/SnowObject1", 1, "SnowObject1");
		registryToMultithread("Prefabs/Ingame/Event/SnowObject2", 1, "SnowObject2");
		registryToMultithread("Prefabs/Ingame/Event/SnowObject3", 1, "SnowObject3");
		registryToMultithread("Prefabs/Ingame/Event/SlimeFace", 1, "SlimeFace");
		registryToMultithread("Prefabs/Ingame/Event/FallEffect", 1, "FallEffect");
		registryToMultithread("Prefabs/Ingame/Event/Reaper", 1, "Reaper");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeModeBackground", 1, "@ElopeModeBackground");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeModeEnemyCharacter", 5, "@ElopeModeEnemyCharacter");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeModeDaemonKing", 1, "@ElopeModeDaemonKing");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeModeGold", 6, "@ElopeModeGold");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeDamageText", 3, "@ElopeDamageText");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopePrincessSlot", 7, "@ElopePrincessSlot");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopePrincess", GameManager.maxPrincessCount, "@ElopePrincess");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeCart", 1, "@ElopeCart");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeResourceDropObject", 6, "@ElopeResourceDropObject");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeHeartCoin", 1, "@ElopeHeartCoin");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeDaemonCriticalEffect", 2, "@ElopeDaemonCriticalEffect");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeDaemonFaceChangeEffect", 2, "@ElopeDaemonFaceChangeEffect");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeDaemonFaceShyEffect", 1, "@ElopeDaemonFaceShyEffect");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopePrincessHeartEffect", 1, "@ElopePrincessHeartEffect");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopePrincessHeartEffect_Fast", 1, "@ElopePrincessHeartEffect_Fast");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeHitEffect", 3, "@ElopeHitEffect");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeUpgradeEffect", 5, "@ElopeUpgradeEffect");
		registryToMultithread("Prefabs/Ingame/ElopeMode/@ElopeDaemonUpgradeEffect", 5, "@ElopeDaemonUpgradeEffect");
		registryToMultithread("Prefabs/Ingame/UI/@ElopeDaemonKingSlot", 8, "@ElopeDaemonKingSlot");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeMap", 10, "@TowerModeMap");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeMapApex", 1, "@TowerModeMapApex");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeCharacter", 1, "@TowerModeCharacter");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeObstacle", 6, "@TowerModeObstacle");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeObstacleIndicator", 6, "@TowerModeObstacleIndicator");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeMonsterObject", 6, "@TowerModeMonsterObject");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeBossObject", 1, "@TowerModeBossObject");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeFastMonsterEffect", 2, "@TowerModeFastMonsterEffect");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeProjectile", 6, "@TowerModeProjectile");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeMiniBossMap", 2, "@TowerModeMiniBossMap");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeBossMap", 1, "@TowerModeBossMap");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeRankingSlot", 15, "@TowerModeRankingSlot");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeFlame", 2, "@TowerModeFlame");
		registryToMultithread("Prefabs/Ingame/TowerMode/@TowerModeSky", 1, "@TowerModeSky");
		registryToMultithread("Prefabs/Ingame/Effect/@HeartCoinObtainEffect", 3, "@HeartCoinObtainEffect");
		registryToMultithread("Prefabs/Ingame/Effect/@BestRecordEffect", 1, "@BestRecordEffect");
		registryToMultithread("Prefabs/Ingame/PVP/@PVPRankingSlot", 9, "@PVPRankingSlot");
		registryToMultithread("Prefabs/Ingame/PVP/@PVPHonorRankingSlot", 9, "@PVPHonorRankingSlot");
		registryToMultithread("Prefabs/Ingame/PVP/@PVPHistorySlot", 9, "@PVPHistorySlot");
		registryToMultithread(delegate
		{
			createColleaguePool();
		});
		registryToMultithread(delegate
		{
			createCharactersPool();
		});
		registryToMultithread(delegate
		{
			createWeaponsPool();
		});
		UIWindowIntro.instance.startLoading();
	}

	private void createColleaguePool()
	{
		for (int i = 0; i < 28; i++)
		{
			ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/UI/ColleagueUIObjects/@ColleagueUIObject" + (i + 1)), 1, "@ColleagueUIObject" + (i + 1));
			ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Colleagues/@Colleague" + (i + 1)), 1, "@Colleague" + (i + 1));
		}
		ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/UI/@ColleagueSlot"), 6, "@ColleagueSlot");
	}

	private void createWeaponsPool()
	{
		ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Weapons/@Weapon"), 3, "@Weapon");
		ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Weapons/@WeaponSkinAuraObject"), 3, "@WeaponSkinAuraObject");
	}

	private void createCharactersPool()
	{
		ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Characters/WarriorCharacters/CharacterWarrior"), 1, "@CharacterWarrior");
		ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Characters/ArcherCharacters/CharacterArcher"), 1, "@CharacterArcher");
		ObjectPool.CreatePool(Resources.Load<GameObject>("Prefabs/Ingame/Characters/PriestCharacters/CharacterPriest"), 1, "@CharacterPriest");
	}

	private void setSprites()
	{
		animationCache = new List<SpriteDictionary>();
		List<Sprite> list = new List<Sprite>();
		animationCache.Add(new SpriteDictionary("Intro", getSpritesInSprites(list.ToArray(), "Intro")));
		setCharacterSprites(ingameAtlas);
		setMonsterSprites(ingameAtlas);
		setBulletSprites(ingameAtlas);
		setThemeSprites(themeAtlas);
		animationCache.Add(new SpriteDictionary("Effect", getSpritesInSprites(ingameAtlas, "Effect")));
		animationCache.Add(new SpriteDictionary("BigGold", getSpritesInSprites(ingameAtlas, "BigGold")));
		animationCache.Add(new SpriteDictionary("UI", getSpritesInSprites(ingameAtlas, "UI")));
		animationCache.Add(new SpriteDictionary("fall", getSpritesInSprites(ingameAtlas, "fall")));
		animationCache.Add(new SpriteDictionary("SkillDragon", getSpritesInSprites(ingameAtlas, "SkillDragon")));
		animationCache.Add(new SpriteDictionary("PassiveSkillEffect", getSpritesInSprites(ingameAtlas, "PassiveSkillEffect")));
		for (int i = 0; i < (int)(long)PVPManager.getTankMaxCount(); i++)
		{
			animationCache.Add(new SpriteDictionary("Tank" + (i + 1), getSpritesInSprites(ingameAtlas, "Tank" + (i + 1))));
		}
	}

	private void setBulletSprites(Sprite[] ingameAtlas)
	{
		animationCache.Add(new SpriteDictionary("Bullet", getSpritesInSprites(ingameAtlas, "Bullet")));
	}

	private void setMonsterSprites(Sprite[] ingameAtlas)
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					string text = ((EnemyManager.MonsterType)(i * 20 + j * 4 + k)).ToString();
					animationCache.Add(new SpriteDictionary(text, getSpritesInSprites(ingameAtlas, text)));
				}
			}
			string text2 = ((EnemyManager.BossType)i).ToString();
			animationCache.Add(new SpriteDictionary(text2, getSpritesInSprites(ingameAtlas, text2)));
			if (i > 5)
			{
				animationCache.Add(new SpriteDictionary(text2 + "Elite", getSpritesInSprites(ingameAtlas, text2 + "Elite")));
			}
		}
		animationCache.Add(new SpriteDictionary("Daemon1", getSpritesInSprites(ingameAtlas, "Daemon1")));
		animationCache.Add(new SpriteDictionary("Daemon1Elite", getSpritesInSprites(ingameAtlas, "Daemon1Elite")));
	}

	private void setCharacterSprites(Sprite[] ingameAtlas)
	{
		for (int i = 0; i < GameManager.maxPrincessCount; i++)
		{
			animationCache.Add(new SpriteDictionary("Princess" + (i + 1), getSpritesInSprites(ingameAtlas, "Princess" + (i + 1))));
		}
	}

	private void setThemeSprites(Sprite[] themeAtlas)
	{
		for (int i = 1; i <= 10; i++)
		{
			animationCache.Add(new SpriteDictionary("Stage" + i + "Block_ground", getSpritesInSpritesForTheme(themeAtlas, "Stage" + i + "-Block")));
			animationCache.Add(new SpriteDictionary("Stage" + i + "Castle", getSpritesInSpritesForTheme(themeAtlas, "Stage" + i + "-Castle")));
			animationCache.Add(new SpriteDictionary("Stage" + i + "Castle2", getSpritesInSpritesForTheme(themeAtlas, "Stage" + i + "-Castle2")));
			animationCache.Add(new SpriteDictionary("Stage" + i + "Floor", getSpritesInSpritesForTheme(themeAtlas, "Stage" + i + "-Floor")));
			animationCache.Add(new SpriteDictionary("Stage" + i + "Boss", getSpritesInSpritesForTheme(themeAtlas, "Stage" + i + "-Boss")));
			animationCache.Add(new SpriteDictionary("Stage" + i + "Stair", getSpritesInSpritesForTheme(themeAtlas, "Stage" + i + "-Stair")));
			if (i != 6)
			{
				animationCache.Add(new SpriteDictionary("Candle" + i, getSpritesInSprites(themeAtlas, "Candle" + i)));
			}
		}
	}

	public Sprite[] getSpritesInSpritesForTheme(Sprite[] sprites, string prefix)
	{
		return sprites.Where((Sprite t) => t.name.StartsWith(prefix)).ToArray();
	}

	public Sprite[] getSpritesInSprites(Sprite[] sprites, string prefix)
	{
		return sprites.Where((Sprite t) => t.name.StartsWith(prefix + "-")).ToArray();
	}

	public void removeFromDictionary(string animationType)
	{
		for (int i = 0; i < animationCache.Count; i++)
		{
			if (animationCache[i].name == animationType)
			{
				animationCache.Remove(animationCache[i]);
				break;
			}
		}
	}

	public Sprite[] getAnimation(string animation)
	{
		Sprite[] result = null;
		for (int i = 0; i < animationCache.Count; i++)
		{
			if (animationCache[i].name.Equals(animation))
			{
				result = animationCache[i].sprites;
			}
		}
		return result;
	}
}
