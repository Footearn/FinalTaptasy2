using UnityEngine;

public class CharacterRebirthAnimation : MonoBehaviour
{
	public Sprite firstFrameSprite;

	public Sprite thirdFrameSprite;

	public SpriteAnimation characterAnimation;

	public void startFadeIn()
	{
		UIWindowProgressRebirth.instance.closeRebirth();
	}

	public void playAnimation()
	{
		characterAnimation.playAnimation("RebirthHero", 0.18f);
	}

	public void setSpriteThirdFrame()
	{
		characterAnimation.stopAnimation();
		characterAnimation.targetImage.sprite = thirdFrameSprite;
		characterAnimation.targetImage.SetNativeSize();
	}

	public void setSpriteFirstFrame()
	{
		characterAnimation.stopAnimation();
		characterAnimation.targetImage.sprite = firstFrameSprite;
		characterAnimation.targetImage.SetNativeSize();
	}
}
