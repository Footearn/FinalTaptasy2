using UnityEngine;

namespace Prime31
{
	public class GoogleIAB
	{
		private static AndroidJavaObject _plugin;

		public static void enableLogging(bool shouldEnable)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (shouldEnable)
				{
					Debug.LogWarning("YOU HAVE ENABLED HIGH DETAIL LOGS. DO NOT DISTRIBUTE THE GENERATED APK PUBLICLY. IT WILL DUMP SENSITIVE INFORMATION TO THE CONSOLE!");
				}
			}
		}

		public static void init(string publicKey)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (_plugin == null)
				{
					using (var pluginClass = new AndroidJavaClass("com.prime31.GoogleIABPlugin"))
						_plugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
				}

				_plugin.Call("init", publicKey);
			}
		}

		public static void queryInventory(string[] skus)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("queryInventory", new object[] { skus });
			}
		}

		public static void purchaseProduct(string sku)
		{
			purchaseProduct(sku, string.Empty);
		}

		public static void purchaseProduct(string sku, string developerPayload)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("purchaseProduct", sku, developerPayload);
			}
		}

		public static void consumeProduct(string sku)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("consumeProduct", sku);
			}
		}

		public static void consumeProducts(string[] skus)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("consumeProducts", new object[] { skus });
			}
		}
	}
}
