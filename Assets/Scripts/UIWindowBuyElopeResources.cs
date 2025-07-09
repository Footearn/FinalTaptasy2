using System;
using UnityEngine.UI;

public class UIWindowBuyElopeResources : UIWindow
{
	public static UIWindowBuyElopeResources instance;

	public Text priceText;

	private Action m_okAction;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openBuyElopeResources(Action okAction)
	{
		string marketPrice = Singleton<PaymentManager>.instance.GetMarketPrice(ShopManager.ElopeModeItemType.ElopeResources);
		if (string.IsNullOrEmpty(marketPrice))
		{
			priceText.text = "$3.99";
		}
		else
		{
			priceText.text = marketPrice;
		}
		m_okAction = okAction;
		open();
	}

	public void OnClickOKButton()
	{
		if (m_okAction != null)
		{
			m_okAction();
			m_okAction = null;
		}
		close();
	}
}
