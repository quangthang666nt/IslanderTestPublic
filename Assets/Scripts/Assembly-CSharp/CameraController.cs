using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController singleton;

	public bool bReactToDeltaInputs = true;

	[Header("Button Input")]
	[SerializeField]
	private float fRotationSpeed = 90f;

	[SerializeField]
	private bool bInvertRotation;

	[SerializeField]
	private float fTiltSpeed = 90f;

	[SerializeField]
	private bool bInvertTilt;

	[Header("Pointer Input")]
	[SerializeField]
	private float fDragRotationSpeed = 135f;

	[SerializeField]
	private bool bDragInvertRotation;

	[SerializeField]
	private float fDragTiltSpeed = 135f;

	[SerializeField]
	private bool bDragInvertTilt;

	[Header("Translation")]
	[SerializeField]
	public float fButtonTranslationSpeed = 5f;

	[SerializeField]
	private float fBorderTranslationSpeed = 3f;

	[SerializeField]
	private float fDragTranslationSpeed = 1f;

	[SerializeField]
	private float fMaxTranslationDistance = 9f;

	public Vector3 v3TranslationOrigin;

	[SerializeField]
	private bool bScreenBorderScroll;

	[Header("Smoothing")]
	[SerializeField]
	private float fRotationSmoothTime = 0.3f;

	[SerializeField]
	private float fTiltSmoothTime = 0.3f;

	[SerializeField]
	private float fButtonTranslationSmoothTime = 0.3f;

	[SerializeField]
	private float fBorderTranslationSmoothTime = 0.1f;

	[SerializeField]
	private float fDragTranslationSmoothTime = 0.3f;

	[Header("Tilt Snap Settings")]
	[SerializeField]
	private float fTiltHighLimit = 20f;

	[SerializeField]
	private float fTiltLowLimit = -20f;

	[SerializeField]
	private float fTiltSnapBackSmoothTime = 0.3f;

	[SerializeField]
	private float fTiltSnapBackDelay = 2f;

	[Header("Center View Input")]
	[SerializeField]
	private float fCenterViewTime = 1f;

	[SerializeField]
	private AnimationCurve acCenterViewAnimationCurve;

	[SerializeField]
	private bool bCenterPosition = true;

	[SerializeField]
	private bool bCenterRotation = true;

	[SerializeField]
	private bool bCenterZoom = true;

	[SerializeField]
	private float fZoomMaxHeight = 3f;

	[SerializeField]
	private Transform tCameraCenter;

	private SaveLoadManager saveLoadManager;

	private float fMovementWithKeys;

	private float fMovementWithMouse;

	private float fRotationWithKeys;

	private float fRotationWithMouse;

	private float m_ZoomWithMouse;

	[HideInInspector]
	public bool bAllowCamMouseRotation = true;

	private bool bCenteringView;

	private float fCenterViewElapsedTime;

	private Vector3 v3CenterViewStartPosition = Vector3.zero;

	private Quaternion qCenterViewStartRotation = Quaternion.identity;

	private float fCurrentTilt;

	private float fCurrentRotation;

	private float fDesiredTilt;

	private float fDesiredRotation;

	private Vector3 v3CurrentTranslation = Vector3.zero;

	private Vector3 v3DesiredTranslation = Vector3.zero;

	private Vector3 v3TranslationRef = Vector3.zero;

	private Vector3 v3CurrentBorderTranslation = Vector3.zero;

	private Vector3 v3DesiredBorderTranslation = Vector3.zero;

	private Vector3 v3BorderTranslationRef = Vector3.zero;

	private Vector3 v3CurrentDragTranslation = Vector3.zero;

	private Vector3 v3DesiredDragTranslation = Vector3.zero;

	private Vector3 v3DragTranslationRef = Vector3.zero;

	private float fTiltDampRef;

	private float fRotationDampRef;

	private float fRotationInput;

	private float fTiltInput;

	private float fMoveXInput;

	private float fMoveYInput;

	private float fMouseMoveXInput;

	private float fMouseMoveYInput;

	private float fRotationDragHorizontal;

	private float fRotationDragVertical;

	private float fTranslationDragHorizontal;

	private float fTranslationDragVertical;

	private float fTiltSnapBackDampRef;

	private float fTiltSnapBackTimer;

	private Vector3 v3NextPosition;

	private Quaternion qNextRotation;

	private CameraZoom czCameraZoom;

	private Transform tDefaultCameraCenter;

	private const int screenBorderBuffer = 3;

	public float FMovementWithKeys => fMovementWithKeys;

	public float FMovementWithMouse => fMovementWithMouse;

	public float FRotationWithKeys => fRotationWithKeys;

	public float FRotationWithMouse => fRotationWithMouse;

	public float ZoomWithMouse
	{
		get
		{
			return m_ZoomWithMouse;
		}
		set
		{
			m_ZoomWithMouse = value;
		}
	}

	private int iIsRotationInverted
	{
		get
		{
			if (bInvertRotation)
			{
				return -1;
			}
			return 1;
		}
	}

	private int iIsTiltInverted
	{
		get
		{
			if (bInvertTilt)
			{
				return -1;
			}
			return 1;
		}
	}

	private int iIsDragRotationInverted
	{
		get
		{
			if (bDragInvertRotation)
			{
				return -1;
			}
			return 1;
		}
	}

	private int iIsDragTiltInverted
	{
		get
		{
			if (bDragInvertTilt)
			{
				return -1;
			}
			return 1;
		}
	}

	public float fButtonTranslationSpeedValue { get; set; }

	public void ResetTutorialStats()
	{
		fMovementWithKeys = 0f;
		fMovementWithMouse = 0f;
		fRotationWithKeys = 0f;
		fRotationWithMouse = 0f;
		m_ZoomWithMouse = 0f;
	}

	private void Awake()
	{
		singleton = this;
		czCameraZoom = GetComponentInChildren<CameraZoom>();
		tCameraCenter = (tDefaultCameraCenter = new GameObject("Default Camera Center").transform);
		tCameraCenter.position += Vector3.up * (fZoomMaxHeight / 2f);
	}

	private void Start()
	{
		saveLoadManager = SaveLoadManager.singleton;
		fButtonTranslationSpeedValue = fButtonTranslationSpeed;
	}

	private void Update()
	{
		if (bCenteringView)
		{
			CenterView();
			return;
		}
		InputManager.InputData inputDataCurrent = InputManager.Singleton.InputDataCurrent;
		if (saveLoadManager.ETransitionStateCurrent != 0)
		{
			InvalidateCameraInput();
		}
		if (inputDataCurrent.bCameraCenterView && HasToCenterView() && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.ArchiveIslandSavePrompt)
		{
			StartCenterView();
			return;
		}
		if (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.Settings || UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.Leaderboard || UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.ScreenshotMenu || SandboxGenerationView.IsOpened() || UiCanvasManager.Singleton.IsArchiveIslandOpen())
		{
			InvalidateCameraInput();
		}
		if (UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.InGamePlaying && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.ScreenshotMode && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.InCamTutorial && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.ArchiveIslandPhotoPrompt && InputManager.Singleton.imLastUsedInputMethod == InputManager.InputMode.Controller)
		{
			inputDataCurrent.fCameraMoveXWithButtons = 0f;
			inputDataCurrent.fCameraMoveZWithButtons = 0f;
		}
		fRotationInput = inputDataCurrent.fCameraRotateWithButtons * -1f * (float)iIsRotationInverted;
		fRotationInput *= fRotationSpeed * Time.deltaTime;
		fRotationWithKeys += Mathf.Abs(fRotationInput);
		fMoveXInput = inputDataCurrent.fCameraMoveXWithButtons;
		fMoveYInput = inputDataCurrent.fCameraMoveZWithButtons;
		fDesiredRotation += fRotationInput;
		fDesiredTilt += fTiltSpeed * fTiltInput * Time.deltaTime;
		v3DesiredTranslation = Vector3.zero;
		v3DesiredTranslation += fButtonTranslationSpeedValue * fMoveXInput * Time.deltaTime * base.transform.right;
		v3DesiredTranslation += fButtonTranslationSpeedValue * fMoveYInput * Time.deltaTime * base.transform.forward;
		fMovementWithKeys += v3DesiredTranslation.magnitude;
		if (bScreenBorderScroll)
		{
			if (inputDataCurrent.v3PointerScreenPos.x <= 3f)
			{
				fMouseMoveXInput = -1f;
			}
			else if (inputDataCurrent.v3PointerScreenPos.x >= (float)(Screen.width - 3))
			{
				fMouseMoveXInput = 1f;
			}
			else
			{
				fMouseMoveXInput = 0f;
			}
			if (inputDataCurrent.v3PointerScreenPos.y <= 3f)
			{
				fMouseMoveYInput = -1f;
			}
			else if (inputDataCurrent.v3PointerScreenPos.y >= (float)(Screen.height - 3))
			{
				fMouseMoveYInput = 1f;
			}
			else
			{
				fMouseMoveYInput = 0f;
			}
			v3DesiredBorderTranslation = Vector3.zero;
			v3DesiredBorderTranslation += fBorderTranslationSpeed * fMouseMoveXInput * Time.deltaTime * base.transform.right;
			v3DesiredBorderTranslation += fBorderTranslationSpeed * fMouseMoveYInput * Time.deltaTime * base.transform.forward;
		}
		v3DesiredDragTranslation = Vector3.zero;
		bool flag = inputDataCurrent.bsCameraDragPan == InputManager.ButtonState.Down || inputDataCurrent.bsCameraDragPan == InputManager.ButtonState.Pressed;
		if (bReactToDeltaInputs && (!UiBuildingButtonManager.singleton.GoBuildingPreview || SettingsManager.Singleton.CurrentData.gameplayData.dragCameraInBuildmode))
		{
			fTranslationDragHorizontal = 0f;
			if (InputManager.Singleton.InputModeCurrent == InputManager.InputMode.Mouse && flag)
			{
				fTranslationDragHorizontal = InputManager.Singleton.InputDataCurrent.v2CameraDragDelta.x * -1f;
			}
			v3DesiredDragTranslation += base.transform.right * fDragTranslationSpeed * fTranslationDragHorizontal;
			fTranslationDragVertical = 0f;
			if (InputManager.Singleton.InputModeCurrent == InputManager.InputMode.Mouse && flag)
			{
				fTranslationDragVertical = InputManager.Singleton.InputDataCurrent.v2CameraDragDelta.y * -1f;
			}
			v3DesiredDragTranslation += base.transform.forward * fDragTranslationSpeed * fTranslationDragVertical;
		}
		fMovementWithMouse += v3DesiredDragTranslation.magnitude;
		bool flag2 = inputDataCurrent.bsCameraDragRotate == InputManager.ButtonState.Down || inputDataCurrent.bsCameraDragRotate == InputManager.ButtonState.Pressed;
		if (bReactToDeltaInputs)
		{
			fRotationDragHorizontal = 0f;
			if (InputManager.Singleton.InputModeCurrent == InputManager.InputMode.Mouse && flag2)
			{
				fRotationDragHorizontal = InputManager.Singleton.InputDataCurrent.v2CameraDragDelta.x * (float)iIsDragRotationInverted;
			}
			float num = fDragRotationSpeed * fRotationDragHorizontal;
			if (bAllowCamMouseRotation)
			{
				fDesiredRotation += num;
				fRotationWithMouse += Mathf.Abs(num);
			}
		}
		if (fTiltInput == 0f && fRotationDragVertical == 0f && fCurrentTilt != 0f && !flag2)
		{
			fTiltSnapBackTimer += Time.deltaTime;
			if (fTiltSnapBackTimer >= fTiltSnapBackDelay)
			{
				fDesiredTilt = Mathf.SmoothDamp(fDesiredTilt, 0f, ref fTiltSnapBackDampRef, fTiltSnapBackSmoothTime);
			}
		}
		else
		{
			fTiltSnapBackTimer = 0f;
		}
		if (fDesiredTilt > fTiltHighLimit)
		{
			fDesiredTilt = fTiltHighLimit;
		}
		if (fDesiredTilt < fTiltLowLimit)
		{
			fDesiredTilt = fTiltLowLimit;
		}
		fCurrentRotation = Mathf.SmoothDamp(fCurrentRotation, fDesiredRotation, ref fRotationDampRef, fRotationSmoothTime);
		fCurrentTilt = Mathf.SmoothDamp(fCurrentTilt, fDesiredTilt, ref fTiltDampRef, fTiltSmoothTime);
		v3CurrentTranslation = Vector3.SmoothDamp(v3CurrentTranslation, v3DesiredTranslation, ref v3TranslationRef, fButtonTranslationSmoothTime);
		v3CurrentBorderTranslation = Vector3.SmoothDamp(v3CurrentBorderTranslation, v3DesiredBorderTranslation, ref v3BorderTranslationRef, fBorderTranslationSmoothTime);
		v3CurrentDragTranslation = Vector3.SmoothDamp(v3CurrentDragTranslation, v3DesiredDragTranslation, ref v3DragTranslationRef, fDragTranslationSmoothTime);
		qNextRotation = Quaternion.Euler(fCurrentTilt, fCurrentRotation, base.transform.rotation.eulerAngles.z);
		if (!bScreenBorderScroll || v3DesiredBorderTranslation == Vector3.zero)
		{
			v3NextPosition = base.transform.position + v3CurrentTranslation;
			v3NextPosition += v3CurrentBorderTranslation;
		}
		else
		{
			v3NextPosition = base.transform.position + v3CurrentBorderTranslation;
		}
		v3NextPosition += v3CurrentDragTranslation;
		float num2 = v3NextPosition.x - v3TranslationOrigin.x;
		float num3 = v3NextPosition.z - v3TranslationOrigin.z;
		if (num2 > fMaxTranslationDistance)
		{
			v3NextPosition.x = v3TranslationOrigin.x + fMaxTranslationDistance;
		}
		else if (num2 < 0f - fMaxTranslationDistance)
		{
			v3NextPosition.x = v3TranslationOrigin.x - fMaxTranslationDistance;
		}
		if (num3 > fMaxTranslationDistance)
		{
			v3NextPosition.z = v3TranslationOrigin.z + fMaxTranslationDistance;
		}
		else if (num3 < 0f - fMaxTranslationDistance)
		{
			v3NextPosition.z = v3TranslationOrigin.z - fMaxTranslationDistance;
		}
		base.transform.SetPositionAndRotation(v3NextPosition, qNextRotation);
	}

	public void SetToDefaultPos()
	{
		v3NextPosition.x = 0f;
		v3NextPosition.z = 0f;
		base.transform.SetPositionAndRotation(v3NextPosition, qNextRotation);
	}

	public void SetCameraCenter(Transform building)
	{
		if ((bool)building)
		{
			tCameraCenter = building;
		}
		else
		{
			tCameraCenter = tDefaultCameraCenter;
		}
	}

	private bool HasToCenterView()
	{
		if (!bCenterPosition && !bCenterRotation)
		{
			return bCenterZoom;
		}
		return true;
	}

	private void StartCenterView()
	{
		bCenteringView = true;
		fCenterViewElapsedTime = 0f;
		if (bCenterPosition)
		{
			v3CenterViewStartPosition = base.transform.position;
		}
		if (bCenterRotation)
		{
			qCenterViewStartRotation = base.transform.rotation;
		}
		if (bCenterZoom)
		{
			czCameraZoom.SetZoom(fCenterViewTime, 0f);
		}
	}

	private void CenterView()
	{
		fCenterViewElapsedTime += Time.deltaTime;
		if (bCenterPosition)
		{
			base.transform.position = Vector3.Lerp(v3CenterViewStartPosition, new Vector3(tCameraCenter.position.x, 0f, tCameraCenter.position.z), acCenterViewAnimationCurve.Evaluate(fCenterViewElapsedTime / fCenterViewTime));
		}
		if (bCenterRotation)
		{
			base.transform.rotation = Quaternion.Lerp(qCenterViewStartRotation, Quaternion.Euler(0f, tCameraCenter.eulerAngles.y, 0f), acCenterViewAnimationCurve.Evaluate(fCenterViewElapsedTime / fCenterViewTime));
		}
		if (fCenterViewElapsedTime >= fCenterViewTime)
		{
			bCenteringView = false;
			if (bCenterPosition)
			{
				base.transform.position = new Vector3(tCameraCenter.position.x, 0f, tCameraCenter.position.z);
			}
			if (bCenterRotation)
			{
				base.transform.rotation = Quaternion.Euler(0f, tCameraCenter.eulerAngles.y, 0f);
				fCurrentRotation = tCameraCenter.eulerAngles.y;
			}
			v3CurrentTranslation = Vector3.zero;
			v3TranslationRef = Vector3.zero;
			fDesiredRotation = fCurrentRotation;
			fRotationDampRef = 0f;
		}
	}

	private void InvalidateCameraInput()
	{
		InputManager.InputData inputDataCurrent = InputManager.Singleton.InputDataCurrent;
		inputDataCurrent.bCameraCenterView = false;
		inputDataCurrent.bsCameraDragPan = InputManager.ButtonState.NotPressed;
		inputDataCurrent.bsCameraDragRotate = InputManager.ButtonState.NotPressed;
		inputDataCurrent.fCameraMoveXWithButtons = 0f;
		inputDataCurrent.fCameraMoveZWithButtons = 0f;
		inputDataCurrent.fCameraRotateWithButtons = 0f;
		inputDataCurrent.fCameraZoom = 0f;
		inputDataCurrent.v2CameraDragDelta = Vector2.zero;
	}

	private void OnValidate()
	{
		SetCameraCenter(tCameraCenter);
	}
}
