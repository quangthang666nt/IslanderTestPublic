using I2.Loc;
using UnityEngine;

public class UIRedefinePrompt : MonoBehaviour
{
	public enum RedefinitionMode
	{
		GamepadButton = 0,
		GamepadAxis = 1,
		Keyboard = 2,
		MouseButton = 3
	}

	private static UIRedefinePrompt Instance;

	public Localize label;

	private void Awake()
	{
		Instance = this;
	}

	public static void Show(RedefinitionMode mode = RedefinitionMode.Keyboard)
	{
		UiCanvasManager.Singleton.ShowRedefinePrompt();
		string term = "";
		switch (mode)
		{
		case RedefinitionMode.GamepadButton:
			term = "Input Actions/AnyButton";
			break;
		case RedefinitionMode.GamepadAxis:
			term = "Input Actions/AnyAxis";
			break;
		case RedefinitionMode.Keyboard:
			term = "Input Actions/AnyKey";
			break;
		case RedefinitionMode.MouseButton:
			term = "Input Actions/MouseButton";
			break;
		}
		Instance.label.Term = term;
	}
}
