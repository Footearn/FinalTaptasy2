using UnityEngine;
using UnityEngine.UI;

public class UIWindowRebirth : UIWindow
{
	public static UIWindowRebirth instance;

	public Text totalRewardKeyText;

	public Text currentStageText;

	public Text rebirthRequireThemeDescriptionText;

	public GameObject canRebirthObject;

	public GameObject canNotRebirthObject;

	public GameObject descriptionObject;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		if ((long)Mathf.Min(Singleton<DataManager>.instance.currentGameData.rebirthCount + 1, 10f) >= 10)
		{
			descriptionObject.SetActive(false);
		}
		else
		{
			descriptionObject.SetActive(true);
		}
		rebirthRequireThemeDescriptionText.text = string.Format(I18NManager.Get("CANNOT_REBIRTH"), Singleton<RebirthManager>.instance.currentRebirthRequireTheme);
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(true);
		UIWindowManageTreasure.instance.rebirthIndigator.SetActive(false);
		currentStageText.text = I18NManager.Get("CURRENT_STAGE") + " : <color=white>" + Singleton<DataManager>.instance.currentGameData.unlockTheme + "-" + Singleton<DataManager>.instance.currentGameData.unlockStage + "</color>";
		totalRewardKeyText.text = "x" + Singleton<RebirthManager>.instance.currentRebirthKeyRewardValue.ToString("N0");
		if (Singleton<DataManager>.instance.currentGameData.unlockTheme > Singleton<RebirthManager>.instance.currentRebirthRequireTheme)
		{
			canRebirthObject.SetActive(true);
			canNotRebirthObject.SetActive(false);
			totalRewardKeyText.color = Color.white;
		}
		else
		{
			canRebirthObject.SetActive(false);
			canNotRebirthObject.SetActive(true);
			totalRewardKeyText.color = Util.getCalculatedColor(255f, 96f, 96f);
		}
		return base.OnBeforeOpen();
	}

	public override bool OnBeforeClose()
	{
		Singleton<CachedManager>.instance.sunBurstEffect.SetActive(false);
		return base.OnBeforeClose();
	}

	public override void OnAfterOpen()
	{
		if (TutorialManager.isTutorial)
		{
			Singleton<TutorialManager>.instance.checkTutorialState(TutorialManager.TutorialType.RebirthTutorialType5);
		}
		base.OnAfterOpen();
	}
}
