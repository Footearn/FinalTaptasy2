using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CachedManager : Singleton<CachedManager>
{
	public EventSystem eventSystem;

	public Transform ingameSetTransform;

	public Transform cameraParentTransform;

	public Camera ingameCamera;

	public ShakeCamera ingameCameraShake;

	public Transform effectCamera;

	public Transform previewCamera;

	public UIWindowIngame uiWindowIngame;

	public UIwindowIngameField uiWindowField;

	public UIWindowResult uiWindowResult;

	public UIWindowOutgame uiWindowOutgame;

	public EnemyInformation[] enemyInformations;

	public EnemyInformation bossInformation;

	public Texture[] characterTextures;

	public HPProgress mainHPProgressBar;

	public Transform cloudTransform;

	public Transform monster;

	public Transform boss;

	public Transform colleagueTransform;

	public Transform bossRaidFollowTargetTransform;

	public Transform bossRaidBackgroundParentTrnasform;

	public Transform uiIngameFieldWrapperTransform;

	public RectTransform canvasAlwaysTopUICenterTransform;

	public Transform characterTransform;

	public LastPrincess princess;

	public Jail jail;

	public EmotionUI emotionUI;

	public FadeUI coverUI;

	public FadeUI ingameCoverUI;

	public FadeUI darkUI;

	public FadeUI whiteCoverUI;

	public Transform backgroundTransform;

	public Transform ingameBackgroundTransform;

	public Transform emptyLeftLinesTransform;

	public Transform emptyRightLinesTransform;

	public Text[] friendsStatsText;

	public Text[] friendsLevelUpStatsText;

	public Text[] friendsPriceText;

	public TextureRendererCamera warriorTextureRendererCamera;

	public TextureRendererCamera archerTextureRendererCamera;

	public TextureRendererCamera priestTextureRendererCamera;

	public SpriteRenderer themebackground;

	public GameObject themeEventGround;

	public GameObject theme6EventBlank;

	public GameObject theme6EventGround;

	public Sprite[] arrowSprites;

	public SpriteGroup townSpriteGroup;

	public Sprite selectedBackgroundSprite;

	public GameObject sunBurstEffect;

	public Transform dummyTransform;

	public Sprite warriorSecondStatIconSprite;

	public Sprite priestSecondStatIconSprite;

	public Sprite archerSecondStatIconSprite;

	public Sprite disableButtonSprite;

	public Sprite enableButtonOrangeSprite;

	public Sprite enableButtonBlueSprite;

	public Sprite enableButtonGreenSprite;

	public Sprite enableButtonPurpleSprite;

	public Sprite enableThumbnailFellowSprite;

	public Sprite enableThumbnailWeaponSprite;

	public Sprite enableThumbnailWeaponEquippedSprite;

	public Sprite enableThumbnailCharacterSkinEquippedSprite;

	public Sprite characterTabOnSprite;

	public Sprite disableThumbnailSprite;

	public Sprite limitedItemBackground;

	public Sprite skinItemBackground;

	public Sprite goldItemBackground;

	public Sprite rubyItemBackground;

	public Sprite[] achievementProgressingSprites;

	public Sprite[] achievementCompleteSprites;

	public Sprite settingOrangeButton;

	public Sprite settingGrayButton;

	public Sprite settingRedButton;

	public Sprite shopPopupTabNoneSelectSprite;

	public Sprite shopPopupTabSelectedSprite;

	public Sprite princessSlotNormalBackgroundSprite;

	public Sprite princessRescueingBackgroundSprite;

	public Sprite collectEventRecievedBackgroundSprite;

	public Sprite collectEventBeforeRecieveBackgroundSprite;

	public Sprite[] frozenGroundSprites;

	public Sprite[] frozenStairSprites;

	public CanvasGroup[] alwaysTopCanvasUICanvasGroup;

	public Sprite elopeModeNonSelectedSprite;

	public Sprite elopeModeSelectedSprite;

	public Sprite towerModeSeasonAndHonorTabSelectedSprite;

	public Sprite towerModeSeasonAndHonorTabNonSelectedSprite;

	public Sprite warriorIconSprite;

	public Sprite preistIconSprite;

	public Sprite archerIconSprite;

	public GameObject ingameSetObject;

	public Sprite newSkinTitleImage;

	public Sprite levelUpSkinTitleImage;

	public Sprite winSprite;

	public Sprite loseSprite;

	public Sprite rubyIconImage;

	public Sprite heartCoinIconImage;

	public Sprite pvpTabSelectedSprite;

	public Sprite pvpTabNonSelectedSprite;

	public Sprite pvpHistoryAttackIconSprite;

	public Sprite pvpHistoryDefenceIconSprite;

	public Sprite pvpSeasonRewardOnSprite;

	public Sprite pvpSeasonRewardOffSprite;

	public Sprite pvpHonorTokenSprite;

	public Sprite[] tankUnlockIconSprites;

	public Sprite[] tankUpgradeIconSprites;

	public Camera pvpCamera;
}
