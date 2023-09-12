using System;
using UnityEngine;

[Serializable]
public class AssetData
{
	public GameObject goAsset;

	public GoList goList;

	public float fTotalScaleMultiplyerMin = 1f;

	public float fTotalScaleMultiplyerMax = 1f;

	public float fScaleMinX = 1f;

	public float fScaleMinY = 1f;

	public float fScaleMinZ = 1f;

	public float fScaleMaxX = 1f;

	public float fScaleMaxY = 1f;

	public float fScaleMaxZ = 1f;

	public float fRotationDefaultX;

	public float fRotationDefaultY;

	public float fRotationDefaultZ;

	public float fRotationSpreadX;

	public float fRotationSpreadY = 360f;

	public float fRotationSpreadZ;

	public float fSteppedYRotation;

	public bool bEditorFoldout = true;

	public AssetDataRandomized GetAssetDataRandomized()
	{
		AssetDataRandomized assetDataRandomized = new AssetDataRandomized();
		if ((bool)goAsset)
		{
			assetDataRandomized.goAsset = goAsset;
		}
		else
		{
			assetDataRandomized.goAsset = goList.GoGetRandom();
		}
		float x = Mathf.Lerp(fScaleMinX, fScaleMaxX, UnityEngine.Random.value);
		float y = Mathf.Lerp(fScaleMinY, fScaleMaxY, UnityEngine.Random.value);
		float z = Mathf.Lerp(fScaleMinZ, fScaleMaxZ, UnityEngine.Random.value);
		assetDataRandomized.v3Scale = new Vector3(x, y, z) * Mathf.Lerp(fTotalScaleMultiplyerMin, fTotalScaleMultiplyerMax, UnityEngine.Random.value);
		float x2 = fRotationDefaultX + fRotationSpreadX * (UnityEngine.Random.value - 0.5f);
		float num = fRotationDefaultY + fRotationSpreadY * (UnityEngine.Random.value - 0.5f);
		float z2 = fRotationDefaultZ + fRotationSpreadZ * (UnityEngine.Random.value - 0.5f);
		if (fSteppedYRotation > 0f)
		{
			num = Mathf.Round(num / fSteppedYRotation) * fSteppedYRotation;
		}
		assetDataRandomized.v3Rotation = new Vector3(x2, num, z2);
		return assetDataRandomized;
	}
}
