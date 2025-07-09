using System.Collections;
using UnityEngine;

public class UIWindowSkill : UIWindow
{
	public static UIWindowSkill instance;

	public Sprite[] skillIcon;

	public Sprite[] skillLockedIcon;

	public Sprite[] passiveSkillIcon;

	public Sprite[] passiveSkillLockedIcon;

	public Sprite[] reinforcementSkillIcon;

	public Sprite[] reinforcementSkillLockedIcon;

	public Sprite[] autoTouchSkillIcons;

	public RectTransform listTab;

	public InfiniteScroll skillScroll;

	public RectTransform previewTab;

	public SkillInventoryData currentSkillData;

	private int currentSkillIdx;

	private float alpha;

	private UIWindowOutgame outgame;

	public override void Awake()
	{
		instance = this;
		int num = 5;
		int num2 = 5;
		int num3 = 5;
		int num4 = 3;
		int num5 = 3;
		skillScroll.refreshMaxCount(num + num2 + num3 + num4 + num5);
		base.Awake();
	}

	public override bool OnBeforeOpen()
	{
		Singleton<StatManager>.instance.refreshAllStats();
		outgame = UIWindowOutgame.instance;
		StopAllCoroutines();
		previewTab.anchoredPosition = new Vector2(width, 756.1f);
		return base.OnBeforeOpen();
	}

	public override void OnAfterActiveGameObject()
	{
		skillScroll.refreshAll();
		base.OnAfterActiveGameObject();
	}

	public override bool OnBeforeClose()
	{
		outgame.previewObject.SetActive(false);
		Singleton<SkillManager>.instance.stopSkillPreview(currentSkillData.skillType);
		outgame.previewGroup.alpha = 0f;
		outgame.topLeftGroup.alpha = 1f;
		outgame.toprightGroup.alpha = 1f;
		currentSkillData = new SkillInventoryData();
		return base.OnBeforeClose();
	}

	public void OnClickPreview(SkillInventoryData skillData)
	{
		if (skillData.skillType != currentSkillData.skillType)
		{
			Singleton<SkillManager>.instance.stopSkillPreview(currentSkillData.skillType);
			currentSkillData = skillData;
			currentSkillIdx = (int)skillData.skillType;
			StopAllCoroutines();
			StartCoroutine("PreviewOpen");
		}
	}

	public void OnClickClose()
	{
		StopAllCoroutines();
		StartCoroutine("PreviewClose");
	}

	private IEnumerator PreviewOpen()
	{
		outgame.previewObject.SetActive(true);
		Singleton<SkillManager>.instance.castSkillPreview(currentSkillData.skillType);
		do
		{
			alpha = (width - previewTab.anchoredPosition.x) / width;
			outgame.previewGroup.alpha = alpha;
			outgame.topLeftGroup.alpha = 1f - alpha * 0.5f;
			outgame.toprightGroup.alpha = 1f - alpha * 0.5f;
			previewTab.anchoredPosition = Vector2.Lerp(previewTab.anchoredPosition, new Vector2(0f, 756.1f), Time.deltaTime * GameManager.timeScale * 12f);
			yield return null;
		}
		while (alpha < 0.99f);
		alpha = 1f;
		outgame.previewGroup.alpha = alpha;
		outgame.topLeftGroup.alpha = 1f - alpha * 0.5f;
		outgame.toprightGroup.alpha = 1f - alpha * 0.5f;
	}

	private IEnumerator PreviewClose()
	{
		Singleton<SkillManager>.instance.stopSkillPreview(currentSkillData.skillType);
		do
		{
			alpha = (width - previewTab.anchoredPosition.x) / width;
			outgame.previewGroup.alpha = alpha;
			outgame.topLeftGroup.alpha = 1f - alpha * 0.5f;
			outgame.toprightGroup.alpha = 1f - alpha * 0.5f;
			previewTab.anchoredPosition = Vector2.Lerp(previewTab.anchoredPosition, new Vector2(width, 756.1f), Time.deltaTime * GameManager.timeScale * 12f);
			yield return null;
		}
		while (alpha > 0.01f);
		outgame.previewObject.SetActive(false);
		alpha = 0f;
		outgame.previewGroup.alpha = alpha;
		outgame.topLeftGroup.alpha = 1f - alpha * 0.5f;
		outgame.toprightGroup.alpha = 1f - alpha * 0.5f;
		currentSkillData = new SkillInventoryData();
	}
}
