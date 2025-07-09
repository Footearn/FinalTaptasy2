public class UIWindowQuest : UIWindow
{
	public static UIWindowQuest instance;

	public QuestObject[] questObjects;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		for (int i = 0; i < questObjects.Length; i++)
		{
			questObjects[i].LoadQuest(i, false);
		}
		refreshCompleteNotification();
		return base.OnBeforeOpen();
	}

	public void refreshCompleteNotification()
	{
		UIWindowOutgame.instance.refreshQuestCompleteIndicator();
	}

	public void OnClickClose()
	{
		close();
	}
}
