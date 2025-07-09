using UnityEngine;

public class NumberText : ObjectBase
{
	public SpriteRenderer m_cachedSpriteRenderer;

	public void setSprites(Sprite targetSprite)
	{
		setSprites(targetSprite, Color.white);
	}

	public void setSprites(Sprite targetSprite, Color color)
	{
		m_cachedSpriteRenderer.color = color;
		m_cachedSpriteRenderer.sprite = targetSprite;
	}
}
