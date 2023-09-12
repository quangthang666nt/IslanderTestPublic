using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

[DisallowMultipleComponent]
public class Building : MonoBehaviour
{
	public enum ESize
	{
		Small = 0,
		Big = 1
	}

	private TerrainGenerator terrainGenerator;

	public int iAmount = 1;

	public int iAmountWater;

	public float fAmountPerUnlocks;

	public float fAmountPerUnlocksWater;

	public float fAmountPerUnlocksVertical;

	public bool bPlaceOnWall;

	public int iOverrideBuildingRotationSteps;

	public bool bReAddIntoDeck = true;

	private bool bInDemolitionProcess;

	[SerializeField]
	private bool bShowBaseValueInTooltip = true;

	public LocalizedString strBuildingName;

	public LocalizedString strLocalizedBaseValueTemplate = "Tooltips/Buildings/Base Score";

	public LocalizedString strLocalizedTooltipTemplatePositive = "Tooltips/Buildings/Tooltip Template Positive 01";

	public LocalizedString strLocalizedTooltipTemplateNegative = "Tooltips/Buildings/Tooltip Template Negative 01";

	private bool bShowScoreNumbersInTooltips = true;

	public GameObject goButtonImage;

	private ProximityManager proximityManager;

	private List<GameObject> liGoBuildingsInteractable = new List<GameObject>();

	private Dictionary<int, int> dicIDToScore = new Dictionary<int, int>();

	private int iScore;

	[SerializeField]
	private float fRange = 10f;

	[Range(0f, 1f)]
	[SerializeField]
	private float fRangeInSnowMultiplyer = 0.6f;

	[SerializeField]
	private List<Vector3> liV3RangeOffset = new List<Vector3>();

	private List<Vector3> liV3TransformedOffset = new List<Vector3>();

	public LayerMask lmBuildingSurface;

	public LayerMask lmGroundedBreakers;

	public LayerMask lmForbiddenOverlaps;

	public LayerMask lmDeleteOnPlacement;

	public int iBaseScore;

	public int iAddScoreSnow;

	public List<BuildingInteraction> liBuildingInteraction = new List<BuildingInteraction>();

	public GameObject goRequiredRelation;

	public float fRequiredRelationAmount;

	[HideInInspector]
	public int iVariation;

	[HideInInspector]
	public int iNumVariations;

	[HideInInspector]
	public int iID;

	[SerializeField]
	private ESize ebuildingSize;

	private List<BoxCollider> liMyBoxColliders = new List<BoxCollider>();

	private List<SphereCollider> liMySphereColliders = new List<SphereCollider>();

	public bool bAlwaysAllowPlacementOnSteepSurfaces;

	public GameObject goOritinalPrefab;

	public bool bPlaced;

	private LocalGameManager localGameManager;

	public UiBuildingButton uiBuildingButtonForUndo;

	public int iBuildingVariationSeedForUndo;

	public int iBuildingVariationSeedForUndoNext;

	private GroundedChecker[] m_arGroundedChecker;

	private List<BoxCollider> m_liBoxColliders = new List<BoxCollider>();

	private List<SphereCollider> m_liSphereColliders = new List<SphereCollider>();

	private Collider[] m_OverlapCollider = new Collider[128];

	private static HashSet<Building> s_AllBuildings = new HashSet<Building>();

	private float fGroundAngle;

	private bool bSkipGroundedCheck;

	private Vector3 v3GroundNormal;

	private RaycastHit rayHit;

	private List<GameObject> liDisabledOverlapGameObjects = new List<GameObject>();

	private List<Collider> liOverlappingCollidersToDelete = new List<Collider>();

	private List<GameObject> m_CloseGameObjectsBuffer = new List<GameObject>();

	private bool demolish;

	private bool baking;

	public string strToolTip => GenerateTooltip();

	public float FRange
	{
		get
		{
			if (!terrainGenerator)
			{
				terrainGenerator = TerrainGenerator.singleton;
			}
			if (!terrainGenerator.bSnow)
			{
				return fRange;
			}
			return fRange * fRangeInSnowMultiplyer;
		}
	}

