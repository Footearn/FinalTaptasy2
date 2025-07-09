using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AN_InAppAndroidClient : MonoBehaviour, AN_InAppClient
{
	private string _processedSKU;

	private AndroidInventory _inventory;

	private bool _IsConnectingToServiceInProcess;

	private bool _IsProductRetrievingInProcess;

	private bool _IsConnected;

	private bool _IsInventoryLoaded;

	public AndroidInventory Inventory
	{
		get
		{
			return _inventory;
		}
	}

	public bool IsConnectingToServiceInProcess
	{
		get
		{
			return _IsConnectingToServiceInProcess;
		}
	}

	public bool IsProductRetrievingInProcess
	{
		get
		{
			return _IsProductRetrievingInProcess;
		}
	}

	public bool IsConnected
	{
		get
		{
			return _IsConnected;
		}
	}

	public bool IsInventoryLoaded
	{
		get
		{
			return _IsInventoryLoaded;
		}
	}

	[method: MethodImpl(32)]
	public event Action<BillingResult> ActionProductPurchased = delegate
	{
	};

	[method: MethodImpl(32)]
	public event Action<BillingResult> ActionProductConsumed = delegate
	{
	};

	[method: MethodImpl(32)]
	public event Action<BillingResult> ActionBillingSetupFinished = delegate
	{
	};

	[method: MethodImpl(32)]
	public event Action<BillingResult> ActionRetrieveProducsFinished = delegate
	{
	};

	private void Awake()
	{
		_inventory = new AndroidInventory();
	}

	public void AddProduct(string SKU)
	{
		GoogleProductTemplate googleProductTemplate = new GoogleProductTemplate();
		googleProductTemplate.SKU = SKU;
		GoogleProductTemplate template = googleProductTemplate;
		AddProduct(template);
	}

	public void AddProduct(GoogleProductTemplate template)
	{
		bool flag = false;
		int index = 0;
		foreach (GoogleProductTemplate product in _inventory.Products)
		{
			if (product.SKU.Equals(template.SKU))
			{
				flag = true;
				index = _inventory.Products.IndexOf(product);
				break;
			}
		}
		if (flag)
		{
			_inventory.Products[index] = template;
		}
		else
		{
			_inventory.Products.Add(template);
		}
	}

	public void RetrieveProducDetails()
	{
		_IsProductRetrievingInProcess = true;
		AN_BillingProxy.RetrieveProducDetails();
	}

	public void Purchase(string SKU)
	{
		Purchase(SKU, string.Empty);
	}

	public void Purchase(string SKU, string DeveloperPayload)
	{
		_processedSKU = SKU;
		AN_SoomlaGrow.PurchaseStarted(SKU);
		AN_BillingProxy.Purchase(SKU, DeveloperPayload);
	}

	public void Subscribe(string SKU)
	{
		Subscribe(SKU, string.Empty);
	}

	public void Subscribe(string SKU, string DeveloperPayload)
	{
		_processedSKU = SKU;
		AN_SoomlaGrow.PurchaseStarted(SKU);
		AN_BillingProxy.Subscribe(SKU, DeveloperPayload);
	}

	public void Consume(string SKU)
	{
		_processedSKU = SKU;
		AN_BillingProxy.Consume(SKU);
	}

	public void LoadStore()
	{
		Connect();
	}

	public void LoadStore(string base64EncodedPublicKey)
	{
		Connect(base64EncodedPublicKey);
	}

	public void Connect()
	{
		if (AndroidNativeSettings.Instance.IsBase64KeyWasReplaced)
		{
			Connect(AndroidNativeSettings.Instance.base64EncodedPublicKey);
			_IsConnectingToServiceInProcess = true;
		}
		else
		{
			Debug.LogError("Replace base64EncodedPublicKey in Androdi Native Setting menu");
		}
	}

	public void Connect(string base64EncodedPublicKey)
	{
		foreach (GoogleProductTemplate inAppProduct in AndroidNativeSettings.Instance.InAppProducts)
		{
			AddProduct(inAppProduct.SKU);
		}
		string text = string.Empty;
		int count = AndroidNativeSettings.Instance.InAppProducts.Count;
		for (int i = 0; i < count; i++)
		{
			if (i != 0)
			{
				text += ",";
			}
			text += AndroidNativeSettings.Instance.InAppProducts[i].SKU;
		}
		AN_BillingProxy.Connect(text, base64EncodedPublicKey);
	}
}
