using UnityEngine;
using UnityEngine.SceneManagement;

public static class GlobalGameManager
{
	public enum ApplicationState
	{
		Menu = 0,
		Playing = 1
	}

	private static ApplicationState m_eAppState;

	public const int MENUSCENE_INDEX = 0;

	public const int GAMESCENE_INDEX = 1;

	private const string FILE_EXTENSION = ".bldr";

	public static ApplicationState eAppState => m_eAppState;

	public static void SetApplicationState(ApplicationState _newState)
	{
		switch (_newState)
		{
		case ApplicationState.Menu:
			SceneManager.LoadScene(0);
			break;
		case ApplicationState.Playing:
			SceneManager.LoadScene(1);
			break;
		}
		m_eAppState = _newState;
	}

	public static void Quit()
	{
		Application.Quit();
	}
}