	public List<Vector3> LiFRangeOffsetPos
	{
		get
		{
			liV3TransformedOffset.Clear();
			foreach (Vector3 item in liV3RangeOffset)
			{
				liV3TransformedOffset.Add(base.transform.position + base.transform.rotation * item);
			}
			if (liV3RangeOffset.Count == 0)
			{
				liV3RangeOffset.Add(Vector3.zero);
			}
			return liV3TransformedOffset;
		}
	}

	public ESize BuildingSize => ebuildingSize;

	public static int LiveBuildingCount => s_AllBuildings.Count;

	public bool IsDemolish => demolish;

	public bool IsBaking
	{
		get
		{
			return baking;
		}
		set
		{
			baking = value;
		}
	}

	public void SelectVariation(int _iVariation = -1)
	{
		iNumVariations = base.transform.childCount;
		if (_iVariation < 0)
		{
			iVariation = Random.Range(0, iNumVariations);
		}
		else
		{
			iVariation = _iVariation;
		}
		GameObject gameObject = base.transform.GetChild(iVariation).gameObject;
		for (int num = iNumVariations - 1; num >= 0; num--)
		{
			GameObject obj = base.transform.GetChild(num).gameObject;
			if (num != iVariation)
			{
				Object.DestroyImmediate(obj);
			}
		}
		gameObject.SetActive(value: true);
		while (gameObject.transform.childCount > 0)
		{
			gameObject.transform.GetChild(0).SetParent(base.transform);
		}
		Object.DestroyImmediate(gameObject);
		liMyBoxColliders.AddRange(GetComponentsInChildren<BoxCollider>());
		liMySphereColliders.AddRange(GetComponentsInChildren<SphereCollider>());
	}

	private void Awake()
	{
		localGameManager = LocalGameManager.singleton;
	}

	private void Start()
	{
		m_arGroundedChecker = GetComponentsInChildren<GroundedChecker>();
		m_liBoxColliders.AddRange(GetComponentsInChildren<BoxCollider>());
		m_liSphereColliders.AddRange(GetComponentsInChildren<SphereCollider>());
		terrainGenerator = TerrainGenerator.singleton;
		proximityManager = ProximityManager.singleton;
		foreach (BuildingInteraction item in liBuildingInteraction)
		{
			liGoBuildingsInteractable.Add(item.goBuilding);
			dicIDToScore.Add(item.goBuilding.GetComponent<StructureID>().iID, item.iScore);
		}
		base.gameObject.AddComponent<UIBuildingTooltip>().building = this;
	}

	public void Init()
	{
		m_liBoxColliders.AddRange(GetComponentsInChildren<BoxCollider>());
		m_liSphereColliders.AddRange(GetComponentsInChildren<SphereCollider>());
	}

	private void OnEnable()
	{
		s_AllBuildings.Add(this);
	}

	private void OnDisable()
	{
		s_AllBuildings.Remove(this);
	}

