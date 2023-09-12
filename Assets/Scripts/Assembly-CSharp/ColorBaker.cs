using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SCS.Gameplay;
using UnityEngine;
using UnityEngine.Rendering;

public class ColorBaker : MonoBehaviour
{
	public class BakedObjectData
	{
		public int iVertIndex;

		public int iVertCount;

		public int iTriangleCount;

		public int iTriangleIndex;
	}

	public static ColorBaker singleton;

	private const ushort BUILDINGS_MAX_VERTEX = 1000;

	private const float LOADING_TARGET_FPS = 0.03f;

	[Help("This script exist for performance reasons. It bakes material colors into vertex colors and combines all meshes into one mega mesh. That drastically reduces the batch count. Mesh renderers of baked objects are disabled.", MessageType.Info)]
	[Tooltip("Enable this to bake material colors into vertex colors. Every object only needs one single material after that.")]
	public bool bEnableBaking = true;

	[Tooltip("Enable this to combine all static meshes into one giant mega mesh.")]
	public bool bEnableMeshMerging = true;

	[Tooltip("Enable this to bake mesh on a coroutine")]
	public bool bCoroutineBaking;

	[Tooltip("Every how many buildings built wait a frame during scene setup")]
	public int iSetupWaitBuilding = 10;

	[Tooltip("Number of meshes generated for the Windows and Doors at night. None if 0")]
	public int iMultipleNightLumMeshBaking = 1;

	[Header("Settings")]
	[Space]
	[Tooltip("The shader of the material after baking.")]
	public Shader shaderToBake;

	[Tooltip("The shader required to back a mesh as secondary mesh.")]
	public ShadersToBakeDatabase secondaryShadersToBake;

	[Tooltip("The material of the meshes after baking.")]
	public Material bakedMaterial;

	[Tooltip("The material of the meshes of the night lights after baking.")]
	public Material sharedMaterialNightLights;

	[Header("Events")]
	[SerializeField]
	private CustomEventHandler bakedSceneEvent;

	private List<Mesh> liMeshOriginals = new List<Mesh>();

	private List<Material[]> liArOriginalMaterials = new List<Material[]>();

	private List<Mesh> liMeshBaked = new List<Mesh>();

	private Dictionary<GameObject, BakedObjectData> dicBakedObjectData = new Dictionary<GameObject, BakedObjectData>();

	private Dictionary<MeshFilter, BakedObjectData> dicSecondaryBakedObjectData = new Dictionary<MeshFilter, BakedObjectData>();

	private Dictionary<MeshFilter, int> dicSecondaryBakedObjectIndex = new Dictionary<MeshFilter, int>();

	private Dictionary<MeshFilter, BakedObjectData> dicNightBakedObjectData = new Dictionary<MeshFilter, BakedObjectData>();

	private Dictionary<MeshFilter, int> dicNightBakedObjectIndex = new Dictionary<MeshFilter, int>();

	private Material[] arSharedMaterialBaked;

	[HideInInspector]
	public bool bBakingInProcess;

	private MeshRenderer meshRendererMegaCombined;

	private MeshFilter meshFilterMegaCombined;

	private MeshRenderer meshRendererMegaCombinedNightLights;

	private MeshFilter meshFilterMegaCombinedNightLights;

	private List<MeshRenderer> multipleMeshRendererMCNightLights = new List<MeshRenderer>();

	private List<MeshFilter> multipleMeshFilterMCNightLights = new List<MeshFilter>();

	private Building buildingMouseOver;

	[Tooltip("The layer mask used for detecting which building the mouse is hovering over. Don't ask me why this has to be in this script. Oops.")]
	public LayerMask lmMouseOverBuildingDetection;

	private Camera mainCam;

	private Queue<GameObject> bakeQueue = new Queue<GameObject>();

	private Coroutine bakeEntireSceneCoroutine;

	private List<MeshFilter> meshFilterMegaBuildingsCombinedList = new List<MeshFilter>();

	private Dictionary<MeshFilter, BakedObjectData> dicBuildingsBakedObjectData = new Dictionary<MeshFilter, BakedObjectData>();

	private Dictionary<MeshFilter, int> dicBuildingsBakedObjectIndex = new Dictionary<MeshFilter, int>();

	private List<KeyValuePair<int, MeshFilter>> meshFilterMegaSecondaryCombinedDictionary = new List<KeyValuePair<int, MeshFilter>>();

	private Dictionary<Material, int> dicSecondaryMeshID = new Dictionary<Material, int>();

	private BakedObjectData nightBakedObjects;

	private int iRandomIndexNL;

	private MeshRenderer mrCache;

	private bool bSameMesh;

	private List<Vector3> liV3Vertecies = new List<Vector3>();

	private List<int> liITriangles = new List<int>();

	private List<Vector3> liV3Normals = new List<Vector3>();

	private List<Color> liVertColors = new List<Color>();

	private Color colUnassigned = new Color(0f, 0f, 0f, 0f);

	private int iTrisOfPrevSubMeshes;

	private Color colTarget;

	private int iVert;

	private Mesh meshBaked;

	private int iBaseVertexCount;

	private int iBaseTriangleCount;

	private List<Vector3> liV3MergeVerts = new List<Vector3>();

	private List<int> liIMergeTris = new List<int>();

	private List<Color> liColorsMerge = new List<Color>();

	private List<Vector3> liV3MergeNormals = new List<Vector3>();

	private List<Vector3> liV3MergeVertsNL = new List<Vector3>();

