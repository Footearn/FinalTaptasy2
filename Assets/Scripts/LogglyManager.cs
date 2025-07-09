using System.Collections;
using UnityEngine;

public class LogglyManager : Singleton<LogglyManager>
{
	private string token = "25180a90-2e42-42fb-a82e-8a6fdabf4e11";

	private string level = string.Empty;

	public void SendLoggly(string logString, string stackTrace, LogType type)
	{
		level = type.ToString();
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("LEVEL", level);
		wWWForm.AddField("Message", logString);
		wWWForm.AddField("Stack_Trace", stackTrace);
		wWWForm.AddField("Device_Model", SystemInfo.deviceModel);
		wWWForm.AddField("UserID", Singleton<NanooAPIManager>.instance.UserID);
		StartCoroutine(SendData(wWWForm));
	}

	public IEnumerator SendData(WWWForm form)
	{
		yield return new WWW("https://logs-01.loggly.com/inputs/" + token + "/tag/FinalTaptasy", form);
	}
}