	private string GenerateTooltip()
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		List<string> list = new List<string>();
		foreach (BuildingInteraction item in liBuildingInteraction)
		{
			Building component = item.goBuilding.GetComponent<Building>();
			StructureName component2 = item.goBuilding.GetComponent<StructureName>();
			string text4 = "Missing_Name_Warning_for_" + item.goBuilding.name;
			if ((bool)component)
			{
				text4 = component.strBuildingName;
			}
			if ((bool)component2) 
			{
				text4 = component2.strStructureName;
			}
			if (list.Contains(text4))
			{
				continue;
			}
			list.Add(text4);
			if (item.iScore < 0)
			{
				text3 = ((text3.Length <= 0) ? (text3 + text4) : (text3 + ", " + text4));
				if (bShowScoreNumbersInTooltips)
				{
					text3 = text3 + " (" + item.iScore + ")";
				}
			}
			if (item.iScore > 0)
			{
				text2 = ((text2.Length <= 0) ? (text2 + text4) : (text2 + ", " + text4));
				if (bShowScoreNumbersInTooltips)
				{
					text2 = text2 + " (" + item.iScore + ")";
				}
			}
		}
		if (iBaseScore != 0 && bShowBaseValueInTooltip)
		{
			if (iBaseScore > 0)
			{
				text = string.Concat(text, strLocalizedBaseValueTemplate, " <style=POS>", iBaseScore.ToString(), ". </style>");
			}
			if (iBaseScore < 0)
			{
				text = string.Concat(text, strLocalizedBaseValueTemplate, " <style=NEG>", iBaseScore.ToString(), ". </style>");
			}
		}
		if (text2.Length > 0)
		{
			text += strLocalizedTooltipTemplatePositive;
			if (text.Contains("{buildings_positive}") && text2.Length > 0)
			{
				text = text.Replace("{buildings_positive}", text2);
			}
		}
		if (text3.Length > 0)
		{
			text += strLocalizedTooltipTemplateNegative;
			if (text.Contains("{buildings_negative}") && text3.Length > 0)
			{
				text = text.Replace("{buildings_negative}", text3);
			}
		}
		return text;
	}

	public Dictionary<string, int> GetInteractions()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add(strLocalizedBaseValueTemplate, iBaseScore);
		foreach (BuildingInteraction item in liBuildingInteraction)
		{
			Building component = item.goBuilding.GetComponent<Building>();
			StructureName component2 = item.goBuilding.GetComponent<StructureName>();
			string key = "Missing_Name_Warning_for_" + item.goBuilding.name;
			if ((bool)component)
			{
				key = component.strBuildingName;
			}
			if ((bool)component2)
			{
				key = component2.strStructureName;
			}
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, item.iScore);
			}
		}
		return dictionary;
	}

	public bool BCheckBuildSpot(LayerMask _lmPlacable)
	{
		bSkipGroundedCheck = false;
		if (bAlwaysAllowPlacementOnSteepSurfaces)
		{
			Physics.Raycast(base.transform.position + Vector3.up * 0.5f, Vector3.down, out rayHit, 1f, _lmPlacable);
			v3GroundNormal = rayHit.normal;
			_ = v3GroundNormal;
			fGroundAngle = Vector3.Dot(v3GroundNormal, Vector3.up);
			if ((double)fGroundAngle > 0.3 && (double)fGroundAngle < 0.95)
			{
				bSkipGroundedCheck = true;
			}
		}
		if (!bSkipGroundedCheck)
		{
			if (m_arGroundedChecker == null)
			{
				m_arGroundedChecker = GetComponentsInChildren<GroundedChecker>();
			}
			for (int i = 0; i < m_arGroundedChecker.Length; i++)
			{
				GroundedChecker groundedChecker = m_arGroundedChecker[i];
				if ((bool)groundedChecker && !groundedChecker.BCheckGrounded(lmBuildingSurface, lmGroundedBreakers))
				{
					return false;
				}
			}
		}
		for (int j = 0; j < m_liBoxColliders.Count; j++)
		{
			BoxCollider bc = m_liBoxColliders[j];
			Vector3 _v3OutPosition = default(Vector3);
			Vector3 _v3OutScale = default(Vector3);
			Quaternion _qOutRotation = default(Quaternion);
			TransformTools.BoxColliderToWorldSpace(bc, ref _v3OutPosition, ref _v3OutScale, ref _qOutRotation);
			int num = Physics.OverlapBoxNonAlloc(_v3OutPosition, _v3OutScale * 0.5f, m_OverlapCollider, _qOutRotation, lmForbiddenOverlaps);
			if (num <= 0)
			{
				continue;
			}
			int num2 = num;
			for (int k = 0; k < num2; k++)
			{
				if (m_OverlapCollider[k] is BoxCollider && m_liBoxColliders.Contains(m_OverlapCollider[k] as BoxCollider))
				{
					num--;
				}
				else if (m_OverlapCollider[k] is SphereCollider && m_liSphereColliders.Contains(m_OverlapCollider[k] as SphereCollider))
				{
					num--;
				}
			}
			if (num > 0)
			{
				return false;
			}
		}
		for (int l = 0; l < m_liSphereColliders.Count; l++)
		{
			SphereCollider sc = m_liSphereColliders[l];
			Vector3 _v3OutPosition2 = default(Vector3);
			float _fOutRadius = 0f;
			TransformTools.SphereColliderToWorldSpace(sc, ref _v3OutPosition2, ref _fOutRadius);
			int num3 = Physics.OverlapSphereNonAlloc(_v3OutPosition2, _fOutRadius, m_OverlapCollider, lmForbiddenOverlaps);
			if (num3 <= 0)
			{
				continue;
			}
			int num4 = num3;
			for (int m = 0; m < num4; m++)
			{
				if (m_OverlapCollider[m] is BoxCollider && m_liBoxColliders.Contains(m_OverlapCollider[m] as BoxCollider))
				{
					num3--;
				}
				else if (m_OverlapCollider[m] is SphereCollider && m_liSphereColliders.Contains(m_OverlapCollider[m] as SphereCollider))
				{
					num3--;
				}
			}
			if (num3 > 0)
			{
				return false;
			}
		}
		return true;
	}

	public void Place()
	{
		Debug.Log("[Buildings] - Place");
		SaveLoadManager singleton = SaveLoadManager.singleton;
		singleton.buildingLastBuiltForUndo = this;
		singleton.iBusyCoinsBeforePlacingLastBuildingForUndo = CoinManager.Singleton.IGetCoinTransitionBalance();
		singleton.iUnlockedBoosterPacksBeforePlacingLastBuildingForUndo = localGameManager.IUnlockedBoosterPacks;
		singleton.iAvailablePlusesBeforePlacingLastBuildingForUndo = localGameManager.liIPlusBuildingButtonsIncludingBuildingCounts.Count;
		singleton.iScoreGoalBeforePlacingLastBuildingForUndo = localGameManager.IRequiredScoreForNextPack;
		singleton.iScoreBaseBeforePlacingLastBuildingForUndo = localGameManager.IRequiredScoreForLastPack;
		singleton.DeleteLastRemovedOverlapObjectsForGood();
		singleton.liGoDisabledObjectsOnLasPlacementForUndo = new List<GameObject>(liDisabledOverlapGameObjects);
		liOverlappingCollidersToDelete.Clear();
		GetComponent<StructureID>().enabled = true;
		ChangeLayerOnPlacement();
		if (localGameManager.GameMode == LocalGameManager.EGameMode.Default)
		{
			iScore = UiBuildingButtonManager.singleton.IScorePreview;
			StatsManager.statsMatch.iTotalBuildingsBuilt++;
			StatsManager.statsMatch.iBuildingsBuilt++;
			StatsManager.statsMatch.iHighscore += iScore;
			StatsManager.statsMatch.iTotalScore += iScore;
			StatsManager.statsMatch.liIPlacedBuildings.Add(iID);
			StatsManager.statsMatch.liIScorePerBuilding.Add(iScore);
			StatsManager.statsMatch.liIMilisecondsPerBuilding.Add((int)(StatsManager.statsMatch.fTotalTimePlayed * 1000f));
			if (iScore < 0)
			{
				StatsManager.statsMatch.iTotalNegativePoints += -iScore;
			}
			StatsManager.statsMatch.CheckForAchievements();
		}
		else
		{
			StatsManager.statsMatch.liIPlacedBuildings.Add(iID);
			StatsManager.statsMatch.CheckForAchievementsSandbox();
			if (SandboxGenerator.SandboxConfig.matchBuildsPlaced < TutorialSandboxManager.singleton.buildsPlacedToLaunch)
			{
				SandboxGenerator.SandboxConfig.matchBuildsPlaced++;
				SandboxGenerator.Save();
			}
		}
		SaveLoadManager.PerformAutosave();
		Invoke("BakeColors", 0.5f);
		GameObject prefabBuildingPlaceFX = FeedbackManager.Singleton.GetPrefabBuildingPlaceFX(ebuildingSize);
		if ((bool)prefabBuildingPlaceFX)
		{
			Object.Instantiate(prefabBuildingPlaceFX, base.transform.position, Quaternion.identity);
		}
		DemolitionController.BuildingWasJustCreatedAndShouldNotBeRemoved();
		bPlaced = true;
		if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			ArchiveManager.SetCurrentStickyIfAvailable();
		}
		RemoveOutlineFeedback();
	}

	private void BakeColors()
	{
		if (!demolish)
		{
			if (ColorBaker.singleton.bCoroutineBaking)
			{
				ColorBaker.singleton.AddObjectToBakeQueue(base.gameObject);
			}
			else
			{
				ColorBaker.singleton.BakeObject(base.gameObject);
			}
		}
	}

	private void ChangeLayerOnPlacement()
	{
		BuildingLayerOnPlacementChanger[] componentsInChildren = base.gameObject.GetComponentsInChildren<BuildingLayerOnPlacementChanger>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SwitchLayer();
		}
	}

	public void PlaceQuick()
	{
		ChangeLayerOnPlacement();
		DeleteColliding();
		bPlaced = true;
	}

	private List<Collider> LiColliderFindOverlapsToDelete(ref List<Collider> _liOverlapColliders)
	{
		_liOverlapColliders.Clear();
		foreach (BoxCollider liMyBoxCollider in liMyBoxColliders)
		{
			Vector3 _v3OutPosition = default(Vector3);
			Vector3 _v3OutScale = default(Vector3);
			Quaternion _qOutRotation = default(Quaternion);
			TransformTools.BoxColliderToWorldSpace(liMyBoxCollider, ref _v3OutPosition, ref _v3OutScale, ref _qOutRotation);
			_liOverlapColliders.AddRange(Physics.OverlapBox(_v3OutPosition, _v3OutScale * 0.5f, _qOutRotation, lmDeleteOnPlacement));
		}
		foreach (SphereCollider liMySphereCollider in liMySphereColliders)
		{
			Vector3 position = liMySphereCollider.transform.position + Vector3.Scale(liMySphereCollider.center, liMySphereCollider.transform.localScale) * base.transform.localScale.x;
			float radius = liMySphereCollider.radius * liMySphereCollider.gameObject.transform.lossyScale.y;
			_liOverlapColliders.AddRange(Physics.OverlapSphere(position, radius, lmDeleteOnPlacement));
		}
		return _liOverlapColliders;
	}

	public void DisableCollidingDecoObjects()
	{
		ReEnableDisabledOverlapGameObjects();
		LiColliderFindOverlapsToDelete(ref liOverlappingCollidersToDelete);
		while (liOverlappingCollidersToDelete.Count > 0)
		{
			if ((bool)liOverlappingCollidersToDelete[0])
			{
				liDisabledOverlapGameObjects.Add(liOverlappingCollidersToDelete[0].gameObject);
				liOverlappingCollidersToDelete[0].gameObject.SetActive(value: false);
			}
			liOverlappingCollidersToDelete.RemoveAt(0);
		}
	}

	public void ReEnableDisabledOverlapGameObjects()
	{
		while (liDisabledOverlapGameObjects.Count > 0)
		{
			if ((bool)liDisabledOverlapGameObjects[0])
			{
				liDisabledOverlapGameObjects[0].SetActive(value: true);
			}
			liDisabledOverlapGameObjects.RemoveAt(0);
		}
	}

	private void DeleteColliding()
	{
		ReEnableDisabledOverlapGameObjects();
		LiColliderFindOverlapsToDelete(ref liOverlappingCollidersToDelete);
		while (liOverlappingCollidersToDelete.Count > 0)
		{
			if ((bool)liOverlappingCollidersToDelete[0])
			{
				Object.Destroy(liOverlappingCollidersToDelete[0].gameObject);
			}
			liOverlappingCollidersToDelete.RemoveAt(0);
		}
	}

	public void RemoveOutlineFeedback()
	{
		foreach (GameObject item in m_CloseGameObjectsBuffer)
		{
			StructureOutline component = item.GetComponent<StructureOutline>();
			if (component != null)
			{
				component.HideOutline();
			}
		}
		m_CloseGameObjectsBuffer.Clear();
	}

	public int UpdateIncomeFeedback()
	{
		iScore = iBaseScore;
		if (TerrainGenerator.singleton.bSnow)
		{
			iScore += iAddScoreSnow;
		}
		if (!proximityManager)
		{
			proximityManager = ProximityManager.singleton;
		}
		foreach (GameObject item in proximityManager.LiFindGameObjectsInRadius(LiFRangeOffsetPos, liGoBuildingsInteractable, FRange))
		{
			if (item != base.gameObject && item.activeInHierarchy)
			{
				int num = dicIDToScore[item.GetComponent<StructureID>().iID];
				iScore += num;
			}
		}
		return iScore;
	}

	public int UpdateIncomeFeedback(ref List<ScoreContributorData> _liScoreContributors)
	{
		iScore = iBaseScore;
		if (TerrainGenerator.singleton.bSnow)
		{
			iScore += iAddScoreSnow;
		}
		_liScoreContributors.Clear();
		if (!proximityManager)
		{
			proximityManager = ProximityManager.singleton;
		}
		List<GameObject> list = proximityManager.LiFindGameObjectsInRadius(LiFRangeOffsetPos, liGoBuildingsInteractable, FRange);
		foreach (GameObject item in m_CloseGameObjectsBuffer)
		{
			if (!list.Contains(item))
			{
				StructureOutline component = item.GetComponent<StructureOutline>();
				if (component != null)
				{
					component.HideOutline();
				}
			}
		}
		m_CloseGameObjectsBuffer.Clear();
		StatsManager.statsMatch.liIScoreAdditionPerCloseBuilding.Clear();
		foreach (GameObject item2 in list)
		{
			if (!(item2 != base.gameObject) || !item2.activeInHierarchy)
			{
				continue;
			}
			int key = item2.GetComponent<StructureID>().iID;
			int num = dicIDToScore[key];
			StatsManager.statsMatch.liIScoreAdditionPerCloseBuilding.Add(new KeyValuePair<int, int>(key, num));
			iScore += num;
			_liScoreContributors.Add(new ScoreContributorData(item2.transform, num));
			StructureOutline component2 = item2.GetComponent<StructureOutline>();
			if (component2 != null)
			{
				if (num > 0)
				{
					component2.ShowPositiveOutline();
				}
				else if (num < 0)
				{
					component2.ShowNegativeOutline();
				}
			}
			m_CloseGameObjectsBuffer.Add(item2);
		}
		return iScore;
	}

	public void Demolish(bool playSound = false, bool delayedCall = false)
	{
		if (bInDemolitionProcess && !delayedCall)
		{
			return;
		}
		bInDemolitionProcess = true;
		if (IsBaking)
		{
			StartCoroutine(DelayedDemolish(playSound));
			return;
		}
		demolish = true;
		GameObject prefabDemolishFX = FeedbackManager.Singleton.GetPrefabDemolishFX(BuildingSize);
		if (prefabDemolishFX != null)
		{
			Object.Instantiate(prefabDemolishFX, base.transform.position, Quaternion.identity);
		}
		if (playSound)
		{
			AudioManager.singleton.PlayDemolishSound();
		}
		ColorBaker.singleton.UnbakeBuilding(base.gameObject);
		if (LocalGameManager.singleton.GameMode == LocalGameManager.EGameMode.Sandbox)
		{
			ArchiveManager.SetCurrentStickyIfAvailable();
		}
		int num = GetComponent<StructureID>().iID;
		for (int num2 = StatsManager.statsMatch.liIPlacedBuildings.Count - 1; num2 >= 0; num2--)
		{
			if (StatsManager.statsMatch.liIPlacedBuildings[num2] == num)
			{
				StatsManager.statsMatch.liIPlacedBuildings.RemoveAt(num2);
				break;
			}
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator DelayedDemolish(bool playSound)
	{
		while (IsBaking)
		{
			yield return null;
		}
		Demolish(playSound, delayedCall: true);
	}

	public bool IsBeingDemolish()
	{
		return bInDemolitionProcess;
	}
}
