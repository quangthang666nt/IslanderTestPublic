using System;

[Serializable]
public class AssetDataDynamicContainer
{
	public enum EType
	{
		AssetData = 0,
		AssetDataCollection = 1
	}

	public EType eType;

	public AssetData assetData;

	public AssetDataCollection assetDataCollection;

	public AssetDataRandomized GetAssetDataRandomized()
	{
		if (eType == EType.AssetData)
		{
			return assetData.GetAssetDataRandomized();
		}
		return assetDataCollection.GetAssetDataRandomized();
	}
}
