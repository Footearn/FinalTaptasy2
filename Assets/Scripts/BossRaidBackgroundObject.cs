using UnityEngine;

public class BossRaidBackgroundObject : ObjectBase
{
	public SpriteRenderer[] groundGroundRenderer;

	public SpriteRenderer mainBackgroundRenderer;

	public SpriteRenderer underGroundRenderer;

	public SpriteRenderer topBackgroundRenderer;

	public GameObject[] themeTorchSet;

	public void init(int targetTheme)
	{
		for (int i = 0; i < themeTorchSet.Length; i++)
		{
			if (themeTorchSet[i].activeSelf)
			{
				themeTorchSet[i].SetActive(false);
			}
		}
		themeTorchSet[targetTheme - 1].SetActive(true);
		string str = "Stage" + targetTheme;
		for (int j = 0; j < groundGroundRenderer.Length; j++)
		{
			groundGroundRenderer[j].sprite = Singleton<ResourcesManager>.instance.getAnimation(str + "Block_ground")[0];
		}
		mainBackgroundRenderer.sprite = Singleton<ResourcesManager>.instance.getAnimation(str + "Boss")[0];
		topBackgroundRenderer.sprite = Singleton<ResourcesManager>.instance.getAnimation(str + "Boss")[0];
		underGroundRenderer.sprite = Singleton<ResourcesManager>.instance.getAnimation(str + "Boss")[2];
	}
}
