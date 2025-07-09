using UnityEngine;

public class UIWindowManager : MonoBehaviour
{
	public static bool isEscapeBlock;

	public static bool isOpeningStageList;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !TutorialManager.isTutorial)
		{
			if (isEscapeBlock)
			{
				return;
			}
			UIWindow topWindow = UIWindow.GetTopWindow();
			if ((!(topWindow != null) || GameManager.isWaitForStartGame) && GameManager.currentDungeonType != GameManager.SpecialDungeonType.ElopeMode && GameManager.currentDungeonType != GameManager.SpecialDungeonType.TowerMode)
			{
				return;
			}
			if (topWindow == UIWindow.Get("UI Notice"))
			{
				UIWindowNotice.instance.close();
			}
			else if (topWindow == UIWindow.Get("UI Balance"))
			{
				UIWindowBalance.instance.close();
			}
			else if (topWindow == UIWindow.Get("UI SurpriseLimitedItem"))
			{
				UIWindowSurpriseLimitedItem.instance.OnClickClose();
			}
			else if (topWindow == UIWindow.Get("UI Setting") && UIWindowSetting.instance.isOpenNextPage)
			{
				UIWindowSetting.instance.closeNextPageUI();
			}
			else if (!isOpeningStageList && topWindow == UIWindow.Get("UI Worldmap") && GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				topWindow.close();
			}
			else if (GameManager.currentGameState != GameManager.GameState.OutGame && topWindow != UIWindow.Get("UI JackPot") && GameManager.currentDungeonType == GameManager.SpecialDungeonType.NormalDungeon)
			{
				if (topWindow.isCanCloseESC)
				{
					topWindow.close();
				}
				else if (!TutorialManager.isTutorial && UIWindowIngame.instance.isCanPause)
				{
					Singleton<GameManager>.instance.Pause(!GameManager.isPause);
				}
			}
			else if (topWindow == UIWindow.Get("UI Review"))
			{
				UIWindowReview.instance.OnClickLater();
			}
			else if (topWindow == UIWindow.Get("UI AdsAngelBuffDialog"))
			{
				UIWindowAdsAngelBuffDialog.instance.onClickCancel();
			}
			else if (topWindow == UIWindow.Get("UI Outgame") || topWindow == UIWindow.Get("UI Colleague") || topWindow == UIWindow.Get("UI ManageHeroAndWeapon") || topWindow == UIWindow.Get("UI ManageShop") || topWindow == UIWindow.Get("UI ManageTreasure") || topWindow == UIWindow.Get("UI Skill"))
			{
				UIWindowDialog.openDescription("EXIT_GAME", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					Singleton<DataManager>.instance.saveData();
					Application.Quit();
				}, string.Empty);
			}
			else if (topWindow == UIWindow.Get("UI WeaponSkinChangeSkin"))
			{
				if (UIWindowWeaponSkinChangeSkin.instance.isOpenWeaponSkinSelectUI)
				{
					UIWindowWeaponSkinChangeSkin.instance.closeWeaponSkinScroll();
				}
				else if (UIWindowWeaponSkinChangeSkin.instance.isCanCloseESC)
				{
					UIWindowWeaponSkinChangeSkin.instance.close();
				}
			}
			else if (topWindow.isCanCloseESC)
			{
				topWindow.close();
			}
			else if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.ElopeMode)
			{
				if (topWindow == UIWindow.Get("UI ElopeShop"))
				{
					topWindow.close();
				}
				else if (topWindow == UIWindow.Get("UI ElopeMode"))
				{
					Singleton<ElopeModeManager>.instance.exitElopeModeWithDialog();
				}
			}
			else if (GameManager.currentDungeonType == GameManager.SpecialDungeonType.TowerMode && topWindow == UIWindow.Get("UI TowerMode"))
			{
				Singleton<GameManager>.instance.Pause(!GameManager.isPause);
			}
		}
		else
		{
			if (!Input.GetKeyDown(KeyCode.Escape) || !TutorialManager.isTutorial)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < UIWindowDialog.instances.Length; i++)
			{
				if (UIWindowDialog.instances[i].isOpen)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				for (int j = 0; j < UIWindowDialog.instances.Length; j++)
				{
					UIWindowDialog.instances[j].close();
				}
			}
			else if (UIWindowIntro.instance != null && UIWindowIntro.instance.gameObject.activeSelf)
			{
				UIWindowDialog.openDescription("EXIT_GAME", UIWindowDialog.DialogState.SelectBetweenYesOrNoUI, delegate
				{
					Singleton<DataManager>.instance.saveData();
					Application.Quit();
				}, string.Empty);
			}
			else
			{
				Singleton<TutorialManager>.instance.skipTutorial();
			}
		}
	}
}
