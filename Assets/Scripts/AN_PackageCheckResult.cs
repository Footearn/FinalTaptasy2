public class AN_PackageCheckResult : AN_Result
{
	private string _packageName;

	public string packageName
	{
		get
		{
			return _packageName;
		}
	}

	public AN_PackageCheckResult(string packId, bool IsResultSucceeded)
		: base(IsResultSucceeded)
	{
		_packageName = packId;
	}
}
