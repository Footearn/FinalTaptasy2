using UnityEngine;
using UnityEngine.UI;

public class IngameQuestObject : ObjectBase
{
	public Image questIcon;

	public Text questText;

	public CanvasGroup cachedCanvasGroup;

	public GameObject questComplete;

	private bool isVisible;

	private QuestManager m_questManager
	{
		get
		{
			return Singleton<QuestManager>.instance;
		}
	}

	private void Start()
	{
		initialize();
	}

	public void initialize()
	{
		for (int i = 0; i < Singleton<QuestManager>.instance.questData.Count; i++)
		{
			if (Singleton<QuestManager>.instance.getQuestIngame(Singleton<QuestManager>.instance.questData[i].questType) && !Singleton<QuestManager>.instance.questData[i].isComplete)
			{
				questReport(Singleton<QuestManager>.instance.questData[i]);
				isVisible = true;
				cachedCanvasGroup.alpha = 1f;
				break;
			}
		}
		if (!isVisible)
		{
			cachedCanvasGroup.alpha = 0f;
		}
	}

	public void questReport(QuestData data)
	{
		questIcon.sprite = m_questManager.questIconSprites[(int)data.questType];
		questIcon.transform.localScale = Vector2.one * 0.5f;
		questIcon.SetNativeSize();
		if (data.questType == QuestManager.QuestType.Gold)
		{
			questText.text = GameManager.changeUnit(data.questValue) + " / " + GameManager.changeUnit(data.questGoal);
		}
		else
		{
			questText.text = (long)data.questValue + "/" + (long)data.questGoal;
		}
		if (data.isComplete)
		{
			questComplete.SetActive(true);
			questText.gameObject.SetActive(false);
		}
		else
		{
			questComplete.SetActive(false);
			questText.gameObject.SetActive(true);
		}
	}
}
