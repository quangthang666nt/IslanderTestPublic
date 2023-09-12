using System.Collections;
using UnityEngine;

public class TO_NarrowWideAssetDrop : TerrainOperation
{
	public enum EFindMode
	{
		FindWide = 0,
		FindNarrow = 1
	}

	public int iAmount = 1;

	public AssetDataDynamicContainer assetDataDynamicContainer;

	public LayerMask lmPlacementSurface;

	[Tooltip("If the ray from the top hits this layer first, objects won't be placed here.")]
	public LayerMask lmPlacementSurfaceRayBreakers;

	public LayerMask lmForbiddenOverlaps;

	public LayerMask lmHeightRaycasts;

	public float fMaxSlopeAngle = 5f;

	public bool bUseGroundedChecker = true;

	public PositionHeatmap positionHeatmap;

	[Range(1f, 1000f)]
	[Tooltip("How many times an Object tries to find a elegible poisition.")]
	public int iIterations = 50;

	public int iSurroundingRaycasts = 8;

	public float fSurroundingRaycastDistance = 1f;

	[Range(0f, 10f)]
	[Tooltip("Height differences larger than this don't make a difference.")]
	public float fMaxVerticalOffsetForScore = 1f;

	public EFindMode eFindMode = EFindMode.FindNarrow;

	public override IEnumerator Execute(TerrainGenerator _terrainGenerator)
	{
		bExecuteDone = false;
		Transform transTerrainParent = _terrainGenerator.TransTerrainParrent;
		positionHeatmap.UpdatePixelData();
		for (int iNr = 0; iNr < iAmount; iNr++)
		{
			AssetDataRandomized assetDataRandomized = assetDataDynamicContainer.GetAssetDataRandomized();
			Vector3 v3BestSolution = Vector3.zero;
			float fBestSolutionScore = 0f;
			GameObject goCreatePrefab = assetDataRandomized.goAsset;
			GameObject goCreated = Object.Instantiate(goCreatePrefab, Vector3.zero, assetDataRandomized.qRotation);
			goCreated.transform.localScale = assetDataRandomized.v3Scale;
			goCreated.transform.SetParent(transTerrainParent);
			for (int iIt = 0; iIt < iIterations; iIt++)
			{
				if (_terrainGenerator.BShouldYield())
				{
					_terrainGenerator.BeforeYield();
					yield return null;
					_terrainGenerator.AfterYield();
				}
				Vector2 vector = positionHeatmap.v2GetRandomPositionInRange() * _terrainGenerator.FRadius;
				Vector3 v3OriginPos = new Vector3(vector.x, 1000f, vector.y);
				v3OriginPos += _terrainGenerator.TransTerrainParrent.position;
				if (!Physics.Raycast(v3OriginPos, Vector3.down, out var hitInfo, float.MaxValue, lmPlacementSurface))
				{
					continue;
				}
				Vector3 v3GroundPos = hitInfo.point;
				if (Physics.Raycast(v3OriginPos, Vector3.down, out hitInfo, float.MaxValue, lmPlacementSurfaceRayBreakers) || Vector3.Angle(hitInfo.normal, Vector3.up) > fMaxSlopeAngle)
				{
					continue;
				}
				goCreated.transform.position = v3GroundPos;
				GroundedChecker[] componentsInChildren = goCreated.GetComponentsInChildren<GroundedChecker>();
				if (!BGrounded(componentsInChildren))
				{
					continue;
				}
				_ = goCreated.transform;
				int layer = goCreated.layer;
				BoxCollider boxCollider = goCreated.GetComponent<BoxCollider>();
				if (!boxCollider)
				{
					boxCollider = goCreated.GetComponentInChildren<BoxCollider>();
				}
				if (BOverlappingWithForbidden(boxCollider, layer))
				{
					continue;
				}
				MeshCollider component = goCreated.GetComponent<MeshCollider>();
				if (!boxCollider && (bool)component && BOverlappingWithForbidden(component.bounds))
				{
					continue;
				}
				if (_terrainGenerator.BShouldYield())
				{
					_terrainGenerator.BeforeYield();
					yield return null;
					_terrainGenerator.AfterYield();
				}
				float num = 0f;
				for (int i = 0; i < iSurroundingRaycasts; i++)
				{
					Vector3 origin = v3OriginPos + Quaternion.Euler(0f, (float)i * (360f / (float)iSurroundingRaycasts), 0f) * new Vector3(fSurroundingRaycastDistance, 0f, 0f);
					float num2 = fMaxVerticalOffsetForScore;
					if (Physics.Raycast(origin, Vector3.down, out hitInfo, float.MaxValue, lmHeightRaycasts))
					{
						num2 = Mathf.Clamp(Mathf.Abs(v3GroundPos.y - hitInfo.point.y), 0f, fMaxVerticalOffsetForScore);
					}
					num = ((eFindMode != 0) ? (num + num2) : (num + (fMaxVerticalOffsetForScore - num2)));
				}
				if (num > fBestSolutionScore)
				{
					fBestSolutionScore = num;
					v3BestSolution = v3GroundPos;
				}
			}
			if (fBestSolutionScore > 0f)
			{
				goCreated.transform.position = v3BestSolution;
				SaveLoadManager.SaveIslandObjectToCurrentSaveFile(goCreatePrefab, goCreated.transform);
			}
			else
			{
				Object.Destroy(goCreated);
			}
		}
		bExecuteDone = true;
		yield return null;
	}

	public bool BOverlappingWithForbidden(BoxCollider _bc, int _iObjectLayer)
	{
		if (!_bc)
		{
			return false;
		}
		Vector3 _v3OutPosition = default(Vector3);
		Vector3 _v3OutScale = default(Vector3);
		Quaternion _qOutRotation = default(Quaternion);
		TransformTools.BoxColliderToWorldSpace(_bc, ref _v3OutPosition, ref _v3OutScale, ref _qOutRotation);
		Vector3 position = _bc.transform.position;
		_bc.transform.position = new Vector3(1000000f, 1000000f, 1000000f);
		if (Physics.OverlapBox(_v3OutPosition, _v3OutScale * 0.5f, _qOutRotation, lmForbiddenOverlaps).Length != 0)
		{
			_bc.transform.position = position;
			return true;
		}
		_bc.transform.position = position;
		return false;
	}

	public bool BOverlappingWithForbidden(Bounds _bounds)
	{
		if (Physics.OverlapBox(_bounds.center, _bounds.size / 2f, Quaternion.identity, lmForbiddenOverlaps).Length != 0)
		{
			return true;
		}
		return false;
	}

	public bool BGrounded(GroundedChecker[] _arGroundedChecker)
	{
		if (!bUseGroundedChecker)
		{
			return true;
		}
		bool result = true;
		for (int i = 0; i < _arGroundedChecker.Length; i++)
		{
			if (!_arGroundedChecker[i].BCheckGrounded(lmPlacementSurface))
			{
				result = false;
				break;
			}
		}
		return result;
	}
}
