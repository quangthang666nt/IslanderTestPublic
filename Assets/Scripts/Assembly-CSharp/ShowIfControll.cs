using Rewired;
using UnityEngine;

public class ShowIfControll : MonoBehaviour
{
	public ControllerType type = ControllerType.Mouse;

	public GameObject target;

	private ControllerType oldControllerType;

	private void OnEnable()
	{
		PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated += OnActiveControllerUpdated;
	}

	private void OnDisable()
	{
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated -= OnActiveControllerUpdated;
		}
	}

	private void OnActiveControllerUpdated(Controller controller)
	{
		if (controller != null)
		{
			ControllerType controllerType = ControllerType.Joystick;
			if (controller != null)
			{
				controllerType = controller.type;
			}
			if (controllerType == ControllerType.Keyboard)
			{
				controllerType = ControllerType.Mouse;
			}
			if (oldControllerType != controllerType)
			{
				oldControllerType = controllerType;
				target.SetActive(controllerType == type);
			}
		}
	}

	private void Start()
	{
		OnActiveControllerUpdated(PlatformPlayerManagerSystem.Instance.LastActiveController);
	}
}
