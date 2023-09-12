using UnityEngine;

public class SaveSettingsOnDisable : MonoBehaviour
{
	private void OnDisable()
	{
		SettingsManager.Singleton.Save();
	}
}
