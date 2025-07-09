using UnityEngine;

public class PrincessKidnappedAngelAnimationObject : MonoBehaviour
{
	public void animationEndEvent()
	{
		Singleton<TutorialManager>.instance.isEndAngelAnimation = true;
	}
}
