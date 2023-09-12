using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public class InputData
	{
		public InputMode imLastUsedInputMethod;

		public float fCameraRotateWithButtons;

		public float fCameraMoveXWithButtons;

		public float fCameraMoveZWithButtons;

		public float fCameraZoom;

		public ButtonState bsCameraDragPan;

		public ButtonState bsCameraDragRotate;

		public Vector2 v2CameraDragDelta;

		public Vector2 UIMove;

		public bool bCameraCenterView;

		public ButtonState bsBuildingPlace;

		public ButtonState bsBuildingDeselect;

		public bool[] bQuickSelect = new bool[10];

		public float fRotateBuilding;

		public ButtonState bsSelectNextBuilding;

		public ButtonState bsSelectPreviousBuilding;

		public bool bCenterPointer;

		public bool bChangeModel;

		public bool bToggleMenu;

		public bool bToggleLeaderboard;

		public Vector3 v3PointerScreenPos = Vector3.zero;

		public Vector2 v2PointerScreenPosNormalizedFromHeight = Vector2.zero;

		public Ray rayPointerToWorld;

		public bool bUndo;

		public bool bUseSpecialTrackPadScrolling;

		public int buildingPackSelectionScroll;

		public ButtonState bsNextIsland;

		public bool bUICancel;

		public bool bUIExit;

		public bool bUIExitUp;

		public bool bUIReset;

		public bool bUIConfirm;

		public bool bUIToggleFilter;

		public bool bUIResetIsland;

		public bool bUIGenerateIsland;

		public float fUISlider;

		public bool bAccountChangeRequested;

		public bool bOpenScreenshotMode;

		public bool bOpenSettings;

		public bool bToggleArchiveIsland;

		public bool bUIModify;

		public bool bUINewScreenshot;

		public bool bUIModifyText;

		public void Reset()
		{
			imLastUsedInputMethod = InputMode.Mouse;
			fCameraRotateWithButtons = 0f;
			fCameraMoveXWithButtons = 0f;
			fCameraMoveZWithButtons = 0f;
			fCameraZoom = 0f;
			bsCameraDragPan = ButtonState.NotPressed;
			bsCameraDragRotate = ButtonState.NotPressed;
			v2CameraDragDelta = Vector2.zero;
			UIMove = Vector2.zero;
			bCameraCenterView = false;
			bCenterPointer = false;
			bChangeModel = false;
			bsBuildingPlace = ButtonState.NotPressed;
			bsBuildingDeselect = ButtonState.NotPressed;
			for (int i = 0; i < bQuickSelect.Length; i++)
			{
				bQuickSelect[i] = false;
			}
			fRotateBuilding = 0f;
			bsSelectNextBuilding = ButtonState.NotPressed;
			bsSelectPreviousBuilding = ButtonState.NotPressed;
			bToggleMenu = false;
			bToggleLeaderboard = false;
			v3PointerScreenPos = Vector3.zero;
			v2PointerScreenPosNormalizedFromHeight = Vector2.zero;
			rayPointerToWorld = default(Ray);
			bUndo = false;
			bUseSpecialTrackPadScrolling = false;
			buildingPackSelectionScroll = 0;
			bsNextIsland = ButtonState.NotPressed;
			bUICancel = false;
			bUIExit = false;
			bUIExitUp = false;
			bUIReset = false;
			bUIToggleFilter = false;
			bUIConfirm = false;
			bOpenScreenshotMode = false;
			bOpenSettings = false;
			bUIGenerateIsland = false;
			fUISlider = 0f;
			bUIResetIsland = false;
			bToggleArchiveIsland = false;
			bUIModify = false;
			bUINewScreenshot = false;
			bUIModifyText = false;
		}
	}

	public enum InputMode
	{
		Mouse = 0,
		Controller = 1,
		Mobile = 2
	}

	public enum ButtonState
	{
		NotPressed = 0,
		Down = 1,
		Pressed = 2,
		Up = 3
	}

	public static bool s_DisableAllInput;

	private InputData inputDataCurrent = new InputData();

	public List<InputIcon> m_AllIcons;

	private Dictionary<string, InputIcon> m_IconDictionary = new Dictionary<string, InputIcon>();

	private static InputManager singleton;

	private InputMode inputModeCurrent;

	private HashSet<object> m_InputLocks = new HashSet<object>();

	public InputMode imLastUsedInputMethod;

	private Player player;

	private const float buildingRotationButtonRepeatTime = 0.1f;

	private float buttonRepeatClock;

	private bool bMouseWheelUsed;

	private bool bBuildingRotationButtonOnCD;

	private int iFramesSinceLastScrollInput;

	private float fBuildingRotation;

	private bool bUseTrackPadScrollingFixOnMac = true;

	[SerializeField]
	private float fScrollStartTime = 0.4f;

	[SerializeField]
	private float fScrollIntervals = 0.1f;

	private Vector3 v3PointerScreenPos;

	private Vector3 v3LastMousePos;

	private float fMovingPointerAtMaxSpeedTimer;

	public float fPointerStartSpeed = 1f;

	public float fPointerSpeedupOnHold = 2f;

	public float fMaxPointerSpeedAddition = 9f;

	public List<string> m_AlternateControllerIconsGUIDs = new List<string>();

	private List<Guid> m_AlternateControllerIconsCachedGUIDs = new List<Guid>();

	public List<string> m_PS4ControllerIconsGUIDs = new List<string>();

	private List<Guid> m_PS4ControllerIconsCachedGUIDs = new List<Guid>();

	public List<string> m_NintendoProControllerIconsGUIDs = new List<string>();

	private List<Guid> m_NintendoProControllerIconsCachedGUIDs = new List<Guid>();

	public List<string> m_XBoxControllerIconsGUIDs = new List<string>();

	private List<Guid> m_XBoxControllerIconsCachedGUIDs = new List<Guid>();

	public List<string> m_StadiaControllerIconsGUIDs = new List<string>();

	private List<Guid> m_StadiaControllerIconsCachedGUIDs = new List<Guid>();

	private float fCameraZoomInLastFrame;

	private float fCameraHoldZoomTime;

	private float fBuildingRotationInLastFrame;

	private float fHoldBuildingRotationTime;

	private float fSelectNextBuildiningHoldTime;

	private float fSelectPreviousBuildingHoldTime;

	public float m_CursorAcceleration = 1f;

	public float m_CursorMaxSpeed = 1f;

	public float m_CursorDamp;

	private Vector2 m_CursorVelocity = Vector2.zero;

	private List<InputActionSourceData> liInputSourceData = new List<InputActionSourceData>();

	public InputData InputDataCurrent => inputDataCurrent;

	public static InputManager Singleton => singleton;

	public InputMode InputModeCurrent => inputModeCurrent;

	[HideInInspector]
	public bool InputLocked => m_InputLocks.Count > 0;

	public Player RewiredPlayer => player;

	public float pointerStartSpeedValue { get; set; }

	public float pointerSpeedupOnHoldValue { get; set; }

	public float maxPointerSpeedAdditionValue { get; set; }

	public static void DisableAllInput()
	{
		s_DisableAllInput = true;
	}

	public static void EnableAllInput()
	{
		s_DisableAllInput = false;
	}

	public void AddInputLocker(object locker)
	{
		if (!m_InputLocks.Contains(locker))
		{
			m_InputLocks.Add(locker);
		}
	}

	public void RemoveInputLocker(object locker)
	{
		if (m_InputLocks.Contains(locker))
		{
			m_InputLocks.Remove(locker);
		}
	}

	private void Awake()
	{
		InitializeSingleton();
		for (int i = 0; i < m_AllIcons.Count; i++)
		{
			m_IconDictionary.Add(m_AllIcons[i].Name, m_AllIcons[i]);
		}
		m_AlternateControllerIconsCachedGUIDs.Clear();
		for (int j = 0; j < m_AlternateControllerIconsGUIDs.Count; j++)
		{
			m_AlternateControllerIconsCachedGUIDs.Add(new Guid(m_AlternateControllerIconsGUIDs[j]));
		}
		m_PS4ControllerIconsCachedGUIDs.Clear();
		for (int k = 0; k < m_PS4ControllerIconsGUIDs.Count; k++)
		{
			m_PS4ControllerIconsCachedGUIDs.Add(new Guid(m_PS4ControllerIconsGUIDs[k]));
		}
		m_NintendoProControllerIconsCachedGUIDs.Clear();
		for (int l = 0; l < m_NintendoProControllerIconsGUIDs.Count; l++)
		{
			m_NintendoProControllerIconsCachedGUIDs.Add(new Guid(m_NintendoProControllerIconsGUIDs[l]));
		}
		m_XBoxControllerIconsCachedGUIDs.Clear();
		for (int m = 0; m < m_XBoxControllerIconsGUIDs.Count; m++)
		{
			m_XBoxControllerIconsCachedGUIDs.Add(new Guid(m_XBoxControllerIconsGUIDs[m]));
		}
		m_StadiaControllerIconsCachedGUIDs.Clear();
		for (int n = 0; n < m_StadiaControllerIconsGUIDs.Count; n++)
		{
			m_StadiaControllerIconsCachedGUIDs.Add(new Guid(m_StadiaControllerIconsGUIDs[n]));
		}
	}

	public InputIcon GetIcon(string name)
	{
		InputIcon value = null;
		m_IconDictionary.TryGetValue(name, out value);
		return value;
	}

	private void InitializeSingleton()
	{
		if ((bool)singleton)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			singleton = this;
		}
	}

	private void Start()
	{
		player = ReInput.players.GetPlayer(0);
		IList<Joystick> joysticks = ReInput.controllers.Joysticks;
		for (int i = 0; i < joysticks.Count; i++)
		{
			LoadPlatformSpecificMaps(joysticks[i]);
		}
		ResetPointer();
		pointerStartSpeedValue = fPointerStartSpeed;
		pointerSpeedupOnHoldValue = fPointerSpeedupOnHold;
		maxPointerSpeedAdditionValue = fMaxPointerSpeedAddition;
	}

	public void LoadPlatformSpecificMaps(Controller controller)
	{
		IList<Player> allPlayers = ReInput.players.AllPlayers;
		for (int i = 0; i < allPlayers.Count; i++)
		{
			if (allPlayers[i].controllers.ContainsController(controller))
			{
				allPlayers[i].controllers.maps.ClearMapsForController(controller.type, controller.id, userAssignableOnly: false);
				allPlayers[i].controllers.maps.LoadMap(controller.type, controller.id, "Default", "Default", startEnabled: true);
			}
		}
	}

	public void ResetPointer()
	{
		v3PointerScreenPos = Vector3.zero;
		v3PointerScreenPos += (float)Screen.width * 0.5f * Vector3.right;
		v3PointerScreenPos += (float)Screen.height * 0.4f * Vector3.up;
		m_CursorVelocity = Vector2.zero;
	}

	public void UpdatePointer()
	{
		v3LastMousePos = v3PointerScreenPos;
		if (Input.mousePosition != v3LastMousePos && imLastUsedInputMethod != InputMode.Controller)
		{
			v3PointerScreenPos = Input.mousePosition;
			fMovingPointerAtMaxSpeedTimer = 0f;
			return;
		}
		Vector2 vector = new Vector2(player.GetAxisRaw("Move Building Horizontal"), player.GetAxisRaw("Move Building Vertical"));
		if (vector.magnitude > 0.9f)
		{
			fMovingPointerAtMaxSpeedTimer += Time.deltaTime;
		}
		else
		{
			fMovingPointerAtMaxSpeedTimer = 0f;
		}
		float num = pointerStartSpeedValue + Mathf.Min(fMovingPointerAtMaxSpeedTimer * pointerSpeedupOnHoldValue, maxPointerSpeedAdditionValue);
		v3PointerScreenPos += (float)Screen.height / 10f * Time.deltaTime * vector.x * Vector3.right * num;
		v3PointerScreenPos += (float)Screen.height / 10f * Time.deltaTime * vector.y * Vector3.up * num;
		v3PointerScreenPos = new Vector3(Mathf.Clamp(v3PointerScreenPos.x, 0f, Screen.width), Mathf.Clamp(v3PointerScreenPos.y, 0f, Screen.height), v3PointerScreenPos.z);
	}

	private void Update()
	{
		UpdatePointer();
		CheckLastUsedInputMethod();
		if (!s_DisableAllInput)
		{
			FetchInput();
		}
		else
		{
			inputDataCurrent = new InputData();
		}
	}

	private void CheckLastUsedInputMethod()
	{
		liInputSourceData.Clear();
		liInputSourceData.AddRange(player.GetCurrentInputSources("Camera Rotate"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Camera Pan X"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Camera Pan Z"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Camera Drag Delta X"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Camera Drag Delta Y"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Camera Drag Pan"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Camera Drag Rotate"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Building Place"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Building Deselect"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Building Rotate"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Menu Overlay Toggle"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Undo"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Camera Zoom"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Select Next Building"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Select Previous Building"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Move Building Horizontal"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Move Building Vertical"));
		liInputSourceData.AddRange(player.GetCurrentInputSources("Next Island"));
		for (int i = 0; i < liInputSourceData.Count; i++)
		{
			if (liInputSourceData[i].controllerType == ControllerType.Joystick || liInputSourceData[i].controllerType == ControllerType.Custom)
			{
				if (imLastUsedInputMethod != InputMode.Controller)
				{
					ResetPointer();
					imLastUsedInputMethod = InputMode.Controller;
					break;
				}
			}
			else if (liInputSourceData[i].controllerType == ControllerType.Mouse)
			{
				imLastUsedInputMethod = InputMode.Mouse;
			}
			else if (liInputSourceData[i].controllerType == ControllerType.Keyboard)
			{
				imLastUsedInputMethod = InputMode.Mouse;
			}
		}
	}

	private void FetchInput()
	{
		if (InputLocked || !Application.isFocused)
		{
			inputDataCurrent.Reset();
			inputDataCurrent.bUIConfirm = player.GetButtonDown("UISubmit");
			inputDataCurrent.bUICancel = player.GetButtonDown("UICancel");
			inputDataCurrent.bUIExit = player.GetButtonDown("UIExit");
			inputDataCurrent.bUIExitUp = player.GetButtonUp("UIExit");
			inputDataCurrent.bUIReset = player.GetButtonDown("UIReset");
			inputDataCurrent.bUIToggleFilter = player.GetButtonDown("UIToggleFilter");
			inputDataCurrent.UIMove = player.GetAxis2D("UIHorizontal", "UIVertical");
			inputDataCurrent.bToggleLeaderboard = player.GetButtonDown("UILeaderboard");
			inputDataCurrent.bOpenScreenshotMode = player.GetButtonDown("UIScreenshotMode");
			inputDataCurrent.bOpenSettings = player.GetButtonDown("UISettings");
			inputDataCurrent.bAccountChangeRequested = player.GetButtonDown("UIAccountChange");
			inputDataCurrent.bToggleMenu = player.GetButtonDown("Menu Overlay Toggle");
			inputDataCurrent.bUIGenerateIsland = player.GetButtonDown("UIGenerateIsland");
			inputDataCurrent.fUISlider = player.GetAxisRaw("UISlider");
			inputDataCurrent.bUIResetIsland = player.GetButtonDown("UIResetIsland");
			inputDataCurrent.imLastUsedInputMethod = imLastUsedInputMethod;
			inputDataCurrent.bToggleArchiveIsland = player.GetButtonDown("UIArchiveIsland");
			inputDataCurrent.bUIModify = player.GetButtonDown("UIModify");
			inputDataCurrent.bUINewScreenshot = player.GetButtonDown("UINewScreenshot");
			inputDataCurrent.bUIModifyText = player.GetButtonDown("UIModifyText");
			CalculateCameraMoveAndRotation();
			if (InputDataCurrent.imLastUsedInputMethod != InputMode.Controller)
			{
				CalculateCameraZoom();
			}
			return;
		}
		inputDataCurrent.imLastUsedInputMethod = imLastUsedInputMethod;
		bMouseWheelUsed = Input.mouseScrollDelta.magnitude != 0f;
		inputDataCurrent.buildingPackSelectionScroll = 0;
		if (player.GetButtonDown("Select Building Pack Left"))
		{
			inputDataCurrent.buildingPackSelectionScroll--;
		}
		if (player.GetButtonDown("Select Building Pack Right"))
		{
			inputDataCurrent.buildingPackSelectionScroll++;
		}
		CalculateCameraMoveAndRotation();
		float num = CalculateCameraZoom();
		inputDataCurrent.bsBuildingPlace = GetButtonState("Building Place");
		inputDataCurrent.bsBuildingDeselect = GetButtonState("Building Deselect");
		inputDataCurrent.bsSelectNextBuilding = GetButtonState("Select Next Building");
		if (inputDataCurrent.bsSelectNextBuilding == ButtonState.Pressed)
		{
			fSelectNextBuildiningHoldTime += Time.deltaTime;
			if (fSelectNextBuildiningHoldTime > fScrollStartTime)
			{
				int num2 = (int)Mathf.Ceil((fSelectNextBuildiningHoldTime - fScrollStartTime) / fScrollIntervals);
				int num3 = (int)Mathf.Ceil((fSelectNextBuildiningHoldTime - Time.deltaTime - fScrollStartTime) / fScrollIntervals);
				if (num2 != num3)
				{
					inputDataCurrent.bsSelectNextBuilding = ButtonState.Down;
				}
			}
		}
		else
		{
			fSelectNextBuildiningHoldTime = 0f;
		}
		inputDataCurrent.bsSelectPreviousBuilding = GetButtonState("Select Previous Building");
		if (inputDataCurrent.bsSelectPreviousBuilding == ButtonState.Pressed)
		{
			fSelectPreviousBuildingHoldTime += Time.deltaTime;
			if (fSelectPreviousBuildingHoldTime > fScrollStartTime)
			{
				int num4 = (int)Mathf.Ceil((fSelectPreviousBuildingHoldTime - fScrollStartTime) / fScrollIntervals);
				int num5 = (int)Mathf.Ceil((fSelectPreviousBuildingHoldTime - Time.deltaTime - fScrollStartTime) / fScrollIntervals);
				if (num4 != num5)
				{
					inputDataCurrent.bsSelectPreviousBuilding = ButtonState.Down;
				}
			}
		}
		else
		{
			fSelectPreviousBuildingHoldTime = 0f;
		}
		float axisRaw = player.GetAxisRaw("Building Rotate");
		inputDataCurrent.fRotateBuilding = axisRaw - fBuildingRotationInLastFrame;
		if (axisRaw > 0f)
		{
			inputDataCurrent.fRotateBuilding = Mathf.Max(inputDataCurrent.fRotateBuilding, 0f);
		}
		else if (axisRaw < 0f)
		{
			inputDataCurrent.fRotateBuilding = Mathf.Min(inputDataCurrent.fRotateBuilding, 0f);
		}
		else
		{
			inputDataCurrent.fRotateBuilding = 0f;
		}
		if (axisRaw == fBuildingRotationInLastFrame && axisRaw != 0f)
		{
			fHoldBuildingRotationTime += Time.deltaTime;
			if (fHoldBuildingRotationTime > fScrollStartTime)
			{
				int num6 = (int)Mathf.Ceil((fHoldBuildingRotationTime - fScrollStartTime) / fScrollIntervals);
				int num7 = (int)Mathf.Ceil((fHoldBuildingRotationTime - Time.deltaTime - fScrollStartTime) / fScrollIntervals);
				if (num6 != num7)
				{
					inputDataCurrent.fRotateBuilding = Mathf.Sign(axisRaw);
				}
			}
		}
		else
		{
			fHoldBuildingRotationTime = 0f;
		}
		fBuildingRotationInLastFrame = axisRaw;
		fBuildingRotation += inputDataCurrent.fRotateBuilding;
		inputDataCurrent.fRotateBuilding = Mathf.Sign(fBuildingRotation) * Mathf.Floor(Mathf.Abs(fBuildingRotation));
		fBuildingRotation -= inputDataCurrent.fRotateBuilding;
		if (UiBuildingButtonManager.singleton.GoSelectedButton != null)
		{
			inputDataCurrent.fCameraZoom -= inputDataCurrent.fRotateBuilding;
			if (num > 0f)
			{
				inputDataCurrent.fCameraZoom = Mathf.Max(inputDataCurrent.fCameraZoom, 0f);
			}
			else if (num < 0f)
			{
				inputDataCurrent.fCameraZoom = Mathf.Min(inputDataCurrent.fCameraZoom, 0f);
			}
			else
			{
				inputDataCurrent.fCameraZoom = 0f;
			}
		}
		inputDataCurrent.bQuickSelect[0] = player.GetButtonDown("Building Hot Select 1");
		inputDataCurrent.bQuickSelect[1] = player.GetButtonDown("Building Hot Select 2");
		inputDataCurrent.bQuickSelect[2] = player.GetButtonDown("Building Hot Select 3");
		inputDataCurrent.bQuickSelect[3] = player.GetButtonDown("Building Hot Select 4");
		inputDataCurrent.bQuickSelect[4] = player.GetButtonDown("Building Hot Select 5");
		inputDataCurrent.bQuickSelect[5] = player.GetButtonDown("Building Hot Select 6");
		inputDataCurrent.bQuickSelect[6] = player.GetButtonDown("Building Hot Select 7");
		inputDataCurrent.bQuickSelect[7] = player.GetButtonDown("Building Hot Select 8");
		inputDataCurrent.bQuickSelect[8] = player.GetButtonDown("Building Hot Select 9");
		inputDataCurrent.bQuickSelect[9] = player.GetButtonDown("Building Hot Select 0");
		inputDataCurrent.bToggleMenu = player.GetButtonDown("Menu Overlay Toggle");
		inputDataCurrent.bToggleLeaderboard = player.GetButtonDown("UILeaderboard");
		inputDataCurrent.bOpenScreenshotMode = player.GetButtonDown("UIScreenshotMode");
		inputDataCurrent.bOpenSettings = player.GetButtonDown("UISettings");
		inputDataCurrent.bUIConfirm = player.GetButtonDown("UISubmit");
		inputDataCurrent.bUICancel = player.GetButtonDown("UICancel");
		inputDataCurrent.bUIExit = player.GetButtonDown("UIExit");
		inputDataCurrent.bUIExitUp = player.GetButtonUp("UIExit");
		inputDataCurrent.UIMove = player.GetAxis2D("UIHorizontal", "UIVertical");
		inputDataCurrent.bAccountChangeRequested = player.GetButtonDown("UIAccountChange");
		inputDataCurrent.bUIGenerateIsland = player.GetButtonDown("UIGenerateIsland");
		inputDataCurrent.fUISlider = player.GetAxisRaw("UISlider");
		inputDataCurrent.bUIResetIsland = player.GetButtonDown("UIResetIsland");
		inputDataCurrent.bToggleArchiveIsland = player.GetButtonDown("UIArchiveIsland");
		inputDataCurrent.bUIModify = player.GetButtonDown("UIModify");
		inputDataCurrent.bUINewScreenshot = player.GetButtonDown("UINewScreenshot");
		inputDataCurrent.bUIModifyText = player.GetButtonDown("UIModifyText");
		inputDataCurrent.v3PointerScreenPos = v3PointerScreenPos;
		inputDataCurrent.rayPointerToWorld = MainCamera.FullResCamera.ScreenPointToRay(inputDataCurrent.v3PointerScreenPos);
		inputDataCurrent.v2PointerScreenPosNormalizedFromHeight = new Vector2(inputDataCurrent.v3PointerScreenPos.x / (float)Screen.height, inputDataCurrent.v3PointerScreenPos.y / (float)Screen.height);
		inputDataCurrent.bUndo = player.GetButtonDown("Undo");
		inputDataCurrent.bsNextIsland = GetButtonState("Next Island");
		inputDataCurrent.bCenterPointer = player.GetButtonDown("Center Pointer");
		inputDataCurrent.bChangeModel = player.GetButtonDown("Change Model");
	}

	private void CalculateCameraMoveAndRotation()
	{
		inputDataCurrent.fCameraRotateWithButtons = player.GetAxisRaw("Camera Rotate");
		inputDataCurrent.fCameraMoveXWithButtons = player.GetAxisRaw("Camera Pan X");
		inputDataCurrent.fCameraMoveZWithButtons = player.GetAxisRaw("Camera Pan Z");
		inputDataCurrent.v2CameraDragDelta = player.GetAxis2DRaw("Camera Drag Delta X", "Camera Drag Delta Y");
		inputDataCurrent.bsCameraDragPan = GetButtonState("Camera Drag Pan");
		inputDataCurrent.bsCameraDragRotate = GetButtonState("Camera Drag Rotate");
		inputDataCurrent.bCameraCenterView = player.GetButtonDown("Camera Center View");
	}

	private float CalculateCameraZoom()
	{
		float axisRaw = player.GetAxisRaw("Camera Zoom");
		inputDataCurrent.fCameraZoom = axisRaw - fCameraZoomInLastFrame;
		if (axisRaw > 0f)
		{
			inputDataCurrent.fCameraZoom = Mathf.Max(inputDataCurrent.fCameraZoom, 0f);
		}
		else if (axisRaw < 0f)
		{
			inputDataCurrent.fCameraZoom = Mathf.Min(inputDataCurrent.fCameraZoom, 0f);
		}
		else
		{
			inputDataCurrent.fCameraZoom = 0f;
		}
		if (axisRaw == fCameraZoomInLastFrame && axisRaw != 0f)
		{
			fCameraHoldZoomTime += Time.deltaTime;
			if (fCameraHoldZoomTime > fScrollStartTime)
			{
				int num = (int)Mathf.Ceil((fCameraHoldZoomTime - fScrollStartTime) / fScrollIntervals);
				int num2 = (int)Mathf.Ceil((fCameraHoldZoomTime - Time.deltaTime - fScrollStartTime) / fScrollIntervals);
				if (num != num2)
				{
					inputDataCurrent.fCameraZoom = Mathf.Sign(axisRaw);
				}
			}
		}
		else
		{
			fCameraHoldZoomTime = 0f;
		}
		fCameraZoomInLastFrame = axisRaw;
		return axisRaw;
	}

	private ButtonState GetButtonState(string _strButtonName)
	{
		if (player.GetButtonDown(_strButtonName))
		{
			return ButtonState.Down;
		}
		if (player.GetButtonUp(_strButtonName))
		{
			return ButtonState.Up;
		}
		if (player.GetButton(_strButtonName))
		{
			return ButtonState.Pressed;
		}
		return ButtonState.NotPressed;
	}

	public static bool ShouldUsePS4ControllerIcons(Guid guid)
	{
		for (int i = 0; i < Singleton.m_PS4ControllerIconsCachedGUIDs.Count; i++)
		{
			if (Singleton.m_PS4ControllerIconsCachedGUIDs[i] == guid)
			{
				return true;
			}
		}
		return false;
	}

	public static bool ShouldUseNintendoProControllerIcons(Guid guid)
	{
		for (int i = 0; i < Singleton.m_NintendoProControllerIconsCachedGUIDs.Count; i++)
		{
			if (Singleton.m_NintendoProControllerIconsCachedGUIDs[i] == guid)
			{
				return true;
			}
		}
		return false;
	}

	public static bool ShouldUseXBoxControllerIcons(Guid guid)
	{
		for (int i = 0; i < Singleton.m_XBoxControllerIconsCachedGUIDs.Count; i++)
		{
			if (Singleton.m_XBoxControllerIconsCachedGUIDs[i] == guid)
			{
				return true;
			}
		}
		return false;
	}

	public static bool ShouldStadiaControllerIcons(Guid guid)
	{
		for (int i = 0; i < Singleton.m_StadiaControllerIconsCachedGUIDs.Count; i++)
		{
			if (Singleton.m_StadiaControllerIconsCachedGUIDs[i] == guid)
			{
				return true;
			}
		}
		return false;
	}
}
