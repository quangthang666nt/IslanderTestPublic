using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	public class LanguageData : ScriptableObject
	{
		[Serializable]
		private class CustomEntry
		{
			public string key;

			public string value;

			public CustomEntry()
			{
			}

			public CustomEntry(string key, string value)
			{
				this.key = key;
				this.value = value;
			}

			public static Dictionary<string, string> ToDictionary(CustomEntry[] array)
			{
				if (array == null)
				{
					return new Dictionary<string, string>();
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null && !string.IsNullOrEmpty(array[i].key) && !string.IsNullOrEmpty(array[i].value))
					{
						if (dictionary.ContainsKey(array[i].key))
						{
							Debug.LogError("Key \"" + array[i].key + "\" is already in dictionary!");
						}
						else
						{
							dictionary.Add(array[i].key, array[i].value);
						}
					}
				}
				return dictionary;
			}
		}

		[SerializeField]
		private LocalizedString _yes = "Yes";

		[SerializeField]
		private LocalizedString _no = "No";

		[SerializeField]
		private LocalizedString _add = "Add";

		[SerializeField]
		private LocalizedString _replace = "Replace";

		[SerializeField]
		private LocalizedString _remove = "Remove";

		[SerializeField]
		private LocalizedString _swap = "Swap";

		[SerializeField]
		private LocalizedString _cancel = "Cancel";

		[SerializeField]
		private LocalizedString _none = "None";

		[SerializeField]
		private LocalizedString _okay = "Okay";

		[SerializeField]
		private LocalizedString _done = "Done";

		[SerializeField]
		private LocalizedString _default = "Default";

		[SerializeField]
		private string _assignControllerWindowTitle = "Choose Controller";

		[SerializeField]
		private LocalizedString _assignControllerWindowMessage = "Press any button or move an axis on the controller you would like to use.";

		[SerializeField]
		private string _controllerAssignmentConflictWindowTitle = "Controller Assignment";

		[SerializeField]
		[Tooltip("{0} = Joystick Name\n{1} = Other Player Name\n{2} = This Player Name")]
		private string _controllerAssignmentConflictWindowMessage = "{0} is already assigned to {1}. Do you want to assign this controller to {2} instead?";

		[SerializeField]
		private string _elementAssignmentPrePollingWindowMessage = "First center or zero all sticks and axes and press any button or wait for the timer to finish.";

		[SerializeField]
		[Tooltip("{0} = Action Name")]
		private string _joystickElementAssignmentPollingWindowMessage = "Now press a button or move an axis to assign it to {0}.";

		[SerializeField]
		[Tooltip("This text is only displayed when split-axis fields have been disabled and the user clicks on the full-axis field. Button/key/D-pad input cannot be assigned to a full-axis field.\n{0} = Action Name")]
		private string _joystickElementAssignmentPollingWindowMessage_fullAxisFieldOnly = "Now move an axis to assign it to {0}.";

		[SerializeField]
		[Tooltip("{0} = Action Name")]
		private LocalizedString _keyboardElementAssignmentPollingWindowMessage = "Press a key to assign it to {0}. Modifier keys may also be used. To assign a modifier key alone, hold it down for 1 second.";

		[SerializeField]
		[Tooltip("{0} = Action Name")]
		private LocalizedString _mouseElementAssignmentPollingWindowMessage = "Press a mouse button or move an axis to assign it to {0}.";

		[SerializeField]
		[Tooltip("This text is only displayed when split-axis fields have been disabled and the user clicks on the full-axis field. Button/key/D-pad input cannot be assigned to a full-axis field.\n{0} = Action Name")]
		private string _mouseElementAssignmentPollingWindowMessage_fullAxisFieldOnly = "Move an axis to assign it to {0}.";

		[SerializeField]
		private LocalizedString _elementAssignmentConflictWindowMessage = "Assignment Conflict";

		[SerializeField]
		[Tooltip("{0} = Element Name")]
		private LocalizedString _elementAlreadyInUseBlocked = "{0} is already in use cannot be replaced.";

		[SerializeField]
		[Tooltip("{0} = Element Name")]
		private LocalizedString _elementAlreadyInUseCanReplace = "{0} is already in use. Do you want to replace it?";

		[SerializeField]
		[Tooltip("{0} = Element Name")]
		private LocalizedString _elementAlreadyInUseCanReplace_conflictAllowed = "{0} is already in use. Do you want to replace it? You may also choose to add the assignment anyway.";

		[SerializeField]
		private LocalizedString _mouseAssignmentConflictWindowTitle = "Mouse Assignment";

		[SerializeField]
		[Tooltip("{0} = Other Player Name\n{1} = This Player Name")]
		private LocalizedString _mouseAssignmentConflictWindowMessage = "The mouse is already assigned to {0}. Do you want to assign the mouse to {1} instead?";

		[SerializeField]
		private string _calibrateControllerWindowTitle = "Calibrate Controller";

		[SerializeField]
		private string _calibrateAxisStep1WindowTitle = "Calibrate Zero";

		[SerializeField]
		[Tooltip("{0} = Axis Name")]
		private string _calibrateAxisStep1WindowMessage = "Center or zero {0} and press any button or wait for the timer to finish.";

		[SerializeField]
		private string _calibrateAxisStep2WindowTitle = "Calibrate Range";

		[SerializeField]
		[Tooltip("{0} = Axis Name")]
		private string _calibrateAxisStep2WindowMessage = "Move {0} through its entire range then press any button or wait for the timer to finish.";

		[SerializeField]
		private string _inputBehaviorSettingsWindowTitle = "Sensitivity Settings";

		[SerializeField]
		private LocalizedString _restoreDefaultsWindowTitle = "Restore Defaults";

		[SerializeField]
		[Tooltip("Message for a single player game.")]
		private LocalizedString _restoreDefaultsWindowMessage_onePlayer = "This will restore the default input configuration. Are you sure you want to do this?";

		[SerializeField]
		[Tooltip("Message for a multi-player game.")]
		private string _restoreDefaultsWindowMessage_multiPlayer = "This will restore the default input configuration for all players. Are you sure you want to do this?";

		[SerializeField]
		private LocalizedString _actionColumnLabel = "Actions";

		[SerializeField]
		private LocalizedString _keyboardColumnLabel = "Keyboard";

		[SerializeField]
		private LocalizedString _mouseColumnLabel = "Mouse";

		[SerializeField]
		private LocalizedString _controllerColumnLabel = "Controller";

		[SerializeField]
		private string _removeControllerButtonLabel = "Remove";

		[SerializeField]
		private string _calibrateControllerButtonLabel = "Calibrate";

		[SerializeField]
		private string _assignControllerButtonLabel = "Assign Controller";

		[SerializeField]
		private string _inputBehaviorSettingsButtonLabel = "Sensitivity";

		[SerializeField]
		private LocalizedString _doneButtonLabel = "Done";

		[SerializeField]
		private LocalizedString _restoreDefaultsButtonLabel = "Restore Defaults";

		[SerializeField]
		private string _playersGroupLabel = "Players:";

		[SerializeField]
		private string _controllerSettingsGroupLabel = "Controller:";

		[SerializeField]
		private string _assignedControllersGroupLabel = "Assigned Controllers:";

		[SerializeField]
		private string _settingsGroupLabel = "Settings:";

		[SerializeField]
		private string _mapCategoriesGroupLabel = "Categories:";

		[SerializeField]
		private string _calibrateWindow_deadZoneSliderLabel = "Dead Zone:";

		[SerializeField]
		private string _calibrateWindow_zeroSliderLabel = "Zero:";

		[SerializeField]
		private string _calibrateWindow_sensitivitySliderLabel = "Sensitivity:";

		[SerializeField]
		private string _calibrateWindow_invertToggleLabel = "Invert";

		[SerializeField]
		private string _calibrateWindow_calibrateButtonLabel = "Calibrate";

		[SerializeField]
		private CustomEntry[] _customEntries;

		private bool _initialized;

		private Dictionary<string, string> customDict;

		public string yes => _yes;

		public string no => _no;

		public string add => _add;

		public string replace => _replace;

		public string remove => _remove;

		public string swap => _swap;

		public string cancel => _cancel;

		public string none => _none;

		public string okay => _okay;

		public string done => _done;

		public string default_ => _default;

		public string assignControllerWindowTitle => _assignControllerWindowTitle;

		public string assignControllerWindowMessage => _assignControllerWindowMessage;

		public string controllerAssignmentConflictWindowTitle => _controllerAssignmentConflictWindowTitle;

		public string elementAssignmentPrePollingWindowMessage => _elementAssignmentPrePollingWindowMessage;

		public string elementAssignmentConflictWindowMessage => _elementAssignmentConflictWindowMessage;

		public string mouseAssignmentConflictWindowTitle => _mouseAssignmentConflictWindowTitle;

		public string calibrateControllerWindowTitle => _calibrateControllerWindowTitle;

		public string calibrateAxisStep1WindowTitle => _calibrateAxisStep1WindowTitle;

		public string calibrateAxisStep2WindowTitle => _calibrateAxisStep2WindowTitle;

		public string inputBehaviorSettingsWindowTitle => _inputBehaviorSettingsWindowTitle;

		public string restoreDefaultsWindowTitle => _restoreDefaultsWindowTitle;

		public string actionColumnLabel => _actionColumnLabel;

		public string keyboardColumnLabel => _keyboardColumnLabel;

		public string mouseColumnLabel => _mouseColumnLabel;

		public string controllerColumnLabel => _controllerColumnLabel;

		public string removeControllerButtonLabel => _removeControllerButtonLabel;

		public string calibrateControllerButtonLabel => _calibrateControllerButtonLabel;

		public string assignControllerButtonLabel => _assignControllerButtonLabel;

		public string inputBehaviorSettingsButtonLabel => _inputBehaviorSettingsButtonLabel;

		public string doneButtonLabel => _doneButtonLabel;

		public string restoreDefaultsButtonLabel => _restoreDefaultsButtonLabel;

		public string controllerSettingsGroupLabel => _controllerSettingsGroupLabel;

		public string playersGroupLabel => _playersGroupLabel;

		public string assignedControllersGroupLabel => _assignedControllersGroupLabel;

		public string settingsGroupLabel => _settingsGroupLabel;

		public string mapCategoriesGroupLabel => _mapCategoriesGroupLabel;

		public string restoreDefaultsWindowMessage
		{
			get
			{
				if (ReInput.players.playerCount > 1)
				{
					return _restoreDefaultsWindowMessage_multiPlayer;
				}
				return _restoreDefaultsWindowMessage_onePlayer;
			}
		}

		public string calibrateWindow_deadZoneSliderLabel => _calibrateWindow_deadZoneSliderLabel;

		public string calibrateWindow_zeroSliderLabel => _calibrateWindow_zeroSliderLabel;

		public string calibrateWindow_sensitivitySliderLabel => _calibrateWindow_sensitivitySliderLabel;

		public string calibrateWindow_invertToggleLabel => _calibrateWindow_invertToggleLabel;

		public string calibrateWindow_calibrateButtonLabel => _calibrateWindow_calibrateButtonLabel;

		public void Initialize()
		{
			if (!_initialized)
			{
				customDict = CustomEntry.ToDictionary(_customEntries);
				_initialized = true;
			}
		}

		public string GetCustomEntry(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return string.Empty;
			}
			if (!customDict.TryGetValue(key, out var value))
			{
				return string.Empty;
			}
			return value;
		}

		public bool ContainsCustomEntryKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}
			return customDict.ContainsKey(key);
		}

		public string GetControllerAssignmentConflictWindowMessage(string joystickName, string otherPlayerName, string currentPlayerName)
		{
			return string.Format(_controllerAssignmentConflictWindowMessage, joystickName, otherPlayerName, currentPlayerName);
		}

		public string GetJoystickElementAssignmentPollingWindowMessage(string actionName)
		{
			return string.Format(_joystickElementAssignmentPollingWindowMessage, actionName);
		}

		public string GetJoystickElementAssignmentPollingWindowMessage_FullAxisFieldOnly(string actionName)
		{
			return string.Format(_joystickElementAssignmentPollingWindowMessage_fullAxisFieldOnly, actionName);
		}

		public string GetKeyboardElementAssignmentPollingWindowMessage(string actionName)
		{
			return string.Format(_keyboardElementAssignmentPollingWindowMessage, actionName);
		}

		public string GetMouseElementAssignmentPollingWindowMessage(string actionName)
		{
			return string.Format(_mouseElementAssignmentPollingWindowMessage, actionName);
		}

		public string GetMouseElementAssignmentPollingWindowMessage_FullAxisFieldOnly(string actionName)
		{
			return string.Format(_mouseElementAssignmentPollingWindowMessage_fullAxisFieldOnly, actionName);
		}

		public string GetElementAlreadyInUseBlocked(string elementName)
		{
			return string.Format(_elementAlreadyInUseBlocked, elementName);
		}

		public string GetElementAlreadyInUseCanReplace(string elementName, bool allowConflicts)
		{
			if (!allowConflicts)
			{
				return string.Format(_elementAlreadyInUseCanReplace, elementName);
			}
			return string.Format(_elementAlreadyInUseCanReplace_conflictAllowed, elementName);
		}

		public string GetMouseAssignmentConflictWindowMessage(string otherPlayerName, string thisPlayerName)
		{
			return string.Format(_mouseAssignmentConflictWindowMessage, otherPlayerName, thisPlayerName);
		}

		public string GetCalibrateAxisStep1WindowMessage(string axisName)
		{
			return string.Format(_calibrateAxisStep1WindowMessage, axisName);
		}

		public string GetCalibrateAxisStep2WindowMessage(string axisName)
		{
			return string.Format(_calibrateAxisStep2WindowMessage, axisName);
		}
	}
}
