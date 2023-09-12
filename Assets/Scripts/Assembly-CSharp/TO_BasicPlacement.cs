using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TO_BasicPlacement : TerrainOperation
{
	public enum BoundsBehaviour
	{
		Pushback = 0,
		Destroy = 1,
		Ignore = 2
	}

	[SerializeField]
	private AssetDataDynamicContainer assetData;

	[SerializeField]
	private Transform transReferenceOverride;

	[SerializeField]
	private bool bAddIdentifier;

	[SerializeField]
	private string identifierToAdd;

	[SerializeField]
	public List<LookAtReferenceData> LookAtTransformReferences;

	[SerializeField]
	private bool bUseAbsoluteDistance;

	[SerializeField]
	private float fDistanceRange = 15f;

	[SerializeField]
	private float fYOffsetMin;

	[SerializeField]
	private float fYOffsetMax;

	[SerializeField]
	[Range(0f, 1f)]
	private float fRelativeDistanceRange = 1f;

	[SerializeField]
	public PositionHeatmap positionHeatmap;

	[SerializeField]
	[HideInInspector]
	private bool bUsingPositionHeatmap;

	[SerializeField]
	private float fDistanceToCenterMin;

	[SerializeField]
	private float fDistanceToCenterMax;

	[SerializeField]
	private float fRelativeDistanceMin;

	[SerializeField]
	private float fRelativeDistanceMax = 1f;

	[SerializeField]
	private int iObjectMinCount = 1;

	[SerializeField]
	private int iObjectMaxCount = 1;

	[SerializeField]
	private BoundsBehaviour eBoundsBehaviour;

	public override IEnumerator Execute(TerrainGenerator _terrainGenerator)
	{
		bExecuteDone = false;
		int iPassObjectCount = Random.Range(iObjectMinCount, iObjectMaxCount + 1);
		for (int i = 0; i < iPassObjectCount; i++)
		{
			AssetDataRandomized adrAsset = assetData.GetAssetDataRandomized();
			Transform transform = ((!transReferenceOverride) ? _terrainGenerator.TransTerrainParrent : transReferenceOverride);
			Vector3 position = transform.position;
			Quaternion identity = Quaternion.identity;
			Vector3 v3TargetPos = position;
			Quaternion qTargetRot = identity;
			if (bUsingPositionHeatmap)
			{
				positionHeatmap.UpdatePixelData();
				Vector2 randomPosition = positionHeatmap.GetRandomPosition();
				float num = ((!bUseAbsoluteDistance) ? (fRelativeDistanceRange * _terrainGenerator.FRadius) : fDistanceRange);
				v3TargetPos += new Vector3(randomPosition.x, 0f, randomPosition.y) * num;
			}
			else
			{
				Vector3 normalized = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
				float num2 = ((!bUseAbsoluteDistance) ? (Random.Range(fRelativeDistanceMin, fRelativeDistanceMax) * _terrainGenerator.FRadius) : Random.Range(fDistanceToCenterMin, fDistanceToCenterMax));
				v3TargetPos += normalized * num2;
			}
			qTargetRot *= adrAsset.goAsset.transform.rotation;
			qTargetRot *= adrAsset.qRotation;
			if (_terrainGenerator.BShouldYield())
			{
				_terrainGenerator.BeforeYield();
				yield return null;
				_terrainGenerator.AfterYield();
			}
			base.transform.position = v3TargetPos + Vector3.up * Random.Range(fYOffsetMin, fYOffsetMax);
			base.transform.rotation = qTargetRot;
			base.transform.localScale = adrAsset.v3Scale;
			MeshFilter component = adrAsset.goAsset.GetComponent<MeshFilter>();
			bool flag = true;
			if (eBoundsBehaviour != BoundsBehaviour.Ignore && !_terrainGenerator.BIsMeshInBounds(component.sharedMesh, base.transform, out var _v3Direction, out var _fDistance))
			{
				switch (eBoundsBehaviour)
				{
				case BoundsBehaviour.Destroy:
					flag = false;
					break;
				case BoundsBehaviour.Pushback:
					base.transform.position += _v3Direction * _fDistance;
					break;
				}
			}
			if (flag)
			{
				GameObject goAsset = adrAsset.goAsset;
				GameObject gameObject = Object.Instantiate(goAsset, base.transform.position, base.transform.rotation, _terrainGenerator.TransTerrainParrent);
				gameObject.transform.localScale = adrAsset.v3Scale;
				if (LookAtTransformReferences != null && LookAtTransformReferences.Count > 0)
				{
					int index = Random.Range(0, LookAtTransformReferences.Count);
					LookAtReferenceData lookAtReferenceData = LookAtTransformReferences[index];
					if (lookAtReferenceData.iUseIdentifier)
					{
						TO_HandlerLookAt tO_HandlerLookAt = gameObject.AddComponent<TO_HandlerLookAt>();
						tO_HandlerLookAt.identifier = lookAtReferenceData.identifierReference;
						tO_HandlerLookAt.offsetEuler = lookAtReferenceData.YRotationOffset;
						tO_HandlerLookAt.variationEuler = lookAtReferenceData.YRotationVariation;
					}
					else if (lookAtReferenceData.transformReference != null)
					{
						Vector3 position2 = lookAtReferenceData.transformReference.position;
						position2.y = base.transform.position.y;
						gameObject.transform.LookAt(position2);
						gameObject.transform.Rotate(Vector3.up * lookAtReferenceData.YRotationOffset);
						float num3 = Random.Range(0f, lookAtReferenceData.YRotationVariation) - lookAtReferenceData.YRotationVariation * 0.5f;
						gameObject.transform.Rotate(Vector3.up * num3);
					}
				}
				if (bAddIdentifier)
				{
					gameObject.AddComponent<TO_HandlerIdentifier>().SetIdentifier(identifierToAdd);
				}
				SpawnPositionOverride[] componentsInChildren = gameObject.GetComponentsInChildren<SpawnPositionOverride>();
				if ((float)componentsInChildren.Length > 0f)
				{
					SpawnPositionOverride spawnPositionOverride = componentsInChildren[Random.Range(0, componentsInChildren.Length)];
					base.transform.position = spawnPositionOverride.transform.position;
					base.transform.rotation = spawnPositionOverride.transform.rotation;
					base.transform.localScale = spawnPositionOverride.transform.localScale;
				}
				_terrainGenerator.SaveObjectToCurrentFileWhenFinish(goAsset, gameObject);
			}
			if (_terrainGenerator.BShouldYield())
			{
				_terrainGenerator.BeforeYield();
				yield return null;
				_terrainGenerator.AfterYield();
			}
		}
		bExecuteDone = true;
		yield return null;
	}
}