	private List<int> liIMergeTrisNL = new List<int>();

	private List<Color> liColorsMergeNL = new List<Color>();

	private List<Vector3> liV3MergeNormalsNL = new List<Vector3>();

	private Matrix4x4 matObjToWorldNormal;

	private int iForEndHere;

	private Dictionary<Transform, Building> m_BuildingCache = new Dictionary<Transform, Building>();

	private void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		meshFilterMegaCombined = GetComponent<MeshFilter>();
		meshFilterMegaCombined.sharedMesh = new Mesh();
		meshFilterMegaCombined.sharedMesh.indexFormat = IndexFormat.UInt32;
		meshRendererMegaCombined = GetComponent<MeshRenderer>();
		meshRendererMegaCombined.shadowCastingMode = ShadowCastingMode.Off;
		arSharedMaterialBaked = new Material[1];
		arSharedMaterialBaked[0] = bakedMaterial;
		mainCam = Object.FindObjectOfType<Camera>();
		GameObject gameObject = new GameObject("NightWindowsRoots");
		gameObject.transform.parent = base.transform;
		meshFilterMegaCombinedNightLights = gameObject.AddComponent<MeshFilter>();
		meshFilterMegaCombinedNightLights.sharedMesh = new Mesh();
		meshFilterMegaCombinedNightLights.sharedMesh.indexFormat = IndexFormat.UInt32;
		meshRendererMegaCombinedNightLights = gameObject.AddComponent<MeshRenderer>();
		meshRendererMegaCombinedNightLights.sharedMaterial = sharedMaterialNightLights;
		if (iMultipleNightLumMeshBaking > 1)
		{
			GameObject gameObject2 = new GameObject("MultipleNightWindowsRoots");
			gameObject2.transform.parent = base.transform;
			gameObject.AddComponent<ActivateChildsRandom>().target = gameObject2.transform;
			for (int i = 0; i < iMultipleNightLumMeshBaking; i++)
			{
				GameObject obj = new GameObject("NightWindowsChild");
				obj.transform.parent = gameObject2.transform;
				MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
				meshFilter.sharedMesh = new Mesh();
				meshFilter.sharedMesh.indexFormat = IndexFormat.UInt32;
				MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = sharedMaterialNightLights;
				multipleMeshFilterMCNightLights.Add(meshFilter);
				multipleMeshRendererMCNightLights.Add(meshRenderer);
			}
		}
		gameObject.AddComponent<DNObjectExclusive>().SetActiveDayState(DNCycleParameters.EDayState.Night);
		if (bCoroutineBaking)
		{
			bakeQueue = new Queue<GameObject>();
			StartCoroutine(BakingRoutine());
		}
	}

	private IEnumerator BakingRoutine()
	{
		while (true)
		{
			if (bakeQueue.Count > 0)
			{
				GameObject gameObject = bakeQueue.Dequeue();
				Building building = gameObject.GetComponent<Building>();
				if (building != null && building.IsDemolish)
				{
					continue;
				}
				if (building != null)
				{
					building.IsBaking = true;
				}
				yield return BakeObjectCoroutine(gameObject);
				if (building != null)
				{
					building.IsBaking = false;
				}
			}
			yield return null;
		}
	}

	public void ClearCache()
	{
		m_BuildingCache.Clear();
		DestroyMesh(meshFilterMegaCombined);
		meshFilterMegaCombined.sharedMesh = new Mesh();
		meshFilterMegaCombined.sharedMesh.indexFormat = IndexFormat.UInt32;
		ClearCacheMegaBuildings();
		ClearCacheMegaSecondary();
		DestroyMesh(meshFilterMegaCombinedNightLights);
		meshFilterMegaCombinedNightLights.sharedMesh = new Mesh();
		meshFilterMegaCombinedNightLights.sharedMesh.indexFormat = IndexFormat.UInt32;
		for (int i = 0; i < multipleMeshFilterMCNightLights.Count; i++)
		{
			DestroyMesh(multipleMeshFilterMCNightLights[i]);
		}
		if (iMultipleNightLumMeshBaking > 1)
		{
			for (int j = 0; j < iMultipleNightLumMeshBaking; j++)
			{
				multipleMeshFilterMCNightLights[j].sharedMesh = new Mesh();
				multipleMeshFilterMCNightLights[j].sharedMesh.indexFormat = IndexFormat.UInt32;
			}
		}
		liMeshOriginals.Clear();
		liArOriginalMaterials.Clear();
		for (int k = 0; k < liMeshBaked.Count; k++)
		{
			Object.Destroy(liMeshBaked[k]);
		}
		liMeshBaked.Clear();
		liV3MergeVerts.Clear();
		liIMergeTris.Clear();
		liColorsMerge.Clear();
		liV3MergeNormals.Clear();
		liV3MergeVertsNL.Clear();
		liIMergeTrisNL.Clear();
		liColorsMergeNL.Clear();
		liV3MergeNormalsNL.Clear();
		dicBakedObjectData.Clear();
		dicBuildingsBakedObjectData.Clear();
		dicBuildingsBakedObjectIndex.Clear();
		dicNightBakedObjectData.Clear();
		dicNightBakedObjectIndex.Clear();
		if (bakeEntireSceneCoroutine != null)
		{
			StopCoroutine(bakeEntireSceneCoroutine);
			bakeEntireSceneCoroutine = null;
		}
	}

	private void DestroyMesh(MeshFilter meshFilter)
	{
		if (meshFilter != null && meshFilter.sharedMesh != null)
		{
			Object.Destroy(meshFilter.sharedMesh);
		}
	}

	private void ClearCacheMegaBuildings()
	{
		foreach (MeshFilter meshFilterMegaBuildingsCombined in meshFilterMegaBuildingsCombinedList)
		{
			DestroyMesh(meshFilterMegaBuildingsCombined);
			Object.Destroy(meshFilterMegaBuildingsCombined.gameObject);
		}
		meshFilterMegaBuildingsCombinedList.Clear();
	}

	private void ClearCacheMegaSecondary()
	{
		foreach (KeyValuePair<int, MeshFilter> item in meshFilterMegaSecondaryCombinedDictionary)
		{
			DestroyMesh(item.Value);
			Object.Destroy(item.Value.gameObject);
		}
		meshFilterMegaSecondaryCombinedDictionary.Clear();
		dicSecondaryMeshID.Clear();
	}

	public void BakeEntireScene()
	{
		if (!bEnableBaking)
		{
			bBakingInProcess = false;
			return;
		}
		if (bakeEntireSceneCoroutine != null)
		{
			StopCoroutine(bakeEntireSceneCoroutine);
			bakeEntireSceneCoroutine = null;
		}
		bakeEntireSceneCoroutine = StartCoroutine(BakeEntireSceneCoroutine());
		bBakingInProcess = true;
	}

	private IEnumerator BakeEntireSceneCoroutine()
	{
		yield return null;
		Building[] buildingsToBake = Object.FindObjectsOfType<Building>();
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		for (int j = 0; j < buildingsToBake.Length; j++)
		{
			BakeObject(buildingsToBake[j].gameObject);
			if (Time.realtimeSinceStartup - realtimeSinceStartup > 0.03f)
			{
				yield return null;
				realtimeSinceStartup = Time.realtimeSinceStartup;
			}
		}
		MeshFilter[] meshFiltersToBake = Object.FindObjectsOfType<MeshFilter>();
		realtimeSinceStartup = Time.realtimeSinceStartup;
		for (int j = 0; j < meshFiltersToBake.Length; j++)
		{
			if (!(meshFiltersToBake[j].gameObject == base.gameObject) && (bool)meshFiltersToBake[j].gameObject != meshFiltersToBake[j].transform.IsChildOf(base.transform) && !meshFiltersToBake[j].transform.IsChildOf(meshRendererMegaCombinedNightLights.transform))
			{
				BakeMeshFilter(meshFiltersToBake[j]);
				if (Time.realtimeSinceStartup - realtimeSinceStartup > 0.03f)
				{
					yield return null;
					realtimeSinceStartup = Time.realtimeSinceStartup;
				}
			}
		}
		yield return null;
		bBakingInProcess = false;
		if (bakedSceneEvent != null)
		{
			bakedSceneEvent.Dispatch();
		}
		bakeEntireSceneCoroutine = null;
	}

	public void AddObjectToBakeQueue(GameObject _go)
	{
		bakeQueue.Enqueue(_go);
	}

	public void BakeObject(GameObject _go)
	{
		if (!bEnableBaking || dicBakedObjectData.ContainsKey(_go) || _go == base.gameObject || _go == meshRendererMegaCombinedNightLights.gameObject || _go.transform.IsChildOf(meshRendererMegaCombinedNightLights.transform))
		{
			return;
		}
		BakedObjectData bakedObjectData = new BakedObjectData();
		bakedObjectData.iVertIndex = meshFilterMegaCombined.sharedMesh.vertexCount;
		bakedObjectData.iTriangleIndex = meshFilterMegaCombined.sharedMesh.triangles.Length;
		MeshFilter[] componentsInChildren = _go.GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			nightBakedObjects = new BakedObjectData();
			iRandomIndexNL = -2;
			BakeMeshFilter(componentsInChildren[i], isBuilding: true);
			if (iRandomIndexNL != -2)
			{
				dicNightBakedObjectData.Add(componentsInChildren[i], nightBakedObjects);
				dicNightBakedObjectIndex.Add(componentsInChildren[i], iRandomIndexNL);
			}
		}
		bakedObjectData.iVertCount = meshFilterMegaCombined.sharedMesh.vertexCount - bakedObjectData.iVertIndex;
		bakedObjectData.iTriangleCount = meshFilterMegaCombined.sharedMesh.triangles.Length - bakedObjectData.iTriangleIndex;
		dicBakedObjectData.Add(_go, bakedObjectData);
	}

	private IEnumerator BakeObjectCoroutine(GameObject _go)
	{
		if (!bEnableBaking || dicBakedObjectData.ContainsKey(_go) || _go == base.gameObject || _go == meshRendererMegaCombinedNightLights.gameObject || _go.transform.IsChildOf(meshRendererMegaCombinedNightLights.transform))
		{
			yield break;
		}
		BakedObjectData bakedObjectData = new BakedObjectData
		{
			iVertIndex = meshFilterMegaCombined.sharedMesh.vertexCount,
			iTriangleIndex = meshFilterMegaCombined.sharedMesh.triangles.Length
		};
		MeshFilter[] meshFiltersToBake = _go.GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i < meshFiltersToBake.Length; i++)
		{
			nightBakedObjects = new BakedObjectData();
			iRandomIndexNL = -2;
			BakeMeshFilter(meshFiltersToBake[i], isBuilding: true);
			if (iRandomIndexNL != -2)
			{
				dicNightBakedObjectData.Add(meshFiltersToBake[i], nightBakedObjects);
				dicNightBakedObjectIndex.Add(meshFiltersToBake[i], iRandomIndexNL);
			}
			yield return null;
		}
		bakedObjectData.iVertCount = meshFilterMegaCombined.sharedMesh.vertexCount - bakedObjectData.iVertIndex;
		bakedObjectData.iTriangleCount = meshFilterMegaCombined.sharedMesh.triangles.Length - bakedObjectData.iTriangleIndex;
		dicBakedObjectData.Add(_go, bakedObjectData);
	}

	private void BakeMeshFilter(MeshFilter _mf, bool isBuilding = false)
	{
		if (!_mf || _mf.sharedMesh == null || !_mf.gameObject.activeInHierarchy)
		{
			return;
		}
		mrCache = _mf.GetComponent<MeshRenderer>();
		if (!mrCache || !mrCache.enabled)
		{
			return;
		}
		if (!mrCache.sharedMaterial.shader.name.Equals(shaderToBake.name) && base.gameObject != mrCache.gameObject)
		{
			if (secondaryShadersToBake.Contains(mrCache.sharedMaterial.shader) && _mf.gameObject.layer != 12 && _mf.gameObject.layer != 11 && !_mf.GetComponent<BasicAnimation>() && !_mf.GetComponent<DontBake>())
			{
				Mesh obj = BakeAllSubMeshes(_mf.sharedMesh);
				if (!dicSecondaryMeshID.ContainsKey(mrCache.sharedMaterial))
				{
					dicSecondaryMeshID.Add(mrCache.sharedMaterial, dicSecondaryMeshID.Count);
				}
				MergeMeshesIntoMegaSecondary(obj, _mf, dicSecondaryMeshID[mrCache.sharedMaterial]);
				Object.Destroy(obj);
				_mf.sharedMesh = null;
				mrCache.enabled = false;
			}
			return;
		}
		Mesh mesh = BakeVertexColors(_mf.sharedMesh, mrCache);
		Mesh mesh2 = null;
		if (iMultipleNightLumMeshBaking > 0)
		{
			mesh2 = BakeVertexNightLights(_mf.sharedMesh, mrCache);
		}
		_mf.sharedMesh = mesh;
		mrCache.sharedMaterials = arSharedMaterialBaked;
		if (bEnableMeshMerging && _mf.gameObject.layer != 12 && _mf.gameObject.layer != 11 && !_mf.GetComponent<BasicAnimation>() && !_mf.GetComponent<DontBake>())
		{
			mrCache.enabled = false;
			if (iMultipleNightLumMeshBaking == -1)
			{
				iRandomIndexNL = -1;
				if (nightBakedObjects != null)
				{
					nightBakedObjects.iVertIndex = meshFilterMegaCombinedNightLights.sharedMesh.vertexCount;
					nightBakedObjects.iTriangleIndex = meshFilterMegaCombinedNightLights.sharedMesh.triangles.Length;
				}
				MergeMeshes(meshFilterMegaCombinedNightLights.sharedMesh, mesh2, _mf.transform, useLocals: true);
				if (nightBakedObjects != null)
				{
					nightBakedObjects.iVertCount = meshFilterMegaCombinedNightLights.sharedMesh.vertexCount - nightBakedObjects.iVertIndex;
					nightBakedObjects.iTriangleCount = meshFilterMegaCombinedNightLights.sharedMesh.triangles.Length - nightBakedObjects.iTriangleIndex;
				}
			}
			else if (iMultipleNightLumMeshBaking > -1)
			{
				iRandomIndexNL = Random.Range(0, iMultipleNightLumMeshBaking - 1);
				if (nightBakedObjects != null)
				{
					nightBakedObjects.iVertIndex = multipleMeshFilterMCNightLights[iRandomIndexNL].sharedMesh.vertexCount;
					nightBakedObjects.iTriangleIndex = multipleMeshFilterMCNightLights[iRandomIndexNL].sharedMesh.triangles.Length;
				}
				MergeMeshes(multipleMeshFilterMCNightLights[iRandomIndexNL].sharedMesh, mesh2, _mf.transform, useLocals: true);
				if (nightBakedObjects != null)
				{
					nightBakedObjects.iVertCount = multipleMeshFilterMCNightLights[iRandomIndexNL].sharedMesh.vertexCount - nightBakedObjects.iVertIndex;
					nightBakedObjects.iTriangleCount = multipleMeshFilterMCNightLights[iRandomIndexNL].sharedMesh.triangles.Length - nightBakedObjects.iTriangleIndex;
				}
			}
			if (isBuilding)
			{
				MergeMeshesIntoMegaBuildings(mesh, _mf);
			}
			else
			{
				MergeMeshes(meshFilterMegaCombined.sharedMesh, mesh, _mf.transform);
			}
		}
		if (mesh2 != null)
		{
			Object.Destroy(mesh2);
		}
	}

	private void MergeMeshesIntoMegaBuildings(Mesh meshBaked, MeshFilter _mf)
	{
		MeshFilter meshFilter2 = meshFilterMegaBuildingsCombinedList.OrderBy((MeshFilter meshFilter) => meshFilter.sharedMesh.vertexCount).FirstOrDefault((MeshFilter meshFilter) => meshFilter.sharedMesh.vertexCount + meshBaked.vertexCount < 1000);
		if (!meshFilter2)
		{
			meshFilter2 = PushNewMegaBuildings();
		}
		BakedObjectData bakedObjectData = new BakedObjectData();
		bakedObjectData.iVertIndex = meshFilter2.sharedMesh.vertexCount;
		bakedObjectData.iTriangleIndex = meshFilter2.sharedMesh.triangles.Length;
		MergeMeshes(meshFilter2.sharedMesh, meshBaked, _mf.transform, useLocals: true);
		bakedObjectData.iVertCount = meshFilter2.sharedMesh.vertexCount - bakedObjectData.iVertIndex;
		bakedObjectData.iTriangleCount = meshFilter2.sharedMesh.triangles.Length - bakedObjectData.iTriangleIndex;
		dicBuildingsBakedObjectData.Add(_mf, bakedObjectData);
		dicBuildingsBakedObjectIndex.Add(_mf, meshFilterMegaBuildingsCombinedList.IndexOf(meshFilter2));
	}

	private MeshFilter PushNewMegaBuildings()
	{
		GameObject obj = new GameObject("MegaBuildingsCombined_" + meshFilterMegaBuildingsCombinedList.Count);
		obj.transform.parent = base.transform;
		MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = new Mesh();
		meshFilter.sharedMesh.indexFormat = IndexFormat.UInt16;
		MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = meshRendererMegaCombined.sharedMaterial;
		meshRenderer.shadowCastingMode = meshRendererMegaCombined.shadowCastingMode;
		meshRenderer.receiveShadows = meshRendererMegaCombined.receiveShadows;
		meshFilterMegaBuildingsCombinedList.Add(meshFilter);
		return meshFilter;
	}

	private void MergeMeshesIntoMegaSecondary(Mesh meshBaked, MeshFilter _mf, int materialKey)
	{
		KeyValuePair<int, MeshFilter> item = meshFilterMegaSecondaryCombinedDictionary.OrderBy((KeyValuePair<int, MeshFilter> pair) => pair.Value.sharedMesh.vertexCount).FirstOrDefault((KeyValuePair<int, MeshFilter> pair) => pair.Value.sharedMesh.vertexCount + meshBaked.vertexCount < 1000 && pair.Key == materialKey);
		if (!item.Value)
		{
			item = PushNewMegaSecondary(materialKey);
		}
		BakedObjectData bakedObjectData = new BakedObjectData();
		bakedObjectData.iVertIndex = item.Value.sharedMesh.vertexCount;
		bakedObjectData.iTriangleIndex = item.Value.sharedMesh.triangles.Length;
		MergeMeshes(item.Value.sharedMesh, meshBaked, _mf.transform, useLocals: true, supportColors: false);
		bakedObjectData.iVertCount = item.Value.sharedMesh.vertexCount - bakedObjectData.iVertIndex;
		bakedObjectData.iTriangleCount = item.Value.sharedMesh.triangles.Length - bakedObjectData.iTriangleIndex;
		dicSecondaryBakedObjectData.Add(_mf, bakedObjectData);
		dicSecondaryBakedObjectIndex.Add(_mf, meshFilterMegaSecondaryCombinedDictionary.IndexOf(item));
	}

	private KeyValuePair<int, MeshFilter> PushNewMegaSecondary(int materialKey)
	{
		GameObject obj = new GameObject("MegaSecondaryCombined_" + mrCache.sharedMaterial.name + "_" + meshFilterMegaSecondaryCombinedDictionary.Count);
		obj.transform.parent = base.transform;
		MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = new Mesh();
		meshFilter.sharedMesh.indexFormat = IndexFormat.UInt16;
		MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = mrCache.sharedMaterial;
		meshRenderer.shadowCastingMode = mrCache.shadowCastingMode;
		KeyValuePair<int, MeshFilter> keyValuePair = new KeyValuePair<int, MeshFilter>(materialKey, meshFilter);
		meshFilterMegaSecondaryCombinedDictionary.Add(keyValuePair);
		return keyValuePair;
	}

	private Mesh BakeVertexColors(Mesh _mesh, MeshRenderer _meshRenderer)
	{
		if (_mesh.vertexCount == 0)
		{
			return _mesh;
		}
		for (int i = 0; i < liMeshOriginals.Count; i++)
		{
			if (_mesh.vertexCount != liMeshOriginals[i].vertexCount || !(_mesh.vertices[0] == liMeshOriginals[i].vertices[0]) || !(_mesh.vertices[_mesh.vertexCount - 1] == liMeshOriginals[i].vertices[_mesh.vertexCount - 1]) || _meshRenderer.sharedMaterials.Length != liArOriginalMaterials[i].Length)
			{
				continue;
			}
			bSameMesh = true;
			for (int j = 0; j < _meshRenderer.sharedMaterials.Length; j++)
			{
				if (_meshRenderer.sharedMaterials[j] != liArOriginalMaterials[i][j])
				{
					bSameMesh = false;
				}
			}
			if (bSameMesh)
			{
				return liMeshBaked[i];
			}
		}
		meshBaked = new Mesh();
		liV3Vertecies.Clear();
		liV3Vertecies.AddRange(_mesh.vertices);
		liITriangles.Clear();
		liITriangles.AddRange(_mesh.triangles);
		liV3Normals.Clear();
		liV3Normals.AddRange(_mesh.normals);
		liVertColors.Clear();
		int num = _mesh.vertices.Length;
		for (int k = 0; k < num; k++)
		{
			liVertColors.Add(colUnassigned);
		}
		iTrisOfPrevSubMeshes = 0;
		for (int l = 0; l < _mesh.subMeshCount; l++)
		{
			if (_meshRenderer.sharedMaterials.Length > l)
			{
				colTarget = _meshRenderer.sharedMaterials[l].color;
			}
			int[] triangles = _mesh.GetTriangles(l);
			for (int m = 0; m < triangles.Length; m++)
			{
				iVert = triangles[m];
				if (liVertColors[iVert] == colUnassigned)
				{
					liVertColors[iVert] = colTarget;
				}
				else if (liVertColors[iVert].r != colTarget.r || liVertColors[iVert].g != colTarget.g || liVertColors[iVert].b != colTarget.b)
				{
					liV3Vertecies.Add(liV3Vertecies[iVert]);
					liV3Normals.Add(liV3Normals[iVert]);
					liVertColors.Add(colTarget);
					liITriangles[iTrisOfPrevSubMeshes + m] = liV3Vertecies.Count - 1;
				}
			}
			iTrisOfPrevSubMeshes += triangles.Length;
		}
		meshBaked.SetVertices(liV3Vertecies);
		meshBaked.SetColors(liVertColors);
		meshBaked.SetTriangles(liITriangles, 0);
		meshBaked.SetNormals(liV3Normals);
		meshBaked.subMeshCount = 1;
		liMeshOriginals.Add(_mesh);
		liMeshBaked.Add(meshBaked);
		liArOriginalMaterials.Add(_meshRenderer.sharedMaterials);
		return meshBaked;
	}

	private Mesh BakeVertexNightLights(Mesh _mesh, MeshRenderer _meshRenderer)
	{
		meshBaked = new Mesh();
		liV3Vertecies.Clear();
		List<Vector3> list = new List<Vector3>();
		list.AddRange(_mesh.vertices);
		liITriangles.Clear();
		List<int> list2 = new List<int>();
		list2.AddRange(_mesh.triangles);
		liV3Normals.Clear();
		List<Vector3> list3 = new List<Vector3>();
		list3.AddRange(_mesh.normals);
		for (int i = 0; i < _mesh.subMeshCount; i++)
		{
			float num = 0f;
			if (_meshRenderer.sharedMaterials.Length > i)
			{
				if (!shaderToBake.name.Equals(_meshRenderer.sharedMaterials[i].shader.name))
				{
					continue;
				}
				colTarget = _meshRenderer.sharedMaterials[i].color;
				num = _meshRenderer.sharedMaterials[i].GetFloat("_NightLum");
			}
			if (num == 1f)
			{
				SubMeshDescriptor subMesh = _mesh.GetSubMesh(i);
				for (int j = subMesh.indexStart; j < subMesh.indexStart + subMesh.indexCount; j++)
				{
					liV3Vertecies.Add(list[list2[j]]);
					liV3Normals.Add(list3[list2[j]]);
					liITriangles.Add(liITriangles.Count);
				}
			}
		}
		meshBaked.SetVertices(liV3Vertecies);
		meshBaked.SetTriangles(liITriangles, 0);
		meshBaked.SetNormals(liV3Normals);
		meshBaked.subMeshCount = 1;
		return meshBaked;
	}

	private Mesh BakeAllSubMeshes(Mesh _mesh)
	{
		meshBaked = new Mesh();
		List<Vector3> list = new List<Vector3>();
		list.AddRange(_mesh.vertices);
		List<int> list2 = new List<int>();
		list2.AddRange(_mesh.triangles);
		List<Vector3> list3 = new List<Vector3>();
		list3.AddRange(_mesh.normals);
		List<Color> list4 = new List<Color>();
		list4.AddRange(_mesh.colors);
		List<Vector2> list5 = new List<Vector2>();
		list5.AddRange(_mesh.uv);
		meshBaked.SetVertices(list);
		meshBaked.SetTriangles(list2, 0);
		meshBaked.SetNormals(list3);
		meshBaked.SetColors(list4);
		meshBaked.SetUVs(0, list5);
		meshBaked.subMeshCount = 1;
		return meshBaked;
	}

	private void MergeMeshes(Mesh _meshBase, Mesh _meshAdd, Transform _transMeshAdd, bool useLocals = false, bool supportColors = true)
	{
		List<Vector3> list;
		List<int> list2;
		List<Color> list3;
		List<Vector3> list4;
		List<Vector2> list5;
		if (useLocals)
		{
			list = new List<Vector3>(_meshBase.vertices);
			list2 = new List<int>(_meshBase.triangles);
			list3 = new List<Color>(_meshBase.colors);
			list4 = new List<Vector3>(_meshBase.normals);
			list5 = new List<Vector2>(_meshBase.uv);
		}
		else
		{
			list = liV3MergeVerts;
			list2 = liIMergeTris;
			list3 = liColorsMerge;
			list4 = liV3MergeNormals;
			list5 = new List<Vector2>(_meshBase.uv);
		}
		iBaseVertexCount = _meshBase.vertexCount;
		list.AddRange(_meshAdd.vertices);
		iForEndHere = list.Count;
		for (int i = iBaseVertexCount; i < iForEndHere; i++)
		{
			list[i] = _transMeshAdd.TransformPoint(list[i]);
		}
		_meshBase.SetVertices(list);
		iBaseTriangleCount = _meshBase.triangles.Length;
		list2.AddRange(_meshAdd.triangles);
		iForEndHere = list2.Count;
		for (int j = iBaseTriangleCount; j < iForEndHere; j++)
		{
			list2[j] += iBaseVertexCount;
		}
		_meshBase.SetTriangles(list2, 0);
		if (supportColors)
		{
			list3.AddRange(_meshAdd.colors);
			_meshBase.SetColors(list3);
		}
		matObjToWorldNormal = _transMeshAdd.localToWorldMatrix.inverse.transpose;
		list4.AddRange(_meshAdd.normals);
		iForEndHere = list4.Count;
		for (int k = iBaseVertexCount; k < iForEndHere; k++)
		{
			list4[k] = matObjToWorldNormal.MultiplyPoint3x4(list4[k]);
		}
		_meshBase.SetNormals(list4);
		list5.AddRange(_meshAdd.uv);
		_meshBase.SetUVs(0, list5);
	}

	public Building BuildingFindMouseOver()
	{
		Physics.Raycast(InputManager.Singleton.InputDataCurrent.rayPointerToWorld, out var hitInfo, 10000f, lmMouseOverBuildingDetection);
		if ((bool)hitInfo.collider)
		{
			Transform parent = hitInfo.collider.transform.parent;
			if ((bool)parent)
			{
				Building value = null;
				if (!m_BuildingCache.TryGetValue(parent, out value))
				{
					value = parent.GetComponent<Building>();
					m_BuildingCache.Add(parent, value);
				}
				return value;
			}
		}
		return null;
	}

	public void UnbakeBuilding(GameObject _go)
	{
		if (!dicBakedObjectData.ContainsKey(_go))
		{
			return;
		}
		BakedObjectData bakedObjectData = dicBakedObjectData[_go];
		Mesh sharedMesh = meshFilterMegaCombined.sharedMesh;
		liV3MergeNormals.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		liColorsMerge.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		liIMergeTris.RemoveRange(bakedObjectData.iTriangleIndex, bakedObjectData.iTriangleCount);
		int iVertCount = bakedObjectData.iVertCount;
		for (int i = bakedObjectData.iTriangleIndex; i < liIMergeTris.Count; i++)
		{
			liIMergeTris[i] -= iVertCount;
		}
		liV3MergeVerts.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		sharedMesh.SetTriangles(liIMergeTris.ToArray(), 0);
		sharedMesh.SetVertices(liV3MergeVerts);
		sharedMesh.SetNormals(liV3MergeNormals);
		sharedMesh.SetColors(liColorsMerge);
		meshFilterMegaCombined.sharedMesh = sharedMesh;
		dicBakedObjectData.Remove(_go);
		foreach (BakedObjectData value in dicBakedObjectData.Values)
		{
			if (value.iTriangleIndex > bakedObjectData.iTriangleIndex)
			{
				value.iTriangleIndex -= bakedObjectData.iTriangleCount;
			}
			if (value.iVertIndex > bakedObjectData.iVertIndex)
			{
				value.iVertIndex -= bakedObjectData.iVertCount;
			}
		}
		MeshFilter[] componentsInChildren = _go.GetComponentsInChildren<MeshFilter>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			UnbakeBuildingNL(componentsInChildren[j]);
			UnbakeSecondayMeshes(componentsInChildren[j]);
			UnbakeBuildingsMeshes(componentsInChildren[j]);
		}
	}

	private void UnbakeBuildingNL(MeshFilter meshFilter)
	{
		if (!dicNightBakedObjectData.ContainsKey(meshFilter))
		{
			return;
		}
		BakedObjectData bakedObjectData = dicNightBakedObjectData[meshFilter];
		int num = dicNightBakedObjectIndex[meshFilter];
		Mesh sharedMesh;
		switch (num)
		{
		case -2:
			return;
		case -1:
			sharedMesh = meshFilterMegaCombinedNightLights.sharedMesh;
			break;
		default:
			sharedMesh = multipleMeshFilterMCNightLights[num].sharedMesh;
			break;
		}
		List<Vector3> list = new List<Vector3>(sharedMesh.vertices);
		List<int> list2 = new List<int>(sharedMesh.triangles);
		new List<Color>(sharedMesh.colors);
		List<Vector3> list3 = new List<Vector3>(sharedMesh.normals);
		list3.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		list2.RemoveRange(bakedObjectData.iTriangleIndex, bakedObjectData.iTriangleCount);
		int iVertCount = bakedObjectData.iVertCount;
		for (int i = bakedObjectData.iTriangleIndex; i < list2.Count; i++)
		{
			list2[i] -= iVertCount;
		}
		list.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		sharedMesh.SetTriangles(list2.ToArray(), 0);
		sharedMesh.SetVertices(list);
		sharedMesh.SetNormals(list3);
		if (num == -1)
		{
			meshFilterMegaCombinedNightLights.sharedMesh = sharedMesh;
		}
		else
		{
			multipleMeshFilterMCNightLights[num].sharedMesh = sharedMesh;
		}
		dicNightBakedObjectData.Remove(meshFilter);
		dicNightBakedObjectIndex.Remove(meshFilter);
		foreach (MeshFilter key in dicNightBakedObjectData.Keys)
		{
			if (dicNightBakedObjectIndex[key] == num)
			{
				if (dicNightBakedObjectData[key].iTriangleIndex > bakedObjectData.iTriangleIndex)
				{
					dicNightBakedObjectData[key].iTriangleIndex -= bakedObjectData.iTriangleCount;
				}
				if (dicNightBakedObjectData[key].iVertIndex > bakedObjectData.iVertIndex)
				{
					dicNightBakedObjectData[key].iVertIndex -= bakedObjectData.iVertCount;
				}
			}
		}
	}

	private void UnbakeSecondayMeshes(MeshFilter meshFilter)
	{
		if (!dicSecondaryBakedObjectData.ContainsKey(meshFilter))
		{
			return;
		}
		BakedObjectData bakedObjectData = dicSecondaryBakedObjectData[meshFilter];
		int num = dicSecondaryBakedObjectIndex[meshFilter];
		if (num == -1)
		{
			return;
		}
		Mesh sharedMesh = meshFilterMegaSecondaryCombinedDictionary[num].Value.sharedMesh;
		List<Vector3> list = new List<Vector3>(sharedMesh.vertices);
		List<int> list2 = new List<int>(sharedMesh.triangles);
		List<Color> list3 = new List<Color>(sharedMesh.colors);
		List<Vector3> list4 = new List<Vector3>(sharedMesh.normals);
		List<Vector2> list5 = new List<Vector2>(sharedMesh.uv);
		if (list4.Count >= bakedObjectData.iVertIndex + bakedObjectData.iVertCount)
		{
			list4.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		}
		if (list3.Count >= bakedObjectData.iVertIndex + bakedObjectData.iVertCount)
		{
			list3.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		}
		if (list5.Count >= bakedObjectData.iVertIndex + bakedObjectData.iVertCount)
		{
			list5.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		}
		list2.RemoveRange(bakedObjectData.iTriangleIndex, bakedObjectData.iTriangleCount);
		int iVertCount = bakedObjectData.iVertCount;
		for (int i = bakedObjectData.iTriangleIndex; i < list2.Count; i++)
		{
			list2[i] -= iVertCount;
		}
		list.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		sharedMesh.SetTriangles(list2.ToArray(), 0);
		sharedMesh.SetVertices(list);
		sharedMesh.SetNormals(list4);
		sharedMesh.SetColors(list3);
		sharedMesh.SetUVs(0, list5);
		meshFilterMegaSecondaryCombinedDictionary[num].Value.sharedMesh = sharedMesh;
		dicSecondaryBakedObjectData.Remove(meshFilter);
		dicSecondaryBakedObjectIndex.Remove(meshFilter);
		foreach (MeshFilter key in dicSecondaryBakedObjectData.Keys)
		{
			if (dicSecondaryBakedObjectIndex[key] == num)
			{
				if (dicSecondaryBakedObjectData[key].iTriangleIndex > bakedObjectData.iTriangleIndex)
				{
					dicSecondaryBakedObjectData[key].iTriangleIndex -= bakedObjectData.iTriangleCount;
				}
				if (dicSecondaryBakedObjectData[key].iVertIndex > bakedObjectData.iVertIndex)
				{
					dicSecondaryBakedObjectData[key].iVertIndex -= bakedObjectData.iVertCount;
				}
			}
		}
	}

	private void UnbakeBuildingsMeshes(MeshFilter meshFilter)
	{
		if (!dicBuildingsBakedObjectData.ContainsKey(meshFilter))
		{
			return;
		}
		BakedObjectData bakedObjectData = dicBuildingsBakedObjectData[meshFilter];
		int num = dicBuildingsBakedObjectIndex[meshFilter];
		if (num == -1)
		{
			return;
		}
		Mesh sharedMesh = meshFilterMegaBuildingsCombinedList[num].sharedMesh;
		List<Vector3> list = new List<Vector3>(sharedMesh.vertices);
		List<int> list2 = new List<int>(sharedMesh.triangles);
		List<Color> list3 = new List<Color>(sharedMesh.colors);
		List<Vector3> list4 = new List<Vector3>(sharedMesh.normals);
		List<Vector2> list5 = new List<Vector2>(sharedMesh.uv);
		if (list4.Count >= bakedObjectData.iVertIndex + bakedObjectData.iVertCount)
		{
			list4.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		}
		if (list3.Count >= bakedObjectData.iVertIndex + bakedObjectData.iVertCount)
		{
			list3.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		}
		if (list5.Count >= bakedObjectData.iVertIndex + bakedObjectData.iVertCount)
		{
			list5.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		}
		list2.RemoveRange(bakedObjectData.iTriangleIndex, bakedObjectData.iTriangleCount);
		int iVertCount = bakedObjectData.iVertCount;
		for (int i = bakedObjectData.iTriangleIndex; i < list2.Count; i++)
		{
			list2[i] -= iVertCount;
		}
		list.RemoveRange(bakedObjectData.iVertIndex, bakedObjectData.iVertCount);
		sharedMesh.SetTriangles(list2.ToArray(), 0);
		sharedMesh.SetVertices(list);
		sharedMesh.SetNormals(list4);
		sharedMesh.SetColors(list3);
		sharedMesh.SetUVs(0, list5);
		meshFilterMegaBuildingsCombinedList[num].sharedMesh = sharedMesh;
		dicBuildingsBakedObjectData.Remove(meshFilter);
		dicBuildingsBakedObjectIndex.Remove(meshFilter);
		foreach (MeshFilter key in dicBuildingsBakedObjectData.Keys)
		{
			if (dicBuildingsBakedObjectIndex[key] == num)
			{
				if (dicBuildingsBakedObjectData[key].iTriangleIndex > bakedObjectData.iTriangleIndex)
				{
					dicBuildingsBakedObjectData[key].iTriangleIndex -= bakedObjectData.iTriangleCount;
				}
				if (dicBuildingsBakedObjectData[key].iVertIndex > bakedObjectData.iVertIndex)
				{
					dicBuildingsBakedObjectData[key].iVertIndex -= bakedObjectData.iVertCount;
				}
			}
		}
	}
}
