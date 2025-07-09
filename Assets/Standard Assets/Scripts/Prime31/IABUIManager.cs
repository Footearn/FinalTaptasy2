using UnityEngine;

namespace Prime31
{
	public class IABUIManager : MonoBehaviourGUI
	{
		private void OnGUI()
		{
			beginColumn();
			if (GUILayout.Button("Initialize IAB"))
			{
				string text = "your public key from the Android developer portal here";
				text = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsdmu4xvSgwaSOA1R9plkAEHPuQ1mQzJ48TE2mTLnp1GWebirXHP5qWPsG+l1gYH+cbQ2K5qHGk9NcL12usGaaGWkVEGc49qqv7Zq5HoXBMNRdom6484Eib3l1xyh0GKgVcmovkHfM1mY0qeVgoANFeixJS/QhZpQlNlud2p4wVd93HVV36FLc2HJ3v33xrDkddCEWXRPJgg0ZnMKo/Qn2H2cAdYXxVSclfqV8fsfkEz9XiVaWer6QqdcWAI5vnUNFiTePg7ZbjSkJKBiYHUT1qzfmMVefGYPNlIoN8viku8L6bU2nzcFIFGKhiFemWFAB6RvmJDvROLE7e5CECqIEQIDAQAB";
				GoogleIAB.init(text);
			}
			if (GUILayout.Button("Query Inventory"))
			{
				string[] skus = new string[4]
				{
					"com.prime31.testproduct",
					"android.test.purchased",
					"com.prime31.managedproduct",
					"com.prime31.noads"
				};
				GoogleIAB.queryInventory(skus);
			}
			if (GUILayout.Button("Are subscriptions supported?"))
			{
				//Debug.Log("subscriptions supported: " + GoogleIAB.areSubscriptionsSupported());
			}
			if (GUILayout.Button("Purchase Test Product"))
			{
				GoogleIAB.purchaseProduct("android.test.purchased");
			}
			if (GUILayout.Button("Consume Test Purchase"))
			{
				GoogleIAB.consumeProduct("android.test.purchased");
			}
			if (GUILayout.Button("Test Unavailable Item"))
			{
				GoogleIAB.purchaseProduct("android.test.item_unavailable");
			}
			endColumn(true);
			if (GUILayout.Button("Purchase Real Product"))
			{
				GoogleIAB.purchaseProduct("com.prime31.testproduct", "payload that gets stored and returned");
			}
			if (GUILayout.Button("Purchase Real Subscription"))
			{
				GoogleIAB.purchaseProduct("com.prime31.testsubscription", "subscription payload");
			}
			if (GUILayout.Button("Consume Real Purchase"))
			{
				GoogleIAB.consumeProduct("com.prime31.testproduct");
			}
			if (GUILayout.Button("Enable High Details Logs"))
			{
				GoogleIAB.enableLogging(true);
			}
			if (GUILayout.Button("Consume Multiple Purchases"))
			{
				string[] skus2 = new string[2]
				{
					"com.prime31.testproduct",
					"android.test.purchased"
				};
				GoogleIAB.consumeProducts(skus2);
			}
			endColumn();
		}
	}
}
