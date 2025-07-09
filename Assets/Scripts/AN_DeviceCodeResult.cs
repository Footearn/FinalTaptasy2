public class AN_DeviceCodeResult : AN_Result
{
	private string _deviceCode = string.Empty;

	private string _userCode = string.Empty;

	private string _verificationUrl = string.Empty;

	private long _expiresIn;

	private long _interval;

	private string _error = string.Empty;

	public string DeviceCode
	{
		get
		{
			return _deviceCode;
		}
	}

	public string UserCode
	{
		get
		{
			return _userCode;
		}
	}

	public string VerificationUrl
	{
		get
		{
			return _verificationUrl;
		}
	}

	public long ExpiresIn
	{
		get
		{
			return _expiresIn;
		}
	}

	public long Interval
	{
		get
		{
			return _interval;
		}
	}

	public string Error
	{
		get
		{
			return _error;
		}
	}

	public AN_DeviceCodeResult(bool result, string error)
		: base(result)
	{
		_error = error;
	}

	public AN_DeviceCodeResult(bool result, string deviceCode, string userCode, string verificationUrl, long expiresIn, long interval)
		: base(result)
	{
		_deviceCode = deviceCode;
		_userCode = userCode;
		_verificationUrl = verificationUrl;
		_expiresIn = expiresIn;
		_interval = interval;
	}
}
