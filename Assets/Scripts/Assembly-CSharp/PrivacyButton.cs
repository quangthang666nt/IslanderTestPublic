using UnityEngine;
using UnityEngine.Analytics;

public class PrivacyButton : MonoBehaviour
{
	private void Awake()
	{
	}

	private static void OnFailure(string reason)
	{
		Debug.LogWarning($"Failed to get data privacy page URL: {reason}");
	}

	private void OnURLReceived(string url)
	{
		Application.OpenURL(url);
	}

	public void OpenDataURL()
	{
		DataPrivacy.FetchPrivacyUrl(OnURLReceived, OnFailure);
	}

	public void OpenPrivacySettings()
	{
		OpenDataURL();
	}
}
