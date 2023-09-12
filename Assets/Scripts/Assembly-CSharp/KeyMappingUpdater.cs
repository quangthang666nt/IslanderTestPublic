using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyMappingUpdater : MonoBehaviour
{
	private static Regex s_InputReplacementRegex = new Regex("(?<=\\{#)(.*?)(?=#\\})", RegexOptions.Compiled);

	public string m_ActionName;

	private Controller m_ControllerSpecific;

	public Image m_MappingImage;

	public bool m_MappingTextAutoSize = true;

	public float m_MappingTextMaxSize = 40f;

	public float m_MappingTextMinSize = 25f;

	public float m_MappingTextSize = 40f;

	public TextMeshProUGUI m_MappingText;

	public Image m_FillImage;

	public Image m_FillBackgroundImage;

	public Sprite m_KeyboardFill;

	public Sprite m_KeyboardFillBackground;

	public Sprite m_JoystickFill;

	public Sprite m_JoystickFillBackground;

	public bool m_Init;

	private void Start()
	{
		if (PlatformPlayerManagerSystem.Instance != null)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated += OnActiveControllerUpdated;
		}
		ForceRefresh();
	}

	private void OnEnable()
	{
		if (PlatformPlayerManagerSystem.Instance != null)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated += OnActiveControllerUpdated;
		}
		OnActiveControllerUpdated(null);
		ForceRefresh();
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated -= OnActiveControllerUpdated;
		}
	}

	public void SetSpecificController(Controller controller)
	{
		m_ControllerSpecific = controller;
		ForceRefresh();
	}

	public void ForceRefresh()
	{
		PlatformPlayerManagerSystem instance = PlatformPlayerManagerSystem.Instance;
		if (instance != null)
		{
			if (m_ControllerSpecific != null)
			{
				OnActiveControllerUpdated(m_ControllerSpecific, forceMouseIsKeyboard: true);
			}
			else
			{
				OnActiveControllerUpdated(instance.LastActiveController, forceMouseIsKeyboard: true);
			}
		}
	}

	public void Enable()
	{
		PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated += OnActiveControllerUpdated;
		base.gameObject.SetActive(value: true);
	}

	public void Disable()
	{
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.OnLastActiveControllerUpdated -= OnActiveControllerUpdated;
		}
		base.gameObject.SetActive(value: false);
	}

	private void OnActiveControllerUpdated(Controller controller)
	{
		if (m_ControllerSpecific == null)
		{
			OnActiveControllerUpdated(controller, forceMouseIsKeyboard: false);
		}
	}

	private void OnActiveControllerUpdated(Controller controller, bool forceMouseIsKeyboard)
	{
		Sprite sprite = null;
		string text = string.Empty;
		ControllerType controllerType = ControllerType.Joystick;
		if (InputManager.Singleton == null)
		{
			return;
		}
		InputIcon icon = InputManager.Singleton.GetIcon("Default");
		if (icon != null)
		{
			sprite = icon.Image;
			text = icon.Text;
			if (InputManager.ShouldUsePS4ControllerIcons(Guid.Empty))
			{
				sprite = icon.PS4ControllerImage;
			}
			else if (InputManager.ShouldUseNintendoProControllerIcons(Guid.Empty))
			{
				sprite = icon.NintendoProControllerImage;
			}
			else if (InputManager.ShouldUseXBoxControllerIcons(Guid.Empty))
			{
				sprite = icon.XBoxControllerImage;
			}
			else if (InputManager.ShouldStadiaControllerIcons(Guid.Empty))
			{
				sprite = icon.StadiaControllerImage;
			}
		}
		if (controller != null)
		{
			controllerType = controller.type;
		}
		if ((!m_Init || forceMouseIsKeyboard) && controllerType == ControllerType.Mouse)
		{
			controllerType = ControllerType.Keyboard;
		}
		switch (controllerType)
		{
		case ControllerType.Mouse:
			base.gameObject.SetActive(value: false);
			return;
		case ControllerType.Keyboard:
			base.gameObject.SetActive(value: false);
			icon = InputManager.Singleton.GetIcon(m_ActionName);
			sprite = null;
			if (icon != null && icon.SpecialAllowedForKeyboard)
			{
				sprite = icon.Image;
				text = icon.Text;
			}
			else
			{
				Player rewiredPlayer2 = InputManager.Singleton.RewiredPlayer;
				if (rewiredPlayer2 != null && rewiredPlayer2.controllers.hasKeyboard && (m_ControllerSpecific == null || rewiredPlayer2.controllers.ContainsController(m_ControllerSpecific)))
				{
					ActionElementMap actionElementMap2 = null;
					string actionName = m_ActionName;
					bool flag = false;
					if (m_ActionName.EndsWith("+"))
					{
						flag = true;
						actionName = m_ActionName.Remove(m_ActionName.Length - 1, 1);
					}
					else if (m_ActionName.EndsWith("-"))
					{
						flag = true;
						actionName = m_ActionName.Remove(m_ActionName.Length - 1, 1);
					}
					if (m_ControllerSpecific != null)
					{
						if (flag)
						{
							List<ActionElementMap> list2 = new List<ActionElementMap>(4);
							rewiredPlayer2.controllers.maps.GetElementMapsWithAction(ControllerType.Keyboard, m_ControllerSpecific.id, actionName, skipDisabledMaps: false, list2);
							for (int j = 0; j < list2.Count; j++)
							{
								if (list2[j].actionDescriptiveName == m_ActionName)
								{
									actionElementMap2 = list2[j];
									break;
								}
							}
						}
						else
						{
							actionElementMap2 = rewiredPlayer2.controllers.maps.GetFirstElementMapWithAction(ControllerType.Keyboard, m_ControllerSpecific.id, actionName, skipDisabledMaps: false);
						}
					}
					else if (flag)
					{
						List<ActionElementMap> list3 = new List<ActionElementMap>(4);
						rewiredPlayer2.controllers.maps.GetElementMapsWithAction(ControllerType.Keyboard, actionName, skipDisabledMaps: false, list3);
						for (int k = 0; k < list3.Count; k++)
						{
							if (list3[k].actionDescriptiveName == m_ActionName)
							{
								actionElementMap2 = list3[k];
								break;
							}
						}
					}
					else
					{
						actionElementMap2 = rewiredPlayer2.controllers.maps.GetFirstElementMapWithAction(ControllerType.Keyboard, actionName, skipDisabledMaps: false);
					}
					if (actionElementMap2 == null)
					{
						break;
					}
					KeyCode keyCode = actionElementMap2.keyCode;
					InputIcon icon4 = InputManager.Singleton.GetIcon(keyCode.ToString());
					if (icon4 == null)
					{
						icon4 = InputManager.Singleton.GetIcon("DefaultKeyboardElement");
						text = keyCode.ToString();
						sprite = icon4?.Image;
						break;
					}
					text = icon4.Text;
					if (icon4.Image == null)
					{
						icon4 = InputManager.Singleton.GetIcon("DefaultKeyboardElement");
					}
					sprite = icon4?.Image;
					break;
				}
			}
			if (m_FillImage != null)
			{
				m_FillImage.sprite = m_KeyboardFill;
			}
			if (m_FillBackgroundImage != null)
			{
				m_FillBackgroundImage.sprite = m_KeyboardFillBackground;
			}
			break;
		case ControllerType.Joystick:
			base.gameObject.SetActive(value: true);
			icon = InputManager.Singleton.GetIcon(m_ActionName);
			if (icon != null && icon.PrioritizeOverActualMapping)
			{
				sprite = icon.Image;
				text = icon.Text;
			}
			else
			{
				ActionElementMap actionElementMap = null;
				IGamepadTemplate gamepadTemplate = null;
				Player rewiredPlayer = InputManager.Singleton.RewiredPlayer;
				if (rewiredPlayer != null && rewiredPlayer.controllers.joystickCount > 0 && (m_ControllerSpecific == null || rewiredPlayer.controllers.ContainsController(m_ControllerSpecific)))
				{
					if (m_ControllerSpecific != null)
					{
						actionElementMap = rewiredPlayer.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, m_ControllerSpecific.id, m_ActionName, skipDisabledMaps: false);
						gamepadTemplate = m_ControllerSpecific.GetTemplate<IGamepadTemplate>();
						if (gamepadTemplate != null)
						{
							break;
						}
					}
					else
					{
						actionElementMap = rewiredPlayer.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, m_ActionName, skipDisabledMaps: false);
						for (int i = 0; i < rewiredPlayer.controllers.joystickCount; i++)
						{
							gamepadTemplate = rewiredPlayer.controllers.Joysticks[i].GetTemplate<IGamepadTemplate>();
							if (gamepadTemplate != null)
							{
								break;
							}
						}
					}
				}
				if (actionElementMap != null && gamepadTemplate != null)
				{
					List<ControllerTemplateElementTarget> list = new List<ControllerTemplateElementTarget>();
					string empty = string.Empty;
					empty = ((gamepadTemplate.GetElementTargets(actionElementMap, list) <= 0) ? actionElementMap.elementIdentifierName : list[0].descriptiveName);
					if (!string.IsNullOrEmpty(empty))
					{
						InputIcon icon2 = InputManager.Singleton.GetIcon(empty);
						if (icon2 != null)
						{
							Guid guid = Guid.Empty;
							if (controller != null && controller is Joystick)
							{
								guid = ((Joystick)controller).hardwareTypeGuid;
							}
							if (InputManager.ShouldUsePS4ControllerIcons(guid))
							{
								sprite = icon2.PS4ControllerImage;
								text = icon2.AlternateText;
							}
							else if (InputManager.ShouldUseNintendoProControllerIcons(guid))
							{
								sprite = icon2.NintendoProControllerImage;
								text = icon2.AlternateText;
							}
							else if (InputManager.ShouldUseXBoxControllerIcons(guid))
							{
								sprite = icon2.XBoxControllerImage;
								text = icon2.AlternateText;
							}
							else if (InputManager.ShouldStadiaControllerIcons(guid))
							{
								sprite = icon2.StadiaControllerImage;
								text = icon2.AlternateText;
							}
							else
							{
								sprite = icon2.Image;
								text = icon2.Text;
							}
						}
						else
						{
							Debug.LogError("[KeyMappingUpdater] Missing Input Icon for Joystick on Action: " + m_ActionName);
						}
					}
					else
					{
						Debug.LogError("[KeyMappingUpdater] Missing Key Mapping for Joystick on Action: " + m_ActionName);
					}
				}
				else
				{
					InputIcon icon3 = InputManager.Singleton.GetIcon(m_ActionName);
					if (icon3 != null)
					{
						Guid guid2 = Guid.Empty;
						if (controller != null && controller is Joystick)
						{
							guid2 = ((Joystick)controller).hardwareTypeGuid;
						}
						if (InputManager.ShouldUsePS4ControllerIcons(guid2))
						{
							sprite = icon3.PS4ControllerImage;
							text = icon3.AlternateText;
						}
						else if (InputManager.ShouldUseNintendoProControllerIcons(guid2))
						{
							sprite = icon3.NintendoProControllerImage;
							text = icon3.AlternateText;
						}
						else if (InputManager.ShouldUseXBoxControllerIcons(guid2))
						{
							sprite = icon3.XBoxControllerImage;
							text = icon3.AlternateText;
						}
						else if (InputManager.ShouldStadiaControllerIcons(guid2))
						{
							sprite = icon3.StadiaControllerImage;
							text = icon3.AlternateText;
						}
						else
						{
							sprite = icon3.Image;
							text = icon3.Text;
						}
					}
				}
			}
			if (m_FillImage != null)
			{
				m_FillImage.sprite = m_JoystickFill;
			}
			if (m_FillBackgroundImage != null)
			{
				m_FillBackgroundImage.sprite = m_JoystickFillBackground;
			}
			break;
		}
		m_Init = true;
		if (sprite != null)
		{
			if (m_MappingImage != null)
			{
				m_MappingImage.sprite = sprite;
			}
			if (m_MappingText != null)
			{
				m_MappingText.enableAutoSizing = m_MappingTextAutoSize;
				m_MappingText.fontSizeMin = m_MappingTextMinSize;
				m_MappingText.fontSizeMax = m_MappingTextMaxSize;
				m_MappingText.fontSize = m_MappingTextSize;
				m_MappingText.text = text;
			}
		}
	}

	public static void ReplaceSpecialText(TextMeshProUGUI textElement, KeyMappingUpdater[] keyMappingObjects, float verticalOffset = 0f, string replacementString = "¤")
	{
		string text = textElement.text;
		if (!text.Contains("{#"))
		{
			return;
		}
		int num = 0;
		if (text.Contains("{#MoveStickRotate#}"))
		{
			text = text.Replace("{#MoveStickRotate#}", "{#MoveHorizontal-#}{#MoveVertical+#}{#MoveHorizontal+#}{#MoveVertical-#}");
		}
		if (text.Contains("{#MoveStickDown#}"))
		{
			text = text.Replace("{#MoveStickDown#}", "{#MoveVertical-#}");
		}
		MatchCollection matchCollection = s_InputReplacementRegex.Matches(text);
		for (int i = 0; i < matchCollection.Count; i++)
		{
			string value = matchCollection[i].Value;
			text = text.Replace("{#" + value + "#}", "     " + replacementString + "     ");
			if (num < keyMappingObjects.Length)
			{
				keyMappingObjects[num].gameObject.SetActive(value: true);
				keyMappingObjects[num].m_ActionName = value;
				keyMappingObjects[num].ForceRefresh();
				num++;
			}
		}
		textElement.text = text;
		textElement.ForceMeshUpdate();
		int startIndex = 0;
		TMP_TextInfo textInfo = textElement.textInfo;
		for (int j = 0; j < num; j++)
		{
			int num2 = text.IndexOf(replacementString, startIndex);
			if (num2 >= 0 && num2 < textInfo.characterInfo.Length)
			{
				Vector3 position = textElement.transform.TransformPoint((textInfo.characterInfo[num2].bottomLeft + textInfo.characterInfo[num2].topRight) / 2f + Vector3.up * verticalOffset);
				startIndex = num2 + 1;
				keyMappingObjects[j].transform.position = position;
			}
		}
		textElement.ForceMeshUpdate();
	}
}
