using UnityEngine;

public class CursorManager : MonoBehaviour
{
	public enum CursorState
	{
		normal = 0,
		demolition = 1,
		disabled = 2
	}

	[Help("Enables / disables the cursor based on various game states.", MessageType.Info)]
	[SerializeField]
	private Texture2D texCursorDefault;

	[SerializeField]
	private Texture2D texCursorDemolish;

	public static CursorManager singleton;

	private InputManager inputManager;

	private UiCanvasManager uiCanvasManager;

	private LocalGameManager localGameManager;

	private UiBuildingButtonManager uiBuildingButtonManager;

	private ColorBaker colorBaker;

	public Vector2 v2CursorSize = Vector2.zero;

	public Vector2 v2CursorSizeLinux = new Vector2(5f, 2f);

	public Vector2 v2CursorSizeMac = new Vector2(5f, 2f);

	public RectTransform rtransPointer;

	public RectTransform rtransPointerDemolition;

	public RectTransform rtransCanvas;

	[SerializeField]
	private EnhancedCursor cursorController;

	public CursorState currentCursorState = CursorState.disabled;

	public CursorState currentPointerState = CursorState.disabled;

	private void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		EnableDefaultCursor();
		inputManager = InputManager.Singleton;
		uiCanvasManager = UiCanvasManager.Singleton;
		localGameManager = LocalGameManager.singleton;
		uiBuildingButtonManager = UiBuildingButtonManager.singleton;
		colorBaker = ColorBaker.singleton;
	}

	private void Update()
	{
		DecideCursorState();
		DecidePointerState();
	}

	private void DecidePointerState()
	{
		if (inputManager.InputDataCurrent.imLastUsedInputMethod != InputManager.InputMode.Controller)
		{
			DisablePointer();
			return;
		}
		if (currentCursorState != CursorState.disabled || UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.InGamePlaying || (uiBuildingButtonManager.GoSelectedButton == null && !uiBuildingButtonManager.IsDeleteBuildingButtonSelected()))
		{
			DisablePointer();
			return;
		}
		if (localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox && uiBuildingButtonManager.GoSelectedButton == null && colorBaker.BuildingFindMouseOver() != null && !DemolitionController.Locked)
		{
			EnableDemolitionPointer();
		}
		else
		{
			EnableNormalPointer();
		}
		if (inputManager.InputDataCurrent.bCenterPointer)
		{
			inputManager.ResetPointer();
		}
		rtransPointer.localPosition = (Vector2)inputManager.InputDataCurrent.v3PointerScreenPos;
		rtransPointer.localPosition = new Vector2(rtransPointer.localPosition.x / (float)Screen.width * rtransCanvas.sizeDelta.x - rtransCanvas.sizeDelta.x * 0.5f, rtransPointer.localPosition.y / (float)Screen.height * rtransCanvas.sizeDelta.y - rtransCanvas.sizeDelta.y * 0.5f);
		rtransPointerDemolition.localPosition = rtransPointer.localPosition;
	}

	private void DisablePointer()
	{
		if (currentPointerState != CursorState.disabled)
		{
			rtransPointer.gameObject.SetActive(value: false);
			rtransPointerDemolition.gameObject.SetActive(value: false);
			currentPointerState = CursorState.disabled;
		}
	}

	private void EnableNormalPointer()
	{
		if (currentPointerState != 0)
		{
			rtransPointer.gameObject.SetActive(value: true);
			rtransPointerDemolition.gameObject.SetActive(value: false);
			currentPointerState = CursorState.normal;
		}
	}

	private void EnableDemolitionPointer()
	{
		if (currentPointerState != CursorState.demolition)
		{
			rtransPointer.gameObject.SetActive(value: false);
			rtransPointerDemolition.gameObject.SetActive(value: true);
			currentPointerState = CursorState.demolition;
		}
	}

	private void DecideCursorState()
	{
		if (inputManager.InputDataCurrent.imLastUsedInputMethod != 0)
		{
			DisableCursor();
		}
		else if (uiCanvasManager.IsInScreenshotMode())
		{
			DisableCursor();
		}
		else if (localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox && uiBuildingButtonManager.GoSelectedButton == null && colorBaker.BuildingFindMouseOver() != null && uiCanvasManager.UIState == UiCanvasManager.EUIState.InGamePlaying && !DemolitionController.Locked)
		{
			EnableDemolisionCursor();
		}
		else
		{
			EnableDefaultCursor();
		}
	}

	public static void EnableDefaultCursor()
	{
		if (singleton.currentCursorState != 0)
		{
			singleton.currentCursorState = CursorState.normal;
			singleton.cursorController.SetDefaultCursor();
		}
	}

	public static void EnableDemolisionCursor()
	{
		if (singleton.currentCursorState != CursorState.demolition)
		{
			singleton.currentCursorState = CursorState.demolition;
			singleton.cursorController.StartDemolitionMode();
		}
	}

	public static void DisableCursor()
	{
		if (singleton.currentCursorState != CursorState.disabled)
		{
			Cursor.visible = false;
			singleton.currentCursorState = CursorState.disabled;
			singleton.cursorController.ShowCursor(isvisible: false);
		}
	}

	public static void TryEnableCursor(bool enable)
	{
		if (!UiCanvasManager.Singleton.IsInScreenshotMode())
		{
			singleton.cursorController.ShowCursor(enable);
		}
	}
}
