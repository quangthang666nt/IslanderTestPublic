using System.Collections;
using System.Collections.Generic;
using SCS.Gameplay;
using UnityEngine;
using UnityEngine.Events;

public class FeedbackManager : MonoBehaviour
{
	public enum ColorStates
	{
		None = 0,
		BuildingPlaceable = 1,
		BuildingNoPlaceable = 2,
		DNCDay = 3,
		DNCNight = 4,
		DNCOff = 5
	}

	private static FeedbackManager singleton;

	public static UnityEvent eventOnBuildingButtonSpawn;

	[SerializeField]
	private CustomEventHandler buildingPlacedEvent;

	[Header("Building Buttons")]
	[SerializeField]
	private RectTransform transButtonsParent;

	[SerializeField]
	private Vector3 v3ButtonSpawnOffset = Vector3.zero;

	[SerializeField]
	private AnimationCurve acButtonMovemenCurve = new AnimationCurve();

	[SerializeField]
	private AnimationCurve acButtonSizeCurve = new AnimationCurve();

	[SerializeField]
	private float fButtonAnimationTime = 2f;

	[SerializeField]
	private float fButtonSpawnIntervall = 0.5f;

	private bool buttonSpawnBusy;

	private int iButtonsInTransition;

	public GameObject goBuildingPackBackground;

	[Header("Building Preview")]
	[SerializeField]
	private UIContributorScorePreview scoreContributorPrefab;

	[SerializeField]
	private Transform transPreviewParent;

	[SerializeField]
	private UIContributorScorePreview tOverallScore;

	[SerializeField]
	private Color colOverallPositive = Color.white;

	[SerializeField]
	private Color colOverallNegative = Color.white;

	[SerializeField]
	private Color colContributorPositive = Color.white;

	[SerializeField]
	private Color colContributorNegative = Color.white;

	[SerializeField]
	private Color colOutlineOverallPositive = Color.white;

	[SerializeField]
	private Color colOutlineOverallNegative = Color.white;

	[SerializeField]
	private Color colOutlineContributorPositive = Color.white;

	[SerializeField]
	private Color colOutlineContributorNegative = Color.white;

	public float fScorePreviewOutlineWidth = 0.6f;

	[SerializeField]
	private Vector3 v3ContributorOffset = Vector3.zero;

	[SerializeField]
	private Vector3 v3OverallScoreOffset = Vector3.zero;

	[SerializeField]
	private List<GameObject> liGoRangeSphere;

	[SerializeField]
	private bool bDeactivateRangeSphereWhenCantPlace = true;

	[SerializeField]
	private bool bDeactivateMainNumberWhenCantPlace = true;

	[SerializeField]
	private bool bDeactivateContributorNumbersWhenCantPlace = true;

	[Header("Building Feedback")]
	[SerializeField]
	private float fBuildingPlaceWiggleTime;

	[SerializeField]
	private AnimationCurve acBuildingPlaceYScale;

	[SerializeField]
	private AnimationCurve acBuildingPlaceXZScale;

	private List<UIContributorScorePreview> liContributorUIs = new List<UIContributorScorePreview>();

	private GameObject goOverallScoreCanvas;

	private List<ScoreContributorData> liContributors = new List<ScoreContributorData>();

	private int iOverallScore;

	[Header("FX")]
	[SerializeField]
	private GameObject goLevelUpFX;

	[SerializeField]
	private Transform transLevelUpFXTarget;

	[Header("FX Placement Building")]
	[SerializeField]
	private GameObject buildingPlacedFX;

	[SerializeField]
	private GameObject eventBuildingPlacedFX;

	[SerializeField]
	private CosmeticID buildingPlaceID;

	[Space(10f)]
	[SerializeField]
	private GameObject buildingPlacedFXBig;

	[SerializeField]
	private GameObject eventBuildingPlacedFXBig;

	[SerializeField]
	private CosmeticID buildingPlaceBigID;

	[Header("FX Demolish")]
	[SerializeField]
	private GameObject DemolishFX;

	[SerializeField]
	private GameObject eventDemolishFX;

	[SerializeField]
	private CosmeticID DemolishID;

	[Space(10f)]
	[SerializeField]
	private GameObject DemolishFXBig;

	[SerializeField]
	private GameObject eventDemolishFXBig;

	[SerializeField]
	private CosmeticID DemolishBigID;

	[Header("Joysticks Colors (PS4 only)")]
	[SerializeField]
	private Color buildingPlaceableColor = Color.green;

