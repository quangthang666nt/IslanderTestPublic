using UnityEngine;
using UnityEngine.UI;

public class PlaylistElement : MonoBehaviour
{
	[HideInInspector]
	public string id = string.Empty;

	[HideInInspector]
	public UIPlaylistsSettings settings;

	private Toggle toggle;

	private void Awake()
	{
		toggle = GetComponent<Toggle>();
	}

	public void ValueChange()
	{
		if ((bool)toggle && (bool)settings)
		{
			settings.ToggleInteracted(toggle.isOn, id);
		}
	}
}
