using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public enum ETutorialState
	{
		DoneAndIgnore = 0,
		LaunchInNextGame = 1,
		CamMoveKey = 2,
		CamMoveMouse = 3,
		CamRotateKey = 4,
		CamRotateMouse = 5,
		CamZoom = 6
	}

	[Help("This script manages the tutorial.", MessageType.Info)]
	[SerializeField]
	[Tooltip("Required building rotation steps till the building rotation tooltip disappears.")]
	private int iRequiredRotationSteps = 5;

	[Tooltip("Tooltips for the first part of the tutorial where you learn the camera movement.")]
	public List<GameObject> m_PCCameraTooltips;

	[Tooltip("Tooltips for the first part of the tutorial where you learn the camera movement.")]
	public List<GameObject> m_ConsoleCameraTooltips;

	private List<GameObject> liGoCamTooltips = new List<GameObject>();

	[SerializeField]
	[Tooltip("Movement treshholds for the first part of the tutorial where you learn the camera movement.")]
	private List<float> liFTreshholds = new List<float>();

	[Tooltip("Tooltip teaching you how to rotate buildings.")]
	public GameObject m_PCRotateBuildingTutorial;

	[Tooltip("Tooltip teaching you how to rotate buildings.")]
	public GameObject m_ConsoleRotateBuildingTutorial;

	[Tooltip("Tooltip pointing you towards the next island button.")]
	public GameObject m_PCNextIslandTutorial;

	[Tooltip("Tooltip pointing you towards the next island button.")]
	public GameObject m_ConsoleNextIslandTutorial;

	private GameObject goRotateBuildingTutorial;

	[SerializeField]
	[Tooltip("Tooltip teaching you how place buildings and score points.")]
	private GameObject goHowBuildingWorksTutorial;

	[SerializeField]
	[Tooltip("Tooltip teaching you how to level up and get new buildings.")]
	private GameObject goLevelupTutorial;

	private GameObject goNextIslandTutorial;

	public static ETutorialState eTutorialState;

	public static bool bTeachHowToRotateBuilding;

	public static bool bTeachHowBuildingWorks;

	public static bool bTeachLevelUp;

	public static bool bTeachNextIsland;

	private ControllerType oldControllerType;

	private void Awake()
	{
		liGoCamTooltips = new List<GameObject>();
		goNextIslandTutorial = m_PCNextIslandTutorial;
		goRotateBuildingTutorial = m_PCRotateBuildingTutorial;
		for (int i = 0; i < m_PCCameraTooltips.Count; i++)
		{
			liGoCamTooltips.Add(m_PCCameraTooltips[i]);
		}
	}

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
		ControllerType controllerType = ControllerType.Joystick;
		if (controller != null)
		{
			controllerType = controller.type;
		}
		if (controllerType == ControllerType.Keyboard)
		{
			controllerType = ControllerType.Mouse;
		}
		if (oldControllerType == controllerType)
		{
			return;
		}
		oldControllerType = controllerType;
		bool activeSelf = goNextIslandTutorial.activeSelf;
		bool activeSelf2 = goRotateBuildingTutorial.activeSelf;
		goNextIslandTutorial.SetActive(value: false);
		goRotateBuildingTutorial.SetActive(value: false);
		if (controllerType == ControllerType.Joystick)
		{
			goNextIslandTutorial = m_ConsoleNextIslandTutorial;
			goRotateBuildingTutorial = m_ConsoleRotateBuildingTutorial;
			foreach (GameObject liGoCamTooltip in liGoCamTooltips)
			{
				liGoCamTooltip.SetActive(value: false);
			}
			liGoCamTooltips.Clear();
			for (int i = 0; i < m_ConsoleCameraTooltips.Count; i++)
			{
				liGoCamTooltips.Add(m_ConsoleCameraTooltips[i]);
			}
		}
		else
		{
			goNextIslandTutorial = m_PCNextIslandTutorial;
			goRotateBuildingTutorial = m_PCRotateBuildingTutorial;
			foreach (GameObject liGoCamTooltip2 in liGoCamTooltips)
			{
				liGoCamTooltip2.SetActive(value: false);
			}
			liGoCamTooltips.Clear();
			for (int j = 0; j < m_PCCameraTooltips.Count; j++)
			{
				liGoCamTooltips.Add(m_PCCameraTooltips[j]);
			}
		}
		goNextIslandTutorial.SetActive(activeSelf);
		goRotateBuildingTutorial.SetActive(activeSelf2);
		if (eTutorialState != 0 && eTutorialState != ETutorialState.LaunchInNextGame)
		{
			CheckCameraTutorialStep();
		}
	}

	public static void AfterLoad()
	{
		if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			return;
		}
		bTeachNextIsland = false;
		CameraController.singleton.bAllowCamMouseRotation = true;
		if (!SettingsManager.Singleton.CurrentData.gameplayData.bShowTutorial)
		{
			eTutorialState = ETutorialState.DoneAndIgnore;
		}
		if ((StatsManager.statsGlobal.iHighscore < 20 || SettingsManager.Singleton.CurrentData.gameplayData.bShowTutorial) && LocalGameManager.singleton.IScore < 20)
		{
			eTutorialState = ETutorialState.LaunchInNextGame;
			CameraController.singleton.ResetTutorialStats();
			bTeachHowToRotateBuilding = true;
			bTeachHowBuildingWorks = true;
			bTeachLevelUp = true;
			if (StatsManager.statsGlobal.iMaxIslandsReachedInAnyOrThisRun < 2 && StatsManager.statsMatch.iMaxIslandsReachedInAnyOrThisRun < 2)
			{
				bTeachNextIsland = true;
			}
		}
		if (StatsManager.statsGlobal.iMaxIslandsReachedInAnyOrThisRun < 2 && StatsManager.statsMatch.iMaxIslandsReachedInAnyOrThisRun < 2)
		{
			bTeachNextIsland = true;
		}
	}

	private void CheckCameraTutorialStep()
	{
		switch (eTutorialState)
		{
		case ETutorialState.CamMoveKey:
			if (oldControllerType == ControllerType.Mouse)
			{
				liGoCamTooltips[0].SetActive(value: true);
			}
			else
			{
				liGoCamTooltips[0].SetActive(value: true);
			}
			break;
		case ETutorialState.CamMoveMouse:
			if (oldControllerType == ControllerType.Mouse)
			{
				Debug.LogError("Unexpected state");
				break;
			}
			liGoCamTooltips[1].SetActive(value: true);
			eTutorialState = ETutorialState.CamRotateKey;
			break;
		case ETutorialState.CamRotateKey:
			if (oldControllerType == ControllerType.Mouse)
			{
				liGoCamTooltips[2].SetActive(value: true);
			}
			else
			{
				liGoCamTooltips[1].SetActive(value: true);
			}
			break;
		case ETutorialState.CamRotateMouse:
			if (oldControllerType == ControllerType.Mouse)
			{
				Debug.LogError("Unexpected state");
				break;
			}
			liGoCamTooltips[2].SetActive(value: true);
			eTutorialState = ETutorialState.CamZoom;
			break;
		case ETutorialState.CamZoom:
			if (oldControllerType == ControllerType.Mouse)
			{
				eTutorialState = ETutorialState.DoneAndIgnore;
				UiCanvasManager.Singleton.ToStartMatch();
			}
			else
			{
				Debug.LogError("Unexpected state");
			}
			break;
		default:
			Debug.LogError("Unexpected state");
			break;
		}
	}

	public void SkipCameraTutorialStep()
	{
		if (eTutorialState == ETutorialState.CamMoveKey)
		{
			foreach (GameObject liGoCamTooltip in liGoCamTooltips)
			{
				liGoCamTooltip.SetActive(value: false);
			}
			if (oldControllerType == ControllerType.Mouse)
			{
				eTutorialState = ETutorialState.CamMoveMouse;
				liGoCamTooltips[1].SetActive(value: true);
			}
			else
			{
				CameraController.singleton.bAllowCamMouseRotation = true;
				eTutorialState = ETutorialState.CamRotateKey;
				liGoCamTooltips[1].SetActive(value: true);
			}
		}
		else if (eTutorialState == ETutorialState.CamMoveMouse)
		{
			CameraController.singleton.bAllowCamMouseRotation = true;
			eTutorialState = ETutorialState.CamRotateKey;
			foreach (GameObject liGoCamTooltip2 in liGoCamTooltips)
			{
				liGoCamTooltip2.SetActive(value: false);
			}
			liGoCamTooltips[2].SetActive(value: true);
		}
		else if (eTutorialState == ETutorialState.CamRotateKey)
		{
			foreach (GameObject liGoCamTooltip3 in liGoCamTooltips)
			{
				liGoCamTooltip3.SetActive(value: false);
			}
			if (oldControllerType == ControllerType.Mouse)
			{
				eTutorialState = ETutorialState.CamRotateMouse;
				liGoCamTooltips[3].SetActive(value: true);
			}
			else
			{
				eTutorialState = ETutorialState.CamZoom;
				CameraController.singleton.ZoomWithMouse = 0f;
				liGoCamTooltips[2].SetActive(value: true);
			}
		}
		else if (eTutorialState == ETutorialState.CamRotateMouse)
		{
			foreach (GameObject liGoCamTooltip4 in liGoCamTooltips)
			{
				liGoCamTooltip4.SetActive(value: false);
			}
			eTutorialState = ETutorialState.DoneAndIgnore;
			UiCanvasManager.Singleton.ToStartMatch();
		}
		else
		{
			if (eTutorialState != ETutorialState.CamZoom)
			{
				return;
			}
			foreach (GameObject liGoCamTooltip5 in liGoCamTooltips)
			{
				liGoCamTooltip5.SetActive(value: false);
			}
			eTutorialState = ETutorialState.DoneAndIgnore;
			UiCanvasManager.Singleton.ToStartMatch();
		}
	}

	public void Update()
	{
		if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			return;
		}
		if (eTutorialState == ETutorialState.LaunchInNextGame && LocalGameManager.singleton.GameState == LocalGameManager.EGameState.InGame && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.MenuWithCurrent && UiCanvasManager.Singleton.UIState != 0 && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.InGameTransition)
		{
			if (LocalGameManager.singleton.IScore > 0)
			{
				eTutorialState = ETutorialState.DoneAndIgnore;
				bTeachHowToRotateBuilding = false;
				bTeachHowBuildingWorks = false;
				bTeachLevelUp = false;
				UiCanvasManager.Singleton.ToStartMatch();
			}
			else
			{
				eTutorialState = ETutorialState.CamMoveKey;
				foreach (GameObject liGoCamTooltip in liGoCamTooltips)
				{
					liGoCamTooltip.SetActive(value: false);
				}
				liGoCamTooltips[0].SetActive(value: true);
			}
		}
		if (eTutorialState == ETutorialState.CamMoveKey && CameraController.singleton.FMovementWithKeys > liFTreshholds[0])
		{
			SkipCameraTutorialStep();
		}
		if (eTutorialState == ETutorialState.CamMoveMouse)
		{
			CameraController.singleton.bAllowCamMouseRotation = false;
			if (CameraController.singleton.FMovementWithMouse > liFTreshholds[1])
			{
				SkipCameraTutorialStep();
			}
		}
		if (eTutorialState == ETutorialState.CamRotateKey && CameraController.singleton.FRotationWithKeys > liFTreshholds[2])
		{
			SkipCameraTutorialStep();
		}
		if (eTutorialState == ETutorialState.CamRotateMouse && CameraController.singleton.FRotationWithMouse > liFTreshholds[3])
		{
			SkipCameraTutorialStep();
		}
		if (eTutorialState == ETutorialState.CamZoom && CameraController.singleton.ZoomWithMouse > liFTreshholds[4])
		{
			SkipCameraTutorialStep();
		}
		if (bTeachHowToRotateBuilding)
		{
			if ((bool)UiBuildingButtonManager.singleton.GoBuildingPreview)
			{
				goRotateBuildingTutorial.SetActive(value: true);
				if (UiBuildingButtonManager.singleton.IRotatedBuildingSteps >= iRequiredRotationSteps)
				{
					goRotateBuildingTutorial.SetActive(value: false);
					bTeachHowToRotateBuilding = false;
				}
			}
			else
			{
				goRotateBuildingTutorial.SetActive(value: false);
			}
		}
		else if (bTeachHowBuildingWorks)
		{
			if ((bool)UiBuildingButtonManager.singleton.GoBuildingPreview)
			{
				goHowBuildingWorksTutorial.SetActive(value: true);
			}
			else
			{
				goHowBuildingWorksTutorial.SetActive(value: false);
				if (LocalGameManager.singleton.IScore >= 20)
				{
					bTeachHowBuildingWorks = false;
					UiBuildingButtonManager.singleton.GoSelectedButton = null;
				}
			}
		}
		else if (bTeachLevelUp)
		{
			if ((bool)UiBuildingButtonManager.singleton.GoBuildingPreview || UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePicking)
			{
				bTeachLevelUp = false;
				goLevelupTutorial.SetActive(value: false);
			}
			else if (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePlaying)
			{
				goLevelupTutorial.SetActive(value: true);
			}
			else
			{
				goLevelupTutorial.SetActive(value: false);
			}
		}
		if (bTeachNextIsland)
		{
			if (LocalGameManager.singleton.BNextIslandAvailable && UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePlaying)
			{
				goNextIslandTutorial.SetActive(value: true);
			}
			else
			{
				goNextIslandTutorial.SetActive(value: false);
			}
			if (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGameNewIslandPrompt)
			{
				goNextIslandTutorial.SetActive(value: false);
				bTeachNextIsland = false;
			}
		}
	}
}
