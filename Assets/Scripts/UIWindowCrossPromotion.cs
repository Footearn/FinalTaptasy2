using UnityEngine;
using UnityEngine.UI;

public class UIWindowCrossPromotion : UIWindow
{
	public static UIWindowCrossPromotion instance;

	public Image promotionImage;

	public RectTransform promotionImageTransform;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openPromotionUI(Sprite promotionSprite)
	{
		if (Singleton<NanooAPIManager>.instance != null && Singleton<NanooAPIManager>.instance.GetPromotionInfo != null)
		{
			promotionImage.sprite = promotionSprite;
			float num = float.Parse(Singleton<NanooAPIManager>.instance.GetPromotionInfo.image_width);
			float num2 = float.Parse(Singleton<NanooAPIManager>.instance.GetPromotionInfo.image_height);
			CanvasScaler component = GetComponent<CanvasScaler>();
			float num3 = component.referenceResolution.x * 0.8f;
			float y = num3 * num2 / num;
			promotionImageTransform.sizeDelta = new Vector2(num3, y);
			open();
		}
	}

	public void OnClickEnterURL()
	{
		Application.OpenURL(Singleton<NanooAPIManager>.instance.GetPromotionInfo.url);
	}
}
