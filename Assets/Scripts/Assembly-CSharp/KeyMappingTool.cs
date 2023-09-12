using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyMappingTool : MonoBehaviour
{
	public enum Mode
	{
		GamepadButton = 0,
		GamepadAxis = 1,
		Keyboard = 2,
		MouseButton = 3
	}

	public string m_ActionName;

	public TextMeshProUGUI text;

	public Mode mode;

	public string[] others;

	private Image image;

	private static ControllerMap gamepadControllerMap;

	private static IGamepadTemplate gamepadTemplate = null;

	private static Guid controllerGuid = Guid.Empty;

	private static ControllerMap keyboardControllerMap;

	private static IControllerTemplate keyboardTemplate = null;

	private static ControllerMap mouseControllerMap;

	private static IControllerTemplate mouseTemplate = null;

	private static InputMapper controlMapper = new InputMapper();

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	private void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		Player player = ReInput.players.GetPlayer(0);
		if (gamepadControllerMap == null && player.controllers.Joysticks.Count > 0)
		{
			gamepadControllerMap = player.controllers.maps.GetMap(player.controllers.Joysticks[0], "Default", "Default");
			gamepadTemplate = player.controllers.Joysticks[0].GetTemplate<IGamepadTemplate>();
		}
		if (keyboardControllerMap == null)
		{
			keyboardControllerMap = player.controllers.maps.GetMap(player.controllers.Keyboard, "Default", "Default");
		}
		if (keyboardTemplate == null && player.controllers.Keyboard.Templates.Count > 0)
		{
			keyboardTemplate = player.controllers.Keyboard.Templates[0];
		}
		if (mouseControllerMap == null)
		{
			mouseControllerMap = player.controllers.maps.GetMap(player.controllers.Mouse, "Default", "Default");
		}
		if (mouseTemplate == null && player.controllers.Mouse.Templates.Count > 0)
		{
			mouseTemplate = player.controllers.Mouse.Templates[0];
		}
		if (controllerGuid == Guid.Empty)
		{
			foreach (Controller controller in player.controllers.Controllers)
			{
				if (controller.type == ControllerType.Joystick)
				{
					controllerGuid = controller.hardwareTypeGuid;
					break;
				}
			}
		}
		InputIcon inputIcon = null;
		string text = m_ActionName;
		Pole pole = Pole.Positive;
		if (text.EndsWith('+') || text.EndsWith('-'))
		{
			if (text.EndsWith('-'))
			{
				pole = Pole.Negative;
			}
			text = text.Substring(0, text.Length - 1);
		}
		this.text.enabled = false;
		image.enabled = false;
		switch (mode)
		{
		case Mode.GamepadButton:
		case Mode.GamepadAxis:
			inputIcon = InputManager.Singleton.GetIcon(m_ActionName);
			if (inputIcon == null && gamepadControllerMap != null)
			{
				List<ActionElementMap> list = new List<ActionElementMap>();
				player.controllers.maps.GetElementMapsWithAction(ControllerType.Joystick, 0, text, skipDisabledMaps: false, list);
				foreach (ActionElementMap item in list)
				{
					if (item.axisContribution == pole)
					{
						List<ControllerTemplateElementTarget> list2 = new List<ControllerTemplateElementTarget>();
						if (gamepadTemplate.GetElementTargets(item, list2) > 0)
						{
							inputIcon = InputManager.Singleton.GetIcon(list2[0].descriptiveName);
							this.text.text = item.elementIdentifierName;
							break;
						}
					}
				}
			}
			if (inputIcon != null)
			{
				if (InputManager.ShouldUsePS4ControllerIcons(controllerGuid))
				{
					image.sprite = inputIcon.PS4ControllerImage;
				}
				else if (InputManager.ShouldUseNintendoProControllerIcons(controllerGuid))
				{
					image.sprite = inputIcon.NintendoProControllerImage;
				}
				else if (InputManager.ShouldUseXBoxControllerIcons(controllerGuid))
				{
					image.sprite = inputIcon.XBoxControllerImage;
				}
				else if (InputManager.ShouldStadiaControllerIcons(controllerGuid))
				{
					image.sprite = inputIcon.StadiaControllerImage;
				}
				else
				{
					image.sprite = inputIcon.Image;
				}
				image.enabled = true;
				this.text.enabled = false;
			}
			else
			{
				this.text.enabled = true;
			}
			break;
		case Mode.Keyboard:
			if (keyboardControllerMap == null)
			{
				break;
			}
			{
				foreach (ActionElementMap item2 in keyboardControllerMap.ElementMapsWithAction(text))
				{
					if (item2.axisContribution == pole)
					{
						this.text.text = item2.elementIdentifierName;
						this.text.enabled = true;
						break;
					}
				}
				break;
			}
		case Mode.MouseButton:
			if (mouseControllerMap == null)
			{
				break;
			}
			{
				foreach (ActionElementMap item3 in mouseControllerMap.ElementMapsWithAction(text))
				{
					if (item3.axisContribution == pole)
					{
						inputIcon = InputManager.Singleton.GetIcon(item3.elementIdentifierName);
						if (inputIcon != null)
						{
							image.sprite = inputIcon.Image;
							image.enabled = true;
						}
						else
						{
							this.text.text = item3.elementIdentifierName;
							this.text.enabled = true;
						}
						break;
					}
				}
				break;
			}
		}
	}

	public void OnSubmit()
	{
		controlMapper.TimedOutEvent += OnTimedOut;
		controlMapper.InputMappedEvent += OnControlMapped;
		controlMapper.ConflictFoundEvent += OnControllerConflictFound;
		controlMapper.ErrorEvent += OnErrorEvent;
		controlMapper.options.allowButtonsOnFullAxisAssignment = true;
		controlMapper.options.timeout = 5f;
		controlMapper.options.ignoreMouseXAxis = true;
		controlMapper.options.ignoreMouseYAxis = true;
		string text = m_ActionName;
		AxisRange actionRange = AxisRange.Positive;
		Pole pole = Pole.Positive;
		if (text.EndsWith('+') || text.EndsWith('-'))
		{
			if (text.EndsWith('-'))
			{
				actionRange = AxisRange.Negative;
				pole = Pole.Negative;
			}
			text = text.Substring(0, text.Length - 1);
		}
		ActionElementMap actionElementMapToReplace = null;
		Player player = ReInput.players.GetPlayer(0);
		switch (mode)
		{
		case Mode.Keyboard:
			UIRedefinePrompt.Show();
			foreach (ActionElementMap item in keyboardControllerMap.ElementMapsWithAction(text))
			{
				if (item.axisContribution == pole)
				{
					actionElementMapToReplace = item;
				}
			}
			controlMapper.options.checkForConflicts = false;
			controlMapper.Start(new InputMapper.Context
			{
				actionName = text,
				controllerMap = keyboardControllerMap,
				actionRange = actionRange,
				actionElementMapToReplace = actionElementMapToReplace
			});
			player.controllers.maps.SetMapsEnabled(state: false, player.controllers.Keyboard, "Default");
			player.controllers.maps.SetMapsEnabled(state: false, player.controllers.Mouse, "Default");
			break;
		case Mode.MouseButton:
			UIRedefinePrompt.Show(UIRedefinePrompt.RedefinitionMode.MouseButton);
			foreach (ActionElementMap item2 in mouseControllerMap.ElementMapsWithAction(text))
			{
				if (item2.axisContribution == pole)
				{
					actionElementMapToReplace = item2;
				}
			}
			controlMapper.options.checkForConflicts = false;
			controlMapper.Start(new InputMapper.Context
			{
				actionName = text,
				controllerMap = mouseControllerMap,
				actionRange = actionRange,
				actionElementMapToReplace = actionElementMapToReplace
			});
			player.controllers.maps.SetMapsEnabled(state: false, player.controllers.Keyboard, "Default");
			player.controllers.maps.SetMapsEnabled(state: false, player.controllers.Mouse, "Default");
			break;
		case Mode.GamepadButton:
			UIRedefinePrompt.Show(UIRedefinePrompt.RedefinitionMode.GamepadButton);
			foreach (ActionElementMap item3 in gamepadControllerMap.ElementMapsWithAction(text))
			{
				if (item3.axisContribution == pole)
				{
					actionElementMapToReplace = item3;
				}
			}
			controlMapper.options.checkForConflicts = false;
			controlMapper.options.allowAxes = false;
			controlMapper.options.allowButtons = true;
			controlMapper.Start(new InputMapper.Context
			{
				actionName = text,
				controllerMap = gamepadControllerMap,
				actionRange = actionRange,
				actionElementMapToReplace = actionElementMapToReplace
			});
			player.controllers.maps.SetMapsEnabled(state: false, player.controllers.Joysticks[0], "Default");
			break;
		case Mode.GamepadAxis:
			UIRedefinePrompt.Show(UIRedefinePrompt.RedefinitionMode.GamepadAxis);
			foreach (ActionElementMap item4 in gamepadControllerMap.ElementMapsWithAction(text))
			{
				if (item4.axisContribution == pole)
				{
					actionElementMapToReplace = item4;
				}
			}
			controlMapper.options.checkForConflicts = false;
			controlMapper.options.allowAxes = true;
			controlMapper.options.allowButtons = false;
			controlMapper.Start(new InputMapper.Context
			{
				actionName = text,
				controllerMap = gamepadControllerMap,
				actionRange = AxisRange.Full,
				actionElementMapToReplace = actionElementMapToReplace
			});
			player.controllers.maps.SetMapsEnabled(state: false, player.controllers.Joysticks[0], "Default");
			break;
		}
	}

	public static ActionElementMap GetKeyboardAction(string ActionName)
	{
		string text = ActionName;
		Pole pole = Pole.Positive;
		if (text.EndsWith('+') || text.EndsWith('-'))
		{
			if (text.EndsWith('-'))
			{
				pole = Pole.Negative;
			}
			text = text.Substring(0, text.Length - 1);
		}
		if (keyboardControllerMap == null)
		{
			Player player = ReInput.players.GetPlayer(0);
			keyboardControllerMap = player.controllers.maps.GetMap(player.controllers.Keyboard, "Default", "Default");
		}
		foreach (ActionElementMap item in keyboardControllerMap.ElementMapsWithAction(text))
		{
			if (item.axisContribution == pole)
			{
				return item;
			}
		}
		return null;
	}

	private void FinishRemap()
	{
		Player player = ReInput.players.GetPlayer(0);
		player.controllers.maps.SetMapsEnabled(state: true, player.controllers.Keyboard, "Default");
		player.controllers.maps.SetMapsEnabled(state: true, player.controllers.Mouse, "Default");
		if (player.controllers.Joysticks.Count > 0)
		{
			player.controllers.maps.SetMapsEnabled(state: true, player.controllers.Joysticks[0], "Default");
		}
		controlMapper.TimedOutEvent -= OnTimedOut;
		controlMapper.InputMappedEvent -= OnControlMapped;
		controlMapper.ConflictFoundEvent -= OnControllerConflictFound;
		controlMapper.ErrorEvent -= OnErrorEvent;
		ReInput.userDataStore.Save();
		Refresh();
	}

	private void OnTimedOut(InputMapper.TimedOutEventData obj)
	{
		UiCanvasManager.Singleton.HideRedefinePrompt();
		obj.inputMapper.mappingContext.actionElementMapToReplace.enabled = true;
		FinishRemap();
	}

	private void OnControlMapped(InputMapper.InputMappedEventData obj)
	{
		UiCanvasManager.Singleton.HideRedefinePrompt();
		if (mode == Mode.GamepadAxis)
		{
			if (obj.actionElementMap.elementIdentifierName.EndsWith("Stick Y"))
			{
				string elementIdentifierName = obj.actionElementMap.elementIdentifierName;
				elementIdentifierName = elementIdentifierName.Substring(0, elementIdentifierName.Length - 1) + "X";
				ActionElementMap actionElementFromOrigin = GetActionElementFromOrigin(elementIdentifierName);
				obj.actionElementMap.elementIdentifierId = actionElementFromOrigin.elementIdentifierId;
			}
			string[] array = others;
			foreach (string actionName in array)
			{
				foreach (ActionElementMap item in gamepadControllerMap.ElementMapsWithAction(actionName))
				{
					string elementIdentifierName2 = obj.actionElementMap.elementIdentifierName;
					elementIdentifierName2 = elementIdentifierName2.Substring(0, elementIdentifierName2.Length - 1) + "Y";
					ActionElementMap actionElementFromOrigin2 = GetActionElementFromOrigin(elementIdentifierName2);
					item.elementIdentifierId = actionElementFromOrigin2.elementIdentifierId;
				}
			}
		}
		FinishRemap();
	}

	private ActionElementMap GetActionElementFromOrigin(string elementIdentifierName)
	{
		Player player = ReInput.players.GetPlayer(0);
		return player.controllers.maps.GetMap(player.controllers.Joysticks[0], "Original", "Default").AllMaps.First((ActionElementMap x) => x.elementIdentifierName == elementIdentifierName);
	}

	private void OnControllerConflictFound(InputMapper.ConflictFoundEventData obj)
	{
		UiCanvasManager.Singleton.HideRedefinePrompt();
		Debug.Log("OnControllerConflictFound");
		_ = obj.conflicts[0].elementMap.actionDescriptiveName;
		FinishRemap();
	}

	private void OnErrorEvent(InputMapper.ErrorEventData obj)
	{
		UiCanvasManager.Singleton.HideRedefinePrompt();
		obj.inputMapper.mappingContext.actionElementMapToReplace.enabled = true;
		Debug.Log("ERROR");
		FinishRemap();
	}
}
