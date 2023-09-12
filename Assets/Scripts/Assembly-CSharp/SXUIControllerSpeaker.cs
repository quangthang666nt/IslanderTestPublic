using UnityEngine;
using UnityEngine.UI;

public class SXUIControllerSpeaker : MonoBehaviour
{
	[SerializeField]
	private UISettingsMenu settingsMenu;

	[SerializeField]
	private Transform optionsParent;

	[SerializeField]
	private Toggle toggle;

	private void Start()
	{
		optionsParent.gameObject.SetActive(value: false);
	}
}
