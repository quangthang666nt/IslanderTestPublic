using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Asset Data Collection", menuName = "CUSTOM/Asset Data Collection", order = 1)]
public class AssetDataCollection : ScriptableObject
{
	public List<AssetData> liAssetData = new List<AssetData>();

	public AssetDataRandomized GetAssetDataRandomized()
	{
		if (liAssetData.Count < 0)
		{
			return null;
		}
		int index = UnityEngine.Random.Range(0, liAssetData.Count);
		return liAssetData[index].GetAssetDataRandomized();
	}
}