	[SerializeField]
	private Color buildingNotPlaceableColor = Color.red;

	[SerializeField]
	private Color dncDayColor = new Color(84f / 85f, 0.64705884f, 1f / 85f);

	[SerializeField]
	private Color dncNightColor = new Color(26f / 85f, 1f / 85f, 84f / 85f);

	[SerializeField]
	private Color dncOffColor = Color.blue;

	private ColorStates lastColorStatesApplied;

	private Color noneColor = Color.white;

	private UiBuildingButtonManager UiBBM;

	private Camera camMain;

	private float buttonActivationPoint = 0.85f;

	[HideInInspector]
	public List<GameObject> liGoResorcesInTransition = new List<GameObject>();

	public static FeedbackManager Singleton => singleton;

	public int IButtonsInTransition => iButtonsInTransition;

	public GameObject BuildingPlaceFX
	{
		get
		{
			if (!CosmeticsManager.HasPlayerTheme())
			{
				return buildingPlacedFX;
			}
			return eventBuildingPlacedFX;
		}
	}

	public GameObject GetPrefabDemolishFX(Building.ESize size)
	{
		GameObject gameObject = null;
		if (CosmeticsManager.HasPlayerTheme())
		{
			return (size == Building.ESize.Small) ? eventDemolishFX : eventDemolishFXBig;
		}
		return (size == Building.ESize.Small) ? DemolishFX : DemolishFXBig;
	}

	public GameObject GetPrefabBuildingPlaceFX(Building.ESize size)
	{
		GameObject gameObject = null;
		if (CosmeticsManager.HasPlayerTheme())
		{
			return (size == Building.ESize.Small) ? eventBuildingPlacedFX : eventBuildingPlacedFXBig;
		}
		return (size == Building.ESize.Small) ? buildingPlacedFX : buildingPlacedFXBig;
	}

	private void Awake()
	{
		if (singleton != null)
		{
			Object.Destroy(this);
		}
		else
		{
			singleton = this;
		}
		eventOnBuildingButtonSpawn = new UnityEvent();
		LocalGameManager.singleton.eventAddBuilding.AddListener(OnBuildingAdd);
	}

	private void Start()
	{
		camMain = UICameraSpaceHelper.Cam;
		UiBBM = UiBuildingButtonManager.singleton;
		UiBBM.eventOnBuildingPlace.AddListener(FeedbackOnBuildingPlace);
		LocalGameManager.singleton.eventOnNewBoosterPack.AddListener(FeedbackOnLevelUp);
		LoadCosmeticsVFXAsync();
	}

	public void LoadCosmeticsVFXAsync()
	{
		StartCoroutine(CosmeticAsyncLoad.LoadModel(buildingPlaceID, delegate(GameObject result)
		{
			eventBuildingPlacedFX = result;
		}, null));
		StartCoroutine(CosmeticAsyncLoad.LoadModel(buildingPlaceBigID, delegate(GameObject result)
		{
			eventBuildingPlacedFXBig = result;
		}, null));
		StartCoroutine(CosmeticAsyncLoad.LoadModel(DemolishID, delegate(GameObject result)
		{
			eventDemolishFX = result;
		}, null));
		StartCoroutine(CosmeticAsyncLoad.LoadModel(DemolishBigID, delegate(GameObject result)
		{
			eventDemolishFXBig = result;
		}, null));
	}

	private void Update()
	{
		UpdateBuildingPreview();
		CheckDNCColors();
	}

