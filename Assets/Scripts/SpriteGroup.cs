using System.Collections.Generic;
using UnityEngine;

public class SpriteGroup : ObjectBase
{
	public float alpha = 1f;

	public List<SpriteRendererColorData> spriteRendererDataList;

	private void Reset()
	{
		spriteRendererDataList = new List<SpriteRendererColorData>();
		resetData();
	}

	public void resetData()
	{
		spriteRendererDataList.Clear();
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>(true);
		TextMesh[] componentsInChildren2 = GetComponentsInChildren<TextMesh>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			spriteRendererDataList.Add(new SpriteRendererColorData(componentsInChildren[i], null, componentsInChildren[i].color));
		}
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			spriteRendererDataList.Add(new SpriteRendererColorData(null, componentsInChildren2[j], componentsInChildren2[j].color));
		}
	}

	public void setAlpha(float targetValue)
	{
		alpha = Mathf.Max(targetValue, 0f);
		for (int i = 0; i < spriteRendererDataList.Count; i++)
		{
			if (spriteRendererDataList[i].spriteRenderer != null)
			{
				Color originColor = spriteRendererDataList[i].originColor;
				originColor.a *= alpha;
				spriteRendererDataList[i].spriteRenderer.color = originColor;
			}
			else if (spriteRendererDataList[i].textMesh != null)
			{
				Color originColor2 = spriteRendererDataList[i].originColor;
				originColor2.a *= alpha;
				spriteRendererDataList[i].textMesh.color = originColor2;
			}
		}
	}
}
