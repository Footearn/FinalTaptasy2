using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InitAndroidInventoryTask : MonoBehaviour
{
	[method: MethodImpl(32)]
	public event Action ActionComplete = delegate
	{
	};

	[method: MethodImpl(32)]
	public event Action ActionFailed = delegate
	{
	};

	public static InitAndroidInventoryTask Create()
	{
		return new GameObject("InitAndroidInventoryTask").AddComponent<InitAndroidInventoryTask>();
	}

	public void Run()
	{
		Debug.Log("InitAndroidInventoryTask task started");
		if (AndroidInAppPurchaseManager.Client.IsConnected)
		{
			OnBillingConnected(null);
			return;
		}
		AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
		if (!AndroidInAppPurchaseManager.Client.IsConnectingToServiceInProcess)
		{
			AndroidInAppPurchaseManager.Client.Connect();
		}
	}

	private void OnBillingConnected(BillingResult result)
	{
		Debug.Log("OnBillingConnected");
		if (result == null)
		{
			OnBillingConnectFinished();
			return;
		}
		AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;
		if (result.IsSuccess)
		{
			OnBillingConnectFinished();
			return;
		}
		Debug.Log("OnBillingConnected Failed");
		this.ActionFailed();
	}

	private void OnBillingConnectFinished()
	{
		Debug.Log("OnBillingConnected COMPLETE");
		if (AndroidInAppPurchaseManager.Client.IsInventoryLoaded)
		{
			Debug.Log("IsInventoryLoaded COMPLETE");
			this.ActionComplete();
			return;
		}
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
		if (!AndroidInAppPurchaseManager.Client.IsProductRetrievingInProcess)
		{
			AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
		}
	}

	private void OnRetrieveProductsFinised(BillingResult result)
	{
		Debug.Log("OnRetrieveProductsFinised");
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
		if (result.IsSuccess)
		{
			Debug.Log("OnRetrieveProductsFinised COMPLETE");
			this.ActionComplete();
		}
		else
		{
			Debug.Log("OnRetrieveProductsFinised FAILED");
			this.ActionFailed();
		}
	}
}