	private void UpdateBuildingPreview()
	{
		bool flag = LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox;
		if (UiBBM.BBuildingPreviewIsShown)
		{
			Building component = UiBBM.GoBuildingPreview.GetComponent<Building>();
			iOverallScore = UiBBM.IScorePreview;
			liContributors = UiBBM.LiScoreContributorsPreview;
			List<Vector3> liFRangeOffsetPos = component.LiFRangeOffsetPos;
			for (int i = 0; i < liGoRangeSphere.Count; i++)
			{
				GameObject gameObject = liGoRangeSphere[i];
				float fRange = component.FRange;
				if (flag)
				{
					gameObject.SetActive(value: false);
				}
				else if (i >= liFRangeOffsetPos.Count)
				{
					gameObject.SetActive(value: false);
				}
				else if (UiBBM.BBuildingPlacable || !bDeactivateRangeSphereWhenCantPlace)
				{
					gameObject.SetActive(value: true);
					gameObject.transform.position = liFRangeOffsetPos[i];
					gameObject.transform.localScale = Vector3.one * fRange;
				}
				else
				{
					gameObject.SetActive(value: false);
				}
			}
			int num = liContributors.Count - liContributorUIs.Count;
			if (num > 0)
			{
				for (int j = 0; j < num; j++)
				{
					liContributorUIs.Add(Object.Instantiate(scoreContributorPrefab, transPreviewParent));
				}
			}
			for (int k = 0; k < liContributorUIs.Count; k++)
			{
				UIContributorScorePreview uIContributorScorePreview = liContributorUIs[k];
				if (flag)
				{
					uIContributorScorePreview.gameObject.SetActive(value: false);
				}
				else if (!UiBBM.BBuildingPlacable && bDeactivateContributorNumbersWhenCantPlace)
				{
					uIContributorScorePreview.gameObject.SetActive(value: false);
				}
				else if (k < liContributors.Count)
				{
					uIContributorScorePreview.gameObject.SetActive(value: true);
					uIContributorScorePreview.transform.position = UICameraSpaceHelper.ScreenPointToCanvasPoint(camMain.WorldToScreenPoint(liContributors[k].trans.position) + v3ContributorOffset);
					if (liContributors[k].iScoreContribution >= 0)
					{
						uIContributorScorePreview.Text = liContributors[k].iScoreContribution.ToString();
						uIContributorScorePreview.Color = colContributorPositive;
						uIContributorScorePreview.OutlineColor = colOutlineContributorPositive;
					}
					else
					{
						uIContributorScorePreview.Color = colContributorNegative;
						uIContributorScorePreview.OutlineColor = colOutlineContributorNegative;
						uIContributorScorePreview.Text = liContributors[k].iScoreContribution.ToString();
					}
				}
				else if (k >= liContributors.Count && uIContributorScorePreview.gameObject.activeSelf)
				{
					uIContributorScorePreview.gameObject.SetActive(value: false);
				}
			}
			if (flag)
			{
				tOverallScore.gameObject.SetActive(value: false);
			}
			else if (UiBBM.BBuildingPlacable || !bDeactivateMainNumberWhenCantPlace)
			{
				tOverallScore.gameObject.SetActive(value: true);
				tOverallScore.transform.position = UICameraSpaceHelper.ScreenPointToCanvasPoint(camMain.WorldToScreenPoint(UiBBM.GoBuildingPreview.transform.position) + v3OverallScoreOffset);
				if (iOverallScore >= 0)
				{
					tOverallScore.Text = "+" + iOverallScore;
					tOverallScore.Color = colOverallPositive;
					tOverallScore.OutlineColor = colOutlineOverallPositive;
				}
				else
				{
					tOverallScore.Text = iOverallScore.ToString();
					tOverallScore.Color = colOverallNegative;
					tOverallScore.OutlineColor = colOutlineOverallNegative;
				}
			}
			else
			{
				tOverallScore.gameObject.SetActive(value: false);
			}
			return;
		}
		foreach (GameObject item in liGoRangeSphere)
		{
			item.SetActive(value: false);
		}
		foreach (UIContributorScorePreview liContributorUI in liContributorUIs)
		{
			liContributorUI.gameObject.SetActive(value: false);
		}
		tOverallScore.gameObject.SetActive(value: false);
	}

	private void FeedbackOnBuildingPlace()
	{
		StartCoroutine(WiggleBuilding(UiBBM.GoBuildingPreview.transform));
		if ((bool)buildingPlacedEvent)
		{
			buildingPlacedEvent.Dispatch();
		}
	}

	private void FeedbackOnLevelUp()
	{
		Object.Instantiate(goLevelUpFX, transLevelUpFXTarget);
	}

	private void OnBuildingAdd(GameObject _goNewBuilding)
	{
		liGoResorcesInTransition.Add(_goNewBuilding);
		iButtonsInTransition++;
		StartCoroutine(SpawnButton(_goNewBuilding));
	}

