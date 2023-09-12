using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
	public struct SaveStructureData
	{
		public GameObject prefab;

		public GameObject structure;
	}

	public static TerrainGenerator singleton;

	[Help("This script is responsible for generating a new random island following the rules layed out by its child objects.", MessageType.Info)]
	[SerializeField]
	[Range(1f, 14f)]
	[Tooltip("The time in MS the generator is allowed to spent on island generation each frame.")]
	private float fTimeBudgetPerFrameMs = 2f;

	[SerializeField]
	[Tooltip("The parent / the transform the new island is generated into.")]
	private Transform transTerrainParent;

	[SerializeField]
	[Range(0.01f, 25f)]
	[Tooltip("The max size of the island.")]
	private float fRadius = 2f;

	[SerializeField]
	[Tooltip("Only positive numbers will actually set the seed. -1 will use a random seed.")]
	public int iIslandSeed = -1;

	[SerializeField]
	[Tooltip("Possible color schemes for this island.")]
	public ColorGenerator.OverrideColorSetup[] overrideColorSetups;

	private Stopwatch stopWatch = new Stopwatch();

	private bool bTerrainGenerationDone = true;

	private Random.State randomStateTerrain;

	private Random.State randomStateBeforeColorGen;

	[Header("Game Design")]
	[Tooltip("Player always starts with water plateaus.")]
	public bool bWaterHeavy;

	[Tooltip("Player always starts with wall plateaus.")]
	public bool bVertical;

	[Tooltip("If true, there will be no fields and no sandpits, but mills brickyards etc. will still exist.")]
	public bool bSnow;

	[Tooltip("You start by placing three totems.")]
	public bool bAncient;

	[Space]
	[Tooltip("If false, grass-buildings like the mill will be completely deactivated.")]
	public bool bAllowGrassUnlocks = true;

	[Tooltip("If false, sand-buildings like sandpit AND brickyard will be completely deactivated.")]
	public bool bAllowSandUnlocks = true;

	[Tooltip("If false, stone-buildings like mason will be completely deactivated.")]
	public bool bAllowRockUnlocks = true;

	[Tooltip("If false, the lumberjack will be deactivated.")]
	public bool bAllowWoodUnlocks = true;

	[Tooltip("If false, the jewlery will be deactivated.")]
	public bool bAllowGoldUnlocks = true;

	[Header("Difficulty (using offset cecommended)")]
	[SerializeField]
	[Range(0.5f, 1.5f)]
	[Tooltip("Multiplies the required score for unlocking the next island.")]
	private float fScoreGoalMultiplyer = 1f;

	[SerializeField]
	[Range(-500f, 500f)]
	[Tooltip("Adds to the required score for unlocking the next island.")]
	private int iScoreGoalOffset;

	private List<SaveStructureData> listStructuresToSave = new List<SaveStructureData>();

	public Transform TransTerrainParrent => transTerrainParent;

	public float FRadius
	{
		get
		{
			return fRadius;
		}
		set
		{
			fRadius = value;
		}
	}

	private Stopwatch StopWatch => stopWatch;

	public bool BTerrainGenerationDone
	{
		get
		{
			return bTerrainGenerationDone;
		}
		set
		{
			bTerrainGenerationDone = value;
		}
	}

	public float FScoreGoalMultiplyer => fScoreGoalMultiplyer;

	public int IScoreGoalOffset => iScoreGoalOffset;

	private void Awake()
	{
		singleton = this;
	}

	public IEnumerator TerrainGeneration(int _iSeed)
	{
		bTerrainGenerationDone = false;
		yield return null;
		stopWatch.Reset();
		stopWatch.Start();
		if (_iSeed >= 0)
		{
			Random.InitState(_iSeed);
		}
		randomStateTerrain = Random.state;
		if (BShouldYield())
		{
			BeforeYield();
			yield return null;
			AfterYield();
		}
		CreateNewIslandParent();
		BeforeYield();
		yield return null;
		AfterYield();
		SaveLoadManager.SaveGameCurrent.randomStateBeforeColorGen = Random.state;
		GenerateColors();
		if (BShouldYield())
		{
			BeforeYield();
			yield return null;
			AfterYield();
		}
		if (_iSeed >= 0)
		{
			Random.InitState(_iSeed);
		}
		listStructuresToSave.Clear();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			TerrainOperation toChild = gameObject.GetComponent<TerrainOperation>();
			if ((bool)toChild)
			{
				IEnumerator ieExecute = toChild.Execute(this);
				toChild.BExecuteDone = false;
				while (!toChild.BExecuteDone)
				{
					ieExecute.MoveNext();
					yield return null;
				}
			}
			if (BShouldYield())
			{
				BeforeYield();
				yield return null;
				AfterYield();
			}
		}
		for (int j = 0; j < listStructuresToSave.Count; j++)
		{
			SaveLoadManager.SaveIslandObjectToCurrentSaveFile(listStructuresToSave[j].prefab, listStructuresToSave[j].structure.transform);
		}
		bTerrainGenerationDone = true;
		yield return null;
	}

	public bool BShouldYield()
	{
		if ((float)stopWatch.ElapsedMilliseconds >= fTimeBudgetPerFrameMs)
		{
			return true;
		}
		return false;
	}

	public void BeforeYield()
	{
		randomStateTerrain = Random.state;
	}

	public void AfterYield(bool _bResetStopwatch = true)
	{
		if (_bResetStopwatch)
		{
			stopWatch.Reset();
			stopWatch.Start();
		}
		Random.state = randomStateTerrain;
	}

	public void SaveObjectToCurrentFileWhenFinish(GameObject prefab, GameObject structure)
	{
		SaveStructureData item = default(SaveStructureData);
		item.prefab = prefab;
		item.structure = structure;
		listStructuresToSave.Add(item);
	}

	public Vector3 v2GetRandomPositionInRange()
	{
		Vector3 result;
		do
		{
			result = new Vector3(fRadius * (Random.value - 0.5f) * 2f, 0f, fRadius * (Random.value - 0.5f) * 2f);
		}
		while (!(result.magnitude < fRadius));
		return result;
	}

	public bool BIsMeshInBounds(Mesh _mesh, Transform _transReference, out Vector3 _v3Direction, out float _fDistance)
	{
		if (!_mesh)
		{
			UnityEngine.Debug.LogError("BIsMeshInBounds function called with missing mesh");
			_v3Direction = Vector3.zero;
			_fDistance = 0f;
			return true;
		}
		Vector3 vector = new Vector3(transTerrainParent.transform.position.x, 0f, transTerrainParent.position.z);
		_v3Direction = Vector3.zero;
		float num = 0f;
		Vector3[] vertices = _mesh.vertices;
		foreach (Vector3 position in vertices)
		{
			Vector3 vector2 = _transReference.TransformPoint(position);
			Vector3 vector3 = new Vector3(vector2.x, 0f, vector2.z);
			float magnitude = (vector3 - vector).magnitude;
			if (magnitude > num)
			{
				num = magnitude;
				_v3Direction = (vector - vector3).normalized;
			}
		}
		_fDistance = num - fRadius;
		if (num <= fRadius)
		{
			return true;
		}
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		if ((bool)transTerrainParent)
		{
			Gizmos.DrawWireSphere(transTerrainParent.position, fRadius);
		}
	}

	public void GenerateColors()
	{
		if (!ColorGenerator.singleton)
		{
			return;
		}
		if (overrideColorSetups.Length != 0)
		{
			List<ColorSetup> list = new List<ColorSetup>();
			ColorGenerator.OverrideColorSetup[] array = overrideColorSetups;
			foreach (ColorGenerator.OverrideColorSetup overrideColorSetup in array)
			{
				float fProbability = overrideColorSetup.fProbability;
				fProbability = ((!(Random.value < fProbability % 1f)) ? Mathf.Floor(fProbability) : Mathf.Ceil(fProbability));
				for (int j = 0; (float)j < fProbability; j++)
				{
					if (overrideColorSetup != null && overrideColorSetup.colorSetup != null)
					{
						list.Add(overrideColorSetup.colorSetup);
					}
				}
			}
			if (list.Count > 0)
			{
				ColorSetup colorSetup = list[Random.Range(0, list.Count)];
				ColorGenerator.singleton.GenerateColorScheme(colorSetup);
			}
			else
			{
				ColorGenerator.singleton.GenerateColorScheme();
			}
		}
		else
		{
			ColorGenerator.singleton.GenerateColorScheme();
		}
	}

	public void CreateNewIslandParent()
	{
		if ((bool)transTerrainParent)
		{
			Object.Destroy(transTerrainParent.gameObject);
		}
		GameObject gameObject = new GameObject("Island Parent");
		transTerrainParent = gameObject.transform;
		transTerrainParent.SetParent(base.transform);
		transTerrainParent.localPosition = Vector3.zero;
		transTerrainParent.localScale = Vector3.one;
		transTerrainParent.localRotation = Quaternion.identity;
	}
}
