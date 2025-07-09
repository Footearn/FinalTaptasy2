using System;
using UnityEngine;

[Serializable]
public struct SpriteRendererColorData
{
	public SpriteRenderer spriteRenderer;

	public TextMesh textMesh;

	public Color originColor;

	public SpriteRendererColorData(SpriteRenderer spriteRenderer, TextMesh textMesh, Color originColor)
	{
		this.spriteRenderer = spriteRenderer;
		this.textMesh = textMesh;
		this.originColor = originColor;
	}
}
