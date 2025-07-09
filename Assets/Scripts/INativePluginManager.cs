internal interface INativePluginManager
{
	void Init();

	string GetClipboardString();

	void SetClipboardString(string name, string content);

	string GetUDIDString();

	void RegistRemoteNotification();
}
