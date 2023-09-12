using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TO_DropAssets : TerrainOperation
{
	[Range(1f, 100000f)]
	[Tooltip("How many times an Object tries to find a elegible poisition.")]
	public int iFailsafeIterations = 10000;

	public int iMinAmount = 1;

	public int iMaxAmount = 1;

	public PositionHeatmap positionHeatmap;

	public AssetDataDynamicContainer assetDataDynamicContainer;

	public LayerMask lmPlacementSurface;

	public LayerMask lmForbiddenOverlaps;

	public float fMaxSlopeAngle = 5f;

	[Tooltip("If True, the object picks a position where all its Ground Checkers are grounded.")]
	public bool bUseGroundedChecker = true;

	[Tooltip("How many times the object gets pushed around in order to find an edge. Set to zero if edge pushing is not desired.")]
	public int iMaxIterations = 100;

	[Tooltip("The distance the object gets translated between two iteration steps.")]
	public float fIterationStepSize = 0.1f;

	[Tooltip("How big is the soft zone (in steps) towards overlap edges.")]
	public int iUseLastStepsRange_ForbiddenOverlap;

	[Tooltip("How big is the soft zone (in steps) towards cliff edges.")]
	public int iUseLastStepsRange_NotGrounded;

	public bool bUseHeatmap = true;

	[SerializeField]
	private float fRelativeDistanceMin;

	[SerializeField]
	private float fRelativeDistanceMax = 1f;

	public override IEnumerator Execute(TerrainGenerator _terrainGenerator)
	{
		bExecuteDone = false;
		Transform transTerrainParent = _terrainGenerator.TransTerrainParrent;
		if (bUseHeatmap)
		{
			positionHeatmap.UpdatePixelData();
		}
		if (_terrainGenerator.BShouldYield())
		{
			_terrainGenerator.BeforeYield();
			yield return null;
			_terrainGenerator.AfterYield();
		}
		int iFailsafe = 0;
		int iAmount = Random.Range(iMinAmount, iMaxAmount + 1);
		for (int iRep = 0; iRep < iAmount; iRep++)
		{
			if (_terrainGenerator.BShouldYield())
			{
				_terrainGenerator.BeforeYield();
				yield return null;
				_terrainGenerator.AfterYield();
			}
			AssetDataRandomized assetDataRandomized = assetDataDynamicContainer.GetAssetDataRandomized();
			_ = Vector2.zero;
			_ = Vector3.zero;
			Vector2 vector = ((!bUseHeatmap) ? ((Vector2)(Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * (_terrainGenerator.FRadius * Vector2.left * Random.Range(fRelativeDistanceMin, fRelativeDistanceMax)))) : (positionHeatmap.v2GetRandomPositionInRange() * _terrainGenerator.FRadius * fRelativeDistanceMax));
			Vector3 origin = new Vector3(vector.x, 1000000f, vector.y);
			origin += _terrainGenerator.TransTerrainParrent.position;
			RaycastHit hitInfo;
			bool num = Physics.Raycast(origin, Vector3.down, out hitInfo, float.MaxValue, lmPlacementSurface);
			bool flag = false;
			if (num)
			{
				flag = Physics.Raycast(origin, Vector3.down, hitInfo.distance - 0.01f, ~(int)lmPlacementSurface);
			}
			if (num && !flag)
			{
				Vector3 point = hitInfo.point;
				if (Vector3.Angle(hitInfo.normal, Vector3.up) > fMaxSlopeAngle)
				{
					iRep--;
					iFailsafe++;
					if (iFailsafe > iFailsafeIterations)
					{
						break;
					}
					continue;
				}
				GameObject goAsset = assetDataRandomized.goAsset;
				GameObject gameObject = Object.Instantiate(goAsset, point, assetDataRandomized.qRotation);
				gameObject.transform.localScale = assetDataRandomized.v3Scale;
				gameObject.transform.SetParent(transTerrainParent);
				GroundedChecker[] componentsInChildren = gameObject.GetComponentsInChildren<GroundedChecker>();
				if (!BGrounded(componentsInChildren))
				{
					gameObject.transform.position = new Vector3(7777f, -10000f, -8888f);
					if (Application.isEditor)
					{
						Object.DestroyImmediate(gameObject);
					}
					else
					{
						Object.Destroy(gameObject);
					}
					iRep--;
					iFailsafe++;
					if (iFailsafe > iFailsafeIterations)
					{
						break;
					}
					continue;
				}
				Transform transform = gameObject.transform;
				_ = gameObject.layer;
				BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
				if (!boxCollider)
				{
					boxCollider = gameObject.GetComponentInChildren<BoxCollider>();
				}
				if (BOverlappingWithForbidden(boxCollider))
				{
					gameObject.transform.position = new Vector3(7777f, -10000f, -8888f);
					if (Application.isEditor)
					{
						Object.DestroyImmediate(gameObject);
					}
					else
					{
						Object.Destroy(gameObject);
					}
					iRep--;
					iFailsafe++;
					if (iFailsafe > iFailsafeIterations)
					{
						break;
					}
					continue;
				}
				MeshCollider component = gameObject.GetComponent<MeshCollider>();
				if (!boxCollider && (bool)component && BOverlappingWithForbidden(component.bounds))
				{
					gameObject.transform.position = new Vector3(7777f, -10000f, -8888f);
					if (Application.isEditor)
					{
						Object.DestroyImmediate(gameObject);
					}
					else
					{
						Object.Destroy(gameObject);
					}
					iRep--;
					iFailsafe++;
					if (iFailsafe > iFailsafeIterations)
					{
						break;
					}
					continue;
				}
				List<Vector3> list = new List<Vector3>();
				list.Add(transform.position);
				Vector3 vector2 = Quaternion.Euler(0f, Random.value * 360f, 0f) * new Vector3(fIterationStepSize, 0f, 0f);
				for (int i = 0; i < iMaxIterations; i++)
				{
					transform.position += vector2;
					if (!BGrounded(componentsInChildren))
					{
						int index = Mathf.Clamp(list.Count - Random.Range(0, iUseLastStepsRange_NotGrounded), 0, list.Count - 1);
						transform.position = list[index];
						break;
					}
					if (BOverlappingWithForbidden(boxCollider))
					{
						int index2 = Mathf.Clamp(list.Count - Random.Range(0, iUseLastStepsRange_ForbiddenOverlap), 0, list.Count - 1);
						transform.position = list[index2];
						break;
					}
					list.Add(transform.position);
				}
				SaveLoadManager.SaveIslandObjectToCurrentSaveFile(goAsset, gameObject.transform);
			}
			else
			{
				iRep--;
				iFailsafe++;
				if (iFailsafe > iFailsafeIterations)
				{
					break;
				}
			}
		}
		bExecuteDone = true;
		yield return null;
	}

	public bool BOverlappingWithForbidden(BoxCollider _bc)
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
