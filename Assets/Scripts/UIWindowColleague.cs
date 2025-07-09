using TMPro;
using UnityEngine;

public class UIWindowColleague : UIWindow
{
	public static UIWindowColleague instance;

	public InfiniteScroll colleagueScrollRectParent;

	public TextMeshProUGUI totalDamageText;

	public bool isInitializedSlots;

	public GameObject skinDescriptionObject;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		if (!isInitializedSlots)
		{
			isInitializedSlots = true;
		}
		refreshTotalDamage();
		return base.OnBeforeOpen();
	}

	public void refreshSkinDescriptionObject()
	{
		if (Singleton<ColleagueManager>.instance.getColleagueInventoryData(ColleagueManager.ColleagueType.Isabelle).isUnlocked && colleagueScrollRectParent.parentScrollRect.content.anchoredPosition.y < 35f && !TutorialManager.isTutorial)
		{
			bool isSeenColleagueSkinDescription = Singleton<DataManager>.instance.currentGameData.isSeenColleagueSkinDescription;
			if (skinDescriptionObject.activeSelf != !isSeenColleagueSkinDescription)
			{
				skinDescriptionObject.SetActive(!isSeenColleagueSkinDescription);
			}
		}
		else if (skinDescriptionObject.activeSelf)
		{
			skinDescriptionObject.SetActive(false);
		}
	}

	private void Update()
	{
		refreshSkinDescriptionObject();
	}

	public override void OnAfterActiveGameObject()
	{
		colleagueScrollRectParent.refreshAll();
		base.OnAfterActiveGameObject();
	}

	public void refreshTotalDamage()
	{
		double num = 0.0;
		for (int i = 0; i < 28; i++)
		{
			ColleagueInventoryData colleagueInventoryData = Singleton<DataManager>.instance.currentGameData.colleagueInventoryList[i];
			if (colleagueInventoryData.isUnlocked)
			{
				num += Singleton<ColleagueManager>.instance.getColleagueDamage(colleagueInventoryData.colleagueType, colleagueInventoryData.level, true, 1f);
			}
		}
		totalDamageText.text = GameManager.changeUnit(num);
	}
}
