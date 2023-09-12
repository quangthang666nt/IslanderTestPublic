using Rewired;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SelectionOutlineUpdater : MonoBehaviour
{
	[SerializeField]
	private bool onlyEnableWithKeyboard;

	private Image image;

	private void Awake()
	{
		image = GetComponent<Image>();
		if (onlyEnableWithKeyboard)
		{
			image.enabled = false;
		}
	}

	private void Start()
	{
		if (PlatformPlayerManagerSystem.Instance != null)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated += OnActiveControllerUpdated;
			OnActiveControllerUpdated(PlatformPlayerManagerSystem.Instance.LastActiveController);
		}
	}

	private void OnDestroy()
	{
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated -= OnActiveControllerUpdated;
		}
	}

	private void OnActiveControllerUpdated(Controller controller)
	{
		switch (controller.type)
		{
		case ControllerType.Mouse:
			image.enabled = false;
			break;
		case ControllerType.Joystick:
		case ControllerType.Custom:
			image.enabled = !onlyEnableWithKeyboard;
			break;
		case ControllerType.Keyboard:
			image.enabled = true;
			break;
		}
	}
}
