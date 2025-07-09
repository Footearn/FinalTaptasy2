using UnityEngine;

public class LineBackground : ObjectBase
{
	public Sprite[] backgroundSprites;

	public SpriteRenderer background;

	public int bgType;

	public void Set(int floor)
	{
		backgroundSprites = Singleton<ResourcesManager>.instance.getAnimation(MapManager.prefixStage + "Floor");
		if (GameManager.currentTheme > 200)
		{
			switch (GameManager.getRealThemeNumber(GameManager.currentTheme))
			{
			case 1:
			case 2:
				background.color = Util.getCalculatedColor(169f, 155f, 255f);
				break;
			case 5:
				background.color = Util.getCalculatedColor(197f, 210f, 246f);
				break;
			case 8:
				background.color = Util.getCalculatedColor(255f, 180f, 180f);
				break;
			case 10:
				background.color = Util.getCalculatedColor(236f, 226f, 255f);
				break;
			}
		}
		else
		{
			background.color = Color.white;
		}
		bgType = floor % (backgroundSprites.Length - 1);
		background.sprite = backgroundSprites[bgType];
	}
}
