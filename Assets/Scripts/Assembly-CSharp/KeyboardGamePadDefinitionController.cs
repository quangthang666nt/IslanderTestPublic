using System.Linq;
using Rewired;
using UnityEngine;

public class KeyboardGamePadDefinitionController : MonoBehaviour
{
	public GameObject keyboard;

	public GameObject gamepad;

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
				keyboard.SetActive(controllerType == ControllerType.Mouse);
				gamepad.SetActive(controllerType == ControllerType.Joystick);
			}
		}
	}

	private void Start()
	{
		OnActiveControllerUpdated(PlatformPlayerManagerSystem.Instance.LastActiveController);
	}

	public void OnRestoreKeyboard()
	{
		UiCanvasManager.Singleton.ToRestoreKeyboardPrompt();
	}

	public void DoRestoreKeyboard()
	{
		Player player = ReInput.players.GetPlayer(0);
		RemapToDefaultControllerMap(player.controllers.maps.GetMap(player.controllers.Keyboard, "Default", "Default"), player.controllers.maps.GetMap(player.controllers.Keyboard, "Original", "Default"));
		RemapToDefaultControllerMap(player.controllers.maps.GetMap(player.controllers.Mouse, "Default", "Default"), player.controllers.maps.GetMap(player.controllers.Mouse, "Original", "Default"));
		KeyMappingTool[] componentsInChildren = keyboard.transform.GetComponentsInChildren<KeyMappingTool>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Refresh();
		}
		ReInput.userDataStore.Save();
	}

	public void OnRestoreGamepad()
	{
		UiCanvasManager.Singleton.ToRestoreGamepadPrompt();
	}

	public void DoRestoreGamepad()
	{
		Player player = ReInput.players.GetPlayer(0);
		RemapToDefaultControllerMap(player.controllers.maps.GetMap(player.controllers.Joysticks[0], "Default", "Default"), player.controllers.maps.GetMap(player.controllers.Joysticks[0], "Original", "Default"));
		KeyMappingTool[] componentsInChildren = gamepad.transform.GetComponentsInChildren<KeyMappingTool>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Refresh();
		}
		ReInput.userDataStore.Save();
	}

	private void RemapToDefaultControllerMap(ControllerMap controllerMapToRemapDefault, ControllerMap originalControllerMap)
	{
		for (int i = 0; i < controllerMapToRemapDefault.AllMaps.Count; i++)
		{
			ActionElementMap map = controllerMapToRemapDefault.AllMaps[i];
			ActionElementMap actionElementMap = originalControllerMap.AllMaps.First((ActionElementMap x) => x.actionDescriptiveName == map.actionDescriptiveName);
			map.elementIdentifierId = actionElementMap.elementIdentifierId;
		}
	}
}
