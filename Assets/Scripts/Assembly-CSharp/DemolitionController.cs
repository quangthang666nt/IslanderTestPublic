using SCS.Gameplay;
using UnityEngine;

public class DemolitionController : MonoBehaviour
{
	private const float HOLD_THRESHOLD = 0.1f;

	private float fMouseDraggedDistanceLeft;

	private Vector2 v2MouseDownPositionLeft;

	private float fMouseMoveTreshholdLeft = 0.01f;

	private static bool locked;

	private static bool unlockNextFrame;

	private static bool lockedThisFrame;

	public static readonly float REDUCE_SPEED_LIMIT = 0.75f;

	public static readonly float START_FILL_PERCENT = 0.1f;

	public static readonly float REDUCE_SPEED = 0.3f;

	private bool correctUIState;

	private UiCanvasManager uiManager;

	private float holdtimer;

	private float holdMaxTime = 2f;

	private static bool bBuildingWasJustCreatedAndShouldNotBeRemoved = false;

	[SerializeField]
	private bool useDemolishAll;

	[SerializeField]
	private bool demolishSameType;

	[Header("Event Callbacks")]
	[SerializeField]
	private FloatEventHandler onFillerEvent;

	[SerializeField]
	private CustomEventHandler onDemolitionDone;

	private InputManager.InputData inputData;

	public static bool Locked => locked;

	public static void BuildingWasJustCreatedAndShouldNotBeRemoved()
	{
		bBuildingWasJustCreatedAndShouldNotBeRemoved = true;
	}

	public static void Lock()
	{
		lockedThisFrame = true;
		locked = true;
		unlockNextFrame = false;
	}

	public static void Unlock()
	{
		if (!lockedThisFrame)
		{
			unlockNextFrame = true;
		}
	}

	private void Start()
	{
		uiManager = UiCanvasManager.Singleton;
	}

	private void LateUpdate()
	{
		correctUIState = uiManager.UIState == UiCanvasManager.EUIState.InGamePlaying && uiManager.UIStateLastFrame == UiCanvasManager.EUIState.InGamePlaying;
		if (!locked && correctUIState)
		{
			inputData = InputManager.Singleton.InputDataCurrent;
			if (UiBuildingButtonManager.singleton.GoSelectedButton == null && LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox)
			{
				bool flag = false;
				CheckDemolishAll();
				if (inputData.bsBuildingPlace == InputManager.ButtonState.Up)
				{
					if (holdtimer > 0.1f)
					{
						ClearDemolishAll();
						return;
					}
					ClearDemolishAll(destroySelected: true);
					fMouseDraggedDistanceLeft = 0f;
					if (InputManager.Singleton.InputDataCurrent.imLastUsedInputMethod == InputManager.InputMode.Controller)
					{
						fMouseDraggedDistanceLeft = 100000f;
						flag = true;
					}
					v2MouseDownPositionLeft = inputData.v2PointerScreenPosNormalizedFromHeight;
				}
				if (inputData.bsBuildingPlace == InputManager.ButtonState.Pressed)
				{
					fMouseDraggedDistanceLeft += (inputData.v2PointerScreenPosNormalizedFromHeight - v2MouseDownPositionLeft).magnitude;
					v2MouseDownPositionLeft = inputData.v2PointerScreenPosNormalizedFromHeight;
				}
				if (!bBuildingWasJustCreatedAndShouldNotBeRemoved && ((inputData.bsBuildingPlace == InputManager.ButtonState.Up && !UIBuildingBarBackground.singleton.BMouseOver) || flag) && (fMouseDraggedDistanceLeft < fMouseMoveTreshholdLeft || flag))
				{
					TriggerDemolish();
				}
			}
			if (inputData.bsBuildingPlace == InputManager.ButtonState.Up)
			{
				bBuildingWasJustCreatedAndShouldNotBeRemoved = false;
			}
		}
		if (unlockNextFrame)
		{
			locked = false;
			unlockNextFrame = false;
		}
		lockedThisFrame = false;
	}

	private void CheckDemolishAll()
	{
		if (ColorBaker.singleton.BuildingFindMouseOver() == null || !useDemolishAll)
		{
			ClearDemolishAll();
			return;
		}
		if (inputData.bsBuildingPlace == InputManager.ButtonState.Down)
		{
			holdtimer = 0f;
		}
		else if (inputData.bsBuildingPlace == InputManager.ButtonState.Pressed)
		{
			holdtimer += ((holdtimer / holdMaxTime <= REDUCE_SPEED_LIMIT) ? Time.deltaTime : (Time.deltaTime * REDUCE_SPEED));
		}
		if (holdtimer >= holdMaxTime)
		{
			holdtimer = 0f;
			DecideDemolishEffect();
		}
		onFillerEvent?.Dispatch(holdtimer / holdMaxTime);
	}

	private void DecideDemolishEffect()
	{
		if (demolishSameType)
		{
			DemolishSameType();
		}
		else
		{
			DemolishAll();
		}
		SaveLoadManager.PerformAutosave();
		onDemolitionDone?.Dispatch();
		ClearDemolishAll();
	}

	private void ClearDemolishAll(bool destroySelected = false)
	{
		holdtimer = 0f;
		onFillerEvent?.Dispatch(0f);
		if (destroySelected)
		{
			Building building = ColorBaker.singleton.BuildingFindMouseOver();
			if ((bool)building)
			{
				building.Demolish();
			}
		}
	}

	[ContextMenu("Demolish All")]
	private void DemolishAll()
	{
		Building[] array = Object.FindObjectsOfType<Building>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Demolish();
		}
	}

	private void DemolishSameType()
	{
		Building building = ColorBaker.singleton.BuildingFindMouseOver();
		Building[] array = Object.FindObjectsOfType<Building>();
		foreach (Building building2 in array)
		{
			if (building2.iID == building.iID)
			{
				building2.Demolish();
			}
		}
	}

	private void TriggerDemolish()
	{
		Building building = ColorBaker.singleton.BuildingFindMouseOver();
		if (!(building == null))
		{
			building.Demolish(playSound: true);
			SaveLoadManager.PerformAutosave();
		}
	}
}