	private IEnumerator SpawnButton(GameObject _goNewBuilding)
	{
		LocalGameManager.singleton.EnsureBuildingButtonIsPresent(_goNewBuilding);
		UiBuildingButton buTarget = LocalGameManager.singleton.GetButton(_goNewBuilding).GetComponent<UiBuildingButton>();
		if (LocalGameManager.singleton.IBuildingsInInventory(_goNewBuilding) < 1)
		{
			buTarget.bPrewarm = true;
		}
		while (buttonSpawnBusy)
		{
			yield return null;
		}
		StartCoroutine(ButtonCooldown());
		yield return null;
		eventOnBuildingButtonSpawn.Invoke();
		GameObject goNewButton = Object.Instantiate(_goNewBuilding.GetComponent<Building>().goButtonImage, transButtonsParent);
		Transform transTarget = buTarget.transform;
		_ = UICameraSpaceHelper.CanvasPointToScreenPoint(transTarget.position) + v3ButtonSpawnOffset;
		Vector3 v3OriginSize = goNewButton.transform.localScale;
		float fTimer = 0f;
		bool bButtonActive = false;
		while (fTimer <= fButtonAnimationTime)
		{
			fTimer += Time.deltaTime;
			float num = Mathf.InverseLerp(0f, fButtonAnimationTime, fTimer);
			Vector3 a = UICameraSpaceHelper.TransCnvs.InverseTransformPoint(transTarget.position) + v3ButtonSpawnOffset;
			goNewButton.transform.localScale = v3OriginSize * acButtonSizeCurve.Evaluate(num);
			goNewButton.transform.position = UICameraSpaceHelper.TransCnvs.TransformPoint(Vector3.LerpUnclamped(a, UICameraSpaceHelper.TransCnvs.InverseTransformPoint(transTarget.position), acButtonMovemenCurve.Evaluate(num)));
			if (num >= buttonActivationPoint && !bButtonActive)
			{
				bButtonActive = true;
				LocalGameManager.singleton.AddBuildingToInventory(_goNewBuilding, 1);
				iButtonsInTransition--;
				liGoResorcesInTransition.Remove(_goNewBuilding);
			}
			yield return null;
		}
		goNewButton.transform.localScale = v3OriginSize;
		Object.Destroy(goNewButton);
	}

	private IEnumerator ButtonCooldown()
	{
		buttonSpawnBusy = true;
		yield return new WaitForSeconds(fButtonSpawnIntervall);
		buttonSpawnBusy = false;
	}

	private IEnumerator WiggleBuilding(Transform _transBuilding)
	{
		Vector3 localScale;
		Vector3 v3OriginScale = (localScale = _transBuilding.localScale);
		bool bRunning = true;
		float fTimer = 0f;
		while (bRunning)
		{
			fTimer += Time.deltaTime;
			if (fTimer >= fBuildingPlaceWiggleTime)
			{
				bRunning = false;
			}
			float time = Mathf.InverseLerp(0f, fBuildingPlaceWiggleTime, fTimer);
			localScale.y = v3OriginScale.y * acBuildingPlaceYScale.Evaluate(time);
			localScale.z = (localScale.x = v3OriginScale.x * acBuildingPlaceXZScale.Evaluate(time));
			if (!_transBuilding)
			{
				break;
			}
			_transBuilding.localScale = localScale;
			yield return null;
		}
		if ((bool)_transBuilding)
		{
			_transBuilding.localScale = v3OriginScale;
		}
	}

	public void ChangeColors(ColorStates state)
	{
		UpdateJoysticksColor(state switch
		{
			ColorStates.BuildingPlaceable => buildingPlaceableColor, 
			ColorStates.BuildingNoPlaceable => buildingNotPlaceableColor, 
			ColorStates.DNCDay => dncDayColor, 
			ColorStates.DNCNight => dncNightColor, 
			ColorStates.DNCOff => dncOffColor, 
			_ => noneColor, 
		});
		lastColorStatesApplied = state;
	}

	private void UpdateJoysticksColor(Color color)
	{
	}

	private void CheckDNCColors()
	{
		if (UiBBM.BBuildingPreviewIsShown || !DayNightCycle.Instance)
		{
			return;
		}
		if (DayNightCycle.Instance.IsUpdatingCycle)
		{
			if (lastColorStatesApplied != ColorStates.DNCDay && (DayNightCycle.Instance.DayState == DNCycleParameters.EDayState.Day || DayNightCycle.Instance.DayState == DNCycleParameters.EDayState.Sunrise))
			{
				ChangeColors(ColorStates.DNCDay);
			}
			else if (lastColorStatesApplied != ColorStates.DNCNight && (DayNightCycle.Instance.DayState == DNCycleParameters.EDayState.Night || DayNightCycle.Instance.DayState == DNCycleParameters.EDayState.Sunset))
			{
				ChangeColors(ColorStates.DNCNight);
			}
		}
		else if (lastColorStatesApplied != ColorStates.DNCOff)
		{
			ChangeColors(ColorStates.DNCOff);
		}
	}
}
