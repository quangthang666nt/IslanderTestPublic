using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiBuildingButtonManager : MonoBehaviour
{
	public static UiBuildingButtonManager singleton;

	private LocalGameManager localGameManager;

	private CameraController cameraController;

	private InputManager inputManager;

	[HideInInspector]
	public UnityEvent eventOnBuildingPlace;

	[HideInInspector]
	public UnityEvent eventOnBuildingButtonClick;

	[HideInInspector]
	public UnityEvent eventOnBuildingCantPlace;

	[Help("This script is responsible for managing the building buttons and placing buildings.", MessageType.Info)]
	public Transform transButtonParentNormalMode;

	public Transform transBuildingParent;

	public Text m_BuildingLimitDebugDisplay;

	public int m_DefaultSandboxBuildingLimit;

	public int m_SwitchSandboxBuildingLimit = 100;

	public int m_PS4SandboxBuildingLimit = 150;

	public int m_XboxOneSandboxBuildingLimit = 150;

	private int m_SandboxBuildingLimit;

	[Header("Placement Settings")]
	[SerializeField]
	private LayerMask lmPlacementRay;

	[SerializeField]
	private float fRotationSteps = 36f;

	[SerializeField]
	private float fMaxRotationPerSecond = 1f;

	[SerializeField]
	private float fRotationStepsOnWalls = 8f;

	[Tooltip("If you move the mouse too much after pressing a building button, the click isn't counted (as youre probably moving the camera).")]
	[SerializeField]
	private float fMouseMoveTreshholdRight = 0.01f;

	[Tooltip("If you move the mouse too much after pressing a building button, the click isn't counted (as youre probably moving the camera).")]
	[SerializeField]
	private float fMouseMoveTreshholdLeft = 0.01f;

	[Tooltip("Building perview gets this color if it can't be placed.")]
	[SerializeField]
	private Color colCantBuild = Color.red;

	[Tooltip("This is the merge amount between the normal color and the cant-build-color")]
	[SerializeField]
	private float fCantBuildColBlend = 1f;

	[Tooltip("This are the shaders where we can apply the cant-build-color")]
	[SerializeField]
	private Shader[] shadersColCantBuild;

	[Tooltip("This are the material that its applied where we cant apply the cant-build-color")]
	[SerializeField]
	private Material matCantBuild;

	[SerializeField]
	private AudioClip auRotateBuildingRight;

	[SerializeField]
	private AudioClip auRotateBuildingLeft;

	[SerializeField]
	private float fRotateBuildingVolume = 0.5f;

	[SerializeField]
	private bool inputChangeModelEnabled = true;

	[Header("Snap Settings")]
	[Tooltip("Maximum snap distance for buildings.")]
	[SerializeField]
	private float fSnapDistance = 0.45f;

	[Tooltip("How often and far buildings should be moved towards the mouse.")]
	[SerializeField]
	private List<float> liFSnapStepIterations = new List<float>();

	[Tooltip("The directions the building should be moved in for each snap step interation. This ensures smoother gliding.")]
	[SerializeField]
	private List<float> liFSnapStepDirections = new List<float>();

	[Tooltip("Length multiplier for all of the directions above.")]
	[SerializeField]
	private List<float> liFSnapStepSize = new List<float>();

	private float fRotationStepSize = 10f;

	private float fRotationStepSizeOnWalls = 10f;

	private float fRotationPauseOnMac = 0.1f;

	private float fRotationPauseClock;

	private bool bMouseOverButtonClicked;

	private GameObject goBuildingPreview;

	private UiBuildingButton uiBuildingButtonSelected;

	private Vector3 v3LastValidPreviewPosition = Vector3.zero;

	private Vector2 v2MouseDownPositionRight = Vector2.zero;

	private Vector2 v2MouseDownPositionLeft = Vector2.zero;

	private float fMouseDraggedDistanceRight;

	private float fMouseDraggedDistanceLeft;

	private MeshRenderer[] arMeshRenderersPreview = new MeshRenderer[0];

	private List<Material> liSharedMaterialsOriginalPreview = new List<Material>();

	private List<Material> liSharedMaterialsModifiedPreview = new List<Material>();

	public bool bJustPlacedBuilding;

	private Vector2 v2LastPlacedBuildingMousePos = Vector2.zero;

	private bool bBuildingPlacableColor;

	private Building buildingInPlaceMode;

	private int iPlacementCooldown;

	private bool bBlockPlacementTillMouseReleased;

	private bool bPlusButtonSelected;

	private bool bUndoButtonSelected;

	private bool bDeleteButtonSelected;

	private Transform transButtonParent;

	private Vector3 v3LastPlacedBuildingPosition;

	private AudioSource audioSource;

	private bool variationChanged;

	public bool bBuildingPlacable;

	private bool bBuildingPreviewIsShown;

	private int iScorePreview;

	private List<ScoreContributorData> liScoreContributorsPreview = new List<ScoreContributorData>();

	private int iRotatedBuildingSteps;

	private List<UiBuildingButton> liBuildingButtonsExisting = new List<UiBuildingButton>();

	public Text m_DebugBuildingCountDisplay;

	private bool m_DebugLimitActivated;

	private bool m_SelectButtonNextFrame;

	private bool m_FirstBuildingPlaced;

	public GameObject[] m_ButtonPrompts;

	public GameObject GoBuildingPreview => goBuildingPreview;

	public List<UiBuildingButton> LiBuildingButtonsExisting => liBuildingButtonsExisting;

	public bool BBuildingPreviewIsShown => bBuildingPreviewIsShown;

	public int IScorePreview => iScorePreview;

	public bool BBuildingPlacable => bBuildingPlacable;

	public List<ScoreContributorData> LiScoreContributorsPreview => liScoreContributorsPreview;

	public int IRotatedBuildingSteps => iRotatedBuildingSteps;

	public UiBuildingButton GoSelectedButton
	{
		get
		{
			return uiBuildingButtonSelected;
		}
		set
		{
			UiBuildingButton uiBuildingButton = uiBuildingButtonSelected;
			uiBuildingButtonSelected = value;
			if (uiBuildingButton == uiBuildingButtonSelected)
			{
				return;
			}
			if ((bool)goBuildingPreview)
			{
				Building component = goBuildingPreview.GetComponent<Building>();
				if ((bool)component)
				{
					component.RemoveOutlineFeedback();
				}
				UnityEngine.Object.Destroy(goBuildingPreview);
			}
			if ((bool)buildingInPlaceMode && !buildingInPlaceMode.bPlaced)
			{
				buildingInPlaceMode.ReEnableDisabledOverlapGameObjects();
			}
			if (!uiBuildingButtonSelected)
			{
				buildingInPlaceMode = null;
				UITooltip.Singleton.Disable(InputManager.InputMode.Controller);
				return;
			}
			if ((bool)uiBuildingButton && (bool)uiBuildingButtonSelected && uiBuildingButton != uiBuildingButtonSelected)
			{
				variationChanged = false;
			}
			Building component2 = uiBuildingButtonSelected.goResourceBuilding.GetComponent<Building>();
			UITooltip.Singleton.EnableImmediate(component2.strBuildingName, component2, InputManager.InputMode.Controller);
			goBuildingPreview = UnityEngine.Object.Instantiate(uiBuildingButtonSelected.goResourceBuilding);
			StructureID component3 = goBuildingPreview.GetComponent<StructureID>();
			buildingInPlaceMode = goBuildingPreview.GetComponent<Building>();
			component3.enabled = false;
			buildingInPlaceMode.iID = component3.iID;
			UnityEngine.Random.State state = UnityEngine.Random.state;
			UnityEngine.Random.InitState(uiBuildingButtonSelected.iBuildingSeed);
			buildingInPlaceMode.uiBuildingButtonForUndo = uiBuildingButtonSelected;
			buildingInPlaceMode.iBuildingVariationSeedForUndo = uiBuildingButtonSelected.iBuildingSeed;
			buildingInPlaceMode.iBuildingVariationSeedForUndoNext = uiBuildingButtonSelected.iBuildingSeedNext;
			buildingInPlaceMode.SelectVariation();
			buildingInPlaceMode.Init();
			UnityEngine.Random.state = state;
			if (buildingInPlaceMode.iOverrideBuildingRotationSteps > 0)
			{
				fRotationStepSize = 360f / (float)buildingInPlaceMode.iOverrideBuildingRotationSteps;
				fRotationStepSizeOnWalls = 360f / (float)buildingInPlaceMode.iOverrideBuildingRotationSteps;
			}
			else
			{
				fRotationStepSize = 360f / fRotationSteps;
				fRotationStepSizeOnWalls = 360f / fRotationStepsOnWalls;
			}
			float y = Mathf.Round(UnityEngine.Random.value * 360f / fRotationStepSize) * fRotationStepSize;
			goBuildingPreview.transform.rotation = Quaternion.Euler(0f, y, 0f);
			CosmeticHandler componentInChildren = goBuildingPreview.GetComponentInChildren<CosmeticHandler>();
			ClearPreviewRender();
			if (componentInChildren == null || !componentInChildren.HasCosmeticActivated())
			{
				BufferAllMeshRenderers();
			}
			else
			{
				arMeshRenderersPreview = new MeshRenderer[0];
				liSharedMaterialsOriginalPreview.Clear();
				componentInChildren.OnComesticApplied = (Action)Delegate.Combine(componentInChildren.OnComesticApplied, new Action(BufferAllMeshRenderers));
			}
			bBuildingPlacable = true;
			bBuildingPlacableColor = true;
			iPlacementCooldown = 2;
			bBlockPlacementTillMouseReleased = true;
			eventOnBuildingButtonClick.Invoke();
		}
	}

	public bool IsPlusButtonSelected()
	{
		return bPlusButtonSelected;
	}

	public bool IsUndoButtonSelected()
	{
		return bUndoButtonSelected;
	}

	public bool IsDeleteBuildingButtonSelected()
	{
		return bDeleteButtonSelected;
	}

	public void DeselectUndoAndPlusButtons()
	{
		bUndoButtonSelected = false;
		bPlusButtonSelected = false;
	}

	private void SelectVariation(int newVariation)
	{
		Quaternion rotation = goBuildingPreview.transform.rotation;
		UnityEngine.Object.Destroy(goBuildingPreview);
		goBuildingPreview = UnityEngine.Object.Instantiate(uiBuildingButtonSelected.goResourceBuilding);
		StructureID component = goBuildingPreview.GetComponent<StructureID>();
		buildingInPlaceMode = goBuildingPreview.GetComponent<Building>();
		component.enabled = false;
		buildingInPlaceMode.iID = component.iID;
		buildingInPlaceMode.uiBuildingButtonForUndo = uiBuildingButtonSelected;
		buildingInPlaceMode.iBuildingVariationSeedForUndo = uiBuildingButtonSelected.iBuildingSeed;
		buildingInPlaceMode.iBuildingVariationSeedForUndoNext = uiBuildingButtonSelected.iBuildingSeedNext;
		buildingInPlaceMode.SelectVariation(newVariation);
		buildingInPlaceMode.Init();
		goBuildingPreview.transform.rotation = rotation;
		CosmeticHandler componentInChildren = goBuildingPreview.GetComponentInChildren<CosmeticHandler>();
		ClearPreviewRender();
		if (componentInChildren == null || !componentInChildren.HasCosmeticActivated())
		{
			BufferAllMeshRenderers();
			return;
		}
		arMeshRenderersPreview = new MeshRenderer[0];
		liSharedMaterialsOriginalPreview.Clear();
		componentInChildren.OnComesticApplied = (Action)Delegate.Combine(componentInChildren.OnComesticApplied, new Action(BufferAllMeshRenderers));
	}

	private void BufferAllMeshRenderers()
	{
		arMeshRenderersPreview = goBuildingPreview.GetComponentsInChildren<MeshRenderer>();
		liSharedMaterialsOriginalPreview.Clear();
		MeshRenderer[] array = arMeshRenderersPreview;
		for (int i = 0; i < array.Length; i++)
		{
			Material[] sharedMaterials = array[i].sharedMaterials;
			foreach (Material item in sharedMaterials)
			{
				liSharedMaterialsOriginalPreview.Add(item);
			}
		}
	}

	private void Awake()
	{
		singleton = this;
		SetDefaultSandboxLimit();
	}

	private void SetDefaultSandboxLimit()
	{
		m_SandboxBuildingLimit = m_DefaultSandboxBuildingLimit;
	}

	public void OnStartMatch()
	{
		if (m_ButtonPrompts != null)
		{
			for (int i = 0; i < m_ButtonPrompts.Length; i++)
			{
				m_ButtonPrompts[i].SetActive(value: true);
			}
		}
		m_FirstBuildingPlaced = false;
		iRotatedBuildingSteps = 0;
	}

	[CommandLine("sandbox_limit_set", "Set sandbox mode building limit to X", null, true)]
	public static void ConsoleDebugSandboxLimit(int buildingLimit)
	{
		if (singleton.SetDebugSandboxLimit(buildingLimit))
		{
			Console.Log("Set building limit: " + buildingLimit);
		}
		else
		{
			Console.Log("Building limit back to default: " + singleton.m_SandboxBuildingLimit);
		}
	}

	private bool SetDebugSandboxLimit(int buildingLimit)
	{
		if (!m_DebugLimitActivated)
		{
			m_SandboxBuildingLimit = buildingLimit;
			if (m_BuildingLimitDebugDisplay != null)
			{
				m_BuildingLimitDebugDisplay.gameObject.SetActive(value: true);
				m_BuildingLimitDebugDisplay.text = "Debug Building Limit: " + buildingLimit;
			}
			m_DebugLimitActivated = true;
			return true;
		}
		SetDefaultSandboxLimit();
		if (m_BuildingLimitDebugDisplay != null)
		{
			m_BuildingLimitDebugDisplay.gameObject.SetActive(value: false);
		}
		m_DebugLimitActivated = false;
		return false;
	}

	private void Start()
	{
		localGameManager = LocalGameManager.singleton;
		inputManager = InputManager.Singleton;
		cameraController = CameraController.singleton;
		audioSource = GetComponent<AudioSource>();
		for (int i = 0; i < transButtonParentNormalMode.childCount; i++)
		{
			UiBuildingButton component = transButtonParentNormalMode.GetChild(i).gameObject.GetComponent<UiBuildingButton>();
			if ((bool)component)
			{
				liBuildingButtonsExisting.Add(component);
			}
		}
	}

	public void OnDisable()
	{
		m_SelectButtonNextFrame = false;
		GoSelectedButton = null;
		bBuildingPreviewIsShown = false;
	}

	private void HandleBuildingBarSelectionViaButtonInputs()
	{
		InputManager.InputData inputDataCurrent = inputManager.InputDataCurrent;
		bool flag = false;
		if (inputDataCurrent.bsBuildingPlace == InputManager.ButtonState.Down && localGameManager.GameMode == LocalGameManager.EGameMode.Default)
		{
			if (bUndoButtonSelected)
			{
				SaveLoadManager.singleton.Undo();
				flag = true;
			}
			if (bPlusButtonSelected)
			{
				AudioManager.singleton.PlayButtonClick();
				LocalGameManager.singleton.OpenBuildingChoice();
				flag = true;
			}
		}
		int num = ((inputDataCurrent.bsSelectNextBuilding == InputManager.ButtonState.Down) ? 1 : 0);
		if (!flag && GoSelectedButton == null && inputDataCurrent.imLastUsedInputMethod == InputManager.InputMode.Controller && !bPlusButtonSelected && !bUndoButtonSelected && !bDeleteButtonSelected)
		{
			num += ((inputDataCurrent.bsBuildingPlace == InputManager.ButtonState.Down) ? 1 : 0);
		}
		num += ((inputDataCurrent.bsSelectPreviousBuilding == InputManager.ButtonState.Down) ? (-1) : 0);
		if (inputDataCurrent.imLastUsedInputMethod == InputManager.InputMode.Controller && inputDataCurrent.bUndo && localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			if (!bDeleteButtonSelected)
			{
				bDeleteButtonSelected = true;
				GoSelectedButton = null;
				UISandboxButtonController.singleton.SetIndex(2);
				eventOnBuildingButtonClick.Invoke();
			}
			else
			{
				bDeleteButtonSelected = false;
			}
		}
		transButtonParent = transButtonParentNormalMode;
		if (localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox && (bool)UISandboxButtonController.singleton)
		{
			transButtonParent = UISandboxButtonController.singleton.TransReturnCurrentButtonParent();
		}
		SelectCorrectButton(num);
		if (UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.InGamePlaying)
		{
			return;
		}
		for (int i = 0; i < inputDataCurrent.bQuickSelect.Length; i++)
		{
			if (!inputDataCurrent.bQuickSelect[i])
			{
				continue;
			}
			int num2 = 0;
			for (int j = 0; j < transButtonParent.childCount; j++)
			{
				UiBuildingButton component = transButtonParent.GetChild(j).GetComponent<UiBuildingButton>();
				if ((bool)component)
				{
					if (component.gameObject.activeInHierarchy)
					{
						num2++;
					}
					if (num2 > i)
					{
						if (GoSelectedButton == component)
						{
							GoSelectedButton = null;
						}
						else
						{
							GoSelectedButton = component;
						}
						break;
					}
				}
				UIPlusBuildingsButton component2 = transButtonParent.GetChild(j).GetComponent<UIPlusBuildingsButton>();
				if ((bool)component2)
				{
					if (component2.gameObject.activeInHierarchy)
					{
						num2++;
					}
					if (num2 > i)
					{
						component2.OnPointerDown();
						break;
					}
				}
			}
		}
	}

	public void SelectCorrectButton(int iInputSelectNeighborButton)
	{
		if (iInputSelectNeighborButton == 0)
		{
			return;
		}
		bool flag;
		bool flag2;
		if (localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			flag = false;
			flag2 = false;
		}
		else
		{
			flag = localGameManager.liIPlusBuildingButtonsIncludingBuildingCounts.Count > 0;
			flag2 = SaveLoadManager.singleton.bIsUndoAvailable();
		}
		int num = 0;
		int num2 = 0;
		bool flag3 = false;
		for (int i = 0; i < transButtonParent.childCount; i++)
		{
			UiBuildingButton component = transButtonParent.GetChild(i).GetComponent<UiBuildingButton>();
			if ((bool)component && component.gameObject.activeInHierarchy)
			{
				if (GoSelectedButton == component)
				{
					flag3 = true;
				}
				if (!flag3)
				{
					num++;
				}
				num2++;
			}
		}
		if (localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			bPlusButtonSelected = false;
			bUndoButtonSelected = false;
			if (!flag3)
			{
				if (iInputSelectNeighborButton > 0)
				{
					if (bDeleteButtonSelected)
					{
						bDeleteButtonSelected = false;
						UISandboxButtonController.singleton.IncreaseIndex();
					}
					else
					{
						num = 0;
						bDeleteButtonSelected = false;
					}
				}
				else if (UISandboxButtonController.singleton.IIndex == 2 && !bDeleteButtonSelected)
				{
					bDeleteButtonSelected = true;
					num = -1;
					eventOnBuildingButtonClick.Invoke();
				}
				else
				{
					bDeleteButtonSelected = false;
					num = num2 - 1;
				}
			}
			else
			{
				num += iInputSelectNeighborButton;
				if (num < 0)
				{
					bDeleteButtonSelected = false;
					UISandboxButtonController.singleton.DecreaseIndex();
					num = -1;
				}
				else if (num >= num2)
				{
					num = -1;
					if (UISandboxButtonController.singleton.IIndex == 2 && !bDeleteButtonSelected)
					{
						bDeleteButtonSelected = true;
						eventOnBuildingButtonClick.Invoke();
					}
					else
					{
						bDeleteButtonSelected = false;
						UISandboxButtonController.singleton.IncreaseIndex();
					}
				}
			}
		}
		else if (num2 > 0)
		{
			bDeleteButtonSelected = false;
			if (!flag3)
			{
				if (iInputSelectNeighborButton > 0)
				{
					if (flag && !bPlusButtonSelected && !bUndoButtonSelected && InputManager.Singleton.imLastUsedInputMethod != 0)
					{
						bPlusButtonSelected = true;
						bUndoButtonSelected = false;
						num = -1;
						eventOnBuildingButtonClick.Invoke();
					}
					else
					{
						num = (bUndoButtonSelected ? (-1) : 0);
						bPlusButtonSelected = false;
						bUndoButtonSelected = false;
					}
				}
				else if (flag2 && !bUndoButtonSelected && !bPlusButtonSelected)
				{
					bPlusButtonSelected = false;
					bUndoButtonSelected = true;
					num = -1;
					eventOnBuildingButtonClick.Invoke();
				}
				else
				{
					num = ((!bPlusButtonSelected) ? (num2 - 1) : (-1));
					bPlusButtonSelected = false;
					bUndoButtonSelected = false;
				}
			}
			else
			{
				num += iInputSelectNeighborButton;
				if (num < 0)
				{
					if (flag && !bPlusButtonSelected && !bUndoButtonSelected)
					{
						bPlusButtonSelected = true;
						bUndoButtonSelected = false;
						num = -1;
						eventOnBuildingButtonClick.Invoke();
					}
					else
					{
						bPlusButtonSelected = false;
						bUndoButtonSelected = false;
						num = -1;
					}
				}
				if (num >= num2)
				{
					if (flag2 && !bUndoButtonSelected && !bPlusButtonSelected)
					{
						bPlusButtonSelected = false;
						bUndoButtonSelected = true;
						num = -1;
						eventOnBuildingButtonClick.Invoke();
					}
					else
					{
						bPlusButtonSelected = false;
						bUndoButtonSelected = false;
						num = -1;
					}
				}
			}
		}
		else if (bPlusButtonSelected)
		{
			if (iInputSelectNeighborButton > 0 && flag2)
			{
				bPlusButtonSelected = false;
				bUndoButtonSelected = true;
				num = -1;
				eventOnBuildingButtonClick.Invoke();
			}
			else
			{
				bPlusButtonSelected = false;
				bUndoButtonSelected = false;
				num = -1;
			}
		}
		else if (bUndoButtonSelected)
		{
			if (iInputSelectNeighborButton < 0 && flag)
			{
				bPlusButtonSelected = true;
				bUndoButtonSelected = false;
				num = -1;
				eventOnBuildingButtonClick.Invoke();
			}
			else
			{
				bPlusButtonSelected = false;
				bUndoButtonSelected = false;
				num = -1;
			}
		}
		else if (iInputSelectNeighborButton < 0)
		{
			if (flag2)
			{
				bPlusButtonSelected = false;
				bUndoButtonSelected = true;
				num = -1;
				eventOnBuildingButtonClick.Invoke();
			}
			else if (flag)
			{
				bPlusButtonSelected = true;
				bUndoButtonSelected = false;
				num = -1;
				eventOnBuildingButtonClick.Invoke();
			}
		}
		else if (flag)
		{
			bPlusButtonSelected = true;
			bUndoButtonSelected = false;
			num = -1;
			eventOnBuildingButtonClick.Invoke();
		}
		else if (flag2)
		{
			bPlusButtonSelected = false;
			bUndoButtonSelected = true;
			num = -1;
			eventOnBuildingButtonClick.Invoke();
		}
		if (num == -1)
		{
			GoSelectedButton = null;
			return;
		}
		int num3 = 0;
		for (int j = 0; j < transButtonParent.childCount; j++)
		{
			UiBuildingButton component2 = transButtonParent.GetChild(j).GetComponent<UiBuildingButton>();
			if ((bool)component2 && component2.gameObject.activeInHierarchy)
			{
				if (num3 == num)
				{
					GoSelectedButton = component2;
					break;
				}
				num3++;
			}
		}
	}

	private void Update()
	{
		InputManager.InputData inputDataCurrent = inputManager.InputDataCurrent;
		if (inputManager.RewiredPlayer.GetButtonTimedPressDown("UILeaderboard", 5f))
		{
			SetDebugSandboxLimit(30);
		}
		bool flag = UpdateButtons();
		if (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePlaying)
		{
			if (m_SelectButtonNextFrame)
			{
				if (flag)
				{
					SelectCorrectButton(1);
				}
				bJustPlacedBuilding = true;
				m_SelectButtonNextFrame = false;
			}
			else
			{
				HandleBuildingBarSelectionViaButtonInputs();
			}
		}
		bool flag2 = false;
		if (inputDataCurrent.bsBuildingDeselect == InputManager.ButtonState.Down)
		{
			fMouseDraggedDistanceRight = 0f;
			v2MouseDownPositionRight = inputDataCurrent.v2PointerScreenPosNormalizedFromHeight;
			if (inputDataCurrent.imLastUsedInputMethod == InputManager.InputMode.Controller)
			{
				flag2 = true;
				fMouseDraggedDistanceRight = 1000000f;
			}
		}
		if (inputDataCurrent.bsBuildingDeselect == InputManager.ButtonState.Pressed)
		{
			fMouseDraggedDistanceRight += (inputDataCurrent.v2PointerScreenPosNormalizedFromHeight - v2MouseDownPositionRight).magnitude;
			v2MouseDownPositionRight = inputDataCurrent.v2PointerScreenPosNormalizedFromHeight;
		}
		if (inputDataCurrent.bsBuildingDeselect == InputManager.ButtonState.Up || flag2)
		{
			if (fMouseDraggedDistanceRight < fMouseMoveTreshholdRight || flag2)
			{
				GoSelectedButton = null;
				bDeleteButtonSelected = false;
				bPlusButtonSelected = false;
				bUndoButtonSelected = false;
			}
			if ((bool)goBuildingPreview)
			{
				Building component = goBuildingPreview.GetComponent<Building>();
				if ((bool)component)
				{
					component.RemoveOutlineFeedback();
				}
			}
			fMouseDraggedDistanceRight = 0f;
			variationChanged = false;
		}
		bool flag3 = false;
		if (inputDataCurrent.bsBuildingPlace == InputManager.ButtonState.Down)
		{
			fMouseDraggedDistanceLeft = 0f;
			v2MouseDownPositionLeft = inputDataCurrent.v2PointerScreenPosNormalizedFromHeight;
			if (inputDataCurrent.imLastUsedInputMethod == InputManager.InputMode.Controller)
			{
				flag3 = true;
			}
		}
		if (inputDataCurrent.bsBuildingPlace == InputManager.ButtonState.Pressed)
		{
			fMouseDraggedDistanceLeft += (inputDataCurrent.v2PointerScreenPosNormalizedFromHeight - v2MouseDownPositionLeft).magnitude;
			v2MouseDownPositionLeft = inputDataCurrent.v2PointerScreenPosNormalizedFromHeight;
		}
		if (UiCanvasManager.Singleton.UIState == UiCanvasManager.EUIState.InGamePlaying && (inputDataCurrent.bsBuildingPlace == InputManager.ButtonState.Up || flag3) && !UIBuildingBarBackground.singleton.BMouseOver && !UILockDemolitionOnMouseOver.BIsMouseOver())
		{
			if (fMouseDraggedDistanceLeft < fMouseMoveTreshholdLeft && !bMouseOverButtonClicked && (bool)goBuildingPreview && iPlacementCooldown <= 0 && !bBlockPlacementTillMouseReleased)
			{
				Building component2 = goBuildingPreview.GetComponent<Building>();
				if ((component2.BCheckBuildSpot(lmPlacementRay) || localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox) && component2.isActiveAndEnabled)
				{
					bool flag4 = true;
					if (localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox && m_SandboxBuildingLimit > 0 && Building.LiveBuildingCount > m_SandboxBuildingLimit)
					{
						flag4 = false;
						bDeleteButtonSelected = true;
						GoSelectedButton = null;
						UISandboxButtonController.singleton.SetIndex(2);
						eventOnBuildingButtonClick.Invoke();
						UiCanvasManager.Singleton.ToSandboxLimitPrompt();
					}
					if (GoSelectedButton == null || GoSelectedButton.bPrewarm)
					{
						flag4 = false;
					}
					if (flag4)
					{
						bJustPlacedBuilding = true;
						v2LastPlacedBuildingMousePos = inputManager.InputDataCurrent.v2PointerScreenPosNormalizedFromHeight;
						UiBuildingButton uiBuildingButton = PlaceBuilding(component2);
						UiBuildingButton goSelectedButton = GoSelectedButton;
						Quaternion rotation = goBuildingPreview.transform.rotation;
						int newVariation = 0;
						if (variationChanged && goBuildingPreview.TryGetComponent<Building>(out var component3) && component3.iNumVariations > 1)
						{
							newVariation = component3.iVariation;
						}
						goBuildingPreview = null;
						GoSelectedButton = null;
						if (localGameManager.IBuildingsInInventory(uiBuildingButton.goResourceBuilding) > 0)
						{
							GoSelectedButton = goSelectedButton;
							goBuildingPreview.transform.rotation = rotation;
							if (variationChanged)
							{
								SelectVariation(newVariation);
							}
						}
						else
						{
							m_SelectButtonNextFrame = PlatformPlayerManagerSystem.Instance.LastActiveController.type == ControllerType.Joystick;
						}
					}
				}
				else
				{
					eventOnBuildingCantPlace.Invoke();
				}
			}
			fMouseDraggedDistanceLeft = 0f;
		}
		iPlacementCooldown--;
		if (inputDataCurrent.bsBuildingPlace == InputManager.ButtonState.NotPressed)
		{
			bBlockPlacementTillMouseReleased = false;
		}
		bBuildingPlacable = false;
		bBuildingPreviewIsShown = false;
		UpdateBuildingPreview(inputDataCurrent);
		if (bJustPlacedBuilding)
		{
			if (bBuildingPlacable || !GoBuildingPreview || v2LastPlacedBuildingMousePos != inputManager.InputDataCurrent.v2PointerScreenPosNormalizedFromHeight)
			{
				bJustPlacedBuilding = false;
			}
			if ((bool)GoBuildingPreview && (GoBuildingPreview.transform.position - v3LastPlacedBuildingPosition).magnitude > 0.25f)
			{
				bJustPlacedBuilding = false;
			}
		}
		if (bJustPlacedBuilding)
		{
			bBuildingPreviewIsShown = false;
			GoBuildingPreview.SetActive(value: false);
		}
		if (inputChangeModelEnabled && inputDataCurrent.bChangeModel && localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox && (bool)goBuildingPreview && goBuildingPreview.TryGetComponent<Building>(out var component4) && component4.iNumVariations > 1)
		{
			SelectVariation((component4.iVariation + 1) % component4.iNumVariations);
			variationChanged = true;
		}
	}

	private bool UpdateButtons()
	{
		bool flag = false;
		foreach (UiBuildingButton item in liBuildingButtonsExisting)
		{
			flag |= item.UpdateButton();
		}
		return flag;
	}

	public List<GameObject> LiGoGetBuildingsOfExistingButtons()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (UiBuildingButton item in liBuildingButtonsExisting)
		{
			list.Add(item.goResourceBuilding);
		}
		return list;
	}

	private UiBuildingButton PlaceBuilding(Building _buildingPreview)
	{
		if (!m_FirstBuildingPlaced)
		{
			if (m_ButtonPrompts != null)
			{
				for (int i = 0; i < m_ButtonPrompts.Length; i++)
				{
					m_ButtonPrompts[i].SetActive(value: false);
				}
			}
			m_FirstBuildingPlaced = true;
		}
		eventOnBuildingPlace.Invoke();
		localGameManager.SubstractBuildingFromInventory(uiBuildingButtonSelected.goResourceBuilding, 1);
		_buildingPreview.goOritinalPrefab = uiBuildingButtonSelected.goResourceBuilding;
		v3LastPlacedBuildingPosition = _buildingPreview.transform.position;
		_buildingPreview.Place();
		_buildingPreview.transform.SetParent(transBuildingParent);
		PlatformPlayerManagerSystem.Instance?.AddPlayerStat("GS_PLACED_BUILDINGS", 1);
		uiBuildingButtonSelected.OnPlace();
		if (m_DebugBuildingCountDisplay != null)
		{
			m_DebugBuildingCountDisplay.text = "Building Count: " + Building.LiveBuildingCount;
		}
		return uiBuildingButtonSelected;
	}

	public void ButtonClicked(UiBuildingButton _goButton)
	{
		GoSelectedButton = _goButton;
		bMouseOverButtonClicked = true;
	}

	public void ButtonExit(UiBuildingButton _goButton)
	{
		if (bMouseOverButtonClicked && Input.GetMouseButton(0))
		{
			GoSelectedButton = null;
		}
		bMouseOverButtonClicked = false;
	}

	public void UndoButtonClicked()
	{
		bMouseOverButtonClicked = true;
	}

	public void UndoButtonExit()
	{
		bMouseOverButtonClicked = false;
	}

	public void UpdateBuildingPreview(InputManager.InputData _inputData)
	{
		if (!goBuildingPreview || !buildingInPlaceMode)
		{
			return;
		}
		Transform transform = goBuildingPreview.transform;
		float num = _inputData.fRotateBuilding * fRotationStepSize;
		float y = transform.rotation.eulerAngles.y + num;
		if (num != 0f && _inputData.imLastUsedInputMethod == InputManager.InputMode.Controller)
		{
			audioSource.pitch = UnityEngine.Random.value * 0.2f + 0.9f;
			if (num > 0f)
			{
				audioSource.PlayOneShot(auRotateBuildingLeft, fRotateBuildingVolume);
			}
			else
			{
				audioSource.PlayOneShot(auRotateBuildingRight, fRotateBuildingVolume);
			}
		}
		iRotatedBuildingSteps += (int)Mathf.Abs(_inputData.fRotateBuilding);
		transform.rotation = Quaternion.Euler(0f, y, 0f);
		if (Physics.Raycast(_inputData.rayPointerToWorld, out var hitInfo, 1000f, lmPlacementRay) || localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			UpdateBuildingPreviewPosition(hitInfo, transform, buildingInPlaceMode);
		}
		else
		{
			bBuildingPlacable = false;
		}
		if (localGameManager.GameMode != LocalGameManager.EGameMode.Sandbox)
		{
			iScorePreview = buildingInPlaceMode.UpdateIncomeFeedback(ref liScoreContributorsPreview);
		}
		UpdateBuildingPreviewColor();
		buildingInPlaceMode.DisableCollidingDecoObjects();
	}

	private void UpdateBuildingPreviewPosition(RaycastHit _hit, Transform _transBuildingPreview, Building _buildingPreview)
	{
		bBuildingPreviewIsShown = true;
		goBuildingPreview.SetActive(value: true);
		goBuildingPreview.transform.position = _hit.point;
		if (_buildingPreview.bPlaceOnWall)
		{
			Vector3 normal = _hit.normal;
			normal = new Vector3(normal.x, 0f, normal.z);
			if (normal.magnitude > 0.1f)
			{
				Quaternion rotation = Quaternion.Euler(0f, Mathf.Round(Quaternion.LookRotation(normal, Vector3.up).eulerAngles.y / fRotationStepSizeOnWalls) * fRotationStepSizeOnWalls, 0f);
				_transBuildingPreview.rotation = rotation;
			}
		}
		if (bBuildingPlacable = _buildingPreview.BCheckBuildSpot(lmPlacementRay))
		{
			v3LastValidPreviewPosition = _transBuildingPreview.position;
		}
		else if (!_buildingPreview.bPlaceOnWall)
		{
			BuildingPreviewSlide(_transBuildingPreview, _buildingPreview);
		}
	}

	private void BuildingPreviewSlide(Transform _transBuildingPreview, Building _buildingPreview)
	{
		if ((v3LastValidPreviewPosition - _transBuildingPreview.position).magnitude > fSnapDistance)
		{
			v3LastValidPreviewPosition = _transBuildingPreview.position;
			return;
		}
		Vector3 position = _transBuildingPreview.position;
		_transBuildingPreview.position = v3LastValidPreviewPosition;
		if (!_buildingPreview.BCheckBuildSpot(lmPlacementRay))
		{
			_transBuildingPreview.position = position;
			v3LastValidPreviewPosition = position;
			return;
		}
		Vector3 normalized = (position - _transBuildingPreview.position).normalized;
		Vector3 position2 = v3LastValidPreviewPosition;
		foreach (float liFSnapStepIteration in liFSnapStepIterations)
		{
			for (int i = 0; i < liFSnapStepDirections.Count; i++)
			{
				float y = liFSnapStepDirections[i];
				float num = liFSnapStepSize[i];
				Vector3 vector = Quaternion.Euler(0f, y, 0f) * normalized * liFSnapStepIteration * num;
				vector = new Vector3(vector.x, 0f, vector.z);
				_transBuildingPreview.position += vector;
				if (_buildingPreview.BCheckBuildSpot(lmPlacementRay))
				{
					v3LastValidPreviewPosition = _transBuildingPreview.position;
				}
				else
				{
					_transBuildingPreview.position = v3LastValidPreviewPosition;
				}
			}
			if ((v3LastValidPreviewPosition - position).magnitude > (position2 - position).magnitude)
			{
				_transBuildingPreview.position = position2;
			}
			else
			{
				position2 = _transBuildingPreview.position;
			}
		}
		bBuildingPlacable = _buildingPreview.BCheckBuildSpot(lmPlacementRay);
		if (bBuildingPlacable)
		{
			v3LastValidPreviewPosition = _transBuildingPreview.position;
		}
	}

	private void UpdateBuildingPreviewColor()
	{
		if (localGameManager.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			return;
		}
		if (bBuildingPreviewIsShown)
		{
			if (bBuildingPlacable)
			{
				FeedbackManager.Singleton.ChangeColors(FeedbackManager.ColorStates.BuildingPlaceable);
			}
			else
			{
				FeedbackManager.Singleton.ChangeColors(FeedbackManager.ColorStates.BuildingNoPlaceable);
			}
		}
		if (bBuildingPlacable == bBuildingPlacableColor || !bBuildingPreviewIsShown)
		{
			return;
		}
		if (bBuildingPlacable)
		{
			if (arMeshRenderersPreview.Length == 0)
			{
				return;
			}
			int num = 0;
			MeshRenderer[] array = arMeshRenderersPreview;
			foreach (MeshRenderer meshRenderer in array)
			{
				Material[] array2 = new Material[meshRenderer.materials.Length];
				for (int j = 0; j < meshRenderer.materials.Length; j++)
				{
					array2[j] = liSharedMaterialsOriginalPreview[num];
					if (meshRenderer.materials[j] != array2[j])
					{
						liSharedMaterialsModifiedPreview.Remove(meshRenderer.materials[j]);
						UnityEngine.Object.Destroy(meshRenderer.materials[j]);
					}
					num++;
				}
				meshRenderer.sharedMaterials = array2;
			}
		}
		else
		{
			if (arMeshRenderersPreview.Length == 0)
			{
				return;
			}
			int num2 = 0;
			MeshRenderer[] array = arMeshRenderersPreview;
			foreach (MeshRenderer meshRenderer2 in array)
			{
				List<int> list = new List<int>();
				for (int k = 0; k < meshRenderer2.sharedMaterials.Length; k++)
				{
					if (IsShaderColCantBuild(meshRenderer2.sharedMaterials[k].shader))
					{
						meshRenderer2.materials[k].color = Color.Lerp(liSharedMaterialsOriginalPreview[num2].color, colCantBuild, fCantBuildColBlend);
					}
					else if (matCantBuild != null)
					{
						list.Add(k);
					}
					num2++;
				}
				if (list.Count > 0)
				{
					Material[] sharedMaterials = meshRenderer2.sharedMaterials;
					for (int l = 0; l < list.Count; l++)
					{
						sharedMaterials[l] = new Material(matCantBuild);
					}
					meshRenderer2.sharedMaterials = sharedMaterials;
				}
				Material[] sharedMaterials2 = meshRenderer2.sharedMaterials;
				for (int m = 0; m < sharedMaterials2.Length; m++)
				{
					liSharedMaterialsModifiedPreview.Add(sharedMaterials2[m]);
				}
			}
		}
		bBuildingPlacableColor = bBuildingPlacable;
	}

	private void ClearPreviewRender()
	{
		for (int i = 0; i < liSharedMaterialsModifiedPreview.Count; i++)
		{
			UnityEngine.Object.Destroy(liSharedMaterialsModifiedPreview[i]);
		}
		liSharedMaterialsModifiedPreview.Clear();
	}

	private bool IsShaderColCantBuild(Shader shader)
	{
		if (shadersColCantBuild == null || shadersColCantBuild.Length == 0 || shader == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < shadersColCantBuild.Length; i++)
		{
			if (shadersColCantBuild[i].name.Equals(shader.name))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public void DeselectAll()
	{
		GoSelectedButton = null;
		bDeleteButtonSelected = false;
		bPlusButtonSelected = false;
		bUndoButtonSelected = false;
	}
}
