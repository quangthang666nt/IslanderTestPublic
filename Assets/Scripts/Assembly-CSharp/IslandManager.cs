using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
	[Help("This script is responsible for loading, creating, picking and managing islands.", MessageType.Info)]
	[SerializeField]
	[Tooltip("Origin position of the islands.")]
	private Vector3 v3OriginIslandSpawnPos = Vector3.zero;

	public static IslandManager singleton;

	[SerializeField]
	[Tooltip("Transform of the main camera.")]
	private Transform transCamera;

	[SerializeField]
	[Tooltip("This list is used to see how many points you need to reach on each island. Whenever you travel to a new island it'll get harder. It also saves the possible island generator prefabs that can appear at that point in your run.")]
	private List<IslandPlusScoreGoal> liIslandsAndGoals = new List<IslandPlusScoreGoal>();

	[Tooltip("The island you're currently on. (Increases by 1 for every island you complete without losing.)")]
	public int iCurrentIslandIndex;

	private GameObject goCurrentIsland;

	private Coroutine crCurrentTransition;

	[HideInInspector]
	public Random.State randomStateCreation;

	[Tooltip("IDs of all the generator prefabs you already encountered during this run. (This way we can avoid encountaring the same prefab twice.)")]
	public List<int> liIIslandsInThisRun;

	[Tooltip("The ID of the current island generator prefab.")]
	public int iCurrentIslandId;

	[Header("IDs: Do NOT shuffle this (keep removed empty):")]
	[Tooltip("This list gives every island generator prefab a uniue ID. Never change this. Only add stuff at the end.")]
	public List<GameObject> liGoAllIslandPrefabs = new List<GameObject>();

	private bool bCreateNewIslandDone;

	private Random.State randomStateContinue;

	public List<string> instanceNames = new List<string>();

	public List<IslandPlusScoreGoal> LiIslandsAndGoals => liIslandsAndGoals;

	public GameObject GoCurrentIsland => goCurrentIsland;

	public bool BCreateNewIslandDone => bCreateNewIslandDone;

	private void Awake()
	{
		singleton = this;
	}

	public void BlockCurrentIslandFromAppearingAgain()
	{
		liIIslandsInThisRun.Add(iCurrentIslandId);
	}

	private GameObject GoGetRandomTerrainPrefab()
	{
		if (iCurrentIslandIndex < 0 || iCurrentIslandIndex >= liIslandsAndGoals.Count)
		{
			iCurrentIslandIndex = liIslandsAndGoals.Count - 1;
		}
		PrefabListWithProbabilities prefabListWithProb = liIslandsAndGoals[iCurrentIslandIndex].prefabListWithProb;
		GameObject gameObject = null;
		int num = -100;
		for (int num2 = 1000; num2 >= 0; num2--)
		{
			gameObject = prefabListWithProb.GoReturnRandom();
			num = liGoAllIslandPrefabs.IndexOf(gameObject);
			if (num >= 0 && !liIIslandsInThisRun.Contains(num))
			{
				iCurrentIslandId = num;
				break;
			}
			if (num2 == 0)
			{
				if (num >= 0)
				{
					iCurrentIslandId = num;
				}
				else
				{
					num = 0;
				}
				break;
			}
		}
		iCurrentIslandIndex++;
		return gameObject;
	}

	public IEnumerator CreateNewIsland(LocalGameManager.EGameMode gameMode, SaveGame.ELoadMode loadMode)
	{
		bCreateNewIslandDone = false;
		randomStateCreation = Random.state;
		List<StructureID> liStructIDRegister = SaveLoadManager.liStructIDRegister;
		for (int num = liStructIDRegister.Count - 1; num >= 0; num--)
		{
			Object.Destroy(liStructIDRegister[num].gameObject);
		}
		randomStateContinue = Random.state;
		yield return null;
		Random.state = randomStateContinue;
		if ((bool)goCurrentIsland)
		{
			goCurrentIsland.transform.position = new Vector3(-1000000f, -1000000f, -1000000f);
			Object.Destroy(goCurrentIsland);
		}
		randomStateContinue = Random.state;
		yield return null;
		Random.state = randomStateContinue;
		if (gameMode == LocalGameManager.EGameMode.Default || (loadMode == SaveGame.ELoadMode.Normal && !SandboxGenerator.SandboxConfig.playerData))
		{
			GameObject gameObject = GoGetRandomTerrainPrefab();
			goCurrentIsland = Object.Instantiate(gameObject, v3OriginIslandSpawnPos + new Vector3(200f, 0f, 200f), gameObject.transform.rotation);
			SaveLoadManager.SaveGameCurrent.iCurrendIslandGenID = liGoAllIslandPrefabs.IndexOf(gameObject);
		}
		else
		{
			goCurrentIsland = SandboxGenerator.singleton.GenerateIslandFromExistingData();
			goCurrentIsland.transform.position = v3OriginIslandSpawnPos + new Vector3(200f, 0f, 200f);
			SaveLoadManager.SaveGameCurrent.iCurrendIslandGenID = 0;
		}
		TerrainGenerator terrainGen = goCurrentIsland.GetComponentInChildren<TerrainGenerator>();
		terrainGen.iIslandSeed = Random.Range(0, 1000000);
		List<int> liIPlaceableIds = SaveLoadManager.SaveGameCurrent.liIPlaceableIds;
		bool bLoadedIsland = false;
		if (liIPlaceableIds.Count > 0)
		{
			terrainGen.CreateNewIslandParent();
			Random.state = SaveLoadManager.SaveGameCurrent.randomStateBeforeColorGen;
			terrainGen.GenerateColors();
			List<SeriTransform> liSeriTransOfPlaceables = SaveLoadManager.SaveGameCurrent.liSeriTransOfPlaceables;
			SaveLoadManager saveLoadManager = SaveLoadManager.singleton;
			for (int i = 0; i < liIPlaceableIds.Count; i++)
			{
				GameObject gameObject2 = saveLoadManager.GoGetIslandPlaceable(liIPlaceableIds[i]);
				instanceNames.Add(gameObject2.name);
				SeriTransform seriTransform = liSeriTransOfPlaceables[i];
				GameObject obj = Object.Instantiate(gameObject2, seriTransform.V3GetPosition(), seriTransform.QGetRotation());
				obj.transform.localScale = seriTransform.V3GetScale();
				obj.transform.SetParent(terrainGen.TransTerrainParrent);
				if ((float)i / 20f == Mathf.Round((float)i / 20f))
				{
					randomStateContinue = Random.state;
					yield return null;
					Random.state = randomStateContinue;
				}
			}
			liStructIDRegister = SaveLoadManager.liStructIDRegister;
			for (int num2 = liStructIDRegister.Count - 1; num2 >= 0; num2--)
			{
				instanceNames.Add("Destroyerd listructid  " + liStructIDRegister[num2].gameObject.name);
				Object.Destroy(liStructIDRegister[num2].gameObject);
			}
			bLoadedIsland = true;
		}
		randomStateContinue = Random.state;
		yield return null;
		Random.state = randomStateContinue;
		if (!bLoadedIsland)
		{
			terrainGen.StartCoroutine(terrainGen.TerrainGeneration(terrainGen.iIslandSeed));
			while (!terrainGen.BTerrainGenerationDone)
			{
				yield return null;
			}
		}
		goCurrentIsland.transform.position = v3OriginIslandSpawnPos;
		StatsManager.statsMatch.SnowIsland = terrainGen.bSnow;
		bCreateNewIslandDone = true;
		yield return null;
	}
}
