public class AN_BillingProxy
{
	private static void CallActivityFunction(string methodName, params object[] args)
	{
		
	}

	public static void Connect(string ids, string base64EncodedPublicKey)
	{
		CallActivityFunction("AN_Connect", ids, base64EncodedPublicKey);
	}

	public static void RetrieveProducDetails()
	{
		CallActivityFunction("AN_RetrieveProducDetails");
	}

	public static void Consume(string SKU)
	{
		CallActivityFunction("AN_Consume", SKU);
	}

	public static void Purchase(string SKU, string developerPayload)
	{
		CallActivityFunction("AN_Purchase", SKU, developerPayload);
	}

	public static void Subscribe(string SKU, string developerPayload)
	{
		CallActivityFunction("AN_Subscribe", SKU, developerPayload);
	}
}
